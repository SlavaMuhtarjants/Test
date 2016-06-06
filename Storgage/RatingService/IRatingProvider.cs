namespace Weezlabs.Storgage.RatingService
{
    using System;
    using System.Collections.Generic;

    using DataTransferObjects.Rating;

    public interface IRatingProvider
    {
        /// <summary>
        /// Ratings provider.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="limit">Limit.</param>
        /// <returns>Searched spaces.</returns>
        RatingInfoForUser GetRatings(Guid userId, Int32 offset, Int32 limit);

        /// <summary>
        /// Posts rating.
        /// </summary>
        /// <param name="offerId">Offer identifier.</param>
        /// <param name="actorId">Actor dentifier.</param>
        /// <param name="request">Post rating request.</param>
        /// <returns>Created rating.</returns>
        RatingInfo PostRating(Guid offerId, Guid actorId, RateUserOfferRequest request);

        /// <summary>
        /// Replies on rating.
        /// </summary>
        /// <param name="ratingId">Rating identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <param name="request">reply on rating request.</param>
        /// <returns>Rating.</returns>
        RatingInfo ReplyOnRating(Guid ratingId, Guid actorId, ReplyOnRatingRequest request);

        /// <summary>
        /// Returns rating by its identifier.
        /// </summary>
        /// <param name="ratingId">Rating identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Rating.</returns>
        RatingInfo GetRatingInfo(Guid ratingId, Guid actorId);
    }
}
