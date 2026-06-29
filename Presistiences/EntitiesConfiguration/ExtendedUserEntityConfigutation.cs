using AI_genda_API.Abstractions.Const;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI_genda_API.Presistiences.EntitiesConfiguration;

public class ExtendedUserEntityConfigutation : IEntityTypeConfiguration<ExtendedUser>
{
    public void Configure(EntityTypeBuilder<ExtendedUser> builder)
    {
     
       builder.Property(eu => eu.FirstName)
              .HasMaxLength(20);

       builder.Property(eu => eu.SecondName)
              .HasMaxLength(20);
  
        builder.
            OwnsMany(x => x.RefreshTokens)
            .ToTable("RefreshTokens") 
            .WithOwner().HasForeignKey("UserId");

        builder.Property(eu => eu.JobTitle)
              .HasMaxLength(50);

        builder.Property(eu => eu.AvatarUrl)
              .HasMaxLength(500);

           builder
             .ToTable(t =>
            t.HasCheckConstraint(
                "CK_ExtenededUser_DueDate_Past",
                "[DateOfBirth] < CAST(GETDATE() AS date)"
            ));


        var PasswordHasher = new PasswordHasher<ExtendedUser>();
        //Default User 
        builder.HasData(
            [
            new ExtendedUser
            {
                Id = DefaultUsers.AdminId,
                FirstName = DefaultUsers.AdminFirstName,
                SecondName = DefaultUsers.AdminSecondName,
                Email = DefaultUsers.AdminEmail,
                NormalizedEmail = DefaultUsers.AdminEmail.ToUpper(),
                UserName = DefaultUsers.AdminEmail,
                NormalizedUserName = DefaultUsers.AdminEmail.ToUpper(),
                PasswordHash = PasswordHasher.HashPassword(null!, DefaultUsers.AdminPassword),
                SecurityStamp = DefaultUsers.AdminSecurityStamp,
                ConcurrencyStamp = DefaultUsers.AdminConcurrencyStamp,
                PhoneNumber = DefaultUsers.AdminPhoneNumber,
                EmailConfirmed = true,
                PhoneNumberConfirmed= true,
                SubscriptionType = DefaultUsers.AdminSubscriptionType,
                JobTitle = DefaultUsers.AdminJobTittle,
                DateOfBirth = DateOnly.FromDateTime(DateTime.Parse(DefaultUsers.AdminDateOfBirth)),
                
            }

            ]

            );




    }
}
