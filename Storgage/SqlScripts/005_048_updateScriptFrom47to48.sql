/*
Author: Nikita Pochechuev
Description: This procedure was created FOR TESTs ONLY, not for actual API using

Test case

begin tran;

declare @xml xml = '
<x>
 <i id="82E38645-B9EB-4C72-A2ED-F59F4A1D9543"/>
 <i id="00A23CCD-0B26-464E-9381-63D137CAC808"/>
 <i email="fmarshall2b@cpanel.net"/>
 <i facebookid="1599004680424822"/>
</x>';

exec tst.spUserDel @userList = @xml;


rollback tran;

*/
ALTER procedure [tst].[spUserDel]
(
 @userList xml
) as

begin

declare @users table(UserId uniqueidentifier);

insert
  into @users
select u.Id
  from @userList.nodes('/x/i') t(c)
 inner
  join dbo.[User] u
    on u.Id = t.c.value('@id', 'uniqueidentifier')
	or u.Email = t.c.value('@email', 'nvarchar(256)')
	or u.FacebookID = t.c.value('@facebookid', 'nvarchar(64)');

--Test select
--select * 
--  from @users

/*
 Testers (Anna,  "у нас атотесты будут генерить в деть по тыщи пользователей к примеру")
 said that we can deleted big amount of users and perfromance is important
 this is why I will NOT to insert data to temporary tables or table variables and 
 I WILL copy-past part of SQL Logic.
*/

--Chats for Delete example, but we can't delete them while there are child records
/*
select *
  from dbo.[User] u --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Chat ch 
 inner
  join dbo.Space s
    on s.Id = ch.SpaceId
    on ch.UserId = u.Id
	or s.UserId = u.Id
*/

--select *
update mo
   set mo.CurretMessageOfferHistory = null -- SET null because these [dbo].[MessageOfferHistory] will be deleted
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Chat ch with(nolock)
 inner
  join dbo.Space s with(nolock)
    on s.Id = ch.SpaceId
    on ch.UserId = u.Id
	or s.UserId = u.Id
 inner
  join dbo.Message m with(nolock) 
    on m.ChatId = ch.Id
 inner
  join dbo.MessageOffer mo
    on mo.Id = m.Id;


--select *
update ch
   set ch.ApprovedMessageOfferHistoryId = null,
       ch.LastMessageOfferId = null
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Chat ch with(nolock)
 inner
  join dbo.Space s with(nolock)
    on s.Id = ch.SpaceId
    on ch.UserId = u.Id
	or s.UserId = u.Id;


/*We can try to optimize to disable/enable trigger because user will be deleted in ny case*/
delete 
  from r
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Chat ch 
 inner
  join dbo.Space s with(nolock)
    on s.Id = ch.SpaceId
    on ch.UserId = u.Id
	or s.UserId = u.Id
 inner
  join dbo.Message m with(nolock) 
    on m.ChatId = ch.Id
 inner
  join dbo.MessageOffer mo with(nolock)
    on mo.Id = m.Id
 inner
  join dbo.MessageOfferHistory moh with(nolock)
    on moh.MessageOfferId = mo.Id
 inner
  join dbo.Rating r
    on r.Id = moh.Id

delete 
  from moh
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Chat ch with(nolock) 
 inner
  join dbo.Space s with(nolock)
    on s.Id = ch.SpaceId
    on ch.UserId = u.Id
	or s.UserId = u.Id
 inner
  join dbo.Message m with(nolock) 
    on m.ChatId = ch.Id
 inner
  join dbo.MessageOffer mo with(nolock)
    on mo.Id = m.Id
 inner
  join dbo.MessageOfferHistory moh
    on moh.MessageOfferId = mo.Id;

delete 
  from mo
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Chat ch with(nolock) 
 inner
  join dbo.Space s with(nolock)
    on s.Id = ch.SpaceId
    on ch.UserId = u.Id
	or s.UserId = u.Id
 inner
  join dbo.Message m with(nolock) 
    on m.ChatId = ch.Id
 inner
  join dbo.MessageOffer mo
    on mo.Id = m.Id;

delete 
  from m
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Chat ch with(nolock) 
 inner
  join dbo.Space s with(nolock)
    on s.Id = ch.SpaceId
    on ch.UserId = u.Id
	or s.UserId = u.Id
 inner
  join dbo.Message m 
    on m.ChatId = ch.Id;

delete 
  from chm
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Chat ch with(nolock) 
 inner
  join dbo.Space s with(nolock)
    on s.Id = ch.SpaceId
    on ch.UserId = u.Id
	or s.UserId = u.Id
 inner
  join dbo.ChatMember chm
    on chm.ChatId = ch.Id;


--For double sure from chat
delete 
  from chm
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.ChatMember chm
    on chm.UserId = u.Id;


delete 
  from ch
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Chat ch 
 inner
  join dbo.Space s with(nolock)
    on s.Id = ch.SpaceId
    on ch.UserId = u.Id
	or s.UserId = u.Id;

delete 
  from ch
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Chat ch 
 inner
  join dbo.Space s with(nolock)
    on s.Id = ch.SpaceId
    on ch.UserId = u.Id
	or s.UserId = u.Id;


update s 
   set s.DefaultPhotoID = null
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Space s with(nolock)
    on s.UserId = u.Id;

delete 
  from phl
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Space s with(nolock)
    on s.UserId = u.Id
 inner
  join dbo.PhotoLibrary phl with(nolock)
    on phl.SpaceId = s.Id;

delete 
  from r
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Invoice i with(nolock)
 inner
  join dbo.Space s with(nolock)
    on s.UserId = i.SpaceId
    on s.UserId = u.Id
    or i.SenderId = u.Id
 inner
  join dbo.Receipt r
    on r.InvoiceId = i.Id;

delete 
  from i
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Invoice i
 inner
  join dbo.Space s with(nolock)
    on s.UserId = i.SpaceId
    on s.UserId = u.Id
    or i.SenderId = u.Id;

update sb
   set sb.NextSpaceBusyID = null
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Space s with(nolock)
    on s.UserId = u.Id
 inner
  join dbo.SpaceBusy sb
    on sb.SpaceId = s.Id;

delete
  from sb
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Space s with(nolock)
    on s.UserId = u.Id
 inner
  join dbo.SpaceBusy sb
    on sb.SpaceId = s.Id;

delete 
  from s
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Space s
    on s.UserId = u.Id;

delete 
  from f
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.Filter f
    on f.UserId = u.Id;

delete 
  from ud
  from dbo.[User] u with(nolock) --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id
 inner
  join dbo.UserDevice ud
    on ud.UserId = u.Id;

delete 
  from rt
  from [dbo].[RefreshToken] rt
 inner
  join @users u_d
    on u_d.UserId = rt.UserId;

delete 
  from u
  from dbo.[User] u --we can don't join this table, it's redundand 
 inner
  join @users u_d
    on u_d.UserId = u.Id;

end
