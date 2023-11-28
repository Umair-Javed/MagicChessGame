using Common.Library.Enums;
using Common.Library.Interfaces;
using Common.Library.MongoDbEntities;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebFront.Models;

namespace WebFront.Controllers
{
    // Controller responsible for Chess game-related actions
    public class ChessController : Controller
    {
        #region Dependencies

        private readonly ILogger<ChessController> _logger;
        private readonly ITokenService _tokenService;
        private readonly ICookieService _cookieService;
        private readonly IMongoDBService _mongoDBService;
        private readonly IServerStatusService _serverStatusService;
        private readonly IPlayerService _playerService;

        #endregion

        #region Constructor

        // Constructor for ChessController
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

        #endregion

        #region Action Methods

        // Action method for rendering the Chess game view
        public IActionResult GameIndex()
        {
            // Check if the server is running
            if (!_serverStatusService.IsServerRunning)
                return RedirectToAction("ServerError");

            var model = new ChessViewModel();

            // Check if there is a serialized IndexModel in TempData
            if (TempData["IndexModel"] != null)
            {
                string? serializedIndexModel = Convert.ToString(TempData["IndexModel"]);
                if (serializedIndexModel != null)
                {
                    var indexModel = Newtonsoft.Json.JsonConvert.DeserializeObject<GameIndexModel>(serializedIndexModel);
                    if (indexModel != null)
                    {
                        // Initialize the model with data from the serialized IndexModel
                        if (string.IsNullOrEmpty(indexModel.MainPlayer))
                            return RedirectToAction("Index", "Lobby");

                        model.SessionId = indexModel.SessionId;
                        model.GroupId = indexModel.GroupId;
                        model.MainPlayer = _playerService.InitializePlayer(indexModel.MainPlayer, PlayerType.MAIN);
                        model.OpponentPlayer = _playerService.InitializePlayer(indexModel.Opponent, PlayerType.OPPONENT);
                        model.Coins = _playerService.GenerateShuffledList();
                        if (!string.IsNullOrEmpty(indexModel.Opponent) && indexModel.Opponent != "Waiting...")
                        {
                            model.IsGameStarted = true;
                            model.IsDisabled = false;
                        }
                    }
                }
            }
            return View(model);
        }

        // Action method for updating the game session
        [HttpPost]
        public ActionResult UpdateSession(GameSession model)
        {
            // Check if the server is running
            if (!_serverStatusService.IsServerRunning)
            {
                return Json(new { IsSuccess = false, IsRedirect = true, RedirectUrl = "/Chess/ServerError" });
            }

            // Check if the provided model is not null
            if (model == null)
                return Json(new { IsSuccess = false, IsRedirect = false, Message = "Session Not Updated!" });

            // Update the session in the MongoDB service and set the session cookie
            var sessionId = _mongoDBService.UpdateSession(model);
            _cookieService.SetSessionCookie(HttpContext, model.GroupId, sessionId);

            return Json(new { IsSuccess = true, Message = "Success", SessionId = sessionId });
        }

        // Action method for rendering the server error view
        public IActionResult ServerError()
        {
            return View();
        }

        // Action method for handling generic errors
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion
    }

}
