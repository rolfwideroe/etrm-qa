using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using DealInsertXmlGenerator.Controller;
using DealInsertXmlGenerator.Controller.Interfaces;
using DealInsertXmlGenerator.Model;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;

namespace DealInsertXmlGenerator.ExcelAddIn
{
    public partial class ThisAddIn:ITransactionView
    {

        private TransactionViewController controller;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            this.controller = new TransactionViewController(this);
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

        public void RequestDealInsertXml()
        {
            this.controller.GetTransactionsAndGenarateXml();
        }

        public void ShowUserError(string errorString)
        {
            MessageBox.Show(errorString);
        }

        public void ShowApplicationError(string errorString)
        {
            MessageBox.Show(errorString);
        }

        public void ShowUnknownError(string errorString)
        {
            MessageBox.Show(errorString);
        }

        public void ShowMessage(string messageString)
        {
            MessageBox.Show(messageString);
        }

        public bool ShowOkCancel(string messageString, string headerText)
        {
            throw new NotImplementedException();
        }

        public IList<TransactionWrapper> Wrappers
        {
            get
            {

                Excel.Worksheet activeWorksheet = ((Excel.Worksheet)Application.ActiveSheet);

                Excel.Range startCell = activeWorksheet.Range["a6"];
                Excel.Range endCell = startCell.End[Excel.XlDirection.xlToRight];

                Excel.Range transRange = activeWorksheet.Range[endCell, startCell.End[Excel.XlDirection.xlDown]];


                object[,] test = (object[,])transRange.Value;
                int numberOfTransactions = test.GetLength(0) - 1;
                int propLength = test.GetLength(1);


                IList<TransactionWrapper> wrapperList = new List<TransactionWrapper>();

                for (int j = 2; j <= numberOfTransactions + 1; j++)
                {
                    TransactionWrapper wrapper = new TransactionWrapper();

                    for (int i = 1; i <= propLength; i++)
                    {
                        string prop = (string)test[1, i];
                        object value = (object)test[j, i];

                        if (value == null)
                            continue;

                        TransactionWrapperHelper.SetProp(wrapper, prop, value);
                    }

                    if (wrapper.TimeSeriesId != null)
                    {
                        TimeSeriesDetailWrapper[] timeSeriesDetail = this.GetTimeSeries(wrapper.TimeSeriesId);
                        wrapper.TimeSeriesDetailWrappers = timeSeriesDetail;
                    }

                    wrapperList.Add(wrapper);

                }

                return wrapperList;

            }
        }

        public string XmlFilePath
        {
            get
            {
                Excel.Worksheet activeWorksheet = ((Excel.Worksheet)Application.ActiveSheet);

                Excel.Range range = activeWorksheet.Range["b1"];

                return range.Value2;
            }
        }

        public TransactionWrapperType TransactionWrapperType
        {
            get
            {
                Excel.Worksheet activeWorksheet = ((Excel.Worksheet)Application.ActiveSheet);

                Excel.Range range = activeWorksheet.Range["b3"];

                string typeString = range.Value2;

                TransactionWrapperType type = TransactionWrapperHelper.GetTransactionType(typeString);

                return type;
            }
        }

        private TimeSeriesDetailWrapper[] GetTimeSeries(string timeSeriesId)
        {

            Excel.Worksheet worksheet = ((Excel.Worksheet)Application.Worksheets["TimeSeries"]);

            Excel.Range startCell = worksheet.Range["a1"];
            Excel.Range endCell = worksheet.Range["i1"];

            Excel.Range range = worksheet.Range[endCell, startCell.End[Excel.XlDirection.xlDown]];

            object[,] test = (object[,])range.Value;

            int numberOfWrappers = test.GetLength(0) - 1;
            TimeSeriesDetailWrapper[] wrappers = new TimeSeriesDetailWrapper[numberOfWrappers];

            for (int i = 0; i < numberOfWrappers; i++)
            {
                string id = (string)test[i + 2, 1];
                DateTime fromDate = (DateTime)test[i + 2, 2];
                DateTime toDate = (DateTime)test[i + 2, 3];
                double volume = (double)test[i + 2, 4];
                double price = (double)test[i + 2, 5];
                double volumeOutsideProfile = test[i + 2, 6] is double ? (double)test[i + 2, 6] : 0;
                double pricesOutsideProfile = test[i + 2, 7] is double ? (double)test[i + 2, 7] : 0;
                double realizedVolume = test[i + 2, 8] is double ? (double)test[i + 2, 8] : 0;
                double realizedPrice = test[i + 2, 9] is double ? (double)test[i + 2, 9] : 0;

                wrappers[i] = new TimeSeriesDetailWrapper(id, fromDate, toDate, volume, price, volumeOutsideProfile, pricesOutsideProfile, realizedVolume, realizedPrice);
            }

            return (TimeSeriesDetailWrapper[])wrappers.Where(x => x.TimeSeriesId == timeSeriesId).ToArray();


        }
    }
}

