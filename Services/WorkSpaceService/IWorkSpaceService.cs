namespace AI_genda_API.Services.FolderService;

public interface IWorkSpaceService
{
    public Task<Result<WorkSpaceResponse>> AddAsync(string UserId, WorkSpaceRequest Requset, CancellationToken cancellationToken = default!);   
    public Task<Result<PaginatedList<WorkSpaceResponse>>> GetAllAsync(FilterRequest filterRequest, CancellationToken cancellationToken = default!);
    public Task<Result<WorkSpaceResponse>> GetByIdAsync(int id, string? userId, CancellationToken cancelationToken);
    public Task<Result<WorkspaceDashboardResponse>> GetWorkspaceDashboardAsync(string UserId, CancellationToken cancellationToken);
    public Task<Result<WorkspaceByIdDashboardResponse>> GetWorkspaceDashboardByIdAsync(int Id, string UserId, CancellationToken cancellationToken = default);
    public Task<Result<WorkSpaceResponse>> UpdateAsync(int Id, string UserId, WorkSpaceRequest requset, CancellationToken cancellationToken);
    public Task<Result> DeleteAsync(int Id, CancellationToken cancellationToken);
    public Task<Result> RestoreAsync(int Id, CancellationToken cancellationToken);
    public Task<Result<IEnumerable<DeletedWorkSpaceResponse>>> GetAllDeletedAsync(string UserId, CancellationToken cancellationToken = default!);
    public Task<Result> AddMemberAsync(int Id, string InviterUserId, InviteMember request, CancellationToken cancellationToken);
    public Task<Result> RemoveMemberAsync(int Id, string RemovedUserId, InviteMember request, CancellationToken cancellationToken = default!);
    public Task<Result<IEnumerable<WorkspaceMemberResponse>>> GetMembersAsync(int Id, string UserId, CancellationToken cancellationToken = default!);
    Task<Result<WorkspaceMemberResponse>> UpdateMemberPermissionsAsync(int Id, string OwnerUserId, string MemberUserId, UpdateWorkspaceMemberPermissionsRequest request,  CancellationToken cancellationToken = default!);
    Task<Result<WorkspaceMemberPermissionsResponse>> GetMemberPermissionsAsync( int Id,string RequesterUserId, string MemberUserId, CancellationToken cancellationToken = default!);
}
