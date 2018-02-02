using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium;

namespace TestSeleniumCRV
{
    [TestFixture]
 
    public class LoginAndCreate : SeleniumBase
    {

       
        [Test]
        public void IsLoginPossible()
        {
            ExecuteTest(driver =>
            {
                driver.LoginAsAdmin();
                driver.Logout();
            });
        }

        [Test]
        public void CreateNewUser()
        {
            Guid g = Guid.NewGuid();
            string GuidString = Convert.ToBase64String(g.ToByteArray());
            GuidString = GuidString.Replace("=", "");
            GuidString = GuidString.Replace("+", "");
            string username = GuidString.Remove(7);
        
            ExecuteTest(driver =>
            {
                driver.LoginAsAdmin();
                driver.CreateNewUser(username);
                driver.Logout();
                driver.LoginAsAdmin();
                driver.DeleteCreatedUser(username);

            });
        }

        [Test]
        public void CreateAccountMapping()
        {
            string elvizCompanyName = "API Test Customer";
            string elvizPortfolioName = "API Test Customer";
            string nasdaqAccount = "VIZ1DA";

            ExecuteTest(driver =>
            {
                driver.LoginAsAdmin();
                driver.CreateAccountMappings(elvizCompanyName, elvizPortfolioName, nasdaqAccount);
                driver.Logout();
                driver.LoginAsAdmin();
                driver.DeleteAccountMappings(elvizCompanyName, elvizPortfolioName, nasdaqAccount);
                driver.Logout();

            });
        }

    }
    
}

