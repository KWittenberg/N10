namespace N10.Models;

public class Color
{
    public Color() { }

    public Color(string name, string hex)
    {
        Name = name;
        Hex = hex;
    }

    public string Name { get; set; } = string.Empty;

    public string Hex { get; set; } = string.Empty;


    public static List<Color> GetAll() =>
    [
        new("blue", "#0d6efd"),
        new("indigo", "#6610f2"),
        new("purple", "#6f42c1"),
        new("pink", "#d63384"),
        new("red", "#dc3545"),
        new("orange", "#fd7e14"),
        new("yellow", "#ffc107"),
        new("green", "#198754"),
        new("teal", "#20c997"),
        new("cyan", "#0dcaf0"),
        new("gray", "#adb5bd"),
        new("black", "#000"),
        new("white", "#fff")
    ];
}