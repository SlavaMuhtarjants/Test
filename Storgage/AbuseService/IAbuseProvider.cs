namespace Weezlabs.Storgage.AbuseService
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Weezlabs.Storgage.DataTransferObjects.Abuse;
    using Weezlabs.Storgage.DataTransferObjects.Misc;

    public interface IAbuseProvider
    {
        /// <summary>
        /// Posts Abuse.
        /// </summary>
        /// <param name="request">Post Abuse Request.</param>
        /// <returns>Created abuse.</returns>
        AbuseInfo PostAbuse(AbuseInternalRequest request);

        /// <summary>
        /// Get Abuse.
        /// </summary>
        /// <param name="abuseId">abuse id.</param>
        /// <returns>Existing abuse.</returns>
        AbuseInfo GetAbuse(Guid abuseId);

        /// <summary>
        /// Upload file for abuse.
        /// </summary>
        /// <param name="abuseId">Abuse identifier.</param>
        /// <param name="userId">autorised User Id.</param>
        /// <param name="formData">IEnumerable of files.</param>
        UploadedFile UploadFile(Guid abuseId, Guid userId, IEnumerable<HttpContent> formData);
    }
}
