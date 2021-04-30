using System;
using Dataport.AppFrameDotNet.RequestResponsePattern.Internal;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Exceptions
{
    /// <summary>
    /// Wird geworfen, wenn kein Handler für einen Request-Typen registriert ist.
    /// </summary>
    public class HandlerNotFoundException : Exception
    {
        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="requestType">Typ des Requests, für den kein Handler registriert ist.</param>
        internal HandlerNotFoundException(Type requestType)
            :base($"Es ist kein Handler für Request {requestType.FullName} registriert.")
        {
            this.AddData("HandlerNotFoundException.RequestType", requestType);
        }
    }
}