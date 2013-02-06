using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FacebookOpenGraph.Models
{
    public class Post : GraphObject
    {
        public object from { get; set; }
        public object to { get; set; }
        public object message { get; set; }
        public object message_tags { get; set; }
        public object picture { get; set; }
        public object link { get; set; }
        public object name { get; set; }
        public object caption { get; set; }
        public object description { get; set; }
        public object source { get; set; }
        public object properties { get; set; }
        public object icon { get; set; }
        public object actions { get; set; }
        public object privacy { get; set; }
        public object type { get; set; }
        public object likes { get; set; }
        public object place { get; set; }
        public object story { get; set; }
        public object story_tags { get; set; }
        public object comments { get; set; }
        public object object_id { get; set; }
        public object application { get; set; }
        public object created_time { get; set; }
        public object updated_time { get; set; }

    }
}
