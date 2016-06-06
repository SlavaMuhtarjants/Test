namespace Weezlabs.Storgage.DataTransferObjects.Space
{
  
    using System;    

    /// <summary>
    /// Contains information about request for publish new space
    /// </summary>
    public class PublishSpaceRequest : SpaceInfo 
    {                     
        /// <summary>
        /// If it is true, then space will be accessible for search.
        /// </summary>
        public Boolean IsListed { get; set; }

        /// <summary>
        /// Full Address Information
        /// </summary>
        public String FullAddress { get; set; }
  
        public PublishSpaceRequest()
            : base()
        {

        }

    }
}