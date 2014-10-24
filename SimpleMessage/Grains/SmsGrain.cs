// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   Orleans grain implementation class SmsGrain
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimpleMessage.Grains
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Orleans;
    using Orleans.Providers;

    using SimpleMessage.GrainInterfaces;

    /// <summary>
    /// Orleans grain implementation class SmsGrain
    /// </summary>
    [StorageProvider(ProviderName = "MyStore")]
    public class SmsGrain : Grain<IMessageState>, ISmsGrain
    {
        /// <summary>
        /// The SMS observers.
        /// </summary>
        private readonly ObserverSubscriptionManager<ISmsObserver> observers = new ObserverSubscriptionManager<ISmsObserver>();

        /// <summary>
        /// Send a message to this instance.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the work performed.
        /// </returns>
        public async Task NewMessage(string message)
        {
            this.State.Messages = this.State.Messages ?? new List<string>();
            this.State.Messages.Add(message);
            await this.State.WriteStateAsync();

            // Notify the observers that a new message has arrived.
            this.observers.Notify(observer => observer.NewMessage(message));
        }

        /// <summary>
        /// Returns the messages held by this instance.
        /// </summary>
        /// <returns>
        /// The messages held by this instance.
        /// </returns>
        public Task<List<string>> GetMessages()
        {
            return Task.FromResult(this.State.Messages);
        }

        /// <summary>
        /// Watch this grain for new messages.
        /// </summary>
        /// <param name="observer">The watcher.</param>
        /// <returns>A <see cref="Task"/> representing the work performed.</returns>
        public Task WatchForMessages(ISmsObserver observer)
        {
            this.observers.Subscribe(observer);
            return Task.FromResult(0);
        }
    }
}