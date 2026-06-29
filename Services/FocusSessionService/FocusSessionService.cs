using AI_genda_API.Abstractions.Enums;
using AI_genda_API.Contracts.FocusSession;

namespace AI_genda_API.Services.FocusSessionService;

public class FocusSessionService(AppContext context) : IFocusSessionService
{
    private readonly AppContext _Context = context;
    private const string DailyGoalLabel = "DAILY GOAL: 4 FOCUS HOURS";

    public async Task<Result<FocusSessionActiveResponse>> StartAsync(int WorkspaceId, string SpaceId, string TaskId, string UserId, FocusSessionStartRequest request, CancellationToken cancellationToken = default!)
    {


        if (!await HasAccessAsync(WorkspaceId, UserId, cancellationToken))
            return Result.Faluire<FocusSessionActiveResponse>(WorkspaceMemberErrors.AccessDenied);

        var taskSnapshot = await GetTaskSnapshotAsync(WorkspaceId, SpaceId, TaskId, cancellationToken);
        if (taskSnapshot is null)
            return Result.Faluire<FocusSessionActiveResponse>(TaskErrors.TaskNotFound);




        var hasCurrent = await _Context.Set<FocusSession>()
            .AnyAsync(x => x.WorkspaceId == WorkspaceId
                        && x.SpaceId == SpaceId
                        && x.TaskId == TaskId
                        && x.UserId == UserId
                        && (x.Status == FocusSessionStatus.Active || x.Status == FocusSessionStatus.Paused), cancellationToken);

        if (hasCurrent)
            return Result.Faluire<FocusSessionActiveResponse>(FocusSessionErrors.ActiveSessionAlreadyExists);

        var completed = taskSnapshot.Value.Subtasks.Count(x => x.IsCompleted);

        var total = taskSnapshot.Value.Subtasks.Count;

        var session = new FocusSession
        {
            WorkspaceId = WorkspaceId,
            SpaceId = SpaceId,
            TaskId = TaskId,
            UserId = UserId,
            TaskName = taskSnapshot.Value.TaskName,
            TeamName = taskSnapshot.Value.TeamName,
            DurationMinutes = request.DurationMinutes,
            AmbientSound = request.AmbientSound,
            BreakAfter = request.BreakAfter,
            BlockNotifications = request.BlockNotifications,
            StartedAt = DateTime.UtcNow,
            Status = FocusSessionStatus.Active,
            InitialCompletedSubtasks = completed,
            InitialTotalSubtasks = total,
            CompletedSubtasks = completed,
            TotalSubtasks = total
        };

        await _Context.Set<FocusSession>().AddAsync(session, cancellationToken);
        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success(ToActiveResponse(session, taskSnapshot.Value.TaskDescription, taskSnapshot.Value.Subtasks));
    }

    public async Task<Result<FocusSessionActiveResponse>> GetCurrentAsync(int WorkspaceId, string SpaceId, string TaskId, string UserId, CancellationToken cancellationToken = default!)
    {
        if (!await HasAccessAsync(WorkspaceId, UserId, cancellationToken))
            return Result.Faluire<FocusSessionActiveResponse>(WorkspaceMemberErrors.AccessDenied);

        var session = await _Context.Set<FocusSession>()
            .Where(x => x.WorkspaceId == WorkspaceId
                     && x.SpaceId == SpaceId
                     && x.TaskId == TaskId
                     && x.UserId == UserId
                     && (x.Status == FocusSessionStatus.Active || x.Status == FocusSessionStatus.Paused))
            .OrderByDescending(x => x.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (session is null)
            return Result.Faluire<FocusSessionActiveResponse>(FocusSessionErrors.NoCurrentSession);

        var taskSnapshot = await GetTaskSnapshotAsync(WorkspaceId, SpaceId, TaskId, cancellationToken);
        if (taskSnapshot is null)
            return Result.Faluire<FocusSessionActiveResponse>(TaskErrors.TaskNotFound);

        session.CompletedSubtasks = taskSnapshot.Value.Subtasks.Count(x => x.IsCompleted);

        session.TotalSubtasks = taskSnapshot.Value.Subtasks.Count;

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success(ToActiveResponse(session, taskSnapshot.Value.TaskDescription, taskSnapshot.Value.Subtasks));
    }

    public async Task<Result<FocusSessionPauseResponse>> PauseAsync(int WorkspaceId, string SpaceId, string TaskId, string SessionId, string UserId, CancellationToken cancellationToken = default!)
    {
        if (!await HasAccessAsync(WorkspaceId, UserId, cancellationToken))
            return Result.Faluire<FocusSessionPauseResponse>(WorkspaceMemberErrors.AccessDenied);

        var session = await GetSessionAsync(WorkspaceId, SpaceId, TaskId, SessionId, UserId, cancellationToken);
        if (session is null)
            return Result.Faluire<FocusSessionPauseResponse>(FocusSessionErrors.SessionNotFound);

        if (session.Status != FocusSessionStatus.Active)
            return Result.Faluire<FocusSessionPauseResponse>(FocusSessionErrors.SessionNotActive);

        session.Status = FocusSessionStatus.Paused;
        session.PausedAt = DateTime.UtcNow;
        session.Interruptions += 1;

        await _Context.SaveChangesAsync(cancellationToken);

        var (progress, remaining) = CalculateProgress(session, DateTime.UtcNow);

        return Result.Success(new FocusSessionPauseResponse(session.Id, progress, remaining, DailyGoalLabel));
    }

    public async Task<Result<FocusSessionActiveResponse>> ResumeAsync(int WorkspaceId, string SpaceId, string TaskId, string SessionId, string UserId, CancellationToken cancellationToken = default!)
    {
        if (!await HasAccessAsync(WorkspaceId, UserId, cancellationToken))
            return Result.Faluire<FocusSessionActiveResponse>(WorkspaceMemberErrors.AccessDenied);

        var session = await  GetSessionAsync(WorkspaceId, SpaceId, TaskId, SessionId, UserId, cancellationToken);
        if (session is null)
            return Result.Faluire<FocusSessionActiveResponse>(FocusSessionErrors.SessionNotFound);

        if (session.Status != FocusSessionStatus.Paused)
            return Result.Faluire<FocusSessionActiveResponse>(FocusSessionErrors.SessionNotPaused);

        if (session.PausedAt.HasValue)
        {
            session.TotalPausedSeconds += Math.Max(0, (int)(DateTime.UtcNow - session.PausedAt.Value).TotalSeconds);
            session.PausedAt = null;
        }

        session.Status = FocusSessionStatus.Active;

        var taskSnapshot = await GetTaskSnapshotAsync(WorkspaceId, SpaceId, TaskId, cancellationToken);
        if (taskSnapshot is null)
            return Result.Faluire<FocusSessionActiveResponse>(TaskErrors.TaskNotFound);

        session.CompletedSubtasks = taskSnapshot.Value.Subtasks.Count(x => x.IsCompleted);
        session.TotalSubtasks = taskSnapshot.Value.Subtasks.Count;

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success(ToActiveResponse(session, taskSnapshot.Value.TaskDescription, taskSnapshot.Value.Subtasks));
    }

    public async Task<Result<FocusSessionCompletedResponse>> CompleteAsync(int WorkspaceId, string SpaceId, string TaskId, string SessionId, string UserId, CancellationToken cancellationToken = default!)
    {
        if (!await HasAccessAsync(WorkspaceId, UserId, cancellationToken))
            return Result.Faluire<FocusSessionCompletedResponse>(WorkspaceMemberErrors.AccessDenied);

        var session = await GetSessionAsync(WorkspaceId, SpaceId, TaskId, SessionId, UserId, cancellationToken);
        if (session is null)
            return Result.Faluire<FocusSessionCompletedResponse>(FocusSessionErrors.SessionNotFound);

        if (session.Status is FocusSessionStatus.Completed or FocusSessionStatus.Abandoned)
            return Result.Faluire<FocusSessionCompletedResponse>(FocusSessionErrors.SessionClosed);

        var taskSnapshot = await GetTaskSnapshotAsync(WorkspaceId, SpaceId, TaskId, cancellationToken);
        if (taskSnapshot is null)
            return Result.Faluire<FocusSessionCompletedResponse>(TaskErrors.TaskNotFound);

        if (session.Status == FocusSessionStatus.Paused && session.PausedAt.HasValue)
        {
            session.TotalPausedSeconds += Math.Max(0, (int)(DateTime.UtcNow - session.PausedAt.Value).TotalSeconds);
            session.PausedAt = null;
        }

        session.CompletedSubtasks = taskSnapshot.Value. Subtasks.Count(x => x.IsCompleted);
        session.TotalSubtasks = taskSnapshot.Value.Subtasks.Count;
        session.Status = FocusSessionStatus.Completed;
        session.EndedAt = DateTime.UtcNow;

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success(BuildCompletedSummary(session));
    }

    public async Task<Result> AbandonAsync(int WorkspaceId, string SpaceId, string TaskId, string SessionId, string UserId, CancellationToken cancellationToken = default!)
    {
        if (!await HasAccessAsync(WorkspaceId, UserId, cancellationToken))
            return Result.Faluire(WorkspaceMemberErrors.AccessDenied);

        var session = await GetSessionAsync(WorkspaceId, SpaceId, TaskId, SessionId, UserId, cancellationToken);
        if (session is null)
            return Result.Faluire(FocusSessionErrors.SessionNotFound);

        if (session.Status is FocusSessionStatus.Completed or FocusSessionStatus.Abandoned)
            return Result.Faluire(FocusSessionErrors.SessionClosed);

        if (session.Status == FocusSessionStatus.Paused && session.PausedAt.HasValue)
        {
            session.TotalPausedSeconds += Math.Max(0, (int)(DateTime.UtcNow - session.PausedAt.Value).TotalSeconds);
            session.PausedAt = null;
        }

        session.Status = FocusSessionStatus.Abandoned;
        session.EndedAt = DateTime.UtcNow;

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<FocusSessionActiveResponse>> ToggleSubTaskAsync(int WorkspaceId, string SpaceId, string TaskId, string SessionId, string SubTaskId, string UserId, ToggleFocusSessionSubTaskRequest request, CancellationToken cancellationToken = default!)
    {
        if (!await HasAccessAsync(WorkspaceId, UserId, cancellationToken))
            return Result.Faluire<FocusSessionActiveResponse>(WorkspaceMemberErrors.AccessDenied);

        var session = await GetSessionAsync(WorkspaceId, SpaceId, TaskId, SessionId, UserId, cancellationToken);
        if (session is null)
            return Result.Faluire<FocusSessionActiveResponse>(FocusSessionErrors.SessionNotFound);

        if (session.Status is FocusSessionStatus.Completed or FocusSessionStatus.Abandoned)
            return Result.Faluire<FocusSessionActiveResponse>(FocusSessionErrors.SessionClosed);

        var subTask = await _Context.SubTasks
            .SingleOrDefaultAsync(st => st.Id == SubTaskId && st.TaskId == TaskId && st.RemovedAt == null, cancellationToken);

        if (subTask is null)
            return Result.Faluire<FocusSessionActiveResponse>(SubTaskErrors.SubTaskNotFound);

        subTask.IsCompleted = request.IsCompleted;
        await _Context.SaveChangesAsync(cancellationToken);

        var taskSnapshot = await GetTaskSnapshotAsync(WorkspaceId, SpaceId, TaskId, cancellationToken);
        if (taskSnapshot is null)
            return Result.Faluire<FocusSessionActiveResponse>(TaskErrors.TaskNotFound);

        session.CompletedSubtasks = taskSnapshot.Value.Subtasks.Count(x => x.IsCompleted);
        session.TotalSubtasks = taskSnapshot.Value.Subtasks.Count;

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success(ToActiveResponse(session, taskSnapshot.Value.TaskDescription, taskSnapshot.Value.Subtasks));
    }

    private async Task<FocusSession?> GetSessionAsync(int workspaceId, string spaceId, string taskId, string sessionId, string userId, CancellationToken cancellationToken)
    {
        return await _Context.Set<FocusSession>().SingleOrDefaultAsync(x =>
            x.Id == sessionId &&
            x.WorkspaceId == workspaceId &&
            x.SpaceId == spaceId &&
            x.TaskId == taskId &&
            x.UserId == userId, cancellationToken);
    }

    private async Task<(string TaskName, string? TaskDescription, string TeamName, List<FocusSessionSubTaskResponse> Subtasks)?> GetTaskSnapshotAsync(
        int workspaceId, string spaceId, string taskId, CancellationToken cancellationToken)
    {
        var taskData = await _Context.Tasks
            .Where(t => t.Id == taskId && t.SpaceId == spaceId && t.RemovedAt == null && t.Space.WorkSpaceId == workspaceId)
            .Select(t => new
            {
                t.Title,
                t.Description,
                TeamName = t.Space.WorkSpace.Name,
                Subtasks = t.SubTasks
                    .Where(st => st.RemovedAt == null)
                    .Select(st => new FocusSessionSubTaskResponse(st.Id, st.Title, st.IsCompleted))
                    .ToList()
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (taskData is null)
            return null;

        return (taskData.Title, taskData.Description, taskData.TeamName, taskData.Subtasks);
    }

    private static FocusSessionActiveResponse ToActiveResponse(FocusSession session, string? taskDescription, List<FocusSessionSubTaskResponse> subtasks)
    {
        var completed = subtasks.Count(x => x.IsCompleted);
        var total = subtasks.Count;
        var progress = total == 0 ? 0 : Math.Round((completed / (double)total) * 100, 2);

        return new FocusSessionActiveResponse(
            session.Id,
            session.TaskName,
            session.TeamName,
            taskDescription,
            session.DurationMinutes,
            session.AmbientSound,
            session.BreakAfter,
            session.BlockNotifications,
            session.StartedAt,
            session.EndedAt,
            session.Status.ToString(),
            session.Interruptions,
            completed,
            total,
            progress,
            $"{completed} of {total} subtasks",
            subtasks
        );
    }

    private static FocusSessionCompletedResponse BuildCompletedSummary(FocusSession session)
    {
        var end = session.EndedAt ?? DateTime.UtcNow;
        var elapsed = GetEffectiveElapsed(session, end);
        var focusMinutes = Math.Max(0, (int)Math.Round(elapsed.TotalMinutes));

        var total = session.TotalSubtasks <= 0 ? 0 : session.TotalSubtasks;
        var before = total == 0 ? 0 : (session.InitialCompletedSubtasks / (double)total) * 100;
        var after = total == 0 ? 0 : (session.CompletedSubtasks / (double)total) * 100;
        var deltaPercent = (int)Math.Round(after - before);

        var completionBonus = total == 0 ? 0 : (session.CompletedSubtasks / (double)total) * 20;
        var scoreRaw = 100 - (session.Interruptions * 10) + completionBonus;
        var score = (int)Math.Round(Math.Clamp(scoreRaw, 0, 100));

        var label = score switch
        {
            >= 90 => "Excellent",
            >= 70 => "Good",
            _ => "Fair"
        };

        const string insight = "You perform better when you complete subtasks early in the session. Keep that pattern.";

        return new FocusSessionCompletedResponse(
            session.Id,
            focusMinutes,
            deltaPercent,
            session.Interruptions,
            score,
            label,
            insight
        );
    }

    private static (double ProgressPercent, string TimeRemaining) CalculateProgress(FocusSession session, DateTime nowUtc)
    {
        var elapsed = GetEffectiveElapsed(session, nowUtc);
        var total = TimeSpan.FromMinutes(session.DurationMinutes);

        if (elapsed < TimeSpan.Zero) elapsed = TimeSpan.Zero;
        if (elapsed > total) elapsed = total;

        var remaining = total - elapsed;
        if (remaining < TimeSpan.Zero) remaining = TimeSpan.Zero;

        var progress = total.TotalSeconds == 0 ? 0 : (elapsed.TotalSeconds / total.TotalSeconds) * 100;
        var remainingText = $"{(int)remaining.TotalMinutes:D2}:{remaining.Seconds:D2}";

        return (Math.Round(Math.Clamp(progress, 0, 100), 2), remainingText);
    }

    private static TimeSpan GetEffectiveElapsed(FocusSession session, DateTime nowUtc)
    {
        var cutoff = (session.Status == FocusSessionStatus.Paused && session.PausedAt.HasValue) ? session.PausedAt.Value : nowUtc;
        var elapsed = cutoff - session.StartedAt - TimeSpan.FromSeconds(session.TotalPausedSeconds);
        return elapsed < TimeSpan.Zero ? TimeSpan.Zero : elapsed;
    }

    private async Task<bool> HasAccessAsync(int WorkspaceId, string UserId, CancellationToken cancellationToken)
    {
        return await _Context.WorkSpaces.AnyAsync(w => w.Id == WorkspaceId && w.CreatedById == UserId, cancellationToken)
            || await _Context.WorkspaceMembers.AnyAsync(m => m.WrokSpaceID == WorkspaceId && m.UserID == UserId, cancellationToken);
    }
}