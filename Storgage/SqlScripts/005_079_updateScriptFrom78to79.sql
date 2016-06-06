/*
Autor: Nikita Pochechuev

Description: This trigger sets value dbo.Chat.ApprovedMessageOfferHistoryId
after insert

--update 2016-04-13
Trigger sets Space.IsListed = false, when offer status is changed to Approved.

May be we NEED after INSERT ONLY, it may imporve performance 

update... Alexand Poploukhin said that we DON'T UPDATE this table, it means that trigger will faster
*/
ALTER trigger [dbo].[tr_MessageOfferHistory_setApproved]
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
output inserted.SpaceId
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