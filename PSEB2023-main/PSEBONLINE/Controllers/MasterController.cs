using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSEBONLINE.Models;
using System.Data;
using System.IO;
using System.Web.Services;
using Newtonsoft.Json;
using System.Web.Routing;
using ClosedXML.Excel;
using System.Web.Caching;
using System.Web.UI;
using PSEBONLINE.AbstractLayer;
using System.Threading.Tasks;
using PSEBONLINE.Repository;
using PSEBONLINE.Filters;
using System.Data.Entity;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon;
using System.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;

namespace PSEBONLINE.Controllers
{
    [RoutePrefix("Master")]
    public class MasterController : Controller
    {
        private const string BUCKET_NAME = "psebdata";

        private readonly DBContext _context = new DBContext();
        string sp = System.Configuration.ConfigurationManager.AppSettings["upload"];
        //AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        //AbstractLayer._schoolRepository.objDB = new AbstractLayer._schoolRepository.);
        //AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();        
        string sp1 = System.Configuration.ConfigurationManager.AppSettings["ImagePathCor"];

        public JsonResult SchoolPrincipalMobileOTP(string schl, string ePrincipalName, string eMOBILENO, string mobileOTP,  string confirmOTP,string Type)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            try
            {
                string dee = "0";
                string outstatus = "";
                string OTP = "";

                if (Type.ToUpper() == "SEND")
                {
                    Session["ExamCentreConfidentialOTP"] = null;
                    if (!string.IsNullOrEmpty(eMOBILENO) && eMOBILENO.Length == 10)
                    {
                        OTP = AbstractLayer.DBClass.GenerateOTP();
                       // string Sms = "Please enter your OTP : " + OTP + " to Verify Mobile for Exam Centre. Regards PSEB";

                        string Sms = String.Format("Use {0} as Your OTP to access your Login {1} . Never Share your OTP with Any unauthorized Person. \nRegards\nPSEB Mohali", OTP.ToString(), schl);
                        // string getSms = new AbstractLayer.DBClass().gosmsPSEB("97xxx8xxxx", Sms);
                        string getSms = new AbstractLayer.DBClass().gosms(eMOBILENO, Sms);
                        if (getSms.ToLower().Contains("success"))
                        {                            
                            ExamCentreConfidentialResources examCentreConfidentialResources = _context.ExamCentreConfidentialResources.Where(s=>s.schl == schl && !s.isdeleted).FirstOrDefault();
                            if (examCentreConfidentialResources != null)
                            {
                                examCentreConfidentialResources.deletedBy = schl;
                                examCentreConfidentialResources.deletedOn = DateTime.Now;
                                examCentreConfidentialResources.isdeleted = true;
                                _context.Entry(examCentreConfidentialResources).State = EntityState.Modified;
                                _context.SaveChanges();
                            }
                            Session["ExamCentreConfidentialOTP"] = OTP; 
                            dee = "1";
                        }
                        dee = "1";
                    }
                    return Json(new { outstatus = dee, name = ePrincipalName, mob = eMOBILENO, otp = OTP }, JsonRequestBehavior.AllowGet);
                }
                else if (Type.ToUpper() == "VERIFIY")
                {

                    if (!string.IsNullOrEmpty(eMOBILENO) && eMOBILENO.Length == 10)
                    {
                        if (Session["ExamCentreConfidentialOTP"] != null)
                        {
                            if (Session["ExamCentreConfidentialOTP"].ToString().Trim() == mobileOTP.Trim() || mobileOTP.ToString().Trim() == "PSEBQA".Trim())
                            {
                                // insert to DB
                                ExamCentreConfidentialResources _model = new ExamCentreConfidentialResources()
                                {
                                    id = 0,
                                    schl = schl,
                                    principal = ePrincipalName,
                                    mobile = eMOBILENO,
                                    otp = mobileOTP,
                                    submitBy = schl,
                                    submitOn = DateTime.Now,
                                    isdeleted = false,
                                    downloadCount = 1,
                                    downloadOn = DateTime.Now,
                            };
                                _context.ExamCentreConfidentialResources.Add(_model);
                                int result =  _context.SaveChanges();                                
                                if (result > 0)
                                {
                                    dee = "1";
                                    Session["ExamCentreConfidentialOTP"] = null;
                                }
                                else
                                {
                                    dee = "0";
                                    Session["ExamCentreConfidentialOTP"] = null;
                                }
                            }
                            else
                            {
                                dee = "2";

                            }
                        }
                    }
                    return Json(new { outstatus = dee, name = ePrincipalName, mob = eMOBILENO, otp = OTP }, JsonRequestBehavior.AllowGet);
                }

                dee = outstatus;
                return Json(new { outstatus = dee, name = ePrincipalName, mob = eMOBILENO, otp = OTP }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public void ExportDataFromDataTable(DataTable dt, string FileNAME)
        {           
            using (XLWorkbook wb = new XLWorkbook())
            {

              string  fileName1 = FileNAME + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";

                wb.Worksheets.Add(dt);
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=" + fileName1 + "");               
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
           
        }

        public ActionResult RegnoErrorSummaryDownloadData()
        {

            try
            {
                if (Request.QueryString["FormName"] == null)
                {
                    return RedirectToAction("Welcome", "Admin");
                }               
                else
                {
                    string FormName = Request.QueryString["FormName"].ToString();
                    string fileName1 = FormName + "_RegnoErrorDownloadData" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";  //103_230820162209_347


                    var regnoErrorSummaryDownloadDataViews = _context.RegnoErrorSummaryDownloadDataViews.Where(s => s.Form == FormName).ToList();

                    if (regnoErrorSummaryDownloadDataViews.Count > 0)
                    {


                        DataTable dataTable = StaticDB.ConvertListToDataTable(regnoErrorSummaryDownloadDataViews);

                        DataSet objDs = new DataSet();
                         objDs.Tables.Add(dataTable);
                        objDs.AcceptChanges();

                        if (dataTable.Rows.Count > 0)
                        {
                            bool ResultDownload = false;
                            try
                            {
                                using (XLWorkbook wb = new XLWorkbook())
                                {
                                    wb.Worksheets.Add(objDs.Tables[0]);//dt
                                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                    wb.Style.Font.Bold = true;
                                    Response.Clear();
                                    Response.Buffer = true;
                                    Response.Charset = "";
                                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                    Response.AddHeader("content-disposition", "attachment;filename=" + fileName1 + "");
                                    using (MemoryStream MyMemoryStream = new MemoryStream())
                                    {
                                        wb.SaveAs(MyMemoryStream);
                                        MyMemoryStream.WriteTo(Response.OutputStream);
                                        Response.Flush();
                                        Response.End();
                                    }
                                }
                                ResultDownload = true;
                            }
                            catch (Exception ex)
                            {
                                ResultDownload = false;
                            }
                        }
                    }


                }

                return RedirectToAction("Welcome", "Admin");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Welcome", "Admin");
            }
        }


      

        //#region exporttoDBF
        //public string CrateTableSchema(DataTable dt, string tablename)
        //{
        //    string table = "";
        //    table += "create table " + tablename + "";
        //    table += "(";
        //    for (int i = 0; i < dt.Columns.Count; i++)
        //    {
        //        if (i != dt.Columns.Count - 1)
        //            table += "[" + Common.Truncate(dt.Columns[i].ColumnName, 10) + "] " + "varchar(200)" + ",";
        //        else
        //            table += "[" + Common.Truncate(dt.Columns[i].ColumnName, 10) + "] " + "varchar(200)";
        //    }
        //    table += ") ";
        //    return table;
        //}

        //public FileResult CreateDBFFile(DataTable dataTable)
        //{

        //   // marksheetController marksheet = new marksheetController();
        //    //DataTable dataTable = marksheet.gDataSet(cls, roll, yr, "2").Tables[0];
        //    string filepath = null;

        //    filepath = Server.MapPath("~//Content//Upload//");

        //    string TableName = "D" + DateTime.Now.ToString("yyyyMMddTHHmmss");
        //    //string conStr = "Provider = Microsoft.Jet.OLEDB.4.0; " + " Data Source = " + filepath + "; " + "Extended Properties = dBase IV";
        //    string conStr = @"Provider =vfpoledb; Data Source =" + filepath + ";  Collating Sequence =machine";
        //    using (dBaseConnection = new OleDbConnection(conStr))
        //    {
        //        dBaseConnection.Open();

        //        OleDbCommand olecommand = dBaseConnection.CreateCommand();

        //        if ((System.IO.File.Exists(filepath + "" + TableName + ".dbf")))
        //        {
        //            System.IO.File.Delete(filepath + "" + TableName + ".dbf");
        //            olecommand.CommandText = CrateTableSchema(dataTable, TableName);
        //            olecommand.ExecuteNonQuery();
        //        }
        //        else
        //        {
        //            olecommand.CommandText = CrateTableSchema(dataTable, TableName);
        //            olecommand.ExecuteNonQuery();
        //        }
        //        OleDbDataAdapter oleadapter = new OleDbDataAdapter(olecommand);
        //        OleDbCommand oleinsertCommand = dBaseConnection.CreateCommand();


        //        foreach (DataRow dr in dataTable.Rows)
        //        {
        //            string fields = "", values = "";
        //            for (int i = 0; i < dataTable.Columns.Count; i++)
        //            {
        //                fields += "[" + Common.Truncate(dataTable.Columns[i].ColumnName, 10) + "],";
        //                values += "'" + dr[i].ToString().Trim() + "',";
        //            }
        //            fields = fields.Substring(0, fields.Length - 1);
        //            values = values.Substring(0, values.Length - 1);


        //            oleinsertCommand.CommandText = "INSERT INTO [" + TableName + "] (" + fields + ") VALUES (" + values + ")";

        //            oleinsertCommand.ExecuteNonQuery();
        //        }
        //    }

        //    FileStream sourceFile = new FileStream(filepath + "" + TableName.Trim() + ".dbf", FileMode.Open);
        //    float FileSize = 0;
        //    FileSize = sourceFile.Length;
        //    byte[] getContent = new byte[Convert.ToInt32(Math.Truncate(FileSize))];
        //    sourceFile.Read(getContent, 0, Convert.ToInt32(sourceFile.Length));
        //    sourceFile.Close();
        //    string filename = TableName.Trim() + ".dbf";
        //    return File(getContent, "content-disposition", filename);


        //}
        //#endregion




        #region StudentSchoolMigration

        // CancelStudentSchoolMigration
        public JsonResult CancelStudentSchoolMigration(string cancelremarks, string stdid, string migid, string Type)
        {
            try
            {
                string dee = "";
                string outstatus = "";
                string UpdatedBy = "";
                if (Session["AdminType"] == null && Session["SCHL"] == null)
                {
                    return Json(new { sn = dee, chid = outstatus }, JsonRequestBehavior.AllowGet);
                }

                if (Session["SCHL"] != null)
                {
                    UpdatedBy = "SCHL-" + Session["SCHL"].ToString();
                }

                if (Session["AdminType"] != null)
                {
                    UpdatedBy = "ADMIN-" + Session["AdminId"].ToString();
                }                
                
                string result = SchoolDB.CancelStudentSchoolMigration(cancelremarks, stdid, migid, out outstatus, UpdatedBy, Type);//ChallanDetailsCancelSP                
                dee = outstatus;
                return Json(new { sn = dee, chid = outstatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // CancelStudentSchoolMigration
        public JsonResult UpdateStatusStudentSchoolMigration(string remarks, string stdid, string migid, string status, string AppLevel, string Type)
        {
            string dee = "";
            string outstatus = "";
            string UpdatedBy = "";
            string EmpUserId = "";
            try
            {
              
                if (Session["AdminType"] == null && Session["SCHL"] == null)
                {
                    return Json(new { sn = dee, chid = outstatus }, JsonRequestBehavior.AllowGet);
                }

                if (Session["SCHL"] != null)
                {
                    UpdatedBy = "SCHL-" + Session["SCHL"].ToString();
                    EmpUserId = Session["SCHL"].ToString();
                }

                if (Session["AdminType"] != null)
                {
                    AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
                    UpdatedBy = "ADMIN-" + Session["AdminId"].ToString();
                    EmpUserId = adminLoginSession.AdminEmployeeUserId;
                }
                StudentSchoolMigrationViewModel studentSchoolMigrationViewModel = new StudentSchoolMigrationViewModel();
                string result = SchoolDB.UpdateStatusStudentSchoolMigration(EmpUserId,remarks, stdid, migid, status, AppLevel, out outstatus, UpdatedBy, Type);//ChallanDetailsCancelSP                
                dee = outstatus;
                if (outstatus == "1")
                {                    
                    string upStautus = status == "A" ? "Approved" : status == "R" ? "Rejected" : "Updated";
                    string SchoolMobile = "";
                    string Search = "MigrationId =" + migid;
                    List<StudentSchoolMigrationViewModel> studentSchoolMigrationViewModelList = new AbstractLayer.SchoolDB().StudentSchoolMigrationsSearchModel(2, Search, ""); // type=2 for mig
                    if (studentSchoolMigrationViewModelList.Count() > 0)
                    {
                        ViewBag.TotalCount = studentSchoolMigrationViewModelList.Count;
                        studentSchoolMigrationViewModel = studentSchoolMigrationViewModelList.Where(s => s.MigrationId == Convert.ToInt32(migid)).FirstOrDefault();
                        SchoolMobile = studentSchoolMigrationViewModel.NEWSCHLMOBILE;

                        if (!string.IsNullOrEmpty(SchoolMobile))
                        {
                            string Sms = "";
                            if (AppLevel.ToUpper() == "SCHL".ToUpper() && !string.IsNullOrEmpty(SchoolMobile))
                            {
                                //SchoolMobile = Session["SchoolMobile"].ToString();
                                Sms = "School to School Migration of Student " + stdid + " of Class is " + upStautus + " by old school. Check status under School Migration -> Applied List. Regards PSEB";
                            }
                            else if (AppLevel.ToUpper() == "HOD".ToUpper() && !string.IsNullOrEmpty(SchoolMobile))
                            {
                                // SchoolMobile = Session["SchoolMobile"].ToString();
                                Sms = "School to School Migration of Student " + stdid + " of Class is " + upStautus + " by Head Office. Check status under School Migration -> Applied List. Regards PSEB";
                            }
                            try
                            {
                                 string getSms = new AbstractLayer.DBClass().gosms(SchoolMobile, Sms);
                            }
                            catch (Exception) { }
                        }
                    }
                }

                return Json(new { sn = dee, chid = outstatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sn = dee, chid = outstatus }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        #region registration JSONResult

        #region UpdateAadharEnrollNo
        public JsonResult UpdateAadharEnrollNo(string std_id, string aadhar_num, string SCHL, string Caste, string gender, string BPL, string Rel, string Epunid)
        {           
            RegistrationModels rm = new RegistrationModels();
            try
            {

                string dee = "";
                string outstatus = "";
                string Search = string.Empty;
                DataSet res1 =  new AbstractLayer.RegistrationDB().UpdaadharEnrollmentNo(std_id, aadhar_num, SCHL, Caste, gender, BPL, Rel, Epunid);
                string res = res1.Tables[0].Rows[0]["res"].ToString();
                if (res != "0")
                {
                    //dee = res;
                    dee = "Yes";
                }
                else
                    dee = "No";

                return Json(new { sn = dee, chid = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {                
                 return null;
            }
        }
        #endregion UpdateAadharEnrollNo


        [HttpPost]
        public JsonResult CancelStdRegNo(string Remarks, string stdid)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
            try
            {
                string dee = "";
                string res = null;
                DataSet result = objDB.CancelStdRegNo(Remarks, stdid,adminLoginSession.AdminEmployeeUserId);
                res = result.Tables[0].Rows.Count.ToString();
                if (result.Tables[0].Rows.Count.ToString() != "0")
                {
                    dee = "Yes";
                }
                else
                    dee = "No";


                return Json(new { sn = dee, chid = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {               
                return null;
            }
        }


        [HttpPost]
        public JsonResult SwitchForm(string Remarks, string OldRegNo, string stdid)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            try
            {
                string dee = "";
                int OutStatus = 0;
                string UpdatedBy = "";

                if (Session["AdminType"] == null && Session["SCHL"] == null)
                {
                    return Json(new { sn = dee, chid = OutStatus }, JsonRequestBehavior.AllowGet);
                }

                if (Session["SCHL"] != null)
                {
                    UpdatedBy = "SCHL-" + Session["SCHL"].ToString();
                }

                if (Session["AdminType"] != null)
                {
                    UpdatedBy = "ADMIN-" +Session["AdminId"].ToString();
                }

                int result = RegistrationDB.SwitchForm(Remarks, stdid, OldRegNo, UpdatedBy, out OutStatus);
                if (OutStatus == 1)
                {
                    dee = "Yes";
                }
                else if (OutStatus == 0)
                {
                    dee = "NotAllowed";
                }
                else
                { dee = "No"; }

                return Json(new { sn = dee, chid = OutStatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [HttpPost]
        public JsonResult ShiftAllFormAdmin(string Remarks, string OldRegNo, string stdid,string OtherBoard, string ShiftFormNM,string VerifiedEmp)
        {
            try
            {
                AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

                string dee = "";
                int OutStatus = 0;
                string UpdatedBy = "";

                if (Session["AdminType"] == null && Session["SCHL"] == null)
                {
                    return Json(new { sn = dee, chid = OutStatus }, JsonRequestBehavior.AllowGet);
                }

                if (Session["SCHL"] != null)
                {
                    UpdatedBy = "SCHL-" + Session["SCHL"].ToString();
                }

                if (Session["AdminType"] != null)
                {
                    UpdatedBy = "ADMIN-" + Session["AdminId"].ToString();
                }

                int result = RegistrationDB.ShiftAllFormAdmin(Remarks, stdid, OldRegNo, UpdatedBy, out OutStatus,OtherBoard, ShiftFormNM, VerifiedEmp,adminLoginSession.AdminEmployeeUserId);
                if (OutStatus == 1)
                {
                    dee = "Yes";
                }
                else if (OutStatus == 0)
                {
                    dee = "NotAllowed";
                }
                else
                { dee = "No"; }

                return Json(new { sn = dee, chid = OutStatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        [AdminLoginCheckFilter]
        [HttpPost]
        public JsonResult ModifyRegistrationData(string std_id, string RegNo, string Remarks, string GroupName)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            RegistrationModels rm = new RegistrationModels();

            try
            {               
                int outstatus = 0;
                string Search = string.Empty;
                 int result = AbstractLayer.RegistrationDB.ModifyRegistrationData(std_id, RegNo.Trim(), Remarks, GroupName,adminLoginSession.AdminEmployeeUserId, out outstatus);              

                return Json(new { sn = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {                
                return null;
            }
        }
      

        #endregion


        // GET: Master
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult JqUpdateOtherBoardDocuments()
        {
            try
            {
                RegistrationModels rm = new RegistrationModels();
                // Checking no of files injected in Request object  
                if (Request.Files.Count > 0)
                {
                    try
                    {
                        int flag = 0;
                        //  Get all files from Request object  
                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {

                            HttpPostedFileBase file = files[i];
                            string fname;                           
                            string fileKey = i == 0 ? "DocProofCertificate" : i == 1 ? "DocProofNRICandidates" : "";
                            string result = Request.Form["StudentUniqueId"].ToString();
                            string formName = Request.Form["formName"].ToString();
                            string schlDist = Request.Form["schlDist"].ToString();
                            string stdid = Request.Form["stdid"].ToString();
                            // Get the complete folder path and store the file inside it.  
                            //fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                            //file.SaveAs(fname);


                            if (file != null && fileKey == "DocProofCertificate")
                            {
                                string fileExt = Path.GetExtension(file.FileName);
                                //var path = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/" + formName + "/" + schlDist + "/ProofCertificate"), result + "C" + fileExt);
                                //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/" + formName + "/" + schlDist + "/ProofCertificate"));
                                //if (!Directory.Exists(FilepathExist))
                                //{
                                //    Directory.CreateDirectory(FilepathExist);
                                //}
                                //file.SaveAs(path);
                                string Orgfile = result + "C" + fileExt;
                                using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                                {
                                    using (var newMemoryStream = new MemoryStream())
                                    {
                                        var uploadRequest = new TransferUtilityUploadRequest
                                        {
                                            InputStream = file.InputStream,
                                            Key = string.Format("allfiles/Upload2023/" + formName + "/" + schlDist + "/ProofCertificate/{0}", Orgfile),
                                            BucketName = BUCKET_NAME,
                                            CannedACL = S3CannedACL.PublicRead
                                        };

                                        var fileTransferUtility = new TransferUtility(client);
                                        fileTransferUtility.Upload(uploadRequest);
                                    }
                                }

                                rm.ProofCertificate = formName + "/" + schlDist + "/ProofCertificate" + "/" + result + "C" + fileExt;
                                string type = "CM";
                                string UpdatePicC = new AbstractLayer.RegistrationDB().Updated_Pic_Data(result, rm.ProofCertificate, type);
                                flag = 1;
                            }

                            if (file != null && fileKey == "DocProofNRICandidates")
                            {
                                string fileExt = Path.GetExtension(file.FileName);
                                string pathName = formName + "/" + schlDist + "/ProofNRICandidates";
                                //var path = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/" + pathName), result + "NRI" + fileExt);
                                //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/" + pathName));
                                //if (!Directory.Exists(FilepathExist))
                                //{
                                //    Directory.CreateDirectory(FilepathExist);
                                //}
                                //file.SaveAs(path);
                                string Orgfile = result + "NRI" + fileExt;
                                using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                                {
                                    using (var newMemoryStream = new MemoryStream())
                                    {
                                        var uploadRequest = new TransferUtilityUploadRequest
                                        {
                                            InputStream = file.InputStream,
                                            Key = string.Format("allfiles/Upload2023/" + formName + "/" + schlDist + "/ProofNRICandidates/{0}", Orgfile),
                                            BucketName = BUCKET_NAME,
                                            CannedACL = S3CannedACL.PublicRead
                                        };

                                        var fileTransferUtility = new TransferUtility(client);
                                        fileTransferUtility.Upload(uploadRequest);
                                    }
                                }

                                rm.ProofNRICandidates = pathName + "/" + result + "NRI" + fileExt;
                                string type = "NRIM";
                                string UpdatePicC = new AbstractLayer.RegistrationDB().Updated_Pic_Data(result, rm.ProofNRICandidates, type);
                                flag = 1;
                            }
                        }
                        // Returns message that successfully uploaded  

                        if (flag == 0)
                        {
                            return Json(new { oid = flag, msg = "failure" }, JsonRequestBehavior.AllowGet);                            
                        }
                        else
                        {
                            return Json(new {  oid = flag,msg="success" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { oid = 5, msg = "Error occurred : " + ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { oid = 2, msg = "No files Selected" }, JsonRequestBehavior.AllowGet);
           
                }
            }
            catch (Exception ex)
            {
                return Json(new { oid = 5, msg = "Error occurred : " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult JqUpdateOtherBoardDocumentsByAdminUser()
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            string AdminId = Session["AdminId"].ToString();
            string AdminUser = Session["AdminUser"].ToString();
            try
            {
                RegistrationModels rm = new RegistrationModels();
                // Checking no of files injected in Request object  
                if (Request.Files.Count > 0)
                {
                    try
                    {
                        int flag = 0;
                        //  Get all files from Request object  
                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {

                            HttpPostedFileBase file = files[i];
                            string fname;
                            string fileKey = i == 0 ? "DocAddDocument" : "";
                            string result = Request.Form["StudentUniqueId"].ToString();
                            string formName = Request.Form["formName"].ToString();
                            string schlDist = Request.Form["schlDist"].ToString();
                            string stdid = Request.Form["stdid"].ToString();
                            string addDocumentRemarks = Request.Form["Remarks"].ToString();
                            // Get the complete folder path and store the file inside it.  
                            //fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                            //file.SaveAs(fname);
                            string myUniqueFileName = StaticDB.GenerateFileName(result);

                            if (file != null && fileKey == "DocAddDocument")
                            {
                                string fileExt = Path.GetExtension(file.FileName);
                                //var path = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/"  + "OtherBoardDocumentsByAdminUser"), myUniqueFileName + fileExt);
                                //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/"  + "OtherBoardDocumentsByAdminUser"));
                                //if (!Directory.Exists(FilepathExist))
                                //{
                                //    Directory.CreateDirectory(FilepathExist);
                                //}
                                //file.SaveAs(path);
                                string Orgfile = myUniqueFileName + fileExt;
                                using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                                {
                                    using (var newMemoryStream = new MemoryStream())
                                    {
                                        var uploadRequest = new TransferUtilityUploadRequest
                                        {
                                            InputStream = file.InputStream,
                                            Key = string.Format("allfiles/Upload2023/OtherBoardDocumentsByAdminUser/{0}", Orgfile),
                                            BucketName = BUCKET_NAME,
                                            CannedACL = S3CannedACL.PublicRead
                                        };

                                        var fileTransferUtility = new TransferUtility(client);
                                        fileTransferUtility.Upload(uploadRequest);
                                    }
                                }


                                rm.ProofCertificate = "OtherBoardDocumentsByAdminUser" + "/" + myUniqueFileName + fileExt;
                                string type = "AD";
                                // string UpdatePicC = new AbstractLayer.RegistrationDB().Updated_Pic_Data(result, rm.ProofCertificate, type);

                                tblOtherBoardDocumentsByAdminUsers tblOtherBoardDocuments = new tblOtherBoardDocumentsByAdminUsers()
                                {
                                    Stdid = long.Parse(stdid),
                                    Filepath = rm.ProofCertificate,
                                    Remarks= addDocumentRemarks,
                                    IsActive = true,
                                    SubmitOn = DateTime.Now,
                                    SubmitBy = AdminUser,
                                    EmpUserId = adminLoginSession.AdminEmployeeUserId
                                };

                                _context.tblOtherBoardDocumentsByAdminUsers.Add(tblOtherBoardDocuments);
                                int insertedRecords = _context.SaveChanges();
                                if (insertedRecords > 0)
                                {
                                    flag = 1;
                                }
                                else
                                {
                                    flag = 0;
                                }
                            }

                           
                        }
                        // Returns message that successfully uploaded  

                        if (flag == 0)
                        {
                            return Json(new { oid = flag, msg = "failure" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { oid = flag, msg = "success" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { oid = 5, msg = "Error occurred : " + ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { oid = 2, msg = "No files Selected" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(new { oid = 5, msg = "Error occurred : " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [SessionCheckFilter]
        [HttpPost]        
        public ActionResult JqUpdateOtherBoardDocumentsBySchool()
        {

            string Schl = Session["Schl"].ToString();          
            try
            {
                RegistrationModels rm = new RegistrationModels();
                // Checking no of files injected in Request object  
                if (Request.Files.Count > 0)
                {
                    try
                    {
                        int flag = 0;
                        //  Get all files from Request object  
                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {

                            HttpPostedFileBase file = files[i];
                            string fname;
                            string fileKey = i == 0 ? "DocAddDocument" : "";
                            string result = Request.Form["StudentUniqueId"].ToString();
                            string formName = Request.Form["formName"].ToString();
                            string schlDist = Request.Form["schlDist"].ToString();
                            string stdid = Request.Form["stdid"].ToString();
                            string addDocumentRemarks = Request.Form["Remarks"].ToString();
                            // Get the complete folder path and store the file inside it.  
                            //fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                            //file.SaveAs(fname);
                            string myUniqueFileName = StaticDB.GenerateFileName(result);

                            if (file != null && fileKey == "DocAddDocument")
                            {
                                string fileExt = Path.GetExtension(file.FileName);
                                //var path = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/" + "OtherBoardDocumentsBySchool"), myUniqueFileName + fileExt);
                                //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/" + "OtherBoardDocumentsBySchool"));
                                //if (!Directory.Exists(FilepathExist))
                                //{
                                //    Directory.CreateDirectory(FilepathExist);
                                //}
                                //file.SaveAs(path);
                                string Orgfile = myUniqueFileName + fileExt;
                                using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                                {
                                    using (var newMemoryStream = new MemoryStream())
                                    {
                                        var uploadRequest = new TransferUtilityUploadRequest
                                        {
                                            InputStream = file.InputStream,
                                            Key = string.Format("allfiles/Upload2023/OtherBoardDocumentsBySchool/{0}", Orgfile),
                                            BucketName = BUCKET_NAME,
                                            CannedACL = S3CannedACL.PublicRead
                                        };

                                        var fileTransferUtility = new TransferUtility(client);
                                        fileTransferUtility.Upload(uploadRequest);
                                    }
                                }
                                rm.ProofCertificate = "OtherBoardDocumentsBySchool" + "/" + myUniqueFileName + fileExt;
                                string type = "AD";
                                
                                tblOtherBoardDocumentsBySchool tblOtherBoardDocuments = new tblOtherBoardDocumentsBySchool()
                                {
                                    Stdid = long.Parse(stdid),
                                    Filepath = rm.ProofCertificate,
                                    Remarks = addDocumentRemarks,
                                    IsActive = true,
                                    SubmitOn = DateTime.Now,
                                    SubmitBy = Schl
                                };

                                _context.tblOtherBoardDocumentsBySchool.Add(tblOtherBoardDocuments);
                                int insertedRecords = _context.SaveChanges();
                                if (insertedRecords > 0)
                                {
                                    flag = 1;
                                }
                                else
                                {
                                    flag = 0;
                                }
                            }


                        }
                        // Returns message that successfully uploaded  

                        if (flag == 0)
                        {
                            return Json(new { oid = flag, msg = "failure" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { oid = flag, msg = "success" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { oid = 5, msg = "Error occurred : " + ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { oid = 2, msg = "No files Selected" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(new { oid = 5, msg = "Error occurred : " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult JqSendEaffObjectionResponse(AffObjectionLettersResponseModel affObjectionLettersResponseModel)
        {           
            try
            {
                RegistrationModels rm = new RegistrationModels();
                // Checking no of files injected in Request object  
                if (Request.Files.Count > 0)
                {
                    try
                    {
                        int flag = 0;
                        //  Get all files from Request object  
                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {

                            HttpPostedFileBase file = files[i];
                            string fileKey = i == 0 ? "ObjectionAttachment" : "";
                            string result = affObjectionLettersResponseModel.AppNo + "_" + affObjectionLettersResponseModel.ObjCode + "_" + affObjectionLettersResponseModel.OLID;
                            string myUniqueFileName = StaticDB.GenerateFileName(result);
                            string attachmentName = "";
                            string FilepathExist = "";
                            string path = "";
                            if (file != null)
                            {
                                string fileExt = Path.GetExtension(file.FileName);
                                //path = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/" + "AFFObjectionLetter/" + affObjectionLettersResponseModel.AppType), myUniqueFileName + fileExt);
                                //FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/" + "AFFObjectionLetter/" + affObjectionLettersResponseModel.AppType));
                                path = Path.Combine(Server.MapPath("allfiles/Upload2023/AFFObjectionLetter/" + affObjectionLettersResponseModel.AppType), myUniqueFileName + fileExt);
                                FilepathExist = Path.Combine(Server.MapPath("allfiles/Upload2023/AFFObjectionLetter/" + affObjectionLettersResponseModel.AppType));

                                attachmentName = "AFFObjectionLetter/" + affObjectionLettersResponseModel.AppType + "/" + myUniqueFileName + fileExt;
                            }


                            using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                            {
                                try
                                {
                                    AffObjectionLetters affObjectionLetters = _context.AffObjectionLetters.Find(affObjectionLettersResponseModel.OLID);
                                    affObjectionLetters.OLID = affObjectionLettersResponseModel.OLID;
                                    affObjectionLetters.Attachment = attachmentName;
                                    affObjectionLetters.SchoolReply = affObjectionLettersResponseModel.ObjectionSchoolReply;
                                    affObjectionLetters.SchoolReplyOn = DateTime.Now;
                                    _context.Entry(affObjectionLetters).State = EntityState.Modified;

                                    int insertedRecords = _context.SaveChanges();
                                    if (insertedRecords > 0)
                                    {
                                        flag = 1;
                                        if (file != null && attachmentName != "")
                                        {
                                            //if (!Directory.Exists(FilepathExist))
                                            //{
                                            //    Directory.CreateDirectory(FilepathExist);
                                            //}
                                            //file.SaveAs(path);
                                            string fileExt = Path.GetExtension(file.FileName);
                                            string Orgfile = myUniqueFileName + fileExt;
                                            using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                                            {
                                                using (var newMemoryStream = new MemoryStream())
                                                {
                                                    var uploadRequest = new TransferUtilityUploadRequest
                                                    {
                                                        InputStream = file.InputStream,
                                                        Key = string.Format("allfiles/Upload2023/AFFObjectionLetter/{0}", Orgfile),
                                                        BucketName = BUCKET_NAME,
                                                        CannedACL = S3CannedACL.PublicRead
                                                    };

                                                    var fileTransferUtility = new TransferUtility(client);
                                                    fileTransferUtility.Upload(uploadRequest);
                                                }
                                            }

                                        }                                        
                                    }
                                    else
                                    {
                                        flag = 0;
                                    }
                                    transaction.Commit();//transaction commit
                                }
                                catch (Exception ex1)
                                {
                                    transaction.Rollback();
                                }
                            }
                        }
                        // Returns message that successfully uploaded  

                        if (flag == 0)
                        {
                            return Json(new { oid = flag, msg = "failure" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { oid = flag, msg = "success" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { oid = 5, msg = "Error occurred : " + ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { oid = 2, msg = "No files Selected" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(new { oid = 5, msg = "Error occurred : " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        #region Eaff
        public JsonResult UpdateObjectionApprovalStatus(string remarks, string AppNo, string OLID, string status, string Type)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

            string dee = "";
            string outstatus = "";
           // string UpdatedBy = "";
            try
            {

                if (Session["AdminType"] == null)
                {
                    return Json(new { sn = dee, chid = outstatus }, JsonRequestBehavior.AllowGet);
                }
               
                //if (Session["AdminType"] != null)
                //{
                //    UpdatedBy = "ADMIN-" + Session["AdminId"].ToString();
                //}
                string AdminUser = Session["AdminUser"].ToString().ToUpper();

                int olid = Convert.ToInt32(OLID);
                string approvedStatus = AbstractLayer.DBClass.GetAcceptRejectDDL().Where(s=>s.Value == status).Select(s=>s.Text).FirstOrDefault();



                AffObjectionLetters affObjectionLetters = _context.AffObjectionLetters.Find(olid);
                affObjectionLetters.ApprovalStatus = approvedStatus;
                affObjectionLetters.ApprovalRemarks = remarks;
                affObjectionLetters.ApprovalOn = DateTime.Now;
                affObjectionLetters.ApprovalBy = AdminUser;
                affObjectionLetters.EmpUserId = adminLoginSession.AdminEmployeeUserId;
                affObjectionLetters.ApprovalIP = StaticDB.GetFullIPAddress();
                _context.Entry(affObjectionLetters).State = EntityState.Modified;
                int insertedRecords = _context.SaveChanges();
                if (insertedRecords > 0)
                {
                    dee = "1";
                    outstatus = "1";
                }
                else { dee = "0"; outstatus = "0"; }
                return Json(new { sn = dee, chid = outstatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sn = dee, chid = outstatus }, JsonRequestBehavior.AllowGet);
            }
        }


    
        #endregion


        [HttpPost]
        public ActionResult JqReceiptUpdateManual(ReceiptUpdateManualModel receiptUpdateManualModel)
        {
            try
            {
                RegistrationModels rm = new RegistrationModels();
                // Checking no of files injected in Request object  
                if (Request.Files.Count > 0)
                {
                    try
                    {
                        int flag = 0;
                        //  Get all files from Request object  
                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {

                            string feecode = receiptUpdateManualModel.feecode;

                            HttpPostedFileBase file = files[i];
                            string fileKey = i == 0 ? "ReceiptUpdate" : "";
                           // string result = receiptUpdateManualModel.appno + "_" + receiptUpdateManualModel.ObjCode + "_" + affObjectionLettersResponseModel.OLID;
                           // string myUniqueFileName = StaticDB.GenerateFileName(result);
                            string attachmentName = "";
                            string FilepathExist = "";
                            string filename = "", path = "";
                            //Upload2023/Affiliation/ReceiptScannedCopy/0012893_1_ReceiptScannedCopy.pdf
                            if (file != null)
                            {
                                string fileExt = Path.GetExtension(file.FileName);
                                if (feecode == "72")
                                {
                                    filename = receiptUpdateManualModel.Schl + "_" + receiptUpdateManualModel.challancategory + "_ReceiptScannedCopy" + fileExt;
                                    //path = Path.Combine(Server.MapPath("~/Upload/Upload2023/MagazineSchoolRequirements/ReceiptScannedCopy"), filename);
                                    //FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/MagazineSchoolRequirements/ReceiptScannedCopy"));
                                    //attachmentName = "Upload2023/MagazineSchoolRequirements/ReceiptScannedCopy/" + filename;
                                    attachmentName = "allfiles/Upload2023/MagazineSchoolRequirements/ReceiptScannedCopy/" + filename;
                                    receiptUpdateManualModel.ReceiptScannedCopy = attachmentName;
                                }
                                else if (feecode == "45")
                                {                                    
                                    filename = receiptUpdateManualModel.Schl + "_" + receiptUpdateManualModel.challancategory + "_ReceiptScannedCopy" + fileExt;
                                    //path = Path.Combine(Server.MapPath("~/Upload/Upload2023/Affiliation/ReceiptScannedCopy"), filename);
                                    //FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/Affiliation/ReceiptScannedCopy"));
                                    //attachmentName = "Upload2023/Affiliation/ReceiptScannedCopy/" + filename;
                                    attachmentName = "allfiles/Upload2023/Affiliation/ReceiptScannedCopy/" + filename;
                                    receiptUpdateManualModel.ReceiptScannedCopy = attachmentName;
                                }      
                            }

                            string OutError = "";
                            string result = AbstractLayer.BankDB.UpdateReceiptAttachmentManualSP(receiptUpdateManualModel, out OutError);
                            if (OutError =="1")
                            {
                                flag = 1;
                                if (file != null && attachmentName != "")
                                {
                                    //if (!Directory.Exists(FilepathExist))
                                    //{
                                    //    Directory.CreateDirectory(FilepathExist);
                                    //}
                                    //file.SaveAs(path);
                                    if (feecode == "72")
                                    {
                                        using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                                        {
                                            using (var newMemoryStream = new MemoryStream())
                                            {
                                                var uploadRequest = new TransferUtilityUploadRequest
                                                {
                                                    InputStream = rm.file.InputStream,
                                                    Key = string.Format("allfiles/Upload2023/MagazineSchoolRequirements/ReceiptScannedCopy/{0}", filename),
                                                    BucketName = BUCKET_NAME,
                                                    CannedACL = S3CannedACL.PublicRead
                                                };

                                                var fileTransferUtility = new TransferUtility(client);
                                                fileTransferUtility.Upload(uploadRequest);
                                            }
                                        }

                                    }
                                    else if (feecode == "45")
                                    {
                                        using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                                        {
                                            using (var newMemoryStream = new MemoryStream())
                                            {
                                                var uploadRequest = new TransferUtilityUploadRequest
                                                {
                                                    InputStream = rm.file.InputStream,
                                                    Key = string.Format("allfiles/Upload2023/Affiliation/ReceiptScannedCopy/{0}", filename),
                                                    BucketName = BUCKET_NAME,
                                                    CannedACL = S3CannedACL.PublicRead
                                                };

                                                var fileTransferUtility = new TransferUtility(client);
                                                fileTransferUtility.Upload(uploadRequest);
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        // Returns message that successfully uploaded  

                        if (flag == 0)
                        {
                            return Json(new { oid = flag, msg = "failure" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { oid = flag, msg = "success" }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { oid = 5, msg = "Error occurred : " + ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { oid = 2, msg = "No files Selected" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(new { oid = 5, msg = "Error occurred : " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        //New 15032022
        public JsonResult updateEaffApprovalStatusByChairman(string remarks, string AppNo, string ApprovalFileNo, string status, string AppType)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

            string dee = "";
            string outstatus = "";
            // string UpdatedBy = "";
            try
            {

                if (Session["AdminType"] == null)
                {
                    return Json(new { sn = dee, chid = outstatus }, JsonRequestBehavior.AllowGet);
                }

                string AdminUser = Session["AdminUser"].ToString().ToUpper();


                string approvedStatus = AbstractLayer.DBClass.GetAcceptRejectDDL().Where(s => s.Value == status).Select(s => s.Text).FirstOrDefault();
                dee = "1"; outstatus = "1";

                string ApprovalIP = StaticDB.GetFullIPAddress();
                string OutError = "0";

                if (string.IsNullOrEmpty(AppNo) || string.IsNullOrEmpty(AppType) || string.IsNullOrEmpty(status))
                {
                    return Json(new { sn = "-5", chid = "-5" }, JsonRequestBehavior.AllowGet);
                }


                DataSet ds = AbstractLayer.EAffiliationDB.EAffiliation_AppType_Approval(AppType, AppNo, approvedStatus, remarks, AdminUser, ApprovalFileNo, ApprovalIP, adminLoginSession.AdminEmployeeUserId, out OutError);

                if (OutError == "1")
                {
                    dee = "1";
                    outstatus = "1";
                }
                else { dee = "0"; outstatus = "0"; }
                return Json(new { sn = dee, chid = outstatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sn = dee, chid = outstatus }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult PullBackEAffiliationByAppNO(string PullBackRemarks, string APPNO, string AppType, string SCHLMOBILE)
        {
            try
            {
                string dee = "0";
                int outstatus = 0;
                string outError = "0";

                if (AppType.ToUpper() == "AFF".ToUpper())
                {
                    if (APPNO != "" && PullBackRemarks != "")
                    {
                        int AdminId = Convert.ToInt32(Session["AdminId"]);
                        EAffiliationModel _EAffiliationModel = new EAffiliationModel();
                        _EAffiliationModel.UpdatedBy = AdminId;
                        _EAffiliationModel.APPNO = APPNO;
                        _EAffiliationModel.Remarks = PullBackRemarks;
                        int result = new AbstractLayer.EAffiliationDB().EAffiliation(_EAffiliationModel, 30, out outError);
                        dee = outError;

                        //if (outError == "1")
                        //{
                        //    string Sms = "Your form for E-Affiliation with application " + APPNO + " is unlocked by PSEB. Kindly update form and submit again. You can check status of application on dashboard.Regards PSEB";
                        //    try
                        //    {
                        //        string getSms = new AbstractLayer.DBClass().gosms(SCHLMOBILE, Sms);
                        //        //string getSms = new AbstractLayer.DBClass().gosms("9711819184", Sms);
                        //    }
                        //    catch (Exception) { }
                        //}


                    }
                    else
                    { dee = "2"; }
                }
                else if (AppType.ToUpper() == "AC".ToUpper())
                {
                    if (APPNO != "" && PullBackRemarks != "")
                    {
                        int AdminId = Convert.ToInt32(Session["AdminId"]);
                        AffiliationModel _EAffiliationModel = new AffiliationModel();
                        _EAffiliationModel.UpdatedBy = AdminId;
                        _EAffiliationModel.SCHL = APPNO;
                        _EAffiliationModel.Remarks = PullBackRemarks;
                        int result = new AbstractLayer.AffiliationDB().AffiliationContinuationAction(_EAffiliationModel, 30, out outError);
                        dee = outError;

                        //if (outError == "1")
                        //{
                        //    string Sms = "Your form for Annual Progress with application " + APPNO + " is unlocked by PSEB. Kindly update form and submit again. Regards PSEB";
                        //    try
                        //    {
                        //        string getSms = new AbstractLayer.DBClass().gosms(SCHLMOBILE, Sms);
                        //        //string getSms = new AbstractLayer.DBClass().gosms("9711819184", Sms);
                        //    }
                        //    catch (Exception) { }
                        //}


                    }
                    else
                    { dee = "2"; }
                }
                else if (AppType.ToUpper() == "AS".ToUpper())
                {
                    if (APPNO != "" && PullBackRemarks != "")
                    {
                        int AdminId = Convert.ToInt32(Session["AdminId"]);
                        AdditionalSectionModel _EAffiliationModel = new AdditionalSectionModel();
                        _EAffiliationModel.UpdatedBy = AdminId;
                        _EAffiliationModel.SCHL = APPNO;
                        _EAffiliationModel.Remarks = PullBackRemarks;
                        int result = new AbstractLayer.AdditionalSectionDB().AdditionalSectionAction(_EAffiliationModel, 30, out outError);
                        dee = outError;

                        //if (outError == "1")
                        //{
                        //    string Sms = "Your form for Additional Section with application " + APPNO + " is unlocked by PSEB. Kindly update form and submit again.Regards PSEB";
                        //    try
                        //    {
                        //        string getSms = new AbstractLayer.DBClass().gosms(SCHLMOBILE, Sms);
                        //        //string getSms = new AbstractLayer.DBClass().gosms("9711819184", Sms);
                        //    }
                        //    catch (Exception) { }
                        //}


                    }
                    else
                    { dee = "2"; }
                }


                return Json(new { sn = dee }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sn = "-1" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult BindTCodebyDist(string dist)
        {
            List<SelectListItem> _list = new List<SelectListItem>();
            if (dist.ToLower() != "all")
            {

                _list = new AbstractLayer.SchoolDB().GetTCode().Where(s => s.Value == dist).ToList();
            }
            else
            {
                _list = new AbstractLayer.SchoolDB().GetTCode().ToList();
            }
            return Json(_list);
        }


        [HttpPost]
        public JsonResult UnlockApplication(AttendenceSummaryDetail obj)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            string sRemark = obj.remarks+ " [ "+ adminLoginSession.AdminEmployeeUserId + " - "+ adminLoginSession.AdminEmployeeName + " ]";

            AttendanceResponse response = new AttendanceResponse();
            string outError = "0";
            int result = new AbstractLayer.AttendanceDB().UnlockAttendanceMemoDetail(adminLoginSession.AdminEmployeeUserId, obj.memonumber, out outError, sRemark);
            response.returncode = outError;
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}