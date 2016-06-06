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

 Password varbinary(32) not null, --I am not sure about "not null" but it's very possible

 Salt varbinary (32) not null,

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

 AvatarLink nvarchar(256)

)
go

create 
unique
 index idx_User_Email_incl
    on dbo.[User](email)
	   include (Id, Password, Salt, Phone/*, IsPhoneVerified*/, Lastname, Firstname, PaymentSystemSecret, AvgRate, PhoneVerificationStatusID, AvatarLink)
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
       include (Id, Password, Salt, Phone/*, IsPhoneVerified*/, Lastname, Firstname, PaymentSystemSecret)
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
 table dbo.MessageStatus
(
 Id uniqueidentifier not null rowguidcol
 constraint pk_MessageStatus 
 primary key
 constraint df_MessageStatus_pk
    default newsequentialid(),

 Title nvarchar(20) not null,
 Description nvarchar(50) not null,

 Synonym varchar(255)
)
go

create
unique
 index idx_MessageStatus_Synonym_incl
    on dbo.MessageStatus(Synonym) 
	   include (Id, Title, Description)
 where Synonym is not null
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
 

 Zip nvarchar(10),

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
 references [dbo].[RootDictionary] (Id)
)
go

--I suppose that we will search Spaces by user, type, city... and some other attributes but correct indexes will depends on future queries
--Actually this table (and related  dbo.AdSpace) will have the biggest amout of indxes
create
 index idx_Space_UserId_incl
    on dbo.Space(UserId)
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
)
go

create 
 index idx_Chat_AdSpaceId_incl
    on dbo.Chat(AdSpaceId)
       include (Id, UserId, LastMessageId)
go

create 
 index idx_Chat_UserId_incl
    on dbo.Chat(UserId)
       include (Id, AdSpaceId, LastMessageId)
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
 Rate money not null,
 
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
 RentSince datetimeoffset,
 RentTill datetimeoffset,

 StatusId uniqueidentifier not null --We need some default Status and mwthod how we can get it.
 constraint fk_Message_StatusId
 foreign key
 references dbo.MessageStatus(Id),

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
)
go

alter table dbo.Chat
 add 
 --drop
      constraint fk_Chat_LastMessageId
 foreign key (LastMessageId)
 references dbo.Message(Id)
go

create
-- drop 
 index idx_Message_ChatId_incl
    on dbo.Message(ChatId)
       include (Id, Rate, ReceivedDate, SentDate, Message, RentSince, RentTill, StatusId, SenderId/*, RecepientId*/)
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
       include (Id, ChatId, Rate, ReceivedDate, Message, RentSince, RentTill, StatusId/*, RecepientId*/)
go

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