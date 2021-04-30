using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Microsoft.Extensions.Logging;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Logging
{
    /// <summary>
    /// Hilfsmethoden das für Logging von Messages.
    /// </summary>
    public static class MessageLoggingExtensions
    {
        /// <summary>
        /// Schreibt UserFriendlyCaption und Details in den Trace.
        /// Scope ist die SourceI der Message.
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="context">Meldung</param>
        /// <param name="logger">Logger</param>
        /// <returns>Meldung (Fluent-Interface)</returns>
        public static T WriteToLog<T>(this T context, ILogger logger)
            where T : IResponseMessage
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            if (context == null)
                return default(T);

            using (logger.BeginScope($"Message:{context.SourceId}"))
            {
                var messageText = !string.IsNullOrWhiteSpace(context.Details)
                    ? $"{context.UserFriendlyCaption}|{context.Details}"
                    : context.UserFriendlyCaption;

                logger.Log(context.EventLevel.EventLevelMapper(), messageText);

                // Nested Messages berücksichtigen.

                IList<IResponseMessage> nestedMessages = context.NestedMessages?.ToList();
                nestedMessages.WriteToLog(logger);
            }

            return context;
        }

        /// <summary>
        /// Schreibt UserFriendlyCaption und Details in den Trace.
        /// </summary>
        /// <typeparam name="T">Typ</typeparam>
        /// <param name="context">Meldungsauflistung</param>
        /// <param name="logger">Logger</param>
        /// <returns>Meldungsauflistung (Fluent-Interface)</returns>
        public static IList<T> WriteToLog<T>(this IList<T> context, ILogger logger)
            where T : IResponseMessage
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (context == null) return null;

            foreach (var entry in context)
            {
                entry.WriteToLog(logger);
            }

            return context;
        }

        // Mapping EventLevel -> LogLevel
        private static LogLevel EventLevelMapper(this EventLevel eventLevel)
        {
            switch (eventLevel)
            {
                case EventLevel.Critical:
                    return LogLevel.Critical;
                case EventLevel.Error:
                    return LogLevel.Error;
                case EventLevel.Informational:
                    return LogLevel.Information;
                case EventLevel.LogAlways:
                    return LogLevel.Trace;
                case EventLevel.Verbose:
                    return LogLevel.Debug;
                case EventLevel.Warning:
                    return LogLevel.Warning;
                default:
                    throw new ArgumentOutOfRangeException(nameof(eventLevel), eventLevel, null);
            }
        }

    }
}