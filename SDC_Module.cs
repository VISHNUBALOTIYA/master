using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace JiraExample.Entities.Issues
{
    public class SDC_Module:BaseEntity
    {    
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("child", NullValueHandling = NullValueHandling.Ignore)]
        public SDC_Module Child { get; set; }
    }
}
