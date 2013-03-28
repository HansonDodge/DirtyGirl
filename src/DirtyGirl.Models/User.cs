using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DirtyGirl.Models
{       
    public class User: ModelBase
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is Required")]
        [RegularExpression(@".*[^ ].*", ErrorMessage = "Please enter a valid user name")]        
        public string UserName { get; set; }

        public string Password { get; set; }
        
        public string Salt { get; set; }

        [Required(ErrorMessage = "First Name is Required")]
        [RegularExpression(@"^(?!\s+$)[a-zA-Z][a-zA-Z- ]+$", ErrorMessage = "Please enter a valid first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is Required")]
        [RegularExpression(@"^(?!\s+$)[a-zA-Z][a-zA-Z- ]+$", ErrorMessage = "Please enter a valid last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Address is Required")]
        [RegularExpression(@"^(?!\s+$)[a-zA-Z0-9][a-zA-Z0-9# -.]+$", ErrorMessage = "Please enter a valid address")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        [Required(ErrorMessage = "City is Required")]
        [RegularExpression(@"^(?!\s+$)[a-zA-Z][a-zA-Z0-9- ]+$", ErrorMessage = "Please enter a valid city")]
        public string Locality { get; set; }

        [Required(ErrorMessage = "State is Required")]
        public int RegionId { get; set; }

        [Required(ErrorMessage = "Zip Code is Required")]
        [RegularExpression(@"^[0-9]{5}(-[0-9]{4})?$", ErrorMessage = "Zip Code is not valid.")]       
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "Email Address is Required")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$", ErrorMessage = "Email address format is invalid")]
        public string EmailAddress { get; set; }

        public Int64? FacebookId { get; set; }

        public byte[] Image { get; set; }

        public bool UseFacebookImage { get; set; }

        //This is nullable so that it doesn't have to pass through the user edit view except for admins
        public bool IsActive { get; set; }

        public string EmailVerificationCode { get; set; }

        public string PasswordResetToken { get; set; }

        public DateTime? PasswordResetRequested { get; set; }


        #region Naviation Properties
        public virtual IList<Role> Roles { get; set; }
        public virtual Region Region { get; set; }
        public virtual IList<Registration> Registrations { get; set; }
        public virtual IList<TeamPost> TeamPosts { get; set; }
        public virtual IList<Cart> Carts { get; set; }
        #endregion
    }
}
