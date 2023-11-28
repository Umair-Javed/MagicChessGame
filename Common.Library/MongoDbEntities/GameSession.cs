using Common.Library.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Library.MongoDbEntities
{
    public class GameSession
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string MainPlayerId { get; set; }
        public string OpponentId { get; set; }
        public PlayerType Turn { get; set; }
        public string ChessBoardHtml { get; set; }
    }
}
