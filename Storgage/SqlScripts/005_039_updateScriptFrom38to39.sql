 /*
 select 
        u.*
  from dbo.[User] u
 order
    by CountRating desc

select * from dbo.Rating 
*/
--cleanup old data, the main part doesn't have ralated data in Rating table
 update u
    set u.AvgRate = null,
	    u.CountRating = 0
   from dbo.[User] u
go

--Fire trigger with two real updates (redundant, update script may be used also)
update t
  set t.Rank = t.Rank + 1
  from dbo.Rating t;
go

update t
  set t.Rank = t.Rank - 1
  from dbo.Rating t;
go