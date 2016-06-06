namespace Weezlabs.Storgage.ChatService
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using DataLayer;
    using DataLayer.ChatAndMessages;
    using DataLayer.Dictionaries;
    using DataLayer.Spaces;
    using DataLayer.Users;
    using DataTransferObjects.Message;
    using Model;
    using Model.Exceptions;
    using Model.ModelExtension;
    using UserNotifier;
    using UserNotifier.Notifications;

    using Castle.Core.Logging;
    using EntityFramework.Extensions;
    using DataTransferObjects.User;


    /// <summary>
    /// Chat manager implementation.
    /// </summary>
    public class ChatManager : IChatManager
    {
        private readonly ISpaceReadonlyRepository spaceRepository;
        private readonly IChatRepository chatRepository;
        private readonly IUserReadonlyRepository userRepository;
        private readonly IMessageRepository messageRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IOfferRepository offerRepository;
        private readonly IOfferHistoryRepository offerHistoryRepository;
        private readonly IChatMemberRepository chatMemberRepository;
        private readonly IUserNotifier userNotifier;
        private readonly IDictionaryProvider dictionaryProvider;

        private static readonly Guid pendingStatusId = Model.Enums.MessageOfferStatus.Pending.GetDictionaryId();
        private static readonly Guid approvedStatusId = Model.Enums.MessageOfferStatus.Approved.GetDictionaryId();
        private static readonly Guid stoppedStatusId = Model.Enums.MessageOfferStatus.Stopped.GetDictionaryId();
        private static readonly Guid rejectedStatusId = Model.Enums.MessageOfferStatus.Rejected.GetDictionaryId();
        private static readonly Guid expiredStatusId = Model.Enums.MessageOfferStatus.Expired.GetDictionaryId();

        /// <summary>
        /// Logger.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
            set { logger = value; }
        }
        private ILogger logger = NullLogger.Instance;

        public ChatManager(IChatRepository chatRepository,
            ISpaceReadonlyRepository spaceRepository,
            IUserReadonlyRepository userRepository,
            IMessageRepository messageRepository,
            IOfferRepository offerRepository,
            IOfferHistoryRepository offerHistoryRepository,
            IChatMemberRepository chatMemberRepository,
            IDictionaryProvider dictionaryProvider,
            IUnitOfWork unitOfWork,
            IUserNotifier userNotifier)
        {
            Contract.Requires(chatRepository != null);
            Contract.Requires(spaceRepository != null);
            Contract.Requires(userRepository != null);
            Contract.Requires(messageRepository != null);
            Contract.Requires(offerRepository != null);
            Contract.Requires(offerHistoryRepository != null);
            Contract.Requires(chatMemberRepository != null);
            Contract.Requires(dictionaryProvider != null);
            Contract.Requires(unitOfWork != null);
            Contract.Requires(userNotifier != null);

            this.chatRepository = chatRepository;
            this.spaceRepository = spaceRepository;
            this.userRepository = userRepository;
            this.messageRepository = messageRepository;
            this.offerRepository = offerRepository;
            this.offerHistoryRepository = offerHistoryRepository;
            this.chatMemberRepository = chatMemberRepository;
            this.unitOfWork = unitOfWork;
            this.userNotifier = userNotifier;
            this.dictionaryProvider = dictionaryProvider;
        }

        /// <summary>
        /// Returns message by identifier.
        /// </summary>
        /// <param name="messageId">Message identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Message.</returns>
        public GetMessageResponse GetMessage(Guid messageId, Guid actorId)
        {
            var messageFromDb = GetMessageModel(messageId);

            if (messageFromDb == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.MessageNotFound, messageId));
            }
            if (!messageFromDb.Chat.HasUserAccess(actorId))
            {
                throw new AccessDeniedException();
            }

            var result = new GetMessageResponse(messageFromDb);
            return result;
        }

        /// <summary>
        /// Creates new chat with host.
        /// </summary>
        /// <param name="spaceId">Space Id.</param>
        /// <param name="request">Request.</param>
        /// <returns>Response.</returns>
        public GetMessageWithChatResponse BeginChat(Guid spaceId, PostMessageRequest request)
        {
            Contract.Requires(request != null);

            var space = spaceRepository.GetAll().Include(x => x.User).Where(x => x.Id == spaceId).FutureFirstOrDefault();

            var sender = userRepository.GetAll().Where(x => x.Id == request.SenderId).FutureFirstOrDefault();

            var offers = offerRepository.GetAll().Include(x => x.MessageOfferHistory);
            var currentChat = chatRepository.GetAll()
                .Where(x => x.UserId == request.SenderId && x.SpaceId == spaceId)
                .Join(offers, inner => inner.LastMessageOfferId, outer => outer.Id, (inner, outer) => outer)
                .Where(x => x.MessageOfferHistory != null
                && (x.MessageOfferHistory.StatusId == approvedStatusId
                || x.MessageOfferHistory.StatusId == pendingStatusId))
                .FutureFirstOrDefault();

            if (space.Value == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.SpaceNotFound, spaceId));
            }

            if (space.Value.IsDeleted || !space.Value.IsListed)
            {
                var message = String.Format(Resources.Messages.SpaceIsNotActiveOrDeleted, spaceId);
                throw new BadRequestException(message);
            }

            if (sender.Value == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.UserNotFound, request.SenderId));
            }

            if (currentChat.Value != null)
            {
                throw new BeginChatException(currentChat.Value.Id);
            }

            var newChat = new Chat
            {
                SpaceId = spaceId,
                UserId = request.SenderId,
            };

            newChat.ChatMembers.Add(new ChatMember { UserId = space.Value.UserId });
            newChat.ChatMembers.Add(new ChatMember { UserId = request.SenderId });
            var messageToPost = IncomeMessageToModel(request);
            newChat.Messages.Add(messageToPost);

            chatRepository.Add(newChat);
            unitOfWork.CommitChanges();


            var chatFromDb = chatRepository.GetAll().AttachIncludes().Where(x => x.Id == newChat.Id).AsNoTracking().SingleOrDefault();

            var result = new GetMessageWithChatResponse
            {
                Id = chatFromDb.LastMessageOfferId.Value,
                Chat = new GetChatResponse(chatFromDb),
                Offer = new GetOfferResponse(chatFromDb.LastMessageOffer),
                Sender = new UserInfo(chatFromDb.User),
                MessageDeliveredStatus = Model.Enums.MessageDeliveredStatus.WasSent,
                SentDate = chatFromDb.LastMessageOffer.Message.ReceivedDate,
                DeliveredDate = chatFromDb.LastMessageOffer.Message.SentDate,
                Message = null,
            };

            var notification = new UserNotification
            {
                Message = String.Format(Resources.Messages.NewChatStartedNotification, sender.Value.Firstname, sender.Value.Lastname),
                ObjectId = chatFromDb.Id,
                EventType = EventType.NewChatStarted,
                Badge = chatFromDb.Space.User.TotalUnreadMessages
            };

            try
            {
                userNotifier.SendMessage(space.Value.UserId, notification);
            }
            catch (CommunicationException ex)
            {
                Logger.Error(Resources.Messages.AmazonException, ex);
            }

            return result;
        }

        /// <summary>
        /// Posts new message to chat.
        /// </summary>
        /// <param name="chatId">Chat identifier.</param>
        /// <param name="actorId">Actorr identifier.</param>
        /// <param name="request">Request to post message.</param>
        /// <returns>Posted message.</returns>
        public GetMessageResponse PostMessage(Guid chatId, Guid actorId, PostMessageRequest request)
        {
            Contract.Requires(request != null);

            var chat = chatRepository.GetAll()
                .Include(x => x.ChatMembers.Select(m => m.User))
                .Include(x => x.LastMessageOffer.MessageOfferHistory)
                .AsNoTracking()
                .SingleOrDefault(x => x.Id == chatId);
            if (chat == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.ChatNotFound, chatId));
            }

            if (!chat.HasUserAccess(actorId))
            {
                throw new AccessDeniedException();
            }

            if (chat.LastMessageOffer != null
                && (chat.LastMessageOffer.MessageOfferHistory.StatusId == rejectedStatusId
                || chat.LastMessageOffer.MessageOfferHistory.StatusId == expiredStatusId))
            {
                throw new PostMessageException(Resources.Messages.ChatClosedForNewMessage);
            }

            if (chat.LastMessageOffer != null
                && request.Offer != null
                && chat.LastMessageOffer.MessageOfferHistory.StatusId != pendingStatusId)
            {
                throw new PostMessageException(Resources.Messages.ChatClosedForNewOffer);
            }

            var messageToPost = IncomeMessageToModel(chat.Id, request);

            messageRepository.Add(messageToPost);
            unitOfWork.CommitChanges();

            //chatMemberRepository.GetAll()
            //    .Where(x => x.ChatId == chat.Id && x.UserId != messageToPost.UserId)
            //    .Update(x => new ChatMember { AmountUnreadMessages = x.AmountUnreadMessages + 1 });

            var resultFromDb = GetMessageModel(messageToPost.Id);
            var result = new GetMessageResponse(resultFromDb);
            var recepientChatMember = resultFromDb.Chat.ChatMembers.Single(x => x.UserId != messageToPost.UserId);
            var totalUnreadMessages = recepientChatMember.User.TotalUnreadMessages;
            var notification = new UserNotification
            {
                Message = String.Format(Resources.Messages.NewMessagesNotification,
                    result.Sender.FullName.Firstname, result.Sender.FullName.Lastname),
                ObjectId = chatId,
                EventType = EventType.NewMessagePosted,
                Badge = totalUnreadMessages
            };

            try
            {
                userNotifier.SendMessage(recepientChatMember.UserId, notification);
            }
            catch (CommunicationException ex)
            {
                Logger.Error(Resources.Messages.AmazonException, ex);
            }
            return result;
        }

        /// <summary>
        /// Returns list of user's chats.
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="offset">Offset.</param>
        /// <param name="limit">Limit.</param>
        /// <returns>List of user's chats.</returns>
        public IEnumerable<GetChatResponse> ListMyChats(Guid userId, Int32 offset = 0, Int32 limit = 0)
        {

            var listMyChats = 
                from ch 
                  in chatRepository.GetAll()
                join chM
                  in chatMemberRepository.GetAll()
                  on ch.Id equals chM.ChatId
               where chM.UserId == userId
               select ch;

            var orderChats = listMyChats
                .OrderByDescending(chM => chM.LastModified)
                .Paging(offset, limit);
                
            var resultList = (from user in userRepository
                                  .GetAll()
                                  .Where(x => x.Id == userId)
                              join ch
                                in orderChats
                                on 1 equals 1
                               into outerChat                              
                               from ch 
                                 in outerChat.DefaultIfEmpty()
                              select ch)
                .AttachIncludes()
                .ToList()
                //.AsNoTracking() //the reason of redundand queries
                .Select(x => x == null ? (GetChatResponse)null : new GetChatResponse(x)
            {
                YourUnreadedMessage = x.ChatMembers.FirstOrDefault(cm => cm.UserId == userId).AmountUnreadMessages
            }).ToList();

            if (resultList.Count() == 0)
            {
                throw new NotFoundException(String.Format(Resources.Messages.UserNotFound, userId));
            }
            else if (resultList.Count() == 1 && resultList[0] == null)
            {
                return new List<GetChatResponse>(); //return empty chat list
            }

            return resultList;

        }

        /// <summary>
        /// Returns chat list where user is Renter with possible status for last offer
        /// </summary>
        /// <param name="userId">User identifier.</param>
        /// <param name="lastOfferStatus">Last Offer Status.</param>
        /// <returns>List of user's chats.</returns>
        public IEnumerable<GetChatResponse> ListRenterChats(Guid userId,
            Weezlabs.Storgage.Model.Enums.MessageOfferStatus? lastOfferStatus)
        {
            Guid mohStatus = Guid.Empty;
            if (lastOfferStatus != null)
            {
                mohStatus = ((Weezlabs.Storgage.Model.Enums.MessageOfferStatus)lastOfferStatus).GetDictionaryId();
            }

            var chatList = lastOfferStatus == null
                ? chatRepository.GetAll()
                : chatRepository.GetAll()
                .Where(ch => ch.LastMessageOffer.MessageOfferHistory.StatusId == mohStatus);

            var query = (
                from user
                  in userRepository
                      .GetAll()
                      .Where(x => x.Id == userId)
                join chat
                  in chatList
                  on user.Id equals chat.UserId
                into outerChat

                from result
                  in outerChat.DefaultIfEmpty()

                select result
              )
              .AttachIncludes();
            //.AsNoTracking(); //redundand queries beacouse this

            List<GetChatResponse> resultList = new List<GetChatResponse>();

            resultList = query
                .ToList()
                .Select(ch => ch == null
                    ? (GetChatResponse)null
                    : new GetChatResponse(ch)
                    {
                        YourUnreadedMessage = ch.ChatMembers.First(m => m.UserId == userId).AmountUnreadMessages
                    })
                .ToList();

            //I think that this approach have better performance
            /*
            foreach (var ch in query)
            {
                resultList.Add(ch == null
                    ? (GetChatResponse)null 
                    : new GetChatResponse(ch) 
                    {
                        YourUnreadedMessage = ch.ChatMembers.First(m => m.UserId == userId).AmountUnreadMessages
                    }
                    );                
            }
            */


            if (resultList.Count() == 0)
            {
                throw new NotFoundException(String.Format(Resources.Messages.UserNotFound, userId));
            }
            else if (resultList.Count() == 1 && resultList[0] == null)
            {
                return new List<GetChatResponse>(); //return empty chat list
            }

            return resultList;

        }


        /// <summary>
        /// Returns chat by id.
        /// </summary>
        /// <param name="chatId">Chat identifier.</param>
        /// <param name="userId">User identifier.</param>
        /// <returns>Chat.</returns>
        public GetChatResponse GetChatById(Guid chatId, Guid userId)
        {
            var chat = chatRepository.GetAll().AttachIncludes().Where(x => x.Id == chatId).SingleOrDefault();
            if (chat == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.ChatNotFound, chatId));
            }

            if (!chat.HasUserAccess(userId))
            {
                throw new AccessDeniedException();
            }

            var response = new GetChatResponse(chat) { YourUnreadedMessage = chat.ChatMembers.First(m => m.UserId == userId).AmountUnreadMessages };
            return response;
        }

        /// <summary>
        /// Retrieves last chat for the specified user and space.
        /// </summary>
        /// <param name="userId">user identifier</param>
        /// <param name="spaceId">space identifier</param>
        /// <returns>chat for the specified user and space</returns>
        public GetChatResponse GetLastChat(Guid userId, Guid spaceId)
        {
            User user = userRepository.GetById(userId);

            if (user == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.UserNotFound, userId));
            }

            Chat[] chats = chatRepository.GetAll().Where
                (c => c.UserId == userId && c.SpaceId == spaceId)
                .OrderByDescending(c => c.Id)
                .AttachIncludes().ToArray();

            if (chats.Count() == 0)
            {
                throw new NotFoundException(String.Format(Resources.Messages.ChatNotExistForSpace, userId, spaceId));
            }

            Chat chat = chats.FirstOrDefault();
            GetChatResponse chatResponse = new GetChatResponse(chat)
            {
                YourUnreadedMessage = chat.ChatMembers.FirstOrDefault(cm => cm.UserId == userId).AmountUnreadMessages
            };

            return chatResponse;
        }

        /// <summary>
        /// Returns list of messages chats.
        /// </summary>
        /// <param name="userId">Chat identifier.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <param name="laterOrEqual">Date time offset for last messages.</param>
        /// <returns>List of messages.</returns>
        public IEnumerable<GetMessageResponse> GetMessagesFromChat(Guid chatId, Guid actorId, DateTimeOffset? laterOrEqual = null)
        {
            var chat = chatRepository
                .GetAll()
                .AttachIncludes()
                .SingleOrDefault(x => x.Id == chatId);
            if (chat == null)
            {
                throw new NotFoundException(String.Format(Resources.Messages.ChatNotFound, chatId));
            }

            if (!chat.HasUserAccess(actorId))
            {
                throw new AccessDeniedException();
            }

            IEnumerable<Message> messages = chat.Messages;
            if (laterOrEqual.HasValue)
            {
                messages = chat.Messages.Where(x => x.ReceivedDate >= laterOrEqual.Value);
            }
            var result = chat.Messages.Select(x => new GetMessageResponse(x)).ToList();

            return result;
        }


        /// <summary>
        /// Updates status of current offer.
        /// </summary>
        /// <param name="chatId">Chat identifier.</param>
        /// <param name="newStatus">New status.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Offer after status updating.</returns>
        public GetOfferResponse UpdateCurrentOfferStatus(Guid chatId, Model.Enums.MessageOfferStatus newStatus, Guid actorId)
        {
            var chat = chatRepository
                .GetAll()
                .GetChatWithCurrentOffer(chatId, actorId);


            if (!chat.HasUserAccess(actorId) || chat.LastMessageOffer.Message.UserId == actorId
                && newStatus == Model.Enums.MessageOfferStatus.Approved)
            {
                throw new AccessDeniedException();
            }

            if (chat.LastMessageOffer.MessageOfferHistory.StatusId != pendingStatusId)
            {
                throw new InvalidOfferStatusException(Resources.Messages.CannotChangeStatus);
            }

            var newStates = new List<MessageOfferHistory>();
            newStates.Add(new MessageOfferHistory
            {
                UserId = actorId,
                ChangedStatusDate = DateTimeOffset.Now,
                StatusId = newStatus.GetDictionaryId(),
                MessageOfferId = chat.LastMessageOfferId.Value
            });

            if (newStatus == Model.Enums.MessageOfferStatus.Approved)
            {
                spaceRepository.GetAll().Where(x => x.Id == chat.SpaceId).Update(x => new Space { AvailableSince = null });

                //Reject last pending offers in other chats for the same space.
                var rejectedStatuses = chatRepository.GetAll().Include(x => x.LastMessageOffer.MessageOfferHistory)
                    .Where(x => x.SpaceId == chat.SpaceId
                        && x.Id != chatId
                        && x.LastMessageOffer != null
                        && x.LastMessageOffer.MessageOfferHistory.StatusId == pendingStatusId)
                    .ToList()
                    .Select(x => new MessageOfferHistory
                    {
                        UserId = actorId,
                        ChangedStatusDate = DateTimeOffset.Now,
                        StatusId = rejectedStatusId,
                        MessageOfferId = x.LastMessageOfferId.Value
                    });
                newStates.AddRange(rejectedStatuses);
            }

            offerHistoryRepository.AddRange(newStates);
            unitOfWork.CommitChanges();

            var recepient = chat.ChatMembers.Single(x => x.UserId != actorId).User;
            var message = GetTitleNotificationForOfferStatus(newStatus);
            var eventType = GetEventTypeForOfferStatus(newStatus);
            var notification = new UserNotification
            {
                Message = String.Format(message, recepient.Firstname, recepient.Lastname),
                ObjectId = chat.Id,
                EventType = eventType
            };
            userNotifier.SendMessage(recepient.Id, notification);

            var fromDb = GetMessageModel(chat.LastMessageOffer.Id);
            var result = new GetOfferResponse(fromDb.MessageOffer);
            return result;
        }

        private static EventType GetEventTypeForOfferStatus(Model.Enums.MessageOfferStatus offerStatus)
        {
            switch (offerStatus)
            {
                case Model.Enums.MessageOfferStatus.Approved:
                    return EventType.OfferWasApproved;
                case Model.Enums.MessageOfferStatus.Rejected:
                    return EventType.OfferWasRejected;
                default:
                    throw new ArgumentException("Invalid offer status.");
            }
        }



        /// <summary>
        /// Update status of offer.
        /// </summary>
        /// <param name="chatId">Chat identifier.</param>        
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Offer after status updating.</returns>
        public GetOfferResponse StopCurrentOffer(Guid chatId, Guid actorId)
        {
            var chat = chatRepository.GetAll().GetChatWithCurrentOffer(chatId, actorId);


            if (!chat.HasUserAccess(actorId))
            {
                throw new AccessDeniedException();
            }

            if (chat.LastMessageOffer.MessageOfferHistory.StatusId != approvedStatusId)
            {
                throw new InvalidOfferStatusException(Resources.Messages.StopStatusError);
            }

            var newState = new MessageOfferHistory
            {
                UserId = actorId,
                ChangedStatusDate = DateTimeOffset.Now,
                StatusId = stoppedStatusId,
                MessageOfferId = chat.LastMessageOfferId.Value
            };
            offerHistoryRepository.Add(newState);

            //Pay attention, this date stores now by trigger 
            //[dbo].[tr_MessageOfferHistory_setStopped] on [dbo].[MessageOfferHistory]
            //and function dbo.fnGetUtc0DateWithOffset()

            //chat.LastMessageOffer.StopAt = stopAt;
            //offerRepository.Update(chat.LastMessageOffer);
            unitOfWork.CommitChanges();

            var fromDb = GetMessageModel(chat.LastMessageOfferId.Value);
            var result = new GetOfferResponse(fromDb.MessageOffer);

            var title = GetTitleNotificationForOfferStatus(Model.Enums.MessageOfferStatus.Stopped);
            var notification = new UserNotification
            {
                Message = String.Format(title, result.CurrentState.ChangedBy.FullName.Firstname,
                    result.CurrentState.ChangedBy.FullName.Lastname),
                ObjectId = chatId,
                EventType = EventType.OfferWasStopped
            };
            Guid recipientId = chat.ChatMembers.SingleOrDefault(x => x.UserId != actorId).UserId;
            try
            {
                userNotifier.SendMessage(recipientId, notification);
            }
            catch (CommunicationException ex)
            {
                Logger.Error(Resources.Messages.AmazonException, ex);
            }

            return result;
        }


        private static String GetTitleNotificationForOfferStatus(Model.Enums.MessageOfferStatus status)
        {
            switch (status)
            {
                case Model.Enums.MessageOfferStatus.Approved:
                    return Resources.Messages.OfferWasApprovedNotification;
                case Model.Enums.MessageOfferStatus.Rejected:
                    return Resources.Messages.OfferWasRejectedNotification;
                case Model.Enums.MessageOfferStatus.Stopped:
                    return Resources.Messages.OfferWasStoppedNotification;
                default:
                    throw new ArgumentException("Unsupported offer status:" + status.ToString());
            }
        }

        /// <summary>
        /// Marks messages as readed.
        /// </summary>
        /// <param name="messageIds">Enumerable of message identifiers.</param>
        /// <param name="actorId">Actor identifier.</param>
        /// <returns>Messages were marked as readed.</returns>
        public IEnumerable<GetMessageResponse> MarkMessagesAsRead(IEnumerable<Guid> messageIds, Guid actorId)
        {
            var messagesToUpdate = messageRepository.GetAll()
                .Include(x => x.Chat.ChatMembers)
                .Where(x => x.Chat.ChatMembers.Any(m => m.UserId == actorId && m.AmountUnreadMessages > 0))
                .Join(messageIds, x => x.Id, id => id, (x, id) => x);

            var wasReadStatusId = Model.Enums.MessageDeliveredStatus.WasRead.GetDictionaryId();

            messagesToUpdate.Update(x => new Message { DeliveredStatusId = wasReadStatusId });

            var result = messagesToUpdate.ToList().Select(x => new GetMessageResponse(x));
            return result;
        }

        private static Message IncomeMessageToModel(PostMessageRequest messageDto)
        {
            var message = new Message
            {
                DeliveredStatusId = Model.Enums.MessageDeliveredStatus.WasNotSent.GetDictionaryId(),
                MessageText = messageDto.Message != null ? messageDto.Message.Text : String.Empty,
                ReceivedDate = DateTimeOffset.Now,
                UserId = messageDto.SenderId,
                MessageOffer = messageDto.Offer == null
                ? null
                : new MessageOffer
                {
                    Rate = (Decimal)messageDto.Offer.Rate,
                    RentPeriodId = messageDto.Offer.RentPeriod.GetDictionaryId(),
                    RentSince = messageDto.Offer.RentSince
                },
            };

            if (message.MessageOffer != null)
            {
                var offerState = new MessageOfferHistory
                {
                    StatusId = pendingStatusId,
                    UserId = message.UserId,
                    ChangedStatusDate = DateTimeOffset.Now,
                };

                message.MessageOffer.MessageOfferHistories.Add(offerState);
            }


            return message;
        }

        private static Message IncomeMessageToModel(Guid chatId, PostMessageRequest messageDto)
        {
            var message = IncomeMessageToModel(messageDto);
            message.ChatId = chatId;

            return message;
        }

        private Message GetMessageModel(Guid messageId)
        {
            var query = messageRepository.GetAll()
               .Include(x => x.Chat.ChatMembers)
               .Include(x => x.MessageOffer.MessageOfferHistories.Select(m => m.MessageOfferStatus))
               .Include(x => x.MessageOffer.MessageOfferHistories.Select(m => m.User))
               .Include(x => x.MessageOffer.RentPeriodType)
               .Include(x => x.MessageDeliveredStatu)
               .Include(x => x.User)
               .Where(x => x.Id == messageId)
               .AsNoTracking();
            var messageFromDb = query.SingleOrDefault();
            return messageFromDb;
        }
    }
}
