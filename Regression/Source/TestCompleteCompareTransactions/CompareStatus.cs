using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Xml;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.LookUpServiceReference;
using ElvizTestUtils.QaLookUp;
using NUnit.Framework;

namespace TestCompleteCompareTransactions
{
    class CompareStatus
    {
        [Test]
        public static string GetTransactionsStatus(int transID)
        {
           //  int transID = 105;
            string result;
            ILookupService serviceLookup =  WCFClientUtil.GetLookUpServiceServiceProxy();

            int[] transIDs = { transID };
            TransactionDTO[] transaction = serviceLookup.GetTransactionsByIds(transIDs);
            if (transaction.Length > 0)
            {
                result = (transaction[0].Status).ToString();
                //SaveQATransactionDTOtoXML(transID);

            }
            else result = "Error retrieving transaction by ID";

            return result;
        }

       // [Test]
        public static void SaveQATransactionDTOtoXML(int id)
        {
          //  int id = 691;
            string relativePath = "\\DealManager\\DealManager\\TestFiles\\ExpectedQaTransactionDTO\\Status\\";
            string baseDirectory = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
            string filename = Path.GetFullPath(baseDirectory + relativePath) + id + ".xml";
         
            QaLookUpClient c = new QaLookUpClient();
            QaTransactionDTO dto = c.GetQaTransactionDTO(id);
            Assert.AreEqual(dto.TransactionId, id);

            XmlDocument xmlDocument = new XmlDocument();
            XmlSerializer serializer = new XmlSerializer(dto.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, dto);
            stream.Position = 0;
            xmlDocument.Load(stream);
            xmlDocument.Save(filename);
        }
        
        public static double TOLERANCE = 0.00001;

        [Test]
        public static IList<string> _Debug_CompareChangedStatus()
        {
           int updId = 113;
            IList<string> errorList = new List<string>();

            string relativePath = "\\Source\\TestComplete\\DealManager\\DealManager\\TestFiles\\ExpectedQaTransactionDTO\\Status\\";
            string baseDirectory = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
            string filename = Path.GetFullPath(baseDirectory + relativePath) + updId + ".xml";

            QaLookUpClient c = new QaLookUpClient();
            QaTransactionDTO dto = c.GetQaTransactionDTO(updId);

            XmlSerializer serializer = new XmlSerializer(typeof(QaTransactionDTO));
            XmlTextReader reader = new XmlTextReader(filename);
            reader.ReadToDescendant("QaTransactionDTO");
            QaTransactionDTO expectedTransactionDTO = (QaTransactionDTO)serializer.Deserialize(reader.ReadSubtree());
            reader.Close();

            string[] excludeProps = new string[] { "TransactionId", 
                                                   "ReferenceData.ModificationDateTimeUtc", 
                                                   "ReferenceData.ReferringId", 
                                                   "TransactionWorkFlowDetails.TimeStampAuthorised", 
                                                   "TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised", 
                                                    "TransactionWorkFlowDetails.TimeStampClearedUtc"       };
            
            errorList = QaTransactionDtoAssert.AreEqualWithErrorList(expectedTransactionDTO, dto, excludeProps, false);

            foreach (string error in errorList)
            {
                Console.WriteLine(error);
            }

            return errorList;
        }
        
        public static IList<string> CompareChangedStatus(int updId)
        {
            //int updId = 119;
            IList<string> errorList = new List<string>();

            string relativePath = "\\DealManager\\DealManager\\TestFiles\\ExpectedQaTransactionDTO\\Status\\";
            string baseDirectory = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
            string filename = Path.GetFullPath(baseDirectory + relativePath) + updId + ".xml";
            
            QaLookUpClient c = new QaLookUpClient();
            QaTransactionDTO dto = c.GetQaTransactionDTO(updId);
            
            XmlSerializer serializer = new XmlSerializer(typeof(QaTransactionDTO));
            XmlTextReader reader = new XmlTextReader(filename);
            reader.ReadToDescendant("QaTransactionDTO");
            QaTransactionDTO expectedTransactionDTO = (QaTransactionDTO)serializer.Deserialize(reader.ReadSubtree());
            reader.Close();

            string[] excludeProps = {   "TransactionId", 
                                        "ReferenceData.ModificationDateTimeUtc", 
                                        "ReferenceData.ReferringId", 
                                        "TransactionWorkFlowDetails.TimeStampAuthorised", 
                                        "TransactionWorkFlowDetails.TimeStampCounterpartyAuthorised",
                                         "TransactionWorkFlowDetails.TimeStampClearedUtc" 
                                    };
            errorList = QaTransactionDtoAssert.AreEqualWithErrorList(expectedTransactionDTO, dto, excludeProps, false);
            
            return errorList; 
        }
      
    }
}
