/*
Author: Nikita Pochechuev

Description: Sets [AvgRate] value for all users according to data in dbo.Rating

Test case
 exec misc.spUserAvgRate;
*/

ALTER procedure [misc].[spUserAvgRate] 
as
begin

with cte as
(
select avg(cast(r.Rank as numeric(4,2))) as Rank,
       s.UserId
  from dbo.Rating r with(nolock)
 inner
  join dbo.MessageOfferHistory moh with(nolock)
    on moh.Id = r. Id
 inner
  join dbo.Message m with(nolock)
    on m.Id = moh.MessageOfferId
 inner
  join dbo.Chat ch with(nolock) 
    on ch.Id = m.ChatId
 inner
  join dbo.Space s with(nolock)
    on s.Id = ch.SpaceId
 group
    by s.UserId
)

update u 
   set u.[AvgRate] = coalesce(c.Rank, 0)
  from dbo.[User] u
 --inner
  left
  join cte c
    on c.UserId = u.Id
 where u.[AvgRate] <> coalesce(c.Rank, 0);  

end;
go

/*
Author: Nikita Pochechuev

Description
Procedure fixes values for [User].TotalUnreadMessages

exec misc.spUser_TotalUnreadMessages
*/

create procedure misc.spUser_TotalUnreadMessages
as
begin

with
cteTotalUnread as 
(
 select chM.UserId,
        sum(chM.AmountUnreadMessages) as TotalUnreadMessages
   from  dbo.ChatMember chM with(nolock)     
  group
     by chM.UserId
)

update u 
   set u.TotalUnreadMessages = tu.TotalUnreadMessages
  from cteTotalUnread tu
 inner
  join dbo.[User] u
    on u.Id = tu.UserId
   and tu.TotalUnreadMessages <> u.TotalUnreadMessages;
end;

go

/*
Author: Nikita Pochechuev

Description
Procedure fixes values for ChatMember.AmountUnreadMessages

exec misc.spChatMember_AmountUnreadMessages
*/
create procedure [misc].[spChatMember_AmountUnreadMessages]
as
begin

with cte as
(
select chM.Id,
	   count(m.ChatId) as AmountUnreadMessages
  from dbo.ChatMember chM
  left
  join dbo.Message m
 inner
  join ns.fnMessageDeliveredStatus() ms
    on m.DeliveredStatusId <> ms.WasRead
    on m.ChatId = chM.ChatId
   and m.UserId <> chM.UserId

-- where AmountUnreadMessages < 0

 group
    by chM.Id
 --order
 --   by AmountUnreadMessages desc
)

update chM
   set chM.AmountUnreadMessages = c.AmountUnreadMessages
  from dbo.ChatMember chM
 inner
  join cte c
    on c.Id = chM.Id
   and c.AmountUnreadMessages <> chM.AmountUnreadMessages;

end

go

/*
Author: Nikita Pochechuev

Description: Run all procedures that should be executed after import or for firx curent values

Test case
 exec misc.spRunAfterImport;
*/

ALTER procedure [misc].[spRunAfterImport]
as
begin
 exec misc.spUserAvgRate; 
 exec misc.spChatMember_AmountUnreadMessages;
 exec misc.spUser_TotalUnreadMessages;
 --the next procedures are possible
end;
go

exec [misc].[spRunAfterImport];
go