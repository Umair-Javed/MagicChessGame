using Common.Library.ConfigModels;
using Common.Library.Enums;
using Common.Library.Interfaces;
using Common.Library.MongoDbEntities;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace Common.Library.Services
{
    // Represents a service for interacting with MongoDB.
    public class MongoDBService : IMongoDBService
    {
        #region Constructor and Private Properties
        // MongoDB database instance.
        private readonly IMongoDatabase _database;

        // Configuration settings for MongoDB connection.
        private readonly ConnectionStrings _connectionSettings;

        // Configuration settings for MongoDB.
        private readonly MongoDBSettings _mongoDBSettings;

        // Connection string for MongoDB.
        private readonly string _connectionString;

        // Collection name for MongoDB user data.
        private readonly string _userCollectionName;

        // Collection name for MongoDB session data.
        private readonly string _sessionCollectionName;

        // Constructor for MongoDBService, taking connection settings through dependency injection.
        public MongoDBService(IOptions<ConnectionStrings> connectionSettings, IOptions<MongoDBSettings> mongoDBSettings)
        {
            // Initialize connection settings.
            _connectionSettings = connectionSettings.Value;

            // Set the connection string.
            _connectionString = _connectionSettings.MongoDBConnection;

            // Initialize MongoDB settings.
            _mongoDBSettings = mongoDBSettings.Value;

            // Create a MongoDB client using the connection string.
            var client = new MongoClient(_connectionString);

            // Get the MongoDB database using the specified database name.
            _database = client.GetDatabase(_mongoDBSettings.DatabaseName);

            // Set the collection names for user and session data.
            _userCollectionName = _mongoDBSettings.UserCollectionName;
            _sessionCollectionName = _mongoDBSettings.SessionCollectionName;
        }
        #endregion

        #region Mongo DB Methods
        public string UpdateSession(GameSession session)
        {
            var collection = _database.GetCollection<GameSession>(_sessionCollectionName);

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
            var collection = _database.GetCollection<GameSession>(_sessionCollectionName);

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
            var collection = _database.GetCollection<UserDetail>(_userCollectionName);
            await collection.InsertOneAsync(userDetail);
        }

        public async Task UpdateUserDetail(UserDetail userDetail)
        {
            var collection = _database.GetCollection<UserDetail>(_userCollectionName);
            var filter = Builders<UserDetail>.Filter.Eq("_id", ObjectId.Parse(userDetail.Id));

            var update = Builders<UserDetail>.Update
                .Set(s => s.IsOnline, userDetail.IsOnline)
                .Set(s => s.GroupId, userDetail.GroupId)
                .Set(s => s.IsPlaying, userDetail.IsPlaying)
                .Set(s => s.Type, userDetail.Type)
                .Set(s => s.ConnectionId, userDetail.ConnectionId);

            collection.UpdateOne(filter, update);
        }

        public UserDetail GetOpponent(string mainPlayerName)
        {
            var collection = _database.GetCollection<UserDetail>(_userCollectionName);

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

        public Task<UserDetail> GetUserDetail(string username)
        {
            var collection = _database.GetCollection<UserDetail>(_userCollectionName);

            // Define the filter criteria to check if a user with the specified username exists
            var filter = Builders<UserDetail>.Filter.Where(u => u.UserName == username);

            // Perform the query to find a user with the specified username
            var existingUser = collection.Find(filter).FirstOrDefault();

            // Return true if a user with the given username already exists, otherwise return false
            return Task.FromResult(existingUser);
        }

        public async Task DeleteGameSession(string groupId)
        {
            var collection = _database.GetCollection<GameSession>(_sessionCollectionName);

            if (!string.IsNullOrEmpty(groupId))
            {
                var filterByGroupId = Builders<GameSession>.Filter.Eq("GroupId", groupId);
                await collection.DeleteOneAsync(filterByGroupId);
            }
        }

        public async Task DeleteUserDetail(string playerId)
        {
            var collection = _database.GetCollection<UserDetail>(_userCollectionName);

            if (!string.IsNullOrEmpty(playerId))
            {
                var filterByUserName = Builders<UserDetail>.Filter.Eq("UserName", playerId);
                await collection.DeleteOneAsync(filterByUserName);
            }
        }
        #endregion
    }
}
