If Exists (Select * From INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS Where CONSTRAINT_NAME = N'fk_Message_CurrentOfferId')
Begin
	Alter Table Chat Drop Constraint fk_Message_CurrentOfferId	
End

If Exists (Select * From INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS Where CONSTRAINT_NAME = N'fk_Chat_CurrentOfferId')
Begin
	Alter Table Chat Drop Constraint fk_Chat_CurrentOfferId	
End

If Exists (Select * From INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS Where CONSTRAINT_NAME = N'fk_Chat_LastMessageId')
Begin
	Alter Table Chat Drop Constraint fk_Chat_LastMessageId
End

If Exists (Select * From INFORMATION_SCHEMA.COLUMNS Where COLUMN_NAME = N'LastMessageId')
Begin
	Drop Index Chat.idx_Chat_SpaceId_incl
	Drop Index Chat.idx_Chat_UserId_incl
	Alter Table Chat Drop Column LastMessageId
End

If Exists (Select * From INFORMATION_SCHEMA.COLUMNS Where COLUMN_NAME = N'CurrentOfferId')
Begin
	Alter Table Chat Drop Column CurrentOfferId
End
go

create 
 index idx_Chat_SpaceId_incl
    on dbo.Chat (SpaceId)
       include (Id, UserId)
go

create 
 index idx_Chat_UserId_incl ON dbo.Chat(UserId)
include (Id, SpaceId) 
go


