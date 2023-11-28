using Common.Library.MongoDbEntities;

namespace Common.Library.Interfaces
{
    public interface IMatchMakingServices
    {
        UserDetail GetOpponent(string MainPlayerName);
    }
}
