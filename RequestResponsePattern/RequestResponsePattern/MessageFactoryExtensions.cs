using System;
using System.Diagnostics.Tracing;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern
{
    /// <summary>
    /// Extension-Methods zum Erstellen einer Response-Message.
    /// </summary>
    public static class MessageFactoryExtensions
    {
        /// <summary>
        /// Erstellen einer Response-Message.
        /// </summary>
        /// <param name="ex">Exception</param>
        /// <returns>Message</returns>
        public static ResponseMessage AsResponseMessage(this Exception ex)
        {
            if (ex == null)
                throw new ArgumentNullException(nameof(ex));

            return new ResponseMessage()
            {
                UserFriendlyCaption = "Es ist ein Fehler aufgetreten.",
                Details = ex.ToString(),
                SourceId = ex.GetType().FullName,
                EventLevel = EventLevel.Error,
                Categories = new[] { DefaultResponseMessageCategories.Exception }
            };
        }

        /// <summary>
        /// Erstellen einer Response-Message aus einem Response.
        /// </summary>
        /// <param name="sourceId">Identifikationsmerkmal für die Nachricht.</param>
        /// <param name="response">Response einer Subabfrage.</param>
        /// <param name="successCaption">Benutzermeldung für Erfolg.</param>
        /// <param name="failedCaption">Benutzermeldung für einen Fehlschlag.</param>
        /// <returns>Die ResponseMessage, die den Response beschreibt.</returns>
        [Obsolete("Bitte zukünftig ApplyAsNestedResponseOf verwenden.")]
        public static ResponseMessage AsResponseMessage(this IResponse response, string sourceId, string successCaption, string failedCaption)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            var message = new ResponseMessage
            {
                SourceId = sourceId,
                NestedMessages = response.Messages,
                Categories = new[] { DefaultResponseMessageCategories.Response }
            };

            if (response.Failed)
            {
                message.EventLevel = EventLevel.Warning;
                message.UserFriendlyCaption = failedCaption;
            }
            else
            {
                message.EventLevel = EventLevel.Informational;
                message.UserFriendlyCaption = successCaption;
            }

            return message;
        }
    }
}
