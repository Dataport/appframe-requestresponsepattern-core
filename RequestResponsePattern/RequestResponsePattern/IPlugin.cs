using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern
{
    /// <summary>
    /// Interface für die Implementierung eines Plugins.
    /// </summary>
    /// <typeparam name="TRequest">Typ des Requests, auf dessen Ausführung das Plugin angewendet werden kann.</typeparam>
    public interface IPlugin<in TRequest>
         where TRequest : IRequestBase
    {
        /// <summary>
        /// Führt das Plugin aus.
        /// </summary>
        /// <param name="request">Auszuführender Request.</param>
        /// <param name="requestState">State-Objekt mit Kontextinformationen zur aktuellen Ausführung des Requests.</param>
        void Execute(TRequest request, State requestState);
    }
}
