using Microsoft.Practices.EnterpriseLibrary.Data;
using PSEBONLINE.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PSEBONLINE.Repository
{

    public class AgencyRepository : IAgencyRepository
    {
        private Database db;
        public AgencyRepository()
        {
            db = DatabaseFactory.CreateDatabase("myDBConnection");
        }

        public int ChangePassword(string UserId, string CurrentPassword, string NewPassword)
        {
            int result;

            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "AgencyChangePasswordSP";
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@OldPwd", CurrentPassword);
                cmd.Parameters.AddWithValue("@NewPwd", NewPassword);
                result = db.ExecuteNonQuery(cmd);
            }
            catch (Exception ex)
            {
                result = -1;
            }
            return result;

        }

        public Task<AgencyLoginSession> CheckAgencyLogin(LoginModel LM)  // Type 1=Regular, 2=Open
        {
            AgencyLoginSession loginSession = new AgencyLoginSession();
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "AgencyLoginSP";// LoginSP(old)
                cmd.Parameters.AddWithValue("@UserName", LM.username);
                cmd.Parameters.AddWithValue("@Pwd", LM.Password);
                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        loginSession.LoginStatus = DBNull.Value != reader["LoginStatus"] ? (int)reader["LoginStatus"] : default(int);
                        loginSession.AgencyId = DBNull.Value != reader["AgencyId"] ? (int)reader["AgencyId"] : default(int);
                        loginSession.UserName = DBNull.Value != reader["UserName"] ? (string)reader["UserName"] : default(string);
                        loginSession.AgencyName = DBNull.Value != reader["AgencyName"] ? (string)reader["AgencyName"] : default(string);
                        loginSession.Mobile = DBNull.Value != reader["Mobile"] ? (string)reader["Mobile"] : default(string);
                        loginSession.EmailId = DBNull.Value != reader["EmailId"] ? (string)reader["EmailId"] : default(string);
                        loginSession.SchoolAllows = DBNull.Value != reader["SchoolAllows"] ? (string)reader["SchoolAllows"] : default(string);
                        loginSession.IsActive = DBNull.Value != reader["IsActive"] ? (bool)reader["IsActive"] : default(bool);
                        loginSession.AllowClass = DBNull.Value != reader["AllowClass"] ? (string)reader["AllowClass"] : default(string);
                        loginSession.AllowSubject = DBNull.Value != reader["AllowSubject"] ? (string)reader["AllowSubject"] : default(string);
                        loginSession.AgencyInchargeName = DBNull.Value != reader["AgencyInchargeName"] ? (string)reader["AgencyInchargeName"] : default(string);
                        loginSession.UserType = DBNull.Value != reader["UserType"] ? (string)reader["UserType"] : default(string);
                    }
                }
                Thread.Sleep(2000);
            }
            catch (Exception)
            {
                loginSession = null;
            }
            return Task.FromResult(loginSession);
        }

        public List<AgencySchoolModel> AgencyMasterSP(int type, int id,string cls,string sub ,string Search, out DataSet ds1)
        {
            List<AgencySchoolModel> AgencySchoolModels = new List<AgencySchoolModel>();
            try
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "AgencyMasterSP"; //CUTLISTSP_AG
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@cls", cls);
                cmd.Parameters.AddWithValue("@sub", sub);
                cmd.Parameters.AddWithValue("@Search", Search);
                ds = db.ExecuteDataSet(cmd);
                if (ds != null)
                {
                    
                    var eList = ds.Tables[0].AsEnumerable().Select(dataRow => new AgencySchoolModel
                    {
                        Schl = dataRow.Field<string>("Schl"),
                        SchlNME = dataRow.Field<string>("SchlNME"),
                        Dist = dataRow.Field<string>("Dist"),
                        DistNM = dataRow.Field<string>("DistNM"),
                        MSET = dataRow.Field<string>("MSET"),
                        Matric = dataRow.Field<string>("Matric"),
                        Senior = dataRow.Field<string>("Senior"),
                        SSET = dataRow.Field<string>("SSET"),
                        NOP = dataRow.Field<int>("NOP"),
                        NOMF = dataRow.Field<int>("NOMF"),
                        Udisecode = dataRow.Field<string>("Udisecode"),
                        UserType = dataRow.Field<string>("UserType"),
                        IsMarkedFilled = dataRow.Field<int>("IsMarkedFilled"),
                        IsActive = dataRow.Field<int>("IsActive"),
                        LastDate = dataRow.Field<DateTime>("LastDate"),
                        FinalStatus = dataRow.Field<string>("FinalStatus"),
                        FinalSubmitBy = dataRow.Field<string>("FinalSubmitBy"),
                        FinalSubmitLot = dataRow.Field<string>("FinalSubmitLot"),
                        FinalSubmitOn = dataRow.Field<string>("FinalSubmitOn"),
                        Mobile = dataRow.Field<string>("SchlMOBILE"),
                        clsName = dataRow.Field<string>("clsName"),
                        SubCode = dataRow.Field<string>("SubCode"),
                    }).ToList();

                    AgencySchoolModels = eList.ToList();
                }
                ds1 = ds.Copy();
            }
            catch (Exception ex)
            {
                ds1 = null;
                AgencySchoolModels = null;
            }
        
            return AgencySchoolModels;
        }


        public  List<TblNsqfMaster> TblNsqfMasterSP(int type, int id, string Search)
        {
            List<TblNsqfMaster> TblNsqfMasters = new List<TblNsqfMaster>();
            try
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "TblNsqfMasterSP"; //CUTLISTSP_AG
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@id", id);         
                cmd.Parameters.AddWithValue("@Search", Search);
                ds = db.ExecuteDataSet(cmd);
                if (ds != null)
                {
                    var eList = ds.Tables[0].AsEnumerable().Select(dataRow => new TblNsqfMaster
                    {
                        AgCode = dataRow.Field<string>("AgCode"),
                        PWD = dataRow.Field<string>("PWD"),
                        Sector = dataRow.Field<string>("Sector"),
                        AgencyNM = dataRow.Field<string>("AgencyNM"),
                        AgencyAdd = dataRow.Field<string>("AgencyAdd"),
                        AgencyMob = dataRow.Field<string>("AgencyMob"),
                        AgencyEmail = dataRow.Field<string>("AgencyEmail"),
                        IsActive = dataRow.Field<bool>("IsActive"),
                        AgencyId = dataRow.Field<int>("AgencyId"),
                        AllowClass = dataRow.Field<string>("AllowClass"),
                        AllowSubject = dataRow.Field<string>("AllowSubject"),
                        PNAME1 = dataRow.Field<string>("PNAME1"),
                        PDESI1 = dataRow.Field<string>("PDESI1"),
                        PMOBILE1 = dataRow.Field<string>("PMOBILE1"),
                        PNAME2 = dataRow.Field<string>("PNAME2"),
                        PDESI2 = dataRow.Field<string>("PDESI2"),
                        PMOBILE2 = dataRow.Field<string>("PMOBILE2"),
                        PNAME3 = dataRow.Field<string>("PNAME3"),
                        PDESI3 = dataRow.Field<string>("PDESI3"),
                        PMOBILE3 = dataRow.Field<string>("PMOBILE3"),
                        PNAME4 = dataRow.Field<string>("PNAME4"),
                        PDESI4 = dataRow.Field<string>("PDESI4"),
                        PMOBILE4 = dataRow.Field<string>("PMOBILE4"),
                        PNAME5 = dataRow.Field<string>("PNAME5"),
                        PDESI5 = dataRow.Field<string>("PDESI5"),
                        PMOBILE5 = dataRow.Field<string>("PMOBILE5"),
                        UPDDATE = dataRow.Field<DateTime?>("UPDDATE"),
                        UserType = dataRow.Field<string>("UserType"),
                    }).ToList();
                    TblNsqfMasters = eList.ToList();
                }
            }
            catch (Exception ex)
            {
                TblNsqfMasters = null;
            }
            return TblNsqfMasters;
        }


        public static DataSet CheckSchlAllowToAgency(int type, string agencyId, string schl, string sub, string Search)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "CheckSchlAllowToAgencySP"; //CUTLISTSP_AG  
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@AgencyId", agencyId);
                cmd.Parameters.AddWithValue("@schl", schl);
                cmd.Parameters.AddWithValue("@sub", sub);
                cmd.Parameters.AddWithValue("@Search", Search);
                return db.ExecuteDataSet(cmd);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public DataSet GetNSQFAssesmentDataFormat(int type, string username, string cls,  string Search)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetNSQFAssesmentDataFormatSP";
                cmd.CommandTimeout = 300;
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@cls", cls); 
                cmd.Parameters.AddWithValue("@search", Search);
                return db.ExecuteDataSet(cmd);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataSet ClassSubjectByAgencyId(int type, string agencyId, string cls, string sub , string Search)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ClassSubjectByAgencyIdSP"; 
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@AgencyId", agencyId);
                cmd.Parameters.AddWithValue("@cls", cls);
                cmd.Parameters.AddWithValue("@sub", sub);
                cmd.Parameters.AddWithValue("@Search", Search);
                return db.ExecuteDataSet(cmd);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataSet NSQFPracExamMarksPendingSchoolList(int type, string Search)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "NSQFPracExamMarksPendingSchoolListSP";
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@Search", Search);
                return db.ExecuteDataSet(cmd);
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        #region NSQF Marks Entry Panel 

        public DataSet GetNSQFMarksEntryDataBySCHL(string search,string sub, string pcent, int pageNumber, string class1, int SelectedAction, string schl)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetNSQFPracMarksEntryDataBySCHLPaging"; //GetNSQFPracMarksEntryDataBySCHL            
                cmd.Parameters.AddWithValue("@class", class1);
                cmd.Parameters.AddWithValue("@rp", "R");
                cmd.Parameters.AddWithValue("@cent", pcent);
                cmd.Parameters.AddWithValue("@schl", schl);
                cmd.Parameters.AddWithValue("@Search", search);
                cmd.Parameters.AddWithValue("@sub", sub);
                cmd.Parameters.AddWithValue("@SelectedAction", SelectedAction);
                cmd.Parameters.AddWithValue("@pageNumber", pageNumber);
                cmd.Parameters.AddWithValue("@PageSize", 20);
                return db.ExecuteDataSet(cmd);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


     
        public string AllotNSQFMarksEntry(string submitby, string RP, DataTable dtSub, string class1, out int OutStatus, out string OutError)
        {
            try
            {
                string result = "";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "AllotNSQFMarksEntry"; //AllotCCESenior
                cmd.Parameters.AddWithValue("@submitby", submitby);
                cmd.Parameters.AddWithValue("@RP", RP);
                cmd.Parameters.AddWithValue("@dtSub", dtSub);
                cmd.Parameters.AddWithValue("@class", class1);
                cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                result = db.ExecuteNonQuery(cmd).ToString();
                OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                OutError = (string)cmd.Parameters["@OutError"].Value;
                return result;
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                OutError = ex.Message;
                //mbox(ex);
                return  "";
            }
        }

        public string RemoveNSQFPracMarks(string submitby, string RP, DataTable dtSub, string class1, out int OutStatus, out string OutError)  // BankLoginSP
        {
            try
            {
                string result = "";
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "RemoveNSQFPracMarks"; //AllotCCESenior
                cmd.Parameters.AddWithValue("@submitby", submitby);
                cmd.Parameters.AddWithValue("@RP", RP);
                cmd.Parameters.AddWithValue("@dtSub", dtSub);
                cmd.Parameters.AddWithValue("@class", class1);
                cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                result = db.ExecuteNonQuery(cmd).ToString();
                OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                OutError = (string)cmd.Parameters["@OutError"].Value;
                return result;
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                OutError = ex.Message;
                //mbox(ex);
                return "";
            }
            
        }


        public DataSet NSQFMarksEntryReport(string CenterId, int reporttype, string search, string schl, string cls, out string OutError)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "NSQFMarksEntryReport"; //
                cmd.Parameters.AddWithValue("@CenterId", CenterId);
                cmd.Parameters.AddWithValue("@reporttype", reporttype);
                cmd.Parameters.AddWithValue("@search", search);
                cmd.Parameters.AddWithValue("@schl", schl);
                cmd.Parameters.AddWithValue("@class", cls);
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                ds = db.ExecuteDataSet(cmd);
                OutError = (string)cmd.Parameters["@OutError"].Value;
                return ds;
            }
            catch (Exception ex)
            {
                OutError = "-1";
                return null;
            }
        }


        public DataSet ViewNSQFMarksEntryFinalSubmit(string class1, string rp, string cent, string Search, int SelectedAction, int pageNumber, string sub, string schl)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ViewNSQFMarksEntryFinalSubmitSP"; //GetDataBySCHL            
                cmd.Parameters.AddWithValue("@class", class1);
                cmd.Parameters.AddWithValue("@rp", rp);
                cmd.Parameters.AddWithValue("@cent", cent);
                cmd.Parameters.AddWithValue("@schl", schl);
                cmd.Parameters.AddWithValue("@Search", Search);
                cmd.Parameters.AddWithValue("@sub", sub);
                cmd.Parameters.AddWithValue("@SelectedAction", SelectedAction);
                cmd.Parameters.AddWithValue("@pageNumber", pageNumber);
                cmd.Parameters.AddWithValue("@PageSize", 20);
                return db.ExecuteDataSet(cmd);
            }
            catch (Exception)
            {

                return null;
            }
        }



        //public string NSQFPracExamFinalSubmit(string ExamCent, string class1, string RP, string cent, string sub, string schl, DataTable dtSub, out int OutStatus, out string OutError)  
        //{
        //    string result = "";
        //    try
        //    {
        //        DataSet ds = new DataSet();
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.CommandText = "NSQFPracExamFinalSubmitSPRN"; //GetDataBySCHL            
        //        cmd.Parameters.AddWithValue("@ExamCent", ExamCent);
        //        cmd.Parameters.AddWithValue("@class", class1);
        //        cmd.Parameters.AddWithValue("@RP", RP);
        //        cmd.Parameters.AddWithValue("@cent", cent);
        //        cmd.Parameters.AddWithValue("@sub", sub);
        //        cmd.Parameters.AddWithValue("@schl", schl);
        //        cmd.Parameters.AddWithValue("@dtSub", dtSub);
        //        cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
        //        cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
        //        result = db.ExecuteNonQuery(cmd).ToString();
        //        OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
        //        OutError = (string)cmd.Parameters["@OutError"].Value;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        OutStatus = -1;
        //        OutError = ex.Message;
        //        //mbox(ex);
        //        return result = "";
        //    }
        //}



        public string NSQFPracExamAllSchoolsFinalSubmit(string class1,  string cent, string sub,  out string OutError)
        {
            string result = "";
            try
            {
                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "NSQFPracExamAllSchoolsFinalSubmit"; //NSQFPracExamAllSchoolsFinalSubmit                          
                cmd.Parameters.AddWithValue("@class", class1);            
                cmd.Parameters.AddWithValue("@cent", cent);
                cmd.Parameters.AddWithValue("@sub", sub); 
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                result = db.ExecuteNonQuery(cmd).ToString();               
                OutError = (string)cmd.Parameters["@OutError"].Value;               
            }
            catch (Exception ex)
            {                
                OutError = ex.Message;               
            }
            return OutError;
        }

        #endregion NSQF Marks Entry Panel 

    }

}