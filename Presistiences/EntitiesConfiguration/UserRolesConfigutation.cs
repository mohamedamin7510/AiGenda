using AI_genda_API.Abstractions.Const;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI_genda_API.Presistiences.EntitiesConfiguration;

public class UserRolesConfigutation : IEntityTypeConfiguration<IdentityUserRole<string>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
    {
        //Assigning default values to the admin role and user
        builder.HasData(
             
             new IdentityUserRole<string>
             {
                 RoleId = DefaultRoles.AdminRoleId,
                 UserId = DefaultUsers.AdminId               
             }
                
            );
            }
}
