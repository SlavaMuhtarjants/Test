--Author: Alexander P.
--Edited: Nikita Pochechuev

Alter Table MessageOffer 
  Add [StopAt] datetimeoffset, 
      [RentSince] datetimeoffset Not Null 
	  constraint df_MessageOffer_RentSince 
      default ToDateTimeOffset(getutcdate(), '+00:00')
go

Alter Table MessageOfferHistory
 Drop Column ExecutionDate
go

Insert Into MessageOfferStatus(Title, [Description], [Synonym])
Values('Expired','Offer was expired', 'Expired')
Insert Into MessageOfferStatus(Title, [Description], [Synonym])
Values('Stopped', 'Offer was stopped', 'Stopped')
go
