using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DealEntryXml.Generator.Model;
using DealEntryXmlGenerator.Controller.Interfaces;
using DealEntryXmlGenerator.Wrapper.Model;

namespace DealEntryXmlGenerator.Controller
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

            if (string.IsNullOrEmpty(xmlPath))
            {
                this.view.ShowUserError("Missing Filepath");
                return;
            }


            try
            {
                IList<DealEntryWrapper> wrappers = this.view.Wrappers;


                XmlGenerator generator = new XmlGenerator(xmlPath);

                foreach (DealEntryWrapper wrapper in wrappers)
                {
                    generator.GenerateDealEntryXmlFile(wrapper);
                }

    

                this.view.ShowMessage("OK");
            }
            catch (Exception ex)
            {
                this.view.ShowApplicationError(ex.Message.ToString());
            }


        }
    }
}
