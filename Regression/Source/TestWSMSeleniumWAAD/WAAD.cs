using System;
using NUnit.Framework;
using SalesManager.Tests.Selenium;

namespace SalesManager.Tests.Selenium.WAAD
{
    public class WAAD
    {
        private TestDriver _testDriver;

        private const string WAADAdminUserName = "wsm-admin@bradyplc.com";
        private const string WAADAdminPassword = "Passw0rd2014!Admin";
        private const string WAADUserUserName = "wsm-user@bradyplc.com";
        private const string WAADUserPassword = "Passw0rd2014!";

        [SetUp]
        public void SetupTest()
        {
            _testDriver = new TestDriver();
            _testDriver.Redirect("/");
        }

        [TearDown]
        public void TeardownTest()
        {
            try
            {
                _testDriver.Stop();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
        }

        private void WaadLogin(string uname, string pwd)
        {
            _testDriver.Sleep(8);
            _testDriver.RunScript(string.Format("$('#UserName').val('{0}');", uname));
            _testDriver.RunScript(string.Format("$('#Password').val('{0}');", pwd));
            _testDriver.Submit("css=form.form-horizontal");
            _testDriver.Sleep(2);//delay
            _testDriver.WaitForElementToApear("logoutLink");
        }

        [Test]
        public void CanLoginAsAdmin()
        {
            WaadLogin(WAADAdminUserName, WAADAdminPassword);
            _testDriver.Sleep(5);

            Assert.IsTrue(_testDriver.IsElementPresent("link=Configuration"));

            _testDriver.Click("logoutLink");
            _testDriver.Sleep(7);

            _testDriver.Click("link=Gå tilbake til applikasjonen");
            _testDriver.Sleep(2);
        }

        [Test]
        public void CanLoginAsUser()
        {
            WaadLogin(WAADUserUserName, WAADUserPassword);
            _testDriver.Sleep(5);

            Assert.IsFalse(_testDriver.IsElementPresent("link=Configuration"));

            _testDriver.Sleep(5);

            _testDriver.Click("logoutLink");
            _testDriver.Sleep(5);

            _testDriver.Click("link=Gå tilbake til applikasjonen");
        }

        [Test]
       // [Repeat(10)]
        public void LoginAsAdminAndUser10Times()
        {
            CanLoginAsAdmin();
            CanLoginAsUser();
        }
    }
}
