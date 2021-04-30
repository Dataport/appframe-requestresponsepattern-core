#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CA1822 // Mark members as static
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Dataport.AppFrameDotNet.RequestResponsePattern.Core;
using Dataport.AppFrameDotNet.RequestResponsePattern.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
// ReSharper disable UnusedMember.Local
// ReSharper disable MemberCanBeMadeStatic.Local

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Core
{
    /// <summary>
    /// Tests zur konventionsbasierten Basis für Handler.
    /// </summary>
    [TestClass]
    public class HanderBase2Tests
    {
        [TestInitialize]
        public void Init()
        {
            // Reset der Runtime.
            Runtime.Current = new Runtime();
        }

        /// <summary>
        /// Findet die Aktionen nach Namenskonvention (Basisklasse zuerst).
        /// </summary>
        [TestMethod]
        public void HanderBase2_Execute_Success()
        {
            var request = new MyRequest();

            new MyHandler().Execute(request);

            var response = request.Response;

            Assert.AreEqual(true, response.HasExecutedWithSuccess, "response.HasExecutedWithSuccess");
            Assert.AreEqual(2, response.Ablauf.Count, "response.Ablauf.Count");
            Assert.AreEqual("MyBaseClassEvaluation", response.Ablauf[0]);
            Assert.AreEqual("MyEvaluation", response.Ablauf[1]);

            // Sollte private Methode durchlaufen sein, aber nur ein Mal.
            // (Da sie nicht auf das Namensmuster für automatische Detection passt,
            // sollte sie nur über den expliziten Aufruf durchlaufen werden).
            // Die nicht passende Methode sollte nicht dazu führen, dass nachfolgende
            // Methodendeklarationen nicht gefunden werden (Implementation).
            Assert.AreEqual(1, request.Response.Metadata.Messages
                .Count(m => m.SourceId == "HelperMethod"), "Message mit SourceID HelperMethod");
            Assert.AreEqual(1, request.Response.Metadata.Messages
                .Count(m => m.SourceId == "Implementation"), "Message mit SourceID Implementation");
        }

        /// <summary>
        /// Findet die Aktionen nach Namenskonvention (Basisklasse zuerst).
        /// </summary>
        [TestMethod]
        public void HanderBase2_Evaluation_Fail()
        {
            var request = new MyRequest()
            {
                FailsOnEvaluation = true
            };

            new MyHandler().Evaluate(request);

            var response = request.Response;

            Assert.AreEqual(true, response.HasFailed, "response.HasFailed");
            Assert.AreEqual(2, response.Ablauf.Count, "response.Ablauf.Count");
            Assert.AreEqual("MyBaseClassEvaluation", response.Ablauf[0]);
            Assert.AreEqual("MyEvaluation", response.Ablauf[1]);

            Assert.AreEqual(1, response.Metadata.Messages.Count, "response.Metadata.Messages.Count");
            Assert.AreEqual("B29FDC25-A37E-4E71-AFD9-917156F42FBF", 
                response.Metadata.Messages.First().SourceId, "response.Metadata.Messages.First().SourceId");
        }

        [TestMethod]
        [ExpectedException(typeof(HandlerInitializationException))]
        public void HanderBase2_Defect_Fail()
        {
            var request = new MyRequest();
            new MyDefectHandler().Execute(request);
        }

        public class MyRequest : RequestBase<TestResponse>
        {
            [StringLength(3)]
            public string FailsIfLongerThenThree { get; set; }

            public bool FailsOnEvaluation { get; set; }
        }

        public class TestResponse : ResponseBase
        {
            public List<string> Ablauf { get; set; } = new List<string>();
        }

        public  class MyHandler : MyHandlerBase<MyRequest, RequestContext>
        {
            protected void MyEvaluation(MyRequest request, RequestContext requestContext)
            {
                request.Response.Ablauf.Add("MyEvaluation");
                if (request.FailsOnEvaluation)
                {
                    // Ausstieg im Fehlerfall.
                    request.Response.AddMessage(
                        ResponseMessage.CreateWarning(
                            "B29FDC25-A37E-4E71-AFD9-917156F42FBF",
                            "FailsOnEvaluation!"));
                    return;
                }

                HelperMethod(request, requestContext);
            }

            // Hilfsmethode, die nicht zur Ablauffolge des Handlers gehört
            // (passt nicht zum Namensmuster). Sollte erlaubt sein, ohne die
            // restlichen Methoden zu beeinflussen.
            // ReSharper disable once UnusedParameter.Local
            private void HelperMethod(MyRequest request, RequestContext requestContext)
            {
                request.Response.AddMessage(ResponseMessage.CreateInformational(
                    "HelperMethod",
                    "Message aus Hilfsmethode."));
            }

            // ReSharper disable once UnusedParameter.Local
            private void Implementation(MyRequest request, RequestContext requestContext)
            {
                request.Response.AddMessage(ResponseMessage.CreateInformational(
                    "Implementation",
                    "Message aus Implementation."));
            }
        }

        public abstract class MyHandlerBase<TRequest, TRequestContext> : HandlerBase2<TRequest, TRequestContext> 
            where TRequest : IRequest<TestResponse> 
            where TRequestContext : IRequestContext, new()
        {
            protected void MyBaseClassEvaluation(TRequest request, TRequestContext requestContext)
            {
                request.Response.Ablauf.Add("MyBaseClassEvaluation");
            }
        }

        public class MyDefectHandler : MyHandlerBase<MyRequest, RequestContext>
        {
            //Falsche Signatur!
            protected void MyEvaluation(string request, RequestContext requestContext)
            {
                //
            }
        }
    }
}
