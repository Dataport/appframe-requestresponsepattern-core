namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Testobjekte
{    
    class MyMultipleHandler1 : HandlerBase<MyMultipleRequest, MyResponse>
    {
        protected override void Implementation(MyMultipleRequest request)
        {
            request.Response.Antwort = $"(1) Deine Frage war: {request.Frage}";
        }
    }
}
