using System;
using System.IO;
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
    }
}