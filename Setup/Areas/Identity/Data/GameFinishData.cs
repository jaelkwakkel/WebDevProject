using System.ComponentModel.DataAnnotations;

namespace Setup.Areas.Identity.Data
{
    public class GameFinishData
    {
        public GameFinishData()
        {
            Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }
        public int Score { get; set; }
        public bool WonGame { get; set; }
        public string WinnerName { get; set; }
    }
}