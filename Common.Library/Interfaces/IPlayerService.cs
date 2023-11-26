using Common.Library.Enums;
using Common.Library.Models;
using Common.Library.MongoDbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library.Interfaces
{
    public interface IPlayerService
    {
        PlayerModel? InitialzePlayer(string name, PlayerType type);
        PlayerModel? InitialzePlayerWithExistingSession(GameSession session, PlayerType type);
        List<CoinsModel> GenerateShuffledList();
    }
}
