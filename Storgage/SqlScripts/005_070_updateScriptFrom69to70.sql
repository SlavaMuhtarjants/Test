-- Author:		Nikita Pochechuev
-- Create date: 2016-02-02

-- Description: This triger updates countes (for now unreaded messages for user) 
-- in [dbo].[ChatMember] after changes in [dbo].[Message]
-- Right now it has the simplest logic because we don't have separated status for each User-message 
-- May be we will define messages that were read by data as workaroud in the future
-- =============================================
/*
begin tran 

declare @testChatMember table (ChatMemberId uniqueidentifier, oldValue int)

insert
  into @testChatMember

select distinct
       chM.Id,
	   chM.AmountUnreadMessages
  from dbo.Message msg with(nolock)
 inner
  join ns.fnMessageDeliveredStatus() ms
    on msg.DeliveredStatusId = ms.WasNotSent
 inner
  join dbo.ChatMember chM
    on chM.ChatId = msg.ChatID
   and chM.UserId <> msg.UserId;
   
--select *
--  from dbo.ChatMember chM
-- inner
--  join @testChatMember t
--    on t.ChatMemberid = chM.Id
-- order by chM.Id;
 
--select * 
update msg
   set msg.DeliveredStatusId = ms.WasRead
  from dbo.Message msg with(nolock)
 inner
  join ns.fnMessageDeliveredStatus() ms
    on msg.DeliveredStatusId = ms.WasNotSent

select *
  from dbo.ChatMember chM
 inner
  join @testChatMember t
    on t.ChatMemberid = chM.Id
 order by chM.Id;

rollback tran;


*/
alter trigger [dbo].[trMessageCounters]
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
        coalesce(i.UserId, d.UserId) SenderID --we need to exclude sender
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
 )
 
 --select * from cteChangedRows
 ,

 cteComputeMessages as
 (
 
 select --This logic will work for two users ONLY
        --when 
        chR.changedChatID,
        cm.UserId as UserId,
		count(msg.ChatId) as cnt
   from cteChangedRows chR
  cross
   join ns.fnMessageDeliveredStatus() ms --may be better way is to seletet value to variable because we use it more than once.

  inner
   join dbo.Chat ch with(nolock)  
     on ch.Id = chR.changedChatID   
   
   left
   join dbo.Space s with(nolock)
     on s.Id = ch.SpaceId

  /*
  inner
   join dbo.ChatMember cm
     on cm.ChatId = chR.changedChatID
	and cm.UserId <> chR.SenderID
  */
   
  cross
  apply (select ch.UserId as UserId
         union all
		 select s.UserId 
		) cm --We need to simulate ChatMember table for this chat because it may doesn't exits 
		     --Also it may be done by separated insertion before computing

   left
   join dbo.Message msg with(nolock)
     on chR.changedChatID = msg.ChatId
	and msg.UserID <> cm.UserId --we exclude senders
    and msg.DeliveredStatusId <> ms.WasRead
   
  group
     by chR.changedChatID, cm.UserId

 )
 --select * from cteComputeMessages

 --User + Chat will be inserted when the is the result in agreaget function ONLY
 merge [dbo].[ChatMember] as target
 using cteComputeMessages as source
    on target.ChatId = source.changedChatID
   and target.UserId = source.UserId
 
  when matched 
  then update set target.AmountUnreadMessages = source.cnt
 
  when not matched
  then insert (ChatId, UserId, AmountUnreadMessages)
       values (source.changedChatID, source.UserId, source.cnt);
    
 print 'trMessageCounters was finished';

END
