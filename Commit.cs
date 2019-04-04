using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JiraExample.Entities.CommitInfo
{
   public class Commit
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("displayId")]
        public string DisplayId { get; set; }

        [JsonProperty("authorTimestamp")]
        public string AuthorTimestamp { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("author")]
        public Author Author { get; set; }

        [JsonProperty("fileCount")]
        public long FileCount { get; set; }

        [JsonProperty("merge")]
        public bool Merge { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("files")]
        public File[] Files { get; set; }


    }
}
