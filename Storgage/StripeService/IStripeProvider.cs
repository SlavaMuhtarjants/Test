namespace Weezlabs.Storgage.StripeService
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using DataTransferObjects.Stripe;

    /// <summary>
    /// Interface for stripe provider.
    /// </summary>
    public interface IStripeProvider
    {
        /// <summary>
        /// Get list of cards for customer
        /// </summary>
        /// <param name="customerId">Customer identifier.</param>
        /// <returns>List of stripe card item</returns>
        Task<List<StripeCardItem>> GetCardsList(String customerId);

        /// <summary>
        /// Get card by identifier.
        /// </summary>
        /// <param name="customerInfo">Customer stripe info.</param>
        /// <returns>Card item.</returns>
        Task<StripeCardItem> GetCard(StripeCustomerInfo customerInfo);

        /// <summary>
        /// Add new card to customer.
        /// </summary>
        /// <param name="customerInfo">Customer stripe info.</param>
        /// <returns>Updated customer stripe info.</returns>
        Task<StripeCustomerInfo> AddCard(StripeCustomerInfo customerInfo);

        /// <summary>
        /// Delete customer's card
        /// </summary>
        /// <param name="customerInfo">Customer stripe info.</param>
        void DeleteCard(StripeCustomerInfo customerInfo);

        /// <summary>
        /// Set default payment for customer by card_id.
        /// </summary>
        /// <param name="customerInfo">Customer info.</param>
        /// <param name="woCheckingCustomer">True if we shouldn't check customer on stripe side.</param>
        void SetDefaultPayment(StripeCustomerInfo customerInfo, Boolean woCheckingCustomer = false);

        /// <summary>
        /// Get bank account.
        /// </summary>
        /// <param name="customerInfo">Customer info.</param>
        /// <param name="сheckAccount">True if we should check account existing (for add bank acc method we shouldn't)</param>
        /// <returns>Bank account item.</returns>
        Task<StripeBankAccountItem> GetBankAccount(StripeCustomerInfo customerInfo, Boolean сheckAccount = true);

        /// <summary>
        /// Add bank account to customer.
        /// </summary>
        /// <param name="customerInfo">Customer stripe info.</param>
        /// <returns>Updated customer stripe info.</returns>
        Task<StripeCustomerInfo> AddBankAccount(StripeCustomerInfo customerInfo);

        /// <summary>
        /// Get debit card.
        /// </summary>
        /// <param name="customerInfo">Customer info.</param>
        /// <param name="сheckAccount">True if we should check account existing (for add bank acc method we shouldn't)</param>
        /// <returns>Debit card item.</returns>
        Task<StripeCardItem> GetDebitCard(StripeCustomerInfo customerInfo, Boolean сheckAccount = true);

        /// <summary>
        /// Add debit card to customer.
        /// </summary>
        /// <param name="customerInfo">Customer stripe info.</param>
        /// <returns>Updated customer stripe info.</returns>
        Task<StripeCustomerInfo> AddDebitCard(StripeCustomerInfo customerInfo);

        /// <summary>
        /// Delete customer's debit card.
        /// </summary>
        /// <param name="customerInfo">Customer stripe info.</param>
        void DeleteDebitCard(StripeCustomerInfo customerInfo);
    }
}