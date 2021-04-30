using System;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Core
{
    /// <summary>
    /// Definiert zusätzliche Aktionen, die der Handler-Ausführung hinzugefügt werden sollen.
    /// </summary>
    public abstract class ActionDefinitionAddInAttribute : Attribute, IActionDefinitionSource
    {
        /// <summary>
        /// Definiert zusätzliche Aktionen, die der Handler-Ausführung hinzugefügt werden sollen.
        /// </summary>
        /// <returns>Definierte Aktionen.</returns>
        public abstract ActionDefinition<IRequestBase, IRequestContext>[] GetActionDefinitions();
    }
}
