using System;
using System.Threading;
using Selenium;
using System.Configuration;

namespace SalesManager.Tests.Selenium
{
    public enum Browser
    {
        InternetExplorer,
        FireFox,
        GoogleChrome
    }

    public class TestDriver
    {
        private readonly DefaultSelenium _selenium;
        private const string PageLoadTimeOut = "120000";
        private const int ActionDefaultSleep = 2000;
        private string APP_URL = ConfigurationManager.AppSettings["ApplicationUrl"];

        public TestDriver(string browserUrl = "/Membership/Login", Browser browser = Browser.GoogleChrome)
        {
            string browserString = "*chrome";
            if (browser == Browser.InternetExplorer)
                browserString = "iexplore";
            else browserString = "*googlechrome ./Chrome-bin/chrome.exe";

            _selenium = new DefaultSelenium("127.0.0.1", 4444, browserString, APP_URL + browserUrl);
            _selenium.Start("commandLineFlags=--disable-web-security");
            //_selenium.WindowMaximize();
        }

        public void Stop()
        {
            _selenium.Stop();
        }

        public void Open(string url)
        {
            //_selenium.Open(APP_URL + url);
            Redirect(url);
            _selenium.WaitForPageToLoad(PageLoadTimeOut);
        }

        public void Redirect(string url)
        {
            _selenium.RunScript(string.Format("location.href='{0}'", APP_URL + url));
        }

        public void Submit(string formLocator)
        {
            _selenium.Submit(formLocator);
        }

        public void Focus(string locator)
        {
            _selenium.Focus(locator);
        }

        public void Type(string locator, string value)
        {
            _selenium.Type(locator, value);
        }

        public void Check(string locator)
        {
            _selenium.Check(locator);
        }

        public void Select(string locator, string value)
        {
            _selenium.Select(locator, value);
        }

        public void WaitForElement(string xpath)
        {
            while (_selenium.GetXpathCount(xpath) == 0)
                Thread.Sleep(ActionDefaultSleep);
        }

        public void Click(string locator)
        {
            _selenium.Click(locator);
            Thread.Sleep(ActionDefaultSleep);
        }

        public void WaitForElementToApear(string id)
        {
            _selenium.WaitForCondition(
                string.Format("selenium.isElementPresent(\"xpath=//*[@id='{0}']\")", id), PageLoadTimeOut);
        }

        public void ClickOpenedDialogOkButton(string dialogIndex = "last()")
        {
            Click(string.Format("//div[@role='dialog'][{0}]//div[@class='ui-dialog-buttonset']/button[1]", dialogIndex));
        }

        public void SortGrid(string tableHtmlId, int columnIndex)
        {
            Click(string.Format("//table[@id='{0}']/thead/tr[1]/th[{1}]", tableHtmlId, columnIndex + 1));
        }

        public void ClickAddNewRecordGridToolbarButton(string htmlTableId)
        {
            Click(string.Format("css=div#{0}_wrapper span.ui-icon-plus", htmlTableId));
        }

        public void PressEnterKey()
        {
            _selenium.KeyPressNative("\\13");
        }

        public void LoginAsAdmin()
        {
            LoginAs("admin", "admin");
        }

        public void ClickGridRow(string tableHtmlId, int index)
        {
            Click(string.Format("//table[@id='{0}']/tbody/tr[{1}]", tableHtmlId, index + 1));
        }

        public void ClickGridRowActionButton(string htmlTableId, int rowIndex, string actionButtonClass)
        {
            Click(string.Format("//table[@id='{0}']/tbody/tr[{1}]//span[@class='ui-button-icon-primary ui-icon {2}']", htmlTableId, rowIndex + 1, actionButtonClass));
        }

        public void ClickGridDeleteRowButton(string htmlTableId, int rowIndex)
        {
            ClickGridRowActionButton(htmlTableId, rowIndex, "ui-icon-trash");
        }

        public void Confirm(string dialogId)
        {
            Click(string.Format("//div[@id='{0}']/..//button[1]", dialogId));
        }

        private void ClickDialogButton(string htmlDialogContentId, int buttinIndex)
        {
            Click(string.Format("//div[@id='{0}'][last()]/..//div[@class='ui-dialog-buttonset']/button[{1}]", htmlDialogContentId, buttinIndex + 1));
        }

        public void ConfirmDialogById(string htmlDialogContentId)
        {
            ClickDialogButton(htmlDialogContentId, 0);
        }

        public void CancelDialogById(string htmlDialogContentId)
        {
            ClickDialogButton(htmlDialogContentId, 1);
        }

        public void ClickGridEditRowButton(string htmlTableId, int rowIndex)
        {
            ClickGridRowActionButton(htmlTableId, rowIndex, "ui-icon-pencil");
        }

        public string GetValue(string htmlElementId)
        {
            return _selenium.GetValue(htmlElementId);
        }

        public string GetEscapedString(string value)
        {
            return value.Replace("\n", "").Replace("\r", "");
        }

        public string GetEscapedValue(string htmlElementId)
        {
            return GetEscapedString(_selenium.GetValue(htmlElementId));
        }

        public string GetSelectedOption(string locator)
        {
            return _selenium.GetSelectedLabel(locator);
        }

        public string GetText(string locator)
        {
            return _selenium.GetText(locator);
        }

        public string GetEval(string javaScript)
        {
            return _selenium.GetEval(javaScript);
        }

        public bool IsChecked(string locator)
        {
            return _selenium.IsChecked(locator);
        }

        public string GetAttribute(string locator)
        {
            return _selenium.GetAttribute(locator);
        }

        public bool ElementHasClass(string jQueryElementSelector, string className)
        {
            return bool.Parse(_selenium.GetEval(string.Format("this.browserbot.getUserWindow().jQuery('{0}').hasClass('{1}')", jQueryElementSelector, className)));
        }

        public bool CheckGridRowHasClass(string htmlElementId, int rowIndex, string className)
        {
            return ElementHasClass(string.Format("#{0} tr:eq({1})", htmlElementId, rowIndex + 1), className);
        }

        public bool ThereIsErrorDialogShown()
        {
            return bool.Parse(_selenium.GetEval(string.Format("this.browserbot.getUserWindow().jQuery(\"div[role='dialog']:last div.ui-dialog-content\").dialog('isOpen')")));
        }

        internal void Logout()
        {
            Click("logoutLink");
        }

        internal void LoginAs(string loginName, string password)
        {
            Open("/Membership/Login");
            Type("LoginName", loginName);
            Type("Password", password);
            Click("//form[@id='login']//button");
        }

        public bool ThereIsDialogWithIdShown(string dialogId)
        {
            return bool.Parse(_selenium.GetEval(string.Format("this.browserbot.getUserWindow().jQuery(\"#{0}\").dialog('isOpen')", dialogId)));
        }

        public void CloseDialogById(string dialogId)
        {
            _selenium.GetEval(string.Format("this.browserbot.getUserWindow().jQuery(\"#{0}\").dialog('close')", dialogId));
        }

        public void WaitForDialog(string dialogId)
        {
            _selenium.WaitForCondition(string.Format("selenium.browserbot.getUserWindow().jQuery(\"#{0}\").dialog('isOpen')", dialogId), PageLoadTimeOut);
        }

        public void Sleep(int sec)
        {
            System.Threading.Thread.Sleep(sec * 1000);
        }

        public void RunScript(string script)
        {
            _selenium.RunScript(script);
        }

        public bool IsElementPresent(string locator)
        {
            return _selenium.IsElementPresent(locator);
        }
    }
}
