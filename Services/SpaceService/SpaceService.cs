using AI_genda_API.Abstractions.Enums;
using AI_genda_API.Contracts.Results;
using Microsoft.Extensions.Caching.Memory;

namespace AI_genda_API.Services.SpaceService;

public class SpaceService(AppContext context, IMemoryCache memoryCache) : ISpaceService
{
    private IMemoryCache _MemoryCache = memoryCache;
    private readonly AppContext _Context = context;
    private readonly string PerfixSpace = "Space";


    public async Task<Result<SpaceDetailResponse>>AddAsync(int WorkspaceId, string UserId, SpaceRequest request, CancellationToken cancellationToken = default!)
    {
        var IsWorkspaceExists = await _Context.WorkSpaces
            .AnyAsync(w => w.Id == WorkspaceId && w.RemovedAt == null, cancellationToken);

        if (!IsWorkspaceExists)
            return Result.Faluire<SpaceDetailResponse>(WorkSpaceErrors.WorkSpaceNotFound);


        var IsMemberOrOwner = await _Context.WorkSpaces
            .AnyAsync(w => w.Id == WorkspaceId && w.CreatedById == UserId, cancellationToken)
            || await _Context.WorkspaceMembers
            .AnyAsync(m => m.WrokSpaceID == WorkspaceId && m.UserID == UserId, cancellationToken);


        if (!IsMemberOrOwner)
            return Result.Faluire<SpaceDetailResponse>(WorkspaceMemberErrors.AccessDenied);

        var space = new Space
        {
            Name = request.Name,
            Description = request.Description,
            IconCode = request.IconCode,
            IsPublic = request.IsPublic,
            WorkSpaceId = WorkspaceId,
            LastActivity = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        await _Context.Spaces.AddAsync(space, cancellationToken);

        string Key = PerfixSpace + "_" + WorkspaceId + "_" + UserId;

        _MemoryCache.Remove(Key);

        await _Context.SaveChangesAsync(cancellationToken);

        var response = space.Adapt<Space, SpaceDetailResponse>();

        return Result.Success(response);
    }

    public async Task<Result<PaginatedList<SpaceDetailResponse>>> GetAllAsync(int WorkspaceId, string UserId, FilterRequest filterRequest, CancellationToken cancellationToken = default!)
    {

        var WorkspaceExists = await _Context.WorkSpaces
          .AnyAsync(w => w.Id == WorkspaceId && w.RemovedAt == null, cancellationToken);

        if (!WorkspaceExists)
            return Result.Faluire<PaginatedList<SpaceDetailResponse>>(WorkSpaceErrors.WorkSpaceNotFound);

        var IsOwner = await _Context.WorkSpaces
            .AnyAsync(w => w.Id == WorkspaceId && w.CreatedById == UserId, cancellationToken);

        var IsMember = !IsOwner && await _Context.WorkspaceMembers
            .AnyAsync(m => m.WrokSpaceID == WorkspaceId && m.UserID == UserId, cancellationToken);

        if (!IsOwner && !IsMember)
            return Result.Faluire<PaginatedList<SpaceDetailResponse>>(WorkspaceMemberErrors.AccessDenied);


       var Query = _Context.Spaces.Where(s => s.WorkSpaceId == WorkspaceId && s.RemovedAt == null);

        if (!string.IsNullOrEmpty(filterRequest.SearchValue))
        {
            var Tokens = filterRequest.SearchValue.Tokenize().ToList();

            Query = Query.Where(x => Tokens.Any(token => x.Name.Contains(token) || x.Description!.Contains(token)));
        }

        if (!string.IsNullOrEmpty(filterRequest.SortColumn))
        {
            Query = Query.OrderBy($"{filterRequest.SortColumn} {filterRequest.SortOrder}");
        }

        IQueryable<SpaceDetailResponse> spaceRespone = null!;
        
        if (IsOwner)
        {
           
               spaceRespone = Query.Select(s => new SpaceDetailResponse(
                           s.Id,
                           s.Name,
                           s.Description,
                           s.IconCode,
                           s.LastActivity,
                           s.IsPublic,
                      s.Tasks.Count(t => t.RemovedAt == null),
                      s.Tasks.Count(t => t.RemovedAt == null && t.Status == TaskStatuss.Completed)
               ))
               .AsNoTracking();
              
        }
        else
        {
            spaceRespone = Query            
            .Select(s => new SpaceDetailResponse(
                        s.Id,
                        s.Name,
                        s.Description,
                        s.IconCode,
                        s.LastActivity,
                        s.IsPublic,
                   s.Tasks.Count(t => t.RemovedAt == null),
                   s.Tasks.Count(t => t.RemovedAt == null && t.Status == TaskStatuss.Completed)
            ))
            .AsNoTracking();
             
        }

        var Response = await PaginatedList<SpaceDetailResponse>.CreateAsync(spaceRespone, filterRequest.PageNumber, filterRequest.PageSize);


        return Result.Success(Response);
    }

    public async Task<Result<SpaceDetailResponse>> GetByIdAsync(int WorkspaceId, string Id, string UserId, CancellationToken cancellationToken = default!)
    {
        var WorkspaceExists = await _Context.WorkSpaces
          .AnyAsync(w => w.Id == WorkspaceId && w.RemovedAt == null, cancellationToken);

        if (!WorkspaceExists)
            return Result.Faluire<SpaceDetailResponse>(WorkSpaceErrors.WorkSpaceNotFound);

        var IsOwner = await _Context.WorkSpaces
            .AnyAsync(w => w.Id == WorkspaceId && w.CreatedById == UserId, cancellationToken);

        var IsMember = !IsOwner && await _Context.WorkspaceMembers
            .AnyAsync(m => m.WrokSpaceID == WorkspaceId && m.UserID == UserId, cancellationToken);

        if (!IsOwner && !IsMember)
            return Result.Faluire<SpaceDetailResponse>(WorkspaceMemberErrors.AccessDenied);

        SpaceDetailResponse? Space = null;

        if (IsOwner)
        {
            Space = await _Context.Spaces
               .Where(s => s.WorkSpaceId == WorkspaceId && s.RemovedAt == null && s.Id == Id)
               .Select(s => new SpaceDetailResponse(
                           s.Id,
                           s.Name,
                           s.Description,
                           s.IconCode,
                           s.LastActivity,
                           s.IsPublic,
                      s.Tasks.Count(t => t.RemovedAt == null),
                      s.Tasks.Count(t => t.RemovedAt == null && t.Status == TaskStatuss.Completed)
               ))
               .AsNoTracking()
               .SingleOrDefaultAsync(cancellationToken);
        }
        else
        {
            Space = await _Context.Spaces
            .Where(s => s.WorkSpaceId == WorkspaceId && s.RemovedAt == null && s.Id == Id && s.IsPublic)
            .Select(s => new SpaceDetailResponse(
                        s.Id,
                        s.Name,
                        s.Description,
                        s.IconCode,
                        s.LastActivity,
                        s.IsPublic,
                   s.Tasks.Count(t => t.RemovedAt == null),
                   s.Tasks.Count(t => t.RemovedAt == null && t.Status == TaskStatuss.Completed)
            ))
            .AsNoTracking()
           .SingleOrDefaultAsync(cancellationToken);
        }

        if (Space is null )
        {
            return Result.Faluire<SpaceDetailResponse>(SpaceErrors.SpaceNotFound);
        }


        return Result.Success(Space!);
    }

    public async Task<Result<SpaceDetailResponse>> UpdateAsync(int WorkspaceId, string Id, string UserId, SpaceRequest request, CancellationToken cancellationToken = default!)
    {
        var WorkspaceExists = await _Context.WorkSpaces
          .AnyAsync(w => w.Id == WorkspaceId && w.RemovedAt == null, cancellationToken);

        if (!WorkspaceExists)
            return Result.Faluire<SpaceDetailResponse>(WorkSpaceErrors.WorkSpaceNotFound);

        var IsOwner = await _Context.WorkSpaces
            .AnyAsync(w => w.Id == WorkspaceId && w.CreatedById == UserId, cancellationToken);

        var IsMember = !IsOwner && await _Context.WorkspaceMembers
            .AnyAsync(m => m.WrokSpaceID == WorkspaceId && m.UserID == UserId, cancellationToken);

        if (!IsOwner && !IsMember)
            return Result.Faluire<SpaceDetailResponse>(WorkspaceMemberErrors.AccessDenied);


        Space? Space = null;

        if (IsOwner)
        {
            Space = await _Context.Spaces
               .Where(s => s.WorkSpaceId == WorkspaceId && s.RemovedAt == null && s.Id == Id)
               .SingleOrDefaultAsync(cancellationToken);
        }
        else
        {
            Space = await _Context.Spaces
            .Where(s => s.WorkSpaceId == WorkspaceId && s.RemovedAt == null && s.Id == Id && s.IsPublic)
            .SingleOrDefaultAsync(cancellationToken);
        }


        if (Space is null)
            return Result.Faluire<SpaceDetailResponse>(SpaceErrors.SpaceNotFound);

        Space.Name = request.Name;
        Space.Description = request.Description;
        Space.IconCode = request.IconCode;
        Space.IsPublic = request.IsPublic;
        Space.LastActivity = DateOnly.FromDateTime(DateTime.UtcNow);

        string Key = PerfixSpace + "_" + WorkspaceId + "_" + UserId;

        _MemoryCache.Remove(Key);

        await _Context.SaveChangesAsync(cancellationToken);

        var response =  Space.Adapt< Space, SpaceDetailResponse >();

        return Result.Success(response);
    }

    public async Task<Result> DeleteAsync(int WorkspaceId, string Id, string UserId, CancellationToken cancellationToken = default!)
    {
        var WorkspaceExists = await _Context.WorkSpaces
          .AnyAsync(w => w.Id == WorkspaceId && w.RemovedAt == null, cancellationToken);

        if (!WorkspaceExists)
            return Result.Faluire<SpaceDetailResponse>(WorkSpaceErrors.WorkSpaceNotFound);

        var IsOwner = await _Context.WorkSpaces
            .AnyAsync(w => w.Id == WorkspaceId && w.CreatedById == UserId, cancellationToken);

        var IsMember = !IsOwner && await _Context.WorkspaceMembers
            .AnyAsync(m => m.WrokSpaceID == WorkspaceId && m.UserID == UserId, cancellationToken);

        if (!IsOwner && !IsMember)
            return Result.Faluire<SpaceDetailResponse>(WorkspaceMemberErrors.AccessDenied);


        var space = await _Context.Spaces
            .SingleOrDefaultAsync(s => s.Id == Id && s.WorkSpaceId == WorkspaceId && s.RemovedAt == null , cancellationToken);

        if (space is null)
            return Result.Faluire(SpaceErrors.SpaceNotFound);

        space.IsActive = false;
        space.RemovedAt = DateTime.UtcNow;
        space.RemovedById = UserId;

        string Key = PerfixSpace + "_" + WorkspaceId + "_" + UserId;

        _MemoryCache.Remove(Key);

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RestoreAsync(int WorkspaceId, string Id, string UserId, CancellationToken cancellationToken = default!)
    {
        var WorkspaceExists = await _Context.WorkSpaces
          .AnyAsync(w => w.Id == WorkspaceId && w.RemovedAt == null, cancellationToken);

        if (!WorkspaceExists)
            return Result.Faluire<SpaceDetailResponse>(WorkSpaceErrors.WorkSpaceNotFound);

        var IsOwner = await _Context.WorkSpaces
            .AnyAsync(w => w.Id == WorkspaceId && w.CreatedById == UserId, cancellationToken);

        var IsMember = !IsOwner && await _Context.WorkspaceMembers
            .AnyAsync(m => m.WrokSpaceID == WorkspaceId && m.UserID == UserId, cancellationToken);

        if (!IsOwner && !IsMember)
            return Result.Faluire<SpaceDetailResponse>(WorkspaceMemberErrors.AccessDenied);


        var Space = await _Context.Spaces
            .SingleOrDefaultAsync(s => s.Id == Id && s.WorkSpaceId == WorkspaceId && s.RemovedAt != null, cancellationToken);

        if (Space is null)
            return Result.Faluire(SpaceErrors.SpaceNotFound);

        Space.IsActive = true;
        Space.RemovedAt = null;
        Space.RemovedById = null;

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<IEnumerable<DeletedSpaceResponse>>> GetAllDeletedAsync(int WorkspaceId, string UserId, CancellationToken cancellationToken = default!)
    {
        var workspaceExists = await _Context.WorkSpaces
            .AnyAsync(w => w.Id == WorkspaceId && w.RemovedAt == null, cancellationToken);

        if (!workspaceExists)
            return Result.Faluire<IEnumerable<DeletedSpaceResponse>>(WorkSpaceErrors.WorkSpaceNotFound);

        var isOwner = await _Context.WorkSpaces
            .AnyAsync(w => w.Id == WorkspaceId && w.CreatedById == UserId , cancellationToken);

        var isMember = !isOwner && await _Context.WorkspaceMembers
            .AnyAsync(m => m.WrokSpaceID == WorkspaceId && m.UserID == UserId, cancellationToken);

        if (!isOwner && !isMember)
            return Result.Faluire<IEnumerable<DeletedSpaceResponse>>(WorkspaceMemberErrors.AccessDenied);

        var deletedSpacesQuery = _Context.Spaces
            .Where(s => s.WorkSpaceId == WorkspaceId && s.RemovedAt != null);

        if (!isOwner)
            deletedSpacesQuery = deletedSpacesQuery.Where(s => s.IsPublic);

        var deletedSpaces = await deletedSpacesQuery
            .OrderByDescending(s => s.RemovedAt)
            .Select(s => new DeletedSpaceResponse(
                s.Id,
                s.Name,
                s.Description,
                s.IconCode,
                s.LastActivity,
                s.RemovedAt!.Value,
                s.Tasks.Count(t => t.RemovedAt == null)
            ))
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<DeletedSpaceResponse>>(deletedSpaces);
    }

    public async Task<Result> MoveAsync(int WorkspaceId, string Id, string UserId, MoveSpaceRequest request, CancellationToken cancellationToken = default!)
    {
        var sourceWorkspaceExists = await _Context.WorkSpaces
            .AnyAsync(w => w.Id == WorkspaceId && w.RemovedAt == null, cancellationToken);

        if (!sourceWorkspaceExists)
            return Result.Faluire(WorkSpaceErrors.WorkSpaceNotFound);

        var targetWorkspaceExists = await _Context.WorkSpaces
            .AnyAsync(w => w.Id == request.TargetWorkspaceId && w.RemovedAt == null , cancellationToken);

        if (!targetWorkspaceExists)
            return Result.Faluire(WorkSpaceErrors.WorkSpaceNotFound);


        var canAccessSource = await _Context.WorkSpaces
            .AnyAsync(w => w.Id == WorkspaceId && w.CreatedById == UserId, cancellationToken) 
            || await _Context.WorkspaceMembers
              .AnyAsync(m => m.WrokSpaceID == WorkspaceId && m.UserID == UserId, cancellationToken);



        var canAccessTarget = await _Context.WorkSpaces
            .AnyAsync(w => w.Id == request.TargetWorkspaceId && w.CreatedById == UserId, cancellationToken)
            || await _Context.WorkspaceMembers
                .AnyAsync(m => m.WrokSpaceID == request.TargetWorkspaceId && m.UserID == UserId, cancellationToken);


        if (!canAccessSource || !canAccessTarget)
            return Result.Faluire(WorkspaceMemberErrors.AccessDenied);

        var isOwnerInSource = await _Context.WorkSpaces
            .AnyAsync(w => w.Id == WorkspaceId && w.CreatedById == UserId, cancellationToken);

        Space? space;

        if (isOwnerInSource)
        {
            space = await _Context.Spaces
                .SingleOrDefaultAsync(s => s.Id == Id && s.WorkSpaceId == WorkspaceId && s.RemovedAt == null, cancellationToken);
        }
        else
        {
            space = await _Context.Spaces
                .SingleOrDefaultAsync(s => s.Id == Id && s.WorkSpaceId == WorkspaceId && s.RemovedAt == null && s.IsPublic, cancellationToken);
        }

        if (space is null)
            return Result.Faluire(SpaceErrors.SpaceNotFound);

        if (space.WorkSpaceId == request.TargetWorkspaceId)
            return Result.Success();



        space.WorkSpaceId = request.TargetWorkspaceId;

        space.LastActivity = DateOnly.FromDateTime(DateTime.UtcNow);

        await _Context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result<SpaceAnalyticsResponse>> GetResultsAsync(int WorkspaceId, string Id, string UserId, SpaceAnalyticsQueryRequest request, CancellationToken cancellationToken = default!)
    {


        var workspaceExists = await _Context.WorkSpaces
            .AnyAsync(w => w.Id == WorkspaceId && w.RemovedAt == null, cancellationToken);

        if (!workspaceExists)
            return Result.Faluire<SpaceAnalyticsResponse>(WorkSpaceErrors.WorkSpaceNotFound);

        var hasAccess = await _Context.WorkspaceMembers.AnyAsync(m => m.WrokSpaceID == WorkspaceId && m.UserID == UserId, cancellationToken);

        if (!hasAccess)
            return Result.Faluire<SpaceAnalyticsResponse>(WorkspaceMemberErrors.AccessDenied);

        var space = await _Context.Spaces
            .SingleOrDefaultAsync(s => s.Id == Id && s.WorkSpaceId == WorkspaceId && s.RemovedAt == null, cancellationToken);

        if (space is null)
            return Result.Faluire<SpaceAnalyticsResponse>(SpaceErrors.SpaceNotFound);


        var fromDate = DateTime.UtcNow.Date.AddDays(-(request.Days - 1));

        var now = DateTime.UtcNow;

        var tasksQuery = _Context.Tasks
            .Where(t =>
                t.SpaceId == Id &&
                t.RemovedAt == null &&
                t.CreatedAt >= fromDate &&
                t.CreatedAt <= now);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var term = request.Search.Trim();

            tasksQuery = tasksQuery.Where(t => t.Title.Contains(term) || (t.Description != null && t.Description.Contains(term)));
        }

        var tasks = await tasksQuery
            .Select(t => new
            {
                t.Id,
                t.Title,
                t.Priority,
                t.Status,
                t.CreatedAt,
                t.UpdatedAt
            })
            .ToListAsync(cancellationToken);

        var totalTasks = tasks.Count;

        var completedTasks = tasks.Count(t => t.Status == TaskStatuss.Completed);

        var highCount = tasks.Count(t => t.Priority == TaskPriority.High || t.Priority == TaskPriority.Critical);

        var mediumCount = tasks.Count(t => t.Priority == TaskPriority.Medium);

        var lowCount = tasks.Count(t => t.Priority == TaskPriority.Low);

        double Percent(int count) => totalTasks == 0 ? 0 : Math.Round((count / (double)totalTasks) * 100, 2);

        var priorityDistribution = new List<PriorityDistributionItem>
        {
            new("High Priority", highCount, Percent(highCount)),
            new("Medium", mediumCount, Percent(mediumCount)),
            new("Low", lowCount, Percent(lowCount))
        };

        var weeks = Math.Max(1, (int)Math.Ceiling(request.Days / 7.0));

        var trend = new List<TaskCompletionTrendPoint>();

        for (var i = 0; i < weeks; i++)
        {
            var bucketStart = fromDate.AddDays(i * 7);

            var bucketEnd = bucketStart.AddDays(7).AddTicks(-1);

            if (bucketEnd > now) 
                bucketEnd = now;

            var planned = tasks.Count(t => t.CreatedAt >= bucketStart && t.CreatedAt <= bucketEnd);

            var completed = tasks.Count(t =>
                t.Status == TaskStatuss.Completed &&
                t.UpdatedAt.HasValue &&
                t.UpdatedAt.Value >= bucketStart &&
                t.UpdatedAt.Value <= bucketEnd);

            trend.Add(new TaskCompletionTrendPoint($"Week {i + 1}", planned, completed));
        }

        var teamProductivityResult = await _Context.TaskAssignees
            .Where(a =>
                a.Task != null &&
                a.Task.SpaceId == Id &&
                a.Task.RemovedAt == null &&
                a.Task.Status == TaskStatuss.Completed &&
                a.Task.UpdatedAt.HasValue &&
                a.Task.UpdatedAt.Value >= fromDate &&
                a.Task.UpdatedAt.Value <= now)
            .GroupBy(a => new
            {
                a.UserId,
                Name = a.User!.FirstName + " " + a.User.SecondName,
                a.User!.AvatarUrl
            })
            .Select(g => new TeamProductivityItem(
                g.Key.UserId,
                g.Key.Name,
                g.Key.AvatarUrl,
                g.Count()))
            .ToListAsync(cancellationToken);

        var teamProductivity = teamProductivityResult
            .OrderByDescending(x=>x.CompletedTasks)
            .ToList();

        var response = new SpaceAnalyticsResponse(
            space.Id,
            space.Name,
            space.IsActive,
            request.Days,
            totalTasks,
            completedTasks,
            trend,
            priorityDistribution,
            teamProductivity
        );

        return Result.Success(response);
    }

    public async Task<Result<SpaceAnalyticsExportResponse>> ExportResultsAsync(int WorkspaceId, string Id, string UserId, SpaceAnalyticsQueryRequest request, CancellationToken cancellationToken = default!)
    {


        var analytics = await GetResultsAsync(WorkspaceId, Id, UserId, request, cancellationToken);

        if (analytics.IsFaluire)
            return Result.Faluire<SpaceAnalyticsExportResponse>(analytics.Error);

        var a = analytics.Value;
        
        var sb = new StringBuilder();

        sb.AppendLine("Space Analytics Report");
        sb.AppendLine($"Space,{a.SpaceName}");
        sb.AppendLine($"Days Range,{a.DaysRange}");
        sb.AppendLine($"Total Tasks,{a.TotalTasks}");
        sb.AppendLine($"Completed Tasks,{a.CompletedTasks}");
        sb.AppendLine();
        sb.AppendLine("Task Completion Trend");
        sb.AppendLine("Label,Planned,Completed");

        foreach (var p in a.TaskCompletionTrend)
            sb.AppendLine($"{p.Label},{p.Planned},{p.Completed}");

        sb.AppendLine();

        sb.AppendLine("Priority Distribution");
        sb.AppendLine("Label,Count,Percent");
        foreach (var p in a.PriorityDistribution)
            sb.AppendLine($"{p.Label},{p.Count},{p.Percent}");


        sb.AppendLine();

        sb.AppendLine("Team Productivity");
        sb.AppendLine("Member,CompletedTasks");
        foreach (var m in a.TeamProductivity)
            sb.AppendLine($"{m.MemberName},{m.CompletedTasks}");


        var bytes = Encoding.UTF8.GetBytes(sb.ToString());

        var fileName = $"space-analytics-{a.SpaceId}-{DateTime.UtcNow:yyyyMMddHHmmss}.csv";


        return Result.Success(new SpaceAnalyticsExportResponse(bytes,"text/csv",fileName));
           
    }


}