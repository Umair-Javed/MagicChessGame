using Common.Library.Interfaces;
using Microsoft.AspNetCore.SignalR;

public class GameHub : Hub
{
    private readonly IMongoDBService _mongoDBService;

    public GameHub(IMongoDBService mongoDBService)
    {
        _mongoDBService = mongoDBService;
    }
    public async Task FlipCoin()
    {
        // Broadcast the coin flip to all connected clients
        //await Clients.All.SendAsync("CoinFlipped");

        // Broadcast the coin flip to all clients in the "gameGroup"
        //Groupname should be combination of user1,user2,token
        //Fetch tableId
        //Fetch tableHTML
        var tableHTML = _mongoDBService.GetSessionBySessionOrGroupId("655f9f566d63483e20a2b7ed",null);

        /// Session Insert entry
       
        //
        await Clients.Group("HU").SendAsync("CoinFlipped", tableHTML.ChessBoardHtml);
    }

    public async Task AddToGameGroup(string tableId)
    {
        tableId = "HU"; //GetTableId
        await Groups.AddToGroupAsync(Context.ConnectionId, tableId);
    }

    public async Task RemoveFromGameGroup(string tableId)
    {
        tableId = "HU";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, tableId);
    }
}