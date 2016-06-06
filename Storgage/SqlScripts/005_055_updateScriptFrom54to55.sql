--begin tran

;
with cte as

(select
        cast(u.phone as nvarchar(1)) as firstSymbol,
		len(u.phone) phoneLen,
		substring(u.phone, 1, 2) firstTwoSymbols,
		 *
   from dbo.[User] u
)

--select 

update uu
   set uu.phone = 
       case
        
		when ( firstTwoSymbols <> '+1' and firstSymbol <> '1')
		then null
			    
		when u.phone like '_%[^0-9]%'  
		 or not(u.firstSymbol = '+' or u.firstSymbol like '[0-9]%')
		then null

		when u.phoneLen between 12 and 12 and u.firstSymbol = '+'
		  or u.phoneLen between 11 and 11 and u.firstSymbol <> '+'
		then 
		  case
		   when firstSymbol <> '+'
		   then '+'
		   else '' 
		  end    
	   end +  --it may be null that ereases phone
	   
	   u.Phone --as newPhone, --generate phone 
       --*
  from cte u
 inner
  join dbo.[User] uu
    on uu.Id = u.Id

where u.phone like '_%[^0-9]%'
   or u.firstSymbol <> '+'
   or u.phoneLen not between 11 and 12
   or ( firstTwoSymbols <> '+1' and firstSymbol <> '1')

--rollback tran; 