using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Validation
{
    /// <summary>
    /// Interface für Implementierungen, die die Validierung
    /// in der RequestResponsePattern-Komponente durchführen können.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validiert ein Objekt.
        /// </summary>
        /// <typeparam name="TTarget">Typ des zu validierenden Objekts.</typeparam>
        /// <param name="target">Objekt, das validiert werden soll.</param>
        /// <returns>Null, falls Objekt gültig ist; ansonsten eine Message mit den Validierungsmeldungen.</returns>
        /// <remarks>Einzelne Validierungsmeldungen werden als NestedMessages erwartet.</remarks>
        IResponseMessage Validate<TTarget>(TTarget target);
    }
}