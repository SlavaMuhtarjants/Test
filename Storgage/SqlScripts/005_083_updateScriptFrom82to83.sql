alter table dbo.Chat 
  add LastModified datetimeoffset not null
  constraint df_Chat_LastModified 
  default dbo.fnGetUtc0DateWithOffset()
go

with cte as
(
select m.ChatId,
       max(m.SentDate) as LastModified
  from dbo.Message m with(nolock)
 group
    by m.ChatId
)

--select *
update ch 
   set ch.LastModified = c.LastModified
  from dbo.Chat ch
 inner
  join cte c
    on c.ChatId = ch.Id
go

with cte as
(
select m.ChatId,
       max(moh.ChangedStatusDate) as LastModified
  from dbo.Message m with(nolock)
 inner
  join dbo.Chat ch
    on ch.Id = m.ChatId
 inner
  join dbo.MessageOfferHistory moh with(nolock)
    on m.Id = moh.MessageOfferId
   and moh.ChangedStatusDate >= ch.LastModified
 group
    by m.ChatId
)

--select *
update ch 
   set ch.LastModified = c.LastModified
  from dbo.Chat ch
 inner
  join cte c
    on c.ChatId = ch.Id
go

drop INDEX [idx_Chat_SpaceId_incl] ON [dbo].[Chat]
go

CREATE INDEX [idx_Chat_SpaceId_incl] ON [dbo].[Chat]
(
	[SpaceId] ASC
)
INCLUDE (Id, UserId, ApprovedMessageOfferHistoryId, LastMessageOfferId, LastModified) 
go

--!!!
--I may replace the [idx_Chat_UserId_ApprovedMessageOfferHistoryId_incl]
--but I create new index for now because it will be used for paging
--!!!

create
 --drop
 index idx_Chat_UserId_LastModified_desc
    on [dbo].[Chat] (UserId, LastModified desc)
       include (Id, SpaceId, ApprovedMessageOfferHistoryId, LastMessageOfferId)
go

-- =============================================
-- Author: Nikita Pochechuev
-- Description:	Trigger updates dbo.Chat.LastModified when we insert new message
-- =============================================
create trigger tr_Message_Chat_LastModified
    on dbo.Message
	   after insert
as
begin
 set nocount on;
 print 'Start tr_Message_Chat_LastModified';
 
 declare 
  @updateDate datetimeoffset = dbo.fnGetUtc0DateWithOffset();
 
 with cte as
 (
 select distinct 
        i.ChatId
   from inserted i
 )
 update ch
    set ch.LastModified = @updateDate
   from cte c
  inner
   join dbo.Chat ch
     on c.ChatId = ch.Id;

 print 'Finish tr_Message_Chat_LastModified';

end
go

-- =============================================
-- Author: Nikita Pochechuev
-- Description:	Trigger updates dbo.Chat.LastModified when we insert new message
-- =============================================
create trigger tr_MessageOfferHystory_Chat_LastModified
    on dbo.MessageOfferHistory
	   after insert
as
begin
 set nocount on;
 print 'Start tr_MessageOfferHistory_Chat_LastModified';
 
 declare 
  @updateDate datetimeoffset = dbo.fnGetUtc0DateWithOffset();
 
 with cte as
 (
 select distinct 
        m.ChatId
   from inserted i
  inner
   join dbo.Message m with(nolock)
     on m.Id = i.MessageOfferId
 )
 update ch
    set ch.LastModified = @updateDate
   from cte c
  inner
   join dbo.Chat ch
     on c.ChatId = ch.Id;

 print 'Finish tr_MessageOfferHistory_Chat_LastModified';

end
go
