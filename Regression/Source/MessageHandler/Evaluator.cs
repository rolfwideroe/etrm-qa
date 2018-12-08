using MessageHandler.Pocos;
using System;
using System.Diagnostics;

namespace MessageHandler
{
    public static class Evaluator
    {
        static LoggingDetails LoggingDetails { get; set; } = new LoggingDetails();

        static Type CallingClass { get; set; }
        static string MethodName { get; set; }
        static string Field { get; set; }
        static string Description { get; set; }
        private static LogLevel LogLevel { get; set; }

        public static MessageDetails MessageConstructor(LogLevel logLevel, Type callingClass, string methodName, string field, string description)
        {
            CallingClass = callingClass;
            MethodName = methodName;
            Field = field;
            Description = description;
            LogLevel = logLevel;

            LogError();

            return GradeError();
        }

        static void LogError()
        {
            LoggingDetails.Level = LogLevel;
            LoggingDetails.Message = $"{Description} for {Field} in {MethodName}";
            LoggingDetails.CallingClass = CallingClass;

            Logger.Record(LoggingDetails);
        }


        static MessageDetails GradeError()
        {

            var messageDetail = new MessageDetails
            {
                CriticalOccurred = (LoggingDetails.Level == LogLevel.Error ||
                                    LoggingDetails.Level == LogLevel.Exception ||
                                    LoggingDetails.Level == LogLevel.Fatal),
                Details = LoggingDetails
            };

            return messageDetail;
        }
    }
}
