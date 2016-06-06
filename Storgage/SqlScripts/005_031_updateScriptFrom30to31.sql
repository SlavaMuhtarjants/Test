--If Not Exists (Select * From INFORMATION_SCHEMA.TABLES Where TABLE_NAME = N'MessageOfferHistory')
--Begin
	Create Table MessageOfferHistory(Id Uniqueidentifier RowGuidCol Not Null 
	constraint df_MessageOfferHistory_Id Default  newsequentialid(),
	 
		ExecutionDate DateTimeOffset,
		MessageOfferId Uniqueidentifier Not Null,
		UserId Uniqueidentifier Not Null,
		StatusId Uniqueidentifier Not Null,
		ChangedStatusDate DateTimeOffset Not Null,

		Constraint pk_MessageOfferHistory Primary Key Clustered (Id),
		Constraint fk_MessageOfferHistory_MessageOfferStatus Foreign Key(StatusId) References MessageOfferStatus(Id),
		Constraint fk_MessageOfferHistory_User Foreign Key(UserId) References [User](Id),
		Constraint fk_MessageOfferHistory_MessageOffer Foreign Key(MessageOfferId) References MessageOffer(Id));
go

		Alter Table MessageOffer Drop Column RentSince;
go
		Alter Table MessageOffer Drop Constraint fk_MessageOffer_StatusId;
go
		Alter Table MessageOffer Drop Column StatusId;			
go
--End