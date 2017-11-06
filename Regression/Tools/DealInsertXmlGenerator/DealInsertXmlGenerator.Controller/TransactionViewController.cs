using System;
using System.Collections.Generic;
using DealInsertXmlGenerator.Controller.Interfaces;
using DealInsertXmlGenerator.Model;

namespace DealInsertXmlGenerator.Controller
{
    public class TransactionViewController
    {
        private readonly ITransactionView view;

        public TransactionViewController(ITransactionView view)
        {
            this.view = view;
        }

        public void GetTransactionsAndGenarateXml()
        {
            string xmlPath = this.view.XmlFilePath;

            if(string.IsNullOrEmpty(xmlPath))
            {
                this.view.ShowUserError("Missing Filepath");
                return;
            }


            try
            {
                   IList<TransactionWrapper> wrappers = this.view.Wrappers;


                   XmlGenerator generator = new XmlGenerator(this.view.TransactionWrapperType);

                   foreach (TransactionWrapper wrapper in wrappers)
                   {
                       generator.AddWrapper(wrapper);
                   }

                   generator.GenerateBulkInsertXmlFile(xmlPath);

                   this.view.ShowMessage("OK");
            }
            catch (Exception ex)
            {
                this.view.ShowApplicationError(ex.Message.ToString());
            }


        }
    }
}
