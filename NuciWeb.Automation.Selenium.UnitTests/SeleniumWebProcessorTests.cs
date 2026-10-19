using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Moq;
using NUnit.Framework;
using NuciWeb.Automation.Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace NuciWeb.Automation.Selenium.UnitTests
{
    [TestFixture]
    public class SeleniumWebProcessorTests
    {
        private Mock<IWebDriver> mockDriver = null!;
        private Mock<IWebElement> mockElement = null!;
        private Mock<IWebElement> mockOption = null!;
        private Mock<IAlert> mockAlert = null!;
        private Mock<ITargetLocator> mockTargetLocator = null!;
        private Mock<INavigation> mockNavigation = null!;
        private Mock<IOptions> mockOptions = null!;
        private Mock<IWindow> mockWindow = null!;
        private SeleniumWebProcessor processor = null!;

        [SetUp]
        public void SetUp()
        {
            mockDriver = new Mock<IWebDriver>();
            mockElement = new Mock<IWebElement>();
            mockOption = new Mock<IWebElement>();
            mockAlert = new Mock<IAlert>();
            mockTargetLocator = new Mock<ITargetLocator>();
            mockNavigation = new Mock<INavigation>();
            mockOptions = new Mock<IOptions>();
            mockWindow = new Mock<IWindow>();

            // IJavaScriptExecutor is not a member of IWebDriver, so it has to be
            // added as an additional interface before the mock object is accessed.
            mockDriver.As<IJavaScriptExecutor>().Setup(j => j.ExecuteScript(It.IsAny<string>(), It.IsAny<object[]>()))
                .Returns("result");
            mockDriver.As<IActionExecutor>().Setup(a => a.PerformActions(It.IsAny<IList<ActionSequence>>()));

            mockDriver.Setup(d => d.SwitchTo()).Returns(mockTargetLocator.Object);
            mockDriver.Setup(d => d.Navigate()).Returns(mockNavigation.Object);
            mockDriver.Setup(d => d.Manage()).Returns(mockOptions.Object);

            mockTargetLocator.Setup(t => t.Alert()).Returns(mockAlert.Object);
            mockTargetLocator.Setup(t => t.Frame(It.IsAny<IWebElement>())).Returns(() => mockDriver.Object);
            mockTargetLocator.Setup(t => t.Window(It.IsAny<string>())).Returns(() => mockDriver.Object);

            mockNavigation.Setup(n => n.GoToUrl(It.IsAny<string>()));
            mockNavigation.Setup(n => n.Refresh());

            mockOptions.Setup(o => o.Window).Returns(mockWindow.Object);

            mockDriver.Setup(d => d.FindElement(It.IsAny<By>()))
                .Returns(mockElement.Object);

            mockDriver.Setup(d => d.FindElements(It.IsAny<By>()))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement> { mockElement.Object }));

            mockDriver.Setup(d => d.PageSource).Returns("<html></html>");
            mockDriver.Setup(d => d.Url).Returns("about:blank");
            mockDriver.Setup(d => d.CurrentWindowHandle).Returns("main-window");
            mockDriver.Setup(d => d.WindowHandles).Returns(new ReadOnlyCollection<string>(new List<string> { "main-window", "tab-1" }));

            mockElement.Setup(e => e.Displayed).Returns(true);
            mockElement.Setup(e => e.Enabled).Returns(true);
            mockElement.Setup(e => e.TagName).Returns("select");
            mockElement.Setup(e => e.Selected).Returns(false);
            mockElement.Setup(e => e.Text).Returns("test text");
            mockElement.Setup(e => e.GetAttribute(It.IsAny<string>())).Returns("attr-value");
            mockElement.Setup(e => e.GetAttribute("multiple")).Returns(string.Empty);
            mockElement.Setup(e => e.FindElements(It.IsAny<By>()))
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement> { mockOption.Object }));
            mockElement.Setup(e => e.Clear());
            mockElement.Setup(e => e.SendKeys(It.IsAny<string>()));
            mockElement.Setup(e => e.Click());
            mockOption.Setup(e => e.Selected).Returns(true);
            mockOption.Setup(e => e.Enabled).Returns(true);
            mockOption.Setup(e => e.Text).Returns("Option 1");
            mockOption.Setup(e => e.GetAttribute("index")).Returns("2");
            mockOption.Setup(e => e.GetAttribute("value")).Returns("value2");
            mockOption.Setup(e => e.Click());

            mockAlert.Setup(a => a.Accept());
            mockAlert.Setup(a => a.Dismiss());

            mockWindow.Setup(w => w.Maximize());

            processor = new SeleniumWebProcessor(mockDriver.Object);
            processor.Tabs.Add("main-window");
            processor.SwitchToTab("main-window");
        }

        [Test]
        public void GivenElementExists_WhenClicking_ThenCallsWebDriverClick()
        {
            string xpath = "//button";
            processor.Click(xpath);
            mockElement.Verify(e => e.Click(), Times.Once);
        }

        [Test]
        public void GivenElementExists_WhenCheckingExistence_ThenReturnsTrue()
        {
            string xpath = "//div";
            bool result = processor.DoesElementExist(xpath);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void GivenElementNotFound_WhenCheckingExistence_ThenReturnsFalse()
        {
            mockDriver.Setup(d => d.FindElement(It.IsAny<By>()))
                .Throws(new NoSuchElementException());

            string xpath = "//nonexistent";
            bool result = processor.DoesElementExist(xpath);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void GivenVisibleElement_WhenCheckingVisibility_ThenReturnsTrue()
        {
            string xpath = "//span";
            bool result = processor.IsElementVisible(xpath);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void GivenElementNotFound_WhenCheckingVisibility_ThenReturnsFalse()
        {
            mockDriver.Setup(d => d.FindElement(It.IsAny<By>()))
                .Throws(new NoSuchElementException());

            string xpath = "//nonexistent";
            bool result = processor.IsElementVisible(xpath);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void GivenElementDisappears_WhenCheckingVisibility_ThenReturnsFalse()
        {
            mockDriver.SetupSequence(d => d.FindElement(It.IsAny<By>()))
                .Returns(mockElement.Object)
                .Throws(new NoSuchElementException());

            bool result = processor.IsElementVisible("//disappearing");

            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void GivenHiddenElement_WhenCheckingVisibility_ThenReturnsFalse()
        {
            mockElement.Setup(e => e.Displayed).Returns(false);

            string xpath = "//hidden";
            bool result = processor.IsElementVisible(xpath);
            Assert.That(result, Is.EqualTo(false));
        }

        [Test]
        public void GivenSelectedElement_WhenCheckingSelection_ThenReturnsTrue()
        {
            mockElement.Setup(e => e.Selected).Returns(true);

            string xpath = "//option";
            bool result = processor.IsSelected(xpath);
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public void GivenUnselectedElement_WhenCheckingSelection_ThenReturnsFalse()
        {
            mockElement.Setup(e => e.Selected).Returns(false);

            string xpath = "//option";
            bool result = processor.IsSelected(xpath);
            Assert.That(result, Is.EqualTo(false));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void GivenCheckboxState_WhenUpdatingCheckbox_ThenClicksOnlyWhenStateDiffers(bool isChecked, bool requestedState)
        {
            mockElement.Setup(e => e.Selected).Returns(isChecked);

            processor.UpdateCheckbox("//checkbox", requestedState);

            int expectedClickCount = isChecked == requestedState ? 0 : 1;
            mockElement.Verify(e => e.Click(), Times.Exactly(expectedClickCount));
        }

        [Test]
        public void GivenElementExists_WhenGettingAttribute_ThenReturnsValue()
        {
            mockElement.Setup(e => e.GetAttribute("href")).Returns("https://example.com");

            string xpath = "//a";
            string result = processor.GetAttribute(xpath, "href");
            Assert.That(result, Is.EqualTo("https://example.com"));
        }

        [Test]
        public void GivenMultipleElements_WhenGettingAttributes_ThenReturnsAllValues()
        {
            List<IWebElement> elements = new()
            {
                mockElement.Object,
                mockElement.Object
            };
            mockDriver.Setup(d => d.FindElements(It.IsAny<By>()))
                .Returns(new ReadOnlyCollection<IWebElement>(elements));

            string xpath = "//a";
            IList<string> result = processor.GetAttributeOfMany(xpath, "href");
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void GivenElementExists_WhenGettingText_ThenReturnsText()
        {
            mockElement.Setup(e => e.Text).Returns("Hello World");

            string xpath = "//div";
            string result = processor.GetText(xpath);
            Assert.That(result, Is.EqualTo("Hello World"));
        }

        [Test]
        public void GivenMultipleElements_WhenGettingText_ThenReturnsAllTexts()
        {
            List<IWebElement> elements = new()
            {
                mockElement.Object,
                mockElement.Object
            };
            mockDriver.Setup(d => d.FindElements(It.IsAny<By>()))
                .Returns(new ReadOnlyCollection<IWebElement>(elements));

            string xpath = "//div";
            IList<string> result = processor.GetTextOfMany(xpath);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void GivenSelectExists_WhenGettingSelectedText_ThenReturnsText()
        {
            List<IWebElement> elements = new() { mockElement.Object };
            mockDriver.Setup(d => d.FindElements(It.IsAny<By>()))
                .Returns(new ReadOnlyCollection<IWebElement>(elements));

            string xpath = "//select";
            string result = processor.GetSelectedText(xpath);
            Assert.That(result, Is.EqualTo("Option 1"));
        }

        [Test]
        public void GivenScript_WhenExecuting_ThenCallsExecutor()
        {
            mockDriver.As<IJavaScriptExecutor>().Setup(j => j.ExecuteScript("return 1+1;", It.IsAny<object[]>()))
                .Returns("2");

            processor.ExecuteScript("return 1+1;");

            mockDriver.As<IJavaScriptExecutor>().Verify(j => j.ExecuteScript("return 1+1;", It.IsAny<object[]>()), Times.Once);
        }

        [Test]
        public void GivenVariableExists_WhenGettingValue_ThenReturnsValue()
        {
            mockDriver.As<IJavaScriptExecutor>().Setup(j => j.ExecuteScript("return myVar;", It.IsAny<object[]>()))
                .Returns("myValue");

            string result = processor.GetVariableValue("myVar");

            Assert.That(result, Is.EqualTo("myValue"));
        }

        [Test]
        public void GivenVariableIsNull_WhenGettingValue_ThenReturnsNull()
        {
            mockDriver.As<IJavaScriptExecutor>().Setup(j => j.ExecuteScript("return myVar;", It.IsAny<object[]>()))
                .Returns((object)null!);

            string result = processor.GetVariableValue("myVar");

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GivenSourceExists_WhenGettingPageSource_ThenReturnsSource()
        {
            mockDriver.Setup(d => d.PageSource).Returns("<html><body>Test</body></html>");

            string result = processor.GetPageSource();
            Assert.That(result, Is.EqualTo("<html><body>Test</body></html>"));
        }

        [Test]
        public void GivenDriverCreatesTab_WhenOpeningTab_ThenReturnsHandle()
        {
            mockDriver.As<IJavaScriptExecutor>().Setup(j => j.ExecuteScript(It.IsAny<string>(), It.IsAny<object[]>()));
            mockDriver.SetupSequence(d => d.WindowHandles)
                .Returns(new ReadOnlyCollection<string>(new List<string> { "main-window" }))
                .Returns(new ReadOnlyCollection<string>(new List<string> { "main-window" }))
                .Returns(new ReadOnlyCollection<string>(new List<string> { "main-window", "new-tab" }));

            string result = processor.NewTab("https://example.com");
            Assert.That(result, Is.EqualTo("new-tab"));
        }

        [Test]
        public void GivenAlertExists_WhenAccepting_ThenAcceptsAlert()
        {
            processor.AcceptAlert();
            mockAlert.Verify(a => a.Accept(), Times.Once);
        }

        [Test]
        public void GivenAlertExists_WhenDismissing_ThenDismissesAlert()
        {
            processor.DismissAlert();
            mockAlert.Verify(a => a.Dismiss(), Times.Once);
        }

        [Test]
        public void GivenTabExists_WhenClosingTab_ThenClosesTab()
        {
            processor.Tabs.Add("tab-1");

            processor.CloseTab("tab-1");

            mockTargetLocator.Verify(t => t.Window("tab-1"), Times.Once);
            mockDriver.Verify(d => d.Close(), Times.Once);
        }

        [Test]
        public void GivenUrl_WhenNavigating_ThenNavigates()
        {
            mockDriver.Setup(d => d.FindElement(It.Is<By>(by => by.ToString().Contains("error-code"))))
                .Throws(new NoSuchElementException());

            processor.GoToUrl("https://example.com");
            mockNavigation.Verify(n => n.GoToUrl("https://example.com"), Times.Once);
        }

        [Test]
        public void GivenCurrentUrl_WhenNavigatingToSameUrl_ThenSkipsNavigation()
        {
            processor.GoToUrl("about:blank", 1, TimeSpan.Zero);

            mockNavigation.Verify(n => n.GoToUrl(It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void GivenBodyElementDisappears_WhenNavigating_ThenRetriesBodyWait()
        {
            mockDriver.SetupSequence(d => d.FindElement(It.Is<By>(by => by.ToString().Contains("/html/body/*"))))
                .Returns(mockElement.Object)
                .Throws(new NoSuchElementException())
                .Returns(mockElement.Object)
                .Throws(new NoSuchElementException())
                .Returns(mockElement.Object)
                .Throws(new NoSuchElementException());
            mockDriver.Setup(d => d.FindElement(It.Is<By>(by => by.ToString().Contains("error-code"))))
                .Throws(new NoSuchElementException());

            processor.GoToUrl("https://example.com", 1, TimeSpan.Zero);

            mockNavigation.Verify(n => n.GoToUrl("https://example.com"), Times.Exactly(4));
        }

        [Test]
        public void GivenBrowserErrorPage_WhenNavigating_ThenThrowsAfterRetries()
        {
            Assert.Throws<Exception>(() => processor.GoToUrl("https://example.com", 1, TimeSpan.Zero));
        }

        [Test]
        public void GivenElementExists_WhenMovingToElement_ThenMovesPointer()
        {
            string xpath = "//div";
            processor.MoveToElement(xpath);

            mockElement.Verify(e => e.Click(), Times.Never);
        }

        [Test]
        public void GivenPageLoaded_WhenRefreshing_ThenRefreshesDriver()
        {
            processor.Refresh();
            mockNavigation.Verify(n => n.Refresh(), Times.Once);
        }

        [Test]
        public void GivenSelectExists_WhenSelectingByIndex_ThenSelectsOption()
        {
            List<IWebElement> elements = new() { mockElement.Object };
            mockDriver.Setup(d => d.FindElements(It.IsAny<By>()))
                .Returns(new ReadOnlyCollection<IWebElement>(elements));

            string xpath = "//select";
            processor.SelectOptionByIndex(xpath, 2);
        }

        [Test]
        public void GivenSelectExists_WhenSelectingByText_ThenSelectsOption()
        {
            mockOption.Setup(e => e.Text).Returns("Option 2");
            List<IWebElement> elements = new() { mockElement.Object };
            mockDriver.Setup(d => d.FindElements(It.IsAny<By>()))
                .Returns(new ReadOnlyCollection<IWebElement>(elements));

            string xpath = "//select";
            processor.SelectOptionByText(xpath, "Option 2");
        }

        [Test]
        public void GivenSelectExists_WhenSelectingByValue_ThenSelectsOption()
        {
            List<IWebElement> elements = new() { mockElement.Object };
            mockDriver.Setup(d => d.FindElements(It.IsAny<By>()))
                .Returns(new ReadOnlyCollection<IWebElement>(elements));

            string xpath = "//select";
            processor.SelectOptionByValue(xpath, "value2");
        }

        [Test]
        public void GivenSelectHasOptions_WhenSelectingRandomOption_ThenSelectsAnOption()
        {
            mockOption.Setup(e => e.Selected).Returns(false);
            mockOption.Setup(e => e.GetAttribute("index")).Returns("0");

            processor.SelectRandomOption("//select");

            mockOption.Verify(e => e.Click(), Times.Once);
        }

        [Test]
        public void GivenElementExists_WhenSettingText_ThenClearsAndSendsKeys()
        {
            string xpath = "//input";
            processor.SetText(xpath, "test input");

            mockElement.Verify(e => e.Clear(), Times.Once);
            mockElement.Verify(e => e.SendKeys("test input"), Times.Once);
        }

        [Test]
        public void GivenIframeExists_WhenSwitchingToIframe_ThenSwitchesToFrame()
        {
            string xpath = "//iframe";
            mockTargetLocator.Setup(t => t.Frame(mockElement.Object)).Throws<WebDriverException>();

            Assert.Throws<WebDriverException>(() => processor.SwitchToIframe(xpath));

            mockTargetLocator.Verify(t => t.Frame(mockElement.Object), Times.Once);
        }

        [Test]
        public void GivenElementCannotBeFound_WhenClicking_ThenThrowsNotFoundException()
        {
            mockDriver.SetupSequence(d => d.FindElement(It.IsAny<By>()))
                .Returns((IWebElement)null!)
                .Throws<WebDriverException>();

            Assert.Throws<NotFoundException>(() => processor.Click("//missing"));
        }

        [Test]
        public void GivenNoElementsCanBeFound_WhenGettingText_ThenThrowsNotFoundException()
        {
            mockDriver.SetupSequence(d => d.FindElements(It.IsAny<By>()))
                .Returns((ReadOnlyCollection<IWebElement>)null!)
                .Returns(new ReadOnlyCollection<IWebElement>(new List<IWebElement>()))
                .Throws<WebDriverException>();

            Assert.Throws<NotFoundException>(() => processor.GetText("//missing"));
        }

        [Test]
        public void GivenNoAlertIsPresent_WhenAcceptingAlert_ThenThrowsNullReferenceException()
        {
            mockTargetLocator.Setup(t => t.Alert()).Throws<NoAlertPresentException>();

            Assert.Throws<NullReferenceException>(() => processor.AcceptAlert());
        }

        [Test]
        public void GivenTabExists_WhenSwitchingTabs_ThenSwitchesWindow()
        {
            processor.Tabs.Add("tab-1");

            processor.SwitchToTab("tab-1");

            mockTargetLocator.Verify(t => t.Window("tab-1"), Times.Once);
        }

        [Test]
        public void GivenInvalidTab_WhenSwitchingTabs_ThenThrowsArgumentException()
            => Assert.Throws<ArgumentException>(() => processor.SwitchToTab("invalid-tab"));

        [Test]
        public void GivenInvalidTab_WhenClosingTab_ThenThrowsArgumentException()
            => Assert.Throws<ArgumentException>(() => processor.CloseTab("invalid-tab"));
    }
}