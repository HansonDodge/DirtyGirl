using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{
    public class Team:ModelBase
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
        public int EventId { get; set; }

        public string Code { get; set; }


        public int CreatorID { get; set; }

        public virtual IList<Registration> Registrations { get; set; }
        public virtual IList<TeamPost> TeamPosts { get; set; }
        public virtual Event Event { get; set; }

    }
}
