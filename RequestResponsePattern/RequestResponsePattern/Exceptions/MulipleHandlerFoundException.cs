using System;
using Dataport.AppFrameDotNet.RequestResponsePattern.Internal;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Exceptions
{
    /// <summary>
    /// Wird geworfen, wenn meherere Handler für einen Request-Typen registriert sind.
    /// </summary>
    public class MulipleHandlerFoundException : Exception
    {
        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="requestType">Typ des Requests, für den mehrere Handler registriert sind.</param>
        internal MulipleHandlerFoundException(Type requestType)
            :base($"Es wurden mehrere Handler für Request {requestType.FullName} registriert.")
        {
            this.AddData("MulipleHandlerFoundException.RequestType", requestType);
        }
    }
}