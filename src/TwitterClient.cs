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

        public void Authenticate(string oAuthConsumerKey, string oAuthConsumerSecret)
        {
            var auth = AuthenticateAsync(oAuthConsumerKey, oAuthConsumerSecret).Result;
            this.apiAuthorizationHeader = $"{auth.token_type} {auth.access_token}";
            Console.WriteLine(this.apiAuthorizationHeader);
        }

        private static async Task<AuthenticateResponse> AuthenticateAsync(string oAuthConsumerKey, string oAuthConsumerSecret)
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
                return JsonConvert.DeserializeObject<AuthenticateResponse>(responseContent);
            }
        }

        public async Task<IEnumerable<string>> FindUsersAsync(string searchTerm)
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
    }
}