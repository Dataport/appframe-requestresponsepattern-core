using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Testobjekte
{
    [MyPlugin(PluginPosition.Enter)]
    [MyPluginFollowup(PluginPosition.Exit)]
    class MyHandler : HandlerBase<MyRequest, MyResponse>
    {
        /// <summary>
        /// Eigene Initialisierungsschritte für den Handler ausführen.
        /// </summary>
        /// <remarks>Plugins hier über RegisterPlugin() hinzufügen!</remarks>
        protected override void CustomInitialization()
        {
            RegisterPlugin(new MyPlugin2(), PluginPosition.Enter);
        }

        protected override void Implementation(MyRequest request)
        {
            request.Response.Antwort = $"Deine Frage war: {request.Frage}";
            request.Response.AddMessage(ResponseMessage.CreateInformational(
                "448F5817-D65E-45BC-A680-2302E4398FE3",
                "Es Antworetete: Dummy-Handler für Unittests."));
        }
    }
}
