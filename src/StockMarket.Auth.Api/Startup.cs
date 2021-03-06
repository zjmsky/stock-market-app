using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using StockMarket.Auth.Api.Models;
using StockMarket.Auth.Api.Services;

namespace StockMarket.Auth.Api
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

            services.AddSingleton<DatabaseContext>();
            services.AddSingleton<UserRepo>();
            services.AddSingleton<AuthProvider>();

            services.AddCustomCors("CustomPolicy");
            services.AddCustomAuth(Configuration.GetSection("Auth"));
            services.AddControllers();
            services.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StockMarket.Auth.Api", Version = "v1" }));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "StockMarket.Auth.Api v1"));
            }

            app.UseCors("CustomPolicy");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
