using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI_genda_API.Presistiences.EntitiesConfiguration;

public class VoiceNoteContentEntityConfiguration : IEntityTypeConfiguration<VoiceNoteContent>
{
    public void Configure(EntityTypeBuilder<VoiceNoteContent> builder)
    {
        builder.HasKey(x => x.NoteId);

        builder.Property(x => x.TranscriptText).HasColumnType("nvarchar(max)");
    }
}