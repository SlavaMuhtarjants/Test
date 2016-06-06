--alter table dbo.Rating
  --This column will be renamed to Id and will be used as primary key
--  drop column MessageOfferId
--  add MessageOfferId uniqueidentifier;
--go

--trying to set existing data
--select mo.*
  --from dbo.MessageOffer mo


alter table MessageOffer 
 -- this column must be add to include part of future indexes to imporov query perfromance (select)
  add CurretMessageOfferHistory uniqueidentifier
  constraint fk_MessageOffer_CurretMessageOfferHistory
  foreign key references [dbo].[MessageOfferHistory] (id)
go



/*select mo.*,
	   current_moh.*	  */
--Set current versions according to order logic
update mo 
   set mo.CurretMessageOfferHistory = current_moh.Id
  from dbo.MessageOffer mo
 cross
 apply (select top 1 moh.Id
          from dbo.MessageOfferHistory moh
		 where moh.MessageOfferId = mo.Id
		 order 
		    by moh.Id desc
		) current_moh
go


---Test case for CurretMessageOfferHistory (check execution plan) 
select *
  from dbo.MessageOffer mo
  left
  join  dbo.MessageOfferHistory moh
    on moh.Id = mo.CurretMessageOfferHistory
go

/*create the simples (and enoght for now) trigger to set current value in CurretMessageOfferHistory column*/

/*
Author: Nikita Pochechuev
Description: 
The simplest version of trigger to set dbo.MessageOffer.CurretMessageOfferHistory column after insert to child table

--Test case

declare @MessageOffer uniqueidentifier,
        @CurretMessageOfferHistory uniqueidentifier;
 
select top 1 
       @MessageOffer = mo.Id,
	   @CurretMessageOfferHistory = mo.CurretMessageOfferHistory
  from dbo.MessageOffer mo
where mo.CurretMessageOfferHistory is not null
order by newid();

--test output before changes 
select *
  from dbo.MessageOffer mo
 inner
  join dbo.MessageOfferHistory moh
    on moh.Id = mo.CurretMessageOfferHistory
 where mo.Id = @MessageOffer

begin tran 

--test insert with copy of previous status
insert
  into [dbo].[MessageOfferHistory] (ExecutionDate, MessageOfferId, UserId, StatusId, ChangedStatusDate)
select ExecutionDate, MessageOfferId, UserId, StatusId, dateadd(day, 1, ChangedStatusDate)
  from [dbo].[MessageOfferHistory] moh
 where moh.Id = @CurretMessageOfferHistory;

select *
  from dbo.MessageOffer mo
 inner
  join dbo.MessageOfferHistory moh
    on moh.Id = mo.CurretMessageOfferHistory
 where mo.Id = @MessageOffer

rollback tran;

*/
create 
--alter
trigger tr_MessageOffer_CurretMessageOfferHistory 
on dbo.MessageOfferHistory after insert
as
begin

 print 'tr_MessageOffer_CurretMessageOfferHistory started';
 --We don't compute the lates value because we don't add more than one row for single MessageOffer by single execution
 --select *
 update mo
    set mo.CurretMessageOfferHistory = i.Id
   from inserted i 
  inner
   join dbo.MessageOffer mo
     on i.MessageOfferId = mo.Id;

 print 'tr_MessageOffer_CurretMessageOfferHistory finished';
end;

go

/*
select *
  from dbo.Rating r
 
--Pay attention !!!
--I want to save data that was inserted and I am trying to find at least one MessageOfferHistory for this
--may be with incorrect STATUS !!!
--Also I DON'T Inserted rows to chat and offerers and Ratings that were not matched willbe DELETED
;
*/

--THIS SCRIPT DIDN'T MAP actual values, they will be mapped by random to the last MessageOfferHistory in the chat

/*
with cte as
(
select s.UserId as SpaceOwnerUser,
       
	   ch.UserId as ArendatorUserId,
	   
	   --MessageOfferHistory that will be used
	   moh.Id OfferHistoryId
	    
  from dbo.Space s --space owner will be here
 inner
  join dbo.Chat ch
    on ch.SpaceId = s.Id
 inner
  join dbo.Message m
    on ch.Id = m.ChatId
 inner
  join dbo.MessageOffer mo
    on mo.Id = m.Id
 inner
  join dbo.MessageOfferHistory moh
    on moh.Id = mo.CurretMessageOfferHistory   
 
  --trying to find 2nd user in chat (reviewer) 
  left
  join dbo.ChatMember cm
    on cm.ChatId = ch.Id
   and cm.UserId <> s.UserId --because we need the 2nd user
),

cteOfferHistoryForRating as
(
--remove duplicates 
select c.SpaceOwnerUser,
       c.ArendatorUserId,
	   max(c.OfferHistoryId) as OfferHistoryId --let it be the latest offer
  from cte c
 group
    by c.SpaceOwnerUser,
       c.ArendatorUserId
)

select *
  from cteOfferHistoryForRating c
  left 
  join [dbo].[Rating] r 
    on r.RatedUserId = c.SpaceOwnerUser
   and r.ReviewerId = c.ArendatorUserId
*/

--IT'S BAD that we don't have "case/event of rent" on DB level
--database my have redundand and incorrect data and two chats may have offeres in the same times

--RANDOM MATHCHING !!!

/*

with cte as
(
select ch.Id as ChatId,
       s.UserId as SpaceOwnerUser,       
	   ch.UserId as ArendatorUserId,
	   
	   --MessageOfferHistory that will be used
	   moh.Id OfferHistoryId
	    
  from dbo.Space s --space owner will be here
 inner
  join dbo.Chat ch
    on ch.SpaceId = s.Id
 inner
  join dbo.Message m
    on ch.Id = m.ChatId
 inner
  join dbo.MessageOffer mo
    on mo.Id = m.Id
 inner
  join dbo.MessageOfferHistory moh
    on moh.Id = mo.CurretMessageOfferHistory   
 
),

cteOfferHistoryForRating as
(
--remove duplicates 
select c.ChatId,
       c.SpaceOwnerUser,
       c.ArendatorUserId,
	   max(c.OfferHistoryId) as OfferHistoryId, --let it be the latest offer,
	   row_number() over (order by c.ChatId) rn
  from cte c
 group
    by c.ChatId,
	   c.SpaceOwnerUser,
       c.ArendatorUserId
)

select *,
       row_number() over (partition by c.ChatId order by
       case
	    when r.RatedUserId = c.SpaceOwnerUser and r.ReviewerId = c.ArendatorUserId then 1
		when r.RatedUserId = c.SpaceOwnerUser then 2
		when r.ReviewerId = c.ArendatorUserId then 3
		else 4
	  end) as mathchPriority --it doesn't gurantee that the same rating may be matched more that one times
  from cteOfferHistoryForRating c
 inner 
  join (select r.*, ROW_NUMBER() over (order by r.Id) as rn from [dbo].[Rating] r) r 
    on r.rn = c.rn



--IT'S BAD that we don't have "case/event of rent" on DB level
--database my have redundand and incorrect data and two chats may have offeres in the same time
*/

/*
alter table dbo.Rating
  --drop column MessageOfferHistoryID
  add MessageOfferHistoryID uniqueidentifier;
  --constraint will be created when column is renamed to Id (or values will be copied and this column will be deleted)
*/
go

--WE NEED some data for testing, at least "random"
--they will be used late in this script
select * into #Rating from dbo.Rating
go

truncate table dbo.Rating
go

--Right now it's extension for [dbo].[MessageOfferHistory] table 
--we can set Rating to finished MessageOffer only
--better name for this is "MessageOfferHistoryRating" (actually MessageOfferHistory is not good name)
alter table dbo.Rating
  add constraint fk_Rating_pk
  foreign key (Id)
  references dbo.MessageOfferHistory (Id)
go

drop index [idx_Rating_RatedUserId_incl]
  on dbo.Rating
go

--we don't need links to users anymore 
--[ReviewerId] - Chat.UserId
--[RatedUserId] - Space.UserId
alter table dbo.Rating
 drop constraint fk_Rating_ReviewerId
go

alter table dbo.Rating
 drop column ReviewerId
go

alter table dbo.Rating
 drop constraint [fk_Rating_RatedUserId]
go

alter table dbo.Rating
 drop column RatedUserId
go

--We don't generate PK any more because it's  extenssion table now
alter table dbo.Rating
 drop constraint [df_Rating_pk]
go

alter table dbo.Rating
  add SpaceOwnerReply nvarchar(300)
go 

/*
Author: Nikita Pochechuev

--Description:
This trigger computes avg Rate and count of rates for user-s
--This trig

--Test case 

begin tran

declare @idRating uniqueidentifier, @IdUser uniqueidentifier ;
select top 1 
       @idRating = r.Id, @IdUser = s.UserId
	   --r.*, s.UserId 
   from dbo.Rating r with(nolock)
  inner 
   join dbo.MessageOfferHistory moh with(nolock)
     on moh.Id = r.Id
  inner
   join dbo.MessageOffer mo with(nolock)
     on mo.Id = moh.MessageOfferId --we can re-write to direct link butindex for CurrentMessageOfferHistory must be created
  inner
   join dbo.Message m with(nolock)
     on mo.Id = m.Id
  inner
   join dbo.Chat c with(nolock)
     on c.Id = m.ChatId
  inner
   join dbo.Space s with(nolock)

     on s.Id = c.SpaceId 
  
  order by newid()

select [AvgRate], CountRating from [dbo].[User] t where t.Id = @IdUser;

update t set t.[Rank] = t.[Rank] + 1 from [dbo].[Rating] t where t.Id = @idRating

select [AvgRate], CountRating from [dbo].[User] t where t.Id = @IdUser;

rollback tran;
*/
ALTER trigger [dbo].[trRating_computeAvgRatingAndCountForUser]
    on [dbo].[Rating] after insert, update, delete
as
begin

 set nocount on;
 if not(update(Rank))
 begin
  print 'trRating_computeAvgRatingAndCountForUser exit without changes';
  return;
 end;

 print 'trRating_computeAvgRatingAndCountForUser was started';

 with cteUpdateRows as
 (
 select distinct
        --coalesce(i.[RatedUserId], d.[RatedUserId]) as [RatedUserId]
		i.Id
   from inserted i
   full
   join deleted d
     on d.Id = i.Id
  where i.Rank <> d.Rank
     or i.Id is null
	 or d.Id is null
 )
 
 --select * from cteUpdateRows
 ,

 cteUpdate as 
 (
 select distinct 
        s.UserId --we don't have direct link to user anymore
   from cteUpdateRows
  inner 
   join dbo.MessageOfferHistory moh with(nolock)
     on moh.Id = cteUpdateRows.Id
  inner
   join dbo.MessageOffer mo with(nolock)
     on mo.Id = moh.MessageOfferId --we can re-write to direct link butindex for CurrentMessageOfferHistory must be created
  inner
   join dbo.Message m with(nolock)
     on mo.Id = m.Id
  inner
   join dbo.Chat c with(nolock)
     on c.Id = m.ChatId
  inner
   join dbo.Space s with(nolock)
     on s.Id = c.SpaceId  
 )
 
 --select * from cteUpdate
 ,

 cteCompute as
 (
 select s.UserId as RatedUserId,
        avg(cast(r.Rank as numeric(4, 2))) as AvgRate,
		count(1) as CountRating
   from dbo.Rating r with(nolock)
  inner 
   join dbo.MessageOfferHistory moh with(nolock)
     on moh.Id = r.Id
  inner
   join dbo.MessageOffer mo with(nolock)
     on mo.Id = moh.MessageOfferId --we can re-write to direct link butindex for CurrentMessageOfferHistory must be created
  inner
   join dbo.Message m with(nolock)
     on mo.Id = m.Id
  inner
   join dbo.Chat c with(nolock)
     on c.Id = m.ChatId
  inner
   join dbo.Space s with(nolock)
  inner
   join cteUpdate u
     on u.UserId = s.UserId

     on s.Id = c.SpaceId  

  group
     by s.UserId

 )

 update u  
    set u.AvgRate = c.AvgRate,
	    u.CountRating = c.CountRating
 --output inserted.*, deleted.*
   from cteCompute c
  inner
   join dbo.[User] u
     on c.RatedUserId = u.Id;	 

 print 'trRating_computeAvgRatingAndCountForUser was finished';

end;
go

--Import Test data
with cte as
(
select ch.Id as ChatId,
       s.UserId as SpaceOwnerUser,       
	   ch.UserId as ArendatorUserId,
	   
	   --MessageOfferHistory that will be used
	   moh.Id OfferHistoryId
	    
  from dbo.Space s --space owner will be here
 inner
  join dbo.Chat ch
    on ch.SpaceId = s.Id
 inner
  join dbo.Message m
    on ch.Id = m.ChatId
 inner
  join dbo.MessageOffer mo
    on mo.Id = m.Id
 inner
  join dbo.MessageOfferHistory moh
    on moh.Id = mo.CurretMessageOfferHistory   
 
),

cteOfferHistoryForRating as
(
--remove duplicates 
select c.ChatId,
       c.SpaceOwnerUser,
       c.ArendatorUserId,
	   max(c.OfferHistoryId) as OfferHistoryId, --let it be the latest offer,
	   row_number() over (order by c.ChatId) rn
  from cte c
 group
    by c.ChatId,
	   c.SpaceOwnerUser,
       c.ArendatorUserId
)

insert
  into dbo.Rating
       (Id, Rank, Message, CreatedDate)
select 
       c.OfferHistoryId,
	   r.Rank, r.Message, r.CreatedDate
  from cteOfferHistoryForRating c
 inner 
  join (select r.*, ROW_NUMBER() over (order by r.Id) as rn from #Rating r) r 
    on r.rn = c.rn


--IT'S BAD that we don't have "case/event of rent" on DB level
--database my have redundand and incorrect data and two chats may have offeres in the same time
