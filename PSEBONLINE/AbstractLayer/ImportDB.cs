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

namespace PSEBONLINE.AbstractLayer
{
    public class ImportDB
    {
        #region Check ConString

        private string CommonCon = "myDBConnection";
        public ImportDB()
        {
            CommonCon = "myDBConnection";

        }
        #endregion  Check ConString


        public static List<SelectListItem> GetImportSearchListMain()
        {
            List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession.Add(new SelectListItem { Text = "Roll", Value = "1" });
            itemSession.Add(new SelectListItem { Text = "Registration Number", Value = "2" });
            //itemSession.Add(new SelectListItem { Text = "Candidate ID", Value = "3" });
            itemSession.Add(new SelectListItem { Text = "Aadhar Number", Value = "7" });
            return itemSession;
        }

        public static List<SelectListItem> GetImportSearchList()
        {
            List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession.Add(new SelectListItem { Text = "Roll", Value = "1" });
            itemSession.Add(new SelectListItem { Text = "Registration Number", Value = "2" });
            itemSession.Add(new SelectListItem { Text = "Candidate ID", Value = "3" });
            itemSession.Add(new SelectListItem { Text = "Aadhar Number", Value = "7" });
            return itemSession;
        }

        public static List<SelectListItem> GetImportSessionLast3()
        {
           List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession.Add(new SelectListItem { Text = "2023", Value = "2023" });
            itemSession.Add(new SelectListItem { Text = "2022", Value = "2022" });
            itemSession.Add(new SelectListItem { Text = "2021", Value = "2021" });
            //itemSession.Add(new SelectListItem { Text = "2020", Value = "2020" });
            return itemSession;
           
        }

        public static List<SelectListItem> GetImportSessionLast3NewReAppear()
        {
            List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession.Add(new SelectListItem { Text = "2023", Value = "2023" });            
            //itemSession.Add(new SelectListItem { Text = "2020", Value = "2020" });
            return itemSession;

        }

        public static List<SelectListItem> GetImportSessionLast2()
        {
            List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession.Add(new SelectListItem { Text = "2022", Value = "2022" });
            itemSession.Add(new SelectListItem { Text = "2021", Value = "2021" });
            //itemSession.Add(new SelectListItem { Text = "2020", Value = "2020" });
            return itemSession;
        }


        public string GetImpschlOcode(int Type1, string SCHL1)
        {
            int result;
            string result1;
            try
            {
                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetImpschlOcodeSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type1);
                    cmd.Parameters.AddWithValue("@SCHL", SCHL1);
                    SqlParameter outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = "@Outstatus";
                    outPutParameter.Size = 100;
                    outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                    outPutParameter.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(outPutParameter);
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    result1 = (string)cmd.Parameters["@Outstatus"].Value;
                    return result1;

                }
            }
            catch (Exception ex)
            {
                return result1 = null;
            }
        }

        public DataSet SelectAll9thPass(string search)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAll9thPass", con);
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
        #region E1importDB
        public DataSet SelectAll10thPass(string search, string schlID, string sesssion, int pageIndex)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    //selectallImport10thpass_self_sp
                    SqlCommand cmd = new SqlCommand("SelectAllImport10thpass_any_Sp", con);                    
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@schl", schlID);
                    cmd.Parameters.AddWithValue("@ses1", sesssion);
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
        public DataTable Import10thpass_Self(string Impschoolcode, string CurrentSchl, string chkid, string year)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Import10thpass_self_sp", con); //GetStudentPassNinthN_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);
                    cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@Ses1", year);
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

        public DataSet SelectAll10thReappear(string search, string schlID, string sesssion, int pageIndex)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //SqlCommand cmd = new SqlCommand("GetAll10thPass", con);
                    SqlCommand cmd = new SqlCommand("SelectAllImport10threappear_any_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@schl", schlID);
                    cmd.Parameters.AddWithValue("@ses1", sesssion);
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
        public DataTable Import10thReappear_AnySchl(string Impschoolcode, string CurrentSchl, string chkid, string search, string year)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("import10threappear_any_sp", con); //GetStudentPassNinthN_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);
                    cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@Ses1", year);
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
        public DataSet SelectAll10thPassAnySchool(string search, string schlID, string sesssion, int pageIndex)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //SqlCommand cmd = new SqlCommand("GetAll10thPass", con);
                    SqlCommand cmd = new SqlCommand("SelectAllImport10thpass_any_For_Reappear_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@schl", schlID);
                    cmd.Parameters.AddWithValue("@ses1", sesssion);
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

        public DataTable Import10thPass_AnySchl(string Impschoolcode, string CurrentSchl, string chkid, string year)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("import10thpass_any_sp", con); //GetStudentPassNinthN_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);
                    cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@Ses1", year);
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

        public DataSet SelectAll11thfailselfschl(string search, string schlID, string sesssion, int pageIndex)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //SqlCommand cmd = new SqlCommand("GetAll10thPass", con);
                    SqlCommand cmd = new SqlCommand("selectallImport11thfail_self_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", "2018-2019");
                    //cmd.Parameters.AddWithValue("@schl", schlID);
                    cmd.Parameters.AddWithValue("@ses1", sesssion);
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
        public DataTable Import11thfail_SelfSchl(string Impschoolcode, string CurrentSchl, string chkid, string year)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Import11thfail_self_sp", con); //GetStudentPassNinthN_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);
                    cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@Ses1", year);
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


        public DataSet SelectAll11thfailtcref(string search, string SString)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectAllImport11thfail_tcref_Sp", con); //SelectAllImport11thfail_tcref_Sp_1609 // SelectTCStudents9thPassed_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@SString", SString);

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

        public DataTable Import_TCREF_11thfail_Students(string schoolcode, string chkid, string SString, string cls)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("import11thfail_tcref_sp", con);//import11thfail_tcref_sp_1609 //GetStudent9thPassedAndContinueTC_SP_29042017
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@SString", SString);
                    cmd.Parameters.AddWithValue("@cls", cls);
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
        #endregion E1importDB
    
        
        public DataSet SelectAll10thPass1(string search, string sesssion, int pageIndex)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAll10thPass", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 10);

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

        public DataSet GetAll10thPassCount(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAll10thPassCount", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 10);
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
        public DataSet GetAll10thPassCount1(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAll10thPassCount", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 10);
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
        public DataSet AllImportDataCount(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AllImportDataCount", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 10);
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
        public DataSet GetAll11thFailedCount(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAll11thFailCount", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 10);
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
        public DataTable Select_All_Pass_Data(string schoolcode, string chkid)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudentPassMatric6Sep_29042017", con); // GetStudentPassMatric6Sep
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
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
        public DataSet SelectAllImportedData(string search, string sesssion,int pageIndex)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAllImportedData", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 10);

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
        public DataSet SearchStudentGetByData_E_Import(string sid, string frmname)
        {

          
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                   
                    SqlCommand cmd = new SqlCommand("GetStudentRecordsByID_E_Import", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@sid", sid);
                   

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

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_E1_E2Forms_Sp", con);
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


                //cmd.Parameters.AddWithValue("@District", frm["MyDistrict"].ToString());
                //cmd.Parameters.AddWithValue("@Tehsil", frm["MYTehsil"].ToString());

                ////cmd.Parameters.AddWithValue("@Tehsil", frm["MyTeh"].ToString());
                //cmd.Parameters.AddWithValue("@Tehsil", RM.Tehsil);
                ////cmd.Parameters.AddWithValue("@District", frm["MyDist"].ToString());
                //cmd.Parameters.AddWithValue("@District", RM.District);
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
                    //cmd.Parameters.AddWithValue("@AWRegisterNo", RM.AWRegisterNo);
                    //cmd.Parameters.AddWithValue("@Admission_Num", RM.Admission_Num);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsPSEBRegNum", 0);
                }
                //------------
                if (FormType == "E1" || FormType == "E2")
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
                }

                cmd.Parameters.AddWithValue("@Section", RM.Section);

                string hostName = Dns.GetHostName(); // Retrive the Name of HOST
                string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                cmd.Parameters.AddWithValue("@MyIP", myIP);

                cmd.Parameters.AddWithValue("@MetricMonth", RM.MetricMonth);
                cmd.Parameters.AddWithValue("@MetricYear", RM.MetricYear);
                cmd.Parameters.AddWithValue("@MetricRollNum", RM.Metric_Roll_Num);

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
        public string Updated_Pic_Data(string Myresult, string PhotoSignName, string Type)
        {
            SqlConnection con = null;
            string result = "";
            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_Uploaded_Photo_Sign", con);
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
        public DataSet GetStudentRecordsSearch_ImportData(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudentRecordsSearch_Impdata2017_02082016", con);
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
        public DataSet SearchStudentGetByData_E(string sid, string frmname)
        {

            // SqlConnection con = null;

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                   
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
        public string Delete_E_FromData(string stdid,string OTID,string oldid,string tcrefno)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Delete_Import_E1_E2Forms_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Std_id", stdid);
                cmd.Parameters.AddWithValue("@otid", OTID);
                cmd.Parameters.AddWithValue("@oldid", oldid);
                cmd.Parameters.AddWithValue("@tcrefno", tcrefno);
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
        //---------------Update Data---------
        public string Update_ImportData_To_E(string schoolcode, string chkid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {               
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());

                SqlCommand cmd = new SqlCommand("Update_ImportData_To_E_Sp6Sep", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                cmd.Parameters.AddWithValue("@chkid", chkid);              

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

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
        //-------------------------11th Compartment--------------------
        public DataSet SelectAll11thCompartment(string search, string sesssion, int pageIndex)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAll11THCompartment", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 10);

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

        public DataSet GetAll11thCompartmentCount(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAll11thFailCount", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 10);
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
        public DataTable Select_All_11ThCompartment_Data(string schoolcode, string chkid)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudent11ThCompartment_29042017", con);   // GetStudent11ThCompartment
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
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
        //----------------------11th Failed---------------------
        public DataSet SelectAll11thFailed(string search, string sesssion,int pageIndex)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAll11THFAILED", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 10);

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
        public DataTable Select_All_11ThFailed_Data(string schoolcode, string chkid)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudent11ThFailed6Sep_29042017", con); // GetStudent11ThFailed6Sep
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
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
        public string Update_ImportData_11Th_Failed(string schoolcode, string chkid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());

                SqlCommand cmd = new SqlCommand("Update_11Th_Failed_Sp6Sep", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                cmd.Parameters.AddWithValue("@chkid", chkid);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

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
        public string Delete_Import_11th_Failed_Data(string stdid, int OID)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Delete_Import_11Th_Failed_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Std_id", stdid);
                cmd.Parameters.AddWithValue("@oldid", OID);
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
        //-----------------------Search TC--
        public DataSet SelectTCStudents(string search, string sesssion)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SearchTCfor10thpass", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                  
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
        public DataTable Select_All_11ThFailed_11THcontinue_TC(string schoolcode, string chkid)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudent11ThFailedAndContinueTC_SP6Sep_29042017", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
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
        //---------------Update TC Data---------
        public string Update_ImportData_Of_TC_To_E(string schoolcode, string chkid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_ImportData_For_TC_To_E_Sp6Sep", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                cmd.Parameters.AddWithValue("@chkid", chkid);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;
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
        //---------------------ImportData 2017--------
        public DataSet SelectImportData2017(string search, string sesssion, int pageIndex)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetImportData2017", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 10);

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

        //--------------------------------------------Import Data For 12 Th----------------------------

        public DataSet SelectAllImport11thPassCount(string search, string sesssion, int pageIndex, string Ses1)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectAllImport11thPassCount", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
                    cmd.Parameters.AddWithValue("@Ses1", Ses1);

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
        public DataSet GetAll11thPassCount(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAll11thPassCount", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
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
        //---------------Update Data For T---------
        public string Update_ImportData_To_T(string schoolcode, string chkid, string Year)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());

                SqlCommand cmd = new SqlCommand("Update_ImportData_To_T_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                cmd.Parameters.AddWithValue("@chkid", chkid);
                cmd.Parameters.AddWithValue("@Ses1", Year);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

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
        //-----------------------12th Fail-------------

      
        public DataSet SelAllImport12thFail_OLD(string search, string sesssion, int pageIndex, string Ses1)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelAllImport12thFail_Sp", con);  
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
                    cmd.Parameters.AddWithValue("@Ses1", Ses1);

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
        public DataSet SelAllImport12thFail_RAN(string search, string sesssion, int pageIndex, string Ses1 ,string schl, string sstring )
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelAllImport12thFail_Sp_RAN", con);   // SelAllImport12thFail_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
                    cmd.Parameters.AddWithValue("@Sstring", sstring);
                    cmd.Parameters.AddWithValue("@SCHID", schl);

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

        //------------------Update Data 12th Fail
        public string Update_ImportData_To_T_12thFail(string schoolcode, string chkid, string Ses1)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());

                SqlCommand cmd = new SqlCommand("Update_ImportData_To_T_12thFail_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                cmd.Parameters.AddWithValue("@chkid", chkid);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

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
        public DataTable GetStudent12thFail(string schoolcode, string chkid, string year)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudent12thFail_sp_29042017", con); //GetStudent12thFail_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    //cmd.Parameters.AddWithValue("@Ses1", year);
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
        //-----------------------Search TC For 11th Pass--
        public string Update_ImportData_Of_TC_To_T(string schoolcode, string chkid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_ImportData_For_TC_To_T_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                cmd.Parameters.AddWithValue("@chkid", chkid);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;
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
        //------------------------------Import 11th Pass Other School----------
        public DataSet SelectAllImport11thPassOtherSchool(string search,string Search1, string sesssion, string sesssion1)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectAllImport11thPassOtherSchool_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@search1", Search1);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@ses1", sesssion1);

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
        public DataTable Select_All_11ThPassed_Other_School(string schoolcode, string chkid, string ses)
        {
            //-------------jnana----------


            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Select_All_11ThPassed_Other_School_SP_29042017", con); //Select_All_11ThPassed_Other_School_SP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
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

        public string Update_ImportData_Of_11ThPassed_Other_School(string schoolcode, string chkid, string Ses)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_ImportData_Of_11ThPassed_Other_School_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                cmd.Parameters.AddWithValue("@chkid", chkid);
                cmd.Parameters.AddWithValue("@ses", Ses);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;
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
        public DataSet ChekResultCompairSubjects(string schoolcode, string chkSub)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("T2SubjectCompair", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schoolid", schoolcode);
                    cmd.Parameters.AddWithValue("@stud_sub", chkSub);
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


        //--------------------------------------------Import Data For 11 Pass----------------------------

        #region Search TC For 11th Pass
        //-----------------------Search TC For 11th Pass--
        public DataSet SelectTCStudents9thPassed(string search, string SearchString,string SelectedSession)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

               using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectTCStudents9thPassed_SP_New", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@SearchString", SearchString);
                    cmd.Parameters.AddWithValue("@SelectedSession", SelectedSession);
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
        public DataTable Select_All_9ThPassed_Continue_TC(string Impschoolcode, string CurrentSchl, string chkid, string SearchString,string SelectedSession)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Select_All_9ThPassed_Continue_TC_new", con); //GetStudent9thPassedAndContinueTC_SP_RAN
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);
                    cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@SearchString", SearchString);
                    cmd.Parameters.AddWithValue("@SelectedSession", SelectedSession);
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
        public string Update_ImportData_Of_TC_To_M(string schoolcode, string chkid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_ImportData_For_TC_To_M_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                cmd.Parameters.AddWithValue("@chkid", chkid);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;
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
        public DataSet SelectAllImport9thPassOtherSchool(string search, string search1, string sesssion,string ses1)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectAllImport9thPassOtherSchool_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@search1", search1);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@ses1", ses1);

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
        public DataTable Select_All_9ThPassed_Other_School(string schoolcode, string chkid, string ses)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                 using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Select_All_9ThPassed_Other_School_SP_29042017", con); // Select_All_9ThPassed_Other_School_SP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
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
        public string Update_ImportData_Of_9ThPassed_Other_School(string schoolcode, string chkid, string Ses)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_ImportData_Of_9ThPassed_Other_School_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                cmd.Parameters.AddWithValue("@chkid", chkid);
                cmd.Parameters.AddWithValue("@ses", Ses);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;
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
        public string insertinbulkexammasterregular2017(string storeid, string storescid, string storeaashirwardno, string IsWantselfcenter)
        {
            if (IsWantselfcenter.Length > 0)
            {
                IsWantselfcenter = IsWantselfcenter.Substring(0, IsWantselfcenter.Length - 1);
            } 
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("insertinbulkexammasterregular2017NEw", con);  //insertinbulkexammasterregular2017
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@storeid", storeid);
                cmd.Parameters.AddWithValue("@storescid", storescid);
                cmd.Parameters.AddWithValue("@IsWantselfcenter", IsWantselfcenter);
                cmd.Parameters.AddWithValue("@storeaashirwardno", storeaashirwardno);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                //con.Open();
                //result = cmd.ExecuteNonQuery().ToString();
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
        public void CheckDuplicateAashirwardNumber(string storeaashirwardno, string storeid, out string duplicateaashirwardno, out string duplicateid, int ClassNo)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("DummyCheckDuplicateAashirwardNumber", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@storeaashirwardno", storeaashirwardno);
                cmd.Parameters.AddWithValue("@storeid", storeid);
                cmd.Parameters.AddWithValue("@ClassNo", ClassNo);
                cmd.Parameters.Add("@duplicateaashirwardno", SqlDbType.NVarChar, 3550).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@duplicatreturnedid", SqlDbType.NVarChar, 3550).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                duplicateaashirwardno = Convert.ToString(cmd.Parameters["@duplicateaashirwardno"].Value);
                duplicateid = Convert.ToString(cmd.Parameters["@duplicatreturnedid"].Value);
                //con.Open();
                //result = cmd.ExecuteNonQuery().ToString();
                //return result;

            }
            catch (Exception ex)
            {
                duplicateaashirwardno = "No";
                duplicateid = "No";
                //mbox(ex);
                // return result = "";
            }
            finally
            {
                con.Close();
            }
        }

        public string resendinsertinbulkexammasterregular2017(string storeid, string storescid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("resendinsertinbulkexammasterregular2017", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@storeid", storeid);
                cmd.Parameters.AddWithValue("@storescid", storescid);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                //con.Open();
                //result = cmd.ExecuteNonQuery().ToString();
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


        public string insertinbulkexammasterregular2017NEwOpen(int class1, string storeid, string storescid, string storeaashirwardno)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("insertinbulkexammasterregular2017NEwOpen", con);  //insertinbulkexammasterregular2017
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@storeid", storeid);
                cmd.Parameters.AddWithValue("@storescid", storescid);
                cmd.Parameters.AddWithValue("@storeaashirwardno", storeaashirwardno);
                cmd.Parameters.AddWithValue("@class", class1);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                //con.Open();
                //result = cmd.ExecuteNonQuery().ToString();
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


        public string resendinsertinbulkexammasterregular2017Open(string storeid, string storescid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("resendinsertinbulkexammasterregular2017Open", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@storeid", storeid);
                cmd.Parameters.AddWithValue("@storescid", storescid);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                //con.Open();
                //result = cmd.ExecuteNonQuery().ToString();
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

        #endregion Search TC For 11th Pass

        #region  ImportData N3Form 8thPass
        public DataSet SelectAllImportN3Form8thPass(string SessionYear, string schl, int pageIndex, string search)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectAllImportN3Form8thPassSP", con); //GetAllImport9thFailandAbsent
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SessionYear", SessionYear);
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 20);                    
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

        public DataTable ImportN3Form8thPass(string SessionYear, string Impschoolcode, string CurrentSchl, string chkid, string year)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ImportN3Form8thPassSP", con); //GetStudentPassNinthN_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SessionYear", SessionYear);
                    cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);             
                    cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@Ses1", year);
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

        #endregion  ImportData N3Form 8thPass

        #region Import N3Data
        //--------------------------Import N3 Form Data-----------------------//
        public DataSet SelectAllImportN3thPass(string search, string sesssion, int pageIndex, string Ses1)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectAllImportN3thPassAbsent_Sp", con); //GetAllImport9thFailandAbsent
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
                    cmd.Parameters.AddWithValue("@Ses1", Ses1);

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
        public DataTable Import_All_Pass_Absent_N3_Data(string Impschoolcode, string CurrentSchl, string chkid, string year)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Import_All_Pass_Absent_N3_Data_Sp", con); //GetStudentPassNinthN_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);
                    cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@Ses1", year);
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

        //----TCREF---
        public DataSet SelectTCREFN3Students(string search, string SString)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectTCREFN3Students_sp", con); // SelectTCStudents9thPassed_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@SString", SString);

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
        public DataTable Import_TCREF_N3_Students(string schoolcode, string chkid, string SString,string cls)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Import_TCREF_N3_Students_SP", con); //GetStudent9thPassedAndContinueTC_SP_29042017
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@SString", SString);
                    cmd.Parameters.AddWithValue("@cls", cls);
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
        //---End TCREF
        //--------------End N3FormData-----------------------------------------//
        #endregion Import N3Data


        //--------------------------------------------Import Data For 9th Or M1 Master--------------------------

        #region Import Data For 9th Or M1 Master  

        public DataSet SelectAllImport9thPassCount(string search, string sesssion, int pageIndex, string Ses1)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectAllImport9thPassCount_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
                    cmd.Parameters.AddWithValue("@Ses1", Ses1);

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

        public DataSet SelectAllImport9thPass(string search, string sesssion, int pageIndex, string Ses1)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAllImport9thPass", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 20);
                    cmd.Parameters.AddWithValue("@Ses1", Ses1);

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
        public DataTable Import9thPassSP(string Impschoolcode, string CurrentSchl, string chkid, string year)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Import9thPassSP", con); //GetStudentPassNinthN_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);
                    cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@Ses1", year);
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

        public DataSet GetAllImport9thReappear(string search, string sesssion, int pageIndex, string Ses1)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAllImport9thReappear", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
                    cmd.Parameters.AddWithValue("@Ses1", Ses1);

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
        public DataTable ImportStudent9thReappear(string Impschoolcode, string CurrentSchl, string chkid, string year)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ImportStudent9thReappear", con); //GetStudent9PassReappear
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);
                    cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@Ses1", year);
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

        public DataSet SelectTCStudents9thReappear(string search, string SearchString)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectTCStudents9thReappear", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@SearchString", SearchString);
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

        public DataTable ImportTCStudent9thReappear(string Impschoolcode, string CurrentSchl, string chkid, string SearchString)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ImportTCStudent9thReappear", con); //ImportTCStudent9thReappear
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);
                    cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@SearchString", SearchString);
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

        public string Update_ImportData_To_N(string schoolcode, string chkid, string Year)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());

                SqlCommand cmd = new SqlCommand("Update_ImportData_To_N_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                cmd.Parameters.AddWithValue("@chkid", chkid);
                cmd.Parameters.AddWithValue("@Ses1", Year);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

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
        public DataSet SelAllImport10thFail(string schl, int pageIndex, string Ses1, string search)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelAllImport10thFail_SpN1_V", con); //SelAllImport10thFail_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 20);
                    cmd.Parameters.AddWithValue("@Ses1", Ses1);
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
        public DataTable ImportStudent10thFail(string Impschoolcode, string CurrentSchl, string chkid, string year)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ImportStudent10thFailN1_V", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);
                    cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@Ses1", year);
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

        public string Update_ImportData_To_T_10thFail(string schoolcode, string chkid, string Ses1)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());

                SqlCommand cmd = new SqlCommand("Update_ImportData_To_T_10thFail_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                cmd.Parameters.AddWithValue("@chkid", chkid);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;

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
	    public DataSet SelAllImport10thFail_NTE(string schl, int pageIndex, string Ses1, string search)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelAllImport10thFail_SpN1_NTE", con); //SelAllImport10thFail_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 20);
                    cmd.Parameters.AddWithValue("@Ses1", Ses1);
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

        public DataTable ImportStudent10thFail_NTE(string Impschoolcode, string CurrentSchl, string chkid, string year)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ImportStudent10thFailN1_NTE", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);
                    cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@Ses1", year);
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

        #endregion Import Data For 9th Or M1 Master

        #region Import Data For T1

        #region 11th pass self
        public DataSet SelectAllImport11thPass(string search, string sesssion, int pageIndex, string Ses1)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAllImport11thPassNew", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 20);
                    cmd.Parameters.AddWithValue("@Ses1", Ses1);

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
        public DataTable Import11thPass(string Impschoolcode, string CurrentSchl, string chkid, string year)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Import11thPass", con); // GetStudentPassElevenT_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);
                    cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@Ses1", year);
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
        #endregion 11th pass self

        #region 12th fail

        public DataSet SelAllImport12thFail(string schl, int pageIndex, string Ses1, string search)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelAllImport12thFail_SpN1_V", con);//SelAllImport12thFail_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 20);
                    cmd.Parameters.AddWithValue("@Ses1", Ses1);
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
        public DataTable ImportStudent12thFail(string Impschoolcode, string CurrentSchl, string chkid, string year)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ImportStudent12thFailN1_V", con);//ImportStudent12thFail
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);
                    cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@Ses1", year);
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

        #endregion 12th fail

        #region Import11thPassedTCRef

        public DataSet SelectTCStudents11thPassed(string search, string SearchString)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectTCStudents11thPassed_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@SearchString", SearchString);

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


        public DataTable Import11thPassedTCRef(string Impschoolcode, string CurrentSchl, string chkid, string SearchString)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Import11thPassedTCRef", con); //-------GetStudent12ThFailedAndContinueTC_SP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);
                    cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@SearchString", SearchString);
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
		
        #endregion Import11thPassedTCRef
        public DataSet SelAllImport12thFail_NTE(string schl, int pageIndex, string Ses1, string search)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelAllImport12thFail_SpN1_NTE", con); //SelAllImport10thFail_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 20);
                    cmd.Parameters.AddWithValue("@Ses1", Ses1);
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

        public DataTable ImportStudent12thFail_NTE(string Impschoolcode, string CurrentSchl, string chkid, string year)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ImportStudent12thFailN1_NTE", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);
                    cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@Ses1", year);
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

        #endregion Import Data For T1

        #region Import9thPassedTCRef
        public DataSet SelectImport9thPassedTCRef(string search, string SString)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectImport9thPassedTCRef_sp", con); // SelectTCStudents9thPassed_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@SString", SString);

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
        public DataTable Import9thPassedTCRef(string schoolcode, string chkid, string SString, string cls)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Import9thPassedTCRef_SP", con); //GetStudent9thPassedAndContinueTC_SP_29042017
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@SString", SString);
                    cmd.Parameters.AddWithValue("@cls", cls);
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
        #endregion Import9thPassedTCRef

        #region Import11thPassedTCRef
        public DataSet SelectImport11thPassedTCRef_N(string search, string SString)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectImport11thPassedTCRef_Sp", con); //SelectAllImport11thfail_tcref_Sp_1609 // SelectTCStudents9thPassed_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@SString", SString);

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
    
        public DataTable Import11thPassedTCRef_N(string schoolcode, string chkid, string SString, string cls)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Import11thPassedTCRef_sp", con);//import11thfail_tcref_sp_1609 //GetStudent9thPassedAndContinueTC_SP_29042017
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
                    cmd.Parameters.AddWithValue("@chkid", chkid);
                    cmd.Parameters.AddWithValue("@SString", SString);
                    cmd.Parameters.AddWithValue("@cls", cls);
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

        #endregion Import11thPassedTCRef


    }
}