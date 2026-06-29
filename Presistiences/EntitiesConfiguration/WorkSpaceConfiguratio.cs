using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI_genda_API.Presistiences.EntitiesConfiguration;

public class WorkSpaceConfiguratio : IEntityTypeConfiguration<WorkSpace>
{
    public void Configure(EntityTypeBuilder<WorkSpace> builder)
    {

        builder.HasKey(x => x.Id);

        builder.HasMany(x => x.workspaceMembers)
            .WithOne(y => y.WorkSpaces)
            .HasForeignKey(x=>x.WrokSpaceID);

        builder.Property(x => x.Name).
             IsRequired(true)
              .HasMaxLength(60);

        builder.Property(x => x.Description)
              .HasMaxLength(300);

        builder.Property(x => x.IconCode)
              .HasMaxLength(500);
    }
}
