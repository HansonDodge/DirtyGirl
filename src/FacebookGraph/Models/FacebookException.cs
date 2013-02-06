using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Security;

namespace FacebookOpenGraph.Models
{
    [Serializable]
    // Important: This attribute is NOT inherited from Exception, and MUST be specified 
    // otherwise serialization will fail with a SerializationException stating that
    // "Type X in Assembly Y is not marked as serializable."
    public class FacebookException : Exception
    {
        private readonly string facebookResult;

        public FacebookException()
        {
        }

        public FacebookException(string message)
            : base(message)
        {
        }

        public FacebookException(string message, Exception innerException)
            : base(message, innerException)
        {
        }


        public FacebookException(string message, string facebookResult, Exception innerException)
            : base(message, innerException)
        {
            this.facebookResult = facebookResult;
        }

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        // Constructor should be protected for unsealed classes, private for sealed classes.
        // (The Serializer invokes this constructor through reflection, so it can be private)
        protected FacebookException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.facebookResult = info.GetString("FacebookResult");
        }

        public string FacebookResult
        {
            get { return this.facebookResult; }
        }

        
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        [SecurityCritical]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("ResourceName", this.FacebookResult);

            // MUST call through to the base class to let it save its own state
            base.GetObjectData(info, context);
        }
    }
}
