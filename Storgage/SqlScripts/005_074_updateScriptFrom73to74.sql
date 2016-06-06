create 
 index idx_ChatMember_UserId
    on dbo.ChatMember(UserID)
       include (Id, ChatId, AmountUnreadMessages)
go

--idx_Message_ChatId_incl
create 
 --drop
 index idx_Message_ChatId_UserId_DeliveredStatusId
    on dbo.Message(ChatId, UserId, DeliveredStatusId)
       include (Id, ReceivedDate, SentDate, Message)
go