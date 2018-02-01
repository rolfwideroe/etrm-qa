using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SalesManager.WebUI.SeleniumTests
{
    public static class DriverExtensions
    {
        public static void Sleep(this IWebDriver driver, int sec)
        {
            Thread.Sleep(sec * 1000);
        }

        public static void Login(this IWebDriver driver, string login, string pwd)
        {
            driver.OpenLocal("/Membership/Login");
            driver.Sleep(3);
            driver.FindElement(By.Id("LoginName")).SendKeys(login);
            driver.FindElement(By.Id("Password")).SendKeys(pwd);
            driver.FindElement(By.XPath("//form[@id='login']//button")).Click();
            driver.Sleep(3);
        }

        public static void LoginAsAdmin(this IWebDriver driver)
        {
            driver.Login("admin", "admin");
        }

        public static void Logout(this IWebDriver driver)
        {
            driver.FindElement(By.Id("logoutLink")).Click();
            driver.Sleep(1);
        }

        public static void OpenLocal(this IWebDriver driver, string url)
        {
            driver.Navigate().GoToUrl(ApplicationSettings.ApplicationUrl + url ?? string.Empty);
        }

        public static void WaitForElement(this IWebDriver driver, By by, int? timeout = null)
        {
            driver.Sleep(1);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout ?? 30));
            wait.Until(d => d.FindElement(by));
        }

        public static void ClickAddNewRecordGridToolbarButton(this IWebDriver driver, string htmlTableId)
        {
            driver.FindElement(By.CssSelector(string.Format("div#{0}_wrapper span.ui-icon-plus", htmlTableId))).Click();
            driver.Sleep(2);
        }

        public static void Select(this IWebDriver driver, By selectLocator, string textToSelect)
        {
            IWebElement dropDownListBox =  driver.FindElement(selectLocator);
            SelectElement clickThis = new SelectElement(dropDownListBox);
            clickThis.SelectByText(textToSelect);
        }

        private static void ClickDialogButton(this IWebDriver driver, string htmlDialogContentId, int buttinIndex)
        {
            driver.FindElement(By.XPath(string.Format("//div[@id='{0}'][last()]/..//div[@class='ui-dialog-buttonset']/button[{1}]", htmlDialogContentId, buttinIndex + 1))).Click();
            driver.Sleep(2);
        }

        public static void ConfirmDialogById(this IWebDriver driver, string htmlDialogContentId)
        {
            driver.ClickDialogButton(htmlDialogContentId, 0);
        }

        public static void AddTestCurve(this IWebDriver driver, string curvesTableName, string priceBookDisplayName, string priceBookName, string refAreaName, string curCode, 
                                        string quotesType, string priceBasis, string curvePriceType)
        {


            driver.ClickAddNewRecordGridToolbarButton(curvesTableName);
            driver.Sleep(2);
            //display name
            driver.WaitForElement(By.Id("Data_DisplayName"));
            driver.FindElement(By.Id("Data_DisplayName")).SendKeys(priceBookDisplayName);

            driver.Select(By.Id("Data_QuotesType"), quotesType);
            driver.Sleep(2);

            driver.FindElement(By.Id("Data_ReferenceAreaName")).SendKeys(refAreaName);

            driver.Select(By.Id("Data_CurvePriceType"), curvePriceType);

            driver.Select(By.Id("Data_Currency_Id"), curCode);
            driver.FindElement(By.Id("Data_PriceBookName")).SendKeys(priceBookName);

            driver.Select(By.Id("Data_PriceBasis_Id"), priceBasis);
            //hardcoded
            driver.FindElement(By.Id("Data_Expiration")).SendKeys("3600");
            driver.FindElement(By.Id("Data_DaysInPast")).SendKeys("0");

            driver.FindElement(By.Id("Data_Margin")).SendKeys("1");


            driver.ConfirmDialogById("createCurveDialog");
            driver.Sleep(2);
        }

        public static void ClickGridRowActionButton(this IWebDriver driver, string htmlTableId, int rowIndex, string actionButtonClass)
        {
            driver.FindElement(By.XPath(string.Format("//table[@id='{0}']/tbody/tr[{1}]//span[@class='ui-button-icon-primary ui-icon {2}']", htmlTableId, rowIndex + 1, actionButtonClass))).Click();
            driver.Sleep(2);
        }


        public static void ClickGridRowUsingCellValueActionButton(this IWebDriver driver, string htmlTableId, string priceBookDisplayName, string actionButtonClass)
        {
            //Web table "curvesTable" doesn't have set data-row-index property for now, so will find row index by cell text
            IList<IWebElement> rows = driver.FindElements(By.TagName("tr"));
           
            int matchedRow = 0;
            for (int i=1; i<rows.Count; i++ )
            {
                if (rows[i].Text.Contains(priceBookDisplayName))
                {
                   // Console.WriteLine(rows[i].Text + " index = " + i);
                    matchedRow = i;
                    break;
                }
            }
            driver.FindElement(By.XPath(string.Format("//table[@id='{0}']/tbody/tr[{1}]//span[@class='ui-button-icon-primary ui-icon {2}']", htmlTableId, matchedRow, actionButtonClass))).Click();
            driver.Sleep(2);
        }

        public static void ClickGridDeleteRowButtonByDisplayName(this IWebDriver driver, string htmlTableId, string priceBookDisplayName)
        {
            //Web table "curvesTable" doesn't have set data-row-index property for now, so will find row index by cell text
            IList<IWebElement> rows = driver.FindElements(By.TagName("tr"));

           // int matchedRow = 0;
            for (int i = 1; i < rows.Count; i++)
            {
                if (rows[i].Text.Contains(priceBookDisplayName))
                {
                    driver.FindElement(By.XPath(string.Format("//table[@id='{0}']/tbody/tr[{1}]//span[@class='ui-button-icon-primary ui-icon {2}']", htmlTableId, i, "ui-icon-trash"))).Click();
                   // driver.ClickGridRowActionButton(htmlTableId, i, "ui-icon-trash");
                    driver.Confirm("deleteCurveDialog");
                    break;
                }
            }
            driver.Sleep(2);
        }


        //public static void ClickGridDeleteRowButton(this IWebDriver driver, string htmlTableId, int rowIndex)
        //{
        //    driver.ClickGridRowActionButton(htmlTableId, rowIndex, "ui-icon-trash");
        //}

        public static void Confirm(this IWebDriver driver, string dialogId)
        {
            driver.FindElement(By.XPath(string.Format("//div[@id='{0}']/..//button[1]", dialogId))).Click();
            driver.Sleep(2);
        }

        public static void WaitForDialog(this IWebDriver driver, string dialogId, string timeout = null)
        {
            driver.WaitForElement(By.Id(dialogId), Int32.MaxValue);
        }

        public static string InnerHtml(this IWebElement el)
        {
            return el.GetAttribute("innerHTML");
        }

        public static void SortGrid(this IWebDriver driver, string tableHtmlId, int columnIndex)
        {
            driver.FindElement(By.XPath(string.Format("//table[@id='{0}']/thead/tr[1]/th[{1}]", tableHtmlId, columnIndex + 1))).Click();
            driver.Sleep(2);
        }

        public static void ClickOpenedDialogOkButton(this IWebDriver driver, string dialogIndex = "last()")
        {
            driver.FindElement(By.XPath(string.Format("//div[@role='dialog'][{0}]//div[@class='ui-dialog-buttonset']/button[1]", dialogIndex))).Click();
            driver.Sleep(2);
        }
    }
}
