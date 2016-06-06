/*
select * from [dbo].[Zip]
where zip is null
*/

alter table [dbo].[Zip]
 alter column zip nvarchar(10) not null
go

create 
-- drop
 index idx_Zip_Zip
    on [dbo].[Zip] (zip)
 include (Id, Rank)
go

