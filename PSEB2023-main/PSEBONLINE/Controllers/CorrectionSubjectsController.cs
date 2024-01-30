using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using PSEBONLINE.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using PSEBONLINE.Filters;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon;
using System.Configuration;
using DocumentFormat.OpenXml.Spreadsheet;

namespace PSEBONLINE.Controllers
{
    public class CorrectionSubjectsController : Controller
    {
        private const string BUCKET_NAME = "psebdata";

        AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB(); //calling class DBClass
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        AbstractLayer.OpenDB openDB = new AbstractLayer.OpenDB();
        // GET: CorrectionSubjects
        public string stdPic, stdSign;
        public ActionResult Index()
        {
            return View();
        }

        #region image correction
        public ActionResult ImageCorrectionPerforma(RegistrationModels rm)
        {
            try
            {
                //ViewBag.SelectedItem
                AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                //RegistrationModels rm = new RegistrationModels();
                string schlid = "";
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");

                }
                else
                {
                    schlid = Session["SCHL"].ToString();
                    DataSet seleLastCan = objDB.PendingPhotoSignCorrection(schlid);
                    if (seleLastCan.Tables[0].Rows.Count > 0)
                    {

                        @ViewBag.message = "1";
                        rm.StoreAllData = seleLastCan;
                        ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                    }
                    else
                    {
                        @ViewBag.message = "Record Not Found";
                    }
                    //ViewBag.MediumNew = objCommon.GetMediumAll();                                  
                }
                DataSet result = objDB.schooltypesCorrection(schlid, "I"); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {
                    ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                    ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
                    ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                    ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
                    ViewBag.N3M1threclock = result.Tables[1].Rows[0]["Nth"].ToString();
                    ViewBag.E1T1threclock = result.Tables[1].Rows[0]["Eth"].ToString();

                    DateTime sDateN = Convert.ToDateTime(result.Tables[6].Rows[0]["sDate"]);
                    DateTime eDateN = Convert.ToDateTime(result.Tables[6].Rows[0]["eDate"]);

                    DateTime sDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["sDate"]);
                    DateTime eDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["eDate"]);

                    DateTime sDateE = Convert.ToDateTime(result.Tables[6].Rows[2]["sDate"]);
                    DateTime eDateE = Convert.ToDateTime(result.Tables[6].Rows[2]["eDate"]);

                    DateTime sDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["sDate"]);
                    DateTime eDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["eDate"]);

                    DateTime sDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["sDate"]);
                    DateTime eDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["eDate"]);

                    DateTime sDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["sDate"]);
                    DateTime eDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["eDate"]);

                    DateTime dtTodate = Convert.ToDateTime(DateTime.Today);

                    List<SelectListItem> itemsch = new List<SelectListItem>();
                    if (ViewBag.N3M1threclock == "1" && dtTodate <= eDateN)
                    {
                        itemsch.Add(new SelectListItem { Text = "9th Class", Value = "5" });
                    }
                    if (ViewBag.Matric == "1" && dtTodate <= eDateM)
                    {
                        itemsch.Add(new SelectListItem { Text = "Matriculation Regular", Value = "1" });
                    }
                    if (ViewBag.OMatric == "1" && dtTodate <= eDateMO)
                    {
                        itemsch.Add(new SelectListItem { Text = "Matriculation Open", Value = "2" });
                    }
                    if (ViewBag.E1T1threclock == "1" && dtTodate <= eDateE)
                    {
                        itemsch.Add(new SelectListItem { Text = "11th Class", Value = "6" });
                    }
                    if (ViewBag.Senior == "1" && dtTodate <= eDateT)
                    {
                        itemsch.Add(new SelectListItem { Text = "Sr.Secondary Regular", Value = "3" });
                    }
                    if (ViewBag.OSenior == "1" && dtTodate <= eDateTO)
                    {
                        itemsch.Add(new SelectListItem { Text = "Sr.Secondary Open", Value = "4" });
                    }

                    if (ViewBag.Matric != "1" && ViewBag.OMatric != "1" && ViewBag.Senior != "1" && ViewBag.OSenior != "1" && ViewBag.N3M1threclock != "1" && ViewBag.E1T1threclock != "1")
                    {
                        itemsch.Add(new SelectListItem { Text = "", Value = "" });
                    }
                    ViewBag.MySch = itemsch.ToList();

                }

                return View(rm);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }
        }
        [HttpPost]
        public ActionResult ImageCorrectionPerforma(RegistrationModels rm, FormCollection frm, string cmd)
        {
            string sid = Convert.ToString(rm.Std_id);
            string formname = null;
            if (rm.SelList == "1")
            {
                formname = "M";
            }
            if (rm.SelList == "2")
            {
                formname = "MO";
            }
            if (rm.SelList == "3")
            {
                formname = "T";
            }
            if (rm.SelList == "4")
            {
                formname = "TO";
            }
            if (rm.SelList == "5")
            {
                formname = "Nth";
            }
            if (rm.SelList == "6")
            {
                formname = "Eth";
            }
            ViewBag.SelectedItem = frm["SelList"];

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            string schlcode = Session["SCHL"].ToString();
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            DataSet result = objDB.schooltypesCorrection(schlcode,"I"); // passing Value to DBClass from model
            if (result.Tables[1].Rows.Count > 0)
            {
                ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
                ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
                ViewBag.N3M1threclock = result.Tables[1].Rows[0]["Nth"].ToString();
                ViewBag.E1T1threclock = result.Tables[1].Rows[0]["Eth"].ToString();

                DateTime sDateN = Convert.ToDateTime(result.Tables[6].Rows[0]["sDate"]);
                DateTime eDateN = Convert.ToDateTime(result.Tables[6].Rows[0]["eDate"]);

                DateTime sDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["sDate"]);
                DateTime eDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["eDate"]);

                DateTime sDateE = Convert.ToDateTime(result.Tables[6].Rows[2]["sDate"]);
                DateTime eDateE = Convert.ToDateTime(result.Tables[6].Rows[2]["eDate"]);

                DateTime sDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["sDate"]);
                DateTime eDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["eDate"]);

                DateTime sDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["sDate"]);
                DateTime eDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["eDate"]);

                DateTime sDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["sDate"]);
                DateTime eDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["eDate"]);

                DateTime dtTodate = Convert.ToDateTime(DateTime.Today);

                List<SelectListItem> itemsch = new List<SelectListItem>();
                if (ViewBag.N3M1threclock == "1" && dtTodate <= eDateN)
                {
                    itemsch.Add(new SelectListItem { Text = "9th Class", Value = "5" });
                }
                if (ViewBag.Matric == "1" && dtTodate <= eDateM)
                {
                    itemsch.Add(new SelectListItem { Text = "Matriculation Regular", Value = "1" });
                }
                if (ViewBag.OMatric == "1" && dtTodate <= eDateMO)
                {
                    itemsch.Add(new SelectListItem { Text = "Matriculation Open", Value = "2" });
                }
                if (ViewBag.E1T1threclock == "1" && dtTodate <= eDateE)
                {
                    itemsch.Add(new SelectListItem { Text = "11th Class", Value = "6" });
                }
                if (ViewBag.Senior == "1" && dtTodate <= eDateT)
                {
                    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Regular", Value = "3" });
                }
                if (ViewBag.OSenior == "1" && dtTodate <= eDateTO)
                {
                    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Open", Value = "4" });
                }


                if (ViewBag.Matric != "1" && ViewBag.OMatric != "1" && ViewBag.Senior != "1" && ViewBag.OSenior != "1" && ViewBag.N3M1threclock != "1" && ViewBag.E1T1threclock != "1")
                {
                    itemsch.Add(new SelectListItem { Text = "", Value = "" });
                }
                ViewBag.MySch = itemsch.ToList();                
            }
            #region View All Correction Pending Record
            if (cmd == "View All Correction Pending Record")
            {


                DataSet seleLastCan = objDB.PendingPhotoSignCorrection(schlcode);
                if (seleLastCan.Tables[0].Rows.Count > 0)
                {

                    @ViewBag.message = "1";
                    rm.StoreAllData = seleLastCan;
                    ViewBag.TotalViewAllCount = seleLastCan.Tables[0].Rows.Count;
                    ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                }
                else
                {
                    @ViewBag.message = "Record Not Found";
                }


                return View(rm);
            }
            #endregion View All Correction Pending Record

            #region View All Record Images
            else if (cmd == "View All Correction Record")
            {
                DataSet seleLastCan = objDB.ViewAllPhotoSignCorrection(schlcode);
                if (seleLastCan.Tables[0].Rows.Count > 0)
                {
                    @ViewBag.message = "1";
                    rm.StoreAllData = seleLastCan;
                    ViewBag.TotalViewAllCount = seleLastCan.Tables[0].Rows.Count;
                    ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                }
                else
                {
                    @ViewBag.message = "Record Not Found";
                }


                return View(rm);
            }
            #endregion View All Record Images

            #region Add Record Region Begin
            else if (cmd == "Add Record")
            {
                //string stdPic, stdSign;
                rm.Std_id = Convert.ToInt32(frm["Std_id"]);
                rm.Class = frm["SelList"];
                rm.Correctiontype = frm["SelListField"];
                string filepathtosave = "";
                DataSet ds = objDB.PhotoSignSearchCorrectionStudentDetails(formname, schlcode, sid);

                if (rm.Correctiontype == "Photo")
                {
                    if (rm.std_Photo != null)
                    {
                        stdPic = Path.GetFileName(rm.std_Photo.FileName);
                    }
                    //var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/ImageCorrection/New/Photo"), rm.Std_id + "P" + ".jpg");
                    var path = Path.Combine(Server.MapPath("allfiles/Upload2023/ImageCorrection/New/Photo"), rm.Std_id + "P" + ".jpg");

                    ////var pathOld = @"\\10.10.10.113\Nucleus\live.psebonline.in\www\upload\Upload2017\" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                    var pathOld = "https://registration2022.pseb.ac.in/Upload/Upload2023/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                    //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/ImageCorrection/New/Photo"));
                    //string FilepathExistOld = Path.Combine(Server.MapPath("~/Upload/Upload2023/ImageCorrection/Old/Photo"));
                    //if (!Directory.Exists(FilepathExist))
                    //{
                    //    Directory.CreateDirectory(FilepathExist);
                    //}
                    //if (!Directory.Exists(FilepathExistOld))
                    //{
                    //    Directory.CreateDirectory(FilepathExistOld);
                    //}

                    //rm.std_Photo.SaveAs(path);
                    string Orgfile = rm.Std_id + "P" + ".jpg";
                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = rm.file.InputStream,
                                Key = string.Format("allfiles/Upload2023/ImageCorrection/New/Photo/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }

                    //filepathtosave = "../Upload/Upload2023/ImageCorrection/New/Photo/" + rm.Std_id + "P" + ".jpg";
                    filepathtosave = "allfiles/Upload2023/ImageCorrection/New/Photo" + rm.Std_id + "P" + ".jpg";
                    ViewBag.ImageURL = filepathtosave;
                    //string PhotoName = "/Upload/Upload2023/ImageCorrection/New/Photo" + "/" + rm.Std_id + "P" + ".jpg";
                    string PhotoName = "allfiles/Upload2023/ImageCorrection/New/Photo" + "/" + rm.Std_id + "P" + ".jpg";
                    rm.oldVal = frm["imgPhotoOld"];
                    rm.newVal = PhotoName;

                    //System.IO.File.Copy(pathOld, FilepathExistOld);
                }
                else if (rm.Correctiontype == "Sign")
                {
                    if (rm.std_Sign != null)
                    {
                        stdPic = Path.GetFileName(rm.std_Sign.FileName);
                    }
                    //var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/ImageCorrection/New/Sign"), rm.Std_id + "S" + ".jpg");
                    ////var pathOld = @"\\10.10.10.113\Nucleus\live.psebonline.in\www\upload\Upload2017\" + ds.Tables[0].Rows[0]["std_Sign"].ToString();
                    //var pathOld = "https://registration2022.pseb.ac.in/Upload/Upload2023/" + ds.Tables[0].Rows[0]["std_Sign"].ToString();
                    //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/ImageCorrection/New/Sign"));
                    //string FilepathExistOld = Path.Combine(Server.MapPath("~/Upload/Upload2023/ImageCorrection/Old/Sign"));
                    //if (!Directory.Exists(FilepathExist))
                    //{
                    //    Directory.CreateDirectory(FilepathExist);
                    //}
                    //if (!Directory.Exists(FilepathExistOld))
                    //{
                    //    Directory.CreateDirectory(FilepathExistOld);
                    //}

                    //rm.std_Sign.SaveAs(path);

                    string Orgfile = rm.Std_id + "S" + ".jpg";
                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = rm.std_Sign.InputStream,
                                Key = string.Format("allfiles/Upload2023/ImageCorrection/New/Sign/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }

                    //filepathtosave = "../Upload/Upload2023/ImageCorrection/New/Sign/" + rm.Std_id + "S" + ".jpg";
                    filepathtosave = "allfiles/Upload2023/ImageCorrection/New/Sign/" + rm.Std_id + "S" + ".jpg";
                    ViewBag.ImageURL = filepathtosave;
                    //string SignName = "/Upload/Upload2023/ImageCorrection/New/Sign" + "/" + rm.Std_id + "S" + ".jpg";
                    string SignName = "allfiles/Upload2023/ImageCorrection/New/Sign/" + "/" + rm.Std_id + "S" + ".jpg";
                    rm.oldVal = frm["imgSignOld"];
                    rm.newVal = SignName;
                    rm.oldVal = frm["imgSignOld"];
                    rm.newVal = SignName;

                    //System.IO.File.Copy(pathOld, FilepathExistOld);
                }
                else
                {
                    rm.newVal = "NULL";
                }

                rm.Remark = frm["Remark"];
               // ViewBag.SelectedItem = frm["SelList"];
                ViewBag.Searchstring = frm["Std_id"];
                rm.SCHL = Session["SCHL"].ToString();

                string result1 = objDB.InsertPhotoSignCorrectionAdd(rm, frm,"","");
                if (result1 == "-1")
                {
                    ViewData["Status"] = "0";
                }
                else
                {
                    ViewData["Status"] = "1";
                }

                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "Photo", Value = "Photo" });
                items.Add(new SelectListItem { Text = "Sign", Value = "Sign" });
                ViewBag.MySchField = new SelectList(items, "Value", "Text");
                DataSet seleLastCanPending = objDB.PendingPhotoSignCorrection(schlcode);
                if (seleLastCanPending.Tables[0].Rows.Count > 0)
                {
                    @ViewBag.message = "1";
                    rm.StoreAllData = seleLastCanPending;
                    ViewBag.TotalViewAllCount = seleLastCanPending.Tables[0].Rows.Count;
                    ViewBag.TotalCount = seleLastCanPending.Tables[0].Rows.Count;
                }
                return View(rm);
            }
            #endregion Add Record

            #region Final Submit
            else if (cmd == "Final Submit Correction")
            {
                if (Session["SCHL"] != null)
                {

                    rm.SCHL = Session["SCHL"].ToString();
                    rm.Correctiontype = "04";
                    string resultFS = objDB.FinalSubmitImageCorrection(rm); // passing Value to DBClass from model
                    if (Convert.ToInt16(resultFS) > 0)
                    {
                        ViewData["resultFS"] = resultFS;
                        //return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects");
                        return RedirectToAction("SchoolCorrectionFinalPrintLst", "RegistrationPortal");
                    }
                    else
                    {
                        ViewData["resultFS"] = "";
                        return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Login");
                }
            }
            #endregion Final Submit

            #region Else Region
            else
            {
                try
                {
                    DataSet seleLastCanPending = objDB.PendingPhotoSignCorrection(schlcode);
                    if (seleLastCanPending.Tables[0].Rows.Count > 0)
                    {
                        @ViewBag.message = "1";
                        rm.StoreAllData = seleLastCanPending;
                        ViewBag.TotalViewAllCount = seleLastCanPending.Tables[0].Rows.Count;
                        ViewBag.TotalCount = seleLastCanPending.Tables[0].Rows.Count;
                    }

                    DataSet seleLastCan = objDB.PhotoSignSearchCorrectionStudentDetails(formname, schlcode, sid);
                    if (seleLastCan.Tables[0].Rows.Count > 0)
                    {
                        @ViewBag.TotalCountSearch = seleLastCan.Tables[0].Rows.Count;
                        List<SelectListItem> items = new List<SelectListItem>();
                        items.Add(new SelectListItem { Text = "Photo", Value = "Photo" });
                        items.Add(new SelectListItem { Text = "Sign", Value = "Sign" });

                        ViewBag.MySchField = new SelectList(items, "Value", "Text");

                        @ViewBag.message = "1";

                        @ViewBag.stdid = seleLastCan.Tables[0].Rows[0]["std_id"].ToString();
                        @ViewBag.Oroll = seleLastCan.Tables[0].Rows[0]["Board_Roll_Num"].ToString();
                        @ViewBag.Regno = seleLastCan.Tables[0].Rows[0]["Registration_num"].ToString();
                        @ViewBag.category = seleLastCan.Tables[0].Rows[0]["category"].ToString();
                        @ViewBag.session = seleLastCan.Tables[0].Rows[0]["Year"].ToString() + "-" + seleLastCan.Tables[0].Rows[0]["Month"].ToString();
                        @ViewBag.canName = seleLastCan.Tables[0].Rows[0]["Candi_Name"].ToString();
                        @ViewBag.FName = seleLastCan.Tables[0].Rows[0]["Father_Name"].ToString();
                        @ViewBag.Mname = seleLastCan.Tables[0].Rows[0]["Mother_Name"].ToString();
                        @ViewBag.lot = seleLastCan.Tables[0].Rows[0]["lot"].ToString();
                        @ViewBag.DOB = seleLastCan.Tables[0].Rows[0]["DOB"].ToString();
                        @ViewBag.Frm = formname;
                        @ViewBag.Subjlist = seleLastCan.Tables[0].Rows[0]["SubjectList"].ToString();
                        if (formname == "MO" || formname == "TO")
                        {
                            @ViewBag.PhotoOld = seleLastCan.Tables[0].Rows[0]["Std_photo"].ToString();
                            @ViewBag.SignOld = seleLastCan.Tables[0].Rows[0]["Std_Sign"].ToString();
                        }
                        else
                        {
                            @ViewBag.PhotoOld = seleLastCan.Tables[0].Rows[0]["Std_photo"].ToString();
                            @ViewBag.SignOld = seleLastCan.Tables[0].Rows[0]["Std_Sign"].ToString();
                        }

                        @ViewBag.CandPhotoFullPath = seleLastCan.Tables[0].Rows[0]["CandPhotoFullPath"].ToString();
                        @ViewBag.CandSignFullPath = seleLastCan.Tables[0].Rows[0]["CandSignFullPath"].ToString();

                    }
                    else
                    {
                        @ViewBag.message = "Record Not Found";
                        return View(rm);
                    }

                    return View(rm);

                }
                catch (Exception ex)
                {
                    return View(rm);
                }

            }
            #endregion Else Region

        }

        public ActionResult CorrDeletePhotoSignData(string id)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB(); //calling class DBClass
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }

            if (id == null)
            {
                return RedirectToAction("ImageCorrectionPerforma", "CorrectionSubjects");

            }
            else
            {
                string result = objDB.DeletePhotoSignData(id);
                if (result == "Deleted")
                {
                    @ViewBag.result = "1";

                }

            }
            return RedirectToAction("ImageCorrectionPerforma", "CorrectionSubjects");
        }
        #endregion image correction

        //-------------------------Matric Subject Correction---------------------------/
        #region matric subject correctionSRSubjectCorrectionPerforma
        public ActionResult SubjectCorrectionPerforma(RegistrationModels rm)
        {
            ViewBag.btnshow = "0";
            try
            {
                List<SelectListItem> SubWEL = new List<SelectListItem>();
                SubWEL.Add(new SelectListItem { Text = "WELCOME LIFE", Value = "210" });
                ViewBag.SubWEL = new SelectList(SubWEL, "Value", "Text");
                //ViewBag.SelectedItem
                AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                //RegistrationModels rm = new RegistrationModels();
                string schlid = "";
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");

                }
                else
                {
                    //@ViewBag.DA = objCommon.GetDA();
                    //@ViewBag.DAb = objCommon.GetDA();
                    //-----------------------Matric Subjects Start----------------------
                    DataSet ds2 = objDB.ElectiveSubjects();

                    ViewBag.SubS9 = ds2.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                    List<SelectListItem> items = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.SubS9.Rows)
                    {
                        //  items.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        items.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                    }

                    ViewBag.SubS10m = ViewBag.SubS9m = ViewBag.SubS9 = new SelectList(items, "Value", "Text");


                    ViewBag.S10 = new SelectList(items, "Value", "Text");

                    DataSet ds1 = objDB.ElectiveSubjects_Blind();
                    ViewBag.SubSb9 = ds1.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                    List<SelectListItem> bitems = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.SubSb9.Rows)
                    {
                        bitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                    }

                    ViewBag.bs2 = new SelectList(bitems, "Value", "Text");
                    ViewBag.bs3 = new SelectList(bitems, "Value", "Text");
                    ViewBag.bs4 = new SelectList(bitems, "Value", "Text");
                    ViewBag.bs5 = new SelectList(bitems, "Value", "Text");
                    ViewBag.bs6 = new SelectList(bitems, "Value", "Text");
                    ViewBag.bs7 = new SelectList(bitems, "Value", "Text");
                    ViewBag.bs8 = new SelectList(bitems, "Value", "Text");
                    ViewBag.bs9 = new SelectList(bitems, "Value", "Text");



                    //-----------------------------------------------------Nsqf -------------------------
                    string ses = Session["Session"].ToString();
                    string schlcode = Session["SCHL"].ToString();
                    DataSet dsnsqf = objDB.CHkNSQF(schlcode, ses);
                    if (dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "True" || dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "1")
                    {
                        ViewData["NSQFSCHOOL"] = "1";
                    }
                    else
                    {
                        ViewData["NSQFSCHOOL"] = "0";
                    }
                    string nsqfsub = null;
                    DataSet nsresult = objDB.SelectMatricNsqfSub(nsqfsub); // passing Value to DBClass from model
                    ViewBag.nsfq = nsresult.Tables[0];
                    List<SelectListItem> nsfqList = new List<SelectListItem>();
                    //nsfqList.Add(new SelectListItem { Text = "NSFQ Subjects", Value = "0" });
                    nsfqList.Add(new SelectListItem { Text = "NO", Value = "NO" });
                    foreach (System.Data.DataRow dr in ViewBag.nsfq.Rows)
                    {
                        nsfqList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["SUB"].ToString() });
                    }
                    ViewBag.nsfqList = new SelectList(nsfqList, "Value", "Text");
                    //-----------------------------------------------------End Nsqf -------------------------

                    List<SelectListItem> itemsub6 = new List<SelectListItem>();
                    itemsub6.Add(new SelectListItem { Text = "SOCIAL STUDIES", Value = "06" });
                    ViewBag.SubS6 = new SelectList(itemsub6, "Value", "Text");

                    List<SelectListItem> itemsub10 = new List<SelectListItem>();
                    itemsub10.Add(new SelectListItem { Text = "HINDI", Value = "03" });
                    itemsub10.Add(new SelectListItem { Text = "SOCIAL STUDIES", Value = "06" });
                    ViewBag.SubS10 = new SelectList(itemsub10, "Value", "Text");

                    ViewBag.BM = ViewBag.BM2m = ViewBag.BM3m = ViewBag.BM4m = ViewBag.BM5m = ViewBag.BM6m = ViewBag.BM7m = ViewBag.BM8m = ViewBag.BM9m = objCommon.GetMediumAll();
                    //----------------------Matric Subjects End---------------------------


                    schlid = Session["SCHL"].ToString();

                    DataSet seleLastCan = objDB.PendingCorrectionSubjects(schlid, "2");
                    if (seleLastCan.Tables[0].Rows.Count > 0)
                    {

                        @ViewBag.message = "1";
                        rm.StoreAllData = seleLastCan;
                        ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                        //for (int i = 0; i < seleLastCan.Tables[0].Rows.Count;i++)
                        //{

                        //    if (seleLastCan.Tables[0].Rows[i]["DiffSub"].ToString() == null || seleLastCan.Tables[0].Rows[i]["DiffSub"].ToString() == "")
                        //    {
                        //        string candid = seleLastCan.Tables[0].Rows[i]["candid"].ToString();
                        //        string subNew = seleLastCan.Tables[0].Rows[i]["Newsubcode"].ToString();
                        //        string subOld = seleLastCan.Tables[0].Rows[i]["Oldsubcode"].ToString();
                        //        string DiffSub = string.Empty;

                        //        //modified
                        //        //Response.Write("modified");
                        //        if (subNew.Length == subOld.Length)
                        //        {
                        //            var list = subOld.Split(' ').Where(x => (!subNew.Split(' ').Contains(x))).ToList();
                        //            int count = list.Count;
                        //            foreach (var item in list)
                        //            {
                        //                DiffSub = item + " Modified";
                        //            }
                        //        }
                        //        ////Removed
                        //        if (subNew.Length > subOld.Length)
                        //        {
                        //            //Response.Write("Added...");
                        //            var list1 = subNew.Split(' ').Where(x => (!subOld.Split(' ').Contains(x))).ToList();
                        //            int count1 = list1.Count;
                        //            foreach (var item in list1)
                        //            {
                        //                DiffSub = item + " Added";
                        //            }
                        //        }

                        //        string DiffSubCorr = objDB.InsertDiffSubjects(candid,DiffSub);

                        //    }
                        //}



                    }
                    else
                    {
                        @ViewBag.message = "Record Not Found";
                    }
                    ViewBag.MediumNew = objCommon.GetMediumAll();
                    //-------------New SubList---------------//
                    //DataSet NewSub = new DataSet();
                    //NewSub = objDB.NewElectiveSubjects();
                    //List<SelectListItem> NewSUBList = new List<SelectListItem>();
                    //foreach (System.Data.DataRow dr in NewSub.Tables[0].Rows)
                    //{
                    //    NewSUBList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });

                    //}
                    //ViewBag.SubNew = NewSUBList;
                    //ViewBag.MediumNew = objCommon.GetMediumAll();
                    //-----------End-------------------//                   
                }

                //var itemsch = new SelectList(new[]{new {ID="1",Name="Matriculation Regular"},new{ID="2",Name="Matriculation Open"},
                //new{ID="3",Name="Sr.Secondary Regular"},new{ID="4",Name="Sr.Secondary Open"},}, "ID", "Name", 1);
                //ViewBag.MySch = itemsch.ToList();

                DataSet result = objDB.schooltypesCorrection(schlid, "S"); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {
                    ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                    ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
                    ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                    ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();

                    DateTime sDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["sDate"]);
                    DateTime eDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["eDate"]);

                    DateTime sDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["sDate"]);
                    DateTime eDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["eDate"]);

                    DateTime sDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["sDate"]);
                    DateTime eDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["eDate"]);

                    DateTime sDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["sDate"]);
                    DateTime eDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["eDate"]);

                    DateTime dtTodate = Convert.ToDateTime(DateTime.Today);

                    List<SelectListItem> itemsch = new List<SelectListItem>();
                    if (ViewBag.Matric == "1" && dtTodate <= eDateM)
                    {
                        itemsch.Add(new SelectListItem { Text = "Matriculation Regular", Value = "1" });
                    }
                    //if (ViewBag.OMatric == "1" && sDateMO <= eDateMO)
                    //{
                    //    itemsch.Add(new SelectListItem { Text = "Matriculation Open", Value = "2" });
                    //}                   
                    //if (ViewBag.Senior == "1" && sDateT <= eDateT)
                    //{
                    //    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Regular", Value = "3" });
                    //}
                    //if (ViewBag.OSenior == "1" && sDateTO <= eDateTO)
                    //{
                    //    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Open", Value = "4" });
                    //}

                    if (ViewBag.Matric != "1" && ViewBag.OMatric != "1" && ViewBag.Senior != "1" && ViewBag.OSenior != "1")
                    {
                        itemsch.Add(new SelectListItem { Text = "", Value = "" });
                    }
                    ViewBag.MySch = itemsch.ToList();
                }

                if (ModelState.IsValid)
                {
                    return View(rm);
                }
                else
                { return View(rm); }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }
        }
        [HttpPost]
        public ActionResult SubjectCorrectionPerforma(RegistrationModels rm, FormCollection frm, string cmd)
        {
            ViewBag.btnshow = "1";
            List<SelectListItem> SubWEL = new List<SelectListItem>();
            SubWEL.Add(new SelectListItem { Text = "WELCOME LIFE", Value = "210" });
            ViewBag.SubWEL = new SelectList(SubWEL, "Value", "Text");

            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            string sid = Convert.ToString(rm.Std_id);
            string formname = null;
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");

            }
            if (rm.SelList == "1")
            {
                formname = "M";
            }
            if (rm.SelList == "2")
            {
                formname = "MO";
            }
            if (rm.SelList == "3")
            {
                formname = "T";
            }
            if (rm.SelList == "4")
            {
                formname = "TO";
            }

            DataSet ds_chk = objDB.SearchStudentGetByData_SubjectCORR(sid, formname);
            if (ds_chk == null || ds_chk.Tables[0].Rows.Count == 0)
            {
                return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects");
                //return View(rm);
            }

            else if (ds_chk != null && ds_chk.Tables[2].Rows.Count == 1)
            {
                TempData["resultUpdate"] = "5"; // CorrectionPerforma Already exist is is not status is null.
            }

            //@ViewBag.DA = objCommon.GetDA();
            //@ViewBag.DAb = objCommon.GetDA();
            //-----------------------Matric Subjects Start----------------------
            #region M Subjects
            DataSet ds2 = objDB.ElectiveSubjects();

            ViewBag.SubS9 = ds2.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.SubS9.Rows)
            {
                //  items.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                items.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }

            ViewBag.SubS10m = ViewBag.SubS9m = ViewBag.SubS9 = new SelectList(items, "Value", "Text");


            ViewBag.S10 = new SelectList(items, "Value", "Text");

            //DataSet ds1 = objDB.ElectiveSubjects_Blind();
            //ViewBag.SubSb9 = ds1.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            //List<SelectListItem> bitems = new List<SelectListItem>();
            //foreach (System.Data.DataRow dr in ViewBag.SubSb9.Rows)
            //{
            //    bitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            //}

            //ViewBag.bs2 = new SelectList(bitems, "Value", "Text");
            //ViewBag.bs3 = new SelectList(bitems, "Value", "Text");
            //ViewBag.bs4 = new SelectList(bitems, "Value", "Text");
            //ViewBag.bs5 = new SelectList(bitems, "Value", "Text");
            //ViewBag.bs6 = new SelectList(bitems, "Value", "Text");
            //ViewBag.bs7 = new SelectList(bitems, "Value", "Text");
            //ViewBag.bs8 = new SelectList(bitems, "Value", "Text");
            //ViewBag.bs9 = new SelectList(bitems, "Value", "Text");

            #region Matric_ElectiveSubjects_Blind_NEW 

            DataSet ds1 = objDB.Matric_ElectiveSubjects_Blind_NEW();
            //ViewBag.SubSb9 = ds1.Tables[0];

            // Blind SUB -2
            List<SelectListItem> bitems2 = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ds1.Tables[0].Rows)
            {
                bitems2.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }
            // Blind SUB -3
            List<SelectListItem> bitems3 = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ds1.Tables[1].Rows)
            {
                bitems3.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }
            // Blind SUB -4
            List<SelectListItem> bitems4 = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ds1.Tables[2].Rows)
            {
                bitems4.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }

            // Blind SUB -5
            List<SelectListItem> bitems5 = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ds1.Tables[3].Rows)
            {
                bitems5.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }

            // Blind SUB -6
            List<SelectListItem> bitems6 = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ds1.Tables[4].Rows)
            {
                bitems6.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }

            // Blind SUB -7
            List<SelectListItem> bitems7 = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ds1.Tables[5].Rows)
            {
                bitems7.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }
            // Blind SUB - 8
            List<SelectListItem> bitems8 = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ds1.Tables[6].Rows)
            {
                bitems8.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }
            // Blind SUB - 9
            List<SelectListItem> bitems9 = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ds1.Tables[7].Rows)
            {
                bitems9.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }


            ViewBag.bs2 = new SelectList(bitems2, "Value", "Text");
            ViewBag.bs3 = new SelectList(bitems3, "Value", "Text");
            ViewBag.bs4 = new SelectList(bitems4, "Value", "Text");
            ViewBag.bs5 = new SelectList(bitems5, "Value", "Text");
            ViewBag.bs6 = new SelectList(bitems6, "Value", "Text");
            ViewBag.bs7 = new SelectList(bitems7, "Value", "Text");
            ViewBag.bs8 = new SelectList(bitems8, "Value", "Text");
            ViewBag.bs9 = new SelectList(bitems9, "Value", "Text");


            #endregion  Matric ElectiveSubjects Blind NEW 



            string nsqfsub = null;
            DataSet nsresult = objDB.SelectMatricNsqfSub(nsqfsub); // passing Value to DBClass from model
            ViewBag.nsfq = nsresult.Tables[0];
            List<SelectListItem> nsfqList = new List<SelectListItem>();
            //nsfqList.Add(new SelectListItem { Text = "NSFQ Subjects", Value = "0" });
            nsfqList.Add(new SelectListItem { Text = "NO", Value = "NO" });
            foreach (System.Data.DataRow dr in ViewBag.nsfq.Rows)
            {

                nsfqList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }
            ViewBag.nsfqList = new SelectList(nsfqList, "Value", "Text");
            ViewBag.nsqfcatg = new SelectList(nsfqList, "Value", "Text");


            List<SelectListItem> itemsub6 = new List<SelectListItem>();
            itemsub6.Add(new SelectListItem { Text = "SOCIAL STUDIES", Value = "06" });
            ViewBag.SubS6 = new SelectList(itemsub6, "Value", "Text");

            List<SelectListItem> itemsub10 = new List<SelectListItem>();
            itemsub10.Add(new SelectListItem { Text = "HINDI", Value = "03" });
            itemsub10.Add(new SelectListItem { Text = "SOCIAL STUDIES", Value = "06" });
            ViewBag.SubS10 = new SelectList(itemsub10, "Value", "Text");

            ViewBag.BM = ViewBag.BM1m = ViewBag.BM3m = ViewBag.BM4m = ViewBag.BM5m = ViewBag.BM6m = ViewBag.BM7m = ViewBag.BM8m = ViewBag.BM9m = objCommon.GetMediumAll();
            //RegistrationModels rm = new RegistrationModels();

            if (sid != null)
            {
                DataSet ds = objDB.SearchStudentGetByData_SubjectCORR(sid, formname);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return RedirectToAction("Logout", "Login");
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 1; i < ds.Tables[1].Rows.Count; i++)
                    {
                        if (ds.Tables[1].Rows[i]["SUB"].ToString() == "72")
                        {
                            //items.Remove(items[i]);
                            ds.Tables[1].Rows.RemoveAt(i);
                        }

                    }
                    for (int i = 1; i < ds.Tables[1].Rows.Count; i++)
                    {
                        if (ds.Tables[1].Rows[i]["SUB"].ToString() == "73")
                        {
                            //items.Remove(items[i]);
                            ds.Tables[1].Rows.RemoveAt(i);
                        }

                    }

                    //-----------------------------------------------------Nsqf -------------------------
                    rm.DA = ds.Tables[0].Rows[0]["Differently_Abled"].ToString();
                    ViewBag.DAb = objCommon.GetDA();
                    ViewBag.DAItem = rm.DA;
                    rm.PreNSQF = ds.Tables[0].Rows[0]["PRE_NSQF_SUB"].ToString();
                    rm.NSQF = ds.Tables[0].Rows[0]["nsqf_flag"].ToString();
                    rm.NsqfsubS6 = ds.Tables[0].Rows[0]["NSQF_SUB"].ToString();

                    string ses = Session["Session"].ToString();
                    string Mschlcode = Session["SCHL"].ToString();
                    DataSet dsnsqf = objDB.CHkNSQF(Mschlcode, ses);


                    if (dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "True" || dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "1")
                    {
                        ViewData["NSQFSCHOOL"] = "1";
                        ViewBag.NSQFSTUDENT = "1";
                    }
                    else
                    {
                        ViewData["NSQFSCHOOL"] = "0";
                        ViewBag.NSQFSTUDENT = "0";
                    }


                    //-----------------------------For NSQF SUBJECTS----------------

                    DataSet isCHkNSQF = objDB.CHkNSQFStudents(sid);

                    if (isCHkNSQF.Tables[0].Rows[0]["NSQF_flag"].ToString() == "True")
                    {

                        ViewBag.SubS9 = ds2.Tables[0];
                        // for dislaying message after saving storing output.
                        // List<SelectListItem> items21 = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.SubS9.Rows)
                        {
                            items.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }
                        ViewBag.SubS10m = ViewBag.SubS9m = ViewBag.SubS9 = new SelectList(items, "Value", "Text");
                        ViewBag.S10 = new SelectList(items, "Value", "Text");
                        //-----------------------

                        ViewData["NSQFSTUDENT"] = "1";
                        rm.NsqfsubS6 = ds.Tables[0].Rows[0]["NSQF_SUB"].ToString();
                        DataSet nsTextresult = objDB.GetNSQFVIEWSUBJECTMATRICSUBJECT(rm.NsqfsubS6, rm.PreNSQF);
                        List<SelectListItem> nssub6 = new List<SelectListItem>();
                        if (rm.PreNSQF == "NO")
                        {
                            nssub6.Add(new SelectListItem { Text = "NO", Value = "NO" });
                            ViewBag.nsqfcatg = nssub6;
                        }
                        else
                        {
                            if (nsTextresult.Tables[0].Rows.Count > 0)
                            {
                                nssub6.Add(new SelectListItem { Text = "NO", Value = "NO" });
                                nssub6.Add(new SelectListItem { Text = nsTextresult.Tables[0].Rows[0]["Name_ENG"].ToString(), Value = rm.NsqfsubS6 });
                                ViewBag.nsqfcatg = nssub6;
                            }
                            else
                            {

                                nssub6.Add(new SelectListItem { Text = "NO", Value = "NO" });
                                ViewBag.nsqfcatg = nssub6;
                            }

                        }

                    }
                    else
                    {
                        ViewBag.SubS9 = ds2.Tables[0];
                        // for dislaying message after saving storing output.
                        //  List<SelectListItem> items = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.SubS9.Rows)
                        {
                            if (@dr["TYPE"].ToString() == "GRADING SUBJECT")
                            {
                                items.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }

                        }
                        ViewBag.SubS10m = ViewBag.SubS9m = ViewBag.SubS9 = new SelectList(items, "Value", "Text");
                        ViewBag.S10 = new SelectList(items, "Value", "Text");
                        //--------------
                        List<SelectListItem> nssub6 = new List<SelectListItem>();
                        nssub6.Add(new SelectListItem { Text = "NO", Value = "NO" });
                        ViewBag.nsqfcatg = nssub6;
                    }


                    //--------------------------------End NSQF SUBJECTS-------------

                    //-----------------------------For NSQF SUBJECTS--------------

                    //------------------------------Fill Subjects----------------//


                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                        {
                            if (rm.DA == "N.A.")
                            {
                                if (i == 0)
                                {
                                    rm.subS1 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.subm1 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 1)
                                {
                                    rm.subS2 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.subM2 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 2)
                                {
                                    rm.subS3 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.subm3 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 3)
                                {
                                    rm.subS4 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.subM4 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 4)
                                {
                                    rm.subS5 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.subM5 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 5)
                                {
                                    rm.subS6 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.subM6 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                    rm.NsqfsubS6Upd = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    //rm.NsqfsubS6 = ds.Tables[1].Rows[5]["SUB"].ToString();
                                    rm.NsqfsubS6 = ds.Tables[0].Rows[0]["NSQF_SUB"].ToString();
                                }
                                else if (i == 6)
                                {
                                    rm.subS7 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.subM7 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 7)
                                {
                                    rm.subS8 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.subM8 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 8 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.s9 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.s9);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.m9 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[i]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.SubS9m = iMEdiumList;
                                }
                                else if (i == 9 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.s10 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.ns10 = rm.s10;
                                    //  rm.m10 =  itemMediumE.Where(s => s.Text == ds.Tables[1].Rows[9]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.s10);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.m10 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[i]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.SubS10m = rm.m10;//iMEdiumList;

                                }
                                else if (i == 10 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.s11 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    //rm.m11 =  itemMediumE.Where(s => s.Text == ds.Tables[1].Rows[10]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                }
                                else if (i == 11)
                                {
                                    rm.s12 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    // rm.m12 =  itemMediumE.Where(s => s.Text == ds.Tables[1].Rows[11]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                }


                            }
                            else
                            {
                                if (i == 0 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS1 = ds.Tables[1].Rows[0]["SUB"].ToString();
                                    rm.bm1 = ds.Tables[1].Rows[0]["MEDIUM"].ToString();
                                }
                                else if (i == 1 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS2 = ds.Tables[1].Rows[1]["SUB"].ToString();
                                    // rm.bM1 = itemMediumBlindE.Where(s => s.Text == ds.Tables[1].Rows[1]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();

                                    DataSet SelectedMediumList = objDB.SelectS1(rm.subbS2);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.bm2 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[1]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.BM1m = iMEdiumList;
                                }
                                else if (i == 2 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS3 = ds.Tables[1].Rows[2]["SUB"].ToString();
                                    // rm.bm3 = itemMediumBlindE.Where(s => s.Text == ds.Tables[1].Rows[2]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.subbS3);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.bm3 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[2]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.BM3m = iMEdiumList;
                                }
                                else if (i == 3 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS4 = ds.Tables[1].Rows[3]["SUB"].ToString();
                                    // rm.bm4 = itemMediumBlindE.Where(s => s.Text == ds.Tables[1].Rows[3]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.subbS4);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.bm4 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[3]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.BM4m = iMEdiumList;
                                }
                                else if (i == 4 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS5 = ds.Tables[1].Rows[4]["SUB"].ToString();
                                    // rm.bm5 = itemMediumBlindE.Where(s => s.Text == ds.Tables[1].Rows[4]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.subbS5);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.bm5 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[4]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.BM5m = iMEdiumList;
                                }
                                else if (i == 5 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS6 = ds.Tables[1].Rows[5]["SUB"].ToString();
                                    //  rm.bm6 = itemMediumBlindE.Where(s => s.Text == ds.Tables[1].Rows[5]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.subbS6);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.bm6 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[5]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.BM6m = iMEdiumList;
                                }

                                else if (i == 6 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS7 = ds.Tables[1].Rows[6]["SUB"].ToString();
                                    // rm.bm7 = itemMediumBlindE.Where(s => s.Text == ds.Tables[1].Rows[6]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.subbS7);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.bm7 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[6]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.BM7m = iMEdiumList;
                                }
                                else if (i == 7 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS8 = ds.Tables[1].Rows[7]["SUB"].ToString();
                                    // rm.bm8 = itemMediumBlindE.Where(s => s.Text == ds.Tables[1].Rows[7]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.subbS8);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.bm8 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[7]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.BM8m = iMEdiumList;
                                }
                                else if (i == 8 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS9 = ds.Tables[1].Rows[8]["SUB"].ToString();
                                    //rm.bm9 = itemMediumBlindE.Where(s => s.Text == ds.Tables[1].Rows[8]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.subbS9);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.bm9 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[8]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.BM9m = iMEdiumList;
                                }
                            }
                        }
                    }

                    //--------------------------End Subject Details--------------

                }

            }
            #endregion M Subjects
            //----------------------Matric Subjects End---------------------------

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            string schlcode = Session["SCHL"].ToString();
            //AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            DataSet result = objDB.schooltypesCorrection(schlcode, "S"); // passing Value to DBClass from model
            if (result.Tables[1].Rows.Count > 0)
            {

                ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
                ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();

                DateTime sDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["sDate"]);
                DateTime eDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["eDate"]);

                DateTime sDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["sDate"]);
                DateTime eDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["eDate"]);

                DateTime sDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["sDate"]);
                DateTime eDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["eDate"]);

                DateTime sDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["sDate"]);
                DateTime eDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["eDate"]);

                DateTime dtTodate = Convert.ToDateTime(DateTime.Today);

                List<SelectListItem> itemsch = new List<SelectListItem>();
                if (ViewBag.Matric == "1" && dtTodate <= eDateM)
                {
                    itemsch.Add(new SelectListItem { Text = "Matriculation Regular", Value = "1" });
                }
                //if (ViewBag.OMatric == "1" && sDateMO <= eDateMO)
                //{
                //    itemsch.Add(new SelectListItem { Text = "Matriculation Open", Value = "2" });
                //}
                //if (ViewBag.Senior == "1" && sDateT <= eDateT)
                //{
                //    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Regular", Value = "3" });
                //}
                //if (ViewBag.OSenior == "1" && sDateTO <= eDateTO)
                //{
                //    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Open", Value = "4" });
                //}
                if (ViewBag.Matric != "1" && ViewBag.OMatric != "1" && ViewBag.Senior != "1" && ViewBag.OSenior != "1")
                {
                    itemsch.Add(new SelectListItem { Text = "", Value = "" });
                }
                ViewBag.MySch = itemsch.ToList();

            }

            DataSet seleLastCanPen = objDB.PendingCorrectionSubjects(schlcode, "2");
            if (seleLastCanPen.Tables[0].Rows.Count > 0)
            {

                @ViewBag.message = "1";
                rm.StoreAllData = seleLastCanPen;
                ViewBag.TotalViewAllCount = seleLastCanPen.Tables[0].Rows.Count;
                ViewBag.TotalCount = seleLastCanPen.Tables[0].Rows.Count;
            }
            else
            {
                @ViewBag.message = "Record Not Found";
            }

            if (cmd == "View All Correction Pending Record")
            {
                //    var itemsch = new SelectList(new[]{new {ID="1",Name="Matriculation Regular"},new{ID="2",Name="Matriculation Open"},
                //new{ID="3",Name="Sr.Secondary Regular"},new{ID="4",Name="Sr.Secondary Open"},}, "ID", "Name", 1);
                //    ViewBag.MySch = itemsch.ToList();


                DataSet seleLastCan = objDB.PendingCorrectionSubjects(schlcode, "2");
                if (seleLastCan.Tables[0].Rows.Count > 0)
                {

                    @ViewBag.message = "1";
                    rm.StoreAllData = seleLastCan;
                    ViewBag.TotalViewAllCount = seleLastCan.Tables[0].Rows.Count;
                    ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                }
                else
                {
                    @ViewBag.message = "Record Not Found";
                }


                return View(rm);
            }
            if (cmd == "View All Correction Record")
            {
                DataSet seleLastCan = objDB.ViewAllCorrectionSubjects(schlcode);
                if (seleLastCan.Tables[0].Rows.Count > 0)
                {

                    @ViewBag.message = "1";
                    rm.StoreAllData = seleLastCan;
                    ViewBag.TotalViewAllCount = seleLastCan.Tables[0].Rows.Count;
                    ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                }
                else
                {
                    @ViewBag.message = "Record Not Found";
                }


                return View(rm);
            }
            else
            {
                try
                {
                    DataSet seleLastCan = objDB.SearchCorrectionStudentDetails(formname, schlcode, sid);
                    if (seleLastCan.Tables[0].Rows.Count > 0)
                    {

                        @ViewBag.message = "1";
                        @ViewBag.stdid = seleLastCan.Tables[0].Rows[0]["std_id"].ToString();
                        @ViewBag.Oroll = seleLastCan.Tables[0].Rows[0]["Board_Roll_Num"].ToString();
                        @ViewBag.Regno = seleLastCan.Tables[0].Rows[0]["Registration_num"].ToString();
                        @ViewBag.category = seleLastCan.Tables[0].Rows[0]["category"].ToString();
                        @ViewBag.session = seleLastCan.Tables[0].Rows[0]["Year"].ToString() + "-" + seleLastCan.Tables[0].Rows[0]["Month"].ToString();
                        @ViewBag.canName = seleLastCan.Tables[0].Rows[0]["Candi_Name"].ToString();
                        @ViewBag.FName = seleLastCan.Tables[0].Rows[0]["Father_Name"].ToString();
                        @ViewBag.Mname = seleLastCan.Tables[0].Rows[0]["Mother_Name"].ToString();
                        @ViewBag.lot = seleLastCan.Tables[0].Rows[0]["lot"].ToString();
                        @ViewBag.DOB = seleLastCan.Tables[0].Rows[0]["DOB"].ToString();
                        @ViewBag.Frm = formname;
                        @ViewBag.Subjlist = seleLastCan.Tables[0].Rows[0]["SubjectList"].ToString();
                    }
                    else
                    {
                        @ViewBag.message = "Record Not Found";
                        return View(rm);
                    }

                    if (ModelState.IsValid)
                    {
                        //-------------New SubList---------------//
                        DataSet NewSub = new DataSet();
                        NewSub = objDB.NewCorrectionSubjects(sid, formname, schlcode);  //NewCorrectionSubjects
                        List<SelectListItem> NewSUBList = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in NewSub.Tables[0].Rows)
                        {
                            NewSUBList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });

                        }
                        ViewBag.SubNew = NewSUBList;
                        ViewBag.MediumNew = objCommon.GetMediumAll();
                        ViewBag.SubNewCnt = NewSub.Tables[0].Rows.Count;
                        //-----------End-------------------//                    

                        //----------Old Subject Fill Start----------//
                        DataSet ds = objDB.SearchOldStudent_Subject(sid, formname, schlcode);
                        if (ds == null || ds.Tables[0].Rows.Count == 0)
                        {
                            return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects");
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {

                            List<SelectListItem> OLDSUBList = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                            {
                                OLDSUBList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });

                            }
                            ViewBag.SubOLd = OLDSUBList;
                            ViewBag.SubCnt = ds.Tables[0].Rows.Count;
                        }

                        else
                        {

                            return View(rm);
                        }

                    }
                    else
                    {
                        return View(rm);
                    }
                    return View(rm);

                }
                catch (Exception ex)
                {
                    return View(rm);
                }

            }

        }

        [HttpPost]
        public ActionResult SubjectCorrectionAdd(RegistrationModels rm, FormCollection frm, string NSQFsubS6)
        {

            List<SelectListItem> SubWEL = new List<SelectListItem>();
            SubWEL.Add(new SelectListItem { Text = "WELCOME LIFE", Value = "210" });
            ViewBag.SubWEL = new SelectList(SubWEL, "Value", "Text");

            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB(); //calling class DBClass
            DataSet ds2 = objDB.ElectiveSubjects();
            ViewBag.SubS9 = ds2.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.SubS9.Rows)
            {
                items.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }

            ViewBag.SubS10m = ViewBag.SubS9m = ViewBag.SubS9 = new SelectList(items, "Value", "Text");
            ViewBag.S10 = new SelectList(items, "Value", "Text");


            DataSet ds1 = objDB.ElectiveSubjects_Blind();
            ViewBag.SubSb9 = ds1.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> bitems = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.SubSb9.Rows)
            {
                bitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }

            ViewBag.bs2 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs3 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs4 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs5 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs6 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs7 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs8 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs9 = new SelectList(bitems, "Value", "Text");

            string nsqfsub = null;
            DataSet nsresult = objDB.SelectS9(nsqfsub); // passing Value to DBClass from model
            ViewBag.nsfq = nsresult.Tables[0];
            List<SelectListItem> nsfqList = new List<SelectListItem>();
            nsfqList.Add(new SelectListItem { Text = "NSFQ Subjects", Value = "0" });
            foreach (System.Data.DataRow dr in ViewBag.nsfq.Rows)
            {
                nsfqList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }
            ViewBag.nsfqList = new SelectList(nsfqList, "Value", "Text");

            List<SelectListItem> itemsub6 = new List<SelectListItem>();
            itemsub6.Add(new SelectListItem { Text = "SOCIAL STUDIES", Value = "06" });
            ViewBag.SubS6 = new SelectList(itemsub6, "Value", "Text");

            List<SelectListItem> itemsub10 = new List<SelectListItem>();
            itemsub10.Add(new SelectListItem { Text = "HINDI", Value = "03" });
            itemsub10.Add(new SelectListItem { Text = "SOCIAL STUDIES", Value = "06" });
            ViewBag.SubS10 = new SelectList(itemsub10, "Value", "Text");
            //---------------------           
            ViewBag.DAb = objCommon.GetDA();
            ViewBag.DA = objCommon.GetDA();

            //string id = rm.Std_id.ToString();
            if (ModelState.IsValid)
            {
                // AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                string id = frm["Std_id"].ToString();
                string formname = "M";
                DataSet ds = objDB.SearchStudentGetByData_SubjectCORR(id, formname);

                //string stdPic = null;
                //string formName = "M2";

                //--------------NSQF---------------------//
                string ses = Session["Session"].ToString();
                string schlcode = Session["SCHL"].ToString();
                DataSet dsnsqf = objDB.CHkNSQF(schlcode, ses);
                if (dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "True" || dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "1")
                {
                    ViewData["NSQFSCHOOL"] = "1";
                    ViewBag.NSQFSTUDENT = "1";
                }
                else
                {
                    ViewData["NSQFSCHOOL"] = "0";
                    ViewBag.NSQFSTUDENT = "0";
                }

                //------------------End NSQF--------------------------//

                // Start Subject Master
                DataTable dtMatricSubject = new DataTable();
                dtMatricSubject.Columns.Add(new DataColumn("CLASS", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUB", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUBNM", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUBABBR", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("MEDIUM", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUBCAT", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUB_SEQ", typeof(int)));
                DataRow dr = null;
                int j = 0;
                for (int i = 1; i <= 12; i++)
                {
                    dr = dtMatricSubject.NewRow();
                    dr["CLASS"] = 2;
                    DataSet dsSub = new DataSet();
                    dr["SUBNM"] = "";
                    dr["SUBABBR"] = "";

                    if (rm.DA == "N.A.")
                    {
                        if (i == 1)
                        {
                            if (rm.subm1 != null)
                            {
                                dr["MEDIUM"] = rm.subm1;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.subS1; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subS1 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subS1.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 2)
                        {
                            if (rm.subM2 != null)
                            {
                                dr["MEDIUM"] = rm.subM2;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.subS2; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subS2 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subS2.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 3)
                        {
                            if (rm.subm3 != null)
                            {
                                dr["MEDIUM"] = rm.subm3;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.subS3; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subS3 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subS3.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 4)
                        {
                            if (rm.subM4 != null)
                            {
                                dr["MEDIUM"] = rm.subM4;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.subS4; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subS4 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subS4.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 5)
                        {
                            if (rm.subM5 != null)
                            {
                                dr["MEDIUM"] = rm.subM5;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }

                            dr["SUB"] = rm.subS5; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subS5 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subS5.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }

                        }
                        else if (i == 6)
                        {
                            if (rm.subM6 != null)
                            {
                                dr["MEDIUM"] = rm.subM6;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }

                            dr["SUB"] = rm.subS6; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subS6 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subS6.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }

                        }

                        //  //If DA=N.A. then fix sub7=Computer Science, sub8-Welcome Life and sub9=Elective Subject/NSQF subject
                        else if (i == 7)
                        {
                            rm.subS7 = "63";//sub7=Computer Science,
                            dr["SUB"] = rm.subS7; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.subS7 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subS7.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 8)
                        {
                            rm.subS8 = "92";// sub8-Welcome Life
                            dr["SUB"] = rm.subS8; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.subS8 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subS8.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 9)
                        {
                            dr["MEDIUM"] = "Medium";

                            if (!string.IsNullOrEmpty(NSQFsubS6))
                            {
                                if (NSQFsubS6.ToUpper() != "NO".ToUpper() && rm.s9 != NSQFsubS6)
                                {
                                    rm.s9 = NSQFsubS6;
                                }
                            }

                            dr["SUB"] = rm.s9; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.s9 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.s9.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 10)
                        {
                            dr["SUB"] = rm.s10; dr["MEDIUM"] = rm.m10; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "A";
                            if (rm.NSQF == "NO")
                            {
                                if (rm.s10 != null)
                                {
                                    dsSub = objDB.GetNAmeAndAbbrbySub(rm.s10.ToString());
                                    dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                    dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                                }
                            }
                            else
                            {
                                dr["SUB"] = rm.ns10;
                                if (rm.ns10 != null && rm.ns10 != "0")
                                {
                                    dsSub = objDB.GetNAmeAndAbbrbySub(rm.ns10.ToString());
                                    dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                    dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                                }
                            }

                        }
                        else if (i == 11)
                        {
                            if (rm.m11 != null)
                            {
                                dr["MEDIUM"] = rm.m11;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.s11; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "A";
                            if (rm.s11 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.s11.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 12)
                        {
                            if (rm.m12 != null)
                            {
                                dr["MEDIUM"] = rm.m12;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.s12; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "A";
                            if (rm.s12 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.s12.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                    }
                    else
                    {
                        if (i == 1)
                        {
                            if (rm.bm1 != null)
                            {
                                dr["MEDIUM"] = rm.bm1;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.subbS1; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subbS1 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS1.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 2)
                        {
                            if (rm.bm2 != null)
                            {
                                dr["MEDIUM"] = rm.bm2;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.subbS2; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subbS2 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS2.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 3)
                        {
                            if (rm.bm3 != null)
                            {
                                dr["MEDIUM"] = rm.bm3;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.subbS3; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subbS3 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS3.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 4)
                        {
                            if (rm.bm4 != null)
                            {
                                dr["MEDIUM"] = rm.bm4;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }

                            dr["SUB"] = rm.subbS4; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subbS4 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS4.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 5)
                        {
                            if (rm.bm5 != null)
                            {
                                dr["MEDIUM"] = rm.bm5;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.subbS5; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subbS5 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS5.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 6)
                        {
                            dr["SUB"] = rm.subbS6; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subbS6 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS6.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 7)
                        {
                            if (rm.bm7 != null)
                            {
                                dr["MEDIUM"] = rm.bm7;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.subbS7; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.subbS7 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS7.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 8)
                        {
                            if (rm.bm8 != null)
                            {
                                dr["MEDIUM"] = rm.bm8;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.subbS8; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.subbS8 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS8.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 9)
                        {
                            if (!string.IsNullOrEmpty(NSQFsubS6))
                            {
                                if (NSQFsubS6.ToUpper() != "NO".ToUpper() && rm.s9 != NSQFsubS6)
                                {
                                    rm.subbS9 = NSQFsubS6;
                                }
                            }

                            dr["SUB"] = rm.subbS9; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.subbS9 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS9.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }

                    }
                    dtMatricSubject.Rows.Add(dr);
                    if (rm.DA == "N.A.")
                    {
                        if (i == 1)
                        {
                            dr = dtMatricSubject.NewRow();
                            dr["CLASS"] = 2;
                            //DataSet dsSub = new DataSet();
                            dr["SUBNM"] = "";
                            dr["SUBABBR"] = "";
                            dr["MEDIUM"] = "Medium";
                            if (rm.subS1 == "01")
                            {
                                dr["SUB"] = "72"; dr["SUB_SEQ"] = 11; dr["SUBCAT"] = "R";
                                if (dr["SUB"] != null)
                                {
                                    dsSub = objDB.GetNAmeAndAbbrbySub(dr["SUB"].ToString());
                                    dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                    dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                                }

                            }
                            else
                            {
                                dr["SUB"] = "73"; dr["SUB_SEQ"] = 11; dr["SUBCAT"] = "R";
                                if (dr["SUB"] != null)
                                {
                                    dsSub = objDB.GetNAmeAndAbbrbySub(dr["SUB"].ToString());
                                    dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                    dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                                }

                            }
                            dtMatricSubject.Rows.Add(dr);
                        }
                        //if (i == 6)
                        //{
                        //    dr = dtMatricSubject.NewRow();
                        //    dr["CLASS"] = 2;
                        //    //DataSet dsSub = new DataSet();
                        //    dr["SUBNM"] = "";
                        //    dr["SUBABBR"] = "";
                        //    dr["MEDIUM"] = "Medium";
                        //    //if (rm.NsqfsubS6 != "NO")
                        //    if (rm.NsqfsubS6 != "NO" && rm.NsqfsubS6 != null && rm.NsqfsubS6 != "")
                        //    {
                        //        dr["SUB"] = "85"; dr["SUB_SEQ"] = 12; dr["SUBCAT"] = "R";
                        //        if (dr["SUB"] != null)
                        //        {
                        //            dsSub = objDB.GetNAmeAndAbbrbySub(dr["SUB"].ToString());
                        //            dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                        //            dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                        //        }
                        //    }
                        //    dtMatricSubject.Rows.Add(dr);
                        //}
                    }
                    else
                    {
                        if (i == 1)
                        {
                            dr = dtMatricSubject.NewRow();
                            dr["CLASS"] = 2;
                            //DataSet dsSub = new DataSet();
                            dr["SUBNM"] = "";
                            dr["SUBABBR"] = "";
                            dr["MEDIUM"] = "Medium";
                            if (rm.subbS1 == "01")
                            {
                                dr["SUB"] = "72"; dr["SUB_SEQ"] = 11; dr["SUBCAT"] = "R";
                                if (dr["SUB"] != null)
                                {
                                    dsSub = objDB.GetNAmeAndAbbrbySub(dr["SUB"].ToString());
                                    dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                    dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                                }

                            }
                            else
                            {
                                dr["SUB"] = "73"; dr["SUB_SEQ"] = 11; dr["SUBCAT"] = "R";
                                if (dr["SUB"] != null)
                                {
                                    dsSub = objDB.GetNAmeAndAbbrbySub(dr["SUB"].ToString());
                                    dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                    dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                                }

                            }
                            dtMatricSubject.Rows.Add(dr);
                        }

                    }


                }

                dtMatricSubject.AcceptChanges();
                dtMatricSubject = dtMatricSubject.AsEnumerable().Where(r => r.ItemArray[1].ToString() != "").CopyToDataTable();



                if (Session["SCHOOLDIST"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }



                var duplicates = dtMatricSubject.AsEnumerable().GroupBy(r => r[2]).Where(gr => gr.Count() > 1).ToList();
                if (duplicates.Any())
                {
                    TempData["Duplicate"] = ViewBag.Duplicate = "Duplicate Subjects: " + String.Join(", ", duplicates.Select(dupl => dupl.Key));
                    TempData["resultUpdate"] = 10;
                    return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects");
                }

                var DTNoOfSub = dtMatricSubject.AsEnumerable().Where(r => r.Field<string>("SUB") != "72" && r.Field<string>("SUB") != "73" && r.Field<string>("SUB") != "205").ToList();

                int NoOfSub = DTNoOfSub.Count();
                if (rm.DA == "N.A.")
                {
                    if (NoOfSub < 9)
                    {
                        TempData["resultUpdate"] = 15;
                        return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects");
                    }
                }
                else
                {
                    if (NoOfSub < 6)
                    {
                        TempData["resultUpdate"] = 15;
                        return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects");
                    }
                }


                // Check NSQF Subject exists in Subject List after NSQF Current Class Selection 
                if (!string.IsNullOrEmpty(NSQFsubS6))
                {
                    if (NSQFsubS6.ToUpper() != "NO".ToUpper())
                    {
                        rm.NsqfsubS6 = NSQFsubS6;
                        var NSQFSubExists = dtMatricSubject.AsEnumerable().Where(r => r.Field<string>("SUB") == NSQFsubS6).Count() > 0;
                        if (!NSQFSubExists)
                        {
                            TempData["resultUpdate"] = "NSQFSUBWANT";
                            return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects");
                        }

                    }
                }


                string result = objDB.Matric_Subject_Correction(rm, frm, id, dtMatricSubject);
                ModelState.Clear();
                //--For Showing Message---------//
                //ViewData["resultUpdate"] = result;
                TempData["resultUpdate"] = result;
                return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects", result);


            }

            return View(rm);
        }
        public ActionResult CorrSubDelete(string id)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB(); //calling class DBClass
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }

            if (id == null)
            {
                return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects");

            }
            else
            {
                string result = objDB.DeleteMatricSubData(id);
                if (result == "Deleted")
                {
                    @ViewBag.result = "1";

                }

            }
            return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects");
        }
        public ActionResult SRCorrSubDelete(string id)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB(); //calling class DBClass
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }

            if (id == null)
            {
                return RedirectToAction("SRSubjectCorrectionPerforma", "CorrectionSubjects");

            }
            else
            {
                string result = objDB.DeleteMatricSubData(id);
                if (result == "Deleted")
                {
                    @ViewBag.result = "1";

                }

            }
            return RedirectToAction("SRSubjectCorrectionPerforma", "CorrectionSubjects");
        }
        public ActionResult openCorrSubDelete(string id)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB(); //calling class DBClass
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }

            if (id == null)
            {
                return RedirectToAction("openSubjectCorrectionPerforma", "CorrectionSubjects");

            }
            else
            {
                string result = objDB.DeleteMatricSubData(id);
                if (result == "Deleted")
                {
                    @ViewBag.result = "1";

                }

            }
            return RedirectToAction("openSubjectCorrectionPerforma", "CorrectionSubjects");
        }

        public ActionResult SubjectCorrectionViewALL(RegistrationModels rm)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB(); //calling class DBClass
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                string schlcode = Session["SCHL"].ToString();
                DataSet seleLastCan = objDB.ViewAllCorrectionSubjects(schlcode);
                if (seleLastCan.Tables[0].Rows.Count > 0)
                {

                    @ViewBag.message = "1";
                    rm.StoreAllData = seleLastCan;
                    ViewBag.TotalViewAllCount = seleLastCan.Tables[0].Rows.Count;
                }
                else
                {
                    @ViewBag.message = "Record Not Found";
                }

            }
            return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects");
        }
        public ActionResult FinalSubmitCorrection(FormCollection frm)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            RegistrationModels rm = new RegistrationModels();
            try
            {
                if (ModelState.IsValid)
                {

                    if (Session["SCHL"] != null)
                    {

                        rm.SCHL = Session["SCHL"].ToString();
                        rm.Correctiontype = "01";
                        string Class = "2";
                        string resultFS = objDB.FinalSubmitSubjectCorrection(rm, Class); // passing Value to DBClass from model
                        if (Convert.ToInt16(resultFS) > 0)
                        {
                            ViewData["resultFS"] = resultFS;
                            //return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects");
                            return RedirectToAction("SchoolCorrectionFinalPrintLst", "RegistrationPortal");
                        }
                        else
                        {
                            ViewData["resultFS"] = "";
                            return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Logout", "Login");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Login");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login");
            }
        }
        #endregion matric subject correction

        //-------------------------End Matric Subject Correction---------------------------/

        //-------------------------Senior Subject Correction---------------------------/

        #region Senior Subject Correction
        public ActionResult SRSubjectCorrectionPerforma(RegistrationModels rm)
        {
            ViewBag.YesNoListCP = new AbstractLayer.DBClass().GetYesNo().ToList();
            ViewBag.YesNoListText = new AbstractLayer.DBClass().GetYesNoText().ToList();
            ViewBag.nsfqPatternList = objCommon.GetNsqfPatternList();
            try
            {
                List<SelectListItem> SubWEL = new List<SelectListItem>();
                SubWEL.Add(new SelectListItem { Text = "WELCOME LIFE", Value = "210" });
                ViewBag.SubWEL = new SelectList(SubWEL, "Value", "Text");
                //ViewBag.SelectedItem
                AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                //RegistrationModels rm = new RegistrationModels();
                string schlid = "";
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    ViewBag.catg = objCommon.GetE2Category();
                    ViewBag.vm5 = objCommon.GetGroupMedium();
                    ViewBag.vm6 = objCommon.GetGroupMedium();
                    ViewBag.vm7 = objCommon.GetGroupMedium();
                    ViewBag.sm2 = objCommon.GetGroupMedium();
                    ViewBag.sm3 = ViewBag.Commonmedium = objCommon.GetGroupMedium();
                    ViewBag.sm4 = objCommon.GetGroupMedium();
                    ViewBag.ccom2 = objCommon.GetGroupMedium();

                    ViewBag.hm2 = objCommon.GetMediumAll();
                    ViewBag.hm5 = objCommon.GetMediumAll();
                    ViewBag.hm6 = objCommon.GetMediumAll();
                    ViewBag.hm7 = objCommon.GetMediumAll();
                    ViewBag.hm8 = objCommon.GetMediumAll();

                    ViewBag.tm2 = objCommon.GetMediumAll();
                    ViewBag.tm5 = objCommon.GetMediumAll();
                    ViewBag.tm6 = objCommon.GetMediumAll();
                    ViewBag.tm7 = objCommon.GetMediumAll();
                    ViewBag.tm8 = objCommon.GetMediumAll();

                    ViewBag.vm10 = objCommon.GetMediumAll();

                    if (Session["SCHL"] == null)
                    {
                        return RedirectToAction("Logout", "Login");
                    }
                    else
                    {
                        int status = objDB.CheckSchoolAssignForm(2, Session["SCHL"].ToString());

                        //int res = 0;
                        //objDB.CheckDateE1E2T1T1(Session["SCHL"].ToString(), out res);
                        //ViewBag.dsts = res;.
                        string admdate = "";
                        objDB.CheckDateE1E2T1T2(Session["SCHL"].ToString(), out admdate);
                        ViewBag.admdate = admdate;
                        if (status > 0)
                        {
                            if (status == 0)
                            { return RedirectToAction("Index", "Home"); }
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }

                    }

                    //-----------------------------------------------------Nsqf -------------------------
                    string ses = Session["Session"].ToString();
                    string schlcode = Session["SCHL"].ToString();
                    DataSet dsnsqf = objDB.CHkNSQF(schlcode, ses);
                    if (dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "True" || dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "1")
                    {
                        ViewData["NSQFSCHOOL"] = "1";
                    }
                    else
                    {
                        ViewData["NSQFSCHOOL"] = "0";
                    }
                    string nsqfsub = null;
                    DataSet nsresult = objDB.SelectS12(nsqfsub); // passing Value to DBClass from model
                    ViewBag.nsfq = nsresult.Tables[0];
                    List<SelectListItem> nsfqList = new List<SelectListItem>();
                    //nsfqList.Add(new SelectListItem { Text = "NSFQ Subjects", Value = "0" });
                    nsfqList.Add(new SelectListItem { Text = "NO", Value = "NO" });
                    foreach (System.Data.DataRow dr in ViewBag.nsfq.Rows)
                    {
                        nsfqList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["SUB"].ToString() });
                    }
                    ViewBag.nsfqList = new SelectList(nsfqList, "Value", "Text");
                    ViewBag.nsqfcatg = new SelectList(nsfqList, "Value", "Text");

                    //-----------------------------------------------------End Nsqf -------------------------
                    #region 12th subject bind by stream

                    DataSet dsCOMM = objDB.SubjectsTweleve_Commerce();
                    if (dsCOMM == null || dsCOMM.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.ssub = dsCOMM.Tables[0];
                        List<SelectListItem> COMMSub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsCOMM.Tables[0].Rows)
                        {
                            COMMSub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }

                        ViewBag.coms9List = new SelectList(COMMSub, "Value", "Text");


                    }

                    DataSet scis = objDB.SubjectsTweleve_SCI();
                    if (scis == null || scis.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.ssub = scis.Tables[0];
                        List<SelectListItem> scisub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.ssub.Rows)
                        {
                            scisub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }

                        ViewBag.sss8 = new SelectList(scisub, "Value", "Text");


                    }
                    DataSet hds = objDB.SubjectsTweleve_hum();
                    if (hds == null || hds.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.hsub = hds.Tables[0];
                        List<SelectListItem> humsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.hsub.Rows)
                        {
                            humsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }
                        ViewBag.hs5 = new SelectList(humsub, "Value", "Text");
                        ViewBag.hs6 = new SelectList(humsub, "Value", "Text");
                        ViewBag.hs7 = new SelectList(humsub, "Value", "Text");
                        ViewBag.hs8 = new SelectList(humsub, "Value", "Text");
                        ViewBag.vs10 = new SelectList(humsub, "Value", "Text"); //--------humanity Subjects Equals To Vocational Aditional Subjects
                    }
                    DataSet tecds = objDB.SubjectsTweleve_tech();
                    if (tecds == null || tecds.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.tsub = tecds.Tables[0];
                        List<SelectListItem> tecsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.tsub.Rows)
                        {
                            tecsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }
                        ViewBag.ts5 = new SelectList(tecsub, "Value", "Text");
                        ViewBag.ts6 = new SelectList(tecsub, "Value", "Text");
                        ViewBag.ts7 = new SelectList(tecsub, "Value", "Text");
                        ViewBag.ts8 = new SelectList(tecsub, "Value", "Text");

                    }

                    DataSet agrds = objDB.SubjectsTweleve_agr();
                    if (agrds == null || agrds.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.agrsub = agrds.Tables[0];
                        List<SelectListItem> agrsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.agrsub.Rows)
                        {
                            agrsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }
                        List<SelectListItem> agrAdditionAlsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in agrds.Tables[1].Rows)
                        {
                            agrAdditionAlsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }
                        ViewBag.as5 = new SelectList(agrsub, "Value", "Text").Where(s => s.Value == "065").ToList();
                        ViewBag.as6 = new SelectList(agrsub, "Value", "Text").Where(s => s.Value != "065").ToList();
                        ViewBag.as7 = new SelectList(agrsub, "Value", "Text").Where(s => s.Value != "065").ToList();
                        ViewBag.as8 = new SelectList(agrAdditionAlsub, "Value", "Text");

                    }
                    DataSet vocds = objDB.Voc_agr();
                    if (vocds == null || vocds.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.vocsub = vocds.Tables[0];
                        List<SelectListItem> vsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.vocsub.Rows)
                        {
                            vsub.Add(new SelectListItem { Text = @dr["group"].ToString(), Value = @dr["group"].ToString() });
                        }
                        ViewBag.VolgroupRN = vsub;
                        ViewBag.selgroup = new SelectList(vsub, "Value", "Text");

                    }
                    DataSet vocsubjects = objDB.SubjectsTweleve_Voc();
                    if (vocsubjects == null || vocsubjects.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.vsub = vocsubjects.Tables[0];
                        List<SelectListItem> vocsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.vsub.Rows)
                        {
                            vocsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }
                        ViewBag.vs5 = ViewBag.vos5 = vocsub;
                        ViewBag.vs6 = ViewBag.vos6 = vocsub;
                        ViewBag.vs7 = ViewBag.vos7 = vocsub;

                        List<SelectListItem> vocAdditionAlsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in vocsubjects.Tables[1].Rows)
                        {
                            vocAdditionAlsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }
                        ViewBag.vocAddSubList = new SelectList(vocAdditionAlsub, "Value", "Text");

                    }

                    DataSet vocTrade = objDB.SubjectsTweleve_Voc_All_Trade();
                    if (vocTrade == null || vocTrade.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.Tsub = vocTrade.Tables[0];
                        List<SelectListItem> vocTsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.Tsub.Rows)
                        {
                            vocTsub.Add(new SelectListItem { Text = @dr["tname"].ToString(), Value = @dr["tcode"].ToString() });
                        }
                        ViewBag.trgroup = new SelectList(vocTsub, "Value", "Text");

                    }

                    #endregion 12th subject bind by stream

                    List<SelectListItem> MyGroupList = objCommon.GroupName();
                    DataTable dtAssignSubject = objCommon.GetAssignSubjectBySchool(Session["SCHL"].ToString());
                    if (dtAssignSubject != null && dtAssignSubject.Rows.Count > 0)
                    {
                        ViewBag.MyGroup = objCommon.GetSubjectsBySchool(dtAssignSubject, MyGroupList);
                    }
                    ViewBag.MyGroup = MyGroupList;

                    //----------------------Senior  Subjects End---------------------------


                    ViewBag.btnshow = "0";

                    schlid = Session["SCHL"].ToString();

                    DataSet seleLastCan = objDB.PendingCorrectionSubjects(schlid, "4");
                    if (seleLastCan.Tables[0].Rows.Count > 0)
                    {

                        @ViewBag.message = "1";
                        rm.StoreAllData = seleLastCan;
                        ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                        //for (int i = 0; i < seleLastCan.Tables[0].Rows.Count; i++)
                        //{

                        //    if (seleLastCan.Tables[0].Rows[i]["DiffSub"].ToString() == null || seleLastCan.Tables[0].Rows[i]["DiffSub"].ToString() == "")
                        //    {
                        //        string candid = seleLastCan.Tables[0].Rows[i]["candid"].ToString();
                        //        string subNew = seleLastCan.Tables[0].Rows[i]["Newsubcode"].ToString();
                        //        string subOld = seleLastCan.Tables[0].Rows[i]["Oldsubcode"].ToString();
                        //        string DiffSub = string.Empty;

                        //        //modified
                        //        //Response.Write("modified");
                        //        if (subNew.Length == subOld.Length)
                        //        {
                        //            var list = subOld.Split(' ').Where(x => (!subNew.Split(' ').Contains(x))).ToList();
                        //            int count = list.Count;
                        //            foreach (var item in list)
                        //            {
                        //                DiffSub = item + " Modified";
                        //            }
                        //        }
                        //        ////Removed
                        //        if (subNew.Length > subOld.Length)
                        //        {
                        //            //Response.Write("Added...");
                        //            var list1 = subNew.Split(' ').Where(x => (!subOld.Split(' ').Contains(x))).ToList();
                        //            int count1 = list1.Count;
                        //            foreach (var item in list1)
                        //            {
                        //                DiffSub = item + " Added";
                        //            }
                        //        }

                        //        string DiffSubCorr = objDB.InsertDiffSubjects(candid, DiffSub);

                        //    }
                        //}



                    }
                    else
                    {
                        @ViewBag.message = "Record Not Found";
                    }
                    ViewBag.MediumNew = objCommon.GetMediumAll();
                    //-------------New SubList---------------//
                    //DataSet NewSub = new DataSet();
                    //NewSub = objDB.NewElectiveSubjects();
                    //List<SelectListItem> NewSUBList = new List<SelectListItem>();
                    //foreach (System.Data.DataRow dr in NewSub.Tables[0].Rows)
                    //{
                    //    NewSUBList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });

                    //}
                    //ViewBag.SubNew = NewSUBList;
                    //ViewBag.MediumNew = objCommon.GetMediumAll();
                    //-----------End-------------------//                   
                }

                //var itemsch = new SelectList(new[]{new {ID="1",Name="Matriculation Regular"},new{ID="2",Name="Matriculation Open"},
                //new{ID="3",Name="Sr.Secondary Regular"},new{ID="4",Name="Sr.Secondary Open"},}, "ID", "Name", 1);
                //ViewBag.MySch = itemsch.ToList();

                DataSet result = objDB.schooltypesCorrection(schlid, "S"); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {
                    ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                    ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
                    ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                    ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();

                    DateTime sDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["sDate"]);
                    DateTime eDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["eDate"]);

                    DateTime sDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["sDate"]);
                    DateTime eDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["eDate"]);

                    DateTime sDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["sDate"]);
                    DateTime eDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["eDate"]);

                    DateTime sDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["sDate"]);
                    DateTime eDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["eDate"]);

                    DateTime dtTodate = Convert.ToDateTime(DateTime.Today);

                    List<SelectListItem> itemsch = new List<SelectListItem>();
                    //if (ViewBag.Matric == "1" && sDateM <= eDateM)
                    //{
                    //    itemsch.Add(new SelectListItem { Text = "Matriculation Regular", Value = "1" });
                    //}
                    //if (ViewBag.OMatric == "1" && sDateMO <= eDateMO)
                    //{
                    //    itemsch.Add(new SelectListItem { Text = "Matriculation Open", Value = "2" });
                    //}
                    if (ViewBag.Senior == "1" && dtTodate <= eDateT)
                    {
                        itemsch.Add(new SelectListItem { Text = "Sr.Secondary Regular", Value = "3" });
                    }
                    //if (ViewBag.OSenior == "1" && sDateTO <= eDateTO)
                    //{
                    //    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Open", Value = "4" });
                    //}

                    if (ViewBag.Matric != "1" && ViewBag.OMatric != "1" && ViewBag.Senior != "1" && ViewBag.OSenior != "1")
                    {
                        itemsch.Add(new SelectListItem { Text = "", Value = "" });
                    }
                    ViewBag.MySch = itemsch.ToList();
                }

                if (ModelState.IsValid)
                {
                    return View(rm);
                }
                else
                { return View(rm); }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }
        }
        [HttpPost]
        public ActionResult SRSubjectCorrectionPerforma(RegistrationModels rm, FormCollection frm, string cmd)
        {
            ViewBag.YesNoListCP = new AbstractLayer.DBClass().GetYesNo().ToList();
            ViewBag.YesNoListText = new AbstractLayer.DBClass().GetYesNoText().ToList();
            ViewBag.nsfqPatternList = objCommon.GetNsqfPatternList();
            ViewBag.btnshow = "1";
            List<SelectListItem> SubWEL = new List<SelectListItem>();
            SubWEL.Add(new SelectListItem { Text = "WELCOME LIFE", Value = "210" });
            ViewBag.SubWEL = new SelectList(SubWEL, "Value", "Text");
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            string sid = Convert.ToString(rm.Std_id);
            string formname = null;
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");

            }
            if (rm.SelList == "1")
            {
                formname = "M";
            }
            if (rm.SelList == "2")
            {
                formname = "MO";
            }
            if (rm.SelList == "3")
            {
                formname = "T";
            }
            if (rm.SelList == "4")
            {
                formname = "TO";
            }
            @ViewBag.DA = objCommon.GetDA();
            @ViewBag.DAb = objCommon.GetDA();
            //-----------------------Senior Subjects Start----------------------

            #region subject
            ViewBag.catg = objCommon.GetE2Category();
            ViewBag.vm5 = objCommon.GetGroupMedium();
            ViewBag.vm6 = objCommon.GetGroupMedium();
            ViewBag.vm7 = objCommon.GetGroupMedium();
            ViewBag.sm2 = objCommon.GetGroupMedium();
            ViewBag.sm3 = ViewBag.Commonmedium = objCommon.GetGroupMedium();
            ViewBag.sm4 = objCommon.GetGroupMedium();
            ViewBag.ccom2 = objCommon.GetGroupMedium();

            ViewBag.hm2 = objCommon.GetMediumAll();
            ViewBag.hm5 = objCommon.GetMediumAll();
            ViewBag.hm6 = objCommon.GetMediumAll();
            ViewBag.hm7 = objCommon.GetMediumAll();
            ViewBag.hm8 = objCommon.GetMediumAll();

            ViewBag.tm2 = objCommon.GetMediumAll();
            ViewBag.tm5 = objCommon.GetMediumAll();
            ViewBag.tm6 = objCommon.GetMediumAll();
            ViewBag.tm7 = objCommon.GetMediumAll();
            ViewBag.tm8 = objCommon.GetMediumAll();

            ViewBag.vm10 = objCommon.GetMediumAll();

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                int status = objDB.CheckSchoolAssignForm(2, Session["SCHL"].ToString());

                //int res = 0;
                //objDB.CheckDateE1E2T1T1(Session["SCHL"].ToString(), out res);
                //ViewBag.dsts = res;.
                string admdate = "";
                objDB.CheckDateE1E2T1T2(Session["SCHL"].ToString(), out admdate);
                ViewBag.admdate = admdate;
                if (status > 0)
                {
                    if (status == 0)
                    { return RedirectToAction("Index", "Home"); }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }

            //-----------------------------------------------------Nsqf -------------------------
            string ses = Session["Session"].ToString();
            string Myschlcode = Session["SCHL"].ToString();
            DataSet dsnsqf = objDB.CHkNSQF(Myschlcode, ses);
            if (dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "True" || dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "1")
            {
                ViewData["NSQFSCHOOL"] = "1";
            }
            else
            {
                ViewData["NSQFSCHOOL"] = "0";
            }
            string nsqfsub = null;
            DataSet nsresult = objDB.SelectS12(nsqfsub); // passing Value to DBClass from model
            ViewBag.nsfq = nsresult.Tables[0];
            List<SelectListItem> nsfqList = new List<SelectListItem>();
            //nsfqList.Add(new SelectListItem { Text = "NSFQ Subjects", Value = "0" });
            nsfqList.Add(new SelectListItem { Text = "NO", Value = "NO" });
            foreach (System.Data.DataRow dr in ViewBag.nsfq.Rows)
            {
                nsfqList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["SUB"].ToString() });
            }
            ViewBag.nsfqList = new SelectList(nsfqList, "Value", "Text");
            ViewBag.nsqfcatg = new SelectList(nsfqList, "Value", "Text");

            //-----------------------------------------------------End Nsqf -------------------------
            #region 12th subject bind by stream

            DataSet dsCOMM = objDB.SubjectsTweleve_Commerce();
            if (dsCOMM == null || dsCOMM.Tables[0].Rows.Count == 0)
            { ViewBag.sub = ""; }
            else
            {
                ViewBag.ssub = dsCOMM.Tables[0];
                List<SelectListItem> COMMSub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in dsCOMM.Tables[0].Rows)
                {
                    COMMSub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                }

                ViewBag.coms9List = new SelectList(COMMSub, "Value", "Text");


            }

            DataSet scis = objDB.SubjectsTweleve_SCI();
            if (scis == null || scis.Tables[0].Rows.Count == 0)
            { ViewBag.sub = ""; }
            else
            {
                ViewBag.ssub = scis.Tables[0];
                List<SelectListItem> scisub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.ssub.Rows)
                {
                    scisub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                }

                ViewBag.sss8 = new SelectList(scisub, "Value", "Text");


            }
            DataSet hds = objDB.SubjectsTweleve_hum();
            if (hds == null || hds.Tables[0].Rows.Count == 0)
            { ViewBag.sub = ""; }
            else
            {
                ViewBag.hsub = hds.Tables[0];
                List<SelectListItem> humsub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.hsub.Rows)
                {
                    humsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                }
                ViewBag.hs5 = new SelectList(humsub, "Value", "Text");
                ViewBag.hs6 = new SelectList(humsub, "Value", "Text");
                ViewBag.hs7 = new SelectList(humsub, "Value", "Text");
                ViewBag.hs8 = new SelectList(humsub, "Value", "Text");
                ViewBag.vs10 = new SelectList(humsub, "Value", "Text"); //--------humanity Subjects Equals To Vocational Aditional Subjects
            }
            DataSet tecds = objDB.SubjectsTweleve_tech();
            if (tecds == null || tecds.Tables[0].Rows.Count == 0)
            { ViewBag.sub = ""; }
            else
            {
                ViewBag.tsub = tecds.Tables[0];
                List<SelectListItem> tecsub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.tsub.Rows)
                {
                    tecsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                }
                ViewBag.ts5 = new SelectList(tecsub, "Value", "Text");
                ViewBag.ts6 = new SelectList(tecsub, "Value", "Text");
                ViewBag.ts7 = new SelectList(tecsub, "Value", "Text");
                ViewBag.ts8 = new SelectList(tecsub, "Value", "Text");

            }

            DataSet agrds = objDB.SubjectsTweleve_agr();
            if (agrds == null || agrds.Tables[0].Rows.Count == 0)
            { ViewBag.sub = ""; }
            else
            {
                ViewBag.agrsub = agrds.Tables[0];
                List<SelectListItem> agrsub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.agrsub.Rows)
                {
                    agrsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                }
                List<SelectListItem> agrAdditionAlsub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in agrds.Tables[1].Rows)
                {
                    agrAdditionAlsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                }
                ViewBag.as5 = new SelectList(agrsub, "Value", "Text").Where(s => s.Value == "065").ToList();
                ViewBag.as6 = new SelectList(agrsub, "Value", "Text").Where(s => s.Value != "065").ToList();
                ViewBag.as7 = new SelectList(agrsub, "Value", "Text").Where(s => s.Value != "065").ToList();
                ViewBag.as8 = new SelectList(agrAdditionAlsub, "Value", "Text");

            }
            DataSet vocds = objDB.Voc_agr();
            if (vocds == null || vocds.Tables[0].Rows.Count == 0)
            { ViewBag.sub = ""; }
            else
            {
                ViewBag.vocsub = vocds.Tables[0];
                List<SelectListItem> vsub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.vocsub.Rows)
                {
                    vsub.Add(new SelectListItem { Text = @dr["group"].ToString(), Value = @dr["group"].ToString() });
                }
                ViewBag.VolgroupRN = vsub;
                ViewBag.selgroup = new SelectList(vsub, "Value", "Text");

            }
            DataSet vocsubjects = objDB.SubjectsTweleve_Voc();
            if (vocsubjects == null || vocsubjects.Tables[0].Rows.Count == 0)
            { ViewBag.sub = ""; }
            else
            {
                ViewBag.vsub = vocsubjects.Tables[0];
                List<SelectListItem> vocsub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.vsub.Rows)
                {
                    vocsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                }
                ViewBag.vs5 = ViewBag.vos5 = vocsub;
                ViewBag.vs6 = ViewBag.vos6 = vocsub;
                ViewBag.vs7 = ViewBag.vos7 = vocsub;

                List<SelectListItem> vocAdditionAlsub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in vocsubjects.Tables[1].Rows)
                {
                    vocAdditionAlsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                }
                ViewBag.vocAddSubList = new SelectList(vocAdditionAlsub, "Value", "Text");

            }

            DataSet vocTrade = objDB.SubjectsTweleve_Voc_All_Trade();
            if (vocTrade == null || vocTrade.Tables[0].Rows.Count == 0)
            { ViewBag.sub = ""; }
            else
            {
                ViewBag.Tsub = vocTrade.Tables[0];
                List<SelectListItem> vocTsub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.Tsub.Rows)
                {
                    vocTsub.Add(new SelectListItem { Text = @dr["tname"].ToString(), Value = @dr["tcode"].ToString() });
                }
                ViewBag.trgroup = new SelectList(vocTsub, "Value", "Text");

            }

            #endregion 12th subject bind by stream

            List<SelectListItem> MyGroupList = objCommon.GroupName();
            DataTable dtAssignSubject = objCommon.GetAssignSubjectBySchool(Session["SCHL"].ToString());
            if (dtAssignSubject != null && dtAssignSubject.Rows.Count > 0)
            {
                ViewBag.MyGroup = objCommon.GetSubjectsBySchool(dtAssignSubject, MyGroupList);
            }
            ViewBag.MyGroup = MyGroupList;
            #endregion subject

            if (sid != null)
            {

                DataSet ds = objDB.SearchStudentGetByData_SubjectCORR(sid, formname);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return RedirectToAction("SRSubjectCorrectionPerforma", "CorrectionSubjects");
                }
                if (ds.Tables[0].Rows.Count > 0)
                {
                    #region Group and subject
                    // DataSet ds = objDB.SearchStudentGetByData(id);
                    rm.Std_id = Convert.ToInt32(ds.Tables[0].Rows[0]["Std_id"].ToString());
                    rm.SCHL = ds.Tables[0].Rows[0]["SCHL"].ToString();
                    //rm.IDNO = ds.Tables[0].Rows[0]["IDNO"].ToString();                   
                    rm.Tgroup = "0";
                    rm.Tgroup = ds.Tables[0].Rows[0]["Group_Name"].ToString().Trim();
                    //ViewBag.MyGroup = objCommon.GroupName();
                    //----------------Subject voc Group-----------
                    if (rm.Tgroup == "VOCATIONAL")
                    {
                        DataSet gp = objDB.selectGP(rm.Std_id);
                        rm.groupsel = gp.Tables[0].Rows[0]["Group"].ToString().Trim();
                        string gsel = gp.Tables[0].Rows[0]["Group"].ToString().Trim();
                        rm.grouptr = gp.Tables[0].Rows[0]["TCODE"].ToString().Trim();
                        DataSet vocTRgroup = objDB.Voc_Trgroup(gsel);
                        if (vocds == null || vocds.Tables[0].Rows.Count == 0)
                        { ViewBag.sub = ""; }
                        else
                        {
                            ViewBag.vocTR = vocTRgroup.Tables[0];
                            List<SelectListItem> vtrsub = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in ViewBag.vocTR.Rows)
                            {
                                vtrsub.Add(new SelectListItem { Text = @dr["TNAME"].ToString(), Value = @dr["TCODE"].ToString() });
                            }

                            ViewBag.trgroup = new SelectList(vtrsub, "Value", "Text");

                        }
                    }
                    //-----------------End---------------

                    string formName = ds.Tables[0].Rows[0]["form_Name"].ToString();

                    rm.PreNSQF = ds.Tables[0].Rows[0]["PRE_NSQF_SUB"].ToString();

                    ViewBag.PreNSQF = ds.Tables[0].Rows[0]["PRE_NSQF_SUB"].ToString();
                    ViewBag.NsqfsubS6 = ds.Tables[0].Rows[0]["NSQF_SUB"].ToString();

                    rm.PreNSQFsci = ds.Tables[0].Rows[0]["PRE_NSQF_SUB"].ToString();
                    rm.NsqfsubS6sci = ds.Tables[0].Rows[0]["NSQF_SUB"].ToString();
                    rm.PreNSQFcomm = ds.Tables[0].Rows[0]["PRE_NSQF_SUB"].ToString();
                    rm.NsqfsubS6comm = ds.Tables[0].Rows[0]["NSQF_SUB"].ToString();
                    rm.PreNSQFvoc = ds.Tables[0].Rows[0]["PRE_NSQF_SUB"].ToString();
                    rm.NsqfsubS6voc = ds.Tables[0].Rows[0]["NSQF_SUB"].ToString();

                    //-----------------------------For NSQF SUBJECTS----------------
                    //string sid = id;
                    DataSet isCHkNSQF = objDB.CHkNSQFStudents(sid);
                    string isNSQFSTUDENT = "0";
                    if (isCHkNSQF.Tables[0].Rows[0]["NSQF_flag"].ToString() == "True")
                    {
                        isNSQFSTUDENT = "1";
                       ViewData["NSQFSTUDENT"] = "1";
                        rm.NsqfsubS6 = ds.Tables[0].Rows[0]["NSQF_SUB"].ToString();
                        DataSet nsTextresult = objDB.GetNSQFVIEWSUBJECT11TH(rm.NsqfsubS6, rm.PreNSQF);
                        List<SelectListItem> nssub6 = new List<SelectListItem>();
                        if (rm.PreNSQF == "NO")
                        {
                            nssub6.Add(new SelectListItem { Text = "NO", Value = "NO" });
                            ViewBag.nsqfcatg = nssub6;
                        }
                        else
                        {
                            if (nsTextresult.Tables[0].Rows.Count > 0)
                            {
                                nssub6.Add(new SelectListItem { Text = "NO", Value = "NO" });
                                nssub6.Add(new SelectListItem { Text = nsTextresult.Tables[0].Rows[0]["Name_ENG"].ToString(), Value = rm.NsqfsubS6 });
                                ViewBag.nsqfcatg = nssub6;
                            }
                            else
                            {
                                nssub6.Add(new SelectListItem { Text = "NO", Value = "NO" });
                                ViewBag.nsqfcatg = nssub6;
                            }

                        }

                    }
                    else
                    {
                        isNSQFSTUDENT = "0";
                        ViewData["NSQFSTUDENT"] = "0";
                        List<SelectListItem> nssub6 = new List<SelectListItem>();
                        nssub6.Add(new SelectListItem { Text = "NO", Value = "NO" });
                        ViewBag.nsqfcatg = nssub6;
                    }



                    if (ViewData["NSQFSCHOOL"].ToString() == "0" && isNSQFSTUDENT == "1")
                    {
                        List<SelectListItem> nsfqList1 = new List<SelectListItem>();
                        nsfqList1.Add(new SelectListItem { Text = "NO", Value = "NO" });                       
                        ViewBag.nsfqList = new SelectList(nsfqList1, "Value", "Text");
                        ViewBag.nsqfcatg = new SelectList(nsfqList1, "Value", "Text");

                    }


                    


                    //--------------------------------End NSQF SUBJECTS-------------


                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                        {
                            if (ds.Tables[0].Rows[0]["Group_Name"].ToString() == "SCIENCE")
                            {
                                if (i == 0)
                                {
                                    rm.coms1 = ds.Tables[1].Rows[0]["SUB"].ToString();
                                    rm.comm1 = ds.Tables[1].Rows[0]["MEDIUM"].ToString();
                                }
                                else if (i == 1)
                                {
                                    rm.coms2 = ds.Tables[1].Rows[1]["SUB"].ToString();
                                    //rm.comm2 = ds.Tables[1].Rows[1]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.coms2);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.comm2 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[1]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }
                                else if (i == 2)
                                {
                                    rm.coms3 = ds.Tables[1].Rows[2]["SUB"].ToString();
                                    //rm.comm3 = ds.Tables[1].Rows[2]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.coms3);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.comm3 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[2]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;

                                }
                                else if (i == 3)
                                {

                                    rm.coms4sci = ds.Tables[1].Rows[3]["SUB"].ToString();
                                    ViewBag.comm4NM = ds.Tables[1].Rows[3]["SUBNM"].ToString();

                                    //rm.comm4 = ds.Tables[1].Rows[3]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.coms4sci);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.comm4 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[3]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }
                                else if (i == 4)
                                {
                                    rm.ssWEL = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.smWEL = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }

                                else if (i == 5)
                                {
                                    rm.ss5 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.sm5 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 6)
                                {
                                    rm.ss6 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.sm6 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 7)
                                {
                                    rm.ss7 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.sm7 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 8)
                                {
                                    rm.ss8 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.sm8 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }

                                //else if (i == 8)
                                //{
                                //    rm.s9 = ds.Tables[1].Rows[8]["SUB"].ToString();
                                //    DataSet SelectedMediumList = objDB.SelectS1(rm.s9);
                                //    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                //    rm.m9 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[8]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                //    ViewBag.SubS9m = iMEdiumList;
                                //}

                            }
                            if (ds.Tables[0].Rows[0]["Group_Name"].ToString() == "COMMERCE")
                            {
                                if (i == 0)
                                {
                                    rm.coms1 = ds.Tables[1].Rows[0]["SUB"].ToString();
                                    rm.comcm1 = ds.Tables[1].Rows[0]["MEDIUM"].ToString();
                                }
                                else if (i == 1)
                                {
                                    rm.comcs2 = ds.Tables[1].Rows[1]["SUB"].ToString();
                                    //rm.comcm2 = ds.Tables[1].Rows[1]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.comcs2);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.comcm2 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[1]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }
                                else if (i == 2)
                                {
                                    rm.coms3 = ds.Tables[1].Rows[2]["SUB"].ToString();
                                    rm.comcm3 = ds.Tables[1].Rows[2]["MEDIUM"].ToString();
                                }
                                else if (i == 3)
                                {
                                    rm.coms4comm = ds.Tables[1].Rows[3]["SUB"].ToString();
                                    ViewBag.comm4NM = ds.Tables[1].Rows[3]["SUBNM"].ToString();
                                    rm.comcm4 = ds.Tables[1].Rows[3]["MEDIUM"].ToString();
                                }
                                else if (i == 4)
                                {
                                    rm.ssWEL = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.smWEL = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 5)
                                {
                                    rm.coms5 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.comm5 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 6)
                                {
                                    rm.coms6 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.comm6 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                //else if (i == 7)
                                //{
                                //    rm.coms7 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                //    rm.comm7 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                //}
                                else if (i == 7)
                                {
                                    rm.coms8 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.comm8 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 8)
                                {
                                    rm.coms9 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.comm9 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                //else if (i == 8)
                                //{
                                //    rm.s9 = ds.Tables[1].Rows[8]["SUB"].ToString();
                                //    DataSet SelectedMediumList = objDB.SelectS1(rm.s9);
                                //    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                //    rm.m9 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[8]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                //    ViewBag.SubS9m = iMEdiumList;
                                //}

                            }
                            if (ds.Tables[0].Rows[0]["Group_Name"].ToString() == "HUMANITIES")
                            {
                                if (i == 0)
                                {
                                    rm.hums1 = ds.Tables[1].Rows[0]["SUB"].ToString();
                                    rm.humm1 = ds.Tables[1].Rows[0]["MEDIUM"].ToString();
                                }
                                else if (i == 1)
                                {
                                    rm.hums2 = ds.Tables[1].Rows[1]["SUB"].ToString();
                                    //rm.humm2 = ds.Tables[1].Rows[1]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.hums2);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.humm2 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[1]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }
                                else if (i == 2)
                                {
                                    rm.hums3 = ds.Tables[1].Rows[2]["SUB"].ToString();
                                    rm.humm3 = ds.Tables[1].Rows[2]["MEDIUM"].ToString();
                                }
                                else if (i == 3)
                                {
                                    rm.hums4 = ds.Tables[1].Rows[3]["SUB"].ToString();
                                    rm.humm4 = ds.Tables[1].Rows[3]["MEDIUM"].ToString();
                                }
                                else if (i == 4)
                                {
                                    rm.ssWEL = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.smWEL = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 5)
                                {
                                    rm.hums5 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.hums5);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.humm5 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[i]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }
                                else if (i == 6)
                                {
                                    rm.hums6 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    // rm.humm6 = ds.Tables[1].Rows[5]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.hums6);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.humm6 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[i]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;

                                }
                                else if (i == 7)
                                {
                                    rm.hums7 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    //rm.humm7 = ds.Tables[1].Rows[6]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.hums7);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.humm7 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[i]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }
                                else if (i == 8)
                                {
                                    rm.hums8 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    //rm.humm8 = ds.Tables[1].Rows[7]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.hums8);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.humm8 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[i]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }


                            }
                            if (ds.Tables[0].Rows[0]["Group_Name"].ToString() == "TECHNICAL")
                            {
                                if (i == 0)
                                {
                                    rm.tecs1 = ds.Tables[1].Rows[0]["SUB"].ToString();
                                    rm.tecm1 = ds.Tables[1].Rows[0]["MEDIUM"].ToString();
                                }
                                else if (i == 1)
                                {
                                    rm.tecs2 = ds.Tables[1].Rows[1]["SUB"].ToString();
                                    //rm.tecm2 = ds.Tables[1].Rows[1]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.tecs2);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.tecm2 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[1]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }
                                else if (i == 2)
                                {
                                    rm.tecs3 = ds.Tables[1].Rows[2]["SUB"].ToString();
                                    rm.tecm3 = ds.Tables[1].Rows[2]["MEDIUM"].ToString();
                                }
                                else if (i == 3)
                                {
                                    rm.tecs4 = ds.Tables[1].Rows[3]["SUB"].ToString();
                                    rm.tecm4 = ds.Tables[1].Rows[3]["MEDIUM"].ToString();
                                }
                                else if (i == 4)
                                {
                                    rm.ssWEL = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.smWEL = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 5)
                                {
                                    rm.tecs5 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    //rm.tecm5 = ds.Tables[1].Rows[4]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.tecs5);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.tecm5 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[i]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }
                                else if (i == 6)
                                {
                                    rm.tecs6 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    //rm.tecm6 = ds.Tables[1].Rows[5]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.tecs6);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.tecm6 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[i]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }
                                else if (i == 7)
                                {
                                    rm.tecs7 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    //rm.tecm7 = ds.Tables[1].Rows[6]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.tecs7);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.tecm7 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[i]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }
                                else if (i == 8)
                                {
                                    rm.tecs8 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.tecm8 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.tecs8);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.tecm8 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[i]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }


                            }
                            if (ds.Tables[0].Rows[0]["Group_Name"].ToString() == "AGRICULTURE")
                            {
                                if (i == 0)
                                {
                                    rm.agrs1 = ds.Tables[1].Rows[0]["SUB"].ToString();
                                    rm.agrm1 = ds.Tables[1].Rows[0]["MEDIUM"].ToString();
                                }
                                else if (i == 1)
                                {
                                    rm.agrs2 = ds.Tables[1].Rows[1]["SUB"].ToString();
                                    //rm.agrm2 = ds.Tables[1].Rows[1]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.agrs2);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.agrm2 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[1]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }
                                else if (i == 2)
                                {
                                    rm.agrs3 = ds.Tables[1].Rows[2]["SUB"].ToString();
                                    rm.agrm3 = ds.Tables[1].Rows[2]["MEDIUM"].ToString();
                                }
                                else if (i == 3)
                                {
                                    rm.agrs4 = ds.Tables[1].Rows[3]["SUB"].ToString();
                                    rm.agrm4 = ds.Tables[1].Rows[3]["MEDIUM"].ToString();
                                }
                                else if (i == 4)
                                {
                                    rm.ssWEL = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.smWEL = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 5)
                                {
                                    rm.agrs5 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    //rm.agrm5 = ds.Tables[1].Rows[4]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.agrs5);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.agrm5 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[i]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }
                                else if (i == 6)
                                {
                                    rm.agrs6 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    //rm.agrm6 = ds.Tables[1].Rows[5]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.agrs6);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.agrm6 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[i]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }
                                else if (i == 7)
                                {
                                    rm.agrs7 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    //rm.agrm7 = ds.Tables[1].Rows[6]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.agrs7);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.agrm7 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[i]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }
                                else if (i == 8)
                                {
                                    rm.agrs8 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    //rm.agrm8 = ds.Tables[1].Rows[7]["MEDIUM"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectTwelveMedium(rm.agrs8);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.agrm8 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[i]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.Commonmedium = iMEdiumList;
                                }

                                //else if (i == 8)
                                //{
                                //    rm.s9 = ds.Tables[1].Rows[8]["SUB"].ToString();
                                //    DataSet SelectedMediumList = objDB.SelectS1(rm.s9);
                                //    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                //    rm.m9 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[8]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                //    ViewBag.SubS9m = iMEdiumList;
                                //}

                            }
                            if (ds.Tables[0].Rows[0]["Group_Name"].ToString() == "VOCATIONAL")
                            {
                                if (i == 0)
                                {
                                    rm.vocs1 = ds.Tables[1].Rows[0]["SUB"].ToString();
                                    rm.vocm1 = ds.Tables[1].Rows[0]["MEDIUM"].ToString();
                                }
                                else if (i == 1)
                                {
                                    rm.vocs2 = ds.Tables[1].Rows[1]["SUB"].ToString();
                                    rm.vocm2 = ds.Tables[1].Rows[1]["MEDIUM"].ToString();
                                }
                                else if (i == 2)
                                {
                                    rm.vocs3 = ds.Tables[1].Rows[2]["SUB"].ToString();
                                    rm.vocm3 = ds.Tables[1].Rows[2]["MEDIUM"].ToString();
                                }
                                else if (i == 3)
                                {
                                    rm.vocs4 = ds.Tables[1].Rows[3]["SUB"].ToString();
                                    ViewBag.vocs4 = ds.Tables[1].Rows[3]["SUBNM"].ToString();
                                    rm.vocm4 = ds.Tables[1].Rows[3]["MEDIUM"].ToString();
                                }
                                else if (i == 4)
                                {
                                    rm.ssWEL = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.smWEL = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 5)
                                {
                                    rm.vocs5 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.vocm5 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                    List<SelectListItem> vos5 = (List<SelectListItem>)ViewBag.vos5;
                                    var VS1 = vos5.ToList().Where(s => s.Value == rm.vocs5).Select(s => s);
                                    ViewBag.vos5 = VS1.ToList();
                                }
                                else if (i == 6)
                                {
                                    rm.vocs6 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.vocm6 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();

                                    List<SelectListItem> vos6 = (List<SelectListItem>)ViewBag.vos6;
                                    var VS2 = vos6.ToList().Where(s => s.Value == rm.vocs6).Select(s => s);
                                    ViewBag.vos6 = VS2.ToList();
                                }
                                else if (i == 7)
                                {
                                    rm.vocs7 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.vocm7 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();

                                    List<SelectListItem> vos7 = (List<SelectListItem>)ViewBag.vos7;
                                    var VS3 = vos7.ToList().Where(s => s.Value == rm.vocs7).Select(s => s);
                                    ViewBag.vos7 = VS3.ToList();
                                }
                                else if (i == 8)
                                {
                                    rm.vocs8 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.vocm8 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 9)
                                {
                                    rm.vocs9 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.vocm9 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 10)
                                {
                                    rm.vocs10 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.vocm10 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                //else if (i == 8)
                                //{
                                //    rm.s9 = ds.Tables[1].Rows[8]["SUB"].ToString();
                                //    DataSet SelectedMediumList = objDB.SelectS1(rm.s9);
                                //    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                //    rm.m9 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[8]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                //    ViewBag.SubS9m = iMEdiumList;
                                //}

                            }
                        }
                    }
                    #endregion Group and subject

                }
                else
                {
                    return RedirectToAction("SRSubjectCorrectionPerforma", "CorrectionSubjects");
                }

            }

            //----------------------Senior  Subjects End---------------------------           

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            string schlcode = Session["SCHL"].ToString();
            //AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            DataSet result = objDB.schooltypesCorrection(schlcode, "S"); // passing Value to DBClass from model
            if (result.Tables[1].Rows.Count > 0)
            {

                ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
                ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();

                DateTime sDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["sDate"]);
                DateTime eDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["eDate"]);

                DateTime sDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["sDate"]);
                DateTime eDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["eDate"]);

                DateTime sDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["sDate"]);
                DateTime eDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["eDate"]);

                DateTime sDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["sDate"]);
                DateTime eDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["eDate"]);

                DateTime dtTodate = Convert.ToDateTime(DateTime.Today);

                List<SelectListItem> itemsch = new List<SelectListItem>();
                //if (ViewBag.Matric == "1" && sDateM <= eDateM)
                //{
                //    itemsch.Add(new SelectListItem { Text = "Matriculation Regular", Value = "1" });
                //}
                //if (ViewBag.OMatric == "1" && sDateMO <= eDateMO)
                //{
                //    itemsch.Add(new SelectListItem { Text = "Matriculation Open", Value = "2" });
                //}
                if (ViewBag.Senior == "1" && dtTodate <= eDateT)
                {
                    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Regular", Value = "3" });
                }
                //if (ViewBag.OSenior == "1" && sDateTO <= eDateTO)
                //{
                //    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Open", Value = "4" });
                //}
                if (ViewBag.Matric != "1" && ViewBag.OMatric != "1" && ViewBag.Senior != "1" && ViewBag.OSenior != "1")
                {
                    itemsch.Add(new SelectListItem { Text = "", Value = "" });
                }
                ViewBag.MySch = itemsch.ToList();

            }

            DataSet seleLastCanPen = objDB.PendingCorrectionSubjects(schlcode, "4");
            if (seleLastCanPen.Tables[0].Rows.Count > 0)
            {

                @ViewBag.message = "1";
                rm.StoreAllData = seleLastCanPen;
                ViewBag.TotalViewAllCount = seleLastCanPen.Tables[0].Rows.Count;
                ViewBag.TotalCount = seleLastCanPen.Tables[0].Rows.Count;
            }
            else
            {
                @ViewBag.message = "Record Not Found";
            }

            if (cmd == "View All Correction Pending Record")
            {
                //    var itemsch = new SelectList(new[]{new {ID="1",Name="Matriculation Regular"},new{ID="2",Name="Matriculation Open"},
                //new{ID="3",Name="Sr.Secondary Regular"},new{ID="4",Name="Sr.Secondary Open"},}, "ID", "Name", 1);
                //    ViewBag.MySch = itemsch.ToList();


                DataSet seleLastCan = objDB.PendingCorrectionSubjects(schlcode, "4");
                if (seleLastCan.Tables[0].Rows.Count > 0)
                {

                    @ViewBag.message = "1";
                    rm.StoreAllData = seleLastCan;
                    ViewBag.TotalViewAllCount = seleLastCan.Tables[0].Rows.Count;
                    ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                }
                else
                {
                    @ViewBag.message = "Record Not Found";
                }


                return View(rm);
            }
            if (cmd == "View All Correction Record")
            {
                DataSet seleLastCan = objDB.ViewAllCorrectionSubjects(schlcode);
                if (seleLastCan.Tables[0].Rows.Count > 0)
                {

                    @ViewBag.message = "1";
                    rm.StoreAllData = seleLastCan;
                    ViewBag.TotalViewAllCount = seleLastCan.Tables[0].Rows.Count;
                    ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                }
                else
                {
                    @ViewBag.message = "Record Not Found";
                }


                return View(rm);
            }
            else
            {
                try
                {
                    DataSet seleLastCan = objDB.SearchCorrectionStudentDetails(formname, schlcode, sid);
                    if (seleLastCan.Tables[0].Rows.Count > 0)
                    {

                        @ViewBag.message = "1";
                        @ViewBag.stdid = seleLastCan.Tables[0].Rows[0]["std_id"].ToString();
                        @ViewBag.Oroll = seleLastCan.Tables[0].Rows[0]["Board_Roll_Num"].ToString();
                        @ViewBag.Regno = seleLastCan.Tables[0].Rows[0]["Registration_num"].ToString();
                        @ViewBag.category = seleLastCan.Tables[0].Rows[0]["category"].ToString();
                        @ViewBag.session = seleLastCan.Tables[0].Rows[0]["Year"].ToString() + "-" + seleLastCan.Tables[0].Rows[0]["Month"].ToString();
                        @ViewBag.canName = seleLastCan.Tables[0].Rows[0]["Candi_Name"].ToString();
                        @ViewBag.FName = seleLastCan.Tables[0].Rows[0]["Father_Name"].ToString();
                        @ViewBag.Mname = seleLastCan.Tables[0].Rows[0]["Mother_Name"].ToString();
                        @ViewBag.lot = seleLastCan.Tables[0].Rows[0]["lot"].ToString();
                        @ViewBag.DOB = seleLastCan.Tables[0].Rows[0]["DOB"].ToString();
                        @ViewBag.Frm = formname;
                        @ViewBag.Subjlist = seleLastCan.Tables[0].Rows[0]["SubjectList"].ToString();
                    }
                    else
                    {
                        @ViewBag.message = "Record Not Found";
                        return View(rm);
                    }

                    if (ModelState.IsValid)
                    {
                        //-------------New SubList---------------//
                        DataSet NewSub = new DataSet();
                        NewSub = objDB.NewCorrectionSubjects(sid, formname, schlcode);  //NewCorrectionSubjects
                        List<SelectListItem> NewSUBList = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in NewSub.Tables[0].Rows)
                        {
                            NewSUBList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });

                        }
                        ViewBag.SubNew = NewSUBList;
                        ViewBag.MediumNew = objCommon.GetMediumAll();
                        ViewBag.SubNewCnt = NewSub.Tables[0].Rows.Count;
                        //-----------End-------------------//                    

                        //----------Old Subject Fill Start----------//
                        DataSet ds = objDB.SearchOldStudent_Subject(sid, formname, schlcode);
                        if (ds == null || ds.Tables[0].Rows.Count == 0)
                        {
                            return RedirectToAction("SRSubjectCorrectionPerforma", "CorrectionSubjects");
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {

                            List<SelectListItem> OLDSUBList = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                            {
                                OLDSUBList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });

                            }
                            ViewBag.SubOLd = OLDSUBList;
                            ViewBag.SubCnt = ds.Tables[0].Rows.Count;
                        }

                        else
                        {

                            return View(rm);
                        }

                    }
                    else
                    {
                        return View(rm);
                    }
                    return View(rm);

                }
                catch (Exception ex)
                {
                    return View(rm);
                }

            }

        }

        [HttpPost]
        public ActionResult SRSubjectCorrectionAdd(RegistrationModels rm, FormCollection frm,string NSQFsubS6)
        {
            ViewBag.YesNoListCP = new AbstractLayer.DBClass().GetYesNo().ToList();
            ViewBag.YesNoListText = new AbstractLayer.DBClass().GetYesNoText().ToList();
            ViewBag.nsfqPatternList = objCommon.GetNsqfPatternList();

            string schlDist = null;
            string id = frm["Std_id"].ToString();
            ViewBag.MyGroup = objCommon.GroupName();
            //-----------------------------------------------------Nsqf -------------------------
            string ses = Session["Session"].ToString();
            string schlcode = Session["SCHL"].ToString();
            DataSet dsnsqf = objDB.CHkNSQF(schlcode, ses);
            if (dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "True" || dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "1")
            {
                ViewData["NSQFSCHOOL"] = "1";
            }
            else
            {
                ViewData["NSQFSCHOOL"] = "0";
            }
            string nsqfsub = null;
            DataSet nsresult = objDB.SelectS11(nsqfsub); // passing Value to DBClass from model
            ViewBag.nsfq = nsresult.Tables[0];
            List<SelectListItem> nsfqList = new List<SelectListItem>();
            //nsfqList.Add(new SelectListItem { Text = "NSFQ Subjects", Value = "0" });
            nsfqList.Add(new SelectListItem { Text = "NO", Value = "NO" });
            foreach (System.Data.DataRow dr in ViewBag.nsfq.Rows)
            {
                nsfqList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["SUB"].ToString() });
            }
            ViewBag.nsfqList = new SelectList(nsfqList, "Value", "Text");
            ViewBag.nsqfcatg = new SelectList(nsfqList, "Value", "Text");

            //-----------------------------------------------------End Nsqf -------------------------

            string session = null;
            string idno = null;
            string schl = null;
            if (Session["Session"] != null)
            {
                session = Session["Session"].ToString();
                idno = Session["IDNO"].ToString();
                schl = Session["SCHL"].ToString();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            //--------------------Start
            ViewBag.vm5 = objCommon.GetGroupMedium();
            ViewBag.vm6 = objCommon.GetGroupMedium();
            ViewBag.vm7 = objCommon.GetGroupMedium();
            ViewBag.sm2 = objCommon.GetGroupMedium();
            ViewBag.sm3 = objCommon.GetGroupMedium();
            ViewBag.sm4 = objCommon.GetGroupMedium();
            ViewBag.Commonmedium = objCommon.GetGroupMedium();
            ViewBag.ccom2 = objCommon.GetGroupMedium();

            ViewBag.hm2 = objCommon.GetMediumAll();
            ViewBag.hm5 = objCommon.GetMediumAll();
            ViewBag.hm6 = objCommon.GetMediumAll();
            ViewBag.hm7 = objCommon.GetMediumAll();
            ViewBag.hm8 = objCommon.GetMediumAll();

            ViewBag.tm2 = objCommon.GetMediumAll();
            ViewBag.tm5 = objCommon.GetMediumAll();
            ViewBag.tm6 = objCommon.GetMediumAll();
            ViewBag.tm7 = objCommon.GetMediumAll();
            ViewBag.tm8 = objCommon.GetMediumAll();

            ViewBag.vm10 = objCommon.GetMediumAll();

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                int status = objDB.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                if (status > 0)
                {
                    if (status == 0)
                    { return RedirectToAction("Index", "Home"); }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

            }
            DataSet scis = objDB.SubjectsTweleve_SCI();
            if (scis == null || scis.Tables[0].Rows.Count == 0)
            { ViewBag.sub = ""; }
            else
            {
                ViewBag.ssub = scis.Tables[0];
                List<SelectListItem> scisub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.ssub.Rows)
                {
                    scisub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                }

                ViewBag.sss8 = new SelectList(scisub, "Value", "Text");


            }
            DataSet hds = objDB.SubjectsTweleve_hum();
            if (hds == null || hds.Tables[0].Rows.Count == 0)
            { ViewBag.sub = ""; }
            else
            {
                ViewBag.hsub = hds.Tables[0];
                List<SelectListItem> humsub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.hsub.Rows)
                {
                    humsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                }
                ViewBag.hs5 = new SelectList(humsub, "Value", "Text");
                ViewBag.hs6 = new SelectList(humsub, "Value", "Text");
                ViewBag.hs7 = new SelectList(humsub, "Value", "Text");
                ViewBag.hs8 = new SelectList(humsub, "Value", "Text");
                ViewBag.vs10 = new SelectList(humsub, "Value", "Text"); //--------humanity Subjects Equals To Vocational Aditional Subjects
            }
            DataSet tecds = objDB.SubjectsTweleve_tech();
            if (tecds == null || tecds.Tables[0].Rows.Count == 0)
            { ViewBag.sub = ""; }
            else
            {
                ViewBag.tsub = tecds.Tables[0];
                List<SelectListItem> tecsub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.tsub.Rows)
                {
                    tecsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                }
                ViewBag.ts5 = new SelectList(tecsub, "Value", "Text");
                ViewBag.ts6 = new SelectList(tecsub, "Value", "Text");
                ViewBag.ts7 = new SelectList(tecsub, "Value", "Text");
                ViewBag.ts8 = new SelectList(tecsub, "Value", "Text");

            }
            DataSet agrds = objDB.SubjectsTweleve_agr();
            if (agrds == null || agrds.Tables[0].Rows.Count == 0)
            { ViewBag.sub = ""; }
            else
            {
                ViewBag.agrsub = agrds.Tables[0];
                List<SelectListItem> agrsub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.agrsub.Rows)
                {
                    agrsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                }

                ViewBag.as5 = new SelectList(agrsub, "Value", "Text");
                ViewBag.as6 = new SelectList(agrsub, "Value", "Text");
                ViewBag.as7 = new SelectList(agrsub, "Value", "Text");
                ViewBag.as8 = new SelectList(agrsub, "Value", "Text");

            }
            DataSet vocds = objDB.Voc_agr();
            if (vocds == null || vocds.Tables[0].Rows.Count == 0)
            { ViewBag.sub = ""; }
            else
            {
                ViewBag.vocsub = vocds.Tables[0];
                List<SelectListItem> vsub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.vocsub.Rows)
                {
                    vsub.Add(new SelectListItem { Text = @dr["group"].ToString(), Value = @dr["group"].ToString() });
                }

                ViewBag.selgroup = new SelectList(vsub, "Value", "Text");

            }
            DataSet vocTrade = objDB.SubjectsTweleve_Voc_All_Trade();
            if (vocTrade == null || vocTrade.Tables[0].Rows.Count == 0)
            { ViewBag.sub = ""; }
            else
            {
                ViewBag.Tsub = vocTrade.Tables[0];
                List<SelectListItem> vocTsub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.Tsub.Rows)
                {
                    vocTsub.Add(new SelectListItem { Text = @dr["tname"].ToString(), Value = @dr["tcode"].ToString() });
                }
                ViewBag.trgroup = new SelectList(vocTsub, "Value", "Text");

            }
            DataSet vocsubjects = objDB.SubjectsTweleve_Voc();
            if (vocsubjects == null || vocsubjects.Tables[0].Rows.Count == 0)
            { ViewBag.sub = ""; }
            else
            {
                ViewBag.vsub = vocsubjects.Tables[0];
                List<SelectListItem> vocsub = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.vsub.Rows)
                {
                    vocsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                }
                ViewBag.vs5 = ViewBag.vos5 = vocsub;
                ViewBag.vs6 = ViewBag.vos6 = vocsub;
                ViewBag.vs7 = ViewBag.vos7 = vocsub;

            }

            List<SelectListItem> MyGroupList = objCommon.GroupName();
            DataTable dtAssignSubject = objCommon.GetAssignSubjectBySchool(Session["SCHL"].ToString());
            if (dtAssignSubject != null && dtAssignSubject.Rows.Count > 0)
            {
                ViewBag.MyGroup = objCommon.GetSubjectsBySchool(dtAssignSubject, MyGroupList);
            }
            ViewBag.MyGroup = MyGroupList;


            //-------------------End----
            //Check server side validation using data annotation
            if (ModelState.IsValid)
            {

                //string stdPic = null;
                string formName = "T2";

                // Start Subject Master
                DataTable dtMatricSubject = new DataTable();
                dtMatricSubject.Columns.Add(new DataColumn("CLASS", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUB", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUBNM", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUBABBR", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("MEDIUM", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUBCAT", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUB_SEQ", typeof(int)));

                dtMatricSubject.Columns.Add(new DataColumn("GROUP", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("TCODE", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("TNAME", typeof(string)));

                DataRow dr = null;
                for (int i = 1; i <= 11; i++)
                {
                    dr = dtMatricSubject.NewRow();
                    dr["CLASS"] = 4;
                    DataSet dsSub = new DataSet();
                    dr["SUBNM"] = "";
                    dr["SUBABBR"] = "";
                    //---------------------------------Science----------------------//
                    if (rm.Tgroup == "SCIENCE")
                    {
                        if (i == 1)
                        {
                            dr["SUB"] = rm.coms1; dr["MEDIUM"] = rm.comm1; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.coms1 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms1.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 2)
                        {
                            if (rm.comm2 != null)
                            {
                                dr["MEDIUM"] = rm.comm2;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.coms2; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.coms2 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms2.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 3)
                        {
                            dr["SUB"] = rm.coms3; dr["MEDIUM"] = rm.comm3; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.coms3 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms3.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 4)
                        {
                            dr["SUB"] = rm.coms4sci; dr["MEDIUM"] = rm.comm4; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.coms4sci != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms4sci.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }

                        else if (i == 5)
                        {
                            dr["SUB"] = rm.ssWEL; dr["MEDIUM"] = rm.smWEL; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.ssWEL != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.ssWEL.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }



                        else if (i == 6)
                        {
                            dr["SUB"] = rm.ss5; dr["MEDIUM"] = rm.sm5; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.ss5 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.ss5.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 7)
                        {
                            dr["SUB"] = rm.ss6; dr["MEDIUM"] = rm.sm6; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.ss6 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.ss6.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 8)
                        {
                            dr["SUB"] = rm.ss7; dr["MEDIUM"] = rm.sm7; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.ss7 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.ss7.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 9)
                        {
                            dr["SUB"] = rm.ss8; dr["MEDIUM"] = rm.sm8; dr["SUB_SEQ"] = 10; dr["SUBCAT"] = "A";
                            if (rm.ss8 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.ss8.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }


                    }
                    //---------------------------------End Science----------------------//
                    //-----------------------------------Commerce------------
                    if (rm.Tgroup == "COMMERCE")
                    {
                        if (i == 1)
                        {
                            dr["SUB"] = rm.coms1; dr["MEDIUM"] = rm.comcm1; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.coms1 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms1.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 2)
                        {
                            if (rm.comcm2 != null)
                            {
                                dr["MEDIUM"] = rm.comcm2;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.comcs2; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.comcs2 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.comcs2.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 3)
                        {
                            dr["SUB"] = rm.coms3; dr["MEDIUM"] = rm.comcm3; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.coms3 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms3.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 4)
                        {
                            dr["SUB"] = rm.coms4comm; dr["MEDIUM"] = rm.comcm4; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.coms4comm != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms4comm.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }

                        else if (i == 5)
                        {
                            dr["SUB"] = rm.ssWEL; dr["MEDIUM"] = rm.smWEL; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.ssWEL != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.ssWEL.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }


                        else if (i == 6)
                        {
                            dr["SUB"] = rm.coms5; dr["MEDIUM"] = rm.comm5; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.coms5 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms5.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 7)
                        {
                            dr["SUB"] = rm.coms6; dr["MEDIUM"] = rm.comm6; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.coms6 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms6.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 8)
                        {
                            dr["SUB"] = rm.coms7; dr["MEDIUM"] = rm.comm7; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.coms7 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms7.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 9)
                        {
                            dr["SUB"] = rm.coms8; dr["MEDIUM"] = rm.comm8; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.coms8 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms8.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 10)
                        {
                            dr["SUB"] = rm.coms9; dr["MEDIUM"] = rm.comm9; dr["SUB_SEQ"] = 10; dr["SUBCAT"] = "A";
                            if (rm.coms9 != null)
                            {
                                if (rm.coms9 == "0")
                                {

                                }
                                else
                                {
                                    dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms9.ToString());
                                    dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                    dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                                }
                            }
                        }

                    }
                    //-----------------------------------End Commerce------------
                    //-----------------------------------Humanity------------
                    if (rm.Tgroup == "HUMANITIES")
                    {
                        if (i == 1)
                        {
                            dr["SUB"] = rm.hums1; dr["MEDIUM"] = rm.humm1; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.hums1 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.hums1.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 2)
                        {
                            if (rm.humm2 != null)
                            {
                                dr["MEDIUM"] = rm.humm2;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.hums2; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.hums2 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.hums2.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 3)
                        {
                            dr["SUB"] = rm.hums3; dr["MEDIUM"] = rm.humm3; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.hums3 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.hums3.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 4)
                        {
                            dr["SUB"] = rm.hums4; dr["MEDIUM"] = rm.humm4; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.hums4 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.hums4.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }

                        else if (i == 5)
                        {
                            dr["SUB"] = rm.ssWEL; dr["MEDIUM"] = rm.smWEL; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.ssWEL != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.ssWEL.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }



                        else if (i == 6)
                        {
                            dr["SUB"] = rm.hums5; dr["MEDIUM"] = rm.humm5; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.hums5 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.hums5.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 7)
                        {
                            dr["SUB"] = rm.hums6; dr["MEDIUM"] = rm.humm6; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.hums6 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.hums6.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 8)
                        {
                            dr["SUB"] = rm.hums7; dr["MEDIUM"] = rm.humm7; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.hums7 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.hums7.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 9)
                        {
                            dr["SUB"] = rm.hums8; dr["MEDIUM"] = rm.humm8; dr["SUB_SEQ"] = 10; dr["SUBCAT"] = "A";
                            if (rm.hums8 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.hums8.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }


                    }
                    //-----------------------------------End Humanity------------
                    //-----------------------------------vocational------------
                    if (rm.Tgroup == "VOCATIONAL")
                    {
                        if (i == 1)
                        {
                            dr["SUB"] = rm.vocs1; dr["MEDIUM"] = rm.vocm1; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "R";
                            if (rm.vocs1 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs1.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();

                            }
                        }
                        else if (i == 2)
                        {
                            if (rm.vocm2 != null)
                            {
                                dr["MEDIUM"] = rm.vocm2;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.vocs2; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "R";
                            if (rm.vocs2 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs2.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();

                                //dr["GROUP"] = dsSub.Tables[0].Rows[0]["GROUP"].ToString();
                                //dr["TCODE"] = dsSub.Tables[0].Rows[0]["TCODE"].ToString();
                                //dr["TNAME"] = dsSub.Tables[0].Rows[0]["TNAME"].ToString();
                            }
                        }
                        else if (i == 3)
                        {
                            dr["SUB"] = rm.vocs3; dr["MEDIUM"] = rm.vocm3; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "G";
                            if (rm.vocs3 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs3.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                                //dr["GROUP"] = dsSub.Tables[0].Rows[0]["GROUP"].ToString();
                                //dr["TCODE"] = dsSub.Tables[0].Rows[0]["TCODE"].ToString();
                                //dr["TNAME"] = dsSub.Tables[0].Rows[0]["TNAME"].ToString();
                            }
                        }
                        else if (i == 4)
                        {
                            dr["SUB"] = rm.vocs4; dr["MEDIUM"] = rm.vocm4; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "G";
                            if (rm.vocs4 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs4.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 5)
                        {
                            dr["SUB"] = rm.ssWEL; dr["MEDIUM"] = rm.smWEL; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.ssWEL != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.ssWEL.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }


                        else if (i == 6)
                        {
                            dr["SUB"] = rm.vocs5; dr["MEDIUM"] = rm.vocm5; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "R";
                            if (rm.vocs5 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs5.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 7)
                        {
                            dr["SUB"] = rm.vocs6; dr["MEDIUM"] = rm.vocm6; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "R";
                            if (rm.vocs6 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs6.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 8)
                        {
                            dr["SUB"] = rm.vocs7; dr["MEDIUM"] = rm.vocm7; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "R";
                            if (rm.vocs7 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs7.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 9)
                        {
                            dr["SUB"] = rm.vocs8; dr["MEDIUM"] = rm.vocm8; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "A";
                            if (rm.vocs8 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs8.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 10)
                        {
                            dr["SUB"] = rm.vocs9; dr["MEDIUM"] = rm.vocm9; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "G";
                            if (rm.vocs9 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs9.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 11)
                        {
                            if (rm.vocm10 != null)
                            {
                                dr["MEDIUM"] = rm.vocm10;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.vocs10; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "G";
                            if (rm.vocs10 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs10.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }


                    }
                    //-----------------------------------End Vocational------------

                    //-----------------------------------technical------------
                    if (rm.Tgroup == "TECHNICAL")
                    {
                        if (i == 1)
                        {
                            dr["SUB"] = rm.tecs1; dr["MEDIUM"] = rm.tecm1; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.tecs1 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.tecs1.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 2)
                        {
                            if (rm.tecm2 != null)
                            {
                                dr["MEDIUM"] = rm.tecm2;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.tecs2; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.tecs2 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.tecs2.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 3)
                        {
                            dr["SUB"] = rm.tecs3; dr["MEDIUM"] = rm.tecm3; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.tecs3 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.tecs3.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 4)
                        {
                            dr["SUB"] = rm.tecs4; dr["MEDIUM"] = rm.tecm4; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.tecs4 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.tecs4.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 5)
                        {
                            dr["SUB"] = rm.ssWEL; dr["MEDIUM"] = rm.smWEL; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.ssWEL != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.ssWEL.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }

                        else if (i == 6)
                        {
                            dr["SUB"] = rm.tecs5; dr["MEDIUM"] = rm.tecm5; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.tecs5 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.tecs5.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 7)
                        {
                            dr["SUB"] = rm.tecs6; dr["MEDIUM"] = rm.tecm6; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.tecs6 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.tecs6.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 8)
                        {
                            dr["SUB"] = rm.tecs7; dr["MEDIUM"] = rm.tecm7; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.tecs7 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.tecs7.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 9)
                        {
                            dr["SUB"] = rm.tecs8; dr["MEDIUM"] = rm.tecm8; dr["SUB_SEQ"] = 10; dr["SUBCAT"] = "A";
                            if (rm.tecs8 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.tecs8.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }

                    }
                    //-----------------------------------End technical------------

                    //-----------------------------------Agriculture------------
                    if (rm.Tgroup == "AGRICULTURE")
                    {
                        if (i == 1)
                        {
                            dr["SUB"] = rm.agrs1; dr["MEDIUM"] = rm.agrm1; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.agrs1 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.agrs1.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 2)
                        {
                            if (rm.agrm2 != null)
                            {
                                dr["MEDIUM"] = rm.agrm2;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.agrs2; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.agrs2 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.agrs2.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 3)
                        {
                            dr["SUB"] = rm.agrs3; dr["MEDIUM"] = rm.agrm3; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.agrs3 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.agrs3.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 4)
                        {
                            dr["SUB"] = rm.agrs4; dr["MEDIUM"] = rm.agrm4; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.agrs4 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.agrs4.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 5)
                        {
                            dr["SUB"] = rm.ssWEL; dr["MEDIUM"] = rm.smWEL; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.ssWEL != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.ssWEL.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }

                        else if (i == 6)
                        {
                            dr["SUB"] = rm.agrs5; dr["MEDIUM"] = rm.agrm5; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.agrs5 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.agrs5.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 7)
                        {
                            dr["SUB"] = rm.agrs6; dr["MEDIUM"] = rm.agrm6; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.agrs6 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.agrs6.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 8)
                        {
                            dr["SUB"] = rm.agrs7; dr["MEDIUM"] = rm.agrm7; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.agrs7 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.agrs7.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 9)
                        {
                            dr["SUB"] = rm.agrs8; dr["MEDIUM"] = rm.agrm8; dr["SUB_SEQ"] = 10; dr["SUBCAT"] = "A";
                            if (rm.agrs8 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.agrs8.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }

                    }
                    //-----------------------------------End Agriculture------------

                    dtMatricSubject.Rows.Add(dr);

                    //    if (rm.Tgroup == "HUMANITIES")
                    //    {
                    //        if (i == 6)
                    //        {
                    //            dr = dtMatricSubject.NewRow();
                    //            dr["CLASS"] = 4;
                    //            //DataSet dsSub = new DataSet();
                    //            dr["SUBNM"] = "";
                    //            dr["SUBABBR"] = "";
                    //            dr["MEDIUM"] = "MEDIUM";
                    //            if (rm.NsqfsubS6 != "NO" && rm.NsqfsubS6 != null && rm.NsqfsubS6 != "")
                    //            {
                    //                dr["SUB"] = "205"; dr["SUB_SEQ"] = 12; dr["SUBCAT"] = "R";
                    //                if (dr["SUB"] != null)
                    //                {
                    //                    dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(dr["SUB"].ToString());
                    //                    dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                    //                    dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                    //                }
                    //            }
                    //            //// dtMatricSubject.Rows.Add(dr); HOT (85,205) subject droped by PSEB , So don't insert HOT (85,205) if NSQF Selected (M1,M2,T1,T2)
                    //        }
                    //    }

                }

                dtMatricSubject.AcceptChanges();
                dtMatricSubject = dtMatricSubject.AsEnumerable().Where(r => r.ItemArray[1].ToString() != "" && r.ItemArray[1].ToString() != "0").CopyToDataTable();

                var duplicates = dtMatricSubject.AsEnumerable().GroupBy(r => r[2]).Where(gr => gr.Count() > 1).ToList();
                if (duplicates.Any())
                {
                    TempData["Duplicate"] = ViewBag.Duplicate = "Duplicate Subjects: " + String.Join(", ", duplicates.Select(dupl => dupl.Key));
                    TempData["resultUpdate"] = 10;
                    return RedirectToAction("SRSubjectCorrectionPerforma", "CorrectionSubjects");

                }

                var DTNoOfSub = dtMatricSubject.AsEnumerable().Where(r => r.Field<string>("SUB") != "72" && r.Field<string>("SUB") != "73" && r.Field<string>("SUB") != "205").ToList();

                int NoOfSub = DTNoOfSub.Count();
                if (rm.DA == "N.A.")
                {
                    if (NoOfSub < 8)
                    {
                        TempData["resultUpdate"] = 15;
                        return RedirectToAction("SRSubjectCorrectionPerforma", "CorrectionSubjects");
                    }
                }
                else
                {
                    if (NoOfSub < 7)
                    {
                        TempData["resultUpdate"] = 15;
                        return RedirectToAction("SRSubjectCorrectionPerforma", "CorrectionSubjects");
                    }
                }



                string msg = "";
                bool status = objDB.SubjectNotAllowed(rm.Tgroup, dtMatricSubject, out msg);
                if (status == false)
                {
                    TempData["resultUpdate"] = 20;
                    TempData["errorMsg"] = msg;
                    return RedirectToAction("SRSubjectCorrectionPerforma", "CorrectionSubjects");
                }


                // Check NSQF Subject exists in Subject List after NSQF Current Class Selection 
                if (!string.IsNullOrEmpty(NSQFsubS6))
                {
                    if (NSQFsubS6.ToUpper() != "NO".ToUpper())
                    {
                        rm.NsqfsubS6 = NSQFsubS6;
                        var NSQFSubExists = dtMatricSubject.AsEnumerable().Where(r => r.Field<string>("SUB") == NSQFsubS6).Count() > 0;
                        if (!NSQFSubExists)
                        {
                            TempData["resultUpdate"] = "NSQFSUBWANT";
                            return RedirectToAction("SRSubjectCorrectionPerforma", "CorrectionSubjects");
                        }

                    }
                }


                string result = objDB.Senior_Subject_Correction(rm, frm, id, dtMatricSubject);

                ModelState.Clear();
                //--For Showing Message---------//
                //ViewData["resultUpdate"] = result;
                TempData["resultUpdate"] = result;
                return RedirectToAction("SRSubjectCorrectionPerforma", "CorrectionSubjects");
            }
            return View(rm);

        }

        public ActionResult SRFinalSubmitCorrection(FormCollection frm)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            RegistrationModels rm = new RegistrationModels();
            try
            {
                if (ModelState.IsValid)
                {

                    if (Session["SCHL"] != null)
                    {

                        rm.SCHL = Session["SCHL"].ToString();
                        rm.Correctiontype = "01";
                        string Class = "4";
                        string resultFS = objDB.FinalSubmitSubjectCorrection(rm, Class); // passing Value to DBClass from model
                        if (Convert.ToInt16(resultFS) > 0)
                        {
                            ViewData["resultFS"] = resultFS;
                            //return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects");
                            return RedirectToAction("SchoolCorrectionFinalPrintLst", "RegistrationPortal");
                        }
                        else
                        {
                            ViewData["resultFS"] = "";
                            return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Logout", "Login");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Login");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login");
            }
        }
        #endregion Senior Subject Correction

        //--------------------------End Senior Subject Correction---------------------//

        //-------------------------Stream Correction Performa---------------------------//
        #region Stream Correction
        public ActionResult StreamCorrectionPerforma(RegistrationModels rm)
        {
            try
            {
                //ViewBag.SelectedItem
                AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                //RegistrationModels rm = new RegistrationModels();
                //var itemsch = new SelectList(new[] { new { ID = "3", Name = "Sr.Secondary Regular" }, }, "ID", "Name", 3);
                //ViewBag.MySch = itemsch.ToList();
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    string schlid = Session["SCHL"].ToString();
                    DataSet result = objDB.schooltypesCorrection(schlid, "S"); // passing Value to DBClass from model
                    if (result == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    if (result.Tables[1].Rows.Count > 0)
                    {
                        ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                        ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
                        ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                        ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
                        ViewBag.E1T1threclock = result.Tables[1].Rows[0]["Eth"].ToString();

                        DateTime sDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["sDate"]);
                        DateTime eDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["eDate"]);

                        DateTime sDateE = Convert.ToDateTime(result.Tables[6].Rows[2]["sDate"]);
                        DateTime eDateE = Convert.ToDateTime(result.Tables[6].Rows[2]["eDate"]);

                        DateTime sDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["sDate"]);
                        DateTime eDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["eDate"]);

                        DateTime sDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["sDate"]);
                        DateTime eDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["eDate"]);

                        DateTime sDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["sDate"]);
                        DateTime eDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["eDate"]);

                        DateTime dtTodate = Convert.ToDateTime(DateTime.Today);

                        List<SelectListItem> itemsch = new List<SelectListItem>();
                        //if (ViewBag.Matric == "1" && sDateM <= eDateM)
                        //{
                        //    itemsch.Add(new SelectListItem { Text = "Matriculation Regular", Value = "1" });
                        //}
                        //if (ViewBag.OMatric == "1" && sDateMO <= eDateMO)
                        //{
                        //    itemsch.Add(new SelectListItem { Text = "Matriculation Open", Value = "2" });
                        //}
                        if (ViewBag.E1T1threclock == "1" && dtTodate <= eDateE)
                        {
                            itemsch.Add(new SelectListItem { Text = "11th Class", Value = "6" });
                        }
                        if (ViewBag.Senior == "1" && dtTodate <= eDateT)
                        {
                            itemsch.Add(new SelectListItem { Text = "Sr.Secondary Regular", Value = "3" });
                        }
                        if (ViewBag.OSenior == "1" && dtTodate <= eDateTO)
                        {
                            itemsch.Add(new SelectListItem { Text = "Sr.Secondary Open", Value = "4" });
                        }
                        if (ViewBag.Matric != "1" && ViewBag.OMatric != "1" && ViewBag.Senior != "1" && ViewBag.OSenior != "1")
                        {
                            itemsch.Add(new SelectListItem { Text = "", Value = "" });
                        }
                        ViewBag.MySch = itemsch.ToList();

                    }
                    //DataSet seleLastCan = objDB.SelectChangesStreams(schlid);
                    DataSet seleLastCan = objDB.PendingCorrectionStreams(schlid);
                    if (seleLastCan.Tables[0].Rows.Count > 0)
                    {

                        @ViewBag.message = "1";
                        rm.StoreAllData = seleLastCan;
                        ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                    }
                    else
                    {
                        @ViewBag.message = "Record Not Found";
                    }
                    ViewBag.MediumNew = objCommon.GetMediumAll();
                    //-------------Fill Stream Allowed To Schoool---------------//

                    ViewBag.vm5 = objCommon.GetGroupMedium();
                    ViewBag.vm6 = objCommon.GetGroupMedium();
                    ViewBag.vm7 = objCommon.GetGroupMedium();
                    if (Session["SCHL"] == null)
                    {
                        return RedirectToAction("Logout", "Login");
                    }
                    else
                    {
                        string admdate = "";
                        objDB.CheckDateE1E2T1T2(Session["SCHL"].ToString(), out admdate);
                        ViewBag.admdate = admdate;
                        //if (status > 0)
                        //{
                        //    if (status == 0)
                        //    { return RedirectToAction("Index", "Home"); }
                        //}
                        //else
                        //{
                        //    return RedirectToAction("Index", "Home");
                        //}
                    }

                    DataSet scis = objDB.SubjectsTweleve_SCI();
                    if (scis == null || scis.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.ssub = scis.Tables[0];
                        List<SelectListItem> scisub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.ssub.Rows)
                        {
                            scisub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }

                        ViewBag.sss8 = new SelectList(scisub, "Value", "Text");


                    }
                    DataSet hds = objDB.SubjectsTweleve_hum();
                    if (hds == null || hds.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.hsub = hds.Tables[0];
                        List<SelectListItem> humsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.hsub.Rows)
                        {
                            humsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }
                        ViewBag.hs5 = new SelectList(humsub, "Value", "Text");
                        ViewBag.hs6 = new SelectList(humsub, "Value", "Text");
                        ViewBag.hs7 = new SelectList(humsub, "Value", "Text");
                        ViewBag.hs8 = new SelectList(humsub, "Value", "Text");
                        ViewBag.vs10 = new SelectList(humsub, "Value", "Text"); //--------humanity Subjects Equals To Vocational Aditional Subjects
                    }
                    DataSet tecds = objDB.SubjectsTweleve_tech();
                    if (tecds == null || tecds.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.tsub = tecds.Tables[0];
                        List<SelectListItem> tecsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.tsub.Rows)
                        {
                            tecsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }
                        ViewBag.ts5 = new SelectList(tecsub, "Value", "Text");
                        ViewBag.ts6 = new SelectList(tecsub, "Value", "Text");
                        ViewBag.ts7 = new SelectList(tecsub, "Value", "Text");
                        ViewBag.ts8 = new SelectList(tecsub, "Value", "Text");

                    }
                    DataSet agrds = objDB.SubjectsTweleve_agr();
                    if (agrds == null || agrds.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.agrsub = agrds.Tables[0];
                        List<SelectListItem> agrsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.agrsub.Rows)
                        {
                            agrsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }

                        ViewBag.as5 = new SelectList(agrsub, "Value", "Text");
                        ViewBag.as6 = new SelectList(agrsub, "Value", "Text");
                        ViewBag.as7 = new SelectList(agrsub, "Value", "Text");
                        ViewBag.as8 = new SelectList(agrsub, "Value", "Text");

                    }
                    DataSet vocds = objDB.Voc_agr();
                    if (vocds == null || vocds.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.vocsub = vocds.Tables[0];
                        List<SelectListItem> vsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.vocsub.Rows)
                        {
                            vsub.Add(new SelectListItem { Text = @dr["group"].ToString(), Value = @dr["group"].ToString() });
                        }

                        ViewBag.selgroup = new SelectList(vsub, "Value", "Text");

                    }

                    List<SelectListItem> MyGroupList = objCommon.GroupName();
                    DataTable dtAssignSubject = objCommon.GetAssignSubjectBySchool(schlid);
                    if (dtAssignSubject != null && dtAssignSubject.Rows.Count > 0)
                    {
                        ViewBag.StreamNew = objCommon.GetSubjectsBySchool(dtAssignSubject, MyGroupList);
                    }
                    ViewBag.StreamNew = MyGroupList;
                    //-----------End-------------------//   
                    //----------------Subject voc Group-----------
                    if (rm.Tgroup == "VOCATIONAL")
                    {
                        DataSet gp = objDB.selectGP(rm.Std_id);
                        if (gp == null || gp.Tables[0].Rows.Count == 0)
                        {
                            //return RedirectToAction("T1Formgrid", "RegistrationPortal");
                        }
                        else
                        {
                            rm.groupsel = gp.Tables[0].Rows[0]["Group"].ToString().Trim();
                            string gsel = gp.Tables[0].Rows[0]["Group"].ToString().Trim();
                            rm.grouptr = gp.Tables[0].Rows[0]["TCODE"].ToString().Trim();
                            DataSet vocTRgroup = objDB.Voc_Trgroup(gsel);
                            if (vocds == null || vocds.Tables[0].Rows.Count == 0)
                            { ViewBag.sub = ""; }
                            else
                            {
                                ViewBag.vocTR = vocTRgroup.Tables[0];
                                List<SelectListItem> vtrsub = new List<SelectListItem>();
                                foreach (System.Data.DataRow dr in ViewBag.vocTR.Rows)
                                {
                                    vtrsub.Add(new SelectListItem { Text = @dr["TNAME"].ToString(), Value = @dr["TCODE"].ToString() });
                                }

                                ViewBag.trgroup = new SelectList(vtrsub, "Value", "Text");

                            }
                        }
                    }
                    //-----------------End---------------                
                }

                //var itemsch = new SelectList(new[]{new {ID="1",Name="Matriculation Regular"},new{ID="2",Name="Matriculation Open"},
                //new{ID="3",Name="Sr.Secondary Regular"},new{ID="4",Name="Sr.Secondary Open"},}, "ID", "Name", 1);
                //ViewBag.MySch = itemsch.ToList();


                if (ModelState.IsValid)
                {
                    return View(rm);
                }
                else
                { return View(rm); }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }
        }
        [HttpPost]
        public ActionResult StreamCorrectionPerforma(RegistrationModels rm, FormCollection frm, string cmd)
        {

            string sid = Convert.ToString(rm.Std_id);
            string formname = null;
            string ORT = null;

            if (rm.SelList == "1")
            {
                formname = "M";
                ORT = "Matriculation Regular";
            }
            if (rm.SelList == "2")
            {
                formname = "MO";
                ORT = "Matriculation Open";
            }
            if (rm.SelList == "3")
            {
                formname = "T";
                ORT = "Sr.Secondary Regular";
            }
            if (rm.SelList == "4")
            {
                formname = "TO";
                ORT = "Sr.Secondary Open";
            }
            if (rm.SelList == "6")
            {
                formname = "E";
                ORT = "11th Class";
            }


            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            string schlcode = Session["SCHL"].ToString();

            DataSet result1 = objDB.schooltypesCorrection(schlcode, "S"); // passing Value to DBClass from model
            if (result1 == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (result1.Tables[1].Rows.Count > 0)
            {

                ViewBag.Matric = result1.Tables[1].Rows[0]["Matric"].ToString();
                ViewBag.OMatric = result1.Tables[1].Rows[0]["OMatric"].ToString();
                ViewBag.Senior = result1.Tables[1].Rows[0]["Senior"].ToString();
                ViewBag.OSenior = result1.Tables[1].Rows[0]["OSenior"].ToString();
                ViewBag.E1T1threclock = result1.Tables[1].Rows[0]["Eth"].ToString();

                DateTime sDateM = Convert.ToDateTime(result1.Tables[6].Rows[1]["sDate"]);
                DateTime eDateM = Convert.ToDateTime(result1.Tables[6].Rows[1]["eDate"]);

                DateTime sDateE = Convert.ToDateTime(result1.Tables[6].Rows[2]["sDate"]);
                DateTime eDateE = Convert.ToDateTime(result1.Tables[6].Rows[2]["eDate"]);

                DateTime sDateT = Convert.ToDateTime(result1.Tables[6].Rows[3]["sDate"]);
                DateTime eDateT = Convert.ToDateTime(result1.Tables[6].Rows[3]["eDate"]);

                DateTime sDateMO = Convert.ToDateTime(result1.Tables[6].Rows[4]["sDate"]);
                DateTime eDateMO = Convert.ToDateTime(result1.Tables[6].Rows[4]["eDate"]);

                DateTime sDateTO = Convert.ToDateTime(result1.Tables[6].Rows[5]["sDate"]);
                DateTime eDateTO = Convert.ToDateTime(result1.Tables[6].Rows[5]["eDate"]);

                List<SelectListItem> itemsch = new List<SelectListItem>();
                //if (ViewBag.Matric == "1" && sDateM <= eDateM)
                //{
                //    itemsch.Add(new SelectListItem { Text = "Matriculation Regular", Value = "1" });
                //}
                //if (ViewBag.OMatric == "1" && sDateMO <= eDateMO)
                //{
                //    itemsch.Add(new SelectListItem { Text = "Matriculation Open", Value = "2" });
                //}
                if (ViewBag.E1T1threclock == "1" && sDateE <= eDateE)
                {
                    itemsch.Add(new SelectListItem { Text = "11th Class", Value = "6" });
                }
                if (ViewBag.Senior == "1" && sDateT <= eDateT)
                {
                    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Regular", Value = "3" });
                }
                if (ViewBag.OSenior == "1" && sDateTO <= eDateTO)
                {
                    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Open", Value = "4" });
                }
                if (ViewBag.Matric != "1" && ViewBag.OMatric != "1" && ViewBag.Senior != "1" && ViewBag.OSenior != "1")
                {
                    itemsch.Add(new SelectListItem { Text = "", Value = "" });
                }
                ViewBag.MySch = itemsch.ToList();
            }
            if (cmd == "View All Correction Pending Record")
            {
                //var itemsch = new SelectList(new[] { new { ID = "3", Name = "Sr.Secondary Regular" }, }, "ID", "Name", 3);
                //ViewBag.MySch = itemsch.ToList();

                AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                DataSet seleLastCan = objDB.PendingCorrectionStreams(schlcode);
                if (seleLastCan.Tables[0].Rows.Count > 0)
                {

                    @ViewBag.message = "1";
                    rm.StoreAllData = seleLastCan;
                    //ViewBag.TotalViewAllCount = seleLastCan.Tables[0].Rows.Count;
                    ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                }
                else
                {
                    @ViewBag.message = "Record Not Found";
                }


                return View(rm);
            }
            if (cmd == "View All Correction Record")
            {
                //var itemsch = new SelectList(new[] { new { ID = "3", Name = "Sr.Secondary Regular" }, }, "ID", "Name", 3);
                //ViewBag.MySch = itemsch.ToList();

                AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                DataSet seleLastCan = objDB.ViewAllCorrectionStreams(schlcode);
                if (seleLastCan.Tables[0].Rows.Count > 0)
                {

                    @ViewBag.message = "1";
                    rm.StoreAllData = seleLastCan;
                    ViewBag.TotalViewAllCount = seleLastCan.Tables[0].Rows.Count;
                    ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                }
                else
                {
                    @ViewBag.message = "Record Not Found";
                }

                return View(rm);
            }
            if (cmd == "Save Subject Details")
            {
                //    var itemsch = new SelectList(new[]{new {ID="1",Name="Matriculation Regular"},new{ID="2",Name="Matriculation Open"},
                //new{ID="3",Name="Sr.Secondary Regular"},new{ID="4",Name="Sr.Secondary Open"},}, "ID", "Name", 1);
                //    ViewBag.MySch = itemsch.ToList();

                //var itemsch = new SelectList(new[] { new { ID = "3", Name = "Sr.Secondary Regular" }, }, "ID", "Name", 3);
                //ViewBag.MySch = itemsch.ToList();

                AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB(); //calling class DBClass

                //DataSet seleLastCan = objDB.ViewAllCorrectionSubjects(schlcode);
                //if (seleLastCan.Tables[0].Rows.Count > 0)
                //{

                //    @ViewBag.message = "1";
                //    rm.StoreAllData = seleLastCan;
                //    ViewBag.TotalViewAllCount = seleLastCan.Tables[0].Rows.Count;
                //    ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                //}
                //else
                //{
                //    @ViewBag.message = "Record Not Found";
                //}

                //-------------------------Subject Start----------------
                // Start Subject Master
                DataTable dtMatricSubject = new DataTable();
                dtMatricSubject.Columns.Add(new DataColumn("CLASS", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUB", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUBNM", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUBABBR", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("MEDIUM", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUBCAT", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUB_SEQ", typeof(int)));

                dtMatricSubject.Columns.Add(new DataColumn("GROUP", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("TCODE", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("TNAME", typeof(string)));

                DataRow dr = null;
                #region Start Subject Combination
                for (int i = 1; i <= 10; i++)
                {
                    dr = dtMatricSubject.NewRow();
                    dr["CLASS"] = 3;
                    DataSet dsSub = new DataSet();
                    dr["SUBNM"] = "";
                    dr["SUBABBR"] = "";


                    dr["GROUP"] = "";
                    dr["TCODE"] = "";
                    dr["TNAME"] = "";


                    //---------------------------------Science----------------------//
                    if (rm.Tgroup == "SCIENCE")
                    {
                        if (i == 1)
                        {
                            dr["SUB"] = rm.coms1; dr["MEDIUM"] = rm.comm1; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.coms1 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms1.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 2)
                        {
                            dr["SUB"] = rm.coms2; dr["MEDIUM"] = rm.comm2; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.coms2 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms2.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 3)
                        {
                            dr["SUB"] = rm.coms3; dr["MEDIUM"] = rm.comm3; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.coms3 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms3.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 4)
                        {
                            dr["SUB"] = rm.coms4; dr["MEDIUM"] = rm.comm4; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.coms4 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms4.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 5)
                        {
                            dr["SUB"] = rm.ss5; dr["MEDIUM"] = rm.sm5; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.ss5 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.ss5.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 6)
                        {
                            dr["SUB"] = rm.ss6; dr["MEDIUM"] = rm.sm6; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.ss6 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.ss6.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 7)
                        {
                            dr["SUB"] = rm.ss7; dr["MEDIUM"] = rm.sm7; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.ss7 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.ss7.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 8)
                        {
                            dr["SUB"] = rm.ss8; dr["MEDIUM"] = rm.sm8; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "A";
                            if (rm.ss8 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.ss8.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }


                    }
                    //---------------------------------End Science----------------------//
                    //-----------------------------------Commerce------------
                    if (rm.Tgroup == "COMMERCE")
                    {
                        if (i == 1)
                        {
                            dr["SUB"] = rm.coms1; dr["MEDIUM"] = rm.comcm1; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.coms1 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms1.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 2)
                        {
                            dr["SUB"] = rm.comcs2; dr["MEDIUM"] = rm.comcm2; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.comcs2 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.comcs2.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 3)
                        {
                            dr["SUB"] = rm.coms3; dr["MEDIUM"] = rm.comcm3; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.coms3 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms3.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 4)
                        {
                            dr["SUB"] = rm.coms4; dr["MEDIUM"] = rm.comcm4; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.coms4 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms4.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 5)
                        {
                            dr["SUB"] = rm.coms5; dr["MEDIUM"] = rm.comm5; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.coms5 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms5.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 6)
                        {
                            dr["SUB"] = rm.coms6; dr["MEDIUM"] = rm.comm6; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.coms6 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms6.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 7)
                        {
                            dr["SUB"] = rm.coms7; dr["MEDIUM"] = rm.comm7; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.coms7 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms7.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 8)
                        {
                            dr["SUB"] = rm.coms8; dr["MEDIUM"] = rm.comm8; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.coms8 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms8.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 9)
                        {
                            dr["SUB"] = rm.coms9; dr["MEDIUM"] = rm.comm9; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "A";
                            if (rm.coms9 != null)
                            {
                                if (rm.coms9 == "0")
                                {

                                }
                                else
                                {
                                    dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.coms9.ToString());
                                    dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                    dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                                }

                            }
                        }

                    }
                    //-----------------------------------End Commerce------------
                    //-----------------------------------Humanity------------
                    if (rm.Tgroup == "HUMANITIES" && ORT != "11th Class")
                    {
                        if (i == 1)
                        {
                            dr["SUB"] = rm.hums1; dr["MEDIUM"] = rm.humm1; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.hums1 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.hums1.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 2)
                        {
                            dr["SUB"] = rm.hums2; dr["MEDIUM"] = rm.humm2; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.hums2 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.hums2.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 3)
                        {
                            dr["SUB"] = rm.hums3; dr["MEDIUM"] = rm.humm3; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.hums3 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.hums3.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 4)
                        {
                            dr["SUB"] = rm.hums4; dr["MEDIUM"] = rm.humm4; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.hums4 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.hums4.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 5)
                        {
                            dr["SUB"] = rm.hums5; dr["MEDIUM"] = rm.humm5; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.hums5 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.hums5.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 6)
                        {
                            dr["SUB"] = rm.hums6; dr["MEDIUM"] = rm.humm6; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.hums6 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.hums6.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 7)
                        {
                            dr["SUB"] = rm.hums7; dr["MEDIUM"] = rm.humm7; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.hums7 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.hums7.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 8)
                        {
                            dr["SUB"] = rm.hums8; dr["MEDIUM"] = rm.humm8; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "A";
                            if (rm.hums8 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.hums8.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }


                    }
                    //-----------------------------------End Humanity------------
                    //-----------------------------------vocational------------
                    if (rm.Tgroup == "VOCATIONAL")
                    {
                        if (i == 1)
                        {
                            dr["SUB"] = rm.vocs1; dr["MEDIUM"] = rm.vocm1; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "R";
                            if (rm.vocs1 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs1.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();

                            }
                        }
                        else if (i == 2)
                        {
                            dr["SUB"] = rm.vocs2; dr["MEDIUM"] = rm.vocm2; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "R";
                            if (rm.vocs2 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs2.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();

                                //dr["GROUP"] = dsSub.Tables[0].Rows[0]["GROUP"].ToString();
                                //dr["TCODE"] = dsSub.Tables[0].Rows[0]["TCODE"].ToString();
                                //dr["TNAME"] = dsSub.Tables[0].Rows[0]["TNAME"].ToString();
                            }
                        }
                        else if (i == 3)
                        {
                            dr["SUB"] = rm.vocs3; dr["MEDIUM"] = rm.vocm3; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "G";
                            if (rm.vocs3 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs3.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                                //dr["GROUP"] = dsSub.Tables[0].Rows[0]["GROUP"].ToString();
                                //dr["TCODE"] = dsSub.Tables[0].Rows[0]["TCODE"].ToString();
                                //dr["TNAME"] = dsSub.Tables[0].Rows[0]["TNAME"].ToString();
                            }
                        }
                        else if (i == 4)
                        {
                            dr["SUB"] = rm.vocs4; dr["MEDIUM"] = rm.vocm4; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "G";
                            if (rm.vocs4 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs4.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 5)
                        {
                            dr["SUB"] = rm.vocs5; dr["MEDIUM"] = rm.vocm5; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "R";
                            if (rm.vocs5 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs5.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 6)
                        {
                            dr["SUB"] = rm.vocs6; dr["MEDIUM"] = rm.vocm6; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "R";
                            if (rm.vocs6 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs6.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 7)
                        {
                            dr["SUB"] = rm.vocs7; dr["MEDIUM"] = rm.vocm7; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "R";
                            if (rm.vocs7 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs7.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 8)
                        {
                            dr["SUB"] = rm.vocs8; dr["MEDIUM"] = rm.vocm8; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "R";
                            if (rm.vocs8 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs8.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 9)
                        {
                            dr["SUB"] = rm.vocs9; dr["MEDIUM"] = rm.vocm9; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "G";
                            if (rm.vocs9 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs9.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 10)
                        {
                            dr["SUB"] = rm.vocs10; dr["MEDIUM"] = rm.vocm10; dr["SUB_SEQ"] = i; dr["GROUP"] = rm.groupsel; dr["TCODE"] = rm.TCODE; dr["TNAME"] = rm.grouptr; dr["SUBCAT"] = "A";
                            if (rm.vocs10 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.vocs10.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }

                    }
                    //-----------------------------------End Vocational------------

                    //-----------------------------------technical------------
                    if (rm.Tgroup == "TECHNICAL")
                    {
                        if (i == 1)
                        {
                            dr["SUB"] = rm.tecs1; dr["MEDIUM"] = rm.tecm1; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.tecs1 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.tecs1.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 2)
                        {
                            dr["SUB"] = rm.tecs2; dr["MEDIUM"] = rm.tecm2; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.tecs2 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.tecs2.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 3)
                        {
                            dr["SUB"] = rm.tecs3; dr["MEDIUM"] = rm.tecm3; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.tecs3 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.tecs3.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 4)
                        {
                            dr["SUB"] = rm.tecs4; dr["MEDIUM"] = rm.tecm4; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.tecs4 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.tecs4.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 5)
                        {
                            dr["SUB"] = rm.tecs5; dr["MEDIUM"] = rm.tecm5; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.tecs5 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.tecs5.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 6)
                        {
                            dr["SUB"] = rm.tecs6; dr["MEDIUM"] = rm.tecm6; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.tecs6 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.tecs6.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 7)
                        {
                            dr["SUB"] = rm.tecs7; dr["MEDIUM"] = rm.tecm7; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.tecs7 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.tecs7.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 8)
                        {
                            dr["SUB"] = rm.tecs8; dr["MEDIUM"] = rm.tecm8; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "A";
                            if (rm.tecs8 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.tecs8.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }

                    }
                    //-----------------------------------End technical------------

                    //-----------------------------------Agriculture------------
                    if (rm.Tgroup == "AGRICULTURE")
                    {
                        if (i == 1)
                        {
                            dr["SUB"] = rm.agrs1; dr["MEDIUM"] = rm.agrm1; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.agrs1 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.agrs1.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 2)
                        {
                            dr["SUB"] = rm.agrs2; dr["MEDIUM"] = rm.agrm2; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.agrs2 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.agrs2.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 3)
                        {
                            dr["SUB"] = rm.agrs3; dr["MEDIUM"] = rm.agrm3; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.agrs3 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.agrs3.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 4)
                        {
                            dr["SUB"] = rm.agrs4; dr["MEDIUM"] = rm.agrm4; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.agrs4 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.agrs4.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 5)
                        {
                            dr["SUB"] = rm.agrs5; dr["MEDIUM"] = rm.agrm5; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.agrs5 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.agrs5.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 6)
                        {
                            dr["SUB"] = rm.agrs6; dr["MEDIUM"] = rm.agrm6; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.agrs6 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.agrs6.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 7)
                        {
                            dr["SUB"] = rm.agrs7; dr["MEDIUM"] = rm.agrm7; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.agrs7 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.agrs7.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 8)
                        {
                            dr["SUB"] = rm.agrs8; dr["MEDIUM"] = rm.agrm8; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "A";
                            if (rm.agrs8 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySubFromSSE(rm.agrs8.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }

                    }
                    //-----------------------------------End Agriculture------------
                    #endregion End Subject Combination

                    dr["MEDIUM"] = "MEDIUM";
                    dtMatricSubject.Rows.Add(dr);

                }

                dtMatricSubject.AcceptChanges();
                if (ORT.ToUpper() == "11TH CLASS")
                {
                    dtMatricSubject.Rows.Clear();
                }
                else
                {
                    dtMatricSubject = dtMatricSubject.AsEnumerable().Where(r => r.ItemArray[1].ToString() != "" && r.ItemArray[1].ToString() != "0").CopyToDataTable();
                }

                int ClassForCorr = 4;

                // End Subject Master
                //string FormType,string Std_id, string schl, DataTable dtMatricSubject
                string result = objDB.Ins_Correction_Senior_Data(rm.SelList, rm.Stream, rm.Tgroup, ClassForCorr, ORT, sid, schlcode, dtMatricSubject);
                //-----------------------------End--------------------------
                if (result == "0" || result == null)
                {
                    //--------------Not saved
                    ViewData["result"] = 0;
                }
                if (result == "-1")
                {
                    //-----alredy exist
                    ViewData["result"] = -1;
                }
                else
                {
                    string schlid = Session["SCHL"].ToString();
                    DataSet seleLastCan = objDB.SelectChangesStreams(schlid);
                    if (seleLastCan.Tables[0].Rows.Count > 0)
                    {

                        @ViewBag.message = "1";
                        rm.StoreAllData = seleLastCan;
                        ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                    }
                    else
                    {
                        @ViewBag.message = "Record Not Found";
                    }
                    return View(rm);
                }
                return View(rm);
            }
            else
            {
                try
                {
                    //        var itemsch = new SelectList(new[]{new {ID="1",Name="Matriculation Regular"},new{ID="2",Name="Matriculation Open"},
                    //new{ID="3",Name="Sr.Secondary Regular"},new{ID="4",Name="Sr.Secondary Open"},}, "ID", "Name", 1);
                    //        ViewBag.MySch = itemsch.ToList();
                    //var itemsch = new SelectList(new[] { new { ID = "3", Name = "Sr.Secondary Regular" }, }, "ID", "Name", 3);
                    //ViewBag.MySch = itemsch.ToList();

                    DataSet seleLastCan = objDB.SearchCorrectionStudentDetails(formname, schlcode, sid);
                    if (seleLastCan.Tables[0].Rows.Count > 0)
                    {

                        @ViewBag.message = "1";
                        @ViewBag.stdid = seleLastCan.Tables[0].Rows[0]["std_id"].ToString();
                        @ViewBag.Oroll = seleLastCan.Tables[0].Rows[0]["Board_Roll_Num"].ToString();
                        @ViewBag.Regno = seleLastCan.Tables[0].Rows[0]["Registration_num"].ToString();
                        @ViewBag.category = seleLastCan.Tables[0].Rows[0]["category"].ToString();
                        @ViewBag.session = seleLastCan.Tables[0].Rows[0]["Year"].ToString() + "-" + seleLastCan.Tables[0].Rows[0]["Month"].ToString();
                        @ViewBag.canName = seleLastCan.Tables[0].Rows[0]["Candi_Name"].ToString();
                        @ViewBag.FName = seleLastCan.Tables[0].Rows[0]["Father_Name"].ToString();
                        @ViewBag.Mname = seleLastCan.Tables[0].Rows[0]["Mother_Name"].ToString();
                        @ViewBag.lot = seleLastCan.Tables[0].Rows[0]["lot"].ToString();
                        @ViewBag.DOB = seleLastCan.Tables[0].Rows[0]["DOB"].ToString();
                        @ViewBag.stdstream = seleLastCan.Tables[0].Rows[0]["Group_Name"].ToString();
                        ViewBag.StreamCnt = seleLastCan.Tables[0].Rows.Count;
                        @ViewBag.Subjlist = seleLastCan.Tables[0].Rows[0]["SubjectList"].ToString();
                    }
                    else
                    {
                        @ViewBag.message = "Record Not Found";
                        return View(rm);
                    }
                    rm.Stream = @ViewBag.stdstream;
                    //-------------Fill Stream Allowed To Schoool---------------//
                    //-------------Fill Stream Allowed To Schoool---------------//

                    ViewBag.vm5 = objCommon.GetGroupMedium();
                    ViewBag.vm6 = objCommon.GetGroupMedium();
                    ViewBag.vm7 = objCommon.GetGroupMedium();
                    if (Session["SCHL"] == null)
                    {
                        return RedirectToAction("Logout", "Login");
                    }
                    string schlid = Session["SCHL"].ToString();

                    List<SelectListItem> MyGroupList = objCommon.GroupName();
                    DataTable dtAssignSubject = objCommon.GetAssignSubjectBySchool(schlid);
                    if (dtAssignSubject != null && dtAssignSubject.Rows.Count > 0)
                    {
                        ViewBag.MyGroup = objCommon.GetSubjectsBySchool(dtAssignSubject, MyGroupList);
                    }
                    ViewBag.MyGroup = MyGroupList;

                    DataSet scis = objDB.SubjectsTweleve_SCI();
                    if (scis == null || scis.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.ssub = scis.Tables[0];
                        List<SelectListItem> scisub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.ssub.Rows)
                        {
                            scisub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }

                        ViewBag.sss8 = new SelectList(scisub, "Value", "Text");


                    }
                    DataSet hds = objDB.SubjectsTweleve_hum();
                    if (hds == null || hds.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.hsub = hds.Tables[0];
                        List<SelectListItem> humsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.hsub.Rows)
                        {
                            humsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }
                        ViewBag.hs5 = new SelectList(humsub, "Value", "Text");
                        ViewBag.hs6 = new SelectList(humsub, "Value", "Text");
                        ViewBag.hs7 = new SelectList(humsub, "Value", "Text");
                        ViewBag.hs8 = new SelectList(humsub, "Value", "Text");
                        ViewBag.vs10 = new SelectList(humsub, "Value", "Text"); //--------humanity Subjects Equals To Vocational Aditional Subjects
                    }
                    DataSet tecds = objDB.SubjectsTweleve_tech();
                    if (tecds == null || tecds.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.tsub = tecds.Tables[0];
                        List<SelectListItem> tecsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.tsub.Rows)
                        {
                            tecsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }
                        ViewBag.ts5 = new SelectList(tecsub, "Value", "Text");
                        ViewBag.ts6 = new SelectList(tecsub, "Value", "Text");
                        ViewBag.ts7 = new SelectList(tecsub, "Value", "Text");
                        ViewBag.ts8 = new SelectList(tecsub, "Value", "Text");

                    }
                    DataSet agrds = objDB.SubjectsTweleve_agr();
                    if (agrds == null || agrds.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.agrsub = agrds.Tables[0];
                        List<SelectListItem> agrsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.agrsub.Rows)
                        {
                            agrsub.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }

                        ViewBag.as5 = new SelectList(agrsub, "Value", "Text");
                        ViewBag.as6 = new SelectList(agrsub, "Value", "Text");
                        ViewBag.as7 = new SelectList(agrsub, "Value", "Text");
                        ViewBag.as8 = new SelectList(agrsub, "Value", "Text");

                    }
                    DataSet vocds = objDB.Voc_agr();
                    if (vocds == null || vocds.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.vocsub = vocds.Tables[0];
                        List<SelectListItem> vsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.vocsub.Rows)
                        {
                            vsub.Add(new SelectListItem { Text = @dr["group"].ToString(), Value = @dr["group"].ToString() });
                        }

                        ViewBag.selgroup = new SelectList(vsub, "Value", "Text");

                    }
                    DataSet vocTrade = objDB.SubjectsTweleve_Voc_All_Trade();
                    if (vocTrade == null || vocTrade.Tables[0].Rows.Count == 0)
                    { ViewBag.sub = ""; }
                    else
                    {
                        ViewBag.Tsub = vocTrade.Tables[0];
                        List<SelectListItem> vocTsub = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.Tsub.Rows)
                        {
                            vocTsub.Add(new SelectListItem { Text = @dr["tname"].ToString(), Value = @dr["tcode"].ToString() });
                        }
                        ViewBag.trgroup = new SelectList(vocTsub, "Value", "Text");

                    }

                    //-----------End-------------------//   
                    //----------------Subject voc Group-----------
                    if (rm.Tgroup == "VOCATIONAL")
                    {
                        DataSet gp = objDB.selectGP(rm.Std_id);
                        if (gp == null || gp.Tables[0].Rows.Count == 0)
                        {
                            //return RedirectToAction("T1Formgrid", "RegistrationPortal");
                        }
                        else
                        {
                            rm.groupsel = gp.Tables[0].Rows[0]["Group"].ToString().Trim();
                            string gsel = gp.Tables[0].Rows[0]["Group"].ToString().Trim();
                            rm.grouptr = gp.Tables[0].Rows[0]["TCODE"].ToString().Trim();
                            DataSet vocTRgroup = objDB.Voc_Trgroup(gsel);
                            if (vocds == null || vocds.Tables[0].Rows.Count == 0)
                            { ViewBag.sub = ""; }
                            else
                            {
                                ViewBag.vocTR = vocTRgroup.Tables[0];
                                List<SelectListItem> vtrsub = new List<SelectListItem>();
                                foreach (System.Data.DataRow dr in ViewBag.vocTR.Rows)
                                {
                                    vtrsub.Add(new SelectListItem { Text = @dr["TNAME"].ToString(), Value = @dr["TCODE"].ToString() });
                                }

                                ViewBag.trgroup = new SelectList(vtrsub, "Value", "Text");

                            }
                        }
                    }
                    //-----------------End---------------      

                    //-----------End-------------------// 

                    //if (ModelState.IsValid)
                    //{
                    //    //-------------New SubList---------------//
                    //    DataSet NewSub = new DataSet();
                    //    NewSub = objDB.NewCorrectionSubjects(sid, formname, schlcode);  //NewCorrectionSubjects
                    //    List<SelectListItem> NewSUBList = new List<SelectListItem>();
                    //    foreach (System.Data.DataRow dr in NewSub.Tables[0].Rows)
                    //    {
                    //        NewSUBList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });

                    //    }
                    //    ViewBag.SubNew = NewSUBList;
                    //    ViewBag.MediumNew = objCommon.GetMediumAll();
                    //    ViewBag.SubNewCnt = NewSub.Tables[0].Rows.Count;
                    //    //-----------End-------------------//                    

                    //    //----------Old Subject Fill Start----------//
                    //    DataSet ds = objDB.SearchOldStudent_Subject(sid, formname, schlcode);
                    //    if (ds == null || ds.Tables[0].Rows.Count == 0)
                    //    {
                    //        return RedirectToAction("SubjectCorrectionPerforma", "CorrectionSubjects");
                    //    }
                    //    if (ds.Tables[0].Rows.Count > 0)
                    //    {

                    //        List<SelectListItem> OLDSUBList = new List<SelectListItem>();
                    //        foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                    //        {
                    //            OLDSUBList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });

                    //        }
                    //        ViewBag.SubOLd = OLDSUBList;
                    //        ViewBag.SubCnt = ds.Tables[0].Rows.Count;
                    //    }

                    //    else
                    //    {

                    //        return View(rm);
                    //    }

                    //}
                    //else
                    //{
                    //    return View(rm);
                    //}
                    return View(rm);

                }
                catch (Exception ex)
                {
                    return View(rm);
                }

            }

        }
        public ActionResult CorrStreamDelete(string id)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB(); //calling class DBClass
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }

            if (id == null)
            {
                return RedirectToAction("StreamCorrectionPerforma", "CorrectionSubjects");

            }
            else
            {
                string result = objDB.DeleteStreamData(id);
                if (result == "Deleted")
                {
                    @ViewBag.result = "1";

                }

            }
            return RedirectToAction("StreamCorrectionPerforma", "CorrectionSubjects");
        }
        public ActionResult FinalSubmitStreamCorrection(FormCollection frm)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            RegistrationModels rm = new RegistrationModels();
            try
            {
                if (ModelState.IsValid)
                {

                    if (Session["SCHL"] != null)
                    {

                        rm.SCHL = Session["SCHL"].ToString();
                        rm.Correctiontype = "03";
                        string Class = "2";
                        string resultFS = objDB.FinalSubmitSubjectCorrection(rm, Class); // passing Value to DBClass from model
                        if (Convert.ToInt16(resultFS) > 0)
                        {
                            ViewData["resultFS"] = resultFS;
                            //return RedirectToAction("StreamCorrectionPerforma", "CorrectionSubjects");
                            return RedirectToAction("SchoolCorrectionFinalPrintLst", "RegistrationPortal");
                        }
                        else
                        {
                            ViewData["resultFS"] = "";
                            return RedirectToAction("StreamCorrectionPerforma", "CorrectionSubjects");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Logout", "Login");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Login");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login");
            }
        }
        #endregion Stream Correction

        #region Correction Performa 
        public ActionResult CorrectionPerforma()
        {
            return View();
        }
        #endregion Correction Performa

        #region Instructions Performa 
        public ActionResult Instructions()
        {
            return View();
        }
        #endregion Instructions Performa

        #region Subject Correction Performa Link
        public ActionResult SubjectCorrectionPerformaLink()
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            return View();
        }
        #endregion Subject Correction Performa Link

        #region Open Subject Correction
        public ActionResult OpenSubjectCorrectionPerforma()
        {

            OpenUserRegistration OUR = new OpenUserRegistration();
            string schlid = "";
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");

            }

            Session["app_no"] = "";
            Session["app_name"] = "";
            Session["app_id"] = "";
            ViewBag.Class = Session["app_class"] = ""; //string appno = "12201814044";  
            Session["app_stream"] = ""; //OpenUserLogin _openUserLogin = openDB.GetRecord(appno);
            Session["app_mob"] = ""; //if (_openUserLogin == null)
            Session["app_email"] = ""; //{ _openUserLogin = new OpenUserLogin(); }
            Session["app_adrs"] = ""; //else if (_openUserLogin.APPNO.ToString() == "0") { return RedirectToAction("Index"); }
            Session["app_adhr"] = ""; //else
            Session["app_subjectlist"] = "";

            Session["regStatus"] = "";
            Session["app_session"] = "";
            Session["app_session_New"] = "";
            Session["CentreStatus"] = "";
            Session["subStatus"] = "";
            Session["payVerify"] = "";


            //--------------------------------------------Grid Start-----------//
            schlid = Session["SCHL"].ToString();

            DataSet seleLastCan = objDB.PendingCorrectionSubjects(schlid, "22,44");
            if (seleLastCan.Tables[0].Rows.Count > 0)
            {
                @ViewBag.message = "1";
                OUR.StoreAllData = seleLastCan;
                ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                for (int i = 0; i < seleLastCan.Tables[0].Rows.Count; i++)
                {

                    if (seleLastCan.Tables[0].Rows[i]["DiffSub"].ToString() == null || seleLastCan.Tables[0].Rows[i]["DiffSub"].ToString() == "")
                    {
                        string candid = seleLastCan.Tables[0].Rows[i]["candid"].ToString();
                        string subNew = seleLastCan.Tables[0].Rows[i]["Newsubcode"].ToString();
                        string subOld = seleLastCan.Tables[0].Rows[i]["Oldsubcode"].ToString();
                        string DiffSub = string.Empty;

                        //modified
                        //Response.Write("modified");
                        if (subNew.Length == subOld.Length)
                        {
                            var list = subOld.Split(' ').Where(x => (!subNew.Split(' ').Contains(x))).ToList();
                            int count = list.Count;
                            foreach (var item in list)
                            {
                                DiffSub = item + " Modified";
                            }
                        }
                        ////Removed
                        if (subNew.Length > subOld.Length)
                        {
                            //Response.Write("Added...");
                            var list1 = subNew.Split(' ').Where(x => (!subOld.Split(' ').Contains(x))).ToList();
                            int count1 = list1.Count;
                            foreach (var item in list1)
                            {
                                DiffSub = item + " Added";
                            }
                        }

                        string DiffSubCorr = objDB.InsertDiffSubjects(candid, DiffSub);

                    }
                }
            }
            else
            {
                @ViewBag.message = "Record Not Found";
            }
            //---------------------------------------------Grid End-----------//

            return View(OUR);

        }

        [HttpPost]
        public ActionResult OpenSubjectCorrectionPerforma(FormCollection fc, string cmd, string Searchstring, string Sub_1_Chk, string Sub_2_Chk, string Sub_3_Chk, string Sub_4_Chk, string Sub_5_Chk, string Sub_6_Chk)
        {
            //FormCollection fc = new FormCollection();
            OpenUserRegistration OUR = new OpenUserRegistration();
            string schl = string.Empty;
            string appno = Searchstring;
            ViewBag.cnt = 0;

            if (Session["schl"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            schl = Session["schl"].ToString();

            #region Click Here to Change Subject Correction 
            if (cmd == "Click Here to Change Subject Correction")
            {

                ViewBag.Searchstring = Searchstring;
                OUR.APPNO = fc["SearchString"];
                appno = fc["SearchString"];//"12201814044";

                OpenUserLogin _openUserLogin = openDB.GetRecordCorr(appno, schl);
                if (_openUserLogin == null)
                { _openUserLogin = new OpenUserLogin(); }
                else if (_openUserLogin.APPNO.ToString() == "0") { return RedirectToAction("OpenSubjectCorrectionPerforma"); }
                else
                {
                    ViewBag.cnt = 1;
                    Session["app_no"] = _openUserLogin.APPNO.ToString();
                    Session["app_name"] = _openUserLogin.NAME.ToString();
                    Session["app_id"] = _openUserLogin.ID.ToString();
                    ViewBag.Class = Session["app_class"] = _openUserLogin.CLASS.ToString();
                    if (ViewBag.Class == "12")
                    {
                        Session["app_stream"] = fc["STREAM"].ToString().Split(',')[0]; //_openUserLogin.STREAM.ToString();
                    }
                    else
                    {
                        Session["app_stream"] = "";
                    }

                    Session["app_mob"] = _openUserLogin.MOBILENO.Trim();
                    Session["app_email"] = _openUserLogin.EMAILID.Trim().ToUpper();
                    Session["app_adrs"] = _openUserLogin.ADDRESS.Trim().ToUpper();
                    Session["app_adhr"] = _openUserLogin.AADHAR_NO.Trim();
                    Session["BOARD"] = fc["BOARD"].ToString().ToUpper();
                    Session["Category"] = fc["Category"].ToString().ToUpper();
                    Session["Month"] = fc["Month"].ToString().ToUpper();
                    Session["Year"] = fc["Year"].ToString().ToUpper();
                    OUR.Month = fc["Month"].ToString().ToUpper();
                    OUR.YEAR = fc["Year"].ToString().ToUpper();

                }
                int regStatus = openDB.IsUserInReg(_openUserLogin.ID.ToString());
                Session["regStatus"] = regStatus.ToString();
                Session["app_session"] = "0";
                Session["app_session_New"] = fc["Year"].ToString().ToUpper();
                Session["CentreStatus"] = "0";
                Session["subStatus"] = openDB.IsUserInSubjects(_openUserLogin.ID.ToString()).ToString();
                Session["payVerify"] = "0";
                ViewBag.regStatus = regStatus.ToString();

                if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
                {
                    return RedirectToAction("Index");
                }
                //else if (Session["payVerify"].ToString() == "1" || Session["regStatus"].ToString() == "0" || Session["payStatus"].ToString() == "1")
                //{
                //    return RedirectToAction("Applicationstatus", "Open");
                //}
                else
                {
                    OpenUserRegistration _openUserRegistration = openDB.GetRegistrationRecord(Session["app_id"].ToString());
                    _openUserLogin = openDB.GetLoginById(Session["app_id"].ToString());
                    string app_class = Session["app_class"].ToString();
                    Session["subStatus"] = openDB.IsUserInSubjects(_openUserLogin.ID.ToString()).ToString();
                    if (openDB.IsUserInReg(_openUserLogin.ID.ToString()) == 1)
                    {
                        Session["CandPhoto"] = "../../" + (_openUserLogin.IMG_RAND.ToString().Trim() == string.Empty ? "Images/NoPhoto.jpg" : "Upload/" + _openUserLogin.IMG_RAND.ToString());
                        //Session["app_session"] = (_openUserRegistration.OSESSION.Length > 8) ? _openUserRegistration.OSESSION.Substring(_openUserRegistration.OSESSION.Length - 4, 4) : "";
                        // Change by rohit
                        Session["app_session"] = _openUserRegistration.OSESSION.Substring(_openUserRegistration.OSESSION.LastIndexOf(' ') + 1);
                        //Session["app_session"] = (_openUserRegistration.OSESSION != "") ? _openUserRegistration.OSESSION.Substring(_openUserRegistration.OSESSION.IndexOf(' '), _openUserRegistration.OSESSION.Length) : "";

                        Session["CentreStatus"] = _openUserRegistration.SCHL == "" ? "0" : "1";
                    }
                    if (app_class == "10")
                    {
                        //if (_openUserLogin.CATEGORY.ToUpper().Contains("10TH FAIL") && _openUserRegistration.PHY_CHAL.Contains("N.A."))
                        if (fc["CATEGORY"].ToUpper().Contains("10TH FAIL")) // change by gulab
                        {
                            if (fc["BOARD"].ToUpper().Contains("P.S.E.B") && fc["CATEGORY"].ToUpper().Contains("REGULAR"))
                            {
                                //string session = Session["app_session"].ToString();
                                //int appSession = Convert.ToInt32(session);
                                int appSession = Convert.ToInt32(fc["Year"]);
                                if (DateTime.Now.Year - 5 <= appSession)//earlier it was 4
                                {
                                    ViewBag.visible = "true";
                                    ViewBag.maxCC = "4";
                                }
                            }
                            else if (fc["BOARD"].ToUpper().Contains("NIOS") && fc["CATEGORY"].ToUpper().Contains("NIOS"))
                            {
                                //int appSession = Convert.ToInt32(Session["app_session"].ToString());
                                int appSession = Convert.ToInt32(fc["Year"]);
                                if (DateTime.Now.Year - 5 <= appSession)//earlier it was 2
                                {
                                    ViewBag.visible = "true";
                                    ViewBag.maxCC = "2";
                                }
                                else
                                {
                                    ViewBag.visible = "false";
                                    ViewBag.maxCC = "0";
                                }
                            }
                        }
                        else
                        {
                            ViewBag.visible = "false";
                            ViewBag.maxCC = "0";
                        }


                        List<SelectListItem> subjects = new List<SelectListItem>();
                        subjects.Add(new SelectListItem() { Text = "Punjabi", Value = "01" });
                        subjects.Add(new SelectListItem() { Text = "Punjab History & Culture", Value = "07" });
                        ViewBag.Sub1 = subjects;
                        subjects = new List<SelectListItem>();
                        if (_openUserRegistration.PHY_CHAL.Trim().ToUpper().Contains("N.A."))
                        {
                            ViewBag.Sub_2_5 = openDB.GetMatricSubjects_1();
                            ViewBag.Sub_6_add = openDB.GetMatricSubjects_2();
                            ViewBag.Sub_Matic_add = openDB.GetMatricSubjects_Add();
                        }
                        else
                        {
                            // + if DA = 'Yes' then sub2 to sub 6 show subject from subject master where opn = 'Y' and sub not in (63, 92, 01, 07)
                            //Add Sub 7,8: show subject from subject master where opn = 'Y' and sub not in (01, 07)
                            Session["Phy_Chal"] = "true";
                            ViewBag.Sub_2_5 = ViewBag.Sub_6_add = openDB.GetMatricSubjects_2();
                            ViewBag.Sub_Matic_add = openDB.GetMatricSubjects_Additional_DA_Yes();
                        }
                        TempData["Sub_2_5"] = subjects;
                        List<OpenUserSubjects> subjects_list = openDB.GetSubjectsForUser_New(Session["app_id"].ToString(), app_class, fc["Year"].ToString());

                        int i = 0;
                        foreach (OpenUserSubjects _openUserSubjects in subjects_list)
                        {
                            i++;
                            if (i < 7)
                            {
                                if (ViewBag.visible != null && ViewBag.visible == "true")
                                {
                                    ViewData["Subject_" + i] = _openUserSubjects.SUB;
                                    ViewData["Sub_" + i + "_Th_Obt"] = (_openUserSubjects.OBTMARKS.Length > 0) ? _openUserSubjects.OBTMARKS : "0";
                                    ViewData["Sub_" + i + "_Th_Min"] = (_openUserSubjects.MINMARKS.Length > 0) ? _openUserSubjects.MINMARKS : "0";
                                    ViewData["Sub_" + i + "_Th_Max"] = (_openUserSubjects.MAXMARKS.Length > 0) ? _openUserSubjects.MAXMARKS : "0";
                                    ViewData["Sub_" + i + "_Pr_Obt"] = (_openUserSubjects.OBTMARKSP.Length > 0) ? _openUserSubjects.OBTMARKSP : "0";
                                    ViewData["Sub_" + i + "_Pr_Min"] = (_openUserSubjects.MINMARKSP.Length > 0) ? _openUserSubjects.MINMARKSP : "0";
                                    ViewData["Sub_" + i + "_Pr_Max"] = (_openUserSubjects.MAXMARKSP.Length > 0) ? _openUserSubjects.MAXMARKSP : "0";
                                    ViewData["Sub_" + i + "_CCE_Obt"] = (_openUserSubjects.OBTMARKSCC.Length > 0) ? _openUserSubjects.OBTMARKSCC : "0";
                                    ViewData["Sub_" + i + "_CCE_Min"] = (_openUserSubjects.MINMARKSCC.Length > 0) ? _openUserSubjects.MINMARKSCC : "0";
                                    ViewData["Sub_" + i + "_CCE_Max"] = (_openUserSubjects.MAXMARKSCC.Length > 0) ? _openUserSubjects.MAXMARKSCC : "0";

                                    //TempData["Subject_" + i] = _openUserSubjects.SUB;
                                    TempData["Sub_" + i + "_Th_Obt"] = (_openUserSubjects.OBTMARKS.Length > 0) ? _openUserSubjects.OBTMARKS : "0";
                                    TempData["Sub_" + i + "_Th_Min"] = (_openUserSubjects.MINMARKS.Length > 0) ? _openUserSubjects.MINMARKS : "0";
                                    TempData["Sub_" + i + "_Th_Max"] = (_openUserSubjects.MAXMARKS.Length > 0) ? _openUserSubjects.MAXMARKS : "0";
                                    TempData["Sub_" + i + "_Pr_Obt"] = (_openUserSubjects.OBTMARKSP.Length > 0) ? _openUserSubjects.OBTMARKSP : "0";
                                    TempData["Sub_" + i + "_Pr_Min"] = (_openUserSubjects.MINMARKSP.Length > 0) ? _openUserSubjects.MINMARKSP : "0";
                                    TempData["Sub_" + i + "_Pr_Max"] = (_openUserSubjects.MAXMARKSP.Length > 0) ? _openUserSubjects.MAXMARKSP : "0";
                                    TempData["Sub_" + i + "_CCE_Obt"] = (_openUserSubjects.OBTMARKSCC.Length > 0) ? _openUserSubjects.OBTMARKSCC : "0";
                                    TempData["Sub_" + i + "_CCE_Min"] = (_openUserSubjects.MINMARKSCC.Length > 0) ? _openUserSubjects.MINMARKSCC : "0";
                                    TempData["Sub_" + i + "_CCE_Max"] = (_openUserSubjects.MAXMARKSCC.Length > 0) ? _openUserSubjects.MAXMARKSCC : "0";
                                }
                                else
                                {
                                    ViewData["Subject_" + i] = _openUserSubjects.SUB;
                                    ViewData["Sub_" + i + "_Th_Obt"] = "0";
                                    ViewData["Sub_" + i + "_Th_Min"] = "0";
                                    ViewData["Sub_" + i + "_Th_Max"] = "0";
                                    ViewData["Sub_" + i + "_Pr_Obt"] = "0";
                                    ViewData["Sub_" + i + "_Pr_Min"] = "0";
                                    ViewData["Sub_" + i + "_Pr_Max"] = "0";
                                    ViewData["Sub_" + i + "_CCE_Obt"] = "0";
                                    ViewData["Sub_" + i + "_CCE_Min"] = "0";
                                    ViewData["Sub_" + i + "_CCE_Max"] = "0";

                                    //TempData["Subject_" + i] = _openUserSubjects.SUB;
                                    TempData["Sub_" + i + "_Th_Obt"] = "0";
                                    TempData["Sub_" + i + "_Th_Min"] = "0";
                                    TempData["Sub_" + i + "_Th_Max"] = "0";
                                    TempData["Sub_" + i + "_Pr_Obt"] = "0";
                                    TempData["Sub_" + i + "_Pr_Min"] = "0";
                                    TempData["Sub_" + i + "_Pr_Max"] = "0";
                                    TempData["Sub_" + i + "_CCE_Obt"] = "0";
                                    TempData["Sub_" + i + "_CCE_Min"] = "0";
                                    TempData["Sub_" + i + "_CCE_Max"] = "0";
                                }



                            }
                            else
                            {
                                ViewData["Subject_Add_" + (i - 6)] = _openUserSubjects.SUB;
                            }
                        }

                        if (subjects_list.Count() > 0)
                        {
                            ViewData["SubjectModify"] = "1";
                        }
                        else
                        { ViewData["SubjectModify"] = "0"; }

                        if (ViewData["Subject_Add_1"] == null)
                        { ViewData["Subject_Add_1"] = ""; }
                        if (ViewData["Subject_Add_2"] == null)
                        { ViewData["Subject_Add_2"] = ""; }




                    }
                    else if (app_class == "12")
                    {
                        string stream = fc["STREAM"].ToString().Split(',')[0];//Session["app_stream"].ToString();
                        List<SelectListItem> subjects = new List<SelectListItem>();
                        subjects.Add(new SelectListItem() { Value = "001", Text = "GENERAL ENGLISH" });
                        ViewBag.Sub1 = subjects;
                        subjects = new List<SelectListItem>();
                        subjects.Add(new SelectListItem() { Value = "002", Text = "GENERAL PUNJABI" });
                        subjects.Add(new SelectListItem() { Value = "003", Text = "PUNJAB HISTORY AND CULTURE" });
                        ViewBag.Sub2 = subjects;
                        subjects = new List<SelectListItem>();

                        if (stream.ToUpper().Contains("COMMERCE"))
                        {
                            subjects = openDB.GetSeniorSubjects_3();
                            ViewBag.comm = "true";
                            ViewBag.Sub_3_5 = subjects;
                            ViewBag.Sub_add = openDB.GetSeniorSubjects_AddSubList_COMM();
                            ViewBag.Sub3 = new List<SelectListItem>() { subjects.Find(f => f.Value == "141") };
                            ViewBag.Sub4 = new List<SelectListItem>() { subjects.Find(f => f.Value == "142") };
                            //ViewBag.Sub5 = new List<SelectListItem>() { subjects.Find(f => f.Value == "143") };
                            //ViewBag.Sub6 = new List<SelectListItem>() { subjects.Find(f => f.Value == "144") };
                            // chnage on 4aug2020 by gulab sir 
                            ViewBag.Sub5 = subjects.Where(f => f.Value == "026" || f.Value == "144").Select(f => f).ToList();
                            TempData["Sub_3_5"] = subjects;
                            TempData["Sub_add"] = (List<SelectListItem>)ViewBag.Sub_add;
                        }
                        else if (stream.ToUpper().Contains("HUMANITIES"))
                        {
                            //subjects = openDB.GetSeniorSubjects_1();
                            subjects = openDB.GetSeniorSubjects_MainSubjects();// by harpal sir 2023-24 : subject Computer Science (146) and environment education (139) do not show in main subject (Sr Sec class)
                            ViewBag.Sub_3_5 = subjects;
                            //ViewBag.Sub_add = openDB.GetSeniorSubjects_1();
                            ViewBag.Sub_add = openDB.GetSeniorSubjects_AddSubList(); //2023-24
                            TempData["Sub_3_5"] = subjects;
                            TempData["Sub_add"] = (List<SelectListItem>)ViewBag.Sub_add;
                        }
                        else if (stream.ToUpper().Contains("SCIENCE"))
                        {
                            subjects = openDB.GetSeniorSubjects_2();
                            ViewBag.Sub_3_5 = subjects;
                            ViewBag.Sub3 = new List<SelectListItem>() { subjects.Find(f => f.Value == "052") };
                            ViewBag.Sub4 = new List<SelectListItem>() { subjects.Find(f => f.Value == "053") };
                            ViewBag.Sub_add = openDB.GetSeniorSubjects_AddSubList_SCI();
                            TempData["Sub_3_5"] = new List<SelectListItem>();
                            TempData["Sub_add"] = (List<SelectListItem>)ViewBag.Sub_add;
                        }
                        else
                        {
                            subjects = openDB.GetSeniorSubjects_MainSubjects();
                            ViewBag.Sub_3_4 = subjects;
                            ViewBag.Sub_5 = openDB.GetSeniorSubjects_2();
                            ViewBag.Sub_add = subjects;
                            TempData["Sub_3_4"] = subjects;
                            TempData["Sub_add"] = subjects;
                            TempData["Sub_5"] = openDB.GetSeniorSubjects_2();
                            TempData["Sub_3_5"] = new List<SelectListItem>();
                        }

                        List<OpenUserSubjects> subjects_list = openDB.GetSubjectsForUser_New(Session["app_id"].ToString(), app_class, fc["Year"].ToString());
                        if (subjects_list.Count() > 0)
                        {
                            ViewData["SubjectModify"] = "1";
                        }
                        else
                        { ViewData["SubjectModify"] = "0"; }
                        int i = 0;
                        foreach (OpenUserSubjects _openUserSubjects in subjects_list)
                        {
                            i++;
                            if (i < 6)
                            {
                                if (ViewBag.visible != null && ViewBag.visible == "true")
                                {
                                    ViewData["Subject_" + i] = _openUserSubjects.SUB;
                                    ViewData["Sub_" + i + "_Th_Obt"] = (_openUserSubjects.OBTMARKS.Length > 0) ? _openUserSubjects.OBTMARKS : "0";
                                    ViewData["Sub_" + i + "_Th_Min"] = (_openUserSubjects.MINMARKS.Length > 0) ? _openUserSubjects.MINMARKS : "0";
                                    ViewData["Sub_" + i + "_Th_Max"] = (_openUserSubjects.MAXMARKS.Length > 0) ? _openUserSubjects.MAXMARKS : "0";
                                    ViewData["Sub_" + i + "_Pr_Obt"] = (_openUserSubjects.OBTMARKSP.Length > 0) ? _openUserSubjects.OBTMARKSP : "0";
                                    ViewData["Sub_" + i + "_Pr_Min"] = (_openUserSubjects.MINMARKSP.Length > 0) ? _openUserSubjects.MINMARKSP : "0";
                                    ViewData["Sub_" + i + "_Pr_Max"] = (_openUserSubjects.MAXMARKSP.Length > 0) ? _openUserSubjects.MAXMARKSP : "0";
                                    ViewData["Sub_" + i + "_CCE_Obt"] = (_openUserSubjects.OBTMARKSCC.Length > 0) ? _openUserSubjects.OBTMARKSCC : "0";
                                    ViewData["Sub_" + i + "_CCE_Min"] = (_openUserSubjects.MINMARKSCC.Length > 0) ? _openUserSubjects.MINMARKSCC : "0";
                                    ViewData["Sub_" + i + "_CCE_Max"] = (_openUserSubjects.MAXMARKSCC.Length > 0) ? _openUserSubjects.MAXMARKSCC : "0";

                                }
                                else
                                {
                                    ViewData["Subject_" + i] = _openUserSubjects.SUB;
                                    ViewData["Sub_" + i + "_Th_Obt"] = "0";
                                    ViewData["Sub_" + i + "_Th_Min"] = "0";
                                    ViewData["Sub_" + i + "_Th_Max"] = "0";
                                    ViewData["Sub_" + i + "_Pr_Obt"] = "0";
                                    ViewData["Sub_" + i + "_Pr_Min"] = "0";
                                    ViewData["Sub_" + i + "_Pr_Max"] = "0";
                                    ViewData["Sub_" + i + "_CCE_Obt"] = "0";
                                    ViewData["Sub_" + i + "_CCE_Min"] = "0";
                                    ViewData["Sub_" + i + "_CCE_Max"] = "0";
                                }

                            }
                            else
                            {
                                if (stream.Contains("COMMERCE"))
                                {
                                    if (i == 6)
                                    {
                                        ViewData["Subject_" + i] = (stream.Contains("COMMERCE")) ? _openUserSubjects.SUB : "";
                                        ViewData["Sub_" + i + "_Th_Obt"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.OBTMARKS.Length > 0) ? _openUserSubjects.OBTMARKS : "0") : "0";
                                        ViewData["Sub_" + i + "_Th_Min"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MINMARKS.Length > 0) ? _openUserSubjects.MINMARKS : "0") : "0";
                                        ViewData["Sub_" + i + "_Th_Max"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MAXMARKS.Length > 0) ? _openUserSubjects.MAXMARKS : "0") : "0";
                                        ViewData["Sub_" + i + "_Pr_Obt"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.OBTMARKSP.Length > 0) ? _openUserSubjects.OBTMARKSP : "0") : "0";
                                        ViewData["Sub_" + i + "_Pr_Min"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MINMARKSP.Length > 0) ? _openUserSubjects.MINMARKSP : "0") : "0";
                                        ViewData["Sub_" + i + "_Pr_Max"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MAXMARKSP.Length > 0) ? _openUserSubjects.MAXMARKSP : "0") : "0";
                                        ViewData["Sub_" + i + "_CCE_Obt"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.OBTMARKSCC.Length > 0) ? _openUserSubjects.OBTMARKSCC : "0") : "0";
                                        ViewData["Sub_" + i + "_CCE_Min"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MINMARKSCC.Length > 0) ? _openUserSubjects.MINMARKSCC : "0") : "0";
                                        ViewData["Sub_" + i + "_CCE_Max"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MAXMARKSCC.Length > 0) ? _openUserSubjects.MAXMARKSCC : "0") : "0";
                                    }
                                    else
                                    {
                                        ViewData["Subject_Add_" + (i - 6)] = _openUserSubjects.SUB;
                                    }
                                }
                                else
                                {
                                    ViewData["Subject_Add_" + (i - 5)] = _openUserSubjects.SUB;
                                    if (i == 6)
                                    {
                                        ViewData["Subject_" + i] = (stream.Contains("COMMERCE")) ? _openUserSubjects.SUB : "";
                                        ViewData["Sub_" + i + "_Th_Obt"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.OBTMARKS.Length > 0) ? _openUserSubjects.OBTMARKS : "0") : "0";
                                        ViewData["Sub_" + i + "_Th_Min"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MINMARKS.Length > 0) ? _openUserSubjects.MINMARKS : "0") : "0";
                                        ViewData["Sub_" + i + "_Th_Max"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MAXMARKS.Length > 0) ? _openUserSubjects.MAXMARKS : "0") : "0";
                                        ViewData["Sub_" + i + "_Pr_Obt"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.OBTMARKSP.Length > 0) ? _openUserSubjects.OBTMARKSP : "0") : "0";
                                        ViewData["Sub_" + i + "_Pr_Min"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MINMARKSP.Length > 0) ? _openUserSubjects.MINMARKSP : "0") : "0";
                                        ViewData["Sub_" + i + "_Pr_Max"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MAXMARKSP.Length > 0) ? _openUserSubjects.MAXMARKSP : "0") : "0";
                                        ViewData["Sub_" + i + "_CCE_Obt"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.OBTMARKSCC.Length > 0) ? _openUserSubjects.OBTMARKSCC : "0") : "0";
                                        ViewData["Sub_" + i + "_CCE_Min"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MINMARKSCC.Length > 0) ? _openUserSubjects.MINMARKSCC : "0") : "0";
                                        ViewData["Sub_" + i + "_CCE_Max"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MAXMARKSCC.Length > 0) ? _openUserSubjects.MAXMARKSCC : "0") : "0";
                                    }
                                }
                            }

                        }

                        if (ViewData["Subject_Add_1"] == null)
                        { ViewData["Subject_Add_1"] = ""; }
                        if (ViewData["Subject_Add_2"] == null)
                        { ViewData["Subject_Add_2"] = ""; }
                        if (ViewData["Subject_6"] == null)
                        {
                            ViewData["Subject_6"] = string.Empty;
                            ViewData["Sub_6_Th_Obt"] = "0";
                            ViewData["Sub_6_Th_Min"] = "0";
                            ViewData["Sub_6_Th_Max"] = "0";
                            ViewData["Sub_6_Pr_Obt"] = "0";
                            ViewData["Sub_6_Pr_Min"] = "0";
                            ViewData["Sub_6_Pr_Max"] = "0";
                            ViewData["Sub_6_CCE_Obt"] = "0";
                            ViewData["Sub_6_CCE_Min"] = "0";
                            ViewData["Sub_6_CCE_Max"] = "0";
                        }


                        if (_openUserLogin.CATEGORY.ToUpper().Contains("12TH FAIL (REGULAR"))
                        {
                            if (_openUserRegistration.BOARD.ToUpper().Contains("P.S.E.B"))
                            {
                                if (Session["app_session"] == null)
                                {
                                    ViewData["Result"] = "AY";
                                    return View(_openUserRegistration);
                                }
                                int appSession = Convert.ToInt32(Session["app_session"].ToString());
                                if (DateTime.Now.Year - 5 < appSession)//earlier it was 2
                                {
                                    ViewBag.visible = "true";
                                    ViewBag.maxCC = "2";
                                }
                            }
                        }
                        else if (_openUserRegistration.BOARD.ToUpper().Contains("NIOS") && _openUserLogin.CATEGORY.ToUpper().Contains("NIOS"))
                        {
                            if (Session["app_session"] == null)
                            {
                                ViewData["Result"] = "AY";
                                return View(_openUserRegistration);
                            }
                            int appSession = Convert.ToInt32(Session["app_session"].ToString());
                            if (DateTime.Now.Year - 5 < appSession)//earlier it was 2
                            {
                                ViewBag.visible = "true";
                                ViewBag.maxCC = "2";
                            }
                        }


                    }
                    else
                    {
                        return RedirectToAction("OpenSubjectCorrectionPerforma");
                    }
                    //--------------------------------------------Grid Start-----------//
                    string schlid = Session["SCHL"].ToString();

                    DataSet seleLastCan = objDB.PendingCorrectionSubjects(schlid, "22,44");
                    if (seleLastCan.Tables[0].Rows.Count > 0)
                    {
                        @ViewBag.message = "1";
                        _openUserRegistration.StoreAllData = seleLastCan;
                        ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                        for (int i = 0; i < seleLastCan.Tables[0].Rows.Count; i++)
                        {

                            if (seleLastCan.Tables[0].Rows[i]["DiffSub"].ToString() == null || seleLastCan.Tables[0].Rows[i]["DiffSub"].ToString() == "")
                            {
                                string candid = seleLastCan.Tables[0].Rows[i]["candid"].ToString();
                                string subNew = seleLastCan.Tables[0].Rows[i]["Newsubcode"].ToString();
                                string subOld = seleLastCan.Tables[0].Rows[i]["Oldsubcode"].ToString();
                                string DiffSub = string.Empty;

                                //modified
                                //Response.Write("modified");
                                if (subNew.Length == subOld.Length)
                                {
                                    var list = subOld.Split(' ').Where(x => (!subNew.Split(' ').Contains(x))).ToList();
                                    int count = list.Count;
                                    foreach (var item in list)
                                    {
                                        DiffSub = item + " Modified";
                                    }
                                }
                                ////Removed
                                if (subNew.Length > subOld.Length)
                                {
                                    //Response.Write("Added...");
                                    var list1 = subNew.Split(' ').Where(x => (!subOld.Split(' ').Contains(x))).ToList();
                                    int count1 = list1.Count;
                                    foreach (var item in list1)
                                    {
                                        DiffSub = item + " Added";
                                    }
                                }

                                string DiffSubCorr = objDB.InsertDiffSubjects(candid, DiffSub);

                            }
                        }



                    }
                    else
                    {
                        @ViewBag.message = "Record Not Found";
                    }
                    //---------------------------------------------Grid End-----------//

                    //--------Start After Student Details Grid Details----------

                    ViewBag.Boards = openDB.GetN2Board();
                    ViewBag.Months = openDB.GetMonths();
                    ViewBag.Years = openDB.GetYears();

                    _openUserLogin = openDB.GetLoginById(appno);
                    ViewBag.subStatus = openDB.IsUserInSubjects(_openUserLogin.ID.ToString()).ToString();

                    if (Session["app_class"].ToString() == "10")
                    {
                        ViewBag.categories = openDB.GetMCategories();
                        _openUserRegistration.CATEGORY = _openUserLogin.CATEGORY;
                    }
                    else
                    {
                        ViewBag.categories = openDB.GetTCategories();
                        _openUserRegistration.CATEGORY = _openUserLogin.CATEGORY;

                        if (_openUserLogin.CATEGORY == "12th FAIL (Regular School-Science Group)")
                        {
                            ViewBag.streams = openDB.GetStreams12_1();
                            _openUserRegistration.STREAM = _openUserLogin.STREAM;
                        }
                        else
                        {
                            ViewBag.streams = openDB.GetStreams12_2();
                            _openUserRegistration.STREAM = _openUserLogin.STREAM;
                        }


                    }

                    List<SelectListItem> casts = objCommon.GetCaste();
                    casts.RemoveAll(r => r.Text.Contains("SC("));
                    ViewBag.Cast = casts;

                    if (_openUserLogin.CATEGORY.ToLower().Contains("direct"))
                    {
                        ViewBag.disableBoard = "true";
                    }
                    else
                    {
                        ViewBag.disableBoard = "false";
                    }



                    if (_openUserLogin.CATEGORY == "10TH PASSED")
                    {
                        //  List<SelectListItem> yearlist = openDB.GetYears();
                        //yearlist.Remove(yearlist[1]);
                        List<SelectListItem> yearlist = new AbstractLayer.DBClass().GetSessionYearSchoolAdmin();
                        yearlist.Remove(yearlist[0]);
                        //yearlist.Remove(yearlist[0]);
                        ViewBag.Years = yearlist;
                    }

                    try
                    {
                        if (_openUserRegistration.CASTE != "")
                        {
                            SelectListItem item = casts.Find(f => f.Value.ToUpper() == _openUserRegistration.CASTE.ToUpper());
                            casts.Remove(item);
                            item.Selected = true;
                            casts.Add(item);
                            ViewBag.Cast = casts;
                        }
                    }
                    catch (Exception) { }

                    if (_openUserRegistration == null)
                    {
                        _openUserRegistration = new OpenUserRegistration();
                        _openUserRegistration.APPNO = appno;
                    }
                    else
                    {
                        if (_openUserRegistration.OSESSION != null && _openUserRegistration.OSESSION != string.Empty)
                        {
                            string[] osession = _openUserRegistration.OSESSION.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            //ViewBag.month = osession[0];
                            //ViewBag.year = osession[1];
                            ViewBag.year = fc["Year"];
                            ViewBag.month = fc["Month"];

                        }
                    }
                    //--------End After Student Details Grid Details----------
                    //ViewBag.streams= fc["STREAM"];
                    //ViewBag.categories= fc["CATEGORY"];
                    //ViewBag.Boards = fc["BOARD"];
                    _openUserRegistration.CATEGORY = fc["CATEGORY"].Split(',')[0];
                    _openUserRegistration.BOARD = fc["BOARD"].Split(',')[0];
                    _openUserRegistration.Month = fc["Month"];
                    _openUserRegistration.YEAR = fc["Year"];
                    if (Session["app_class"].ToString() == "12")
                    {
                        _openUserRegistration.STREAM = fc["STREAM"].Split(',')[0];
                    }


                    ViewBag.disableBoard = "true";
                    return View(_openUserRegistration);
                }
            }
            #endregion Click Here to Change Subject Correction 

            if (cmd == "Reset")
            {
                ViewBag.disableBoard = "false";
                return RedirectToAction("OpenSubjectCorrectionPerforma");
                // return View(_openUserRegistration);
            }
            #region Search
            if (cmd == "Search")
            {

                ViewBag.Searchstring = Searchstring;
                OUR.APPNO = fc["SearchString"];
                appno = fc["SearchString"];//"12201814044";

                OpenUserLogin _openUserLogin = openDB.GetRecordCorr(appno, schl);
                if (_openUserLogin == null)
                { _openUserLogin = new OpenUserLogin(); }
                else if (_openUserLogin.APPNO.ToString() == "0")
                {

                    return RedirectToAction("OpenSubjectCorrectionPerforma");
                }
                else
                {
                    ViewBag.cnt = 1;
                    Session["app_no"] = _openUserLogin.APPNO.ToString();
                    Session["app_name"] = _openUserLogin.NAME.ToString();
                    Session["app_id"] = _openUserLogin.ID.ToString();
                    ViewBag.Class = Session["app_class"] = _openUserLogin.CLASS.ToString();
                    Session["app_stream"] = _openUserLogin.STREAM.ToString();
                    Session["app_mob"] = _openUserLogin.MOBILENO.Trim();
                    Session["app_email"] = _openUserLogin.EMAILID.Trim().ToUpper();
                    Session["app_adrs"] = _openUserLogin.ADDRESS.Trim().ToUpper();
                    Session["app_adhr"] = _openUserLogin.AADHAR_NO.Trim();
                    Session["app_subjectlist"] = _openUserLogin.SubjectList.Trim();
                }
                int regStatus = openDB.IsUserInReg(_openUserLogin.ID.ToString());
                Session["regStatus"] = regStatus.ToString();
                Session["app_session"] = "0";
                Session["CentreStatus"] = "0";
                Session["subStatus"] = openDB.IsUserInSubjects(_openUserLogin.ID.ToString()).ToString();
                Session["payVerify"] = "0";
                ViewBag.regStatus = regStatus.ToString();

                if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
                {
                    return RedirectToAction("Index");
                }
                //else if (Session["payVerify"].ToString() == "1" || Session["regStatus"].ToString() == "0" || Session["payStatus"].ToString() == "1")
                //{
                //    return RedirectToAction("Applicationstatus", "Open");
                //}
                else
                {
                    OpenUserRegistration _openUserRegistration = openDB.GetRegistrationRecord(Session["app_id"].ToString());
                    _openUserLogin = openDB.GetLoginById(Session["app_id"].ToString());
                    string app_class = Session["app_class"].ToString();
                    Session["subStatus"] = openDB.IsUserInSubjects(_openUserLogin.ID.ToString()).ToString();
                    if (openDB.IsUserInReg(_openUserLogin.ID.ToString()) == 1)
                    {
                        Session["CandPhoto"] = "../../" + (_openUserLogin.IMG_RAND.ToString().Trim() == string.Empty ? "Images/NoPhoto.jpg" : "Upload/" + _openUserLogin.IMG_RAND.ToString());
                        //Session["app_session"] = (_openUserRegistration.OSESSION.Length > 8) ? _openUserRegistration.OSESSION.Substring(_openUserRegistration.OSESSION.Length - 4, 4) : "";
                        // Change by rohit
                        Session["app_session"] = _openUserRegistration.OSESSION.Substring(_openUserRegistration.OSESSION.LastIndexOf(' ') + 1);
                        //Session["app_session"] = (_openUserRegistration.OSESSION != "") ? _openUserRegistration.OSESSION.Substring(_openUserRegistration.OSESSION.IndexOf(' '), _openUserRegistration.OSESSION.Length) : "";

                        Session["CentreStatus"] = _openUserRegistration.SCHL == "" ? "0" : "1";
                    }
                    if (app_class == "10")
                    {
                        List<SelectListItem> subjects = new List<SelectListItem>();
                        subjects.Add(new SelectListItem() { Text = "Punjabi", Value = "01" });
                        ViewBag.Sub1 = subjects;
                        subjects = new List<SelectListItem>();
                        if (_openUserRegistration.PHY_CHAL.Trim().ToUpper().Contains("N.A."))
                        {
                            ViewBag.Sub_2_5 = openDB.GetMatricSubjects_1();
                            ViewBag.Sub_6_add = openDB.GetMatricSubjects_2();
                        }
                        else
                        {
                            Session["Phy_Chal"] = "true";
                            ViewBag.Sub_2_5 = ViewBag.Sub_6_add = openDB.GetMatricSubjects_2();
                        }
                        TempData["Sub_2_5"] = subjects;
                        List<OpenUserSubjects> subjects_list = openDB.GetSubjectsForUser(Session["app_id"].ToString());

                        int i = 0;
                        foreach (OpenUserSubjects _openUserSubjects in subjects_list)
                        {
                            i++;
                            if (i < 7)
                            {
                                ViewData["Subject_" + i] = _openUserSubjects.SUB;
                                ViewData["Sub_" + i + "_Th_Obt"] = (_openUserSubjects.OBTMARKS.Length > 0) ? _openUserSubjects.OBTMARKS : "0";
                                ViewData["Sub_" + i + "_Th_Min"] = (_openUserSubjects.MINMARKS.Length > 0) ? _openUserSubjects.MINMARKS : "0";
                                ViewData["Sub_" + i + "_Th_Max"] = (_openUserSubjects.MAXMARKS.Length > 0) ? _openUserSubjects.MAXMARKS : "0";
                                ViewData["Sub_" + i + "_Pr_Obt"] = (_openUserSubjects.OBTMARKSP.Length > 0) ? _openUserSubjects.OBTMARKSP : "0";
                                ViewData["Sub_" + i + "_Pr_Min"] = (_openUserSubjects.MINMARKSP.Length > 0) ? _openUserSubjects.MINMARKSP : "0";
                                ViewData["Sub_" + i + "_Pr_Max"] = (_openUserSubjects.MAXMARKSP.Length > 0) ? _openUserSubjects.MAXMARKSP : "0";
                                ViewData["Sub_" + i + "_CCE_Obt"] = (_openUserSubjects.OBTMARKSCC.Length > 0) ? _openUserSubjects.OBTMARKSCC : "0";
                                ViewData["Sub_" + i + "_CCE_Min"] = (_openUserSubjects.MINMARKSCC.Length > 0) ? _openUserSubjects.MINMARKSCC : "0";
                                ViewData["Sub_" + i + "_CCE_Max"] = (_openUserSubjects.MAXMARKSCC.Length > 0) ? _openUserSubjects.MAXMARKSCC : "0";

                                //TempData["Subject_" + i] = _openUserSubjects.SUB;
                                TempData["Sub_" + i + "_Th_Obt"] = (_openUserSubjects.OBTMARKS.Length > 0) ? _openUserSubjects.OBTMARKS : "0";
                                TempData["Sub_" + i + "_Th_Min"] = (_openUserSubjects.MINMARKS.Length > 0) ? _openUserSubjects.MINMARKS : "0";
                                TempData["Sub_" + i + "_Th_Max"] = (_openUserSubjects.MAXMARKS.Length > 0) ? _openUserSubjects.MAXMARKS : "0";
                                TempData["Sub_" + i + "_Pr_Obt"] = (_openUserSubjects.OBTMARKSP.Length > 0) ? _openUserSubjects.OBTMARKSP : "0";
                                TempData["Sub_" + i + "_Pr_Min"] = (_openUserSubjects.MINMARKSP.Length > 0) ? _openUserSubjects.MINMARKSP : "0";
                                TempData["Sub_" + i + "_Pr_Max"] = (_openUserSubjects.MAXMARKSP.Length > 0) ? _openUserSubjects.MAXMARKSP : "0";
                                TempData["Sub_" + i + "_CCE_Obt"] = (_openUserSubjects.OBTMARKSCC.Length > 0) ? _openUserSubjects.OBTMARKSCC : "0";
                                TempData["Sub_" + i + "_CCE_Min"] = (_openUserSubjects.MINMARKSCC.Length > 0) ? _openUserSubjects.MINMARKSCC : "0";
                                TempData["Sub_" + i + "_CCE_Max"] = (_openUserSubjects.MAXMARKSCC.Length > 0) ? _openUserSubjects.MAXMARKSCC : "0";



                                //if (i==3)
                                //{
                                //    _openUserRegistration.Sub_3_Th_Obt = ViewData["Sub_" + i + "_Th_Obt"].ToString();
                                //    _openUserRegistration.Sub_3_Pr_Obt = ViewData["Sub_" + i + "_Pr_Obt"].ToString();
                                //    _openUserRegistration.Sub_3_CCE_Obt = ViewData["Sub_" + i + "_CCE_Obt"].ToString();
                                //}

                            }
                            else
                            {
                                ViewData["Subject_Add_" + (i - 6)] = _openUserSubjects.SUB;
                            }
                        }

                        if (subjects_list.Count() > 0)
                        {
                            ViewData["SubjectModify"] = "1";
                        }
                        else
                        { ViewData["SubjectModify"] = "0"; }

                        if (ViewData["Subject_Add_1"] == null)
                        { ViewData["Subject_Add_1"] = ""; }
                        if (ViewData["Subject_Add_2"] == null)
                        { ViewData["Subject_Add_2"] = ""; }



                        //if (_openUserLogin.CATEGORY.ToUpper().Contains("10TH FAIL") && _openUserRegistration.PHY_CHAL.Contains("N.A."))
                        if (_openUserLogin.CATEGORY.ToUpper().Contains("10TH FAIL")) // change by gulab
                        {
                            if (_openUserRegistration.BOARD.ToUpper().Contains("P.S.E.B") && _openUserLogin.CATEGORY.ToUpper().Contains("REGULAR"))
                            {
                                string session = Session["app_session"].ToString();
                                int appSession = Convert.ToInt32(session);
                                if (DateTime.Now.Year - 5 <= appSession)//earlier it was 4
                                {
                                    ViewBag.visible = "true";
                                    ViewBag.maxCC = "4";
                                }
                            }
                            else if (_openUserRegistration.BOARD.ToUpper().Contains("NIOS") && _openUserLogin.CATEGORY.ToUpper().Contains("NIOS"))
                            {
                                int appSession = Convert.ToInt32(Session["app_session"].ToString());
                                if (DateTime.Now.Year - 5 <= appSession)//earlier it was 2
                                {
                                    ViewBag.visible = "true";
                                    ViewBag.maxCC = "2";
                                }
                            }
                        }
                        else
                        {
                            ViewBag.visible = "false";
                            ViewBag.maxCC = "0";
                        }
                    }
                    else if (app_class == "12")
                    {
                        string stream = Session["app_stream"].ToString();
                        List<SelectListItem> subjects = new List<SelectListItem>();
                        subjects.Add(new SelectListItem() { Value = "001", Text = "GENERAL ENGLISH" });
                        ViewBag.Sub1 = subjects;
                        subjects = new List<SelectListItem>();
                        subjects.Add(new SelectListItem() { Value = "002", Text = "GENERAL PUNJABI" });
                        subjects.Add(new SelectListItem() { Value = "003", Text = "PUNJAB HISTORY AND CULTURE" });
                        ViewBag.Sub2 = subjects;
                        subjects = new List<SelectListItem>();

                        if (stream.ToUpper().Contains("COMMERCE"))
                        {
                            subjects = openDB.GetSeniorSubjects_3();
                            ViewBag.comm = "true";
                            ViewBag.Sub_3_5 = subjects;
                            ViewBag.Sub_add = openDB.GetSeniorSubjects_1();

                            ViewBag.Sub3 = new List<SelectListItem>() { subjects.Find(f => f.Value == "141") };
                            ViewBag.Sub4 = new List<SelectListItem>() { subjects.Find(f => f.Value == "142") };
                            //ViewBag.Sub5 = new List<SelectListItem>() { subjects.Find(f => f.Value == "143") };
                            //ViewBag.Sub6 = new List<SelectListItem>() { subjects.Find(f => f.Value == "144") };
                            // chnage on 4aug2020 by gulab sir 
                            ViewBag.Sub5 = subjects.Where(f => f.Value == "026" || f.Value == "144").Select(f => f).ToList();
                            TempData["Sub_3_5"] = subjects;
                            TempData["Sub_add"] = (List<SelectListItem>)ViewBag.Sub_add;
                        }
                        else if (stream.ToUpper().Contains("HUMANITIES"))
                        {
                            subjects = openDB.GetSeniorSubjects_1();
                            ViewBag.Sub_3_5 = subjects;
                            ViewBag.Sub_add = openDB.GetSeniorSubjects_1();
                            TempData["Sub_3_5"] = subjects;
                            TempData["Sub_add"] = (List<SelectListItem>)ViewBag.Sub_add;
                        }
                        else if (stream.ToUpper().Contains("SCIENCE"))
                        {
                            subjects = openDB.GetSeniorSubjects_2();
                            ViewBag.Sub_3_5 = subjects;
                            ViewBag.Sub_add = openDB.GetSeniorSubjects_1();
                            TempData["Sub_3_5"] = new List<SelectListItem>();
                            TempData["Sub_add"] = (List<SelectListItem>)ViewBag.Sub_add;
                        }
                        else
                        {
                            subjects = openDB.GetSeniorSubjects_1();
                            ViewBag.Sub_3_4 = subjects;
                            ViewBag.Sub_5 = openDB.GetSeniorSubjects_2();
                            ViewBag.Sub_add = subjects;
                            TempData["Sub_3_4"] = subjects;
                            TempData["Sub_add"] = subjects;
                            TempData["Sub_5"] = openDB.GetSeniorSubjects_2();
                            TempData["Sub_3_5"] = new List<SelectListItem>();
                        }

                        List<OpenUserSubjects> subjects_list = openDB.GetSubjectsForUser(Session["app_id"].ToString());
                        if (subjects_list.Count() > 0)
                        {
                            ViewData["SubjectModify"] = "1";
                        }
                        else
                        { ViewData["SubjectModify"] = "0"; }
                        int i = 0;
                        foreach (OpenUserSubjects _openUserSubjects in subjects_list)
                        {
                            i++;
                            if (i < 6)
                            {
                                ViewData["Subject_" + i] = _openUserSubjects.SUB;
                                ViewData["Sub_" + i + "_Th_Obt"] = (_openUserSubjects.OBTMARKS.Length > 0) ? _openUserSubjects.OBTMARKS : "0";
                                ViewData["Sub_" + i + "_Th_Min"] = (_openUserSubjects.MINMARKS.Length > 0) ? _openUserSubjects.MINMARKS : "0";
                                ViewData["Sub_" + i + "_Th_Max"] = (_openUserSubjects.MAXMARKS.Length > 0) ? _openUserSubjects.MAXMARKS : "0";
                                ViewData["Sub_" + i + "_Pr_Obt"] = (_openUserSubjects.OBTMARKSP.Length > 0) ? _openUserSubjects.OBTMARKSP : "0";
                                ViewData["Sub_" + i + "_Pr_Min"] = (_openUserSubjects.MINMARKSP.Length > 0) ? _openUserSubjects.MINMARKSP : "0";
                                ViewData["Sub_" + i + "_Pr_Max"] = (_openUserSubjects.MAXMARKSP.Length > 0) ? _openUserSubjects.MAXMARKSP : "0";
                                ViewData["Sub_" + i + "_CCE_Obt"] = (_openUserSubjects.OBTMARKSCC.Length > 0) ? _openUserSubjects.OBTMARKSCC : "0";
                                ViewData["Sub_" + i + "_CCE_Min"] = (_openUserSubjects.MINMARKSCC.Length > 0) ? _openUserSubjects.MINMARKSCC : "0";
                                ViewData["Sub_" + i + "_CCE_Max"] = (_openUserSubjects.MAXMARKSCC.Length > 0) ? _openUserSubjects.MAXMARKSCC : "0";
                            }
                            else
                            {
                                if (stream.Contains("COMMERCE"))
                                {
                                    if (i == 6)
                                    {
                                        ViewData["Subject_" + i] = (stream.Contains("COMMERCE")) ? _openUserSubjects.SUB : "";
                                        ViewData["Sub_" + i + "_Th_Obt"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.OBTMARKS.Length > 0) ? _openUserSubjects.OBTMARKS : "0") : "0";
                                        ViewData["Sub_" + i + "_Th_Min"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MINMARKS.Length > 0) ? _openUserSubjects.MINMARKS : "0") : "0";
                                        ViewData["Sub_" + i + "_Th_Max"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MAXMARKS.Length > 0) ? _openUserSubjects.MAXMARKS : "0") : "0";
                                        ViewData["Sub_" + i + "_Pr_Obt"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.OBTMARKSP.Length > 0) ? _openUserSubjects.OBTMARKSP : "0") : "0";
                                        ViewData["Sub_" + i + "_Pr_Min"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MINMARKSP.Length > 0) ? _openUserSubjects.MINMARKSP : "0") : "0";
                                        ViewData["Sub_" + i + "_Pr_Max"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MAXMARKSP.Length > 0) ? _openUserSubjects.MAXMARKSP : "0") : "0";
                                        ViewData["Sub_" + i + "_CCE_Obt"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.OBTMARKSCC.Length > 0) ? _openUserSubjects.OBTMARKSCC : "0") : "0";
                                        ViewData["Sub_" + i + "_CCE_Min"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MINMARKSCC.Length > 0) ? _openUserSubjects.MINMARKSCC : "0") : "0";
                                        ViewData["Sub_" + i + "_CCE_Max"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MAXMARKSCC.Length > 0) ? _openUserSubjects.MAXMARKSCC : "0") : "0";
                                    }
                                    else
                                    {
                                        ViewData["Subject_Add_" + (i - 6)] = _openUserSubjects.SUB;
                                    }
                                }
                                else
                                {
                                    ViewData["Subject_Add_" + (i - 5)] = _openUserSubjects.SUB;
                                    if (i == 6)
                                    {
                                        ViewData["Subject_" + i] = (stream.Contains("COMMERCE")) ? _openUserSubjects.SUB : "";
                                        ViewData["Sub_" + i + "_Th_Obt"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.OBTMARKS.Length > 0) ? _openUserSubjects.OBTMARKS : "0") : "0";
                                        ViewData["Sub_" + i + "_Th_Min"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MINMARKS.Length > 0) ? _openUserSubjects.MINMARKS : "0") : "0";
                                        ViewData["Sub_" + i + "_Th_Max"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MAXMARKS.Length > 0) ? _openUserSubjects.MAXMARKS : "0") : "0";
                                        ViewData["Sub_" + i + "_Pr_Obt"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.OBTMARKSP.Length > 0) ? _openUserSubjects.OBTMARKSP : "0") : "0";
                                        ViewData["Sub_" + i + "_Pr_Min"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MINMARKSP.Length > 0) ? _openUserSubjects.MINMARKSP : "0") : "0";
                                        ViewData["Sub_" + i + "_Pr_Max"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MAXMARKSP.Length > 0) ? _openUserSubjects.MAXMARKSP : "0") : "0";
                                        ViewData["Sub_" + i + "_CCE_Obt"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.OBTMARKSCC.Length > 0) ? _openUserSubjects.OBTMARKSCC : "0") : "0";
                                        ViewData["Sub_" + i + "_CCE_Min"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MINMARKSCC.Length > 0) ? _openUserSubjects.MINMARKSCC : "0") : "0";
                                        ViewData["Sub_" + i + "_CCE_Max"] = (stream.Contains("COMMERCE")) ? ((_openUserSubjects.MAXMARKSCC.Length > 0) ? _openUserSubjects.MAXMARKSCC : "0") : "0";
                                    }
                                }
                            }

                        }

                        if (ViewData["Subject_Add_1"] == null)
                        { ViewData["Subject_Add_1"] = ""; }
                        if (ViewData["Subject_Add_2"] == null)
                        { ViewData["Subject_Add_2"] = ""; }
                        if (ViewData["Subject_6"] == null)
                        {
                            ViewData["Subject_6"] = string.Empty;
                            ViewData["Sub_6_Th_Obt"] = "0";
                            ViewData["Sub_6_Th_Min"] = "0";
                            ViewData["Sub_6_Th_Max"] = "0";
                            ViewData["Sub_6_Pr_Obt"] = "0";
                            ViewData["Sub_6_Pr_Min"] = "0";
                            ViewData["Sub_6_Pr_Max"] = "0";
                            ViewData["Sub_6_CCE_Obt"] = "0";
                            ViewData["Sub_6_CCE_Min"] = "0";
                            ViewData["Sub_6_CCE_Max"] = "0";
                        }

                        if (_openUserLogin.CATEGORY.ToUpper().Contains("12TH FAIL (REGULAR"))
                        {
                            if (_openUserRegistration.BOARD.ToUpper().Contains("P.S.E.B"))
                            {
                                int appSession = Convert.ToInt32(Session["app_session"].ToString());

                                if (DateTime.Now.Year - 5 <= appSession)//earlier it was 2
                                {
                                    ViewBag.visible = "true";
                                    ViewBag.maxCC = "2";
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("OpenSubjectCorrectionPerforma");
                    }
                    //--------------------------------------------Grid Start-----------//
                    string schlid = Session["SCHL"].ToString();

                    DataSet seleLastCan = objDB.PendingCorrectionSubjects(schlid, "22,44");
                    if (seleLastCan.Tables[0].Rows.Count > 0)
                    {
                        @ViewBag.message = "1";
                        _openUserRegistration.StoreAllData = seleLastCan;
                        ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                        for (int i = 0; i < seleLastCan.Tables[0].Rows.Count; i++)
                        {

                            if (seleLastCan.Tables[0].Rows[i]["DiffSub"].ToString() == null || seleLastCan.Tables[0].Rows[i]["DiffSub"].ToString() == "")
                            {
                                string candid = seleLastCan.Tables[0].Rows[i]["candid"].ToString();
                                string subNew = seleLastCan.Tables[0].Rows[i]["Newsubcode"].ToString();
                                string subOld = seleLastCan.Tables[0].Rows[i]["Oldsubcode"].ToString();
                                string DiffSub = string.Empty;

                                //modified
                                //Response.Write("modified");
                                if (subNew.Length == subOld.Length)
                                {
                                    var list = subOld.Split(' ').Where(x => (!subNew.Split(' ').Contains(x))).ToList();
                                    int count = list.Count;
                                    foreach (var item in list)
                                    {
                                        DiffSub = item + " Modified";
                                    }
                                }
                                ////Removed
                                if (subNew.Length > subOld.Length)
                                {
                                    //Response.Write("Added...");
                                    var list1 = subNew.Split(' ').Where(x => (!subOld.Split(' ').Contains(x))).ToList();
                                    int count1 = list1.Count;
                                    foreach (var item in list1)
                                    {
                                        DiffSub = item + " Added";
                                    }
                                }

                                string DiffSubCorr = objDB.InsertDiffSubjects(candid, DiffSub);

                            }
                        }



                    }
                    else
                    {
                        @ViewBag.message = "Record Not Found";
                    }
                    //---------------------------------------------Grid End-----------//

                    //--------Start After Student Details Grid Details----------

                    ViewBag.Boards = openDB.GetN2Board();
                    ViewBag.Months = openDB.GetMonths();
                    ViewBag.Years = openDB.GetYears();

                    _openUserLogin = openDB.GetLoginById(appno);
                    ViewBag.subStatus = openDB.IsUserInSubjects(_openUserLogin.ID.ToString()).ToString();

                    if (Session["app_class"].ToString() == "10")
                    {
                        ViewBag.categories = openDB.GetMCategories();
                        _openUserRegistration.CATEGORY = _openUserLogin.CATEGORY;
                    }
                    else
                    {
                        ViewBag.categories = openDB.GetTCategories();
                        _openUserRegistration.CATEGORY = _openUserLogin.CATEGORY;

                        if (_openUserLogin.CATEGORY.ToUpper() == ("12th FAIL (Regular School-Science Group)").ToUpper())
                        {
                            ViewBag.streams = openDB.GetStreams12_1();
                            _openUserRegistration.STREAM = _openUserLogin.STREAM;
                        }
                        else
                        {
                            ViewBag.streams = openDB.GetStreams12_2();
                            _openUserRegistration.STREAM = _openUserLogin.STREAM;
                        }


                    }

                    List<SelectListItem> casts = objCommon.GetCaste();
                    casts.RemoveAll(r => r.Text.Contains("SC("));
                    ViewBag.Cast = casts;

                    //if (_openUserLogin.CATEGORY.ToLower().Contains("direct"))
                    //{
                    //    ViewBag.disableBoard = "true";
                    //}
                    //else
                    //{
                    //    ViewBag.disableBoard = "false";
                    //}

                    ViewBag.disableBoard = "false";

                    if (_openUserLogin.CATEGORY == "10TH PASSED")
                    {
                        //  List<SelectListItem> yearlist = openDB.GetYears();
                        //yearlist.Remove(yearlist[1]);
                        List<SelectListItem> yearlist = new AbstractLayer.DBClass().GetSessionYearSchoolAdmin();
                        yearlist.Remove(yearlist[0]);
                        //yearlist.Remove(yearlist[0]);
                        ViewBag.Years = yearlist;
                    }

                    try
                    {
                        if (_openUserRegistration.CASTE != "")
                        {
                            SelectListItem item = casts.Find(f => f.Value.ToUpper() == _openUserRegistration.CASTE.ToUpper());
                            casts.Remove(item);
                            item.Selected = true;
                            casts.Add(item);
                            ViewBag.Cast = casts;
                        }
                    }
                    catch (Exception) { }

                    if (_openUserRegistration == null)
                    {
                        _openUserRegistration = new OpenUserRegistration();
                        _openUserRegistration.APPNO = appno;
                    }
                    else
                    {
                        if (_openUserRegistration.OSESSION != null && _openUserRegistration.OSESSION != string.Empty)
                        {
                            string[] osession = _openUserRegistration.OSESSION.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            ViewBag.month = osession[0];
                            ViewBag.year = osession[1];
                        }
                    }
                    //--------End After Student Details Grid Details----------

                    return View(_openUserRegistration);
                }
            }

            #endregion Search

            #region SAVE Details
            if (cmd == "SAVE Details")
            {
                //OpenUserLogin _openUserLogin = openDB.GetRecord(appno);
                OpenUserLogin _openUserLogin = openDB.GetRecordCorr(Session["app_id"].ToString(), Session["SCHL"].ToString());
                if (_openUserLogin == null)
                { _openUserLogin = new OpenUserLogin(); }
                else if (_openUserLogin.APPNO.ToString() == "0") { return RedirectToAction("OpenSubjectCorrectionPerforma"); }
                else
                {
                    //ViewBag.cnt = 1;
                    Session["app_no"] = _openUserLogin.APPNO.ToString();
                    Session["app_name"] = _openUserLogin.NAME.ToString();
                    Session["app_id"] = _openUserLogin.ID.ToString();
                    ViewBag.Class = Session["app_class"] = _openUserLogin.CLASS.ToString();
                    //Session["app_stream"] = _openUserLogin.STREAM.ToString();
                    Session["app_mob"] = _openUserLogin.MOBILENO.Trim();
                    Session["app_email"] = _openUserLogin.EMAILID.Trim().ToUpper();
                    Session["app_adrs"] = _openUserLogin.ADDRESS.Trim().ToUpper();
                    Session["app_adhr"] = _openUserLogin.AADHAR_NO.Trim();

                }
                int regStatus = openDB.IsUserInReg(_openUserLogin.ID.ToString());
                Session["regStatus"] = regStatus.ToString();
                Session["app_session"] = "0";
                Session["CentreStatus"] = "0";
                Session["subStatus"] = openDB.IsUserInSubjects(_openUserLogin.ID.ToString()).ToString();
                Session["payVerify"] = "0";
                ViewBag.regStatus = regStatus.ToString();

                if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
                {
                    return RedirectToAction("OpenSubjectCorrectionPerforma");
                }
                string app_class = Session["app_class"].ToString();
                string app_id = Session["app_id"].ToString();
                string app_stream = Session["app_stream"].ToString();

                string app_Board = Session["BOARD"].ToString();
                string app_Category = Session["Category"].ToString();
                string app_Month = Session["Month"].ToString();
                string app_Year = Session["Year"].ToString();


                string[] subjects_array = new string[10];
                string[] subAbbr_array = new string[10];
                string[] subcat_array = new string[10];
                List<SelectListItem> subjects = openDB.GetMatricSubjects_2();
                if (fc["Subject_1"] != null && fc["Subject_1"] != string.Empty)
                {
                    subjects_array[0] = fc["Subject_1"].ToString();
                    subcat_array[0] = Sub_1_Chk == "on" ? "C" : "R";
                }
                else
                {
                    return RedirectToAction("Subjects");
                }
                if (fc["Subject_2"] != null && fc["Subject_2"] != string.Empty)
                {
                    subjects_array[1] = fc["Subject_2"].ToString();
                    subcat_array[1] = Sub_2_Chk == "on" ? "C" : "R";
                }
                else
                {
                    return RedirectToAction("Subjects");
                }
                if (fc["Subject_3"] != null && fc["Subject_3"] != string.Empty)
                {
                    subjects_array[2] = fc["Subject_3"].ToString();
                    subcat_array[2] = Sub_3_Chk == "on" ? "C" : "R";
                }
                else
                {
                    return RedirectToAction("Subjects");
                }
                if (fc["Subject_4"] != null && fc["Subject_4"] != string.Empty)
                {
                    subjects_array[3] = fc["Subject_4"].ToString();
                    subcat_array[3] = Sub_4_Chk == "on" ? "C" : "R";
                }
                else
                {
                    return RedirectToAction("Subjects");
                }
                if (fc["Subject_5"] != null && fc["Subject_5"] != string.Empty)
                {
                    subjects_array[4] = fc["Subject_5"].ToString();
                    subcat_array[4] = Sub_5_Chk == "on" ? "C" : "R";
                }
                else
                {
                    return RedirectToAction("Subjects");
                }
                if (fc["Subject_6"] != null && fc["Subject_6"] != string.Empty)
                {
                    subjects_array[5] = fc["Subject_6"].ToString();
                    subcat_array[5] = Sub_6_Chk == "on" ? "C" : "R";
                }
                else
                {
                    if (app_stream.Contains("10") || app_class.Contains("10"))
                    {
                        return View();
                    }
                }
                if (fc["Subject_Add_1"] != null && fc["Subject_Add_1"] != string.Empty)
                {
                    subjects_array[6] = fc["Subject_Add_1"].ToString();
                    subcat_array[6] = "A";
                }
                if (fc["Subject_Add_2"] != null && fc["Subject_Add_2"] != string.Empty)
                {
                    subjects_array[7] = fc["Subject_Add_2"].ToString();
                    subcat_array[7] = "A";
                }
                //

                // Check  language subjects for 12th
                OpenUserRegistration _openUserRegistration = openDB.GetRegistrationRecord(Session["app_id"].ToString());

                if (app_class == "12")
                {
                    if (subjects_array.Contains("004") && subjects_array.Contains("005"))
                    {
                        ViewData["Result"] = "ML1";
                        return View();
                    }

                    if (subjects_array.Contains("036") && subjects_array.Contains("037") && subjects_array.Contains("038"))
                    {
                        ViewData["Result"] = "ML2";
                        return View();
                    }

                    if (_openUserRegistration.CLASS == "12" && _openUserRegistration.CAT.ToUpper().Contains("NIOS"))
                    {
                        int flag = 0;
                        if (subjects_array.Contains("001"))
                        {
                            int index = Array.IndexOf(subjects_array, subjects_array.Where(x => x.Contains("001")).FirstOrDefault());
                            if (subcat_array[index].Contains("C"))
                            {
                                flag = 1;
                            }
                        }

                        if (subjects_array.Contains("002"))
                        {
                            int index = Array.IndexOf(subjects_array, subjects_array.Where(x => x.Contains("002")).FirstOrDefault());
                            if (subcat_array[index].Contains("C"))
                            {
                                if (flag == 1) { flag = 2; }
                                else { flag = 1; }

                            }
                        }

                        if (flag == 2)
                        {
                            ViewData["Result"] = "12NA";
                            return View();
                        }

                    }
                }
                else if (app_class == "10")
                {
                    //eng- 02, hindi - 03,sankri - 09,urdu (elective)-10
                    if (subjects_array.Contains("02") && subjects_array.Contains("03") && subjects_array.Contains("09") && subjects_array.Contains("10"))
                    {
                        ViewData["Result"] = "ML3";
                        return View();
                    }

                    if (_openUserRegistration.CLASS == "10" && _openUserRegistration.CAT.ToUpper().Contains("NIOS")) // change by gulab
                    {
                        //if (subjects_array.Contains("01") || subjects_array.Contains("72"))
                        //{
                        //    ViewData["Result"] = "10NA";
                        //    return View();
                        //}
                        int flag = 0;
                        if (subjects_array.Contains("01"))
                        {
                            int index = Array.IndexOf(subjects_array, subjects_array.Where(x => x.Contains("01")).FirstOrDefault());
                            if (subcat_array[index].Contains("C"))
                            {
                                flag = 1;
                            }
                        }

                        if (subjects_array.Contains("72"))
                        {
                            int index = Array.IndexOf(subjects_array, subjects_array.Where(x => x.Contains("72")).FirstOrDefault());
                            if (subcat_array[index].Contains("C"))
                            {
                                if (flag == 1) { flag = 2; }
                                else { flag = 1; }

                            }
                        }

                        if (flag == 2)
                        {
                            ViewData["Result"] = "10NA";
                            return View();
                        }
                    }
                }

                //

                try
                {
                    // Check Duplicacy   
                    bool CheckStatus = AbstractLayer.StaticDB.CheckArrayDuplicates(subjects_array);
                    // bool CheckStatus = CheckArrayDuplicates(subjects_array);
                    if (CheckStatus == false)
                    {


                        DataSet sResult = openDB.IsUserInSubjectsCORR(Session["app_id"].ToString());
                        if (sResult != null)
                        {
                            if (sResult.Tables[0].Rows[0]["res"].ToString() == "1")
                            {
                                int cc = subjects_array.Count(s => s != null);
                                if (cc > 4)
                                {
                                    Session["subStatus"] = sResult;
                                    // openDB.InsertOpenCorrSubjects(subjects_array, app_class, app_id, app_stream, schl, fc, app_Board, app_Category, app_Month, app_Year);
                                    openDB.InsertOpenCorrSubjectsNew(subjects_array, subcat_array, app_class, app_id, app_stream, schl, fc, app_Board, app_Category, app_Month, app_Year);

                                    ViewData["Result"] = "1";
                                }
                                else
                                {
                                    ViewData["Result"] = "2";
                                }
                            }
                            else
                            {
                                ViewData["Result"] = sResult.Tables[0].Rows[0]["res"].ToString();
                            }
                        }



                    }
                    else
                    {
                        ViewData["Result"] = "2";
                    }
                    return View();
                    // return RedirectToAction("Applicationstatus");
                }
                catch (Exception e)
                { ViewData["Result"] = "-1"; return View(); }
            }
            #endregion SAVE Details
            else
            {
                return RedirectToAction("Logout", "Login");
            }



        }

        public JsonResult GetStreams(string category)
        {
            List<SelectListItem> streams = new List<SelectListItem>();
            if (category == "12th FAIL (Regular School-Science Group)")
            {
                streams = openDB.GetStreams12_1();
            }
            //else if (category == "12th FAIL (Open School-AllGroups)" || category == "10th PASSED" || category == "12th FAIL (NIOS-All Groups)" || category == "12th FAIL (Regular School-Other Groups)")
            else
            {
                streams = openDB.GetStreams12_2();
            }

            ViewBag.streams = streams;

            return Json(streams);
        }
        public JsonResult getCredit(string text, string st)
        {
            OpenUserRegistration _openUserRegistration = openDB.GetRegistrationRecord(Session["app_id"].ToString());
            OpenUserLogin _openUserLogin = openDB.GetLoginById(Session["app_id"].ToString());
            ViewBag.visible = "false";
            if (_openUserLogin.CATEGORY.ToUpper().Contains("12TH FAIL (REGULAR"))
            {
                if (_openUserRegistration.BOARD.ToUpper().Contains("P.S.E.B"))
                {
                    //int appSession = Convert.ToInt32(Session["app_session"].ToString());
                    int appSession = 2009;//Convert.ToInt32(st);
                    if (DateTime.Now.Year - 5 <= appSession)//earlier it was 2
                    {
                        ViewBag.visible = "true";
                        ViewBag.maxCC = "2";
                    }
                }
            }
            return Json(ViewBag.visible);
        }
        #endregion Open Subject Correction




        #region Admin ImageCorrectionPerforma

        [AdminLoginCheckFilter]
        public ActionResult AdminImageCorrectionPerforma(RegistrationModels rm)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

            try
            {
                Session["SCHL"] = "9999999";

                //ViewBag.SelectedItem
                AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                //RegistrationModels rm = new RegistrationModels();
                string schlid = "";
                if (Session["AdminType"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                else
                {
                    schlid = Session["SCHL"].ToString();
                    ViewBag.message = "Record Not Found";                                     
                }
                DataSet result = objDB.schooltypesCorrection(schlid, "I"); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {

                    ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                    ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
                    ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                    ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
                    ViewBag.N3M1threclock = result.Tables[1].Rows[0]["Nth"].ToString();
                    ViewBag.E1T1threclock = result.Tables[1].Rows[0]["Eth"].ToString();

                    DateTime sDateN = Convert.ToDateTime(result.Tables[6].Rows[0]["sDate"]);
                    DateTime eDateN = Convert.ToDateTime(result.Tables[6].Rows[0]["eDate"]);

                    DateTime sDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["sDate"]);
                    DateTime eDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["eDate"]);

                    DateTime sDateE = Convert.ToDateTime(result.Tables[6].Rows[2]["sDate"]);
                    DateTime eDateE = Convert.ToDateTime(result.Tables[6].Rows[2]["eDate"]);

                    DateTime sDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["sDate"]);
                    DateTime eDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["eDate"]);

                    DateTime sDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["sDate"]);
                    DateTime eDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["eDate"]);

                    DateTime sDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["sDate"]);
                    DateTime eDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["eDate"]);

                    DateTime dtTodate = Convert.ToDateTime(DateTime.Today);

                    //Bind Class asign by admin id
                    ViewBag.MySch = AbstractLayer.DBClass.GetCorrectionClassAssignListByAdminId(Session["ClassAssign"].ToString()).ToList();

                }

                if (ModelState.IsValid)
                {
                    return View(rm);
                }
                else
                { return View(rm); }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult AdminImageCorrectionPerforma(RegistrationModels rm, FormCollection frm, string cmd)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

            string sid = Convert.ToString(rm.Std_id);
            string formname = null;
            if (rm.SelList == "2")
            {
                formname = "M";
            }
            if (rm.SelList == "22")
            {
                formname = "MO";
            }
            if (rm.SelList == "4")
            {
                formname = "T";
            }
            if (rm.SelList == "44")
            {
                formname = "TO";
            }
            if (rm.SelList == "9")
            {
                formname = "Nth";
            }
            if (rm.SelList == "11")
            {
                formname = "Eth";
            }

            ViewBag.SelectedItem = frm["SelList"];

            rm.SCHL = frm["SCHL"].ToString();
            Session["SCHL"] = frm["SCHL"].ToString();
            ViewBag.SCHLstring = rm.SCHL;

            string schlcode = Session["SCHL"].ToString();
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            DataSet result = objDB.schooltypesCorrection(schlcode, "I"); // passing Value to DBClass from model
            if (result.Tables[1].Rows.Count > 0)
            {

                ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
                ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
                ViewBag.N3M1threclock = result.Tables[1].Rows[0]["Nth"].ToString();
                ViewBag.E1T1threclock = result.Tables[1].Rows[0]["Eth"].ToString();

                DateTime sDateN = Convert.ToDateTime(result.Tables[6].Rows[0]["sDate"]);
                DateTime eDateN = Convert.ToDateTime(result.Tables[6].Rows[0]["eDate"]);

                DateTime sDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["sDate"]);
                DateTime eDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["eDate"]);

                DateTime sDateE = Convert.ToDateTime(result.Tables[6].Rows[2]["sDate"]);
                DateTime eDateE = Convert.ToDateTime(result.Tables[6].Rows[2]["eDate"]);

                DateTime sDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["sDate"]);
                DateTime eDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["eDate"]);

                DateTime sDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["sDate"]);
                DateTime eDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["eDate"]);

                DateTime sDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["sDate"]);
                DateTime eDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["eDate"]);

                DateTime dtTodate = Convert.ToDateTime(DateTime.Today);

           
                ViewBag.MySch = AbstractLayer.DBClass.GetCorrectionClassAssignListByAdminId(Session["ClassAssign"].ToString()).ToList();

            }
            #region View All Correction Pending Record
            if (cmd == "View All Correction Pending Record")
            {


                DataSet seleLastCan = objDB.PendingPhotoSignCorrection(schlcode);
                if (seleLastCan.Tables[0].Rows.Count > 0)
                {

                    @ViewBag.message = "1";
                    rm.StoreAllData = seleLastCan;
                    ViewBag.TotalViewAllCount = seleLastCan.Tables[0].Rows.Count;
                    ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                }
                else
                {
                    @ViewBag.message = "Record Not Found";
                }


                return View(rm);
            }
            #endregion View All Correction Pending Record

            #region View All Record Images
            else if (cmd == "View All Correction Record")
            {
                DataSet seleLastCan = objDB.ViewAllPhotoSignCorrection(schlcode);
                if (seleLastCan.Tables[0].Rows.Count > 0)
                {
                    @ViewBag.message = "1";
                    rm.StoreAllData = seleLastCan;
                    ViewBag.TotalViewAllCount = seleLastCan.Tables[0].Rows.Count;
                    ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                }
                else
                {
                    @ViewBag.message = "Record Not Found";
                }


                return View(rm);
            }
            #endregion View All Record Images

            #region Add Record Region Begin
            else if (cmd == "Add Record")
            {
                //string stdPic, stdSign;
                rm.Std_id = Convert.ToInt32(frm["Std_id"]);
                rm.Class = frm["SelList"];
                rm.Correctiontype = frm["SelListField"];
                string filepathtosave = "";
                DataSet ds = objDB.PhotoSignSearchCorrectionStudentDetails(formname, schlcode, sid);

                if (rm.Correctiontype == "Photo")
                {
                    if (rm.std_Photo != null)
                    {
                        stdPic = Path.GetFileName(rm.std_Photo.FileName);
                    }
                    //var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/ImageCorrection/New/Photo"), rm.Std_id + "P" + ".jpg");

                    ////var pathOld = @"\\10.10.10.113\Nucleus\live.psebonline.in\www\upload\Upload2017\" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                    //var pathOld = "https://registration2022.pseb.ac.in/Upload/Upload2023/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                    //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/ImageCorrection/New/Photo"));
                    //string FilepathExistOld = Path.Combine(Server.MapPath("~/Upload/Upload2023/ImageCorrection/Old/Photo"));
                    //if (!Directory.Exists(FilepathExist))
                    //{
                    //    Directory.CreateDirectory(FilepathExist);
                    //}
                    //if (!Directory.Exists(FilepathExistOld))
                    //{
                    //    Directory.CreateDirectory(FilepathExistOld);
                    //}

                    //rm.std_Photo.SaveAs(path);
                    string Orgfile = rm.Std_id + "P" + ".jpg";
                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = rm.file.InputStream,
                                Key = string.Format("allfiles/Upload2023/ImageCorrection/New/Photo/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }

                    //filepathtosave = "../Upload/Upload2023/ImageCorrection/New/Photo/" + rm.Std_id + "P" + ".jpg";
                    filepathtosave = "allfiles/Upload2023/ImageCorrection/New/Photo/" + rm.Std_id + "P" + ".jpg";
                    ViewBag.ImageURL = filepathtosave;
                    //string PhotoName = "/Upload/Upload2023/ImageCorrection/New/Photo" + "/" + rm.Std_id + "P" + ".jpg";
                    string PhotoName = "allfiles/Upload2023/ImageCorrection/New/Photo" + "/" + rm.Std_id + "P" + ".jpg";
                    rm.oldVal = frm["imgPhotoOld"];
                    rm.newVal = PhotoName;

                    //System.IO.File.Copy(pathOld, FilepathExistOld);
                }
                else if (rm.Correctiontype == "Sign")
                {
                    if (rm.std_Sign != null)
                    {
                        stdPic = Path.GetFileName(rm.std_Sign.FileName);
                    }
                    //var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/ImageCorrection/New/Sign"), rm.Std_id + "S" + ".jpg");
                    ////var pathOld = @"\\10.10.10.113\Nucleus\live.psebonline.in\www\upload\Upload2017\" + ds.Tables[0].Rows[0]["std_Sign"].ToString();
                    //var pathOld = "https://registration2022.pseb.ac.in/Upload/Upload2023/" + ds.Tables[0].Rows[0]["std_Sign"].ToString();
                    //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/ImageCorrection/New/Sign"));
                    //string FilepathExistOld = Path.Combine(Server.MapPath("~/Upload/Upload2023/ImageCorrection/Old/Sign"));
                    //if (!Directory.Exists(FilepathExist))
                    //{
                    //    Directory.CreateDirectory(FilepathExist);
                    //}
                    //if (!Directory.Exists(FilepathExistOld))
                    //{
                    //    Directory.CreateDirectory(FilepathExistOld);
                    //}


                    //rm.std_Sign.SaveAs(path);
                    string Orgfile = rm.Std_id + "S" + ".jpg";
                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = rm.std_Sign.InputStream,
                                Key = string.Format("allfiles/Upload2023/ImageCorrection/New/Sign/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }

                    //filepathtosave = "../Upload/Upload2023/ImageCorrection/New/Sign/" + rm.Std_id + "S" + ".jpg";
                    filepathtosave = "allfiles/Upload2023/ImageCorrection/New/Sign/" + rm.Std_id + "S" + ".jpg";
                    ViewBag.ImageURL = filepathtosave;
                    //string SignName = "/Upload/Upload2023/ImageCorrection/New/Sign" + "/" + rm.Std_id + "S" + ".jpg";
                    string SignName = "allfiles/Upload2023/ImageCorrection/New/Sign/" + "/" + rm.Std_id + "S" + ".jpg";
                    rm.oldVal = frm["imgSignOld"];
                    rm.newVal = SignName;
                    rm.oldVal = frm["imgSignOld"];
                    rm.newVal = SignName;

                    //System.IO.File.Copy(pathOld, FilepathExistOld);
                }
                else
                {
                    rm.newVal = "NULL";
                }

                rm.Remark = frm["Remark"];
               
                ViewBag.Searchstring = frm["Std_id"];
                rm.SCHL = Session["SCHL"].ToString();



                string result1 = objDB.InsertPhotoSignCorrectionAdd(rm, frm, adminLoginSession.USERNAME,adminLoginSession.AdminEmployeeUserId);
                if (result1 == "-1")
                {
                    ViewData["Status"] = "0";
                }
                else
                {
                    ViewData["Status"] = "1";
                }

                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "Photo", Value = "Photo" });
                items.Add(new SelectListItem { Text = "Sign", Value = "Sign" });
                ViewBag.MySchField = new SelectList(items, "Value", "Text");
                DataSet seleLastCanPending = objDB.PendingPhotoSignCorrection(schlcode);
                if (seleLastCanPending.Tables[0].Rows.Count > 0)
                {
                    @ViewBag.message = "1";
                    rm.StoreAllData = seleLastCanPending;
                    ViewBag.TotalViewAllCount = seleLastCanPending.Tables[0].Rows.Count;
                    ViewBag.TotalCount = seleLastCanPending.Tables[0].Rows.Count;
                }
                return View(rm);
            }
            #endregion Add Record

            #region Final Submit
            else if (cmd == "Final Submit Correction")
            {
                if (Session["SCHL"] != null)
                {

                    rm.SCHL = Session["SCHL"].ToString();
                    rm.Correctiontype = "04";
                    string resultFS = objDB.FinalSubmitImageCorrection(rm); // passing Value to DBClass from model
                    if (Convert.ToInt16(resultFS) > 0)
                    {
                        ViewData["resultFS"] = resultFS;                   
                    }
                    else
                    {
                        ViewData["resultFS"] = "";                     
                    }
                }
               
                return RedirectToAction("AdminImageCorrectionPerforma", "CorrectionSubjects");
            }
            #endregion Final Submit

            #region Else Region
            else
            {
                try
                {
                    DataSet seleLastCanPending = objDB.PendingPhotoSignCorrection(schlcode);
                    if (seleLastCanPending.Tables[0].Rows.Count > 0)
                    {
                        @ViewBag.message = "1";
                        rm.StoreAllData = seleLastCanPending;
                        ViewBag.TotalViewAllCount = seleLastCanPending.Tables[0].Rows.Count;
                        ViewBag.TotalCount = seleLastCanPending.Tables[0].Rows.Count;
                    }

                    DataSet seleLastCan = objDB.PhotoSignSearchCorrectionStudentDetails(formname, schlcode, sid);
                    if (seleLastCan.Tables[0].Rows.Count > 0)
                    {
                        @ViewBag.TotalCountSearch = seleLastCan.Tables[0].Rows.Count;
                        List<SelectListItem> items = new List<SelectListItem>();
                        items.Add(new SelectListItem { Text = "Photo", Value = "Photo" });
                        items.Add(new SelectListItem { Text = "Sign", Value = "Sign" });

                        ViewBag.MySchField = new SelectList(items, "Value", "Text");

                        @ViewBag.message = "1";

                        @ViewBag.stdid = seleLastCan.Tables[0].Rows[0]["std_id"].ToString();
                        @ViewBag.Oroll = seleLastCan.Tables[0].Rows[0]["Board_Roll_Num"].ToString();
                        @ViewBag.Regno = seleLastCan.Tables[0].Rows[0]["Registration_num"].ToString();
                        @ViewBag.category = seleLastCan.Tables[0].Rows[0]["category"].ToString();
                        @ViewBag.session = seleLastCan.Tables[0].Rows[0]["Year"].ToString() + "-" + seleLastCan.Tables[0].Rows[0]["Month"].ToString();
                        @ViewBag.canName = seleLastCan.Tables[0].Rows[0]["Candi_Name"].ToString();
                        @ViewBag.FName = seleLastCan.Tables[0].Rows[0]["Father_Name"].ToString();
                        @ViewBag.Mname = seleLastCan.Tables[0].Rows[0]["Mother_Name"].ToString();
                        @ViewBag.lot = seleLastCan.Tables[0].Rows[0]["lot"].ToString();
                        @ViewBag.DOB = seleLastCan.Tables[0].Rows[0]["DOB"].ToString();
                        @ViewBag.Frm = formname;
                        @ViewBag.Subjlist = seleLastCan.Tables[0].Rows[0]["SubjectList"].ToString();
                        if (formname == "MO" || formname == "TO")
                        {
                            @ViewBag.PhotoOld = seleLastCan.Tables[0].Rows[0]["Std_photo"].ToString();
                            @ViewBag.SignOld = seleLastCan.Tables[0].Rows[0]["Std_Sign"].ToString();
                        }
                        else
                        {
                            @ViewBag.PhotoOld = seleLastCan.Tables[0].Rows[0]["Std_photo"].ToString();
                            @ViewBag.SignOld = seleLastCan.Tables[0].Rows[0]["Std_Sign"].ToString();
                        }
                        @ViewBag.CandPhotoFullPath = seleLastCan.Tables[0].Rows[0]["CandPhotoFullPath"].ToString();
                        @ViewBag.CandSignFullPath = seleLastCan.Tables[0].Rows[0]["CandSignFullPath"].ToString();

                    }
                    else
                    {
                        @ViewBag.message = "Record Not Found";
                        return View(rm);
                    }

                    return View(rm);

                }
                catch (Exception ex)
                {
                    return View(rm);
                }

            }
            #endregion Else Region

        }

        public ActionResult AdminCorrDeletePhotoSignData(string id)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB(); //calling class DBClass
           
            if (id == null)
            {
                return RedirectToAction("AdminImageCorrectionPerforma", "CorrectionSubjects");

            }
            else
            {
                string result = objDB.DeletePhotoSignData(id);
                if (result == "Deleted")
                {
                    @ViewBag.result = "1";

                }

            }
            return RedirectToAction("AdminImageCorrectionPerforma", "CorrectionSubjects");
        }
        #endregion Admin Image  orrectionPerforma

        #region Admin Subject Correction Performa Link

        [AdminLoginCheckFilter]
        public ActionResult AdminSubjectCorrectionPerformaLink()
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

            return View();
        }
        #endregion Subject Correction Performa Link

        //-------------------------Matric Subject Correction---------------------------/
        #region matric subject correctionSRSubjectCorrectionPerforma
        public ActionResult AdminSubjectCorrectionPerforma(RegistrationModels rm)
        {
            ViewBag.btnshow = "0";
            try
            {
                List<SelectListItem> SubWEL = new List<SelectListItem>();
                SubWEL.Add(new SelectListItem { Text = "WELCOME LIFE", Value = "210" });
                ViewBag.SubWEL = new SelectList(SubWEL, "Value", "Text");
                //ViewBag.SelectedItem
                AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                //RegistrationModels rm = new RegistrationModels();
                string schlid = "";
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");

                }
                else
                {
                    //@ViewBag.DA = objCommon.GetDA();
                    //@ViewBag.DAb = objCommon.GetDA();
                    //-----------------------Matric Subjects Start----------------------
                    DataSet ds2 = objDB.ElectiveSubjects();

                    ViewBag.SubS9 = ds2.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                    List<SelectListItem> items = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.SubS9.Rows)
                    {
                        //  items.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        items.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                    }

                    ViewBag.SubS10m = ViewBag.SubS9m = ViewBag.SubS9 = new SelectList(items, "Value", "Text");


                    ViewBag.S10 = new SelectList(items, "Value", "Text");

                    DataSet ds1 = objDB.ElectiveSubjects_Blind();
                    ViewBag.SubSb9 = ds1.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                    List<SelectListItem> bitems = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.SubSb9.Rows)
                    {
                        bitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                    }

                    ViewBag.bs2 = new SelectList(bitems, "Value", "Text");
                    ViewBag.bs3 = new SelectList(bitems, "Value", "Text");
                    ViewBag.bs4 = new SelectList(bitems, "Value", "Text");
                    ViewBag.bs5 = new SelectList(bitems, "Value", "Text");
                    ViewBag.bs6 = new SelectList(bitems, "Value", "Text");
                    ViewBag.bs7 = new SelectList(bitems, "Value", "Text");
                    ViewBag.bs8 = new SelectList(bitems, "Value", "Text");
                    ViewBag.bs9 = new SelectList(bitems, "Value", "Text");



                    //-----------------------------------------------------Nsqf -------------------------
                    string ses = Session["Session"].ToString();
                    string schlcode = Session["SCHL"].ToString();
                    DataSet dsnsqf = objDB.CHkNSQF(schlcode, ses);
                    if (dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "True" || dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "1")
                    {
                        ViewData["NSQFSCHOOL"] = "1";
                    }
                    else
                    {
                        ViewData["NSQFSCHOOL"] = "0";
                    }
                    string nsqfsub = null;
                    DataSet nsresult = objDB.SelectMatricNsqfSub(nsqfsub); // passing Value to DBClass from model
                    ViewBag.nsfq = nsresult.Tables[0];
                    List<SelectListItem> nsfqList = new List<SelectListItem>();
                    //nsfqList.Add(new SelectListItem { Text = "NSFQ Subjects", Value = "0" });
                    nsfqList.Add(new SelectListItem { Text = "NO", Value = "NO" });
                    foreach (System.Data.DataRow dr in ViewBag.nsfq.Rows)
                    {
                        nsfqList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["SUB"].ToString() });
                    }
                    ViewBag.nsfqList = new SelectList(nsfqList, "Value", "Text");
                    //-----------------------------------------------------End Nsqf -------------------------

                    List<SelectListItem> itemsub6 = new List<SelectListItem>();
                    itemsub6.Add(new SelectListItem { Text = "SOCIAL STUDIES", Value = "06" });
                    ViewBag.SubS6 = new SelectList(itemsub6, "Value", "Text");

                    List<SelectListItem> itemsub10 = new List<SelectListItem>();
                    itemsub10.Add(new SelectListItem { Text = "HINDI", Value = "03" });
                    itemsub10.Add(new SelectListItem { Text = "SOCIAL STUDIES", Value = "06" });
                    ViewBag.SubS10 = new SelectList(itemsub10, "Value", "Text");

                    ViewBag.BM = ViewBag.BM2m = ViewBag.BM3m = ViewBag.BM4m = ViewBag.BM5m = ViewBag.BM6m = ViewBag.BM7m = ViewBag.BM8m = ViewBag.BM9m = objCommon.GetMediumAll();
                    //----------------------Matric Subjects End---------------------------


                    schlid = Session["SCHL"].ToString();

                    DataSet seleLastCan = objDB.PendingCorrectionSubjects(schlid, "2");
                    if (seleLastCan.Tables[0].Rows.Count > 0)
                    {

                        @ViewBag.message = "1";
                        rm.StoreAllData = seleLastCan;
                        ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                    }
                    else
                    {
                        @ViewBag.message = "Record Not Found";
                    }
                    ViewBag.MediumNew = objCommon.GetMediumAll();
                               
                }

             

                DataSet result = objDB.schooltypesCorrection(schlid, "S"); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {
                    ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                    ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
                    ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                    ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();

                    DateTime sDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["sDate"]);
                    DateTime eDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["eDate"]);

                    DateTime sDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["sDate"]);
                    DateTime eDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["eDate"]);

                    DateTime sDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["sDate"]);
                    DateTime eDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["eDate"]);

                    DateTime sDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["sDate"]);
                    DateTime eDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["eDate"]);

                    DateTime dtTodate = Convert.ToDateTime(DateTime.Today);

                    List<SelectListItem> itemsch = new List<SelectListItem>();
                    if (ViewBag.Matric == "1" && dtTodate <= eDateM)
                    {
                        itemsch.Add(new SelectListItem { Text = "Matriculation Regular", Value = "1" });
                    }
                    //if (ViewBag.OMatric == "1" && sDateMO <= eDateMO)
                    //{
                    //    itemsch.Add(new SelectListItem { Text = "Matriculation Open", Value = "2" });
                    //}                   
                    //if (ViewBag.Senior == "1" && sDateT <= eDateT)
                    //{
                    //    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Regular", Value = "3" });
                    //}
                    //if (ViewBag.OSenior == "1" && sDateTO <= eDateTO)
                    //{
                    //    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Open", Value = "4" });
                    //}

                    if (ViewBag.Matric != "1" && ViewBag.OMatric != "1" && ViewBag.Senior != "1" && ViewBag.OSenior != "1")
                    {
                        itemsch.Add(new SelectListItem { Text = "", Value = "" });
                    }
                    ViewBag.MySch = itemsch.ToList();
                }

                if (ModelState.IsValid)
                {
                    return View(rm);
                }
                else
                { return View(rm); }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }
        }
        [HttpPost]
        public ActionResult AdminSubjectCorrectionPerforma(RegistrationModels rm, FormCollection frm, string cmd)
        {
            ViewBag.btnshow = "1";
            List<SelectListItem> SubWEL = new List<SelectListItem>();
            SubWEL.Add(new SelectListItem { Text = "WELCOME LIFE", Value = "210" });
            ViewBag.SubWEL = new SelectList(SubWEL, "Value", "Text");

            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            string sid = Convert.ToString(rm.Std_id);
            string formname = null;
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");

            }
            if (rm.SelList == "1")
            {
                formname = "M";
            }
            if (rm.SelList == "2")
            {
                formname = "MO";
            }
            if (rm.SelList == "3")
            {
                formname = "T";
            }
            if (rm.SelList == "4")
            {
                formname = "TO";
            }

            DataSet ds_chk = objDB.SearchStudentGetByData_SubjectCORR(sid, formname);
            if (ds_chk == null || ds_chk.Tables[0].Rows.Count == 0)
            {
                return RedirectToAction("AdminSubjectCorrectionPerforma", "CorrectionSubjects");
                //return View(rm);
            }

            else if (ds_chk != null && ds_chk.Tables[2].Rows.Count == 1)
            {
                TempData["resultUpdate"] = "5"; // CorrectionPerforma Already exist is is not status is null.
            }

            //@ViewBag.DA = objCommon.GetDA();
            //@ViewBag.DAb = objCommon.GetDA();
            //-----------------------Matric Subjects Start----------------------
            #region M Subjects
            DataSet ds2 = objDB.ElectiveSubjects();

            ViewBag.SubS9 = ds2.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.SubS9.Rows)
            {
                //  items.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                items.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }

            ViewBag.SubS10m = ViewBag.SubS9m = ViewBag.SubS9 = new SelectList(items, "Value", "Text");


            ViewBag.S10 = new SelectList(items, "Value", "Text");

            DataSet ds1 = objDB.ElectiveSubjects_Blind();
            ViewBag.SubSb9 = ds1.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> bitems = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.SubSb9.Rows)
            {
                bitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }

            ViewBag.bs2 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs3 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs4 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs5 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs6 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs7 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs8 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs9 = new SelectList(bitems, "Value", "Text");



            string nsqfsub = null;
            DataSet nsresult = objDB.SelectMatricNsqfSub(nsqfsub); // passing Value to DBClass from model
            ViewBag.nsfq = nsresult.Tables[0];
            List<SelectListItem> nsfqList = new List<SelectListItem>();
            //nsfqList.Add(new SelectListItem { Text = "NSFQ Subjects", Value = "0" });
            nsfqList.Add(new SelectListItem { Text = "NO", Value = "NO" });
            foreach (System.Data.DataRow dr in ViewBag.nsfq.Rows)
            {

                nsfqList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }
            ViewBag.nsfqList = new SelectList(nsfqList, "Value", "Text");
            ViewBag.nsqfcatg = new SelectList(nsfqList, "Value", "Text");


            List<SelectListItem> itemsub6 = new List<SelectListItem>();
            itemsub6.Add(new SelectListItem { Text = "SOCIAL STUDIES", Value = "06" });
            ViewBag.SubS6 = new SelectList(itemsub6, "Value", "Text");

            List<SelectListItem> itemsub10 = new List<SelectListItem>();
            itemsub10.Add(new SelectListItem { Text = "HINDI", Value = "03" });
            itemsub10.Add(new SelectListItem { Text = "SOCIAL STUDIES", Value = "06" });
            ViewBag.SubS10 = new SelectList(itemsub10, "Value", "Text");

            ViewBag.BM = ViewBag.BM1m = ViewBag.BM3m = ViewBag.BM4m = ViewBag.BM5m = ViewBag.BM6m = ViewBag.BM7m = ViewBag.BM8m = ViewBag.BM9m = objCommon.GetMediumAll();
            //RegistrationModels rm = new RegistrationModels();

            if (sid != null)
            {
                DataSet ds = objDB.SearchStudentGetByData_SubjectCORR(sid, formname);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    return RedirectToAction("Logout", "Login");
                }

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 1; i < ds.Tables[1].Rows.Count; i++)
                    {
                        if (ds.Tables[1].Rows[i]["SUB"].ToString() == "72")
                        {
                            //items.Remove(items[i]);
                            ds.Tables[1].Rows.RemoveAt(i);
                        }

                    }
                    for (int i = 1; i < ds.Tables[1].Rows.Count; i++)
                    {
                        if (ds.Tables[1].Rows[i]["SUB"].ToString() == "73")
                        {
                            //items.Remove(items[i]);
                            ds.Tables[1].Rows.RemoveAt(i);
                        }

                    }

                    //-----------------------------------------------------Nsqf -------------------------
                    rm.DA = ds.Tables[0].Rows[0]["Differently_Abled"].ToString();
                    ViewBag.DAb = objCommon.GetDA();
                    ViewBag.DAItem = rm.DA;
                    rm.PreNSQF = ds.Tables[0].Rows[0]["PRE_NSQF_SUB"].ToString();
                    rm.NSQF = ds.Tables[0].Rows[0]["nsqf_flag"].ToString();
                    rm.NsqfsubS6 = ds.Tables[0].Rows[0]["NSQF_SUB"].ToString();

                    string ses = Session["Session"].ToString();
                    string Mschlcode = Session["SCHL"].ToString();
                    DataSet dsnsqf = objDB.CHkNSQF(Mschlcode, ses);


                    if (dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "True" || dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "1")
                    {
                        ViewData["NSQFSCHOOL"] = "1";
                        ViewBag.NSQFSTUDENT = "1";
                    }
                    else
                    {
                        ViewData["NSQFSCHOOL"] = "0";
                        ViewBag.NSQFSTUDENT = "0";
                    }


                    //-----------------------------For NSQF SUBJECTS----------------

                    DataSet isCHkNSQF = objDB.CHkNSQFStudents(sid);

                    if (isCHkNSQF.Tables[0].Rows[0]["NSQF_flag"].ToString() == "True")
                    {

                        ViewBag.SubS9 = ds2.Tables[0];
                        // for dislaying message after saving storing output.
                        // List<SelectListItem> items21 = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.SubS9.Rows)
                        {
                            items.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        }
                        ViewBag.SubS10m = ViewBag.SubS9m = ViewBag.SubS9 = new SelectList(items, "Value", "Text");
                        ViewBag.S10 = new SelectList(items, "Value", "Text");
                        //-----------------------

                        ViewData["NSQFSTUDENT"] = "1";
                        rm.NsqfsubS6 = ds.Tables[0].Rows[0]["NSQF_SUB"].ToString();
                        DataSet nsTextresult = objDB.GetNSQFVIEWSUBJECTMATRICSUBJECT(rm.NsqfsubS6, rm.PreNSQF);
                        List<SelectListItem> nssub6 = new List<SelectListItem>();
                        if (rm.PreNSQF == "NO")
                        {
                            nssub6.Add(new SelectListItem { Text = "NO", Value = "NO" });
                            ViewBag.nsqfcatg = nssub6;
                        }
                        else
                        {
                            if (nsTextresult.Tables[0].Rows.Count > 0)
                            {
                                nssub6.Add(new SelectListItem { Text = "NO", Value = "NO" });
                                nssub6.Add(new SelectListItem { Text = nsTextresult.Tables[0].Rows[0]["Name_ENG"].ToString(), Value = rm.NsqfsubS6 });
                                ViewBag.nsqfcatg = nssub6;
                            }
                            else
                            {

                                nssub6.Add(new SelectListItem { Text = "NO", Value = "NO" });
                                ViewBag.nsqfcatg = nssub6;
                            }

                        }

                    }
                    else
                    {
                        ViewBag.SubS9 = ds2.Tables[0];
                        // for dislaying message after saving storing output.
                        //  List<SelectListItem> items = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.SubS9.Rows)
                        {
                            if (@dr["TYPE"].ToString() == "GRADING SUBJECT")
                            {
                                items.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }

                        }
                        ViewBag.SubS10m = ViewBag.SubS9m = ViewBag.SubS9 = new SelectList(items, "Value", "Text");
                        ViewBag.S10 = new SelectList(items, "Value", "Text");
                        //--------------
                        List<SelectListItem> nssub6 = new List<SelectListItem>();
                        nssub6.Add(new SelectListItem { Text = "NO", Value = "NO" });
                        ViewBag.nsqfcatg = nssub6;
                    }


                    //--------------------------------End NSQF SUBJECTS-------------

                    //-----------------------------For NSQF SUBJECTS--------------

                    //------------------------------Fill Subjects----------------//


                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                        {
                            if (rm.DA == "N.A.")
                            {
                                if (i == 0)
                                {
                                    rm.subS1 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.subm1 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 1)
                                {
                                    rm.subS2 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.subM2 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 2)
                                {
                                    rm.subS3 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.subm3 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 3)
                                {
                                    rm.subS4 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.subM4 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 4)
                                {
                                    rm.subS5 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.subM5 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 5)
                                {
                                    rm.subS6 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.subM6 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                    rm.NsqfsubS6Upd = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    //rm.NsqfsubS6 = ds.Tables[1].Rows[5]["SUB"].ToString();
                                    rm.NsqfsubS6 = ds.Tables[0].Rows[0]["NSQF_SUB"].ToString();
                                }
                                else if (i == 6)
                                {
                                    rm.subS7 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.subM7 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 7)
                                {
                                    rm.subS8 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.subM8 = ds.Tables[1].Rows[i]["MEDIUM"].ToString();
                                }
                                else if (i == 8 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.s9 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.s9);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.m9 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[i]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.SubS9m = iMEdiumList;
                                }
                                else if (i == 9 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.s10 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    rm.ns10 = rm.s10;
                                    //  rm.m10 =  itemMediumE.Where(s => s.Text == ds.Tables[1].Rows[9]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.s10);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.m10 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[i]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.SubS10m = rm.m10;//iMEdiumList;

                                }
                                else if (i == 10 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.s11 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    //rm.m11 =  itemMediumE.Where(s => s.Text == ds.Tables[1].Rows[10]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                }
                                else if (i == 11)
                                {
                                    rm.s12 = ds.Tables[1].Rows[i]["SUB"].ToString();
                                    // rm.m12 =  itemMediumE.Where(s => s.Text == ds.Tables[1].Rows[11]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                }


                            }
                            else
                            {
                                if (i == 0 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS1 = ds.Tables[1].Rows[0]["SUB"].ToString();
                                    rm.bm1 = ds.Tables[1].Rows[0]["MEDIUM"].ToString();
                                }
                                else if (i == 1 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS2 = ds.Tables[1].Rows[1]["SUB"].ToString();
                                    // rm.bM1 = itemMediumBlindE.Where(s => s.Text == ds.Tables[1].Rows[1]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();

                                    DataSet SelectedMediumList = objDB.SelectS1(rm.subbS2);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.bm2 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[1]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.BM1m = iMEdiumList;
                                }
                                else if (i == 2 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS3 = ds.Tables[1].Rows[2]["SUB"].ToString();
                                    // rm.bm3 = itemMediumBlindE.Where(s => s.Text == ds.Tables[1].Rows[2]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.subbS3);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.bm3 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[2]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.BM3m = iMEdiumList;
                                }
                                else if (i == 3 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS4 = ds.Tables[1].Rows[3]["SUB"].ToString();
                                    // rm.bm4 = itemMediumBlindE.Where(s => s.Text == ds.Tables[1].Rows[3]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.subbS4);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.bm4 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[3]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.BM4m = iMEdiumList;
                                }
                                else if (i == 4 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS5 = ds.Tables[1].Rows[4]["SUB"].ToString();
                                    // rm.bm5 = itemMediumBlindE.Where(s => s.Text == ds.Tables[1].Rows[4]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.subbS5);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.bm5 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[4]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.BM5m = iMEdiumList;
                                }
                                else if (i == 5 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS6 = ds.Tables[1].Rows[5]["SUB"].ToString();
                                    //  rm.bm6 = itemMediumBlindE.Where(s => s.Text == ds.Tables[1].Rows[5]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.subbS6);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.bm6 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[5]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.BM6m = iMEdiumList;
                                }

                                else if (i == 6 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS7 = ds.Tables[1].Rows[6]["SUB"].ToString();
                                    // rm.bm7 = itemMediumBlindE.Where(s => s.Text == ds.Tables[1].Rows[6]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.subbS7);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.bm7 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[6]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.BM7m = iMEdiumList;
                                }
                                else if (i == 7 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS8 = ds.Tables[1].Rows[7]["SUB"].ToString();
                                    // rm.bm8 = itemMediumBlindE.Where(s => s.Text == ds.Tables[1].Rows[7]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.subbS8);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.bm8 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[7]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.BM8m = iMEdiumList;
                                }
                                else if (i == 8 && ds.Tables[1].Rows[i]["SUB"].ToString() != "72" && ds.Tables[1].Rows[i]["SUB"].ToString() != "73")
                                {
                                    rm.subbS9 = ds.Tables[1].Rows[8]["SUB"].ToString();
                                    //rm.bm9 = itemMediumBlindE.Where(s => s.Text == ds.Tables[1].Rows[8]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    DataSet SelectedMediumList = objDB.SelectS1(rm.subbS9);
                                    List<SelectListItem> iMEdiumList = objDB.GetMatricMediumList(SelectedMediumList);
                                    rm.bm9 = iMEdiumList.Where(s => s.Text == ds.Tables[1].Rows[8]["MEDIUM"].ToString()).Select(s => s.Value).FirstOrDefault();
                                    ViewBag.BM9m = iMEdiumList;
                                }
                            }
                        }
                    }

                    //--------------------------End Subject Details--------------

                }

            }
            #endregion M Subjects
            //----------------------Matric Subjects End---------------------------

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            string schlcode = Session["SCHL"].ToString();
            //AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            DataSet result = objDB.schooltypesCorrection(schlcode, "S"); // passing Value to DBClass from model
            if (result.Tables[1].Rows.Count > 0)
            {

                ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
                ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();

                DateTime sDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["sDate"]);
                DateTime eDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["eDate"]);

                DateTime sDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["sDate"]);
                DateTime eDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["eDate"]);

                DateTime sDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["sDate"]);
                DateTime eDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["eDate"]);

                DateTime sDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["sDate"]);
                DateTime eDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["eDate"]);

                DateTime dtTodate = Convert.ToDateTime(DateTime.Today);

                List<SelectListItem> itemsch = new List<SelectListItem>();
                if (ViewBag.Matric == "1" && dtTodate <= eDateM)
                {
                    itemsch.Add(new SelectListItem { Text = "Matriculation Regular", Value = "1" });
                }
               
                if (ViewBag.Matric != "1" && ViewBag.OMatric != "1" && ViewBag.Senior != "1" && ViewBag.OSenior != "1")
                {
                    itemsch.Add(new SelectListItem { Text = "", Value = "" });
                }
                ViewBag.MySch = itemsch.ToList();

            }

            DataSet seleLastCanPen = objDB.PendingCorrectionSubjects(schlcode, "2");
            if (seleLastCanPen.Tables[0].Rows.Count > 0)
            {

                @ViewBag.message = "1";
                rm.StoreAllData = seleLastCanPen;
                ViewBag.TotalViewAllCount = seleLastCanPen.Tables[0].Rows.Count;
                ViewBag.TotalCount = seleLastCanPen.Tables[0].Rows.Count;
            }
            else
            {
                @ViewBag.message = "Record Not Found";
            }

            if (cmd == "View All Correction Pending Record")
            {
                //    var itemsch = new SelectList(new[]{new {ID="1",Name="Matriculation Regular"},new{ID="2",Name="Matriculation Open"},
                //new{ID="3",Name="Sr.Secondary Regular"},new{ID="4",Name="Sr.Secondary Open"},}, "ID", "Name", 1);
                //    ViewBag.MySch = itemsch.ToList();


                DataSet seleLastCan = objDB.PendingCorrectionSubjects(schlcode, "2");
                if (seleLastCan.Tables[0].Rows.Count > 0)
                {

                    @ViewBag.message = "1";
                    rm.StoreAllData = seleLastCan;
                    ViewBag.TotalViewAllCount = seleLastCan.Tables[0].Rows.Count;
                    ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                }
                else
                {
                    @ViewBag.message = "Record Not Found";
                }


                return View(rm);
            }
            if (cmd == "View All Correction Record")
            {
                DataSet seleLastCan = objDB.ViewAllCorrectionSubjects(schlcode);
                if (seleLastCan.Tables[0].Rows.Count > 0)
                {

                    @ViewBag.message = "1";
                    rm.StoreAllData = seleLastCan;
                    ViewBag.TotalViewAllCount = seleLastCan.Tables[0].Rows.Count;
                    ViewBag.TotalCount = seleLastCan.Tables[0].Rows.Count;
                }
                else
                {
                    @ViewBag.message = "Record Not Found";
                }


                return View(rm);
            }
            else
            {
                try
                {
                    DataSet seleLastCan = objDB.SearchCorrectionStudentDetails(formname, schlcode, sid);
                    if (seleLastCan.Tables[0].Rows.Count > 0)
                    {

                        @ViewBag.message = "1";
                        @ViewBag.stdid = seleLastCan.Tables[0].Rows[0]["std_id"].ToString();
                        @ViewBag.Oroll = seleLastCan.Tables[0].Rows[0]["Board_Roll_Num"].ToString();
                        @ViewBag.Regno = seleLastCan.Tables[0].Rows[0]["Registration_num"].ToString();
                        @ViewBag.category = seleLastCan.Tables[0].Rows[0]["category"].ToString();
                        @ViewBag.session = seleLastCan.Tables[0].Rows[0]["Year"].ToString() + "-" + seleLastCan.Tables[0].Rows[0]["Month"].ToString();
                        @ViewBag.canName = seleLastCan.Tables[0].Rows[0]["Candi_Name"].ToString();
                        @ViewBag.FName = seleLastCan.Tables[0].Rows[0]["Father_Name"].ToString();
                        @ViewBag.Mname = seleLastCan.Tables[0].Rows[0]["Mother_Name"].ToString();
                        @ViewBag.lot = seleLastCan.Tables[0].Rows[0]["lot"].ToString();
                        @ViewBag.DOB = seleLastCan.Tables[0].Rows[0]["DOB"].ToString();
                        @ViewBag.Frm = formname;
                        @ViewBag.Subjlist = seleLastCan.Tables[0].Rows[0]["SubjectList"].ToString();
                    }
                    else
                    {
                        @ViewBag.message = "Record Not Found";
                        return View(rm);
                    }

                    if (ModelState.IsValid)
                    {
                        //-------------New SubList---------------//
                        DataSet NewSub = new DataSet();
                        NewSub = objDB.NewCorrectionSubjects(sid, formname, schlcode);  //NewCorrectionSubjects
                        List<SelectListItem> NewSUBList = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in NewSub.Tables[0].Rows)
                        {
                            NewSUBList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });

                        }
                        ViewBag.SubNew = NewSUBList;
                        ViewBag.MediumNew = objCommon.GetMediumAll();
                        ViewBag.SubNewCnt = NewSub.Tables[0].Rows.Count;
                        //-----------End-------------------//                    

                        //----------Old Subject Fill Start----------//
                        DataSet ds = objDB.SearchOldStudent_Subject(sid, formname, schlcode);
                        if (ds == null || ds.Tables[0].Rows.Count == 0)
                        {
                            return RedirectToAction("AdminSubjectCorrectionPerforma", "CorrectionSubjects");
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {

                            List<SelectListItem> OLDSUBList = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                            {
                                OLDSUBList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });

                            }
                            ViewBag.SubOLd = OLDSUBList;
                            ViewBag.SubCnt = ds.Tables[0].Rows.Count;
                        }

                        else
                        {

                            return View(rm);
                        }

                    }
                    else
                    {
                        return View(rm);
                    }
                    return View(rm);

                }
                catch (Exception ex)
                {
                    return View(rm);
                }

            }

        }

        [HttpPost]
        public ActionResult AdminSubjectCorrectionAdd(RegistrationModels rm, FormCollection frm)
        {

            List<SelectListItem> SubWEL = new List<SelectListItem>();
            SubWEL.Add(new SelectListItem { Text = "WELCOME LIFE", Value = "210" });
            ViewBag.SubWEL = new SelectList(SubWEL, "Value", "Text");

            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB(); //calling class DBClass
            DataSet ds2 = objDB.ElectiveSubjects();
            ViewBag.SubS9 = ds2.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.SubS9.Rows)
            {
                items.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }

            ViewBag.SubS10m = ViewBag.SubS9m = ViewBag.SubS9 = new SelectList(items, "Value", "Text");
            ViewBag.S10 = new SelectList(items, "Value", "Text");


            DataSet ds1 = objDB.ElectiveSubjects_Blind();
            ViewBag.SubSb9 = ds1.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> bitems = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.SubSb9.Rows)
            {
                bitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }

            ViewBag.bs2 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs3 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs4 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs5 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs6 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs7 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs8 = new SelectList(bitems, "Value", "Text");
            ViewBag.bs9 = new SelectList(bitems, "Value", "Text");

            string nsqfsub = null;
            DataSet nsresult = objDB.SelectS9(nsqfsub); // passing Value to DBClass from model
            ViewBag.nsfq = nsresult.Tables[0];
            List<SelectListItem> nsfqList = new List<SelectListItem>();
            nsfqList.Add(new SelectListItem { Text = "NSFQ Subjects", Value = "0" });
            foreach (System.Data.DataRow dr in ViewBag.nsfq.Rows)
            {
                nsfqList.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
            }
            ViewBag.nsfqList = new SelectList(nsfqList, "Value", "Text");

            List<SelectListItem> itemsub6 = new List<SelectListItem>();
            itemsub6.Add(new SelectListItem { Text = "SOCIAL STUDIES", Value = "06" });
            ViewBag.SubS6 = new SelectList(itemsub6, "Value", "Text");

            List<SelectListItem> itemsub10 = new List<SelectListItem>();
            itemsub10.Add(new SelectListItem { Text = "HINDI", Value = "03" });
            itemsub10.Add(new SelectListItem { Text = "SOCIAL STUDIES", Value = "06" });
            ViewBag.SubS10 = new SelectList(itemsub10, "Value", "Text");
            //---------------------           
            ViewBag.DAb = objCommon.GetDA();
            ViewBag.DA = objCommon.GetDA();

            //string id = rm.Std_id.ToString();
            if (ModelState.IsValid)
            {
                // AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                string id = frm["Std_id"].ToString();
                string formname = "M";
                DataSet ds = objDB.SearchStudentGetByData_SubjectCORR(id, formname);


                //--------------NSQF---------------------//
                string ses = Session["Session"].ToString();
                string schlcode = Session["SCHL"].ToString();
                DataSet dsnsqf = objDB.CHkNSQF(schlcode, ses);
                if (dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "True" || dsnsqf.Tables[0].Rows[0]["NSQF_flag"].ToString() == "1")
                {
                    ViewData["NSQFSCHOOL"] = "1";
                    ViewBag.NSQFSTUDENT = "1";
                }
                else
                {
                    ViewData["NSQFSCHOOL"] = "0";
                    ViewBag.NSQFSTUDENT = "0";
                }

                //------------------End NSQF--------------------------//

                // Start Subject Master
                DataTable dtMatricSubject = new DataTable();
                dtMatricSubject.Columns.Add(new DataColumn("CLASS", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUB", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUBNM", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUBABBR", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("MEDIUM", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUBCAT", typeof(string)));
                dtMatricSubject.Columns.Add(new DataColumn("SUB_SEQ", typeof(int)));
                DataRow dr = null;
                int j = 0;
                for (int i = 1; i <= 12; i++)
                {
                    dr = dtMatricSubject.NewRow();
                    dr["CLASS"] = 2;
                    DataSet dsSub = new DataSet();
                    dr["SUBNM"] = "";
                    dr["SUBABBR"] = "";

                    if (rm.DA == "N.A.")
                    {
                        if (i == 1)
                        {
                            if (rm.subm1 != null)
                            {
                                dr["MEDIUM"] = rm.subm1;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.subS1; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subS1 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subS1.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();


                            }

                        }
                        else if (i == 2)
                        {
                            dr["SUB"] = rm.subS2; dr["MEDIUM"] = rm.subM2; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subS2 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subS2.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 3)
                        {
                            if (rm.subm3 != null)
                            {
                                dr["MEDIUM"] = rm.subm3;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.subS3; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subS3 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subS3.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 4)
                        {
                            dr["SUB"] = rm.subS4; dr["MEDIUM"] = rm.subM4; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subS4 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subS4.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 5)
                        {
                            dr["SUB"] = rm.subS5; dr["MEDIUM"] = rm.subM5; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subS5 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subS5.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 6)
                        {
                            dr["SUB"] = rm.subS6; dr["MEDIUM"] = rm.subM6; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subS6 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subS6.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 7)
                        {
                            dr["SUB"] = rm.subS7; dr["MEDIUM"] = rm.subM7; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.subS7 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subS7.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 8)
                        {
                            dr["SUB"] = rm.subS8; dr["MEDIUM"] = rm.subM8; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.subS8 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subS8.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 9)
                        {
                            if (rm.m9 != null)
                            {
                                dr["MEDIUM"] = rm.m9;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.s9; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.s9 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.s9.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 10)
                        {
                            if (rm.m10 != null)
                            {
                                dr["MEDIUM"] = rm.m10;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.s10; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "A";
                            if (rm.s10 != null)
                            {
                                if (rm.s10 == "0")
                                {
                                }
                                else
                                {
                                    dsSub = objDB.GetNAmeAndAbbrbySub(rm.s10.ToString());
                                    dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                    dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                                }

                            }
                        }
                        else if (i == 11)
                        {
                            if (rm.m11 != null)
                            {
                                dr["MEDIUM"] = rm.m11;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.s11; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "A";
                            if (rm.s11 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.s11.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 12)
                        {
                            if (rm.m12 != null)
                            {
                                dr["MEDIUM"] = rm.m12;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }
                            dr["SUB"] = rm.s12; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "A";
                            if (rm.s12 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.s12.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        j = i;
                    }
                    else
                    {
                        if (i == 1)
                        {
                            if (rm.bm1 != null)
                            {
                                dr["MEDIUM"] = rm.bm1;
                            }
                            else
                            {
                                dr["MEDIUM"] = "Medium";
                            }

                            dr["SUB"] = rm.subbS1; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subbS1 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS1.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 2)
                        {
                            dr["SUB"] = rm.subbS2; dr["MEDIUM"] = rm.bm2; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subbS2 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS2.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 3)
                        {
                            dr["SUB"] = rm.subbS3; dr["MEDIUM"] = rm.bm3; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subbS3 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS3.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 4)
                        {
                            dr["SUB"] = rm.subbS4; dr["MEDIUM"] = rm.bm4; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subbS4 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS4.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 5)
                        {
                            dr["SUB"] = rm.subbS5; dr["MEDIUM"] = rm.bm5; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subbS5 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS5.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 6)
                        {
                            dr["SUB"] = rm.subbS6; dr["MEDIUM"] = rm.bm6; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "R";
                            if (rm.subbS6 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS6.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 7)
                        {
                            dr["SUB"] = rm.subbS7; dr["MEDIUM"] = rm.bm7; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.subbS7 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS7.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 8)
                        {
                            dr["SUB"] = rm.subbS8; dr["MEDIUM"] = rm.bm8; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.subbS8 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS8.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        else if (i == 9)
                        {
                            dr["SUB"] = rm.subbS9; dr["MEDIUM"] = rm.bm9; dr["SUB_SEQ"] = i; dr["SUBCAT"] = "G";
                            if (rm.subbS9 != null)
                            {
                                dsSub = objDB.GetNAmeAndAbbrbySub(rm.subbS9.ToString());
                                dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                            }
                        }
                        j = i;

                    }
                    dtMatricSubject.Rows.Add(dr);

                    if (rm.DA == "N.A.")
                    {
                        if (i == 1)
                        {
                            dr = dtMatricSubject.NewRow();
                            dr["CLASS"] = 2;
                            //DataSet dsSub = new DataSet();
                            dr["SUBNM"] = "";
                            dr["SUBABBR"] = "";
                            dr["MEDIUM"] = "Medium";
                            if (rm.subS1 == "01")
                            {
                                dr["SUB"] = "72"; dr["SUB_SEQ"] = 11; dr["SUBCAT"] = "R";
                                if (dr["SUB"] != null)
                                {
                                    dsSub = objDB.GetNAmeAndAbbrbySub(dr["SUB"].ToString());
                                    dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                    dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                                }

                            }
                            else
                            {
                                dr["SUB"] = "73"; dr["SUB_SEQ"] = 11; dr["SUBCAT"] = "R";
                                if (dr["SUB"] != null)
                                {
                                    dsSub = objDB.GetNAmeAndAbbrbySub(dr["SUB"].ToString());
                                    dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                    dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                                }

                            }

                            dtMatricSubject.Rows.Add(dr);

                        }
                        if (i == 6)
                        {
                            dr = dtMatricSubject.NewRow();
                            dr["CLASS"] = 2;
                            //DataSet dsSub = new DataSet();
                            dr["SUBNM"] = "";
                            dr["SUBABBR"] = "";
                            dr["MEDIUM"] = "Medium";
                            if (rm.NsqfsubS6 != "NO" && rm.NsqfsubS6 != null && rm.NsqfsubS6 != "")
                            {
                                dr["SUB"] = "85"; dr["SUB_SEQ"] = 12; dr["SUBCAT"] = "R";
                                if (dr["SUB"] != null)
                                {
                                    dsSub = objDB.GetNAmeAndAbbrbySub(dr["SUB"].ToString());
                                    dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                    dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                                }
                            }
                            dtMatricSubject.Rows.Add(dr);
                        }

                    }
                    else
                    {
                        if (i == 1)
                        {
                            dr = dtMatricSubject.NewRow();
                            dr["CLASS"] = 2;
                            //DataSet dsSub = new DataSet();
                            dr["SUBNM"] = "";
                            dr["SUBABBR"] = "";
                            dr["MEDIUM"] = "Medium";
                            if (rm.subbS1 == "01")
                            {
                                dr["SUB"] = "72"; dr["SUB_SEQ"] = 11; dr["SUBCAT"] = "R";
                                if (dr["SUB"] != null)
                                {
                                    dsSub = objDB.GetNAmeAndAbbrbySub(dr["SUB"].ToString());
                                    dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                    dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                                }

                            }
                            else
                            {
                                dr["SUB"] = "73"; dr["SUB_SEQ"] = 11; dr["SUBCAT"] = "R";
                                if (dr["SUB"] != null)
                                {
                                    dsSub = objDB.GetNAmeAndAbbrbySub(dr["SUB"].ToString());
                                    dr["SUBNM"] = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                                    dr["SUBABBR"] = dsSub.Tables[0].Rows[0]["ABBR_ENG"].ToString();
                                }

                            }
                            dtMatricSubject.Rows.Add(dr);
                        }

                    }



                }

                dtMatricSubject.AcceptChanges();
                dtMatricSubject = dtMatricSubject.AsEnumerable().Where(r => r.ItemArray[1].ToString() != "").CopyToDataTable();



                if (Session["SCHOOLDIST"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string result = objDB.Matric_Subject_Correction(rm, frm, id, dtMatricSubject);



                ModelState.Clear();
                //--For Showing Message---------//
                //ViewData["resultUpdate"] = result;
                TempData["resultUpdate"] = result;
                return RedirectToAction("AdminSubjectCorrectionPerforma", "CorrectionSubjects", result);


            }

            return View(rm);
        }
        public ActionResult AdminCorrSubDelete(string id)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB(); //calling class DBClass
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }

            if (id == null)
            {
                return RedirectToAction("AdminSubjectCorrectionPerforma", "CorrectionSubjects");

            }
            else
            {
                string result = objDB.DeleteMatricSubData(id);
                if (result == "Deleted")
                {
                    @ViewBag.result = "1";

                }

            }
            return RedirectToAction("AdminSubjectCorrectionPerforma", "CorrectionSubjects");
        }
        public ActionResult AdminSRCorrSubDelete(string id)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB(); //calling class DBClass
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }

            if (id == null)
            {
                return RedirectToAction("AdminSRSubjectCorrectionPerforma", "CorrectionSubjects");

            }
            else
            {
                string result = objDB.DeleteMatricSubData(id);
                if (result == "Deleted")
                {
                    @ViewBag.result = "1";

                }

            }
            return RedirectToAction("AdminSRSubjectCorrectionPerforma", "CorrectionSubjects");
        }
        public ActionResult AdminopenCorrSubDelete(string id)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB(); //calling class DBClass
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }

            if (id == null)
            {
                return RedirectToAction("AdminopenSubjectCorrectionPerforma", "CorrectionSubjects");

            }
            else
            {
                string result = objDB.DeleteMatricSubData(id);
                if (result == "Deleted")
                {
                    @ViewBag.result = "1";

                }

            }
            return RedirectToAction("AdminopenSubjectCorrectionPerforma", "CorrectionSubjects");
        }

        public ActionResult AdminSubjectCorrectionViewALL(RegistrationModels rm)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB(); //calling class DBClass
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                string schlcode = Session["SCHL"].ToString();
                DataSet seleLastCan = objDB.ViewAllCorrectionSubjects(schlcode);
                if (seleLastCan.Tables[0].Rows.Count > 0)
                {

                    @ViewBag.message = "1";
                    rm.StoreAllData = seleLastCan;
                    ViewBag.TotalViewAllCount = seleLastCan.Tables[0].Rows.Count;
                }
                else
                {
                    @ViewBag.message = "Record Not Found";
                }

            }
            return RedirectToAction("AdminSubjectCorrectionPerforma", "CorrectionSubjects");
        }
        public ActionResult AdminFinalSubmitCorrection(FormCollection frm)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            RegistrationModels rm = new RegistrationModels();
            try
            {
                if (ModelState.IsValid)
                {

                    if (Session["SCHL"] != null)
                    {

                        rm.SCHL = Session["SCHL"].ToString();
                        rm.Correctiontype = "01";
                        string Class = "2";
                        string resultFS = objDB.FinalSubmitSubjectCorrection(rm, Class); // passing Value to DBClass from model
                        if (Convert.ToInt16(resultFS) > 0)
                        {
                            ViewData["resultFS"] = resultFS;
                            return RedirectToAction("AdminSchoolCorrectionFinalPrintLst", "RegistrationPortal");
                        }
                        else
                        {
                            ViewData["resultFS"] = "";
                            return RedirectToAction("AdminSubjectCorrectionPerforma", "CorrectionSubjects");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Logout", "Login");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Login");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login");
            }
        }
        #endregion Admin Matric subject correction

        //-------------------------End Admin Matric Subject Correction---------------------------/


    }
}