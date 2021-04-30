using System.ComponentModel.DataAnnotations;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Testobjekte
{
    class MyResponse : ResponseBase
    {
        [Required]
        public string Antwort { get; set; }
    }
}