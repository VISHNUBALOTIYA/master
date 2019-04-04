using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

using JiraExample.Entities.Issues;

namespace JiraExample.Entities.Issues
{
    public class DefectTypeCategory : BaseEntity
    {


        [JsonProperty("value")]
        public string Value { get; set; }


        [JsonProperty("id")]
        public string ID { get; set; }

    }

   
}
