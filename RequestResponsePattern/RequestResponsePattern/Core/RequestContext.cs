using System;
using Microsoft.Extensions.Logging;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Core
{
    /// <summary>
    /// Standardimplementation eines Request-Kontext-Objekts für Handler-Implementationen.
    /// Bietet Daten und ggf. Zugriff auf weitere "Werkzeuge" wie den Logger,
    /// die für die Ausführungskette des Request relevant sind, aber nicht Teil
    /// des Response sein dürfen.
    /// </summary>
    public class RequestContext : IRequestContext
    {
        private bool _disposed;

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <inheritdoc />
        public Exception Exception { get; set; }

        /// <inheritdoc />
        public string LastAction { get; set; }

        /// <summary>
        /// Überschreiben um Dispose-Aktion durchzuführen.
        /// </summary>
        /// <param name="disposing">True, falls Dispose explizit aufgerufen wird; false im Rahmen eines Destruktor-Aufrufs.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destruktor zum automatischen Aufräumen von Ressourcen.
        /// </summary>
        ~RequestContext() => Dispose(false);
    }
}
