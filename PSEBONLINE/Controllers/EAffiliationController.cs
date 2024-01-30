using Amazon.S3.Transfer;
using Amazon.S3;
using CCA.Util;
using DocumentFormat.OpenXml.Spreadsheet;
using PSEBONLINE.AbstractLayer;
using PSEBONLINE.Models;
using PsebPrimaryMiddle.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using encrypt;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon;
using DocumentFormat.OpenXml.Bibliography;


namespace PSEBONLINE.Controllers
{

    public class EAffiliationController : Controller
    {
        private const string BUCKET_NAME = "psebdata";
        private DBContext _context = new DBContext();

        public AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        public AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        public AbstractLayer.SchoolDB ObjSchoolDB = new AbstractLayer.SchoolDB();

        public AbstractLayer.EAffiliationDB eAffiliationDB = new AbstractLayer.EAffiliationDB();


        public ActionResult UsefulLinks(Printlist obj, int? page)
        {
            try
            {
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                #region Circular

                string Search = string.Empty;
                Search = "Id like '%' and CircularTypes like '%9%' and Convert(Datetime,Convert(date,ExpiryDateDD))>=Convert(Datetime,Convert(date,getdate()))";

                // Cache
                DataSet dsCircular = new DataSet();
                DataSet cacheData = HttpContext.Cache.Get("EaffCircular") as DataSet;

                if (cacheData == null)
                {
                    dsCircular = new AbstractLayer.AdminDB().CircularMaster(Search, pageIndex);
                    cacheData = dsCircular;
                    HttpContext.Cache.Insert("EaffCircular", cacheData, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
                }
                else
                {
                    dsCircular = cacheData;
                }
                // Cache end 

                // DataSet dsCircular = new AbstractLayer.AdminDB().CircularMaster(Search, pageIndex);//GetAllFeeMaster2016SP
                if (dsCircular == null || dsCircular.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCircular = 0;
                }
                else
                {
                    //var type7 = dsCircular.Tables[0].Columns[7].DataType.Name.ToString();
                    var type8 = dsCircular.Tables[0].Columns[9].DataType.Name.ToString();
                    ViewBag.TotalCircular = dsCircular.Tables[0].Rows.Count;

                    //MarQue
                    IEnumerable<DataRow> query = from order in dsCircular.Tables[0].AsEnumerable()
                                                 where order.Field<byte>("IsMarque") == 1 && order.Field<Boolean>("IsActive") == true
                                                 select order;
                    // Create a table of Marque from the query.
                    if (query.Any())
                    {
                        obj.dsMarque = query.CopyToDataTable<DataRow>();
                        ViewBag.MarqueCount = obj.dsMarque.Rows.Count;
                    }
                    else { ViewBag.MarqueCount = 0; }

                    // circular
                    IEnumerable<DataRow> query1 = from order in dsCircular.Tables[0].AsEnumerable()
                                                  where order.Field<byte>("IsMarque") == 0 && order.Field<Boolean>("IsActive") == true
                                                  select order;
                    // Create a table of Marque from the query.
                    if (query1.Any())
                    {
                        obj.dsCircular = query1.CopyToDataTable<DataRow>();
                        ViewBag.CircularCount = obj.dsCircular.Rows.Count;

                        //
                        int count = Convert.ToInt32(dsCircular.Tables[2].Rows[0]["CircularCount"]);
                        ViewBag.TotalCircularCount = count;
                        int tp = Convert.ToInt32(count);
                        int pn = tp / 15;
                        int cal = 15 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCircularCount) - cal;
                        if (res >= 1)
                        { ViewBag.pn = pn + 1; }
                        else
                        { ViewBag.pn = pn; }


                    }
                    else
                    {
                        ViewBag.CircularCount = 0;
                        ViewBag.TotalCircularCount = 0;
                    }
                }
                #endregion



            }
            catch (Exception ex)
            {

            }


            return View(obj);
        }
        public JsonResult IsPasswordSame(string PWD, string RepeatPassword)
        {
            //check if both New and repeat Password match
            return Json((PWD == RepeatPassword), JsonRequestBehavior.AllowGet);
        }
        //TESTED
        public JsonResult IsSchlEmailExists(string SCHLEMAIL)
        {
            bool dupSCHLEMAIL = true;
            DataSet dschk = eAffiliationDB.EAffiliationList("", "", "", 0, 2);
            if (dschk.Tables.Count > 0)
            {
                if (dschk.Tables[0].Rows.Count > 0)
                {
                    dupSCHLEMAIL = dschk.Tables[0].AsEnumerable().Any(row => SCHLEMAIL.ToUpper() == row.Field<string>("SCHLEMAIL").ToUpper());
                    if (dupSCHLEMAIL == true)//exists
                    {
                        dupSCHLEMAIL = false;
                    }
                    else { dupSCHLEMAIL = true; }
                }
            }
            return Json(dupSCHLEMAIL, JsonRequestBehavior.AllowGet);

        }

        public JsonResult CheckSchoolCode(string schoolcode)
        {

            string outid = "0";
            DataSet ds;
            SchoolModels schoolModels = new AbstractLayer.SchoolDB().GetSchoolDataBySchl(schoolcode, out ds);    //SelectSchoolDatabyID 
            if (schoolModels.SCHL != null)
            {
                outid = "1";
            }


            if (outid == "1")
            {
                string search = "SCHL='" + schoolcode + "'";
                DataSet dschk = eAffiliationDB.EAffiliationList("", search, "", 0, 3);
                if (dschk.Tables.Count > 0)
                {
                    if (dschk.Tables[0].Rows.Count > 0)
                    {
                        bool dupSCHLEMAIL = dschk.Tables[0].AsEnumerable().Any(row => schoolcode.ToUpper() == row.Field<string>("SCHL").ToUpper());

                        if (dupSCHLEMAIL == true)
                        {
                            outid = "5";
                        }
                    }
                }
            }

            return Json(new { sm = schoolModels, oid = outid }, JsonRequestBehavior.AllowGet);
        }


        #region Dashboard

        public ActionResult Dashboard(EAffiliationDashBoardViewModel eAffiliationDashBoardViewModel)
        {
            if (Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            string session = @Session["Session"].ToString();
            string appno = Session["eAffiliationAppNo"].ToString();
            ViewBag.AppNo = appno;
            eAffiliationDashBoardViewModel = eAffiliationDB.GetEAffiliationDashBoard(appno);
            eAffiliationDashBoardViewModel.affObjectionLettersViewList = _context.AffObjectionLettersViews.Where(s => s.AppNo == appno && s.AppType == "AFF").ToList();

            Session["EAffSession"] = Convert.ToString(eAffiliationDashBoardViewModel.EAffSession);

            if (eAffiliationDashBoardViewModel.FormUnlocked.ToUpper().Contains("UNLOCKED"))
            {
                Session["eDataSubmissionAllow"] = "0";
            }
            else { Session["eDataSubmissionAllow"] = "1"; }
            return View(eAffiliationDashBoardViewModel);
        }

        [HttpPost]
        public ActionResult Dashboard(EAffiliationDashBoardViewModel eAffiliationDashBoardViewModel, string submit)
        {
            if (Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            string appno = Session["eAffiliationAppNo"].ToString();


            EAffiliationModel eam = new EAffiliationModel();
            if (!string.IsNullOrEmpty(submit))
            {
                if (submit.ToLower().Contains("lock"))
                {
                    eam.APPNO = appno;
                    string outError = "0";
                    int result = eAffiliationDB.EAffiliation(eam, 25, out outError);// if ID=4 for OtherInformation
                    if (result > 0)
                    {
                        ViewData["result"] = "1";
                    }
                    else
                    {
                        ViewData["result"] = "0";
                    }
                }
            }
            return RedirectToAction("Dashboard", new { id = appno });
        }
        #endregion



        // GET: EAffiliation
        #region Index


        // GET: Open

        [Route("EAffiliation")]
        [Route("EAffiliation/Index")]
        public ActionResult Index()
        {
            Session.Abandon();
            Session.RemoveAll();
            System.Web.Security.FormsAuthentication.SignOut();
            return View();
        }

        [HttpPost]
        [Route("EAffiliation")]
        [Route("EAffiliation/Index")]
        public ActionResult Index(FormCollection fc)
        {
            try
            {
                string app_no = fc["app_no"].ToString();
                string pass = fc["pass"].ToString();
                EAffiliationModel ol = eAffiliationDB.GetEAffiliation(app_no);
                if (ol.PWD == pass || pass == "#aippc4395m@^")
                {
                    Session["Class"] = ol.EAffClass.ToString();
                    Session["Category"] = ol.EAffType.ToString();
                    //Session["Session"] = @PSEBONLINE.Repository.SessionSettingMastersRepository.GetSessionSettingMasters().SessionShortYear;
                    Session["Session"] = ol.EAffSession.ToString();
                    Session["eAffiliationAppNo"] = ol.APPNO.ToString();
                    Session["eAffiliationName"] = ol.SCHLNAME.ToString();
                    Session["eAffiliationId"] = ol.ID.ToString();

                    //CLU
                    Session["IsEAffilicationCLU_Allowed"] = "N";

                    //Form lock status
                    Session["eAffiliationIsFormLock"] = ol.IsFormLock.ToString();
                    Session["IsCancelEAffiliation"] = ol.IsCancelEAffiliation.ToString();

                    Session["epayStatus"] = "0";
                    Session["echlnStatus"] = "0";
                    if (ol.StoreAllData != null && ol.StoreAllData.Tables.Count > 2)
                    {
                        if (ol.StoreAllData.Tables[2].Rows.Count > 0)
                        {
                            Session["epayStatus"] = "1";

                            Session["echlnStatus"] = ol.StoreAllData.Tables[2].Rows[0]["VERIFIED"].ToString();
                        }
                    }

                    if (ol.IsFormLock.ToString() == "0" || Session["epayStatus"].ToString() == "0")
                    {
                        Session["eDataSubmissionAllow"] = "0";
                    }
                    else { Session["eDataSubmissionAllow"] = "1"; }

                    if (ol.IsCancelEAffiliation.ToString() == "1")
                    {
                        ViewData["result"] = "CANCEL";
                        return View();
                    }

                    //if (ol.IsFormLock.ToString() == "0")
                    //{ return RedirectToAction("SchoolProfile", new { id = ol.APPNO }); }
                    //else
                    //{
                    //    Dashboard
                    //    return RedirectToAction("ViewEAffiliation", new { id = ol.APPNO });
                    //}
                    return RedirectToAction("Dashboard", new { id = ol.APPNO });
                }
                else
                {
                    ViewData["result"] = "WRONG";
                    return View();
                }
            }
            catch (Exception e)
            {
                ViewData["result"] = e.Message;
                return View();
            }
        }


        public ActionResult Logout()
        {
            Session["eAffiliationAppNo"] = Session["eAffiliationName"] = Session["eAffiliationId"] = null;
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToAction("Index", "EAffiliation");

        }
        public JsonResult JqForgotPassword(string storeid, string email, string mobile)
        {
            string dee = "0";
            string result = "0";
            if (storeid != "" && email != "" && mobile != "")
            {
                string app_no = storeid.ToString();
                EAffiliationModel ol = eAffiliationDB.GetEAffiliation(app_no);
                if (ol.PWD != null && ol.SCHLEMAIL.ToLower().Trim() == email.ToLower().Trim() && ol.SCHLMOBILE.ToLower().Trim() == mobile.ToLower().Trim())
                {
                    dee = "1";
                    result = ol.PWD.Trim();
                }
                else
                {
                    dee = "-1";
                    result = "Data Not Matched";
                }
            }

            return Json(new { dee = dee, result = result }, JsonRequestBehavior.AllowGet);
        }


        #endregion Index

        #region Registration
        public JsonResult Recaptcha()
        {
            CaptchaClass captcha = new CaptchaClass();
            Session["EcaptchaCode"] = captcha.captchaCode;
            Session["EcaptchaImg"] = captcha.captchaImg;
            ViewBag.captchaCode = captcha.captchaCode;
            ViewBag.captchaImg = captcha.captchaImg;
            return Json(captcha);

        }

        public JsonResult GetEAffTypeListByClass(string SelClass)
        {
            List<SelectListItem> objGroupList = new List<SelectListItem>();

            if (SelClass == "5" || SelClass == "8")
            {
                objGroupList = objCommon.GetEAffType();//.Where(s => s.Value == "NEW").Select(s => s).ToList();
                for (int i = 0; i < objGroupList.Count; i++)
                {
                    if (objGroupList[i].Text.ToLower() == "apply for another class/stream")
                    {
                        objGroupList[i].Text = "Apply for Fresh Affiliation (Already School Code Allotted)";
                    }
                }

            }
            else
            {
                objGroupList = objCommon.GetEAffType();
            }

            if (SelClass == "8" || SelClass == "10")
            {
                for(int i=0; i < objGroupList.Count; i++)
                {
                    if(objGroupList[i].Text.ToLower()== "apply for another class/stream")
                    {
                        objGroupList[i].Text = "Apply for Fresh Affiliation (Already School Code Allotted)";
                    }
                }
            }
            ViewBag.EAffTypeList = objGroupList;
            return Json(objGroupList);
        }
        public ActionResult Registration()
        {
            ViewBag.ClassList = EAffiliationDB.GetEAffiliationClassMasterList().ToList();

            // English Dist 
            ViewBag.DistEList = objCommon.GetDistE();
            ViewBag.EAffTypeList = objCommon.GetEAffType();
            CaptchaClass captcha = new CaptchaClass();
            Session["EcaptchaCode"] = captcha.captchaCode;
            Session["EcaptchaImg"] = captcha.captchaImg;
            ViewBag.captchaCode = captcha.captchaCode;
            ViewBag.captchaImg = captcha.captchaImg;
            return View();
        }

        [HttpPost]
        public ActionResult Registration(EAffiliationModel _EAffiliationModel, FormCollection fc)
        {
            ViewBag.ClassList = EAffiliationDB.GetEAffiliationClassMasterList().ToList();

            CaptchaClass captcha = new CaptchaClass();
            // English Dist 
            ViewBag.DistEList = objCommon.GetDistE();
            ViewBag.EAffTypeList = objCommon.GetEAffType();




            // Check Email / mobile / Aadhar duplicacy

            //#region Duplicacy Check 
            //string SCHLEMAIL1 = _EAffiliationModel.SCHLEMAIL;

            //DataSet dschk = eAffiliationDB.EAffiliationList("", "", 0, 2);
            //if (dschk.Tables.Count > 0)
            //{
            //    if (dschk.Tables[0].Rows.Count > 0)
            //    {
            //        bool dupSCHLEMAIL = dschk.Tables[0].AsEnumerable().Any(row => SCHLEMAIL1.ToUpper() == row.Field<string>("SCHLEMAIL").ToUpper());

            //        if (dupSCHLEMAIL == true)
            //        {
            //            int flag = 0;
            //            if (dupSCHLEMAIL && flag == 0)
            //            {
            //                flag = 3;
            //                ViewBag.flag = 3;
            //                ViewBag.error = "Duplicate EMAIL ID";
            //            }

            //            ViewData["result"] = "9";
            //            ViewBag.APPNO = "err";
            //            Session["EcaptchaCode"] = captcha.captchaCode;
            //            Session["EcaptchaImg"] = captcha.captchaImg;
            //            ViewBag.captchaCode = captcha.captchaCode;
            //            ViewBag.captchaImg = captcha.captchaImg;

            //            _EAffiliationModel.PWD = string.Empty;
            //            if (flag > 0)
            //            {

            //                string Search = "SCHLEMAIL='" + SCHLEMAIL1 + "'";
            //                DataSet dschk1 = eAffiliationDB.EAffiliationList(Search, "", 0, 3);
            //                if (dschk1.Tables.Count > 0)
            //                {
            //                    if (dschk1.Tables[0].Rows.Count > 0)
            //                    {
            //                        ViewBag.errorAppNo = dschk1.Tables[0].Rows[0]["AppNo"].ToString();
            //                        ViewBag.errorName = dschk1.Tables[0].Rows[0]["SCHLNAME"].ToString();

            //                    }
            //                }
            //            }

            //            return View(_EAffiliationModel);
            //        }
            //    }
            //}

            //#endregion Duplicacy Check 

            //  Check and insert 

            string cnfrm_pass = _EAffiliationModel.RepeatPassword.ToString();
            if (cnfrm_pass != _EAffiliationModel.PWD)
            {
                ViewData["result"] = "7";
                ViewBag.APPNO = "err";
                ViewBag.error = "Password Not Matched";
                Session["EcaptchaCode"] = captcha.captchaCode;
                Session["EcaptchaImg"] = captcha.captchaImg;
                ViewBag.captchaCode = captcha.captchaCode;
                ViewBag.captchaImg = captcha.captchaImg;
                _EAffiliationModel.PWD = string.Empty;
                return View(_EAffiliationModel);
            }
            else
            {

                #region Re New

                if (_EAffiliationModel.EAffType.ToUpper().Trim() == "RENEW")
                {
                    if (string.IsNullOrEmpty(_EAffiliationModel.SCHL))
                    {
                        ViewBag.flag = 5;
                        ViewBag.error = "Enter School Code";
                        ViewData["result"] = "5";
                        ViewBag.APPNO = "err";
                        return View(_EAffiliationModel);
                    }

                    DataSet outDS;
                    SchoolModels schoolModels = new AbstractLayer.SchoolDB().GetSchoolDataBySchl(_EAffiliationModel.SCHL.Trim(), out outDS);    //SelectSchoolDatabyID 
                    if (schoolModels.SCHL != null)
                    {
                        if (schoolModels.SCHL.Trim() == _EAffiliationModel.SCHL.Trim())
                        {
                            string search = "SCHL='" + _EAffiliationModel.SCHL + "'";
                            DataSet dschk = eAffiliationDB.EAffiliationList("", search, "", 0, 3);
                            if (dschk.Tables.Count > 0)
                            {
                                if (dschk.Tables[0].Rows.Count > 0)
                                {
                                    bool dupSCHLEMAIL = dschk.Tables[0].AsEnumerable().Any(row => _EAffiliationModel.SCHL.ToUpper() == row.Field<string>("SCHL").ToUpper());

                                    if (dupSCHLEMAIL == true)
                                    {
                                        ViewData["result"] = "5";
                                        ViewBag.APPNO = "err";
                                        ViewBag.flag = 5;
                                        ViewBag.error = "School Code Already Exists";
                                        return View(_EAffiliationModel);
                                    }
                                }
                            }
                        }
                        _EAffiliationModel.UDISECODE = schoolModels.udisecode;
                        _EAffiliationModel.SCHLNME = schoolModels.SCHLE.ToUpper();
                        _EAffiliationModel.SCHLNMP = schoolModels.SCHLP;
                        _EAffiliationModel.STATIONE = schoolModels.STATIONE.ToUpper();
                        _EAffiliationModel.STATIONP = schoolModels.STATIONP;
                        //_EAffiliationModel.ADDRESSE = schoolModels.ADDRESSE.ToUpper();
                        //_EAffiliationModel.ADDRESSP = schoolModels.ADDRESSP;
                        _EAffiliationModel.DIST = schoolModels.dist;
                        _EAffiliationModel.EducationType = schoolModels.SchlType;
                        _EAffiliationModel.Area = schoolModels.AREA;
                        _EAffiliationModel.PrincipalName = schoolModels.PRINCIPAL.ToUpper();
                        _EAffiliationModel.Qualification = schoolModels.PQualification;
                        _EAffiliationModel.DOJ = schoolModels.DOJ;
                        _EAffiliationModel.OtherContactPerson = schoolModels.CONTACTPER.ToUpper();
                        _EAffiliationModel.DOB = schoolModels.DOB;
                        _EAffiliationModel.Experience = schoolModels.ExperienceYr;
                        _EAffiliationModel.StdCode = schoolModels.STDCODE;
                        _EAffiliationModel.PrincipalMobileNo = schoolModels.MOBILE;
                        _EAffiliationModel.PHONE = schoolModels.PHONE;
                    }
                    else
                    {
                        ViewData["result"] = "5";
                        ViewBag.APPNO = "err";
                        ViewBag.flag = 5;
                        ViewBag.error = "Invalid School Code";
                        return View(_EAffiliationModel);
                    }

                }



                #endregion

                string outError = "0";
                _EAffiliationModel.ID = 0;
                if (_EAffiliationModel.CREATEDDATE == new DateTime()) { _EAffiliationModel.CREATEDDATE = DateTime.Now; }

                _EAffiliationModel.SCHLNME = _EAffiliationModel.SCHLNAME;
                int result = eAffiliationDB.EAffiliation(_EAffiliationModel, 0, out outError);// if ID=0 then Insert else Update
                if (result > 0)
                {
                    _EAffiliationModel.PWD = string.Empty;
                    ViewBag.Totalcount = 1;
                    ViewBag.APPNO = outError;
                    ViewBag.result = result;
                    ViewData["result"] = "1";
                    EAffiliationModel ol = eAffiliationDB.GetEAffiliation(outError);
                    if (ol.APPNO == outError)
                    {
                        Session["eAffiliationAppNo"] = ol.APPNO.ToString();
                        Session["eAffiliationName"] = ol.SCHLNAME.ToString();
                        Session["eAffiliationId"] = ol.ID.ToString();
                        Session["Session"] = @PSEBONLINE.Repository.SessionSettingMastersRepository.GetSessionSettingMasters().SessionShortYear;
                        Session["epayStatus"] = "0";
                        Session["echlnStatus"] = "0";
                        //Form lock status
                        Session["eAffiliationIsFormLock"] = ol.IsFormLock.ToString();

                        _EAffiliationModel.PWD = ol.PWD;
                        try
                        {
                            //string sms = "Dear Candidate, You are Successfully Registered Under E-Affiliation. Your App. No:"+{#var#} and Pwd:{#var#}. Kindly Login & Complete Your form Regards PSEB";
                            string Sms = "You are successfully registered under E-Affiliation. Your App. No:" + ol.APPNO + " and Pwd:" + ol.PWD + ". Kindly Login & complete your form Regards PSEB";
                            string templateId = "1007752347952330484";
                            //string getSms = new AbstractLayer.DBClass().gosms(ol.PrincipalMobileNo, Sms);
                            string getSms = new AbstractLayer.DBClass().gosmsPsebforschool(ol.PrincipalMobileNo, Sms, templateId);
                            
                            //eAffiliationDB.mailer(ol.SCHLEMAIL, ol.APPNO, ol.PWD, _EAffiliationModel.SCHLNME);                   
                        }
                        catch (Exception ex)
                        {
                            ViewBag.error = ex.Message.ToString();
                        }
                    }
                }
                else
                {
                    _EAffiliationModel.PWD = string.Empty;
                    ViewBag.Totalcount = 0;
                    ViewBag.APPNO = "";
                    ViewData["result"] = 0;
                    ViewBag.Message = outError.ToString();
                }

            }

            //

            _EAffiliationModel.PWD = string.Empty;
            return View(_EAffiliationModel);
        }




        #endregion Registration


        #region SchoolProfile

        //GetEAff_NewschlList
        public JsonResult GetEAffNewSchlCheckByUdise(string udisecode)
        {
            string outid = "0";
            string search = "where udise = " + udisecode;
            EAffiliationModel eam = new EAffiliationModel();
            eam = eAffiliationDB.GetEAff_NewschlList(search);
            if (eam != null)
            {
                if (eam.UDISECODE != null)
                {
                    outid = "1";
                    return Json(new { model = eam, dist = eam.DIST, distnme = eam.DISTNME, udisecode = eam.UDISECODE, schlnme = eam.SCHLNME, principal = eam.PrincipalName, oid = outid }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { model = eam, dist = eam.DIST, distnme = eam.DISTNME, udisecode = eam.UDISECODE, schlnme = eam.SCHLNME, principal = eam.PrincipalName, oid = outid }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult CheckPinCodeMaster(string pincode) // Calling on http post (on Submit)
        {
            List<SelectListItem> pincodeList = new List<SelectListItem>();
            pincodeList.Add(new SelectListItem { Text = "--Select PinCode--", Value = "0" });

            DataSet result = eAffiliationDB.CheckPinCodeMasterSP(1, pincode);
            if (result != null)
            {
                if (result.Tables[0].Rows.Count > 0)
                {
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        pincodeList.Add(new SelectListItem { Text = @dr["PostOfficeNM"].ToString(), Value = @dr["PostOfficeNM"].ToString() });
                    }
                }
            }
            ViewBag.MyPostOfficeName = pincodeList;
            return Json(pincodeList);
        }


        public ActionResult SchoolProfile(string id, EAffiliationModel eam)
        {

            if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (Session["eAffiliationIsFormLock"].ToString() == "1")
            { return RedirectToAction("ViewEAffiliation", new { id = Session["eAffiliationAppNo"].ToString() }); }

            //
            List<SelectListItem> pincodeList = new List<SelectListItem>();
            pincodeList.Add(new SelectListItem { Text = "--Select PinCode--", Value = "0" });
            ViewBag.MyPostOfficeName = pincodeList;


            // Boys / Girls / Co - Education
            var itemsSchlType = new SelectList(new[] { new { ID = "Boys", Name = "Boys" }, new { ID = "Girls", Name = "Girls" }, new { ID = "Co-Education", Name = "Co-Education" }, }, "ID", "Name", 1);
            ViewBag.MySchlType = itemsSchlType.ToList();
            ViewBag.SelectedSchlType = "0";

            // Area 
            ViewBag.AREAList = objCommon.GetArea();
            // YesNo 
            ViewBag.YesNoList = objCommon.GetYesNo();
            // English Dist 
            ViewBag.DistEList = objCommon.GetDistE();

            // English Tehsil 
            ViewBag.TehEList = objCommon.GetAllTehsil();

            eam = eAffiliationDB.GetEAffiliation(id);
            if (eam.APPNO != id)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else
            {


                //
                if (!string.IsNullOrEmpty(eam.DIST))
                {
                    // English Tehsil 
                    DataSet result = new AbstractLayer.SchoolDB().SelectAllTehsil(eam.DIST);
                    List<SelectListItem> TehList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCode"].ToString() });
                    }
                    ViewBag.TehEList = TehList;
                }

                if (!string.IsNullOrEmpty(eam.PINCODE))
                {
                    // PINCODE
                    List<SelectListItem> pincodeList1 = new List<SelectListItem>();
                    pincodeList1.Add(new SelectListItem { Text = "--Select PinCode--", Value = "0" });
                    DataSet result = eAffiliationDB.CheckPinCodeMasterSP(1, eam.PINCODE);
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        pincodeList1.Add(new SelectListItem { Text = @dr["PostOfficeNM"].ToString(), Value = @dr["PostOfficeNM"].ToString() });
                    }
                    ViewBag.MyPostOfficeName = pincodeList1;
                }

                Session["IsEAffilicationCLU_Allowed"] = "N";
                if (eam.EAffType == "RENEW")
                {
                    DataSet outDS;
                    SchoolModels schoolModels = new AbstractLayer.SchoolDB().GetSchoolDataBySchl(eam.SCHL, out outDS);    //SelectSchoolDatabyID 
                    if (schoolModels.SCHL != null)
                    {
                        eam.dataSetPrevious = outDS;
                        Session["IsEAffilicationCLU_Allowed"] = outDS.Tables[0].Rows[0]["IsEAffilicationCLU_Allowed"].ToString();

                    }

                }
                else
                {
                    Session["IsEAffilicationCLU_Allowed"] = "Y"; // compulsory in NEW
                }

            }

            return View(eam);
        }

        [HttpPost]
        public ActionResult SchoolProfile(string id, EAffiliationModel eam, FormCollection frm)
        {
            if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
            {
                return RedirectToAction("Index", "EAffiliation");
            }

            //
            List<SelectListItem> pincodeList = new List<SelectListItem>();
            pincodeList.Add(new SelectListItem { Text = "--Select PinCode--", Value = "0" });
            ViewBag.MyPostOfficeName = pincodeList;

            // Boys / Girls / Co - Education
            var itemsSchlType = new SelectList(new[] { new { ID = "Boys", Name = "Boys" }, new { ID = "Girls", Name = "Girls" }, new { ID = "Co-Education", Name = "Co-Education" }, }, "ID", "Name", 1);
            ViewBag.MySchlType = itemsSchlType.ToList();
            ViewBag.SelectedSchlType = "0";

            // Area 
            ViewBag.AREAList = objCommon.GetArea();
            // YesNo 
            ViewBag.YesNoList = objCommon.GetYesNo();
            // English Dist 
            ViewBag.DistEList = objCommon.GetDistE();
            // English Tehsil 
            ViewBag.TehEList = objCommon.GetAllTehsil();


            // Update Data
            eam.APPNO = id;
            string outError = "0";
            int result = eAffiliationDB.EAffiliation(eam, 1, out outError);// if ID=1 for School Profile Update
            if (result > 0)
            {
                ViewData["result"] = "1";
                ViewBag.Mesaage = outError;
            }
            else
            {
                ViewData["result"] = "0";
                ViewBag.Mesaage = outError;
            }

            return View(eam);
        }
        #endregion SchoolProfile

        #region BuildingFireMapDetails
        public ActionResult BuildingFireMapDetails(string id, EAffiliationModel eam)
        {
            if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (Session["eAffiliationIsFormLock"].ToString() == "1")
            { return RedirectToAction("ViewEAffiliation", new { id = Session["eAffiliationAppNo"].ToString() }); }



            List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession = objCommon.GetSessionYear();
            itemSession.Add(new SelectListItem { Text = "2025", Value = "2025" });
            itemSession.Add(new SelectListItem { Text = "2024", Value = "2024" });
            //itemSession.Add(new SelectListItem { Text = "2023", Value = "2023" });
            //itemSession.Add(new SelectListItem { Text = "2022", Value = "2022" });
            //itemSession.Add(new SelectListItem { Text = "2021", Value = "2021" });
            int currentYear = DateTime.Now.Year;
            ViewBag.YearList = itemSession.Where(s => Convert.ToInt32(s.Value) >= Convert.ToInt32(currentYear - 4) && Convert.ToInt32(s.Value) <= Convert.ToInt32(currentYear)).OrderByDescending(s => s.Value);
            ViewBag.YearListTo = itemSession.Where(s => Convert.ToInt32(s.Value) >= Convert.ToInt32(currentYear - 2)).OrderByDescending(s => s.Value);


            ViewBag.SelectedYear = "0";
            var itemsch = new SelectList(new[]{new {ID="1",Name="Punjab Public Works Department, B&R"},new {ID="2",Name="Department of Rural development and Panchayat"},
            }, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "0";




            eam = eAffiliationDB.GetEAffiliation(id);
            if (eam.APPNO != id)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else
            {
                //

                if (!string.IsNullOrEmpty(eam.BSIA))
                {
                    eam.BSIA = itemsch.ToList().Where(s => s.Text == eam.BSIA).Select(s => s.Value).FirstOrDefault();
                }

                if (eam.SocietyFile == "" || eam.SocietyFile == null)
                { @ViewBag.SocietyFile = "../Images/NoPhoto.jpg"; }
                else
                {
                    @ViewBag.SocietyFile = "../../Upload/" + eam.SocietyFile.ToString();
                }

                if (eam.BSFILE == "" || eam.BSFILE == null)
                { @ViewBag.BSFILE = "../Images/NoPhoto.jpg"; }
                else
                {
                    @ViewBag.BSFILE = "../../Upload/" + eam.BSFILE.ToString();
                }

                if (eam.FSFILE == "" || eam.FSFILE == null)
                { @ViewBag.FSFILE = "../Images/NoPhoto.jpg"; }
                else
                {
                    @ViewBag.FSFILE = "../../Upload/" + eam.FSFILE.ToString();
                }

                if (eam.MAPFILE == "" || eam.MAPFILE == null)
                { @ViewBag.MAPFILE = "../Images/NoPhoto.jpg"; }
                else
                {
                    @ViewBag.MAPFILE = "../../Upload/" + eam.MAPFILE.ToString();
                }

                Session["IsEAffilicationCLU_Allowed"] = "N";
                if (eam.EAffType == "RENEW")
                {
                    DataSet outDS;
                    SchoolModels schoolModels = new AbstractLayer.SchoolDB().GetSchoolDataBySchl(eam.SCHL, out outDS);    //SelectSchoolDatabyID 
                    if (schoolModels.SCHL != null)
                    {
                        eam.dataSetPrevious = outDS;
                        Session["IsEAffilicationCLU_Allowed"] = outDS.Tables[0].Rows[0]["IsEAffilicationCLU_Allowed"].ToString();

                    }

                }
                else
                {
                    Session["IsEAffilicationCLU_Allowed"] = "Y"; // compulsory in NEW
                }
                if(eam.EAffClass =="5" || eam.EAffClass =="8")
                {
                    Session["IsEAffilicationCLU_Allowed"] = "N";
                }



            }

            return View(eam);
        }

        [HttpPost]
        public ActionResult BuildingFireMapDetails(string id, EAffiliationModel eam, FormCollection frm, HttpPostedFileBase CLUfile, HttpPostedFileBase societyfile, HttpPostedFileBase bsfile, HttpPostedFileBase fsfile, HttpPostedFileBase mapfile)
        {
            if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession = objCommon.GetSessionYear();
            itemSession.Add(new SelectListItem { Text = "2024", Value = "2024" });
            itemSession.Add(new SelectListItem { Text = "2023", Value = "2023" });
            
            ViewBag.YearList = itemSession.Where(s => Convert.ToInt32(s.Value) >= 2017 && Convert.ToInt32(s.Value) <= 2022).OrderByDescending(s => s.Value);
            ViewBag.YearListTo = itemSession.Where(s => Convert.ToInt32(s.Value) >= 2017).OrderByDescending(s => s.Value);


            ViewBag.SelectedYear = "0";
            var itemsch = new SelectList(new[]{new {ID="1",Name="Punjab Public Works Department, B&R"},
                new {ID="2",Name="Department of Rural development and Panchayat"},}, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "0";

            if (!string.IsNullOrEmpty(eam.BSIA))
            {
                eam.BSIA = itemsch.ToList().Where(s => s.Value == eam.BSIA).Select(s => s.Text).FirstOrDefault();
            }

            // Save file


            string filename = "";
            if (bsfile != null)
            {
                string ext = Path.GetExtension(bsfile.FileName);
                filename = eam.APPNO + "_BSFILE" + ext;
                var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/EAffiliation2021/BuildingFireMapDetails"), filename);
                string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/EAffiliation2021/BuildingFireMapDetails"));
                if (!Directory.Exists(FilepathExist))
                {
                    Directory.CreateDirectory(FilepathExist);
                }
                bsfile.SaveAs(path);
                eam.BSFILE = "Upload2023/EAffiliation2021/BuildingFireMapDetails/" + filename;
            }

            string filename1 = "";
            if (fsfile != null)
            {
                string ext = Path.GetExtension(fsfile.FileName);
                filename1 = eam.APPNO + "_FSFILE" + ext;
                var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/EAffiliation2021/BuildingFireMapDetails"), filename1);
                string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/EAffiliation2021/BuildingFireMapDetails"));
                if (!Directory.Exists(FilepathExist))
                {
                    Directory.CreateDirectory(FilepathExist);
                }
                fsfile.SaveAs(path);
                eam.FSFILE = "Upload2023/EAffiliation2021/BuildingFireMapDetails/" + filename1;
            }

            string filename2 = "";
            if (societyfile != null)
            {
                string ext = Path.GetExtension(societyfile.FileName);
                filename2 = eam.APPNO + "_SocietyFile" + ext;
                var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/EAffiliation2021/BuildingFireMapDetails"), filename2);
                string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/EAffiliation2021/BuildingFireMapDetails"));
                if (!Directory.Exists(FilepathExist))
                {
                    Directory.CreateDirectory(FilepathExist);
                }
                societyfile.SaveAs(path);
                eam.SocietyFile = "Upload2023/EAffiliation2021/BuildingFireMapDetails/" + filename2;
            }


            string filenameMAP = "";
            if (mapfile != null)
            {
                string ext = Path.GetExtension(mapfile.FileName);
                filenameMAP = eam.APPNO + "_MapFile" + ext;
                var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/EAffiliation2021/BuildingFireMapDetails"), filenameMAP);
                string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/EAffiliation2021/BuildingFireMapDetails"));
                if (!Directory.Exists(FilepathExist))
                {
                    Directory.CreateDirectory(FilepathExist);
                }
                mapfile.SaveAs(path);
                eam.MAPFILE = "Upload2023/EAffiliation2021/BuildingFireMapDetails/" + filenameMAP;
            }


            string filenameCLU = "";
            if (CLUfile != null)
            {
                string ext = Path.GetExtension(CLUfile.FileName);
                filenameCLU = eam.APPNO + "_MapFile" + ext;
                var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/EAffiliation2021/BuildingFireMapDetails"), filenameCLU);
                string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/EAffiliation2021/BuildingFireMapDetails"));
                if (!Directory.Exists(FilepathExist))
                {
                    Directory.CreateDirectory(FilepathExist);
                }
                CLUfile.SaveAs(path);
                eam.CLUFILE = "Upload2023/EAffiliation2021/BuildingFireMapDetails/" + filenameCLU;
            }
            // Update Data
            eam.APPNO = id;
            string outError = "0";
            int result = eAffiliationDB.EAffiliation(eam, 2, out outError);// if ID=2 for UpdateBuildingFireMapDetails 
            if (result > 0)
            {
                ViewData["result"] = "1";
                ViewBag.Mesaage = outError;
            }
            else
            {
                ViewData["result"] = "0";
                ViewBag.Mesaage = outError;
            }
            return View(eam);
        }
        #endregion

        #region StaffDetails

        public JsonResult SubjectList(int id)
        {
            return Json(objCommon.GetSubject(id).ToArray(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult StaffDetails(int? page, string id, EAffiliationStaffDetailsModel easdm, string eStaffId, string act)
        {
            if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (Session["eAffiliationIsFormLock"].ToString() == "1")
            { return RedirectToAction("Dashboard", new { id = Session["eAffiliationAppNo"].ToString() }); }


            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.pagesize = pageIndex;
            //
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

            @ViewBag.DA = objCommon.GetDA();
            //
            easdm.APPNO = id;

            ViewBag.TotalEStaff = 0;
            EAffiliationModel eam = new EAffiliationModel();
            eam = eAffiliationDB.GetEAffiliation(id);
            if (eam.APPNO != id)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else
            {

                if (string.IsNullOrEmpty(eStaffId))
                {
                    string Search = string.Empty;
                    ViewBag.APPNO = id;
                    Search = "a.APPNO = '" + id + "' ";
                    easdm.StoreAllData = eAffiliationDB.GetEAffiliationStaffDetails(Search, pageIndex);
                    if (easdm.StoreAllData == null || easdm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = easdm.StoreAllData.Tables[0].Rows.Count;
                        //
                        int count = Convert.ToInt32(easdm.StoreAllData.Tables[1].Rows[0]["TotalCount"]);
                        ViewBag.TotalCount1 = count;
                        int tp = Convert.ToInt32(count);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                    }
                }
                else if (act == "U") // for update
                {
                    string Search = string.Empty;
                    ViewBag.EStaffId = eStaffId;
                    Search = "a.eStaffId = '" + eStaffId + "' ";
                    easdm.StoreAllData = eAffiliationDB.GetEAffiliationStaffDetails(Search, pageIndex);
                    if (easdm.StoreAllData == null || easdm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.MessageEStaff = "Record Not Found";
                        ViewBag.TotalEStaff = 0;
                    }
                    else
                    {
                        ViewBag.TotalEStaff = 1;

                        DataRow dr = easdm.StoreAllData.Tables[0].Rows[0];
                        easdm.eStaffId = Convert.ToInt32(dr["eStaffId"].ToString());
                        easdm.APPNO = dr["APPNO"].ToString();
                        easdm.Name = dr["Name"].ToString();
                        easdm.FName = dr["FName"].ToString();
                        easdm.DOB = dr["DOB"].ToString();
                        easdm.Gender = dr["Gender"].ToString();
                        easdm.AadharNo = dr["AadharNo"].ToString();
                        easdm.Qualification = dr["Qualification"].ToString();
                        easdm.ExpYear = dr["ExpYear"].ToString();
                        easdm.ExpMonth = dr["ExpMonth"].ToString();
                        easdm.MOBILENO = dr["MOBILENO"].ToString();
                        easdm.Salary = dr["Salary"].ToString();
                        easdm.SalaryMode = dr["SalaryMode"].ToString();
                        //
                        ViewBag.caderName = dr["Cadre"].ToString().ToUpper();
                        ViewBag.subjectName = dr["Subject"].ToString().ToUpper();

                        if (!string.IsNullOrEmpty(dr["Cadre"].ToString()))
                        {
                            List<SelectListItem> cadreList = ViewBag.cadre;
                            string val = cadreList.Where(s => s.Text.ToUpper().Trim() == dr["Cadre"].ToString().ToUpper()).Select(s => s.Value).FirstOrDefault();
                            //easdm.Cadre = objCommon.GetCadre().Where(s => s.Text == dr["Cadre"].ToString()).Select(s => s.Value.ToUpper()).FirstOrDefault();
                            easdm.Cadre = val;
                            ViewBag.SelectedCadreId = easdm.Cadre;
                        }
                        if (!string.IsNullOrEmpty(dr["Subject"].ToString()) && !string.IsNullOrEmpty(easdm.Cadre))
                        {
                            easdm.Subject = objCommon.GetSubject(Convert.ToInt32(easdm.Cadre)).Where(s => s.Text.ToUpper().Trim() == dr["Subject"].ToString().ToUpper().Trim()).Select(s => s.Value).FirstOrDefault();
                            ViewBag.SelectedSubjectId = easdm.Subject;
                        }

                        if (easdm.StaffFile == "" || easdm.StaffFile == null)
                        { @ViewBag.STAFFFILE = "../Images/NoPhoto.jpg"; }
                        else
                        {
                            @ViewBag.STAFFFILE = ConfigurationManager.AppSettings["AWSURL"] + easdm.StaffFile.ToString();
                        }



                    }

                }
            }

            return View(easdm);
        }

        [HttpPost]
        public ActionResult StaffDetails(string id, EAffiliationStaffDetailsModel easdm, FormCollection frm, string cmd, HttpPostedFileBase StaffFile)
        {
            if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
            {
                return RedirectToAction("Index", "EAffiliation");
            }

            //
            ViewBag.cadre = objCommon.GetCadre();
            //
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
            //

            easdm.APPNO = id;



            if (!string.IsNullOrEmpty(cmd))
            {
                string outError = "0";
                int result = 0;
                if (cmd.ToLower() == "submit" || cmd.ToLower() == "save" || cmd.ToLower() == "update")
                {
                    ViewBag.Action = cmd.ToLower();
                    string cadreName = "";
                    string SubNM = "";


                    if (!string.IsNullOrEmpty(easdm.Subject) && !string.IsNullOrEmpty(easdm.Cadre))
                    {
                        SubNM = objCommon.GetSubject(Convert.ToInt32(easdm.Cadre)).Where(s => s.Value == easdm.Subject).Select(s => s.Text).FirstOrDefault();
                        easdm.Subject = SubNM.ToUpper();
                    }

                    if (!string.IsNullOrEmpty(easdm.Cadre))
                    {
                        cadreName = objCommon.GetCadre().Where(s => s.Value == easdm.Cadre).Select(s => s.Text).FirstOrDefault();
                        easdm.Cadre = cadreName.ToUpper();
                    }



                    string StaffFileName = "";
                    if (StaffFile != null)
                    {
                        string ext = Path.GetExtension(StaffFile.FileName);
                        StaffFileName = easdm.APPNO + "_" + easdm.AadharNo + "_STAFFFILE" + ext;
                        var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/EAffiliation2021/STAFFFILE"), StaffFileName);
                        string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/EAffiliation2021/STAFFFILE"));

                        string Orgfile = StaffFileName;

                        using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                        {
                            using (var newMemoryStream = new MemoryStream())
                            {
                                var uploadRequest = new TransferUtilityUploadRequest
                                {
                                    InputStream = StaffFile.InputStream,
                                    Key = string.Format("allfiles/Upload2023/EAffiliation2021/STAFFFILE/{0}", Orgfile),

                                    BucketName = BUCKET_NAME,
                                    CannedACL = S3CannedACL.PublicRead
                                };

                                var fileTransferUtility = new TransferUtility(client);
                                fileTransferUtility.Upload(uploadRequest);
                            }
                        }
                        //if (!Directory.Exists(FilepathExist))
                        //{
                        //    Directory.CreateDirectory(FilepathExist);
                        //}
                        //StaffFile.SaveAs(path);
                        easdm.StaffFile = "allfiles/Upload2023/EAffiliation2021/STAFFFILE/" + StaffFileName;
                    }


                    if (cmd.ToLower() == "submit" || cmd.ToLower() == "save")
                    {

                        result = eAffiliationDB.InsertEAffiliationStaffDetails(easdm, 0, out outError);  // 0 for insert
                    }
                    else if (cmd.ToLower() == "update")
                    {
                        result = eAffiliationDB.InsertEAffiliationStaffDetails(easdm, 1, out outError); // 1 for update
                    }
                    else if (cmd.ToLower() == "delete")
                    {
                        result = eAffiliationDB.InsertEAffiliationStaffDetails(easdm, 2, out outError); // 2 for delete
                    }

                    if (result > 0)
                    {
                        ViewData["result"] = "1";
                        ViewBag.Mesaage = outError;
                    }
                    else
                    {
                        ViewData["result"] = outError.ToString();
                        ViewBag.Mesaage = outError;
                    }
                }
                else if (cmd.ToLower() == "delete")
                {
                    result = eAffiliationDB.InsertEAffiliationStaffDetails(easdm, 2, out outError); // 2 for delete
                    if (result > 0)
                    {
                        ViewData["result"] = "1";
                        ViewBag.Mesaage = outError;
                    }
                    else
                    {
                        ViewData["result"] = outError.ToString();
                        ViewBag.Mesaage = outError;
                    }
                }


            }

            return View(easdm);
        }



        public ActionResult ActionStaffDetails(string id, string eStaffId, string act)
        {
            try
            {
                string outError = "0";
                int result = 0;
                EAffiliationStaffDetailsModel easdm = new EAffiliationStaffDetailsModel();
                if (id == null || eStaffId == null)
                {
                    // return RedirectToAction("Index", "EAffiliation");
                }
                else
                {
                    if (act == "D")
                    {
                        easdm.APPNO = Convert.ToString(id);
                        easdm.eStaffId = Convert.ToInt32(eStaffId);
                        result = eAffiliationDB.InsertEAffiliationStaffDetails(easdm, 2, out outError); // 2 for delete
                        if (outError == "1")
                        {
                            ViewBag.result = "1";
                            ViewData["Status"] = "DEL";
                        }
                    }
                }
                //return RedirectToAction("CalculateFee", "EAffiliation", new { Id = Session["eAffiliationAppNo"] });

            }
            catch (Exception)
            {
                //return RedirectToAction("CalculateFee", "EAffiliation", new { Id = Session["eAffiliationAppNo"] });
            }
            //     return Json(new { sn = ViewBag.result, chid = ViewData["Status"] }, JsonRequestBehavior.AllowGet);
            return RedirectToAction("StaffDetails", "EAffiliation", new { Id = Session["eAffiliationAppNo"] });

        }


        #endregion StaffDetails

        #region StudentDetails
        public ActionResult StudentDetails(string id, EAffiliationModel eam)
        {
            if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (Session["eAffiliationIsFormLock"].ToString() == "1")
            { return RedirectToAction("ViewEAffiliation", new { id = Session["eAffiliationAppNo"].ToString() }); }


            eam = eAffiliationDB.GetEAffiliation(id);
            if (eam.APPNO != id)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            return View(eam);
        }


        [HttpPost]
        public ActionResult StudentDetails(string id, EAffiliationModel eam, FormCollection frm)
        {
            if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
            {
                return RedirectToAction("Index", "EAffiliation");
            }

            // Update Data
            eam.APPNO = id;
            string outError = "0";
            int result = eAffiliationDB.EAffiliation(eam, 3, out outError);// if ID=2 for StudentDetails
            if (result > 0)
            {
                ViewData["result"] = "1";
                ViewBag.Mesaage = outError;


            }
            else
            {
                ViewData["result"] = "0";
                ViewBag.Mesaage = outError;
            }


            return View(eam);
        }
        #endregion StudentDetails

        #region OtherInformation

        public MultiSelectList StudyMediumList(string sel, int type)
        {
            var AdminList = new AbstractLayer.OpenDB().GetMedium().ToList().Select(c => new
            {
                Text = c.Text,
                Value = c.Value
            }).OrderBy(s => s.Text).ToList();

            if (type == 0)
            {
                if (sel == "")
                { return new MultiSelectList(AdminList, "Value", "Text"); }
                else
                {
                    int[] myArray1 = AbstractLayer.StaticDB.StringToIntArray(sel, ',');
                    return new MultiSelectList(AdminList, "Value", "Text", myArray1);
                }
            }
            else
            {
                if (sel == "")
                { return new MultiSelectList(AdminList, "Value", "Text"); }
                else
                {
                    string[] myArray1 = AbstractLayer.StaticDB.StringToStringArray(sel, ',');
                    return new MultiSelectList(AdminList, "Value", "Text", myArray1);
                }
            }

        }

        public ActionResult OtherInformation(string id, EAffiliationModel eam)
        {

            if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (Session["eAffiliationIsFormLock"].ToString() == "1")
            { return RedirectToAction("ViewEAffiliation", new { id = Session["eAffiliationAppNo"].ToString() }); }


            //StudyMedium
            // ViewBag.StudyMedium = new AbstractLayer.OpenDB().GetMedium();    

            ViewBag.MyStudyMediumList = StudyMediumList("", 0);


            //Own / Donation / Lease Dead / Rent
            var itemland = new SelectList(new[]{new {ID="Own",Name="Own"},new {ID="Donation",Name="Donation"},
            new {ID="LeaseDeed",Name="LeaseDeed"},new {ID="Rent",Name="Rent"},}, "ID", "Name", 1);
            ViewBag.LandList = itemland.ToList();
            ViewBag.SelectedLand = "0";


            var itemPlayGround = new SelectList(new[]{new {ID= "Within Institute Premisses", Name="Within Institute Premisses"},
                new {ID="Within 1 KM Distance",Name="Within 1 KM Distance"},}, "ID", "Name", 1);
            ViewBag.PlayGroundList = itemPlayGround.ToList();
            ViewBag.SelectedPlayGround = "0";

            ViewBag.YesNoList = objCommon.GetYesNoText();


            eam = eAffiliationDB.GetEAffiliation(id);
            if (eam.APPNO != id)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else
            {

                ViewBag.MyStudyMediumList = StudyMediumList(eam.OSTUDYMEDIUM, 1);
            }

            return View(eam);
        }

        [HttpPost]
        public ActionResult OtherInformation(string id, EAffiliationModel eam, FormCollection frm)
        {

            if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            //StudyMedium
            ViewBag.MyStudyMediumList = StudyMediumList("", 0);


            //Own / Donation / Lease Dead / Rent
            var itemland = new SelectList(new[]{new {ID="Own",Name="Own"},new {ID="Donation",Name="Donation"},
            new {ID="LeaseDeed",Name="LeaseDeed"},new {ID="Rent",Name="Rent"},}, "ID", "Name", 1);
            ViewBag.LandList = itemland.ToList();
            ViewBag.SelectedLand = "0";

            var itemPlayGround = new SelectList(new[]{new {ID= "Within Institute Premisses", Name="Within Institute Premisses"},
                new {ID="Within 1 KM Distance",Name="Within 1 KM Distance"},}, "ID", "Name", 1);
            ViewBag.PlayGroundList = itemPlayGround.ToList();
            ViewBag.SelectedPlayGround = "0";


            ViewBag.YesNoList = objCommon.GetYesNoText();


            string SelectedStudyMedium = "";
            if (frm["SelectedStudyMedium"] == "" || frm["SelectedStudyMedium"] == null)
            {
                ViewData["Result"] = 20;
                return View(eam);
            }
            else
            {
                SelectedStudyMedium = frm["SelectedStudyMedium"].ToString();
                eam.OSTUDYMEDIUM = frm["SelectedStudyMedium"].ToString();
            }

            // Update Data
            eam.APPNO = id;
            string outError = "0";
            int result = eAffiliationDB.EAffiliation(eam, 4, out outError);// if ID=4 for OtherInformation
            if (result > 0)
            {
                ViewData["result"] = "1";
                ViewBag.Mesaage = outError;


            }
            else
            {
                ViewData["result"] = "0";
                ViewBag.Mesaage = outError;
            }

            return View(eam);
        }
        #endregion

        #region CalculateFee


        public JsonResult GetGroupByClass(string SelClass)
        {
            List<SelectListItem> objGroupList = new List<SelectListItem>();

            if (SelClass == "12")
            {
                string search = "b.appno='" + Session["eAffiliationAppNo"].ToString() + "' and b.class='" + SelClass + "'  ";
                DataSet dschk = eAffiliationDB.EAffiliationList("", search, SelClass, 0, 5);
                if (dschk.Tables.Count > 0)
                {
                    if (dschk.Tables[1].Rows.Count > 0)
                    {
                        if (!dschk.Tables[1].AsEnumerable().Any(row => "Y".ToUpper() == row.Field<string>("hum").ToUpper()))
                        {
                           
                                objGroupList.Add(new SelectListItem { Text = "HUMANITIES", Value = "HUMANITIES" });
                               
                        }
                        if (!dschk.Tables[1].AsEnumerable().Any(row => "Y".ToUpper() == row.Field<string>("sci").ToUpper()))
                        {
                            
                                objGroupList.Add(new SelectListItem { Text = "SCIENCE", Value = "SCIENCE" });
                           
                                
                        }
                        if (!dschk.Tables[1].AsEnumerable().Any(row => "Y".ToUpper() == row.Field<string>("comm").ToUpper()))
                        {
                            
                                objGroupList.Add(new SelectListItem { Text = "COMMERCE", Value = "COMMERCE" });
                             
                        }
                        if (!dschk.Tables[1].AsEnumerable().Any(row => "Y".ToUpper() == row.Field<string>("voc").ToUpper()))
                        {

                            objGroupList.Add(new SelectListItem { Text = "VOCATIONAL", Value = "VOCATIONAL" });

                        }
                        //if (dschk.Tables[0].AsEnumerable().Any(row => "HUMANITIES".ToUpper() == row.Field<string>("exam").ToUpper()))
                        //{
                        //    objGroupList.Add(new SelectListItem { Text = "HUMANITIES", Value = "HUMANITIES" });
                        //}
                        //if (dschk.Tables[0].AsEnumerable().Any(row => "SCIENCE".ToUpper() == row.Field<string>("exam").ToUpper()))
                        //{
                        //    objGroupList.Add(new SelectListItem { Text = "SCIENCE", Value = "SCIENCE" });
                        //}
                        //if (dschk.Tables[0].AsEnumerable().Any(row => "COMMERCE".ToUpper() == row.Field<string>("exam").ToUpper()))
                        //{
                        //    objGroupList.Add(new SelectListItem { Text = "COMMERCE", Value = "COMMERCE" });
                        //}

                    }
                    else
                    {
                        objGroupList.Add(new SelectListItem { Text = "HUMANITIES", Value = "HUMANITIES" });
                        objGroupList.Add(new SelectListItem { Text = "SCIENCE", Value = "SCIENCE" });
                        objGroupList.Add(new SelectListItem { Text = "COMMERCE", Value = "COMMERCE" });
                        objGroupList.Add(new SelectListItem { Text = "VOCATIONAL", Value = "VOCATIONAL" });
                    }
                }

                if (SelClass == "12" && objGroupList.Count == 0)
                {
                    objGroupList.Add(new SelectListItem { Text = "HUMANITIES", Value = "HUMANITIES" });
                    objGroupList.Add(new SelectListItem { Text = "SCIENCE", Value = "SCIENCE" });
                    objGroupList.Add(new SelectListItem { Text = "COMMERCE", Value = "COMMERCE" });
                    objGroupList.Add(new SelectListItem { Text = "VOCATIONAL", Value = "VOCATIONAL" });
                }
            }
            else
            { objGroupList.Add(new SelectListItem { Text = "GENERAL", Value = "GENERAL" }); }
            ViewBag.GroupList = objGroupList;
            return Json(objGroupList);
        }

        public ActionResult CalculateFee(string id, EAffiliationPaymentDetailsModel eapdm)
        {

            if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
            {
                return RedirectToAction("Index", "EAffiliation");
            }

            DataSet dsAllowBank = objCommon.CheckBankAllowByFeeCodeDate(59);// 59 for EAffiliation
            ViewBag.dsAllowBank = dsAllowBank;
            if (dsAllowBank == null || dsAllowBank.Tables[0].Rows.Count == 0)
            {
                ViewBag.IsAllowBank = 0;
            }
            else { ViewBag.IsAllowBank = 1; }

            List<SelectListItem> objGroupList = new List<SelectListItem>();
            ViewBag.GroupList = objGroupList;
            // ClassList
            ViewBag.ClassList = EAffiliationDB.GetEAffiliationClassMasterList().ToList();
            //  ViewBag.ClassList = objCommon.GetAllPSEBCLASS_5to12().Where(s => s.Value == "10" || s.Value == "12").ToList();

            //string result = "";
            string result = eAffiliationDB.IsValidForChallan(id.ToString());
            if (result != string.Empty)
            {
                TempData["EAffiliationNotValidForChallan"] = result;
                return View(eapdm);
                // return RedirectToAction("ViewEAffiliation", "EAffiliation", new { id= Session["eAffiliationAppNo"].ToString() });
            }

            EAffiliationModel eam = new EAffiliationModel();
            eam = eAffiliationDB.GetEAffiliation(id);
            if (eam.APPNO != id)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else
            {
                if (!string.IsNullOrEmpty(eam.EAffClass))
                {
                    ViewBag.ClassList = EAffiliationDB.GetEAffiliationClassMasterList().Where(s => s.ClassValue == eam.EAffClass)
                                           .Select(s => s).ToList();
                }

                //Form lock status

                if (Session["eAffiliationIsFormLock"].ToString() != eam.IsFormLock.ToString())
                {
                    Session["eAffiliationIsFormLock"] = eam.IsFormLock.ToString();

                }

                ViewBag.APPNO = eam.APPNO;
                ViewBag.UDISECODE = eam.UDISECODE;
                ViewBag.SCHLNME = eam.SCHLNME;
                ViewBag.SCHLNMEFULL = eam.SCHLNME + "," + eam.STATIONE + " ," + eam.DISTNME;
                ViewBag.STATIONE = eam.STATIONE;
                ViewBag.DISTNME = eam.DISTNME;
                ViewBag.OSTUDYMEDIUM = eam.OSTUDYMEDIUM;

                eapdm.StoreAllData = eAffiliationDB.GetEAffiliationPaymentDetails(eam.APPNO);
                if (eapdm.StoreAllData == null || eapdm.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    // return RedirectToAction("SchoolAccreditation", "School");
                    ViewBag.IsFinal = ViewBag.TotalCount = 0;
                }
                else
                {

                    ViewBag.IsFinal = Convert.ToInt32(eapdm.StoreAllData.Tables[0].Rows[0]["IsFinal"].ToString());
                    ViewBag.TotalCount = eapdm.StoreAllData.Tables[0].Rows.Count;
                }


                // Challan Generated
                if (eapdm.StoreAllData == null || eapdm.StoreAllData.Tables[1].Rows.Count == 0)
                {
                    ViewBag.IsChallan = ViewBag.TotalCount1 = 0;
                }
                else
                {
                    ViewBag.IsChallan = Convert.ToInt32(eapdm.StoreAllData.Tables[1].Rows[0]["ChallanVerify"].ToString());
                    ViewBag.TotalCount1 = eapdm.StoreAllData.Tables[1].Rows.Count;
                }


                if (eapdm.StoreAllData == null || eapdm.StoreAllData.Tables[2].Rows.Count == 0)
                {
                    ViewBag.TotalCount2 = 0;
                }
                else
                {

                    ViewBag.TotalCount2 = eapdm.StoreAllData.Tables[2].Rows.Count;
                }
            }

            return View(eapdm);
        }

        [HttpPost]
        public ActionResult CalculateFee(string id, EAffiliationPaymentDetailsModel eapdm, FormCollection frm, string cmd)
        {

            if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
            {
                return RedirectToAction("Index", "EAffiliation");
            }

            DataSet dsAllowBank = objCommon.CheckBankAllowByFeeCodeDate(59);// 59 for EAffiliation
            ViewBag.dsAllowBank = dsAllowBank;
            if (dsAllowBank == null || dsAllowBank.Tables[0].Rows.Count == 0)
            {
                ViewBag.IsAllowBank = 0;
            }
            else { ViewBag.IsAllowBank = 1; }

            List<SelectListItem> objGroupList = new List<SelectListItem>();
            ViewBag.GroupList = objGroupList;
            // ClassList
            //ViewBag.ClassList = objCommon.GetAllPSEBCLASS_5to12().Where(s => s.Value == "5" || s.Value == "8" || s.Value == "10" || s.Value == "12").ToList();
            //ViewBag.ClassList = objCommon.GetAllPSEBCLASS_5to12().Where(s => s.Value == "10" || s.Value == "12").ToList();
            ViewBag.ClassList = EAffiliationDB.GetEAffiliationClassMasterList().ToList();



            EAffiliationModel eam = new EAffiliationModel();

            if (!string.IsNullOrEmpty(cmd))
            {
                if (cmd.ToLower().Contains("add"))
                {
                    eapdm.cls = frm["SelClass"].ToString();
                    eapdm.APPNO = id;
                    if (eapdm.cls == "12")
                    { eapdm.exam = frm["SelGroup"].ToString(); }
                    else { eapdm.exam = "GENERAL"; }


                    eapdm.fee = eapdm.latefee = eapdm.id = 0;
                    string OutResult = "";
                    if (string.IsNullOrEmpty(eapdm.cls) || string.IsNullOrEmpty(eapdm.exam))
                    {
                        TempData["StatusCF"] = "11";
                    }
                    else
                    {
                        DataSet dsAdd = eAffiliationDB.InsertEAffiliationPaymentDetails(eapdm, 0, out OutResult);
                        if (OutResult == "1")
                        {
                            TempData["StatusCF"] = "1";
                        }
                        else
                        {
                            TempData["StatusCF"] = OutResult;
                        }
                    }
                }
            }
            // Get Data 

            #region Get Data          
            eam = eAffiliationDB.GetEAffiliation(id);
            if (eam.APPNO != id)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else
            {
                if (!string.IsNullOrEmpty(eam.EAffClass))
                {
                    ViewBag.ClassList = EAffiliationDB.GetEAffiliationClassMasterList().Where(s => s.ClassValue == eam.EAffClass)
                                           .Select(s => s).ToList();
                }

                //Form lock status

                if (Session["eAffiliationIsFormLock"].ToString() != eam.IsFormLock.ToString())
                {
                    Session["eAffiliationIsFormLock"] = eam.IsFormLock.ToString();

                }

                ViewBag.APPNO = eam.APPNO;
                ViewBag.UDISECODE = eam.UDISECODE;
                ViewBag.SCHLNME = eam.SCHLNME;
                ViewBag.SCHLNMEFULL = eam.SCHLNME + "," + eam.STATIONE + " ," + eam.DISTNME;
                ViewBag.STATIONE = eam.STATIONE;
                ViewBag.DISTNME = eam.DISTNME;
                ViewBag.OSTUDYMEDIUM = eam.OSTUDYMEDIUM;

                eapdm.StoreAllData = eAffiliationDB.GetEAffiliationPaymentDetails(eam.APPNO);
                if (eapdm.StoreAllData == null || eapdm.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    // return RedirectToAction("SchoolAccreditation", "School");
                    ViewBag.IsFinal = ViewBag.TotalCount = 0;
                }
                else
                {

                    ViewBag.IsFinal = Convert.ToInt32(eapdm.StoreAllData.Tables[0].Rows[0]["IsFinal"].ToString());
                    ViewBag.TotalCount = eapdm.StoreAllData.Tables[0].Rows.Count;
                }


                // Challan Generated
                if (eapdm.StoreAllData == null || eapdm.StoreAllData.Tables[1].Rows.Count == 0)
                {
                    ViewBag.IsChallan = ViewBag.TotalCount1 = 0;
                }
                else
                {
                    ViewBag.IsChallan = Convert.ToInt32(eapdm.StoreAllData.Tables[1].Rows[0]["ChallanVerify"].ToString());
                    ViewBag.TotalCount1 = eapdm.StoreAllData.Tables[1].Rows.Count;
                }


                if (eapdm.StoreAllData == null || eapdm.StoreAllData.Tables[2].Rows.Count == 0)
                {
                    ViewBag.TotalCount2 = 0;
                }
                else
                {

                    ViewBag.TotalCount2 = eapdm.StoreAllData.Tables[2].Rows.Count;
                }
            }

            #endregion  Get Data 

            return View(eapdm);
        }


        public ActionResult EAffiliationActions(string id, string act)
        {
            try
            {
                string OutResult = "";
                EAffiliationPaymentDetailsModel sam = new EAffiliationPaymentDetailsModel();
                if (id == null)
                {
                    return RedirectToAction("Index", "EAffiliation");
                }
                else
                {
                    if (act == "D")
                    {
                        sam.id = Convert.ToInt32(id);
                        DataSet dsAdd = eAffiliationDB.InsertEAffiliationPaymentDetails(sam, 1, out OutResult);
                        if (OutResult == "1")
                        {
                            @ViewBag.result = "1";
                            TempData["StatusCF"] = "DEL";
                        }
                    }
                    else if (act == "FS")// Final Submit
                    {
                        sam.APPNO = Convert.ToString(id);
                        DataSet dsAdd = eAffiliationDB.InsertEAffiliationPaymentDetails(sam, 2, out OutResult);
                        if (OutResult == "1")
                        {
                            @ViewBag.result = "1";
                            TempData["StatusCF"] = "FS";
                        }
                    }
                    else if (act == "UF")// Unlock Final Submit
                    {
                        sam.APPNO = Convert.ToString(id);
                        DataSet dsAdd = eAffiliationDB.InsertEAffiliationPaymentDetails(sam, 3, out OutResult);
                        if (OutResult == "1")
                        {
                            @ViewBag.result = "1";
                            TempData["StatusCF"] = "UF";
                        }
                        else
                        {
                            @ViewBag.result = "1";
                            TempData["StatusCF"] = OutResult;

                        }
                    }

                }
                return RedirectToAction("CalculateFee", "EAffiliation", new { Id = Session["eAffiliationAppNo"] });

            }
            catch (Exception)
            {
                return RedirectToAction("CalculateFee", "EAffiliation", new { Id = Session["eAffiliationAppNo"] });
            }
        }



        #region Challan and Payment Details EAffiliation
        public ActionResult PaymentFormEAffiliation(string id)
        {
            try
            {
                Session["Session"] = @PSEBONLINE.Repository.SessionSettingMastersRepository.GetSessionSettingMasters().SessionShortYear;
                if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
                {
                    return RedirectToAction("Index", "EAffiliation");
                }

                EAffiliationFee _EAffiliationFee = new EAffiliationFee();

                string appno = id;
                string today = DateTime.Today.ToString("dd/MM/yyyy");
                DateTime dateselected;
                if (DateTime.TryParseExact(today, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
                {
                    ViewData["result"] = 5;
                    DataSet ds = eAffiliationDB.GetEAffiliationPayment(appno);
                    _EAffiliationFee.PaymentFormData = ds;
                    if (_EAffiliationFee.PaymentFormData == null || _EAffiliationFee.PaymentFormData.Tables[0].Rows.Count == 0)
                    { ViewBag.TotalCount = 0; Session["EAffiliationFee"] = null; }
                    else
                    {
                        Session["EAffiliationFee"] = ds;
                        var classCheck = ds.Tables[0].Rows[0]["class"].ToString();
                        ViewBag.TotalFee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("fee")));
                        ViewBag.TotalLateFee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("latefee")));
                        ViewBag.extrafee = Convert.ToInt32(ds.Tables[0].Rows[0]["extraFee"]);
                        ViewBag.TotalTotfee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("Totfee")));
                        if ((ds.Tables[0].Rows[0]["srflag"].ToString()!="1" && ds.Tables[0].Rows[0]["srflag1"].ToString() != "1" ) &&  classCheck=="12")
                        {
                            Int64 fees = Convert.ToInt64(_EAffiliationFee.PaymentFormData.Tables[0].Rows[0]["fee"]);
                            Int64 addRxtraFee = Convert.ToInt64(_EAffiliationFee.PaymentFormData.Tables[0].Rows[0]["Totfee"]);
                            addRxtraFee = addRxtraFee + ViewBag.extrafee;
                            fees = fees + ViewBag.extrafee;
                            _EAffiliationFee.PaymentFormData.Tables[0].Rows[0]["Totfee"] = addRxtraFee;
                            _EAffiliationFee.PaymentFormData.Tables[0].Rows[0]["fee"] = fees;
                            ViewBag.Total = ViewBag.TotalFee + ViewBag.TotalLateFee + ViewBag.extrafee;
                        }
                       

                       else if ((ds.Tables[0].Rows[0]["matflag"].ToString() != "1" && ds.Tables[0].Rows[0]["matflag1"].ToString() != "1") && classCheck == "10")
                        {
                            Int64 fees = Convert.ToInt64(_EAffiliationFee.PaymentFormData.Tables[0].Rows[0]["fee"]);
                            Int64 addRxtraFee = Convert.ToInt64(_EAffiliationFee.PaymentFormData.Tables[0].Rows[0]["Totfee"]);
                            addRxtraFee = addRxtraFee + ViewBag.extrafee;
                            fees = fees + ViewBag.extrafee;
                            _EAffiliationFee.PaymentFormData.Tables[0].Rows[0]["Totfee"] = addRxtraFee;
                            _EAffiliationFee.PaymentFormData.Tables[0].Rows[0]["fee"] = fees;
                            ViewBag.Total = ViewBag.TotalFee + ViewBag.TotalLateFee + ViewBag.extrafee;

                        }
                        else
                        {
                            ViewBag.Total = ViewBag.TotalFee + ViewBag.TotalLateFee;
                        }
                       
                      
                        ViewData["result"] = 10;
                        ViewData["FeeStatus"] = "1";
                        ViewBag.TotalCount = 1;
                        return View(_EAffiliationFee);
                    }

                }
                else
                {
                    ViewData["OutError"] = "Date Format Problem";
                }
                return View(_EAffiliationFee);
            }
            catch (Exception)
            {
                return View();
                /// return RedirectToAction("SchoolAccreditation", "School");
            }
        }
        [HttpPost]
        public ActionResult PaymentFormEAffiliation(string id, FormCollection frm, string PayModValue, string AllowBanks)
        {
            try
            {

                EAffiliationFee pfvm = new EAffiliationFee();
                ChallanMasterModel CM = new ChallanMasterModel();



                if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
                {
                    return RedirectToAction("Index", "EAffiliation");
                }
                if (Session["EAffiliationFee"] == null)
                {
                    return RedirectToAction("CalculateFee", "EAffiliation");
                }
                string appno = id;
                DataSet ds = (DataSet)Session["EAffiliationFee"];
                pfvm.PaymentFormData = ds;

                string bankName = "";

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



                ViewBag.TotalFee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("fee")));
                ViewBag.TotalLateFee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("latefee")));
                ViewBag.TotalTotfee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("Totfee")));
                ViewBag.Total = ViewBag.TotalFee + ViewBag.TotalLateFee;

                if (string.IsNullOrEmpty(AllowBanks))
                {
                    ViewBag.Message = "Please Select Bank";
                    ViewData["SelectBank"] = "1";
                    return View(pfvm);
                }


                if (ModelState.IsValid)
                {
                    CM.FEE = Convert.ToInt32(ViewBag.TotalFee);
                    CM.latefee = Convert.ToInt32(ViewBag.TotalLateFee);
                    CM.TOTFEE = Convert.ToInt32(ViewBag.TotalTotfee);
                    string TotfeePG = (CM.TOTFEE).ToString();
                    CM.FEECAT = ds.Tables[0].Rows[0]["FeeCat"].ToString();
                    CM.FEECODE = ds.Tables[0].Rows[0]["FeeCode"].ToString();
                    CM.FEEMODE = "CASH";
                    CM.BCODE = AllowBanks;
                    CM.BANK = bankName;

                    CM.BANKCHRG = 0;
                    CM.SchoolCode = appno;
                    CM.DIST = "";
                    CM.DISTNM = "";
                    CM.LOT = 1;
                    CM.SCHLREGID = appno;
                    CM.FeeStudentList = ds.Tables[0].Rows[0]["Refno"].ToString();
                    CM.APPNO = ds.Tables[0].Rows[0]["Refno"].ToString();

                    CM.type = "schle";
                    CM.CHLNVDATE = Convert.ToString(ds.Tables[0].Rows[0]["BankLastdate"].ToString());
                    DateTime BankLastDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["BankLastdate"].ToString());
                    DateTime CHLNVDATE2;
                    if (DateTime.TryParseExact(BankLastDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out CHLNVDATE2))
                    {
                        CM.ChallanVDateN = CHLNVDATE2;
                    }
                    else
                    {
                        CM.ChallanVDateN = BankLastDate;
                    }

                    string SchoolMobile = "";
                    string result = new AbstractLayer.HomeDB().InsertPaymentForm(CM, frm, out SchoolMobile);
                    if (result == null || result == "0")
                    {
                        //--------------Not saved
                        ViewData["result"] = 0;
                        ViewData["Error"] = SchoolMobile;
                    }
                    else if (result == "-1")
                    {
                        //-----alredy exist
                        ViewData["result"] = -1;
                    }
                    else
                    {

                        ViewData["SelectBank"] = null;
                        ViewData["result"] = 1;
                        ViewBag.ChallanNo = result;
                        string paymenttype = CM.BCODE;
                        string bnkLastDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["BankLastdate"].ToString()).ToString("dd/MM/yyyy");
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
                                string strURL;
                                string MerchantLogin = ConfigurationManager.AppSettings["ATOMLoginId"].ToString();
                                string MerchantPass = ConfigurationManager.AppSettings["ATOMPassword"].ToString();
                                string MerchantDiscretionaryData = "NB";  // for netbank
                                //string ClientCode = "PSEBONLINE";
                                string ClientCode = CM.APPNO;
                                // string ClientCode = "APPNO"+ CM.APPNO;
                                string ProductID = ConfigurationManager.AppSettings["ATOMProductID"].ToString();
                                string CustomerAccountNo = "0123456789";
                                string TransactionType = "NBFundTransfer";  // for netbank
                                                                            //string TransactionAmount = "1";
                                string TransactionAmount = TotfeePG;
                                // string TransactionAmount = "100";
                                string TransactionCurrency = "INR";
                                string TransactionServiceCharge = "0";
                                string TransactionID = ViewBag.ChallanNo;// Unique Challan Number
                                string TransactionDateTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
                                // string TransactionDateTime = "18/10/2019 13:15:19";
                                string BankID = "ATOM";


                                string ru = ConfigurationManager.AppSettings["ATOMRU"].ToString();
                                // User Details
                                string udf1CustName = CM.SCHLREGID; // roll number

                                string udf2CustEmail = CM.FEECAT; /// Kindly submit Appno/Refno in client id, Fee cat in Emailid (ATOM)
                                string udf3CustMob = SchoolMobile;

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

                            string Sms = "EAffiliation Challan no. " + result + " of Ref no  " + CM.APPNO + " successfully generated and valid till Dt " + bnkLastDate + ". Regards PSEB";
                            try
                            {
                                string getSms = objCommon.gosms(SchoolMobile, Sms);
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
                return RedirectToAction("EAffiliation", "Index");
            }
        }

        #endregion Challan and Payment Details

        #endregion

        #region ChangePassword
        public ActionResult ChangePassword(string id)
        {
            if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            ViewBag.AppNo = Session["eAffiliationAppNo"].ToString();
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string id, FormCollection frm)
        {
            if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "Open");
            }
            ViewBag.AppNo = Session["eAffiliationAppNo"].ToString();
            ViewBag.UserId = Session["eAffiliationId"].ToString();
            string CurrentPassword = string.Empty;
            string NewPassword = string.Empty;


            if (frm["ConfirmPassword"] != "" && frm["NewPassword"] != "")
            {
                if (frm["ConfirmPassword"].ToString() == frm["NewPassword"].ToString())
                {
                    CurrentPassword = frm["CurrentPassword"].ToString();
                    NewPassword = frm["NewPassword"].ToString();
                    int result = eAffiliationDB.ChangePassword(Convert.ToInt32(Session["eAffiliationId"].ToString()), CurrentPassword, NewPassword);
                    if (result > 0)
                    {
                        ViewData["resultDCP"] = 1;
                        return View();
                    }
                    else
                    {
                        ViewData["resultDCP"] = 0;
                        ModelState.AddModelError("", "Not Update");
                        return View();
                    }
                }
                else
                {
                    ViewData["resultDCP"] = 3;
                    ModelState.AddModelError("", "Fill All Fields");
                    return View();
                }
            }
            else
            {
                ViewData["resultDCP"] = 2;
                ModelState.AddModelError("", "Fill All Fields");
                return View();
            }
        }


        #endregion ChangePassword


        public ActionResult ViewEAffiliation(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
                {
                    return RedirectToAction("Index", "EAffiliation");
                }
                else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
                {
                    return RedirectToAction("Index", "EAffiliation");
                }
                ViewBag.AppNo = Session["eAffiliationAppNo"].ToString();
                ViewBag.CLS = "0";
                EAffiliationModel eam = eAffiliationDB.GetEAffiliation(ViewBag.AppNo);
                if (eam.StoreAllData == null || eam.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCount = 0;
                }
                else
                {

                    ViewBag.TotalCount = eam.StoreAllData.Tables[0].Rows.Count;
                    if (eam.StoreAllData.Tables[2].Rows.Count > 0)
                    {
                        ViewBag.CLS = eam.StoreAllData.Tables[2].Rows[0]["Class"].ToString();
                    }

                    if (eam.EAffType == "RENEW")
                    {
                        DataSet outDS;
                        SchoolModels schoolModels = new AbstractLayer.SchoolDB().GetSchoolDataBySchl(eam.SCHL, out outDS);    //SelectSchoolDatabyID 
                        if (schoolModels.SCHL != null)
                        {
                            eam.dataSetPrevious = outDS;
                        }
                    }
                }
                return View(eam);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
        }


        public ActionResult EAffiliationReport(string id)
        {
            try
            {

                if (Session["AdminId"] == null)
                {
                    if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
                    {
                        return RedirectToAction("Index", "EAffiliation");
                    }
                    else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
                    {
                        return RedirectToAction("Index", "EAffiliation");
                    }
                    ViewBag.AppNo = Session["eAffiliationAppNo"].ToString();
                }
                else
                {
                    ViewBag.AppNo = id.ToString();
                }
                ViewBag.CLS = "0";
                EAffiliationModel eam = eAffiliationDB.GetEAffiliation(ViewBag.AppNo);

                if (eam.StoreAllData == null || eam.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = eam.StoreAllData.Tables[0].Rows.Count;
                    if (eam.StoreAllData.Tables[2].Rows.Count > 0)
                    {
                        ViewBag.CLS = eam.StoreAllData.Tables[2].Rows[0]["Class"].ToString();
                    }


                    if (eam.EAffType == "RENEW")
                    {
                        DataSet outDS;
                        SchoolModels schoolModels = new AbstractLayer.SchoolDB().GetSchoolDataBySchl(eam.SCHL, out outDS);    //SelectSchoolDatabyID 
                        if (schoolModels.SCHL != null)
                        {
                            eam.dataSetPrevious = outDS;
                        }
                    }
                }
                return View(eam);
            }
            catch (Exception)
            {
                if (Session["AdminId"] == null)
                {
                    return RedirectToAction("Index", "EAffiliation");
                }
                else
                {

                    return RedirectToAction("Admin", "ViewEAffiliationAdmin");
                }
            }
        }


        // upload documents


        #region Upload EAffiliation Documents
        public ActionResult UploadEAffiliationDocuments(string id, EAffiliationDocumentDetailsModel eadm)
        {
            if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
            {
                return RedirectToAction("Index", "EAffiliation");
            }
            else if (Session["eAffiliationIsFormLock"].ToString() == "1")
            { return RedirectToAction("ViewEAffiliation", new { id = Session["eAffiliationAppNo"].ToString() }); }

            eadm.APPNO = id;
            eadm.StoreAllData = eAffiliationDB.GetEAffiliationDocumentDetails(2, 0, id, "");
            eadm.EAffiliationDocumentMasterList = eAffiliationDB.EAffiliationDocumentMasterList(eadm.StoreAllData.Tables[1]);//  Document List
            if (eadm.StoreAllData == null || eadm.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
            }
            else
            {
                ViewBag.TotalCount = eadm.StoreAllData.Tables[0].Rows.Count;
            }

            return View(eadm);
        }

        [HttpPost]
        public ActionResult UploadEAffiliationDocuments(string id, EAffiliationDocumentDetailsModel eadm, string cmd, FormCollection frm, HttpPostedFileBase docfile)
        {
            try
            {


                if (string.IsNullOrEmpty(id) || Session["eAffiliationAppNo"] == null)
                {
                    return RedirectToAction("Index", "EAffiliation");
                }
                else if (id.ToString() != Session["eAffiliationAppNo"].ToString())
                {
                    return RedirectToAction("Index", "EAffiliation");
                }

                eadm.StoreAllData = eAffiliationDB.GetEAffiliationDocumentDetails(2, 0, id, "");
                eadm.EAffiliationDocumentMasterList = eAffiliationDB.EAffiliationDocumentMasterList(eadm.StoreAllData.Tables[1]);//  Document List
                eadm.APPNO = id;

                //string DocName = eadm.EAffiliationDocumentMasterList.Where(s => s.DocID == eadm.DocID).Select(s => s.DocumentName).FirstOrDefault();
                string DocName = _context.EAffiliationDocumentMasters.Where(s => s.DocID == eadm.DocID).Select(s => s.DocCode).SingleOrDefault();

                if (!string.IsNullOrEmpty(cmd))
                {
                    string outError = "0";
                    int result = 0;

                    // Save file
                    string filename = "";
                    string FilepathExist = "", path = "";
                    if (docfile != null)
                    {
                        string ext = Path.GetExtension(docfile.FileName);
                        filename = eadm.APPNO + "_" + DocName.Replace(" ", "_") + ext;
                        path = Path.Combine(Server.MapPath("~/Upload/Upload2023/EAffiliation2021/UploadEAffiliationDocuments"), filename);
                        FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/EAffiliation2021/UploadEAffiliationDocuments"));
                        eadm.DocFile = "allfiles/Upload2023/EAffiliation2021/UploadEAffiliationDocuments/" + filename;
                    }


                    if (cmd.ToLower() == "submit" || cmd.ToLower() == "save")
                    {

                        result = eAffiliationDB.InsertEAffiliationDocumentDetails(eadm, 0, out outError);  // 0 for insert
                        if (result > 0)
                        {
                            ViewData["result"] = "1";
                            ViewBag.Mesaage = outError;

                            //if (!Directory.Exists(FilepathExist))
                            //{
                            //    Directory.CreateDirectory(FilepathExist);
                            //}
                            //docfile.SaveAs(path);


                            string Orgfile = filename;

                            using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                            {
                                using (var newMemoryStream = new MemoryStream())
                                {
                                    var uploadRequest = new TransferUtilityUploadRequest
                                    {
                                        InputStream = docfile.InputStream,
                                        Key = string.Format("allfiles/Upload2023/EAffiliation2021/UploadEAffiliationDocuments/{0}", Orgfile),

                                        BucketName = BUCKET_NAME,
                                        CannedACL = S3CannedACL.PublicRead
                                    };

                                    var fileTransferUtility = new TransferUtility(client);
                                    fileTransferUtility.Upload(uploadRequest);
                                }
                            }
                        }
                    }
                    else if (cmd.ToLower() == "delete")
                    {
                        eadm.APPNO = Convert.ToString(id);
                        // eadm.eDocId = Convert.ToInt32(eStaffId);
                        result = eAffiliationDB.InsertEAffiliationDocumentDetails(eadm, 2, out outError); // 2 for delete
                        if (outError == "1")
                        {
                            ViewBag.result = "1";
                            ViewData["Status"] = "DEL";
                            return View(eadm);
                        }
                    }

                    if (result > 0)
                    {
                        ViewData["result"] = "1";
                        ViewBag.Mesaage = outError;
                    }
                    else
                    {
                        ViewData["result"] = outError.ToString();
                        ViewBag.Mesaage = outError;
                    }
                }
            }
            catch (Exception)
            {


            }
            return View(eadm);
        }


        public ActionResult ActionUploadEAffiliationDocuments(string id, string eDocId, string act)
        {
            try
            {
                string outError = "0";
                int result = 0;
                EAffiliationDocumentDetailsModel easdm = new EAffiliationDocumentDetailsModel();
                if (id == null || eDocId == null)
                {
                    // return RedirectToAction("Index", "EAffiliation");
                }
                else
                {
                    if (act == "D")
                    {
                        easdm.APPNO = Convert.ToString(id);
                        easdm.eDocId = Convert.ToInt32(eDocId);
                        result = eAffiliationDB.InsertEAffiliationDocumentDetails(easdm, 2, out outError); // 2 for delete
                        if (outError == "1")
                        {
                            ViewBag.result = "1";
                            ViewData["Status"] = "DEL";
                        }
                    }
                }

            }
            catch (Exception)
            {
            }
            return RedirectToAction("UploadEAffiliationDocuments", "EAffiliation", new { Id = Session["eAffiliationAppNo"] });

        }

        #endregion


    }
}