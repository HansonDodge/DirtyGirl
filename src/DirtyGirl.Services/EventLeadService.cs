using System;
using System.Collections.Generic;
using System.Linq;
using DirtyGirl.Data.DataInterfaces.RepositoryGroups;
using DirtyGirl.Models;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Services.Utils;

namespace DirtyGirl.Services
{
    public class EventLeadService : ServiceBase, IEventLeadService
    {

        #region constructor

        public EventLeadService(IRepositoryGroup repository) : base(repository, false)
        {
        }

        public EventLeadService(IRepositoryGroup repository, bool isSharedRepository)
            : base(repository, isSharedRepository)
        {
        }

        #endregion

        #region validation

        protected bool ValidateEventLead(EventLead eventLeadToValidate, ServiceResult serviceResult)
        {
            //if (couponToValidate.EndDateTime < couponToValidate.StartDateTime)
            //{
            //    serviceResult.AddServiceError("EndDateTime",
            //                                  "The effective end date must be after the effective start date.");
            //}


            //foreach (Coupon c in existingCoupons)
            //{
            //    if (c.IsActive &&
            //        (
            //            (c.EndDateTime != null && c.EndDateTime > couponToValidate.StartDateTime &&
            //             c.StartDateTime < couponToValidate.EndDateTime) ||
            //            (c.EndDateTime == null && c.StartDateTime < couponToValidate.EndDateTime) ||
            //            (couponToValidate.EndDateTime != null && c.StartDateTime < couponToValidate.EndDateTime &&
            //             couponToValidate.StartDateTime < c.EndDateTime) ||
            //            (couponToValidate.EndDateTime == null && c.EndDateTime > couponToValidate.StartDateTime) ||
            //            (couponToValidate.EndDateTime == null && c.EndDateTime == null)
            //        )
            //        )
            //    {
            //        serviceResult.AddServiceError("Code",
            //                                      "There is already an active coupon with the same code in effect during the selected effective date range.");
            //    }
            //}


            return serviceResult.Success;
        }

        #endregion


        public IList<EventLead> GetAllGlobalEventLeads()
        {
            return _repository.EventLeads.Filter(x => x.EventId.HasValue == false).ToList();
        }

        public IList<EventLeadType> GetAllEventLeadTypes()
        {
            return _repository.EventLeadTypes.All().ToList();
        }

        public ServiceResult CreateEventLead(EventLead eventLead)
        {
            var result = new ServiceResult();
            try
            {
                if (ValidateEventLead(eventLead, result))
                {
                    _repository.EventLeads.Create(eventLead);

                    if (!_sharedRepository)
                        _repository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }
            return result;
        }

        public ServiceResult UpdateEventLead(EventLead eventLead)
        {
            var result = new ServiceResult();

            try
            {
                if (ValidateEventLead(eventLead, result))
                {
                    var updateEventLead = _repository.EventLeads.Find(x => x.EventLeadId == eventLead.EventLeadId);

                    updateEventLead.EventLeadTypeId = eventLead.EventLeadTypeId;
                    updateEventLead.DisplayText = eventLead.DisplayText;
                    updateEventLead.Title = eventLead.Title;

                    if (!_sharedRepository)
                        _repository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }

            return result;
        }

        public ServiceResult RemoveEventLead(int eventLeadId)
        {
            var result = new ServiceResult();

            try
            {
                var eventLeadToDelete = _repository.EventLeads.Find(x => x.EventLeadId == eventLeadId);
                _repository.EventLeads.Delete(eventLeadToDelete);
                if (!_sharedRepository)
                    _repository.SaveChanges();
            }

            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }
            return result;
        }
    }
}