using System;
using log4net;
using MessageHandler.Pocos;

namespace MessageHandler
{
    public static class Logger
    {
        static ILog Log { get; } = LogManager.GetLogger(typeof(Logger));


        public static void Record(LoggingDetails loggingDetail)
        {
            var message = $"{loggingDetail.Message} called from {loggingDetail.CallingClass}";
            switch (loggingDetail.Level)
            {
                case LogLevel.Fatal:
                case LogLevel.Exception:
                    Log.Fatal(message);
                    break;
                case LogLevel.Error:
                    Log.Error(message);
                    break;
                case LogLevel.Warning:
                    Log.Warn(message);
                    break;
                case LogLevel.Info:
                    Log.Info(message);
                    break;
                case LogLevel.Debug:
                    Log.Debug(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{loggingDetail.Level} is not a valid {typeof(LogLevel)}");
            }
        }
    }
}
