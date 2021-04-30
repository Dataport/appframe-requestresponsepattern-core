using System.Linq;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests
{
    /// <summary>
    /// Tests zu den Hilfsmethoden zum Umgang mit Responses.
    /// </summary>
    [TestClass]
    public class ResponseExtensionTests
    {
        [TestMethod]
        public void ApplyAsNestedResponseOf_Fail()
        {
            var masterResponse = new ResponseBase();

            var nestedResponse = new ResponseBase() { Metadata = { Failed = true } };
            nestedResponse.AddMessage(new ResponseMessage("nestedSourceId", "NestedCaption"));

            var result = nestedResponse.ApplyAsNestedResponseOf(masterResponse,
                "successSourceId",
                "successCaption",
                "failSourceId",
                "failCaption");

            Assert.AreEqual(false, result, "result");
            Assert.AreEqual(true, masterResponse.HasFailed, "HasFailed");
            Assert.AreEqual(1, masterResponse.Metadata.Messages.Count, "Messages.Count");
            Assert.AreEqual("failSourceId", masterResponse.Metadata.Messages.First().SourceId, "Messages.First().SourceId");
            Assert.AreEqual("failCaption", masterResponse.Metadata.Messages.First().UserFriendlyCaption, "Messages.First().UserFriendlyCaption");
            Assert.AreEqual(1, masterResponse.Metadata.Messages.First().NestedMessages.Count(), "NestedMessages.Count()");
            Assert.AreEqual("nestedSourceId", masterResponse.Metadata.Messages.First().NestedMessages.First().SourceId, "NestedMessages.First().SourceId");
        }

        [TestMethod]
        public void ApplyAsNestedResponseOf_Fail_OverrideCascade()
        {
            var masterResponse = new ResponseBase();

            var nestedResponse = new ResponseBase() { Metadata = { Failed = true } };
            nestedResponse.AddMessage(new ResponseMessage("nestedSourceId", "NestedCaption"));

            var result = nestedResponse.ApplyAsNestedResponseOf(masterResponse,
                "successSourceId",
                "successCaption",
                "failSourceId",
                "failCaption",
                applyFailOnMaster: false);

            Assert.AreEqual(false, result, "result");
            Assert.AreEqual(false, masterResponse.HasFailed, "HasFailed");
            Assert.AreEqual(1, masterResponse.Metadata.Messages.Count, "Messages.Count");
            Assert.AreEqual("failSourceId", masterResponse.Metadata.Messages.First().SourceId, "Messages.First().SourceId");
            Assert.AreEqual("failCaption", masterResponse.Metadata.Messages.First().UserFriendlyCaption, "Messages.First().UserFriendlyCaption");
            Assert.AreEqual(1, masterResponse.Metadata.Messages.First().NestedMessages.Count(), "NestedMessages.Count()");
            Assert.AreEqual("nestedSourceId", masterResponse.Metadata.Messages.First().NestedMessages.First().SourceId, "NestedMessages.First().SourceId");
        }

        [TestMethod]
        public void ApplyAsNestedResponseOf_Fail_WithoutNested()
        {
            var masterResponse = new ResponseBase();

            var nestedResponse = new ResponseBase() { Metadata = { Failed = true } };

            var result = nestedResponse.ApplyAsNestedResponseOf(masterResponse,
                "successSourceId",
                "successCaption",
                "failSourceId",
                "failCaption");

            Assert.AreEqual(false, result, "result");
            Assert.AreEqual(true, masterResponse.HasFailed, "HasFailed");
            Assert.AreEqual(1, masterResponse.Metadata.Messages.Count, "Messages.Count");
            Assert.AreEqual("failSourceId", masterResponse.Metadata.Messages.First().SourceId, "Messages.First().SourceId");
            Assert.AreEqual("failCaption", masterResponse.Metadata.Messages.First().UserFriendlyCaption, "Messages.First().UserFriendlyCaption");
            Assert.AreEqual(0, masterResponse.Metadata.Messages.First().NestedMessages.Count(), "NestedMessages.Count()");
        }

        [TestMethod]
        public void ApplyAsNestedResponseOf_Success()
        {
            var masterResponse = new ResponseBase();

            var nestedResponse = new ResponseBase() { Metadata = { Failed = false } };
            nestedResponse.AddMessage(new ResponseMessage("nestedSourceId", "NestedCaption"));

            var result = nestedResponse.ApplyAsNestedResponseOf(masterResponse,
                "successSourceId",
                "successCaption",
                "failSourceId",
                "failCaption");

            Assert.AreEqual(true, result, "result");
            Assert.AreEqual(false, masterResponse.HasFailed, "HasFailed");
            Assert.AreEqual(1, masterResponse.Metadata.Messages.Count, "Messages.Count");
            Assert.AreEqual("successSourceId", masterResponse.Metadata.Messages.First().SourceId, "Messages.First().SourceId");
            Assert.AreEqual("successCaption", masterResponse.Metadata.Messages.First().UserFriendlyCaption, "Messages.First().UserFriendlyCaption");
            Assert.AreEqual(1, masterResponse.Metadata.Messages.First().NestedMessages.Count(), "NestedMessages.Count()");
            Assert.AreEqual("nestedSourceId", masterResponse.Metadata.Messages.First().NestedMessages.First().SourceId, "NestedMessages.First().SourceId");
        }

        [TestMethod]
        public void ApplyAsNestedResponseOf_Success_WithoutNested()
        {
            var masterResponse = new ResponseBase();

            var nestedResponse = new ResponseBase() { Metadata = { Failed = false } };

            var result = nestedResponse.ApplyAsNestedResponseOf(masterResponse,
                "successSourceId",
                "successCaption",
                "failSourceId",
                "failCaption");

            Assert.AreEqual(true, result, "result");
            Assert.AreEqual(false, masterResponse.HasFailed, "HasFailed");
            Assert.AreEqual(0, masterResponse.Metadata.Messages.Count, "Messages.Count");
        }

        [TestMethod]
        public void ApplyAsNestedResponseOf_WithoutNested_OverrideNoMessage()
        {
            var masterResponse = new ResponseBase();

            var nestedResponse = new ResponseBase() { Metadata = { Failed = false } };

            var result = nestedResponse.ApplyAsNestedResponseOf(masterResponse,
                "successSourceId",
                "successCaption",
                "failSourceId",
                "failCaption",
                successMessageOnlyWithNestedMessages: false);

            Assert.AreEqual(true, result, "result");
            Assert.AreEqual(false, masterResponse.HasFailed, "HasFailed");
            Assert.AreEqual(1, masterResponse.Metadata.Messages.Count, "Messages.Count");
            Assert.AreEqual("successSourceId", masterResponse.Metadata.Messages.First().SourceId, "Messages.First().SourceId");
            Assert.AreEqual("successCaption", masterResponse.Metadata.Messages.First().UserFriendlyCaption, "Messages.First().UserFriendlyCaption");
        }
    }
}
