namespace Weezlabs.Storgage.ChatService
{
    using System;
    using System.Collections.Generic;   

    using DataTransferObjects.Message;   

    /// <summary>
    /// Chat manager interface.
    /// </summary>
    public interface IChatManager
    {
        /// <summary>
        /// Creates new chat with host.
        /// </summary>
        /// <param name="spaceId">Space Id.</param>
        /// <param name="request">Request.</param>
        /// <returns>Response.</returns>
        GetMessageWithChatResponse BeginChat(Guid spaceId, PostMessageRequest request);

        /// <summary>
        /// Posts new message to chat.
        /// </summary>
        /// <param name="chatId">Chat identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <param name="request">Request to post message.</param>
        /// <returns>Posted message.</returns>
        GetMessageResponse PostMessage(Guid chatId, Guid actorId, PostMessageRequest request);

        /// <summary>
        /// Returns list of user's chats.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="limit">Limit.</param>
        /// <returns>List of user's chats.</returns>
        IEnumerable<GetChatResponse> ListMyChats(Guid userId, Int32 offset = 0, Int32 limit = 0);

         /// <summary>
        /// Returns chat list where user is Renter with possible status for last offer
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="lastOfferStatus">Last Offer Status.</param>
        /// <returns>List of user's chats.</returns>
        IEnumerable<GetChatResponse> ListRenterChats(Guid userId,
            Weezlabs.Storgage.Model.Enums.MessageOfferStatus? lastOfferStatus);

        /// <summary>
        /// Retrieves chat for the specified user and space
        /// </summary>
        /// <param name="userId">user identifier</param>
        /// <param name="spaceId">space identifier</param>
        /// <returns>chat for the specified user and space</returns>
        GetChatResponse GetLastChat(Guid userId, Guid spaceId);

        /// <summary>
        /// Returns chat by chat identifier.
        /// </summary>
        /// <param name="chatId">Chat identifier.</param>
        /// <param name="userId">User identifier.</param>
        /// <returns>Chat.</returns>
        GetChatResponse GetChatById(Guid chatId, Guid userId);

        /// <summary>
        /// Returns list of messages chats.
        /// </summary>
        /// <param name="userId">Chat identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <param name="laterOrEqual">Date time offset for last messages.</param>
        /// <returns>List of messages.</returns>
        IEnumerable<GetMessageResponse> GetMessagesFromChat(Guid chatId, Guid actorId, DateTimeOffset? laterOrEqual = null);

        /// <summary>
        /// Returns message by identifier.
        /// </summary>
        /// <param name="messageId">Message identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Message.</returns>
        GetMessageResponse GetMessage(Guid messageId, Guid actorId);

        /// <summary>
        /// Updates status of current offer.
        /// </summary>
        /// <param name="chatId">Chat identifier.</param>
        /// <param name="newStatus">New status.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Offer after status updating.</returns>
        GetOfferResponse UpdateCurrentOfferStatus(Guid chatId, Model.Enums.MessageOfferStatus newStatus, Guid actorId);

        /// <summary>
        /// Stops current offer.
        /// </summary>
        /// <param name="chatId">Chat identifier.</param>        
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Offer after status updating.</returns>
        GetOfferResponse StopCurrentOffer(Guid chatId, Guid actorId);       

        /// <summary>
        /// Marks messages as readed.
        /// </summary>
        /// <param name="messageIds">Enumerable of message identifiers.</param>
        /// <param name="ActorId">Actor Identifier.</param>
        /// <returns>Messages were marked as readed.</returns>
        IEnumerable<GetMessageResponse> MarkMessagesAsRead(IEnumerable<Guid> messageIds, Guid ActorId);
    }
}
