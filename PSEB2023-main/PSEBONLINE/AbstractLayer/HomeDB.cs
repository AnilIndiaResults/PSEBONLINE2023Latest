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
using System.Data.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace PSEBONLINE.AbstractLayer
{
    public class HomeDB
    {

        #region Check ConString

        private string CommonCon = "myDBConnection";
        public HomeDB()
        {
            CommonCon = "myDBConnection";
        }
        #endregion  Check ConString


        #region Challan Deposit Details
        public DataSet ChallanDepositDetails(ChallanDepositDetailsModel cdm, out string OutError)
        {           
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ChallanDepositDetailsSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CHALLANID", cdm.CHALLANID);
                    cmd.Parameters.AddWithValue("@BRCODECAND", cdm.BRCODECAND);
                    cmd.Parameters.AddWithValue("@BRANCHCAND", cdm.BRANCHCAND);
                    cmd.Parameters.AddWithValue("@J_REF_NOCAND", cdm.J_REF_NOCAND);
                    cmd.Parameters.AddWithValue("@DEPOSITDTCAND", cdm.DEPOSITDTCAND);
                    cmd.Parameters.AddWithValue("@ChallanRemarks", cdm.challanremarks);
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
                OutError = "0";
                return result = null;
            }
        }
        #endregion Challan Deposit Details

        public string CheckOpenSchool(string schl)
        {
            string res = "false";
            try
            {
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_checkOpenSchool";
                cmd.Parameters.AddWithValue("@schl", schl);
                DataSet ds = new DataSet();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(ds);
                int count = 0;
                count = Convert.ToInt32(ds.Tables[0].Rows[0]["Count"].ToString());
                if (count > 0)
                {
                    res = "true";
                }
            }
            catch (Exception ex)
            { res = "false"; }
            return res;
        }

        public List<SelectListItem> OpenSchoolDistricts()
        {
            List<SelectListItem> districts = new List<SelectListItem>();
            try
            {

                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "sp_openSchoolDistricts";
                DataSet ds = new DataSet();
                SqlDataAdapter adp = new SqlDataAdapter(cmd);
                adp.Fill(ds);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    districts.Add(new SelectListItem() { Text = dr["dist"].ToString(), Value = dr["dist"].ToString() });
                }
            }
            catch (Exception e) { }

            return districts;
        }


        public void insertUdisecode(string SCHL, string udisecode, out string outstatus)
        {

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("insertUdisecode", con);//InsertSMFSP   //InsertSMFSPNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@udisecode", udisecode);
                    cmd.Parameters.Add("@outstatus", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    outstatus = Convert.ToString(cmd.Parameters["@outstatus"].Value);

                }
            }
            catch (Exception ex)
            {
                outstatus = "-1";
            }
            finally
            {
                // con.Close();
            }
        }

        public DataSet findUdisecodeWithDetails(string SCHL)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("findUdisecodeWithDetails", con);
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
        public DataSet findOPENSCHOOL(string SCHL)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("findOPENSCHOOL_sp", con);
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
        public void findUdisecode(string SCHL, out string outstatus)
        {

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("findUdisecodeD", con);//InsertSMFSP   //InsertSMFSPNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.Add("@outstatus", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    outstatus = Convert.ToString(cmd.Parameters["@outstatus"].Value);

                }
            }
            catch (Exception ex)
            {
                outstatus = "-1";
            }
            finally
            {
                // con.Close();
            }
        }
        public void insertUdisecodeD1(string SCHL, Printlist obj, out string outstatus)
        {

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("insertUdisecodeD1", con);//InsertSMFSP   //InsertSMFSPNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@udisecode", obj.udisecode);
                    cmd.Parameters.AddWithValue("@numofhum", obj.numofhum);
                    cmd.Parameters.AddWithValue("@numofsci", obj.numofsci);
                    cmd.Parameters.AddWithValue("@numofcomm", obj.numofcomm);
                    cmd.Parameters.AddWithValue("@numofvoc", obj.numofvoc);
                    cmd.Parameters.AddWithValue("@numoftech", obj.numoftech);
                    cmd.Parameters.AddWithValue("@numofagri", obj.numofagri);
                    cmd.Parameters.AddWithValue("@numofregular", obj.numofregular);
                    cmd.Parameters.AddWithValue("@numofnsqf", obj.numofnsqf);
                    cmd.Parameters.Add("@outstatus", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    outstatus = Convert.ToString(cmd.Parameters["@outstatus"].Value);

                }
            }
            catch (Exception ex)
            {
                outstatus = "-1";
            }
            finally
            {
                // con.Close();
            }
        }
        //public void findUdisecode(string SCHL, out string outstatus)
        //{

        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
        //        {
        //            // SM.SCHL = SM.id.ToString();
        //            SqlCommand cmd = new SqlCommand("findUdisecode", con);//InsertSMFSP   //InsertSMFSPNew
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@SCHL", SCHL);
        //            cmd.Parameters.Add("@outstatus", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;
        //            con.Open();
        //            cmd.ExecuteNonQuery();
        //            outstatus = Convert.ToString(cmd.Parameters["@outstatus"].Value);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        outstatus = "-1";
        //    }
        //    finally
        //    {
        //        // con.Close();
        //    }
        //}
        public DataSet GetMissingCheckFeeStatus(string Search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetMissingCheckFeeStatusSP", con);
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

       

        public DataSet CheckFeeStatusJunior(string SCHL, string type, string id, DateTime date)
        {
            try
            {
                Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "CheckFeeStatusSPByViewJunior";
                cmd.Parameters.AddWithValue("@SCHL", SCHL);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@form", id);
                return db.ExecuteDataSet(cmd);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public DataSet GetCalculateFeeBySchoolJunior(string cls, string search, string schl, DateTime? date = null)
        {
            Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetCalculateFeeBySchoolSP_Junior_Exemption";//GetCalculateFeeBySchoolSP_Junior
            cmd.Parameters.AddWithValue("@cls", cls);
            cmd.Parameters.AddWithValue("@search", search);
            cmd.Parameters.AddWithValue("@Schl", schl);
            cmd.Parameters.AddWithValue("@date", date);
            return db.ExecuteDataSet(cmd);
        }

        public DataSet CheckFeeStatus(string SCHL, string type, string id, DateTime date)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            SqlCommand cmd;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    if (type == "Admin")
                    { cmd = new SqlCommand("CheckFeeStatusSPAdmin_Rohit", con); } //From Admin Panel                  
                    else
                    {
                        cmd = new SqlCommand("CheckFeeStatusSPByView_0507amar", con);
                    } // Form wise calculate fee
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@form", id);
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

        public DataSet GetCalculateFeeBySchool(string cls, string search, string schl, DateTime? date = null)
        {
            Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetCalculateFeeBySchoolSP_Main";//GetCalculateFeeBySchoolSP_Junior
            cmd.Parameters.AddWithValue("@cls", cls);
            cmd.Parameters.AddWithValue("@search", search);
            cmd.Parameters.AddWithValue("@Schl", schl);
            cmd.Parameters.AddWithValue("@date", date);
            return db.ExecuteDataSet(cmd);
        }


        //public DataSet GetCalculateFeeBySchool(string search, string schl, DateTime? date = null)
        //{
        //    DataSet result = new DataSet();
        //    SqlDataAdapter ad = new SqlDataAdapter();
        //    SqlCommand cmd;
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
        //        {
        //            cmd = new SqlCommand("GetCalculateFeeBySchoolSP_Main", con);//GetCalculateFeeBySchoolSP_0507amar

        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@search", search);
        //            cmd.Parameters.AddWithValue("@Schl", schl);
        //            if (date != null)
        //                cmd.Parameters.AddWithValue("@date", date);
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
        //change by rohit for admin calculation
        public DataSet GetCalculateFeeBySchoolAdmin(string id, string search, string schl, DateTime? date = null)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            SqlCommand cmd;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {                   
                    cmd = new SqlCommand("GetCalculateFeeBySchoolSP_MainAdmin", con);// change for 2023-24 by RN
                   // SqlCommand cmd = new SqlCommand("GetCalculateFeeBySchoolSPNewD", con);//GetCalculateFeeBySchoolSPNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@Schl", schl);
                    if (date != null)
                        cmd.Parameters.AddWithValue("@date", date);
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


        public DataSet GetSchoolLotDetails(string schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetSchoolLotDetailsSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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


        public string InsertPaymentForm(ChallanMasterModel CM, FormCollection frm, out string SchoolMobile)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("InsertPaymentFormMainSP", con);   //InsertPaymentFormMainSP_RohitTesting  //InsertPaymentFormMainSP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.Parameters.AddWithValue("@EmpUserId", CM.EmpUserId);
                cmd.Parameters.AddWithValue("@SchoolCode", CM.SchoolCode);
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
                //cmd.Parameters.AddWithValue("@ChallanGDateN", CM.ChallanGDateN);
                cmd.Parameters.AddWithValue("@ChallanVDateN", CM.ChallanVDateN);
                cmd.Parameters.Add("@CHALLANID", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@SchoolMobile", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string OutError = (string)cmd.Parameters["@OutError"].Value;
                string outuniqueid = (string)cmd.Parameters["@CHALLANID"].Value;
                SchoolMobile = (string)cmd.Parameters["@SchoolMobile"].Value;
                
                return outuniqueid;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                SchoolMobile = ex.Message;
                return result = "0";

            }
            finally
            {
                con.Close();
            }
        }


        public DataSet ReGenerateChallaanById(string ChallanId, string usertype, out int OutStatus)  // ReGenerateChallaan
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ReGenerateChallaanByIdSPNew", con); //ReGenerateChallaanByIdSP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CHALLANID", ChallanId);
                    cmd.Parameters.AddWithValue("@type", usertype);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
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


        public DataSet ReGenerateChallaanByIdBank(string ChallanId,string BCODE, string usertype, out int OutStatus, out string CHALLANIDOut)  // ReGenerateChallaan
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ReGenerateChallaanByIdBankSP", con); //ReGenerateChallaanByIdBankSPTest
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CHALLANID", ChallanId);
                    cmd.Parameters.AddWithValue("@BCODE",BCODE);
                    cmd.Parameters.AddWithValue("@type", usertype);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    SqlParameter outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = "@CHALLANIDOut";
                    outPutParameter.Size = 100;
                    outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                    outPutParameter.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(outPutParameter); 
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    CHALLANIDOut = (string)cmd.Parameters["@CHALLANIDOut"].Value;
                    return dataTable;

                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                CHALLANIDOut = "-1";
                return null;
            }
        }

        public DataSet GetChallanDetailsByStudentList(string studentlist)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetChallanDetailsByStudentList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@studentlist", studentlist);
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


        public DataSet GetChallanDetailsBySearch(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetChallanDetailsBySearch", con);
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

        //ChallanDepositDetailsCancel
        public void ChallanDepositDetailsCancel(string cancelremarks, string challanid, out string outstatus, int AdminId)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ChallanDepositDetailsCancelSP", con);//ChallanDepositDetailsCancelSP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CHALLANID", challanid);
                    cmd.Parameters.AddWithValue("@AdminId", AdminId);
                    cmd.Parameters.AddWithValue("@cancelremarks", cancelremarks);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    outstatus = Convert.ToString(cmd.Parameters["@OutStatus"].Value);

                }
            }
            catch (Exception ex)
            {
                outstatus = "-1";
            }
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


        public DataSet GetFinalPrintChallan(string SchoolCode)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetFinalPrintChallanSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SchoolCode", SchoolCode);
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

        public DataSet CheckChallanIsVerified(string SCHL, int LOT)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CheckChallanIsVerified", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@LOT", LOT);
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


        // Exam Portal

        public DataSet CheckChallanIsVerifiedByFeeCode(string SCHL, int LOT, int FeeCode)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CheckChallanIsVerifiedByFeeCode", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@LOT", LOT);
                    cmd.Parameters.AddWithValue("@Feecode", FeeCode);
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

        public DataSet ExamReportCalculateFeeSPByDate(string search, string schl, string class1, DateTime? date = null, DateTime? insertdate = null)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ExamReportCalculateFeeSPByDateAdmin", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@Schl", schl);
                    cmd.Parameters.AddWithValue("@class", class1);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@insertdate", insertdate);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception)
            {
                return result = null;
            }
        }

        public DataSet ExamCalculateFeeAdmin(string search, string schl, string class1, DateTime? date = null)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ExamCalculateFeeAdmin", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@Schl", schl);
                    cmd.Parameters.AddWithValue("@class", class1);
                    cmd.Parameters.AddWithValue("@date", date);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception)
            {
                return result = null;
            }
        }
        public DataSet ExamReportCountRecordsClassWise(string search, string schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ExamReportCountRecordsClassWise_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@Schl", schl);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception)
            {
                return result = null;
            }
        }

        public DataSet ExamReportMatricCalculateFee(string search, string schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ExamReportMatricCalculateFeeSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@Schl", schl);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception)
            {
                return result = null;
            }
        }

        public DataSet ExamReportSeniorCalculateFee(string search, string schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ExamReportSeniorCalculateFeeSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@Schl", schl);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception)
            {
                return result = null;
            }
        }


        public DataSet ExamReportMatricCalculateFeeOPEN(string search, string schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ExamReportMatricCalculateFeeSPOPEN", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@Schl", schl);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception)
            {
                return result = null;
            }
        }

        public DataSet ExamReportSeniorCalculateFeeOPEN(string search, string schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ExamReportSeniorCalculateFeeSPOPEN", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@Schl", schl);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception)
            {
                return result = null;
            }
        }

       
        public DataSet GetSchoolPrintLotDetails(string schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetSchoolPrintLotDetailsSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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
       
        //-------------------------------------------Examination Challan Verification--------------
        public DataSet GetFinalPrintChallanExam(string SchoolCode)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetFinalPrintChallanExam_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SchoolCode", SchoolCode);
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
        //------------------------------------------End-Examination Challan Verification--------------


        public string ExamInsertPaymentForm(ChallanMasterModel CM, FormCollection frm, out string SchoolMobile, DataTable StudentWiseFeeDT)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());    
                SqlCommand cmd = new SqlCommand("ExamInsertPaymentFormMainSP", con);   //ExamInsertPaymentFormSP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.Parameters.AddWithValue("@EmpUserId", CM.EmpUserId);
                cmd.Parameters.AddWithValue("@SchoolCode", CM.SchoolCode);
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
                cmd.Parameters.AddWithValue("@pracfee", CM.pracfee);
                cmd.Parameters.AddWithValue("@addsubfee", CM.addsubfee);
                cmd.Parameters.AddWithValue("@add_sub_count", CM.add_sub_count);
                cmd.Parameters.AddWithValue("@prac_sub_count ", CM.prac_sub_count);
                cmd.Parameters.AddWithValue("@regfee", CM.regfee);
                cmd.Parameters.AddWithValue("@type", CM.type);
                cmd.Parameters.AddWithValue("@PrintLOT", CM.LOT); // print lot
                cmd.Parameters.AddWithValue("@FeeStudentList", CM.FeeStudentList);               
                cmd.Parameters.AddWithValue("@ChallanVDateN", CM.ChallanVDateN);
                cmd.Parameters.AddWithValue("@StudentWiseFeeDT", StudentWiseFeeDT);
                cmd.Parameters.AddWithValue("@Class", CM.Class);
                cmd.Parameters.AddWithValue("@FormType", CM.FormType);
                if (CM.LSFRemarks != null && CM.LSFRemarks != "")
                {
                    cmd.Parameters.AddWithValue("@LumsumFine", CM.LumsumFine);
                    cmd.Parameters.AddWithValue("@LSFRemarks", CM.LSFRemarks);
                }
                cmd.Parameters.Add("@CHALLANID", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@SchoolMobile", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string OutError = (string)cmd.Parameters["@OutError"].Value;
                string outuniqueid = (string)cmd.Parameters["@CHALLANID"].Value;
                SchoolMobile = (string)cmd.Parameters["@SchoolMobile"].Value;
                return outuniqueid;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                SchoolMobile = "";
                return result = "0";

            }
            finally
            {
                con.Close();
            }
        }


        public string ExamInsertPaymentFormSPByDate(ChallanMasterModel CM, FormCollection frm, out string SchoolMobile, DataTable StudentWiseFeeDT)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("ExamInsertPaymentFormSP", con);   //ExamInsertPaymentFormSPByDate,  ExamInsertPaymentFormSPTest
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.Parameters.AddWithValue("@SchoolCode", CM.SchoolCode);
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
                cmd.Parameters.AddWithValue("@pracfee", CM.pracfee);
                cmd.Parameters.AddWithValue("@addsubfee", CM.addsubfee);
                cmd.Parameters.AddWithValue("@add_sub_count", CM.add_sub_count);
                cmd.Parameters.AddWithValue("@prac_sub_count ", CM.prac_sub_count);
                cmd.Parameters.AddWithValue("@regfee", CM.regfee);
                cmd.Parameters.AddWithValue("@type", CM.type);
                cmd.Parameters.AddWithValue("@PrintLOT", CM.LOT); // print lot
                cmd.Parameters.AddWithValue("@FeeStudentList", CM.FeeStudentList);
                cmd.Parameters.AddWithValue("@ChallanVDateN", CM.ChallanVDateN);
                cmd.Parameters.AddWithValue("@StudentWiseFeeDT", StudentWiseFeeDT);
                cmd.Parameters.AddWithValue("@Class", CM.Class);
                cmd.Parameters.AddWithValue("@FormType", CM.FormType);
                if (CM.LSFRemarks != null && CM.LSFRemarks != "")
                {
                    cmd.Parameters.AddWithValue("@LumsumFine", CM.LumsumFine);
                    cmd.Parameters.AddWithValue("@LSFRemarks", CM.LSFRemarks);
                }
                cmd.Parameters.Add("@CHALLANID", SqlDbType.VarChar, 30).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@SchoolMobile", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
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
    }
}