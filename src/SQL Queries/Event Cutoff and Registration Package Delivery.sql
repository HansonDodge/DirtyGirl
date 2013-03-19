alter table event add RegistrationCutoff datetime
alter table event add EmailCutoff datetime

update event
set registrationcutoff = dateadd(hh, -10, d.mindate),
    emailcutoff = dateadd(hh, -36, d.mindate)
from event
inner join 
	(select ev.EventID, min(ed.DateOfEvent) as minDate
	from event ev
	inner join EventDate ed on ed.eventID = ev.EventID
	group by ev.EventID) d on d.eventID = event.eventid
	
select

