using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FacebookOpenGraph.Models
{
    public class Album
    {
        public string id { get; set; }
        public string name { get; set; }
        public string cover_photo { get; set; }
        public string created_time { get; set; }
        public string updated_time { get; set; }
        public string description { get; set; }
        public string location { get; set; }
        public string count { get; set; }
        public string link { get; set; }
    }
}
