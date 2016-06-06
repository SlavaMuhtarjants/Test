namespace Weezlabs.Storgage.DataLayer.Ratings
{
    using System;
    using System.Data.Entity;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using DataLayer.Users;
    using Model;

    /// <summary>
    /// Readonly repository for ratings.
    /// </summary>
    public class RatingRepository : BaseRepository<Rating, Guid>, IRatingRepository
    {
        private readonly IUserReadonlyRepository userRepository;

        /// <summary>
        /// Create instance.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="userRepository">User repository.</param>
        public RatingRepository(DbContext context, IUserReadonlyRepository userRepository)
            : base(context)
        {
            Contract.Requires(userRepository != null);

            this.userRepository = userRepository;
        }

        /// <summary>
        /// Returns ratings.
        /// </summary>
        /// <returns>Ratings.</returns>
        protected override IQueryable<Rating> DoGetAll()
        {
            return base.DoGetAll().Include(x => x.MessageOfferHistory.MessageOffer.Message.Chat.Space.User)
                .Include(x => x.MessageOfferHistory.MessageOffer.Message.Chat.User);
        }

    }
}
