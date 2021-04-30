using System.Collections.Generic;
using System.Linq;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Dataport.AppFrameDotNet.RequestResponsePattern.Core;
using Dataport.AppFrameDotNet.RequestResponsePattern.HandlerProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests.HandlerProvider
{
    /// <summary>
    /// Tests zur Basis von HandlerProvidern, die einen bestimmten Request-Typen bedienen.
    /// </summary>
    [TestClass]
    public class HandlerProviderForRequestTests
    {
        [TestMethod]
        public void HandlerProviderForRequest_ProvidesForExactTypeMatch()
        {
            var sut = new HandlerProviderUnderTest();
            var result = sut.GetHandler<MyRequest>();
            Assert.AreEqual(1, result.Count());
        }

        [TestMethod]
        public void HandlerProviderForRequest_DoesNotProvideForOtherTypes()
        {
            var sut = new HandlerProviderUnderTest();
            var result = sut.GetHandler<MyOtherRequest>();
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void HandlerProviderForRequest_ProvidesForSubType()
        {
            var sut = new HandlerProviderUnderTest();
            var result = sut.GetHandler<MySubRequest>();
            Assert.AreEqual(1, result.Count());
        }

        private class HandlerProviderUnderTest : HandlerProviderForRequest<MyRequest>
        {
            protected override IEnumerable<IHandler<MyRequest>> GetHandlerForRequest()
            {
                // Instanz zurückgeben, auf die in Tests geprüft werden kann.
                yield return new ActionHandler<MyRequest>();
            }
        }

        private class MyRequest: RequestBase<ResponseBase> { }
        private class MySubRequest : MyRequest { }
        private class MyOtherRequest: RequestBase<ResponseBase> { }
    }
}