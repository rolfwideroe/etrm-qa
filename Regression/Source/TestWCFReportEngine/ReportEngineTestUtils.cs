using System;
using System.Collections.Generic;
using ElvizTestUtils;
using ElvizTestUtils.AssertTools;
using ElvizTestUtils.InternalReportingServiceReference;
using ElvizTestUtils.ReportEngineTests;
using NUnit.Framework;
using System.Data;
using System.Linq;

namespace TestWCFReportEngine
{
	public static class ReportEngineTestUtils
	{
		
		/// <summary>
		/// Checks all ExpectedDataTableArtifacts and ExpectedTimeSeriesArtifacts defined in the test object. 
		/// If errorTableArtifactId is set, this will be checked too.
		/// </summary>
		public static void TestAllArtifacts(TestCaseReportEngine test, string errorTableArtifactId = null)
		{
		    ElvizConfiguration[] elvizConfigurations = test.InputData.ElvizConfigurations;

		    try
		    {

                if (elvizConfigurations != null)
                {
                    ElvizConfigurationTool utility = new ElvizConfigurationTool();

                    utility.UpdateConfiguration(elvizConfigurations);
                }


		        IInternalReportingApiService service = WCFClientUtil.GetInternalReportingServiceProxy();
		        Setting[] reportSettings = ReportEngineTestHelper.GetReportSettings(test, service);

		        List<string> completeArtifactList = new List<string>();

		        ExpectedDataTableArtifact[] expectedDataTableArtifacts = test.ExpectedValues.ExpectedDataTableArtifacts;
		        if (expectedDataTableArtifacts != null)
		        {
		            IEnumerable<string> expectedDataTableIDs = expectedDataTableArtifacts.Select(x => x.ArtifactId);
		            completeArtifactList.AddRange(expectedDataTableIDs);
		        }

		        ExpectedTimeSeriesArtifact[] expectedTimeSeriesArtifacts = test.ExpectedValues.ExpectedTimeSeriesArtifacts;
		        if (expectedTimeSeriesArtifacts != null)
		        {
		            IEnumerable<string> expectedTimeSeriesArtifactIds = expectedTimeSeriesArtifacts.Select(x => x.ArtifactId);
		            completeArtifactList.AddRange(expectedTimeSeriesArtifactIds);
		        }

		        if (errorTableArtifactId != null)
		            completeArtifactList.Add(errorTableArtifactId);

		        Report report = null;
		        try
		        {
		            report = service.RunReport(test.InputData.ReportId, reportSettings, completeArtifactList.ToArray());
		        }
		        catch (Exception e)
		        {
		            Assert.Fail(e.Message);
		        }

		        if (errorTableArtifactId != null)
		        {
		            

		            ExpectedDataTableArtifact expectedErrorDataTableArtifact=null;

                    if(expectedDataTableArtifacts!=null)
                        expectedErrorDataTableArtifact = test.ExpectedValues.ExpectedDataTableArtifacts.SingleOrDefault(x => x.ArtifactId.Equals(errorTableArtifactId));


		            if (expectedErrorDataTableArtifact == null)
		            {
		                if (ReportHasErrors(report))
		                {
                            DataTable errorDataTable = ReportEngineTestHelper.GetDataTableArtifactById("errorTable", report.DataTableArtifacts, -1);
                            string errorMessage = "The report contains errors:\n";
                            errorMessage += ReportEngineTestHelper.PrintDataTableToString(errorDataTable);
                            Assert.Fail(errorMessage);
		                }
		            }
		            else
		            {
		                if(!ReportHasErrors(report)) Assert.Fail("Expected to have Error Datatable artifact");
		                else
		                {
                            DataTable errorDataTable = ReportEngineTestHelper.GetDataTableArtifactById("errorTable", report.DataTableArtifacts, -1);
		                    ComplexTypeAssert.AssertDataTables(expectedErrorDataTableArtifact.DataTable,errorDataTable);
                            return;
		                }
		            }


		            

		        }

		        TestDataTableArtifacts(report, expectedDataTableArtifacts, errorTableArtifactId);
		        TestTimeSeriesArtifacts(report, test.ExpectedValues.ExpectedTimeSeriesArtifacts);

		    }
		    finally
		    {
                if (elvizConfigurations != null)
                {
                    ElvizConfigurationTool utility = new ElvizConfigurationTool();

                    utility.RevertAllConfigurationsToDefault();
                }
		    }
		}

		private static void TestDataTableArtifacts(Report report, ExpectedDataTableArtifact[] expectedDataTableArtifacts, string errorTableArtifactId)
		{
			if (expectedDataTableArtifacts == null || !expectedDataTableArtifacts.Any())
				return;

			string[] expectedArtifactIds = expectedDataTableArtifacts.OrderBy(y => y.ArtifactId).Select(x => x.ArtifactId).ToArray();
			List<DataTableArtifact> dataTableArtifacts = report.DataTableArtifacts.ToList();
			DataTableArtifact errorArtifact = dataTableArtifacts.SingleOrDefault(x => x.ArtifactId.Equals(errorTableArtifactId));
			dataTableArtifacts.Remove(errorArtifact);
			string[] actualArtifactIds = dataTableArtifacts.OrderBy(y => y.ArtifactId).Select(x => x.ArtifactId).ToArray();
			Assert.AreEqual(expectedArtifactIds.Length, actualArtifactIds.Length, "Expected number of Datatable Artifacts did not match :");

			foreach (ExpectedDataTableArtifact expectedDataTableArtifact in expectedDataTableArtifacts)
			{
				DataTable expectedDataTable = expectedDataTableArtifact.DataTable;
                DataTable actualDataTable = ReportEngineTestHelper.GetDataTableArtifactById(expectedDataTableArtifact.ArtifactId, report.DataTableArtifacts, expectedDataTableArtifact.TransactionIdColumnIndex);
				
      
                // PrintReportToConsole(actualDataTable);
				Assert.NotNull(actualDataTable, expectedDataTableArtifact.ArtifactId + " Does not exist in the Report");
				Assert.IsFalse(actualDataTable.HasErrors);

				ComplexTypeAssert.AssertDataTables(expectedDataTable, actualDataTable);
			}
		}

		private static void TestTimeSeriesArtifacts(Report report, ExpectedTimeSeriesArtifact[] expectedTimeSeriesArtifacts)
		{
			if (expectedTimeSeriesArtifacts == null || !expectedTimeSeriesArtifacts.Any())
				return;

			TimeSeriesGroupArtifact[] timeSeriesGroupArtifacts = report.TimeSeriesArtifacts;
			foreach (ExpectedTimeSeriesArtifact expectedTimeSeriesArtifact in expectedTimeSeriesArtifacts)
			{
				DataTable actualTimeSeriesArtifactAsDataTable = ReportEngineTestHelper.GetTimeSeriesArifactAsDataTable(expectedTimeSeriesArtifact, timeSeriesGroupArtifacts);
				//  ReportEngineTestHelper.PrintReportToConsole(actualTimeSerieArtifactAsDataTable);
				DataTable expectedTimeSerieArtifactAsTable = expectedTimeSeriesArtifact.DataTable;

				ComplexTypeAssert.AssertDataTables(expectedTimeSerieArtifactAsTable, actualTimeSeriesArtifactAsDataTable);
			}
		}

	    private static bool ReportHasErrors(Report report)
	    {
	        if (report.DataTableArtifacts == null) return false;

            DataTable errorDataTable = ReportEngineTestHelper.GetDataTableArtifactById("errorTable",report.DataTableArtifacts, -1);

	        if (errorDataTable == null) return false;

	        if (errorDataTable.Rows.Count == 0) return false;

	        return true;
	    }

	}
}
