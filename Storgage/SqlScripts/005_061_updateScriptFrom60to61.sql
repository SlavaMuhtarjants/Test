alter table dbo.[User]
  add AccessFailedCount int not null
  constraint df_User_AccessFailedCount 
  default (0),

  LockoutEndDate datetimeoffset
go

drop INDEX [idx_User_Email_unique_incl] ON [dbo].[User]
go


CREATE UNIQUE NONCLUSTERED INDEX [idx_User_Email_unique_incl] ON [dbo].[User]
(
	[Email] ASC
)
INCLUDE ( 	[Id],
	[Password],
	[Phone],
	[Lastname],
	[Firstname],
	[PaymentSystemSecret],
	[AvgRate],
	[PhoneVerificationStatusID],
	[AvatarLink],
	[CountRating],
	[FacebookID],
	[EmailVerificationStatusID],
	[IsExternal],
	[OutPaymentPoint],
	[IncPaymentPoint],
	AccessFailedCount,
	LockoutEndDate) 
WHERE ([Email] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

drop INDEX [idx_User_FacebookID_incl] ON [dbo].[User]
go

/****** Object:  Index [idx_User_FacebookID_incl]    Script Date: 27.04.2016 17:18:39 ******/
CREATE UNIQUE NONCLUSTERED INDEX [idx_User_FacebookID_incl] ON [dbo].[User]
(
	[FacebookID] ASC
)
INCLUDE ( 	[Id],
	[Email],
	[Password],
	[Phone],
	[Lastname],
	[Firstname],
	[PaymentSystemSecret],
	[AvgRate],
	[PhoneVerificationStatusID],
	[AvatarLink],
	[CountRating],
	[EmailVerificationStatusID],
	[IsExternal],
	[OutPaymentPoint],
	[IncPaymentPoint],
	AccessFailedCount,
	LockoutEndDate) 
WHERE ([FacebookID] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

