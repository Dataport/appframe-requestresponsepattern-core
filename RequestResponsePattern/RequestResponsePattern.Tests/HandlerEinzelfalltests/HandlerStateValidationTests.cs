using System.ComponentModel.DataAnnotations;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable UnusedMember.Local

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.HandlerEinzelfalltests
{
    /// <summary>
    /// Tests zur automatischen Validierung des HandlerStates.
    /// </summary>
    [TestClass]
    public class HandlerStateValidationTests
    {
        [TestMethod]
        public void HandlerStateValidationTests_Fail()
        {
            var request = new MyRequest();
            (new MyHandler()).Execute(request);
            Assert.AreEqual(false, request.Response.HasExecutedWithSuccess);
            Assert.IsTrue(request.ContainsMessageWithUserFriendlyCaption("2D3DE672-A125-4ED5-913E-AA9F2C0C4A9B"));
        }
    
        private class MyRequest : RequestBase<MyResponse> { }

        private class MyResponse : ResponseBase { }

        private class MyHandler : HandlerBase<MyRequest, MyResponse>
        {
            protected override void Implementation(MyRequest request)
            {
                HandlerState.MyObject = new MyObject();
            }
        }

        private class MyObject
        {
            [Required(ErrorMessage = "2D3DE672-A125-4ED5-913E-AA9F2C0C4A9B")]
            public string Text { get; set; }
        }
    }
}
