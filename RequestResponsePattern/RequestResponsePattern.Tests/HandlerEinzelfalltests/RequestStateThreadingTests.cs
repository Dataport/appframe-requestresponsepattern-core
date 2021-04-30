using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.HandlerEinzelfalltests
{
    /// <summary>
    /// Tests zum korrekten Verhalten des Request-States bei parallelen Ausführungen.
    /// </summary>
    [TestClass]
    public class RequestStateThreadingTests
    {
        /// <summary>
        /// Prüft, ob der RequestState tatsächlich Threadspezifisch
        /// und damit Threadsafe ist.
        /// </summary>
        [TestMethod]
        public void RequestStateThreadingTests_Success()
        {
            var request1 = new MyRequest();
            var request2 = new MyRequest();

            //"Singleton"
            var handler = new MyHandler(); 

            Parallel.Invoke(()=> handler.Execute(request1),
                    () => handler.Execute(request2));
            
            Assert.AreEqual(true, request1.Response.HasExecutedWithSuccess);
            Assert.AreEqual(true, request2.Response.HasExecutedWithSuccess);

            Assert.AreNotEqual(request1.Response.MyObjectId, request2.Response.MyObjectId);
            Assert.AreNotEqual(request1.Response.ThreadId, request2.Response.ThreadId);
        }
    
        private class MyRequest : RequestBase<MyResponse> { }

        private class MyResponse : ResponseBase
        {
            [Required]
            public Guid? MyObjectId { get; set; }

            [Required]
            public int ThreadId { get; set; }
        }

        private class MyHandler : HandlerBase<MyRequest, MyResponse>
        {
            protected override void OnEnter(MyRequest request)
            {
                RequestState.MyObject = new MyObject();
                Thread.Sleep(10);
            }

            protected override void Implementation(MyRequest request)
            {
                request.Response.MyObjectId = RequestState.MyObject.Id;
                request.Response.ThreadId = Thread.CurrentThread.ManagedThreadId;
            }
        }

        private class MyObject
        {
            // ReSharper disable once UnusedMember.Local
            public Guid Id { get; } = Guid.NewGuid();
        }
    }
}
