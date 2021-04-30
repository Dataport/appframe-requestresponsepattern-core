using System;
using System.Collections.Generic;
using System.Linq;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Core
{
    /// <summary>
    /// Positionierung innerhalb der Ausführungskette eines Handlers.
    /// </summary>
    public enum ActionPosition
    {
        /// <summary>
        /// Verlassen des Handlers bei einer Exception.
        /// </summary>
        OnError = -2,

        /// <summary>
        /// Verlassen des Handlers ohne eine Exception auszulösen (sowohl im Erfolgsfall, als auch im Fehlerfall).
        /// </summary>
        OnExit = -1,


        /// <summary>
        /// Standardwert des Enums, ohne Funktion.
        /// </summary>
        None = 0,

        #region "Normale Sequenz"

        /// <summary>
        /// Bei Eintritt des Requests in den Handler.
        /// </summary>
        OnEnter,

        /// <summary>
        /// Validierung des Requests (in sich, ohne Zuziehen weiterer Quellen und Systemzustände).
        /// </summary>
        RequestValidation,

        /// <summary>
        /// Vorbereitung der weiteren Ausführung (z.B. Daten laden, die für Evaluation und Implementierung relevant sind).
        /// </summary>
        Preparation,

        /// <summary>
        /// Programmatische Validierungsschritte um die potentielle Ausführbarkeit des Request zu prüfen.
        /// Hier können auch weitere Daten und der globale Systemzustand berücksichtigt werden.
        /// </summary>
        Evaluation,

        /// <summary>
        /// Prüfen, ob der RequestState nach Abschluss der Evaluationsphase gültig ist.
        /// </summary>
        PostEvaluationRequestStateValidation,

        /// <summary>
        /// Wird ausgeführt, wenn die Evaluationsphase erfolgreich ist.
        /// </summary>
        /// <remarks>Dies geschieht unabhängig davon, ob ein Evaluate() oder Execute() ausgeführt wird.</remarks>
        OnEvaluatedWithSuccess,

        /// <summary>
        /// Auszuführende Aktion.
        /// </summary>
        Implementation,

        /// <summary>
        /// Prüfen, ob der RequestState nach Ausführung noch gültig ist.
        /// </summary>
        PostImplementationRequestStateValidation,

        /// <summary>
        /// Response überprüfen, um zu prüfen, dass die Implementierung die Kommunikationsvereinbarung (Contract) eingehalten hat.
        /// </summary>
        ResponseValidation,

        /// <summary>
        /// Request wurde erfolgreich ausgeführt.
        /// </summary>
        OnExecutedWithSuccess

        #endregion
    }

    /// <summary>
    /// Hilfsmethoden für den ActionPosition-Enum.
    /// </summary>
    public static class ActionPositionTools
    {
        /// <summary>
        /// Enum-Einträge als "Linq-fähige" Iteration.
        /// </summary>
        /// <returns>Enum-Einträge als "Linq-fähige" Iteration.</returns>
        public static readonly IEnumerable<ActionPosition> ActionPositions =
            Enum.GetValues(typeof(ActionPosition)).Cast<ActionPosition>().ToArray();
    }

}
