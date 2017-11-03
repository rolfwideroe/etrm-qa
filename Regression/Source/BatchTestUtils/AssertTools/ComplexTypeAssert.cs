using System;
using System.Data;
using NUnit.Framework;

namespace ElvizTestUtils.AssertTools
{
    public class ComplexTypeAssert
    {
        public static void AssertDataTables(DataTable expectedDataTable, DataTable actualDataTable,string[] ignoreColumns, double tollerance = GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE)
        {
            if (ignoreColumns != null)
            {
                foreach (string ignoreColumn in ignoreColumns)
                {
                    expectedDataTable.Columns.Remove(ignoreColumn);
                    actualDataTable.Columns.Remove(ignoreColumn);
                }
            }
            AssertDataTables(expectedDataTable,actualDataTable,tollerance);
            
        }

        public static void AssertDataTables(DataTable expectedDataTable, DataTable actualDataTable, double tollerance = GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE)
        {
            Assert.AreEqual(expectedDataTable.Rows.Count, actualDataTable.Rows.Count, "The Records in report does not match expected");

            //commented out  - use in case when should be no records in datawarehouse, no exposure expected for end of day
            //if(expectedDataTable.Rows.Count==0&&actualDataTable.Rows.Count==0) Assert.Fail("Expected and Actual Datatable contains no data");

            Assert.AreEqual(expectedDataTable.Columns.Count, actualDataTable.Columns.Count, "Number of Columns are not equal");

            for (int i = 0; i < expectedDataTable.Columns.Count; i++)
            {
                string expectedName = expectedDataTable.Columns[i].ColumnName;
                string actualName = actualDataTable.Columns[i].ColumnName;

                Assert.AreEqual(expectedName, actualName, "Table name : "+expectedDataTable.TableName +"\nColumn Name does not match, ColumnIndex=" + i);

                Type expectedType = expectedDataTable.Columns[i].DataType;
                Type actualType = actualDataTable.Columns[i].DataType;

                Assert.AreEqual(expectedType, actualType, "Table name : " + expectedDataTable.TableName + "\nColumn Data Type does not match, ColumnIndex=" + i + " ColumnName=" + expectedName);
            }



            for (int i = 0; i < actualDataTable.Rows.Count; i++)
            {
                for (int j = 0; j < actualDataTable.Columns.Count; j++)
                {
                    object expected = expectedDataTable.Rows[i][j];
                    object actual = actualDataTable.Rows[i][j];

                    string errorMessage = "Table name : " + expectedDataTable.TableName + "\nError on RowIndex=" + i + " Column=" + actualDataTable.Columns[j].ColumnName;

                    if (actualDataTable.Columns[j].DataType == typeof(double))
                    {
                        AssertDoubleNullable(expected, actual, errorMessage,tollerance);
                    }
                    else if (actualDataTable.Columns[j].DataType == typeof(string))
                    {
                        string expectedString = expected as string;
                        string actualString = actual as string;

                        if(!(string.IsNullOrEmpty(expectedString)&&string.IsNullOrEmpty(actualString)))
                            Assert.AreEqual(expectedString,actualString);
                    }
                    else
                    {
                        Assert.AreEqual(expected, actual, errorMessage);
                    }

                }
            }
        }

        public  static void AssertDoubleNullable(object expectedOject, object actualObject, string errorMessage, double tollerance = GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE)
        {
            double? expected = expectedOject as double?;
            double? actual = actualObject as double?;


            AssertDoubleNullable(expected,actual,errorMessage,tollerance);

        }

        public static void AssertDoubleNullable(double? expected, double? actual, string errorMessage, double tollerance = GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE)
        {
            if (expected == null && actual == null) return;

            if (expected == null) Assert.Fail(errorMessage + "\n Expected is null, actual is :" + actual);
            if (actual == null) Assert.Fail(errorMessage + "\n Actual is null, Expected is : " + expected);

            double expectedDouble = (double)expected;
            double actualDouble = (double)actual;


            AssertDouble(expectedDouble, actualDouble, errorMessage,tollerance);
        }

        public static void AssertDouble(double expected, double actual, string errorMessage,double tollerance = GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE)
        {
            

            if (Math.Abs(actual) >= 1e-7 || Math.Abs(expected) >= 1e-7)
            {
                errorMessage += "\n  Expected : " + expected + ", but Actual was : " + actual + "\n  With relative error Tollerance of: " + tollerance;
                double relativeError = (actual / expected) - 1;
                Assert.AreEqual(0, relativeError, tollerance, errorMessage);
            }

        }
    }
}
