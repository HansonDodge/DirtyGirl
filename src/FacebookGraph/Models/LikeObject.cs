using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FacebookOpenGraph.Models
{
    public class LikeObject : GraphObject
    {
        public string name { get; set; }
        public string category { get; set; }
        public string created_time { get; set; }
    }
}
