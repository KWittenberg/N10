namespace N10.Data.Configurations;

public class BaseAttachmentEntityConfiguration : IEntityTypeConfiguration<BaseAttachmentEntity>
{
    public void Configure(EntityTypeBuilder<BaseAttachmentEntity> entity)
    {
        // Postavljamo TPC mapping - ovo će osigurati da se base ne mapira u tablicu, već samo derived
        entity.UseTpcMappingStrategy();

        entity.Property(e => e.FileName).IsRequired().HasMaxLength(BaseAttachmentEntityConst.FileNameLength);

        entity.Property(e => e.FileUrl).IsRequired().HasMaxLength(BaseAttachmentEntityConst.FileUrlLength);

        entity.Property(e => e.MimeType).HasMaxLength(BaseAttachmentEntityConst.MimeTypeLength);

        entity.Property(e => e.FileSize);

        entity.Property(e => e.IsEncrypted).IsRequired();
    }
}
