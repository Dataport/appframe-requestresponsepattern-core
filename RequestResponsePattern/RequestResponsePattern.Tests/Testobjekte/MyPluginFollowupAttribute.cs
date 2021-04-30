namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Testobjekte
{
    public class MyPluginFollowupAttribute : PluginAttribute
    {
        public MyPluginFollowupAttribute(PluginPosition position) : base(position)
        {  }

        public override object CreatePlugin()
        {
            return new MyPluginFollowup();
        }
    }
}
