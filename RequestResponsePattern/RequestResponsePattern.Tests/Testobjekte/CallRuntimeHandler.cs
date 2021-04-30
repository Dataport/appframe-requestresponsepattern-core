namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Testobjekte
{
    class CallRuntimeHandler : HandlerBase<CallRuntime, CallRuntimeResponse>
    {     
        protected override void Implementation(CallRuntime request)
        {
            request.Response.Message = "Hallo Runtime!";
        }
    }
}
