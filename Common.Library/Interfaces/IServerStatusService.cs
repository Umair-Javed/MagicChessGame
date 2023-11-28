namespace Common.Library.Interfaces
{
    public interface IServerStatusService
    {
        bool IsServerRunning { get; }
        void SetServerStatus(bool isRunning);
        void LogError(string errorMessage);
    }
}
