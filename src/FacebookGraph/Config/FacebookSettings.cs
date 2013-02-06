using System;
using System.Collections;
using System.Configuration;

namespace FacebookOpenGraph.Config
{ 
    public class FacebookSettings : ConfigurationSection
    {

        private static FacebookSettings settings = ConfigurationManager.GetSection("FacebookConfigurationSettings/FacebookSettings") as FacebookSettings;

        public static FacebookSettings Settings
        {
            get { return settings; }
        }

        [ConfigurationProperty("AuthUrl", DefaultValue = "https://graph.facebook.com/oauth/authorize", IsRequired = false)]
        public string AuthUrl
        {
            get { return this["AuthUrl"].ToString(); }
        }

        [ConfigurationProperty("RedirectUri", DefaultValue="http://www.facebook.com/connect/login_success.html",  IsRequired = false)]
        public string RedirectUri
        {
            get { return this["RedirectUri"].ToString(); }
        }

        [ConfigurationProperty("GraphUrl", DefaultValue = "https://graph.facebook.com/", IsRequired = false)]
        public string GraphUrl
        {
            get { return this["GraphUrl"].ToString(); }
        }

        [ConfigurationProperty("AppSecret", IsRequired = true)]
        public string AppSecret
        {
            get { return this["AppSecret"].ToString(); }
        }

        [ConfigurationProperty("ClientId", IsRequired = true)]
        public string ClientId
        {
            get { return this["ClientId"].ToString(); }
        }

        [ConfigurationProperty("Scope", DefaultValue = "user_about_me", IsRequired= false)]
        public string Scope
        {
            get {return this["Scope"].ToString();}
        }

        [ConfigurationProperty("Display", DefaultValue = "page", IsRequired = false)]
        public string Display
        {
            get { return this["Display"].ToString(); }
        }

        [ConfigurationProperty("LogoutRedirectUri", DefaultValue = "http://www.facebook.com/connect/login_success.html", IsRequired = false)]
        public string LogoutRedirectUri
        {
            get { return this["LogoutRedirectUri"].ToString(); }
        }


    }
}
