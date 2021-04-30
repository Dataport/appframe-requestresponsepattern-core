using System.Linq;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Dataport.AppFrameDotNet.RequestResponsePattern.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Core
{
    /// <summary>
    /// Test, ob das Plugin gefunden und ausgeführt wurde.
    /// </summary>
    [TestClass]
    public class ActionDefinitionAddInAttributeTests
    {
        [TestInitialize]
        public void Init()
        {
            //Reset der Runtime.
            Runtime.Current = new Runtime();
        }

        [TestMethod]
        public void ActionDefinitionAddInAttribute_Success()
        {
            var request = new MyRequest();
            new MyHandler().Execute(request);
            var response = request.Response;

            Assert.IsTrue(response.HasExecutedWithSuccess, "response.HasExecutedWithSuccess");
            Assert.AreEqual(1, response.Metadata.Messages.Count, "response.Metadata.Messages.Count");
            Assert.AreEqual("526E7B45-B48E-4B74-85D1-45414348948F", response.Metadata.Messages.First().SourceId, "response.Metadata.Messages.First().SourceId");
        }

        public class MyRequest : RequestBase<ResponseBase> { }

        [MyAttribute]
        public class MyHandler : HandlerBase2<MyRequest, RequestContext>
        { }

        public class MyAttribute : ActionDefinitionAddInAttribute
        {
            /// <inheritdoc />
            public override ActionDefinition<IRequestBase, IRequestContext>[] GetActionDefinitions()
            {
                return new[]
                {
                    new ActionDefinition<IRequestBase, IRequestContext>()
                    {
                        Name="MyPluginAction",
                        ActionPosition = ActionPosition.OnEnter,
                        Action = (request, state) => request.Response.Messages.Add(
                            ResponseMessage.CreateInformational("526E7B45-B48E-4B74-85D1-45414348948F", "Hello, World!"))
                    }
                };
            }
        }
    }
}
