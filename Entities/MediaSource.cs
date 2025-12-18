namespace N10.Entities;

public class MediaSource : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string BasePath { get; set; } = string.Empty;

    public MediaType Type { get; set; }


    public ICollection<Movie> Movies { get; set; } = new List<Movie>();
}