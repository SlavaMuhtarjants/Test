IF EXISTS(
  SELECT TOP 1 1
  FROM INFORMATION_SCHEMA.COLUMNS
  WHERE 
    [TABLE_NAME] = 'Message'
    AND [COLUMN_NAME] = 'SenderId')
Begin
	EXEC sp_rename 'Message.SenderId', 'UserId', 'COLUMN'
End
go

exec sp_rename N'dbo.Message.idx_Message_SenderId_SentDate_incl', N'idx_Message_UserId_SentDate_incl', N'INDEX';
go

--fk_Message_SenderId
exec sp_rename 'dbo.fk_Message_SenderId', 'fk_Message_UserId';
go