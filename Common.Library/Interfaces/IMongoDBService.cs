using Common.Library.MongoDbEntities;

namespace Common.Library.Interfaces
{
    public interface IMongoDBService
    {
        string UpdateSession(GameSession session);
        GameSession GetSessionBySessionOrGroupId(string sessionId, string groupId);
        Task AddUserDetail(UserDetail userDetail);
        Task UpdateUserDetail(UserDetail userDetail);
        Task<UserDetail> GetUserDetail(string username);
        Task DeleteGameSession(string groupId);
        Task DeleteUserDetail(string playerId);
        UserDetail GetOpponent(string mainPlayerName);
    }
}
