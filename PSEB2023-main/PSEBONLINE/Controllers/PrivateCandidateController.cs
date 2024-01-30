using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSEBONLINE.Models;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Globalization;
using PSEBONLINE.Filters;
using PsebPrimaryMiddle.Controllers;
using CCA.Util;
using System.Configuration;
using QRCoder;
using encrypt;
using static System.Net.WebRequestMethods;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon;
using Amazon.S3.IO;

namespace PSEBONLINE.Controllers
{

    public class PrivateCandidateController : Controller
    {
        private const string BUCKET_NAME = "psebdata";        

        public static Byte[] QRCoder(string qr)
        {
            QRCodeGenerator _qrCode = new QRCodeGenerator();
            QRCodeData _qrCodeData = _qrCode.CreateQrCode("https://registration2022.pseb.ac.in/PrivateCandidate/AdmitCardPrivate/Senior?refno=" + QueryStringModule.Encrypt(qr), QRCodeGenerator.ECCLevel.Q);
            //QRCodeData _qrCodeData = _qrCode.CreateQrCode("https://test2022.psebonline.in/AdmitCard/Index/" + QueryStringModule.Encrypt(qr), QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(_qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            return (BitmapToBytesCode(qrCodeImage));

        }
        [NonAction]
        private static Byte[] BitmapToBytesCode(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }




        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.HomeDB objDB = new AbstractLayer.HomeDB();
        private HomeModels Hm = new HomeModels();

        public string stdPic, stdSign;
        // GET: PrivateCandidate



        public ActionResult Index()
        {
            return View();
        }
        #region Private Admit Card Matric
        public ActionResult FinalPrintPrivateMatricAdmitCardSearch()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FinalPrintPrivateMatricAdmitCardSearch(FormCollection frc, string cmd)
        {
            try
            {
                //ViewBag.SelectedItem
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                PrivateCandidateModels rm = new PrivateCandidateModels();
                //if (Session["SCHL"] == null)
                //{
                //    return RedirectToAction("Logout", "Login");
                //}
                //else
                string Search = string.Empty;
                if (cmd == "Search")
                {
                    Search += " a.refno is not null";

                    if (frc["category"] != "" && frc["category"] != "0")
                    {
                        Search += " and a.cat ='" + frc["category"].ToString() + "'";
                    }
                    //if (frc["Exam_Type"] != "")
                    //{
                    //    Search += " and a.rp ='" + frc["Exam_Type"].ToString() + "'";
                    //}
                    if (frc["refNo"] != "")
                    {
                        Search += " and a.refno ='" + frc["refNo"].ToString() + "'";
                    }
                    if (frc["Candi_Name"].Trim() != "")
                    {
                        Search += " and a.name like '" + frc["Candi_Name"].ToString() + "%'";
                    }
                    if (frc["Father_Name"].Trim() != "")
                    {
                        Search += " and a.fname like '" + frc["Father_Name"].ToString() + "%'";
                    }
                    if (frc["OROLL"].Trim() != "")
                    {
                        Search += " and a.roll ='" + frc["OROLL"].ToString() + "'";
                    }
                    rm.category = frc["category"].ToString();
                    //rm.Exam_Type = frc["Exam_Type"].ToString();
                    rm.OROLL = frc["OROLL"].ToString();
                    rm.refNo = frc["refNo"].ToString();
                    rm.Candi_Name = frc["Candi_Name"].ToString();
                    rm.Father_Name = frc["Father_Name"].ToString();
                    rm.QRCode = QRCoder(rm.refNo.ToString());

                    rm.StoreAllData = objDB.GetFinalPrintPrivateMatricAdmitCardSearch(Search);
                    ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                }
                if (cmd == "download")
                {
                    rm.refNo = frc["refno"].ToString();
                    rm.StoreAllData = objDB.GetFinalPrintPrivateMatricAdmitCardSearch(Search);
                    ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                }



                if (ModelState.IsValid)
                { return View(rm); }
                else
                { return View(); }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public ActionResult FinalPrintPrivateMatricAdmitCard(string id)
        {
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            PrivateCandidateModels rm = new PrivateCandidateModels();
            if (id != "" && id != null)
            {
                rm.refNo = id;
                rm.StoreAllData = objDB.GetFinalPrintPrivateMatricAdmitCard(rm);
                ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message2 = "Record Not Found";
                    ViewBag.TotalCount2 = 0;
                }
            }

            return View(rm);
        }
        #endregion Private Admit Card Matric

        #region Private Admit Card
        public ActionResult FinalPrintPrivateAdmitCardSearch()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FinalPrintPrivateAdmitCardSearch(FormCollection frc, string cmd)
        {
            try
            {
                //ViewBag.SelectedItem
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                PrivateCandidateModels rm = new PrivateCandidateModels();
                //if (Session["SCHL"] == null)
                //{
                //    return RedirectToAction("Logout", "Login");
                //}
                //else
                string Search = string.Empty;
                if (cmd == "Search")
                {
                    Search += " a.refno is not null";

                    if (frc["category"] != "" && frc["category"] != "0")
                    {
                        Search += " and a.cat ='" + frc["category"].ToString() + "'";
                    }
                    //if (frc["Exam_Type"] != "")
                    //{
                    //    Search += " and a.rp ='" + frc["Exam_Type"].ToString() + "'";
                    //}
                    if (frc["refNo"] != "")
                    {
                        Search += " and a.refno ='" + frc["refNo"].ToString() + "'";
                    }
                    if (frc["Candi_Name"].Trim() != "")
                    {
                        Search += " and a.name like '" + frc["Candi_Name"].ToString() + "%'";
                    }
                    if (frc["Father_Name"].Trim() != "")
                    {
                        Search += " and a.fname like '" + frc["Father_Name"].ToString() + "%'";
                    }
                    if (frc["OROLL"].Trim() != "")
                    {
                        Search += " and a.roll ='" + frc["OROLL"].ToString() + "'";
                    }
                    rm.category = frc["category"].ToString();
                    //rm.Exam_Type = frc["Exam_Type"].ToString();
                    rm.OROLL = frc["OROLL"].ToString();
                    rm.refNo = frc["refNo"].ToString();
                    rm.Candi_Name = frc["Candi_Name"].ToString();
                    rm.Father_Name = frc["Father_Name"].ToString();

                    rm.StoreAllData = objDB.GetFinalPrintPrivateAdmitCardSearch(Search);
                    ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                }
                if (cmd == "download")
                {
                    rm.refNo = frc["refno"].ToString();
                    rm.StoreAllData = objDB.GetFinalPrintPrivateAdmitCardSearch(Search);
                    ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                }



                if (ModelState.IsValid)
                { return View(rm); }
                else
                { return View(); }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public ActionResult FinalPrintPrivateAdmitCard(string id)
        {
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            PrivateCandidateModels rm = new PrivateCandidateModels();
            if (id != "" && id != null)
            {
                rm.refNo = id;
                rm.StoreAllData = objDB.GetFinalPrintPrivateAdmitCard(rm);
                ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message2 = "Record Not Found";
                    ViewBag.TotalCount2 = 0;
                }
            }

            return View(rm);
        }

        #endregion Private Admit Card 

        public ActionResult PrivateRefrenceUnlockPage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PrivateRefrenceUnlockPage(FormCollection frc)
        {
            PrivateCandidateModels MS = new PrivateCandidateModels();
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            MS.refNo = frc["refNo"];

            if (MS.refNo != null && MS.refNo.Length == 13)
            {
                MS.StoreAllData = objDB.PrivateRefrenceUnlockPage(MS);
            }
            return View();
        }
        public ActionResult Private_Candidate_Examination_Form()
        {
            Session["Oroll"] = null;
            Session["refno"] = null;
            return View();
        }
        [HttpPost]
        public ActionResult Private_Candidate_Examination_Form(FormCollection frc)
        {
            try
            {
                PrivateCandidateModels MS = new PrivateCandidateModels();

                MS.refNo = frc["refNo"].Trim();
                MS.OROLL = frc["OROLL"].Trim();

                if (MS.refNo != null)
                {
                    if (MS.refNo.Substring(5, 2) == "21")
                    {
                        Session["Session"] = @PSEBONLINE.Repository.SessionSettingMastersRepository.GetSessionSettingMasters().SessionShortYear;
                    }
                    //else if (MS.refNo.Substring(5, 2) == "20")
                    //{
                    //    Session["Session"] = "2020-2021";
                    //}

                    AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();

                    MS.StoreAllData = objDB.GetDetailPC(MS);
                    ViewBag.Message = "";
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Given Reference & Roll number not currect.";
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                    else
                    {

                        if (MS.refNo == MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString() && MS.OROLL == MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString())
                        {
                            Session["Oroll"] = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                            ViewData["refno"] = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                            Session["refno"] = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                            ViewData["roll"] = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                            ViewData["Status"] = "1";
                            return RedirectToAction("PrivateCandidateConfirmation", "PrivateCandidate");
                        }
                        return View(MS);
                    }
                }
                return View();
            }
            catch (Exception ex)
            {

                return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
            }

        }
        public ActionResult ForgotPassword()
        {

            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            PrivateCandidateModels MS = new PrivateCandidateModels();

            List<SelectListItem> yearlist = objDB.GetSessionYear1();
            yearlist.Reverse();
            ViewBag.MyYear = yearlist;


            List<SelectListItem> Monthlist = objDB.GetMonth();
            ViewBag.MyMonth = Monthlist;

            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(FormCollection frm, PrivateCandidateModels MS)
        {
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            //PrivateCandidateModels MS = new PrivateCandidateModels();
            try
            {
                DataSet result = objDB.GetSessionYear(); // passing Value to DBClass from model
                ViewBag.MyYear = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyYear.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["sessionYear"].ToString(), Value = @dr["sessionYear"].ToString() });
                }
                ViewBag.MyYear = new SelectList(items, "Value", "Text");

                DataSet result1 = objDB.GetSessionMonth(); // passing Value to DBClass from model
                ViewBag.MyMonth = result1.Tables[0];
                List<SelectListItem> items1 = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyMonth.Rows)
                {
                    items1.Add(new SelectListItem { Text = @dr["sessionMonth"].ToString(), Value = @dr["sessionMonth"].ToString() });
                }
                ViewBag.MyMonth = new SelectList(items1, "Value", "Text");


                MS.Class = frm["Class"];
                MS.category = frm["category"];
                MS.Exam_Type = frm["Exam_Type"];
                MS.SelMonth = frm["SelMonth"];
                MS.SelYear = frm["SelYear"];
                MS.OROLL = frm["OROLL"];
                MS.emailID = frm["emailID"];
                MS.mobileNo = frm["mobileNo"];

                if (frm["OROLL"] != null && frm["OROLL"] != "")
                {
                    MS.StoreAllData = objDB.getForgotPassword(MS);

                    if (MS.StoreAllData.Tables[0].Rows.Count > 0)
                    {

                        ViewData["refno"] = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                        ViewData["roll"] = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                        ViewData["Status"] = "1";

                        try
                        {
                            AbstractLayer.DBClass dbclass = new AbstractLayer.DBClass();
                            if (MS.mobileNo != null || MS.mobileNo != "")
                            {
                                string Sms = "Your refrence no. " + ViewData["refno"] + "  against old roll no. " + ViewData["roll"] + ", Keep this for further use till result declaration.";

                                //string getSms = dbclass.gosms(MS.mobileNo, Sms);
                                // string getSms = objCommon.gosms("9711819184", Sms);
                            }
                            //if (MS.emailID != null || MS.emailID != "")
                            //{
                            //    string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Dear " + ViewData["name"] + "</b>,</td></tr><tr><td height=" + 30 + ">As per your request Dated <b>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</b> Regarding Private Form</td></tr><tr><td><b>You are successfully registred for:-</b><br /><b>Class :</b> " + TempData["Classinfo"] + " March 2017 <br /><b> Reference No. :</b> " + ViewData["refno"] + "<br /><b> Old Roll No. :</b> " + ViewData["roll"] + "<br /><b> Keep this for further use till result declaration.</b> <br /></td></tr><tr><td height=" + 30 + "><b>Click Here To Login</b> <a href=https://www.registration.pseb.ac.in/PrivateCandidate/Private_Candidate_Examination_Form target = _blank>www.registration.pseb.ac.in</a></td></tr><tr><td><b>Note:</b> Please Read Instruction Carefully Before filling the Online Form .</td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /><tr><td><b>Contact Us</b><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 18002700280<br>DISTRICTS:- BARNALA, FATEHGARH SAHIB, GURDASPUR, HOSHIARPUR, JALANDHAR, KAPURTHALA, SHRI MUKTSAR SAHIB, S.B.S. NAGAR, PATHANKOT, PATIALA, SANGRUR, CHANDIGARH &amp; OTHER STATES<br><br><b>Email Id:</b> <a href=mailto:Contact2@psebonline.in target=_blank>contact2@psebonline.in</a><br><b>Toll Free Help Line No. :</b> 18004190690<br>DISTRICTS:- AMRITSAR, BATHINDA, FARIDKOT, FAZILKA, FEROZEPUR, LUDHIANA, MANSA, MOGA, ROOP NAGAR, S.A.S NAGAR,TARN TARAN<br></td></tr>";

                            //    string subject = "PSEB-Private Form Notification";
                            //    bool result = dbclass.mail(subject, body, MS.emailID);
                            //}

                        }
                        catch (Exception) { }
                        return View(MS);
                        //return RedirectToAction("MigrationRec", "MigrateSchool");
                    }

                    else
                    {
                        ViewData["roll"] = MS.OROLL;
                        ViewData["Status"] = "0";
                        return View(MS);
                    }

                }
            }
            catch (Exception ex)
            {

                return View(MS);
            }


            return View(MS);
        }
        public void Capcha()
        {
            Color brushColor = System.Drawing.Color.Red;

            // Creating object for bitmap
            Bitmap objBitmap = new System.Drawing.Bitmap(80, 20);

            // Creating object for Graphics class
            Graphics objGraphics = System.Drawing.Graphics.FromImage(objBitmap);
            objGraphics.Clear(Color.Transparent);

            // Creating object for Font class
            Font objFont = new Font("Times New Roman", 14, FontStyle.Regular);

            string inputNumberString = "";

            Random r = new Random();

            //int a = r.Next(99, 999);
            //int b = r.Next(99, 999);
            int a = r.Next(0, 9);
            int b = r.Next(0, 9);

            int c = a + b;

            inputNumberString = a.ToString() + " + " + b.ToString() + " = ";

            //Storing the captcha value in the session
            Session["CaptchaValue"] = c.ToString();

            SolidBrush myBrush = new SolidBrush(brushColor);

            objGraphics.DrawString(inputNumberString, objFont, myBrush, 3, 3);

            //Adding the content type
            Response.ContentType = "image/png";

            System.IO.MemoryStream mem = new MemoryStream();

            //Saving the bitmap image
            objBitmap.Save(mem, ImageFormat.Png);

            //Writing the image to output screen
            mem.WriteTo(Response.OutputStream);

            // Disposing Font Object
            objFont.Dispose();

            // Disposing Graphics Object
            objGraphics.Dispose();

            // Disposing Bitmap Object
            objBitmap.Dispose();
        }


        public JsonResult BindYearByBatchandCat(string SelBatch, string SelBatchYear, string SelCat, string SelClass, string SelRP) // Calling on http post (on Submit)
        {
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            PrivateCandidateModels MS = new PrivateCandidateModels();
            List<SelectListItem> yearlist1 = new List<SelectListItem>();
            if (SelBatch == "1" && SelBatchYear == "2022")
            {
                //if (SelCat == "SD" && SelRP == "R")
                if (SelCat == "SD")
                {
                    List<SelectListItem> yearlist = objDB.GetSessionYear1().Where(s => Convert.ToInt32(s.Value) >= 1970 && Convert.ToInt32(s.Value) <= 2018).ToList();
                    yearlist.Reverse();
                    ViewBag.MyYear = yearlist;
                    return Json(yearlist);
                }
                // else if (SelCat == "R" && SelClass == "10" && SelRP == "O")
                else if (SelCat == "R" && SelRP == "O")
                {
                    List<SelectListItem> yearlist = objDB.GetSessionYear1().Where(s => Convert.ToInt32(s.Value) >= 2019 && Convert.ToInt32(s.Value) <= 2021).ToList();
                    yearlist.Reverse();
                    ViewBag.MyYear = yearlist;
                    return Json(yearlist);
                }
                else if (SelCat == "R" && SelRP == "R")
                {
                    List<SelectListItem> yearlist = objDB.GetSessionYear1().Where(s => Convert.ToInt32(s.Value) >= 2021 && Convert.ToInt32(s.Value) <= 2021).ToList();
                    yearlist.Reverse();
                    ViewBag.MyYear = yearlist;
                    return Json(yearlist);
                }
            }
            else if (SelBatch == "11" && SelBatchYear == "2021") // OCT
            {
                if (SelCat == "A")
                {
                    List<SelectListItem> yearlist = objDB.GetSessionYear1().Where(s => Convert.ToInt32(s.Value) >= 1950 && Convert.ToInt32(s.Value) <= 2021).ToList();

                    ViewBag.MyYear = yearlist;
                    return Json(yearlist);
                }
            }
            else if (SelBatch == "9" && SelBatchYear == "2023") // OCT
            {
                if (SelCat == "R" && SelRP == "R")
                {
                    List<SelectListItem> yearlist = objDB.GetSessionYear1().Where(s => Convert.ToInt32(s.Value) >= 2023 && Convert.ToInt32(s.Value) <= 2023).ToList();

                    ViewBag.MyYear = yearlist;
                    return Json(yearlist);
                }
                else if (SelCat == "R" && SelRP == "O")
                {
                    List<SelectListItem> yearlist = objDB.GetSessionYear1().Where(s => Convert.ToInt32(s.Value) >= 2021 && Convert.ToInt32(s.Value) <= 2023).ToList();

                    ViewBag.MyYear = yearlist;
                    return Json(yearlist);
                }
                else if (SelCat == "A" && SelRP == "P")
                {
                    List<SelectListItem> yearlist = objDB.GetSessionYear1().Where(s => Convert.ToInt32(s.Value) >= 1901 && Convert.ToInt32(s.Value) <= 2023).ToList();

                    ViewBag.MyYear = yearlist;
                    return Json(yearlist);
                }
            }
            else if (SelBatch == "3" && SelBatchYear == "2023")
            {
                //if (SelCat == "R" && SelClass == "12")
                if (SelCat == "R")
                {
                    if (SelRP == "R")
                    {
                        List<SelectListItem> yearlist = objDB.GetSessionYear1().Where(s => Convert.ToInt32(s.Value) == 2022).ToList();
                        // yearlist.Reverse();
                        ViewBag.MyYear = yearlist;
                        return Json(yearlist);
                    }
                    else if (SelRP == "O")
                    {
                        List<SelectListItem> yearlist = objDB.GetSessionYear1().Where(s => Convert.ToInt32(s.Value) >= 2020 && Convert.ToInt32(s.Value) <= 2022).ToList();

                        ViewBag.MyYear = yearlist;
                        return Json(yearlist);
                    }

                }
                else if (SelCat == "A")
                {
                    List<SelectListItem> yearlist = objDB.GetSessionYear1().Where(s => Convert.ToInt32(s.Value) >= 1950 && Convert.ToInt32(s.Value) <= 2022).ToList();

                    ViewBag.MyYear = yearlist;
                    return Json(yearlist);
                }
                else if (SelCat == "D") //for last 2 year only
                {
                    List<SelectListItem> yearlist = objDB.GetSessionYear1().Where(s => Convert.ToInt32(s.Value) >= 2021 && Convert.ToInt32(s.Value) <= 2022).ToList();
                    ViewBag.MyYear = yearlist;
                    return Json(yearlist);
                }
                //else if (SelCat == "R" && SelClass == "10" && SelRP == "O")
                //{
                //    List<SelectListItem> yearlist = objDB.GetSessionYear1().Where(s => Convert.ToInt32(s.Value) >= 2019 && Convert.ToInt32(s.Value) <= 2022).ToList();
                //    yearlist.Reverse();
                //    ViewBag.MyYear = yearlist;
                //    return Json(yearlist);
                //}
            }
            return Json(yearlist1);
        }


        #region for Admin
        public JsonResult GetPrivateCandidateCategoryListByBatchForAdmin(string SelBatch, string SelBatchYear)
        {
            List<PrivateCandidateCategoryMasters> objList = new List<PrivateCandidateCategoryMasters>();
            if (!string.IsNullOrEmpty(SelBatch))
            {
                objList = AbstractLayer.PrivateCandidateDB.GetPrivateCandidateCategoryMasterListByBatchForAdmin(1, SelBatch, SelBatchYear).ToList();
            }
            return Json(objList);
        }


        public ActionResult Private_Candidate_Introduction_Form_Admin()
        {
            ViewBag.CategoryList = new List<PrivateCandidateCategoryMasters>();
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            PrivateCandidateModels MS = new PrivateCandidateModels();

            List<SelectListItem> yearlist = objDB.GetSessionYear1();
            yearlist.Reverse();
            ViewBag.MyYear = yearlist;
            List<SelectListItem> Monthlist = objDB.GetMonth();
            ViewBag.MyMonth = Monthlist;
            return View();
        }

        [HttpPost]
        public ActionResult Private_Candidate_Introduction_Form_Admin(FormCollection frm, PrivateCandidateModels MS, string batch, string batchYear)
        {

            ViewBag.CategoryList = new List<PrivateCandidateCategoryMasters>();
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();

            DataSet result = objDB.GetSessionYear(); // passing Value to DBClass from model
            ViewBag.MyYear = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.MyYear.Rows)
            {
                items.Add(new SelectListItem { Text = @dr["sessionYear"].ToString(), Value = @dr["sessionYear"].ToString() });
            }
            ViewBag.MyYear = new SelectList(items, "Value", "Text");

            DataSet result1 = objDB.GetSessionMonth(); // passing Value to DBClass from model
            ViewBag.MyMonth = result1.Tables[0];
            List<SelectListItem> items1 = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.MyMonth.Rows)
            {
                items1.Add(new SelectListItem { Text = @dr["sessionMonth"].ToString(), Value = @dr["sessionMonth"].ToString() });
            }
            ViewBag.MyMonth = new SelectList(items1, "Value", "Text");

            try
            {
                MS.Class = frm["Class"];
                MS.category = frm["category"];
                MS.Exam_Type = frm["Exam_Type"];
                MS.SelMonth = frm["SelMonth"];
                MS.SelYear = frm["SelYear"];
                MS.OROLL = frm["OROLL"];
                MS.emailID = frm["emailID"];
                MS.mobileNo = frm["mobileNo"];

                MS.batch = batch;
                MS.batchYear = batchYear;


                MS.Board = frm["Board"];
                if (MS.Board == "OTHER BOARD")
                {
                    MS.Other_Board = frm["Other_Board"];
                }
                else
                {
                    MS.Other_Board = null;
                }

                if (frm["OROLL"] != null && frm["OROLL"] != "" && batchYear != "" && batch != "")
                {
                    DataSet result2 = objDB.InsertPrivateCandidateForm(MS);
                    if (result2.Tables[0].Rows.Count > 0)
                    {
                        if (result2.Tables[0].Rows[0]["result"].ToString() == "1")
                        {

                            ViewData["Status"] = "1";
                            return RedirectToAction("Pre_Private_Candidate_Introduction_Form",
                                new
                                {
                                    id = MS.OROLL,
                                    Clss = MS.Class,
                                    Yar = MS.SelYear,
                                    Mnth = MS.SelMonth,
                                    Cat = MS.category,
                                    btch = MS.batch,
                                    btchYr = MS.batchYear
                                });

                        }
                        else
                        {
                            ViewData["roll"] = MS.OROLL;
                            ViewData["RefNo"] = result2.Tables[0].Rows[0]["refno"].ToString();
                            ViewData["Status"] = result2.Tables[0].Rows[0]["result"].ToString();
                            return View(MS);
                        }
                    }
                    else
                    {
                        ViewData["roll"] = MS.OROLL;
                        ViewData["Status"] = "0";
                        return View(MS);
                    }

                }
            }
            catch (Exception ex)
            {

                return View(MS);
            }


            return View(MS);
        }

        #endregion for Admin


        public JsonResult GetPrivateCandidateCategoryListByBatch(string SelBatch, string SelBatchYear)
        {
            List<PrivateCandidateCategoryMasters> objList = new List<PrivateCandidateCategoryMasters>();
            if (!string.IsNullOrEmpty(SelBatch))
            {
                objList = AbstractLayer.PrivateCandidateDB.GetPrivateCandidateCategoryMasterListByBatch(1, SelBatch, SelBatchYear).ToList();
            }
            return Json(objList);
        }


        public ActionResult Private_Candidate_Introduction_Form()
        {
            ViewBag.CategoryList = new List<PrivateCandidateCategoryMasters>();
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            PrivateCandidateModels MS = new PrivateCandidateModels();

            List<SelectListItem> yearlist = objDB.GetSessionYear1();
            yearlist.Reverse();
            ViewBag.MyYear = yearlist;
            List<SelectListItem> Monthlist = objDB.GetMonth();
            ViewBag.MyMonth = Monthlist;
            return View();
        }

        [HttpPost]
        public ActionResult Private_Candidate_Introduction_Form(FormCollection frm, PrivateCandidateModels MS, string batch, string batchYear)
        {

            ViewBag.CategoryList = new List<PrivateCandidateCategoryMasters>();
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();

            DataSet result = objDB.GetSessionYear(); // passing Value to DBClass from model
            ViewBag.MyYear = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.MyYear.Rows)
            {
                items.Add(new SelectListItem { Text = @dr["sessionYear"].ToString(), Value = @dr["sessionYear"].ToString() });
            }
            ViewBag.MyYear = new SelectList(items, "Value", "Text");

            DataSet result1 = objDB.GetSessionMonth(); // passing Value to DBClass from model
            ViewBag.MyMonth = result1.Tables[0];
            List<SelectListItem> items1 = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.MyMonth.Rows)
            {
                items1.Add(new SelectListItem { Text = @dr["sessionMonth"].ToString(), Value = @dr["sessionMonth"].ToString() });
            }
            ViewBag.MyMonth = new SelectList(items1, "Value", "Text");

            try
            {
                MS.Class = frm["Class"];
                MS.category = frm["category"];
                MS.Exam_Type = frm["Exam_Type"];
                MS.SelMonth = frm["SelMonth"];
                MS.SelYear = frm["SelYear"];
                MS.OROLL = frm["OROLL"];
                MS.emailID = frm["emailID"];
                MS.mobileNo = frm["mobileNo"];

                MS.batch = batch;
                MS.batchYear = batchYear;


                MS.Board = frm["Board"];
                if (MS.Board == "OTHER BOARD")
                {
                    MS.Other_Board = frm["Other_Board"];
                }
                else
                {
                    MS.Other_Board = null;
                }

                if (frm["OROLL"] != null && frm["OROLL"] != "" && batchYear != "" && batch != "")
                {
                    DataSet result2 = objDB.InsertPrivateCandidateForm(MS);
                    if (result2.Tables[0].Rows.Count > 0)
                    {
                        if (result2.Tables[0].Rows[0]["result"].ToString() == "1")
                        {

                            ViewData["Status"] = "1";
                            return RedirectToAction("Pre_Private_Candidate_Introduction_Form",
                                new
                                {
                                    id = MS.OROLL,
                                    Clss = MS.Class,
                                    Yar = MS.SelYear,
                                    Mnth = MS.SelMonth,
                                    Cat = MS.category,
                                    btch = MS.batch,
                                    btchYr = MS.batchYear
                                });

                        }
                        else
                        {
                            ViewData["roll"] = MS.OROLL;
                            ViewData["RefNo"] = result2.Tables[0].Rows[0]["refno"].ToString();
                            ViewData["Status"] = result2.Tables[0].Rows[0]["result"].ToString();
                            return View(MS);
                        }
                    }
                    else
                    {
                        ViewData["roll"] = MS.OROLL;
                        ViewData["Status"] = "0";
                        return View(MS);
                    }

                }
            }
            catch (Exception ex)
            {

                return View(MS);
            }


            return View(MS);
        }


        public ActionResult Pre_Private_Candidate_Introduction_Form(string id, string Clss, string Yar, string Mnth, string Cat, string btch, string btchYr)
        {
            try
            {
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                PrivateCandidateModels MS = new PrivateCandidateModels();
                if (id != null && Clss != null && Yar != null && Mnth != null && Cat != null)
                {
                    MS.StoreAllData = objDB.GetPrivateCandidateDetailsNew(id, Clss, Yar, Mnth, Cat, btch, btchYr);
                    ViewBag.totcount = MS.StoreAllData.Tables[0].Rows.Count;
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        TempData["RStatus"] = "0";
                        return RedirectToAction("Private_Candidate_Introduction_Form", "PrivateCandidate");
                        //return View();
                    }
                    else if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A")
                    {

                        MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["RPNM"].ToString();
                        //if (MS.Exam_Type == "R")
                        //{
                        //    MS.Exam_Type = "Regular";
                        //}
                        //if (MS.Exam_Type == "O")
                        //{
                        //    MS.Exam_Type = "Open";
                        //}
                        //if (MS.Exam_Type == "P")
                        //{
                        //    MS.Exam_Type = "Private";
                        //}

                        MS.category = MS.StoreAllData.Tables[0].Rows[0]["CATNM"].ToString();
                        Session["category"] = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                        //if (MS.category == "R")
                        //{
                        //    MS.category = "Reappear/Compartment";
                        //}
                        //if (MS.category == "D")
                        //{
                        //    MS.category = "Division improvement";
                        //}
                        //if (MS.category == "A")
                        //{
                        //    MS.category = "Additional Subject";
                        //}

                        MS.Class = MS.StoreAllData.Tables[0].Rows[0]["ClassNM"].ToString();


                        MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                        MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
                        MS.Session = MS.SelMonth + ' ' + MS.SelYear;
                        MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                        MS.Result = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();
                        MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();
                        MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();

                        MS.SCHL = "";// MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                        MS.Candi_Name = "";// MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                        MS.Father_Name = "";//MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                        MS.Mother_Name = "";//MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();

                        MS.Pname = "";//MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
                        MS.PFname = "";//MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
                        MS.PMname = "";//MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();
                        MS.EPname = MS.Candi_Name + '/' + MS.Pname;
                        MS.EPFname = MS.Father_Name + '/' + MS.PFname;
                        MS.EPMname = MS.Mother_Name + '/' + MS.PMname;

                        MS.DOB = "";//MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        MS.RegNo = "";//MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();


                        MS.batch = MS.StoreAllData.Tables[0].Rows[0]["batch"].ToString();
                        MS.batchYear = MS.StoreAllData.Tables[0].Rows[0]["batchYear"].ToString();
                        return View(MS);
                    }
                    else
                    {

                        MS.batch = MS.StoreAllData.Tables[0].Rows[0]["batch"].ToString();
                        MS.batchYear = MS.StoreAllData.Tables[0].Rows[0]["batchYear"].ToString();
                        MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["RPNM"].ToString();
                        //if (MS.Exam_Type == "R")
                        //{
                        //    MS.Exam_Type = "Regular";
                        //}
                        //if (MS.Exam_Type == "O")
                        //{
                        //    MS.Exam_Type = "Open";
                        //}
                        //if (MS.Exam_Type == "P")
                        //{
                        //    MS.Exam_Type = "Private";
                        //}

                        MS.category = MS.StoreAllData.Tables[0].Rows[0]["CATNM"].ToString();
                        Session["category"] = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                        //if (MS.category == "R")
                        //{
                        //    MS.category = "Reappear/Compartment";
                        //}
                        //if (MS.category == "D")
                        //{
                        //    MS.category = "Division improvement";
                        //}
                        //if (MS.category == "A")
                        //{
                        //    MS.category = "Additional Subject";
                        //}

                        MS.Class = MS.StoreAllData.Tables[0].Rows[0]["ClassNM"].ToString();
                        //if (MS.Class == "10")
                        //{
                        //    MS.Class = "Matriculation";
                        //}
                        //if (MS.Class == "12")
                        //{
                        //    MS.Class = "Senior Secondary";
                        //}

                        MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                        MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
                        MS.Session = MS.SelMonth + ' ' + MS.SelYear;
                        MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                        MS.Result = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();
                        MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();
                        MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();

                        MS.SCHL = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                        MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                        MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                        MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();

                        MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
                        MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
                        MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();
                        MS.EPname = MS.Candi_Name + '/' + MS.Pname;
                        MS.EPFname = MS.Father_Name + '/' + MS.PFname;
                        MS.EPMname = MS.Mother_Name + '/' + MS.PMname;

                        MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();


                        return View(MS);
                        //return Private_Candidate_Examination_Form();
                        //return Pre_Private_Candidate_Introduction_Form(frm);
                    }
                }
                else
                {
                    //return View(MS);

                    //return Private_Candidate_Examination_Form();
                    return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
                }
            }
            catch (Exception ex)
            {
                //return Private_Candidate_Examination_Form();
                return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
            }
        }

        [HttpPost]
        public ActionResult Pre_Private_Candidate_Introduction_Form(FormCollection frm)
        {
            try
            {
                PrivateCandidateModels MS = new PrivateCandidateModels();
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();


                MS.Class = frm["class"].ToString();
                if (MS.Class.ToLower() == "Matriculation".ToLower())
                {
                    MS.Class = "10";
                    MS.DOB = frm["DOB"].ToString();
                }
                else if (MS.Class.ToLower() == "Senior Secondary".ToLower())
                {
                    MS.Class = "12";
                    MS.DOB = "";
                }
                else if (MS.Class.ToLower() == "Primary".ToLower())
                {
                    MS.Class = "5";
                    MS.DOB = frm["DOB"].ToString();
                }
                else if (MS.Class.ToLower() == "Middle".ToLower())
                {
                    MS.Class = "8";
                    MS.DOB = frm["DOB"].ToString();
                }

                MS.category = frm["category"].ToString();
                if (MS.category == "Reappear/Compartment")
                {
                    MS.category = "R";
                }
                if (MS.category == "Division improvement")
                {
                    MS.category = "D";
                }
                if (MS.category == "Additional Subject")
                {
                    MS.category = "A";
                }
                if (MS.category == "Golden Chance Re-Appear")
                {
                    MS.category = "SR";
                }
                if (MS.category == "Golden Chance Improvement")
                {
                    MS.category = "SD";
                }
                if (MS.category == "Golden Chance Additional")
                {
                    MS.category = "SA";
                }
                MS.Exam_Type = frm["Exam_Type"].ToString();
                if (MS.Exam_Type == "Private")
                {
                    MS.Exam_Type = "P";
                }
                if (MS.Exam_Type == "Regular")
                {
                    MS.Exam_Type = "R";
                }
                if (MS.Exam_Type == "Open")
                {
                    MS.Exam_Type = "O";
                }

                MS.SelMonth = frm["SelMonth"];
                MS.SelYear = frm["SelYear"];

                //MS.Session = frm["Session"].ToString();
                MS.OROLL = frm["OROLL"].ToString();
                Session["Oroll"] = MS.OROLL;
                MS.emailID = frm["emailID"].ToString();
                MS.mobileNo = frm["mobileNo"].ToString();

                MS.Result = frm["Result"].ToString();
                MS.RegNo = frm["RegNo"].ToString();
                MS.Candi_Name = frm["Candi_Name"].ToString();
                MS.Pname = frm["Pname"].ToString();
                MS.Father_Name = frm["Father_Name"].ToString();
                MS.PFname = frm["PFname"].ToString();
                MS.Mother_Name = frm["Mother_Name"].ToString();
                MS.PMname = frm["PMname"].ToString();
                MS.batch = frm["batch"].ToString();
                MS.batchYear = frm["batchYear"].ToString();

                //if (string.IsNullOrEmpty(MS.batch))
                //{
                //    if (MS.batchYear == "2022")
                //    {
                //        MS.batch = "3";
                //        MS.batchYear = frm["batchYear"];
                //    }
                //    else
                //    {
                //        if (MS.category == "SR" || MS.category == "SD" || MS.category == "SA")
                //        {
                //            MS.batch = "11";
                //            MS.batchYear = frm["batchYear"];
                //        }
                //        else
                //        {
                //            MS.batch = "10";
                //            MS.batchYear = frm["batchYear"];
                //        }
                //    }
                //}

                //MS.batch = frm["batch"].ToString();
                //MS.batchYear = frm["batchYear"].ToString();
                string batchMonth = "";
                switch (MS.batch) { case "3": batchMonth = "March"; break; case "6": batchMonth = "Jun"; break; case "7": batchMonth = "Jul"; break; case "8": batchMonth = "Aug"; break; case "9": batchMonth = "Sep"; break; case "10": batchMonth = "Oct"; break; case "11": batchMonth = "Nov"; break; }
                //case 6 : 'June' when @batch = '7' then 'July' when @batch = '8' then 'August' when @batch = '10' then 'October' when @batch = '11' then 'November'
                batchMonth = batchMonth + "-" + MS.batchYear;
                if (MS.OROLL != null)
                {
                    MS.StoreAllData = objDB.GetPrivateCandidateDetailsNew(MS.OROLL, MS.Class, MS.SelYear, MS.SelMonth, MS.category, MS.batch, MS.batchYear);
                    MS.Class = MS.StoreAllData.Tables[0].Rows[0]["class"].ToString();

                    MS.category = MS.StoreAllData.Tables[0].Rows[0]["CATNM"].ToString();
                    Session["category"] = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                }
                else
                {
                    return Private_Candidate_Examination_Form();
                }

                DataSet result2 = objDB.GeneratePrivateCandidateFormRefno(MS);

                if (result2.Tables[0].Rows[0]["RESULT"].ToString() == "1")
                {
                    //string clss=string.Empty;
                    if (MS.Class == "10") { MS.Class = "Matric"; }
                    else if (MS.Class == "12") { MS.Class = "Sr. Sec"; }
                    else if (MS.Class == "5") { MS.Class = "Primary"; }
                    else if (MS.Class == "8") { MS.Class = "Middle"; }

                    ViewData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    Session["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    TempData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    ViewData["roll"] = result2.Tables[0].Rows[0]["roll"].ToString();
                    TempData["roll"] = result2.Tables[0].Rows[0]["roll"].ToString();
                    ViewData["name"] = result2.Tables[0].Rows[0]["name"].ToString();
                    //TempData["ClassDetails"] = result2.Tables[0].Rows[0]["class"].ToString();                    

                    TempData["Classinfo"] = MS.Class + ", " + MS.category + " Exam, ";
                    ViewData["Status"] = "1";
                    //------------------
                    try
                    {
                        AbstractLayer.DBClass dbclass = new AbstractLayer.DBClass();
                        if (MS.mobileNo != null || MS.mobileNo != "")
                        {
                            string Sms = "You are registered for " + TempData["Classinfo"] + batchMonth + " with Ref No. " + ViewData["refno"] + " against Old Roll No. " + ViewData["roll"] + ",save it till result declared.";
                            //string Sms = "You are successfully registred for Class " + TempData["Classinfo"] + "August 2017 and your ref no. " + ViewData["refno"] + " is generated against old roll no. " + ViewData["roll"] + ", Keep this for further use till result declaration.";
                            //string Sms = "You are registred for " + TempData["Classinfo"] + "October 2017 with Ref No. " + ViewData["refno"] + " against Old Roll No. " + ViewData["roll"] + ", save it till result declaration.";
                            //You are registred for Sr.Sec Reap/ Comp Exam Aug 2017 with ref no. 1234567890123 against old roll no 1234567890 save it till result declaration.
                            string getSms = dbclass.gosms(MS.mobileNo, Sms);
                            // string getSms = objCommon.gosms("9711819184", Sms);
                        }
                        if (MS.emailID != null || MS.emailID != "")
                        {
                            string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Dear " + ViewData["name"] + "</b>,</td></tr><tr><td height=" + 30 + ">As per your request Dated <b>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</b> Regarding Private Form</td></tr><tr><td><b>You are successfully registred for:-</b><br /><b>Class :</b> " + TempData["Classinfo"] + " March 2023 <br /><b> Reference No. :</b> " + ViewData["refno"] + "<br /><b> Old Roll No. :</b> " + ViewData["roll"] + "<br /><b> Keep this for further use till result declaration.</b> <br /></td></tr><tr><td height=" + 30 + "><b>Click Here To Login</b> <a href=https://registration2021.pseb.ac.in/PrivateCandidate/Private_Candidate_Examination_Form target = _blank>www.registration.pseb.ac.in</a></td></tr><tr><td><b>Note:</b> Please Read Instruction Carefully Before filling the Online Form .</td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /><tr><td><b>Contact Us</b><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 18002700280<br>DISTRICTS:- BARNALA, FATEHGARH SAHIB, GURDASPUR, HOSHIARPUR, JALANDHAR, KAPURTHALA, SHRI MUKTSAR SAHIB, S.B.S. NAGAR, PATHANKOT, PATIALA, SANGRUR, CHANDIGARH &amp; OTHER STATES<br><br><b>Email Id:</b> <a href=mailto:Contact2@psebonline.in target=_blank>contact2@psebonline.in</a><br><b>Toll Free Help Line No. :</b> 18004190690<br>DISTRICTS:- AMRITSAR, BATHINDA, FARIDKOT, FAZILKA, FEROZEPUR, LUDHIANA, MANSA, MOGA, ROOP NAGAR, S.A.S NAGAR,TARN TARAN<br></td></tr>";

                            string subject = "PSEB-Private Form Notification";
                            bool result = dbclass.mail(subject, body, MS.emailID);
                        }
                        return RedirectToAction("PrivateCandidateConfirmation", "PrivateCandidate");
                    }
                    catch (Exception) { }


                    return View(MS);
                }
                else if (result2.Tables[0].Rows[0]["RESULT"].ToString() == "2")
                {
                    ViewData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    ViewData["roll"] = result2.Tables[0].Rows[0]["roll"].ToString();
                    ViewData["Status"] = "2";
                    // return View(MS);
                }
                //else if (result2.Tables[0].Rows[0]["RESULT"].ToString() == "0")
                else
                {
                    ViewData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    ViewData["roll"] = result2.Tables[0].Rows[0]["roll"].ToString();
                    ViewData["Status"] = "0";
                    // return View(MS);
                }

                return RedirectToAction("Pre_Private_Candidate_Introduction_Form",
                                          new
                                          {
                                              id = MS.OROLL,
                                              Clss = MS.Class,
                                              Yar = MS.SelYear,
                                              Mnth = MS.SelMonth,
                                              Cat = MS.category,
                                              btch = MS.batch,
                                              btchYr = MS.batchYear
                                          });
            }
            catch (Exception ex)
            {
                return Private_Candidate_Examination_Form();
            }


        }
        public ActionResult PrivateCandidateConfirmation(string refno, PrivateCandidateModels MS)
        {
            ViewBag.YesNoList = new AbstractLayer.DBClass().GetYesNoText();
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();

            if (Session["Oroll"] == null || Session["refno"].ToString() == null)
            {
                return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
            }


            string start = Request.QueryString["Oroll"];
            string Oroll = string.Empty;
            try
            {
                Oroll = Session["Oroll"].ToString();
                refno = Session["refno"].ToString();
                if (refno != null && refno != "")
                {
                    if (refno.Substring(5, 2) == "21" || refno.Substring(5, 2) == "22")
                    {
                        Session["Session"] = "2023-2024";
                    }
                    else if (refno.Substring(5, 2) == "20" || refno.Substring(5, 2) == "21")
                    {
                        Session["Session"] = "2020-2021";
                    }



                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(refno);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        return View();
                    }
                    else
                    {

                        if (MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"] == null || MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"].ToString() == "")
                        {
                            MS.DisabilityPercent = 0;
                        }
                        else { MS.DisabilityPercent = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"].ToString()); }



                        MS.IsHardCopyCertificate = MS.StoreAllData.Tables[0].Rows[0]["IsHardCopyCertificate"].ToString();
                        MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["rpNM"].ToString();
                        //if (MS.Exam_Type == "R")
                        //{
                        //    MS.Exam_Type = "Regular";
                        //}
                        //if (MS.Exam_Type == "O")
                        //{
                        //    MS.Exam_Type = "Open";
                        //}
                        //if (MS.Exam_Type == "P")
                        //{
                        //    MS.Exam_Type = "Private";
                        //}
                        //MS.category = MS.StoreAllData.Tables[1].Rows[0]["category"].ToString();
                        MS.category = MS.StoreAllData.Tables[0].Rows[0]["catNM"].ToString();
                        //if (MS.category == "R")
                        //{
                        //    MS.category = "Reappear/Compartment";
                        //}
                        //if (MS.category == "D")
                        //{
                        //    MS.category = "Division improvement";
                        //}
                        //if (MS.category == "A")
                        //{
                        //    MS.category = "Additional Subject";
                        //}
                        //if (MS.category == "SR")
                        //{
                        //    MS.category = "Golden Chance Re-Appear";
                        //}
                        //if (MS.category == "SD")
                        //{
                        //    MS.category = "Golden Chance Improvement";
                        //}
                        //if (MS.category == "SA")
                        //{
                        //    MS.category = "Golden Chance Additional";
                        //}

                        Session["form"] = MS.StoreAllData.Tables[0].Rows[0]["Feecat"].ToString();

                        MS.Class = MS.StoreAllData.Tables[0].Rows[0]["class"].ToString();
                        if (MS.Class == "10")
                        {
                            MS.Class = MS.StoreAllData.Tables[0].Rows[0]["classNM"].ToString();
                            MS.MatricSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            MS.MatricSub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString();
                        }
                        else if (MS.Class == "12")
                        {
                            MS.TwelveSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            MS.Class = MS.StoreAllData.Tables[0].Rows[0]["classNM"].ToString();
                        }
                        else if (MS.Class == "5" || MS.Class == "8")
                        {
                            MS.TwelveSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            MS.Class = MS.StoreAllData.Tables[0].Rows[0]["classNM"].ToString();
                        }
                        MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                        MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
                        MS.Session = MS.SelMonth + ' ' + MS.SelYear;

                        MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                        Session["OROLL"] = MS.OROLL;
                        Session["category"] = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();

                        MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();
                        MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();

                        MS.refNo = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                        MS.SCHL = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();

                        MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                        MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                        MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();

                        MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();
                        MS.Result = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();

                        MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
                        MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
                        MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();

                        MS.EPname = MS.Candi_Name + '/' + MS.Pname;
                        MS.EPFname = MS.Father_Name + '/' + MS.PFname;
                        MS.EPMname = MS.Mother_Name + '/' + MS.PMname;

                        MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["sex"].ToString();
                        MS.CastList = MS.StoreAllData.Tables[0].Rows[0]["caste"].ToString();
                        MS.Area = MS.StoreAllData.Tables[0].Rows[0]["area"].ToString();
                        if (MS.Area == "U")
                        {
                            MS.Area = "Urban";
                        }
                        if (MS.Area == "R")
                        {
                            MS.Area = "Rural";
                        }
                        MS.Relist = MS.StoreAllData.Tables[0].Rows[0]["religion"].ToString();
                        if (MS.Relist == "H")
                        {
                            MS.Relist = "Hindu";
                        }
                        if (MS.Relist == "M")
                        {
                            MS.Relist = "Muslim";
                        }
                        if (MS.Relist == "S")
                        {
                            MS.Relist = "Sikh";
                        }
                        if (MS.Relist == "C")
                        {
                            MS.Relist = "Christian";
                        }
                        if (MS.Relist == "O")
                        {
                            MS.Relist = "Others";
                        }


                        MS.phyChal = MS.StoreAllData.Tables[0].Rows[0]["phy_chal"].ToString();

                        MS.rdoWantWriter = MS.StoreAllData.Tables[0].Rows[0]["writer"].ToString();
                        if (MS.rdoWantWriter == "True")
                        {
                            MS.rdoWantWriter = "1";
                        }
                        if (MS.rdoWantWriter == "False")
                        {
                            MS.rdoWantWriter = "0";
                        }

                        MS.IsPracExam = MS.StoreAllData.Tables[0].Rows[0]["prac"].ToString();
                        if (MS.IsPracExam == "True")
                        {
                            MS.IsPracExam = "1";
                        }
                        if (MS.IsPracExam == "False")
                        {
                            MS.IsPracExam = "0";
                        }


                        MS.address = MS.StoreAllData.Tables[0].Rows[0]["homeadd"].ToString();
                        MS.landmark = MS.StoreAllData.Tables[0].Rows[0]["LandMark"].ToString();
                        MS.block = MS.StoreAllData.Tables[0].Rows[0]["block"].ToString();

                        MS.SelExamDist = MS.StoreAllData.Tables[0].Rows[0]["dist"].ToString();

                        int exmdist = Convert.ToInt32(MS.SelExamDist);
                        DataSet resultEC = objDB.SelectAllTehsilEC(exmdist);
                        ViewBag.MyTehsilEC = resultEC.Tables[0];
                        ViewBag.MyTehsilEC2 = resultEC.Tables[0];
                        List<SelectListItem> TehListEC = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyTehsilEC.Rows)
                        {

                            TehListEC.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                        }

                        ViewBag.MyTehsilEC = TehListEC;
                        ViewBag.MyTehsilEC2 = TehListEC;

                        MS.tehsilEC = MS.StoreAllData.Tables[0].Rows[0]["Cent_1"].ToString();
                        MS.tehsilEC2 = MS.StoreAllData.Tables[0].Rows[0]["Cent_2"].ToString();

                        MS.SelDist = MS.StoreAllData.Tables[0].Rows[0]["homedistco"].ToString();
                        MS.tehsil = MS.StoreAllData.Tables[0].Rows[0]["tehsil"].ToString();
                        MS.pinCode = MS.StoreAllData.Tables[0].Rows[0]["pincode"].ToString();
                        Session["ChallanID"] = MS.StoreAllData.Tables[0].Rows[0]["challanid"].ToString();
                        DataSet result = objDB.SelectDist(); // passing Value to DBClass from model
                        ViewBag.MyDist = result.Tables[0];// TempData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> items = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                        {
                            items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                        }
                        // ViewBag.MyDist = items;
                        ViewBag.MyDist = new SelectList(items, "Value", "Text");

                        if (MS.SelDist == "")
                        {
                            MS.SelDist = "0";
                        }

                        ViewBag.MyExamDist = result.Tables[0];// TempData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> items1 = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyExamDist.Rows)
                        {
                            items1.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                        }
                        // ViewBag.MyDist = items;
                        ViewBag.MyExamDist = new SelectList(items1, "Value", "Text");

                        if (MS.SelExamDist == "")
                        {
                            MS.SelExamDist = "0";
                        }

                        int dist = Convert.ToInt32(MS.SelDist);
                        DataSet result1 = objDB.SelectAllTehsil(dist);
                        ViewBag.MyTehsil = result1.Tables[0];
                        List<SelectListItem> TehList = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
                        {

                            TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                        }
                        ViewBag.MyTehsil = TehList;


                        if (Session["category"].ToString() == "R" || Session["category"].ToString() == "SR" || Session["category"].ToString() == "SD" || Session["category"].ToString() == "SA")
                        {
                            MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                            MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                            MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString();
                            MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString();
                            MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString();
                            MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString();
                            MS.sub7 = MS.StoreAllData.Tables[0].Rows[0]["rsub7"].ToString();
                            MS.sub8 = MS.StoreAllData.Tables[0].Rows[0]["rsub8"].ToString();
                        }
                        if ((Session["category"].ToString() == "SR" || Session["category"].ToString() == "SD" || Session["category"].ToString() == "SA") && MS.StoreAllData.Tables[0].Rows[0]["formstatus"].ToString() == "0")
                        {
                            MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString();
                            MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString();
                            MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString();
                            MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString();
                            MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString();
                            MS.sub7 = MS.StoreAllData.Tables[0].Rows[0]["rsub7code"].ToString();
                            MS.sub8 = MS.StoreAllData.Tables[0].Rows[0]["rsub8code"].ToString();
                        }
                        if (Session["category"].ToString() == "D")
                        {
                            MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            List<SelectListItem> DMitems1 = new List<SelectListItem>();
                            if (MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub1"].ToString())
                            {
                                DMitems1.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString() });
                            }
                            else
                            {
                                DMitems1.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub1"].ToString() });
                            }
                            ViewBag.sub1Matric = new SelectList(DMitems1, "Value", "Text");

                            List<SelectListItem> DMitems2 = new List<SelectListItem>();
                            MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString();
                            if (MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub2"].ToString())
                            {
                                DMitems2.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString() });
                            }
                            else
                            {
                                DMitems2.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub2"].ToString() });
                            }
                            ViewBag.sub2Matric = new SelectList(DMitems2, "Value", "Text");

                            List<SelectListItem> DMitems3 = new List<SelectListItem>();
                            MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString();
                            if (MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub3"].ToString())
                            {
                                DMitems3.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString() });
                            }
                            else
                            {
                                DMitems3.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub3"].ToString() });
                            }
                            ViewBag.sub3Matric = new SelectList(DMitems3, "Value", "Text");

                            List<SelectListItem> DMitems4 = new List<SelectListItem>();
                            MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString();
                            if (MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub4"].ToString())
                            {
                                DMitems4.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString() });
                            }
                            else
                            {
                                DMitems4.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub4"].ToString() });
                            }
                            ViewBag.sub4Matric = new SelectList(DMitems4, "Value", "Text");

                            List<SelectListItem> DMitems5 = new List<SelectListItem>();
                            MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString();
                            if (MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub5"].ToString())
                            {
                                DMitems5.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString() });
                            }
                            else
                            {
                                DMitems5.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub5"].ToString() });
                            }
                            ViewBag.sub5Matric = new SelectList(DMitems5, "Value", "Text");

                            List<SelectListItem> DMitems6 = new List<SelectListItem>();
                            MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString();
                            if (MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub6"].ToString())
                            {
                                DMitems6.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString() });
                            }
                            else
                            {
                                DMitems6.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub6"].ToString() });
                            }
                            ViewBag.sub6Matric = new SelectList(DMitems6, "Value", "Text");

                            List<SelectListItem> DMitems7 = new List<SelectListItem>();
                            MS.sub7 = MS.StoreAllData.Tables[0].Rows[0]["rsub7code"].ToString();
                            if (MS.StoreAllData.Tables[0].Rows[0]["rsub7code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub7"].ToString())
                            {
                                DMitems7.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub7"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub7code"].ToString() });
                            }
                            else
                            {
                                DMitems7.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub7"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub7"].ToString() });
                            }
                            ViewBag.sub7Matric = new SelectList(DMitems7, "Value", "Text");

                            List<SelectListItem> DMitems8 = new List<SelectListItem>();
                            MS.sub8 = MS.StoreAllData.Tables[0].Rows[0]["rsub8code"].ToString();
                            if (MS.StoreAllData.Tables[0].Rows[0]["rsub8code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub8"].ToString())
                            {
                                DMitems8.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub8"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub8code"].ToString() });
                            }
                            else
                            {
                                DMitems8.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub8"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub8"].ToString() });
                            }
                            ViewBag.sub8Matric = new SelectList(DMitems8, "Value", "Text");
                        }
                        if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                        {
                            List<SelectListItem> Mitems = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[1].Rows)
                            {
                                Mitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.SubMatric = new SelectList(Mitems, "Value", "Text");
                        }
                        if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "12")
                        {
                            List<SelectListItem> sitems = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[2].Rows)
                            {
                                sitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.SubTwelve = new SelectList(sitems, "Value", "Text");
                        }

                        if ((Session["category"].ToString() == "SR" || Session["category"].ToString() == "SD" || Session["category"].ToString() == "SA") && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                        {
                            List<SelectListItem> DMitems1 = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[3].Rows)
                            {
                                DMitems1.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.sub1Matric = new SelectList(DMitems1, "Value", "Text");
                        }
                        if ((Session["category"].ToString() == "SR" || Session["category"].ToString() == "SD" || Session["category"].ToString() == "SA") && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "12")
                        {
                            List<SelectListItem> DMitems1 = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[4].Rows)
                            {
                                DMitems1.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.Sub1Twelve = new SelectList(DMitems1, "Value", "Text");
                        }
                        MS.imgPhoto = MS.StoreAllData.Tables[0].Rows[0]["Photo_url"].ToString() != "" ? MS.StoreAllData.Tables[0].Rows[0]["Photo_url"].ToString().Replace("UPLOAD2021", "").Replace("//", "/").Replace("UPLOAD2023/", "").Replace("Upload2021/", "").Replace("UPLOAD2022", "Upload2022").Replace("Upload2021", "").Replace("UPLOAD2022/ ", "").Replace("UPLOAD2023/ ", "").Replace("OPEN2021", "Open2021").Replace("OPEN2022", "Open2022").Replace("PHOTO", "Photo").Replace("JPG", "jpg").Replace("upload//", "") : "";
                        MS.imgSign = MS.StoreAllData.Tables[0].Rows[0]["Sign_url"].ToString() != "" ? MS.StoreAllData.Tables[0].Rows[0]["Sign_url"].ToString().Replace("UPLOAD2021", "").Replace("//", "/").Replace("UPLOAD2023/", "").Replace("Upload2021/", "").Replace("UPLOAD2022", "Upload2022").Replace("Upload2021", "").Replace("Upload2021", "").Replace("UPLOAD2021/", "").Replace("UPLOAD2021/", "").Replace("UPLOAD2022/", "").Replace("UPLOAD2023/", "").Replace("OPEN2021", "Open2021").Replace("OPEN2022", "Open2022").Replace("SIGN", "Sign").Replace("JPG", "jpg").Replace("upload//", "") : "";
                        ViewBag.PhotoExist = "0";
                        ViewBag.signExist = "0";
                        if (MS.imgPhoto != "")
                        {
                            ViewBag.PhotoExist = "1";
                        }
                        if (MS.imgSign != "")
                        {
                            ViewBag.signExist = "1";
                        }
                        //var filePath1 = "";                        

                        // if (MS.imgPhoto == null || MS.imgSign == null)
                        if (MS.StoreAllData.Tables[0].Rows[0]["rp"].ToString().ToUpper() == "R" && MS.imgPhoto.Contains("PvtPhoto") == false)
                        {
                            //@ViewBag.Photo = "https://registration2022.pseb.ac.in/Upload/Upload2021/" + MS.imgPhoto;
                            @ViewBag.Photo = MS.imgPhoto;
                            @ViewBag.sign = MS.imgSign;
                            //@ViewBag.sign = "https://registration2022.pseb.ac.in/Upload/Upload2021/" + MS.imgSign;
                            MS.PathPhoto = ViewBag.Photo;
                            MS.PathSign = ViewBag.sign;

                            //filePath1 = MS.imgPhoto = @"upload2017/" + MS.StoreAllData.Tables[0].Rows[0]["Photo_url"].ToString();
                            //string PhotoExist = Path.Combine(Server.MapPath("~/Upload2021/" + MS.imgPhoto));
                            //string SignExist = Path.Combine(Server.MapPath("~/Upload2021/" + MS.imgSign));
                            //ViewBag.PhotoExist = System.IO.File.Exists(PhotoExist) ? "1" : "0";
                            //ViewBag.signExist = System.IO.File.Exists(SignExist) ? "1" : "0";

                        }
                        else if (MS.StoreAllData.Tables[0].Rows[0]["rp"].ToString().ToUpper() == "O" && MS.imgPhoto.Contains("PvtPhoto") == false)
                        {
                            //@ViewBag.Photo = "https://registration2022.pseb.ac.in/Upload/" + MS.imgPhoto;
                            //@ViewBag.sign = "https://registration2022.pseb.ac.in/Upload/" + MS.imgSign;
                            @ViewBag.Photo = MS.imgPhoto;
                            @ViewBag.sign = MS.imgSign;
                            MS.PathPhoto = ViewBag.Photo;
                            MS.PathSign = ViewBag.sign;

                            //filePath1 = MS.imgPhoto = @"" + MS.StoreAllData.Tables[0].Rows[0]["Photo_url"].ToString();
                            //string PhotoExist = Path.Combine(Server.MapPath("~/" + MS.imgPhoto.Replace("https://registration2022.pseb.ac.in/", "")));
                            //string SignExist = Path.Combine(Server.MapPath("~/" + MS.imgSign.Replace("https://registration2022.pseb.ac.in/", "")));
                            //ViewBag.PhotoExist = System.IO.File.Exists(PhotoExist) ? "1" : "0";
                            //ViewBag.signExist = System.IO.File.Exists(SignExist) ? "1" : "0";
                        }
                        else
                        {
                            @ViewBag.Photo = MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
                            @ViewBag.sign = MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();
                            MS.PathPhoto = ViewBag.Photo;
                            MS.PathSign = ViewBag.sign;

                            //string PhotoExist = Path.Combine(Server.MapPath("~/Upload2023/" + MS.imgPhoto));
                            //string SignExist = Path.Combine(Server.MapPath("~/Upload2023/" + MS.imgSign));
                            //ViewBag.PhotoExist = System.IO.File.Exists(PhotoExist) ? "1" : "0";
                            //ViewBag.signExist = System.IO.File.Exists(SignExist) ? "1" : "0";
                        }

                        #region update photo and sign  if mis match
                        int k = 0;
                        for (int i = 0; i < MS.StoreAllData.Tables[0].Rows.Count; i++)
                        {

                            string stdPic = "", stdSign = "";
                            string filepathtosave = "", filepathtosaveSign = "";
                            var filePath1 = "";
                            var filePath1Sign = "";
                            MS.refNo = MS.StoreAllData.Tables[0].Rows[i]["refno"].ToString().ToUpper();
                            var rp = MS.StoreAllData.Tables[0].Rows[i]["rp"].ToString().ToUpper();
                            var phtURL = MS.StoreAllData.Tables[0].Rows[i]["Photo_url"].ToString();
                            var signURL = MS.StoreAllData.Tables[0].Rows[i]["Sign_url"].ToString();
                            var cat = MS.StoreAllData.Tables[0].Rows[i]["cat"].ToString().ToUpper();
                            string Oldpath = "", OldpathSign = "";

                            string imgBatchPath = "Batch" + MS.refNo.Substring(3, 4);


                            if (phtURL.Contains(imgBatchPath) == false)
                            {
                                if (rp == "R" && (phtURL.Contains("PvtPhoto/Photo") == true || phtURL.Contains("PvtPhoto/Batch0321/Photo") == true))
                                {
                                    filePath1 = MS.imgPhoto = @"Upload2023/" + phtURL;
                                    //Oldpath = Path.Combine(Server.MapPath("~/Upload2021/" + phtURL));
                                }
                                else if (rp == "R" && phtURL.Contains("PvtPhoto") == false)
                                {
                                    filePath1 = MS.imgPhoto = @"Upload2021/" + phtURL;
                                    //Oldpath = Path.Combine(Server.MapPath("~/Upload2021/" + phtURL));
                                }
                                else if (phtURL.Contains("PvtPhoto") == true && (phtURL.Contains("21/Photo") == true || phtURL.Contains("21/Photo") == true))
                                {
                                    filePath1 = MS.imgPhoto = @"Upload2023/" + phtURL;
                                    //Oldpath = Path.Combine(Server.MapPath("~/Upload2023/" + phtURL));
                                }
                                else if (phtURL.Contains("PvtPhoto") == true && phtURL.Contains("20/Photo") == true)
                                {
                                    filePath1 = MS.imgPhoto = @"Upload2021/" + phtURL;
                                    //Oldpath = Path.Combine(Server.MapPath("~/Upload2021/" + phtURL));
                                }
                                else if (rp == "O" && phtURL.Contains("PvtPhoto") == false)
                                {
                                    filePath1 = MS.imgPhoto = @"" + phtURL;
                                    //Oldpath = Path.Combine(Server.MapPath("~/" + phtURL));
                                }
                                else if (cat == "SR" || cat.ToUpper() == "SD")
                                {
                                    filePath1 = MS.imgPhoto = "~/Upload2023/" + phtURL;
                                    //Oldpath = Path.Combine(Server.MapPath("~/Upload2023/" + phtURL));
                                }

                                if (filePath1 != null && filePath1 != "" && filePath1.Contains("Upload") && phtURL.Contains(imgBatchPath) == false)
                                {
                                    var path = Path.Combine(Server.MapPath("~/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo"), MS.refNo + "P" + ".jpg");
                                    string FilepathExist = Path.Combine(Server.MapPath("~/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo"));

                                    if (MS.Class == "Matriculation" || MS.Class == "Senior Secondary" || MS.Class == "Primary" || MS.Class == "Middle")
                                    {
                                        string type = "P";
                                        string UpdatePic = new AbstractLayer.PrivateCandidateDB().Updated_PrivateCandidate_PhotoSign_ByRefNo(MS.refNo, MS.PathPhoto, type);
                                    }
                                    //else
                                    //{

                                    //    if (!Directory.Exists(FilepathExist))
                                    //    {
                                    //        Directory.CreateDirectory(FilepathExist);
                                    //    }
                                    //    if (System.IO.File.Exists(path))
                                    //    {
                                    //        System.IO.File.Delete(path);
                                    //    }

                                    //    if (System.IO.File.Exists(Oldpath))
                                    //    {
                                    //        System.IO.File.Copy(Oldpath, path);
                                    //        filepathtosave = "PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo/" + MS.refNo + "P" + ".jpg";
                                    //        MS.PathPhoto = filepathtosave;
                                    //        MS.imgPhoto = filepathtosave;
                                    //        string PhotoName = MS.refNo + "P" + ".jpg";
                                    //        string type = "P";
                                    //        string UpdatePic = new AbstractLayer.PrivateCandidateDB().Updated_PrivateCandidate_PhotoSign_ByRefNo(MS.refNo, MS.PathPhoto, type);
                                    //        k++;
                                    //    }
                                    //}

                                }

                            }

                            if (signURL.Contains(imgBatchPath) == false)
                            {
                                if (rp == "R" && (signURL.Contains("PvtPhoto/Sign") == true || signURL.Contains("PvtPhoto/Batch0321/Sign") == true))
                                {
                                    filePath1Sign = MS.imgSign = @"Upload2021/" + signURL;
                                    //OldpathSign = Path.Combine(Server.MapPath("~/Upload2021/" + signURL));
                                }
                                else if (rp == "R" && signURL.Contains("PvtPhoto") == false)
                                {
                                    filePath1Sign = MS.imgSign = @"Upload2021/" + signURL;
                                    //OldpathSign = Path.Combine(Server.MapPath("~/Upload2021/" + signURL));
                                }

                                else if (signURL.Contains("PvtPhoto") == true && (signURL.Contains("20/Sign") == true || signURL.Contains("21/Sign") == true))
                                {
                                    filePath1Sign = MS.imgSign = @"Upload2023/" + signURL;
                                    //OldpathSign = Path.Combine(Server.MapPath("~/Upload2023/" + signURL));
                                }
                                else if (signURL.Contains("PvtPhoto") == true && signURL.Contains("19/Sign") == true)
                                {
                                    filePath1Sign = MS.imgSign = @"Upload2021/" + signURL;
                                    //OldpathSign = Path.Combine(Server.MapPath("~/Upload2021/" + signURL));
                                }
                                else if (rp == "O" && signURL.Contains("PvtPhoto") == false)
                                {
                                    filePath1Sign = MS.imgSign = @"" + signURL;
                                    //OldpathSign = Path.Combine(Server.MapPath("~/" + signURL));
                                }
                                if (cat == "SR" || cat == "SD")
                                {
                                    filePath1Sign = MS.imgPhoto = "~/Upload2023/" + signURL;
                                    //OldpathSign = Path.Combine(Server.MapPath("~/Upload2023/" + signURL));
                                }
                                if (signURL.Contains("Correction1819") == true)
                                {
                                    filePath1Sign = MS.imgSign = @"Upload2021/" + signURL;
                                    //OldpathSign = Path.Combine(Server.MapPath("~/Upload2021/" + signURL));
                                }

                                if (filePath1Sign != null && filePath1Sign != "" && filePath1Sign.Contains("Upload") && signURL.Contains(imgBatchPath) == false)
                                {
                                    var path = Path.Combine(Server.MapPath("~/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign"), MS.refNo + "S" + ".jpg");
                                    string FilepathExist = Path.Combine(Server.MapPath("~/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign"));

                                    if (MS.Class == "Matriculation" || MS.Class == "Senior Secondary" || MS.Class == "Primary" || MS.Class == "Middle")
                                    {
                                        string type = "S";
                                        string UpdatePic = new AbstractLayer.PrivateCandidateDB().Updated_PrivateCandidate_PhotoSign_ByRefNo(MS.refNo, MS.PathSign, type);
                                    }

                                    //if (!Directory.Exists(FilepathExist))
                                    //{
                                    //    Directory.CreateDirectory(FilepathExist);
                                    //}
                                    //if (System.IO.File.Exists(path))
                                    //{
                                    //    System.IO.File.Delete(path);
                                    //}



                                    //if (System.IO.File.Exists(OldpathSign))
                                    //{
                                    //    System.IO.File.Copy(OldpathSign, path);

                                    //    //System.IO.File.Copy(filePath1, path);
                                    //    filepathtosaveSign = "PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign/" + MS.refNo + "S" + ".jpg";
                                    //    MS.PathSign = filepathtosaveSign;
                                    //    MS.imgSign = filepathtosaveSign;
                                    //    string PhotoName = MS.refNo + "S" + ".jpg";
                                    //    string type = "S";
                                    //    string UpdatePic = new AbstractLayer.PrivateCandidateDB().Updated_PrivateCandidate_PhotoSign_ByRefNo(MS.refNo, MS.PathSign, type);
                                    //}
                                }
                            }
                        }
                        #endregion




                        Session["imgPhoto"] = MS.imgPhoto;
                        Session["imgSign"] = MS.imgSign;


                        MS.FormStatus = MS.StoreAllData.Tables[0].Rows[0]["FormStatus"].ToString();

                        return View(MS);
                        //return Private_Candidate_Examination_Form();
                        //return Pre_Private_Candidate_Introduction_Form(frm);
                    }
                }
                else
                {
                    //return View(MS);

                    //return Private_Candidate_Examination_Form();
                    return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
                }
            }
            catch (Exception ex)
            {
                //return Private_Candidate_Examination_Form();
                return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
            }
        }

        [HttpPost]
        public ActionResult PrivateCandidateConfirmation(PrivateCandidateModels MS, FormCollection frm)
        {
            if (Session["Oroll"] == null || Session["refno"].ToString() == null)
            {
                return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
            }

            ViewBag.YesNoList = new AbstractLayer.DBClass().GetYesNoText();
            try
            {

                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();

                DataSet result = objDB.SelectDist(); // passing Value to DBClass from model
                ViewBag.MyDist = result.Tables[0];// TempData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }
                // ViewBag.MyDist = items;
                ViewBag.MyDist = new SelectList(items, "Value", "Text");

                ViewBag.MyExamDist = result.Tables[0];// TempData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items1 = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyExamDist.Rows)
                {
                    items1.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }
                // ViewBag.MyDist = items;
                ViewBag.MyExamDist = new SelectList(items1, "Value", "Text");

                if (MS.SelExamDist == "")
                {
                    MS.SelExamDist = "0";
                }


                if (frm["SelExamDist"] != null)
                {
                    MS.SelExamDist = frm["SelExamDist"].ToString();
                }
                else
                {
                    MS.SelExamDist = "0";
                }

                MS.Exam_Type = frm["Exam_Type"].ToString();
                MS.refNo = frm["refNo"].ToString();
                MS.Session = frm["Session"].ToString();
                MS.Result = frm["result"].ToString();
                MS.Class = frm["Class"].ToString();

                if (MS.Class == "Matriculation")
                {
                    MS.Class = "10";
                    MS.DOB = frm["DOB"].ToString();
                }
                else if (MS.Class == "Senior Secondary")
                {
                    MS.Class = "12";
                    MS.DOB = "";
                }
                else if (MS.Class.ToLower() == "Primary".ToLower() || MS.Class.ToLower() == "Middle".ToLower())
                {
                    MS.Class = MS.Class.ToLower() == "Primary".ToLower() ? "5" : MS.Class.ToLower() == "Middle".ToLower() ? "8" : "";
                    MS.DOB = "";
                }




                MS.Candi_Name = frm["Candi_Name"].ToString();
                MS.Father_Name = frm["Father_Name"].ToString();
                MS.Mother_Name = frm["Mother_Name"].ToString();

                MS.Pname = frm["PNAME"].ToString();
                MS.PFname = frm["PFNAME"].ToString();
                MS.PMname = frm["PMNAME"].ToString();



                MS.RegNo = frm["RegNo"].ToString();
                MS.OROLL = frm["OROLL"].ToString();
                Session["OROLL"] = MS.OROLL;
                Session["refno"] = MS.refNo;
                MS.mobileNo = frm["mobileNo"].ToString();
                MS.emailID = frm["emailID"].ToString();

                MS.Gender = frm["Gender"].ToString();

                MS.CastList = frm["CastList"].ToString();
                MS.Area = frm["Area"].ToString();
                if (MS.Area == "Urban")
                {
                    MS.Area = "U";
                }
                if (MS.Area == "Rural")
                {
                    MS.Area = "R";
                }
                MS.Relist = frm["Relist"].ToString();
                if (MS.Relist == "Hindu")
                {
                    MS.Relist = "H";
                }
                if (MS.Relist == "Muslim")
                {
                    MS.Relist = "M";
                }
                if (MS.Relist == "Sikh")
                {
                    MS.Relist = "S";
                }
                if (MS.Relist == "Christian")
                {
                    MS.Relist = "C";
                }
                if (MS.Relist == "Others")
                {
                    MS.Relist = "O";
                }

                MS.IsphysicalChall = frm["phyChal"].ToString();
                MS.DisabilityPercent = Convert.ToInt32(frm["DisabilityPercent"].ToString());
                if (MS.IsphysicalChall != "N.A." && frm["rdoWantWriter"] != null && MS.DisabilityPercent > 0)
                {
                    //MS.IsphysicalChall = frm["phyChal"].ToString();
                    MS.rdoWantWriter = frm["rdoWantWriter"].ToString();
                    if (MS.rdoWantWriter == "Yes")
                    {
                        MS.rdoWantWriter = "1";
                    }
                    else
                    {
                        MS.rdoWantWriter = "0";
                    }
                }
                else
                {
                    MS.IsphysicalChall = "N.A.";
                    MS.rdoWantWriter = "0";
                    MS.DisabilityPercent = 0;
                }

                int exmdist = Convert.ToInt32(MS.SelExamDist);
                DataSet resultEC = objDB.SelectAllTehsilEC(exmdist);
                ViewBag.MyTehsilEC = resultEC.Tables[0];
                List<SelectListItem> TehListEC = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyTehsilEC.Rows)
                {

                    TehListEC.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                }
                ViewBag.MyTehsilEC = TehListEC;
                ViewBag.MyTehsilEC2 = TehListEC;


                if (MS.Class.ToLower() == "5".ToLower() || MS.Class.ToLower() == "8".ToLower())
                {
                    MS.Choice1 = "";
                    MS.Choice2 = "";
                }
                else
                {

                    MS.Choice1 = frm["tehsilEC"].ToString();
                    MS.Choice2 = frm["tehsilEC2"].ToString();
                }


                MS.address = frm["address"].ToString();

                MS.landmark = frm["landmark"].ToString();
                MS.block = frm["block"].ToString();
                MS.SelDist = frm["SelDist"].ToString();

                int dist = Convert.ToInt32(MS.SelDist);
                DataSet result1 = objDB.SelectAllTehsil(dist);
                ViewBag.MyTehsil = result1.Tables[0];
                List<SelectListItem> TehList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
                {

                    TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                }
                ViewBag.MyTehsil = TehList;

                MS.tehsil = frm["tehsil"].ToString();
                MS.pinCode = frm["pinCode"].ToString();
                MS.category = frm["category"].ToString();

                if (MS.category == "Reappear/Compartment")
                {
                    MS.category = "R";
                }
                if (MS.category == "Division improvement")
                {
                    MS.category = "D";
                }
                if (MS.category == "Additional Subject")
                {
                    MS.category = "A";
                }
                if (MS.category == "Golden Chance Re-Appear")
                {
                    MS.category = "SR";
                }
                if (MS.category == "Golden Chance Improvement")
                {
                    MS.category = "SD";
                }
                if (MS.category == "Golden Chance Additional")
                {
                    MS.category = "SA";
                }
                if (MS.category == "A")
                {
                    MS.IsPracExam = "0";
                }
                else
                {
                    MS.IsPracExam = frm["IsPracExam"].ToString();
                    if (MS.IsPracExam == "Yes")
                    {
                        MS.IsPracExam = "1";
                    }
                    else
                    {
                        MS.IsPracExam = "0";
                    }
                }

                if (MS.category == "R" || MS.category == "SR" || MS.category == "SD" || MS.category == "SA")
                {
                    if (frm["sub1"] != null) { MS.sub1 = frm["sub1"].ToString(); }
                    else { MS.sub1 = ""; }
                    if (frm["sub2"] != null) { MS.sub2 = frm["sub2"].ToString(); }
                    else { MS.sub2 = ""; }
                    if (frm["sub3"] != null) { MS.sub3 = frm["sub3"].ToString(); }
                    else { MS.sub3 = ""; }
                    if (frm["sub4"] != null) { MS.sub4 = frm["sub4"].ToString(); }
                    else { MS.sub4 = ""; }
                    if (frm["sub5"] != null) { MS.sub5 = frm["sub5"].ToString(); }
                    else { MS.sub5 = ""; }
                    if (frm["sub6"] != null) { MS.sub6 = frm["sub6"].ToString(); }
                    else { MS.sub6 = ""; }
                    if (frm["sub7"] != null) { MS.sub7 = frm["sub7"].ToString(); }
                    else { MS.sub7 = ""; }
                    if (frm["sub8"] != null) { MS.sub8 = frm["sub8"].ToString(); }
                    else { MS.sub8 = ""; }

                }

                else if (MS.category == "A" && MS.Class == "10")
                {
                    MS.sub1 = frm["MatricSub"].ToString();
                    //MS.sub2 = frm["sub2"].ToString();
                    if (frm["MatricSub2"] != null && frm["MatricSub2"] != "0") { MS.sub2 = frm["MatricSub2"].ToString(); }
                    else { MS.sub2 = ""; }
                    MS.sub3 = "";
                    MS.sub4 = "";
                    MS.sub5 = "";
                    MS.sub6 = "";
                }
                else if (MS.category == "A" && MS.Class == "12")
                {
                    MS.sub1 = frm["TwelveSub"].ToString();
                    MS.sub2 = "";
                    MS.sub3 = "";
                    MS.sub4 = "";
                    MS.sub5 = "";
                    MS.sub6 = "";
                }



                MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);


                if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "D")
                {
                    MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                    List<SelectListItem> DMitems1 = new List<SelectListItem>();
                    if (MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub1"].ToString())
                    {
                        DMitems1.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString() });
                    }
                    else
                    {
                        DMitems1.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub1"].ToString() });
                    }
                    ViewBag.sub1Matric = new SelectList(DMitems1, "Value", "Text");

                    List<SelectListItem> DMitems2 = new List<SelectListItem>();
                    MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString();
                    if (MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub2"].ToString())
                    {
                        DMitems2.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString() });
                    }
                    else
                    {
                        DMitems2.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub2"].ToString() });
                    }
                    ViewBag.sub2Matric = new SelectList(DMitems2, "Value", "Text");

                    List<SelectListItem> DMitems3 = new List<SelectListItem>();
                    MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString();
                    if (MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub3"].ToString())
                    {
                        DMitems3.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString() });
                    }
                    else
                    {
                        DMitems3.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub3"].ToString() });
                    }
                    ViewBag.sub3Matric = new SelectList(DMitems3, "Value", "Text");

                    List<SelectListItem> DMitems4 = new List<SelectListItem>();
                    MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString();
                    if (MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub4"].ToString())
                    {
                        DMitems4.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString() });
                    }
                    else
                    {
                        DMitems4.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub4"].ToString() });
                    }
                    ViewBag.sub4Matric = new SelectList(DMitems4, "Value", "Text");

                    List<SelectListItem> DMitems5 = new List<SelectListItem>();
                    MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString();
                    if (MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub5"].ToString())
                    {
                        DMitems5.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString() });
                    }
                    else
                    {
                        DMitems5.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub5"].ToString() });
                    }
                    ViewBag.sub5Matric = new SelectList(DMitems5, "Value", "Text");

                    List<SelectListItem> DMitems6 = new List<SelectListItem>();
                    MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString();
                    if (MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub6"].ToString())
                    {
                        DMitems6.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString() });
                    }
                    else
                    {
                        DMitems6.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub6"].ToString() });
                    }
                    ViewBag.sub6Matric = new SelectList(DMitems6, "Value", "Text");

                    List<SelectListItem> DMitems7 = new List<SelectListItem>();
                    MS.sub7 = MS.StoreAllData.Tables[0].Rows[0]["rsub7code"].ToString();
                    if (MS.StoreAllData.Tables[0].Rows[0]["rsub7code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub7"].ToString())
                    {
                        DMitems7.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub7"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub7code"].ToString() });
                    }
                    else
                    {
                        DMitems7.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub7"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub7"].ToString() });
                    }
                    ViewBag.sub7Matric = new SelectList(DMitems7, "Value", "Text");

                    List<SelectListItem> DMitems8 = new List<SelectListItem>();
                    MS.sub8 = MS.StoreAllData.Tables[0].Rows[0]["rsub8code"].ToString();
                    if (MS.StoreAllData.Tables[0].Rows[0]["rsub8code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub8"].ToString())
                    {
                        DMitems8.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub8"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub8code"].ToString() });
                    }
                    else
                    {
                        DMitems8.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub8"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub8"].ToString() });
                    }
                    ViewBag.sub8Matric = new SelectList(DMitems8, "Value", "Text");
                }
                else if ((MS.category == "SR" || MS.category == "SD" || MS.category == "SA") && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                {
                    List<SelectListItem> DMitems1 = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[3].Rows)
                    {
                        DMitems1.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                    }
                    ViewBag.sub1Matric = new SelectList(DMitems1, "Value", "Text");
                }
                else if ((MS.category == "SR" || MS.category == "SD" || MS.category == "SA") && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "12")
                {
                    List<SelectListItem> DMitems1 = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[4].Rows)
                    {
                        DMitems1.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                    }
                    ViewBag.Sub1Twelve = new SelectList(DMitems1, "Value", "Text");
                }


                ViewBag.Photo = "";
                ViewBag.sign = "";
                if (MS.std_Photo != null) //  confirmation post
                {
                    stdPic = Path.GetFileName(MS.std_Photo.FileName);
                    //string Filepath = Server.MapPath("~/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo/");
                    //if (!Directory.Exists(Filepath))
                    //{
                    //    Directory.CreateDirectory(Filepath);
                    //}

                    //string pathPhoto = Path.Combine(Server.MapPath("~/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo/"), MS.refNo + "P" + ".jpg");


                    string Orgfile = MS.refNo + "P" + ".jpg";
                    string Filename = "allfiles/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo/";
                    MS.PathPhoto = "PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo/" + Orgfile;
                    string pathPhoto = MS.refNo + "P" + ".jpg";
                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            ///file.CopyTo(newMemoryStream);

                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = MS.std_Photo.InputStream,
                                Key = string.Format("allfiles/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }

                    //MS.std_Photo.SaveAs(pathPhoto);
                    ViewBag.Photo = MS.PathPhoto != "" ? MS.PathPhoto.Replace("UPLOAD2023/", "").Replace("OPEN2022", "Open2022").Replace("PHOTO", "Photo").Replace("JPG", "jpg") : "";
                    ViewBag.ImageURL = MS.PathPhoto != "" ? MS.PathPhoto.ToString().Replace("UPLOAD2023/", "").Replace("OPEN2022", "Open2022").Replace("SIGN", "Sign").Replace("JPG", "jpg") : ""; ;

                    //ViewBag.Photo = MS.PathPhoto != "" ? MS.PathPhoto.Replace("UPLOAD2023", "Upload2023").Replace("OPEN2022", "Open2022").Replace("PHOTO", "Photo").Replace("JPG", "") : "";
                    //ViewBag.ImageURL = MS.PathPhoto;
                    Session["imgPhoto"] = MS.PathPhoto;

                }
                else
                {
                    string filepathtosave = "";
                    var filePath1 = "";
                    var rp = MS.StoreAllData.Tables[0].Rows[0]["rp"].ToString().ToUpper();
                    var phtURL = MS.StoreAllData.Tables[0].Rows[0]["Photo_url"].ToString();
                    var cat = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString().ToUpper();
                    string Oldpath = "";
                    string imgBatchPath = "Batch" + MS.refNo.Substring(3, 4);

                    if (phtURL.Contains(imgBatchPath) == false)
                    {
                        if (rp == "R" && (phtURL.Contains("PvtPhoto/Photo") == true || phtURL.Contains("PvtPhoto/Batch0321/Photo") == true))
                        {
                            filePath1 = MS.imgPhoto = @"Upload2021/" + phtURL;
                            //Oldpath = Path.Combine(Server.MapPath("~/Upload2021/" + phtURL));
                        }
                        else if (rp == "R" && phtURL.Contains("PvtPhoto") == false)
                        {
                            if (!phtURL.Contains("Upload2021"))
                            {
                                filePath1 = MS.imgPhoto = @"Upload2021/" + phtURL;
                                //Oldpath = Path.Combine(Server.MapPath("~/Upload2021/" + phtURL));
                            }

                        }
                        else if (phtURL.Contains("PvtPhoto") == true && (phtURL.Contains("21/Photo") == true || phtURL.Contains("22/Photo") == true))
                        {
                            if (!phtURL.Contains("Upload2023"))
                            {
                                filePath1 = MS.imgPhoto = @"Upload2023/" + phtURL;
                                //Oldpath = Path.Combine(Server.MapPath("~/Upload2023/" + phtURL));
                            }
                        }
                        else if (phtURL.Contains("PvtPhoto") == true && phtURL.Contains("20/Photo") == true)
                        {
                            if (!phtURL.Contains("Upload2021"))
                            {
                                filePath1 = MS.imgPhoto = @"Upload2021/" + phtURL;
                                //Oldpath = Path.Combine(Server.MapPath("~/Upload2021/" + phtURL));
                            }
                        }
                        else if (rp == "O" && phtURL.Contains("PvtPhoto") == false)
                        {
                            filePath1 = MS.imgPhoto = @"" + phtURL;
                            //Oldpath = Path.Combine(Server.MapPath("~/" + phtURL));
                        }
                        else if (cat == "SR" || cat.ToUpper() == "SD")
                        {
                            if (!phtURL.Contains("Upload2023"))
                            {
                                filePath1 = MS.imgPhoto = "~/Upload2023/" + phtURL;
                                //Oldpath = Path.Combine(Server.MapPath("~/Upload2023/" + phtURL));
                            }
                        }

                        bool isExistsInBatch = phtURL.Contains(imgBatchPath);
                        if (!string.IsNullOrEmpty(filePath1) && isExistsInBatch == false)
                        // if (filePath1 != null && filePath1 != "" && filePath1.Contains("Upload") && phtURL.Contains(imgBatchPath) == false)
                        {
                            var path = Path.Combine(Server.MapPath("~/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo"), MS.refNo + "P" + ".jpg");
                            string FilepathExist = Path.Combine(Server.MapPath("~/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo"));


                            if (!Directory.Exists(FilepathExist))
                            {
                                Directory.CreateDirectory(FilepathExist);
                            }
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }

                            if (System.IO.File.Exists(Oldpath))
                            {
                                System.IO.File.Copy(Oldpath, path);
                                filepathtosave = "PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo/" + MS.refNo + "P" + ".jpg";
                                MS.PathPhoto = filepathtosave;

                                ViewBag.Photo = filepathtosave;
                                ViewBag.ImageURL = filepathtosave;
                                Session["imgPhoto"] = filepathtosave;



                                //string PhotoName = MS.refNo + "P" + ".jpg";
                                //string type = "P";
                                //string UpdatePic = new AbstractLayer.PrivateCandidateDB().Updated_PrivateCandidate_PhotoSign_ByRefNo(MS.refNo, MS.PathPhoto, type);

                            }
                            string type = "P";
                            string UpdatePic = new AbstractLayer.PrivateCandidateDB().Updated_PrivateCandidate_PhotoSign_ByRefNo(MS.refNo, MS.PathPhoto, type);
                        }
                    }

                }
                if (MS.std_Sign != null)
                {
                    stdSign = Path.GetFileName(MS.std_Sign.FileName);
                    string Orgfile = MS.refNo + "S" + ".jpg";
                    string Filename = "allfiles/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign/";
                    MS.PathSign = "PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign/" + Orgfile;
                    string pathSign = MS.refNo + "S" + ".jpg";
                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            ///file.CopyTo(newMemoryStream);

                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = MS.std_Sign.InputStream,
                                Key = string.Format("allfiles/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }

                    //stdSign = Path.GetFileName(MS.std_Sign.FileName);
                    //string Filepath = Server.MapPath("~/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign/");
                    //if (!Directory.Exists(Filepath))
                    //{
                    //    Directory.CreateDirectory(Filepath);
                    //}

                    //string pathSign = Path.Combine(Server.MapPath("~/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign/"), MS.refNo + "S" + ".jpg");
                    //MS.PathSign = "PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign/" + MS.refNo + "S" + ".jpg";
                    //MS.std_Sign.SaveAs(pathSign);
                    ViewBag.sign = MS.PathSign;
                    Session["imgSign"] = MS.PathSign;

                }
                else
                {

                    string stdSign = "";
                    string filepathtosaveSign = "";
                    var filePath1 = "";
                    var filePath1Sign = "";
                    var rp = MS.StoreAllData.Tables[0].Rows[0]["rp"].ToString().ToUpper();
                    var signURL = MS.StoreAllData.Tables[0].Rows[0]["Sign_url"].ToString();
                    var cat = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString().ToUpper();
                    string Oldpath = "", OldpathSign = "";

                    string imgBatchPath = "Batch" + MS.refNo.Substring(3, 4);


                    if (signURL.Contains(imgBatchPath) == false)
                    {
                        if (rp == "R" && (signURL.Contains("PvtPhoto/Sign") == true || signURL.Contains("PvtPhoto/Batch0321/Sign") == true))
                        {
                            if (!signURL.Contains("Upload2021"))
                            {
                                filePath1Sign = MS.imgSign = @"Upload2021/" + signURL;
                                //OldpathSign = Path.Combine(Server.MapPath("~/Upload2021/" + signURL));
                            }
                        }
                        else if (rp == "R" && signURL.Contains("PvtPhoto") == false)
                        {
                            if (!signURL.Contains("Upload2021"))
                            {
                                filePath1Sign = MS.imgSign = @"Upload2021/" + signURL;
                                //OldpathSign = Path.Combine(Server.MapPath("~/Upload2021/" + signURL));
                            }
                        }

                        else if (signURL.Contains("PvtPhoto") == true && (signURL.Contains("21/Sign") == true || signURL.Contains("22/Sign") == true))
                        {
                            if (!signURL.Contains("Upload2023"))
                            {
                                filePath1Sign = MS.imgSign = @"Upload2023/" + signURL;
                                // OldpathSign = Path.Combine(Server.MapPath("~/Upload2023/" + signURL));
                            }
                        }
                        else if (signURL.Contains("PvtPhoto") == true && signURL.Contains("19/Sign") == true)
                        {
                            if (!signURL.Contains("Upload2021"))
                            {
                                filePath1Sign = MS.imgSign = @"Upload2021/" + signURL;
                                //OldpathSign = Path.Combine(Server.MapPath("~/Upload2021/" + signURL));
                            }
                        }
                        else if (rp == "O" && signURL.Contains("PvtPhoto") == false)
                        {
                            filePath1Sign = MS.imgSign = @"" + signURL;
                            //OldpathSign = Path.Combine(Server.MapPath("~/" + signURL));
                        }
                        if (cat == "SR" || cat == "SD")
                        {
                            if (!signURL.Contains("Upload2023"))
                            {
                                filePath1Sign = MS.imgPhoto = "~/Upload2023/" + signURL;
                                //OldpathSign = Path.Combine(Server.MapPath("~/Upload2023/" + signURL));
                            }
                        }
                        if (signURL.Contains("Correction1819") == true)
                        {
                            if (!signURL.Contains("Upload2021"))
                            {
                                filePath1Sign = MS.imgSign = @"Upload2021/" + signURL;
                                //OldpathSign = Path.Combine(Server.MapPath("~/Upload2021/" + signURL));
                            }
                        }

                        bool isExistsInBatch = signURL.Contains(imgBatchPath);
                        if (!string.IsNullOrEmpty(filePath1Sign) && isExistsInBatch == false)
                        {
                            var path = Path.Combine(Server.MapPath("~/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign"), MS.refNo + "S" + ".jpg");
                            string FilepathExist = Path.Combine(Server.MapPath("~/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign"));

                            if (!Directory.Exists(FilepathExist))
                            {
                                Directory.CreateDirectory(FilepathExist);
                            }
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }

                            if (System.IO.File.Exists(OldpathSign))
                            {
                                System.IO.File.Copy(OldpathSign, path);

                                //System.IO.File.Copy(filePath1, path);
                                filepathtosaveSign = "PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign/" + MS.refNo + "S" + ".jpg";
                                ViewBag.ImageURL = filepathtosaveSign;
                                MS.PathSign = filepathtosaveSign;
                                ViewBag.sign = filepathtosaveSign;
                                Session["imgSign"] = filepathtosaveSign;

                                //string PhotoName = MS.refNo + "S" + ".jpg";
                                //string type = "S";
                                //string UpdatePic = new AbstractLayer.PrivateCandidateDB().Updated_PrivateCandidate_PhotoSign_ByRefNo(MS.refNo, MS.PathSign, type);
                            }
                            string type = "S";
                            string UpdatePic = new AbstractLayer.PrivateCandidateDB().Updated_PrivateCandidate_PhotoSign_ByRefNo(MS.refNo, MS.PathSign, type);
                        }
                    }
                }

                MS.PathPhoto = MS.PathPhoto = Session["imgPhoto"].ToString();
                MS.PathSign = MS.PathSign = Session["imgSign"].ToString();

                if (MS.category == "D")
                {
                    if (frm["sub1"] != null) { MS.sub1 = frm["sub1"].ToString(); }
                    else { MS.sub1 = ""; }
                    if (frm["sub2"] != null) { MS.sub2 = frm["sub2"].ToString(); }
                    else { MS.sub2 = ""; }
                    if (frm["sub3"] != null) { MS.sub3 = frm["sub3"].ToString(); }
                    else { MS.sub3 = ""; }
                    if (frm["sub4"] != null) { MS.sub4 = frm["sub4"].ToString(); }
                    else { MS.sub4 = ""; }
                    if (frm["sub5"] != null) { MS.sub5 = frm["sub5"].ToString(); }
                    else { MS.sub5 = ""; }
                    if (frm["sub6"] != null) { MS.sub6 = frm["sub6"].ToString(); }
                    else { MS.sub6 = ""; }
                    if (frm["sub7"] != null) { MS.sub7 = frm["sub7"].ToString(); }
                    else { MS.sub7 = ""; }
                    if (frm["sub8"] != null) { MS.sub8 = frm["sub8"].ToString(); }
                    else { MS.sub8 = ""; }

                }

                int flag = 0;
                if (MS.PathPhoto != "" || MS.PathSign != "")
                {

                }
                else
                {
                    if ((MS.std_Photo == null || MS.std_Sign == null) && (MS.imgPhoto == null || MS.imgSign == null))
                    {
                        MS.category = frm["category"].ToString();
                        MS.Class = frm["Class"].ToString();
                        MS.Gender = frm["Gender"].ToString();
                        MS.Area = frm["Area"].ToString();
                        MS.Relist = frm["Relist"].ToString();
                        MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                        ViewBag.Message = "Select Photo and Sign";
                        TempData["SelectPhotoSign"] = "0";
                        flag = 1;
                    }
                }

                if (MS.Gender == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select Gender";
                    TempData["SelectGender"] = "0";
                    flag = 1;
                }

                if (MS.CastList == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select Cast";
                    TempData["SelectCast"] = "0";
                    flag = 1;
                }
                if (MS.Area == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select Area";
                    TempData["SelectArea"] = "0";
                    flag = 1;
                }
                if (MS.Relist == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select Religion";
                    TempData["SelectRelist"] = "0";
                    flag = 1;
                }

                if (MS.address == "")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Enter Address";
                    TempData["Selectaddress"] = "0";
                    flag = 1;
                }
                if (MS.SelDist == "" || MS.tehsil == "" || MS.tehsil == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select District & Tehsil";
                    TempData["SelectDist"] = "0";
                    return View(MS);
                }
                if (MS.pinCode == "" || MS.pinCode.Count() != 6)
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Enter Pin Code";
                    TempData["SelectPin"] = "0";
                    flag = 1;
                }



                MS.IsHardCopyCertificate = frm["IsHardCopyCertificate"].ToString();
                if (string.IsNullOrEmpty(MS.IsHardCopyCertificate))
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select Hard Copy of Certificate";
                    TempData["SelectPin"] = "0";
                    flag = 1;
                }


                if (flag == 0)
                {
                    string OutError = "";
                    DataSet result2 = AbstractLayer.PrivateCandidateDB.InsertPrivateCandidateConfirmation(MS, out OutError);
                    if (OutError == "1")
                    {
                        TempData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                        TempData["Status"] = "3";
                        TempData["OutError"] = "";
                    }
                    else
                    {
                        TempData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                        TempData["Status"] = "ERR";
                        TempData["OutError"] = OutError;
                    }
                }
                return RedirectToAction("PrivateCandidateConfirmation", "PrivateCandidate");
                //return View(MS);
            }
            catch (Exception ex)
            {
                return Private_Candidate_Examination_Form();
            }
        }


        public ActionResult PrivateCandidateConfirmationEdit()
        {
            try
            {
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                PrivateCandidateModels MS = new PrivateCandidateModels();

                DataSet result = objDB.SelectDist(); // passing Value to DBClass from model
                                                     //ViewBag.MyDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                                                     //List<SelectListItem> items = new List<SelectListItem>();
                                                     //foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                                                     //{
                                                     //    items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                                                     //}
                                                     //// ViewBag.MyDist = items;
                                                     //ViewBag.MyDist = new SelectList(items, "Value", "Text", 047);

                if (Session["refno"].ToString() == null || Session["refno"].ToString() == "")
                {
                    return Private_Candidate_Examination_Form();
                }
                string Oroll = Session["Oroll"].ToString();
                string RefNo = Session["refno"].ToString();
                MS.OROLL = Oroll;
                MS.refNo = RefNo;
                int result1 = objDB.EditPrivateCandidateConfirmation(MS);
                if (result1 > 0)
                {
                    //MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    //Session["OROLL"] = MS.OROLL;
                    //return RedirectToAction("PrivateCandidateConfirmation", "PrivateCandidate");

                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        return View();
                    }
                    else
                    {
                        if (MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"] == null || MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"].ToString() == "")
                        {
                            MS.DisabilityPercent = 0;
                        }
                        else { MS.DisabilityPercent = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"].ToString()); }
                        MS.IsHardCopyCertificate = MS.StoreAllData.Tables[0].Rows[0]["IsHardCopyCertificate"].ToString();
                        MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["rp"].ToString();
                        //MS.category = MS.StoreAllData.Tables[1].Rows[0]["category"].ToString();
                        if (MS.Exam_Type == "R")
                        {
                            MS.Exam_Type = "Regular";
                        }
                        if (MS.Exam_Type == "O")
                        {
                            MS.Exam_Type = "Open";
                        }
                        if (MS.Exam_Type == "P")
                        {
                            MS.Exam_Type = "Private";
                        }
                        //MS.category = MS.StoreAllData.Tables[1].Rows[0]["category"].ToString();
                        MS.category = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                        if (MS.category == "R")
                        {
                            MS.category = "Reappear/Compartment";
                        }
                        if (MS.category == "D")
                        {
                            MS.category = "Division improvement";
                        }
                        if (MS.category == "A")
                        {
                            MS.category = "Additional Subject";
                        }
                        if (MS.category == "SR")
                        {
                            MS.category = "Golden Chance Re-Appear";
                        }
                        if (MS.category == "SD")
                        {
                            MS.category = "Golden Chance Improvement";
                        }
                        if (MS.category == "SA")
                        {
                            MS.category = "Golden Chance Additional";
                        }
                        MS.Class = MS.StoreAllData.Tables[0].Rows[0]["class"].ToString();
                        if (MS.Class == "10")
                        {
                            MS.Class = "Matriculation";
                            MS.MatricSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            MS.MatricSub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString();
                        }
                        else if (MS.Class == "12")
                        {
                            MS.TwelveSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            MS.Class = MS.StoreAllData.Tables[0].Rows[0]["classNM"].ToString();
                        }
                        else if (MS.Class == "5" || MS.Class == "8")
                        {
                            MS.TwelveSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            MS.Class = MS.StoreAllData.Tables[0].Rows[0]["classNM"].ToString();
                        }

                        MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                        MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
                        MS.Session = MS.SelMonth + ' ' + MS.SelYear;

                        MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                        Session["OROLL"] = MS.OROLL;
                        Session["category"] = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();

                        MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();
                        MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();

                        MS.refNo = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                        MS.SCHL = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();

                        MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                        MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                        MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();

                        MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();
                        MS.Result = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();

                        MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
                        MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
                        MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();

                        MS.EPname = MS.Candi_Name + '/' + MS.Pname;
                        MS.EPFname = MS.Father_Name + '/' + MS.PFname;
                        MS.EPMname = MS.Mother_Name + '/' + MS.PMname;

                        MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["sex"].ToString();
                        MS.CastList = MS.StoreAllData.Tables[0].Rows[0]["caste"].ToString();
                        MS.Area = MS.StoreAllData.Tables[0].Rows[0]["area"].ToString();
                        if (MS.Area == "U")
                        {
                            MS.Area = "Urban";
                        }
                        if (MS.Area == "R")
                        {
                            MS.Area = "Rural";
                        }
                        MS.Relist = MS.StoreAllData.Tables[0].Rows[0]["religion"].ToString();
                        if (MS.Relist == "H")
                        {
                            MS.Relist = "Hindu";
                        }
                        if (MS.Relist == "M")
                        {
                            MS.Relist = "Muslim";
                        }
                        if (MS.Relist == "S")
                        {
                            MS.Relist = "Sikh";
                        }
                        if (MS.Relist == "C")
                        {
                            MS.Relist = "Christian";
                        }
                        if (MS.Relist == "O")
                        {
                            MS.Relist = "Others";
                        }

                        //MS.phyChal= MS.StoreAllData.Tables[0].Rows[0]["phy_chal"].ToString();
                        //if (MS.phyChal == "true")
                        //{
                        //    MS.phyChal = "true";
                        //}
                        //MS.rdoWantWriter = MS.StoreAllData.Tables[0].Rows[0]["writer"].ToString();
                        //if (MS.rdoWantWriter == "True")
                        //{
                        //    MS.rdoWantWriter = "true";
                        //}

                        //@ViewBag.DA = objDB.GetDA();
                        MS.phyChal = MS.StoreAllData.Tables[0].Rows[0]["phy_chal"].ToString();

                        //if (MS.phyChal != "")
                        //{
                        //    @ViewBag.DA = MS.phyChal;
                        //}

                        MS.rdoWantWriter = MS.StoreAllData.Tables[0].Rows[0]["writer"].ToString();
                        if (MS.rdoWantWriter == "True")
                        {
                            MS.rdoWantWriter = "1";
                        }
                        if (MS.rdoWantWriter == "False")
                        {
                            MS.rdoWantWriter = "0";
                        }

                        MS.IsPracExam = MS.StoreAllData.Tables[0].Rows[0]["prac"].ToString();
                        if (MS.IsPracExam == "True")
                        {
                            MS.IsPracExam = "1";
                        }
                        if (MS.IsPracExam == "False")
                        {
                            MS.IsPracExam = "0";
                        }



                        MS.address = MS.StoreAllData.Tables[0].Rows[0]["homeadd"].ToString();

                        MS.landmark = MS.StoreAllData.Tables[0].Rows[0]["LandMark"].ToString();
                        MS.block = MS.StoreAllData.Tables[0].Rows[0]["block"].ToString();
                        //MS.SelDist = MS.StoreAllData.Tables[0].Rows[0]["distName"].ToString();
                        //MS.tehsil = MS.StoreAllData.Tables[0].Rows[0]["tehsilName"].ToString();

                        MS.SelDist = MS.StoreAllData.Tables[0].Rows[0]["homedistco"].ToString();
                        MS.tehsil = MS.StoreAllData.Tables[0].Rows[0]["tehsil"].ToString();
                        MS.pinCode = MS.StoreAllData.Tables[0].Rows[0]["pincode"].ToString();
                        DataSet Distresult = objDB.SelectDist(); // passing Value to DBClass from model
                        ViewBag.MyDist = Distresult.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> items = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                        {
                            items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                        }
                        // ViewBag.MyDist = items;
                        ViewBag.MyDist = new SelectList(items, "Value", "Text");

                        MS.SelExamDist = MS.StoreAllData.Tables[0].Rows[0]["dist"].ToString();
                        ViewBag.MyExamDist = result.Tables[0];// Edit Mode for dislaying message after saving storing output.
                        List<SelectListItem> items1 = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyExamDist.Rows)
                        {
                            items1.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                        }
                        // ViewBag.MyDist = items;
                        ViewBag.MyExamDist = new SelectList(items1, "Value", "Text");

                        if (MS.SelExamDist == "")
                        {
                            MS.SelExamDist = "0";
                        }

                        //MS.Choice1 = MS.StoreAllData.Tables[0].Rows[0]["cent_1"].ToString();
                        //MS.Choice2 = MS.StoreAllData.Tables[0].Rows[0]["cent_2"].ToString();
                        int exmdist = Convert.ToInt32(MS.SelExamDist);
                        DataSet resultEC = objDB.SelectAllTehsilEC(exmdist);
                        ViewBag.MyTehsilEC = resultEC.Tables[0];
                        ViewBag.MyTehsilEC2 = resultEC.Tables[0];
                        List<SelectListItem> TehListEC = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyTehsilEC.Rows)
                        {

                            TehListEC.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                        }

                        ViewBag.MyTehsilEC = TehListEC;
                        ViewBag.MyTehsilEC2 = TehListEC;

                        MS.tehsilEC = MS.StoreAllData.Tables[0].Rows[0]["Cent_1"].ToString();
                        MS.tehsilEC2 = MS.StoreAllData.Tables[0].Rows[0]["Cent_2"].ToString();
                        //MS.Choice1 = "";
                        //MS.Choice2 = "";

                        //ViewBag.MyTehsil = MS.tehsil.ToString();

                        int dist = Convert.ToInt32(MS.SelDist);
                        DataSet Tehresult = objDB.SelectAllTehsil(dist);
                        ViewBag.MyTehsil = Tehresult.Tables[0];
                        List<SelectListItem> TehList = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
                        {

                            TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                        }
                        ViewBag.MyTehsil = TehList;



                        if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "R")
                        {
                            MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                            MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                            MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString();
                            MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString();
                            MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString();
                            MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString();
                            MS.sub7 = MS.StoreAllData.Tables[0].Rows[0]["rsub7"].ToString();
                            MS.sub8 = MS.StoreAllData.Tables[0].Rows[0]["rsub8"].ToString();
                        }
                        if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "SR" || MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "SD" || MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "SA")
                        {
                            MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString();
                            MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString();
                            MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString();
                            MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString();
                            MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString();
                            MS.sub7 = MS.StoreAllData.Tables[0].Rows[0]["rsub7code"].ToString();
                            MS.sub8 = MS.StoreAllData.Tables[0].Rows[0]["rsub8code"].ToString();
                        }
                        if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "D")
                        {
                            MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            List<SelectListItem> DMitems1 = new List<SelectListItem>();
                            if (MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub1"].ToString())
                            {
                                DMitems1.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString() });
                            }
                            else
                            {
                                DMitems1.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub1"].ToString() });
                            }
                            ViewBag.sub1Matric = new SelectList(DMitems1, "Value", "Text");

                            List<SelectListItem> DMitems2 = new List<SelectListItem>();
                            MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString();
                            if (MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub2"].ToString())
                            {
                                DMitems2.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString() });
                            }
                            else
                            {
                                DMitems2.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub2"].ToString() });
                            }
                            ViewBag.sub2Matric = new SelectList(DMitems2, "Value", "Text");

                            List<SelectListItem> DMitems3 = new List<SelectListItem>();
                            MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString();
                            if (MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub3"].ToString())
                            {
                                DMitems3.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString() });
                            }
                            else
                            {
                                DMitems3.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub3"].ToString() });
                            }
                            ViewBag.sub3Matric = new SelectList(DMitems3, "Value", "Text");

                            List<SelectListItem> DMitems4 = new List<SelectListItem>();
                            MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString();
                            if (MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub4"].ToString())
                            {
                                DMitems4.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString() });
                            }
                            else
                            {
                                DMitems4.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub4"].ToString() });
                            }
                            ViewBag.sub4Matric = new SelectList(DMitems4, "Value", "Text");

                            List<SelectListItem> DMitems5 = new List<SelectListItem>();
                            MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString();
                            if (MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub5"].ToString())
                            {
                                DMitems5.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString() });
                            }
                            else
                            {
                                DMitems5.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub5"].ToString() });
                            }
                            ViewBag.sub5Matric = new SelectList(DMitems5, "Value", "Text");

                            List<SelectListItem> DMitems6 = new List<SelectListItem>();
                            MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString();
                            if (MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub6"].ToString())
                            {
                                DMitems6.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString() });
                            }
                            else
                            {
                                DMitems6.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub6"].ToString() });
                            }
                            ViewBag.sub6Matric = new SelectList(DMitems6, "Value", "Text");

                            List<SelectListItem> DMitems7 = new List<SelectListItem>();
                            MS.sub7 = MS.StoreAllData.Tables[0].Rows[0]["rsub7code"].ToString();
                            if (MS.StoreAllData.Tables[0].Rows[0]["rsub7code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub7"].ToString())
                            {
                                DMitems7.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub7"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub7code"].ToString() });
                            }
                            else
                            {
                                DMitems7.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub7"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub7"].ToString() });
                            }
                            ViewBag.sub7Matric = new SelectList(DMitems7, "Value", "Text");

                            List<SelectListItem> DMitems8 = new List<SelectListItem>();
                            MS.sub8 = MS.StoreAllData.Tables[0].Rows[0]["rsub8code"].ToString();
                            if (MS.StoreAllData.Tables[0].Rows[0]["rsub8code"].ToString() == MS.StoreAllData.Tables[0].Rows[0]["sub8"].ToString())
                            {
                                DMitems8.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub8"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["rsub8code"].ToString() });
                            }
                            else
                            {
                                DMitems8.Add(new SelectListItem { Text = MS.StoreAllData.Tables[0].Rows[0]["rsub8"].ToString(), Value = MS.StoreAllData.Tables[0].Rows[0]["sub8"].ToString() });
                            }
                            ViewBag.sub8Matric = new SelectList(DMitems8, "Value", "Text");

                        }
                        if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                        {
                            List<SelectListItem> Mitems = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[1].Rows)
                            {
                                Mitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.SubMatric = new SelectList(Mitems, "Value", "Text");
                        }
                        if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "12")
                        {
                            List<SelectListItem> sitems = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[2].Rows)
                            {
                                sitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.SubTwelve = new SelectList(sitems, "Value", "Text");
                        }
                        //if (MS.category == "SR" || MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                        //{
                        //    List<SelectListItem> DMitems1 = new List<SelectListItem>();
                        //    foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[3].Rows)
                        //    {
                        //        DMitems1.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        //    }
                        //    ViewBag.subMatric = new SelectList(DMitems1, "Value", "Text");
                        //}
                        //if (MS.category == "SR" || MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "12")
                        //{
                        //    List<SelectListItem> DMitems1 = new List<SelectListItem>();
                        //    foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[4].Rows)
                        //    {
                        //        DMitems1.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                        //    }
                        //    ViewBag.SubTwelve = new SelectList(DMitems1, "Value", "Text");
                        //}
                        if ((MS.category == "SR" || MS.category == "SD" || MS.category == "SA") || MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                        {
                            List<SelectListItem> DMitems1 = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[3].Rows)
                            {
                                DMitems1.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.sub1Matric = new SelectList(DMitems1, "Value", "Text");
                        }
                        if ((MS.category == "SR" || MS.category == "SD" || MS.category == "SA") || MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "12")
                        {
                            List<SelectListItem> DMitems1 = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[4].Rows)
                            {
                                DMitems1.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.Sub1Twelve = new SelectList(DMitems1, "Value", "Text");
                        }
                        if (MS.category == "SR")
                        {
                            MS.category = "Golden Chance Re-Appear";
                        }
                        if (MS.category == "SD")
                        {
                            MS.category = "Golden Chance Improvement";
                        }
                        if (MS.category == "SA")
                        {
                            MS.category = "Golden Chance Additional";
                        }
                        MS.imgPhoto = MS.StoreAllData.Tables[0].Rows[0]["Photo_url"].ToString();
                        MS.imgSign = MS.StoreAllData.Tables[0].Rows[0]["Sign_url"].ToString();

                        ViewBag.PhotoExist = "0";
                        ViewBag.signExist = "0";

                        if (MS.StoreAllData.Tables[0].Rows[0]["rp"].ToString().ToUpper() == "R" && MS.imgPhoto.Contains("PvtPhoto") == false)
                        {
                            @ViewBag.Photo = "https://psebdata.s3.ap-south-1.amazonaws.com/allfiles/Upload2023/" + MS.imgPhoto;
                            @ViewBag.sign = "https://psebdata.s3.ap-south-1.amazonaws.com/allfiles/Upload2023/" + MS.imgSign;
                            MS.PathPhoto = ViewBag.Photo;
                            MS.PathSign = ViewBag.sign;
                            MS.imgPhoto = ViewBag.Photo;
                            MS.imgSign = ViewBag.sign;

                            //filePath1 = MS.imgPhoto = @"upload2017/" + MS.StoreAllData.Tables[0].Rows[0]["Photo_url"].ToString();
                            string PhotoExist = "https://psebdata.s3.ap-south-1.amazonaws.com/allfiles/Upload2023/" + MS.imgPhoto;
                            string SignExist = "https://psebdata.s3.ap-south-1.amazonaws.com/allfiles/Upload2023/" + MS.imgSign;
                            ViewBag.PhotoExist = System.IO.File.Exists(PhotoExist) ? "1" : "0";
                            ViewBag.signExist = System.IO.File.Exists(SignExist) ? "1" : "0";
                        }
                        else if (MS.StoreAllData.Tables[0].Rows[0]["rp"].ToString().ToUpper() == "O" && MS.imgPhoto.Contains("PvtPhoto") == false)
                        {
                            @ViewBag.Photo = "https://registration2022.pseb.ac.in/Upload/" + MS.imgPhoto;
                            @ViewBag.sign = "https://registration2022.pseb.ac.in/Upload/" + MS.imgSign;
                            MS.PathPhoto = ViewBag.Photo;
                            MS.PathSign = ViewBag.sign;
                            MS.imgPhoto = ViewBag.Photo;
                            MS.imgSign = ViewBag.sign;

                            //filePath1 = MS.imgPhoto = @"upload2017/" + MS.StoreAllData.Tables[0].Rows[0]["Photo_url"].ToString();
                            string PhotoExist = "https://psebdata.s3.ap-south-1.amazonaws.com/allfiles/" + MS.imgPhoto.Replace("https://registration2022.pseb.ac.in/", "");
                            string SignExist = "https://psebdata.s3.ap-south-1.amazonaws.com/allfiles/" + MS.imgSign.Replace("https://registration2022.pseb.ac.in/", "");
                            ViewBag.PhotoExist = System.IO.File.Exists(PhotoExist) ? "1" : "0";
                            ViewBag.signExist = System.IO.File.Exists(SignExist) ? "1" : "0";
                        }
                        else
                        {
                            @ViewBag.Photo = MS.imgPhoto;
                            @ViewBag.sign = MS.imgSign;

                            string PhotoExist = "https://psebdata.s3.ap-south-1.amazonaws.com/allfiles/Upload2023/" + MS.imgPhoto;
                            string SignExist = "https://psebdata.s3.ap-south-1.amazonaws.com/allfiles/Upload2023/" + MS.imgSign;
                            ViewBag.PhotoExist = System.IO.File.Exists(PhotoExist) ? "1" : "0";
                            ViewBag.signExist = System.IO.File.Exists(SignExist) ? "1" : "0";
                        }

                        Session["imgPhoto"] = MS.imgPhoto;
                        Session["imgSign"] = MS.imgSign;


                        MS.FormStatus = MS.StoreAllData.Tables[0].Rows[0]["FormStatus"].ToString();

                        return View(MS);
                        //return Private_Candidate_Examination_Form();
                        //return Pre_Private_Candidate_Introduction_Form(frm);
                    }
                }
                else
                {
                    //return View(MS);
                    TempData["Status"] = "11";
                    //return Private_Candidate_Examination_Form();
                    return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
                }
            }
            catch (Exception ex)
            {
                //return Private_Candidate_Examination_Form();
                return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
            }
        }
        public ActionResult PrivateCandidateFinalPrint(FormCollection frc)
        {
            PrivateCandidateModels MS = new PrivateCandidateModels();

            try
            {
                if (Session["OROLL"].ToString() != null || Session["OROLL"].ToString() != "")
                {
                    if (Session["refNo"].ToString().Substring(5, 2) == "22")
                    {
                        Session["Session"] = "2023-2024";
                    }
                    else if (Session["refNo"].ToString().Substring(5, 2) == "21")
                    {
                        Session["Session"] = "2021-2022";
                    }


                    AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();

                    MS.OROLL = Session["OROLL"].ToString();
                    MS.refNo = Session["refNo"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmationPrint(MS.refNo);
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                    ///----------------------------------------------------------------
                    ///
                    MS.centrCode = MS.StoreAllData.Tables[0].Rows[0]["cent"].ToString();
                    MS.setNo = MS.StoreAllData.Tables[0].Rows[0]["set"].ToString();
                    MS.OsetNo = MS.StoreAllData.Tables[0].Rows[0]["Oset"].ToString();
                    MS.SelExamDist = MS.StoreAllData.Tables[0].Rows[0]["ExamDist"].ToString();
                    MS.distName = MS.StoreAllData.Tables[0].Rows[0]["DISTNM"].ToString();
                    MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["utype"].ToString();
                    MS.category = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                    MS.Stream = MS.StoreAllData.Tables[0].Rows[0]["stream"].ToString();

                    if (MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"] == null || MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"].ToString() == "")
                    {
                        MS.DisabilityPercent = 0;
                    }
                    else { MS.DisabilityPercent = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"].ToString()); }
                    MS.IsHardCopyCertificate = MS.StoreAllData.Tables[0].Rows[0]["IsHardCopyCertificate"].ToString();

                    if (MS.category == "R")
                    {
                        MS.category = "Reappear/Compartment";
                    }
                    if (MS.category == "D")
                    {
                        MS.category = "Division improvement";
                    }
                    if (MS.category == "A")
                    {
                        MS.category = "Additional Subject";
                    }
                    MS.phyChal = MS.StoreAllData.Tables[0].Rows[0]["phy_chal"].ToString();


                    MS.Class = MS.StoreAllData.Tables[0].Rows[0]["Class"].ToString();
                    if (MS.Class.ToLower() == "5".ToLower() || MS.Class.ToLower() == "8".ToLower())
                    {
                        MS.Choice1 = "";
                        MS.Choice2 = "";
                    }
                    else
                    {
                        MS.Choice1 = MS.StoreAllData.Tables[0].Rows[0]["cent_1"].ToString();
                        MS.Choice2 = MS.StoreAllData.Tables[0].Rows[0]["cent_2"].ToString();
                    }

                    //@ViewBag.Photo = "../../upload/" + MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
                    //@ViewBag.sign = "../../upload/" + MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();

                    @ViewBag.Photo = MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString() != "" ? MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString().Replace("UPLOAD2021", "").Replace("Upload2021/", "").Replace("Upload2021", "").Replace("UPLOAD2022/ ", "").Replace("UPLOAD2023/ ", "").Replace("OPEN2021", "Open2021").Replace("OPEN2022", "Open2022").Replace("PHOTO", "Photo").Replace("JPG", "jpg").Replace("upload//", "") : "";
                    @ViewBag.sign = MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString() != "" ? MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString().Replace("UPLOAD2022/", "").Replace("UPLOAD2021", "").Replace("Upload2021/", "").Replace("Upload2021", "").Replace("Upload2021", "").Replace("UPLOAD2021/", "").Replace("UPLOAD2021/", "").Replace("UPLOAD2022/", "").Replace("UPLOAD2023/", "").Replace("OPEN2021", "Open2021").Replace("OPEN2022", "Open2022").Replace("SIGN", "Sign").Replace("JPG", "jpg").Replace("upload//", "") : "";

                    //@ViewBag.Photo = MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
                    //@ViewBag.sign = MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();

                    MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                    MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
                    MS.Session = MS.SelMonth + '/' + MS.SelYear;
                    MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                    MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();

                    MS.Fee = MS.StoreAllData.Tables[0].Rows[0]["FEE"].ToString();
                    MS.challanNo = MS.StoreAllData.Tables[0].Rows[0]["challanid"].ToString();
                    MS.BANK = MS.StoreAllData.Tables[0].Rows[0]["BANK"].ToString();
                    MS.BRANCH = MS.StoreAllData.Tables[0].Rows[0]["BRANCH"].ToString();
                    MS.DEPOSITDT = MS.StoreAllData.Tables[0].Rows[0]["DEPOSITDT"].ToString();


                    MS.refNo = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                    MS.SCHL = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                    MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                    MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                    MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();
                    MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
                    MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
                    MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();
                    MS.EPname = MS.Candi_Name + '/' + MS.Pname;
                    MS.EPFname = MS.Father_Name + '/' + MS.PFname;
                    MS.EPMname = MS.Mother_Name + '/' + MS.PMname;

                    MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["sex"].ToString();
                    MS.CastList = MS.StoreAllData.Tables[0].Rows[0]["caste"].ToString();
                    MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                    MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();
                    MS.Result = MS.StoreAllData.Tables[0].Rows[0]["result"].ToString();
                    MS.SearchResult = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();

                    MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();
                    MS.Relist = MS.StoreAllData.Tables[0].Rows[0]["religion"].ToString();
                    if (MS.Relist == "H")
                    {
                        MS.Relist = "Hindu";
                    }
                    if (MS.Relist == "M")
                    {
                        MS.Relist = "Muslim";
                    }
                    if (MS.Relist == "S")
                    {
                        MS.Relist = "Sikh";
                    }
                    if (MS.Relist == "C")
                    {
                        MS.Relist = "Christian";
                    }
                    if (MS.Relist == "O")
                    {
                        MS.Relist = "Others";
                    }

                    MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                    MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                    MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString();
                    MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString();
                    MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString();
                    MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString();
                    MS.sub7 = MS.StoreAllData.Tables[0].Rows[0]["rsub7"].ToString();
                    MS.sub8 = MS.StoreAllData.Tables[0].Rows[0]["rsub8"].ToString();

                    MS.sub1code = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                    MS.sub2code = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString();
                    MS.sub3code = MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString();
                    MS.sub4code = MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString();
                    MS.sub5code = MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString();
                    MS.sub6code = MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString();
                    MS.sub7code = MS.StoreAllData.Tables[0].Rows[0]["rsub7code"].ToString();
                    MS.sub8code = MS.StoreAllData.Tables[0].Rows[0]["rsub8code"].ToString();

                    MS.address = MS.StoreAllData.Tables[0].Rows[0]["homeadd"].ToString();
                    MS.block = MS.StoreAllData.Tables[0].Rows[0]["block"].ToString();
                    MS.tehsil = MS.StoreAllData.Tables[0].Rows[0]["TEHSILENM"].ToString();
                    MS.pinCode = MS.StoreAllData.Tables[0].Rows[0]["pinCode"].ToString();

                    MS.addressfull = (MS.address + ',' + MS.block + ',' + MS.distName + ',' + MS.tehsil + ',' + MS.pinCode);




                    return View(MS);
                }
                else
                {
                    return Private_Candidate_Examination_Form();
                }
            }
            catch (Exception ex)
            {
                return Private_Candidate_Examination_Form();
            }
        }

        public ActionResult UpdateSyncResultDetails(PrivateCandidateModels MS)
        {
            if (Session["refno"] == null || Session["Oroll"] == null || Session["Oroll"].ToString() == "" || Session["form"] == null)
            {
                return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
            }

            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            string Oroll = Session["Oroll"].ToString();
            string refno = Session["refno"].ToString();

            MS.StoreAllData = objDB.SetUpdateSyncResultDetails(refno, Oroll);
            ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
            TempData["Status"] = MS.StoreAllData.Tables[0].Rows[0]["status"].ToString();
            return RedirectToAction("PrivateCandidateConfirmation", "PrivateCandidate");
        }

        public JsonResult GetTehID(int DIST) // Calling on http post (on Submit)
        {
            //AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();

            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            PrivateCandidateModels MS = new PrivateCandidateModels();

            DataSet result = objDB.SelectAllTehsil(DIST); // passing Value to DBClass from model

            ViewBag.MyTeh = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> TehList = new List<SelectListItem>();
            //List<string> items = new List<string>();
            TehList.Add(new SelectListItem { Text = "---Select Tehsil---", Value = "0" });
            foreach (System.Data.DataRow dr in ViewBag.MyTeh.Rows)
            {

                TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

            }
            ViewBag.MyTeh = TehList;

            return Json(TehList);

        }
        public JsonResult GetExamTehID(int DIST) // Calling on http post (on Submit)
        {
            //AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();

            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            PrivateCandidateModels MS = new PrivateCandidateModels();

            DataSet result = objDB.SelectAllTehsilEC(DIST); // passing Value to DBClass from model

            ViewBag.MyTehEC = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> TehList = new List<SelectListItem>();
            //List<string> items = new List<string>();
            TehList.Add(new SelectListItem { Text = "---Select Tehsil---", Value = "0" });
            foreach (System.Data.DataRow dr in ViewBag.MyTehEC.Rows)
            {

                TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

            }
            ViewBag.MyTehEC = TehList;

            return Json(TehList);

        }


        public JsonResult GetMonthID(string CATE) // Calling on http post (on Submit)
        {
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            PrivateCandidateModels MS = new PrivateCandidateModels();
            if (CATE == "A")
            {
                List<SelectListItem> Monthlist = objDB.GetMonth();
                ViewBag.MyMonth = Monthlist;
                return Json(Monthlist);
            }
            else
            {
                List<SelectListItem> Monthlist = objDB.GetMonth();
                ViewBag.MyMonth = Monthlist;
                return Json(Monthlist);
                //DataSet Monthlist1 = objDB.GetSessionMonth();
                //ViewBag.MyMonth = Monthlist1.Tables[0];
                //List<SelectListItem> Monthlist = new List<SelectListItem>();
                ////Monthlist.Add(new SelectListItem { Text = "Select Month", Value = "0" });
                //foreach (System.Data.DataRow dr in ViewBag.MyMonth.Rows)
                //{
                //    Monthlist.Add(new SelectListItem { Text = @dr["sessionMonth"].ToString(), Value = @dr["sessionMonth"].ToString() });
                //}
                //ViewBag.MyMonth = Monthlist;
                //return Json(Monthlist);
            }
        }
        public JsonResult GetYearID(string CATE) // Calling on http post (on Submit)
        {
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            PrivateCandidateModels MS = new PrivateCandidateModels();

            if (CATE == "A")
            {
                List<SelectListItem> yearlist = objDB.GetSessionYear1();
                //yearlist.Add(new SelectListItem { Text = "Select Year", Value = "0" });
                yearlist.Reverse();
                ViewBag.MyYear = yearlist;
                return Json(yearlist);
            }

            else if (CATE == "D")
            {
                DataSet yearlist1 = objDB.GetSessionYear();
                ViewBag.MyYear = yearlist1.Tables[0];
                List<SelectListItem> yearlist = new List<SelectListItem>();
                //yearlist.Add(new SelectListItem { Text = "Select Year", Value = "0" });
                foreach (System.Data.DataRow dr in ViewBag.MyYear.Rows)
                {
                    yearlist.Add(new SelectListItem { Text = @dr["sessionYear"].ToString(), Value = @dr["sessionYear"].ToString() });
                }
                yearlist.RemoveAt(2);
                ViewBag.MyYear = yearlist;
                return Json(yearlist);
            }
            else
            {
                DataSet yearlist1 = objDB.GetSessionYear();
                ViewBag.MyYear = yearlist1.Tables[0];
                List<SelectListItem> yearlist = new List<SelectListItem>();
                //yearlist.Add(new SelectListItem { Text = "Select Year", Value = "0" });
                foreach (System.Data.DataRow dr in ViewBag.MyYear.Rows)
                {
                    yearlist.Add(new SelectListItem { Text = @dr["sessionYear"].ToString(), Value = @dr["sessionYear"].ToString() });
                }
                ViewBag.MyYear = yearlist;
                return Json(yearlist);
            }

        }
        //----------------------------------------- Challan ---------------------------
        public ActionResult PaymentForm(string roll)
        {
            PrivatePaymentformViewModel pfvm = new PrivatePaymentformViewModel();

            if (Session["refno"] == null || Session["Oroll"] == null || Session["Oroll"].ToString() == "" || Session["form"] == null)
            {
                return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
            }

            roll = Session["Oroll"].ToString();
            string RefNo = Session["refno"].ToString();
            if (Session["refNo"].ToString().Substring(5, 2) == "22")
            {
                Session["Session"] = "2023-2024";
            }
            else if (Session["refNo"].ToString().Substring(5, 2) == "21")
            {
                Session["Session"] = "2021-2022";
            }



            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            // ViewBag.BankList = objCommon.GetBankList();
            //string schl = string.Empty;
            //schl = Session["SCHL"].ToString();
            string form = Session["form"].ToString();
            DataSet ds = objDB.GetPrivateCandidateDetailsPayment(RefNo, form);
            pfvm.PaymentFormData = ds;
            if (pfvm.PaymentFormData == null || pfvm.PaymentFormData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                return View();
            }
            else
            {
                //pfvm.LOTNo = Convert.ToInt32(ds.Tables[0].Rows[0]["LOT"].ToString());

                pfvm.Class = ds.Tables[0].Rows[0]["class"].ToString();
                if (pfvm.Class == "10")
                {
                    pfvm.Class = "Matriculation";

                }
                else if (pfvm.Class == "12")
                {
                    pfvm.Class = "Senior Secondary";
                }

                else if (pfvm.Class.ToLower() == "5".ToLower() || pfvm.Class.ToLower() == "8".ToLower())
                {
                    pfvm.Class = pfvm.Class.ToLower() == "5".ToLower() ? "Primary" : pfvm.Class.ToLower() == "8".ToLower() ? "Middle" : "";
                }


                pfvm.ExamType = ds.Tables[0].Rows[0]["rp"].ToString();
                if (pfvm.ExamType == "R")
                {
                    pfvm.ExamType = "Regular";
                }
                if (pfvm.ExamType == "O")
                {
                    pfvm.ExamType = "Open";
                }
                if (pfvm.ExamType == "P")
                {
                    pfvm.ExamType = "Private";
                }
                pfvm.category = ds.Tables[0].Rows[0]["cat"].ToString();
                if (pfvm.category == "R")
                {
                    pfvm.category = "Reappear/Compartment";
                }
                if (pfvm.category == "D")
                {
                    pfvm.category = "Division improvement";
                }
                if (pfvm.category == "A")
                {
                    pfvm.category = "Additional Subject";
                }
                if (pfvm.category == "SR")
                {
                    pfvm.category = "Golden Chance Re-Appear";
                }
                if (pfvm.category == "SD")
                {
                    pfvm.category = "Golden Chance Improvement";
                }

                pfvm.Name = ds.Tables[0].Rows[0]["name"].ToString();
                pfvm.RegNo = ds.Tables[0].Rows[0]["regno"].ToString();
                pfvm.RefNo = ds.Tables[0].Rows[0]["refno"].ToString();
                pfvm.roll = ds.Tables[0].Rows[0]["roll"].ToString();

                //pfvm.Dist = ds.Tables[0].Rows[0]["Dist"].ToString();
                pfvm.Dist = ds.Tables[0].Rows[0]["homedistco"].ToString();
                pfvm.District = ds.Tables[0].Rows[0]["DISTNM"].ToString();
                //pfvm.District = ds.Tables[0].Rows[0]["Dist"].ToString();
                //pfvm.DistrictFull = ds.Tables[0].Rows[0]["Dist"].ToString();
                //pfvm.SchoolCode = Convert.ToInt32(ds.Tables[0].Rows[0]["schl"].ToString());
                pfvm.SchoolCode = ds.Tables[0].Rows[0]["schl"].ToString();
                pfvm.SchoolName = ds.Tables[0].Rows[0]["SCHLE"].ToString(); // Schollname with station and dist 
                ViewBag.TotalCount = pfvm.PaymentFormData.Tables[0].Rows.Count;

                DataSet dscalFee = ds; //(DataSet)Session["CalculateFee"];
                pfvm.TotalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["fee"].ToString());
                pfvm.TotalLateFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["latefee"].ToString());
                pfvm.TotalCertFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["certfee"].ToString());//hard cert fee
                pfvm.TotalFinalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["totfee"].ToString());

                string rps = NumberToWords(Convert.ToInt32(dscalFee.Tables[1].Rows[0]["totfee"].ToString()));
                //pfvm.TotalFeesInWords = dscalFee.Tables[1].Rows[0]["totfee"].ToString();
                pfvm.TotalFeesInWords = rps;

                //  pfvm.FeeDate = Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy");
                //pfvm.FeeDate =Convert.ToDateTime(dscalFee.Tables[1].Rows[0]["eDate"].ToString());
                pfvm.FeeDate = dscalFee.Tables[1].Rows[0]["banklastdate"].ToString() != "" ? Convert.ToDateTime(dscalFee.Tables[1].Rows[0]["banklastdate"].ToString()) : DateTime.Now;
                //TotalCandidates
                //pfvm.TotalCandidates = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("CountStudents")));
                pfvm.FeeCode = dscalFee.Tables[1].Rows[0]["FEECODE"].ToString();
                pfvm.FeeCategory = dscalFee.Tables[1].Rows[0]["FEECAT"].ToString();
                pfvm.BankLastDate = Convert.ToDateTime(dscalFee.Tables[1].Rows[0]["BankLastdate"].ToString());
                //pfvm.AllowBank = dscalFee.Tables[1].Rows[0]["AllowBanks"].ToString();

                Session["PaymentForm"] = pfvm;

                //string CheckFee = ds.Tables[1].Rows[0]["TotalFeeAmount"].ToString();
                if (pfvm.TotalFinalFees == 0 && pfvm.TotalFees == 0)
                {
                    ViewBag.CheckForm = 1; // only verify for M1 and T1 
                    Session["CheckFormFee"] = 0;
                }
                else
                {
                    ViewBag.CheckForm = 0; // only verify for M1 and T1 
                    Session["CheckFormFee"] = 1;
                }
                return View(pfvm);
            }
        }
        [HttpPost]
        public ActionResult PaymentForm(PrivatePaymentformViewModel pfvm, FormCollection frm, string PayModValue, string AllowBanks)
        {
            PrivateChallanMasterModel CM = new PrivateChallanMasterModel();
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();


            if (Session["Oroll"] == null || Session["Oroll"].ToString() == "")
            {
                return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
            }
            string roll = Session["Oroll"].ToString();

            string bankName = "";
            AllowBanks = pfvm.BankCode;
            if (AllowBanks == "301" || AllowBanks == "302")
            {
                PayModValue = "online";
                if (AllowBanks == "301")
                {
                    bankName = "HDFC Bank";
                }
                else if (AllowBanks == "302")
                {
                    bankName = "Punjab And Sind Bank";
                }
            }
            else if (AllowBanks == "203")
            {
                PayModValue = "hod";
                bankName = "PSEB HOD";
            }
            else if (AllowBanks == "202" || AllowBanks == "204")
            {
                PayModValue = "offline";
                if (AllowBanks == "202")
                {
                    bankName = "Punjab National Bank";
                }
                else if (AllowBanks == "204")
                {
                    bankName = "State Bank of India";
                }
            }
            pfvm.BankName = bankName;
            if (ModelState.IsValid)
            {
                CM.FeeStudentList = "1";
                PrivatePaymentformViewModel PFVMSession = (PrivatePaymentformViewModel)Session["PaymentForm"];
                CM.roll = roll;
                CM.FEE = Convert.ToInt32(PFVMSession.TotalFees);
                CM.latefee = Convert.ToInt32(PFVMSession.TotalLateFees);
                CM.addsubfee = Convert.ToInt32(PFVMSession.TotalCertFees);
                CM.TOTFEE = Convert.ToInt32(PFVMSession.TotalFinalFees);
                CM.FEECAT = PFVMSession.FeeCategory;
                CM.FEECODE = PFVMSession.FeeCode;
                CM.FEEMODE = "CASH";
                CM.BANK = pfvm.BankName;
                CM.BCODE = pfvm.BankCode;
                CM.BANKCHRG = PFVMSession.BankCharges;
                CM.SchoolCode = "";// PFVMSession.SchoolCode.ToString();
                CM.DIST = PFVMSession.Dist.ToString();
                CM.DISTNM = PFVMSession.District;
                //CM.LOT = PFVMSession.LOTNo;
                CM.LOT = 1;

                CM.SCHLREGID = PFVMSession.roll.ToString();
                CM.FeeStudentList = PFVMSession.RefNo.ToString();
                CM.APPNO = PFVMSession.RefNo.ToString();

                CM.category = PFVMSession.category;
                CM.type = "candt";
                CM.CHLNVDATE = DateTime.Now.ToString("dd/MM/yyyy"); //PFVMSession.FeeDate;
                CM.ChallanVDateN = PFVMSession.FeeDate; //PFVMSession.FeeDate;                


                string CandiMobile = "";
                // string result = "0";

                if (pfvm.BankCode == null)
                {
                    ViewBag.Message = "Please Select Bank";
                    ViewData["SelectBank"] = "1";
                    return View(pfvm);
                }

                string result = objDB.InsertPaymentFormPrivate(CM, frm, out CandiMobile);
                if (result == "0")
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
                    Session["ChallanID"] = result;
                    ViewBag.ChallanNo = result;
                    string paymenttype = CM.BCODE;
                    string TotfeePG = (CM.TOTFEE).ToString();

                    if (PayModValue.ToString().ToLower().Trim() == "online" && result.ToString().Length > 10)
                    {
                        #region Payment Gateyway

                        if (paymenttype.ToUpper() == "301" && ViewBag.ChallanNo != "") /*HDFC*/
                        {
                            string AccessCode = ConfigurationManager.AppSettings["CcAvenueAccessCode"];
                            string CheckoutUrl = ConfigurationManager.AppSettings["CcAvenueCheckoutUrl"];
                            string WorkingKey = ConfigurationManager.AppSettings["CcAvenueWorkingKey"];
                            //******************
                            string invoiceNumber = ViewBag.ChallanNo;
                            string amount = TotfeePG;
                            //***************
                            var queryParameter = new CCACrypto();

                            string strURL = GatewayController.BuildCcAvenueRequestParameters(invoiceNumber, amount);

                            return View("../Gateway/CcAvenue", new CcAvenueViewModel(queryParameter.Encrypt
                                       (strURL, WorkingKey), AccessCode, CheckoutUrl));

                        }
                        else if (paymenttype.ToUpper() == "302" && ViewBag.ChallanNo != "")/*ATOM*/
                        {

                            string TransactionID = encrypt.QueryStringModule.Encrypt(ViewBag.ChallanNo);
                            string TransactionAmount = encrypt.QueryStringModule.Encrypt(TotfeePG);
                            string clientCode = CM.APPNO;
                            // User Details
                            string udf1CustName = encrypt.QueryStringModule.Encrypt(CM.SCHLREGID); // roll number
                            string udf2CustEmail = CM.FEECAT; /// Kindly submit Appno/Refno in client id, Fee cat in Emailid (ATOM)
                            string udf3CustMob = encrypt.QueryStringModule.Encrypt(CandiMobile);

                            //AtomCheckoutUrl(string ChallanNo, string amt, string clientCode, string cmn, string cme, string cmno)
                            return RedirectToAction("AtomCheckoutUrl", "Gateway", new { ChallanNo = TransactionID, amt = TransactionAmount, clientCode = clientCode, cmn = udf1CustName, cme = udf2CustEmail, cmno = udf3CustMob });

                        }
                        #endregion Payment Gateyway
                    }
                    else
                    {
                        CM.CHLNVDATE = (Convert.ToString(PFVMSession.FeeDate)).Substring(0, 10);
                        //  string Sms = "Your Challan no. " + result + " generated  for Catg " + CM.category + " and Ref No. " + CM.FeeStudentList + " valid till Dt " + CM.CHLNVDATE + ". Regards PSEB";
                        try
                        {
                            //    string getSms = objCommon.gosms(CandiMobile, Sms);
                            // string getSms = objCommon.gosms("9711819184", Sms);
                        }
                        catch (Exception) { }

                        ModelState.Clear();
                        //--For Showing Message---------//                   
                        return RedirectToAction("GenerateChallaan", "PrivateCandidate", new { ChallanId = result });
                    }
                }
            }
            return View(pfvm);
        }
        public ActionResult GenerateChallaan()
        {
            Session["Session"] = "2023-2024";
            Session["RoleType"] = "Private";
            string ChallanId = Session["ChallanID"].ToString();
            if (ChallanId == null || ChallanId == "0")
            {
                return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
            }

            if (Session["Oroll"] == null || Session["Oroll"].ToString() == "")
            {
                return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
            }
            //PrivateChallanMasterModel CM = new PrivateChallanMasterModel();
            ChallanMasterModel CM = new ChallanMasterModel();
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();

            //-----------Update By Deepak
            string schl = "";
            if (Convert.ToString(Session["SCHL"]) != "")
            {
                schl = Session["SCHL"].ToString();
            }
            //-------End----------
            //string schl = Session["SCHL"].ToString();
            string ChallanId1 = ChallanId.ToString();

            DataSet ds = objDB.GetChallanDetailsById(ChallanId1);
            CM.ChallanMasterData = ds;
            if (CM.ChallanMasterData == null || CM.ChallanMasterData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                return View();
            }
            else
            {
                CM.CHALLANID = ds.Tables[0].Rows[0]["CHALLANID"].ToString();
                CM.CHLNDATE = ds.Tables[0].Rows[0]["ChallanGDateN1"].ToString();
                CM.CHLNVDATE = ds.Tables[0].Rows[0]["ChallanVDateN1"].ToString();
                // CM.CHLNDATE = ds.Tables[0].Rows[0]["CHLNDATE"].ToString();
                // CM.CHLNVDATE = ds.Tables[0].Rows[0]["CHLNVDATE"].ToString();

                CM.FEE = float.Parse(ds.Tables[0].Rows[0]["FEE"].ToString());
                CM.latefee = int.Parse(ds.Tables[0].Rows[0]["latefee"].ToString());
                CM.TOTFEE = float.Parse(ds.Tables[0].Rows[0]["PaidFees"].ToString());
                CM.FEECAT = ds.Tables[0].Rows[0]["FEECAT"].ToString();
                CM.FEECODE = ds.Tables[0].Rows[0]["FEECODE"].ToString();
                CM.FEEMODE = ds.Tables[0].Rows[0]["FEEMODE"].ToString();
                CM.BANK = ds.Tables[0].Rows[0]["BANK"].ToString();
                ViewBag.BCODE = CM.BCODE = ds.Tables[0].Rows[0]["BCODE"].ToString();
                CM.BANKCHRG = float.Parse(ds.Tables[0].Rows[0]["BANKCHRG"].ToString());
                //CM.SchoolCode = ds.Tables[0].Rows[0]["SchoolCode"].ToString();
                CM.SchoolCode = ds.Tables[0].Rows[0]["SCHLREGID"].ToString();
                CM.APPNO = ds.Tables[0].Rows[0]["APPNO"].ToString();
                CM.SCHLREGID = ds.Tables[0].Rows[0]["SCHLREGID"].ToString();
                // CM.SchoolCode = Session["SCHL"].ToString();
                CM.DIST = ds.Tables[0].Rows[0]["DIST"].ToString();
                CM.DISTNM = ds.Tables[0].Rows[0]["DISTNM"].ToString();
                CM.LOT = Convert.ToInt32(ds.Tables[0].Rows[0]["LOT"].ToString());
                CM.TotalFeesInWords = ds.Tables[0].Rows[0]["TotalFeesInWords"].ToString();
                //CM.SchoolName = ds.Tables[0].Rows[0]["SchoolName"].ToString();
                CM.SCHLCANDNM = ds.Tables[0].Rows[0]["candidateNM"].ToString();
                CM.DepositoryMobile = ds.Tables[0].Rows[0]["DepositoryMobile"].ToString();
                CM.type = ds.Tables[0].Rows[0]["type"].ToString();
                if (ds.Tables[0].Rows[0]["Verified"].ToString() == "1")
                {
                    CM.BRCODE = ds.Tables[0].Rows[0]["BRCODE"].ToString();
                    CM.BRANCH = ds.Tables[0].Rows[0]["BRANCH"].ToString();
                    CM.J_REF_NO = ds.Tables[0].Rows[0]["J_REF_NO"].ToString();
                    CM.DEPOSITDT = ds.Tables[0].Rows[0]["DEPOSITDT"].ToString();
                }

                Session["CalculateFee"] = null;
                Session["PaymentForm"] = null;
                Session["FeeStudentList"] = null;
                return View(CM);
            }
        }
        public JsonResult ReGenerateChallaan(string ChallanId, string BCODE)
        {
            string OutStatus = "";
            string dee = "0";
            try
            {
                if (ChallanId == null)
                {
                    dee = "-1";//return RedirectToAction("PaymentForm", "Home");
                }
                PrivateChallanMasterModel CM = new PrivateChallanMasterModel();
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                if (Session["refno"] == null || Session["refno"].ToString() == "")
                {
                    dee = "-1"; //return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
                }
                string schl = Session["refno"].ToString();
                string ChallanId1 = ChallanId.ToString();
                string Usertype = "PvtUser";


                OutStatus = objDB.ReGenerateChallaanById(ChallanId1, Usertype, BCODE);
                if (OutStatus.ToString().Length > 2)
                {
                    Session["ChallanID"] = OutStatus;
                    Session["resultReGenerate"] = "1";
                    dee = "1";


                }
                else
                {
                    Session["resultReGenerate"] = "0";
                    ViewBag.Message = "Re Generate Challaan Failure";
                    //return RedirectToAction("FinalPrint", "Home");
                    dee = "0";
                }
            }
            catch (Exception ex)
            {
                dee = "-3";
                //return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
            }
            return Json(new { dee = dee, chid = OutStatus }, JsonRequestBehavior.AllowGet);
        }

        public string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

        #region Begin final Submit
        public ActionResult FinalSubmit(FormCollection frc)
        {
            PrivateCandidateModels MS = new PrivateCandidateModels();
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            try
            {
                if (Session["OROLL"] != null && Session["refNo"] != "")
                {
                    MS.OROLL = Session["OROLL"].ToString();
                    MS.refNo = Session["refNo"].ToString();
                    // check photo and sign exists or not
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    string photo = MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
                    string sign = MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();
                    string PhotoExist = "https://psebdata.s3.ap-south-1.amazonaws.com/allfiles/" + "Upload2023/" + MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
                    string SignExist = "https://psebdata.s3.ap-south-1.amazonaws.com/allfiles/" + "Upload2023/" + MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();
                    ViewBag.PhotoExist = System.IO.File.Exists(PhotoExist) ? "1" : "0";
                    ViewBag.signExist = System.IO.File.Exists(SignExist) ? "1" : "0";
                    MS.StoreAllData = null;

                    if (photo != "" && sign != "")
                    {
                        MS.StoreAllData = objDB.GetPrivateCandidateConfirmationFinalSubmit(MS.refNo);
                        //MS.StoreAllData = objDB.GetPrivateCandidateConfirmationPrint(MS.refNo);
                        if (MS.StoreAllData != null)
                        {
                            if (MS.StoreAllData.Tables[0].Rows[0]["result"].ToString() == "1")
                            {
                                ViewData["roll"] = Session["OROLL"].ToString();
                                ViewData["refno"] = Session["refNo"].ToString();
                                ViewData["Status"] = "1";
                            }
                        }
                    }

                    //if (ViewBag.PhotoExist == "1" && ViewBag.signExist == "1")
                    //{
                    //    MS.StoreAllData = objDB.GetPrivateCandidateConfirmationFinalSubmit(MS.refNo);
                    //    //MS.StoreAllData = objDB.GetPrivateCandidateConfirmationPrint(MS.refNo);
                    //    if (MS.StoreAllData != null)
                    //    {
                    //        if (MS.StoreAllData.Tables[0].Rows[0]["result"].ToString() == "1")
                    //        {
                    //            ViewData["roll"] = Session["OROLL"].ToString();
                    //            ViewData["refno"] = Session["refNo"].ToString();
                    //            ViewData["Status"] = "1";
                    //        }
                    //    }
                    //}
                    else
                    {
                        TempData["Status"] = "11";
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("PrivateCandidateConfirmation", "PrivateCandidate");
        }



        #endregion end final Submit

        #region Private Admit Card (Compartment)
        public ActionResult AdmitCardPrivateSearchStatus()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AdmitCardPrivateSearchStatus(FormCollection frc, string cmd, string id)
        {
            try
            {
                string CLASS1 = "0";
                ViewBag.cid = id;
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                PrivateCandidateModels rm = new PrivateCandidateModels();

                string Search = "a.refno is not null ";
                if (cmd == "Search")
                {
                    rm.SelList = frc["SelList"].ToString();
                    rm.batchYear = frc["batchYear"].ToString();
                    rm.SearchBy = frc["SearchBy"].ToString();

                    // Search += "batch='" + rm.batchYear.Substring(0,1)+ "' and batchYear = '" + rm.batchYear.Substring(1,4)+"'";

                    if (rm.SelList == "1") { Search += " and a.refno ='" + rm.SearchBy + "'"; }
                    else if (rm.SelList == "2") { Search += " and a.roll ='" + rm.SearchBy + "'"; }
                    else if (rm.SelList == "3") { Search += " and a.examroll ='" + rm.SearchBy + "'"; }
                    else if (rm.SelList == "4") { Search += " and a.Mobile ='" + rm.SearchBy + "'"; }
                    else if (rm.SelList == "5") { Search += " and a.challanid ='" + rm.SearchBy + "'"; }

                    rm.StoreAllData = objDB.PrivateCandidateStatusCheck(Search);
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message2 = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                    }

                }
                return View(rm);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult AdmitCardPrivateSearch(string id)
        {
            if (id == null || id == "")
            {
                ViewData["result"] = "0";
                return View();
            }
            else
            {
                ViewData["result"] = "1";
                string CLASS1 = "";
                if (id.ToUpper() == "SENIOR") // For Senior
                {
                    CLASS1 = "12";
                    ViewBag.ClassName = "Senior Secondary";
                }
                else if (id.ToUpper() == "MATRIC") // For MAtric
                {
                    CLASS1 = "10";
                    ViewBag.ClassName = "Matric";
                }
                ViewBag.cid = id;
                return View();
            }
        }
        [HttpPost]
        public ActionResult AdmitCardPrivateSearch(FormCollection frc, string cmd, string id)
        {
            if (id == null || id == "")
            {
                ViewData["result"] = "0";
                return View();

            }
            else
            { ViewData["result"] = "1"; }

            try
            {
                string CLASS1 = "";
                if (id.ToUpper() == "SENIOR") // For Senior
                {
                    CLASS1 = "12";
                    ViewBag.ClassName = "Senior Secondary";
                }
                else if (id.ToUpper() == "MATRIC") // For MAtric
                {
                    CLASS1 = "10";
                    ViewBag.ClassName = "Matric";
                }
                else if (id.ToUpper() == "MIDDLE") // For MIDDLE
                {
                    CLASS1 = "8";
                    ViewBag.ClassName = "Middle";
                }
                else if (id.ToUpper() == "PRIMARY") // For Primary
                {
                    CLASS1 = "5";
                    ViewBag.ClassName = "Primary";
                }
                ViewBag.cid = id;
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                PrivateCandidateModels rm = new PrivateCandidateModels();

                string Search = string.Empty;
                if (cmd == "Search")
                {
                    Search += " a.refno is not null";
                    //if (frc["category"] != "" && frc["category"] != "0")
                    //{
                    //    Search += " and a.cat ='" + frc["category"].ToString() + "'";
                    //}                    
                    if (frc["refNo"] != "")
                    {
                        Search += " and a.refno ='" + frc["refNo"].ToString() + "'";
                    }
                    if (frc["Candi_Name"].Trim() != "")
                    {
                        Search += " and a.name like '" + frc["Candi_Name"].ToString() + "%'";
                    }
                    if (frc["Father_Name"].Trim() != "")
                    {
                        Search += " and a.fname like '" + frc["Father_Name"].ToString() + "%'";
                    }
                    if (frc["OROLL"].Trim() != "")
                    {
                        Search += " and a.roll ='" + frc["OROLL"].ToString() + "'";
                    }
                    if (frc["EXAMROLL"].Trim() != "")
                    {
                        Search += " and a.EXAMROLL ='" + frc["EXAMROLL"].ToString() + "'";
                    }
                    // rm.category = frc["category"].ToString();                  
                    rm.OROLL = frc["OROLL"].ToString();
                    rm.refNo = frc["refNo"].ToString();
                    rm.Candi_Name = frc["Candi_Name"].ToString();
                    rm.Father_Name = frc["Father_Name"].ToString();
                    //rm.QRCode = QRCoder(rm.refNo.ToString());

                    rm.StoreAllData = objDB.AdmitCardPrivateSearch(Search, Convert.ToInt32(CLASS1));
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message2 = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                    }

                }


                if (ModelState.IsValid)
                { return View(rm); }
                else
                { return View(); }
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult AdmitCardPrivate(string id, string RefNo)
        {
            if (id == null || id == "")
            { return RedirectToAction("AdmitCardPrivateSearch", "PrivateCandidate"); }

            string CLASS1 = "";
            if (id.ToUpper() == "SENIOR") // For Senior
            {
                CLASS1 = "12";
                ViewBag.ClassName = "Senior Secondary";
            }
            else if (id.ToUpper() == "MATRIC") // For MAtric
            {
                CLASS1 = "10";
                ViewBag.ClassName = "Matric";
            }
            else if (id.ToUpper() == "MIDDLE") // For MIDDLE
            {
                CLASS1 = "8";
                ViewBag.ClassName = "Middle";
            }
            else if (id.ToUpper() == "PRIMARY") // For PRIMARY
            {
                CLASS1 = "5";
                ViewBag.ClassName = "Primary";
            }
            ViewBag.cid = id;
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            PrivateCandidateModels rm = new PrivateCandidateModels();
            if (RefNo != "" && RefNo != null && id != "" && id != null)
            {
                RefNo = encrypt.QueryStringModule.Decrypt(RefNo);
                rm.refNo = RefNo;
                string search = "";
                search += " pc.refno='" + rm.refNo + "'";
                //rm.StoreAllData = objDB.GetCompartmentPrivateAdmitCardByRefNo(rm);
                rm.StoreAllData = new AbstractLayer.SchoolDB().AdmitCardPrivate(search, "", Convert.ToInt32(CLASS1));
                if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message2 = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.ClassName = rm.StoreAllData.Tables[0].Rows[0]["ClassName"].ToString();
                }
            }

            return View(rm);
        }

        #endregion Private Admit Card (Compartment)

        #region Photo and Image Upload and Email 
        public ActionResult imgUpdPvt(string id, string cmd)
        {
            try
            {
                FormCollection frm = new FormCollection();
                PrivateCandidateModels MS = new PrivateCandidateModels();
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                if (id == null)
                {
                    //return View();
                    return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
                }
                else
                {

                    TempData["id"] = id;
                    return View();
                }

            }
            catch (Exception ex)
            {
                return View();
            }


        }
        [HttpPost]
        public ActionResult imgUpdPvt(PrivateCandidateModels MS, FormCollection frm, string id)
        {
            try
            {
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                if (TempData["id"] == null)
                {
                    return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
                }
                else
                if (TempData["id"] != null)
                {
                    {
                        id = TempData["id"].ToString();
                        id = encrypt.QueryStringModule.Decrypt(id);
                        MS.refNo = id;
                        //-----------photo upload
                        if (MS.std_Photo != null)
                        {
                            string Orgfile = MS.refNo + "P" + ".jpg";
                            MS.PathPhoto = "PvtPhoto/Photo/" + MS.refNo + "P" + ".jpg";
                            using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                            {
                                using (var newMemoryStream = new MemoryStream())
                                {
                                    ///file.CopyTo(newMemoryStream);

                                    var uploadRequest = new TransferUtilityUploadRequest
                                    {
                                        InputStream = MS.std_Photo.InputStream,
                                        Key = string.Format("allfiles/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo/{0}", Orgfile),
                                        BucketName = BUCKET_NAME,
                                        CannedACL = S3CannedACL.PublicRead
                                    };

                                    var fileTransferUtility = new TransferUtility(client);
                                    fileTransferUtility.Upload(uploadRequest);
                                }
                            }
                        }
                        else
                        {
                            //Response.Write("<script>alert('Please Upload Photo')</script>");
                            //return View();
                        }
                        if (MS.std_Sign != null)
                        {
                            string Orgfile = MS.refNo + "S" + ".jpg";
                            MS.PathSign = "PvtPhoto/Sign/" + MS.refNo + "S" + ".jpg";

                            using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                            {
                                using (var newMemoryStream = new MemoryStream())
                                {
                                    ///file.CopyTo(newMemoryStream);

                                    var uploadRequest = new TransferUtilityUploadRequest
                                    {
                                        InputStream = MS.std_Photo.InputStream,
                                        Key = string.Format("allfiles/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign/{0}", Orgfile),
                                        BucketName = BUCKET_NAME,
                                        CannedACL = S3CannedACL.PublicRead
                                    };

                                    var fileTransferUtility = new TransferUtility(client);
                                    fileTransferUtility.Upload(uploadRequest);
                                }
                            }
                        }
                        DataSet result2 = objDB.InsertimgUpdPvt(MS);
                        if (result2 != null)
                        {
                            ViewData["Status"] = result2.Tables[0].Rows[0]["RESULT"].ToString();
                        }

                        //MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                        //-------------
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult imgUpdPvtemail(PrivateCandidateModels MS)
        {
            FormCollection frm = new FormCollection();
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            string search = "";
            MS.StoreAllData = objDB.imgUpdPvtemail(search);
            ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
            return View(MS);
        }
        [HttpPost]
        public ActionResult imgUpdPvtemail(PrivateCandidateModels MS, FormCollection frm)
        {
            try
            {
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                MS.batchYear = frm["batchYear"].ToString();
                MS.SearchBy = frm["SearchBy"].ToString();
                MS.SearchString = frm["SearchString"].ToString();

                if (ModelState.IsValid)
                {
                    string Search = string.Empty;
                    if (MS.batchYear != "")
                    {
                        Search = "a.batch='" + MS.batchYear.Substring(0, 1) + "' and a.batchYear='" + MS.batchYear.Substring(2, 4) + "'";
                    }
                    if (MS.SearchBy != "0")
                    {
                        string sString = MS.SearchString.Trim();
                        switch (MS.SearchBy)
                        {
                            case "1": Search += " and a.Roll        ='" + sString + "'"; break;
                            case "2": Search += " and a.refno       ='" + sString + "'"; break;
                            case "3": Search += " and a.name       like '%" + sString + "%'"; break;
                            case "4": Search += " and a.fname      like '%" + sString + "%'"; break;
                        }
                    }

                    MS.StoreAllData = objDB.imgUpdPvtemail(Search);
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Enter Correct Refno/Roll no";
                        ViewBag.TotalCount = 0;
                    }
                }
                if (ModelState.IsValid)
                { return View(MS); }
                else
                { return View(); }
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult Pvtemail(string id, string email, string name)
        {
            PrivateCandidateModels MS = new PrivateCandidateModels();
            AbstractLayer.DBClass dbclass = new AbstractLayer.DBClass();
            FormCollection frm = new FormCollection();
            MS.emailID = email;
            ViewData["name"] = name;

            if (MS.emailID != null || MS.emailID != "")
            {
                string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "> <tr><td><b>Dear " + ViewData["name"] + "</b>,</td></tr><tr><td height=" + 30 + ">As per your request Dated <b>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</b> Regarding Private Form Photo and Sign</td></tr><tr><td><b>You are alredy registred for Matric/Senior Secondary Supplementary Exam (Reap/Additional):-</b><br /></td></tr><tr><td height=" + 30 + "><b>Click Here To Upload Photo and Sign </b> <a href=https://www.registration.pseb.ac.in/PrivateCandidate/imgUpdPvt?id=" + id + " target = _blank>www.registration.pseb.ac.in</a></td></tr><tr><td><b>Note:</b> Please Read Instruction Carefully Before uploading files, Online Form .</td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /><tr><td><b>Contact Us</b><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 18002700280<br>DISTRICTS:- BARNALA, FATEHGARH SAHIB, GURDASPUR, HOSHIARPUR, JALANDHAR, KAPURTHALA, SHRI MUKTSAR SAHIB, S.B.S. NAGAR, PATHANKOT, PATIALA, SANGRUR, CHANDIGARH &amp; OTHER STATES<br><br><b>Email Id:</b> <a href=mailto:Contact2@psebonline.in target=_blank>contact2@psebonline.in</a><br><b>Toll Free Help Line No. :</b> 18004190690<br>DISTRICTS:- AMRITSAR, BATHINDA, FARIDKOT, FAZILKA, FEROZEPUR, LUDHIANA, MANSA, MOGA, ROOP NAGAR, S.A.S NAGAR,TARN TARAN<br></td></tr>";
                string subject = "PSEB-Private Form Notification";
                bool result = dbclass.mail(subject, body, MS.emailID);
                ViewData["result"] = result;
            }

            return RedirectToAction("imgUpdPvtemail", "PrivateCandidate");

            //  return View(id);
        }

        public ActionResult imgUpd(string id, string cmd)
        {
            try
            {
                //FormCollection frm = new FormCollection();
                //PrivateCandidateModels MS = new PrivateCandidateModels();
                //AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                //if (id == null)
                //{
                //    //return View();
                //    return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
                //}
                //else
                //{

                //    TempData["id"] = id;
                //    return View();
                //}
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }


        }
        [HttpPost]
        public ActionResult imgUpd(PrivateCandidateModels MS, FormCollection frm)
        {
            try
            {
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                //if (TempData["id"] == null)
                //{
                //    return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
                //}
                //else


                if (Session["refno"] != null)
                {
                    {
                        //id = TempData["id"].ToString();
                        //id = encrypt.QueryStringModule.Decrypt(id);
                        MS.refNo = Session["refno"].ToString();
                        //-----------photo upload
                        if (MS.std_Photo != null)
                        {
                            stdPic = Path.GetFileName(MS.std_Photo.FileName);
                            string Orgfile = MS.refNo + "P" + ".jpg";
                            string Filename = "allfiles/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo/";
                            MS.PathPhoto = "PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo/" + Orgfile;
                            string pathPhoto = MS.refNo + "P" + ".jpg";
                            using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                            {
                                using (var newMemoryStream = new MemoryStream())
                                {
                                    ///file.CopyTo(newMemoryStream);

                                    var uploadRequest = new TransferUtilityUploadRequest
                                    {
                                        InputStream = MS.std_Photo.InputStream,
                                        Key = string.Format("allfiles/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo/{0}", Orgfile),
                                        BucketName = BUCKET_NAME,
                                        CannedACL = S3CannedACL.PublicRead
                                    };

                                    var fileTransferUtility = new TransferUtility(client);
                                    fileTransferUtility.Upload(uploadRequest);
                                }
                            }

                            //MS.std_Photo.SaveAs(pathPhoto);
                            ViewBag.Photo = MS.PathPhoto != "" ? MS.PathPhoto.Replace("UPLOAD2023/", "").Replace("OPEN2022", "Open2022").Replace("PHOTO", "Photo").Replace("JPG", "jpg") : "";
                            ViewBag.ImageURL = MS.PathPhoto != "" ? MS.PathPhoto.ToString().Replace("UPLOAD2023/", "").Replace("OPEN2022", "Open2022").Replace("SIGN", "Sign").Replace("JPG", "jpg") : ""; ;

                            //ViewBag.Photo = MS.PathPhoto != "" ? MS.PathPhoto.Replace("UPLOAD2023", "Upload2023").Replace("OPEN2022", "Open2022").Replace("PHOTO", "Photo").Replace("JPG", "") : "";
                            //ViewBag.ImageURL = MS.PathPhoto;
                            Session["imgPhoto"] = MS.PathPhoto;
                        }
                        else
                        {
                            //Response.Write("<script>alert('Please Upload Photo')</script>");
                            //return View();
                        }
                        if (MS.std_Sign != null)
                        {
                            stdSign = Path.GetFileName(MS.std_Sign.FileName);
                            string Orgfile = MS.refNo + "S" + ".jpg";
                            string Filename = "allfiles/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign/";
                            MS.PathSign = "PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign/" + Orgfile;
                            string pathSign = MS.refNo + "S" + ".jpg";
                            using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                            {
                                using (var newMemoryStream = new MemoryStream())
                                {
                                    ///file.CopyTo(newMemoryStream);

                                    var uploadRequest = new TransferUtilityUploadRequest
                                    {
                                        InputStream = MS.std_Sign.InputStream,
                                        Key = string.Format("allfiles/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign/{0}", Orgfile),
                                        BucketName = BUCKET_NAME,
                                        CannedACL = S3CannedACL.PublicRead
                                    };

                                    var fileTransferUtility = new TransferUtility(client);
                                    fileTransferUtility.Upload(uploadRequest);
                                }
                            }


                            ViewBag.sign = MS.PathSign;
                            Session["imgSign"] = MS.PathSign;
                        }
                        DataSet result2 = objDB.InsertimgUpdPvt(MS);
                        if (result2 != null)
                        {
                            ViewData["Status"] = result2.Tables[0].Rows[0]["RESULT"].ToString();
                        }

                        //MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                        //-------------
                    }
                }
                return View();
                //return RedirectToAction("PrivateCandidateConfirmation", "PrivateCandidate");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
            }
        }
        #endregion Photo and Image Upload and Email 

        public ActionResult AdminPrivateSignChart()
        {
            return View();
        }
        public ActionResult AdminPrivateConfidentialList()
        {
            return View();
        }
        #region Private Final Print using Admin Login
        public ActionResult AdminFPPrivateSearch(string id)
        {
            if (Session["AdminType"] == null)
            {
                return RedirectToAction("Logout", "Admin");
            }

            #region Action Assign Method
            if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
            { ViewBag.IsModify = 1; ViewBag.IsView = 1; }
            else
            {

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                //string AdminType = Session["AdminType"].ToString();
                //GetActionOfSubMenu(string cont, string act)
                DataSet aAct = objCommon.GetActionOfSubMenu(Convert.ToInt32(Session["AdminId"]), controllerName, actionName);
                if (aAct.Tables[0].Rows.Count > 0)
                {
                    ViewBag.IsModify = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Contains("EDIT")).Count();
                    ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Contains("VIEW")).Count();
                }
            }

            #endregion Action Assign Method
            return View();

        }
        [HttpPost]
        public ActionResult AdminFPPrivateSearch(FormCollection frc, string cmd)
        {
            try
            {
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                PrivateCandidateModels rm = new PrivateCandidateModels();

                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsModify = 1; ViewBag.IsView = 1; }
                else
                {

                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    //string AdminType = Session["AdminType"].ToString();
                    //GetActionOfSubMenu(string cont, string act)
                    DataSet aAct = objCommon.GetActionOfSubMenu(Convert.ToInt32(Session["AdminId"]), controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsModify = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Contains("EDIT")).Count();
                        ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Contains("VIEW")).Count();
                    }
                }

                #endregion Action Assign Method

                string Search = string.Empty;
                if (cmd == "Search")
                {
                    Search += " a.refno is not null";
                    if (frc["refNo"] != "")
                    {
                        Search += " and a.refno ='" + frc["refNo"].ToString() + "'";
                    }
                    if (frc["Candi_Name"].Trim() != "")
                    {
                        Search += " and a.name like '" + frc["Candi_Name"].ToString() + "%'";
                    }
                    if (frc["Father_Name"].Trim() != "")
                    {
                        Search += " and a.fname like '" + frc["Father_Name"].ToString() + "%'";
                    }
                    if (frc["OROLL"].Trim() != "")
                    {
                        Search += " and a.roll ='" + frc["OROLL"].ToString() + "'";
                    }
                    if (frc["NROLL"].Trim() != "")
                    {
                        Search += " and a.ExamRoll ='" + frc["NROLL"].ToString() + "'";
                    }
                    rm.NROLL = frc["NROLL"].ToString();
                    rm.OROLL = frc["OROLL"].ToString();
                    rm.refNo = frc["refNo"].ToString();
                    rm.Candi_Name = frc["Candi_Name"].ToString();
                    rm.Father_Name = frc["Father_Name"].ToString();


                    rm.StoreAllData = objDB.AdminFPPrivateSearch(Search);
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message2 = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                    }
                }
                if (ModelState.IsValid)
                { return View(rm); }
                else
                { return View(); }
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult AdminFPPrivate(string RefNo)
        {
            try
            {

                if (Session["AdminType"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                PrivateCandidateModels MS = new PrivateCandidateModels();
                if (RefNo != "" && RefNo != null)
                {
                    RefNo = encrypt.QueryStringModule.Decrypt(RefNo);
                    MS.refNo = RefNo;
                    Session["refno"] = RefNo;
                    MS.capcha = RefNo;
                    string search = "";
                    search += " pc.refno='" + MS.refNo + "'";
                    MS.StoreAllData = objDB.AdminFPPrivate(MS);
                    //MS.StoreAllData = new AbstractLayer.SchoolDB().AdmitCardPrivate(search, "", Convert.ToInt32(CLASS1));
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message2 = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        DataSet result = objDB.SelectDist(); // passing Value to DBClass from model
                        ViewBag.MyDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> items = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                        {
                            items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                        }
                        // ViewBag.MyDist = items;
                        ViewBag.MyDist = new SelectList(items, "Value", "Text");

                        if (MS.SelDist == "")
                        {
                            MS.SelDist = "0";
                        }

                        ViewBag.MyExamDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> items1 = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyExamDist.Rows)
                        {
                            items1.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                        }
                        // ViewBag.MyDist = items;
                        ViewBag.MyExamDist = new SelectList(items1, "Value", "Text");

                        if (MS.SelExamDist == "")
                        {
                            MS.SelExamDist = "0";
                        }



                        MS.SelExamDist = MS.StoreAllData.Tables[0].Rows[0]["dist"].ToString();

                        int exmdist = Convert.ToInt32(MS.SelExamDist);
                        DataSet resultEC = objDB.SelectAllTehsilEC(exmdist);
                        ViewBag.MyTehsilEC = resultEC.Tables[0];
                        ViewBag.MyTehsilEC2 = resultEC.Tables[0];
                        List<SelectListItem> TehListEC = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyTehsilEC.Rows)
                        {

                            TehListEC.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                        }

                        ViewBag.MyTehsilEC = TehListEC;
                        ViewBag.MyTehsilEC2 = TehListEC;

                        MS.tehsilEC = MS.StoreAllData.Tables[0].Rows[0]["Cent_1"].ToString();
                        MS.tehsilEC2 = MS.StoreAllData.Tables[0].Rows[0]["Cent_2"].ToString();

                        //---------------------- New ENtry--------------------------------------------------------
                        //
                        if (MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"] == null || MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"].ToString() == "")
                        {
                            MS.DisabilityPercent = 0;
                        }
                        else { MS.DisabilityPercent = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"].ToString()); }
                        MS.IsHardCopyCertificate = MS.StoreAllData.Tables[0].Rows[0]["IsHardCopyCertificate"].ToString();
                        MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["rp"].ToString();
                        //MS.category = MS.StoreAllData.Tables[1].Rows[0]["category"].ToString();
                        if (MS.Exam_Type == "R")
                        {
                            MS.Exam_Type = "Regular";
                        }
                        if (MS.Exam_Type == "O")
                        {
                            MS.Exam_Type = "Open";
                        }
                        if (MS.Exam_Type == "P")
                        {
                            MS.Exam_Type = "Private";
                        }
                        //MS.category = MS.StoreAllData.Tables[1].Rows[0]["category"].ToString();
                        MS.category = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                        if (MS.category == "R")
                        {
                            MS.category = "Reappear/Compartment";
                        }
                        if (MS.category == "D")
                        {
                            MS.category = "Division improvement";
                        }
                        if (MS.category == "A")
                        {
                            MS.category = "Additional Subject";
                        }
                        if (MS.category == "SR")
                        {
                            MS.category = "Golden Chance Re-Appear";
                        }
                        if (MS.category == "SD")
                        {
                            MS.category = "Golden Chance Improvement";
                        }
                        if (MS.category == "SA")
                        {
                            MS.category = "Golden Chance Additional";
                        }
                        Session["form"] = MS.StoreAllData.Tables[0].Rows[0]["feecat"].ToString();


                        MS.Class = MS.StoreAllData.Tables[0].Rows[0]["class"].ToString();
                        if (MS.Class == "10")
                        {
                            MS.Class = MS.StoreAllData.Tables[0].Rows[0]["classNM"].ToString();
                            MS.MatricSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            MS.MatricSub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString();
                        }
                        else if (MS.Class == "12")
                        {
                            MS.TwelveSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            MS.Class = MS.StoreAllData.Tables[0].Rows[0]["classNM"].ToString();
                        }
                        else if (MS.Class == "5" || MS.Class == "8")
                        {
                            MS.TwelveSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            MS.Class = MS.StoreAllData.Tables[0].Rows[0]["classNM"].ToString();
                        }



                        MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                        MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
                        MS.Session = MS.SelMonth + ' ' + MS.SelYear;

                        MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                        Session["OROLL"] = MS.OROLL;
                        Session["category"] = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();

                        MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();
                        MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();

                        MS.refNo = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                        MS.SCHL = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();

                        MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                        MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                        MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();

                        MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();
                        MS.Result = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();

                        MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
                        MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
                        MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();

                        MS.EPname = MS.Candi_Name + '/' + MS.Pname;
                        MS.EPFname = MS.Father_Name + '/' + MS.PFname;
                        MS.EPMname = MS.Mother_Name + '/' + MS.PMname;

                        MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["sex"].ToString();
                        MS.CastList = MS.StoreAllData.Tables[0].Rows[0]["caste"].ToString();
                        MS.Area = MS.StoreAllData.Tables[0].Rows[0]["area"].ToString();
                        if (MS.Area == "U")
                        {
                            MS.Area = "Urban";
                        }
                        if (MS.Area == "R")
                        {
                            MS.Area = "Rural";
                        }
                        MS.Relist = MS.StoreAllData.Tables[0].Rows[0]["religion"].ToString();
                        if (MS.Relist == "H")
                        {
                            MS.Relist = "Hindu";
                        }
                        if (MS.Relist == "M")
                        {
                            MS.Relist = "Muslim";
                        }
                        if (MS.Relist == "S")
                        {
                            MS.Relist = "Sikh";
                        }
                        if (MS.Relist == "C")
                        {
                            MS.Relist = "Christian";
                        }
                        if (MS.Relist == "O")
                        {
                            MS.Relist = "Others";
                        }

                        MS.phyChal = MS.StoreAllData.Tables[0].Rows[0]["phy_chal"].ToString();
                        MS.rdoWantWriter = MS.StoreAllData.Tables[0].Rows[0]["writer"].ToString();
                        if (MS.rdoWantWriter == "True")
                        {
                            MS.rdoWantWriter = "1";
                        }
                        if (MS.rdoWantWriter == "False")
                        {
                            MS.rdoWantWriter = "0";
                        }

                        MS.IsPracExam = MS.StoreAllData.Tables[0].Rows[0]["prac"].ToString();
                        if (MS.IsPracExam == "True")
                        {
                            MS.IsPracExam = "1";
                        }
                        if (MS.IsPracExam == "False")
                        {
                            MS.IsPracExam = "0";
                        }
                        MS.address = MS.StoreAllData.Tables[0].Rows[0]["homeadd"].ToString();

                        MS.landmark = MS.StoreAllData.Tables[0].Rows[0]["LandMark"].ToString();
                        MS.block = MS.StoreAllData.Tables[0].Rows[0]["block"].ToString();
                        MS.SelExamDist = MS.StoreAllData.Tables[0].Rows[0]["dist"].ToString();
                        MS.SelDist = MS.StoreAllData.Tables[0].Rows[0]["homedistco"].ToString();
                        MS.tehsil = MS.StoreAllData.Tables[0].Rows[0]["tehsil"].ToString();
                        MS.pinCode = MS.StoreAllData.Tables[0].Rows[0]["pincode"].ToString();
                        Session["ChallanID"] = MS.StoreAllData.Tables[0].Rows[0]["challanid"].ToString();

                        int dist = Convert.ToInt32(MS.SelDist);
                        DataSet result1 = objDB.SelectAllTehsil(dist);
                        ViewBag.MyTehsil = result1.Tables[0];
                        List<SelectListItem> TehList = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
                        {

                            TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                        }
                        ViewBag.MyTehsil = TehList;

                        MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                        MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                        MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString();
                        MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString();
                        MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString();
                        MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString();

                        @ViewBag.Photo = MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
                        //MS.imgSign= MS.StoreAllData.Tables[1].Rows[0]["PathSign"].ToString();                        
                        @ViewBag.sign = MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();

                        MS.imgSign = MS.StoreAllData.Tables[0].Rows[0]["Sign_url"].ToString();
                        MS.imgPhoto = MS.StoreAllData.Tables[0].Rows[0]["Photo_url"].ToString();

                        Session["imgPhoto"] = MS.imgPhoto;
                        Session["imgSign"] = MS.imgSign;


                        MS.FormStatus = MS.StoreAllData.Tables[0].Rows[0]["FormStatus"].ToString();

                        return View(MS);
                    }

                }
                return View(MS);
            }
            catch (Exception)
            {
                return RedirectToAction("Logout", "Admin");
            }


        }
        public ActionResult AdminFPPrivateFinalPrint(FormCollection frc)
        {
            PrivateCandidateModels MS = new PrivateCandidateModels();
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            try
            {
                if (Session["OROLL"].ToString() != null || Session["OROLL"].ToString() != "")
                {
                    MS.OROLL = Session["OROLL"].ToString();
                    MS.refNo = Session["refNo"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmationPrint(MS.refNo);
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                    ///----------------------------------------------------------------
                    MS.centrCode = MS.StoreAllData.Tables[0].Rows[0]["cent"].ToString();
                    MS.setNo = MS.StoreAllData.Tables[0].Rows[0]["set"].ToString();
                    MS.SelExamDist = MS.StoreAllData.Tables[0].Rows[0]["ExamDist"].ToString();
                    MS.distName = MS.StoreAllData.Tables[0].Rows[0]["DISTNM"].ToString();

                    if (MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"] == null || MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"].ToString() == "")
                    {
                        MS.DisabilityPercent = 0;
                    }
                    else { MS.DisabilityPercent = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"].ToString()); }
                    MS.IsHardCopyCertificate = MS.StoreAllData.Tables[0].Rows[0]["IsHardCopyCertificate"].ToString();
                    MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["utype"].ToString();
                    MS.category = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                    MS.Stream = MS.StoreAllData.Tables[0].Rows[0]["stream"].ToString();
                    if (MS.category == "R")
                    {
                        MS.category = "Reappear/Compartment";
                    }
                    if (MS.category == "D")
                    {
                        MS.category = "Division improvement";
                    }
                    if (MS.category == "A")
                    {
                        MS.category = "Additional Subject";
                    }
                    MS.phyChal = MS.StoreAllData.Tables[0].Rows[0]["phy_chal"].ToString();


                    MS.Class = MS.StoreAllData.Tables[0].Rows[0]["Class"].ToString();
                    if (MS.Class.ToLower() == "5".ToLower() || MS.Class.ToLower() == "8".ToLower())
                    {
                        MS.Choice1 = "";
                        MS.Choice2 = "";
                    }
                    else
                    {
                        MS.Choice1 = MS.StoreAllData.Tables[0].Rows[0]["cent_1"].ToString();
                        MS.Choice2 = MS.StoreAllData.Tables[0].Rows[0]["cent_2"].ToString();
                    }


                    @ViewBag.Photo = MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
                    @ViewBag.sign = MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();

                    MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                    MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
                    MS.Session = MS.SelMonth + '/' + MS.SelYear;
                    MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                    MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();

                    MS.Fee = MS.StoreAllData.Tables[0].Rows[0]["FEE"].ToString();
                    MS.challanNo = MS.StoreAllData.Tables[0].Rows[0]["challanid"].ToString();
                    MS.BANK = MS.StoreAllData.Tables[0].Rows[0]["BANK"].ToString();
                    MS.BRANCH = MS.StoreAllData.Tables[0].Rows[0]["BRANCH"].ToString();
                    MS.DEPOSITDT = MS.StoreAllData.Tables[0].Rows[0]["DEPOSITDT"].ToString();


                    MS.refNo = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                    MS.SCHL = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                    MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                    MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                    MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();
                    MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
                    MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
                    MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();
                    MS.EPname = MS.Candi_Name + '/' + MS.Pname;
                    MS.EPFname = MS.Father_Name + '/' + MS.PFname;
                    MS.EPMname = MS.Mother_Name + '/' + MS.PMname;

                    MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["sex"].ToString();
                    MS.CastList = MS.StoreAllData.Tables[0].Rows[0]["caste"].ToString();
                    MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                    MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();
                    MS.Result = MS.StoreAllData.Tables[0].Rows[0]["result"].ToString();
                    MS.SearchResult = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();

                    MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();
                    MS.Relist = MS.StoreAllData.Tables[0].Rows[0]["religion"].ToString();
                    if (MS.Relist == "H")
                    {
                        MS.Relist = "Hindu";
                    }
                    if (MS.Relist == "M")
                    {
                        MS.Relist = "Muslim";
                    }
                    if (MS.Relist == "S")
                    {
                        MS.Relist = "Sikh";
                    }
                    if (MS.Relist == "C")
                    {
                        MS.Relist = "Christian";
                    }
                    if (MS.Relist == "O")
                    {
                        MS.Relist = "Others";
                    }

                    MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                    MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                    MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString();
                    MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString();
                    MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString();
                    MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString();

                    MS.sub1code = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                    MS.sub2code = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString();
                    MS.sub3code = MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString();
                    MS.sub4code = MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString();
                    MS.sub5code = MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString();
                    MS.sub6code = MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString();

                    MS.address = MS.StoreAllData.Tables[0].Rows[0]["homeadd"].ToString();
                    MS.block = MS.StoreAllData.Tables[0].Rows[0]["block"].ToString();
                    MS.tehsil = MS.StoreAllData.Tables[0].Rows[0]["TEHSILENM"].ToString();
                    MS.pinCode = MS.StoreAllData.Tables[0].Rows[0]["pinCode"].ToString();

                    MS.addressfull = (MS.address + ',' + MS.block + ',' + MS.distName + ',' + MS.tehsil + ',' + MS.pinCode);




                    return View(MS);
                }
                else
                {
                    return Private_Candidate_Examination_Form();
                }
            }
            catch (Exception ex)
            {
                return Private_Candidate_Examination_Form();
            }
        }
        #endregion Private Final Print using Admin Login

        public ActionResult AdminPrivateCandidateConfirmation(string refno1)
        {
            ViewBag.YesNoList = new AbstractLayer.DBClass().GetYesNoText();
            string start = Request.QueryString["Oroll"];
            string Oroll = string.Empty;
            try
            {
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                PrivateCandidateModels MS = new PrivateCandidateModels();
                if (Session["AdminType"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                //Oroll = "1014819040";

                if (refno1 != "" && refno1 != null)
                {
                    TempData["refno1"] = refno1;
                    refno1 = encrypt.QueryStringModule.Decrypt(refno1);
                    MS.refNo = refno1;
                    Session["refno"] = refno1;
                    //refno = Session["refno"].ToString();

                    List<SelectListItem> Monthlist = objDB.GetMonth();
                    ViewBag.MyMonth = Monthlist;

                    List<SelectListItem> yearlist = objDB.GetSessionYear1();
                    ViewBag.MyYear = yearlist;


                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);

                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        return View();
                    }
                    else
                    {
                        if (MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"] == null || MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"].ToString() == "")
                        {
                            MS.DisabilityPercent = 0;
                        }
                        else { MS.DisabilityPercent = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["DisabilityPercent"].ToString()); }
                        MS.IsHardCopyCertificate = MS.StoreAllData.Tables[0].Rows[0]["IsHardCopyCertificate"].ToString();
                        MS.Stream = MS.StoreAllData.Tables[0].Rows[0]["Exam"].ToString();
                        MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["rp"].ToString();
                        //MS.category = MS.StoreAllData.Tables[1].Rows[0]["category"].ToString();
                        if (MS.Exam_Type == "R")
                        {
                            MS.Exam_Type = "Regular";
                        }
                        if (MS.Exam_Type == "O")
                        {
                            MS.Exam_Type = "Open";
                        }
                        if (MS.Exam_Type == "P")
                        {
                            MS.Exam_Type = "Private";
                        }
                        if (MS.category == "SR")
                        {
                            MS.category = "Golden Chance Re-Appear";
                        }
                        if (MS.category == "SD")
                        {
                            MS.category = "Golden Chance Improvement";
                        }
                        if (MS.category == "SA")
                        {
                            MS.category = "Golden Chance Additional";
                        }
                        //MS.category = MS.StoreAllData.Tables[1].Rows[0]["category"].ToString();
                        MS.category = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                        if (MS.category == "R")
                        {
                            MS.category = "Reappear/Compartment";
                        }
                        if (MS.category == "D")
                        {
                            MS.category = "Division improvement";
                        }
                        if (MS.category == "A")
                        {
                            MS.category = "Additional Subject";
                        }
                        //if (MS.category == "SR")
                        //{
                        //    MS.category = "Golden Chance Re-Appear";
                        //}
                        //if (MS.category == "SD")
                        //{
                        //    MS.category = "Golden Chance Improvement";
                        //}
                        //if (MS.category == "SA")
                        //{
                        //    MS.category = "Golden Chance Additional";
                        //}
                        Session["form"] = MS.StoreAllData.Tables[0].Rows[0]["Feecat"].ToString();



                        MS.Class = MS.StoreAllData.Tables[0].Rows[0]["class"].ToString();
                        if (MS.Class == "10")
                        {
                            MS.Class = MS.StoreAllData.Tables[0].Rows[0]["classNM"].ToString();
                            MS.MatricSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            MS.MatricSub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString();
                        }
                        else if (MS.Class == "12")
                        {
                            MS.TwelveSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            MS.Class = MS.StoreAllData.Tables[0].Rows[0]["classNM"].ToString();
                        }
                        else if (MS.Class == "5" || MS.Class == "8")
                        {
                            MS.TwelveSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            MS.Class = MS.StoreAllData.Tables[0].Rows[0]["classNM"].ToString();
                        }
                        MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                        MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
                        MS.Session = MS.SelMonth + ' ' + MS.SelYear;

                        MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                        Session["OROLL"] = MS.OROLL;
                        Session["category"] = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();

                        MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();
                        MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();

                        MS.refNo = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                        MS.SCHL = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();

                        MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                        MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                        MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();

                        MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();
                        MS.Result = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();

                        MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
                        MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
                        MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();

                        MS.EPname = MS.Candi_Name + '/' + MS.Pname;
                        MS.EPFname = MS.Father_Name + '/' + MS.PFname;
                        MS.EPMname = MS.Mother_Name + '/' + MS.PMname;

                        MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["sex"].ToString();
                        MS.CastList = MS.StoreAllData.Tables[0].Rows[0]["caste"].ToString();
                        MS.Area = MS.StoreAllData.Tables[0].Rows[0]["area"].ToString();
                        if (MS.Area == "U")
                        {
                            MS.Area = "Urban";
                        }
                        if (MS.Area == "R")
                        {
                            MS.Area = "Rural";
                        }
                        MS.Relist = MS.StoreAllData.Tables[0].Rows[0]["religion"].ToString();
                        if (MS.Relist == "H")
                        {
                            MS.Relist = "Hindu";
                        }
                        if (MS.Relist == "M")
                        {
                            MS.Relist = "Muslim";
                        }
                        if (MS.Relist == "S")
                        {
                            MS.Relist = "Sikh";
                        }
                        if (MS.Relist == "C")
                        {
                            MS.Relist = "Christian";
                        }
                        if (MS.Relist == "O")
                        {
                            MS.Relist = "Others";
                        }

                        //MS.phyChal= MS.StoreAllData.Tables[0].Rows[0]["phy_chal"].ToString();
                        //if (MS.phyChal == "true")
                        //{
                        //    MS.phyChal = "true";
                        //}
                        //MS.rdoWantWriter = MS.StoreAllData.Tables[0].Rows[0]["writer"].ToString();
                        //if (MS.rdoWantWriter == "True")
                        //{
                        //    MS.rdoWantWriter = "true";
                        //}

                        //@ViewBag.DA = objDB.GetDA();
                        MS.phyChal = MS.StoreAllData.Tables[0].Rows[0]["phy_chal"].ToString();

                        //if (MS.phyChal != "")
                        //{
                        //    @ViewBag.DA = MS.phyChal;
                        //}

                        MS.rdoWantWriter = MS.StoreAllData.Tables[0].Rows[0]["writer"].ToString();
                        if (MS.rdoWantWriter == "True")
                        {
                            MS.rdoWantWriter = "1";
                        }
                        if (MS.rdoWantWriter == "False")
                        {
                            MS.rdoWantWriter = "0";
                        }

                        MS.IsPracExam = MS.StoreAllData.Tables[0].Rows[0]["prac"].ToString();
                        if (MS.IsPracExam == "True")
                        {
                            MS.IsPracExam = "1";
                        }
                        if (MS.IsPracExam == "False")
                        {
                            MS.IsPracExam = "0";
                        }


                        MS.address = MS.StoreAllData.Tables[0].Rows[0]["homeadd"].ToString();

                        MS.landmark = MS.StoreAllData.Tables[0].Rows[0]["LandMark"].ToString();
                        MS.block = MS.StoreAllData.Tables[0].Rows[0]["block"].ToString();
                        //MS.MyDist = MS.StoreAllData.Tables[0].Rows[0]["distName"].ToString();
                        MS.SelExamDist = MS.StoreAllData.Tables[0].Rows[0]["dist"].ToString();

                        int exmdist = Convert.ToInt32(MS.SelExamDist);
                        DataSet resultEC = objDB.SelectAllTehsilEC(exmdist);
                        ViewBag.MyTehsilEC = resultEC.Tables[0];
                        ViewBag.MyTehsilEC2 = resultEC.Tables[0];
                        List<SelectListItem> TehListEC = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyTehsilEC.Rows)
                        {

                            TehListEC.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                        }

                        ViewBag.MyTehsilEC = TehListEC;
                        ViewBag.MyTehsilEC2 = TehListEC;


                        //MS.Choice1 = "";
                        //MS.Choice2 = "";

                        MS.Class = MS.StoreAllData.Tables[0].Rows[0]["Class"].ToString();
                        if (MS.Class.ToLower() == "5".ToLower() || MS.Class.ToLower() == "8".ToLower())
                        {
                            MS.tehsilEC = "";
                            MS.tehsilEC2 = "";
                        }
                        else
                        {
                            MS.tehsilEC = MS.StoreAllData.Tables[0].Rows[0]["Cent_1"].ToString();
                            MS.tehsilEC2 = MS.StoreAllData.Tables[0].Rows[0]["Cent_2"].ToString();
                        }


                        MS.SelDist = MS.StoreAllData.Tables[0].Rows[0]["homedistco"].ToString();
                        MS.tehsil = MS.StoreAllData.Tables[0].Rows[0]["tehsil"].ToString();
                        MS.pinCode = MS.StoreAllData.Tables[0].Rows[0]["pincode"].ToString();
                        Session["ChallanID"] = MS.StoreAllData.Tables[0].Rows[0]["challanid"].ToString();
                        DataSet result = objDB.SelectDist(); // passing Value to DBClass from model
                        ViewBag.MyDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> items = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                        {
                            items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                        }
                        // ViewBag.MyDist = items;
                        ViewBag.MyDist = new SelectList(items, "Value", "Text");

                        if (MS.SelDist == "")
                        {
                            MS.SelDist = "0";
                        }

                        ViewBag.MyExamDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> items1 = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyExamDist.Rows)
                        {
                            items1.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                        }
                        // ViewBag.MyDist = items;
                        ViewBag.MyExamDist = new SelectList(items1, "Value", "Text");

                        if (MS.SelExamDist == "")
                        {
                            MS.SelExamDist = "0";
                        }

                        int dist = Convert.ToInt32(MS.SelDist);
                        DataSet result1 = objDB.SelectAllTehsil(dist);
                        ViewBag.MyTehsil = result1.Tables[0];
                        List<SelectListItem> TehList = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
                        {

                            TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                        }
                        ViewBag.MyTehsil = TehList;


                        //if (MS.category == "R" || MS.category == "SR" || MS.category == "SD" || MS.category == "SA")
                        //{
                        //    MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                        //    MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                        //    MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString();
                        //    MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString();
                        //    MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString();
                        //    MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString();
                        //}
                        //if ((MS.category == "SR" || MS.category == "SD" || MS.category == "SA"))
                        //{
                        //    MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                        //    MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString();
                        //    MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString();
                        //    MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString();
                        //    MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString();
                        //    MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString();
                        //}
                        MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                        MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString();
                        MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString();
                        MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString();
                        MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString();
                        MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString();
                        MS.sub7 = MS.StoreAllData.Tables[0].Rows[0]["rsub7code"].ToString();
                        MS.sub8 = MS.StoreAllData.Tables[0].Rows[0]["rsub8code"].ToString();
                        if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "R" || MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "D" || MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A")
                        {
                            //MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                            //MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                            //MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString();
                            //MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString();
                            //MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString();
                            //MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString();

                            //MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            //MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString();
                            //MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString();
                            //MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString();
                            //MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString();
                            //MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString();
                            List<SelectListItem> Mitems = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[1].Rows)
                            {
                                Mitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.SubMatric = new SelectList(Mitems, "Value", "Text");

                            List<SelectListItem> sitems = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[2].Rows)
                            {
                                sitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.SubTwelve = new SelectList(sitems, "Value", "Text");
                        }
                        //else if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "D")
                        //{
                        //    MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                        //    MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                        //    MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString();
                        //    MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString();
                        //    MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString();
                        //    MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString();
                        //}
                        else if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                        {
                            List<SelectListItem> Mitems = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[1].Rows)
                            {
                                Mitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.SubMatric = new SelectList(Mitems, "Value", "Text");


                        }
                        else if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "12")
                        {
                            List<SelectListItem> sitems = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[2].Rows)
                            {
                                sitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.SubTwelve = new SelectList(sitems, "Value", "Text");
                        }
                        if ((MS.category == "SR" || MS.category == "SD" || MS.category == "SA") && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                        {
                            List<SelectListItem> DMitems1 = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[3].Rows)
                            {
                                DMitems1.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.sub1Matric = new SelectList(DMitems1, "Value", "Text");
                        }
                        if ((MS.category == "SR" || MS.category == "SD" || MS.category == "SA") && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "12")
                        {
                            List<SelectListItem> DMitems1 = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[4].Rows)
                            {
                                DMitems1.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.Sub1Twelve = new SelectList(DMitems1, "Value", "Text");
                        }
                        if (MS.category == "SR")
                        {
                            MS.category = "Golden Chance Re-Appear";
                        }
                        if (MS.category == "SD")
                        {
                            MS.category = "Golden Chance Improvement";
                        }
                        if (MS.category == "SA")
                        {
                            MS.category = "Golden Chance Additional";
                        }
                        @ViewBag.Photo = MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
                        //MS.imgSign= MS.StoreAllData.Tables[1].Rows[0]["PathSign"].ToString();                        
                        @ViewBag.sign = MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();

                        MS.imgSign = MS.StoreAllData.Tables[0].Rows[0]["Sign_url"].ToString();
                        MS.imgPhoto = MS.StoreAllData.Tables[0].Rows[0]["Photo_url"].ToString();

                        Session["imgPhoto"] = MS.imgPhoto;
                        Session["imgSign"] = MS.imgSign;


                        MS.FormStatus = MS.StoreAllData.Tables[0].Rows[0]["FormStatus"].ToString();

                        return View(MS);
                        //return Private_Candidate_Examination_Form();
                        //return Pre_Private_Candidate_Introduction_Form(frm);
                    }
                }
                else
                {
                    //return View(MS);

                    //return Private_Candidate_Examination_Form();
                    return RedirectToAction("AdminFPPrivateSearch", "PrivateCandidate");
                }
            }
            catch (Exception ex)
            {
                //return Private_Candidate_Examination_Form();
                return RedirectToAction("AdminFPPrivateSearch", "PrivateCandidate");
            }
        }


        [HttpPost]
        public ActionResult AdminPrivateCandidateConfirmation(PrivateCandidateModels MS, FormCollection frm)
        {
            ViewBag.YesNoList = new AbstractLayer.DBClass().GetYesNoText();
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            try
            {

                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();

                if (Session["AdminType"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }

                DataSet result = objDB.SelectDist(); // passing Value to DBClass from model
                ViewBag.MyDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }
                // ViewBag.MyDist = items;
                ViewBag.MyDist = new SelectList(items, "Value", "Text");

                ViewBag.MyExamDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items1 = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyExamDist.Rows)
                {
                    items1.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }
                // ViewBag.MyDist = items;
                ViewBag.MyExamDist = new SelectList(items1, "Value", "Text");

                if (MS.SelExamDist == "")
                {
                    MS.SelExamDist = "0";
                }


                if (frm["SelExamDist"] != null)
                {
                    MS.SelExamDist = frm["SelExamDist"].ToString();
                }
                else
                {
                    MS.SelExamDist = "0";
                }

                List<SelectListItem> Monthlist = objDB.GetMonth();
                ViewBag.MyMonth = Monthlist;

                List<SelectListItem> yearlist = objDB.GetSessionYear1();
                ViewBag.MyYear = yearlist;

                MS.SearchString = frm["SearchString"].ToString(); // Remarks enter by admin
                MS.Exam_Type = frm["Exam_Type"].ToString();
                MS.Stream = frm["Stream"].ToString();

                MS.refNo = frm["refNo"].ToString();
                //MS.Session = frm["Session"].ToString();
                MS.SelMonth = frm["SelMonth"].ToString();
                MS.SelYear = frm["SelYear"].ToString();
                MS.Result = frm["result"].ToString();
                MS.Class = frm["Class"].ToString();
                if (MS.Class.ToLower() == "Matriculation".ToLower())
                {
                    MS.Class = "10";
                    MS.DOB = frm["DOB"].ToString();
                }
                else if (MS.Class.ToLower() == "Senior Secondary".ToLower())
                {
                    MS.Class = "12";
                    MS.DOB = "";
                }
                else if (MS.Class.ToLower() == "Primary".ToLower())
                {
                    MS.Class = "5";
                    MS.DOB = frm["DOB"].ToString();
                }
                else if (MS.Class.ToLower() == "Middle".ToLower())
                {
                    MS.Class = "8";
                    MS.DOB = frm["DOB"].ToString();
                }


                MS.Candi_Name = frm["Candi_Name"].ToString();
                MS.Father_Name = frm["Father_Name"].ToString();
                MS.Mother_Name = frm["Mother_Name"].ToString();

                MS.Pname = frm["PNAME"].ToString();
                MS.PFname = frm["PFNAME"].ToString();
                MS.PMname = frm["PMNAME"].ToString();



                MS.RegNo = frm["RegNo"].ToString();
                MS.OROLL = frm["OROLL"].ToString();
                Session["OROLL"] = MS.OROLL;
                Session["refno"] = MS.refNo;
                MS.mobileNo = frm["mobileNo"].ToString();
                MS.emailID = frm["emailID"].ToString();

                MS.Gender = frm["Gender"].ToString();

                MS.CastList = frm["CastList"].ToString();
                MS.Area = frm["Area"].ToString();
                if (MS.Area == "Urban")
                {
                    MS.Area = "U";
                }
                if (MS.Area == "Rural")
                {
                    MS.Area = "R";
                }
                MS.Relist = frm["Relist"].ToString();
                if (MS.Relist == "Hindu")
                {
                    MS.Relist = "H";
                }
                if (MS.Relist == "Muslim")
                {
                    MS.Relist = "M";
                }
                if (MS.Relist == "Sikh")
                {
                    MS.Relist = "S";
                }
                if (MS.Relist == "Christian")
                {
                    MS.Relist = "C";
                }
                if (MS.Relist == "Others")
                {
                    MS.Relist = "O";
                }

                MS.IsphysicalChall = frm["phyChal"].ToString();
                if (MS.IsphysicalChall != "N.A." && frm["rdoWantWriter"] != null)
                {
                    //MS.IsphysicalChall = frm["phyChal"].ToString();
                    MS.rdoWantWriter = frm["rdoWantWriter"].ToString();
                    if (MS.rdoWantWriter == "Yes")
                    {
                        MS.rdoWantWriter = "1";
                    }
                    else
                    {
                        MS.rdoWantWriter = "0";
                    }
                }
                else
                {
                    MS.IsphysicalChall = "N.A.";
                    MS.rdoWantWriter = "0";
                }


                int exmdist = Convert.ToInt32(MS.SelExamDist);
                DataSet resultEC = objDB.SelectAllTehsilEC(exmdist);
                ViewBag.MyTehsilEC = resultEC.Tables[0];
                List<SelectListItem> TehListEC = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyTehsilEC.Rows)
                {

                    TehListEC.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                }
                ViewBag.MyTehsilEC = TehListEC;
                ViewBag.MyTehsilEC2 = TehListEC;




                if (MS.Class.ToLower() == "5".ToLower() || MS.Class.ToLower() == "8".ToLower())
                {
                    MS.Choice1 = "";
                    MS.Choice2 = "";
                }
                else
                {
                    MS.Choice1 = frm["tehsilEC"].ToString();
                    MS.Choice2 = frm["tehsilEC2"].ToString();
                }

                MS.address = frm["address"].ToString();

                MS.landmark = frm["landmark"].ToString();
                MS.block = frm["block"].ToString();
                MS.SelDist = frm["SelDist"].ToString();

                int dist = Convert.ToInt32(MS.SelDist);
                DataSet result1 = objDB.SelectAllTehsil(dist);
                ViewBag.MyTehsil = result1.Tables[0];
                List<SelectListItem> TehList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
                {

                    TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                }
                ViewBag.MyTehsil = TehList;

                MS.tehsil = frm["tehsil"].ToString();
                MS.pinCode = frm["pinCode"].ToString();
                MS.category = frm["category"].ToString();

                if (MS.category == "Reappear/Compartment")
                {
                    MS.category = "R";
                }
                if (MS.category == "Division improvement")
                {
                    MS.category = "D";
                }
                if (MS.category == "Additional Subject")
                {
                    MS.category = "A";
                }
                if (MS.category == "A" || MS.category == "D")
                {
                    MS.IsPracExam = "0";
                }
                if (MS.category == "Golden Chance Re-Appear")
                {
                    MS.category = "SR";
                }
                if (MS.category == "Golden Chance Improvement")
                {
                    MS.category = "SD";
                }
                if (MS.category == "Golden Chance Additional")
                {
                    MS.category = "SA";
                }
                else
                {
                    MS.IsPracExam = frm["IsPracExam"].ToString();
                    if (MS.IsPracExam == "Yes")
                    {
                        MS.IsPracExam = "1";
                    }
                    else
                    {
                        MS.IsPracExam = "0";
                    }
                }
                if (MS.category == "R" || MS.category == "D" || MS.category == "A" || MS.category == "SR" || MS.category == "SD" || MS.category == "SA")
                {
                    if (frm["sub1"] != null) { MS.sub1 = frm["sub1"].ToString(); }
                    else { MS.sub1 = ""; }
                    if (frm["sub2"] != null) { MS.sub2 = frm["sub2"].ToString(); }
                    else { MS.sub2 = ""; }
                    if (frm["sub3"] != null) { MS.sub3 = frm["sub3"].ToString(); }
                    else { MS.sub3 = ""; }
                    if (frm["sub4"] != null) { MS.sub4 = frm["sub4"].ToString(); }
                    else { MS.sub4 = ""; }
                    if (frm["sub5"] != null) { MS.sub5 = frm["sub5"].ToString(); }
                    else { MS.sub5 = ""; }
                    if (frm["sub6"] != null) { MS.sub6 = frm["sub6"].ToString(); }
                    else { MS.sub6 = ""; }
                    if (frm["sub7"] != null) { MS.sub7 = frm["sub7"].ToString(); }
                    else { MS.sub7 = ""; }
                    if (frm["sub8"] != null) { MS.sub8 = frm["sub8"].ToString(); }
                    else { MS.sub8 = ""; }

                }



                if (MS.std_Photo != null)
                {
                    string Orgfile = MS.refNo + "P" + ".jpg";
                    //MS.PathPhoto = "PvtPhoto/Photo/" + MS.refNo + "P" + ".jpg";
                    MS.PathPhoto = "PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo/" + Orgfile;

                    //MS.std_Photo.SaveAs(pathPhoto);

                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            ///file.CopyTo(newMemoryStream);

                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = MS.std_Photo.InputStream,
                                Key = string.Format("allfiles/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }
                }
                else
                {
                    //Response.Write("<script>alert('Please Upload Photo')</script>");
                    //return View();
                }
                if (MS.std_Sign != null)
                {
                    string Orgfile = MS.refNo + "S" + ".jpg";
                    MS.PathSign = "PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign/" + Orgfile;

                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            ///file.CopyTo(newMemoryStream);

                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = MS.std_Sign.InputStream,
                                Key = string.Format("allfiles/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }


                }
                if (MS.PathPhoto != "" && MS.PathPhoto != null)
                {
                    MS.imgPhoto = MS.PathPhoto;
                }
                if (MS.PathSign != "" && MS.PathSign != null)
                {
                    MS.imgSign = MS.PathSign;
                }
                if (MS.PathPhoto == "" || MS.PathPhoto == null)
                {
                    MS.PathPhoto = Session["imgPhoto"].ToString();
                    MS.imgPhoto = Session["imgPhoto"].ToString();
                }
                if (MS.PathSign == "" || MS.PathSign == null)
                {
                    MS.PathSign = Session["imgSign"].ToString();
                    MS.imgSign = Session["imgSign"].ToString();
                }






                if ((MS.std_Photo == null || MS.std_Sign == null) && (MS.imgPhoto == null || MS.imgSign == null))
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select Photo and Sign";
                    TempData["SelectPhotoSign"] = "0";
                    return View(MS);
                }


                if (MS.Gender == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select Gender";
                    TempData["SelectGender"] = "0";
                    return View(MS);
                }

                if (MS.CastList == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select Cast";
                    TempData["SelectCast"] = "0";
                    return View(MS);
                }
                if (MS.Area == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select Area";
                    TempData["SelectArea"] = "0";
                    return View(MS);
                }
                if (MS.Relist == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select Religion";
                    TempData["SelectRelist"] = "0";
                    return View(MS);
                }





                if (MS.address == "")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Enter Address";
                    TempData["Selectaddress"] = "0";
                    return View(MS);
                }
                if (MS.SelDist == "" || MS.tehsil == "" || MS.tehsil == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select District & Tehsil";
                    TempData["SelectDist"] = "0";
                    return View(MS);
                }
                if (MS.pinCode == "" || MS.pinCode.Count() != 6)
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Enter Pin Code";
                    TempData["SelectPin"] = "0";
                    return View(MS);
                }

                MS.AdminId = adminLoginSession.AdminId.ToString();
                MS.EmpUserId = adminLoginSession.AdminEmployeeUserId;

                DataSet result2 = objDB.UpdateAdminPrivateCandidateConfirmation(MS);
                if (result2.Tables[0].Rows[0]["RESULT"].ToString() == "1")
                {
                    TempData["refno"] = MS.refNo;
                    TempData["Status"] = "3";
                }


            }
            catch (Exception ex)
            {
            }
            return RedirectToAction("AdminPrivateCandidateConfirmation", "PrivateCandidate", new { refno1 = encrypt.QueryStringModule.Encrypt(MS.refNo) });

        }



        #region PvtPracticalExamCentre

        public ActionResult PvtPracticalExamCentre()
        {

            var itemAction = new SelectList(new[] { new { ID = "1", Name = "Refno" }, new { ID = "2", Name = "Roll No" }, }, "ID", "Name", 1);
            ViewBag.MyAction = itemAction.ToList();
            ViewBag.SelectedAction = "0";
            //------------------------
            return View();
        }

        [HttpPost]
        public ActionResult PvtPracticalExamCentre(FormCollection frm, string cmd)
        {
            try
            {

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "Refno" }, new { ID = "2", Name = "Roll No" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                PrivateCandidateModels rm = new PrivateCandidateModels();
                string Search = string.Empty;
                if (cmd == "Search")
                {
                    Search += "refno is not null";

                    int SelAction = 0;
                    if (frm["SelAction"] != "")
                    {
                        ViewBag.SelectedAction = frm["SelAction"];
                        SelAction = Convert.ToInt32(frm["SelAction"].ToString());
                        if (frm["SelAction"] != "" && frm["SearchString"].ToString() != "")
                        {
                            if (SelAction == 1)
                            { Search += " and refno='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelAction == 2)
                            { Search += " and roll='" + frm["SearchString"].ToString() + "'"; }

                        }
                    }

                    rm.StoreAllData = objDB.PvtPracticalExamCentre(Search);
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                    }
                }
                return View(rm);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        #endregion PvtPracticalExamCentre

        #region Private DIC pricatical subject update
        public ActionResult PrivateSearchDIC_Prac(string id)
        {
            if (id == null || id == "")
            {
                ViewData["result"] = "0";
                return View();
            }
            else
            {
                ViewData["result"] = "1";
                string CLASS1 = "";
                if (id.ToUpper() == "SENIOR") // For Senior
                {
                    CLASS1 = "12";
                    ViewBag.ClassName = "Senior Secondary";
                }
                else if (id.ToUpper() == "MATRIC") // For MAtric
                {
                    CLASS1 = "10";
                    ViewBag.ClassName = "Matric";
                }
                ViewBag.cid = id;
                return View();
            }
        }
        [HttpPost]
        public ActionResult PrivateSearchDIC_Prac(FormCollection frc, string cmd, string id)
        {
            try
            {
                ViewBag.cid = id;
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                PrivateCandidateModels rm = new PrivateCandidateModels();

                string Search = string.Empty;
                if (cmd == "Search")
                {
                    Search += " a.refno is not null";

                    if (frc["refNo"] != "")
                    {
                        Search += " and a.refno ='" + frc["refNo"].ToString() + "'";
                    }
                    if (frc["Candi_Name"].Trim() != "")
                    {
                        Search += " and a.name like '" + frc["Candi_Name"].ToString() + "%'";
                    }
                    if (frc["Father_Name"].Trim() != "")
                    {
                        Search += " and a.fname like '" + frc["Father_Name"].ToString() + "%'";
                    }
                    if (frc["OROLL"].Trim() != "")
                    {
                        Search += " and a.roll ='" + frc["OROLL"].ToString() + "'";
                    }
                    // rm.category = frc["category"].ToString();                  
                    rm.OROLL = frc["OROLL"].ToString();
                    rm.refNo = frc["refNo"].ToString();
                    rm.Candi_Name = frc["Candi_Name"].ToString();
                    rm.Father_Name = frc["Father_Name"].ToString();

                    rm.StoreAllData = objDB.PrivateSearchDIC_Prac(Search);
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message2 = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                    }

                }

                if (ModelState.IsValid)
                { return View(rm); }
                else
                { return View(); }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        public JsonResult upd_PrivateSearchDIC_Prac(string refno, string Prac)
        {

            try
            {
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                string dee = "";
                string outstatus = "";
                string Search = string.Empty;
                //string UserType = "Admin";               
                //float fee = 0;              
                //DateTime date;              
                string res = objDB.upd_PrivateSearchDIC_Prac(refno, Prac);
                if (res != "0")
                {
                    dee = "Yes";
                }
                else
                    dee = "No";


                return Json(new { sn = dee, chid = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return null;
            }
        }
        #endregion Private DIC pricatical subject update

        #region Candidates to View Practical Exam Centre Details. Regular,Open, Re-Appear/Additional/DIC,
        public ActionResult CandViewPracExamCentDtl()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CandViewPracExamCentDtl(FormCollection frc, string cmd)
        {
            try
            {
                string CLASS1 = "";
                AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
                PrivateCandidateModels rm = new PrivateCandidateModels();

                string Search = string.Empty;
                if (cmd == "Search")
                {
                    rm.Exam_Type = frc["Exam_Type"].ToString();
                    rm.SearchBy = frc["SearchBy"].ToString();
                    rm.SearchString = frc["SearchString"].ToString();

                    Search += " reg.refno is not null";
                    if (rm.SearchBy.Contains("Roll"))
                    {
                        Search += " and reg.roll ='" + rm.SearchString + "'";
                    }
                    else if (rm.SearchBy.Contains("Ref"))
                    {
                        Search += " and reg.refno ='" + rm.SearchString + "'";
                    }

                    if (rm.SearchBy != "0")
                    {
                        rm.StoreAllData = objDB.CandViewPracExamCentDtl(Search, rm.Exam_Type);
                        if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message2 = "Record Not Found";
                            ViewBag.TotalCount = 0;
                        }
                        else
                        {
                            ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                        }
                    }

                }
                if (ModelState.IsValid)
                { return View(rm); }
                else
                { return View(); }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        #endregion Candidates to View Practical Exam Centre Details. Regular,Open, Re-Appear/Additional/DIC,


        public ActionResult UnlockFinalSubmitPrivateCandidate(string roll)
        {
            PrivateCandidateModels MS = new PrivateCandidateModels();

            if (Session["refno"] == null || Session["Oroll"] == null || Session["Oroll"].ToString() == "" || Session["form"] == null)
            {
                return RedirectToAction("Private_Candidate_Examination_Form", "PrivateCandidate");
            }

            roll = Session["Oroll"].ToString();
            string RefNo = Session["refno"].ToString();
            MS.StoreAllData = new AbstractLayer.PrivateCandidateDB().UnlockFinalSubmitPrivateCandidate(RefNo);
            if (MS.StoreAllData != null)
            {
                if (MS.StoreAllData.Tables[0].Rows[0]["result"].ToString() == "1")
                {
                    ViewData["roll"] = Session["OROLL"].ToString();
                    ViewData["refno"] = Session["refNo"].ToString();
                    ViewData["Status"] = "1";
                    return RedirectToAction("PrivateCandidateConfirmation", "PrivateCandidate");
                }
            }

            return Private_Candidate_Examination_Form();
        }






    }
}


