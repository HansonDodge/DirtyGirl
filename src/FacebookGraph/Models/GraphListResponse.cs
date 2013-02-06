using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FacebookOpenGraph.Models
{
    public class GraphListResponse<T>
    {
        public IEnumerable<T> data { get; set; }
        public Paging paging { get; set; }
    }
}
