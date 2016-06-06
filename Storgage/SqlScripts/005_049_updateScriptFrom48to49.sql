alter table [dbo].[Space]
  add AvailableSince datetimeoffset null
go

drop INDEX [idx_Space_UserId_IsDeleted_incl] ON [dbo].[Space]
go


CREATE INDEX [idx_Space_UserId_IsDeleted_IsListed_incl] ON [dbo].[Space]
(
	[UserId] ASC, --May be wee need index without this field here or with it in the end of list because we will seach Spaces without user check
	[IsDeleted] ASC,
	IsListed
)
INCLUDE ( 	[Id],
	[SizeTypeId],
	[SpaceAccessTypeId],
	[Title],
	[Decription],
	--[IsListed],
	[SpaceTypeId],
	[Location],
	[DefaultPhotoID],
	[createdDate],
	[Rate],
	[FullAddress],
	[ShortAddress],
	[ZipId],
	AvailableSince) 
go

/*
Autor: Nikita Pochechuev

Description: This trigger sets values 

according to: https://weezlabs.atlassian.net/browse/STOR-574

2. Set Space.IsListed = true, when offer status is changed to Stopped.
3. Return MessageOffer.StopAt + 2 weeks for Spaces which are displayed to tenants

*/
alter trigger [dbo].[tr_MessageOfferHistory_setStopped]
    on [dbo].[MessageOfferHistory]
	after insert --, update
as
begin
  
  print 'Start trigger tr_MessageOfferHistory_setStopped';
  
  --declare @StopAt datetimeoffset = dateadd(day, 14, dbo.fnGetUtc0DateWithOffset());

update s
   set s.AvailableSince = dateadd(day, 14, mo.StopAt), --I am not sure that this date should disable possible contract if less amount of days has been passed
       s.IsListed = 0
  from inserted moh
 inner
  join ns.fnMessageOfferStatus() mos
    on mos.Stopped = moh.StatusId
 inner
  join dbo.MessageOffer mo with(nolock) --we need date from this table and we can't join dbo.Message directly
    on mo.Id = moh.MessageOfferId
 inner
  join dbo.Message m with(nolock)
    on m.Id = mo.Id
 inner
  join dbo.Chat ch with(nolock)
    on ch.Id = m.ChatId
 inner
  join dbo.Space s --with(nolock)
    on s.Id = ch.SpaceId
   and s.IsListed = 0

  print 'Finish trigger tr_MessageOfferHistory_setStopped';

end;
go

--Update script for existing data

/*select s.*,
       currentMoh.*,
	   dateadd(day, 14, currentMoh.StopAt)*/
update s 
   set s.AvailableSince = dateadd(day, 14, currentMoh.StopAt),
       s.IsListed = 1
  from dbo.Space s
 outer
 apply
 (
   select top 1
          moh.*,
		  mo.StopAt 
     from dbo.Chat ch
	inner
	 join dbo.MessageOffer mo
	   on ch.LastMessageOfferId = mo.Id
	inner
	 join dbo.MessageOfferHistory moh
	   on mo.CurretMessageOfferHistory = moh.Id
   where ch.SpaceId = s.Id
   order by moh.ChangedStatusDate desc
 ) currentMoh

 inner
  join ns.fnMessageOfferStatus() mos
    on mos.Stopped = currentMoh.StatusId
go