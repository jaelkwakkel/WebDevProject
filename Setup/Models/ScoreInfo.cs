using Setup.Areas.Identity.Data;

namespace Setup.Models
{
    public class ScoreInfo
    {
        public int highScore { get; set; }
        public List<GameFinishData> FinishedGames { get; set; }
    }
}
