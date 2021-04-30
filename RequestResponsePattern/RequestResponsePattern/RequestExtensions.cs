using System;
using System.Linq;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern
{
    /// <summary>
    /// Hilfsmethoden für den Umgang mit Requests.
    /// </summary>
    public static class RequestExtensions
    {
        /// <summary>
        /// Prüft, ob im aktuellen Response eine Message mit der
        /// angegebenen SourceId vorhanden ist.
        /// </summary>
        /// <param name="context">Request.</param>
        /// <param name="sourceId">SourceId.</param>
        /// <returns>True, falls eine Message mit der SourceId gefunden wurde; ansonsten false.</returns>
        public static bool ContainsMessageWithSourceId(this IRequestBase context, string sourceId) =>
            context.ContainsMessage(m => m.SourceId == sourceId);

        /// <summary>
        /// Prüft, ob im aktuellem Response eine Message mit der
        /// angegebenen UserFriendlyCaption vorhanden ist.
        /// </summary>
        /// <param name="context">Request.</param>
        /// <param name="userFriendlyCaption">UserFriendlyCaption.</param>
        /// <returns>True, falls eine Message mit der UserFriendlyCaption gefunden wurde; ansonsten false.</returns>
        public static bool ContainsMessageWithUserFriendlyCaption(this IRequestBase context, string userFriendlyCaption) =>
            context.ContainsMessage(m => m.UserFriendlyCaption == userFriendlyCaption);

        /// <summary>
        /// Prüft, ob im aktuellem Response eine Message vorhanden ist,
        /// die die angegebene Bedingung erfüllt.
        /// </summary>
        /// <param name="context">Request.</param>
        /// <param name="predicate">Bedingung.</param>
        /// <returns>True, falls eine Message gefunden wurde, die die Bedingung erfüllt; ansonsten false.</returns>
        public static bool ContainsMessage(this IRequestBase context, Predicate<IResponseMessage> predicate)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            if (context.Response == null) return false;

            return context.Response.Messages.Any(m => m.ContainsMessage(predicate));
        }

        /// <summary>
        /// Prüft, ob die Message oder eine der NestedMessages die angegebene Bedingung erfüllt.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="predicate">Bedingung.</param>
        /// <returns>True, falls die Message selbst oder eine der NestedMessages die Bedingung erfüllt; ansonsten false.</returns>
        public static bool ContainsMessage(this IResponseMessage message, Predicate<IResponseMessage> predicate)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            // Message selbst auf Bedingung prüfen.
            return predicate(message) ||
                // Rekursiv die NestedMessages prüfen.
                (message.NestedMessages?.Any(x => ContainsMessage(x, predicate)) ?? false);
        }
    }
}