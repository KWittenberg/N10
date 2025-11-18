namespace N10.DTOs;

public class FilterOptionsDto
{
    public List<int> AvailableYears { get; set; } = new();
    public List<string> AvailableGenres { get; set; } = new();
    public List<string> AvailableResolutions { get; set; } = new();
}