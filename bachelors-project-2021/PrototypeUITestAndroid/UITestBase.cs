using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium;

namespace PrototypeUITestAndroid;

public class UITestBase
{
    /// <summary>
    ///     The Android driver instance.
    /// </summary>
    private AndroidDriver _driver;

    [OneTimeSetUp]
    public void SetUp()
    {
        try
        {
            // Default Appium server URI when running locally
            var serverUri = new Uri(Environment.GetEnvironmentVariable("APPIUM_HOST") ?? "http://127.0.0.1:4723/");
            var driverOptions = new AppiumOptions()
            {
                AutomationName = AutomationName.AndroidUIAutomator2,
                PlatformName = "Android",
                DeviceName = "Android Emulator",
            };

            driverOptions.AddAdditionalAppiumOption("appPackage", "com.companyname.prototype");
            driverOptions.AddAdditionalAppiumOption("appActivity", "crc64fc63109d11082323.MainActivity");
            driverOptions.AddAdditionalAppiumOption("noReset", true);

            _driver = new AndroidDriver(serverUri, driverOptions, TimeSpan.FromSeconds(180));
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            Console.WriteLine("Driver initialized successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing driver: {ex.Message}");
            throw;
        }
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        if (_driver != null)
        {
            _driver.Dispose();
            Console.WriteLine("Driver disposed successfully.");
        }
        else
        {
            Console.WriteLine("Driver was not initialized.");
        }
    }

    [Test]
    public void TestLoginToTeacherView_CorrectCredentials_Logins()
    {
        var teacherButton = _driver.FindElement(By.XPath("//*[@text='Opettaja']"));
        teacherButton.Click();

        var loginButton = _driver.FindElement(By.XPath("//*[@text='Kirjaudu']"));
        Assert.IsNotNull(loginButton, "New element should be displayed after button click.");

        // Locate the username and password input fields and enter the credentials
        var usernameField = _driver.FindElement(By.XPath("(//android.widget.EditText)[1]"));
        var passwordField = _driver.FindElement(By.XPath("(//android.widget.EditText)[2]"));
        usernameField.SendKeys("opettaja");
        passwordField.SendKeys("opehuone");

        loginButton.Click();

        // Assert: Check that the teacher dashboard is displayed after successful login
        var CreateNewButton = _driver.FindElement(By.XPath("//*[@text='Luo uusi juttunurkka']"));
        Assert.IsNotNull(CreateNewButton, "Teacher Dashboard should be displayed after successful login.");
    }

    [Test]
    public void TestLoginToTeacherView_FaultyCredentials_ThowsError()
    {
        var teacherButton = _driver.FindElement(By.XPath("//*[@text='Opettaja']"));
        teacherButton.Click();

        var loginButton = _driver.FindElement(By.XPath("//*[@text='Kirjaudu']"));
        Assert.IsNotNull(loginButton, "New element should be displayed after button click.");

        // Locate the username and password input fields and enter the wrong credentials
        var usernameField = _driver.FindElement(By.XPath("(//android.widget.EditText)[1]"));
        var passwordField = _driver.FindElement(By.XPath("(//android.widget.EditText)[2]"));
        usernameField.SendKeys("wrongUsername");
        passwordField.SendKeys("wrongPassword");

        loginButton.Click();

        // Assert: Check that login fails and error message is displayed
        var errorText = _driver.FindElement(By.XPath("//*[@text='VIRHE']"));
        Assert.IsNotNull(errorText, "Show error message to user.");

    }
}
