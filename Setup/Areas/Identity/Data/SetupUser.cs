using Microsoft.AspNetCore.Identity;

namespace Setup.Areas.Identity.Data;

// Add profile data for application users by adding properties to the SetupUser class
public class SetupUser : IdentityUser
{
    public int highScore { get; set; }

    public List<GameFinishData> FinishedGames { get; set; }
}

