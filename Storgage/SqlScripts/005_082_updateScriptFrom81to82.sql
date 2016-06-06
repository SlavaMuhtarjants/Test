--select * from [dbo].[AbuseTypeDictionary]

insert
  into [dbo].[AbuseTypeDictionary] (Title, Description, Synonym, id)
select 'Bug report', 'Contact us about bug', 'ContactUsBugReport', '33330B13-731E-E611-BEC9-50465D445D31'
union all
select 'Enhancement', 'Contact us about enhancement', 'ContactUsEnhancement', '34330B13-731E-E611-BEC9-50465D445D31'
union all
select 'Other', 'Contact us about enhancement', 'ContactUsOther', '35330B13-731E-E611-BEC9-50465D445D31'
go

alter table dbo.Abuse
  add FileName nvarchar(256)
go

drop INDEX [idx_Abuse_ReporterId] ON [dbo].[Abuse]
go

CREATE INDEX [idx_Abuse_ReporterId] ON [dbo].[Abuse]
(
	[ReporterId] ASC
)
INCLUDE (Id, Message, ReportedAt, FileName)