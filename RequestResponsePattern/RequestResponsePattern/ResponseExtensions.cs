using System;
using System.Diagnostics.Tracing;
using System.Linq;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern
{
    /// <summary>
    /// Erweiterungsmethoden für Responses.
    /// </summary>
    public static class ResponseExtensions
    {
        /// <summary>
        /// Einen Response als untergordneten Response anwenden (wenn ein Handler einen anderen Request ausführt).
        /// </summary>
        /// <param name="response">Response des ausgeführten Requests.</param>
        /// <param name="masterResponse">Response des Handlers, der den Nested Request ausgeführt hat.</param>
        /// <param name="successSourceId">ID der Message im Falle des Erfolgs.</param>
        /// <param name="successCaption">Benutzermeldung  im Falle des Fehlschlags.</param>
        /// <param name="failSourceId">ID der Message im Falle des Fehlschlags.</param>
        /// <param name="failCaption">Benutzermeldung im Falle des Fehlschlags.</param>
        /// <param name="applyFailOnMaster">Fehlschlag kaskadiert auf den ursrünglichen Request (Default: true).</param>
        /// <param name="successMessageOnlyWithNestedMessages">Beim Erfolg wird keine Meldung generiert, 
        /// wenn es keine Meldungen im ausgeführtem "NestedRequest" gibt (Default: true).</param>
        /// <returns>Erfolg des angehängten Requests.</returns>
        public static bool ApplyAsNestedResponseOf(this IResponse response,
            ResponseBase masterResponse,
            string successSourceId,
            string successCaption,
            string failSourceId,
            string failCaption,
            bool applyFailOnMaster = true,
            bool successMessageOnlyWithNestedMessages = true)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (masterResponse == null) throw new ArgumentNullException(nameof(masterResponse));

            // ggf. Status übernehmen
            if (applyFailOnMaster && response.Failed)
                masterResponse.MarkAsFailed();

            // ggf. bei Erfolg ohne weitere Infos keine Message generieren
            if (successMessageOnlyWithNestedMessages && !response.Failed && !response.Messages.Any())
                return true;

            var message = new ResponseMessage();

            if (response.Failed)
            {
                message.SourceId = failSourceId;
                message.EventLevel = EventLevel.Warning;
                message.UserFriendlyCaption = failCaption;
            }
            else
            {
                message.SourceId = successSourceId;
                message.EventLevel = EventLevel.Informational;
                message.UserFriendlyCaption = successCaption;
            }

            message.NestedMessages = response.Messages;
            message.Categories = new[] { DefaultResponseMessageCategories.Response };

            masterResponse.AddMessage(message);

            return !response.Failed;
        }
    }
}
