using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DirtyGirl.Models
{

    public enum DisplayMessageType
    {
        General,
        ErrorMessage,
        SuccessMessage,
        Warning        
    }

    public class DisplayMessage
    {
        public DisplayMessageType MessageType { get; set; }

        [DisplayFormat(NullDisplayText="")]
        public string Message { get; set; }

        public DisplayMessage() { }

        public DisplayMessage(DisplayMessageType type, string message)
        {
            this.MessageType = type;
            this.Message = message;
        }
    }

}
