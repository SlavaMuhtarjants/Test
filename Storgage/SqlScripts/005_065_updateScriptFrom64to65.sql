ALTER TABLE Filter 
  ADD Location nvarchar(100) NOT NULL
  constraint df_Filter_Location 
  DEFAULT '';
​
ALTER TABLE Filter 
  ADD ZipCodeId uniqueidentifier NULL
  CONSTRAINT FK_Filter_ZipCodeId
  FOREIGN KEY(ZipCodeID)
  REFERENCES Zip ([Id])
GO
​
create 
 index idx_Filter_UserId
    on dbo.Filter(UserId)
       include (Id, BoundingBox, MinPrice, MaxPrice, RentStartDate, CheckSizeType, CheckType, CheckAccessType)

GO

ALTER TABLE Filter 
DROP CONSTRAINT df_Filter_Location
GO