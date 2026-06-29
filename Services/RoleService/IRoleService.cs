namespace AI_genda_API.Services.RoleService;

public interface IRoleService
{
    Task<Result<RoleDetailsResponse>> AddAsync ( RoleRequest request , CancellationToken cancellationToken);
    Task<Result<IEnumerable<RoleResponse>>> GetAllAsync(CancellationToken cancellationToken, bool? hasIncluded);
    Task<Result<RoleDetailsResponse>> GetAsync(string id);
    Task<Result> UpdateAsync(string Id, RoleRequest request, CancellationToken cancellationToken);
    Task<Result> ToggleStatusAsync(string Id);

}
