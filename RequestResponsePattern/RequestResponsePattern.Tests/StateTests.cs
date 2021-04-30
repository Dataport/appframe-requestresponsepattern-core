using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dataport.AppFrameDotNet.RequestResponsePattern.Tests
{
    /// <summary>
    /// Tests zu den States eines Handlers (HandlerState, RequestState).
    /// </summary>
    [TestClass]
    public class StateTests
    {
        /// <summary>
        /// Testet, ob der Dispose-Aufruf in die Objekte in 
        /// Properties kaskadiert wird.
        /// </summary>
        [TestMethod]
        public void State_Dispose()
        {
            var wasDisposed = false;

            using (var state = new State())
            {
                state.Properties.Hallo = new MyDisposable(() => wasDisposed = true);
                state.Properties.Welt = "Welt";
                state.Properties.Null = null;
            }

            Assert.AreEqual(true, wasDisposed);
        }

        /// <summary>
        /// Testet, ob der Validate-Aufruf in die Objekte in 
        /// Properties kaskadiert wird.
        /// </summary>
        [TestMethod]
        public void State_Validate()
        {
            var state = new State();

            state.Properties.Hallo = new MyObject();
            state.Properties.Null = null;
            
            var messages = state.SelfValidation().ToList();

            Assert.AreEqual(1, messages.Count(), "messages.Count()");
            Assert.IsTrue(messages.First().ContainsMessage(m => m.UserFriendlyCaption == "A66CBB52-1989-4034-AF9A-7DB914EA9C22"));
        }

        private class MyDisposable : IDisposable
        {
            private readonly Action _onDispose;

            public MyDisposable(Action onDispose)
            {
                _onDispose = onDispose;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                _onDispose.Invoke();
            }
        }

        private class MyObject
        {
            [Required(ErrorMessage = "A66CBB52-1989-4034-AF9A-7DB914EA9C22")]
            // ReSharper disable once UnusedMember.Local
            public string Text { get; set; }
        }
    }
}
