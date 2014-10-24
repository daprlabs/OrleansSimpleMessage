// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The SMS watcher.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SmsConsoleClient
{
    using System;

    using SimpleMessage.GrainInterfaces;

    /// <summary>
    /// The SMS watcher.
    /// </summary>
    public class SmsObserver : ISmsObserver
    {
        /// <summary>
        /// The id of the grain being watched.
        /// </summary>
        private readonly string id;

        /// <summary>
        /// Initializes a new instance of the <see cref="SmsObserver"/> class.
        /// </summary>
        /// <param name="id">
        /// The id of the grain being watched.
        /// </param>
        public SmsObserver(string id)
        {
            this.id = id;
        }

        /// <summary>
        /// A message was sent to this instance.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void NewMessage(string message)
        {
            Console.WriteLine("New message for {0}:\n  {1}", this.id, message);
        }
    }
}
