namespace Weezlabs.Storgage.DataTransferObjects.Rating
{
    using System.Collections.Generic;
    using User;

    /// <summary>
    /// User ratings response.
    /// </summary>
    public class UserRatingsResponse
    {
        /// <summary>
        /// Info about rated user.
        /// </summary>
        public UserInfo ReviewedUser { get; set; }

        /// <summary>
        /// Ratings for rated user.
        /// </summary>
        public IEnumerable<RatingInfo> Ratings { get; set; }
    }
}
