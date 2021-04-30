using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Dataport.AppFrameDotNet.RequestResponsePattern.Exceptions;

// ReSharper disable UnusedMember.Local

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Core
{
    /// <summary>
    /// Basisklasse für einen Handler, der für einen bestimmten Request-Typ zuständig ist.
    /// </summary>
    /// <typeparam name="TRequest">Typ des Requests.</typeparam>
    /// <typeparam name="TState">Typ des Kontexts zur Ausführung.</typeparam>
    public abstract class HandlerBase2<TRequest, TState> : HandlerCore<TRequest, TState>
        where TRequest : IRequestBase
        where TState : IRequestContext, new()
    {
        /// <inheritdoc />
        protected sealed override ActionDefinition<TRequest, TState>[] GetActionDefinitions()
        {
            return GetHandlerActions(this.GetType()).ToArray();
        }

        // Ermittelt die Aktionen für die Handlerausführung automatisch
        // per Namenskonventionen auf Basis der vorhandenen Methoden.
        private IEnumerable<ActionDefinition<TRequest, TState>> GetHandlerActions(Type definingType)
        {
            // Zurerst runter in den Basistyp gehen (allerdings nicht in diese Implementierung)
            var baseType = definingType.BaseType;
            if (baseType != null && baseType.Namespace != "Dataport.AppFrameDotNet.RequestResponsePattern.Core")
                foreach (var method in GetHandlerActions(baseType))
                    yield return method;

            // Eigene Methoden ermitteln:
            // Alle Sichtbarkeiten, auch statische, allerdings nur aus der aktuellen Vererbungsebene
            foreach (var methodInfo in
                definingType.GetMethods(
                    BindingFlags.Instance | BindingFlags.Static
                    | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            {
                // Postfix-Prüfung
                // --> "TestImplementation" ist eine Action (= ActionPosition.Implementation)
                // --> "DoSomething" ist keine Action (überspringen)
                var actionPostfixPositon = ActionPositionTools.ActionPositions.SingleOrDefault(x =>
                    methodInfo.Name.EndsWith(x.ToString()));
                if (actionPostfixPositon == ActionPosition.None) continue;

                // Signatur(TRequest, TState) / Pflicht, sonst Exception!
                ValidateParameters(methodInfo);

                yield return new ActionDefinition<TRequest, TState>()
                {
                    Name = methodInfo.Name,
                    ActionPosition = actionPostfixPositon,
                    Action = (request, state) => methodInfo.Invoke(this, new object[] { request, state })
                };
            }
        }

        // Validieren der Parameter aus der Methodensignatur auf erwartetes Format.
        private void ValidateParameters(MethodInfo methodInfo)
        {
            var methodeParameters = methodInfo.GetParameters();

            if (methodeParameters.Length != 2)
            {
                throw new HandlerInitializationException(
                    $"{this.GetType().FullName}: Methode {methodInfo.Name} muss zwei Parameter für TRequest und TReuqestState haben.");
            }

            if (!typeof(TRequest).IsAssignableFrom(methodeParameters.First().ParameterType))
            {
                throw new HandlerInitializationException(
                    $"{this.GetType().FullName}: Der erste Parameter von Methode {methodInfo.Name} muss {typeof(TRequest).FullName} zugewiesen werden können.");
            }

            if (!typeof(TState).IsAssignableFrom(methodeParameters.Last().ParameterType))
            {
                throw new HandlerInitializationException(
                    $"{this.GetType().FullName}: Der zweite Parameter von Methode {methodInfo.Name} muss {typeof(TState).FullName} zugewiesen werden können.");
            }
        }
    }
}
