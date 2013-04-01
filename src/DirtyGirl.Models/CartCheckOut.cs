using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace DirtyGirl.Models
{
    public class CartCheckOut
    {
        public IList<SelectListItem> ExpirationMonthList { get; set; }

        public IList<SelectListItem> ExpirationYearList { get; set; }

        public CartSummary CartSummary { get; set; }

        public bool DiscountApplied { get; set; }

        [Required(ErrorMessage = "Card holder first name is required.")]
        [RegularExpression(@"^(?!\s+$)[a-zA-Z][a-zA-Z- ]+$", ErrorMessage = "Please enter a valid cardholder first name")]
        public string CardHolderFirstname { get; set; }

        [Required(ErrorMessage = "Card holder last name is required.")]
        [RegularExpression(@"^(?!\s+$)[a-zA-Z][a-zA-Z- ]+$", ErrorMessage = "Please enter a valid card holder last name")]
        public string CardHolderLastname { get; set; }

        [Required(ErrorMessage = "Card holder zip code is required.")]
        [RegularExpression(@"^[0-9]{5}(-[0-9]{4})?$", ErrorMessage = "Zip Code is not valid.")]      
        public string CardHolderZipCode { get; set; }

        [Required(ErrorMessage="card type is required")]
        public int CardType {get; set;}

        [DataType(DataType.CreditCard)]
        [Required(ErrorMessage="Credit card number is required")]
        [CreditCard(ErrorMessage="Please enter a valid card number")] 
        public string CardNumber { get; set; }
        
        [Required(ErrorMessage = "Expiration month is required")]
        public int ExpirationMonth { get; set; }

        [Required(ErrorMessage = "Expiration year is required")]
        public int ExpirationYear { get; set; }

        [Required(ErrorMessage = "CCV required")]
        [RegularExpression("^[0-9]{3,4}$", ErrorMessage = "Please enter a valid ccv")]
        public string CCVNumber { get; set; }        

    }
}
