using System;
using NUnit.Framework;
using NuciWeb.Automation.Selenium;
using OpenQA.Selenium;

namespace NuciWeb.Automation.Selenium.UnitTests
{
    [TestFixture]
    public class WebDriverInitialiserTests
    {
        [TestCase(true)]
        [TestCase(false)]
        [Explicit("Requires Firefox and geckodriver.")]
        public void GivenDebugModeSetting_WhenInitialisingFirefox_ThenReturnsDriver(bool isDebugModeEnabled)
        {
            using IWebDriver driver = WebDriverInitialiser.InitialiseFirefoxDriver(isDebugModeEnabled, 90);

            Assert.That(driver, Is.Not.Null);
        }

        [TestCase(true)]
        [TestCase(false)]
        [Explicit("Requires Chrome and chromedriver.")]
        public void GivenDebugModeSetting_WhenInitialisingChrome_ThenReturnsDriver(bool isDebugModeEnabled)
        {
            using IWebDriver driver = WebDriverInitialiser.InitialiseChromeDriver(isDebugModeEnabled, 90);

            Assert.That(driver, Is.Not.Null);
        }

        [Test]
        [Explicit("Requires an installed browser and matching driver.")]
        public void GivenAvailableBrowser_WhenInitialisingAvailableDriver_ThenReturnsDriver()
        {
            using IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(true, 90);

            Assert.That(driver, Is.Not.Null);
        }
    }
}