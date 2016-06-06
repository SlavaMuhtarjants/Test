alter table dbo.[User]
 add FacebookID nvarchar(64)
go

create 
 --drop
 unique
 index idx_User_FacebookID_incl
    on dbo.[User] (FacebookID)
include (Id, Email, Password, Phone, Lastname, Firstname, PaymentSystemSecret, AvgRate, PhoneVerificationStatusID, AvatarLink, CountRating)
where FacebookID is not null
go


drop INDEX [idx_User_Email_unique_incl] 
   ON [dbo].[User]
go


CREATE unique INDEX [idx_User_Email_unique_incl] 
   ON [dbo].[User]
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
	FacebookID) 
where [Email] is not null
go
