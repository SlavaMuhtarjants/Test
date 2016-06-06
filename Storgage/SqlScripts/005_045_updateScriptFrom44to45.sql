drop trigger [dbo].[tr_MessageOffer_setLastMessageOfferId]
go
/*
Autor: Nikita Pochechuev

Description: This trigger sets value dbo.Chat.LastMessageOfferId
after insert 

*/
create trigger [dbo].[tr_MessageOffer_setLastMessageOfferId]
    on [dbo].[MessageOffer]
	after insert --, update
as
begin
  
  print 'Start trigger tr_MessageOffer_setLastMessageOfferId';

update ch
   set ch.LastMessageOfferId = mo.Id
  from inserted mo
 inner
  join dbo.Message m with(nolock)
    on m.Id = mo.Id
 inner
  join dbo.Chat ch
    on m.ChatId = ch.Id;

  print 'Finish trigger tr_MessageOffer_setLastMessageOfferId';

end;
go

/*
Author: Nikita Pochechuev

Description: 
SP should be used for testing only when we need to delete chat for space and user 

I DON'T support this SP.
I will change it when I need this SP ONLY

--Test case
begin tran 
exec tst.spChatDeleteForSpaceAndUser
  @UserId = '5C7866A9-1138-4B28-8856-D50850205F42',
  @SpaceId = '97D3C1F7-BAE6-E511-811A-02B1C768461B';
rollback tran;

*/
create procedure tst.spChatDeleteForSpaceAndUser
(
@UserId uniqueidentifier,
@SpaceId uniqueidentifier
)
as
begin
 --select * from dbo.Chat
--'6e3cae1c-8900-e611-bec1-6c71d91fabb6'

--declare
-- @UserId uniqueidentifier = '5C7866A9-1138-4B28-8856-D50850205F42',
-- @SpaceId uniqueidentifier = '97D3C1F7-BAE6-E511-811A-02B1C768461B'


delete 
  from  chm
  from dbo.Chat ch
 inner
  join dbo.ChatMember chm
    on chm.ChatId = ch.Id   
where ch.UserId = @UserId
  and ch.SpaceId = @SpaceId

update mo
   set mo.CurretMessageOfferHistory = null
  from dbo.Chat ch
 inner
  join dbo.Message m
    on m.ChatId = ch.Id   
 inner
  join dbo.MessageOffer mo
    on m.id = mo.Id
 inner
  join dbo.MessageOfferHistory moh
    on moh.MessageOfferId = mo.Id
 
where ch.UserId = @UserId
  and ch.SpaceId = @SpaceId

delete 
  from  r
  from dbo.Chat ch
 inner
  join dbo.Message m
    on m.ChatId = ch.Id   
 inner
  join dbo.MessageOffer mo
    on m.id = mo.Id
 inner
  join dbo.MessageOfferHistory moh
    on moh.MessageOfferId = mo.Id
 inner
  join dbo.Rating r
    on r.Id = moh.Id
 
where ch.UserId = @UserId
  and ch.SpaceId = @SpaceId

delete 
  from  moh
  from dbo.Chat ch
 inner
  join dbo.Message m
    on m.ChatId = ch.Id   
 inner
  join dbo.MessageOffer mo
    on m.id = mo.Id
 inner
  join dbo.MessageOfferHistory moh
    on moh.MessageOfferId = mo.Id
 
where ch.UserId = @UserId
  and ch.SpaceId = @SpaceId

update ch
   set ch.LastMessageOfferId = null
  from dbo.Chat ch
 
where ch.UserId = @UserId
  and ch.SpaceId = @SpaceId

delete 
  from  mo
  from dbo.Chat ch
 inner
  join dbo.Message m
    on m.ChatId = ch.Id   
 inner
  join dbo.MessageOffer mo
    on m.id = mo.Id
 
where ch.UserId = @UserId
  and ch.SpaceId = @SpaceId

delete 
  from  m
  from dbo.Chat ch
 inner
  join dbo.Message m
    on m.ChatId = ch.Id   
where ch.UserId = @UserId
  and ch.SpaceId = @SpaceId

--select * from dbo.Chat ch
delete 
  from  ch
  from dbo.Chat ch
--where id = '6e3cae1c-8900-e611-bec1-6c71d91fabb6'
where ch.UserId = @UserId
  and ch.SpaceId = @SpaceId

end;
go

exec sp_rename 'dbo.tr_MessageOffer_CurretMessageOfferHistory', 'tr_MessageOfferHistory_CurretMessageOfferHistory';
go