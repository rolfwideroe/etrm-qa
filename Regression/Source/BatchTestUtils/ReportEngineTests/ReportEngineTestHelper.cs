using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using ElvizTestUtils.AssertTools;
using ElvizTestUtils.BatchTests;
using ElvizTestUtils.DatabaseTools;
using ElvizTestUtils.InternalReportingServiceReference;
using ElvizTestUtils.QaLookUp;
using Property = ElvizTestUtils.InternalReportingServiceReference.Property;
using TimeSeries = ElvizTestUtils.InternalReportingServiceReference.TimeSeries;

namespace ElvizTestUtils.ReportEngineTests
{
	public class ReportEngineTestHelper
	{
		public static DataTable GetDataTableArtifactById(string artifactId, DataTableArtifact[] dataTableArtifact,int transactionIdColumn)
		{
			DataTableArtifact artifact = dataTableArtifact.SingleOrDefault(x => x.ArtifactId == artifactId);

		    if (artifact == null) return null;
		    
            DataTable artifactTable = artifact.DataTable;

		    if (transactionIdColumn == -1)
		    {
		        return artifactTable;
		    }

            QaDao qaDao = new QaDao();

		    foreach (DataRow row in artifactTable.Rows)
		    {
		        // translate from externalIds to transactionIds
                string[] transIdsAsString= row[transactionIdColumn].ToString().Split(';');

		        int[] transactionIds = Array.ConvertAll(transIdsAsString, int.Parse);
                string[] externalIds = qaDao.GetExternalIdsFromTransactionIds(transactionIds);

                row[transactionIdColumn] = string.Join(";", externalIds);

		    }

		    string transactionIdColumnName = artifactTable.Columns[transactionIdColumn].ColumnName;

		    artifactTable.DefaultView.Sort = transactionIdColumnName + " asc";

		    artifactTable = artifactTable.DefaultView.ToTable();

		    return artifactTable;
		}

		public static string[] GetExpectedArtifactIds(TestCaseReportEngine testCaseReportEngine)
		{
			IList<string> artifactIds = new List<string>();

			if (testCaseReportEngine.ExpectedValues.ExpectedDataTableArtifacts != null)
			{
				foreach (
					ExpectedDataTableArtifact expectedDataTableArtifact in
						testCaseReportEngine.ExpectedValues.ExpectedDataTableArtifacts)
				{
					artifactIds.Add(expectedDataTableArtifact.ArtifactId);
				}
			}

			if (testCaseReportEngine.ExpectedValues.ExpectedTimeSeriesArtifacts != null)
			{
				foreach (
					ExpectedTimeSeriesArtifact expectedTimeSeriesArtifact in
						testCaseReportEngine.ExpectedValues.ExpectedTimeSeriesArtifacts)
				{
					artifactIds.Add(expectedTimeSeriesArtifact.ArtifactId);
				}
			}

			return artifactIds.ToArray();
		}

		public static DataTable GetTimeSeriesArifactAsDataTable(ExpectedTimeSeriesArtifact expectedTimeSeriesArtifact, TimeSeriesGroupArtifact[] timeSeriesGroupArtifacts)
		{

			TimeSeriesGroupArtifact timeSeriesGroupArtifact = timeSeriesGroupArtifacts.FirstOrDefault(x => x.ArtifactId == expectedTimeSeriesArtifact.ArtifactId);
			if (timeSeriesGroupArtifact == null)
				return null;

			DataTable table = new DataTable("TimeSeriesReport");

			TestProperties[] testedTimeSeriesProperties = expectedTimeSeriesArtifact.TimeSeriesProperties;

			string sortString = "";

			foreach (TestProperties timeSeriesProperty in testedTimeSeriesProperties)
			{
				Type type = Type.GetType(timeSeriesProperty.DataType);

				if (type == null) throw new ArgumentException("Datatype not known : " + timeSeriesProperty.Name);

				table.Columns.Add(timeSeriesProperty.Name, type);
				sortString += timeSeriesProperty.Name + ",";
			}


			table.Columns.Add("TimeZone", typeof(string));
			table.Columns.Add("From", typeof(DateTime));
			table.Columns.Add("Until", typeof(DateTime));
			table.Columns.Add("Value", typeof(double));

			sortString += "TimeZone,From,Until,Value";


			TimeSeries[] ts = timeSeriesGroupArtifact.TimeSeries;

			if (ts != null)
			{
				foreach (TimeSeries timeSerie in ts)
				{
					TimeSeriesElement[] elements = timeSerie.TimeSeriesElements;

					Property[] properties = timeSerie.Properties;

					string prop = "";

					foreach (Property property in properties)
					{
						prop += property.PropertyName + "-" + property.PropertyValue + ";";
					}

					IList<TestProperties> propWithValues = new List<TestProperties>(testedTimeSeriesProperties.Length);

					foreach (TestProperties timeSeriesProperty in testedTimeSeriesProperties)
					{
						string propValue =
							timeSerie.Properties.Where(x => x.PropertyName == timeSeriesProperty.Name)
								.Select(x => x.PropertyValue)
								.FirstOrDefault();
						propWithValues.Add(new TestProperties() { Name = timeSeriesProperty.Name, Value = propValue });
					}

					string timeZone = timeSerie.TimeZone;

					foreach (TimeSeriesElement element in elements)
					{
						DataRow row = table.NewRow();

						foreach (TestProperties testProperty in propWithValues)
						{
							row[testProperty.Name] = testProperty.Value;
						}

						row["TimeZone"] = timeZone;
						row["From"] = element.FromTime;
						row["Until"] = element.UntilTime;
						row["Value"] = element.Value;

						table.Rows.Add(row);
					}
				}
			}

			DataView view = table.DefaultView;

			view.Sort = sortString;

			DataTable sortedTable = view.ToTable();

			return sortedTable;
		}

		public static Setting[] GetReportSettings(TestCaseReportEngine test, IInternalReportingApiService service)
		{
			IList<Setting> reportSettings = new List<Setting>();

			foreach (TestSetting testSetting in test.InputData.Settings)
			{
				reportSettings.Add(new Setting() { SettingId = testSetting.SettingId, IsNotSet = testSetting.IsNotSet, XmlEncodedValue = testSetting.XmlEncodedValue });
			}

			List<Setting> defaultSettings = service.GetPredefinedSettingsSet(test.InputData.ReportId, "Default").ToList();
			foreach (Setting defaultSetting in defaultSettings)
			{
				bool settingAlreadyExists = reportSettings.Any(x => x.SettingId.Equals(defaultSetting.SettingId));
				if (!settingAlreadyExists)
				{
					reportSettings.Add(defaultSetting);
				}
			}

			return reportSettings.ToArray();
		}

		public static string PrintDataTableToString(DataTable table)
		{
			string outputString = "";
			string colString = "";

			foreach (DataColumn column in table.Columns)
			{
				if (colString.Length > 0) colString += ",";
				colString += column.ColumnName;
			}

			outputString += colString + "\n";

			foreach (DataRow dataRow in table.Rows)
			{
				string rowString = "";
				foreach (DataColumn column in table.Columns)
				{
					if (rowString.Length > 0) rowString += ",";

					if (column.DataType == typeof(DateTime))
					{
					    if (dataRow[column.ColumnName] == DBNull.Value)
                        {     rowString += "NULL";}
					    else
					    {
                            DateTime dateTime = (DateTime)dataRow[column.ColumnName];
                            rowString += dateTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                        }
 
					}
					else rowString += dataRow[column.ColumnName];

					//rowString += ",";
				}

				outputString += rowString + "\n";
			}

			return outputString;
		}

		public static void PrintReportToConsole(DataTable table)
		{
			string colString = "";

			foreach (DataColumn column in table.Columns)
			{
				if (colString.Length > 0) colString += ",";
				colString += column.ColumnName;
			}

			Console.WriteLine(colString);

			foreach (DataRow dataRow in table.Rows)
			{
				string rowString = "";
				foreach (DataColumn column in table.Columns)
				{
					if (rowString.Length > 0) rowString += ",";
					else rowString = "<ExpectedRecord>";
					if (column.DataType == typeof(DateTime))
					{
                        if (dataRow[column.ColumnName] == DBNull.Value)
                        { rowString += "NULL"; }
                        else
                        {
                            DateTime dateTime = (DateTime)dataRow[column.ColumnName];
                            rowString += dateTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                        }
                    }
					else rowString += dataRow[column.ColumnName];

					//rowString += ",";
				}
				rowString += "</ExpectedRecord>";
				Console.WriteLine(rowString);
			}
		}

	    public static DataTable CreateDataTableFromExpectedRecords(string tableName, string columnsAsCSV, string columnDataTypesAsCSV,string[] expectedRecords, string dateTimeFormat)
	    {
            string[] columnsStrings = columnsAsCSV.Split(',');
            string[] columnsDataTypesStrings = columnDataTypesAsCSV.Split(',');

            if (columnsStrings.Length != columnsDataTypesStrings.Length)
                throw new ArgumentException("Columns and ColumnsDataTypes does not have the same number of elements by comma seperator");

            Column[] columns=new Column[columnsStrings.Length];

            int columnLength = columnsStrings.Length;
            for (int i = 0; i < columnLength; i++)
            {
                Column column=new Column {ColumnDataType = columnsDataTypesStrings[i], ColumnName = columnsStrings[i]};

                columns[i] = column;
            }

	        return DataTableHelper.CreateDataTableFromExpectedRecords(tableName, columns, expectedRecords, dateTimeFormat);
	    }
	}
}
