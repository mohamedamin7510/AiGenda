using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI_genda_API.Presistiences.EntitiesConfiguration;

public class NoteEntityConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(x => new { x.SpaceId, x.RemovedAt, x.CreatedAt });

        builder.HasOne(x => x.Space)
            .WithMany(x => x.Notes)
            .HasForeignKey(x => x.SpaceId);

        builder.HasOne(x => x.TextContent)
            .WithOne(x => x.Note)
            .HasForeignKey<TextNoteContent>(x => x.NoteId);

        builder.HasOne(x => x.VoiceContent)
            .WithOne(x => x.Note)
            .HasForeignKey<VoiceNoteContent>(x => x.NoteId);

        builder.HasOne(x => x.ImageContent)
            .WithOne(x => x.Note)
            .HasForeignKey<ImageNoteContent>(x => x.NoteId);

        builder.HasOne(x => x.HandDrawContent)
            .WithOne(x => x.Note)
            .HasForeignKey<HandDrawNoteContent>(x => x.NoteId);

        builder.HasMany(x => x.Assets)
            .WithOne(x => x.Note)
            .HasForeignKey(x => x.NoteId);
    }
}