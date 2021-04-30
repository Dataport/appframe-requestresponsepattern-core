using System;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern
{
    /// <summary>
    /// Ermöglicht es einen Handler vereinfacht durch eine Action zu implementieren.
    /// Macht im Wesentlichen bei UnitTests Sinn, um dort die Mockups bereitzustellen
    /// und on-the-fly als RuntimeListeners zu registrieren.
    /// </summary>
    /// <typeparam name="TRequest">Typ des Request.</typeparam>
    /// <typeparam name="TResponse">Typ des zu instanzierenden Response.</typeparam>
    public class ActionHandler<TRequest, TResponse> : HandlerBase<TRequest, TResponse>
        where TResponse : ResponseBase, new() where TRequest : RequestBase<TResponse>
    {
        /// <summary>
        /// Konstruktor.
        /// </summary>
        public ActionHandler()
        { }

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="implementationAction">Als Implementierung auszuführende Aktion.</param>
        public ActionHandler(Action<TRequest> implementationAction)
        {
            ImplementationAction = implementationAction;
        }

        /// <summary>
        /// Als Implementierung auszuführende Aktion.
        /// </summary>
        /// <returns>Als Implementierung auszuführende Aktion.</returns>
        public Action<TRequest> ImplementationAction { get; set; }

        /// <inheritdoc />
        protected sealed override void Implementation(TRequest request)
        {
            ImplementationAction?.Invoke(request);
        }
    }
}
