using Common.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library.Services
{
    // Represents a service for managing the status of the Logon server.
    public class ServerStatusService : IServerStatusService
    {
        #region Fields

        // Indicates whether the server is running.
        private bool _isServerRunning = true;

        #endregion

        #region Properties

        // Gets the current status of the server.
        public bool IsServerRunning => _isServerRunning;

        #endregion

        #region Server Status Management

        // Sets the status of the server.
        public void SetServerStatus(bool isRunning)
        {
            _isServerRunning = isRunning;
        }

        #endregion

        #region Logging

        // Logs an error message.
        public void LogError(string errorMessage)
        {
            // Log the error, you can use your logging mechanism here
            Console.WriteLine($"Error: {errorMessage}");
        }

        #endregion
    }

}
