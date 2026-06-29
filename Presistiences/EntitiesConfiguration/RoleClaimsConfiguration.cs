using AI_genda_API.Abstractions.Const;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BucketSurvey.Api.Presistence.EntitiesConfigurations;

public class RoleClaimsConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<string>>
{

    public void Configure(EntityTypeBuilder<IdentityRoleClaim<string>>  builder)
    {

        var AllPermissions = Permissions.GetAllPerimision;

        var adminClaims = new List<IdentityRoleClaim<string>>();

        for (int i = 0; i < AllPermissions.Count(); i++)
        {
            adminClaims.Add(new IdentityRoleClaim<string>
            {
                Id = i + 1,
                RoleId = DefaultRoles.AdminRoleId,
                ClaimType = Permissions.Type,
                ClaimValue = AllPermissions[i],
            });
        }


        builder.HasData(adminClaims);
            
            
    }
}
