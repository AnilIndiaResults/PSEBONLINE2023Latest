using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using PSEBONLINE.Models;
using System.IO;
using System.Web.Mvc;
using System.Net;
using System.Web.Services;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace PSEBONLINE.AbstractLayer
{
    public class RegistrationDB
    {
        private DBContext _context = new DBContext();
        #region Check ConString

        private string CommonCon = "myDBConnection";
        public RegistrationDB()
        {
            if (HttpContext.Current.Session["Session"] == null)
            {
                CommonCon = "myDBConnection";
            }           
            else
            {
                CommonCon = "myDBConnection";
            }

        }


        #endregion  Check ConString



        public static List<SelectListItem> GetSeniorSubjectsBySubList(string subList)
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            DataSet ds = new DataSet();
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select sub,name_eng from ssnew where sub in ("+ subList+")  order by sub";
                cmd.CommandType = CommandType.Text;
                ds = db.ExecuteDataSet(cmd);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return subjects;
        }


        public string IsValidForMatricSubjects(DataTable dtSubject, string Group)
        {
            string res = string.Empty;
          


            if (Group.ToUpper() == "H".ToUpper() || Group.ToUpper() == "HUMANITIES".ToUpper())
            {
                string SUB1 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 1).Select(s => s.Field<string>("SUB")).SingleOrDefault();
                string SUB2 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 2).Select(s => s.Field<string>("SUB")).SingleOrDefault();
                string SUB3 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 3).Select(s => s.Field<string>("SUB")).SingleOrDefault();
                string SUB4 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 4).Select(s => s.Field<string>("SUB")).SingleOrDefault();
                string SUB5 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 5).Select(s => s.Field<string>("SUB")).SingleOrDefault();
                string SUB6 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 6).Select(s => s.Field<string>("SUB")).SingleOrDefault();
                string SUB7 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 7).Select(s => s.Field<string>("SUB")).SingleOrDefault();
                string SUB8 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 8).Select(s => s.Field<string>("SUB")).SingleOrDefault();
                string SUB9 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 9).Select(s => s.Field<string>("SUB")).SingleOrDefault();


               if(SUB1 != "001"){ res += "Subject No 1 is Wrong, "; }
               if(SUB2 != "002" || SUB2 != "003") { res += "Subject No 2 is Wrong, "; }
               if(SUB3 != "139") { res += "Subject No 3 is Wrong, "; }
               if(SUB4 != "146") { res += "Subject No 4 is Wrong, "; }
               if(SUB5 != "210") { res += "Subject No 5 is Wrong, "; }
               if(SUB6 != "141") { res += "Subject No 6 is Wrong, "; }
               if(SUB7 != "142") { res += "Subject No 7 is Wrong, "; }
               if(SUB8 != "026" || SUB8 != "144") { res += "Subject No 8 is Wrong, "; }

               
            }
            else if (Group.ToUpper() == "C".ToUpper() || Group.ToUpper() == "COMMERCE".ToUpper())
            {
                var SUB1rr = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 1);

                string SUB1 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 1).Select(s => s.Field<string>("SUB")).SingleOrDefault();
                string SUB2 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 2).Select(s => s.Field<string>("SUB")).SingleOrDefault();
                string SUB3 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 3).Select(s => s.Field<string>("SUB")).SingleOrDefault();
                string SUB4 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 4).Select(s => s.Field<string>("SUB")).SingleOrDefault();
                string SUB5 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 5).Select(s => s.Field<string>("SUB")).SingleOrDefault();
                string SUB6 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 6).Select(s => s.Field<string>("SUB")).SingleOrDefault();
                string SUB7 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 7).Select(s => s.Field<string>("SUB")).SingleOrDefault();
                string SUB8 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 8).Select(s => s.Field<string>("SUB")).SingleOrDefault();
                string SUB9 = dtSubject.AsEnumerable().Where(r => r.Field<int>("SUB_SEQ") == 9).Select(s => s.Field<string>("SUB")).SingleOrDefault();

                if (SUB1 != "001") { res += "Subject No 1 is Wrong, "; }
                if (SUB2 != "002" && SUB2 != "003") { res += "Subject No 2 is Wrong, "; }
                if (SUB3 != "139") { res += "Subject No 3 is Wrong, "; }
                if (SUB4 != "146") { res += "Subject No 4 is Wrong, "; }
                if (SUB5 != "210") { res += "Subject No 5 is Wrong, "; }
                if (SUB6 != "141") { res += "Subject No 6 is Wrong, "; }
                if (SUB7 != "142") { res += "Subject No 7 is Wrong, "; }
                if (SUB8 != "026" && SUB8 != "144") { res += "Subject No 8 is Wrong, "; }
                if (SUB9 != "026" && SUB9 != "144") { res += "Subject No 9 is Wrong, "; }


            }
            return res;
        }



        #region  GetStudentPreviousYearSearch
        public static List<SeniorStudentMatricResultMarksViews> GetSeniorStudentMatricResultMarksSearch(string form_name, string schl, out DataSet dsOut)
        {
            List<SeniorStudentMatricResultMarksViews> registrationSearchModels = new List<SeniorStudentMatricResultMarksViews>();
            DataSet ds = new DataSet();
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetStudentRecordsSearchPM";
                cmd.Parameters.AddWithValue("@form_name", form_name);// PREV
                cmd.Parameters.AddWithValue("@schl", schl);
                ds = db.ExecuteDataSet(cmd);
                if (ds != null)
                {
                    var eList = StaticDB.DataTableToList<SeniorStudentMatricResultMarksViews>(ds.Tables[0]);
                    registrationSearchModels = eList.ToList();
                }
            }
            catch (Exception ex)
            {

            }
            dsOut = ds;
            return registrationSearchModels;
        }

        //StudentPreviousYear
        public static List<RegistrationSearchStudentPreviousYearMarksModel> GetStudentPreviousYearSearch(string form_name, string schl, out DataSet dsOut)
        {
            List<RegistrationSearchStudentPreviousYearMarksModel> registrationSearchModels = new List<RegistrationSearchStudentPreviousYearMarksModel>();
            DataSet ds = new DataSet();
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetStudentRecordsSearchPM";
                cmd.Parameters.AddWithValue("@form_name", form_name);// PREV
                cmd.Parameters.AddWithValue("@schl", schl);
                ds = db.ExecuteDataSet(cmd);
                if (ds != null)
                {
                    var eList = ds.Tables[0].AsEnumerable().Select(dataRow => new RegistrationSearchStudentPreviousYearMarksModel
                    {
                        Std_id = dataRow.Field<int>("Std_id"),
                        Roll = dataRow.Field<string>("Roll"),
                        form_Name = dataRow.Field<string>("form_Name"),
                        schl = dataRow.Field<string>("schl"),
                        Admission_Date = dataRow.Field<string>("Admission_Date"),
                        Candi_Name = dataRow.Field<string>("Candi_Name"),
                        Father_Name = dataRow.Field<string>("Father_Name"),
                        Mother_Name = dataRow.Field<string>("Mother_Name"),
                        DOB = dataRow.Field<string>("DOB"),
                        LOT = dataRow.Field<int>("LOT"),
                        aadhar_num = dataRow.Field<string>("aadhar_num"),
                        CreatedDate = dataRow.Field<DateTime?>("CreatedDate"),
                        UPDT = dataRow.Field<DateTime?>("UPDT"),
                        SubjectList = dataRow.Field<string>("SubjectList"),
                        //
                        REGNO = dataRow.Field<string>("Registration_num"),
                        ProofCertificate = dataRow.Field<string>("ProofCertificate"),
                        ProofNRICandidates = dataRow.Field<string>("ProofNRICandidates"),
                        StudentUniqueId = dataRow.Field<string>("StudentUniqueId"),
                        schlDist = dataRow.Field<string>("SCHLDIST"),
                        //
                        PYID = dataRow.Field<long>("PYID"),
                        MAT_OBTMARKS = dataRow.Field<int>("MAT_OBTMARKS"),
                        MAT_MAXMARKS = dataRow.Field<int>("MAT_MAXMARKS"),
                        MAT_PERCENTAGE = dataRow.Field<int>("MAT_PERCENTAGE"),
                        MAT_RESULT = dataRow.Field<string>("MAT_RESULT"),
                        ELV_OBTMARKS = dataRow.Field<int>("ELV_OBTMARKS"),
                        ELV_MAXMARKS = dataRow.Field<int>("ELV_MAXMARKS"),
                        ELV_PERCENTAGE = dataRow.Field<int>("ELV_PERCENTAGE"),
                        ELV_RESULT = dataRow.Field<string>("ELV_RESULT"),
                        SubmitOn = dataRow.Field<DateTime?>("SubmitOn"),
                        IsFinalLock = dataRow.Field<bool>("IsFinalLock"),
                        FinalSubmitOn = dataRow.Field<DateTime?>("FinalSubmitOn"),

                        MAT_YEAR = dataRow.Field<string>("MAT_YEAR"),
                        MAT_MONTH = dataRow.Field<string>("MAT_MONTH"),
                        MAT_BOARD = dataRow.Field<string>("MAT_BOARD"),
                        MAT_ROLL = dataRow.Field<string>("MAT_ROLL"),
                        ELV_YEAR = dataRow.Field<string>("ELV_YEAR"),
                        ELV_MONTH = dataRow.Field<string>("ELV_MONTH"),
                        ELV_BOARD = dataRow.Field<string>("ELV_BOARD"),
                        ELV_ROLL = dataRow.Field<string>("ELV_ROLL"),

                    }).ToList();

                    registrationSearchModels = eList.ToList();
                }
            }
            catch (Exception ex)
            {

            }
            dsOut = ds;
            return registrationSearchModels;
        }



        public static List<RegistrationSearchModel> GetStudentRecordsSearchPM(string form_name, string schl, out DataSet dsOut)
        {
            List<RegistrationSearchModel> registrationSearchModels = new List<RegistrationSearchModel>();
            DataSet ds = new DataSet();
            Database db = DatabaseFactory.CreateDatabase("myDBConnection");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetStudentRecordsSearchPM";
            cmd.Parameters.AddWithValue("@form_name", form_name);
            cmd.Parameters.AddWithValue("@schl", schl);
            ds = db.ExecuteDataSet(cmd);
            if (ds != null)
            {
                var eList = ds.Tables[0].AsEnumerable().Select(dataRow => new RegistrationSearchModel
                {
                    Std_id = dataRow.Field<int>("Std_id"),
                    form_Name = dataRow.Field<string>("form_Name"),
                    schl = dataRow.Field<string>("schl"),
                    Admission_Date = dataRow.Field<string>("Admission_Date"),
                    Candi_Name = dataRow.Field<string>("Candi_Name"),
                    Father_Name = dataRow.Field<string>("Father_Name"),
                    Mother_Name = dataRow.Field<string>("Mother_Name"),
                    DOB = dataRow.Field<string>("DOB"),
                    LOT = dataRow.Field<int>("LOT"),
                    aadhar_num = dataRow.Field<string>("aadhar_num"),
                    CreatedDate = dataRow.Field<DateTime?>("CreatedDate"),
                    UPDT = dataRow.Field<DateTime?>("UPDT"),
                    SubjectList = dataRow.Field<string>("SubjectList"),
                    //
                    REGNO = dataRow.Field<string>("Registration_num"),
                    ProofCertificate = dataRow.Field<string>("ProofCertificate"),
                    ProofNRICandidates = dataRow.Field<string>("ProofNRICandidates"),
                    StudentUniqueId = dataRow.Field<string>("StudentUniqueId"),
                    schlDist = dataRow.Field<string>("SCHLDIST"),
                    //
                    Exam = dataRow.Field<string>("Exam"),
                    Group_Name = dataRow.Field<string>("Group_Name"),
                    E_punjab_Std_id = dataRow.Field<string>("E_punjab_Std_id"),
                    Caste = dataRow.Field<string>("Caste"),
                    Differently_Abled = dataRow.Field<string>("Differently_Abled"),
                    Gender = dataRow.Field<string>("Gender"),
                    Belongs_BPL = dataRow.Field<string>("Belongs_BPL"),
                    Religion = dataRow.Field<string>("Religion"),         

                }).ToList();

                registrationSearchModels = eList.ToList();
            }
            dsOut = ds;
            return registrationSearchModels;

        }

        #endregion


        public static int ModifyRegistrationData(string  std_id, string RegNo, string Remarks, string GroupName, string EmpUserId,out int OutStatus)
        {
            try
            {
                string result = "";
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ModifyRegistrationDataSPGroup";
                cmd.Parameters.AddWithValue("@std_id", std_id);
                cmd.Parameters.AddWithValue("@RegNo", RegNo);
                cmd.Parameters.AddWithValue("@Remarks", Remarks);
                cmd.Parameters.AddWithValue("@GroupName", GroupName);
                cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                cmd.Parameters.Add("@outstatus", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                result = db.ExecuteNonQuery(cmd).ToString();
                OutStatus = (int)cmd.Parameters["@outstatus"].Value;
                return OutStatus;
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return OutStatus;
            }

        }

        public static int SwitchForm(string remarks, string stdid, string OldRegNo,string UpdatedBy, out int OutStatus)
        {
            try
            {
                string result = "";
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SwitchFormSP";
                cmd.Parameters.AddWithValue("@OldRegNo", OldRegNo);
                cmd.Parameters.AddWithValue("@remarks", remarks);
                cmd.Parameters.AddWithValue("@stdid", stdid);
                cmd.Parameters.AddWithValue("@UpdatedBy", UpdatedBy);
                string userIP = AbstractLayer.StaticDB.GetFullIPAddress();
                cmd.Parameters.AddWithValue("@UserIP", userIP);               
                cmd.Parameters.Add("@outstatus", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                result = db.ExecuteNonQuery(cmd).ToString();
                OutStatus = (int)cmd.Parameters["@outstatus"].Value;
                return OutStatus;
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return OutStatus;
            }

        }

        public static int ShiftAllFormAdmin(string remarks, string stdid, string OldRegNo, string UpdatedBy, out int OutStatus,
            string OtherBoard, string ShiftFormNM, string VerifiedEmp,string EmpUserId)
        {
            try
            {
                string result = "";
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ShiftAllFormAdminSP";
                cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                cmd.Parameters.AddWithValue("@stdid", stdid);
                cmd.Parameters.AddWithValue("@OldRegNo", OldRegNo);
                cmd.Parameters.AddWithValue("@remarks", remarks);
                cmd.Parameters.AddWithValue("@OtherBoard", OtherBoard);
                cmd.Parameters.AddWithValue("@ShiftFormNM", ShiftFormNM);
                cmd.Parameters.AddWithValue("@VerifiedEmp", VerifiedEmp);              
                cmd.Parameters.AddWithValue("@UpdatedBy", UpdatedBy);
                string userIP = AbstractLayer.StaticDB.GetFullIPAddress();
                cmd.Parameters.AddWithValue("@UserIP", userIP);
                cmd.Parameters.Add("@outstatus", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                result = db.ExecuteNonQuery(cmd).ToString();
                OutStatus = (int)cmd.Parameters["@outstatus"].Value;
                return OutStatus;
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return OutStatus;
            }

        }


        public int InsertParticularCorrectionStaffDetails(ParticularCorrectionStaffDetails model)
        {
            try
            {              
                _context.ParticularCorrectionStaffDetails.Add(model);
                int insertedRecords = _context.SaveChanges();
                // _context?.Dispose();
                return insertedRecords;
            }
            catch (Exception ex)
            {

                return 0;
            }
        }

        public List<SelectListItem> GetStaffListBySCHL(int type, string schl, int staffid)
        {
            List<SelectListItem> staffListt = new List<SelectListItem>();
            DataSet dsSchool = GetStaffDataTableBySCHL(type, schl, staffid); // passing Value to SchoolDB from model
            if (dsSchool.Tables[0].Rows.Count > 0)
            {
                var itemSubUType = dsSchool.Tables[0].AsEnumerable().Select(dataRow => new SelectListItem
                {
                    Text = dataRow.Field<string>("DisplayName").ToString(),
                    Value = dataRow.Field<int>("staffid").ToString(),
                }).ToList();
                staffListt = itemSubUType.ToList();
            }
            return staffListt;
        }
        public DataSet GetStaffDataTableBySCHL(int type,string schl, int staffid)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStaffListBySCHL", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@staffid", staffid);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    // int count = Convert.ToInt32(cmd.Parameters["@PageCount"].Value);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public bool SubjectNotAllowed(string exam, DataTable dtSubject, out string msg)
        {
            // check Sub not allowed
            bool status = true;
            msg = "";
            int cntSub = 0;

            if (exam == "HUMANITIES")
            {
                // '026','141','142'(any one) 
                //(means candidate can choose 2 subject of this category(1 in elective subject & 1 in additional Subject)
                int checkHumML3 = dtSubject.AsEnumerable().Where(r => r.Field<string>("SUBCAT") != "A" && (r.Field<string>("SUB") == "026" || r.Field<string>("SUB") == "141" || r.Field<string>("SUB") == "142")).Count();
                if (checkHumML3 > 1)
                {
                    msg = "Only Two Subjects Are Allowed (1 in elective subject & 1 in additional Subject) - (ECONOMICS(026),BUSINESS STUDIES-II(141),ACCOUNTANCY-II(142)!";
                    status = false;
                    cntSub = checkHumML3;
                }
                //'019','023','024' (any one)
                int checkHumML1 = dtSubject.AsEnumerable().Where(r => r.Field<string>("SUB") == "019" || r.Field<string>("SUB") == "023" || r.Field<string>("SUB") == "024").Count();
                if (checkHumML1 > 1)
                {
                    msg = "Only one Subjects Are Allowed (SANSKRIT(019),FRENCH(023),GERMAN(024)!";
                    status = false;
                    cntSub = checkHumML1;

                }
                // '004','005','006','007'(any one)
                //(means candidate can only choose 1 subject of these category(1 subject in elective subject or in additional Subject)
                int checkHumML2 = dtSubject.AsEnumerable().Where(r => r.Field<string>("SUB") == "004" || r.Field<string>("SUB") == "005" || r.Field<string>("SUB") == "006" || r.Field<string>("SUB") == "007").Count();
                if (checkHumML2 > 1)
                {
                    msg = "Only one Subjects Are Allowed (PUNJABI (ELECTIVE) (004),HINDI (ELECTIVE) (005),ENGLISH (ELECTIVE) (006),URDU (007))!";
                    status = false;
                    cntSub = checkHumML2;
                }
            }
            return status;
        }

        public int CheckSchoolAssignForm(int Type1, string SCHL1)
        {
            int result;
            try
            {
               using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CheckSchoolAssignFormSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type1);
                    cmd.Parameters.AddWithValue("@SCHL", SCHL1);                    
                    cmd.Parameters.Add("@Outstatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    result = (int)cmd.Parameters["@Outstatus"].Value;
                    return result;

                }
            }
            catch (Exception)
            {
                return result = -1;
            }
        }

        public DataSet schooltypes(string schid)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("GetSchoolType", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", schid);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet SearchStudentGetByData1(string schl, string session, string form, int pageIndex)
        {

            // SqlConnection con = null;

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
                    // SqlCommand cmd = new SqlCommand("GetStudentRecordsByID", con);
                    SqlCommand cmd = new SqlCommand("pro_GetStudentBySessionAndFormNameSeparate", con);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@session", session);
                    cmd.Parameters.AddWithValue("@formname", form);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 50);
                    cmd.Parameters.Add("@PageCount", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                    //return GetData(cmd, session);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Close();
                    return result;
                }



            }
            catch (Exception ex)
             {
                 return result = null;
            }
        }
        private static DataSet GetData(SqlCommand cmd, string session)
        {
            string strConnString = ConfigurationManager.ConnectionStrings["myConn"].ConnectionString;           
            using (SqlConnection con = new SqlConnection(strConnString))
            //using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter())
                {
                    cmd.Connection = con;
                    sda.SelectCommand = cmd;
                    using (DataSet ds = new DataSet())
                    {
                        if (session == "2013-2014")
                            sda.Fill(ds, "regMaster2013_14");
                        else if (session == "2014-2015")
                            sda.Fill(ds, "regMasterRegular2014");
                        else if (session == "2015-2016")
                            sda.Fill(ds, "regMasterRegular2015");
                        else if (session == "2016-2017")
                            sda.Fill(ds, "regMasterRegular2016");
                        DataTable dt = new DataTable("PageCount");
                        dt.Columns.Add("PageCount");
                        dt.Rows.Add();
                        dt.Rows[0][0] = cmd.Parameters["@PageCount"].Value;
                        int count = Convert.ToInt32(cmd.Parameters["@PageCount"].Value);
                        ds.Tables.Add(dt);
                        return ds;
                    }
                }
            }
        }


        public DataSet SearchStudentGetByData2(string schl, string session, string form, string searchby, int pageIndex)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("pro_GetStudentBySessionAndFormNameSearchSeparate", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@session", session);
                    cmd.Parameters.AddWithValue("@formname", form);
                    cmd.Parameters.AddWithValue("@searchby", searchby);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 50);
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(ds);                   
                    return ds;
                }
            }
            catch (Exception ex)
            {               
                return null;
            }
        }



        //public DataSet SearchStudentGetByData2(string schl, string session, string form, string searchby, int pageIndex)
        //{
        //   DataSet result = new DataSet();
        //    SqlDataAdapter ad = new SqlDataAdapter();
        //    try
        //    {

        //        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
        //        {
        //           SqlCommand cmd = new SqlCommand("pro_GetStudentBySessionAndFormNameSearchSeparate", con);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@schl", schl);
        //            cmd.Parameters.AddWithValue("@session", session);
        //            cmd.Parameters.AddWithValue("@formname", form);
        //            cmd.Parameters.AddWithValue("@searchby", searchby);
        //            cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
        //            cmd.Parameters.AddWithValue("@PageSize", 50);
        //            cmd.Parameters.Add("@PageCount", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
        //            con.Open();                   
        //            ad.SelectCommand = cmd;
        //            ad.Fill(result);
        //            int PageCount = (int)cmd.Parameters["@PageCount"].Value;
        //            return result;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return result = null;
        //    }
        //}

        public DataSet CheckLogin(LoginModel LM)  // Type 1=Regular, 2=Open
        {
            // int iRetVal;
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("LoginSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", LM.username);
                    cmd.Parameters.AddWithValue("@Password", LM.Password);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    //iRetVal = cmd.ExecuteNonQuery();
                    //iRetVal = (int)cmd.Parameters["@OutStatus"].Value;
                    //return iRetVal;
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    return dataTable;

                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                // con.Close();
            }
        }

        //---------------Select AllDist
        public DataSet SelectDist()
        {

            // SqlConnection con = null;

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("GetAllDistrict", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@SCHL", schid);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        //---------------Select AllTehsil
        public DataSet SelectAllTehsil(int DISTID)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("GetAllTehsil", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DISTID", DISTID);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SelectlastEntryCandidate(string formName, string schl)
        {

            // SqlConnection con = null;

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("GetLastEntryStudentRecords", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@formName", formName);
                    cmd.Parameters.AddWithValue("@schl", schl);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        //-------------------Rough Report and Std Verification Form--------------------
        public DataSet GetStudentRoughReport(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudentRoughReport_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetStudentVerificationForm(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudentVerificationForm_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        //Final Report 2017-18
        public DataSet GetStudentFinalPrint(string schl, string lot,string Challanid)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudentFinalPrintSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@lot", lot);
                    cmd.Parameters.AddWithValue("@Challanid", Challanid);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        //Final Report 2017-18
        //public DataSet GetStudentFinalPrint(string search)
        //{
        //    DataSet result = new DataSet();
        //    SqlDataAdapter ad = new SqlDataAdapter();
        //    try
        //    {

        //        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
        //        {
        //            SqlCommand cmd = new SqlCommand("GetStudentFinalPrint_SP", con);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@search", search);

        //            ad.SelectCommand = cmd;
        //            ad.Fill(result);
        //            con.Open();
        //            return result;
        //        }



        //    }
        //    catch (Exception ex)
        //    {
        //        return result = null;
        //    }
        //}
        //--------------------------------------------------------------------------------//


        //

        public string Ins_Data(RegistrationModels RM, FormCollection frm, string FormType, string session, string idno, string schl)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("insert_N1_N2_N3Forms_Sp", con); //   //insert_N1_N2_N3Forms_Ranjan_Sp
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IsNRICandidate", RM.IsNRICandidate);
                cmd.Parameters.AddWithValue("@form_Name", FormType);
                cmd.Parameters.AddWithValue("@Category", RM.Category);
                cmd.Parameters.AddWithValue("@Board", RM.Board);
                cmd.Parameters.AddWithValue("@Other_Board", RM.Other_Board);
                cmd.Parameters.AddWithValue("@Board_Roll_Num", RM.Board_Roll_Num);
                // cmd.Parameters.AddWithValue("@Prev_School_Name", RM.Prev_School_Name);
               // cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                cmd.Parameters.AddWithValue("@Month", RM.Month);
                cmd.Parameters.AddWithValue("@Year", RM.Year);
                //cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                // cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);
                cmd.Parameters.AddWithValue("@Admission_Date", RM.Admission_Date);
                cmd.Parameters.AddWithValue("@Class_Roll_Num_Section", RM.Class_Roll_Num_Section);

                cmd.Parameters.AddWithValue("@Candi_Name", RM.Candi_Name);
                cmd.Parameters.AddWithValue("@Candi_Name_P", RM.Candi_Name_P);
                cmd.Parameters.AddWithValue("@Father_Name", RM.Father_Name);
                cmd.Parameters.AddWithValue("@Father_Name_P", RM.Father_Name_P);
                cmd.Parameters.AddWithValue("@Mother_Name", RM.Mother_Name);
                cmd.Parameters.AddWithValue("@Mother_Name_P", RM.Mother_Name_P);
                cmd.Parameters.AddWithValue("@Caste", RM.Caste);
                cmd.Parameters.AddWithValue("@Gender", RM.Gender);
                cmd.Parameters.AddWithValue("@Differently_Abled", frm["DA"].ToString());
                cmd.Parameters.AddWithValue("@Religion", RM.Religion);
                cmd.Parameters.AddWithValue("@DOB", RM.DOB);
                cmd.Parameters.AddWithValue("@Belongs_BPL", RM.Belongs_BPL);
                cmd.Parameters.AddWithValue("@Mobile", RM.Mobile);
                cmd.Parameters.AddWithValue("@Aadhar_num", RM.Aadhar_num);
                cmd.Parameters.AddWithValue("@E_punjab_Std_id", RM.E_punjab_Std_id);
                cmd.Parameters.AddWithValue("@Address", RM.Address);
                cmd.Parameters.AddWithValue("@LandMark", RM.LandMark);
                cmd.Parameters.AddWithValue("@Block", RM.Block);
                cmd.Parameters.AddWithValue("@Tehsil", frm["MyTeh"].ToString());
                cmd.Parameters.AddWithValue("@District", frm["MyDist"].ToString());
                cmd.Parameters.AddWithValue("@PinCode", RM.PinCode);

                if (HttpContext.Current.Session["Session"].ToString() == "2023-2024")
                {
                if(frm["DA"].ToString()=="N.A.")
                {
                    cmd.Parameters.AddWithValue("@DP", 0);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@DP",RM.DP);
                }
                }
                    

                if (FormType == "N1" || FormType == "N2" || FormType == "N3")
                {
                    if (RM.Provisional.ToString() == "True")
                    {

                        cmd.Parameters.AddWithValue("@Provisional", 1);
                        //cmd.Parameters.AddWithValue("@AWRegisterNo", NULL);
                        //cmd.Parameters.AddWithValue("@Admission_Num", NULL);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Provisional", 0);
                        cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                        cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);

                    }
                    if (RM.IsLateAdm.ToString() == "True")
                    {
                        cmd.Parameters.AddWithValue("@RequestID", RM.requestID);                        
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@RequestID", 0);
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                    cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);
                }


                if (RM.IsPrevSchoolSelf.ToString() == "True")
                {
                    //cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", RM.IsPrevSchoolSelf);
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 1);
                    cmd.Parameters.AddWithValue("@Prev_School_Name", RM.Prev_School_Name);
                }
                else
                {
                    //cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", RM.IsPrevSchoolSelf);
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 0);
                    cmd.Parameters.AddWithValue("@Prev_School_Name", RM.Prev_School_Name);
                }

                if (RM.file != null)
                {
                    cmd.Parameters.AddWithValue("@std_Photo", RM.file.FileName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Photo", null);
                }
                if (RM.std_Sign != null)
                {
                    cmd.Parameters.AddWithValue("@std_Sign", RM.std_Sign.FileName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Sign", null);
                }

                //cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", RM.IsPrevSchoolSelf.ToString() == "True" ? 1 : 0);
                cmd.Parameters.AddWithValue("@Section", RM.Section);

                cmd.Parameters.AddWithValue("@SCHL", schl);
                cmd.Parameters.AddWithValue("@IDNO", idno);
                cmd.Parameters.AddWithValue("@SESSION", session);
                

                if (FormType == "N1")
                {
                    if (HttpContext.Current.Session["Session"].ToString() == "2023-2024")
                    {
                    cmd.Parameters.AddWithValue("@Registration_num", null);
                    cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                    cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                    if (RM.subS6 == "" || RM.subS6 == null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", null);

                    }
                    else if (RM.subS6 != "" || RM.subS6 != null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.subS6);

                    }
                    }
                    
                }
                else if (FormType == "N2")
                {
                    if (RM.IsPSEBRegNum.ToString() == "True")
                    {
                        cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                        cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Registration_num", null);
                        cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                    }
                    
                    cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQF);
                    if(RM.Category.ToUpper() == "8th PASSED".ToUpper())
                    {
                        if (RM.subS6 == "" || RM.subS6 == null)
                        {
                            if (RM.PreNSQF == "NO")
                            {
                                cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                                cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                                cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            }

                        }
                        else if (RM.subS6 != "" || RM.subS6 != null)
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", RM.subS6);

                        }
                    }
                    else if (RM.Category.ToUpper() == "9th Failed".ToUpper())
                    {
                        if (RM.NsqfsubS6 == "" || RM.NsqfsubS6 == null)
                        {
                            if (RM.PreNSQF == "NO")
                            {
                                cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                                cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                                cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            }

                        }
                        else if (RM.NsqfsubS6 != "" || RM.NsqfsubS6 != null)
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6);

                        }
                    }

                }

                else if (FormType == "N3")
                {
                    cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                    cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                    cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                    if (RM.subS6 == "" || RM.subS6 == null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", null);

                    }
                    else if (RM.subS6 != "" || RM.subS6 != null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.subS6);

                    }
                }
                cmd.Parameters.AddWithValue("@CandStudyMedium", RM.CandiMedium);

                cmd.Parameters.AddWithValue("@NSQFPattern", RM.NSQFPattern);

                // cmd.Parameters.Add("@StudentUniqueId", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                SqlParameter outPutParameter = new SqlParameter();
                outPutParameter.ParameterName = "@StudentUniqueId";
                outPutParameter.Size = 20;
                outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                outPutParameter.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(outPutParameter);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = (string)cmd.Parameters["@StudentUniqueId"].Value;
                return outuniqueid;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }

        public string Update_Regular_Photo_Sign_Docs(string CANDID, string Photo, string Sign, string ProofCertificate, string ProofNRICandidates, string Type)
        {
            SqlConnection con = null;
            string result = "";
            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                
                SqlCommand cmd = new SqlCommand("Update_Regular_Photo_Sign_DocsSP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StudentUniqueID", CANDID);
                cmd.Parameters.AddWithValue("@Photo", Photo);
                cmd.Parameters.AddWithValue("@Sign", Sign);
                cmd.Parameters.AddWithValue("@ProofCertificate", ProofCertificate);
                cmd.Parameters.AddWithValue("@ProofNRICandidates", ProofNRICandidates);
                cmd.Parameters.AddWithValue("@Type", Type);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }

        public string Updated_PhotoSign_ByStudentId(string stdid, string PhotoSignName, string Type,string IsNew)
        {
            SqlConnection con = null;
            string result = "";
            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                //  con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("Updated_PhotoSign_ByStudentIdSP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@stdid", stdid);
                cmd.Parameters.AddWithValue("@PhotoSignName", PhotoSignName);
                cmd.Parameters.AddWithValue("@Type", Type);
                cmd.Parameters.AddWithValue("@IsNew", IsNew);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }


        public string Updated_Pic_Data(string Myresult, string PhotoSignName, string Type, string IsNew="YES")
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
              //  con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("Update_Uploaded_Photo_Sign", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StudentUniqueID", Myresult);
                cmd.Parameters.AddWithValue("@PhotoSignName", PhotoSignName);
                cmd.Parameters.AddWithValue("@Type", Type);
                cmd.Parameters.AddWithValue("@IsNew", IsNew);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }
        public string Update_N1_Data(RegistrationModels RM, FormCollection frm, string FormType, string sid, string FilePhoto, string sign)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                //string formName = "N1";
                RM.Caste = frm["CasteSelected"];
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                // con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("Update_N1_N2_N3Forms_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IsCorrectionInParticular", RM.IsCorrectionInParticular);
                cmd.Parameters.AddWithValue("@IsNRICandidate", RM.IsNRICandidate);
                cmd.Parameters.AddWithValue("@Std_id", sid);
                cmd.Parameters.AddWithValue("@form_Name", FormType);
                cmd.Parameters.AddWithValue("@Category", RM.Category);
                cmd.Parameters.AddWithValue("@Board", RM.Board);
                cmd.Parameters.AddWithValue("@Other_Board", RM.Other_Board);
                cmd.Parameters.AddWithValue("@Board_Roll_Num", RM.Board_Roll_Num);
                // cmd.Parameters.AddWithValue("@Prev_School_Name", RM.Prev_School_Name);
                //cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                cmd.Parameters.AddWithValue("@Month", RM.Month);
                cmd.Parameters.AddWithValue("@Year", RM.Year);
                // cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                // cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);
                cmd.Parameters.AddWithValue("@Admission_Date", RM.Admission_Date);
                cmd.Parameters.AddWithValue("@Class_Roll_Num_Section", RM.Class_Roll_Num_Section);
                cmd.Parameters.AddWithValue("@Candi_Name", RM.Candi_Name);
                cmd.Parameters.AddWithValue("@Candi_Name_P", RM.Candi_Name_P);
                cmd.Parameters.AddWithValue("@Father_Name", RM.Father_Name);
                cmd.Parameters.AddWithValue("@Father_Name_P", RM.Father_Name_P);
                cmd.Parameters.AddWithValue("@Mother_Name", RM.Mother_Name);
                cmd.Parameters.AddWithValue("@Mother_Name_P", RM.Mother_Name_P);
                cmd.Parameters.AddWithValue("@Caste", RM.Caste);
                cmd.Parameters.AddWithValue("@Gender", RM.Gender);
                //cmd.Parameters.AddWithValue("@Differently_Abled", frm["DA"].ToString());
                cmd.Parameters.AddWithValue("@Differently_Abled", RM.Differently_Abled);
                if (HttpContext.Current.Session["Session"].ToString() == "2023-2024")
                {
                if (RM.Differently_Abled == "N.A.")
                {
                    cmd.Parameters.AddWithValue("@DP", 0);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@DP", RM.DP);
                }
                }
               
                //cmd.Parameters.AddWithValue("@Religion", RM.Religion);
                if (RM.Religion == "Hindu" || RM.Religion == "Muslim" || RM.Religion == "Sikh" || RM.Religion == "Christian" || RM.Religion == "Others")
                {
                    cmd.Parameters.AddWithValue("@Religion", RM.Religion);
                }
                else
                {
                    RM.Religion = "Others";
                    cmd.Parameters.AddWithValue("@Religion", RM.Religion);
                }


                cmd.Parameters.AddWithValue("@DOB", RM.DOB);
                cmd.Parameters.AddWithValue("@Belongs_BPL", RM.Belongs_BPL);
                cmd.Parameters.AddWithValue("@Mobile", RM.Mobile);
                cmd.Parameters.AddWithValue("@Aadhar_num", RM.Aadhar_num);
                cmd.Parameters.AddWithValue("@E_punjab_Std_id", RM.E_punjab_Std_id);
                cmd.Parameters.AddWithValue("@Address", RM.Address);
                cmd.Parameters.AddWithValue("@LandMark", RM.LandMark);
                cmd.Parameters.AddWithValue("@Block", RM.Block);

                //cmd.Parameters.AddWithValue("@District",RM.District);
                cmd.Parameters.AddWithValue("@District", RM.MyDistrict);
                cmd.Parameters.AddWithValue("@Tehsil", RM.Tehsil);


                if (FormType == "N1" || FormType == "N2" || FormType == "N3")
                {
                    if (RM.Provisional.ToString() == "True")
                    {

                        cmd.Parameters.AddWithValue("@Provisional", 1);
                        //cmd.Parameters.AddWithValue("@AWRegisterNo", NULL);
                        //cmd.Parameters.AddWithValue("@Admission_Num", NULL);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Provisional", 0);
                        cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                        cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);

                    }
                    if (RM.IsLateAdm.ToString() == "True")
                    {
                        cmd.Parameters.AddWithValue("@RequestID", RM.requestID);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@RequestID", 0);
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                    cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);
                }
                if (RM.IsPrevSchoolSelf.ToString() == "True")
                {
                    //cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", RM.IsPrevSchoolSelf);
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 1);
                    cmd.Parameters.AddWithValue("@Prev_School_Name", RM.Prev_School_Name);
                }
                else
                {
                    //cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", RM.IsPrevSchoolSelf);
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 0);
                    cmd.Parameters.AddWithValue("@Prev_School_Name", RM.Prev_School_Name);
                }
                cmd.Parameters.AddWithValue("@PinCode", RM.PinCode);
                if (RM.file == null)
                {
                    cmd.Parameters.AddWithValue("@std_Photo", FilePhoto);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Photo", RM.file.FileName);
                }
                if (RM.std_Sign == null)
                {
                    cmd.Parameters.AddWithValue("@std_Sign", sign);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Sign", RM.std_Sign.FileName);
                }

                cmd.Parameters.AddWithValue("@Section", RM.Section);

                string hostName = Dns.GetHostName(); // Retrive the Name of HOST
                string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                cmd.Parameters.AddWithValue("@MyIP", myIP);



                if (FormType == "N1")
                {

                    cmd.Parameters.AddWithValue("@Registration_num", null);
                    cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                    cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                    if (RM.subS6 == "" || RM.subS6 == null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                    }
                    else if (RM.subS6 != "" || RM.subS6 != null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.subS6);

                    }


                }
                else if (FormType == "N2")
                {
                    if (RM.IsPSEBRegNum.ToString() == "True")
                    {
                        cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                        cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Registration_num", null);
                        cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                    }


                    if (RM.subS6 == "" || RM.subS6 == null)
                    {
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                        cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                    }
                    else if (RM.subS6 != "" || RM.subS6 != null)
                    {
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.subS6);

                    }

                    //if (RM.Category.ToUpper() == "8th PASSED".ToUpper())
                    //{
                    //    if (RM.subS6 == "" || RM.subS6 == null)
                    //    {
                    //        if (RM.PreNSQF == "NO")
                    //        {
                    //            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                    //            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                    //            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                    //        }
                    //        else
                    //        {
                    //            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                    //            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                    //            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                    //        }

                    //    }
                    //    else if (RM.subS6 != "" || RM.subS6 != null)
                    //    {
                    //        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                    //        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.subS6);
                    //        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);

                    //    }
                    //}
                    //else if (RM.Category.ToUpper() == "9th Failed".ToUpper())
                    //{
                    //    cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQF);
                    //    if (RM.NsqfsubS6 == "" || RM.NsqfsubS6 == null)
                    //    {
                    //        if (RM.PreNSQF == "NO")
                    //        {
                    //            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                    //            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                    //        }
                    //        else
                    //        {
                    //            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                    //            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                    //        }

                    //    }
                    //    else if (RM.NsqfsubS6 != "" || RM.NsqfsubS6 != null)
                    //    {
                    //        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                    //        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6);

                    //    }
                    //}

                }

                else if (FormType == "N3")
                {
                    cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                    cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                    cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                    if (RM.subS6 == "" || RM.subS6 == null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                    }
                    else if (RM.subS6 != "" || RM.subS6 != null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.subS6);

                    }
                }
                if (HttpContext.Current.Session["Session"].ToString() == "2023-2024")
                {
                cmd.Parameters.AddWithValue("@CandStudyMedium", RM.CandiMedium);
                }


                cmd.Parameters.AddWithValue("@NSQFPattern", RM.NSQFPattern);
                //
                SqlParameter outPutParameter = new SqlParameter();
                outPutParameter.ParameterName = "@StudentUniqueId";
                outPutParameter.Size = 20;
                outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                outPutParameter.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(outPutParameter);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = (string)cmd.Parameters["@StudentUniqueId"].Value;
                return outuniqueid;


                //con.Open();
                //result = cmd.ExecuteNonQuery().ToString();
                //return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "-1";
            }
            finally
            {
                con.Close();
            }
        }
        public string Update_N2_Data(RegistrationModels RM, FormCollection frm, string FormType, string sid, string FilePhoto, string sign)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                //string formName = "N1";
                RM.Caste = frm["CasteSelected"];
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("Update_N1_N2_N3Forms_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IsNRICandidate", RM.IsNRICandidate);
                cmd.Parameters.AddWithValue("@Std_id", sid);
                cmd.Parameters.AddWithValue("@form_Name", FormType);
                cmd.Parameters.AddWithValue("@Category", RM.Category);
                cmd.Parameters.AddWithValue("@Board", RM.Board);
                if (RM.Board != "OTHER BOARD")
                {
                    cmd.Parameters.AddWithValue("@Other_Board", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Other_Board", RM.Other_Board);
                }


                cmd.Parameters.AddWithValue("@Board_Roll_Num", RM.Board_Roll_Num);
                cmd.Parameters.AddWithValue("@Prev_School_Name", RM.Prev_School_Name);
                cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                cmd.Parameters.AddWithValue("@Month", RM.Month);
                cmd.Parameters.AddWithValue("@Year", RM.Year);
                cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);
                cmd.Parameters.AddWithValue("@Admission_Date", RM.Admission_Date);
                cmd.Parameters.AddWithValue("@Class_Roll_Num_Section", RM.Class_Roll_Num_Section);
                cmd.Parameters.AddWithValue("@Candi_Name", RM.Candi_Name);
                cmd.Parameters.AddWithValue("@Candi_Name_P", RM.Candi_Name_P);
                cmd.Parameters.AddWithValue("@Father_Name", RM.Father_Name);
                cmd.Parameters.AddWithValue("@Father_Name_P", RM.Father_Name_P);
                cmd.Parameters.AddWithValue("@Mother_Name", RM.Mother_Name);
                cmd.Parameters.AddWithValue("@Mother_Name_P", RM.Mother_Name_P);
                cmd.Parameters.AddWithValue("@Caste", RM.Caste);
                cmd.Parameters.AddWithValue("@Gender", RM.Gender);
                //cmd.Parameters.AddWithValue("@Differently_Abled", frm["DA"].ToString());
                cmd.Parameters.AddWithValue("@Differently_Abled", RM.Differently_Abled);
                cmd.Parameters.AddWithValue("@Religion", RM.Religion);
                cmd.Parameters.AddWithValue("@DOB", RM.DOB);
                cmd.Parameters.AddWithValue("@Belongs_BPL", RM.Belongs_BPL);
                cmd.Parameters.AddWithValue("@Mobile", RM.Mobile);
                cmd.Parameters.AddWithValue("@Aadhar_num", RM.Aadhar_num);
                cmd.Parameters.AddWithValue("@E_punjab_Std_id", RM.E_punjab_Std_id);
                cmd.Parameters.AddWithValue("@Address", RM.Address);
                cmd.Parameters.AddWithValue("@LandMark", RM.LandMark);
                cmd.Parameters.AddWithValue("@Block", RM.Block);

               
                cmd.Parameters.AddWithValue("@District", RM.MyDistrict);
                cmd.Parameters.AddWithValue("@Tehsil", RM.Tehsil);
                cmd.Parameters.AddWithValue("@PinCode", RM.PinCode);
                if (RM.file == null)
                {
                    cmd.Parameters.AddWithValue("@std_Photo", FilePhoto);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Photo", RM.file.FileName);
                }
                if (RM.std_Sign == null)
                {
                    cmd.Parameters.AddWithValue("@std_Sign", sign);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Sign", RM.std_Sign.FileName);
                }

                cmd.Parameters.AddWithValue("@Section", RM.Section);
                cmd.Parameters.AddWithValue("@NSQFPattern", RM.NSQFPattern);
                string hostName = Dns.GetHostName(); // Retrive the Name of HOST
                string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                cmd.Parameters.AddWithValue("@MyIP", myIP);


                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "-1";
            }
            finally
            {
                con.Close();
            }
        }
        public DataSet SearchStudentGetByData(string schid, string session, string id, string formname)
        {

            // SqlConnection con = null;

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
                    // SqlCommand cmd = new SqlCommand("GetStudentRecordsByID", con);
                    SqlCommand cmd = new SqlCommand("GetStudentRecordsBySchIDAndSession", con);

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schid", schid);
                    cmd.Parameters.AddWithValue("@session", session);
                    cmd.Parameters.AddWithValue("@sid", id);
                    cmd.Parameters.AddWithValue("@formName", formname);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SearchStudentGetByData(string sid, string frmname)
        {

            // SqlConnection con = null;

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("GetStudentRecordsByID", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@sid", sid);
                    cmd.Parameters.AddWithValue("@formName", frmname);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet GetStudentRecordsSearch(string search, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudentRecordsSearch", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        //------------------------Delete Data------------------
        public string DeleteFromData(string stdid, string epunid, string imptblname)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Delete_N1_N2_N3Forms_Import_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Std_id", stdid);
                cmd.Parameters.AddWithValue("@epunid", epunid);
                cmd.Parameters.AddWithValue("@imptblname", imptblname);

                cmd.Parameters.AddWithValue("@MyIP", myIP);

                con.Open();
                result = cmd.ExecuteScalar().ToString();
                return result;

            }
            catch (Exception ex)
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }

        //------------------------Delete Data------------------
        public string DeleteFromDataN2(string stdid)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Delete_N1_N2_N3Forms_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Std_id", stdid);
                cmd.Parameters.AddWithValue("@MyIP", myIP);

                con.Open();
                result = cmd.ExecuteScalar().ToString();
                return result;

            }
            catch (Exception ex)
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }
        //-----------------
        //-----------------
        public string DeleteFromDataN3(string stdid, string Oldid, string oldyr, string Imptblname)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            try
            {
                
                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("DeleteFromDataN3Imported_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Std_id", stdid);
                cmd.Parameters.AddWithValue("@Oldid", Oldid);
                cmd.Parameters.AddWithValue("@oldyr", oldyr);
                cmd.Parameters.AddWithValue("@Imptblname", Imptblname);

                cmd.Parameters.AddWithValue("@MyIP", myIP);

                con.Open();
                result = cmd.ExecuteScalar().ToString();
                return result;

            }
            catch (Exception ex)
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }
        //-------------------------------------Work On E2-Forms---------------------
        public string Ins_E_Form_Data(RegistrationModels RM, FormCollection frm, string FormType, string session, string idno, string schl)
        {
            SqlConnection con = null;
            string result = "";
            try
            {

                               
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("insert_E1_E2_E3Forms_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@OTID", RM.OTID);
                cmd.Parameters.AddWithValue("@Imptblname", RM.Imptblname);
                cmd.Parameters.AddWithValue("@IsNRICandidate", RM.IsNRICandidate);
                cmd.Parameters.AddWithValue("@form_Name", FormType);
                cmd.Parameters.AddWithValue("@Category", RM.Category);
                cmd.Parameters.AddWithValue("@Board", RM.Board);
                cmd.Parameters.AddWithValue("@Other_Board", RM.Other_Board);
                cmd.Parameters.AddWithValue("@Board_Roll_Num", RM.Board_Roll_Num);
                cmd.Parameters.AddWithValue("@Prev_School_Name", RM.Prev_School_Name);
                //cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                //cmd.Parameters.AddWithValue("@Group_Name", frm["MyGroup"].ToString());

                cmd.Parameters.AddWithValue("@MetricMonth", RM.MetricMonth);
                cmd.Parameters.AddWithValue("@MetricYear", RM.MetricYear);
                cmd.Parameters.AddWithValue("@MetricRollNum", RM.Metric_Roll_Num);

                cmd.Parameters.AddWithValue("@Group_Name", RM.MyGroup);
                if (FormType == "E2")
                {
                    if (RM.IsPSEBRegNum.ToString() == "True")
                    {

                        cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                        cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                    }
                    else
                    {

                        cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                    cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                }
                if (FormType == "E1" || FormType == "E2")
                {
                    if (RM.Provisional.ToString() == "True")
                    {

                        cmd.Parameters.AddWithValue("@Provisional", 1);
                        cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                        cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Provisional", 0);
                        cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                        cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);

                    }
                    if (RM.IsLateAdm.ToString() == "True")
                    {
                        cmd.Parameters.AddWithValue("@RequestID", RM.requestID);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@RequestID", 0);
                    }
                }
                cmd.Parameters.AddWithValue("@Month", RM.Month);
                cmd.Parameters.AddWithValue("@Year", RM.Year);
                // cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                //cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);
                cmd.Parameters.AddWithValue("@Admission_Date", RM.Admission_Date);
                cmd.Parameters.AddWithValue("@Class_Roll_Num_Section", RM.Class_Roll_Num_Section);

                cmd.Parameters.AddWithValue("@Candi_Name", RM.Candi_Name);
                cmd.Parameters.AddWithValue("@Candi_Name_P", RM.Candi_Name_P);
                cmd.Parameters.AddWithValue("@Father_Name", RM.Father_Name);
                cmd.Parameters.AddWithValue("@Father_Name_P", RM.Father_Name_P);
                cmd.Parameters.AddWithValue("@Mother_Name", RM.Mother_Name);
                cmd.Parameters.AddWithValue("@Mother_Name_P", RM.Mother_Name_P);
                cmd.Parameters.AddWithValue("@Caste", RM.Caste);
                cmd.Parameters.AddWithValue("@Gender", RM.Gender);
                cmd.Parameters.AddWithValue("@Differently_Abled", frm["DA"].ToString());
                cmd.Parameters.AddWithValue("@Religion", RM.Religion);
                cmd.Parameters.AddWithValue("@DOB", RM.DOB);
                cmd.Parameters.AddWithValue("@Belongs_BPL", RM.Belongs_BPL);
                cmd.Parameters.AddWithValue("@Mobile", RM.Mobile);
                cmd.Parameters.AddWithValue("@Aadhar_num", RM.Aadhar_num);
                cmd.Parameters.AddWithValue("@E_punjab_Std_id", RM.E_punjab_Std_id);
                cmd.Parameters.AddWithValue("@Address", RM.Address);
                cmd.Parameters.AddWithValue("@LandMark", RM.LandMark);
                cmd.Parameters.AddWithValue("@Block", RM.Block);
                cmd.Parameters.AddWithValue("@Tehsil", frm["MyTeh"].ToString());
                cmd.Parameters.AddWithValue("@District", frm["MyDist"].ToString());
                cmd.Parameters.AddWithValue("@PinCode", RM.PinCode);
                if (RM.IsPrevSchoolSelf.ToString() == "True")
                {
                    //cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", RM.IsPrevSchoolSelf);
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 0);
                }

                if (RM.file != null)
                {
                    cmd.Parameters.AddWithValue("@std_Photo", RM.file.FileName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Photo", null);
                }
                if (RM.std_Sign != null)
                {
                    cmd.Parameters.AddWithValue("@std_Sign", RM.std_Sign.FileName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Sign", null);
                }

                //cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", RM.IsPrevSchoolSelf.ToString() == "True" ? 1 : 0);
                cmd.Parameters.AddWithValue("@Section", RM.Section);
                if (HttpContext.Current.Session["Session"].ToString() == "2023-2024")
                {
                if (frm["DA"].ToString() == "N.A.")
                {
                    cmd.Parameters.AddWithValue("@DP", 0);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@DP", RM.DP);
                }
                }


                cmd.Parameters.AddWithValue("@SCHL", schl);
                cmd.Parameters.AddWithValue("@IDNO", idno);
                cmd.Parameters.AddWithValue("@SESSION", session);
             
                cmd.Parameters.AddWithValue("@MatricBoard", RM.MetricBoard);
                    cmd.Parameters.AddWithValue("@CandStudyMedium", RM.CandiMedium);
                cmd.Parameters.AddWithValue("@MatricResult", RM.MatricResult);

                if (RM.PreNSQF == "" || RM.PreNSQF == null)
                {
                    if (RM.PreNSQF == "NO")
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                    }

                }
                else if (RM.PreNSQF != "" || RM.PreNSQF != null)
                {
                    cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                    cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6);
                    cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQF);

                }

                cmd.Parameters.AddWithValue("@NSQFPattern", RM.NSQFPattern);

                // cmd.Parameters.Add("@StudentUniqueId", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                SqlParameter outPutParameter = new SqlParameter();
                outPutParameter.ParameterName = "@StudentUniqueId";
                outPutParameter.Size = 20;
                outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                outPutParameter.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(outPutParameter);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = (string)cmd.Parameters["@StudentUniqueId"].Value;
                return outuniqueid;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "-1";
            }
            finally
            {
                con.Close();
            }
        }
        public string Updated_Pic_E_Form_Data(string Myresult, string PhotoSignName, string Type)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                //string formName = "N1";

                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_Uploaded_E_Form_Photo_Sign", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StudentUniqueID", Myresult);
                cmd.Parameters.AddWithValue("@PhotoSignName", PhotoSignName);
                cmd.Parameters.AddWithValue("@Type", Type);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }
        public DataSet SelectlastEntry_E1_E2_Candidate(string formName, string schl)
        {

            // SqlConnection con = null;

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("GetLastEntry_E1_E2_StudentRecords", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@formName", formName);
                    cmd.Parameters.AddWithValue("@schl", schl);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetStudentRecordsSearch_E(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudentRecordsSearch_E", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetStudentRecordsSearch_ED(string search, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudentRecordsSearch_ED", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SearchStudentGetByData_E(string sid, string frmname)
        {

            // SqlConnection con = null;

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    // SqlCommand cmd = new SqlCommand("GetStudentRecordsByID_E2", con);
                    SqlCommand cmd = new SqlCommand("GetStudentRecordsByID_E", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@sid", sid);
                    cmd.Parameters.AddWithValue("@formName", frmname);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string Update_E_Data(RegistrationModels RM, FormCollection frm, string FormType, string sid, string FilePhoto, string sign)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                //string formName = "E2";
                //string s = frm["CasteSelected"];
                string caste = frm["CasteSelected"].Split(',')[0];
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("Update_E1_E2Forms_Sp", con); //Update_E1_E2Forms_Sp_Test
                cmd.CommandType = CommandType.StoredProcedure;               
                cmd.Parameters.AddWithValue("@IsCorrectionInParticular", RM.IsCorrectionInParticular);
                cmd.Parameters.AddWithValue("@IsNRICandidate", RM.IsNRICandidate);
                cmd.Parameters.AddWithValue("@Std_id", sid);
                cmd.Parameters.AddWithValue("@form_Name", FormType);
                cmd.Parameters.AddWithValue("@Category", RM.Category);
                cmd.Parameters.AddWithValue("@Board", RM.Board);
                cmd.Parameters.AddWithValue("@Other_Board", RM.Other_Board);
                cmd.Parameters.AddWithValue("@Board_Roll_Num", RM.Board_Roll_Num);
                cmd.Parameters.AddWithValue("@Prev_School_Name", RM.Prev_School_Name);
                //cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                cmd.Parameters.AddWithValue("@Month", RM.Month);
                cmd.Parameters.AddWithValue("@Year", RM.Year);
                //cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                //cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);
                cmd.Parameters.AddWithValue("@Admission_Date", RM.Admission_Date);
                cmd.Parameters.AddWithValue("@Class_Roll_Num_Section", RM.Class_Roll_Num_Section);
                cmd.Parameters.AddWithValue("@Candi_Name", RM.Candi_Name);
                cmd.Parameters.AddWithValue("@Candi_Name_P", RM.Candi_Name_P);
                cmd.Parameters.AddWithValue("@Father_Name", RM.Father_Name);
                cmd.Parameters.AddWithValue("@Father_Name_P", RM.Father_Name_P);
                cmd.Parameters.AddWithValue("@Mother_Name", RM.Mother_Name);
                cmd.Parameters.AddWithValue("@Mother_Name_P", RM.Mother_Name_P);
                //cmd.Parameters.AddWithValue("@Caste", RM.Caste);
                cmd.Parameters.AddWithValue("@Caste", caste);
                cmd.Parameters.AddWithValue("@Gender", RM.Gender);
                //cmd.Parameters.AddWithValue("@Differently_Abled", frm["DA"].ToString());
                cmd.Parameters.AddWithValue("@Differently_Abled", RM.Differently_Abled);
                cmd.Parameters.AddWithValue("@Religion", RM.Religion);
                cmd.Parameters.AddWithValue("@DOB", RM.DOB);
                cmd.Parameters.AddWithValue("@Belongs_BPL", RM.Belongs_BPL);
                cmd.Parameters.AddWithValue("@Mobile", RM.Mobile);
                cmd.Parameters.AddWithValue("@Aadhar_num", RM.Aadhar_num);
                cmd.Parameters.AddWithValue("@Group_Name", RM.MyGroup);
                cmd.Parameters.AddWithValue("@E_punjab_Std_id", RM.E_punjab_Std_id);
                cmd.Parameters.AddWithValue("@Address", RM.Address);
                cmd.Parameters.AddWithValue("@LandMark", RM.LandMark);
                cmd.Parameters.AddWithValue("@Block", RM.Block);

                //cmd.Parameters.AddWithValue("@District",RM.District);
                cmd.Parameters.AddWithValue("@District", RM.MyDistrict);
                cmd.Parameters.AddWithValue("@Tehsil", RM.Tehsil);

                if (HttpContext.Current.Session["Session"].ToString() == "2023-2024")
                {
                if (RM.Differently_Abled.ToString() == "N.A.")
                {
                    cmd.Parameters.AddWithValue("@DP", 0);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@DP", RM.DP);
                }
                
                }

                
                if (RM.file == null)
                {
                    cmd.Parameters.AddWithValue("@std_Photo", FilePhoto);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Photo", RM.file.FileName);
                }
                if (RM.std_Sign == null)
                {
                    cmd.Parameters.AddWithValue("@std_Sign", sign);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Sign", RM.std_Sign.FileName);
                }

                //----
                if (RM.IsPrevSchoolSelf.ToString() == "True")
                {
                    //cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", RM.IsPrevSchoolSelf);
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 0);
                }
                if (RM.IsPSEBRegNum.ToString() == "True")
                {
                    cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                    cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                    cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                }
                //------------
                if (FormType == "E1" || FormType == "E2")
                {
                    if (RM.Provisional.ToString() == "True")
                    {

                        cmd.Parameters.AddWithValue("@Provisional", 1);
                        cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                        cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@Provisional", 0);
                        cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                        cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);

                    }
                    if (RM.IsLateAdm.ToString() == "True")
                    {
                        cmd.Parameters.AddWithValue("@RequestID", RM.requestID);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@RequestID", 0);
                    }
                }
                cmd.Parameters.AddWithValue("@PinCode", RM.PinCode);
                cmd.Parameters.AddWithValue("@Section", RM.Section);

                string hostName = Dns.GetHostName(); // Retrive the Name of HOST
                string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                cmd.Parameters.AddWithValue("@MyIP", myIP);
                
                cmd.Parameters.AddWithValue("@MetricMonth", RM.MetricMonth);
                cmd.Parameters.AddWithValue("@MetricYear", RM.MetricYear);
                cmd.Parameters.AddWithValue("@MetricRollNum", RM.Metric_Roll_Num);
                
                if (HttpContext.Current.Session["Session"].ToString() == "2023-2024")
                {
                    cmd.Parameters.AddWithValue("@MatricBoard", RM.MetricBoard);
                    cmd.Parameters.AddWithValue("@MatricResult", RM.MatricResult);
                    cmd.Parameters.AddWithValue("@CandStudyMedium", RM.CandiMedium);

                if (RM.PreNSQF == "" || RM.PreNSQF == null)
                {
                    if (RM.PreNSQF == "NO")
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                    }

                }
                else if (RM.PreNSQF != "" || RM.PreNSQF != null)
                {
                    cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                    cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6);
                    cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQF);

                }
                }

                cmd.Parameters.AddWithValue("@NSQFPattern", RM.NSQFPattern);

                // cmd.Parameters.Add("@StudentUniqueId", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                SqlParameter outPutParameter = new SqlParameter();
                outPutParameter.ParameterName = "@StudentUniqueId";
                outPutParameter.Size = 20;
                outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                outPutParameter.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(outPutParameter);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = (string)cmd.Parameters["@StudentUniqueId"].Value;
                return outuniqueid;


                //con.Open();
                //result = cmd.ExecuteNonQuery().ToString();
                //return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "-1";
            }
            finally
            {
                con.Close();
            }
        }
        public string Delete_E_FromData(string stdid)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("Delete_E1_E2Forms_Sp", con); // 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Std_id", stdid);
                cmd.Parameters.AddWithValue("@MyIP", myIP);

                con.Open();
                result = cmd.ExecuteScalar().ToString();
                return result;

            }
            catch (Exception ex)
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }

        public string Delete_E_FromData(string stdid, string Oldid, string oldyr, string Imptblname)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            try
            {

                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("DeleteFromDataE1Imported_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Std_id", stdid);
                cmd.Parameters.AddWithValue("@Oldid", Oldid);
                cmd.Parameters.AddWithValue("@oldyr", oldyr);
                cmd.Parameters.AddWithValue("@Imptblname", Imptblname);

                cmd.Parameters.AddWithValue("@MyIP", myIP);

                con.Open();
                result = cmd.ExecuteScalar().ToString();
                return result;

            }
            catch (Exception ex)
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }

        //---------------------------End-----------------------
        //--------------------M1 Starts---------------------
        public DataSet CHkNSQFStudents(string NSQFid)
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("CHkNSQFStudents_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", NSQFid);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet CHkNSQF(string SCHLCODE, string ses)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CHkNSQF_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schoolcode", SCHLCODE);
                    cmd.Parameters.AddWithValue("@ses", ses);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }       

        public DataSet ElectiveSubjects()
        {

            // SqlConnection con = null;

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("ElectiveSubjects_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;


                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }





        public DataSet Matric_ElectiveSubjects_Blind_NEW()
        {
            DataSet ds = new DataSet();
            Database db = DatabaseFactory.CreateDatabase("myDBConnection");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Matric_ElectiveSubjects_Blind_NEWSP";
            ds = db.ExecuteDataSet(cmd);
            return ds;
        }




        public DataSet ElectiveSubjects_Blind()
        {

            // SqlConnection con = null;

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("ElectiveSubjects_For_Blind_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;


                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet NSQFSubjects()
        {

            // SqlConnection con = null;

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("NSQFSubjects_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;


                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SelectS1(string S1)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("Getmedium", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@S1", S1);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        //--------------------M1 End-------------------------
        //-------------------------------------Work On M2-Forms---------------------
        public string Ins_M_Form_Data(RegistrationModels RM, FormCollection frm, string FormType, string session, string idno, string schl, DataTable dtMatricSubject)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                // con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("insert_M1_M2_Subject_Forms_Sp", con); //insert_M1_M2_Subject_Forms_Sp , insert_M1_M2_Subject_Forms_Sp_new
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IsSmartPhone", RM.IsSmartPhone);
                cmd.Parameters.AddWithValue("@NSQFPattern", RM.NSQFPattern);
                cmd.Parameters.AddWithValue("@OTID", RM.OTID);
                cmd.Parameters.AddWithValue("@Imptblname", RM.Imptblname);
                cmd.Parameters.AddWithValue("@IsNRICandidate", RM.IsNRICandidate);
                cmd.Parameters.AddWithValue("@form_Name", FormType);
                cmd.Parameters.AddWithValue("@Category", RM.Category);
                cmd.Parameters.AddWithValue("@Board", RM.Board);
                cmd.Parameters.AddWithValue("@Other_Board", RM.Other_Board);
                cmd.Parameters.AddWithValue("@Board_Roll_Num", RM.Board_Roll_Num);
                cmd.Parameters.AddWithValue("@Prev_School_Name", RM.Prev_School_Name);
                //cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                //cmd.Parameters.AddWithValue("@Group_Name", frm["MyGroup"].ToString());
                cmd.Parameters.AddWithValue("@Group_Name", RM.MyGroup);
                if (FormType == "M2")
                {
                    if (RM.IsPSEBRegNum.ToString() == "True")
                    {
                        cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                        cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                    }
                    else
                    {

                        cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                    }
                }
                else
                {
                    if (RM.IsImportedStudent == "1")
                    {
                        cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                        if (RM.IsPSEBRegNum.ToString() == "True")
                        {
                            //cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                            cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                        }
                    }
                    else
                    {
                        RM.IsRegNoExists = "Y";
                        if (RM.IsRegNoExists.ToString() == "Y")
                        {
                            cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                            cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Registration_num", "");
                            cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                        }
                    }
                }

                cmd.Parameters.AddWithValue("@Month", RM.Month);
                cmd.Parameters.AddWithValue("@Year", RM.Year);
                //cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                //cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);
                cmd.Parameters.AddWithValue("@Admission_Date", RM.Admission_Date);
                cmd.Parameters.AddWithValue("@Class_Roll_Num_Section", RM.Class_Roll_Num_Section);

                cmd.Parameters.AddWithValue("@Candi_Name", RM.Candi_Name);
                cmd.Parameters.AddWithValue("@Candi_Name_P", RM.Candi_Name_P);
                cmd.Parameters.AddWithValue("@Father_Name", RM.Father_Name);
                cmd.Parameters.AddWithValue("@Father_Name_P", RM.Father_Name_P);
                cmd.Parameters.AddWithValue("@Mother_Name", RM.Mother_Name);
                cmd.Parameters.AddWithValue("@Mother_Name_P", RM.Mother_Name_P);
                cmd.Parameters.AddWithValue("@Caste", RM.Caste);
                cmd.Parameters.AddWithValue("@Gender", RM.Gender);
                cmd.Parameters.AddWithValue("@Differently_Abled", frm["DA"].ToString());
                cmd.Parameters.AddWithValue("@Religion", RM.Religion);
                cmd.Parameters.AddWithValue("@DOB", RM.DOB);
                cmd.Parameters.AddWithValue("@Belongs_BPL", RM.Belongs_BPL);
                cmd.Parameters.AddWithValue("@Mobile", RM.Mobile);
                cmd.Parameters.AddWithValue("@Aadhar_num", RM.Aadhar_num);
                cmd.Parameters.AddWithValue("@E_punjab_Std_id", RM.E_punjab_Std_id);
                cmd.Parameters.AddWithValue("@Address", RM.Address);
                cmd.Parameters.AddWithValue("@LandMark", RM.LandMark);
                cmd.Parameters.AddWithValue("@Block", RM.Block);
                cmd.Parameters.AddWithValue("@Tehsil", frm["MyTeh"].ToString());
                cmd.Parameters.AddWithValue("@District", frm["MyDist"].ToString());
                cmd.Parameters.AddWithValue("@PinCode", RM.PinCode);

                if (RM.Provisional.ToString() == "True")
                {

                    cmd.Parameters.AddWithValue("@Provisional", 1);
                    //cmd.Parameters.AddWithValue("@AWRegisterNo", NULL);
                    //cmd.Parameters.AddWithValue("@Admission_Num", NULL);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Provisional", 0);
                    cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                    cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);

                }
                if (RM.IsLateAdm.ToString() == "True")
                {
                    cmd.Parameters.AddWithValue("@RequestID", RM.requestID);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@RequestID", 0);
                }
                if (RM.IsPrevSchoolSelf.ToString() == "True")
                {
                    //cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", RM.IsPrevSchoolSelf);
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 0);
                }

                if (RM.file != null)
                {
                    cmd.Parameters.AddWithValue("@std_Photo", RM.file.FileName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Photo", null);
                }
                if (RM.std_Sign != null)
                {
                    cmd.Parameters.AddWithValue("@std_Sign", RM.std_Sign.FileName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Sign", null);
                }



                if (frm["DA"].ToString() == "N.A.")
                {
                    cmd.Parameters.AddWithValue("@ScribeWriter", "NO");

                    if (RM.PreNSQF == "" || RM.PreNSQF == null)
                    {
                        if (RM.PreNSQF == "NO")
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                        }

                    }
                    else if (RM.PreNSQF != "" || RM.PreNSQF != null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQF);

                    }


                   
                    
                }
                else
                {
                    //cmd.Parameters.AddWithValue("@NSQF", 0);
                    cmd.Parameters.AddWithValue("@ScribeWriter", RM.scribeWriter);

                    if (RM.PreNSQF == "" || RM.PreNSQF == null)
                    {
                        if (RM.PreNSQF == "NO")
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                        }

                    }
                    else if (RM.PreNSQF != "" || RM.PreNSQF != null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQF);

                    }
                }
                //----------------------End SubDetails-----------------
                if (frm["DA"].ToString() == "N.A.")
                {
                    cmd.Parameters.AddWithValue("@DP", 0);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@DP", RM.DP);
                }

                cmd.Parameters.AddWithValue("@MatricSubjects", dtMatricSubject);
                cmd.Parameters.AddWithValue("@Section", RM.Section);

                cmd.Parameters.AddWithValue("@SCHL", schl);
                //cmd.Parameters.AddWithValue("@IDNO", idno);
                cmd.Parameters.AddWithValue("@SESSION", session);
                cmd.Parameters.AddWithValue("@CandStudyMedium", RM.CandiMedium);

                // cmd.Parameters.Add("@StudentUniqueId", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                SqlParameter outPutParameter = new SqlParameter();
                outPutParameter.ParameterName = "@StudentUniqueId";
                outPutParameter.Size = 20;
                outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                outPutParameter.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(outPutParameter);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = (string)cmd.Parameters["@StudentUniqueId"].Value;
                return outuniqueid;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "-1";
            }
            finally
            {
                con.Close();
            }
        }

        public DataSet GetNAmeAndAbbrbySub(string SUB)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetNAmeAndAbbrbySubSP", con);// GetNAmeAndAbbrbySubSP
                    cmd.CommandType = CommandType.StoredProcedure;                    
                    cmd.Parameters.AddWithValue("@SUB", SUB);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        
        public DataSet SearchStudentGetByData_Subject(string sid, string frmname)
        {

            // SqlConnection con = null;

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    // SqlCommand cmd = new SqlCommand("GetStudentRecordsByID_E2", con);
                    SqlCommand cmd = new SqlCommand("GetStudentRecordsByID_SUBJECT_Rohit", con);// GetStudentRecordsByID_SUBJECT
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@sid", sid);
                    cmd.Parameters.AddWithValue("@formName", frmname);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string Update_M_Data(RegistrationModels RM, FormCollection frm, string FormType, string sid, string FilePhoto, string sign,DataTable dtMatricSubject)
        {
            SqlConnection con = null;
            string result = "";
            try
            {               
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_M1_M2_SubjectsForms_Sp_Main", con); //Update_M1_M2_SubjectsForms_Sp_Rohit
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IsSmartPhone", RM.IsSmartPhone);
                cmd.Parameters.AddWithValue("@NSQFPattern", RM.NSQFPattern);
                cmd.Parameters.AddWithValue("@IsCorrectionInParticular", RM.IsCorrectionInParticular);
                cmd.Parameters.AddWithValue("@IsNRICandidate", RM.IsNRICandidate);

                cmd.Parameters.AddWithValue("@Std_id", sid);
                cmd.Parameters.AddWithValue("@form_Name", FormType);
                cmd.Parameters.AddWithValue("@Category", RM.Category);
                cmd.Parameters.AddWithValue("@Board", RM.Board);
                cmd.Parameters.AddWithValue("@Other_Board", RM.Other_Board);
                cmd.Parameters.AddWithValue("@Board_Roll_Num", RM.Board_Roll_Num);
                cmd.Parameters.AddWithValue("@Prev_School_Name", RM.Prev_School_Name);            
                cmd.Parameters.AddWithValue("@Month", RM.Month);
                cmd.Parameters.AddWithValue("@Year", RM.Year);
       
                cmd.Parameters.AddWithValue("@Admission_Date", RM.Admission_Date);
                cmd.Parameters.AddWithValue("@Class_Roll_Num_Section", RM.Class_Roll_Num_Section);
                cmd.Parameters.AddWithValue("@Candi_Name", RM.Candi_Name);
                cmd.Parameters.AddWithValue("@Candi_Name_P", RM.Candi_Name_P);
                cmd.Parameters.AddWithValue("@Father_Name", RM.Father_Name);
                cmd.Parameters.AddWithValue("@Father_Name_P", RM.Father_Name_P);
                cmd.Parameters.AddWithValue("@Mother_Name", RM.Mother_Name);
                cmd.Parameters.AddWithValue("@Mother_Name_P", RM.Mother_Name_P);
                cmd.Parameters.AddWithValue("@Caste", RM.Caste);
                cmd.Parameters.AddWithValue("@Gender", RM.Gender);              
                cmd.Parameters.AddWithValue("@Differently_Abled", RM.DA);
                cmd.Parameters.AddWithValue("@Religion", RM.Religion);
                cmd.Parameters.AddWithValue("@DOB", RM.DOB);
                cmd.Parameters.AddWithValue("@Belongs_BPL", RM.Belongs_BPL);
                cmd.Parameters.AddWithValue("@Mobile", RM.Mobile);
                cmd.Parameters.AddWithValue("@Father_MobNo", RM.Father_MobNo);
                cmd.Parameters.AddWithValue("@Father_Occup", RM.Father_Occup);
                cmd.Parameters.AddWithValue("@Mother_MobNo", RM.Mother_MobNo);
                cmd.Parameters.AddWithValue("@Mother_Occup", RM.Mother_Occup);
                cmd.Parameters.AddWithValue("@Aadhar_num", RM.Aadhar_num);
                cmd.Parameters.AddWithValue("@Group_Name", RM.MyGroup);
                cmd.Parameters.AddWithValue("@E_punjab_Std_id", RM.E_punjab_Std_id);
                cmd.Parameters.AddWithValue("@Address", RM.Address);
                cmd.Parameters.AddWithValue("@LandMark", RM.LandMark);
                cmd.Parameters.AddWithValue("@Block", RM.Block);
                cmd.Parameters.AddWithValue("@District", RM.MyDistrict);
                cmd.Parameters.AddWithValue("@Tehsil", RM.Tehsil);
                cmd.Parameters.AddWithValue("@PinCode", RM.PinCode);
                if (RM.file == null)
                {
                    cmd.Parameters.AddWithValue("@std_Photo", FilePhoto);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Photo", RM.file.FileName);
                }
                if (RM.std_Sign == null)
                {
                    cmd.Parameters.AddWithValue("@std_Sign", sign);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Sign", RM.std_Sign.FileName);
                }

                //----
                if (RM.IsPrevSchoolSelf.ToString() == "True")
                {
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 0);
                }
             
                if (FormType == "M2")
                {
                    cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                    if (RM.IsPSEBRegNum.ToString() == "True")                    {
                       
                        cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                    }
                    else
                    {                       
                        cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                    }
                }
                else
                {
                    if (RM.IsImportedStudent == "1")
                    {
                        cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                        if (RM.IsPSEBRegNum.ToString() == "True")
                        {
                            //cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                            cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                        }
                    }
                    else
                    {
                        RM.IsRegNoExists = "Y";
                        if (RM.IsRegNoExists.ToString() == "Y")
                        {
                            cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                            cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Registration_num", "");
                            cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                        }
                    }
                }

                if (RM.Provisional.ToString() == "True")
                {

                    cmd.Parameters.AddWithValue("@Provisional", 1);
                    cmd.Parameters.AddWithValue("@AWRegisterNo", null);
                    cmd.Parameters.AddWithValue("@Admission_Num", null);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Provisional", 0);
                    cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                    cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);

                }
                if (RM.IsLateAdm.ToString() == "True")
                {
                    cmd.Parameters.AddWithValue("@RequestID", RM.requestID);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@RequestID", 0);
                }
               
                if (frm["DA"].ToString() == "N.A.")
                {
                    cmd.Parameters.AddWithValue("@ScribeWriter", "NO");

                    if (RM.NsqfsubS6 == "" || RM.NsqfsubS6 == null)
                    {
                        if (RM.PreNSQF == "NO")
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@DP", 0);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@DP", 0);
                        }

                    }
                    else 
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQF);
                        cmd.Parameters.AddWithValue("@DP", 0);
                    }
                  
                }
                else
                {
                    cmd.Parameters.AddWithValue("@DP", RM.DP);
                    //cmd.Parameters.AddWithValue("@NSQF", 0);
                    cmd.Parameters.AddWithValue("@ScribeWriter", RM.scribeWriter);

                    if (RM.NsqfsubS6 == "" || RM.NsqfsubS6 == null)
                    {
                        if (RM.PreNSQF == "NO")
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");                            
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);                           
                        }

                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQF);                      
                    }
                }

                //----------------------End SubDetails-----------------
                cmd.Parameters.AddWithValue("@MatricSubjects", dtMatricSubject);
                cmd.Parameters.AddWithValue("@Section", RM.Section);
                cmd.Parameters.AddWithValue("@CandStudyMedium", RM.CandiMedium);        

                string myIP = AbstractLayer.StaticDB.GetFullIPAddress();
                cmd.Parameters.AddWithValue("@MyIP", myIP);

                cmd.Parameters.Add("@StudentUniqueId", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = (string)cmd.Parameters["@StudentUniqueId"].Value;
                string OutError = (string)cmd.Parameters["@OutError"].Value;
                return outuniqueid;
            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "-5";
            }
            finally
            {
                con.Close();
            }
        }
        public string Delete_M_FromData(string stdid)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Delete_M1_M2Forms_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Std_id", stdid);
                cmd.Parameters.AddWithValue("@MyIP", myIP);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

            }
            catch (Exception ex)
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }
        ///////////////------------------------------M2  End -----------------------------
      

        public DataTable GetEditSchoolStaffDetails(int id)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetEditSchoolStaffDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    // int count = Convert.ToInt32(cmd.Parameters["@PageCount"].Value);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }


        //public void pro_insertupdateschool_staff_details(SchoolStaffDetailsModel obj, out string outputstatus, out string getid)
        //{
        //    SqlConnection con = null;
        //    string result = "";
        //    try
        //    {


        //        con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
        //        SqlCommand cmd = new SqlCommand("pro_insertupdateschool_staff_details", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        if (obj.id != 0)
        //        {
        //            cmd.Parameters.AddWithValue("@id", obj.id);
        //            cmd.Parameters.AddWithValue("@upd_date", DateTime.Now.ToString());
        //        }
        //        cmd.Parameters.AddWithValue("@schoolcode", obj.schoolcode);
        //        cmd.Parameters.AddWithValue("@Name", obj.Name);
        //        cmd.Parameters.AddWithValue("@FatherName", obj.FName);
        //        cmd.Parameters.AddWithValue("@DOB", obj.DOB);
        //        cmd.Parameters.AddWithValue("@Gender", obj.Gender);
        //        cmd.Parameters.AddWithValue("@AadharNo", obj.AadharNo);
        //        cmd.Parameters.AddWithValue("@MobileNo", obj.MobileNo);
        //        cmd.Parameters.AddWithValue("@StdCode", obj.stdCode);
        //        cmd.Parameters.AddWithValue("@PhoneNo", obj.PhoneNo);
        //        cmd.Parameters.AddWithValue("@Email", obj.Email);
        //        cmd.Parameters.AddWithValue("@DateOfAppointment", obj.appointmentDate);
        //        cmd.Parameters.AddWithValue("@DateOfJoining", obj.joiningDate);
        //        cmd.Parameters.AddWithValue("@Cadreid", obj.Cadreid);
        //        cmd.Parameters.AddWithValue("@Subjectid", obj.Subjectid);
        //        cmd.Parameters.AddWithValue("@HouseFlatNo", obj.HouseFlatNo);
        //        cmd.Parameters.AddWithValue("@VillWardCity", obj.VillWardCity);
        //        cmd.Parameters.AddWithValue("@LandMark", obj.LandMark);
        //        cmd.Parameters.AddWithValue("@districtid", obj.DistrictId);
        //        cmd.Parameters.AddWithValue("@statename", obj.State);
        //        cmd.Parameters.AddWithValue("@DistanceFromSchool", obj.DistanceFromSchool);
        //        cmd.Parameters.AddWithValue("@PinCode", obj.PinCode);
        //        cmd.Parameters.AddWithValue("@staffstatus", "1");
        //        //cmd.Parameters.AddWithValue("@photo", obj.photo);
        //        cmd.Parameters.AddWithValue("@otherdistrict", obj.otherdistrict);
        //        cmd.Parameters.AddWithValue("@otherstate", obj.otherstate);
        //        cmd.Parameters.Add("@OutId", SqlDbType.Int).Direction = ParameterDirection.Output;
        //        cmd.Parameters.Add("@GetId", SqlDbType.Int).Direction = ParameterDirection.Output;
        //        con.Open();
        //        result = cmd.ExecuteNonQuery().ToString();
        //        outputstatus = Convert.ToString(cmd.Parameters["@OutId"].Value);
        //        getid = Convert.ToString(cmd.Parameters["@GetId"].Value);


        //    }
        //    catch (Exception ex)
        //    {
        //        //mbox(ex);
        //        outputstatus = "";
        //        getid = "";
        //    }
        //    finally
        //    {
        //        con.Close();
        //    }
        //}

        public void pro_insertupdateschool_staff_details(SchoolStaffDetailsModel obj, out string outputstatus, out string getid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {


                // con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("pro_insertupdateschool_staff_details_New", con);
                cmd.CommandType = CommandType.StoredProcedure;
                if (obj.id != 0)
                {
                    cmd.Parameters.AddWithValue("@id", obj.id);
                  //  cmd.Parameters.AddWithValue("@upd_date", DateTime.Now.ToString());
                }
                cmd.Parameters.AddWithValue("@schoolcode", obj.schoolcode);
                cmd.Parameters.AddWithValue("@Name", obj.Name);
                cmd.Parameters.AddWithValue("@FatherName", obj.FName);
                cmd.Parameters.AddWithValue("@DOB", obj.DOB);
                cmd.Parameters.AddWithValue("@Gender", obj.Gender);
                cmd.Parameters.AddWithValue("@AadharNo", obj.AadharNo);
                cmd.Parameters.AddWithValue("@MobileNo", obj.MobileNo);
                cmd.Parameters.AddWithValue("@StdCode", obj.stdCode);
                cmd.Parameters.AddWithValue("@PhoneNo", obj.PhoneNo);
                cmd.Parameters.AddWithValue("@Email", obj.Email);
                cmd.Parameters.AddWithValue("@DateOfAppointment", obj.appointmentDate);
                cmd.Parameters.AddWithValue("@DateOfJoining", obj.joiningDate);


                cmd.Parameters.AddWithValue("@ExpMonth", obj.ExpMonth);
                cmd.Parameters.AddWithValue("@ExpYear", obj.ExpYear);

                cmd.Parameters.AddWithValue("@Cadreid", obj.Cadreid);
                cmd.Parameters.AddWithValue("@Subjectid", obj.Subjectid);
                cmd.Parameters.AddWithValue("@HouseFlatNo", obj.HouseFlatNo);
                cmd.Parameters.AddWithValue("@VillWardCity", obj.VillWardCity);
                cmd.Parameters.AddWithValue("@LandMark", obj.LandMark);
                cmd.Parameters.AddWithValue("@districtid", obj.DistrictId);
                cmd.Parameters.AddWithValue("@statename", obj.State);
                cmd.Parameters.AddWithValue("@DistanceFromSchool", obj.DistanceFromSchool);
                cmd.Parameters.AddWithValue("@PinCode", obj.PinCode);
                cmd.Parameters.AddWithValue("@staffstatus", "1");
                cmd.Parameters.AddWithValue("@photo", obj.photo);
                cmd.Parameters.AddWithValue("@otherdistrict", obj.otherdistrict);
                cmd.Parameters.AddWithValue("@otherstate", obj.otherstate);

                //-------------
                cmd.Parameters.AddWithValue("@Quali", obj.Quali);
                cmd.Parameters.AddWithValue("@Phychal", obj.Phychall);
                cmd.Parameters.AddWithValue("@tehsil", obj.tehsil);
                cmd.Parameters.AddWithValue("@EDUBLOCK", obj.Edublock);
                cmd.Parameters.AddWithValue("@EDUCLUSTER", obj.EduCluster);
                cmd.Parameters.AddWithValue("@SCHLTYPE", obj.SchlType);
                cmd.Parameters.AddWithValue("@SCHlESTD", obj.SchlEstd);
                cmd.Parameters.AddWithValue("@BANKAC", obj.Bank);
                cmd.Parameters.AddWithValue("@IFSC", obj.IFSC);
                cmd.Parameters.AddWithValue("@GEOLOC", obj.geoloc);

                //-----------------


                cmd.Parameters.Add("@OutId", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@GetId", SqlDbType.Int).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                outputstatus = Convert.ToString(cmd.Parameters["@OutId"].Value);
                getid = Convert.ToString(cmd.Parameters["@GetId"].Value);


            }
            catch (Exception ex)
            {
                //mbox(ex);
                outputstatus = "";
                getid = "";
            }
            finally
            {
                con.Close();
            }
        }
        public string Updated_Pic_Data(string Myresult, string PhotoName)
        {
            SqlConnection con = null;
            string result = "";
            try
            {

                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_Uploaded_Photo_SchoolStaff", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", Myresult);
                cmd.Parameters.AddWithValue("@Photoname", PhotoName);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }


        public string deleteRecordSchoolStaffDetails(int id)
        {
            SqlConnection con = null;
            string result = "";
            try
            {


                // con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("deleterecordstaffdetailsById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ccfid", id);
                cmd.Parameters.Add("@OutId", SqlDbType.Int).Direction = ParameterDirection.Output;

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = Convert.ToString(cmd.Parameters["@OutId"].Value);
                return outuniqueid;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }


        public DataSet GetStaffRecordsSearch(string search, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStaffRecordsSearch", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 50);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    // int count = Convert.ToInt32(cmd.Parameters["@PageCount"].Value);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet GetStaffRecordsCount(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStaffRecordsCount", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    // int count = Convert.ToInt32(cmd.Parameters["@PageCount"].Value);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet SelectNSQFTEXT(string SUBCODE)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectNSQFTEXT_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@subNSQF", SUBCODE);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SelectS11(string S11)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetS11NSFQSubjects_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet SelectS12(string S12)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetS12NSFQSubjects_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SelectS9(string S9)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {                  
                    SqlCommand cmd = new SqlCommand("GetS9NSFQSubjects", con);
                    cmd.CommandType = CommandType.StoredProcedure;                  
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
         }
        public DataSet SelectMatricNsqfSub(string S9)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectMatricNsqfSub_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetNSQFVIEWSUBJECTMATRICSUBJECT(string subNSQF, string preNSQF)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetNSQFVIEWSUBJECTMATRICSUBJECT_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@subNSQF", subNSQF);
                    cmd.Parameters.AddWithValue("@preNSQF", preNSQF);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetNSQFVIEWSUBJECT11TH(string subNSQF, string preNSQF)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetNSQFVIEWSUBJECT11TH_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@subNSQF", subNSQF);
                    cmd.Parameters.AddWithValue("@preNSQF", preNSQF);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetNSQFVIEWSUBJECT(string subNSQF, string preNSQF)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetNSQFVIEWSUBJECT_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@subNSQF", subNSQF);
                    cmd.Parameters.AddWithValue("@preNSQF", preNSQF);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public List<SelectListItem> GetMatricMediumList(DataSet dssetM)
        {                  
            List<SelectListItem> itemMList = new List<SelectListItem>();
            itemMList.Add(new SelectListItem { Text = "medium", Value = "0" });
            foreach (System.Data.DataRow dr in dssetM.Tables[0].Rows)
            {
                if (@dr["medium2"].ToString() == "" || @dr["medium2"].ToString() == null)
                {
                    itemMList.Add(new SelectListItem { Text = @dr["medium1"].ToString(), Value = @dr["medium1"].ToString() });

                }
                else if (@dr["medium3"].ToString() == "" || @dr["medium3"].ToString() == null)
                {
                    itemMList.Add(new SelectListItem { Text = @dr["medium1"].ToString(), Value = @dr["medium1"].ToString() });
                    itemMList.Add(new SelectListItem { Text = @dr["medium2"].ToString(), Value = @dr["medium2"].ToString() });
                }
                else
                {
                    itemMList.Add(new SelectListItem { Text = @dr["medium1"].ToString(), Value = @dr["medium1"].ToString() });
                    itemMList.Add(new SelectListItem { Text = @dr["medium2"].ToString(), Value = @dr["medium2"].ToString() });
                    itemMList.Add(new SelectListItem { Text = @dr["medium3"].ToString(), Value = @dr["medium3"].ToString() });
                }
            }
            return itemMList;
        }


        //////////////---------------------------T2 start-----------------------------

        public string Ins_T_Form_Data(RegistrationModels RM, FormCollection frm, string FormType, string session, string idno, string schl, DataTable dtMatricSubject)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("insert_T1_T2_Subject_Forms_Sp", con); //insert_M1_M2_Subject_Forms_Sp , insert_M1_M2_Subject_Forms_Sp_new
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IsSmartPhone", RM.IsSmartPhone);
                cmd.Parameters.AddWithValue("@NSQFPattern", RM.NSQFPattern);
                cmd.Parameters.AddWithValue("@OTID", RM.OTID);
                cmd.Parameters.AddWithValue("@Imptblname", RM.Imptblname);
                cmd.Parameters.AddWithValue("@IsNRICandidate", RM.IsNRICandidate);
                cmd.Parameters.AddWithValue("@form_Name", FormType);
                cmd.Parameters.AddWithValue("@Category", RM.Category);
                cmd.Parameters.AddWithValue("@Board", RM.Board);
                cmd.Parameters.AddWithValue("@Other_Board", RM.Other_Board);
                cmd.Parameters.AddWithValue("@Board_Roll_Num", RM.Board_Roll_Num);
                cmd.Parameters.AddWithValue("@Prev_School_Name", RM.Prev_School_Name);
                //cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                //cmd.Parameters.AddWithValue("@Group_Name", frm["MyGroup"].ToString());
                cmd.Parameters.AddWithValue("@Group_Name", RM.Tgroup);
                if (FormType == "T2")
                {
                    if (RM.IsPSEBRegNum.ToString() == "True")
                    {
                        cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                        cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                    }
                    else
                    {

                        cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                    }
                }
                else
                {
                    if (RM.IsImportedStudent == "1")
                    {
                        cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                        if (RM.IsPSEBRegNum.ToString() == "True")
                        {
                            //cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                            cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                        }
                    }
                    else
                    {
                        RM.IsRegNoExists = "Y";
                        if (RM.IsRegNoExists.ToString() == "Y")
                        {
                            cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                            cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Registration_num", "");
                            cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                        }
                    }

                }

                cmd.Parameters.AddWithValue("@Month", RM.Month);
                cmd.Parameters.AddWithValue("@Year", RM.Year);

                cmd.Parameters.AddWithValue("@Admission_Date", RM.Admission_Date);
                cmd.Parameters.AddWithValue("@Class_Roll_Num_Section", RM.Class_Roll_Num_Section);

                cmd.Parameters.AddWithValue("@Candi_Name", RM.Candi_Name);
                cmd.Parameters.AddWithValue("@Candi_Name_P", RM.Candi_Name_P);
                cmd.Parameters.AddWithValue("@Father_Name", RM.Father_Name);
                cmd.Parameters.AddWithValue("@Father_Name_P", RM.Father_Name_P);
                cmd.Parameters.AddWithValue("@Mother_Name", RM.Mother_Name);
                cmd.Parameters.AddWithValue("@Mother_Name_P", RM.Mother_Name_P);
                cmd.Parameters.AddWithValue("@Caste", RM.Caste);
                cmd.Parameters.AddWithValue("@Gender", RM.Gender);
                cmd.Parameters.AddWithValue("@Differently_Abled", frm["DA"].ToString());
                cmd.Parameters.AddWithValue("@Religion", RM.Religion);
                cmd.Parameters.AddWithValue("@DOB", RM.DOB);
                cmd.Parameters.AddWithValue("@Belongs_BPL", RM.Belongs_BPL);
                cmd.Parameters.AddWithValue("@Mobile", RM.Mobile);
                cmd.Parameters.AddWithValue("@Aadhar_num", RM.Aadhar_num);
                cmd.Parameters.AddWithValue("@E_punjab_Std_id", RM.E_punjab_Std_id);
                cmd.Parameters.AddWithValue("@Address", RM.Address);
                cmd.Parameters.AddWithValue("@LandMark", RM.LandMark);
                cmd.Parameters.AddWithValue("@Block", RM.Block);
                cmd.Parameters.AddWithValue("@Tehsil", frm["MyTeh"].ToString());
                cmd.Parameters.AddWithValue("@District", frm["MyDist"].ToString());
                cmd.Parameters.AddWithValue("@PinCode", RM.PinCode);

                if (RM.Provisional.ToString() == "True")
                {

                    cmd.Parameters.AddWithValue("@Provisional", 1);

                }
                else
                {
                    cmd.Parameters.AddWithValue("@Provisional", 0);
                    cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                    cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);

                }
                if (RM.IsLateAdm.ToString() == "True")
                {
                    cmd.Parameters.AddWithValue("@RequestID", RM.requestID);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@RequestID", 0);
                }
                if (RM.IsPrevSchoolSelf.ToString() == "True")
                {
                    //cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", RM.IsPrevSchoolSelf);
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 0);
                }

                if (RM.file != null)
                {
                    cmd.Parameters.AddWithValue("@std_Photo", RM.file.FileName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Photo", null);
                }
                if (RM.std_Sign != null)
                {
                    cmd.Parameters.AddWithValue("@std_Sign", RM.std_Sign.FileName);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Sign", null);
                }

                if (frm["DA"].ToString() == "N.A.")
                {
                    cmd.Parameters.AddWithValue("@DP", 0);
                    cmd.Parameters.AddWithValue("@ScribeWriter", "NO");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@DP", RM.DP);
                cmd.Parameters.AddWithValue("@ScribeWriter", RM.scribeWriter);
                }

                
                cmd.Parameters.AddWithValue("@MatricSubjects", dtMatricSubject);


                //----------------------End SubDetails-----------------
                cmd.Parameters.AddWithValue("@Section", RM.Section);

                cmd.Parameters.AddWithValue("@SCHL", schl);
                //cmd.Parameters.AddWithValue("@IDNO", idno);
                cmd.Parameters.AddWithValue("@SESSION", session);

                cmd.Parameters.AddWithValue("@MetricMonth", RM.MetricMonth);
                cmd.Parameters.AddWithValue("@MetricYear", RM.MetricYear);
                cmd.Parameters.AddWithValue("@MetricRollNum", RM.Metric_Roll_Num);
                cmd.Parameters.AddWithValue("@MatricBoard", RM.MetricBoard);
                cmd.Parameters.AddWithValue("@CandStudyMedium", RM.CandiMedium);
                cmd.Parameters.AddWithValue("@MatricResult", RM.MatricResult);

                if (RM.Tgroup == "HUMANITIES" || RM.Tgroup == "SCIENCE" || RM.Tgroup == "COMMERCE" || RM.Tgroup == "VOCATIONAL")
                {
                    if (RM.PreNSQF == "" || RM.PreNSQF == null)
                    {
                        if (RM.PreNSQF == "NO")
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                        }

                    }
                    else if (RM.PreNSQF != "" || RM.PreNSQF != null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQF);

                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                    cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                    cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                }
                
                // cmd.Parameters.Add("@StudentUniqueId", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                SqlParameter outPutParameter = new SqlParameter();
                outPutParameter.ParameterName = "@StudentUniqueId";
                outPutParameter.Size = 20;
                outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                outPutParameter.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(outPutParameter);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = (string)cmd.Parameters["@StudentUniqueId"].Value;
                return outuniqueid;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "-1";
            }
            finally
            {
                con.Close();
            }
        }
        public string Update_T_Data(RegistrationModels RM, FormCollection frm, string FormType, string sid, string FilePhoto, string sign, DataTable dtMatricSubject)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                //string formName = "T2";
               // RM.Caste = frm["CasteSelected"];
                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_T1_T2_SubjectsForms_Sp", con); //Update_M1_M2_SubjectsForms_Sp //Update_M1_M2_SubjectsForms_Sp_Rohit_Test
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IsSmartPhone", RM.IsSmartPhone);
                cmd.Parameters.AddWithValue("@NSQFPattern", RM.NSQFPattern);
                cmd.Parameters.AddWithValue("@IsCorrectionInParticular", RM.IsCorrectionInParticular);
                cmd.Parameters.AddWithValue("@IsNRICandidate", RM.IsNRICandidate);
                cmd.Parameters.AddWithValue("@Std_id", sid);
                cmd.Parameters.AddWithValue("@form_Name", FormType);
                cmd.Parameters.AddWithValue("@Category", RM.Category);
                cmd.Parameters.AddWithValue("@Board", RM.Board);
                cmd.Parameters.AddWithValue("@Other_Board", RM.Other_Board);
                cmd.Parameters.AddWithValue("@Board_Roll_Num", RM.Board_Roll_Num);
                cmd.Parameters.AddWithValue("@Prev_School_Name", RM.Prev_School_Name);
               // cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                cmd.Parameters.AddWithValue("@Month", RM.Month);
                cmd.Parameters.AddWithValue("@Year", RM.Year);
                //cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                //cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);
                cmd.Parameters.AddWithValue("@Admission_Date", RM.Admission_Date);
                cmd.Parameters.AddWithValue("@Class_Roll_Num_Section", RM.Class_Roll_Num_Section);
                cmd.Parameters.AddWithValue("@Candi_Name", RM.Candi_Name);
                cmd.Parameters.AddWithValue("@Candi_Name_P", RM.Candi_Name_P);
                cmd.Parameters.AddWithValue("@Father_Name", RM.Father_Name);
                cmd.Parameters.AddWithValue("@Father_Name_P", RM.Father_Name_P);
                cmd.Parameters.AddWithValue("@Mother_Name", RM.Mother_Name);
                cmd.Parameters.AddWithValue("@Mother_Name_P", RM.Mother_Name_P);
                cmd.Parameters.AddWithValue("@Caste", RM.Caste);
                cmd.Parameters.AddWithValue("@Gender", RM.Gender);
                //cmd.Parameters.AddWithValue("@Differently_Abled", frm["DA"].ToString());
                cmd.Parameters.AddWithValue("@Differently_Abled", RM.DA);
                cmd.Parameters.AddWithValue("@Religion", RM.Religion);
                cmd.Parameters.AddWithValue("@DOB", RM.DOB);
                cmd.Parameters.AddWithValue("@Belongs_BPL", RM.Belongs_BPL);
                cmd.Parameters.AddWithValue("@Mobile", RM.Mobile);
                cmd.Parameters.AddWithValue("@Father_MobNo", RM.Father_MobNo);
                cmd.Parameters.AddWithValue("@Father_Occup", RM.Father_Occup);
                cmd.Parameters.AddWithValue("@Mother_MobNo", RM.Mother_MobNo);
                cmd.Parameters.AddWithValue("@Mother_Occup", RM.Mother_Occup);

                cmd.Parameters.AddWithValue("@Aadhar_num", RM.Aadhar_num);
                cmd.Parameters.AddWithValue("@Group_Name", RM.Tgroup);
                cmd.Parameters.AddWithValue("@E_punjab_Std_id", RM.E_punjab_Std_id);
                cmd.Parameters.AddWithValue("@Address", RM.Address);
                cmd.Parameters.AddWithValue("@LandMark", RM.LandMark);
                cmd.Parameters.AddWithValue("@Block", RM.Block);

                //cmd.Parameters.AddWithValue("@District",RM.District);
                cmd.Parameters.AddWithValue("@District", RM.MyDistrict);
                cmd.Parameters.AddWithValue("@Tehsil", RM.Tehsil);



                cmd.Parameters.AddWithValue("@PinCode", RM.PinCode);
                if (RM.file == null)
                {
                    cmd.Parameters.AddWithValue("@std_Photo", FilePhoto);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Photo", RM.file.FileName);
                }
                if (RM.std_Sign == null)
                {
                    cmd.Parameters.AddWithValue("@std_Sign", sign);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Sign", RM.std_Sign.FileName);
                }

                //----
                if (RM.IsPrevSchoolSelf.ToString() == "True")
                {
                    //cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", RM.IsPrevSchoolSelf);
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 0);
                }
                //if (RM.IsPSEBRegNum.ToString() == "True")
                //{
                //    //cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                //    cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                //}
                //else
                //{
                //    cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                //}

                if (FormType == "T2")
                {
                    cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                    if (RM.IsPSEBRegNum.ToString() == "True")
                    {
                        
                        cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                    }
                    else
                    {

                        cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                    }
                }
                else
                {
                    if (RM.IsImportedStudent == "1")
                    {
                        cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                        if (RM.IsPSEBRegNum.ToString() == "True")
                        {
                            //cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                            cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                        }
                    }
                    else
                    {
                        // RM.IsRegNoExists = string.IsNullOrEmpty(RM.IsRegNoExists) ? "N" : RM.IsRegNoExists;
                        RM.IsRegNoExists = "Y";
                        if (RM.IsRegNoExists.ToString() == "Y")
                        {
                            cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                            cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Registration_num", "");
                            cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                        }
                    }
                }

                if (RM.Provisional.ToString() == "True")
                {

                    cmd.Parameters.AddWithValue("@Provisional", 1);
                    //cmd.Parameters.AddWithValue("@AWRegisterNo", NULL);
                    //cmd.Parameters.AddWithValue("@Admission_Num", NULL);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Provisional", 0);
                    cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                    cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);

                }
                if (RM.IsLateAdm.ToString() == "True")
                {
                    cmd.Parameters.AddWithValue("@RequestID", RM.requestID);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@RequestID", 0);
                }
                if (frm["DA"].ToString() == "N.A.")
                {
                    cmd.Parameters.AddWithValue("@DP", 0);
                    cmd.Parameters.AddWithValue("@ScribeWriter", "NO");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@DP", RM.DP);
                cmd.Parameters.AddWithValue("@ScribeWriter", RM.scribeWriter);
                }
                //cmd.Parameters.AddWithValue("@ScribeWriter", RM.scribeWriter);
                cmd.Parameters.AddWithValue("@MatricSubjects", dtMatricSubject);
                cmd.Parameters.AddWithValue("@Section", RM.Section);

                cmd.Parameters.AddWithValue("@MetricMonth", RM.MetricMonth);
                cmd.Parameters.AddWithValue("@MetricYear", RM.MetricYear);
                cmd.Parameters.AddWithValue("@MetricRollNum", RM.Metric_Roll_Num);
                cmd.Parameters.AddWithValue("@MatricBoard", RM.MetricBoard);
                cmd.Parameters.AddWithValue("@CandStudyMedium", RM.CandiMedium);
                cmd.Parameters.AddWithValue("@MatricResult", RM.MatricResult);

                if (RM.Tgroup == "HUMANITIES")
                {
                    if (RM.PreNSQF == "" || RM.PreNSQF == null)
                    {
                        if (RM.PreNSQF == "NO")
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                        }

                    }
                    else if (RM.PreNSQF != "" || RM.PreNSQF != null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQF);

                    }
                }
                else if (RM.Tgroup == "SCIENCE")
                {
                    if (RM.PreNSQFsci == "" || RM.PreNSQFsci == null)
                    {
                        if (RM.PreNSQFsci == "NO")
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                        }

                    }
                    else if (RM.PreNSQFsci != "" || RM.PreNSQFsci != null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6sci);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQFsci);

                    }
                }
                else if (RM.Tgroup == "COMMERCE")
                {
                    if (RM.PreNSQFcomm == "" || RM.PreNSQFcomm == null)
                    {
                        if (RM.PreNSQFcomm == "NO")
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                        }

                    }
                    else if (RM.PreNSQFcomm != "" || RM.PreNSQFcomm != null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6comm);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQFcomm);

                    }
                }
                else if (RM.Tgroup == "VOCATIONAL")
                {
                    if (RM.PreNSQFvoc == "" || RM.PreNSQFvoc == null)
                    {
                        if (RM.PreNSQFvoc == "NO")
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                        }

                    }
                    else if (RM.PreNSQFvoc != "" || RM.PreNSQFvoc != null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6voc);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQFvoc);

                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                    cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                    cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                }


                string hostName = Dns.GetHostName(); // Retrive the Name of HOST
                string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                cmd.Parameters.AddWithValue("@MyIP", myIP);

                cmd.Parameters.Add("@StudentUniqueId", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                //SqlParameter outPutParameter = new SqlParameter();
                //outPutParameter.ParameterName = "@StudentUniqueId";
                //outPutParameter.Size = 20;
                //outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                //outPutParameter.Direction = System.Data.ParameterDirection.Output;
                //cmd.Parameters.Add(outPutParameter);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = (string)cmd.Parameters["@StudentUniqueId"].Value;
                return outuniqueid;


                //con.Open();
                //result = cmd.ExecuteNonQuery().ToString();
                //return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "-1";
            }
            finally
            {
                con.Close();
            }
        }

        public DataSet GetNAmeAndAbbrbySubFromSSE(string SUB)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetNAmeAndAbbrbySubSSE_sp", con);// GetNAmeAndAbbrbySubSP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SUB", SUB);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SubjectsTweleve()
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SubjectsTweleve_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet SubjectsTweleve_Commerce()
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Tweleve_sub_COMM_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SubjectsTweleve_SCI()
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Tweleve_sub_SCI_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SubjectsTweleve_Voc()
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Tweleve_sub_Voc_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SubjectsTweleve_hum()
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Tweleve_sub_hum_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SubjectsTweleve_tech()
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Tweleve_sub_tech_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SubjectsTweleve_agr()
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Tweleve_sub_agr_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet Voc_agr()
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Tweleve_voc_group", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet Voc_Trgroup(string selg)
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Tweleve_vocGroupTR", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@selg", selg);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet SubjectsTweleve_Voc_All_Trade()
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Tweleve_vocGroupAllTR", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SelectTwelveMedium(string S1)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("GetTwelvemedium", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@S1", S1);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SelectT1(string t1)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("GetSubjectTwelve", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@t1", t1);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SelectT2mediums(string T2M)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("GetT2medium", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@T2M", T2M);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet gettrade(string trade)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Tweleve_voc_get_trade", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@tra", trade);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet gettradeSubjects(string tradeSub)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("Tweleve_voc_get_trade_Subjects", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@tradeSub", tradeSub);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet selectGP(int id)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("GetAllGroup_Voc", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public string Delete_T_FromData(string stdid)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Delete_T1_T2Forms_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Std_id", stdid);
                cmd.Parameters.AddWithValue("@MyIP", myIP);             
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;
            }
            catch (Exception ex)
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }
        //---------------------------------------T2end----------------------

        public void FindSchoolName(string schoolcode, out string schoolname)
        {
            SqlConnection con = null;
            string result = "";
            try
            {


                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("FindSchoolName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                cmd.Parameters.Add("@schoolname", SqlDbType.NVarChar, 950).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                schoolname = Convert.ToString(cmd.Parameters["@schoolname"].Value);

            }
            catch (Exception ex)
            {
                schoolname = "";
            }
            finally
            {
                con.Close();
            }
        }

        public void ApplyToImport(string schoolcode,string schoolreqcode, out string schoolname)
        {
            SqlConnection con = null;
            string result = "";
            try
            {


                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("ApplyToImport", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                cmd.Parameters.AddWithValue("@schoolreqcode", schoolreqcode);
                cmd.Parameters.Add("@schoolname", SqlDbType.NVarChar, 950).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                schoolname = Convert.ToString(cmd.Parameters["@schoolname"].Value);

            }
            catch (Exception ex)
            {
                schoolname = "";
            }
            finally
            {
                con.Close();
            }
        }
        //--------------------------Check Date By Deepak-------------

        public DataSet CheckReg_AdmDate_and_LateAdmDate(string SCHL1,string Form, out string AdmDate, out string LateAdmDate)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            AdmDate = "";
            LateAdmDate = "";
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CheckReg_AdmDate_and_LateAdmDateSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL1);
                    cmd.Parameters.AddWithValue("@Form", Form);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    AdmDate = result.Tables[0].Rows[0]["AdmDate"].ToString();
                    LateAdmDate = result.Tables[0].Rows[0]["LateAdmDate"].ToString();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }


        public void CheckDate(string SCHL1, out string admdate)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DummyCheckDate", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL1);
                    cmd.Parameters.Add("@OutGDate", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    admdate = Convert.ToString(cmd.Parameters["@OutGDate"].Value);
                    //res1 = (int)cmd.Parameters["@Outstatus1"].Value;


                }
            }
            catch (Exception ex)
            {
                admdate = "";
                //res1 = -1;
            }
        }

        public void CheckDateE1E2T1T2(string SCHL1, out string admdate)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DummyCheckDateE1E2T1T2", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL1);
                    cmd.Parameters.Add("@OutGDate", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    admdate = Convert.ToString(cmd.Parameters["@OutGDate"].Value);

                }
            }
            catch (Exception)
            {
                admdate = "";
            }
        }
        //-----------------------Start T1-----------------
        public DataSet GetStudentRecordsSearch_T(string search, int PageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudentRecordsSearch_TD", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@PageIndex", PageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet SearchStudentGetByData_T(string sid, string frmname)
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("GetStudentRecordsByID_T", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@sid", sid);
                    cmd.Parameters.AddWithValue("@formName", frmname);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string Delete_T_FromData_Import(string stdid, string Yr, string oldid)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("Delete_T1_Imported_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Std_id", stdid);
                cmd.Parameters.AddWithValue("@Yr", Yr);
                cmd.Parameters.AddWithValue("@oldid", oldid);
                cmd.Parameters.AddWithValue("@MyIP", myIP);

                con.Open();
                result = cmd.ExecuteScalar().ToString();
                return result;

            }
            catch (Exception ex)
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }
        public string Update_T1_Data(RegistrationModels RM, FormCollection frm, string FormType, string sid, string FilePhoto, string sign, DataTable dtMatricSubject)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                //string formName = "T2";
                //RM.Caste = frm["CasteSelected"];
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("Update_T1_SubjectsForms_Sp", con); //Update_M1_M2_SubjectsForms_Sp //Update_M1_M2_SubjectsForms_Sp_Rohit_Test
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Std_id", sid);
                cmd.Parameters.AddWithValue("@form_Name", FormType);
                cmd.Parameters.AddWithValue("@Category", RM.Category);
                cmd.Parameters.AddWithValue("@Board", RM.Board);
                cmd.Parameters.AddWithValue("@Other_Board", RM.Other_Board);
                cmd.Parameters.AddWithValue("@Board_Roll_Num", RM.Board_Roll_Num);
                cmd.Parameters.AddWithValue("@Prev_School_Name", RM.Prev_School_Name);
                cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                cmd.Parameters.AddWithValue("@Month", RM.Month);
                cmd.Parameters.AddWithValue("@Year", RM.Year);
                //cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                //cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);
                cmd.Parameters.AddWithValue("@Admission_Date", RM.Admission_Date);
                cmd.Parameters.AddWithValue("@Class_Roll_Num_Section", RM.Class_Roll_Num_Section);
                cmd.Parameters.AddWithValue("@Candi_Name", RM.Candi_Name);
                cmd.Parameters.AddWithValue("@Candi_Name_P", RM.Candi_Name_P);
                cmd.Parameters.AddWithValue("@Father_Name", RM.Father_Name);
                cmd.Parameters.AddWithValue("@Father_Name_P", RM.Father_Name_P);
                cmd.Parameters.AddWithValue("@Mother_Name", RM.Mother_Name);
                cmd.Parameters.AddWithValue("@Mother_Name_P", RM.Mother_Name_P);
                cmd.Parameters.AddWithValue("@Caste", RM.Caste);
                cmd.Parameters.AddWithValue("@Gender", RM.Gender);
                //cmd.Parameters.AddWithValue("@Differently_Abled", frm["DA"].ToString());
                cmd.Parameters.AddWithValue("@Differently_Abled", RM.DA);
                cmd.Parameters.AddWithValue("@Religion", RM.Religion);
                cmd.Parameters.AddWithValue("@DOB", RM.DOB);
                cmd.Parameters.AddWithValue("@Belongs_BPL", RM.Belongs_BPL);
                cmd.Parameters.AddWithValue("@Mobile", RM.Mobile);
                cmd.Parameters.AddWithValue("@Aadhar_num", RM.Aadhar_num);
                cmd.Parameters.AddWithValue("@Group_Name", RM.Tgroup);
                cmd.Parameters.AddWithValue("@E_punjab_Std_id", RM.E_punjab_Std_id);
                cmd.Parameters.AddWithValue("@Address", RM.Address);
                cmd.Parameters.AddWithValue("@LandMark", RM.LandMark);
                cmd.Parameters.AddWithValue("@Block", RM.Block);

                //cmd.Parameters.AddWithValue("@District",RM.District);
                cmd.Parameters.AddWithValue("@District", RM.MyDistrict);
                cmd.Parameters.AddWithValue("@Tehsil", RM.Tehsil);



                cmd.Parameters.AddWithValue("@PinCode", RM.PinCode);
                if (RM.file == null)
                {
                    cmd.Parameters.AddWithValue("@std_Photo", FilePhoto);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Photo", RM.file.FileName);
                }
                if (RM.std_Sign == null)
                {
                    cmd.Parameters.AddWithValue("@std_Sign", sign);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Sign", RM.std_Sign.FileName);
                }

                //----
                if (RM.IsPrevSchoolSelf.ToString() == "True")
                {
                    //cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", RM.IsPrevSchoolSelf);
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 0);
                }
                if (RM.IsPSEBRegNum.ToString() == "True")
                {
                    //cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                    cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                }

                if (RM.Provisional.ToString() == "True")
                {

                    cmd.Parameters.AddWithValue("@Provisional", 1);
                    //cmd.Parameters.AddWithValue("@AWRegisterNo", NULL);
                    //cmd.Parameters.AddWithValue("@Admission_Num", NULL);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Provisional", 0);
                    cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                    cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);

                }

                //cmd.Parameters.AddWithValue("@ScribeWriter", RM.scribeWriter);
                cmd.Parameters.AddWithValue("@MatricSubjects", dtMatricSubject);
                cmd.Parameters.AddWithValue("@Section", RM.Section);

                cmd.Parameters.AddWithValue("@MetricMonth", RM.MetricMonth);
                cmd.Parameters.AddWithValue("@MetricYear", RM.MetricYear);
                cmd.Parameters.AddWithValue("@MetricRollNum", RM.Metric_Roll_Num);
                cmd.Parameters.AddWithValue("@MatricBoard", RM.MetricBoard);
                cmd.Parameters.AddWithValue("@CandStudyMedium", RM.CandiMedium);

                string hostName = Dns.GetHostName(); // Retrive the Name of HOST
                string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                cmd.Parameters.AddWithValue("@MyIP", myIP);

                // cmd.Parameters.Add("@StudentUniqueId", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                SqlParameter outPutParameter = new SqlParameter();
                outPutParameter.ParameterName = "@StudentUniqueId";
                outPutParameter.Size = 20;
                outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                outPutParameter.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(outPutParameter);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = (string)cmd.Parameters["@StudentUniqueId"].Value;
                return outuniqueid;


                //con.Open();
                //result = cmd.ExecuteNonQuery().ToString();
                //return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "-1";
            }
            finally
            {
                con.Close();
            }
        }
        //-------------------------End T1-----------------

        //--------------------Start M1 -------------------------//
        public string Delete_M1_FromData_Import(string stdid, string Yr, string oldid)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("Delete_M1_M2Forms_Import_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Std_id", stdid);
                cmd.Parameters.AddWithValue("@Yr", Yr);
                cmd.Parameters.AddWithValue("@oldid", oldid);
                //cmd.Parameters.AddWithValue("@MyIP", myIP);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                //result = cmd.ExecuteScalar().ToString();
                return result;

            }
            catch (Exception ex)
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }
        public string Update_M1_Import_Data(RegistrationModels RM, FormCollection frm, string FormType, string sid, string FilePhoto, string sign, DataTable dtMatricSubject)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                

                // con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_M1_ImportData_SubjectsForms_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Std_id", sid);
                cmd.Parameters.AddWithValue("@form_Name", FormType);
                cmd.Parameters.AddWithValue("@Category", RM.Category);
                cmd.Parameters.AddWithValue("@Board", RM.Board);
                cmd.Parameters.AddWithValue("@Other_Board", RM.Other_Board);
                cmd.Parameters.AddWithValue("@Board_Roll_Num", RM.Board_Roll_Num);
                cmd.Parameters.AddWithValue("@Prev_School_Name", RM.Prev_School_Name);
                cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                cmd.Parameters.AddWithValue("@Month", RM.Month);
                cmd.Parameters.AddWithValue("@Year", RM.Year);
                //cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                //cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);
                cmd.Parameters.AddWithValue("@Admission_Date", RM.Admission_Date);
                cmd.Parameters.AddWithValue("@Class_Roll_Num_Section", RM.Class_Roll_Num_Section);
                cmd.Parameters.AddWithValue("@Candi_Name", RM.Candi_Name);
                cmd.Parameters.AddWithValue("@Candi_Name_P", RM.Candi_Name_P);
                cmd.Parameters.AddWithValue("@Father_Name", RM.Father_Name);
                cmd.Parameters.AddWithValue("@Father_Name_P", RM.Father_Name_P);
                cmd.Parameters.AddWithValue("@Mother_Name", RM.Mother_Name);
                cmd.Parameters.AddWithValue("@Mother_Name_P", RM.Mother_Name_P);
                cmd.Parameters.AddWithValue("@Caste", RM.Caste);
                cmd.Parameters.AddWithValue("@Gender", RM.Gender);
                //cmd.Parameters.AddWithValue("@Differently_Abled", frm["DA"].ToString());
                cmd.Parameters.AddWithValue("@Differently_Abled", RM.DA);
                cmd.Parameters.AddWithValue("@Religion", RM.Religion);
                cmd.Parameters.AddWithValue("@DOB", RM.DOB);
                cmd.Parameters.AddWithValue("@Belongs_BPL", RM.Belongs_BPL);
                cmd.Parameters.AddWithValue("@Mobile", RM.Mobile);
                cmd.Parameters.AddWithValue("@Aadhar_num", RM.Aadhar_num);

                cmd.Parameters.AddWithValue("@Group_Name", RM.MyGroup);

                cmd.Parameters.AddWithValue("@E_punjab_Std_id", RM.E_punjab_Std_id);
                cmd.Parameters.AddWithValue("@Address", RM.Address);
                cmd.Parameters.AddWithValue("@LandMark", RM.LandMark);
                cmd.Parameters.AddWithValue("@Block", RM.Block);

                //cmd.Parameters.AddWithValue("@District",RM.District);
                cmd.Parameters.AddWithValue("@District", RM.MyDistrict);
                cmd.Parameters.AddWithValue("@Tehsil", RM.Tehsil);



                cmd.Parameters.AddWithValue("@PinCode", RM.PinCode);
                if (RM.file == null)
                {
                    cmd.Parameters.AddWithValue("@std_Photo", FilePhoto);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Photo", RM.file.FileName);
                }
                if (RM.std_Sign == null)
                {
                    cmd.Parameters.AddWithValue("@std_Sign", sign);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@std_Sign", RM.std_Sign.FileName);
                }

                //----
                if (RM.IsPrevSchoolSelf.ToString() == "True")
                {
                    //cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", RM.IsPrevSchoolSelf);
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsPrevSchoolSelf", 0);
                }
                if (RM.IsPSEBRegNum.ToString() == "True")
                {
                    //cmd.Parameters.AddWithValue("@Registration_num", RM.Registration_num);
                    cmd.Parameters.AddWithValue("@IsPSEBRegNum", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                }

                if (RM.Provisional.ToString() == "True")
                {

                    cmd.Parameters.AddWithValue("@Provisional", 1);
                    //cmd.Parameters.AddWithValue("@AWRegisterNo", NULL);
                    //cmd.Parameters.AddWithValue("@Admission_Num", NULL);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Provisional", 0);
                    cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                    cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);

                }
                //------------
                //---------------------Subject Details-----------------
                if (frm["DA"].ToString() == "N.A.")
                {
                    if(RM.NSQF== "YES")
                    {
                        cmd.Parameters.AddWithValue("@NSQF", 1);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@NSQF", 0);
                    }
                   
                    cmd.Parameters.AddWithValue("@MatricSubjects", dtMatricSubject);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@ScribeWriter", RM.scribeWriter);
                    cmd.Parameters.AddWithValue("@MatricSubjects", dtMatricSubject);
                }

                //----------------------End SubDetails-----------------

                cmd.Parameters.AddWithValue("@Section", RM.Section);
                cmd.Parameters.AddWithValue("@CandStudyMedium", RM.CandiMedium);               

                string hostName = Dns.GetHostName(); // Retrive the Name of HOST
                string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                cmd.Parameters.AddWithValue("@MyIP", myIP);

                // cmd.Parameters.Add("@StudentUniqueId", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                SqlParameter outPutParameter = new SqlParameter();
                outPutParameter.ParameterName = "@StudentUniqueId";
                outPutParameter.Size = 20;
                outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                outPutParameter.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(outPutParameter);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = (string)cmd.Parameters["@StudentUniqueId"].Value;
                return outuniqueid;


                //con.Open();
                //result = cmd.ExecuteNonQuery().ToString();
                //return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "-1";
            }
            finally
            {
                con.Close();
            }
        }
        //--------------------------------------T1 Start-----------------------


        //----
        #region Particular Correction Data Begin
        public DataSet schooltypesCorrection(string schid,string Corrections)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("schooltypesCorrection", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", schid);
                    cmd.Parameters.AddWithValue("@Corrections", Corrections);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetSchoolCorrectionAllRecord(string schlid)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetSchoolCorrectionAllRecord_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schlid", schlid);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }


        public DataSet GetCorrPunjabiName(string id)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCorrPunjabiName_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@sid", id);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }


        public DataSet getCorrrectionField(string std_Class)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("getCorrrectionFieldsp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@std_Class", std_Class);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet getAllTehsil()
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("getAllTehsilsp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet getAllDistrict()
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("getAllDistrictsp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet GetCorrectionStudentRecordsSearch_ED(string search, int formName, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCorrectionStudentRecordsSearch_ED", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@formName", formName);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetStudentRecordsCorrectiondata(string schlid)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudentRecordsCorrectiondata_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schlid", schlid);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetStudentRecordsCorrectiondataPending(string schlid)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudentRecordsCorrectiondataPending_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schlid", schlid);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }


        public DataSet InsertSchoolCorrectionAdd(RegistrationModels RM, FormCollection frm)
        {
            SqlConnection con = null;
            //string result = "";
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("InsertSchoolCorrectionAdd_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Schl", RM.SCHL);
                cmd.Parameters.AddWithValue("@Class", RM.Class);
                cmd.Parameters.AddWithValue("@Std_id", RM.Std_id);
                cmd.Parameters.AddWithValue("@OldValue", RM.oldVal);
                cmd.Parameters.AddWithValue("@NewValue", RM.newVal);
                cmd.Parameters.AddWithValue("@CorrectionType", RM.Correctiontype);
                cmd.Parameters.AddWithValue("@Remark", RM.Remark);
                //con.Open();
                //result = cmd.ExecuteNonQuery().ToString();
                //return result;

                ad.SelectCommand = cmd;
                ad.Fill(result);
                con.Open();
                return result;

            }
            catch (Exception ex)
            {
                return result = null;
            }
            finally
            {
                con.Close();
            }
        }
        //------Insert Admin School Correction Add on 05/12/2017----
        public DataSet InsertAdminSchoolCorrectionAdd(RegistrationModels RM, FormCollection frm)
        {
            SqlConnection con = null;
            //string result = "";
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("InsertAdminSchoolCorrectionAdd_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Schl", RM.SCHL);
                cmd.Parameters.AddWithValue("@Class", RM.Class);
                cmd.Parameters.AddWithValue("@Std_id", RM.Std_id);
                cmd.Parameters.AddWithValue("@OldValue", RM.oldVal);
                cmd.Parameters.AddWithValue("@NewValue", RM.newVal);
                cmd.Parameters.AddWithValue("@CorrectionType", RM.Correctiontype);
                cmd.Parameters.AddWithValue("@Remark", RM.Remark);
                cmd.Parameters.AddWithValue("@MyIP", myIP);
                cmd.Parameters.AddWithValue("@hostName", hostName);
                //con.Open();
                //result = cmd.ExecuteNonQuery().ToString();
                //return result;

                ad.SelectCommand = cmd;
                ad.Fill(result);
                con.Open();
                return result;

            }
            catch (Exception ex)
            {
                return result = null;
            }
            finally
            {
                con.Close();
            }
        }
        public string AiddedCorrectionRecordDelete(string CorrectionId)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            //string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("AiddedCorrectionRecordDelete_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CorrectionId", CorrectionId);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

            }
            catch (Exception ex)
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }

        public string FinalSubmitCorrection(RegistrationModels RM)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            //string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("FinalSubmitCorrection_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Schl", RM.SCHL);
                //cmd.Parameters.AddWithValue("@cl", RM.Class);
                cmd.Parameters.AddWithValue("@cr", RM.Correctiontype);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

            }
            catch (Exception ex)
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }


        public DataSet SchoolCorrectionPerformaFinalPrintSession(RegistrationModels RM)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SchoolCorrectionPerformaFinalPrintSessionsp", con);
                    cmd.Parameters.AddWithValue("@SCHL", RM.SCHL);
                    cmd.Parameters.AddWithValue("@CorrectionLot", RM.CorrectionLot);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet GetCorrectiondataFinalPrintList(RegistrationModels RM)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCorrectiondataFinalPrintList_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schl", RM.SCHL);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet SchoolCorrectionPerformaRoughReport(RegistrationModels RM)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SchoolCorrectionPerformaRoughReport_sp", con);
                    cmd.Parameters.AddWithValue("@SCHL", RM.SCHL);
                    cmd.Parameters.AddWithValue("@CType", RM.CorrectionLot);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        #endregion Particular Correction Data End

        //------------------------------------------------------------------Correction Performa Subject Begin----------------
        #region Subject Correction Performa Begin
        public DataSet SearchOldStudent_Subject(string sid, string frmname, string scode)
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SearchOldStudent_Subject_sp", con);// GetStudentRecordsByID_SUBJECT
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@sid", sid);
                    cmd.Parameters.AddWithValue("@formName", frmname);
                    cmd.Parameters.AddWithValue("@scode", scode);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet NewElectiveSubjects()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("NewElectiveSubjects_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string Ins_CorrectionSub(RegistrationModels RM, int cls, string regopenType, int CANDID, string schlCode, string CRC)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Ins_CorrectionSub_Sp", con); ///insert_N1_N2_N3Forms_Sp
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Class", cls);
                cmd.Parameters.AddWithValue("@OpenRegularType", regopenType);
                cmd.Parameters.AddWithValue("@CANDID", CANDID);
                cmd.Parameters.AddWithValue("@Oldsubcode", RM.OldSub);
                cmd.Parameters.AddWithValue("@Newsubcode", RM.NewSub);
                cmd.Parameters.AddWithValue("@Newsubmedium", RM.NewMedium);
                cmd.Parameters.AddWithValue("@Correctiontype", RM.Correctiontype);
                cmd.Parameters.AddWithValue("@CorrectionLot", "0");
                cmd.Parameters.AddWithValue("@schlCode", schlCode);
                cmd.Parameters.AddWithValue("@CRC", CRC);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                if (con.State == ConnectionState.Open)
                    con.Close();

                return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                if (con != null)
                {
                    con.Dispose();
                    con.Close();
                    con = null;
                }
                return result = "";

            }
            finally
            {
                if (con != null)
                {
                    con.Dispose();
                    con.Close();
                    con = null;
                }
            }
        }

        public DataSet SelectChangesMatricSubjects(int cls, string id)
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);GetLastEntryStudentRecords
                    SqlCommand cmd = new SqlCommand("SelectChangesMatricSubjects_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cls", cls);
                    cmd.Parameters.AddWithValue("@id", id);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SelectChangesSubjects(string SCHL)
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);GetLastEntryStudentRecords
                    SqlCommand cmd = new SqlCommand("SelectChangesSubjects_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schoolCode", SCHL);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SelectOLDSUB(string id, string frmname)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectOLDSUB_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@sid", id);
                    cmd.Parameters.AddWithValue("@fname", frmname);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string DeleteMatricSubData(string Corid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("DeleteMatricSubData_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Corid", Corid);
                con.Open();
                result = cmd.ExecuteScalar().ToString();
                if (con.State == ConnectionState.Open)
                    con.Close();
                return result;

            }
            catch (Exception ex)
            {
                con.Close();
                con.Dispose();
                con = null;
                return result = "";
            }
            finally
            {
                con.Close();
                con.Dispose();
                con = null;
            }
        }
        public DataSet NewCorrectionSubjects(string sid, string frmname, string scode)
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("NewCorrectionSubjects_sp", con);// GetStudentRecordsByID_SUBJECT
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@sid", sid);
                    cmd.Parameters.AddWithValue("@formName", frmname);
                    cmd.Parameters.AddWithValue("@scode", scode);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SelectResgularOpenMedium(string S1, string fname)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("GetmediumOpenRegular", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@S1", S1);
                    cmd.Parameters.AddWithValue("@fname", fname);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SearchCorrectionStudentDetails(string formName, string schl, string sid)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SearchCorrectionStudentDetails_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@formName", formName);
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@sid", sid);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet ViewAllCorrectionSubjects(string Schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ViewAllCorrectionSubjects_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cls", Schl);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet PendingCorrectionSubjects(string Schl, string cls)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PendingCorrectionSubjects_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Schl", Schl);
                    cmd.Parameters.AddWithValue("@cls", cls);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string FinalSubmitSubjectCorrection(RegistrationModels RM,string Class)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            //string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("FinalSubmitCorrection_Sp_221218", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Schl", RM.SCHL);
                cmd.Parameters.AddWithValue("@cl", Class);
                cmd.Parameters.AddWithValue("@cr", RM.Correctiontype);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

            }
            catch (Exception ex)
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }

        #endregion Subject Correction Performa Begin

        //------------------------------------------------------------------------------Correction Performa Subject End-----------

        //------------FinalPrintAdminCard
        #region AdminCard DB Begin
        public DataSet GetFinalPrintSeniorAdmitCard(string schlid, string ClsType)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetFinalPrintSeniorAdmitCard_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", schlid);
                    cmd.Parameters.AddWithValue("@ClsType", ClsType);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetFinalPrintSeniorOpenAdmitCard(string schlid, string ClsType)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetFinalPrintSeniorOpenAdmitCard_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", schlid);
                    cmd.Parameters.AddWithValue("@ClsType", ClsType);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet GetFinalPrintSeniorAdmitCardSearch(string Search, string schlid)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetFinalPrintSeniorAdmitCardSearch_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@SCHL", schlid);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetFinalPrintSeniorOpenAdmitCardSearch(string Search, string schlid)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetFinalPrintSeniorOpenAdmitCardSearch_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@SCHL", schlid);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }


        //----- Matric Admit Card Regular DB
        public DataSet GetFinalPrintMatricAdmitCardSearch(string Search, string schlid)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetFinalPrintMatricAdmitCardSearch_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@SCHL", schlid);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        //----- Matric Admit Card Open DB
        public DataSet GetFinalPrintMatricOpenAdmitCardSearch(string Search, string schlid)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetFinalPrintMatricOpenAdmitCardSearch_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@SCHL", schlid);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        #endregion AdminCard DB End
        //-----------FinalPrintAdminCard
        //------------------------------------------------------------------------------Correction Performa Stream Begin-----------
        #region Correction Performa Stream Begin
        public string Ins_Correction_Senior_Data(string FormType, string oldstream, string NewStream, int cls, string opnreg, string Std_id, string schl, DataTable dtMatricSubject)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Ins_Correction_Senior_Data_Sp", con); //insert_M1_M2_Subject_Forms_Sp , insert_M1_M2_Subject_Forms_Sp_new
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@form_Name", FormType);
                cmd.Parameters.AddWithValue("@OldStream", oldstream);
                cmd.Parameters.AddWithValue("@NewStream", NewStream);
                cmd.Parameters.AddWithValue("@OpenRegularType", opnreg);
                cmd.Parameters.AddWithValue("@Class", cls);
                cmd.Parameters.AddWithValue("@CANDID", Std_id);
                cmd.Parameters.AddWithValue("@SCHL", schl);
                cmd.Parameters.AddWithValue("@MatricSubjects", dtMatricSubject);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                if (con.State == ConnectionState.Open)
                    con.Close();
                return result;
            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }
        public DataSet SelectChangesStreams(string SCHL)
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("SelectChangesStreams_Sp", con); //SelectChangesSubjects_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schoolCode", SCHL);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string DeleteStreamData(string Corid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("DeleteStreamData_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Corid", Corid);
                con.Open();
                result = cmd.ExecuteScalar().ToString();
                if (con.State == ConnectionState.Open)
                    con.Close();
                return result;

            }
            catch (Exception ex)
            {
                con.Close();
                con.Dispose();
                con = null;
                return result = "";
            }
            finally
            {
                con.Close();
                con.Dispose();
                con = null;
            }
        }
        public DataSet PendingCorrectionStreams(string Schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PendingCorrectionStreams_sp", con); //PendingCorrectionSubjects_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cls", Schl);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet ViewAllCorrectionStreams(string Schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ViewAllCorrectionStreams_sp", con); //ViewAllCorrectionSubjects_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cls", Schl);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        #endregion Correction Performa Stream Begin
        //------------------------------------------------------------------------------Correction Performa Stream End-----------
        //--------------region Correction Photo_Sign

        #region Correction Photo_Sign
        public DataSet PhotoSignSearchCorrectionStudentDetails(string formName, string schl, string sid)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PhotoSignSearchCorrectionStudentDetails_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@formName", formName);
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@sid", sid);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string InsertPhotoSignCorrectionAdd(RegistrationModels RM, FormCollection frm,string AdminUser, string EmpUserId)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("InsertPhotoSignCorrectionAdd_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Schl", RM.SCHL);
                cmd.Parameters.AddWithValue("@Class", RM.Class);
                cmd.Parameters.AddWithValue("@Std_id", RM.Std_id);
                cmd.Parameters.AddWithValue("@OldValue", RM.oldVal);
                cmd.Parameters.AddWithValue("@NewValue", RM.newVal);
                cmd.Parameters.AddWithValue("@CorrectionType", RM.Correctiontype);
                cmd.Parameters.AddWithValue("@Remark", RM.Remark);
                cmd.Parameters.AddWithValue("@AdminUser", AdminUser);
                cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

            }
            catch (Exception ex)
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }
        public string FinalSubmitImageCorrection(RegistrationModels RM)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            //string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("FinalSubmitCorrection_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Schl", RM.SCHL);
                //cmd.Parameters.AddWithValue("@cl", RM.Class);
                cmd.Parameters.AddWithValue("@cr", RM.Correctiontype);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

            }
            catch (Exception ex)
            {
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }
        public DataSet ViewAllPhotoSignCorrection(string Schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ViewAllPhotoSignCorrection_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Schl", Schl);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet PendingPhotoSignCorrection(string Schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PendingPhotoSignCorrection_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Schl", Schl);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string DeletePhotoSignData(string Corid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("DeletePhotoSignData_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Corid", Corid);
                con.Open();
                result = cmd.ExecuteScalar().ToString();
                if (con.State == ConnectionState.Open)
                    con.Close();
                return result;

            }
            catch (Exception ex)
            {
                con.Close();
                con.Dispose();
                con = null;
                return result = "";
            }
            finally
            {
                con.Close();
                con.Dispose();
                con = null;
            }
        }
        #endregion Correction Photo_Sign

        //-------
        public DataSet SearchSchoolDist(string Oschlcode)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("GetSchlDist", con);// GetStudentRecordsByID_SUBJECT
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Oschlcode", Oschlcode);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        //------------------------------------------------- Start View Registration of N2/M2/E2/T2 --------------------------------     
        public DataSet ViewRegNoBySchool(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ViewRegNoBySchoolSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        
        //------------------------------------------------- End View Registration of N2/M2/E2/T2 --------------------------------
        #region Final Submitted Records DB
        public DataSet FinalSubmittedRecordsAll(string search, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("FinalSubmittedRecordsAll_spwithGroup", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet CommanFormView(string sid, string formName)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {                 
                    SqlCommand cmd = new SqlCommand("CommanFormView_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@sid", sid);
                    cmd.Parameters.AddWithValue("@formName", formName);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        #endregion Final Submitted Records DB

        #region UpdateAadharEnrollNo
        public DataSet UpdaadharEnrollmentNo(string std_id, string aadhar_num, string SCHL, string Caste, string gender, string BPL, string Rel, string Epunid)
        // public string UpdaadharEnrollmentNo(string std_id, string aadhar_num, string SCHL)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UpdateaadharNum_Sp", con);//ReGenerateChallaanByIdSPAdminNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@std_id", std_id);
                    cmd.Parameters.AddWithValue("@aadhar_num", aadhar_num);
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);

                    //cmd.Parameters.AddWithValue("@Caste", Caste);
                    //cmd.Parameters.AddWithValue("@gender", gender);
                    //cmd.Parameters.AddWithValue("@BPL", BPL);
                    //cmd.Parameters.AddWithValue("@Rel", Rel);
                    //cmd.Parameters.AddWithValue("@Epunid", Epunid);

                    //con.Open();
                    //result = cmd.ExecuteNonQuery().ToString();
                    //if (con.State == ConnectionState.Open)
                    //    con.Close();

                    //return result;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;

                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        #endregion UpdateAadharEnrollNos

        //-----------------------------Maric Subject Correction--------------------//
        #region start Subject Correction
        public DataSet SearchStudentGetByData_SubjectCORR(string sid, string frmname)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudentRecordsByID_SUBJECTCORR", con);// GetStudentRecordsByID_SUBJECT
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@sid", sid);
                    cmd.Parameters.AddWithValue("@formName", frmname);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string Senior_Subject_Correction(RegistrationModels RM, FormCollection frm, string sid, DataTable dtMatricSubject)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                //string formName = "T2";
                // RM.Caste = frm["CasteSelected"];
                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_Senior_SubjectCorrection_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Std_id", sid);
                cmd.Parameters.AddWithValue("@Group_Name", RM.Tgroup);

                if (RM.Tgroup == "HUMANITIES")
                {
                    if (RM.PreNSQF == "" || RM.PreNSQF == null)
                    {
                        if (RM.PreNSQF == "NO")
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                        }

                    }
                    else if (RM.PreNSQF != "" || RM.PreNSQF != null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQF);

                    }
                }
                else if (RM.Tgroup == "SCIENCE")
                {
                    if (RM.PreNSQFsci == "" || RM.PreNSQFsci == null)
                    {
                        if (RM.PreNSQFsci == "NO")
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                        }

                    }
                    else if (RM.PreNSQFsci != "" || RM.PreNSQFsci != null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6sci);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQFsci);

                    }
                }
                else if (RM.Tgroup == "COMMERCE")
                {
                    if (RM.PreNSQFcomm == "" || RM.PreNSQFcomm == null)
                    {
                        if (RM.PreNSQFcomm == "NO")
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                        }

                    }
                    else if (RM.PreNSQFcomm != "" || RM.PreNSQFcomm != null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6comm);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQFcomm);

                    }
                }
                else if (RM.Tgroup == "VOCATIONAL")
                {
                    if (RM.PreNSQFvoc == "" || RM.PreNSQFvoc == null)
                    {
                        if (RM.PreNSQFvoc == "NO")
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                        }

                    }
                    else if (RM.PreNSQFvoc != "" || RM.PreNSQFvoc != null)
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6voc);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQFvoc);

                    }
                }
                else
                {
                    cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                    cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                    cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                }
                cmd.Parameters.AddWithValue("@MatricSubjects", dtMatricSubject);

                string hostName = Dns.GetHostName(); // Retrive the Name of HOST
                string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                cmd.Parameters.AddWithValue("@MyIP", myIP);

                // cmd.Parameters.Add("@StudentUniqueId", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                SqlParameter outPutParameter = new SqlParameter();
                outPutParameter.ParameterName = "@StudentUniqueId";
                outPutParameter.Size = 20;
                outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                outPutParameter.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(outPutParameter);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = (string)cmd.Parameters["@StudentUniqueId"].Value;
                return outuniqueid;


                //con.Open();
                //result = cmd.ExecuteNonQuery().ToString();
                //return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "";
            }
            finally
            {
                con.Close();
            }

        }
        public string Matric_Subject_Correction(RegistrationModels RM, FormCollection frm, string sid, DataTable dtMatricSubject)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                //string formName = "E2";
                //RM.Caste = frm["CasteSelected"];
                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_Matric_SubjectCorrection_SpNew", con); 
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Std_id", sid);
                cmd.Parameters.AddWithValue("@Differently_Abled", RM.DA);               
                
                if (RM.DA.ToString() == "N.A.")
                {
                    cmd.Parameters.AddWithValue("@ScribeWriter", "NO");

                    if (RM.NsqfsubS6 == "" || RM.NsqfsubS6 == null)
                    {
                        if (RM.PreNSQF == "NO")
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@DP", 0);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@DP", 0);
                        }

                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQF);
                        cmd.Parameters.AddWithValue("@DP", 0);
                    }

                }
                else
                {
                    cmd.Parameters.AddWithValue("@DP", RM.DP);
                    //cmd.Parameters.AddWithValue("@NSQF", 0);
                    cmd.Parameters.AddWithValue("@ScribeWriter", RM.scribeWriter);

                    if (RM.NsqfsubS6 == "" || RM.NsqfsubS6 == null)
                    {
                        if (RM.PreNSQF == "NO")
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", "NO");
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", "NO");
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@NSQF_flag", 0);
                            cmd.Parameters.AddWithValue("@NSQF_SUB", null);
                            cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", null);
                        }

                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@NSQF_flag", 1);
                        cmd.Parameters.AddWithValue("@NSQF_SUB", RM.NsqfsubS6);
                        cmd.Parameters.AddWithValue("@PRE_NSQF_SUB", RM.PreNSQF);
                    }
                }

                //----------------------End SubDetails-----------------
                cmd.Parameters.AddWithValue("@MatricSubjects", dtMatricSubject);
               
                string hostName = Dns.GetHostName(); // Retrive the Name of HOST
                string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                cmd.Parameters.AddWithValue("@MyIP", myIP);                

                cmd.Parameters.Add("@StudentUniqueId", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string OutError = (string)cmd.Parameters["@OutError"].Value;
                string outuniqueid = (string)cmd.Parameters["@StudentUniqueId"].Value;                
                return outuniqueid;
            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }
        public string InsertDiffSubjects(string candid, string DiffSub)
        {
            string result = "";
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("InsertDiffSubjects_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@candid", candid);
                    cmd.Parameters.AddWithValue("@DiffSub", DiffSub);

                    con.Open();
                    result = cmd.ExecuteNonQuery().ToString();                  
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        #endregion End Subject Correction
        //----------------------------End matric Subject Correction----------------//

        #region SCHLSTAFF

        public DataSet SelectBlock(string DIST)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetALLBLOCK_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DIST", DIST);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet Select_CLUSTER_NAME(string BLOCK)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetALLCluster_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BLOCK", BLOCK);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet GetStaffFinalSubmit(string SCHL)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStaffFinalSubmit_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    // int count = Convert.ToInt32(cmd.Parameters["@PageCount"].Value);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string ReleavedRecordSchoolStaffDetails(string STAFFID, string Reason, string comment)
        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ReleavedRecordSchoolStaffDetailsNew_SP", con);//ReGenerateChallaanByIdSPAdminNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@STAFFID", STAFFID);
                    cmd.Parameters.AddWithValue("@Reason", Reason);
                    cmd.Parameters.AddWithValue("@comment", comment);
                    con.Open();
                    result = cmd.ExecuteNonQuery().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }

        public string ReleavedRecordSchoolStaffDetails(int id)
        {
            SqlConnection con = null;
            string result = "";
            try
            {


                // con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("ReleavedRecordSchoolStaffDetails_SP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ccfid", id);
                cmd.Parameters.Add("@OutId", SqlDbType.Int).Direction = ParameterDirection.Output;

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = Convert.ToString(cmd.Parameters["@OutId"].Value);
                return outuniqueid;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }
        public DataSet GetStaffRecordsToImportSearch(string search, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStaffRecordsToImportSearch_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 50);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    // int count = Convert.ToInt32(cmd.Parameters["@PageCount"].Value);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string ImportStaffToSCHOOL(string SCHL, string CHKStaffID)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("ImportStaffToSCHOOL_SP", con); ///Update_CCODE_To_CENTRE_SP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SCHL", SCHL);
                cmd.Parameters.AddWithValue("@CHKStaffID", CHKStaffID);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                if (con.State == ConnectionState.Open)
                    con.Close();

                return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                if (con != null)
                {
                    con.Dispose();
                    con.Close();
                    con = null;
                }
                return result = "";

            }
            finally
            {
                if (con != null)
                {
                    con = null;
                }
            }
        }
        #endregion SCHLSTAFFsssssss

        #region Update NADID DB
        public DataSet UpdateNADID(string std_id, string NADID, string SCHL)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UpdateNADID_Sp", con);//ReGenerateChallaanByIdSPAdminNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@std_id", std_id);
                    cmd.Parameters.AddWithValue("@NADID", NADID);
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        #endregion Update NADID DB

        #region Late Admission


        public DataSet GetLateAdmissionSchl(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetLateAdmissionSchl_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public string SetLateAdmissionSchl(string CreatedBy, string ModifyBy, string PanelType,string schl, string RID, string cls, string formNM, string regno, string name, string fname, string mname, string dob, string mobileno, string file, string usertype,string OBoard)
        {
            SqlConnection con = null;
            string result = null;
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("SetLateAdmissionSchl_sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PanelType", PanelType);
                cmd.Parameters.AddWithValue("@schl", schl);
                cmd.Parameters.AddWithValue("@RID", RID);
                cmd.Parameters.AddWithValue("@Class", cls);
                cmd.Parameters.AddWithValue("@form", formNM);
                cmd.Parameters.AddWithValue("@regno", regno);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@fname", fname);
                cmd.Parameters.AddWithValue("@mname", mname);
                cmd.Parameters.AddWithValue("@dob", dob);
                cmd.Parameters.AddWithValue("@mobileno", mobileno);
                cmd.Parameters.AddWithValue("@filepath", file);
                cmd.Parameters.AddWithValue("@usertype", usertype);
                cmd.Parameters.AddWithValue("@OBoard", OBoard);
                cmd.Parameters.AddWithValue("@CreatedBy", CreatedBy);
                cmd.Parameters.AddWithValue("@ModifyBy", ModifyBy);
                cmd.Parameters.Add("@Outstatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                result = Convert.ToString(cmd.Parameters["@Outstatus"].Value);
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
            finally
            {
                con.Close();
            }
        }

        public string FinalSubmitLateAdmissionSchl(string RID)
        {
            SqlConnection con = null;
            string result = null;
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());                
                SqlCommand cmd = new SqlCommand("FinalSubmitLateAdmissionSchl_sp", con);
                cmd.CommandType = CommandType.StoredProcedure;                
                cmd.Parameters.AddWithValue("@RID", RID);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
            finally
            {
                con.Close();
            }
        }

        public DataSet LateAdmPrintLetter(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetLateAdmPrintLetter_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet LateAdmHistory(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetLateAdmHistory_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string ApproveRejectLateAdmissionAdmin(string RID,string action)
        {
            SqlConnection con = null;
            string result = null;
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("ApprRejLateAdmissionAdmin_sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RID", RID);
                cmd.Parameters.AddWithValue("@action", action);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
            finally
            {
                con.Close();
            }
        }
        public string UpdStsLateAdmissionAdmin(string UserNM, string RID, string status, string ApprDate, string remarks)
        {
            SqlConnection con = null;
            string result = null;
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());                
                SqlCommand cmd = new SqlCommand("UpdStsLateAdmissionAdmin_sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserNM", UserNM);                
                cmd.Parameters.AddWithValue("@RID", RID);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@ApprDate", ApprDate);
                cmd.Parameters.AddWithValue("@remarks", remarks);                
                cmd.Parameters.Add("@Outstatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                result = Convert.ToString(cmd.Parameters["@Outstatus"].Value);
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
            finally
            {
                con.Close();
            }
        }
        public DataSet GetLateAdmRIDVerify(int RID)
        {         
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {                 
                    SqlCommand cmd = new SqlCommand("GetLateAdmRIDVerify_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RID", RID);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetLateAdmRIDDataVerify(int RID, string CNM, string FNM, string MNM, string DOB)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetLateAdmRIDDataVerify_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RID", RID);
                    cmd.Parameters.AddWithValue("@CNM", CNM);
                    cmd.Parameters.AddWithValue("@FNM", FNM);
                    cmd.Parameters.AddWithValue("@MNM", MNM);
                    cmd.Parameters.AddWithValue("@DOB", DOB);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        #endregion Late Admission

        #region For Api Admit Card Status DB
        public DataSet AdmitCardStatusSearch(string search, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AdmitCardStatusSearch_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string AdmitCardAcceptReject(string acceptid, string rejectid, string removeid, string adminid, out string OutStatus)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("AdmitCardAcceptReject_SP", con);  //CreateAdminUserSP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@acceptid", acceptid);
                cmd.Parameters.AddWithValue("@rejectid", rejectid);
                cmd.Parameters.AddWithValue("@removeid", removeid);
                cmd.Parameters.AddWithValue("@adminid", adminid);
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                OutStatus = (string)cmd.Parameters["@OutError"].Value;
                return result;

            }
            catch (Exception ex)
            {
                OutStatus = "-1";
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }
        #endregion For Api  Admit Card Status DB

        //----- Comman Admit Card Regular DB
        public DataSet GetCommanAdmitCardSearch(string Search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCommanAdmitCardSearch_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    //cmd.Parameters.AddWithValue("@SCHL", schlid);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }


        #region UnlockStdForm
        public DataSet UnlockStdForm(string std_id, string TeacherId, string TeacherMobile, string UnlockReason,string OTP, string SCHL, out int outstatus)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UnlockStdFormSP", con);//ReGenerateChallaanByIdSPAdminNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@std_id", std_id);
                    cmd.Parameters.AddWithValue("@TeacherId", TeacherId);
                    cmd.Parameters.AddWithValue("@TeacherMobile", TeacherMobile);
                    cmd.Parameters.AddWithValue("@UnlockReason", UnlockReason);
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@OTP", OTP);
                    cmd.Parameters.Add("@Outstatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    outstatus = (int)cmd.Parameters["@Outstatus"].Value;
                    return result;
                }
            }
            catch (Exception ex)
            {
                outstatus = -2;
                return result = null;
            }
        }




        #endregion UnlockStdForm


        public DataSet GetAllFormNameBySchl(string SCHL)
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("GetAllFormNameBySchlSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }



        public DataSet GetRegularStudentDataSearch(int type, string search, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetRegularStudentDataSearchSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }



            }
            catch (Exception ex)
            {
                return result = null;
            }
        }


        public DataSet GetStudentDataById(string studentid)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("GetStudentDataByIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@studentid", studentid);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }


        public static DataSet GetInterBoardMigrationPayFee(string RequestID)
        {
            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "GetInterBoardMigrationPayFeeSP"; 
                cmd.Parameters.AddWithValue("@RequestID", RequestID);
                ds = db.ExecuteDataSet(cmd);
                //result = db.ExecuteNonQuery(cmd);                
                return ds;

            }
            catch (Exception ex)
            {

                return null;
            }

        }




    }
}