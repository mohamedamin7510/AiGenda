using AI_genda_API.Abstractions.Const;
using BucketSurvey.Api.Errors;

namespace AI_genda_API.Services.RoleService;

public class RoleService(RoleManager<ApplicationRole> roleManager , AppContext context) : IRoleService
{
    private readonly RoleManager<ApplicationRole> _RoleManager = roleManager;
    private readonly AppContext _Context = context;

    public async Task<Result<IEnumerable<RoleResponse>>> GetAllAsync(CancellationToken cancellationToken, bool? hasIncluded = false)
    { 

        var response = await _RoleManager.Roles
        .Where(x => !x.IsDefault && (!x.IsDeleted || (hasIncluded.HasValue && hasIncluded.Value)))
        .ProjectToType<RoleResponse>()
        .ToListAsync(cancellationToken);

        return Result.Success<IEnumerable<RoleResponse>>(response);     
    }
    public async Task<Result<RoleDetailsResponse>> GetAsync(string id)
    {
        if (await _RoleManager.FindByIdAsync(id) is not { } Role)
          return Result.Faluire<RoleDetailsResponse>(RoleErrors.RoleNotFound);

        var permissions = await _RoleManager.GetClaimsAsync(Role);

        var response = new RoleDetailsResponse(Role!.Id , Role.Name! , Role.IsDeleted , permissions.Select(x=> x.Value));

        return Result.Success(response);
    }
    public async Task<Result<RoleDetailsResponse>> AddAsync( RoleRequest request, CancellationToken cancellationToken)
    {
        if (await _RoleManager.RoleExistsAsync(request.Name))
            return Result.Faluire<RoleDetailsResponse>(RoleErrors.RoleDuplicated);

        if (request.Permissions.Except(Permissions.GetAllPerimision).Any())
            return Result.Faluire<RoleDetailsResponse>(RoleErrors.InvalidRole);
       
        var role = new ApplicationRole()
        {
            Name = request.Name,
            IsDefault = false , 
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };

        var result = await _RoleManager.CreateAsync(role); 

        if (result.Succeeded)
        {
          
            var claims = request.Permissions.Select(x => new IdentityRoleClaim<string>()
               {
                    ClaimType = Permissions.Type,
                    ClaimValue = x,
                    RoleId = role.Id
               });

            await _Context.AddRangeAsync(claims);

            await _Context.SaveChangesAsync(cancellationToken);

            var response = new RoleDetailsResponse(role.Id, role.Name!, role.IsDeleted, request.Permissions!);

           return Result.Success(response);
        }

        var error = result.Errors.First();

        return Result.Faluire<RoleDetailsResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> UpdateAsync(string Id , RoleRequest request, CancellationToken cancellationToken)
    {

        if (await _RoleManager.FindByIdAsync(Id) is not { } Role)
            return Result.Faluire(RoleErrors.RoleNotFound);

        if (await _RoleManager.Roles.AnyAsync(x =>x.Name == request.Name && x.Id != Id ))
            return Result.Faluire<RoleDetailsResponse>(RoleErrors.RoleDuplicated);

        var AllValidPermissions = Permissions.GetAllPerimision;

        if (request.Permissions.Except(AllValidPermissions) is not { })
            return Result.Faluire(RoleErrors.InvalidRole);

        Role.Name = request.Name;

        var result = await _RoleManager.UpdateAsync(Role);

        if (result.Succeeded)
        {

            var CurrentPermissions =  _RoleManager.GetClaimsAsync(Role).Result.Select(x=>x.Value).ToList();

            var newPermissions = request.Permissions.Except(CurrentPermissions)
            .Select(x => new IdentityRoleClaim<string>()
            {
                ClaimType = Permissions.Type,
                ClaimValue = x,
                RoleId = Role.Id
            }
            );
               

            var removedPermissions = CurrentPermissions.Except(request.Permissions);

            await _Context.AddRangeAsync(newPermissions);

            await _Context.RoleClaims
                .Where(x => x.RoleId == Id && removedPermissions.Contains(x.ClaimValue) && x.ClaimType == Permissions.Type)
                .ExecuteDeleteAsync(cancellationToken);

           await _Context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        var error = result.Errors.First();

        return Result.Faluire<RoleDetailsResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }
    public async Task<Result> ToggleStatusAsync(string Id)
    {

        if (await _RoleManager.FindByIdAsync(Id) is not { } role)
            return Result.Faluire(RoleErrors.RoleNotFound);

        role.IsDeleted = !role.IsDeleted;

        await  _RoleManager.UpdateAsync(role);

        return Result.Success();
    }


}
