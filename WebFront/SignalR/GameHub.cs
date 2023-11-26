using Common.Library.Interfaces;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using WebFront.SignalR;

public class GameHub : Hub, IGameHub
{
    private readonly IMongoDBService _mongoDBService;
    public GameHub(IMongoDBService mongoDBService)
    {
        _mongoDBService = mongoDBService;
    }

    public async Task FlipCoin(string SessionId, string GroupId)
    {
        // Broadcast the coin flip to all connected clients
        //await Clients.All.SendAsync("CoinFlipped");

        // Broadcast the coin flip to all clients in the "gameGroup"
        //Groupname should be combination of user1,user2,token
        //Fetch tableId
        //Fetch tableHTML
        if (Clients != null)
        {
            var tableHTML = _mongoDBService.GetSessionBySessionOrGroupId(SessionId, GroupId);

            /// Session Insert entry

            //
            if (tableHTML != null)
            {
                await Clients.All.SendAsync("CoinFlipped", tableHTML.ChessBoardHtml);
               // await Clients.Group(GroupId).SendAsync("CoinFlipped", tableHTML.ChessBoardHtml);
            }
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