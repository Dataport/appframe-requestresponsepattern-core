using System.Collections.Generic;
using System.Linq;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Dataport.AppFrameDotNet.RequestResponsePattern.Core;
using Dataport.AppFrameDotNet.RequestResponsePattern.HandlerProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests
{
    /// <summary>
    /// Tests zur LocalRuntime, die für einen bestimmten Scope (in einem using-Block) gültig ist.
    /// </summary>
    [TestClass]
    public class LocalRuntimeTests
    {
        [TestMethod]
        public void LocalRuntime_LifetcycleTest()
        {
            //HandlerProvider nicht verfügbar
            var responsePre = new MyRequest().Call().Responses;
            Assert.AreEqual(0, responsePre.Count());

            using (new LocalRuntime(new Runtime(null, null, new MyHandlerProvider())))
            {
                //HandlerProvider über LocalRuntime verfügbar
                var responseIn = new MyRequest().Do().Response;
                Assert.AreEqual("395E09A4-FD70-4524-9C78-D05A4951652D", responseIn.Metadata.Messages.First().SourceId);
            }

            //HandlerProvider außerhalb Using-Block nicht verfügbar
            var responsePost = new MyRequest().Call().Responses;
            Assert.AreEqual(0, responsePost.Count());
        }

        public class MyRequest : RequestBase<ResponseBase>
        { }

        public class MyHandlerProvider : IHandlerProvider
        {
            /// <inheritdoc />
            public IEnumerable<IHandler<TRequest>> GetHandler<TRequest>() where TRequest : IRequestBase
            {
                yield return new Handler<TRequest, RequestContext>(
                    new ActionDefinition<TRequest, RequestContext>()
                    {
                        Name = "Dummy",
                        ActionPosition = ActionPosition.Implementation,
                        Action = ((request, context) => request.Response.Messages.Add(
                            ResponseMessage.CreateInformational("395E09A4-FD70-4524-9C78-D05A4951652D", "Local")))
                    });
            }
        }
    }
}
