using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace NuciWeb.Automation.Selenium
{
    /// <summary>
    /// Implements the <see cref="IWebProcessor"/> interface using the Selenium WebDriver to perform web automation tasks.
    /// </summary>
    /// <param name="driver">The Selenium WebDriver instance used to interact with the web browser.</param>
    public sealed class SeleniumProcessor(IWebDriver driver) : WebProcessor, IWebProcessor
    {
        readonly IWebDriver driver = driver;

        protected override bool PerformDoesElementExist(string xpath)
        {
            SwitchToTab(CurrentTab);

            try
            {
                IWebElement element = driver.FindElement(By.XPath(xpath));
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected override bool PerformIsCheckboxChecked(string xpath)
            => GetElement(xpath).Selected.Equals(true);

        protected override bool PerformIsElementVisible(string xpath)
        {
            SwitchToTab(CurrentTab);

            try
            {
                IWebElement element = driver.FindElement(By.XPath(xpath));
                return element.Displayed;
            }
            catch
            {
                return false;
            }
        }

        protected override bool PerformIsSelected(string xpath)
            => GetElement(xpath).Selected;

        protected override IEnumerable<string> PerformGetAttribute(string xpath, string attribute)
            => GetElements(xpath).Select(x => x.GetAttribute(attribute));

        protected override IEnumerable<string> PerformGetSelectedText(string xpath)
            => GetSelectElements(xpath).Select(x => x.SelectedOption.Text);

        protected override IEnumerable<string> PerformGetText(string xpath)
            => GetElements(xpath).Select(x => x.Text);

        protected override int PerformGetSelectOptionsCount(string xpath)
            => GetSelectElements(xpath).First().Options.Count;

        protected override string PerformExecuteScript(string script)
        {
            IJavaScriptExecutor scriptExecutor = (IJavaScriptExecutor)driver;

            object result = scriptExecutor.ExecuteScript(script);

            if (result is null)
            {
                return null;
            }

            return (string)result;
        }

        protected override string PerformGetPageSource()
        {
            string oldHandle = driver.CurrentWindowHandle;

            SwitchToTab(CurrentTab);
            string source = driver.PageSource;

            driver.SwitchTo().Window(oldHandle);

            return source;
        }

        protected override string PerformNewTab(string url)
        {
            driver.SwitchTo().Window(driver.WindowHandles[0]);

            // TODO: This is not covered by the retry mechanism
            string newTabScript =
                "var d=document,a=d.createElement('a');" +
                "a.target='_blank';a.href='" + url + "';" +
                "a.innerHTML='new tab';" +
                "d.body.appendChild(a);" +
                "a.click();" +
                "a.parentNode.removeChild(a);";

            IList<string> oldWindowTabs = [.. driver.WindowHandles];

            IJavaScriptExecutor scriptExecutor = (IJavaScriptExecutor)driver;
            scriptExecutor.ExecuteScript(newTabScript);

            IList<string> newWindowTabs = [.. driver.WindowHandles];
            return newWindowTabs.Except(oldWindowTabs).Single();
        }

        protected override void PerformAcceptAlert()
            => GetAlert().Accept();

        protected override void PerformClick(string xpath)
            => GetElement(xpath).Click();

        protected override void PerformCloseTab(string tab)
            => driver.SwitchTo().Window(tab).Close();

        protected override void PerformDismissAlert()
            => GetAlert().Dismiss();

        protected override void PerformGoToUrl(string url, int httpRetries, TimeSpan retryDelay)
        {
            if (driver.Url.Equals(url))
            {
                return;
            }

            string errorSelectorChrome = Select.ByClass("error-code");
            string anythingSelector = Select.ByXPath(@"/html/body/*");

            for (int attempt = 0; attempt < httpRetries; attempt++)
            {
                driver.Navigate().GoToUrl(url);

                for (int i = 0; i < 3; i++)
                {
                    WaitForElementToExist(anythingSelector);
                    if (DoesElementExist(anythingSelector))
                    {
                        break;
                    }

                    driver.Navigate().GoToUrl(url);
                }

                if (!IsAnyElementVisible(errorSelectorChrome))
                {
                    return;
                }

                GoToUrl("about:blank");
                Wait(retryDelay);
            }

            throw new Exception($"Failed to load the requested URL after {httpRetries} attempts");
        }

        protected override void PerformMoveToElement(string xpath)
        {
            IWebElement element = GetElement(xpath);

            Actions actions = new(driver);
            actions.MoveToElement(element);
            actions.Perform();
        }

        protected override void PerformRefresh()
            => driver.Navigate().Refresh();

        protected override void PerformSelectOptionByIndex(string xpath, int index)
            => GetSelectElements(xpath).First().SelectByIndex(index);

        protected override void PerformSelectOptionByText(string xpath, string text)
            => GetSelectElements(xpath).First().SelectByText(text);

        protected override void PerformSelectOptionByValue(string xpath, object value)
            => GetSelectElements(xpath).First().SelectByValue(value.ToString());

        protected override void PerformSetText(string xpath, string text)
        {
            IWebElement element = GetElement(xpath);

            element.Clear();
            element.SendKeys(text);
        }

        protected override void PerformSwitchToIframe(string xpath)
            => driver.SwitchTo().Frame(GetElement(xpath));

        protected override void PerformSwitchToTab(string tab)
            => driver.SwitchTo().Window(tab);

        IWebElement GetElement(string xpath)
        {
            SwitchToTab(CurrentTab);

            DateTime beginTime = DateTime.Now;

            while (DateTime.Now - beginTime < DefaultTimeout)
            {
                try
                {
                    IWebElement element = driver.FindElement(By.XPath(xpath));

                    if (element is not null && element.Displayed)
                    {
                        return element;
                    }
                }
                catch { }
                finally
                {
                    Wait();
                }
            }

            throw new NotFoundException($"No element with the `XPath {xpath}` exists!");
        }

        IList<SelectElement> GetSelectElements(string xpath)
        {
            IList<IWebElement> elements = GetElements(xpath);
            IList<SelectElement> selectElements = [];

            foreach (IWebElement element in elements)
            {
                SelectElement selectElement = new(element);
                selectElements.Add(selectElement);
            }

            return selectElements;
        }

        ReadOnlyCollection<IWebElement> GetElements(string xpath)
        {
            SwitchToTab(CurrentTab);

            DateTime beginTime = DateTime.Now;

            while (DateTime.Now - beginTime < DefaultTimeout)
            {
                try
                {
                    ReadOnlyCollection<IWebElement> elements = driver.FindElements(By.XPath(xpath));

                    if (elements is not null && elements.Count > 0)
                    {
                        return elements;
                    }
                }
                catch { }
                finally
                {
                    Wait();
                }
            }

            throw new NotFoundException($"No elements with the `XPath {xpath}` exist!");
        }

        IAlert GetAlert()
        {
            SwitchToTab(CurrentTab);

            DateTime beginTime = DateTime.Now;

            while (DateTime.Now - beginTime < DefaultTimeout)
            {
                try
                {
                    return driver.SwitchTo().Alert();
                }
                catch { }
                finally
                {
                    Wait();
                }
            }

            return null;
        }
    }
}
