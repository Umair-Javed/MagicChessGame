using Common.Library.Enums;

namespace WebFront.SignalR
{
    public interface IGameHub
    {
        Task FlipCoin(string SessionId, string GroupId, PlayerType Turn, string OpponentId, string ConnectionId);
        Task AddToGameGroup(string connectionId, string groupId);
        Task RemoveFromGameGroup(string connectionId, string groupId);
    }
}
