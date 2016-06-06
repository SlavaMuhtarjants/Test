namespace Weezlabs.Storgage.SecurityService
{
    using System.Diagnostics.Contracts;

    using DataLayer;
    using DataLayer.Dictionaries;
    using DataLayer.Users;            

    /// <summary>
    /// User store for integration ASP Identity.
    /// </summary>
    public partial class UserIdentityStore        
    {

        private readonly IUserRepository userRepository;
        private readonly IDictionaryProvider dictionaryProvider;
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Create User Identity Store instance.
        /// </summary>
        /// <param name="userRepository">User repository.</param>        
        /// <param name="dictionaryProvider">Dictionary provider.</param>
        public UserIdentityStore(IUserRepository userRepository,
            IDictionaryProvider dictionaryProvider,
            IUnitOfWork unitOfWork)
        {
            Contract.Requires(userRepository != null);
            Contract.Requires(dictionaryProvider != null);
            Contract.Requires(unitOfWork != null);

            this.userRepository = userRepository;            
            this.dictionaryProvider = dictionaryProvider;
            this.unitOfWork = unitOfWork;
        }                
    }
}
