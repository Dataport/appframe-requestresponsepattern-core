using System;
using System.Collections.Generic;
using System.Linq;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Dataport.AppFrameDotNet.RequestResponsePattern.HandlerProvider;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Testobjekte
{
    /// <summary>
    /// Mockup-Implementierung eines HandlerProviders für die Testumgebung.
    /// Stellt vordefinierte Instanzen für entsprechende Request-Typen bereit.
    /// </summary>
    public class MyHandlerProvider : IHandlerProvider
    {
        private static readonly MyHandler MyHandler = new MyHandler();
        private static readonly MyMultipleHandler1 MyMultipleHandler1 = new MyMultipleHandler1();
        private static readonly MyMultipleHandler2 MyMultipleHandler2 = new MyMultipleHandler2();

        /// <summary>
        /// 0-n Handler bereitstellen.
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <returns></returns>
        public IEnumerable<IHandler<TRequest>> GetHandler<TRequest>() where TRequest : IRequestBase
        {
            foreach (var handler in GetHandlerInternal(typeof(TRequest)))
            {
                yield return (IHandler<TRequest>)handler;
            }
        }

        private IEnumerable<object> GetHandlerInternal(Type requestType)
        {
            if (requestType == typeof(MyRequest)) return new[] { MyHandler };
            if (requestType == typeof(MyMultipleRequest)) return new object[] { MyMultipleHandler1, MyMultipleHandler2 };

            return Enumerable.Empty<object>();
        }
    }
}