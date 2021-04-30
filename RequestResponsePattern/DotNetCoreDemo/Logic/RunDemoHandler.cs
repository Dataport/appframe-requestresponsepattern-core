using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Dataport.AppFrameDotNet.RequestResponsePattern.Core;
using DotNetCoreDemo.Contracts;

namespace DotNetCoreDemo.Logic
{
    /// <summary>
    /// ...
    /// </summary>
    public class RunDemoHandler : HandlerBase2<RunDemo, RequestContext>
    {
        protected void ExtendedRequestEvaluation(RunDemo request, RequestContext context)
        {
            if (!IsValid(request.MyHello))
            {
                request.Response.AddMessage(ResponseMessage.CreateWarning(
                    "EDA738B8-C062-4649-9A93-487E3E720065",
                    "Das 'Hallo' im Request ist 'BAD'.")
                );
                request.Response.MarkAsFailed();
            }
        }

        private bool IsValid(string myHello)
        {
            return myHello != "BAD";
        }

        protected void Implementation(RunDemo request, RequestContext state)
        {
            request.Response.MyGreetings = $"Hallo! {request.MyHello}";
            request.Response.AddMessage(ResponseMessage.CreateInformational(
                "8DE5D3BF-5982-4432-ABEC-50AE91FFA03F", "Yeah!"));
        }
    }
}
