using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AI_genda_API.Presistiences.EntitiesConfiguration;

public class NoteAssetEntityConfiguration : IEntityTypeConfiguration<NoteAsset>
{
    public void Configure(EntityTypeBuilder<NoteAsset> builder)
    {
        builder.Property(x => x.FileName)
            .HasMaxLength(300)
            .IsRequired();


        builder.Property(x => x.StorageUrl)
            .HasMaxLength(1000)
            .IsRequired();
        
        builder.Property(x => x.MimeType)
            .HasMaxLength(120)
            .IsRequired();

        builder.HasIndex(x => new { x.NoteId, x.AssetType });
    }
}