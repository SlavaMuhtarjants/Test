namespace Weezlabs.Storgage.DataLayer.Abuse
{
    using Model;
    using System;
    using System.Linq;
    using System.Data.Entity;
    using EntityFramework.Extensions;

    /// <summary>
    /// Readonly repository for ratings.
    /// </summary>
    public class AbuseRepository : BaseRepository<Abuse, Guid>, IAbuseRepository
    {     

        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="userRepository">User repository.</param>
        public AbuseRepository(DbContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Method updates single attribute "FileName" for abuse
        /// </summary>
        /// <param name="abuseId">Abuse Id</param>
        /// <param name="fileName">File Name</param>
        public void UpdateFile (Guid abuseId, String fileName)
        {
            this.GetAll()
                .Where(a => a.Id == abuseId)
                .Update(a => new Abuse {FileName = fileName});           
        }

    }
}
