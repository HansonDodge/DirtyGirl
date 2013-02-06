using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace FacebookOpenGraph.Models
{
    [JsonObject(MemberSerialization.OptOut)]
    public class FacebookUser
    {
        [JsonProperty(PropertyName="id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName="first_name")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName="last_name")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        public string AccessToken { get; set; }

        public string Code { get; set; }

        public FacebookUser()
        {
                     
        }

    }
}
