using AI_genda_API.Abstractions.Enums;
using Task = System.Threading.Tasks.Task;

namespace AI_genda_API.Services.FolderService;

public class WorkSpaceService(AppContext context, IHttpContextAccessor httpContextAccessor, IEmailSender emailSender) : IWorkSpaceService
{
    private readonly AppContext _Context = context;
    private readonly IHttpContextAccessor _HttpContextAccessor = httpContextAccessor;
    private readonly IEmailSender _EmailSender = emailSender;

    public async Task<Result<WorkSpaceResponse>> AddAsync(string UserId, WorkSpaceRequest Requset, CancellationToken cancellationToken = default!)
    {

        var NumofworkSpaces = _Context.WorkSpaces
             .Where(x => x.CreatedById == UserId && x.RemovedAt == null)
             .Count();

        var user = await _Context.Users.Where(x => x.Id == UserId).SingleOrDefaultAsync(cancellationToken);

        if (NumofworkSpaces >= 5 && user!.SubscriptionType == (int)UserType.Free)
            return Result.Faluire<WorkSpaceResponse>(UserErrors.SubscriptionModel);

        var WorkSpace = Requset.Adapt<WorkSpaceRequest, WorkSpace>();

        await _Context.WorkSpaces.AddAsync(WorkSpace, cancellationToken);

        await _Context.SaveChangesAsync(cancellationToken);

        var WorkSpaceMember = new WorkspaceMember
        {
            UserID = UserId,
            WrokSpaceID = WorkSpace.Id,
            IsOwner = true,
            JoinedAt = DateTime.UtcNow,
            Permissions = OwnerPermissions.GetAllPerimision
        };

        await _Context.WorkspaceMembers.AddAsync(WorkSpaceMember, cancellationToken);

        await _Context.SaveChangesAsync(cancellationToken);

        var response = WorkSpace.Adapt<WorkSpace, WorkSpaceResponse>();

        return Result.Success(response);
    }

    public async Task<Result<PaginatedList<WorkSpaceResponse>?>> GetAllAsync(FilterRequest request, CancellationToken cancellationToken = default!)
    {


        var UserId = _HttpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var query = _Context.WorkSpaces
            .Where(ws => ws.RemovedAt == null && ws.workspaceMembers.Any(m => m.UserID == UserId && m.WrokSpaceID == ws.Id));


        if (!string.IsNullOrEmpty(request.SearchValue))
        {
            var Tokens = request.SearchValue.Tokenize().ToList();

            query = query.Where(x => Tokens.Any(token => x.Name.Contains(token) || x.Description!.Contains(token)));
        }


        if (!string.IsNullOrEmpty(request.SortColumn))
        {
            query = query.OrderBy($"{request.SortColumn} {request.SortOrder}");
        }


        var workSpaceResponses = query
          .Select(ws => new WorkSpaceResponse(
              ws.Id,
              ws.Name,
              ws.Description!,
              ws.IconCode!,
              ws.Visibility,
              ws.workspaceMembers.Count(m => m.WrokSpaceID == ws.Id),
              ws.Spaces
                  .Where(s => s.IsActive && s.RemovedAt == null)
                  .SelectMany(s => s.Tasks.Where(t => t.IsActive && t.RemovedAt == null))
                  .Count(),
              ws.CreatedById == UserId
          ))
          .AsNoTracking();

        var Response = await PaginatedList<WorkSpaceResponse>.CreateAsync(workSpaceResponses, request.PageNumber, request.PageSize);


        return Result.Success<PaginatedList<WorkSpaceResponse>?>(Response);
    }

    public async Task<Result<WorkSpaceResponse>> GetByIdAsync(int id, string? userId, CancellationToken cancelationToken)
    {


        var response = await _Context.WorkSpaces
            .Where(ws =>
                ws.Id == id &&
                ws.RemovedAt == null &&
                (
                    ws.CreatedById == userId ||
                    ws.workspaceMembers.Any(m => m.UserID == userId && m.WrokSpaceID == id)
                ))
            .Select(ws => new WorkSpaceResponse(
                ws.Id,
                ws.Name,
                ws.Description!,
                ws.IconCode!,
                ws.Visibility,
                ws.workspaceMembers.Count(m => m.WrokSpaceID == ws.Id),
                ws.Spaces
                    .Where(s => s.IsActive && s.RemovedAt == null)
                    .SelectMany(s => s.Tasks.Where(t => t.IsActive && t.RemovedAt == null))
                    .Count(),
                ws.CreatedById == userId
            ))
            .AsNoTracking()
            .SingleOrDefaultAsync(cancelationToken);

        if (response is null)
            return Result.Faluire<WorkSpaceResponse>(WorkSpaceErrors.WorkSpaceNotFound);

        return Result.Success(response);
    }

    public async Task<Result<WorkspaceDashboardResponse>> GetWorkspaceDashboardAsync(string UserId, CancellationToken cancellationToken = default)
    {


        var UserIsFounded = await _Context.Users
            .AnyAsync(u => u.Id == UserId && u.IsDisabled == false, cancellationToken);

        if (!UserIsFounded)
            return Result.Faluire<WorkspaceDashboardResponse>(UserErrors.UserNotFounded);


        var WeekStart = DateTime.UtcNow.Date.AddDays(-6);

        var ActiveTasksQuery = _Context.Tasks
            .Where(t => t.RemovedAt == null && t.Space.WorkSpace.CreatedById == UserId && t.Space.WorkSpace.RemovedAt == null && t.Space.RemovedAt == null);

        var TaskStatistics = await ActiveTasksQuery
            .GroupBy(_ => 1)
            .Select(AllItemsAsOne => new
            {
                Total = AllItemsAsOne.Count(),
                Completed = AllItemsAsOne.Count(t => t.Status == TaskStatuss.Completed),
                InProgress = AllItemsAsOne.Count(t => t.Status == TaskStatuss.Ongoing),
                Todo = AllItemsAsOne.Count(t => t.Status == TaskStatuss.Todo),
                Overdue = AllItemsAsOne.Count(t => t.DueDate != null && t.DueDate < DateTime.UtcNow && t.Status != TaskStatuss.Completed)
            })
            .SingleOrDefaultAsync(cancellationToken);


        var WeeklyFocusSessions = await _Context.FocusSessions
           .Where(fs => fs.Space.WorkSpace.CreatedById == UserId && fs.Space.WorkSpace.RemovedAt == null && fs.Space.RemovedAt == null
           && fs.Status == FocusSessionStatus.Completed && fs.EndedAt != null && fs.EndedAt.Value >= WeekStart)
           .Select(fs => new
           {
               fs.StartedAt,
               fs.EndedAt,
               fs.TotalPausedSeconds,
               fs.CompletedSubtasks,
               fs.InitialCompletedSubtasks
           })
           .AsNoTracking()
           .ToListAsync(cancellationToken);


        var DailyFocusHours = Enumerable.Range(0, 7)
            .Select(i => DateTime.UtcNow.Date.AddDays(-(6 - i)))
            .ToDictionary(d => d, _ => 0d);

        foreach (var s in WeeklyFocusSessions)
        {
            var end = s.EndedAt!.Value;

            var hours = (end - s.StartedAt - TimeSpan.FromSeconds(Math.Max(0, s.TotalPausedSeconds))).TotalHours;

            if (hours < 0)
                hours = 0;

            if (DailyFocusHours.ContainsKey(end.Date))
                DailyFocusHours[end.Date] += hours;
        }


        var FocusSessionsThisWeek = WeeklyFocusSessions.Count;

        var FocusTimeHours = Math.Round(DailyFocusHours.Values.Sum(), 2);

        var ProductiveSessions = WeeklyFocusSessions.Count(s => s.CompletedSubtasks > s.InitialCompletedSubtasks);

        var FocusCompletionRate = FocusSessionsThisWeek == 0 ? 0 : (int)Math.Round((ProductiveSessions / (double)FocusSessionsThisWeek) * 100);

        var Statisitics = new DashboardStatsResponse(
            TaskStatistics?.Total ?? 0,
            TaskStatistics?.Completed ?? 0,
            TaskStatistics?.InProgress ?? 0,
            TaskStatistics?.Todo ?? 0,
            TaskStatistics?.Overdue ?? 0,
            FocusSessionsThisWeek,
            FocusTimeHours,
            FocusCompletionRate,
             TaskStatistics?.Total == 0 ? 0 :
                             (int)Math.Round(((TaskStatistics?.Completed ?? 0) / (double)(TaskStatistics?.Total ?? 1)) * 100)
        );


        var WeeklyFocusTime = new WeeklyFocusTimeResponse
               (
                DailyFocusHours
                    .OrderBy(x => x.Key)
                    .Select(x => new FocusDayResponse(x.Key.ToString("ddd", System.Globalization.CultureInfo.GetCultureInfo("en-US")), Math.Round(x.Value, 2)))
                    .ToList()
               );


        return Result.Success(new WorkspaceDashboardResponse(Statisitics, WeeklyFocusTime));
    }

    public async Task<Result<WorkSpaceResponse>> UpdateAsync(int Id, string UserId, WorkSpaceRequest requset, CancellationToken cancellationToken)
    {

        var workSpace = await _Context.WorkSpaces.
            Where(x => x.CreatedById == UserId && x.Id == Id && x.RemovedAt == null)
            .SingleOrDefaultAsync(cancellationToken);

        if (workSpace is null)
            return Result.Faluire<WorkSpaceResponse>(WorkSpaceErrors.WorkSpaceNotFound);

        workSpace.Name = requset.Name;
        workSpace.Description = requset.Description;
        workSpace.IconCode = requset.IconCode;
        workSpace.Visibility = requset.Visibility;

        await _Context.SaveChangesAsync(cancellationToken);

        var response = workSpace.Adapt<WorkSpace, WorkSpaceResponse>();

        return Result.Success(response);
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {

        var workspace = await _Context.WorkSpaces.SingleOrDefaultAsync(x => x.Id == id && x.RemovedAt == null, cancellationToken);

        if (workspace is null)
            return Result.Faluire(WorkSpaceErrors.WorkSpaceNotFound);

        workspace.IsActive = false;
        workspace.RemovedAt = DateTime.UtcNow;
        workspace.RemovedById = _HttpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RestoreAsync(int id, CancellationToken cancellationToken)
    {
        var workspace = await _Context.WorkSpaces.SingleOrDefaultAsync(x => x.Id == id && x.RemovedAt != null, cancellationToken);

        if (workspace is null)
            return Result.Faluire(WorkSpaceErrors.WorkSpaceNotFound);

        workspace.IsActive = true;
        workspace.RemovedAt = null;
        workspace.RemovedById = null;

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> AddMemberAsync(int Id, string UserId, InviteMember request, CancellationToken cancellationToken)
    {
        var workspace = await _Context.WorkSpaces
            .Where(x => x.CreatedById == UserId && x.Id == Id && x.RemovedAt == null)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);


        if (workspace is null)
            return Result.Faluire(WorkSpaceErrors.WorkSpaceNotFound);

        if (workspace.Visibility == 0)
            return Result.Faluire(WorkspaceMemberErrors.InvalidAssign);

        var normalizedEmail = request.email.Trim().ToUpper();

        var invitedUser = await _Context.Users
            .Where(x => x.NormalizedEmail == normalizedEmail && !x.IsDisabled)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (invitedUser is null)
            return Result.Faluire(UserErrors.EmailnotFounded);

        var isExistsBefore = await _Context.WorkspaceMembers
            .AnyAsync(x => x.UserID == invitedUser.Id && x.WrokSpaceID == Id, cancellationToken);

        if (isExistsBefore)
            return Result.Faluire(UserErrors.AddedMemberBefore);

        var inviterUser = await _Context.Users
            .Where(x => x.Id == UserId && !x.IsDisabled)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (inviterUser is null)
            return Result.Faluire(UserErrors.UserNotFounded);

        var workSpaceMember = new WorkspaceMember
        {
            UserID = invitedUser.Id,
            WrokSpaceID = Id,
            IsOwner = false,
            JoinedAt = DateTime.UtcNow,
            Permissions =
            [
               Permissions.GetWorkSpaces,
               Permissions.GetSpaces,
               Permissions.GetTasks,
               Permissions.GetNotes
            ]
        };

        await _Context.WorkspaceMembers.AddAsync(workSpaceMember, cancellationToken);

        await _Context.SaveChangesAsync(cancellationToken);

        await SendEmailInvite(inviterUser, invitedUser, workspace, "✅ AiGenda Team: Addition Service");

        return Result.Success();


    }

    public async Task<Result> RemoveMemberAsync(int Id, string RemoverUserId, InviteMember request, CancellationToken cancellationToken = default!)
    {

        var workspace = await _Context.WorkSpaces
          .Where(w => w.Id == Id && w.RemovedAt == null)
          .Select(w => new { w.Id, w.CreatedById })
          .SingleOrDefaultAsync(cancellationToken);

        if (workspace is null)
            return Result.Faluire(WorkSpaceErrors.WorkSpaceNotFound);

        var normalizedEmail = request.email.Trim().ToUpperInvariant();

        var removedMember = await _Context.Users
            .Where(x => x.NormalizedEmail == normalizedEmail)
            .Select(x => new { x.Id })
            .SingleOrDefaultAsync(cancellationToken);

        if (removedMember is null)
            return Result.Faluire(UserErrors.EmailnotFounded);

        var isOwner = workspace.CreatedById == RemoverUserId;
        var isSelfRemoval = removedMember.Id == RemoverUserId;

        if (!isOwner && !isSelfRemoval)
            return Result.Faluire(WorkspaceMemberErrors.AccessDenied);

        // Safety: do not allow removing workspace creator membership record
        if (removedMember.Id == workspace.CreatedById)
            return Result.Faluire(WorkspaceMemberErrors.AccessDenied);

        var memberRecord = await _Context.WorkspaceMembers
            .SingleOrDefaultAsync(m => m.WrokSpaceID == Id && m.UserID == removedMember.Id, cancellationToken);

        if (memberRecord is null)
            return Result.Faluire(WorkspaceMemberErrors.MemberNotFounded);

        _Context.WorkspaceMembers.Remove(memberRecord);

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success();


    }

    public async Task<Result<IEnumerable<WorkspaceMemberResponse>>> GetMembersAsync(int Id, string UserId, CancellationToken cancellationToken = default!)
    {
        var workspace = await _Context.WorkSpaces
            .Where(w => w.Id == Id && w.RemovedAt == null)
            .Select(w => new { w.Id, w.CreatedById })
            .SingleOrDefaultAsync(cancellationToken);

        if (workspace is null)
            return Result.Faluire<IEnumerable<WorkspaceMemberResponse>>(WorkSpaceErrors.WorkSpaceNotFound);

        var isOwner = workspace.CreatedById == UserId;

        var isMember = !isOwner && await _Context.WorkspaceMembers
            .AnyAsync(m => m.WrokSpaceID == Id && m.UserID == UserId, cancellationToken);

        if (!isOwner && !isMember)
            return Result.Faluire<IEnumerable<WorkspaceMemberResponse>>(WorkspaceMemberErrors.AccessDenied);

        var members = await _Context.WorkspaceMembers
            .Where(m => m.WrokSpaceID == Id)
            .OrderByDescending(m => m.IsOwner)
            .ThenBy(m => m.JoinedAt)
            .Select(m => new WorkspaceMemberResponse(
                m.UserID,
                (m.User!.FirstName + " " + m.User.SecondName).Trim(),
                m.User.Email ?? string.Empty,
                m.IsOwner,
                m.JoinedAt,
                m.Permissions
            ))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<WorkspaceMemberResponse>>(members);
    }

    public async Task<Result<WorkspaceMemberResponse>> UpdateMemberPermissionsAsync(int Id, string OwnerUserId, string MemberUserId, UpdateWorkspaceMemberPermissionsRequest request, CancellationToken cancellationToken = default!)
    {
        var workspace = await _Context.WorkSpaces
            .Where(w => w.Id == Id && w.RemovedAt == null)
            .Select(w => new { w.Id, w.CreatedById })
            .SingleOrDefaultAsync(cancellationToken);

        if (workspace is null)
            return Result.Faluire<WorkspaceMemberResponse>(WorkSpaceErrors.WorkSpaceNotFound);

        // Only workspace owner can update member permissions
        if (workspace.CreatedById != OwnerUserId)
            return Result.Faluire<WorkspaceMemberResponse>(WorkspaceMemberErrors.AccessDenied);

        var memberRecord = await _Context.WorkspaceMembers
            .SingleOrDefaultAsync(m => m.WrokSpaceID == Id && m.UserID == MemberUserId, cancellationToken);

        if (memberRecord is null)
            return Result.Faluire<WorkspaceMemberResponse>(WorkspaceMemberErrors.MemberNotFounded);

        // Do not allow editing owner membership permissions
        if (memberRecord.IsOwner || MemberUserId == workspace.CreatedById)
            return Result.Faluire<WorkspaceMemberResponse>(WorkspaceMemberErrors.OwnerPermissionsImmutable);

        var allowedPermissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {

            Permissions.UpdateSpaces,
            Permissions.AddSpaces,
            Permissions.DeleteSpaces,

            Permissions.UpdateTasks,
            Permissions.AddTasks,
            Permissions.DeleteTasks,

            Permissions.UpdateNotes,
            Permissions.AddNotes,
            Permissions.DeleteNotes
        };

        var requested = request.Permissions
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var hasInvalidPermission = requested.Any(p => !allowedPermissions.Contains(p));
        if (hasInvalidPermission)
            return Result.Faluire<WorkspaceMemberResponse>(WorkspaceMemberErrors.InvalidPermissions);

        // Baseline workspace visibility must stay enabled for any member
        if (!requested.Contains(Permissions.GetWorkSpaces, StringComparer.OrdinalIgnoreCase))
        {
            requested.Insert(0, Permissions.GetWorkSpaces);
            requested.Insert(1, Permissions.GetSpaces);
            requested.Insert(2, Permissions.GetTasks);
            requested.Insert(3, Permissions.GetNotes);

        }

        memberRecord.Permissions = requested;

        await _Context.SaveChangesAsync(cancellationToken);

        var response = await _Context.WorkspaceMembers
            .Where(m => m.WrokSpaceID == Id && m.UserID == MemberUserId)
            .Select(m => new WorkspaceMemberResponse(
                m.UserID,
                (m.User!.FirstName + " " + m.User.SecondName).Trim(),
                m.User.Email ?? string.Empty,
                m.IsOwner,
                m.JoinedAt,
                m.Permissions
            ))
            .AsNoTracking()
            .SingleAsync(cancellationToken);

        return Result.Success(response);
    }

    public async Task<Result<WorkspaceMemberPermissionsResponse>> GetMemberPermissionsAsync(int Id, string RequesterUserId, string MemberUserId, CancellationToken cancellationToken = default!)
    {

        var workspace = await _Context.WorkSpaces
            .Where(w => w.Id == Id && w.RemovedAt == null)
            .Select(w => new { w.Id, w.CreatedById })
            .SingleOrDefaultAsync(cancellationToken);

        if (workspace is null)
            return Result.Faluire<WorkspaceMemberPermissionsResponse>(WorkSpaceErrors.WorkSpaceNotFound);

        var isOwner = workspace.CreatedById == RequesterUserId;

        var requesterIsMember = !isOwner && await _Context.WorkspaceMembers
            .AnyAsync(m => m.WrokSpaceID == Id && m.UserID == RequesterUserId, cancellationToken);


        if (!isOwner && !requesterIsMember)
            return Result.Faluire<WorkspaceMemberPermissionsResponse>(WorkspaceMemberErrors.AccessDenied);


        if (!isOwner && RequesterUserId != MemberUserId)
            return Result.Faluire<WorkspaceMemberPermissionsResponse>(WorkspaceMemberErrors.AccessDenied);

        if (MemberUserId == workspace.CreatedById)
        {
            return Result.Success(new WorkspaceMemberPermissionsResponse(
                MemberUserId,
                true,
                OwnerPermissions.GetAllPerimision
            ));
        }

        var memberPermissions = await _Context.WorkspaceMembers
            .Where(m => m.WrokSpaceID == Id && m.UserID == MemberUserId)
            .Select(m => new WorkspaceMemberPermissionsResponse(
                m.UserID,
                m.IsOwner,
                m.Permissions
            ))
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (memberPermissions is null)
            return Result.Faluire<WorkspaceMemberPermissionsResponse>(WorkspaceMemberErrors.MemberNotFounded);

        return Result.Success(memberPermissions);
    }

    public async Task<Result<IEnumerable<DeletedWorkSpaceResponse>>> GetAllDeletedAsync(string UserId, CancellationToken cancellationToken = default!)
    {
        var response = await _Context.WorkSpaces
            .Where(ws => ws.CreatedById == UserId && ws.RemovedAt != null)
            .OrderByDescending(ws => ws.RemovedAt)
            .Select(ws => new DeletedWorkSpaceResponse(
                ws.Id,
                ws.Name,
                ws.Description!,
                ws.IconCode!,
                ws.Visibility,
                ws.RemovedAt!.Value,
                ws.workspaceMembers.Count(),
                ws.Spaces
                    .Where(s => s.IsActive && s.RemovedAt == null)
                    .SelectMany(s => s.Tasks.Where(t => t.IsActive && t.RemovedAt == null))
                    .Count()
            ))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<DeletedWorkSpaceResponse>>(response);
    }

    public async Task<Result<WorkspaceByIdDashboardResponse>> GetWorkspaceDashboardByIdAsync(int Id, string UserId, CancellationToken cancellationToken = default)
    {

        var user = await _Context.Users
            .Where(u => u.Id == UserId && !u.IsDisabled)
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
            return Result.Faluire<WorkspaceByIdDashboardResponse>(UserErrors.UserNotFounded);

        var workspaceExists = await _Context.WorkspaceMembers
            .AnyAsync(w => w.WrokSpaceID == Id  && w.UserID == UserId , cancellationToken);

        if (!workspaceExists)
            return Result.Faluire<WorkspaceByIdDashboardResponse>(WorkSpaceErrors.WorkSpaceNotFound);


        var CurrentweekStart = DateTime.UtcNow.Date.AddDays(-6);

        var PreviousweekStart = CurrentweekStart.AddDays(-7);

        var TaskStatisitics = await _Context.Tasks
            .Where(t => t.Space.WorkSpaceId == Id && t.Space.WorkSpace.RemovedAt == null && t.RemovedAt == null && t.Space.RemovedAt == null )
            .GroupBy(_ => 1)
            .Select( OneItemelements => new
            {
                Total = OneItemelements.Count(),
                Completed = OneItemelements.Count(t => t.Status == TaskStatuss.Completed),
                NewToday = OneItemelements.Count(t => t.CreatedAt >= DateTime.UtcNow.Date)
            })
            .SingleOrDefaultAsync(cancellationToken);



        var ActiveSpaces = await _Context.Spaces
            .CountAsync(s => s.WorkSpaceId == Id && s.RemovedAt == null, cancellationToken);

        var ActiveCollaborators = await _Context.WorkspaceMembers
            .CountAsync(m => m.WrokSpaceID == Id && !m.User!.IsDisabled, cancellationToken);


        var FocusSessions = await _Context.FocusSessions
            .Where(fs => fs.WorkspaceId == Id
                && fs.Status == FocusSessionStatus.Completed
                && fs.EndedAt != null
                && fs.Space.RemovedAt == null
                && fs.Space.WorkSpace.RemovedAt == null
                && fs.EndedAt.Value >= PreviousweekStart)
            .Select(fs => new
            {
                fs.StartedAt,
                EndedAt = fs.EndedAt!.Value,
                fs.TotalPausedSeconds
            })
            .AsNoTracking()
            .ToListAsync(cancellationToken);


        var DailyFocusHours = Enumerable.Range(0, 7)
            .Select(i => CurrentweekStart.AddDays(i))
            .ToDictionary(d => d, _ => 0d);

        var currentWeekHours = 0d;

        var previousWeekHours = 0d;

        foreach (var s in FocusSessions)
        {
            var hours = CalculateFocusHours(s.StartedAt, s.EndedAt, s.TotalPausedSeconds);

            if (s.EndedAt.Date >= CurrentweekStart && s.EndedAt.Date <= DateTime.UtcNow.Date)
            {
                currentWeekHours += hours;

                if (DailyFocusHours.ContainsKey(s.EndedAt.Date))
                    DailyFocusHours[s.EndedAt.Date] += hours;
            }
            else if (s.EndedAt.Date >= PreviousweekStart && s.EndedAt.Date < CurrentweekStart)
            {
                previousWeekHours += hours;
            }
        }

        currentWeekHours = Math.Round(currentWeekHours, 2);

        var averageDailyFocusHours = Math.Round(currentWeekHours / 7d, 1);

        var productivityScore = (TaskStatisitics?.Total ?? 0) == 0 ? 0 : (int)Math.Round((TaskStatisitics!.Completed / (double)TaskStatisitics.Total) * 100);
           
        var productivityDeltaPercent = previousWeekHours <= 0
            ? (currentWeekHours > 0 ? 100 : 0)
            : (int)Math.Round(((currentWeekHours - previousWeekHours) / previousWeekHours) * 100);


        var WeeklyFocusTime = new WeeklyFocusTimeResponse(
            DailyFocusHours
                .OrderBy(x => x.Key)
                .Select(x => new FocusDayResponse(x.Key.ToString("ddd", System.Globalization.CultureInfo.GetCultureInfo("en-US")),Math.Round(x.Value, 2)))
                .ToList());


        var PriorityTasks = await _Context.Tasks
            .Where(t => t.Space.WorkSpaceId == Id
                && t.RemovedAt == null
                && t.Space.RemovedAt == null
                && t.Status != TaskStatuss.Completed
                && t.Status != TaskStatuss.Cancelled)
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.DueDate ?? DateTime.MaxValue)
            .ThenByDescending(t => t.CreatedAt)
            .Take(5)
            .Select(t => new WorkspaceDashboardPriorityTaskResponse(
                t.Id,
                t.Title,
                t.Priority,
                t.Status,
                t.DueDate,
                t.Space.Name))
            .AsNoTracking()
            .ToListAsync(cancellationToken);


        var TaskActivities = await _Context.Tasks
            .Where(t => t.Space.WorkSpaceId == Id
                && t.RemovedAt == null
                && t.Space.RemovedAt == null
                && t.Status == TaskStatuss.Completed
                && t.UpdatedAt != null)
            .OrderByDescending(t => t.UpdatedAt)
            .Take(10)
            .Select(t => new WorkspaceRecentActivityResponse($"Task completed in {t.Space.Name}: {t.Title}", t.UpdatedAt!.Value,"success"))                                               
            .AsNoTracking()
            .ToListAsync(cancellationToken);


        var NoteActivities = await _Context.Notes
            .Where(n => n.Space.WorkSpaceId == Id && n.RemovedAt == null && n.Space.RemovedAt == null)
            .OrderByDescending(n => n.CreatedAt)
            .Take(10)
            .Select(n => new WorkspaceRecentActivityResponse(
                n.Type == NoteType.Image || n.Type == NoteType.Voice? $"File uploaded: {n.Title}": $"New note added: {n.Title}",
                n.CreatedAt, 
                n.Type == NoteType.Image || n.Type == NoteType.Voice ? "info" : "primary"))
            .AsNoTracking()
            .ToListAsync(cancellationToken);


        var FocusActivities = await _Context.FocusSessions
            .Where(fs => fs.WorkspaceId == Id
                && fs.Status == FocusSessionStatus.Completed
                && fs.EndedAt != null
                && fs.Space.RemovedAt == null)
            .OrderByDescending(fs => fs.EndedAt)
            .Take(10)
            .Select(fs => new WorkspaceRecentActivityResponse( $"Focus session completed: {fs.TaskName}",fs.EndedAt!.Value,"warning"))
            .AsNoTracking()
            .ToListAsync(cancellationToken);


        var RecentActivities = TaskActivities
            .Concat(NoteActivities)
            .Concat(FocusActivities)
            .OrderByDescending(x => x.OccurredAt)
            .Take(5)
            .ToList();


        var Cards = new WorkspaceDashboardCardsResponse(
            TaskStatisitics?.Total ?? 0,
            TaskStatisitics?.NewToday ?? 0,
            currentWeekHours,
            averageDailyFocusHours,
            ActiveSpaces,
            ActiveCollaborators,
            productivityScore);


        var response = new WorkspaceByIdDashboardResponse(
            user.FirstName + " " + user.SecondName,
            productivityDeltaPercent,
            Cards,
            WeeklyFocusTime,
            RecentActivities,
            PriorityTasks);


        return Result.Success(response);
    }





    private async Task SendEmailInvite(ExtendedUser User, ExtendedUser InvitedUser, WorkSpace Workspace, string Message)
    {
        var origin = _HttpContextAccessor.HttpContext?.Request.Headers.Origin;

        var BuilderMessage = EmailBodyBuilder.GenerateEmailBody("WorkspaceMemberInvite",

        new Dictionary<string, string>
            {
                        {"{{WorkSpaceName}}", Workspace.Name},
                        {"{{FullName}}", InvitedUser.FirstName + " "+ InvitedUser.SecondName },
                        {"{{AddedUserName}}", User.FirstName + " "+ User.SecondName },
                        { "{{action_url}}", $"{origin}/WorkSpace/Add-member?workspaceId={Workspace.Id}&&UserId={User.Id}" }
            }
        );

        BackgroundJob.Enqueue(() => _EmailSender.SendEmailAsync(User.Email!, Message, BuilderMessage));

    }

    private static double CalculateFocusHours(DateTime startedAt, DateTime endedAt, int totalPausedSeconds)
    {
        var hours = (endedAt - startedAt - TimeSpan.FromSeconds(Math.Max(0, totalPausedSeconds))).TotalHours;

        return hours < 0 ? 0 : hours;
    }

}

