using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Setup.Areas.Identity.Data;

namespace Setup.Controllers;

public class RoleController : Controller
{
    private readonly UserManager<SetupUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<RoleController> _logger;

    public RoleController(UserManager<SetupUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<RoleController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    [Authorize(Roles = "admin")]
    public IActionResult RoleManager()
    {
        ViewBag.Users = _userManager.Users.Select(x => x.UserName);

        return View();
    }

    [Authorize(Roles = "moderator")]
    public IActionResult GameManager()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> OnPost(IFormCollection collection)
    {

        var user = await _userManager.FindByNameAsync(collection["users"]);
        bool deleteUser = !collection["deleteUserCheckbox"].ToString().IsNullOrEmpty();

        Console.WriteLine("deleteUser: " + deleteUser + " | " + (deleteUser ? "TRUE" : "FALSE"));

        ViewBag.Users = _userManager.Users.Select(x => x.UserName);

        if (user is null)
        {
            ViewBag.Message = collection["users"] + " is not a user";
            return View("RoleManager");
        }

        if (!await _roleManager.RoleExistsAsync(collection["roles"]))
        {
            if (collection["roles"] == "user" || collection["roles"] == "moderator" || collection["roles"] == "admin")
            {
                //Create role if not exist, only if role is user, moderator or admin
                await _roleManager.CreateAsync(new IdentityRole(collection["roles"]));
            }
        }

        if (deleteUser)
        {
            await _userManager.DeleteAsync(user);
            ViewBag.Message = "Deleted user " + collection["users"];
            _logger.LogInformation("Moderator deleted user with Id: '{UserId}'", user.Id);
        }
        else
        {
            await _userManager.AddToRolesAsync(user, collection["roles"]);
            ViewBag.Message = "Changed role of " + collection["users"] + " to " + collection["roles"];
            _logger.LogInformation("changed role of user with Id: '{UserId}' to '{Role}'", user.Id, collection["roles"]);
        }

        return View("RoleManager");
    }
}
