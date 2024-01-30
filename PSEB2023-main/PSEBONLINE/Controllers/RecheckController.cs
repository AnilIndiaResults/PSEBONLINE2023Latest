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
using ClosedXML;
using ClosedXML.Excel;
using System.Data.OleDb;
using System.Configuration;
using CCA.Util;
using PSEBONLINE.Filters;
using System.Threading.Tasks;
using PsebPrimaryMiddle.Controllers;
using PSEBONLINE.Controllers;

namespace PSEBONLINE.Controllers
{
    public class RecheckController : Controller
    {
        #region SiteMenu
        //Executes before every action
        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {


                string actionName = context.ActionDescriptor.ActionName;
                string controllerName = context.ActionDescriptor.ControllerDescriptor.ControllerName;
                base.OnActionExecuting(context);
                if (Session["AdminId"] == null)
                { }
                else
                {
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    string AdminType = Session["AdminType"].ToString();
                    List<SiteMenu> all = new List<SiteMenu>();
                    DataSet result = objCommon.GetAdminDetailsById(Convert.ToInt32(Session["AdminId"]), Convert.ToInt32(Session["Session"].ToString().Substring(0, 4)));
                    if (result.Tables[2].Rows.Count > 0)
                    {
                        bool exists = true;
                        DataSet dsIsExists = objCommon.GetActionOfSubMenu(0, controllerName, actionName);
                        int IsExists = Convert.ToInt32(dsIsExists.Tables[0].Rows[0]["IsExist"].ToString());
                        if (IsExists == 1 || Session["myIP"] != null || AdminType.ToString().ToUpper() == "ADMIN" || actionName.ToString().ToUpper() == "PAGENOTAUTHORIZED" || actionName.ToString().ToUpper() == "INDEX" || actionName.ToString().ToUpper() == "LOGOUT" || actionName.ToString().ToUpper() == "Change_Password")
                        {
                            exists = true;
                        }
                        else
                        {
                            exists = result.Tables[2].AsEnumerable().Where(c => c.Field<string>("Controller").ToUpper().Equals(controllerName.ToUpper()) && c.Field<string>("Action").ToUpper().Equals(actionName.ToUpper())).Count() > 0;
                        }

                        if (exists == false)
                        {
                            context.Result = new RedirectToRouteResult(
                             new System.Web.Routing.RouteValueDictionary(new { controller = "Admin", action = "PageNotAuthorized" }));
                            return;
                        }
                        else
                        {
                            foreach (System.Data.DataRow dr in result.Tables[2].Rows)
                            {
                                all.Add(new SiteMenu { MenuID = Convert.ToInt32(@dr["MenuID"]), MenuName = @dr["MenuName"].ToString(), MenuUrl = @dr["MenuUrl"].ToString(), ParentMenuID = Convert.ToInt32(@dr["ParentMenuID"]), IsMenu = Convert.ToInt32(@dr["IsMenu"]) });
                            }

                            if (result.Tables[1].Rows.Count > 0)
                            {
                                string DistAllow = "";
                                if (Session["DistAllow"].ToString() == "")
                                {
                                    ViewBag.DistAllow = null;
                                }
                                else
                                {
                                    if (Session["DistAllow"].ToString().EndsWith(","))
                                    { DistAllow = Session["DistAllow"].ToString().Remove(Session["DistAllow"].ToString().LastIndexOf(","), 1); }
                                    else
                                    {
                                        DistAllow = Session["DistAllow"].ToString();
                                    }
                                    ViewBag.DistAllow = DistAllow;
                                }

                                List<SelectListItem> itemDist = new List<SelectListItem>();
                                foreach (System.Data.DataRow dr in result.Tables[1].Rows)
                                {
                                    itemDist.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                                }

                                ViewBag.DistUser = itemDist;
                            }
                        }
                    }
                    else
                    {
                        context.Result = new RedirectToRouteResult(
                            new System.Web.Routing.RouteValueDictionary(new { controller = "Admin", action = "PageNotAuthorized" }));
                        return;
                    }
                    ViewBag.SiteMenu = all;
                }
            }
            catch (Exception)
            {
                context.Result = new RedirectToRouteResult(
                             new System.Web.Routing.RouteValueDictionary(new { controller = "Admin", action = "Index" }));
                return;
            }
        }

        #endregion SiteMenu        
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.HomeDB objDBHome = new AbstractLayer.HomeDB();
        AbstractLayer.RecheckDB objDB = new AbstractLayer.RecheckDB();
        RecheckModels MS = new RecheckModels();
        // GET: Recheck

        public JsonResult IsRollNoExists(string OROLL)
        {
            bool dup = true;
            string search = "Roll = '" + OROLL + "'";
            DataSet dschk = objDB.RecheckMasterCheckSearch(1, search);
            if (dschk.Tables.Count > 0)
            {
                if (dschk.Tables[0].Rows.Count > 0)
                {
                    dup = false;
                }
            }
            return Json(dup, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Index()
        {
            return View();
        }
        #region Recheck Login and New User Introduction        
        public ActionResult RecheckForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RecheckForgotPassword(FormCollection frm, RecheckModels MS)
        {
            AbstractLayer.RecheckDB objDB = new AbstractLayer.RecheckDB();
            try
            {
                MS.Class = frm["Class"];
                MS.Exam_Type = frm["Exam_Type"];
                MS.SelMonth = frm["batch"];
                MS.SelYear = frm["batchYear"];
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
        public ActionResult RecheckExamination()
        {
            Session["refno"] = null;
            Session["roll"] = null;
            Session["ChallanID"] = null;


            return View();
        }
        [HttpPost]
        public ActionResult RecheckExamination(FormCollection frc)
        {
            try
            {
                RecheckModels MS = new RecheckModels();
                AbstractLayer.RecheckDB objDB = new AbstractLayer.RecheckDB();
                MS.refNo = frc["refNo"];
                MS.ROLL = frc["ROLL"];

                if (MS.refNo != null && MS.refNo.ToString() != "")
                {
                    MS.StoreAllData = objDB.GetRecheckExamination(MS);
                    ViewBag.Message = "";
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Given Reference & Roll number not currect.";
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                    else
                    {

                        if (MS.refNo == MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString() && MS.ROLL == MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString())
                        {
                            Session["roll"] = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                            ViewData["refno"] = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                            Session["refno"] = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                            ViewData["roll"] = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                            ViewData["Status"] = "1";
                            //return View(MS);
                            return RedirectToAction("RecheckConfirmation", "Recheck");
                            //return RedirectToAction("MigrationRec", "MigrateSchool");
                        }
                        return View(MS);
                    }
                }
                return View();
            }
            catch (Exception ex)
            {

                return RedirectToAction("RecheckExamination", "Recheck");
            }

        }

        public ActionResult RecheckAgree()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RecheckAgree(FormCollection frm)
        {
            try
            {
                string s = frm["Agree"].ToString();
                //ViewBag.FormName = Session["FormName"].ToString();
                if (s == "Agree")
                {
                    return RedirectToAction("RecheckIntroduction", "Recheck");
                }
                else
                {
                    return RedirectToAction("RecheckExamination", "Recheck");
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction("RecheckExamination", "Recheck");
            }
        }
        public ActionResult RecheckIntroduction(RecheckModels MS)
        {
            return View();
        }
        [HttpPost]
        public ActionResult RecheckIntroduction(FormCollection frm, RecheckModels MS)
        {
            AbstractLayer.RecheckDB objDB = new AbstractLayer.RecheckDB();

            List<SelectListItem> yearlist = objDB.GetSessionYear1();
            ViewBag.MyYear = yearlist;
            List<SelectListItem> Monthlist = objDB.GetMonth();
            ViewBag.MyMonth = Monthlist;
            try
            {
                MS.batch = frm["batch"];
                MS.batchYear = frm["batchYear"];
                MS.Class = frm["Class"];
                MS.Exam_Type = frm["Exam_Type"];
                MS.OROLL = frm["OROLL"];
                MS.emailID = frm["emailID"];
                MS.mobileNo = frm["mobileNo"];

                if (frm["OROLL"] != null && frm["OROLL"] != "")
                {
                    DataSet result2 = objDB.InsertTblRecheck(MS); // After Verify Roll no and All Data Insert Data and Create RefNo 
                    if (result2.Tables[0].Rows.Count > 0)
                    {
                        if (result2.Tables[0].Rows[0]["result"].ToString() == "1")
                        {
                            string Oroll = frm["OROLL"].ToString();
                            string Clss = frm["Class"];
                            string Yar = frm["SelYear"];
                            string Mnth = frm["SelMonth"];
                            string refno = result2.Tables[0].Rows[0]["refno"].ToString();
                            ViewData["Status"] = result2.Tables[0].Rows[0]["result"].ToString();
                            Session["Oroll"] = Oroll;
                            Session["refno"] = refno;
                            TempData["refno"] = refno;
                            TempData["roll"] = Oroll;
                            TempData["Mnth"] = Mnth;

                            //TempData["Yar"] = Yar;
                            //TempData["Mnth"] = Mnth;

                            //----------------- Email and SMS --------
                            try
                            {
                                string clsNm = string.Empty;
                                AbstractLayer.DBClass dbclass = new AbstractLayer.DBClass();
                                if (MS.mobileNo != null || MS.mobileNo != "")
                                {

                                    string Sms = string.Empty;
                                    if (Clss == "10")
                                    {
                                        clsNm = "Matriculation";
                                        Sms = "Your are registered for Re-Checking for class Matric " + Mnth + " - " + Yar + " with ref no " + refno + " against roll no " + Oroll + ", keep this sms till result declaration.";
                                    }
                                    else
                                    {
                                        clsNm = "Senior Secondy";
                                        Sms = "Your are registered for Re-Checking for class Sr. Sec " + Mnth + " - " + Yar + " with ref no " + refno + " against roll no " + Oroll + ", keep this sms till result declaration.";
                                    }
                                    string getSms = dbclass.gosms(MS.mobileNo, Sms);
                                    // string getSms = objCommon.gosms("9711021501", Sms);
                                }
                                if (MS.emailID != null || MS.emailID != "")
                                {
                                    string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Dear " + ViewData["name"] + "</b>,</td></tr><tr><td height=" + 30 + ">As per your request Dated <b>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</b> Regarding Recheching Examination Form</td></tr><tr><td><b>You are successfully registred for:-</b><br /><b>Class :</b> " + clsNm + ", " + MS.batch + " 2017 <br /><b> Reference No. :</b> " + refno + "<br /><b> Roll No. :</b> " + Oroll + " <br /><b> Keep this for further use till result declaration.</b> <br /></td></tr><tr><td height=" + 30 + "><b>Click Here To Login</b> <a href=https://www.registration.pseb.ac.in/Recheck/RecheckExamination target = _blank>www.registration.pseb.ac.in</a></td></tr><tr><td><b>Note:</b> Please Read Instruction Carefully Before filling the Online Form .</td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /><tr><td><b>Contact Us</b><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 18002700280<br>DISTRICTS:- BARNALA, FATEHGARH SAHIB, GURDASPUR, HOSHIARPUR, JALANDHAR, KAPURTHALA, SHRI MUKTSAR SAHIB, S.B.S. NAGAR, PATHANKOT, PATIALA, SANGRUR, CHANDIGARH &amp; OTHER STATES<br><br><b>Email Id:</b> <a href=mailto:Contact2@psebonline.in target=_blank>contact2@psebonline.in</a><br><b>Toll Free Help Line No. :</b> 18004190690<br>DISTRICTS:- AMRITSAR, BATHINDA, FARIDKOT, FAZILKA, FEROZEPUR, LUDHIANA, MANSA, MOGA, ROOP NAGAR, S.A.S NAGAR,TARN TARAN<br></td></tr>";

                                    string subject = "PSEB-Rechecking Subject Form Notification";
                                    bool result3 = dbclass.mail(subject, body, MS.emailID);
                                }

                            }
                            catch (Exception ex)
                            {
                                return View(MS);
                            }
                            return View(MS);


                            //----------------- email and sms

                        }
                        else if (result2.Tables[0].Rows[0]["result"].ToString() == "2")
                        {
                            ViewData["roll"] = MS.OROLL;
                            ViewData["RefNo"] = result2.Tables[0].Rows[0]["refno"].ToString();
                            ViewData["Status"] = "2";

                            return View(MS);
                        }
                        else if (result2.Tables[0].Rows[0]["result"].ToString() == "3")
                        {

                            ViewData["roll"] = MS.OROLL;
                            ViewData["Status"] = "3";

                            return View(MS);

                        }
                        else if (result2.Tables[0].Rows[0]["result"].ToString() == "0")
                        {
                            ViewData["roll"] = MS.OROLL;
                            ViewData["Status"] = "0";

                            return View(MS);
                        }
                        else
                        {
                            ViewData["roll"] = MS.OROLL;
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
        #endregion Recheck Login and New User Introduction

        #region Step-2 Confirmation Page for Rechecking and RTI
        public ActionResult RecheckConfirmation()
        {
            try
            {
                if (Session["refno"] != null && Session["refno"].ToString() != "")
                {
                    MS.refNo = Session["refno"].ToString();
                    MS.StoreAllData = objDB.GetRecheckConfirmation(MS.refNo); // Get Data for Particular Ref_No

                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        return RedirectToAction("RecheckExamination", "Recheck");
                    }
                    else
                    {
                        if (MS.StoreAllData.Tables[2].Rows.Count == 0)
                        {
                            ViewBag.Message = "Subject Not Added for Rechecking and RTI";
                            ViewBag.Islock2 = ViewBag.TotalCount2 = 0;
                        }
                        else
                        {
                            ViewBag.Islock2 = MS.StoreAllData.Tables[2].Rows[0]["Islock"].ToString();

                            ViewBag.TotalCount2 = MS.StoreAllData.Tables[2].Rows.Count;
                        }


                        if (MS.StoreAllData.Tables[3].Rows.Count == 0)
                        {
                            ViewBag.Message = "Subject Not Added for Rechecking and RTI";
                            ViewBag.Islock3 = ViewBag.TotalCount3 = 0;
                        }
                        else
                        {
                            ViewBag.Islock3 = MS.StoreAllData.Tables[3].Rows[0]["Islock"].ToString();

                            ViewBag.TotalCount3 = MS.StoreAllData.Tables[3].Rows.Count;
                        }


                        if (MS.StoreAllData.Tables[4].Rows.Count == 0)
                        {
                            ViewBag.TotalCount4 = 0;
                        }
                        else
                        {
                            //ViewBag.Islock3 = MS.StoreAllData.Tables[4].Rows[0]["Islock"].ToString();

                            ViewBag.TotalCount4 = MS.StoreAllData.Tables[4].Rows.Count;
                        }

                        ViewBag.RecheckLastDate = MS.StoreAllData.Tables[0].Rows[0]["RecheckLastDate"].ToString();
                        MS.SdtID = MS.StoreAllData.Tables[0].Rows[0]["Std_Id"].ToString();
                        MS.refNo = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                        MS.Class = MS.StoreAllData.Tables[0].Rows[0]["Class"].ToString();
                        MS.ClassNM = MS.StoreAllData.Tables[0].Rows[0]["ClassNM"].ToString();
                        MS.ROLL = MS.StoreAllData.Tables[0].Rows[0]["ROLL"].ToString();
                        MS.batch = MS.StoreAllData.Tables[0].Rows[0]["month"].ToString();
                        MS.batchYear = MS.StoreAllData.Tables[0].Rows[0]["Year"].ToString();
                        MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["Candi_Name"].ToString();
                        MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["Father_Name"].ToString();
                        MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["Mother_Name"].ToString();
                        MS.address = MS.StoreAllData.Tables[0].Rows[0]["address"].ToString();
                        MS.Result = MS.StoreAllData.Tables[0].Rows[0]["Result"].ToString();
                        MS.IsRecheck = true;
                        //MS.obtmark = MS.StoreAllData.Tables[0].Rows[0]["Result"].ToString();
                        Session["ChallanID"] = MS.StoreAllData.Tables[0].Rows[0]["challanid"].ToString();


                        ViewBag.MySubList = MS.StoreAllData.Tables[1];// ViewData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> items = new List<SelectListItem>();
                        if (MS.StoreAllData.Tables[1].Rows.Count > 0)
                        {
                            foreach (System.Data.DataRow dr in ViewBag.MySubList.Rows)
                            {
                                //items.Add(new SelectListItem { Text = @dr["subNM"].ToString(), Value = @dr["sub"].ToString() });
                                items.Add(new SelectListItem { Text = @dr["subNM"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            string groupNM = MS.StoreAllData.Tables[1].Rows[0]["Exam"].ToString();
                            if (groupNM.ToUpper() == "V")
                            {
                                items.Add(new SelectListItem { Text = "GENERAL FOUNDATION COURSE", Value = "138/000" });
                            }
                        }
                        ViewBag.MySubList = new SelectList(items, "Value", "Text");
                    }
                    return View(MS);
                }
                else
                {
                    return RedirectToAction("RecheckExamination", "Recheck");
                }

            }
            catch (Exception)
            {
                return RedirectToAction("RecheckExamination", "Recheck");
            }


        }
        [HttpPost]
        public ActionResult RecheckConfirmation(FormCollection frm, string Recheckevaluation, bool IsRTI)
        {
            RecheckModels MS = new RecheckModels();
            try
            {
                if (Session["refno"] != null && Session["refno"].ToString() != "")
                {
                    if ((Recheckevaluation != null || Recheckevaluation != "" || IsRTI != false) && MS.SubList != "")
                    {
                        MS.SubList = frm["SubList"].ToString();
                        MS.refNo = Session["refno"].ToString();
                        MS.SdtID = frm["SdtId"].ToString();
                        MS.ROLL = frm["ROLL"].ToString();
                        MS.Class = frm["Class"].ToString();
                        MS.address = frm["address"].ToString();

                        MS.StoreAllData = objDB.InsertRecheckSubjectList(MS, Recheckevaluation, IsRTI);
                        if (MS.StoreAllData.Tables[0].Rows.Count > 0)
                        {
                            ViewData["Status"] = MS.StoreAllData.Tables[0].Rows[0]["result"].ToString();
                            //IsRecheck = false;
                            //IsRTI = false;
                        }

                    }


                    MS.StoreAllData = objDB.GetRecheckConfirmation(MS.refNo); // Get Data for Particular Ref_No                                        
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        return RedirectToAction("RecheckExamination", "Recheck");

                        //ViewBag.Message = "Given Reference & Roll number not currect.";
                        //ViewBag.TotalCount = 0;
                        //return View(MS);
                    }
                    else
                    {
                        if (MS.StoreAllData.Tables[2].Rows.Count == 0)
                        {
                            ViewBag.Message = "Subject Not Added for Rechecking and RTI";
                            ViewBag.TotalCount2 = 0;
                        }
                        else
                        {
                            ViewBag.TotalCount2 = MS.StoreAllData.Tables[2].Rows.Count;
                        }
                        MS.refNo = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                        MS.Class = MS.StoreAllData.Tables[0].Rows[0]["Class"].ToString();
                        MS.ClassNM = MS.StoreAllData.Tables[0].Rows[0]["ClassNM"].ToString();
                        MS.ROLL = MS.StoreAllData.Tables[0].Rows[0]["ROLL"].ToString();
                        MS.batch = MS.StoreAllData.Tables[0].Rows[0]["month"].ToString();
                        MS.batchYear = MS.StoreAllData.Tables[0].Rows[0]["Year"].ToString();
                        MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["Candi_Name"].ToString();
                        MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["Father_Name"].ToString();
                        MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["Mother_Name"].ToString();
                        MS.address = MS.StoreAllData.Tables[0].Rows[0]["address"].ToString();
                        MS.Result = MS.StoreAllData.Tables[0].Rows[0]["Result"].ToString();
                        //MS.obtmark = MS.StoreAllData.Tables[0].Rows[0]["Result"].ToString();
                        Session["ChallanID"] = MS.StoreAllData.Tables[0].Rows[0]["challanid"].ToString();



                        ViewBag.MySubList = MS.StoreAllData.Tables[1];// ViewData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> items = new List<SelectListItem>();
                        if (MS.StoreAllData.Tables[1].Rows.Count > 0)
                        {
                            foreach (System.Data.DataRow dr in ViewBag.MySubList.Rows)
                            {
                                //items.Add(new SelectListItem { Text = @dr["subNM"].ToString(), Value = @dr["sub"].ToString() });
                                items.Add(new SelectListItem { Text = @dr["subNM"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            string groupNM = MS.StoreAllData.Tables[1].Rows[0]["Exam"].ToString();
                            if (groupNM.ToUpper() == "V")
                            {
                                items.Add(new SelectListItem { Text = "GENERAL FOUNDATION COURSE", Value = "138/000" });
                            }
                        }
                        ViewBag.MySubList = new SelectList(items, "Value", "Text");
                    }
                    return View(MS);
                }
                else
                {
                    return RedirectToAction("RecheckExamination", "Recheck");
                }

            }
            catch (Exception)
            {
                return RedirectToAction("RecheckExamination", "Recheck");
            }


        }

        public ActionResult UnlockRecheckFinalSubmit(FormCollection frm)
        {
            try
            {
                if (Session["refno"] == null || Session["refno"].ToString() == "")
                {
                    return RedirectToAction("RecheckExamination", "Recheck");
                }
                else
                {
                    MS.refNo = Session["refno"].ToString();
                    string result = objDB.UnlockRecheckFinalSubmit(MS.refNo);
                    if (result != "")
                    {
                        ViewBag.result = "1";
                        ViewData["Status"] = result;
                    }

                }
                return RedirectToAction("RecheckConfirmation", "Recheck");
            }
            catch (Exception)
            {
                return RedirectToAction("RecheckConfirmation", "Recheck");
            }

        }

        public ActionResult RecheckFinalSubmit(FormCollection frm)
        {
            try
            {
                if (Session["refno"] == null || Session["refno"].ToString() == "")
                {
                    return RedirectToAction("RecheckExamination", "Recheck");
                }
                else
                {
                    MS.refNo = Session["refno"].ToString();
                    string result = objDB.RecheckFinalSubmit(MS.refNo);
                    if (result != "")
                    {
                        @ViewBag.result = "1";
                        ViewData["Status"] = result;
                    }

                }
                return RedirectToAction("RecheckConfirmation", "Recheck");
            }
            catch (Exception)
            {
                return RedirectToAction("RecheckConfirmation", "Recheck");
            }

        }
        public ActionResult RecheckDeleteRecord(string id)
        {
            try
            {
                if (Session["refno"] == null || Session["refno"].ToString() == "")
                {
                    return RedirectToAction("RecheckExamination", "Recheck");
                }
                if (id == null)
                {
                    return RedirectToAction("RecheckExamination", "Recheck");
                }
                else
                {
                    string result = objDB.RecheckDeleteRecord(id);
                    if (result.ToUpper() == "DELETED")
                    {
                        @ViewBag.result = "1";
                        ViewData["Status"] = "DEL";
                    }

                }
                return RedirectToAction("RecheckConfirmation", "Recheck");
            }
            catch (Exception)
            {
                return RedirectToAction("RecheckConfirmation", "Recheck");
            }
        }

        public ActionResult RecheckFinalPrint(string Id)
        {
            try
            {
                if (Session["refno"] == null || Session["refno"].ToString() == "")
                {
                    return RedirectToAction("RecheckExamination", "Recheck");
                }
                else
                {
                    string ChallanId = Id.ToString();
                    MS.refNo = Session["refno"].ToString();
                    MS.StoreAllData = objDB.GetRecheckFinalPrint_SPRN(MS.refNo, ChallanId); // Get Data for Particular Ref_No                                        
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        return RedirectToAction("RecheckExamination", "Recheck");
                    }
                    ViewBag.IsRecheckReEval = MS.StoreAllData.Tables[0].Rows[0]["IsRecheckReEval"].ToString();
                    MS.Fee = NumberToWords(Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["totalfee"].ToString())).ToUpper();
                    return View(MS);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("RecheckExamination", "Recheck");
            }
        }
        #endregion Step-2 Confurmation End


        #region Challan and Payment Details
        public ActionResult PaymentForm()
        {
            try
            {
                RecheckPaymentformViewModel pfvm = new RecheckPaymentformViewModel();
                if (Session["refno"] == null || Session["refno"].ToString() == "")
                {
                    return RedirectToAction("RecheckExamination", "Recheck");
                }
                string RefNo = Session["refno"].ToString();
                string form = "";//set in procedure
                DataSet ds = objDB.GetRecheckDetailsPayment(RefNo, form);
                pfvm.PaymentFormData = ds;
                if (pfvm.PaymentFormData == null || pfvm.PaymentFormData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View(pfvm);
                }
                else
                {
                    if (string.IsNullOrEmpty(ds.Tables[0].Rows[0]["LOT"].ToString()))
                    { pfvm.LOTNo = 1; }
                    else
                    {
                        pfvm.LOTNo = Convert.ToInt32(ds.Tables[0].Rows[0]["LOT"].ToString()) + 1;
                    }

                    pfvm.Class = ds.Tables[0].Rows[0]["class"].ToString();
                    if (pfvm.Class == "10")
                    {
                        pfvm.Class = "Matriculation";

                    }
                    if (pfvm.Class == "12")
                    {
                        pfvm.Class = "Senior Secondary";
                    }
                    pfvm.ExamType = ds.Tables[0].Rows[0]["type"].ToString();
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
                    pfvm.category = ds.Tables[0].Rows[0]["category"].ToString();
                    pfvm.Name = ds.Tables[0].Rows[0]["name"].ToString();
                    pfvm.RegNo = ds.Tables[0].Rows[0]["regno"].ToString();
                    pfvm.RefNo = ds.Tables[0].Rows[0]["refno"].ToString();
                    pfvm.roll = ds.Tables[0].Rows[0]["roll"].ToString();

                    Session["roll"] = pfvm.roll;  // Set roll no for Payment

                    //pfvm.Dist = ds.Tables[0].Rows[0]["Dist"].ToString();
                    if (pfvm.category.ToUpper() == "PVT")
                    {
                        pfvm.Dist = ds.Tables[0].Rows[0]["DistPVT"].ToString();
                        pfvm.District = ds.Tables[0].Rows[0]["DISTNMPVT"].ToString();
                        pfvm.SchoolCode = "0000000";
                        pfvm.SchoolName = "NONE"; // Schollname with station and dist 
                    }
                    else
                    {
                        pfvm.Dist = ds.Tables[0].Rows[0]["Dist"].ToString();
                        pfvm.District = ds.Tables[0].Rows[0]["DISTNM"].ToString();
                        pfvm.SchoolCode = ds.Tables[0].Rows[0]["schl"].ToString();
                        pfvm.SchoolName = ds.Tables[0].Rows[0]["SCHLE"].ToString(); // Schollname with station and dist 
                    }

                    ViewBag.Message = "success";
                    ViewBag.TotalCount = pfvm.PaymentFormData.Tables[0].Rows.Count;

                    DataSet dscalFee = ds; //(DataSet)Session["RecheckCalculateFee"];
                    pfvm.TotalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["totfee"].ToString());
                    pfvm.TotalLateFees = 0;//Convert.ToInt32(dscalFee.Tables[1].Rows[0]["RTI_fee"].ToString());
                    pfvm.TotalFinalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["totfee"].ToString());

                    string rps = NumberToWords(Convert.ToInt32(dscalFee.Tables[1].Rows[0]["totfee"].ToString()));
                    //pfvm.TotalFeesInWords = dscalFee.Tables[1].Rows[0]["totfee"].ToString();
                    pfvm.TotalFeesInWords = rps;

                    //  pfvm.FeeDate = Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy");
                    pfvm.FeeDate = Convert.ToDateTime(dscalFee.Tables[1].Rows[0]["eDate"].ToString());

                    //TotalCandidates
                    //pfvm.TotalCandidates = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("CountStudents")));
                    pfvm.FeeCode = dscalFee.Tables[1].Rows[0]["FEECODE"].ToString();
                    pfvm.FeeCategory = dscalFee.Tables[1].Rows[0]["FEECAT"].ToString();
                    //pfvm.BankLastDate = Convert.ToDateTime(dscalFee.Tables[1].Rows[0]["BankLastdate"].ToString());
                    pfvm.BankLastDate = Convert.ToDateTime(dscalFee.Tables[1].Rows[0]["BankLastdate"].ToString());

                    Session["RecheckPaymentform"] = pfvm;
                    if (pfvm.TotalFinalFees == 0 && pfvm.TotalFees == 0)
                    {
                        ViewBag.CheckForm = 1; // only verify for M1 and T1 
                        Session["ReCheckFormFee"] = 0;
                    }
                    else
                    {
                        ViewBag.CheckForm = 0; // only verify for M1 and T1 
                        Session["ReCheckFormFee"] = 1;
                    }
                    return View(pfvm);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("RecheckExamination", "Recheck");
            }
        }
        [HttpPost]
        public ActionResult PaymentForm(RecheckPaymentformViewModel pfvm, FormCollection frm)
        {
            try
            {
                RecheckChallanMasterModel CM = new RecheckChallanMasterModel();

                if (Session["refno"] == null || Session["refno"].ToString() == "")
                {
                    return RedirectToAction("RecheckExamination", "Recheck");
                }
                string refno = Session["refno"].ToString();
                string roll = Session["roll"].ToString();

                if (pfvm.BankCode == null)
                {
                    ViewBag.Message = "Please Select Bank";
                    ViewData["SelectBank"] = "1";
                    return View(pfvm);
                }

                if (Session["ReCheckFormFee"].ToString() == "0")
                { pfvm.BankCode = "203"; }


                string bankName = "";
                string PayModValue = "";
                if (pfvm.BankCode == "301" || pfvm.BankCode == "302")
                {
                    PayModValue = "online";
                    if (pfvm.BankCode == "301")
                    {
                        bankName = "HDFC Bank";
                    }
                    else if (pfvm.BankCode == "302")
                    {
                        bankName = "Punjab And Sind Bank";
                    }
                }
                else if (pfvm.BankCode == "203")
                {
                    PayModValue = "hod";
                    bankName = "PSEB HOD";
                }
                //else if (pfvm.BankCode == "202" || pfvm.BankCode == "204")
                //{
                //    PayModValue = "offline";
                //    if (pfvm.BankCode    == "202")
                //    {
                //        bankName = "Punjab National Bank";
                //    }
                //    else if (pfvm.BankCode == "204")
                //    {
                //        bankName = "State Bank of India";
                //    }
                //}

                if (ModelState.IsValid)
                {
                    CM.FeeStudentList = "1";
                    RecheckPaymentformViewModel PFVMSession = (RecheckPaymentformViewModel)Session["RecheckPaymentform"];
                    CM.roll = roll;
                    CM.RefNo = refno;
                    //CM.FEE = Convert.ToInt32(PFVMSession.TotalFinalFees);
                    CM.FEE = Convert.ToInt32(PFVMSession.TotalFees);
                    CM.latefee = Convert.ToInt32(PFVMSession.TotalLateFees);
                    CM.TOTFEE = Convert.ToInt32(PFVMSession.TotalFinalFees);
                    CM.FEECAT = PFVMSession.FeeCategory;
                    CM.FEECODE = PFVMSession.FeeCode;
                    CM.FEEMODE = "CASH";
                    CM.BCODE = pfvm.BankCode;
                    CM.BANK = pfvm.BankName = bankName;
                    CM.BANKCHRG = PFVMSession.BankCharges;
                    CM.SchoolCode = PFVMSession.SchoolCode.ToString();
                    CM.DIST = PFVMSession.Dist.ToString();
                    CM.DISTNM = PFVMSession.District;
                    CM.LOT = PFVMSession.LOTNo;
                    // CM.LOT = 1;
                    //CM.SCHLREGID = PFVMSession.roll.ToString();
                    CM.SCHLREGID = PFVMSession.RefNo.ToString();
                    CM.FeeStudentList = PFVMSession.roll.ToString();
                    CM.APPNO = PFVMSession.roll.ToString();
                    CM.type = "candt";
                    CM.CHLNVDATE = Convert.ToString(PFVMSession.BankLastDate);
                    DateTime CHLNVDATE2;
                    if (DateTime.TryParseExact(PFVMSession.BankLastDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out CHLNVDATE2))
                    {
                        CM.ChallanVDateN = CHLNVDATE2;
                    }
                    else
                    {
                        CM.ChallanVDateN = PFVMSession.BankLastDate;
                    }


                    string CandiMobile = "";
                    //string result = "0";

                    // Stop due to online payment 
                    string result = objDB.InsertPaymentFormRecheck(CM, frm, out CandiMobile);
                    if (result == "0")
                    {
                        //--------------Not saved
                        ViewData["result"] = 0;
                    }
                    else if (result == "-1")
                    {
                        //-----alredy exist
                        ViewData["result"] = -1;
                    }
                    else
                    {
                        //Session["ChallanID"] = result;
                        CM.CHLNVDATE = (Convert.ToString(PFVMSession.FeeDate)).Substring(0, 10);
                        var chllanVdt = PFVMSession.FeeDate.ToString("dd/MM/yyyy");


                        ViewBag.ChallanNo = result;
                        string paymenttype = CM.BCODE;
                        // string bnkLastDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["BankLastdate"].ToString()).ToString("dd/MM/yyyy");
                        if (PayModValue.ToString().ToLower().Trim() == "online" && result.ToString().Length > 10)
                        {
                            string TotfeePG = (CM.TOTFEE).ToString();
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
                                string strURL;
                                string MerchantLogin = ConfigurationManager.AppSettings["ATOMLoginId"].ToString();
                                string MerchantPass = ConfigurationManager.AppSettings["ATOMPassword"].ToString();
                                string MerchantDiscretionaryData = "NB";  // for netbank
                                                                          // string ClientCode = "PSEBONLINE";
                                                                          // string ClientCode = CM.SCHLREGID;
                                string ClientCode = CM.APPNO;
                                string ProductID = ConfigurationManager.AppSettings["ATOMProductID"].ToString();
                                string CustomerAccountNo = "0123456789";
                                string TransactionType = "NBFundTransfer";  // for netbank
                                                                            //string TransactionAmount = "1";
                                string TransactionAmount = TotfeePG;

                                string TransactionCurrency = "INR";
                                string TransactionServiceCharge = "0";
                                string TransactionID = ViewBag.ChallanNo;// Unique Challan Number
                                string TransactionDateTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
                                // string TransactionDateTime = "18/10/2019 13:15:19";
                                string BankID = "ATOM";

                                string ru = ConfigurationManager.AppSettings["ATOMRU"].ToString();
                                // User Details
                                //string udf1CustName = CM.SCHLREGID;
                                //string udf2CustEmail = CM.APPNO;
                                //string udf3CustMob = CandiMobile;

                                // User Details
                                string udf1CustName = CM.SCHLREGID; // roll number

                                string udf2CustEmail = CM.FEECAT; /// Kindly submit Appno/Refno in client id, Fee cat in Emailid (ATOM)
                                string udf3CustMob = CandiMobile;


                                strURL = GatewayController.ATOMTransferFund(MerchantLogin, MerchantPass, MerchantDiscretionaryData, ProductID, ClientCode, CustomerAccountNo, TransactionType,
                                  TransactionAmount, TransactionCurrency, TransactionServiceCharge, TransactionID, TransactionDateTime, BankID, ru, udf1CustName, udf2CustEmail, udf3CustMob);

                                if (!string.IsNullOrEmpty(strURL))
                                {
                                    return View("../Gateway/AtomCheckoutUrl", new AtomViewModel(strURL));
                                }
                                else
                                {
                                    ViewData["result"] = -10;
                                    return View(pfvm);
                                }
                            }
                            #endregion Payment Gateyway
                        }
                        else if (result.Length > 5)
                        {

                            string Sms = "Your Challan no. " + result + " against Ref No. " + CM.SCHLREGID + " successfully generated and valid till Dt " + chllanVdt + ". Regards PSEB";
                            try
                            {
                                //string getSms = objCommon.gosms(CandiMobile, Sms);
                            }
                            catch (Exception) { }
                            return RedirectToAction("GenerateChallaan", "Home", new { ChallanId = result });

                        }

                    }
                }
                return View(pfvm);
            }
            catch (Exception ex)
            {
                ViewData["SelectBank"] = "ERR";
                ViewData["ErrorMessage"] = ex.Message;
                return View(pfvm);
                // return RedirectToAction("RecheckExamination", "Recheck");
            }
        }
        public ActionResult GenerateChallaan(string Id)
        {
            try
            {
                Session["Session"] = "2020-2021";
                Session["RoleType"] = "recheck";
                string ChallanId = Id.ToString();
                if (ChallanId == null || ChallanId == "0" || Session["refno"] == null || Session["refno"].ToString() == "")
                {
                    return RedirectToAction("RecheckExamination", "Recheck");
                }
                ChallanMasterModel CM = new ChallanMasterModel();
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
                    CM.APPNO = (ds.Tables[0].Rows[0]["APPNO"].ToString() + " / " + ds.Tables[0].Rows[0]["SCHLREGID"].ToString());
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

                    Session["RecheckCalculateFee"] = null;
                    Session["RecheckPaymentform"] = null;
                    Session["RecheckFeeStudentList"] = null;
                    return View(CM);

                }
            }
            catch (Exception)
            {
                return RedirectToAction("RecheckExamination", "Recheck");
            }
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
        #endregion Challan and Payment Details




        #region Recheck Others Controller
        public ActionResult RecheckUpdateList()
        {
            FormCollection frm = new FormCollection();
            AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
            AdminModels am = new AdminModels();
            var itemsch = new SelectList(new[] { new { ID = "1", Name = "Roll No" }, new { ID = "2", Name = "Ref No" }, }, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();

            return View();

        }
        [HttpPost]
        public ActionResult RecheckUpdateList(string cmd, string SelList, string SearchString, FormCollection frm)
        {
            AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
            AdminModels am = new AdminModels();
            var itemsch = new SelectList(new[] { new { ID = "1", Name = "Roll No" }, new { ID = "2", Name = "Ref No" }, }, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            try
            {
                string Search = "";
                Search = "a.subnm like '%%'";

                if (cmd == "Search")
                {
                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SearchString != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and b.[roll]='" + SearchString.ToString().Trim() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += "and a.refno='" + SearchString.ToString().Trim() + "'"; }
                            Session["Search"] = Search;
                        }
                        ViewBag.Searchstring = SearchString.ToString().Trim();

                        am.StoreAllData = objDB.RecheckUpdateList(Search);
                        ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.lot = SearchString.ToString().Trim();
                        if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                            return View(am);
                        }
                        else
                        {
                            ViewBag.refno = am.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                            ViewBag.TotalCount = 1;
                            return View(am);
                        }

                    }
                    else
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(am);
                    }
                }
                return View(am);

            }
            catch (Exception)
            {
                return View(am);
                //return RedirectToAction("Index", "Admin");
            }
        }
        #endregion Recheck Others  Controller



        #region  Update Recheck Bar Data


        [AdminLoginCheckFilter]
        public ActionResult UpdateRecheckBarData()
        {
            return View();

        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult UpdateRecheckBarData(AdminModels AM, FormCollection frm) // HttpPostedFileBase file
        {
            string AdminType = Session["AdminType"].ToString();
            int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
            try
            {

                string id = frm["Filevalue"].ToString();

                string fileLocation = "";
                string filename = "";
                if (AM.file != null)
                {
                    filename = Path.GetFileName(AM.file.FileName);
                }
                else
                {
                    ViewData["Result"] = "-4";
                    ViewBag.Message = "Please select .xls file only";
                    return View();
                }
                DataSet ds = new DataSet();
                if (AM.file.ContentLength > 0)  //(Request.Files["file"].ContentLength > 0
                {
                    // string fileName1 = "ErrorMIS_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

                    string fileName1 = "UpdateRecheckBarData_" + id.ToString().ToUpper() + '_' + AdminType + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");

                    string fileExtension = System.IO.Path.GetExtension(AM.file.FileName);
                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        fileLocation = Server.MapPath("~/BankUpload/" + fileName1 + fileExtension);
                        if (System.IO.File.Exists(fileLocation))
                        {
                            try
                            {
                                System.IO.File.Delete(fileLocation);
                            }
                            catch (Exception)
                            {

                            }
                        }
                        AM.file.SaveAs(fileLocation);
                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        //connection String for xls file format.
                        //if (Path.GetExtension(path).ToLower().Trim() == ".xls" && Environment.Is64BitOperatingSystem == false)
                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        //connection String for xlsx file format.
                        else if (fileExtension == ".xlsx")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                            fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        //Create Connection to Excel work book and add oledb namespace
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        DataTable dt = new DataTable();
                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString(); // bank_mis     TABLE_NAME
                            t++;
                        }
                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
                        string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(ds);
                        }


                        string CheckMis = "";
                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            ViewData["Result"] = "20";
                            ViewBag.Message = "Empty Excel file";
                            return View();
                        }


                        DataTable dtexport = new DataTable();
                        var duplicates = ds.Tables[0].AsEnumerable()
                             .GroupBy(i => new { Name = i.Field<string>("BAR") })
                             .Where(g => g.Count() > 1)
                             .Select(g => new { g.Key.Name }).ToList();

                        var duplicates2 = ds.Tables[0].AsEnumerable()
                             .GroupBy(i => new { ROLL = i.Field<string>("ROLL"), SUB = i.Field<string>("SUB") })
                             .Where(g => g.Count() > 1)
                             .Select(g => new { g.Key.ROLL, g.Key.SUB }).ToList();


                        if (duplicates.Count() > 0 || duplicates2.Count() > 0)
                        {
                            ViewData["Result"] = "11";
                            ViewBag.Message = "Duplicate Data,Either BAR Code or Roll/Sub Combination";
                            return View();
                        }
                        CheckMis = AbstractLayer.RecheckDB.CheckUpdateRecheckBarDataExcel(ds, out dtexport); // REG

                        if (CheckMis == "")
                        {
                            DataTable dt1 = ds.Tables[0];
                            if (dt1.Columns.Contains("ErrStatus"))
                            {
                                dt1.Columns.Remove("ErrStatus");
                            }
                            dt1.AcceptChanges();

                            string OutResult = "0";

                            var idlist = dt1.AsEnumerable().Select(r => r.Field<string>("BAR")).ToArray();
                            string allbarcode = string.Join(",", idlist);
                            List<Bar12Details> bar12DetailsList = new List<Bar12Details>();
                            if (!string.IsNullOrEmpty(allbarcode))
                            {
                                List<BarAPIModel> barAPIModelsList = new AbstractLayer.PsebAPIServiceDB().GetBar12DetailsByBarCodeAPI(allbarcode);
                                if (barAPIModelsList.Count > 0)
                                {
                                    foreach (BarAPIModel barAPIModel in barAPIModelsList)
                                    {
                                        string ROLL1 = dt1.AsEnumerable().Where(r => r.Field<string>("BAR").Trim() == barAPIModel.Bar.Trim()).Select(r => r.Field<string>("ROLL")).FirstOrDefault();
                                        string SUB1 = dt1.AsEnumerable().Where(r => r.Field<string>("BAR").Trim() == barAPIModel.Bar.Trim()).Select(r => r.Field<string>("SUB")).FirstOrDefault();
                                        string Lvl = dt1.AsEnumerable().Where(r => r.Field<string>("BAR").Trim() == barAPIModel.Bar.Trim()).Select(r => r.Field<string>("LEVEL")).FirstOrDefault();
                                        int Level = Convert.ToInt32(Lvl);
                                        Bar12Details bar12Details = new Bar12Details()
                                        {
                                            Roll = ROLL1,
                                            SubCode = SUB1,

                                            Bag1 = Level == 1 ? barAPIModel.Bag : "",
                                            Bar1 = Level == 1 ? barAPIModel.Bar : "",
                                            Marks1 = Level == 1 ? barAPIModel.Marks : "",
                                            ExEpunjab1 = Level == 1 ? barAPIModel.ExEpunjabId : "",
                                            ExDetail1 = Level == 1 ? barAPIModel.ExDetails : "",
                                            HeEpunjab1 = Level == 1 ? barAPIModel.HeEpunjabId : "",
                                            HeDetail1 = Level == 1 ? barAPIModel.HeDetails : "",
                                            CaEpunjab1 = Level == 1 ? barAPIModel.CaEpunjabid : "",
                                            CaDetail1 = Level == 1 ? barAPIModel.CaDetails : "",
                                            EvnCode1 = Level == 1 ? barAPIModel.EvnCode : "",
                                            EvnDetail1 = Level == 1 ? barAPIModel.EvnDetail : "",


                                            Bag2 = Level == 2 ? barAPIModel.Bag : "",
                                            Bar2 = Level == 2 ? barAPIModel.Bar : "",
                                            Marks2 = Level == 2 ? barAPIModel.Marks : "",
                                            ExEpunjab2 = Level == 2 ? barAPIModel.ExEpunjabId : "",
                                            ExDetail2 = Level == 2 ? barAPIModel.ExDetails : "",
                                            HeEpunjab2 = Level == 2 ? barAPIModel.HeEpunjabId : "",
                                            HeDetail2 = Level == 2 ? barAPIModel.HeDetails : "",
                                            CaEpunjab2 = Level == 2 ? barAPIModel.CaEpunjabid : "",
                                            CaDetail2 = Level == 2 ? barAPIModel.CaDetails : "",
                                            EvnCode2 = Level == 2 ? barAPIModel.EvnCode : "",
                                            EvnDetail2 = Level == 2 ? barAPIModel.EvnDetail : "",

                                            Bag3 = Level == 3 ? barAPIModel.Bag : "",
                                            Bar3 = Level == 3 ? barAPIModel.Bar : "",
                                            Marks3 = Level == 3 ? barAPIModel.Marks : "",
                                            ExEpunjab3 = Level == 3 ? barAPIModel.ExEpunjabId : "",
                                            ExDetail3 = Level == 3 ? barAPIModel.ExDetails : "",
                                            HeEpunjab3 = Level == 3 ? barAPIModel.HeEpunjabId : "",
                                            HeDetail3 = Level == 3 ? barAPIModel.HeDetails : "",
                                            CaEpunjab3 = Level == 3 ? barAPIModel.CaEpunjabid : "",
                                            CaDetail3 = Level == 3 ? barAPIModel.CaDetails : "",
                                            EvnCode3 = Level == 3 ? barAPIModel.EvnCode : "",
                                            EvnDetail3 = Level == 3 ? barAPIModel.EvnDetail : "",
                                            OldResult = "",
                                            NewResult = "",
                                            Diff1 = "",
                                            Diff2 = "",
                                        };

                                        bar12DetailsList.Add(bar12Details);
                                    }
                                }

                                if (bar12DetailsList.Count > 0)
                                {

                                    //Convert list to datatable                                    
                                    DataTable dtBar = new DataTable();
                                    dtBar = AbstractLayer.StaticDB.ConvertListToDataTable(bar12DetailsList);
                                    string OutError = "";

                                    var duplicates3 = dtBar.AsEnumerable()
                                    .GroupBy(i => new { ROLL = i.Field<string>("ROLL"), SUB = i.Field<string>("SubCode") })
                                    .Where(g => g.Count() > 1)
                                    .Select(g => new { g.Key.ROLL, g.Key.SUB }).ToList();

                                    if (duplicates3.Count() > 0)
                                    {
                                        var newDt = dtBar.AsEnumerable()
                                       .GroupBy(i => new { ROLL = i.Field<string>("ROLL"), SUB = i.Field<string>("SubCode") })
                                        .Select(y => y.First())
                                        .CopyToDataTable();

                                        dtBar.Clear();
                                        dtBar = newDt;
                                    }

                                    DataSet dtResult = AbstractLayer.RecheckDB.UpdateRecheckBarData(dtBar, AdminId, out OutError);
                                    if (OutError == "1")
                                    {
                                        ViewBag.Message = "File Uploaded Successfully";
                                        ViewData["Result"] = "1";
                                    }
                                    else
                                    {
                                        ViewBag.Message = "Uploaded Failure, Reason: " + OutError;
                                        ViewData["Result"] = "0";
                                    }
                                }
                            }
                            return View();
                        }
                        else
                        {
                            if (dtexport != null)
                            {
                                return RedirectToAction("ExportDataFromDataTable", "Admin", new { dt = dtexport, filename = id.ToString().ToUpper() + "_ErrorReport" });
                                //  ExportDataFromDataTable(dtexport, id.ToString().ToUpper() + "_ErrorReport");
                            }
                            ViewData["Result"] = "-1";
                            ViewBag.Message = CheckMis;
                            return View();
                        }
                    }
                    else
                    {

                        ViewData["Result"] = "-2";
                        ViewBag.Message = "Please Upload Only .xls file only";
                        return View();
                    }
                }

            }
            catch (Exception ex)
            {
                ////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }
            return View();
        }
        #endregion  UpdateRecheckBarData


        #region RecheckingReEvaluationTypeWiseReports
        public ActionResult RecheckingReEvaluationTypeWiseReports(ReportModel RM)
        {
            if (Session["UserName"] != null)
            {
                var itemModule = new SelectList(new[] { new { ID = "1", Name = "Rechecking" },new { ID = "2", Name = "Rechecking With RTI" },
                    new { ID = "3", Name = "Re-Evaluation" },new { ID = "4", Name = "Re-Evaluation With RTI" },
                    new { ID = "5", Name = "RTI Only" },}, "ID", "Name", 1);
                ViewBag.MyModule = itemModule.ToList();
                ViewBag.SelectedModule = "0";

                ViewBag.Mybatch = new AbstractLayer.RecheckDB().RecheckCurrentYearBatchList().ToList();
                ViewBag.Month = "0";

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Complete Staff List" }, new { ID = "2", Name = "More than 10 percent" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "12", Name = "Senior" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";
                var itemRP = new SelectList(new[] { new { ID = "1", Name = "Regular" }, new { ID = "2", Name = "Open" }, new { ID = "3", Name = "Pvt" }, }, "ID", "Name", 1);
                ViewBag.MyRP = itemRP.ToList();
                ViewBag.SelectedRP = "0";
                return View(RM);
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult RecheckingReEvaluationTypeWiseReports(ReportModel RM, FormCollection frm, string batch, string SelList, string submit) // HttpPostedFileBase file
        {
            try
            {

                var itemModule = new SelectList(new[] { new { ID = "1", Name = "Rechecking" },new { ID = "2", Name = "Rechecking With RTI" },
                    new { ID = "3", Name = "Re-Evaluation" },new { ID = "4", Name = "Re-Evaluation With RTI" },
                    new { ID = "5", Name = "RTI Only" },}, "ID", "Name", 1);
                ViewBag.MyModule = itemModule.ToList();
                ViewBag.SelectedModule = "0";

                ViewBag.Mybatch = new AbstractLayer.RecheckDB().RecheckCurrentYearBatchList().ToList();
                ViewBag.Month = "0";

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Complete Staff List" }, new { ID = "2", Name = "More than 10 percent" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "12", Name = "Senior" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";
                var itemRP = new SelectList(new[] { new { ID = "1", Name = "Regular" }, new { ID = "2", Name = "Open" }, new { ID = "3", Name = "Pvt" }, }, "ID", "Name", 1);
                ViewBag.MyRP = itemRP.ToList();
                ViewBag.SelectedRP = "0";
                string firm = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
                if (Session["UserName"] != null)
                {
                    string AdminType = Session["AdminType"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                    string Search = string.Empty;
                    if (frm["Class"] != "" && !string.IsNullOrEmpty(batch) && !string.IsNullOrEmpty(SelList))
                    {
                        string cls = string.Empty;
                        ViewBag.Selectedcls = cls = frm["Class"];

                        string Selbatch = batch;
                        ViewBag.Month = batch;
                        string selMonth = Selbatch.Split('-')[0];
                        string selYear = Selbatch.Split('-')[1];
                        ViewBag.SelectedItem = SelList;

                        string Module = ViewBag.SelectedModule = frm["Module"];

                        Search = "r.RefNo is not null ";
                        if (!string.IsNullOrEmpty(Module))
                        {
                            if (Module == "1")
                            { Search += " and IsRecheck=1 "; }
                            else if (Module == "2")
                            { Search += " and IsRecheck=1 and IsRTI=1 "; }
                            else if (Module == "3")
                            { Search += " and IsReEvaluation=1 "; }
                            else if (Module == "4")
                            { Search += " and IsReEvaluation=1 and IsRTI=1 "; }
                            else if (Module == "5")
                            { Search += " and IsRTI=1 "; }
                            ViewBag.SelectedModule = Module;
                        }

                        int type = Convert.ToInt32(SelList);
                        string Rp = ViewBag.SelectedRP = frm["RP"];
                        RM.StoreAllData = AbstractLayer.RecheckDB.RecheckingReEvaluationTypeWiseReports(type, cls, selMonth, selYear, "", Search);
                        if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                        }
                        else
                        {
                            ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                        }
                    }
                    else
                    {
                        ViewBag.Message = "2";
                        ViewBag.TotalCount = 0;
                    }
                    return View(RM);
                }
                else
                {
                    return RedirectToAction("Index", "Admin");
                }

            }
            catch (Exception ex)
            {
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }

        }

        #endregion RecheckingReevaluationReports

    }
}