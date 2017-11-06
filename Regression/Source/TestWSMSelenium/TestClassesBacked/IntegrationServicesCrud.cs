//using System;
//using SalesManager.WebUI.SeleniumTests;
//using Selenium;
//using NUnit.Framework;

//namespace SalesManager.WebUI.SeleniumTests.TestClassesBacked
//{
//    [TestFixture]
//    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
//    public class IntegrationServicesCrud : SeleniumBase
//    {
//        private const string TestPriceBookName = "_test Price Book Name";
//        private const string TestReferenceAreaName = "_test Reference Area Name";
//        private const string TestCurrency = "EUR";
//        private const string TestExpiration = "1";
//        private const string TestDaysInPast = "2";
//        private const string CurvesTableName = "curvesTable";

//        protected void ExecuteIntegrationTest(Action<ISelenium> test)
//        {
//            ExecuteBackedTest(driver =>
//            {
//                driver.LoginAsAdmin();
//                driver.WaitForElementToApear("logoutLink");
//                driver.Open("/Integration/Service");
//                driver.Sleep(4);
//                driver.WaitForElementToApear(CurvesTableName);
//                driver.Sleep(6);
//                driver.SortGrid(CurvesTableName, 0);
//                test(driver);
//            });
//        }

//        private void AddTestCurve(ISelenium _testDriver)
//        {
//            _testDriver.Sleep(3);// added

//            _testDriver.WaitForElementToApear(CurvesTableName);

//            _testDriver.Sleep(3);// added

//            _testDriver.ClickAddNewRecordGridToolbarButton(CurvesTableName);

//            _testDriver.Sleep(5); //added

//            _testDriver.WaitForElementToApear("Data_PriceBookName");

//            _testDriver.Type("Data_PriceBookName", TestPriceBookName);
//            _testDriver.Type("Data_ReferenceAreaName", TestReferenceAreaName);
//            _testDriver.Select("Data_Currency_Id", "label=" + TestCurrency);
//            _testDriver.Type("Data_Expiration", TestExpiration);
//            _testDriver.Type("Data_DaysInPast", TestDaysInPast);

//            _testDriver.ConfirmDialogById("createCurveDialog");

//            _testDriver.Sleep(5);
//        }

//        private void DeleteTestCurve(ISelenium _testDriver)
//        {
//            _testDriver.WaitForElementToApear(CurvesTableName);
//            _testDriver.Sleep(3);// added
//            _testDriver.ClickGridDeleteRowButton(CurvesTableName, 0);
//            _testDriver.Confirm("deleteCurveDialog");
//        }

//        [Test]
//        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
//        public void CreateCurveSettings()
//        {
//            ExecuteIntegrationTest(_testDriver =>
//            {
//                AddTestCurve(_testDriver);

//                _testDriver.Sleep(3);
//                _testDriver.ClickGridEditRowButton(CurvesTableName, 0); //open data to read the input values

//                _testDriver.Sleep(10);//added

//                Assert.AreEqual(TestPriceBookName, _testDriver.GetValue("Data_PriceBookName"));
//                Assert.AreEqual(TestReferenceAreaName, _testDriver.GetValue("Data_ReferenceAreaName"));
//                Assert.AreEqual(TestCurrency, _testDriver.GetSelectedLabel("Data_Currency_Id"));
//                Assert.AreEqual(TestExpiration, _testDriver.GetValue("Data_Expiration"));
//                Assert.AreEqual(TestDaysInPast, _testDriver.GetValue("Data_DaysInPast"));

//                _testDriver.CancelDialogById("editCurveDialog");

//                DeleteTestCurve(_testDriver);
//            });
//        }

//        [Test]
//        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
//        public void EditCurveSettings()
//        {
//            ExecuteIntegrationTest(_testDriver =>
//            {
//                AddTestCurve(_testDriver);

//                _testDriver.Sleep(7);//added

//                _testDriver.ClickGridEditRowButton(CurvesTableName, 0); //open data to read the input values

//                _testDriver.Sleep(5);

//                const string changedPriceBookName = TestPriceBookName + "2";
//                const string changedReferenceAreaName = TestReferenceAreaName + "2";
//                const string changedExpiration = TestExpiration + "2";
//                const string changedDayesInPast = TestDaysInPast + "2";
//                const string changedCurrency = "USD";

//                _testDriver.Type("Data_PriceBookName", changedPriceBookName);
//                _testDriver.Type("Data_ReferenceAreaName", changedReferenceAreaName);
//                _testDriver.Select("Data_Currency_Id", "label=" + changedCurrency);
//                _testDriver.Type("Data_Expiration", changedExpiration);
//                _testDriver.Type("Data_DaysInPast", changedDayesInPast);

//                _testDriver.ConfirmDialogById("editCurveDialog");

//                _testDriver.Sleep(5);//added

//                _testDriver.ClickGridEditRowButton(CurvesTableName, 0); //open again to see if the data is changed

//                _testDriver.Sleep(5);//added

//                Assert.AreEqual(changedPriceBookName, _testDriver.GetValue("Data_PriceBookName"));
//                Assert.AreEqual(changedReferenceAreaName, _testDriver.GetValue("Data_ReferenceAreaName"));
//                Assert.AreEqual(changedCurrency, _testDriver.GetSelectedLabel("Data_Currency_Id"));
//                Assert.AreEqual(changedExpiration, _testDriver.GetValue("Data_Expiration"));
//                Assert.AreEqual(changedDayesInPast, _testDriver.GetValue("Data_DaysInPast"));

//                _testDriver.CancelDialogById("editCurveDialog");

//                DeleteTestCurve(_testDriver);
//            });
//        }

//        [Test]
//        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethod]
//        public void CannotInsertDuplicateCurveSettings()
//        {
//            ExecuteIntegrationTest(_testDriver =>
//            {
//                AddTestCurve(_testDriver);

//                _testDriver.Sleep(7);//added

//                _testDriver.ClickAddNewRecordGridToolbarButton(CurvesTableName); //open data to read the input values and try to add the same data

//                _testDriver.Sleep(5);

//                _testDriver.Type("Data_PriceBookName", TestPriceBookName);
//                _testDriver.Type("Data_ReferenceAreaName", TestReferenceAreaName);
//                _testDriver.Select("Data_Currency_Id", "label=" + TestCurrency);
//                _testDriver.Type("Data_Expiration", TestExpiration);
//                _testDriver.Type("Data_DaysInPast", TestDaysInPast);

//                _testDriver.ConfirmDialogById("createCurveDialog");
//                _testDriver.Sleep(3);
//                Assert.IsTrue(_testDriver.ThereIsErrorDialogShown());
//                _testDriver.ClickOpenedDialogOkButton();

//                _testDriver.CancelDialogById("createCurveDialog");

//                DeleteTestCurve(_testDriver);
//                _testDriver.Sleep(3);
//            });
//        }
//    }
//}
