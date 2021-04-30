using System.Collections.Generic;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.HandlerProvider
{
    /// <summary>
    /// Interface für Strategien zur Bereitstellung von Handlern.
    /// </summary>
    public interface IHandlerProvider
    {
        /// <summary>
        /// '0' bis 'n' Handler bereitstellen.
        /// </summary>
        /// <typeparam name="TRequest">Typ des Requests, der durch die Handler verarbeitet wird.</typeparam>
        /// <returns>Eine Sequenz mit den Handlern, die den Request-Typen verarbeiten.</returns>
        IEnumerable<IHandler<TRequest>> GetHandler<TRequest>() where TRequest : IRequestBase;
    }
}
