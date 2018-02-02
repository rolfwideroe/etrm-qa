using System;
using System.Configuration;
using System.Reflection;
using NUnit.Framework;

namespace TestSeleniumCRV
{
    public class ApplicationSettings
    {
        /// <summary>
        /// Root application url.
        /// </summary>
        public static string ApplicationUrl = "http://netvs-qa161a02.nortest.bradyplc.com/CRV";


        /// <summary>
        /// Web browser for testing. Available values: Chrome, Firefox, IE.
        /// </summary>
        public static string WebBrowser = "Chrome";

        /// <summary>
        /// Folder path with webdrivers.
        /// </summary>
        public static string WebDriversFolderPath = "WebDrivers";

        /// <summary>
        /// Settings initializer.
        /// </summary>
        static ApplicationSettings()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            Type type = typeof(ApplicationSettings);
            FieldInfo[] settings = type.GetFields();

            foreach (FieldInfo setting in settings)
            {
                string strValue = ConfigurationManager.AppSettings[setting.Name];
                object objValue = Convert.ChangeType(strValue, setting.FieldType);
                setting.SetValue(null, objValue);
            }
        }
    }
}
