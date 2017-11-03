using System;
using SalesManager.WebUI.SeleniumTests;
using Selenium;
using NUnit.Framework;

namespace SalesManager.WebUI.SeleniumTests.TestClassesBacked
{
    // Temporary disable ATC tests [TestFixture]
    //[Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class AtcCrud : SeleniumBase
    {
        private const string TestAtcSamplingFrom = "02.02.2011";
        private const string TestAtcSamplingTo = "03.02.2011";
        private const string TestAtcQuantity = "1";
        private const string TransactionsTableName = "transactionsTable";

        protected void ExecuteAtcTest(Action<ISelenium> test)
        {
            ExecuteBackedTest(_testDriver =>
            {
                _testDriver.LoginAsAdmin();

                _testDriver.Open("/Transactions/Main");

                _testDriver.WaitForElementToApear(TransactionsTableName);
                _testDriver.Sleep(5);
                _testDriver.SortGrid(TransactionsTableName, 0);

                test(_testDriver);
            });
        }

        private void AddTestAtc(ISelenium _testDriver)
        {
            _testDriver.ClickAndWait("actionNewAtc");
            _testDriver.WaitForElementToApear("Transaction_SamplingFrom");
            _testDriver.Type("Transaction_SamplingFrom", TestAtcSamplingFrom);
            _testDriver.Type("Transaction_SamplingTo", TestAtcSamplingTo);
            _testDriver.Type("Transaction_Quantity", TestAtcQuantity);

            _testDriver.ConfirmDialogById("createAtcDialog");

            _testDriver.ClickOpenedDialogOkButton(); //click ok on operation result dialog
        }

        private void DeleteTestAtc(ISelenium _testDriver)
        {
            _testDriver.ClickGridDeleteRowButton(TransactionsTableName, 0);
            _testDriver.Confirm("confirmDeleteAtcDialog");
        }

        // Temporary disable ATC tests [Test]
        //[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CanCreateAtc()
        {
            ExecuteAtcTest(_testDriver =>
            {
                AddTestAtc(_testDriver);

                _testDriver.ClickGridEditRowButton(TransactionsTableName, 0); //open data to read the input values
                _testDriver.WaitForElementToApear("Transaction_SamplingFrom");

                Assert.AreEqual(TestAtcSamplingFrom, _testDriver.GetValue("Transaction_SamplingFrom"));
                Assert.AreEqual(TestAtcSamplingTo, _testDriver.GetValue("Transaction_SamplingTo"));
                Assert.AreEqual(decimal.Parse(TestAtcQuantity), decimal.Parse(_testDriver.GetValue("Transaction_Quantity")));

                _testDriver.CancelDialogById("editAtcDialog");

                DeleteTestAtc(_testDriver);
            });
        }

        // Temporary disable ATC tests [Test]
        //[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CanEditAtc()
        {
            ExecuteAtcTest(_testDriver =>
            {
                AddTestAtc(_testDriver);

                _testDriver.ClickGridEditRowButton(TransactionsTableName, 0);
                _testDriver.WaitForElementToApear("Transaction_SamplingFrom");

                const string changedSamplingFrom = "04.02.2011";
                const string changedSamplingTo = "05.02.2011";
                const string changedQuantity = "2";

                _testDriver.Type("Transaction_SamplingFrom", changedSamplingFrom);
                _testDriver.Type("Transaction_SamplingTo", changedSamplingTo);
                _testDriver.Type("Transaction_Quantity", changedQuantity);

                _testDriver.ConfirmDialogById("editAtcDialog");

                _testDriver.ClickGridEditRowButton(TransactionsTableName, 0);
                _testDriver.WaitForElementToApear("Transaction_SamplingFrom");

                Assert.AreEqual(changedSamplingFrom, _testDriver.GetValue("Transaction_SamplingFrom"));
                Assert.AreEqual(changedSamplingTo, _testDriver.GetValue("Transaction_SamplingTo"));
                Assert.AreEqual(decimal.Parse(changedQuantity), decimal.Parse(_testDriver.GetValue("Transaction_Quantity")));

                _testDriver.CancelDialogById("editAtcDialog");

                DeleteTestAtc(_testDriver);
            });
        }

        // Temporary disable ATC tests [Test]
        //[Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CannotInsertInvalidData()
        {
            ExecuteAtcTest(_testDriver =>
            {
                _testDriver.ClickAndWait("actionNewAtc");
                _testDriver.WaitForElementToApear("Transaction_SamplingFrom");

                //exchange sampiling from with sampling to - incorrect interval
                _testDriver.Type("Transaction_SamplingFrom", TestAtcSamplingTo);
                _testDriver.Type("Transaction_SamplingTo", TestAtcSamplingFrom);
                _testDriver.Type("Transaction_Quantity", TestAtcQuantity);

                _testDriver.ConfirmDialogById("createAtcDialog");

                Assert.IsTrue(_testDriver.ElementHasClass("#Transaction_SamplingFrom", "error"));

                _testDriver.CancelDialogById("createAtcDialog");
            });
        }
    }
}
