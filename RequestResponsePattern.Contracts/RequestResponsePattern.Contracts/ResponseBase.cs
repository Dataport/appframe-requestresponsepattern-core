using System;
using System.Collections.Generic;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Contracts
{
    /// <summary>
    /// Basisklasse für Rückmeldungen.
    /// </summary>
    public class ResponseBase : IResponse
    {
        private IList<IResponseMessage> _messages = new List<IResponseMessage>();

        /// <summary>
        /// Rückmeldungen.
        /// </summary>
        /// <returns>Liste mit Rückmeldungen</returns>
        IList<IResponseMessage> IResponse.Messages
        {
            get { return _messages; }
            set
            {
                if (_messages == null) throw new ArgumentNullException(nameof(IResponse.Messages));
                _messages = value;
            }
        }

        /// <summary>
        /// Einzelne Message hinzufügen.
        /// </summary>
        /// <param name="message">Message</param>
        public void AddMessage(IResponseMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            _messages.Add(message);
        }

        /// <summary>
        /// Mehrere Messages hinzuzufügen.
        /// </summary>
        /// <param name="messages">Auflistung von Messages</param>
        public void AddMessages(IEnumerable<IResponseMessage> messages)
        {
            if (messages == null) throw new ArgumentNullException(nameof(messages));

            foreach (var message in messages) _messages.Add(message);
        }

        /// <summary>
        /// Response als Failed kennzeichnen.
        /// </summary>
        public void MarkAsFailed()
        {
            Metadata.Failed = true;
        }

        /// <summary>
        /// Zugriff auf die allgemeinen Response-Metadaten.
        /// </summary>
        /// <returns>Metadaten</returns>
        /// <remarks>Diese Metadaten können auch mit einem manuellem Cast auf 'IResponseBase' sichtbar gemacht werden.</remarks>
        public IResponse Metadata => this;

        /// <summary>
        /// Status, ob der Request ausgeführt wurde.
        /// </summary>
        /// <returns>true, wenn der Request ausgeführt wurde.</returns>
        bool IResponse.Executed { get; set; }

        /// <summary>
        /// Status, ob der Request nicht erfolgreich ausgeführt wurde.
        /// </summary>
        /// <returns>true, wenn der Request nicht erfolgreich war.</returns>
        bool IResponse.Failed { get; set; }

        /// <summary>
        /// Status, ob der Request eine Ausführung mit Erfolg wiedergibt.
        /// </summary>
        /// <returns>true, wenn die Ausführung erfolgreich war.</returns>
        public bool HasExecutedWithSuccess => Metadata.Executed && !Metadata.Failed;

        /// <summary>
        /// Status, ob der Request eine Evaluation (Vorprüfung) mit Erfolg wiedergibt.
        /// </summary>
        /// <returns>true, wenn die Vorprüfung erfolgreich war.</returns>
        public bool HasEvaluatedWithSuccess => !Metadata.Executed && !Metadata.Failed;

        /// <summary>
        /// Status, ob der Response eine Ausführung mit Fehler wiedergibt.
        /// </summary>
        /// <returns>true, wenn die Ausführung nicht erfolgreich war.</returns>
        public bool HasFailed => Metadata.Failed;

        /// <summary>
        /// Eindeutige ID der Antwort.
        /// </summary>
        /// <returns>GUID der Antwort</returns>
        Guid IResponse.Id { get; set; } = Guid.NewGuid();
    }
}
