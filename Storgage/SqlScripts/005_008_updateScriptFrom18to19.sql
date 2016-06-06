create table ChatMember 
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_ChatMember 
 primary key
 constraint df_ChatMember_pk
    default newsequentialid(),
 
 ChatId uniqueidentifier not null
 constraint fk_ChatMember_ChatId 
 foreign key
 references [dbo].[Chat] (Id),

 UserId uniqueidentifier not null
 constraint fk_ChatMember_UserId
 foreign key
 references [dbo].[User] (Id),

 AmountUnreadMessages int not null 
 constraint df_ChatMember_AmountUnreadMessages
 default (0)
)
go

/*
  alter table ChatMember 
   with nocheck
    add constraint  uk_ChatMember_ChatId_UserId
  unique (ChatId, UserId)    
go
*/
/*
I don't create unique constriant, I create UNIQUE index because it has INCLUDE part
I am not usre about GOOD order, we can query data by ChatId or by UserId only and I don't know what is more important.
May be we will create additional index in future or change this index
*/
 create
 unique
   --drop
  index idx_ChatMember_ChatId_UserId
     on ChatMember (ChatId, UserId)
include (Id, AmountUnreadMessages) --all fields should be included.
go

/*
--We will use new table
insert
  into MessageStatus (Id, Title, Description, Synonym)
values ('B8FEEE09-2709-421A-9111-AC18B2A1B59A', 'Unread',  'Message was not readed by 2nd (all) other(s) user in chat', 'UNREAD')
go
*/

--Was renamed from "Statuses"
create 
 --drop
 table dbo.MessageDeliveredStatus
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_MessageDeliveredStatus
 primary key
 constraint df_MessageDeliveredStatus_pk
    default newsequentialid(),

 Title nvarchar(20) not null,
 Description nvarchar(50) not null,

 Synonym varchar(255)
)
go

create
unique
 index idx_MessageDeliveredStatus_Synonym_incl
    on dbo.MessageDeliveredStatus(Synonym) 
	   include (Id, Title, Description)
 where Synonym is not null
go

/*
delete from dbo.MessageDeliveredStatus
go
*/
insert
  into dbo.MessageDeliveredStatus (Id, Title, Description, Synonym)
select 'B8FEEE09-2709-421A-9111-AC18B2A1B59A', 'Sent',  'Message was sent to 2nd (all) other(s) user(s)', 'SENT'
union all
select '6C807B96-1B4C-43A6-9BEF-0F46C0247423', 'Was read',  'User has read message', 'WAS_READ'
union all
select '6E84260A-E5F8-4880-9DE1-ED0945E90BEB', 'Not Sent',  'User has read message', 'NOT_SENT'
go

select * from dbo.MessageDeliveredStatus;
go

--This schema will be used like name spaces 
--I did it at first just for tests and investigations because I don't want to hard-code synonyms for example
--and
create schema ns;
go

--Author: Nikita Pochechuev
--Description: Fuction was created for trMessageCounters
--and it's the simplest version 

/*
--Test case
select dbo.fnMessageDeliveredStatusGetId('WAS_READ')
*/

create
 --drop
 function dbo.fnMessageDeliveredStatusGetId(@Synonym varchar(255)) 
returns uniqueidentifier
begin
 
 return (
 select id
   from [dbo].MessageDeliveredStatus mds
  where mds.Synonym = @Synonym)

end;
go

-- =============================================
-- Author:		Nikita Pochechuev
-- Create date: 2016-02-02
-- Description:	It's test fucntions that will work like "Enum" in C# code and map statuses to table
-- =============================================

/*
-- Test case
select * from ns.fnMessageDeliveredStatus()
*/
create 
--alter
function ns.fnMessageDeliveredStatus
(	
)
returns table
as
return
(
 select dbo.fnMessageDeliveredStatusGetId('WAS_READ') as WasRead,
        dbo.fnMessageDeliveredStatusGetId('SENT') as WasSent,
		dbo.fnMessageDeliveredStatusGetId('NOT_SENT') as WasNotSent
)
GO

alter table dbo.Message
  add DeliveredStatusId uniqueidentifier not null --We need some default Status and mwthod how we can get it.
 constraint fk_Message_DeliveredStatusId
 foreign key
 references dbo.MessageDeliveredStatus(Id)
go

--indexes must be recreated beacuse we add new field

  drop 
 index idx_Message_ChatId_incl
    on dbo.Message
go

create
-- drop 
 index idx_Message_ChatId_incl
    on dbo.Message(ChatId)
       include (Id, Rate, ReceivedDate, SentDate, Message, RentSince, RentTill, StatusId, SenderId, DeliveredStatusId/*, RecepientId*/)
go

  drop
 index idx_Message_SenderId_SentDate_incl
    on dbo.Message
go

create 
 index idx_Message_SenderId_SentDate_incl
    on dbo.Message(SenderId, SentDate)
       include (Id, ChatId, Rate, ReceivedDate, Message, RentSince, RentTill, StatusId, DeliveredStatusId/*, RecepientId*/)
go

-- Author:		Nikita Pochechuev
-- Create date: 2016-02-02

-- Description: This triger updates countes (for now unreaded messages for user) 
-- in [dbo].[ChatMember] after changes in [dbo].[Message]
-- Right now it has the simplest logic because we don't have separated status for each User-message 
-- May be we will define messages that were read by data as workaroud in the future
-- =============================================
create 
 --alter
 trigger dbo.trMessageCounters
    on [dbo].[Message]
 after insert, delete, update --right now we recompute after ALL events
AS 
BEGIN 
 set nocount on;
 print 'trMessageCounters was started';

 if not(update([StatusId]))
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


--Triger trMessageCounters test case
--select newID()
begin tran

insert
  into [dbo].[Chat] (Id, AdSpaceId, UserId)
values ('687EDD28-E24E-4989-A6DF-2DF69277D4A9', '69B3821B-1A12-44F8-BE8C-4182D85035E0', '74D91545-4389-43CD-B709-0AB71E752229')

--select * from [dbo].[Chat]

insert 
  into [dbo].[MessageStatus] (Id, Title, Description, Synonym)
 values ('189B96E7-91CC-4AC3-8A67-B995D2D951DB', '', '', '')

insert
  into [dbo].[Message]
  (ChatId, Rate, ReceivedDate, SentDate, Message, RentSince, RentTill, StatusId, SenderId, DeliveredStatusId)
  values ('687EDD28-E24E-4989-A6DF-2DF69277D4A9', 10, getdate(), getdate(), 'Test trigger', getdate(), getdate(), '189B96E7-91CC-4AC3-8A67-B995D2D951DB', 'B99B1E6B-5F99-49D3-A886-09201449AD9D', 'B8FEEE09-2709-421A-9111-AC18B2A1B59A')

insert
  into [dbo].[Message]
  (ChatId, Rate, ReceivedDate, SentDate, Message, RentSince, RentTill, StatusId, SenderId, DeliveredStatusId)
  values ('687EDD28-E24E-4989-A6DF-2DF69277D4A9', 15, getdate(), getdate(), 'Test trigger 2', getdate(), getdate(), '189B96E7-91CC-4AC3-8A67-B995D2D951DB', 'F38834FC-3A19-40FB-98E1-230C87DE669E', 'B8FEEE09-2709-421A-9111-AC18B2A1B59A')

insert
  into [dbo].[Message]
  (ChatId, Rate, ReceivedDate, SentDate, Message, RentSince, RentTill, StatusId, SenderId, DeliveredStatusId)
  values ('687EDD28-E24E-4989-A6DF-2DF69277D4A9', 5, getdate(), getdate(), 'Test trigger 3', getdate(), getdate(), '189B96E7-91CC-4AC3-8A67-B995D2D951DB', 'DFE41874-29DF-4136-B1C1-80ADA3093480', 'B8FEEE09-2709-421A-9111-AC18B2A1B59A')


insert
  into [dbo].[Message]
  (ChatId, Rate, ReceivedDate, SentDate, Message, RentSince, RentTill, StatusId, SenderId, DeliveredStatusId)
  values ('687EDD28-E24E-4989-A6DF-2DF69277D4A9', 15, getdate(), getdate(), 'Test trigger 4', getdate(), getdate(), '189B96E7-91CC-4AC3-8A67-B995D2D951DB', 'D206F6AE-BDD9-474A-AB22-7D802A84BEBE', 'B8FEEE09-2709-421A-9111-AC18B2A1B59A')



select * from [dbo].[Message]

select * from [dbo].[ChatMember]

rollback tran;
go