using DirtyGirl.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class Coupon : DiscountItem 
    {
        protected DateTime _startDate;
        protected DateTime? _endDate;

        [Required(ErrorMessage="Please Select a Coupon Type")]
        public CouponType CouponType { get; set; }      

        public int? MaxRegistrantCount { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date Effective is required.")]
        public DateTime StartDateTime
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                _startDate = DateTime.SpecifyKind(_startDate, DateTimeKind.Utc);
            }
        }

        [DataType(DataType.Date)]
        public DateTime? EndDateTime
        {

            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
                if (_endDate.HasValue)
                {
                    _endDate = value.Value;
                    _endDate = DateTime.SpecifyKind(_endDate.Value, DateTimeKind.Utc);
                }
            }
        }

        public bool IsReusable { get; set; }

        public string Description { get; set; }        

        public int? EventId { get; set; }        

        #region Naviation Properties

        public virtual Event Event { get; set; }

        #endregion

    }
}
