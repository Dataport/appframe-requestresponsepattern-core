using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Dataport.AppFrameDotNet.RequestResponsePattern.Core;

namespace DotNetCoreDemo.Logic
{
    /// <summary>
    /// ...
    /// </summary>
    public class MyGlobalPlugin : IActionDefinitionSource
    {
        /// <inheritdoc />
        public ActionDefinition<IRequestBase, IRequestContext>[] GetActionDefinitions()
        {
            return new[]
            {
                new ActionDefinition<IRequestBase, IRequestContext>()
                {
                    Name = "MyGlobalPlugin",
                    ActionPosition = ActionPosition.OnExecutedWithSuccess,
                    Action = (request, state) => request.Response.Messages.Add(
                        ResponseMessage.CreateInformational(
                            "EB338BF7-EE5A-454C-B728-A2913F9F5A07",
                        "Bei einem Erfolg meldet sich auch MyGlobalPlugin!"))
                }
            };
        }
    }
}
