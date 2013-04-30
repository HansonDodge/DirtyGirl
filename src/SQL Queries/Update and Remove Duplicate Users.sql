DECLARE @name VARCHAR(50) -- database name  
declare @mainUser integer

DECLARE db_cursor CURSOR FOR  
select [UserName] FRom (SELECT [UserName], count(*) as uCount  
FROM [User]
group by UserName) cnt where cnt.uCount > 1

OPEN db_cursor   
FETCH NEXT FROM db_cursor INTO @name   

WHILE @@FETCH_STATUS = 0   
BEGIN   
	  drop table #AllUsers
	  drop table #OtherUsers

      select userid into #AllUsers from [user] where UserName = @name
	  select @mainUser = (select top 1 userid from #AllUsers)
	  select userid into #OtherUsers from #AllUsers where UserId <> @mainUser

	  --select @mainUser, userid from #OtherUsers

	  update Registration set userid = @mainUser where userid in (select userid from #OtherUsers)
	  update Cart set userid = @mainUser where userid in (select userid from #OtherUsers)
	  update Team set creatorid = @mainUser where creatorid in (select userid from #OtherUsers)
	  delete from user_role where userid in (select userid from #OtherUsers)
	  delete from [user] where userid in (select userid from #OtherUsers)
	  
	  drop table #AllUsers
	  drop table #OtherUsers

      FETCH NEXT FROM db_cursor INTO @name   
END   

CLOSE db_cursor   
DEALLOCATE db_cursor