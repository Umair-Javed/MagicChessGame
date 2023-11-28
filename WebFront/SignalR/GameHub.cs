using Common.Library.Enums;
using Common.Library.Interfaces;
using Microsoft.AspNetCore.SignalR;
using WebFront.SignalR;

// SignalR Hub for managing game-related communication
public class GameHub : Hub, IGameHub
{
    #region Fields

    private readonly IMongoDBService _mongoDBService;

    #endregion

    #region Constructor

    // Constructor for GameHub
    public GameHub(IMongoDBService mongoDBService)
    {
        _mongoDBService = mongoDBService;
    }

    #endregion

    #region Hub Methods

    // Hub method for adding a connection to a game group
    public async Task AddToGameGroup(string connectionId, string groupId)
    {
        try
        {
            await Groups.AddToGroupAsync(connectionId, groupId);
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately, log or notify
            Console.WriteLine($"Error in AddToGameGroup: {ex.Message}");
        }
    }

    // Hub method for removing a connection from a game group
    public async Task RemoveFromGameGroup(string connectionId, string groupId)
    {
        try
        {
            await Groups.RemoveFromGroupAsync(connectionId, groupId);
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately, log or notify
            Console.WriteLine($"Error in RemoveFromGameGroup: {ex.Message}");
        }
    }

    // Hub method for flipping the coin in the game
    public async Task FlipCoin(string sessionId, string groupId, PlayerType turn, string opponentId, string connectionId)
    {
        try
        {
            var session = _mongoDBService.GetSessionBySessionOrGroupId(sessionId, groupId);
            if (session != null)
            {
                await Clients.Group(groupId).SendAsync("CoinFlipped", session.ChessBoardHtml, (int)turn, opponentId, connectionId);
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately, log or notify
            Console.WriteLine($"Error in FlipCoin: {ex.Message}");
        }
    }

    // Hub method for handling game over event
    public async Task GameOver(string playerName, string groupId)
    {
        try
        {
            var redirectUrl = "/Lobby/GameOver?Winner=" + playerName;
            var session = _mongoDBService.GetSessionBySessionOrGroupId(string.Empty, groupId);
            if (session != null)
            {
                // Cleanup game-related data after game over
                await _mongoDBService.DeleteGameSession(groupId);
                await _mongoDBService.DeleteUserDetail(session.MainPlayerId);
                await _mongoDBService.DeleteUserDetail(session.OpponentId);
            }

            await Clients.Group(groupId).SendAsync("GameOverResponse", redirectUrl);
        }
        catch (Exception ex)
        {
            // Handle exceptions appropriately, log or notify
            Console.WriteLine($"Error in GameOver: {ex.Message}");
        }
    }

    #endregion
}
