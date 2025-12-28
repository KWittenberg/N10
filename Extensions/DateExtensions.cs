namespace N10.Extensions;

public static class DateExtensions
{
    // Extension metoda na 'int'
    // Korištenje: @SelectedDate.Month.ToCroatianMonthName()
    public static string ToCroatianMonthName(this int month) => month switch
    {
        1 => "Siječnja",
        2 => "Veljače",
        3 => "Ožujka",
        4 => "Travnja",
        5 => "Svibnja",
        6 => "Lipnja",
        7 => "Srpnja",
        8 => "Kolovoza",
        9 => "Rujna",
        10 => "Listopada",
        11 => "Studenoga",
        12 => "Prosinca",
        _ => ""
    };

    // OPTIONAL: Extension metoda direktno na DateTime/DateOnly za puni ispis
    // Korištenje: @SelectedDate.ToCroatianDateString() -> "24. Prosinca"
    public static string ToCroatianDateString(this DateTime date)
    {
        return $"{date.Day}. {date.Month.ToCroatianMonthName()}";
    }
}