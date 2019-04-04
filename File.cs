using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JiraExample.Entities.CommitInfo
{
   public class File
    {
        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("changeType")]
        public string ChangeType { get; set; }


    }
}
