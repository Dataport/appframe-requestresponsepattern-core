using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Reflection;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Dataport.AppFrameDotNet.RequestResponsePattern.Exceptions;
using Dataport.AppFrameDotNet.RequestResponsePattern.Internal;
using Dataport.AppFrameDotNet.RequestResponsePattern.Logging;
using Microsoft.Extensions.Logging;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Core
{
    /// <summary>
    /// Basisklasse für einen Handler, der für einen bestimmten Request-Typ zuständig ist.
    /// </summary>
    /// <typeparam name="TRequest">Typ des Request</typeparam>
    /// <typeparam name="TRequestContext">Typ des Kontexts zur Ausführung.</typeparam>
    public abstract class HandlerCore<TRequest, TRequestContext> : IHandler<TRequest>
        where TRequest : IRequestBase
        where TRequestContext : IRequestContext, new()
    {
        /// <summary>
        /// ILogger-Instanz für diesen Handler.
        /// </summary>
        /// <returns>ILogger-Instanz für diesen Handler.</returns>
        protected ILogger Logger { get; private set; }

        /// <summary>
        /// Methode um die auszuführenden Aktionen zu ermitteln.
        /// </summary>
        /// <returns>Methode um die auszuführenden Aktionen zu ermitteln.</returns>
        protected abstract ActionDefinition<TRequest, TRequestContext>[] GetActionDefinitions();

        private ActionDefinition<TRequest, TRequestContext>[] _evaluationActions;
        private ActionDefinition<TRequest, TRequestContext>[] _executionActions;
        private ActionDefinition<TRequest, TRequestContext>[] _exitActions;
        private ActionDefinition<TRequest, TRequestContext>[] _errorActions;

        private readonly object _lock = new object();
        private bool _isInitialized;

        /// <summary>
        /// Initalisierung der Bearbeitungslogik.
        /// </summary>
        protected void Initialization()
        {
            Logger = Runtime.Current.CreateLogger(GetType());

            Logger.Log(LogLevel.Debug, $"[HandlerBase] Initialization of {GetType().FullName}");

            var actions = GetPredefinedActionDefinitions()
                .Concat(GetActionDefinitions())
                .Concat(GetExternalActionDefinitions())
                .ToList();

            //Aktions-Sequenz bis Ende der Evaluation festlegen
            _evaluationActions = FilterAndPreserveOrder(actions, ActionPosition.OnEnter, ActionPosition.OnEvaluatedWithSuccess);

            //Aktions-Sequenz bis Ende der Ausführung festlegen
            _executionActions = FilterAndPreserveOrder(actions, ActionPosition.OnEnter, ActionPosition.OnExecutedWithSuccess);

            //Aktions-Sequenz bei Ende der Ausführung
            _exitActions = FilterAndPreserveOrder(actions, ActionPosition.OnExit, ActionPosition.OnExit);

            //Aktions-Sequenz bei Fehler in der Ausführung
            _errorActions = FilterAndPreserveOrder(actions, ActionPosition.OnError, ActionPosition.OnError);

            if (!Logger.IsEnabled(LogLevel.Debug)) return;

            Logger.Log(LogLevel.Debug,
                $"Handler-Aktionssequenz Evaluation ist: \r\n{string.Join("\r\n", _evaluationActions.Select(x => x.Name))}");
            Logger.Log(LogLevel.Debug,
                $"Handler-Aktionssequenz Execution ist: \r\n{string.Join("\r\n", _executionActions.Select(x => x.Name))}");
            Logger.Log(LogLevel.Debug,
                $"Handler-Aktionssequenz Exit ist: \r\n{string.Join("\r\n", _exitActions.Select(x => x.Name))}");
            Logger.Log(LogLevel.Debug,
                $"Handler-Aktionssequenz Error ist: \r\n{string.Join("\r\n", _errorActions.Select(x => x.Name))}");
        }

        // "Eingebaute" Standardaktionen als erstes hinzufügen.
        // Werden dann auch als erstes ausgeführt (innerhalb der allgemeinen ActionPosition-Reihenfolge).
        private static ActionDefinition<TRequest, TRequestContext>[] GetPredefinedActionDefinitions()
        {
            return new[]
            {
                ValidateRequestActionDefinitionMetadata,
                ValidatePreStateActionDefinitionMetadata,
                ValidatePostStateActionDefinitionMetadata,
                ValidateResponseActionDefinitionMetadata
            };
        }

        // Durch externe Quellen bereitgestelle Definitionen hinzufügen.
        private IEnumerable<ActionDefinition<TRequest, TRequestContext>> GetExternalActionDefinitions()
        {
            return GetActionDefinitionsFromAttributes().Concat(GetActionDefinitionsFromRuntime());
        }

        // Plugin-Attribute auswerten.
        private IEnumerable<ActionDefinition<TRequest, TRequestContext>> GetActionDefinitionsFromAttributes()
        {
            return GetFromActionDefinitionsSource(GetType().GetCustomAttributes<ActionDefinitionAddInAttribute>(inherit: true));
        }

        // Globale "Plugins" aus Runtime übernehmen.
        private IEnumerable<ActionDefinition<TRequest, TRequestContext>> GetActionDefinitionsFromRuntime()
        {
            return GetFromActionDefinitionsSource(Runtime.Current.GlobalActionDefinitionSources);
        }

        // Definitionen aus externer Quelle übernehmen.
        private IEnumerable<ActionDefinition<TRequest, TRequestContext>> GetFromActionDefinitionsSource(IEnumerable<IActionDefinitionSource> sources)
        {
            return from source in sources
                   from actionDefinition in source.GetActionDefinitions()
                   select FromActionDefinitionsSource(actionDefinition, source);
        }

        // Konkrete Aktionen aus generischen Quellen erstellen.
        private ActionDefinition<TRequest, TRequestContext> FromActionDefinitionsSource(ActionDefinition<IRequestBase, IRequestContext> actionDefinition, IActionDefinitionSource source)
        {
            try
            {
                return new ActionDefinition<TRequest, TRequestContext>()
                {
                    Name = actionDefinition.Name,
                    ActionPosition = actionDefinition.ActionPosition,
                    Action = (request, state) => actionDefinition.Action(request, state)
                };
            }
            catch (Exception ex)
            {
                throw new HandlerInitializationException(
                    $"Handler {GetType().FullName}: Action {actionDefinition.Name} aus Quelle {source.GetType().FullName} konnte nicht registriert werden.",
                    ex);
            }
        }

        /// <summary>
        /// Prüft ob die Initialisierung durchgeführt wurde.
        /// </summary>
        protected void EnsureInitialization()
        {
            // Doppeltes IF dient der Vermeidung von Locks
            if (_isInitialized) return;

            lock (_lock)
            {
                if (_isInitialized) return;
                Initialization();
                _isInitialized = true;
            }
        }

        /// <inheritdoc />
        public void Evaluate(TRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            EnsureInitialization();

            using (var state = IntializeResponse(request))
            {
                InvokeActions(_evaluationActions, request, state);
                FinalizeResponse(request, state);
            }
        }

        /// <inheritdoc />
        public void Execute(TRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            EnsureInitialization();

            using (var state = IntializeResponse(request))
            {
                InvokeActions(_executionActions, request, state);
                FinalizeResponse(request, state);
            }
        }

        // Response erstellen und im Request hinterlegen.
        private TRequestContext IntializeResponse(TRequest request)
        {
            // Request detailiert ausgeben, wenn Trace eingeschaltet ist.
            if (Logger.IsEnabled(LogLevel.Trace))
                Logger.Log(LogLevel.Trace, request.ToTraceString());

            // Response instanzieren und am Request ablegen
            try
            {
                request.AddResponse(Activator.CreateInstance(request.ResponseType));
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Response {request.ResponseType.FullName} konnte nicht instanziert und dem Request {request.GetType().FullName} hinzugefügt werden.");
                throw;
            }

            // RequestState instanzieren
            try
            {
                return new TRequestContext()
                {
                    Logger = Logger
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"State-Objekt {typeof(TRequestContext).FullName} konnte nicht instanziert werden.");
                throw;
            }

        }

        // Response nach Abschluss des Handler-Ablaufs finalisieren.
        private void FinalizeResponse(TRequest request, TRequestContext state)
        {
            // Response und State detailiert ausgeben, wenn Trace eingeschaltet ist.
            if (!Logger.IsEnabled(LogLevel.Trace))
                return;

            Logger.Log(LogLevel.Trace, request.Response?.ToTraceString());
            request.Response?.Messages?.WriteToLog(Logger);
            Logger.Log(LogLevel.Trace, state?.ToTraceString());
        }

        // Ausführen der einzelnen Aktionen des Handlers.
        private void InvokeActions(IList<ActionDefinition<TRequest, TRequestContext>> actionList, TRequest request, TRequestContext state)
        {
            // Ermittelte Aktionen in definierter Reihenfolge durchführen
            var currentActionPosition = ActionPosition.None;
            foreach (var actionDefinition in actionList)
            {
                // bei einem Fehler wird nach der aktuellen Phase (ActionPosition) gestoppt.
                if (currentActionPosition != actionDefinition.ActionPosition && request.Response.Failed)
                {
                    // Liegt ein "weicher Fehler" vor, werden die OnExit-Aktionen noch ausgeführt.
                    break;
                }

                currentActionPosition = actionDefinition.ActionPosition;

                // Letzte Aktion im State-Objekt setzen
                state.LastAction = actionDefinition.Name;

                // ggf. Request-Status anpassen
                if (!request.Response.Executed && actionDefinition.ActionPosition >= ActionPosition.Implementation)
                    request.Response.Executed = true;

                try
                {
                    // Aktion ausführen
                    actionDefinition.Action(request, state);
                }
                catch (Exception ex)
                {
                    // Fehlerbehandlung inkl. der dafür vorgesehene Funktionen
                    ErrorHandler(ex, request, state);

                    // Ausstieg ohne weitere Verarbeitung
                    return;
                }

                // Automatisch: Fehlerzustand setzen bei Warnung "oder Schlimmer"
                if (request.Response.Messages.Any(x => x.EventLevel <= EventLevel.Warning))
                    request.Response.Failed = true;
            }

            // OnExit-Aktionen ausführen
            // Es werden alle Aktionen ungeachtet des Fehler-Status ausgeführt
            foreach (var actionDefinition in _exitActions)
            {
                try
                {
                    // Aktion ausführen
                    actionDefinition.Action(request, state);
                }
                catch (Exception ex)
                {
                    ErrorHandler(ex, request, state);
                    return;
                }
            }
        }

        // Fehlerbehandlung. Der Fehler wird im Response vermerkt und die für den Fehlerfall vorgesehenen Aktionen ausgeführt.
        private void ErrorHandler(Exception catchedEx, TRequest request, TRequestContext state)
        {
            Logger.Log(LogLevel.Error, catchedEx, "[HandlerBase]Exception at request execution");

            var exceptionMessage = catchedEx.AsResponseMessage();
            request.Response.Messages.Add(exceptionMessage);
            request.Response.Failed = true;

            state.Exception = catchedEx;

            foreach (var actionDefinition in _errorActions)
            {
                try
                {
                    // Aktion ausführen
                    actionDefinition.Action(request, state);
                }
                catch (Exception ex)
                {
                    Logger.Log(LogLevel.Error, ex, "[HandlerBase]Exception at error handling");
                    throw;
                }
            }
        }

        #region "Default Actions"

        /// <summary>
        /// Validierung des Requests (auf Basis seiner 'Annotations').
        /// </summary>
        private static readonly ActionDefinition<TRequest, TRequestContext>
          ValidateRequestActionDefinitionMetadata = new ActionDefinition<TRequest, TRequestContext>()
          {
              Name = "ValidateRequest",
              ActionPosition = ActionPosition.RequestValidation,
              Action = ValidateRequest
          };

        // Validierung des Requests (auf Basis seiner 'Annotations').
        private static void ValidateRequest(TRequest request, TRequestContext requestContext)
        {
            ExecuteValidation(request, request);
        }

        /// <summary>
        /// Validierung des Response (auf Basis seiner 'Annotations').
        /// </summary>
        private static readonly ActionDefinition<TRequest, TRequestContext>
            ValidateResponseActionDefinitionMetadata = new ActionDefinition<TRequest, TRequestContext>()
            {
                Name = "ValidateResponse",
                ActionPosition = ActionPosition.ResponseValidation,
                Action = ValidateResponse
            };

        // Validierung des Response (auf Basis seiner 'Annotations').
        private static void ValidateResponse(TRequest request, TRequestContext requestContext)
        {
            ExecuteValidation(request, request.Response);
        }

        /// <summary>
        /// Validierung des State vor Ausführung der Implementierung (auf Basis seiner 'Annotations').
        /// </summary>
        private static readonly ActionDefinition<TRequest, TRequestContext>
            ValidatePreStateActionDefinitionMetadata = new ActionDefinition<TRequest, TRequestContext>()
            {
                Name = "PreStateValidation",
                ActionPosition = ActionPosition.PostEvaluationRequestStateValidation,
                Action = ValidateRequestState
            };

        /// <summary>
        /// Validierung des State vor Ausführung der Implementierung (auf Basis seiner 'Annotations').
        /// </summary>
        private static readonly ActionDefinition<TRequest, TRequestContext>
            ValidatePostStateActionDefinitionMetadata = new ActionDefinition<TRequest, TRequestContext>()
            {
                Name = "PostStateValidation",
                ActionPosition = ActionPosition.PostImplementationRequestStateValidation,
                Action = ValidateRequestState
            };

        // Validierung des RequestState (auf Basis seiner 'Annotations').
        private static void ValidateRequestState(TRequest request, TRequestContext requestContext)
        {
            ExecuteValidation(request, requestContext);
        }

        // Basisimplementierung der Validierung.
        private static void ExecuteValidation<T>(IRequestBase request, T target)
        {
            // Validierung ausführen.
            var result = Validation.Validation.Validate(target);

            // Auf Response anwenden.
            request.Response.Failed = !result.IsValid;
            foreach (var responseMessage in result.ResponseMessages)
                request.Response.Messages.Add(responseMessage);
        }



        #endregion

        #region "Tools"

        // Reduziert die Gesamtmenge an Aktionen auf einen bestimmten Ausschnitt der Ausführung
        // unter Beibehaltung der Reihenfolge.
        private static ActionDefinition<TRequest, TRequestContext>[] FilterAndPreserveOrder(
            IList<ActionDefinition<TRequest, TRequestContext>> input,
            ActionPosition startActionPosition, ActionPosition stopActionPosition)
        {
            var output = new List<ActionDefinition<TRequest, TRequestContext>>();
            foreach (var actionPosition in ActionPositionTools.ActionPositions
                .Where(x => x >= startActionPosition && x <= stopActionPosition)
                .OrderBy(x => (int)x))
            {
                output.AddRange(input.Where(x => x.ActionPosition == actionPosition));
            }

            return output.ToArray();
        }

        #endregion
    }
}
