/*
Author: Nikita Pochechuev

Description:
 Function takes 101 recent rows from Space table by Zip and SizeType
 and computes PERCENTILE_DISC

declare
 @SizeTypeId uniqueidentifier,
 @ZipId uniqueidentifier;


select
 @SizeTypeId = 'B13495F2-DA94-421B-8EBF-2266900A86B0',
 @ZipId = 'F991530A-96C3-E511-8271-D8CB8A7F42F1';

select dbo.fnSpaceForecastRate(@SizeTypeId, @ZipId) as singleValue



--find Id-s for test
select s.SizeTypeId,
	   s.ZipId,
	   count(1) as cnt,
	   count(distinct Rate) as distRate,
	   dbo.fnSpaceForecastRate(s.SizeTypeId, s.ZipId) --Performacne is not good
  from dbo.Space s
 group
    by s.SizeTypeId,
	   s.ZipId
 order
    by distRate desc, cnt desc


*/

create function dbo.fnSpaceForecastRate
(
 @SizeTypeId uniqueidentifier,
 @ZipId uniqueidentifier
) returns money
as
begin
/*
--CTE can't be used WITHOUT variable

with cte as
(
select top 101 
       s.SizeTypeId,
	   s.ZipId,
	   s.Rate	   
  from dbo.Space s
 where s.SizeTypeId = @SizeTypeId
   and s.ZipId = @ZipId
   and s.Rate > 0
 order
    by s.LastModified desc
)
*/

return (

select top 1
       PERCENTILE_DISC(0.5) WITHIN GROUP (ORDER BY s.Rate) OVER () AS MedianDisc
  from 
(
select top 101 
       s.SizeTypeId,
	   s.ZipId,
	   s.Rate	   
  from dbo.Space s
 where s.SizeTypeId = @SizeTypeId
   and s.ZipId = @ZipId
   and s.Rate > 0
 order
    by s.LastModified desc
) s

)
end;

go


create
 index idx_Space_ZipId_SizeTypeId_Rate_LastModified
    on dbo.Space (SizeTypeId, ZipId, Rate, LastModified desc)
        include (Id, UserId, SpaceAccessTypeId, Title, Decription, IsListed, SpaceTypeId, Location, DefaultPhotoID, IsDeleted, createdDate, FullAddress, ShortAddress, AvailableSince)
go

/*
Aututhor: Nikita Pochechuev

Description
This funciton was created because EF doesn't work with functions good
It must be used for single execution only!

--Test case
declare
 @SizeTypeId uniqueidentifier,
 @ZipCode nvarchar(10),
 @ForecastedRate money;


exec dbo.spSpaceGetForecastRate
 @SizeTypeId = 'B13495F2-DA94-421B-8EBF-2266900A86B0',
 @ZipCode = '90401',
 @ForecastedRate = @ForecastedRate output;

select @ForecastedRate;
*/

create procedure dbo.spSpaceGetForecastRate
(
 @SizeTypeId uniqueidentifier,
 @ZipCode nvarchar(10),
 @ForecastedRate money = null output
)
as
begin

 select @ForecastedRate = dbo.fnSpaceForecastRate(@SizeTypeId, z.Id)
   from dbo.Zip z with(nolock)
 where z.Zip = @ZipCode

end
go

--dbo.fnSpaceForecastReate
/*
Author: Nikita Pochechuev

Description:
 Function takes 101 recent rows from Space table by Zip and SizeType
 and computes PERCENTILE_DISC

declare
 @SizeTypeId uniqueidentifier,
 @ZipId uniqueidentifier;


select
 @SizeTypeId = 'B13495F2-DA94-421B-8EBF-2266900A86B0',
 @ZipId = '03261608-9BF5-E511-BEBE-6C71D91FABB6';

select * from dbo.fnSpaceForecastRateTable(@SizeTypeId, @ZipId) t



with cte as
(
select distinct
       s.SizeTypeId,
	   s.ZipId
  from dbo.Space s
 group
    by s.SizeTypeId,
	   s.ZipId
)

select c.SizeTypeId,
       c.ZipId,
	   t.*
  from cte c
 cross
 apply [dbo].[fnSpaceForecastRateTable] (c.SizeTypeId, c.ZipId) t


*/

create function [dbo].[fnSpaceForecastRateTable]
(
 @SizeTypeId uniqueidentifier,
 @ZipId uniqueidentifier
) RETURNS TABLE 
AS

/*
--CTE can't be used WITHOUT variable

with cte as
(
select top 101 
       s.SizeTypeId,
	   s.ZipId,
	   s.Rate	   
  from dbo.Space s
 where s.SizeTypeId = @SizeTypeId
   and s.ZipId = @ZipId
   and s.Rate > 0
 order
    by s.LastModified desc
)
*/

return (

select top 1
       PERCENTILE_DISC(0.5) WITHIN GROUP (ORDER BY s.Rate) OVER () AS MedianDisc,
	   count(1) over() as cntRate
  from 
(
select top 101 
       s.SizeTypeId,
	   s.ZipId,
	   s.Rate	   
  from dbo.Space s
 where s.SizeTypeId = @SizeTypeId
   and s.ZipId = @ZipId
   and s.Rate > 0
 order
    by s.LastModified desc
) s


)

go