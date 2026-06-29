using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = AI_genda_API.Entities.Task;

namespace AI_genda_API.Presistiences.EntitiesConfiguration;

public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {

        builder.HasKey(t => t.Id);

        builder.HasOne(x => x.Space)
            .WithMany(x => x.Tasks)
            .HasForeignKey(x => x.SpaceId);

        builder.Property(x => x.Title)
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(400);
    }

}