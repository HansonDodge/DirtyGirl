
declare @UserID int
declare @RoleID int

insert into Role([Name],[Description]) values ('EventManager','Event Manager')
 
set @RoleID = @@IDENTITY

insert into [User] (UserName,password,salt,firstname,lastname, address1, locality, regionid, postalcode, dateadded,emailaddress, dob)
values ('EvtManager','9K14jbzqUXy3Kx1xOJ/uFsAQLs0/pPUX','P+LEu3koVw7/D8DVJsxkdRLwUqZ5wFkm','Event', 'Manager','Somewhere','Milwaukee',51,'55555',getdate(),'evtManager@godirtygirl.com','1/1/1950')

set @UserID = @@IDENTITY

insert into User_Role(RoleId,UserId) values (@roleid,@UserID)
go

