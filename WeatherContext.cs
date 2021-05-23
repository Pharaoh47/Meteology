using System;
using Microsoft.EntityFrameworkCore;
using Meteology.Model;

namespace Meteology
{
    public class WeatherContext : DbContext
    {
        public WeatherContext(){}

        private string conectionString;
        public WeatherContext(string conectionString)            
        {
            this.conectionString = conectionString;
        }
        /*public WeatherContext(DbContextOptions<WeatherContext> options)
                : base(options)
        {}*/
        public DbSet<City> Cities { get; set; }
        public DbSet<Temperature> Temperatures { get; set; }

        /*protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseMySQL(conectionString);*/

        public WeatherContext(DbContextOptions<WeatherContext> options)
            : base(options)
        {}
    }
}