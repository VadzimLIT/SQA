using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting
{
    public class SystemMonitor
    {
        private FileExtensionManager fileManager;
        private ICrashLoggingService crashLogger;
        private IEmailService emailLogger;
        private ICorruptFileLoggingService corruptFileLogger;// my change=====================================
        public SystemMonitor()
        {
            fileManager = new FileExtensionManager();
            crashLogger = new CrashLoggingService();
            emailLogger = new EmailService();
            corruptFileLogger = new CorruptFileLoggingService();
        }
        public SystemMonitor(FileExtensionManager fileExtensionManager, ICrashLoggingService crashLoggingService, IEmailService emailService, ICorruptFileLoggingService corruptFileLoggerService)
        {
            fileManager = fileExtensionManager;
            crashLogger = crashLoggingService;
            emailLogger = emailService;
            corruptFileLogger = corruptFileLoggerService;
        }

        public void ProcessDump(string DumpFile)
        {
            if (fileManager.IsValid(DumpFile))
            {
                /* Dump file valid so log details with the crashLoggingService Web service. */
                try
                {
                    crashLogger.LogError("Dump file is valid" + DumpFile);
                    // called crashLoggingService
                }
                catch (Exception e)
                {
                    emailLogger.SendEmail("HelpDesk@lit.ie","crashLoggingService Web service threw exception", e.Message);
                    /* i.e. called emailService which logs the exception with the email service */
                }
            }
            else
            {
                /* Dump file is invalid so log details with the corruptFileLoggingService Web service. */
                try
                {
                    corruptFileLogger.LogCorruptionDetails("Dump file is corrupt: " + DateTime.Now + DumpFile);
                    // called crashLoggingService
                }
                catch (Exception e)
                {
                    emailLogger.SendEmail("HelpDesk@lit.ie", "corruptFileLoggingService Web service threw exception", e.Message);
                    /* i.e. called emailService which logs the exception with the email service */
                }
            }
        }
    }
}
