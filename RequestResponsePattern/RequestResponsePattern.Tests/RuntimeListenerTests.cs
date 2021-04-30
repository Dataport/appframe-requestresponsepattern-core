using System.Linq;
using Dataport.AppFrameDotNet.RequestResponsePattern.Exceptions;
using Dataport.AppFrameDotNet.RequestResponsePattern.HandlerProvider;
using Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Testobjekte;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests
{
    /// <summary>
    /// Tests zu den RuntimeListeners, die zur Laufzeit weitere Handler
    /// auf Requests reagieren lassen.
    /// </summary>
    [TestClass]
    public class RuntimeListenerTests
    {
        [TestMethod]
        public void RuntimeListener_Try_Success()
        {
            RuntimeListeners.Global.ClearListeners();

            RuntimeListeners.Global.AddListener(new CallRuntimeHandler());

            var result = new CallRuntime().Try().Response;

            Assert.AreEqual(true, result.HasExecutedWithSuccess);
            Assert.AreEqual("Hallo Runtime!", result.Message);
        }

        [TestMethod]
        [ExpectedException(typeof(HandlerNotFoundException))]
        public void RuntimeListener_Try_NoHandler_Fail()
        {
            RuntimeListeners.Global.ClearListeners();

            new CallRuntime().Try();
        }

        [TestMethod]
        [ExpectedException(typeof(MulipleHandlerFoundException))]
        public void RuntimeListener_Try_MultipleHandler_Fail()
        {
            RuntimeListeners.Global.ClearListeners();

            RuntimeListeners.Global.AddListener(new CallRuntimeHandler());
            RuntimeListeners.Global.AddListener(new CallRuntimeHandler());

            new CallRuntime().Try();
        }

        [TestMethod]
        public void RuntimeListener_Call()
        {
            RuntimeListeners.Global.ClearListeners();

            RuntimeListeners.Global.AddListener(new CallRuntimeHandler());
            RuntimeListeners.Global.AddListener(new CallRuntimeHandler());

            var result = new CallRuntime().Call().Responses.ToList();

            Assert.AreEqual(2, result.Count());

            Assert.AreEqual(true, result.First().HasExecutedWithSuccess);
            Assert.AreEqual("Hallo Runtime!", result.First().Message);

            Assert.AreEqual(true, result.Last().HasExecutedWithSuccess);
            Assert.AreEqual("Hallo Runtime!", result.Last().Message);
        }

        [TestMethod]
        public void RuntimeListener_UseLocal_Try_Success()
        {
            RuntimeListeners.Global.ClearListeners();

            using var locals = RuntimeListeners.CreateLocalSet();
            locals.AddListener(new CallRuntimeHandler());

            var result = new CallRuntime().Try().Response;

            Assert.AreEqual(true, result.HasExecutedWithSuccess);
            Assert.AreEqual("Hallo Runtime!", result.Message);
        }

        [TestMethod]
        [ExpectedException(typeof(HandlerNotFoundException))]
        public void RuntimeListener_OutOfScope_Try_Fail()
        {
            RuntimeListeners.Global.ClearListeners();

            using (var locals = RuntimeListeners.CreateLocalSet())
            {
                locals.AddListener(new CallRuntimeHandler());
            }

            new CallRuntime().Try();
        }
    }
}
