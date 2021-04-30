using System;
using System.Text;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Exceptions
{
    /// <summary>
    /// Wird geworfen, wenn mit Do() ein Request ausgeführt wird, dessen Response
    /// den Status 'Fehlgeschlagen' hat. Als Meldung werden die ResponseMessages ausgegeben.
    /// </summary>
    public class RequestFailedException : Exception
    {
        internal RequestFailedException(IRequestBase request)
            : base(CreateMessageText(request))
        { }

        private static string CreateMessageText(IRequestBase request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var builder = new StringBuilder();

            builder.AppendLine($"Die Ausführung des Requests {request.GetType().Name} ist mit folgenden Meldungen fehlgeschlagen:");

            foreach (var entry in request.Response.Messages)
            {
                builder.AppendLine("---------------------------");
                builder.AppendLine(entry.CompleteText);
            }

            return builder.ToString();
        }
    }
}
