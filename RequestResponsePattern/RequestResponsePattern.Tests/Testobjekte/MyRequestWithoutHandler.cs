using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.Testobjekte
{
    /// <summary>
    /// Request ohne registrierten Handler.
    /// </summary>
    public class MyRequestWithoutHandler : RequestBase<ResponseBase>
    {  }
}
