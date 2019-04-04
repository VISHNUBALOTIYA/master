using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace JiraExample.Entities.Issues
{
    /// <summary>
    /// Represents a Fields JSON object
    /// </summary>
    /// <remarks>
    /// "fields" : {
    ///	    "summary" : "Some summary",
    ///	    "status" : {
    ///	    	...
    ///	    },
    ///	    "assignee" : {
    ///	    	...
    ///	    }
    /// }    
    /// </remarks>
    public class Fields
    {
        [JsonProperty("summary")]
        public string Summary { get; set; }


        [JsonProperty("created")]
        public string createdDate { get; set; }

        [JsonProperty("resolutiondate")]
        public string resolutionDate { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("status")] 
        public Status Status { get; set; }

        [JsonProperty("assignee")]
        public Assignee Assignee { get; set; }

        [JsonProperty("customfield_10201")]
        public DefectTypeCategory[] DefectType { get; set; }

        [JsonProperty("customfield_10400")]
        public Product_Module PModule { get; set; }

        [JsonProperty("customfield_10903")]
        public SDC_Module SModule { get; set; }

        [JsonProperty("resolution")]
        public Resolution Resolution { get; set; }

       



    }
}


