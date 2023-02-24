using Microsoft.AspNetCore.Mvc;

namespace Setup.Controllers;

public class GameController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }

    public string GetBoard()
    {
        var board = "";
        for (var i = 0; i < 10; i++)
        {
            board += "<div class='row'>";
            for (var j = 0; j < 10; j++) board += "<div class='col'>Column</div>";
            board += "</div>";
        }

        return board;
    }
}