using System;
using Selenium;
using NUnit.Framework;

namespace SalesManager.WebUI.SeleniumTests.TestClassesBacked
{
    [TestFixture]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class MembersCrud : SeleniumBase
    {
        private const string TestUserFirstName = "_testUserFirstName";
        private const string TestUserLastName = "_testUserLastName";
        private const string TestUserLoginName = "_testUserLoginName";
        private const string TestUserPassword = "1";
        private const string TestUserRoleId = "1";

        protected void ExecuteMembersTest(Action<ISelenium> test)
        {
            ExecuteBackedTest(_testDriver =>
            {
                _testDriver.LoginAsAdmin();
                _testDriver.Sleep(3);
                _testDriver.WaitForElementToApear("logoutLink");
                _testDriver.Open("/Membership/Configuration");
                _testDriver.Sleep(10);
                _testDriver.WaitForElementToApear("membersTable");
                _testDriver.SortGrid("membersTable", 0);
                test(_testDriver);
            });
        }

        private void CreateTestMemberWithTwoPortfolios(ISelenium _testDriver)
        {
            Helpers.CreateUser(_testDriver, TestUserFirstName, TestUserLastName, TestUserLoginName, TestUserPassword);
            _testDriver.Sleep(3);

            #region Add Portfolios

            _testDriver.ClickAndWait("portfoliosButton");
            _testDriver.Sleep(3);
            _testDriver.WaitForElementToApear("memberPortfoliosTable");
            _testDriver.ClickAddNewRecordGridToolbarButton("memberPortfoliosTable");
            _testDriver.Sleep(3);
            _testDriver.WaitForElementToApear("portfoliosNotEditableTable");
            _testDriver.ClickGridRow("portfoliosNotEditableTable", 0);
            _testDriver.Sleep(3);
            _testDriver.ConfirmDialogById("addPortfolioDialog");

            _testDriver.ClickAddNewRecordGridToolbarButton("memberPortfoliosTable");
            _testDriver.Sleep(3);
            _testDriver.WaitForElementToApear("portfoliosNotEditableTable");
            _testDriver.ClickGridRow("portfoliosNotEditableTable", 1);
            _testDriver.Sleep(3);
            _testDriver.ConfirmDialogById("addPortfolioDialog");
            _testDriver.Sleep(3);

            _testDriver.ClickGridRowActionButton("memberPortfoliosTable", 1, "ui-icon-check"); //select second portfolio
            _testDriver.Sleep(3);

            _testDriver.ConfirmDialogById("editPortfoliosDialog"); //close portfolios dialog
            _testDriver.Sleep(3);

            #endregion

            _testDriver.ConfirmDialogById("addMemberDialog"); //close Add Member dialog
            _testDriver.Sleep(3);
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void MemberIsCreatedWithProperData()
        {
            ExecuteMembersTest(_testDriver =>
            {
                CreateTestMemberWithTwoPortfolios(_testDriver);

                _testDriver.ClickGridEditRowButton("membersTable", 0); //opens Edit dialog
                _testDriver.Sleep(3);

                Assert.AreEqual(TestUserFirstName, _testDriver.GetValue("MemberData_FirstName"));
                Assert.AreEqual(TestUserLastName, _testDriver.GetValue("MemberData_LastName"));
                Assert.AreEqual(TestUserRoleId, _testDriver.GetValue("MemberData_RoleId"));

                _testDriver.ClickAndWait("portfoliosButton");
                _testDriver.Sleep(3);
                Assert.IsTrue(_testDriver.CheckGridRowHasClass("memberPortfoliosTable", 1, "defaultPortfolio"), "Default portfolio was not set poperly for the created member!"); //check that default portfolio is set to the second

                _testDriver.Refresh(); // Refresh is the easiest way to close all dialogs.
                _testDriver.Sleep(10);
                _testDriver.WaitForElementToApear("membersTable");
                _testDriver.SortGrid("membersTable", 0);
                _testDriver.Sleep(5);
                Helpers.DeleteTestMember(_testDriver);
                _testDriver.Sleep(3);
            });
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CanEditMemberInformation()
        {
            ExecuteMembersTest(_testDriver =>
            {
                CreateTestMemberWithTwoPortfolios(_testDriver);
                _testDriver.Sleep(10);
                _testDriver.ClickGridEditRowButton("membersTable", 0); //opens Edit dialog
                _testDriver.Sleep(3);

                const string changedMemberFirstName = TestUserFirstName + "2";
                const string changedMemberLastName = TestUserLastName + "2";
                var changedMemberRole = new { name = "User", value = "2" };


                _testDriver.Type("MemberData_FirstName", changedMemberFirstName);
                _testDriver.Type("MemberData_LastName", changedMemberLastName);
                _testDriver.Select("MemberData_RoleId", changedMemberRole.name);

                _testDriver.ConfirmDialogById("editMemberDialog"); //close edit dialog 
                _testDriver.Sleep(3);

                _testDriver.ClickGridEditRowButton("membersTable", 0); //opens Edit dialog
                _testDriver.Sleep(3);

                Assert.AreEqual(changedMemberFirstName, _testDriver.GetValue("MemberData_FirstName"));
                Assert.AreEqual(changedMemberLastName, _testDriver.GetValue("MemberData_LastName"));
                Assert.AreEqual(changedMemberRole.value, _testDriver.GetValue("MemberData_RoleId"));

                _testDriver.ClickAndWait("portfoliosButton");
                _testDriver.Sleep(5);
                Assert.IsTrue(_testDriver.CheckGridRowHasClass("memberPortfoliosTable", 1, "defaultPortfolio"), "Default portfolio was not set poperly for the created member!"); //check that default portfolio is set to the second

                _testDriver.Refresh(); // Refresh is the easiest way to close all dialogs.
                _testDriver.Sleep(10);
                _testDriver.WaitForElementToApear("membersTable");
                _testDriver.SortGrid("membersTable", 0);
                _testDriver.Sleep(5);
                Helpers.DeleteTestMember(_testDriver);
            });
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CannotInsertDuplicateMember()
        {
            ExecuteMembersTest(_testDriver =>
            {
                CreateTestMemberWithTwoPortfolios(_testDriver);

                _testDriver.Sleep(3);
                _testDriver.ClickAddNewRecordGridToolbarButton("membersTable");
                _testDriver.Sleep(3);

                string another = "_another";

                //Try to add a member with the same login name
                _testDriver.Type("MemberData_FirstName", TestUserFirstName + another);
                _testDriver.Type("MemberData_LastName", TestUserLastName + another);
                _testDriver.Type("MemberData_LoginName", TestUserLoginName);
                _testDriver.Type("MemberData_Password", TestUserPassword + another);
                _testDriver.Type("MemberData_PasswordConfirmation", TestUserPassword + another);

                _testDriver.ConfirmDialogById("addMemberDialog");
                _testDriver.Sleep(3);

                Assert.IsTrue(_testDriver.ElementHasClass("#MemberData_LoginName", "error"), "Could insert duplicate member");

                _testDriver.CancelDialogById("addMemberDialog");
                _testDriver.Sleep(3);

                Helpers.DeleteTestMember(_testDriver);
                _testDriver.Sleep(3);
            });
        }

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CanResetPassword()
        {
            ExecuteMembersTest(_testDriver =>
            {
                CreateTestMemberWithTwoPortfolios(_testDriver);

                _testDriver.ClickGridEditRowButton("membersTable", 0); //opens Edit dialog
                _testDriver.Sleep(3);

                const string newPassword = TestUserPassword + "2";


                _testDriver.Type("MemberData_Password", newPassword);
                _testDriver.Type("MemberData_PasswordConfirmation", newPassword);

                _testDriver.ConfirmDialogById("editMemberDialog"); //close edit dialog             
                _testDriver.Sleep(3);

                _testDriver.Logout();
                _testDriver.Sleep(3);

                _testDriver.LoginAs(TestUserLoginName, newPassword);

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