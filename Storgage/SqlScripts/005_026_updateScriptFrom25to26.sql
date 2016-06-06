
alter table dbo.Space
  add createdDate datetimeoffset(7) NOT NULL
      constraint  df_Space_createdDate
	  default (todatetimeoffset(getutcdate(),'+00:00')),
	  Rate money, --NOT NULL,
	  RentTill date,
	  RentSince date
go

--select *
update s
   set s.Rate = adS.Rate
  from dbo.Space s
 inner
  join dbo.AdSpace adS
    on s.Id = adS.SpaceId
go

update s
   set s.Rate = 0
  from dbo.Space s
 where s.Rate is null
go


alter table dbo.Space
 alter column Rate money not null
go

  drop 
 index idx_Space_UserId_IsDeleted_incl
   on [dbo].[Space]
go

create
 index idx_Space_UserId_IsDeleted_incl
   on [dbo].[Space] (UserId, IsDeleted)      
	  include  (Id, SizeTypeId, SpaceAccessTypeId, Title, Decription, IsListed, SpaceTypeId, Location, DefaultPhotoID, createdDate, Rate, RentTill, RentSince) 
go

alter table dbo.Chat
  add SpaceId uniqueidentifier --will be "not null" and will have constraint FK
go

--select *
update c 
   set c.SpaceId = adS.SpaceId
  from dbo.AdSpace adS   
 inner
  join dbo.Chat c
    on c.[AdSpaceId] = adS.Id
go

alter table dbo.Chat
 alter column SpaceId uniqueidentifier not null
go

alter table dbo.Chat
 add constraint fk_Chat_SpaceId
 foreign key (SpaceId)
 references dbo.Space(Id)
go

  drop
 index idx_Chat_AdSpaceId_incl
    on dbo.Chat
go

create
 index idx_Chat_SpaceId_incl
    on [dbo].[Chat] ([SpaceId])
       include (Id, UserId, LastMessageId, CurrentOfferId)
go

  drop
 index idx_Chat_UserId_incl 
    on [dbo].[Chat]
go

create 
 index idx_Chat_UserId_incl 
    on dbo.Chat(UserId)
       include (Id, LastMessageId, CurrentOfferId, SpaceId)
go

alter table dbo.Chat
 drop constraint fk_Chat_AdSpaceId
go

alter table dbo.Chat
 drop column AdSpaceId
go

drop table [dbo].[AdSpace]
go

