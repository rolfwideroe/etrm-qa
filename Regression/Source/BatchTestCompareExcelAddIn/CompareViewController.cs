using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using ElvizTestUtils;
using ElvizTestUtils.AssertTools;
using ElvizTestUtils.BatchTests;
using ElvizTestUtils.CurveServiceReference;
using ElvizTestUtils.CurveTests;
using ElvizTestUtils.CustomDwhTest;
using ElvizTestUtils.DatabaseTools;
using ElvizTestUtils.InternalReportingServiceReference;
using ElvizTestUtils.QaLookUp;
using ElvizTestUtils.ReportEngineTests;
using ElvizTestUtils.ReportingDbTimeSeries;

namespace BatchTestCompareExcelAddIn
{
    public class CompareViewController
    {
        private readonly ICompareView view;
        private QaDao qaDao;

        public CompareViewController(ICompareView view)
        {
            this.view = view;

            try
            {
                this.qaDao = new QaDao();
            }
            catch (Exception ex)
            {
                
                this.view.ShowApplicationError(ex.ToString());
            }

            
        }

        public void CreateBatchTestFiles()
        {
            try
            {
                IList<BatchTestFile> testFiles = this.view.BatchTestFilesStructure;

                string folderPath = this.view.ExportBatchTestFilesPath;

               

                foreach (BatchTestFile batchTestFile in testFiles)
                {
                    string path = Path.Combine(folderPath, batchTestFile.Name);
                    TestXmlTool.SerializeToXml(batchTestFile, path);
                }

                this.view.ShowMessage("Export Ok");
            }
            catch (Exception ex)
            {
                
                this.view.ShowApplicationError(ex.ToString());
            }
          

        }

        public void CompareRdTimeSeriesFromEcmTests()
        {
            const string EcmTestFolder = "..\\ECMBatch\\TestFiles\\";
            try
            {

           
            IList<string> testFilePaths = this.view.XmlFilePaths;

            foreach (string testFilePath in testFilePaths)
            {
                BatchTestFile batchTest = BatchTestFileParser.DeserializeXml(testFilePath);

                DataTable expecteDataTable = batchTest.Assertions[0].ExpectedDataTable;

                string filter = TestReportingDbUtil.GetFilterFromBatchTest(batchTest);
                string fileName = Path.GetFileName(testFilePath);
                string jobAlias = batchTest.Setup.Workspace;
                        //"";if (fileName != null) jobAlias = fileName.Replace(".xml", "");

                DateTime reportDate = batchTest.Setup.ReportDate;

                DataTable actualDataTable = TestReportingDbUtil.GetLatestExecutionActualDataTableFromEcm(jobAlias,
                    reportDate,
                    batchTest.Assertions[0],testFilePath);

                DataTable actualDataAsXml = ActualDataAsXml(actualDataTable, "record", true);

				DataTable testData = new DataTable();
                testData.Columns.Add("Key", typeof (string));
                testData.Columns.Add("Value", typeof (string));
                testData.Rows.Add(new object[] {"Test Name", batchTest.Name});
                testData.Rows.Add(new object[] {"Workspace", batchTest.Setup.Workspace});
                testData.Rows.Add(new object[] {"Filter", filter});
                testData.Rows.Add(new object[] {"ReportDate", reportDate.ToString("yyyy-MM-dd")});
                testData.Rows.Add(new object[] {"Tollerance", GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE});
                testData.Rows.Add(new object[] {"Test File", Path.Combine(this.view.CurrentFileDialogDirectory, testFilePath)});

                ElvizConfiguration[] elvizConfigurations = batchTest.Setup.ElvizConfigurations;

                if (elvizConfigurations != null)
                {
                    foreach (ElvizConfiguration configuration in elvizConfigurations)
                    {
                        string excelName = "ElvizConfiguartion-" + configuration.Name;
                        testData.Rows.Add(new object[] {excelName, configuration.Value});
                    }
                }

                this.view.ShowCompare(testData, actualDataTable, expecteDataTable, actualDataAsXml, GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE);
            }
        }

             catch (Exception ex)
            {
                string errorString = ex.Message + "\n\n" + ex.StackTrace;

                this.view.ShowApplicationError(errorString);
            }
        }

        public void CompareRdTimeSeriesTests()
        {



            try
            {


                IList<string> testFilePaths = this.view.XmlFilePaths;


                foreach (string testFilePath in testFilePaths)
                {
                    ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(testFilePath);
                    string type = testCase.ExpectedValues.ExpectedDataTable.DataType;

                    DataTable expecteDataTable;
					
                    string jobName = testCase.InputData.JobName;
                    DateTime reportDate = testCase.InputData.ReportDate;
                    string fileName = Path.GetFileName(testFilePath);
                    if (fileName == null) throw new ArgumentException("Missing filepath");
                    string testName = fileName.Replace(".xml", "");

                    int? latestJobExecutioid = this.qaDao.GetLatestExecutionIdForJob(jobName);

                    if (latestJobExecutioid == null)
                        throw new ArgumentException("The Job : " + testCase.InputData.JobName +
                                                    " has not been run or does not exists. Pls run it first to compare");

                    DataTable actualDataTable;
                    switch (type)
                    {
                        case "TimeSeries":
                            expecteDataTable = TestReportingDbUtil.GetExpectedTimeSeriesDataTable(testCase, testFilePath);
                            actualDataTable = TestReportingDbUtil.GetActualTimeSeriesDataTable(jobName, reportDate, latestJobExecutioid.Value, testFilePath);
                            break;
                        case "KeyValues":
                            expecteDataTable = TestReportingDbUtil.GetExpectedKeyValuesDataTable(testCase, testFilePath);
                            actualDataTable = TestReportingDbUtil.GetActualKeyValuesDataTable(jobName, reportDate, latestJobExecutioid.Value, testFilePath);
                            break;
                        case "BCRContractExport":
                            expecteDataTable = TestReportingDbUtil.GetExpectedBcrContractExportDataTable(testCase, testFilePath);
                            actualDataTable = TestReportingDbUtil.GetActualBcrContractExportDataTable(jobName, testFilePath);
                            break;
                        default:
                            this.view.ShowUserError("Unknown type : " + type);
                            return;


                    }

                   

                    DataTable testData = new DataTable();
                    testData.Columns.Add("Key", typeof (string));
                    testData.Columns.Add("Value", typeof (string));
                    testData.Rows.Add(new object[] {"Test Name", testName});
                    testData.Rows.Add(new object[] {"JobName", jobName});
                    testData.Rows.Add(new object[] {"ReportDate", reportDate.ToString("yyyy-MM-dd")});
                    testData.Rows.Add(new object[] {"Tollerance", GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE});
                    testData.Rows.Add(new object[] {"Test File", Path.Combine(this.view.CurrentFileDialogDirectory, testFilePath)});
                    testData.Rows.Add(new object[] {"Type",type });

                    ElvizConfiguration[] elvizConfigurations = testCase.InputData.ElvizConfigurations;

                    if (elvizConfigurations != null)
                    {
                        foreach (ElvizConfiguration configuration in elvizConfigurations)
                        {
                            string excelName = "ElvizConfiguartion-" + configuration.Name;
                            testData.Rows.Add(new object[] {excelName, configuration.Value});
                        }
                    }

                    DataTable actualDataAsXml = ActualDataAsXml(actualDataTable, "ExpectedRecord", true);

                    this.view.ShowCompare(testData, actualDataTable, expecteDataTable, actualDataAsXml, GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE);
                }

            }
            catch (Exception ex)
            {
                string errorString = ex.Message + "\n\n" + ex.StackTrace;

                this.view.ShowApplicationError(errorString);
            }
        }


        public void CompareReportEngineTests()
        {
            try
            {
                InternalReportingApiServiceClient client = WCFClientUtil.GetInternalReportingServiceProxy();

                IList<string> testFilePaths = this.view.XmlFilePaths;

                foreach (string testFilePath in testFilePaths)
                {
                     TestCaseReportEngine test = TestXmlTool.Deserialize<TestCaseReportEngine>(testFilePath);

                        ElvizConfiguration[] elvizConfigurations = test.InputData.ElvizConfigurations;
                    try
                    {
                        if (elvizConfigurations != null)
                        {
                            ElvizConfigurationTool  tool=new ElvizConfigurationTool();
                            tool.UpdateConfiguration(elvizConfigurations);
                        }

	                    List<Setting> reportSettings = ReportEngineTestHelper.GetReportSettings(test, client).ToList();

                        string[] expectedArtifactIds = ReportEngineTestHelper.GetExpectedArtifactIds(test);

                        IList<string> expList = expectedArtifactIds.ToList();
                        expList.Add("errorTable");


                        Report report = client.RunReport(test.InputData.ReportId, reportSettings.ToArray(),
                            expList.ToArray());

                       

                        DataTable errorDataTable = ReportEngineTestHelper.GetDataTableArtifactById("errorTable", report.DataTableArtifacts, -1);

                        if (errorDataTable.Rows.Count>0)
                        {
                            this.view.ShowMessage("Report contains error");
                            
                            return;
                        }

                        string[] actualArtifactIds =
                            report.DataTableArtifacts.OrderBy(y => y.ArtifactId).Select(x => x.ArtifactId).ToArray();

                        string fileText = testFilePath.Split('\\').Last();
                        string testName = fileText.Split('.').First();

                        ExpectedDataTableArtifact[] expectedDataTableArtifacts =
                            test.ExpectedValues.ExpectedDataTableArtifacts;

                        if (expectedDataTableArtifacts != null)
                        {
                            foreach (ExpectedDataTableArtifact expectedDataTableArtifact in expectedDataTableArtifacts)
                            {
                                DataTable expectedDataTable = expectedDataTableArtifact.DataTable;
                                DataTable actualDataTable =
                                    ReportEngineTestHelper.GetDataTableArtifactById(
                                        expectedDataTableArtifact.ArtifactId,
                                        report.DataTableArtifacts, expectedDataTableArtifact.TransactionIdColumnIndex);



                                ExportReportEngineArtifacts(expectedDataTable, actualDataTable,
                                    expectedDataTableArtifact.ArtifactId, testName, test.InputData.Settings,
                                    testFilePath);
                            }
                        }

                        ExpectedTimeSeriesArtifact[] expectedTimeSeriesArtifacts =
                            test.ExpectedValues.ExpectedTimeSeriesArtifacts;

                        if (expectedTimeSeriesArtifacts != null)
                        {
                            foreach (
                                ExpectedTimeSeriesArtifact expectedTimeSeriesArtifact in expectedTimeSeriesArtifacts)
                            {
                                DataTable expectedDataTable = expectedTimeSeriesArtifact.DataTable;
                                DataTable actualDataTable =
                                    ReportEngineTestHelper.GetTimeSeriesArifactAsDataTable(expectedTimeSeriesArtifact,
                                        report.TimeSeriesArtifacts);

                                ExportReportEngineArtifacts(expectedDataTable, actualDataTable,
                                    expectedTimeSeriesArtifact.ArtifactId, testName, test.InputData.Settings,
                                    testFilePath);
                            }
                        }

                    }
                    finally
                    {
                        if (elvizConfigurations != null)
                        {
                            ElvizConfigurationTool tool = new ElvizConfigurationTool();
                            tool.RevertAllConfigurationsToDefault();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorString = ex.Message + "\n\n" + ex.StackTrace;

                this.view.ShowApplicationError(errorString);
            }
        }

        private void ExportReportEngineArtifacts(DataTable expectedDataTable,DataTable actualDataTable,string artifactId,string testName,TestSetting[] testSettings,string testFilePath)
        {
            if (actualDataTable == null || actualDataTable.Rows.Count == 0)
                this.view.ShowMessage("No Actual Data for Artifact : " + artifactId);

       

            DataTable newXmlDataTable = ActualDataAsXml(actualDataTable, "ExpectedRecord", false);

            DataTable testData = new DataTable();
            testData.Columns.Add("Key", typeof (string));
            testData.Columns.Add("Value", typeof (string));
            testData.Rows.Add(new object[] {"Test Name", testName});
            testData.Rows.Add(new object[] {"ArtiFact Id", artifactId});

            foreach (TestSetting setting in testSettings)
            {
                testData.Rows.Add(new object[] {setting.SettingId, setting.XmlEncodedValue});
            }

            testData.Rows.Add(new object[] { "Test File", Path.Combine(this.view.CurrentFileDialogDirectory, testFilePath) });

            this.view.ShowCompare(testData, actualDataTable, expectedDataTable, newXmlDataTable, GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE);
        }

        public void CompareExchangeRateTests()
        {
            try
            {
                IList<string> testFilePaths = this.view.XmlFilePaths;

                foreach (string testFilePath in testFilePaths)
                {
                    ExchangeRateTestCase testCase = TestXmlTool.Deserialize<ExchangeRateTestCase>(testFilePath);

                    KeyValuePair<DateTime, double>[] exchangeRates = ExchangeRateTestUtil.GetExchangeRates(testCase);

                    DataTable testData = new DataTable();
                    testData.Columns.Add("Key", typeof(string));
                    testData.Columns.Add("Value", typeof(string));

                    foreach (ElvizConfiguration elvizConfiguration in testCase.InputData.ElvizConfigurations)
                    {
                        testData.Rows.Add(new object[] {elvizConfiguration.Name, elvizConfiguration.Value });
                    }

                    testData.Rows.Add(new object[] { "BaseCurrency", testCase.InputData.BaseCurrencyISOCode });
                    testData.Rows.Add(new object[] { "CrossCurrency", testCase.InputData.CrossCurrencyISOCode });
                    testData.Rows.Add(new object[] { "CurrencySource", testCase.InputData.CurrencySource });
                    testData.Rows.Add(new object[] { "ReportDate", testCase.InputData.ReportDate });
                    testData.Rows.Add(new object[] { "LastDate", testCase.InputData.LastDate });
                    testData.Rows.Add(new object[] { "Resolution", testCase.InputData.Resolution });

                    DataTable actualTable = CurveTestHelper.KeyValuePairsToDataTable(exchangeRates);

                    DataTable expectedTable = CurveTestHelper.ExpectedCurveValuesToData(testCase.ExpectedCurveValues);


                    DataTable acutalAsXmlTable = new DataTable();
                    acutalAsXmlTable.Columns.Add("ActualXml");

                    foreach (KeyValuePair<DateTime,double> rate in exchangeRates)
                    {
                        string xmlDate = TestXmlTool.ConvertToXmlDateTimeString(rate.Key);

                        string xmlString = "<ExpectedCurveValue Date=\"" + xmlDate + "\" Value=\"" + rate.Value + "\"/>";

                        acutalAsXmlTable.Rows.Add(xmlString);
                    }

                    this.view.ShowCompare(testData, actualTable, expectedTable, acutalAsXmlTable, GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE);
                }

            }
            catch (Exception ex)
            {

                this.view.ShowUnknownError(ex.ToString());
            }
            finally
            {
                ElvizConfigurationTool utility=new ElvizConfigurationTool();
                utility.RevertAllConfigurationsToDefault();
            }

        }

        public void CompareCurveTest()
        {
            ElvizConfigurationTool tool = new ElvizConfigurationTool();
            try
            {

                IList<string> testFilePaths = this.view.XmlFilePaths;

                foreach (string testFilePath in testFilePaths)
                {
                    CurveTestCase test = TestXmlTool.Deserialize<CurveTestCase>(testFilePath);

                    QaPriceCurveDtoWrapper wrapper =CurveTestUtil.GetCurve(test);

                    DataTable testData = new DataTable();
                    testData.Columns.Add("Key", typeof (string));
                    testData.Columns.Add("Value", typeof (string));


                    if (test.InputData.ElvizConfigurations != null)
                    {
                        foreach (ElvizConfiguration elvizConfiguration in test.InputData.ElvizConfigurations)
                        {
                            testData.Rows.Add(new object[] {elvizConfiguration.Name, elvizConfiguration.Value});
                        }
                    }
                    testData.Rows.Add(new object[] {"TemplateName", test.InputData.TemplateName});
                    testData.Rows.Add(new object[] {"ReferenceAreaName", test.InputData.ReferenceAreaName});
                    testData.Rows.Add(new object[] {"PriceBookAppendix", test.InputData.PriceBookAppendix});
                    testData.Rows.Add(new object[] {"ReportDate", test.InputData.ReportDate});
                    testData.Rows.Add(new object[] {"ReportCurrencyIsoCode", test.InputData.ReportCurrencyIsoCode});
                    testData.Rows.Add(new object[] {"FromDate", test.InputData.FromDate});
                    testData.Rows.Add(new object[] {"ToDate", test.InputData.ToDate});
                    testData.Rows.Add(new object[] {"Resolution", test.InputData.Resolution});
                    testData.Rows.Add(new object[]
                    {"Test File", Path.Combine(this.view.CurrentFileDialogDirectory, testFilePath)});


                    DataTable actualTable = wrapper.GetCurveValuesAsDataTable();

                    DataTable expectedTable =
                        CurveTestHelper.ExpectedCurveValuesToData(test.ExpectedValues.ExpectedCurveValues);

                    DataTable acutalDataAsXml = new DataTable();
                    acutalDataAsXml.Columns.Add("ActualXml");

                    foreach (DateTimeValue dateTimeValue in wrapper.GetCurveValues())
                    {
                        string xmlDate = TestXmlTool.ConvertToXmlDateTimeString(dateTimeValue.DateTime);

                        string xmlString = "<ExpectedCurveValue Date=\"" + xmlDate + "\" Value=\"" + dateTimeValue.Value +
                                           "\"/>";

                        acutalDataAsXml.Rows.Add(xmlString);
                    }



                    this.view.ShowCompare(testData, actualTable, expectedTable, acutalDataAsXml, GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE);

                }

            }
            catch (Exception ex)
            {
                this.view.ShowApplicationError(ex.ToString());
            }
            finally
            {
                tool.RevertAllConfigurationsToDefault();
            }


        }

        public void CompareBatchTests()
        {
            string dbType = "VizDatawarehouse";
            try
            {

                IList<string> testFilePaths = this.view.XmlFilePaths;

                foreach (string testFile in testFilePaths)
                {

                    BatchTestFile testCase = BatchTestFileParser.DeserializeXml(testFile);
           

                    foreach (Assertion assertion in testCase.Assertions)
                    {
                        string sql = assertion.DbQuery.PreparedSqlQuery;

                       // string dbName = ElvizInstallationUtility.GetEtrmDbName(dbType);

                        DataTable dataWarehouseResultTable =QaDao.DataTableFromSql(dbType, sql);


                        if (dataWarehouseResultTable.Rows.Count == 0&&assertion.ExpectedRecords.Length>0)
                        {
                            this.view.ShowUserError("DWH SqlQuery returned nothing");
                            return;
                        }

                        DataTable expectedTable = dataWarehouseResultTable.Clone();

                        expectedTable.Locale=new CultureInfo("en-us");

                        for (int i = 0; i < assertion.ExpectedRecords.Length; i++)
                        {
                            string assertRow = assertion.ExpectedRecords[i];

                            object[] asserts = assertRow.Split(new string[] {","}, StringSplitOptions.None);

                            DataRow row = expectedTable.NewRow();

                            

                            for (int j = 0; j < asserts.Length; j++)
                            {
                                string s = asserts[j] as string;
                                if (s != null && s.ToUpper() == "NULL")
                                {
                                    row[j] = DBNull.Value;
                                    continue;
                                }

                                if (expectedTable.Columns[j].DataType==typeof(bool) )
                                {
                                    if ((asserts[j] is int ? (int) asserts[j] : 0) == 1)
                                    {
                                        row[j] = true;

                                    }
                                    else
                                    {
                                        row[j] = false;
                                    }
                                    continue;
                                }
                                row[j] = asserts[j];
                            }
                            expectedTable.Rows.Add(row);
                         
                        }

                        DataTable actualDataAsXml = ActualDataAsXml(dataWarehouseResultTable,"record",true);


                        DataTable testData = new DataTable();
                        testData.Columns.Add("Key", typeof(string));
                        testData.Columns.Add("Value", typeof(string));
                        testData.Rows.Add(new object[] { "Test Name", testCase.Name });
                        testData.Rows.Add(new object[] { "Workspace", testCase.Setup.Workspace });
                        testData.Rows.Add(new object[] { "Monitor", testCase.Setup.Monitor });
                        testData.Rows.Add(new object[] { "ReportDate", testCase.Setup.ReportDate.ToString("yyyy-MM-dd") });

                        double tollerance;

                        if (testCase.Setup.Tollerance == null)
                            tollerance = GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE;
                        else tollerance = testCase.Setup.Tollerance.Value;

                        testData.Rows.Add(new object[] { "Tollerance", tollerance });
                        testData.Rows.Add(new object[] { "Query File", assertion.Query.FileName});
                        testData.Rows.Add(new object[] { "Test File", Path.Combine(this.view.CurrentFileDialogDirectory, testFile) });

                        ElvizConfiguration[] elvizConfigurations = testCase.Setup.ElvizConfigurations;

                        if (elvizConfigurations != null)
                        {
                            foreach (ElvizConfiguration configuration in elvizConfigurations)
                            {
                                string excelName = "ElvizConfiguartion-"+configuration.Name;
                                testData.Rows.Add(new object[] { excelName, configuration.Value });
                            }
                        }

                        this.view.ShowCompare(testData, dataWarehouseResultTable, expectedTable, actualDataAsXml,tollerance);
                    }
                }
            }
            catch (Exception ex)
            {
                string errorString = ex.Message + "\n\n" + ex.StackTrace;

                this.view.ShowApplicationError(errorString);
            }



        }

        public void CompareCustomDwh()
        {

            try
            {

                IList<string> testFilePaths = this.view.XmlFilePaths;

                foreach (string testFilePath in testFilePaths)
                {

                    CustomDwhTest testCase = TestXmlTool.Deserialize<CustomDwhTest>(testFilePath);
                    string name = Path.GetFileNameWithoutExtension(testFilePath);

                    foreach (Monitor monitor in testCase.Assert)
                    {


                        DbQuery query = TestXmlTool.Deserialize<DbQuery>(Path.Combine(testFilePath, monitor.QueryFilePath));

                        DataTable expecteDataTable = monitor.GetExpectedDataTable(testCase.InputData, query);

                        DataTable actualTable = monitor.GetActualDataTable(testCase.InputData, query);


                        if (expecteDataTable.Rows.Count == 0)
                        {
                            this.view.ShowUserError("Expected DWH "+testCase.InputData.ExpectedDatawareHouse +" SqlQuery returned nothing");
                            return;
                        }

                        if (actualTable.Rows.Count == 0)
                        {
                            this.view.ShowUserError("Actual DWH " + testCase.InputData.ActualDatawareHouse + " SqlQuery returned nothing");
                            return;
                        }

                         
                        DataTable testData = new DataTable();
                        testData.Columns.Add("Key", typeof(string));
                        testData.Columns.Add("Value", typeof(string));
                        testData.Rows.Add("Test Name", name);
                        testData.Rows.Add("Sql Instance", testCase.InputData.SqlInstance);
                        testData.Rows.Add("Expected DWH", testCase.InputData.ExpectedDatawareHouse);
                        testData.Rows.Add("Actual DWH", testCase.InputData.ActualDatawareHouse);
                        testData.Rows.Add("Workspace", testCase.InputData.Workspace);
                        testData.Rows.Add("Monitor", monitor.Name);
                        testData.Rows.Add("ReportDate", testCase.InputData.ReportDate.ToString("yyyy-MM-dd"));
                        testData.Rows.Add("Tollerance", GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE);
                        testData.Rows.Add("Query File", Path.GetFileName(monitor.QueryFilePath));


                        DataTable actualDataAsXml = ActualDataAsXml(actualTable, "ExpectedRecord", true);



                        this.view.ShowCompare(testData, expecteDataTable, actualTable,actualDataAsXml, GlobalConstTestSettings.DEFAULT_FLOATING_POINT_TOLLERANCE);
                    }
                }
            }
            catch (Exception ex)
            {
                string errorString = ex.Message + "\n\n" + ex.StackTrace;

                this.view.ShowApplicationError(errorString);
            }



        }




        public void UpdateTimeSeriesTestFile()
        {
            try
            {
                string filePath = this.view.CurrentTestFilePath;

                if (filePath == null)
                {
                    this.view.ShowUserError("Missing Filepath");
                    return;
                }

                string[] newExpectedRecords = this.view.CurrentExpectedRecords;

                ReportingDBTestCase testCase = TestXmlTool.Deserialize<ReportingDBTestCase>(filePath);

                testCase.ExpectedValues.ExpectedDataTable.ExpectedRecord = newExpectedRecords;

                FileInfo info = new FileInfo(filePath);

                if (info.IsReadOnly)
                {
                    this.view.ShowUserError("The File : " + filePath + " is Read Only");
                    return;
                }

                TestXmlTool.SerializeToXml(testCase, filePath);
                this.view.ShowMessage("Updated Batch Test File: \n" + filePath);

            }
            catch (Exception ex)
            {

                this.view.ShowApplicationError(ex.ToString());
            }
        }



        public void UpdateBatchTestFile()
        {
            try
            {
                string filePath = this.view.CurrentTestFilePath;

                if (filePath == null)
                {
                    this.view.ShowUserError("Missing Filepath");
                    return;
                }

                string[] newExpectedRecords= this.view.CurrentExpectedRecords;

                BatchTestFile testCase = TestXmlTool.Deserialize<BatchTestFile>(filePath);

                string queryFile = this.view.CurrentQueryFile;

                if (testCase.Assertions == null)
                {
                    this.view.ShowUserError("The testfile does not have any assertions");
                }



                Assertion ass = testCase.Assertions.FirstOrDefault(x => x.Query.FileName == queryFile);

                if (ass == null)
                {
                    this.view.ShowUserError("The testfile does not contain the Assertion with Query file : "+queryFile);
                    return;
                }
                ass.ExpectedRecords = newExpectedRecords;

                FileInfo info=new FileInfo(filePath);

                if (info.IsReadOnly)
                {
                    this.view.ShowUserError("The File : "+filePath+" is Read Only");
                    return;
                }

                TestXmlTool.SerializeToXml(testCase,filePath);
                this.view.ShowMessage("Updated Batch Test File: \n" + filePath);

            }
            catch (Exception ex)
            {
                
                this.view.ShowApplicationError(ex.ToString());
            }
        }

        public void UpdateReportEngineTestFile(bool updateColumns)
        {
            try
            {
                string filePath = this.view.CurrentTestFilePath;
                string artifactId = this.view.CurrentArtifactId;

                if (filePath == null)
                {
                    this.view.ShowUserError("Missing Filepath");
                    return;
                }

                if (artifactId == null)
                {
                    this.view.ShowUserError("This sheet is missing Artifact Id");
                }

	            string[] newExpectedRecords = this.view.CurrentExpectedRecords;

                TestCaseReportEngine testCase = TestXmlTool.Deserialize<TestCaseReportEngine>(filePath);

                if (testCase.ExpectedValues.ExpectedDataTableArtifacts == null &&
                    testCase.ExpectedValues.ExpectedTimeSeriesArtifacts == null)
                {
                    this.view.ShowUserError("Testfile has neither ExpectedDataTableArtifact nor ExpectedTimeSeriesArtifact");
                    return;
                }

                bool testFileHasArtifact = false;

                if (testCase.ExpectedValues.ExpectedDataTableArtifacts != null)
                {
                    ExpectedDataTableArtifact dataTableArtifact = testCase.ExpectedValues.ExpectedDataTableArtifacts.FirstOrDefault(x => x.ArtifactId == artifactId);
					if (dataTableArtifact != null)
                    {
	                    if (updateColumns)
	                    {
		                    IDictionary<string, Type> columnInfo = this.view.GetCurrentExpectedColumnNamesAndTypes();
		                    dataTableArtifact.Columns = string.Join(",", columnInfo.Keys);
		                    ICollection<Type> types = columnInfo.Values;
		                    dataTableArtifact.ColumnsDataTypes = string.Join(",", types.Select(x => x.ToString()));
	                    }

	                    dataTableArtifact.ExpectedRecord = newExpectedRecords;
                        testFileHasArtifact = true;
                    }
                }

                if (testCase.ExpectedValues.ExpectedTimeSeriesArtifacts != null)
                {
                    ExpectedTimeSeriesArtifact timeSeriesArtifact = testCase.ExpectedValues.ExpectedTimeSeriesArtifacts.FirstOrDefault(x => x.ArtifactId == artifactId);

                    if (timeSeriesArtifact != null)
                    {
                        timeSeriesArtifact.ExpectedRecord = newExpectedRecords;
                        testFileHasArtifact = true;
                    }
                }

                if (!testFileHasArtifact)
                {
                    this.view.ShowUserError("Testfile did not contain Artifact : "+artifactId);
                }

                FileInfo info = new FileInfo(filePath);

                if (info.IsReadOnly)
                {
                    this.view.ShowUserError("The File : " + filePath + " is Read Only");
                    return;
                }

                TestXmlTool.SerializeToXml(testCase, filePath);

                this.view.ShowMessage("Updated Report Engine Artifact : "+artifactId+"\n"+filePath);

            }
            catch (Exception ex)
            {

                this.view.ShowApplicationError(ex.ToString());
            }
        }

        public void UpdateCurveTestFile()
        {
            try
            {
                string filePath = this.view.CurrentTestFilePath;
               

                if (filePath == null)
                {
                    this.view.ShowUserError("Missing Filepath");
                    return;
                }

             

                string[] newExpectedRecords = this.view.CurrentExpectedRecords;

                TestCaseReportEngine testCase = TestXmlTool.Deserialize<TestCaseReportEngine>(filePath);


                FileInfo info = new FileInfo(filePath);

                if (info.IsReadOnly)
                {
                    this.view.ShowUserError("The File : " + filePath + " is Read Only");
                    return;
                }

                TestXmlTool.SerializeToXml(testCase, filePath);

            //    this.view.ShowMessage("Updated Report Engine Artifact : " + artifactId + "\n In :" + filePath);

            }
            catch (Exception ex)
            {

                this.view.ShowApplicationError(ex.ToString());
            }
        }


        private static DataTable ActualDataAsXml(DataTable actualDataTable,string xmlTagName,bool batchXmlDateFormat)
        {
            DataTable actualDataAsXml = new DataTable("Actual Xml");
            actualDataAsXml.Columns.Add("ActualXml");

            int columnLengh = actualDataTable.Columns.Count;
            int rowLength = actualDataTable.Rows.Count;

            for (int i = 0; i < rowLength; i++)
            {
                string rowString = "<"+xmlTagName+">";
                for (int j = 0; j < columnLengh; j++)
                {
                    object dataTableObject = actualDataTable.Rows[i][j];

                    if (dataTableObject == DBNull.Value)
                        dataTableObject = "NULL";

                    if (actualDataTable.Columns[j].DataType == typeof(DateTime))
                    {
                        if (batchXmlDateFormat)
                        {
                            dataTableObject = TestXmlTool.ConvertToBatchXmlDateTimeString(dataTableObject as DateTime?);
                        }
                        else
                        {
                            dataTableObject = TestXmlTool.ConvertToXmlDateTimeString(dataTableObject as DateTime?);
                        }
                    }
                    if (actualDataTable.Columns[j].DataType == typeof (bool))
                    {
                        if (batchXmlDateFormat)
                        {
                            bool boolObject =(bool) dataTableObject;

                            dataTableObject = boolObject ? 1 : 0;
                        }
                    }

	                if ((!dataTableObject.Equals("NULL")) && (actualDataTable.Columns[j].DataType == typeof (double)))
	                {
		                double d = (double) dataTableObject;
						IFormatProvider numberFormat = new NumberFormatInfo
						{
							NumberDecimalSeparator = ".",
							NumberGroupSeparator = ""
						};
		                dataTableObject = d.ToString(numberFormat);
	                }

                    rowString += dataTableObject;
                    if (j != columnLengh - 1) rowString += ",";
                }

                rowString += "</"+xmlTagName+">";
                actualDataAsXml.Rows.Add(rowString);
            }
            return actualDataAsXml;
        }


    }
}
