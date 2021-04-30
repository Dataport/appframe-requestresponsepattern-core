using System;
using System.Linq;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Dataport.AppFrameDotNet.RequestResponsePattern.Exceptions;
using Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Testobjekte;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests
{
    /// <summary>
    /// Tests zu den RequestOperations zur Ausführung von Requests.
    /// </summary>
    [TestClass]
    public class RequestOperationsTest
    {
        [TestInitialize]
        public void Init()
        {
            // Mockup-HandlerProvider verwenden.
            Runtime.Current = new Runtime(
                null,
                null,
                new MyHandlerProvider());
        }

        [TestMethod]
        public void RequestOperations_Do_Success()
        {
            var request = new MyRequest() { Frage = "Hallo?", MeineNummer = 42 };

            var response = request.Do().Response;

            Assert.AreEqual(true, response.HasExecutedWithSuccess);
            Assert.IsTrue(response.Antwort.Contains("Hallo?"));
        }

        /// <summary>
        /// Es wurde DoOptional ausgeführt und es existiert ein Handler.
        /// </summary>
        [TestMethod]
        public void RequestOperations_DoOptional_Success()
        {
            var request = new MyRequest() { Frage = "Hallo?", MeineNummer = 42 };

            var response = request.DoOptional().Response;

            Assert.AreEqual(true, response.HasExecutedWithSuccess);
            Assert.IsTrue(response.Antwort.Contains("Hallo?"));
        }

        /// <summary>
        /// Es wurde DoOptional ausgeführt und es existiert ein Handler.
        /// </summary>
        [TestMethod]
        public void RequestOperations_DoOptional_WithoutHandler()
        {
            var request = new MyRequestWithoutHandler();

            var response = request.DoOptional().Response;

            Assert.IsNull(response);
        }

        [TestMethod]
        public void RequestOperations_DoDynamicSuccess()
        {
            IRequestBase request = new MyRequest() { Frage = "Hallo?", MeineNummer = 42 };

            var response = request.DoDynamic().Response;

            Assert.AreEqual(true, response.Executed);
            Assert.AreEqual(false, response.Failed);
            Assert.IsTrue(((MyResponse)response).Antwort.Contains("Hallo?"));
        }

        [TestMethod]
        [ExpectedException(typeof(RequestFailedException))]
        public void RequestOperations_Do_Failed()
        {
            var request = new MyRequest() { Frage = null };

            var response = request.Do().Response;

            Console.WriteLine(response.Antwort);
        }

        [TestMethod]
        public void RequestOperations_Try_Success()
        {
            var request = new MyRequest() { Frage = "Hallo?", MeineNummer = 42 };

            var response = request.Try().Response;

            Assert.AreEqual(true, response.HasExecutedWithSuccess);
            Assert.AreEqual(false, response.HasFailed);
            Assert.IsTrue(response.Antwort.Contains("Hallo?"));
        }

        /// <summary>
        /// TryOptional wurde ausgeführt und es gibt einen Handler.
        /// </summary>
        [TestMethod]
        public void RequestOperations_TryOptional_HandlerExists()
        {
            var request = new MyRequest() { Frage = "Hallo?", MeineNummer = 42 };

            var response = request.TryOptional().Response;

            Assert.AreEqual(true, response.HasExecutedWithSuccess);
            Assert.AreEqual(false, response.HasFailed);
            Assert.IsTrue(response.Antwort.Contains("Hallo?"));
        }

        /// <summary>
        /// TryOptional wurde ausgeführt und es gibt keinen Handler.
        /// Kein Fehler aber auch keine Rückgabe erwartet.
        /// </summary>
        [TestMethod]
        public void RequestOperations_TryOptional_WithoutHandler()
        {
            var request = new MyRequestWithoutHandler();

            var response = request.TryOptional().Response;

            Assert.IsNull(response);
        }

        [TestMethod]
        public void RequestOperations_TryDynamic_Success()
        {
            IRequestBase request = new MyRequest() { Frage = "Hallo?", MeineNummer = 42 };

            var response = request.TryDynamic().Response;

            Assert.AreEqual(true, response.Executed);
            Assert.AreEqual(false, response.Failed);
            Assert.IsTrue(((MyResponse)response).Antwort.Contains("Hallo?"));
        }

        [TestMethod]
        public void RequestOperations_Try_Failed()
        {
            var request = new MyRequest() { Frage = null };

            var response = request.Try().Response;

            Assert.AreEqual(false, response.HasExecutedWithSuccess);
            Assert.AreEqual(true, response.HasFailed);
            Assert.IsTrue(request.ContainsMessageWithUserFriendlyCaption("CDC0753C-C5AC-49BE-BDD8-2C1D60B4919D"));
        }

        [TestMethod]
        public void RequestOperations_Evaluate_Success()
        {
            var request = new MyRequest() { Frage = "Hallo?", MeineNummer = 42 };

            var response = request.Evaluate().Response;

            Assert.AreEqual(true, response.HasEvaluatedWithSuccess);
        }

        [TestMethod]
        public void RequestOperations_EvaluateDynamic_Success()
        {
            IRequestBase request = new MyRequest() { Frage = "Hallo?", MeineNummer = 42 };

            var response = request.EvaluateDynamic().Response;

            Assert.AreEqual(false, response.Failed);
            Assert.AreEqual(false, response.Executed);
        }

        [TestMethod]
        public void RequestOperations_Evaluate_Failed()
        {
            var request = new MyRequest() { Frage = null };

            var response = request.Evaluate().Response;

            Assert.AreEqual(true, response.HasFailed);
            Assert.IsTrue(request.ContainsMessageWithUserFriendlyCaption("CDC0753C-C5AC-49BE-BDD8-2C1D60B4919D"));
        }

        [TestMethod]
        public void RequestOperations_Call_Success()
        {
            var request = new MyMultipleRequest() { Frage = "Hallo?" };

            request.Call();

            var response1 = request.Responses.First(x => x.Antwort.Contains("1"));
            var response2 = request.Responses.First(x => x.Antwort.Contains("2"));

            Assert.IsTrue(response1.Antwort.Contains("Hallo?"));
            Assert.IsTrue(response2.Antwort.Contains("Hallo?"));
        }

        [TestMethod]
        public void RequestOperations_CallDynamic_Success()
        {
            IRequestBase request = new MyMultipleRequest() { Frage = "Hallo?" };

            request.CallDynamic();

            var response1 = request.Responses.First(x => ((MyResponse)x).Antwort.Contains("1")) as MyResponse;
            var response2 = request.Responses.First(x => ((MyResponse)x).Antwort.Contains("2")) as MyResponse;

            Assert.IsNotNull(response1);
            Assert.IsNotNull(response2);

            Assert.IsTrue(response1.Antwort.Contains("Hallo?"));
            Assert.IsTrue(response2.Antwort.Contains("Hallo?"));
        }

        [TestMethod]
        public void RequestOperations_Call_Failure()
        {
            var request = new MyMultipleRequest() { Frage = null };

            request.Call();

            Assert.AreEqual(2, request.Responses.Count());

            Assert.AreEqual(true, request.Response.HasFailed);
            foreach (var response in request.Responses)
            {
                Assert.AreEqual(true, response.HasFailed);
            }
        }

        [TestMethod]
        public void RequestOperations_HandlerPluginExecuted_AttributRegistration_Success()
        {
            var request = new MyRequest() { Frage = "Hallo?", MeineNummer = 42 };

            var response = request.Do().Response;

            var pluginMessage =
                response.Metadata.Messages
                    .First(x => x.SourceId == "23990743-960B-42B1-A7D9-A1A8495EFFFB");

            Assert.IsTrue(pluginMessage.UserFriendlyCaption.Contains("42"));
        }

        [TestMethod]
        public void RequestOperations_HandlerPluginExecuted_CustomInitializerRegistration_Success()
        {
            var request = new MyRequest() { Frage = "Hallo?", MeineNummer = 42 };

            var response = request.Do().Response;

            var pluginMessage =
                response.Metadata.Messages
                    .First(x => x.SourceId == "5920C20C-3705-4597-AEDF-8B07B3E91546");

            Assert.IsTrue(pluginMessage.UserFriendlyCaption.Contains("42"));
        }

        /// <summary>
        /// Szenario: Ein Plugin schreibt in RequestState, der andere liest.
        /// </summary>
        [TestMethod]
        public void RequestOperations_HandlerPluginExecuted_RequestStateAccess_Success()
        {
            MyPluginFollowup.MyPluginPropertyAsFound = null;

            new MyRequest() { Frage = "Hallo?", MeineNummer = 42 }.Do();

            Assert.AreEqual("Hallo aus Plugin via RequestState", MyPluginFollowup.MyPluginPropertyAsFound);
        }
    }
}
