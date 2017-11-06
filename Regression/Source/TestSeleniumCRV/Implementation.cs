using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace TestSeleniumCRV
{
    public static class Implementation
    {
        
        public static void Sleep(this IWebDriver driver, int sec)
        {
            Thread.Sleep(sec * 1000);
        }

        public static void Login(this IWebDriver driver, string login, string pwd)
        {
            driver.OpenLocal("/Reports/RepArchive.aspx");
            // maximize window
            driver.Manage().Window.Maximize();
            driver.Sleep(1);
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtLoginName")).SendKeys(login);// login
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtPassword")).SendKeys(pwd); //password
            driver.FindElement(By.Name("ctl00$MainPlaceHolder$ctl04")).Click();
            driver.Sleep(3);
        }

        public static void LoginAsAdmin(this IWebDriver driver)
        {
            driver.Login("admin", "admin");
        }

        public static void Logout(this IWebDriver driver)
        {
            driver.FindElement(By.Id("btn-user")).Click();
            driver.FindElement(By.Id("logout")).Click();
            driver.Sleep(3);
        }

        public static void CreateNewUser(this IWebDriver driver, string login_password)
        {
           
            //  driver.CreateNewUser(dr);
           // driver.Navigate().GoToUrl(baseURL + "/CRV/Reports/MembershipAdministration.aspx?");
            driver.FindElement(By.Id("btn-user")).Click();
            driver.FindElement(By.LinkText("User Management")).Click();
            driver.Sleep(3);
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtLoginName")).Clear();
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtLoginName")).SendKeys(login_password);
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtFirstName")).Clear();
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtFirstName")).SendKeys(login_password);
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtLastName")).Clear();
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtLastName")).SendKeys(login_password);
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtPassword")).Clear();
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtPassword")).SendKeys(login_password);
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtRetypePassword")).Clear();
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtRetypePassword")).SendKeys(login_password);
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_btnCreateUser")).Click();
            driver.Sleep(5);
        }


        public static void DeleteCreatedUser(this IWebDriver driver, string login_password)
        {
            // string baseURL = "http://bervs-qa132a";
            //driver.Navigate().GoToUrl(baseURL + "/CRV/Reports/MembershipAdministration.aspx?");
            driver.FindElement(By.Id("btn-user")).Click();
            driver.FindElement(By.LinkText("User Management")).Click();
            driver.Sleep(3);
           
            int index;
            IWebElement table = driver.FindElement(By.Id("ctl00_MainPlaceHolder_gridStores"));
            IWebElement tbody = table.FindElement(By.TagName("tbody"));
            List<IWebElement> rows = new List<IWebElement>(tbody.FindElements(By.TagName("tr")));
            if (rows != null)
            {
                for (index = 1; index < rows.Count; index++)
                {
                    IWebElement row = tbody.FindElement(By.XPath(string.Format(("//table[@id='ctl00_MainPlaceHolder_gridStores']/tbody/tr[{0}]"), index)));
                    if (row.Text.Contains((login_password)))
                    {
                        string deleteButtonID;
                        if (index < 10)
                            deleteButtonID = "ctl00_MainPlaceHolder_gridStores_ctl0"+index+"_btnDelete";
                        else deleteButtonID = "ctl00_MainPlaceHolder_gridStores_ctl" + index + "_btnDelete"; 
                        
                        driver.FindElement(By.Id(deleteButtonID)).Click();
                        driver.Sleep(3);
                        Assert.IsTrue(Regex.IsMatch(GetAlertsText(driver),
                            "^Are you sure you want to delete this user[\\s\\S]$"));
                        Assert.AreEqual(true, ClickOK_Alert(driver), "Alert was not accepted: ");
                         break;
                    }
                }
            }
            driver.Sleep(1);
        }
        
        private static bool ClickOK_Alert(this IWebDriver driver)
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                alert.Accept();
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
       

        private static string GetAlertsText(this IWebDriver driver)
        {
            string alertText;
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                alertText = alert.Text;
            }
            catch (Exception ex)
            {
                alertText = "Error getting alert text." + ex.Message;
            }
            return alertText;
        }
        public static void CreateAccountMappings(this IWebDriver driver, string elvizCompanyName, string elvizPortfolioName, string nasdaqAccount)
        {
            driver.FindElement(By.Id("btn-user")).Click();
            driver.FindElement(By.LinkText("Account Mapping")).Click();
            driver.Sleep(3);
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtElvizCompanyName")).Clear();
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtElvizCompanyName")).SendKeys(elvizCompanyName);
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtElvizCompanyName")).SendKeys(Keys.Enter);
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtElvizPortfolioName")).Clear();
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtElvizPortfolioName")).SendKeys(elvizPortfolioName);
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtElvizPortfolioName")).SendKeys(Keys.Enter);
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtNordPoolAccountId")).Clear();
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_txtNordPoolAccountId")).SendKeys(nasdaqAccount);
            driver.FindElement(By.Id("ctl00_MainPlaceHolder_btnRegister")).Click();
            driver.Sleep(3);
        }

        public static void DeleteAccountMappings(this IWebDriver driver, string elvizCompanyName, string elvizPortfolioName, string nasdaqAccount)
        {
            
            driver.Navigate().GoToUrl(ApplicationSettings.ApplicationUrl + "/Reports/MembershipAdministration.aspx?");
            driver.FindElement(By.Id("btn-user")).Click();
            driver.FindElement(By.LinkText("Account Mapping")).Click();
            driver.Sleep(3);

            int index;
            IWebElement table = driver.FindElement(By.Id("ctl00_MainPlaceHolder_gridStores"));
            IWebElement tbody = table.FindElement(By.TagName("tbody"));
            List<IWebElement> rows = new List<IWebElement>(tbody.FindElements(By.TagName("tr")));
            if (rows != null)
            {
                for (index = 1; index < rows.Count; index++)
                {
                    IWebElement row = tbody.FindElement(By.XPath(string.Format(("//table[@id='ctl00_MainPlaceHolder_gridStores']/tbody/tr[{0}]"), index)));
                    if ((row.Text.Contains(elvizCompanyName)) && (row.Text.Contains(elvizPortfolioName))&& (row.Text.Contains((nasdaqAccount))))
                    {
                       string deleteButtonID;
                        if (index < 10) 
                            deleteButtonID = "ctl00_MainPlaceHolder_gridStores_ctl0" + index + "_btnDeleteMapping";
                        else deleteButtonID = "ctl00_MainPlaceHolder_gridStores_ctl" + index + "_btnDeleteMapping";

                        driver.FindElement(By.Id(deleteButtonID)).Click();
                        driver.Sleep(3);
                        Assert.IsTrue(Regex.IsMatch(GetAlertsText(driver), "^Are you sure you want to delete this mapping[\\s\\S]$"));
                        Assert.AreEqual(true, ClickOK_Alert(driver), "Alert was not accepted: ");
                        break;
                    }
                }
            }
            driver.Sleep(3);
        }

        public static void OpenLocal(this IWebDriver driver, string url)
        {
            driver.Navigate().GoToUrl(ApplicationSettings.ApplicationUrl + url ?? string.Empty);
        }
    }
}
