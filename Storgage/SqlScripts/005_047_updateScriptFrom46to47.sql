/*
Author: Nikita Pochechuev

Description:
Function prevents hard-code and copy-past for current date 

Test case:
select dbo.fnGetUtc0DateWithOffset() 
*/
create function dbo.fnGetUtc0DateWithOffset() 
returns datetimeoffset 
as
begin
 return todatetimeoffset(getutcdate(),'+00:00');
end;
go

/*
Autor: Nikita Pochechuev

Description: This trigger sets value dbo.Chat.ApprovedMessageOfferHistoryId
after insert

--update 2016-04-13
Trigger sets Space.IsListed = false, when offer status is changed to Approved.

May be we NEED after INSERT ONLY, it may imporve performance 

update... Alexand Poploukhin said that we DON'T UPDATE this table, it means that trigger will faster
*/
create trigger [dbo].[tr_MessageOfferHistory_setApproved]
    on [dbo].[MessageOfferHistory]
	after insert --, update
as
begin
  
  print 'Start trigger tr_MessageOfferHistory_setApproved';

  --here I will store the list of spaces that should be changed to Space.IsListed = false
  --it prevents copy-past query and redundant searching, but I am not 100% sure that it's better
  declare @tSpace table (SpaceId uniqueidentifier);

update ch
   set ch.ApprovedMessageOfferHistoryId = moh.Id
output inserted.LastMessageOfferId
  into @tSpace
  from inserted moh
 inner
  join ns.fnMessageOfferStatus() mos
    on mos.Approved = moh.StatusId
 inner
  join dbo.MessageOffer mo with(nolock)
    on mo.Id = moh.MessageOfferId
 inner
  join dbo.Message m with(nolock)
    on m.Id = mo.Id
 inner
  join dbo.Chat ch
    on m.ChatId = ch.Id;


  --Set Space.IsListed = false, when offer status is changed to Approved.

update s
   set s.IsListed = 0
  from dbo.Space s
 inner
  join @tSpace su
	on su.SpaceId = s.Id
 where s.IsListed = 1;

  print 'Finish trigger tr_MessageOfferHistory_setApproved';

end;
go

drop trigger tr_MessageOfferHistory_setApprovedMessageOfferHistoryId
go

/*
Autor: Nikita Pochechuev

Description: This trigger sets values 

according to: https://weezlabs.atlassian.net/browse/STOR-574

2. Set Space.IsListed = true, when offer status is changed to Stopped.
3. Return MessageOffer.StopAt + 2 weeks for Spaces which are displayed to tenants

*/
create trigger [dbo].[tr_MessageOfferHistory_setStopped]
    on [dbo].[MessageOfferHistory]
	after insert --, update
as
begin
  
  print 'Start trigger tr_MessageOfferHistory_setStopped';

  --here I will store the list of spaces that should be changed to Space.IsListed = false
  --it prevents copy-past query and redundant searching, but I am not 100% sure that it's better
  declare @tMessageOffer table (Id uniqueidentifier);
  
  declare @StopAt datetimeoffset = dateadd(day, 14, dbo.fnGetUtc0DateWithOffset());

update mo
   set mo.StopAt = @StopAt
output inserted.Id
  into @tMessageOffer
  from inserted moh
 inner
  join ns.fnMessageOfferStatus() mos
    on mos.Stopped = moh.StatusId
 inner
  join dbo.MessageOffer mo with(nolock)
    on mo.Id = moh.MessageOfferId;
  --Set Space.IsListed = false, when offer status is changed to Approved.

update s
   set s.IsListed = 1
  from @tMessageOffer mo_i
 inner
  join dbo.MessageOffer mo with(nolock)
    on mo.Id = mo_i.Id
 inner
  join dbo.Message m with(nolock)
    on m.Id = mo.Id
 inner
  join dbo.Chat ch with(nolock)
    on ch.Id = m.ChatId
 inner
  join dbo.Space s with(nolock)
    on s.Id = ch.SpaceId
   and s.IsListed = 0

  print 'Finish trigger tr_MessageOfferHistory_setStopped';

end;
go