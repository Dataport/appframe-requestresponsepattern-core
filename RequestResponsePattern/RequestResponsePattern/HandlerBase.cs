using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Dataport.AppFrameDotNet.RequestResponsePattern.Internal;
using Dataport.AppFrameDotNet.RequestResponsePattern.Logging;
using Dataport.AppFrameDotNet.RequestResponsePattern.Validation;
using Microsoft.Extensions.Logging;

namespace Dataport.AppFrameDotNet.RequestResponsePattern
{
    /// <summary>
    /// Basisklasse für einen Handler, der für einen bestimmten Request-Typ zuständig ist.
    /// </summary>
    /// <typeparam name="TRequest">Typ des Request.</typeparam>
    /// <typeparam name="TResponse">Typ des zu instanzierenden Response.</typeparam>
    public abstract class HandlerBase<TRequest, TResponse> : IHandler<TRequest>
        where TResponse : ResponseBase, new()
        where TRequest : RequestBase<TResponse>, IRequestBase, IRequest<TResponse>
    {
        // Sequenz der Aktionen, die bei einem 'Execute' ausgeführt wird.
        private readonly List<Action<TRequest>> _actionList = new List<Action<TRequest>>();

        // Liste der Plugins, die vor der eigentlichen Implementierung ausgeführt werden sollen.
        private readonly List<Tuple<PluginPosition, IPlugin<TRequest>>> _plugins = new List<Tuple<PluginPosition, IPlugin<TRequest>>>();

        /// <summary>
        /// Initialisierung der Bearbeitungslogik.
        /// </summary>
        protected void Initialization()
        {
            Logger = Runtime.Current.CreateLogger(GetType());

            Logger.Log(LogLevel.Debug, $"[HandlerBase] Initialization of {GetType().FullName}");

            InternalInitialization();

            CustomInitialization();

            _actionList.Add(OnEnter);

            _actionList.Add(ExecutePluginsAtPosition(PluginPosition.Enter));

            _actionList.Add(ValidateRequest);

            _actionList.Add(OnPreImplementation);

            _actionList.Add(ExecutePluginsAtPosition(PluginPosition.PreImplementation));

            _actionList.Add(PrepareImplementationCall);

            _actionList.Add(Implementation);

            _actionList.Add(OnPostImplementation);

            _actionList.Add(ExecutePluginsAtPosition(PluginPosition.PostImplementation));

            _actionList.Add(ValidateHandlerState);

            _actionList.Add(ValidateRequestState);

            _actionList.Add(ValidateResponse);

            _actionList.Add(OnSuccess);

            _actionList.Add(ExecutePluginsAtPosition(PluginPosition.OnSuccess));
        }

        private readonly object _lock = new object();
        private bool _isInitialized;

        /// <summary>
        /// Prüft, ob die Intialisierung durchgeführt wurde.
        /// </summary>
        protected void EnsureInitialization()
        {
            // Doppeltes IF dient der Vermeidung von Locks
            if (_isInitialized)
                return;

            lock (_lock)
            {
                if (_isInitialized)
                    return;

                Initialization();
                _isInitialized = true;
            }
        }

        /// <summary>
        /// Intern: Initialisierung
        /// </summary>
        private void InternalInitialization()
        {
            var pluginAttributes = GetType().GetCustomAttributes(typeof(PluginAttribute), true);

            foreach (PluginAttribute pluginAttribute in pluginAttributes)
            {
                var plugin = pluginAttribute.CreatePlugin();

                var typedPlugin = plugin as IPlugin<TRequest>;

                _plugins.Add(new Tuple<PluginPosition, IPlugin<TRequest>>(pluginAttribute.Position, typedPlugin));
            }
        }

        /// <summary>
        /// ILogger-Instanz für diesen Handler.
        /// </summary>
        /// <returns>Logger-Instanz</returns>
        protected ILogger Logger { get; private set; }

        /// <summary>
        /// Plugin progammatisch registrieren.
        /// </summary>
        /// <param name="plugin">Plugin.</param>
        /// <param name="pluginPosition">Ausführungszeitpunkt des Plugins.</param>
        public void RegisterPlugin(IPlugin<TRequest> plugin, PluginPosition pluginPosition)
        {
            if (plugin == null)
                throw new ArgumentNullException(nameof(plugin));

            if (!Enum.IsDefined(typeof(PluginPosition), pluginPosition))
                throw new InvalidEnumArgumentException(nameof(pluginPosition), (int)pluginPosition,
                    typeof(PluginPosition));

            Logger.Log(LogLevel.Debug,
                $"[HandlerBase] Register Plugin {plugin.GetType().FullName} on {GetType().FullName} at position {pluginPosition}");

            _plugins.Add(new Tuple<PluginPosition, IPlugin<TRequest>>(pluginPosition, plugin));
        }

        /// <summary>
        /// Die Methode die aufgerufen wird, wenn der Request ausgeführt werden soll.
        /// </summary>
        /// <param name="request">Auszuführender Request.</param>
        public void Execute(TRequest request)
        {
            try
            {
                if (request == null)
                    throw new ArgumentNullException(nameof(request));

                EnsureInitialization();

                Logger.Log(LogLevel.Debug, $"[HandlerBase] Execute {GetType().FullName}");

                Execute(request, _actionList);

            }
            finally
            {
                _requestState.Value = null;
            }
        }

        readonly ThreadLocal<State> _requestState = new ThreadLocal<State>();

        /// <summary>
        /// Request-spezifischer, interner Status des Handler.
        /// </summary>
        /// <returns>interner Status des Handler</returns>
        /// <remarks>...ist somit Thread-Safe.</remarks>
        protected dynamic RequestState => _requestState.Value.Properties;

        private readonly State _handlerState = new State();

        /// <summary>
        /// Handlerstatus.
        /// </summary>
        /// <returns>Handlerstatus.</returns>
        /// <remarks>...ist nicht Thread-Safe und kann somit statische Konfigurationsinformationen enthalten.</remarks>
        protected dynamic HandlerState => _handlerState.Properties;

        /// <summary>
        /// Die Methode die aufgerufen wird, wenn der Request auf Ausführbarkeit geprüft werden soll.
        /// </summary>
        /// <param name="request">Der zu prüfende Request</param>
        public void Evaluate(TRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            EnsureInitialization();

            Logger.Log(LogLevel.Debug, $"[HandlerBase] Evaluate {GetType().FullName}");

            using (_requestState.Value = new State())
            {
                Execute(request, _actionList.TakeWhile(x => x != PrepareImplementationCall));
            }

            _requestState.Value = null;
        }

        /// <summary>
        /// Die Methode die aufgerufen wird, wenn der Request ausgeführt werden soll.
        /// </summary>
        /// <param name="request">Auszuführender Request.</param>
        /// <param name="actualActions">Aktionen, die ausgeführt werden sollen.</param>
        private void Execute(TRequest request, IEnumerable<Action<TRequest>> actualActions)
        {
            // Request detailiert ausgeben, wenn Trace eingeschaltet ist
            if (Logger.IsEnabled(LogLevel.Trace))
                Logger.Log(LogLevel.Trace, request.ToTraceString());

            using (_requestState.Value = new State())
            {
                var response = new TResponse();
                request.AddResponse(response);

                foreach (var action in actualActions)
                {
                    try
                    {
                        // Aktion ausführen
                        action(request);
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler(ex, request);
                    }

                    if (request.Response.Metadata.Failed)
                    {
                        OnFailure(request);
                        ExecutePluginsAtPosition(PluginPosition.OnFailure)(request);
                        break;
                    }
                }

                OnExit(request);

                ExecutePluginsAtPosition(PluginPosition.Exit)(request);

                // Response detailiert ausgeben, wenn Trace eingeschaltet ist
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.Log(LogLevel.Trace, request.Response?.ToTraceString());
                    request.Response?.Metadata?.Messages?.WriteToLog(Logger);
                }
            }
        }

        /// <summary>
        /// Fehlerbehandlung. Der Fehler wird im Response vermerkt und die für den Fehlerfall vorgesehenen Plugins ausgeführt.
        /// </summary>
        /// <param name="catchedEx">Zu behandelnde Exception.</param>
        /// <param name="request">Request, in dem die Exception vermerkt werden soll.</param>
        private void ErrorHandler(Exception catchedEx, TRequest request)
        {
            Logger.Log(LogLevel.Error, catchedEx, "[HandlerBase]Exception at request execution");

            var exceptionMessage = catchedEx.AsResponseMessage();
            request.Response.AddMessage(exceptionMessage);
            request.Response.Metadata.Failed = true;

            try
            {
                OnError(request, catchedEx, exceptionMessage);
                ExecutePluginsAtPosition(PluginPosition.OnError)(request);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex, "[HandlerBase]Exception at error handling");
                throw;
            }
        }

        // Basisimplementierung der Validierung.
        private void ExecuteValidation<T>(TRequest request, T target, Func<TRequest, T, IEnumerable<IResponseMessage>> extender)
        {
            // Validierung ausführen.
            var result = Validation.Validation.Validate(target)
                .Add(extender?.Invoke(request, target).ToArray());

            // Unterdrückbare Validierungsmeldungen berücksichtigen.
            HandleOmittableValidationMessages(request, result);

            // Auf Response anwenden.
            request.Response.Metadata.Failed = !result.IsValid;
            request.Response.AddMessages(result.ResponseMessages);
        }

        // Verarbeiten von Validierungsmeldungen, die optional sind und vom
        // Benutzer unterdrückt worden sein könnten (entsprechend im Request vermerkt).
        private void HandleOmittableValidationMessages(TRequest request, ValidationResponseCollection validationResult)
        {
            // Defensiv: null-Werte normalisieren auf leere Mengen.
            var omittedSourceIds = request.OmitValidationSourceIds?.ToArray() ?? Array.Empty<string>();
            var omittableSourceIds = GetOmittableValidationSourceIds(request)?.ToArray() ?? Array.Empty<string>();

            // Prüfen, dass nur erlaubte Validierungsmeldungen unterdrückt werden sollen.
            var invalidSourceIds = omittedSourceIds.Where(id => !omittableSourceIds.Contains(id)).ToList();
            if (invalidSourceIds.Any())
            {
                validationResult.Add(ResponseMessage.CreateWarning("82906E4F-58E3-4FC0-9EC6-AE9243E68C2F",
                    $"Es ist nicht erlaubt, die angegebenen Validierungsmeldungen zu unterdrücken: {string.Join(", ", invalidSourceIds)}"));
            }

            // Messages "markieren", die vom Aufrufer unterdrückt werden können.
            foreach (var message in validationResult.ResponseMessages.Where(
                m => omittableSourceIds.Contains(m.SourceId)))
            {
                message.IsOmittableValidation = true;
            }

            // Unterdrückte Validierungsmeldungen entfernen und merken.
            var omittedMessages = validationResult.Remove(m => omittedSourceIds.Contains(m.SourceId));
            AddOmittedValidationMessages(omittedMessages);
        }

        // Validierung des Request bevor weitere Aktionen passieren.
        private void ValidateRequest(TRequest request)
        {
            ExecuteValidation(request, request, (req, target) => ExtendedRequestValidation(request));
        }

        /// <summary>
        /// Möglichkeit, weitere Validierung programmatisch durchzuführen.
        /// </summary>
        /// <param name="request">Auszuführender Request.</param>
        /// <returns>Validierungsmeldungen; oder eine leere Sequenz, wenn es keine Validierungsfehler gab.</returns>
        protected virtual IEnumerable<IResponseMessage> ExtendedRequestValidation(TRequest request) =>
            Enumerable.Empty<IResponseMessage>();

        /// <summary>
        /// Kann überschrieben werden, um Validierungsmeldungen festzulegen,
        /// die vom Anwender unterdrückt werden können.
        /// </summary>
        /// <param name="request">Aktuell verarbeiteter Request.</param>
        /// <returns>Eine Sequenz mit den Source IDs der unterdrückbaren Validierungsmeldungen.</returns>
        protected virtual IEnumerable<string> GetOmittableValidationSourceIds(TRequest request)
            => Enumerable.Empty<string>();

        /// <summary>
        /// Gibt alle Validierungsmeldungen zurück, die in der aktuellen Ausführung
        /// festgestellt, aber vom Benutzer explizit unterdrückt wurden.
        /// </summary>
        /// <returns>Validierungsmeldungen, die in der aktuellen Ausführung festgestellt, aber vom Benutzer explizit unterdrückt wurden.</returns>
        protected IEnumerable<IResponseMessage> OmittedValidationMessages
            // In RequestState vorhalten; Fallback auf leere Menge.
            => _requestState.Value.PropertiesByName.ContainsKey(nameof(OmittedValidationMessages))
                ? RequestState.OmittedValidationMessages
                : Enumerable.Empty<IResponseMessage>();


        // Vom Benutzer explizit unterdrückte Meldungen merken.
        private void AddOmittedValidationMessages(IEnumerable<IResponseMessage> messages)
        {
            RequestState.OmittedValidationMessages = OmittedValidationMessages
                .Concat(messages).ToArray();
        }

        // Validierung des Handler-State.
        private void ValidateHandlerState(TRequest request)
        {
            ExecuteValidation(request, _handlerState, (r, s) => s.SelfValidation());
        }

        // Validierung des Request-State.
        private void ValidateRequestState(TRequest request)
        {
            ExecuteValidation(request, _requestState.Value, (r, s) => s.SelfValidation());
        }

        // Validierung des Response bevor die Rückgabe erfolgt.
        private void ValidateResponse(TRequest request)
        {
            ExecuteValidation(request, request.Response, (req, target) => ExtendedResponseValidation(request));
        }

        /// <summary>
        /// Möglichkeit weitere Validierung programmatisch durchzuführen.
        /// </summary>
        /// <param name="request">Ausgeführter Request mit dem Response der Ausführung.</param>
        /// <returns>Validierungsmeldungen; oder eine leere Sequenz, wenn es keine Validierungsfehler gab.</returns>
        protected virtual IEnumerable<IResponseMessage> ExtendedResponseValidation(TRequest request) =>
            Enumerable.Empty<IResponseMessage>();

        // Erstellt eine Action zur Ausführung von Plugins an einer bestimmten Position im Ablauf.
        private Action<TRequest> ExecutePluginsAtPosition(PluginPosition position)
            => request => ExecutePlugins(position, request);

        // Basismethode zum Ausführen einer Liste von Plugins.
        private void ExecutePlugins(PluginPosition position, TRequest request)
        {
            var plugins = _plugins.Where(x => x.Item1 == position).Select(x => x.Item2);

            foreach (var plugin in plugins)
            {
                try
                {
                    plugin.Execute(request, _requestState.Value);
                }
                catch (Exception ex)
                {
                    request.Response.AddMessage(ex.AsResponseMessage());
                    request.Response.Metadata.Failed = true;
                }

                if (request.Response.Metadata.Failed) break;
            }
        }

        /// <summary>
        /// Vorbereitende Maßnahmen vor der Ausführen der Implementierung.
        ///  - Response.Executed auf True setzen
        /// </summary>
        /// <param name="request">Auszuführender Request.</param>
        private void PrepareImplementationCall(TRequest request)
        {
            request.Response.Metadata.Executed = true;
        }

        /// <summary>
        /// Add your Code here! -- Hier ist in der abgeleiteten Klasse die eigentliche 
        /// Business Logik zu implementieren.
        /// </summary>
        /// <param name="request">Auszuführender Request.</param>
        protected abstract void Implementation(TRequest request);

        /// <summary>
        /// Eigene Initialisierungsschritte für den Handler ausführen.
        /// </summary>
        /// <remarks>Plugings hier über RegisterPlugin() hinzufügen!</remarks>
        protected virtual void CustomInitialization()
        {
            // Keine Basisimplementierung
        }

        #region "On-Methoden"

        /// <summary>
        /// Wird vor 'ExecuteEnterPlugins' ausgeführt.
        /// </summary>
        /// <param name="request">Auszuführender Request.</param>
        protected virtual void OnEnter(TRequest request)
        { }

        /// <summary>
        /// Wird vor 'PreImplementationPlugins' ausgeführt.
        /// </summary>
        /// <param name="request">Auszuführender Request.</param>
        protected virtual void OnPreImplementation(TRequest request)
        { }

        /// <summary>
        /// Wird vor 'PostImplementationPlugins' ausgeführt.
        /// </summary>
        /// <param name="request">Auszuführender Request.</param>
        protected virtual void OnPostImplementation(TRequest request)
        { }

        /// <summary>
        /// Wird vor 'OnSuccessPlugins' ausgeführt.
        /// </summary>
        /// <param name="request">Auszuführender Request.</param>
        protected virtual void OnSuccess(TRequest request)
        { }

        /// <summary>
        /// Wird vor 'OnFailurePlugins' ausgeführt.
        /// </summary>
        /// <param name="request">Auszuführender Request.</param>
        protected virtual void OnFailure(TRequest request)
        { }

        /// <summary>
        /// Wird vor 'ExitPlugins' ausgeführt.
        /// </summary>
        /// <param name="request">Auszuführender Request.</param>
        protected virtual void OnExit(TRequest request)
        { }

        /// <summary>
        /// Wird im ErrorHandler ausgeführt.
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="ex">Ausgelöste Exception</param>
        /// <param name="errorMessage">Erstellte Message zur Exception</param>
        protected virtual void OnError(TRequest request, Exception ex, ResponseMessage errorMessage)
        { }

        #endregion
    }
}
