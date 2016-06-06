CREATE
 --drop
 TABLE [dbo].[RentPeriodType]
 (
	[Id] [uniqueidentifier] ROWGUIDCOL  NOT NULL CONSTRAINT [df_RentPeriodType_Id]  DEFAULT (newsequentialid()),
	[Title] [nvarchar](30) NOT NULL,
	[Description] [nvarchar](300) NULL,
	[Synonym] [nvarchar](30) NOT NULL,
 CONSTRAINT [PK_RentPeriodType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


create index idx_RentPeriodType_Synonym 
    on [dbo].[RentPeriodType] ([Synonym])
       include (Id, Title, Description) 
go

insert 
  into dbo.RentPeriodType(Id, Title, Description, Synonym)
values ('5894A15C-B6D4-E511-BEB9-6C71D91FABB6', 'LesserOrEqualThreeMonths', null, 'LesserOrEqualThreeMonths'),
       ('5794A15C-B6D4-E511-BEB9-6C71D91FABB6', 'LesserOrEqualSixMonths', null, 'LesserOrEqualSixMonths'),
       ('5994A15C-B6D4-E511-BEB9-6C71D91FABB6', 'LesserOrEqualYear', null, 'LesserOrEqualYear'),
       ('5A94A15C-B6D4-E511-BEB9-6C71D91FABB6', 'MoreYear', null, 'MoreYear')
go

select * from dbo.RentPeriodType
go

alter table dbo.MessageOffer
 drop column RentTill
go


alter table dbo.MessageOffer
  add RentPeriodId uniqueidentifier not null
      constraint fk_MessageOffer_RentPeriodId 
 foreign key(RentPeriodId) 
 references dbo.RentPeriodType(Id)
go

insert 
  into [dbo].[MessageOfferStatus] (id, Title, Description, Synonym)
select '11DC56D3-BAD4-E511-BEB9-6C71D91FABB6', 'Approved', 'Offer was approved', 'Approved'
union all
select '12DC56D3-BAD4-E511-BEB9-6C71D91FABB6', 'Pending', 'Offere is pending', 'Pending'
union all
select '13DC56D3-BAD4-E511-BEB9-6C71D91FABB6', 'Rejected', 'Offere was Rejected', 'Rejected'

select * from [MessageOfferStatus]