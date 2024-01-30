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
using DocumentFormat.OpenXml;
using ClosedXML.Excel;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace PSEBONLINE.AbstractLayer
{
    public class DEODB
    {
        #region Check ConString

        private string CommonCon = "myDBConnection";
        public DEODB()
        {
            CommonCon = "myDBConnection";
        }
        #endregion  Check ConString

        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        public DataSet GetDeoMonthYear(int type)  // Type 1=Regular, 2=Open
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetDeoMonthYear", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@type", type);
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //con.Close();
            }
        }


        public DataSet CheckLogin(DEOModel DM,string ExamCentre)  // Type 1=Regular, 2=Open
        {

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("LoginDEOSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserName", DM.username);
                    cmd.Parameters.AddWithValue("@Password", DM.Password);
                    cmd.Parameters.AddWithValue("@ExamCentre", ExamCentre);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    return dataTable;

                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                //con.Close();
            }
        }
        public DataSet CapacityLetter(string SCHL)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CapacityLetter_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet AdminGetDeoDIST(string Deologin)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AdminGetDeoDIST_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Deologin", Deologin);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetDeoDIST(string Deologin)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetDeoDIST_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Deologin", Deologin);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetDeoClusterSchlDISTWise(string DeoDist)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetDeoClusterSchlDISTWise_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DeoDist", DeoDist);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetDeoSchlDISTWise(string DeoDist)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetDeoSchlDISTWise_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DeoDist", DeoDist);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SelectClusterListByUser(string search, string sesssion, int pageIndex,string ExamMonth)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectClusterListByUser_sp", con); //GetAll10thPass
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 10);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);

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
        public DataSet SelectClusterListByUserCount(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectClusterListByUserCount_sp", con); //GetAll10thPass
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
        public DataSet SelectCenterListByUserReportPrint(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectCenterListByUserReportPrint", con); //GetAll10thPass
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    //cmd.Parameters.AddWithValue("@ses", sesssion);
                    //cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    //cmd.Parameters.AddWithValue("@PageSize", 10);

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
        public DataSet SelectCenterListByUserReport(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectCenterListByUserReport", con); //GetAll10thPass
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
        public DataSet SelectCenterListByUserCountReport(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectCenterListByUserCountReport_sp", con); //GetAll10thPass
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
        public DataSet ViewClusterCentre(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ViewClusterCentre_sp", con); //GetAll10thPass
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
        public DataSet CenterList(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CenterList_sp", con); //GetAll10thPass
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

        public static DataSet GetCenterListSP(string search,string exammonth)
        {
            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetCenterListSP"; //GetAllFormName_sp
                cmd.Parameters.AddWithValue("@search", search);
                cmd.Parameters.AddWithValue("@exammonth", exammonth);
                ds = db.ExecuteDataSet(cmd);
                return ds;

            }
            catch (Exception ex)
            {               
                return null;
            }

        }


        public DataSet SelectCenterListByUser(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectCenterListByUser_sp", con); //GetAll10thPass
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
        //public DataSet SearchCenterListByCCODE(string search, string sesssion, int pageIndex)
        //{
        //    DataSet result = new DataSet();
        //    SqlDataAdapter ad = new SqlDataAdapter();

        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
        //        {
        //            SqlCommand cmd = new SqlCommand("SearchCenterListByCCODE_sp", con); //SelectCenterListByUser_sp
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@search", search);
        //            cmd.Parameters.AddWithValue("@ses", sesssion);
        //            cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
        //            cmd.Parameters.AddWithValue("@PageSize", 10);

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
        public string Update_Cluster_To_CentreShift(string CCODE, string Dist, string CHKCent, string OldClsid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_Cluster_To_CentreShift_sp", con); ///Update_Cluster_To_StaffShift_sp
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CCODE", CCODE);
                cmd.Parameters.AddWithValue("@Dist", Dist);
                cmd.Parameters.AddWithValue("@CHKCent", CHKCent);
                cmd.Parameters.AddWithValue("@oldCLS", OldClsid);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                if (con.State == ConnectionState.Open)
                    con.Close();

                return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                if (con != null)
                {
                    con.Dispose();
                    con.Close();
                    con = null;
                }
                return result = "";

            }
            finally
            {
                if (con != null)
                {
                    con = null;
                }
            }
        }
        public DataSet CenterCount(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CenterCount_sp", con); //GetAll10thPass
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
        public DataSet SelectCenterListByUserCount(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectCenterListByUserCount_sp", con); //GetAll10thPass
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

        public DataSet SelectClusterListByUserCountReport(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectClusterListByUserCountReport", con); //GetAll10thPass
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
        public DataSet FinalSubmitDeoPortalGridValidation(string DIST)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("FinalSubmitDeoPortalGridValidation_sp", con); //GetAll10thPass
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DIST", DIST);
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
        public DataSet FinalSubmitDeoPortalGrid(string DIST)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("FinalSubmitDeoPortalGrid", con); //GetAll10thPass
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DIST", DIST);
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
        public DataSet SelectClusterWiseCentreReportLot(string search, string lot)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectClusterWiseCentreReportPrintlot", con); //SelectClusterWiseCentreReport
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@lot", lot);
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

        public DataSet ClusterListReportPrintlot(string search, string lot)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectClusterListReportPrintlot", con); //GetAll10thPass
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@lot", lot);
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
        public DataSet SelectClusterWiseStaffListReportLot(string search, string lot)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectClusterWiseStaffListReportLOT", con); //GetAll10thPass
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@lot", lot);
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
        public DataSet SelectClusterWiseStaffListReport(string search, string sesssion, int pageIndex,string ExamMonth)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectClusterWiseStaffListReport", con); //GetAll10thPass
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
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

        public DataSet SelectClusterWiseCentreReport(string search, string sesssion, int pageIndex,string ExamMonth)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectClusterWiseCentreReport", con); //GetAll10thPass
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 10);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
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
        public DataSet SelectClusterListByUserReport(string search, string sesssion, int pageIndex,string ExamMonth)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectClusterListByUserReport", con); //GetAll10thPass
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
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
        public string CreateClusterNew(string ClusterName, string Person1, string Mobile1, string Person2, string Mobile2, string Pincode, string CENT, string DISTCODE, string USERID,string ExamMonth)
        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CreateClusterNew_Sp", con);//ReGenerateChallaanByIdSPAdminNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CENT", CENT);
                    cmd.Parameters.AddWithValue("@DISTCODE", DISTCODE);
                    cmd.Parameters.AddWithValue("@USERID", USERID);
                    cmd.Parameters.AddWithValue("@ClusterName", ClusterName);
                    cmd.Parameters.AddWithValue("@Person1", Person1);
                    cmd.Parameters.AddWithValue("@Mobile1", Mobile1);
                    cmd.Parameters.AddWithValue("@Person2", Person2);
                    cmd.Parameters.AddWithValue("@Mobile2", Mobile2);
                    cmd.Parameters.AddWithValue("@Pincode", Pincode);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);

                    con.Open();
                    result = cmd.ExecuteNonQuery().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }
        public DataTable ReportCenterListByUser(string search)
        {
            DataTable result = new DataTable();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ReportCenterListByUser_sp", con); //GetAll10thPass
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
        public DataSet GetClusterSTAFFWise(string CCODE, string dist)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetClusterSTAFFWise_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ccode", CCODE);
                    cmd.Parameters.AddWithValue("@dist", dist);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        //-------------------------------STAFF DETAILS------------------------
        public string Update_Cluster_To_StaffShift(string CCODE, string Dist, string CHKStaffID, string ExamMonth)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_Cluster_To_StaffShift_sp", con); ///Update_CCODE_To_CENTRE_SP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CCODE", CCODE);
                cmd.Parameters.AddWithValue("@Dist", Dist);
                cmd.Parameters.AddWithValue("@CHKStaffID", CHKStaffID);
                cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                if (con.State == ConnectionState.Open)
                    con.Close();

                return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                if (con != null)
                {
                    con.Dispose();
                    con.Close();
                    con = null;
                }
                return result = "";

            }
            finally
            {
                if (con != null)
                {

                    con = null;
                }
            }
        }
        public List<SelectListItem> GetStaffExp()
        {
            List<SelectListItem> DperList = new List<SelectListItem>();
            //for (int i = 74; i >=1; i--)
            //{
            //    DperList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            //} 
            for (int i = 1; i <= 40; i++)
            {
                DperList.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
            return DperList;
        }
        public DataSet GetClusterNameBuldingCount(string CCODE,string ExamMonth)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetClusterNameBuldingCount_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ccode", CCODE);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetClusterSCHOOLSTAFF(string CCODE,string ExamMonth)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetClusterSCHOOLSTAFF_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ccode", CCODE);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SelectSTAFFClusterANDStaffWise(string Stfid, string CCODE,string ExamMonth)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectSTAFFClusterANDStaffWise_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Staffid", Stfid);
                    cmd.Parameters.AddWithValue("@ccode", CCODE);
                    cmd.Parameters.AddWithValue("@ExamMonth ", ExamMonth);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetSTAFFClusterWise(string CCODE, string ExamMonth)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetSTAFFClusterWise_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ccode", CCODE);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        //public string ADDSTAFFDETAILS(string uid,string dist,string CCODE,string TD, string fname, string Name, string expe, string Month, string gen, string mob, string SCHL, string Schoolname, string Aadharnum, string Epunjabid, string cadre, string desi, string IFSC, string Accno,string DOR, string Remark)
        public string BulkUploadADDSTAFFDETAILS(string uid, string dist, string CCODE, string TD, string fname, string Name, string expe, string Month, string gen, string mob, string SCHL, 
            string Schoolname, string Aadharnum, string Epunjabid, string cadre, string desi, string IFSC, string Accno,string ExamMonth)

        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BulkUploadADDSTAFFDETAILS_Sp", con);//CreateClusterNew_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@uid", uid);
                    cmd.Parameters.AddWithValue("@dist", dist);
                    cmd.Parameters.AddWithValue("@ccode", CCODE);
                    cmd.Parameters.AddWithValue("@TD", TD);
                    cmd.Parameters.AddWithValue("@fname", fname);
                    cmd.Parameters.AddWithValue("@Name", Name);
                    cmd.Parameters.AddWithValue("@expe", expe);
                    cmd.Parameters.AddWithValue("@Month", Month);
                    cmd.Parameters.AddWithValue("@gen", gen);
                    cmd.Parameters.AddWithValue("@mob", mob);
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@Schoolname", Schoolname);

                    cmd.Parameters.AddWithValue("@Aadharnum", Aadharnum);
                    cmd.Parameters.AddWithValue("@Epunjabid", Epunjabid);
                    cmd.Parameters.AddWithValue("@cadre", cadre);
                    cmd.Parameters.AddWithValue("@desi", desi);
                    cmd.Parameters.AddWithValue("@IFSC", IFSC);
                    cmd.Parameters.AddWithValue("@Accno", Accno);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
                    //cmd.Parameters.AddWithValue("@DOR", DOR);
                    //cmd.Parameters.AddWithValue("@Remark", Remark);

                    con.Open();
                    //result = cmd.ExecuteNonQuery().ToString();
                    result = cmd.ExecuteScalar().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }
        public string ADDSTAFFDETAILS(string DeoUser, string uid, string dist, string CCODE, string TD, string fname, string Name, string expe, string Month, string gen, string mob, string SCHL, string Schoolname, string Aadharnum, string Epunjabid, string cadre, string desi, string IFSC, string Accno, string phy, string DOB, string SelDist, string homeaddress, string homedist, string bankname)
        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ADDSTAFFDETAILS_Sp", con);//CreateClusterNew_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DeoUser", DeoUser);
                    cmd.Parameters.AddWithValue("@uid", uid);
                    cmd.Parameters.AddWithValue("@dist", dist);
                    cmd.Parameters.AddWithValue("@ccode", CCODE);
                    cmd.Parameters.AddWithValue("@TD", TD);
                    cmd.Parameters.AddWithValue("@fname", fname);
                    cmd.Parameters.AddWithValue("@Name", Name);
                    cmd.Parameters.AddWithValue("@expe", expe);
                    cmd.Parameters.AddWithValue("@Month", Month);
                    cmd.Parameters.AddWithValue("@gen", gen);
                    cmd.Parameters.AddWithValue("@mob", mob);
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@Schoolname", Schoolname);

                    cmd.Parameters.AddWithValue("@Aadharnum", Aadharnum);
                    cmd.Parameters.AddWithValue("@Epunjabid", Epunjabid);
                    cmd.Parameters.AddWithValue("@cadre", cadre);
                    cmd.Parameters.AddWithValue("@desi", desi);
                    cmd.Parameters.AddWithValue("@IFSC", IFSC);
                    cmd.Parameters.AddWithValue("@Accno", Accno);

                    cmd.Parameters.AddWithValue("@phy", phy);
                    cmd.Parameters.AddWithValue("@DOB", DOB);
                    cmd.Parameters.AddWithValue("@SelDist", SelDist);
                    cmd.Parameters.AddWithValue("@homeaddress", homeaddress);
                    cmd.Parameters.AddWithValue("@homedist", homedist);
                    cmd.Parameters.AddWithValue("@bankname", bankname);

                    //cmd.Parameters.AddWithValue("@DOR", DOR);
                    //cmd.Parameters.AddWithValue("@Remark", Remark);

                    con.Open();
                    //result = cmd.ExecuteNonQuery().ToString();
                    result = cmd.ExecuteScalar().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }
        public string ADDSTAFFDETAILS_NEW(string uid, string dist, string CCODE, string TD, string fname, string Name, string expe, string Month, string gen, string mob, string SCHL, string Schoolname, string Aadharnum, string Epunjabid, string cadre, string desi, string IFSC, string Accno, string edublock, string educluster, string UDISE, string schlType, string schlmgmt, string SCHLCAT, string dob)
        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ADDSTAFFDETAILS_New_Sp", con);//CreateClusterNew_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@uid", uid);
                    cmd.Parameters.AddWithValue("@dist", dist);
                    cmd.Parameters.AddWithValue("@ccode", CCODE);
                    cmd.Parameters.AddWithValue("@TD", TD);
                    cmd.Parameters.AddWithValue("@fname", fname);
                    cmd.Parameters.AddWithValue("@Name", Name);
                    cmd.Parameters.AddWithValue("@expe", expe);
                    cmd.Parameters.AddWithValue("@Month", Month);
                    cmd.Parameters.AddWithValue("@gen", gen);
                    cmd.Parameters.AddWithValue("@mob", mob);
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@Schoolname", Schoolname);

                    cmd.Parameters.AddWithValue("@Aadharnum", Aadharnum);
                    cmd.Parameters.AddWithValue("@Epunjabid", Epunjabid);
                    cmd.Parameters.AddWithValue("@cadre", cadre);
                    cmd.Parameters.AddWithValue("@desi", desi);
                    cmd.Parameters.AddWithValue("@IFSC", IFSC);
                    cmd.Parameters.AddWithValue("@Accno", Accno);

                    cmd.Parameters.AddWithValue("@edublock", edublock);
                    cmd.Parameters.AddWithValue("@educluster", educluster);
                    cmd.Parameters.AddWithValue("@UDISE", UDISE);
                    cmd.Parameters.AddWithValue("@schlType", schlType);
                    cmd.Parameters.AddWithValue("@schlmgmt", schlmgmt);
                    cmd.Parameters.AddWithValue("@SCHLCAT", SCHLCAT);
                    cmd.Parameters.AddWithValue("@dob", dob);

                    //cmd.Parameters.AddWithValue("@DOR", DOR);
                    //cmd.Parameters.AddWithValue("@Remark", Remark);

                    con.Open();
                    //result = cmd.ExecuteNonQuery().ToString();
                    result = cmd.ExecuteScalar().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }
        public string UPDATESTAFFDETAILS(string staffid, string TD, string fname, string Name, string expe, string Month, string gen, string mob, string SCHL, string Schoolname, string Aadharnum, string Epunjabid, string cadre, string desi, string IFSC, string Accno,string ExamMonth)
        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UPDATESTAFFDETAILS_Sp", con);//CreateClusterNew_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StaffID", staffid);
                    cmd.Parameters.AddWithValue("@TD", TD);
                    cmd.Parameters.AddWithValue("@fname", fname);
                    cmd.Parameters.AddWithValue("@Name", Name);
                    cmd.Parameters.AddWithValue("@expe", expe);
                    cmd.Parameters.AddWithValue("@Month", Month);

                    cmd.Parameters.AddWithValue("@gen", gen);
                    cmd.Parameters.AddWithValue("@mob", mob);
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@Schoolname", Schoolname);

                    cmd.Parameters.AddWithValue("@Aadharnum", Aadharnum);
                    cmd.Parameters.AddWithValue("@Epunjabid", Epunjabid);
                    cmd.Parameters.AddWithValue("@cadre", cadre);
                    cmd.Parameters.AddWithValue("@desi", desi);
                    cmd.Parameters.AddWithValue("@IFSC", IFSC);
                    cmd.Parameters.AddWithValue("@Accno", Accno);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);

                    con.Open();
                    //result = cmd.ExecuteNonQuery().ToString();
                    result = cmd.ExecuteScalar().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }
        public DataSet GetCADRE()
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCADRE_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetStates()
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStates_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet txtGETSchoolName(string SCHL)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStaffSCHLE_SP", con);
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
        public DataSet GetBulkStaffErrorList(string Elist, string dist)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetBulkStaffErrorList_sp", con); //GetCADRESUB
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Elist", Elist);
                    cmd.Parameters.AddWithValue("@dist", dist);
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
        public DataSet SelectAllDESIGCADREWISE(string CADRE)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCADRESUB", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CADRE", CADRE);
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
        public string DeleteStaffData(string Staffid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("DeleteStaffData_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Staffid", Staffid);
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
        public string DeleteClusterListData(string Corid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("DeleteClusterListData_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Corid", Corid);
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

        //-----------------------Add Center----------------
        public string DeleteCentreListData(string CENTID)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("DeleteCentreListData_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CENTID", CENTID);
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
        public DataSet Select_ADD_CenterListByUser(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Select_ADD_CenterListByUser_sp", con); //SelectCenterListByUser_sp
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
        public DataSet Select_ADD_CenterListByUserCount(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Select_ADD_CenterListByUserCount_sp", con); //SelectCenterListByUserCount_sp
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
        public string Update_CCODE_To_CENTRE(string CCODE, string CHKSCHOOL)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_CCODE_To_CENTRE_SP", con); ///Ins_CorrectionSub_Sp
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CCODE", CCODE);
                cmd.Parameters.AddWithValue("@CHKSCHOOL", CHKSCHOOL);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                if (con.State == ConnectionState.Open)
                    con.Close();

                return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                if (con != null)
                {
                    con.Dispose();
                    con.Close();
                    con = null;
                }
                return result = "";

            }
            finally
            {
                if (con != null)
                {
                    con.Dispose();
                    con.Close();
                    con = null;
                }
            }
        }
        public string FinalSubmitDeoPortal(string district)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("FinalSubmitDeoPortal_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@dist", district);
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
        public DataSet FillFinalPrintLot(string dist, int lot)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("FillFinalPrintLot_sp", con); //FinalSubmitDeoPortalGrid
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Dist", dist);
                    cmd.Parameters.AddWithValue("@userid", lot);
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
        //--------------------------------

        //---------------------------------------------Start Examinor--------------- 

        //AddExaminerDETAILS(cls, SelListItem, name, Fname, desi, Schoolcode, emailid, mobno, Quali, Adrs, hdist, hteh, pincode, texp, eexp, remark);
        public string AddExaminerDETAILS(string cls, string SelListItem, string name, string Fname, string desi, string Schoolcode, string emailid, string mobno, string Quali, string Adrs, string hdist, string hteh, string pincode, string texp, string eexp, string remark, int udid, string district, string subcode, string cadre, string DOR)
        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //Class,SubjectList,Name,fname,Designation,SchoolCode,EmailID,Mobile,Quali,HomeAddress,HomeDist,HomeTehsil,HomePinCode,Teachingexp
                    //Evaexp,Remarks,insdate,Updatedate
                    SqlCommand cmd = new SqlCommand("AddExaminerDETAILS_Sp", con);//ADDSTAFFDETAILS_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Class", cls);
                    cmd.Parameters.AddWithValue("@SubjectList", SelListItem);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@fname", Fname);
                    cmd.Parameters.AddWithValue("@Designation", desi);
                    cmd.Parameters.AddWithValue("@SchoolCode", Schoolcode);
                    cmd.Parameters.AddWithValue("@EmailID", emailid);
                    cmd.Parameters.AddWithValue("@Mobile", mobno);
                    cmd.Parameters.AddWithValue("@Quali", Quali);
                    cmd.Parameters.AddWithValue("@HomeAddress", Adrs);
                    cmd.Parameters.AddWithValue("@HomeDist", hdist);
                    cmd.Parameters.AddWithValue("@HomeTehsil", hteh);
                    cmd.Parameters.AddWithValue("@HomePinCode", pincode);
                    cmd.Parameters.AddWithValue("@Teachingexp", texp);
                    cmd.Parameters.AddWithValue("@Evaexp", eexp);
                    //cmd.Parameters.AddWithValue("@Remarks", remark);
                    cmd.Parameters.AddWithValue("@udid", udid);
                    cmd.Parameters.AddWithValue("@district", district);
                    cmd.Parameters.AddWithValue("@subcode", subcode);
                    cmd.Parameters.AddWithValue("@cadre", cadre);
                    cmd.Parameters.AddWithValue("@DOR", DOR);
                    cmd.Parameters.AddWithValue("@remark", remark);

                    con.Open();
                    result = cmd.ExecuteNonQuery().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }
        public string UpdateExaminerDETAILS(int id, string cls, string SelListItem, string name, string Fname, string desi, string Schoolcode, string emailid, string mobno, string Quali, string Adrs, string hdist, string hteh, string pincode, string texp, string eexp, string remark, string subcode, string cadre, string DOR)
        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //Class,SubjectList,Name,fname,Designation,SchoolCode,EmailID,Mobile,Quali,HomeAddress,HomeDist,HomeTehsil,HomePinCode,Teachingexp
                    //Evaexp,Remarks,insdate,Updatedate
                    SqlCommand cmd = new SqlCommand("UpdateExaminerDETAILS_Sp", con);//AddExaminerDETAILS_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Exid", id);
                    cmd.Parameters.AddWithValue("@Class", cls);
                    cmd.Parameters.AddWithValue("@SubjectList", SelListItem);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@fname", Fname);
                    cmd.Parameters.AddWithValue("@Designation", desi);
                    cmd.Parameters.AddWithValue("@SchoolCode", Schoolcode);
                    cmd.Parameters.AddWithValue("@EmailID", emailid);
                    cmd.Parameters.AddWithValue("@Mobile", mobno);
                    cmd.Parameters.AddWithValue("@Quali", Quali);
                    cmd.Parameters.AddWithValue("@HomeAddress", Adrs);
                    cmd.Parameters.AddWithValue("@HomeDist", hdist);
                    cmd.Parameters.AddWithValue("@HomeTehsil", hteh);
                    cmd.Parameters.AddWithValue("@HomePinCode", pincode);
                    cmd.Parameters.AddWithValue("@Teachingexp", texp);
                    cmd.Parameters.AddWithValue("@Evaexp", eexp);
                    cmd.Parameters.AddWithValue("@Remarks", remark);
                    //cmd.Parameters.AddWithValue("@udid", udid);
                    //cmd.Parameters.AddWithValue("@district", district);
                    cmd.Parameters.AddWithValue("@subcode", subcode);
                    cmd.Parameters.AddWithValue("@cadre", cadre);
                    cmd.Parameters.AddWithValue("@DOR", DOR);

                    con.Open();
                    result = cmd.ExecuteNonQuery().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }
        public string DeleteExaminerData(string ExaminerDataid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("DeleteExaminerData_Sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ExaminerDataid", ExaminerDataid);
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
        public DataSet SelectlastEntryExaminer(string district)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("GetLastEntryOfExaminor", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@district", district);
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
        public DataSet SelectEntryExaminer(int Examinerid)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("GetEntryOfExaminor", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Examinerid", Examinerid);
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
        public DataSet GoForFinalSubmitExaminer(string district)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("GoForFinalSubmitExaminer_Sp", con);//GetLastEntryOfExaminor
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@district", district);
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
        public DataSet ViewAllExaminerDistwise(string district)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("ViewAllExaminerDistwise_sp", con);//GetLastEntryOfExaminor
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@district", district);
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
        public DataSet ViewAllExaminer(string search, string district)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("SearchViewAllExaminer_sp", con);//GetLastEntryOfExaminor
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", search);
                    cmd.Parameters.AddWithValue("@district", district);
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
        public DataSet GetSchool()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAllSchool", con);
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
        public DataSet GetLassWiseSubjects(string Classes)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetLassWiseSubjects_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Classes", Classes);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SelectDist()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAllDistrict", con);
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
        public DataSet GetSubDesi()
        {

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetSubDesi_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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
        public string FinalSubmitExaminer(string district, int uid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("FinalSubmitExaminer_Sp", con); //FinalSubmitDeoPortal_Sp
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@dist", district);
                cmd.Parameters.AddWithValue("@uid", uid);
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
                //con.Close();
                //con.Dispose();
                con = null;
            }
        }
        public DataSet ExaminerReport(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ExaminerReport", con); //GetAll10thPass
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

        //-------------------------------------------------------End Examinor------------//
        //------------------------------------Start Profile---------------------------
        public DataSet GetDesignation()
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetDesignation_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string UPDATEDEOPROFILE(string DeoUser, string district, string name, string Email, string mob, string std, string phone, string op1, string op2, string op3, string op4, string op5, string Pdesi1, string Pdesi2, string Pdesi3, string Pdesi4, string Pdesi5, string mob1, string mob2, string mob3, string mob4, string mob5)
        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UPDATEDEOPROFILE_Sp", con);//CreateClusterNew_Sp
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@DeoUser", DeoUser);
                    cmd.Parameters.AddWithValue("@district", district);
                    cmd.Parameters.AddWithValue("@name", name);

                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@mob", mob);
                    cmd.Parameters.AddWithValue("@std", std);
                    cmd.Parameters.AddWithValue("@phone", phone);

                    cmd.Parameters.AddWithValue("@op1", op1);
                    cmd.Parameters.AddWithValue("@op2", op2);
                    cmd.Parameters.AddWithValue("@op3", op3);
                    cmd.Parameters.AddWithValue("@op4", op4);
                    cmd.Parameters.AddWithValue("@op5", op5);

                    cmd.Parameters.AddWithValue("@Pdesi1", Pdesi1);
                    cmd.Parameters.AddWithValue("@Pdesi2", Pdesi2);
                    cmd.Parameters.AddWithValue("@Pdesi3", Pdesi3);
                    cmd.Parameters.AddWithValue("@Pdesi4", Pdesi4);
                    cmd.Parameters.AddWithValue("@Pdesi5", Pdesi5);

                    cmd.Parameters.AddWithValue("@mob1", mob1);
                    cmd.Parameters.AddWithValue("@mob2", mob2);
                    cmd.Parameters.AddWithValue("@mob3", mob3);
                    cmd.Parameters.AddWithValue("@mob4", mob4);
                    cmd.Parameters.AddWithValue("@mob5", mob5);

                    con.Open();
                    result = cmd.ExecuteNonQuery().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }
        public DataSet CHKDeoFlag(string DU, string Dist)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CHKDeoFlag_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DU", DU);
                    cmd.Parameters.AddWithValue("@Dist", Dist);
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
        //---------------------------Change Password-----------------------
        public int DeoChangePassword(string Dist, string User, string CurrentPassword, string NewPassword)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DeoChangePassword_SP", con); //SchoolChangePasswordSP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Dist", Dist);
                    cmd.Parameters.AddWithValue("@User", User);
                    cmd.Parameters.AddWithValue("@CurrentPassword", CurrentPassword);
                    cmd.Parameters.AddWithValue("@NewPassword", NewPassword);
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

        //-----------------------------------End Profile------------------------------
        //---------------------------------------------------------Forgot password-----------------//
        public DataSet GetEmailForgotpasswordDeoportal(string Deoid)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetEmailForgotpasswordDeoportal_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Deoid", Deoid);
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
        //-------------------------------------------------End----------------------------//

        //---------------------Admin Report--------------------//
        public DataSet DeoSummaryReport(string ExamMonth)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //  SqlCommand cmd = new SqlCommand("DeoSummaryReport", con);
                    SqlCommand cmd = new SqlCommand("DeoSummaryReport_WithExamMonth", con); 
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
                    //cmd.Parameters.AddWithValue("@ses", sesssion);
                    //cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    //cmd.Parameters.AddWithValue("@PageSize", 10);

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
        public DataSet ExaminerSummaryReport()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SummaryReportExaminer", con); //DeoSummaryReport
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
        //---------------------------------//
        //private void UpdateExcel(string sheetName, int row, int col, string data, string FN)
        //{
        //    Microsoft.Office.Interop.Excel.Application oXL = null;
        //    Microsoft.Office.Interop.Excel._Workbook oWB = null;
        //    Microsoft.Office.Interop.Excel._Worksheet oSheet = null;
        //    try
        //    {
        //        string UpdateFilePath = System.Configuration.ConfigurationManager.AppSettings["StaffFile"];
        //        oXL = new Microsoft.Office.Interop.Excel.Application();

        //        string fl = UpdateFilePath + FN + ".xls";
        //        oWB = oXL.Workbooks.Open(fl);
        //        oSheet = String.IsNullOrEmpty(sheetName) ? (Microsoft.Office.Interop.Excel._Worksheet)oWB.ActiveSheet : 
        //            (Microsoft.Office.Interop.Excel._Worksheet)oWB.Worksheets[sheetName];

        //        oSheet.Cells[row, col] = data;
        //        oWB.Save();               
        //    }
        //    catch (Exception ex)
        //    {
        //        oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName("OpenExcel"));

        //    }
        //    finally
        //    {
        //        if (oWB != null)
        //            oWB.Close();
        //    }
        //}
        //-------------------------------------------ChekExcelSheet-------------------------
        public string CheckMisExcel(DataSet ds, string MIS_FILENM)
        {
            string ExcelResult = "";
            string Result = "";
            string fname = "";
            int CountError = 0;
            try
            {

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ExcelResult = "";
                    if (ds.Tables[0].Rows[i][0].ToString().Length < 1 || ds.Tables[0].Rows[i][0].ToString() == "")
                    {

                        int RowNo = i + 2;
                        string DC = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Duty Code " + DC + " of in row " + RowNo + ",  ";
                        ExcelResult += "Duty Code " + ",";
                        fname = MIS_FILENM;
                        ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                        CountError++;
                        //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);

                    }
                    if (ds.Tables[0].Rows[i][0].ToString() != "")
                    {
                        if (ds.Tables[0].Rows[i][0].ToString() != "1" && ds.Tables[0].Rows[i][0].ToString() != "2" && ds.Tables[0].Rows[i][0].ToString() != "3")
                        {
                            int RowNo = i + 2;
                            string DC = ds.Tables[0].Rows[i][0].ToString();
                            Result += "Duty Code " + DC + " of in row " + RowNo + ",  ";
                            ExcelResult += "Duty Code Should be(1 Or 2 Or 3) " + ",";
                            ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                            CountError++;
                            //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                        }

                    }

                    if (ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string pid = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Punjab ID " + pid + " in row " + RowNo + ",  ";
                        ExcelResult += "Punjab ID " + ",";
                        fname = MIS_FILENM;
                        ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                        CountError++;
                        // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                        //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                    }
                    if (ds.Tables[0].Rows[i][2].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string AAdhar = ds.Tables[0].Rows[i][2].ToString();
                        Result += "Aadhar " + AAdhar + " in row " + RowNo + ",  ";
                        ExcelResult += "Aadhar Num " + ",";
                        fname = MIS_FILENM;
                        ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                        CountError++;
                    }
                    if (ds.Tables[0].Rows[i][2].ToString() != "")
                    {
                        if ((ds.Tables[0].Rows[i][2].ToString().Length < 12) || (ds.Tables[0].Rows[i][2].ToString().Length > 12))
                        {
                            int RowNo = i + 2;
                            string AAdhar = ds.Tables[0].Rows[i][2].ToString();
                            Result += "Aadhar " + AAdhar + " in row " + RowNo + ",  ";
                            ExcelResult += "Aadhar Num Should Be 12 Digit " + ",";
                            fname = MIS_FILENM;
                            ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                            CountError++;
                        }

                    }
                    if (ds.Tables[0].Rows[i][3].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string Tn = ds.Tables[0].Rows[i][3].ToString();
                        Result += "Teacher's name " + Tn + " in row " + RowNo + ",  ";
                        ExcelResult += "Teacher's name " + ",";
                        fname = MIS_FILENM;
                        ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                        CountError++;

                        // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                        // UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                    }
                    //if (ds.Tables[0].Rows[i][3].ToString() != "")
                    //{
                    //    System.Text.RegularExpressions.Regex regex = null;
                    //    regex = new System.Text.RegularExpressions.Regex("/^[a-zA-Z ]*$/");                       
                    //    if (!regex.IsMatch(ds.Tables[0].Rows[i][3].ToString()))
                    //    {
                    //        int RowNo = i + 2;
                    //        string Tn = ds.Tables[0].Rows[i][3].ToString();
                    //        Result += "Teacher's name " + Tn + " in row " + RowNo + ",  ";
                    //        ExcelResult += "Teacher's name Should Be In letter " + ",";
                    //        fname = MIS_FILENM;
                    //        ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                    //        CountError++;
                    //    }

                    //}
                    if (ds.Tables[0].Rows[i][4].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string Fn = ds.Tables[0].Rows[i][4].ToString();
                        Result += " Teacher Father's name " + Fn + "in row " + RowNo + ",  ";
                        ExcelResult += "Father's name " + ",";
                        ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                        CountError++;
                        // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                        //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                    }
                    //if (ds.Tables[0].Rows[i][4].ToString() != "")
                    //{
                    //    System.Text.RegularExpressions.Regex regex = null;
                    //    regex = new System.Text.RegularExpressions.Regex("/^[a-zA-Z ]*$/");
                    //    if (!regex.IsMatch(ds.Tables[0].Rows[i][4].ToString()))
                    //    { 
                    //        int RowNo = i + 2;
                    //    string Fn = ds.Tables[0].Rows[i][4].ToString();
                    //    Result += " Teacher Father's name " + Fn + "in row " + RowNo + ",  ";
                    //    ExcelResult += "Father's name Should Be In letter " + ",";
                    //    ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                    //    CountError++;
                    //    }
                    //}
                    if (ds.Tables[0].Rows[i][5].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string schl = ds.Tables[0].Rows[i][5].ToString();
                        Result += "School Code(Alloted By PSEB)  " + schl + " in row " + RowNo + ",  ";
                        ExcelResult += "School Code(Alloted By PSEB)" + ",";
                        fname = MIS_FILENM;
                        ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                        CountError++;
                        // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                        //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                    }
                    if (ds.Tables[0].Rows[i][5].ToString() != "")
                    {

                        //if((Convert.ToInt32(ds.Tables[0].Rows[i][5].ToString())> 7) || (Convert.ToInt32(ds.Tables[0].Rows[i][5].ToString()) < 7))
                        if ((ds.Tables[0].Rows[i][5].ToString().Length > 7) || (ds.Tables[0].Rows[i][5].ToString().Length < 7))
                        {
                            int RowNo = i + 2;
                            string schl = ds.Tables[0].Rows[i][5].ToString();
                            Result += "School Code(Alloted By PSEB)  " + schl + " in row " + RowNo + ",  ";
                            ExcelResult += "School Code should be 7 Digit" + ",";
                            fname = MIS_FILENM;
                            ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                            CountError++;
                            // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                            //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                        }
                        string resSchl = GetSCHLFromSchoolmaster(ds.Tables[0].Rows[i][5].ToString());
                        if ((resSchl == null) || (resSchl == ""))
                        {
                            int RowNo = i + 2;
                            string schl = ds.Tables[0].Rows[i][5].ToString();
                            Result += "School Code(Alloted By PSEB)  " + schl + " in row " + RowNo + ",  ";
                            ExcelResult += "School Code Does Not Exist" + ",";
                            fname = MIS_FILENM;
                            ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                            CountError++;
                        }

                    }
                    if (ds.Tables[0].Rows[i][6].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string Cadre = ds.Tables[0].Rows[i][6].ToString();
                        Result += " Cadre " + Cadre + " of  in row " + RowNo + ",  ";
                        ExcelResult += " Cadre " + ",";
                        fname = MIS_FILENM;
                        ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                        CountError++;
                        // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                        //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                    }
                    if (ds.Tables[0].Rows[i][6].ToString() != "")
                    {
                        // string resCadre = GetCADREFromExcel("Computer Faculty12");
                        string resCadre = GetCADREFromExcel(ds.Tables[0].Rows[i][6].ToString());
                        if ((resCadre == null) || (resCadre == ""))
                        {
                            int RowNo = i + 2;
                            string Cadre = ds.Tables[0].Rows[i][6].ToString();
                            Result += " Cadre " + Cadre + " of  in row " + RowNo + ",  ";
                            ExcelResult += " Fill Cadre Refer to Sheet2 in Given Excel Sheet " + ",";
                            fname = MIS_FILENM;
                            ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                            CountError++;
                            // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                            //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                        }

                    }
                    if (ds.Tables[0].Rows[i][7].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string Desig = ds.Tables[0].Rows[i][7].ToString();
                        Result += " Designation " + Desig + " of  in row " + RowNo + ",  ";
                        ExcelResult += "Fill Designation Against Cadre Refer to Sheet2 in Given Excel Sheet  " + ",  ";
                        fname = MIS_FILENM;
                        ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                        CountError++;
                        // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                        //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                    }
                    if (ds.Tables[0].Rows[i][7].ToString() != "")
                    {
                        string resDegi = GetCADREDesigFromExcel(ds.Tables[0].Rows[i][6].ToString(), ds.Tables[0].Rows[i][7].ToString());
                        if ((resDegi == null) || (resDegi == ""))
                        {
                            int RowNo = i + 2;
                            string Desig = ds.Tables[0].Rows[i][7].ToString();
                            Result += " Designation " + Desig + " of  in row " + RowNo + ",  ";
                            ExcelResult += "Designation " + ",  ";
                            fname = MIS_FILENM;
                            ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                            CountError++;
                            // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                            //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                        }

                    }
                    if (ds.Tables[0].Rows[i][8].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string mob = ds.Tables[0].Rows[i][8].ToString();
                        Result += "Mobile " + mob + " of  in row " + RowNo + ",  ";
                        ExcelResult += "Mobile " + ",  ";
                        fname = MIS_FILENM;
                        ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                        CountError++;
                        // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                        //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                    }
                    if (ds.Tables[0].Rows[i][8].ToString() != "")
                    {
                        if ((ds.Tables[0].Rows[i][8].ToString().Length > 10) || (ds.Tables[0].Rows[i][8].ToString().Length < 10))
                        {
                            int RowNo = i + 2;
                            string mob = ds.Tables[0].Rows[i][8].ToString();
                            Result += "Mobile " + mob + " of  in row " + RowNo + ",  ";
                            ExcelResult += "Mobile should be 10 digit " + ",  ";
                            fname = MIS_FILENM;
                            ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                            CountError++;
                            // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                            //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                        }

                    }
                    if (ds.Tables[0].Rows[i][9].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string gen = ds.Tables[0].Rows[i][9].ToString();
                        Result += " Gender " + gen + " of  in row " + RowNo + ",  ";
                        ExcelResult += " Gender " + ",  ";
                        fname = MIS_FILENM;
                        ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                        CountError++;
                        // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                        //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                    }
                    if (ds.Tables[0].Rows[i][9].ToString() != "")
                    {
                        if ((ds.Tables[0].Rows[i][9].ToString() != "male" && ds.Tables[0].Rows[i][9].ToString() != "Male") && (ds.Tables[0].Rows[i][9].ToString() != "female" && ds.Tables[0].Rows[i][9].ToString() != "Female"))
                        {
                            int RowNo = i + 2;
                            string gen = ds.Tables[0].Rows[i][9].ToString();
                            Result += " Gender " + gen + " of  in row " + RowNo + ",  ";
                            ExcelResult += " Gender Should Be Male Or Female " + ",  ";
                            fname = MIS_FILENM;
                            ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                            CountError++;
                            // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                            //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                        }

                    }
                    if (ds.Tables[0].Rows[i][10].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string yr = ds.Tables[0].Rows[i][10].ToString();
                        Result += " Experince year " + yr + " of  in row " + RowNo + ",  ";
                        ExcelResult += " Experince year " + ",  ";
                        fname = MIS_FILENM;
                        ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                        CountError++;
                        // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                        //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                    }
                    if (ds.Tables[0].Rows[i][10].ToString() != "")
                    {
                        int year;
                        bool isYear = Int32.TryParse(ds.Tables[0].Rows[i][10].ToString(), out year);
                        if (isYear)
                        {
                            if (((ds.Tables[0].Rows[i][10].ToString().Length) < 0) || ((ds.Tables[0].Rows[i][10].ToString().Length) > 2))
                            {
                                int RowNo = i + 2;
                                string yr = ds.Tables[0].Rows[i][10].ToString();
                                Result += " Experince year " + yr + " of  in row " + RowNo + ",  ";
                                ExcelResult += " Experince year  Should be less then two Digit" + ",  ";
                                fname = MIS_FILENM;
                                ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                                CountError++;
                                // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                                //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                            }
                        }


                    }
                    if (ds.Tables[0].Rows[i][11].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string mon = ds.Tables[0].Rows[i][11].ToString();
                        Result += " Experince Month " + mon + " in row " + RowNo + ",  ";
                        ExcelResult += " Experince Month " + ",  ";
                        fname = MIS_FILENM;
                        ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                        CountError++;
                        // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                        //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                    }
                    if (ds.Tables[0].Rows[i][11].ToString() != "")
                    {
                        // int a = Convert.ToInt32(ds.Tables[0].Rows[i][11].ToString());
                        int num;
                        bool isNum = Int32.TryParse(ds.Tables[0].Rows[i][11].ToString(), out num);
                        if (isNum)
                        {
                            //Is a Number
                            if (((ds.Tables[0].Rows[i][11].ToString().Length) < 0) || ((ds.Tables[0].Rows[i][11].ToString().Length) > 2))
                            {
                                int RowNo = i + 2;
                                string mon = ds.Tables[0].Rows[i][11].ToString();
                                Result += " Experince Month " + mon + " in row " + RowNo + ",  ";
                                ExcelResult += " Experince Month Should be less then two Digit " + ",  ";
                                fname = MIS_FILENM;
                                ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                                CountError++;
                                // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                                //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                            }
                        }
                        if (!isNum)
                        {
                            int RowNo = i + 2;
                            string mon = ds.Tables[0].Rows[i][11].ToString();
                            Result += " Experince Month " + mon + " in row " + RowNo + ",  ";
                            ExcelResult += " Experince Month Should be Numeric " + ",  ";
                            fname = MIS_FILENM;
                            ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                            CountError++;
                            // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                            //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                        }



                    }
                    if (ds.Tables[0].Rows[i][12].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string IFSC = ds.Tables[0].Rows[i][12].ToString();
                        Result += " IFSC Code " + IFSC + " in row " + RowNo + ",  ";
                        ExcelResult += " IFSC " + ",";
                        fname = MIS_FILENM;
                        ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                        CountError++;
                        // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                        //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                    }

                    //if (ds.Tables[0].Rows[i][13].ToString() == "")
                    //{
                    //    int RowNo = i + 2;
                    //    string ACCno = ds.Tables[0].Rows[i][13].ToString();
                    //    Result += " Bank Accno. " + ACCno + " in row " + RowNo + ",  ";
                    //    ExcelResult += " Bank Accno. " + ",";
                    //    fname = MIS_FILENM;
                    //    // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                    //    UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);
                    //}
                    if (ds.Tables[0].Rows[i][13].ToString().Length < 5)
                    {

                        int RowNo = i + 2;
                        string ACCno = ds.Tables[0].Rows[i][13].ToString();
                        Result += " Bank Accno. " + ACCno + " in row " + RowNo + ",  ";
                        ExcelResult += " Bank Accno. " + ",";
                        fname = MIS_FILENM;
                        ds.Tables[0].Rows[i]["Error List"] = ExcelResult;
                        CountError++;
                        // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                        //UpdateExcel("Staff Performa", RowNo, 15, ExcelResult, MIS_FILENM);

                    }
                }

                //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //{
                //    if (ds.Tables[0].Rows[i][1].ToString() == "")
                //    {
                //        int RowNo = i + 2;
                //        string pid = ds.Tables[0].Rows[i][1].ToString();
                //        //Result += "Punjab ID " + pid + " in row " + RowNo + ",  ";
                //        Result = "Punjab ID " + ",";
                //        fname = MIS_FILENM;
                //        // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "InvalidArgs", "alert('Please enter Employee ID in row " + RowNo + "');", true);
                //        UpdateExcel("Staff Performa", RowNo, 15, Result, MIS_FILENM);
                //    }
                //}

            }
            catch (Exception ex)
            {
                Result = ex.Message;
                //return "Please Check All Fields";
            }
            if (CountError > 0)
            {
                DataTable dt = ds.Tables[0];
                //export datatable to excel
                string UpdateFilePath = System.Configuration.ConfigurationManager.AppSettings["StaffFile"];
                using (XLWorkbook wb = new XLWorkbook())
                {
                    var ws = wb.Worksheets.Add(dt, "StaffUpload");// Customers
                    ws.Tables.FirstOrDefault().ShowAutoFilter = false;
                    wb.SaveAs(UpdateFilePath + fname + "copy.xlsx");
                }
            }
            insertStaffErrorFromExcel(Result, fname);
            return Result;
        }
        public void insertStaffErrorFromExcel(string ErrorList, string MIS_FILENM)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try

            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("insertStaffErrorFromExcel_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ErrorList", ErrorList);
                    cmd.Parameters.AddWithValue("@MIS_FILENM", MIS_FILENM);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();

                }
            }
            catch (Exception ex)
            {

            }
        }
        public string GetSCHLFromSchoolmaster(string Schl)
        {
            string Schlresult = "";
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetSCHLFromSchoolmaster_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Schl", Schl);
                    con.Open();
                    Schlresult = cmd.ExecuteScalar().ToString();
                    //int aa=   cmd.ExecuteNonQuery();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return Schlresult;
                }

            }
            catch (Exception ex)
            {
                return Schlresult = null;
            }
        }
        public string GetCADREFromExcel(string cad)
        {
            string CADREresult = "";
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCADREFromExcel_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cad", cad);
                    con.Open();
                    CADREresult = cmd.ExecuteScalar().ToString();
                    //int aa=   cmd.ExecuteNonQuery();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return CADREresult;
                }

            }
            catch (Exception ex)
            {
                return CADREresult = null;
            }
        }
        public string GetCADREDesigFromExcel(string cad, string Desig)
        {
            string CADREDesigresult = "";
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCADREDesigFromExcel_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cad", cad);
                    cmd.Parameters.AddWithValue("@Desig", Desig);
                    con.Open();
                    CADREDesigresult = cmd.ExecuteScalar().ToString();
                    //int aa=   cmd.ExecuteNonQuery();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return CADREDesigresult;
                }

            }
            catch (Exception ex)
            {
                return CADREDesigresult = null;
            }
        }
        public string UploadBulkSTAFFDETAILS(string uid, string dist, string CCODE, string TD, string fname, string Name, string expe, string Month, string gen, string mob, string SCHL, string Schoolname, string Aadharnum, string Epunjabid, string cadre, string desi, string IFSC, string Accno)
        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UploadBulkSTAFFDETAILS_Sp", con);//ADDSTAFFDETAILS_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@uid", uid);
                    cmd.Parameters.AddWithValue("@dist", dist);
                    cmd.Parameters.AddWithValue("@ccode", CCODE);
                    cmd.Parameters.AddWithValue("@TD", TD);
                    cmd.Parameters.AddWithValue("@fname", fname);
                    cmd.Parameters.AddWithValue("@Name", Name);
                    cmd.Parameters.AddWithValue("@expe", expe);
                    cmd.Parameters.AddWithValue("@Month", Month);
                    cmd.Parameters.AddWithValue("@gen", gen);
                    cmd.Parameters.AddWithValue("@mob", mob);
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@Schoolname", Schoolname);

                    cmd.Parameters.AddWithValue("@Aadharnum", Aadharnum);
                    cmd.Parameters.AddWithValue("@Epunjabid", Epunjabid);
                    cmd.Parameters.AddWithValue("@cadre", cadre);
                    cmd.Parameters.AddWithValue("@desi", desi);
                    cmd.Parameters.AddWithValue("@IFSC", IFSC);
                    cmd.Parameters.AddWithValue("@Accno", Accno);
                    //cmd.Parameters.AddWithValue("@DOR", DOR);
                    //cmd.Parameters.AddWithValue("@Remark", Remark);

                    con.Open();
                    result = cmd.ExecuteNonQuery().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }
        //-----------------------------------------End ChkExcel---------------------------------
        //---------------------------------------------------------Mail Centre Not Added-----------------//
        public DataSet GetEmailMailCentreNotAdded(string Distid)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetEmailMailCentreNotAdded_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Dist", Distid);
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
        //-------------------------------------------------End----------------------------//



        //-------------------------------------Admin view Staff--------------------//

        public DataSet AdminViewStaff(string search, string sesssion, int pageIndex,string ExamMonth)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AdminViewStaffSP", con); //GetSTAFFAllDistStaffAdmin_Sp//GetSTAFFAllDistStaffAdmin_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 20);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }


        public DataSet GetSTAFFAllDistStaffAdmin(string search, string sesssion, int pageIndex, string ExamMonth)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetSTAFFAllDistStaffReplaceAdmin_Sp", con); //GetSTAFFAllDistStaffAdmin_Sp//GetSTAFFAllDistStaffAdmin_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 20);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        public DataSet DeoStaffReplaceByAdmin(string MainEpunID, string RplcEpunID, string DeoUser, string resultlist, string remarks, string ExamMonth)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DeoStaffReplaceByAdmin_Sp", con); //GetSTAFFAllDistStaffAdmin_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MainEpunID", MainEpunID);
                    cmd.Parameters.AddWithValue("@RplcEpunID", RplcEpunID);
                    cmd.Parameters.AddWithValue("@DeoUser", DeoUser);
                    cmd.Parameters.AddWithValue("@TStatus", resultlist);
                    cmd.Parameters.AddWithValue("@remarks", remarks);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet DeoStaffReplaceData(string dist, string DeoUser, string ExamMonth)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DeoStaffReplaceData_Sp", con); //GetSTAFFAllDistStaffAdmin_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Dist", dist);
                    cmd.Parameters.AddWithValue("@DeoUser", DeoUser);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet DeoStaffReplaceDataDownload(string dist, string DeoUser, string ExamMonth)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DeoStaffReplaceDataDownload_Sp", con); //GetSTAFFAllDistStaffAdmin_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Dist", dist);
                    cmd.Parameters.AddWithValue("@DeoUser", DeoUser);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet DeoNewAppointmentLetter(string id, string ExamMonth)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DeoNewAppointmentLetter_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        //--------------------------End---------------------------------//

        //--------------------Center Wise StaffList Report---------------//
        public DataSet SupervisoryStaffList(string Dist)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SupervisoryStaffList_Sp", con); //GetAll10thPass
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@dist", Dist);
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
        //---------------------------------------End------//
        //--------------------StaffLetter---------------//
        public DataSet StaffLetterList(string Search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("StaffLetter_Sp", con); //GetAll10thPass
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", Search);
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
        //---------------------------------------End------//
        //------------------------Admin Staff-----------------------//
        public DataSet GetCentreSTAFFWise(string CCODE)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCentreSTAFFWise_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ccode", CCODE);
                    //cmd.Parameters.AddWithValue("@dist", dist);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string Update_Centre_To_StaffShift(string cent, int uid, string CHKStaffID, string updcentdate)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Update_Centre_To_StaffShift_sp", con); ///Update_CCODE_To_CENTRE_SP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@cent", cent);
                cmd.Parameters.AddWithValue("@uid", uid);
                cmd.Parameters.AddWithValue("@CHKStaffID", CHKStaffID);
                cmd.Parameters.AddWithValue("@centreDate", updcentdate);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                if (con.State == ConnectionState.Open)
                    con.Close();

                return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                if (con != null)
                {
                    con.Dispose();
                    con.Close();
                    con = null;
                }
                return result = "";

            }
            finally
            {
                if (con != null)
                {

                    con = null;
                }
            }
        }
        public DataSet GetAdminSTAFFDetails(string search, string sesssion, int pageIndex)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAdminSTAFFDetails_Sp", con); //GetSTAFFAllDistStaffAdmin_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@ses", sesssion);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 20);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SelectAllClusterDistWISE(string Dist)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectAllClusterDistWISE_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Dist", Dist);
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
        public DataSet ShowAllCluster()
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ShowAllCluster_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        //---------------------------End-----------------------------//
        //-------------------Replacement Staff--------------------------//
        public DataSet ViewReplacementStaff(string staffid, string district, int uid,string ExamMonth)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("ViewReplacementStaff_sp", con);//GetLastEntryOfExaminor
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@staffid", staffid);
                    cmd.Parameters.AddWithValue("@district", district);
                    cmd.Parameters.AddWithValue("@uid", uid);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
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
        public DataSet ViewReplacementALLShiftStaff(string district, int pageIndex, int udid,string ExamMonth)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("ViewReplacementALLShiftStaff_sp", con);//GetLastEntryOfExaminor
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@uid", udid);
                    cmd.Parameters.AddWithValue("@district", district);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 20);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
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
        //------------------------------End--------------------------------//
        //---------------------Insert ShiftStaff---------------------// oldstffid, district, newStaffid
        public string Ins_Shift_Staff(string ostffid, string dist, string nStaffid, string cendate, int udid, string Reason)
        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Ins_Shift_Staff_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Ostaffid", ostffid);
                    cmd.Parameters.AddWithValue("@district", dist);
                    cmd.Parameters.AddWithValue("@newStaffid", nStaffid);
                    cmd.Parameters.AddWithValue("@cendate", cendate);
                    cmd.Parameters.AddWithValue("@uid", udid);
                    cmd.Parameters.AddWithValue("@reason", Reason);

                    //Add the output parameter to the command object
                    SqlParameter outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = "@Error";
                    outPutParameter.SqlDbType = System.Data.SqlDbType.Int;
                    outPutParameter.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(outPutParameter);

                    con.Open();
                    result = cmd.ExecuteNonQuery().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }
        //------------------------------End-----------------------//
        //-------------------Deo Letter--------------------------//
        public DataSet DeoLetter(string district, string sid,string ExamMonth)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DeoLetter_sp", con);//GetLastEntryOfExaminor
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@dist", district);
                    cmd.Parameters.AddWithValue("@sid", sid);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
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
        //------------------------------End--------------------------------//

        //-------------------Extra Staff---------------------------------//
        public DataSet SelectClusterWiseExtraStaffListReport(string search, string sesssion, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectClusterWiseExtraStaffListReport", con); //GetAll10thPass
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
        //-------------------------End---------------------------------//

        public DataSet sendSmsToschl()  // 
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SendSMSTOSchl", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        //----------------------------------End-------------------------
        //---------------------AdminStaff---------------------
        public DataSet GetAdminSTAFFUserWise(string uid)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAdminSTAFFUserWise", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UseriD", uid);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        //----------------End---------------------------------
        //---------Exam Center--------------------
        public DataSet CentreForExam(string Schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CentreForExam_Sp", con);//GetLastEntryOfExaminor
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Schl", Schl);
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
        //-----------------End----------------------
        //------------- IMPORT DATA------------------------------//
        public DataSet GetALLDIST()
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetALLDIST_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@ccode", CCODE);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SelectBlock(string DIST)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetALLBLOCK_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DIST", DIST);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet Select_CLUSTER_NAME(string BLOCK)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetALLCluster_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BLOCK", BLOCK);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet SEARCHSTAFFDATA(string sstring, int pageIndex)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SEARCHSTAFFDATA_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@sstring", sstring);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string UpdateClusterToStaff(string dcode, string CCODE, string CHKStaffID)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("UpdateClusterToStaff_sp", con); ///Update_CCODE_To_CENTRE_SP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@dcode", dcode);
                cmd.Parameters.AddWithValue("@CCODE", CCODE);
                cmd.Parameters.AddWithValue("@CHKStaffID", CHKStaffID);

                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                if (con.State == ConnectionState.Open)
                    con.Close();

                return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                if (con != null)
                {
                    con.Dispose();
                    con.Close();
                    con = null;
                }
                return result = "";

            }
            finally
            {
                if (con != null)
                {

                    con = null;
                }
            }
        }


        public DataSet AdminGetALLSCHL()
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AdminGetALLSCHL_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        #region DEO SCHIOOL Data Download
        public DataSet DownloadDeoSchoolDistWise(string DIST)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            SqlCommand cmd;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    cmd = new SqlCommand("DISTRICTWISESCHOOLDETAILS_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DIST", DIST);
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    return dataTable;

                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion DEO SCHIOOL Data Download
        public DataSet GetDeoDISTSCHL(string DeologinSchl)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetDeoDISTSCHL_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", DeologinSchl);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        #region begin Admin Result Update MIS
        public string CheckResultMisExcel(DataTable dt, string userNM, out DataTable dt1)
        {
            string Result = "";
            try
            {
                dt1 = dt;
                dt1.Columns.Add(new DataColumn("Status", typeof(string)));
                string itemFirm = "";
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    {
                        //itemFirm = "SAI,PERF";
                        //if (!AbstractLayer.StaticDB.ContainsValue(itemFirm.ToUpper(), dt.Rows[i][0].ToString().ToUpper()))
                        //{
                        //    int RowNo = i + 2;
                        //    string FirmNM = dt.Rows[i][0].ToString().ToUpper();
                        //    Result += "Please check FIRM " + FirmNM + " in row " + RowNo + ",  ";
                        //    dt1.Rows[i]["Status"] += "Please check FIRM " + FirmNM + " in row " + RowNo + ",  ";
                        //}
                        //if (dt.Rows[i][12].ToString() == "")
                        //{
                        //    int RowNo = i + 2;
                        //    string ID = dt.Rows[i][12].ToString();
                        //    Result += "Please check ID " + ID + " in row " + RowNo + ",  ";
                        //    dt1.Rows[i]["Status"] += "Please check ID " + ID + " in row " + RowNo + ",  ";
                        //}

                        //if (dt.Rows[i][13].ToString() == "")
                        //{
                        //    int RowNo = i + 2;
                        //    string Roll = dt.Rows[i][13].ToString();
                        //    Result += "Please check Roll " + Roll + " in row " + RowNo + ",  ";
                        //    dt1.Rows[i]["Status"] += "Please check Roll " + Roll + " in row " + RowNo + ",  ";
                        //}
                    }
                }

            }
            catch (Exception ex)
            {
                dt1 = null;
                Result = ex.Message;
                //return "Please Check All Fields";
            }
            return Result;
        }

        public DataTable AdminUpdateCentreMaster(DataTable dt1, int adminid, out string OutError)  // ExamErrorListMISSP
        {
            int OutStatus;
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("AdminUpdateCentreMaster_SP", con);//AdminResultUpdateMIS_SPNew//AdminResultUpdateMIS_SP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@TypetblAdminUpdateCentreMaster", dt1);
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

        #endregion  Admin Result Update MIS


        public string UpdateCentStaff(string staffid, string cent)
        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UpdateCentStaff_Sp", con);//ReGenerateChallaanByIdSPAdminNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@staffid", staffid);
                    cmd.Parameters.AddWithValue("@cent", cent);

                    con.Open();
                    result = cmd.ExecuteNonQuery().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }

        public DataSet GetAdminDeoUser(string search)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAdminDeoUser_Sp", con); //GetSTAFFAllDistStaffAdmin_Sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet DeoUserUnlock(string dist)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("FinalSubmitDeoPortalUnlock_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@dist", dist);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet ObserversReport(DEOModel obj, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ObserverReport", con); //GetAll10thPass
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", obj.SelDist);
                    cmd.Parameters.AddWithValue("@class", obj.Class);
                    cmd.Parameters.AddWithValue("@examdate", obj.centreDate);
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

        public DataSet OldStaffAppointmentReport(DEOModel obj, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("OldStaffAppointmentReport", con); //ObserverReport
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", obj.SelDist);
                    cmd.Parameters.AddWithValue("@class", obj.Class);
                    cmd.Parameters.AddWithValue("@examdate", obj.centreDate);
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
        public DataSet PreviousStaffReport(DEOModel obj, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PreviousStaffReport_SP", con); //GetAll10thPass
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@userid", obj.SelDist);
                    //cmd.Parameters.AddWithValue("@class", obj.Class);
                    //cmd.Parameters.AddWithValue("@examdate", obj.centreDate);
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

        #region Sports Marks Entry DB
        public DataSet GetDeoStudentDetails(string Rollno, string currentUser)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetDeoStudentDetails_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Rollno", Rollno);
                    cmd.Parameters.AddWithValue("@currentUser", currentUser);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet GetDeoAllDataSportsMarksEntry(string currentusr, string selUser)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetDeoAllDataSportsMarksEntry_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@currentusr", currentusr);
                    cmd.Parameters.AddWithValue("@selUser", selUser);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public DataSet DeoFSSportsMarksEntry(string currentusr, string selUser)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DeoFSSportsMarksEntry_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@currentusr", currentusr);
                    cmd.Parameters.AddWithValue("@selUser", selUser);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
        public string UpdateSportMarkEntry(string Rollno, string refno, string SportName, string currentUser, string ccode, string ag, string ses, string pos)
        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UpdateSportMarkEntry_Sp", con);//ReGenerateChallaanByIdSPAdminNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Rollno", Rollno);
                    cmd.Parameters.AddWithValue("@refno", refno);
                    cmd.Parameters.AddWithValue("@SportName", SportName);
                    cmd.Parameters.AddWithValue("@currentUser", currentUser);
                    cmd.Parameters.AddWithValue("@ccode", ccode);
                    cmd.Parameters.AddWithValue("@ag", ag);
                    cmd.Parameters.AddWithValue("@ses", ses);
                    cmd.Parameters.AddWithValue("@pos", pos);
                    con.Open();
                    result = cmd.ExecuteNonQuery().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }
        public string DeleteSportEntry(string Rollno, string currentUser)
        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DeleteSportEntry_Sp", con);//ReGenerateChallaanByIdSPAdminNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Rollno", Rollno);
                    cmd.Parameters.AddWithValue("@currentUser", currentUser);
                    con.Open();
                    result = cmd.ExecuteNonQuery().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }
        public string BackToRecheck(string Rollno, string currentUser)
        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BackToRecheck_Sp", con);//ReGenerateChallaanByIdSPAdminNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Rollno", Rollno);
                    cmd.Parameters.AddWithValue("@currentUser", currentUser);
                    con.Open();
                    result = cmd.ExecuteNonQuery().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }
        public string BackToRecheckAll(string currentUser, string selUser)
        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BackToRecheckAll_Sp", con);//ReGenerateChallaanByIdSPAdminNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@currentUser", currentUser);
                    cmd.Parameters.AddWithValue("@selUser", selUser);
                    con.Open();
                    result = cmd.ExecuteNonQuery().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }
        public DataSet GetSportsMarksViewList(string currentUser)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetSportsMarksViewList_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@currentUser", currentUser);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        #endregion Sports Marks Entry DB



        #region new changes
        public DataSet SupervisoryStaffList_SpByMonthYear(string month,string year, string Dist,string ExamMonth)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SupervisoryStaffList_SpByMonthYear", con); //GetAll10thPass
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@month", month);
                    cmd.Parameters.AddWithValue("@yr", year);
                    cmd.Parameters.AddWithValue("@dist", Dist);
                    cmd.Parameters.AddWithValue("@ExamMonth", ExamMonth);
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

        #endregion


        public DataSet Get_PSTET_Master(string SCHL)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Get_PSTET_MasterSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    //con.Open();
                    return result;
                }

            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
    }
}