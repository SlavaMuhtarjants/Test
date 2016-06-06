--select *
update u
   set u.Phone = '+' + u.Phone
  from dbo.[User] u
where u.Phone not like '+%'