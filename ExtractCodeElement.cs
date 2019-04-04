using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;
using XAct;
using System.Configuration;
using DefectDataManagment.Logger;

namespace DefectDataManagment.App_Code
{

    public class RepositoryCloaning
    {
      
        private static Process _process;
        private static readonly object processLock = new object();
        public static string temppath = Path.GetTempPath();
        public readonly string _cloneurlPath;
        private readonly string _username;
        public static string _mainbranch;
        private static AutoResetEvent resetEvent = new AutoResetEvent(false);
        private static BackgroundWorker worker = new BackgroundWorker();
        public static bool cloned = false;
        public static string gitRepoPath = ConfigurationManager.AppSettings["gitRepoPath"];

        internal Iparse Codeparse { get; set; }

        public RepositoryCloaning(string mainbranch, string username, SyntaxTreeCodeParse syntax)
        {
            _username = username;
            _mainbranch = mainbranch;
            Codeparse = syntax;
        }


        public enum RepoCloned { Empty, Partial, Full };

        public bool CheckRepoExist()
        {
            try
            {
                cloned = CheckAndCloneRepo();
                if (cloned)
              {
                    SwitchTomasterbranch();
                    CloseGitprocess();
                    //startclone("git pull");
                    //resetEvent.WaitOne();
                    ExecuteGitCommand(temppath, "git pull"); 
                }
                CloseGitprocess();
                return cloned;
            }
            catch (Exception ex)
            {
                LogEntry.LogMsg.Error(ex.Message);
                return cloned;
            }

           
        }

        public bool CheckAndCloneRepo()
        {
            try
            {
                CloseGitprocess();
                RepoCloned repclone = CheckRepoCloned();

                switch (repclone)
                {
                    case RepoCloned.Empty:
                        startclone("Star cloaning...");
                        resetEvent.WaitOne();
                        break;
                    case RepoCloned.Partial:
                        LogEntry.LogMsg.Info("Delete partial repo");
                        CloseGitprocess();
                        if (DeletepartialCloneddirectories())
                        {
                            startclone("Start cloaning...");
                            resetEvent.WaitOne();
                        }
                        else { cloned = false; }
                        break;
                    case RepoCloned.Full:
                        cloned = true;
                        LogEntry.LogMsg.Info("Repository is alread cloned ");
                        break;
                }
                return cloned;

            }
            catch (Exception ex)
            {
                cloned = false;
                throw ex;
            }
            
        }

        public static void startclone(string message)
        {
            try
            {
                worker.DoWork += worker_DoWork;
                worker.RunWorkerCompleted += worker_RunWorkerCompleted;
               // worker.ProgressChanged += worker_ProgressChanged;
               // worker.WorkerReportsProgress = true;
               // worker.WorkerSupportsCancellation = true;
                LogEntry.LogMsg.Info("start background worker thread for "+ message);
                worker.RunWorkerAsync(message);
            }
            catch (Exception ex)
            {
                throw ex ;
            }
        }

        static void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //LogEntry.LogMsg.Info(e.ProgressPercentage.ToString());
        }

        static void worker_DoWork(object sender, DoWorkEventArgs e)
        {
           
            var command  = e.Argument.ToString();
            string gitcommand;
            LogEntry.LogMsg.Info("background worker started for cloning");
            if (!command.Contains("pull"))
                gitcommand = "git clone " + gitRepoPath;
            else
                gitcommand = command;
          
            
            try
            {
                
                Process process;
                process = new Process
                {
                    StartInfo =
                        {
                            FileName = "cmd.exe",
                            CreateNoWindow = false,
                            Verb = "runas",
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            Arguments = gitcommand,
                            UseShellExecute = false,
                            WorkingDirectory =temppath
                    }
                };
                LogEntry.LogMsg.Info("start");
                process.OutputDataReceived += p_OutputDataReceived;
                process.EnableRaisingEvents = false;

                process.Start();

                process.StandardInput.WriteLine(gitcommand);
                process.StandardInput.Flush();
                process.StandardInput.Close();

                process.BeginOutputReadLine();


                process.WaitForExit();

            }
            catch (Exception ex)
            {
                LogEntry.LogMsg.Error("Exception while cloaning: " + ex.Message);
                cloned = false;
                throw ex;
            }
            
            
        }

        static void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && e.Cancelled == false)
                {
                    cloned = true;
                    if (temppath.Split('\\').Last() != _mainbranch)
                    {
                        temppath = temppath + _mainbranch;
                    }

                    LogEntry.LogMsg.Info("Repoitory cloned at location :" + temppath);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                resetEvent.Set();
            }
        }
      
        public static void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string status = e.Data;
            LogEntry.LogMsg.Info(status);

        }

        public static bool DeletepartialCloneddirectories()
        {
            try
            {
                ExecuteGitCommand(RepositoryCloaning.temppath, "rmdir /q /s master");
                Thread.Sleep(2000);
                if (!Directory.Exists(Path.GetTempPath() + "\\" + "master"))
                { return true; }
                else
                { return false; }

            }
            catch (Exception ex)
            {
                throw new Exception("Repo Files not deleted from local repo " + ex.Message);
            }
        }

        public RepoCloned CheckRepoCloned()
        {
            try
            {
                var directories = Directory.GetDirectories(temppath).ToList();
                foreach (var dir in directories)
                {
                    if (dir.Split('\\').Last() == _mainbranch)
                    {
                        double RepoSize = 3156942080;   //Hardcodedc/Calculated based on current Repo size
                        var repoCopied = Path.GetTempPath() + _mainbranch;
                        double repocopiedsize = CalculateFolderSize(repoCopied);
                        var CopiedPercentage = Convert.ToInt32(repocopiedsize / RepoSize * 100);

                        if (CopiedPercentage > 99)
                        {
                            temppath = dir;
                            return RepoCloned.Full;
                        }
                        else
                        {
                            return RepoCloned.Partial;
                        }
                    }
                }
                return RepoCloned.Empty;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static float CalculateFolderSize(string folder)
        {
            float folderSize = 0.0f;
            try
            {
                //Checks if the path is valid or not
                if (!Directory.Exists(folder))
                    return folderSize;
                else
                {
                    try
                    {
                        foreach (string file in Directory.GetFiles(folder))
                        {
                            if (File.Exists(file))
                            {
                                FileInfo finfo = new FileInfo(file);
                                folderSize += finfo.Length;
                            }
                        }

                        foreach (string dir in Directory.GetDirectories(folder))
                            folderSize += CalculateFolderSize(dir);
                    }
                    catch (NotSupportedException e)
                    {
                        LogEntry.LogMsg.Error("Unable to calculate folder size: {0}" + e.Message);
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                LogEntry.LogMsg.Info("Unable to calculate folder size: {0}" + e.Message);
            }
            return folderSize;
        }

        public void CloseGitprocess()
        {
            var Remotegitprocess = Process.GetProcessesByName("git-remote-http");
            var gitprocess = Process.GetProcessesByName("git");

            foreach (var p1 in Remotegitprocess)
            {
                p1.Kill();
            }
            foreach (var p2 in gitprocess)
            {
                p2.Kill();
            }
        }

        public static void SwitchTomasterbranch()
        {
            try
            {
                var brnaches = ExecuteGitCommand(RepositoryCloaning.temppath, "git branch");
                var resplines = brnaches.Split('\n');
                var isbranchavailabel = (resplines.Contains("* master") || resplines.Contains("  master"));
                if (!(isbranchavailabel))
                {
                    //create a branch
                    var bnames = ExecuteGitCommand(RepositoryCloaning.temppath, "git branch master");
                    var brnacheslist = ExecuteGitCommand(RepositoryCloaning.temppath, "git branch");
                    var bnamelist = brnacheslist.Split('\n');
                    if (bnamelist.Contains("  master") || bnamelist.Contains("* master"))
                    {
                        LogEntry.LogMsg.Info("master branch created");
                    }
                    else
                    {
                        throw new Exception("master branch not created to run git command");
                    }
                }

                //switch to master branch
                var checkouts = ExecuteGitCommand(RepositoryCloaning.temppath, "git checkout master");

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static string ExecuteGitCommand(string directoryPath, string gitcommand)
        {
            try
            {
                lock (processLock)
                {
                    _process = new Process
                    {
                        StartInfo =
                        {
                            FileName = "cmd.exe",
                            CreateNoWindow = true,
                            Verb = "runas",
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            WorkingDirectory =directoryPath
                        }
                    };


                    _process.Start();
                    _process.StandardInput.WriteLine(gitcommand);
                    _process.StandardInput.Flush();
                    _process.StandardInput.Close();
                    StreamReader streamReader;
                    string output;

                    using (streamReader = _process.StandardOutput)
                    {
                        output = streamReader.ReadToEnd();
                    }

                    streamReader.Close();
                    _process.WaitForExit();

                    return output;
                }
            }

            catch (Exception ex)
            {

                throw new Exception("Error while running Git command " + ex.Message);
            }

        }

        public string CompareCommitedfiles(IEnumerable<string> files)
        {
            try
            {
                temppath = Path.GetTempPath();
                temppath = temppath + "master";

                List<string> NewFile = new List<string>();
                List<string> Olfile = new List<string>();
                List<FilesDetails> Csfilenamestruct = new List<FilesDetails>();
                FilesDetails f1;
                foreach (string csfilepath in files)
                {
                   
                    CodeElementsParams.Filename = csfilepath.Split('/').Last();
                    CodeElementsParams.Filepath = csfilepath.Remove(csfilepath.LastIndexOf('/'));

                    f1.Filename = CodeElementsParams.Filename;
                    f1.filePath = CodeElementsParams.Filepath;

                    string fileLoc = csfilepath.Replace('/', '\\');
                    string path = Path.Combine(temppath +"\\"+ fileLoc);

                    string csfilecomand = "git checkout" + " " + CodeElementsParams.CommitId + " " + csfilepath;
                    ExecuteGitCommand(temppath, csfilecomand);
                    string latestFile = File.ReadAllText(path);

                    string oldcsfilecomand = "git checkout" + " " + CodeElementsParams.CommitId + "~1" + " " + csfilepath;
                    ExecuteGitCommand(temppath, oldcsfilecomand);
                    string oldFile = File.ReadAllText(path);

                    if (latestFile != oldFile)
                    {
                        NewFile.Add(latestFile);
                        Olfile.Add(oldFile);
                        Csfilenamestruct.Add(f1);
                    }

                    //Get  latest version of csfile
                    string updatecommand = "git checkout" + " " + _mainbranch + " " + "--" + " " + csfilepath;
                    ExecuteGitCommand(temppath, updatecommand);
                }

                var len = NewFile.Count();
                List<string> codeElementValues = new List<string>();
                for (int i = 0; i < len; i++)
                {
                    FilesDetails Fdetails = Csfilenamestruct[i];
                    var val = Codeparse.Createsyntaxtree(NewFile[i], Olfile[i], Fdetails);
                    codeElementValues.Add(val);                  
                }

                codeElementValues = codeElementValues.Distinct().ToList();
                string codeelementcode = string.Join("", codeElementValues.ToArray());
                return codeelementcode;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }


        public static void GetSpan()
        {
            
            string difflog = " git show -U0 "+ CodeElementsParams.CommitId;
            var ss = RepositoryCloaning.ExecuteGitCommand(RepositoryCloaning.temppath+"master", difflog);
            // IDictionary<string, List<string>> collect = new Dictionary<string, List<string>>();
            FilesDetails.FileSpan = new Dictionary<string, List<string>>();
            string span = string.Empty;
            var ds = ss.Split(new string[] { "diff --git" }, StringSplitOptions.None);
            List<string> linestart;

            foreach (var blob in ds.Skip(1))
            {
                var bloblines = blob.Split('\n');
                var filename = bloblines[0].Split('/').Last();
                if (filename.Split('.')[1] == "cs")
                {
                    linestart = new List<string>();

                    var dsa = bloblines.Where(str => str.Contains("@@"));
                    foreach (var item in dsa)
                    {
                        span = item.Split('+')[1].Split(' ')[0];
                        if (span.Contains(','))
                        {
                            span = span.Split(',')[0];
                        }

                        linestart.Add(span);
                    }

                    if (FilesDetails.FileSpan.ContainsKey(filename))
                    {
                        FilesDetails.FileSpan[filename] = linestart;
                    }
                    else
                    {
                        FilesDetails.FileSpan.Add(filename, linestart);
                    }
                }

            }


        }

    }

}


