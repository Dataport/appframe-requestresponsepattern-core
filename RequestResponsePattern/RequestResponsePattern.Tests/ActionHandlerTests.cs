using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests
{
    /// <summary>
    /// Tests zum ActionHandler, der einen Handler durch eine Action definiert.
    /// </summary>
    [TestClass]
    public class ActionHandlerTests
    {
        [TestMethod]
        public void ActionHandler_Execute()
        {
            var wasExecuted = false;

            var request = new MyRequest();

            var handler = new ActionHandler<MyRequest, ResponseWith<bool>>(x => {
                wasExecuted = true;
                x.Response.Data = true;
            });

            handler.Execute(request);

            Assert.AreEqual(true, request.Response.HasExecutedWithSuccess, "HasExecutedWithSuccess");
            Assert.AreEqual(true, request.Response.Data, "Data");
            Assert.AreEqual(true, wasExecuted, "wasExecuted");
        }

        public class MyRequest : RequestBase<ResponseWith<bool>>
        { }

        
    }

}
