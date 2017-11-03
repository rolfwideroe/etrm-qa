using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace ElvizTestUtils.CurveTests
{
    public class ExpectedCurveValue
    {
        [XmlAttribute("Date")]
        public DateTime Date { get; set; }

        [XmlAttribute("Value")]
        public double Value { get; set; }
    }

    public class CurveTestHelper
    {
        public static DataTable ExpectedCurveValuesToData(ExpectedCurveValue[] expectedCurveValues)
        {

            DataTable table = new DataTable();
            table.Columns.Add("Date", typeof(DateTime));
            table.Columns.Add("Value", typeof(double));

            foreach (ExpectedCurveValue curveValue in expectedCurveValues)
            {
                table.Rows.Add(new object[] {curveValue.Date, curveValue.Value});
            }

            return table;
        }

        public static DataTable KeyValuePairsToDataTable(KeyValuePair<DateTime, double>[] keyValuePairs)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Date", typeof(DateTime));
            table.Columns.Add("Value", typeof(double));

            foreach (KeyValuePair<DateTime, double> keyValuePair in keyValuePairs)
            {
                table.Rows.Add(new object[] {keyValuePair.Key, keyValuePair.Value});

            }

            return table;
        }
    }
}