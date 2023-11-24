using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using WebFront.Models;
using WebFront.Services;

namespace WebFront.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITokenService _tokenService;
        private readonly ICookieService _cookieService;
        private readonly IMongoDBService _mongoDBService;
        private readonly IServerStatusService _serverStatusService;

        public HomeController(
            ILogger<HomeController> logger,
            ITokenService tokenService,
            ICookieService cookieService,
            IMongoDBService mongoDBService,
            IServerStatusService serverStatusService
            )
        {
            _logger = logger;
            _tokenService = tokenService;
            _cookieService = cookieService;
            _mongoDBService = mongoDBService;
            _serverStatusService = serverStatusService;
        }

        public IActionResult Index()
        {
            if (!_serverStatusService.IsServerRunning)
            {
                // Redirect to an error page or take other actions
                return RedirectToAction("ServerError");
            }

            var model = new ChessViewModel();
            model.FlippedIconUrl = "/Content/Images/flipped.png";
            var Session = GetSession();
            if (Session == null)
            {
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
                model.IsNewSession = true;
            }
            else
            {
                model.MainPlayer = new Player
                {
                    Name = Session.MainPlayerId,
                    Type = PlayerType.MAIN,
                    IsMyTurn = Session.Turn == PlayerType.MAIN ? true : false,
                    UserIcon = "/Content/Images/Player1/0.png"
                };

                model.OpponentPlayer = new Player
                {
                    Name = Session.OpponentId,
                    Type = PlayerType.OPPONENT,
                    IsMyTurn = Session.Turn == PlayerType.OPPONENT ? true : false,
                    UserIcon = "/Content/Images/Player2/0.png"
                };

                model.SessionId = Session.Id;
                model.ChessBoardHtml = Session.ChessBoardHtml;
            }

            return View(model);
        }

        public IActionResult ServerError()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UpdateSession(SessionModel model)
        {
            if (!_serverStatusService.IsServerRunning)
            {
                return Json(new { IsSuccess = false, IsRedirect = true, RedirectUrl = "/Home/ServerError" });
            }

            if (model == null)
                return Json(new { IsSuccess = false, IsRedirect = false, Message = "Session Not Updated!" });

            var sessionId = _mongoDBService.UpdateSession(model);
            SetSessionCookie(sessionId);
            return Json(new { IsSuccess = true, Message = "Success" });
        }

        public void SetSessionCookie(string sessionId)
        {
            var sessionCookie = new CookieModel
            {
                SessionId = sessionId,
            };

            var encryptedToken = _tokenService.EncryptToken(sessionCookie);
            _cookieService.SetCookie(HttpContext, "ChessGameData", encryptedToken);
        }

        public SessionModel GetSession()
        {
            var encryptedToken = _cookieService.GetCookie(HttpContext, "ChessGameData");

            if (encryptedToken != null)
            {
                var decryptedData = _tokenService.DecryptToken<CookieModel>(encryptedToken);
                if (decryptedData != null && !string.IsNullOrEmpty(decryptedData.SessionId))
                {
                    SessionModel sessionModel = new SessionModel();
                    sessionModel = _mongoDBService.GetSessionById(decryptedData.SessionId);
                    if (sessionModel != null)
                    {
                        return sessionModel;
                    }
                }
            }
            return null;
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