namespace Setup.Areas.Identity.Data
{
    public class GameFinishData
    {
        public string Id { get; set; }
        public int Score { get; set; }
        public bool WonGame { get; set; }
        public string WinnerName { get; set; }
    }
}