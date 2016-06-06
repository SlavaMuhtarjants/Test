create 
 --drop
 table dbo.MessageOffer
(
 Id uniqueidentifier not null rowguidcol
 
 constraint pk_MessageOffer
 primary key
 constraint df_MessageOffer_pk
    default newsequentialid()
 
 constraint fk_MessageOffer_pk
 foreign key 
 references dbo.Message(Id)
 ,

 --We can start to chat when want to change Rate or we need to setup the same reate. Correct?
 Rate money not null,
 
 --What about timezones?
 --they may change dates also
 --StartRent, EndRent are better
 RentSince datetimeoffset,
 RentTill datetimeoffset,

 StatusId uniqueidentifier not null --We need some default Status and mwthod how we can get it.
 constraint fk_MessageOffer_StatusId
 foreign key
 references dbo.MessageStatus(Id),
)
go

insert
  into dbo.MessageOffer(Id, Rate, RentSince, RentTill, StatusId)
select Id, Rate, RentSince, RentTill, StatusId
  from dbo.MessageOffer
 where Rate > 0;

alter table dbo.Chat
  add CurrentOfferId uniqueidentifier 
  constraint fk_Message_CurrentOfferId 
  foreign key
  references dbo.MessageOffer(Id)
go

--We need to change indexes


drop 
 index idx_Message_SenderId_SentDate_incl 
    on dbo.Message
go

drop 
 index idx_Message_ChatId_incl
    on dbo.Message
go


alter table dbo.Message
 drop column Rate
go

alter table dbo.Message
 drop column RentSince
go

alter table dbo.Message
 drop column RentTill
go


alter table dbo.Message
 drop constraint fk_Message_StatusId
go

alter table dbo.Message
 drop column StatusId
go


create 
 index idx_Message_SenderId_SentDate_incl
    on dbo.Message(SenderId, SentDate)
       include (Id, ChatId, /*Rate,*/ ReceivedDate, Message, /*RentSince, RentTill, StatusId,*/ DeliveredStatusId/*, RecepientId*/)
go

create
-- drop 
 index idx_Message_ChatId_incl
    on dbo.Message(ChatId)
       include (Id, /*Rate,*/ ReceivedDate, SentDate, Message, /*RentSince, RentTill, StatusId,*/ SenderId, DeliveredStatusId/*, RecepientId*/)
go

  drop 
 index idx_Chat_AdSpaceId_incl
    on dbo.Chat
go

  drop 
 index idx_Chat_UserId_incl
    on dbo.Chat
go

create 
 index idx_Chat_AdSpaceId_incl
    on dbo.Chat(AdSpaceId)
       include (Id, UserId, LastMessageId, CurrentOfferId)
go

create 
 index idx_Chat_UserId_incl
    on dbo.Chat(UserId)
       include (Id, AdSpaceId, LastMessageId, CurrentOfferId)
go

exec sp_rename 'dbo.MessageStatus', 'MessageOfferStatus';
go

exec sp_rename N'dbo.MessageOfferStatus.idx_MessageStatus_Synonym_incl', N'idx_MessageOfferStatus_Synonym_incl', N'INDEX';
go

exec sp_rename 'dbo.pk_MessageStatus', 'pk_MessageOfferStatus';
go

exec sp_rename 'dbo.df_MessageStatus_pk', 'df_MessageOfferStatus_pk';
go
  
-- Author:		Nikita Pochechuev
-- Create date: 2016-02-02

-- Description: This triger updates countes (for now unreaded messages for user) 
-- in [dbo].[ChatMember] after changes in [dbo].[Message]
-- Right now it has the simplest logic because we don't have separated status for each User-message 
-- May be we will define messages that were read by data as workaroud in the future
-- =============================================
alter trigger dbo.trMessageCounters
    on [dbo].[Message]
 after insert, delete, update --right now we recompute after ALL events
AS 
BEGIN 
 set nocount on;
 print 'trMessageCounters was started';

 if not(update(DeliveredStatusId))
 begin
  print 'trMessageCounters datachanges were ignored';
  return;
 end;

--Unortunarly we use ORM and don't create functions
--this function was created as "fast version"
 

 with cteChangedRows as
 (
 select distinct --actually this operator is redundand, it decrease perfromance but it may be usefill if someone do big direct update/isert for this table 
        coalesce(i.ChatId, d.ChatId) changedChatID, 
        coalesce(i.SenderId, d.SenderId) SenderID --we need to exclude sender
   from ns.fnMessageDeliveredStatus() ms

  inner
   join inserted i
   full 
   join deleted d
     on d.Id = i.Id
     
	 on  (   
	         ms.WasRead <> d.DeliveredStatusId 
	      or ms.WasRead <> i.DeliveredStatusId
		 ) --we need to compute and recompute messages that were not read.
	and (   
	        d.DeliveredStatusId <> i.DeliveredStatusId 
	     or d.DeliveredStatusId is null --inserted messages
		 or i.DeliveredStatusId is null --deleted messages 
		)   
 ),

 cteComputeMessages as
 (
 
 select --This logic will work for two users ONLY
        --when 
        chR.changedChatID,
        chR.SenderID,
		count(1) as cnt
   from dbo.Message msg with(nolock)
  cross
   join ns.fnMessageDeliveredStatus() ms --may be better way is to seletet value to variable because we use it more than once.

  inner
   join cteChangedRows chR
     on chR.changedChatID = msg.ChatId
	and chR.SenderID <> msg.SenderId --we exclude senders
	and msg.DeliveredStatusId <> ms.WasRead
   
  group
     by chR.changedChatID, chR.SenderID

 ),

 --it includes owner of Chat (Chat.UserId)
 --and owner of Space (Chat.AdSpace > AdSpace.SpaceId > Space.UserID
 cteComputedWithUser as
 (
 select cm.changedChatID,
        coalesce(s.UserId, ch.UserId) as UserId,  --we need one ID only!!!
        cm.cnt
   from cteComputeMessages cm
  inner
   join dbo.Chat ch with(nolock)
     on ch.Id = cm.changedChatID
   left
   join dbo.AdSpace adS with(nolock)
   left
   join dbo.Space s with(nolock)
     on s.Id = adS.SpaceId
     on adS.Id = ch.AdSpaceId
	and ch.UserId = cm.changedChatID --We need to check level up if users were the same on this level
 )

 --User + Chat will be inserted when the is the result in agreaget function ONLY
 merge [dbo].[ChatMember] as target
 using cteComputedWithUser as source
    on target.ChatId = source.changedChatID
   and target.UserId = source.UserId
 
  when matched 
  then update set target.AmountUnreadMessages = source.cnt
 
  when not matched
  then insert (ChatId, UserId, AmountUnreadMessages)
       values (source.changedChatID, source.UserId, source.cnt);

 print 'trMessageCounters was finished';

END
GO