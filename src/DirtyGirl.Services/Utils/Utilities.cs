using DirtyGirl.Models;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace DirtyGirl.Services.Utils
{
    public static class Utilities
    {
        public static string GetInnerMostException(Exception ex)
        {
            return ex.GetBaseException().Message;
        }

        public static byte[] ResizeImage(byte[] imageToResize, int desiredWidth, int desiredHeight)
        {
            var msIn = new MemoryStream(imageToResize);
            Image workingImage = Image.FromStream(msIn);

            int sourceWidth = workingImage.Width;
            int sourceHeight = workingImage.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)desiredWidth / (float)sourceWidth);
            nPercentH = ((float)desiredHeight / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((desiredWidth -
                              (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((desiredHeight -
                              (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(desiredWidth, desiredHeight,
                              PixelFormat.Format32bppArgb);
            bmPhoto.SetResolution(workingImage.HorizontalResolution,
                             workingImage.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Transparent);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(workingImage,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();

            MemoryStream msOut = new MemoryStream();
            bmPhoto.Save(msOut, ImageFormat.Png);
            return msOut.ToArray();
        }

        private static ServiceResult SendEmail(string fromAddress, string[] toAddresses, string subject, string messageBody, bool isHtml, SmtpClient smtpClient, ICredentialsByHost hostCredentials)
        {
            var result = new ServiceResult();
            try
            {
                smtpClient.Credentials = hostCredentials;
                var message = new MailMessage(fromAddress, string.Join("; ", toAddresses), subject, messageBody);
                message.IsBodyHtml = isHtml;
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                result.AddServiceError(Utilities.GetInnerMostException(ex));
            }
            return result;
        }

        public static ServiceResult SendEmail(string fromAddress, string[] toAddresses, string subject, string messageBody, bool isHtml)
        {
            var smtpClient = new SmtpClient(DirtyGirlServiceConfig.Settings.SMTPServerName, DirtyGirlServiceConfig.Settings.SMTPServerPort);
            smtpClient.Credentials = new NetworkCredential(DirtyGirlServiceConfig.Settings.SMTPServerUsername, DirtyGirlServiceConfig.Settings.SMTPServerPassword);
            return SendEmail(fromAddress, toAddresses, subject, messageBody, isHtml, smtpClient, smtpClient.Credentials);
        }

        public  static DateTime AdjustCurrentTimeForTimezone()
        {
            return DateTime.Now.AddHours(DirtyGirlServiceConfig.Settings.TimezoneOffset);
        }
    }
}
