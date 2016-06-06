alter table dbo.Message
  add constraint df_Message_SentDate
  default dbo.fnGetUtc0DateWithOffset()
  for SentDate
go

alter table [dbo].[MessageOfferHistory]
  add constraint df_MessageOfferHistory_ChangedStatusDate
  default dbo.fnGetUtc0DateWithOffset()
  for ChangedStatusDate
go