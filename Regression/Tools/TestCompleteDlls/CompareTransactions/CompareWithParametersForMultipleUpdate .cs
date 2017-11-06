using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.LookUpServiceReference;
using NUnit.Framework;

// Description
// 1 step - create XML with TransactionDTO - modify original transaction with parematers -  CreateXMLTransactionDTO()
// 2 step - get TransactionDTO after multipleupdate and compare with TransactionDTO from XML as two objects - CompareTwoAsObjectsWithOriginalFromXML()

namespace CompareTransactions
{
    
    class CompareWithParametersMultipleUpdate
    {
        public static string path = @"\\Berpc-in7\Shared\DealManager\DealManager\TestFiles\TestInfo\MultipleUpdate\";

        [Test]
        public static void CreateXMLTransactionDTO(int transID, double clearingFee, string ticketN, string dealgroupfirst)
        {
            //int transID = 65;
            //double clFee = 0.23;
            //string ticketN = "number 2"; 

            ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
            //ILookupService serviceLookup = new LookupServiceClient();
            string[] results = { "no error" };
            int[] transIDs = { transID };
            TransactionDTO[] origTransaction = serviceLookup.GetTransactionsByIds(transIDs);

            if (origTransaction.Length > 0)
            {
                DateTime modificationDate = new DateTime();
                origTransaction[0].ModificationDate = modificationDate;
                origTransaction[0].ExpiryDate = modificationDate;
                origTransaction[0].ClearingFee = clearingFee;
                origTransaction[0].TicketNumber = ticketN ;
                origTransaction[0].DealGroup = dealgroupfirst;  //fields have same values!!! Recheck after changint TransactionDTO
                origTransaction[0].GroupField1 = dealgroupfirst;//

                Assert.AreEqual(modificationDate, origTransaction[0].ModificationDate);
                Assert.AreEqual(modificationDate, origTransaction[0].ExpiryDate);
                Assert.AreEqual(clearingFee, origTransaction[0].ClearingFee);
                Assert.AreEqual(ticketN, origTransaction[0].TicketNumber);
                Assert.AreEqual(dealgroupfirst, origTransaction[0].DealGroup);
                Assert.AreEqual(dealgroupfirst, origTransaction[0].GroupField1);

                System.Xml.Serialization.XmlSerializer writer =
                    new System.Xml.Serialization.XmlSerializer(origTransaction[0].GetType());
                string filename = path + "OriginalTransactionDTO-" + transID + ".xml";
                System.IO.StreamWriter file = new System.IO.StreamWriter(filename, false);
                Console.WriteLine(filename);
                writer.Serialize(file, origTransaction[0]);
                file.Close();
               // string actualfilename = path + +origId + ".xml";
                //XMLDiffCompare(filename, actualfilename, origId);

            }
            else results[0] = "Error retrieving original transaction by ID";
        }

      
        [Test]
        public static List<string> CompareTwoAsObjectsWithOriginalFromXML(int updId)
       {
           //int origId = 21;
          // int updId = 27 ;
           var results = new List<string>();

           ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
          // ILookupService serviceLookup = new LookupServiceClient();

           XmlSerializer mySerializer = new XmlSerializer(typeof(TransactionDTO)); //(origTr.GetType());
           FileStream myFileStream = new FileStream(path + "OriginalTransactionDTO-"+ updId+ ".xml", FileMode.Open);
           // Call the Deserialize method and cast to the object type.
           TransactionDTO origTr = (TransactionDTO)
                    mySerializer.Deserialize(myFileStream);
        
            
           int[] upIds = { updId };
           TransactionDTO[] updTransactions = serviceLookup.GetTransactionsByIds(upIds);
         

           if (updTransactions.Length != 1)
           {
               results.Add("Error retrieving updated transaction by ID: expected only one transaction");
               return results;
           }
           //change obviously different properties to the same
           
           DateTime modificationDate = new DateTime();
           
            TransactionDTO dtoup = updTransactions[0];
           dtoup.ModificationDate = modificationDate;
           dtoup.ExpiryDate = modificationDate;
           
           Assert.AreEqual(modificationDate, dtoup.ModificationDate);
           Assert.AreEqual(modificationDate, dtoup.ExpiryDate);
           //Console.WriteLine(dto.TradeTime + " == " + dtoup.TradeTime);
           
            myFileStream.Close();
           string[] errorStrings = CompareTransactionsAsObjects.CompareTransactions(origTr, updTransactions[0]);

           results.AddRange(errorStrings);

           foreach (var item in results) Console.WriteLine(item);
                  
        return results;
      }
    }
}
