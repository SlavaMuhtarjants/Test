create 
 table dbo.EmailVerificationStatus
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_EmailVerificationStatus
 primary key
 constraint df_EmailVerificationStatus_pk
    default newsequentialid(),

 Title nvarchar(20) not null, 
 Description nvarchar(255),
 Synonym varchar(255) not null
)
go

create
unique
 index idx_EmailVerificationStatus_Synonym_incl
    on dbo.EmailVerificationStatus(Synonym) 
	   include (Id, Title, Description)
 where Synonym is not null
go

insert
  into dbo.EmailVerificationStatus(Id, Title, Synonym)
values 
	('5631FA69-3EF0-E511-811A-02B1C768461B', 'MustVerified', 'MustVerified')
insert
  into dbo.EmailVerificationStatus(Title, Synonym)
values 
	('Verified', 'Verified'),		
	('NotVerified', 'NotVerified')
go


drop 
 index idx_User_Email_unique_incl
   on [dbo].[User]
go

drop 
 index idx_User_FacebookID_incl
   on [dbo].[User]
go

alter table dbo.[User]
  add EmailVerificationStatusID uniqueidentifier not null
	constraint fk_User_EmailVerificationStatusID
	foreign key
	references dbo.EmailVerificationStatus(Id)

	constraint df_User_EmailVerificationStatusID
	default ('5631FA69-3EF0-E511-811A-02B1C768461B')
go

create
 index idx_User_Email_unique_incl
    on dbo.[User](email)
	   include (Id, Password, Phone, Lastname, Firstname, PaymentSystemSecret, AvgRate, PhoneVerificationStatusID, AvatarLink, CountRating, FacebookID, EmailVerificationStatusID)
go

create 
 unique
 index idx_User_FacebookID_incl
    on dbo.[User] (FacebookID)
include (Id, Email, Password, Phone, Lastname, Firstname, PaymentSystemSecret, AvgRate, PhoneVerificationStatusID, AvatarLink, CountRating, EmailVerificationStatusID)
where FacebookID is not null
go
