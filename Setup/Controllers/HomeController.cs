using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Setup.Models;

namespace Setup.Controllers;

public class HomeController : Controller
{
    private const string PageViews = "PageViews";
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        UpdatePageViewCookie();
        return View(new HomeModel(Request.Cookies[PageViews]));
    }

    [HttpPost]
    public ActionResult Contact(ContactFormModel input)
    {
        if (!ModelState.IsValid) return View(input);

        return View();
    }

    public IActionResult About()
    {
        return View(new AccountDetails());
    }

    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public void UpdatePageViewCookie()
    {
        var currentCookieValue = Request.Cookies[PageViews];

        if (currentCookieValue == null)
        {
            Response.Cookies.Append(PageViews, "1");
        }
        else
        {
            var newCookieValue = short.Parse(currentCookieValue) + 1;

            Response.Cookies.Append(PageViews, newCookieValue.ToString());
        }
    }
}