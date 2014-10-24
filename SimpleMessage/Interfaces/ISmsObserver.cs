// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Orleans grain communication interface ISmsGrain
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimpleMessage.GrainInterfaces
{
    using Orleans;

    /// <summary>
    /// Orleans grain communication interface ISmsGrain
    /// </summary>
    public interface ISmsObserver : IGrainObserver
    {
        /// <summary>
        /// A message was sent to this instance.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void NewMessage(string message);
    }

}
