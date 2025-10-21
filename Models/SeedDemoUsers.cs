namespace N10.Models;

public class SeedDemoUsers
{
    public List<string>? FirstNames { get; set; }

    public List<string>? LastNames { get; set; }

    public List<Location>? Locations { get; set; }



    public class Location
    {
        public string Country { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public string PlaceId { get; set; } = string.Empty;
    }
}