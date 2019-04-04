using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JiraExample.Entities.CommitInfo
{
   public class Detail
    {
        [JsonProperty("repositories")]
        public Repository[] Repositories { get; set; }
    }
}
