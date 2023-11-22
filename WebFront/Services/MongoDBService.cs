using MongoDB.Driver;
using WebFront.Models;

namespace WebFront.Services
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

        public void InsertExistingSession(ExistingSessionModel session)
        {
            var collection = _database.GetCollection<ExistingSessionModel>("SessionTbl");
            collection.InsertOne(session);
        }

        public ExistingSessionModel GetExistingSessionById(int sessionId)
        {
            var collection = _database.GetCollection<ExistingSessionModel>("SessionTbl");
            var filter = Builders<ExistingSessionModel>.Filter.Eq("Id", sessionId);
            return collection.Find(filter).FirstOrDefault();
        }
    }
}
