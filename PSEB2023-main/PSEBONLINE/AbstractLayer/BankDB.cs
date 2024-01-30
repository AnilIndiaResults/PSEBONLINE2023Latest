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
using System.Globalization;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace PSEBONLINE.AbstractLayer
{
    public class BankDB
    {
        #region Check ConString

        private string CommonCon = "myDBConnection";
        public BankDB()
        {
            CommonCon = "myDBConnection";
        }
        #endregion  Check ConString

        public List<SelectListItem> GetBankSessionList()
        {
            List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession.Add(new SelectListItem { Text = "2023-2024", Value = "2023-2024" });    
            return itemSession;
        }
        #region Get All Fee Details Deposit 
        public DataSet GetAllFeeDepositByBCODE(BankModels BM, string search, out string OutError)  // GetAllFeeDepositByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAllFeeDepositByBCODE", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BCODE", BM.BCODE);
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                    return dataTable;

                }
            }
            catch (Exception ex)
            {
                OutError = "-1";
                return null;
            }
        }
        #endregion Get All Fee Details Deposit 

        //// Exam Challan Cancel DB------ Begin---------///
        public string ChallanCancel(string ChallanID, string userName)
        {
            SqlConnection con = null;
            string result = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();


            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("ChallanCancel_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ChallanID", ChallanID);
                cmd.Parameters.AddWithValue("@userName", userName);
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

        //// Exam Challan Cancel DB------ End---------///

        public DataTable BankLogin(LoginModel LM, out int OutStatus)  // BankLoginSP
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BankLoginSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", LM.username);
                    cmd.Parameters.AddWithValue("@Password", LM.Password);
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

        public DataSet GetBankDataByBCODE(BankModels BM, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetBankDataByBCODESP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BCODE", BM.BCODE);
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

        public DataSet UpdateBankDataByBCODE(BankModels BM, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UpdateBankDataByBCODESP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BCODE", BM.BCODE);
                    cmd.Parameters.AddWithValue("@ADDRESS", BM.ADDRESS);
                    cmd.Parameters.AddWithValue("@DISTRICT", BM.DISTRICT);
                    cmd.Parameters.AddWithValue("@PINCODE", BM.PINCODE);
                    cmd.Parameters.AddWithValue("@MOBILE", BM.MOBILE);
                    cmd.Parameters.AddWithValue("@STD", BM.STD);
                    cmd.Parameters.AddWithValue("@PHONE", BM.PHONE);
                    cmd.Parameters.AddWithValue("@EMAILID1", BM.EMAILID1);
                    cmd.Parameters.AddWithValue("@EMAILID2", BM.EMAILID2);
                    cmd.Parameters.AddWithValue("@ACNO", BM.ACNO);
                    cmd.Parameters.AddWithValue("@IFSC", BM.IFSC);
                    cmd.Parameters.AddWithValue("@MICR", BM.MICR);
                    cmd.Parameters.AddWithValue("@buser_id", BM.buser_id);
                    cmd.Parameters.AddWithValue("@password", BM.password);
                    cmd.Parameters.AddWithValue("@BRNCODE", BM.BRNCODE);
                    cmd.Parameters.AddWithValue("@NODAL_BRANCH", BM.NODAL_BRANCH);
                    cmd.Parameters.AddWithValue("@MNAGER_NM", BM.MNAGER_NM);
                    cmd.Parameters.AddWithValue("@TECHNICAL_PERSON", BM.TECHNICAL_PERSON);
                    cmd.Parameters.AddWithValue("@OTCONTACT", BM.OTCONTACT);
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


        public DataSet ChangePasswordBank(BankModels BM, out int OutStatus)  // GetBankDataByBCODE
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ChangePasswordBank", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BCODE", BM.BCODE);
                    cmd.Parameters.AddWithValue("@oldpassword", BM.OldPassword);
                    cmd.Parameters.AddWithValue("@newpassword", BM.Newpassword);
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

        public DataSet DownloadChallanTextFormat(BankModels BM, out int OutStatus)  // BankLoginSP
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DownloadChallanTextFormatSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BCODE", BM.BCODE);
                    cmd.Parameters.AddWithValue("@Session", BM.Session);
                    cmd.Parameters.AddWithValue("@DOWNLDFLOT", BM.DOWNLDFLOT);
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
        public DataSet DownloadChallan(BankModels BM, out int OutStatus)  // BankLoginSP
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DownloadChallanSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BCODE", BM.BCODE);
                    cmd.Parameters.AddWithValue("@Session", BM.Session);
                    cmd.Parameters.AddWithValue("@DOWNLDFLG", BM.DOWNLDFLG);
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


        public DataSet BankMisDetails(BankModels BM, int Type1, out int OutStatus)  // BankLoginSP
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BankMisDetailsSp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BCODE", BM.BCODE);
                    cmd.Parameters.AddWithValue("@LOT", BM.LOT);
                    cmd.Parameters.AddWithValue("@Type", Type1);
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


        public DataTable ImportBankMisSP_PSEB(BankModels BM, int UPLOADLOT, out int OutStatus, out string Mobile,string EmpUserId,string FeeDistrict)  // BankLoginSP
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();


            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ImportBankMisSP_PSEB", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FeeDistrict", FeeDistrict);
                    cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                    cmd.Parameters.AddWithValue("@CHALLANID", BM.CHALLANID);
                    cmd.Parameters.AddWithValue("@TOTFEE", BM.TOTFEE);
                    cmd.Parameters.AddWithValue("@BRCODE", BM.BRCODE);
                    cmd.Parameters.AddWithValue("@BRANCH", BM.BRANCH);
                    cmd.Parameters.AddWithValue("@J_REF_NO", BM.J_REF_NO);
                    cmd.Parameters.AddWithValue("@DEPOSITDT", BM.DEPOSITDT);
                    cmd.Parameters.AddWithValue("@MIS_FILENM", BM.MIS_FILENM);
                    cmd.Parameters.AddWithValue("@IP_ADD", myIP);
                    cmd.Parameters.AddWithValue("@BCODE", BM.BCODE);
                    cmd.Parameters.AddWithValue("@UPLOADLOT", UPLOADLOT);
                    cmd.Parameters.AddWithValue("@DepositRemarks", BM.DepositRemarks);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    SqlParameter outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = "@Mobile";
                    outPutParameter.Size = 50;
                    outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                    outPutParameter.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(outPutParameter);

                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    Mobile = (string)cmd.Parameters["@Mobile"].Value;
                    return dataTable;

                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                Mobile = "0";
                return null;
            }
        }


        public DataTable ImportBankMis(BankModels BM, int UPLOADLOT, out int OutStatus, out string Mobile)  // BankLoginSP
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();


            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ImportBankMisSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CHALLANID", BM.CHALLANID);
                    cmd.Parameters.AddWithValue("@TOTFEE", BM.TOTFEE);
                    cmd.Parameters.AddWithValue("@BRCODE", BM.BRCODE);
                    cmd.Parameters.AddWithValue("@BRANCH", BM.BRANCH);
                    cmd.Parameters.AddWithValue("@J_REF_NO", BM.J_REF_NO);
                    cmd.Parameters.AddWithValue("@DEPOSITDT", BM.DEPOSITDT);
                    cmd.Parameters.AddWithValue("@MIS_FILENM", BM.MIS_FILENM);
                    cmd.Parameters.AddWithValue("@IP_ADD", myIP);
                    cmd.Parameters.AddWithValue("@BCODE", BM.BCODE);
                    cmd.Parameters.AddWithValue("@UPLOADLOT", UPLOADLOT);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    SqlParameter outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = "@Mobile";
                    outPutParameter.Size = 50;
                    outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                    outPutParameter.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(outPutParameter);

                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    Mobile = (string)cmd.Parameters["@Mobile"].Value;
                    return dataTable;

                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                Mobile = "0";
                return null;
            }
        }
        public DataSet GetTotBankMIS()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GettotMis_Sp", con);
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


        public DataSet GetChallanDetailsByIdSPNew(string ChallanId)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetChallanDetailsByIdSPNew", con);
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

        public DataSet GetChallanDetailsByIdSPBank(string ChallanId)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetChallanDetailsByIdSPBank", con);
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


        public DataSet BankMisDetailsSearch(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BankMisDetailsSpSearch", con);
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

        public void insertErrorFromExcel(string ErrorList)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try

            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("insertErrorFromExcel_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ErrorList", ErrorList);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();

                }
            }
            catch (Exception ex)
            {

            }
        }

        public string CheckMisExcelExport(DataSet ds, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("STATUS", typeof(string)));

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].ToString().Length < 13 || dt.Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string challanid = dt.Rows[i][0].ToString();
                        Result = "Please check ChallanId " + challanid + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check ChallanId " + challanid + " in row " + RowNo + ",  ";
                    }
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int totalfeeamount = 0;
                    if (dt.Rows[i][1].ToString() != "")
                    {
                        totalfeeamount = Convert.ToInt32(dt.Rows[i][1].ToString());
                    }
                    if (totalfeeamount <= 0 || dt.Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        //** gcm 
                        //dt.Rows[i]["ErrDetails"]+="Total Fee Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of Total Fees in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of Total Fees in row " + RowNo + ",  ";
                    }
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    int totalfeeamount = 0;
                    if (dt.Rows[i][1].ToString() != "")
                    {
                        totalfeeamount = Convert.ToInt32(dt.Rows[i][1].ToString());

                    }
                    DataSet dsChallanDetails = GetChallanDetailsByIdSPBank(dt.Rows[i]["ChallanID"].ToString());
                    if (dsChallanDetails == null || dsChallanDetails.Tables[0].Rows.Count == 0)
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "Invalid Challan Id";
                        string challanid = dt.Rows[i][0].ToString();
                        //Result += "Please check ChallanId " + challanid + " of  Total Fees and challan id in row " + RowNo + ",   ";
                        Result += "Please check ChallanId " + challanid + " for Challan Details/NotFound    in row " + RowNo + ",   ";
                        dt.Rows[i]["Status"] = "Please check ChallanId " + challanid + " for Challan Details/NotFound    in row " + RowNo + ",   ";
                    }
                    else
                    {
                        if (Convert.ToInt32(dsChallanDetails.Tables[0].Rows[0]["TOTFEE"].ToString()) != totalfeeamount)
                        {
                            int RowNo = i + 2;
                            // dt.Rows[i]["ErrDetails"] += "Total Fee Error";
                            string challanid = dt.Rows[i][0].ToString();
                            Result += "Please check ChallanId " + challanid + " of Total Fees not matched with challan  in row " + RowNo + ",   ";
                            dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of Total Fees not matched with challan  in row " + RowNo + ",   ";
                        }
                    }
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][2].ToString() == "")
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "BRCODE Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of BRCODE in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = dt.Rows[i]["Status"].ToString() + " , " + "Please check ChallanId " + challanid + " of BRCODE in row " + RowNo + ",  ";
                    }
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][3].ToString() == "")
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "BRANCH Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of BRANCH in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of BRANCH in row " + RowNo + ",  ";
                    }
                    else if (dt.Rows[i][3].ToString().Length > 75)
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "BRANCH Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of BRANCH in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of BRANCH Length Can't be greater than 75 Characters in row " + RowNo + ",  ";
                    }
                }

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][4].ToString() == "")
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "J_REF_NO Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of J_REF_NO in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of J_REF_NO in row " + RowNo + ",  ";
                    }
                }

                // CHALLANID TOTFEE  BRCODE BRANCH  J_REF_NO DEPOSITDT

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DataSet dsChallanDetails = GetChallanDetailsByIdSPBank(dt.Rows[i]["ChallanID"].ToString());
                    //24/08/2016 05:55:38PM
                    if (dt.Rows[i][5].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " for Fill Date in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " for Fill Date in row " + RowNo + ",  ";
                    }
                    else if (dt.Rows[i][5].ToString() != "")
                    {
                        if (dsChallanDetails.Tables[0].Rows.Count > 0)
                        {
                            DateTime LastDepositDate = Convert.ToDateTime(dsChallanDetails.Tables[0].Rows[0]["BankLastdateNew"].ToString());
                            DateTime GenDate = Convert.ToDateTime(dsChallanDetails.Tables[0].Rows[0]["ChallanGDateN"].ToString());
                            // 24/08/2016 05:55:38PM
                            DateTime fromDateValue;
                            string s = dt.Rows[i][5].ToString();
                            //  string s = dt.Rows[i][5].ToString().Substring(0,10);
                            // var formats = new[] { "dd/MM/yyyy hh:mm:sstt", "yyyy-MM-dd" };                      
                            if (DateTime.TryParseExact(s, "dd/MM/yyyy hh:mm:sstt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateValue))
                            { // do for valid date 

                                if (fromDateValue.Date < GenDate.Date)
                                {
                                    int RowNo = i + 2;
                                    string challanid = ds.Tables[0].Rows[i][0].ToString();
                                    Result += "Please Check ChallanId " + challanid + " for Wrong Deposit Date  in row " + RowNo + ",  ";
                                    dt.Rows[i]["Status"] += " , " + "Please Check ChallanId " + challanid + " for Wrong Deposit Date in row " + RowNo + ",  ";
                                }
                                if (fromDateValue.Date > LastDepositDate.Date)
                                {
                                    int RowNo = i + 2;
                                    string challanid = ds.Tables[0].Rows[i][0].ToString();
                                    Result += "Please Check ChallanId " + challanid + " for Wrong Deposit Date  in row " + RowNo + ",  ";
                                    dt.Rows[i]["Status"] += " , " + "Please Check ChallanId " + challanid + " for Wrong Deposit Date in row " + RowNo + ",  ";
                                }

                            }
                            else
                            {
                                int RowNo = i + 2;
                                string challanid = dt.Rows[i][0].ToString();
                                Result += "Please Check ChallanId " + challanid + " for Date Format in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] += " , " + "Please Check ChallanId " + challanid + " for Date Format in row " + RowNo + ",  ";
                                // do for invalid date
                            }

                            if (dsChallanDetails.Tables[0].Rows[0]["VERIFIED"].ToString() == "1")
                            {
                                int RowNo = i + 2;
                                // dt.Rows[i]["ErrDetails"] += "Total Fee Error";
                                string challanid = dt.Rows[i][0].ToString();
                                Result += "Please check ChallanId " + challanid + " is Already Verified  in row " + RowNo + ",   ";
                                dt.Rows[i]["Status"] = "Already Verified";
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                dt = null;
                Result = ex.Message;
                //return "Please Check All Fields";
            }
            //insertErrorFromExcel(Result);

            return Result;
        }

        public DataSet ChallanAffiliationReport(string search, string sesssion, int pageIndex)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ChallanAffiliationReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
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


        public DataSet GetChallanDetails(string search, string sesssion, int pageIndex)
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetChallanDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
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

        public DataSet GetErrorDetails()
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetErrorDetails", con);
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


        public string CheckMisExcelExportRN(DataSet ds, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("STATUS", typeof(string)));

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].ToString().Length < 13 || dt.Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string challanid = dt.Rows[i][0].ToString();
                        Result = "Please check ChallanId " + challanid + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check ChallanId " + challanid + " in row " + RowNo + ",  ";
                    }


                    int totalfeeamount = 0;
                    if (dt.Rows[i][1].ToString() != "")
                    {
                        totalfeeamount = Convert.ToInt32(dt.Rows[i][1].ToString());
                    }
                    if (totalfeeamount <= 0 || dt.Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        //** gcm 
                        //dt.Rows[i]["ErrDetails"]+="Total Fee Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of Total Fees in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of Total Fees in row " + RowNo + ",  ";
                    }

                    DataSet dsChallanDetails = GetChallanDetailsByIdSPBank(dt.Rows[i]["ChallanID"].ToString());
                    if (dsChallanDetails == null || dsChallanDetails.Tables[0].Rows.Count == 0)
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "Invalid Challan Id";
                        string challanid = dt.Rows[i][0].ToString();
                        //Result += "Please check ChallanId " + challanid + " of  Total Fees and challan id in row " + RowNo + ",   ";
                        Result += "Please check ChallanId " + challanid + " for Challan Details/NotFound    in row " + RowNo + ",   ";
                        dt.Rows[i]["Status"] = "Please check ChallanId " + challanid + " for Challan Details/NotFound    in row " + RowNo + ",   ";
                    }
                    else
                    {
                        if (Convert.ToInt32(dsChallanDetails.Tables[0].Rows[0]["TOTFEE"].ToString()) != totalfeeamount)
                        {
                            int RowNo = i + 2;
                            // dt.Rows[i]["ErrDetails"] += "Total Fee Error";
                            string challanid = dt.Rows[i][0].ToString();
                            Result += "Please check ChallanId " + challanid + " of Total Fees not matched with challan  in row " + RowNo + ",   ";
                            dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of Total Fees not matched with challan  in row " + RowNo + ",   ";
                        }
                    }


                    if (dt.Rows[i][2].ToString() == "")
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "BRCODE Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of BRCODE in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = dt.Rows[i]["Status"].ToString() + " , " + "Please check ChallanId " + challanid + " of BRCODE in row " + RowNo + ",  ";
                    }



                    if (dt.Rows[i][3].ToString() == "")
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "BRANCH Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of BRANCH in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of BRANCH in row " + RowNo + ",  ";
                    }
                    else if (dt.Rows[i][3].ToString().Length > 75)
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "BRANCH Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of BRANCH in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of BRANCH Length Can't be greater than 75 Characters in row " + RowNo + ",  ";
                    }



                    if (dt.Rows[i][4].ToString() == "")
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "J_REF_NO Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of J_REF_NO in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of J_REF_NO in row " + RowNo + ",  ";
                    }

                    // DataSet dsChallanDetails = GetChallanDetailsByIdSPBank(dt.Rows[i]["ChallanID"].ToString());
                    //24/08/2016 05:55:38PM
                    if (dt.Rows[i][5].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " for Fill Date in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " for Fill Date in row " + RowNo + ",  ";
                    }
                    else if (dt.Rows[i][5].ToString() != "")
                    {
                        if (dsChallanDetails.Tables[0].Rows.Count > 0)
                        {
                            DateTime LastDepositDate = Convert.ToDateTime(dsChallanDetails.Tables[0].Rows[0]["BankLastdateNew"].ToString());
                            DateTime GenDate = Convert.ToDateTime(dsChallanDetails.Tables[0].Rows[0]["ChallanGDateN"].ToString());
                            // 24/08/2016 05:55:38PM
                            DateTime fromDateValue;
                            string s = dt.Rows[i][5].ToString();
                            //  string s = dt.Rows[i][5].ToString().Substring(0,10);
                            // var formats = new[] { "dd/MM/yyyy hh:mm:sstt", "yyyy-MM-dd" };                      
                            if (DateTime.TryParseExact(s, "dd/MM/yyyy hh:mm:sstt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateValue))
                            { // do for valid date 

                                if (fromDateValue.Date < GenDate.Date)
                                {
                                    int RowNo = i + 2;
                                    string challanid = ds.Tables[0].Rows[i][0].ToString();
                                    Result += "Please Check ChallanId " + challanid + " for Wrong Deposit Date  in row " + RowNo + ",  ";
                                    dt.Rows[i]["Status"] += " , " + "Please Check ChallanId " + challanid + " for Wrong Deposit Date in row " + RowNo + ",  ";
                                }
                                if (fromDateValue.Date > LastDepositDate.Date)
                                {
                                    int RowNo = i + 2;
                                    string challanid = ds.Tables[0].Rows[i][0].ToString();
                                    Result += "Please Check ChallanId " + challanid + " for Wrong Deposit Date  in row " + RowNo + ",  ";
                                    dt.Rows[i]["Status"] += " , " + "Please Check ChallanId " + challanid + " for Wrong Deposit Date in row " + RowNo + ",  ";
                                }

                            }
                            else
                            {
                                int RowNo = i + 2;
                                string challanid = dt.Rows[i][0].ToString();
                                Result += "Please Check ChallanId " + challanid + " for Date Format in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] += " , " + "Please Check ChallanId " + challanid + " for Date Format in row " + RowNo + ",  ";
                                // do for invalid date
                            }

                            if (dsChallanDetails.Tables[0].Rows[0]["VERIFIED"].ToString() == "1")
                            {
                                int RowNo = i + 2;
                                // dt.Rows[i]["ErrDetails"] += "Total Fee Error";
                                string challanid = dt.Rows[i][0].ToString();
                                Result += "Please check ChallanId " + challanid + " is Already Verified  in row " + RowNo + ",   ";
                                dt.Rows[i]["Status"] = "Already Verified";
                            }
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                dt = null;
                Result = ex.Message;
                //return "Please Check All Fields";
            }
            //insertErrorFromExcel(Result);

            return Result;
        }



        #region  BulkChallanBank
        public DataTable BulkChallanBank(DataTable dt1, int adminid, int UPLOADLOT, BankModels BM, out int OutStatus, out string OutError)  // BulkChallanBank
        {
            //string OutError = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BulkChallanBankSP", con); // BulkChallanBankSPTest
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.AddWithValue("@ADMINID", adminid);
                    cmd.Parameters.AddWithValue("@MIS_FILENM", BM.MIS_FILENM);
                    cmd.Parameters.AddWithValue("@IP_ADD", myIP);
                    cmd.Parameters.AddWithValue("@BCODE", BM.BCODE);
                    cmd.Parameters.AddWithValue("@UPLOADLOT", UPLOADLOT);
                    cmd.Parameters.AddWithValue("@BulkChallanBank", dt1);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                OutError = ex.Message;
                OutStatus = -1;
                return null;
            }
        }
        #endregion  BulkChallanBank

        #region  BulkChallanBank For PSEB HOD


        public string CheckMisExcelExportPSEB(DataSet ds, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("STATUS", typeof(string)));

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].ToString().Length < 13 || dt.Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string challanid = dt.Rows[i][0].ToString();
                        Result = "Please check ChallanId " + challanid + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check ChallanId " + challanid + " in row " + RowNo + ",  ";
                    }

                    //

                    int totalfeeamount = 0;
                    if (dt.Rows[i][1].ToString() != "")
                    {
                        totalfeeamount = Convert.ToInt32(dt.Rows[i][1].ToString());
                    }
                    if (totalfeeamount <= 0 || dt.Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        //** gcm 
                        //dt.Rows[i]["ErrDetails"]+="Total Fee Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of Fees in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of Fees in row " + RowNo + ",  ";
                    }

                    //                    
                    DataSet dsChallanDetails = GetChallanDetailsByIdSPBank(dt.Rows[i]["ChallanID"].ToString());
                    if (dsChallanDetails == null || dsChallanDetails.Tables[0].Rows.Count == 0)
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "Invalid Challan Id";
                        string challanid = dt.Rows[i][0].ToString();
                        //Result += "Please check ChallanId " + challanid + " of  Total Fees and challan id in row " + RowNo + ",   ";
                        Result += "Please check ChallanId " + challanid + " for Challan Details/NotFound    in row " + RowNo + ",   ";
                        dt.Rows[i]["Status"] = "Please check ChallanId " + challanid + " for Challan Details/NotFound    in row " + RowNo + ",   ";
                    }
                    else
                    {
                        if (Convert.ToInt32(dsChallanDetails.Tables[0].Rows[0]["FEE"].ToString()) != totalfeeamount)
                        {
                            if (Convert.ToInt32(dsChallanDetails.Tables[0].Rows[0]["TOTFEE"].ToString()) != totalfeeamount)
                            {
                                int RowNo = i + 2;
                                // dt.Rows[i]["ErrDetails"] += "Total Fee Error";
                                string challanid = dt.Rows[i][0].ToString();
                                Result += "Please check ChallanId " + challanid + " of Total Fees not matched with challan  in row " + RowNo + ",   ";
                                dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of Total Fees not matched with challan  in row " + RowNo + ",   ";
                            }
                        }
                    }
                    //
                    if (dt.Rows[i][2].ToString() == "")
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "BRCODE Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of BRCODE in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of BRCODE in row " + RowNo + ",  ";
                    }

                    //
                    if (dt.Rows[i][3].ToString() == "")
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "BRANCH Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of BRANCH in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of BRANCH in row " + RowNo + ",  ";
                    }

                    //
                    if (dt.Rows[i][4].ToString() == "")
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "J_REF_NO Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of J_REF_NO in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of J_REF_NO in row " + RowNo + ",  ";
                    }

                    //

                    // DataSet dsChallanDetails = GetChallanDetailsByIdSPBank(dt.Rows[i]["ChallanID"].ToString());
                    //24/08/2016 05:55:38PM
                    if (dt.Rows[i][5].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " for Fill Date in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " for Fill Date in row " + RowNo + ",  ";
                    }
                    else if (dt.Rows[i][5].ToString() != "")
                    {
                        if (dsChallanDetails.Tables[0].Rows.Count > 0)
                        {
                            DateTime LastDepositDate = Convert.ToDateTime(dsChallanDetails.Tables[0].Rows[0]["BankLastdateNew"].ToString());
                            DateTime GenDate = Convert.ToDateTime(dsChallanDetails.Tables[0].Rows[0]["ChallanGDateN"].ToString());
                            // 24/08/2016 05:55:38PM
                            DateTime fromDateValue;
                            string s = dt.Rows[i][5].ToString();
                            //  string s = dt.Rows[i][5].ToString().Substring(0,10);
                            // var formats = new[] { "dd/MM/yyyy hh:mm:sstt", "yyyy-MM-dd" };                      
                            if (DateTime.TryParseExact(s, "dd/MM/yyyy hh:mm:sstt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateValue))
                            { // do for valid date 

                                //if (fromDateValue.Date < GenDate.Date)
                                //{
                                //    int RowNo = i + 2;
                                //    string challanid = ds.Tables[0].Rows[i][0].ToString();
                                //    Result += "Please Check ChallanId " + challanid + " for Wrong Deposit Date  in row " + RowNo + ",  ";
                                //    dt.Rows[i]["Status"] += " , " + "Please Check ChallanId " + challanid + " for Wrong Deposit Date in row " + RowNo + ",  ";

                                //}
                                //if (fromDateValue.Date > LastDepositDate.Date)
                                //{
                                //    int RowNo = i + 2;
                                //    string challanid = ds.Tables[0].Rows[i][0].ToString();
                                //    Result += "Please Check ChallanId " + challanid + " for Wrong Deposit Date  in row " + RowNo + ",  ";
                                //    dt.Rows[i]["Status"] += " , " + "Please Check ChallanId " + challanid + " for Wrong Deposit Date in row " + RowNo + ",  ";

                                //}
                            }
                            else
                            {

                                int RowNo = i + 2;
                                string challanid = dt.Rows[i][0].ToString();
                                Result += "Please Check ChallanId " + challanid + " for Date Format in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] += " , " + "Please Check ChallanId " + challanid + " for Date Format in row " + RowNo + ",  ";
                                // do for invalid date
                            }

                            if (dsChallanDetails.Tables[0].Rows[0]["VERIFIED"].ToString() == "1")
                            {
                                int RowNo = i + 2;
                                // dt.Rows[i]["ErrDetails"] += "Total Fee Error";
                                string challanid = dt.Rows[i][0].ToString();
                                Result += "Please check ChallanId " + challanid + " is Already Verified  in row " + RowNo + ",   ";
                                dt.Rows[i]["Status"] = "Already Verified";
                            }

                        }


                    }
                }

            }
            catch (Exception ex)
            {
                dt = null;
                Result = ex.Message;
                //return "Please Check All Fields";
            }
            //insertErrorFromExcel(Result);

            return Result;
        }

        public DataTable BulkChallanBankPSEBHOD(DataTable dt1, int adminid, int UPLOADLOT, BankModels BM, out int OutStatus)  // BulkChallanBank
        {
            string OutError = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BulkChallanBankPSEBHOD", con); // BulkChallanBankSPTest
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ADMINID", adminid);
                    cmd.Parameters.AddWithValue("@MIS_FILENM", BM.MIS_FILENM);
                    cmd.Parameters.AddWithValue("@IP_ADD", myIP);
                    cmd.Parameters.AddWithValue("@BCODE", BM.BCODE);
                    cmd.Parameters.AddWithValue("@UPLOADLOT", UPLOADLOT);
                    cmd.Parameters.AddWithValue("@BulkChallanBank", dt1);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                OutError = "";
                OutStatus = -1;
                return null;
            }
        }
        #endregion BulkChallanBank For PSEB HOD


        #region Upload Bulk online Payment
        public string CheckMisExcelExportOnline(DataSet ds, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("STATUS", typeof(string)));
                dt.Columns.Add(new DataColumn("CHALLANSTATUS", typeof(string)));
                
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].ToString().Length < 13 || dt.Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string challanid = dt.Rows[i][0].ToString();
                        Result = "Please check ChallanId " + challanid + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check ChallanId " + challanid + " in row " + RowNo + ",  ";
                    }


                    int totalfeeamount = 0;
                    if (dt.Rows[i][1].ToString() != "")
                    {
                        totalfeeamount = Convert.ToInt32(dt.Rows[i][1].ToString());
                    }
                    if (totalfeeamount <= 0 || dt.Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        //** gcm 
                        //dt.Rows[i]["ErrDetails"]+="Total Fee Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of Total Fees in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of Total Fees in row " + RowNo + ",  ";
                    }

                    DataSet dsChallanDetails = GetChallanDetailsByIdSPBank(dt.Rows[i]["ChallanID"].ToString());
                    if (dsChallanDetails == null || dsChallanDetails.Tables[0].Rows.Count == 0)
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "Invalid Challan Id";
                        string challanid = dt.Rows[i][0].ToString();
                        //Result += "Please check ChallanId " + challanid + " of  Total Fees and challan id in row " + RowNo + ",   ";
                       // Result += "Please check ChallanId " + challanid + " for Challan Details/NotFound    in row " + RowNo + ",   ";
                        dt.Rows[i]["Status"] = "Please check ChallanId " + challanid + " for Challan Details/NotFound    in row " + RowNo + ",   ";
                    }
                    else
                    {
                        if (Convert.ToInt32(dsChallanDetails.Tables[0].Rows[0]["TOTFEE"].ToString()) != totalfeeamount)
                        {
                            int RowNo = i + 2;
                            // dt.Rows[i]["ErrDetails"] += "Total Fee Error";
                            string challanid = dt.Rows[i][0].ToString();
                            Result += "Please check ChallanId " + challanid + " of Total Fees not matched with challan  in row " + RowNo + ",   ";
                            dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of Total Fees not matched with challan  in row " + RowNo + ",   ";
                        }
                    }


                    if (dt.Rows[i][2].ToString() == "")
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "BRCODE Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of BRCODE in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = dt.Rows[i]["Status"].ToString() + " , " + "Please check ChallanId " + challanid + " of BRCODE in row " + RowNo + ",  ";
                    }



                    if (dt.Rows[i][3].ToString() == "")
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "BRANCH Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of BRANCH in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of BRANCH in row " + RowNo + ",  ";
                    }
                   

                    if (dt.Rows[i][4].ToString() == "")
                    {
                        int RowNo = i + 2;
                        // dt.Rows[i]["ErrDetails"] += "J_REF_NO Error";
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " of J_REF_NO in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " of J_REF_NO in row " + RowNo + ",  ";
                    }

                    if (dt.Rows[i][5].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " for Fill Date in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " for Fill Date in row " + RowNo + ",  ";
                    }


                    //PAYMETHOD
                    if (dt.Rows[i][6].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " for PAYMETHOD in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " for PAYMETHOD in row " + RowNo + ",  ";
                    }
                    //PAYSTATUS
                    if (dt.Rows[i][7].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " for PAYSTATUS in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " for PAYSTATUS in row " + RowNo + ",  ";
                    }
                    else if (dt.Rows[i][7].ToString().ToUpper().Trim() != "SUCCESS")
                    {
                        int RowNo = i + 2;
                        string challanid = dt.Rows[i][0].ToString();
                        Result += "Please check ChallanId " + challanid + " for PAYSTATUS Must be SUCCESS Only in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " for PAYSTATUS  Must be SUCCESS Only in row " + RowNo + ",  ";
                    }
                   
                    
                    
                    if (dt.Rows[i][5].ToString() != "")
                    {
                        if (dsChallanDetails.Tables[0].Rows.Count > 0)
                        {
                            if (dsChallanDetails.Tables[0].Rows[0]["VERIFIED"].ToString() == "1")
                            {
                                int RowNo = i + 2;
                                // dt.Rows[i]["ErrDetails"] += "Total Fee Error";
                                string challanid = dt.Rows[i][0].ToString();
                                // Result += "Please check ChallanId " + challanid + " is Already Verified  in row " + RowNo + ",   ";
                                dt.Rows[i]["Status"] = "Already Verified";
                                dt.Rows[i]["CHALLANSTATUS"] = "VERIFIED";
                            }
                            else
                            {
                                dt.Rows[i]["CHALLANSTATUS"] = "PENDING";
                            }
                        }
                        else {
                            dt.Rows[i]["CHALLANSTATUS"] = "WRONG";
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                dt = null;
                Result = ex.Message;
                //return "Please Check All Fields";
            }
           

            var query = from p in dt.AsEnumerable()
                        where p.Field<string>("CHALLANSTATUS").ToUpper().Trim() == "PENDING".ToUpper().Trim()
                        select p;

            var pendingChallan = dt.AsEnumerable().Where(row => row.Field<string>("CHALLANSTATUS").ToUpper() == "PENDING".ToUpper()).ToList();
            if (query.Any())
            {
                //Creating a table from the query 
                dt = query.CopyToDataTable<DataRow>();
               // dtNEw = query.CopyToDataTable<DataRow>();

            }


            if (dt.Columns.Contains("CHALLANSTATUS"))
            {
                dt.Columns.Remove("CHALLANSTATUS");
               
            }

            return Result;
        }



        #region  BulkOnlinePayment
        public DataTable BulkOnlinePayment(DataTable dt1, int adminid, int UPLOADLOT, BankModels BM, out int OutStatus, out string OutError)  // BulkChallanBank
        {
            //string OutError = "";
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BulkOnlinePaymentSP", con); // BulkChallanBankSPTest
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.AddWithValue("@ADMINID", adminid);
                    cmd.Parameters.AddWithValue("@MIS_FILENM", BM.MIS_FILENM);
                    cmd.Parameters.AddWithValue("@IP_ADD", myIP);
                    cmd.Parameters.AddWithValue("@BCODE", BM.BCODE);
                    cmd.Parameters.AddWithValue("@UPLOADLOT", UPLOADLOT);
                    cmd.Parameters.AddWithValue("@BulkOnlinePayment", dt1);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                OutError = ex.Message;
                OutStatus = -1;
                return null;
            }
        }
        #endregion  BulkOnlinePayment
        #endregion





        #region ManualPaidFee

        public static string UpdateReceiptAttachmentManualSP(ReceiptUpdateManualModel receiptUpdateManualModel,out string OutError)
        {
            string result = "";
            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UpdateReceiptAttachmentManualSP";
                cmd.Parameters.AddWithValue("@challanid", receiptUpdateManualModel.challanid);
                cmd.Parameters.AddWithValue("@challancategory", receiptUpdateManualModel.challancategory);
                cmd.Parameters.AddWithValue("@schl", receiptUpdateManualModel.Schl);
                cmd.Parameters.AddWithValue("@appno", receiptUpdateManualModel.appno);
                cmd.Parameters.AddWithValue("@feecode", receiptUpdateManualModel.feecode);
                cmd.Parameters.AddWithValue("@ReceiptScannedCopy", receiptUpdateManualModel.ReceiptScannedCopy);
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                result = db.ExecuteNonQuery(cmd).ToString();
                OutError = (string)cmd.Parameters["@OutError"].Value;
                return result;
            }
            catch (Exception ex)
            {
                OutError = ex.Message;
                return null;
            }
        }

            public static DataSet Get_VerifyManualPaidFee_ChallanDetails(string search, string sesssion, int pageIndex)
        {
            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Get_VerifyManualPaidFee_ChallanDetails";
                cmd.Parameters.AddWithValue("@search", search);
                cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                cmd.Parameters.AddWithValue("@PageSize", 20);
                ds = db.ExecuteDataSet(cmd);
                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static string ImportBankMisSP_PSEB_ManualChallanSP(string FeeDistrict, string OldReceiptNo, string OldDepositDate, string OldChallanId, string OldAmount,string EmpUserId, BankModels BM, int UPLOADLOT, out int OutStatus, out string Mobile)  // BankLoginSP
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            string result = "";
            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "ImportBankMisSP_PSEB_ManualChallanSP";
                cmd.Parameters.AddWithValue("@FeeDistrict", FeeDistrict);
                cmd.Parameters.AddWithValue("@ApprovalStatus", BM.ApprovalStatus);
                cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                cmd.Parameters.AddWithValue("@CHALLANID", BM.CHALLANID);
                cmd.Parameters.AddWithValue("@TOTFEE", BM.TOTFEE);
                cmd.Parameters.AddWithValue("@BRCODE", BM.BRCODE);
                cmd.Parameters.AddWithValue("@BRANCH", BM.BRANCH);
                cmd.Parameters.AddWithValue("@J_REF_NO", BM.J_REF_NO);
                cmd.Parameters.AddWithValue("@DEPOSITDT", BM.DEPOSITDT);
                cmd.Parameters.AddWithValue("@MIS_FILENM", BM.MIS_FILENM);
                cmd.Parameters.AddWithValue("@IP_ADD", myIP);
                cmd.Parameters.AddWithValue("@BCODE", BM.BCODE);
                cmd.Parameters.AddWithValue("@UPLOADLOT", UPLOADLOT);
                cmd.Parameters.AddWithValue("@DepositRemarks", BM.DepositRemarks);
                //
                cmd.Parameters.AddWithValue("@OldReceiptNo", OldReceiptNo);
                cmd.Parameters.AddWithValue("@OldDepositDate", OldDepositDate);
                cmd.Parameters.AddWithValue("@OldChallanId", OldChallanId);
                cmd.Parameters.AddWithValue("@OldAmount", OldAmount);

                cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@Mobile", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                result = db.ExecuteNonQuery(cmd).ToString();
                OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                Mobile = (string)cmd.Parameters["@Mobile"].Value;
                result = OutStatus.ToString();
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                Mobile = "0";
                result = OutStatus.ToString();
            }
            return result;
        }

        #endregion
    }

}