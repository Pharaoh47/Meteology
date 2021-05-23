using System;
using System.Threading.Tasks;
using System.Linq;
using Meteology.Parsing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;


namespace Meteology
{
    class Program
    {
        // system entry point
        public static async Task Main(string[] args)
        {
            // Make an host
            using (IHost host = CreateHostBuilder(args).Build()){
                // If user lunch the parser
                if (args.Contains("--parse"))
                    // Instantinate context
                    using (WeatherContext context = (WeatherContext) host.Services.GetService(typeof(WeatherContext))){
                        // Parse gismeteo to context
                        await Parser.Parse(context);
                        // Save updated cities and temperatures to database
                        context.SaveChanges();
                    }                   
                else
                    //Else standart dotnet runner
                    host.Run();
            }            
        }

        // Here we can specify some basic config if we wants
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            // We start from typical behavior   
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Use class named Startup -- it is typical too
                    webBuilder.UseStartup<Startup>();
                });
    }
}
