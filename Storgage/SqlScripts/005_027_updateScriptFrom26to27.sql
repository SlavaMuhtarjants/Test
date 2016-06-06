  drop 
 index idx_Space_UserId_IsDeleted_incl
   on [dbo].[Space]
go

alter table dbo.Space
  drop column RentTill
go

alter table dbo.Space
  drop column RentSince
go

create
 index idx_Space_UserId_IsDeleted_incl
   on [dbo].[Space] (UserId, IsDeleted)      
	  include  (Id, SizeTypeId, SpaceAccessTypeId, Title, Decription, IsListed, SpaceTypeId, Location, DefaultPhotoID, createdDate, Rate) 
go