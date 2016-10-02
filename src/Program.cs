using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            var allStatuses = new List<string>();
            foreach (var userDir in knownUserDirsForSearchTerm)
            {
                var files = Directory.GetFiles(userDir);
                if (files.Length == 0)
                {
                    var screenName = Path.GetFileName(userDir);
                    var statuses = await client.GetUserTimeline(screenName);
                    if (statuses == null) continue;
                    foreach (var status in statuses)
                    {
                        File.WriteAllText(Path.Combine(userDir, status.id + ".txt"), status.text);
                    }
                    files = Directory.GetFiles(userDir);
                }
                foreach (var file in files)
                {
                    allStatuses.Add(File.ReadAllText(file));
                }
            }

            var statusAnalyser = new StatusAnalyser(allStatuses);
            statusAnalyser.RemoveContaining(searchTerm);
            var frequentWords = statusAnalyser.GetWordFrequencies();
            foreach (var word in frequentWords.OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"{word.Key} ({word.Value})");
            }
        }
    }
}