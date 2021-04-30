using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Tracing;
using System.Linq;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Validation
{
    /// <summary>
    /// Führt Validierung über die .NET built-in DataAnnotations durch.
    /// </summary>
    public class DataAnnotationValidator : IValidator
    {
        /// <summary>
        /// EventLevel, der in ResponseMessages gesetzt werden soll, die durch Validierung entstehen.
        /// </summary>
        /// <returns>EventLevel, der in ResponseMessages gesetzt werden soll, die durch Validierung entstehen.</returns>
        public EventLevel DefaultValidationResultEventLevel { get; set; } = EventLevel.Warning;

        /// <summary>
        /// Benutzerfreundlicher Bezeichner für die Kopf-Message einer Validierung.
        /// </summary>
        /// <returns>Benutzerfreundlicher Bezeichner für die Kopf-Message einer Validierung.</returns>
        public string UserFriendlyCaption { get; set; } =
            "Die eingegebenen Parameter der Anfrage enthielten Fehler.";

        /// <summary>
        /// Validiert ein Objekt.
        /// </summary>
        /// <typeparam name="TTarget">Typ des zu validierenden Objekts.</typeparam>
        /// <param name="target">Objekt, das validiert werden soll.</param>
        /// <returns>Null, falls Objekt gültig ist; ansonsten eine Message mit den Validierungsmeldungen.</returns>
        /// <remarks>Einzelne Validierungsmeldungen werden als NestedMessages erwartet.</remarks>
        /// TODO: http://www.technofattie.com/2011/10/05/recursive-validation-using-dataannotations.htm
        /// in einem Extra Pakete implementieren.
        public IResponseMessage Validate<TTarget>(TTarget target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));

            var results = new List<ValidationResult>();
            Validator.TryValidateObject(target, new ValidationContext(target), results, true);

            if (!results.Any()) return null;

            return new ResponseMessage()
            {
                EventLevel = DefaultValidationResultEventLevel,
                UserFriendlyCaption = UserFriendlyCaption,
                Categories = new[] { DefaultResponseMessageCategories.ValidationResult },

                // Ergebnisse in NestedMessages übersetzen.
                NestedMessages = results.Select(x => new ResponseMessage
                {
                    EventLevel = DefaultValidationResultEventLevel,
                    Categories = new[] { DefaultResponseMessageCategories.ValidationResult },
                    UserFriendlyCaption = x.ErrorMessage
                }).ToList()
            };
        }
    }
}