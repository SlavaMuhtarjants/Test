use [storgage]
go


/*
This table will contain values with the SAME PK value that other dictionary tables like dbo.SizeType
I suppose that we will copy data to this table by trigger, for example.
May be on DAL level
*/
create 
 --drop
 table dbo.RootDictionary --I still think that simple CommonDictionary is good approach 
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_FilterSizeType
 primary key
 
 --I decided don't generate PK here for now
 --constraint df_FilterSizeType_pk
 --   default newsequentialid()
 -- other attributes (for sorting, logical deletion and etc) should be created late
)
go

create 
 --drop
 table dbo.PhoneVerificationStatus
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_PhoneVerificationStatus
 primary key
 constraint df_PhoneVerificationStatus_pk
    default newsequentialid(),

 Title nvarchar(20) not null, 
 Description nvarchar(255),

 Synonym varchar(255)
)
go

create
unique
 index idx_PhoneVerificationStatus_Synonym_incl
    on dbo.PhoneVerificationStatus(Synonym) 
	   include (Id, Title, Description)
 where Synonym is not null
go

--We will hard code this ID.
insert
  into dbo.PhoneVerificationStatus(Id, Title)
values 
         ('8B912FAB-60BF-E511-BEB2-89294490C0F5', 'NotVerified')
insert
  into dbo.PhoneVerificationStatus(Title)
values 
		 ('Verified'),		
		 ('MustVerified')

select t.* 
  from dbo.PhoneVerificationStatus t
       
go

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

--AccessType {Limited, Unlimited}
create 
 --drop
 table dbo.SpaceAccessType
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_SpaceAccessType
 primary key
 constraint df_SpaceAccessType_pk
    default newsequentialid(),

 Title nvarchar(20) not null, 
 Description nvarchar(255),

 Synonym varchar(255)
)
go

create
unique
 index idx_SpaceAccessType_Synonym_incl
    on dbo.SpaceAccessType(Synonym) 
	   include (Id, Title, Description)
 where Synonym is not null
go

--We will hard code this ID.
insert
  into dbo.SpaceAccessType(Id, Title)
values 
         ('98391066-61BF-E511-BEB2-89294490C0F5', 'Unlimited')
insert
  into dbo.SpaceAccessType(Title)
values 
		 ('Limited')	
		 
insert
  into dbo.RootDictionary(Id)
select Id
  from dbo.SpaceAccessType		 		
go

select t.* 
  from dbo.SpaceAccessType t
       
go

alter table dbo.SpaceAccessType
 add 
 --drop
     constraint fk_SpaceAccessType_Id
 foreign key (Id)
 references RootDictionary(id)
 
alter table dbo.SpaceAccessType 
 nocheck constraint fk_SpaceAccessType_Id
go


create
  --drop 
 table dbo.[User] --This is why I prefere to use prefixes 
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_User 
 primary key
 constraint df_User_pk
    default newsequentialid(),

 Email nvarchar(256) not null
 --Constraint will not be created because unique index with "include" statement will be created
 --it's frequently operation when we return all info about user by email
 --especially for ORMs
 /*constraint uk_User_Email
 unique*/
 ,

 Password nvarchar(256) not null, -- varbinary(32) not null, --I am not sure about "not null" but it's very possible

 --Salt varbinary (32) not null,

 /*
 We can have single phone number for user ONLY and we don't have history of changes for
 this attribute. Other users will see current/actual phone number even for previous events
 
 Also it looks like it IS NOT unique and the same phone may be used by few users
 (or some user may use OLD number of another user)
 */
 Phone varchar(15),

 /*
 Task was:
 1 - phone number was verfied with success.
 0 - phone number was verified without success
 Null - phone number has not been verified yet"
 
 But I think that if we need 3 statuses then we need link to some dictionary
 (without enums and etc).
 It's abnormal to learn values (especially for big amount of values).
 I do dictionary even for "Gender" attribute for "Person" that has 2 values in most case 
 (sometimes additional "Not defined"). 
 In this case I can join dictionary in any place withot hard codings (analog case-operator and etc).
 when I need to display results or list of values.
 */
 --IsPhoneVerified bit,

 /*
 We don't have separated entity type as "Person"
 and we don't have history of changes for lastname/firstname.
 All users will have current values only event for PAST CONTRACTS!
 */
 Lastname nvarchar(30) not null, --may be we need to increase this length in the future
 Firstname nvarchar(30) not null,

 PaymentSystemSecret nvarchar(50) --As I understand it should be unique but null values possible
                                  --May be unique index with "where PaymentSystemSecret is not null"
								  --condition will be needed in the future if we search by this value
 ,
 --TotalRate, actually it's "avarage rate" or just "rate" 
 --I don't think that it will have more than 2 signs before comma/point, I think 1 sign only, but let it be 2 
 --and I think that it will have 2 digits after comma.
 AvgRate numeric(4, 2),

PhoneVerificationStatusID uniqueidentifier not null
constraint fk_User_PhoneVerificationStatusID
foreign key
references dbo.PhoneVerificationStatus(Id)

constraint df_User_PhoneVerificationStatusID
--I don't like hard code by ID! I prefere Synonyms (like in CommonDictionary logic).
--Usually I do it on stored procedure level that inserts/updates data and setup default value if this parameter is NULL
--I thin the same should be done on Web-server level in DAL
default ('8B912FAB-60BF-E511-BEB2-89294490C0F5'),

 AvatarLink nvarchar(256),

 CountRating int not null
  constraint df_User_CountRating
  default (0)

)
go

create 
unique
 index idx_User_Email_incl
    on dbo.[User](email)
	   include (Id, Password, Phone/*, IsPhoneVerified*/, Lastname, Firstname, PaymentSystemSecret, AvgRate, PhoneVerificationStatusID, AvatarLink, CountRating)
go

create
  --drop 
 table dbo.UserDevice --This is why I prefere to use prefixes 
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_UserDevice
 primary key
 constraint df_UserDevice_pk
    default newsequentialid(),
 
 UserId uniqueidentifier not null 
 constraint fk_UserDevice_UserId
 foreign key
 references dbo.[User](Id),

 PushNotificationToken nvarchar(max) not null--varchar(36) not null
)
go

--It's better to create UNIQUE index for UserId + PushNotificationToken (PushNotificationToken should be 2nd)
--but we don't know lenght of PushNotificationToken field
--I hope that user will not have too big amount of devices
create
 index idx_UserDevice_UserId
    on dbo.UserDevice(UserId)
	   include (Id, PushNotificationToken)
go

--Indexes   

--It's very posible that we will search users by email (this is whe it's first) and check password)
--But email must be unique and we will need to check password in single row
--this is whey password will be in "include" part only
--but condition "where email=@email and password = @password" is very possible
create 
 index idx_User_Email_unique_incl
    on dbo.[User] (Email)
       include (Id, Password, Phone/*, IsPhoneVerified*/, Lastname, Firstname, PaymentSystemSecret, AvgRate, PhoneVerificationStatusID, AvatarLink, CountRating)
go


/*
I will not create indexes for the simplest dictionaries now.
The will have PK only
But tCommonDictionary has big amount of indexes for different cases.
*/


create 
 --drop
 table dbo.SizeType
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_SizeType 
 primary key
 constraint df_SizeType_pk
    default newsequentialid(),

 Title nvarchar(10) not null, --not sure about sizes 10 and 30 for common case but it's possible for this specific case
 Description nvarchar(30) not null, --in common case it can be null-able

 Synonym varchar(255)
)
go

create
unique
 index idx_SizeType_Synonym_incl
    on dbo.SizeType(Synonym) 
	   include (Id, Title, Description)
 where Synonym is not null
go

create 
 --drop
 table dbo.SpaceType
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_SpaceType 
 primary key
 constraint df_SpaceType_pk
    default newsequentialid(),

 Title nvarchar(20) not null,
 Description nvarchar(50) null,

 Synonym varchar(255)
)
go

create
unique
 index idx_SpaceType_Synonym_incl
    on dbo.SpaceType(Synonym) 
	   include (Id, Title, Description)
 where Synonym is not null
go

--Was renamed from "Statuses"
create 
 --drop
 table dbo.MessageOfferStatus
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_MessageOfferStatus 
 primary key
 constraint df_MessageOfferStatus_pk
    default newsequentialid(),

 Title nvarchar(20) not null,
 Description nvarchar(50) not null,

 Synonym varchar(255)
)
go

create
unique
 index idx_MessageOfferStatus_Synonym_incl
    on dbo.MessageOfferStatus(Synonym) 
	   include (Id, Title, Description)
 where Synonym is not null
go

insert 
  into [dbo].[MessageOfferStatus] (id, Title, Description, Synonym)
select '11DC56D3-BAD4-E511-BEB9-6C71D91FABB6', 'Approved', 'Offer was approved', 'Approved'
union all
select '12DC56D3-BAD4-E511-BEB9-6C71D91FABB6', 'Pending', 'Offere is pending', 'Pending'
union all
select '13DC56D3-BAD4-E511-BEB9-6C71D91FABB6', 'Rejected', 'Offere was Rejected', 'Rejected'
go

select * from [MessageOfferStatus]
go
/*
I still think that we need have a ling to Zip codes.
This table looks type dictionary of Zip, but I will change name ZipRank to Zip
Also PK value will be used 
It's possible that this table will have other attributes (not Rank only).
*/
create 
 --drop
 table dbo.Zip
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_Zip
 primary key
 constraint df_Zip_pk
    default newsequentialid(),
 

 Zip nvarchar(10) not null,

 Rank int not null
 /*
 According to comment:
Popularity of area that correspond zip code
1 - small
2 - medium
3 - large
5 - extra large

 I don't understand is it INT value, link to some dictionary or ENUM without 4-value?

 The same question for the next table TrainingSet
 */

)
go

create 
-- drop
 index idx_Zip_Zip
    on [dbo].[Zip] (zip)
 include (Id, Rank)
go


--I am not sure that we will search by firstname/lastname/phone now, because these data will be accessible after payment
--New indexes will be created based on the future queries.

create 
 --drop
 table dbo.Space
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_Space 
 primary key
 constraint df_Space_pk
    default newsequentialid(),
 
 --It means that single space may have single OWNER only!
 --Actually it's possible that owner may be changed in real word
 --Single user may lease store (thay are not shared in family or etc).
 UserId uniqueidentifier not null --I SUPPOSE that we need it
 constraint fk_Space_UserId
 foreign key
 references dbo.[User](Id),

 SizeTypeId uniqueidentifier not null --I make sure about not null, changed NAME from "SizeId"
 constraint fk_Space_SizeTypeId 
 foreign key
 references dbo.SizeType(Id),

 /*
 ZipId uniqueidentifier not null
 constraint fk_Space_ZipId 
 foreign key 
 references dbo.Zip(Id),
 */
 --Actually I think that it's bad idea to use nvarchar(50) type,
 --I think it must be link to dictionary also and we need to prevent 
 --select distinct City from dbo.Space
 --queries when we list of cities
 -- the same for ZIP codes
 --City nvarchar(50) not null,

 /*
 For GIS better way is to use good dictionary with buildings 
 but for our case, I think, it's enough and it's not a navigator
 */
 --Address nvarchar(50) not null,

 --it's VERY possible that computed columns with long/lat-itude will be created later to optimize searching
 --and query perfromance
 Location geography,

 SpaceAccessTypeId uniqueidentifier not null
 constraint fk_Space_SpaceAccessTypeId
 foreign key
 references dbo.SpaceAccessType(Id)

 constraint df_Space_SpaceAccessTypeId
 default ('98391066-61BF-E511-BEB2-89294490C0F5'), --As I understand it's NOT limited by default,

 Title nvarchar(50) not null, --may be it will be indexed, it depends on future queries

 Decription nvarchar(255), --I think good idea is to use FULL-TEXT SEARCH for Description and Title, may be ZIP, Address shold be included also 

 --IMPORTANT
 --As I understnad we don't need here
 --1) Time Zones (and datatypes lkie datetimeoffset)
 --2) Exact time (datetime datatype) and etc.
 --For hotels and etc, it's possoble that others clients may start to use room
 --in 2nd part of day for example. Is it possible here? May be we need some additional comment?
 
 --Moved to SpaceBusy
 --BusySince date,
 --BusyTill date,
 
 --Moved to AdSpace
 --RentTill date, --don't fill the difference good between BusyTill and RentTill 
 --RentSince date,

 IsListed bit not null 
 constraint df_Space_IsListed
 default (0), --IMPORTANT - not sure about DF value because "0 - Users can't find this storgage"
             --may be we need to remove it.


 SpaceTypeId uniqueidentifier not null
 constraint fk_Space_SpaceTypeId  
 foreign key 
 references dbo.SpaceType(Id),

 DefaultPhotoID uniqueidentifier 
 constraint fk_Space_DefaultPhotoID
 foreign key
 references [dbo].[RootDictionary] (Id),

 IsDeleted bit not null
  constraint df_Space_IsDeleted
  default (0)
)
go

--I suppose that we will search Spaces by user, type, city... and some other attributes but correct indexes will depends on future queries
--Actually this table (and related  dbo.AdSpace) will have the biggest amout of indxes
create
 index idx_Space_UserId_IsDeleted_incl
    on dbo.Space(UserId, IsDeleted)
 include (Id, SizeTypeId, /*City, Address,*/ Location, SpaceAccessTypeID, Title, Decription, /*BusySince, BusyTill, RentTill, RentSince,*/ IsListed, SpaceTypeId, DefaultPhotoID)

go

create 
 --drop
 table dbo.AdSpace
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_AdSpace
 primary key
 constraint df_AdSpace_pk
    default newsequentialid(),

 SpaceId uniqueidentifier not null
 constraint fk_AdSpace_SpaceId 
 foreign key
 references dbo.Space(Id),

 createdDate datetimeoffset not null
 constraint df_AdSpace_createdDate 
 default ToDateTimeOffset(getutcdate(), '+00:00'),

  /*
 I am not sure about comment

 "Rate per month for storgage"

 Because I rember something about "weeks and weekly payment"
 */
 Rate money not null, --I CHANGED data type here from float
 
 /*
 Here we will need other specific attributes
 May be each Ad will have some specific Descriptions and Titels

 Also I am not sure whethre we need some history of changes for Ad.
 May be we need toknow price change history inside each AD or something else/.
 */
 
 RentTill date, --don't fill the difference good between BusyTill and RentTill 
 RentSince date
)
go

create 
 index idx_AdSpace_SpaceId_incl 
    on dbo.AdSpace(SpaceId)
	   include (Id, createdDate, Rate, RentTill, RentSince)
go

create 
 --drop
 table dbo.SpaceBusy
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_SpaceBusy
 primary key
 constraint df_SpaceBusy_pk
    default newsequentialid(),

 SpaceId uniqueidentifier not null
 constraint fk_SpaceBusy_SpaceId 
 foreign key
 references dbo.Space(Id),

 --we always have "startDate"
 StartDate datetimeoffset not null,
 
 --This date is nullable
 --if this date is null then we can't set NEXT record because we don't know when current renter finish to rent space
 --I suppose that when endDate is finished we can setup the next period
 EndDate datetimeoffset,


 --Before I've supposed to have link to previous period
 --but now I thnik that it's better solution because
 --we don't need redundant index (PreviousSpaceBusyID) include (....) by default (may be we will have in for some task in future)
 --it's easy to define next record existing even without join (and subquery with ordering by date)
 NextSpaceBusyID uniqueidentifier 
 constraint fk_SpaceBusy_NextSpaceBusyID
 foreign key
 references  dbo.SpaceBusy(Id)
)

create 
unique
  --drop
 index idx_SpaceBusy_NextSpaceBusyID_incl 
    on dbo.SpaceBusy(NextSpaceBusyID)
	   include (Id, SpaceId, startDate, endDate)
 where NextSpaceBusyID is not null
go

/*
!!!
I think that we should have LINK from Space to this table (backward link for StorgageId)
and remove IsDefault
It prevents additional condtion "... and isDefault = 1" and link to PK will work much better
Also it gurantee that we have SINGLE value only and it may prevent multiplications
*/ 
create 
 --drop
 table dbo.PhotoLibrary
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_PhotoLibrary 
 primary key
 constraint df_PhotoLibrary_pk
    default newsequentialid(),

 Link nvarchar(256) not null,
 
 --I changed name from StorgageId because tables is "Space"
 SpaceId uniqueidentifier not null
 constraint fk_PhotoLibrary_SpaceId
 references dbo.Space(Id)
)

/*
Actually,
in the future not Space only may have Photo.
Other entity types may use photos also (feedbacks and etc, f.e.).
Better way is to have some main entity type as "Photo Galery"  but "Space" and other possible entity types
may link to this Galery. Galary has the same child entity PhotoLibrary and link to default photo
*/
go

/*
Pay attention to provided example 
001_TestTableAndData.sql
002_generateQueryParts.sql

It contains "OR" logic also
and it deosn't contains, IsSearchBySize, IsSearchByLocation, IsSearchByRateRange 
(I think that these fields are redundand)

Here user may create few filters if it wants to see something like "OR" operation 
but when Space meets to few filters it will see the same item few times.
Also this approach doesn't allow "Exclude" logic.
*/
create 
 --drop
 table dbo.Filter
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_Filter
 primary key
 constraint df_Filter_pk
    default newsequentialid(),

 UserID uniqueidentifier not null
 constraint fk_Filter_UserID
 foreign key 
 references dbo.[User](Id),

 /*
 Later I WANT TO DISCUSS this functionaity.
 Frequenlty we will need to check whether new created or edited Space meets to some filter condition
 */
 IsNotifyMe bit not null 
 constraint df_Filter_IsNotifyMe
 default (0)
)

/*
Also I don't think that filter must be related to User directly.
It should be retated to some "Event of rent".
It should not be active when this event is ended or canceled.
It should have, at least, "end date".
*/
go

create 
 --drop
 table dbo.FilterCondition
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_FilterCondition
 primary key
 constraint df_FilterCondition_pk
    default newsequentialid(),  
 
 FilterId uniqueidentifier not null
 constraint fk_FilterCondition_FilterId 
 foreign key 
 references dbo.Filter(Id),


 /*
 As I said before, I prefre searching by rectangle (or square).
 Also in my variant user may define few rectangles by OR-logic
 and describe needed region by rectangles.
 Sometimes radius from some point may include river and another bank (riverside).
 This search type doesn't have sence in big amount of cases
 */
 Location geometry,

 IsLimitedAccess bit not null 
 constraint df_FilterCondition_IsSearchByLimitedAccess
 default (0),

 --Changed type from float to money
 --I think that it must be nullable ignored when value is null when we search by filter
 MinRate money not null 
 constraint df_Filter_MinRate
 default (0),
 MaxRate money not null 
 constraint df_Filter_MaxRate
 default (0),

  --We know that we use these fields for search,
 --it field and others may be named withput "SearchBy"
 --like "Location" is not "SearchByLocation"
 --Actually in should be called as "SpaceTypeId" (the same name as for "Space")

 /*SpaceTypeId uniqueidentifier not null
 constraint fk_FilterCondition_SpaceTypeId  
 foreign key 
 references dbo.SpaceType(Id),*/

 RentTill date,
 RentSince date
 )
 go


alter table dbo.SizeType
 add 
 --drop
     constraint fk_SizeType_Id
 foreign key (Id)
 references RootDictionary(id)
 
alter table dbo.SizeType 
 nocheck constraint fk_SizeType_Id
go

--dbo.SpaceType
alter table dbo.SpaceType
 add 
 --drop
     constraint fk_SpaceType_Id
 foreign key (Id)
 references RootDictionary(id)
 
alter table dbo.SpaceType 
 nocheck constraint fk_SpaceType_Id
go

create 
 --drop
 table dbo.FilterRootDictionary --dbo.SizeTypesInFilter --NAME WAS CHANGED
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_FilterRootDictionary
 primary key
 constraint df_FilterRootDictionary_pk
    default newsequentialid(),

 FilterId uniqueidentifier not null
 constraint fk_FilterRootDictionary_FilterId
 references dbo.Filter(Id),

 RootDictionaryId uniqueidentifier not null --I make sure about not null
 constraint fk_FilterRootDictionary_SizeTypeId 
 foreign key
 references dbo.RootDictionary(Id),
)
go

--We will search by Filter and return values
--May be and vice versa for emai notificaiton but another index will be created in this case
create 
 index idx_FilterRootDictionary_FilterId_incl
    on dbo.FilterRootDictionary(FilterId)
       include (Id, RootDictionaryId)
go


/*
Why it's "Chart" and not "Chat/Conversation/auction" ?
*/
create 
 --drop
 table dbo.Chat--Chart
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_Chat
 primary key
 constraint df_Chart_pk
    default newsequentialid(),
 
 /*
 SpaceId uniqueidentifier not null
 constraint fk_Chart_SpaceId
 references dbo.Space(Id),
 */

 --Link changed to AdSpace 
 AdSpaceId uniqueidentifier not null
 constraint fk_Chat_AdSpaceId
 references dbo.AdSpace(Id),
 

 UserId uniqueidentifier not null
 constraint fk_Chat_UserId
 foreign key
 references dbo.[User](Id),

 LastMessageId uniqueidentifier -- constraint will be created late, after child table creation
 ,
 CurrentOfferId uniqueidentifier
)
go

create 
 index idx_Chat_AdSpaceId_incl
    on dbo.Chat(AdSpaceId)
       include (Id, UserId, LastMessageId, CurrentOfferId)
go

create 
 index idx_Chat_UserId_incl
    on dbo.Chat(UserId)
       include (Id, AdSpaceId, LastMessageId, CurrentOfferId)
go


create 
 --drop
 table dbo.MessageDeliveredStatus
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_MessageDeliveredStatus
 primary key
 constraint df_MessageDeliveredStatus_pk
    default newsequentialid(),

 Title nvarchar(20) not null,
 Description nvarchar(50) not null,

 Synonym varchar(255)
)
go

create
unique
 index idx_MessageDeliveredStatus_Synonym_incl
    on dbo.MessageDeliveredStatus(Synonym) 
	   include (Id, Title, Description)
 where Synonym is not null
go

/*
delete from dbo.MessageDeliveredStatus
go
*/
insert
  into dbo.MessageDeliveredStatus (Id, Title, Description, Synonym)
select 'B8FEEE09-2709-421A-9111-AC18B2A1B59A', 'Sent',  'Message was sent to 2nd (all) other(s) user(s)', 'WasSent'
union all
select '6C807B96-1B4C-43A6-9BEF-0F46C0247423', 'Was read',  'User has read message', 'WasRead'
union all
select '6E84260A-E5F8-4880-9DE1-ED0945E90BEB', 'Not Sent',  'User has read message', 'WasNotSent'
go

select * from dbo.MessageDeliveredStatus;
go
 

create 
 --drop
 table dbo.Message
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_Message
 primary key
 constraint df_Message_pk
    default newsequentialid(),

 ChatId uniqueidentifier not null
 constraint fk_Message_ChatId
 references dbo.Chat(Id),

 --We can start to chat when want to change Rate or we need to setup the same reate. Correct?
 --Rate money not null,
 
 --Whe timestamp (not date)
 --names recieveDate, sentDate are better
 --But in any case single column with this datatype is possible
 --WhenCame timestamp not null,
 --WhenSent timestamp not null,
 ReceivedDate datetimeoffset not null,
 SentDate datetimeoffset not null, --withot timezone, UTC0
 

 Message nvarchar(300), -- not null, Is it possble that message is empty?

 --What about timezones?
 --they may change dates also
 --StartRent, EndRent are better
 --RentSince datetimeoffset,
 --RentTill datetimeoffset,

 /*
 StatusId uniqueidentifier not null --We need some default Status and mwthod how we can get it.
 constraint fk_Message_StatusId
 foreign key
 references dbo.MessageOfferStatus(Id),
 */
 --Actually we already have two persons who changes and we enoght to know direction or who sent message or target user
 --Also I still think that we need to separated Messages and Offrece and Contracts
 --in this case we will need to repeat Rate each times when owner of space answers

 SenderId uniqueidentifier not null
 constraint fk_Message_SenderId 
 foreign key 
 references dbo.[User](Id),

 /*
 RecepientId uniqueidentifier not null
 constraint fk_Message_RecepientId 
 foreign key 
 references dbo.[User](Id)
 */

 DeliveredStatusId uniqueidentifier not null --We need some default Status and mwthod how we can get it.
 constraint fk_Message_DeliveredStatusId
 foreign key
 references dbo.MessageDeliveredStatus(Id)
)
go

alter table dbo.Chat
 add 
 --drop
      constraint fk_Chat_LastMessageId
 foreign key (LastMessageId)
 references dbo.Message(Id)
go

/*
May be need these indexes but it is actuall IF WE NEED this columns
Usually we need to know who sent message in chat
*/
/*
create 
--drop
 index idx_Message_RecepientId_SentDate_incl
    on dbo.Message(RecepientId, SentDate)
       include (Id, ChartId, Rate, ReceivedDate, Message, RentSince, RentTill, StatusId, SenderId)
go
*/
create 
 index idx_Message_SenderId_SentDate_incl
    on dbo.Message(SenderId, SentDate)
       include (Id, ChatId, /*Rate,*/ ReceivedDate, Message, /*RentSince, RentTill, StatusId,*/ DeliveredStatusId/*, RecepientId*/)
go

create
-- drop 
 index idx_Message_ChatId_incl
    on dbo.Message(ChatId)
       include (Id, /*Rate,*/ ReceivedDate, SentDate, Message, /*RentSince, RentTill, StatusId,*/ SenderId, DeliveredStatusId/*, RecepientId*/)
go

create 
 --drop
 table dbo.MessageOffer
(
 Id uniqueidentifier not null rowguidcol
 
 constraint pk_MessageOffer
 primary key
 constraint df_MessageOffer_pk
    default newsequentialid()
 
 constraint fk_MessageOffer_pk
 foreign key 
 references dbo.Message(Id)
 ,

 --We can start to chat when want to change Rate or we need to setup the same reate. Correct?
 Rate money not null,
 
 --What about timezones?
 --they may change dates also
 --StartRent, EndRent are better
 RentSince datetimeoffset,
 --RentTill datetimeoffset,

 StatusId uniqueidentifier not null --We need some default Status and mwthod how we can get it.
 constraint fk_MessageOffer_StatusId
 foreign key
 references dbo.MessageOfferStatus(Id),

 RentPeriodId uniqueidentifier not null
      constraint fk_MessageOffer_RentPeriodId 
 foreign key(RentPeriodId) 
 references dbo.RentPeriodType(Id)
)
go

alter table dbo.Chat
  add constraint fk_Chat_CurrentOfferId 
      foreign key (CurrentOfferId)
      references dbo.MessageOffer(Id)
go

create table ChatMember 
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_ChatMember 
 primary key
 constraint df_ChatMember_pk
    default newsequentialid(),
 
 ChatId uniqueidentifier not null
 constraint fk_ChatMember_ChatId 
 foreign key
 references [dbo].[Chat] (Id),

 UserId uniqueidentifier not null
 constraint fk_ChatMember_UserId
 foreign key
 references [dbo].[User] (Id),

 AmountUnreadMessages int not null 
 constraint df_ChatMember_AmountUnreadMessages
 default (0)
)
go

/*
  alter table ChatMember 
   with nocheck
    add constraint  uk_ChatMember_ChatId_UserId
  unique (ChatId, UserId)    
go
*/
/*
I don't create unique constriant, I create UNIQUE index because it has INCLUDE part
I am not usre about GOOD order, we can query data by ChatId or by UserId only and I don't know what is more important.
May be we will create additional index in future or change this index
*/
 create
 unique
   --drop
  index idx_ChatMember_ChatId_UserId
     on ChatMember (ChatId, UserId)
include (Id, AmountUnreadMessages) --all fields should be included.
go

/*
--We will use new table
insert
  into MessageOfferStatus (Id, Title, Description, Synonym)
values ('B8FEEE09-2709-421A-9111-AC18B2A1B59A', 'Unread',  'Message was not readed by 2nd (all) other(s) user in chat', 'UNREAD')
go
*/


/*
I am not sure that it's good idea to mix user rating and owner of space rating 
(he may be renter also for another case).

Also it's good idea to have rating of space (and dictionary of spaces).

This rating should be related to event of rent and has exact link to "Message or chat or some Contract/Order".

Also we need avg(Rank) and count(1) on User (or another) table level.
*/
create 
 --drop
 table dbo.Rating --dbo.UserRating would be better
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_Rating
 primary key
 constraint df_Rating_pk
    default newsequentialid(),
 

 Rank int not null,

 ReviewerId uniqueidentifier not null
 constraint fk_Rating_ReviewerId
 foreign key
 references dbo.[User](Id),

 RatedUserId uniqueidentifier not null
 constraint fk_Rating_RatedUserId
 foreign key
 references dbo.[User](Id),

 Message nvarchar(300),

 CreatedDate datetime not null 
 constraint df_Rating_CreatedDate
 default ToDateTimeOffset(getutcdate(), '+00:00')


)
go

--I am not sure whether we need index by ReviewerId (if it wants to search own reviews) 
--but we need to search by RatedUserId to compute AVG value and to display reviews
create 
 index idx_Rating_RatedUserId_incl
    on dbo.Rating(RatedUserId, CreatedDate desc)
       include (Id, Rank, ReviewerId, Message)
go


create 
 --drop
 table dbo.Invoice
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_Invoice
 primary key
 constraint df_Invoice_pk
    default newsequentialid(),
 
 --We can not create two column with Timestamp datatype
 --CreatedDate, ProceccedDate looks better
 CreatedDate datetimeoffset not null 
 constraint df_Invoice_CreatedDate 
 default ToDateTimeOffset(getutcdate(), '+00:00'), --may be we need to change to got UTC0 value

 ProcessedDate datetimeoffset,

 Amount money not null,


 SpaceId uniqueidentifier not null
 constraint fk_Invoice_SpaceId
 foreign key
 references dbo.Space(Id),

 SenderId uniqueidentifier not null
 constraint fk_Invoice_SenderId
 foreign key
 references dbo.[User](Id),

 --not sure about nullable for both columns
 RecepientToken nvarchar(50),
 SenderToken nvarchar(50)

)
go

create 
 index idx_Invoice_SpaceId_incl
    on dbo.Invoice(SpaceId)
       include (Id, CreatedDate, ProcessedDate, Amount, SenderId, RecepientToken, SenderToken)
go

create 
 index idx_Invoice_SenderId_incl
    on dbo.Invoice(SenderId)
       include (Id, CreatedDate, ProcessedDate, Amount, SpaceId, RecepientToken, SenderToken)
go

create 
 --drop
 table dbo.Receipt
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_Receipt
 primary key
 constraint df_Receipt_pk
    default newsequentialid(),
 
 InvoiceId uniqueidentifier not null
 constraint fk_Receipt_InvoiceId
 foreign key
 references dbo.Invoice(Id),

 --We can not create two column with Timestamp datatype
 --CreatedDate, ProceccedDate looks better
 CreatedDate datetimeoffset not null 
 constraint df_Receipt_CreatedDate 
 default ToDateTimeOffset(getutcdate(), '+00:00'), --may be we need to change to got UTC0 value

 SentDate datetimeoffset,

 StripeTransactionId nvarchar(50)
)
go

create 
 index idx_Receipt_InvoiceId_incl
    on dbo.Receipt(InvoiceId)
       include (Id, CreatedDate, SentDate, StripeTransactionId)
go


create 
 --drop
 table dbo.TrainingSet
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_TrainingSet
 primary key
 constraint df_TrainingSet_pk
    default newsequentialid(),
 
 --looks like link to dictionary
 --why dbo.Zip has column with name "Rank" ?
 Size int not null,

 --We need link to ZIP code
 --and FK constaint
 ZipId uniqueidentifier not null
 constraint fk_TrainingSet_ZipId 
 foreign key 
 references dbo.Zip(id)
 ,

 --looks like enum. 
 --if we don't have WEB-server code, how can we define what 1 or 2 value means?
 Type int not null,
 Accessebility int not null,
 
 Rate money not null
)
go

--Indexes for dbo.TrainingSet will be created late (we have questions about Zip and I don't see optimal index for now, I need to see queries).

create schema misc;
go

create table dbo.[SpaceAddress]
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_SpaceAddress
 primary key
 constraint fk_SpaceAddress_Id 
 foreign key
 references dbo.Space(Id),
 /*
 ID must be THE SAME as Id in Space
 constraint df_Address_pk
    default newsequentialid(),
 */
 CountryLongName nvarchar(50),
 CountryShortName nvarchar(3),
 AreaLongName nvarchar(50),
 AreaShortName nvarchar(50),
 SubAreaLongName nvarchar(50),
 SubAreaShortName nvarchar(50),
 LocalityLongName nvarchar(50),
 LocalityShortName nvarchar(50),
 SubLocalityLongName nvarchar(50),
 SubLocalityShortName nvarchar(50),
 StreetLongName nvarchar(50),
 StreetShortName nvarchar(50),
 HouseNumber nvarchar(15),
 
 ZipId uniqueidentifier not null
 constraint fk_SpaceAddress_ZipId 
 foreign key 
 references dbo.Zip(Id),
)
go


--CODE PART !

--This schema will be used like name spaces 
--I did it at first just for tests and investigations because I don't want to hard-code synonyms for example
--and
create schema ns;
go

--Author: Nikita Pochechuev
--Description: Fuction was created for trMessageCounters
--and it's the simplest version 

/*
--Test case
select dbo.fnMessageDeliveredStatusGetId('WasRead')
*/

create
 --drop
 function dbo.fnMessageDeliveredStatusGetId(@Synonym varchar(255)) 
returns uniqueidentifier
begin
 
 return (
 select id
   from [dbo].MessageDeliveredStatus mds
  where mds.Synonym = @Synonym)

end;
go

-- =============================================
-- Author:		Nikita Pochechuev
-- Create date: 2016-02-02
-- Description:	It's test fucntions that will work like "Enum" in C# code and map statuses to table
-- =============================================

/*
-- Test case
select * from ns.fnMessageDeliveredStatus()
*/
create 
--alter
function ns.fnMessageDeliveredStatus
(	
)
returns table
as
return
(
 select dbo.fnMessageDeliveredStatusGetId('WasRead') as WasRead,
        dbo.fnMessageDeliveredStatusGetId('WasSent') as WasSent,
		dbo.fnMessageDeliveredStatusGetId('WasNotSent') as WasNotSent
)
GO


-- Author:		Nikita Pochechuev
-- Create date: 2016-02-02

-- Description: This triger updates countes (for now unreaded messages for user) 
-- in [dbo].[ChatMember] after changes in [dbo].[Message]
-- Right now it has the simplest logic because we don't have separated status for each User-message 
-- May be we will define messages that were read by data as workaroud in the future
-- =============================================
create trigger dbo.trMessageCounters
    on [dbo].[Message]
 after insert, delete, update --right now we recompute after ALL events
AS 
BEGIN 
 set nocount on;
 print 'trMessageCounters was started';

 if not(update(DeliveredStatusId))
 begin
  print 'trMessageCounters datachanges were ignored';
  return;
 end;

--Unortunarly we use ORM and don't create functions
--this function was created as "fast version"
 

 with cteChangedRows as
 (
 select distinct --actually this operator is redundand, it decrease perfromance but it may be usefill if someone do big direct update/isert for this table 
        coalesce(i.ChatId, d.ChatId) changedChatID, 
        coalesce(i.SenderId, d.SenderId) SenderID --we need to exclude sender
   from ns.fnMessageDeliveredStatus() ms

  inner
   join inserted i
   full 
   join deleted d
     on d.Id = i.Id
     
	 on  (   
	         ms.WasRead <> d.DeliveredStatusId 
	      or ms.WasRead <> i.DeliveredStatusId
		 ) --we need to compute and recompute messages that were not read.
	and (   
	        d.DeliveredStatusId <> i.DeliveredStatusId 
	     or d.DeliveredStatusId is null --inserted messages
		 or i.DeliveredStatusId is null --deleted messages 
		)   
 ),

 cteComputeMessages as
 (
 
 select --This logic will work for two users ONLY
        --when 
        chR.changedChatID,
        chR.SenderID,
		count(1) as cnt
   from dbo.Message msg with(nolock)
  cross
   join ns.fnMessageDeliveredStatus() ms --may be better way is to seletet value to variable because we use it more than once.

  inner
   join cteChangedRows chR
     on chR.changedChatID = msg.ChatId
	and chR.SenderID <> msg.SenderId --we exclude senders
	and msg.DeliveredStatusId <> ms.WasRead
   
  group
     by chR.changedChatID, chR.SenderID

 ),

 --it includes owner of Chat (Chat.UserId)
 --and owner of Space (Chat.AdSpace > AdSpace.SpaceId > Space.UserID
 cteComputedWithUser as
 (
 select cm.changedChatID,
        coalesce(s.UserId, ch.UserId) as UserId,  --we need one ID only!!!
        cm.cnt
   from cteComputeMessages cm
  inner
   join dbo.Chat ch with(nolock)
     on ch.Id = cm.changedChatID
   left
   join dbo.AdSpace adS with(nolock)
   left
   join dbo.Space s with(nolock)
     on s.Id = adS.SpaceId
     on adS.Id = ch.AdSpaceId
	and ch.UserId = cm.changedChatID --We need to check level up if users were the same on this level
 )

 --User + Chat will be inserted when the is the result in agreaget function ONLY
 merge [dbo].[ChatMember] as target
 using cteComputedWithUser as source
    on target.ChatId = source.changedChatID
   and target.UserId = source.UserId
 
  when matched 
  then update set target.AmountUnreadMessages = source.cnt
 
  when not matched
  then insert (ChatId, UserId, AmountUnreadMessages)
       values (source.changedChatID, source.UserId, source.cnt);

 print 'trMessageCounters was finished';

END
GO


/*
Author: Nikita Pochechuev

--Description:
This trigger computes avg Rate and count of rates for user-s
--This trig

--Test case 

begin tran

declare @idRating uniqueidentifier, @IdUser uniqueidentifier ;
select top 1 @idRating = t.Id, @IdUser = t.RatedUserId from [dbo].[Rating] t

select [AvgRate], CountRating from [dbo].[User] where Id = @IdUser;

update t set t.[Rank] = t.[Rank] + 1 from [dbo].[Rating] t where t.Id = @idRating

select [AvgRate], CountRating from [dbo].[User] t where t.Id = @IdUser;

rollback tran;
*/
create trigger trRating_computeAvgRatingAndCountForUser
    on [dbo].[Rating] after insert, update, delete
as
begin

 set nocount on;
 if not(update(Rank))
 begin
  print 'trRating_computeAvgRatingAndCountForUser exit without changes';
  return;
 end;

 print 'trRating_computeAvgRatingAndCountForUser was started';

 with cteUpdate as
 (
 select distinct
        coalesce(i.[RatedUserId], d.[RatedUserId]) as [RatedUserId]
   from inserted i
   full
   join deleted d
     on d.Id = i.Id
  where i.Rank <> d.Rank
     or i.Id is null
	 or d.Id is null
 ),

 cteCompute as
 (
 select r.RatedUserId,
        avg(cast(r.Rank as numeric(4, 2))) as AvgRate,
		count(1) as CountRating
   from cteUpdate u
  inner
   join dbo.Rating r with(nolock)
     on u.RatedUserId = r.RatedUserId
  group
     by r.RatedUserId
 )

 update u  
    set u.AvgRate = c.AvgRate,
	    u.CountRating = c.CountRating
   from cteCompute c
  inner
   join dbo.[User] u
     on c.RatedUserId = u.Id;

 print 'trRating_computeAvgRatingAndCountForUser was finished';

end;