using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
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

        StringValues unsafeUserName = collection["users"];
        StringValues unsafeRoleName = collection["roles"];
        if (unsafeUserName.IsNullOrEmpty())
        {
            ViewBag.Message = unsafeUserName + " is null or empty.";
            return View("RoleManager");
        }

        HtmlSanitizer htmlSanitizer = new();
        string userName = htmlSanitizer.Sanitize(unsafeUserName.ToString());
        string roleName = htmlSanitizer.Sanitize(unsafeRoleName.ToString());

        var user = await _userManager.FindByNameAsync(userName);
        bool deleteUser = !collection["deleteUserCheckbox"].ToString().IsNullOrEmpty();

        Console.WriteLine("deleteUser: " + deleteUser + " | " + (deleteUser ? "TRUE" : "FALSE"));

        ViewBag.Users = _userManager.Users.Select(x => x.UserName);

        if (user is null)
        {
            ViewBag.Message = userName + " is not a user";
            return View("RoleManager");
        }

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            if (roleName == "user" || roleName == "moderator" || roleName == "admin")
            {
                //Create role if not exist, only if role is user, moderator or admin
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        if (deleteUser)
        {
            await _userManager.DeleteAsync(user);
            ViewBag.Message = "Deleted user " + userName;
            _logger.LogInformation("Moderator deleted user with Id: '{UserId}'", user.Id);
        }
        else
        {
            await _userManager.AddToRoleAsync(user, roleName);
            ViewBag.Message = "Changed role of " + userName + " to " + roleName;
            _logger.LogInformation("changed role of user with Id: '{UserId}' to '{Role}'", user.Id, roleName);
        }

        return View("RoleManager");
    }
}
