using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using Setup.Areas.Identity.Data;
using Setup.Models;
using System.Diagnostics;

namespace Setup.Controllers;

[Authorize]
public class HomeController : Controller
{
    //private const string PageViews = "PageViews";

    private readonly SetupContext _context;
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<SetupUser> _userManager;

    public HomeController(SetupContext context, ILogger<HomeController> logger, UserManager<SetupUser> userManager)
    {
        _userManager = userManager;
        _logger = logger;
        _context = context;
    }

    [AllowAnonymous]
    public IActionResult Index()
    {
        //UpdatePageViewCookie();
        //return View(new HomeModel(Request.Cookies[PageViews]));
        return View();
    }

    public async Task<IActionResult> ScoresAsync()
    {
        var info = await RetrieveScoreDataAsync();
        return View(info);
    }

    private async Task<ScoreInfo> RetrieveScoreDataAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        return new ScoreInfo
        {
            HighScore = user.HighScore,
            FinishedGames = _context.Entry(user)
                .Collection(b => b.FinishedGames)
                .Query()
                .ToList()
        };
    }

    [HttpPost]
    public IActionResult Contact(ContactFormModel input)
    {
        if (!ModelState.IsValid) return View(input);

        CreateNewContact(input);

        return View();
    }

    private void CreateNewContact(ContactFormModel input)
    {
        SendMail(input).Wait();
        _context.SaveChanges();
    }

    private static async Task SendMail(ContactFormModel input)
    {
        if (input.Email is null || input.Subject is null || input.Message is null) return;

        //Sanitize input before doing anything with it.
        var sanitizer = new HtmlSanitizer();
        var sanitizedEmail = sanitizer.Sanitize(input.Email);
        var sanitizedSubject = sanitizer.Sanitize(input.Subject);
        var sanitizedMessage = sanitizer.Sanitize(input.Message);

        var apiKey = Environment.GetEnvironmentVariable("WebdevProject");
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("contactwebdevformmail@gmail.com", sanitizedEmail);
        var to = new EmailAddress("contactwebdevformmail@gmail.com", "Contact");
        var plainTextContent = sanitizedMessage;
        var msg = MailHelper.CreateSingleEmail(from, to, sanitizedSubject, plainTextContent, sanitizedMessage);
        await client.SendEmailAsync(msg);
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

    //public void UpdatePageViewCookie()
    //{
    //    var currentCookieValue = Request.Cookies[PageViews];

    //    if (currentCookieValue == null)
    //    {
    //        Response.Cookies.Append(PageViews, "1");
    //    }
    //    else
    //    {
    //        var newCookieValue = short.Parse(currentCookieValue) + 1;

    //        Response.Cookies.Append(PageViews, newCookieValue.ToString());
    //    }
    //}
}