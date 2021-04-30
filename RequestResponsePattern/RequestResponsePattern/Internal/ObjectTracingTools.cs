using System;
using System.Linq;
using System.Text;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Internal
{
    internal static class ObjectTracingTools
    {
        // Umwandlung eines beliebigen Objekts zur Darstellung in einem String
        // durch Auflistung der Eigenschaften.
        internal static string ToTraceString<T>(this T context)
        {
            if (context == null) return null;

            var props = context.GetType().GetProperties().Where(x => x.PropertyType.IsValueType || x.PropertyType == typeof(string));
            var sb = new StringBuilder();
            sb.AppendLine($"({context.GetType().FullName})");
            foreach (var p in props)
            {
                sb.AppendLine(p.Name + ": " + p.GetValue(context, null));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Fügt einen Eintrag zur Data-Auflistung zu.
        /// </summary>
        /// <param name="context">Exception</param>
        /// <param name="key">Schlüssel des Eintrags.</param>
        /// <param name="value">Wert des Eintrags.</param>
        /// <remarks>Kann als Fluent-Interface verkettet aufgerufen werden.</remarks>
        /// <returns>Exception (Fluent-Interface).</returns>
        public static Exception AddData(this Exception context, object key, object value)
        {
            if (context == null) return null;

            //Sicherungsmaßnahme um versehentlichen doppelten Key zu vermeiden
            if (context.Data.Contains(key)) key = $"{key}_{Guid.NewGuid()}";

            context.Data.Add(key, value);

            return context;
        }
    }
}