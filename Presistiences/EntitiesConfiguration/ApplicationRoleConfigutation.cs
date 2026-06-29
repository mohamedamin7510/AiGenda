using AI_genda_API.Abstractions.Const;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI_genda_API.Presistiences.EntitiesConfiguration;

public class ApplicationRoleConfigutation : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {   

        //Default Roles 
        builder.HasData(
            [
                new ApplicationRole
                {
                    Id = DefaultRoles.AdminRoleId,
                    Name = DefaultRoles.Admin,
                    NormalizedName = DefaultRoles.Admin.ToUpper(),
                    ConcurrencyStamp = DefaultRoles.AdminConcurrencyStamp,
                    IsDeleted = false,
                    IsDefault = false
                },
                new ApplicationRole
                {
                    Id = DefaultRoles.MemberID,
                    Name = DefaultRoles.Member,
                    NormalizedName = DefaultRoles.Member.ToUpper(),
                    ConcurrencyStamp= DefaultRoles.MemberConcurrencyStamp,
                    IsDeleted = false,
                    IsDefault = true
                }
             ]

            );

    }
}
