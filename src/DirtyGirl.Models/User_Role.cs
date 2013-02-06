using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class User_Role
    {
        public int User_RoleId { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
    }
}
