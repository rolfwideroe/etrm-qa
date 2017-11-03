using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using ElvizTestUtils;

namespace SalesManager.WebUI.SeleniumTests.TestClasses
{
    [TestFixture]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class ChartTests : SeleniumBase
    {
        [TestFixtureSetUp]
        public void Setup()
        {
            List<string> jobs = new List<string>(new String[]
            {
                "NPXSYSALL(Close Price)",
                "NPX4WSM(Close Price)",
                "__WSM_BID_TEST(Bid Price)",
                "__WSM_ASK_TEST(Ask Price)"
            });

            foreach (string jobDescription in jobs)
            {
                int jobId = JobAPI.GetJobsIdByDescription(jobDescription, "Live Price Book Snapshot Job");
                JobAPI.ExecuteAndAssertJob(jobId, 300);
            }

        }
       

        [Test]
        public void _IsLoginPossible()
        {
            ExecuteTest(driver =>
            {
                driver.LoginAsAdmin();
                driver.Logout();
            });
        }

        private const string CurvesTableName = "curvesTable";


        private void OpenAndSortCurveSettings(IWebDriver driver)
        {
            driver.OpenLocal("/Integration/Service");
            driver.WaitForElement(By.Id(CurvesTableName));
            driver.SortGrid(CurvesTableName, 0);
            driver.Sleep(2);
        }

        //private void DeleteTestCurve(IWebDriver driver)
        //{
        //    driver.WaitForElement(By.Id(CurvesTableName));
        //    driver.Sleep(3);
        //    driver.ClickGridDeleteRowButton(CurvesTableName, 0);
        //    driver.Confirm("deleteCurveDialog");
        //}

        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CreateEpadCurve()
        {

            ExecuteTest(driver =>
            {
                driver.LoginAsAdmin();
                OpenAndSortCurveSettings(driver);

                string priceBookDisplayName = "NPX4WSM_Epad_curve";
                string validEpadCurvePriceBookName = "NPX4WSM";
                string validCurveRefAreaName = "SE1";
                string validCurveCurrency = "EUR";
                string priceBasis = "SYLUL";
                string curvePriceType = "Area spread price";

                // Create Bid epad curve setting.
                driver.AddTestCurve(CurvesTableName, priceBookDisplayName, validEpadCurvePriceBookName, validCurveRefAreaName,
                    validCurveCurrency, "Buy", priceBasis, curvePriceType);
                driver.Sleep(2);
              
                OpenAndSortCurveSettings(driver);

                // Update epad curve
                driver.ClickGridRowUsingCellValueActionButton(CurvesTableName, priceBookDisplayName, "ui-icon-arrowreturnthick-1-n");
                driver.WaitForDialog("resultsCurveDialog", Int32.MaxValue.ToString());
                driver.Sleep(2);
                driver.ClickOpenedDialogOkButton();

                // Check that the curve has been successfully updated.
                Assert.IsTrue(driver.FindElement(By.Id("resultsCurveDialog")).InnerHtml().Contains("success"));

            });
        }


        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void CreateBuySellCurves()
        {
            ExecuteTest(driver =>
            {
                driver.LoginAsAdmin();
                driver.Sleep(3);
                OpenAndSortCurveSettings(driver);
                driver.Sleep(3);

                string validBidCurvePriceBookName = "__WSM_BID_TEST";
                string validAskCurvePriceBookName = "__WSM_ASK_TEST";
                string validCurveRefAreaName = "NPS";
                string priceBasis = "NordPool System Price";
                string curvePriceType = "Area price";
                string validCurveCurrency = "EUR";

                // Create Bid curve setting.
                driver.AddTestCurve(CurvesTableName, validBidCurvePriceBookName, validBidCurvePriceBookName, validCurveRefAreaName,
                    validCurveCurrency, "Buy", priceBasis, curvePriceType);
                driver.Sleep(5);

                // Update Bid curve.
                // driver.ClickGridRowActionButton(CurvesTableName, 0, "ui-icon-arrowreturnthick-1-n");
                driver.ClickGridRowUsingCellValueActionButton(CurvesTableName, validBidCurvePriceBookName, "ui-icon-arrowreturnthick-1-n");
                driver.WaitForDialog("resultsCurveDialog", Int32.MaxValue.ToString());

                driver.Sleep(4);
                // Check that the curve has been successfully updated.
                Assert.IsTrue(driver.FindElement(By.Id("resultsCurveDialog")).InnerHtml().Contains("success"));

                driver.ClickOpenedDialogOkButton();
                driver.Sleep(4);

                OpenAndSortCurveSettings(driver);
                driver.Sleep(3);

                // Create Ask curve setting.
                driver.AddTestCurve(CurvesTableName, validAskCurvePriceBookName, validAskCurvePriceBookName, validCurveRefAreaName,
                    validCurveCurrency, "Sell", priceBasis, curvePriceType);
                driver.Sleep(5);

                // Update Ask curve.
                //driver.ClickGridRowActionButton(CurvesTableName, 0, "ui-icon-arrowreturnthick-1-n");
                driver.ClickGridRowUsingCellValueActionButton(CurvesTableName, validAskCurvePriceBookName, "ui-icon-arrowreturnthick-1-n");
                driver.WaitForDialog("resultsCurveDialog", Int32.MaxValue.ToString());
                driver.Sleep(4);
                driver.ClickOpenedDialogOkButton();

                // Check that the curve has been successfully updated.
                Assert.IsTrue(driver.FindElement(By.Id("resultsCurveDialog")).InnerHtml().Contains("success"));

                //driver.OpenLocal("/");
                //driver.Sleep(15);

                //driver.Select(By.Id("Parameters_AreaId"), string.Format("{0} ({1})", validCurveRefAreaName, validBidCurvePriceBookName));
                //driver.Sleep(3);
                //driver.Select(By.Id("Parameters_Curve"), validCurveCurrency);
                //driver.Sleep(3);
                //Assert.IsTrue(driver.FindElement(By.Id("saveButton")).InnerHtml().Contains("Buy"));

                //driver.Select(By.Id("Parameters_AreaId"), string.Format("{0} ({1})", validCurveRefAreaName, validAskCurvePriceBookName));
                //driver.Sleep(3);
                //driver.Select(By.Id("Parameters_Curve"), validCurveCurrency);
                //driver.Sleep(3);
                //Assert.IsTrue(driver.FindElement(By.Id("saveButton")).InnerHtml().Contains("Sell"));

                //OpenAndSortCurveSettings(driver);
                //driver.WaitForElement(By.CssSelector("#" + CurvesTableName + " tbody tr td"));
                //driver.Sleep(5);
                OpenAndSortCurveSettings(driver);
                driver.ClickGridDeleteRowButtonByDisplayName(CurvesTableName, validBidCurvePriceBookName);
                OpenAndSortCurveSettings(driver);
                driver.ClickGridDeleteRowButtonByDisplayName(CurvesTableName, validAskCurvePriceBookName);
         
            });
        }


        [Test]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
        public void DisableBuyElectricityCombo()
        {
            ExecuteTest(driver =>
            {
                driver.LoginAsAdmin();
                Assert.IsFalse(driver.FindElement(By.Id("saveButton")).GetAttribute("class").Contains("ui-button-disabled"));
                driver.Sleep(6);
                driver.FindElement(By.LinkText("Configuration")).Click();
                driver.Sleep(6);
                driver.FindElement(By.XPath("//table[@id='membersTable']/tbody/tr/td[6]/span/span")).Click();
                driver.Sleep(6);
                driver.FindElement(By.Id("MemberData_Password")).Clear();
                driver.FindElement(By.Id("MemberData_Password")).SendKeys("");
                driver.FindElement(By.Id("disableECBuyButton")).Click();
                driver.FindElement(By.XPath("(//button[@type='button'])[3]")).Click();
                driver.Sleep(6);
                driver.FindElement(By.LinkText("Chart")).Click();
                driver.Sleep(6);

                Assert.IsTrue(driver.FindElement(By.Id("saveButton")).GetAttribute("class").Contains("ui-button-disabled"));

                driver.FindElement(By.LinkText("Configuration")).Click();
                driver.Sleep(6);
                driver.FindElement(By.XPath("//table[@id='membersTable']/tbody/tr/td[6]/span/span")).Click();
                driver.Sleep(6);
                driver.FindElement(By.Id("MemberData_Password")).Clear();
                driver.FindElement(By.Id("MemberData_Password")).SendKeys("");
                driver.FindElement(By.Id("enableECBuyButton")).Click();
                driver.FindElement(By.XPath("(//button[@type='button'])[3]")).Click();
                driver.Sleep(6);
                driver.FindElement(By.LinkText("Chart")).Click();
                driver.Sleep(6);

                Assert.IsFalse(driver.FindElement(By.Id("saveButton")).GetAttribute("class").Contains("ui-button-disabled"));

                driver.Logout();
            });
        }


    }
}
