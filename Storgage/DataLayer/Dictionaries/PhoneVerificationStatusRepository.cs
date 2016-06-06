namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// Phone verification statuses repository.
    /// </summary>
    public class PhoneVerificationStatusRepository : BaseReadonlyRepository<PhoneVerificationStatus, Guid>, 
        IPhoneVerificationStatusReadonlyRepository
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="context">Database context.</param>
        public PhoneVerificationStatusRepository(DbContext context)
            : base(context)
        {
        }
    }
}
