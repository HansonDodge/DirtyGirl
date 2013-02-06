using DirtyGirl.Data.DataInterfaces.RepositoryGroups;
using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirtyGirl.Services
{
    public class ReportingService : ServiceBase, IReportingService
    {

        #region constructor

        public ReportingService(IRepositoryGroup repository) : base(repository, false) { }
        public ReportingService(IRepositoryGroup repository, bool isShared) : base(repository, isShared) { }

        #endregion

        #region Event Reporting

        public IList<Event> GetEventList()
        {
            return _repository.Events.All().OrderByDescending(x => x.EventDates.Min(y => y.DateOfEvent)).ToList();
        }

        public Event GetEventById(int eventId)
        {
            return _repository.Events.Find(x => x.EventId == eventId);
        }

        #endregion
        
        #region Registration Reporting

        public IList<Registration> GetRegistrationsByEventId(int eventId)
        {
            return _repository.Registrations.Filter(x => x.EventWave.EventDate.EventId == eventId && x.RegistrationStatus == RegistrationStatus.Active).ToList();
        }

        public int GetEventCount(int? eventId, DateTime startDate, DateTime endDate)
        {
            var eList = _repository.EventFees.Filter(x => x.EffectiveDate <= endDate && x.Event.EventDates.Max(y => y.DateOfEvent) >= startDate).GroupBy(x => x.Event).Select(x => x.Key);

            if (eventId.HasValue)
                eList = eList.Where(x => x.EventId == eventId.Value);

            return eList.Count();
        }

        public int GetSpotsAvailable(int? eventId, DateTime startDate, DateTime endDate)
        {
            var eList = _repository.EventFees.Filter(x => x.EffectiveDate <= endDate && x.Event.EventDates.Max(y => y.DateOfEvent) >= startDate).GroupBy(x => x.Event).Select(x => x.Key);

            if (eventId.HasValue)
                eList = eList.Where(x => x.EventId == eventId.Value);

            return eList.Sum(x => x.EventDates.Sum(y => y.EventWaves.Sum(z => z.MaxRegistrants)));                
        }

        public int GetSpotsTaken(int? eventId, DateTime startDate, DateTime endDate)
        {
            var rList = _repository.Registrations.Filter(x => x.RegistrationStatus == RegistrationStatus.Active && x.DateAdded >= startDate && x.DateAdded <= endDate);

            if (eventId.HasValue)
                rList = rList.Where(x => x.EventWave.EventDate.EventId == eventId.Value);

            return rList.Count();
        }

        public int GetRedemptionSpots(int? eventId, DateTime startDate, DateTime endDate)
        {
            var rList = _repository.Registrations.Filter(x => x.ParentRegistrationId.HasValue &&  x.RegistrationStatus == RegistrationStatus.Active && x.DateAdded >= startDate && x.DateAdded <= endDate);

            if (eventId.HasValue)
                rList = rList.Where(x => x.EventWave.EventDate.EventId == eventId.Value);

            return rList.Count();
        }

        public decimal GetRegistrationPathValue(int registrationId)
        {
            IRegistrationService service = new RegistrationService(this._repository, false);
            return service.GetRegistrationPathValue(registrationId);
        }

        #endregion

        #region Fee Reporting

        public List<FeeReport> GetFeeReport(int? eventId, DateTime startDate, DateTime endDate)
        {
            var fees = _repository.EventFees.All();

            if (eventId.HasValue)
                fees = fees.Where(x => x.EventId == eventId.Value);                   

            var report = from f in fees
                         join ci in _repository.CartItems.All() on f.PurchaseItemId equals ci.PurchaseItemId
                         where ci.DateAdded >= startDate && ci.DateAdded <= endDate
                         group ci by new { f.EventFeeType, f.Cost } into g
                         select new FeeReport
                         {
                             FeeType = g.Key.EventFeeType,
                             Cost = g.Key.Cost,
                             UseCount = g.Count(),
                             CostTotal = g.Sum(x => x.Cost),
                             LocalTaxTotal = g.Sum(x => (decimal?)x.LocalTaxValue.Value ?? 0),
                             StateTaxTotal = g.Sum(x => (decimal?)x.StateTaxValue.Value ?? 0),
                             DiscountTotal = g.Sum(x => (decimal?)x.DiscountValueTotal.Value ?? 0),
                             ActualTotal = g.Sum(x => x.Total)
                         };            
            
            return report.ToList();
        }
        
        #endregion

        #region Charge Reporting

        public List<ChargeReport> GetEventChargeReport(int? eventId, DateTime startDate, DateTime endDate)
        {
            var fees = _repository.EventFees.All().Where(x => x.EventFeeType == EventFeeType.ChangeEvent);

            if (eventId.HasValue)
                fees = fees.Where(x => x.EventId == eventId.Value);  

            var carts = from c in _repository.Carts.All()
                        join ci in _repository.CartItems.All() on c.CartId equals ci.CartId
                        join f in fees on ci.PurchaseItemId equals f.PurchaseItemId
                        where ci.DateAdded >= startDate && ci.DateAdded <= endDate
                        group c by new {c.CartId} into g
                        select g.Key;


            var report = from ci in _repository.CartItems.All()
                          join c in carts on ci.CartId equals c.CartId
                          join ch in _repository.Charges.All() on ci.PurchaseItemId equals ch.PurchaseItemId                        
                          group ci by new {ch.PurchaseItemId, ch.Name, ch.Description} into g
                          select new ChargeReport
                          {
                            ChargeId = g.Key.PurchaseItemId,
                            Name = g.Key.Name,
                            CostTotal = g.Sum(x => x.Cost),
                            LocalTaxTotal = g.Sum(x => (decimal?)x.LocalTaxValue.Value ?? 0),
                            StateTaxTotal = g.Sum(x => (decimal?)x.StateTaxValue.Value ?? 0),
                            DiscountTotal = g.Sum(x => (decimal?)x.DiscountValueTotal.Value ?? 0),
                            ActualTotal = g.Sum(x => x.Total)
                          };

            return report.ToList();

        }

        #endregion

    }
}
