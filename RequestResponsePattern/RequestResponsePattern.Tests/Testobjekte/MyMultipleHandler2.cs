namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Testobjekte
{
    class MyMultipleHandler2 : HandlerBase<MyMultipleRequest, MyResponse>
    {
        protected override void Implementation(MyMultipleRequest request)
        {
            request.Response.Antwort = $"(2) Deine Frage war: {request.Frage}";
        }
    }
}
