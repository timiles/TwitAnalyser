using System.Collections.Generic;

namespace TwitAnalyser
{
    public class UserTimelineReponse : List<Status>
    {
    }

    public class Status
    {
        public string created_at { get; set; }
        public object id { get; set; }
        public string id_str { get; set; }
        public string text { get; set; }
        public bool truncated { get; set; }
        public string source { get; set; }
        public long? in_reply_to_status_id { get; set; }
        public string in_reply_to_status_id_str { get; set; }
        public long? in_reply_to_user_id { get; set; }
        public string in_reply_to_user_id_str { get; set; }
        public string in_reply_to_screen_name { get; set; }
        public User user { get; set; }
        public object geo { get; set; }
        public object coordinates { get; set; }
        public object place { get; set; }
        public object contributors { get; set; }
        public bool is_quote_status { get; set; }
        public int retweet_count { get; set; }
        public int favorite_count { get; set; }
        public bool favorited { get; set; }
        public bool retweeted { get; set; }
        public string lang { get; set; }
        public long? quoted_status_id { get; set; }
        public string quoted_status_id_str { get; set; }
        public bool? possibly_sensitive { get; set; }
    }

    public class User
    {
        public long id { get; set; }
        public string id_str { get; set; }
    }
}