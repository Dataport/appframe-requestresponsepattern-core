using System.Linq;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Dataport.AppFrameDotNet.RequestResponsePattern.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Core
{
    /// <summary>
    /// Test, ob die globalen Definitionsquellen aus der Runtime angewendet wurden.
    /// </summary>
    [TestClass]
    public class GlobalActionDefinitionsTests
    {
        [TestInitialize]
        public void Init()
        {
            //Reset der Runtime.
            Runtime.Current = new Runtime(
                globalActionDefinitionSources: new[]
                {
                    new MyGlobalSource()
                });
        }

        [TestMethod]
        public void GlobalActionDefinitionsTestsAttribute_Success()
        {
            var request = new MyRequest();
            new MyHandler().Execute(request);
            var response = request.Response;

            Assert.IsTrue(response.HasExecutedWithSuccess, "response.HasExecutedWithSuccess");
            Assert.AreEqual(1, response.Metadata.Messages.Count, "response.Metadata.Messages.Count");
            Assert.AreEqual("9251AD4E-A7A8-4936-B6B5-10C53634C2D7", response.Metadata.Messages.First().SourceId, "response.Metadata.Messages.First().SourceId");
        }

        public class MyRequest : RequestBase<ResponseBase> { }

        public class MyHandler : HandlerBase2<MyRequest, RequestContext>
        { }

        public class MyGlobalSource : IActionDefinitionSource
        {
            /// <inheritdoc />
            public ActionDefinition<IRequestBase, IRequestContext>[] GetActionDefinitions()
            {
                return new[]
                {
                    new ActionDefinition<IRequestBase, IRequestContext>()
                    {
                        Name="MyGlobalAction",
                        ActionPosition = ActionPosition.OnEnter,
                        Action = (request, state) => request.Response.Messages.Add(
                            ResponseMessage.CreateInformational("9251AD4E-A7A8-4936-B6B5-10C53634C2D7", "Hello, World!"))
                    }
                };
            }
        }
    }
}
