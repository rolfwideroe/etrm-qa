using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ElvizTestUtils;
using Microsoft.Office.Tools.Ribbon;

namespace BatchTestCompareExcelAddIn
{
    public partial class BatchTestCompareRibbon
    {
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void compareResults_Click(object sender, RibbonControlEventArgs e)
        {

            Globals.ThisAddIn.RequestCompare();

        }

        private void reportEngineCompareButton_Click(object sender, RibbonControlEventArgs e)
        {
           Globals.ThisAddIn.RequestReportEngineCompare();
        }

        private void updateBatchTestButton_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.RequestUpdateBatchFile();
        }

		private void updateReportEngineTestIncludingColumnsButton_Click(object sender, RibbonControlEventArgs e)
		{
			Globals.ThisAddIn.RequestUpdateReportEngineFile(true);
		}

		private void updateReportEngineTestButton_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.RequestUpdateReportEngineFile(false);
        }

        private void compareCurveTestButton_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.RequestCurveCompare();
        }

        private void compareExchangeRateButton_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.RequestCompareExchangeRates();
        }

        private void createBatchFilesButton_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.RequestCreateBatchFiles();
        }

        private void compareReportDbTimeSeriesButton_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.RequestCompareRdTimeSeriesEcm();
        }

        private void compareRdTimeseriesButton_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.RequestCompareRTimeSeries();
        }

        private void updateRdTimeSeriesButton_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.RequestUpdateRdTimeSeries();
        }

        private void compareCustomDwhButton_Click(object sender, RibbonControlEventArgs e)
        {
            Globals.ThisAddIn.RequestCompareCustomDwh();
        }
    }
}
