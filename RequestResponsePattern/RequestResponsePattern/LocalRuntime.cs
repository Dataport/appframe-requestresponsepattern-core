using System;

namespace Dataport.AppFrameDotNet.RequestResponsePattern
{
    /// <summary>
    /// Thread-Spezifische Runtime innerhalb eines Using-Blocks festlegen. 
    /// </summary>
    public class LocalRuntime : IDisposable
    {
        /// <summary>
        /// Thread-Spezifische Runtime innerhalb eines Using-Blocks festlegen. 
        /// </summary>
        /// <param name="runtime">Lokale Runtime</param>
        /// <exception cref="ArgumentNullException">Runtime muss angegeben sein</exception>
        public LocalRuntime(Runtime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException(nameof(runtime));

            Runtime.SetLocalRuntime(runtime);
        }

        // Freigeben der Runtime mit ihren gebundenen Ressourcen.
        private void ReleaseUnmanagedResources()
        {
            Runtime.SetLocalRuntime(null);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Destruktor zum automatischen Aufräumen von Ressourcen.
        /// </summary>
        ~LocalRuntime()
        {
            ReleaseUnmanagedResources();
        }
    }
}
