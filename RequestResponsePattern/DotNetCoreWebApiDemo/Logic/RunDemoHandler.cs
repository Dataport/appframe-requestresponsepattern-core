using Dataport.AppFrameDotNet.RequestResponsePattern;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using DotNetCoreWebApiDemo.Contracts;
using Microsoft.Extensions.Logging;

namespace DotNetCoreWebApiDemo.Logic
{
    /// <summary>
    /// ...
    /// </summary>
    public class RunDemoHandler : HandlerBase<RunDemo, RunDemoResponse>
    {
        /// <inheritdoc />
        protected override void Implementation(RunDemo request)
        {
            Logger.Log(LogLevel.Information, "Handler sagt Hi!");

            request.Response.MyGreetings = $"Hallo! {request.MyHello}";
            request.Response.AddMessage(ResponseMessage.CreateInformational(
                "D950089F-56CA-4BC3-8B11-773BCC95760F", "Yeah!"));
        }
    }
}
