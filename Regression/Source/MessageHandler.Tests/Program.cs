using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using MessageHandler.Pocos;
using Shouldly;
using static MessageHandler.Logger;


namespace MessageHandler.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Reporter.Display(ConfigureMessages(LogLevel.Warning), ShouldlyEvaluation.Empty);


            Console.WriteLine("Hit enter");
            Console.ReadLine();
        }

        private static IEnumerable<MessageDetails> ConfigureMessages(LogLevel leveller)
        {
            var callingClass = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
            IList<MessageDetails> MessageDetailsList = new List<MessageDetails>();

            if (leveller == LogLevel.Warning)
            {
                var loggingDetails = new LoggingDetails
                {
                    Message = "This is a Warning",
                    Level = LogLevel.Warning,
                    CallingClass = callingClass
                };

                MessageDetailsList.Add(Evaluator.MessageConstructor(LogLevel.Warning, callingClass, GetCurrentMethodName(), "Warning Field",
                    "Testing Warning Message Flow"));
            }

            if (leveller == LogLevel.Fatal)
            {
                var loggingDetails = new LoggingDetails
                {
                    Message = "This is a Fatal",
                    Level = LogLevel.Fatal,
                    CallingClass = callingClass
                };
                MessageDetailsList.Add(Evaluator.MessageConstructor(LogLevel.Fatal, callingClass, GetCurrentMethodName(), "Fatal Field",
                    "Testing Fatal Message Flow"));
            }

            if (leveller == LogLevel.Error)
            {
                var loggingDetails = new LoggingDetails
                {
                    Message = "This is an Error",
                    Level = LogLevel.Error,
                    CallingClass = callingClass
                };
                MessageDetailsList.Add(Evaluator.MessageConstructor(LogLevel.Error, callingClass, GetCurrentMethodName(), "Error Field",
                    "Testing Error Message Flow"));
            }

            if (leveller == LogLevel.Info)
            {

                var loggingDetails = new LoggingDetails
                {
                    Message = "This is a Info",
                    Level = LogLevel.Info,
                    CallingClass = callingClass
                };
                MessageDetailsList.Add(Evaluator.MessageConstructor(LogLevel.Info, callingClass, GetCurrentMethodName(), "Info Field",
                    "Testing Info Message Flow"));
            }

            if (leveller == LogLevel.Debug)
            {
                var loggingDetails = new LoggingDetails
                {
                    Message = "This is a Debug",
                    Level = LogLevel.Debug,
                    CallingClass = callingClass
                };
                MessageDetailsList.Add(Evaluator.MessageConstructor(LogLevel.Debug, callingClass, GetCurrentMethodName(), "Debug Field",
                    "Testing Debug Message Flow"));
            }

            if (leveller == LogLevel.Exception)
            {
                var enumerator = 10;
                var denomenator = 0;
                try
                {
                    var result = enumerator / denomenator;
                }
                catch (DivideByZeroException sByZeroException)
                {
                    var loggingDetails = new LoggingDetails
                    {
                        Message = GetMostDescriptiveMessage(sByZeroException),
                        Level = LogLevel.Exception,
                        CallingClass = callingClass
                    };
                    MessageDetailsList.Add(Evaluator.MessageConstructor(LogLevel.Exception, callingClass, GetCurrentMethodName(),
                                                                                $"DivideByZero Field {enumerator}, {denomenator}", "Testing Exception Message Flow"));
                }
            }

            if (leveller == LogLevel.Exception)
            {
                const string connectionString =
                    "Data Source = MSSQL1; Initial Catalog = AdventureWorks2;Integrated Security=true";
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                    }
                }
                catch (SqlException sInvalidOperationException)
                {
                    var loggingDetails = new LoggingDetails
                    {
                        Message = GetMostDescriptiveMessage(sInvalidOperationException),
                        Level = LogLevel.Exception,
                        CallingClass = callingClass
                    };
                    MessageDetailsList.Add(Evaluator.MessageConstructor(LogLevel.Exception, callingClass, GetCurrentMethodName(),
                        $"SQL Exception Field {connectionString}", "Testing SQL Exception Message Flow"));
                }
            }

            return MessageDetailsList;
        }

        private static string ReportCriticalErrors(IEnumerable<MessageDetails> evaluationErrors)
        {
            var stringBuilder = new StringBuilder();
            var messageDetailsList = evaluationErrors.ToList();
            var totalNumberOfErrors = messageDetailsList.Count();
            var numberOfErrors = 0;
            foreach (var evaluationError in messageDetailsList)
            {
                stringBuilder.Append(evaluationError.Details.Message);
                numberOfErrors += 1;
                if (numberOfErrors < totalNumberOfErrors) stringBuilder.Append("\n");
            }

            return stringBuilder.ToString();
        }

        private static string GetMostDescriptiveMessage(Exception exception)
        {
            return exception.InnerException?.Message ?? exception.Message;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetCurrentMethodName()
        {
            var stackTrace = new StackTrace();
            var stackFrame = stackTrace.GetFrame(1);

            return stackFrame.GetMethod().Name;
        }
    }
}
