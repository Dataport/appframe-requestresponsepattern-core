namespace Dataport.AppFrameDotNet.RequestResponsePattern
{
    /// <summary>
    /// Bestimmt den Ausführungszeitpunkt eines Plugins der bei Abarbeitung eines Requests.
    /// </summary>
    public enum PluginPosition
    {
        /// <summary>
        /// Im Fehlerfall.
        /// </summary>
        OnError = -1,

        /// <summary>
        /// Bei Eintritt in den Handler, aber vor Validierung des Requests.
        /// </summary>
        Enter = 0,

        /// <summary>
        /// Vor Ausführung der Implementierung, aber nach Validierung des Requests.
        /// </summary>
        PreImplementation,

        /// <summary>
        /// Nach Ausführung der Implementierung, aber vor Validierung des Response.
        /// </summary>
        PostImplementation,

        /// <summary>
        /// Nach Ausführung der Implementierung und nach Validierung des Response.
        /// </summary>
        OnSuccess,

        /// <summary>
        /// Wenn die Verarbeitung in einem beliebigem Schritt abgebrochen wurde.
        /// </summary>
        /// <remarks>z.B. Validierungsfehler</remarks>
        OnFailure,

        /// <summary>
        ///  Bei Austritt aus dem Handler.
        /// </summary>
        Exit
    }
}
