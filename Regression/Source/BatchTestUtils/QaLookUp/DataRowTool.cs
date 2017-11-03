using System;
using System.Data;

namespace ElvizTestUtils.QaLookUp
{
    public class DataRowTool
    {
        public static string GetColumnStringValue(DataRow row, string columnName)
        {
            if (row[columnName] == DBNull.Value) return null;

            return (string)row[columnName];
        }

        public static int? GetColumnIntValue(DataRow row, string columnName)
        {

            if (row[columnName] == DBNull.Value) return null;

            return (int)row[columnName];
        }

        public static double? GetColumnDoubleValue(DataRow row, string columnName)
        {

            if (row[columnName] == DBNull.Value) return null;

            return (double)row[columnName];
        }

        public static bool? GetColumnBoolValue(DataRow row, string columnName)
        {
            if (row[columnName] == DBNull.Value) return null;

            return (bool)row[columnName];
        }

        public static TimeSpan? GetColumnTimeSpanValue(DataRow row, string columnName)
        {

            if (row[columnName] == DBNull.Value) return null;
            return (TimeSpan)row[columnName];
        }

        public static DateTime? GetColumnDateTimeValue(DataRow row, string columnName)
        {

            if (row[columnName] == DBNull.Value) return null;

            return (DateTime)row[columnName];
        }
    }
}