using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StockMarket.Company.Api.Models;
using StockMarket.Company.Api.Services;

namespace StockMarket.Company.Api
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
            services.AddOptions<AuthConfig>().Bind(Configuration.GetSection("Auth"));
            services.AddOptions<DatabaseConfig>().Bind(Configuration.GetSection("Database"));
            services.AddOptions<EventBusConfig>().Bind(Configuration.GetSection("EventBus"));

            services.AddSingleton<DatabaseContext>();
            services.AddSingleton<EventBus>();
            services.AddSingleton<ExchangeSync>();
            services.AddSingleton<SectorSync>();
            services.AddScoped<CompanyRepo>();

            services.AddControllers();

            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StockMarket.Company.Api", Version = "v1" }));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "StockMarket.Company.Api v1"));
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
