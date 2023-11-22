using WebFront.Models;

namespace WebFront.Services
{
    public interface IMongoDBService
    {
        void InsertExistingSession(ExistingSessionModel session);
        ExistingSessionModel GetExistingSessionById(int sessionId);
    }
}
