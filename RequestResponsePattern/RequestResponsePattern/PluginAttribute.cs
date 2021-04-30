using System;

namespace Dataport.AppFrameDotNet.RequestResponsePattern
{
    /// <summary>
    /// Basisklasse zur Definition eines Attributes mit dem ein Plugin am Handler hinzugefügt werden kann.
    /// </summary>
    public abstract class PluginAttribute : Attribute
    {
        /// <summary>
        /// Konstruktor.
        /// </summary>
        /// <param name="position">Ausführungsposition des Plugins.</param>
        /// <returns>Das erstellte Plugin.</returns>
        protected PluginAttribute(PluginPosition position)
        {
            Position = position;
        }

        /// <summary>
        /// Ausführungsposition des Plugins.
        /// </summary>
        /// <returns>Ausführungsposition des Plugins.</returns>
        public PluginPosition Position { get; }

        /// <summary>
        /// Fatory-Methode um das Plugin zu erstellen.
        /// </summary>
        /// <returns>Erstellen des Plugins zur Laufzeit, das durch das Attribut beschrieben wird.</returns>
        public abstract object CreatePlugin();
    }
}
