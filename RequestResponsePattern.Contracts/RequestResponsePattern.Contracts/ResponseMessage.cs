using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Contracts
{
    /// <summary>
    /// Generische Basisklasse für Meldungen.
    /// </summary>
    public class ResponseMessage : IResponseMessage
    {
        /// <summary>
        /// Default Konstruktor.
        /// </summary>
        public ResponseMessage()
        { }

        /// <summary>
        /// Konstruktor für eine einfache Informationsmeldung.
        /// </summary>
        /// <param name="sourceId">Identifikationsmerkmal für die Nachricht</param>
        /// <param name="userFriendlyCaption">Text, der zur Anzeige an Benutzer geeignet ist.</param>
        public ResponseMessage(string sourceId, string userFriendlyCaption)
        {
            SourceId = sourceId;
            UserFriendlyCaption = userFriendlyCaption;
        }

        /// <summary>
        /// Auflistung von Kategorien, mit denen die Art der Message angegeben werden kann.
        /// </summary>
        /// <returns>Aufzählung von Kategorien</returns>
        public IEnumerable<string> Categories { get; set; }

        /// <summary>
        /// Einstufung der Message. Folgt den Einstufungen im Diagnostics-Bereich.
        /// Standardwert ist "Informational".
        /// </summary>
        /// <returns>Einstufung der Message</returns>
        /// TODO: EventLevel "nachbauen"
        public EventLevel EventLevel { get; set; } = EventLevel.Informational;

        /// <summary>
        /// Text, der zur Anzeige an Benutzer geeignet ist.
        /// </summary>
        /// <returns>Anzeigetext</returns>
        public string UserFriendlyCaption { get; set; }

        /// <summary>
        /// Detailinformationen.
        /// </summary>
        /// <returns>Detailinformationen</returns>
        public string Details { get; set; }

        /// <summary>
        /// Identifikationsmerkmal für die Nachricht.
        /// </summary>
        /// <returns>Identifikationsmerkmal</returns>
        public string SourceId { get; set; }

        /// <summary>
        /// Gibt an, ob es sich um eine Validierungsmeldung handelt, die
        /// vom Anwender unterdrückt werden kann.
        /// </summary>
        /// <returns>true, wenn die Meldung unterdrückt werden kann.</returns>
        public bool IsOmittableValidation { get; set; }

        /// <summary>
        /// Meldungen, die dieser als Details untergeordnet sind.
        /// </summary>
        /// <returns>Aufzählung von untergeordneten Meldungen</returns>
        public IEnumerable<IResponseMessage> NestedMessages { get; set; }

        /// <summary>
        /// Gibt die Message mit allen Inhalten als Text aus (z.B. für Tracing).
        /// </summary>
        /// <returns>Zusammengefasste Meldung</returns>
        public string CompleteText => GetCompleteText();

        /// <summary>
        /// Erstellt eine ResponseMessage mit dem EventLevel 'Error'.
        /// </summary>
        /// <param name="sourceId">Identifikationsmerkmal für die Nachricht.</param>
        /// <param name="userFriendlyCaption">Text der zur Anzeige an Benutzer geeignet ist.</param>
        /// <returns>Die erzeugte ResponseMessage mit dem EventLevel Error.</returns>
        public static ResponseMessage CreateError(string sourceId, string userFriendlyCaption) =>
            new ResponseMessage(sourceId, userFriendlyCaption) { EventLevel = EventLevel.Error };

        /// <summary>
        /// Erstellt eine ResponseMessage mit dem EventLevel 'Informational'.
        /// </summary>
        /// <param name="sourceId">Identifikationsmerkmal für die Nachricht.</param>
        /// <param name="userFriendlyCaption">Text der zur Anzeige an Benutzer geeignet ist.</param>
        /// <returns>Die erzeugte ResponseMessage mit dem EventLevel Informational.</returns>
        public static ResponseMessage CreateInformational(string sourceId, string userFriendlyCaption) =>
            new ResponseMessage(sourceId, userFriendlyCaption) { EventLevel = EventLevel.Informational };

        /// <summary>
        /// Erstellt eine ResponseMessage mit dem EventLevel 'Warning'.
        /// </summary>
        /// <param name="sourceId">Identifikationsmerkmal für die Nachricht.</param>
        /// <param name="userFriendlyCaption">Text der zur Anzeige an Benutzer geeignet ist.</param>
        /// <returns>Die erzeugte ResponseMessage mit dem EventLevel Warning.</returns>
        public static ResponseMessage CreateWarning(string sourceId, string userFriendlyCaption) =>
            new ResponseMessage(sourceId, userFriendlyCaption) { EventLevel = EventLevel.Warning };

        /// <summary>
        /// Zusammenfassen aller Details dieser ResponseMessage
        /// </summary>
        /// <returns>Zusammengefasste Meldung</returns>
        private string GetCompleteText()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"[{GetType().Name}:{SourceId}]");

            if (Categories != null)
            {
                sb.Append("(");
                sb.Append(string.Join(",", Categories));
                sb.AppendLine(")");
            }

            sb.AppendLine(UserFriendlyCaption);

            sb.AppendLine();

            if (!string.IsNullOrWhiteSpace(Details))
            {
                sb.AppendLine(Details);
                sb.AppendLine();
            }

            foreach (var nested in NestedMessages ?? new IResponseMessage[] { })
            {
                var first = true;
                foreach (var line in nested.CompleteText.Split(new[] { "\r\n" }, StringSplitOptions.None))
                {
                    if (first)
                    {
                        sb.Append("+---");
                        first = false;
                    }
                    else
                    {
                        sb.Append("    ");
                    }

                    sb.AppendLine(line);
                }
            }

            sb.AppendLine();

            return sb.ToString();
        }
    }
}
