using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI_genda_API.Presistiences.EntitiesConfiguration;

public class ImageNoteContentEntityConfiguration : IEntityTypeConfiguration<ImageNoteContent>
{
    public void Configure(EntityTypeBuilder<ImageNoteContent> builder)
    {
        builder.HasKey(x => x.NoteId);
        builder.Property(x => x.OcrText).HasColumnType("nvarchar(max)");
        builder.Property(x => x.Caption).HasMaxLength(500);
    }
}