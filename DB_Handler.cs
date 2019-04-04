using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using DefectDataManagment.Logger;

namespace DefectDataManagment.App_Code
{

    public static class mysqldata
    {
        public static Dictionary<int, List<string>> dict = new Dictionary<int, List<string>>();
        //changes password --vishnu
        public static string con_string = "Server=localhost;Database=defectsdata;Uid=root;password=root";
        //string connStr = "server=server;user=user;database=db;password=*****;";

        public static string Key, Summary, status, description, defecttype, SDCModule, ProModule, resolution, createdDate, ResolutionDate, TextAnalysisData;
        public static string IfMerge, methodname, codeElement, CHange_CodeElement, Defect_Category, CommitID, Message, Author;
        public static string Fileschanged, CommitedDate, FilesAdded, commitAuthor, commitMessage, steps;
        public static int IssueID;
      


        public static MySqlConnection connection = new MySqlConnection(DefectDataManagment.App_Code.mysqldata.con_string);

        public static void Initialize_Stat_Vars()
        {
            Key = Summary = status = description = defecttype = SDCModule = ProModule = resolution = createdDate = ResolutionDate = CommitID = Fileschanged = commitAuthor = commitMessage = IfMerge = TextAnalysisData = string.Empty;
            IssueID = 0;
            CHange_CodeElement = CommitedDate =steps = Fileschanged = Message = Author = Defect_Category = methodname = codeElement = CommitID = string.Empty;

        }

        public static string convert_date_to_mysql_format(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static string convert_date_to_indian_time(DateTime date)
        {
            return date.ToString("dd-MM-yyyy");
        }


        public static string Insert_to_DefectsDB()
        {
            bool datauploaded = true;
            string F_Status = string.Empty;
            MySqlCommand cmd = new MySqlCommand();

            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                    connection.Open();
                cmd.CommandText = "INSERT INTO jira_defect_info (DefectKey,Summary,State,Steps,DefectType,SDCModule,ProModule,Resolution,CreatedDate,ResolutionDate,IssueID,TextAnalysisData) VALUES (@DK,@SU,@ST,@STEP,@DT,@SDMod,@PRMod,@RES,@CRDAT,@RESDAT,@ISSID,@TAD)";
                cmd.Connection = DefectDataManagment.App_Code.mysqldata.connection;
                cmd.Parameters.AddWithValue("@DK", DefectDataManagment.App_Code.mysqldata.Key);
                cmd.Parameters.AddWithValue("@SU", DefectDataManagment.App_Code.mysqldata.Summary);
                cmd.Parameters.AddWithValue("@ST", DefectDataManagment.App_Code.mysqldata.status);
                cmd.Parameters.AddWithValue("@STEP", DefectDataManagment.App_Code.mysqldata.description);
                cmd.Parameters.AddWithValue("@DT", DefectDataManagment.App_Code.mysqldata.defecttype);
                cmd.Parameters.AddWithValue("@SDMod", DefectDataManagment.App_Code.mysqldata.SDCModule);
                cmd.Parameters.AddWithValue("@PRMod", DefectDataManagment.App_Code.mysqldata.ProModule);
                cmd.Parameters.AddWithValue("@RES", DefectDataManagment.App_Code.mysqldata.resolution);
                cmd.Parameters.AddWithValue("@ISSID", DefectDataManagment.App_Code.mysqldata.IssueID);
                cmd.Parameters.AddWithValue("@CRDAT", DefectDataManagment.App_Code.mysqldata.createdDate);
                cmd.Parameters.AddWithValue("@RESDAT", DefectDataManagment.App_Code.mysqldata.ResolutionDate);
                cmd.Parameters.AddWithValue("@TAD", DefectDataManagment.App_Code.mysqldata.TextAnalysisData);

                //connection.Open();
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                datauploaded = false;
                F_Status = ex.Message.ToString();

            }
            finally
            {
                //if (connection.State == System.Data.ConnectionState.Open)
                //  connection.Close();

                if (datauploaded)
                    F_Status = "Sucess";
                else
                    F_Status = "Failed with Error:" + F_Status.ToString();

            }
            return F_Status;
        }


        public static string Insert_to_CommitInfoDB()
        {
            bool datauploaded = true;
           string F_Status = string.Empty;
            MySqlCommand cmd = new MySqlCommand();

            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                    connection.Open();
                cmd.CommandText = "INSERT INTO commitsinfo (IssueID,CommitID,Author,Message,FilesChanged,IfMerge,CommitedDate,CHange_CodeElement) VALUES (@ISSID,@CID,@CAuthor,@CMsg,@FChange,@IfM,@CD,@CodeElement)";
                cmd.Connection = DefectDataManagment.App_Code.mysqldata.connection;
                cmd.Parameters.AddWithValue("@ISSID", DefectDataManagment.App_Code.mysqldata.IssueID);
                cmd.Parameters.AddWithValue("@CID", DefectDataManagment.App_Code.mysqldata.CommitID);
                cmd.Parameters.AddWithValue("@CAuthor", DefectDataManagment.App_Code.mysqldata.commitAuthor);
                cmd.Parameters.AddWithValue("@CMsg", DefectDataManagment.App_Code.mysqldata.commitMessage);
                cmd.Parameters.AddWithValue("@FChange", DefectDataManagment.App_Code.mysqldata.Fileschanged);
                cmd.Parameters.AddWithValue("@IfM", DefectDataManagment.App_Code.mysqldata.IfMerge);
                cmd.Parameters.AddWithValue("@CD", DefectDataManagment.App_Code.mysqldata.CommitedDate);
                cmd.Parameters.AddWithValue("@CodeElement", DefectDataManagment.App_Code.mysqldata.CHange_CodeElement);
                // connection.Open();
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                datauploaded = false;
                F_Status = ex.Message.ToString();

            }
            finally
            {
                if (datauploaded)
                    F_Status = "Sucess";
                else
                    F_Status = "Failed with Error:" + F_Status.ToString();

            }
            return F_Status;
        }



        public static string Insert_to_DefectsDB_temp()
        {
            bool datauploaded = true;
            string F_Status = string.Empty;
            MySqlCommand cmd = new MySqlCommand();

            try
            {
                cmd.CommandText = "INSERT INTO temp (Runtype,DefectKey,Summary,State,Steps,DefectType,SDCModule,ProModule,Resolution,CreatedDate,ResolutionDate,IssueID,TextAnalysisData) VALUES (@RT,@DK,@SU,@ST,@STEP,@DT,@SDMod,@PRMod,@RES,@CRDAT,@RESDAT,@ISSID,@TAD)";
                cmd.Connection = DefectDataManagment.App_Code.mysqldata.connection;
                cmd.Parameters.AddWithValue("@RT", Program.OneTimeRuntype);
                cmd.Parameters.AddWithValue("@DK", DefectDataManagment.App_Code.mysqldata.Key);
                cmd.Parameters.AddWithValue("@SU", DefectDataManagment.App_Code.mysqldata.Summary);
                cmd.Parameters.AddWithValue("@ST", DefectDataManagment.App_Code.mysqldata.status);
                cmd.Parameters.AddWithValue("@STEP", DefectDataManagment.App_Code.mysqldata.description);
                cmd.Parameters.AddWithValue("@DT", DefectDataManagment.App_Code.mysqldata.defecttype);
                cmd.Parameters.AddWithValue("@SDMod", DefectDataManagment.App_Code.mysqldata.SDCModule);
                cmd.Parameters.AddWithValue("@PRMod", DefectDataManagment.App_Code.mysqldata.ProModule);
                cmd.Parameters.AddWithValue("@RES", DefectDataManagment.App_Code.mysqldata.resolution);
                cmd.Parameters.AddWithValue("@ISSID", DefectDataManagment.App_Code.mysqldata.IssueID);
                cmd.Parameters.AddWithValue("@CRDAT", DefectDataManagment.App_Code.mysqldata.createdDate);
                cmd.Parameters.AddWithValue("@RESDAT", DefectDataManagment.App_Code.mysqldata.ResolutionDate);
                cmd.Parameters.AddWithValue("@TAD", DefectDataManagment.App_Code.mysqldata.TextAnalysisData);

                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                datauploaded = false;
                F_Status = ex.Message.ToString();

            }
            finally
            {
                //if (connection.State == System.Data.ConnectionState.Open)
                //  connection.Close();

                if (datauploaded)
                    F_Status = "Sucess";
                else
                    F_Status = "Failed with Error:" + F_Status.ToString();

            }
            return F_Status;
        }

  
        public static string Insert_to_CommitInfoDB_to_temp()
        {
            bool datauploaded = true;
            string F_Status = string.Empty;
            MySqlCommand cmd = new MySqlCommand();

            try
            {
                cmd.CommandText = "SET SQL_SAFE_UPDATES = 0";
                cmd.Connection = DefectDataManagment.App_Code.mysqldata.connection;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "Update temp SET FilesChanged='" + mysqldata.Fileschanged + "',CommitID='" + mysqldata.CommitID + "', Author='" + mysqldata.commitAuthor + "', Message='" + mysqldata.commitMessage + "', IfMerge='" + mysqldata.IfMerge + "' where IssueID =" + mysqldata.IssueID;
                cmd.Connection = DefectDataManagment.App_Code.mysqldata.connection;
                cmd.Parameters.AddWithValue("@CID", DefectDataManagment.App_Code.mysqldata.CommitID);
                cmd.Parameters.AddWithValue("@CAuthor", DefectDataManagment.App_Code.mysqldata.commitAuthor);
                cmd.Parameters.AddWithValue("@CMsg", DefectDataManagment.App_Code.mysqldata.commitMessage);
                cmd.Parameters.AddWithValue("@FChange", DefectDataManagment.App_Code.mysqldata.Fileschanged);
                cmd.Parameters.AddWithValue("@IfM", DefectDataManagment.App_Code.mysqldata.IfMerge);

                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                datauploaded = false;
                F_Status = ex.Message.ToString();

            }
            finally
            {
                // if (connection.State == System.Data.ConnectionState.Open)
                //   connection.Close();

                if (datauploaded)
                    F_Status = "Sucess";
                else
                    F_Status = "Failed with Error:" + F_Status.ToString();

            }
            return F_Status;
        }

    
        public static bool InsertCreatedDateCommitinfo(string  Cid, string createddate)
        {

            bool datauploaded = true;
            string fStatus = string.Empty;
            MySqlCommand cmd1 = new MySqlCommand();

            try
            {

                if (connection.State == System.Data.ConnectionState.Closed)
                    connection.Open();
                cmd1 = new MySqlCommand();
                cmd1.CommandText = "use defectsdata; update commitsinfo set CommitedDate = '"+createddate+"' WHERE commitsinfo.CommitID = "+"'"+Cid+"'";
                cmd1.Connection = DefectDataManagment.App_Code.mysqldata.connection;
                cmd1.ExecuteNonQuery();
                cmd1.Dispose();
                
            }
            catch (Exception ex)
            {
                datauploaded = false;
                fStatus = ex.Message.ToString();

            }
            finally
            {

                if (datauploaded)
                    fStatus = "Sucess";
                else
                    fStatus = "Failed with Error:" + fStatus.ToString();

            }
            return datauploaded;

        }

        public static bool InsertCodeElements(Dictionary<string, string> data, string colname)
        {

            bool datauploaded = true;
            string fStatus = string.Empty;
            MySqlCommand cmd1 = new MySqlCommand();

            try
            {
                foreach (var dict in data)
                {
                    cmd1 = new MySqlCommand();
                    CodeElementsParams.CommitId = dict.Key;
                    string textdata = dict.Value;
                    cmd1.CommandText = "SET SQL_SAFE_UPDATES = 0";
                    cmd1.Connection = DefectDataManagment.App_Code.mysqldata.connection;
                    cmd1.ExecuteNonQuery();

                    // defectsdata.commitsinfo
                    cmd1.CommandText = "UPDATE defectsdata.commitsinfo SET " + colname + "='" + textdata +
                                  "'where commitid='" + CodeElementsParams.CommitId + "'";


                    //cmd1.CommandText = "UPDATE " + tablename + " SET " + colname + "='" + textdata +
                    //                   "'where commitid='" + CodeElementsParams.CommitId + "'";

                    cmd1.Connection = DefectDataManagment.App_Code.mysqldata.connection;
                    cmd1.Parameters.AddWithValue("@" + colname, textdata);
                    cmd1.ExecuteNonQuery();
                    cmd1.Dispose();
                }
            }
            catch (Exception ex)
            {
                datauploaded = false;
                fStatus = ex.Message.ToString();

            }
            finally
            {

                if (datauploaded)
                    fStatus = "Sucess";
                else
                    fStatus = "Failed with Error:" + fStatus.ToString();

            }
            return datauploaded;

        }

        public static bool checktempTableExist()
        {
            bool Is_exist = false;
            string fStatus = string.Empty;
            string SQL_COMMAND = "SELECT count(*) FROM information_schema.TABLES WHERE(TABLE_SCHEMA = 'defectsdata') AND(TABLE_NAME = 'temp')";

            MySqlConnection connection = new MySqlConnection(mysqldata.con_string);
            MySqlCommand cmd; MySqlDataReader sdr = null;
            cmd = new MySqlCommand();

            try
            {
                cmd = connection.CreateCommand();
                if (connection.State != System.Data.ConnectionState.Open)
                    connection.Open();
                cmd.CommandText = SQL_COMMAND;
                sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    if (sdr["count(*)"].ToString() == "1")
                    {
                        Is_exist = true;
                    }
                    else
                    {
                        Is_exist = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Is_exist = false;
                fStatus = ex.Message.ToString();

            }
            finally
            {
                if (Is_exist)
                {
                    LogEntry.LogMsg.Info("Temp Table Exist " + fStatus);
                }
                else
                    LogEntry.LogMsg.Info("Temp Table Exist " + fStatus);
            }
            return Is_exist;

        }


        public static bool CreateTable()
        {
            bool IsTableCreated = false;
            string fStatus = string.Empty;
            try
            {
                string query = "CREATE TABLE temp(Runtype varchar(15),DefectKey varchar(300), Summary mediumtext, State varchar(15),DefectType varchar(15), SDCModule varchar(100), ProModule varchar(100),Resolution varchar(30), CreatedDate varchar(100), ResolutionDate varchar(50),IssueID int(11), Steps longtext, TextAnalysisData longtext)";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                IsTableCreated = true;
            }
            catch (Exception ex)
            {
                IsTableCreated = false;
                fStatus = ex.Message.ToString();

            }
            finally
            {
                if (IsTableCreated)
                {
                    LogEntry.LogMsg.Info(fStatus);
                }
                else
                    LogEntry.LogMsg.Info(fStatus);
            }
            return IsTableCreated;


        }


        /// <summary>
        /// This Method used to Drop Temp Table
        /// </summary>
        public static bool DropTable()
        {
            bool Isdropped = false;
            string fStatus = string.Empty;
            try
            {
                var query = "DROP TABLE temp";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                Isdropped = true;

            }
            catch (Exception ex)
            {
                Isdropped = false;
                fStatus = ex.Message.ToString();

            }
            finally
            {
                if (Isdropped)
                {
                    LogEntry.LogMsg.Info("Temp Table deleted " + fStatus);
                }
                else
                    LogEntry.LogMsg.Info(fStatus);
            }
            return Isdropped;


        }

        /// <summary>
        /// This Method used to Check Cmmit-ID in commmit ID table in Order to reduce duplicate
        /// </summary>
        public static bool checkCommitIDRecords(string commitid)
        {
            
            bool exist = false;
            string fStatus = string.Empty;
            string SQL_COMMAND = "SELECT * FROM defectsdata.commitsinfo where commitID="+"'"+commitid+"'";
           // MySqlConnection connection = new MySqlConnection(mysqldata.con_string);
            MySqlCommand cmd; MySqlDataReader sdr = null;
            
            try
            {
                cmd = new MySqlCommand();
                cmd = connection.CreateCommand();
                if(connection.State!= System.Data.ConnectionState.Open)
                connection.Open();
                cmd.CommandText = SQL_COMMAND;
                sdr = cmd.ExecuteReader();
                if (sdr.Read())
                {
                    if (sdr["CommitID"].ToString() != null)
                    {
                        exist = true;
                    }
                }
            }
            catch (Exception ex)
            {
                exist = false;
                fStatus = ex.Message.ToString();

            }
            finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                  connection.Close();

                if (exist)
                {
                    LogEntry.LogMsg.Info("Check Cmmit - ID in commmitInfo" + exist);
                }
                else
                    LogEntry.LogMsg.Info(fStatus);
            }
            return exist;

        }

        /// <summary>
        /// This Method used to update/Copy-Paste all record from "temp table" to main database tables(JIRa defects and Commits Defect Tablses) on daily based Run.
        /// </summary>
        public static bool UpdateOneTimeExecutionRecord()
        {
            bool Isupdated = true;
            string fStatus = string.Empty;
            List<string> recordruntype = new List<string>();
            recordruntype.Add("created");
            recordruntype.Add("updated");

            try
            {
                foreach (string Runtype in recordruntype)
                {
                    string deleteRecord = "use defectsdata; SET SQL_SAFE_UPDATES = 0; Delete jira_defect_info FROM jira_defect_info LEFT join temp On jira_defect_info.DefectKey = temp.DefectKey where temp.Runtype =" + "'" + Runtype + "'";
                    bool Iscreatedrc = ExecuteQuery(deleteRecord);
                    string insertRecord = "use defectsdata; INSERT INTO jira_defect_info(DefectKey, Summary, State, SDCModule, ProModule, Resolution, CreatedDate, ResolutionDate, IssueID, Steps, TextAnalysisData) SELECT DefectKey, Summary, State, SDCModule, ProModule, Resolution, CreatedDate, ResolutionDate, IssueID, Steps, TextAnalysisData FROM temp where Runtype =" + "'" + Runtype + "'";
                    bool Isupdatedrc = ExecuteQuery(insertRecord);

                    if (!(Iscreatedrc && Isupdatedrc))
                    {
                        Isupdated = false;
                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                Isupdated = false;
                fStatus = ex.Message.ToString();

            }
            finally
            {
                if (Isupdated)
                {
                    LogEntry.LogMsg.Info("update/Copy-Paste all record from temp table to main database tables" + fStatus);
                }
                else
                    LogEntry.LogMsg.Info(fStatus);
            }
            return Isupdated;
        }

        /// <summary>
        /// This is common Method used to Execute Query
        /// As a argument have to pass Query string
        /// </summary>
        public static bool ExecuteQuery(string querystring)
        {
            bool datauploaded = true;
            string fStatus = string.Empty;
            MySqlCommand cmd1 = new MySqlCommand();

            try
            {
                cmd1.CommandText = querystring;
                cmd1.Connection = DefectDataManagment.App_Code.mysqldata.connection;
                cmd1.ExecuteNonQuery();
                cmd1.Dispose();
                datauploaded = true;

            }

            catch (Exception ex)
            {
                datauploaded = false;
                fStatus = ex.Message.ToString();

            }
            finally
            {
                if (datauploaded)
                {
                    LogEntry.LogMsg.Info("Query" + querystring + "Executed "+ fStatus);
                }
                else
                    LogEntry.LogMsg.Info(fStatus);
            }
            return datauploaded;

        }

        
      public static List<string> Fetch_Item_Name(string columnname)
      {
          string Item_name = string.Empty;
          bool datauploaded = true;
          string fStatus = string.Empty;
          List<string> commitslist = new List<string>();
          MySqlConnection connection = new MySqlConnection(mysqldata.con_string);
          MySqlCommand cmd;

          try
          {
              if (connection.State == System.Data.ConnectionState.Closed)
                    connection.Open();
                 cmd = new MySqlCommand("select CommitID from defectsdata.commitsinfo", connection);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //DefectKey,Summary,State,Steps,DefectType,SDCModule,ProModule,Resolution,CreatedDate,ResolutionDate,IssueID,TextAnalysisData
                        Item_name = reader[columnname].ToString();
                        commitslist.Add(Item_name);
                    }
                }

             
          }
            catch (Exception ex)
            {
                datauploaded = false;
                fStatus = ex.Message.ToString();

            }
            finally
            {

               
                if (datauploaded)
                {
                    LogEntry.LogMsg.Info("Query Executed " + fStatus);
                }
                else
                    LogEntry.LogMsg.Info(fStatus);
            }
            return commitslist;
      }
      

        /*
       public static void GetTempDBData()
       {
           try
           {

               MySqlCommand command = new MySqlCommand("SELECT * FROM defectsdata.temp", connection);
               using (MySqlDataReader reader = command.ExecuteReader())
               {
                   while (reader.Read())
                   {
                       //DefectKey,Summary,State,Steps,DefectType,SDCModule,ProModule,Resolution,CreatedDate,ResolutionDate,IssueID,TextAnalysisData

                       //JIRA DB
                       mysqldata.IssueID = Convert.ToInt32((reader["IssueID"].ToString()));
                       mysqldata.defecttype = reader["DefectType"].ToString();
                       mysqldata.Summary = reader["DefectKey"].ToString();
                       mysqldata.Summary = reader["Summary"].ToString();
                       mysqldata.status = reader["status"].ToString();
                       mysqldata.SDCModule = reader["SDCModule"].ToString();
                       mysqldata.ProModule = reader["ProModule"].ToString();
                       mysqldata.resolution = reader["resolution"].ToString();
                       mysqldata.createdDate = reader["createdDate"].ToString();
                       mysqldata.steps = reader["steps"].ToString();
                       mysqldata.TextAnalysisData = reader["TextAnalysisData"].ToString();
                       //COMMIT DB
                       mysqldata.CommitID = reader["CommitID"].ToString();
                       mysqldata.Fileschanged = reader["Fileschanged"].ToString();
                       mysqldata.IfMerge = reader["IfMerge"].ToString();
                       mysqldata.Author = reader["Author"].ToString();
                       mysqldata.Message = reader["Message"].ToString();
                       mysqldata.CHange_CodeElement = reader["CHange_CodeElement"].ToString();
                       mysqldata.Defect_Category = reader["Defect_Category"].ToString();

                       Delete_jira_defect_update();

                   }
               }
           }
           catch (Exception ex)
           {
               // handle exception here
           }
           finally
           {
               connection.Close();
           }
       }
       */

        /*
    public static void Delete_jira_defect_update()
    {
        bool datauploaded = true;
        string fStatus = string.Empty;
        MySqlCommand cmd1 = new MySqlCommand();

        try
        {
            cmd1.CommandText = "SET SQL_SAFE_UPDATES = 0";
            cmd1.Connection = DefectDataManagment.App_Code.mysqldata.connection;
            cmd1.ExecuteNonQuery();

            //Delete row from defectsdata.jira_defect_info
            cmd1.CommandText = "Delete from defectsdata.jira_defect_info where IssueID = " + mysqldata.IssueID;
            cmd1.Connection = DefectDataManagment.App_Code.mysqldata.connection;
            cmd1.ExecuteNonQuery();
            cmd1.Dispose();

            Insert_to_DefectsDB();

            Fetch_Item_Name(mysqldata.IssueID.ToString(), mysqldata.CommitID);
        }

        catch (Exception ex)
        {
            datauploaded = false;
            fStatus = ex.Message.ToString();

        }
        finally
        {

            if (datauploaded)
                fStatus = "Sucess";
            else
                fStatus = "Failed with Error:" + fStatus.ToString();

        }
        // return datauploaded;
    }
    */

    }

}
 