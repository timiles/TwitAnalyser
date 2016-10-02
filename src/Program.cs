namespace TwitAnalyser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var oAuthConsumerKey = args[0];
            var oAuthConsumerSecret = args[1];

            var client = new TwitterClient();
            client.Authenticate(oAuthConsumerKey, oAuthConsumerSecret);
        }
    }
}