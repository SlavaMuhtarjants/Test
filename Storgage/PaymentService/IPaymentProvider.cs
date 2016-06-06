namespace Weezlabs.Storgage.PaymentService
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using DataTransferObjects.Stripe;

    /// <summary>
    /// Interface for payment provider
    /// </summary>
    public interface IPaymentProvider
    {
        /// <summary>
        /// Get cards for user
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>List of stripe card items.</returns>
        Task<List<StripeCardItem>> GetCardsList(Guid userId);

        /// <summary>
        /// Get card by identifier.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="cardId">Card identifier.</param>
        /// <returns>Card item.</returns>
        Task<StripeCardItem> GetCard(Guid userId, String cardId);

        /// <summary>
        /// Add card to customer
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="cardToken">Stripe card token.</param>
        /// <param name="setDefault">True if should set new card as default.</param>
        /// <returns>Stripe card.</returns>
        Task<StripeCardItem> AddCard(Guid userId, String cardToken, Boolean setDefault = false);

        /// <summary>
        /// Delete customer's card 
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="cardId">Stripe card identifier.</param>
        void DeleteCard(Guid userId, String cardId);

        /// <summary>
        /// Set default payment for customer
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="cardId">Stripe card identifier.</param>
        void SetDefaultPayment(Guid userId, String cardId);
        
        /// <summary>
        /// Get bank account.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>Bank account item.</returns>
        Task<StripeBankAccountItem> GetBankAccount(Guid userId);

        /// <summary>
        /// Add bank account to managed account.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="bankAccountToken">Stripe bank acoount token.</param>
        /// <returns>Bank account item.</returns>
        Task<StripeBankAccountItem> AddBankAccount(Guid userId, String bankAccountToken);

        /// <summary>
        /// Get debit card.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>Debit card item.</returns>
        Task<StripeCardItem> GetDebitCard(Guid userId);

        /// <summary>
        /// Add debit card to managed account
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="debitCardToken">Stripe debit card token.</param>
        /// <returns>Debit card item.</returns>
        Task<StripeCardItem> AddDebitCard(Guid userId, String debitCardToken);

        /// <summary>
        /// Delete debit card
        /// </summary>
        /// <param name="userId">User identifier.</param>
        void DeleteDebitCard(Guid userId);
    }
}