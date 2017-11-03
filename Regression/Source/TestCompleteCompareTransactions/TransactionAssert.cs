using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.LookUpServiceReference;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;

namespace TestCompleteCompareTransactions
{
    public class TransactionAssert
    {
        //compare QATransactionDTO for all transactions in filter with ExpectedQaTransactionDTO 
        [Test]

        public IList<int> GetAllTransactionInFilter(string filterName)
        {
            //string filterName = "MultipleUpdate";
            ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
            TransactionDTO[] allTransactionInFilter = serviceLookup.GetTransactionsInFilter("MultipleUpdate", null);

            IList<int> ids = new List<int>();

            foreach (TransactionDTO item in allTransactionInFilter)
            {
                ids.Add(item.TransactionId);
               // Console.WriteLine(item.TransactionId);
            }
            return ids;
        }

        [Test]
        public IList<string> CompareTransactionWithExpectedQaDTO(int transaction)
        {
            //int transaction = 99;
            IList<string> errors = new string[] { };

            //*** debug
            //string relativePath = "\\Source\\TestComplete\\DealManager\\DealManager\\TestFiles\\BulkUpdate\\BulkUpdateExpectedQATransactionDTO\\";
            //string baseDirectory = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
            //*** debug

            string relativePath = "\\DealManager\\DealManager\\TestFiles\\BulkUpdate\\BulkUpdateExpectedQATransactionDTO\\";
            string baseDirectory = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();

            // Directory.GetFiles(Path.GetFullPath(baseDirectory + relativePath); 


            string filenameTemplate = transaction + "_*.xml";
                string[] dirs = Directory.GetFiles(Path.GetFullPath(baseDirectory + relativePath), filenameTemplate);

                if (dirs.Length != 1)
                {
                    errors.Add(" Can't find file with expected QATransactionDTO for transaction with id=" +
                               transaction);
                    return errors;
                }

                string filename = dirs[0];

                XmlSerializer serializer = new XmlSerializer(typeof(QaTransactionDTO));
                XmlTextReader reader = new XmlTextReader(filename);
                reader.ReadToDescendant("QaTransactionDTO");
                QaTransactionDTO expecteDto = (QaTransactionDTO) serializer.Deserialize(reader.ReadSubtree());
                reader.Close();

                QaLookUpClient c = new QaLookUpClient();
                QaTransactionDTO updatedDto = c.GetQaTransactionDTO(transaction);

                //change obviously different properties to the same
                DateTime modificationDate = new DateTime();

                updatedDto.ReferenceData.ModificationDateTimeUtc = modificationDate;
                string[] excludeProps =
                {
                    "TransactionId",
                    "ReferenceData.ModificationDateTimeUtc",
                    "ReferenceData.ReferringId",
                    "TransactionWorkFlowDetails.TimeStampAuthorised",
                    "TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised",
                    "TransactionWorkFlowDetails.TimeStampClearedUtc"
                };
                errors = QaTransactionDtoAssert.AreEqualWithErrorList(expecteDto, updatedDto, excludeProps, false);



            //print out alll errors
            //foreach (var item in errors)
            //{
            //    Console.WriteLine(item);
            //}

            return errors;

        }
       
    }
}
