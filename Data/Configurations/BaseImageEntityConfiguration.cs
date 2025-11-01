namespace N10.Data.Configurations;

public class BaseImageEntityConfiguration : IEntityTypeConfiguration<BaseImageEntity>
{
    public void Configure(EntityTypeBuilder<BaseImageEntity> entity)
    {
        // Postavljamo TPC mapping - ovo će osigurati da se base ne mapira u tablicu, već samo derived
        entity.UseTpcMappingStrategy();

        entity.Property(e => e.FileName).HasMaxLength(BaseImageEntityConst.FileNameLength).IsRequired();

        entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(BaseImageEntityConst.TypeLength).IsRequired();

        entity.Property(e => e.FileUrl).HasMaxLength(BaseImageEntityConst.FileUrlLength).IsRequired();

        entity.Property(e => e.Width);

        entity.Property(e => e.Height);

        entity.Property(e => e.FileSize);

        entity.Property(e => e.MimeType).HasMaxLength(BaseImageEntityConst.MimeTypeLength);
    }
}