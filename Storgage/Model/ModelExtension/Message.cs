namespace Weezlabs.Storgage.Model
{        
    using System;
    using System.Linq;

    using Contracts;    

    /// <summary>
    /// Message extension.
    /// </summary>
    public partial class Message : IAclVerifiable<Guid>
    {        
        /// <summary>
        /// Checks that user has access to this message.
        /// </summary>
        /// <param name="actorId">User identifier.</param>
        /// <returns>True if user has access to message.</returns>
        public Boolean HasUserAccess(Guid actorId)
        {
            var result = Chat.ChatMembers.Any(x => x.UserId == actorId);
            return result;
        }
    }
}
