using System.Collections.Generic;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Contracts
{
    /// <summary> 
    /// Interface für Klassen, die einen Request implementieren.
    /// </summary>
    /// <typeparam name="TResponse">Typ der Response</typeparam>
    public interface IRequest<out TResponse> : IRequestBase
        where TResponse : IResponse
    {
        /// <summary>
        /// Ergebnis, falls die Anfrage ausgeführt wurde.
        /// Bei Mehrfachausführung das letzte Ergebnis.
        /// </summary>
        /// <returns>Das (letzte) Ergebnis</returns>
        new TResponse Response { get; }


        /// <summary>
        /// Alle Ergebnisse in zeitlich absteigender Reihenfolge. 
        /// Bei Mehrfachausführungen relevant.
        /// </summary>
        /// <returns>Alle Ergebnisse (absteigend)</returns>
        new IEnumerable<TResponse> Responses { get; }
    }
}
