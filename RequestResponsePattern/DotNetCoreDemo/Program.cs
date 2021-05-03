using Dataport.AppFrameDotNet.RequestResponsePattern;
using Dataport.AppFrameDotNet.RequestResponsePattern.HandlerProvider;
using DotNetCoreDemo.Contracts;
using DotNetCoreDemo.Logic;
using Microsoft.Extensions.Logging;
using System;

namespace DotNetCoreDemo
{
    /// <summary>
    /// ...
    /// </summary>
    class Program
    {
        static void Main()
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Dataport", LogLevel.Trace)
                    .AddFilter("DotNetCoreDemo", LogLevel.Trace)
                    .AddConsole();
            });

            Runtime.Current = new Runtime(loggerFactory,
                globalActionDefinitionSources: new[] { new MyGlobalPlugin() });

            using var locals = RuntimeListeners.CreateLocalSet();
            locals.AddListener(new RunDemoHandler());

            RunDemoPositivTest(); 
            RunDemoNegativTest();


            Console.ReadKey();
        }

        private static void RunDemoPositivTest()
        {
            // Positiv-Test
            var greetings = new RunDemo()
            {
                MyHello = "Demo hier"
            }.Do().Response;

            Console.WriteLine(greetings.MyGreetings);
        }

        private static void RunDemoNegativTest()
        {
            // Negativ-Test
            var greetings = new RunDemo()
            {
                MyHello = "BAD"
            }.Try().Response;
            
            if (greetings.HasFailed) 
                Console.WriteLine("Wie erwartet fehlgeschlagen.");
            else
                throw new ApplicationException("Test fehlgeschlagen.");
        }
    }
}
