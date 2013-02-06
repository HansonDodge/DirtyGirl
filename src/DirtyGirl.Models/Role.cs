using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class Role
    {
        public int RoleID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual IList<User> Users { get; set; }
    }
}
