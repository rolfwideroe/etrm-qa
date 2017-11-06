using System;
using SalesManager.WebUI.SeleniumTests;
using Selenium;
using NUnit.Framework;

namespace SalesManager.WebUI.SeleniumTests.TestClassesBacked
{
    [TestFixture]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class OffersCrud : SeleniumBase
    {
        private const string TestOfferName = "_testOfferName";
        private const string TestUserFirstName = "_testUserFirstName";
        private const string TestUserLastName = "_testUserLastName";
        private const string TestUserLoginName = "_testUserLoginName";
        private const string TestUserPassword = "1";
        private const string TestOfferVolume = "20";

        protected void ExecuteOffersTest(Action<ISelenium> test)
        {
            ExecuteBackedTest(_testDriver =>
            {
                _testDriver.LoginAsAdmin();
                _testDriver.Sleep(15);
                _testDriver.WaitForElementToApear("logoutLink");
                test(_testDriver);
            });
        }

        private void CreateTestOffer(ISelenium _testDriver)
        {
            _testDriver.Type("Parameters_Volume", TestOfferVolume);
            _testDriver.ClickAndWait("offerButton");
            _testDriver.Sleep(3);
            _testDriver.Type("css=.confirmDialog #offertitle", TestOfferName);
            _testDriver.ClickAndWait("css=.confirmDialog #offervisible");
            _testDriver.ConfirmDialogById("saveOfferDialog");
            _testDriver.Sleep(15);
            _testDriver.ClickOpenedDialogOkButton();
            _testDriver.Sleep(10);
            _testDriver.WaitForElementToApear("offersTable");
        }

        private void DeleteTestOffer(ISelenium _testDriver)
        {
            _testDriver.Open("/Offers/Main");
            _testDriver.Sleep(15);
            _testDriver.ClickAndWait("offersTable_last");
            _testDriver.Sleep(15);
            _testDriver.ClickAndWait("css=tbody > tr:last-child .ui-icon-trash");
            _testDriver.Sleep(5);
            _testDriver.ClickOpenedDialogOkButton();
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CanCreateAndUseOffer()
        {
            ExecuteOffersTest(_testDriver =>
            {
                CreateTestOffer(_testDriver);
                _testDriver.Sleep(15); // This method need a lot of sleeping due fixed price calculating.
                _testDriver.ClickAndWait("offersTable_last");
                _testDriver.Sleep(15);
                _testDriver.ClickAndWait("css=tbody > tr:last-child .ui-icon-play");
                _testDriver.Sleep(20);
                _testDriver.WaitForElementToApear("Parameters_Volume");
                Assert.AreEqual(TestOfferVolume, _testDriver.GetValue("Parameters_Volume"));
                DeleteTestOffer(_testDriver);
            });
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CannotReadPrivateOffer()
        {
            ExecuteOffersTest(_testDriver =>
            {
                CreateTestOffer(_testDriver);
                _testDriver.Open("/Membership/Configuration");
                _testDriver.Sleep(10);
                Helpers.CreateUser(_testDriver, TestUserFirstName, TestUserLastName, TestUserLoginName, TestUserPassword);
                _testDriver.ConfirmDialogById("addMemberDialog");
                _testDriver.Sleep(3);
                _testDriver.ClickAndWait("logoutLink");
                _testDriver.Sleep(5);
                _testDriver.LoginAs(TestUserLoginName, TestUserPassword);
                _testDriver.Sleep(2);
                _testDriver.Open("/Offers/Main");
                _testDriver.Sleep(15);
                _testDriver.ClickAndWait("offersTable_last");
                _testDriver.Sleep(15);
                Assert.IsFalse(_testDriver.GetText("css=tbody > tr:last-child td:first-child").Contains(TestOfferName));
                _testDriver.ClickAndWait("logoutLink");
                _testDriver.Sleep(5);
                _testDriver.LoginAsAdmin();
                _testDriver.Sleep(2);
                DeleteTestOffer(_testDriver);

                _testDriver.Open("/Membership/Configuration");
                _testDriver.Sleep(10);
                _testDriver.WaitForElementToApear("membersTable");
                _testDriver.SortGrid("membersTable", 0);
                _testDriver.Sleep(5);
                Helpers.DeleteTestMember(_testDriver);
            });
        }
    }
}