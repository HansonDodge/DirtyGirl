using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;

namespace DirtyGirl.Web.Areas.Admin.Models
{
    public class vmAdmin_RegistrantListItem
    {
        public int RegistrantId { get; set; }
        public DateTime WaveDate { get; set; }
        public DateTime WaveTime { get; set; }
        public string RegistrationType { get; set; }
        public string ThirdParty { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
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
        public string RegistrationValue { get; set; }
        public bool AgreeToTerms { get; set; }
        public DateTime DateAdded { get; set; }
    }
}