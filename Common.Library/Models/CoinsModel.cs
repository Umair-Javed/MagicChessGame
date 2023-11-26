using Common.Library.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library.Models
{
    public class CoinsModel
    {
        public int Number { get; set; }
        public string? ImgPath { get; set; }
        public PlayerType Type { get; set; }
    }
}
