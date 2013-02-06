using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using FacebookOpenGraph.Config;
using FacebookOpenGraph.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Collections;
using System.Collections.Generic;
using FacebookOpenGraph.Authentication;

namespace FacebookOpenGraph.Graph
{
    public static class FacebookGraph
    {

       #region Public Methods

        public static GraphListResponse<Album> GetAlbums(string id, string accessToken = "")
        {
            GraphListResponse<Album> data = new GraphListResponse<Album>();
            string rawData = GetRawJson(id, accessToken, "albums");
            if (!string.IsNullOrEmpty(rawData))
            {
                JObject rawJson = JObject.Parse(rawData);
                string error = (String)rawJson["error"];

                if (string.IsNullOrEmpty(error))
                {
                    data = JsonConvert.DeserializeObject<GraphListResponse<Album>>(rawData);
                }

                rawJson = null;
            }
            return data;
        }


        public static GraphListResponse<Post> GetPosts(string id, string accessToken = "")
        {
            if (accessToken == "")
                accessToken = FacebookAuthentication.GetFacebookApplicationToken().Token;

            GraphListResponse<Post> data = new GraphListResponse<Post>();
            string rawData = GetRawJson(id, accessToken, "posts");
            if (!string.IsNullOrEmpty(rawData))
            {
                JObject rawJson = JObject.Parse(rawData);
                string error = (String)rawJson["error"];

                if (string.IsNullOrEmpty(error))
                {
                    data = JsonConvert.DeserializeObject<GraphListResponse<Post>>(rawData);
                }

                rawJson = null;
            }
            return data;
        }

        public static GraphListResponse<Photo> GetPhotos(string id, string accessToken = "")
        {
            GraphListResponse<Photo> data = new GraphListResponse<Photo>();
            string rawData = GetRawJson(id, accessToken, "photos");
            if (!string.IsNullOrEmpty(rawData))
            {
                JObject rawJson = JObject.Parse(rawData);
                string error = (String)rawJson["error"];

                if (string.IsNullOrEmpty(error))
                {
                    data = JsonConvert.DeserializeObject<GraphListResponse<Photo>>(rawData);
                }

                rawJson = null;
            }
            return data;
        }

        public static GraphListResponse<Post> GetFeed(string id, string accessToken)
        {
            GraphListResponse<Post> data = new GraphListResponse<Post>();                        
            string rawData = GetRawJson(id, accessToken, "feed");
            if (!string.IsNullOrEmpty(rawData))
            {
                JObject rawJson = JObject.Parse(rawData);
                string error = (String)rawJson["error"];

                if (string.IsNullOrEmpty(error))
                {
                    data = JsonConvert.DeserializeObject<GraphListResponse<Post>>(rawData);                                
                }

                rawJson = null;
            }
            return data;
        }


        public static GraphListResponse<LikeObject> GetLikes(string id, string accessToken)
        {            
            GraphListResponse<LikeObject> data = new GraphListResponse<LikeObject>();
            string rawData = GetRawJson(id, accessToken, "likes");
            if (!string.IsNullOrEmpty(rawData))
            {
                JObject rawJson = JObject.Parse(rawData);
                string error = (String)rawJson["error"];

                if (string.IsNullOrEmpty(error))
                {
                    data = JsonConvert.DeserializeObject<GraphListResponse<LikeObject>>(rawData);
                }

                rawJson = null;
            }
            return data;
        }

        public static string GetRawJson(string id, string accessToken, string connectionType)
        {
            WebClient wc = new WebClient();
            string graphCall = string.Format("{0}/{1}/{2}?access_token={3}", FacebookSettings.Settings.GraphUrl, id, connectionType, accessToken);
            string rawData = wc.DownloadString(graphCall);
            return rawData;
        }

        public static string GetProfilePhoto(string id)
        {
            return string.Format("{0}/{1}/picture?type=large", FacebookSettings.Settings.GraphUrl, id);
        }

        public static FacebookUser GetPublicData(string accessToken)
        {

            WebClient wc = new WebClient();  
            FacebookUser fbUser = new FacebookUser();            
            string graphCall = string.Format("{0}/me?access_token={1}", FacebookSettings.Settings.GraphUrl, accessToken);
            string rawData = wc.DownloadString(graphCall);

            if (!string.IsNullOrEmpty(rawData))
            {
                JObject rawJson = JObject.Parse(rawData);
                string error = (String)rawJson["error"];                

                if (string.IsNullOrEmpty(error))
                    fbUser = JsonConvert.DeserializeObject<FacebookUser>(rawData);
                else
                    //fbUser.FBError = JsonConvert.DeserializeObject<FacebookError>(rawData);

                rawJson = null;
            }
                        
            wc.Dispose();            

            return fbUser;
        }

        public static bool PostToWall(string accessToken, string post)
        {
            return PostToWall(accessToken,post, "me");
        }

        public static bool PostToWall(string accesstoken, string post, string friend)
        {
            return PostToWall(accesstoken, post, "me", "");
        }

        public static bool PostToWall(string accesstoken, string post, string friend, string link)
        {
            return PostToWall(accesstoken, post, "me", "", "");
        }

        public static bool PostToWall(string accesstoken, string post, string friend, string link, string image)
        {
            WebClient wc = new WebClient();  
           
            string graphCall = string.Format("{0}{1}/feed/", FacebookSettings.Settings.GraphUrl, friend);

            NameValueCollection fbData = new NameValueCollection();

            fbData.Add("access_token", accesstoken);
            fbData.Add("message", post);
            if (!string.IsNullOrEmpty(link))
                fbData.Add("link", link);
            if (!string.IsNullOrEmpty(image))
                fbData.Add("picture", image);

            try
            {
                byte[] fbResponse = wc.UploadValues(graphCall, "POST", fbData);
                string fbResult = Encoding.UTF8.GetString(fbResponse);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                wc.Dispose();
            }

            return true;           
           
        }

        #endregion

    }
}