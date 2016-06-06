alter table [dbo].[Space]
  add IsDeleted bit not null
  constraint df_Space_IsDeleted
  default (0)
go

drop index [idx_Space_UserId_incl]
  on [dbo].[Space]
go

CREATE NONCLUSTERED INDEX [idx_Space_UserId_IsDeleted_incl] ON [dbo].[Space]
(
	[UserId] ASC,
	IsDeleted
)
INCLUDE ( 	[Id],
	[SizeTypeId],
	[Location],
	[SpaceAccessTypeId],
	[Title],
	[Decription],
	[IsListed],
	[SpaceTypeId],
	[DefaultPhotoID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

