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
        public string CardHolderFirstname { get; set; }

        [Required(ErrorMessage = "Card holder last name is required.")]
        public string CardHolderLastname { get; set; }

        [Required(ErrorMessage = "Card holder zip code is required.")]
        public string CardHolderZipCode { get; set; }

        [Required(ErrorMessage="card type is required")]
        public int CardType {get; set;}

        [DataType(DataType.CreditCard)]
        [Required(ErrorMessage="Credit card number is required")]
        [CreditCard(ErrorMessage="Please enter a valid card number")]
        public string CardNumber { get; set; }
        
        [Required(ErrorMessage = "Expiration month")]
        public int ExpirationMonth { get; set; }

        [Required(ErrorMessage = "Expiration year required")]
        public int ExpirationYear { get; set; }

        [Required(ErrorMessage = "CCV required")]
        [RegularExpression("^[0-9]{3,4}$", ErrorMessage = "Please enter a valid ccv")]
        public string CCVNumber { get; set; }        

    }
}
