namespace Weezlabs.Storgage.DataLayer.Abuse
{
    using Model;
    using System;

    /// <summary>
    /// Interface for rating repository.
    /// </summary>
    public interface IAbuseRepository : IRepository<Abuse, Guid>
    {
        /// <summary>
        /// Method updates single attribute "FileName" for abuse
        /// </summary>
        /// <param name="abuseId">Abuse Id</param>
        /// <param name="fileName">File Name</param>
        void UpdateFile(Guid abuseId, String fileName);
    }
}