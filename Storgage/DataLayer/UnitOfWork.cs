namespace Weezlabs.Storgage.DataLayer
{
    using System;
    using System.Data.Entity;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Unit of work implementation.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext context;

        /// <summary>
        /// Create instance of unit of work.
        /// </summary>
        /// <param name="context">Database context.</param>
        public UnitOfWork(DbContext context)
        {
            Contract.Requires(context != null);
            this.context = context;
        }

        /// <summary>
        /// Commits all changes in repositories.
        /// <returns>Number of entities that were commited.</returns>
        /// </summary>       
        public Int32 CommitChanges()
        {
            return context.SaveChanges();
        }
    }
}
