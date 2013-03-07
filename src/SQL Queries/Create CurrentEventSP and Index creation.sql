create index IX_EventID on EventFee(EventID) 

create index IX_EventID on EventFee(EventID) 

create index IX_EventFeeEventDate on EventFee(EffectiveDate) 

create index IX_EventDateEventID on EventDate(EventID) 

create index IX_EventWaveEventDate on EventWave(EventDateID) 


Create PROCEDURE [dbo].[GetCurrentEventCounts]
	@EventId INT = NULL
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	       Select evts.EventId
, evts.EventDateId
, evts.DateOfEvent
, evts.IsActive
, evts.GeneralLocality
, evts.place
, evts.address1
, evts.locality
, evts.Name
, evts.code
, evts.postalcode
, evts.RegionID
, evts.maxregistrants	
, evts.RegistrationCount
, evts.PurchaseItemID
, evts.PinXCoordinate
, evts.PinYCoordinate    
, pi.cost
, ef.feeIconID
, fi.ImagePath
from (
Select EventDate.EventId, eventDate.EventDateId, ev.IsActive
,ev.GeneralLocality, ev.place, ev.address1, ev.locality, ev.RegionID
, rg.Name
, rg.code
, ev.postalcode
, eventDate.DateOfEvent
,(select sum(maxregistrants) from EventWave where EventWave.EventDateID = EventDate.EventDateID) as maxregistrants	
,(Select Count(reg2.RegistrationId) As RegistrationCount
  From EventWave ev2 inner join Registration reg2 ON ev2.EventWaveId = reg2.EventWaveId
  Where ev2.IsActive = 1 AND reg2.RegistrationId IS NOT NULL AND reg2.RegistrationStatus = 1)  as RegistrationCount
,(select top (1) ef.PurchaseItemId from EventFee ef where ef.EventId = ev.EventID and ef.EventFeeType = 1 and ef.EffectiveDate < GetDate() order by EffectiveDate desc ) as PurchaseItemID
,ev.PinXCoordinate
,ev.PinYCoordinate                          
From EventDate 
inner join Event ev on EventDate.EventId = ev.EventID
inner join Region rg on rg.regionid = ev.regionID 
where EventDate.DateOfEvent > getdate()and ev.EventID = IsNull(@EventID,ev.EventID)
) evts
inner join EventFee ef on ef.PurchaseItemId = evts.PurchaseItemID
inner join PurchaseItem pi on pi.PurchaseItemId = evts.PurchaseItemID
left outer join FeeIcon fi on fi.feeiconid = ef.feeIconID
               
END

