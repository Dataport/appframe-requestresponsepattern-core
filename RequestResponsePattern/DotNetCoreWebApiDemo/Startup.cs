using System.Text;
using Dataport.AppFrameDotNet.RequestResponsePattern;
using DotNetCoreWebApiDemo.Contracts;
using DotNetCoreWebApiDemo.Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DotNetCoreWebApiDemo
{
    /// <summary>
    /// ...
    /// </summary>
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //RunDemo-Handler registrieren als Singleton
            services.AddSingleton<IHandler<RunDemo>, RunDemoHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync(
                        new RunDemo()
                        {
                            MyHello = "Aufgelöst mit IServiceProvider"
                        }.Do().Response.MyGreetings, Encoding.Unicode);
                });
            });
        }
    }
}
