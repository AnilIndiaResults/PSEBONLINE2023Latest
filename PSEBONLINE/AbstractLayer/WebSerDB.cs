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

namespace PSEBONLINE.AbstractLayer
{
    public class WebSerDB
    {
        #region Check ConString

        private string CommonCon = "myDBConnection";
        public WebSerDB()
        {
            CommonCon = "myDBConnection";

            //if (HttpContext.Current.Session["Session"] == null)
            //{
            //    CommonCon = "myConn2019";
            //}
            //else if (HttpContext.Current.Session["Session"].ToString() == "2018-2019")
            //{
            //    CommonCon = "myConn2018";
            //}
            //else if (HttpContext.Current.Session["Session"].ToString() == "2019-2020")
            //{
            //    CommonCon = "myConn2019";
            //}
            //else if (HttpContext.Current.Session["Session"].ToString() == "2020-2021")
            //{
            //    CommonCon = "myConn2020";
            //}
            //else if (HttpContext.Current.Session["Session"].ToString() == "2021-2022")
            //{
            //    CommonCon = "myConn2021";
            //}
            //else if (HttpContext.Current.Session["Session"].ToString() == "2023-2024")
            //{
            //    CommonCon = "myConn2022";
            //}
            //else
            //{
            //    CommonCon = "myConn2019";
            //}
        }


        #endregion  Check ConString




        public string GetUdiCode(string schl)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                //con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("GetUdiCode_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@schlcode", schl);

                cmd.Parameters.Add("@uid", SqlDbType.VarChar, 500);
                cmd.Parameters["@uid"].Direction = ParameterDirection.Output;


                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string uid = cmd.Parameters["@uid"].Value.ToString();
                //string outuniqueid = (string)cmd.Parameters["@id"].Value;
                return uid;

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
        public string Ins_Temp_Data(webSerModel wm, FormCollection frc, DataTable dt)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                //con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("Insert_udiCodeTemp_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@udiCode", wm.UdiseCode);
                cmd.Parameters.AddWithValue("@udiStdDetails", dt);
                cmd.Parameters.AddWithValue("@status", 0);

                cmd.Parameters.Add("@id", SqlDbType.Int, 500);
                cmd.Parameters["@id"].Direction = ParameterDirection.Output;

                //SqlParameter outPutParameter = new SqlParameter();
                //outPutParameter.ParameterName = "@StudentUniqueId";
                //outPutParameter.Size = 20;
                //outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                //outPutParameter.Direction = System.Data.ParameterDirection.Output;
                //cmd.Parameters.Add(outPutParameter);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string id = cmd.Parameters["@id"].Value.ToString();
                //string outuniqueid = (string)cmd.Parameters["@id"].Value;
                return id;

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
        public DataSet GetudiCodeDetails(string udicode)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetudiCodeDetails_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@udicode", udicode);
                    //cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    //cmd.Parameters.AddWithValue("@PageIndex", 1);
                    //cmd.Parameters.AddWithValue("@PageSize", 30);
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
        }//Name, Pname, fname, Pfname, mname, Pmname,
        public string Import_All_N1_Data(out int OutStatus, string CurrentSchl, string epunid, string aadhar, string Name, string Pname, string fname, string Pfname, string mname, string Pmname, string dob, string sex, string caste, string reli, string DA, string mob, string admno, string Address, string pincode)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                string formName = "N1";
                //con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("insert_Webservice_Data", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
                cmd.Parameters.AddWithValue("@epunid", epunid);
                cmd.Parameters.AddWithValue("@form_Name", formName);
                cmd.Parameters.AddWithValue("@aadhar", aadhar);
                cmd.Parameters.AddWithValue("@admno", admno);
                cmd.Parameters.AddWithValue("@Name", Name);
                cmd.Parameters.AddWithValue("@fname", fname);
                cmd.Parameters.AddWithValue("@mname", mname);

                cmd.Parameters.AddWithValue("@Pname", Pname);
                cmd.Parameters.AddWithValue("@Pfname", Pfname);
                cmd.Parameters.AddWithValue("@Pmname", Pmname);

                cmd.Parameters.AddWithValue("@dob", dob);
                cmd.Parameters.AddWithValue("@sex", sex);
                cmd.Parameters.AddWithValue("@caste", caste);
                cmd.Parameters.AddWithValue("@reli", reli);
                cmd.Parameters.AddWithValue("@DA", DA);
                cmd.Parameters.AddWithValue("@mob", mob);
                cmd.Parameters.AddWithValue("@Address", Address);
                cmd.Parameters.AddWithValue("@pincode", pincode);
                cmd.Parameters.Add("@OutStatus", SqlDbType.Int, 4).Direction = ParameterDirection.Output;


                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                return result;


            }
            catch (Exception ex)
            {
                OutStatus = -1;
                //mbox(ex);
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }
        //public DataTable Import_All_N1_Data(string Impschoolcode, string CurrentSchl, string chkid)
        //{
        //    DataTable result = new DataTable();
        //    SqlDataAdapter ad = new SqlDataAdapter();
        //    try
        //    {

        //        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
        //        {
        //            SqlCommand cmd = new SqlCommand("Import_All_N1_Data_Sp", con); //GetStudentPassNinthN_sp
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@Impschoolcode", Impschoolcode);
        //            cmd.Parameters.AddWithValue("@CurrentSchl", CurrentSchl);
        //            cmd.Parameters.AddWithValue("@chkid", chkid);                    
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

        #region Deo-Portal Staff Master 
        public string Import_All_deostaff_Data(string ImportStaffFlag, string DeoUser, string userdist, string uid, string BlockID, string TD, string epunjabid, string adharno, string schl, string name, string fname, string cadre, string subject,
            string schlnm, string DISTNM, string mobile, string gender, string DOB, string Disability, string expyear, string expmonth, string bank, string ifsc,
            string acno, string Updatedate, string EDUBLOCK, string EDUCLUSTER, string UDISE, string ADDRESS, string HOME_DIST, string Staff_Status, string ExamCent, string ExamMonth)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("insert_Webservice_deostaff_Data", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@ImportStaffFlag", ImportStaffFlag);
                cmd.Parameters.AddWithValue("@DeoUser", DeoUser);
                cmd.Parameters.AddWithValue("@userdist", userdist);
                cmd.Parameters.AddWithValue("@uid", uid);

                cmd.Parameters.AddWithValue("@BlockID", BlockID);
                cmd.Parameters.AddWithValue("@TD", TD);
                cmd.Parameters.AddWithValue("@epunjabid", epunjabid);
                cmd.Parameters.AddWithValue("@adharno", adharno);
                cmd.Parameters.AddWithValue("@schl", schl);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@fname", fname);
                cmd.Parameters.AddWithValue("@cadre", cadre);
                cmd.Parameters.AddWithValue("@subject", subject);

                cmd.Parameters.AddWithValue("@schlnm", schlnm);
                cmd.Parameters.AddWithValue("@DISTNM", DISTNM);
                cmd.Parameters.AddWithValue("@mobile", mobile);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@DOB", DOB);
                cmd.Parameters.AddWithValue("@Disability", Disability);
                cmd.Parameters.AddWithValue("@expyear", expyear);
                cmd.Parameters.AddWithValue("@expmonth", expmonth);
                cmd.Parameters.AddWithValue("@bank", bank);
                cmd.Parameters.AddWithValue("@ifsc", ifsc);

                cmd.Parameters.AddWithValue("@acno", acno);
                cmd.Parameters.AddWithValue("@Updatedate", Updatedate);
                cmd.Parameters.AddWithValue("@EDUBLOCK", EDUBLOCK);
                cmd.Parameters.AddWithValue("@EDUCLUSTER", EDUCLUSTER);
                cmd.Parameters.AddWithValue("@UDISE", UDISE);
                cmd.Parameters.AddWithValue("@ADDRESS", ADDRESS);
                cmd.Parameters.AddWithValue("@HOME_DIST", HOME_DIST);
                cmd.Parameters.AddWithValue("@Staff_Status", Staff_Status);
                cmd.Parameters.AddWithValue("@ExamCent", ExamCent);
                //
                cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);

                con.Open();
                // result = cmd.ExecuteNonQuery().ToString();     
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

        #endregion Deo-Portal Staff Master 

        #region School Import Staff from Deo
        public string getStaffDetailsDeo(string examcent, string epunjabid)
        {
            SqlConnection con = null;
            string result = null;
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("getStaffDetailsDeo_SP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@epunjabid", epunjabid);
                cmd.Parameters.AddWithValue("@examcent", examcent);

                con.Open();
                // result = cmd.ExecuteNonQuery().ToString();     
                result = cmd.ExecuteScalar().ToString();

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
        public string IndexStaffImport(string TD, string epunjabid, string examcent)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("insert_Webservice_IndexStaffImport", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TD", TD);
                cmd.Parameters.AddWithValue("@epunjabid", epunjabid);
                cmd.Parameters.AddWithValue("@examcent", examcent);

                con.Open();
                // result = cmd.ExecuteNonQuery().ToString();     
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

        #endregion School Import Staff from Deo
    }
}