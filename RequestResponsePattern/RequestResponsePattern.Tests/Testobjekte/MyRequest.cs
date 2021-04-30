using System.ComponentModel.DataAnnotations;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Testobjekte
{
    class MyRequest : RequestBase<MyResponse>
    {
        [Required(ErrorMessage = "CDC0753C-C5AC-49BE-BDD8-2C1D60B4919D")]
        public string Frage { get; set; }

        public int MeineNummer { get; set; }
    }
}