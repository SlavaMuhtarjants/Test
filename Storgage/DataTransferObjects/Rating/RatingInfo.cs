namespace Weezlabs.Storgage.DataTransferObjects.Rating
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using User;
    using RatingDB = Model.Rating;
    using UserDB = Model.User;

    /// <summary>
    /// Contains information about estimation of user.
    /// </summary>
    public class RatingInfo : RateUserOfferRequest
    {
        /// <summary>
        /// Public + contact information about reviewer.
        /// </summary>
        public UserInfo Reviewer { get; set; }

        /// <summary>
        /// Date time when rating was posted.
        /// </summary>
        public DateTimeOffset RatedAt { get; set; }

        /// <summary>
        /// Reply of user, who was rated.
        /// </summary>
        public String Reply { get; set; }

        /// <summary>
        /// Date time of reply.
        /// </summary>
        public DateTimeOffset? RepliedAt { get; set; }

        /// <summary>
        /// Rating identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RatingInfo()
        {

        }

        /// <summary>
        /// Creates rating dto from model.
        /// </summary>
        /// <param name="ratingDB">Model object.</param>
        public RatingInfo(RatingDB ratingDB, UserDB reviewer = null)
        {
            Contract.Requires(ratingDB != null);
            Id = ratingDB.Id;
            Rank = ratingDB.Rank;
            Message = ratingDB.Message;
            RatedAt = ratingDB.CreatedDate;
            Reviewer = new UserInfo(reviewer??ratingDB.MessageOfferHistory.MessageOffer.Message.Chat.User);
            Reply = ratingDB.SpaceOwnerReply;
            RepliedAt = ratingDB.RepliedAt;
        }
    }
}