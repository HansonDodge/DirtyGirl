using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class Region
    {
        public int RegionId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CountryId { get; set; }
        public virtual Country Country { get; set; }        
    }
}
