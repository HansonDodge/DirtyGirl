using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DirtyGirl.Models
{
    public class EventDateOverview
    {

        public int EventId { get; set; }

        public string Place { get; set; }

        public string GeneralLocality { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string StateCode { get; set; }

        public string Zip { get; set; }

        public string Location
        {
            get { return string.Format("{0}, {1}", GeneralLocality, StateCode); }
        }

        public DateTime DateOfEvent { get; set; }

        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal CurrentCost { get; set; }

        public int MaxRegistrants { get; set; }

        public int RegistrationCount { get; set; }

        public bool isRegistrationCutoff { get; set; }

        public int SpotsLeft
        {
            get { return MaxRegistrants - RegistrationCount < 0 ? 0 : MaxRegistrants - RegistrationCount; }
        }

        public bool isFull
        {
            get { return SpotsLeft <= 0; }
        }

        public string EventUrl
        {
            get { 
                
                string seo = StateCode;                             // default to just state
                if (!String.IsNullOrWhiteSpace(GeneralLocality))    // if locality exists, make it locality + state
                {
                    seo = GeneralLocality+"-"+StateCode;
                }
                else
                {
                    if (!string.IsNullOrEmpty(City))                // if no locality, check if city exists
                    {
                        seo = City + "-" + StateCode;
                    }
                    
                }
                seo = seo.Replace(" ", "-");        // replace spaces with dashes
                seo = Regex.Replace(seo, "[^a-zA-Z0-9_.-]+", "-", RegexOptions.Compiled); // get rid of any non valid characters
                string url = "/mud-run/" + seo + "/" + EventId.ToString(CultureInfo.InvariantCulture);

                return url;
            }
        }
        public bool DisplayIcon { get; set; }

        public string IconImagePath { get; set; }

        public string PinXCoordinate { get; set; }

        public string PinYCoordinate { get; set; }
    }
}
