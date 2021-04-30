namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Testobjekte
{
    public class MyPluginAttribute : PluginAttribute
    {
        public MyPluginAttribute(PluginPosition position) : base(position)
        {  }

        public override object CreatePlugin()
        {
            return new MyPlugin();
        }
    }
}
