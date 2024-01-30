using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSEBONLINE.Models;
using System.Data;
using System.Web.UI;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon;
using System.Configuration;

namespace PSEBONLINE.Controllers
{
    public class SchoolProfileController : Controller
    {

        private const string BUCKET_NAME = "psebdata";
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


        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
        // GET: SchoolProfile
        public ActionResult Index()
        {
            return View();
        }


        public JsonResult SubjectList(int id)
        {

            return Json(objCommon.GetSubject(id).ToArray(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEduCluster(string BLOCK) // Calling on http post (on Submit)
        {
            AbstractLayer.DEODB OBJDB = new AbstractLayer.DEODB();
            DataSet result = OBJDB.Select_CLUSTER_NAME(BLOCK);
            ViewBag.MyEduCluster = result.Tables[0];
            List<SelectListItem> EduClusterList = new List<SelectListItem>();
            EduClusterList.Add(new SelectListItem { Text = "---Edu Cluster---", Value = "0" });
            foreach (System.Data.DataRow dr in ViewBag.MyEduCluster.Rows)
            {
                EduClusterList.Add(new SelectListItem { Text = @dr["CLUSTER_NAME"].ToString(), Value = @dr["CLUSTER_NAME"].ToString() });
            }
            ViewBag.MyEduCluster = EduClusterList;
            return Json(EduClusterList);

        }


        public JsonResult GetBankDetailsByIFSC(string IFSC)
        {
            string bankname = "";
            string branch = "";
            string address = "";
            string district = "";
            string outid = "0";
            string BANKID = "";
            DataSet ds = objCommon.GetBankNameList(2, "", IFSC);
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    outid = "1";
                    ViewBag.bankname = bankname = ds.Tables[0].Rows[0]["BANK"].ToString();
                    ViewBag.branch = branch = ds.Tables[0].Rows[0]["BRANCH"].ToString().Trim();
                    ViewBag.address = address = ds.Tables[0].Rows[0]["address"].ToString().Trim();
                    ViewBag.district = district = ds.Tables[0].Rows[0]["district"].ToString().Trim();
                    BANKID = ds.Tables[0].Rows[0]["BANKID"].ToString().Trim();
                }
            }
            return Json(new { bank = bankname, br = branch, add = address, dist = district, bankid = BANKID, oid = outid }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Update_School_Information()
        {
            try
            {

                //ViewBag.SessionList = objCommon.GetSessionAll();
                ViewBag.EstalimentYearList = objCommon.GetEstalimentYearList();

                string SCHL = string.Empty;
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                { return RedirectToAction("Index", "Home"); }
                else
                {
                    SCHL = Session["SCHL"].ToString();
                }



                string dist = Session["SCHOOLDIST"].ToString();

                //ViewBag.BankList
                List<SelectListItem> objBankList = new List<SelectListItem>();
                DataSet dsBankData = objCommon.GetBankNameList(0, "", "");
                if (dsBankData.Tables.Count > 0)
                {
                    if (dsBankData.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dsBankData.Tables[0].Rows)
                        {
                            objBankList.Add(new SelectListItem { Text = dr["Bank"].ToString(), Value = dr["Bank"].ToString() });
                        }

                    }
                }
                ViewBag.BankList = objBankList;
                //

                DataSet result1 = objDB.SelectAllTehsil(dist);
                ViewBag.MyTeh = result1.Tables[0];
                List<SelectListItem> TehList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyTeh.Rows)
                {

                    TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                }
                ViewBag.MyTeh = TehList;


                //bind block by dist
                DataSet result = objDB.SelectBlock(dist);
                ViewBag.MyEdublock = result.Tables[0];
                List<SelectListItem> BlockList = new List<SelectListItem>();
                //BlockList.Add(new SelectListItem { Text = "---Edu Block---", Value = "0" });
                foreach (System.Data.DataRow dr in ViewBag.MyEdublock.Rows)
                {
                    BlockList.Add(new SelectListItem { Text = @dr["Edu Block Name"].ToString(), Value = @dr["Edu Block Name"].ToString() });
                }
                ViewBag.MyEdublock = BlockList;

                List<SelectListItem> EduClusterList = new List<SelectListItem>();
                EduClusterList.Add(new SelectListItem { Text = "---Edu Cluster---", Value = "0" });
                ViewBag.MyEduCluster = EduClusterList;


                ViewBag.AREAList = objCommon.GetArea();
                // YesNo 
                ViewBag.YesNoList = objCommon.GetYesNo();
                // Status
                ViewBag.StatusList = objCommon.GetStatus();
                // Session 
                ViewBag.SessionList = objCommon.GetSessionAll(); //ViewBag.SessionList = objCommon.GetSessionAdmin();
                // Class 
                ViewBag.ClassTypeList = objCommon.GetClass();
                // School 
                ViewBag.SchoolTypeList = objCommon.GetSchool();
                // English Dist 
                ViewBag.DistEList = objCommon.GetDistE();
                // Punjabi   Dist         
                ViewBag.DistPList = objCommon.GetDistP();

                DataSet ds = new DataSet();
                SchoolModels sm = objDB.GetSchoolDataBySchl(SCHL, out ds);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    ViewData["resultUSI"] = 2;
                    return View();
                }
                else
                {
                    ViewBag.Bank = sm.Bank;
                    ViewBag.ACNO = sm.acno;
                    ViewBag.NSQF_flag = sm.NSQF_flag;
                    //

                    DataSet Eduresult = new AbstractLayer.RegistrationDB().Select_CLUSTER_NAME(sm.Edublock);
                    ViewBag.MyEduCluster = Eduresult.Tables[0];
                    foreach (System.Data.DataRow dr in ViewBag.MyEduCluster.Rows)
                    {
                        EduClusterList.Add(new SelectListItem { Text = @dr["CLUSTER_NAME"].ToString(), Value = @dr["CLUSTER_NAME"].ToString() });
                    }

                    ViewBag.MyEduCluster = EduClusterList;
                    ViewBag.ESTDYR = sm.SchlEstd;


                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        sm.CorrectionNoOld = ds.Tables[1].Rows[0]["CorrectionNoOld"].ToString();
                        sm.RemarksOld = ds.Tables[1].Rows[0]["RemarksOld"].ToString();
                        sm.RemarkDateOld = ds.Tables[1].Rows[0]["RemarkDateOld"].ToString();
                    }


                }

                return View(sm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }


        [HttpPost]
        public ActionResult Update_School_Information(SchoolModels sm, string chkBank, string BankValue)
        {
            //ViewBag.SessionList = objCommon.GetSessionAll();
            ViewBag.EstalimentYearList = objCommon.GetEstalimentYearList();

            //ViewBag.BankList
            List<SelectListItem> objBankList = new List<SelectListItem>();
            DataSet dsBankData = objCommon.GetBankNameList(0, "", "");
            if (dsBankData.Tables.Count > 0)
            {
                if (dsBankData.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsBankData.Tables[0].Rows)
                    {
                        objBankList.Add(new SelectListItem { Text = dr["Bank"].ToString(), Value = dr["Bank"].ToString() });
                    }

                }
            }
            ViewBag.BankList = objBankList;


            try
            {
                string DOB = sm.DOB;
                string DOJ = sm.DOJ;
                string ExperienceYr = sm.ExperienceYr;
                string PQualification = sm.PQualification;

                string Emailid = sm.EMAILID;
                string Mobile = sm.MOBILE;
                string SCHL = string.Empty;
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                { return RedirectToAction("Index", "Home"); }
                else
                {
                    SCHL = Session["SCHL"].ToString();
                    sm.SCHL = SCHL;
                }


                string dist = Session["SCHOOLDIST"].ToString();

                DataSet mresult1 = objDB.SelectAllTehsil(dist);
                ViewBag.MyTeh = mresult1.Tables[0];
                List<SelectListItem> TehList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyTeh.Rows)
                {

                    TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                }
                ViewBag.MyTeh = TehList;

                //
                DataSet mresult = objDB.SelectBlock(dist);
                ViewBag.MyEdublock = mresult.Tables[0];
                List<SelectListItem> BlockList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyEdublock.Rows)
                {
                    BlockList.Add(new SelectListItem { Text = @dr["Edu Block Name"].ToString(), Value = @dr["Edu Block Name"].ToString() });
                }
                ViewBag.MyEdublock = BlockList;

                List<SelectListItem> EduClusterList = new List<SelectListItem>();
                EduClusterList.Add(new SelectListItem { Text = "---Edu Cluster---", Value = "0" });
                ViewBag.MyEduCluster = EduClusterList;

                // Check A/c number and ifsc code

                if (!string.IsNullOrEmpty(sm.confirmacno))
                {

                    if (string.IsNullOrEmpty(sm.acno))
                    {
                        if (chkBank == null && BankValue == "ViewBank")
                        {
                            sm.acno = sm.confirmacno;
                        }
                        else
                        {
                            ViewData["resultUSI"] = 10;
                            return View(sm);

                        }

                    }
                    else if (sm.acno != sm.confirmacno)
                    {
                        ViewData["resultUSI"] = 11;
                        return View(sm);
                    }
                }

                if (!string.IsNullOrEmpty(sm.IFSC))
                {
                    DataSet dschkIFSC = objCommon.GetBankNameList(1, sm.Bank, sm.IFSC);
                    if (dschkIFSC.Tables == null || dschkIFSC.Tables[0].Rows.Count == 0)
                    {
                        ViewData["resultUSI"] = 12;
                        return View(sm);
                    }
                }
                //


                //#region Call API to update school master details
                //string apiStatus = "";
                //SchoolApiViewModel savm = new SchoolApiViewModel();
                //try
                //{
                //    sm.userip = AbstractLayer.StaticDB.GetFullIPAddress();
                //    savm = new AbstractLayer.PsebAPIServiceDB().UpdateUSIPSEBMainToPsebJunior(sm);
                //    apiStatus = savm.statusCode;
                //    ViewBag.ApiStatus = apiStatus;
                //}
                //catch (Exception)
                //{
                //    ViewBag.ApiStatus = apiStatus;
                //}
                //#endregion

                //int result = apiStatus == "200" ? 1 : 0;


                int result = objDB.UpdateUSI(sm); // passing Value to SchoolDB from model and Type 1 For regular
                if (result > 0)
                {
                    //----------------Sending SMS-----------------

                    //string Sms = "Thanks for using online portal. Your School profile has been updated. Thanks Team, Online Support, PSEB";
                    //try
                    //{
                    //    string getSms = dbclass.gosms(Mobile, Sms);

                    //    //---------------Sending SMS-------------------

                    //    //-------------Sending Email-------------
                    //    string to = Emailid;
                    //    string body = "Thanks for using online portal. Your school profile has been updated. <br/><br/> Thanks,<br/><br/>   Team,<br/> Online Support,PSEB";
                    //    string subject = "Online school Profile update - PSEB";
                    //    bool result1 = dbclass.mail(subject, body, to);                        
                    //    ViewData["resultUSI"] = 1;
                    //    ViewBag.Message = "Your school information is updated successfully. Your correction number is " + result;
                    //    return View(sm);                       
                    //}
                    //catch (Exception ex)
                    //{
                    //    ViewData["resultUSI"] = -1;
                    //    return View();
                    //}
                    ViewData["resultUSI"] = 1;
                    //ViewBag.Message = "Your school information is updated successfully. Your correction number is " + savm.Object.CorrectionNo;
                    ViewBag.Message = "Your school information is updated successfully. Your correction number is " + result;
                    return View(sm);

                }
                else
                {
                    ViewData["resultUSI"] = 0;
                    ModelState.AddModelError("", "Not Update");
                    return View();
                }

            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        public ActionResult Change_Password()
        {
            if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
            { return RedirectToAction("Index", "Home"); }

            return View();
        }


        [HttpPost]
        public async Task<ActionResult> Change_Password(FormCollection frm)
        {
            string SCHL = string.Empty;
            string CurrentPassword = string.Empty;
            string NewPassword = string.Empty;

            if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
            { return RedirectToAction("Index", "Home"); }
            else
            {
                SCHL = Session["SCHL"].ToString();
            }


            if (frm["ConfirmPassword"] != "" && frm["NewPassword"] != "")
            {
                if (frm["ConfirmPassword"].ToString() == frm["NewPassword"].ToString())
                {
                    CurrentPassword = frm["CurrentPassword"].ToString();
                    NewPassword = frm["NewPassword"].ToString();


                    SchoolChangePasswordModel sm = new SchoolChangePasswordModel()
                    {
                        SCHL = Session["SCHL"].ToString(),
                        CurrentPassword = frm["CurrentPassword"].ToString(),
                        NewPassword = frm["NewPassword"].ToString()
                    };

                    //#region Call API to update school master details
                    //string apiStatus = "";
                    //try
                    //{
                    //    apiStatus = await new AbstractLayer.PsebAPIServiceDB().SchoolChangePasswordPSEBMAIN(sm);//SchoolChangePasswordPSEBMAIN
                    //    ViewBag.ApiStatus = apiStatus;
                    //}
                    //catch (Exception)
                    //{
                    //    ViewBag.ApiStatus = apiStatus;
                    //}
                    //#endregion

                    //int result = apiStatus == "200" ? 1 : 0;

                    int result = objDB.SchoolChangePassword(SCHL, CurrentPassword, NewPassword);
                    if (result > 0)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewData["resultSCP"] = 0;
                        ModelState.AddModelError("", "Not Update");
                        return View();
                    }
                }
                else
                {
                    ViewData["resultSCP"] = 3;
                    ModelState.AddModelError("", "Fill All Fields");
                    return View();
                }
            }
            else
            {
                ViewData["resultSCP"] = 2;
                ModelState.AddModelError("", "Fill All Fields");
                return View();
            }
        }


        //[HttpPost]
        //public ActionResult Change_Password(FormCollection frm)
        //{
        //    string SCHL = string.Empty;
        //    string CurrentPassword = string.Empty;
        //    string NewPassword = string.Empty;

        //    if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
        //    { return RedirectToAction("Index", "Home"); }
        //    else
        //    {
        //        SCHL = Session["SCHL"].ToString();
        //    }


        //    if (frm["ConfirmPassword"] != "" && frm["NewPassword"] != "")
        //    {
        //        if (frm["ConfirmPassword"].ToString() == frm["NewPassword"].ToString())
        //        {
        //            CurrentPassword = frm["CurrentPassword"].ToString();
        //            NewPassword = frm["NewPassword"].ToString();


        //            int result = objDB.SchoolChangePassword(SCHL, CurrentPassword, NewPassword); // passing Value to SchoolDB from model and Type 1 For regular
        //            if (result > 0)
        //            {
        //                return RedirectToAction("Index", "Home");
        //            }
        //            else
        //            {
        //                ViewData["resultSCP"] = 0;
        //                ModelState.AddModelError("", "Not Update");
        //                return View();
        //            }
        //        }
        //        else
        //        {
        //            ViewData["resultSCP"] = 3;
        //            ModelState.AddModelError("", "Fill All Fields");
        //            return View();
        //        }
        //    }
        //    else
        //    {
        //        ViewData["resultSCP"] = 2;
        //        ModelState.AddModelError("", "Fill All Fields");
        //        return View();
        //    }
        //}

        //public ActionResult School_Staff_Details(int? id)
        //{
        //    try
        //    {
        //        AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
        //        ViewBag.cadre = objCommon.GetCadre();
        //        // ViewBag.district = objCommon.GetDistED();
        //        DataSet ds = objCommon.Fll_Dist_Details();
        //        District objDis = new District();// create the object of class Employee 
        //        List<District> disList = new List<District>();
        //        int table = Convert.ToInt32(ds.Tables.Count);// count the number of table in dataset
        //        for (int i = 0; i < table; i++)// set the table value in list one by one
        //        {
        //            foreach (DataRow dr in ds.Tables[0].Rows)
        //            {
        //                disList.Add(new District { DIST = Convert.ToInt32(dr["DIST"]), DISTNM = Convert.ToString(dr["DISTNM"]) });
        //            }
        //            disList.Add(new District { DIST = Convert.ToInt32(0), DISTNM = Convert.ToString("Others") });
        //        }
        //        ViewBag.district1 = disList.ToList();

        //        ViewBag.tehs = objCommon.TehsilMaster();
        //        string cmd = "";
        //        if (id != null)
        //        {
        //            if (Convert.ToString(Request.QueryString["command"]) != null)
        //            {
        //                cmd = Request.QueryString["command"].ToString();

        //                if (cmd == "modify")
        //                {
        //                    //ViewBag.cadrename = objCommon.GetCadre();
        //                    int recid = Convert.ToInt32(id);
        //                    DataTable dt = objDB.GetEditSchoolStaffDetails(recid);
        //                    SchoolStaffDetailsModel obj = new SchoolStaffDetailsModel();
        //                    obj.id = Convert.ToInt32(dt.Rows[0]["staffid"]);
        //                    obj.schoolcode = Convert.ToString(dt.Rows[0]["schl"]);
        //                    obj.DistrictId = Convert.ToInt32(dt.Rows[0]["homedistcode"]);
        //                    ViewBag.img = Convert.ToString(dt.Rows[0]["PHOTO"]);
        //                    obj.Name = Convert.ToString(dt.Rows[0]["name"]);
        //                    obj.FName = Convert.ToString(dt.Rows[0]["fname"]);
        //                    obj.DOB = Convert.ToString(dt.Rows[0]["DOB"]);
        //                    obj.Gender = Convert.ToString(dt.Rows[0]["gender"]);
        //                    obj.AadharNo = Convert.ToString(dt.Rows[0]["aadharno"]);
        //                    obj.MobileNo = Convert.ToString(dt.Rows[0]["MOBILENO"]);
        //                    obj.stdCode = Convert.ToString(dt.Rows[0]["STDCODE"]);
        //                    obj.PhoneNo = Convert.ToString(dt.Rows[0]["PHNO"]);
        //                    obj.Email = Convert.ToString(dt.Rows[0]["EMAIL"]);
        //                    obj.appointmentDate = Convert.ToString(dt.Rows[0]["DOA"]);
        //                    obj.joiningDate = Convert.ToString(dt.Rows[0]["DOJ"]);
        //                    //obj.Cadreid = Convert.ToString(dt.Rows[0]["cadre"]);
        //                    //obj.Subjectid = Convert.ToString(dt.Rows[0]["SUBNM"]);
        //                    ViewBag.subjecttid = Convert.ToString(dt.Rows[0]["SUBNM"]);
        //                    ViewBag.caderid = Convert.ToString(dt.Rows[0]["cadre"]);
        //                    //ViewBag.cadre = Convert.ToString(dt.Rows[0]["SUBNM"]);
        //                    // string subjectname = objCommon.GetSubjectName(ViewBag.subjecttid);
        //                    ViewBag.subjectname = ViewBag.subjecttid;
        //                    obj.HouseFlatNo = Convert.ToString(dt.Rows[0]["homeno"]);
        //                    obj.VillWardCity = Convert.ToString(dt.Rows[0]["homecity"]);
        //                    obj.LandMark = Convert.ToString(dt.Rows[0]["homelandmark"]);
        //                    obj.DistanceFromSchool = Convert.ToString(dt.Rows[0]["Distance"]);
        //                    obj.PinCode = Convert.ToString(dt.Rows[0]["pincode"]);
        //                    obj.otherdistrict = Convert.ToString(dt.Rows[0]["homedist"]);
        //                    obj.otherstate = Convert.ToString(dt.Rows[0]["otherstate"]);
        //                    obj.State = Convert.ToString(dt.Rows[0]["homestate"]);
        //                    ViewBag.distchk = Convert.ToString(dt.Rows[0]["homedistcode"]);
        //                    ViewBag.statechk = Convert.ToString(dt.Rows[0]["homestate"]);
        //                    ViewBag.btn = "Update";
        //                    return View(obj);
        //                    //TempData["msg"] = "Record Deleted";
        //                    //return RedirectToAction("Display", "CallCenter");

        //                }
        //            }

        //        }
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
        //        //return RedirectToAction("Logout", "Login");
        //        return View();
        //    }
        //}

        //[HttpPost]
        //public ActionResult School_Staff_Details(SchoolStaffDetailsModel obj, IEnumerable<HttpPostedFileBase> files, string cmd, int? id)
        //{
        //    try
        //    {
        //        AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
        //        ViewBag.cadre = objCommon.GetCadre();
        //        DataSet ds = objCommon.Fll_Dist_Details();
        //        District objDis = new District();// create the object of class Employee 
        //        List<District> disList = new List<District>();
        //        int table = Convert.ToInt32(ds.Tables.Count);// count the number of table in dataset
        //        for (int i = 0; i < table; i++)// set the table value in list one by one
        //        {
        //            foreach (DataRow dr in ds.Tables[0].Rows)
        //            {
        //                disList.Add(new District { DIST = Convert.ToInt32(dr["DIST"]), DISTNM = Convert.ToString(dr["DISTNM"]) });
        //            }
        //            disList.Add(new District { DIST = Convert.ToInt32(0), DISTNM = Convert.ToString("Others") });
        //        }
        //        ViewBag.district1 = disList.ToList();
        //        ViewBag.tehs = objCommon.TehsilMaster();
        //        string outputstatus = "";
        //        string getid = "";

        //        //if (ModelState.IsValid)
        //        //{
        //        //obj.schoolcode = "1111112";
        //        if (obj.DistrictId != 0)
        //            obj.otherdistrict = "";
        //        if (obj.State != "Other")
        //            obj.otherstate = "";
        //        obj.schoolcode = Convert.ToString(Session["SCHL"]);
        //        if (id != null)
        //        {

        //        }
        //        else
        //        {
        //            obj.photo = "";
        //        }
        //        string dist = Convert.ToString(obj.DistrictId);
        //        obj.Cadreid = obj.cadrename;
        //        obj.Subjectid = obj.subjectname;

        //        objDB.pro_insertupdateschool_staff_details(obj, out outputstatus, out getid);
        //        if (outputstatus == "1")
        //        {
        //            TempData["msg"] = "Updated Successfully.";
        //        }
        //        else if (outputstatus == "2")
        //        {
        //            TempData["msg"] = "Inserted Successfully.";
        //        }
        //        int flag = 0;
        //        HttpPostedFileBase file = null;
        //        if (id != null)
        //        {
        //            foreach (var file1 in files)
        //            {

        //                if (file1 != null)
        //                {
        //                    file = file1;
        //                    flag = 1;

        //                }
        //            }
        //        }
        //        else
        //        {
        //            file = obj.file1;
        //            flag = 1;
        //        }
        //        //var path = Path.Combine(Server.MapPath("~/Upload/"+ formName + "/" + dist + "/Photo"), stdPic);
        //        if (flag == 1)
        //        {
        //            var path = Path.Combine(Server.MapPath("~/Upload/" + "StaffDetails" + "/" + dist + "/Photo"), getid + "P" + ".jpg");
        //            string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + "StaffDetails" + "/" + dist + "/Photo"));
        //            if (!Directory.Exists(FilepathExist))
        //            {
        //                Directory.CreateDirectory(FilepathExist);
        //            }
        //            file.SaveAs(path);
        //            string filepathtosave = "../Upload/" + "StaffDetails" + "/" + dist + "/Photo/" + getid + "P" + ".jpg";
        //            ViewBag.ImageURL = filepathtosave;

        //            string PhotoName = "StaffDetails" + "/" + dist + "/Photo" + "/" + getid + "P" + ".jpg";
        //            string UpdatePic = objDB.Updated_Pic_Data(getid, PhotoName);
        //        }
        //        ModelState.Clear();
        //        return RedirectToAction("DisplaySchoolStaffDetails", "SchoolProfile");

        //    }
        //    catch (Exception ex)
        //    {
        //        oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
        //        //return RedirectToAction("Logout", "Login");
        //        return View();
        //    }

        //    // }
        //    // return View();
        //}
        public ActionResult School_Staff_Details_View(int? id)
        {
            string SCHLDIST = string.Empty;
            try
            {

                if (Session["SCHOOLDIST"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    SCHLDIST = Session["SCHOOLDIST"].ToString();
                }
                AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                ViewBag.cadre = objCommon.GetCadre();
                // ViewBag.district = objCommon.GetDistED();
                DataSet ds = objCommon.Fll_Dist_Details();
                District objDis = new District();// create the object of class Employee 
                List<District> disList = new List<District>();
                int table = Convert.ToInt32(ds.Tables.Count);// count the number of table in dataset
                for (int i = 0; i < table; i++)// set the table value in list one by one
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        disList.Add(new District { DIST = Convert.ToInt32(dr["DIST"]), DISTNM = Convert.ToString(dr["DISTNM"]) });
                    }
                    disList.Add(new District { DIST = Convert.ToInt32(0), DISTNM = Convert.ToString("Others") });
                }
                ViewBag.district1 = disList.ToList();

                @ViewBag.DA = objCommon.GetDA();

                int dist = Convert.ToInt32(Session["SCHOOLDIST"].ToString());
                DataSet result1 = objDB.SelectAllTehsil(dist);
                ViewBag.MyTeh = result1.Tables[0];
                List<SelectListItem> TehList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyTeh.Rows)
                {

                    TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                }
                ViewBag.MyTeh = TehList;

                string mydist = Session["SCHOOLDIST"].ToString();
                DataSet result = objDB.SelectBlock(mydist);
                ViewBag.MyEdublock = result.Tables[0];
                List<SelectListItem> BlockList = new List<SelectListItem>();
                //BlockList.Add(new SelectListItem { Text = "---Edu Block---", Value = "0" });
                foreach (System.Data.DataRow dr in ViewBag.MyEdublock.Rows)
                {
                    BlockList.Add(new SelectListItem { Text = @dr["Edu Block Name"].ToString(), Value = @dr["Edu Block Name"].ToString() });
                }
                ViewBag.MyEdublock = BlockList;


                List<SelectListItem> EduClusterList = new List<SelectListItem>();
                EduClusterList.Add(new SelectListItem { Text = "---Edu Cluster---", Value = "0" });
                ViewBag.MyEduCluster = EduClusterList;

                ViewBag.tehs = objCommon.TehsilMaster();
                string cmd = "";
                if (id != null)
                {
                    if (Convert.ToString(Request.QueryString["command"]) != null)
                    {
                        cmd = Request.QueryString["command"].ToString();

                        if (cmd == "View")
                        {
                            //ViewBag.cadrename = objCommon.GetCadre();
                            int recid = Convert.ToInt32(id);
                            DataTable dt = objDB.GetEditSchoolStaffDetails(recid);
                            SchoolStaffDetailsModel obj = new SchoolStaffDetailsModel();
                            obj.id = Convert.ToInt32(dt.Rows[0]["staffid"]);
                            obj.schoolcode = Convert.ToString(dt.Rows[0]["schl"]);
                            obj.DistrictId = Convert.ToInt32(dt.Rows[0]["homedistcode"]);
                            ViewBag.img = Convert.ToString(dt.Rows[0]["PHOTO"]);
                            if (ViewBag.img != null || ViewBag.img != "")
                            {
                                obj.photo = ConfigurationManager.AppSettings["AWSURL"] + ViewBag.img;
                                ViewBag.img = ConfigurationManager.AppSettings["AWSURL"] + ViewBag.img;
                            }
                            else
                            {
                                obj.photo = "StaffDetails" + "/" + SCHLDIST + "/Photo/" + Path.GetFileName(obj.file1.FileName);
                            }
                            obj.Name = Convert.ToString(dt.Rows[0]["name"]);
                            obj.FName = Convert.ToString(dt.Rows[0]["fname"]);
                            obj.DOB = Convert.ToString(dt.Rows[0]["DOB"]);
                            obj.Gender = Convert.ToString(dt.Rows[0]["gender"]);
                            obj.AadharNo = Convert.ToString(dt.Rows[0]["aadharno"]);
                            obj.MobileNo = Convert.ToString(dt.Rows[0]["MOBILENO"]);
                            obj.stdCode = Convert.ToString(dt.Rows[0]["STDCODE"]);
                            obj.PhoneNo = Convert.ToString(dt.Rows[0]["PHNO"]);
                            obj.Email = Convert.ToString(dt.Rows[0]["EMAIL"]);
                            obj.appointmentDate = Convert.ToString(dt.Rows[0]["DOA"]);
                            obj.joiningDate = Convert.ToString(dt.Rows[0]["DOJ"]);
                            //obj.Cadreid = Convert.ToString(dt.Rows[0]["cadre"]);
                            //obj.Subjectid = Convert.ToString(dt.Rows[0]["SUBNM"]);
                            ViewBag.subjecttid = Convert.ToString(dt.Rows[0]["SUBNM"]);
                            ViewBag.caderid = Convert.ToString(dt.Rows[0]["cadre"]);
                            //ViewBag.cadre = Convert.ToString(dt.Rows[0]["SUBNM"]);
                            // string subjectname = objCommon.GetSubjectName(ViewBag.subjecttid);
                            ViewBag.subjectname = ViewBag.subjecttid;
                            obj.HouseFlatNo = Convert.ToString(dt.Rows[0]["homeno"]);
                            obj.VillWardCity = Convert.ToString(dt.Rows[0]["homecity"]);
                            obj.LandMark = Convert.ToString(dt.Rows[0]["homelandmark"]);
                            obj.DistanceFromSchool = Convert.ToString(dt.Rows[0]["Distance"]);
                            obj.PinCode = Convert.ToString(dt.Rows[0]["pincode"]);
                            obj.otherdistrict = Convert.ToString(dt.Rows[0]["homedist"]);
                            obj.otherstate = Convert.ToString(dt.Rows[0]["otherstate"]);
                            obj.State = Convert.ToString(dt.Rows[0]["homestate"]);
                            ViewBag.distchk = Convert.ToString(dt.Rows[0]["homedistcode"]);
                            ViewBag.statechk = Convert.ToString(dt.Rows[0]["homestate"]);

                            //-----------------------
                            obj.Quali = Convert.ToString(dt.Rows[0]["Quali"]);
                            obj.Phychall = dt.Rows[0]["Phychal"].ToString();
                            obj.tehsil = dt.Rows[0]["tehsil"].ToString();
                            obj.Edublock = dt.Rows[0]["EDUBLOCK"].ToString();

                            obj.ExpMonth = dt.Rows[0]["ExpMonth"].ToString();
                            obj.ExpYear = dt.Rows[0]["ExpYear"].ToString();

                            string block = dt.Rows[0]["EDUBLOCK"].ToString();
                            DataSet Eduresult = objDB.Select_CLUSTER_NAME(block);
                            ViewBag.MyEduCluster = Eduresult.Tables[0];
                            //List<SelectListItem> EduClusterList = new List<SelectListItem>();
                            // EduClusterList.Add(new SelectListItem { Text = "---Edu Cluster---", Value = "0" });
                            foreach (System.Data.DataRow dr in ViewBag.MyEduCluster.Rows)
                            {
                                EduClusterList.Add(new SelectListItem { Text = @dr["CLUSTER_NAME"].ToString(), Value = @dr["CLUSTER_NAME"].ToString() });
                            }
                            ViewBag.MyEduCluster = EduClusterList;

                            obj.EduCluster = dt.Rows[0]["EDUCLUSTER"].ToString();
                            obj.SchlType = dt.Rows[0]["SCHLTYPE"].ToString();
                            obj.SchlEstd = dt.Rows[0]["SchlEstd"].ToString();
                            obj.Bank = dt.Rows[0]["BANKAC"].ToString();
                            obj.IFSC = dt.Rows[0]["IFSC"].ToString();
                            obj.geoloc = dt.Rows[0]["GEOLOC"].ToString();
                            //-----------------------

                            ViewBag.btn = "VIEW";
                            return View(obj);
                            //TempData["msg"] = "Record Deleted";
                            //return RedirectToAction("Display", "CallCenter");

                        }
                    }

                }
                return View();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        #region School_Staff_Details       

        public ActionResult School_Staff_Details(int? id)
        {
            string SCHLDIST = string.Empty;
            try
            {

                if (Session["SCHOOLDIST"] == null)
                {
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    SCHLDIST = Session["SCHOOLDIST"].ToString();
                }
                AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                ViewBag.cadre = objCommon.GetCadre();
                // ViewBag.district = objCommon.GetDistED();
                DataSet ds = objCommon.Fll_Dist_Details();
                District objDis = new District();// create the object of class Employee 
                List<District> disList = new List<District>();
                int table = Convert.ToInt32(ds.Tables.Count);// count the number of table in dataset
                for (int i = 0; i < table; i++)// set the table value in list one by one
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        disList.Add(new District { DIST = Convert.ToInt32(dr["DIST"]), DISTNM = Convert.ToString(dr["DISTNM"]) });
                    }
                    disList.Add(new District { DIST = Convert.ToInt32(0), DISTNM = Convert.ToString("Others") });
                }
                ViewBag.district1 = disList.ToList();

                @ViewBag.DA = objCommon.GetDA();

                int dist = Convert.ToInt32(Session["SCHOOLDIST"].ToString());
                DataSet result1 = objDB.SelectAllTehsil(dist);
                ViewBag.MyTeh = result1.Tables[0];
                List<SelectListItem> TehList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyTeh.Rows)
                {

                    TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                }
                ViewBag.MyTeh = TehList;

                string mydist = Session["SCHOOLDIST"].ToString();
                DataSet result = objDB.SelectBlock(mydist);
                ViewBag.MyEdublock = result.Tables[0];
                List<SelectListItem> BlockList = new List<SelectListItem>();
                //BlockList.Add(new SelectListItem { Text = "---Edu Block---", Value = "0" });
                foreach (System.Data.DataRow dr in ViewBag.MyEdublock.Rows)
                {
                    BlockList.Add(new SelectListItem { Text = @dr["Edu Block Name"].ToString(), Value = @dr["Edu Block Name"].ToString() });
                }
                ViewBag.MyEdublock = BlockList;


                List<SelectListItem> EduClusterList = new List<SelectListItem>();
                EduClusterList.Add(new SelectListItem { Text = "---Edu Cluster---", Value = "0" });
                ViewBag.MyEduCluster = EduClusterList;

                ViewBag.tehs = objCommon.TehsilMaster();
                string cmd = "";
                if (id != null)
                {
                    if (Convert.ToString(Request.QueryString["command"]) != null)
                    {
                        cmd = Request.QueryString["command"].ToString();

                        if (cmd == "modify")
                        {
                            //ViewBag.cadrename = objCommon.GetCadre();
                            int recid = Convert.ToInt32(id);
                            DataTable dt = objDB.GetEditSchoolStaffDetails(recid);
                            SchoolStaffDetailsModel obj = new SchoolStaffDetailsModel();
                            obj.id = Convert.ToInt32(dt.Rows[0]["staffid"]);
                            obj.schoolcode = Convert.ToString(dt.Rows[0]["schl"]);
                            obj.DistrictId = Convert.ToInt32(dt.Rows[0]["homedistcode"]);
                            ViewBag.img = Convert.ToString(dt.Rows[0]["PHOTO"]);
                            if (ViewBag.img != null || ViewBag.img != "")
                            {

                                obj.photo = ViewBag.img;

                            }
                            else
                            {
                                obj.photo = "StaffDetails" + "/" + SCHLDIST + "/Photo/" + Path.GetFileName(obj.file1.FileName);
                            }
                            obj.Name = Convert.ToString(dt.Rows[0]["name"]);
                            obj.FName = Convert.ToString(dt.Rows[0]["fname"]);
                            obj.DOB = Convert.ToString(dt.Rows[0]["DOB"]);
                            obj.Gender = Convert.ToString(dt.Rows[0]["gender"]);
                            obj.AadharNo = Convert.ToString(dt.Rows[0]["aadharno"]);
                            obj.MobileNo = Convert.ToString(dt.Rows[0]["MOBILENO"]);
                            obj.stdCode = Convert.ToString(dt.Rows[0]["STDCODE"]);
                            obj.PhoneNo = Convert.ToString(dt.Rows[0]["PHNO"]);
                            obj.Email = Convert.ToString(dt.Rows[0]["EMAIL"]);
                            obj.appointmentDate = Convert.ToString(dt.Rows[0]["DOA"]);
                            obj.joiningDate = Convert.ToString(dt.Rows[0]["DOJ"]);
                            //obj.Cadreid = Convert.ToString(dt.Rows[0]["cadre"]);
                            //obj.Subjectid = Convert.ToString(dt.Rows[0]["SUBNM"]);
                            ViewBag.subjecttid = Convert.ToString(dt.Rows[0]["SUBNM"]);
                            ViewBag.caderid = Convert.ToString(dt.Rows[0]["cadre"]);
                            //ViewBag.cadre = Convert.ToString(dt.Rows[0]["SUBNM"]);
                            // string subjectname = objCommon.GetSubjectName(ViewBag.subjecttid);
                            ViewBag.subjectname = ViewBag.subjecttid;
                            obj.HouseFlatNo = Convert.ToString(dt.Rows[0]["homeno"]);
                            obj.VillWardCity = Convert.ToString(dt.Rows[0]["homecity"]);
                            obj.LandMark = Convert.ToString(dt.Rows[0]["homelandmark"]);
                            obj.DistanceFromSchool = Convert.ToString(dt.Rows[0]["Distance"]);
                            obj.PinCode = Convert.ToString(dt.Rows[0]["pincode"]);
                            obj.otherdistrict = Convert.ToString(dt.Rows[0]["homedist"]);
                            obj.otherstate = Convert.ToString(dt.Rows[0]["otherstate"]);
                            obj.State = Convert.ToString(dt.Rows[0]["homestate"]);
                            ViewBag.distchk = Convert.ToString(dt.Rows[0]["homedistcode"]);
                            ViewBag.statechk = Convert.ToString(dt.Rows[0]["homestate"]);

                            //-----------------------
                            obj.Quali = Convert.ToString(dt.Rows[0]["Quali"]);
                            obj.Phychall = dt.Rows[0]["Phychal"].ToString();
                            obj.tehsil = dt.Rows[0]["tehsil"].ToString();
                            obj.Edublock = dt.Rows[0]["EDUBLOCK"].ToString();

                            obj.ExpMonth = dt.Rows[0]["ExpMonth"].ToString();
                            obj.ExpYear = dt.Rows[0]["ExpYear"].ToString();

                            string block = dt.Rows[0]["EDUBLOCK"].ToString();
                            DataSet Eduresult = objDB.Select_CLUSTER_NAME(block);
                            ViewBag.MyEduCluster = Eduresult.Tables[0];
                            //List<SelectListItem> EduClusterList = new List<SelectListItem>();
                            // EduClusterList.Add(new SelectListItem { Text = "---Edu Cluster---", Value = "0" });
                            foreach (System.Data.DataRow dr in ViewBag.MyEduCluster.Rows)
                            {
                                EduClusterList.Add(new SelectListItem { Text = @dr["CLUSTER_NAME"].ToString(), Value = @dr["CLUSTER_NAME"].ToString() });
                            }
                            ViewBag.MyEduCluster = EduClusterList;

                            obj.EduCluster = dt.Rows[0]["EDUCLUSTER"].ToString();
                            obj.SchlType = dt.Rows[0]["SCHLTYPE"].ToString();
                            obj.SchlEstd = dt.Rows[0]["SchlEstd"].ToString();
                            obj.Bank = dt.Rows[0]["BANKAC"].ToString();
                            obj.IFSC = dt.Rows[0]["IFSC"].ToString();
                            obj.geoloc = dt.Rows[0]["GEOLOC"].ToString();
                            //-----------------------

                            ViewBag.btn = "Update";
                            return View(obj);
                            //TempData["msg"] = "Record Deleted";
                            //return RedirectToAction("Display", "CallCenter");

                        }
                    }

                }
                return View();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        [HttpPost]
        public ActionResult School_Staff_Details(SchoolStaffDetailsModel obj, IEnumerable<HttpPostedFileBase> files, string cmd, int? id)
        {
            @ViewBag.DA = objCommon.GetDA();
            string SCHLDIST = "";
            if (Session["SCHOOLDIST"] == null)
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                SCHLDIST = Session["SCHOOLDIST"].ToString();
            }
            try
            {
                AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                ViewBag.cadre = objCommon.GetCadre();
                DataSet ds = objCommon.Fll_Dist_Details();
                District objDis = new District();// create the object of class Employee 
                List<District> disList = new List<District>();
                int table = Convert.ToInt32(ds.Tables.Count);// count the number of table in dataset
                for (int i = 0; i < table; i++)// set the table value in list one by one
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        disList.Add(new District { DIST = Convert.ToInt32(dr["DIST"]), DISTNM = Convert.ToString(dr["DISTNM"]) });
                    }
                    disList.Add(new District { DIST = Convert.ToInt32(0), DISTNM = Convert.ToString("Others") });
                }
                ViewBag.district1 = disList.ToList();
                int dist1 = Convert.ToInt32(Session["SCHOOLDIST"].ToString());
                DataSet result1 = objDB.SelectAllTehsil(dist1);
                ViewBag.MyTeh = result1.Tables[0];
                List<SelectListItem> TehList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyTeh.Rows)
                {

                    TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                }
                ViewBag.MyTeh = TehList;

                string mydist = Session["SCHOOLDIST"].ToString();
                DataSet result = objDB.SelectBlock(mydist);
                ViewBag.MyEdublock = result.Tables[0];
                List<SelectListItem> BlockList = new List<SelectListItem>();
                //BlockList.Add(new SelectListItem { Text = "---Edu Block---", Value = "0" });
                foreach (System.Data.DataRow dr in ViewBag.MyEdublock.Rows)
                {
                    BlockList.Add(new SelectListItem { Text = @dr["Edu Block Name"].ToString(), Value = @dr["Edu Block Name"].ToString() });
                }
                ViewBag.MyEdublock = BlockList;


                List<SelectListItem> EduClusterList = new List<SelectListItem>();
                EduClusterList.Add(new SelectListItem { Text = "---Edu Cluster---", Value = "0" });
                ViewBag.MyEduCluster = EduClusterList;

                ViewBag.tehs = objCommon.TehsilMaster();
                string outputstatus = "";
                string getid = "";

                //if (ModelState.IsValid)
                //{
                //obj.schoolcode = "1111112";
                if (obj.DistrictId != 0)
                    obj.otherdistrict = "";
                if (obj.State != "Other")
                    obj.otherstate = "";
                obj.schoolcode = Convert.ToString(Session["SCHL"]);
                if (id != null)
                {

                }
                else
                {
                    obj.photo = "";
                }
                string dist = Convert.ToString(obj.DistrictId);
                obj.Cadreid = obj.cadrename;
                obj.Subjectid = obj.subjectname;

                obj.ExpMonth = obj.ExpMonth;
                obj.ExpYear = obj.ExpYear;

                if (obj.file1 != null)
                {
                    //stdPic = Path.GetFileName(frm["file"]);
                    obj.photo = "allfiles/StaffDetail/" + obj.AadharNo + "P" + ".jpg"; //Path.GetFileName(rm.file.FileName);

                }

                objDB.pro_insertupdateschool_staff_details(obj, out outputstatus, out getid);
                if (outputstatus == "1")
                {
                    TempData["msg"] = "Updated Successfully.";
                    ViewData["msg"] = '1';
                }
                else if (outputstatus == "2")
                {
                    TempData["msg"] = "Inserted Successfully.";
                    ViewData["msg"] = '2';
                }
                else if (outputstatus == "3")
                {
                    TempData["msg"] = "Duplicate Aadhar Number";
                    ViewData["msg"] = '3';
                }
                int flag = 0;
                HttpPostedFileBase file = null;
                if (id != null)
                {
                    if (obj.file1 != null)
                    {
                        file = obj.file1;
                        flag = 1;

                    }
                    else
                    {
                        //flag = ViewBag.img;
                    }
                    //foreach (var file1 in files)
                    //{

                    //    if (file1 != null)
                    //    {
                    //        file = file1;
                    //        flag = 1;

                    //    }
                    //}
                }
                else
                {
                    file = obj.file1;
                    flag = 1;
                }
                //var path = Path.Combine(Server.MapPath("~/Upload/"+ formName + "/" + dist + "/Photo"), stdPic);
                if (flag == 1 && obj.file1 != null)
                {
                    string Orgfile = obj.AadharNo + "P" + ".jpg";
                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            ///file.CopyTo(newMemoryStream);

                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = obj.file1.InputStream,
                                Key = string.Format("allfiles/StaffDetail/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }



                    string PhotoName = "allfiles/StaffDetail/" + obj.AadharNo + "P" + ".jpg";
                    string UpdatePic = objDB.Updated_Pic_Data(getid, PhotoName);
                }
                ModelState.Clear();
                //return RedirectToAction("DisplaySchoolStaffDetails", "SchoolProfile");

            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }

            // }
            return View();
        }
        #endregion School_Staff_Details

        public ActionResult DisplaySchoolStaffDetails(int? page, int? id)
        {
            try
            {
                AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                string user = "";
                string cmd = "";
                if (id != null && Convert.ToString(Request.QueryString["command"]) != null)
                {
                    cmd = Request.QueryString["command"].ToString();
                    if (cmd == "delete")
                    {
                        int recid = Convert.ToInt32(id);
                        string res = objDB.deleteRecordSchoolStaffDetails(recid);
                        if (res == "1")
                        {
                            TempData["Dmsg"] = "1";
                            return RedirectToAction("DisplaySchoolStaffDetails", "SchoolProfile");
                        }
                    }
                    //if (cmd == "Releaved")
                    //{
                    //    int recid = Convert.ToInt32(id);
                    //    string res = objDB.ReleavedRecordSchoolStaffDetails(recid);
                    //    if (res == "1")
                    //    {
                    //        TempData["Rmsg"] = "1";
                    //        return RedirectToAction("DisplaySchoolStaffDetails", "SchoolProfile");
                    //    }
                    //}

                    return View();
                }
                else
                {
                    SchoolStaffDetailsModel rm = new SchoolStaffDetailsModel();
                    string Search = string.Empty;
                    string schcode = Convert.ToString(Session["SCHL"]);
                    //string schcode = "1111112";
                    ViewBag.SCHL = Convert.ToString(Session["SCHL"]);
                    Search = "a.schl = '" + schcode + "' ";
                    rm.StoreAllData = objDB.GetStaffRecordsSearch(Search, pageIndex);
                    rm.TotalCount = objDB.GetStaffRecordsCount(Search);
                    // asm.StoreAllData = objDB.SearchSchoolDetails(Search);
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                        int tp = Convert.ToInt32(rm.TotalCount.Tables[0].Rows[0]["decount"]);

                        int pn = tp / 50;
                        ViewBag.pn = pn;
                        ViewBag.FSR = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[0]["FSR"]);
                        return View(rm);
                    }

                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        [HttpPost]
        public ActionResult DisplaySchoolStaffDetails(int? page, int? id, string cmd)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            SchoolStaffDetailsModel rm = new SchoolStaffDetailsModel();

            try
            {
                if (cmd == "Final Submit")
                {
                    rm.StoreAllData = objDB.GetStaffFinalSubmit(Session["SCHL"].ToString());
                    if (rm.StoreAllData.Tables[0].Rows[0]["res"].ToString() == "1")
                    {
                        ViewData["msg"] = "1";
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.msg = "0";
                    }
                }


            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
            return View(rm);
        }

        public ActionResult SchoolStaffDetailsPrint(string id, SchoolStaffDetailsModel rm)
        {
            try
            {
                if (id == null)
                { return RedirectToAction("Login", "Login"); }
                if (Session["SCHL"] == null)
                { return RedirectToAction("Login", "Login"); }
                else if (id != Session["SCHL"].ToString())
                { return RedirectToAction("Login", "Login"); }
                else
                {

                    AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                    string Search = string.Empty;
                    string schcode = Convert.ToString(Session["SCHL"]);
                    //string schcode = "1111112";
                    ViewBag.SCHL = Convert.ToString(Session["SCHL"]);
                    Search = "a.schl = '" + schcode + "' ";
                    rm.StoreAllData = objDB.GetStaffRecordsSearch(Search, 0);
                    rm.TotalCount = objDB.GetStaffRecordsCount(Search);
                    // asm.StoreAllData = objDB.SearchSchoolDetails(Search);
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                    }
                    return View(rm);
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        public ActionResult IMPORT_SCHL_STAFF(int? page, int? id)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            SchoolStaffDetailsModel rm = new SchoolStaffDetailsModel();
            try
            {

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                string user = "";
                string cmd = "";





                //string Search = string.Empty;
                //string schcode = Convert.ToString(Session["SCHL"]);
                ////string schcode = "1111112";
                //ViewBag.SCHL = Convert.ToString(Session["SCHL"]);
                //Search = "a.schl = '" + schcode + "' ";

                //rm.StoreAllData = objDB.GetStaffRecordsSearch(Search, pageIndex);
                //if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                //{
                //    ViewBag.Message = "Record Not Found";
                //    ViewBag.TotalCount = 0;
                //    return View();
                //}
                //else
                //{
                //    ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                //    int tp = Convert.ToInt32(rm.TotalCount.Tables[0].Rows[0]["decount"]);
                //    int pn = tp / 50;
                //    ViewBag.pn = pn;

                //    return View(rm);
                //}
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
            return View(rm);
        }
        [HttpPost]
        public ActionResult IMPORT_SCHL_STAFF(int? page, int? id, string cmd, string Category, string SearchString, FormCollection frc)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            SchoolStaffDetailsModel rm = new SchoolStaffDetailsModel();
            string Search = string.Empty;
            try
            {

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                if (cmd == "Search")
                {
                    ViewBag.Searchstring = SearchString;
                    if (Category != "")
                    {
                        ViewBag.SelectedItem = Category;
                        int SelValueSch = Convert.ToInt32(Category.ToString());

                        if (SelValueSch == 1)
                        { Search += " a.aadharno='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueSch == 2)
                        { Search += " a.MOBILENO ='" + SearchString.ToString().Trim() + "'"; }
                    }

                    rm.StoreAllData = objDB.GetStaffRecordsToImportSearch(Search, pageIndex);
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                        int tp = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int pn = tp / 50;
                        ViewBag.pn = pn;

                        return View(rm);
                    }
                }
                if (cmd == "IMPORT SELECT STAFF")
                {
                    string catg = frc["Category"];
                    string stext = frc["SearchString"];
                    string ChkStaff = frc["StaffName"];
                    string schcode = Convert.ToString(Session["SCHL"]);

                    string StaffClusterresult = objDB.ImportStaffToSCHOOL(schcode, ChkStaff);
                    if (StaffClusterresult == "0")
                    {
                        //--------------Not Updated
                        // ViewData["result"] = 0;
                        TempData["Impresult"] = 0;
                    }
                    else
                    {
                        TempData["Impresult"] = "1";
                        return RedirectToAction("DisplaySchoolStaffDetails", "SchoolProfile");
                    }
                    //rm.StoreAllData = objDB.GetStaffRecordsToImportSearch(Search, pageIndex);
                    //if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    //{
                    //    ViewBag.Message = "Record Not Found";
                    //    ViewBag.TotalCount = 0;
                    //    return View();
                    //}
                    //else
                    //{
                    //    ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                    //    int tp = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[0]["decount"]);
                    //    int pn = tp / 50;
                    //    ViewBag.pn = pn;

                    //    return View(rm);
                    //}
                }

            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
            return View(rm);
        }

        public JsonResult CreateComments(string Reason, string comment, string STAFFID)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();

            try
            {

                string dee = "";
                string outstatus = "";
                string Search = string.Empty;
                //string res = OBJDB.CreateClusterNew(Reason, comment);               
                string res = objDB.ReleavedRecordSchoolStaffDetails(STAFFID, Reason, comment);
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
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return null;
            }
        }


        #region School Premises Information



        public ActionResult SchoolPremisesInformation(string id, string mod, SchoolPremisesInformation sm)
        {
            try
            {
                string SCHL = string.Empty;

                if (mod == null)
                {
                    if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                    { return RedirectToAction("Index", "Home"); }
                    else
                    {
                        SCHL = Session["SCHL"].ToString();
                    }
                }
                else
                {
                    SCHL = id;
                }
                ViewBag.mod = Session["premisesMOD"] = mod;

                // Area 
                // string dist = Session["SCHOOLDIST"].ToString();
                // YesNo 
                ViewBag.YesNoList = objCommon.GetYesNoText();
                // Status
                ViewBag.PropertyStatusList = objCommon.GetPropertyStatus();
                DataSet ds = new DataSet();
                sm = objDB.SchoolPremisesInformationBySchl(SCHL, out ds);
                sm.SCHL = SCHL;
                ViewBag.SCHLCODE = SCHL;
                ViewBag.IdNo = ds.Tables[0].Rows[0]["IDNO"].ToString();
                ViewBag.SCHLE = ds.Tables[0].Rows[0]["SCHLEfull"].ToString();
                ViewBag.dist = ds.Tables[0].Rows[0]["DIST"].ToString();
                ViewBag.DISTNM = ds.Tables[0].Rows[0]["DISTNM"].ToString();
                ViewBag.SCHLP = ds.Tables[0].Rows[0]["SCHLPfull"].ToString();
                sm.UDISECODE = ds.Tables[0].Rows[0]["UDISECODE"].ToString();




                if (sm.ID == 0 || sm == null)
                {
                    ViewBag.SID = 0;
                    ViewData["result"] = 2;
                    ViewBag.IsFinalSubmit = 0;
                    ViewBag.ChallanId = 0;
                    ViewBag.IsVerified = 0;

                }
                else
                {
                    ViewBag.ChallanId = sm.ChallanId;
                    ViewBag.IsVerified = sm.challanVerify;

                    ViewBag.SID = 1;
                    //ViewBag.PropertyFloorList = MultiPropertyFloor(sm.ECD17.ToString());
                    ViewBag.IsFinalSubmit = sm.IsFinalSubmit;
                    ViewBag.ExpireVDate = 0;
                    if (sm.IsFinalSubmit == 0)
                    {
                        string today = DateTime.Today.ToString("dd/MM/yyyy");
                        DateTime dateselected;
                        AffiliationFee _affiliationFee1 = new AffiliationFee();
                        if (DateTime.TryParseExact(today, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
                        {
                            // 10 ( for school infrastructure)
                            _affiliationFee1 = new AbstractLayer.AffiliationDB().AffiliationFee(10, sm.SCHL, dateselected);
                            if (_affiliationFee1.FEECODE > 0)
                            {
                                ViewBag.IsFeeDateOver = 1;
                                ViewBag.totfee = _affiliationFee1.totfee;
                                ViewBag.eDate = _affiliationFee1.eDate;
                                ViewBag.BankLastdate = _affiliationFee1.BankLastdate;
                                ViewBag.AllowBanks = ViewBag.AllowBanksCode = _affiliationFee1.AllowBanks;
                                string[] bls = _affiliationFee1.AllowBanks.Split(',');
                                BankModels BM = new BankModels();
                                List<BankListModel> blm = new List<BankListModel>();
                                string BANKNAME = "";
                                for (int b = 0; b < bls.Count(); b++)
                                {
                                    int OutStatus;
                                    BM.BCODE = bls[b].ToString();
                                    DataSet ds1 = new AbstractLayer.BankDB().GetBankDataByBCODE(BM, out OutStatus);
                                    BANKNAME += ds1.Tables[0].Rows[0]["BANKNAME"].ToString();
                                }
                                ViewBag.AllowBanks = BANKNAME;

                            }
                            else
                            {
                                ViewBag.IsFeeDateOver = 0;

                            }

                        }
                    }
                    else
                    {
                        if (ds.Tables.Count > 1)
                        {
                            if (ds.Tables[1].Rows.Count > 0)
                            {
                                ViewBag.ExpireVDate = ds.Tables[1].Rows[0]["ExpireVDate"].ToString();
                                ViewBag.FeeDepositStatus = ds.Tables[1].Rows[0]["FeeDepositStatus"].ToString();
                            }
                        }

                        if (ds.Tables.Count > 2)
                        {
                            if (ds.Tables[2].Rows.Count > 0)
                            {
                                ViewBag.AllowBanks = ViewBag.AllowBanksCode = ds.Tables[2].Rows[0]["AllowBanks"].ToString();
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(sm.ECD17))
                    {
                        List<SelectListItemCheckBox> _floorlist = new List<SelectListItemCheckBox>();
                        foreach (SelectListItem dr in objCommon.GetPropertyFloor())
                        {
                            if (AbstractLayer.StaticDB.ContainsValue(sm.ECD17, dr.Value))
                            {
                                _floorlist.Add(new SelectListItemCheckBox { Text = dr.Text.ToString(), Value = dr.Value.ToString(), Selected = true });
                            }
                            else
                            { _floorlist.Add(new SelectListItemCheckBox { Text = dr.Text.ToString(), Value = dr.Value.ToString() }); }
                        }
                        ViewBag.PropertyFloorList = sm.PropertyFloorList = _floorlist;

                    }
                }

                if (sm.PropertyFloorList == null)
                {
                    List<SelectListItemCheckBox> _floorlist = new List<SelectListItemCheckBox>();
                    foreach (SelectListItem dr in objCommon.GetPropertyFloor())
                    {
                        _floorlist.Add(new SelectListItemCheckBox { Text = dr.Text.ToString(), Value = dr.Value.ToString() });
                    }
                    ViewBag.PropertyFloorList = sm.PropertyFloorList = _floorlist;
                }

                return View(sm);
            }
            catch (Exception ex)
            {
                return View(sm);
            }
        }


        [HttpPost]
        public ActionResult SchoolPremisesInformation(string id, string mod, SchoolPremisesInformation sm, FormCollection fc, string submit)
        {

            try
            {
                string SCHL = string.Empty;
                if (Session["premisesMOD"] == null)
                {

                    if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                    { return RedirectToAction("Index", "Home"); }
                    else
                    {
                        SCHL = Session["SCHL"].ToString();
                    }
                }
                else
                {
                    SCHL = id;
                }

                ViewBag.mod = Session["premisesMOD"];


                ViewBag.SCHLCODE = SCHL;
                // Area 
                //string dist = Session["SCHOOLDIST"].ToString();

                // YesNo 
                ViewBag.YesNoList = objCommon.GetYesNoText();
                // Status
                ViewBag.PropertyStatusList = objCommon.GetPropertyStatus();
                // Session 
                sm.ECD17 = String.Join(", ", sm.PropertyFloorList.Where(s => s.Selected == true).Select(s => s.Value));
                ViewBag.ChallanId = sm.ChallanId;
                ViewBag.IsVerified = sm.challanVerify;
                ViewBag.ChallanDt = sm.ChallanDt;
                if (submit != null)
                {
                    if (submit.ToLower().Contains("save"))
                    {

                        string outError = "0";
                        int result = objDB.SchoolPremisesInformation(sm, out outError);// if ID=0 then Insert else Update
                        if (result > 0)
                        {
                            ViewBag.SID = sm.ID;
                            ViewBag.result = result;
                            ViewData["result"] = 1;
                        }
                        else
                        {
                            ViewBag.SID = sm.ID;
                            ViewData["result"] = 0;
                            ViewBag.Message = outError.ToString();
                        }

                    }
                    else if (submit.ToLower().Contains("final"))
                    {
                        if (sm.ID > 0)
                        {

                            if (string.IsNullOrEmpty(sm.SSD1) || string.IsNullOrEmpty(sm.SSD2) || sm.SSD3 == 0 || sm.CB5 == 0
                                || sm.CB6 == 0 || sm.CB7 == 0 || sm.CB8 == 0 || sm.CB9 == 0 || string.IsNullOrEmpty(sm.CB16) || string.IsNullOrEmpty(sm.ECD25)
                                || string.IsNullOrEmpty(sm.PG51) || string.IsNullOrEmpty(sm.PG53) || string.IsNullOrEmpty(sm.LIB56) || string.IsNullOrEmpty(sm.CLAB71)
                                || string.IsNullOrEmpty(sm.OTH72) || string.IsNullOrEmpty(sm.OTH73) || string.IsNullOrEmpty(sm.OTH74) || string.IsNullOrEmpty(sm.OTH75)
                                || string.IsNullOrEmpty(sm.OTH76) || string.IsNullOrEmpty(sm.OTH77) || string.IsNullOrEmpty(sm.OTH78) || string.IsNullOrEmpty(sm.OTH79)
                                || string.IsNullOrEmpty(sm.OTH80) || string.IsNullOrEmpty(sm.OTH81) || string.IsNullOrEmpty(sm.OTH82) || string.IsNullOrEmpty(sm.OTH83) || string.IsNullOrEmpty(sm.UDISECODE))
                            {
                                ViewBag.SID = sm.ID;
                                ViewData["result"] = 5;
                            }
                            else
                            {

                                string outError = "0";
                                int result = objDB.SchoolPremisesInformation(sm, out outError);// if ID=0 then Insert else Update
                                if (result > 0)
                                {
                                    ViewBag.SID = sm.ID;
                                    ViewBag.result = result;
                                    ViewData["result"] = 1;

                                    #region Challan Generate

                                    string today = DateTime.Today.ToString("dd/MM/yyyy");
                                    DateTime dateselected;
                                    AffiliationFee _affiliationFee1 = new AffiliationFee();
                                    if (DateTime.TryParseExact(today, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
                                    {
                                        // 10 ( for school infrastructure)
                                        _affiliationFee1 = new AbstractLayer.AffiliationDB().AffiliationFee(10, sm.SCHL, dateselected);
                                        ViewBag.totfee = _affiliationFee1.totfee;
                                        if (_affiliationFee1.totfee == 0)
                                        {
                                            ViewData["result"] = 25;
                                            return View(sm);
                                        }
                                        _affiliationFee1.BankCode = _affiliationFee1.AllowBanks;
                                        //  _affiliationFee1.BankCode = "205";
                                        string BankCode = _affiliationFee1.BankCode;
                                        ChallanMasterModel CM = new ChallanMasterModel();
                                        if (sm.ID > 0)
                                        {
                                            CM.APPNO = sm.ID.ToString();
                                            CM.FeeStudentList = sm.SCHL;
                                            CM.SCHLREGID = sm.SCHL.ToString();
                                            CM.SchoolCode = sm.SCHL.ToString();
                                            CM.addfee = 0; // AdmissionFee / ADDFEE
                                            CM.latefee = _affiliationFee1.latefee;
                                            CM.prosfee = 0;
                                            CM.addsubfee = 0;
                                            CM.add_sub_count = 0;
                                            CM.regfee = 0;
                                            CM.FEE = _affiliationFee1.fee;
                                            CM.TOTFEE = _affiliationFee1.totfee;
                                            CM.FEECAT = _affiliationFee1.FEECAT;
                                            CM.FEECODE = _affiliationFee1.FEECODE.ToString();
                                            CM.FEEMODE = "CASH";
                                            CM.BANK = "";
                                            CM.BCODE = _affiliationFee1.BankCode;
                                            CM.BANKCHRG = Convert.ToInt32(0);
                                            CM.DIST = "";
                                            CM.DISTNM = "";
                                            CM.LOT = 0;
                                            CM.DepositoryMobile = "CASH";
                                            CM.type = "schle";
                                            DateTime CHLNVDATE2;
                                            if (DateTime.TryParseExact(_affiliationFee1.BankLastdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out CHLNVDATE2))
                                            {
                                                CM.ChallanVDateN = CHLNVDATE2;
                                            }
                                            CM.CHLNVDATE = _affiliationFee1.BankLastdate;
                                            CM.LumsumFine = Convert.ToInt32(0);
                                            CM.LSFRemarks = "";
                                            string SchoolMobile = "";

                                            string result1 = new AbstractLayer.HomeDB().InsertPaymentForm(CM, fc, out SchoolMobile);
                                            if (result1 == "0" || result1 == "")
                                            {
                                                //--------------Not saved
                                                ViewData["result"] = 0;
                                                return View(sm);
                                            }
                                            if (result1 == "-1")
                                            {
                                                //-----alredy exist
                                                ViewData["result"] = -1;
                                                return View(sm);
                                            }
                                            else
                                            {
                                                ViewData["result"] = 1;
                                                ViewBag.ChallanNo = result1;
                                                string OutErrorFinal = "";
                                                int OutStatusFinal = 0;
                                                ; string dee = objDB.SchoolPremisesInformationFinalSubmit(SCHL, out OutStatusFinal, out OutErrorFinal);
                                                if (OutErrorFinal == "1")
                                                {
                                                    ViewData["result"] = 10;
                                                    string Sms = "Your Challan no. " + ViewBag.ChallanNo + " of School Infrastructure Performa is successfully generated and valid till Dt " + CM.CHLNVDATE + ". Regards PSEB";
                                                    try
                                                    {
                                                        // string getSms = new AbstractLayer.DBClass().gosms(SchoolMobile, Sms);
                                                    }
                                                    catch (Exception) { }

                                                }
                                                return RedirectToAction("GenerateChallaan", "Home", new { ChallanId = ViewBag.ChallanNo });

                                            }
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    ViewBag.SID = sm.ID;
                                    ViewData["result"] = 0;
                                    ViewBag.Message = outError.ToString();
                                }
                            }
                        }

                    }
                }
                List<SelectListItemCheckBox> _floorlist = new List<SelectListItemCheckBox>();
                foreach (SelectListItem dr in objCommon.GetPropertyFloor())
                {
                    _floorlist.Add(new SelectListItemCheckBox { Text = dr.Text.ToString(), Value = dr.Value.ToString() });
                }
                ViewBag.PropertyFloorList = sm.PropertyFloorList = _floorlist;
                return View(sm);

            }
            catch (Exception ex)
            {
                return View();
            }
        }


        [HttpPost]
        public JsonResult JqSchoolPremisesInformationFinalSubmit(string SCHL)
        {
            var flag = 1;
            string OutError = string.Empty;
            if (flag == 1 && SCHL != "")
            {
                DataSet ds = new DataSet();
                SchoolPremisesInformation sm = objDB.SchoolPremisesInformationBySchl(SCHL, out ds);
                if (sm.ID > 0)
                {

                    if (string.IsNullOrEmpty(sm.SSD1) || string.IsNullOrEmpty(sm.SSD2) || sm.SSD3 == 0 || sm.CB5 == 0
                        || sm.CB6 == 0 || sm.CB7 == 0 || sm.CB8 == 0 || sm.CB9 == 0 || string.IsNullOrEmpty(sm.CB16) || string.IsNullOrEmpty(sm.ECD25)
                        || string.IsNullOrEmpty(sm.PG51) || string.IsNullOrEmpty(sm.PG53) || string.IsNullOrEmpty(sm.LIB56) || string.IsNullOrEmpty(sm.CLAB71)
                        || string.IsNullOrEmpty(sm.OTH72) || string.IsNullOrEmpty(sm.OTH73) || string.IsNullOrEmpty(sm.OTH74) || string.IsNullOrEmpty(sm.OTH75)
                        || string.IsNullOrEmpty(sm.OTH76) || string.IsNullOrEmpty(sm.OTH77) || string.IsNullOrEmpty(sm.OTH78) || string.IsNullOrEmpty(sm.OTH79)
                        || string.IsNullOrEmpty(sm.OTH80) || string.IsNullOrEmpty(sm.OTH81) || string.IsNullOrEmpty(sm.OTH82) || string.IsNullOrEmpty(sm.OTH83) || string.IsNullOrEmpty(sm.UDISECODE))
                    {

                        OutError = "-2";
                        var results = new
                        {
                            status = OutError
                        };
                        return Json(results);
                    }
                    else
                    {


                        string dee = "0";
                        int OutStatus = 0;

                        dee = objDB.SchoolPremisesInformationFinalSubmit(SCHL, out OutStatus, out OutError);
                        var results = new
                        {
                            status = OutError
                        };
                        return Json(results);
                    }
                }
                else
                {
                    var results = new
                    {
                        status = 5
                    };
                    return Json(results);
                }
            }
            else
            {
                var results = new
                {
                    status = flag
                };
                return Json(results);
            }
        }


        public ActionResult SchoolPremisesInformationReport(string SCHL, string UI)
        {
            try
            {
                SchoolPremisesInformation sm = new Models.SchoolPremisesInformation();
                if (SCHL != null)
                {
                    ViewBag.UI = UI;
                    DataSet ds = new DataSet();
                    sm = objDB.SchoolPremisesInformationBySchl(SCHL, out ds);
                    sm.SCHL = SCHL;
                    ViewBag.SCHLCODE = SCHL;
                    ViewBag.IdNo = ds.Tables[0].Rows[0]["IDNO"].ToString();
                    ViewBag.SCHLE = ds.Tables[0].Rows[0]["SCHLEfull"].ToString();
                    ViewBag.dist = ds.Tables[0].Rows[0]["DIST"].ToString();
                    ViewBag.DISTNM = ds.Tables[0].Rows[0]["DISTNM"].ToString();
                    ViewBag.SCHLP = ds.Tables[0].Rows[0]["SCHLPfull"].ToString();
                    sm.UDISECODE = ds.Tables[0].Rows[0]["UDISECODE"].ToString();
                    if (sm.ID == 0 || sm == null)
                    {

                        ViewBag.TotalCount = 0;
                        ViewBag.IsFinalSubmit = 0;
                        ViewBag.ChallanId = 0;
                        ViewBag.IsVerified = 0;
                        ViewBag.ChallanDt = 0;
                    }
                    else
                    {
                        sm.StoreAllData = ds;
                        ViewBag.TotalCount = 1;
                        // ViewBag.PropertyFloorList = MultiPropertyFloor(sm.ECD17.ToString());
                        ViewBag.IsFinalSubmit = sm.IsFinalSubmit;
                        ViewBag.FinalSubmitOn = sm.FinalSubmitOn;
                        ViewBag.ChallanId = sm.ChallanId;
                        ViewBag.IsVerified = sm.challanVerify;
                        ViewBag.ChallanDt = sm.ChallanDt;
                    }
                }
                else { return RedirectToAction("Index", "Home"); }
                return View(sm);
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        #endregion


        public JsonResult jsCheckAadharDuplicate(string id)
        {
            DataSet ds = objCommon.jsCheckAadharDuplicate(id);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return Json(new { sn = ds.Tables[0].Rows[0]["DupData"].ToString() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { sn = "" }, JsonRequestBehavior.AllowGet);
            }

        }


    }

}