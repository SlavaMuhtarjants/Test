namespace Weezlabs.Storgage.DataTransferObjects.Rating
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Weezlabs.Storgage.DataTransferObjects.User;

    public class RatingInfoForUser
    {
        public RatingInfoForUser(Model.User user)
        {
            Contract.Requires(user != null);
            ReviewedUser = new UserFullInfo(user);
        }

        public UserFullInfo ReviewedUser { get; private set; }
        public List<RatingInfo> Ratings { get; set; }
    }
}
