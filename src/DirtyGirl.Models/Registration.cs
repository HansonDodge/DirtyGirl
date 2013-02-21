using DirtyGirl.Models.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DirtyGirl.Models.Validation;
using System;

namespace DirtyGirl.Models
{
    public class Registration : ModelBase
    {
        public int RegistrationId {get; set;}

        [Required(ErrorMessage="Registration Type is required.")]       
        public RegistrationType RegistrationType { get; set; }

        public RegistrationStatus RegistrationStatus { get; set; }

        public int UserId { get; set; }

        public int EventWaveId { get; set; }

        public int? TeamId { get; set; }

        [Required(ErrorMessage = "How did you hear about us is required")]
        [Range(0, 1000, ErrorMessage = "Please let us know how you heard about this")]
        public int? EventLeadId { get; set; }

        [Required(ErrorMessage = "Are you registering for someone else?")]
        public bool? IsThirdPartyRegistration { get; set; }

        [Required(ErrorMessage="First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage="Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email Address is Required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Email address format is invalid")]
        public string Email { get; set; }

        [Required(ErrorMessage="Phone is Required")]        
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Phone number is not valid.")]
        public string Phone { get; set; }

        [Required(ErrorMessage="Address is required")]
        public string Address1 { get; set; }

        public string Address2 { get; set; }

        [Required(ErrorMessage="City is Required")]
        public string Locality { get; set; }

        [Required(ErrorMessage = "State is required")]
        public int RegionId { get; set; }

        [Required(ErrorMessage="Zip code is required")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Emergency contact is required")]
        public string EmergencyContact { get; set; }

        [Required(ErrorMessage="Emergency contact phone number is required.")]        
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Emergency phone format is not valid.")]
        public string EmergencyPhone { get; set; }

        public string MedicalInformation { get; set; }

        public string SpecialNeeds { get; set; }

        [Required(ErrorMessage = "T-shirt size is required")]
        public TShirtSize? TShirtSize { get; set; }

        public string Gender { get; set; }

        [MustBeTrue(ErrorMessage = "Only women may register for this event.")]
        [DisplayName("By checking this box I am verifying that I, or the person I am registering for, is female.")]
        public bool IsFemale { get; set; }

        [MustBeTrue(ErrorMessage = "Must agree to liability waiver to register for this event.")]
        [DisplayName("I acknowledge I have carefully read, accept and agree to the terms on this Release and Waiver, and know and understand their contents and I sign the same on my own free act and deed.")]
        public bool AgreeToTerms { get; set; }

        [MustBeTrue(ErrorMessage = "Registrant must be 15 years old the day of the event.")]
        [DisplayName("By checking this box I am verifying that I, or the person I’m registering for, is 15 years of age of older.")]
        public bool IsOfAge { get; set; }

        public RegistrationMaterialsDeliveryOption? PacketDeliveryOption { get; set; }

        public string ReferenceAnswer { get; set; }

        public int? CartItemId { get; set; }

        public int? ParentRegistrationId { get; set; }

        public DateTime? DateUpdated { get; set; }

        #region Navigation Properties
       
        public virtual EventWave EventWave { get; set; }
        public virtual Team Team { get; set; }
        public virtual User User { get; set; }        
        public virtual Region Region { get; set; }
        public virtual CartItem CartItem { get; set; }       

        #endregion

    }
}
