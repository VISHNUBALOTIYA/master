using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DefectDataManagment
{

    /// <summary>
    /// An enumeration representing the various resources that can
    /// be used in a JIRA REST request
    /// </summary>
    public enum JiraResource
    {
       search,
       Issue,
       detail

    }

    public enum APIResource
    {
        Searchapi,
        Devstatus_Detail

    }
}
