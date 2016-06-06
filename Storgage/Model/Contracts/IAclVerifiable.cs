namespace Weezlabs.Storgage.Model.Contracts
{
    using System;

    /// <summary>
    /// Interface for resource ACL that should be verified.
    /// </summary>
    /// <typeparam name="TKey">User  and resource key type.</typeparam>
    public interface IAclVerifiable<TKey>
    {
        /// <summary>
        /// Checks that actor has access to current resource.
        /// </summary>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>True if actor has access to current resource.</returns>
        Boolean HasUserAccess(TKey actorId);
    }
}
