// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Orleans grain communication interface ISmsGrain
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimpleMessage.GrainInterfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Orleans;

    /// <summary>
    /// Orleans grain communication interface ISmsGrain
    /// </summary>
    public interface ISmsGrain : IGrainWithStringKey
    {
        /// <summary>
        /// Send a message to this instance.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the work performed.
        /// </returns>
        Task NewMessage(string message);

        /// <summary>
        /// Returns the messages held by this instance.
        /// </summary>
        /// <returns>
        /// The messages held by this instance.
        /// </returns>
        Task<List<string>> GetMessages();
        
        /// <summary>
        /// Watch this grain for new messages.
        /// </summary>
        /// <param name="observer">The watcher.</param>
        /// <returns>A <see cref="Task"/> representing the work performed.</returns>
        Task WatchForMessages(ISmsObserver observer);
    }

}
