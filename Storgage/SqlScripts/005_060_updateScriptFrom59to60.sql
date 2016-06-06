create 
 --drop
 table dbo.PaymentPointType
(
 id uniqueidentifier rowguidcol not null
 constraint pk_PaymentPointType
 primary key
 constraint df_PaymentPointType_id
 default newsequentialid(),
 Title nvarchar(20) not null,
 Description nvarchar(255),
 Synonym nvarchar(255) not null
)
go

create 
 unique
 --drop
 index idx_PaymentPointType_Synonym
    on dbo.PaymentPointType(Synonym)
       include(id, Title, Description)
go

--delete from dbo.PaymentPointType

insert
  into dbo.PaymentPointType (Title, Description, Synonym, id)

select 'Account' as Title, 'Bank account payment point type' as Description, 'Account' Synonym, 'AA737AB4-BE0B-E611-BEC5-6C71D91FABB6' as id
union all
select 'Card' as Title, 'Bank card (credit or debit)' as Description, 'Card' Synonym, 'AB737AB4-BE0B-E611-BEC5-6C71D91FABB6' as id

select * from dbo.PaymentPointType
go

create 
 --drop
 table dbo.PaymentPoint
(
 id uniqueidentifier rowguidcol not null
 constraint pk_PaymentPoint
 primary key
 constraint df_PaymentPoint_id
 default newsequentialid(),

 Number nvarchar(100) not null,

 UserId uniqueidentifier not null
 constraint fk_PaymentPoint_UserId
 foreign key
 references dbo.[User](Id),
 
 TypeId uniqueidentifier not null
 constraint fk_PaymentPoint_PaymentPointTypeId
 foreign key
 references dbo.PaymentPointType(Id),

 PaymentSystemId nvarchar(64) not null,

 IsDeleted bit not null
 constraint df_PaymentPoint_IsDeleted
 default(0)

)
go

create 
 index idx_PaymentPoint_UserId
    on dbo.PaymentPoint (UserId, IsDeleted)
	include (id, Number, TypeId, PaymentSystemId)
go

create
unique 
 index idx_PaymentPoint_PaymentSystemId
    on dbo.PaymentPoint (PaymentSystemId)
	include (id, Number, TypeId, UserId, IsDeleted)
go

alter table dbo.[User]
  add OutPaymentPoint uniqueidentifier 
  constraint fk_User_OutPaymentPoint
  references dbo.PaymentPoint(Id)
go

alter table dbo.[User]
  add IncPaymentPoint uniqueidentifier 
  constraint fk_User_IncPaymentPoint
  references dbo.PaymentPoint(Id)
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
	OutPaymentPoint,
	IncPaymentPoint) 
WHERE ([Email] IS NOT NULL)
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
	[IsExternal],
	OutPaymentPoint,
	IncPaymentPoint) 
WHERE ([FacebookID] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

