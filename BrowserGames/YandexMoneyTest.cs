using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace BrowserGames
{
    [TestFixture]
    public class YandexMoneyTest
    {
        private static KeyValuePair<string, string> validCredentials = new KeyValuePair<string, string>("sergey.borovykh", "AccountPassword");
        public ChromeDriver Driver;

        //TODO Add different browsers as parameters
        //TODO Fix current and check for different locales
        //TODO Think about signs of successfull or failed login

        public ChromeDriver GetDriver()
        {
            var options = new ChromeOptions();
            //Options.AddArguments("headless");
            options.AddArguments("window-size=1200x600");
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;

            var driver = new ChromeDriver(chromeDriverService, options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            return driver;
        }

        [SetUp]
        public void Initialization()
        {
            // SetUp must exist to allow TearDown run.
        }

        [Test, Timeout(30000)]
        public void LogInTestPositive()
        {
            Driver = YandexMoneyLoginFromScratch(validCredentials);

            var userNameSpans = Driver.FindElements(By.XPath("//span[@class='user__name']"));
            Assert.IsTrue(userNameSpans.First().Text.Contains(validCredentials.Key), "Login sign was not found. Failed login?");
        }

        [Test, Timeout(30000)]
        [TestCase("sergey.boovykh", "TestAccountPassword")] //wrong login
        [TestCase("sergey.borovykh", "")]                   //empty password
        [TestCase("sergey.boovykh", "testaccountpassword")] //wrong password
        public void LogInTestNegative(string login, string passwd)
        {
            var credential = new KeyValuePair<string, string>(login, passwd);
            Driver = YandexMoneyLoginFromScratch(credential);

            var enterAccountButton = Driver.FindElementsByClassName("passport-Button");
            Assert.IsTrue(enterAccountButton.Any(), $"EnterAccount button was not found.");

            //Something strange with timeouts.
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
            var errorFormDivShown = wait.Until(drv => drv.FindElement(By.XPath("//div[@class='passport-Domik-Form-Error']")).Enabled);
            Assert.IsTrue(errorFormDivShown, $"Error message div was not found (bool).");
        }

        [Test, Timeout(30000)]
        public void LogOutTest()
        {
            Driver = YandexMoneyLoginFromScratch(validCredentials);

            throw new NotImplementedException();
        }


        private ChromeDriver YandexMoneyLoginFromScratch(KeyValuePair<string, string> credentials)
        {
            var driver = GetDriver();
            driver.Navigate().GoToUrl("https://money.yandex.ru");
            var button = driver.FindElement(By.XPath("//a[@title= 'Log in']"));
            button.Click();

            driver.FindElementByName("login").SendKeys(credentials.Key);
            driver.FindElementByName("passwd").SendKeys(credentials.Value);
            driver.FindElementByClassName("passport-Button").Click();
            return driver;
        }

        [TearDown]
        public void ClearBrowser()
        {
            //chromeDriver.GetScreenshot().SaveAsFile(@"D:\\GitHub\\BrowserGames\\BrowserGames\\screen.png", ScreenshotImageFormat.Jpeg);

            Driver.Quit();
        }

    }
}
