using System;
using System.Collections.Generic;
using System.Dynamic;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern
{
    /// <summary>
    /// Interner State eines Handlers.
    /// </summary>
    public sealed class State : IDisposable
    {
        /// <summary>
        /// Eigenschaften des State.
        /// </summary>
        /// <returns>Eigenschaften des State.</returns>
        public dynamic Properties { get; } = new ExpandoObject();

        /// <summary>
        /// Zugriff auf Eigenschaften des State in Form eines Dictionaries.
        /// </summary>
        /// <returns>Zugriff auf Eigenschaften des State in Form eines Dictionaries.</returns>
        public IDictionary<string, object> PropertiesByName
            // Einfaches Delegieren ist sicher, da Implementierung
            // ein ExpandoObject garantiert.
            => Properties;

        /// <summary>
        /// Validiert die, in Properties hinterlegten, Objekte.
        /// </summary>
        /// <returns>Validierungsmeldungen; oder eine leere Sequenz, wenn keine Validierungsfehler vorliegen.</returns>
        public IEnumerable<IResponseMessage> SelfValidation()
        {
            foreach (var entry in Properties)
            {
                // Ausstieg, falls kein Wert in Property hinterlegt.
                if (!(entry.Value is object value))
                    continue;

                // Validierung durchführen und Ergebnis ausgeben.
                var message = Runtime.Current.Validator.Validate(value);
                if (message != null)
                    yield return message;
            }
        }

        /// <summary>
        /// Intern: Dispose auf enthaltene Objekte kaskadieren.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose(true);
            // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
            // TODO: Review
            GC.SuppressFinalize(this);
        }

        private bool _disposed;

        /// <summary>
        /// Intern: Dispose auf enthaltene Objekte kaskadieren.
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (var entry in Properties)
                    {
                        (entry.Value as IDisposable)?.Dispose();
                    }
                }
            }

            _disposed = true;
        }
    }
}
