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
    public class LobbyController : Controller
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly IMongoDBService _mongoDBService;
        private readonly ICookieService _cookieService;
        public LobbyController(
            IMongoDBService mongoDBService,
            ICookieService cookieService,
            IHubContext<GameHub> hubContext)
        {
            _mongoDBService = mongoDBService;
            _cookieService = cookieService;
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            if (TempData["WarningMsg"] != null)
            {
                ViewBag.Msg = TempData["WarningMsg"];
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InitializeGame(string Username)
        {
            MatchMakingServices matchMakingServices = new MatchMakingServices();
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

            var playerOnWaiting = matchMakingServices.GetOpponent(Username);
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

        public ActionResult GameOver(string Winner)
        {
            ViewBag.Winner = Winner;
            return View();
        }
    }
}
