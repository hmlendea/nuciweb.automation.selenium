# Troubleshooting

## Purpose

Documents common issues and their solutions for the NuciWeb.Automation.Selenium library.

## Driver Issues

### Driver Not Found

**Problem:** `WebDriverException: Unable to find executable ...`

**Solution:**

1. **Install geckodriver (Firefox):**
   ```bash
   # Ubuntu/Debian
   sudo apt install firefox-geckodriver

   # Or download from: https://github.com/mozilla/geckodriver/releases
   ```

2. **Install ChromeDriver:**
   ```bash
   # Ubuntu/Debian
   sudo apt install chromium-driver

   # Or download from: https://sites.google.com/a/chromium.org/chromedriver/
   ```

3. **Verify PATH:** Ensure driver executables are in system PATH
   ```bash
   which geckodriver
   which chromedriver
   ```

### Driver Version Mismatch

**Problem:** Driver version doesn't match browser version

**Solution:**

1. **Check versions:**
   ```bash
   geckodriver --version
   chromedriver --version
   ```

2. **Update drivers:** Download latest versions from official sources
3. **Use automatic detection:** Library prefers Firefox when both available

## Browser Issues

### Browser Not Installed

**Problem:** Firefox or Chrome not found

**Solution:**

1. **Install Firefox:**
   ```bash
   # Ubuntu/Debian
   sudo apt install firefox

   # Or download from: https://www.mozilla.org/firefox/
   ```

2. **Install Chrome:**
   ```bash
   # Ubuntu/Debian
   sudo apt install google-chrome-stable

   # Or download from: https://www.google.com/chrome/
   ```

### Headless Mode Issues

**Problem:** Headless mode fails or produces unexpected results

**Solution:**

1. **Try debug mode:** Use `isDebugModeEnabled = true`
2. **Check driver compatibility:** Ensure driver supports headless
3. **Update browser:** Use latest browser version
4. **Check system requirements:** Ensure system supports headless

## Configuration Issues

### Invalid Timeout Values

**Problem:** `ArgumentOutOfRangeException` for timeout values

**Solution:**

```csharp
// Use valid timeout values
IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(
    isDebugModeEnabled: false,
    pageLoadTimeout: 90); // Valid: 1-300 seconds
```

### Debug Mode Conflicts

**Problem:** Debug mode and headless mode conflict

**Solution:**

```csharp
// Debug mode disables headless automatically
IWebDriver driver = WebDriverInitialiser.InitialiseAvailableWebDriver(
    isDebugModeEnabled: true,  // Disables headless
    pageLoadTimeout: 90);
```

## Element Location Issues

### XPath Syntax Errors

**Problem:** Invalid XPath expressions

**Solution:**

1. **Use valid XPath:**
   ```xpath
   //button[@id='submit']
   //div[contains(@class, 'content')]
   //input[@name='username' and @type='text']
   ```

2. **Test XPath manually:** Use browser developer tools
3. **Use simpler selectors:** Prefer ID or class when possible

### Dynamic Content Issues

**Problem:** Elements not found due to dynamic loading

**Solution:**

1. **Add explicit waits:**
   ```csharp
   // Wait for element to be visible
   WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
   wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.XPath("//div")));
   ```

2. **Use `DoesElementExist()`:** Check before interacting
3. **Handle loading states:** Check for loading indicators

## Script Execution Issues

### JavaScript Errors

**Problem:** JavaScript execution fails

**Solution:**

1. **Check script syntax:** Ensure valid JavaScript
2. **Use try/catch:** Handle script errors gracefully
3. **Test in browser:** Verify script works in browser console

### Page Source Issues

**Problem:** `GetPageSource()` returns empty or incomplete

**Solution:**

1. **Wait for page load:** Ensure page is fully loaded
2. **Check network:** Verify all resources loaded
3. **Use debug mode:** See what's happening in browser

## Tab Management Issues

### Tab Not Found

**Problem:** Cannot switch to specific tab

**Solution:**

1. **Get tab IDs:** Use `NewTab()` to get tab identifiers
2. **Verify tab exists:** Check before switching
3. **Handle tab closure:** Be aware of tab lifecycle

### Multiple Tabs

**Problem:** Multiple tabs cause confusion

**Solution:**

1. **Track tab IDs:** Store tab identifiers
2. **Use explicit switching:** Always specify which tab to switch to
3. **Clean up tabs:** Close tabs when done

## Alert Handling Issues

### Alert Not Present

**Problem:** `AcceptAlert()` or `DismissAlert()` fails

**Solution:**

1. **Check for alert:** Use explicit wait
2. **Handle gracefully:** Check if alert exists before handling
3. **Use try/catch:** Handle missing alerts

### Alert Timing

**Problem:** Alerts appear at unexpected times

**Solution:**

1. **Wait for alert:** Use explicit wait for alert presence
2. **Handle multiple alerts:** Be prepared for multiple alerts
3. **Test manually:** Verify alert timing in browser

## Iframe Issues

### Iframe Not Found

**Problem:** `SwitchToIframe()` fails

**Solution:**

1. **Check iframe exists:** Verify iframe is present
2. **Use correct selector:** Ensure iframe selector is unique
3. **Handle nested iframes:** Switch to parent first if needed

## Network Issues

### Connection Timeout

**Problem:** Pages fail to load due to network issues

**Solution:**

1. **Increase timeout:** Use longer `pageLoadTimeout`
2. **Check connectivity:** Verify internet connection
3. **Use proxy:** Configure proxy if needed
4. **Retry:** Implement retry logic

### SSL Certificate Issues

**Problem:** SSL certificate errors

**Solution:**

1. **Accept insecure certs:** Configure driver to accept self-signed certs
2. **Use HTTPS:** Ensure URLs use HTTPS
3. **Update browser:** Use latest browser version

## Performance Issues

### Slow Page Loads

**Problem:** Pages load slowly

**Solution:**

1. **Increase timeout:** Use longer `pageLoadTimeout`
2. **Optimize selectors:** Use efficient XPath/CSS selectors
3. **Cache elements:** Reuse element references
4. **Use headless:** Enable headless mode for speed

### High Memory Usage

**Problem:** High memory consumption

**Solution:**

1. **Limit concurrent instances:** Use one driver per test
2. **Dispose properly:** Always call `driver.Quit()`
3. **Monitor memory:** Watch for memory leaks

## Related Documentation

- [architecture.md](./architecture.md) - Architecture overview
- [configuration.md](./configuration.md) - Configuration options
- [error-handling.md](./error-handling.md) - Error handling patterns
- [quick-start.md](./quick-start.md) - Getting started guide
- [faq.md](./faq.md) - Common questions