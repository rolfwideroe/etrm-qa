using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;

namespace TestCompleteCompareTransactions
{
    class CompareCreatedDeal
    {
        //compare QATransactionDTO from ExpectedQaTransactionDTO with current transaction after updating
        [Test]
        public IList<string> CompareCreatedQaTransactionDTO(int id, string path)
        {
            //int id = 2047;
            //string currentfile = "PassElectricityForwardFee-EEX-NPSP-All.xml";
            IList<string> errors = new string[] { };
            
            XmlSerializer serializer = new XmlSerializer(typeof(QaTransactionDTO));
            XmlTextReader reader = new XmlTextReader(path);
            reader.ReadToDescendant("QaTransactionDTO");
            QaTransactionDTO originalDto = (QaTransactionDTO)serializer.Deserialize(reader.ReadSubtree());
            reader.Close();

            QaLookUpClient c = new QaLookUpClient();
            QaTransactionDTO updatedDto = c.GetQaTransactionDTO(id);

            if (updatedDto.TransactionId != id)
            {
                errors.Add("QaTransaction.GetQaTransactionDTO() return wrong TransactionId: " + updatedDto.TransactionId + " != " + id);
                return errors;
            }

            //change obviously different properties to the same
            DateTime modificationDate = new DateTime();

            updatedDto.ReferenceData.ModificationDateTimeUtc = modificationDate;
            string[] excludeProps = {   "TransactionId", 
                                        "ReferenceData.ModificationDateTimeUtc", 
                                        "ReferenceData.ReferringId", 
                                        "TransactionWorkFlowDetails.TimeStampAuthorised", 
                                        "TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised",
                                         "TransactionWorkFlowDetails.TimeStampClearedUtc" 
                                    };
            errors = QaTransactionDtoAssert.AreEqualWithErrorList(originalDto, updatedDto, excludeProps, false);
           // return errors;

            // //print out alll errors    
            //foreach (var item in errors)
            //{
            //    Console.WriteLine(item);
            //}

             return errors;
        }  
    }

}
