using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Dataport.AppFrameDotNet.RequestResponsePattern.Core;
using Dataport.AppFrameDotNet.RequestResponsePattern.HandlerProvider;
using Dataport.AppFrameDotNet.RequestResponsePattern.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Dataport.AppFrameDotNet.RequestResponsePattern
{
    /// <summary>
    /// Einstellungen, die zur Laufzeit verwendet werden sollen.
    /// </summary>
    public class Runtime
    {
        #region "Static Context"

        private static Runtime _current;
        private static readonly object Lock = new object();

        private static readonly ThreadLocal<Runtime> LocalRuntime = new ThreadLocal<Runtime>();

        internal static void SetLocalRuntime(Runtime localRuntime)
        {
            LocalRuntime.Value = localRuntime;
        }

        /// <summary>
        /// Aktuelle Settings für diese Bibliothek.
        /// </summary>
        /// <returns>Aktuelle Settings für diese Bibliothek.</returns>
        public static Runtime Current
        {
            get
            {
                // Runtime aus Using-Block wenn vorhanden
                if (LocalRuntime.Value != null)
                    return LocalRuntime.Value;

                // ...ansonsten, wie gewohnt, die "statische Fassade"
                EnsureInitialization();
                return _current;
            }
            set => _current = value ?? throw new NullReferenceException();
        }

        // Ausführung der Initialisierung, falls noch nicht geschehen.
        private static void EnsureInitialization()
        {
            if (_current != null) return;
            lock (Lock)
            {
                if (_current != null) return;
                _current = new Runtime();
            }
        }

        #endregion

        private readonly IServiceProvider _serviceProvider;
        private ILoggerFactory _loggerFactory;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        public Runtime()
        {
            Create(null, null, null, null);
        }

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="loggerFactory">Logger Factory (optional)</param>
        /// <param name="validator">Implementierung der Validierung (optional, bei "null" wird System.Annotation verwendet)</param>
        /// <param name="handlerProviders">Zusätzliche HandlerProvider (z.B. aus dem WebApiClient-Paket)</param>
        public Runtime(ILoggerFactory loggerFactory = null, IValidator validator = null, params IHandlerProvider[] handlerProviders)
        {
            Create(loggerFactory, validator, handlerProviders, null);
        }

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="loggerFactory">Logger Factory (optional)</param>
        /// <param name="validator">Implementierung der Validierung (optional, bei "null" wird System.Annotation verwendet)</param>
        /// <param name="handlerProviders">Zusätzliche HandlerProvider (z.B. aus dem WebApiClient-Paket)</param>
        /// <param name="globalActionDefinitionSources">Globale "Plugins" die ALLEN Core-Handlern Aktionen hinzufügen. (Neue Implementierung)</param>
        public Runtime(ILoggerFactory loggerFactory = null, IValidator validator = null, IEnumerable<IActionDefinitionSource> globalActionDefinitionSources = null, IEnumerable<IHandlerProvider> handlerProviders = null)
        {
            Create(loggerFactory, validator, handlerProviders, globalActionDefinitionSources);
        }

        /// <summary>
        /// Konstruktor (Abhängigkeiten werden via IServiceProvider aufgelöst).
        /// </summary>
        /// <param name="serviceProvider">ServiceProvider (z.B. host.Services in DotNetCore WebApi)</param>
        public Runtime(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            Create((ILoggerFactory)serviceProvider.GetService(typeof(ILoggerFactory)),
                (IValidator)serviceProvider.GetService(typeof(IValidator)),
                (IEnumerable<IHandlerProvider>)serviceProvider.GetService(typeof(IEnumerable<IHandlerProvider>)),
                (IEnumerable<IActionDefinitionSource>)serviceProvider.GetService(typeof(IEnumerable<IActionDefinitionSource>)));
        }

        private void Create(ILoggerFactory loggerFactory, IValidator validator, IEnumerable<IHandlerProvider> handlerProviders,
            IEnumerable<IActionDefinitionSource> globalActionDefinitionSources)
        {
            _loggerFactory = loggerFactory;
            Validator = validator ?? new DataAnnotationValidator();
            _handlerProviderList = GetHandlerProvider().Concat(handlerProviders ?? Array.Empty<IHandlerProvider>()).ToList();
            GlobalActionDefinitionSources = globalActionDefinitionSources ?? Array.Empty<IActionDefinitionSource>();
        }

        /// <summary>
        /// Globale "Plugins" die ALLEN Core-Handlern Aktionen hinzufügen. (Neue Implementierung)
        /// </summary>
        /// <returns>Globale "Plugins" die ALLEN Core-Handlern Aktionen hinzufügen.</returns>
        public IEnumerable<IActionDefinitionSource> GlobalActionDefinitionSources { get; private set; }

        // Interne Funktion zum Erstellen eines Loggers.
        internal ILogger CreateLogger<T>()
        {
            return (ILogger)_loggerFactory?.CreateLogger<T>() ?? NullLogger.Instance;
        }

        // Interne Funktion zum Erstellen eines Loggers.
        internal ILogger CreateLogger(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            return _loggerFactory?.CreateLogger(type) ?? NullLogger.Instance;
        }

        private IList<IHandlerProvider> _handlerProviderList;

        /// <summary>
        /// Fügt einen neuen HandlerProvider hinzu, der zum Erzeugen von
        /// Handlern zu Requests herangezogen werden soll.
        /// </summary>
        /// <param name="handlerProvider">HandlerProvider, der hinzugefügt werden soll.</param>
        public void AddHandlerProvider(IHandlerProvider handlerProvider)
        {
            if (handlerProvider == null) throw new ArgumentNullException(nameof(handlerProvider));

            _handlerProviderList.Add(handlerProvider);
        }

        /// <summary>
        /// Setzt die Liste der verwendeten HandlerProvider auf den
        /// Standardzustand zurück (entfernt alle nachträglich hinzugefügten HandlerProvider).
        /// </summary>
        public void ReinitializeHandlerProviderList()
        {
            _handlerProviderList.Clear();

            foreach (var defaultHandlerProvider in GetHandlerProvider())
            {
                _handlerProviderList.Add(defaultHandlerProvider);
            }
        }

        // Ausgeben aller bekannten HandlerProvider.
        internal IList<IHandlerProvider> GetHandlerProvider()
        {
            var list = new List<IHandlerProvider>() { RuntimeListeners.Global };
            if (_serviceProvider != null)
                list.Add(new ServiceProviderHandlerProvider(_serviceProvider));
            return list;
        }

        // Gibt alle registrierten Handler für einen bestimmten Request-Typen zurück.
        internal IEnumerable<IHandler<TRequest>> GetHandler<TRequest>()
            where TRequest : IRequestBase
        {
            return _handlerProviderList.SelectMany(x => x.GetHandler<TRequest>());
        }

        /// <summary>
        /// Validationsimplementierung. (System.Annotation ist Default)
        /// </summary>
        /// <returns>Validationsimplementierung. (System.Annotation ist Default)</returns>
        public IValidator Validator { get; private set; }
    }
}
