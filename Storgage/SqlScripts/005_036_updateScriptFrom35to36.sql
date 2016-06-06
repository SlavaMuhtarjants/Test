--Author: Alexander P.
--Edited: Nikita Pochechuev

truncate Table dbo.UserDevice 
go

Alter Table dbo.UserDevice 
  Add [MobileEndpoint] nvarchar(256) Not Null
      constraint uk_UserDevice_MobileEndpoint 
	  unique
      ,
      [SubscriptionEndpoint] nvarchar(256) Not Null
      constraint uk_UserDevice_SubscriptionEndpoint 
	  unique,

	  IsPushNotificationEnabled bit Not Null
	  constraint df_UserDevice_IsPushNotificationEnabled 
	  default (1)
go


drop INDEX [idx_UserDevice_UserId] 
  ON [dbo].[UserDevice]
go

alter table  dbo.UserDevice 
  alter column [PushNotificationToken] nvarchar(64) not null
go


CREATE NONCLUSTERED INDEX [idx_UserDevice_UserId] ON [dbo].[UserDevice]
(
	[UserId] ASC
)
INCLUDE ( 	[Id], [PushNotificationToken], [MobileEndpoint], [SubscriptionEndpoint], IsPushNotificationEnabled) 
go


create unique index idx_UserDevice_PushNotificationToken
    on dbo.UserDevice ([PushNotificationToken])
       include ([Id], [UserId], [MobileEndpoint], [SubscriptionEndpoint], IsPushNotificationEnabled)
go
