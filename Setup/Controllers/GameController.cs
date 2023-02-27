using Microsoft.AspNetCore.Mvc;
using Setup.Models;

namespace Setup.Controllers;

public class GameController : Controller
{
    // private const string UserId = "UserGroup";

    // GET
    public IActionResult Index()
    {
        return View();
    }

    //POST
    [HttpPost]
    public IActionResult Play(GameSetup setup)
    {
        return View(setup);
    }

    //GET
    public IActionResult Play()
    {
        return Redirect("/game/");
    }

    // //POST
    // [HttpPost]
    // public IActionResult Index(LobbyModel input)
    // {
    //     TryCreateUser(input.UserName);
    //     return Redirect("/Play");
    //     // return View();
    // }

    // public string? TryCreateUser(string user)
    // {
    //
    //     // if (currentCookieValue is null) 
    //     Response.Cookies.Append(UserId, user);
    //     var currentCookieValue = Request.Cookies[UserId];
    //     return currentCookieValue;
    //
    //     // var newCookieValue = short.Parse(currentCookieValue) + 1;
    //     // Response.Cookies.Append(UserId, newCookieValue.ToString());
    //
    //     // Request.Cookies.ContainsKey("Group");
    //     // //create a cookie
    //     // HttpCookie myCookie = new HttpCookie("myCookie");
    //     //
    //     // //Add key-values in the cookie
    //     // myCookie.Values.Add("userid", objUser.id.ToString());
    //     //
    //     // //set cookie expiry date-time. Made it to last for next 12 hours.
    //     // myCookie.Expires = DateTime.Now.AddHours(12);
    //     //
    //     // //Most important, write the cookie to client.
    //     // Response.Cookies.Add(myCookie);
    // }

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