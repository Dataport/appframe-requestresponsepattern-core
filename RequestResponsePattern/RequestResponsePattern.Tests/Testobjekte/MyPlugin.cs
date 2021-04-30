using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Testobjekte
{
    class MyPlugin : IPlugin<MyRequest>
    {
        public void Execute(MyRequest request, State requestState)
        {
            request.Response.AddMessage(new ResponseMessage("23990743-960B-42B1-A7D9-A1A8495EFFFB", $"Plugin ausgeührt. Die Nummer war {request.MeineNummer}!"));
            requestState.Properties.MyPluginProperty = "Hallo aus Plugin via RequestState";
        }
    }
}
