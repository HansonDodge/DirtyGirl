using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class EventTemplate
    {
        public int EventTemplateId { get; set; }
        public string Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int WaveDuration { get; set; }
        public int MaxRegistrantsPerWave { get; set; }
        public decimal DefaultRegistrationCost { get; set; }
        public decimal DefaultChangeFeeCost { get; set; }
        public decimal DefaultTransferFeeCost { get; set; }
        public decimal DefaultCancellationFeeCost { get; set; }
        public decimal DefaultShippingFeeCost { get; set; }
        public string DefaultPlaceName { get; set; }      

        private DateTime dateAdded = default(DateTime);
        public DateTime DateAdded
        {
            get { return (this.dateAdded == default(DateTime)) ? DateTime.Now : this.dateAdded; }
            set { dateAdded = value; }
        }

        #region navigation properties

        public virtual IList<EventTemplate_PayScale> PayScales { get; set; }

        #endregion
    }
}
