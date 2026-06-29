using AI_genda_API.Contracts.SubTask;

namespace AI_genda_API.Services.SubTaskService;

public class SubTaskService(AppContext context) : ISubTaskService
{
    private readonly AppContext _Context = context;

    public async Task<Result<SubTaskResponse>> AddSubTaskAsync(int WorkspaceId, string SpaceId, string TaskId, string UserId, SubTaskRequest request, CancellationToken cancellationToken = default!)
    {

        var hasAccess = await HasAccessAsync(WorkspaceId, UserId, cancellationToken);

        if (!hasAccess)
            return Result.Faluire<SubTaskResponse>(WorkspaceMemberErrors.AccessDenied);

        var taskExists = await _Context.Tasks
            .AnyAsync(t => t.Id == TaskId && t.SpaceId == SpaceId && t.RemovedAt == null, cancellationToken);

        if (!taskExists)
            return Result.Faluire<SubTaskResponse>(TaskErrors.TaskNotFound);

        var subTask = new SubTask
        {
            Title = request.Title,
            TaskId = TaskId,
        };


        await _Context.SubTasks.AddAsync(subTask, cancellationToken);

        await _Context.SaveChangesAsync(cancellationToken);

        var response = new SubTaskResponse(subTask.Id, subTask.Title, subTask.IsCompleted, subTask.CreatedAt);

        return Result.Success(response);
    }

    public async Task<Result<SubTaskResponse>> UpdateSubTaskAsync(int WorkspaceId, string SpaceId, string TaskId, string Id, string UserId, SubTaskRequest request, CancellationToken cancellationToken = default!)
    {

        var hasAccess = await HasAccessAsync(WorkspaceId, UserId, cancellationToken);

        if (!hasAccess)
            return Result.Faluire<SubTaskResponse>(WorkspaceMemberErrors.AccessDenied);

        var subTask = await _Context.SubTasks
            .SingleOrDefaultAsync(st => st.Id == Id && st.TaskId == TaskId && st.RemovedAt == null, cancellationToken);

        if (subTask is null)
            return Result.Faluire<SubTaskResponse>(SubTaskErrors.SubTaskNotFound);


        subTask.Title = request.Title;
        
        await _Context.SaveChangesAsync(cancellationToken);

        var response = new SubTaskResponse(subTask.Id, subTask.Title, subTask.IsCompleted, subTask.CreatedAt);

        return Result.Success(response);
    }

    public async Task<Result> UpdateSubTaskStatusAsync(int WorkspaceId, string SpaceId, string TaskId, string Id, string UserId, UpdateSubTaskStatusRequest request, CancellationToken cancellationToken = default!)
    {

        var hasAccess = await HasAccessAsync(WorkspaceId, UserId, cancellationToken);

        if (!hasAccess)
            return Result.Faluire(WorkspaceMemberErrors.AccessDenied);

        var subTask = await _Context.SubTasks
            .SingleOrDefaultAsync(st => st.Id == Id && st.TaskId == TaskId && st.RemovedAt == null, cancellationToken);

        if (subTask is null)
            return Result.Faluire(SubTaskErrors.SubTaskNotFound);

        subTask.IsCompleted = request.IsCompleted;

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> DeleteSubTaskAsync(int WorkspaceId, string SpaceId, string TaskId, string Id, string UserId, CancellationToken cancellationToken = default!)
    {

        var hasAccess = await HasAccessAsync(WorkspaceId, UserId, cancellationToken);

        if (!hasAccess)
            return Result.Faluire(WorkspaceMemberErrors.AccessDenied);

        var subTask = await _Context.SubTasks
            .SingleOrDefaultAsync(st => st.Id == Id && st.TaskId == TaskId && st.RemovedAt == null, cancellationToken);

        if (subTask is null)
            return Result.Faluire(SubTaskErrors.SubTaskNotFound);

        subTask.RemovedAt = DateTime.UtcNow;

        subTask.RemovedById = UserId;

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }


    private async Task<bool> HasAccessAsync(int WorkspaceId, string UserId, CancellationToken cancellationToken)
    {
        return await _Context.WorkSpaces.AnyAsync(w => w.Id == WorkspaceId && w.CreatedById == UserId, cancellationToken)
            || await _Context.WorkspaceMembers.AnyAsync(m => m.WrokSpaceID == WorkspaceId && m.UserID == UserId, cancellationToken);
    }

}
