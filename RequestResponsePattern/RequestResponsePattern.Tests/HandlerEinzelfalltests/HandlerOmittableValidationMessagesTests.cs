using System.Collections.Generic;
using System.Linq;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.HandlerEinzelfalltests
{
    /// <summary>
    /// Tests zu den unterdrückbaren Validierungsmeldungen,
    /// die in der Basisimplementierung von Handlern berücksichtigt werden.
    /// </summary>
    [TestClass]
    public class HandlerOmittableValidationMessagesTests
    {
        /// <summary>
        /// ResponseMessages, die unterdrückbare Validierungsmeldungen darstellen,
        /// sollten über ein entsprechendes Flag gekennzeichnet werden.
        /// </summary>
        [TestMethod]
        public void HandlerOmittableValidationMessages_FlagInResponseMessages()
        {
            var request = new TestRequest();
            new HandlerUnderTest().Execute(request);

            // Response prüfen (darf nicht durchgehen).
            var response = request.Response;
            Assert.AreEqual(false, response.HasExecutedWithSuccess);

            // Messages mit Flag prüfen.
            var omittableMessages = response.Metadata.Messages.Where(m => m.IsOmittableValidation);
            CollectionAssert.AreEquivalent(
                new[] { "Unterdrückbare Meldung 1", "Unterdrückbare Meldung 2" },
                omittableMessages.Select(x => x.UserFriendlyCaption).ToArray());

            // Messages ohne Flag prüfen.
            var nonOmittableMessages = response.Metadata.Messages.Where(m => !m.IsOmittableValidation);
            CollectionAssert.AreEquivalent(
                new[] { "NICHT unterdrückbare Meldung" },
                nonOmittableMessages.Select(x => x.UserFriendlyCaption).ToArray());
        }

        /// <summary>
        /// Der Handler sollte bei der Validierung die Meldungen unterdrücken,
        /// die im Request als "vom Anwender quittiert" angegeben wurden.
        /// </summary>
        [TestMethod]
        public void HandlerOmittableValidationMessages_OmitsValidationMessages()
        {
            // Erste Meldung unterdrücken.
            var request = new TestRequest("88552DAA-BF90-45B0-85C6-2E5EB7421884");
            new HandlerUnderTest().Execute(request);

            // Response prüfen (darf immer noch nicht durchgehen).
            var response = request.Response;
            Assert.AreEqual(false, response.HasExecutedWithSuccess);

            // Es dürfen nur noch zwei Meldungen übrig bleiben, da die Erste unterdrückt wurde.
            CollectionAssert.AreEquivalent(
                new[] { "Unterdrückbare Meldung 2", "NICHT unterdrückbare Meldung" },
                response.Metadata.Messages.Select(x => x.UserFriendlyCaption).ToArray());
        }

        /// <summary>
        /// Der Handler sollte den Request ausführen, wenn nur noch Validierungsmeldungen
        /// übrig bleiben, die alle unterdrückt werden sollen.
        /// </summary>
        [TestMethod]
        public void HandlerOmittableValidationMessages_Success_AllValidationMessagesOmitted()
        {
            // Alle Meldungen unterdrücken.
            var request = new TestRequest(
                "88552DAA-BF90-45B0-85C6-2E5EB7421884",
                "949793E7-D298-4EB6-BCB1-72B82B1A855D");

            // Diesmal Variante ohne "ununterdrückbare" Validierung.
            new HandlerUnderTest { CreateUnomittableMessage = false }.Execute(request);

            // Response prüfen (darf jetzt durchgehen).
            var response = request.Response;
            Assert.AreEqual(true, response.HasExecutedWithSuccess);
        }

        /// <summary>
        /// Der Handler sollte den Zugriff auf Meldungen ermöglichen, die
        /// aufgetreten sind, aber explizit vom Benutzer unterdrückt wurden.
        /// </summary>
        [TestMethod]
        public void HandlerOmittableValidationMessages_StoresOmittedMessages()
        {
            // Alle Meldungen unterdrücken.
            var request = new TestRequest(
                "88552DAA-BF90-45B0-85C6-2E5EB7421884",
                "949793E7-D298-4EB6-BCB1-72B82B1A855D");

            // Diesmal Variante ohne "ununterdrückbare" Validierung (=> Erfolg).
            var handler = new HandlerUnderTest { CreateUnomittableMessage = false };
            handler.Execute(request);

            // Prüfen, dass die unterdrückten Meldungen während der
            // Ausführung des Handlers zur Verfügung standen.
            Assert.AreEqual(2, handler.OmittedMessages.Count());
            Assert.AreEqual("88552DAA-BF90-45B0-85C6-2E5EB7421884", handler.OmittedMessages.ElementAt(0).SourceId);
            Assert.AreEqual("Unterdrückbare Meldung 1", handler.OmittedMessages.ElementAt(0).UserFriendlyCaption);
            Assert.AreEqual("949793E7-D298-4EB6-BCB1-72B82B1A855D", handler.OmittedMessages.ElementAt(1).SourceId);
            Assert.AreEqual("Unterdrückbare Meldung 2", handler.OmittedMessages.ElementAt(1).UserFriendlyCaption);
        }

        /// <summary>
        /// Der Handler sollte eine Validierungsmeldung erzeugen, wenn versucht
        /// wurde, eine Meldung zu unterdrücken, die NICHT unterdrückt werden darf.
        /// </summary>
        [TestMethod]
        public void HandlerOmittableValidationMessages_Fail_InvalidOmitSourceId()
        {
            // Versuchen, "ununterdrückbare" Meldung zu unterdrücken.
            var request = new TestRequest("F9DED588-2705-4F0E-9381-3B7B57668CE2");
            new HandlerUnderTest().Execute(request);

            // Response prüfen (darf immer noch nicht durchgehen).
            Assert.AreEqual(false, request.Response.HasExecutedWithSuccess);

            // Auf Message aus Basis prüfen, die die fehlerhafte unterdrückte ID angibt.
            Assert.IsTrue(request.ContainsMessageWithSourceId("82906E4F-58E3-4FC0-9EC6-AE9243E68C2F"));
        }

        private class TestRequest : RequestBase<ResponseBase>
        {
            // Vereinfachte Angabe von OmitValidationSourceIds in Testfällen.
            public TestRequest(params string[] omitValidationSourceIds)
            {
                Metadata.OmitValidationSourceIds = omitValidationSourceIds;
            }
        }

        private class HandlerUnderTest : HandlerBase<TestRequest, ResponseBase>
        {
            public bool CreateUnomittableMessage { get; set; } = true;

            // Protected Member für Prüfung im Unit-Test veröffentlichen.
            public IEnumerable<IResponseMessage> OmittedMessages { get; private set; }

            // IDs der Validierungen, die unterdrückt werden dürfen.
            protected override IEnumerable<string> GetOmittableValidationSourceIds(TestRequest request) => new[]
            {
                "88552DAA-BF90-45B0-85C6-2E5EB7421884",
                "949793E7-D298-4EB6-BCB1-72B82B1A855D"
            };

            protected override IEnumerable<IResponseMessage> ExtendedRequestValidation(TestRequest request)
            {
                yield return ResponseMessage.CreateWarning("88552DAA-BF90-45B0-85C6-2E5EB7421884",
                    "Unterdrückbare Meldung 1");

                yield return ResponseMessage.CreateWarning("949793E7-D298-4EB6-BCB1-72B82B1A855D",
                    "Unterdrückbare Meldung 2");

                if (CreateUnomittableMessage)
                {
                    yield return ResponseMessage.CreateWarning("F9DED588-2705-4F0E-9381-3B7B57668CE2",
                        "NICHT unterdrückbare Meldung");
                }
            }

            protected override void Implementation(TestRequest request)
            {
                OmittedMessages = OmittedValidationMessages;
            }
        }
    }
}