using System;
using System.IO;
using System.Threading.Tasks;

namespace TwitAnalyser
{
    public class Program
    {
        const int UsersPerSearchTerm = 1000;
        const string DataDirPath = "data";

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
            var searchTermDirPath = Path.Combine(DataDirPath, searchTerm);
            if (!Directory.Exists(searchTermDirPath))
            {
                Directory.CreateDirectory(searchTermDirPath);
            }

            var client = new TwitterClient();
            await client.Authenticate(oAuthConsumerKey, oAuthConsumerSecret);

            var knownUserDirsForSearchTerm = Directory.GetDirectories(searchTermDirPath);
            if (knownUserDirsForSearchTerm.Length < UsersPerSearchTerm)
            {
                var users = await client.FindUsers(searchTerm, UsersPerSearchTerm - knownUserDirsForSearchTerm.Length);
                foreach (var user in users)
                {
                    Directory.CreateDirectory(Path.Combine(searchTermDirPath, user));
                }

                knownUserDirsForSearchTerm = Directory.GetDirectories(searchTermDirPath);
            }

            foreach (var userDir in knownUserDirsForSearchTerm)
            {
                if (Directory.GetFiles(userDir).Length == 0)
                {
                    var screenName = Path.GetFileName(userDir);
                    var statuses = await client.GetUserTimeline(screenName);
                    if (statuses == null) continue;
                    foreach (var status in statuses)
                    {
                        File.WriteAllText(Path.Combine(userDir, status.id + ".txt"), status.text);
                    }
                }
            }
        }
    }
}