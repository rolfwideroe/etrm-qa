using System;
using System.Linq;
using SalesManager.WebUI.SeleniumTests;
using Selenium;
using NUnit.Framework;
using System.Threading;

namespace SalesManager.WebUI.SeleniumTests.TestClassesBacked
{
    [TestFixture]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class MarginsCRUD : SeleniumBase
    {
        private const string TestMarginConceptName = "_test Margin Concept";
        private const string MarginConceptTableName = "marginConceptsTable";

        private const string FirstMarginCurrency =  "EUR";
        private const string FirstMarginBaseMargin = "1";
        private const string FirstMarginCurrencyMargin = "2";
        private const string FirstMarginOptionAbsMargin = "3";
        private const string FirstMarginOptionRelMargin = "4";

        private const string SecondMarginCurrency = "USD";
        private const string SecondMarginBaseMargin = "3";
        private const string SecondMarginCurrencyMargin = "4";

        private const string FirstMarginCurrencyComboBox = "//table[@id='marginsTable']/tbody/tr[1]/td[1]/select";
        private const string FirstMarginBaseMarginInput = "//table[@id='marginsTable']/tbody/tr[1]/td[2]/input";
        private const string FirstMarginCurrencyMarginInput = "//table[@id='marginsTable']/tbody/tr[1]/td[3]/input";
        private const string FirstMarginOptionAbsMarginInput = "//table[@id='marginsTable']/tbody/tr[1]/td[4]/input";
        private const string FirstMarginOptionRelMarginInput = "//table[@id='marginsTable']/tbody/tr[1]/td[5]/input";

        protected void ExecuteMarginsTest(Action<ISelenium> test)
        {
            ExecuteBackedTest(_testDriver =>
            {
                _testDriver.LoginAsAdmin();
                _testDriver.Sleep(3);
                _testDriver.WaitForElementToApear("logoutLink");
                _testDriver.Sleep(3);
                OpenMargins(_testDriver);
                test(_testDriver);
            });
        }

        private void OpenMargins(ISelenium _testDriver)
        {
            _testDriver.Open("/Membership/Configuration#ui-tabs-2");
            _testDriver.Sleep(10);
            _testDriver.WaitForElementToApear(MarginConceptTableName);
            _testDriver.SortGrid(MarginConceptTableName, 0);
        }

        private void AddTestMargin(ISelenium _testDriver, bool zero = false)
        {
            _testDriver.ClickAddNewRecordGridToolbarButton(MarginConceptTableName);
            _testDriver.Sleep(3);
            _testDriver.WaitForElementToApear("Name");

            _testDriver.Type("Name", TestMarginConceptName);
            _testDriver.ClickAndWait("IsDefault");
            
            _testDriver.ClickAddNewRecordGridToolbarButton("marginsTable");
            _testDriver.Select(FirstMarginCurrencyComboBox, "label=" + FirstMarginCurrency);
            _testDriver.Type(FirstMarginBaseMarginInput, zero ? "0" : FirstMarginBaseMargin);
            _testDriver.Type(FirstMarginCurrencyMarginInput, zero ? "0" : FirstMarginCurrencyMargin);
            _testDriver.Type(FirstMarginOptionAbsMarginInput, zero ? "0" : FirstMarginOptionAbsMargin);
            _testDriver.Type(FirstMarginOptionRelMarginInput, zero ? "0" : FirstMarginOptionRelMargin);

            _testDriver.ConfirmDialogById("createMarginConceptDialog"); //save margin concept
        }

        private void DeleteTestMargin(ISelenium _testDriver)
        {
            _testDriver.ClickGridDeleteRowButton(MarginConceptTableName, 0);
            _testDriver.Sleep(3);
            _testDriver.Confirm("deleteMarginConfirmDialog");
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CreateNewMarginConcept()
        {
            ExecuteMarginsTest(_testDriver =>
            {
                AddTestMargin(_testDriver);
                _testDriver.Sleep(3);

                _testDriver.ClickGridEditRowButton(MarginConceptTableName, 0); //open data to read the input values
                _testDriver.Sleep(3);
                _testDriver.WaitForElementToApear("Name");

                Assert.AreEqual(TestMarginConceptName, _testDriver.GetValue("Name"));
                Assert.IsTrue(_testDriver.IsChecked("IsDefault"));
                Assert.AreEqual(FirstMarginCurrency, _testDriver.GetSelectedLabel(FirstMarginCurrencyComboBox));
                Assert.AreEqual(FirstMarginBaseMargin, _testDriver.GetValue(FirstMarginBaseMarginInput));
                Assert.AreEqual(FirstMarginCurrencyMargin, _testDriver.GetValue(FirstMarginCurrencyMarginInput));

                _testDriver.CancelDialogById("editMarginConceptDialog");
                _testDriver.Sleep(3);

                DeleteTestMargin(_testDriver);
            });
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void EditMarginConcept()
        {
            ExecuteMarginsTest(_testDriver =>
            {
                string changedName = TestMarginConceptName + "2";

                AddTestMargin(_testDriver);
                _testDriver.Sleep(3);

                _testDriver.ClickGridEditRowButton(MarginConceptTableName, 0);
                _testDriver.Sleep(3);
                _testDriver.WaitForElementToApear("Name");

                //change data
                _testDriver.Type("Name", changedName);
                _testDriver.Select(FirstMarginCurrencyComboBox, "label=" + SecondMarginCurrency);
                _testDriver.Type(FirstMarginBaseMarginInput, SecondMarginBaseMargin);
                _testDriver.Type(FirstMarginCurrencyMarginInput, SecondMarginCurrencyMargin);

                _testDriver.ConfirmDialogById("editMarginConceptDialog");
                _testDriver.Sleep(3);

                _testDriver.ClickGridEditRowButton(MarginConceptTableName, 0); //open data to read the input values
                _testDriver.Sleep(3);
                _testDriver.WaitForElementToApear("Name");

                Assert.AreEqual(changedName, _testDriver.GetValue("Name"));
                Assert.IsTrue(_testDriver.IsChecked("IsDefault"));
                Assert.AreEqual(SecondMarginCurrency, _testDriver.GetSelectedLabel(FirstMarginCurrencyComboBox));
                Assert.AreEqual(SecondMarginBaseMargin, _testDriver.GetValue(FirstMarginBaseMarginInput));
                Assert.AreEqual(SecondMarginCurrencyMargin, _testDriver.GetValue(FirstMarginCurrencyMarginInput));

                _testDriver.CancelDialogById("editMarginConceptDialog");
                _testDriver.Sleep(3);

                DeleteTestMargin(_testDriver);
            });
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CannotInsertDuplicateNameMargin()
        {
            ExecuteMarginsTest(_testDriver =>
            {
                AddTestMargin(_testDriver);
                _testDriver.Sleep(3);

                _testDriver.ClickAddNewRecordGridToolbarButton(MarginConceptTableName);
                _testDriver.Sleep(3);
                _testDriver.WaitForElementToApear("Name");
                _testDriver.Type("Name", TestMarginConceptName);
                _testDriver.ConfirmDialogById("createMarginConceptDialog");
                _testDriver.Sleep(3);

                Assert.IsTrue(_testDriver.ElementHasClass("#Name", "error"), "Could insert duplicate margin");

                _testDriver.CancelDialogById("createMarginConceptDialog");
                _testDriver.Sleep(3);

                DeleteTestMargin(_testDriver);
            });
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void OnlyOneDefaultConceptAllowed()
        {
            ExecuteMarginsTest(_testDriver =>
            {
                AddTestMargin(_testDriver);
                _testDriver.Sleep(3);

                const string secondTestConceptName = TestMarginConceptName + "2";

                //add another margin concept
                _testDriver.ClickAddNewRecordGridToolbarButton(MarginConceptTableName);
                _testDriver.Sleep(3);
                _testDriver.WaitForElementToApear("Name");
                _testDriver.Type("Name", secondTestConceptName);
                _testDriver.Check("IsDefault");
                _testDriver.ConfirmDialogById("createMarginConceptDialog");
                _testDriver.Sleep(3);

                //fist concept should loose IsDefault property
                _testDriver.ClickGridEditRowButton(MarginConceptTableName, 0);
                _testDriver.Sleep(3);
                _testDriver.WaitForElementToApear("Name");
                Assert.IsFalse(_testDriver.IsChecked("IsDefault"));
                _testDriver.CancelDialogById("editMarginConceptDialog");
                _testDriver.Sleep(3);
                DeleteTestMargin(_testDriver); // delete second 

                DeleteTestMargin(_testDriver); //delete first
            });
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CannotChangeNameToExisting()
        {
            ExecuteMarginsTest(_testDriver =>
            {
                AddTestMargin(_testDriver);
                _testDriver.Sleep(6);

                const string secondTestConceptName = TestMarginConceptName + "2";

                //add another margin concept
                _testDriver.ClickAddNewRecordGridToolbarButton(MarginConceptTableName);
                _testDriver.Sleep(3);
                _testDriver.WaitForElementToApear("Name");
                _testDriver.Type("Name", secondTestConceptName);
                _testDriver.Check("IsDefault");
                _testDriver.ConfirmDialogById("createMarginConceptDialog");
                _testDriver.Sleep(3);

                _testDriver.ClickGridEditRowButton(MarginConceptTableName, 1);
                _testDriver.Sleep(3);
                _testDriver.WaitForElementToApear("Name");
                _testDriver.Type("Name", TestMarginConceptName); //try to change name to the first test margin's
                _testDriver.ConfirmDialogById("editMarginConceptDialog");
                _testDriver.Sleep(3);

                Assert.IsTrue(_testDriver.ElementHasClass("#Name", "error"), "Could update margin to duplicate name");

                _testDriver.CancelDialogById("editMarginConceptDialog");
                _testDriver.Sleep(3);

                DeleteTestMargin(_testDriver);
                DeleteTestMargin(_testDriver);
            });
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void ApplyingPremiumMargin()
        {
            ExecuteBackedTest(_testDriver =>
            {
                double optionAbsMargin = double.Parse(FirstMarginOptionAbsMargin);
                double optionRelMargin = double.Parse(FirstMarginOptionRelMargin);

                _testDriver.LoginAsAdmin();
                AddTestProfile(_testDriver);

                _testDriver.Open("/");

                OpenMargins(_testDriver);
                _testDriver.Sleep(5);
                AddTestMargin(_testDriver, true);
                _testDriver.Sleep(6);

                double premium = GetCurrentPremium(_testDriver, 20000, 3);

                OpenMargins(_testDriver);
                _testDriver.Sleep(5);
                DeleteTestMargin(_testDriver);
                _testDriver.Sleep(3);

                OpenMargins(_testDriver);
                _testDriver.Sleep(5);
                AddTestMargin(_testDriver);
                _testDriver.Sleep(6);

                double premiumWithMargin = GetCurrentPremium(_testDriver, 20000, 3);

                OpenMargins(_testDriver);
                _testDriver.Sleep(5);
                DeleteTestMargin(_testDriver);
                _testDriver.Sleep(3);

                _testDriver.Open("/");
                _testDriver.Sleep(3);
                DeleteTestProfile(_testDriver);
                _testDriver.Sleep(3);
                
                double expectedPremiumWithMargin = (premium * (1 + optionRelMargin / 100)) + optionAbsMargin;
                Assert.AreEqual(expectedPremiumWithMargin, premiumWithMargin, 0.01);
            });
        }

        private const string TestProfileName = "_testProfile";
        private const string TestProfileResolution = "Year by month";
        private const string ProfilesTableName = "profilesTable";
        private const string TestProfileData = "1\n1\n1\n1\n1\n1\n1\n1\n1\n1\n1\n1";

        private void AddTestProfile(ISelenium _testDriver)
        {
            _testDriver.Open("/Membership/Configuration#ui-tabs-3");
            _testDriver.Sleep(6);
            _testDriver.WaitForElementToApear(ProfilesTableName);
            _testDriver.ClickAddNewRecordGridToolbarButton(ProfilesTableName);
            _testDriver.Sleep(3);
            _testDriver.WaitForElementToApear("Profile_Name");
            _testDriver.Type("Profile_Name", TestProfileName);
            _testDriver.Select("Profile_Resolution", "label=" + TestProfileResolution);
            _testDriver.Type("Profile_Data", TestProfileData);

            _testDriver.ConfirmDialogById("createProfileDialog");
            _testDriver.Sleep(6);
        }

        private void DeleteTestProfile(ISelenium _testDriver)
        {
            _testDriver.Open("/Membership/Configuration#ui-tabs-3");
            _testDriver.Sleep(6);
            _testDriver.WaitForElementToApear(ProfilesTableName);
            _testDriver.SortGrid(ProfilesTableName, 0);
            _testDriver.Sleep(4);
            _testDriver.ClickGridDeleteRowButton(ProfilesTableName, 0);
            _testDriver.Sleep(3);
            _testDriver.Confirm("deleteProfileConfirmDialog");
        }

        private double GetCurrentPremium(ISelenium driver, int volume, int cap)
        {
            driver.Open("/");
            driver.Sleep(15);
            driver.ClickAndWait("currentProfile");
            driver.Sleep(5);
            driver.WaitForDialog("allProfilesDialog");

            driver.Click("//img[contains(@alt, '" + TestProfileName + "')]");
            driver.Sleep(5);

            driver.Type("Parameters_Volume", volume.ToString());
            driver.Type("Parameters_Cap", cap.ToString());
            driver.ClickAndWait("Parameters_UseCap");
            driver.ClickAndWait("applyButton");
            driver.Sleep(10);
            string premiumInfo = driver.GetText("capPremiumTitle");
            premiumInfo = new string(premiumInfo.ToCharArray().SkipWhile(c => !Char.IsDigit(c)).Reverse().
                SkipWhile(c => !Char.IsDigit(c)).Reverse().ToArray());
            return double.Parse(premiumInfo);
        }
    }
}
