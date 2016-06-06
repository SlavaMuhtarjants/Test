update t
   set t.Synonym =
       case synonym
	    when 'WAS_READ' then 'WasRead'
		when 'SENT' then 'WasSent'
		when 'NOT_SENT' then 'WasNotSent'
	   end
  from [dbo].[MessageDeliveredStatus] t

go

alter table [dbo].[User]
  add CountRating int not null
  constraint df_User_CountRating
  default (0)
go

drop
 index idx_User_Email_incl
    on dbo.[User]
go


  drop
 index idx_User_Email_unique_incl
    on dbo.[User]

create 
 index idx_User_Email_unique_incl
    on dbo.[User] (Email)
       include (Id, Password, Phone/*, IsPhoneVerified*/, Lastname, Firstname, PaymentSystemSecret, AvgRate, PhoneVerificationStatusID, AvatarLink, CountRating)
go

create 
unique
 index idx_User_Email_incl
    on dbo.[User](email)
	   include (Id, Password, Phone/*, IsPhoneVerified*/, Lastname, Firstname, PaymentSystemSecret, AvgRate, PhoneVerificationStatusID, AvatarLink, CountRating)
go

with cte as
(
select r.RatedUserId,
       count(1) as [CountRating],
	   cast(avg(cast(r.Rank as numeric(4, 2))) as numeric(4, 2)) as [AvgRate]

  from [dbo].[Rating] r
 group
    by r.RatedUserId
)
/*select u.Email,
       u.AvgRate,
	   r.**/
update u
   set u.[AvgRate] = u.[AvgRate],
       u.[CountRating] = r.[CountRating]
  from dbo.[User] u
  --left
  join cte r
    on r.RatedUserId = u.Id
go

/*
Author: Nikita Pochechuev

--Description:
This trigger computes avg Rate and count of rates for user-s
--This trig

--Test case 

begin tran

declare @idRating uniqueidentifier, @IdUser uniqueidentifier ;
select top 1 @idRating = t.Id, @IdUser = t.RatedUserId from [dbo].[Rating] t

select [AvgRate], CountRating from [dbo].[User] where Id = @IdUser;

update t set t.[Rank] = t.[Rank] + 1 from [dbo].[Rating] t where t.Id = @idRating

select [AvgRate], CountRating from [dbo].[User] t where t.Id = @IdUser;

rollback tran;
*/
create trigger trRating_computeAvgRatingAndCountForUser
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

 with cteUpdate as
 (
 select distinct
        coalesce(i.[RatedUserId], d.[RatedUserId]) as [RatedUserId]
   from inserted i
   full
   join deleted d
     on d.Id = i.Id
  where i.Rank <> d.Rank
     or i.Id is null
	 or d.Id is null
 ),

 cteCompute as
 (
 select r.RatedUserId,
        avg(cast(r.Rank as numeric(4, 2))) as AvgRate,
		count(1) as CountRating
   from cteUpdate u
  inner
   join dbo.Rating r with(nolock)
     on u.RatedUserId = r.RatedUserId
  group
     by r.RatedUserId
 )

 update u  
    set u.AvgRate = c.AvgRate,
	    u.CountRating = c.CountRating
   from cteCompute c
  inner
   join dbo.[User] u
     on c.RatedUserId = u.Id;

 print 'trRating_computeAvgRatingAndCountForUser was finished';

end;
go

alter
function ns.fnMessageDeliveredStatus
(	
)
returns table
as
return
(
 select dbo.fnMessageDeliveredStatusGetId('WasRead') as WasRead,
        dbo.fnMessageDeliveredStatusGetId('WasSent') as WasSent,
		dbo.fnMessageDeliveredStatusGetId('WasNotSent') as WasNotSent
)
GO