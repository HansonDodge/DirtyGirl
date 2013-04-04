using System.Globalization;
using System.IO;
using DirtyGirl.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DirtyGirl.Services;
using DirtyGirl.Services.ServiceInterfaces;
using DirtyGirl.Models.Enums;

namespace DirtyGirl.Web.Utils
{
    public static class Utilities
    {
        public static void AddModelStateErrors(ModelStateDictionary currentState, IEnumerable<ServiceError> errors)
        {
            foreach(var error in errors)
            {
                currentState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
        }

        public static Image ResizeImage(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            nPercent = nPercentH < nPercentW ? nPercentH : nPercentW;

            var destWidth = (int)(sourceWidth * nPercent);
            var destHeight = (int)(sourceHeight * nPercent);

            var b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b; 

        }        

        //public static IList<SelectListItem> CreateSelectList<T>(IList<T> entities, Func<T, object> funcToGetValue, Func<T, object> funcToGetText)
        //{
        //    return CreateSelectList(entities, funcToGetValue, funcToGetText, true);
        //}

        public static IList<SelectListItem> CreateSelectList<T>(IEnumerable<T> entities, Func<T, object> funcToGetValue, Func<T, object> funcToGetText, bool addDefaultSelectItem = true)
        {
            var eList = entities
                   .Select(x => new SelectListItem
                   {
                       Value = funcToGetValue(x).ToString(),
                       Text = funcToGetText(x).ToString()
                   }).ToList();

            if (addDefaultSelectItem)
                eList.Insert(0, new SelectListItem { Selected = true, Text = "Select", Value = null });

            return eList;

        }

        public static string GetRootDomain(HttpRequest request)
        {
            return string.Format("{0}://{1}{2}{3}",
                                    request.Url.Scheme,
                                    request.Url.Host,
                                    request.Url.Port == 80 ? string.Empty : ":" + request.Url.Port, request.ApplicationPath);
        }

        public static string GetShareBodyText(Team team, EventWave eventWave, User user, EventDate eventDate,
                                              bool includeJavascriptLineBreaks)
        {
            IEmailService emailService = new EmailService();
            return emailService.GetShareBodyText(team, eventWave, user, eventDate, includeJavascriptLineBreaks);
        }

        public static IList<SelectListItem> CreateNumericSelectList(int startNumber, int endNumber)
        {

            var list = new List<SelectListItem>();

            if (startNumber < endNumber)
            {
                for (int i = startNumber; i <= endNumber; i++)
                {
                    list.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = false });
                }
            }
            else
            {
                for (int i = endNumber; i <= startNumber; i++)
                {
                    list.Add(new SelectListItem { Text = i.ToString(CultureInfo.InvariantCulture), Value = i.ToString(CultureInfo.InvariantCulture), Selected = false });
                }
            }

            return list;
        }

        private static byte[] ReadFully(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.Position = 0;
                input.CopyTo(ms);
                input.Position = 0;
                return ms.ToArray();
            }
        }

        public static bool VerifyFileIsImage(MemoryStream stream)
        {
            byte[] fileData = ReadFully(stream);

            return (fileData.IsGif() || fileData.IsJpeg() || fileData.IsPng());
        }

        public static bool IsValidCart()
        {
            if (SessionManager.CurrentCart == null || 
                SessionManager.CurrentCart.ActionItems == null ||
                SessionManager.CurrentCart.ActionItems.Count() == 0)
            {             
                return false;
            }

            return true;
        }

        public static bool IsRegistrationInCart(int regId)
        {
            if (!IsValidCart())
                return false;

            foreach (var actionItem in SessionManager.CurrentCart.ActionItems.Values)
            {
                if (!actionItem.ItemReadyForCheckout)
                    continue;

                if (actionItem.ActionType == CartActionType.CancelRegistration)
                {
                    var cancelAction = (CancellationAction)actionItem.ActionObject;
                    if (cancelAction.RegistrationId == regId )
                        return true;
                }
                if (actionItem.ActionType == CartActionType.TransferRregistration)
                {
                    var transferAction = (TransferAction)actionItem.ActionObject;
                    if (transferAction.RegistrationId == regId)
                        return true;
                }
                if (actionItem.ActionType == CartActionType.EventChange)
                {
                    if (actionItem.ItemReadyForCheckout == false)
                        return false;

                    var changeAction = (ChangeEventAction)actionItem.ActionObject;
                    if (changeAction.RegistrationId == regId)
                        return true;
                }

            }
            return false;
        }

    }
}