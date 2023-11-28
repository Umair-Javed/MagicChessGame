using Common.Library.ConfigModels;
using Common.Library.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using System.Text;

namespace WebFront.BackgroundServices
{
    //Background service to ping the server periodically
    public class PingBackgroundService : BackgroundService
    {
        #region Fields

        // Service to manage the status of the server
        private readonly IServerStatusService _serverStatusService;

        // Settings for the logon server
        private readonly LogonServerSettings _serverSettings;

        #endregion

        #region Constructor

        // Constructor for PingBackgroundService
        public PingBackgroundService(IServerStatusService serverStatusService, IOptions<LogonServerSettings> serverSettings)
        {
            _serverStatusService = serverStatusService;
            _serverSettings = serverSettings.Value;
        }

        #endregion

        #region Background Service Execution

        // Implementation of the background service execution
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (TcpClient tcpClient = new TcpClient(_serverSettings.IpAddress, _serverSettings.Port))
                    {
                        using (NetworkStream stream = tcpClient.GetStream())
                        {
                            // Send a ping message to the server
                            string message = "Ping from client!";
                            byte[] buffer = Encoding.ASCII.GetBytes(message);

                            await stream.WriteAsync(buffer, 0, buffer.Length);

                            // Receive and process the server's response
                            buffer = new byte[1024];
                            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                            string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                            Console.WriteLine($"Server response: {response}");

                            // Set the server status to running
                            _serverStatusService.SetServerStatus(true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error connecting to the server: {ex.Message}");

                    // Set the server status to not running
                    _serverStatusService.SetServerStatus(false);

                    // Log the error
                    _serverStatusService.LogError(ex.Message);
                }

                // Adjust the interval as needed
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        }

        #endregion
    }

}
