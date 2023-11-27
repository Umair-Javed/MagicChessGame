using Common.Library.MongoDbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
