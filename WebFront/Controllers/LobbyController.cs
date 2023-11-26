using Common.Library.Interfaces;
using Common.Library.MongoDbEntities;
using Common.Library.Services;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        public async Task<IActionResult> InitializeGame(string Username, string ConnectionId)
        {
            MatchMakingServices matchMakingServices = new MatchMakingServices();
            var mainPlayerDetail = new UserDetail();
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
                    mainPlayerDetail = existingUser;
                    mainPlayerDetail.ConnectionId = ConnectionId;
                }
            }
            else
            {
                mainPlayerDetail = new UserDetail
                {
                    UserName = Username,
                    IsOnline = true,
                    IsPlaying = false,
                    CreatedOn = DateTime.Now,
                    ConnectionId = ConnectionId
                };

                await _mongoDBService.AddUserDetail(mainPlayerDetail);
            }

            var opponentDetail = matchMakingServices.GetOpponent(Username);
            if (opponentDetail != null)
            {
                var groupId = Guid.NewGuid().ToString();

                // update main player status
                mainPlayerDetail.GroupId = groupId;
                mainPlayerDetail.IsPlaying = true;
                mainPlayerDetail.IsOnline = true;
                await _mongoDBService.UpdateUserDetail(mainPlayerDetail);

                // update opponent status
                opponentDetail.GroupId = groupId;
                opponentDetail.IsPlaying = true;
                opponentDetail.IsOnline = true;

                await _mongoDBService.UpdateUserDetail(opponentDetail);

                // write cookie in client browser
                _cookieService.SetSessionCookie(HttpContext, groupId, "", ConnectionId);

                await _hubContext.Groups.AddToGroupAsync(ConnectionId, groupId); // assign a group to main player
                await _hubContext.Groups.AddToGroupAsync(opponentDetail.ConnectionId, groupId); // assign same group to the opponent
                //await _hubContext.Clients.All.SendAsync("CoinFlipped", "");                                                                                 // Get the list of connections in the group

                return RedirectToAction("GameIndex", "Chess",
                    new
                    {
                        MainPlayer = mainPlayerDetail.UserName,
                        Opponent = opponentDetail.UserName,
                        GroupId = groupId,
                      //  SessionId = 
                    });
            }

            return RedirectToAction("GameIndex", "Chess",
                    new
                    {
                        MainPlayer = mainPlayerDetail.UserName,
                        Opponent = "",
                        GroupId = "",
                    });
        }
    }
}
