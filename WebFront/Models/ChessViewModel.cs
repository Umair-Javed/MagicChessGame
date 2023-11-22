namespace WebFront.Models
{
    public class ChessViewModel
    {
        public int RowSize { get; set; } = 4;
        public string FlippedIconUrl { get; set; }
        public Player MainPlayer { get; set; }
        public Player OpponentPlayer { get; set; }
        public List<CoinsModel> Coins { get; set; }
    }

    public class Player
    {
        public string Name { get; set; }
        public PlayerType Type { get; set; }
        public bool IsMyTurn { get; set; }
        public bool IsCoinExposed { get; set; }
        public string UserIcon { get; set; }
    }

    public class CoinsModel
    {
        public int Number { get; set; }
        public string ImgPath { get; set; }
        public PlayerType Type { get; set; }
    }

    public enum PlayerType
    {
        MAIN = 1,
        OPPONENT =2
    }

    public enum enCoins
    {
        PAWN =1,
        CANNON =2,
        KNIGHT =3,
        ROOK =4,
        BISHOP =5,
        ADVISOR =6,
        KING =7
    }
}
