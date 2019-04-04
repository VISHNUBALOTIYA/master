using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using JiraExample.Entities.Issues;
using JiraExample.Entities.CommitInfo;

namespace JiraExample.Entities.Searching
{
    /// <summary>
    /// A class representing a JIRA REST search response
    /// </summary>
    public class SearchResponse
    {
        [JsonProperty("expand")]
        public string Expand { get; set; }

        [JsonProperty("startAt")]
        public int StartAt { get; set; }

        [JsonProperty("maxResults")]
        public int MaxResults { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("issues")]
        public List<Issue> IssueDescriptions { get; set; }

        [JsonProperty("detail")]
        public Detail[] Detail { get; set; }
    }


    public class Login
    {
        public string issueId { get; set; }
        public string applicationType { get; set; }
        public string dataType { get; set; }
    }
}

