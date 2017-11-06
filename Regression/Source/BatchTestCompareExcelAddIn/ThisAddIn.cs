using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using ElvizTestUtils.BatchTests;
using ElvizTestUtils.Excel;
using ElvizTestUtils.Excel.BatchTestCompareExcelAddIn;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;


namespace BatchTestCompareExcelAddIn
{
    public partial class ThisAddIn:ICompareView
    {
        private CompareViewController controller;
        private string currentFileDialogDirectory;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            this.controller=new CompareViewController(this);
            
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion

        public string CurrentFileDialogDirectory
        {
            get { return currentFileDialogDirectory; }
        }

        public string CurrentTestFilePath
        {
            get
            {
                const string key = "Test File";

                return GetTestDataValue(key);
            }
        }

        private string GetTestDataValue(string key)
        {
            Excel.Workbook activeWorkBook = (Excel.Workbook) Application.ActiveWorkbook;
            
            BaseExcel baseExcel = new BaseExcel(activeWorkBook,activeWorkBook.ActiveSheet);

            object[,] testDataRange = baseExcel.GetRegion(1, 1);


            for (int i = 1; i <= testDataRange.GetUpperBound(0); i++)
            {
                if (testDataRange[i, 1].ToString() == key)
                    return testDataRange[i, 2].ToString();
            }

            return null;
        }

        public string CurrentArtifactId
        {
            get
            {
                const string key = "ArtiFact Id";

                return GetTestDataValue(key); 

              
            }
        }

        public string CurrentQueryFile
        {
            get
            {
                const string key = "Query File";
                return GetTestDataValue(key); 
            }
        }

	    public IDictionary<string, Type> GetCurrentExpectedColumnNamesAndTypes()
	    {
			Excel.Workbook activeWorkBook = Application.ActiveWorkbook;
			BaseExcel baseExcel = new BaseExcel(activeWorkBook,activeWorkBook.ActiveSheet);

			object[,] testDataRange = baseExcel.GetRegion(1, 1);

			int testDataRows = testDataRange.GetUpperBound(0) + 3;

			object[,] range = baseExcel.GetRegion(testDataRows, 1);

			int numCols = range.GetUpperBound(1);

		    Dictionary<string, Type> columnInfo = new Dictionary<string, Type>();

		    for (int i=1 ; i<=numCols ; i++)
		    {
			    string header = range[1, i].ToString(); // header row
			    object data = range[2, i];
			    Type dataType = data?.GetType() ?? typeof(string);
				if (columnInfo.ContainsKey(header))
					break; 

			    columnInfo.Add(header, dataType);
		    }
			
			return columnInfo;
		}

        public string[] CurrentExpectedRecords
        {
            get
            {
                Excel.Workbook activeWorkBook = (Excel.Workbook)Application.ActiveWorkbook;

                BaseExcel baseExcel = new BaseExcel(activeWorkBook,activeWorkBook.ActiveSheet);

                object[,] testDataRange = baseExcel.GetRegion(1, 1);

                int testDataRows = testDataRange.GetUpperBound(0) + 3;

                object[,] range = baseExcel.GetRegion(testDataRows, 1);

                int colsBound = range.GetUpperBound(1);
                int rowBound = range.GetUpperBound(0);

                IList<string> currentExpectedRecords=new List<string>();

                for (int i = 2; i <= rowBound; i++)
                {
                    string s = range[i, colsBound].ToString();

                    string s1= Regex.Replace(s, "<record>", "");
                    string s2=Regex.Replace(s1, "</record>", "");
                    string s3 = Regex.Replace(s2, "<ExpectedRecord>", "");
                    string s4 = Regex.Replace(s3, "</ExpectedRecord>", "");
                    currentExpectedRecords.Add(s4);
                    
                    

                }



                return currentExpectedRecords.ToArray();
            }
        }

        public IList<BatchTestFile> BatchTestFilesStructure
        {
            get
            {
                Excel.Workbook activeWorkBook = (Excel.Workbook)Application.ActiveWorkbook;

                BaseExcel baseExcel = new BaseExcel(activeWorkBook,activeWorkBook.ActiveSheet);

                object[,] batchTestsRange = baseExcel.GetRegion(3, 1);

                IList<BatchTestFile> testFiles=new List<BatchTestFile>();

                int numberOfRows = batchTestsRange.GetUpperBound(0);
                int numberOfCols = batchTestsRange.GetUpperBound(1);

                for (int i = 2; i <= numberOfRows; i++)
                {

                    BatchTestFile batchTestFile=new BatchTestFile();


                    string fileName = (string)batchTestsRange[i, 1];
                    string user = (string)batchTestsRange[i, 2];
                    string password = (string)batchTestsRange[i, 3];
                    string workspace = (string)batchTestsRange[i, 4];
                    string monitor = (string)batchTestsRange[i, 5];
                    DateTime reportDate = (DateTime)batchTestsRange[i, 6];
                    string query = (string)batchTestsRange[i, 7];

                    batchTestFile.Name = fileName;

                    batchTestFile.Setup=new Setup(){User = user,Password = password,Workspace = workspace,Monitor = monitor,ReportDate = reportDate};

                    batchTestFile.Assertions=new Assertion[]
                    {
                        new Assertion(){Query = new Query(){FileName = query},ExpectedRecords = new string[0]}
                        
                    };

                    testFiles.Add(batchTestFile);
                }

                return testFiles;
            }
        }

        public string ExportBatchTestFilesPath
        {
            get
            {
                string path;
                Excel.Workbook activeWorkBook = (Excel.Workbook)Application.ActiveWorkbook;

                BaseExcel baseExcel = new BaseExcel(activeWorkBook,activeWorkBook.ActiveSheet);

                path = baseExcel.ReadFromCell(1,2);

                return path;
                
                ; }
        }


        public void RequestCreateBatchFiles()
        {
            this.controller.CreateBatchTestFiles();
            
        }

        public void RequestCompareRdTimeSeriesEcm()
        {
            this.controller.CompareRdTimeSeriesFromEcmTests();

        }

        public void RequestCompareRTimeSeries()
        {
            this.controller.CompareRdTimeSeriesTests();

        }

        public void RequestUpdateRdTimeSeries()
        {
            this.controller.UpdateTimeSeriesTestFile();

        }

        public void RequestCompare()
        {
            this.controller.CompareBatchTests();
        }

        public void RequestCompareExchangeRates()
        {
            this.controller.CompareExchangeRateTests();
        }


        public void RequestUpdateBatchFile()
        {
            this.controller.UpdateBatchTestFile();
        }

		public void RequestUpdateReportEngineFile(bool updateColumns)
        {
            this.controller.UpdateReportEngineTestFile(updateColumns);
        }

        public void RequestReportEngineCompare()
        {
            this.controller.CompareReportEngineTests();
        }

        public void RequestCurveCompare()
        {
            this.controller.CompareCurveTest();
        }

        public void RequestCompareCustomDwh()
        {
            this.controller.CompareCustomDwh();
        }

        public IList<string> XmlFilePaths
        {
            get
            {
              
                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.Filter = "Test Files (.xml)|*.xml";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = true;
               

                IList<string> collection = new List<string>();

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string fileName in openFileDialog.FileNames)
                    {
                        collection.Add(fileName);
                    }
                 
                    string currentFileDialogDirectoryPath = Path.GetDirectoryName(openFileDialog.FileName);
                    if (currentFileDialogDirectoryPath != null)
                        this.currentFileDialogDirectory =
                            currentFileDialogDirectoryPath.Substring(
                                currentFileDialogDirectoryPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                }

                return collection;
            }

        }

    
        public void ShowCompare(DataTable testData,DataTable actualTable,DataTable expectedTable,DataTable newXmlTable,double tollerance)
        {
            Excel.Workbook activeWorkBook =(Excel.Workbook) Application.ActiveWorkbook;
            
            CompareExcel excel = new CompareExcel(activeWorkBook,activeWorkBook.ActiveSheet);
            excel.Compare(testData,actualTable,expectedTable,newXmlTable,tollerance);


            //BaseExcel baseExcel=new BaseExcel(activeWorkBook);

            //int numActualColumns = actualTable.Columns.Count;
            //int numExpectedColumns = expectedTable.Columns.Count;
            //int numberOfRows = actualTable.Rows.Count;

            //string sheetName=baseExcel.AddWorkSheet();

            //int startRow = testData.Rows.Count + 3;

            //baseExcel.ExportDataTable(testData,1,1);

            //baseExcel.ExportDataTableWithColumns(actualTable,startRow,1,System.Drawing.Color.Yellow);

            //baseExcel.ExportDataTableWithColumns(expectedTable,startRow,1+numActualColumns,System.Drawing.Color.LightGreen);

            //DataTable emptyTable = expectedTable.Clone();

            //baseExcel.ExportDataTableWithColumns(emptyTable,startRow,1+numActualColumns+numExpectedColumns, System.Drawing.Color.Red);

            //string actualStartCellAdress = baseExcel.GetAdress(startRow+1, 1);
            //string expectedStartCellAdress = baseExcel.GetAdress(startRow+1, 1 + numActualColumns);

            //int t = int.Parse(tollerance.ToString("E").Split('-').Last());

            //string formula = @"=IF(" + actualStartCellAdress + @"<>" + expectedStartCellAdress + @",IF(AND(ABS(" + actualStartCellAdress + @")<0.0000001,ABS(" + expectedStartCellAdress + @")<0.0000001),"""",ROUND(" + actualStartCellAdress + @"/" + expectedStartCellAdress + @"-1," + t + @" )),"""")";


            //baseExcel.AssignFormulaToRange(startRow+1,1+numActualColumns+numExpectedColumns,numberOfRows,numExpectedColumns,formula);

            //baseExcel.ExportDataTableWithColumns(newXmlTable, startRow, 1 + numActualColumns+numExpectedColumns*2, System.Drawing.Color.LightBlue);

            //baseExcel.ChangeWidth(1+ numActualColumns + numExpectedColumns * 2, 30);

            //baseExcel.ApplyFilter(startRow);


        }
		
        public void ShowUserError(string errorString)
        {
            MessageBox.Show(errorString,"Incorrect operation",MessageBoxButtons.OK,MessageBoxIcon.Hand);
        }

        public void ShowApplicationError(string errorString)
        {
            MessageBox.Show(errorString,"Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
        }

        public void ShowUnknownError(string errorString)
        {
            MessageBox.Show(errorString, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowMessage(string messageString)
        {
            MessageBox.Show(messageString, "Message",  MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        public bool ShowOkCancel(string messageString, string headerText)
        {
            throw new NotImplementedException();
        }
    }
}
