using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.HandlerProvider
{
    /// <summary>
    /// Zur Laufzeit registrierte Handler, die einen bestimmten Request behandeln sollen.
    /// </summary>
    public class RuntimeListeners : IHandlerProvider, IDisposable
    {
        private static readonly ThreadLocal<List<RuntimeListeners>> LocalListeners = new ThreadLocal<List<RuntimeListeners>>(() => new List<RuntimeListeners>());
        private readonly List<Tuple<Type, object>> _listeners = new List<Tuple<Type, object>>();

        // Kennzeichnung der Hauptinstanz, die für die tatsächliche Bereitstellung der Handler erforderlich ist.
        // Alle weiteren Instanzen sammeln lediglich weitere Handler, werden aber nicht als HandlerProvider registriert.
        // Die Bereitstellung übernimmt nur die Hauptinstanz.
        private bool Main { get; set; }

        /// <summary>
        /// Zur Laufzeit zu verwendendes Singleton für global bereitgestellte Instanzen.
        /// </summary>
        /// <returns>Zur Laufzeit zu verwendendes Singleton für global bereitgestellte Instanzen.</returns>
        public static RuntimeListeners Global { get; } = new RuntimeListeners() { Main = true };

        /// <summary>
        /// Konstruktor.
        /// </summary>
        internal RuntimeListeners()
        { }

        /// <summary>
        /// Per Using-Direktive zu verwendendes lokales - sprich thread-spezifisches(!) - Set.
        /// </summary>
        /// <returns>Das Set, in dem weitere Handler innerhalb eines using-Blocks ergänzt werden können.</returns>
        public static RuntimeListeners CreateLocalSet()
        {
            var set = new RuntimeListeners();
            LocalListeners.Value.Add(set);
            return set;
        }

        /// <summary>
        /// Handler als Listener zufügen.
        /// </summary>
        /// <typeparam name="TRequest">Typ des Requests, der durch den Handler verarbeitet werden soll.</typeparam>
        /// <param name="handler">Handler, der hinzugefügt werden soll.</param>
        public void AddListener<TRequest>(IHandler<TRequest> handler)
            where TRequest : IRequestBase
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            _listeners.Add(new Tuple<Type, object>(typeof(TRequest), handler));
        }

        /// <summary>
        /// Handler als Listener entfernen.
        /// </summary>
        /// <typeparam name="TRequest">Typ des Requests, der durch den Handler verarbeitet wird.</typeparam>
        /// <param name="handler">Handler, der die Requests nicht weiter verarbeiten soll.</param>
        public void RemoveListener<TRequest>(IHandler<TRequest> handler)
            where TRequest : IRequestBase
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            var entry = _listeners.SingleOrDefault(x => x.Item2 == handler);

            if (entry == null)
                throw new ArgumentOutOfRangeException(nameof(handler), "listener ist nicht registriert.");

            _listeners.Remove(entry);
        }

        /// <summary>
        /// Alle Listener löschen.
        /// </summary>
        public void ClearListeners()
        {
            _listeners.Clear();
        }

        /// <inheritdoc />
        IEnumerable<IHandler<TRequest>> IHandlerProvider.GetHandler<TRequest>()
        {
            if (Main)
            {
                return
                    GetHandlerInternal<TRequest>()
                        .Concat(LocalListeners.Value.SelectMany(x => x.GetHandlerInternal<TRequest>()))
                        .ToArray();
            }

            return GetHandlerInternal<TRequest>().ToArray();
        }

        // Handler für den Request-Typen aus lokaler Liste suchen.
        private IEnumerable<IHandler<TRequest>> GetHandlerInternal<TRequest>() where TRequest : IRequestBase
        {
            return _listeners
              .Where(x => x.Item1 == typeof(TRequest))
              .Select(x => (IHandler<TRequest>)x.Item2);
        }

        /// <inheritdoc />
        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;

        /// <summary>
        /// Intern: Per Using-Direktive bereitgestellte Auflistung entfernen.
        /// </summary>
        /// <param name="disposing">True, falls Dispose explizit aufgerufen wird; false im Rahmen eines Destruktor-Aufrufs.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (LocalListeners.Value.Contains(this))
                        LocalListeners.Value.Remove(this);
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// Destruktor zum automatischen Aufräumen von Ressourcen.
        /// </summary>
        ~RuntimeListeners() => Dispose(false);
    }
}