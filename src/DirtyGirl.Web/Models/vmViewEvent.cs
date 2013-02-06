using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Models
{
    public class vmViewEvent
    {
        public EventOverview OverView {get; set;}
        public EventDetails EventDetails { get; set; }
    }
}