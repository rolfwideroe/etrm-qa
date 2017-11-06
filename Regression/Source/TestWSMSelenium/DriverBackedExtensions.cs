using Selenium;
using System.Threading;

namespace SalesManager.WebUI.SeleniumTests
{
    public static class DriverBackedExtensions
    {
        private const string PageLoadTimeOut = "600000";
        private const int ActionDefaultSleep = 2000;

        public static void Sleep(this ISelenium driver, int sec)
        {
            Thread.Sleep(sec * 1000);
        }

        public static void Open(this ISelenium driver, string url)
        {
            driver.Redirect(url);
            driver.WaitForPageToLoad(PageLoadTimeOut);
        }

        public static void Redirect(this ISelenium driver, string url)
        {
            driver.RunScript(string.Format("location.href='{0}'", ApplicationSettings.ApplicationUrl + url));
        }

        public static void WaitForElement(this ISelenium driver, string xpath)
        {
            while (driver.GetXpathCount(xpath) == 0)
                Thread.Sleep(ActionDefaultSleep);
        }

        public static void ClickAndWait(this ISelenium driver, string locator)
        {
            driver.Click(locator);
            Thread.Sleep(ActionDefaultSleep);
        }

        public static void WaitForElementToApear(this ISelenium driver, string id)
        {
            driver.WaitForCondition(
                string.Format("selenium.isElementPresent(\"xpath=//*[@id='{0}']\")", id), PageLoadTimeOut);
        }

        public static void ClickOpenedDialogOkButton(this ISelenium driver, string dialogIndex = "last()")
        {
            driver.ClickAndWait(string.Format("//div[@role='dialog'][{0}]//div[@class='ui-dialog-buttonset']/button[1]", dialogIndex));
        }

        public static void SortGrid(this ISelenium driver, string tableHtmlId, int columnIndex)
        {
            driver.ClickAndWait(string.Format("//table[@id='{0}']/thead/tr[1]/th[{1}]", tableHtmlId, columnIndex + 1));
        }

        public static void ClickAddNewRecordGridToolbarButton(this ISelenium driver, string htmlTableId)
        {
            driver.ClickAndWait(string.Format("css=div#{0}_wrapper span.ui-icon-plus", htmlTableId));
        }

        public static void PressEnterKey(this ISelenium driver)
        {
            driver.KeyPressNative("\\13");
        }

        public static void LoginAsAdmin(this ISelenium driver)
        {
            driver.LoginAs("admin", "admin");
        }

        public static void ClickGridRow(this ISelenium driver, string tableHtmlId, int index)
        {
            driver.ClickAndWait(string.Format("//table[@id='{0}']/tbody/tr[{1}]", tableHtmlId, index + 1));
        }

        public static void ClickGridRowActionButton(this ISelenium driver, string htmlTableId, int rowIndex, string actionButtonClass)
        {
            driver.ClickAndWait(string.Format("//table[@id='{0}']/tbody/tr[{1}]//span[@class='ui-button-icon-primary ui-icon {2}']", htmlTableId, rowIndex + 1, actionButtonClass));
        }

        public static void ClickGridDeleteRowButton(this ISelenium driver, string htmlTableId, int rowIndex)
        {
            driver.ClickGridRowActionButton(htmlTableId, rowIndex, "ui-icon-trash");
        }

        public static void Confirm(this ISelenium driver, string dialogId)
        {
            driver.ClickAndWait(string.Format("//div[@id='{0}']/..//button[1]", dialogId));
        }

        private static void ClickDialogButton(this ISelenium driver, string htmlDialogContentId, int buttinIndex)
        {
            driver.ClickAndWait(string.Format("//div[@id='{0}'][last()]/..//div[@class='ui-dialog-buttonset']/button[{1}]", htmlDialogContentId, buttinIndex + 1));
        }

        public static void ConfirmDialogById(this ISelenium driver, string htmlDialogContentId)
        {
            driver.ClickDialogButton(htmlDialogContentId, 0);
        }

        public static void CancelDialogById(this ISelenium driver, string htmlDialogContentId)
        {
            driver.ClickDialogButton(htmlDialogContentId, 1);
        }

        public static void ClickGridEditRowButton(this ISelenium driver, string htmlTableId, int rowIndex)
        {
            driver.ClickGridRowActionButton(htmlTableId, rowIndex, "ui-icon-pencil");
        }

        public static string GetEscapedString(this ISelenium driver, string value)
        {
            return value.Replace("\n", "").Replace("\r", "");
        }

        public static string GetEscapedValue(this ISelenium driver, string htmlElementId)
        {
            return driver.GetEscapedString(driver.GetValue(htmlElementId));
        }

        public static bool ElementHasClass(this ISelenium driver, string jQueryElementSelector, string className)
        {
            return bool.Parse(driver.GetEval(string.Format("jQuery('{0}').hasClass('{1}')", jQueryElementSelector, className)));
        }

        public static bool CheckGridRowHasClass(this ISelenium driver, string htmlElementId, int rowIndex, string className)
        {
            return driver.ElementHasClass(string.Format("#{0} tr:eq({1})", htmlElementId, rowIndex + 1), className);
        }

        public static bool ThereIsErrorDialogShown(this ISelenium driver)
        {
            return bool.Parse(driver.GetEval(string.Format("jQuery(\"div[role='dialog']:last div.ui-dialog-content\").dialog('isOpen')")));
        }

        public static void Logout(this ISelenium driver)
        {
            driver.ClickAndWait("logoutLink");
        }

        public static void LoginAs(this ISelenium driver, string loginName, string password)
        {
            driver.Open("/Membership/Login");
            driver.Type("LoginName", loginName);
            driver.Type("Password", password);
            driver.ClickAndWait("//form[@id='login']//button");
            driver.Sleep(3);
            driver.WaitForElementToApear("logoutLink");
        }

        public static bool ThereIsDialogWithIdShown(this ISelenium driver, string dialogId)
        {
            return bool.Parse(driver.GetEval(string.Format("jQuery(\"#{0}\").dialog('isOpen')", dialogId)));
        }

        public static void CloseDialogById(this ISelenium driver, string dialogId)
        {
            driver.GetEval(string.Format("var q = jQuery('#{0}'); var res = q.dialog('close');", dialogId));
        }

        public static void WaitForDialog(this ISelenium driver, string dialogId)
        {
            driver.WaitForCondition(string.Format("jQuery(\"#{0}\").dialog('isOpen')", dialogId), PageLoadTimeOut);
        }
    }
}
