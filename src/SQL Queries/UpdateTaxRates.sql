

update event set statetax = 0, localtax = 0 

update event set statetax = 5, localtax = 0.1 where regionid = 51

update event set statetax = 7.3, localtax = 0 where regionid = 5

update event set statetax = 7, localtax = 0 where regionid = 12



select * from event order by statetax desc
