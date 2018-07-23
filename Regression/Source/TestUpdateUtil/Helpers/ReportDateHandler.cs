﻿using System;
using System.Globalization;

namespace TestElvizUpdateTool.Helpers
{
    public class ReportDateHandler
    {
        public ReportDateHandler(DateTime reportDate)
        {
            ReportDate = reportDate;
            DaylightTime = TimeZone.CurrentTimeZone.GetDaylightChanges(ReportDate.Year);
            StartOfDayLightTime = DaylightTime.Start.Date;
            EndOfDayLightTime = DaylightTime.End.Date;
        }
        DateTime ReportDate { get; }
        DaylightTime DaylightTime { get; }
        DateTime StartOfDayLightTime {get;}
        DateTime EndOfDayLightTime { get; }

        public bool IsDayLightTime() => ReportDate == StartOfDayLightTime || ReportDate == EndOfDayLightTime;
        public DateTime IgnoreWeekends() => ReportDate.DayOfWeek == DayOfWeek.Sunday ?
                                           ReportDate.AddDays(-2) :
                                           ReportDate.DayOfWeek == DayOfWeek.Saturday ?
                                           ReportDate.AddDays(-1) :
                                           ReportDate;

    }
}
