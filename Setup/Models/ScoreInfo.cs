using Setup.Areas.Identity.Data;

namespace Setup.Models;

public class ScoreInfo
{
    public int HighScore { get; set; }
    public List<GameFinishData> FinishedGames { get; set; }
}