using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using ElvizTestUtils;
using ElvizTestUtils.CurrencyAccountsServiceReference;
using NUnit.Framework;

namespace TestWCFCurrencyAccountBalanceService
{
    public class TestWCFCurrencyAccountService
    {

        [Test]
        public void FailIncorrectIsoCurrency()
        {
            const string expectedError = "An exception occurred in CurrencyAccountManager.UpdateCurrencyAccountBalances. : \r\nSpecified argument was out of the range of valid values.\r\nParameter name: Unsupported currency 123";

            ICurrencyAccountService service = WCFClientUtil.GetCurrencyAccountServiceProxy();

            CurrencyAccountDTO dto =new CurrencyAccountDTO(){BalanceDate = new DateTime(2014,01,01),CurrencyIsoCode = "123",NominalAmount = 123};

            try
            {
                service.UpdateCurrencyAccountBalances(new DateTime(2014,01,01),new []{dto});
            }
            catch (Exception ex)
            {
                

                Console.WriteLine(ex.Message);
                Assert.AreEqual(expectedError,ex.Message);
                return;
            }

            Assert.Fail("Test Passed but Expected to fail with error message:\n" + expectedError);
        }

        //[Test]
        //public void FailNotEnabledIsoCurrency()
        //{
        //    const string expectedError = "An exception occurred in CurrencyAccountManager.UpdateCurrencyAccountBalances. : \r\nSpecified argument was out of the range of valid values.\r\nParameter name: Unsupported currency ECU";

        //    ICurrencyAccountService service = WCFClientUtil.GetCurrencyAccountServiceProxy();

        //    CurrencyAccountDTO dto = new CurrencyAccountDTO() { BalanceDate = new DateTime(2014, 01,01), CurrencyIsoCode = "ECU", NominalAmount = 123 };

        //    try
        //    {
        //        service.UpdateCurrencyAccountBalances(new DateTime(2014, 01, 01), new[] { dto });
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        Assert.AreEqual(expectedError, ex.Message);
        //        return;
        //    }

        //    Assert.Fail("Test Passed but Expected to fail with error message:\n"+expectedError);
        //}

        [Test]
        public void TestCorrectUpdatedByUtc()
        {
            DateTime balanceDate = new DateTime(2001, 01, 01);

            ICurrencyAccountService service = WCFClientUtil.GetCurrencyAccountServiceProxy();

            CurrencyAccountDTO dto = new CurrencyAccountDTO() {CurrencyIsoCode = "EUR", NominalAmount = 123 };

            service.UpdateCurrencyAccountBalances(balanceDate,new[]{dto});

            CurrencyAccountDTO resultDto = service.GetCurrencyAccounts(balanceDate, balanceDate)[0];

            //Tollerance of +- 10 sec
            Assert.That(resultDto.UpdateTimeStampUtc,Is.EqualTo(DateTime.UtcNow).Within(10).Seconds);
        }

        private static readonly IEnumerable<string> TestFilesWCFCurrencyAccountService = TestCasesFileEnumeratorByFolder.TestCaseFiles("TestFiles");

        [Test]
        public void TestAccountsNotAvailable()
        {
            DateTime balanceDate = new DateTime(2005, 01, 01);

            ICurrencyAccountService service = WCFClientUtil.GetCurrencyAccountServiceProxy();

            CurrencyAccountDTO dto = new CurrencyAccountDTO() { CurrencyIsoCode = "NOK", NominalAmount = 123 };

            service.UpdateCurrencyAccountBalances(balanceDate, new[] { dto });

            DateTime from=new DateTime(2004-01-01);
            DateTime to =new DateTime(2014-01-05);

            CurrencyAccountDTO[] resultDto = service.GetCurrencyAccounts(from, to);

            Assert.AreEqual(0,resultDto.Length);


        }


        [Test, Timeout(1000*1000), TestCaseSource("TestFilesWCFCurrencyAccountService")]
        public void TestWcfInsertAndRetrieveCurrencyBalance(string testFile)
        {            
            string filepath = Path.Combine(Directory.GetCurrentDirectory(),"TestFiles\\" + testFile);


            CurrencyAccountTestCase testCase = TestXmlTool.Deserialize<CurrencyAccountTestCase>(filepath);

            ICurrencyAccountService service = WCFClientUtil.GetCurrencyAccountServiceProxy();
            //first of all we will update the currency accounts with the data from the test case.
            foreach (Insert insert in testCase.InputData.CurrencyAccountInserts)
            {
                IList<CurrencyAccountDTO> currencyAccountDTOs = new List<CurrencyAccountDTO>();
                foreach (CurrencyAccount currencyAccount  in insert.CurrencyAccounts)
                {
                    CurrencyAccountDTO currencyAccountDTO =new CurrencyAccountDTO()
                    {
                        BalanceDate =  currencyAccount.BalanceDate,
                        CurrencyIsoCode = currencyAccount.CurrencyIsoCode,
                        NominalAmount = currencyAccount.NominalAmount,
                        UpdatedBy = currencyAccount.UpdatedBy,
                      
                    };

                    currencyAccountDTOs.Add(currencyAccountDTO);

                    try
                    {
                        service.UpdateCurrencyAccountBalances(insert.InsertBalanceDate, currencyAccountDTOs.ToArray());
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail(string.Format("Test failed updating/inserting CurrencyAccountBalances for test case file {0} \nWith error:\n{1}", testFile, ex.Message), ex);
                    }
                }
                             
            }
       

            //Assuming the update goes in we now need to check whether the results are expected.

            if(testCase.ExpectedResults ==null||testCase.ExpectedResults.Length==0) throw new ArgumentException("Missing Expected Values");

       

            foreach (ExpectedResult expectedResult in testCase.ExpectedResults)
            {
                IList<CurrencyAccountDTO> returnedAccounts;
                if (expectedResult.GetMethod.MostRecent)
                {
                    returnedAccounts = service.GetMostRecentCurrencyAccountBalances();
                }
                else
                {
                    DateRangeCriteria criteria = expectedResult.GetMethod.DateRangeCriteria;

                    returnedAccounts =
                        service.GetCurrencyAccounts(criteria.BalanceDateFrom, criteria.BalanceDateTo);

                }

                CurrencyAccountDTO[] sortedReturnedAccountDtos =
                    returnedAccounts.OrderBy(x => x.CurrencyIsoCode).ThenBy(x => x.BalanceDate).ToArray();

         //       PrintCurrencyAccounts(sortedReturnedAccountDtos.ToArray());

                AssertBalanceAccountValues(sortedReturnedAccountDtos, expectedResult.CurrencyAccounts,expectedResult.Description);
            }

     


    
            
        }

        private void AssertBalanceAccountValues(CurrencyAccountDTO[] actualAccounts,CurrencyAccount[] expectedAccounts,string description)
        {
            Assert.AreEqual(expectedAccounts.Length,actualAccounts.Length,description+"\nThe number of expected results did not match the number of returned results for the Account Balances");


            for (int i = 0; i < expectedAccounts.Length; i++)
            {
                CurrencyAccount expected = expectedAccounts[i];
                CurrencyAccountDTO actual = actualAccounts[i];
                
                Assert.AreEqual(expected.BalanceDate,actual.BalanceDate,description+"\nBalance Data did not match:");
                Assert.AreEqual(expected.CurrencyIsoCode,actual.CurrencyIsoCode,description+"\nISO Currency did not match:");
                Assert.AreEqual(expected.NominalAmount,actual.NominalAmount,description+"\nNominal Amount did not match:");
                Assert.AreEqual(expected.UpdatedBy,actual.UpdatedBy,description+"\nUpdated by did not match:");
                
               
            }            
         }

        private void PrintCurrencyAccounts(CurrencyAccountDTO[] dtos)
        {
            foreach (CurrencyAccountDTO dto in dtos)
            {
                CurrencyAccount a=new CurrencyAccount(){BalanceDate = dto.BalanceDate,CurrencyIsoCode = dto.CurrencyIsoCode,NominalAmount = dto.NominalAmount,UpdatedBy = dto.UpdatedBy};

                Console.WriteLine(TestXmlTool.Serialize(a));
            }
        }
    }
    
}
