drop index idx_User_Email_unique_incl 
  on dbo.[User]
go

exec sp_rename N'dbo.[User].idx_User_Email_incl', N'idx_User_Email_unique_incl', N'INDEX';
go