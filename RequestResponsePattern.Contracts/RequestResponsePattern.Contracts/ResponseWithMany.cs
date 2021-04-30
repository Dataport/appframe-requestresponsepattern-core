using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Contracts
{
    /// <summary>
    /// Rückgabe einer Liste von Objekten (üblicherweise ein Model).
    /// </summary>
    /// <typeparam name="T">Typ der Response.</typeparam>
    public class ResponseWithMany<T> : ResponseBase
    {
        /// <summary>
        /// Daten.
        /// </summary>
        /// <returns>Aufzählung von Daten</returns>
        [Required]
        public IEnumerable<T> Data { get; set; }
    }
}
