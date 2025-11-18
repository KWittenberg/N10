namespace N10.DTOs;

public class MovieFilter
{
    public string? SearchTerm { get; set; }
    public string? Letter { get; set; }
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    public List<string>? Genres { get; set; }
    public List<string>? Resolutions { get; set; }
    public string? SortBy { get; set; } = "title";
    public bool SortDescending { get; set; } = false;
}