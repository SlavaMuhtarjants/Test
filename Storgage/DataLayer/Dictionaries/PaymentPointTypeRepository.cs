namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// Repository for size types.
    /// </summary>
    public class PaymentPointTypeRepository : BaseReadonlyRepository<PaymentPointType, Guid>, IPaymentPointTypeReadonlyRepository
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="context">Database context.</param>
        public PaymentPointTypeRepository(DbContext context)
            : base(context)
        {
        }
    }
}
