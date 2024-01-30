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
    public class TCDB
    {
        #region Check ConString

        private string CommonCon = "myDBConnection";
        public TCDB()
        {
            CommonCon = "myDBConnection";

        }
        #endregion  Check ConString
        public DataSet SelectDist()
        {

            // SqlConnection con = null;

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());//
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

        public DataSet GetAllSchoolsByDistTC(string Search)
        {

            // SqlConnection con = null;

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("GetAllSchoolsByDistTC_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);

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
        #region TC Generate View School Data
        public DataSet SearchSchoolDetailsTC(string search, string SelYear)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spSearchSchoolDetailsTCnw", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@SelYear", SelYear);
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
        public int GenerateTC(TCModels sm)
        {
            int result;
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spGenerateTC", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@stdID", sm.ID);
                    cmd.Parameters.AddWithValue("@SCHL", sm.SCHL);
                    cmd.Parameters.AddWithValue("@dispatchNo", sm.dispatchNo);
                    cmd.Parameters.AddWithValue("@Tot_Atd", sm.attendanceTot);
                    cmd.Parameters.AddWithValue("@attendancePresnt", sm.attendancePresnt);
                    cmd.Parameters.AddWithValue("@struckOff", sm.struckOff);
                    cmd.Parameters.AddWithValue("@reasonFrSchoolLeav", sm.reasonFrSchoolLeav);
                    cmd.Parameters.AddWithValue("@SelYear", sm.SelYear);
                    //cmd.Parameters.Add("@OutId", SqlDbType.Int).Direction = ParameterDirection.Output;
                    //ad.SelectCommand = cmd;
                    //ad.Fill(result);
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    con.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = 0;
            }
        }
        #endregion TC Generate View School Data


        # region Second Page Candidate List of School

        public DataSet GetRegEntryviewTC(string search, string SelYear)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetRegEntryviewTCnw_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@SelYear", SelYear);

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
        public DataSet GetRegEntryviewTC_Search(string search, string schl, string SelYear)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetRegEntryviewTCnw_Search_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@SelYear", SelYear);

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

        public DataSet GetAllFormName(string SCHL)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("GetAllFormNameTC_sp", con);
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

        public DataSet GetAllLot(string SCHL)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("GetAllLotTC_sp", con);
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


        #endregion  Second Page Candidate List of School

        public DataSet getSCHLstatusTC(string SCHL, string SelYear)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("getSCHLstatusTCnw_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@SelYear", SelYear);                   
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                //return result = 0;
                return result = null;
            }

        }


        public DataSet SelectTCSchools_Print(string id, string SelYear)
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
                    SqlCommand cmd = new SqlCommand("SelectTCSchoolsnw_Print_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@SelYear", SelYear);
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
        public string TCCancel(string stdid, string SelYear)
        {
            SqlConnection con = null;
            string result = "";
            //string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            //string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("TCCancelnw_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@stdid", stdid);
                cmd.Parameters.AddWithValue("@SelYear", SelYear);
                //cmd.Parameters.AddWithValue("@MyIP", myIP);
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


        #region TCDB for School  
        public DataSet GetAllFormNameSchl(string SCHL)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("GetAllFormNameTCSchl_sp", con);
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
        public DataSet GetAllLotSchl(string SCHL)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("GetAllLotTCSchl_sp", con);
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
        public DataSet getSCHLstatusTCSchl(string SCHL)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("getSCHLstatusTCSchl_SP", con);
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
                //return result = 0;
                return result = null;
            }

        }
        public DataSet GetRegEntryviewTCSchl(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetRegEntryviewTCSchl_SP", con);
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
        public DataSet GetRegEntryviewTCSchl_Search(string search, string schl, string SelYear)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetRegEntryviewTCSchlnw_Search_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@SelYear", SelYear);


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
        public DataSet SearchSchoolDetailsTCSchl(string search, string SelYear)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spSearchSchoolDetailsTCSchlnw", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@SelYear", SelYear);
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
        public int GenerateTCSchl(TCModels sm)
        {
            int result;
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spGenerateTCSchlnw", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@stdID", sm.ID);
                    cmd.Parameters.AddWithValue("@SCHL", sm.SCHL);
                    cmd.Parameters.AddWithValue("@dispatchNo", sm.dispatchNo);
                    cmd.Parameters.AddWithValue("@Tot_Atd", sm.attendanceTot);
                    cmd.Parameters.AddWithValue("@attendancePresnt", sm.attendancePresnt);
                    cmd.Parameters.AddWithValue("@struckOff", sm.struckOff);
                    cmd.Parameters.AddWithValue("@reasonFrSchoolLeav", sm.reasonFrSchoolLeav);
                    cmd.Parameters.AddWithValue("@SelYear", sm.SelYear);
                    cmd.Parameters.AddWithValue("@TCdate", sm.TCdate);

                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    con.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = 0;
            }
        }
        public DataSet SelectTCSchoolsSchl_Print(string id, string SelYear)
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
                    SqlCommand cmd = new SqlCommand("SelectTCSchoolsSchlnw_Print_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@SelYear", SelYear);
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

        public string TCCancelSchl(string stdid)
        {
            SqlConnection con = null;
            string result = "";
            //string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            //string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("TCCancelSchl_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@stdid", stdid);
                //cmd.Parameters.AddWithValue("@MyIP", myIP);
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
        #endregion TCDB for School  
    }
}