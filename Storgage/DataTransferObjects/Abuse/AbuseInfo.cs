namespace Weezlabs.Storgage.DataTransferObjects.Abuse
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Weezlabs.Storgage.Model;
    using Weezlabs.Storgage.Model.Enums;

    using AbuseTypeDictionaryEn = Weezlabs.Storgage.Model.Enums.AbuseTypeDictionary;
    using ContactUsDictionaryEn = Weezlabs.Storgage.Model.Enums.ContactUsDictionary;


    public class AbuseInfo : AbuseInternalRequest
    {
        public Guid Id { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AbuseInfo()
        {

        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AbuseInfo(Abuse abuse)
        {
            Id = abuse.Id;
            Message = abuse.Message;
            ReporterId = abuse.ReporterId;

            var space = abuse.AbuseSpaces.SingleOrDefault();
            SpaceId = (space != null) ? (Guid?)space.SpaceId : null;

            var rating = abuse.AbuseRatings.SingleOrDefault();
            RatingId = (rating != null) ? (Guid?)rating.RatingId : null;

            //AbuseType = abuse.AbuseTypes.Select(x => x.AbuseTypeDictionary.ToEnumAbuse()).Where(x => x != null).Select( x => (Weezlabs.Storgage.Model.Enums.AbuseTypeDictionary)x);
            //ContactUsType = abuse.AbuseTypes.Select(x => x.AbuseTypeDictionary.ToEnumContactUs()).Where( x => x != null).Select(x => (Weezlabs.Storgage.Model.Enums.ContactUsDictionary)x).SingleOrDefault();

            var allAbuses = abuse.AbuseTypes.Select(x => x.AbuseTypeDictionary.Synonym).ToList();

            ContactUsDictionaryEn currentContactUs;
            AbuseTypeDictionaryEn currentAbuseType;
            List<AbuseTypeDictionaryEn> abuseTypeList = new List<AbuseTypeDictionaryEn>();

            foreach (var c in allAbuses)
            {               
               if (Enum.TryParse(c, true, out currentContactUs))
               {
                   ContactUsType = currentContactUs;
               }

               if (Enum.TryParse(c, true, out currentAbuseType ))
               {
                   abuseTypeList.Add(currentAbuseType);
               }
            }

           AbuseType =  abuseTypeList.Count() > 0 ? abuseTypeList.AsEnumerable(): null;

        }
    }
}
