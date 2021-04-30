using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Core
{
    /// <summary>
    /// Quelle zum Definieren zusätzlicher Aktionen, die der Handler-Ausführung hinzugefügt werden sollen.
    /// </summary>
    public interface IActionDefinitionSource
    {
        /// <summary>
        /// Definiert zusätzliche Aktionen, die der Handler-Ausführung hinzugefügt werden sollen.
        /// </summary>
        /// <returns>Hinzuzufügende Aktionen.</returns>
        ActionDefinition<IRequestBase, IRequestContext>[] GetActionDefinitions();
    }
}