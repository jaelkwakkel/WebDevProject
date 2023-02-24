using Microsoft.AspNetCore.Mvc;
using Setup.Models.Account;

namespace Setup.Controllers;

public class AccountController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }

    // GET
    public IActionResult CreateAccount()
    {
        return View();
    }

    // GET
    public IActionResult Login()
    {
        return View();
    }

    //POST
    [HttpPost]
    public IActionResult CreateAccount(AccountModel input)
    {
        if (!ModelState.IsValid) return View(input);

        return Redirect("/");
    }

    //POST
    [HttpPost]
    public IActionResult Login(AccountModel input)
    {
        if (!ModelState.IsValid) return View(input);

        return Redirect("/");
    }
}