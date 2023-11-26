using Microsoft.AspNet.SignalR.Hubs;

namespace WebFront.SignalR
{
    public interface IGameHub 
    {
        Task FlipCoin(string SessionId, string GroupId);
        Task AddToGameGroup(string connectionId, string groupId);
        Task RemoveFromGameGroup(string connectionId, string groupId);
    }
}
