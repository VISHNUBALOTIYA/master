using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiraExample.Entities.Issues;
using JiraExample.Entities.Searching;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JiraExample.Entities.CommitInfo;
using System.Net.Cache;
using System.Net.Http;

namespace DefectDataManagment
{
    public class JiraManager
    {
        private const string m_BaseUrl = "http://endoscmeng01.endo.strykercorp.com:8080/rest/api/latest/";
        private const string m_BaseUrlDev_Status = "http://endoscmeng01.endo.strykercorp.com:8080/rest/dev-status/latest/issue/detail?issueId=";
        private string m_Username;
        private string m_Password;

        public JiraManager(string username, string password)
        {
            m_Username = username;
            m_Password = password;
        }

        /// <summary>
        /// Runs a query towards the JIRA REST api
        /// </summary>
        /// <param name="resource">The kind of resource to ask for</param>
        /// <param name="argument">Any argument that needs to be passed, such as a project key</param>
        /// <param name="data">More advanced data sent in POST requests</param>
        /// <param name="method">Either GET or POST</param>
        /// <param name="APIBase">Base or DevStatus</param>
        /// <returns></returns>
        protected string RunQuery( JiraResource resource, APIResource apiresource, string argument = null, string data = null, string method = "GET")
        {
            string url = string.Format("{0}{1}/", m_BaseUrl, resource.ToString());

            if (argument != null)
            {
                url = string.Format("{0}{1}/", url, argument);
            }

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            HttpRequestCachePolicy noCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.CachePolicy = noCachePolicy;
            request.ContentType = "application/json";
            request.Method = method;

            if (data != null)
            {
                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(data);
                }
            }

            string base64Credentials = GetEncodedCredentials();
            request.Headers.Add("Authorization", "Basic " + base64Credentials);
            
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            string result = string.Empty;
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        protected string RunQuerywithParam(int IssueID, APIResource apiresource,string method = "GET")
        {
            string url ;
            HttpWebRequest request = null ;
            HttpWebResponse response = null;
            string result = null;

            if (apiresource == APIResource.Devstatus_Detail)
            {

                url = m_BaseUrlDev_Status + IssueID + "&applicationType=stash&dataType=repository";
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ContentType = "application/json";
                request.Method = method;

            }

            if (request != null)
            {
                string base64Credentials = GetEncodedCredentials();
                request.Headers.Add("Authorization", "Basic " + base64Credentials);

                response = request.GetResponse() as HttpWebResponse;
                result= string.Empty;
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }

        public List<Issue> GetIssues(string jql,List<string> fields = null, int startAt = 0, int maxResult = 3000)
        {
            fields = fields ?? new List<string> { "summary", "status", "assignee", "description" , "customfield_10201" , "customfield_10903" , "customfield_10400" ,"resolution" , "created" , "resolutiondate" };
            
            SearchRequest request = new SearchRequest();
            request.Fields = fields;
            request.JQL = jql;
            request.MaxResults = maxResult;
            request.StartAt = startAt;
            string data = JsonConvert.SerializeObject(request);
            string result = RunQuery(JiraResource.search, APIResource.Searchapi, data: data, method: "POST");

            SearchResponse response = JsonConvert.DeserializeObject<SearchResponse>(result);

           return response.IssueDescriptions;
        }

        public Detail[] GetCommitInfo(int issueID)
        {
            
            string result = RunQuerywithParam(issueID,APIResource.Devstatus_Detail, method: "GET");

            SearchResponse response = JsonConvert.DeserializeObject<SearchResponse>(result);

            return response.Detail;
        }
        
        private string GetEncodedCredentials()
        {
            string mergedCredentials = string.Format("{0}:{1}", m_Username, m_Password);
            byte[] byteCredentials = UTF8Encoding.UTF8.GetBytes(mergedCredentials);
            return Convert.ToBase64String(byteCredentials);
        }
    }

}
