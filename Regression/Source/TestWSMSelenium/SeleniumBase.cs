using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using Selenium;
using OpenQA.Selenium;

namespace SalesManager.WebUI.SeleniumTests
{
    public class SeleniumBase
    {
        //[Obsolete("ExecuteBackedTest is deprecated, please use ExecuteTest instead.")]
        protected void ExecuteBackedTest(Action<ISelenium> test)
        {
            ExecuteBackedTest(string.Empty, test);
        }

        //[Obsolete("ExecuteBackedTest is deprecated, please use ExecuteTest instead.")]
        protected void ExecuteBackedTest(string localUrl, Action<ISelenium> test)
        {
            ExecuteTest(localUrl, driver =>
            {
                ISelenium rcDriver = new WebDriverBackedSelenium(driver, ApplicationSettings.ApplicationUrl + localUrl ?? string.Empty);
                rcDriver.Start();
                test(rcDriver);
            });
        }

        protected void ExecuteTest(Action<IWebDriver> test)
        {
            ExecuteTest(string.Empty, test);
        }

        protected void ExecuteTest(string localUrl, Action<IWebDriver> test)
        {
            using (IWebDriver driver = CreateRequiredDriver(ApplicationSettings.WebBrowser))
            {
                try
                {
                    driver.Manage().Window.Maximize();
                    driver.OpenLocal(localUrl);
                    driver.Sleep(2);
                    test(driver);
                }
                finally
                {
                    driver.Close();
                    driver.Quit();
                }
            }
        }

        private IWebDriver CreateRequiredDriver(string requiredWebBrowser)
        {
            string browser = requiredWebBrowser.ToLower();

            switch (browser)
            {
                case "chrome":
                    return new ChromeDriver(ApplicationSettings.WebDriversFolderPath);
                case "firefox":
                    return new FirefoxDriver();
                case "ie":
                    return new InternetExplorerDriver(ApplicationSettings.WebDriversFolderPath);
                default:
                    throw new ArgumentOutOfRangeException("Unsupported web-browser: " + requiredWebBrowser);
            }
        }
    }
}
