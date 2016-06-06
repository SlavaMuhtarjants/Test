namespace Weezlabs.Storgage.RestApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Http.Description;

    using ChatService;
    using DataTransferObjects.Message;
    using Helpers;
    using Model.Exceptions;
using Weezlabs.Storgage.Model.Enums;
        

    /// <summary>
    /// Chats controller
    /// </summary>
    [RoutePrefix("api")]
    public class ChatsController : ApiController
    {
        private readonly IChatManager chatManager;
     

        /// <summary>
        /// Creates controler.
        /// </summary>
        /// <param name="chatManager">Chat manager.</param>
        public ChatsController(IChatManager chatManager)
        {
            Contract.Requires(chatManager != null);            

            this.chatManager = chatManager;            
        }

        /// <summary>
        /// Returns message by identifuer.
        /// </summary>
        /// <param name="messageId">Message identifier.</param>
        /// <returns>Message.</returns>
        /// <response code="200">Message was returned.</response>        
        /// <response code="401">User is unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="404">Message not found.</response>
        [HttpGet]
        [Route("messages/{messageId}")]
        [ResponseType(typeof(GetMessageResponse))]
        [Authorize]
        public IHttpActionResult GetMessage(Guid messageId)
        {            
            try
            {
                var actorId = RequestContext.Principal.GetUserIdFromClaim();
                GetMessageResponse result = (GetMessageResponse) chatManager.GetMessage(messageId, actorId);
                return Ok<GetMessageResponse>(result);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }
        }

        /// <summary>
        /// Begins new chat with host.
        /// </summary>
        /// <param name="spaceId">Space identifier of host.</param>
        /// <param name="request">Begin chat request.</param>
        /// <returns>Created chat.</returns>
        /// <response code="201">Chat was created.</response>
        /// <response code="400">Bad request. See description to get detailed information.</response>
        /// <response code="401">User is unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="404">Space not found.</response>
        [HttpPost]
        [Route("spaces/{spaceId}/contacthost")]
        [ResponseType(typeof(GetMessageWithChatResponse))]
        [Authorize]
        public IHttpActionResult ContactHost(Guid spaceId, [FromBody] ContactHostRequest request)
        {
            if (request == null)
            {
                return BadRequest(Resources.Messages.PostBodyCannotBeNull);
            }            

            if (!RequestContext.Principal.IsValidUser(request.SenderId))
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }

            try
            {
                var postedMessage = chatManager.BeginChat(spaceId, request);
                var location =
                   new Uri(new Uri(Request.RequestUri, RequestContext.VirtualPathRoot),
                       String.Format("api/messages/{0}", postedMessage.Id));

                return Created<GetMessageWithChatResponse>(location, postedMessage);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (BeginChatException)
            {
                return BadRequest(Resources.Messages.ChatAlwaysExist);
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Posts a new message to chat.
        /// </summary>
        /// <param name="chatId"></param>
        /// <param name="request"></param>
        /// <returns>Posted message.</returns>
        /// <response code="201">Chat was created.</response>
        /// <response code="400">Bad request. See description to get detailed information.</response>
        /// <response code="401">User is unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="404">Chat not found.</response>
        [HttpPost]
        [Route("chats/{chatId}/messages")]
        [ResponseType(typeof(GetMessageResponse))]
        [Authorize]
        public IHttpActionResult PostMessage(Guid chatId, [FromBody] PostMessageRequest request)
        {
            if (request == null)
            {
                return BadRequest(Resources.Messages.PostBodyCannotBeNull);
            }

            if (request.Message != null && request.Offer != null
               || request.Message == null && request.Offer == null)
            {
                return BadRequest(Resources.Messages.PostMessagesBadFormat);
            }

            if (!RequestContext.Principal.IsValidUser(request.SenderId))
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }

            try
            {
                var actorId = RequestContext.Principal.GetUserIdFromClaim();
                var postedMessage = chatManager.PostMessage(chatId, actorId, request);
                var location =
                   new Uri(new Uri(Request.RequestUri, RequestContext.VirtualPathRoot),
                       String.Format("api/messages/{0}", postedMessage.Id));

                return Created<GetMessageResponse>(location, postedMessage);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }
            catch (PostMessageException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Returns list of user's chats.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="limit">Limit.</param>
        /// <returns>List of user's chats.</returns>
        /// <response code="200">Ok. Data was returned.</response>        
        /// <response code="401">User is unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="404">User not found.</response>
        [HttpGet]
        [Route("users/{userId}/mychats")]
        [ResponseType(typeof(IEnumerable<GetChatResponse>))]
        [Authorize]
        public IHttpActionResult ListMyChats(Guid userId, Int32 offset = 0, Int32 limit = 0)
        {
            if(!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }

            try
            {
                var result = chatManager.ListMyChats(userId, offset, limit);
                return Ok<IEnumerable<GetChatResponse>>(result);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Returns chat by identifier.
        /// </summary>
        /// <param name="chatId">Chat identifier.</param>
        /// <returns>Chat.</returns>
        /// <response code="200">Ok. Data was returned.</response>        
        /// <response code="401">User is unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="404">User not found.</response>
        [HttpGet]
        [Route("chats/{chatId}")]
        [ResponseType(typeof(GetChatResponse))]
        [Authorize]
        public IHttpActionResult GetChat(Guid chatId)
        {
            try
            {
                var actorId = RequestContext.Principal.GetUserIdFromClaim();
                var result = chatManager.GetChatById(chatId, actorId);
                return Ok<GetChatResponse>(result);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (AccessDeniedException ex)
            {
                return this.Forbidden(ex.Message);
            }
        }

        /// <summary>
        /// Retrieves last chat for the specified user and space
        /// </summary>
        /// <param name="userId">user (tenant) identifier</param>
        /// <param name="spaceId">space identifier</param>
        /// <returns>Chat for the specified user and space</returns>
        /// <response code="200">Ok. Chat is found</response>        
        /// <response code="401">User is unauthorized.</response>
        /// <response code="403">Access is forbidden.</response>
        /// <response code="404">Chat for the space is not found.</response>
        [HttpGet]
        [Route("users/{userId}/last-chat/for/spaces/{spaceId}")]
        [ResponseType(typeof(GetChatResponse))]
        [Authorize]
        public IHttpActionResult GetLastChat(Guid userId, Guid spaceId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }

            try
            {
                var result = chatManager.GetLastChat(userId, spaceId);
                return Ok<GetChatResponse>(result);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Returns list of messages from chat.
        /// </summary>
        /// <param name="chatId">Chat identifier.</param>
        /// <param name="laterOrEqaul">Date time offset for lastt messages.</param>
        /// <returns>List of messages.</returns>
        /// <response code="200">Ok. Data was returned.</response>        
        /// <response code="401">User is unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="404">Chat not found.</response>
        [HttpGet]
        [Route("chats/{chatId}/messages")]
        [ResponseType(typeof(IEnumerable<GetMessageResponse>))]
        [Authorize]
        public IHttpActionResult GetMessagesFromChat(Guid chatId, DateTimeOffset? laterOrEqaul = null)
        {            
            try
            {
                var actorId = RequestContext.Principal.GetUserIdFromClaim();
                var result = chatManager.GetMessagesFromChat(chatId, actorId, laterOrEqaul);
                return Ok<IEnumerable<GetMessageResponse>>(result);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }
        }


        /// <summary>
        /// Marks messages as read.
        /// </summary>
        /// <param name="userId">Who read messages.</param>
        /// <param name="messageIds">Enumerable of message identifiers to mark messages as read.</param>
        /// <returns>Enumerable of messages were marked as read.</returns>
        /// <response code="200">Ok. Messages were marked as read.</response>        
        /// <response code="401">User is unauthorized.</response>
        /// <response code="403">Access forbidden.</response>        
        [HttpPut]
        [Route("messages/wasreadby/{userId}")]
        [ResponseType(typeof(IEnumerable<GetMessageResponse>))]
        [Authorize]
        public IHttpActionResult MarkMessagesAsRead(Guid userId, [FromBody] IEnumerable<Guid> messageIds)
        {
            if (!messageIds.Any())
            {
                return Ok<IEnumerable<GetMessageResponse>>(new GetMessageResponse[0]);
            }

            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }

            var result = chatManager.MarkMessagesAsRead(messageIds, userId);
            return Ok<IEnumerable<GetMessageResponse>>(result);
        }

        /// <summary>
        /// Approves offer.
        /// </summary>
        /// <param name="chatId">Chat identifier to approve current offer.</param>    
        /// <param name="userId">User who approved offer.</param>    
        /// <returns>Approved offer.</returns>
        /// <response code="200">Ok. Offer was approved.</response>        
        /// <response code="401">User is unauthorized.</response>
        /// <response code="403">Access forbidden.</response>        
        /// <response code="404">Offer not found.</response>        
        [HttpPut]
        [Route("users/{userId}/chats/{chatId}/approve/current-offer")]
        [ResponseType(typeof(GetOfferResponse))]
        [Authorize]
        public IHttpActionResult ApproveOffer(Guid chatId, Guid userId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }
            try
            {             
                var result = chatManager.UpdateCurrentOfferStatus(chatId, Model.Enums.MessageOfferStatus.Approved, userId);
                return Ok<GetOfferResponse>(result);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }
            catch (InvalidOfferStatusException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Rejects offer.
        /// </summary>
        /// <param name="chatId">Chat identifier to reject current offer.</param> 
        /// <param name="userId">User who rejected offer.</param>         
        /// <returns>Rejected offer.</returns>
        /// <response code="200">Ok. Offer was rejected.</response>        
        /// <response code="401">User is unauthorized.</response>
        /// <response code="403">Access forbidden.</response>        
        /// <response code="404">Offer not found.</response>        
        [HttpPut]
        [Route("users/{userId}/chats/{chatId}/reject/current-offer")]
        [ResponseType(typeof(GetOfferResponse))]
        [Authorize]
        public IHttpActionResult RejectOffer(Guid chatId, Guid userId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }

            try
            {            
                var result = chatManager.UpdateCurrentOfferStatus(chatId, Model.Enums.MessageOfferStatus.Rejected, userId);
                return Ok<GetOfferResponse>(result);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }
            catch (InvalidOfferStatusException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Stops offer.
        /// </summary>
        /// <param name="chatId">Chat identifier to stop current offer.</param> 
        /// <param name="userId">User who stopped offer.</param>
        /// <returns>Rejected offer.</returns>
        /// <response code="200">Ok. Offer was stopped.</response>        
        /// <response code="401">User is unauthorized.</response>
        /// <response code="403">Access forbidden.</response>        
        /// <response code="404">Offer not found.</response>        
        [HttpPut]
        [Route("users/{userId}/chats/{chatId}/stop/current-offer")]
        [ResponseType(typeof(GetOfferResponse))]
        [Authorize]
        public IHttpActionResult StopOffer(Guid chatId, Guid userId)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }

            try
            {
                var result = chatManager.StopCurrentOffer(chatId, userId);
                return Ok<GetOfferResponse>(result);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
            catch (AccessDeniedException)
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }
            catch (InvalidOfferStatusException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Returns list of chats that user started as renter with possible filter by last offer status.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <returns>List of renters's chats.</returns>
        /// <response code="200">Ok. Data was returned.</response>        
        /// <response code="401">User is unauthorized.</response>
        /// <response code="403">Access forbidden.</response>
        /// <response code="404">User not found.</response>
        [HttpGet]
        [Route("users/{userId}/mychats/astenant")]
        [ResponseType(typeof(IEnumerable<GetChatResponse>))]
        [Authorize]
        public IHttpActionResult ListRenterChats(Guid userId, MessageOfferStatus? lastOfferStatus)
        {
            if (!RequestContext.Principal.IsValidUser(userId))
            {
                return this.Forbidden(Resources.Messages.AccessDenied);
            }

            try
            {
                var result = chatManager.ListRenterChats(userId, lastOfferStatus);
                return Ok<IEnumerable<GetChatResponse>>(result);
            }
            catch (NotFoundException ex)
            {
                return this.NotFound(ex.Message);
            }
        }
       
    }
}
