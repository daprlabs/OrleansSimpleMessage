// --------------------------------------------------------------------------------------------------------------------
// <summary>
//   State of the recipe Grain.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SimpleMessage.Grains
{
    using System.Collections.Generic;

    using Orleans;

    /// <summary>
    /// State of the recipe Grain.
    /// </summary>
    public interface IMessageState : IGrainState
    {
        /// <summary>
        /// Gets or sets the list of messages.
        /// </summary>
        List<string> Messages { get; set; }
    }

}