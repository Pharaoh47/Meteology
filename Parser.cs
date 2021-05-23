using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Meteology.Model;
using Microsoft.EntityFrameworkCore;

namespace Meteology.Parsing
{
    // An static parsing class with set of static async methods
    public static class Parser
    {
        // Base parse url of GisMeteo
        const string mainUrl = "https://www.gismeteo.ru";

        // Method for extraction proper url for every city of interest
        static async Task<Dictionary<string, string>> ParseMainPage(String url)
        {
            // Url -- name holder
            Dictionary<string, string> cityData = new Dictionary<string, string>();

            // Async download and parse document
            HtmlDocument doc = await new HtmlWeb().LoadFromWebAsync(url);

            // Select proper html block, it is noscript block before client js goes work
            foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//noscript/a"))
                // Links within contains name and url of the city
                cityData.Add(node.Attributes["data-name"].Value, node.Attributes["href"].Value);

            // return dictionary
            return cityData;
        }

        // Method for extracting list of min/max measurements from downloaded city's page
        static async Task<City> ParseCityPage(City city, string url)
        {
            // We a starting to download several pages at once
            // Sometimes underlying network system block some of request
            // So we need to await timeout and resend request
            HtmlDocument doc = null;
            do
            {
                try
                {
                    // Async download and parse document
                    doc = await new HtmlWeb().LoadFromWebAsync(url);
                }
                catch
                {
                    // Prevent blocking by ddos protection
                    Thread.Sleep(3000);
                }
            } while (doc == null);

            // Current date without time
            DateTime currentDate = DateTime.Now.Date;

            // Data holder container on page and its descendants
            HtmlNode valueNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'templine')]//div[contains(@class, 'values')]");
            // Foreach descendant holder
            foreach (HtmlNode node in valueNode.SelectNodes("div[contains(@class, 'value')]"))
            {
                // Extract min and max celsius measurements
                string max = node.SelectSingleNode("div[contains(@class, 'maxt')]/span[@class='unit unit_temperature_c']").InnerText;
                string min = node.SelectSingleNode("div[contains(@class, 'mint')]/span[@class='unit unit_temperature_c']").InnerText;

                // Update measurements
                city.UpdateMeasurements(currentDate,
                    // Remove + sign from positive integers
                    Convert.ToInt32(min.Trim(new Char[] { ' ', '+' })),
                    Convert.ToInt32(max.Trim(new Char[] { ' ', '+' })));
                
                // Increment date counter
                currentDate = currentDate.AddDays(1);
            }

            // Return updated city
            return city;
        }

        // Main static asynchronous method to parse entire tree of cities and measurements
        public static async Task Parse(WeatherContext context)
        {
            // Extract url for every city on the page block
            Dictionary<string, string> cityUrls = await ParseMainPage(mainUrl);

            // List of parallel task for downloading and process city pages
            List<Task<City>> cityTasks = new List<Task<City>>();

            // Foreach significant city
            foreach (string cityName in cityUrls.Keys)
            {
                // Load city and actual measurements from database
                City city = context.Cities
                    .Where(c => c.Name == cityName)
                    .Include(c => c.Temperatures
                        .Where(t => t.Date >= DateTime.Now.Date))
                    .FirstOrDefault();

                // Create city object if none of it is in database
                if (city == null)
                {
                    city = new City()
                    {
                        Name = cityName,
                        Temperatures = new List<Temperature>()
                    };
                    // Add city to context
                    context.Cities.Add(city);
                }
                // Add update city's measurements async process to tasks
                cityTasks.Add(ParseCityPage(city, mainUrl + cityUrls[cityName] + "/10-days/"));
            }

            // Wait for all cities to update its measurements
            Task.WaitAll(cityTasks.ToArray());
        }
    }
}