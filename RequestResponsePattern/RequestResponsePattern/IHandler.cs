using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern
{
    /// <summary>
    /// Interface für Handler, die einen bestimmten Request ausführen.
    /// </summary>
    /// <typeparam name="TRequest">Typ des Requests, der vom Handler ausgeführt werden kann.</typeparam>
    public interface IHandler<in TRequest>
         where TRequest : IRequestBase
    {
        /// <summary>
        /// Führt den Request aus.
        /// </summary>
        /// <param name="request">Auszuführender Request.</param>
        void Execute(TRequest request);

        /// <summary>
        /// Prüft, ob sich der Request ausführen lassen könnte.
        /// </summary>
        /// <param name="request">Auszuführender Request.</param>
        void Evaluate(TRequest request);
    }
}
