using Stripe;

namespace Weezlabs.Storgage.DataTransferObjects.Stripe
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Information about stripe customer
    /// </summary>
    public class StripeCustomerInfo
    {
        /// <summary>
        /// True if new customer
        /// </summary>
        public Boolean IsNew { get; internal set; }

        /// <summary>
        /// Stripe account
        /// </summary>
        public StripeAccount Account { get; set; }

        private String accountId { get; set; }
        /// <summary>
        /// Account identifier
        /// </summary>
        public String AccountId
        {
            get { return accountId; }
            set
            {
                if (accountId != value)
                {
                    accountId = value;
                }
                this.IsNew = true;
            }
        }


        /// <summary>
        /// User identifier
        /// </summary>
        public Guid UserId { get; set; }

        private String customerId { get; set; }
        /// <summary>
        /// Customer identifier
        /// </summary>
        public String CustomerId
        {
            get { return customerId; }
            set
            {
                if (customerId != value)
                {
                    customerId = value;
                }
                this.IsNew = true;
            }
        }

        /// <summary>
        /// Customer's email
        /// </summary>
        public String Email { get; set; }
        
        /// <summary>
        /// Customer description for stripe
        /// </summary>
        public String CustomerDescription { get { return String.Format("storgage user id: '{0}'", this.UserId); } }

        /// <summary>
        /// Customer's source
        /// </summary>
        public StripeCustomerSource CustomerCard { get; set; }

        /// <summary>
        /// Customer's bank account
        /// </summary>
        public StripeBankAccountInfo CustomerBankAccount { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public StripeCustomerInfo()
        {
            this.IsNew = false;
        }

        /// <summary>
        /// Create instance for concreate customer
        /// </summary>
        /// <param name="user"></param>
        public StripeCustomerInfo(Model.User user) : this()
        {
            Contract.Requires(user != null);

            this.UserId = user.Id;
            this.Email = user.Email;
            this.CustomerCard = new StripeCustomerSource();
            this.CustomerBankAccount = new StripeBankAccountInfo();

            if (!String.IsNullOrWhiteSpace(user.StripeCustomerId))
            {
                this.customerId = user.StripeCustomerId;
            }
            
            if (!String.IsNullOrWhiteSpace(user.StripeAccountId))
            {
                this.accountId = user.StripeAccountId;
            }
        }
    }
}
