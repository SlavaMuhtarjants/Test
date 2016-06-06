alter table [dbo].[User]
  add IsExternal bit not null 
  constraint df_User_IsExternal 
  default (0)
go

drop INDEX [idx_User_Email_unique_incl] ON [dbo].[User]
go

CREATE NONCLUSTERED INDEX [idx_User_Email_unique_incl] ON [dbo].[User]
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
	[IsExternal]
	)
go

drop INDEX [idx_User_FacebookID_incl] ON [dbo].[User]
go

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
	[IsExternal]) 
WHERE ([FacebookID] IS NOT NULL)
go

/*
This logic may be incorrect, because we don't know when and how password was set
*/

--select u.*
update u
   set u.IsExternal = 1
  from dbo.[User] u
 where u.FacebookID is not null
   and (u.Password is null or u.Password = '')