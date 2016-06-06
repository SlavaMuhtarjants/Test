namespace Weezlabs.Storgage.DataTransferObjects.Abuse
{
    using System;

    public class AbuseInternalRequest : AbuseRequest
    {

        /// <summary>
        /// Possible link to space
        /// </summary>
        public Guid? SpaceId { get; set; }

        /// <summary>
        /// Possible link to rating
        /// </summary>
        public Guid? RatingId { get; set; }
        
        /// <summary>
        /// Default constructor.
        /// </summary>
        public AbuseInternalRequest()
        {

        }

        public AbuseInternalRequest (AbuseRequest abuseRequest)
        {
            AbuseType = abuseRequest.AbuseType;
            Message = abuseRequest.Message;
            ReporterId = abuseRequest.ReporterId;
            ContactUsType = abuseRequest.ContactUsType;
        }
    }
}
