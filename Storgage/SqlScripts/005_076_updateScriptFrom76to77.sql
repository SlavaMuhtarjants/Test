
CREATE NONCLUSTERED INDEX [idx_Space_IsDeleted_IsListed] ON [dbo].[Space] 
(
	[IsListed],
	[IsDeleted]
)
INCLUDE 
(	[Id],
	[SizeTypeId],
	[SpaceAccessTypeId],
	[UserId],
	[Title],
	[Decription],
	[SpaceTypeId],
	[Location],
	[DefaultPhotoID],
	[createdDate],
	[Rate],
	[FullAddress],
	[ShortAddress],
	[ZipId],
	[AvailableSince],
	[Latitude],
	[Longitude],
	[LastModified]
)
GO