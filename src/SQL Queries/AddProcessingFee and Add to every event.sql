declare @eventID int
declare @discountitemid int
DECLARE db_cursor CURSOR FOR  
SELECT EventID 
FROM Event

OPEN db_cursor   
FETCH NEXT FROM db_cursor INTO @eventID

WHILE @@FETCH_STATUS = 0   
BEGIN   
    insert into PurchaseItem(cost, discountable, taxable, dateadded) 
    values (5.69, 0 , 0, getdate()) 

	set @discountitemid = @@identity;
		
	insert into eventfee (purchaseitemid, eventid, eventfeetype, effectivedate)
	values (@discountitemid, @eventID, 6, getdate())
	
    FETCH NEXT FROM db_cursor INTO @eventID   
END   

CLOSE db_cursor   
DEALLOCATE db_cursor