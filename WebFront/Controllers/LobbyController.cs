using Common.Library.Interfaces;
using Common.Library.MongoDbEntities;
using Common.Library.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace WebFront.Controllers
{
    public class LobbyController : Controller
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly IMongoDBService _mongoDBService;
        private readonly ICookieService _cookieService;
        public LobbyController(
            IHubContext<GameHub> hubContext,
            IMongoDBService mongoDBService,
            ICookieService cookieService)
        {
            _hubContext = hubContext;
            _mongoDBService = mongoDBService;
            _cookieService = cookieService;

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
            var mainPlayerDetail = new UserDetail
            {
                UserName = Username,
                IsOnline = true,
                IsPlaying = false,
                CreatedOn = DateTime.Now,
            };

            bool IsExist = await _mongoDBService.IsUsernameAlreadyExist(Username);
            if (IsExist)
            {
                TempData["WarningMsg"] = "Username Already Exist";
                return RedirectToAction("Index");
            }

            await _mongoDBService.AddUserDetail(mainPlayerDetail);
            var opponentDetail = matchMakingServices.GetOpponent(Username);
            if (opponentDetail != null)
            {
                var groupId = Guid.NewGuid().ToString();

                // update main player status
                mainPlayerDetail.GroupId = groupId;
                mainPlayerDetail.IsPlaying = true;
                await _mongoDBService.UpdateUserDetail(mainPlayerDetail);

                // update opponent status
                opponentDetail.GroupId = groupId;
                opponentDetail.IsPlaying = true;
                await _mongoDBService.UpdateUserDetail(opponentDetail);

                // write cookie in client browser
                _cookieService.SetSessionCookie(HttpContext, groupId);

                return RedirectToAction("GameIndex", "Chess",
                    new
                    {
                        MainPlayer = mainPlayerDetail.UserName,
                        Opponent = opponentDetail.UserName,
                        GroupId = groupId
                    });
            }

            // Call the hub function to call Logon Hub for match Making
            //await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Controller", "Hello from controller!");

            return RedirectToAction("GameIndex", "Chess",
                    new
                    {
                        MainPlayer = mainPlayerDetail.UserName
                    });
        }
    }
}
