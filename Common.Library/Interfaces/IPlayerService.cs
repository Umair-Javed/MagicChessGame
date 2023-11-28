using Common.Library.Enums;
using Common.Library.Models;
using Common.Library.MongoDbEntities;

namespace Common.Library.Interfaces
{
    public interface IPlayerService
    {
        PlayerModel? InitializePlayer(string name, PlayerType type);
        PlayerModel? InitializePlayerWithExistingSession(GameSession session, PlayerType type);
        List<CoinsModel> GenerateShuffledList();
    }
}
