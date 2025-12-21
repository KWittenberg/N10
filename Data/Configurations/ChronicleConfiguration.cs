namespace N10.Data.Configurations;

public class ChronicleConfiguration : IEntityTypeConfiguration<Chronicle>
{
    public void Configure(EntityTypeBuilder<Chronicle> builder)
    {
        builder.ToTable("Chronicles");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Date).IsRequired(false);

        builder.Property(x => x.Title).HasMaxLength(255).IsRequired(false);

        builder.Property(x => x.Content).HasColumnType("nvarchar(max)").IsRequired();

        // Type - Enum
        // Po defaultu sprema int (0, 1, 2...). 
        // Ako želiš da u bazi piše "Historical", "Personal" (kao string),
        // otkomentiraj liniju .HasConversion<string>();
        builder.Property(x => x.Type).IsRequired(); // .HasConversion<string>(); 

        // Indeksi - Kasnije ćeš sigurno tražiti po datumu ("Daj mi sve za 1. siječnja")
        // Pa je pametno odmah staviti indeks na datum radi brzine.
        builder.HasIndex(x => x.Date);
    }
}