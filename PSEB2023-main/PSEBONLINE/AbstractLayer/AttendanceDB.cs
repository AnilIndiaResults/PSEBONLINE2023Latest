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
using DocumentFormat.OpenXml.Office.Word;


namespace PSEBONLINE.AbstractLayer
{
    public class AttendanceDB
    {
        #region Check ConString
        private string CommonCon = "myDBConnection";
        public AttendanceDB()
        {
            CommonCon = "myDBConnection";
        }
        #endregion  Check ConString


        #region Public Method



        public int UnlockAttendanceMemoDetail(string empId, string MemoNumber, out string OutStatus, string remarks)
        {
            try
            {
                Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "UnlockAttendanceMemoDetailwithRemarks";
                cmd.Parameters.AddWithValue("@EmpUserId", empId);
                cmd.Parameters.AddWithValue("@remarks", remarks);
                cmd.Parameters.AddWithValue("@memoNumber", MemoNumber);
                cmd.Parameters.Add("@OutStatus", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                DataSet ds = db.ExecuteDataSet(cmd);
                OutStatus = (string)cmd.Parameters["@OutStatus"].Value;
                return 1;
            }
            catch (Exception ex)
            {
                OutStatus = "0";
                return 0;
            }
        }

        public static DataSet StudentAttendanceSave(DataTable AttendanceStudentDetail, string centrecode, string subCode, string schl, string cls, string rp, string examDate, string examBatch, string status,
        string createdby, string modifyby, string EmpUserId, out string OutMemoNumber, out string OutStatus)
        {
            try
            {
                Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 3600;
                cmd.CommandText = "SaveStudentAttendanceSPFinal";
                cmd.Parameters.AddWithValue("@AttendanceStudentDetail", AttendanceStudentDetail);
                cmd.Parameters.AddWithValue("@centrecode", centrecode);
                cmd.Parameters.AddWithValue("@subCode", subCode);
                cmd.Parameters.AddWithValue("@schl", schl);
                cmd.Parameters.AddWithValue("@cls", cls);
                cmd.Parameters.AddWithValue("@rp", rp);
                cmd.Parameters.AddWithValue("@examDate", examDate);
                cmd.Parameters.AddWithValue("@examBatch", examBatch);
                cmd.Parameters.AddWithValue("@CreatedBy", createdby);
                cmd.Parameters.AddWithValue("@ModifiedBy", modifyby);
                cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.Add("@OutMemoNumber", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@OutStatus", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;              //
                DataSet ds = db.ExecuteDataSet(cmd);
                OutMemoNumber = (string)cmd.Parameters["@OutStatus"].Value;
                OutStatus = (string)cmd.Parameters["@OutStatus"].Value;
                return ds;
            }
            catch (Exception ex)
            {
                OutMemoNumber = "-1";
                OutStatus = "error";
                return null;
            }

        }


        public static DataSet StudentAttendanceFinalSubmit(DataTable AttendanceStudentDetail, string memoNumber, string status, string finalSubmitBy, string EmpUserId)
        {
            try
            {
                DataSet ds = new DataSet();
                Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "StudentAttendanceFinalSubmitSP";
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@MemoNumber", memoNumber);
                cmd.Parameters.AddWithValue("@FinalSubmitBy", finalSubmitBy);
                cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                cmd.Parameters.AddWithValue("@AttendanceStudentDetail", AttendanceStudentDetail);
                //
                ds = db.ExecuteDataSet(cmd);
                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public static List<AttendenceSummaryDetail> AttendenceSummaryDetails(string centrecode, string cls, string rp)
        {
            List<AttendenceSummaryDetail> subjectAttendancedetail = new List<AttendenceSummaryDetail>();
            try
            {
                DataSet ds = new DataSet();
                Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "AttendenceSummaryDetailsSP";
                cmd.Parameters.AddWithValue("@centrecode", centrecode);
                cmd.Parameters.AddWithValue("@cls", cls);
                cmd.Parameters.AddWithValue("@rp", rp);
                ds = db.ExecuteDataSet(cmd);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var itemSubUType = StaticDB.DataTableToList<AttendenceSummaryDetail>(ds.Tables[0]);
                    subjectAttendancedetail = itemSubUType.ToList();
                }
                return subjectAttendancedetail;
            }
            catch (Exception ex)
            {
                return subjectAttendancedetail;
            }

        }


        public static List<AttendanceAdminReport> AttendenceSummaryDetailsAdmin(string centrecode, string cls, string rp)
        {
            List<AttendanceAdminReport> subjectAttendancedetail = new List<AttendanceAdminReport>();
            try
            {
                DataSet ds = new DataSet();
                Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "AttendenceSummaryDetailsSPAdmin";
                cmd.Parameters.AddWithValue("@centrecode", centrecode);
                cmd.Parameters.AddWithValue("@cls", cls);
                cmd.Parameters.AddWithValue("@rp", rp);
                ds = db.ExecuteDataSet(cmd);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    var itemSubUType = StaticDB.DataTableToList<AttendanceAdminReport>(ds.Tables[0]);
                    subjectAttendancedetail = itemSubUType.ToList();
                }
                return subjectAttendancedetail;
            }
            catch (Exception ex)
            {
                return subjectAttendancedetail;
            }

        }

        #endregion

    }
}