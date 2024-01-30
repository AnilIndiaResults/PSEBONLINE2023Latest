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
using System.Net.Mail;
using System.Collections;
using System.Data.Odbc;
using System.Text;

namespace PSEBONLINE.AbstractLayer
{
    public class OpenDB
    {
        SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myDBConnection"].ConnectionString);
        SqlCommand cmd = new SqlCommand();
        private DBContext _context = new DBContext();

        // 

        #region Check ConString

        private string CommonCon = "myDBConnection";
        public OpenDB()
        {
            CommonCon = "myDBConnection";
        }
        #endregion  Check ConString
        //



        //UploadPhotoSign(app_id, imgSign, imgPhoto)
        #region UploadPhotoSign      
        public int CorrectEntryOpen(string id)
        {
            // SqlConnection con = null;
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("CorrectEntryOpenSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);                   
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    con.Open();
                    if (ds.Tables[0].Rows.Count >= 1)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                return -1 ;
            }
        }


        public int UploadPhotoSignOpen(string app_id, string imgSign, string imgPhoto)
        {
            string OutError = "";
            int result = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UploadPhotoSignOpen", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@app_id", app_id);
                    cmd.Parameters.AddWithValue("@imgSign", imgSign);
                    cmd.Parameters.AddWithValue("@imgPhoto", imgPhoto);                 
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;                                
                    try
                    {
                        con.Open();
                        result =  cmd.ExecuteNonQuery();
                        con.Close();
                        return 1;
                    }
                    catch (Exception e)
                    {
                        return 0;
                    }
                }


            }
            catch (Exception ex)
            {
                OutError = "";
                return 0;
            }
        }

        #endregion UploadPhotoSign

        public DataSet CheckSubMaster_MINMAX(string cls, string YEAR, string SUB, string MIN, string MAX, string search)
        {
            string OutError="";
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CheckSubMaster_MINMAX", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CLASS", cls);
                    cmd.Parameters.AddWithValue("@YEAR", YEAR);
                    cmd.Parameters.AddWithValue("@SUB", SUB);
                    cmd.Parameters.AddWithValue("@MIN", MIN);
                    cmd.Parameters.AddWithValue("@MAX", MAX);
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                    return result;
                }
            }
            catch (Exception ex)
            {
                OutError = "";
                return result = null;
            }
        }

        public DataSet GetSchoolRecords(string dist)
        {
            cmd.CommandText = "sp_GetSchoolRecords";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@dist", dist);
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            return ds;
        }


      

        public DataSet GetChallanDetailsById(string ChallanId)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetChallanDetailsByIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CHALLANID", ChallanId);
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

        public List<SelectListItem> GetAdminDistrict(string districts)
        {
            cmd.CommandText = "sp_GetAdminDistrict";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@districts", districts);
            List<SelectListItem> distList = new List<SelectListItem>();
            distList.Add(new SelectListItem { Text = "---Select District---", Value = "0" });
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string txt = dr["DISTNM"].ToString();
                string val = dr["DIST"].ToString();

                distList.Add(new SelectListItem() { Text = txt, Value = val });
            }
            return distList;
        }

        public List<SelectListItem> GetYears()
        {
            List<SelectListItem> years = new List<SelectListItem>();
            //years.Add(new SelectListItem() { Text = "--Select--", Value = "" });
            for (int i = DateTime.Now.Year; i > 1959; i--)
            {
                years.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            return years;
        }

        public List<SelectListItem> GetMonths()
        {
            List<SelectListItem> months = new List<SelectListItem>();
            months.Add(new SelectListItem() { Text = "--Select--", Value = "" });
            months.Add(new SelectListItem() { Text = "January", Value = "January" });
            months.Add(new SelectListItem() { Text = "February", Value = "February" });
            months.Add(new SelectListItem() { Text = "March", Value = "March" });
            months.Add(new SelectListItem() { Text = "April", Value = "April" });
            months.Add(new SelectListItem() { Text = "May", Value = "May" });
            months.Add(new SelectListItem() { Text = "June", Value = "June" });
            months.Add(new SelectListItem() { Text = "July", Value = "July" });
            months.Add(new SelectListItem() { Text = "August", Value = "August" });
            months.Add(new SelectListItem() { Text = "September", Value = "September" });
            months.Add(new SelectListItem() { Text = "October", Value = "October" });
            months.Add(new SelectListItem() { Text = "November", Value = "November" });
            months.Add(new SelectListItem() { Text = "December", Value = "December" });

            return months;
        }

        public List<SelectListItem> GetGenders()
        {
            List<SelectListItem> genders = new List<SelectListItem>();
            genders.Add(new SelectListItem() { Text = "--Select--", Value = "" });
            genders.Add(new SelectListItem() { Text = "Female", Value = "female" });
            genders.Add(new SelectListItem() { Text = "Male", Value = "male" });
            return genders;
        }

        public List<SelectListItem> GetStreams_1()
        {
            List<SelectListItem> streamsList = new List<SelectListItem>();
            streamsList.Add(new SelectListItem { Text = "--Select--", Value = "" });
			//streamsList.Add(new SelectListItem { Text = "GENERAL", Value = "G" });
            streamsList.Add(new SelectListItem { Text = "HUMANITIES", Value = "H" });
            streamsList.Add(new SelectListItem { Text = "SCIENCE", Value = "S" });
            streamsList.Add(new SelectListItem { Text = "COMMERCE", Value = "C" });
            streamsList.Add(new SelectListItem { Text = "INTIGRATED GROUP", Value = "IG" });
            return streamsList;
        }

        public List<SelectListItem> GetStreams_2()
        {
            List<SelectListItem> streamsList = new List<SelectListItem>();
            streamsList.Add(new SelectListItem { Text = "--Select--", Value = "" });
            streamsList.Add(new SelectListItem { Text = "HUMANITIES", Value = "H" });
            streamsList.Add(new SelectListItem { Text = "COMMERCE", Value = "C" });
            streamsList.Add(new SelectListItem { Text = "INTIGRATED GROUP", Value = "IG" });
            return streamsList;
        }

        public List<SelectListItem> GetStreams12_1()
        {
            List<SelectListItem> streamsList = new List<SelectListItem>();
            streamsList.Add(new SelectListItem { Text = "--Select--", Value = "" });
            streamsList.Add(new SelectListItem { Text = "HUMANITIES", Value = "HUMANITIES" });
            streamsList.Add(new SelectListItem { Text = "SCIENCE", Value = "SCIENCE" });
            streamsList.Add(new SelectListItem { Text = "COMMERCE", Value = "COMMERCE" });
            streamsList.Add(new SelectListItem { Text = "INTIGRATED GROUP", Value = "INTIGRATED GROUP" });
            return streamsList;
        }

        public List<SelectListItem> GetStreams12_2()
        {
            List<SelectListItem> streamsList = new List<SelectListItem>();
            streamsList.Add(new SelectListItem { Text = "--Select--", Value = "" });
            streamsList.Add(new SelectListItem { Text = "HUMANITIES", Value = "HUMANITIES" });
            streamsList.Add(new SelectListItem { Text = "COMMERCE", Value = "COMMERCE" });
            streamsList.Add(new SelectListItem { Text = "INTIGRATED GROUP", Value = "INTIGRATED GROUP" });
            return streamsList;
        }

        public List<SelectListItem> GetMedium()
        {
            List<SelectListItem> TM = new List<SelectListItem>();
            TM.Add(new SelectListItem { Text = "PUNJABI", Value = "PUNJABI" });
            TM.Add(new SelectListItem { Text = "HINDI", Value = "HINDI" });
            TM.Add(new SelectListItem { Text = "ENGLISH", Value = "ENGLISH" });

            return TM;
        }

        public List<SelectListItem> GetStreamDistrict(string strm)
        {
            List<SelectListItem> StrmTehList = new List<SelectListItem>();
            cmd.CommandText = "sp_Stream_District";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;

            switch (strm)
            {
                case "H": cmd.Parameters.AddWithValue("@strm", "OHUM"); break;
                case "S": cmd.Parameters.AddWithValue("@strm", "OSCI"); break;
                case "C": cmd.Parameters.AddWithValue("@strm", "OCOMM"); break;
                case "IG": cmd.Parameters.AddWithValue("@strm", "OHUM,OSCI"); break;
                default: cmd.Parameters.AddWithValue("@strm", "OMATRIC"); break;
            }
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                StrmTehList.Add(new SelectListItem { Text = "--Select--", Value = "" });
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    StrmTehList.Add(new SelectListItem() { Text = dr["DISTNM"].ToString(), Value = dr["DIST"].ToString() });
                }
            }
            else
            {
                StrmTehList.Add(new SelectListItem() { Text = "No District", Value = "" });
            }

            return StrmTehList;
        }

        public List<SelectListItem> GetStreamTehsil(string dist, string strm)
        {
            cmd.Parameters.Clear();
            List<SelectListItem> StrmTehList = new List<SelectListItem>();
            cmd.CommandText = "sp_Stream_Tehsil";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            if (dist.Length == 2)
            {
                dist = "0" + dist;
            }
            cmd.Parameters.AddWithValue("@dist", dist);
            switch (strm)
            {
                case "H": cmd.Parameters.AddWithValue("@strm", "OHUM"); break;
                case "S": cmd.Parameters.AddWithValue("@strm", "OSCI"); break;
                case "C": cmd.Parameters.AddWithValue("@strm", "OCOMM"); break;
                case "IG": cmd.Parameters.AddWithValue("@strm", "OHUM,OSCI"); break;
                default: cmd.Parameters.AddWithValue("@strm", "OMATRIC"); break;
            }
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            try
            {
                adp.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    StrmTehList.Add(new SelectListItem { Text = "--Select--", Value = "" });
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        StrmTehList.Add(new SelectListItem() { Text = dr["TEHSIL"].ToString(), Value = dr["TCODE"].ToString() });
                    }
                }
                else
                {
                    StrmTehList.Add(new SelectListItem() { Text = "NO TEHSIL", Value = "" });
                }
            }
            catch (Exception e)
            {
                StrmTehList.Add(new SelectListItem() { Text = "NO TEHSIL", Value = "" });
            }


            return StrmTehList;
        }

        public List<SelectListItem> GetTCategories()
        {
            List<SelectListItem> catgilist = new List<SelectListItem>();
            catgilist.Add(new SelectListItem { Text = "--Select--", Value = "" });
            catgilist.Add(new SelectListItem { Text = "10th PASSED", Value = "10th PASSED" });
            catgilist.Add(new SelectListItem { Text = "11th PASSED", Value = "11th PASSED" });
            catgilist.Add(new SelectListItem { Text = "11th FAIL", Value = "11th FAIL" });
            catgilist.Add(new SelectListItem { Text = "12th FAIL (Open School-AllGroups)", Value = "12th FAIL (Open School-AllGroups)" });
            catgilist.Add(new SelectListItem { Text = "12th FAIL (NIOS-All Groups)", Value = "12th FAIL (NIOS-All Groups)" });
            catgilist.Add(new SelectListItem { Text = "12th FAIL (Regular School-Science Group)", Value = "12th FAIL (Regular School-Science Group)" });
            catgilist.Add(new SelectListItem { Text = "12th FAIL (Regular School-Other Groups)", Value = "12th FAIL (Regular School-Other Groups)" });
            return catgilist;
        }

        public OpenUserLogin GetAdminDetailsById(int adminid)
        {
            if (adminid != 0)
            {
                cmd.CommandText = "sp_GetLoginOpen ";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                string search = "where id = " + adminid;
                cmd.Parameters.AddWithValue("@search", search);
                DataSet ds = new DataSet();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(ds);
                try
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        OpenUserLogin _openUserLogin = new OpenUserLogin();
                        DataRow dr = ds.Tables[0].Rows[0];
                        _openUserLogin.AADHAR_NO = dr["AADHAR_NO"].ToString();
                        _openUserLogin.ADDRESS = dr["ADDRESS"].ToString();
                        _openUserLogin.ADMINUSER = (float)Convert.ToDecimal(dr["ADMINUSER"].ToString());
                        _openUserLogin.APPNO = Convert.ToInt64(dr["APPNO"].ToString());
                        _openUserLogin.BLOCK = dr["BLOCK"].ToString();
                        _openUserLogin.CATEGORY = dr["CATEGORY"].ToString();
                        _openUserLogin.CHALLANDT = Convert.ToDateTime(dr["CHALLANDT"].ToString());
                        if (dr["CHALLANFLA"].ToString() == "true")
                        {
                            _openUserLogin.CHALLANFLA = 1;
                        }
                        else
                        {
                            _openUserLogin.CHALLANFLA = 0;
                        }
                        _openUserLogin.CLASS = dr["CLASS"].ToString();
                        _openUserLogin.correctionid = dr["correctionid"].ToString();
                        _openUserLogin.correction_dt = Convert.ToDateTime(dr["correction_dt"].ToString());
                        _openUserLogin.DIST = dr["DIST"].ToString();
                        _openUserLogin.DISTNME = dr["DISTNME"].ToString();
                        _openUserLogin.DOB = Convert.ToDateTime(dr["DOB"]).ToString("dd-MM-yyyy");
                        _openUserLogin.DOC_A_RAND = dr["DOC_A_RAND"].ToString();
                        _openUserLogin.DOC_B_RAND = dr["DOC_B_RAND"].ToString();
                        _openUserLogin.DOC_C_RAND = dr["DOC_C_RAND"].ToString();
                        _openUserLogin.DOWNLOADDA = Convert.ToDateTime(dr["DOWNLOADDA"].ToString());
                        if (dr["DOWNLOADFL"].ToString() == "true")
                        {
                            _openUserLogin.DOWNLOADFL = 1;
                        }
                        else
                        {
                            _openUserLogin.DOWNLOADFL = 0;
                        }
                        _openUserLogin.EMAILID = dr["EMAILID"].ToString();
                        _openUserLogin.FLG_DIST = dr["FLG_DIST"].ToString();
                        _openUserLogin.FORM = dr["FORM"].ToString();
                        _openUserLogin.HOMEDIST = dr["HOMEDIST"].ToString();
                        _openUserLogin.HOMEDISTNM = dr["HOMEDISTNM"].ToString();
                        _openUserLogin.ID = Convert.ToInt32(dr["ID"].ToString());
                        _openUserLogin.IMGSIGN_RA = dr["IMGSIGN_RA"].ToString();
                        _openUserLogin.IMG_RAND = dr["IMG_RAND"].ToString();
                        _openUserLogin.INSERTDT = Convert.ToDateTime(dr["INSERTDT"].ToString());
                        if (dr["ISCOMPLETE"].ToString() == "true")
                        {
                            _openUserLogin.ISCOMPLETE = 1;
                        }
                        else
                        {
                            _openUserLogin.ISCOMPLETE = 0;
                        }
                        if (dr["ISSCHLCHOO"].ToString() == "true")
                        {
                            _openUserLogin.ISSCHLCHOO = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSCHLCHOO = 0;
                        }
                        if (dr["ISSTEP1"].ToString() == "true")
                        {
                            _openUserLogin.ISSTEP1 = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSTEP1 = 0;
                        }

                        if (dr["ISSTEP2"].ToString() == "true")
                        {
                            _openUserLogin.ISSTEP2 = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSTEP2 = 0;
                        }
                        _openUserLogin.ISSTEP1DT = Convert.ToDateTime(dr["ISSTEP1DT"].ToString());

                        if (dr["ISSTEP2B"].ToString() == "true")
                        {
                            _openUserLogin.ISSTEP2B = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSTEP2B = 0;
                        }

                        if (dr["ISSUBJECT"].ToString() == "true")
                        {
                            _openUserLogin.ISSUBJECT = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSUBJECT = 0;
                        }
                        _openUserLogin.ISSTEP2DT = Convert.ToDateTime(dr["ISSTEP2DT"].ToString());
                        _openUserLogin.LANDMARK = dr["LANDMARK"].ToString();
                        _openUserLogin.MOBILENO = dr["MOBILENO"].ToString();
                        _openUserLogin.MODIFYBY = dr["MODIFYBY"].ToString();
                        _openUserLogin.MODIFYDT = dr["MODIFYDT"].ToString();
                        _openUserLogin.NAME = dr["NAME"].ToString();
                        _openUserLogin.PINCODE = dr["PINCODE"].ToString();
                        _openUserLogin.PNAME = dr["PNAME"].ToString();
                        _openUserLogin.PWD = dr["PWD"].ToString();
                        _openUserLogin.RDATE = dr["RDATE"].ToString();
                        _openUserLogin.RECEIVEFLA = (float)Convert.ToDecimal(dr["RECEIVEFLA"].ToString());
                        _openUserLogin.REGDATE = dr["REGDATE"].ToString();
                        _openUserLogin.REMARK = dr["REMARK"].ToString();
                        _openUserLogin.SCHL = dr["SCHL"].ToString();
                        _openUserLogin.SCHOOLE = dr["SCHOOLE"].ToString();
                        _openUserLogin.STREAM = dr["STREAM"].ToString();
                        _openUserLogin.STREAMCODE = dr["STREAMCODE"].ToString();
                        _openUserLogin.TEHSIL = dr["TEHSIL"].ToString();
                        _openUserLogin.TOKENNO = dr["TOKENNO"].ToString();
                        _openUserLogin.UPDT = Convert.ToDateTime(dr["UPDT"].ToString());

                        return _openUserLogin;
                    }
                    else
                    {
                        return new OpenUserLogin();
                    }
                }
                catch (Exception e)
                {

                }
            }
            return new OpenUserLogin();
        }

        public List<SelectListItem> GetN2Board()
        {
            List<SelectListItem> BoardN2List = new List<SelectListItem>();
            BoardN2List.Add(new SelectListItem { Text = "---Select Board--", Value = "0" });
            BoardN2List.Add(new SelectListItem { Text = "P.S.E.B BOARD", Value = "P.S.E.B BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "CBSE BOARD", Value = "CBSE BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "NIOS BOARD", Value = "NIOS BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "I.C.S.E BOARD", Value = "ICSE BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "HARYANA BOARD", Value = "HARYANA BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "HIMACHAL BOARD", Value = "HIMACHAL BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "J&K BOARD", Value = "J&K BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "RAJASTHAN BOARD", Value = "RAJASTHAN BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "OTHER BOARD", Value = "OTHER BOARD" });
            return BoardN2List;
        }

        //public List<SelectListItem> GetMCategories()
        //{
        //    List<SelectListItem> catgilist = new List<SelectListItem>();
        //    catgilist.Add(new SelectListItem { Text = "--Select--", Value = "" });
        //    catgilist.Add(new SelectListItem { Text = "9th COMPARTMENT", Value = "9th_COMPARTMENT" });
        //    catgilist.Add(new SelectListItem { Text = "9TH PASSED", Value = "9TH_PASSED" });
        //    catgilist.Add(new SelectListItem { Text = "10TH FAILED", Value = "10TH_FAILED" });
        //    catgilist.Add(new SelectListItem { Text = "10TH REAPPEAR", Value = "10TH_REAPPEAR" });
        //    catgilist.Add(new SelectListItem { Text = "10TH ABSENT", Value = "10TH_ABSENT" });

        //    return catgilist;
        //}

        public List<SelectListItem> GetMCategories()
        {
            List<SelectListItem> catgilist = new List<SelectListItem>();
            catgilist.Add(new SelectListItem { Text = "--Select--", Value = "" });
            catgilist.Add(new SelectListItem { Text = "8th Passed", Value = "8th Passed" });
            catgilist.Add(new SelectListItem { Text = "9th Failed", Value = "9th Failed" });
            catgilist.Add(new SelectListItem { Text = "9th Passed", Value = "9th Passed" });
            catgilist.Add(new SelectListItem { Text = "10th Fail (Open School)", Value = "10th Fail (Open School)" });
            catgilist.Add(new SelectListItem { Text = "10th Fail (NIOS)", Value = "10th Fail (NIOS)" });
            catgilist.Add(new SelectListItem { Text = "10th Fail (Regular School)", Value = "10th Fail (Regular School)" });
            catgilist.Add(new SelectListItem { Text = "Direct 14 Year Age", Value = "Direct 14 Year Age" });

            return catgilist;
        }

        public List<SelectListItem> GetDistrict()
        {
            cmd.CommandText = "sp_GetDistrict";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.Clear();
            List<SelectListItem> distList = new List<SelectListItem>();
            distList.Add(new SelectListItem { Text = "---Select District---", Value = "0" });
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string txt = dr["DISTNM"].ToString();
                string val = dr["DIST"].ToString();

                distList.Add(new SelectListItem() { Text = txt, Value = val });
            }
            return distList;
        }

        public OpenUserRegistration GetRegistrationRecord(string app_id)
        {
            cmd.CommandText = "sp_GetRegistrationOpen";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            string search = " where appno = " + app_id;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@search", search);
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            try
            {
                OpenUserRegistration _openUserRegistration = new OpenUserRegistration();
                if (ds.Tables[0].Rows.Count >= 1)
                {
                    DataRow dr = ds.Tables[0].Rows[0];
                    _openUserRegistration.DisabilityPercent = Convert.ToInt32(dr["DisabilityPercent"]);// Add in 2023-24 by Rohit
                    _openUserRegistration.AppearingYear = dr["AppearingYear"].ToString();// Add in 2023-24 by Rohit
                    //
                    _openUserRegistration.APPNO = dr["APPNO"].ToString();
                    _openUserRegistration.AADHAR_NO = dr["AADHAR_NO"].ToString();
                    _openUserRegistration.BOARD = dr["BOARD"].ToString();
                    _openUserRegistration.CandStudyMedium = dr["CandStudyMedium"].ToString();
                    _openUserRegistration.CASTE = dr["CASTE"].ToString();
                    _openUserRegistration.CAT = dr["CAT"].ToString();
                    _openUserRegistration.CLASS = dr["CLASS"].ToString();
                    _openUserRegistration.correctionid = dr["correctionid"].ToString();
                    _openUserRegistration.correction_dt = Convert.ToDateTime(dr["correction_dt"].ToString());
                    _openUserRegistration.DIST = dr["DIST"].ToString();
                    _openUserRegistration.DOB = dr["DOB"].ToString();
                    _openUserRegistration.emr17flag = Convert.ToInt32(dr["emr17flag"].ToString());
                    _openUserRegistration.EXAM = dr["EXAM"].ToString();
                    _openUserRegistration.FEE_EXMPT = (float)Convert.ToDecimal(dr["FEE_EXMPT"].ToString());
                    _openUserRegistration.FLG_DIST = dr["FLG_DIST"].ToString();
                    _openUserRegistration.FNAME = dr["FNAME"].ToString();
                    _openUserRegistration.FORM = dr["FORM"].ToString();
                    _openUserRegistration.FORMNO = dr["FORMNO"].ToString();
                    _openUserRegistration.ID = Convert.ToInt32(dr["ID"].ToString());
                    _openUserRegistration.INSERTDT = Convert.ToDateTime(dr["INSERTDT"].ToString());
                    _openUserRegistration.MNAME = dr["MNAME"].ToString();
                    _openUserRegistration.NAME = dr["NAME"].ToString();
                    _openUserRegistration.NATION = dr["NATION"].ToString();
                    _openUserRegistration.OROLL = dr["OROLL"].ToString();
                    _openUserRegistration.OSCHOOL = dr["OSCHOOL"].ToString();
                    _openUserRegistration.OSESSION = dr["OSESSION"].ToString();
                    _openUserRegistration.PFNAME = dr["PFNAME"].ToString();
                    _openUserRegistration.PHY_CHAL = dr["PHY_CHAL"].ToString();
                    _openUserRegistration.PMNAME = dr["PMNAME"].ToString();
                    _openUserRegistration.PNAME = dr["PNAME"].ToString();
                    _openUserRegistration.PRINTLOT = (float)Convert.ToDecimal(dr["PRINTLOT"].ToString());
                    _openUserRegistration.PRINTSTATU = (float)Convert.ToDecimal(dr["PRINTSTATU"].ToString());
                    _openUserRegistration.REGNO = dr["REGNO"].ToString();
                    _openUserRegistration.REGNO1 = dr["REGNO1"].ToString();
                    _openUserRegistration.REGNOOLD = dr["REGNOOLD"].ToString();
                    _openUserRegistration.RELIGION = dr["RELIGION"].ToString();
                    _openUserRegistration.RP = dr["RP"].ToString();
                    _openUserRegistration.SCHL = dr["SCHL"].ToString();
                    _openUserRegistration.SCHL1 = dr["SCHL1"].ToString();
                    _openUserRegistration.SCHL2 = dr["SCHL2"].ToString();
                    _openUserRegistration.SCHL3 = dr["SCHL3"].ToString();
                    _openUserRegistration.SCHLUPD_DT = Convert.ToDateTime(dr["SCHLUPD_DT"].ToString());
                    _openUserRegistration.SCHOOLE = dr["SCHOOLE"].ToString();
                    _openUserRegistration.SEX = dr["SEX"].ToString();
                    _openUserRegistration.SET = dr["SET"].ToString();
                    _openUserRegistration.AADHAR_NO = dr["AADHAR_NO"].ToString();
                    _openUserRegistration.SUBJ = dr["SUBJ"].ToString();
                    _openUserRegistration.TEMPREGNO = dr["TEMPREGNO"].ToString();
                    _openUserRegistration.UPDT = Convert.ToDateTime(dr["UPDT"].ToString());
                    _openUserRegistration.YEAR = dr["YEAR"].ToString();
                    _openUserRegistration.IsSmartPhone = dr["IsSmartPhone"].ToString();
                    _openUserRegistration.IsHardCopyCertificate = dr["IsHardCopyCertificate"].ToString();

                }
                else
                {
                    cmd.CommandText = "sp_GetLoginOpen";
                    cmd.Connection = con;
                    cmd.CommandType = CommandType.StoredProcedure;
                    search = " where id = " + app_id;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@search", search);
                    adp = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    adp.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        DataRow dr = ds.Tables[0].Rows[0];
                        string app_no = dr["APPNO"].ToString();
                        OpenUserLogin _openUserLogin = GetRecord(app_no);
                        if (_openUserLogin != null)
                        {                            
                            _openUserRegistration.APPNO = _openUserLogin.APPNO.ToString();
                            _openUserRegistration.AADHAR_NO = _openUserLogin.AADHAR_NO;
                            _openUserRegistration.BOARD = "";
                            _openUserRegistration.CandStudyMedium = "";
                            _openUserRegistration.CASTE = "";
                            _openUserRegistration.CAT = _openUserLogin.CATEGORY;
                            _openUserRegistration.CLASS = _openUserLogin.CLASS;
                            _openUserRegistration.correctionid = "";
                            _openUserRegistration.correction_dt = Convert.ToDateTime("1/1/1753 12:00:00 AM");
                            _openUserRegistration.DIST = _openUserLogin.HOMEDISTNM;
                            _openUserRegistration.DOB = _openUserLogin.DOB.ToString();
                            _openUserRegistration.emr17flag = 0;
                            _openUserRegistration.EXAM = _openUserLogin.STREAMCODE;
                            _openUserRegistration.FEE_EXMPT = 0;
                            _openUserRegistration.FLG_DIST = _openUserLogin.FLG_DIST;
                            _openUserRegistration.FNAME = "";
                            if (_openUserLogin.CLASS == "10")
                            { _openUserRegistration.FORM = "M3"; }
                            else
                            { _openUserRegistration.FORM = "T3"; }
                            _openUserRegistration.FORMNO = "";
                            _openUserRegistration.ID = 0;
                            _openUserRegistration.INSERTDT = DateTime.Now;
                            _openUserRegistration.MNAME = "";
                            _openUserRegistration.NAME = _openUserLogin.NAME;
                            _openUserRegistration.NATION = "";
                            _openUserRegistration.OROLL = "";
                            _openUserRegistration.OSCHOOL = "";
                            _openUserRegistration.OSESSION = "";
                            _openUserRegistration.PFNAME = "";
                            _openUserRegistration.PHY_CHAL = "";
                            _openUserRegistration.PMNAME = "";
                            _openUserRegistration.PNAME = "";
                            _openUserRegistration.PRINTLOT = 0;
                            _openUserRegistration.PRINTSTATU = 0;
                            _openUserRegistration.REGNO = "";
                            _openUserRegistration.REGNO1 = "";
                            _openUserRegistration.REGNOOLD = "";
                            _openUserRegistration.RELIGION = "";
                            _openUserRegistration.RP = "O";
                            _openUserRegistration.SCHL = "";
                            _openUserRegistration.SCHL1 = "";
                            _openUserRegistration.SCHL2 = "";
                            _openUserRegistration.SCHL3 = "";
                            _openUserRegistration.SCHLUPD_DT = Convert.ToDateTime("1/1/1753 12:00:00 AM");
                            _openUserRegistration.SCHOOLE = "";
                            _openUserRegistration.SET = "";
                            _openUserRegistration.SUBJ = "";
                            _openUserRegistration.TEMPREGNO = "";
                            _openUserRegistration.UPDT = Convert.ToDateTime("1/1/1753 12:00:00 AM");
                            _openUserRegistration.YEAR = @PSEBONLINE.Repository.SessionSettingMastersRepository.GetSessionSettingMasters().SessionShortYear;
                        }
                    }
                }
                return _openUserRegistration;
            }
            catch (Exception e)
            {
                return new OpenUserRegistration();
            }


            return new OpenUserRegistration();
        }

        public OpenUserLogin GetLoginById(string appid)
        {
            if (appid != null)
            {
                cmd.CommandText = "sp_GetLoginOpen";
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                string search = " where id = " + appid;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@search", search);
                DataSet ds = new DataSet();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(ds);
                try
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        OpenUserLogin _openUserLogin = new OpenUserLogin();
                        DataRow dr = ds.Tables[0].Rows[0];
                        _openUserLogin.AADHAR_NO = dr["AADHAR_NO"].ToString();
                        _openUserLogin.ADDRESS = dr["ADDRESS"].ToString();
                        _openUserLogin.ADMINUSER = (float)Convert.ToDecimal(dr["ADMINUSER"].ToString());
                        _openUserLogin.APPNO = Convert.ToInt64(dr["APPNO"].ToString());
                        _openUserLogin.BLOCK = dr["BLOCK"].ToString();
                        _openUserLogin.CATEGORY = dr["CATEGORY"].ToString();
                        _openUserLogin.CHALLANDT = Convert.ToDateTime(dr["CHALLANDT"].ToString());
                        if (dr["CHALLANFLA"].ToString().ToLower() == "true")
                        {
                            _openUserLogin.CHALLANFLA = 1;
                        }
                        else
                        {
                            _openUserLogin.CHALLANFLA = 0;
                        }
                        _openUserLogin.CLASS = dr["CLASS"].ToString();
                        _openUserLogin.correctionid = dr["correctionid"].ToString();
                        _openUserLogin.correction_dt = Convert.ToDateTime(dr["correction_dt"].ToString());
                        _openUserLogin.DIST = dr["DIST"].ToString();
                        _openUserLogin.DISTNME = dr["DISTNME"].ToString();
                        _openUserLogin.DOB = dr["DOB"].ToString();
                        _openUserLogin.DOC_A_RAND = dr["DOC_A_RAND"].ToString();
                        _openUserLogin.DOC_B_RAND = dr["DOC_B_RAND"].ToString();
                        _openUserLogin.DOC_C_RAND = dr["DOC_C_RAND"].ToString();
                        _openUserLogin.DOWNLOADDA = Convert.ToDateTime(dr["DOWNLOADDA"].ToString());
                        if (dr["DOWNLOADFL"].ToString().ToLower() == "true")
                        {
                            _openUserLogin.DOWNLOADFL = 1;
                        }
                        else
                        {
                            _openUserLogin.DOWNLOADFL = 0;
                        }
                        _openUserLogin.EMAILID = dr["EMAILID"].ToString();
                        _openUserLogin.FLG_DIST = dr["FLG_DIST"].ToString();
                        _openUserLogin.FORM = dr["FORM"].ToString();
                        _openUserLogin.HOMEDIST = dr["HOMEDIST"].ToString();
                        _openUserLogin.HOMEDISTNM = dr["HOMEDISTNM"].ToString();
                        _openUserLogin.ID = Convert.ToInt32(dr["ID"].ToString());
                        _openUserLogin.IMGSIGN_RA = dr["IMGSIGN_RA"].ToString();
                        _openUserLogin.IMG_RAND = dr["IMG_RAND"].ToString();
                        _openUserLogin.INSERTDT = Convert.ToDateTime(dr["INSERTDT"].ToString());
                        if (dr["ISCOMPLETE"].ToString().ToLower() == "true")
                        {
                            _openUserLogin.ISCOMPLETE = 1;
                        }
                        else
                        {
                            _openUserLogin.ISCOMPLETE = 0;
                        }
                        if (dr["ISSCHLCHOO"].ToString().ToLower() == "true")
                        {
                            _openUserLogin.ISSCHLCHOO = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSCHLCHOO = 0;
                        }
                        if (dr["ISSTEP1"].ToString().ToLower() == "true")
                        {
                            _openUserLogin.ISSTEP1 = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSTEP1 = 0;
                        }

                        if (dr["ISSTEP2"].ToString().ToLower() == "true")
                        {
                            _openUserLogin.ISSTEP2 = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSTEP2 = 0;
                        }
                        _openUserLogin.ISSTEP1DT = Convert.ToDateTime(dr["ISSTEP1DT"].ToString());

                        if (dr["ISSTEP2B"].ToString().ToLower() == "true")
                        {
                            _openUserLogin.ISSTEP2B = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSTEP2B = 0;
                        }

                        if (dr["ISSUBJECT"].ToString().ToLower() == "true")
                        {
                            _openUserLogin.ISSUBJECT = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSUBJECT = 0;
                        }
                        _openUserLogin.ISSTEP2DT = Convert.ToDateTime(dr["ISSTEP2DT"].ToString());
                        _openUserLogin.LANDMARK = dr["LANDMARK"].ToString();
                        _openUserLogin.MOBILENO = dr["MOBILENO"].ToString();
                        _openUserLogin.MODIFYBY = dr["MODIFYBY"].ToString();
                        _openUserLogin.MODIFYDT = dr["MODIFYDT"].ToString();
                        _openUserLogin.NAME = dr["NAME"].ToString();
                        _openUserLogin.PINCODE = dr["PINCODE"].ToString();
                        _openUserLogin.PNAME = dr["PNAME"].ToString();
                        _openUserLogin.PWD = dr["PWD"].ToString();
                        _openUserLogin.RDATE = dr["RDATE"].ToString();
                        _openUserLogin.RECEIVEFLA = (float)Convert.ToDecimal(dr["RECEIVEFLA"].ToString());
                        _openUserLogin.REGDATE = dr["REGDATE"].ToString();
                        _openUserLogin.REMARK = dr["REMARK"].ToString();
                        _openUserLogin.SCHL = dr["SCHL"].ToString();
                        _openUserLogin.SCHOOLE = dr["SCHOOLE"].ToString();
                        _openUserLogin.STREAM = dr["STREAM"].ToString();
                        _openUserLogin.STREAMCODE = dr["STREAMCODE"].ToString();
                        _openUserLogin.TEHSIL = dr["TEHSIL"].ToString();
                        _openUserLogin.TOKENNO = dr["TOKENNO"].ToString();
                        _openUserLogin.UPDT = Convert.ToDateTime(dr["UPDT"].ToString());

                        return _openUserLogin;
                    }
                    else
                    {
                        return new OpenUserLogin();
                    }
                }
                catch (Exception e)
                {

                }
            }
            return new OpenUserLogin();
        }

        public int InsertRegistrationUser(OpenUserRegistration _openUserRegistration, string imgPhoto, string imgSign)
        {
            if (_openUserRegistration.APPNO == null) { return 0; }
            else
            {
                OpenUserLogin _openUserLogin = GetLoginById(_openUserRegistration.APPNO);
                if (_openUserRegistration.CLASS == string.Empty || _openUserRegistration.CLASS == null)
                { _openUserRegistration.CLASS = _openUserLogin.CLASS; }

                if (_openUserRegistration.FORM == string.Empty || _openUserRegistration.FORM == null)
                { _openUserRegistration.FORM = _openUserLogin.FORM; }

                if (_openUserLogin.CLASS == "10")
                {
                    _openUserRegistration.EXAM = "G";
                    _openUserLogin.STREAM = "GENERAL";
                    _openUserLogin.STREAMCODE = "G";                 
                }
              
                _openUserRegistration.EXAM = _openUserLogin.STREAMCODE.ToUpper();
                _openUserRegistration.NAME = _openUserLogin.NAME;
                _openUserRegistration.DOB = _openUserLogin.DOB;
                _openUserRegistration.NATION = "INDIA";
                _openUserRegistration.AADHAR_NO = _openUserLogin.AADHAR_NO;

                if (imgSign != string.Empty) { _openUserLogin.IMGSIGN_RA = imgSign; _openUserLogin.UPDT = DateTime.Now; }
                if (imgPhoto != string.Empty) { _openUserLogin.IMG_RAND = imgPhoto; _openUserLogin.UPDT = DateTime.Now; }
                if (_openUserLogin.ISSTEP2 == 0) { _openUserLogin.ISSTEP2 = 1; _openUserLogin.ISSTEP2DT = DateTime.Now; _openUserLogin.UPDT = DateTime.Now; }
                UpdateLoginUser(_openUserLogin);
            }
            try
            {
                OpenUserRegistration _oUserRegistration = GetRegistrationRecord(_openUserRegistration.APPNO);
                _openUserRegistration.INSERTDT = _oUserRegistration.INSERTDT;
                _openUserRegistration.UPDT = DateTime.Now;
            }
            catch (Exception)
            {
                _openUserRegistration.INSERTDT = DateTime.Now;
                _openUserRegistration.UPDT = DateTime.Now;
            }
            if (_openUserRegistration.SCHLUPD_DT == new DateTime()) { _openUserRegistration.SCHLUPD_DT = Convert.ToDateTime("1/1/1753 12:00:00 AM"); }
            if (_openUserRegistration.correction_dt == new DateTime()) { _openUserRegistration.correction_dt = Convert.ToDateTime("1/1/1753 12:00:00 AM"); }

            if (_openUserRegistration.AADHAR_NO == null) { _openUserRegistration.AADHAR_NO = string.Empty; }

            if (_openUserRegistration.YEAR == null) { _openUserRegistration.YEAR = @PSEBONLINE.Repository.SessionSettingMastersRepository.GetSessionSettingMasters().SessionShortYear; }
            if (_openUserRegistration.SET == null) { _openUserRegistration.SET = string.Empty; }
            if (_openUserRegistration.CLASS == null) { _openUserRegistration.CLASS = string.Empty; }
            if (_openUserRegistration.FORM == null) { _openUserRegistration.FORM = string.Empty; }
            if (_openUserRegistration.DIST == null) { _openUserRegistration.DIST = string.Empty; }
            if (_openUserRegistration.RP == null) { _openUserRegistration.RP = "O"; }
            if (_openUserRegistration.EXAM == null) { _openUserRegistration.EXAM = string.Empty; }
            if (_openUserRegistration.SCHL == null) { _openUserRegistration.SCHL = string.Empty; }
            if (_openUserRegistration.REGNO == null) { _openUserRegistration.REGNO = string.Empty; }
            if (_openUserRegistration.NAME == null) { _openUserRegistration.NAME = string.Empty; }
            if (_openUserRegistration.PNAME == null) { _openUserRegistration.PNAME = string.Empty; }
            if (_openUserRegistration.FNAME == null) { _openUserRegistration.FNAME = string.Empty; }
            if (_openUserRegistration.PFNAME == null) { _openUserRegistration.PFNAME = string.Empty; }
            if (_openUserRegistration.MNAME == null) { _openUserRegistration.MNAME = string.Empty; }
            if (_openUserRegistration.PMNAME == null) { _openUserRegistration.PMNAME = string.Empty; }
            if (_openUserRegistration.DOB == null) { _openUserRegistration.DOB = string.Empty; }
            if (_openUserRegistration.PHY_CHAL == null) { _openUserRegistration.PHY_CHAL = string.Empty; }
            if (_openUserRegistration.SEX == null) { _openUserRegistration.SEX = string.Empty; }
            if (_openUserRegistration.CASTE == null) { _openUserRegistration.CASTE = string.Empty; }
            if (_openUserRegistration.RELIGION == null) { _openUserRegistration.RELIGION = string.Empty; }
            if (_openUserRegistration.NATION == null) { _openUserRegistration.NATION = string.Empty; }
            if (_openUserRegistration.CAT == null) { _openUserRegistration.CAT = string.Empty; }
            if (_openUserRegistration.BOARD == null) { _openUserRegistration.BOARD = string.Empty; }
            if (_openUserRegistration.OROLL == null) { _openUserRegistration.OROLL = string.Empty; }
            if (_openUserRegistration.OSESSION == null) { _openUserRegistration.OSESSION = string.Empty; }
            if (_openUserRegistration.OSCHOOL == null) { _openUserRegistration.OSCHOOL = string.Empty; }
            if (_openUserRegistration.SCHOOLE == null) { _openUserRegistration.SCHOOLE = string.Empty; }
            if (_openUserRegistration.SCHL2 == null) { _openUserRegistration.SCHL2 = string.Empty; }
            if (_openUserRegistration.SCHL3 == null) { _openUserRegistration.SCHL3 = string.Empty; }
            if (_openUserRegistration.SCHL1 == null) { _openUserRegistration.SCHL1 = string.Empty; }
            if (_openUserRegistration.FLG_DIST == null) { _openUserRegistration.FLG_DIST = string.Empty; }
            if (_openUserRegistration.SUBJ == null) { _openUserRegistration.SUBJ = string.Empty; }
            if (_openUserRegistration.FORMNO == null) { _openUserRegistration.FORMNO = string.Empty; }
            if (_openUserRegistration.TEMPREGNO == null) { _openUserRegistration.TEMPREGNO = string.Empty; }
            if (_openUserRegistration.REGNO1 == null) { _openUserRegistration.REGNO1 = string.Empty; }
            if (_openUserRegistration.REGNOOLD == null) { _openUserRegistration.REGNOOLD = string.Empty; }
            if (_openUserRegistration.CandStudyMedium == null) { _openUserRegistration.CandStudyMedium = string.Empty; }
            if (_openUserRegistration.correctionid == null) { _openUserRegistration.correctionid = string.Empty; }
            if (_openUserRegistration.AppearingYear == null) { _openUserRegistration.AppearingYear = string.Empty; }           

            if (IsUserInReg(_openUserRegistration.APPNO) == 1)
            {
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_tblRegistrationOpen_update";
            }
            else
            {
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_tblRegistrationOpen_insert";
            }
            cmd.Parameters.Clear();
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@APPNO", _openUserRegistration.APPNO.ToUpper());
            // cmd.Parameters.AddWithValue("@YEAR", _openUserRegistration.YEAR.ToUpper());
            cmd.Parameters.AddWithValue("@YEAR", @PSEBONLINE.Repository.SessionSettingMastersRepository.GetSessionSettingMasters().SessionShortYear);
            cmd.Parameters.AddWithValue("@SET", _openUserRegistration.SET.ToUpper());
            cmd.Parameters.AddWithValue("@CLASS", _openUserRegistration.CLASS.ToUpper());
            cmd.Parameters.AddWithValue("@FORM", _openUserRegistration.FORM.ToUpper());
            cmd.Parameters.AddWithValue("@DIST", _openUserRegistration.DIST.ToUpper());
            cmd.Parameters.AddWithValue("@RP", _openUserRegistration.RP.ToUpper());
            cmd.Parameters.AddWithValue("@EXAM", _openUserRegistration.EXAM.ToUpper());
            cmd.Parameters.AddWithValue("@SCHL", _openUserRegistration.SCHL.ToUpper());
            cmd.Parameters.AddWithValue("@REGNO", _openUserRegistration.REGNO.ToUpper());
            cmd.Parameters.AddWithValue("@NAME", _openUserRegistration.NAME.ToUpper());
            cmd.Parameters.AddWithValue("@PNAME", _openUserRegistration.PNAME);
            cmd.Parameters.AddWithValue("@FNAME", _openUserRegistration.FNAME.ToUpper());
            cmd.Parameters.AddWithValue("@PFNAME", _openUserRegistration.PFNAME);
            cmd.Parameters.AddWithValue("@MNAME", _openUserRegistration.MNAME.ToUpper());
            cmd.Parameters.AddWithValue("@PMNAME", _openUserRegistration.PMNAME);
            cmd.Parameters.AddWithValue("@DOB", _openUserRegistration.DOB.ToUpper());
            cmd.Parameters.AddWithValue("@PHY_CHAL", _openUserRegistration.PHY_CHAL.ToUpper());
            cmd.Parameters.AddWithValue("@SEX", _openUserRegistration.SEX.ToUpper());
            cmd.Parameters.AddWithValue("@CASTE", _openUserRegistration.CASTE.ToUpper());
            cmd.Parameters.AddWithValue("@RELIGION", _openUserRegistration.RELIGION.ToUpper());
            cmd.Parameters.AddWithValue("@NATION", _openUserRegistration.NATION.ToUpper());
            cmd.Parameters.AddWithValue("@CAT", _openUserRegistration.CAT.ToUpper());
            cmd.Parameters.AddWithValue("@BOARD", _openUserRegistration.BOARD.ToUpper());
            cmd.Parameters.AddWithValue("@OROLL", _openUserRegistration.OROLL.ToUpper());
            cmd.Parameters.AddWithValue("@OSESSION", _openUserRegistration.OSESSION.ToUpper());
            cmd.Parameters.AddWithValue("@OSCHOOL", _openUserRegistration.OSCHOOL.ToUpper());
            cmd.Parameters.AddWithValue("@SCHOOLE", _openUserRegistration.SCHOOLE.ToUpper());
            cmd.Parameters.AddWithValue("@INSERTDT", _openUserRegistration.INSERTDT);
            cmd.Parameters.AddWithValue("@UPDT", _openUserRegistration.UPDT);
            cmd.Parameters.AddWithValue("@SCHL2", _openUserRegistration.SCHL2.ToUpper());
            cmd.Parameters.AddWithValue("@SCHL3", _openUserRegistration.SCHL3.ToUpper());
            cmd.Parameters.AddWithValue("@AADHAR_NO", _openUserRegistration.AADHAR_NO.ToUpper());
            cmd.Parameters.AddWithValue("@SCHL1", _openUserRegistration.SCHL1.ToUpper());
            cmd.Parameters.AddWithValue("@SCHLUPD_DT", _openUserRegistration.SCHLUPD_DT);
            cmd.Parameters.AddWithValue("@FLG_DIST", _openUserRegistration.FLG_DIST.ToUpper());
            cmd.Parameters.AddWithValue("@PRINTLOT", _openUserRegistration.PRINTLOT);
            cmd.Parameters.AddWithValue("@PRINTSTATU", _openUserRegistration.PRINTSTATU);
            cmd.Parameters.AddWithValue("@FEE_EXMPT", _openUserRegistration.FEE_EXMPT);
            cmd.Parameters.AddWithValue("@SUBJ", _openUserRegistration.SUBJ.ToUpper());
            cmd.Parameters.AddWithValue("@FORMNO", _openUserRegistration.FORMNO.ToUpper());
            cmd.Parameters.AddWithValue("@TEMPREGNO", _openUserRegistration.TEMPREGNO.ToUpper());
            cmd.Parameters.AddWithValue("@REGNO1", _openUserRegistration.REGNO1.ToUpper());
            cmd.Parameters.AddWithValue("@REGNOOLD", _openUserRegistration.REGNOOLD.ToUpper());
            cmd.Parameters.AddWithValue("@emr17flag", _openUserRegistration.emr17flag);
            cmd.Parameters.AddWithValue("@CandStudyMedium", _openUserRegistration.CandStudyMedium.ToUpper());
            cmd.Parameters.AddWithValue("@correctionid", _openUserRegistration.correctionid.ToUpper());
            cmd.Parameters.AddWithValue("@correction_dt", _openUserRegistration.correction_dt);
            cmd.Parameters.AddWithValue("@AppearingYear", _openUserRegistration.AppearingYear.ToUpper());
            cmd.Parameters.AddWithValue("@DisabilityPercent", _openUserRegistration.DisabilityPercent);
            cmd.Parameters.AddWithValue("@IsSmartPhone", _openUserRegistration.IsSmartPhone);
            cmd.Parameters.AddWithValue("@IsHardCopyCertificate", _openUserRegistration.IsHardCopyCertificate);
            
            try
            {
                con.Open();
                int result = cmd.ExecuteNonQuery();
                con.Close();
                if (result > 0)
                { return 1; }
                else
                { return 0; }
              
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public int UpdateLoginUser(OpenUserLogin _openUserLogin)
        {
            OpenUserLogin ol = new OpenUserLogin();
            ol = GetLoginById(_openUserLogin.ID.ToString());
            if (ol != _openUserLogin)
            {
                if (_openUserLogin.CLASS == null) { _openUserLogin.CLASS = string.Empty; }
                if (_openUserLogin.FORM == null) { _openUserLogin.FORM = string.Empty; }
                if (_openUserLogin.REGDATE == null) { _openUserLogin.REGDATE = string.Empty; }
                if (_openUserLogin.DIST == null) { _openUserLogin.DIST = string.Empty; }
                if (_openUserLogin.SCHL == null) { _openUserLogin.SCHL = string.Empty; }
                if (_openUserLogin.NAME == null) { _openUserLogin.NAME = string.Empty; }
                if (_openUserLogin.PNAME == null) { _openUserLogin.PNAME = string.Empty; }
                if (_openUserLogin.MOBILENO == null) { _openUserLogin.MOBILENO = string.Empty; }
                if (_openUserLogin.DISTNME == null) { _openUserLogin.DISTNME = string.Empty; }
                if (_openUserLogin.SCHOOLE == null) { _openUserLogin.SCHOOLE = string.Empty; }
                if (_openUserLogin.EMAILID == null) { _openUserLogin.EMAILID = string.Empty; }
                if (_openUserLogin.PWD == null) { _openUserLogin.PWD = string.Empty; }
                if (_openUserLogin.STREAM == null) { _openUserLogin.STREAM = string.Empty; }
                if (_openUserLogin.STREAMCODE == null) { _openUserLogin.STREAMCODE = string.Empty; }
                if (_openUserLogin.IMG_RAND == null) { _openUserLogin.IMG_RAND = string.Empty; }
                if (_openUserLogin.IMGSIGN_RA == null) { _openUserLogin.IMGSIGN_RA = string.Empty; }
                if (_openUserLogin.DOC_A_RAND == null) { _openUserLogin.DOC_A_RAND = string.Empty; }
                if (_openUserLogin.DOC_B_RAND == null) { _openUserLogin.DOC_B_RAND = string.Empty; }
                if (_openUserLogin.DOC_C_RAND == null) { _openUserLogin.DOC_C_RAND = string.Empty; }
                if (_openUserLogin.AADHAR_NO == null) { _openUserLogin.AADHAR_NO = string.Empty; }
                if (_openUserLogin.RDATE == null) { _openUserLogin.RDATE = string.Empty; }
                if (_openUserLogin.TOKENNO == null) { _openUserLogin.TOKENNO = string.Empty; }
                if (_openUserLogin.ADDRESS == null) { _openUserLogin.ADDRESS = string.Empty; }
                if (_openUserLogin.LANDMARK == null) { _openUserLogin.LANDMARK = string.Empty; }
                if (_openUserLogin.BLOCK == null) { _openUserLogin.BLOCK = string.Empty; }
                if (_openUserLogin.TEHSIL == null) { _openUserLogin.TEHSIL = string.Empty; }
                if (_openUserLogin.PINCODE == null) { _openUserLogin.PINCODE = string.Empty; }
                if (_openUserLogin.FLG_DIST == null) { _openUserLogin.FLG_DIST = string.Empty; }
                if (_openUserLogin.REMARK == null) { _openUserLogin.REMARK = string.Empty; }
                if (_openUserLogin.MODIFYBY == null) { _openUserLogin.MODIFYBY = string.Empty; }
                if (_openUserLogin.MODIFYDT == null) { _openUserLogin.MODIFYDT = string.Empty; }
                if (_openUserLogin.CATEGORY == null) { _openUserLogin.CATEGORY = string.Empty; }
                if (_openUserLogin.HOMEDIST == null) { _openUserLogin.HOMEDIST = string.Empty; }
                if (_openUserLogin.HOMEDISTNM == null) { _openUserLogin.HOMEDISTNM = string.Empty; }
                if (_openUserLogin.correctionid == null) { _openUserLogin.correctionid = string.Empty; }

                if (_openUserLogin.STREAM == string.Empty || _openUserLogin.STREAMCODE == "G")
                {
                    _openUserLogin.CLASS = "10";
                    _openUserLogin.FORM = "M3";
                }
                else
                {
                    _openUserLogin.CLASS = "12";
                    _openUserLogin.FORM = "T3";
                }

                if (_openUserLogin.HOMEDISTNM != string.Empty && _openUserLogin.HOMEDIST == string.Empty)
                {
                    List<SelectListItem> lst = GetDistrict();
                    SelectListItem sel = lst.Find(f => f.Value == _openUserLogin.HOMEDISTNM);
                    _openUserLogin.HOMEDIST = _openUserLogin.HOMEDISTNM;
                    _openUserLogin.HOMEDISTNM = sel.Text;
                }

                cmd = new SqlCommand();
                cmd.CommandText = "sp_tblloginOpen_update";
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@ID", _openUserLogin.ID);
                cmd.Parameters.AddWithValue("@APPNO", _openUserLogin.APPNO);
                cmd.Parameters.AddWithValue("@CLASS", _openUserLogin.CLASS.ToUpper());
                cmd.Parameters.AddWithValue("@FORM", _openUserLogin.FORM.ToUpper());
                cmd.Parameters.AddWithValue("@REGDATE", _openUserLogin.REGDATE.ToUpper());
                cmd.Parameters.AddWithValue("@DIST", _openUserLogin.DIST.ToUpper());
                cmd.Parameters.AddWithValue("@SCHL", _openUserLogin.SCHL.ToUpper());
                cmd.Parameters.AddWithValue("@NAME", _openUserLogin.NAME.ToUpper());
                cmd.Parameters.AddWithValue("@PNAME", _openUserLogin.PNAME.ToUpper());
                cmd.Parameters.AddWithValue("@MOBILENO", _openUserLogin.MOBILENO.ToUpper());
                cmd.Parameters.AddWithValue("@DOB", _openUserLogin.DOB.ToUpper());
                cmd.Parameters.AddWithValue("@DISTNME", _openUserLogin.DISTNME.ToUpper());
                cmd.Parameters.AddWithValue("@SCHOOLE", _openUserLogin.SCHOOLE.ToUpper());
                cmd.Parameters.AddWithValue("@EMAILID", _openUserLogin.EMAILID.ToUpper());
                cmd.Parameters.AddWithValue("@PWD", _openUserLogin.PWD);
                cmd.Parameters.AddWithValue("@ISSTEP1", _openUserLogin.ISSTEP1);
                cmd.Parameters.AddWithValue("@ISSTEP2", _openUserLogin.ISSTEP2);
                cmd.Parameters.AddWithValue("@ISCOMPLETE", _openUserLogin.ISCOMPLETE);
                cmd.Parameters.AddWithValue("@CHALLANFLA", _openUserLogin.CHALLANFLA);
                cmd.Parameters.AddWithValue("@CHALLANDT", _openUserLogin.CHALLANDT);
                cmd.Parameters.AddWithValue("@ISSTEP1DT", _openUserLogin.ISSTEP1DT);
                cmd.Parameters.AddWithValue("@ISSTEP2DT", _openUserLogin.ISSTEP2DT);
                cmd.Parameters.AddWithValue("@INSERTDT", _openUserLogin.INSERTDT);
                cmd.Parameters.AddWithValue("@UPDT", _openUserLogin.UPDT);
                cmd.Parameters.AddWithValue("@STREAM", _openUserLogin.STREAM.ToUpper());
                cmd.Parameters.AddWithValue("@STREAMCODE", _openUserLogin.STREAMCODE.ToUpper());
                cmd.Parameters.AddWithValue("@IMG_RAND", _openUserLogin.IMG_RAND.ToUpper());
                cmd.Parameters.AddWithValue("@IMGSIGN_RA", _openUserLogin.IMGSIGN_RA.ToUpper());
                cmd.Parameters.AddWithValue("@DOC_A_RAND", _openUserLogin.DOC_A_RAND.ToUpper());
                cmd.Parameters.AddWithValue("@DOC_B_RAND", _openUserLogin.DOC_B_RAND.ToUpper());
                cmd.Parameters.AddWithValue("@DOC_C_RAND", _openUserLogin.DOC_C_RAND.ToUpper());
                cmd.Parameters.AddWithValue("@ISSTEP2B", _openUserLogin.ISSTEP2B);
                cmd.Parameters.AddWithValue("@ISSUBJECT", _openUserLogin.ISSUBJECT);
                cmd.Parameters.AddWithValue("@AADHAR_NO", _openUserLogin.AADHAR_NO);
                cmd.Parameters.AddWithValue("@ISSCHLCHOO", _openUserLogin.ISSCHLCHOO);
                cmd.Parameters.AddWithValue("@RECEIVEFLA", _openUserLogin.RECEIVEFLA);
                cmd.Parameters.AddWithValue("@RDATE", _openUserLogin.RDATE.ToUpper());
                cmd.Parameters.AddWithValue("@TOKENNO", _openUserLogin.TOKENNO.ToUpper());
                cmd.Parameters.AddWithValue("@ADMINUSER", _openUserLogin.ADMINUSER);
                cmd.Parameters.AddWithValue("@ADDRESS", _openUserLogin.ADDRESS.ToUpper());
                cmd.Parameters.AddWithValue("@LANDMARK", _openUserLogin.LANDMARK.ToUpper());
                cmd.Parameters.AddWithValue("@BLOCK", _openUserLogin.BLOCK.ToUpper());
                cmd.Parameters.AddWithValue("@TEHSIL", _openUserLogin.TEHSIL.ToUpper());
                cmd.Parameters.AddWithValue("@PINCODE", _openUserLogin.PINCODE.ToUpper());
                cmd.Parameters.AddWithValue("@FLG_DIST", _openUserLogin.FLG_DIST.ToUpper());
                cmd.Parameters.AddWithValue("@REMARK", _openUserLogin.REMARK.ToUpper());
                cmd.Parameters.AddWithValue("@MODIFYBY", _openUserLogin.MODIFYBY.ToUpper());
                cmd.Parameters.AddWithValue("@MODIFYDT", _openUserLogin.MODIFYDT.ToUpper());
                cmd.Parameters.AddWithValue("@CATEGORY", _openUserLogin.CATEGORY.ToUpper());
                cmd.Parameters.AddWithValue("@DOWNLOADFL", _openUserLogin.DOWNLOADFL);
                cmd.Parameters.AddWithValue("@DOWNLOADDA", _openUserLogin.DOWNLOADDA);
                cmd.Parameters.AddWithValue("@HOMEDIST", _openUserLogin.HOMEDIST.ToUpper());
                cmd.Parameters.AddWithValue("@HOMEDISTNM", _openUserLogin.HOMEDISTNM.ToUpper());
                cmd.Parameters.AddWithValue("@correctionid", _openUserLogin.correctionid.ToUpper());
                cmd.Parameters.AddWithValue("@correction_dt", _openUserLogin.correction_dt);
                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    return 1;
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
            else
            {
                return -1;
            }
        }

        public string InsertUser(OpenUserLogin _ouserLogin)
        {
            if (_ouserLogin.CLASS == null) { _ouserLogin.CLASS = string.Empty; }
            if (_ouserLogin.FORM == null) { _ouserLogin.FORM = string.Empty; }
            if (_ouserLogin.REGDATE == null) { _ouserLogin.REGDATE = string.Empty; }
            if (_ouserLogin.DIST == null) { _ouserLogin.DIST = string.Empty; }
            if (_ouserLogin.SCHL == null) { _ouserLogin.SCHL = string.Empty; }
            if (_ouserLogin.NAME == null) { _ouserLogin.NAME = string.Empty; }
            if (_ouserLogin.PNAME == null) { _ouserLogin.PNAME = string.Empty; }
            if (_ouserLogin.MOBILENO == null) { _ouserLogin.MOBILENO = string.Empty; }
            if (_ouserLogin.DISTNME == null) { _ouserLogin.DISTNME = string.Empty; }
            if (_ouserLogin.SCHOOLE == null) { _ouserLogin.SCHOOLE = string.Empty; }
            if (_ouserLogin.EMAILID == null) { _ouserLogin.EMAILID = string.Empty; }
            if (_ouserLogin.PWD == null) { _ouserLogin.PWD = string.Empty; }
            if (_ouserLogin.STREAM == null) { _ouserLogin.STREAM = string.Empty; }
            if (_ouserLogin.STREAMCODE == null) { _ouserLogin.STREAMCODE = string.Empty; }
            if (_ouserLogin.IMG_RAND == null) { _ouserLogin.IMG_RAND = string.Empty; }
            if (_ouserLogin.IMGSIGN_RA == null) { _ouserLogin.IMGSIGN_RA = string.Empty; }
            if (_ouserLogin.DOC_A_RAND == null) { _ouserLogin.DOC_A_RAND = string.Empty; }
            if (_ouserLogin.DOC_B_RAND == null) { _ouserLogin.DOC_B_RAND = string.Empty; }
            if (_ouserLogin.DOC_C_RAND == null) { _ouserLogin.DOC_C_RAND = string.Empty; }
            if (_ouserLogin.AADHAR_NO == null) { _ouserLogin.AADHAR_NO = string.Empty; }
            if (_ouserLogin.RDATE == null) { _ouserLogin.RDATE = string.Empty; }
            if (_ouserLogin.TOKENNO == null) { _ouserLogin.TOKENNO = string.Empty; }
            if (_ouserLogin.ADDRESS == null) { _ouserLogin.ADDRESS = string.Empty; }
            if (_ouserLogin.LANDMARK == null) { _ouserLogin.LANDMARK = string.Empty; }
            if (_ouserLogin.BLOCK == null) { _ouserLogin.BLOCK = string.Empty; }
            if (_ouserLogin.TEHSIL == null) { _ouserLogin.TEHSIL = string.Empty; }
            if (_ouserLogin.PINCODE == null) { _ouserLogin.PINCODE = string.Empty; }
            if (_ouserLogin.FLG_DIST == null) { _ouserLogin.FLG_DIST = string.Empty; }
            if (_ouserLogin.REMARK == null) { _ouserLogin.REMARK = string.Empty; }
            if (_ouserLogin.MODIFYBY == null) { _ouserLogin.MODIFYBY = string.Empty; }
            if (_ouserLogin.MODIFYDT == null) { _ouserLogin.MODIFYDT = string.Empty; }
            if (_ouserLogin.CATEGORY == null) { _ouserLogin.CATEGORY = string.Empty; }
            if (_ouserLogin.HOMEDIST == null) { _ouserLogin.HOMEDIST = string.Empty; }
            if (_ouserLogin.HOMEDISTNM == null) { _ouserLogin.HOMEDISTNM = string.Empty; }
            if (_ouserLogin.correctionid == null) { _ouserLogin.correctionid = string.Empty; }

            if (_ouserLogin.STREAM == string.Empty)
            {
                _ouserLogin.CLASS = "10";
                _ouserLogin.FORM = "M3";
                _ouserLogin.STREAM = "GENERAL";
                _ouserLogin.STREAMCODE = "G";
            }
            else
            {
                _ouserLogin.CLASS = "12";
                _ouserLogin.FORM = "T3";
            }

            if (_ouserLogin.HOMEDISTNM != string.Empty && _ouserLogin.HOMEDIST == string.Empty)
            {
                List<SelectListItem> lst = GetDistrict();
                SelectListItem sel = lst.Find(f => f.Value == _ouserLogin.HOMEDISTNM);
                _ouserLogin.HOMEDIST = _ouserLogin.HOMEDISTNM;
                _ouserLogin.HOMEDISTNM = sel.Text;
            }
            _ouserLogin.ISSTEP1 = 1;
            _ouserLogin.ISSTEP1DT = DateTime.Now;
            _ouserLogin.UPDT = DateTime.Now;
            _ouserLogin.INSERTDT = DateTime.Now;

            cmd = new SqlCommand();
            cmd.CommandText = "sp_tblloginOpen_insert";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@APPNO", _ouserLogin.APPNO);
            cmd.Parameters.AddWithValue("@CLASS", _ouserLogin.CLASS.ToUpper());
            cmd.Parameters.AddWithValue("@FORM", _ouserLogin.FORM.ToUpper());
            cmd.Parameters.AddWithValue("@REGDATE", _ouserLogin.REGDATE.ToUpper());
            cmd.Parameters.AddWithValue("@DIST", _ouserLogin.DIST.ToUpper());
            cmd.Parameters.AddWithValue("@SCHL", _ouserLogin.SCHL.ToUpper());
            cmd.Parameters.AddWithValue("@NAME", _ouserLogin.NAME.ToUpper());
            cmd.Parameters.AddWithValue("@PNAME", _ouserLogin.PNAME);
            cmd.Parameters.AddWithValue("@MOBILENO", _ouserLogin.MOBILENO.ToUpper());
            cmd.Parameters.AddWithValue("@DOB", _ouserLogin.DOB.ToUpper());
            cmd.Parameters.AddWithValue("@DISTNME", _ouserLogin.DISTNME.ToUpper());
            cmd.Parameters.AddWithValue("@SCHOOLE", _ouserLogin.SCHOOLE.ToUpper());
            cmd.Parameters.AddWithValue("@EMAILID", _ouserLogin.EMAILID.ToUpper());
            cmd.Parameters.AddWithValue("@PWD", _ouserLogin.PWD);
            cmd.Parameters.AddWithValue("@ISSTEP1", _ouserLogin.ISSTEP1);
            cmd.Parameters.AddWithValue("@ISSTEP2", _ouserLogin.ISSTEP2);
            cmd.Parameters.AddWithValue("@ISCOMPLETE", _ouserLogin.ISCOMPLETE);
            cmd.Parameters.AddWithValue("@CHALLANFLA", _ouserLogin.CHALLANFLA);
            cmd.Parameters.AddWithValue("@CHALLANDT", _ouserLogin.CHALLANDT);
            cmd.Parameters.AddWithValue("@ISSTEP1DT", _ouserLogin.ISSTEP1DT);
            cmd.Parameters.AddWithValue("@ISSTEP2DT", _ouserLogin.ISSTEP2DT);
            cmd.Parameters.AddWithValue("@INSERTDT", _ouserLogin.INSERTDT);
            cmd.Parameters.AddWithValue("@UPDT", _ouserLogin.UPDT);
            cmd.Parameters.AddWithValue("@STREAM", _ouserLogin.STREAM.ToUpper());
            cmd.Parameters.AddWithValue("@STREAMCODE", _ouserLogin.STREAMCODE.ToUpper());
            cmd.Parameters.AddWithValue("@IMG_RAND", _ouserLogin.IMG_RAND.ToUpper());
            cmd.Parameters.AddWithValue("@IMGSIGN_RA", _ouserLogin.IMGSIGN_RA.ToUpper());
            cmd.Parameters.AddWithValue("@DOC_A_RAND", _ouserLogin.DOC_A_RAND.ToUpper());
            cmd.Parameters.AddWithValue("@DOC_B_RAND", _ouserLogin.DOC_B_RAND.ToUpper());
            cmd.Parameters.AddWithValue("@DOC_C_RAND", _ouserLogin.DOC_C_RAND.ToUpper());
            cmd.Parameters.AddWithValue("@ISSTEP2B", _ouserLogin.ISSTEP2B);
            cmd.Parameters.AddWithValue("@ISSUBJECT", _ouserLogin.ISSUBJECT);
            cmd.Parameters.AddWithValue("@AADHAR_NO", _ouserLogin.AADHAR_NO.ToUpper());
            cmd.Parameters.AddWithValue("@ISSCHLCHOO", _ouserLogin.ISSCHLCHOO);
            cmd.Parameters.AddWithValue("@RECEIVEFLA", _ouserLogin.RECEIVEFLA);
            cmd.Parameters.AddWithValue("@RDATE", _ouserLogin.RDATE.ToUpper());
            cmd.Parameters.AddWithValue("@TOKENNO", _ouserLogin.TOKENNO.ToUpper());
            cmd.Parameters.AddWithValue("@ADMINUSER", _ouserLogin.ADMINUSER);
            cmd.Parameters.AddWithValue("@ADDRESS", _ouserLogin.ADDRESS.ToUpper());
            cmd.Parameters.AddWithValue("@LANDMARK", _ouserLogin.LANDMARK.ToUpper());
            cmd.Parameters.AddWithValue("@BLOCK", _ouserLogin.BLOCK.ToUpper());
            cmd.Parameters.AddWithValue("@TEHSIL", _ouserLogin.TEHSIL.ToUpper());
            cmd.Parameters.AddWithValue("@PINCODE", _ouserLogin.PINCODE.ToUpper());
            cmd.Parameters.AddWithValue("@FLG_DIST", _ouserLogin.FLG_DIST.ToUpper());
            cmd.Parameters.AddWithValue("@REMARK", _ouserLogin.REMARK.ToUpper());
            cmd.Parameters.AddWithValue("@MODIFYBY", _ouserLogin.MODIFYBY.ToUpper());
            cmd.Parameters.AddWithValue("@MODIFYDT", _ouserLogin.MODIFYDT.ToUpper());
            cmd.Parameters.AddWithValue("@CATEGORY", _ouserLogin.CATEGORY.ToUpper());
            cmd.Parameters.AddWithValue("@DOWNLOADFL", _ouserLogin.DOWNLOADFL);
            cmd.Parameters.AddWithValue("@DOWNLOADDA", _ouserLogin.DOWNLOADDA);
            cmd.Parameters.AddWithValue("@HOMEDIST", _ouserLogin.HOMEDIST.ToUpper());
            cmd.Parameters.AddWithValue("@HOMEDISTNM", _ouserLogin.HOMEDISTNM.ToUpper());
            cmd.Parameters.AddWithValue("@correctionid", _ouserLogin.correctionid.ToUpper());
            cmd.Parameters.AddWithValue("@correction_dt", _ouserLogin.correction_dt);

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);
            try
            {

                string id = ds.Tables[0].Rows[0]["APPNO"].ToString();
                return id;
            }
            catch (Exception e)
            {
                return "-1";
            }
            //return 0;

        }

        public OpenUserLogin GetRecord(string appno)
        {
            if (appno != null)
            {
                cmd.CommandText = "sp_GetLoginOpen";
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                string search = "where APPNO = " + appno;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@search", search);
                DataSet ds = new DataSet();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(ds);
                try
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        OpenUserLogin _openUserLogin = new OpenUserLogin();
                        DataRow dr = ds.Tables[0].Rows[0];
                        _openUserLogin.AADHAR_NO = dr["AADHAR_NO"].ToString();
                        _openUserLogin.ADDRESS = dr["ADDRESS"].ToString();
                        _openUserLogin.ADMINUSER = (float)Convert.ToDecimal(dr["ADMINUSER"].ToString());
                        _openUserLogin.APPNO = Convert.ToInt64(dr["APPNO"].ToString());
                        _openUserLogin.BLOCK = dr["BLOCK"].ToString();
                        _openUserLogin.CATEGORY = dr["CATEGORY"].ToString();
                        _openUserLogin.CHALLANDT = Convert.ToDateTime(dr["CHALLANDT"].ToString());
                        if (dr["CHALLANFLA"].ToString() == "true")
                        {
                            _openUserLogin.CHALLANFLA = 1;
                        }
                        else
                        {
                            _openUserLogin.CHALLANFLA = 0;
                        }
                        _openUserLogin.CLASS = dr["CLASS"].ToString();
                        _openUserLogin.correctionid = dr["correctionid"].ToString();
                        _openUserLogin.correction_dt = Convert.ToDateTime(dr["correction_dt"].ToString());
                        _openUserLogin.DIST = dr["DIST"].ToString();
                        _openUserLogin.DISTNME = dr["DISTNME"].ToString();
                        _openUserLogin.DOB = dr["DOB"].ToString();
                        _openUserLogin.DOC_A_RAND = dr["DOC_A_RAND"].ToString();
                        _openUserLogin.DOC_B_RAND = dr["DOC_B_RAND"].ToString();
                        _openUserLogin.DOC_C_RAND = dr["DOC_C_RAND"].ToString();
                        _openUserLogin.DOWNLOADDA = Convert.ToDateTime(dr["DOWNLOADDA"].ToString());
                        if (dr["DOWNLOADFL"].ToString() == "true")
                        {
                            _openUserLogin.DOWNLOADFL = 1;
                        }
                        else
                        {
                            _openUserLogin.DOWNLOADFL = 0;
                        }
                        _openUserLogin.EMAILID = dr["EMAILID"].ToString();
                        _openUserLogin.FLG_DIST = dr["FLG_DIST"].ToString();
                        _openUserLogin.FORM = dr["FORM"].ToString();
                        _openUserLogin.HOMEDIST = dr["HOMEDIST"].ToString();
                        _openUserLogin.HOMEDISTNM = dr["HOMEDISTNM"].ToString();
                        _openUserLogin.ID = Convert.ToInt32(dr["ID"].ToString());
                        _openUserLogin.IMGSIGN_RA = dr["IMGSIGN_RA"].ToString();
                        _openUserLogin.IMG_RAND = dr["IMG_RAND"].ToString();
                        _openUserLogin.INSERTDT = Convert.ToDateTime(dr["INSERTDT"].ToString());
                        if (dr["ISCOMPLETE"].ToString() == "true")
                        {
                            _openUserLogin.ISCOMPLETE = 1;
                        }
                        else
                        {
                            _openUserLogin.ISCOMPLETE = 0;
                        }
                        if (dr["ISSCHLCHOO"].ToString() == "true")
                        {
                            _openUserLogin.ISSCHLCHOO = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSCHLCHOO = 0;
                        }
                        if (dr["ISSTEP1"].ToString() == "true")
                        {
                            _openUserLogin.ISSTEP1 = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSTEP1 = 0;
                        }

                        if (dr["ISSTEP2"].ToString() == "true")
                        {
                            _openUserLogin.ISSTEP2 = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSTEP2 = 0;
                        }
                        _openUserLogin.ISSTEP1DT = Convert.ToDateTime(dr["ISSTEP1DT"].ToString());

                        if (dr["ISSTEP2B"].ToString() == "true")
                        {
                            _openUserLogin.ISSTEP2B = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSTEP2B = 0;
                        }

                        if (dr["ISSUBJECT"].ToString() == "true")
                        {
                            _openUserLogin.ISSUBJECT = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSUBJECT = 0;
                        }
                        _openUserLogin.ISSTEP2DT = Convert.ToDateTime(dr["ISSTEP2DT"].ToString());
                        _openUserLogin.LANDMARK = dr["LANDMARK"].ToString();
                        _openUserLogin.MOBILENO = dr["MOBILENO"].ToString();
                        _openUserLogin.MODIFYBY = dr["MODIFYBY"].ToString();
                        _openUserLogin.MODIFYDT = dr["MODIFYDT"].ToString();
                        _openUserLogin.NAME = dr["NAME"].ToString();
                        _openUserLogin.PINCODE = dr["PINCODE"].ToString();
                        _openUserLogin.PNAME = dr["PNAME"].ToString();
                        _openUserLogin.PWD = dr["PWD"].ToString();
                        _openUserLogin.RDATE = dr["RDATE"].ToString();
                        _openUserLogin.RECEIVEFLA = (float)Convert.ToDecimal(dr["RECEIVEFLA"].ToString());
                        _openUserLogin.REGDATE = dr["REGDATE"].ToString();
                        _openUserLogin.REMARK = dr["REMARK"].ToString();
                        _openUserLogin.SCHL = dr["SCHL"].ToString();
                        _openUserLogin.SCHOOLE = dr["SCHOOLE"].ToString();
                        _openUserLogin.STREAM = dr["STREAM"].ToString();
                        _openUserLogin.STREAMCODE = dr["STREAMCODE"].ToString();
                        _openUserLogin.TEHSIL = dr["TEHSIL"].ToString();
                        _openUserLogin.TOKENNO = dr["TOKENNO"].ToString();
                        _openUserLogin.UPDT = Convert.ToDateTime(dr["UPDT"].ToString());

                        _openUserLogin.IsCancel = Convert.ToInt32(dr["IsCancel"].ToString());
                        _openUserLogin.CancelRemarks = dr["CancelRemarks"].ToString();

                        return _openUserLogin;
                    }
                    else
                    {
                        return new OpenUserLogin();
                    }
                }
                catch (Exception e)
                {

                }
            }
            return new OpenUserLogin();
        }

        public int IsUserInReg(string appno)
        {
            cmd.CommandText = "sp_GetRegistrationOpen";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            string search = "where appno='" + appno + "'";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@search", search);
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count >= 1)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public List<SelectListItem> GetMatricSubjects_1()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();            
            subjects.Add(new SelectListItem() { Value = "02", Text = "ENGLISH" });
            subjects.Add(new SelectListItem() { Value = "03", Text = "HINDI" });
            subjects.Add(new SelectListItem() { Value = "04", Text = "MATHEMATICS" });
            subjects.Add(new SelectListItem() { Value = "05", Text = "SCIENCE" });
            subjects.Add(new SelectListItem() { Value = "06", Text = "SOCIAL STUDIES" });
            subjects.Add(new SelectListItem() { Value = "71", Text = "URDU (IN LIEV OF HINDI)" });

            return subjects;
        }

        
        public List<SelectListItem> GetMatricSubjects_2_OLD()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();

            //subjects.Add(new SelectListItem() { Value = "", Text = "--Select--" });
            subjects.Add(new SelectListItem() { Value = "02", Text = "ENGLISH" });
            subjects.Add(new SelectListItem() { Value = "03", Text = "HINDI" });
            subjects.Add(new SelectListItem() { Value = "04", Text = "MATHEMATICS" });
            subjects.Add(new SelectListItem() { Value = "05", Text = "SCIENCE" });
            subjects.Add(new SelectListItem() { Value = "06", Text = "SOCIAL STUDIES" });
            subjects.Add(new SelectListItem() { Value = "71", Text = "URDU (IN LIEV OF HINDI)" });
            subjects.Add(new SelectListItem() { Value = "08", Text = "HEALTH & PHY. EDU." });
            subjects.Add(new SelectListItem() { Value = "09", Text = "SANSKRIT" });
            subjects.Add(new SelectListItem() { Value = "10", Text = "URDU ELECTIVE" });
            subjects.Add(new SelectListItem() { Value = "28", Text = "MECH. DRAWING & PAINTING" });
            subjects.Add(new SelectListItem() { Value = "29", Text = "CUTTING & TAILORING" });
            subjects.Add(new SelectListItem() { Value = "30", Text = "MUSIC (VOCAL)" });
            subjects.Add(new SelectListItem() { Value = "31", Text = "MUSIC (INSTR.)" });
            subjects.Add(new SelectListItem() { Value = "32", Text = "MUSIC (TABLA)" });
            subjects.Add(new SelectListItem() { Value = "33", Text = "HOME SCIENCE" });
            subjects.Add(new SelectListItem() { Value = "35", Text = "AGRICULTURE" });
            subjects.Add(new SelectListItem() { Value = "63", Text = "COMPUTER SCIENCE" });
            subjects.Add(new SelectListItem() { Value = "70", Text = "HEALTH SCIENCE" });

            return subjects;
        }

        // for 2023-2024
        public List<SelectListItem> GetMatricSubjects_2()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            //  cmd.CommandText = "select sub,name_eng from matric where sub not in ('01','07','72','73','63','92') and NSQF='' and[TYPE] != 'PRE-VOCATIONAL' order by sub";

            //change in 2023-24
            //13. sub 6 : show subject from subject master where opn='Y' and sub not in (63,92,01,07)
            cmd.CommandText = "select sub,name_eng  from matric  where opn='Y' and sub not in (63,92,01,07,72,73)  order by sub";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                }
            }

            return subjects;
        }

        public List<SelectListItem> GetMatricSubjects_Add()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            cmd.CommandText = "select sub,name_eng from matric where opn='Y' and sub not in ('01','07','72','73') and NSQF='' and[TYPE] != 'PRE-VOCATIONAL' order by sub";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                }
            }

            return subjects;
        }

        public List<SelectListItem> GetMatricSubjects_Additional_DA_Yes()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            cmd.CommandText = "select sub,name_eng from matric where opn='Y' and sub not in (01,07) order by sub";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                }
            }

            return subjects;
        }


        public List<SelectListItem> GetAllMatricSubjects()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            cmd.CommandText = "select sub,name_eng from matric where opn='Y'  order by sub";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                }
            }

            return subjects;
        }

        public void InsertUserInSubjects(string[] subjects_array,string [] subcat_array, string app_class, string app_id, string app_stream, FormCollection fc)
        {

           List<tblsubjectopen> _tblsubjectopenList = new List<tblsubjectopen>();
            OpenUserSubjects _openUserSubjects = new OpenUserSubjects();
            _openUserSubjects.CLASS = app_class;
            _openUserSubjects.APPNO = app_id;
            _openUserSubjects.INSERTDT = DateTime.Now;

            if (app_class == "10" )
            {
                _openUserSubjects.STREAM = "GENERAL";
               _openUserSubjects.STREAMCODE = "G";
            }
            else
            {
                List<SelectListItem> streams = GetStreams_1();
                _openUserSubjects.STREAM = app_stream;
                _openUserSubjects.STREAMCODE = streams.Find(f => f.Text == _openUserSubjects.STREAM).Value;
            }


            List<SelectListItem> subjects = new List<SelectListItem>();
            if (_openUserSubjects.CLASS == "10")
            {
                subjects = GetMatricSubjects_2();
            }
            else
            {
                subjects = GetAllSeniorSubjects();
            }


            int i = 0;
            //if (IsUserInSubjects(_openUserSubjects.APPNO) != 0)
            //{
            //    RemoveUserSubjects(_openUserSubjects.APPNO);
            //}


            OpenUserLogin ol = new OpenUserLogin();
            ol = GetLoginById(app_id);

            foreach (string str in subjects_array)
            {
                if (str != string.Empty && str != null)
                {
                    string[] cc = new string[9];
                    _openUserSubjects.SUB_SEQ = i + 1;
                    _openUserSubjects.SUB = str;
                    if (_openUserSubjects.SUB == "01")
                    {
                        _openUserSubjects.SUBNM = "Punjabi";
                    }
                    else if (_openUserSubjects.SUB == "07")
                    {
                        _openUserSubjects.SUBNM = "Punjab History & Culture";
                    }
                    else
                    {
                        _openUserSubjects.SUBNM = subjects.Find(f => f.Value == _openUserSubjects.SUB).Text;
                    }

                    if (i < 6)
                    {
                        //  _openUserSubjects.SUBCAT = "R";
                        _openUserSubjects.SUBCAT = subcat_array[i];
                    }
                    else
                    { _openUserSubjects.SUBCAT = "A"; }
                        //else
                        //{
                        //    if ((_openUserSubjects.CLASS == "10" && i == 5) || (_openUserSubjects.CLASS == "12" && i == 5 && _openUserSubjects.STREAMCODE.Trim().ToUpper() == "C"))
                        //    { _openUserSubjects.SUBCAT = "R"; }
                        //    else
                        //    { _openUserSubjects.SUBCAT = "A"; }
                        //}



                        switch (i)
                    {
                        case 0:
                            cc[0] = (fc["Sub_1_Th_Obt"] != "0") ? fc["Sub_1_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_1_Th_Min"] != "0") ? fc["Sub_1_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_1_Th_Max"] != "0") ? fc["Sub_1_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_1_Pr_Obt"] != "0") ? fc["Sub_1_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_1_Pr_Min"] != "0") ? fc["Sub_1_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_1_Pr_Max"] != "0") ? fc["Sub_1_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_1_CCE_Obt"] != "0") ? fc["Sub_1_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_1_CCE_Min"] != "0") ? fc["Sub_1_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_1_CCE_Max"] != "0") ? fc["Sub_1_CCE_Max"] : string.Empty; break;
                        case 1:
                            cc[0] = (fc["Sub_2_Th_Obt"] != "0") ? fc["Sub_2_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_2_Th_Min"] != "0") ? fc["Sub_2_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_2_Th_Max"] != "0") ? fc["Sub_2_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_2_Pr_Obt"] != "0") ? fc["Sub_2_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_2_Pr_Min"] != "0") ? fc["Sub_2_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_2_Pr_Max"] != "0") ? fc["Sub_2_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_2_CCE_Obt"] != "0") ? fc["Sub_2_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_2_CCE_Min"] != "0") ? fc["Sub_2_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_2_CCE_Max"] != "0") ? fc["Sub_2_CCE_Max"] : string.Empty; break;
                        case 2:
                            cc[0] = (fc["Sub_3_Th_Obt"] != "0") ? fc["Sub_3_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_3_Th_Min"] != "0") ? fc["Sub_3_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_3_Th_Max"] != "0") ? fc["Sub_3_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_3_Pr_Obt"] != "0") ? fc["Sub_3_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_3_Pr_Min"] != "0") ? fc["Sub_3_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_3_Pr_Max"] != "0") ? fc["Sub_3_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_3_CCE_Obt"] != "0") ? fc["Sub_3_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_3_CCE_Min"] != "0") ? fc["Sub_3_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_3_CCE_Max"] != "0") ? fc["Sub_3_CCE_Max"] : string.Empty; break;
                        case 3:
                            cc[0] = (fc["Sub_4_Th_Obt"] != "0") ? fc["Sub_4_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_4_Th_Min"] != "0") ? fc["Sub_4_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_4_Th_Max"] != "0") ? fc["Sub_4_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_4_Pr_Obt"] != "0") ? fc["Sub_4_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_4_Pr_Min"] != "0") ? fc["Sub_4_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_4_Pr_Max"] != "0") ? fc["Sub_4_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_4_CCE_Obt"] != "0") ? fc["Sub_4_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_4_CCE_Min"] != "0") ? fc["Sub_4_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_4_CCE_Max"] != "0") ? fc["Sub_4_CCE_Max"] : string.Empty; break;
                        case 4:
                            cc[0] = (fc["Sub_5_Th_Obt"] != "0") ? fc["Sub_5_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_5_Th_Min"] != "0") ? fc["Sub_5_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_5_Th_Max"] != "0") ? fc["Sub_5_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_5_Pr_Obt"] != "0") ? fc["Sub_5_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_5_Pr_Min"] != "0") ? fc["Sub_5_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_5_Pr_Max"] != "0") ? fc["Sub_5_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_5_CCE_Obt"] != "0") ? fc["Sub_5_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_5_CCE_Min"] != "0") ? fc["Sub_5_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_5_CCE_Max"] != "0") ? fc["Sub_5_CCE_Max"] : string.Empty; break;
                        case 5:
                            if (_openUserSubjects.CLASS == "10" || (_openUserSubjects.CLASS == "12" && _openUserSubjects.STREAMCODE.Trim().ToUpper() == "C"))
                            {
                                cc[0] = (fc["Sub_6_Th_Obt"] != "0") ? fc["Sub_6_Th_Obt"] : string.Empty;
                                cc[1] = (fc["Sub_6_Th_Min"] != "0") ? fc["Sub_6_Th_Min"] : string.Empty;
                                cc[2] = (fc["Sub_6_Th_Max"] != "0") ? fc["Sub_6_Th_Max"] : string.Empty;
                                cc[3] = (fc["Sub_6_Pr_Obt"] != "0") ? fc["Sub_6_Pr_Obt"] : string.Empty;
                                cc[4] = (fc["Sub_6_Pr_Min"] != "0") ? fc["Sub_6_Pr_Min"] : string.Empty;
                                cc[5] = (fc["Sub_6_Pr_Max"] != "0") ? fc["Sub_6_Pr_Max"] : string.Empty;
                                cc[6] = (fc["Sub_6_CCE_Obt"] != "0") ? fc["Sub_6_CCE_Obt"] : string.Empty;
                                cc[7] = (fc["Sub_6_CCE_Min"] != "0") ? fc["Sub_6_CCE_Min"] : string.Empty;
                                cc[8] = (fc["Sub_6_CCE_Max"] != "0") ? fc["Sub_6_CCE_Max"] : string.Empty;
                            }
                            break;
                        default: for (int t = 0; t < 9; t++) { cc[t] = string.Empty; } break;
                    }

                    _openUserSubjects.OBTMARKS = cc[0];
                    _openUserSubjects.MINMARKS = cc[1];
                    _openUserSubjects.MAXMARKS = cc[2];
                    _openUserSubjects.OBTMARKSP = cc[3];
                    _openUserSubjects.MINMARKSP = cc[4];
                    _openUserSubjects.MAXMARKSP = cc[5];
                    _openUserSubjects.OBTMARKSCC = cc[6];
                    _openUserSubjects.MINMARKSCC = cc[7];
                    _openUserSubjects.MAXMARKSCC = cc[8];

                    if (_openUserSubjects.SUBCAT == "C")
                    {
                       if (_openUserSubjects.OBTMARKSCC == "000" || _openUserSubjects.OBTMARKSCC == "" || ol.CATEGORY.ToLower().Contains("direct"))
                        {
                            _openUserSubjects.OBTMARKS = "";
                            _openUserSubjects.MINMARKS = "";
                            _openUserSubjects.MAXMARKS = "";
                            _openUserSubjects.OBTMARKSP = "";
                            _openUserSubjects.MINMARKSP = "";
                            _openUserSubjects.MAXMARKSP = "";
                            _openUserSubjects.OBTMARKSCC = "";
                            _openUserSubjects.MINMARKSCC = "";
                            _openUserSubjects.MAXMARKSCC = "";
                            _openUserSubjects.SUBCAT = "R";
                        }
                    }
                    else
                    {
                        _openUserSubjects.OBTMARKS = "";
                        _openUserSubjects.MINMARKS = "";
                        _openUserSubjects.MAXMARKS = "";
                        _openUserSubjects.OBTMARKSP = "";
                        _openUserSubjects.MINMARKSP = "";
                        _openUserSubjects.MAXMARKSP = "";
                        _openUserSubjects.OBTMARKSCC = "";
                        _openUserSubjects.MINMARKSCC = "";
                        _openUserSubjects.MAXMARKSCC = "";
                    }

                    int ivalue = i; 
                    //C - (6)   R
                    //H - (5) R
                    if (i > 5)
                    {
                        if (_openUserSubjects.CLASS == "12" && i == 5 && _openUserSubjects.STREAMCODE.Trim().ToUpper() == "C")
                        { _openUserSubjects.SUBCAT = "R"; }
                        else
                        { _openUserSubjects.SUBCAT = "A"; }

                        //if (i > 6)
                        //{
                        //    _openUserSubjects.SUBCAT = "A";
                        //}
                        if (i > 5) // after commece 5 subject (reg)
                        {
                            _openUserSubjects.SUBCAT = "A";
                        }
                    }
                    else
                    {
                        if (_openUserSubjects.CLASS == "12" && i == 5 && _openUserSubjects.STREAMCODE.Trim().ToUpper() != "C")
                        {
                            _openUserSubjects.SUBCAT = "A";
                        }
                        else if (_openUserSubjects.CLASS == "12" && i == 5 && _openUserSubjects.STREAMCODE.Trim().ToUpper() == "C")
                        {
                            _openUserSubjects.SUBCAT = "A";
                        }
                    }

                   
                    _tblsubjectopenList.Add(new tblsubjectopen 
                    {                        
                        APPNO = _openUserSubjects.APPNO,
                        SUB = _openUserSubjects.SUB,
                        MEDIUM = "",
                        SUBCAT = _openUserSubjects.SUBCAT == null ? "" : _openUserSubjects.SUBCAT,
                        SUB_SEQ = _openUserSubjects.SUB_SEQ,
                        OBTMARKS = _openUserSubjects.OBTMARKS,
                        OBTMARKSP = _openUserSubjects.OBTMARKSP,
                        OBTMARKSCC = _openUserSubjects.OBTMARKSCC,
                        INSERTDT = _openUserSubjects.INSERTDT,
                        UPDT = Convert.ToDateTime("1/1/1900 12:00:00 AM"),
                        correctionid = "",
                        correction_dt = Convert.ToDateTime("1/1/1900 12:00:00 AM"),
                });

                   //// // InsertUserSubjects(_openUserSubjects);
                    i++;
                }
            }

            //if (_tblsubjectopenList.Count > 0)
            //{
            //    if (IsUserInSubjects(_openUserSubjects.APPNO) != 0)
            //    {
            //        // RemoveUserSubjects(_openUserSubjects.APPNO);
            //        _context.tblsubjectopen.Remove(_context.tblsubjectopen.FirstOrDefault(x => x.APPNO == _openUserSubjects.APPNO));
            //        _context.SaveChanges();
            //    }

                
            //    //Loop and insert records.
            //    foreach (tblsubjectopen opensubj in _tblsubjectopenList)
            //    {
            //        _context.tblsubjectopen.Add(opensubj);
            //    }
            //    int insertedRecords = _context.SaveChanges();
            //    //                    _context.SaveChanges();
            //    _context?.Dispose();

            //    if (ol.ISSUBJECT == 0)
            //    {
            //        ol.ISSUBJECT = 1;
            //        ol.UPDT = DateTime.Now;
            //        UpdateLoginUser(ol);
            //    }
            //}

        }


        public List<tblsubjectopen> checkInsertUserInSubjects(string[] subjects_array, string[] subcat_array, string app_class, string app_id, string app_stream, FormCollection fc)
        {

            List<tblsubjectopen> _tblsubjectopenList = new List<tblsubjectopen>();
            OpenUserSubjects _openUserSubjects = new OpenUserSubjects();
            _openUserSubjects.CLASS = app_class;
            _openUserSubjects.APPNO = app_id;
            _openUserSubjects.INSERTDT = DateTime.Now;

            if (app_class == "10")
            {
                _openUserSubjects.STREAM = "GENERAL";
                _openUserSubjects.STREAMCODE = "G";
            }
            else
            {
                List<SelectListItem> streams = GetStreams_1();
                _openUserSubjects.STREAM = app_stream;
                _openUserSubjects.STREAMCODE = streams.Find(f => f.Text == _openUserSubjects.STREAM).Value;
            }


            List<SelectListItem> subjects = new List<SelectListItem>();
            if (_openUserSubjects.CLASS == "10")
            {
                subjects = GetAllMatricSubjects();
            }
            else
            {
                subjects = GetAllSeniorSubjects();
            }


            int i = 0;
            //if (IsUserInSubjects(_openUserSubjects.APPNO) != 0)
            //{
            //    RemoveUserSubjects(_openUserSubjects.APPNO);
            //}


            OpenUserLogin ol = new OpenUserLogin();
            ol = GetLoginById(app_id);

            foreach (string str in subjects_array)
            {
                if (str != string.Empty && str != null)
                {
                    string[] cc = new string[9];
                    _openUserSubjects.SUB_SEQ = i + 1;
                    _openUserSubjects.SUB = str;
                    if (_openUserSubjects.SUB == "01")
                    {
                        _openUserSubjects.SUBNM = "Punjabi";
                    }
                    else if (_openUserSubjects.SUB == "07")
                    {
                        _openUserSubjects.SUBNM = "Punjab History & Culture";
                    }
                    else
                    {
                        _openUserSubjects.SUBNM = subjects.Find(f => f.Value == _openUserSubjects.SUB).Text;
                    }

                    if (i < 6)
                    {
                        //  _openUserSubjects.SUBCAT = "R";
                        _openUserSubjects.SUBCAT = subcat_array[i];
                    }
                    else
                    { _openUserSubjects.SUBCAT = "A"; }

                    switch (i)
                    {
                        case 0:
                            cc[0] = (fc["Sub_1_Th_Obt"] != "0") ? fc["Sub_1_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_1_Th_Min"] != "0") ? fc["Sub_1_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_1_Th_Max"] != "0") ? fc["Sub_1_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_1_Pr_Obt"] != "0") ? fc["Sub_1_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_1_Pr_Min"] != "0") ? fc["Sub_1_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_1_Pr_Max"] != "0") ? fc["Sub_1_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_1_CCE_Obt"] != "0") ? fc["Sub_1_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_1_CCE_Min"] != "0") ? fc["Sub_1_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_1_CCE_Max"] != "0") ? fc["Sub_1_CCE_Max"] : string.Empty; break;
                        case 1:
                            cc[0] = (fc["Sub_2_Th_Obt"] != "0") ? fc["Sub_2_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_2_Th_Min"] != "0") ? fc["Sub_2_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_2_Th_Max"] != "0") ? fc["Sub_2_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_2_Pr_Obt"] != "0") ? fc["Sub_2_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_2_Pr_Min"] != "0") ? fc["Sub_2_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_2_Pr_Max"] != "0") ? fc["Sub_2_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_2_CCE_Obt"] != "0") ? fc["Sub_2_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_2_CCE_Min"] != "0") ? fc["Sub_2_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_2_CCE_Max"] != "0") ? fc["Sub_2_CCE_Max"] : string.Empty; break;
                        case 2:
                            cc[0] = (fc["Sub_3_Th_Obt"] != "0") ? fc["Sub_3_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_3_Th_Min"] != "0") ? fc["Sub_3_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_3_Th_Max"] != "0") ? fc["Sub_3_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_3_Pr_Obt"] != "0") ? fc["Sub_3_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_3_Pr_Min"] != "0") ? fc["Sub_3_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_3_Pr_Max"] != "0") ? fc["Sub_3_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_3_CCE_Obt"] != "0") ? fc["Sub_3_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_3_CCE_Min"] != "0") ? fc["Sub_3_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_3_CCE_Max"] != "0") ? fc["Sub_3_CCE_Max"] : string.Empty; break;
                        case 3:
                            cc[0] = (fc["Sub_4_Th_Obt"] != "0") ? fc["Sub_4_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_4_Th_Min"] != "0") ? fc["Sub_4_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_4_Th_Max"] != "0") ? fc["Sub_4_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_4_Pr_Obt"] != "0") ? fc["Sub_4_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_4_Pr_Min"] != "0") ? fc["Sub_4_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_4_Pr_Max"] != "0") ? fc["Sub_4_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_4_CCE_Obt"] != "0") ? fc["Sub_4_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_4_CCE_Min"] != "0") ? fc["Sub_4_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_4_CCE_Max"] != "0") ? fc["Sub_4_CCE_Max"] : string.Empty; break;
                        case 4:
                            cc[0] = (fc["Sub_5_Th_Obt"] != "0") ? fc["Sub_5_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_5_Th_Min"] != "0") ? fc["Sub_5_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_5_Th_Max"] != "0") ? fc["Sub_5_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_5_Pr_Obt"] != "0") ? fc["Sub_5_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_5_Pr_Min"] != "0") ? fc["Sub_5_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_5_Pr_Max"] != "0") ? fc["Sub_5_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_5_CCE_Obt"] != "0") ? fc["Sub_5_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_5_CCE_Min"] != "0") ? fc["Sub_5_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_5_CCE_Max"] != "0") ? fc["Sub_5_CCE_Max"] : string.Empty; break;
                        case 5:
                            if (_openUserSubjects.CLASS == "10" || (_openUserSubjects.CLASS == "12" && _openUserSubjects.STREAMCODE.Trim().ToUpper() == "C"))
                            {
                                cc[0] = (fc["Sub_6_Th_Obt"] != "0") ? fc["Sub_6_Th_Obt"] : string.Empty;
                                cc[1] = (fc["Sub_6_Th_Min"] != "0") ? fc["Sub_6_Th_Min"] : string.Empty;
                                cc[2] = (fc["Sub_6_Th_Max"] != "0") ? fc["Sub_6_Th_Max"] : string.Empty;
                                cc[3] = (fc["Sub_6_Pr_Obt"] != "0") ? fc["Sub_6_Pr_Obt"] : string.Empty;
                                cc[4] = (fc["Sub_6_Pr_Min"] != "0") ? fc["Sub_6_Pr_Min"] : string.Empty;
                                cc[5] = (fc["Sub_6_Pr_Max"] != "0") ? fc["Sub_6_Pr_Max"] : string.Empty;
                                cc[6] = (fc["Sub_6_CCE_Obt"] != "0") ? fc["Sub_6_CCE_Obt"] : string.Empty;
                                cc[7] = (fc["Sub_6_CCE_Min"] != "0") ? fc["Sub_6_CCE_Min"] : string.Empty;
                                cc[8] = (fc["Sub_6_CCE_Max"] != "0") ? fc["Sub_6_CCE_Max"] : string.Empty;
                            }
                            break;
                        default: for (int t = 0; t < 9; t++) { cc[t] = string.Empty; } break;
                    }

                    _openUserSubjects.OBTMARKS = cc[0];
                    _openUserSubjects.MINMARKS = cc[1];
                    _openUserSubjects.MAXMARKS = cc[2];
                    _openUserSubjects.OBTMARKSP = cc[3];
                    _openUserSubjects.MINMARKSP = cc[4];
                    _openUserSubjects.MAXMARKSP = cc[5];
                    _openUserSubjects.OBTMARKSCC = cc[6];
                    _openUserSubjects.MINMARKSCC = cc[7];
                    _openUserSubjects.MAXMARKSCC = cc[8];

                    if (_openUserSubjects.SUBCAT == "C")
                    {
                        if (_openUserSubjects.OBTMARKSCC == "000" || _openUserSubjects.OBTMARKSCC == "" || ol.CATEGORY.ToLower().Contains("direct"))
                        {
                            _openUserSubjects.OBTMARKS = "";
                            _openUserSubjects.MINMARKS = "";
                            _openUserSubjects.MAXMARKS = "";
                            _openUserSubjects.OBTMARKSP = "";
                            _openUserSubjects.MINMARKSP = "";
                            _openUserSubjects.MAXMARKSP = "";
                            _openUserSubjects.OBTMARKSCC = "";
                            _openUserSubjects.MINMARKSCC = "";
                            _openUserSubjects.MAXMARKSCC = "";
                            _openUserSubjects.SUBCAT = "R";
                        }
                    }
                    else
                    {
                        _openUserSubjects.OBTMARKS = "";
                        _openUserSubjects.MINMARKS = "";
                        _openUserSubjects.MAXMARKS = "";
                        _openUserSubjects.OBTMARKSP = "";
                        _openUserSubjects.MINMARKSP = "";
                        _openUserSubjects.MAXMARKSP = "";
                        _openUserSubjects.OBTMARKSCC = "";
                        _openUserSubjects.MINMARKSCC = "";
                        _openUserSubjects.MAXMARKSCC = "";
                    }

                    int ivalue = i;
                    //C - (6)   R
                    //H - (5) R
                    if (i > 5)
                    {
                        if (_openUserSubjects.CLASS == "12" && i == 5 && _openUserSubjects.STREAMCODE.Trim().ToUpper() == "C")
                        { _openUserSubjects.SUBCAT = "R"; }
                        else
                        { _openUserSubjects.SUBCAT = "A"; }

                        //if (i > 6)
                        //{
                        //    _openUserSubjects.SUBCAT = "A";
                        //}
                        if (i > 5) // after commece 5 subject (reg)
                        {
                            _openUserSubjects.SUBCAT = "A";
                        }
                    }
                    else
                    {
                        if (_openUserSubjects.CLASS == "12" && i == 5 && _openUserSubjects.STREAMCODE.Trim().ToUpper() != "C")
                        {
                            _openUserSubjects.SUBCAT = "A";
                        }
                        else if (_openUserSubjects.CLASS == "12" && i == 5 && _openUserSubjects.STREAMCODE.Trim().ToUpper() == "C")
                        {
                            _openUserSubjects.SUBCAT = "A";
                        }
                    }


                    _tblsubjectopenList.Add(new tblsubjectopen
                    {
                        APPNO = _openUserSubjects.APPNO,
                        SUB = _openUserSubjects.SUB,
                        MEDIUM = "",
                        SUBCAT = _openUserSubjects.SUBCAT == null ? "" : _openUserSubjects.SUBCAT,
                        SUB_SEQ = _openUserSubjects.SUB_SEQ,
                        OBTMARKS = _openUserSubjects.OBTMARKS,
                        OBTMARKSP = _openUserSubjects.OBTMARKSP,
                        OBTMARKSCC = _openUserSubjects.OBTMARKSCC,
                        INSERTDT = _openUserSubjects.INSERTDT,
                        UPDT = Convert.ToDateTime("1/1/1900 12:00:00 AM"),
                        correctionid = "",
                        correction_dt = Convert.ToDateTime("1/1/1900 12:00:00 AM"),
                    });

                    if (_openUserSubjects.SUB == "01")
                    {
                        //Automatic Add/Remove 72 with 01 and 73 with 07 .. sub seq of this subject is 9. (Do it in backend, don't show on entry page)
//17. Show subject 72 with 01 and 73 with 07 in perview and admission forms etc.. 
                        _tblsubjectopenList.Add(new tblsubjectopen
                        {
                            APPNO = _openUserSubjects.APPNO,
                            SUB = "72",
                            MEDIUM = "",
                            SUBCAT = "R",
                            SUB_SEQ = 9,
                            OBTMARKS = "",
                            OBTMARKSP = "",
                            OBTMARKSCC = "",
                            INSERTDT = _openUserSubjects.INSERTDT,
                            UPDT = Convert.ToDateTime("1/1/1900 12:00:00 AM"),
                            correctionid = "",
                            correction_dt = Convert.ToDateTime("1/1/1900 12:00:00 AM"),
                        });
                    }
                    else if (_openUserSubjects.SUB == "07")
                    {
                        _tblsubjectopenList.Add(new tblsubjectopen
                        {
                            APPNO = _openUserSubjects.APPNO,
                            SUB = "73",
                            MEDIUM = "",
                            SUBCAT = "R",
                            SUB_SEQ = 9,
                            OBTMARKS = "",
                            OBTMARKSP = "",
                            OBTMARKSCC = "",
                            INSERTDT = _openUserSubjects.INSERTDT,
                            UPDT = Convert.ToDateTime("1/1/1900 12:00:00 AM"),
                            correctionid = "",
                            correction_dt = Convert.ToDateTime("1/1/1900 12:00:00 AM"),
                        });
                    }

                    //// // InsertUserSubjects(_openUserSubjects);
                    i++;
                }
            }




            return _tblsubjectopenList;

        }


        public int RemoveUserSubjects(string appno)
        {
            cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_RemoveUserSubjects";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@search", appno);
            con.Open();
            int res = cmd.ExecuteNonQuery();
            con.Close();
            return res;
        }

        private List<SelectListItem> GetAllSeniorSubjects()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            cmd.CommandText = "select sub,name_eng from ssnew  order by sub";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                }
            }

            return subjects;
        }

        public int InsertUserSubjects(OpenUserSubjects _openUserSubjects)
        {
            if (_openUserSubjects.APPNO == null || _openUserSubjects.APPNO == string.Empty)
            {
                return 0;
            }
            if (_openUserSubjects.CLASS == null || _openUserSubjects.CLASS == string.Empty)
            {
                return 0;
            }

            if (_openUserSubjects.correctionid == null) { _openUserSubjects.correctionid = string.Empty; }
            if (_openUserSubjects.correction_dt == new DateTime()) { _openUserSubjects.correction_dt = Convert.ToDateTime("1/1/1900 12:00:00 AM"); }
            if (_openUserSubjects.GROUP == null) { _openUserSubjects.GROUP = string.Empty; }
            if (_openUserSubjects.GROUPCODE == null) { _openUserSubjects.GROUPCODE = string.Empty; }
            if (_openUserSubjects.INSERTDT == new DateTime()) { _openUserSubjects.INSERTDT = Convert.ToDateTime("1/1/1900 12:00:00 AM"); }

            if (_openUserSubjects.MAXMARKS == null) { _openUserSubjects.MAXMARKS = string.Empty; }
            if (_openUserSubjects.MAXMARKSCC == null) { _openUserSubjects.MAXMARKSCC = string.Empty; }
            if (_openUserSubjects.MAXMARKSP == null) { _openUserSubjects.MAXMARKSP = string.Empty; }
            if (_openUserSubjects.MEDIUM == null) { _openUserSubjects.MEDIUM = string.Empty; }
            if (_openUserSubjects.MINMARKS == null) { _openUserSubjects.MINMARKS = string.Empty; }

            if (_openUserSubjects.MINMARKSCC == null) { _openUserSubjects.MINMARKSCC = string.Empty; }
            if (_openUserSubjects.MINMARKSP == null) { _openUserSubjects.MINMARKSP = string.Empty; }
            if (_openUserSubjects.OBTMARKS == null) { _openUserSubjects.OBTMARKS = string.Empty; }
            if (_openUserSubjects.OBTMARKSCC == null) { _openUserSubjects.OBTMARKSCC = string.Empty; }

            if (_openUserSubjects.OBTMARKSP == null) { _openUserSubjects.OBTMARKSP = string.Empty; }
            if (_openUserSubjects.SCHL == null) { _openUserSubjects.SCHL = string.Empty; }
            if (_openUserSubjects.STREAM == null) { _openUserSubjects.STREAM = string.Empty; }
            if (_openUserSubjects.STREAMCODE == null) { _openUserSubjects.STREAMCODE = string.Empty; }
            if (_openUserSubjects.SUB == null) { _openUserSubjects.SUB = string.Empty; }
            if (_openUserSubjects.SUBABBR == null) { _openUserSubjects.SUBABBR = string.Empty; }
            if (_openUserSubjects.SUBCAT == null) { _openUserSubjects.SUBCAT = string.Empty; }
            if (_openUserSubjects.SUBNM == null) { _openUserSubjects.SUBNM = string.Empty; }
            if (_openUserSubjects.TRADE == null) { _openUserSubjects.TRADE = string.Empty; }
            if (_openUserSubjects.TRADECODE == null) { _openUserSubjects.TRADECODE = string.Empty; }
            if (_openUserSubjects.UPDT == new DateTime()) { _openUserSubjects.UPDT = Convert.ToDateTime("1/1/1900 12:00:00 AM"); }


            if (IsSubjectInAppNo(_openUserSubjects.APPNO, _openUserSubjects.SUB) == 0)
            {
                cmd = new SqlCommand();
                cmd.CommandText = "sp_InsertSubjectsOpen";
            }
            else
            {
                cmd = new SqlCommand();
                cmd.CommandText = "sp_UpdateSubjectsOpen";
            }
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@APPNO", _openUserSubjects.APPNO.ToUpper());
            cmd.Parameters.AddWithValue("@CLASS", _openUserSubjects.CLASS.ToUpper());
            cmd.Parameters.AddWithValue("@OBTMARKSP", _openUserSubjects.OBTMARKSP.ToUpper());
            cmd.Parameters.AddWithValue("@SCHL", _openUserSubjects.SCHL.ToUpper());
            cmd.Parameters.AddWithValue("@SUB", _openUserSubjects.SUB.ToUpper());
            cmd.Parameters.AddWithValue("@SUBNM", _openUserSubjects.SUBNM.ToUpper());
            cmd.Parameters.AddWithValue("@SUBABBR", _openUserSubjects.SUBABBR.ToUpper());
            cmd.Parameters.AddWithValue("@MEDIUM", _openUserSubjects.MEDIUM.ToUpper());
            cmd.Parameters.AddWithValue("@SUBCAT", _openUserSubjects.SUBCAT.ToUpper());
            cmd.Parameters.AddWithValue("@OBTMARKS", _openUserSubjects.OBTMARKS.ToUpper());
            cmd.Parameters.AddWithValue("@MINMARKS", _openUserSubjects.MINMARKS.ToUpper());


            cmd.Parameters.AddWithValue("@MAXMARKS", _openUserSubjects.MAXMARKS.ToUpper());

            cmd.Parameters.AddWithValue("@MINMARKSP", _openUserSubjects.MINMARKSP.ToUpper());
            cmd.Parameters.AddWithValue("@MAXMARKSP", _openUserSubjects.MAXMARKSP.ToUpper());
            cmd.Parameters.AddWithValue("@STREAM", _openUserSubjects.STREAM.ToUpper());

            cmd.Parameters.AddWithValue("@STREAMCODE", _openUserSubjects.STREAMCODE.ToUpper());
            cmd.Parameters.AddWithValue("@GROUP", _openUserSubjects.GROUP.ToUpper());
            cmd.Parameters.AddWithValue("@GROUPCODE", _openUserSubjects.GROUPCODE.ToUpper());
            cmd.Parameters.AddWithValue("@TRADE", _openUserSubjects.TRADE.ToUpper());
            cmd.Parameters.AddWithValue("@TRADECODE", _openUserSubjects.TRADECODE.ToUpper());
            cmd.Parameters.AddWithValue("@INSERTDT", _openUserSubjects.INSERTDT);
            cmd.Parameters.AddWithValue("@UPDT", _openUserSubjects.UPDT);
            cmd.Parameters.AddWithValue("@SUB_SEQ", _openUserSubjects.SUB_SEQ);
            cmd.Parameters.AddWithValue("@OBTMARKSCC", _openUserSubjects.OBTMARKSCC.ToUpper());
            cmd.Parameters.AddWithValue("@MINMARKSCC", _openUserSubjects.MINMARKSCC.ToUpper());
            cmd.Parameters.AddWithValue("@MAXMARKSCC", _openUserSubjects.MAXMARKSCC.ToUpper());
            cmd.Parameters.AddWithValue("@correctionid", _openUserSubjects.correctionid.ToUpper());
            cmd.Parameters.AddWithValue("@correction_dt", _openUserSubjects.correction_dt);

            //int res = 0;
            //con.Open();
            //res = cmd.ExecuteNonQuery();
            //con.Close();

            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);

            return 0;
        }

        public int IsUserInSubjects(string appno)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_GetSubjectsForUser";
            cmd.Connection = con;
            string search = "where appno = " + appno;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@search", search);
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count >= 1)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public int IsSubjectInAppNo(string appno, string sub)
        {
            cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_GetSubjectsForUser";
            cmd.Connection = con;
            string search = "where APPNO='" + appno + "' and SUB='" + sub + "'";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@search", search);
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count >= 1)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public List<SelectListItem> GetSeniorSubjects_AddSubList_COMM()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            // New in 2023-24
            cmd.CommandText = "select sub, name_eng from ssnew  where sub in ('139','146','210','144','026')";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                }
            }

            return subjects;
        }
        public List<SelectListItem> GetSeniorSubjects_AddSubList_SCI()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            // New in 2023-24
            cmd.CommandText = "select sub, name_eng from ssnew where sub in ('139','146','210','028','054')";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                }
            }

            return subjects;
        }

        public List<SelectListItem> GetSeniorSubjects_AddSubList()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            //  Old
            cmd.CommandText = "select sub,name_eng from ssnew where sub in ('004','005','025','026','028','031','032','035','036','037','038','042','045','049','065','072','150','139','146','210')";
            // New in 2023-24
            //cmd.CommandText = "select sub, name_eng from ssnew where sub in ('139','146','210')";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                }
            }

            return subjects;
        }

        public List<SelectListItem> GetSeniorSubjects_MainSubjects()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            //  subjects.Add(new SelectListItem() { Value = "", Text = "--Select--" });
            cmd.CommandText = "select sub,name_eng from ssnew where sub in ('004','005','025','026','028','031','032','035','036','037','038','042','045','049','065','072','150')";
            // by harpal sir for 2023-24
            // cmd.CommandText = "select sub,name_eng from ssnew where sub in ('004','005','025','026','028','031','032','035','036','037','038','042','045','049','065','072','150')";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                }
            }

            return subjects;
        }

        public List<SelectListItem> GetSeniorSubjects_1()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            //  Old
            cmd.CommandText = "select sub,name_eng from ssnew where sub in ('004','005','025','026','028','031','032','035','036','037','038','042','045','049','065','072','146','150','139','210')";
            // New in 2023-24
            //cmd.CommandText = "select sub, name_eng from ssnew where sub in ('146', '139', '210')";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                }
            }

            return subjects;
        }

        public List<SelectListItem> GetSeniorSubjects_SCI_3_4()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            // subjects.Add(new SelectListItem() { Value = "", Text = "--Select--" });
            cmd.CommandText = "select sub,name_eng from ssnew where sub in ('052','053')";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                }
            }

            return subjects;
        }

        public List<SelectListItem> GetSeniorSubjects_SCI_5()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            // subjects.Add(new SelectListItem() { Value = "", Text = "--Select--" });
            cmd.CommandText = "select sub,name_eng from ssnew where sub in ('054','028')";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                }
            }

            return subjects;
        }

        public List<SelectListItem> GetSeniorSubjects_2()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            // subjects.Add(new SelectListItem() { Value = "", Text = "--Select--" });
            cmd.CommandText = "select sub,name_eng from ssnew where sub in ('052','053','054','028')";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                }
            }

            return subjects;
        }

        public List<SelectListItem> GetSeniorSubjects_3()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            // subjects.Add(new SelectListItem() { Value = "", Text = "--Select--" });
            cmd.CommandText = "select sub,name_eng from ssnew where sub in ('141','142','026','144')";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = con;
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                }
            }

            return subjects;
        }

        public List<OpenUserSubjects> GetSubjectsForUser(string app_no)
        {
            List<OpenUserSubjects> subjects_list = new List<OpenUserSubjects>();
            cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_GetSubjectsForUser";
            cmd.Connection = con;
            string search = "where appno = " + app_no;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@search", search);
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            OpenUserSubjects _openUserSubjects = new OpenUserSubjects();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                _openUserSubjects = new OpenUserSubjects();
                _openUserSubjects.APPNO = dr["APPNO"].ToString();
                _openUserSubjects.CLASS = dr["CLASS"].ToString();
                _openUserSubjects.correctionid = dr["correctionid"].ToString();
                _openUserSubjects.correction_dt = Convert.ToDateTime(dr["correction_dt"].ToString());
                _openUserSubjects.GROUP = dr["GROUP"].ToString();
                _openUserSubjects.GROUPCODE = dr["GROUPCODE"].ToString();
                _openUserSubjects.ID = Convert.ToInt32(dr["ID"].ToString());
                _openUserSubjects.INSERTDT = Convert.ToDateTime(dr["INSERTDT"].ToString());
                _openUserSubjects.MAXMARKS = dr["MAXMARKS"].ToString();
                _openUserSubjects.MAXMARKSCC = dr["MAXMARKSCC"].ToString();
                _openUserSubjects.MAXMARKSP = dr["MAXMARKSP"].ToString();
                _openUserSubjects.MEDIUM = dr["MEDIUM"].ToString();
                _openUserSubjects.MINMARKS = dr["MINMARKS"].ToString();
                _openUserSubjects.MINMARKSCC = dr["MINMARKSCC"].ToString();
                _openUserSubjects.MINMARKSP = dr["MINMARKSP"].ToString();
                _openUserSubjects.OBTMARKS = dr["OBTMARKS"].ToString();
                _openUserSubjects.OBTMARKSCC = dr["OBTMARKSCC"].ToString();
                _openUserSubjects.OBTMARKSP = dr["OBTMARKSP"].ToString();
                _openUserSubjects.SCHL = dr["SCHL"].ToString();
                _openUserSubjects.STREAM = dr["STREAM"].ToString();
                _openUserSubjects.STREAMCODE = dr["STREAMCODE"].ToString();
                _openUserSubjects.SUB = dr["SUB"].ToString();
                _openUserSubjects.SUBABBR = dr["SUBABBR"].ToString();
                _openUserSubjects.SUBCAT = dr["SUBCAT"].ToString();
                _openUserSubjects.SUBNM = dr["SUBNM"].ToString();
                _openUserSubjects.SUB_SEQ = Convert.ToInt32(dr["SUB_SEQ"].ToString());
                _openUserSubjects.TRADE = dr["TRADE"].ToString();
                _openUserSubjects.TRADECODE = dr["TRADECODE"].ToString();
                _openUserSubjects.UPDT = Convert.ToDateTime(dr["UPDT"].ToString());

                subjects_list.Add(_openUserSubjects);
            }

            return subjects_list;
        }
        public List<OpenUserSubjects> GetSubjectsForUser_New(string app_no, string cls, string yr)
        {
            List<OpenUserSubjects> subjects_list = new List<OpenUserSubjects>();
            cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_GetSubjectsForUser_New";
            cmd.Connection = con;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@appno", app_no);
            cmd.Parameters.AddWithValue("@CLASS", cls);
            cmd.Parameters.AddWithValue("@YEAR", yr);
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            OpenUserSubjects _openUserSubjects = new OpenUserSubjects();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                _openUserSubjects = new OpenUserSubjects();
                _openUserSubjects.APPNO = dr["APPNO"].ToString();
                _openUserSubjects.CLASS = dr["CLASS"].ToString();
                _openUserSubjects.correctionid = dr["correctionid"].ToString();
                _openUserSubjects.correction_dt = Convert.ToDateTime(dr["correction_dt"].ToString());
                _openUserSubjects.GROUP = dr["GROUP"].ToString();
                _openUserSubjects.GROUPCODE = dr["GROUPCODE"].ToString();
                _openUserSubjects.ID = Convert.ToInt32(dr["ID"].ToString());
                _openUserSubjects.INSERTDT = Convert.ToDateTime(dr["INSERTDT"].ToString());
                _openUserSubjects.MAXMARKS = dr["MAXMARKS"].ToString();
                _openUserSubjects.MAXMARKSCC = dr["MAXMARKSCC"].ToString();
                _openUserSubjects.MAXMARKSP = dr["MAXMARKSP"].ToString();
                _openUserSubjects.MEDIUM = dr["MEDIUM"].ToString();
                _openUserSubjects.MINMARKS = dr["MINMARKS"].ToString();
                _openUserSubjects.MINMARKSCC = dr["MINMARKSCC"].ToString();
                _openUserSubjects.MINMARKSP = dr["MINMARKSP"].ToString();
                _openUserSubjects.OBTMARKS = dr["OBTMARKS"].ToString();
                _openUserSubjects.OBTMARKSCC = dr["OBTMARKSCC"].ToString();
                _openUserSubjects.OBTMARKSP = dr["OBTMARKSP"].ToString();
                _openUserSubjects.SCHL = dr["SCHL"].ToString();
                _openUserSubjects.STREAM = dr["STREAM"].ToString();
                _openUserSubjects.STREAMCODE = dr["STREAMCODE"].ToString();
                _openUserSubjects.SUB = dr["SUB"].ToString();
                _openUserSubjects.SUBABBR = dr["SUBABBR"].ToString();
                _openUserSubjects.SUBCAT = dr["SUBCAT"].ToString();
                _openUserSubjects.SUBNM = dr["SUBNM"].ToString();
                _openUserSubjects.SUB_SEQ = Convert.ToInt32(dr["SUB_SEQ"].ToString());
                _openUserSubjects.TRADE = dr["TRADE"].ToString();
                _openUserSubjects.TRADECODE = dr["TRADECODE"].ToString();
                _openUserSubjects.UPDT = Convert.ToDateTime(dr["UPDT"].ToString());

                subjects_list.Add(_openUserSubjects);
            }

            return subjects_list;
        }

        public int mailer(string email, string app_no, string pwd, string name)
        {

            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(email));
            msg.Subject = "Confirm Your Registration - PSEB";
            msg.From = new MailAddress("noreply@psebonline.in", "psebonline.in");
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("ਪਿਆਰੇ <strong>" + name.ToUpper() + "</strong>");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("ਪੰਜਾਬ ਸਕੂਲ ਸਿੱਖਿਆ ਬੋਰਡ, ਓਪਨ ਸਕੂਲ ਪ੍ਰਣਾਲੀ ਅਧੀਨ ਆਨਲਾਈਨ ਦਾਖਲਾ/ਰਜਿਸਟਰੇਸ਼ਨ ਕਰਨ ਲਈ ਤੁਹਾਡਾ ਖਾਤਾ ਸਫਲਤਾਪੂਰਕ ਖੁਲ ਗਿਆ ਹੈ ਲਾਗਇਨ ਕਰਨ ਲਈ ਵੇਰਵੇ ਦਿੱਤੇ ਗਏ ਹਨ : -<br/>");
            sb.AppendLine("<center><table style='font-size:1.1em;font-weight:500;font-style:copperplate-gothic;'><tr><td>ਬਿਨੇ ਪੱਤਰ ਨੰ. / ਲਾਗਇਨ ਆਈ. ਡੀ</td><td>" + app_no + "</td></tr><tr><td>ਪਾਸਵਰਡ </td><td>" + pwd + "</td></tr></table>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<a href='https://registration2021.pseb.ac.in/Open'>ਲਾਗਇਨ ਕਰਨ ਲਈ ਇੱਥੇ ਕਲਿੱਕ ਕਰੋ</a></center><br/>");
            sb.AppendLine("<strong>ਜਰੂਰੀ ਨੋਟ :-</strong>");
            sb.AppendLine("<br/>");
            sb.AppendLine("1 ਆਨਲਾਇਨ ਫਾਰਮ ਭਰਨ ਤੋ ਪਹਿਲਾ ਬੋਰਡ ਵਲੋ ਜਾਰੀ ਦਾਖਲੇ ਸਬੰਧੀ ਹਦਾਇਤਾ ਜਰੁਰ ਪੜ ਲਈਆ ਜਾਣ ਅਤੇ ਫਾਰਮ ਭਰਨ ਸਮੇ ਵੇਰਵਿਆ ਨੂੰ ਧਿਆਨ ਨਾਲ ਚੈਕ ਕਰਕੇ ਹੀ ਸਬਮਿਟ ਕੀਤਾ ਜਾਵੇ, ਇਕ ਵਾਰ ਸਬਮਿਟ ਕਰਨ ਤੋ ਬਾਅਦ ਕੋਈ ਵੀ ਸੋਧ ਨਹੀ ਕੀਤੀ ਜਾ ਸਕੇਗੀ। ਇਸ ਸਬੰਧੀ ਸਾਰੀ ਜਿਮੇਵਾਰੀ ਪ੍ਰੀਖਿਆਰਥੀ ਦੀ ਹੋਵੇਗੀ।");
            sb.AppendLine("<br/>");
            sb.AppendLine("2 ਇਹ ਇੱਕ ਕੰਪਿਊਟਰ ਸਿਸਟਮ ਦੁਆਰਾ ਤਿਆਰ ਕੀਤੀ ਈ - ਮੇਲ ਹੈ ਅਤੇ ਇਸ ਈ - ਮੇਲ ਤੇ ਮੇਲ ਨਾ ਕੀਤੀ ਜਾਵੇ । ਕਿਰਪਾ ਕਰਕੇ ਇਸ ਈ - ਮੇਲ noreply@psebonline.in ਨੂੰ White List / Safe Sender List ਵਿੱਚ ਸ਼ਾਮਲ ਕਰੋ ਨਹੀ ਤਾ ISP (ਇੰਟਰਨੈੱਟ ਸਰਵਿਸ ਪ੍ਰੋਵਾਈਡਰ ) ਰਾਹੀ ਈ -ਮੇਲ ਪ੍ਰਾਪਤ ਕਰਨ ਤੋ ਰੋਕ ਲਗ ਸਕਦੀ ਹੈ.");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("<br/>ਤੁਹਾਡੀ ਸੇਵਾ ਵਿਚ,");
            sb.AppendLine("<br/>ਈ - ਓਪਨ ਸਕੂਲ ਟੀਮ ,");
            sb.AppendLine("<br/>ਪੰਜਾਬ ਸਕੂਲ ਸਿੱਖਿਆ ਬੋਰਡ");
            sb.AppendLine("<br/>ਮੋਹਾਲੀ।");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("ਓਪਨ ਸਕੂਲ ਸਬੰਧੀ ਕਿਸੇ ਵੀ ਜਾਣਕਾਰੀ ਲਈ ਸਾਡੇ ਨਾਲ ਸੰਪਰਕ ਕਰੋ : 0172-5227195, 5227197");
            sb.AppendLine("<br/><hr/>");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("Dear <strong>" + name.ToUpper() + "</strong>");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("Your Account For Punjab School Education Open School Online Registration has been Generated Successfully.<br/>");
            sb.AppendLine("Your Login Details are given Below :-");
            sb.AppendLine("<center><table style='font-size:1.1em;font-weight:500;'><tr><td><b>User ID / Login ID: </b></td><td>" + app_no + "</td></tr><tr><td><b>Password: </b></td><td>" + pwd + "</td></tr></table>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<a href='https://registration2021.pseb.ac.in/Open'><b>Click Here To Login</b></a></center><br/>");
            sb.AppendLine("<strong>Note:</strong> Please Read Instruction Carefully Before filling the Online Form");
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>This is a system generated e-mail and please do not reply. Add noreply@psebonline.in to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.");
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>Regards,");
            sb.AppendLine("<br/>e-Open School Team,");
            sb.AppendLine("<br/>Punjab School Education Board");
            sb.AppendLine("<br/>Contact Us: 0172-5227195, 5227197");

            msg.IsBodyHtml = true;
            msg.Body = sb.ToString();

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "mail.smtp2go.com";
            smtp.Port = 2525;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("noreply@psebonline.in", "YWZtam9qZWtrNHRr");
            smtp.EnableSsl = true;
            smtp.Send(msg);

            return 0;
        }

        public int IsChallanVerified(string appno, string challanId)
        {
            cmd.CommandText = "sp_ChallanMasterOpen";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            string search = "'" + appno + "' and CHALLANID = '" + challanId + "'";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@search", search);
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count >= 1)
            {
                challanId = ds.Tables[0].Rows[0]["VERIFIED"].ToString();
                return Convert.ToInt32(challanId);
            }
            else
            {
                challanId = "";
                return 0;
            }
        }

        public string IsValidForChallan(string app_id)
        {
            string res = string.Empty;
            OpenUserLogin _openUserLogin = GetLoginById(app_id);
            OpenUserRegistration _openUserRegistration = GetRegistrationRecord(app_id);
            List<OpenUserSubjects> _openUserSubjects = GetSubjectsForUser(app_id);

            if (_openUserLogin.AADHAR_NO.Trim() == string.Empty)
            {
                res += "Aadhar number, ";
            }
            if (_openUserLogin.CLASS.Trim() == string.Empty || (_openUserLogin.CLASS.Trim() != "12" && _openUserLogin.CLASS.Trim() != "10"))
            {
                res += "Class, ";
            }
            else
            {
                if (_openUserLogin.CLASS.Trim() == "12")
                {
                    if (!(_openUserSubjects.Count >= 5))
                    {
                        res += "Subjects, ";
                    }
                }
                else
                {
                    if (!(_openUserSubjects.Count >= 6))
                    {
                        res += "Subjects, ";
                    }
                }
            }
            if (string.IsNullOrEmpty(_openUserLogin.FORM.Trim()))
            {
                res += "Form, ";
            }
            if (string.IsNullOrEmpty(_openUserLogin.MOBILENO.Trim()))
            {
                res += "Mobile Number, ";
            }
            if (string.IsNullOrEmpty(_openUserLogin.DOB.Trim()))
            {
                res += "Date Of Birth, ";
            }
            if (string.IsNullOrEmpty(_openUserLogin.EMAILID.Trim()))
            {
                res += "Email Id, ";
            }
            if (string.IsNullOrEmpty(_openUserLogin.PWD.Trim()))
            {
                res += "Password, ";
            }
            if (string.IsNullOrEmpty(_openUserLogin.IMG_RAND.Trim()))
            {
                res += "Photo, ";
            }
            if (string.IsNullOrEmpty(_openUserLogin.IMGSIGN_RA.Trim()))
            {
                res += "Signature, ";
            }
            if (string.IsNullOrEmpty(_openUserLogin.ADDRESS.Trim()))
            {
                res += "Address, ";
            }
            if (string.IsNullOrEmpty(_openUserLogin.TEHSIL.Trim()))
            {
                res += "Tehsil, ";
            }
            if (string.IsNullOrEmpty(_openUserLogin.PINCODE.Trim()))
            {
                res += "Pincode, ";
            }
            if (string.IsNullOrEmpty(_openUserLogin.CATEGORY.Trim()))
            {
                res += "Category, ";
            }
            if (string.IsNullOrEmpty(_openUserLogin.HOMEDIST.Trim()))
            {
                res += "District, ";
            }
            if (string.IsNullOrEmpty(_openUserLogin.HOMEDISTNM.Trim()))
            {
                res += "District Name, ";
            }

            if (_openUserRegistration != null)
            {
                if (string.IsNullOrEmpty(_openUserRegistration.NAME))
                {
                    res += "Name, ";
                }
                if (string.IsNullOrEmpty(_openUserRegistration.FNAME))
                {
                    res += "Fathers Name, ";
                }
                if (string.IsNullOrEmpty(_openUserRegistration.MNAME))
                {
                    res += "Mothers Name, ";
                }
                if (string.IsNullOrEmpty(_openUserRegistration.SEX))
                {
                    res += "Gender, ";
                }
                //if (string.IsNullOrEmpty(_openUserRegistration.CASTE))
                //{
                //    res += "Caste, ";
                //}
                //if (string.IsNullOrEmpty(_openUserRegistration.RELIGION))
                //{
                //    res += "Religion, ";
                //}
                if (string.IsNullOrEmpty(_openUserRegistration.NATION))
                {
                    res += "Nation, ";
                }
                if (string.IsNullOrEmpty(_openUserRegistration.CAT))
                {
                    res += "Category, ";
                }
                if (_openUserRegistration.CLASS == "10" && _openUserLogin.CATEGORY.Trim().ToUpper().Contains("DIRECT"))
                { }
                else
                {
                    if (string.IsNullOrEmpty(_openUserRegistration.BOARD))
                    {
                        res += "Board, ";
                    }
                    if (string.IsNullOrEmpty(_openUserRegistration.OSESSION))
                    {
                        res += "Session, ";
                    }

                }
            }
            else
            {
                res += "registration form, ";
            }
            return res;
        }

        #region Study_Center

        public List<SelectListItem> GetStudyCenters(string dist, string stream)
        {
            List<SelectListItem> studyCenters = new List<SelectListItem>();
            List<SelectListItem> streams = GetStreams_1();
            if (stream.Trim() == "" || stream.ToUpper().Trim() == "GENERAL")
            { stream = "OMATRIC"; }
            else
            {
                SelectListItem sel = streams.Find(f => f.Text == stream.Trim());
                string strm = sel.Value;
                switch (strm)
                {
                    case "H": stream = "OHUM"; break;
                    case "S": stream = "OSCI"; break;
                    case "C": stream = "OCOMM"; break;
                    case "IG": stream = "OHUM,OSCI"; break;
                    default: stream = "OMATRIC"; break;
                }
            }

            cmd = new SqlCommand();
            cmd.CommandText = "sp_GetStudyCenterOpen";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@dist", dist);
            cmd.Parameters.AddWithValue("@stream", stream);

            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            adp.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    studyCenters.Add(new SelectListItem() { Text = dr["SCHLE"].ToString().ToUpper(), Value = dr["SCHL"].ToString().ToUpper() });
                }
            }
            if (studyCenters.Count != 1)
            {
                studyCenters.Insert(0, new SelectListItem() { Text = "--Select Study Center--", Value = "" });
            }

            return studyCenters;
        }

        public DataSet InsertStudyCenter(FormCollection fc, string dist, string app_no)
        {
            try
            {
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                cmd.CommandText = "sp_updateRegistrationSchoolDetails";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@schl", (fc["StudyCenter_1"] == null) ? string.Empty : fc["StudyCenter_1"].ToString().ToUpper());
                cmd.Parameters.AddWithValue("@schl2", (fc["StudyCenter_2"] == null) ? string.Empty : fc["StudyCenter_2"].ToString().ToUpper());
                cmd.Parameters.AddWithValue("@schl3", (fc["StudyCenter_3"] == null) ? string.Empty : fc["StudyCenter_3"].ToString().ToUpper());
                cmd.Parameters.AddWithValue("@dist", (dist == null) ? string.Empty : dist.ToUpper());
                cmd.Parameters.AddWithValue("@app_no", (app_no == null) ? string.Empty : app_no.ToUpper());

                //int x = 0;
                //con.Open();
                //x = cmd.ExecuteNonQuery();
                //con.Close();

                string[] dataArray = new string[4];
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds);
                if (ds != null && ds != new DataSet())
                {
                    OpenUserLogin ol = new OpenUserLogin();
                    ol = GetLoginById(app_no);
                    if (ol.ISSCHLCHOO == 0) { ol.ISCOMPLETE = 1; ol.ISSCHLCHOO = 1; ol.UPDT = DateTime.Now; UpdateLoginUser(ol); }
                }
                return ds;
            }
            catch (Exception e)
            {
                return new DataSet();
            }
        }

        public int Study_Center_Mailer(string appno, string email, string challanId)
        {
            OpenUserLogin ol = GetRecord(appno);
            string tehsilE = "";
            if (ol.TEHSIL != "")
            {
                tehsilE = new AbstractLayer.DBClass().GetAllTehsil().Where(s => s.Value == ol.TEHSIL).FirstOrDefault().Text;
            }
            string FullAddress = ol.ADDRESS + "," + ol.HOMEDISTNM + "," + tehsilE + "," + ol.PINCODE;
            //email = "dexter9k9@gmail.com";
            //email = "rohit.nanda@ethical.in";
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(email));
            msg.Subject = "Study Centre Selection";
            msg.From = new MailAddress("noreply@psebonline.in", "psebonline.in");
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("ਸਤਿਕਾਰਯੋਗ ਪ੍ਰਿੰਸੀਪਲ/ਹੈਡਮਾਸਟਰ,");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("ਪੰਜਾਬ ਸਕੂਲ ਸਿੱਖਿਆ ਬੋਰਡ ਦੀ ਓਪਨ ਸਕੂਲ ਪ੍ਰਣਾਲੀ ਅਧੀਨ ਸੈਸ਼ਨ 2023-24 ਲਈ ਵਿਦਿਆਰਥੀ ਬਿਨੈ ਪੱਤਰ ਨੰ: " + ol.APPNO + " ਆਨਲਾਈਨ, ਸ਼੍ਰੇਣੀ " + ol.CLASS + " ਲਈ ਤੁਹਾਡੇ ਅਧਿਐਨ ਕੇਂਦਰ ਵਿੱਚ Enroll ਹੋ ਗਿਆ ਹੈ|<br/>");
            sb.AppendLine("ਵਿਦਿਆਰਥੀ ਦੇ ਵੇਰਵੇ ਹੇਠ ਲਿਖੇ ਅਨੁਸਾਰ ਹਨ:");
            sb.AppendLine("<table style='font-size:1.1em;font-weight:500;font-style:copperplate-gothic;'><tr><td>ਨਾਮ: </td><td>" + ol.NAME + "</td></tr><tr><td>ਪਤਾ: </td><td>" + FullAddress + "</td></tr><tr><td>ਫੋਨ ਨੰ: </ td><td>" + ol.MOBILENO + "</td></tr><tr><td>ਚਲਾਨ ਆਈ.ਡੀ. </ td><td>" + challanId + "</td></tr></table>");
            sb.AppendLine("<br/>");
            sb.AppendLine("ਜ਼ਰੂਰੀ ਨੋਟ:- ਅਧਿਐਨ ਕੇਂਦਰ ਨੂੰ ਸਲਾਹ ਦਿੱਤੀ ਜਾਂਦੀ ਹੈ ਕਿ ਉਪਰੋਕਤ ਈ-ਮੇਲ ਪ੍ਰਾਪਤ ਹੋਣ ਉਪਰੰਤ ਵਿਦਿਆਰਥੀ ਨਾਲ ਤੁਰੰਤ ਸੰਪਰਕ ਕੀਤਾ ਜਾਵੇ|<br/>");
            sb.AppendLine("<br/>ਈ-ਓਪਨ ਸਕੂਲ ਟੀਮ,");
            sb.AppendLine("<br/>ਪੰਜਾਬ ਸਕੂਲ ਸਿੱਖਿਆ ਬੋਰਡ,");
            sb.AppendLine("<br/>ਐੱਸ.ਏ.ਐੱਸ. ਨਗਰ (ਮੋਹਾਲੀ)");
            sb.AppendLine("<br/><br/>ਓਪਨ ਸਕੂਲ ਸਬੰਧੀ ਕਿਸੇ ਵੀ ਜਾਣਕਾਰੀ ਲਈ ਸਾਡੇ ਨਾਲ ਸੰਪਰਕ ਕਰੋ: 0172-5227195, 197");
            sb.AppendLine("<br/>ਟੋਲ ਫਰੀ ਨੰ: 18002700280");
            sb.AppendLine("<br/><br/><hr/><br/><br/>");
            sb.AppendLine("Respected Principal/Headmaster,");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("Student Application no " + ol.APPNO + " has been successfully enrolled under Open School for class " + ol.CLASS + "<sup>th</sup>. For the session 2023-24 in your Study Centre.");
            sb.AppendLine("<br/>");
            sb.AppendLine("Student details are given below:");
            sb.AppendLine("<table style='font-size:1.1em;font-weight:500;font-style:copperplate-gothic;'><tr><td>Student Name </td><td>" + ol.NAME + "</td></tr><tr><td>Address </td><td>" + FullAddress + "</td></tr><tr><td>Mobile No</ td><td>" + ol.MOBILENO + "</td></tr><tr><td>Challan Id</ td><td>" + challanId + "</td></tr></table>");
            sb.AppendLine("<br/>");
            sb.AppendLine("Note: Study centres are advised to contact the student immediately on receiving the e-mail message.<br/>");
            sb.AppendLine("<br/>Regards,");
            sb.AppendLine("<br/>e-Open School Team,");
            sb.AppendLine("<br/>Punjab School Education Board");
            sb.AppendLine("<br/>Contact us: 0172-5227195, 197");
            sb.AppendLine("<br/>Toll free no- 18002700280");

            msg.IsBodyHtml = true;
            msg.Body = sb.ToString();

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "mail.smtp2go.com";
            smtp.Port = 2525;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("noreply@psebonline.in", "YWZtam9qZWtrNHRr");
            smtp.EnableSsl = true;
            smtp.Send(msg);

            return 0;
        }

        #endregion Study_Center

        #region Fee

        public int IsUserInChallan(string appno, out string challanId)
        {
            cmd.CommandText = "sp_ChallanMasterOpen";
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@search", "'" + appno.ToString() + "'");
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count >= 1)
            {
                challanId = ds.Tables[0].Rows[0]["CHALLANID"].ToString();
                return 1;
            }
            else
            {
                challanId = "";
                return 0;
            }
        }

        public FeeOpen spFeeDetailsOpen2017Admin(string AppNo, DateTime dt1)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spFeeDetailsOpen2017Admin", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@APPNO", AppNo);
                    cmd.Parameters.AddWithValue("@Challandt", dt1);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    try
                    {
                        if (result.Tables[0].Rows.Count > 0)
                        {
                            FeeOpen _feeOpen = new FeeOpen();
                            DataRow dr = result.Tables[0].Rows[0];
                            _feeOpen.SCHL = dr["SCHL"].ToString();
                            _feeOpen.AppNo = dr["APPNO"].ToString();
                            _feeOpen.FeeCode = dr["FEECODE"].ToString();
                            _feeOpen.FeeCat = dr["FEECAT"].ToString();
                            _feeOpen.ID = Convert.ToInt32(dr["Id"].ToString());
                            _feeOpen.FORM = dr["form"].ToString();
                            _feeOpen.EndDate = dr["eDate"].ToString();
                            _feeOpen.BankLastDate = Convert.ToDateTime(dr["BankLastDate"].ToString());
                            _feeOpen.LateFee = Convert.ToInt32(dr["latefee"].ToString());
                            _feeOpen.ProsFee = Convert.ToInt32(dr["prosfee"].ToString());
                            _feeOpen.RegConti = Convert.ToInt32(dr["RegConti"].ToString());
                            _feeOpen.RegContiCat = dr["RegContiCat"].ToString();
                            _feeOpen.AdmissionFee = Convert.ToInt32(dr["AdmissionFee"].ToString());
                            _feeOpen.AddSubFee = Convert.ToInt32(dr["AddSubFee"].ToString());
                            _feeOpen.NoAddSub = Convert.ToInt32(dr["NoAddSub"].ToString());
                            _feeOpen.TotalFee = Convert.ToInt32(dr["TotalFee"].ToString());
                            _feeOpen.TotalFeesInWords = dr["TotalFeesInWords"].ToString();
                            // ExamFee
                            DataRow dr1 = result.Tables[1].Rows[0];
                            _feeOpen.ExamRegFee = Convert.ToInt32(dr1["Fee"].ToString());
                            _feeOpen.ExamLateFee = Convert.ToInt32(dr1["LateFee"].ToString());
                            _feeOpen.ExamTotalFee = Convert.ToInt32(dr1["TotFee"].ToString());
                            _feeOpen.ExamNOAS = Convert.ToInt32(dr1["NOAS"].ToString());
                            _feeOpen.ExamNOPS = Convert.ToInt32(dr1["NOPS"].ToString());
                            _feeOpen.ExamPrSubFee = Convert.ToInt32(dr1["PrSubFee"].ToString());
                            _feeOpen.ExamAddSubFee = Convert.ToInt32(dr1["AddSubFee"].ToString());
                            _feeOpen.ExamStartDate = dr1["sDate"].ToString();
                            _feeOpen.ExamEndDate = dr1["eDate"].ToString();
                            _feeOpen.ExamBankLastDate = Convert.ToDateTime(dr1["BankLastDate"].ToString());                          
                            return _feeOpen;
                        }
                        else
                        {
                            return new FeeOpen();
                        }
                    }
                    catch (Exception ex)
                    {
                        return new FeeOpen();
                    }
                }
            }
            catch (Exception ex)
            {
                return new FeeOpen();
            }
        }

        public FeeOpen spFeeDetailsOpen2017_Admin_Phy_Chln(string AppNo, DateTime dt1, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spFeeDetailsOpen2017_Admin_Phy_Chln", con);//spFeeDetailsOpen2017 // spFeeDetailsOpen2017_Phy_Chln
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@APPNO", AppNo);
                    cmd.Parameters.AddWithValue("@Challandt", dt1);
                    con.Open();
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    try
                    {
                        if (result.Tables[0].Rows.Count > 0)
                        {
                            FeeOpen _feeOpen = new FeeOpen();
                            DataRow dr = result.Tables[0].Rows[0];
                            _feeOpen.SCHL = dr["SCHL"].ToString();
                            _feeOpen.AppNo = dr["APPNO"].ToString();
                            _feeOpen.FeeCode = dr["FEECODE"].ToString();
                            _feeOpen.FeeCat = dr["FEECAT"].ToString();
                            _feeOpen.ID = Convert.ToInt32(dr["Id"].ToString());
                            _feeOpen.FORM = dr["form"].ToString();
                            _feeOpen.EndDate = dr["eDate"].ToString();
                            _feeOpen.BankLastDate = Convert.ToDateTime(dr["BankLastDate"].ToString());
                            _feeOpen.LateFee = Convert.ToInt32(dr["latefee"].ToString());
                            _feeOpen.ProsFee = Convert.ToInt32(dr["prosfee"].ToString());
                            _feeOpen.RegConti = Convert.ToInt32(dr["RegConti"].ToString());
                            _feeOpen.RegContiCat = dr["RegContiCat"].ToString();
                            _feeOpen.AdmissionFee = Convert.ToInt32(dr["AdmissionFee"].ToString());
                            _feeOpen.AddSubFee = Convert.ToInt32(dr["AddSubFee"].ToString());
                            _feeOpen.NoAddSub = Convert.ToInt32(dr["NoAddSub"].ToString());
                            _feeOpen.TotalFee = Convert.ToInt32(dr["TotalFee"].ToString());
                            _feeOpen.TotalFeesInWords = dr["TotalFeesInWords"].ToString();
                            // ExamFee
                            DataRow dr1 = result.Tables[1].Rows[0];
                            _feeOpen.ExamRegFee = Convert.ToInt32(dr1["Fee"].ToString());
                            _feeOpen.ExamLateFee = Convert.ToInt32(dr1["LateFee"].ToString());
                            _feeOpen.ExamTotalFee = Convert.ToInt32(dr1["TotFee"].ToString());
                            _feeOpen.ExamNOAS = Convert.ToInt32(dr1["NOAS"].ToString());
                            _feeOpen.ExamNOPS = Convert.ToInt32(dr1["NOPS"].ToString());
                            _feeOpen.ExamPrSubFee = Convert.ToInt32(dr1["PrSubFee"].ToString());
                            _feeOpen.ExamAddSubFee = Convert.ToInt32(dr1["AddSubFee"].ToString());
                            _feeOpen.ExamStartDate = dr1["sDate"].ToString();
                            _feeOpen.ExamEndDate = dr1["eDate"].ToString();
                            _feeOpen.ExamBankLastDate = Convert.ToDateTime(dr1["BankLastDate"].ToString());
                            OutError = "1";
                            return _feeOpen;
                        }
                        else
                        {
                            OutError = "0";
                            return new FeeOpen();
                        }
                    }
                    catch (Exception ex)
                    {
                        OutError = ex.Message;
                        return new FeeOpen();
                    }
                }
            }
            catch (Exception ex)
            {
                OutError = ex.Message;
                return new FeeOpen();
            }
        }



        public FeeOpen spFeeDetailsOpen2017(string AppNo, DateTime dt1, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spFeeDetailsOpen2017_Phy_Chln", con);//spFeeDetailsOpen2017 // spFeeDetailsOpen2017_Phy_Chln
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@APPNO", AppNo);
                    cmd.Parameters.AddWithValue("@Challandt", dt1);
                    con.Open();
                    ad.SelectCommand = cmd;
                    ad.Fill(result);                    
                    try
                    {
                        if (result.Tables[0].Rows.Count > 0)
                        {
                            FeeOpen _feeOpen = new FeeOpen();
                            DataRow dr = result.Tables[0].Rows[0];
                            _feeOpen.SCHL = dr["SCHL"].ToString();
                            _feeOpen.AppNo = dr["APPNO"].ToString();
                            _feeOpen.FeeCode = dr["FEECODE"].ToString();
                            _feeOpen.FeeCat = dr["FEECAT"].ToString();
                            _feeOpen.ID = Convert.ToInt32(dr["Id"].ToString());
                            _feeOpen.FORM = dr["form"].ToString();
                            _feeOpen.EndDate = dr["eDate"].ToString();
                            _feeOpen.BankLastDate = Convert.ToDateTime(dr["BankLastDate"].ToString());
                            _feeOpen.LateFee = Convert.ToInt32(dr["latefee"].ToString());
                            _feeOpen.ProsFee = Convert.ToInt32(dr["prosfee"].ToString());
                            _feeOpen.RegConti = Convert.ToInt32(dr["RegConti"].ToString());
                            _feeOpen.RegContiCat = dr["RegContiCat"].ToString();
                            _feeOpen.AdmissionFee = Convert.ToInt32(dr["AdmissionFee"].ToString());
                            _feeOpen.AddSubFee = Convert.ToInt32(dr["AddSubFee"].ToString());
                            _feeOpen.NoAddSub = Convert.ToInt32(dr["NoAddSub"].ToString());
                            _feeOpen.TotalFee = Convert.ToInt32(dr["TotalFee"].ToString());
                            _feeOpen.TotalFeesInWords = dr["TotalFeesInWords"].ToString();                          
                            // ExamFee
                            DataRow dr1 = result.Tables[1].Rows[0];
                            _feeOpen.ExamRegFee = Convert.ToInt32(dr1["Fee"].ToString());
                            _feeOpen.ExamLateFee = Convert.ToInt32(dr1["LateFee"].ToString());
                            _feeOpen.ExamTotalFee = Convert.ToInt32(dr1["TotFee"].ToString());
                            _feeOpen.ExamNOAS = Convert.ToInt32(dr1["NOAS"].ToString());
                            _feeOpen.ExamNOPS = Convert.ToInt32(dr1["NOPS"].ToString());
                            _feeOpen.ExamPrSubFee = Convert.ToInt32(dr1["PrSubFee"].ToString());
                            _feeOpen.ExamAddSubFee= Convert.ToInt32(dr1["AddSubFee"].ToString());
                            _feeOpen.ExamStartDate = dr1["sDate"].ToString();
                            _feeOpen.ExamEndDate = dr1["eDate"].ToString();
                            _feeOpen.ExamBankLastDate = Convert.ToDateTime(dr1["BankLastDate"].ToString());
                            _feeOpen.HardCopyCertificateFee = Convert.ToInt32(dr1["HardCopyCertificateFee"].ToString());
                            OutError = "1";
                            return _feeOpen;
                        }
                        else
                        {
                            OutError = "0";
                            return new FeeOpen();
                        }
                    }
                    catch (Exception ex)
                    {
                        OutError = ex.Message;
                        return new FeeOpen();
                    }
                }
            }
            catch (Exception ex)
            {
                OutError = ex.Message;
                return new FeeOpen();
            }
        }

        public string OpenInsertPaymentForm(ChallanMasterModel CM, FormCollection frm, out string SchoolMobile)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("OpenInsertPaymentFormSP", con);   //InsertPaymentFormSPTest  // [InsertPaymentFormSP_Rohit]
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@APPNO", CM.APPNO);
                cmd.Parameters.AddWithValue("@CHLNDATE", CM.CHLNDATE);
                cmd.Parameters.AddWithValue("@CHLNVDATE", CM.CHLNVDATE);
                cmd.Parameters.AddWithValue("@FEEMODE", CM.FEEMODE);
                cmd.Parameters.AddWithValue("@FEECODE", CM.FEECODE);
                cmd.Parameters.AddWithValue("@FEECAT", CM.FEECAT);
                cmd.Parameters.AddWithValue("@BCODE", CM.BCODE);
                cmd.Parameters.AddWithValue("@BANK", CM.BANK);
                cmd.Parameters.AddWithValue("@ACNO", CM.ACNO);
                cmd.Parameters.AddWithValue("@FEE", CM.FEE);
                cmd.Parameters.AddWithValue("@BANKCHRG", CM.BANKCHRG);
                cmd.Parameters.AddWithValue("@TOTFEE", CM.TOTFEE);
                cmd.Parameters.AddWithValue("@SCHLREGID", CM.SCHLREGID);
                cmd.Parameters.AddWithValue("@DIST", CM.DIST);
                cmd.Parameters.AddWithValue("@DISTNM", CM.DISTNM);
                cmd.Parameters.AddWithValue("@SCHLCANDNM", CM.SCHLCANDNM);
                cmd.Parameters.AddWithValue("@BRCODE", CM.BRCODE);
                cmd.Parameters.AddWithValue("@BRANCH", CM.BRANCH);
                cmd.Parameters.AddWithValue("@addfee", CM.addfee);
                cmd.Parameters.AddWithValue("@latefee", CM.latefee);
                cmd.Parameters.AddWithValue("@prosfee", CM.prosfee);
                cmd.Parameters.AddWithValue("@addsubfee", CM.addsubfee);
                cmd.Parameters.AddWithValue("@add_sub_count", CM.add_sub_count);
                cmd.Parameters.AddWithValue("@regfee", CM.regfee);
                cmd.Parameters.AddWithValue("@type", CM.type);
                cmd.Parameters.AddWithValue("@LOT", CM.LOT);
                cmd.Parameters.AddWithValue("@FeeStudentList", CM.FeeStudentList);
                if (CM.LSFRemarks != null && CM.LSFRemarks != "")
                {
                    cmd.Parameters.AddWithValue("@LumsumFine", CM.LumsumFine);
                    cmd.Parameters.AddWithValue("@LSFRemarks", CM.LSFRemarks);
                }
                cmd.Parameters.AddWithValue("@ChallanVDateN", CM.ChallanVDateN);
                //
                cmd.Parameters.AddWithValue("@OpenExamFee", CM.OpenExamFee);
                cmd.Parameters.AddWithValue("@OpenLateFee", CM.OpenLateFee);
                cmd.Parameters.AddWithValue("@OpenTotalFee", CM.OpenTotalFee);
                //
                SqlParameter outPutParameter = new SqlParameter();
                outPutParameter.ParameterName = "@CHALLANID";
                outPutParameter.Size = 100;
                outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                outPutParameter.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(outPutParameter);
                SqlParameter outPutParameter1 = new SqlParameter();
                outPutParameter1.ParameterName = "@SchoolMobile";
                outPutParameter1.Size = 20;
                outPutParameter1.SqlDbType = System.Data.SqlDbType.VarChar;
                outPutParameter1.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(outPutParameter1);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = (string)cmd.Parameters["@CHALLANID"].Value;
                SchoolMobile = (string)cmd.Parameters["@SchoolMobile"].Value;
                return outuniqueid;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                SchoolMobile = "";
                return result = "";

            }
            finally
            {
                con.Close();
            }
        }

        public DataSet GetOpenChallanByAppNo(string AppNo)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetOpenChallanByAppNoSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@AppNo", AppNo);
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


        #endregion Fee

        #region ApplicationForm
        public DataSet GetApplicationFormById(string AppNo)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetApplicationFormByIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@AppNo", AppNo);
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
        #endregion ApplicationForm

        #region ChangePassword

        public int ChangePassword(int UserId, string CurrentPassword, string NewPassword)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("OpenChangePasswordSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@OldPwd", CurrentPassword);
                    cmd.Parameters.AddWithValue("@NewPwd", NewPassword);
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    return result;

                }
            }
            catch (Exception ex)
            {
                return result = -1;
            }
            finally
            {
                // con.Close();
            }
        }
        #endregion ChangePassword

        #region SchoolOpen
        public DataSet OpenStudentlist(string search, string clas, int PageNumber, int type)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("OpenStudentlistSP", con);  //SelectPrintList_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@type", type); // O for Admin 1 for School else Openstudent
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@class", clas);
                    cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", 20);
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

        public DataSet OpenStudentlistAdmin(string search, string dist, int PageNumber)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("D_OpenStudentlistSP", con);  //SelectPrintList_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@Dist", dist);
                    cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", 20);
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

        public int UpdateStudyCenter(OpenUserRegistration _oUserRegistration)
        {
            try
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = con;
                cmd.CommandText = "sp_Update_Dist_StudyCenter";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@dist", _oUserRegistration.DIST.Trim());
                cmd.Parameters.AddWithValue("@schl1", _oUserRegistration.SCHL1.Trim());
                cmd.Parameters.AddWithValue("@schl2", _oUserRegistration.SCHL2.Trim());
                cmd.Parameters.AddWithValue("@schl3", _oUserRegistration.SCHL3.Trim());
                cmd.Parameters.AddWithValue("@appno", (float)Convert.ToDecimal(_oUserRegistration.APPNO.Trim()));

                int x = 0;
                con.Open();
                x = cmd.ExecuteNonQuery();
                con.Close();
                return x;
            }
            catch (Exception e)
            {
                return -1;
            }

        }
        #endregion SchoolOpen


        //#region ApplicationForm
        //public DataSet GetApplicationFormById(string AppNo)
        //{
        //    DataSet result = new DataSet();
        //    SqlDataAdapter ad = new SqlDataAdapter();
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
        //        {
        //            SqlCommand cmd = new SqlCommand("GetApplicationFormByIdSP", con);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@AppNo", AppNo);
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
        //#endregion ApplicationForm

        //#region ChangePassword

        //public int ChangePassword(int UserId, string CurrentPassword, string NewPassword)
        //{
        //    int result;
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
        //        {
        //            SqlCommand cmd = new SqlCommand("OpenChangePasswordSP", con);
        //            cmd.CommandType = CommandType.StoredProcedure;                   
        //            cmd.Parameters.AddWithValue("@UserId", UserId);
        //            cmd.Parameters.AddWithValue("@OldPwd", CurrentPassword);
        //            cmd.Parameters.AddWithValue("@NewPwd", NewPassword);
        //            con.Open();
        //            result = cmd.ExecuteNonQuery();
        //            return result;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return result = -1;
        //    }
        //    finally
        //    {
        //        // con.Close();
        //    }
        //}
        //#endregion ChangePassword


        public int UpdateOpenRegistrationByAdmin(string adminid, OpenUserRegistration _openUserRegistration, string imgPhoto, string imgSign, OpenUserLogin _openUserLogin,string EmpUserId)
        {
            if (_openUserRegistration.APPNO == null) { return 0; }         
            try
            {
                OpenUserRegistration _oUserRegistration = GetRegistrationRecord(_openUserRegistration.APPNO);
                _openUserRegistration.INSERTDT = _oUserRegistration.INSERTDT;
                _openUserRegistration.UPDT = DateTime.Now;
            }
            catch (Exception)
            {
                _openUserRegistration.INSERTDT = DateTime.Now;
                _openUserRegistration.UPDT = DateTime.Now;
            }
            if (_openUserRegistration.SCHLUPD_DT == new DateTime()) { _openUserRegistration.SCHLUPD_DT = Convert.ToDateTime("1/1/1753 12:00:00 AM"); }
            if (_openUserRegistration.correction_dt == new DateTime()) { _openUserRegistration.correction_dt = Convert.ToDateTime("1/1/1753 12:00:00 AM"); }

            if (_openUserRegistration.AADHAR_NO == null) { _openUserRegistration.AADHAR_NO = string.Empty; }

            if (_openUserRegistration.YEAR == null) { _openUserRegistration.YEAR = @PSEBONLINE.Repository.SessionSettingMastersRepository.GetSessionSettingMasters().SessionShortYear; }
            if (_openUserRegistration.SET == null) { _openUserRegistration.SET = string.Empty; }
            if (_openUserRegistration.CLASS == null) { _openUserRegistration.CLASS = string.Empty; }
            if (_openUserRegistration.FORM == null) { _openUserRegistration.FORM = string.Empty; }
            if (_openUserRegistration.DIST == null) { _openUserRegistration.DIST = string.Empty; }
            if (_openUserRegistration.RP == null) { _openUserRegistration.RP = "O"; }
            if (_openUserRegistration.EXAM == null) { _openUserRegistration.EXAM = string.Empty; }
            if (_openUserRegistration.SCHL == null) { _openUserRegistration.SCHL = string.Empty; }
            if (_openUserRegistration.REGNO == null) { _openUserRegistration.REGNO = string.Empty; }
            if (_openUserRegistration.NAME == null) { _openUserRegistration.NAME = string.Empty; }
            if (_openUserRegistration.PNAME == null) { _openUserRegistration.PNAME = string.Empty; }
            if (_openUserRegistration.FNAME == null) { _openUserRegistration.FNAME = string.Empty; }
            if (_openUserRegistration.PFNAME == null) { _openUserRegistration.PFNAME = string.Empty; }
            if (_openUserRegistration.MNAME == null) { _openUserRegistration.MNAME = string.Empty; }
            if (_openUserRegistration.PMNAME == null) { _openUserRegistration.PMNAME = string.Empty; }
            if (_openUserRegistration.DOB == null) { _openUserRegistration.DOB = string.Empty; }
            if (_openUserRegistration.PHY_CHAL == null) { _openUserRegistration.PHY_CHAL = string.Empty; }
            if (_openUserRegistration.SEX == null) { _openUserRegistration.SEX = string.Empty; }
            if (_openUserRegistration.CASTE == null) { _openUserRegistration.CASTE = string.Empty; }
            if (_openUserRegistration.RELIGION == null) { _openUserRegistration.RELIGION = string.Empty; }
            if (_openUserRegistration.NATION == null) { _openUserRegistration.NATION = string.Empty; }
            if (_openUserRegistration.CAT == null) { _openUserRegistration.CAT = string.Empty; }
            if (_openUserRegistration.BOARD == null) { _openUserRegistration.BOARD = string.Empty; }
            if (_openUserRegistration.OROLL == null) { _openUserRegistration.OROLL = string.Empty; }
            if (_openUserRegistration.OSESSION == null) { _openUserRegistration.OSESSION = string.Empty; }
            if (_openUserRegistration.OSCHOOL == null) { _openUserRegistration.OSCHOOL = string.Empty; }
            if (_openUserRegistration.SCHOOLE == null) { _openUserRegistration.SCHOOLE = string.Empty; }
            if (_openUserRegistration.SCHL2 == null) { _openUserRegistration.SCHL2 = string.Empty; }
            if (_openUserRegistration.SCHL3 == null) { _openUserRegistration.SCHL3 = string.Empty; }
            if (_openUserRegistration.SCHL1 == null) { _openUserRegistration.SCHL1 = string.Empty; }
            if (_openUserRegistration.FLG_DIST == null) { _openUserRegistration.FLG_DIST = string.Empty; }
            if (_openUserRegistration.SUBJ == null) { _openUserRegistration.SUBJ = string.Empty; }
            if (_openUserRegistration.FORMNO == null) { _openUserRegistration.FORMNO = string.Empty; }
            if (_openUserRegistration.TEMPREGNO == null) { _openUserRegistration.TEMPREGNO = string.Empty; }
            if (_openUserRegistration.REGNO1 == null) { _openUserRegistration.REGNO1 = string.Empty; }
            if (_openUserRegistration.REGNOOLD == null) { _openUserRegistration.REGNOOLD = string.Empty; }
            if (_openUserRegistration.CandStudyMedium == null) { _openUserRegistration.CandStudyMedium = string.Empty; }
            if (_openUserRegistration.correctionid == null) { _openUserRegistration.correctionid = string.Empty; }


            if (IsUserInReg(_openUserRegistration.APPNO) == 1)
            {
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UpdateOpenRegistrationByAdmin";

                cmd.Parameters.Clear();
                cmd.Connection = con;
                
                cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                cmd.Parameters.AddWithValue("@adminid", adminid);
                cmd.Parameters.AddWithValue("@APPNO", _openUserRegistration.APPNO.ToUpper());
                cmd.Parameters.AddWithValue("@YEAR", @PSEBONLINE.Repository.SessionSettingMastersRepository.GetSessionSettingMasters().SessionShortYear);
                cmd.Parameters.AddWithValue("@SET", _openUserRegistration.SET.ToUpper());
                cmd.Parameters.AddWithValue("@CLASS", _openUserRegistration.CLASS.ToUpper());
                cmd.Parameters.AddWithValue("@FORM", _openUserRegistration.FORM.ToUpper());
                cmd.Parameters.AddWithValue("@DIST", _openUserRegistration.DIST.ToUpper());
                cmd.Parameters.AddWithValue("@RP", _openUserRegistration.RP.ToUpper());
                cmd.Parameters.AddWithValue("@EXAM", _openUserRegistration.EXAM.ToUpper());
                cmd.Parameters.AddWithValue("@SCHL", _openUserRegistration.SCHL.ToUpper());
                cmd.Parameters.AddWithValue("@REGNO", _openUserRegistration.REGNO.ToUpper());
                cmd.Parameters.AddWithValue("@NAME", _openUserRegistration.NAME.ToUpper());
                cmd.Parameters.AddWithValue("@PNAME", _openUserRegistration.PNAME);
                cmd.Parameters.AddWithValue("@FNAME", _openUserRegistration.FNAME.ToUpper());
                cmd.Parameters.AddWithValue("@PFNAME", _openUserRegistration.PFNAME);
                cmd.Parameters.AddWithValue("@MNAME", _openUserRegistration.MNAME.ToUpper());
                cmd.Parameters.AddWithValue("@PMNAME", _openUserRegistration.PMNAME);
                cmd.Parameters.AddWithValue("@DOB", _openUserRegistration.DOB.ToUpper());
                cmd.Parameters.AddWithValue("@PHY_CHAL", _openUserRegistration.PHY_CHAL.ToUpper());
                cmd.Parameters.AddWithValue("@SEX", _openUserRegistration.SEX.ToUpper());
                cmd.Parameters.AddWithValue("@CASTE", _openUserRegistration.CASTE.ToUpper());
                cmd.Parameters.AddWithValue("@RELIGION", _openUserRegistration.RELIGION.ToUpper());
                cmd.Parameters.AddWithValue("@NATION", _openUserRegistration.NATION.ToUpper());
                cmd.Parameters.AddWithValue("@CAT", _openUserRegistration.CAT.ToUpper());
                cmd.Parameters.AddWithValue("@BOARD", _openUserRegistration.BOARD.ToUpper());
                cmd.Parameters.AddWithValue("@OROLL", _openUserRegistration.OROLL.ToUpper());
                cmd.Parameters.AddWithValue("@OSESSION", _openUserRegistration.OSESSION.ToUpper());
                cmd.Parameters.AddWithValue("@OSCHOOL", _openUserRegistration.OSCHOOL.ToUpper());
                cmd.Parameters.AddWithValue("@SCHOOLE", _openUserRegistration.SCHOOLE.ToUpper());
                cmd.Parameters.AddWithValue("@INSERTDT", _openUserRegistration.INSERTDT);
                cmd.Parameters.AddWithValue("@UPDT", _openUserRegistration.UPDT);
                cmd.Parameters.AddWithValue("@SCHL2", _openUserRegistration.SCHL2.ToUpper());
                cmd.Parameters.AddWithValue("@SCHL3", _openUserRegistration.SCHL3.ToUpper());
                cmd.Parameters.AddWithValue("@AADHAR_NO", _openUserRegistration.AADHAR_NO.ToUpper());
                cmd.Parameters.AddWithValue("@SCHL1", _openUserRegistration.SCHL1.ToUpper());
                cmd.Parameters.AddWithValue("@SCHLUPD_DT", _openUserRegistration.SCHLUPD_DT);
                cmd.Parameters.AddWithValue("@FLG_DIST", _openUserRegistration.FLG_DIST.ToUpper());
                cmd.Parameters.AddWithValue("@PRINTLOT", _openUserRegistration.PRINTLOT);
                cmd.Parameters.AddWithValue("@PRINTSTATU", _openUserRegistration.PRINTSTATU);
                cmd.Parameters.AddWithValue("@FEE_EXMPT", _openUserRegistration.FEE_EXMPT);
                cmd.Parameters.AddWithValue("@SUBJ", _openUserRegistration.SUBJ.ToUpper());
                cmd.Parameters.AddWithValue("@FORMNO", _openUserRegistration.FORMNO.ToUpper());
                cmd.Parameters.AddWithValue("@TEMPREGNO", _openUserRegistration.TEMPREGNO.ToUpper());
                cmd.Parameters.AddWithValue("@REGNO1", _openUserRegistration.REGNO1.ToUpper());
                cmd.Parameters.AddWithValue("@REGNOOLD", _openUserRegistration.REGNOOLD.ToUpper());
                cmd.Parameters.AddWithValue("@emr17flag", _openUserRegistration.emr17flag);
                cmd.Parameters.AddWithValue("@CandStudyMedium", _openUserRegistration.CandStudyMedium.ToUpper());
                cmd.Parameters.AddWithValue("@correctionid", _openUserRegistration.correctionid.ToUpper());
                cmd.Parameters.AddWithValue("@correction_dt", _openUserRegistration.correction_dt);
                cmd.Parameters.AddWithValue("@IMG_RAND", imgPhoto.ToUpper());
                cmd.Parameters.AddWithValue("@IMGSIGN_RA", imgSign.ToUpper());               
                cmd.Parameters.AddWithValue("@AppearingYear", _openUserRegistration.AppearingYear);
                // user login
                cmd.Parameters.AddWithValue("@STREAM", _openUserLogin.STREAM);
                cmd.Parameters.AddWithValue("@STREAMCODE", _openUserLogin.STREAMCODE);
        
                cmd.Parameters.AddWithValue("@MOBILENO", _openUserLogin.MOBILENO.ToUpper());
                cmd.Parameters.AddWithValue("@EMAILID", _openUserLogin.EMAILID.ToUpper());
                cmd.Parameters.AddWithValue("@ADDRESS", _openUserLogin.ADDRESS.ToUpper());
                cmd.Parameters.AddWithValue("@TEHSIL", _openUserLogin.TEHSIL.ToUpper());
                cmd.Parameters.AddWithValue("@PINCODE", _openUserLogin.PINCODE.ToUpper());
                cmd.Parameters.AddWithValue("@HOMEDIST", _openUserLogin.HOMEDIST.ToUpper());
                cmd.Parameters.AddWithValue("@HOMEDISTNM", _openUserLogin.HOMEDISTNM.ToUpper());
                string myIP = AbstractLayer.StaticDB.GetFullIPAddress();
                cmd.Parameters.AddWithValue("@MyIP", myIP);

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    return 1;
                }
                catch (Exception e)
                {
                    return 0;
                }
            }
            else
            { return -1; }
        }
                
        public string CancelApplicationOpen(string cancelremarks, string appno, out string outstatus, int AdminId,string EmpUserId)
        {
            int result;            
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CancelApplicationOpen", con);//ChallanDetailsCancelSP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                    cmd.Parameters.AddWithValue("@appno", appno);
                    cmd.Parameters.AddWithValue("@CancelRemarks", cancelremarks);
                    cmd.Parameters.AddWithValue("@AdminId", AdminId);       
                    cmd.Parameters.Add("@OutStatus", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    outstatus = Convert.ToString(cmd.Parameters["@OutStatus"].Value);
                    return outstatus;
                }
            }
            catch (Exception ex)
            {
               return  outstatus = "-1";
            }
        }

        #region Open Subject Correction DB
        public OpenUserLogin GetRecordCorr(string appno, string schl)
        {
            if (appno != null)
            {
                cmd.CommandText = "sp_GetLoginOpenCorr";
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                string search = "where a.ID = " + appno + " and b.schl = " + schl;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@search", search);
                DataSet ds = new DataSet();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(ds);
                try
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        OpenUserLogin _openUserLogin = new OpenUserLogin();
                        DataRow dr = ds.Tables[0].Rows[0];
                        _openUserLogin.AADHAR_NO = dr["AADHAR_NO"].ToString();
                        _openUserLogin.ADDRESS = dr["ADDRESS"].ToString();
                        _openUserLogin.ADMINUSER = (float)Convert.ToDecimal(dr["ADMINUSER"].ToString());
                        _openUserLogin.APPNO = Convert.ToInt64(dr["APPNO"].ToString());
                        _openUserLogin.BLOCK = dr["BLOCK"].ToString();
                        _openUserLogin.CATEGORY = dr["CATEGORY"].ToString();
                        _openUserLogin.CHALLANDT = Convert.ToDateTime(dr["CHALLANDT"].ToString());
                        _openUserLogin.SubjectList = dr["SubjectLis"].ToString();
                        if (dr["CHALLANFLA"].ToString() == "true")
                        {
                            _openUserLogin.CHALLANFLA = 1;
                        }
                        else
                        {
                            _openUserLogin.CHALLANFLA = 0;
                        }
                        _openUserLogin.CLASS = dr["CLASS"].ToString();
                        _openUserLogin.correctionid = dr["correctionid"].ToString();
                        _openUserLogin.correction_dt = Convert.ToDateTime(dr["correction_dt"].ToString());
                        _openUserLogin.DIST = dr["DIST"].ToString();
                        _openUserLogin.DISTNME = dr["DISTNME"].ToString();
                        _openUserLogin.DOB = dr["DOB"].ToString();
                        _openUserLogin.DOC_A_RAND = dr["DOC_A_RAND"].ToString();
                        _openUserLogin.DOC_B_RAND = dr["DOC_B_RAND"].ToString();
                        _openUserLogin.DOC_C_RAND = dr["DOC_C_RAND"].ToString();
                        _openUserLogin.DOWNLOADDA = Convert.ToDateTime(dr["DOWNLOADDA"].ToString());
                        if (dr["DOWNLOADFL"].ToString() == "true")
                        {
                            _openUserLogin.DOWNLOADFL = 1;
                        }
                        else
                        {
                            _openUserLogin.DOWNLOADFL = 0;
                        }
                        _openUserLogin.EMAILID = dr["EMAILID"].ToString();
                        _openUserLogin.FLG_DIST = dr["FLG_DIST"].ToString();
                        _openUserLogin.FORM = dr["FORM"].ToString();
                        _openUserLogin.HOMEDIST = dr["HOMEDIST"].ToString();
                        _openUserLogin.HOMEDISTNM = dr["HOMEDISTNM"].ToString();
                        _openUserLogin.ID = Convert.ToInt32(dr["ID"].ToString());
                        _openUserLogin.IMGSIGN_RA = dr["IMGSIGN_RA"].ToString();
                        _openUserLogin.IMG_RAND = dr["IMG_RAND"].ToString();
                        _openUserLogin.INSERTDT = Convert.ToDateTime(dr["INSERTDT"].ToString());
                        if (dr["ISCOMPLETE"].ToString() == "true")
                        {
                            _openUserLogin.ISCOMPLETE = 1;
                        }
                        else
                        {
                            _openUserLogin.ISCOMPLETE = 0;
                        }
                        if (dr["ISSCHLCHOO"].ToString() == "true")
                        {
                            _openUserLogin.ISSCHLCHOO = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSCHLCHOO = 0;
                        }
                        if (dr["ISSTEP1"].ToString() == "true")
                        {
                            _openUserLogin.ISSTEP1 = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSTEP1 = 0;
                        }

                        if (dr["ISSTEP2"].ToString() == "true")
                        {
                            _openUserLogin.ISSTEP2 = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSTEP2 = 0;
                        }
                        _openUserLogin.ISSTEP1DT = Convert.ToDateTime(dr["ISSTEP1DT"].ToString());

                        if (dr["ISSTEP2B"].ToString() == "true")
                        {
                            _openUserLogin.ISSTEP2B = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSTEP2B = 0;
                        }

                        if (dr["ISSUBJECT"].ToString() == "true")
                        {
                            _openUserLogin.ISSUBJECT = 1;
                        }
                        else
                        {
                            _openUserLogin.ISSUBJECT = 0;
                        }
                        _openUserLogin.ISSTEP2DT = Convert.ToDateTime(dr["ISSTEP2DT"].ToString());
                        _openUserLogin.LANDMARK = dr["LANDMARK"].ToString();
                        _openUserLogin.MOBILENO = dr["MOBILENO"].ToString();
                        _openUserLogin.MODIFYBY = dr["MODIFYBY"].ToString();
                        _openUserLogin.MODIFYDT = dr["MODIFYDT"].ToString();
                        _openUserLogin.NAME = dr["NAME"].ToString();
                        _openUserLogin.PINCODE = dr["PINCODE"].ToString();
                        _openUserLogin.PNAME = dr["PNAME"].ToString();
                        _openUserLogin.PWD = dr["PWD"].ToString();
                        _openUserLogin.RDATE = dr["RDATE"].ToString();
                        _openUserLogin.RECEIVEFLA = (float)Convert.ToDecimal(dr["RECEIVEFLA"].ToString());
                        _openUserLogin.REGDATE = dr["REGDATE"].ToString();
                        _openUserLogin.REMARK = dr["REMARK"].ToString();
                        _openUserLogin.SCHL = dr["SCHL"].ToString();
                        _openUserLogin.SCHOOLE = dr["SCHOOLE"].ToString();
                        _openUserLogin.STREAM = dr["STREAM"].ToString();
                        _openUserLogin.STREAMCODE = dr["STREAMCODE"].ToString();
                        _openUserLogin.TEHSIL = dr["TEHSIL"].ToString();
                        _openUserLogin.TOKENNO = dr["TOKENNO"].ToString();
                        _openUserLogin.UPDT = Convert.ToDateTime(dr["UPDT"].ToString());

                        return _openUserLogin;
                    }
                    else
                    {
                        return new OpenUserLogin();
                    }
                }
                catch (Exception e)
                {

                }
            }
            return new OpenUserLogin();
        }
        public void InsertOpenCorrSubjects(string[] subjects_array, string app_class, string app_id, string app_stream, string schl, FormCollection fc, string app_Board, string app_Category, string app_Month, string app_Year)
        {
            OpenUserSubjects _openUserSubjects = new OpenUserSubjects();
            _openUserSubjects.CLASS = app_class;
            _openUserSubjects.APPNO = app_id;
            _openUserSubjects.SCHL = schl;
            _openUserSubjects.BOARD = app_Board.Split(',')[0];
            _openUserSubjects.CATEGORY = app_Category.Split(',')[0];
            _openUserSubjects.MONTH = app_Month;
            _openUserSubjects.YEAR = app_Year;
            _openUserSubjects.INSERTDT = DateTime.Now;

            if (app_stream == string.Empty)
            {
                _openUserSubjects.STREAM = _openUserSubjects.STREAMCODE = "10";
            }
            else
            {
                List<SelectListItem> streams = GetStreams_1();
                _openUserSubjects.STREAM = app_stream;
                _openUserSubjects.STREAMCODE = streams.Find(f => f.Text == _openUserSubjects.STREAM).Value;
            }


            List<SelectListItem> subjects = new List<SelectListItem>();
            if (_openUserSubjects.CLASS == "10")
            {
                subjects = GetMatricSubjects_2();
            }
            else
            {
                subjects = GetAllSeniorSubjects();
            }


            int i = 0;
            if (IsUserInSubjects(_openUserSubjects.APPNO) != 0)
            {
                //RemoveUserSubjects(_openUserSubjects.APPNO);
                RemoveUserSubjectsCorr(_openUserSubjects.APPNO);
            }
            foreach (string str in subjects_array)
            {
                if (str != string.Empty && str != null)
                {
                    string[] cc = new string[9];
                    _openUserSubjects.SUB_SEQ = i + 1;
                    _openUserSubjects.SUB = str;
                    //_openUserSubjects.SUBABBR = str;
                    if (_openUserSubjects.SUB == "01")
                    {
                        _openUserSubjects.SUBNM = "Punjabi";
                    }
                    else if (_openUserSubjects.SUB == "07")
                    {
                        _openUserSubjects.SUBNM = "Punjab History & Culture";
                    }
                    else
                    {
                        _openUserSubjects.SUBNM = subjects.Find(f => f.Value == _openUserSubjects.SUB).Text;
                    }

                    //if (i < 5)
                    //{
                    //    _openUserSubjects.SUBCAT = "R";
                    //}
                    //else
                    //{
                    //    if ((_openUserSubjects.CLASS == "10" && i == 5) || (_openUserSubjects.CLASS == "12" && i == 5 && _openUserSubjects.STREAMCODE.Trim().ToUpper() == "C"))
                    //    { _openUserSubjects.SUBCAT = "R"; }
                    //    else
                    //    { _openUserSubjects.SUBCAT = "A"; }
                    //}
                    if ((_openUserSubjects.CLASS == "10" && _openUserSubjects.SUB_SEQ > 6) || (_openUserSubjects.CLASS == "12" && _openUserSubjects.SUB_SEQ > 5))
                    {
                        _openUserSubjects.SUBCAT = "A";
                    }
                    else if (_openUserSubjects.CLASS == "10" && fc["Sub_" + _openUserSubjects.SUB_SEQ + "_Th_min"] != "0" && _openUserSubjects.SUB_SEQ < 7)
                    {
                        _openUserSubjects.SUBCAT = "C";

                    }
                    else if (_openUserSubjects.CLASS == "12" && fc["Sub_" + _openUserSubjects.SUB_SEQ + "_Th_min"] != "0" && _openUserSubjects.SUB_SEQ < 6)
                    {
                        _openUserSubjects.SUBCAT = "C";

                    }
                    else
                    {
                        _openUserSubjects.SUBCAT = "R";
                    }

                    switch (i)
                    {
                        case 0:
                            cc[0] = (fc["Sub_1_Th_Obt"] != "0") ? fc["Sub_1_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_1_Th_Min"] != "0") ? fc["Sub_1_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_1_Th_Max"] != "0") ? fc["Sub_1_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_1_Pr_Obt"] != "0") ? fc["Sub_1_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_1_Pr_Min"] != "0") ? fc["Sub_1_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_1_Pr_Max"] != "0") ? fc["Sub_1_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_1_CCE_Obt"] != "0") ? fc["Sub_1_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_1_CCE_Min"] != "0") ? fc["Sub_1_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_1_CCE_Max"] != "0") ? fc["Sub_1_CCE_Max"] : string.Empty; break;
                        case 1:
                            cc[0] = (fc["Sub_2_Th_Obt"] != "0") ? fc["Sub_2_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_2_Th_Min"] != "0") ? fc["Sub_2_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_2_Th_Max"] != "0") ? fc["Sub_2_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_2_Pr_Obt"] != "0") ? fc["Sub_2_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_2_Pr_Min"] != "0") ? fc["Sub_2_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_2_Pr_Max"] != "0") ? fc["Sub_2_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_2_CCE_Obt"] != "0") ? fc["Sub_2_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_2_CCE_Min"] != "0") ? fc["Sub_2_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_2_CCE_Max"] != "0") ? fc["Sub_2_CCE_Max"] : string.Empty; break;
                        case 2:
                            cc[0] = (fc["Sub_3_Th_Obt"] != "0") ? fc["Sub_3_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_3_Th_Min"] != "0") ? fc["Sub_3_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_3_Th_Max"] != "0") ? fc["Sub_3_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_3_Pr_Obt"] != "0") ? fc["Sub_3_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_3_Pr_Min"] != "0") ? fc["Sub_3_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_3_Pr_Max"] != "0") ? fc["Sub_3_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_3_CCE_Obt"] != "0") ? fc["Sub_3_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_3_CCE_Min"] != "0") ? fc["Sub_3_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_3_CCE_Max"] != "0") ? fc["Sub_3_CCE_Max"] : string.Empty; break;
                        case 3:
                            cc[0] = (fc["Sub_4_Th_Obt"] != "0") ? fc["Sub_4_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_4_Th_Min"] != "0") ? fc["Sub_4_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_4_Th_Max"] != "0") ? fc["Sub_4_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_4_Pr_Obt"] != "0") ? fc["Sub_4_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_4_Pr_Min"] != "0") ? fc["Sub_4_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_4_Pr_Max"] != "0") ? fc["Sub_4_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_4_CCE_Obt"] != "0") ? fc["Sub_4_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_4_CCE_Min"] != "0") ? fc["Sub_4_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_4_CCE_Max"] != "0") ? fc["Sub_4_CCE_Max"] : string.Empty; break;
                        case 4:
                            cc[0] = (fc["Sub_5_Th_Obt"] != "0") ? fc["Sub_5_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_5_Th_Min"] != "0") ? fc["Sub_5_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_5_Th_Max"] != "0") ? fc["Sub_5_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_5_Pr_Obt"] != "0") ? fc["Sub_5_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_5_Pr_Min"] != "0") ? fc["Sub_5_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_5_Pr_Max"] != "0") ? fc["Sub_5_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_5_CCE_Obt"] != "0") ? fc["Sub_5_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_5_CCE_Min"] != "0") ? fc["Sub_5_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_5_CCE_Max"] != "0") ? fc["Sub_5_CCE_Max"] : string.Empty; break;
                        case 5:
                            if (_openUserSubjects.CLASS == "10" || (_openUserSubjects.CLASS == "12" && _openUserSubjects.STREAMCODE.Trim().ToUpper() == "C"))
                            {
                                cc[0] = (fc["Sub_6_Th_Obt"] != "0") ? fc["Sub_6_Th_Obt"] : string.Empty;
                                cc[1] = (fc["Sub_6_Th_Min"] != "0") ? fc["Sub_6_Th_Min"] : string.Empty;
                                cc[2] = (fc["Sub_6_Th_Max"] != "0") ? fc["Sub_6_Th_Max"] : string.Empty;
                                cc[3] = (fc["Sub_6_Pr_Obt"] != "0") ? fc["Sub_6_Pr_Obt"] : string.Empty;
                                cc[4] = (fc["Sub_6_Pr_Min"] != "0") ? fc["Sub_6_Pr_Min"] : string.Empty;
                                cc[5] = (fc["Sub_6_Pr_Max"] != "0") ? fc["Sub_6_Pr_Max"] : string.Empty;
                                cc[6] = (fc["Sub_6_CCE_Obt"] != "0") ? fc["Sub_6_CCE_Obt"] : string.Empty;
                                cc[7] = (fc["Sub_6_CCE_Min"] != "0") ? fc["Sub_6_CCE_Min"] : string.Empty;
                                cc[8] = (fc["Sub_6_CCE_Max"] != "0") ? fc["Sub_6_CCE_Max"] : string.Empty;
                            }
                            break;
                        default: for (int t = 0; t < 9; t++) { cc[t] = string.Empty; } break;
                    }

                    _openUserSubjects.OBTMARKS = cc[0];
                    _openUserSubjects.MINMARKS = cc[1];
                    _openUserSubjects.MAXMARKS = cc[2];
                    _openUserSubjects.OBTMARKSP = cc[3];
                    _openUserSubjects.MINMARKSP = cc[4];
                    _openUserSubjects.MAXMARKSP = cc[5];
                    _openUserSubjects.OBTMARKSCC = cc[6];
                    _openUserSubjects.MINMARKSCC = cc[7];
                    _openUserSubjects.MAXMARKSCC = cc[8];

                    InsertOpenCorrSubjects(_openUserSubjects);
                    i++;
                }
            }
            //OpenUserLogin ol = new OpenUserLogin();
            //ol = GetLoginById(app_id);
            //if (ol.ISSUBJECT == 0)
            //{
            //    ol.ISSUBJECT = 1;
            //    ol.UPDT = DateTime.Now;
            //    UpdateLoginUser(ol);
            //}

        }
        public int InsertOpenCorrSubjects(OpenUserSubjects _openUserSubjects)
        {
            if (_openUserSubjects.APPNO == null || _openUserSubjects.APPNO == string.Empty)
            {
                return 0;
            }
            if (_openUserSubjects.CLASS == null || _openUserSubjects.CLASS == string.Empty)
            {
                return 0;
            }

            if (_openUserSubjects.correctionid == null) { _openUserSubjects.correctionid = string.Empty; }
            if (_openUserSubjects.correction_dt == new DateTime()) { _openUserSubjects.correction_dt = Convert.ToDateTime("1/1/1900 12:00:00 AM"); }
            if (_openUserSubjects.GROUP == null) { _openUserSubjects.GROUP = string.Empty; }
            if (_openUserSubjects.GROUPCODE == null) { _openUserSubjects.GROUPCODE = string.Empty; }
            if (_openUserSubjects.INSERTDT == new DateTime()) { _openUserSubjects.INSERTDT = Convert.ToDateTime("1/1/1900 12:00:00 AM"); }

            if (_openUserSubjects.MAXMARKS == null) { _openUserSubjects.MAXMARKS = string.Empty; }
            if (_openUserSubjects.MAXMARKSCC == null) { _openUserSubjects.MAXMARKSCC = string.Empty; }
            if (_openUserSubjects.MAXMARKSP == null) { _openUserSubjects.MAXMARKSP = string.Empty; }
            if (_openUserSubjects.MEDIUM == null) { _openUserSubjects.MEDIUM = string.Empty; }
            if (_openUserSubjects.MINMARKS == null) { _openUserSubjects.MINMARKS = string.Empty; }

            if (_openUserSubjects.MINMARKSCC == null) { _openUserSubjects.MINMARKSCC = string.Empty; }
            if (_openUserSubjects.MINMARKSP == null) { _openUserSubjects.MINMARKSP = string.Empty; }
            if (_openUserSubjects.OBTMARKS == null) { _openUserSubjects.OBTMARKS = string.Empty; }
            if (_openUserSubjects.OBTMARKSCC == null) { _openUserSubjects.OBTMARKSCC = string.Empty; }

            if (_openUserSubjects.OBTMARKSP == null) { _openUserSubjects.OBTMARKSP = string.Empty; }
            if (_openUserSubjects.SCHL == null) { _openUserSubjects.SCHL = string.Empty; }
            if (_openUserSubjects.STREAM == null) { _openUserSubjects.STREAM = string.Empty; }
            if (_openUserSubjects.STREAMCODE == null) { _openUserSubjects.STREAMCODE = string.Empty; }
            if (_openUserSubjects.SUB == null) { _openUserSubjects.SUB = string.Empty; }
            if (_openUserSubjects.SUBABBR == null) { _openUserSubjects.SUBABBR = string.Empty; }
            if (_openUserSubjects.SUBCAT == null) { _openUserSubjects.SUBCAT = string.Empty; }
            if (_openUserSubjects.SUBNM == null) { _openUserSubjects.SUBNM = string.Empty; }
            if (_openUserSubjects.TRADE == null) { _openUserSubjects.TRADE = string.Empty; }
            if (_openUserSubjects.TRADECODE == null) { _openUserSubjects.TRADECODE = string.Empty; }
            if (_openUserSubjects.UPDT == new DateTime()) { _openUserSubjects.UPDT = Convert.ToDateTime("1/1/1900 12:00:00 AM"); }

            if (_openUserSubjects.BOARD == null) { _openUserSubjects.BOARD = string.Empty; }
            if (_openUserSubjects.CATEGORY == null) { _openUserSubjects.CATEGORY = string.Empty; }
            if (_openUserSubjects.MONTH == null) { _openUserSubjects.MONTH = string.Empty; }
            if (_openUserSubjects.YEAR == null) { _openUserSubjects.YEAR = string.Empty; }



            if (IsSubjectInAppNoCORR(_openUserSubjects.APPNO, _openUserSubjects.SUB, _openUserSubjects.SUBABBR) == 0)
            {
                //cmd = new SqlCommand();
                //cmd.CommandText = "sp_InsertSubjectsOpen";
                cmd = new SqlCommand();
                cmd.CommandText = "sp_InsertSubjectsOpenCorr_2112";
            }
            else
            {
                //cmd = new SqlCommand();
                //cmd.CommandText = "sp_UpdateSubjectsOpen";
                //-------By Ranjan----
                //cmd = new SqlCommand();
                //cmd.CommandText = "Update_Open_SubjectCorrection_Sp";
            }
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = con;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@APPNO", _openUserSubjects.APPNO.ToUpper());
            cmd.Parameters.AddWithValue("@CLASS", _openUserSubjects.CLASS.ToUpper());
            cmd.Parameters.AddWithValue("@OBTMARKSP", _openUserSubjects.OBTMARKSP.ToUpper());
            cmd.Parameters.AddWithValue("@SCHL", _openUserSubjects.SCHL.ToUpper());
            cmd.Parameters.AddWithValue("@SUB", _openUserSubjects.SUB.ToUpper());
            cmd.Parameters.AddWithValue("@SUBNM", _openUserSubjects.SUBNM.ToUpper());
            cmd.Parameters.AddWithValue("@SUBABBR", _openUserSubjects.SUBABBR.ToUpper());
            cmd.Parameters.AddWithValue("@MEDIUM", _openUserSubjects.MEDIUM.ToUpper());
            cmd.Parameters.AddWithValue("@SUBCAT", _openUserSubjects.SUBCAT.ToUpper());
            cmd.Parameters.AddWithValue("@OBTMARKS", _openUserSubjects.OBTMARKS.ToUpper());
            cmd.Parameters.AddWithValue("@MINMARKS", _openUserSubjects.MINMARKS.ToUpper());


            cmd.Parameters.AddWithValue("@MAXMARKS", _openUserSubjects.MAXMARKS.ToUpper());

            cmd.Parameters.AddWithValue("@MINMARKSP", _openUserSubjects.MINMARKSP.ToUpper());
            cmd.Parameters.AddWithValue("@MAXMARKSP", _openUserSubjects.MAXMARKSP.ToUpper());
            cmd.Parameters.AddWithValue("@STREAM", _openUserSubjects.STREAM.ToUpper());

            cmd.Parameters.AddWithValue("@STREAMCODE", _openUserSubjects.STREAMCODE.ToUpper());
            cmd.Parameters.AddWithValue("@GROUP", _openUserSubjects.GROUP.ToUpper());
            cmd.Parameters.AddWithValue("@GROUPCODE", _openUserSubjects.GROUPCODE.ToUpper());
            cmd.Parameters.AddWithValue("@TRADE", _openUserSubjects.TRADE.ToUpper());
            cmd.Parameters.AddWithValue("@TRADECODE", _openUserSubjects.TRADECODE.ToUpper());
            cmd.Parameters.AddWithValue("@INSERTDT", _openUserSubjects.INSERTDT);
            cmd.Parameters.AddWithValue("@UPDT", _openUserSubjects.UPDT);
            cmd.Parameters.AddWithValue("@SUB_SEQ", _openUserSubjects.SUB_SEQ);
            cmd.Parameters.AddWithValue("@OBTMARKSCC", _openUserSubjects.OBTMARKSCC.ToUpper());
            cmd.Parameters.AddWithValue("@MINMARKSCC", _openUserSubjects.MINMARKSCC.ToUpper());
            cmd.Parameters.AddWithValue("@MAXMARKSCC", _openUserSubjects.MAXMARKSCC.ToUpper());
            cmd.Parameters.AddWithValue("@correctionid", _openUserSubjects.correctionid.ToUpper());
            cmd.Parameters.AddWithValue("@correction_dt", _openUserSubjects.correction_dt);

            cmd.Parameters.AddWithValue("@BOARD", _openUserSubjects.BOARD.Split(',')[0]);
            cmd.Parameters.AddWithValue("@CATEGORY", _openUserSubjects.CATEGORY.Split(',')[0]);
            cmd.Parameters.AddWithValue("@MONTH", _openUserSubjects.MONTH);
            cmd.Parameters.AddWithValue("@YEAR", _openUserSubjects.YEAR);

            //int res = 0;
            //con.Open();
            //res = cmd.ExecuteNonQuery();
            //con.Close();

            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);

            return 0;
        }
        public int RemoveUserSubjectsCorr(string appno)
        {
            cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_RemoveUserSubjectsCORR";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@search", appno);
            con.Open();
            int res = cmd.ExecuteNonQuery();
            con.Close();
            return res;
        }
        public int IsSubjectInAppNoCORR(string appno, string sub, string subAbbr)
        {
            cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "sp_GetSubjectsForUserCORR";
            cmd.Connection = con;
            string search = "where APPNO='" + appno + "' and SUB='" + sub + "'";
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@search", search);
            DataSet ds = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter(cmd);
            adp.Fill(ds);
            if (ds.Tables[0].Rows.Count >= 1)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public DataSet IsUserInSubjectsCORR(string appno)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter(cmd);
            try
            {
                cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "IsUserInSubjectsCORR_sp";
                cmd.Connection = con;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@appno", appno);
                ad.SelectCommand = cmd;
                ad.Fill(ds);
                return ds;
            }
            catch (Exception e)
            {
                return null;
            }
        }



        public void InsertOpenCorrSubjectsNew(string[] subjects_array, string[] subcat_array, string app_class, string app_id, string app_stream, string schl, FormCollection fc, string app_Board, string app_Category, string app_Month, string app_Year)
        {

            List<tblsubjectopen> _tblsubjectopenList = new List<tblsubjectopen>();  
            OpenUserSubjects _openUserSubjects = new OpenUserSubjects();
            _openUserSubjects.CLASS = app_class;
            _openUserSubjects.APPNO = app_id;
            _openUserSubjects.SCHL = schl;
            _openUserSubjects.BOARD = app_Board.Split(',')[0];
            _openUserSubjects.CATEGORY = app_Category.Split(',')[0];
            _openUserSubjects.MONTH = app_Month;
            _openUserSubjects.YEAR = app_Year;
            _openUserSubjects.INSERTDT = DateTime.Now;

           

            if (app_class == "10")
            {
                _openUserSubjects.STREAM = "GENERAL";
                _openUserSubjects.STREAMCODE = "G";
            }
            else
            {
                List<SelectListItem> streams = GetStreams_1();
                _openUserSubjects.STREAM = app_stream;
                _openUserSubjects.STREAMCODE = streams.Find(f => f.Text == _openUserSubjects.STREAM).Value;
            }


            List<SelectListItem> subjects = new List<SelectListItem>();
            if (_openUserSubjects.CLASS == "10")
            {
                subjects = GetMatricSubjects_2();
            }
            else
            {
                subjects = GetAllSeniorSubjects();
            }


            int i = 0;
            //if (IsUserInSubjects(_openUserSubjects.APPNO) != 0)
            //{
            //    RemoveUserSubjects(_openUserSubjects.APPNO);
            //}


            OpenUserLogin ol = new OpenUserLogin();
            ol = GetLoginById(app_id);

            foreach (string str in subjects_array)
            {
                if (str != string.Empty && str != null)
                {
                    string[] cc = new string[9];
                    _openUserSubjects.SUB_SEQ = i + 1;
                    _openUserSubjects.SUB = str;
                    if (_openUserSubjects.SUB == "01")
                    {
                        _openUserSubjects.SUBNM = "Punjabi";
                    }
                    else if (_openUserSubjects.SUB == "07")
                    {
                        _openUserSubjects.SUBNM = "Punjab History & Culture";
                    }
                    else
                    {
                        _openUserSubjects.SUBNM = subjects.Find(f => f.Value == _openUserSubjects.SUB).Text;
                    }

                    if (i < 6)
                    {
                        //  _openUserSubjects.SUBCAT = "R";
                        _openUserSubjects.SUBCAT = subcat_array[i];
                    }
                    else
                    { _openUserSubjects.SUBCAT = "A"; }
                    //else
                    //{
                    //    if ((_openUserSubjects.CLASS == "10" && i == 5) || (_openUserSubjects.CLASS == "12" && i == 5 && _openUserSubjects.STREAMCODE.Trim().ToUpper() == "C"))
                    //    { _openUserSubjects.SUBCAT = "R"; }
                    //    else
                    //    { _openUserSubjects.SUBCAT = "A"; }
                    //}



                    switch (i)
                    {
                        case 0:
                            cc[0] = (fc["Sub_1_Th_Obt"] != "0") ? fc["Sub_1_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_1_Th_Min"] != "0") ? fc["Sub_1_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_1_Th_Max"] != "0") ? fc["Sub_1_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_1_Pr_Obt"] != "0") ? fc["Sub_1_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_1_Pr_Min"] != "0") ? fc["Sub_1_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_1_Pr_Max"] != "0") ? fc["Sub_1_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_1_CCE_Obt"] != "0") ? fc["Sub_1_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_1_CCE_Min"] != "0") ? fc["Sub_1_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_1_CCE_Max"] != "0") ? fc["Sub_1_CCE_Max"] : string.Empty; break;
                        case 1:
                            cc[0] = (fc["Sub_2_Th_Obt"] != "0") ? fc["Sub_2_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_2_Th_Min"] != "0") ? fc["Sub_2_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_2_Th_Max"] != "0") ? fc["Sub_2_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_2_Pr_Obt"] != "0") ? fc["Sub_2_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_2_Pr_Min"] != "0") ? fc["Sub_2_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_2_Pr_Max"] != "0") ? fc["Sub_2_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_2_CCE_Obt"] != "0") ? fc["Sub_2_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_2_CCE_Min"] != "0") ? fc["Sub_2_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_2_CCE_Max"] != "0") ? fc["Sub_2_CCE_Max"] : string.Empty; break;
                        case 2:
                            cc[0] = (fc["Sub_3_Th_Obt"] != "0") ? fc["Sub_3_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_3_Th_Min"] != "0") ? fc["Sub_3_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_3_Th_Max"] != "0") ? fc["Sub_3_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_3_Pr_Obt"] != "0") ? fc["Sub_3_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_3_Pr_Min"] != "0") ? fc["Sub_3_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_3_Pr_Max"] != "0") ? fc["Sub_3_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_3_CCE_Obt"] != "0") ? fc["Sub_3_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_3_CCE_Min"] != "0") ? fc["Sub_3_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_3_CCE_Max"] != "0") ? fc["Sub_3_CCE_Max"] : string.Empty; break;
                        case 3:
                            cc[0] = (fc["Sub_4_Th_Obt"] != "0") ? fc["Sub_4_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_4_Th_Min"] != "0") ? fc["Sub_4_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_4_Th_Max"] != "0") ? fc["Sub_4_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_4_Pr_Obt"] != "0") ? fc["Sub_4_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_4_Pr_Min"] != "0") ? fc["Sub_4_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_4_Pr_Max"] != "0") ? fc["Sub_4_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_4_CCE_Obt"] != "0") ? fc["Sub_4_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_4_CCE_Min"] != "0") ? fc["Sub_4_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_4_CCE_Max"] != "0") ? fc["Sub_4_CCE_Max"] : string.Empty; break;
                        case 4:
                            cc[0] = (fc["Sub_5_Th_Obt"] != "0") ? fc["Sub_5_Th_Obt"] : string.Empty;
                            cc[1] = (fc["Sub_5_Th_Min"] != "0") ? fc["Sub_5_Th_Min"] : string.Empty;
                            cc[2] = (fc["Sub_5_Th_Max"] != "0") ? fc["Sub_5_Th_Max"] : string.Empty;
                            cc[3] = (fc["Sub_5_Pr_Obt"] != "0") ? fc["Sub_5_Pr_Obt"] : string.Empty;
                            cc[4] = (fc["Sub_5_Pr_Min"] != "0") ? fc["Sub_5_Pr_Min"] : string.Empty;
                            cc[5] = (fc["Sub_5_Pr_Max"] != "0") ? fc["Sub_5_Pr_Max"] : string.Empty;
                            cc[6] = (fc["Sub_5_CCE_Obt"] != "0") ? fc["Sub_5_CCE_Obt"] : string.Empty;
                            cc[7] = (fc["Sub_5_CCE_Min"] != "0") ? fc["Sub_5_CCE_Min"] : string.Empty;
                            cc[8] = (fc["Sub_5_CCE_Max"] != "0") ? fc["Sub_5_CCE_Max"] : string.Empty; break;
                        case 5:
                            if (_openUserSubjects.CLASS == "10" || (_openUserSubjects.CLASS == "12" && _openUserSubjects.STREAMCODE.Trim().ToUpper() == "C"))
                            {
                                cc[0] = (fc["Sub_6_Th_Obt"] != "0") ? fc["Sub_6_Th_Obt"] : string.Empty;
                                cc[1] = (fc["Sub_6_Th_Min"] != "0") ? fc["Sub_6_Th_Min"] : string.Empty;
                                cc[2] = (fc["Sub_6_Th_Max"] != "0") ? fc["Sub_6_Th_Max"] : string.Empty;
                                cc[3] = (fc["Sub_6_Pr_Obt"] != "0") ? fc["Sub_6_Pr_Obt"] : string.Empty;
                                cc[4] = (fc["Sub_6_Pr_Min"] != "0") ? fc["Sub_6_Pr_Min"] : string.Empty;
                                cc[5] = (fc["Sub_6_Pr_Max"] != "0") ? fc["Sub_6_Pr_Max"] : string.Empty;
                                cc[6] = (fc["Sub_6_CCE_Obt"] != "0") ? fc["Sub_6_CCE_Obt"] : string.Empty;
                                cc[7] = (fc["Sub_6_CCE_Min"] != "0") ? fc["Sub_6_CCE_Min"] : string.Empty;
                                cc[8] = (fc["Sub_6_CCE_Max"] != "0") ? fc["Sub_6_CCE_Max"] : string.Empty;
                            }
                            break;
                        default: for (int t = 0; t < 9; t++) { cc[t] = string.Empty; } break;
                    }

                    _openUserSubjects.OBTMARKS = cc[0];
                    _openUserSubjects.MINMARKS = cc[1];
                    _openUserSubjects.MAXMARKS = cc[2];
                    _openUserSubjects.OBTMARKSP = cc[3];
                    _openUserSubjects.MINMARKSP = cc[4];
                    _openUserSubjects.MAXMARKSP = cc[5];
                    _openUserSubjects.OBTMARKSCC = cc[6];
                    _openUserSubjects.MINMARKSCC = cc[7];
                    _openUserSubjects.MAXMARKSCC = cc[8];

                    if (_openUserSubjects.SUBCAT == "C")
                    {
                        if (_openUserSubjects.OBTMARKSCC == "000" || _openUserSubjects.OBTMARKSCC == "" || ol.CATEGORY.ToLower().Contains("direct"))
                        {
                            _openUserSubjects.OBTMARKS = "";
                            _openUserSubjects.MINMARKS = "";
                            _openUserSubjects.MAXMARKS = "";
                            _openUserSubjects.OBTMARKSP = "";
                            _openUserSubjects.MINMARKSP = "";
                            _openUserSubjects.MAXMARKSP = "";
                            _openUserSubjects.OBTMARKSCC = "";
                            _openUserSubjects.MINMARKSCC = "";
                            _openUserSubjects.MAXMARKSCC = "";
                            _openUserSubjects.SUBCAT = "R";
                        }
                    }
                    else
                    {
                        _openUserSubjects.OBTMARKS = "";
                        _openUserSubjects.MINMARKS = "";
                        _openUserSubjects.MAXMARKS = "";
                        _openUserSubjects.OBTMARKSP = "";
                        _openUserSubjects.MINMARKSP = "";
                        _openUserSubjects.MAXMARKSP = "";
                        _openUserSubjects.OBTMARKSCC = "";
                        _openUserSubjects.MINMARKSCC = "";
                        _openUserSubjects.MAXMARKSCC = "";
                    }

                    int ivalue = i;
                    //C - (6)   R
                    //H - (5) R
                    if (i > 5)
                    {
                        if (_openUserSubjects.CLASS == "12" && i == 5 && _openUserSubjects.STREAMCODE.Trim().ToUpper() == "C")
                        { _openUserSubjects.SUBCAT = "R"; }
                        else
                        { _openUserSubjects.SUBCAT = "A"; }

                        //if (i > 6)
                        //{
                        //    _openUserSubjects.SUBCAT = "A";
                        //}
                        if (i > 5) // after commece 5 subject (reg)
                        {
                            _openUserSubjects.SUBCAT = "A";
                        }
                    }
                    else
                    {
                        if (_openUserSubjects.CLASS == "12" && i == 5 && _openUserSubjects.STREAMCODE.Trim().ToUpper() != "C")
                        {
                            _openUserSubjects.SUBCAT = "A";
                        }
                        else if (_openUserSubjects.CLASS == "12" && i == 5 && _openUserSubjects.STREAMCODE.Trim().ToUpper() == "C")
                        {
                            _openUserSubjects.SUBCAT = "A";
                        }
                    }


                    //_tblsubjectopenList.Add(new tblsubjectopen
                    //{
                    //    APPNO = _openUserSubjects.APPNO,
                    //    SUB = _openUserSubjects.SUB,
                    //    MEDIUM = "",
                    //    SUBCAT = _openUserSubjects.SUBCAT == null ? "" : _openUserSubjects.SUBCAT,
                    //    SUB_SEQ = _openUserSubjects.SUB_SEQ,
                    //    OBTMARKS = _openUserSubjects.OBTMARKS,
                    //    OBTMARKSP = _openUserSubjects.OBTMARKSP,
                    //    OBTMARKSCC = _openUserSubjects.OBTMARKSCC,
                    //    INSERTDT = _openUserSubjects.INSERTDT,
                    //    UPDT = Convert.ToDateTime("1/1/1900 12:00:00 AM"),
                    //    correctionid = "",
                    //    correction_dt = Convert.ToDateTime("1/1/1900 12:00:00 AM"),
                    //});

                    //// // InsertUserSubjects(_openUserSubjects);
                    InsertOpenCorrSubjects(_openUserSubjects);
                    i++;
                }
            }                       
        }

        #endregion Open Subject Correction DB





    }
}