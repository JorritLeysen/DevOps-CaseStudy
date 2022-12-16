// Nodige pakketten om de code te doen werken
using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Text.Json;

// Project waarin we werken
namespace Twitter_webscraper
{
    // Klasse job voor de uitvoer naar bestanden
    public class Tweet
    {
        // Constructor
        public Tweet(string name, string likes, string retweets, string comments, DateTime releaseDate, string link)
        {
            Name = name;
            Likes = likes;
            Retweets = retweets;
            Comments = comments;
            ReleaseDate = releaseDate;
            Link = link;
        }
        // Eigenschappen
        public string Name { get; set; }
        public string Likes { get; set; }
        public string Retweets { get; set; }
        public string Comments { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Link { get; set; }
    }
    // Twitter webscraper
    class Twitter
    {        
        static void Main(string[] args)
        {
            // Chrome als browser kiezen
            IWebDriver driver = new ChromeDriver();

            // Input van zoekterm
            Console.Write("Enter a Twitter searchterm you want information of: ");
            string searchTerm = Console.ReadLine();
            Console.WriteLine();

            // Twitter
            // Counter voor console uitvoer
            int tcount = 1;
            // Naar URL gaan
            string url = "https://twitter.com/home";
            driver.Navigate().GoToUrl(url);

            // Log in is nodig omdat we anders geen tweets kunnen bekijken
            Console.WriteLine("You will need to log in to Twitter.");

            // Invoer email
            Console.Write("Phone, email address or username: ");
            string email = Console.ReadLine();

            // Input email
            // Zoek invoerveld
            IWebElement emailInput = driver.FindElement(By.Name("text"));
            // Typ email
            emailInput.SendKeys(email + Keys.Enter);

            // Invoer password
            Console.Write("Password: ");
            string password = Console.ReadLine();
            Console.WriteLine();

            // Input paswoord
            // Zoek invoerveld
            IWebElement passwordInput = driver.FindElement(By.Name("password"));
            // Typ paswoord
            passwordInput.SendKeys(password + Keys.Enter);

            // Wacht tot de pagina geladen is
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            // Zoek de zoekbalk
            driver.FindElement(By.XPath("//*[@id=\"react-root\"]/div/div/div[2]/main/div/div/div/div[2]/div/div[2]/div/div/div/div[1]/div/div/div/form/div[1]/div/div/div/label/div[2]/div/input")).SendKeys(searchTerm + Keys.Enter);

            // Zoek alle tweets op de pagina en voeg deze toe aan een lijst
            By elem_tweet_link = By.CssSelector("[data-testid=\"tweet\"]");
            ReadOnlyCollection<IWebElement> tweets = driver.FindElements(elem_tweet_link);

            // Implicit wait zodat de tweets zeker geladen zijn
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            // Lijst for export naar bestanden
            List<Tweet> tweetlist = new List<Tweet>();

            // Loop om enkel de eerste 5 tweets te bekijken
            for (int i = 0; i < 5; i++)
            {
                // Variabelen voor uitvoer
                string name, likes, retweets, comments, link;
                DateTime postDate;

                // Zoek naar gebruikersnaam
                IWebElement tweetName = tweets[i].FindElement(By.CssSelector("[data-testid=\"User-Names\"] > div a"));
                name = tweetName.Text;

                // Zoek naar het aantal likes
                IWebElement tweetLikes = tweets[i].FindElement(By.CssSelector("[data-testid=\"like\"]"));
                likes = tweetLikes.Text;

                // Zoek naar het aantal retweets
                IWebElement tweetRetweets = tweets[i].FindElement(By.CssSelector("[data-testid=\"retweet\"]"));
                retweets = tweetRetweets.Text;

                // Zoek naar het aantal reacties
                IWebElement tweetComments = tweets[i].FindElement(By.CssSelector("[data-testid=\"reply\"]"));
                comments = tweetComments.Text;

                // Zoek naar de datum van de tweet
                IWebElement tweetPostDate = tweets[i].FindElement(By.TagName("time"));
                postDate = DateTime.Parse(tweetPostDate.GetAttribute("datetime"));

                // Zoek naar de tweet link
                IWebElement tweetLink = tweets[i].FindElement(By.CssSelector("a[aria-label][dir]"));
                link = tweetLink.GetAttribute("href");

                // Voeg de elementen toe als tweet aan de lijst
                tweetlist.Add(new Tweet(name, likes, retweets, comments, postDate, link));

                // Output naar de console (als overzicht)
                Console.WriteLine("******* Tweet " + tcount + " *******");
                Console.WriteLine("Tweet User: " + name);
                Console.WriteLine("Tweet Likes: " + likes);
                Console.WriteLine("Tweet Retweets: " + retweets);
                Console.WriteLine("Tweet Comments: " + comments);
                Console.WriteLine("Tweet Post Date: " + postDate);
                Console.WriteLine("Tweet Link: " + link);
                Console.WriteLine("\n");
                tcount++;
            }

            // Naar CSV schrijven
            // Locatie op pc
            using (var writer = new StreamWriter(@"D:\DATA\Bureaublad\tweets.csv"))

            // Elementen uit de lijst naar het bestand schrijven
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(tweetlist);
            }
            // Bevestiging
            Console.WriteLine("Your data has been exported to CSV.");

            // Naar JSON schrijven
            string json = JsonSerializer.Serialize(tweetlist);

            // Locatie op pc
            File.WriteAllText(@"D:\DATA\Bureaublad\tweets.json", json);

            // Bevestiging
            Console.WriteLine("Your data has been exported to JSON.");
        }
    }
}
