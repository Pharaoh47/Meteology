using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Meteology
{
    // Base aspnet core configuration class, we specify here every thing we use on server side
    public class Startup
    {
        // We accept an IConfiguration on construct from Program class ad host builder
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }        

        // Services configuration goes here, for test project it is minimalistic
        public void ConfigureServices(IServiceCollection services)
        {
            // We use MySQL database, connection string from config
            services.AddDbContext<WeatherContext>(options => { options.UseMySQL(Configuration["ConnectionString"]); });   
            // We use MVC controllers for web api, so add it
            services.AddControllers();
            // An Swagger used to show CRUD summary
            services.AddSwaggerGen();
            // And razor page (single) we use to initiate SPA
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Swagger config
            app.UseSwagger();
            app.UseSwaggerUI(c => {c.SwaggerEndpoint("/swagger/v1/swagger.json", "Meteology API V1");});
            // Use developer exception page for test project
            app.UseDeveloperExceptionPage();
            // Use https redirection as an web standart
            app.UseHttpsRedirection();
            // Use static files for js and css of libraries and for site.js
            app.UseStaticFiles();
            // Use routing for web api controllers routes
            app.UseRouting();
            // Use endpoints defaults to MVC and Razor
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
