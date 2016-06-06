namespace Weezlabs.Storgage.ChatService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DataLayer;
    using DataLayer.ChatAndMessages;
    using DataLayer.Dictionaries;
    using DataLayer.Spaces;
    using DataLayer.Users;
    using DataTransferObjects.Message;
    using DataTransferObjects.Space;
    using Model;   
    using Model.Exceptions;
    using UserNotifier;

    using Moq;
    using NUnit.Framework;
    using UserNotifier.Notifications;

    /// <summary>
    /// Test for chat manager.
    /// </summary>
    [TestFixture]
    public class TestChatManager
    {
        private Mock<IChatRepository> chatRepositoryMock = new Mock<IChatRepository>();
        private  Mock<IMessageRepository> messageRepositoryMock = new Mock<IMessageRepository>();
        private Mock<ISpaceReadonlyRepository> spaceRepositoryMock = new Mock<ISpaceReadonlyRepository>();
        private Mock<IUserReadonlyRepository> userRepositoryMock = new Mock<IUserReadonlyRepository>();
        private Mock<IOfferRepository> offerRepositoryMock = new Mock<IOfferRepository>();
        private Mock<IUnitOfWork> unitOfWorkMock = new Mock<IUnitOfWork>();
        private Mock<IDictionaryProvider> dictionaryProviderMock = new Mock<IDictionaryProvider>();
        private Mock<IOfferHistoryRepository> offerHistoryRepositoryMock = new Mock<IOfferHistoryRepository>();
        private Mock<IChatMemberRepository> chatMemberRepositoryMock = new Mock<IChatMemberRepository>();
        private Mock<IUserNotifier> userNotifierMock = new Mock<IUserNotifier>();

        private IList<Chat> chatList;
        private IList<Message> messageList;
        private IEnumerable<Space> spaces;
        private IEnumerable<User> users;

        private ChatManager chatManager;

        private User Owner { get { return users.Single(x => x.Lastname == "Owner"); } }

        private User Tenant { get { return users.Single(x => x.Lastname == "Tenant"); } }

        private User User { get { return users.Single(x => x.Lastname == "User"); } }

        private MessageDeliveredStatus WasNotSent
        {
            get
            {
                return 
                    dictionaryProviderMock.Object.MessageDeliveredStatuses
                        .Single(x => x.ToEnum() == Model.Enums.MessageDeliveredStatus.WasNotSent);
            }
        }

        private MessageDeliveredStatus WasRead
        {
            get
            {
                return
                    dictionaryProviderMock.Object.MessageDeliveredStatuses
                        .Single(x => x.ToEnum() == Model.Enums.MessageDeliveredStatus.WasRead);
            }
        }

        private MessageOfferStatus Pending
        {
            get
            {
                return
                    dictionaryProviderMock.Object.MessageOfferStatuses
                        .Single(x => x.ToEnum() == Model.Enums.MessageOfferStatus.Pending);
            }
        }

        private MessageOfferStatus Approved
        {
            get
            {
                return
                    dictionaryProviderMock.Object.MessageOfferStatuses
                        .Single(x => x.ToEnum() == Model.Enums.MessageOfferStatus.Approved);
            }
        }

        private MessageOfferStatus Rejected
        {
            get
            {
                return
                    dictionaryProviderMock.Object.MessageOfferStatuses
                        .Single(x => x.ToEnum() == Model.Enums.MessageOfferStatus.Rejected);
            }
        }

        private RentPeriodType LesserThanThreeMonths
        {
            get
            {
                return
                    dictionaryProviderMock.Object.RentPeriodTypes
                        .Single(x => x.ToEnum() == Model.Enums.RentPeriodType.LesserOrEqualThreeMonths);
            }
        }


        #region Init moqs
        private void Init()
        {            
            dictionaryProviderMock.Setup(x => x.MessageOfferStatuses).Returns(new MessageOfferStatus[]
            {
                new MessageOfferStatus { Id = Guid.NewGuid(), Synonym = Model.Enums.MessageOfferStatus.Approved.ToString() },
                new MessageOfferStatus { Id = Guid.NewGuid(), Synonym = Model.Enums.MessageOfferStatus.Pending.ToString() },
                new MessageOfferStatus { Id = Guid.NewGuid(), Synonym = Model.Enums.MessageOfferStatus.Rejected.ToString() }
            });

            dictionaryProviderMock.Setup(x => x.MessageDeliveredStatuses).Returns(new MessageDeliveredStatus[]
            {
                new MessageDeliveredStatus { Id = Guid.NewGuid(), Synonym = Model.Enums.MessageDeliveredStatus.WasNotSent.ToString() },
                new MessageDeliveredStatus { Id = Guid.NewGuid(), Synonym = Model.Enums.MessageDeliveredStatus.WasRead.ToString() },
                new MessageDeliveredStatus { Id = Guid.NewGuid(), Synonym = Model.Enums.MessageDeliveredStatus.WasSent.ToString() }
            });

            dictionaryProviderMock.Setup(x => x.PhoneVerificationStatuses).Returns(new PhoneVerificationStatus[]
            {
                new PhoneVerificationStatus { Id = Guid.NewGuid(), Synonym = Model.Enums.PhoneVerificationStatus.MustVerified.ToString() },
                new PhoneVerificationStatus { Id = Guid.NewGuid(), Synonym = Model.Enums.PhoneVerificationStatus.NotVerified.ToString() },
                new PhoneVerificationStatus { Id = Guid.NewGuid(), Synonym = Model.Enums.PhoneVerificationStatus.Verified.ToString() }
            });

            dictionaryProviderMock.Setup(x => x.RentPeriodTypes).Returns(new RentPeriodType[] 
            {
                new RentPeriodType { Id = Guid.NewGuid(), Synonym = Model.Enums.RentPeriodType.LesserOrEqualThreeMonths.ToString() },        
                new RentPeriodType { Id = Guid.NewGuid(), Synonym = Model.Enums.RentPeriodType.LesserOrEqualSixMonths.ToString() },
                new RentPeriodType { Id = Guid.NewGuid(), Synonym = Model.Enums.RentPeriodType.LesserOrEqualYear.ToString() },
                new RentPeriodType { Id = Guid.NewGuid(), Synonym = Model.Enums.RentPeriodType.MoreYear.ToString() }
            });

            dictionaryProviderMock.Setup(x => x.SizeTypes).Returns(new SizeType[] 
            {
                new SizeType { Id = Guid.NewGuid(), Synonym = Model.Enums.SizeType.Small.ToString() },
                new SizeType { Id = Guid.NewGuid(), Synonym = Model.Enums.SizeType.Medium.ToString() },
                new SizeType { Id = Guid.NewGuid(), Synonym = Model.Enums.SizeType.Large.ToString() },
                new SizeType { Id = Guid.NewGuid(), Synonym = Model.Enums.SizeType.XLarge.ToString() }
            });

            dictionaryProviderMock.Setup(x => x.SpaceAccessTypes).Returns(new SpaceAccessType[] 
            {
                new SpaceAccessType { Id = Guid.NewGuid(), Synonym = Model.Enums.SpaceAccessType.Limited.ToString() },
                new SpaceAccessType { Id = Guid.NewGuid(), Synonym = Model.Enums.SpaceAccessType.Unlimited.ToString() }
            });

            dictionaryProviderMock.Setup(x => x.SpaceTypes).Returns(new SpaceType[] 
            {
                new SpaceType { Id = Guid.NewGuid(), Synonym = Model.Enums.SpaceType.Indoor.ToString() },
                new SpaceType { Id = Guid.NewGuid(), Synonym = Model.Enums.SpaceType.Outdoor.ToString() }
            });

            chatList = new List<Chat>();
            messageList = new List<Message>();

            var verifiedPhoneStatus = 
                dictionaryProviderMock.Object.PhoneVerificationStatuses
                    .Single(x => x.ToEnum() == Model.Enums.PhoneVerificationStatus.Verified);
            var mustVerifiedPhoneStatus = dictionaryProviderMock.Object.PhoneVerificationStatuses
                            .Single(x => x.ToEnum() == Model.Enums.PhoneVerificationStatus.MustVerified);
            users = new User[]
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Lastname = "Owner",
                    Firstname = "Owner",
                    Email = "owner@test.tst",
                    Phone = "1234567890",
                    PhoneVerificationStatus = verifiedPhoneStatus,
                    PhoneVerificationStatusID = verifiedPhoneStatus.Id
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Lastname = "Tenant",
                    Firstname = "Tenant",
                    Email = "tenant@test.tst",
                    Phone = "1234567890",
                    PhoneVerificationStatus = mustVerifiedPhoneStatus,
                    PhoneVerificationStatusID = mustVerifiedPhoneStatus.Id
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Lastname = "User",
                    Firstname = "User",
                    Email = "user@test.tst",
                    Phone = "1234567890",
                    PhoneVerificationStatus = mustVerifiedPhoneStatus,
                    PhoneVerificationStatusID = mustVerifiedPhoneStatus.Id
                }
            };

            Tenant.ChatMembers.Clear();
            Owner.ChatMembers.Clear();

            var spaceSize = dictionaryProviderMock.Object.SizeTypes.Single(x => x.ToEnum() == Model.Enums.SizeType.Medium);
            var spaceAccessType = dictionaryProviderMock.Object.SpaceAccessTypes
                .Single(x => x.ToEnum() == Model.Enums.SpaceAccessType.Unlimited);
            var spaceType = dictionaryProviderMock.Object.SpaceTypes.Single(x => x.ToEnum() == Model.Enums.SpaceType.Indoor);
            var zip = new Zip { Id = Guid.NewGuid(), ZipCode = "1234567890" };
            spaces = new Space[]
            {
                new Space
                {
                    Id = Guid.NewGuid(),
                    UserId = Owner.Id,
                    User = Owner,
                    IsListed = true,
                    Rate = (Decimal)100.0,
                    Location = new GeoPoint { Latitude = 47.4567, Longitude = 22.4567 }.GetPoint(),
                    SizeTypeId = spaceSize.Id,
                    SizeType = spaceSize,
                    SpaceAccessTypeId = spaceAccessType.Id,
                    SpaceAccessType = spaceAccessType,
                    SpaceTypeId = spaceType.Id,
                    SpaceType = spaceType,  
                    FullAddress = "USA, New York, Manhatten, 5th Avenue, East side, 104",
                    ShortAddress = "USA, New York, Manhatten, 5th Avenue, East side",
                    Zip = zip,
                    ZipId = zip.Id
                },
            };

            userRepositoryMock.Setup(x => x.GetAll()).Returns(users.AsQueryable());
            userRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).Returns<Guid>(id => users.SingleOrDefault(x => x.Id == id));
            
            spaceRepositoryMock.Setup(x => x.GetAll()).Returns(spaces.AsQueryable());
            spaceRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).Returns<Guid>(id => spaces.SingleOrDefault(x => x.Id == id));

            unitOfWorkMock.Setup(x => x.CommitChanges()).Callback(() => { });

            chatRepositoryMock.Setup(x => x.GetAll()).Returns(chatList.AsQueryable());
            chatRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>())).Returns<Guid>(id => chatList.SingleOrDefault(x => x.Id == id));
            chatRepositoryMock.Setup(x => x.Add(It.IsAny<Chat>())).Callback<Chat>(x =>
            {
                x.Id = Guid.NewGuid();
                x.Space = spaces.Single(s => s.Id == x.SpaceId);
                x.User = users.Single(u => u.Id == x.UserId);
                x.ChatMembers.ToList().ForEach(cm =>
                {
                    cm.ChatId = x.Id;
                    cm.Chat = x;
                    cm.User = users.Single(u => u.Id == cm.UserId);
                    users.Single(u => u.Id == cm.UserId).ChatMembers.Add(cm);
                });
                chatList.Add(x);

                x.Messages.ToList().ForEach(m =>
                {
                    m.ChatId = x.Id;                    
                    messageRepositoryMock.Object.Add(m);
                });
            });
            chatRepositoryMock.Setup(x => x.Update(It.IsAny<Chat>())).Callback<Chat>(x => 
            {
                var toRemove = chatList.Single(y => y.Id == x.Id);
                chatList.Remove(toRemove);
                chatList.Add(x);
            });

            messageRepositoryMock.Setup(x => x.GetAll()).Returns(messageList.AsQueryable());
            messageRepositoryMock.Setup(x => x.GetById(It.IsAny<Guid>()))
                .Returns<Guid>(id => messageList.SingleOrDefault(x => x.Id == id));
            messageRepositoryMock.Setup(x => x.Add(It.IsAny<Message>())).Callback<Message>(x =>
            {
                x.Id = Guid.NewGuid();
                x.MessageDeliveredStatu = dictionaryProviderMock.Object.MessageDeliveredStatuses.Single(s => s.Id == x.DeliveredStatusId);
                if (x.MessageOffer != null)
                {
                    x.MessageOffer.Id = x.Id;
                    x.MessageOffer.Message = x;
                    x.MessageOffer.RentPeriodType = dictionaryProviderMock.Object.RentPeriodTypes.Single(r => r.Id == x.MessageOffer.RentPeriodId);
                    x.MessageOffer.MessageOfferHistories.ToList().ForEach(h => 
                    {
                        h.Id = Guid.NewGuid();
                        h.MessageOffer = x.MessageOffer;
                        h.MessageOfferStatus = dictionaryProviderMock.Object.MessageOfferStatuses.Single(s => s.Id == h.StatusId);
                        h.User = users.Single(u => u.Id == h.UserId);
                    });                                                                               
                            
                }
                x.User = users.Single(u => u.Id == x.UserId);
                x.Chat = chatList.Single(c => c.Id == x.ChatId);                
                x.User = users.Single(u => u.Id == x.UserId);                
                messageList.Add(x);
            });
            messageRepositoryMock.Setup(x => x.Update(It.IsAny<Message>())).Callback<Message>(x =>
            {
                var toRemove = messageList.Single(y => y.Id == x.Id);
                messageList.Remove(toRemove);
                messageList.Add(x);
            });

            offerRepositoryMock.Setup(x => x.GetAll())
                .Returns(messageList.Where(x => x.MessageOffer != null).Select(x => x.MessageOffer).AsQueryable());
            offerRepositoryMock.Setup(x => x.Update(It.IsAny<MessageOffer>())).Callback<MessageOffer>(x => 
            {
                var message = messageList.Single(m => m.Id == x.Id);                
            });

            offerHistoryRepositoryMock.Setup(x => x.Add(It.IsAny<MessageOfferHistory>())).Callback<MessageOfferHistory>(x => 
            {
                x.Id = Guid.NewGuid();
                x.User = users.Single(u => u.Id == x.UserId);
                x.MessageOffer = messageList.Single(m => m.Id == x.MessageOfferId).MessageOffer;
                x.MessageOfferStatus = dictionaryProviderMock.Object.MessageOfferStatuses.Single(s => s.Id == x.StatusId);
                x.MessageOffer.MessageOfferHistories.Add(x);
            });

            chatMemberRepositoryMock.Setup(x => x.Update(It.IsAny<ChatMember>()))
                .Callback<ChatMember>(x =>
                {                    
                    var chat = chatList.Single(c => c.Id == x.ChatId);
                    var itemToRemove = chat.ChatMembers.Single(m => m.Id == x.Id);
                    chat.ChatMembers.Remove(itemToRemove);
                    chat.ChatMembers.Add(x);
                });

            userNotifierMock.Setup(x => x.SendMessage(It.IsAny<Guid>(), It.IsAny<UserNotification>()))
                .Callback<UserNotification>(x => { });

            chatManager = new ChatManager(chatRepositoryMock.Object,
                spaceRepositoryMock.Object,
                userRepositoryMock.Object,
                messageRepositoryMock.Object,
                offerRepositoryMock.Object,
                offerHistoryRepositoryMock.Object,
                chatMemberRepositoryMock.Object,
                dictionaryProviderMock.Object,
                unitOfWorkMock.Object,                
                userNotifierMock.Object);
        }
        #endregion
   
        [Test]
        public void TestGetMessagesFromChat()
        {
            Init();
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Space = spaces.First(),
                SpaceId = spaces.First().Id,
                User = Tenant,
                UserId = Tenant.Id,                
            };

            chat.ChatMembers.Add(new ChatMember
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                User = Tenant,
                UserId = Tenant.Id,
            });

            chat.ChatMembers.Add(new ChatMember
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                User = Owner,
                UserId = Owner.Id,
                AmountUnreadMessages = 1,
            });

            var message = new Message
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                DeliveredStatusId = WasNotSent.Id,
                MessageDeliveredStatu = WasNotSent,
                MessageText = "Hi, I'm going to Costa-Rica",
                ReceivedDate = DateTimeOffset.UtcNow,
                User = Tenant,
                UserId = Tenant.Id,
            };

            var messageOffer = new Message
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                DeliveredStatusId = WasNotSent.Id,
                MessageDeliveredStatu = WasNotSent,                
                ReceivedDate = DateTimeOffset.UtcNow,
                User = Tenant,
                UserId = Tenant.Id,
            };

            var pendingOfferState = new MessageOfferHistory
            {
                Id = Guid.NewGuid(),                
                StatusId = Pending.Id,
                MessageOfferStatus = Pending,
                ChangedStatusDate = DateTimeOffset.Now,
                UserId = Tenant.Id,
                User = Tenant,
                MessageOfferId = messageOffer.Id,                                 
            };

            var approvedOfferState = new MessageOfferHistory
            {
                Id = Guid.NewGuid(),                
                StatusId = Approved.Id,
                MessageOfferStatus = Approved,
                ChangedStatusDate = DateTimeOffset.Now,
                UserId = Owner.Id,
                User = Owner,
                MessageOfferId = messageOffer.Id,
            };

            var offer = new MessageOffer
            {
                Id = messageOffer.Id,
                Message = messageOffer,
                Rate = 100,              
                RentPeriodId = LesserThanThreeMonths.Id,
                RentPeriodType = LesserThanThreeMonths,                
            };
            approvedOfferState.MessageOffer = pendingOfferState.MessageOffer = offer;
            messageOffer.MessageOffer = offer;
            offer.MessageOfferHistories.Add(pendingOfferState);
            offer.MessageOfferHistories.Add(approvedOfferState);
            chat.Messages.Add(message);
            chat.Messages.Add(messageOffer);
            chatList.Add(chat);
            messageList.Add(message);
            messageList.Add(messageOffer);

            var getMessagesResponse = chatManager.GetMessagesFromChat(chat.Id, Tenant.Id);
            Assert.AreEqual(2, getMessagesResponse.Count());
            Assert.IsTrue(getMessagesResponse.Any(x => x.Message != null && x.Message.Text == message.MessageText));
            Assert.IsTrue(getMessagesResponse.Any(x => x.Offer != null && (decimal)x.Offer.Rate == offer.Rate));
            Assert.IsTrue(getMessagesResponse.Any(x => x.Offer != null && x.Offer.States.Count() == 2));
            Assert.IsTrue(getMessagesResponse.Any(x => x.Offer != null && x.Offer.States.Any(y => y.Status == Model.Enums.MessageOfferStatus.Pending)));
            Assert.IsTrue(getMessagesResponse.Any(x => x.Offer != null && x.Offer.States.Any(y => y.Status == Model.Enums.MessageOfferStatus.Approved)));            
            Assert.AreEqual(1, getMessagesResponse.Count(x => x.Offer == null));
            Assert.AreEqual(1, getMessagesResponse.Count(x => x.Message == null));

            getMessagesResponse = chatManager.GetMessagesFromChat(chat.Id, Owner.Id);
            Assert.AreEqual(2, getMessagesResponse.Count());
            Assert.IsTrue(getMessagesResponse.Any(x => x.Message != null && x.Message.Text == message.MessageText));
            Assert.IsTrue(getMessagesResponse.Any(x => x.Offer != null && (decimal)x.Offer.Rate == offer.Rate));
            Assert.IsTrue(getMessagesResponse.Any(x => x.Offer != null && x.Offer.States.Any(y => y.Status == Model.Enums.MessageOfferStatus.Pending)));
            Assert.IsTrue(getMessagesResponse.Any(x => x.Offer != null && x.Offer.States.Any(y => y.Status == Model.Enums.MessageOfferStatus.Approved)));            
            Assert.AreEqual(1, getMessagesResponse.Count(x => x.Offer == null));
            Assert.AreEqual(1, getMessagesResponse.Count(x => x.Message == null));

            Assert.Throws<NotFoundException>(() => chatManager.GetMessagesFromChat(Guid.NewGuid(), Tenant.Id));

            Assert.Throws<AccessDeniedException>(() => chatManager.GetMessagesFromChat(chat.Id, Guid.NewGuid()));
        }

        [Test]
        public void TestBeginChat()
        {
            Init();

            Assert.AreEqual(0, chatList.Count);

            var request = new PostMessageRequest
            {
                SenderId = Tenant.Id,
                Message = new MessageInfo { Text = "Hi..." },
            };
            chatManager.BeginChat(spaces.First().Id, request);

            Assert.AreEqual(1, chatList.Count);
            Assert.AreEqual(1, chatList.Single().Messages.Count);
            Assert.AreEqual(request.Message.Text, chatList.Single().Messages.Single().MessageText);
            Assert.AreEqual(request.SenderId, chatList.Single().Messages.Single().UserId);

            Assert.AreEqual(0, chatList.Single().ChatMembers.Single(x => x.UserId == Tenant.Id).AmountUnreadMessages);
            Assert.AreEqual(1, chatList.Single().ChatMembers.Single(x => x.UserId == Owner.Id).AmountUnreadMessages);

            Assert.Throws<BeginChatException>(() => chatManager.BeginChat(spaces.First().Id, request));

            var offerState = new MessageOfferHistory
            {
                Id = Guid.NewGuid(),
                UserId = Tenant.Id,                
                StatusId = Rejected.Id,
                MessageOfferStatus = Rejected,
                ChangedStatusDate = DateTimeOffset.Now
            };

            var offer = new MessageOffer
            {
                Id = Guid.NewGuid(),
                Rate = 100,
                RentPeriodId = LesserThanThreeMonths.Id,                
            };
            offer.MessageOfferHistories.Add(offerState);                        

            chatManager.BeginChat(spaces.First().Id, request);

            Assert.AreEqual(2, chatList.Count);
            Assert.AreEqual(1, chatList.Last().Messages.Count);
            Assert.AreEqual(request.Message.Text, chatList.Last().Messages.Single().MessageText);
            Assert.AreEqual(request.SenderId, chatList.Last().Messages.Single().UserId);

            Assert.Throws<NotFoundException>(() => chatManager.BeginChat(Guid.NewGuid(), request));
            request.SenderId = Guid.NewGuid();
            Assert.Throws<NotFoundException>(() => chatManager.BeginChat(spaces.First().Id, request));
        }

        [Test]
        public void TestListMyChats()
        {
            Init();
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Space = spaces.First(),
                SpaceId = spaces.First().Id,
                User = Tenant,
                UserId = Tenant.Id,
            };

            var tenantMember = new ChatMember
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                User = Tenant,
                UserId = Tenant.Id,
            };
            chat.ChatMembers.Add(tenantMember);

            var ownerMember = new ChatMember
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                User = Owner,
                UserId = Owner.Id,
                AmountUnreadMessages = 1,
            };
            chat.ChatMembers.Add(ownerMember);

            var message = new Message
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                DeliveredStatusId = WasNotSent.Id,
                MessageDeliveredStatu = WasNotSent,
                MessageText = "Hi, I'm going to Costa-Rica",
                ReceivedDate = DateTimeOffset.UtcNow,
                User = Tenant,
                UserId = Tenant.Id,
            };
            
            chat.Messages.Add(message);            
            chatList.Add(chat);
            messageList.Add(message);
            Tenant.ChatMembers.Add(tenantMember);
            Owner.ChatMembers.Add(ownerMember);

            var listMyChatsResponse = chatManager.ListMyChats(Tenant.Id);
            Assert.AreEqual(1, listMyChatsResponse.Count());
            Assert.AreEqual(chat.Id, listMyChatsResponse.Single().Id);
            Assert.AreEqual(0, listMyChatsResponse.Single().YourUnreadedMessage);

            listMyChatsResponse = chatManager.ListMyChats(Owner.Id);
            Assert.AreEqual(1, listMyChatsResponse.Count());
            Assert.AreEqual(chat.Id, listMyChatsResponse.Single().Id);
            Assert.AreEqual(1, listMyChatsResponse.Single().YourUnreadedMessage);

            Assert.Throws<NotFoundException>(() => chatManager.ListMyChats(Guid.NewGuid()));
            Assert.IsEmpty(chatManager.ListMyChats(User.Id));
        }        

        [Test]
        public void TestPostMessage()
        {
            Init();
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Space = spaces.First(),
                SpaceId = spaces.First().Id,
                User = Tenant,
                UserId = Tenant.Id,
            };

            var tenantMember = new ChatMember
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                User = Tenant,
                UserId = Tenant.Id,
            };
            chat.ChatMembers.Add(tenantMember);

            var ownerMember = new ChatMember
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                User = Owner,
                UserId = Owner.Id,
                AmountUnreadMessages = 1,
            };
            chat.ChatMembers.Add(ownerMember);

            var message = new Message
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                DeliveredStatusId = WasNotSent.Id,
                MessageDeliveredStatu = WasNotSent,
                MessageText = "Hi, I'm going to Costa-Rica",
                ReceivedDate = DateTimeOffset.UtcNow,
                User = Tenant,
                UserId = Tenant.Id,
            };

            chat.Messages.Add(message);
            chatList.Add(chat);
            messageList.Add(message);

            var request = new PostMessageRequest
            {
                SenderId = Tenant.Id,
                Offer = new PostOfferRequest
                {
                    Rate = 100,
                    RentPeriod = Model.Enums.RentPeriodType.MoreYear,
                    RentSince = DateTimeOffset.UtcNow + TimeSpan.FromDays(1)
                }
            };

            Assert.AreEqual(1, messageList.Count);
            Assert.AreEqual(1, chatList.Count);
            Assert.IsNull(chatList.Single().LastMessageOffer);
            var getMessageResponse = chatManager.PostMessage(chat.Id, Tenant.Id, request);

            Assert.AreEqual(2, messageList.Count);
            Assert.AreEqual(request.Offer.Rate, getMessageResponse.Offer.Rate);
            Assert.AreEqual(request.SenderId, getMessageResponse.Sender.Id);
            Assert.AreEqual(2, chatList.Single().ChatMembers.Single(x => x.UserId == Owner.Id).AmountUnreadMessages);
            Assert.IsNotNull(chatList.Single().LastMessageOffer);
            Assert.AreEqual(chatList.Single().LastMessageOfferId, getMessageResponse.Id);

            Assert.Throws<NotFoundException>(() => chatManager.PostMessage(Guid.NewGuid(), Tenant.Id, request));
            Assert.Throws<AccessDeniedException>(() => chatManager.PostMessage(chat.Id, Guid.NewGuid(), request));
            Assert.Throws<AccessDeniedException>(() => chatManager.PostMessage(chat.Id, User.Id, request));

            request = new PostMessageRequest
            {
                SenderId = Owner.Id,
                Offer = new PostOfferRequest
                {
                    Rate = 150,
                    RentPeriod = Model.Enums.RentPeriodType.MoreYear,
                    RentSince = DateTimeOffset.UtcNow + TimeSpan.FromDays(1)
                }
            };

            getMessageResponse = chatManager.PostMessage(chat.Id, Owner.Id, request);

            Assert.AreEqual(3, messageList.Count);
            Assert.AreEqual(request.Offer.Rate, getMessageResponse.Offer.Rate);
            Assert.AreEqual(request.SenderId, getMessageResponse.Sender.Id);                  

            Assert.Throws<PostMessageException>(() => chatManager.PostMessage(chat.Id, Tenant.Id, new PostMessageRequest
            {
                SenderId = Tenant.Id,
                Offer = new PostOfferRequest
                {
                    Rate = 100,
                    RentPeriod = Model.Enums.RentPeriodType.MoreYear,
                    RentSince = DateTimeOffset.UtcNow + TimeSpan.FromDays(1)
                }
            }));

            Assert.Throws<PostMessageException>(() => chatManager.PostMessage(chat.Id, Tenant.Id, new PostMessageRequest
            {
                SenderId = Tenant.Id,
                Offer = new PostOfferRequest
                {
                    Rate = 100,
                    RentPeriod = Model.Enums.RentPeriodType.MoreYear,
                    RentSince = DateTimeOffset.UtcNow + TimeSpan.FromDays(1)
                }
            }));

            Assert.Throws<PostMessageException>(() => chatManager.PostMessage(chat.Id, Tenant.Id, new PostMessageRequest
            {
                SenderId = Tenant.Id,
                Message = new MessageInfo { Text = "Hi" }
            }));                    

            Assert.Throws<PostMessageException>(() => chatManager.PostMessage(chat.Id, Tenant.Id, new PostMessageRequest
            {
                SenderId = Tenant.Id,
                Offer = new PostOfferRequest
                {
                    Rate = 100,
                    RentPeriod = Model.Enums.RentPeriodType.MoreYear,
                    RentSince = DateTimeOffset.UtcNow + TimeSpan.FromDays(1)
                }
            }));

            getMessageResponse = chatManager.PostMessage(chat.Id, Tenant.Id, new PostMessageRequest
            {
                SenderId = Tenant.Id,
                Message = new MessageInfo { Text = "Hi" }
            });

            Assert.AreEqual(4, messageList.Count);

        }

        [Test]
        public void TestUpdateOfferStatus()
        {
            Init();
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Space = spaces.First(),
                SpaceId = spaces.First().Id,
                User = Tenant,
                UserId = Tenant.Id,
            };

            chat.ChatMembers.Add(new ChatMember
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                User = Tenant,
                UserId = Tenant.Id,
            });

            chat.ChatMembers.Add(new ChatMember
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                User = Owner,
                UserId = Owner.Id,
                AmountUnreadMessages = 1,
            });

            var message = new Message
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                DeliveredStatusId = WasNotSent.Id,
                MessageDeliveredStatu = WasNotSent,
                MessageText = "Hi, I'm going to Costa-Rica",
                ReceivedDate = DateTimeOffset.UtcNow,
                User = Tenant,
                UserId = Tenant.Id,
            };

            var messageOffer = new Message
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                DeliveredStatusId = WasNotSent.Id,
                MessageDeliveredStatu = WasNotSent,
                ReceivedDate = DateTimeOffset.UtcNow,
                User = Tenant,
                UserId = Tenant.Id,
            };

            var pendingOfferState = new MessageOfferHistory
            {
                Id = Guid.NewGuid(),                
                StatusId = Pending.Id,
                MessageOfferStatus = Pending,
                ChangedStatusDate = DateTimeOffset.Now,
                UserId = Tenant.Id,
                User = Tenant,
                MessageOfferId = messageOffer.Id,
            };        

            var offer = new MessageOffer
            {
                Id = messageOffer.Id,
                Message = messageOffer,
                Rate = 100,
                RentPeriodId = LesserThanThreeMonths.Id,
                RentPeriodType = LesserThanThreeMonths,                
            };
            pendingOfferState.MessageOffer = offer;
            messageOffer.MessageOffer = offer;
            offer.MessageOfferHistories.Add(pendingOfferState);            
            chat.Messages.Add(message);
            chat.Messages.Add(messageOffer);
            chatList.Add(chat);
            messageList.Add(message);
            messageList.Add(messageOffer);

            Assert.AreEqual(Model.Enums.MessageOfferStatus.Pending.GetDictionaryId(), offer.CurrentMessageOfferHistory.Value);
            Assert.Throws<AccessDeniedException>(() => chatManager.UpdateCurrentOfferStatus(offer.Id, Model.Enums.MessageOfferStatus.Approved, Tenant.Id));
            var response = chatManager.UpdateCurrentOfferStatus(offer.Id, Model.Enums.MessageOfferStatus.Approved, Owner.Id);
            Assert.AreEqual(Model.Enums.MessageOfferStatus.Approved.GetDictionaryId(), offer.CurrentMessageOfferHistory.Value);

            Assert.Throws<NotFoundException>(() => chatManager.UpdateCurrentOfferStatus(Guid.NewGuid(), Model.Enums.MessageOfferStatus.Approved, Owner.Id));
            Assert.Throws<AccessDeniedException>(() => chatManager.UpdateCurrentOfferStatus(offer.Id, Model.Enums.MessageOfferStatus.Approved, Guid.NewGuid()));
            Assert.Throws<InvalidOfferStatusException>(() => chatManager.UpdateCurrentOfferStatus(offer.Id, Model.Enums.MessageOfferStatus.Rejected, Owner.Id));
           
            Assert.Throws<InvalidOfferStatusException>(() => chatManager.UpdateCurrentOfferStatus(offer.Id, Model.Enums.MessageOfferStatus.Approved, Owner.Id));
        }

        [Test]
        [Ignore("Should rework mocking")]
        public void TestMarkMessagesAsRead()
        {
            Init();
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                Space = spaces.First(),
                SpaceId = spaces.First().Id,
                User = Tenant,
                UserId = Tenant.Id,
            };

            chat.ChatMembers.Add(new ChatMember
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                User = Tenant,
                UserId = Tenant.Id,
            });

            chat.ChatMembers.Add(new ChatMember
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                User = Owner,
                UserId = Owner.Id,
                AmountUnreadMessages = 2,
            });

            var message = new Message
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                DeliveredStatusId = WasNotSent.Id,
                MessageDeliveredStatu = WasNotSent,
                MessageText = "Hi, I'm going to Costa-Rica",
                ReceivedDate = DateTimeOffset.UtcNow,
                User = Tenant,
                UserId = Tenant.Id,
            };

            var messageOffer = new Message
            {
                Id = Guid.NewGuid(),
                Chat = chat,
                ChatId = chat.Id,
                DeliveredStatusId = WasNotSent.Id,
                MessageDeliveredStatu = WasNotSent,
                ReceivedDate = DateTimeOffset.UtcNow,
                User = Tenant,
                UserId = Tenant.Id,
            };

            var pendingOfferState = new MessageOfferHistory
            {
                Id = Guid.NewGuid(),                
                StatusId = Pending.Id,
                MessageOfferStatus = Pending,
                ChangedStatusDate = DateTimeOffset.Now,
                UserId = Tenant.Id,
                User = Tenant,
                MessageOfferId = messageOffer.Id,
            };

            var approvedOfferState = new MessageOfferHistory
            {
                Id = Guid.NewGuid(),                
                StatusId = Approved.Id,
                MessageOfferStatus = Approved,
                ChangedStatusDate = DateTimeOffset.Now,
                UserId = Owner.Id,
                User = Owner,
                MessageOfferId = messageOffer.Id,
            };

            var offer = new MessageOffer
            {
                Id = messageOffer.Id,
                Message = messageOffer,
                Rate = 100,
                RentPeriodId = LesserThanThreeMonths.Id,
                RentPeriodType = LesserThanThreeMonths,                
            };
            approvedOfferState.MessageOffer = pendingOfferState.MessageOffer = offer;
            messageOffer.MessageOffer = offer;
            offer.MessageOfferHistories.Add(pendingOfferState);
            offer.MessageOfferHistories.Add(approvedOfferState);
            chat.Messages.Add(message);
            chat.Messages.Add(messageOffer);
            chatList.Add(chat);
            messageList.Add(message);
            messageList.Add(messageOffer);

            Assert.AreEqual(2, chatList.Single().ChatMembers.Single(x => x.UserId == Owner.Id).AmountUnreadMessages);
            var request = new Guid[] { message.Id, messageOffer.Id };
            var response = chatManager.MarkMessagesAsRead(request, Owner.Id);
            Assert.AreEqual(2, response.Count());
            Assert.AreEqual(2, response.Select(x => x.Id).Join(request, x => x, y => y, (x, y) => x).Count());
            Assert.AreEqual(0, chatList.Single().ChatMembers.Single(x => x.UserId == Owner.Id).AmountUnreadMessages);
        }
    }
}
