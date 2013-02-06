using DirtyGirl.Models;
using System.Collections.Generic;

namespace DirtyGirl.Services.ServiceInterfaces
{
    public interface IEventLeadService
    {
        #region Event Lead

        IList<EventLead> GetAllGlobalEventLeads();

        IList<EventLeadType> GetAllEventLeadTypes();
            
        ServiceResult CreateEventLead(EventLead coupon);

        ServiceResult UpdateEventLead(EventLead coupon);

        ServiceResult RemoveEventLead(int eventLeadId);

        #endregion
    }
}
