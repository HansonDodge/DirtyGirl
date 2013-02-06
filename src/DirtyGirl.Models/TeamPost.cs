using System.Collections.Generic;

namespace DirtyGirl.Models
{
    public class TeamPost : ModelBase
    {
        public int TeamPostId { get; set; }

        public string Post { get; set; }

        public int TeamId { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}