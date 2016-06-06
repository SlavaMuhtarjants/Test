namespace Weezlabs.Storgage.StripeService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using DataTransferObjects.Stripe;
    using Model.Exceptions;
    using Resources;
    using UtilService;

    using Newtonsoft.Json;
    using RestSharp;
    using Stripe;

    /// <summary>
    /// Stripe provider
    /// </summary>
    public class StripeProvider : IStripeProvider
    {
        private readonly IAppSettings appSettings;

        /// <summary>
        /// Create instanse of stripe provider
        /// </summary>
        /// <param name="appSettings">Application settings.</param>
        public StripeProvider(IAppSettings appSettings)
        {
            Contract.Requires(appSettings != null);

            this.appSettings = appSettings;
        }

        private readonly StripeServiceWrapper stripeService = new StripeServiceWrapper();

        /// <summary>
        /// Get list of cards for customer
        /// </summary>
        /// <param name="customerId">Customer identifier.</param>
        /// <returns>List of stripe card item</returns>
        public async Task<List<StripeCardItem>> GetCardsList(String customerId)
        {
            if (String.IsNullOrWhiteSpace(customerId))
            {
                // we have no cards for not exists customer
                return new List<StripeCardItem>();
            }

            StripeCustomer customer = await stripeService.StripeCustomerService.GetAsync(customerId);
            if (customer.Created == default(DateTime))
            {
                return new List<StripeCardItem>();
            }

            var cardsList =
                customer.SourceList.Data.Where(x => x.Object == "card").Select(x => new StripeCardItem(x)).ToList();
            if (cardsList.Any())
            {
                cardsList.First(x => x.CardId == customer.DefaultSourceId).Default = true;
            }

            return cardsList;
        }


        /// <summary>
        /// Get card by identifier.
        /// </summary>
        /// <param name="customerInfo">Customer stripe info.</param>
        /// <returns>Card item.</returns>
        public async Task<StripeCardItem> GetCard(StripeCustomerInfo customerInfo)
        {
            Contract.Requires(customerInfo != null);

            customerInfo = GetOrCreateCustomer(customerInfo, false); // get customer or throw if not exists

            StripeCard card =
                await
                    stripeService.StripeCardService.GetAsync(customerInfo.CustomerId, customerInfo.CustomerCard.CardId);

            return new StripeCardItem(card);
        }

        /// <summary>
        /// Add new card to customer.
        /// </summary>
        /// <param name="customerInfo">Customer stripe info.</param>
        /// <returns>Updated customer stripe info.</returns>
        public async Task<StripeCustomerInfo> AddCard(StripeCustomerInfo customerInfo)
        {
            Contract.Requires(customerInfo != null);

            customerInfo = GetOrCreateCustomer(customerInfo);// get or create customer

            // for new customer we add new card and set as default automatically
            // for exists customer we add new card and set default if need
            if (!customerInfo.IsNew)
            {
                var newCard = new StripeCardCreateOptions { SourceToken = customerInfo.CustomerCard.CardToken };
                StripeCard stripeCard =
                    await stripeService.StripeCardService.CreateAsync(customerInfo.CustomerId, newCard);
                customerInfo.CustomerCard.CardId = stripeCard.Id;
                customerInfo.CustomerCard.Card = new StripeCardItem(stripeCard)
                {
                    Default = customerInfo.CustomerCard.SetDefault
                };

                if (customerInfo.CustomerCard.SetDefault)
                {
                    SetDefaultPayment(customerInfo, true);
                }
            }

            return customerInfo;
        }

        /// <summary>
        /// Delete customer's card
        /// </summary>
        /// <param name="customerInfo">Customer stripe info.</param>
        public void DeleteCard(StripeCustomerInfo customerInfo)
        {
            Contract.Requires(customerInfo != null);

            customerInfo = GetOrCreateCustomer(customerInfo, false); // get customer or throw if not exists

            stripeService.StripeCardService.Delete(customerInfo.CustomerId,
                customerInfo.CustomerCard.CardId);
        }

        /// <summary>
        /// Set default payment for customer by card_id.
        /// </summary>
        /// <param name="customerInfo">Customer info.</param>
        /// <param name="woCheckingCustomer">True if we shouldn't check customer on stripe side.</param>
        public void SetDefaultPayment(StripeCustomerInfo customerInfo, Boolean woCheckingCustomer = false)
        {
            Contract.Requires(customerInfo != null);

            // note: remove redundant call api when set default after added card.
            if (!woCheckingCustomer)
            {
                customerInfo = GetOrCreateCustomer(customerInfo, false); // get customer or throw if not exists
            }

            var updateCustomerOptions = new StripeCustomerUpdateOptions
            {
                DefaultSource = customerInfo.CustomerCard.CardId
            };
            
            stripeService.StripeCustomerService.Update(customerInfo.CustomerId, updateCustomerOptions);
        }

        /// <summary>
        /// Get bank account.
        /// </summary>
        /// <param name="customerInfo">Customer info.</param>
        /// <param name="сheckAccount">True if we should check account existing (for add bank acc method we shouldn't)</param>
        /// <returns>Bank account item.</returns>
        public async Task<StripeBankAccountItem> GetBankAccount(StripeCustomerInfo customerInfo, Boolean сheckAccount = true)
        {
            if (сheckAccount)
            {
                customerInfo = GetOrCreateAccount(customerInfo, false); // get or throw if not exists
            }

            var client = new RestClient(appSettings.GetSetting<String>("StripeApiEndpoint"));
            var request = new RestRequest("accounts/{account}/external_accounts?object=bank_account&limit=1", Method.GET);
            request.AddAutorizationHeader();
            request.AddUrlSegment("account", customerInfo.AccountId);

            IRestResponse response = client.Execute(request);
            StripeBankAccountItem bankAccount =
                JsonConvert.DeserializeObject<StripeBankAccountsListResponse>(response.Content)
                    .Data.Select(x => new StripeBankAccountItem(x))
                    .FirstOrDefault();

            if (bankAccount == null)
            {
                throw new NotFoundException(Messages.BankAccountNotFound);
            }

            return bankAccount;
        }

        /// <summary>
        /// Add bank account to customer.
        /// </summary>
        /// <param name="customerInfo">Customer stripe info.</param>
        /// <returns>Updated customer stripe info.</returns>
        public async Task<StripeCustomerInfo> AddBankAccount(StripeCustomerInfo customerInfo)
        {
            customerInfo = GetOrCreateAccount(customerInfo); // get or create account

            // check bank accounts if customer exists earlier
            if (!customerInfo.IsNew)
            {
                StripeBankAccountItem oldBankAccount = null;
                try
                {
                    // check for exists error
                    // call without checking account existing (we does it earlier)
                    oldBankAccount = await GetBankAccount(customerInfo, false);
                }
                catch (NotFoundException)
                {
                    // status Ok. We have no bank account yet.
                }
                
                // create new
                var client = new RestClient(appSettings.GetSetting<String>("StripeApiEndpoint"));
                var request = new RestRequest("accounts/{account}/external_accounts", Method.POST);
                request.AddAutorizationHeader();
                request.AddUrlSegment("account", customerInfo.AccountId);
                request.AddParameter("external_account", customerInfo.CustomerBankAccount.StripeBankAccountToken);
                request.AddParameter("default_for_currency", true);

                IRestResponse response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(
                        JsonConvert.DeserializeObject<StripeRestSharpException>(response.Content).Error.Message);
                }

                var data = JsonConvert.DeserializeObject<DataTransferObjects.Stripe.StripeBankAccount>(response.Content);

                customerInfo.CustomerBankAccount.StripeBankAccountId = data.Id;
                customerInfo.CustomerBankAccount.StripeBankAccount = new StripeBankAccountItem(data);

                // delete previous (we can't delete before creating new default for currency - restriction from stripe)
                if (oldBankAccount != null)
                {
                    customerInfo.CustomerBankAccount.StripeBankAccountId = oldBankAccount.BankAccountId;
                    DeleteBankAccount(customerInfo);
                }
            }

            return customerInfo;
        }

        /// <summary>
        /// Delete customer's bank account.
        /// </summary>
        /// <param name="customerInfo">Customer stripe info.</param>
        private void DeleteBankAccount(StripeCustomerInfo customerInfo)
        {
            Contract.Requires(customerInfo != null);
            Contract.Requires(!String.IsNullOrWhiteSpace(customerInfo.CustomerBankAccount.StripeBankAccountId));

            var client = new RestClient(appSettings.GetSetting<String>("StripeApiEndpoint"));
            var request = new RestRequest("accounts/{account}/external_accounts/{bank}", Method.DELETE);
            request.AddAutorizationHeader();
            request.AddUrlSegment("account", customerInfo.AccountId);
            request.AddUrlSegment("bank", customerInfo.CustomerBankAccount.StripeBankAccountId);

            client.Execute(request);
        }

        /// <summary>
        /// Get debit card.
        /// </summary>
        /// <param name="customerInfo">Customer info.</param>
        /// <param name="сheckAccount">True if we should check account existing (for add bank acc method we shouldn't)</param>
        /// <returns>Debit card item.</returns>
        public async Task<StripeCardItem> GetDebitCard(StripeCustomerInfo customerInfo, Boolean сheckAccount = true)
        {
            if (сheckAccount)
            {
                customerInfo = GetOrCreateAccount(customerInfo, false); // get or throw if not exists
            }

            StripeCard debitCard = customerInfo.Account.ExternalCards.FirstOrDefault();

            if (debitCard == null)
            {
                throw new NotFoundException(Messages.DebitCardNotFound);
            }

            return new StripeCardItem(debitCard);
        }

        /// <summary>
        /// Add debit card to customer.
        /// </summary>
        /// <param name="customerInfo">Customer stripe info.</param>
        /// <returns>Updated customer stripe info.</returns>
        public async Task<StripeCustomerInfo> AddDebitCard(StripeCustomerInfo customerInfo)
        {
            customerInfo = GetOrCreateAccount(customerInfo); // get or create account

            // check debit cards if customer exists earlier
            if (!customerInfo.IsNew)
            {
                StripeCardItem oldDebitCard = null;
                try
                {
                    // check for exists error
                    // call without checking account existing (we does it earlier)
                    oldDebitCard = await GetDebitCard(customerInfo, false);
                }
                catch (NotFoundException)
                {
                    // status Ok. We have no bank account yet.
                }

                // create new
                var client = new RestClient(appSettings.GetSetting<String>("StripeApiEndpoint"));
                var request = new RestRequest("accounts/{account}/external_accounts", Method.POST);
                request.AddAutorizationHeader();
                request.AddUrlSegment("account", customerInfo.AccountId);
                request.AddParameter("external_account", customerInfo.CustomerCard.CardToken);

                // bank account is primary
                Stripe.StripeBankAccount bankAccount = customerInfo.Account.ExternalBankAccounts.FirstOrDefault();
                if (bankAccount == null)
                {
                    // set default only if we haven't bank account
                    customerInfo.CustomerCard.SetDefault = true;
                    request.AddParameter("default_for_currency", true);
                }

                IRestResponse response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception(
                        JsonConvert.DeserializeObject<StripeRestSharpException>(response.Content).Error.Message);
                }

                var data = JsonConvert.DeserializeObject<StripeCard>(response.Content);

                customerInfo.CustomerCard.CardId = data.Id;
                customerInfo.CustomerCard.Card = new StripeCardItem(data)
                {
                    Default = customerInfo.CustomerCard.SetDefault
                };

                // delete previous (we can't delete before creating new default for currency - restriction from stripe)
                if (oldDebitCard != null)
                {
                    customerInfo.CustomerCard.CardId = oldDebitCard.CardId;
                    DeleteDebitCard(customerInfo);
                }
            }

            return customerInfo;
        }

        /// <summary>
        /// Delete customer's debit card.
        /// </summary>
        /// <param name="customerInfo">Customer stripe info.</param>
        public void DeleteDebitCard(StripeCustomerInfo customerInfo)
        {
            Contract.Requires(customerInfo != null);

            String cardId = customerInfo.CustomerCard.CardId;
            if (String.IsNullOrWhiteSpace(cardId))
            {
                customerInfo = GetOrCreateAccount(customerInfo);
                StripeCard card = customerInfo.Account.ExternalCards.FirstOrDefault();
                if (card == null)
                {
                    throw new NotFoundException(Resources.Messages.DebitCardNotFound);
                }
                cardId = card.Id;
            }

            var client = new RestClient(appSettings.GetSetting<String>("StripeApiEndpoint"));
            var request = new RestRequest("accounts/{account}/external_accounts/{card}", Method.DELETE);
            request.AddAutorizationHeader();
            request.AddUrlSegment("account", customerInfo.AccountId);
            request.AddUrlSegment("card", cardId);

            IRestResponse response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(
                    JsonConvert.DeserializeObject<StripeRestSharpException>(response.Content).Error.Message);
            }
        }

        /// <summary>
        /// Get account by customer info.
        /// </summary>
        /// <param name="customerInfo">Stripe customer info.</param>
        /// <param name="create">Create new account if account not exists.</param>
        /// <returns>StripeCustomerInfo</returns>
        private StripeCustomerInfo GetOrCreateAccount(StripeCustomerInfo customerInfo, Boolean create = true)
        {
            Contract.Requires(customerInfo != null);

            // get exists
            if (!String.IsNullOrWhiteSpace(customerInfo.AccountId))
            {
                try
                {
                    // try to get
                    customerInfo.Account = stripeService.StripeAccountService.Get(customerInfo.AccountId);
                    // NOTE! not forgot for charges! fields_needed
                    //if (!String.IsNullOrWhiteSpace(customerInfo.Account.AccountVerification.DisabledReason))
                    //{
                    //    throw new Exception(String.Format(Resources.Messages.StripeDisabledAccount,
                    //        customerInfo.Account.AccountVerification.DisabledReason));
                    //}
                    return customerInfo;
                }
                catch (StripeException)
                {   
                    // we should create new account. We will do it below.
                }
            }

            if (!create)
            {
                throw new NotFoundException();
            }

            // create new
            var newAccount = new StripeAccountCreateOptions
            {
                Email = customerInfo.Email,
                Managed = true,
                Country = "US",
                Metadata = new Dictionary<String, String> {{"userId", customerInfo.UserId.ToString()}}
            };
            // note: in our case we can create just one external account at the moment
            // also create bank account if we have token
            if (!String.IsNullOrWhiteSpace(customerInfo.CustomerBankAccount.StripeBankAccountToken))
            {
                newAccount.ExternalBankAccount = new StripeAccountBankAccountOptions
                {
                    TokenId = customerInfo.CustomerBankAccount.StripeBankAccountToken
                };
            }
            // also create debit card if we have token
            if (!String.IsNullOrWhiteSpace(customerInfo.CustomerCard.CardToken))
            {
                newAccount.ExternalBankAccount = new StripeAccountBankAccountOptions
                {
                    TokenId = customerInfo.CustomerCard.CardToken
                };
            }

            StripeAccount stripeAccount = stripeService.StripeAccountService.Create(newAccount);

            customerInfo.Account = stripeAccount;
            customerInfo.AccountId = stripeAccount.Id;
            // we can set bank account object if we created it
            if (!String.IsNullOrWhiteSpace(customerInfo.CustomerBankAccount.StripeBankAccountToken))
            {
                var stripeBankAccount = GetBankAccount(customerInfo, false).Result;
                customerInfo.CustomerBankAccount.StripeBankAccountId = stripeBankAccount.BankAccountId;
                customerInfo.CustomerBankAccount.StripeBankAccount = stripeBankAccount;
            }
            // we can set bank account object if we created it
            if (!String.IsNullOrWhiteSpace(customerInfo.CustomerCard.CardToken))
            {
                StripeCard stripeDebitCard = stripeAccount.ExternalCards.First();
                customerInfo.CustomerCard.CardId = stripeDebitCard.Id;
                customerInfo.CustomerCard.Card = new StripeCardItem(stripeDebitCard) {Default = true};
            }

            return customerInfo;
        }

        /// <summary>
        /// Get customer by customer info.
        /// </summary>
        /// <param name="customerInfo">Stripe customer info.</param>
        /// <param name="create">Create new customer if customer not exists.</param>
        /// <returns>StripeCustomerInfo</returns>
        private StripeCustomerInfo GetOrCreateCustomer(StripeCustomerInfo customerInfo, Boolean create = true)
        {
            Contract.Requires(customerInfo != null);

            // get exists
            if (!String.IsNullOrWhiteSpace(customerInfo.CustomerId))
            {
                StripeCustomer customer = stripeService.StripeCustomerService.Get(customerInfo.CustomerId);
                if (customer.Created != default(DateTime))
                {
                    return customerInfo;
                }
            }

            if (!create)
            {
                throw new NotFoundException();
            }

            // create new
            var newCustomer = new StripeCustomerCreateOptions
            {
                Email = customerInfo.Email,
                Description = customerInfo.CustomerDescription
            };
            // also create card if we have token
            if (!String.IsNullOrWhiteSpace(customerInfo.CustomerCard.CardToken))
            {
                newCustomer.SourceToken = customerInfo.CustomerCard.CardToken;
            }

            StripeCustomer stripeCustomer = stripeService.StripeCustomerService.Create(newCustomer);

            customerInfo.CustomerId = stripeCustomer.Id;
            // we can set Card object if we created it
            if (!String.IsNullOrWhiteSpace(customerInfo.CustomerCard.CardToken))
            {
                customerInfo.CustomerCard.Card = new StripeCardItem(stripeCustomer.SourceList.Data.First(x => x.Object == "card")) { Default = true };
            }

            return customerInfo;
        }

        /// <summary>
        /// Update customer by customer info.
        /// </summary>
        /// <param name="customerInfo">Stripe customer info</param>
        /// <returns>StripeCustomerInfo</returns>
        [Obsolete]
        public async void UpdateCustomer(StripeCustomerInfo customerInfo)
        {
            Contract.Requires(customerInfo != null);

            customerInfo = GetOrCreateCustomer(customerInfo); // get or create customer

            var updateCustomerOptions = new StripeCustomerUpdateOptions
            {
                Email = customerInfo.Email,
                Description = customerInfo.CustomerDescription
            };

            await stripeService.StripeCustomerService.UpdateAsync(customerInfo.CustomerId, updateCustomerOptions);
        }
    }
}
