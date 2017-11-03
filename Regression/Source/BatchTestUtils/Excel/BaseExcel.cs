using System;
using System.Reflection;

namespace ElvizTestUtils.Excel
{
    using Excel = Microsoft.Office.Interop.Excel;
    using DataTable = System.Data.DataTable;

    namespace BatchTestCompareExcelAddIn
    {
        public class BaseExcel
        {
            //User defined Regional Settins -> must be changed to English in order to work

            //private readonly System.Globalization.CultureInfo oldCultSetting;
            //private readonly System.Globalization.CultureInfo cultSettings;


            // Excel Properties
           
            readonly Excel.Workbook activeWorkBook;

         //   readonly Workbooks objBooks;
            readonly Excel.Sheets objSheets;
            Excel.Worksheet activeWorkSheet;
         //   Range range;


            public BaseExcel(Excel.Workbook activeWorkBook,Excel.Worksheet worksheet)
            {
                       //oldCultSetting = System.Threading.Thread.CurrentThread.CurrentCulture;
                       // cultSettings = new System.Globalization.CultureInfo("en-US");
                       // System.Threading.Thread.CurrentThread.CurrentCulture = cultSettings;

                this.activeWorkBook = activeWorkBook;
                this.activeWorkSheet = worksheet;
                objSheets = activeWorkBook.Worksheets;

            }

            public void AddWorkSheet(string name)
            {
                string croppedName = name;
                
                if(name.Length>30)
                   croppedName= name.Substring(0, 30);
                Excel.Worksheet newWorksheet;
                newWorksheet = (Excel.Worksheet)activeWorkBook.Worksheets.Add();
                newWorksheet.Name = croppedName;
                this.activeWorkSheet = newWorksheet;

            }

            public void SelectCell(int row, int col)
            {
                Excel.Range range;
                range = activeWorkSheet.Range[activeWorkSheet.Cells[row, col], activeWorkSheet.Cells[row, col]];
                range.Select();
            }

            public string AddWorkSheet()
            {
                
                Excel.Worksheet newWorksheet;
                newWorksheet = (Excel.Worksheet)activeWorkBook.Worksheets.Add();
                
                this.activeWorkSheet = newWorksheet;

                return newWorksheet.Name;

            }

            //public BaseExcel(int worksheetIndex, string worksheetName)
            //{
            //    // Changes the culture to English-US
            //    if (objApp == null)
            //    {
            //        oldCultSetting = System.Threading.Thread.CurrentThread.CurrentCulture;
            //        cultSettings = new System.Globalization.CultureInfo("en-US");
            //        System.Threading.Thread.CurrentThread.CurrentCulture = cultSettings;
            //        objApp = new Application();
            //    }

            //    //  Excel.Worksheet activeWorksheet = ((Excel.Worksheet)Application.ActiveSheet);
            //    ////  Excel.Worksheet activeWorksheet = ((Excel.Worksheet)Application.);

            //    objBooks = objApp.Workbooks;
            //    activeWorkBook = objBooks.Add(Missing.Value);
            //    objSheets = activeWorkBook.Worksheets;
            //    activeWorkSheet = (Worksheet)objSheets.get_Item(worksheetIndex);
            //    activeWorkSheet.Name = worksheetName;
            //}

            public void ChangeWorksheetIndexName( int worksheetIndex, string name)
            {
                activeWorkSheet = (Excel.Worksheet)objSheets.get_Item(worksheetIndex);
                activeWorkSheet.Name = name;
            }


            public void ChangeWidth(int colNumber, int width)
            {
                Excel.Range range;
                range = activeWorkSheet.Range[activeWorkSheet.Cells[1, colNumber], activeWorkSheet.Cells[1, colNumber]];
                range.ColumnWidth = width;
            }

            public void ChangeWidth(int colNumber1, int colNumber2, int width)
            {
                Excel.Range range;
                range = activeWorkSheet.Range[activeWorkSheet.Cells[1, colNumber1], activeWorkSheet.Cells[1, colNumber2]];
                range.ColumnWidth = width;
            }

            public void ChangeFormat(int[] destCell1, int[] destCell2, String format)
            {
                Excel.Range range;
                range = activeWorkSheet.Range[activeWorkSheet.Cells[destCell1[0], destCell1[1]], activeWorkSheet.Cells[destCell2[0], destCell2[1]]];
                range.NumberFormat = format;
            }

            public void ChangeFormat(int rowA, int colA, int rowB, int colB, String format)
            {
                Excel.Range range;
                range = activeWorkSheet.Range[activeWorkSheet.Cells[rowA, colA], activeWorkSheet.Cells[rowB, colB]];
                range.NumberFormat = format;
            }


            public void ExportArray<T>(int cell1Row, int cell1Col, int cell2Row, int cell2Col, T[,] exportArray)
            {
                Excel.Range range;
                range = activeWorkSheet.Range[activeWorkSheet.Cells[cell1Row, cell1Col], activeWorkSheet.Cells[cell2Row, cell2Col]];

                //Set the range value to the array.
                range.set_Value(Missing.Value, exportArray);


            }

            public void ExportArray<T>(int cell1Row, int cell1Col, int cell2Row, int cell2Col, T[] exportArray)
            {
                Excel.Range range;
                range = activeWorkSheet.Range[activeWorkSheet.Cells[cell1Row, cell1Col], activeWorkSheet.Cells[cell2Row, cell2Col]];

                //Set the range value to the array.
                range.set_Value(Missing.Value, exportArray);


            }

            public void ChangeRangeToNumberFormat(int cell1Row, int cell1Col, int cell2Row, int cell2Col)
            {
                Excel.Range range;
                range = activeWorkSheet.Range[activeWorkSheet.Cells[cell1Row, cell1Col], activeWorkSheet.Cells[cell2Row, cell2Col]];
                range.Formula = range.Value2;


            }


            public void AssignToCell(int row, int col, string value)
            {
                
                    Excel.Range range;
                    range = activeWorkSheet.Range[activeWorkSheet.Cells[row, col], activeWorkSheet.Cells[row, col]];

                    //Set the range value to the array.
                    range.set_Value(Missing.Value, value);
               

               
            }

            public string ReadFromCell(int row, int col)
            {
                Excel.Range range = activeWorkSheet.Range[activeWorkSheet.Cells[row, col], activeWorkSheet.Cells[row, col]];

                object e= (activeWorkSheet.Cells[row, col] as Excel.Range).Value;

                if (e != null)
                    return e.ToString();

                return null;

            }

            public void AssignFormulaToCell(int row, int col, string formula)
            {
                Excel.Range range;
                range = activeWorkSheet.Range[activeWorkSheet.Cells[row, col], activeWorkSheet.Cells[row, col]];

                //Set the range value to the array.
                range.Formula = formula;
            }

            public string GetAdress(int rowNumber, int colNumber)
            {
                Excel.Range startCell = activeWorkSheet.Range[activeWorkSheet.Cells[rowNumber, colNumber], activeWorkSheet.Cells[rowNumber, colNumber]];

                string absoluteAdress = startCell.AddressLocal;

                string relativeAdress = absoluteAdress.Replace("$", "");

                return relativeAdress;
            }

            public void ApplyFilter(int rowNumber)
            {
                Excel.Range row = (Excel.Range)activeWorkSheet.Rows[rowNumber];
                row.Activate();
                row.Select();
                row.AutoFilter(1,
                                    Type.Missing,
                                    Excel.XlAutoFilterOperator.xlAnd,
                                    Type.Missing,
                                    true);


                Excel.Range startCell = activeWorkSheet.Range[activeWorkSheet.Cells[1, 1], activeWorkSheet.Cells[1, 1]];
                startCell.Select();

            }

            public void AssignFormulaToRange(int startRow, int startCol,int rowLength,int columnLenght, string formula)
            {
                Excel.Range startCell = activeWorkSheet.Range[activeWorkSheet.Cells[startRow, startCol], activeWorkSheet.Cells[startRow, startCol]];

                string re = startCell.AddressLocal;

                startCell.Formula = formula;

                startCell.Copy();



                Excel.Range ranger = activeWorkSheet.Range[activeWorkSheet.Cells[startRow, startCol],
                              activeWorkSheet.Cells[startRow + rowLength - 1, startCol + columnLenght - 1]];


              


                //Set the range value to the array.
                ranger.PasteSpecial(Excel.XlPasteType.xlPasteAll);
            }



            public void ExportDataTable(DataTable dataTable, int startRow, int startCol)
            {

                int columnLengh = dataTable.Columns.Count;
                int rowLength = dataTable.Rows.Count;

                object[,] outPutObject = new object[rowLength, columnLengh];

                for (int i = 0; i < rowLength; i++)
                {
                    for (int j = 0; j < columnLengh; j++)
                    {
                        outPutObject[i, j] = dataTable.Rows[i][j];
                    }

                }
                Excel.Range ranger = activeWorkSheet.Range[activeWorkSheet.Cells[startRow, startCol],
                               activeWorkSheet.Cells[startRow + rowLength - 1, startCol + columnLengh - 1]];

               
                ranger.set_Value(Missing.Value, outPutObject);
            }

            public void ExportDataTableWithColumns(DataTable dataTable, int startRow, int startCol,System.Drawing.Color color)
            {
                if(dataTable == null) return;

                int columnLengh = dataTable.Columns.Count;
                int rowLength = dataTable.Rows.Count;

                object[,] columnOutPut=new object[1,columnLengh];

                for (int j = 0; j < columnLengh; j++)
                {
                    columnOutPut[0, j] = dataTable.Columns[j].ColumnName;
                }

                Excel.Range columnRange =
                    activeWorkSheet.Range[
                        activeWorkSheet.Cells[startRow, startCol], activeWorkSheet.Cells[startRow, startCol + columnLengh - 1]];

                columnRange.set_Value(Missing.Value,columnOutPut);

                columnRange.Columns.AutoFit();

                object[,] outPutObject = new object[rowLength, columnLengh];

                for (int i = 0; i < rowLength; i++)
                {
                    for (int j = 0; j < columnLengh; j++)
                    {
                        outPutObject[i, j] = dataTable.Rows[i][j];
                    }

                }
                Excel.Range ranger = activeWorkSheet.Range[activeWorkSheet.Cells[startRow+1, startCol],
                               activeWorkSheet.Cells[startRow + rowLength, startCol + columnLengh - 1]];

               
                ranger.set_Value(Missing.Value, outPutObject);

                Excel.Range totalRange = activeWorkSheet.Range[activeWorkSheet.Cells[startRow, startCol],
                               activeWorkSheet.Cells[startRow + rowLength, startCol + columnLengh - 1]];

                totalRange.Columns.AutoFit();

                totalRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(color);
            }

            //public void ExportGridView(DataGridView reportGridView, ref int excelRow)
            //{
            //    int numColumns = reportGridView.Columns.Count;

            //    //sets the headers into an range which will be exported to the spreadsheet
            //    Excel.Range range;
            //    range = activeWorkSheet.get_Range(activeWorkSheet.Cells[excelRow, 1], activeWorkSheet.Cells[excelRow, numColumns]);

            //    string[] headers = new string[numColumns];

            //    for (int i = 0; i < numColumns; i++)
            //    {
            //        headers[i] = reportGridView.Columns[i].HeaderText;
            //    }

            //    range.set_Value(Missing.Value, headers);
            //    range.Font.Bold = true;

            //    excelRow++;

            //    //sets the datagridviewrows into the spreadsheet

            //    int j = 1;
            //    foreach (DataGridViewRow dr in reportGridView.Rows)
            //    {
            //        object[] row = new object[numColumns];
            //        for (int i = 0; i < numColumns; i++)
            //        {
            //            row[i] = dr.Cells[i].Value;
            //        }

            //        range = activeWorkSheet.get_Range(activeWorkSheet.Cells[excelRow, 1], activeWorkSheet.Cells[excelRow, numColumns]);
            //        range.set_Value(Missing.Value, row);

            //        if (j == reportGridView.Rows.Count)
            //        {
            //            range.Font.Bold = true;
            //        }
            //        j++;
            //        excelRow++;
            //    }
            //}

            public object[,] GetRegion(int startRow, int startCol)
            {
                Excel.Range ranger = activeWorkSheet.Range[activeWorkSheet.Cells[startRow, startCol], activeWorkSheet.Cells[startRow, startCol].End[Excel.XlDirection.xlToRight].End[Excel.XlDirection.xlDown]];
            
                object[,] j = ranger.Value;
                return j;
            }


            //public void GiveControl()
            //{
            //    objApp.Visible = true;
            //    objApp.UserControl = true;
            //    activeWorkSheet = (Worksheet)objSheets.get_Item(1);

            //}

            //public void Dipose()
            //{
            //    SetOriginalCultureSettings();
            //}

            //public void SetOriginalCultureSettings()
            //{
            //    try
            //    {
            //        System.Threading.Thread.CurrentThread.CurrentCulture = oldCultSetting;
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception(ex.ToString());
            //    }
            //}
        }


    }

}
