declare @id int

insert into DiscountItem (code, discounttype, value, isactive, dateadded)
values ('–DGserves_2013–', 1, 10, 1, getdate())

set @id = @@identity;

insert into coupon (discountitemid, coupontype, maxregistrantcount, startdatetime, enddatetime, isreusable, eventid)
values (@id, 1, 10000, '1/1/2013', '12/31/2013', 1, null)
