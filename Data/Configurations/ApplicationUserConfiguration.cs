namespace N10.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.AvatarUrl).HasMaxLength(ApplicationUserConst.AvatarUrlLength);


        builder.Property(x => x.FirstName).HasMaxLength(ApplicationUserConst.FirstNameLength);

        builder.Property(x => x.LastName).HasMaxLength(ApplicationUserConst.LastNameLength);

        builder.Property(x => x.CompanyName).HasMaxLength(ApplicationUserConst.CompanyNameLength);


        builder.Property(x => x.Country).HasMaxLength(ApplicationUserConst.CountryLength);

        builder.Property(x => x.Zip).HasMaxLength(ApplicationUserConst.ZipLength);

        builder.Property(x => x.City).HasMaxLength(ApplicationUserConst.CityLength);

        builder.Property(x => x.Street).HasMaxLength(ApplicationUserConst.StreetLength);


        builder.Property(x => x.PlaceId).HasMaxLength(ApplicationUserConst.PlaceIdLength);


        //Ako često pretražujete po PlaceId, Country, ili Zip, razmislite o dodavanju indeksa!
        //builder.HasIndex(x => x.PlaceId);
        //builder.HasIndex(x => x.Country);
    }
}