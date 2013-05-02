using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DirtyGirl.Models.Enums;

namespace DirtyGirl.Web.Areas.Admin.Models
{
    public class vmAdmin_RegistrationListItem
    {
        //Event Information
        public DateTime WaveDateTime { get; set; }
        public DateTime Eventdate { get; set; }
        public string EventLocation { get; set; }
        public string EventPlace { get; set; }
        public int EventId { get; set; }
        public int EventWaveId { get; set; }
        public int RegistrationId { get; set; }
        public string CreatedByUsername { get; set; }

        //Registrant Details
        public int UserId { get; set; }
        public bool? IsThirdPartyRegistration { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string EmergencyContact { get; set; }
        public string EmergencyPhone { get; set; }
        public string MedicalInformation { get; set; }
        public string SpecialNeeds { get; set; }
        public DateTime DateAdded { get; set; }
        public string RegistrationType { get; set; }
        public string TShirtSize { get; set; }
        public string RegistrationStatus { get; set; }
        public bool IsOfAge { get; set; }
        public bool IsFemale { get; set; }
        public bool AgreeToTerms { get; set; }
        public bool AgreeToTrademark { get; set; }
        public string TotalCost { get; set; }
        public string Discount { get; set; }
        public string Cost { get; set; }
        public string StateTax { get; set; }
        public string LocalTax { get; set; }
        public string ConfirmationCode { get; set; }
    }
}