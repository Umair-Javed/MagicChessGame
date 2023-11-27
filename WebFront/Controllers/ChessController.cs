using Common.Library.Enums;
using Common.Library.Interfaces;
using Common.Library.MongoDbEntities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebFront.Models;

namespace WebFront.Controllers
{
    public class ChessController : Controller
    {
        private readonly ILogger<ChessController> _logger;
        private readonly ITokenService _tokenService;
        private readonly ICookieService _cookieService;
        private readonly IMongoDBService _mongoDBService;
        private readonly IServerStatusService _serverStatusService;
        private readonly IPlayerService _playerService;

        public ChessController(
          ILogger<ChessController> logger,
          ITokenService tokenService,
          ICookieService cookieService,
          IMongoDBService mongoDBService,
          IServerStatusService serverStatusService,
          IPlayerService playerService
          )
        {
            _logger = logger;
            _tokenService = tokenService;
            _cookieService = cookieService;
            _mongoDBService = mongoDBService;
            _serverStatusService = serverStatusService;
            _playerService = playerService;
        }


        public IActionResult GameIndex()
        {
            if (!_serverStatusService.IsServerRunning)
                return RedirectToAction("ServerError");

            var model = new ChessViewModel();

            if (TempData["IndexModel"] != null)
            {
                string? serializedIndexModel = Convert.ToString(TempData["IndexModel"]);
                if (serializedIndexModel != null)
                {
                    var indexModel = Newtonsoft.Json.JsonConvert.DeserializeObject<GameIndexModel>(serializedIndexModel);
                    if (indexModel != null)
                    {
                        if (string.IsNullOrEmpty(indexModel.MainPlayer))
                            return RedirectToAction("Index", "Lobby");

                        model.SessionId = indexModel.SessionId;
                        model.GroupId = indexModel.GroupId;
                        model.MainPlayer = _playerService.InitialzePlayer(indexModel.MainPlayer, PlayerType.MAIN);
                        model.OpponentPlayer = _playerService.InitialzePlayer(indexModel.Opponent, PlayerType.OPPONENT);
                        model.Coins = _playerService.GenerateShuffledList();
                        if (!string.IsNullOrEmpty(indexModel.Opponent) && indexModel.Opponent!= "Waiting...")
                        {
                            model.IsGameStarted = true;
                            model.IsDisabled = false;
                        }
                    }
                }
            }

            //var existingToken = _cookieService.GetExistingToken(HttpContext);
            //if (existingToken != null)
            //{
            //    var existingSession = new GameSession();
            //    existingSession = _mongoDBService.GetSessionBySessionOrGroupId(existingToken.SessionId, existingSession.GroupId);
            //    if (existingSession != null)
            //    {
            //        // model.GroupId = GroupId;
            //        model.SessionId = existingSession.Id;
            //        model.MainPlayer = _playerService.InitialzePlayerWithExistingSession(existingSession, PlayerType.MAIN);
            //        model.OpponentPlayer = _playerService.InitialzePlayerWithExistingSession(existingSession, PlayerType.OPPONENT);
            //        model.ChessBoardHtml = existingSession.ChessBoardHtml;
            //        model.IsGameStarted = true;
            //        model.IsDisabled = false;
            //        model.IsNewSession = false;
            //        return View(model);
            //    }
            //}

            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateSession(GameSession model)
        {
            if (!_serverStatusService.IsServerRunning)
            {
                return Json(new { IsSuccess = false, IsRedirect = true, RedirectUrl = "/Chess/ServerError" });
            }

            if (model == null)
                return Json(new { IsSuccess = false, IsRedirect = false, Message = "Session Not Updated!" });

            var sessionId = _mongoDBService.UpdateSession(model);
            _cookieService.SetSessionCookie(HttpContext, model.GroupId, sessionId);
            return Json(new { IsSuccess = true, Message = "Success", SessionId = sessionId });
        }

        public IActionResult ServerError()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
