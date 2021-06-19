using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace StockMarket.Integration.Test.Fixtures
{
    public class ExchangeApiFactory: WebApplicationFactory<Exchange.Api.Startup>
    {
        public IConfiguration Configuration { get; private set; }

        private string GetProjectDirectory()
        {
            var binDir = Directory.GetParent(Directory.GetCurrentDirectory());
            var testDir = binDir.Parent.Parent;
            var rootDir = testDir.Parent;
            var projectDir = Path.Combine(rootDir.FullName, "StockMarket.Exchange.Api");
            return projectDir;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(GetProjectDirectory())
                .AddJsonFile("appsettings.Testing.json", false)
                .AddEnvironmentVariables()
                .Build();

            builder.ConfigureAppConfiguration(config => config.AddConfiguration(Configuration));
        }
    }
}