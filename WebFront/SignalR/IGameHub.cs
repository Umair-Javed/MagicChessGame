using Common.Library.Enums;

namespace WebFront.SignalR
{
    public interface IGameHub
    {
        Task AddToGameGroup(string connectionId, string groupId);
        Task RemoveFromGameGroup(string connectionId, string groupId);
        Task FlipCoin(string SessionId, string GroupId, PlayerType Turn, string OpponentId, string ConnectionId);
    }
}
