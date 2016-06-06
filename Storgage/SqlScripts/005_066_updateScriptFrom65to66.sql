ALTER TABLE [Space] ADD LastModified DATETIME NOT NULL DEFAULT GETUTCDATE()
GO


CREATE SPATIAL INDEX [sp_idx_Filter_BoundingBox] ON [dbo].[Filter]
(
	[BoundingBox]
)USING  GEOGRAPHY_GRID 
WITH (GRIDS =(LEVEL_1 = MEDIUM,LEVEL_2 = MEDIUM,LEVEL_3 = MEDIUM,LEVEL_4 = MEDIUM), 
CELLS_PER_OBJECT = 16, PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


/*
Autor: Slava Muhtarjants

Description: This trigger updates the LastModified column of the Space table

according to: https://weezlabs.atlassian.net/browse/STOR-513

This column is updated each time the values of SpaceAccessTypeID, SpaceTypeID,
SizeTypeID, Location, Rate and AvailableSince columns are changed.

*/
CREATE trigger [dbo].[tr_Space_setLastModified]
    on [dbo].[Space]
	after update
as
begin
  
	print 'Start trigger tr_Space_setLastModified';

	if not(update(SizeTypeId) or update(SpaceAccessTypeId) or update(Rate) or update(AvailableSince) or update(Location))
	begin
	 return;
	 print 'Return from tr_Space_setLastModified without chagnes'
	end;

	declare @utcDate datetime = getutcdate();

	update s
	   set s.LastModified = @utcDate
	  from inserted i 
	 inner 
	  join deleted d 
	    on i.Id = d.Id
	   and (
	        d.SizeTypeId != i.SizeTypeId
		 or d.SpaceAccessTypeId != i.SpaceAccessTypeId
		 or d.SpaceTypeId != i.SpaceTypeId
		 or d.Rate != i.Rate
		 or d.AvailableSince != i.AvailableSince
		 or d.Location.STEquals(i.Location) = 0
		   )
	  inner 
	   join dbo.Space s
	     on s.Id = i.Id

	print 'Finish trigger tr_Space_setLastModified';

end