using System.Collections.Generic;
using System.Linq;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.HandlerProvider
{
    /// <summary>
    /// Vereinfacht die Definition eines Handler-Providers,
    /// der einen bestimmten Request-Typen bedient.
    /// </summary>
    /// <typeparam name="TRequest">Typ des Requests, der vom HandlerProvider bedient wird.</typeparam>
    public abstract class HandlerProviderForRequest<TRequest>
        : IHandlerProvider where TRequest : IRequestBase
    {
        /// <inheritdoc />
        public IEnumerable<IHandler<T>> GetHandler<T>()
            where T : IRequestBase
        {
            // Nur bei passendem Request-Typen den Handler zurückgeben.
            return typeof(TRequest).IsAssignableFrom(typeof(T))
                ? GetHandlerForRequest().Cast<IHandler<T>>().ToList()
                : Enumerable.Empty<IHandler<T>>();
        }

        /// <summary>
        /// Wird überschrieben, um zu definieren, welche Handler die Requests verarbeiten soll.
        /// </summary>
        /// <returns>Den Handler, der die Requests verarbeiten soll.</returns>
        protected abstract IEnumerable<IHandler<TRequest>> GetHandlerForRequest();
    }
}