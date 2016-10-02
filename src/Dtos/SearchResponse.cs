using System.Collections.Generic;

namespace TwitAnalyser.Dtos
{
    public class SearchResponse
    {
        public List<Status> statuses { get; set; }
        public SearchMetadata search_metadata { get; set; }

        public class Status
        {
            public string created_at { get; set; }
            public string text { get; set; }
            public bool truncated { get; set; }
            public User user { get; set; }
            public bool is_quote_status { get; set; }
            public bool possibly_sensitive { get; set; }
        }

        public class User
        {
            public object id { get; set; }
            public string id_str { get; set; }
            public string name { get; set; }
            public string screen_name { get; set; }
            public string location { get; set; }
            public string description { get; set; }
            public string url { get; set; }
            public bool @protected { get; set; }
            public int followers_count { get; set; }
            public int friends_count { get; set; }
            public int listed_count { get; set; }
            public string created_at { get; set; }
            public int favourites_count { get; set; }
            public int? utc_offset { get; set; }
            public string time_zone { get; set; }
            public bool geo_enabled { get; set; }
            public bool verified { get; set; }
            public int statuses_count { get; set; }
            public string lang { get; set; }
            public bool contributors_enabled { get; set; }
            public bool is_translator { get; set; }
            public bool is_translation_enabled { get; set; }
            public string profile_background_color { get; set; }
            public string profile_background_image_url { get; set; }
            public string profile_background_image_url_https { get; set; }
            public bool profile_background_tile { get; set; }
            public string profile_image_url { get; set; }
            public string profile_image_url_https { get; set; }
            public string profile_banner_url { get; set; }
            public string profile_link_color { get; set; }
            public string profile_sidebar_border_color { get; set; }
            public string profile_sidebar_fill_color { get; set; }
            public string profile_text_color { get; set; }
            public bool profile_use_background_image { get; set; }
            public bool has_extended_profile { get; set; }
            public bool default_profile { get; set; }
            public bool default_profile_image { get; set; }
        }

        public class SearchMetadata
        {
            public double completed_in { get; set; }
            public long max_id { get; set; }
            public string max_id_str { get; set; }
            public string next_results { get; set; }
            public string query { get; set; }
            public string refresh_url { get; set; }
            public int count { get; set; }
            public int since_id { get; set; }
            public string since_id_str { get; set; }
        }

    }

}