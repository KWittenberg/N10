namespace N10.Models;

public class Background
{
    public Background() { }

    public Background(string name, string imageUrl)
    {
        Name = name;
        ImageUrl = imageUrl;
    }

    public string Name { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;


    public static List<Background> GetAll() =>
    [
        new("Default", "img/bg/bg-image1.jpg"),
        new("Solaris", "img/bg/1.Solaris.jpg"),
        new("Galaxy", "img/bg/2.Galaxy.jpg"),
        new("Red Velvet", "img/bg/4.Red Velvet.jpg"),
        new("Desert", "img/bg/6.Northern Light.jpg"),
        new("Sunset", "img/bg/7.Sunset.jpg"),
        new("Kinetic", "img/bg/8.Kinetic.jpg"),
        new("Simple Gray", "img/bg/9.Simple Gray.jpg")
    ];
}