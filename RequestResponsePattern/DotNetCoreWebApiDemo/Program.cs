using Dataport.AppFrameDotNet.RequestResponsePattern;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DotNetCoreWebApiDemo
{
    /// <summary>
    /// ...
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            //siehe auch Startup.ConfigureServices()
            Runtime.Current = new Runtime(host.Services);

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
