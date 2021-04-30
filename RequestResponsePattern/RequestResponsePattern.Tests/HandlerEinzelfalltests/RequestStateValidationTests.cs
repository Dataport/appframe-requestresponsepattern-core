using System.ComponentModel.DataAnnotations;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.HandlerEinzelfalltests
{
    /// <summary>
    /// Tests zur automatischen Validierung des RequestStates.
    /// </summary>
    [TestClass]
    public class RequestStateValidationTests
    {
        [TestMethod]
        public void RequestStateValidationTests_Fail()
        {
            var request = new MyRequest();
            (new MyHandler()).Execute(request);
            Assert.AreEqual(false, request.Response.HasExecutedWithSuccess);
            Assert.IsTrue(request.ContainsMessageWithUserFriendlyCaption("936FB36D-DC58-4A47-954F-E83E3110E17E"));
        }
    
        private class MyRequest : RequestBase<MyResponse> { }

        private class MyResponse : ResponseBase { }

        private class MyHandler : HandlerBase<MyRequest, MyResponse>
        {
            protected override void Implementation(MyRequest request)
            {
                RequestState.MyObject = new MyObject();
            }
        }

        private class MyObject
        {
            [Required(ErrorMessage = "936FB36D-DC58-4A47-954F-E83E3110E17E")]
            // ReSharper disable once UnusedMember.Local
            public string Text { get; set; }
        }
    }
}
