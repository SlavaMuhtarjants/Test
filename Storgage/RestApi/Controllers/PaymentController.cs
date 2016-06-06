using System.Net;
using Stripe;

namespace Weezlabs.Storgage.RestApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Description;

    using DataTransferObjects.Stripe;
    using Model.Exceptions;
    using PaymentService;
    using Resources;
    using Helpers;

    /// <summary>
    /// Provides ability to create payments methods for user. 
    /// </summary>
    [RoutePrefix("api/payment")]
    public class PaymentController : ApiController
    {
        private readonly IPaymentProvider paymentProvider;

        /// <summary>
        /// Create instance of payment controller
        /// </summary>
        /// <param name="paymentProvider"></param>
        public PaymentController(IPaymentProvider paymentProvider)
        {
            Contract.Requires(paymentProvider != null);

            this.paymentProvider = paymentProvider;
        }

        #region Cards

        /// <summary>
        /// Get cards list for user
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>List of stripe card items.</returns>
        /// <response code="200">Ok.</response>      
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>       
        /// <response code="404">Not found. User or cards not found.</response>       
        [Route("{userId}/cards")]
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(List<StripeCardItem>))]
        public async Task<IHttpActionResult> GetCardsList(Guid userId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            List<StripeCardItem> cardsList;
            try
            {
                cardsList = await paymentProvider.GetCardsList(userId);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }

            return Ok<List<StripeCardItem>>(cardsList);
        }

        /// <summary>
        /// Get card by identifier.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="cardId">Card identifier.</param>
        /// <returns>Card item.</returns>
        /// <response code="200">Ok.</response>      
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>       
        /// <response code="404">Not found. User or card not found.</response>       
        [Route("{userId}/cards/{cardId}")]
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(StripeCardItem))]
        public async Task<IHttpActionResult> GetCard(Guid userId, String cardId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            StripeCardItem card;
            try
            {
                card = await paymentProvider.GetCard(userId, cardId);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                var exception = ex as StripeException;
                if (exception != null && exception.HttpStatusCode == HttpStatusCode.NotFound)
                {
                    return this.NotFound(String.Format(Messages.CardNotFound, cardId));
                }

                return this.BadRequest(ex.Message);
            }

            return Ok<StripeCardItem>(card);
        }

        /// <summary>
        /// Add card for user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="cardResponse">Stripe card object.</param>
        /// <returns>New card identifier from stripe.</returns>
        /// <response code="201">Card was created.</response>      
        /// <response code="400">Bad parameters. See description to get extra information. Used for stripe exceptions.</response>    
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>       
        /// <response code="404">Not found. User to create card was not found.</response>       
        [Route("{userId}/cards")]
        [Authorize]
        [HttpPost]
        [ResponseType(typeof(StripeCardItem))]
        public async Task<IHttpActionResult> AddCard(Guid userId, [FromBody] CardRequest cardResponse)
        {
            if (cardResponse == null)
            {
                return BadRequest(Messages.PostBodyCannotBeNull);
            }

            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            StripeCardItem card;
            try
            {
                card = await paymentProvider.AddCard(userId, cardResponse.CardToken, cardResponse.SetDefault);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }

            var location = new Uri(
                new Uri(Request.RequestUri, RequestContext.VirtualPathRoot),
                String.Format("api/payment/{0}/cards/{1}", userId, card.CardId));

            return Created<StripeCardItem>(location, card);
        }

        /// <summary>
        /// Set default payment for user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="cardId">Card identifier from stripe.</param>
        /// <returns>IHttpActionResult</returns>
        /// <response code="200">Ok.</response>      
        /// <response code="400">Bad parameters. See description to get extra information. Used for stripe exceptions.</response>    
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>       
        /// <response code="404">Not found. User to update was not found.</response>     
        [Route("{userId}/cards/{cardId}/setdefault")]
        [Authorize]
        [HttpPost]
        public IHttpActionResult SetDefaultPayment(Guid userId, String cardId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            try
            {
                paymentProvider.SetDefaultPayment(userId, cardId);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Delete user's card
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="cardId">Card identifier from stripe.</param>
        /// <returns>IHttpActionResult</returns>
        /// <response code="200">Ok.</response>      
        /// <response code="400">Bad parameters. See description to get extra information. Used for stripe exceptions.</response>    
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>       
        /// <response code="404">Not found. User or card not found.</response>     
        [Route("{userId}/cards/{cardId}")]
        [Authorize]
        [HttpDelete]
        public IHttpActionResult DeleteCard(Guid userId, String cardId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            try
            {
                paymentProvider.DeleteCard(userId, cardId);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }

            return Ok();
        }

        #endregion Cards

        #region BankAccounts

        /// <summary>
        /// Get debit card
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>Bank account item.</returns>
        [Route("{userId}/bank")]
        [Authorize]
        [HttpGet]
        [ResponseType(typeof (StripeBankAccountItem))]
        public async Task<IHttpActionResult> GetBankAccount(Guid userId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            StripeBankAccountItem bankAccount;
            try
            {
                bankAccount = await paymentProvider.GetBankAccount(userId);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }

            return Ok<StripeBankAccountItem>(bankAccount);
        }
        
        /// <summary>
        /// Add or update bank account for user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="bankAccountRequest">Stripe bank account object.</param>
        /// <returns>Bank account item.</returns>
        /// <response code="201">Bank account was created.</response>      
        /// <response code="400">Bad parameters. See description to get extra information. Used for stripe exceptions.</response>    
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>       
        /// <response code="404">Not found. User to create bank account was not found.</response>       
        [Route("{userId}/bank")]
        [Authorize]
        [HttpPut]
        [ResponseType(typeof(StripeBankAccountItem))]
        public async Task<IHttpActionResult> AddBankAccount(Guid userId, [FromBody] ManagedAccountPaymentRequest bankAccountRequest)
        {
            if (bankAccountRequest == null)
            {
                return BadRequest(Messages.PostBodyCannotBeNull);
            }

            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            StripeBankAccountItem bankAccount;
            try
            {
                bankAccount = await paymentProvider.AddBankAccount(userId, bankAccountRequest.Token);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }

            var location = new Uri(
                new Uri(Request.RequestUri, RequestContext.VirtualPathRoot),
                String.Format("api/payment/{0}/bank", userId));

            return Created<StripeBankAccountItem>(location, bankAccount);
        }

        #endregion BankAccounts

        #region Debit cards for managed account

        /// <summary>
        /// Get debit card.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>Debit card item.</returns>
        [Route("{userId}/cards/debit")]
        [Authorize]
        [HttpGet]
        [ResponseType(typeof(StripeCardItem))]
        public async Task<IHttpActionResult> GetDebitCard(Guid userId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            StripeCardItem debitCard;
            try
            {
                debitCard = await paymentProvider.GetDebitCard(userId);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }

            return Ok<StripeCardItem>(debitCard);
        }

        /// <summary>
        /// Add or update debit card for user.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="debitCardRequest">Stripe debit card object.</param>
        /// <returns>Bank account item.</returns>
        /// <response code="201">Bank account was created.</response>      
        /// <response code="400">Bad parameters. See description to get extra information. Used for stripe exceptions.</response>    
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>       
        /// <response code="404">Not found. User to create debit card was not found.</response>       
        [Route("{userId}/cards/debit")]
        [Authorize]
        [HttpPut]
        [ResponseType(typeof(StripeCardItem))]
        public async Task<IHttpActionResult> AddDebitCard(Guid userId, [FromBody] ManagedAccountPaymentRequest debitCardRequest)
        {
            if (debitCardRequest == null)
            {
                return BadRequest(Messages.PostBodyCannotBeNull);
            }

            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            StripeCardItem debitCard;
            try
            {
                debitCard = await paymentProvider.AddDebitCard(userId, debitCardRequest.Token);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }

            var location = new Uri(
                new Uri(Request.RequestUri, RequestContext.VirtualPathRoot),
                String.Format("api/payment/{0}/cards/debit", userId));

            return Created<StripeCardItem>(location, debitCard);
        }

        /// <summary>
        /// Delete user's debit card
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>IHttpActionResult</returns>
        /// <response code="200">Ok.</response>      
        /// <response code="400">Bad parameters. See description to get extra information. Used for stripe exceptions.</response>    
        /// <response code="401">Unauthorized.</response>
        /// <response code="403">Access forbidden.</response>       
        /// <response code="404">Not found. User or card not found.</response>     
        [Route("{userId}/cards/debit")]
        [Authorize]
        [HttpDelete]
        public IHttpActionResult DeleteDebitCard(Guid userId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Messages.AccessDenied);
            }

            try
            {
                paymentProvider.DeleteDebitCard(userId);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }

            return Ok();
        }

        #endregion Debit cards for managed account

    }
}
