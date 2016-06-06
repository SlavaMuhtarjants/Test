-- drop indexes as the BoundingBox column is going to be altered

DROP INDEX [idx_Filter_UserId] ON [dbo].[Filter]
DROP INDEX [sp_idx_Filter_BoundingBox] ON [dbo].[Filter]
GO

delete FilterRootDictionary from 
	FilterRootDictionary as frd
inner join
	Filter as f on f.Id = frd.FilterId
where
	f.BoundingBox is null

delete from Filter where BoundingBox is null

alter table Filter alter column BoundingBox geography not null

-- recreate indexes

CREATE NONCLUSTERED INDEX [idx_Filter_UserId] ON [dbo].[Filter]
(
	[UserId] ASC
)
INCLUDE 
(
	[Id],
	[BoundingBox],
	[MinPrice],
	[MaxPrice],
	[RentStartDate],
	[CheckSizeType],
	[CheckType],
	[CheckAccessType]
)

CREATE SPATIAL INDEX [sp_idx_Filter_BoundingBox] ON [dbo].[Filter]
(
	[BoundingBox]
)USING  GEOGRAPHY_GRID 
WITH 
(
	GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM), 
	CELLS_PER_OBJECT = 16
) ON [PRIMARY]
GO