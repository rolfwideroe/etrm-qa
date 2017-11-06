using System.Data;
using System.IO;
using ClosedXML.Excel;

namespace ElvizTestUtils.Excel
{
    public class ClosedXmlTool
    {
         readonly XLWorkbook workbook;

        public XLWorkbook Workbook => workbook;

        private readonly IXLWorksheet activeSheet;
        public ClosedXmlTool(string firstSheetName)
        {
            this.workbook=new XLWorkbook();
            this.activeSheet= workbook.AddWorksheet(firstSheetName);
        }

        public void SaveAndOverwriteIfExists(string fileName)
        {
          //  string fileName = "C:\\TFS\\test.xlsx";

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            this.workbook.SaveAs(fileName);
        }

        public void ExportDataTable(DataTable dataTable,int startRow,int starCol,XLColor headerColor,XLColor contentColor)
        {               
            IXLTable table=activeSheet.Cell(startRow, starCol).InsertTable(dataTable);

            table.Theme = XLTableTheme.None;

            
            table.HeadersRow().Style.Font.Bold = true;
            table.Cells().Style.Fill.BackgroundColor = contentColor;
            table.HeadersRow().Style.Fill.BackgroundColor = headerColor;
            activeSheet.Columns().AdjustToContents();
        }

        public string GetAddress(int startRow, int startCol)
        {
            IXLCell cell = this.activeSheet.Cell(startRow, startCol);

            string adress = cell.Address.ToString();

            return adress;
        }

        public void AssignFormulaCell(int row,int col, string formula)
        {
            IXLCell cell = this.activeSheet.Cell(row, col);

            cell.FormulaA1 = formula;
        }


        public void AssignFormulaToRange(int startRow, int startCol, int rowLength, int columnLenght, string formula)
        {
            IXLRange startCell = this.activeSheet.Range(startRow, startCol, startRow, startCol);

            //Excel.Range startCell = 
                //activeWorkSheet.Range[activeWorkSheet.Cells[startRow, startCol], activeWorkSheet.Cells[startRow, startCol]];

            //string re = startCell.AddressLocal;

           // string re=startCell.RangeAddress

            startCell.FormulaA1 = formula;
           // startCell.Value = formula;

            //ix

            int endRow = startRow + rowLength - 1;
            int endCol = startCol + columnLenght - 1;

            IXLRange pasteRange = this.activeSheet.Range(startRow, startCol, endRow, endCol);
            //pasteRange.fo(startCell);
           // pasteRange.FormulaA1 = startCell.Cell(1,1).FormulaA1;

            //startCell.CopyTo(pasteRange);



            //Excel.Range ranger = activeWorkSheet.Range[activeWorkSheet.Cells[startRow, startCol],
            //              activeWorkSheet.Cells[startRow + rowLength - 1, startCol + columnLenght - 1]];





            ////Set the range value to the array.
            //ranger.PasteSpecial(Excel.XlPasteType.xlPasteAll);
        }
    }
}
