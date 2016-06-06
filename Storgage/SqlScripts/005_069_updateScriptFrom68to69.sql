alter table [dbo].[User] 
add TotalUnreadMessages int not null
constraint df_User_TotalUnreadMessages default (0)
GO

drop INDEX [idx_User_Email_unique_incl] ON [dbo].[User]
go

CREATE UNIQUE NONCLUSTERED INDEX [idx_User_Email_unique_incl] ON [dbo].[User]
(
	[Email] ASC
)
INCLUDE (Id, Password, Phone, Lastname, Firstname, PaymentSystemSecret, AvgRate, PhoneVerificationStatusID, AvatarLink, CountRating, FacebookID, EmailVerificationStatusID, IsExternal, OutPaymentPoint, IncPaymentPoint, AccessFailedCount, LockoutEndDate, StripeCustomerId, TotalUnreadMessages) 
WHERE ([Email] IS NOT NULL)
go

drop INDEX [idx_User_FacebookID_incl] ON [dbo].[User]
go

CREATE UNIQUE NONCLUSTERED INDEX [idx_User_FacebookID_incl] ON [dbo].[User]
(
	[FacebookID] ASC
)
INCLUDE (Id, Email, Password, Phone, Lastname, Firstname, PaymentSystemSecret, AvgRate, PhoneVerificationStatusID, AvatarLink, CountRating, EmailVerificationStatusID, IsExternal, OutPaymentPoint, IncPaymentPoint, AccessFailedCount, LockoutEndDate, StripeCustomerId, TotalUnreadMessages ) 
WHERE ([FacebookID] IS NOT NULL)
go

--Set initial correct values
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
go



/*
begin tran
 
 declare @UserId uniqueidentifier, @ChatMemmberId uniqueidentifier;

 select top 1
        @UserId = chM.UserId,
		@ChatMemmberId = chM.Id
   from dbo.ChatMember chM with(nolock);
 
 select TotalUnreadMessages, * from dbo.[User] u where u.Id = @UserId;

 update chM
    set chM.AmountUnreadMessages = chM.AmountUnreadMessages + 3
   from dbo.ChatMember chM with(nolock)
  where chM.Id = @ChatMemmberId;
 
 select TotalUnreadMessages, * from dbo.[User] u where u.Id = @UserId;
  

rollback tran;
*/

CREATE TRIGGER [dbo].[trChatMember_TotalUnreadMessages]
    ON [dbo].[ChatMember]
after INSERT, UPDATE, delete
AS
BEGIN 
 set nocount on;    
 print 'Start trigger trChatMember_TotalUnreadMessages';
 
 if not(update(AmountUnreadMessages))
 begin  
  print 'Exit from trChatMember_TotalUnreadMessages without changes';
  return;
 end;

 with cteUser as
 (
 select distinct
        coalesce(i.UserId, d.UserId) as UserId
        --coalesce(i.Id, d.Id)
   from inserted i
   full
   join deleted d
     on i.Id = d.Id
  where i.AmountUnreadMessages <> d.AmountUnreadMessages
     or i.AmountUnreadMessages is null and d.AmountUnreadMessages > 0
	 or d.AmountUnreadMessages is null and i.AmountUnreadMessages > 0
 ),

cteTotalUnread as 
(
 select chM.UserId,
        sum(chM.AmountUnreadMessages) as TotalUnreadMessages
   from cteUser u
  inner
   join dbo.ChatMember chM with(nolock)
     on chM.UserId = u.UserId
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
   
 print 'Finish trigger trChatMember_TotalUnreadMessages';

END 
GO



