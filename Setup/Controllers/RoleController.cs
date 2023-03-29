using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Setup.Areas.Identity.Data;

namespace Setup.Controllers;

public class RoleController : Controller
{
    private readonly UserManager<SetupUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleController(UserManager<SetupUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [Authorize(Roles = "Admin")]
    public IActionResult RoleManager()
    {
        ViewBag.Users = _userManager.Users.Select(x => x.UserName);

        return View();
    }

    [Authorize(Roles = "Moderator")]
    public IActionResult GameManager()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> OnPost(IFormCollection collection)
    {

        var user = await _userManager.FindByNameAsync(collection["users"]);
        ViewBag.Users = _userManager.Users.Select(x => x.UserName);

        if (user is null)
        {
            ViewBag.Message = collection["users"] + " is not a user";
            return View("RoleManager");
        }

        if (_roleManager.RoleExistsAsync(collection["roles"]) == null &&
            collection["roles"] == "User" ||
            collection["roles"] == "Moderator" ||
            collection["roles"] == "Admin")
        {
            await _roleManager.CreateAsync(new IdentityRole(collection["roles"]));
        }

        await _userManager.AddToRolesAsync(user, collection["roles"]);

        ViewBag.Message = "Changed role of " + collection["users"] + " to " + collection["roles"];
        return View("RoleManager");
    }
}
