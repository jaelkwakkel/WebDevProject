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

public class HomeController : Controller
{
    private const string PageViews = "PageViews";

    private readonly SetupContext _context;
    private readonly UserManager<SetupUser> _userManager;
    private readonly ILogger<HomeController> _logger;

    public HomeController(SetupContext context, ILogger<HomeController> logger, UserManager<SetupUser> userManager)
    {
        _userManager = userManager;
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        UpdatePageViewCookie();
        return View(new HomeModel(Request.Cookies[PageViews]));
    }

    [Authorize]
    public async Task<IActionResult> ScoresAsync()
    {
        ScoreInfo info = await RetrieveScoreDataAsync();
        return View(info);
    }

    private async Task<ScoreInfo> RetrieveScoreDataAsync()
    {
        SetupUser user = await _userManager.GetUserAsync(User);

        Console.WriteLine(user);
        user.FinishedGames.ForEach(x => Console.WriteLine(x.WinnerName));
        //SetupUser? setupUser = _userManager.Users.FirstOrDefault(u => u.Id == User.Identity.GetUserId());
        return new ScoreInfo()
        {
            highScore = user.highScore,
            FinishedGames = user.FinishedGames,
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

        //_context.ContactFormModels.Add(input);
        _context.SaveChanges();
    }

    private static async Task SendMail(ContactFormModel input)
    {
        //Sanitize input before doing anything with it.
        var sanitizer = new HtmlSanitizer();
        var sanitizedEmail = sanitizer.Sanitize(input.Email);
        var sanitizedSubject = sanitizer.Sanitize(input.Subject);
        var sanitizedMessage = sanitizer.Sanitize(input.Message);

        var apiKey = Environment.GetEnvironmentVariable("WebdevProject");
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("contactwebdevformmail@gmail.com", sanitizedEmail);
        var subject = sanitizedSubject;
        var to = new EmailAddress("contactwebdevformmail@gmail.com", "Contact");
        var plainTextContent = sanitizedMessage;
        var htmlContent = sanitizedMessage;
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
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