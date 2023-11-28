using Common.Library.Interfaces;
using Common.Library.MongoDbEntities;

namespace Common.Library.Services
{
    // Represents services for matchmaking operations.
    public class MatchMakingServices : IMatchMakingServices
    {
        #region Constructor and Private Properties
        // MongoDB service for database interactions.
        private readonly IMongoDBService _mongoDBService;

        // Constructor for MatchMakingServices, taking MongoDB service through dependency injection.
        public MatchMakingServices(IMongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }
        #endregion

        #region Methods
        // Retrieves an opponent for the main player.
        public UserDetail GetOpponent(string mainPlayerName)
        {
            return _mongoDBService.GetOpponent(mainPlayerName);
        }
        #endregion
    }
}
