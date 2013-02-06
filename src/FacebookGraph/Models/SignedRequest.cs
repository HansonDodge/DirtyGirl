using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace FacebookOpenGraph.Models
{
    [DataContract]
    public class FaceBookPayload
    {
        [DataMember(Name = "algorithm")]
        public string Algorithm { get; set; }

        [DataMember(Name = "expires")]
        public string Expires { get; set; }

        [DataMember(Name = "issued_at")]
        public string Issued_At { get; set; }

        [DataMember(Name = "oath_token")]
        public string AuthToken { get; set; }

        [DataMember(Name = "user")]
        public FacebookPayLoadUser PayloadUser { get; set; }

        [DataMember(Name = "page")]
        public FaceBookPage Page { get; set; }

        [DataMember(Name = "user_id")]
        public string FaceBook_id { get; set; }

        [DataMember(Name = "app_data")]
        public string AppData { get; set; }
    }

    [DataContract]
    public class FaceBookPage
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "liked")]
        public bool Liked { get; set; }

        [DataMember(Name = "admin")]
        public bool Admin { get; set; }
    }

    [DataContract]
    public class FacebookPayLoadUser
    {
        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "locale")]
        public string Locale { get; set; }

        [DataMember(Name = "age")]
        public FaceBookAge Age { get; set; }
    }

    [DataContract]
    public class FaceBookAge
    {
        [DataMember(Name = "min")]
        public string Min { get; set; }
    }

    [DataContract]
    public class AccessToken
    {
        [DataMember(Name = "access_token")]
        public string Token { get; set; }

        [DataMember(Name = "expires")]
        public long Expires { get; set; }
    }
}
