using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JiraExample.Entities.CommitInfo
{
    public class Repository
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("avatarDescription")]
        public string AvatarDescription { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("commits")]
        public Commit[] Commits { get; set; }

    }
}
