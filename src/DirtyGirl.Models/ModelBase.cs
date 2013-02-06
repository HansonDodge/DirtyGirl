using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public abstract class ModelBase
    {
        private DateTime dateAdded = default(DateTime);
        public DateTime DateAdded
        {
            get { return (this.dateAdded == default(DateTime)) ? DateTime.Now : this.dateAdded; }
            set { dateAdded = value; }
        }
    }
}
