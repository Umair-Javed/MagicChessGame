namespace WebFront.Services
{
    public class ServerStatusService : IServerStatusService
    {
        private bool _isServerRunning = true;

        public bool IsServerRunning => _isServerRunning;

        public void SetServerStatus(bool isRunning)
        {
            _isServerRunning = isRunning;
        }

        public void LogError(string errorMessage)
        {
            // Log the error, you can use your logging mechanism here
            Console.WriteLine($"Error: {errorMessage}");
        }
    }
}
