using System;
using SalesManager.WebUI.SeleniumTests;
using Selenium;
using NUnit.Framework;

namespace SalesManager.WebUI.SeleniumTests.TestClassesBacked
{
    [TestFixture]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class ProfilesCrud : SeleniumBase
    {
        private const string TestProfileName = "_testProfile";
        private const string TestProfileResolution = "Year by month";
        private const string ProfilesTableName = "profilesTable";
        private const string TestProfileData = "1\n1\n1\n1\n1\n1\n1\n1\n1\n1\n1\n1";

        protected void ExecuteProfilesTest(Action<ISelenium> test)
        {
            ExecuteBackedTest(_testDriver =>
            {
                _testDriver.LoginAsAdmin();
                _testDriver.Sleep(3);
                _testDriver.WaitForElementToApear("logoutLink");
                _testDriver.Open("/Membership/Configuration#ui-tabs-3");
                _testDriver.Sleep(6);
                _testDriver.WaitForElementToApear(ProfilesTableName);
                _testDriver.SortGrid(ProfilesTableName, 0);
                _testDriver.Sleep(3);
                test(_testDriver);
            });
        }

        private void AddTestProfile(ISelenium _testDriver)
        {
            _testDriver.ClickAddNewRecordGridToolbarButton(ProfilesTableName);
            _testDriver.Sleep(3);
            _testDriver.WaitForElementToApear("Profile_Name");
            _testDriver.Type("Profile_Name", TestProfileName);
            _testDriver.Select("Profile_Resolution", "label=" + TestProfileResolution);
            //_testDriver.Select("Profile_Concept_Id", FirstMarginBaseMargin);
            _testDriver.Type("Profile_Data", TestProfileData);

            _testDriver.ConfirmDialogById("createProfileDialog"); //save
            _testDriver.Sleep(2);
        }

        private void DeleteTestProfile(ISelenium _testDriver)
        {
            _testDriver.ClickGridDeleteRowButton(ProfilesTableName, 0);
            _testDriver.Sleep(3);
            _testDriver.Confirm("deleteProfileConfirmDialog");
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CreateNewMarginConcept()
        {
            ExecuteProfilesTest(_testDriver =>
            {
                AddTestProfile(_testDriver);
                _testDriver.Sleep(7);

                _testDriver.ClickGridEditRowButton(ProfilesTableName, 0); //open data to read the input values
                _testDriver.Sleep(5);
                _testDriver.WaitForElementToApear("Profile_Name");

                Assert.AreEqual(TestProfileName, _testDriver.GetValue("Profile_Name"));
                Assert.AreEqual(TestProfileResolution, _testDriver.GetSelectedLabel("Profile_Resolution"));
                Assert.AreEqual(_testDriver.GetEscapedString(TestProfileData), _testDriver.GetEscapedValue("Profile_Data"));

                _testDriver.CancelDialogById("editProfileDialog");
                _testDriver.Sleep(3);

                DeleteTestProfile(_testDriver);
            });
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void EditMarginProfile()
        {
            ExecuteProfilesTest(_testDriver =>
            {
                const string changedName = TestProfileName + "2";
                string changedData = TestProfileData.Replace("1", "2");

                AddTestProfile(_testDriver);
                _testDriver.Sleep(7);
                _testDriver.ClickGridEditRowButton(ProfilesTableName, 0);
                _testDriver.Sleep(10);
                _testDriver.WaitForElementToApear("Profile_Name");

                //change data
                _testDriver.Type("Profile_Name", changedName);
                _testDriver.Type("Profile_Data", changedData);

                _testDriver.ConfirmDialogById("editProfileDialog");
                _testDriver.Sleep(5);

                _testDriver.ClickGridEditRowButton(ProfilesTableName, 0); //open data to read the input values
                _testDriver.Sleep(5);
                _testDriver.WaitForElementToApear("Profile_Name");

                Assert.AreEqual(changedName, _testDriver.GetValue("Profile_Name"));
                Assert.AreEqual(TestProfileResolution, _testDriver.GetSelectedLabel("Profile_Resolution"));
                Assert.AreEqual(_testDriver.GetEscapedString(changedData), _testDriver.GetEscapedValue("Profile_Data"));

                _testDriver.CancelDialogById("editProfileDialog");
                _testDriver.Sleep(5);

                DeleteTestProfile(_testDriver);
            });
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CannotInsertDuplicateNameMargin()
        {
            ExecuteProfilesTest(_testDriver =>
            {
                AddTestProfile(_testDriver);
                _testDriver.Sleep(6);
                //try to add profile with the same name
                _testDriver.ClickAddNewRecordGridToolbarButton(ProfilesTableName);
                _testDriver.Sleep(3);
                _testDriver.WaitForElementToApear("Profile_Name");

                _testDriver.Type("Profile_Name", TestProfileName);
                _testDriver.Select("Profile_Resolution", "label=" + TestProfileResolution);
                _testDriver.Type("Profile_Data", TestProfileData);

                _testDriver.ConfirmDialogById("createProfileDialog");
                _testDriver.Sleep(3);

                Assert.IsTrue(_testDriver.ElementHasClass("#Profile_Name", "error"), "Could insert duplicate profile");

                _testDriver.CancelDialogById("createProfileDialog");
                _testDriver.Sleep(3);

                DeleteTestProfile(_testDriver);
            });
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CannotChangeNameToExisting()
        {
            ExecuteProfilesTest(_testDriver =>
            {
                string changedProdileName = TestProfileName + "2";

                AddTestProfile(_testDriver);
                _testDriver.Sleep(6);
                //try to add profile with the same name
                _testDriver.ClickAddNewRecordGridToolbarButton(ProfilesTableName);
                _testDriver.Sleep(5);
                _testDriver.WaitForElementToApear("Profile_Name");

                _testDriver.Type("Profile_Name", changedProdileName);
                _testDriver.Select("Profile_Resolution", "label=" + TestProfileResolution);
                _testDriver.Type("Profile_Data", TestProfileData);

                _testDriver.ConfirmDialogById("createProfileDialog"); //save second test profile

                _testDriver.Sleep(5);
                _testDriver.ClickGridEditRowButton(ProfilesTableName, 1); //try to edit second profile and alter its name to the first one's
                _testDriver.Sleep(7);
                _testDriver.WaitForElementToApear("Profile_Name");

                _testDriver.Type("Profile_Name", TestProfileName);

                _testDriver.ConfirmDialogById("editProfileDialog");
                _testDriver.Sleep(3);
                Assert.IsTrue(_testDriver.ElementHasClass("#Profile_Name", "error"), "Could insert duplicate profile");

                _testDriver.CancelDialogById("editProfileDialog");
                _testDriver.Sleep(3);

                DeleteTestProfile(_testDriver);
                _testDriver.Sleep(3);
                DeleteTestProfile(_testDriver);
                _testDriver.Sleep(3);
            });
        }
    }
}
