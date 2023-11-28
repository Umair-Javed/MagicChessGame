using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Common.Library.Enums;
using Common.Library.Models;

namespace WebFront.Models
{
    // View model for the Chess game
    public class ChessViewModel
    {
        #region Chess Board Configuration

        public int RowSize { get; set; } = 4;
        public string? FlippedIconUrl { get; set; } = "/Content/Images/flipped.png";

        #endregion

        #region Player Information

        public PlayerModel? MainPlayer { get; set; }
        public PlayerModel? OpponentPlayer { get; set; }

        #endregion

        #region Game State

        public string? ChessBoardHtml { get; set; }
        public string? GroupId { get; set; }
        public string? SessionId { get; set; }
        public bool IsGameStarted { get; set; }
        public bool IsNewSession { get; set; } = true;
        public bool IsDisabled { get; set; } = true;

        #endregion

        #region Coin Configuration

        public List<CoinsModel> Coins { get; set; }

        #endregion
    }

}
