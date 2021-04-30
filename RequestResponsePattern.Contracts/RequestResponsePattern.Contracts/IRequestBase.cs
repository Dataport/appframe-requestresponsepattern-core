using System;
using System.Collections.Generic;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Contracts
{
    /// <summary>
    /// Interface für Klassen, die einen Request implementieren.
    /// </summary>
    public interface IRequestBase
    {
        /// <summary>
        /// Eindeutige ID der Anfrage.
        /// </summary>
        /// <returns>GUID der Anfrage</returns>
        Guid Id { get; set; }

        /// <summary>
        /// Menge der Source IDs von Validierungsmeldungen, die
        /// bei der Ausführung unterdrückt werden sollen (vom Anwender "quittiert").
        /// </summary>
        /// <returns>Source IDs</returns>
        IEnumerable<string> OmitValidationSourceIds { get; set; }

        /// <summary>
        /// Ergebnis, falls die Anfrage ausgeführt wurde.
        /// Bei Mehrfachausführung das letzte Ergebnis.
        /// </summary>
        /// <returns>Das (letzte) Ergebnis</returns>
        IResponse Response { get; }

        /// <summary>
        /// Alle Ergebnisse in zeitlich absteigender Reihenfolge. 
        /// Bei Mehrfachausführungen relevant.
        /// </summary>
        /// <returns>Alle Ergebnisse (absteigend)</returns>
        IEnumerable<IResponse> Responses { get; }

        /// <summary>
        /// Typ der Responses.
        /// </summary>
        /// <returns>Response-Typ</returns>
        Type ResponseType { get; }

        /// <summary>
        /// Response bei Ausführung eines Handlers hinzufügen.
        /// </summary>
        /// <param name="response">Neuer Response</param>
        void AddResponse(object response);
    }
}
