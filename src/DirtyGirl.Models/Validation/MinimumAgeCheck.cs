using System;
using System.ComponentModel.DataAnnotations;

namespace DirtyGirl.Models.Validation
{

    public class MinimumAgeCheck : ValidationAttribute
    {
        private readonly int _min;
        private readonly string _defaultErrorMessage = "";
        public MinimumAgeCheck(int min, string defaultErrorMessage)
            : base(defaultErrorMessage)
        {
            _min = min;
            _defaultErrorMessage = defaultErrorMessage.Replace("{0}", _min.ToString());
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime bday = (DateTime)value;
            
            DateTime today = DateTime.Today;
            int age = today.Year - bday.Year;
            if (bday > today.AddYears(-age)) age--;

            if (age < _min)
            {
                return new ValidationResult(_defaultErrorMessage);
            }
            
            return ValidationResult.Success;

        }

    }
 
}


