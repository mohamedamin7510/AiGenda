using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI_genda_API.Presistiences.EntitiesConfiguration;

public class HandDrawNoteContentEntityConfiguration : IEntityTypeConfiguration<HandDrawNoteContent>
{
    public void Configure(EntityTypeBuilder<HandDrawNoteContent> builder)
    {
        builder.HasKey(x => x.NoteId);
        builder.Property(x => x.DrawingJson).HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.ExtractedText).HasColumnType("nvarchar(max)");
    }
}