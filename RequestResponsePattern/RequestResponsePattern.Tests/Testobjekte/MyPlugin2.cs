using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Testobjekte
{
    class MyPlugin2 : IPlugin<MyRequest>
    {
        public void Execute(MyRequest request, State requestState)
        {
            request.Response.AddMessage(new ResponseMessage("5920C20C-3705-4597-AEDF-8B07B3E91546", $"Plugin ausgeührt. Die Nummer war {request.MeineNummer}!"));
        }
    }
}
