using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library.Interfaces
{
    public interface IServerStatusService
    {
        bool IsServerRunning { get; }
        void SetServerStatus(bool isRunning);
        void LogError(string errorMessage);
    }
}
