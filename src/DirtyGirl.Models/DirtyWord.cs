using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirtyGirl.Models
{
    public class DirtyWord
    {
        [Key]
        public int WordID { get; set; }
        public string Word { get; set; }
    }
}
