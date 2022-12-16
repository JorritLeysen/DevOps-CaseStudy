// Nodige pakketten om de code te doen werken
using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Text.Json;

// Project waarin we werken
namespace Ictob_webscraper
{
    // Klasse job voor de uitvoer naar bestanden
    public class Job
    {
        // Constructor
        public Job(string title, string company, string location, string keywords, string detailPagina)
        {
            Title = title;
            Company = company;
            Location = location;
            Keywords = keywords;
            DetailPagina = detailPagina;
        }
        // Eigenschappen
        public string Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string Keywords { get; set; }
        public string DetailPagina { get; set; }
    }
    // Ictjob.be webscraper
    class Ictjob
    {
        static void Main(string[] args)
        {
            // Chrome als browser kiezen
            IWebDriver driver = new ChromeDriver();

            // Input van zoekterm
            Console.Write("Enter a job you want to search for: ");
            string searchTerm = Console.ReadLine();
            Console.WriteLine();

            // Ictjob
            // Naar URL gaan
            string url = "https://www.ictjob.be/nl/";
            driver.Navigate().GoToUrl(url);

            // Implicit wait zodat de pagina zeker volledig laadt
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            // Inputvak vinden en zoekterm intypen
            driver.FindElement(By.Name("keywords")).SendKeys(searchTerm + Keys.Enter);

            // Lijst for export naar bestanden
            List<Job> joblist = new List<Job>();

            // Counter voor console uitvoer
            int counter = 1;
            // Loop om de eerste 5 jobs to nemen
            for (int jcount = 1; jcount < 7; jcount++)
            {
                // 4e job geeft een error omdat dit een call to action is
                if (jcount != 4) { 
                    // Variabelen voor uitvoer
                    string title, company, location, keywords, detailPagina;

                    // Zoek naar titel
                    IWebElement jobTitle = driver.FindElement(By.XPath("//*[@id=\"search-result-body\"]/div/ul/li[" + jcount.ToString() + "]/span[2]/a/h2"));
                    title = jobTitle.Text;

                    // Zoek naar het bedrijf
                    IWebElement jobCompany = driver.FindElement(By.XPath("//*[@id=\"search-result-body\"]/div/ul/li[" + jcount.ToString() + "]/span[2]/span[1]"));
                    company = jobCompany.Text;

                    // Zoek naar de locatie
                    IWebElement jobLocation = driver.FindElement(By.XPath("//*[@id=\"search-result-body\"]/div/ul/li[" + jcount.ToString() + "]/span[2]/span[2]/span[2]/span/span"));
                    location = jobLocation.Text;

                    // Zoek naar de beschrijving
                    IWebElement jobKeywords = driver.FindElement(By.XPath("//*[@id=\"search-result-body\"]/div/ul/li[" + jcount.ToString() + "]/span[2]/span[3]"));
                    keywords = jobKeywords.Text;

                    // Zoek naar de link
                    IWebElement jobLink = driver.FindElement(By.XPath("//*[@id=\"search-result-body\"]/div/ul/li[" + jcount.ToString() + "]/span[2]/a"));
                    detailPagina = jobLink.GetAttribute("href");

                    // Voeg de elementen toe als video aan de lijst
                    joblist.Add(new Job(title, company, location, keywords, detailPagina));

                    // Output naar de console (als overzicht)
                    Console.WriteLine("******* Job " + counter + " *******");
                    Console.WriteLine("Title: " + title);
                    Console.WriteLine("Company: " + company);
                    Console.WriteLine("Location: " + location);
                    Console.WriteLine("Keywords: " + keywords.ToString());
                    Console.WriteLine("Link detailpagina: " + detailPagina);
                    Console.WriteLine("\n");
                    counter++;
                }
            }

            // Naar CSV schrijven
            // Locatie op pc
            using (var writer = new StreamWriter(@"D:\DATA\Bureaublad\CaseStudy\jobs.csv"))

            // Elementen uit de lijst naar het bestand schrijven
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(joblist);
            }
            // Bevestiging
            Console.WriteLine("Your data has been exported to CSV.");

            // Naar JSON schrijven
            string json = JsonSerializer.Serialize(joblist);

            // Locatie op pc
            File.WriteAllText(@"D:\DATA\Bureaublad\CaseStudy\jobs.json", json);

            // Bevestiging
            Console.WriteLine("Your data has been exported to JSON.");
        }
    }
}
