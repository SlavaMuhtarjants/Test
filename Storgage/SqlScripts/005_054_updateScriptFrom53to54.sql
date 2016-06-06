Alter Table dbo.UserDevice 
  Add RefreshedAt datetimeoffset Not Null
  constraint df_UserDevice_RefreshedAt
  Default dateadd(year, 1000, getdate())
go


/****** Object:  Index [idx_UserDevice_PushNotificationToken]    Script Date: 20.04.2016 11:04:11 ******/
CREATE UNIQUE NONCLUSTERED INDEX [idx_UserDevice_PushNotificationToken_incl] ON [dbo].[UserDevice]
(
	[PushNotificationToken] ASC
)
INCLUDE ( 	[Id],
	[UserId],
	[MobileEndpoint],
	[SubscriptionEndpoint],
	[IsPushNotificationEnabled],
	[TopicEndpoint],
    [RefreshedAt]
	) 
go

drop index [idx_UserDevice_PushNotificationToken] ON [dbo].[UserDevice]
go

/****** Object:  Index [idx_UserDevice_UserId]    Script Date: 20.04.2016 11:08:10 ******/
CREATE NONCLUSTERED INDEX [idx_UserDevice_UserId_RefreshedAt_incl] ON [dbo].[UserDevice]
(
	[UserId], RefreshedAt
)
INCLUDE ( 	[Id],
	[PushNotificationToken],
	[MobileEndpoint],
	[SubscriptionEndpoint],
	[IsPushNotificationEnabled],
	[TopicEndpoint]
	)
go

drop index [idx_UserDevice_UserId] on dbo.[UserDevice]
go