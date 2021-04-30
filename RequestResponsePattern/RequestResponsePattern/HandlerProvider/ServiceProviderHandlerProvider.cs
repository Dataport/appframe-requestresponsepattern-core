using System;
using System.Collections.Generic;
using System.Linq;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.HandlerProvider
{
    /// <summary>
    /// Ermittelt Handler mittels <see cref="IServiceProvider"/>.
    /// </summary>
    public class ServiceProviderHandlerProvider : IHandlerProvider
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="serviceProvider">ServiceProvider, aus dem die Handler bezogen werden sollen.</param>
        public ServiceProviderHandlerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <inheritdoc />
        public IEnumerable<IHandler<TRequest>> GetHandler<TRequest>() where TRequest : IRequestBase
        {
            //Hinweis: GetService() liefert ein registriertes Objekt sowohl bei
            //"IHandler<TRequest>" als auch bei "IEnumerable<IHandler<TRequest>>"
            return
                ((IEnumerable<IHandler<TRequest>>)_serviceProvider.GetService(typeof(IEnumerable<IHandler<TRequest>>)))
                .Where(x => x != null);
        }
    }
}
