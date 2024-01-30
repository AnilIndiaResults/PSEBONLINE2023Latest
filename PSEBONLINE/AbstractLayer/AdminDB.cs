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
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PSEBONLINE.AbstractLayer
{
    public class AdminDB
    {
        #region Check ConString
        private DBContext _context = new DBContext();

        private string CommonCon = "myDBConnection";
        public AdminDB()
        {   
                CommonCon = "myDBConnection";
            
        }
        #endregion  Check ConString

        #region
        public List<AdminEmployeeMasters> GetAdminEmployeeMasters()
        {
            List<AdminEmployeeMasters> obj = _context.AdminEmployeeMasters.AsNoTracking().ToList();
            return obj;

        }

        public AdminEmployeeMasters GetAdminEmployeeMastersByUserId(string userId)
        {
            AdminEmployeeMasters obj = _context.AdminEmployeeMasters.AsNoTracking().SingleOrDefault(s => s.Userid == userId);
            return obj;

        }


        public AdminEmployeeMasters UpdateAdminEmployeeMastersByUserIdAPI(string userid, string pwd, AdminEmployeeAPIModel adminEmployeeAPIModel ,out int updStatus)
        {
            int status = 0;
            try
            {
                bool isExists = _context.AdminEmployeeMasters.AsNoTracking().Where(s => s.Userid == userid).Count() > 0;

                if (!isExists)// insert
                {
                    AdminEmployeeMasters adminEmployeeMasters = new AdminEmployeeMasters()
                    {
                        Userid = userid,
                        pwd = pwd,
                        id = adminEmployeeAPIModel.id,                        
                        Name = adminEmployeeAPIModel.Name,
                        FName = adminEmployeeAPIModel.FName,                      
                        Post = adminEmployeeAPIModel.Post,
                        Type = adminEmployeeAPIModel.Type,
                        employeeImg = adminEmployeeAPIModel.employeeImg,
                        HouseNo = adminEmployeeAPIModel.HouseNo,
                        AddressLine1 = adminEmployeeAPIModel.AddressLine1,
                        AddressLine2 = adminEmployeeAPIModel.AddressLine2,
                        City_Village = adminEmployeeAPIModel.City_Village,
                        Tehsil = adminEmployeeAPIModel.Tehsil,
                        Pin = adminEmployeeAPIModel.Pin,
                        MobileNo = adminEmployeeAPIModel.MobileNo,
                        Email = adminEmployeeAPIModel.Email,
                        MobileUniqueId = adminEmployeeAPIModel.MobileUniqueId,
                        IsApp = adminEmployeeAPIModel.IsApp,
                        role = adminEmployeeAPIModel.role,
                        pan = adminEmployeeAPIModel.pan,
                        dob = adminEmployeeAPIModel.dob,
                        dor = adminEmployeeAPIModel.dor,
                        doj = adminEmployeeAPIModel.doj,
                        Bcode = adminEmployeeAPIModel.Bcode,
                        postcode = adminEmployeeAPIModel.postcode,
                        BranchName = adminEmployeeAPIModel.BranchName,
                        Status = adminEmployeeAPIModel.Status,
                        remarks = adminEmployeeAPIModel.remarks,
                        AttVerifier = adminEmployeeAPIModel.AttVerifier,
                        LastUpdateDate = DateTime.Now,
                    };
                    _context.AdminEmployeeMasters.Add(adminEmployeeMasters);
                    status = _context.SaveChanges();
                    updStatus = status;
                    return adminEmployeeMasters;
                }
                else
                {


                    AdminEmployeeMasters adminEmployeeMasters = _context.AdminEmployeeMasters.Find(userid);
                    adminEmployeeMasters.Userid = userid;
                    adminEmployeeMasters.pwd = pwd;
                    adminEmployeeMasters.id = adminEmployeeAPIModel.id;
                    adminEmployeeMasters.Name = adminEmployeeAPIModel.Name;
                    adminEmployeeMasters.FName = adminEmployeeAPIModel.FName;
                    /// adminEmployeeMasters.pwd = adminEmployeeAPIModel.pwd;
                    adminEmployeeMasters.Post = adminEmployeeAPIModel.Post;
                    adminEmployeeMasters.Type = adminEmployeeAPIModel.Type;
                    adminEmployeeMasters.employeeImg = adminEmployeeAPIModel.employeeImg;
                    adminEmployeeMasters.HouseNo = adminEmployeeAPIModel.HouseNo;
                    adminEmployeeMasters.AddressLine1 = adminEmployeeAPIModel.AddressLine1;
                    adminEmployeeMasters.AddressLine2 = adminEmployeeAPIModel.AddressLine2;
                    adminEmployeeMasters.City_Village = adminEmployeeAPIModel.City_Village;
                    adminEmployeeMasters.Tehsil = adminEmployeeAPIModel.Tehsil;
                    adminEmployeeMasters.Pin = adminEmployeeAPIModel.Pin;
                    adminEmployeeMasters.MobileNo = adminEmployeeAPIModel.MobileNo;
                    adminEmployeeMasters.Email = adminEmployeeAPIModel.Email;
                    adminEmployeeMasters.MobileUniqueId = adminEmployeeAPIModel.MobileUniqueId;
                    adminEmployeeMasters.IsApp = adminEmployeeAPIModel.IsApp;
                    adminEmployeeMasters.role = adminEmployeeAPIModel.role;
                    adminEmployeeMasters.pan = adminEmployeeAPIModel.pan;
                    adminEmployeeMasters.dob = adminEmployeeAPIModel.dob;
                    adminEmployeeMasters.dor = adminEmployeeAPIModel.dor;
                    adminEmployeeMasters.doj = adminEmployeeAPIModel.doj;
                    adminEmployeeMasters.Bcode = adminEmployeeAPIModel.Bcode;
                    adminEmployeeMasters.postcode = adminEmployeeAPIModel.postcode;
                    adminEmployeeMasters.BranchName = adminEmployeeAPIModel.BranchName;
                    adminEmployeeMasters.Status = adminEmployeeAPIModel.Status;
                    adminEmployeeMasters.remarks = adminEmployeeAPIModel.remarks;
                    adminEmployeeMasters.AttVerifier = adminEmployeeAPIModel.AttVerifier;
                    adminEmployeeMasters.LastUpdateDate = DateTime.Now;
                    _context.Entry(adminEmployeeMasters).State = System.Data.Entity.EntityState.Modified;
                    status = _context.SaveChanges();
                    updStatus = status;
                    return adminEmployeeMasters;
                }
                
            }
            catch (Exception ex)
            {
                status = - 1;
            }
            updStatus = status;
            return null;
        }
        #endregion

        public static AdminEmployeeAPIModel CheckAdminEmployeeLogin(LoginModel LM, out int empLoginStatus)  // Type 1=Regular, 2=Open
        {
            AdminEmployeeAPIModel adminEmployeeAPIModel = new AdminEmployeeAPIModel();
            int emploginStatus1= 0;
            try
            {

                HttpClientHandler handler = new HttpClientHandler();
                HttpClient client = new HttpClient(handler);

                var authenticationString = $"{LM.AdminEmployeeUserId}:{LM.AdminEmployeePassword}";


                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization",
                            Convert.ToBase64String(Encoding.Default.GetBytes(authenticationString)));
                //Need to change the PORT number where your WEB API service is running         
                //string url = String.Format("https://api.pseb.ac.in/api/SalaryMasters/GetEmployeeBasicDetailCust?UserName={0}", LM.AdminEmployeeUserId);

                // with SSL
                string url = String.Format("https://api.pseb.ac.in/api/SalaryMasters/GetEmployeeBasicDetailCust?UserName={0}", LM.AdminEmployeeUserId);

                var result = client.GetAsync(new Uri(url)).Result;
                if (result.IsSuccessStatusCode)
                {
                    // Console.WriteLine("Done" + result.StatusCode);
                    var JsonContent = result.Content.ReadAsStringAsync().Result;

                    var readTask = result.Content.ReadAsAsync<AdminEmployeeAPIModel>();
                    readTask.Wait();
                    emploginStatus1 = 1;
                    adminEmployeeAPIModel = readTask.Result;
                }
                else //web api sent error response 
                {
                    emploginStatus1 = -5;
                    adminEmployeeAPIModel = null;

                }             

            }
            catch (Exception ex)
            {         
                
            }
            empLoginStatus = emploginStatus1;
            return adminEmployeeAPIModel;
        }


        public static AdminLoginSession CheckAdminLogin(LoginModel LM)  // Type 1=Regular, 2=Open
        {
            AdminLoginSession adminLoginSession = new AdminLoginSession();
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "AdminLoginSP";// LoginSP(old)
                cmd.Parameters.AddWithValue("@UserName", LM.username);
               cmd.Parameters.AddWithValue("@Password", LM.Password);
               cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    if (reader.Read())
                    {
                        adminLoginSession.AdminId = DBNull.Value != reader["id"] ? (int)reader["id"] : default(int);
                        adminLoginSession.USER = DBNull.Value != reader["USER"] ? (string)reader["USER"] : default(string);
                        adminLoginSession.AdminType = DBNull.Value != reader["Usertype"] ? (string)reader["Usertype"] : default(string);
                        adminLoginSession.USERNAME = DBNull.Value != reader["User_fullnm"] ? (string)reader["User_fullnm"] : default(string);
                        adminLoginSession.PAccessRight = DBNull.Value != reader["PAccessRight"] ? (string)reader["PAccessRight"] : default(string);
                        adminLoginSession.Dist_Allow = DBNull.Value != reader["Dist_Allow"] ? (string)reader["Dist_Allow"] : default(string);
                        adminLoginSession.ActionRight = DBNull.Value != reader["ActionRight"] ? (string)reader["ActionRight"] : default(string);
                        adminLoginSession.LoginStatus = DBNull.Value != reader["Status"] ? (int)reader["Status"] : default(int);
                        //
                        adminLoginSession.RoleType = DBNull.Value != reader["RoleType"] ? (string)reader["RoleType"] : default(string);
                        adminLoginSession.ClassAssign = DBNull.Value != reader["ActionRight"] ? (string)reader["ActionRight"] : default(string);


                    }
                }
            }
            catch (Exception ex)
            {
                adminLoginSession.LoginStatus = 0;
            }
            return adminLoginSession;
        }

        //public static DataTable CheckAdminLogin(LoginModel LM)  // Type 1=Regular, 2=Open
        //{
        //    // int iRetVal;
        //    SqlDataAdapter dataAdapter = new SqlDataAdapter();
        //    DataTable dataTable = new DataTable();
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
        //        {
        //            SqlCommand cmd = new SqlCommand("AdmimLoginSP", con);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@UserName", LM.username);
        //            cmd.Parameters.AddWithValue("@Password", LM.Password);
        //            cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
        //            con.Open();

        //            dataAdapter.SelectCommand = cmd;
        //            dataAdapter.Fill(dataTable);
        //            return dataTable;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //    finally
        //    {
        //        // con.Close();
        //    }
        //}


   

        public DataSet ExecuteSqlQuery(string Sqlquery)
        {
            string query = Sqlquery.ToString();
            DataSet ds = new DataSet();
            return ds;
            //using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
            //{
            //    con.Open();
            //    SqlDataAdapter da = new SqlDataAdapter(query, con);
            //    da.SelectCommand.CommandTimeout = 1000;
            //    da.Fill(ds);
            //    return ds;               
            //}
        }

        public List<AffObjectionListMasters> GetAffObjectionListMasters()
        {
            List<AffObjectionListMasters> itemList = new List<AffObjectionListMasters>();
           // itemList = _context.AffObjectionListMasters.AsNoTracking().Where(s => s.IsActive == 1).ToList();
            foreach (AffObjectionListMasters dr in _context.AffObjectionListMasters.AsNoTracking().Where(s => s.IsActive == 1).ToList())
            {
                itemList.Add(new AffObjectionListMasters { objection = dr.objcode.ToString() + " - "+dr.objection.ToString(), objcode = dr.objcode.ToString() });
            }
            return itemList;

        }

       

        public static List<SelectListItem> getAdminDistAllowList(string usertype,string adminid)
        {
            List<SelectListItem> itemDist = new List<SelectListItem>();
            if (usertype.ToLower() == "admin")
            {
                DataSet ds = new AbstractLayer.AdminDB().GetAllAdminUser(Convert.ToInt32(adminid), "");                
                foreach (System.Data.DataRow dr in ds.Tables[1].Rows)
                {
                    itemDist.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }
            }
            else if (usertype.ToLower() == "deo")
            {
                DataSet Dresult = new AbstractLayer.DEODB().AdminGetDeoDIST(adminid.ToString()); // passing Value to DBClass from model            
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {
                    itemDist.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }
            }

            return itemDist;
        }



        #region ShiftChallanDetails
        public DataSet ShiftChallanDetails(ShiftChallanDetails scd, out string OutError, out int OutSID)
        {
            DataSet dataTable = new DataSet();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ShiftChallanDetailsSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ShiftId", scd.ShiftId);
                    cmd.Parameters.AddWithValue("@WrongChallan1", scd.WrongChallan);
                    cmd.Parameters.AddWithValue("@CorrectChallan1", scd.CorrectChallan); //.Replace('?','2')
                    cmd.Parameters.AddWithValue("@ShiftFile", scd.ShiftFile);
                    cmd.Parameters.AddWithValue("@ShiftRemarks", scd.ShiftRemarks);
                    cmd.Parameters.AddWithValue("@Type", scd.ActionType);
                    cmd.Parameters.AddWithValue("@AdminId", scd.AdminId);
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@OutSID", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                    OutSID = (int)cmd.Parameters["@OutSID"].Value;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                OutError = "";
                OutSID = -1;
                return null;
            }
        }

        public DataSet ViewShiftChallanDetails(int Sid, int Type, string search, int pageNumber, int PageSize)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ViewShiftChallanDetailsSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Sid", Sid);
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

        #endregion ShiftChallanDetails


        public DataSet CommonCalculateFeeAdmin(string RefNo, string form)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CommonCalculateFeeAdminSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", RefNo);
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






        #region DuplicateCertificate
        public string DuplicateCertificate(DuplicateCertificate obj, int type1, out string OutError)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("DuplicateCertificateSP", con); //DuplicateCertificateSPTEST
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", obj.id);
                cmd.Parameters.AddWithValue("@DairyNo", obj.DairyNo);
                cmd.Parameters.AddWithValue("@DairyDate", obj.DairyDate);
                cmd.Parameters.AddWithValue("@Class", obj.Class);
                cmd.Parameters.AddWithValue("@Session", obj.Session);
                cmd.Parameters.AddWithValue("@Roll", obj.Roll);
                cmd.Parameters.AddWithValue("@Name", obj.Name);
                cmd.Parameters.AddWithValue("@Dist", obj.Dist);
                cmd.Parameters.AddWithValue("@Mobile", obj.Mobile);
                cmd.Parameters.AddWithValue("@CertNo", obj.CertNo);
                cmd.Parameters.AddWithValue("@CertDate", obj.CertDate);
                cmd.Parameters.AddWithValue("@DispNo", obj.DispNo);
                cmd.Parameters.AddWithValue("@DispDate", obj.DispDate);
                cmd.Parameters.AddWithValue("@Remarks", obj.Remarks);
                cmd.Parameters.AddWithValue("@CreatedBy", obj.Adminid);
                cmd.Parameters.AddWithValue("@UpdatedBy", obj.Adminid);
                cmd.Parameters.AddWithValue("@Type", type1);
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                cmd.Parameters.AddWithValue("@Address", obj.Address);
                cmd.Parameters.AddWithValue("@Year", obj.Year);
                cmd.Parameters.AddWithValue("@IsSameAsRecord", obj.IsSameAsRecord);
                cmd.Parameters.AddWithValue("@TypeOf", obj.TypeOf);
                cmd.Parameters.AddWithValue("@Before", obj.Before);
                cmd.Parameters.AddWithValue("@After", obj.After);
                cmd.Parameters.AddWithValue("@FNAME", obj.FNAME);
                cmd.Parameters.AddWithValue("@MNAME", obj.MNAME);
                cmd.Parameters.AddWithValue("@ReceiptNo", obj.ReceiptNo);
                cmd.Parameters.AddWithValue("@ReceiptDate", obj.ReceiptDate);
                cmd.Parameters.AddWithValue("@FeeAmount", obj.FeeAmount);
                cmd.Parameters.AddWithValue("@ScanFile", obj.ScanFile);
                cmd.Parameters.AddWithValue("@IsForward", obj.IsForward);
                cmd.Parameters.AddWithValue("@ObjectionLetter", obj.ObjectionLetter);
                cmd.Parameters.AddWithValue("@SameAsRecordDT", obj.SameAsRecordDT);
                cmd.Parameters.AddWithValue("@isType", obj.IsType);
                cmd.Parameters.AddWithValue("@PrevCert", obj.PrevCert);
                cmd.Parameters.AddWithValue("@OrderBy", Convert.ToInt32(obj.OrderBy));
                cmd.Parameters.AddWithValue("@OrderNo", obj.OrderNo);
                cmd.Parameters.AddWithValue("@OrderDate", obj.OrderDate);
                cmd.Parameters.AddWithValue("@Result", obj.Result);
                cmd.Parameters.AddWithValue("@ResultDt", obj.ResultDt);
                cmd.Parameters.AddWithValue("@MaxMarks", obj.MaxMarks);
                cmd.Parameters.AddWithValue("@ObtMarks", obj.ObtMarks);
                cmd.Parameters.AddWithValue("@DOB", obj.DOB);
                cmd.Parameters.AddWithValue("@REGNO", obj.REGNO);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                OutError = (string)cmd.Parameters["@OutError"].Value;
                return OutError;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                OutError = "0";
                return result = "-1";
            }
            finally
            {
                con.Close();
            }
        }


        public DataSet ViewDuplicateCertificate(int DCid, int Type, string search, int pageNumber, int PageSize)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ViewDuplicateCertificatesP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DCid", DCid);
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

        public string DuplicateCertificateForward(string ForwardList, string Action, int RemarksBy, out string OutError)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("DuplicateCertificateForward", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ForwardList", ForwardList);
                cmd.Parameters.AddWithValue("@Action", Action);
                cmd.Parameters.AddWithValue("@RemarksBy", RemarksBy);
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                OutError = (string)cmd.Parameters["@OutError"].Value;
                return OutError;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                OutError = "0";
                return result = "-1";
            }
            finally
            {
                con.Close();
            }
        }


        public List<SelectListItem> GetDuplicateCertificateType(int tid)
        {
            DataTable dsType = DuplicateCertificateType(tid); // SessionMasterSPAdmin
            List<SelectListItem> itemType = new List<SelectListItem>();
            // itemSession.Add(new SelectListItem { Text = "2017", Value = "2017" });
            foreach (System.Data.DataRow dr in dsType.Rows)
            {
                itemType.Add(new SelectListItem { Text = @dr["Name"].ToString(), Value = @dr["Tid"].ToString() });
            }
            return itemType;
        }


        public DataTable DuplicateCertificateType(int TID)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DuplicateCertificateTypeSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TID", TID);
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

        public DataSet DuplicateCertificateSummary(string Search, int Type)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DuplicateCertificateSummary", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@Type", Type);
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


        #endregion DuplicateCertificate


        #region  Inbox Master

        public DataTable MailReplyMaster(InboxModel im, out string OutError)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("MailReplyMasterSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MId", im.id);
                    cmd.Parameters.AddWithValue("@Reply", im.Reply);
                    cmd.Parameters.AddWithValue("@ReplyFile", im.Attachments);
                    cmd.Parameters.AddWithValue("@ReplyTo", im.To);
                    cmd.Parameters.AddWithValue("@ReplyBy", im.From);
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
                OutError = "";
                return null;
            }
        }


        public DataTable AddInboxMaster(InboxModel im, out string OutError)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AddInboxMaster", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SentTo", im.To);
                    cmd.Parameters.AddWithValue("@SentFrom", im.From);
                    cmd.Parameters.AddWithValue("@Subject", im.Subject);
                    cmd.Parameters.AddWithValue("@Body", im.Body);
                    cmd.Parameters.AddWithValue("@Attachment", im.Attachments);
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
                OutError = "";
                return null;
            }
        }

        public DataSet ViewInboxMaster(int MailId, int Type, int adminid, string search, int pageNumber, int PageSize)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("ViewInboxMasterNew", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MailId", MailId);
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@adminid", adminid);
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

        public string ReadInbox(int Id, int AdminId, int type1, out string OutError)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("ReadInboxSP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Id", Id);
                cmd.Parameters.AddWithValue("@AdminId", AdminId);
                cmd.Parameters.AddWithValue("@type1", type1);
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                OutError = (string)cmd.Parameters["@OutError"].Value;
                return OutError;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                OutError = "0";
                return result = "-1";
            }
            finally
            {
                con.Close();
            }
        }

        #endregion  Inbox Master

        #region Menu Master
        public DataSet GetAllMenu(int id)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("MenuSP", con);
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


        public string CreateMenu(SiteMenu model, int IsParent, int ParentMenuId, int IsMenu, string AssignYear)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("CreateMenuSP", con);  //CreateAdminUserSP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MenuName", model.MenuName);
                cmd.Parameters.AddWithValue("@MenuUrl", model.MenuUrl);
                cmd.Parameters.AddWithValue("@IsParent", IsParent);
                cmd.Parameters.AddWithValue("@ParentMenuId", ParentMenuId);
                cmd.Parameters.AddWithValue("@IsMenu", IsMenu);
                cmd.Parameters.AddWithValue("@AssignYear", AssignYear);
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

        public DataSet UpdateMenu(SiteMenu model, int ParentMenuId, out int OutStatus, string AssignYear)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UpdateMenuSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MenuId", model.MenuID);
                    cmd.Parameters.AddWithValue("@MenuName", model.MenuName);
                    cmd.Parameters.AddWithValue("@MenuUrl", model.MenuUrl);
                    cmd.Parameters.AddWithValue("@ParentMenuId", ParentMenuId);
                    cmd.Parameters.AddWithValue("@AssignYear", AssignYear);
                    cmd.Parameters.Add("@Outstatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    OutStatus = (int)cmd.Parameters["@outstatus"].Value;
                    return result;
                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return result = null;
            }
        }

        public DataSet ListingMenu(int type, int menuid, out int OutStatus)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ListingMenuSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@menuid", menuid);
                    cmd.Parameters.Add("@outstatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    OutStatus = (int)cmd.Parameters["@outstatus"].Value;
                    return result;
                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return result = null;
            }
        }


        //public string ListingMenu(int type, int menuid, out int OutStatus)
        //{
        //    SqlConnection con = null;
        //    string result = "";
        //    try
        //    {
        //        con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
        //        SqlCommand cmd = new SqlCommand("ListingMenuSP", con);  //CreateAdminUserSP
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@type", type);
        //        cmd.Parameters.AddWithValue("@menuid", menuid);
        //        cmd.Parameters.Add("@outstatus", SqlDbType.Int).Direction = ParameterDirection.Output;
        //        con.Open();
        //        result = cmd.ExecuteNonQuery().ToString();
        //        OutStatus = (int)cmd.Parameters["@outstatus"].Value;
        //        return result;

        //    }
        //    catch (Exception ex)
        //    {
        //        OutStatus = -1;
        //        return result = "";
        //    }
        //    finally
        //    {
        //        con.Close();
        //    }
        //}


        #endregion Menu Master

        #region Admin User Master      



        public string ListingUser(int type, int menuid, out int OutStatus)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("ListingUserSP", con);  //ListingUserSP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@id", menuid);
                cmd.Parameters.Add("@outstatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                OutStatus = (int)cmd.Parameters["@outstatus"].Value;
                return result;

            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }


        public DataSet GetAllAdminUser(int id, string search)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("GetAllAdminUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
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

        public string CreateAdminUser(AdminUserModel model, out int OutStatus)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("CreateAdminUserSP", con);  //CreateAdminUserSP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", model.id);
                cmd.Parameters.AddWithValue("@user", model.user);
                cmd.Parameters.AddWithValue("@pass", model.pass);
                cmd.Parameters.AddWithValue("@PAccessRight", model.PAccessRight);
                cmd.Parameters.AddWithValue("@Usertype", model.Usertype);
                cmd.Parameters.AddWithValue("@Dist_Allow", model.Dist_Allow);
                cmd.Parameters.AddWithValue("@User_fullnm", model.User_fullnm);
                cmd.Parameters.AddWithValue("@Designation", model.Designation);
                cmd.Parameters.AddWithValue("@Branch", model.Branch);
                cmd.Parameters.AddWithValue("@Mobno", model.Mobno);
                cmd.Parameters.AddWithValue("@Remarks", model.Remarks);
                cmd.Parameters.AddWithValue("@STATUS", model.STATUS);
                cmd.Parameters.AddWithValue("@EmailID", model.EmailID);
                cmd.Parameters.AddWithValue("@SAccessRight", model.SAccessRight);
                cmd.Parameters.AddWithValue("@ActionRight", model.ActionRight);
                cmd.Parameters.AddWithValue("@utype", model.utype);
                cmd.Parameters.AddWithValue("@Set_Allow", model.Set_Allow);
                cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                return result;

            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }

        public string AssignMenuToUser(string adminid, string adminlist, string pagelist, out string OutError)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("AssignMenuToUserSP", con);  //CreateAdminUserSP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@adminlist", adminlist);
                cmd.Parameters.AddWithValue("@pagelist", pagelist);
                cmd.Parameters.AddWithValue("@adminid", adminid);
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                OutError = (string)cmd.Parameters["@OutError"].Value;
                return result;

            }
            catch (Exception ex)
            {
                OutError = "";
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }


        public DataSet GetAdminIdWithMenuId(int MenuId)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAdminIdWithMenuIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MenuId", MenuId);
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


        //------------------------Delete Data------------------
        public string DeleteAdminUser(string id)
        {
            SqlConnection con = null;
            string result = "";
            try
            {

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("DeleteAdminUser", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                con.Open();
                result = cmd.ExecuteScalar().ToString();
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


        #endregion Admin User Master

        //
        #region DB Recheck Conduct Branch
        public DataSet ConductPrintList(string ccls, string clot, string ser)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ConductPrintList_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;                 
                    cmd.Parameters.AddWithValue("@Class", ccls);
                    cmd.Parameters.AddWithValue("@lot", clot);
                    cmd.Parameters.AddWithValue("@ser", ser);
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
        public DataSet ConductPrintList_New(string ccls, string clot, string ser)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ConductPrintList_New_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;                  
                    cmd.Parameters.AddWithValue("@Class", ccls);
                    cmd.Parameters.AddWithValue("@lot", clot);
                    cmd.Parameters.AddWithValue("@ser", ser);
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
        public DataSet RecheckConductPrint(AdminModels am, string LoginUser, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //SqlCommand cmd = new SqlCommand("GetFirmFinalPrint_SP", con);
                    SqlCommand cmd = new SqlCommand("RecheckConductPrint_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LoginUser", LoginUser);
                    //cmd.Parameters.AddWithValue("@CorrectionType", am.CorrectionType);
                    //cmd.Parameters.AddWithValue("@CorrectionFromDate", am.CorrectionFromDate);
                    //cmd.Parameters.AddWithValue("@CorrectionToDate", am.CorrectionToDate);

                    ////cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                    ////cmd.Parameters.AddWithValue("@PageSize", 20);
                    ////cmd.Parameters.AddWithValue("@search", userNM);
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
        public DataSet RecheckConductCreateLot(AdminModels am, string LoginUser, string Cls)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //SqlCommand cmd = new SqlCommand("GetFirmFinalPrint_SP", con);
                    SqlCommand cmd = new SqlCommand("RecheckConductCreateLot_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LoginUser", LoginUser);
                    cmd.Parameters.AddWithValue("@Cls", Cls);
                    //cmd.Parameters.AddWithValue("@CorrectionType", am.CorrectionType);
                    //cmd.Parameters.AddWithValue("@CorrectionFromDate", am.CorrectionFromDate);
                    //cmd.Parameters.AddWithValue("@CorrectionToDate", am.CorrectionToDate);

                    ////cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                    ////cmd.Parameters.AddWithValue("@PageSize", 20);
                    ////cmd.Parameters.AddWithValue("@search", userNM);
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

        public DataSet ConductUpdateList(string updtStrY, string updtStrN)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ConductUpdateList_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@updtStrY", updtStrY);
                    cmd.Parameters.AddWithValue("@updtStrN", updtStrN);
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
        public DataSet ConductUpdateListRemove(string updtStrY, string updtStrN)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ConductUpdateListRemove_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@updtStrY", updtStrY);
                    cmd.Parameters.AddWithValue("@updtStrN", updtStrN);
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
        public DataSet ConductUpdateListFS(string updtStrY, string updtStrN)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ConductUpdateListFS_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@updtStrY", updtStrY);
                    cmd.Parameters.AddWithValue("@updtStrN", updtStrN);
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
        public DataSet RecheckUpdateList(string ser)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("RecheckUpdateList_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ser", ser);
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
        public DataSet CompartmentStatus(string ser)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CompartmentStatus_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ser", ser);
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

        #endregion DB Recheck Conduct Branch
        //


        //
        #region DB Recheck Secrecy Branch

        public DataSet SecrecyViewList(string ccls, string clot, string ser)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SecrecyViewList", con);//SecrecyPrintList_SPTEMP
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Class", ccls);
                    cmd.Parameters.AddWithValue("@lot", clot);
                    cmd.Parameters.AddWithValue("@ser", ser);
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



        public DataSet SecrecyPrintList(string ccls, string clot, string ser, string searchstring)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SecrecyPrintList_SP", con);//SecrecyPrintList_SPTEMP
                    cmd.CommandType = CommandType.StoredProcedure;                   
                    cmd.Parameters.AddWithValue("@Class", ccls);
                    cmd.Parameters.AddWithValue("@lot", clot);
                    cmd.Parameters.AddWithValue("@ser", ser);
                    cmd.Parameters.AddWithValue("@searchstring", searchstring);
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
        public DataSet RecheckSecrecyPrint(AdminModels am, string LoginUser, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("RecheckSecrecyPrint_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LoginUser", LoginUser);
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
        public DataSet RecheckSecrecyCreateLot(AdminModels am, string LoginUser, string Cls)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("RecheckSecrecyCreateLot_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@LoginUser", LoginUser);
                    cmd.Parameters.AddWithValue("@Cls", Cls);
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
        public DataSet GetSecrecyUpdateListRemove(string ccls, string clot, string ser)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetSecrecyUpdateListRemove_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Class", ccls);
                    cmd.Parameters.AddWithValue("@lot", clot);
                    cmd.Parameters.AddWithValue("@ser", ser);
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
        public DataSet SecrecyUpdateListRemove(string updtStrY, string updtStrN)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SecrecyUpdateListRemove_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@updtStrY", updtStrY);
                    cmd.Parameters.AddWithValue("@updtStrN", updtStrN);
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
        #endregion DB Recheck Secrecy Branch
        //

        //
        #region   Download Compartment Data Firm    
        public string CheckComptDataMis(DataSet ds, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("Status", typeof(string)));
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string ColName = ds.Tables[0].Columns[0].ColumnName.ToString();
                    if (ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string REFNO = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check REFNO " + REFNO + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += "Please check REFNO " + REFNO + " in row " + RowNo + ",  ";

                    }
                    else
                    {
                        DataSet ds1 = GetDataByIdandTypeRN(ds.Tables[0].Rows[i][0].ToString(), "P", ds.Tables[0].Rows[i][0].ToString());// Regular
                        if (ds1 == null || ds1.Tables.Count == 0 || ds1.Tables[0].Rows.Count == 0)
                        {
                            int RowNo = i + 2;
                            string REFNO = ds.Tables[0].Rows[i][0].ToString();
                            Result += "Please check REFNO " + REFNO + " Not Found in row " + RowNo + ",  ";
                            dt.Rows[i]["Status"] += "Please check REFNO " + REFNO + "Not Found in row " + RowNo + ",  ";
                        }
                        else
                        {
                            if (ds1.Tables[0].Rows[0]["challanverify"].ToString().ToLower() != "true")
                            {
                                int RowNo = i + 2;
                                string REFNO = ds.Tables[0].Rows[i][0].ToString();
                                Result += "Please check REFNO " + REFNO + " : Chln Not Verified in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] += "Please check REFNO " + REFNO + " : Chln Not Verified in row " + RowNo + ",  ";

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
            return Result;
        }
        //  Download By Lot
        public DataSet GetPvtCompDataByFirmLOT(AdminModels am, int adminid, string firm, int DOWNLOT, out int OutStatus)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            SqlCommand cmd;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    cmd = new SqlCommand("GetPvtCompDataByFirmLOTSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@batch", am.ID);
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@firm", firm);
                    cmd.Parameters.AddWithValue("@DOWNLOT", DOWNLOT);
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
        //  Download pending
        public DataSet GetPvtCompDataByFirmPending(AdminModels am, int adminid, string firm, string search, int Type, out int OutStatus)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            SqlCommand cmd;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    cmd = new SqlCommand("GetPvtCompDataByFirmPendingSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@batch", am.ID);
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@firm", firm);
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



        #endregion  Download Compartment Data Firm
        //

        #region   AllotRegNo OPEN

        public DataSet GetPendingREGNOByAdminId(string Classallow, int adminid, string Type, out int OutStatus)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetPendingREGNOByAdminIdSP", con);  //insertinbulkexammasterregular2017
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Class", Classallow);
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@Type", Type);
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
        public string CheckAllotRegNoOPENMis(string chkbtn, DataSet ds, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("STATUS", typeof(string)));

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string ColName = ds.Tables[0].Columns[0].ColumnName.ToString();
                    if (ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string CANDID = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check CANDID " + CANDID + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check CANDID " + CANDID + " in row " + RowNo + ",  ";

                    }
                    else
                    {
                        if (chkbtn.ToLower().Contains("generate"))
                        {
                            string CANDID = ds.Tables[0].Rows[i][0].ToString();
                            //  OpenUserRegistration _openUserRegistration = openDB.GetRegistrationRecord(_openUserLogin.ID.ToString());
                            OpenUserRegistration _openUserRegistration = new AbstractLayer.OpenDB().GetRegistrationRecord(CANDID.ToString());

                            if (_openUserRegistration == null || string.IsNullOrEmpty(_openUserRegistration.APPNO) || _openUserRegistration.ID == 0)
                            {
                                int RowNo = i + 2;
                                Result += "Please check CANDID " + CANDID + " NotFound in row " + RowNo + ",   ";
                                dt.Rows[i]["Status"] = "Please check CANDID " + CANDID + " NotFound in row " + RowNo + ",  ";
                            }
                            else
                            {
                                if (_openUserRegistration.REGNO.Trim() != "")
                                {
                                    int RowNo = i + 2;
                                    // dt.Rows[i]["ErrDetails"] += "Total Fee Error";
                                    string challanid = dt.Rows[i][0].ToString();
                                    Result += "Please check CANDID " + CANDID + " of REGNO Already Assigned in row " + RowNo + ",   ";
                                    dt.Rows[i]["Status"] += " , " + "Please check CANDID " + CANDID + " REGNO Already Assigned in row " + RowNo + ",   ";
                                }
                            }
                        }
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

        public DataSet AutomaticAllotRegnoOPEN(int AdminId, string storeid, out int OutStatus, out int REGNOLOT, out string OutResult)
        {
            DataSet dataTable = new DataSet();
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            SqlConnection con = null;
            SqlCommand cmd;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                if (CommonCon.ToString() == "myConn")
                { cmd = new SqlCommand("AutomaticAllotRegnoOPENSP", con); }
                else
                {
                    cmd = new SqlCommand("AutomaticAllotRegnoOPENSP17RN", con);//AutomaticAllotRegnoOPENSP17
                    cmd.Parameters.AddWithValue("@userid", AdminId);
                }
                //insertinbulkexammasterregular2017
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.Parameters.AddWithValue("@storeid", storeid);
                cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@REGNOLOT", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@OutResult", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                con.Open();
                // result = cmd.ExecuteNonQuery().ToString();
                dataAdapter.SelectCommand = cmd;
                dataAdapter.Fill(dataTable);
                OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                REGNOLOT = (int)cmd.Parameters["@REGNOLOT"].Value;
                OutResult = (string)cmd.Parameters["@OutResult"].Value;
                // return result;
                return dataTable;

            }
            catch (Exception ex)
            {
                OutResult = ex.Message;
                OutStatus = -1;
                REGNOLOT = -1;
                return null;
                // return result = "";
            }
            finally
            {
                con.Close();
            }
        }


        public DataSet GetAllotRegnoOPENByLOT(int adminid, string Type, string classallow, int REGNOLOT, out int OutStatus)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            SqlCommand cmd;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    if (CommonCon.ToString() == "myConn")
                    { cmd = new SqlCommand("GetAllotRegnoOPENByLOTSP", con); }
                    else
                    { cmd = new SqlCommand("GetAllotRegnoOPENByLOTSP17", con); }
                    //  SqlCommand cmd = new SqlCommand("GetAllotRegnoOPENByLOTSP", con);  //insertinbulkexammasterregular2017
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@Class", classallow);
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@REGNOLOT", REGNOLOT);
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


        public DataSet DownloadRegNoAgainstID(string storeid, string type, out int OutStatus)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            SqlCommand cmd;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    cmd = new SqlCommand("DownloadRegNoAgainstIDSP", con);  //GetAllotRegnoOPENByLOTSP17
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@storeid", storeid);
                    cmd.Parameters.AddWithValue("@Type", type);
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

        #endregion  AllotRegNo OPEN

        #region  StudentRollNoMIS

        public string CheckStdRollNoMisOnly(DataSet ds, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("Status", typeof(string)));

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string ColName = ds.Tables[0].Columns[1].ColumnName.ToString();
                    if (ds.Tables[0].Rows[i][0].ToString().Length < 7 || ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string SCHL = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check SCHL " + SCHL + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check SCHL " + SCHL + " in row " + RowNo + ",  ";
                    }

                    if (ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string CANDID = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check CANDID " + CANDID + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += "Please check CANDID " + CANDID + " in row " + RowNo + ",  ";

                    }

                    DataSet ds1 = GetDataByIdandTypeRN(ds.Tables[0].Rows[i][1].ToString(), "R", ds.Tables[0].Rows[i][2].ToString());// Regular
                    if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                    {
                        if (ds1.Tables[0].Rows[0]["schl"].ToString() != ds.Tables[0].Rows[i][0].ToString())
                        {
                            int RowNo = i + 2;
                            string schl = ds.Tables[0].Rows[i][0].ToString();
                            Result += "Please check CandId and SCHL Not Matched in row " + RowNo + ",  ";
                            dt.Rows[i]["Status"] += "Please check CandId and SCHL Not Matched in row " + RowNo + ",  ";
                        }
                        else
                        {

                            if (ds.Tables[0].Rows[i][2].ToString().Length != 10 || ds.Tables[0].Rows[i][2].ToString() == "")
                            {
                                int RowNo = i + 2;
                                string roll = ds.Tables[0].Rows[i][2].ToString();
                                Result += "Please check ROLL No " + roll + " in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] += "Please check ROLL No " + roll + " in row " + RowNo + ",  ";
                            }
                            else
                            {
                                // check roll duplicacy
                                DataSet ds2 = GetDataByIdandTypeRN(ds.Tables[0].Rows[i][1].ToString(), "RR", ds.Tables[0].Rows[i][2].ToString());// Regular
                                if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                {
                                    int RowNo = i + 2;
                                    string roll = ds.Tables[0].Rows[i][2].ToString();
                                    Result += "Duplicate roll No " + roll + " in row " + RowNo + ",  ";
                                    dt.Rows[i]["Status"] += "Duplicate Roll No " + roll + " in row " + RowNo + ",  ";
                                }
                            }                           
                            if (ds1.Tables[0].Rows[0]["Examchallanverify"].ToString().ToLower() != "true")
                            {
                                int RowNo = i + 2;
                                string Std_id = ds.Tables[0].Rows[i][0].ToString();
                                Result += "Please check Refno " + Std_id + " : Challan Not Verified in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] += "Please check Refno " + Std_id + " : Challan Not Verified in row " + RowNo + ",  ";

                            }
                        }
                    }
                    else
                    {
                        int RowNo = i + 2;
                        string CANDID = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check CANDID " + CANDID + " Not Found in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += "Please check CANDID " + CANDID + "  Not Found  in row " + RowNo + ",  ";

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


        public string CheckStdRollNoMis(DataSet ds, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("Status", typeof(string)));

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string ColName = ds.Tables[0].Columns[1].ColumnName.ToString();
                    if (ds.Tables[0].Rows[i][0].ToString().Length < 7 || ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string SCHL = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check SCHL " + SCHL + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check SCHL " + SCHL + " in row " + RowNo + ",  ";
                    }

                    if (ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string CANDID = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check CANDID " + CANDID + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += "Please check CANDID " + CANDID + " in row " + RowNo + ",  ";

                    }

                    DataSet ds1 = GetDataByIdandTypeRN(ds.Tables[0].Rows[i][1].ToString(), "R", ds.Tables[0].Rows[i][2].ToString());// Regular
                    if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                    {
                        if (ds1.Tables[0].Rows[0]["schl"].ToString() != ds.Tables[0].Rows[i][0].ToString())
                        {
                            int RowNo = i + 2;
                            string schl = ds.Tables[0].Rows[i][0].ToString();
                            Result += "Please check CandId and SCHL Not Matched in row " + RowNo + ",  ";
                            dt.Rows[i]["Status"] += "Please check CandId and SCHL Not Matched in row " + RowNo + ",  ";
                        }
                        else
                        {

                            if (ds.Tables[0].Rows[i][2].ToString().Length != 10 || ds.Tables[0].Rows[i][2].ToString() == "")
                            {
                                int RowNo = i + 2;
                                string roll = ds.Tables[0].Rows[i][2].ToString();
                                Result += "Please check ROLL No " + roll + " in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] += "Please check ROLL No " + roll + " in row " + RowNo + ",  ";
                            }
                            else
                            {
                                // check roll duplicacy
                                DataSet ds2 = GetDataByIdandTypeRN(ds.Tables[0].Rows[i][1].ToString(), "RR", ds.Tables[0].Rows[i][2].ToString());// Regular
                                if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                {
                                    int RowNo = i + 2;
                                    string roll = ds.Tables[0].Rows[i][2].ToString();
                                    Result += "Duplicate roll No " + roll + " in row " + RowNo + ",  ";
                                    dt.Rows[i]["Status"] += "Duplicate Roll No " + roll + " in row " + RowNo + ",  ";
                                }
                            }

                            if (ds.Tables[0].Rows[i][3].ToString().Length < 5 || ds.Tables[0].Rows[i][3].ToString() == "")
                            {
                                int RowNo = i + 2;
                                string cent = ds.Tables[0].Rows[i][3].ToString();
                                Result += "Please check CENT " + cent + " in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] += "Please check CENT " + cent + " in row " + RowNo + ",  ";
                            }

                            if (ds1.Tables[0].Rows[0]["Examchallanverify"].ToString().ToLower() != "true")
                            {
                                int RowNo = i + 2;
                                string Std_id = ds.Tables[0].Rows[i][0].ToString();
                                Result += "Please check Refno " + Std_id + " : Challan Not Verified in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] += "Please check Refno " + Std_id + " : Challan Not Verified in row " + RowNo + ",  ";

                            }
                        }
                    }
                    else
                    {
                        int RowNo = i + 2;
                        string CANDID = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check CANDID " + CANDID + " Not Found in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += "Please check CANDID " + CANDID + "  Not Found  in row " + RowNo + ",  ";

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

        public DataTable StudentRollNoMIS(DataTable dt1, int adminid, out int OutStatus)  // ExamErrorListMISSP
        {

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("StudentRollNoMISSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 250;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@tblStudentRollNo", dt1);
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



        public DataTable StudentRollNoMISONLY(DataTable dt1, int adminid, out int OutStatus)  // ExamErrorListMISSP
        {

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("StudentRollNoMISONLYSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 200;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@tblStudentRollNoOnly", dt1);
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

        public string CheckStdRollNoRangeMis(DataSet ds, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("Status", typeof(string)));

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][1].ToString().Length < 7 || ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string SCHL = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check SCHL " + SCHL + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check SCHL " + SCHL + " in row " + RowNo + ",  ";

                    }
                    if (ds.Tables[0].Rows[i][1].ToString().Length != 10 || ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string roll = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check Start ROLL No " + roll + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += "Please check Start ROLL No " + roll + " in row " + RowNo + ",  ";
                    }

                    if (ds.Tables[0].Rows[i][2].ToString().Length != 10 || ds.Tables[0].Rows[i][2].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string roll = ds.Tables[0].Rows[i][2].ToString();
                        Result += "Please check End ROLL No " + roll + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += "Please check End ROLL No " + roll + " in row " + RowNo + ",  ";
                    }

                    if (ds.Tables[0].Rows[i][3].ToString().Length < 5 || ds.Tables[0].Rows[i][3].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string cent = ds.Tables[0].Rows[i][3].ToString();
                        Result += "Please check CENT " + cent + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += "Please check CENT " + cent + " in row " + RowNo + ",  ";
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

        public DataTable StudentRollNoRangeMIS(DataTable dt1, int adminid, out int OutStatus)  // ExamErrorListMISSP
        {

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("StudentRollNoRangeMISSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@tblStudentRollNoRange", dt1);
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

        public string CheckStdRollNoMisPvt(DataSet ds, string firm, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("STATUS", typeof(string)));

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string ColName = ds.Tables[0].Columns[1].ColumnName.ToString();
                    if (ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string OROLL = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check OROLL " + OROLL + "  in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check OROLL " + OROLL + "  in row " + RowNo + ",  ";
                    }

                    // string ColName = ds.Tables[0].Columns[1].ColumnName.ToString();

                    if (ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string REFNO = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check REFNO " + REFNO + "   in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check REFNO " + REFNO + " in row " + RowNo + ",  ";
                    }

                    if (ds.Tables[0].Rows[i][2].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string REFNO = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check REFNO " + REFNO + " , ROLL Can't Be Blank in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check REFNO " + REFNO + " ROLL Can't Be Blank in row " + RowNo + ",  ";
                    }

                    if (ds.Tables[0].Rows[i][3].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string REFNO = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check REFNO " + REFNO + " , CENT Can't Be Blank in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check REFNO " + REFNO + " CENT Can't Be Blank in row " + RowNo + ",  ";
                    }

                    DataSet ds1 = GetDataByIdandTypeRN(ds.Tables[0].Rows[i][1].ToString(), "P", ds.Tables[0].Rows[i][2].ToString());// Regular
                    if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                    {
                        if (ds1.Tables[0].Rows[0]["roll"].ToString() != ds.Tables[0].Rows[i][0].ToString())
                        {
                            int RowNo = i + 2;
                            string schl = ds.Tables[0].Rows[i][1].ToString();
                            Result += "Please check OROLL and REFNO are  Not Matched in row " + RowNo + ",  ";
                            dt.Rows[i]["Status"] += "Please check OROLL and REFNO are Not Matched in row " + RowNo + ",  ";
                        }
                        else
                        {
                            // string ColName = ds.Tables[0].Columns[1].ColumnName.ToString();

                         
                            if (ds.Tables[0].Rows[i][2].ToString().Length != 10 || ds.Tables[0].Rows[i][2].ToString() == "")
                            {
                                int RowNo = i + 2;
                                string roll = ds.Tables[0].Rows[i][2].ToString();
                                Result += "Please check ROLL No " + roll + " (Must be of 10 Digits) in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] = "Please check ROLL No " + roll + "  (Must be of  10 Digits) in row " + RowNo + ",  ";
                            }
                            else
                            {
                                // check roll duplicacy
                                DataSet ds2 = GetDataByIdandTypeRN(ds.Tables[0].Rows[i][1].ToString(), "PR", ds.Tables[0].Rows[i][2].ToString());// Regular
                                if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                {
                                    int RowNo = i + 2;
                                    string roll = ds.Tables[0].Rows[i][2].ToString();
                                    Result += "Duplicate roll No " + roll + " in row " + RowNo + ",  ";
                                    dt.Rows[i]["Status"] += "Duplicate Roll No " + roll + " in row " + RowNo + ",  ";
                                }
                            }

                            if (ds.Tables[0].Rows[i][3].ToString().Length != 5 || ds.Tables[0].Rows[i][3].ToString() == "")
                            {
                                int RowNo = i + 2;
                                string cent = ds.Tables[0].Rows[i][3].ToString();
                                Result += "Please check CENT " + cent + " (Must be 5 Digits) in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] = "Please check CENT " + cent + " (Must be 5 Digits) in row " + RowNo + ",  ";
                            }

                            if (ds1.Tables[0].Rows[0]["Examchallanverify"].ToString().ToLower() != "true")
                            {
                                int RowNo = i + 2;
                                string Std_id = ds.Tables[0].Rows[i][0].ToString();
                                Result += "Please check Refno " + Std_id + " : Challan Not Verified in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] += "Please check Refno " + Std_id + " : Challan Not Verified in row " + RowNo + ",  ";

                            }
                        }
                    }
                    else
                    {
                        int RowNo = i + 2;
                        string REFNO = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check REFNO " + REFNO + "  Not Found in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check REFNO " + REFNO + "  Not Found in row " + RowNo + ",  ";
                    }


                    if (ds.Tables[0].Rows[i][4].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string REFNO = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check REFNO " + REFNO + " , SET Can't Be Blank in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check REFNO " + REFNO + " SET Can't Be Blank in row " + RowNo + ",  ";
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

        public string CheckStdRollNoMisPvtDown(DataSet ds, string firm, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("STATUS", typeof(string)));

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string ColName = ds.Tables[0].Columns[1].ColumnName.ToString();
                    if (ds.Tables[0].Rows[i][0].ToString().Length != 13 || ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string REFNO = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check REFNO  " + REFNO + " (Must be 13 Digits)  in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check REFNO  " + REFNO + " (Must be 13 Digits)  in row " + RowNo + ",  ";
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
        public DataTable StudentRollNoMISPVT(DataTable dt1, int adminid, out string OutResult)  // ExamErrorListMISSP
        {

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("StudentRollNoMISSPPVT", con);//StudentRollNoMISSP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 200;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@tblStudentRollNoPvt", dt1);
                    cmd.Parameters.Add("@OutResult", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutResult = (string)cmd.Parameters["@OutResult"].Value;
                    return dataTable;

                }
            }
            catch (Exception ex)
            {
                OutResult = "-1";
                return null;
            }
        }

        //  Download pending
        public DataSet StudentRollNoMISPVTDown(int adminid, string firm, string search, int Type, out int OutStatus)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            SqlCommand cmd;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    cmd = new SqlCommand("GetPvtCompDataByFirmPendingSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@firm", firm);
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


        #endregion  StudentRollNoMIS
        //

        //
        #region Open Image download
        public string FirmDownloadStudentRecordOpen(string Std_id)
        {
            string DwonSp = System.Configuration.ConfigurationManager.AppSettings["Download"];
            string ImagePathCor = WebConfigurationManager.AppSettings["ImagePathCor"].ToString();
            string result = "0";
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                foreach (string str in FirmDownloadStudentRecordOpenPS(Std_id))
                {
                    string filelocation = "";

                    if (str.ToString().ToUpper().Contains("CORR"))
                    {
                        filelocation = Path.Combine(ImagePathCor + "/", str);
                    }
                    else
                    {
                        filelocation = Path.Combine(HttpContext.Current.Server.MapPath("~/"), str);
                    }

                    if (File.Exists(filelocation) == true)
                    {
                        zip.AddFile(filelocation, "PhotoandSign");
                    }
                    else
                    {
                        //Response.Write("Missing Image :" + str);
                        // zip.AddFile(filelocation, "ImgandSing");
                        //  zip.AddFile(@"Images\NOPhoto.jpg");
                    }
                }
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.BufferOutput = false;
                string zipName = String.Format("Zip" + "ImageFile_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                HttpContext.Current.Response.ContentType = "application/zip";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                zip.Save(HttpContext.Current.Response.OutputStream);
                HttpContext.Current.Response.End();
                result = "1";
            }
            return result;
        }
        private string[,] FirmDownloadStudentRecordOpenPS(string Std_id)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("InsertPhotoSignPath_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Std_id", Std_id);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();

                    string[,] arrvalues = new string[2, result.Tables[0].Rows.Count];
                    //loopcounter
                    for (int loopcounter = 0; loopcounter < result.Tables[0].Rows.Count; loopcounter++)
                    {
                        //assign dataset values to array
                        arrvalues[0, loopcounter] = result.Tables[0].Rows[loopcounter]["Photo"].ToString();
                        arrvalues[1, loopcounter] = result.Tables[0].Rows[loopcounter]["Sign"].ToString();
                    }
                    return arrvalues;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            { SqlCon.Close(); }
        }
        #endregion Open Image download
        //
        //
        #region Start Firm photo and sign upload
        public DataSet FirmUpdated_Bulk_Pic_Data(string Myresult, string PhotoSignName, string Type, string FirmNM)
        {
            SqlConnection con = null;
            try
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                DataSet dataTable = new DataSet();
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("FirmUploaded_Bulk_Photo_Sign", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@examROLL", Myresult);
                cmd.Parameters.AddWithValue("@PhotoSignName", PhotoSignName);
                cmd.Parameters.AddWithValue("@Type", Type);
                cmd.Parameters.AddWithValue("@FirmNM", FirmNM);

                con.Open();
                dataAdapter.SelectCommand = cmd;
                dataAdapter.Fill(dataTable);
                return dataTable;
            }
            catch (Exception ex)
            {
                //mbox(ex);
                return null;
            }
            finally
            {
                con.Close();
            }
        }

        #endregion Firm photo and sign upload
        //
        #region ExamErrorListMIS

        public DataSet GetDataByIdandTypeRN(string id, string type, string roll)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetDataByIdandTypeRN", con);// GetDataByIdandTypeRN
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@roll", roll);
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

        public string CheckExamMisExcel(DataSet ds, out DataTable dt)
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
                        string CANDID = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check CANDID " + CANDID + " in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] = "Please check CANDID  " + CANDID + " in row " + RowNo + ",  ";

                    }

                    if (ds.Tables[0].Rows[i][1].ToString().Length != 7 || ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string schl = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check SCHL " + schl + " in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] += "Please check SCHL " + schl + " in row " + RowNo + ",  ";
                    }

                    //get data of candidate id 
                    DataSet ds1 = GetDataByIdandTypeRN(ds.Tables[0].Rows[i][0].ToString(), "R", ds.Tables[0].Rows[i][0].ToString());// Regular
                    if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                    {
                        if (ds1.Tables[0].Rows[0]["schl"].ToString() != ds.Tables[0].Rows[i][1].ToString())
                        {
                            int RowNo = i + 2;
                            string schl = ds.Tables[0].Rows[i][1].ToString();
                            Result += "Please check CandId and SCHL Not Matched in row " + RowNo + ",  ";
                            dt.Rows[i]["ErrStatus"] += "Please check CandId and SCHL Not Matched in row " + RowNo + ",  ";
                        }
                        else
                        {
                            if (ds.Tables[0].Rows[i][2].ToString() == "")
                            {
                                int RowNo = i + 2;
                                string errcode = ds.Tables[0].Rows[i][2].ToString();
                                Result += "Please check ERRCODE " + errcode + " in row " + RowNo + ",  ";
                                dt.Rows[i]["ErrStatus"] += "Please check ERRCODE " + errcode + " in row " + RowNo + ",  ";
                            }

                            if (ds.Tables[0].Rows[i][3].ToString() == "")
                            {
                                int RowNo = i + 2;
                                string errcode = ds.Tables[0].Rows[i][3].ToString();
                                Result += "Please check STATUS " + errcode + " in row " + RowNo + ",  ";
                                dt.Rows[i]["ErrStatus"] += "Please  check STATUS " + errcode + " in row " + RowNo + ",  ";
                            }

                            if (ds1.Tables[0].Rows[0]["Examchallanverify"].ToString().ToLower() != "true")
                            {
                                int RowNo = i + 2;
                                string Std_id = ds.Tables[0].Rows[i][0].ToString();
                                Result += "Please check Refno " + Std_id + " : Challan Not Verified in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] += "Please check Refno " + Std_id + " : Challan Not Verified in row " + RowNo + ",  ";

                            }
                        }

                    }
                    else
                    {
                        int RowNo = i + 2;
                        string CANDID = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check CANDID " + CANDID + " Not Found in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] = "Please check CANDID  " + CANDID + "  Not Found  in row " + RowNo + ",  ";

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

        public DataTable ExamErrorListMIS(DataTable dt1, int adminid, out int OutStatus)  // ExamErrorListMISSP
        {

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ExamErrorListMISSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 200;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@tblCandExamError", dt1);
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



        public string CheckExamMisExcelPVT(DataSet ds, string firm, out DataTable dt)
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
                        string OROLL = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check OROLL " + OROLL + " in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] = "Please check OROLL " + OROLL + "  in row " + RowNo + ",  ";
                    }

                    if (ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string REFNO = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check REFNO " + REFNO + " in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] = "Please check REFNO " + REFNO + " in row " + RowNo + ",  ";
                    }


                    DataSet ds1 = GetDataByIdandTypeRN(ds.Tables[0].Rows[i][1].ToString(), "P", ds.Tables[0].Rows[i][1].ToString());// Regular
                    if (ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                    {
                        if (ds1.Tables[0].Rows[0]["Roll"].ToString() != ds.Tables[0].Rows[i][0].ToString())
                        {
                            int RowNo = i + 2;
                            string schl = ds.Tables[0].Rows[i][1].ToString();
                            Result += "Please check OROLL and REFNO Not Matched in row " + RowNo + ",  ";
                            dt.Rows[i]["ErrStatus"] += "Please check OROLL and REFNO Not Matched in row " + RowNo + ",  ";
                        }
                        else
                        {
                            if (ds.Tables[0].Rows[i][2].ToString() == "")
                            {
                                int RowNo = i + 2;
                                string errcode = ds.Tables[0].Rows[i][2].ToString();
                                Result += "Please check ERRCODE " + errcode + " in row " + RowNo + ",  ";
                                dt.Rows[i]["ErrStatus"] = "Please check ERRCODE " + errcode + " in row " + RowNo + ",  ";
                            }

                            if (ds.Tables[0].Rows[i][3].ToString() == "")
                            {
                                int RowNo = i + 2;
                                string errcode = ds.Tables[0].Rows[i][3].ToString();
                                Result += "Please check STATUS " + errcode + " in row " + RowNo + ",  ";
                                dt.Rows[i]["ErrStatus"] = "Please check STATUS " + errcode + " in row " + RowNo + ",  ";
                            }

                            if (ds1.Tables[0].Rows[0]["Examchallanverify"].ToString().ToLower() != "true")
                            {
                                int RowNo = i + 2;
                                string Std_id = ds.Tables[0].Rows[i][0].ToString();
                                Result += "Please check Refno " + Std_id + " : Challan Not Verified in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] += "Please check Refno " + Std_id + " : Challan Not Verified in row " + RowNo + ",  ";

                            }
                        }
                    }
                    else
                    {
                        int RowNo = i + 2;
                        string REFNO = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check REFNO " + REFNO + " Not Found in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] = "Please check REFNO " + REFNO + "   Not Found  in row " + RowNo + ",  ";
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

        public DataTable ExamErrorListMISPVT(DataTable dt1, int adminid, out string OutResult)  // ExamErrorListMISSP
        {

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ExamErrorListMISSPPVT", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 200;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@tblCandExamErrorPvt", dt1);
                    cmd.Parameters.Add("@OutResult", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutResult = (string)cmd.Parameters["@OutResult"].Value;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                OutResult = "0";
                return null;
            }
        }


        public DataSet GetErrorListMISByFirmId(int type, int adminid, out string OutResult)
        {

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetErrorListMISByFirmId", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.Add("@OutResult", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(dataTable);
                    OutResult = (string)cmd.Parameters["@OutResult"].Value;
                    return dataTable;
                }
            }
            catch (Exception ex)
            {
                OutResult = "0";
                return null;
            }
        }

        #endregion ExamErrorListMIS


        public List<SelectListItem> GetAllFormName()
        {
            List<SelectListItem> formNameList = new List<SelectListItem>();
            formNameList.Add(new SelectListItem { Text = "---Select Form--", Value = "0" });
            formNameList.Add(new SelectListItem { Text = "N1", Value = "N1" });
            formNameList.Add(new SelectListItem { Text = "N2", Value = "N2" });
            formNameList.Add(new SelectListItem { Text = "N3", Value = "N3" });
            formNameList.Add(new SelectListItem { Text = "E1", Value = "E1" });
            formNameList.Add(new SelectListItem { Text = "E2", Value = "E2" });
            formNameList.Add(new SelectListItem { Text = "M1", Value = "M1" });
            formNameList.Add(new SelectListItem { Text = "M2", Value = "M2" });
            formNameList.Add(new SelectListItem { Text = "T1", Value = "T1" });
            formNameList.Add(new SelectListItem { Text = "T2", Value = "T2" });
            return formNameList;
        }

        public int GetClassNumber(string FormName)
        {
            int ClassNo = 0;
            if (FormName == "N1" || FormName == "N2" || FormName == "N3")
            { ClassNo = 9; }
            if (FormName == "M1" || FormName == "M2")
            { ClassNo = 10; }
            if (FormName == "E1" || FormName == "E2")
            { ClassNo = 11; }
            if (FormName == "T1" || FormName == "T2")
            { ClassNo = 12; }
            return ClassNo;
        }

        public DataSet GetFeeCodeMaster(int Type, int FeeCode)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetFeeCodeMaster", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@FeeCode", FeeCode);
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

        #region  BulkFeeMaster
        public DataTable BulkFeeMaster(DataTable dt1, int adminid, out int OutStatus)  // BulkChallanBank
        {
            string OutError = "";
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BulkFeeMasterSP", con); // BulkChallanBankSPTest
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ADMINID", adminid);
                    cmd.Parameters.AddWithValue("@BulkFeeMaster", dt1);
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
        #endregion  BulkChallanBank


        public DataSet GetAllFeeMaster2016(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAllFeeMaster2016SP", con);
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


     


        #region Fee Master
        public int Insert_FeeMaster(FeeModels FM)
        {
            SqlConnection con = null;
            int result = 0;
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("InsertFeeMaster2016SP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ID", FM.ID);
                cmd.Parameters.AddWithValue("@Type", FM.Type);
                cmd.Parameters.AddWithValue("@FORM", FM.FORM);
                cmd.Parameters.AddWithValue("@FeeCat", FM.FeeCat);
                cmd.Parameters.AddWithValue("@Class", FM.Class);
                cmd.Parameters.AddWithValue("@StartDate", FM.StartDate);
                cmd.Parameters.AddWithValue("@EndDate", FM.EndDate);
                cmd.Parameters.AddWithValue("@BankLastdate", FM.BankLastDate);
                cmd.Parameters.AddWithValue("@Fee", FM.Fee);
                cmd.Parameters.AddWithValue("@LateFee", FM.LateFee);
                cmd.Parameters.AddWithValue("@FeeCode", FM.FeeCode);
                cmd.Parameters.AddWithValue("@AllowBanks", FM.AllowBanks);
                cmd.Parameters.AddWithValue("@RP", FM.RP);
                cmd.Parameters.AddWithValue("@IsActive", FM.IsActive);
                cmd.Parameters.Add("@OutId", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery();
                int outuniqueid = (int)cmd.Parameters["@OutId"].Value;
                return outuniqueid;

            }
            catch (Exception ex)
            {
                return result = -1;
            }
            finally
            {
                con.Close();
            }
        }
        #endregion

        #region Admin Result
        public DataSet GetSchoolRecordsSearch(string search, string year)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetSchoolRecordsSearchD", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@year", year);

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


        public int UpdateStudentRecords(AdminModels am, string year)
        {
            SqlConnection con = null;
            int result = 0;
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("UpdateStudentRecordsD", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@stdid", am.ID);
                cmd.Parameters.AddWithValue("@TotalMarks", am.TotalMarks);
                cmd.Parameters.AddWithValue("@ObtainedMarks", am.ObtainedMarks);
                cmd.Parameters.AddWithValue("@Result", am.Result);
                cmd.Parameters.AddWithValue("@reclock", am.reclock);
                cmd.Parameters.AddWithValue("@EXAM", am.EXAM);
                cmd.Parameters.AddWithValue("@year", year);
                cmd.Parameters.Add("@OutId", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery();
                //int outuniqueid = (int)cmd.Parameters["@OutId"].Value;
                return result;

            }
            catch (Exception ex)
            {
                return result = -1;
            }
            finally
            {
                con.Close();
            }
        }

        public int FinalsubmitResult(AdminModels am)
        {
            SqlConnection con = null;
            int result = 0;
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("SPFinalsubmitResultnw", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SCHL", am.SchlCode);
                cmd.Parameters.AddWithValue("@SelYear", am.SelYear);
                cmd.Parameters.Add("@OutId", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery();
                //int outuniqueid = (int)cmd.Parameters["@OutId"].Value;
                return result;
            }
            catch (Exception ex)
            {
                return result = -1;
            }
            finally
            {
                con.Close();
            }
        }
        #endregion

        #region Get Admin Challan Details Begin
        public DataSet GetAdminChallanDetailSearch(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAdminChallanDetail", con);
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

        public DataSet GetViewChallanDetail(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SP_GetViewChallanDetail", con);
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
        #endregion Get Admin Challan Details End

        //public void Findduplicacyonregno(string regno, int id, string yr, out int OutStatus)
        //{
        //    SqlConnection con = null;
        //    int result = 0;
        //    try
        //    {
        //        con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
        //        SqlCommand cmd = new SqlCommand("Findduplicacyonregno", con);
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        cmd.Parameters.AddWithValue("@REGNO", regno);
        //        cmd.Parameters.AddWithValue("@id", id);
        //        cmd.Parameters.AddWithValue("@yr", yr);
        //        cmd.Parameters.Add("@OutStatus", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
        //        con.Open();
        //        result = cmd.ExecuteNonQuery();
        //        OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
        //    }
        //    catch (Exception ex)
        //    {
        //        OutStatus = -1;
        //    }
        //    finally
        //    {
        //        con.Close();
        //    }
        //}

        public void Findduplicacyonregno(string remarks, string regno, string GroupNM, int id, string yr, out int OutStatus)
        {
            SqlConnection con = null;
            int result = 0;
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("Findduplicacyonregno", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@remarks", remarks);
                cmd.Parameters.AddWithValue("@REGNO", regno);
                cmd.Parameters.AddWithValue("@GroupNM", GroupNM);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@yr", yr);
                cmd.Parameters.Add("@OutStatus", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery();
                OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
            }
            catch (Exception ex)
            {
                OutStatus = -1;
            }
            finally
            {
                con.Close();
            }
        }
        #region Firm Correction Data START

        public DataSet GetFirmSchoolErrorList(string Search, string FirmNM, string CrType, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("FirmSchoolErrorList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@firmNM", FirmNM);
                    cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
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

        public DataSet GetFirmPrivateCandidateErrorList(int Type, string Search, string FirmNM, string CrType, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //change by rohit on 29012018
                    SqlCommand cmd = new SqlCommand("ExamErrorStatusSP", con);//GetFirmPrivateCandidateErrorList_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@firmNM", FirmNM);
                    cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
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
        public DataSet GetADDErrorPrivateCandidate(string id, string FirmNM)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetADDErrorPrivateCandidate_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@firmNM", FirmNM);
                    cmd.Parameters.AddWithValue("@refno", id);
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
        public DataSet GetRemovePrivateCandidateErrorList(string id, string FirmNM)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetRemovePrivateCandidateErrorList_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@firmNM", FirmNM);
                    cmd.Parameters.AddWithValue("@refno", id);
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
        public DataSet GetFirmSchoolErrorListDownload(string Search, string FirmNM, string CrType, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("FirmSchoolErrorList4Download", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@firmNM", FirmNM);
                    cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
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

        SqlConnection SqlCon = new SqlConnection("server=43.224.136.122,2499;database=firmdata_online;uid=sa;pwd=pU74h5#x;Connect Timeout=1200;");
        public DataSet GetCorrectionDataFirm(string search, string CrType, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCorrectionDataFirm_SP_Cor", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@CrType", CrType);
                    cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
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

        #region
        public DataSet GetCorrectionDataFirmUpdatedByAdmin(string search, string CrType, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCorrectionDataFirmUpdated_SP_Cor_ByAdmin", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
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

        public DataSet GetCorrectionDataFirmUpdated(string search, string CrType, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCorrectionDataFirmUpdated_SP_Cor", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
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
        public DataSet GetCorrectionDataFirmPending(string search, string CrType, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCorrectionDataFirmPending_SP_Cor", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
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
        public DataSet GetCorrectionDataFirmFinalSubmit(string userNM, string CorType)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCorrectionDataFirmFinalSubmit_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirmUser", userNM);
                    cmd.Parameters.AddWithValue("@cr", CorType);
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
        public DataSet VerifyFirmSchoolCorrection(string id, string FirmUser)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("VerifyFirmSchoolCorrection_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirmUser", FirmUser);
                    cmd.Parameters.AddWithValue("@CorID", id);
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
        public DataSet CancelFirmSchoolCorrection(string id, string FirmUser)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CancelFirmSchoolCorrection_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirmUser", FirmUser);
                    cmd.Parameters.AddWithValue("@CorID", id);
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

        public DataSet GetAllCorrectionDataFirm(string UserName)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAllCorrectionDataFirm_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirmName", UserName);
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
        public DataSet FirmSchoolCorrectionAllCorrectionRecord(string firmUser, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("FirmSchoolCorrectionAllCorrectionRecord_SP_New28", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@firmUser", firmUser);
                    cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
                    //cmd.Parameters.AddWithValue("@search", userNM);
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
        public DataSet FirmSchoolCorrectionPendingCorrectionRecord(string firmUser, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("FirmSchoolCorrectionPendingCorrectionRecord_SP_New28", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@firmUser", firmUser);
                    cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
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
        public DataSet GetFirmFinalPrint(AdminModels am, string firmUser, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetFirmFinalPrint_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@firmUser", firmUser);
                    cmd.Parameters.AddWithValue("@CorrectionType", am.CorrectionType);
                    cmd.Parameters.AddWithValue("@CorrectionFromDate", am.CorrectionFromDate);
                    cmd.Parameters.AddWithValue("@CorrectionToDate", am.CorrectionToDate);
                    //cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                    //cmd.Parameters.AddWithValue("@PageSize", 20);
                    //cmd.Parameters.AddWithValue("@search", userNM);
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
        public DataSet DownloadFirmFinalPrint(string FirmCorLot, string FirmNM, string CType, string SearchString = "")
        {
            //CTYPR = "PARTICULAR";

            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            string mdbPathCor = WebConfigurationManager.AppSettings["mdbPathCor"].ToString();
            string mdbPathCorBlank = WebConfigurationManager.AppSettings["mdbPathCorBlank"].ToString();
            System.Data.OleDb.OleDbConnection AccessConn = new
               System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbPathCor + "");
            try
            {

                if (File.Exists(mdbPathCor))
                {
                    File.Delete(mdbPathCor);
                }
                else
                {
                    File.Copy(mdbPathCorBlank, mdbPathCor);
                }
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd2 = new SqlCommand("GetStdIdByFirmPrintLot_SP_Test", con);
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@firmUser", FirmCorLot);
                    ad.SelectCommand = cmd2;
                    ad.Fill(result);
                    con.Open();
                    // return result;
                }


                string Search = "";
                Int32 i = 0;
                if (CType != "ALLstd")
                {
                    foreach (DataTable table in result.Tables)
                    {
                        foreach (DataRow dr in table.Rows)
                        {
                            Search += result.Tables[0].Rows[i][0].ToString() + ",";
                            i++;
                        }
                    }
                    Search = Search.Remove(Search.Length - 1);
                }
                else
                {
                    //CType = "ALL";
                    Search = SearchString;
                }

                DataSet resultTmp = new DataSet();
                SqlDataAdapter adTmp = new SqlDataAdapter();
                switch (CType)
                {
                    case "PARTICULAR":
                        try
                        {
                            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                            {
                                SqlCommand cmdTmp = new SqlCommand("InsertAllCorrectionDataFirmFP", con);
                                cmdTmp.CommandType = CommandType.StoredProcedure;
                                cmdTmp.Parameters.AddWithValue("@Search", Search);
                                cmdTmp.Parameters.AddWithValue("@CorType", "PARTICULAR");
                                adTmp.SelectCommand = cmdTmp;
                                adTmp.Fill(resultTmp);
                                con.Open();
                                //return resultTmp;
                            }
                        }
                        catch (Exception ex)
                        {
                            return resultTmp = null;
                        }

                        AccessConn.Open();
                        System.Data.OleDb.OleDbCommand AccessCommand = new System.Data.OleDb.OleDbCommand("select * into [regMasterRegular2016Access]  FROM [regMasterRegular2016tmp4$]  IN '' [ODBC;Driver={SQL Server};Server=43.224.136.122,2499;Database=firmdata_online;uid=sa;pwd=pU74h5#x;Connect Timeout=1200;];", AccessConn);
                        AccessCommand.ExecuteNonQuery();
                        AccessConn.Close();
                        System.Threading.Thread.Sleep(10000);
                        AccessConn.Open();
                        System.Data.OleDb.OleDbCommand AccessCommand11 = new System.Data.OleDb.OleDbCommand("select * into [tblregistrationOPENAccess]  FROM [regMasterRegular2016OPENtmp4$]  IN '' [ODBC;Driver={SQL Server};Server=43.224.136.122,2499;Database=firmdata_online;uid=sa;pwd=pU74h5#x;Connect Timeout=1200;];", AccessConn);
                        AccessCommand11.ExecuteNonQuery();
                        AccessConn.Close();
                        System.Threading.Thread.Sleep(5000);
                        break;
                    case "SUBJECT":
                    case "STREAM":

                        try
                        {
                            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                            {
                                SqlCommand cmdST = new SqlCommand("InsertAllCorrectionDataFirmFP", con);
                                cmdST.CommandType = CommandType.StoredProcedure;
                                cmdST.Parameters.AddWithValue("@Search", Search);
                                cmdST.Parameters.AddWithValue("@CorType", "SUBJECTSTREAM");
                                adTmp.SelectCommand = cmdST;
                                adTmp.Fill(resultTmp);
                                con.Open();
                                //return resultTmp;
                            }
                        }
                        catch (Exception ex)
                        {
                            return resultTmp = null;
                        }


                        AccessConn.Open();
                        System.Data.OleDb.OleDbCommand AccessCommand1 = new System.Data.OleDb.OleDbCommand("SELECT * INTO [tblSubjectsAccess]  FROM [tblSubjectstmp4$]  IN '' [ODBC;Driver={SQL Server};Server=43.224.136.122,2499;Database=firmdata_online;uid=sa;pwd=pU74h5#x;Connect Timeout=1200;];", AccessConn);
                        AccessCommand1.ExecuteNonQuery();
                        AccessConn.Close();
                        System.Threading.Thread.Sleep(10000);
                        AccessConn.Open();
                        System.Data.OleDb.OleDbCommand AccessCommand7 = new System.Data.OleDb.OleDbCommand("SELECT * INTO [tblSubjectsOpenAccess]  FROM [tblSubjectsOpentmp4$]  IN '' [ODBC;Driver={SQL Server};Server=43.224.136.122,2499;Database=firmdata_online;uid=sa;pwd=pU74h5#x;Connect Timeout=1200;];", AccessConn);
                        AccessCommand7.ExecuteNonQuery();
                        AccessConn.Close();
                        break;
                    case "PHOTOSIGN":

                        try
                        {
                            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                            {
                                SqlCommand cmdTmp = new SqlCommand("InsertAllCorrectionDataFirmFP", con);
                                cmdTmp.CommandType = CommandType.StoredProcedure;
                                cmdTmp.Parameters.AddWithValue("@Search", Search);
                                cmdTmp.Parameters.AddWithValue("@CorType", "PHOTOSIGN");
                                adTmp.SelectCommand = cmdTmp;
                                adTmp.Fill(resultTmp);
                                con.Open();
                                //return resultTmp;
                            }
                        }
                        catch (Exception ex)
                        {
                            return resultTmp = null;
                        }

                        break;
                    case "ALL":
                    case "ALLstd":
                        try
                        {
                            if (CType == "ALL")
                            {
                                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                                {
                                    SqlCommand cmdTmp = new SqlCommand("InsertAllCorrectionDataFirmFP", con);
                                    cmdTmp.CommandType = CommandType.StoredProcedure;
                                    cmdTmp.Parameters.AddWithValue("@Search", Search);
                                    cmdTmp.Parameters.AddWithValue("@CorType", "ALL");
                                    adTmp.SelectCommand = cmdTmp;
                                    adTmp.Fill(resultTmp);
                                    con.Open();
                                    //return resultTmp;
                                }
                            }
                            else
                            {
                                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                                {
                                    SqlCommand cmdTmp = new SqlCommand("InsertAllCorrectionDataFirmFP", con);
                                    cmdTmp.CommandType = CommandType.StoredProcedure;
                                    cmdTmp.Parameters.AddWithValue("@Search", Search);
                                    cmdTmp.Parameters.AddWithValue("@CorType", "ALLstd");
                                    adTmp.SelectCommand = cmdTmp;
                                    adTmp.Fill(resultTmp);
                                    con.Open();
                                    //return resultTmp;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            return resultTmp = null;
                        }

                        AccessConn.Open();
                        System.Data.OleDb.OleDbCommand AccessCommandA = new System.Data.OleDb.OleDbCommand("select * into [regMasterRegular2016Access] FROM [regMasterRegular2016tmp4$] IN '' [ODBC;Driver={SQL Server};Server=43.224.136.122,2499;Database=firmdata_online;uid=sa;pwd=pU74h5#x;Connect Timeout=300;];", AccessConn);
                        AccessCommandA.ExecuteNonQuery();
                        AccessConn.Close();
                        System.Threading.Thread.Sleep(100);

                        AccessConn.Open();
                        System.Data.OleDb.OleDbCommand AccessCommandB = new System.Data.OleDb.OleDbCommand("select * into [tblregistrationOPENAccess] FROM [regMasterRegular2016OPENtmp4$] IN '' [ODBC;Driver={SQL Server};Server=43.224.136.122,2499;Database=firmdata_online;uid=sa;pwd=pU74h5#x;Connect Timeout=300;];", AccessConn);
                        AccessCommandB.ExecuteNonQuery();
                        AccessConn.Close();
                        System.Threading.Thread.Sleep(100);

                        AccessConn.Open();
                        System.Data.OleDb.OleDbCommand AccessCommandC = new System.Data.OleDb.OleDbCommand("SELECT * INTO [tblSubjectsAccess]  FROM [tblSubjectstmp4$] IN '' [ODBC;Driver={SQL Server};Server=43.224.136.122,2499;Database=firmdata_online;uid=sa;pwd=pU74h5#x;Connect Timeout=300;];", AccessConn);
                        AccessCommandC.ExecuteNonQuery();
                        AccessConn.Close();
                        System.Threading.Thread.Sleep(100);

                        AccessConn.Open();
                        System.Data.OleDb.OleDbCommand AccessCommandD = new System.Data.OleDb.OleDbCommand("SELECT * INTO [tblSubjectsOpenAccess]  FROM [tblSubjectsOpentmp4$]  IN '' [ODBC;Driver={SQL Server};Server=43.224.136.122,2499;Database=firmdata_online;uid=sa;pwd=pU74h5#x;Connect Timeout=300;];", AccessConn);
                        AccessCommandD.ExecuteNonQuery();
                        AccessConn.Close();
                        System.Threading.Thread.Sleep(100);

                        break;
                    default:
                        break;
                }


                //

                //
                System.Threading.Thread.Sleep(10000);
                DownloadFiles(CType, FirmNM, FirmCorLot);
                return result;
            }
            catch (Exception ex)
            {
                SqlCon.Close();
                AccessConn.Close();
                return result = null;

            }
            finally
            {
                AccessConn.Close();
            }
        }
        public void DownloadFiles(string CType, string FirmNM, string FirmCorLot)
        {
            if (CType.ToUpper() == "ALL") { FirmCorLot = "Merged"; }
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                //    zip.AddDirectoryByName("PhotoAndSignatures");
                /*  
              foreach (GridViewRow row in GridView1.Rows)
              {
                  if ((row.FindControl("chkSelect") as CheckBox).Checked)
                  {
                 string filePath = (row.FindControl("lblFilePath") as Label).Text;
                 zip.AddFile(filePath, "Files");
                  }
              }
                 */
                //string fn="",ph="";
                if (CType.ToUpper() == "PHOTOSIGN" || CType.ToUpper() == "ALL" || CType.ToUpper() == "ALLSTD")
                {
                    string str1 = string.Empty;
                    foreach (string str in GetListofPhotoSignature())
                    {
                        // fn = Path.GetFileName(str);
                        //ph=Path.GetDirectoryName(str);
                        if (File.Exists(str) == true)
                        {
                            zip.AddFile(str, "ImgandSing");

                        }
                        else
                        {
                            str1 = str1 + " " + str;
                            //Response.Write("Missing Image :" + str);
                        }

                    }
                    if (CType.ToUpper() == "ALL" || CType.ToUpper() == "ALLSTD")
                    {
                        string mdbPathCor = WebConfigurationManager.AppSettings["mdbPathCor"].ToString();
                        zip.AddFile(mdbPathCor, "DataFile");
                        //zip.AddFile("D:\\PSEBONLINE\\PSEBONLINE\\Download\\Backup.mdb", "DataFile");

                    }
                }

                else
                {
                    string mdbPathCor = WebConfigurationManager.AppSettings["mdbPathCor"].ToString();
                    zip.AddFile(mdbPathCor, "DataFile");
                    //zip.AddFile("D:\\PSEBONLINE\\PSEBONLINE\\Download\\Backup.mdb", "DataFile");

                }



                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.BufferOutput = false;
                string zipName = String.Format(FirmCorLot + CType + "Data_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                HttpContext.Current.Response.ContentType = "application/zip";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                zip.Save(HttpContext.Current.Response.OutputStream);
                HttpContext.Current.Response.End();



            }
        }
        //private string[,] GetListofPhotoSignature()
        //{
        //    try
        //    {
        //        string ImagePathCor = WebConfigurationManager.AppSettings["ImagePathCor"].ToString();
        //        string ImagePathOpenCor = WebConfigurationManager.AppSettings["DownloadOpen"].ToString();

        //        SqlCommand cmd = new SqlCommand("select '" + ImagePathCor + "'+std_Photo as Photo,'" + ImagePathCor + "'+std_Sign as Sign from  regMasterRegular2016tmp4$ UNION ALL                    select (case when substring(IMG_RAND,1,5)='https' then replace(IMG_RAND,'https://s3-ap-southeast-1.amazonaws.com/pseb/open2016/','" + ImagePathOpenCor + "') else '" + ImagePathCor + "'+IMG_RAND end) as Photo, (case when substring(IMGSIGN_RA, 1, 5) = 'https' then replace(IMGSIGN_RA,'https://s3-ap-southeast-1.amazonaws.com/pseb/open2016/','" + ImagePathOpenCor + "') else '" + ImagePathCor + "' + IMGSIGN_RA end) as Sign from  regMasterRegular2016OPENtmp4$", SqlCon);
        //        SqlDataAdapter da = new SqlDataAdapter();
        //        da.SelectCommand = cmd;
        //        DataSet ds = new DataSet();
        //        da.Fill(ds);
        //        string[,] arrvalues = new string[2, ds.Tables[0].Rows.Count];
        //        //loopcounter
        //        for (int loopcounter = 0; loopcounter < ds.Tables[0].Rows.Count; loopcounter++)
        //        {
        //            //assign dataset values to array
        //            arrvalues[0, loopcounter] = ds.Tables[0].Rows[loopcounter]["Photo"].ToString();
        //            arrvalues[1, loopcounter] = ds.Tables[0].Rows[loopcounter]["Sign"].ToString();
        //        }
        //        return arrvalues;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw;
        //    }
        //    finally
        //    { SqlCon.Close(); }

        //}

        private string[,] GetListofPhotoSignature()
        {
            try
            {
                string ImagePathCor = WebConfigurationManager.AppSettings["ImagePathCor"].ToString();
                string ImagePathOpenCor = WebConfigurationManager.AppSettings["DownloadOpen"].ToString();
                string ImagePathOpen = WebConfigurationManager.AppSettings["ImagePathOpen"].ToString();

                SqlCommand cmd = new SqlCommand("select '" + ImagePathCor + "'+std_Photo as Photo,'" + ImagePathCor + "'+std_Sign as Sign from  regMasterRegular2016tmp4$ UNION ALL                    select (case when substring(IMG_RAND,1,5)='https' then replace(IMG_RAND,'https://s3-ap-southeast-1.amazonaws.com/pseb/open2016/','" + ImagePathOpen + "') else '" + ImagePathCor + "'+IMG_RAND end) as Photo, (case when substring(IMGSIGN_RA, 1, 5) = 'https' then replace(IMGSIGN_RA,'https://s3-ap-southeast-1.amazonaws.com/pseb/open2016/','" + ImagePathOpen + "') else '" + ImagePathCor + "' + IMGSIGN_RA end) as Sign from  regMasterRegular2016OPENtmp4$", SqlCon);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);
                string[,] arrvalues = new string[2, ds.Tables[0].Rows.Count];
                //loopcounter
                for (int loopcounter = 0; loopcounter < ds.Tables[0].Rows.Count; loopcounter++)
                {
                    //assign dataset values to array
                    arrvalues[0, loopcounter] = ds.Tables[0].Rows[loopcounter]["Photo"].ToString();
                    arrvalues[1, loopcounter] = ds.Tables[0].Rows[loopcounter]["Sign"].ToString();
                }
                return arrvalues;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            { SqlCon.Close(); }

        }

        public int FirmChangePassword(AdminModels am)
        {
            int result = 0;
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("FirmChangePassword_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CorrectionUserId", am.CorrectionUserId);
                    cmd.Parameters.AddWithValue("@CorrectionOldPwd", am.CorrectionOldPwd);
                    cmd.Parameters.AddWithValue("@CorrectionNewPwd", am.CorrectionNewPwd);
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = -1;
            }
        }
        public DataSet SetCorrectionDataFirmFeeDetails(AdminModels am, string userNM,string EmpUserId)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SetCorrectionDataFirmFeeDetails_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirmUser", userNM);
                    cmd.Parameters.AddWithValue("@Correctionlot", am.CorrectionLot);
                    cmd.Parameters.AddWithValue("@RecieptNo", am.CorrectionRecieptNo);
                    cmd.Parameters.AddWithValue("@RecieptDate", am.CorrectionRecieptDate);
                    cmd.Parameters.AddWithValue("@NoCapproved", am.CorrectionNoCapproved);
                    cmd.Parameters.AddWithValue("@Amount", am.CorrectionAmount);
                    cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
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

        public DataSet CheckCorrectionFee(string userNM, string CorrDate)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CheckCorrectionFee", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirmUser", userNM);
                    cmd.Parameters.AddWithValue("@CorrDate", CorrDate);
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
        public DataSet GetCorrectionFeeDetailLot(string userNM, string CorType)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCorrectionFeeDetailLot_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirmUser", userNM);
                    cmd.Parameters.AddWithValue("@cr", CorType);
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


        public DataSet AllVerifyFirmSchoolCorrection(string FirmUser, string CorType, string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AllVerifyFirmSchoolCorrection_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirmUser", FirmUser);
                    cmd.Parameters.AddWithValue("@CrType", CorType);
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
        public DataSet AllCancelFirmSchoolCorrection(string FirmUser, string CorType, string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AllCancelFirmSchoolCorrection_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FirmUser", FirmUser);
                    cmd.Parameters.AddWithValue("@CrType", CorType);
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

        public string DownloadZIPFileSP(string tablename)
        {
            string DwonSp = System.Configuration.ConfigurationManager.AppSettings["Download"];
            string result = "0";
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                foreach (string str in GetListofPhotoSignatureBytblSP(tablename))
                {
                    string filelocation = "";
                    // zip.AddFile(@"MyMusic\Handel\Messiah-01.mp3");
                    // string filelocation = Path.Combine(HttpContext.Current.Server.MapPath("~/Upload"), str);        
                    if (tablename.ToString().ToUpper().Contains("REG"))
                    {
                        filelocation = Path.Combine(DwonSp + "/", str);
                    }
                    else
                    {
                        //filelocation = Path.Combine(HttpContext.Current.Server.MapPath("~/"), str);
                        filelocation = Path.Combine(str);
                    }


                    if (File.Exists(filelocation) == true)
                    {
                        zip.AddFile(filelocation, "PhotoandSign");
                    }
                    else
                    {
                        //Response.Write("Missing Image :" + str);
                        // zip.AddFile(filelocation, "ImgandSing");
                        //  zip.AddFile(@"Images\NOPhoto.jpg");
                    }
                }
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.BufferOutput = false;
                string zipName = String.Format("ZipFile" + "Data_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                HttpContext.Current.Response.ContentType = "application/zip";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                zip.Save(HttpContext.Current.Response.OutputStream);
                HttpContext.Current.Response.End();
                result = "1";
            }
            return result;
        }
        private string[,] GetListofPhotoSignatureBytblSP(string tblname)
        {
            try
            {
                string qry = "select PHOTO,Sign from [" + tblname + "]";
                // string ImagePathCor = WebConfigurationManager.AppSettings["ImagePathCor"].ToString();
                SqlCommand cmd = new SqlCommand(qry, SqlCon);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);
                string[,] arrvalues = new string[2, ds.Tables[0].Rows.Count];
                //loopcounter
                for (int loopcounter = 0; loopcounter < ds.Tables[0].Rows.Count; loopcounter++)
                {
                    //assign dataset values to array
                    arrvalues[0, loopcounter] = ds.Tables[0].Rows[loopcounter]["Photo"].ToString();
                    arrvalues[1, loopcounter] = ds.Tables[0].Rows[loopcounter]["Sign"].ToString();
                }
                return arrvalues;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            { SqlCon.Close(); }

        }
        #endregion Firm Correction Data END

        public string DownloadZIPFile(string tablename, string db)
        {
            //  string DwonSp = System.Configuration.ConfigurationManager.AppSettings["Download"];
            string DwonSp = System.Configuration.ConfigurationManager.AppSettings["Upload"];
            string result = "0";
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                foreach (string str in GetListofPhotoSignatureBytbl(tablename, db))
                {
                    string filelocation = "";
                    // zip.AddFile(@"MyMusic\Handel\Messiah-01.mp3");
                    // string filelocation = Path.Combine(HttpContext.Current.Server.MapPath("~/Upload"), str);        
                    if (!tablename.ToString().ToUpper().Contains("http"))
                    {
                        filelocation = Path.Combine(DwonSp + "/", str);
                    }
                    else
                    {
                        filelocation = str;
                    }


                    if (File.Exists(filelocation) == true)
                    {
                        zip.AddFile(filelocation, "PhotoandSign");
                    }
                    else
                    {
                        //Response.Write("Missing Image :" + str);
                        // zip.AddFile(filelocation, "ImgandSing");
                        //  zip.AddFile(@"Images\NOPhoto.jpg");
                    }
                }
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.BufferOutput = false;
                string zipName = String.Format("ZipFile" + "Data_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                HttpContext.Current.Response.ContentType = "application/zip";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                zip.Save(HttpContext.Current.Response.OutputStream);
                HttpContext.Current.Response.End();
                result = "1";
            }
            return result;
        }
        private string[,] GetListofPhotoSignatureBytbl(string tblname, string db)
        {
            try
            {
                CommonCon = db;
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    string qry = "select PHOTO,Sign from [" + tblname + "]";
                    // string ImagePathCor = WebConfigurationManager.AppSettings["ImagePathCor"].ToString();
                    SqlCommand cmd = new SqlCommand(qry, con);
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    string[,] arrvalues = new string[2, ds.Tables[0].Rows.Count];
                    //loopcounter
                    for (int loopcounter = 0; loopcounter < ds.Tables[0].Rows.Count; loopcounter++)
                    {
                        //assign dataset values to array
                        arrvalues[0, loopcounter] = ds.Tables[0].Rows[loopcounter]["Photo"].ToString();
                        arrvalues[1, loopcounter] = ds.Tables[0].Rows[loopcounter]["Sign"].ToString();
                    }
                    return arrvalues;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            { SqlCon.Close(); }

        }

        //CutList
        #region Begin CutList
        public DataSet UpdateCutListSubjects(string schid, string CLASS1, string Type1, out int OutStatus)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("UpdateCutListSubjects", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", schid);
                    cmd.Parameters.AddWithValue("@CLASS", CLASS1);
                    cmd.Parameters.AddWithValue("@Type", Type1);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return result = null;
            }
        }
        #endregion End CutList  

        //

        public DataSet GetSummaryPerformaReport()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetSummaryPerformaReportsp", con);
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

        #region DB Firm School CCE Grading Report Download
        public DataSet SelectDistCCEGrading(string FirmNM)
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
                    SqlCommand cmd = new SqlCommand("GetAllDistrictCCEGrading", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@firmNM", FirmNM);
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
        public DataSet FirmSchoolCCEGrading(string Search, string FirmNM, string CrType, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("FirmSchoolCCEGrading_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@firmNM", FirmNM);
                    cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 30);
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

        public DataSet FirmSchoolCCEGradingDownload(string Search, string FirmNM)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("FirmSchoolCCEGrading4Download", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@firmNM", FirmNM);
                    //cmd.Parameters.AddWithValue("@pageIndex", pageIndex);
                    //cmd.Parameters.AddWithValue("@PageSize", 30);
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
        #endregion DB Firm School CCE Grading Report Download

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
                    if (HttpContext.Current.Session["Session"].ToString() == "2016-2017")
                    {
                        itemFirm = "CIPL,SAI,DATA,PERF";
                        if (!AbstractLayer.StaticDB.ContainsValue(itemFirm.ToUpper(), dt.Rows[i][0].ToString().ToUpper()))
                        {
                            int RowNo = i + 2;
                            string FirmNM = dt.Rows[i][0].ToString().ToUpper();
                            Result += "Please check FIRM " + FirmNM + " in row " + RowNo + ",  ";
                            dt1.Rows[i]["Status"] += "Please check FIRM " + FirmNM + " in row " + RowNo + ",  ";
                        }

                        if (dt.Rows[i][11].ToString() == "")
                        {
                            int RowNo = i + 2;
                            string ID = dt.Rows[i][11].ToString();
                            Result += "Please check ID " + ID + " in row " + RowNo + ",  ";
                            dt1.Rows[i]["Status"] += "Please check ID " + ID + " in row " + RowNo + ",  ";
                        }

                        if (dt.Rows[i][12].ToString() == "")
                        {
                            int RowNo = i + 2;
                            string Roll = dt.Rows[i][12].ToString();
                            Result += "Please check Roll " + Roll + " in row " + RowNo + ",  ";
                            dt1.Rows[i]["Status"] += "Please check Roll " + Roll + " in row " + RowNo + ",  ";
                        }
                    }
                    else
                    {
                        itemFirm = "SAI,PERF";
                        if (!AbstractLayer.StaticDB.ContainsValue(itemFirm.ToUpper(), dt.Rows[i][0].ToString().ToUpper()))
                        {
                            int RowNo = i + 2;
                            string FirmNM = dt.Rows[i][0].ToString().ToUpper();
                            Result += "Please check FIRM " + FirmNM + " in row " + RowNo + ",  ";
                            dt1.Rows[i]["Status"] += "Please check FIRM " + FirmNM + " in row " + RowNo + ",  ";
                        }
                        if (dt.Rows[i][12].ToString() == "")
                        {
                            int RowNo = i + 2;
                            string ID = dt.Rows[i][12].ToString();
                            Result += "Please check ID " + ID + " in row " + RowNo + ",  ";
                            dt1.Rows[i]["Status"] += "Please check ID " + ID + " in row " + RowNo + ",  ";
                        }

                        if (dt.Rows[i][13].ToString() == "")
                        {
                            int RowNo = i + 2;
                            string Roll = dt.Rows[i][13].ToString();
                            Result += "Please check Roll " + Roll + " in row " + RowNo + ",  ";
                            dt1.Rows[i]["Status"] += "Please check Roll " + Roll + " in row " + RowNo + ",  ";
                        }
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

        public DataTable AdminResultUpdateMIS(DataTable dt1, int adminid, out string OutError)  // ExamErrorListMISSP
        {
            int OutStatus;
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("AdminResultUpdateMIS_SPNew", con);//AdminResultUpdateMIS_SP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    if (HttpContext.Current.Session["Session"].ToString() == "2016-2017")
                    {
                        cmd.Parameters.AddWithValue("@TypetblAdminResultUpdateMIS", dt1);
                    }
                    else
                    { cmd.Parameters.AddWithValue("@TypetblAdminResultUpdateMIS2018", dt1); }
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    cmd.CommandTimeout = 300;
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


        #region Final Exam Result 2017
        public DataSet GetFinalExamResult(int year, int Type, string search, int pageNumber, int PageSize)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetFinalExamResult", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@year", year);
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


        public DataSet GetExamResultByRollNoNYear(string roll, int year, int Type)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetExamResultByRollNoNYear", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@roll", roll);
                    cmd.Parameters.AddWithValue("@year", year);
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

        #endregion Final Exam Result 2017


        #region DB Download Pvt PHOTO SIGN
        public string DownloadZIPFileSPPvt(string UserNM)
        {
            //string DwonSp = System.Configuration.ConfigurationManager.AppSettings["Download"];
            string DwonSp = System.Configuration.ConfigurationManager.AppSettings["Upload"];
            string result = "0";
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                foreach (string str in GetListofPhotoSignatureBytblSPpvt(UserNM))
                {
                    string filelocation = "";
                    // zip.AddFile(@"MyMusic\Handel\Messiah-01.mp3");
                    //filelocation = Path.Combine(HttpContext.Current.Server.MapPath("~/Upload"), str);        
                    if (str.ToString().ToUpper().Contains("OPEN2016"))
                    {
                        filelocation = Path.Combine(HttpContext.Current.Server.MapPath("~/"), str);
                    }
                    else
                    {
                        filelocation = Path.Combine(DwonSp + "/", str);
                        //filelocation = Path.Combine(str);
                    }

                    //zip.AddFile(filelocation, "PhotoandSign");
                    if (File.Exists(filelocation) == true)
                    {
                        zip.AddFile(filelocation, "PhotoandSign");
                    }
                    else
                    {
                        //Response.Write("Missing Image :" + str);
                        // zip.AddFile(filelocation, "ImgandSing");
                        //  zip.AddFile(@"Images\NOPhoto.jpg");
                    }
                }
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.BufferOutput = false;
                string zipName = String.Format("ZipFile" + "Data_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                HttpContext.Current.Response.ContentType = "application/zip";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                zip.Save(HttpContext.Current.Response.OutputStream);
                HttpContext.Current.Response.End();
                result = "1";
            }
            return result;
        }
        private string[,] GetListofPhotoSignatureBytblSPpvt(string firm)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet ds = new DataSet();
            SqlCommand cmd;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    cmd = new SqlCommand("GetPvtCompDataByFirm_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@firm", firm);
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(ds);
                    string[,] arrvalues = new string[2, ds.Tables[0].Rows.Count];
                    for (int loopcounter = 0; loopcounter < ds.Tables[0].Rows.Count; loopcounter++)
                    {
                        arrvalues[0, loopcounter] = ds.Tables[0].Rows[loopcounter]["Photo"].ToString();
                        arrvalues[1, loopcounter] = ds.Tables[0].Rows[loopcounter]["Sign"].ToString();
                    }
                    return arrvalues;
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            { SqlCon.Close(); }

        }

        public string DownloadZIPFileSPPvtREFNO(string UserNM, string search)
        {
            //string DwonSp = System.Configuration.ConfigurationManager.AppSettings["Download"];
            string DwonSp = System.Configuration.ConfigurationManager.AppSettings["Upload"];
            string result = "0";
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                foreach (string str in GetListofPhotoSignatureBytblSPpvtREFNO(UserNM, search))
                {
                    string filelocation = "";
                    // zip.AddFile(@"MyMusic\Handel\Messiah-01.mp3");
                    //filelocation = Path.Combine(HttpContext.Current.Server.MapPath("~/Upload"), str);        
                    if (str.ToString().ToUpper().Contains("OPEN2016"))
                    {
                        filelocation = Path.Combine(HttpContext.Current.Server.MapPath("~/"), str);
                    }
                    else
                    {
                        filelocation = Path.Combine(DwonSp + "/", str);
                        //filelocation = Path.Combine(str);
                    }

                    //zip.AddFile(filelocation, "PhotoandSign");
                    if (File.Exists(filelocation) == true)
                    {
                        zip.AddFile(filelocation, "PhotoandSign");
                    }
                    else
                    {
                        //Response.Write("Missing Image :" + str);
                        // zip.AddFile(filelocation, "ImgandSing");
                        //  zip.AddFile(@"Images\NOPhoto.jpg");
                    }
                }
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.BufferOutput = false;
                string zipName = String.Format("ZipFile" + "Data_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                HttpContext.Current.Response.ContentType = "application/zip";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                zip.Save(HttpContext.Current.Response.OutputStream);
                HttpContext.Current.Response.End();
                result = "1";
            }
            return result;
        }
        private string[,] GetListofPhotoSignatureBytblSPpvtREFNO(string firm, string search)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet ds = new DataSet();
            SqlCommand cmd;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    cmd = new SqlCommand("GetPvtCompDataByFirmREFNO_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@firm", firm);
                    cmd.Parameters.AddWithValue("@search", search);
                    con.Open();
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(ds);
                    string[,] arrvalues = new string[2, ds.Tables[0].Rows.Count];
                    for (int loopcounter = 0; loopcounter < ds.Tables[0].Rows.Count; loopcounter++)
                    {
                        arrvalues[0, loopcounter] = ds.Tables[0].Rows[loopcounter]["Photo"].ToString();
                        arrvalues[1, loopcounter] = ds.Tables[0].Rows[loopcounter]["Sign"].ToString();
                    }
                    return arrvalues;
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            { SqlCon.Close(); }

        }
        #endregion

        #region update ImpDataUpdate 2015 and 2016 file
        public string CheckImpDataMisExcel(DataSet ds, string userNM)
        {
            string Result = "";
            try
            {
                //string itemFirm = "CIPL,SAI,DATA,PERF";
                //for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                //{
                //    if (!AbstractLayer.StaticDB.ContainsValue(itemFirm.ToUpper(), ds.Tables[0].Rows[i][1].ToString().ToUpper()))
                //    {
                //        int RowNo = i + 2;
                //        string FirmNM = ds.Tables[0].Rows[i][0].ToString().ToUpper();
                //        Result += "Please check FIRM " + FirmNM + " in row " + RowNo + ",  ";
                //    }

                //    //if (ds.Tables[0].Rows[i][0].ToString().ToUpper() != userNM.ToUpper())
                //    //{
                //    //    int RowNo = i + 2;

                //    //    string FirmNM = ds.Tables[0].Rows[i][0].ToString();
                //    //    Result += "Please check FIRM " + FirmNM + " in row " + RowNo + ",  ";
                //    //}
                //}
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string ID = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check TID " + ID + " in row " + RowNo + ",  ";
                    }
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][28].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string ID = ds.Tables[0].Rows[i][28].ToString();
                        Result += "Please check Year " + ID + " in row " + RowNo + ",  ";
                    }
                }


            }
            catch (Exception ex)
            {
                Result = ex.Message;
                //return "Please Check All Fields";
            }
            return Result;
        }
        public DataTable ImpDataUpdate(DataTable dt1, int adminid, out int OutStatus)  // PvtRollEcentreUpdate
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ImpDataUpdate_SP", con); //PvtRollEcentreUpdate_SP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@ImpDataUpdate", dt1);
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
        #endregion update ImpDataUpdate 2015 and 2016 file

        #region Pvt 1. Roll & Ecentre Update 2. Error Add and Remove
        public string CheckRollEcentreMisExcel(DataSet ds, string userNM)
        {
            string Result = "";
            try
            {
                string itemFirm = "CIPL,SAI,DATA,PERF";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!AbstractLayer.StaticDB.ContainsValue(itemFirm.ToUpper(), ds.Tables[0].Rows[i][0].ToString().ToUpper()))
                    {
                        int RowNo = i + 2;
                        string FirmNM = ds.Tables[0].Rows[i][0].ToString().ToUpper();
                        Result += "Please check FIRM " + FirmNM + " in row " + RowNo + ",  ";
                    }

                    //if (ds.Tables[0].Rows[i][0].ToString().ToUpper() != userNM.ToUpper())
                    //{
                    //    int RowNo = i + 2;

                    //    string FirmNM = ds.Tables[0].Rows[i][0].ToString();
                    //    Result += "Please check FIRM " + FirmNM + " in row " + RowNo + ",  ";
                    //}
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string ID = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check OROLL " + ID + " in row " + RowNo + ",  ";
                    }
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][2].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string ID = ds.Tables[0].Rows[i][2].ToString();
                        Result += "Please check REFNO " + ID + " in row " + RowNo + ",  ";
                    }
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][3].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string ID = ds.Tables[0].Rows[i][3].ToString();
                        Result += "Please check ROLL " + ID + " in row " + RowNo + ",  ";
                    }
                }

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][4].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string Roll = ds.Tables[0].Rows[i][4].ToString();
                        Result += "Please check CENT " + Roll + " in row " + RowNo + ",  ";
                    }
                }

            }
            catch (Exception ex)
            {
                Result = ex.Message;
                //return "Please Check All Fields";
            }
            return Result;
        }       //CheckRollEcentreMisExcel
        public DataTable PvtRollEcentreUpdate(DataTable dt1, string userNM, out int OutStatus)  // PvtRollEcentreUpdate
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PvtRollEcentreUpdate_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@adminid", userNM);
                    cmd.Parameters.AddWithValue("@PvtRollEcentreUpdate", dt1);
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

        public string CheckPvtErrUpdateMisExcel(DataSet ds, string userNM)
        {
            string Result = "";
            try
            {
                string itemFirm = "CIPL,SAI,DATA,PERF";
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (!AbstractLayer.StaticDB.ContainsValue(itemFirm.ToUpper(), ds.Tables[0].Rows[i][0].ToString().ToUpper()))
                    {
                        int RowNo = i + 2;
                        string FirmNM = ds.Tables[0].Rows[i][0].ToString().ToUpper();
                        Result += "Please check FIRM " + FirmNM + " in row " + RowNo + ",  ";
                    }
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string ID = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check OROLL " + ID + " in row " + RowNo + ",  ";
                    }
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][2].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string ID = ds.Tables[0].Rows[i][2].ToString();
                        Result += "Please check REFNO " + ID + " in row " + RowNo + ",  ";
                    }
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][3].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string ID = ds.Tables[0].Rows[i][3].ToString();
                        Result += "Please check ERRCODE " + ID + " in row " + RowNo + ",  ";
                    }
                }

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][4].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string Roll = ds.Tables[0].Rows[i][4].ToString();
                        Result += "Please check STATUS " + Roll + " in row " + RowNo + ",  ";
                    }
                }

            }
            catch (Exception ex)
            {
                Result = ex.Message;
                //return "Please Check All Fields";
            }
            return Result;
        }       //CheckRollEcentreMisExcel
        public DataTable PvtErrUpdate(DataTable dt1, string UserNM, out int OutStatus)  // PvtRollEcentreUpdate
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PvtErrUpdate_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@adminid", UserNM);
                    cmd.Parameters.AddWithValue("@PvtErrorUpdate", dt1);
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

        #endregion Pvt 1. Roll & Ecentre Update 2. Error Add and Remove


        #region Final Submitted Records form All Forms For Admin
        public DataSet CancelStdRegNo(string remarks, string stdid,string EmpUserId)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CancelStdRegNo_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                    cmd.Parameters.AddWithValue("@remarks", remarks);
                    cmd.Parameters.AddWithValue("@stdid", stdid);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();

                    if (con.State == ConnectionState.Open)
                        con.Close();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }

        #endregion Final Submitted Records form All Forms For Admin

        public DataSet RPTDuplicateCertificate(int Type, string Stdate, string Enddate)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("RPTDuplicateCertificateSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@Stdate", Stdate);
                    cmd.Parameters.AddWithValue("@Enddate", Enddate);
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


        #region Rohit FirmSchoolCorrection

        public DataSet SchoolCorrectionStatus(string schlid, string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SchoolCorrectionStatusSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schlid", schlid);
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


        public string CorrLotAcceptReject(string EmpUserId, string correctionType, string correctionLot, string acceptid, string rejectid, string removeid, string adminid, out string OutStatus)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("CorrLotAcceptRejectSP", con);  //CreateAdminUserSP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.Parameters.AddWithValue("@correctionType", correctionType);
                cmd.Parameters.AddWithValue("@correctionLot", correctionLot);
                cmd.Parameters.AddWithValue("@acceptid", acceptid);
                cmd.Parameters.AddWithValue("@rejectid", rejectid);
                cmd.Parameters.AddWithValue("@removeid", removeid);
                cmd.Parameters.AddWithValue("@adminid", adminid);
                cmd.Parameters.AddWithValue("@EmpUserId",EmpUserId);
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                OutStatus = (string)cmd.Parameters["@OutError"].Value;
                return result;

            }
            catch (Exception ex)
            {
                OutStatus = "-1";
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }


        public DataSet CheckFeeAllCorrectionDataByFirmSP(int ActionType, string UserName, string FirmCorrectionLot)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CheckFeeAllCorrectionDataByFirmSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.AddWithValue("@ActionType", ActionType);
                    cmd.Parameters.AddWithValue("@FirmName", UserName);
                    cmd.Parameters.AddWithValue("@FirmCorrectionLot", FirmCorrectionLot);


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

        public DataSet CorrectionDataFirmFinalSubmitSPRNByCorrectionLot(string EmpUserId, string CorrectionLot, string userNM, out string FirmCorrectionLot, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CorrectionDataFirmFinalSubmitSPRNByCorrectionLot", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.AddWithValue("@CorrectionLot", CorrectionLot);
					cmd.Parameters.AddWithValue("@FirmUser", userNM);
                    cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId); 

					cmd.Parameters.Add("@FirmCorrectionLot", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    FirmCorrectionLot = (string)cmd.Parameters["@FirmCorrectionLot"].Value;
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                    return result;
                }
            }
            catch (Exception ex)
            {
                FirmCorrectionLot = "-1";
                OutError = "-1";
                return result = null;
            }
        }



        public DataSet CorrectionDataFirmFinalSubmitSPRN(string EmpUserId, string userNM, out string FirmCorrectionLot, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CorrectionDataFirmFinalSubmitSPRN", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                    cmd.Parameters.AddWithValue("@FirmUser", userNM);
					cmd.Parameters.AddWithValue("@CorrectionLot", "");
					cmd.Parameters.Add("@FirmCorrectionLot", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    FirmCorrectionLot = (string)cmd.Parameters["@FirmCorrectionLot"].Value;
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                    return result;
                }
            }
            catch (Exception ex)
            {
                FirmCorrectionLot = "-1";
                OutError = "-1";
                return result = null;
            }
        }

		public DataSet CorrectionDataFirmFinalSubmitSPRNBySchool(string EmpUserId, string userNM,string CorrectionLot, out string FirmCorrectionLot, out string OutError)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("CorrectionDataFirmFinalSubmitSPRN", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 300;
					cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
					cmd.Parameters.AddWithValue("@FirmUser", userNM);
					cmd.Parameters.AddWithValue("@CorrectionLot", CorrectionLot);
					cmd.Parameters.Add("@FirmCorrectionLot", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
					cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
					ad.SelectCommand = cmd;
					ad.Fill(result);
					con.Open();
					FirmCorrectionLot = (string)cmd.Parameters["@FirmCorrectionLot"].Value;
					OutError = (string)cmd.Parameters["@OutError"].Value;
					return result;
				}
			}
			catch (Exception ex)
			{
				FirmCorrectionLot = "-1";
				OutError = "-1";
				return result = null;
			}
		}

		public string CorrLotRejectRemarksSP(string corid, string remarks, string adminid)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("CorrLotRejectRemarksSP", con);  //CorrLotRejectRemarksSP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.Parameters.AddWithValue("@corid", corid);
                cmd.Parameters.AddWithValue("@remarks", remarks);
                cmd.Parameters.AddWithValue("@adminid", adminid);
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
        #endregion Rohit FirmSchoolCorrection

        #region Firm Exam Data Download
        public DataSet FirmExamDataDownload(int Type, string RegOpen, string FirmUser, string Search, out string ErrStatus)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("FirmExamDataDownloadSPNew", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 300;
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@RegOpen", RegOpen);
                    cmd.Parameters.AddWithValue("@FirmUser", FirmUser);
                    cmd.Parameters.AddWithValue("@Search", Search);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    ErrStatus = "1";
                    return result;
                }
            }
            catch (Exception ex)
            {
                ErrStatus = ex.Message;
                return result = null;
            }
        }

        public string CheckFirmExamDataDownloadMis(DataSet ds, out DataTable dt, string RP)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("Status", typeof(string)));
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string ColName = ds.Tables[0].Columns[0].ColumnName.ToString();
                    if (ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string Std_id = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check Std_id " + Std_id + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += "Please check Std_id " + Std_id + " in row " + RowNo + ",  ";

                    }
                    else
                    {
                        if (RP != "")
                        {
                            DataSet ds1 = GetDataByIdandTypeRN(ds.Tables[0].Rows[i][0].ToString(), RP, ds.Tables[0].Rows[i][0].ToString());// Regular
                            if (ds1 == null || ds1.Tables.Count == 0 || ds1.Tables[0].Rows.Count == 0)
                            {
                                int RowNo = i + 2;
                                string Std_id = ds.Tables[0].Rows[i][0].ToString();
                                Result += "Please check Std_id " + Std_id + " Not Found in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] += "Please check Std_id " + Std_id + "Not Found in row " + RowNo + ",  ";
                            }
                            else
                            {

                                if (ds1.Tables[0].Rows[0]["Examchallanverify"].ToString().ToLower() != "true")
                                {
                                    int RowNo = i + 2;
                                    string Std_id = ds.Tables[0].Rows[i][0].ToString();
                                    Result += "Please check Std_id " + Std_id + " : Challan Not Verified in row " + RowNo + ",  ";
                                    dt.Rows[i]["Status"] += "Please check Std_id " + Std_id + " : Challan Not Verified in row " + RowNo + ",  ";

                                }
                            }
                        }
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

        // Photo Download of Exam Data Reg and Open
        public string DownloadZIPFileFirmExam(DataTable dt)
        {
            string DwonSp = System.Configuration.ConfigurationManager.AppSettings["Upload"];
            string result = "0";
            try
            {
               
                using (ZipFile zip = new ZipFile())
                {
                    zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                    foreach (string str in GetListofPhotoSignatureByDataTable(dt))
                    {
                        string filelocation = "";
                        if (str != "")
                        {
                            if (str.ToString().ToUpper().Contains("PvtPhoto"))
                            {
                                filelocation = Path.Combine(DwonSp + "/Upload2023/", str);
                            }
                            else if (str.ToString().ToUpper().Contains("ImageCorrection"))
                            {
                                filelocation = Path.Combine(DwonSp + "/Upload2023/", str);
                            }
                            else if (str.ToString().ToUpper().Contains("PVT")
                                || str.ToString().ToUpper().Contains("OPEN") 
                                || str.ToString().ToUpper().Contains("OPEN2019") || str.ToString().ToUpper().Contains("OPEN2018"))
                            {
                                filelocation = Path.Combine(DwonSp + "/", str);
                            }
                            else
                            {
                                filelocation = Path.Combine("X:/", str);
                            }

                            //// check File Exists
                            if (File.Exists(filelocation) == true)
                            {
                                zip.AddFile(filelocation, "PhotoandSign");
                            }
                        }
                    }
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.BufferOutput = false;
                    string zipName = String.Format("ZipFile" + "Data_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                    HttpContext.Current.Response.ContentType = "application/zip";
                    HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                    zip.Save(HttpContext.Current.Response.OutputStream);
                    HttpContext.Current.Response.End();
                    result = "1";
                }
            }
            catch (Exception)
            {

                throw;
            }
           
            return result;
        }

        private string[,] GetListofPhotoSignatureByDataTable(DataTable dt)
        {
            try
            {
                string[,] arrvalues = new string[2, 0];
                if (dt.Rows.Count > 0)
                {
                    arrvalues = new string[2, dt.Rows.Count];
                    //loopcounter
                    for (int loopcounter = 0; loopcounter < dt.Rows.Count; loopcounter++)
                    {
						//assign dataset values to array
						arrvalues[0, loopcounter] = dt.Rows[loopcounter]["Photo"].ToString();
						arrvalues[1, loopcounter] = dt.Rows[loopcounter]["Sign"].ToString();
						

						//                  arrvalues[0, loopcounter] = dt.Rows[loopcounter]["SignPath"].ToString();
						//arrvalues[1, loopcounter] = dt.Rows[loopcounter]["PhotoPath"].ToString();
					}
				}
                return arrvalues;
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            { SqlCon.Close(); }

        }

        #endregion Firm Exam Data Download

        #region DEO Profile Data Download
        public DataSet DownloadDeoProfile(AdminModels am)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            SqlCommand cmd;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    cmd = new SqlCommand("DownloadDeoProfile_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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
        #endregion DEO Profile Data Download

        #region Practical Cent Update Master

        public string CheckStdPracticalCentUpdateMaster(DataSet ds, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("Status", typeof(string)));

                if (ds.Tables[0].Rows.Count == 0)
                {
                    string ROLL = ds.Tables[0].Rows[0][0].ToString();
                    Result += "Data Not Found";
                    dt.Rows[0]["Status"] = "Data Not Found in file, please change in Text Format ";
                }

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string CANDID = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check CANDID " + CANDID + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check CANDID " + CANDID + " in row " + RowNo + ",  ";

                    }
                    if (ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string CENT = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check CENT " + CENT + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check CENT " + CENT + " in row " + RowNo + ",  ";
                    }
                    if (ds.Tables[0].Rows[i][2].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string SUB = ds.Tables[0].Rows[i][2].ToString();
                        Result += "Please check SUB " + SUB + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check SUB " + SUB + " in row " + RowNo + ",  ";
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

        public string CheckPrivatePracticalCentUpdateMaster(DataSet ds, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("Status", typeof(string)));

                if (ds.Tables[0].Rows.Count == 0)
                {                    
                    string ROLL = ds.Tables[0].Rows[0][0].ToString();
                    Result += "Data Not Found";
                    dt.Rows[0]["Status"] = "Data Not Found in file, please change in Text Format ";
                }

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string ROLL = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check ROLL " + ROLL + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check ROLL " + ROLL + " in row " + RowNo + ",  ";

                    }
                    if (ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string REFNO = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check REFNO " + REFNO + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check REFNO " + REFNO + " in row " + RowNo + ",  ";
                    }
                    if (ds.Tables[0].Rows[i][2].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string PRC_CENT = ds.Tables[0].Rows[i][2].ToString();
                        Result += "Please check PRC_CENT " + PRC_CENT + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check PRC_CENT " + PRC_CENT + " in row " + RowNo + ",  ";
                    }
                    if (ds.Tables[0].Rows[i][3].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string SUB = ds.Tables[0].Rows[i][3].ToString();
                        Result += "Please check SUB " + SUB + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check SUB " + SUB + " in row " + RowNo + ",  ";
                    }
                    if (ds.Tables[0].Rows[i][1].ToString() != "" && ds.Tables[0].Rows[i][3].ToString() != "")
                    {
                        int RowNo = i + 2;
                        DataSet dtp = GetPrivateSubjectByRefnoSub(ds.Tables[0].Rows[i][1].ToString(), ds.Tables[0].Rows[i][3].ToString());
                        if (dtp.Tables.Count > 0)
                        {
                            if (dtp.Tables[1].Rows.Count == 0)
                            {
                                Result += "Please check REFNO are Not Found in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] = "Please check REFNO Not Found in row" + RowNo + ",  ";
                            }
                            if (dtp.Tables[0].Rows.Count == 0)
                            {
                                Result += "Please check REFNO and SUB are not Matched in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] = "Please check REFNO and SUB are not Matched in row" + RowNo + ",  ";
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
            return Result;
        }

        public DataSet GetPrivateSubjectByRefnoSub(string refno, string sub)  // ExamErrorListMISSP
        {

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetPrivateSubjectByRefnoSub", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@REFNO", refno);
                    cmd.Parameters.AddWithValue("@SUB", sub);                    
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

        public string CheckRegOpenPracticalCentUpdateMaster(DataSet ds,string cls, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("Status", typeof(string)));

                if (ds.Tables[0].Rows.Count == 0)
                {
                    string ROLL = ds.Tables[0].Rows[0][0].ToString();
                    Result += "Data Not Found";
                    dt.Rows[0]["Status"] = "Data Not Found in file, please change in Text Format ";
                }

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][6].ToString() != "" && cls != "")
                    {
                        int RowNo = i + 2;
                        DataSet dtp = CheckPracticalCenter(ds.Tables[0].Rows[i][6].ToString(), cls);
                        if (dtp.Tables.Count > 0)
                        {                           
                            if (dtp.Tables[0].Rows.Count == 0)
                            {
                                Result += "Please check Practical Centre is not allowed for this class in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] = "Please check Practical Centre is not allowed for this class in row" + RowNo + ",  ";
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
            return Result;
        }

        public DataSet CheckPracticalCenter(string cent, string cls)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CheckPracticalCenter", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cent", cent);
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


        public DataTable PracticalCentUpdateMaster(DataTable dt1, int adminid, string examtype, string cls, out string OutError)  // ExamErrorListMISSP
        {

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            SqlCommand cmd;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //pvt
                    if (examtype == "2")
                    {
                        cmd = new SqlCommand("PracCentPVTSP", con);
                        cmd.Parameters.AddWithValue("@PracCentPVT", dt1);
                    }
                    else
                    {
                        cmd = new SqlCommand("PracCentALLSP", con);
                        cmd.Parameters.AddWithValue("@PracCentALL", dt1);
                    }

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 200;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@examtype", examtype);
                    cmd.Parameters.AddWithValue("@cls", cls);
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


        public DataTable PracCentSTD(DataTable dt1, int adminid, string examtype, string cls, out string OutError)  // ExamErrorListMISSP
        {

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PracCentSTDSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@examtype", examtype);
                    cmd.Parameters.AddWithValue("@cls", cls);
                    cmd.Parameters.AddWithValue("@PracCentSTD", dt1);
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

        #endregion Practical Cent Update Master
               
        #region Practical Submission Unlocked

        public DataSet CheckPracFinalSubmission(string cls, string rp, string pcent, string sub, string fplot)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CheckPracFinalSubmission", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@class", cls);
                    cmd.Parameters.AddWithValue("@rp", rp);
                    cmd.Parameters.AddWithValue("@cent", pcent);
                    cmd.Parameters.AddWithValue("@sub", sub);
                    cmd.Parameters.AddWithValue("@fplot", fplot);
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

        public string CheckPracticalSubmissionUnlocked(DataSet ds, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("Status", typeof(string)));

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString() == "" || ds.Tables[0].Rows[i][1].ToString() == ""
                         || ds.Tables[0].Rows[i][2].ToString() == "" || ds.Tables[0].Rows[i][3].ToString() == ""
                         || ds.Tables[0].Rows[i][4].ToString() == "" || ds.Tables[0].Rows[i][5].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string CLASS = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check Empty Value in Mandatory Columns in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] = "Please check Empty Value in Mandatory Columns in row " + RowNo + ",  ";

                    }
                    
                    if (dt.Rows[i][5].ToString() != "")
                    {
                        DateTime todDate = DateTime.Today;
                        DateTime fromDateValue;
                        string s = dt.Rows[i][5].ToString();
                        if (DateTime.TryParseExact(s, "dd/MM/yyyy hh:mm:sstt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateValue))
                        { // do for valid date 

                            if (fromDateValue.Date < todDate.Date)
                            {
                                int RowNo = i + 2;
                                string challanid = ds.Tables[0].Rows[i][0].ToString();
                                Result += " Last Date must be greater or equal to today date  in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] += " , " + "Last Date must be greater or equal to today date in row " + RowNo + ",  ";
                            } 
                        }
                    }

                    DataSet ds1 = CheckPracFinalSubmission(ds.Tables[0].Rows[i][0].ToString(), ds.Tables[0].Rows[i][1].ToString(), ds.Tables[0].Rows[i][2].ToString(), ds.Tables[0].Rows[i][3].ToString(), ds.Tables[0].Rows[i][4].ToString());// Regular
                    if (ds1 == null || ds1.Tables.Count == 0 || ds1.Tables[0].Rows.Count == 0)
                    {
                        int RowNo = i + 2;
                        string REFNO = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check Practical Submission are Not Found in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += "Please check Practical Submission are Not Found in row " + RowNo + ",  ";
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

        public DataTable PracticalSubmissionUnlocked(string LastDate,string cls, string rp, string cent, string sub, string fplot, int adminid, out string OutError)  // ExamErrorListMISSP
        {

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            SqlCommand cmd;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    cmd = new SqlCommand("PracticalSubmissionUnlocked", con);//PracticalSubmissionUnlocked
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@class", cls);
                    cmd.Parameters.AddWithValue("@rp", rp);
                    cmd.Parameters.AddWithValue("@cent", cent);
                    cmd.Parameters.AddWithValue("@sub", sub);
                    cmd.Parameters.AddWithValue("@fplot", fplot);
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@LastDate", LastDate);
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

        #endregion  Practical Submission Unlocked
        
        #region  Cce Prac Marks Download

        public string CheckCcePracMarksDownloadMis(DataSet ds, out DataTable dt, string RP)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("Status", typeof(string)));
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string ColName = ds.Tables[0].Columns[0].ColumnName.ToString();
                    if (ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string Roll = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check Roll " + Roll + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += "Please check Roll " + Roll + " in row " + RowNo + ",  ";

                    }
                    //if (ds.Tables[0].Rows[i][1].ToString() == "")
                    //{
                    //    int RowNo = i + 2;
                    //    string Sub = ds.Tables[0].Rows[i][1].ToString();
                    //    Result += "Please check Sub " + Sub + " in row " + RowNo + ",  ";
                    //    dt.Rows[i]["Status"] += "Please check Sub " + Sub + " in row " + RowNo + ",  ";

                    //}
                    else
                    {
                        if (RP != "")
                        {
                            DataSet ds1 = GetDataByIdandTypeRN(ds.Tables[0].Rows[i][0].ToString(), RP, ds.Tables[0].Rows[i][0].ToString());// Regular
                            if (ds1 == null || ds1.Tables.Count == 0 || ds1.Tables[0].Rows.Count == 0)
                            {
                                int RowNo = i + 2;
                                string Roll = ds.Tables[0].Rows[i][0].ToString();
                                Result += "Please check Roll " + Roll + " Not Found in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] += "Please check Roll " + Roll + "Not Found in row " + RowNo + ",  ";
                            }

                        }
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


        public DataSet CcePracMarksDownload(int Type, int cls, string RP, int adminid, string Search, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CcePracMarksDownloadSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@class", cls);
                    cmd.Parameters.AddWithValue("@RP", RP);
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@Search", Search);
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
                OutError = ex.Message;
                return result = null;
            }
        }

        #endregion Cce Prac Marks Download

        #region Download Private Data
        public DataSet DownloadPrivateData(int Type, string batch, string FirmUser, string Search, out string ErrStatus)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DownloadPrivateDataSPNew", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@batch", batch);
                    cmd.Parameters.AddWithValue("@FirmUser", FirmUser);
                    cmd.Parameters.AddWithValue("@Search", Search);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    ErrStatus = "1";
                    return result;
                }
            }
            catch (Exception ex)
            {
                ErrStatus = ex.Message;
                return result = null;
            }
        }

        public string CheckDownloadPrivateDataMis(DataSet ds, out DataTable dt, string batch)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("Status", typeof(string)));
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string ColName = ds.Tables[0].Columns[0].ColumnName.ToString();
                    if (ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string Refno = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check Refno " + Refno + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += "Please check Refno " + Refno + " in row " + RowNo + ",  ";

                    }
                    else
                    {
                        if (batch != "")
                        {
                            //REFNO/batch/roll
                            DataSet ds1 = CheckPvtDatabyBatch(ds.Tables[0].Rows[i][0].ToString(), batch, ds.Tables[0].Rows[i][0].ToString());// Regul
                            if (ds1 == null || ds1.Tables.Count == 0 || ds1.Tables[0].Rows.Count == 0)
                            {
                                int RowNo = i + 2;
                                string Refno = ds.Tables[0].Rows[i][0].ToString();
                                Result += "Please check Refno " + Refno + " Not Found in row " + RowNo + ",  ";
                                dt.Rows[i]["Status"] += "Please check Refno " + Refno + "Not Found in row " + RowNo + ",  ";
                            }
                            else
                            {
                                if (ds1.Tables[0].Rows[0]["challanverify"].ToString().ToLower() != "1")
                                {
                                    int RowNo = i + 2;
                                    string Refno = ds.Tables[0].Rows[i][0].ToString();
                                    Result += "Please check Refno " + Refno + " : Challan Not Verified in row " + RowNo + ",  ";
                                    dt.Rows[i]["Status"] += "Please check Refno " + Refno + " : Challan Not Verified in row " + RowNo + ",  ";

                                }
                            }
                        }
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


        public DataSet CheckPvtDatabyBatch(string refno, string batch, string roll)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CheckPvtDatabyBatch", con);// CheckPvtDatabyBatch
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", refno);
                    cmd.Parameters.AddWithValue("@batch", batch);
                    cmd.Parameters.AddWithValue("@roll", roll);
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


        #endregion Download Private Data

        #region Affiliation Certificate
        public DataSet AffiliationCertificate(string SCHL)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AffiliationCertificate_SP", con);
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
        public DataSet Upd_AffiliationCertificate(string SCHL, string CertNo, string CertDate, string Remarks)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Upd_AffiliationCertificate_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@CertNo", CertNo);
                    cmd.Parameters.AddWithValue("@CertDate", CertDate);
                    cmd.Parameters.AddWithValue("@Remarks", Remarks);
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



        #endregion Affiliation Certificate

        #region Firm Exam Data Download
        public DataSet UpdateMasterData(string SelYear, int Type, string RP, int Adminid, DataTable dt, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UpdateMasterDataSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SelYear", SelYear);
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@RP", RP);
                    cmd.Parameters.AddWithValue("@Adminid", Adminid);
                    cmd.Parameters.AddWithValue("@tblMasterData", dt);
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
                OutError = ex.Message;
                return result = null;
            }
        }

        public string CheckUpdateMasterDataMis(DataSet ds, out DataTable dt, string RP)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("Status", typeof(string)));
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string ColName = ds.Tables[0].Columns[0].ColumnName.ToString();
                    if (ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string Roll = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check Roll " + Roll + " in row " + RowNo + ",  ";
                        dt.Rows[i]["Status"] += "Please check Roll " + Roll + " in row " + RowNo + ",  ";

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


        public DataSet SearchMasterData(string SelYear, int Type, string RP, int Adminid, string search, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SearchMasterDataSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SelYear", SelYear);
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@RP", RP);
                    cmd.Parameters.AddWithValue("@Adminid", Adminid);
                    cmd.Parameters.AddWithValue("@Search", search);
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
                OutError = ex.Message;
                return result = null;
            }
        }


        #endregion
        
        //------------------------------------------DB Class Region------------------

        #region Circular

        public string ListingCircular(int type, int id, out int OutStatus)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("ListingCircularSP", con);  //ListingUserSP
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.Add("@outstatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                OutStatus = (int)cmd.Parameters["@outstatus"].Value;
                return result;

            }
            catch (Exception ex)
            {
                OutStatus = -1;
                return result = "";
            }
            finally
            {
                con.Close();
            }
        }



        public DataSet CircularTypeMaster()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CircularTypeMasterSP", con);
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


        public DataSet CircularMaster(string search, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CircularMasterSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 15);
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

        public int InsertCircularMaster(string EmpUserId, CircularModels FM, out string outCircularNo)
        {
            SqlConnection con = null;
            int result = 0;
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("InsertCircularMasterSP", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                cmd.Parameters.AddWithValue("@Type", FM.Type);
                cmd.Parameters.AddWithValue("@ID", FM.ID);
                cmd.Parameters.AddWithValue("@Session", FM.Session);
                cmd.Parameters.AddWithValue("@Title", FM.Title);
                cmd.Parameters.AddWithValue("@Attachment", FM.Attachment);
                cmd.Parameters.AddWithValue("@UrlLink", FM.UrlLink);
                cmd.Parameters.AddWithValue("@Category", FM.Category);
                cmd.Parameters.AddWithValue("@UploadDate", FM.UploadDate);
                cmd.Parameters.AddWithValue("@ExpiryDate", FM.ExpiryDate);
                cmd.Parameters.AddWithValue("@IsMarque", FM.IsMarque);
                cmd.Parameters.AddWithValue("@IsActive", FM.IsActive);
                cmd.Parameters.AddWithValue("@CreatedBy", FM.CreatedBy);
                cmd.Parameters.AddWithValue("@UpdatedBy", FM.UpdatedBy);
                cmd.Parameters.AddWithValue("@CircularTypes", FM.SelectedCircularTypes);
                cmd.Parameters.AddWithValue("@CircularRemarks", FM.CircularRemarks);
                cmd.Parameters.Add("@OutId", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@CircularNo", SqlDbType.NVarChar, 30).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery();
                int outuniqueid = (int)cmd.Parameters["@OutId"].Value;
                outCircularNo = (string)cmd.Parameters["@CircularNo"].Value;
                return outuniqueid;

            }
            catch (Exception ex)
            {
                outCircularNo = "";
                return result = -1;
            }
            finally
            {
                con.Close();
            }
        }

        #endregion

        #region  Download Challan Master Data
        public DataSet Download_ChallanMaster(string feecat, string FirmUser, string Search, out string ErrStatus)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Download_ChallanMasterSP", con); //DownloadPrivateDataSPNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@feecat", feecat);
                    cmd.Parameters.AddWithValue("@FirmUser", FirmUser);
                    cmd.Parameters.AddWithValue("@Search", Search);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    ErrStatus = "1";
                    return result;
                }
            }
            catch (Exception ex)
            {
                ErrStatus = ex.Message;
                return result = null;
            }
        }
        #endregion  Download Challan Master Data

        #region open school report
        public DataSet GetSchoolRecords(string dist)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetSchoolRecords", con);
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
                return result = null;
            }
        }

        //Open18-19
        public DataSet GetSchoolRecords1819(string dist)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetSchoolRecords", con);
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
                return result = null;
            }
        }
        #endregion open school report

        #region ViewAllExamForm
        public DataSet GetExamFormCalFeeSPAdmin(int AdminId, string SCHL)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetExamFormCalFeeSPAdmin", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@AdminId", AdminId);
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
        #endregion ViewAllExamForm

        #region Late Admission
        public DataSet GetLateAdmissionSchl(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetLateAdmissionSchl_Sp", con);
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

        public string SetLateAdmissionSchl(string schl, string RID, string cls, string formNM, string regno, string name, string fname, string mname, string dob, string mobileno, string file, string usertype,string OBoard)
        {
            SqlConnection con = null;
            string result = null;
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("SetLateAdmissionSchl_sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@schl", schl);
                cmd.Parameters.AddWithValue("@RID", RID);
                cmd.Parameters.AddWithValue("@Class", cls);
                cmd.Parameters.AddWithValue("@form", formNM);
                cmd.Parameters.AddWithValue("@regno", regno);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@fname", fname);
                cmd.Parameters.AddWithValue("@mname", mname);
                cmd.Parameters.AddWithValue("@dob", dob);
                cmd.Parameters.AddWithValue("@mobileno", mobileno);
                cmd.Parameters.AddWithValue("@filepath", file);
                cmd.Parameters.AddWithValue("@usertype", usertype);
                cmd.Parameters.Add("@Outstatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                result = Convert.ToString(cmd.Parameters["@Outstatus"].Value);
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
            finally
            {
                con.Close();
            }
        }

        public string FinalSubmitLateAdmissionSchl(string RID)
        {
            SqlConnection con = null;
            string result = null;
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("FinalSubmitLateAdmissionSchl_sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RID", RID);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
            finally
            {
                con.Close();
            }
        }

        public DataSet LateAdmPrintLetter(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetLateAdmPrintLetter_Sp", con);
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
        public DataSet LateAdmHistory(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetLateAdmHistory_Sp", con);
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
        public string ApproveRejectLateAdmissionAdmin(string RID, string action)
        {
            SqlConnection con = null;
            string result = null;
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("ApprRejLateAdmissionAdmin_sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@RID", RID);
                cmd.Parameters.AddWithValue("@action", action);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
            finally
            {
                con.Close();
            }
        }
        public string UpdStsLateAdmissionAdmin(string ApprovalBy, string EmpUserId, string UserNM, string RID, string status, string ApprDate, string remarks)
        {
            SqlConnection con = null;
            string result = null;
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("UpdStsLateAdmissionAdmin_sp", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ApprovalBy", ApprovalBy);
                cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                cmd.Parameters.AddWithValue("@UserNM", UserNM);
                cmd.Parameters.AddWithValue("@RID", RID);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@ApprDate", ApprDate);
                cmd.Parameters.AddWithValue("@remarks", remarks);
         
                cmd.Parameters.Add("@Outstatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                result = Convert.ToString(cmd.Parameters["@Outstatus"].Value);
                return result;
            }
            catch (Exception ex)
            {
                return result;
            }
            finally
            {
                con.Close();
            }
        }
        public DataSet GetLateAdmRIDVerify(int RID)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetLateAdmRIDVerify_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RID", RID);
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
        public DataSet GetLateAdmRIDDataVerify(int RID, string CNM, string FNM, string MNM, string DOB)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetLateAdmRIDDataVerify_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RID", RID);
                    cmd.Parameters.AddWithValue("@CNM", CNM);
                    cmd.Parameters.AddWithValue("@FNM", FNM);
                    cmd.Parameters.AddWithValue("@MNM", MNM);
                    cmd.Parameters.AddWithValue("@DOB", DOB);
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
        #endregion Late Admission

        #region DM Capacity Letter
        public DataSet DMCapacityLetter(string Allowdist)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DMCapacityLetter_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Allowdist", Allowdist);
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
        public DataSet DMCapacityLetterCentrewise(string Allowdist, string SearchResult)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("DMCapacityLetterCentrewise_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Allowdist", Allowdist);
                    cmd.Parameters.AddWithValue("@cent", SearchResult);
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
        public DataSet DMgetallDistAllow()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("getallDistAllowDM_SP", con);
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
        #endregion DM Capacity Letter



        public static DataSet OtherBoardDocumentsBySchoolSP(int type,string search)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("OtherBoardDocumentsBySchoolSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@type", type);
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


        #region UnlockClusterTheoryMarks
        public static DataSet UnlockClusterTheoryMarks(DataTable dt1, int adminid, out string OutError)
        {
            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UnlockClusterTheoryMarksSP"; //GetAllFormName_sp
                cmd.Parameters.AddWithValue("@adminid", adminid);
                cmd.Parameters.AddWithValue("@tblUnlockClusterTheoryMarks", dt1);
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


        public static string CheckUnlockClusterTheoryExcel(DataSet ds, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("ErrStatus", typeof(string)));
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString().Length != 7 || ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string schl = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check SCHL " + schl + " in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] += "Please check SCHL " + schl + " in row " + RowNo + ",  ";

                    }

                    if (ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string ccode = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check CCODE " + ccode + " in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] += "Please check CCODE " + ccode + " in row " + RowNo + ",  ";
                    }

                    if (ds.Tables[0].Rows[i][2].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string Remarks = ds.Tables[0].Rows[i][2].ToString();
                        Result += "Please Enter Remarks " + Remarks + " in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] += "Please Enter Remarks " + Remarks + " in row " + RowNo + ",  ";
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


        #endregion

        #region UnlockStudentPreviousYearMarks
        public static DataSet UnlockStudentPreviousYearMarks(DataTable dt1, int adminid, out string OutError)
        {
            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UnlockStudentPreviousYearMarksSP"; //GetAllFormName_sp
                cmd.Parameters.AddWithValue("@adminid", adminid);
                cmd.Parameters.AddWithValue("@tblUnlockStudentPreviousYearMarks", dt1);
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


        public static string CheckUnlockStudentPreviousYearMarksExcel(DataSet ds, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("ErrStatus", typeof(string)));
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString().Length != 7 || ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string schl = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check SCHL " + schl + " in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] += "Please check SCHL " + schl + " in row " + RowNo + ",  ";

                    }

                    if (ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string Remarks = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please Enter Remarks " + Remarks + " in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] += "Please Enter Remarks " + Remarks + " in row " + RowNo + ",  ";
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


        #endregion

        #region Unlock Senior Student Matric Result Marks
        public static DataSet UnlockSeniorStudentMatricResultMarks(DataTable dt1, int adminid, out string OutError)
        {
            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "UnlockSeniorStudentMatricResultMarksSP"; //GetAllFormName_sp
                cmd.Parameters.AddWithValue("@adminid", adminid);
                cmd.Parameters.AddWithValue("@tblUnlockSeniorStudentMatricResultMarks", dt1);
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


        public static string CheckUnlockSeniorStudentMatricResultMarksExcel(DataSet ds, out DataTable dt)
        {
            string Result = "";
            try
            {
                dt = ds.Tables[0];
                dt.Columns.Add(new DataColumn("ErrStatus", typeof(string)));
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString().Length != 7 || ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string schl = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check SCHL " + schl + " in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] += "Please check SCHL " + schl + " in row " + RowNo + ",  ";

                    }

                    if (ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string Remarks = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please Enter Remarks " + Remarks + " in row " + RowNo + ",  ";
                        dt.Rows[i]["ErrStatus"] += "Please Enter Remarks " + Remarks + " in row " + RowNo + ",  ";
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

		public DataSet UpdateEXFileData(DataTable dt,string schl,out string OutError)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
                
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("update_Ex_file_data_sp", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@UpdateEXFile_tblType", dt);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.Add("@outError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
					ad.SelectCommand = cmd;
					ad.Fill(result);
					con.Open();
					OutError = (string)cmd.Parameters["@OutError"].Value;
					return result;
				}
			}
			catch (Exception ex)
			{
				OutError = ex.Message;
				return result = null;
			}
		}

		#endregion

	}
}