using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Models
{
    public class vmRegistration_WaveList
    {    
        public IList<vmRegistration_WaveItem> MorningWaves{get; set;}

        public IList<vmRegistration_WaveItem> EveningWaves{get; set;}     
    }
}