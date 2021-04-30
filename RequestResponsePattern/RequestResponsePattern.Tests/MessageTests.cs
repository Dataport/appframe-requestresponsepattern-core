using System;
using Dataport.AppFrameDotNet.RequestResponsePattern.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests
{
    /// <summary>
    /// Tests zu ResponseMessages.
    /// </summary>
    [TestClass]
    public class MessageTests
    {
        [TestMethod]
        public void Message_CompleteText()
        {
            var msg = new ResponseMessage("3660C65C-E278-4926-8A98-F67301C301A1", "Text für Benutzer")
            {
                Details = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. Stet clita kasd gubergren, no sea takimata sanctus est Lorem ipsum dolor sit amet.",
                NestedMessages = new IResponseMessage[] { new Exception("Exception").AsResponseMessage() }
            };

            var txt = msg.CompleteText;

            Assert.IsNotNull(txt);

            Console.WriteLine(txt);
        }
    }
}
