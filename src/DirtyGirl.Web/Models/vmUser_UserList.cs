using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Models
{
    public class vmUser_UserList
    {
        public vmUser_UserList(){}
        public vmUser_UserList(IEnumerable<vmUser_Detail> users)
        {
            this.Users = users;
        }
        public IEnumerable<vmUser_Detail> Users { get; set; }
    }
}