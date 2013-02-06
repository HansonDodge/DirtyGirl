using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Utils
{
    public class DirtyGirlConfig : ConfigurationSection
    {
        private static DirtyGirlConfig settings = ConfigurationManager.GetSection("DirtyGirlConfigurationSettings") as DirtyGirlConfig;

        public static DirtyGirlConfig Settings
        {
            get { return settings; }
        }

        [ConfigurationProperty("DefaultCountryId")]
        public int DefaultCountryId
        {
            get { return int.Parse(this["DefaultCountryId"].ToString()); }
        }

        [ConfigurationProperty("DefaultMessageKey")]
        public string DisplayMessageKey
        {
            get { return this["DefaultMessageKey"].ToString(); }
        }

        [ConfigurationProperty("EventImageFolder")]
        public string EventImageFolder
        {
            get { return this["EventImageFolder"].ToString(); }
        }

        [ConfigurationProperty("LogoHieght")]
        public int LogoHieght
        {
            get { return int.Parse(this["LogoHieght"].ToString()); }
        }

        [ConfigurationProperty("LogoWidth")]
        public int LogoWidth
        {
            get { return int.Parse(this["LogoWidth"].ToString()); }
        }

        [ConfigurationProperty("GoogleAPIKey")]
        public string GooleAPIKey
        {
            get {return this["GoogleAPIKey"].ToString();}
        }

        [ConfigurationProperty("DisplaySpotsAvailableCount")]
        public int DisplaySpotsAvailableCount
        {
            get {return int.Parse(this["DisplaySpotsAvailableCount"].ToString()); }
        }

        [ConfigurationProperty("CurrentCartKey")]
        public string CurrentCartKey
        {
            get { return this["CurrentCartKey"].ToString(); }
        }

        [ConfigurationProperty("CurrentTransactionKey")]
        public string CurrentTransactionKey
        {
            get { return this["CurrentTransactionKey"].ToString(); }
        }

        [ConfigurationProperty("ServerUrl")]
        public string ServerUrl
        {
            get { return this["ServerUrl"].ToString(); }
        }
                
    }
    
}