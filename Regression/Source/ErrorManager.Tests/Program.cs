using System;
using System.Collections.Generic;


namespace ErrorManager.Tests
{
    class Program
    {

        static void Main(string[] args)
        {
            var callingClass = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;

            var loggingDetails = new List<LoggingDetails>();

            var loggingDetails1 = new LoggingDetails
            {
                Message = "This is a Warning",
                Level = LogLevel.Warning,
                CallingClass = callingClass
            };
            loggingDetails.Add(loggingDetails1);

            var loggingDetails2 = new LoggingDetails
            {
                Message = "This is a Fatal",
                Level = LogLevel.Fatal,
                CallingClass = callingClass
            };
            loggingDetails.Add(loggingDetails2);

            var loggingDetails3 = new LoggingDetails
            {
                Message = "This is a Error",
                Level = LogLevel.Error,
                CallingClass = callingClass
            };
            loggingDetails.Add(loggingDetails3);

            var loggingDetails4 = new LoggingDetails
            {
                Message = "This is a Info",
                Level = LogLevel.Info,
                CallingClass = callingClass
            };
            loggingDetails.Add(loggingDetails4);

            var loggingDetails5 = new LoggingDetails
            {
                Message = "This is a Debug",
                Level = LogLevel.Debug,
                CallingClass = callingClass
            };
            loggingDetails.Add(loggingDetails5);

            var enumerator = 10;
            var denomenator = 0;
            try
            {
                var result = enumerator / denomenator;
            }
            catch (DivideByZeroException sByZeroException)
            {
                var loggingDetails6 = new LoggingDetails
                {
                    Message = GetMostDescriptiveMessage(sByZeroException),
                    Level = LogLevel.Exception,
                    CallingClass = callingClass
                };
                loggingDetails.Add(loggingDetails6);

              //  throw;
            }

            Logger.Record(loggingDetails);

            Console.WriteLine("Hit enter");
            Console.ReadLine();
        }

        private static string GetMostDescriptiveMessage(DivideByZeroException sByZeroException)
        {
            return sByZeroException.InnerException?.Message ?? sByZeroException.Message;
        }
    }
}
