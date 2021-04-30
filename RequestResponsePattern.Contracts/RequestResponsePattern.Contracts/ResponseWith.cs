using System.ComponentModel.DataAnnotations;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Contracts
{
    /// <summary>
    /// Rückgabe eines Objekts (üblicherweise ein Model).
    /// </summary>
    /// <typeparam name="T">Typ der Response</typeparam>
    public class ResponseWith<T> : ResponseBase
    {
        /// <summary>
        /// Daten.
        /// </summary>
        /// <returns>Die übergebenen Daten.</returns>
        [Required]
        public T Data { get; set; }
    }
}
