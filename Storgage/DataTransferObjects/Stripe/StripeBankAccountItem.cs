using Stripe;

namespace Weezlabs.Storgage.DataTransferObjects.Stripe
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Stripe bank account.
    /// </summary>
    public class StripeBankAccountItem
    {
        /// <summary>
        /// Bank account Id.
        /// </summary>
        public String BankAccountId { get; set; }

        /// <summary>
        /// Account holder name.
        /// </summary>
        public String HolderName { get; set; }

        /// <summary>
        /// Account holder type.
        /// </summary>
        public String HolderType { get; set; }

        /// <summary>
        /// Bank name.
        /// </summary>
        public String BankName { get; set; }
        
        /// <summary>
        /// Country.
        /// </summary>
        public String Country { get; set; }

        /// <summary>
        /// Currency.
        /// </summary>
        public String Currency { get; set; }
        
        /// <summary>
        /// Last 4 symbols.
        /// </summary>
        public String Last4 { get; set; }


        /// <summary>
        /// Create instance of stripe bank account response
        /// </summary>
        /// <param name="bankAccount">Bank account model.</param>
        public StripeBankAccountItem(StripeBankAccount bankAccount)
        {
            Contract.Requires(bankAccount != null);

            this.BankAccountId = bankAccount.Id;
            this.HolderName = bankAccount.AccountHolderName ?? String.Empty;
            this.HolderType = bankAccount.AccountHolderType;
            this.BankName = bankAccount.BankName ?? String.Empty;
            this.Country = bankAccount.Country;
            this.Currency = bankAccount.Currency;
            this.Last4 = bankAccount.Last4;
        }
    }
}
