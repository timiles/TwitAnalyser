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

        public async Task<IEnumerable<string>> FindUsers(string searchTerm, int? numberOfUsersToGet = null)
        {
            Func<string, Task<SearchResponse>> getSearchResponse = async (url) =>
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Headers["Authorization"] = this.apiAuthorizationHeader;
                request.Method = "GET";

                using (var response = await request.GetResponseAsync())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseContent = reader.ReadToEnd();
                    var searchResponse = JsonConvert.DeserializeObject<SearchResponse>(responseContent);
                    return searchResponse;
                }
            };

            var q = Uri.EscapeDataString(searchTerm);
            // max count = 100, ref: https://dev.twitter.com/rest/reference/get/search/tweets
            var searchUrl = $"https://api.twitter.com/1.1/search/tweets.json?q={q}&count=100";
            var nextSearchUrl = searchUrl;

            var users = new List<string>();
            do
            {
                var searchResponse = await getSearchResponse(nextSearchUrl);
                users.AddRange(searchResponse.statuses.Select(x => x.user.screen_name.ToLower()).Distinct());
                // don't use searchResponse.search_metadata.next_results, often returns null :(
                var minId = searchResponse.statuses.Select(x => x.id).Min();
                nextSearchUrl = searchUrl + $"&max_id={(minId - 1)}";
            }
            while (users.Count() < numberOfUsersToGet);

            return users;
        }

        public async Task<IEnumerable<Status>> GetUserTimeline(string screenName)
        {
            try
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
                    return userTimelineResponse;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + screenName + ", " + ex.ToString());
                return null;
            }
        }
    }
}