using MongoDB.Bson;
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
        public string UpdateSession(SessionModel session)
        {
            var collection = _database.GetCollection<SessionModel>("SessionTbl");

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
                var filter = Builders<SessionModel>.Filter.Eq("_id", ObjectId.Parse(session.Id));

                var update = Builders<SessionModel>.Update
                    .Set(s => s.Turn, session.Turn == PlayerType.MAIN ? PlayerType.OPPONENT : PlayerType.MAIN)
                    .Set(s => s.ChessBoardHtml, session.ChessBoardHtml);

                collection.UpdateOne(filter, update);
            }
            return session.Id;
        }

        public SessionModel GetSessionById(string sessionId)
        {
            var collection = _database.GetCollection<SessionModel>("SessionTbl");
            var filter = Builders<SessionModel>.Filter.Eq("Id", sessionId);
            return collection.Find(filter).FirstOrDefault();
        }
    }
}
