using Common.Library.Interfaces;
using Common.Library.MongoDbEntities;

namespace Common.Library.Services
{
    public class MatchMakingServices
    {
        MongoDBService mongoDbServices = new MongoDBService();
        public UserDetail GetOpponent(string MainPlayerName)
        {
            return mongoDbServices.GetOpponent(MainPlayerName);
        }
    }
}
