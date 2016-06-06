Truncate Table dbo.UserDevice 
go

Alter Table dbo.UserDevice 
  Add [TopicEndpoint] nvarchar(256) Not Null
      constraint uk_UserDevice_TopicEndpoint 
	  unique
go