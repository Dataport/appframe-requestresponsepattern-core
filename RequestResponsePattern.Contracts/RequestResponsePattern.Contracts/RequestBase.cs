using System;
using System.Collections.Generic;
using System.Linq;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Contracts
{
    /// <summary>
    /// Basisklasse für Requests.
    /// </summary>
    /// <typeparam name="T">Request-Typ</typeparam>
    public abstract class RequestBase<T> : IRequest<T>
        where T : ResponseBase, IResponse
    {
        /// <summary>
        /// Eindeutige ID der Anfrage.
        /// </summary>
        /// <returns>GUID der Anfrage</returns>
        Guid IRequestBase.Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Menge der Source IDs von Validierungsmeldungen, die
        /// bei der Ausführung unterdrückt werden sollen (vom Anwender "quittiert").
        /// </summary>
        /// <returns>Aufzählung der Source-IDs</returns>
        IEnumerable<string> IRequestBase.OmitValidationSourceIds { get; set; }

        /// <summary>
        /// Ergebnis, falls die Anfrage ausgeführt wurde.
        /// Bei Mehrfachausführung das letzte Ergebnis.
        /// </summary>
        /// <returns>Das (letzte) Ergebnis der Anfrage</returns>
        public T Response => _responses.FirstOrDefault();

        /// <summary>
        /// Stack mit Anfrage-Ergebnissen
        /// </summary>
        /// <returns>Das letzte Ergebnis der Anfrage</returns>
        IResponse IRequestBase.Response => _responses.FirstOrDefault();

        readonly Stack<T> _responses = new Stack<T>();

        /// <summary>
        /// Alle Ergebnisse in zeitlich absteigender Reihenfolge. 
        /// Bei Mehrfachausführungen relevant.
        /// </summary>
        /// <returns>Alle Ergebnisse (absteigend)</returns>
        public IEnumerable<T> Responses => _responses.AsEnumerable();

        /// <summary>
        /// Alle Ergebnisse in zeitlich absteigender Reihenfolge. 
        /// Bei Mehrfachausführungen relevant.
        /// </summary>
        /// <returns>Alle Ergebnisse (absteigend)</returns>
        IEnumerable<IResponse> IRequestBase.Responses => _responses.AsEnumerable();

        /// <summary>
        /// Typ der Responses.
        /// </summary>
        /// <returns>Response-Typ</returns>
        Type IRequestBase.ResponseType => typeof(T);

        /// <summary>
        /// Response bei Ausführung eines Handlers hinzufügen.
        /// </summary>
        /// <param name="response">Typisierte Response</param>
        internal void AddResponse(T response)
        {
            _responses.Push(response);
        }

        /// <summary>
        /// Response bei Ausführung eines Handlers hinzufügen.
        /// </summary>
        /// <param name="response">untypisierte Response</param>
        /// <remarks>Außenschnittstelle für Erweiterungen.</remarks>
        public void AddResponse(object response)
        {
            _responses.Push((T)response);
        }

        /// <summary>
        /// Zugriff auf die allgemeinen Request-Metadaten.
        /// </summary>
        /// <returns>Metadaten</returns>
        /// <remarks>Diese könenn auch mit einem manuellem Cast auf IRequestBase sichtbar gemacht werden.</remarks>
        public IRequestBase Metadata => this;
    }
}
