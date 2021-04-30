using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Dataport.AppFrameDotNet.RequestResponsePattern.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Core
{
    /// <summary>
    /// Ist implizit ein Test von HandlerCore, da dieser die Basisklasse von Handler ist.
    /// </summary>
    [TestClass]
    public class HandlerTests
    {
        [TestInitialize]
        public void Init()
        {
            //Reset der Runtime.
            Runtime.Current = new Runtime();
        }

        /// <summary>
        /// Prüft den erfolgreichen Durchlauf einer Execute-Aktion (Try, Do), insbesondere auf
        /// Reihenfolge Durchlauf der Aktionen.
        /// </summary>
        [TestMethod]
        public void Handler_Execute_Success()
        {
            var handler = new Handler<DummyRequest, DummyState>(
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    Name = "Evaluation1",
                    ActionPosition = ActionPosition.Evaluation,
                    Action = (request, state) => request.Response.Ablauf.Add("Evaluation1")
                },
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    Name = "Implementation",
                    ActionPosition = ActionPosition.Implementation,
                    Action = (request, state) => request.Response.Ablauf.Add("Implementation")
                },
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    Name = "Evaluation2",
                    ActionPosition = ActionPosition.Evaluation,
                    Action = (request, state) => request.Response.Ablauf.Add("Evaluation2")
                },
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    Name = "OnExit",
                    ActionPosition = ActionPosition.OnExit,
                    Action = (request, state) => request.Response.Ablauf.Add("OnExit")
                },
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    Name = "OnExecutedWithSuccess",
                    ActionPosition = ActionPosition.OnExecutedWithSuccess,
                    Action = (request, state) => request.Response.Ablauf.Add("OnExecutedWithSuccess")
                },
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    Name = "OnEvaluatedWithSuccess",
                    ActionPosition = ActionPosition.OnEvaluatedWithSuccess,
                    Action = (request, state) => request.Response.Ablauf.Add("OnEvaluatedWithSuccess")
                });

            var dummyRequest = new DummyRequest();

            handler.Execute(dummyRequest);

            var response = dummyRequest.Response;

            Assert.IsTrue(response.HasExecutedWithSuccess);

            Assert.AreEqual(6, response.Ablauf.Count);
            Assert.AreEqual("Evaluation1", response.Ablauf[0]);
            Assert.AreEqual("Evaluation2", response.Ablauf[1]);
            Assert.AreEqual("OnEvaluatedWithSuccess", response.Ablauf[2]);
            Assert.AreEqual("Implementation", response.Ablauf[3]);
            Assert.AreEqual("OnExecutedWithSuccess", response.Ablauf[4]);
            Assert.AreEqual("OnExit", response.Ablauf[5]);
        }

        /// <summary>
        /// Prüft den Abbruch einer Execute-Aktion (Try, Do) exemplarisch an der RequestValidation, insbesondere auf
        /// Reihenfolge Durchlauf der Aktionen.
        /// </summary>
        [TestMethod]
        public void Handler_Execute_RequestValidationFail()
        {
            var handler = new Handler<DummyRequest, DummyState>(
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    //Darf nicht ausgeführt werden
                    Name = "Evaluation1",
                    ActionPosition = ActionPosition.Evaluation,
                    Action = (request, state) => request.Response.Ablauf.Add("Evaluation1")
                },
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    Name = "OnExit",
                    ActionPosition = ActionPosition.OnExit,
                    Action = (request, state) => request.Response.Ablauf.Add("OnExit")
                });

            var dummyRequest = new DummyRequest()
            {
                FailsIfLongerThenThree = "FAIL"
            };

            handler.Execute(dummyRequest);

            var response = dummyRequest.Response;

            Assert.IsFalse(response.HasExecutedWithSuccess);

            Assert.AreEqual(1, response.Ablauf.Count);
            Assert.AreEqual("OnExit", response.Ablauf[0]);
        }

        /// <summary>
        /// Prüft den erfolgreichen Durchlauf einer Execute-Aktion (Try, Do), insbesondere auf
        /// Reihenfolge Durchlauf der Aktionen.
        /// </summary>
        [TestMethod]
        public void Handler_Evaluate_Success()
        {
            var handler = new Handler<DummyRequest, DummyState>(
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    Name = "Evaluation1",
                    ActionPosition = ActionPosition.Evaluation,
                    Action = (request, state) => request.Response.Ablauf.Add("Evaluation1")
                },
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    //Darf nicht ausgeführt werden
                    Name = "Implementation",
                    ActionPosition = ActionPosition.Implementation,
                    Action = (request, state) => request.Response.Ablauf.Add("Implementation")
                },
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    Name = "Evaluation2",
                    ActionPosition = ActionPosition.Evaluation,
                    Action = (request, state) => request.Response.Ablauf.Add("Evaluation2")
                },
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    Name = "OnExit",
                    ActionPosition = ActionPosition.OnExit,
                    Action = (request, state) => request.Response.Ablauf.Add("OnExit")
                },
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    //Darf nicht ausgeführt werden
                    Name = "OnExecutedWithSuccess",
                    ActionPosition = ActionPosition.OnExecutedWithSuccess,
                    Action = (request, state) => request.Response.Ablauf.Add("OnExecutedWithSuccess")
                },
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    Name = "OnEvaluatedWithSuccess",
                    ActionPosition = ActionPosition.OnEvaluatedWithSuccess,
                    Action = (request, state) => request.Response.Ablauf.Add("OnEvaluatedWithSuccess")
                });

            var dummyRequest = new DummyRequest();

            handler.Evaluate(dummyRequest);

            var response = dummyRequest.Response;

            Assert.IsTrue(response.HasEvaluatedWithSuccess);
            Assert.IsFalse(response.HasExecutedWithSuccess);

            Assert.AreEqual(4, response.Ablauf.Count);
            Assert.AreEqual("Evaluation1", response.Ablauf[0]);
            Assert.AreEqual("Evaluation2", response.Ablauf[1]);
            Assert.AreEqual("OnEvaluatedWithSuccess", response.Ablauf[2]);
            Assert.AreEqual("OnExit", response.Ablauf[3]);
        }

        /// <summary>
        /// Prüft den Abbruch mit Exception.
        /// </summary>
        [TestMethod]
        public void Handler_Execute_Fail_Error()
        {
            var handler = new Handler<DummyRequest, DummyState>(
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    Name = "Implementation",
                    ActionPosition = ActionPosition.Implementation,
                    Action = (request, state) => throw new Exception()
                },
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    //Darf nicht ausgeführt werden
                    Name = "OnExit",
                    ActionPosition = ActionPosition.OnExit,
                    Action = (request, state) => request.Response.Ablauf.Add("OnExit")
                },
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    //Darf nicht ausgeführt werden
                    Name = "OnExecutedWithSuccess",
                    ActionPosition = ActionPosition.OnExecutedWithSuccess,
                    Action = (request, state) => request.Response.Ablauf.Add("OnExecutedWithSuccess")
                },
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    Name = "OnError",
                    ActionPosition = ActionPosition.OnError,
                    Action = (request, state) =>
                    {
                        Assert.IsNotNull(state.Exception, "state.Exception");
                        Assert.AreEqual("Implementation", state.LastAction, "state.LastAction");
                        request.Response.Ablauf.Add("OnError");
                    }
                });

            var dummyRequest = new DummyRequest();

            handler.Execute(dummyRequest);

            var response = dummyRequest.Response;

            Assert.IsFalse(response.HasExecutedWithSuccess, "response.HasExecutedWithSuccess");

            Assert.AreEqual(1, response.Ablauf.Count, "response.Ablauf.Count");
            Assert.AreEqual("OnError", response.Ablauf[0]);
        }

        /// <summary>
        /// Prüft, dass bei einem Fail die aktuelle Phase (ActionPosition) noch ganz durchlaufen wird.
        /// </summary>
        /// <remarks>
        /// Dies ist insbesondere für Validierungen so implementiert, damit diese vollständig durchlaufen.
        /// </remarks>
        [TestMethod]
        public void Handler_Execute_Fail_PhaseBeendet()
        {
            var handler = new Handler<DummyRequest, DummyState>(
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    Name = "Evaluation1",
                    ActionPosition = ActionPosition.Evaluation,
                    Action = (request, state) =>
                    {
                        request.Response.Ablauf.Add("Evaluation1");
                        request.Response.AddMessage(
                            ResponseMessage.CreateWarning("1DDADAE7-CB9B-4456-A43F-1E798CDD0A25",
                                "Fail!"));
                    }
                },
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    Name = "Implementation",
                    ActionPosition = ActionPosition.Implementation,
                    Action = (request, state) => request.Response.Ablauf.Add("Implementation")
                },
                new ActionDefinition<DummyRequest, DummyState>()
                {
                    Name = "Evaluation2",
                    ActionPosition = ActionPosition.Evaluation,
                    Action = (request, state) => request.Response.Ablauf.Add("Evaluation2")
                });

            var dummyRequest = new DummyRequest();

            handler.Execute(dummyRequest);

            var response = dummyRequest.Response;

            Assert.IsFalse(response.HasExecutedWithSuccess);

            Assert.AreEqual(2, response.Ablauf.Count);
            Assert.AreEqual("Evaluation1", response.Ablauf[0]);
            Assert.AreEqual("Evaluation2", response.Ablauf[1]);
        }


        public class DummyRequest : RequestBase<TestResponse>
        {
            [StringLength(3)]
            public string FailsIfLongerThenThree { get; set; }
        }

        public class TestResponse : ResponseBase
        {
            public List<string> Ablauf { get; set; } = new List<string>();
        }

        public class DummyState : RequestContext
        { }
    }
}
