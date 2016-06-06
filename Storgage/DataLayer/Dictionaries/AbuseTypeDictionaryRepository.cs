namespace Weezlabs.Storgage.DataLayer.Dictionaries
{
    using System;
    using System.Data.Entity;

    using Model;

    /// <summary>
    /// Repository for AbuseTypeDictionary.
    /// </summary>
    public class AbuseTypeDictionaryRepository : BaseReadonlyRepository<AbuseTypeDictionary, Guid>, IAbuseTypeDictionaryReadonlyRepository
    {
        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="context">Database context.</param>
        public AbuseTypeDictionaryRepository(DbContext context)
            : base(context)
        {
        }
    }
}