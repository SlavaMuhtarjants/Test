namespace Weezlabs.Storgage.DataLayer
{
    using System;

    /// <summary>
    /// Interface for unit of work.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Commits all changes in repositories.
        /// <returns>Number of entities that were commited.</returns>
        /// </summary>        
        Int32 CommitChanges();
    }
}
