using System;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Dataport.AppFrameDotNet.RequestResponsePattern.Exceptions;
using Microsoft.Extensions.Logging;

namespace Dataport.AppFrameDotNet.RequestResponsePattern
{
    /// <summary>
    /// Operationen (Verben), mit denen ein Request ausgeführt werden kann.
    /// </summary>
    /// <remarks>
    /// Alle Log-Meldungen innerhalb der Ausführung des Verbs haben den
    /// Klassennamen (mit Namespace) des Requests als Kategorie und seine Id als ActivityId.
    /// </remarks>
    public static class RequestOperations
    {
        private static readonly Lazy<ILogger> Logger
            = new Lazy<ILogger>(() => Runtime.Current.CreateLogger(typeof(RequestOperations)));

        /// <summary>
        /// Request ausführen. Bei einem Fehler (sowohl Validierung als auch interne Exception) Exception auslösen.
        /// Wenn ein Response zurückgegeben wird, ist er gültig.
        /// Vorbedingung: Es gibt nur einen bekannten Handler!
        /// </summary>
        /// <typeparam name="TRequest">Typ des Requests, der ausgeführt werden soll.</typeparam>
        /// <param name="request">Request, der ausgeführt werden soll.</param>
        /// <returns>Request nach der Ausführung (mit Ergebnis-Response).</returns>
        /// <exception cref="RequestFailedException">Wird geworfen, wenn die Ausführung des Requests fehlgeschlagen ist (fehlerhafter Response).</exception>
        public static TRequest Do<TRequest>(this TRequest request)
            where TRequest : IRequestBase
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            request.Try();

            if (request.Response.Failed) throw new RequestFailedException(request);

            return request;
        }

        /// <summary>
        /// Request ausführen, falls ein Handler registriert ist. Bei einem Fehler (sowohl Validierung als auch interne Exception) Exception auslösen.
        /// Wenn ein Response zurückgegeben wird, ist er gültig.
        /// Vorbedingung: Es gibt maximal einen bekannten Handler!
        /// </summary>
        /// <typeparam name="TRequest">Typ des Requests, der ausgeführt werden soll.</typeparam>
        /// <param name="request">Request, der ausgeführt werden soll.</param>
        /// <returns>Response, falls ein Handler gefunden wurde. ACHTUNG: Kann entsprechend, abweichend von den anderen Methoden, auch null sein!</returns>
        /// <exception cref="RequestFailedException">Wird geworfen, wenn die Ausführung des Requests fehlgeschlagen ist (fehlerhafter Response).</exception>
        public static TRequest DoOptional<TRequest>(this TRequest request)
            where TRequest : IRequestBase
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            request.TryOptional();

            if (request.Response?.Failed ?? false) throw new RequestFailedException(request);

            return request;
        }

        /// <summary>
        /// Request ausführen. Bei einem Fehler (sowohl Validierung als auch interne Exception) Exception auslösen.
        /// Wenn ein Response zurückgegeben wird, ist er gültig.
        /// Vorbedingung: Es gibt nur einen bekannten Handler!
        /// </summary>
        /// <param name="request">Request, der ausgeführt werden soll.</param>
        /// <returns>Request nach der Ausführung (mit Ergebnis-Response).</returns>
        /// <exception cref="RequestFailedException">Wird geworfen, wenn die Ausführung des Requests fehlgeschlagen ist (fehlerhafter Response).</exception>
        /// <remarks>Untypisierte Variante, zu verwenden, wo sich der RequestTyp erst zur Laufzeit ergibt.</remarks>
        public static IRequestBase DoDynamic(this IRequestBase request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            request.TryDynamic();

            if (request.Response.Failed) throw new RequestFailedException(request);

            return request;
        }

        /// <summary>
        /// Request ausführen. Bei einem Fehler (sowohl Validierung als auch interne Exception) Exception auslösen.
        /// Wenn ein Response zurückgegeben wird, ist er gültig.
        /// Vorbedingung: Es gibt nur einen bekannten Handler!
        /// </summary>
        /// <param name="request">Request, der ausgeführt werden soll.</param>
        /// <returns>Request nach der Ausführung (mit Ergebnis-Response).</returns>
        /// <remarks>Untypisierte Variante, zu verwenden, wo sich der RequestTyp erst zur Laufzeit ergibt.</remarks>
        public static IRequestBase TryDynamic(this IRequestBase request)
        {
            // Try-Methodenaufruf für den Requesttyp erstellen
            // ReSharper disable once PossibleNullReferenceException
            var tryMethodInfo = typeof(RequestOperations).GetMethod("Try").MakeGenericMethod(request.GetType());

            try
            {
                // Try-Methodenaufruf ausführen
                tryMethodInfo.Invoke(null, new object[] { request });
            }
            catch (TargetInvocationException ex)
            {
                // TargetInvocationException "auspacken"
                // ReSharper disable once AssignNullToNotNullAttribute
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            return request;
        }

        /// <summary>
        /// Request ausführen. Fehler aller Art werden im Response als Messages zurückgegeben.
        /// Es wird auf jeden Fall ein Response ausgegeben, der kann allerdings auch ungültig sein.
        /// Vorbedingung: Es gibt nur einen bekannten Handler!
        /// </summary>
        /// <typeparam name="TRequest">Typ des Requests, der ausgeführt werden soll.</typeparam>
        /// <param name="request">Request, der ausgeführt werden soll.</param>
        /// <returns>Request nach der Ausführung (mit Ergebnis-Response).</returns>
        public static TRequest Try<TRequest>(this TRequest request)
           where TRequest : IRequestBase
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var loggingMessage = $"[RequestOperations] Try({request.GetType().Name}:{request.Id})";
            Logger.Value.Log(LogLevel.Debug, loggingMessage);

            using (Logger.Value.BeginScope(loggingMessage))
            {
                var handler = Runtime.Current.GetHandler<TRequest>().ToArray();

                if (!handler.Any()) throw new HandlerNotFoundException(typeof(TRequest));
                if (handler.Count() > 1) throw new MulipleHandlerFoundException(typeof(TRequest));

                handler.First().Execute(request);

                return request;
            }
        }

        /// <summary>
        /// Request ausführen, falls ein Handler registriert ist. Fehler aller Art werden im Response als Messages zurückgegeben.
        /// Es wird auf jeden Fall ein Response ausgegeben, der kann allerdings auch ungültig sein.
        /// Vorbedingung: Es gibt maximal einen bekannten Handler!
        /// </summary>
        /// <typeparam name="TRequest">Typ des Requests, der ausgeführt werden soll.</typeparam>
        /// <param name="request">Request, der ausgeführt werden soll.</param>
        /// <returns>Response, falls ein Handler gefunden wurde. ACHTUNG: Kann entsprechend, abweichend von den anderen Methoden, auch null sein!</returns>
        public static TRequest TryOptional<TRequest>(this TRequest request)
            where TRequest : IRequestBase
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var loggingMessage = $"[RequestOperations] TryOptional({request.GetType().Name}:{request.Id})";
            Logger.Value.Log(LogLevel.Debug, loggingMessage);

            using (Logger.Value.BeginScope(loggingMessage))
            {
                var handler = Runtime.Current.GetHandler<TRequest>().ToArray();

                // Der Fall "kein Handler" ist hier zulässig (deswegen "optional")
                if (!handler.Any()) return request;

                if (handler.Count() > 1) throw new MulipleHandlerFoundException(typeof(TRequest));

                handler.First().Execute(request);

                return request;
            }
        }

        /// <summary>
        /// Request ausführen. Es werden 0-n Responses zurückgegeben.
        /// Responses können ungültig sein.
        /// Wenn kein Handler bekannt ist, wird kein Response zurückgegeben
        /// </summary>
        /// <typeparam name="TRequest">Typ des Requests, der ausgeführt werden soll.</typeparam>
        /// <param name="request">Request, der ausgeführt werden soll.</param>
        /// <returns>Request nach der Ausführung (mit Ergebnis-Response).</returns>
        public static TRequest Call<TRequest>(this TRequest request)
          where TRequest : IRequestBase
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var loggingMessage = $"[RequestOperations] Call({request.GetType().Name}:{request.Id})";
            Logger.Value.Log(LogLevel.Debug, loggingMessage);
            using (Logger.Value.BeginScope(loggingMessage))
            {
                var handlers = Runtime.Current.GetHandler<TRequest>().ToArray();

                foreach (var handler in handlers)
                {
                    handler.Execute(request);
                }

                return request;
            }
        }

        /// <summary>
        /// Request ausführen. Es werden 0-n Responses zurückgegeben.
        /// Responses können ungültig sein.
        /// Wenn kein Handler bekannt ist, wird kein Response zurückgegeben
        /// </summary>
        /// <param name="request">Request, der ausgeführt werden soll.</param>
        /// <returns>Request nach der Ausführung (mit Ergebnis-Response).</returns>
        /// <remarks>Untypisierte Variante, zu verwenden, wo sich RequestTyp erst zur Laufzeit ergibt.</remarks>
        public static IRequestBase CallDynamic(this IRequestBase request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            // ReSharper disable once PossibleNullReferenceException
            var callMethodInfo = typeof(RequestOperations).GetMethod("Call").MakeGenericMethod(request.GetType());

            try
            {
                // Try-Methodenaufruf ausführen
                callMethodInfo.Invoke(null, new object[] { request });
            }
            catch (TargetInvocationException ex)
            {
                // TargetInvocationException "auspacken"
                // ReSharper disable once AssignNullToNotNullAttribute
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            return request;
        }

        /// <summary>
        /// Durch den Handler prüfen lassen, ob sich der Request ausführen lassen würde.
        /// Vorbedingung: Es gibt nur einen bekannten Handler!
        /// </summary>
        /// <typeparam name="TRequest">Typ des Requests, der ausgeführt werden soll.</typeparam>
        /// <param name="request">Request, der ausgeführt werden soll.</param>
        /// <returns>Request nach der Ausführung (mit Ergebnis-Response).</returns>
        public static TRequest Evaluate<TRequest>(this TRequest request)
           where TRequest : IRequestBase
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var handler = Runtime.Current.GetHandler<TRequest>().ToArray();

            if (!handler.Any()) throw new HandlerNotFoundException(typeof(TRequest));
            if (handler.Count() > 1) throw new MulipleHandlerFoundException(typeof(TRequest));

            handler.First().Evaluate(request);

            return request;
        }

        /// <summary>
        /// Durch den Handler prüfen lassen, ob sich der Request ausführen lassen würde.
        /// Vorbedingung: Es gibt nur einen bekannten Handler!
        /// </summary>
        /// <param name="request">Request, der ausgeführt werden soll.</param>
        /// <returns>Request nach der Ausführung (mit Ergebnis-Response).</returns>
        /// <remarks>Untypisierte Variante, zu verwenden, wo sich RequestTyp erst zur Laufzeit ergibt.</remarks>
        public static IRequestBase EvaluateDynamic(this IRequestBase request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            // ReSharper disable once PossibleNullReferenceException
            var evaluateMethodInfo = typeof(RequestOperations).GetMethod("Evaluate").MakeGenericMethod(request.GetType());

            try
            {
                // Try-Methodenaufruf ausführen
                evaluateMethodInfo.Invoke(null, new object[] { request });
            }
            catch (TargetInvocationException ex)
            {
                // TargetInvocationException "auspacken"
                // ReSharper disable once AssignNullToNotNullAttribute
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            return request;
        }
    }
}
