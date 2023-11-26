using Common.Library.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library.Models
{
    public class PlayerModel
    {
        public string? Name { get; set; }
        public PlayerType Type { get; set; }
        public bool IsMyTurn { get; set; }
        public bool IsCoinExposed { get; set; }
        public string? UserIcon { get; set; }
    }
}
