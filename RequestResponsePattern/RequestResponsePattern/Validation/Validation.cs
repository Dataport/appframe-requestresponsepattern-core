namespace Dataport.AppFrameDotNet.RequestResponsePattern.Validation
{
    /// <summary>
    /// Fassade zur Konfiguration und Verwendung des Validierungsmechanismus
    /// in der Request-Response Komponente.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Validiert das gegebene Objekt mit dem konfigurierten Validator.
        /// </summary>
        /// <typeparam name="T">Typ des zu validierenden Objekts.</typeparam>
        /// <param name="target">Objekt, das validiert werden soll.</param>
        /// <returns>Das Ergebnis der Validierung.</returns>
        public static ValidationResponseCollection Validate<T>(T target) =>
            new ValidationResponseCollection().Add(Runtime.Current.Validator.Validate(target));
    }
}