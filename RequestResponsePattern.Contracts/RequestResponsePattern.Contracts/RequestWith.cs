using System.ComponentModel.DataAnnotations;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Contracts
{
    /// <summary>
    /// Übergabe eines Objects (üblicherweise ein Model).
    /// </summary>
    /// <typeparam name="TResponse">Typ des Response</typeparam>
    /// <typeparam name="TData">Zu übergebene Daten</typeparam>
    public abstract class RequestWith<TData, TResponse> : RequestBase<TResponse>
        where TResponse : ResponseBase
    {
        /// <summary>
        /// Daten.
        /// </summary>
        /// <returns>Die übergebenen Daten.</returns>
        [Required]
        public TData Data { get; set; }
    }
}
