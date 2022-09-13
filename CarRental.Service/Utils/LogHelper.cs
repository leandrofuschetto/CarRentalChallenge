using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Service.Utils
{
    static class LogHelper
    {
        public const string ERROR = "ERROR";
        public const string INFORMATION = "INFORMATION";

        public static void Log(
            ILogger logger, 
            string message, 
            string level, 
            string className, 
            string method)
        {
            string logMessage = $"{message}. At {className}, {method}";

            switch (level)
            {
                case "ERROR":
                    logger.LogError(logMessage);
                    break;
                default:
                    logger.LogInformation(logMessage);
                    break;
            }
        }
    }
}
