using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElvizTestUtils.Excel.BatchTestCompareExcelAddIn;
using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;

namespace ElvizTestUtils.Excel
{
    public class CompareExcel:BaseExcel
    {
        public CompareExcel(Workbook activeWorkBook,Worksheet worksheet) : base(activeWorkBook,worksheet)
        {

        }

        public void Compare(DataTable testData, DataTable actualTable, DataTable expectedTable,double tollerance)
        {
            int numActualColumns = actualTable.Columns.Count;
            int numExpectedColumns = expectedTable.Columns.Count;
            int numberOfRows = actualTable.Rows.Count;

            string sheetName = this.AddWorkSheet();

            int startRow = testData.Rows.Count + 3;
            
            this.SelectCell(1,1);

            this.AssignToCell(1,1,"A");

            this.ExportDataTable(testData, 1, 1);

            this.ExportDataTableWithColumns(actualTable, startRow, 1, System.Drawing.Color.Yellow);

            this.ExportDataTableWithColumns(expectedTable, startRow, 1 + numActualColumns, System.Drawing.Color.LightGreen);

            DataTable emptyTable = expectedTable.Clone();

            this.ExportDataTableWithColumns(emptyTable, startRow, 1 + numActualColumns + numExpectedColumns, System.Drawing.Color.Red);

            string actualStartCellAdress = this.GetAdress(startRow + 1, 1);
            string expectedStartCellAdress = this.GetAdress(startRow + 1, 1 + numActualColumns);

            int t = int.Parse(tollerance.ToString("E").Split('-').Last());

            string formula = @"=IF(" + actualStartCellAdress + @"<>" + expectedStartCellAdress + @",IF(AND(ABS(" + actualStartCellAdress + @")<0.0000001,ABS(" + expectedStartCellAdress + @")<0.0000001),"""",ROUND(" + actualStartCellAdress + @"/" + expectedStartCellAdress + @"-1," + t + @" )),"""")";


            this.AssignFormulaToRange(startRow + 1, 1 + numActualColumns + numExpectedColumns, numberOfRows, numExpectedColumns, formula);



            this.ChangeWidth(1 + numActualColumns + numExpectedColumns * 2, 30);

            this.ApplyFilter(startRow);
        }

        public void Compare(DataTable testData, DataTable actualTable, DataTable expectedTable, DataTable newXmlTable, double tollerance)
        {


            int numActualColumns = actualTable.Columns.Count;
            int numExpectedColumns = expectedTable.Columns.Count;
            int startRow = testData.Rows.Count + 3;

            this.Compare(testData,actualTable,expectedTable,tollerance);

            this.ExportDataTableWithColumns(newXmlTable, startRow, 1 + numActualColumns + numExpectedColumns * 2, System.Drawing.Color.LightBlue);

        }
    }
}
