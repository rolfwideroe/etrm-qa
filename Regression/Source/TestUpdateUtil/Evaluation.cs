using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using ElvizTestUtils;
using ElvizTestUtils.DatabaseTools;
using MessageHandler;
using MessageHandler.Pocos;


namespace TestElvizUpdateTool
{
    public class Evaluation
    {
        public Evaluation(string testCaseName, string description, DateTime reportDate, bool isDayLightTime)
        {
            TestCaseName = testCaseName;
            Description = description;
            ReportDate = reportDate;
            IsDayLightTime = isDayLightTime;
        }

        int RecordMultiplier { get; set; } = 1;
        string CurrentInstrumentArea { get; set; }
        string CurrentResolution { get; set; }
        string TestCaseName { get; set; }
        string Description { get; set; }
        DateTime ReportDate { get; set; }
        string ExecutionVenue { get; set; }
        IEnumerable<InstrumentPrice> InstrumentPricesList { get; set; }
        IEnumerable<SpotPrice> SpotPricesList { get; set; }
        bool IsDayLightTime { get; set; }
        IList<MessageDetails> MessageDetails = new List<MessageDetails>();
        Type CallingClass { get; set; } = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
        string SpotPriceQueryMessage { get; set; } = "Query did not return correct number of spot price records for report date";
        string InstrumentPriceQueryMessage { get; set; } = "Query did not return any instrument price record for report date";

        public IEnumerable<MessageDetails> Result()
        {
            SetUpAndEvaluate();

            return MessageDetails;
        }

        private void SetUpAndEvaluate()
        {
            var testCaseFilePath = Path.Combine(Directory.GetCurrentDirectory(), TestCaseName);

            var jobsTestCase = TestXmlTool.Deserialize<JobsTestCase>(testCaseFilePath);

            var jobItem = jobsTestCase.JobItems.Single(x => x.Description == Description);

            if (jobItem == null)
                throw new ArgumentException("Could not find EUT source by description: " + Description);

            ExecutionVenue = jobItem.ExecutionVenue;
            InstrumentPricesList = jobItem.InstrumentPrices;
            SpotPricesList = jobItem.SpotPrices;

            if (InstrumentPricesList != null && ExecutionVenue != null) EvaluateInstrumentPrices();
            if (SpotPricesList != null) EvaluateSpotPrices();

        }

        private void EvaluateSpotPrices()
        {
            foreach (var spotPrice in SpotPricesList)
            {
                CurrentInstrumentArea = spotPrice.Area;
                CurrentResolution = spotPrice.Resolution;

                //for NordPool each ares has prices in 4 currencies, expected records will be *4 for day, hour spot prices
                if (spotPrice.ExpectedRecords != null)
                {
                    RecordMultiplier = Convert.ToInt16(spotPrice.ExpectedRecords);
                }

                var results = QaDao.GetSpotPricesByArea(CurrentInstrumentArea, ReportDate.ToString("yyyy-MM-dd"), CurrentResolution);

                switch (CurrentResolution.ToUpper())
                {
                    case "DAY" when results != 1 * RecordMultiplier:
                       MessageDetails.Add(Evaluator.MessageConstructor(LogLevel.Error, CallingClass, GetCurrentMethodName(), CurrentResolution,
                            $"{SpotPriceQueryMessage} = {ReportDate} , price area = {CurrentInstrumentArea}, " +
                            $"resolution = {CurrentResolution}.Expected: 1, but was {results}"));
                        break;
                    case "HOUR" when IsDayLightTime:
                        switch (ReportDate.Month)
                        {
                            case 10 when results != 25 * RecordMultiplier:
                                MessageDetails.Add(Evaluator.MessageConstructor(LogLevel.Error, CallingClass, GetCurrentMethodName(), CurrentResolution,
                                    $"{SpotPriceQueryMessage} (daylight Saving) = {ReportDate} price area = {CurrentInstrumentArea}, " +
                                    $"resolution = {CurrentResolution}.Expected: 25(100), but was {results}"));
                                break;
                            default:
                                if (results != 23 * RecordMultiplier)
                                    MessageDetails.Add(Evaluator.MessageConstructor(LogLevel.Error, CallingClass, GetCurrentMethodName(), CurrentResolution,
                                        $"{SpotPriceQueryMessage} (daylight Saving) = {ReportDate} price area = {CurrentInstrumentArea}, " +
                                        $"resolution = {CurrentResolution}.Expected: 23(92), but was {results}"));
                                break;
                        }
                        break;
                    case "HOUR":
                        if (results != 24 * RecordMultiplier)
                            MessageDetails.Add(Evaluator.MessageConstructor(LogLevel.Error, CallingClass, GetCurrentMethodName(), CurrentResolution,
                                $"{SpotPriceQueryMessage} = {ReportDate} price area = {CurrentInstrumentArea}, " +
                                $"resolution = {CurrentResolution}.Expected: 24(96), but was {results}"));
                        break;
                    default:
                        break;
                }
            }

        }

        private void EvaluateInstrumentPrices()
        {
            foreach (var areaPrice in InstrumentPricesList)
            {
                CurrentInstrumentArea = areaPrice.Area;
                var results = QaDao.GetInstrumentPricesByArea(ExecutionVenue, CurrentInstrumentArea,
                    ReportDate.ToString("yyyy-MM-dd"), areaPrice.CfdArea);

                if (results.Rows.Count >= 1) continue;
                MessageDetails.Add(Evaluator.MessageConstructor(LogLevel.Error, CallingClass, GetCurrentMethodName(), CurrentInstrumentArea,
                                                                        $"{InstrumentPriceQueryMessage} = {ReportDate}, price area = {CurrentInstrumentArea}, " +
                                                                        $"execution venue = {ExecutionVenue}"));
            }
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
