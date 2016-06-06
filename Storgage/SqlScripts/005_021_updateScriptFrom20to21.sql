drop index idx_User_Email_incl
  on dbo.[User]
go

 drop index idx_User_Email_unique_incl
    on dbo.[User]
go

alter table [User]
 alter column [Password] nvarchar(256) not null
go

alter table [User]
 drop column Salt 

go

create 
unique
 index idx_User_Email_incl
    on dbo.[User](email)
	   include (Id, Password, Phone/*, IsPhoneVerified*/, Lastname, Firstname, PaymentSystemSecret, AvgRate, PhoneVerificationStatusID, AvatarLink)
go

create 
 index idx_User_Email_unique_incl
    on dbo.[User] (Email)
       include (Id, Password, Phone/*, IsPhoneVerified*/, Lastname, Firstname, PaymentSystemSecret, AvgRate, PhoneVerificationStatusID, AvatarLink)
go