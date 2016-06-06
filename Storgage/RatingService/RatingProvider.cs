namespace Weezlabs.Storgage.RatingService
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using DataLayer;
    using DataLayer.ChatAndMessages;
    using DataLayer.Ratings;
    using DataTransferObjects.Rating;
    using DataLayer.Users;
    using Model.Exceptions;
    using Model.ModelExtension;
    using RatingDB = Model.Rating;
    using Weezlabs.Storgage.DataLayer.Dictionaries;

    public class RatingProvider : IRatingProvider
    {
        private readonly IRatingRepository ratingRepository;
        private readonly IUserReadonlyRepository userRepository;
        private readonly IOfferRepository offerRepository;
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Create instance of rating searcher.
        /// </summary>
        /// <param name="ratingRepository">Rating repository.</param>
        /// <param name="userRepository">User repository.</param>
        /// <param name="offerRepository">Offer repository.</param>
        /// 
        public RatingProvider(
            IRatingRepository ratingRepository,
            IUserReadonlyRepository userRepository,
            IOfferRepository offerRepository,
            IUnitOfWork unitOfWork)
        {
            Contract.Requires(ratingRepository != null);
            Contract.Requires(userRepository != null);
            Contract.Requires(offerRepository != null);
            Contract.Requires(unitOfWork != null);

            this.ratingRepository = ratingRepository;
            this.userRepository = userRepository;
            this.offerRepository = offerRepository;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Returns information about all reatings for user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="limit">Limit.</param>
        /// <returns></returns>
        public RatingInfoForUser GetRatings(Guid userId, Int32 offset, Int32 limit)
        {            
            IQueryable<RatingDB> pagedRating = ratingRepository.GetAll()
                .Where(x => x.MessageOfferHistory.MessageOffer.Message.Chat.Space.UserId == userId)                
                .OrderBy(x => x.CreatedDate)
                .Paging(offset, limit);

            var resultList = (

                from u  in userRepository.GetAll()
                join ur in pagedRating
                  on u.Id equals ur.MessageOfferHistory.MessageOffer.Message.Chat.Space.UserId
                into outer
                from item
                  in outer.DefaultIfEmpty()
               where u.Id == userId
                select new { PagedRating = item, RatedUser = u, Reviewer = item.MessageOfferHistory.MessageOffer.Message.Chat.User }).ToList();

            if (resultList.Count == 0)
            {
                return null;
            }

            if (resultList.Count == 1 && resultList[0].PagedRating == null)
            {
                return new RatingInfoForUser(resultList[0].RatedUser);
            }

            return new RatingInfoForUser(resultList[0].RatedUser) { Ratings = resultList.Select(x => new RatingInfo(x.PagedRating, x.Reviewer)).ToList() };
        }

        /// <summary>
        /// Posts rating.
        /// </summary>
        /// <param name="offerId">Offer identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <param name="request">Post rating request.</param>
        /// <returns>Created rating.</returns>
        public RatingInfo PostRating(Guid offerId, Guid actorId, RateUserOfferRequest request)
        {
            try
            {
                var chat = offerRepository.GetAll()
                 
                    .Where(x => x.Id == offerId)
                    .Select(x => x.Message)
                    .Select(x => x.Chat)

                    //We need to define chat and after we have two variants
                    //1) define current state for approved ApprovedMessageOfferHistory
                    //.Include(x => x.Message.Chat.ApprovedMessageOfferHistory.MessageOffer.CurrentMessageOfferState.MessageOfferStatus)
                    //or
                    //2) define LastMessage and it's state                    
                    .Include(x => x.LastMessageOffer.MessageOfferHistory)
                    .SingleOrDefault();
                    

                if (chat == null)
                {
                    //Old message because we pass "offerId" still but we need ChatId 
                    throw new NotFoundException(String.Format(Resources.Messages.OfferNotFound, offerId));
                }

                if (chat.UserId != actorId)
                {
                    throw new AccessDeniedException();
                }

                var approvedMessageOfferHistoryId = chat.ApprovedMessageOfferHistoryId;

                if (chat.LastMessageOffer.MessageOfferHistory.StatusId != Model.Enums.MessageOfferStatus.Stopped.GetDictionaryId()
                    || 
                    approvedMessageOfferHistoryId == null //Chat must have approved offer
                    )
                {
                    throw new BadRequestException(Resources.Messages.RateOfferError);
                }

                var newRating = new RatingDB
                {
                    Message = request.Message,
                    Rank = request.Rank,
                    CreatedDate = DateTime.UtcNow,
                    Id = (Guid)approvedMessageOfferHistoryId //We can store Rate for approved offer history ONLY
                };
                ratingRepository.Add(newRating);
                unitOfWork.CommitChanges();
                var rating = GetRatingInfoInternal(newRating.Id, actorId);
                var result = new RatingInfo(rating);

                return result;
            }
            catch (DataException ex)
            {
                if (ex.ToString().Contains("pk_Rating"))
                {
                    throw new BadRequestException(Resources.Messages.RatingWasPostedAlways);
                }
                throw;
            }
        }

        /// <summary>
        /// Replies on rating.
        /// </summary>
        /// <param name="ratingId">Rating identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <param name="request">reply on rating request.</param>
        /// <returns>Rating.</returns>
        public RatingInfo ReplyOnRating(Guid ratingId, Guid actorId, ReplyOnRatingRequest request)
        {
            var rating = GetRatingInfoInternal(ratingId, actorId);            

            if (rating.MessageOfferHistory.MessageOffer.Message.Chat.Space.UserId != actorId)
            {
                throw new AccessDeniedException();
            }

            rating.SpaceOwnerReply = request.Reply;
            rating.RepliedAt = DateTimeOffset.Now;
            ratingRepository.Update(rating);
            unitOfWork.CommitChanges();
            var result = new RatingInfo(rating, rating.MessageOfferHistory.MessageOffer.Message.Chat.User);
            return result;
        }

        /// <summary>
        /// Returns rating by its identifier.
        /// </summary>
        /// <param name="ratingId">Rating identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Rating.</returns>
        public RatingInfo GetRatingInfo(Guid ratingId, Guid actorId)
        {
            var rating = GetRatingInfoInternal(ratingId, actorId);

            if (!rating.MessageOfferHistory.MessageOffer.Message.Chat.HasUserAccess(actorId))
            {
                throw new AccessDeniedException();
            }

            var result = new RatingInfo(rating, rating.MessageOfferHistory.MessageOffer.Message.Chat.User);
            return result;
        }

        private RatingDB GetRatingInfoInternal(Guid ratingId, Guid actorId)
        {
            var rating = ratingRepository.GetAll()
                .Include(x => x.MessageOfferHistory.MessageOffer.Message.Chat.Space)
                .Include(x => x.MessageOfferHistory.MessageOffer.Message.Chat.User)
                .Include(x => x.MessageOfferHistory.MessageOffer.Message.Chat.ChatMembers)
                .SingleOrDefault(x => x.Id == ratingId);

            if (rating == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.RatingNotFound, ratingId));
            }

            return rating;
        }
    }
}
