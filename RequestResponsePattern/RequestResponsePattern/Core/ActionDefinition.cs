using System;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Core
{
    /// <summary>
    /// Definiert eine Aktion innerhalb der Ausführungskette des Handler.
    /// </summary>
    /// <typeparam name="TRequest">RequestTyp.</typeparam>
    /// <typeparam name="TRequestContext">RequestState-Typ.</typeparam>
    public struct ActionDefinition<TRequest, TRequestContext>
        where TRequest : IRequestBase
        where TRequestContext : IRequestContext
    {
        /// <summary>
        /// Technischer Name der Aktion (für Protokollierung, etc.).
        /// </summary>
        /// <returns>Technischer Name der Aktion (für Protokollierung, etc.).</returns>
        public string Name { get; set; }

        /// <summary>
        /// Positionierung innerhalb der Ausführungskette.
        /// </summary>
        /// <returns>Positionierung innerhalb der Ausführungskette.</returns>
        public ActionPosition ActionPosition { get; set; }

        /// <summary>
        /// Implementierung der Aktion.
        /// </summary>
        /// <returns>Implementierung der Aktion.</returns>
        public Action<TRequest, TRequestContext> Action { get; set; }
    }
}
