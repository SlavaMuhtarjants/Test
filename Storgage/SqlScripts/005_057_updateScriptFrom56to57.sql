/*
Author: Nikta Pochechuev

Description:
Procedure fixes value MessageOffer.CurretMessageOfferHistory.
It may be usefull after update scripts

Actually it's possible to re-write trigger to work "after delete" or etc
OR create new trigger with THIS logic

but I think this aproach is better

--Test case 
begin tran
 exec misc.spMessageOffer_setCurretMessageOfferHistory
rollback tran;

*/
create procedure [misc].[spMessageOffer_setCurretMessageOfferHistory]
as
begin

--Set current versions according to order logic
update mo 
   set mo.CurretMessageOfferHistory = current_moh.Id
  from dbo.MessageOffer mo
 cross
 apply (select top 1 moh.Id
          from dbo.MessageOfferHistory moh
		 where moh.MessageOfferId = mo.Id
		 order 
		    by moh.ChangedStatusDate desc
		) current_moh
 where mo.CurretMessageOfferHistory <> current_moh.Id 
    or mo.CurretMessageOfferHistory is null and current_moh.Id is not null;

 --Extended functionality
 
 --If last status is Approved then we need make sure that Chat.ApprovedMessageOfferHistoryId is correct
 --I do it for ALL records for now, but it's possible to do it for updated on previous step records ONLY
 --in this case we need to use "output" statement to table variable as trigger does it

update ch 
   set ch.ApprovedMessageOfferHistoryId = current_moh.Id
  from dbo.Chat ch
 cross
 apply (select top 1 moh.Id
          from dbo.MessageOfferHistory moh		 
         inner
          join ns.fnMessageOfferStatus() mos
            on mos.Approved = moh.StatusId
		  inner
           join dbo.MessageOffer mo with(nolock)
             on mo.Id = moh.MessageOfferId --here we do it for CURRENT and APPROVED statuses only
          inner
           join dbo.Message m with(nolock)
             on m.Id = mo.Id
		    and m.ChatId = ch.Id
		 order 
		    by moh.ChangedStatusDate desc
		) current_moh
 where ch.ApprovedMessageOfferHistoryId <> current_moh.Id 
    or ch.ApprovedMessageOfferHistoryId is null and current_moh.Id is not null;

 
 --I disided to check [dbo].[Chat].[LastMessageOfferId]
 --for double sure

 update ch 
   set ch.[LastMessageOfferId] = current_mo.Id
  from dbo.Chat ch
 cross
 apply (select top 1 mo.Id
          from dbo.MessageOffer mo with(nolock)             
          inner
           join dbo.Message m with(nolock)
             on m.Id = mo.Id
		    and m.ChatId = ch.Id
		 order 
		    by m.SentDate desc
		) current_mo
 where ch.LastMessageOfferId <> current_mo.Id 
    or ch.LastMessageOfferId is null and current_mo.Id is not null;

update s 
   set s.IsListed = 0
  from dbo.Space s 
 cross
 apply (select top 1 moh.Id, moh.StatusId
          from dbo.MessageOfferHistory moh
		 inner
		  join dbo.MessageOffer mo with(nolock)             
		    on mo.CurretMessageOfferHistory = moh.Id 
         inner
          join dbo.Message m with(nolock)
            on m.Id = mo.Id 
		 inner
		  join dbo.Chat ch
		    on m.ChatId = ch.Id
		   and ch.SpaceId = s.Id
		 order 
		    by moh.ChangedStatusDate desc
		) current_moh
     
 inner 
  join ns.fnMessageOfferStatus() mos
    on mos.Approved = current_moh.StatusId 
   and s.IsListed = 1;

end;
go

 --We need to delete ALL expired MOH by task in any case

 --They may be used as Current
 --select *
 update mo
    set mo.CurretMessageOfferHistory = null
   from dbo.MessageOfferHistory moh with(nolock)
  inner
   join [ns].[fnMessageOfferStatus]() mos
     on mos.Expired = moh.StatusId
  inner
   join dbo.MessageOffer mo
     on mo.CurretMessageOfferHistory = moh.Id;

--Then delete MessageOfferHistory
delete
  from moh
  from dbo.MessageOfferHistory moh
  inner
   join [ns].[fnMessageOfferStatus]() mos
     on mos.Expired = moh.StatusId;

--Set the latest MessageOfferHistory as Current before for make sure that we will not delete incorrect value

exec misc.spMessageOffer_setCurretMessageOfferHistory;

--Remove duplicated MessageOfferId + StatusId
--Last values will be left
;
with cte as 
(
select MessageOfferId,
       StatusId,
	   count(1) as cnt,
	   max(Id) as maxId
  from dbo.MessageOfferHistory moh
 group 
    by MessageOfferId,
       StatusId
having count(1) > 1
)

delete
  from moh
  from dbo.MessageOfferHistory moh
 inner
  join cte c
    on c.MessageOfferId = moh.MessageOfferId
   and c.StatusId = moh.StatusId
   and c.maxId <> moh.Id;

--It's stange but this table DID'T HAVE ANY INDEX !!!
create
unique
 index idx_MessageOfferHistory_MessageOfferId_StatusId
    on [dbo].[MessageOfferHistory] ([MessageOfferId], [StatusId])
       include (Id, UserId, ChangedStatusDate)
go
