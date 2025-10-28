namespace N10.Data.Configurations;

public class BaseImageEntityConfiguration : IEntityTypeConfiguration<BaseImageEntity>
{
    public void Configure(EntityTypeBuilder<BaseImageEntity> entity)
    {
        // Postavljamo TPC mapping - ovo će osigurati da se base ne mapira u tablicu, već samo derived
        entity.UseTpcMappingStrategy();

        entity.Property(e => e.FileName).IsRequired().HasMaxLength(BaseImageEntityConst.FileNameLength);

        entity.Property(e => e.Type).IsRequired();

        entity.Property(e => e.FileUrl).IsRequired().HasMaxLength(BaseImageEntityConst.FileUrlLength);

        entity.Property(e => e.Width);

        entity.Property(e => e.Height);

        entity.Property(e => e.FileSize);

        entity.Property(e => e.MimeType).HasMaxLength(BaseImageEntityConst.MimeTypeLength);
    }
}
