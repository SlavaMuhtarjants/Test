with cte as
(
select u.email, 
       count(1) as cnt,
	   min(u.Id) minUserId
  from dbo.[User] u
where u.email is not null
 group
    by u.email
having count(1) > 1
)
,
cteUpdate as
(
select row_number() over (partition by c.email order by  u.Id) as rowNum,
       u.Id
  from dbo.[User] u
 inner
  join cte c
    on c.email = u.email
   and c.minUserId <> u.Id
 )

--select * 
update u
   set u.email = u.email + '_' + cast(c.rowNum as nvarchar(10))
  from cteUpdate c
 inner
  join dbo.[User] u
    on c.Id = u.Id
go

drop INDEX [idx_User_Email_unique_incl] 
   ON [dbo].[User]
go

/****** Object:  Index [idx_User_Email_unique_incl]    Script Date: 18.04.2016 11:27:44 ******/
CREATE unique INDEX [idx_User_Email_unique_incl] ON [dbo].[User]
(
	[Email] ASC
)
INCLUDE ( 	[Id],
	[Password],
	[Phone],
	[Lastname],
	[Firstname],
	[PaymentSystemSecret],
	[AvgRate],
	[PhoneVerificationStatusID],
	[AvatarLink],
	[CountRating],
	[FacebookID],
	[EmailVerificationStatusID],
	[IsExternal]) 
where [Email] is not null
go
