using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace JiraExample.Entities.Searching
{
    /// <summary>
    /// A class representing a JIRA REST search request
    /// </summary>
    public class SearchRequest
    {
        [JsonProperty("jql")]
        public string JQL { get; set; }

        [JsonProperty("startAt")]
        public int StartAt { get; set; }

        [JsonProperty("maxResults")]
        public int MaxResults { get; set; }

        [JsonProperty("fields")]
        public List<string> Fields { get; set; }

        

        public SearchRequest()
        {
            Fields = new List<string>();
        }
    }
}
