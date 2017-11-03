using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.LookUpServiceReference;
using NUnit.Framework;

// Description
// 1 step - create XML with TransactionDTO - modify original transaction with parematers -  CreateXMLTransactionDTO()
// 2 step - get TransactionDTO after multipleupdate and compare with TransactionDTO from XML as two objects - CompareTwoAsObjectsWithOriginalFromXML()

namespace CompareTransactions
{
    class CompareTransactionsForStatusTests
    {
        public static string path = @"\\Berpc-in7\Shared\DealManager\DealManager\TestFiles\TestInfo\Status\";
       
        [Test]
        public static string GetTransactionsStatus()//int transID)
        {
            int transID = 103;
            string result;
            ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
            
            int[] transIDs = { transID };
            TransactionDTO[] transaction = serviceLookup.GetTransactionsByIds(transIDs);
            if (transaction.Length > 0)
            {
                result = (transaction[0].Status).ToString();
                
            }
                else result= "Error retrieving transaction by ID";
            
            return result;
        }

        [Test]
        public static List<string> CreateXMLTransactionDTOStatus(int transID, string newstatus)
        {
            //int transID = 103;
            //string newstatus = "Active"; 

            ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
            //ILookupService serviceLookup = new LookupServiceClient();
            var results = new List<string>();
           int[] transIDs = { transID };
            TransactionDTO[] origTransaction = serviceLookup.GetTransactionsByIds(transIDs);

            if (origTransaction.Length > 0)
            {
                try
                {
                    //find enum element by string value

                    TransactionStatus setStatus =
                        (TransactionStatus) System.Enum.Parse(typeof (TransactionStatus), newstatus);

                    Console.WriteLine(setStatus.ToString());
                    DateTime modificationDate = new DateTime();
                    origTransaction[0].ModificationDate = modificationDate;
                    origTransaction[0].ExpiryDate = modificationDate;
                    origTransaction[0].Status = setStatus;

                    Assert.AreEqual(modificationDate, origTransaction[0].ModificationDate);
                    Assert.AreEqual(modificationDate, origTransaction[0].ExpiryDate);
                    Assert.AreEqual(setStatus, origTransaction[0].Status);


                    System.Xml.Serialization.XmlSerializer writer =
                        new System.Xml.Serialization.XmlSerializer(origTransaction[0].GetType());
                    string filename = path + "StatusTransactionDTO-" + transID + ".xml";
                    System.IO.StreamWriter file = new System.IO.StreamWriter(filename, false);
                    Console.WriteLine(filename);
                    writer.Serialize(file, origTransaction[0]);
                    file.Close();
                }
                catch (Exception exception)
                {
                    results.Add("Error when trying to create a XML-file. ID = " + transID + "; Status= "+ newstatus );
                    results.Add(exception.Message);
                    Console.WriteLine(exception.Message);
                    //throw;
                }
               
                
               
               //// string actualfilename = path + +origId + ".xml";
               // //XMLDiffCompare(filename, actualfilename, origId);

            }
            else results[0] = "Error retrieving original transaction by ID";
            
            return results;
        }

      
        [Test]
        public static List<string> CompareTwoAsObjectsWithOriginalFromXML(int updId)
       {
          
           //int updId = 103 ;
           var results = new List<string>();

           ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
          // ILookupService serviceLookup = new LookupServiceClient();

           XmlSerializer mySerializer = new XmlSerializer(typeof(TransactionDTO)); //(origTr.GetType());
           FileStream myFileStream = new FileStream(path + "StatusTransactionDTO-" + updId + ".xml", FileMode.Open);
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
            // if transaction chnages status to Cleared - TimeStampClearedUTC = null
           dtoup.TimeStampClearedUTC = null;
           
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
