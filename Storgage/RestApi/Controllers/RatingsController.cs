namespace Weezlabs.Storgage.RestApi.Controllers
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Web.Http;
    using System.Web.Http.Description;

    using DataTransferObjects.Rating;
    using RatingService;
    using Resources;
    using Helpers;
    using Model.Exceptions;


    /// <summary>
    /// Provides possibility to:
    /// - get total rating of user
    /// - get all ratings of user
    /// - get the worst ratings of user
    /// - get the best ratings of user
    /// - rate user offer.
    /// </summary>
    [RoutePrefix("api")]
    public class RatingsController : ApiController
    {
        private readonly IRatingProvider ratingProvider;

        /// <summary>
        /// Create controller.
        /// </summary>
        /// <param name="ratingProvider">Space Searcher.</param>
        public RatingsController(IRatingProvider ratingProvider)
        {
            Contract.Requires(ratingProvider != null);
            this.ratingProvider = ratingProvider;
        }


        /// <summary>
        /// Returns all ratings of user.
        /// </summary>
        /// <param name="userId">Identifier of user that will be estimated.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="limit">Limit.</param>
        /// <returns>Enumerable of ratings.</returns>
        /// <response code="200">Ok.</response>
        /// <response code="404">User not found.</response>
        [HttpGet]
        [Route("users/{userId}/ratings/all")]
        [ResponseType(typeof(RatingInfoForUser))]
        [AllowAnonymous]
        public IHttpActionResult GetAllRatingsOfUser(Guid userId, Int32 offset = 0, Int32 limit = 0)
        {
            RatingInfoForUser result;
            try
            {
                result = ratingProvider.GetRatings(userId, offset, limit);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(ex.Message);
            }

            if (result == null)
            {
                return this.NotFound(String.Format(Messages.UserNotFound, userId.ToString()));
            }

            return Ok<RatingInfoForUser>(result);
        }

        /// <summary>
        /// Rates user offer.
        /// <param name="userId">User identifier.</param>
        /// <param name="offerId">Offer identifier.</param>
        /// <param name="request">Request for posting new rate.</param>
        /// </summary>
        /// <returns>Posted rating.</returns>
        /// <response code="201">Rating was posted.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden.</response>
        /// <response code="404">User or offer cannot be found.</response>
        [HttpPost]
        [Route("users/{userId}/rate/offers/{offerId}")]
        [Authorize]
        [ResponseType(typeof(RatingInfo))]
        public IHttpActionResult RateUserOffer(Guid userId, Guid offerId, [FromBody] RateUserOfferRequest request)
        {
            if (request == null)
            {
                throw new BadRequestException(Messages.PostBodyCannotBeNull);
            }
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }

            try
            {             
                var postedRating = ratingProvider.PostRating(offerId, userId, request);
                var location = new Uri(new Uri(Request.RequestUri, RequestContext.VirtualPathRoot),
                       String.Format("api/ratings/{0}", postedRating.Id));
                return Created<RatingInfo>(location, postedRating);
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(Messages.AccessDenied);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Returns rating.
        /// </summary>
        /// <param name="ratingId">Rating identifier.</param>
        /// <returns>Rating.</returns>
        /// <response code="200">Rating was returned.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden.</response>
        /// <response code="404">Rating cannot be found.</response>
        [HttpGet]
        [Route("ratings/{ratingId}")]
        [Authorize]
        [ResponseType(typeof(RatingInfo))]
        public IHttpActionResult GetRating(Guid ratingId)
        {
            try
            {
                var actorId = RequestContext.Principal.GetUserIdFromClaim();
                var result = ratingProvider.GetRatingInfo(ratingId, actorId);
                return Ok<RatingInfo>(result);
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(Messages.AccessDenied);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
        }
        /// <summary>
        /// Replies on ratings.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="ratingId">Rating identifier.</param>
        /// <param name="request">Request for replying on rating.</param>
        /// <returns>Rating with reply.</returns>
        /// <response code="200">Reply was posted.</response>
        /// <response code="400">Bad request.</response>
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Forbidden.</response>
        /// <response code="404">Rating cannot be found.</response>
        [HttpPut]
        [Route("users/{userId}/reply-on/ratings/{ratingId}")]
        [Authorize]
        [ResponseType(typeof(RatingInfo))]
        public IHttpActionResult ReplyOnRating(Guid userId, Guid ratingId, [FromBody] ReplyOnRatingRequest request)
        {
            if (request == null)
            {
                throw new BadRequestException(Messages.PostBodyCannotBeNull);
            }

            if (String.IsNullOrWhiteSpace(request.Reply))
            {
                throw new BadRequestException(Messages.ReplyOnRatingEmptyError);
            }

            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            try
            {
                var actorId = RequestContext.Principal.GetUserIdFromClaim();
                var rating = ratingProvider.ReplyOnRating(ratingId, actorId, request);
                return Ok<RatingInfo>(rating);
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(Messages.AccessDenied);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
        }
    }
}
