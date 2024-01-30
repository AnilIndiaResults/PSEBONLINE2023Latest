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
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace PSEBONLINE.AbstractLayer
{
    public class ReportDB
    {
        #region Check ConString

        private string CommonCon = "myDBConnection";
        public ReportDB()
        {
            CommonCon = "myDBConnection";
        }
        #endregion  Check ConString



        public DataSet NinthEleventhReport()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("NinthEleventhReport_SP", con);
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

        public DataSet CCEGradingReport(int Type)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CCEGradingReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 200;
                    cmd.Parameters.AddWithValue("@Type", Type);
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

        public DataSet CCEGradingPendingSchoolList(int Type)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CCEGradingPendingSchoolList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type);
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

        public DataSet RegandExamFormFeeSummarywithDate(int Type, string fromDate, string toDate)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("RegandExamFormFeeSummarywithDate", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@fromDate", fromDate);
                    cmd.Parameters.AddWithValue("@toDate", toDate);
                    ad.SelectCommand = cmd;
                    cmd.CommandTimeout = 300;
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

        public DataSet PSEBReport(int Type)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("dummyPSEBReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type);
                    ad.SelectCommand = cmd;
                    cmd.CommandTimeout = 300;
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

        public DataSet PSEBPaymentSummaryReport()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PSEBPaymentSummaryReport", con);
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

        public DataSet GetDataByQuery(string query)
        {
            DataSet ds = new DataSet();
           // SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlDataAdapter ad = new SqlDataAdapter(query, con);
                    ad.Fill(ds);
                    con.Open();
                    return ds;   
                }
            }
            catch (Exception ex)
            {
                return ds = null;
            }
        }


        public DataSet FinalPrint(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("FinalPrintSP", con);
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


        public DataSet finalprintsubDetail(string schl, string lot, string formM1,string formM2,string formT1,string formT2)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("finalprintsubDetailrohit", con); //[finalprintsubDetail]
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@lot", lot);
                    cmd.Parameters.AddWithValue("@formM1", formM1);
                    cmd.Parameters.AddWithValue("@formM2", formM2);
                    cmd.Parameters.AddWithValue("@formT1", formT1);
                    cmd.Parameters.AddWithValue("@formT2", formT2);
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

        public DataSet SubjectSPRegular(string schl, string lot, string form, string exam, string flag)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SubjectSPRegular", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@lot", lot);
                    cmd.Parameters.AddWithValue("@Form_Name", form);
                    cmd.Parameters.AddWithValue("@exam", exam);
                    cmd.Parameters.AddWithValue("@flag", flag);                   
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


        public DataSet spFeeUndertaking(string schl, string lot, DateTime Admissiondate)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("spFeeUndertaking", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schl", schl);
                    cmd.Parameters.AddWithValue("@lot", lot);
                    cmd.Parameters.AddWithValue("@Admissiondate", Admissiondate);                   
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

        //-----------------------------Report DB----
        public DataSet getPrivateExamFormsStatus()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PrivateExamFormsStatus_SP", con);
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

        //--------------------------EndDb-------------

        #region  BankWiseFeeCollectionDetails
        public DataSet BankWiseFeeCollectionDetails(string search, out string OutError )
        {           
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BankWiseFeeCollectionDetailsSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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
        #endregion  BankWiseFeeCollectionDetails

        #region  CategoryWiseFeeCollectionDetails
        public DataSet CategoryWiseFeeCollectionDetails(string search, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CategoryWiseFeeCollectionDetailsSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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
        #endregion  CategoryWiseFeeCollectionDetails
        #region ChallanFormReceivingStatus

        public DataSet Get_ChallanFormReceivingStatus(string feecodes)
        {
            if (string.IsNullOrEmpty(feecodes))
            {
                feecodes = "31,32";
            }
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConn2017"].ConnectionString);
            SqlCommand cmd = new SqlCommand("sp_ChallanFormRecievingStatus_Report", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@feecode", feecodes);
            DataSet ds = new DataSet();
            new SqlDataAdapter(cmd).Fill(ds);
            return ds;
        }

        public DataSet Get_feeCodes()
        {
            SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConn2017"].ConnectionString);
            SqlCommand cmd = new SqlCommand("sp_Get_feeCodes", con);
            cmd.CommandType = CommandType.StoredProcedure;
            DataSet ds = new DataSet();
            new SqlDataAdapter(cmd).Fill(ds);
            return ds;
        }
        #endregion ChallanFormReceivingStatus

        #region Registration Report

        public DataSet RegistrationReport(string adminid)
        {            
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("RegistrationReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
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

        public DataSet RegistrationReportSearch(string Search, string Session, string SelUserType)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("RegistrationReportSearchSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.AddWithValue("@Session", Session);
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@SelUserType", SelUserType);
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


        #endregion Registration Report

        #region PrivateCountReport


        public DataSet PrivateCountReport(int Type, string batch, string search, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PrivateCountReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@batch", batch);
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

        #endregion PrivateCountReport

        #region ExamReport

        public DataSet SummaryOfExaminationReport(string adminid)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SummaryOfExaminationReport_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
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

        //public DataSet SummaryOfExaminationReport(string adminid)
        //{
        //    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ConnectionString);
        //    SqlDataAdapter adp = new SqlDataAdapter("SummaryOfExaminationReport_sp", con);
        //    adp.SelectCommand.CommandType = CommandType.StoredProcedure;        //  
        //    adp.SelectCommand.Parameters.AddWithValue("@adminid", adminid);
        //    DataSet ds = new DataSet();
        //    ds.Clear();
        //    ds.EnforceConstraints = false;
        //    try
        //    {
        //        adp.Fill(ds);
        //    }
        //    catch (Exception e)
        //    {
        //        ds = new DataSet();
        //    }
        //    return ds;
        //}

        #endregion ExamReport

        #region RegNoStatus Report
        public DataSet RegNoStatusReport()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("RegNo_Status_SP", con);
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

        #endregion RegNoStatus Report

		 public DataSet StatusofCorrection()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("StatusofCorrectionReportSPNEW", con);//StatusofCorrectionReportSP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 200;
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


        #region CCE Summary Report
        public DataSet CCESummaryReport(int Type, int cls)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CCESummaryReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@cls", cls);
                    cmd.CommandTimeout = 200;
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

        #endregion CCE Summary Report

        #region Practical Summary Report
        public DataSet PracticalSummaryReport(int Type, int cls)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PracticalSummaryReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 180;
                    cmd.Parameters.AddWithValue("@Type", Type);
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

        #endregion Practical Summary Report



        #region  DateWiseFeeCollectionDetails
        public DataSet DateWiseFeeCollectionDetails(string DateType,string search,string type, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DateWiseFeeCollectionDetailsSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DateType", DateType);
                    cmd.Parameters.AddWithValue("@type", type);
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
        #endregion  DateWiseFeeCollectionDetails

        #region SchoolPremisesInformationReport
        public DataSet SchoolPremisesInformationReport()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SchoolPremisesInformationReport", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 200;
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
        #endregion SchoolPremisesInformationReport

        #region MigrationCountReport DB
        public DataSet MigrationCountReport(string search, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("MigrationCountReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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

        #endregion MigrationCountReport DB

        #region SchoolStaffSummaryReport
        public DataSet SchoolStaffSummaryReport()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SchoolStaffSummaryReport", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 200;
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
        #endregion SchoolStaffSummaryReport

        #region StoppedRollSummaryReport
        public DataSet StoppedRollSummaryReport()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("StoppedRollSummaryReport", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 200;
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
        #endregion StoppedRollSummaryReport

        #region PracticalExamPendingSchoolReport
        public DataSet PracticalExamPendingSchoolReport(string RP, int cls)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PracticalExamPendingSchoolReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 200;
                    cmd.Parameters.AddWithValue("@RP", RP);
                    cmd.Parameters.AddWithValue("@Class", cls);
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

        #endregion PracticalExamPendingSchoolReport


        #region PracticalPendingCandidatesReport
        public DataSet PracticalPendingCandidatesReport(string type,string RP, int cls)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PracticalPendingCandidatesReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.CommandTimeout = 100;
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@Class", cls);
                    cmd.Parameters.AddWithValue("@RP", RP);
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

        #endregion PracticalPendingCandidatesReport

        #region RecheckSummaryReport
        public DataSet RecheckSummaryReport(string cls, string month, string year, string RP)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("RecheckSummaryReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.CommandTimeout = 200;
                    cmd.Parameters.AddWithValue("@class", cls);
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.Parameters.AddWithValue("@year", year);
                    cmd.Parameters.AddWithValue("@RP", RP);
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

        #endregion RecheckSummaryReport




        #region EAffiliationReport
        public DataSet EAffiliationReport()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("EAffiliationReport", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.CommandTimeout = 200;
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
        #endregion StoppedRollSummaryReport


        #region ClassSubjectWiseExaminationReport
        public DataSet ClassSubjectWiseExaminationReport(int type,string RP, string DIST,int cls)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ClassSubjectWiseExaminationReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@RP", RP);
                    cmd.Parameters.AddWithValue("@DIST", DIST);                    
                    cmd.Parameters.AddWithValue("@Class", cls);
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

        #endregion ClassSubjectWiseExaminationReport


        #region EAffiliationApplicationsReceivedReport
        public DataSet EAffiliationApplicationsReceivedReport(int type,string DIST, int cls, string Search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("EAffiliationApplicationsReceivedReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@type", type);                    
                    cmd.Parameters.AddWithValue("@DIST", DIST);
                    cmd.Parameters.AddWithValue("@Class", cls);
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

        #endregion EAffiliationApplicationsReceivedReport


        public  DataSet ClusterRegisterReport(string search, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ClusterRegisterReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;                    
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
                return null;
            }
        }


        #region OpenSchoolAccreditationReport

        public DataSet OpenSchoolAccreditationReport(string dist)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("OpenSchoolAccreditationReport", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@dist", dist);                    
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {               
                return null;
            }
        }



        #endregion OpenSchoolAccreditationReport

        #region  EAffiliationSummaryReport
        public DataSet EAffiliationSummaryReport(string search, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("EAffiliationSummaryReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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
        #endregion  EAffiliationSummaryReport


        #region Middle Primary 
        public static DataSet ExaminationAllClassWiseSubjectWiseReport(string ReportType, int cls, string dist)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "ExaminationAllClassWiseSubjectWiseReportSP";
                cmd.Parameters.AddWithValue("@ReportType", ReportType);
                cmd.Parameters.AddWithValue("@Class", cls);
                cmd.Parameters.AddWithValue("@Dist", dist);
                return db.ExecuteDataSet(cmd);
            }
            catch (Exception)
            {
                return null;
            }

        }


        public static DataSet PanelWiseMarkPendingSummaryReport(int ReportType, int cls)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "PanelWiseMarkPendingSummaryReportSP";
                cmd.Parameters.AddWithValue("@ReportType", ReportType);
                cmd.Parameters.AddWithValue("@cls", cls);
                return db.ExecuteDataSet(cmd);
            }
            catch (Exception)
            {
                return null;
            }

        }

        #endregion

        public static DataSet ClassWiseSchoolWiseReport(string ReportType, int cls, string dist)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "ClassWiseSchoolWiseReportSP";
                cmd.Parameters.AddWithValue("@ReportType", ReportType);
                cmd.Parameters.AddWithValue("@Class", cls);
                cmd.Parameters.AddWithValue("@Dist", dist);
                return db.ExecuteDataSet(cmd);
            }
            catch (Exception)
            {
                return null;
            }

        }


        public static DataSet ClusterMarkingStatusReport(int ReportType, string userId, string usertype, string search, out string OutError)
        {
            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "ClusterMarkingStatusReportSP";
                cmd.Parameters.AddWithValue("@reporttype", ReportType);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@usertype", usertype);
                cmd.Parameters.AddWithValue("@search", search);
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                ds = db.ExecuteDataSet(cmd);
                OutError = (string)cmd.Parameters["@OutError"].Value;
                return ds;
            }
            catch (Exception ex)
            {
                OutError = "";
                return null;
            }

        }

        public static ClusterReportModel BindAllListofClusterReport()
        {
            ClusterReportModel clusterReportModel = new ClusterReportModel();
            try
            {

                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "BindAllListofClusterReport";
                ds = db.ExecuteDataSet(cmd);
                if (ds != null)
                {
                    var eList = ds.Tables[0].AsEnumerable().Select(dataRow => new ClusterModel
                    {
                        dist = dataRow.Field<string>("dist"),
                        ccode = dataRow.Field<string>("ccode"),
                        clusternm = dataRow.Field<string>("clusterdetails"),
                    }).ToList();

                    var subList = ds.Tables[1].AsEnumerable().Select(dataRow => new SubjectModel
                    {
                        sub = dataRow.Field<string>("sub"),
                        subnm = dataRow.Field<string>("name_eng"),
                    }).ToList();

                    clusterReportModel.ClusterList = eList.ToList();
                    clusterReportModel.SubList = subList.ToList();
                }

            }
            catch (Exception ex)
            {
                //return null;
            }

            return clusterReportModel;
        }


        public static DataSet TheoryMarksStatusReport(string ReportType, int cls)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "TheoryMarksStatusReportSP";
                cmd.Parameters.AddWithValue("@ReportType", ReportType);
                cmd.Parameters.AddWithValue("@Class", cls);

                return db.ExecuteDataSet(cmd);
            }
            catch (Exception)
            {
                return null;
            }

        }


        #region PanelWiseClassWiseSummaryReport

        public static DataSet PanelWiseClassWiseSummaryReport(int Type, int cls)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "PanelWiseClassWiseSummaryReportSP";
                cmd.Parameters.AddWithValue("@Type", Type);
                cmd.Parameters.AddWithValue("@cls", cls);
                return db.ExecuteDataSet(cmd);
            }
            catch (Exception)
            {
                return null;
            }
        }

       #endregion PanelWiseClassWiseSummaryReport

        #region BankWiseChallanMasterData

        public DataSet BankWiseChallanMasterData(string DateType, string search, string bankcode, string feecat,string type, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BankWiseChallanMasterDataSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DateType", DateType);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@bankcode", bankcode);
                    cmd.Parameters.AddWithValue("@feecat", feecat);
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

        #endregion BankWiseChallanMasterData

        #region PanelWiseClassWiseSummaryReport
        public DataSet EAffiliationModuleWiseSummaryReport(int Type, int cls)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("EAffiliationModuleWiseSummaryReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type);                    
                    cmd.CommandTimeout = 300;
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

        #endregion PanelWiseClassWiseSummaryReport

        #region  MonthWiseCategoryWiseFeeCollectionDetail
        public DataSet MonthWiseCategoryWiseFeeCollectionDetails(string search, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("MonthWiseCategoryWiseFeeCollectionDetailsSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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
        #endregion  CategoryWiseFeeCollectionDetails


        #region MagazineSchoolRequirementsReport
        public DataSet MagazineSchoolRequirementsReport(int Type)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("MagazineSchoolRequirementsReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.CommandTimeout = 300;
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

        #endregion PanelWiseClassWiseSummaryReport

        #region UndertakingOfQuestionPapersReport
        public static DataSet UndertakingOfQuestionPapersReport(int Type, string Month, string Year)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "UndertakingOfQuestionPapersReportSP";
                cmd.Parameters.AddWithValue("@Month", Month);
                cmd.Parameters.AddWithValue("@Year", Year);
                cmd.Parameters.AddWithValue("@Type", Type);
                return db.ExecuteDataSet(cmd);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion UndertakingOfQuestionPapersReport


        #region PanelWiseClassWiseSummaryReport
        public DataSet PreviousClassMarksOfSeniorPanelWiseSummaryReport(int Type, int cls)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PreviousClassMarksOfSeniorPanelWiseSummaryReportSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@cls", cls);
                    cmd.CommandTimeout = 300;
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

        #endregion PreviousClassMarksOfSeniorPanelWiseSummaryReport


        #region  OverAllVerifiedFeeCollectionDetails
        public static DataSet OverAllVerifiedFeeCollectionDetails(string DateType, string search, string type, out string OutError)
        {
           
            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "OverAllVerifiedFeeCollectionDetailsSP";
                cmd.Parameters.AddWithValue("@DateType", DateType);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@search", search);
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                ds =  db.ExecuteDataSet(cmd);
                OutError = (string)cmd.Parameters["@OutError"].Value;
                return ds;
            }
            catch (Exception)
            {
                OutError = "";
                return null;
            }

        }
        #endregion  OverAllVerifiedFeeCollectionDetails


        #region  OpenSchoolAdmissionCandidateReport
        public static DataSet OpenSchoolAdmissionCandidateReport(int SelType, string search,  out string OutError)
        {

            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "OpenSchoolAdmissionCandidateReportSP";
                cmd.Parameters.AddWithValue("@SelType", SelType);
                cmd.Parameters.AddWithValue("@search", search);
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                ds = db.ExecuteDataSet(cmd);
                OutError = (string)cmd.Parameters["@OutError"].Value;
                return ds;
            }
            catch (Exception)
            {
                OutError = "";
                return null;
            }

        }
        #endregion  OverAllVerifiedFeeCollectionDetails


        #region MagazineSchoolRequirementsReport
        public static DataSet ModuleWisePendingSummaryReport(int ReportType, int cls)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "ModuleWisePendingSummaryReportSP";
                cmd.Parameters.AddWithValue("@ReportType", ReportType);
                cmd.Parameters.AddWithValue("@cls", cls);
                return db.ExecuteDataSet(cmd);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion PanelWiseClassWiseSummaryReport


        #region OnDemandCertificatesSummaryReport
        public static DataSet OnDemandCertificatesSummaryReport(int Type, string cls)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "OnDemandCertificatesSummaryReportSP";
                cmd.Parameters.AddWithValue("@Type", Type);
                cmd.Parameters.AddWithValue("@cls", cls);
                return db.ExecuteDataSet(cmd);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion OnDemandCertificatesSummaryReport

        public static DataSet EAffiliation_AppType_DownloadData(string AppType)
        {
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "EAffiliation_AppType_DownloadDataSP";
                cmd.Parameters.AddWithValue("@AppType", AppType);

                return db.ExecuteDataSet(cmd);
            }
            catch (Exception)
            {
                return null;
            }

        }

        
    }
}
