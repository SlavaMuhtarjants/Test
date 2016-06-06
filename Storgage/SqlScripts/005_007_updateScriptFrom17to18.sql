exec sp_rename 'dbo.Chart', 'Chat';
go

exec sp_rename 'dbo.pk_Chart', 'pk_Chat';
go

exec sp_rename 'dbo.fk_Chart_AdSpaceId', 'fk_Chat_AdSpaceId';
go

exec sp_rename 'dbo.fk_Chart_UserId', 'fk_Chat_UserId';
go

exec sp_rename N'dbo.Chat.idx_Chart_AdSpaceId_incl', N'idx_Chat_AdSpaceId_incl', N'INDEX';
go

exec sp_rename N'dbo.Chat.idx_Chart_UserId_incl', N'idx_Chat_UserId_incl', N'INDEX';
go

exec sp_rename N'dbo.Message.ChartId', N'ChatId', N'column';
go

exec sp_rename 'dbo.fk_Message_ChartId', 'fk_Message_ChatId';
go

exec sp_rename 'dbo.fk_Chart_LastMessageId', 'fk_Chat_LastMessageId';
go

exec sp_rename N'dbo.Message.idx_Message_ChartId_incl', N'idx_Message_ChatId_incl', N'INDEX';
go

alter table dbo.Space
  add DefaultPhotoID uniqueidentifier 
  constraint fk_Space_DefaultPhotoID
  foreign key
  references [dbo].PhotoLibrary (Id)
go

--select *
update s 
   set s.DefaultPhotoID = pl.Id
  from dbo.Space s
 inner
  join dbo.PhotoLibrary pl
    on pl.SpaceId = s.Id
   and pl.IsDefault = 1
go

alter table dbo.PhotoLibrary
 drop constraint df_PhotoLibrary_IsDefault
go

alter table dbo.PhotoLibrary
 drop column IsDefault
go

  drop
 index idx_Space_UserId_incl
    on dbo.Space
go

create
 index idx_Space_UserId_incl
    on dbo.Space(UserId)
 include (Id, SizeTypeId, /*City, Address,*/ Location, SpaceAccessTypeID, Title, Decription, /*BusySince, BusyTill, RentTill, RentSince,*/ IsListed, SpaceTypeId, DefaultPhotoID)

go