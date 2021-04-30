namespace Dataport.AppFrameDotNet.RequestResponsePattern.Contracts
{
    /// <summary>
    /// Responses mit Ergebnis in Binärform, z.B. Dateiinhalte.
    /// </summary>
    public interface IBinaryResponse : IResponse
    {
        /// <summary>
        /// Ergebnis in Binärform.
        /// </summary>
        /// <returns>Binärinhalt</returns>
        byte[] Content { get; }

        /// <summary>
        /// MimeType, der die Art des Ergebnisses angibt.
        /// </summary>
        /// <returns>MimeType</returns>
        string MimeType { get; }

        /// <summary>
        /// Empfohlener Dateiname für den Inhalt.
        /// </summary>
        /// <returns>Dateiname</returns>
        string FileName { get; }
    }
}