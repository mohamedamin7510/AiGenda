using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI_genda_API.Presistiences.EntitiesConfiguration;

public class TextNoteContentEntityConfiguration : IEntityTypeConfiguration<TextNoteContent>
{
    public void Configure(EntityTypeBuilder<TextNoteContent> builder)
    {
        builder.HasKey(x => x.NoteId);

        builder.Property(x => x.PlainText)
            .HasMaxLength(5000).IsRequired();

        builder.Property(x => x.HtmlContent)
            .HasColumnType("nvarchar(max)").IsRequired();

        builder.Property(x => x.RichTextJson)
            .HasColumnType("nvarchar(max)");
    }
}