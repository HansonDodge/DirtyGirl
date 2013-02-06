using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirtyGirl.Services.ServiceInterfaces
{
    public interface IReportingService
    {

        #region Event Reporting

        IList<Event> GetEventList();

        Event GetEventById(int eventId);

        #endregion

        #region Registration Report

        IList<Registration> GetRegistrationsByEventId(int eventId);

        int GetEventCount(int? eventId, DateTime startDate, DateTime endDate);

        int GetSpotsAvailable(int? eventId, DateTime startDate, DateTime endDate);

        int GetSpotsTaken(int? eventId, DateTime startDate, DateTime endDate);

        int GetRedemptionSpots(int? eventId, DateTime startDate, DateTime endDate);

        decimal GetRegistrationPathValue(int registrationId);        

        #endregion

        #region Fee Reporting

        List<FeeReport> GetFeeReport(int? eventId, DateTime startDate, DateTime endDate);        

        #endregion

        #region Charge Reporting

        List<ChargeReport> GetEventChargeReport(int? eventId, DateTime startDate, DateTime endDate);

        #endregion

    }
}
