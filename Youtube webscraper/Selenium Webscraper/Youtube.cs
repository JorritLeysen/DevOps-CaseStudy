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
namespace Youtube_webscraper
{
    // Klasse video voor de uitvoer naar bestanden
    public class Video
    {
        // Constructor
        public Video(string title, string uploader, string views, string link)
        {
            Title = title;
            Uploader = uploader;
            Views = views;
            Link = link;
        }
        // Eigenschappen
        public string Title { get; set; }
        public string Uploader { get; set; }
        public string Views { get; set; }
        public string Link { get; set; }
    }
    // Youtube webscraper
    class Youtube
    {
        static void Main(string[] args)
        {
            // Chrome als browser kiezen
            IWebDriver driver = new ChromeDriver();

            // Input van zoekterm
            Console.Write("Enter a Youtube searchterm you want information of: ");
            string searchTerm = Console.ReadLine();
            Console.WriteLine();

            // Youtube
            // Counter voor console uitvoer
            int vcount = 1;

            // URL maken met zoekterm en sorteren op recent
            string url = "https://www.youtube.com/results?search_query=" + searchTerm + "&sp=CAI%253D"; // "&sp=CAI%253D" sorteerd op recente uploads

            // Surf naar de URL
            driver.Navigate().GoToUrl(url);

            // Zoek naar alle video's op de pagina en voeg deze toe aan een lijst
            By elem_video_link = By.CssSelector("ytd-video-renderer.style-scope.ytd-item-section-renderer");
            ReadOnlyCollection<IWebElement> videos = driver.FindElements(elem_video_link);

            // Lijst for export naar bestanden
            List<Video> videolist = new List<Video>();

            // Loop om enkel de eerste 5 video's te bekijken
            for (int i=0; i<5; i++)
            {
                // Variabelen voor uitvoer
                string title, videoUrl, uploader, views;

                // Zoek naar de titel
                IWebElement videoTitle = videos[i].FindElement(By.CssSelector("#video-title"));
                title = videoTitle.Text;

                // Neem de URL van het titel-element
                videoUrl = videoTitle.GetAttribute("href");

                // Zoek het kanaal van de video
                IWebElement videoUploader = videos[i].FindElement(By.XPath(".//*[@id=\"text\"]/a"));
                uploader = videoUploader.GetAttribute("text");

                // Zoek het aantal views
                IWebElement videoViews = videos[i].FindElement(By.XPath(".//*[@id='metadata-line']/span[1]"));
                views = videoViews.Text;

                // Voeg de elementen toe als video aan de lijst
                videolist.Add(new Video(title, uploader, views, videoUrl));
                
                // Output naar de console (als overzicht)
                Console.WriteLine("*** Video " + vcount + " ***");
                Console.WriteLine("Video Title: " + title);
                Console.WriteLine("Video Uploader: " + uploader);
                Console.WriteLine("Video Views: " + views);
                Console.WriteLine("Video Link: " + videoUrl);
                Console.WriteLine("\n");
                vcount++;
            }

            // Naar CSV schrijven
            // Locatie op pc
            using (var writer = new StreamWriter(@"D:\DATA\Bureaublad\videos.csv"))

            // Elementen uit de lijst naar het bestand schrijven
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(videolist);
            }
            // Bevestiging
            Console.WriteLine("Your data has been exported to CSV.");

            // Naar JSON schrijven
            string json = JsonSerializer.Serialize(videolist);

            // Locatie op pc
            File.WriteAllText(@"D:\DATA\Bureaublad\videos.json", json);

            // Bevestiging
            Console.WriteLine("Your data has been exported to JSON.");
        }
    }
}
