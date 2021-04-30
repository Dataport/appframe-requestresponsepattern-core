using System;
using Microsoft.Extensions.Logging;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Core
{
    /// <summary>
    /// Request-Kontext-Objekt für Handler-Implementationen.
    /// Bietet Daten und ggf. Zugriff auf weitere "Werkzeuge" wie den Logger,
    /// die für die Ausführungskette des Request relevant sind, aber nicht Teil
    /// des Response sein dürfen.
    /// </summary>
    public interface IRequestContext : IDisposable
    {
        /// <summary>
        /// Logger, der im Rahmen des Requests verwendet werden kann.
        /// </summary>
        /// <returns>Logger, der im Rahmen des Requests verwendet werden kann.</returns>
        ILogger Logger { get; set; }

        /// <summary>
        /// Aufgetretener Fehler.
        /// </summary>
        /// <returns>Aufgetretener Fehler.</returns>
        Exception Exception { get; set; }

        /// <summary>
        /// Letzte ausgeführte "reguläre" Aktion (also nicht aus dem Block OnExit oder OnError).
        /// </summary>
        /// <returns>Letzte ausgeführte "reguläre" Aktion (also nicht aus dem Block OnExit oder OnError).</returns>
        string LastAction { get; set; }
    }
}