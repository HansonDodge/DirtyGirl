using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using DirtyGirl.Models.Validation;
using DirtyGirl.Web.Helpers;
using DirtyGirl.Models.Enums;

namespace DirtyGirl.Web.Models
{
    public class vmRegistration_Details
    {
        public Guid ItemId { get; set; }

        public Registration RegistrationDetails { get; set; }
        public EventOverview EventOverview { get; set; }        
        public EventWave EventWave { get; set; }

        public IList<Region> RegionList { get; set; }
        public IList<SelectListItem> RegistrationTypeList { get; set; }
        public IList<SelectListItem> PacketDeliveryOptionList { get; set; }
        public IList<SelectListItem> TShirtSizeList { get; set; }
        public IList<EventLead> EventLeadList { get; set; }

        public vmRegistration_Details()
        {
            RegistrationDetails = new Registration();
        }

    }
}