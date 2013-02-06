using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FacebookOpenGraph.Config;
using FacebookOpenGraph.Models;

using FacebookOpenGraph.Graph;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace FacebookOpenGraph.Authentication
{
    public class FacebookAuthentication
    {
        #region FacebookAuthenticationUrl

        public static string GetFaceBookAuthUrl()
        {
            return string.Format("{0}?client_id={1}&scope={2}&redirect_uri={3}",
                                FacebookSettings.Settings.AuthUrl,
                                FacebookSettings.Settings.ClientId,
                                FacebookSettings.Settings.Scope,
                                FacebookSettings.Settings.RedirectUri);
        }

        #endregion


        #region GetFacebookPayload

        public static FaceBookPayload GetFaceBookPayload(string signedRequest)
        {
            FaceBookPayload fbPayload = new FaceBookPayload();
            string decodedPayload;

            ValidateSignedRequest(signedRequest, out decodedPayload);

            if (!string.IsNullOrEmpty(decodedPayload))
            {
                using (MemoryStream stream = new MemoryStream(System.Text.ASCIIEncoding.ASCII.GetBytes(decodedPayload)))
                {
                    DataContractJsonSerializer json = new DataContractJsonSerializer(typeof(FaceBookPayload));
                    fbPayload = json.ReadObject(stream) as FaceBookPayload;
                }
            }

            return fbPayload;
        }

        public static AccessToken GetFacebookApplicationToken()
        {
            string url = string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&client_secret={1}&grant_type=client_credentials", 
                                            FacebookSettings.Settings.ClientId, 
                                            FacebookSettings.Settings.AppSecret);
            WebClient wc = new WebClient();
            string tokenResponse = "";

            try
            {
                tokenResponse = wc.DownloadString(url);
                Regex rx = new Regex("^access_token=([\\w\\W]+)$");
                Match match = rx.Match(tokenResponse);

                if (match.Groups.Count >= 2)
                {
                    AccessToken token = new AccessToken { Token = match.Groups[1].Value };
                    return token;
                }
                else
                    return null;
            }
            catch (System.Net.WebException wex)
            {
                StreamReader sr = new StreamReader(wex.Response.GetResponseStream());
                string response = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
                //throw new Exception(response);
                throw new FacebookException("AuthException", response, wex);
            }
        }

        public static AccessToken GetFacebookAccessToken(string code)
        {
            string getTokenUrl = string.Format("https://graph.facebook.com/oauth/access_token?client_id={0}&redirect_uri={1}&client_secret={2}&code={3}",
                                                    FacebookSettings.Settings.ClientId,
                                                    FacebookSettings.Settings.RedirectUri,
                                                    FacebookSettings.Settings.AppSecret,
                                                    code);
            WebClient wc = new WebClient();
            string tokenResponse = "";

            try
            {
                tokenResponse = wc.DownloadString(getTokenUrl);
                Regex rx = new Regex("^access_token=(\\w+)&expires=(\\d+)$");
                Match match = rx.Match(tokenResponse);

                if (match.Groups.Count >= 3)
                {
                    AccessToken token = new AccessToken { Token = match.Groups[1].Value, Expires = Convert.ToInt64(match.Groups[2].Value) };
                    return token;
                }
                else
                    return null;
            }
            catch (System.Net.WebException wex)
            {
                string response = "";
                if (wex.Response != null)
                {
                    StreamReader sr = new StreamReader(wex.Response.GetResponseStream());
                    response = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();
                }
                //throw new Exception(response);
                throw new FacebookException("AuthException", response, wex);
            }
            catch (Exception ex)
            {
                throw new FacebookException("AuthException", tokenResponse, ex);
            }
        }

        #region Validate Signed Request
        private static bool ValidateSignedRequest(string signedRequest, out string decodedPayload)
        {
            decodedPayload = null;
            string applicationSecret = Config.FacebookSettings.Settings.AppSecret;
            string[] signedRequestArray = signedRequest.Split('.');
            string expectedSignature = signedRequestArray[0];
            string payload = signedRequestArray[signedRequestArray.Length - 1];

            var Hmac = SignWithHmac(UTF8Encoding.UTF8.GetBytes(payload), UTF8Encoding.UTF8.GetBytes(applicationSecret));
            var HmacBase64 = ToUrlBase64String(Hmac);

            bool result = (HmacBase64 == expectedSignature);
            if (result)
            {
                decodedPayload = UTF8Encoding.UTF8.GetString(FromBase64ForUrlString(payload));
            }
            return result;
        }

        #endregion

        #region From Base64ForUrlstring
        private static byte[] FromBase64ForUrlString(string base64ForUrlInput)
        {
            int padChars = (base64ForUrlInput.Length % 4) == 0 ? 0 : (4 - (base64ForUrlInput.Length % 4));

            StringBuilder result = new StringBuilder(base64ForUrlInput, base64ForUrlInput.Length + padChars);
            result.Append(String.Empty.PadRight(padChars, '='));

            result.Replace('-', '+');
            result.Replace('_', '/');

            return Convert.FromBase64String(result.ToString());
        }
        #endregion

        #region ToUrlBase54String
        private static string ToUrlBase64String(byte[] Input)
        {
            return Convert.ToBase64String(Input).Replace("=", String.Empty)
                                                .Replace('+', '-')
                                                .Replace('/', '_');
        }
        #endregion

        #region SignWtithHmac
        private static byte[] SignWithHmac(byte[] dataToSign, byte[] keyBody)
        {
            using (var hmacAlgorithm = new HMACSHA256(keyBody))
            {
                hmacAlgorithm.ComputeHash(dataToSign);
                return hmacAlgorithm.Hash;
            }
        }
        #endregion

        #endregion

    }
}
