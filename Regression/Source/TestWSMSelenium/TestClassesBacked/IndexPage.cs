using System;
using SalesManager.WebUI.SeleniumTests;
using Selenium;
using NUnit.Framework;

namespace SalesManager.WebUI.SeleniumTests.TestClassesBacked
{
    [TestFixture]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class IndexPage : SeleniumBase
    {
        //private const string TestAtcSamplingFrom = "02.02.2011";
        //private const string TestAtcSamplingTo = "03.02.2011";
        //private const string TestAtcQuantity = "1";
        //private const string TransactionsTableName = "transactionsTable";

        protected void ExecuteIndexTest(Action<ISelenium> test)
        {
            ExecuteBackedTest(driver =>
            {
                driver.LoginAsAdmin();
                driver.WaitForElementToApear("logoutLink");
                driver.Open("/");
                test(driver);
            });
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CanViewCurrentProfile()
        {
            ExecuteIndexTest(_testDriver =>
            {
                _testDriver.Sleep(5);
                _testDriver.ClickAndWait("currentProfileDetails");
                _testDriver.Sleep(5);
                Assert.True(_testDriver.ThereIsDialogWithIdShown("currentProfileFull"));
                _testDriver.CloseDialogById("currentProfileFull");
            });
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CanSelectOtherProfile()
        {
            ExecuteIndexTest(_testDriver =>
            {
                _testDriver.Sleep(5);

                _testDriver.ClickAndWait("currentProfile");

                _testDriver.Sleep(5);

                _testDriver.WaitForDialog("allProfilesDialog");

                string secondLoadProfileName = _testDriver.GetText("//div[@id='allProfilesDialog']/div[2]/h3");
                string secondLoadProfileImagePath = _testDriver.GetAttribute("//div[@id='allProfilesDialog']/div[2]/img@src");

                _testDriver.ClickAndWait("//div[@id='allProfilesDialog']/div[2]"); //select 2nd profile         

                _testDriver.Sleep(5);

                Assert.AreEqual(secondLoadProfileName, _testDriver.GetText("currentProfileDetails")); //successfully selected
                Assert.AreEqual(secondLoadProfileImagePath, _testDriver.GetAttribute("currentProfile@src"));
            });
        }
    }
}
