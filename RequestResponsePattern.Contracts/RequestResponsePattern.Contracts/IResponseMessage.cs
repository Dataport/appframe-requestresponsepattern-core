using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Contracts
{
    /// <summary>
    /// Interface für die Definition von Messages, die mit dem Response zurückgegeben werden können.
    /// </summary>
    public interface IResponseMessage
    {
        /// <summary>
        /// Auflistung von Kategorien, mit denen die Art der Message angegeben werden kann.
        /// </summary>
        /// <returns>Liste der Kategorien</returns>
        IEnumerable<string> Categories { get; set; }

        /// <summary>
        /// Einstufung der Message. Folgt den Einstufungen im Diagnostics-Bereich.
        /// </summary>
        /// <returns>Einstufung der Message</returns>
        EventLevel EventLevel { get; set; }

        /// <summary>
        /// Text, der zur Anzeige an Benutzer geeignet ist.
        /// </summary>
        /// <returns>Anzeigetext</returns>
        string UserFriendlyCaption { get; set; }

        /// <summary>
        /// Detailinformationen.
        /// </summary>
        /// <returns>Detailinformationen</returns>
        string Details { get; set; }

        /// <summary>
        /// Identifikationsmerkmal für die Nachricht.
        /// </summary>
        /// <returns>Identifikationsmerkmal</returns>
        string SourceId { get; set; }

        /// <summary>
        /// Gibt an, ob es sich um eine Validierungsmeldung handelt, die
        /// vom Anwender unterdrückt werden kann.
        /// </summary>
        /// <returns>true, wenn unterdrücken möglich ist</returns>
        bool IsOmittableValidation { get; set; }

        /// <summary>
        /// Meldungen, die dieser als Details untergeordnet sind.
        /// </summary>
        /// <returns>Untergeordnete Meldungen</returns>
        IEnumerable<IResponseMessage> NestedMessages { get; set; }

        /// <summary>
        /// Gibt die Message mit allen Inhalten als Text aus (z.B. für Tracing).
        /// </summary>
        /// <returns>Zusammengefasster Text</returns>
        string CompleteText { get; }
    }
}