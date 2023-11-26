using Common.Library.Enums;
using Common.Library.Interfaces;
using Common.Library.MongoDbEntities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Common.Library.Services
{
    public class MongoDBService : IMongoDBService
    {
        private readonly IMongoDatabase _database;
        public readonly string connectionString = "mongodb+srv://hammadurrehman26:Admin%40123**@mycluster.b9pkgxd.mongodb.net/?retryWrites=true&w=majority";
        public readonly string databaseName = "ChessDB";

        public MongoDBService()
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }
        public string UpdateSession(GameSession session)
        {
            var collection = _database.GetCollection<GameSession>("GameSession");

            // Check if _id is null to determine if it's an insert or update
            if (session.Id == null)
            {
                // It's an insert, MongoDB will generate a new ObjectId
                session.Turn = session.Turn == PlayerType.MAIN ? PlayerType.OPPONENT : PlayerType.MAIN;
                collection.InsertOne(session);
            }
            else
            {
                // It's an update
                var filter = Builders<GameSession>.Filter.Eq("_id", ObjectId.Parse(session.Id));

                var update = Builders<GameSession>.Update
                    .Set(s => s.Turn, session.Turn == PlayerType.MAIN ? PlayerType.OPPONENT : PlayerType.MAIN)
                    .Set(s => s.ChessBoardHtml, session.ChessBoardHtml);

                collection.UpdateOne(filter, update);
            }
            return session.Id;
        }

        public GameSession GetSessionBySessionOrGroupId(string sessionId, string groupId)
        {
            var collection = _database.GetCollection<GameSession>("GameSession");

            if (!string.IsNullOrEmpty(sessionId))
            {
                // If sessionId is provided, search by sessionId
                var filterBySessionId = Builders<GameSession>.Filter.Eq("Id", sessionId);
                return collection.Find(filterBySessionId).FirstOrDefault();
            }
            else if (!string.IsNullOrEmpty(groupId))
            {
                // If sessionId is null but groupId is provided, search by groupId
                var filterByGroupId = Builders<GameSession>.Filter.Eq("GroupId", groupId);
                return collection.Find(filterByGroupId).FirstOrDefault();
            }
            else
            {
                // If both sessionId and groupId are null, return null
                return null;
            }
        }


        public async Task AddUserDetail(UserDetail userDetail)
        {
            var collection = _database.GetCollection<UserDetail>("UserDetail");
            await collection.InsertOneAsync(userDetail);
        }

        public async Task UpdateUserDetail(UserDetail userDetail)
        {
            var collection = _database.GetCollection<UserDetail>("UserDetail");
            var filter = Builders<UserDetail>.Filter.Eq("_id", ObjectId.Parse(userDetail.Id));

            var update = Builders<UserDetail>.Update
                .Set(s => s.IsOnline, userDetail.IsOnline)
                .Set(s => s.GroupId, userDetail.GroupId)
                .Set(s => s.IsPlaying, userDetail.IsPlaying)
                .Set(s => s.ConnectionId, userDetail.ConnectionId);

            collection.UpdateOne(filter, update);
        }

        public UserDetail GetOpponent(string mainPlayerName)
        {
            var collection = _database.GetCollection<UserDetail>("UserDetail");

            // Define your filter criteria
            var filter = Builders<UserDetail>.Filter.Where(u =>
                u.IsOnline == true &&
                u.IsPlaying == false &&
                u.UserName != mainPlayerName
            );

            // Order by CreatedOn in Ascending order
            var sortDefinition = Builders<UserDetail>.Sort.Ascending(u => u.CreatedOn);

            // Perform the query and get the first matching document
            var opponent = collection
                .Find(filter)
                .Sort(sortDefinition)
                .FirstOrDefault();

            return opponent;
        }

        public Task<bool> IsUsernameAlreadyExist(string username)
        {
            var collection = _database.GetCollection<UserDetail>("UserDetail");

            // Define the filter criteria to check if a user with the specified username exists
            var filter = Builders<UserDetail>.Filter.Where(u => u.UserName == username);

            // Perform the query to find a user with the specified username
            var existingUser = collection.Find(filter).FirstOrDefault();

            // Return true if a user with the given username already exists, otherwise return false
            return Task.FromResult(existingUser != null);
        }
    }
}
