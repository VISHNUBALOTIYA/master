using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefectDataManagment.App_Code
{
    interface Iparse
    {
        string Createsyntaxtree(string newcsfile, string oldCsfile, FilesDetails filedetails);
    }
}
