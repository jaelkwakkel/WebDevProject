namespace Setup.Models;

public class HomeModel
{
    public HomeModel(string? amount)
    {
        AmountOfPageViews = amount ?? "0";
    }

    public string AmountOfPageViews;
}