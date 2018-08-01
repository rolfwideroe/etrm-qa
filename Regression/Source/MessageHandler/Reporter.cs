using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessageHandler.Pocos;
using Shouldly;

namespace MessageHandler
{
    public static class Reporter
    {
        private static IList<MessageDetails> MessageDetails { get; set; } = new List<MessageDetails>();

        public static void Display(IEnumerable<MessageDetails> messageDetailsList, ShouldlyEvaluation shouldlyEvaluation)
        {
            MessageDetails = messageDetailsList.ToList();
            var evaluationErrors = MessageDetails.Where(x => x.CriticalOccurred);

            switch (shouldlyEvaluation)
            {
                case ShouldlyEvaluation.Empty:
                    evaluationErrors.ShouldBeEmpty($"Critical Errors {ReportCriticalErrors(evaluationErrors)}");
                    break;
                case ShouldlyEvaluation.Null:
                    break;
                case ShouldlyEvaluation.NotNull:
                    break;
            }
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
    }
}
