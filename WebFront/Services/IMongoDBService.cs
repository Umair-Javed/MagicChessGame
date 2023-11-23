using WebFront.Models;

namespace WebFront.Services
{
    public interface IMongoDBService
    {
        string UpdateSession(SessionModel session);
        SessionModel GetSessionById(string sessionId);
    }
}
