alter table [dbo].[Space]
  add IsOccupied bit not null 
  constraint df_Space_IsOccupied 
  default (0)
go

drop index [idx_Space_LastModified_Latitude_Longitude_AvailableSince_Rate_IsListed_IsDeleted] ON [dbo].[Space]
go

CREATE INDEX [idx_Space_LastModified_Latitude_Longitude_AvailableSince_Rate_IsListed_IsDeleted] ON [dbo].[Space]
(
	[LastModified] ASC,
	[Latitude] ASC,
	[Longitude] ASC,
	[AvailableSince] ASC,
	[Rate] ASC,
	[IsListed] ASC,
	[IsDeleted] ASC
)
INCLUDE (Id, UserId, SizeTypeId, SpaceAccessTypeId, Title, Decription, SpaceTypeId, Location, DefaultPhotoID, createdDate, FullAddress, ShortAddress, ZipId, IsOccupied)


drop INDEX [idx_Space_UserId_IsDeleted_IsListed_incl] ON [dbo].[Space]
go

CREATE  INDEX [idx_Space_UserId_IsDeleted_IsListed_incl] ON [dbo].[Space]
(
	[UserId] ASC,
	[IsDeleted] ASC,
	[IsListed] ASC
)
INCLUDE (Id, SizeTypeId, SpaceAccessTypeId, Title, Decription, SpaceTypeId, Location, DefaultPhotoID, createdDate, Rate, FullAddress, ShortAddress, ZipId, AvailableSince, LastModified, Latitude, Longitude 	)

drop INDEX [idx_Space_ZipId_SizeTypeId_Rate_LastModified] ON [dbo].[Space]
go

CREATE INDEX [idx_Space_ZipId_SizeTypeId_Rate_LastModified] ON [dbo].[Space]
(
	[SizeTypeId] ASC,
	[ZipId] ASC,
	[Rate] ASC,
	[LastModified] DESC
)
INCLUDE (Id, UserId, SpaceAccessTypeId, Title, Decription, IsListed, SpaceTypeId, Location, DefaultPhotoID, IsDeleted, createdDate, FullAddress, ShortAddress, AvailableSince, Latitude, Longitude) 
go

/*
Autor: Nikita Pochechuev

Description: This trigger sets value dbo.Chat.ApprovedMessageOfferHistoryId
after insert

--update 2016-04-13
Trigger sets Space.IsListed = false, when offer status is changed to Approved.

May be we NEED after INSERT ONLY, it may imporve performance 

update... Alexand Poploukhin said that we DON'T UPDATE this table, it means that trigger will faster

begin tran

insert
  into MessageOfferHistory(MessageOfferId, UserId, StatusId)

select top 5
       --*
	   moh.MessageOfferId, moh.UserId, mos.Approved
  from dbo.MessageOffer mo with(nolock)
 inner
  join dbo.MessageOfferHistory moh with(nolock)
    on mo.CurretMessageOfferHistory = moh.Id
 inner
  join ns.fnMessageOfferStatus() mos
    on mos.Pending = moh.StatusId

rollback tran;

*/
ALTER trigger [dbo].[tr_MessageOfferHistory_setApproved]
    on [dbo].[MessageOfferHistory]
	after insert --, update
as
begin
  
  set nocount on;
  
  print 'Start trigger tr_MessageOfferHistory_setApproved';

  --here I will store the list of spaces that should be changed to Space.IsListed = false
  --it prevents copy-past query and redundant searching, but I am not 100% sure that it's better
  declare @tSpace table (SpaceId uniqueidentifier);

  --This logic is not "the fastets", but it prevents possible mistakes in the future by preventing redundant trigger calls
  declare @Chat 
    table (
	       ChatID uniqueidentifier,
		   ApprovedMessageOfferHistoryId uniqueidentifier
	      );

insert
  into @Chat (ChatId, ApprovedMessageOfferHistoryId)  
select m.ChatId,
       max(moh.Id) ApprovedMessageOfferHistoryId --as "Last"
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
 group
    by m.ChatId;

 if @@rowcount = 0
 begin
  print 'Finish trigger tr_MessageOfferHistory_setApproved without changes';
  return;
 end;

update ch
   set ch.ApprovedMessageOfferHistoryId = i.ApprovedMessageOfferHistoryId
output inserted.SpaceId
  into @tSpace
  from @Chat i
 inner
  join dbo.Chat ch
    on i.ChatId = ch.Id;


  --Set Space.IsListed = false, when offer status is changed to Approved.

update s
   set s.IsListed = 0,
       s.IsOccupied = 1 --This attribute can't be changed by User
  from dbo.Space s
 inner
  join @tSpace su
	on su.SpaceId = s.Id
 where s.IsListed = 1;

  print 'Finish trigger tr_MessageOfferHistory_setApproved';

end;
go

/*
Autor: Nikita Pochechuev

Description: This trigger sets values 

according to: https://weezlabs.atlassian.net/browse/STOR-574

2. Set Space.IsListed = true, when offer status is changed to Stopped.
3. Return MessageOffer.StopAt + 2 weeks for Spaces which are displayed to tenants


begin tran

insert
  into MessageOfferHistory(MessageOfferId, UserId, StatusId)

select top 5
       --*
	   moh.MessageOfferId, moh.UserId, mos.Stopped
  from dbo.MessageOffer mo with(nolock)
 inner
  join dbo.MessageOfferHistory moh with(nolock)
    on mo.CurretMessageOfferHistory = moh.Id
 inner
  join ns.fnMessageOfferStatus() mos
    on mos.Approved = moh.StatusId

rollback tran;

*/
ALTER trigger [dbo].[tr_MessageOfferHistory_setStopped]
    on [dbo].[MessageOfferHistory]
	after insert --, update
as
begin
  
  set nocount on;

  print 'Start trigger tr_MessageOfferHistory_setStopped';

  --here I will store the list of spaces that should be changed to Space.IsListed = false
  --it prevents copy-past query and redundant searching, but I am not 100% sure that it's better
  declare @tMessageOffer table (Id uniqueidentifier);
  
  declare @StopAt datetimeoffset = dbo.fnGetUtc0DateWithOffset();
  declare @AvailableSince datetimeoffset = dateadd(day, 14, @StopAt);

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

 if @@rowcount = 0
 begin
  print 'Finish trigger tr_MessageOfferHistory_setStopped without changes';
  return;
 end;

update s
   set s.IsListed = 1,
       --s.AvailableSince = dateadd(day, 14, mo.StopAt)
	   s.AvailableSince = @AvailableSince,
	   s.IsOccupied = 0
  from @tMessageOffer mo_i
 --inner
 -- join dbo.MessageOffer mo with(nolock)
 --   on mo.Id = mo_i.Id
 inner
  join dbo.Message m with(nolock)
    on m.Id = mo_i.Id
 inner
  join dbo.Chat ch with(nolock)
    on ch.Id = m.ChatId
 inner
  join dbo.Space s with(nolock)
    on s.Id = ch.SpaceId
   --and s.IsListed = 0 we update it in ANY case to "1" even if it's "1" because we need update 

  print 'Finish trigger tr_MessageOfferHistory_setStopped';

end;
go

