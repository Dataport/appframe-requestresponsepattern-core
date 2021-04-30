using System;

namespace Dataport.AppFrameDotNet.RequestResponsePattern
{
    /// <summary>
    /// Extension Methoden für das Bootstrapping.
    /// </summary>
    public static class RuntimeExtensions
    {
        /// <summary>
        /// Initialisiert die Laufzeitfassade für diese Bibliothek.
        /// </summary>
        /// <param name="serviceProvider">ServiceProvider, der für registrierte Komponenten wie HandlerProvider herangezogen werden soll.</param>
        /// <returns>Den verwendeten ServiceProvider (fluent API).</returns>
        public static IServiceProvider CreateRequestResponsePatternRuntime(
            this IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

            Runtime.Current = new Runtime(serviceProvider);

            return serviceProvider;
        }
    }
}
