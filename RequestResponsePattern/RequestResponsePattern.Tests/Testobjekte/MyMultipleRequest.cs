using System.ComponentModel.DataAnnotations;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Testobjekte
{
    class MyMultipleRequest : RequestBase<MyResponse>
    {
        [Required]
        public string Frage { get; set; }
    }
}
