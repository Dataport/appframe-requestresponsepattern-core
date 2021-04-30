using System;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Core
{
    /// <summary>
    /// Definiert einen Handler auf Basis der im Konstruktor übergebenen Aktionen.
    /// </summary>
    /// <typeparam name="TRequest">Typ des Requests.</typeparam>
    /// <typeparam name="TRequestContext">Typ des Kontexts zur Ausführung.</typeparam>
    public sealed class Handler<TRequest, TRequestContext> : HandlerCore<TRequest, TRequestContext>
        where TRequest : IRequestBase
        where TRequestContext : IRequestContext, new()
    {
        private readonly ActionDefinition<TRequest, TRequestContext>[] _definedActions;

        /// <summary>
        /// Erstellt den Handler.
        /// </summary>
        /// <param name="definedActions">Zu verwendende Aktionen.</param>
        public Handler(params ActionDefinition<TRequest, TRequestContext>[] definedActions)
        {
            _definedActions = definedActions ?? throw new ArgumentNullException(nameof(definedActions));
        }

        /// <inheritdoc />
        protected override ActionDefinition<TRequest, TRequestContext>[] GetActionDefinitions()
        {
            return _definedActions;
        }
    }
}
