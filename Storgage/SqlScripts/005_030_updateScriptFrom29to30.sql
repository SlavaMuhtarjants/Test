alter table dbo.Space
  add FullAddress nvarchar(100),
      ShortAddress nvarchar(100)
go

alter table dbo.Space
  add ZipId uniqueidentifier --not null
go

--select s.*
update s
   set s.ZipId = sa.ZipId
  from dbo.Space s
 inner
  join dbo.SpaceAddress sa
    on sa.Id = s.Id
go

;
with cte as
(select top 1 Id from dbo.Zip)
--select c.*, s.*
update s
   set s.ZipId = c.Id
  from dbo.Space s
 cross
  join cte c
where ZipId is null
go

alter table dbo.Space
 alter column ZipId uniqueidentifier --not null
go

alter table dbo.Space
  add constraint fk_Space_ZipId
  foreign key (ZipId)
  references dbo.Zip(Id)
go

drop table [dbo].[SpaceAddress]
go

drop index [idx_Space_UserId_IsDeleted_incl] 
    on dbo.Space 
go

create index [idx_Space_UserId_IsDeleted_incl] 
    on dbo.Space (UserId, IsDeleted)
        include (Id, SizeTypeId, SpaceAccessTypeId, Title, Decription, IsListed, SpaceTypeId, Location, DefaultPhotoID, createdDate, Rate, FullAddress, ShortAddress, ZipId)
go

