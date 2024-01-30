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
    public class DMDB
    {
        #region Check ConString

        private string CommonCon = "myDBConnection";
        public DMDB()
        {
            CommonCon = "myDBConnection";
        }
        #endregion  Check ConString

        //---------------------------Change Password-----------------------
        public int ChangePassword(int UserId, string CurrentPassword, string NewPassword)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DMChangePasswordSP", con); //SchoolChangePasswordSP
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Dist", Dist);
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
        //-------------------------End Change Password-------------------

        #region Begin Challan Panel
        public DataSet DMChallanList(int Type, string search, int pageNumber, int PageSize)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DMChallanList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", PageSize);
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
        //

        public void ReceiveChln(int UserId, string Challanid, string RNo, string Remarks, out string OutReceiveNo, DateTime? ReceiveDate = null)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ReceiveChlnSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@challanid", Challanid);
                    cmd.Parameters.AddWithValue("@RNo", RNo);
                    cmd.Parameters.AddWithValue("@ReceiveDate", ReceiveDate);
                    cmd.Parameters.AddWithValue("@Remarks", Remarks);               
                    cmd.Parameters.Add("@OutReceiveNo", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    OutReceiveNo = Convert.ToString(cmd.Parameters["@OutReceiveNo"].Value);
                }
            }
            catch (Exception ex)
            {
                OutReceiveNo = "0";
            }
        }


        public int DMCancelReceiving(int UserId,string challanid, out int OutReceiveNo)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DMCancelReceivingSP", con); //SchoolChangePasswordSP
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Dist", Dist);
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@challanid", challanid);
                    cmd.Parameters.Add("@OutReceiveNo", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    OutReceiveNo = Convert.ToInt32(cmd.Parameters["@OutReceiveNo"].Value);
                    return result;

                }
            }
            catch (Exception ex)
            {
                OutReceiveNo = -1;
                return result = -1;
            }
            finally
            {
                // con.Close();
            }
        }
        #endregion Begin Challan Panel




        #region Begin DM Panel
        public DataSet DMSchoolList(string search, int pageNumber, int PageSize)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DMSchoolList", con);
                    cmd.CommandType = CommandType.StoredProcedure;                  
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", PageSize);
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
        //

        public void ReceiveCCE(int UserId , int class1, string RType, string Remarks, string schl, out string OutReceiveNo, DateTime? ReceiveDate = null)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ReceiveCCESP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@RType", RType);
                    cmd.Parameters.AddWithValue("@Schl", schl);
                    cmd.Parameters.AddWithValue("@Class", class1);               
                    cmd.Parameters.AddWithValue("@Remarks", Remarks);
                    cmd.Parameters.AddWithValue("@ReceiveDate", ReceiveDate);
                    cmd.Parameters.Add("@OutReceiveNo", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    OutReceiveNo = Convert.ToString(cmd.Parameters["@OutReceiveNo"].Value);

                }
            }
            catch (Exception ex)
            {
                OutReceiveNo = "0";
            }
        }

        //

        public DataSet GetReceivedCCE(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetReceivedCCESP", con);
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
        //

        public void GenerateChallanCCE(int class1, string RType, string DistAllow,  out string OutDairyNo)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GenerateChallanCCESP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RType", RType);                   
                    cmd.Parameters.AddWithValue("@Class", class1);
                    cmd.Parameters.AddWithValue("@Dist", DistAllow);                  
                    cmd.Parameters.Add("@OutDairyNo", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    OutDairyNo = Convert.ToString(cmd.Parameters["@OutDairyNo"].Value);

                }
            }
            catch (Exception ex)
            {
                OutDairyNo = "0";
            }
        }
        //


        public DataSet GetChallanDetailsByDairyNo(string DairyNo)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetChallanDetailsByDairyNoSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DairyNo", DairyNo);
                   // cmd.Parameters.AddWithValue("@UserId", UserId);
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

        //

        public void DeleteDairyNo(string DairyNo, int UserId, out int Outstatus)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DeleteDairyNoSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DairyNo", DairyNo);
                    cmd.Parameters.AddWithValue("@UserId", UserId);                    
                    cmd.Parameters.Add("@Outstatus", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    Outstatus = Convert.ToInt32(cmd.Parameters["@Outstatus"].Value);


                }
            }
            catch (Exception ex)
            {               
                Outstatus = 0;
            }
        }
        #endregion Begin DM Panel


        #region ExamBulkChallanEntry

        bool IsAllDigits(string s)
        {
            return s.All(char.IsDigit);
        }
      
        public string CheckBulkChallanExcelExport(DataSet ds, int UserId, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("STATUS", typeof(string)));
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string ReceivingNo = dt.Rows[i][0].ToString();
                        Result = "Please check Receiving No " + ReceivingNo + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check Receiving No " + ReceivingNo + " in row " + RowNo + ",  ";
                    }
                    else
                    {
                        // check numeric value
                        if (dt.Rows[i][0].ToString() != "")
                        {
                            int RowNo = i + 2;
                            string ReceivingNo = dt.Rows[i][0].ToString();
                            bool isDig = IsAllDigits(dt.Rows[i][0].ToString()); // check dig
                            if (isDig == false)
                            {
                                Result = "Enter Number Only " + ReceivingNo + " in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] =  "Enter Number Only " + ReceivingNo + " in row " + RowNo + ",  ";
                            }
                            
                        }
                    }


                    //
                    if (dt.Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string ChallanID = dt.Rows[i][1].ToString();
                        Result += "Please check ChallanID " + ChallanID + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check ChallanID " + ChallanID + " in row " + RowNo + ",  ";
                    }
                    else
                    {
                        // Check challan exists and verified
                        DataSet dsChallanDetails = new AbstractLayer.BankDB().GetChallanDetailsByIdSPNew(dt.Rows[i][1].ToString());
                        if (dsChallanDetails == null || dsChallanDetails.Tables[0].Rows.Count == 0)
                        {
                            int RowNo = i + 2;
                            string challanid = dt.Rows[i][1].ToString();
                            Result += "Please check ChallanId " + challanid + " for Challan Details/NotFound in row " + RowNo + ",   ";
                            dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " for Challan Details/NotFound in row " + RowNo + ",   ";
                        }
                        else
                        {
                            // IsVerify
                            if (Convert.ToInt32(dsChallanDetails.Tables[0].Rows[0]["VERIFIED"].ToString()) != 1)
                            {
                                int RowNo = i + 2;
                                string challanid = dt.Rows[i][1].ToString();
                                Result += "Please check ChallanId " + challanid + " is Not Verified in row " + RowNo + ",   ";
                                dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " is Not Verified in row " + RowNo + ",   ";
                            }
                            // Receive No. already assigned
                            //if (dsChallanDetails.Tables[0].Rows[0]["DMReceiveNo"].ToString() != "")
                            //{
                            //    int RowNo = i + 2;
                            //    string challanid = dt.Rows[i][1].ToString();
                            //    Result += "Please check ChallanId " + challanid + " Receive No. is assigned already in row " + RowNo + ",   ";
                            //    dt.Rows[i]["Status"] += " , " + "Please check ChallanId " + challanid + " Receive No. is assigned already in row " + RowNo + ",   ";
                            //}

                            // DuplicateChlnReceiveNo
                            //////(remove) byte harpal sir
                            //////int OutStatus = 0;
                            //////DuplicateChlnReceiveNo(UserId, dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), out OutStatus);
                            //////if (OutStatus == 1)
                            //////{
                            //////    int RowNo = i + 2;
                            //////    string ReceivingNo = dt.Rows[i][0].ToString();
                            //////    Result += "Please check Receiving No " + ReceivingNo + " is Duplicate in row " + RowNo + ",  ";
                            //////    dt.Rows[i]["Status"] += " , " + "Please check Receiving No " + ReceivingNo + " is Duplicate in row " + RowNo + ",  ";
                            //////}

                        }
                    }

                    //

                    if (dt.Rows[i][2].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string schl = dt.Rows[i][2].ToString();
                        Result += "Please check SCHLREGID " + schl + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please check SCHLREGID " + schl + " in row " + RowNo + ",  ";
                    }
                    //
                    DateTime fromDateValue;
                    string ReceivingDate = dt.Rows[i][3].ToString();
                    if (DateTime.TryParseExact(ReceivingDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out fromDateValue))
                    // if (DateTime.TryParseExact(s, "dd/MM/yyyy hh:mm:sstt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateValue))
                    { // do for valid date 

                    }
                    else
                    {
                        int RowNo = i + 2;
                        Result += "Please Check Receiving Date " + ReceivingDate + " for Date Format in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += " , " + "Please Check Receiving Date " + ReceivingDate + " for Date Format in row " + RowNo + ",  ";
                    }
                }
            }
            catch (Exception ex)
            {
                dt = null;
                Result = ex.Message;
                //return "Please Check All Fields";
            }
            return Result;
        }


        public void DuplicateChlnReceiveNo(int UserId, string ReceiveNo, string ChallanId, out int OutStatus)
        {
            SqlConnection con = null;
            int result = 0;
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("DuplicateChlnReceiveNo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@RNo", ReceiveNo);
                cmd.Parameters.AddWithValue("@ChallanId", ChallanId);
                cmd.Parameters.Add("@OutStatus", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery();
                OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
            }
            catch (Exception)
            {
                OutStatus = -1;
            }
            finally
            {
                con.Close();
            }
        }

        public DataTable BulkChallanEntry(DataTable dt1, int adminid, out int OutStatus)  // BulkChallanEntry
        {
            string OutError = "";
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BulkChallanEntrySP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@BulkChallanEntry", dt1);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar,1000).Direction = ParameterDirection.Output;
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
        #endregion  ExamBulkChallanEntry

        #region Begin Book Assessment 
        public DataSet BookAssessmentFormList(int Type, string search, int pageNumber, int PageSize)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BookAssessmentFormList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", PageSize);
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


        public DataSet ViewBookAssessmentFormList(int type, string Search, string schl, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ViewBookAssessmentFormListSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    // cmd.CommandTimeout = 120;
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@schl", schl);
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
                OutError = "-1";
                return null;
            }
        }


        #endregion


    }
}