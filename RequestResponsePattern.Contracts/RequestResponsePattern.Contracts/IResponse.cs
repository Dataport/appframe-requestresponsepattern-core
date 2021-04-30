using System;
using System.Collections.Generic;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Contracts
{
    /// <summary>
    /// Interface für Response-Daten.
    /// </summary>
    public interface IResponse
    {
        /// <summary>
        /// Eindeutige ID der Antwort.
        /// </summary>
        /// <returns>GUID der Antwort</returns>
        Guid Id { get; set; }

        /// <summary>
        /// Rückmeldungen.
        /// </summary>
        /// <returns>Liste mit Rückmeldungen</returns>
        IList<IResponseMessage> Messages { get; set; }

        /// <summary>
        /// Status, ob der Request ausgeführt wurde.
        /// </summary>
        /// <returns>true, wenn ausgeführt</returns>
        bool Executed { get; set; }

        /// <summary>
        /// Status, ob der Request nicht erfolgreich ausgeführt wurde.
        /// </summary>
        /// <returns>true, wenn nicht erfolgreich</returns>
        bool Failed { get; set; }
    }
}