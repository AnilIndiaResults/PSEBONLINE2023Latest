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
using Ionic.Zip;
using System.Web.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using System.Data.Entity;

namespace PSEBONLINE.AbstractLayer
{
    public class OnDemandCertificateDB
    {
        private DBContext context;
        public OnDemandCertificateDB()
        {
            context = new DBContext();
        }
        public static List<OnDemandCertificateSearchModel> GetOnDemandCertificateStudentList(string type, string RP, string cls, string schl, string search, out DataSet dsOut)
        {
            List<OnDemandCertificateSearchModel> registrationSearchModels = new List<OnDemandCertificateSearchModel>();
            DataSet ds = new DataSet();
            Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetOnDemandCertificateStudentListSP";
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@RP", RP);
            cmd.Parameters.AddWithValue("@Class", cls);
            cmd.Parameters.AddWithValue("@schl", schl);
            cmd.Parameters.AddWithValue("@search", search);
            ds = db.ExecuteDataSet(cmd);
            if (ds != null)
            {
                var eList = ds.Tables[0].AsEnumerable().Select(dataRow => new OnDemandCertificateSearchModel
                {
                    Roll = dataRow.Field<string>("Roll"),
                    Std_id = dataRow.Field<long>("Std_id"),
                    form = dataRow.Field<string>("form"),
                    SCHL = dataRow.Field<string>("SCHL"),
                    AdmDate = dataRow.Field<string>("AdmDate"),
                    name = dataRow.Field<string>("name"),
                    fname = dataRow.Field<string>("fname"),
                    mname = dataRow.Field<string>("mname"),
                    DOB = dataRow.Field<string>("DOB"),
                    Aadhar = dataRow.Field<string>("Aadhar"),
                    //
                    regno = dataRow.Field<string>("regno"),
                    Dist = dataRow.Field<string>("Dist"),
                    EXAM = dataRow.Field<string>("EXAM"),
                    phy_chal = dataRow.Field<string>("phy_chal"),
                    IsExistsInOnDemandCertificates = dataRow.Field<int>("IsExistsInOnDemandCertificates"),
                    DemandId = dataRow.Field<long>("DemandId"),
                    IsChallanCancel = dataRow.Field<int>("IsChallanCancel"),
                    IsHardCopyCertificate = dataRow.Field<string>("IsHardCopyCertificate"),
                    OnDemandCertificatesStatus = dataRow.Field<string>("OnDemandCertificatesStatus"),

                }).ToList();

                registrationSearchModels = eList.ToList();
            }
            dsOut = ds;
            return registrationSearchModels;

        }

        public static List<OnDemandCertificate_ChallanDetailsViews> OnDemandCertificate_ChallanList(string schl, out DataSet dsOut)
        {
            List<OnDemandCertificate_ChallanDetailsViews> registrationSearchModels = new List<OnDemandCertificate_ChallanDetailsViews>();
            DataSet ds = new DataSet();
            Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "OnDemandCertificate_ChallanListSP";          
            cmd.Parameters.AddWithValue("@schl", schl);
            ds = db.ExecuteDataSet(cmd);
            if (ds != null)
            {
                var eList = StaticDB.DataTableToList<OnDemandCertificate_ChallanDetailsViews>(ds.Tables[0]);              
                registrationSearchModels = eList.ToList();
            }
            dsOut = ds;
            return registrationSearchModels;

        }
        public int InsertOnDemandCertificateSingleRemove(OnDemandCertificates onDemandCertificates)
        {
            int result = 0;

            try
            {
                context.OnDemandCertificates.Attach(onDemandCertificates);
                context.OnDemandCertificates.Remove(onDemandCertificates);
                result = context.SaveChanges();

            }
            catch (Exception e1)
            {
               
            }
            if (result == 0)
            {
                try
                {
                    context.Entry(onDemandCertificates).State = EntityState.Deleted;
                    result = context.SaveChanges();
                }
                catch (Exception ee)
                {
                   
                }
            }
              
            return result;
        }

        public int InsertOnDemandCertificateSingle(OnDemandCertificates onDemandCertificates)
        {
            int result = 0;
            context.OnDemandCertificates.Add(onDemandCertificates);
            result = context.SaveChanges();
            return result;
        }

        public int InsertOnDemandCertificateStudentList(List<OnDemandCertificates> list)
        {
            int result = 0;
            if (list.Count() > 0)
            {
                context.OnDemandCertificates.AddRange(list);
                result = context.SaveChanges();
            }
            return result;
        }

        public int RemoveRangeOnDemandCertificateStudentList(List<OnDemandCertificates> list)
        {
            int result = 0;
            if (list.Count() > 0)
            {
                //context.OnDemandCertificates.RemoveRange(list);  
                int i = 0;
                foreach (OnDemandCertificates onDemandCertificates in list)
                {
                    context.OnDemandCertificates.Attach(onDemandCertificates);
                    context.OnDemandCertificates.Remove(onDemandCertificates);
                    context.SaveChanges();
                    i++;
                }
                result = i;
            }
            return result;
        }

        public  bool IsRollNumberVerified(string roll)
        {
            bool IsExists = false;
            if (!string.IsNullOrEmpty(roll))
            {
                 IsExists = context.OnDemandCertificatesVerifiedStudentCompleteDetailsViews.Where(s => s.roll == roll && s.ChallanVerify == true).Count() > 0;
            }
            return IsExists;
        }


        public static DataSet OnDemandCertificatesCountRecordsClassWise(string search, string schl)
        {
            List<OnDemandCertificate_ChallanDetailsViews> registrationSearchModels = new List<OnDemandCertificate_ChallanDetailsViews>();
            DataSet ds = new DataSet();
            Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "OnDemandCertificatesCountRecordsClassWise_SP";
            cmd.Parameters.AddWithValue("@search", search);
            cmd.Parameters.AddWithValue("@Schl", schl);
            ds = db.ExecuteDataSet(cmd);          
          
            return ds;

        }


        public static DataSet OnDemandCertificateCalculateFee(string cls,string date, string search, string schl)
        {
            List<OnDemandCertificate_ChallanDetailsViews> registrationSearchModels = new List<OnDemandCertificate_ChallanDetailsViews>();
            DataSet ds = new DataSet();
            Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "OnDemandCertificateCalculateFeeSP";
            cmd.Parameters.AddWithValue("@cls", cls);
            cmd.Parameters.AddWithValue("@Schl", schl);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@search", search);
           
            ds = db.ExecuteDataSet(cmd);

            return ds;

        }



        #region For Individuals

        public static Task<OnDemandCertificatesLoginSession> CheckLogin(OnDemandCertificatesLoginModel LM)  // Type 1=Regular, 2=Open
        {
            OnDemandCertificatesLoginSession loginSession = new OnDemandCertificatesLoginSession();
            try
            {
               Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "OnDemandCertificatesLoginSP";
                cmd.Parameters.AddWithValue("@Session", LM.Session);
                cmd.Parameters.AddWithValue("@RP", LM.RP);
                cmd.Parameters.AddWithValue("@ROLL", LM.ROLL);
                cmd.Parameters.AddWithValue("@REGNO", LM.REGNO);
                cmd.Parameters.Add("@OutStatus", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        loginSession.Schl = DBNull.Value != reader["Schl"] ? (string)reader["Schl"] : default(string);
                        loginSession.std_id = DBNull.Value != reader["std_id"] ? (long)reader["std_id"] : default(long);
                        //
                        loginSession.CAT = DBNull.Value != reader["CAT"] ? (string)reader["CAT"] : default(string);
                        loginSession.CLASS = DBNull.Value != reader["CLASS"] ? (string)reader["CLASS"] : default(string);
                        loginSession.YEAR = DBNull.Value != reader["YEAR"] ? (string)reader["YEAR"] : default(string);
                        loginSession.MONTH = DBNull.Value != reader["MONTH"] ? (string)reader["MONTH"] : default(string);

                        loginSession.RP = DBNull.Value != reader["RP"] ? (string)reader["RP"] : default(string);
                        loginSession.ROLL = DBNull.Value != reader["ROLL"] ? (string)reader["ROLL"] : default(string);
                        loginSession.REGNO = DBNull.Value != reader["REGNO"] ? (string)reader["REGNO"] : default(string);


                        loginSession.NAME = DBNull.Value != reader["NAME"] ? (string)reader["NAME"] : default(string);
                        loginSession.FNAME = DBNull.Value != reader["FNAME"] ? (string)reader["FNAME"] : default(string);
                        loginSession.MNAME = DBNull.Value != reader["MNAME"] ? (string)reader["MNAME"] : default(string);

                        loginSession.RESULT = DBNull.Value != reader["RESULT"] ? (string)reader["RESULT"] : default(string);
                        loginSession.RESULTDTL = DBNull.Value != reader["RESULTDTL"] ? (string)reader["RESULTDTL"] : default(string);

                        loginSession.Mobile = DBNull.Value != reader["Mobile"] ? (string)reader["Mobile"] : default(string);
                        loginSession.Address = DBNull.Value != reader["Address"] ? (string)reader["Address"] : default(string);

                        loginSession.LoginStatus = DBNull.Value != reader["LoginStatus"] ? (int)reader["LoginStatus"] : default(int);
                        loginSession.AppliedSession = DBNull.Value != reader["AppliedSession"] ? (string)reader["AppliedSession"] : default(string);



                    }
                }
                Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                loginSession = null;
            }
            return Task.FromResult(loginSession);
        }



        public  Task<OnDemandCertificatesIndividuals> GetDataByLoginDetails(OnDemandCertificatesLoginSession LM)  // Type 1=Regular, 2=Open
        {
            OnDemandCertificatesIndividuals obj = new OnDemandCertificatesIndividuals();
            if (LM != null)
            {
                obj = context.OnDemandCertificatesIndividuals.SingleOrDefault(x => x.Roll.Trim() == LM.ROLL.Trim() && x.RegNo.Trim() == LM.REGNO.Trim());
            }
            Thread.Sleep(2000);
            return Task.FromResult(obj);

        }


        public async Task<string> OnDemandCertificatesIndividualsSave(OnDemandCertificatesIndividuals onDemandCertificatesIndividuals)  // Type 1=Regular, 2=Open
        {
            string result = "";
            if (onDemandCertificatesIndividuals.OnDemandCertificatesIndividualId == 0)
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        OnDemandCertificatesIndividuals onDemandCertificatesIndividuals1 = new OnDemandCertificatesIndividuals()
                        {
                            CandId = onDemandCertificatesIndividuals.CandId,
                            Schl = onDemandCertificatesIndividuals.Schl,
                            Class = onDemandCertificatesIndividuals.Class,
                            ApplyYear = onDemandCertificatesIndividuals.ApplyYear,
                            ApplyMonth = onDemandCertificatesIndividuals.ApplyMonth,
                            RP = onDemandCertificatesIndividuals.RP,
                            Roll = onDemandCertificatesIndividuals.Roll,
                            RegNo = onDemandCertificatesIndividuals.RegNo,
                            Name = onDemandCertificatesIndividuals.Name,
                            FName = onDemandCertificatesIndividuals.FName,
                            MName = onDemandCertificatesIndividuals.MName,
                            Result = onDemandCertificatesIndividuals.Result,
                            Resultdtl = onDemandCertificatesIndividuals.Resultdtl,
                            Mobile = onDemandCertificatesIndividuals.Mobile,
                            Address = onDemandCertificatesIndividuals.Address,
                            CreatedBy = onDemandCertificatesIndividuals.Roll,
                            CreatedOn = DateTime.Now,
                            ModifyOn = DateTime.Now,
                            IsActive = true,
                            IsFinalSubmit = 0,
                            FilePath = "",
                            IsCancel = onDemandCertificatesIndividuals.IsCancel,
                        };

                        context.OnDemandCertificatesIndividuals.Add(onDemandCertificatesIndividuals1);
                        int insertedRecords = await context.SaveChangesAsync();
                        if (insertedRecords > 0)
                        {
                            result = "S";
                            try
                            {
                                OnDemandCertificates onDemandCertificates = new OnDemandCertificates()
                                {
                                    Std_id = onDemandCertificatesIndividuals.CandId,
                                    Schl = onDemandCertificatesIndividuals.Schl,
                                    Cls = Convert.ToInt32(onDemandCertificatesIndividuals.Class),                                    
                                    Fee = 0,
                                    LateFee = 0,
                                    TotalFee = 0,
                                    IsPrinted = 0,
                                    IsChallanCancel = 0,
                                    SubmitOn = DateTime.Now,
                                    SubmitBy = "USER",
                                };
                                //call method insert single 
                                int insertData = InsertOnDemandCertificateSingle(onDemandCertificates);
                            }
                            catch (Exception ex2)
                            {

                            }

                        }
                        else
                        { result = "N"; }
                        transaction.Commit();//transaction commit
                    }
                    catch (Exception ex1)
                    {
                        result = "Error : " + ex1.Message;
                        transaction.Rollback();
                    }
                }
            }
            return result;

        }


        public Task<string> OnDemandCertificatesIndividualsUpdate(OnDemandCertificatesIndividuals onDemandCertificatesIndividuals)  // Type 1=Regular, 2=Open
        {
            string result = "";
            if (onDemandCertificatesIndividuals.OnDemandCertificatesIndividualId > 0)
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        OnDemandCertificatesIndividuals onDemandCertificatesIndividuals1 = context.OnDemandCertificatesIndividuals.Find(onDemandCertificatesIndividuals.OnDemandCertificatesIndividualId);
                        onDemandCertificatesIndividuals1.IsFinalSubmit = 0;
                        onDemandCertificatesIndividuals1.Remarks = onDemandCertificatesIndividuals.Remarks;
                        onDemandCertificatesIndividuals1.ModifyBy = onDemandCertificatesIndividuals.Roll;
                        onDemandCertificatesIndividuals1.ModifyOn = DateTime.Now;
                        onDemandCertificatesIndividuals1.IsActive = true;
                        onDemandCertificatesIndividuals1.Address = onDemandCertificatesIndividuals.Address;
                        onDemandCertificatesIndividuals1.IsCancel = onDemandCertificatesIndividuals.IsCancel;
                        context.Entry(onDemandCertificatesIndividuals1).State = EntityState.Modified;
                        context.SaveChanges();
                        result = "M";
                        transaction.Commit();//transaction commit
                    }
                    catch (Exception ex1)
                    {
                        result = "Error : " + ex1.Message;
                        transaction.Rollback();
                    }
                }
            }
            return Task.FromResult(result);

        }

        public Task<string> OnDemandCertificatesIndividualsUnlock(OnDemandCertificatesIndividuals onDemandCertificatesIndividuals)  // Type 1=Regular, 2=Open
        {
            string result = "";
            if (onDemandCertificatesIndividuals.OnDemandCertificatesIndividualId > 0)
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        OnDemandCertificatesIndividuals onDemandCertificatesIndividuals1 = context.OnDemandCertificatesIndividuals.Find(onDemandCertificatesIndividuals.OnDemandCertificatesIndividualId);
                        onDemandCertificatesIndividuals1.IsFinalSubmit = 0;
                        onDemandCertificatesIndividuals1.Remarks = onDemandCertificatesIndividuals.Remarks;
                        onDemandCertificatesIndividuals1.ModifyBy = onDemandCertificatesIndividuals.Roll;
                        onDemandCertificatesIndividuals1.ModifyOn = DateTime.Now;
                        onDemandCertificatesIndividuals1.IsActive = true;
                        onDemandCertificatesIndividuals1.Address = onDemandCertificatesIndividuals.Address;
                        onDemandCertificatesIndividuals1.IsCancel = onDemandCertificatesIndividuals.IsCancel;
                        onDemandCertificatesIndividuals1.CancelOn = onDemandCertificatesIndividuals.CancelOn;
                        onDemandCertificatesIndividuals1.CancelBy = onDemandCertificatesIndividuals.CancelBy;
                        onDemandCertificatesIndividuals1.CancelRemarks = onDemandCertificatesIndividuals.CancelRemarks;
                        context.Entry(onDemandCertificatesIndividuals1).State = EntityState.Modified;
                        context.SaveChanges();
                        result = "UL";                       
                        transaction.Commit();//transaction commit
                    }
                    catch (Exception ex1)
                    {
                        result = "Error : " + ex1.Message;
                        transaction.Rollback();
                    }
                }
            }
            return Task.FromResult(result);

        }


        public Task<string> OnDemandCertificatesIndividualsCancel(OnDemandCertificatesIndividuals onDemandCertificatesIndividuals)  // Type 1=Regular, 2=Open
        {
            string result = "";
            if (onDemandCertificatesIndividuals.OnDemandCertificatesIndividualId > 0)
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        OnDemandCertificatesIndividuals onDemandCertificatesIndividuals1 = context.OnDemandCertificatesIndividuals.Find(onDemandCertificatesIndividuals.OnDemandCertificatesIndividualId);
                        onDemandCertificatesIndividuals1.IsFinalSubmit = 0;
                        onDemandCertificatesIndividuals1.Remarks = onDemandCertificatesIndividuals.Remarks;
                        onDemandCertificatesIndividuals1.ModifyBy = onDemandCertificatesIndividuals.Roll;
                        onDemandCertificatesIndividuals1.ModifyOn = DateTime.Now;
                        onDemandCertificatesIndividuals1.IsActive = true;
                        onDemandCertificatesIndividuals1.Address = onDemandCertificatesIndividuals.Address;
                        onDemandCertificatesIndividuals1.IsCancel = onDemandCertificatesIndividuals.IsCancel;
                        onDemandCertificatesIndividuals1.CancelOn = onDemandCertificatesIndividuals.CancelOn;
                        onDemandCertificatesIndividuals1.CancelBy = onDemandCertificatesIndividuals.CancelBy;
                        onDemandCertificatesIndividuals1.CancelRemarks = onDemandCertificatesIndividuals.CancelRemarks;
                        context.Entry(onDemandCertificatesIndividuals1).State = EntityState.Modified;
                        context.SaveChanges();
                        result = "C";

                        try
                        {
                            OnDemandCertificates onDemandCertificates = new OnDemandCertificates()
                            {
                                Std_id = onDemandCertificatesIndividuals.CandId,
                                Schl = onDemandCertificatesIndividuals.Schl,
                                Cls = Convert.ToInt32(onDemandCertificatesIndividuals.Class),
                                Fee = 0,
                                LateFee = 0,
                                TotalFee = 0,
                                IsPrinted = 0,
                                IsChallanCancel = 0,
                                SubmitOn = DateTime.Now,
                                SubmitBy = "USER",
                            };
                            //call method insert single 
                            int insertData = InsertOnDemandCertificateSingleRemove(onDemandCertificates);
                        }
                        catch (Exception ex2)
                        {

                        }
                        transaction.Commit();//transaction commit
                    }
                    catch (Exception ex1)
                    {
                        result = "Error : " + ex1.Message;
                        transaction.Rollback();
                    }
                }
            }
            return Task.FromResult(result);

        }

        public Task<string> OnDemandCertificatesIndividualsFinalSubmit(OnDemandCertificatesIndividuals onDemandCertificatesIndividuals)  // Type 1=Regular, 2=Open
        {
            string result = "";
            if (onDemandCertificatesIndividuals.OnDemandCertificatesIndividualId > 0)
            {
                using (DbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        OnDemandCertificatesIndividuals onDemandCertificatesIndividuals1 = context.OnDemandCertificatesIndividuals.Find(onDemandCertificatesIndividuals.OnDemandCertificatesIndividualId);
                        onDemandCertificatesIndividuals1.IsFinalSubmit = 1;
                        onDemandCertificatesIndividuals1.IsFinalSubmitOn = DateTime.Now;
                        context.Entry(onDemandCertificatesIndividuals1).State = EntityState.Modified;
                        context.SaveChanges();
                        result = "F";
                        transaction.Commit();//transaction commit
                    }
                    catch (Exception ex1)
                    {
                        result = "Error : " + ex1.Message;
                        transaction.Rollback();
                    }
                }
            }
            return Task.FromResult(result);   

        }


        public static DataSet OnDemandCertificateCalculateFeeForIndividual(string std_id, string cls, string date, string search, string schl)
        {
            List<OnDemandCertificate_ChallanDetailsViews> registrationSearchModels = new List<OnDemandCertificate_ChallanDetailsViews>();
            DataSet ds = new DataSet();
            Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "OnDemandCertificateCalculateFeeForIndividualSP";         
            cmd.Parameters.AddWithValue("@std_id", std_id);
            cmd.Parameters.AddWithValue("@cls", cls);
            cmd.Parameters.AddWithValue("@Schl", schl);
            cmd.Parameters.AddWithValue("@date", date);
            cmd.Parameters.AddWithValue("@search", search);

            ds = db.ExecuteDataSet(cmd);

            return ds;

        }


        public static List<OnDemandCertificate_ChallanDetailsViews> OnDemandCertificateIndividuals_ChallanList(string StudentList, out DataSet dsOut)
        {
            List<OnDemandCertificate_ChallanDetailsViews> registrationSearchModels = new List<OnDemandCertificate_ChallanDetailsViews>();
            DataSet ds = new DataSet();
            Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "OnDemandCertificateIndividuals_ChallanListSP";
            cmd.Parameters.AddWithValue("@StudentList", StudentList);
            ds = db.ExecuteDataSet(cmd);
            if (ds != null)
            {
                var eList = StaticDB.DataTableToList<OnDemandCertificate_ChallanDetailsViews>(ds.Tables[0]);
                registrationSearchModels = eList.ToList();
            }
            dsOut = ds;
            return registrationSearchModels;

        }

        #endregion

        #region Admin Panel 
        public static DataSet OnDemandCertificateDownloadData(string SelType,string EmpUserId,string AdminUser, int DOWNLOADLOT ,string Search,  out int OutStatus)
        {
            DataSet ds = new DataSet();
            Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "OnDemandCertificateDownloadDataSP";//DownloadChallanSP OnDemandCertificatedata
            cmd.Parameters.AddWithValue("@Search", Search);
            cmd.Parameters.AddWithValue("@SelType", SelType);
            cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
            cmd.Parameters.AddWithValue("@AdminUser", AdminUser);
            cmd.Parameters.AddWithValue("@DOWNLOADLOT", DOWNLOADLOT);
            cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
            ds = db.ExecuteDataSet(cmd);
            OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
            return ds;

        }
        public static List<OnDemandCertificatesVerifiedStudentCompleteDetailsViews> GetVerifiedOnDemandCertificateStudentList(string type, string search, out DataSet dsOut)
        {
            List<OnDemandCertificatesVerifiedStudentCompleteDetailsViews> registrationSearchModels = new List<OnDemandCertificatesVerifiedStudentCompleteDetailsViews>();
            DataSet ds = new DataSet();
            Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetVerifiedOnDemandCertificateStudentListSP";
            cmd.Parameters.AddWithValue("@type", type);           
            cmd.Parameters.AddWithValue("@search", search);
            ds = db.ExecuteDataSet(cmd);
            if (ds != null)
            {

                var eList = StaticDB.DataTableToList<OnDemandCertificatesVerifiedStudentCompleteDetailsViews>(ds.Tables[0]);
                registrationSearchModels = eList.ToList();
            }
            dsOut = ds;
            return registrationSearchModels;

        }


        public static List<OnDemandCertificatesVerifiedStudentCompleteDetailsViews> GetCompleteOnDemandCertificateStudentList(string type, string search, out DataSet dsOut)
        {
            List<OnDemandCertificatesVerifiedStudentCompleteDetailsViews> registrationSearchModels = new List<OnDemandCertificatesVerifiedStudentCompleteDetailsViews>();
            DataSet ds = new DataSet();
            Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetCompleteOnDemandCertificateStudentListSP";
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@search", search);
            ds = db.ExecuteDataSet(cmd);
            if (ds != null)
            {

                var eList = StaticDB.DataTableToList<OnDemandCertificatesVerifiedStudentCompleteDetailsViews>(ds.Tables[0]);
                registrationSearchModels = eList.ToList();
            }
            dsOut = ds;
            return registrationSearchModels;

        }



        public  string CheckRegistryMISExcelExport(DataSet ds, out DataTable dt)
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
                        string roll1 = dt.Rows[i][0].ToString();
                        Result = "Please check Roll Number " + roll1 + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check Roll Number " + roll1 + " in row " + RowNo + ",  ";
                    }

                    if (dt.Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string challanid = dt.Rows[i][1].ToString();
                        Result = "Please check Certficate Number " + challanid + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check Certficate Number " + challanid + " in row " + RowNo + ",  ";
                    }


                    if (dt.Rows[i][2].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string challanid = dt.Rows[i][2].ToString();
                        Result = "Please check Registry Number " + challanid + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check Registry Number " + challanid + " in row " + RowNo + ",  ";
                    }


                    if ( dt.Rows[i][3].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string challanid = dt.Rows[i][3].ToString();
                        Result = "Please check Registry Date " + challanid + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check Registry Date " + challanid + " in row " + RowNo + ",  ";
                    }

                    string roll = dt.Rows[i][0].ToString();
                    bool IsVerified = IsRollNumberVerified(roll);

                    if (!IsVerified)
                    {
                        int RowNo = i + 2;                 
                        Result = "Please check Roll Number is not Verified  " + roll + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check Roll Number is not Verified  " + roll + " in row " + RowNo + ",  ";
                    }

                }

            }
            catch (Exception ex)
            {
                dt = null;
                Result = ex.Message;
                //return "Please Check All Fields";
            }

            if (dt.Columns.Contains("STATUS"))
            {
                dt.Columns.Remove("STATUS");

            }
            dt.AcceptChanges();
            return Result;
        }

        public static int BulkRegistryMIS(DataTable dt1, int adminid, string EmpUserId, string MIS_FILENM, out int OutStatus, out string OutError)  // BulkChallanBank
        {


            DataSet ds = new DataSet();
            Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "BulkOnDemandRegistryMISSP";
            cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
            cmd.Parameters.AddWithValue("@ADMINID", adminid);
            cmd.Parameters.AddWithValue("@MIS_FILENM", MIS_FILENM); 
            cmd.Parameters.AddWithValue("@BulkOnDemandRegistry", dt1);          
            cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
            cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
            ds = db.ExecuteDataSet(cmd);
            OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
            OutError = (string)cmd.Parameters["@OutError"].Value;
            return OutStatus;
        }



        public static DataSet ViewRegistryMISSP(int selType,string search)
        {
            DataSet ds = new DataSet();
            Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "ViewRegistryMISSP";
            cmd.Parameters.AddWithValue("@selType", selType);
            cmd.Parameters.AddWithValue("@search", search);
            ds = db.ExecuteDataSet(cmd);        
            return ds;
        }
        #endregion


    }
}