// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   The recipe controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimpleMessage.Web.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Web.Http;

    using Orleans;

    using SimpleMessage.GrainInterfaces;

    /// <summary>
    /// The recipe controller.
    /// </summary>
    [RoutePrefix("api/sms")]
    public class SmsController : ApiController
    {
        /// <summary>
        /// Returns the instance with the specified <paramref name="id"/>.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The instance with the specified <paramref name="id"/>.
        /// </returns>
        [HttpGet, Route("{id}")]
        public async Task<List<string>> Get(string id)
        {
            var grain = GrainFactory.GetGrain<ISmsGrain>(id);
            return await grain.GetMessages();
        }

        /// <summary>
        /// Sends a message.
        /// </summary>
        /// <param name="id">The id of the recipient.</param>
        /// <param name="message">The new password.</param>
        /// <returns>A <see cref="Task"/> representing the work performed.</returns>
        [HttpPost, Route("{id}")]
        public async Task Send(string id, string message)
        {
            var grain = GrainFactory.GetGrain<ISmsGrain>(id);
            await grain.NewMessage(message);
        }
    }
}