using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StockMarket.Sector.Api.Models;
using StockMarket.Sector.Api.Services;

namespace StockMarket.Sector.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions<DatabaseConfig>().Bind(Configuration.GetSection("Database"));
            services.AddOptions<EventBusConfig>().Bind(Configuration.GetSection("EventBus"));

            services.AddSingleton<DatabaseContext>();
            services.AddSingleton<EventBus>();
            services.AddSingleton<SectorRepo>();
            services.AddSingleton<CompanyRepo>();
            services.AddSingleton<CompanySync>();

            services.AddCustomCors("CustomPolicy");
            services.AddCustomAuth(Configuration.GetSection("Auth"));
            services.AddControllers();
            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StockMarket.Sector.Api", Version = "v1" }));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StockMarket.Sector.Api v1"));
            }

            app.ApplicationServices.GetService<CompanySync>();

            app.UseCors("CustomPolicy");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
