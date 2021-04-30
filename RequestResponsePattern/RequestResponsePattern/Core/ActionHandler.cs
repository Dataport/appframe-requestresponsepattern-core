using System;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Core
{
    /// <summary>
    /// Ermöglicht es, einen Handler vereinfacht durch eine Action zu implementieren.
    /// Macht im Wesentlichen bei UnitTests Sinn, um dort die Mockups bereitzustellen
    /// und on-the-fly als RuntimeListeners zu registrieren.
    /// </summary>
    /// <typeparam name="TRequest">Typ des Request</typeparam>
    public class ActionHandler<TRequest> : HandlerCore<TRequest, RequestContext>
        where TRequest : IRequestBase
    {
        private readonly ActionDefinition<TRequest, RequestContext> _implementationActionDefinition;

        /// <summary>
        /// Konstruktor.
        /// Erstellt einen "funktionslosen" Handler.
        /// </summary>
        public ActionHandler()
        {
            _implementationActionDefinition = new ActionDefinition<TRequest, RequestContext>()
            {
                Name = "Implementation",
                ActionPosition = ActionPosition.Implementation,
                Action = (request, state) => { }
            };
        }

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="implementationAction">Als Implementierung auszuführende Aktion.</param>
        public ActionHandler(Action<TRequest> implementationAction)
        {
            if (implementationAction == null) throw new ArgumentNullException(nameof(implementationAction));

            _implementationActionDefinition = new ActionDefinition<TRequest, RequestContext>()
            {
                Name = "Implementation",
                ActionPosition = ActionPosition.Implementation,
                Action = (request, state) => implementationAction(request)
            };
        }

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="implementationAction">Als Implementierung auszuführende Aktion.</param>
        public ActionHandler(Action<TRequest, RequestContext> implementationAction)
        {
            if (implementationAction == null) throw new ArgumentNullException(nameof(implementationAction));

            _implementationActionDefinition = new ActionDefinition<TRequest, RequestContext>()
            {
                Name = "Implementation",
                ActionPosition = ActionPosition.Implementation,
                Action = implementationAction
            };
        }

        /// <inheritdoc />
        protected override ActionDefinition<TRequest, RequestContext>[] GetActionDefinitions()
        {
            return new[] { _implementationActionDefinition };
        }
    }
}
