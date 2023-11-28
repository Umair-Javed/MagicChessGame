namespace WebFront.Models
{
    [Serializable]
    public class GameIndexModel
    {
        public string? MainPlayer { get; set; }
        public string? Opponent { get; set; }
        public string? GroupId { get; set; }
        public string? SessionId { get; set; }
    }
}
