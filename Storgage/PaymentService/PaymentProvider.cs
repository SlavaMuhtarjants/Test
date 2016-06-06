namespace Weezlabs.Storgage.PaymentService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;

    using DataLayer;
    using DataTransferObjects.Stripe;
    using Model;
    using Model.Exceptions;
    using SecurityService;
    using StripeService;

    /// <summary>
    /// Payment provider
    /// </summary>
    public class PaymentProvider : IPaymentProvider
    {
        private readonly IStripeProvider stripeProvider;
        private readonly IAuthProvider authProvider;
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// Create instance of payment controller
        /// </summary>
        /// <param name="stripeProvider">Stripe provider.</param>
        /// <param name="authProvider">Authentification provider.</param>
        /// <param name="unitOfWork">Unit of work.</param>
        public PaymentProvider(IStripeProvider stripeProvider, IAuthProvider authProvider, IUnitOfWork unitOfWork)
        {
            Contract.Requires(stripeProvider != null);
            Contract.Requires(authProvider != null);
            Contract.Requires(unitOfWork != null);

            this.stripeProvider = stripeProvider;
            this.authProvider = authProvider;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get cards for user
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>List of stripe card items.</returns>
        public async Task<List<StripeCardItem>> GetCardsList(Guid userId)
        {
            User user = GetUser(userId);

            return await stripeProvider.GetCardsList(user.StripeCustomerId);
        }

        /// <summary>
        /// Get card by identifier.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="cardId">Card identifier.</param>
        /// <returns>Card item.</returns>
        public async Task<StripeCardItem> GetCard(Guid userId, String cardId)
        {
            User user = GetUser(userId);

            var customerInfo = new StripeCustomerInfo(user)
            {
                CustomerCard = new StripeCustomerSource() {CardId = cardId}
            };

            return await stripeProvider.GetCard(customerInfo);
        }

        /// <summary>
        /// Add card to customer
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="cardToken">Stripe card token.</param>
        /// <param name="setDefault">True if should set new card as default.</param>
        /// <returns>Stripe card.</returns>
        public async Task<StripeCardItem> AddCard(Guid userId, String cardToken, Boolean setDefault = false)
        {
            User user = GetUser(userId);

            var customerInfo = new StripeCustomerInfo(user)
            {
                CustomerCard = new StripeCustomerSource() {CardToken = cardToken, SetDefault = setDefault}
            };

            var updatedCustomer = await stripeProvider.AddCard(customerInfo);

            // note: we should update user.customerId if we are create new customer on stripe
            // it's possible if we are delete customer from stripe manually
            if (String.IsNullOrWhiteSpace(user.StripeCustomerId) || updatedCustomer.IsNew)
            {
                user.StripeCustomerId = updatedCustomer.CustomerId;
                unitOfWork.CommitChanges();
            }

            return updatedCustomer.CustomerCard.Card;
        }

        /// <summary>
        /// Delete customer's card 
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="cardId">Stripe card identifier.</param>
        public void DeleteCard(Guid userId, String cardId)
        {
            User user = GetUser(userId);

            var customerInfo = new StripeCustomerInfo(user)
            {
                CustomerCard = new StripeCustomerSource() {CardId = cardId}
            };

            try
            {
                stripeProvider.DeleteCard(customerInfo);
            }
            catch (NotFoundException)
            {
                // we have no customer for user at now
                // note: if we have customer, but have no card with required cardId 
                // we handle exception like Exception in controller
                throw new NotFoundException(String.Format(Resources.Messages.CardNotFound, cardId));
            }
        }

        /// <summary>
        /// Set default payment for customer
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="cardId">Stripe card identifier.</param>
        public void SetDefaultPayment(Guid userId, String cardId)
        {
            User user = GetUser(userId);

            var customerInfo = new StripeCustomerInfo(user)
            {
                CustomerCard = new StripeCustomerSource() {CardId = cardId, SetDefault = true}
            };

            try
            {
                stripeProvider.SetDefaultPayment(customerInfo);
            }
            catch (NotFoundException)
            {
                // we have no customer for user at now
                // note: if we have customer, but have no card with required cardId 
                // we handle exception like Exception in controller
                throw new NotFoundException(String.Format(Resources.Messages.CardNotFound, cardId));
            }
        }

        /// <summary>
        /// Get User by user identifier.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>User entity model.</returns>
        private User GetUser(Guid userId)
        {
            User user = authProvider.GetUser(userId).Result;

            if (user == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.UserNotFound, userId));
            }

            return user;
        }
        
        /// <summary>
        /// Get bank account.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>Bank account item.</returns>
        public async Task<StripeBankAccountItem> GetBankAccount(Guid userId)
        {
            User user = GetUser(userId);

            var customerInfo = new StripeCustomerInfo(user);
            
            if (String.IsNullOrWhiteSpace(customerInfo.AccountId))
            {
                // we have no bank account for not exists managed account
                throw new NotFoundException(Resources.Messages.BankAccountNotFound);
            }
            
            return await stripeProvider.GetBankAccount(customerInfo);
        }

        /// <summary>
        /// Add bank account to managed account.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="bankAccountToken">Stripe bank acoount token.</param>
        /// <returns>Bank account item.</returns>
        public async Task<StripeBankAccountItem> AddBankAccount(Guid userId, String bankAccountToken)
        {
            User user = GetUser(userId);

            var customerInfo = new StripeCustomerInfo(user)
            {
                CustomerBankAccount = new StripeBankAccountInfo()
                {
                    StripeBankAccountToken = bankAccountToken
                }
            };

            var updatedCustomer = await stripeProvider.AddBankAccount(customerInfo);

            // note: we should update user.stripeAccountId if we are create new account on stripe
            // it's possible if we are delete account from stripe manually
            if (String.IsNullOrWhiteSpace(user.StripeAccountId ) || updatedCustomer.IsNew)
            {
                user.StripeAccountId = updatedCustomer.AccountId;
                unitOfWork.CommitChanges();
            }

            return updatedCustomer.CustomerBankAccount.StripeBankAccount;
        }

        /// <summary>
        /// Get debit card.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>Debit card item.</returns>
        public async Task<StripeCardItem> GetDebitCard(Guid userId)
        {
            User user = GetUser(userId);

            var customerInfo = new StripeCustomerInfo(user);

            if (String.IsNullOrWhiteSpace(customerInfo.AccountId))
            {
                // we have no cards for not exists managed account
                throw new NotFoundException(Resources.Messages.DebitCardNotFound);
            }

            return await stripeProvider.GetDebitCard(customerInfo);
        }

        /// <summary>
        /// Add debit card to managed account
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="debitCardToken">Stripe debit card token.</param>
        /// <returns>Debit card item.</returns>
        public async Task<StripeCardItem> AddDebitCard(Guid userId, String debitCardToken)
        {
            User user = GetUser(userId);

            var customerInfo = new StripeCustomerInfo(user)
            {
                CustomerCard = new StripeCustomerSource()
                {
                    CardToken = debitCardToken
                }
            };

            var updatedCustomer = await stripeProvider.AddDebitCard(customerInfo);

            // note: we should update user.stripeAccountId if we are create new account on stripe
            // it's possible if we are delete account from stripe manually
            if (String.IsNullOrWhiteSpace(user.StripeAccountId) || updatedCustomer.IsNew)
            {
                user.StripeAccountId = updatedCustomer.AccountId;
                unitOfWork.CommitChanges();
            }

            return updatedCustomer.CustomerCard.Card;
        }

        /// <summary>
        /// Delete debit card
        /// </summary>
        /// <param name="userId">User identifier.</param>
        public void DeleteDebitCard(Guid userId)
        {
            User user = GetUser(userId);

            var customerInfo = new StripeCustomerInfo(user);

            try
            {
                stripeProvider.DeleteDebitCard(customerInfo);
            }
            catch (NotFoundException)
            {
                // we have no customer for user at now
                // note: if we have customer, but have no card with required cardId 
                // we handle exception like Exception in controller
                throw new NotFoundException(Resources.Messages.DebitCardNotFound);
            }
        }
    }
}
