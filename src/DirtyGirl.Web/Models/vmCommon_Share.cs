using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DirtyGirl.Web.Models
{
    public class vmCommon_Share
    {
        public int InstanceNumber { get; set; }

        [Required(ErrorMessage="Email address required")]
        public string EmailAddresses { get; set; }

        [Required(ErrorMessage = "A message is required")]
        public string UserMessageBody { get; set; }

        [Required(ErrorMessage="A message is required")]
        public string MessageBody { get; set; }

        public string Title { get; set; }

        [Required(ErrorMessage="A subject is required")]
        public string MessageSubject { get; set; }


    }
}