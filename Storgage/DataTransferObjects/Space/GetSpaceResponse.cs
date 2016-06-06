namespace Weezlabs.Storgage.DataTransferObjects.Space
{
    using System; 

    using User;


    /// <summary>
    /// Contains information about space.
    /// </summary>
    public class GetSpaceResponse : BaseGetSpaceResponse
    {
        /// <summary>
        /// Returns owner.
        /// </summary>
        public UserInfo Owner { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GetSpaceResponse()
        {
        }

        /// <summary>
        /// Create Dto from model.
        /// </summary>
        /// <param name="space">Model object.</param>
        /// <param name="displayContactInfo">Display contact info.</param>
        public GetSpaceResponse(Model.Space space, Boolean displayContactInfo = false):base(space)
        {
            Owner = displayContactInfo ? new UserFullInfo(space.User) : new UserInfo(space.User);
        }
    }
}