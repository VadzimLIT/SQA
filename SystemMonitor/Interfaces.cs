using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting
{

    public interface ICrashLoggingService
    {
        void LogError(string message);
    }

    public interface IEmailService
    {
        void SendEmail(string to, string subject, string body);
    }

    public interface ICorruptFileLoggingService// my change ========================================================
    {
        void LogCorruptionDetails(string message);
    }
}
