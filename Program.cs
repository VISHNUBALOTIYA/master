using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using JiraExample.Entities.Issues;
using JiraExample.Entities.CommitInfo;
using DefectDataManagment.App_Code;
using DefectDataManagment.CredentialManager;
using DefectDataManagment.Logger;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DefectDataManagment
{
    
    public class Program
    {
           
        public static Dictionary<string, string> methodCodeElements = new Dictionary<string, string>();
        public static Dictionary<string, string> defectcategory = new Dictionary<string, string>();
        public static int Inputcounter=0;
        public static bool DailyUpdate;
        public static string username;
        public static string  password;
        public static string OneTimeRuntype;
        public static bool UpdateRecordAndDropTable;


        public static void Main(string[] args)
        {
            try
            {

                // System.Diagnostics.Debugger.Launch();
                string mainbranch = ConfigurationManager.AppSettings["mainbranch"];
                string projectKey = ConfigurationManager.AppSettings["projectkey"];
                string issueType = ConfigurationManager.AppSettings["issueType"];
                string cloneurlPath = ConfigurationManager.AppSettings["gitRepoPath"];
                bool Isrepocloned = false;
                bool ISCommitid_Exist;
                string SearchAPIjql;


                DailyUpdate = false; // Convert.ToBoolean(args[0]);  // false;
                SearchAPIjql = "project = SDC4 AND issuetype = Bug";// args[1];
                username = "vishnu.balotiya"; //args[2];
                password = "KA12**ka";  // args[3];


                if (DailyUpdate)
                {
                    OneTimeRuntype = args[4];
                    UpdateRecordAndDropTable = Convert.ToBoolean(args[5]);

                    if (!mysqldata.checktempTableExist())
                    {
                        if (DefectDataManagment.App_Code.mysqldata.connection.State == System.Data.ConnectionState.Closed)
                            DefectDataManagment.App_Code.mysqldata.connection.Open();
                        DefectDataManagment.App_Code.mysqldata.CreateTable();
                    }
                }
                else
                {
                    if (DefectDataManagment.App_Code.mysqldata.connection.State == System.Data.ConnectionState.Closed)
                        DefectDataManagment.App_Code.mysqldata.connection.Open();
                    string Deleterecordjira = ConfigurationManager.AppSettings["deleteJIRATable"];
                    string DeleterecordCommmit = ConfigurationManager.AppSettings["deleteCommitTable"];

                    //string deleteRecord = "use defectsdata; SET SQL_SAFE_UPDATES = 0; Delete jira_defect_info FROM jira_defect_info LEFT join temp On jira_defect_info.DefectKey = temp.DefectKey where temp.Runtype =" + "'" + Runtype + "'";
                    //bool Isdeletedjirarec = mysqldata.ExecuteQuery(Deleterecordjira);
                    //bool Isdeletedcommitrec = mysqldata.ExecuteQuery(DeleterecordCommmit);
                }


                Console.WriteLine(SearchAPIjql);
                UserPass Credentials;
                RepositoryCloaning codeElemt;
                RepoClone(mainbranch, cloneurlPath, out Isrepocloned, out Credentials, out codeElemt);

                JiraManager manager = new JiraManager(Credentials._username, Credentials._password);
                List<Issue> issueDescriptions = manager.GetIssues(SearchAPIjql);
                Detail[] commitDetals = null;
                if (DefectDataManagment.App_Code.mysqldata.connection.State == System.Data.ConnectionState.Closed)
                    DefectDataManagment.App_Code.mysqldata.connection.Open();
                string TempModule, TempModule2 = "";

                Console.WriteLine("Num of Issues=:" + issueDescriptions.Count);
                List<string> tempcsfiles = new List<string>();

                foreach (Issue result in issueDescriptions)
                {
                    
                    DefectDataManagment.App_Code.mysqldata.Initialize_Stat_Vars();
                    DefectDataManagment.App_Code.mysqldata.Key = result.Key != null ? result.Key.ToString() : "";
                    
                    DefectDataManagment.App_Code.mysqldata.Summary = result.Fields.Summary != null ? result.Fields.Summary.ToString() : "";
                    DefectDataManagment.App_Code.mysqldata.status = result.Fields.Status.Name != null ? result.Fields.Status.Name.ToString() : "";
                    DefectDataManagment.App_Code.mysqldata.description = result.Fields.Description != null ? result.Fields.Description.ToString() : "";
                    DefectDataManagment.App_Code.mysqldata.defecttype = result.Fields.DefectType != null && result.Fields.DefectType[0].Value != null ? result.Fields.DefectType[0].Value.ToString() : "";

                    //SDCModule
                    TempModule = result.Fields.SModule != null && result.Fields.SModule.Value != null ? result.Fields.SModule.Value : "";
                    TempModule2 = result.Fields.SModule != null && result.Fields.SModule.Child != null && result.Fields.SModule.Child.Value != null ? result.Fields.SModule.Child.Value : "";

                    if (TempModule == "" && TempModule2 == "")
                    {
                        DefectDataManagment.App_Code.mysqldata.SDCModule = "";
                    }

                    else if (TempModule == "" || TempModule2 == "")
                    {
                        if (TempModule == "")
                        { DefectDataManagment.App_Code.mysqldata.SDCModule = TempModule2; }

                        else
                        { DefectDataManagment.App_Code.mysqldata.SDCModule = TempModule; }

                    }
                    else
                    { DefectDataManagment.App_Code.mysqldata.SDCModule = TempModule + "_" + TempModule2; }


                    //ProductModule
                    TempModule = result.Fields.PModule != null && result.Fields.PModule.Value != null ? result.Fields.PModule.Value : "";
                    TempModule2 = result.Fields.PModule != null && result.Fields.PModule.Child != null
                        && result.Fields.PModule.Child.Value != null ? result.Fields.PModule.Child.Value : "";


                    if (TempModule == "" && TempModule2 == "")
                    {
                        DefectDataManagment.App_Code.mysqldata.ProModule = "";
                    }

                    else if (TempModule == "" || TempModule2 == "")
                    {
                        if (TempModule == "")
                        { DefectDataManagment.App_Code.mysqldata.ProModule = TempModule2; }

                        else
                        { DefectDataManagment.App_Code.mysqldata.ProModule = TempModule; }

                    }
                    else
                    { DefectDataManagment.App_Code.mysqldata.ProModule = TempModule + "_" + TempModule2; }

                    DefectDataManagment.App_Code.mysqldata.resolution = result.Fields.Resolution != null && result.Fields.Resolution.Name != null ? result.Fields.Resolution.Name : "";

                    string createdDate = result.Fields.createdDate != null ? result.Fields.createdDate : "";
                    if (!String.IsNullOrEmpty(createdDate))
                    { DefectDataManagment.App_Code.mysqldata.createdDate = createdDate.Split('T')[0]; }

                    string ResolutionDate = result.Fields.resolutionDate != null ? result.Fields.resolutionDate : "";
                    if (!String.IsNullOrEmpty(ResolutionDate))
                    { DefectDataManagment.App_Code.mysqldata.ResolutionDate = ResolutionDate.Split('T')[0]; }

                    DefectDataManagment.App_Code.mysqldata.IssueID = result.Id;

                    DefectDataManagment.App_Code.mysqldata.TextAnalysisData = DefectDataManagment.App_Code.mysqldata.Summary + " " + DefectDataManagment.App_Code.mysqldata.description;


                    LogEntry.LogMsg.Info("---------------------------------------------------------------------------");
                    if (DailyUpdate)
                    {
                       // LogEntry.LogMsg.Info(DefectDataManagment.App_Code.mysqldata.Insert_to_DefectsDB_temp());
                    }
                    else
                    {
                        //LogEntry.LogMsg.Info(DefectDataManagment.App_Code.mysqldata.Insert_to_DefectsDB());
                    }

                    commitDetals = manager.GetCommitInfo(DefectDataManagment.App_Code.mysqldata.IssueID);


                    if (commitDetals == null || commitDetals.ElementAt(0) == null || commitDetals.ElementAt(0).Repositories.Length == 0)
                    {
                        //commit id not present in the defect 
                    }
                    else
                    {
                        foreach (Detail det in commitDetals)
                        {
                            var repoCount = det.Repositories.Count();
                            for (int i = 0; i < repoCount; i++)
                            {
                                var commitList = det.Repositories[i].Commits;
                                string projname = det.Repositories[i].Avatar.Split('/').ToList()[4];
                                Logger.LogEntry.LogMsg.Info(projname);

                                bool checkproj = false;
                                foreach (Commit obj in commitList)
                                {
                                    if (projname != "SDC4")
                                    {
                                        // methodCodeElements.Add(obj.Id, projname + " project changes");
                                        DefectDataManagment.App_Code.mysqldata.CHange_CodeElement = "Changes in "+ projname;
                                        checkproj = true;
                                    }

                                    DefectDataManagment.App_Code.mysqldata.CommitID = obj.Id;
                                    DefectDataManagment.App_Code.mysqldata.commitAuthor = obj.Author.Name;
                                    DefectDataManagment.App_Code.mysqldata.commitMessage = obj.Message;
                                    DefectDataManagment.App_Code.mysqldata.IfMerge = obj.Merge.ToString();
                                    DefectDataManagment.App_Code.mysqldata.Fileschanged = "";

                                    //Get commit ID Date
                                    string command = "git show -s --format=%cd  --date=short" + " " + DefectDataManagment.App_Code.mysqldata.CommitID;
                                    var patchLines = RepositoryCloaning.ExecuteGitCommand(RepositoryCloaning.temppath, command).Split('\n');
                                    foreach (var str in patchLines)
                                    {
                                        DateTime dateValue;
                                        if (DateTime.TryParseExact(str, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
                                        {
                                            // mysqldata.InsertCreatedDateCommitinfo(DefectDataManagment.App_Code.mysqldata.CommitID, str);
                                            DefectDataManagment.App_Code.mysqldata.CommitedDate = str;
                                            break;
                                        }
                                    }


                                    tempcsfiles = new List<string>();
                                    string fileaddtag = "File Added\n";
                                    string fileModifiedtag = "File Modified\n";
                                    string fileDeletedtag = "File Deleted\n";
                                    foreach (JiraExample.Entities.CommitInfo.File filename in obj.Files)
                                    {
                                        if (filename.ChangeType.ToLower() == FileOperation.ADDED.ToString().ToLower())
                                        {
                                            DefectDataManagment.App_Code.mysqldata.Fileschanged += fileaddtag + filename.Path + ";\n";
                                            fileaddtag = string.Empty;
                                        }

                                        if (filename.ChangeType.ToLower() == FileOperation.DELETED.ToString().ToLower())
                                        {
                                            DefectDataManagment.App_Code.mysqldata.Fileschanged += fileDeletedtag + filename.Path + ";\n";
                                            fileDeletedtag = string.Empty;
                                        }

                                        if (filename.ChangeType.ToLower() == FileOperation.MODIFIED.ToString().ToLower())
                                        {
                                            DefectDataManagment.App_Code.mysqldata.Fileschanged += fileModifiedtag + filename.Path + ";\n";
                                            fileModifiedtag = string.Empty;
                                            var extractfile = filename.Path.Split('/').ToList()[0].ToString().ToLower();

                                            if (extractfile != "src" && extractfile != "common" && extractfile != "server" && extractfile != "api" && extractfile != "unittest")
                                            {
                                                if (filename.Path.EndsWith(".cs"))
                                                    tempcsfiles.Add(filename.Path);
                                            }

                                            //File methodCodeElements
                                            //String values:js/html/xaml/xml/cpp/h/vcxproj/sln/cmsg/filters/dat/bat/xsd
                                            //methodCodeElements.Add(mysqldata.CommitID,"")
                                        }

                                    }

                                    List<string> cSFiles = new List<string>(tempcsfiles);
                                    tempcsfiles = null;
                                    CodeElementsParams.CommitId = mysqldata.CommitID;

                                    if (cSFiles.Count() > 0)
                                    {
                                        RepositoryCloaning.GetSpan();
                                        var codeDataElement = codeElemt.CompareCommitedfiles(cSFiles);
                                        DefectDataManagment.App_Code.mysqldata.CHange_CodeElement = codeDataElement.ToString();
                                    }
                                    else
                                    {
                                       //if(!checkproj)
                                       //DefectDataManagment.App_Code.mysqldata.CHange_CodeElement = "Changes in Non CS files";
                                    }

                                    // CodeElmentsExecutionModification(codeElemt, cSFiles);
                                    //if (!methodCodeElements.ContainsKey(CodeElementsParams.CommitId) && codeDataElement != string.Empty)
                                    //{
                                    //methodCodeElements.Add(CodeElementsParams.CommitId, codeDataElement);
                                    // }
                                

                                if (DailyUpdate)
                                {
                                    //put condition because developer comminting at a time for all defects so defect ID will be same for all defect.
                                    ISCommitid_Exist = DefectDataManagment.App_Code.mysqldata.checkCommitIDRecords(mysqldata.CommitID.ToString());
                                    if (!ISCommitid_Exist)
                                    {
                                        if (String.IsNullOrEmpty(DefectDataManagment.App_Code.mysqldata.Fileschanged.ToString()))
                                        { }
                                        LogEntry.LogMsg.Info("---------------------------------------------------------------------------");
                                        LogEntry.LogMsg.Info(DefectDataManagment.App_Code.mysqldata.Insert_to_CommitInfoDB());
                                    }
                                }
                                else
                                {
                                    if (DefectDataManagment.App_Code.mysqldata.Fileschanged.Count() > 0)
                                    {
                                        LogEntry.LogMsg.Info("---------------------------------------------------------------------------");
                                        LogEntry.LogMsg.Info(DefectDataManagment.App_Code.mysqldata.Insert_to_CommitInfoDB());
                                    }

                                }
                            }



                            }
                        }

                    }
                }


                //if (methodCodeElements.Count > 0)
                 //   DefectDataManagment.App_Code.mysqldata.InsertCodeElements(methodCodeElements, "CHange_CodeElement");
                if (defectcategory.Count > 0)
                    DefectDataManagment.App_Code.mysqldata.InsertCodeElements(defectcategory, "Defect_Category");

                Console.Read();
            }

            catch (Exception ex)
            {
                LogEntry.LogMsg.Error(ex.Message);
            }
            finally
            {
                //Update all record in main DB and Delete temp File
                if (DailyUpdate)
                { 
                    if (UpdateRecordAndDropTable)
                    {
                        if (DefectDataManagment.App_Code.mysqldata.connection.State == System.Data.ConnectionState.Closed)
                            DefectDataManagment.App_Code.mysqldata.connection.Open();
                        bool updated = DefectDataManagment.App_Code.mysqldata.UpdateOneTimeExecutionRecord();
                        if (updated)
                        {
                            bool Droppedtable = DefectDataManagment.App_Code.mysqldata.DropTable();
                            LogEntry.LogMsg.Info("Temp Table Dropped: " + Droppedtable);
                        }
                    }
              }

                //Close the generic connection 
                if (DefectDataManagment.App_Code.mysqldata.connection.State == System.Data.ConnectionState.Open)
                    DefectDataManagment.App_Code.mysqldata.connection.Close();
            }
           
        }

        private static void RepoClone(string mainbranch, string cloneurlPath, out bool Isrepocloned, out UserPass Credentials, out RepositoryCloaning codeElemt)
        {
            CreateCredentials cred = new CreateCredentials();
            cred.CreateAndSaveCredntitials(cloneurlPath, username, password);
            Credentials = CredentialUtil.GetCredential(cloneurlPath);
            SyntaxTreeCodeParse suntaxObj = new SyntaxTreeCodeParse();


            codeElemt = new RepositoryCloaning(mainbranch, username, suntaxObj);
            Isrepocloned = codeElemt.CheckRepoExist();
            codeElemt.CloseGitprocess();
        }



        /*
        private static void CodeElementsExecutionModification(RepositoryCloaning codeElemt, IEnumerable<string> cSFiles)
        {
            try
            {
                var codeDataElement = codeElemt.CompareCommitedfiles(cSFiles);
                if (!methodCodeElements.ContainsKey(CodeElementsParams.CommitId) && codeDataElement != string.Empty)
                {
                    methodCodeElements.Add(CodeElementsParams.CommitId, codeDataElement);
                }

                // DefectedCodeDetails.StoreMaxDefectedClassAndMethod(codeDataElement);
                // if (!methodCodeElements.ContainsKey(CodeElements.CommitId) && codeDataElement != string.Empty)
                //var Addedfilelist = mysqldata.FilesAdded.Split(';').ToList();
                // var SPIProjectfiles = mysqldata.Fileschanged.Split(';').ToList()[0].Split('/').ToList()[0];
                //var xmlfiles = mysqldata.Fileschanged.Split(';').ToList().Where(q => q.EndsWith(".xml")).ToList();
                // var xaml = mysqldata.Fileschanged.Split(';').ToList().Where(q => q.EndsWith(".xaml")).ToList();
            }


            catch (Exception ex)
            {
                LogEntry.LogMsg.Error(ex.Message);
                throw ex;
            }
        }
        */
      
     
    }




}
