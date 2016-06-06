DELETE FROM RootDictionary
DELETE FROM FilterRootDictionary
DELETE FROM FilterCondition
DELETE FROM Filter
GO

-- data structure changes

ALTER TABLE dbo.FilterRootDictionary DROP CONSTRAINT fk_FilterRootDictionary_SizeTypeId
ALTER TABLE dbo.FilterRootDictionary DROP CONSTRAINT fk_FilterRootDictionary_FilterId
GO
DROP INDEX idx_FilterRootDictionary_FilterId_incl ON dbo.FilterRootDictionary
ALTER TABLE dbo.FilterRootDictionary DROP COLUMN FilterId
GO

ALTER TABLE dbo.FilterCondition DROP CONSTRAINT fk_FilterCondition_FilterId
ALTER TABLE dbo.FilterCondition	DROP COLUMN FilterId
DROP TABLE Filter
DROP TABLE FilterCondition
GO

ALTER TABLE FilterRootDictionary ADD FilterId uniqueidentifier NOT NULL
GO

CREATE TABLE [dbo].[Filter]
(
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[BoundingBox] [geography] NULL,
	[MinPrice] [money] NOT NULL,
	[MaxPrice] [money] NOT NULL,
	[RentStartDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Filter] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Filter] ADD  CONSTRAINT [DF_Filter_Id]  DEFAULT (newsequentialid()) FOR [Id]
GO

ALTER TABLE dbo.FilterRootDictionary ADD CONSTRAINT	FK_FilterRootDictionary_Filter FOREIGN KEY
	(
		FilterId
	) REFERENCES dbo.Filter
	(
		Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.Filter ADD CONSTRAINT FK_Filter_User FOREIGN KEY
	(
		UserId
	) REFERENCES dbo.[User]
	(
		Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE dbo.FilterRootDictionary ADD CONSTRAINT	FK_FilterRootDictionary_RootDictionary FOREIGN KEY
	(
		RootDictionaryId
	) REFERENCES dbo.RootDictionary
	(
		Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
GO

-- new lookup data insertion
INSERT INTO RootDictionary
SELECT Id from SizeType
UNION ALL
SELECT Id from SpaceType
UNION ALL
SELECT Id from SpaceAccessType