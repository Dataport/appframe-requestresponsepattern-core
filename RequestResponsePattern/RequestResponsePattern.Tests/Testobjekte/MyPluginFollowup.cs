namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Testobjekte
{
    class MyPluginFollowup : IPlugin<MyRequest>
    {
        public static string MyPluginPropertyAsFound;

        public void Execute(MyRequest request, State requestState)
        {

            MyPluginPropertyAsFound = requestState.Properties.MyPluginProperty;
        }
    }
}
