create 
 --drop
 table dbo.Abuse
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_Abuse primary key 
 constraint df_Abuse_Id default newsequentialid()
 ,
 Message nvarchar(300), -- not null, -- too short, actually we can user even Nvarchar max
 ReportedAt datetimeoffset(7)
 constraint df_Abuse_ReportedAt
 default (dbo.fnGetUtc0DateWithOffset()),

 ReporterId uniqueidentifier not null --not sure about nullable and "ReporterId (nulllable, because no authorized user can report about non valid content).",
                                  --We can have DDOS b this method
 constraint fk_Abuse_ReporterId
 references dbo.[User](id)
 
)
go

create
 --drop
 index idx_Abuse_ReporterId
    on dbo.Abuse (ReporterId)
	   include (Id, Message, ReportedAt)
 --where ReporterId is not null
go

create 
 --drop
 table dbo.AbuseTypeDictionary 
(
 Id uniqueidentifier rowguidcol  not null
 constraint pk_AbuseTypeDictionary primary key 
 constraint df_AbuseTypeDictionary_Id default newsequentialid(),

 Title [nvarchar](100) NOT NULL,
 Description nvarchar(300) NOT NULL,
 Synonym varchar(255) NULL
)
go

create
unique
 index idx_AbuseTypeDictionary_Synonym_incl
    on dbo.AbuseTypeDictionary (Synonym)
       include (id, Title, Description)
 where synonym is not null
go

create 
 --drop
 table dbo.AbuseType
(
 Id uniqueidentifier rowguidcol  not null
 constraint pk_AbuseType primary key 
 constraint df_AAbuseType_Id default newsequentialid(),

 AbuseId uniqueidentifier not null
 constraint fk_AbuseType_AbuseId
 references dbo.Abuse(Id),

 TypeId uniqueidentifier not null
 constraint fk_AbuseType_TypeId
 references dbo.AbuseTypeDictionary(Id)

)
go

create 
unique
 index idx_AbuseType_AbuseId_TypeId
    on dbo.AbuseType(AbuseId, TypeId)
       include (Id)
go

create 
--unique
 index idx_AbuseType_TypeId
    on dbo.AbuseType(TypeId)
       include (Id, AbuseId)
go


create 
 --drop
 table dbo.AbuseSpace
(
 Id uniqueidentifier rowguidcol  not null
 constraint pk_AbuseSpace primary key 
 constraint df_AbuseSpace_Id default newsequentialid(),

 AbuseId uniqueidentifier not null
 constraint fk_AbuseSpace_AbuseId
 references dbo.Abuse(Id),

 SpaceId uniqueidentifier not null
 constraint fk_AbuseSpace_SpaceId
 foreign key 
 references dbo.Space(Id)
)
go

create
unique
 index idx_AbuseSpace_SpaceId_AbuseId
    on dbo.AbuseSpace(SpaceId, AbuseId) --for performance, when we search Abuses for space
       include (Id)
go

create
unique
 index idx_AbuseSpace_AbuseId
    on dbo.AbuseSpace(AbuseId) --Actually single Abuse may be used once only for now
       include (Id, SpaceId)
go


--Copy past Entity as AbuseSpace
create 
 --drop
 table dbo.AbuseRating
(
 Id uniqueidentifier rowguidcol  not null
 constraint pk_AbuseRating primary key 
 constraint df_AbuseRating_Id default newsequentialid(),

 AbuseId uniqueidentifier not null
 constraint fk_AbuseRating_AbuseId
 references dbo.Abuse(Id),

 RatingId uniqueidentifier not null
 constraint fk_AbuseRating_RatingId
 foreign key 
 references dbo.Rating(Id)
)
go

create
unique
 index idx_AbuseRating_RatingId_AbuseId
    on dbo.AbuseRating(RatingId, AbuseId) --for performance, when we search Abuses for space
       include (Id)
go

create
unique
 index idx_AbuseRating_AbuseId
    on dbo.AbuseRating(AbuseId) --Actually single Abuse may be used once only for now
       include (Id, RatingId)
go     

insert
  into dbo.AbuseTypeDictionary (Title, Description, Synonym, id)
values 

 ('Fraud', 'Fraud', 'Fraud', 'C71B0C1C-DF11-E611-BEC6-6C71D91FABB6'),
 ('Abuse', 'Abuse', 'Abuse', 'C81B0C1C-DF11-E611-BEC6-6C71D91FABB6'),
 ('Illegal content', 'Illegal content', 'IllegalContent', 'C91B0C1C-DF11-E611-BEC6-6C71D91FABB6'),
 ('Property issues', 'Property issues', 'PropertyIssues', 'CA1B0C1C-DF11-E611-BEC6-6C71D91FABB6'),
 ('Other', 'Other', 'Other', 'CB1B0C1C-DF11-E611-BEC6-6C71D91FABB6')

select * from dbo.AbuseTypeDictionary