using System.Web.Hosting;
using DirtyGirl.Data.DataInterfaces.RepositoryGroups;
using DirtyGirl.Data.RepositoryGroups;
using DirtyGirl.Models;
using DirtyGirl.Models.Enums;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Services.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;


namespace DirtyGirl.Services
{
    public class EmailService : ServiceBase, IEmailService
    {

        private readonly string emailTemplatePath = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", DirtyGirlServiceConfig.Settings.EmailTemplatePath));

        #region constructor

        public EmailService() : base(new RepositoryGroup(), false) { }

        #endregion

        #region Registration Confirmation

        public bool SendRegistrationConfirmationEmail(int userId)
        {
            try
            {
                User user = _repository.Users.Find(x => x.UserId == userId);

                string messageBody = File.ReadAllText(emailTemplatePath + DirtyGirlServiceConfig.Settings.ConfirmationEmailBody)
                    .Replace("{FirstName}", user.FirstName)
                    .Replace("{LastName}", user.LastName)
                    .Replace("{Username}", user.UserName)
                    .Replace("{ConfirmationCode}", user.EmailVerificationCode);

                SendEmail(user.EmailAddress, DirtyGirlServiceConfig.Settings.ConfirmationEmailSubject, messageBody);

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        #endregion

        #region Password Reset Request

        public bool SendPasswordResetRequestEmail(int userId)
        {
            try
            {
                var user = _repository.Users.Find(x => x.UserId == userId);

                string messageBody = File.ReadAllText(emailTemplatePath +
                                                      DirtyGirlServiceConfig.Settings.PasswordResetRequestBody)
                                         .Replace("{Username}",user.UserName)
                                         .Replace("{Firstname}", user.FirstName)
                                         .Replace("{ServerUrl}", DirtyGirlServiceConfig.Settings.ServerUrl)
                                         .Replace("{ResetToken}", user.PasswordResetToken);

                SendEmail(user.EmailAddress, DirtyGirlServiceConfig.Settings.PasswordResetRequestSubject, messageBody);

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        #endregion

        #region Payment Confirmation

        public bool SendPaymentConfirmationEmail(int CartId)
        {
            try
            {
                Cart cart = _repository.Carts.Find(x => x.CartId == CartId);                
                
                StringBuilder cartBody = new StringBuilder();

                String city = string.Empty;
                String date = string.Empty;
                
                foreach (var item in cart.CartItems)
                {
                    cartBody.Append("<tr>");

                    Registration reg;
                    string purchaseName = string.Empty;
                    string purchaseDescription = string.Empty;
                    string purchaseCost = item.Total.ToString();
                                        
                    if (item.PurchaseItem is EventFee)
                    {
                        EventFee fee = (EventFee)item.PurchaseItem;
                        purchaseName = fee.EventFeeType.GetAttributeValue<DisplayAttribute>(x => x.Name);

                        switch (fee.EventFeeType)
                        {
                            case EventFeeType.Transfer:
                                purchaseDescription = "fee associated with transfering your event to a friend.";
                                break;
                            case EventFeeType.Cancellation:
                                purchaseDescription = "fee associated with cancelling your registration.";
                                break;
                            default:
                                reg = _repository.Registrations.Find(x => x.CartItemId == item.CartItemId);
                                if (reg != null)
                                {
                                    purchaseDescription = string.Format("{0}, {1} : {2} {3}", reg.EventWave.EventDate.Event.GeneralLocality, reg.EventWave.EventDate.Event.Region.Code, reg.EventWave.EventDate.DateOfEvent.ToString("dddd  MMMM, dd yyyy"), reg.EventWave.StartTime.ToString("h:mm tt"));
                                    city = reg.EventWave.EventDate.Event.GeneralLocality;
                                    date = reg.EventWave.EventDate.DateOfEvent.Date.ToShortDateString();
                                }
                                break;
                        }                        
                    }

                    if (item.PurchaseItem is CartCharge)
                    {
                        CartCharge charge = (CartCharge)item.PurchaseItem;
                        purchaseName = charge.Name;
                        purchaseDescription = charge.Description;                        
                    }

                    cartBody.Append(string.Format("<td style='padding:0 0 20px;'><table cellpadding='0' cellspacing='0' border='0' bgcolor='#ffffff'><tr><td valign='top' width='150' align='left'><p style='font-family: Arial; font-size: 11px; color:#573f3f;padding:0 10px 0 0;'>{0}</p></td><td valign='top' width='350' align='left'><p style='font-family: Arial; font-size: 11px; color:#573f3f;padding:0 40px 0 0;'>{1}</p></td><td valign='top' width='30' align='left'><p style='font-family: Arial; font-size: 11px; color:#573f3f;'>{2}</p></td></tr></table></td>", purchaseName, purchaseDescription, purchaseCost));

                    cartBody.Append("</tr>");
                }

                cartBody.Append(string.Format("<tr><td><p style='font-family: Arial; font-size: 11px; color:#573f3f;'>Total: {0}</p></td></tr>", cart.TotalCost));

                string messageBody = File.ReadAllText(emailTemplatePath + DirtyGirlServiceConfig.Settings.PaymentConfirmationEmailBody)
                    .Replace("{FirstName}", cart.User.FirstName)
                    .Replace("{LastName}", cart.User.LastName)
                    .Replace("{PaymentId}", cart.TransactionId)
                    .Replace("{City}", city)
                    .Replace("{Date}", date)
                    .Replace("{Cart}", cartBody.ToString());
                
                SendEmail(cart.User.EmailAddress, string.Format(DirtyGirlServiceConfig.Settings.PaymentConfirmationEmailSubject, city), messageBody);

            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }

            return true;
        }

        #endregion

        #region Transfer Email

        public bool SendTransferEmail(int discountItemId, string toName, string email)
        {            
            try
            {
                RedemptionCode redemptionCode = _repository.RedemptionCodes.Find(x => x.DiscountItemId == discountItemId);      

                string messageBody = File.ReadAllText(emailTemplatePath + DirtyGirlServiceConfig.Settings.TransferEmailBody)
                    .Replace("{ToName}", toName)
                    .Replace("{FromName}", redemptionCode.GeneratingRegistration.User.FirstName + " " + redemptionCode.GeneratingRegistration.User.LastName)
                    .Replace("{WaveDetails}", string.Format("{0}, wave start time of {1}",redemptionCode.GeneratingRegistration.EventWave.EventDate.DateOfEvent.ToString("dddd  MMMM, dd yyyy"), redemptionCode.GeneratingRegistration.EventWave.StartTime.ToString("h:mm tt")))
                    .Replace("{Code}", redemptionCode.Code);                   

                SendEmail(email, DirtyGirlServiceConfig.Settings.TransferEmailSubject, messageBody);

                return true;

            }
            catch(Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }

        }

        #endregion

        #region Canellation Email

        public bool SendCancellationEmail(int discountItemId)
        {                       
            try
            {
                RedemptionCode redemptionCode = _repository.RedemptionCodes.Find(x => x.DiscountItemId == discountItemId);

                string messageBody = File.ReadAllText(emailTemplatePath + DirtyGirlServiceConfig.Settings.CancellationEmailBody)
                    .Replace("{Name}", redemptionCode.GeneratingRegistration.FirstName)                   
                    .Replace("{WaveDetails}", string.Format("{0} {1}", redemptionCode.GeneratingRegistration.EventWave.EventDate.DateOfEvent.ToString("dddd  MMMM, dd yyyy"), redemptionCode.GeneratingRegistration.EventWave.StartTime.ToString("h:mm tt")))
                    .Replace("{Code}", redemptionCode.Code);

                SendEmail(redemptionCode.GeneratingRegistration.Email, DirtyGirlServiceConfig.Settings.CancellationEmailSubject, messageBody);

                return true;
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }  
        }

        #endregion        

        #region Team Emails

        public bool SendTeamShareEmail(string[] toAddresses, string subject, string messageBody)
        {
            try
            {
                SendEmail(String.Join(",", toAddresses), subject, messageBody);

                return true;

            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return false;
            }
        }

        #endregion

        #region Share Body
        public string GetShareBodyText(Team team, EventWave eventWave, User user, EventDate eventDate,
                              bool includeJavascriptLineBreaks)
        {
            if (team == null || user == null || eventDate == null)
                return string.Empty;

            var filePath = HostingEnvironment.MapPath(string.Format("~/{0}/{1}", DirtyGirlServiceConfig.Settings.EmailTemplatePath,
                                                      DirtyGirlServiceConfig.Settings.TeamInviteBody)) ?? "";
            var bodyText =
                System.IO.File.ReadAllText(filePath)
                      .Replace("{RegistrantName}",
                               string.Format("{0} {1}", user.FirstName, user.LastName))
                      .Replace("{EventID}", team.Event.EventId.ToString())
                      .Replace("{RaceLocation}", team.Event.GeneralLocality)
                      .Replace("{DayOfWeek}", eventDate.DateOfEvent.ToString("dddd"))
                      .Replace("{Month}", eventDate.DateOfEvent.ToString("MMMM"))
                      .Replace("{Day}", eventDate.DateOfEvent.ToString("dd"))
                      .Replace("{Year}", eventDate.DateOfEvent.ToString("yyyy"))
                      .Replace("{WaveNumber}", eventWave.EventWaveId.ToString())
                      .Replace("{BeginTime}", eventWave.StartTime.ToString("hh:mm tt"))
                      .Replace("{EndTime}", eventWave.EndTime.ToString("hh:mm tt"))
                      .Replace("{TeamCode}", team.Code)
                      .Replace("{LineBreak}", (includeJavascriptLineBreaks ? "\\" : ""));
            return bodyText;
        }
        #endregion

        #region private methods

        private void SendEmail(string toAddresses, string subject, string messageBody)
        {
            SmtpClient smtpClient = new SmtpClient(DirtyGirlServiceConfig.Settings.SMTPServerName, DirtyGirlServiceConfig.Settings.SMTPServerPort);
            smtpClient.Credentials = new NetworkCredential(DirtyGirlServiceConfig.Settings.SMTPServerUsername, DirtyGirlServiceConfig.Settings.SMTPServerPassword);

            string emailContent = File.ReadAllText(emailTemplatePath + DirtyGirlServiceConfig.Settings.EmailTemplateBody)
                .Replace("{Content}", messageBody)
                .Replace("{ServerUrl}", DirtyGirlServiceConfig.Settings.ServerUrl);              
            
            MailMessage message = new MailMessage(DirtyGirlServiceConfig.Settings.EmailServiceFromAddress, toAddresses, subject, emailContent);
            message.IsBodyHtml = true;

            smtpClient.Send(message);
        }



        #endregion

       
    }
}
