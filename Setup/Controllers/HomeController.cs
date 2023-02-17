using Microsoft.AspNetCore.Mvc;
using SendGrid;
using SendGrid.Helpers.Mail;
using Setup.Models;
using System.Diagnostics;

namespace Setup.Controllers;

public class HomeController : Controller
{
    private const string PageViews = "PageViews";

    private readonly ContactFormDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ContactFormDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
    }

    // public HomeController(ContactFormDbContext context)
    // {
    //     _context = context;
    // }
    //
    // public HomeController(ILogger<HomeController> logger)
    // {
    //     _logger = logger;
    // }

    public IActionResult Index()
    {
        UpdatePageViewCookie();
        return View(new HomeModel(Request.Cookies[PageViews]));
    }

    [HttpPost]
    public ActionResult Contact(ContactFormModel input)
    {
        if (!ModelState.IsValid) return View(input);

        CreateNewContact(input);

        return View();
    }

    private void CreateNewContact(ContactFormModel input)
    {
        // using var db = new ContactFormDbContext(null);
        _context.ContactFormModels.Add(input);
        _context.SaveChanges();

        SendMail().Wait();
    }

    static async Task SendMail()
    {
        var apiKey = Environment.GetEnvironmentVariable("WebdevProject");
        var client = new SendGridClient(apiKey);
        var from = new EmailAddress("contactwebdevformmail@gmail.com", "Example User");
        var subject = "Sending with SendGrid is Fun";
        var to = new EmailAddress("contactwebdevformmail@gmail.com", "Example User");
        var plainTextContent = "and easy to do anywhere, even with C#";
        var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
        var response = await client.SendEmailAsync(msg);
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