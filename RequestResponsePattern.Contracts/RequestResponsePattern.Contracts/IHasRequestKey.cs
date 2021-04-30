namespace Dataport.AppFrameDotNet.RequestResponsePattern.Contracts
{
    /// <summary>
    /// Interface für Requests, deren Inhalt über einen Schlüssel identifiziert werden kann.
    /// </summary>
    /// <remarks>
    /// Dient primär der Möglichkeit Request-Ausführungen zu cachen. Über den Key wird ermittelt, welche
    /// Requests zum selben Ergebnis führen.
    /// </remarks>
    public interface IHasRequestKey
    {
        /// <summary>
        /// Schlüssel, der den Inhalt zweier Requests vergleichbar macht.
        /// </summary>
        /// <returns>Key</returns>
        /// <remarks>Der Schlüssel muss eine Parameterkonstellation 
        /// eindeutig identifizieren können, so dass erwartet werden kann,
        /// dass Requests mit dem selben Key den selben Response zurückgeben.</remarks>
        string GetKey();
    }
}
