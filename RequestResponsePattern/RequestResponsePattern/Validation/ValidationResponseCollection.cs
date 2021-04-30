using System;
using System.Collections.Generic;
using System.Linq;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Validation
{
    /// <summary>
    /// Sammlung von Validierungsergebnissen in Form von ResponseMessages.
    /// </summary>
    public class ValidationResponseCollection
    {
        private readonly List<IResponseMessage> _responseMessages = new List<IResponseMessage>();

        /// <summary>
        /// Menge der ResponseMessages, die durch die Validierungen erzeugt wurden.
        /// </summary>
        /// <returns>Menge der ResponseMessages, die durch die Validierungen erzeugt wurden.</returns>
        public IEnumerable<IResponseMessage> ResponseMessages => _responseMessages;

        /// <summary>
        /// Gibt an, ob bei der Validierung Ungültigkeiten festgestellt wurden.
        /// </summary>
        /// <returns>Gibt an, ob bei der Validierung Ungültigkeiten festgestellt wurden.</returns>
        public bool IsValid => !ResponseMessages.Any();

        /// <summary>
        /// Fügt Validierungsergebnisse in Form von ResponseMessages hinzu.
        /// </summary>
        /// <param name="responseMessages">Durch Validierung entstandene ResponseMessages.</param>
        /// <returns>Die Sammlung selbst (Fluent Interface).</returns>
        public ValidationResponseCollection Add(params IResponseMessage[] responseMessages)
        {
            if (responseMessages == null) return this;

            _responseMessages.AddRange(responseMessages.Where(x => x != null));
            return this;
        }

        /// <summary>
        /// Entfernt Validierungsergebnisse, die die angegebene Bedingung erfüllen.
        /// </summary>
        /// <param name="match">Bedingung, für die die Validierungsergebnisse entfernt werden sollen.</param>
        /// <returns>Die entfernten Validierungsergebnisse.</returns>
        public IEnumerable<IResponseMessage> Remove(Predicate<IResponseMessage> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));

            // Betroffene Meldungen für Rückgabe merken und entfernen.
            var messages = _responseMessages.FindAll(match);
            _responseMessages.RemoveAll(match);

            return messages;
        }
    }
}