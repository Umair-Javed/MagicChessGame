using Common.Library.Interfaces;
using Common.Library.MongoDbEntities;
using Common.Library.Services;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using WebFront.Models;
using WebFront.SignalR;

namespace WebFront.Controllers
{
    // Controller responsible for Lobby-related actions
    public class LobbyController : Controller
    {
        #region Dependencies

        private readonly IHubContext<GameHub> _hubContext;
        private readonly IMongoDBService _mongoDBService;
        private readonly ICookieService _cookieService;
        private readonly IMatchMakingServices _matchMakingService;

        #endregion

        #region Constructor

        // Constructor for LobbyController
        public LobbyController(
            IMongoDBService mongoDBService,
            ICookieService cookieService,
            IHubContext<GameHub> hubContext,
            IMatchMakingServices matchMakingService)
        {
            _mongoDBService = mongoDBService;
            _cookieService = cookieService;
            _hubContext = hubContext;
            _matchMakingService = matchMakingService;
        }

        #endregion

        #region Action Methods

        // Action method for rendering the Lobby index view
        public IActionResult Index()
        {
            if (TempData["WarningMsg"] != null)
            {
                ViewBag.Msg = TempData["WarningMsg"];
            }
            return View();
        }

        // Action method for initializing a new game
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InitializeGame(string Username)
        {
            var playerDetail = new UserDetail();
            var groupId = Guid.NewGuid().ToString();
            var existingUser = await _mongoDBService.GetUserDetail(Username);
            if (existingUser != null)
            {
                if (existingUser.IsOnline || existingUser.IsPlaying)
                {
                    TempData["WarningMsg"] = "Username Already Exist";
                    return RedirectToAction("Index");
                }
                else
                {
                    playerDetail = existingUser;
                    playerDetail.GroupId = groupId;
                }
            }
            else
            {
                playerDetail = new UserDetail
                {
                    UserName = Username,
                    IsOnline = true,
                    IsPlaying = false,
                    CreatedOn = DateTime.Now,
                    GroupId = groupId
                };

                await _mongoDBService.AddUserDetail(playerDetail);
            }

            var playerOnWaiting = _matchMakingService.GetOpponent(Username);
            if (playerOnWaiting != null)
            {
                groupId = playerOnWaiting.GroupId; // assign group of Main User to Opponent
                                                   // update main player status
                playerDetail.GroupId = groupId;
                playerDetail.IsPlaying = true;
                playerDetail.IsOnline = true;
                playerDetail.Type = Common.Library.Enums.PlayerType.OPPONENT;
                await _mongoDBService.UpdateUserDetail(playerDetail);

                // update opponent status
                playerOnWaiting.GroupId = groupId;
                playerOnWaiting.IsPlaying = true;
                playerOnWaiting.IsOnline = true;
                playerOnWaiting.Type = Common.Library.Enums.PlayerType.MAIN;

                await _mongoDBService.UpdateUserDetail(playerOnWaiting);

                // write cookie in client browser
                _cookieService.SetSessionCookie(HttpContext, groupId, "");

                var gameIndexModel = new GameIndexModel
                {
                    GroupId = groupId,
                    MainPlayer = playerOnWaiting.UserName,
                    Opponent = playerDetail.UserName,
                    SessionId = ""
                };

                TempData["IndexModel"] = Newtonsoft.Json.JsonConvert.SerializeObject(gameIndexModel);
            }
            else
            {
                var gameIndexModel = new GameIndexModel
                {
                    GroupId = groupId,
                    MainPlayer = playerDetail.UserName,
                    Opponent = "Waiting...",
                    SessionId = ""
                };

                TempData["IndexModel"] = Newtonsoft.Json.JsonConvert.SerializeObject(gameIndexModel);
            }

            return RedirectToAction("GameIndex", "Chess");
        }

        // Action method for rendering the GameOver view
        public ActionResult GameOver(string Winner)
        {
            ViewBag.Winner = Winner;
            return View();
        }

        #endregion
    }

}
