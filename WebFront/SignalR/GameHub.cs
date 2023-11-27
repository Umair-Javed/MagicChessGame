using Common.Library.Enums;
using Common.Library.Interfaces;
using Microsoft.AspNetCore.SignalR;
using WebFront.SignalR;

public class GameHub : Hub, IGameHub
{
    private readonly IMongoDBService _mongoDBService;
    public GameHub(IMongoDBService mongoDBService)
    {
        _mongoDBService = mongoDBService;
    }

    public async Task FlipCoin(string SessionId, string GroupId, PlayerType Turn, string OpponentId, string ConnectionId)
    {
        // Turn = Turn == PlayerType.MAIN ? PlayerType.OPPONENT : PlayerType.MAIN;

        if (Clients != null)
        {
            var tableHTML = _mongoDBService.GetSessionBySessionOrGroupId(SessionId, GroupId);
            if (tableHTML != null)
            {
                await Clients.Group(GroupId).SendAsync("CoinFlipped", tableHTML.ChessBoardHtml, (int)Turn, OpponentId, ConnectionId);
            }
        }
    }
    public async Task GameOver(string PlayerName, string GroupId)
    {
        if (Clients != null)
        {
            var redirectUrl = "/Lobby/GameOver?Winner=" + PlayerName;
            await _mongoDBService.DeleteGameSession(GroupId);
            await Clients.Group(GroupId).SendAsync("GameOverResponse", redirectUrl);
        }
    }

    public async Task AddToGameGroup(string connectionId, string groupId)
    {
        await Groups.AddToGroupAsync(connectionId, groupId);
    }

    public async Task RemoveFromGameGroup(string connectionId, string groupId)
    {
        await Groups.RemoveFromGroupAsync(connectionId, groupId);
    }
}