using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefectAnalysis_POC
{
   public class DBSchema_Defects
   {

        public string key { get; set; }

        public string Summary { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string DefectType { get; set; }
        public string SDCModule { get; set; }
        public string ProModule { get; set; }
        public string Resolution { get; set; }
        public string CreatedDate { get; set; }

        public string ResolutionDate { get; set; }

    }
}
