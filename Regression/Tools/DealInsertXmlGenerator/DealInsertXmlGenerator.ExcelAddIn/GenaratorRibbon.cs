using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace DealInsertXmlGenerator.ExcelAddIn
{
    public partial class GenaratorRibbon
    {
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void GenerateButton_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.RequestDealInsertXml();
        }
    }
}
