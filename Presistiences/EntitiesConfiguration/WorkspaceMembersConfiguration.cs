using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI_genda_API.Presistiences.EntitiesConfiguration;

public class WorkspaceMembersConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.HasKey(x => new {x.WrokSpaceID , x.UserID });

        builder.HasOne(x => x.WorkSpaces)
            .WithMany(x=>x.workspaceMembers);
    }
}
