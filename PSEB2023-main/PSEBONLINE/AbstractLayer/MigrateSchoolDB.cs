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
    public class MigrateSchoolDB
    {
        #region Check ConString

        private string CommonCon = "myDBConnection";
        public MigrateSchoolDB()
        {
            CommonCon = "myDBConnection";
        }
        #endregion  Check ConString

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
        public DataSet SelectForMigrateSchools(string Search)
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
                    SqlCommand cmd = new SqlCommand("SelectForMigrateSchools_sp", con);
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

        //public List<SelectListItem> GetAllFormName()
        //{
        //    List<SelectListItem> formNameList = new List<SelectListItem>();                
        //    //formNameList.Add(new SelectListItem { Text = "---Select Form--", Value = "0" });
        //    formNameList.Add(new SelectListItem { Text = "N1", Value = "N1" });
        //    //formNameList.Add(new SelectListItem { Text = "N2", Value = "N2" });
        //    //formNameList.Add(new SelectListItem { Text = "N3", Value = "N3" });
        //    formNameList.Add(new SelectListItem { Text = "E1", Value = "E1" });
        //    //formNameList.Add(new SelectListItem { Text = "E2", Value = "E2" });
        //    //formNameList.Add(new SelectListItem { Text = "M1", Value = "M1" });
        //    //formNameList.Add(new SelectListItem { Text = "M2", Value = "M2" });
        //    //formNameList.Add(new SelectListItem { Text = "T1", Value = "T1" });
        //    //formNameList.Add(new SelectListItem { Text = "T2", Value = "T2" });
        //    return formNameList;
        //}

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
                    SqlCommand cmd = new SqlCommand("GetAllFormName_sp", con);
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
                    SqlCommand cmd = new SqlCommand("GetAllLot_sp", con);
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

      

        public DataSet GetStudentRegNoNotAlloted(string search,string schl, int pageNumber)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudentRegNoNotAllotedSPPaging", con);//[GetStudentRegNoNotAllotedSP]
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
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

        public DataSet ViewAllotRegNo(string search, string schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ViewAllotRegNoSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
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


        public DataSet GetRegEntryviewMigrate(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetRegEntryviewMigrate_SP", con);
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
        public DataSet GetRegEntryviewMigrate_Search(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetRegEntryviewMigrate_Search_SP", con);
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

        public DataSet GetMigrationForm(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetMigrationForm_SP", con);
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

        public DataSet GetSchl(int SchlID)
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
                    SqlCommand cmd = new SqlCommand("GetSchoolDetailsMigrate", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SchlID", SchlID);

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

        public DataSet Insert_MigrationForm(MigrateSchoolModels MS, FormCollection frm)  // Type 1=Regular, 2=Open
        {            
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string userIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("Insert_MigrationForm_SP", con);//InsertSMFSP   //InsertSMFSPNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StdId", MS.StdId);
                    cmd.Parameters.AddWithValue("@SchlCode", MS.SchlCode);
                    cmd.Parameters.AddWithValue("@RegNo", MS.RegNo);
                    cmd.Parameters.AddWithValue("@groupNM", MS.Std_Sub);                    
                    cmd.Parameters.AddWithValue("@Candi_Name", MS.Candi_Name);
                    cmd.Parameters.AddWithValue("@Father_Name", MS.Father_Name);
                    
                    cmd.Parameters.AddWithValue("@Mother_Name", MS.Mother_Name);
                    cmd.Parameters.AddWithValue("@SchlCodeNew", MS.SchlCodeNew);
                    cmd.Parameters.AddWithValue("@DISTNM", MS.DistName);
                    //cmd.Parameters.AddWithValue("@rdoDD", MS.rdoDD);
                    //cmd.Parameters.AddWithValue("@rdoBrdRcpt", MS.rdoBrdRcpt);
                    cmd.Parameters.AddWithValue("@DDRcptNo", MS.DDRcptNo);
                    cmd.Parameters.AddWithValue("@Amount", MS.Amount);
                    cmd.Parameters.AddWithValue("@DepositDt", MS.DepositDt);

                    cmd.Parameters.AddWithValue("@BankName", MS.BankName);
                    cmd.Parameters.AddWithValue("@DiryOrderNo", MS.DiryOrderNo);
                    cmd.Parameters.AddWithValue("@OrderDt", MS.OrderDt);
                    cmd.Parameters.AddWithValue("@OrderBy", MS.OrderBy);

                    cmd.Parameters.AddWithValue("@Remark", MS.Remark);
                    cmd.Parameters.AddWithValue("@userip", userIP);
                    cmd.Parameters.AddWithValue("@userName", MS.UserName);
                    cmd.Parameters.Add("@GetCorrectionNo", SqlDbType.Int).Direction = ParameterDirection.Output;
                    //con.Open();
                    //result = cmd.ExecuteNonQuery();
                    ////result = (int)cmd.Parameters["@GetCorrectionNo"].Value;
                    //return result;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;

                }
            }
            catch (Exception ex)
            {
                //return result = -1;
                return result = null;
            }
            finally
            {
                // con.Close();
            }
        }

        public DataSet GetStudentRecordsByID(string sid)
        {            
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {                    
                    SqlCommand cmd = new SqlCommand("GetStudentRecordsByID_SP", con);
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
        public DataSet SelectMigrateSchools(string Search)
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
                    SqlCommand cmd = new SqlCommand("SelectMigrateSchools_sp", con);
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

        public DataSet SelectMigrateSchools_Print(string id)
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
                    SqlCommand cmd = new SqlCommand("SelectMigrateSchools_Print_sp", con);
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
        public string DeleteFromData(string stdid)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("DeleteFromData_Sp", con);
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

        public DataSet ChekResultCompairSubjects(string schoolcode, string chkSub)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ChekResultCompairSubjects_sp", con);
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



        /******************* Start Allot Regno  **************/

        public string RemoveRegno(string storeid, int Action, int userid, string DocumentVerifyingEmpcode, string EmpUserId)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("RemoveRegnoSP", con);  //insertinbulkexammasterregular2017
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@storeid", storeid);
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@DocumentVerifyingEmpcode", DocumentVerifyingEmpcode);
                cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
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

        public string AutomaticAllotRegno(string storeid, string EmpUserId)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("AutomaticAllotRegnoSP", con);  //insertinbulkexammasterregular2017
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@storeid", storeid);
                cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
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


        public string ErrorAllotRegno(string stdid, string storeid, int Action, int userid, string remarks, string DocumentVerifyingEmpcode, string EmpUserId)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("ErrorAllotRegnoSP", con);  //insertinbulkexammasterregular2017
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@stdid", stdid);
                cmd.Parameters.AddWithValue("@storeid", storeid);
                cmd.Parameters.AddWithValue("@action", Action);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@remarks", remarks);
                cmd.Parameters.AddWithValue("@DocumentVerifyingEmpcode", DocumentVerifyingEmpcode);
                cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
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



        public DataTable ManualAllotRegno(string stdid, string regno, out int OutStatus, int userid, string remarks, string DocumentVerifyingEmpcode, string EmpUserId)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ManualAllotRegnoSP", con);  //insertinbulkexammasterregular2017
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@stdid", stdid);
                    cmd.Parameters.AddWithValue("@regno", regno);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.AddWithValue("@userid", userid);
                    cmd.Parameters.AddWithValue("@remarks", remarks);
                    cmd.Parameters.AddWithValue("@DocumentVerifyingEmpcode", DocumentVerifyingEmpcode);
                    cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    return dataTable;

                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return null;
            }
        }


        public string ApprovedOtherBoardDocumentStudent(string stdid, string storeid, int Action, int userid, string remarks, string DocumentVerifyingEmpcode, string EmpUserId)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("ApprovedOtherBoardDocumentStudentSP", con);  //insertinbulkexammasterregular2017
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@stdid", stdid);
                cmd.Parameters.AddWithValue("@storeid", storeid);
                cmd.Parameters.AddWithValue("@action", Action);
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@remarks", remarks);
                cmd.Parameters.AddWithValue("@DocumentVerifyingEmpcode", DocumentVerifyingEmpcode);
                cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
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

    }
}