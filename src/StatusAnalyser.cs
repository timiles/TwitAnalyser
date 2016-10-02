using System.Collections.Generic;
using System.Linq;

namespace TwitAnalyser
{
    public class StatusAnalyser
    {
        private List<StatusInfo> _statuses;

        public StatusAnalyser(List<string> statuses)
        {
            this._statuses = statuses.Select(x => new StatusInfo(x)).ToList();
        }

        public void RemoveContaining(string word)
        {
            var loweredWord = word.ToLower();
            this._statuses = this._statuses.Where(x => !x.LoweredWords.Contains(loweredWord)).ToList();
        }

        public Dictionary<string, int> GetWordFrequencies()
        {
            return this._statuses.SelectMany(x => x.LoweredWords)
            .GroupBy(x => x)
            .ToDictionary(x => x.Key, x => x.Count());
        }

        class StatusInfo
        {
            private const string splitChars = " \r\n\",.?!|:-#";
            public StatusInfo(string status)
            {
                this.OriginalStatus = status;
                this.LoweredWords = status.ToLower().Split(splitChars.ToArray(), System.StringSplitOptions.RemoveEmptyEntries);
            }

            public string OriginalStatus { get; set; }
            public string[] LoweredWords { get; set; }
        }
    }
}