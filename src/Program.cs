using System;
using System.Threading.Tasks;

namespace TwitAnalyser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var oAuthConsumerKey = args[0];
            var oAuthConsumerSecret = args[1];

            var mode = args[2];

            var task = RunAsync(oAuthConsumerKey, oAuthConsumerSecret, mode, args[3]);
            Task.WaitAll(task);
        }

        private static async Task RunAsync(string oAuthConsumerKey, string oAuthConsumerSecret, string mode, string arg)
        {
            var client = new TwitterClient();
            await client.Authenticate(oAuthConsumerKey, oAuthConsumerSecret);

            switch (mode)
            {
                case "findusers":
                    {
                        var users = await client.FindUsers(arg);
                        foreach (var user in users)
                        {
                            Console.WriteLine(user);
                        }
                        break;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException(mode);
                    }
            }
        }
    }
}