Drop Index [UserDevice].[idx_UserDevice_UserId_RefreshedAt_incl]
Go

Drop Index [UserDevice].[idx_UserDevice_PushNotificationToken_incl]
Go

alter table dbo.[UserDevice]
 drop constraint df_UserDevice_RefreshedAt
go

Alter Table [UserDevice] Drop Column RefreshedAt
Go

/****** Object:  Index [idx_UserDevice_PushNotificationToken_incl]    Script Date: 18.05.2016 20:30:14 ******/
CREATE UNIQUE NONCLUSTERED INDEX [idx_UserDevice_PushNotificationToken_incl] ON [dbo].[UserDevice]
(
	[PushNotificationToken] ASC
)
INCLUDE ( 	[Id],
	[UserId],
	[MobileEndpoint],
	[SubscriptionEndpoint],
	[IsPushNotificationEnabled],
	[TopicEndpoint]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

CREATE NONCLUSTERED INDEX [idx_UserDevice_UserId_incl] ON [dbo].[UserDevice]
(
    [UserId] ASC
)
INCLUDE (   [Id],
    [PushNotificationToken],
    [MobileEndpoint],
    [SubscriptionEndpoint],
    [IsPushNotificationEnabled],
    [TopicEndpoint]) 
GO
