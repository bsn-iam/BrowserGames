using System;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace BrowserGames
{
    [TestFixture]
    public class YandexMoneyTest
    {
        private const string validLogin = "sergey.borovykh";
        private const string validPassword = "YaMGnKz";
        public static ChromeDriver Driver;

        //TODO Add support for different browsers
        //TODO Check for different locales
        //TODO Think about signs of successfull or failed login
        //TODO Think about excessive Asserts with error info

        [Test, Timeout(30000)]
        public void LogInTestPositive()
        {
            Driver = YandexMoneyLoginFromScratch(validLogin, validPassword);

            var userNameSpans = Driver.FindElements(By.XPath("//span[@class='user__name']"));
            Assert.IsTrue(userNameSpans.First().Text.Contains(validLogin), "Login sign was not found. Failed login?");
        }

        [Test, Timeout(30000)]
        [TestCase("wrongLogin", validPassword)]
        [TestCase(validLogin, "")]             
        [TestCase(validLogin, "wrongPassword")]
        public void LogInTestNegative(string login, string passwd)
        {
            Driver = YandexMoneyLoginFromScratch(login, passwd);

            var enterAccountButton = Driver.FindElementsByClassName("passport-Button");
            Assert.IsTrue(enterAccountButton.Any(), $"EnterAccount button was not found.");

            waitUntil("//div[@class='passport-Domik-Form-Error']");
        }

        [Test, Timeout(30000)]
        public void LogOutTest()
        {
            Driver = YandexMoneyLoginFromScratch(validLogin, validPassword);

            var userNameSpans = Driver.FindElements(By.XPath("//span[@class='user__name']"));
            userNameSpans.First().Click();
            waitUntil("//div[@class='popup__content']");

            var exitButtons = Driver.FindElementsByXPath("//*[contains(@class, 'user__logout')]");
            Assert.IsTrue(exitButtons.Count == 1 && exitButtons.First().Displayed, "Logout button is not found.");

            exitButtons.First().Click();
            waitUntil("//a[@title= 'Log in']");

            var loginButtons = Driver.FindElementsByXPath("//a[@title= 'Log in']");
            Assert.IsTrue(loginButtons.Count == 1 && loginButtons.First().Displayed, "Login button is not found.");
        }


        private void waitUntil(string xpath)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(5));
            var errorFormDivShown = wait.Until(drv => drv.FindElement(By.XPath(xpath)).Enabled);
        }

        private ChromeDriver YandexMoneyLoginFromScratch(string login, string passwd)
        {
            var driver = GetDriver();
            driver.Navigate().GoToUrl("https://money.yandex.ru");
            var button = driver.FindElement(By.XPath("//a[@title= 'Log in']"));
            button.Click();

            driver.FindElementByName("login").SendKeys(login);
            driver.FindElementByName("passwd").SendKeys(passwd);
            driver.FindElementByClassName("passport-Button").Click();
            return driver;
        }

        public ChromeDriver GetDriver()
        {
            var options = new ChromeOptions();
            //options.AddArguments("headless");
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

        [TearDown]
        public void ClearBrowser()
        {
            Driver.Quit();
        }

    }
}
