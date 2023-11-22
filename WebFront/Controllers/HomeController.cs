using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using WebFront.Models;

namespace WebFront.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var model = new ChessViewModel();
            model.FlippedIconUrl = "/Content/Images/flipped.png";

            model.MainPlayer = new Player
            {
                Name = "UMAIR",
                Type = PlayerType.MAIN,
                IsMyTurn = true,
                IsCoinExposed = false,
                UserIcon = "/Content/Images/Player1/0.png"
            };

            model.OpponentPlayer = new Player
            {
                Name = "HAMMAD",
                Type = PlayerType.OPPONENT,
                IsMyTurn = false,
                IsCoinExposed = false,
                UserIcon = "/Content/Images/Player2/0.png"
            };

            model.Coins = GenerateShuffledList();
            return View(model);
        }

        public List<CoinsModel> GenerateShuffledList()
        {
            Random random = new Random();
            List<int> numbers = new List<int> { 1, 1, 1, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7 };

            List<CoinsModel> combinedList = numbers
                .SelectMany(number => new[]
                {
                    new CoinsModel { Number = number, Type = PlayerType.MAIN, ImgPath = $"/Content/Images/Player1/{number}.png" },
                    new CoinsModel { Number = number, Type = PlayerType.OPPONENT, ImgPath = $"/Content/Images/Player2/{number}.png" }
                })
                .OrderBy(_ => random.Next())
                .ToList();

            return combinedList;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}