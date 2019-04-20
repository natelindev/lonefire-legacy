using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace lonefire
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration(
            (hostingContext, config) =>
            {
                //Load Additional Json configs
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("db_string.json", optional: false, reloadOnChange: true);
                config.AddJsonFile("upload_env.json", optional: false, reloadOnChange: true);
                config.AddJsonFile("toast_option.json", optional: false, reloadOnChange: true);
            })
            .UseStartup<Startup>();
    }
}
