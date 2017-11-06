using System;
using System.Collections.Generic;
using System.Linq;
using ElvizTestUtils;
using ElvizTestUtils.LookUpServiceReference;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;

namespace CompareTransactions
{
    class CompareTransactionsAsObjects
    {
        [Test]
        public static List<string> CompareTwoAsObjects(int origId, int updId)
       {
           //int origId = 145;
           //int updId = 1059;
           var results = new List<string>();

           ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();
           
           int[] orIds = { origId };
           TransactionDTO[] origTransaction = serviceLookup.GetTransactionsByIds(orIds);
           int[] upIds = { updId };
           TransactionDTO[] updTransactions = serviceLookup.GetTransactionsByIds(upIds);

           if (origTransaction.Length != 1)
           {
               results.Add("Error retrieving original transaction by ID: expected only one transaction");
               return results;
           }

           if (updTransactions.Length != 1)
           {
               results.Add("Error retrieving updated transaction by ID: expected only one transaction");
               return results;
           }
           //change obviously different properties to the same
           TransactionDTO dto = origTransaction[0];
           dto.TransactionId = 0;
           DateTime modificationDate = new DateTime();
           origTransaction[0].ModificationDate = modificationDate;
           dto.ModificationDate = modificationDate;
           dto.ExpiryDate = modificationDate;
           Assert.AreEqual(0, dto.TransactionId);
           Assert.AreEqual(modificationDate, dto.ModificationDate);
           Assert.AreEqual(modificationDate, dto.ExpiryDate);
           
           TransactionDTO dtoup = updTransactions[0];
           dtoup.TransactionId = 0;
           dtoup.ModificationDate = modificationDate;
           dtoup.ExpiryDate = modificationDate;
           Assert.AreEqual(0, dtoup.TransactionId);
           Assert.AreEqual(modificationDate, dtoup.ModificationDate);
           Assert.AreEqual(modificationDate, dtoup.ExpiryDate);
           //Console.WriteLine(dto.TradeTime + " == " + dtoup.TradeTime);

           string[] errorStrings = CompareTransactions(origTransaction[0], updTransactions[0]);

           results.AddRange(errorStrings);

           foreach (var item in results) Console.WriteLine(item);
                  
        return results;
      }



       // function from KellermanSoftware.CompareNetObjects - if it needs should be added as References
       public static string[] CompareTransactions(TransactionDTO origTransaction, TransactionDTO updTransactions)
       {

           Type t = origTransaction.Currency.CurrencyId.GetType();
           var compareObjects = new CompareObjects()
                                    {
                                        CompareChildren = true,
                                        //this turns deep compare one, otherwise it's shallow
                                        CompareFields = false,
                                        CompareReadOnly = true,
                                        ComparePrivateFields = false,
                                        ComparePrivateProperties = false,
                                        CompareProperties = true,
                                        MaxDifferences = 10,
                                        ElementsToIgnore = new List<string>() { "Filter", "CurrencyId", "ReferringId", "TimeSeriesGuid" }
                                    };
           bool hasDiff = compareObjects.Compare(origTransaction, updTransactions);

           IList<string> errorStrings = new List<string>();

           if (!hasDiff)
           {
               foreach (Difference difference in compareObjects.Differences)
               {
                   string propName = difference.PropertyName;
                   string expectedValue = difference.Object1Value;
                   string actualValue = difference.Object2Value;
                   string errorString = propName + " - Expected : " + expectedValue + " , But was : " + actualValue;

                   errorStrings.Add(errorString);

               }
           }

           return errorStrings.ToArray();
       }

        [Test]
       public static List<string> DebugCompareTwoAsObjects()
       {
           int origId = 1901;
           int updId = 77;
            var results = new List<string>();

            ILookupService serviceLookup = WCFClientUtil.GetLookUpServiceServiceProxy();

           int[] orIds = { origId };
           TransactionDTO[] origTransaction = serviceLookup.GetTransactionsByIds(orIds);
           int[] upIds = { updId };
           TransactionDTO[] updTransactions = serviceLookup.GetTransactionsByIds(upIds);

           if (origTransaction.Length != 1)
           {
               results.Add("Error retrieving original transaction by ID: expected only one transaction");
               return results;
           }

           if (updTransactions.Length != 1)
           {
               results.Add("Error retrieving updated transaction by ID: expected only one transaction");
               return results;
           }
           //change obviously different properties to the same
           TransactionDTO dto = origTransaction[0];
           dto.TransactionId = 0;
           DateTime modificationDate = new DateTime();
           origTransaction[0].ModificationDate = modificationDate;
           dto.ModificationDate = modificationDate;
           dto.ExpiryDate = modificationDate;
           Assert.AreEqual(0, dto.TransactionId);
           Assert.AreEqual(modificationDate, dto.ModificationDate);
           Assert.AreEqual(modificationDate, dto.ExpiryDate);

           TransactionDTO dtoup = updTransactions[0];
           dtoup.TransactionId = 0;
           dtoup.ModificationDate = modificationDate;
           dtoup.ExpiryDate = modificationDate;
           Assert.AreEqual(0, dtoup.TransactionId);
           Assert.AreEqual(modificationDate, dtoup.ModificationDate);
           Assert.AreEqual(modificationDate, dtoup.ExpiryDate);
           //Console.WriteLine(dto.TradeTime + " == " + dtoup.TradeTime);

           string[] errorStrings = CompareTransactions(origTransaction[0], updTransactions[0]);

           results.AddRange(errorStrings);

           foreach (var item in results) Console.WriteLine(item);

           return results;
       }


    }
}
