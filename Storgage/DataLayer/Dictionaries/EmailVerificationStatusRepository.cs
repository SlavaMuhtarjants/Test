namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// Email verification statuses repository.
    /// </summary>
    public class EmailVerificationStatusRepository : BaseReadonlyRepository<EmailVerificationStatus, Guid>,
        IEmailVerificationStatusReadonlyRepository
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="context">Database context.</param>
        public EmailVerificationStatusRepository(DbContext context) : base(context)
        {
        }
    }
}
