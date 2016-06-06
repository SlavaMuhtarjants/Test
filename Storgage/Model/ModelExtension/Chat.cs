namespace Weezlabs.Storgage.Model
{
    using System;
    using System.Linq;

    using Contracts;

    /// <summary>
    /// Chat extension.
    /// </summary>
    public partial class Chat : IAclVerifiable<Guid>
    {        
        /// <summary>
        /// Checks that user has access to chat.
        /// </summary>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>True if user has access to chat.</returns>
        public bool HasUserAccess(Guid actorId)
        {
            var result = ChatMembers.Any(x => x.UserId == actorId);
            return result;
        }        
    }
}
