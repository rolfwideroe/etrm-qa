using Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesManager.WebUI.SeleniumTests;

namespace SalesManager.WebUI.SeleniumTests.TestClassesBacked
{
    public class Helpers
    {
        public static void CreateUser(ISelenium _testDriver, string TestUserFirstName, string TestUserLastName, string TestUserLoginName, string TestUserPassword)
        {
            _testDriver.ClickAddNewRecordGridToolbarButton("membersTable");
            _testDriver.Sleep(3);
            _testDriver.WaitForElementToApear("MemberData_FirstName");

            #region Fill User Fields
            _testDriver.Type("MemberData_FirstName", TestUserFirstName);
            _testDriver.Type("MemberData_LastName", TestUserLastName);
            _testDriver.Type("MemberData_LoginName", TestUserLoginName);
            _testDriver.Type("MemberData_Password", TestUserPassword);
            _testDriver.Type("MemberData_PasswordConfirmation", TestUserPassword);
            #endregion
        }

        public static void DeleteTestMember(ISelenium _testDriver)
        {
            _testDriver.ClickGridDeleteRowButton("membersTable", 0);
            _testDriver.Sleep(3);
            _testDriver.Confirm("deleteMemberConfirmDialog");
        }
    }
}
