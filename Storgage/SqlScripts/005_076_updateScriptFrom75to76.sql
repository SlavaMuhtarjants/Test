-- Structure changes

DROP INDEX [idx_Filter_UserId] ON [dbo].[Filter]
DROP INDEX [idx_Space_ZipId_SizeTypeId_Rate_LastModified] ON [dbo].[Space]
DROP INDEX [idx_Space_UserId_IsDeleted_IsListed_incl] ON [dbo].[Space]

ALTER TABLE [Space] ALTER COLUMN Location GEOGRAPHY NOT NULL
ALTER TABLE [Space] ADD Latitude FLOAT(25) NULL
ALTER TABLE [Space] ADD Longitude FLOAT(25) NULL

ALTER TABLE [Filter] ADD TopLatitude FLOAT(25) NULL
ALTER TABLE [Filter] ADD BottomLatitude FLOAT(25) NULL
ALTER TABLE [Filter] ADD LeftLongitude FLOAT(25) NULL
ALTER TABLE [Filter] ADD RightLongitude FLOAT(25) NULL
GO

-- Data changes

update [Space] set
	Latitude = Location.Lat,
	Longitude = Location.Long

update Filter set
	TopLatitude = BoundingBox.STPointN(1).Lat,
	BottomLatitude = BoundingBox.STPointN(3).Lat,
	LeftLongitude = BoundingBox.STPointN(1).Long,
	RightLongitude = BoundingBox.STPointN(3).Long
from
	Filter as f
where
	BoundingBox.STIsClosed() = 1 and BoundingBox.STNumPoints() = 5	-- the surface is closed, actually 4 points

-- Structure changes. Constrain not null values

ALTER TABLE [Space] ALTER COLUMN Latitude FLOAT(25) NOT NULL
ALTER TABLE [Space] ALTER COLUMN Longitude FLOAT(25) NOT NULL

ALTER TABLE [Filter] ALTER COLUMN TopLatitude FLOAT(25) NOT NULL
ALTER TABLE [Filter] ALTER COLUMN BottomLatitude FLOAT(25) NOT NULL
ALTER TABLE [Filter] ALTER COLUMN LeftLongitude FLOAT(25) NOT NULL
ALTER TABLE [Filter] ALTER COLUMN RightLongitude FLOAT(25) NOT NULL
GO

-- Create indices.

CREATE NONCLUSTERED INDEX [idx_Space_ZipId_SizeTypeId_Rate_LastModified] ON [dbo].[Space]
(
	[SizeTypeId] ASC,
	[ZipId] ASC,
	[Rate] ASC,
	[LastModified] DESC
)
INCLUDE 
(
	[Id],
	[UserId],
	[SpaceAccessTypeId],
	[Title],
	[Decription],
	[IsListed],
	[SpaceTypeId],
	[Location],
	[DefaultPhotoID],
	[IsDeleted],
	[createdDate],
	[FullAddress],
	[ShortAddress],
	[AvailableSince],
	[Latitude],
	[Longitude]
)
GO

CREATE NONCLUSTERED INDEX [idx_Space_UserId_IsDeleted_IsListed_incl] ON [dbo].[Space]
(
	[UserId] ASC,
	[IsDeleted] ASC,
	[IsListed] ASC
)
INCLUDE 
(
 	[Id],
	[SizeTypeId],
	[SpaceAccessTypeId],
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
	[Longitude]
)
GO

CREATE NONCLUSTERED INDEX [idx_Space_LastModified_Latitude_Longitude_AvailableSince_Rate_IsListed_IsDeleted] ON [dbo].[Space]
(
	[LastModified],
    [Latitude],
    [Longitude],
    [AvailableSince],
    [Rate],
    [IsListed] ASC,
    [IsDeleted] ASC
    
)
INCLUDE 
(
    [Id],
    [UserId],
    [SizeTypeId],
    [SpaceAccessTypeId],
    [Title],
    [Decription],
    [SpaceTypeId],
    [Location],
    [DefaultPhotoID],
    [createdDate],
    [FullAddress],
    [ShortAddress],
    [ZipId]
)
GO

CREATE NONCLUSTERED INDEX [idx_Filter_UserId] ON [dbo].[Filter]
(
    [UserId] ASC
)
INCLUDE 
(
	[Id],
	[BoundingBox],
	[MinPrice],
	[MaxPrice],
	[RentStartDate],
	[CheckSizeType],
	[CheckType],
	[CheckAccessType],
	[Location],
	[ZipCodeId],
	[TopLatitude],
	[BottomLatitude],
	[LeftLongitude],
	[RightLongitude]
) 
GO