using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NuciWeb.Automation.Selenium;
using OpenQA.Selenium;

namespace NuciWeb.Automation.Selenium.IntegrationTests
{
    [TestFixture]
    public sealed class SeleniumWebProcessorIntegrationTests
    {
        private IWebDriver driver = null!;
        private SeleniumWebProcessor processor = null!;

        [SetUp]
        public void SetUp()
        {
            driver = WebDriverInitialiser.InitialiseChromeDriver(false, 30);
            processor = new SeleniumWebProcessor(driver);
        }

        [TearDown]
        public void TearDown()
        {
            processor?.Dispose();
            driver?.Quit();
        }

        [Test]
        public void GivenADataPage_WhenNavigating_ThenThePageIsLoaded()
        {
            processor.GoToUrl(PageUrl);

            Assert.That(processor.GetText("//h1"), Is.EqualTo("Nucilandia"));
            Assert.That(processor.GetPageSource(), Does.Contain("Nucilandia"));
        }

        [Test]
        public void GivenADataPage_WhenReadingElements_ThenReturnsTextAttributesAndState()
        {
            processor.GoToUrl(PageUrl);

            Assert.Multiple(() =>
            {
                Assert.That(processor.DoesElementExist("//input[@id='name']"), Is.True);
                Assert.That(processor.IsElementVisible("//button[@id='submit']"), Is.True);
                Assert.That(processor.GetAttribute("//a", "href"), Does.Contain("example.com"));
                Assert.That(processor.GetClass("//h1"), Is.EqualTo("title"));
                Assert.That(processor.GetId("//h1"), Is.EqualTo("heading"));
                Assert.That(processor.GetValue("//input[@id='name']"), Is.EqualTo("John Doe"));
                Assert.That(processor.GetTextOfMany("//li"), Has.Count.EqualTo(2));
            });
        }

        [Test]
        public void GivenAnInput_WhenSettingAppendingAndClearingText_ThenTheValueChanges()
        {
            processor.GoToUrl(PageUrl);

            processor.SetText("//input[@id='name']", "Bob Ross");
            Assert.That(processor.GetValue("//input[@id='name']"), Is.EqualTo("Bob Ross"));

            processor.AppendText("//input[@id='name']", " Jr");
            Assert.That(processor.GetValue("//input[@id='name']"), Is.EqualTo(" Jr"));

            processor.ClearText("//input[@id='name']");
            Assert.That(processor.GetValue("//input[@id='name']"), Is.Empty);
        }

        [Test]
        public void GivenAButton_WhenClicking_ThenThePageStateChanges()
        {
            processor.GoToUrl(PageUrl);

            processor.Click("//button[@id='submit']");

            Assert.That(processor.GetText("//p[@id='status']"), Is.EqualTo("Submitted"));
        }

        [Test]
        public void GivenASelect_WhenSelectingByIndexValueAndText_ThenTheSelectionChanges()
        {
            processor.GoToUrl(PageUrl);

            processor.SelectOptionByIndex("//select", 1);
            Assert.That(processor.GetSelectedText("//select"), Is.EqualTo("Scotland"));

            processor.SelectOptionByValue("//select", "nucilandia");
            Assert.That(processor.GetSelectedText("//select"), Is.EqualTo("Nucilandia"));

            processor.SelectOptionByText("//select", "Romania");
            Assert.That(processor.GetSelectedText("//select"), Is.EqualTo("Romania"));
        }

        [Test]
        public void GivenACheckbox_WhenUpdatingItsState_ThenTheStateMatchesTheRequest()
        {
            processor.GoToUrl(PageUrl);

            processor.UpdateCheckbox("//input[@id='enabled']", true);
            Assert.That(processor.IsSelected("//input[@id='enabled']"), Is.True);

            processor.UpdateCheckbox("//input[@id='enabled']", false);
            Assert.That(processor.IsSelected("//input[@id='enabled']"), Is.False);
        }

        [Test]
        public void GivenJavaScript_WhenExecuting_ThenReturnsTheScriptValue()
        {
            processor.GoToUrl(PageUrl);

            processor.ExecuteScript("document.title = 'Astora';");

            Assert.That(driver.Title, Is.EqualTo("Astora"));
        }

        [Test]
        public void GivenAnAlert_WhenAcceptingAndDismissing_ThenTheAlertIsHandled()
        {
            processor.GoToUrl(PageUrl);

            processor.ExecuteScript("window.alert('hello');");
            processor.AcceptAlert();
            processor.ExecuteScript("window.confirm('hello');");
            processor.DismissAlert();

            Assert.That(driver.Url, Does.StartWith("data:text/html"));
        }

        [Test]
        public void GivenMultipleTabs_WhenOpeningAndClosing_ThenTabStateIsMaintained()
        {
            processor.GoToUrl(PageUrl);
            string tab = processor.NewTab(PageUrl);

            Assert.That(processor.Tabs, Has.Count.EqualTo(2));
            Assert.That(processor.CurrentTab, Is.EqualTo(tab));

            processor.CloseTab(tab);

            Assert.That(processor.Tabs, Has.Count.EqualTo(1));
        }

        [Test]
        public void GivenARefresh_WhenRefreshing_ThenThePageRemainsAvailable()
        {
            processor.GoToUrl(PageUrl);
            processor.Refresh();

            Assert.That(processor.IsElementVisible("//h1"), Is.True);
        }

        [Test]
        public void GivenMissingElements_WhenQuerying_ThenTheProcessorReportsAbsence()
        {
            processor.GoToUrl(PageUrl);

            Assert.Multiple(() =>
            {
                Assert.That(processor.DoesElementExist("//missing"), Is.False);
                Assert.That(processor.IsElementVisible("//missing"), Is.False);
                Assert.That(processor.IsAnyElementVisible("//missing", "//also-missing"), Is.False);
                Assert.That(processor.DoAllElementsExist("//h1", "//missing"), Is.False);
            });
        }

        private static string PageUrl =>
            "data:text/html," + Uri.EscapeDataString(
                "<html><head><title>Test</title></head><body>" +
                "<h1 id='heading' class='title'>Nucilandia</h1>" +
                "<input id='name' value='John Doe'>" +
                "<input id='enabled' type='checkbox'>" +
                "<button id='submit' onclick=\"document.getElementById('status').textContent='Submitted'\">Submit</button>" +
                "<p id='status'>Not submitted</p>" +
                "<a href='https://example.com'>Example</a>" +
                "<ul><li>Astora</li><li>Scotland</li></ul>" +
                "<select><option value='nucilandia'>Nucilandia</option><option value='scotland'>Scotland</option><option value='romania'>Romania</option></select>" +
                "<iframe id='frame' srcdoc=\"<p>Iframe content</p>\"></iframe>" +
                "</body></html>");
    }
}
