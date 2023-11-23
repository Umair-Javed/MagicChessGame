using Microsoft.AspNetCore.SignalR;

public class GameHub : Hub
{
    public async Task FlipCoin()
    {
        // Broadcast the coin flip to all connected clients
        //await Clients.All.SendAsync("CoinFlipped");

        // Broadcast the coin flip to all clients in the "gameGroup"
        //Groupname should be combination of user1,user2,token
        //Fetch tableId
        
        await Clients.Group("HU").SendAsync("CoinFlipped");
    }

    public async Task AddToGameGroup(string userId)
    {
        var tableId = "HU"; //GetTableId
        await Groups.AddToGroupAsync(Context.ConnectionId, tableId);
    }

    public async Task RemoveFromGameGroup(string userId)
    {
        var tableId = "HU";
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, tableId);
    }
}