using System;
using System.IO;
using System.Threading.Tasks;

namespace TwitAnalyser
{
    public class Program
    {
        const int UsersPerSearchTerm = 1000;

        public static void Main(string[] args)
        {
            var oAuthConsumerKey = args[0];
            var oAuthConsumerSecret = args[1];

            var searchTerm = args[2];

            var task = RunAsync(oAuthConsumerKey, oAuthConsumerSecret, searchTerm);
            Task.WaitAll(task);
        }

        private static async Task RunAsync(string oAuthConsumerKey, string oAuthConsumerSecret, string searchTerm)
        {
            if (!Directory.Exists(searchTerm))
            {
                Directory.CreateDirectory(searchTerm);
            }

            var knownUsersCount = Directory.GetDirectories(searchTerm).Length;
            if (knownUsersCount < UsersPerSearchTerm)
            {
                var client = new TwitterClient();
                await client.Authenticate(oAuthConsumerKey, oAuthConsumerSecret);

                var users = await client.FindUsers(searchTerm, UsersPerSearchTerm - knownUsersCount);
                foreach (var user in users)
                {
                    Directory.CreateDirectory(Path.Combine(searchTerm, user));
                }
            }
        }
    }
}