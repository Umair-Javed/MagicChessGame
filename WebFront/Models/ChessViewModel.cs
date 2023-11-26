using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Common.Library.Enums;
using Common.Library.Models;

namespace WebFront.Models
{
    public class ChessViewModel
    {
        public int RowSize { get; set; } = 4;
        public string? FlippedIconUrl { get; set; } = "/Content/Images/flipped.png";
        public PlayerModel? MainPlayer { get; set; }
        public PlayerModel? OpponentPlayer { get; set; }
        public string? ChessBoardHtml { get; set; }
        public string? GroupId { get; set; }
        public string? SessionId { get; set; }
        public bool IsGameStarted { get; internal set; }
        public bool IsNewSession { get; set; } = true; 
        public bool IsDisabled { get; internal set; } = true;
        public List<CoinsModel> Coins { get; set; }
    }
}
