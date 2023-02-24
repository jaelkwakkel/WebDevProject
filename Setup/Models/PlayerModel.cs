namespace Setup.Models
{
    public class PlayerModel
    {
        public string ConnectionId { get; set; }
        public string Name { get; set; }
        public string? Group { get; set; }

        public PlayerModel(string name, string connectionId)
        {
            Name = name;
            ConnectionId = connectionId;
        }

        public PlayerModel(string name, string group, string connectionId) : this(name, connectionId)
        {
            Group = group;
        }
    }
}
