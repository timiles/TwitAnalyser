using System;

namespace TwitAnalyser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var oAuthConsumerKey = args[0];
            var oAuthConsumerSecret = args[1];

            var mode = args[2];

            var client = new TwitterClient();
            client.Authenticate(oAuthConsumerKey, oAuthConsumerSecret);

            switch (mode)
            {
                case "findusers":
                    {
                        var users = client.FindUsersAsync(args[3]).Result;
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