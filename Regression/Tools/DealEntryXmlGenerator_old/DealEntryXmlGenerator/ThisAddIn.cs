using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using DealEntryXmlGenerator.Controller;
using DealEntryXmlGenerator.Controller.Interfaces;
using DealEntryXmlGenerator.Wrapper.Model;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;

namespace DealEntryXmlGenerator
{
    public partial class ThisAddIn:ITransactionView 
    {
        private TransactionViewController controller;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            this.controller=new TransactionViewController(this);
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

        public void RequestXml()
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

        public IList<DealEntryWrapper> Wrappers
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


                IList<DealEntryWrapper> wrapperList = new List<DealEntryWrapper>();

                for (int j = 2; j <= numberOfTransactions + 1; j++)
                {
                    DealEntryWrapper wrapper = new DealEntryWrapper();

                    for (int i = 1; i <= propLength; i++)
                    {
                        string prop = (string)test[1, i];
                        object value = (object)test[j, i];

                        if (value == null)
                            continue;

                        TransactionWrapperHelper.SetProp(wrapper, prop, value);
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
    }
}
