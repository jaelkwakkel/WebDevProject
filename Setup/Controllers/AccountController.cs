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

    [HttpPost]
    public IActionResult CreateAccount(AccountModel input)
    {
        if (!ModelState.IsValid) return View(input);

        return View();
    }

    // GET
    public IActionResult CreateAccount()
    {
        return View();
    }
}