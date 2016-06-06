namespace Weezlabs.Storgage.DataTransferObjects.Space
{
    using System;
    
    using User;

    /// <summary>
    /// Contains information about space.
    /// </summary>
    public class GetSpaceResponseForOwner : BaseGetSpaceResponse
    {
        public Boolean IsListed { get; set; }
        /// <summary>
        /// This attribute can't be changed by user but must be used when we try to change IsListed manually 
        /// they work together (on DB level by triggers on MessageOffer table) but IsListed may be changed by user when IsOccupied = false
        /// </summary>
        public Boolean IsOccupied { get; set; }

        /// <summary>
        /// Full Address Information
        /// </summary>
        public String FullAddress { get; set; }

        /// <summary>
        /// Information about owner.
        /// </summary>
        public UserInfo Owner { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GetSpaceResponseForOwner()
        {
        }

        /// <summary>
        /// Create Dto from model.
        /// </summary>
        /// <param name="space">Model object.</param>
        public GetSpaceResponseForOwner(Model.Space space)
            : base(space)
        {
            IsListed = space.IsListed;
            IsOccupied = space.IsOccupied;

            FullAddress = space.FullAddress;
            Owner = new UserInfo(space.User);       
        }
    }
}
