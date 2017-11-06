using System;
using System.Data;
using System.Globalization;
using ElvizTestUtils.AssertTools;

namespace ElvizTestUtils.ReportEngineTests
{
    public class DataTableHelper
    {
        public static DataTable CreateDataTableFromExpectedRecords(string tableName,Column[] columns, string[] expectedRecords,string dateTimeFormat)
        {
            DataTable dataTable = new DataTable();
            dataTable.TableName = tableName;
            int columnLength = columns.Length;

            for (int i = 0; i < columnLength; i++)
            {
                Type type = Type.GetType(columns[i].ColumnDataType);
                if (type == null)
                    throw new ArgumentException("Datatype not known : " + columns[i].ColumnName);

                string columnString = columns[i].ColumnName;

                dataTable.Columns.Add(columnString, type);
            }


            CultureInfo provider = CultureInfo.InvariantCulture;

            for (int i = 0; i < expectedRecords?.Length; i++)
            {
                string recordString = expectedRecords[i];
                string[] records = recordString.Split(',');
                if (records.Length != columnLength)
                    throw new ArgumentException("Record number : " + i + " does not have correct number column elements.\n  Expected : " + columnLength + "\n  But was  : " + records.Length);

                object[] rowObjects = new object[columnLength];
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    Type columnType = dataTable.Columns[j].DataType;
                    string columnName = dataTable.Columns[j].ColumnName;
                    string elementString = records[j];
                    rowObjects[j] = records[j];

                    //if (j == transactionIdColumnIndex)
                    //{
                    //    // translate from externalIds to transactionIds
                    //    string[] externalIds = elementString.Split(';');
                    //    int[] transactionIds = QaLookUpClient.GetTransactionIdByExternalId(externalIds);
                    //    rowObjects[j] = string.Join(";", transactionIds);
                    //}
					
                    if (columnType == typeof(string))
                    {
                        if (string.IsNullOrEmpty(elementString) || elementString == "NULL")
                            rowObjects[j] = null;
                        else rowObjects[j] = elementString;
                    }
                    else if (columnType == typeof(double))
                    {
                        if (string.IsNullOrEmpty(elementString) || elementString == "NULL")
                            rowObjects[j] = null;
                        else
                        {
                            double doubleValue;
                            if (double.TryParse(elementString, NumberStyles.Any, provider, out doubleValue))
                                rowObjects[j] = doubleValue;
                            else
                                rowObjects[j] = null;
                        }
                    }
                    else if (columnType == typeof(DateTime))
                    {
                        if (string.IsNullOrEmpty(elementString) || elementString == "NULL")
                            rowObjects[j] = null;
                        else
                        {
                            DateTime dateTimeValue;
                            if (DateTime.TryParseExact(elementString, dateTimeFormat, provider, DateTimeStyles.None, out dateTimeValue))
                            {
                                rowObjects[j] = dateTimeValue;
                            }
                            else
                            {
                                throw new ArgumentException(elementString + " Does not match expected DateTime Format: " + dateTimeFormat+" ColumnName : "+columnName+", RowIndex : "+i);
                            }
                        }
                    }
                    else if (columnType == typeof(bool))
                    {
                        if (string.IsNullOrEmpty(elementString))
                            rowObjects[j] = null;
                        else
                        {
                            bool boolValue;
                            if (bool.TryParse(elementString, out boolValue))
                            {
                                rowObjects[j] = boolValue;
						        
                            }
                            else
                            {
                                switch (elementString)
                                {
                                    case "1":
                                        rowObjects[j] = true;
                                        break;
                                    case "0":
                                        rowObjects[j] = false;
                                        break;
                                    default:
                                        throw new ArgumentException("Could not parse : " + elementString + " to type System.Bool, ColumnName : " + columnName);
                                }						        
                            }
                        }
                    }
                }
                dataTable.Rows.Add(rowObjects);
            }
            return dataTable;
        }
    }
}