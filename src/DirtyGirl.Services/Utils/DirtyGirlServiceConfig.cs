using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DirtyGirl.Services.Utils
{
    public class DirtyGirlServiceConfig : ConfigurationSection
    {
        private static DirtyGirlServiceConfig settings = ConfigurationManager.GetSection("DirtyGirlServiceConfigurationSettings") as DirtyGirlServiceConfig;
                
        public static DirtyGirlServiceConfig Settings
        {
            get { return settings; }
        }


        #region SMTP Configurations

        [ConfigurationProperty("SMTPServerName")]
        public string SMTPServerName
        {
            get { return this["SMTPServerName"].ToString(); }
        }

        [ConfigurationProperty("SMTPServerPort")]
        public int SMTPServerPort
        {
            get { return int.Parse(this["SMTPServerPort"].ToString()); }
        }

        [ConfigurationProperty("SMTPServerUsername")]
        public string SMTPServerUsername
        {
            get { return this["SMTPServerUsername"].ToString(); }
        }

        [ConfigurationProperty("SMTPServerPassword")]
        public string SMTPServerPassword
        {
            get { return this["SMTPServerPassword"].ToString(); }
        }

        [ConfigurationProperty("SMTPServerFromAddress")]
        public string SMTPServerFromAddress
        {
            get { return this["SMTPServerFromAddress"].ToString(); }
        }

        #endregion

        #region server properties

        [ConfigurationProperty("ServerUrl")]
        public string ServerUrl
        {
            get { return this["ServerUrl"].ToString(); }
        }

        #endregion

        #region Email Properties

        [ConfigurationProperty("EmailTemplateBody")]
        public string EmailTemplateBody
        {
            get { return this["EmailTemplateBody"].ToString(); }
        }

        [ConfigurationProperty("EmailServiceFromAddress")]
        public string EmailServiceFromAddress
        {
            get {return this["EmailServiceFromAddress"].ToString();}
        }

        [ConfigurationProperty("EmailTemplatePath")]
        public string EmailTemplatePath
        {
            get { return this["EmailTemplatePath"].ToString(); }
        }

        #endregion

        #region Transfer Email

        [ConfigurationProperty("TransferEmailBody")]
        public string TransferEmailBody
        {
            get { return this["TransferEmailBody"].ToString(); }
        }

        [ConfigurationProperty("TransferEmailSubject")]
        public string TransferEmailSubject
        {
            get { return this["TransferEmailSubject"].ToString(); }
        }

        #endregion

        #region Cancellation Email

        [ConfigurationProperty("CancellationEmailBody")]
        public string CancellationEmailBody
        {
            get { return this["CancellationEmailBody"].ToString(); }
        }

        [ConfigurationProperty("CancellationEmailSubject")]
        public string CancellationEmailSubject
        {
            get { return this["CancellationEmailSubject"].ToString(); }
        }

        #endregion

        #region Confirmation Email

        [ConfigurationProperty("UsersMustConfirmEmail", DefaultValue="false")]
        public bool UsersMustConfirmEmail
        {
            get { return Boolean.Parse(this["UsersMustConfirmEmail"].ToString()); }
        }

        [ConfigurationProperty("ConfirmationEmailBody")]
        public string ConfirmationEmailBody
        {
            get { return this["ConfirmationEmailBody"].ToString(); }
        }

        [ConfigurationProperty("ConfirmationEmailSubject")]
        public string ConfirmationEmailSubject
        {
            get { return this["ConfirmationEmailSubject"].ToString(); }
        }


        #endregion

        #region Password Reset Request

        [ConfigurationProperty("RequestValidForHours", DefaultValue="24")]
        public int RequestValidForHours
        {
            get { return Int32.Parse(this["RequestValidForHours"].ToString()); }
        }

        [ConfigurationProperty("WaitMinutesBetweenPasswordResetRequestEmailSent", DefaultValue = "5")]
        public int WaitMinutesBetweenPasswordResetRequestEmailSent
        {
            get { return Int32.Parse(this["WaitMinutesBetweenPasswordResetRequestEmailSent"].ToString()); }
        }

        [ConfigurationProperty("PasswordResetRequestBody")]
        public string PasswordResetRequestBody
        {
            get { return this["PasswordResetRequestBody"].ToString(); }
        }

        [ConfigurationProperty("PasswordResetRequestSubject")]
        public string PasswordResetRequestSubject
        {
            get { return this["PasswordResetRequestSubject"].ToString(); }
        }

        #endregion

        #region PaymentConfirmation

        [ConfigurationProperty("PaymentConfirmationEmailBody")]
        public string PaymentConfirmationEmailBody
        {
            get { return this["PaymentConfirmationEmailBody"].ToString(); }
        }

        [ConfigurationProperty("PaymentConfirmationEmailSubject")]
        public string PaymentConfirmationEmailSubject
        {
            get { return this["PaymentConfirmationEmailSubject"].ToString(); }
        }

        #endregion

        #region Team Invite
        [ConfigurationProperty("TeamInviteBody")]
        public string TeamInviteBody
        {
            get { return this["TeamInviteBody"].ToString(); }
        }
        #endregion

        #region Payment Gateway

        [ConfigurationProperty("PaymentGatewayId")]
        public string PaymentGatewayId
        {
            get { return this["PaymentGatewayId"].ToString(); }
        }

        [ConfigurationProperty("PaymentGatewayKey")]
        public string PaymentGatewayKey
        {
            get { return this["PaymentGatewayKey"].ToString(); }
        }

        [ConfigurationProperty("PaymentPostUrl")]
        public string PaymentPostUrl
        {
            get { return this["PaymentPostUrl"].ToString(); }
        }

        [ConfigurationProperty("PaymentTestMode")]
        public bool PaymentTestMode
        {
            get { return Convert.ToBoolean(this["PaymentTestMode"].ToString()); }
        }

        #endregion

        #region Transfer Days

        [ConfigurationProperty("MaxTransferHeldDays")]
        public int MaxTransferHeldDays
        {
            get { return int.Parse(this["MaxTransferHeldDays"].ToString()); }
        }

        #endregion
    }
}
