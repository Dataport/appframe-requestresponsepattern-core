using System;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Exceptions
{
    /// <summary>
    /// Wird geworfen, wenn ein Handler nicht regelkonform 
    /// gemäß der Namenskonventionen oder Methodensignaturen ist.
    /// </summary>
    public class HandlerInitializationException : Exception
    {
        /// <inheritdoc />
        internal HandlerInitializationException(string message)
            : base(message)
        { }

        /// <inheritdoc />
        internal HandlerInitializationException(string message, Exception ex)
            : base(message, ex)
        { }
    }
}