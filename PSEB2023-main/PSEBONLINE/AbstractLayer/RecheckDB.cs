using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using PSEBONLINE.Models;
using System.Web.Mvc;
using Ionic.Zip;
using System.IO;
using System.Web.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace PSEBONLINE.AbstractLayer
{
    public class RecheckDB
    {
        #region Check ConString
        //   REcheck for Session 2016-17 Only...thats why  assign 2016-17 DB 
        private string CommonCon = "myDBConnection";
        public RecheckDB()
        {
            CommonCon = "myDBConnection";
        }

        #endregion  Check ConString

        public DataSet RecheckMasterCheckSearch(int type1, string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("RecheckMasterCheckSearchSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@type", type1);
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
            finally
            {
                // con.Close();
            }
        }


        public List<SelectListItem> RecheckCurrentYearBatchList()
        {
            List<SelectListItem> MonthList = new List<SelectListItem>();
            DataSet dschk = RecheckMasterCheckSearch(2, "");
            if (dschk.Tables.Count > 0)
            {
                if (dschk.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dschk.Tables[0].Rows.Count; i++)
                    {
                        string batch = dschk.Tables[0].Rows[i]["Month"].ToString() + "-" + dschk.Tables[0].Rows[i]["Year"].ToString();
                        MonthList.Add(new SelectListItem { Text = batch, Value = batch });
                    }
                }
            }
            return MonthList;
        }


        public List<SelectListItem> GetMonth()
        {
            List<SelectListItem> MonthList = new List<SelectListItem>();
            //MonthList.Add(new SelectListItem { Text = "Select Month", Value = "0" });
            MonthList.Add(new SelectListItem { Text = "JANUARY", Value = "JANUARY" });
            MonthList.Add(new SelectListItem { Text = "FEBUARY", Value = "FEBUARY" });
            MonthList.Add(new SelectListItem { Text = "MARCH", Value = "MARCH" });
            MonthList.Add(new SelectListItem { Text = "APRIL", Value = "APRIL" });
            MonthList.Add(new SelectListItem { Text = "MAY", Value = "MAY" });
            MonthList.Add(new SelectListItem { Text = "JUN", Value = "JUN" });
            MonthList.Add(new SelectListItem { Text = "JULY", Value = "JULY" });
            MonthList.Add(new SelectListItem { Text = "AUGUST", Value = "AUGUST" });
            MonthList.Add(new SelectListItem { Text = "SEPTEMBER", Value = "SEPTEMBER" });
            MonthList.Add(new SelectListItem { Text = "OCTOBER", Value = "OCTOBER" });
            MonthList.Add(new SelectListItem { Text = "NOVEMBER", Value = "NOVEMBER" });
            MonthList.Add(new SelectListItem { Text = "DECEMBER", Value = "DECEMBER" });
            return MonthList;
        }
        public List<SelectListItem> GetSessionYear1()
        {
            //DataTable dsSession = SessionMaster(); // passing Value to SchoolDB from model
            //List<SelectListItem> itemSession = new List<SelectListItem>();
            //foreach (System.Data.DataRow dr in dsSession.Rows)
            //{
            //    itemSession.Add(new SelectListItem { Text = @dr["yearfrom"].ToString(), Value = @dr["yearfrom"].ToString() });
            //}
            //return itemSession;
            List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession.Add(new SelectListItem { Text = "1950", Value = "1950" });
            itemSession.Add(new SelectListItem { Text = "1951", Value = "1951" });
            itemSession.Add(new SelectListItem { Text = "1952", Value = "1952" });
            itemSession.Add(new SelectListItem { Text = "1953", Value = "1953" });
            itemSession.Add(new SelectListItem { Text = "1954", Value = "1954" });
            itemSession.Add(new SelectListItem { Text = "1955", Value = "1955" });
            itemSession.Add(new SelectListItem { Text = "1956", Value = "1956" });
            itemSession.Add(new SelectListItem { Text = "1957", Value = "1957" });
            itemSession.Add(new SelectListItem { Text = "1958", Value = "1958" });
            itemSession.Add(new SelectListItem { Text = "1959", Value = "1959" });
            itemSession.Add(new SelectListItem { Text = "1960", Value = "1960" });

            itemSession.Add(new SelectListItem { Text = "1961", Value = "1961" });
            itemSession.Add(new SelectListItem { Text = "1962", Value = "1962" });
            itemSession.Add(new SelectListItem { Text = "1963", Value = "1963" });
            itemSession.Add(new SelectListItem { Text = "1964", Value = "1964" });
            itemSession.Add(new SelectListItem { Text = "1965", Value = "1965" });
            itemSession.Add(new SelectListItem { Text = "1966", Value = "1966" });
            itemSession.Add(new SelectListItem { Text = "1967", Value = "1967" });
            itemSession.Add(new SelectListItem { Text = "1968", Value = "1968" });
            itemSession.Add(new SelectListItem { Text = "1969", Value = "1969" });
            itemSession.Add(new SelectListItem { Text = "1970", Value = "1970" });

            itemSession.Add(new SelectListItem { Text = "1971", Value = "1971" });
            itemSession.Add(new SelectListItem { Text = "1972", Value = "1972" });
            itemSession.Add(new SelectListItem { Text = "1973", Value = "1973" });
            itemSession.Add(new SelectListItem { Text = "1974", Value = "1974" });
            itemSession.Add(new SelectListItem { Text = "1975", Value = "1975" });
            itemSession.Add(new SelectListItem { Text = "1976", Value = "1976" });
            itemSession.Add(new SelectListItem { Text = "1977", Value = "1977" });
            itemSession.Add(new SelectListItem { Text = "1978", Value = "1978" });
            itemSession.Add(new SelectListItem { Text = "1979", Value = "1979" });
            itemSession.Add(new SelectListItem { Text = "1980", Value = "1980" });

            itemSession.Add(new SelectListItem { Text = "1981", Value = "1981" });
            itemSession.Add(new SelectListItem { Text = "1982", Value = "1982" });
            itemSession.Add(new SelectListItem { Text = "1983", Value = "1983" });
            itemSession.Add(new SelectListItem { Text = "1984", Value = "1984" });
            itemSession.Add(new SelectListItem { Text = "1985", Value = "1985" });
            itemSession.Add(new SelectListItem { Text = "1986", Value = "1986" });
            itemSession.Add(new SelectListItem { Text = "1987", Value = "1987" });
            itemSession.Add(new SelectListItem { Text = "1988", Value = "1988" });
            itemSession.Add(new SelectListItem { Text = "1989", Value = "1989" });
            itemSession.Add(new SelectListItem { Text = "1990", Value = "1990" });

            itemSession.Add(new SelectListItem { Text = "1991", Value = "1991" });
            itemSession.Add(new SelectListItem { Text = "1992", Value = "1992" });
            itemSession.Add(new SelectListItem { Text = "1993", Value = "1993" });
            itemSession.Add(new SelectListItem { Text = "1994", Value = "1994" });
            itemSession.Add(new SelectListItem { Text = "1995", Value = "1995" });
            itemSession.Add(new SelectListItem { Text = "1996", Value = "1996" });
            itemSession.Add(new SelectListItem { Text = "1997", Value = "1997" });
            itemSession.Add(new SelectListItem { Text = "1998", Value = "1998" });
            itemSession.Add(new SelectListItem { Text = "1999", Value = "1999" });
            itemSession.Add(new SelectListItem { Text = "2000", Value = "2000" });

            itemSession.Add(new SelectListItem { Text = "2001", Value = "2001" });
            itemSession.Add(new SelectListItem { Text = "2002", Value = "2002" });
            itemSession.Add(new SelectListItem { Text = "2003", Value = "2003" });
            itemSession.Add(new SelectListItem { Text = "2004", Value = "2004" });
            itemSession.Add(new SelectListItem { Text = "2005", Value = "2005" });
            itemSession.Add(new SelectListItem { Text = "2006", Value = "2006" });
            itemSession.Add(new SelectListItem { Text = "2007", Value = "2007" });
            itemSession.Add(new SelectListItem { Text = "2008", Value = "2008" });
            itemSession.Add(new SelectListItem { Text = "2009", Value = "2009" });
            itemSession.Add(new SelectListItem { Text = "2010", Value = "2010" });
            itemSession.Add(new SelectListItem { Text = "2011", Value = "2011" });
            itemSession.Add(new SelectListItem { Text = "2012", Value = "2012" });
            itemSession.Add(new SelectListItem { Text = "2013", Value = "2013" });
            itemSession.Add(new SelectListItem { Text = "2014", Value = "2014" });
            itemSession.Add(new SelectListItem { Text = "2015", Value = "2015" });
            itemSession.Add(new SelectListItem { Text = "2016", Value = "2016" });
            itemSession.Add(new SelectListItem { Text = "2017", Value = "2017" });
            itemSession.Add(new SelectListItem { Text = "2018", Value = "2018" });
            return itemSession;
        }
        public DataTable SessionMaster()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SessionMasterPrivateSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result.Tables[0];
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public DataSet getForgotPassword(RecheckModels MS)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("getForgotPasswordTblRecheck_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Exam_Type", MS.Exam_Type);
                    cmd.Parameters.AddWithValue("@Class", MS.Class);
                    cmd.Parameters.AddWithValue("@SelYear", MS.SelYear);
                    cmd.Parameters.AddWithValue("@SelMonth", MS.SelMonth);
                    cmd.Parameters.AddWithValue("@OROLL", MS.OROLL);
                    cmd.Parameters.AddWithValue("@emailID", MS.emailID);
                    cmd.Parameters.AddWithValue("@mobileNo", MS.mobileNo);
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
        public DataSet GetRecheckExamination(RecheckModels MS)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("GetRecheckExamination_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refNo", MS.refNo);
                    cmd.Parameters.AddWithValue("@roll", MS.ROLL);
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
        public DataSet InsertTblRecheck(RecheckModels MS)  // Type 1=Regular, 2=Open
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("InsertTblRecheck_SP", con);//InsertSMFSP   //InsertSMFSPNew
                                                                                //SqlCommand cmd = new SqlCommand("InsertTblRecheck_SP_1307", con);//InsertSMFSP   //InsertSMFSPNew
                                                                                //SqlCommand cmd = new SqlCommand("InsertTblRecheck_SP_311017", con);//InsertSMFSP   
                                                                                // SqlCommand cmd = new SqlCommand("InsertTblRecheck_SP_141217", con);//InsertSMFSP   
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Month", MS.batch);
                    cmd.Parameters.AddWithValue("@Year", MS.batchYear);
                    cmd.Parameters.AddWithValue("@Type", MS.Exam_Type);
                    cmd.Parameters.AddWithValue("@Class", MS.Class);
                    cmd.Parameters.AddWithValue("@ROLL", MS.OROLL.Trim());
                    cmd.Parameters.AddWithValue("@email", MS.emailID);
                    cmd.Parameters.AddWithValue("@mobile", MS.mobileNo);
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
        public DataSet GetRecheckConfirmation(string refno)  // Type 1=Regular, 2=Open
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("GetRecheckConfirmation_SP_March", con);
                    //SqlCommand cmd = new SqlCommand("GetRecheckConfirmation_SP", con);//InsertSMFSP   //InsertSMFSPNew
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@refno", refno);
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

        public DataSet InsertRecheckSubjectList(RecheckModels MS, string Recheckevaluation, bool IsRTI)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("InsertRecheckSubjectList_Ranjan_SP", con);// InsertRecheckSubjectList_SP//InsertSMFSP   //InsertSMFSPNew
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@refno", MS.refNo);
                    cmd.Parameters.AddWithValue("@ROLL", MS.ROLL);
                    cmd.Parameters.AddWithValue("@SdtID", MS.SdtID);
                    cmd.Parameters.AddWithValue("@Class", MS.Class);
                    cmd.Parameters.AddWithValue("@SubList", MS.SubList);
                    cmd.Parameters.AddWithValue("@Recheckevaluation", Recheckevaluation);
                    cmd.Parameters.AddWithValue("@IsRTI", IsRTI);
                    cmd.Parameters.AddWithValue("@Address", MS.address);
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

        public string RecheckDeleteRecord(string Id)
        {

            string result = "";
            try
            {


                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("RecheckDeleteRecord_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", Id);
                con.Open();
                result = cmd.ExecuteScalar().ToString();
                if (con.State == ConnectionState.Open)
                    con.Close();
                return result;

            }
            catch (Exception ex)
            {

                return result = "";
            }
            finally
            {

            }
        }

        public string UnlockRecheckFinalSubmit(string refno)
        {
            SqlConnection con = null;
            string result = "";
            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("UnlockRecheckFinalSubmit_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@refno", refno);
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



        public string RecheckFinalSubmit(string refno)
        {
            SqlConnection con = null;
            string result = "";
            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("RecheckFinalSubmit_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@refno", refno);
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

        public DataSet GetRecheckFinalPrint(string refno, string ChallanId)  // Type 1=Regular, 2=Open
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("GetRecheckFinalPrint_SP", con);//InsertSMFSP   //InsertSMFSPNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", refno);
                    cmd.Parameters.AddWithValue("@ChallanId", ChallanId);
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

        public DataSet GetRecheckFinalPrint_SPRN(string refno, string ChallanId)  // Type 1=Regular, 2=Open
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("GetRecheckFinalPrint_SPRN", con);//InsertSMFSP   //InsertSMFSPNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", refno);
                    cmd.Parameters.AddWithValue("@ChallanId", ChallanId);
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


        //----------------------------BEGIN----Challan-------------
        public DataSet GetRecheckDetailsPayment(string RefNo, string form)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetRecheckDetailsPaymentSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", RefNo);
                    //cmd.Parameters.AddWithValue("@roll", roll);
                    cmd.Parameters.AddWithValue("@form", form);

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
        public string InsertPaymentFormRecheck(RecheckChallanMasterModel CM, FormCollection frm, out string CandiMobile)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                CommonCon = "myDBConnection";
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("InsertPaymentFormSP_Recheck", con);   //InsertPaymentFormSP_Recheck
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
                //cmd.Parameters.AddWithValue("@DEPOSITDT", CM.DEPOSITDT);
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
                // result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = (string)cmd.Parameters["@CHALLANID"].Value;
                CandiMobile = (string)cmd.Parameters["@SchoolMobile"].Value;
                string OutError = (string)cmd.Parameters["@OutError"].Value;
                return outuniqueid;
            }
            catch (Exception ex)
            {
                //mbox(ex);
                CandiMobile = "";
                return result = "0";

            }
            finally
            {
                con.Close();
            }
        }
        public DataSet GetChallanDetailsById(string ChallanId)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                CommonCon = "myDBConnection";
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetChallanDetailsByIdRecheckSP", con);
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
        public DataSet CheckFeeStatus(string SCHL, string type)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CheckFeeStatusSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@type", type);
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
        public DataSet GetCalculateFeeBySchool(string search, string schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCalculateFeeBySchoolSPNew", con);//GetCalculateFeeBySchoolSP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@Schl", schl);
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
        //---------------------------END----Challan-------------

        #region CheckUpdateRecheckBarDataExcel
        public static string CheckUpdateRecheckBarDataExcel(DataSet ds, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("ErrStatus", typeof(string)));
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string roll = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check ROLL " + roll + " in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] += "Please check ROLL " + roll + " in row " + RowNo + ",  ";

                    }

                    if (ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string sub = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check SUB " + sub + " in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] += "Please check SUB " + sub + " in row " + RowNo + ",  ";
                    }

                    if (ds.Tables[0].Rows[i][2].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string BAR = ds.Tables[0].Rows[i][2].ToString();
                        Result += "Please Enter BAR " + BAR + " in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] += "Please Enter BAR " + BAR + " in row " + RowNo + ",  ";
                    }

                    if (ds.Tables[0].Rows[i][3].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string LEVEL = ds.Tables[0].Rows[i][2].ToString();
                        Result += "Please Enter LEVEL " + LEVEL + " in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] += "Please Enter LEVEL " + LEVEL + " in row " + RowNo + ",  ";
                    }
                }
            }
            catch (Exception ex)
            {
                dt = null;
                Result = ex.Message;
            }
            return Result;
        }


        public static DataSet UpdateRecheckBarData(DataTable dt1, int adminid, out string OutError)
        {
            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UpdateBar12DetailsAPISP"; //GetAllFormName_sp
                cmd.Parameters.AddWithValue("@adminid", adminid);
                cmd.Parameters.AddWithValue("@tblTypeBar12Details", dt1);
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
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

        #endregion
        #region RecheckingReEvaluationTypeWiseReports


        public static DataSet RecheckingReEvaluationTypeWiseReports(int type, string cls, string month, string year, string RP, string search)
        {

            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "RecheckingReEvaluationTypeWiseReportsSP"; //GetAllFormName_sp
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@class", cls);
                cmd.Parameters.AddWithValue("@month", month);
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.AddWithValue("@RP", RP);
                cmd.Parameters.AddWithValue("@search", search);
                ds = db.ExecuteDataSet(cmd);
                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        #endregion RecheckingReEvaluationTypeWiseReports
    }
}