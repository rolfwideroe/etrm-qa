using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TestLiveCurveScheduling
{
    public class TickerParser
    {
        private const string pattern = @"\{(.*?)\}";

        public static string ParseCustomPeriod(string tickerRule, DateTime dateTime)
        {
            string parsedTicker = tickerRule;
            string query = tickerRule;
            MatchCollection matches = Regex.Matches(query, pattern);

            foreach (object r in matches)
            {
                string dateFormat = r.ToString().Replace("{", "").Replace("}", "");

                string datePart = dateTime.ToString(dateFormat, CultureInfo.CreateSpecificCulture("en-US")).ToUpper();

                string replacedDate = r.ToString();

                parsedTicker = parsedTicker.Replace(replacedDate, datePart);

            }

            return parsedTicker;
        }

        public static string Parse(string tickerRule,string resolution,int offSet)
        {
            string parsedTicker = tickerRule;
            string query = tickerRule;
            MatchCollection matches = Regex.Matches(query, pattern);
            
            foreach (object r in matches)
            {
                string datePart = ParseDate(r.ToString(), resolution, offSet);
                string replacedDate = r.ToString();

                parsedTicker = parsedTicker.Replace(replacedDate, datePart);
            }

            return parsedTicker;
        }

        private static string ParseDate(string dateFormat, string resolution, int offSet)
        {
            dateFormat = dateFormat.Replace("{", "").Replace("}", "");
            if (dateFormat.Contains("MMMM")) return GetStartDate(DateTime.Today, resolution, offSet).ToString(dateFormat.Replace("MMMM","MMM"), CultureInfo.CreateSpecificCulture("en-US"));
            
            switch (resolution.ToUpper())
            {
                case "QUARTER":
                    DateTime startDate = GetStartDate(DateTime.Today, resolution, offSet);
                    int quarter = (int)Math.Floor(((decimal)startDate.Month + 2) / 3);
                    string adjDateFormat= dateFormat.Replace("q", "q" + quarter);
                    return GetStartDate(DateTime.Today, resolution, offSet).ToString(adjDateFormat, CultureInfo.CreateSpecificCulture("en-US")).ToUpper();
                    
                default:
                    return GetStartDate(DateTime.Today, resolution, offSet).ToString(dateFormat, CultureInfo.CreateSpecificCulture("en-US")).ToUpper();
            }
        }

        public static DateTime GetStartDate(DateTime dateTime, string resolution, int offSet)
        {
            switch (resolution.ToUpper())
            {
                case "MONTH":
                    return new DateTime(dateTime.Year,dateTime.Month,1).AddMonths(offSet);
                
                case "DAY":
                   return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).AddDays(offSet);
                
                case "WORKDAY":
                {
                    DayOfWeek day = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).AddDays(offSet).DayOfWeek;
                    if (day ==DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).AddDays(offSet+2);
                    
                    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).AddDays(offSet);
                }

                case "QUARTER":
                {
                        dateTime = dateTime.AddMonths(3*offSet);
                        int quarter = (int)Math.Floor(((decimal)dateTime.Month + 2) / 3);
                        switch (quarter)
                        {
                            case 1:
                                {
                                    return new DateTime(dateTime.Year, 1, 1);
                                }

                            case 2:
                                {
                                    return new DateTime(dateTime.Year, 4, 1);
                                }
                            case 3:
                                {
                                    return new DateTime(dateTime.Year, 7, 1);
                                }
                            case 4:
                                {
                                    return new DateTime(dateTime.Year, 10, 1);
                                }
                        }
                        break;
                    }
                case "YEAR":
                    return new DateTime(dateTime.Year, 1, 1).AddYears(offSet);
                    
                default:
                    throw new ArgumentException(resolution+" Not supported");
            }
            return new DateTime();
        }

        public static DateTime GetToDate(DateTime dateTime, string resolution, int offSet)
        {
            switch (resolution.ToUpper())
            {
                case "MONTH":
                    return GetStartDate(dateTime,resolution,offSet).AddMonths(1).AddDays(-1);

                case "DAY":
                    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).AddDays(offSet);

                case "WORKDAY":
                    {
                        DayOfWeek day = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).AddDays(offSet).DayOfWeek;
                        if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).AddDays(offSet + 2);

                        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).AddDays(offSet);
                    }

                case "QUARTER":
                    return GetStartDate(dateTime, resolution, offSet).AddMonths(3).AddDays(-1);
                    
                case "YEAR":
                    return GetStartDate(dateTime, resolution, offSet).AddYears(1).AddDays(-1);
                    
                default:
                    throw new ArgumentException(resolution + " Not supported");
            }
        }
    }
}
