using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BrowserGames
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            // Initialize the Chrome Driver
            using (var driver = new ChromeDriver())
            {
                // Go to the home page
                driver.Navigate().GoToUrl("http://testing-ground.scraping.pro/login");

                // Get the page elements
                var userNameField = driver.FindElementById("usr");
                var userPasswordField = driver.FindElementById("pwd");
                var loginButton = driver.FindElementByXPath("//input[@value='Login']");

                // Type user name and password
                userNameField.SendKeys("admin");
                userPasswordField.SendKeys("12345");

                // and click the login button
                loginButton.Click();

                // Extract the text and save it into result.txt
                var result = driver.FindElementByXPath("//div[@id='case_login']/h3").Text;
                
                File.WriteAllText("result.txt", result);

                // Take a screenshot and save it into screen.png
                driver.GetScreenshot().SaveAsFile(@"D:\\GitHub\\BrowserGames\\BrowserGames\\screen.png", ScreenshotImageFormat.Jpeg);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            ChromeOptions options = new ChromeOptions();
            options.AddArguments("headless");
            options.AddArguments("window-size=1200x600");
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;

            var driver = new ChromeDriver(chromeDriverService, options);

            driver.Navigate().GoToUrl("http://russianaicup.ru/profile/bsn-iam");

            var allLinks = driver.FindElements(By.TagName("a"));

            var place = driver.FindElement(By.CssSelector("a[href*='standings-row-for-place']"));

            placeValue.Text = place.Text;

            driver.Close();
            driver.Quit();
        }
    }
}
