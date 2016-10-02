using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TwitAnalyser.Dtos;

namespace TwitAnalyser
{
    public class TwitterClient
    {
        private string apiAuthorizationHeader;

        public async Task Authenticate(string oAuthConsumerKey, string oAuthConsumerSecret)
        {
            var oAuthUrl = "https://api.twitter.com/oauth2/token";
            var postBody = "grant_type=client_credentials";

            var authHeader = "Basic " +
                Convert.ToBase64String(Encoding.UTF8.GetBytes(
                    Uri.EscapeDataString(oAuthConsumerKey) + ":" + Uri.EscapeDataString(oAuthConsumerSecret))
            );

            var request = (HttpWebRequest)WebRequest.Create(oAuthUrl);
            request.Headers["Authorization"] = authHeader;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            // TODO: decompress gzip
            // request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            // request.Headers["Accept-Encoding"] = "gzip";

            using (Stream stream = await request.GetRequestStreamAsync())
            {
                byte[] content = ASCIIEncoding.ASCII.GetBytes(postBody);
                stream.Write(content, 0, content.Length);
            }

            using (var response = await request.GetResponseAsync())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var responseContent = reader.ReadToEnd();
                var authenticateResponse = JsonConvert.DeserializeObject<AuthenticateResponse>(responseContent);

                this.apiAuthorizationHeader = $"{authenticateResponse.token_type} {authenticateResponse.access_token}";
            }
        }

        public async Task<IEnumerable<string>> FindUsers(string searchTerm)
        {
            var q = Uri.EscapeDataString(searchTerm);
            // max count = 100, ref: https://dev.twitter.com/rest/reference/get/search/tweets
            var searchUrl = $"https://api.twitter.com/1.1/search/tweets.json?q={q}&count=100";
            var request = (HttpWebRequest)WebRequest.Create(searchUrl);
            request.Headers["Authorization"] = this.apiAuthorizationHeader;
            request.Method = "GET";

            using (var response = await request.GetResponseAsync())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var responseContent = reader.ReadToEnd();
                var searchResponse = JsonConvert.DeserializeObject<SearchResponse>(responseContent);
                return searchResponse.statuses.Select(x => x.user.screen_name).Distinct();
            }
        }

        public async Task<IEnumerable<string>> GetUserTimeline(string screenName)
        {
            // max count = 200, ref: https://dev.twitter.com/rest/reference/get/statuses/user_timeline
            var timelineUrl = $"https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={screenName}&include_rts=1&exclude_replies=0&trim_user=1&count=200";
            var request = (HttpWebRequest)WebRequest.Create(timelineUrl);
            request.Headers["Authorization"] = this.apiAuthorizationHeader;
            request.Method = "GET";

            using (var response = await request.GetResponseAsync())
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var responseContent = reader.ReadToEnd();
                var userTimelineResponse = JsonConvert.DeserializeObject<UserTimelineReponse>(responseContent);
                return userTimelineResponse.Select(x => x.text);
            }
        }
    }
}