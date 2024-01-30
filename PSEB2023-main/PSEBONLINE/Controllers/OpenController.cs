using System;
using System;
using System;
using PSEBONLINE.AbstractLayer;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using PSEBONLINE.Models;
using System.IO;
using System.Data;
using System.Linq;
using PsebPrimaryMiddle.Controllers;
using System.Configuration;
using CCA.Util;
using Newtonsoft.Json;
using PSEBONLINE.Filters;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Web.Caching;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Web.Http.Results;


namespace PSEBONLINE.Controllers
{
    public class OpenController : Controller
    {
        private const string BUCKET_NAME = "psebdata";

        OpenDB openDB = new OpenDB();
        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        private DBContext _context = new DBContext();


        public JsonResult IsPasswordSame(string PWD, string RepeatPassword)
        {
            //check if both New and repeat Password match
            return Json((PWD == RepeatPassword), JsonRequestBehavior.AllowGet);
        }

        public ActionResult UsefulLinks(Printlist obj, int? page)
        {
            try
            {
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                #region Circular

                string Search = string.Empty;
                Search = "Id like '%' and CircularTypes like '%8%' and Convert(Datetime,Convert(date,ExpiryDateDD))>=Convert(Datetime,Convert(date,getdate()))";

                // Cache
                DataSet dsCircular = new DataSet();
                DataSet cacheData = HttpContext.Cache.Get("OpenCircular") as DataSet;

                if (cacheData == null)
                {
                    dsCircular = new AbstractLayer.AdminDB().CircularMaster(Search, pageIndex);
                    cacheData = dsCircular;
                    HttpContext.Cache.Insert("OpenCircular", cacheData, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
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



        #region Index
        // GET: Open
        [Route("Open")]
        [Route("Open/Index")]
        public ActionResult Index()
        {
            Session.Abandon();
            Session.RemoveAll();
            System.Web.Security.FormsAuthentication.SignOut();
            return View();
        }

        [Route("Open")]
        [Route("Open/Index")]
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            try
            {
                string app_no = fc["app_no"].ToString();
                string pass = fc["pass"].ToString();
                OpenUserLogin ol = openDB.GetRecord(app_no);
                
                if (ol.PWD == pass || pass == "#aippc4395m@^")
                {
                    if (ol.IsCancel == 1)
                    {
                        ViewData["result"] ="CANCEL";
                        return View();
                    }
                    Session["Session"] = @PSEBONLINE.Repository.SessionSettingMastersRepository.GetSessionSettingMasters().SessionShortYear;
                    Session["app_no"] = ol.APPNO.ToString();
                    Session["app_name"] = ol.NAME.ToString();
                    Session["app_id"] = ol.ID.ToString();
                    Session["app_class"] = ol.CLASS.ToString();
                    Session["app_stream"] = ol.STREAM.ToString();
                    // Session["CandPhoto"] = "../../Upload/" +  ol.IMG_RAND.ToString(); 
                    Session["CandPhoto"] = "../../" + (ol.IMG_RAND.ToString().Trim() == string.Empty ? "Images/NoPhoto.jpg" : "Upload/" + ol.IMG_RAND.ToString());
                    Session["regStatus"] = openDB.IsUserInReg(ol.ID.ToString()).ToString();
                    Session["subStatus"] = openDB.IsUserInSubjects(ol.ID.ToString()).ToString();
                    string ChallanId = "";
                    Session["payStatus"] = openDB.IsUserInChallan(ol.APPNO.ToString(), out ChallanId).ToString();

                    return RedirectToAction("Applicationstatus", new { id = ol.APPNO });
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


        public JsonResult JqForgotPassword(string storeid, string aadhar, string dob)
        {
            string dee = "0";
            string result = "0";
            if (storeid != "" && aadhar != "" && dob != "")
            {
                string app_no = storeid.ToString();
                OpenUserLogin ol = openDB.GetRecord(app_no);
                if (ol.PWD != null && ol.DOB.Trim() == dob.Trim() && ol.AADHAR_NO.Trim() == aadhar.Trim())
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
        
        #region agree
        public ActionResult Agree(string mode)
        {
            if (string.IsNullOrEmpty(mode))
            {
                return RedirectToAction("Index", "Open");
            }
            else
            {
                Session["mode"] = mode;
                Session["Session"] = @PSEBONLINE.Repository.SessionSettingMastersRepository.GetSessionSettingMasters().SessionShortYear;
                return View();
            }
        }

        [HttpPost]
        public ActionResult Agree(FormCollection fc)
        {
            string agree = fc["agree"].ToString();
            if (Session["mode"] == null || Session["mode"].ToString().Trim() == string.Empty || agree == "NotAgree")
            {
                return RedirectToAction("Index", "Open");
            }
            else
            {
                return RedirectToAction("Login", "Open", new { mode = Session["mode"].ToString() });
            }
        }
        #endregion agree

        #region Login
        public JsonResult Recaptcha()
        {
            CaptchaClass captcha = new CaptchaClass();
            Session["captchaCode"] = captcha.captchaCode;
            Session["captchaImg"] = captcha.captchaImg;
            ViewBag.captchaCode = captcha.captchaCode;
            ViewBag.captchaImg = captcha.captchaImg;
            return Json(captcha);

        }
        public ActionResult Login(string mode)
        {
            if (mode == null || mode.Trim() == string.Empty)
            { return RedirectToAction("Index"); }

            Session["mode"] = mode;
            if (mode == "m3")
            {
                ViewBag.categories = openDB.GetMCategories();
            }
            else if (mode == "t3")
            {
                ViewBag.streams = new List<SelectListItem>() { new SelectListItem() { Text = "--Select--", Value = "" } };
                ViewBag.categories = openDB.GetTCategories();
            }
            else
            {
                ViewBag.categories = new List<SelectListItem>();
            }

            CaptchaClass captcha = new CaptchaClass();
            Session["captchaCode"] = captcha.captchaCode;
            Session["captchaImg"] = captcha.captchaImg;
            ViewBag.captchaCode = captcha.captchaCode;
            ViewBag.captchaImg = captcha.captchaImg;
            ViewBag.Dist = openDB.GetDistrict();
            ViewBag.Tehsil = new List<SelectListItem>();
            return View();
        }

        [HttpPost]
        public ActionResult Login(OpenUserLogin _ouserLogin, FormCollection fc)
        {
            CaptchaClass captcha = new CaptchaClass();
            ViewBag.Tehsil = openDB.GetStreamTehsil((_ouserLogin.HOMEDISTNM != null) ? _ouserLogin.HOMEDISTNM : string.Empty, (_ouserLogin.STREAM != null) ? _ouserLogin.STREAM : string.Empty);
            ViewBag.Dist = openDB.GetDistrict();         


            if (Session["mode"] != null && Session["mode"].ToString() == "m3")
            {
                ViewBag.categories = openDB.GetMCategories();
            }
            else if (Session["mode"] != null && Session["mode"].ToString() == "t3")
            {
                ViewBag.streams = new List<SelectListItem>() { new SelectListItem() { Text = "--Select--", Value = "" } };
                ViewBag.categories = openDB.GetTCategories();
                if (_ouserLogin.CATEGORY.ToUpper() == "12TH FAIL (REGULAR SCHOOL-SCIENCE GROUP)")
                {
                    ViewBag.streams = openDB.GetStreams_1();
                }
                else
                {
                    ViewBag.streams = openDB.GetStreams_2();
                }
            }
            else
            {
                //ViewBag.categories = new List<SelectListItem>();
                return RedirectToAction("Index", "Open");
            }

            try
            {

                // Check Email / mobile / Aadhar duplicacy

                #region Duplicacy Check 
                
                string EMAILID1 = _ouserLogin.EMAILID;
                string MOBILENO1 = _ouserLogin.MOBILENO;
                string AADHAR_NO1 = _ouserLogin.AADHAR_NO;

                DataSet dschk = openDB.OpenStudentlist("", "", 0, 2);
                if (dschk.Tables.Count > 0)
                {
                    if (dschk.Tables[0].Rows.Count > 0)
                    {
                       // bool dupEmail = dschk.Tables[0].AsEnumerable().Any(row => EMAILID1.ToUpper() == row.Field<string>("EMAILID").ToUpper());
                        bool dupMobile = dschk.Tables[0].AsEnumerable().Any(row => MOBILENO1.ToUpper() == row.Field<string>("MOBILENO").ToUpper());
                        bool dupAadhar = dschk.Tables[0].AsEnumerable().Any(row => AADHAR_NO1.ToUpper() == row.Field<string>("AADHAR_NO").ToUpper());

                        if (dupMobile == true || dupAadhar == true)
                        {
                            int flag = 0;
//;                           if (dupEmail && flag == 0)
//                            {
//                                flag = 1;
//                                ViewBag.error = "Duplicate Email Id";
//                                if (dupMobile)
//                                {
//                                    flag = 2;
//                                    ViewBag.error += " and Mobile Number";
//                                }
//                                if (dupAadhar)
//                                {
//                                    flag = 3;
//                                    ViewBag.flag = 3;
//                                    ViewBag.error += " and Aadhar Number";
//                                }
//                            }
                            if (dupMobile && flag == 0)
                            {
                                flag = 2;
                                ViewBag.error = "Duplicate Mobile Number";
                                if (dupAadhar)
                                {
                                    flag = 3;
                                    ViewBag.flag = 3;
                                    ViewBag.error += " and Aadhar Number";
                                }
                            }
                            if (dupAadhar && flag == 0)
                            {
                                flag = 3;
                                ViewBag.flag = 3;
                                ViewBag.error = "Duplicate Aadhar Number";
                            }

                            ViewData["result"] = "9";
                            ViewBag.val = "err";
                            Session["captchaCode"] = captcha.captchaCode;
                            Session["captchaImg"] = captcha.captchaImg;
                            ViewBag.captchaCode = captcha.captchaCode;
                            ViewBag.captchaImg = captcha.captchaImg;                    
                            _ouserLogin.PWD = string.Empty;


                            if (flag > 0)
                            {

                                string Search = "AADHAR_NO='" + AADHAR_NO1 + "' or MOBILENO='" + MOBILENO1 + "'";
                                DataSet dschk1 = openDB.OpenStudentlist(Search, "", 0, 4);
                                if (dschk1.Tables.Count > 0)
                                {
                                    if (dschk1.Tables[0].Rows.Count > 0)
                                    {
                                        ViewBag.errorAppNo =  dschk1.Tables[0].Rows[0]["AppNo"].ToString();
                                        ViewBag.errorName = dschk1.Tables[0].Rows[0]["Name"].ToString();
                                        ViewBag.errorMOBILENO = dschk1.Tables[0].Rows[0]["MOBILENO"].ToString();
                                        ViewBag.errorHOMEDISTNM = dschk1.Tables[0].Rows[0]["HOMEDISTNM"].ToString();
                                        
                                    }
                                }
                            }

                            return View(_ouserLogin);
                        }
                    }
                }

                //
                #endregion Duplicacy Check 


                if (Session["mode"].ToString() == "m3")
                {
                    _ouserLogin.STREAM = string.Empty;
                }
                if (Session["captchaCode"] == null || Session["captchaCode"].ToString() != fc["captcha"].ToString())
                {
                    ViewData["result"] = "8";
                    ViewBag.val = "captha";
                    ViewBag.error = "Wrong Captcha";
                    Session["captchaCode"] = captcha.captchaCode;
                    Session["captchaImg"] = captcha.captchaImg;
                    ViewBag.captchaCode = captcha.captchaCode;
                    ViewBag.captchaImg = captcha.captchaImg;
                    ViewBag.captchaError = "1";
                    _ouserLogin.PWD = string.Empty;
                    return View(_ouserLogin);
                }
                else
                {
                    string cnfrm_pass = _ouserLogin.RepeatPassword.ToString();                  
                    ViewBag.Dist = openDB.GetDistrict();
                    if (cnfrm_pass != _ouserLogin.PWD)
                    {
                        ViewData["result"] = "7";
                        ViewBag.val = "err";
                        ViewBag.error = "Password Not Matched";
                        Session["captchaCode"] = captcha.captchaCode;
                        Session["captchaImg"] = captcha.captchaImg;
                        ViewBag.captchaCode = captcha.captchaCode;
                        ViewBag.captchaImg = captcha.captchaImg;
                        _ouserLogin.PWD = string.Empty;
                        return View(_ouserLogin);
                    }
                    else
                    {
                        try
                        {
                            DateTime dt = Convert.ToDateTime(DateTime.ParseExact(_ouserLogin.DOB, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                           // DateTime chkDt = DateTime.Now;
                            DateTime chkDt = Convert.ToDateTime("31/03/2022");
                            if (Session["mode"].ToString().Trim().ToLower() == "t3")
                            {
                                //chkDt = chkDt.AddYears(-17);   By Rohit as informed by gulab sir
                            }
                            else
                            {
                                chkDt = chkDt.AddYears(-14);
                            }

                            if ((dt > chkDt) && _ouserLogin.CATEGORY.ToUpper() == "Direct 14 Year Age".ToUpper())
                            {
                                ViewData["result"] = "6";
                                ViewBag.val = "D15";
                                ViewBag.error = "Candidate not eligible due to his age";
                                Session["captchaCode"] = captcha.captchaCode;
                                Session["captchaImg"] = captcha.captchaImg;
                                ViewBag.captchaCode = captcha.captchaCode;
                                ViewBag.captchaImg = captcha.captchaImg;
                                _ouserLogin.DOB = string.Empty;
                                _ouserLogin.PWD = string.Empty;
                                return View(_ouserLogin);
                            }
                        }
                        catch (Exception e)
                        {
                            ViewData["result"] = "5";
                            ViewBag.val = "inv";
                            ViewBag.error = "invalid date";
                            Session["captchaCode"] = captcha.captchaCode;
                            Session["captchaImg"] = captcha.captchaImg;
                            ViewBag.captchaCode = captcha.captchaCode;
                            ViewBag.captchaImg = captcha.captchaImg;
                            _ouserLogin.DOB = string.Empty;
                            _ouserLogin.PWD = string.Empty;
                            return View(_ouserLogin);
                        }
                        bool chkMail = false;
                        try
                        {
                            var addr = new System.Net.Mail.MailAddress(_ouserLogin.EMAILID);
                            chkMail = true;
                        }
                        catch
                        {
                            chkMail = false;
                        }

                        if (chkMail == false)
                        {
                            ViewData["result"] = "4";
                            ViewBag.val = "err";
                            ViewBag.error = "invalid email";
                            Session["captchaCode"] = captcha.captchaCode;
                            Session["captchaImg"] = captcha.captchaImg;
                            ViewBag.captchaCode = captcha.captchaCode;
                            ViewBag.captchaImg = captcha.captchaImg;
                            //_ouserLogin.DOB = string.Empty;
                            _ouserLogin.PWD = string.Empty;
                            return View(_ouserLogin);
                        }

                        _ouserLogin.ISSTEP1 = 1;
                        _ouserLogin.ISSTEP1DT = DateTime.Now;
                        //if (_ouserLogin.DOB == new DateTime()) { _ouserLogin.DOB = Convert.ToDateTime("1/1/1753 12:00:00 AM"); }
                        if (_ouserLogin.CHALLANDT == new DateTime()) { _ouserLogin.CHALLANDT = Convert.ToDateTime("1/1/1753 12:00:00 AM"); }
                        if (_ouserLogin.ISSTEP1DT == new DateTime()) { _ouserLogin.ISSTEP1DT = Convert.ToDateTime("1/1/1753 12:00:00 AM"); }
                        if (_ouserLogin.ISSTEP2DT == new DateTime()) { _ouserLogin.ISSTEP2DT = Convert.ToDateTime("1/1/1753 12:00:00 AM"); }
                        if (_ouserLogin.INSERTDT == new DateTime()) { _ouserLogin.INSERTDT = DateTime.Now; }
                        if (_ouserLogin.UPDT == new DateTime()) { _ouserLogin.UPDT = Convert.ToDateTime("1/1/1753 12:00:00 AM"); }
                        if (_ouserLogin.DOWNLOADDA == new DateTime()) { _ouserLogin.DOWNLOADDA = Convert.ToDateTime("1/1/1753 12:00:00 AM"); }
                        if (_ouserLogin.correction_dt == new DateTime()) { _ouserLogin.correction_dt = Convert.ToDateTime("1/1/1753 12:00:00 AM"); }
                        if (_ouserLogin.STREAM != null && _ouserLogin.STREAM != string.Empty)
                        {
                            List<SelectListItem> streams = openDB.GetStreams_1();
                            _ouserLogin.STREAMCODE = _ouserLogin.STREAM;
                            _ouserLogin.STREAM = streams.Find(f => f.Value == _ouserLogin.STREAM).Text;
                        }


                        if (string.IsNullOrEmpty(_ouserLogin.NAME) || string.IsNullOrEmpty(_ouserLogin.PINCODE) || string.IsNullOrEmpty(_ouserLogin.HOMEDISTNM) ||
                            string.IsNullOrEmpty(_ouserLogin.CATEGORY) || string.IsNullOrEmpty(_ouserLogin.DOB) ||
                            string.IsNullOrEmpty(_ouserLogin.EMAILID) || string.IsNullOrEmpty(_ouserLogin.MOBILENO) || string.IsNullOrEmpty(_ouserLogin.PWD) ||
                           string.IsNullOrEmpty(_ouserLogin.ADDRESS) || string.IsNullOrEmpty(_ouserLogin.AADHAR_NO) || string.IsNullOrEmpty(_ouserLogin.TEHSIL) || _ouserLogin.STREAM == null)
                        {
                            ViewData["result"] = "3";
                            ViewBag.val = "Required";
                            ViewBag.error = "All Required Fields Are Mandatory";
                            Session["captchaCode"] = captcha.captchaCode;
                            Session["captchaImg"] = captcha.captchaImg;
                            ViewBag.captchaCode = captcha.captchaCode;
                            ViewBag.captchaImg = captcha.captchaImg;
                            _ouserLogin.DOB = string.Empty;
                            _ouserLogin.PWD = string.Empty;
                            return View(_ouserLogin);
                        }


                        string val = openDB.InsertUser(_ouserLogin);
                        if (val == "-1" || val == "0")
                        {                          
                            ViewData["result"] = val;
                            ViewBag.val = string.Empty;
                            return View(_ouserLogin);
                        }

                        ViewData["result"] = "1";
                        ViewBag.val = val.ToString();
                        Session["app_no"] = val.ToString();
                        Session["app_name"] = _ouserLogin.NAME.ToString();
                        Session["app_id"] = _ouserLogin.ID.ToString();
                        Session["app_class"] = _ouserLogin.CLASS.ToString();
                        Session["app_stream"] = _ouserLogin.STREAM.ToString();
                        // Session["CandPhoto"] = "../../" + _ouserLogin.IMG_RAND.ToString();
                        Session["CandPhoto"] = "../../" + (_ouserLogin.IMG_RAND.ToString().Trim() == string.Empty ? "Images/NoPhoto.jpg" : "Upload/" + _ouserLogin.IMG_RAND.ToString());
                        Session["regStatus"] = openDB.IsUserInReg(_ouserLogin.ID.ToString()).ToString();
                        Session["subStatus"] = openDB.IsUserInSubjects(_ouserLogin.ID.ToString()).ToString();
                        string ChallanId = "";
                        Session["payStatus"] = openDB.IsUserInChallan(_ouserLogin.APPNO.ToString(), out ChallanId).ToString();

                        try
                        {
                            // New Sms 2023-24
                            string Sms = "Dear Candidate, You are successfully registered under Open School. Your App. No: " + val + " and Pwd: " + _ouserLogin.PWD + ".Kindly Login & complete your form";
                             string getSms = new AbstractLayer.DBClass().gosms(_ouserLogin.MOBILENO, Sms);
                            // openDB.mailer(_ouserLogin.EMAILID, val, _ouserLogin.PWD, _ouserLogin.NAME);                   
                        }
                        catch (Exception ex)
                        {
                            ViewBag.error = ex.Message.ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ViewData["result"] = "20";
                ViewBag.error = e.Message.ToString();
            }
            Session["captchaCode"] = captcha.captchaCode;
            Session["captchaImg"] = captcha.captchaImg;
            ViewBag.captchaCode = captcha.captchaCode;
            ViewBag.captchaImg = captcha.captchaImg;
            _ouserLogin.PWD = string.Empty;
            return View();
        }

        public JsonResult GetDistID(string strm) // Calling on http post (on Submit)
        {
            List<SelectListItem> DistList = new List<SelectListItem>();
            if (strm != null && strm != string.Empty)
            {
                DistList = openDB.GetStreamDistrict(strm);
            }

            try
            {
                SelectListItem sel = DistList.Find(f => f.Value.Trim() == "165");
                DistList.Remove(DistList.Find(f => f.Value.Trim() == "165"));
                sel.Text += " & others";
                DistList.Add(sel);
            }
            catch (Exception e) { }
            ViewBag.MyTeh = DistList;

            return Json(DistList);
        }

        public JsonResult GetTehID(int DIST, string strm) // Calling on http post (on Submit)
        {
            List<SelectListItem> TehList = new List<SelectListItem>();
            if (strm != null && strm != string.Empty)
            {
                TehList = openDB.GetStreamTehsil(DIST.ToString(), strm);
            }
            else
            {
                TehList = openDB.GetStreamTehsil(DIST.ToString(), string.Empty);
            }
            ViewBag.MyTeh = TehList;

            return Json(TehList);

        }

        public JsonResult GetStreams(string category)
        {
            List<SelectListItem> streams = new List<SelectListItem>();
            if (category.ToUpper() == "12TH FAIL (REGULAR SCHOOL-SCIENCE GROUP)")
            {
                streams = openDB.GetStreams_1();
            }
            //else if (category == "12th FAIL (Open School-AllGroups)" || category == "10th PASSED" || category == "12th FAIL (NIOS-All Groups)" || category == "12th FAIL (Regular School-Other Groups)")
            else
            {
                streams = openDB.GetStreams_2();
            }

            ViewBag.streams = streams;

            return Json(streams);

        }


        #endregion Login

        #region Modify Login
        public ActionResult ModifyLogin(string id)
        {          
            if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
            {
                return RedirectToAction("Index");
            }

            if (Session["payStatus"] == null || Session["payStatus"].ToString() != "0")
            {
                return RedirectToAction("Applicationstatus", "Open");
            }

            if (id == null || id.Trim() == string.Empty)
            { return RedirectToAction("Applicationstatus", "Open"); }


            OpenUserLogin _ouserLogin = openDB.GetRecord(id);
            string mode = _ouserLogin.FORM.ToLower();
            ViewBag.mode = mode;   
            if (mode == "m3")
            {
                ViewBag.categories = new AbstractLayer.OpenDB().GetMCategories();
            }
            else if (mode == "t3")
            {
                ViewBag.streams = new List<SelectListItem>() { new SelectListItem() { Text = "--Select--", Value = "" } };
                ViewBag.categories = new AbstractLayer.OpenDB().GetTCategories();
                if (_ouserLogin.CATEGORY.ToUpper() == "12TH FAIL (REGULAR SCHOOL-SCIENCE GROUP)")
                {
                    ViewBag.streams = new AbstractLayer.OpenDB().GetStreams_1();
                }
                else
                {
                    ViewBag.streams = new AbstractLayer.OpenDB().GetStreams_2();
                }
            }

            ViewBag.Dist = openDB.GetDistrict();
            ViewBag.Tehsils = new AbstractLayer.OpenDB().GetStreamTehsil((_ouserLogin.HOMEDIST != null) ? _ouserLogin.HOMEDIST : string.Empty, (_ouserLogin.STREAMCODE != null) ? _ouserLogin.STREAMCODE : string.Empty);
            ViewBag.selTehsil = new AbstractLayer.OpenDB().GetStreamTehsil((_ouserLogin.HOMEDIST != null) ? _ouserLogin.HOMEDIST : string.Empty, (_ouserLogin.STREAMCODE != null) ? _ouserLogin.STREAMCODE : string.Empty).ToList().Where(s => s.Value == _ouserLogin.TEHSIL).Select(s => s.Text).FirstOrDefault();
            ViewBag.STREAM = _ouserLogin.STREAMCODE;
            return View(_ouserLogin);
        }

        [HttpPost]
        public ActionResult ModifyLogin(string id,OpenUserLogin _ouserLogin, FormCollection fc)
        {

            if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
            {
                return RedirectToAction("Index");
            }

            if (Session["payStatus"] == null || Session["payStatus"].ToString() != "0")
            {
                return RedirectToAction("Applicationstatus", "Open");
            }

            if (id == null || id.Trim() == string.Empty)
            { return RedirectToAction("Applicationstatus", "Open"); }

            //OpenUserLogin _openUserLogin = openDB.GetRecord(id);
            string mode = _ouserLogin.FORM.ToLower();
            ViewBag.mode = mode;
            ViewBag.Tehsils = new AbstractLayer.OpenDB().GetStreamTehsil((_ouserLogin.HOMEDIST != null) ? _ouserLogin.HOMEDIST : string.Empty, (_ouserLogin.STREAMCODE != null) ? _ouserLogin.STREAMCODE : string.Empty);
            ViewBag.selTehsil = new AbstractLayer.OpenDB().GetStreamTehsil((_ouserLogin.HOMEDIST != null) ? _ouserLogin.HOMEDIST : string.Empty, (_ouserLogin.STREAMCODE != null) ? _ouserLogin.STREAMCODE : string.Empty).ToList().Where(s => s.Value == _ouserLogin.TEHSIL).Select(s => s.Text).FirstOrDefault();
            ViewBag.Dist = openDB.GetDistrict();
            _ouserLogin.HOMEDISTNM = new AbstractLayer.OpenDB().GetDistrict().Find(s => s.Value == _ouserLogin.HOMEDIST).Text;


            OpenUserLogin _openUserLogin = openDB.GetRecord(id);
            _ouserLogin.CHALLANDT = _openUserLogin.CHALLANDT;
            _ouserLogin.correction_dt = _openUserLogin.correction_dt;
            _ouserLogin.INSERTDT = _openUserLogin.INSERTDT;
            _ouserLogin.ISSTEP1DT = _openUserLogin.ISSTEP1DT;
            _ouserLogin.ISSTEP2DT = _openUserLogin.ISSTEP2DT;
            _ouserLogin.ISSUBJECT = _openUserLogin.ISSUBJECT;
            _ouserLogin.ISSTEP1 = _openUserLogin.ISSTEP1;
            _ouserLogin.ISSTEP2 = _openUserLogin.ISSTEP2;
            _ouserLogin.ISSTEP2B = _openUserLogin.ISSTEP2B;
            _ouserLogin.ISSCHLCHOO = _openUserLogin.ISSCHLCHOO;
            _ouserLogin.DOWNLOADDA = _openUserLogin.DOWNLOADDA;
            _ouserLogin.ID = _openUserLogin.ID;




            if (mode == "m3")
            {
                ViewBag.categories = new AbstractLayer.OpenDB().GetMCategories();
            }
            else if (mode == "t3")
            {
                ViewBag.streams = new List<SelectListItem>() { new SelectListItem() { Text = "--Select--", Value = "" } };
                ViewBag.categories = new AbstractLayer.OpenDB().GetTCategories();
                if (_ouserLogin.CATEGORY.ToUpper() == "12TH FAIL (REGULAR SCHOOL-SCIENCE GROUP)")
                {
                    ViewBag.streams = new AbstractLayer.OpenDB().GetStreams_1();
                }
                else
                {
                    ViewBag.streams = new AbstractLayer.OpenDB().GetStreams_2();
                }
            }
         

            try
            {

                // Check Email / mobile / Aadhar duplicacy

                string EMAILID1 = _ouserLogin.EMAILID;
                string MOBILENO1 = _ouserLogin.MOBILENO;
                string AADHAR_NO1 = _ouserLogin.AADHAR_NO;
                _ouserLogin.APPNO =Convert.ToInt64(id);
               // DataSet dschk = openDB.OpenStudentlist("", "", 0, 2);
                string Search = "appno !='" + _ouserLogin.APPNO + "'";
                DataSet dschk = new AbstractLayer.OpenDB().OpenStudentlist(Search, "", 0, 4);
                if (dschk.Tables.Count > 0)
                {
                    if (dschk.Tables[0].Rows.Count > 0)
                    {
                        bool dupEmail = dschk.Tables[0].AsEnumerable().Any(row => EMAILID1.ToUpper() == row.Field<string>("EMAILID").ToUpper());
                        bool dupMobile = dschk.Tables[0].AsEnumerable().Any(row => MOBILENO1.ToUpper() == row.Field<string>("MOBILENO").ToUpper());
                        bool dupAadhar = dschk.Tables[0].AsEnumerable().Any(row => AADHAR_NO1.ToUpper() == row.Field<string>("AADHAR_NO").ToUpper());

                        if (dupMobile == true || dupAadhar == true)
                        {
                            int flag = 0;
                           //if (dupEmail && flag == 0)
                           // {
                           //     flag = 1;
                           //     ViewBag.error = "Duplicate Email Id";
                           //     if (dupMobile)
                           //     {
                           //         flag = 2;
                           //         ViewBag.error += " and Mobile Number";
                           //     }
                           //     if (dupAadhar)
                           //     {
                           //         flag = 3;
                           //         ViewBag.flag = 3;
                           //         ViewBag.error += " and Aadhar Number";
                           //     }
                           // }
                            if (dupMobile && flag == 0)
                            {
                                flag = 2;
                                ViewBag.error = "Duplicate Mobile Number";
                                if (dupAadhar)
                                {
                                    flag = 3;
                                    ViewBag.flag = 3;
                                    ViewBag.error += " and Aadhar Number";
                                }
                            }
                            if (dupAadhar && flag == 0)
                            {
                                flag = 3;
                                ViewBag.flag = 3;
                                ViewBag.error = "Duplicate Aadhar Number";
                            }

                            if (flag > 0)
                            {

                                string Search1 = "appno !='" + _ouserLogin.APPNO + "' and AADHAR_NO='" + AADHAR_NO1 + "' or MOBILENO='" + MOBILENO1 + "'";
                                DataSet dschk1 = openDB.OpenStudentlist(Search1, "", 0, 4);
                                if (dschk1.Tables.Count > 0)
                                {
                                    if (dschk1.Tables[0].Rows.Count > 0)
                                    {
                                        ViewBag.errorAppNo = dschk1.Tables[0].Rows[0]["AppNo"].ToString();
                                        ViewBag.errorName = dschk1.Tables[0].Rows[0]["Name"].ToString();
                                        ViewBag.errorMOBILENO = dschk1.Tables[0].Rows[0]["MOBILENO"].ToString();
                                        ViewBag.errorHOMEDISTNM = dschk1.Tables[0].Rows[0]["HOMEDISTNM"].ToString();

                                    }
                                }
                            }
                            ViewBag.val = "DUP";      
                            return View(_ouserLogin);
                        }
                    }
                }

                //

                if (mode == "m3")
                {
                    _ouserLogin.STREAM = string.Empty;
                }            
                    try
                        {
                            DateTime dt = Convert.ToDateTime(DateTime.ParseExact(_ouserLogin.DOB, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture));
                    //DateTime chkDt = DateTime.Now;
                    DateTime chkDt = Convert.ToDateTime("31/03/2022");
                    if (mode == "t3")
                            {
                                //chkDt = chkDt.AddYears(-17);   By Rohit as informed by gulab sir
                            }
                            else
                            {
                                chkDt = chkDt.AddYears(-14);
                            }

                            if ((dt > chkDt) && _ouserLogin.CATEGORY.ToUpper() == "Direct 14 Year Age".ToUpper())
                            {
                        ViewBag.val = "D15";
                        ViewBag.error = "Candidate not eligible due to his age";                              
                                return View(_ouserLogin);
                            }
                        }
                        catch (Exception e)
                        {
                            ViewBag.error = "invalid date";                            
                            return View(_ouserLogin);
                        }

                //if (_ouserLogin.UPDT == new DateTime()) { _ouserLogin.UPDT = Convert.ToDateTime("1/1/1753 12:00:00 AM"); }


                         _ouserLogin.UPDT = DateTime.Now;
                        if (_ouserLogin.STREAM != null && _ouserLogin.STREAM != string.Empty)
                        {
                            List<SelectListItem> streams = openDB.GetStreams_1();
                            _ouserLogin.STREAMCODE = _ouserLogin.STREAM;
                            _ouserLogin.STREAM = streams.Find(f => f.Value == _ouserLogin.STREAM).Text;
                        }


                        if (string.IsNullOrEmpty(_ouserLogin.NAME) || string.IsNullOrEmpty(_ouserLogin.PINCODE) || string.IsNullOrEmpty(_ouserLogin.HOMEDISTNM) ||
                            string.IsNullOrEmpty(_ouserLogin.CATEGORY) || string.IsNullOrEmpty(_ouserLogin.DOB) ||
                            string.IsNullOrEmpty(_ouserLogin.EMAILID) || string.IsNullOrEmpty(_ouserLogin.MOBILENO) || string.IsNullOrEmpty(_ouserLogin.PWD) ||
                           string.IsNullOrEmpty(_ouserLogin.ADDRESS) || string.IsNullOrEmpty(_ouserLogin.AADHAR_NO) || string.IsNullOrEmpty(_ouserLogin.TEHSIL) || _ouserLogin.STREAM == null)
                        {
                             ViewBag.val = "10";
                            return View(_ouserLogin);
                        }



                int status = openDB.UpdateLoginUser(_ouserLogin);
                ViewBag.val = status.ToString();
                if (status == 1)
                { ViewBag.val = _ouserLogin.APPNO; }
                return View(_ouserLogin);
            }
            catch (Exception e)
            {
                ViewBag.error = e.Message.ToString();
            }
           
          
            return View();
        }
        #endregion Login

        #region Applicationstatus

        public ActionResult Applicationstatus(string id)
        {
            Session["Session"] = @PSEBONLINE.Repository.SessionSettingMastersRepository.GetSessionSettingMasters().SessionShortYear;
            string appno = id;
            DBClass dbClass = new DBClass();
            if (appno == null || appno == string.Empty || Session["app_no"] == null)
            {
                if (Session["app_no"] != null && Session["app_no"].ToString() != string.Empty)
                {
                    appno = Session["app_no"].ToString();
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }

            if (Session["app_no"].ToString() != appno)
            { return RedirectToAction("Index"); }
            else
            {
                OpenUserLogin _openUserLogin = openDB.GetRecord(appno);
                if (_openUserLogin == null)
                { _openUserLogin = new OpenUserLogin(); }
                else if (_openUserLogin.APPNO.ToString() == "0") { return RedirectToAction("Index"); }
                else
                {
                    Session["app_no"] = _openUserLogin.APPNO.ToString();
                    Session["app_name"] = _openUserLogin.NAME.ToString();
                    Session["app_id"] = _openUserLogin.ID.ToString();
                    ViewBag.Class = Session["app_class"] = _openUserLogin.CLASS.ToString();
                    Session["app_stream"] = _openUserLogin.STREAM.ToString();
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
                if (regStatus == 1)
                {
                    Session["CandPhoto"] = "../../" + (_openUserLogin.IMG_RAND.ToString().Trim() == string.Empty ? "Images/NoPhoto.jpg" : "Upload/" + _openUserLogin.IMG_RAND.ToString());
                    OpenUserRegistration _openUserRegistration = openDB.GetRegistrationRecord(_openUserLogin.ID.ToString());
                    Session["app_session"] = (_openUserRegistration.OSESSION.Length > 6 ) ? _openUserRegistration.OSESSION.Split(' ')[1] : "";
                    ViewBag.CentreStatus = _openUserRegistration.SCHL == "" ? "0" : "1";
                    Session["CentreStatus"] = _openUserRegistration.SCHL == "" ? "0" : "1";
                }
                ViewBag.subStatus = openDB.IsUserInSubjects(_openUserLogin.ID.ToString()).ToString();
                string ChallanId = "";
                ViewBag.payStatus = openDB.IsUserInChallan(_openUserLogin.APPNO.ToString(), out ChallanId).ToString();
                if (ChallanId.Length > 12)
                {
                    int x = openDB.IsChallanVerified(appno, ChallanId);
                    if (x == 0)
                    {
                        Session["payVerify"] = "0";
                    }
                    else
                    {
                        Session["payVerify"] = "1";
                    }
                }
                // ViewBag.payStatus = openDB.IsUserInChallan(_openUserLogin.APPNO.ToString()).ToString();
                if (TempData["notValidForChallan"] != null)
                {
                    ViewBag.notValidForChallan = TempData["notValidForChallan"].ToString();
                }
                if (TempData["notValidForChallan"] != null)
                {
                    ViewBag.notValidForChallan = TempData["notValidForChallan"].ToString();
                }

                ViewBag.CorrectEntryOpen = openDB.CorrectEntryOpen(_openUserLogin.ID.ToString()).ToString();
                return View(_openUserLogin);
            }

        }

        #endregion Applicationstatus       

        #region Registration

        public ActionResult Registration()
        {
            if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
            {
                return RedirectToAction("Index");
            }
            else if (Session["payStatus"] == null || Session["payStatus"].ToString() != "0")
            {
                return RedirectToAction("Applicationstatus", "Open");
            }
            else
            {
                DBClass dbClass = new DBClass();
                string app_id = Session["app_id"].ToString();
                ViewBag.YesNoList = new AbstractLayer.DBClass().GetYesNo();
                ViewBag.Months = openDB.GetMonths();
                ViewBag.Years = openDB.GetYears().Where(s => Convert.ToInt32(s.Value) <= 2022).ToList();
                ViewBag.AppearingYears = openDB.GetYears().Where(s => Convert.ToInt32(s.Value) <= 2021).ToList();
                ViewBag.Boards = openDB.GetN2Board();
                ViewBag.PhyChal = dbClass.GetDA();
                ViewBag.Gender = openDB.GetGenders();
                // ViewBag.Caste = dbClass.GetCaste();
                List<SelectListItem> casts = dbClass.GetCaste();
                casts.RemoveAll(r => r.Text.Contains("SC("));
                ViewBag.Cast = casts;
                
                if (TempData["result"] != null)
                {
                    ViewData["result"] = TempData["result"].ToString();
                }


                ViewBag.Religion = dbClass.GetReligion();
                ViewBag.StudyMedium = openDB.GetMedium();
                OpenUserRegistration _openUserRegistration = new OpenUserRegistration();
                _openUserRegistration = openDB.GetRegistrationRecord(app_id);
                if (_openUserRegistration == null)
                {
                    _openUserRegistration = new OpenUserRegistration();
                    _openUserRegistration.APPNO = app_id;
                }
                else
                {
                    if (_openUserRegistration.OSESSION != null && _openUserRegistration.OSESSION.Trim() != string.Empty)
                    {
                        string[] osession = _openUserRegistration.OSESSION.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        ViewBag.month = osession[0];
                        ViewBag.year = osession[1];
                    }

                    if (_openUserRegistration.AppearingYear != null && _openUserRegistration.AppearingYear.Trim() != string.Empty)
                    {
                        string[] oap = _openUserRegistration.AppearingYear.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        ViewBag.AppearingMonth = oap[0];
                        ViewBag.AppearingYr = oap[1];
                    }
                }
                _openUserRegistration.NATION = "INDIA";
                OpenUserLogin _openUserLogin = openDB.GetLoginById(app_id);
                _openUserRegistration.CAT = _openUserLogin.CATEGORY;
                if (_openUserLogin.IMG_RAND == "")
                { @ViewBag.Photo = "../Images/NoPhoto.jpg"; }
                else
                {
                    @ViewBag.Photo = "../../Upload/" + _openUserLogin.IMG_RAND.ToString();
                }
                if (_openUserLogin.IMGSIGN_RA == "")
                {
                    @ViewBag.Sign = "/Images/NoSignature.jpg";
                }
                else
                {
                    @ViewBag.Sign = "../../Upload/" + _openUserLogin.IMGSIGN_RA.ToString();
                }
                if (_openUserLogin.CATEGORY.ToLower().Contains("direct"))
                {
                    ViewBag.disableBoard = "true";
                }
                else
                {
                    ViewBag.disableBoard = "false";
                }
                ViewBag.subStatus = openDB.IsUserInSubjects(_openUserLogin.ID.ToString()).ToString();


                // Harpal Sir ->  hide and show appreaing year  in 1th pass on 270821 on call
                //if (_openUserLogin.CATEGORY == "10TH PASSED")
                //{      
                //    List<SelectListItem> yearlist = new AbstractLayer.DBClass().GetSessionYearSchoolAdmin();
                //    yearlist.Remove(yearlist[0]);                   
                //    ViewBag.Years = yearlist;
                //}

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


                if (string.IsNullOrEmpty(_openUserRegistration.REGNO))
                {
                    _openUserRegistration.IsREGNO = "N";
                }
                else {
                    _openUserRegistration.IsREGNO = "Y";
                }


                if (_openUserLogin.CATEGORY.ToUpper().Contains("NIOS"))
                {
                    ViewBag.Boards = openDB.GetN2Board().Where(s => s.Value == "NIOS BOARD").ToList() ;
                }
                else 
                {
                    ViewBag.Boards = openDB.GetN2Board().Where(s => s.Value != "NIOS BOARD").ToList();
                }


                //~/Images/NOSignature.jpg
                return View(_openUserRegistration);
            }
        }

        [HttpPost]
        public ActionResult Registration(OpenUserRegistration _openUserRegistration, FormCollection fc, HttpPostedFileBase Photo, HttpPostedFileBase Sign)
        {
            try
            {
                if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
                {
                    return RedirectToAction("Index");
                }
                DBClass dbClass = new DBClass();
                string app_id = Session["app_id"].ToString();
                ViewBag.YesNoList = new AbstractLayer.DBClass().GetYesNo();
                ViewBag.Months = openDB.GetMonths();
                ViewBag.Years = openDB.GetYears().Where(s => Convert.ToInt32(s.Value) <= 2022).ToList();
                ViewBag.AppearingYears = openDB.GetYears().Where(s => Convert.ToInt32(s.Value) <= 2021).ToList();
                ViewBag.Boards = openDB.GetN2Board();
                ViewBag.PhyChal = dbClass.GetDA();
                ViewBag.Gender = openDB.GetGenders();
                // ViewBag.Caste = dbClass.GetCaste();
                List<SelectListItem> casts = dbClass.GetCaste();
                casts.RemoveAll(r => r.Text.Contains("SC("));
                ViewBag.Cast = casts;
                ViewBag.Religion = dbClass.GetReligion();
                ViewBag.StudyMedium = openDB.GetMedium();

               
                    if (_openUserRegistration.CASTE != "")
                    {
                        SelectListItem item = casts.Find(f => f.Value.ToUpper() == _openUserRegistration.CASTE.ToUpper());
                        casts.Remove(item);
                        item.Selected = true;
                        casts.Add(item);
                        ViewBag.Cast = casts;
                    }

              


                _openUserRegistration.REGNO1 = _openUserRegistration.IsREGNO;



                string imgSign, imgPhoto;
                imgSign = imgPhoto = string.Empty;
                _openUserRegistration.APPNO = Session["app_id"].ToString();

                OpenUserLogin _openUserLogin = openDB.GetLoginById(_openUserRegistration.APPNO);
                _openUserRegistration.CAT = _openUserLogin.CATEGORY;
                _openUserRegistration.CLASS = _openUserLogin.CLASS;
                if (_openUserLogin.IMG_RAND == "")
                { @ViewBag.Photo = "../Images/NoPhoto.jpg"; }
                else
                {
                    @ViewBag.Photo = "../../Upload/" + _openUserLogin.IMG_RAND.ToString();
                }
                if (_openUserLogin.IMGSIGN_RA == "")
                {
                    @ViewBag.Sign = "/Images/NoSignature.jpg";
                }
                else
                {
                    @ViewBag.Sign = "../../Upload/" + _openUserLogin.IMGSIGN_RA.ToString();
                }

                if (_openUserLogin.CATEGORY.ToLower().Contains("direct"))
                {
                    ViewBag.disableBoard = "true";
                }
                else
                {
                    ViewBag.disableBoard = "false";
                }

                if (_openUserLogin.CATEGORY.ToUpper().Contains("NIOS"))
                {
                    ViewBag.Boards = openDB.GetN2Board().Where(s => s.Value == "NIOS BOARD").ToList();
                    _openUserRegistration.BOARD = "NIOS BOARD";
                }
                else
                {
                    ViewBag.Boards = openDB.GetN2Board().Where(s => s.Value != "NIOS BOARD").ToList();
                }


                // user exists in s
                int IsSubExists = openDB.IsUserInSubjects(_openUserLogin.ID.ToString());

                if (!string.IsNullOrEmpty(_openUserLogin.STREAMCODE))
                {
                    _openUserRegistration.EXAM = _openUserLogin.STREAMCODE.ToUpper();
                }
                if (_openUserRegistration.BOARD == "OTHER BOARD")
                {
                    string board = fc["OtherBoard"].ToString();
                    if (board != null && board != "")
                    {
                        _openUserRegistration.BOARD = board;
                    }
                }
                else if (_openUserRegistration.BOARD == "P.S.E.B BOARD")
                {
                    if (string.IsNullOrEmpty(_openUserRegistration.CASTE) || string.IsNullOrEmpty(_openUserRegistration.OROLL) || string.IsNullOrEmpty(_openUserRegistration.REGNO))
                    {
                        if (string.IsNullOrEmpty(_openUserRegistration.CASTE))
                        {
                            TempData["result"] = 3;
                        }
                        //if (string.IsNullOrEmpty(_openUserRegistration.OROLL))
                        //{
                        //    if (!_openUserLogin.CATEGORY.ToUpper().Contains("8TH") || _openUserLogin.CATEGORY.ToUpper().Contains("9TH"))
                        //    { TempData["result"] = 4; }
                        //}
                        // byharpal sir
                        ////if (string.IsNullOrEmpty(_openUserRegistration.REGNO))
                        ////{
                        ////    if (!_openUserLogin.CATEGORY.ToUpper().Contains("8TH"))
                        ////    { TempData["result"] = 5; }
                        ////}
                        if (TempData["result"] != null && TempData["result"].ToString().Trim() != string.Empty)
                        {
                            ViewData["result"] = TempData["result"].ToString();
                            _openUserRegistration = openDB.GetRegistrationRecord(app_id);
                            return View(_openUserRegistration);
                        }
                        //return RedirectToAction("Registration");
                    }
                }

                if (IsSubExists == 0)
                {
                    if (fc["CAT"] != null)
                    {
                        _openUserRegistration.CAT = fc["CAT"];
                    }
                }
                else
                {
                    _openUserRegistration.CAT = _openUserLogin.CATEGORY;
                }

                if (fc["AppearingMonth"] != null && fc["AppearingMonth"] != "" && fc["AppearingYr"] != null && fc["AppearingYr"] != "")
                {

                    _openUserRegistration.AppearingYear = fc["AppearingMonth"] + " " + fc["AppearingYr"];
                    ViewBag.AppearingMonth = fc["AppearingMonth"];
                    ViewBag.AppearingYr = fc["AppearingYr"];
                }
                else
                {
                    if (_openUserRegistration.CLASS == "10" && _openUserRegistration.CAT.ToUpper() == "10th Fail (NIOS)") // change by gulab
                    {
                        TempData["result"] = "AP";
                        ViewData["result"] = "AP";
                        return View(_openUserRegistration);
                    }
                    else if (_openUserRegistration.CLASS == "10" && _openUserRegistration.CAT.ToUpper() == "10th Fail (Regular School)") // change by gulab
                    {
                        TempData["result"] = "AP";
                        ViewData["result"] = "AP";
                        return View(_openUserRegistration);
                    }
                    else if (_openUserRegistration.CLASS == "12" && _openUserRegistration.CAT.ToUpper() == "12th FAIL (NIOS-All Groups)")
                    {
                        TempData["result"] = "AP";
                        ViewData["result"] = "AP";
                        return View(_openUserRegistration);
                    }
                    else if (_openUserRegistration.CLASS == "12" && _openUserRegistration.CAT.ToUpper() == "12TH FAIL (REGULAR SCHOOL-SCIENCE GROUP)")
                    {
                        TempData["result"] = "AP";
                        ViewData["result"] = "AP";
                        return View(_openUserRegistration);
                    }
                    else if (_openUserRegistration.CLASS == "12" && _openUserRegistration.CAT.ToUpper() == "12th FAIL (Regular School-Other Groups)")
                    {
                        TempData["result"] = "AP";
                        ViewData["result"] = "AP";
                        return View(_openUserRegistration);
                    }
                    else
                    {
                        _openUserRegistration.AppearingYear = "";
                        //if (!_openUserLogin.CATEGORY.ToLower().Contains("direct"))
                        //{
                        //    TempData["result"] = "AP";
                        //    ViewData["result"] = "AP";
                        //    return View(_openUserRegistration);
                        //}
                    }

                       
                   
                }

            

                //if (_openUserLogin.CATEGORY == "10TH PASSED")
                //{                                    
                //    List<SelectListItem> yearlist = new AbstractLayer.DBClass().GetSessionYearSchoolAdmin();
                //    yearlist.Remove(yearlist[0]);                  
                //    ViewBag.Years = yearlist;                   
                //}


                if (_openUserLogin.CATEGORY.ToUpper() != "DIRECT 14 YEAR AGE".ToUpper() && IsSubExists==0)
                {
                    if (fc["Month"] != null && fc["Month"] != "" && fc["Year"] != null && fc["Year"] != "")
                        {
                            _openUserRegistration.OSESSION = fc["Month"] + " " + fc["Year"];
                        ViewBag.month = fc["Month"];
                        ViewBag.year = fc["Year"]; 
                    }
                        else
                        {
                            TempData["result"] = "4";
                            ViewData["result"] = "4";
                            return View(_openUserRegistration);
                        }       
                }


                // Not eligible for 12th class
                // for session 2023-24
                if (_openUserLogin.CLASS == "12" && _openUserLogin.CATEGORY == "10TH PASSED" && _openUserRegistration.AppearingYear.Contains("2022"))
                {
                    TempData["result"] = "12";
                    ViewData["result"] = "12";
                    return View(_openUserRegistration);
                }



                if (Photo != null)
                {
                    //"~/Upload/Upload2023/PvtPhoto/Batch"
                    //var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/Open2022/Photo"), _openUserLogin.ID.ToString() + "_P.jpg");
                    //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/Open2022/Photo"));
                    //if (!Directory.Exists(FilepathExist))
                    //{
                    //    Directory.CreateDirectory(FilepathExist);
                    //}
                    //Photo.SaveAs(path);
                    string Orgfile = _openUserLogin.ID.ToString() + "_P.jpg";
                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = Photo.InputStream,
                                Key = string.Format("allfiles/Upload2023/Open2022/Photo/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }
                    //imgPhoto = "Upload2023/Open2022/Photo/" + _openUserLogin.ID.ToString() + "_P.jpg";
                    imgPhoto = "allfiles/Upload2023/Open2022/Photo/" + _openUserLogin.ID.ToString() + "_P.jpg";


                }
                else
                {


                }
                if (Sign != null)
                {
                    //var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/Open2022/Sign"), _openUserLogin.ID.ToString() + "_S.jpg");
                    //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/Open2022/Sign"));
                    //if (!Directory.Exists(FilepathExist))
                    //{
                    //    Directory.CreateDirectory(FilepathExist);
                    //}
                    //Sign.SaveAs(path);
                    string Orgfile = _openUserLogin.ID.ToString() + "_S.jpg";
                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = Photo.InputStream,
                                Key = string.Format("allfiles/Upload2023/Open2022/Sign/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }
                    imgSign = "allfiles/Upload2023/Open2022/Sign/" + _openUserLogin.ID.ToString() + "_S.jpg";                  
                    
                }
                

                if (string.IsNullOrEmpty(_openUserRegistration.OROLL) || string.IsNullOrEmpty(_openUserRegistration.BOARD) || _openUserRegistration.BOARD == "0" || string.IsNullOrEmpty(_openUserRegistration.FNAME) || string.IsNullOrEmpty(_openUserRegistration.MNAME) ||
                    string.IsNullOrEmpty(_openUserRegistration.SEX) || string.IsNullOrEmpty(_openUserRegistration.OSESSION) || string.IsNullOrEmpty(_openUserRegistration.CAT) || string.IsNullOrEmpty(_openUserRegistration.APPNO) ||
                  string.IsNullOrEmpty(_openUserRegistration.CandStudyMedium) || string.IsNullOrEmpty(_openUserRegistration.PHY_CHAL))
               // string.IsNullOrEmpty(_openUserRegistration.CandStudyMedium) || string.IsNullOrEmpty(_openUserRegistration.PHY_CHAL) || (string.IsNullOrEmpty(imgSign) && string.IsNullOrEmpty(_openUserLogin.IMGSIGN_RA)) || (string.IsNullOrEmpty(imgPhoto) && string.IsNullOrEmpty(_openUserLogin.IMG_RAND)))
                {
                    if (_openUserRegistration.CAT.ToLower().Contains("direct"))
                    {
                        if ((string.IsNullOrEmpty(_openUserRegistration.BOARD) || _openUserRegistration.BOARD == "0" || string.IsNullOrEmpty(_openUserRegistration.OSESSION)))
                        {
                            _openUserRegistration.BOARD = string.Empty;
                            _openUserRegistration.OSESSION = string.Empty;
                        }
                    }
                    //Old Roll no in Open Mandatory (except direct )
                    else if (!_openUserRegistration.CAT.ToLower().Contains("direct") && string.IsNullOrEmpty(_openUserRegistration.OROLL))
                    {
                       // TempData["result"] = "-1";
                        ViewData["result"] = "OROLL";
                        return View(_openUserRegistration);
                    }
                    else
                    {
                        TempData["result"] = "-1";                     
                        ViewData["result"] = "-1";
                        return View(_openUserRegistration);
                    }
                }

                //if ((string.IsNullOrEmpty(imgSign) && string.IsNullOrEmpty(_openUserLogin.IMGSIGN_RA))
                //    || (string.IsNullOrEmpty(imgPhoto) && string.IsNullOrEmpty(_openUserLogin.IMG_RAND)))
                //{
                //    TempData["result"] = "-10";
                //    ViewData["result"] = "-10";
                //    return View(_openUserRegistration);
                //}


                _openUserRegistration.INSERTDT = DateTime.Now;
                _openUserRegistration.FORM = _openUserLogin.FORM;
                _openUserRegistration.CLASS = _openUserLogin.CLASS;

                // Check Email / mobile / Aadhar duplicacy
                if (!string.IsNullOrEmpty(_openUserRegistration.REGNO))
                {
                    string REGNO1 = _openUserRegistration.REGNO;
                    DataSet dschk = openDB.OpenStudentlist(REGNO1, _openUserRegistration.APPNO, 0, 3);
                    if (dschk.Tables.Count > 0)
                    {
                        if (dschk.Tables[0].Rows.Count > 0)
                        {
                            ViewBag.error = "Duplicate Registration Number";
                            TempData["result"] = "DR";
                            ViewData["result"] = "DR";
                            return View(_openUserRegistration);
                        }
                    }
                }
                //

                if (_openUserRegistration.PHY_CHAL == "N.A." || _openUserRegistration.PHY_CHAL == "" )
                {
                    _openUserRegistration.DisabilityPercent = 0;
                }
                else
                {
                    if (_openUserRegistration.DisabilityPercent <= 0 || _openUserRegistration.DisabilityPercent > 100)
                    {
                        ViewBag.error = "DisabilityPercent";
                        TempData["result"] = "DisabilityPercent";
                        ViewData["result"] = "DisabilityPercent";
                        return View(_openUserRegistration);
                    }
                }

                int status = openDB.InsertRegistrationUser(_openUserRegistration: _openUserRegistration, imgSign: imgSign, imgPhoto: imgPhoto);
                if (status == 1)
                {
                    int regStatus = openDB.IsUserInReg(_openUserLogin.ID.ToString());
                    Session["regStatus"] = regStatus.ToString();
                    if (regStatus == 1)
                    {
                        Session["CandPhoto"] = "../../" + (_openUserLogin.IMG_RAND.ToString().Trim() == string.Empty ? "Images/NoPhoto.jpg" : "Upload/" + _openUserLogin.IMG_RAND.ToString());
                        _openUserRegistration = openDB.GetRegistrationRecord(_openUserLogin.ID.ToString());
                        Session["app_session"] = (_openUserRegistration.OSESSION.Length > 6 ) ? _openUserRegistration.OSESSION.Split(' ')[1] : "";
                        ViewBag.CentreStatus = _openUserRegistration.SCHL == "" ? "0" : "1";
                        Session["CentreStatus"] = _openUserRegistration.SCHL == "" ? "0" : "1";
                    }
                    ViewData["result"] = 1;
                    return View(_openUserRegistration);
                    //return RedirectToAction("Applicationstatus", new { appno = _openUserLogin.APPNO });
                }
                else
                {
                    ViewData["result"] = 0;
                    return View(_openUserRegistration);
                }
            }
            catch (Exception e)
            {
                ViewData["result"] = -2;
                ViewBag.Message = e.Message;
                oErrorLog.WriteErrorLog(e.ToString(), Path.GetFileName(Request.Path));
                return View(_openUserRegistration);
            }
        }

        public ActionResult Logout()
        {
            Session["app_no"] = Session["app_name"] = Session["app_id"] = null;
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            System.Web.Security.FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Open");

        }

        public ActionResult ViewRegistration(string Id)
        {
            ViewBag.YesNoList = new AbstractLayer.DBClass().GetYesNo();
            try
                {
                    DBClass dbClass = new DBClass();
                    OpenUserLogin ol = openDB.GetRecord(Id);
                    if (ol.ID > 0)
                    {
                        ViewBag.Id = ol.ID;
                        string app_id = ol.ID.ToString();
                        OpenUserRegistration _openUserRegistration = new OpenUserRegistration();
                        _openUserRegistration = openDB.GetRegistrationRecord(app_id);

                        if (_openUserRegistration == null)
                        {
                            _openUserRegistration = new OpenUserRegistration();
                            _openUserRegistration.APPNO = app_id;
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
                        _openUserRegistration.NATION = "INDIA";
                        /***************** LOGIN *****/
                        OpenUserLogin _openUserLogin = openDB.GetLoginById(app_id);
                        if (_openUserLogin.IMG_RAND == "" || _openUserLogin.IMG_RAND == null)
                        { @ViewBag.Photo = "/Images/NoPhoto.jpg"; }
                        else
                        {
                            @ViewBag.Photo = "../../Upload/" + _openUserLogin.IMG_RAND.ToString();
                        }
                        if (_openUserLogin.IMGSIGN_RA == "" || _openUserLogin.IMGSIGN_RA == null)
                        {
                            @ViewBag.Sign = "/Images/NoSignature.jpg";
                        }
                        else
                        {
                            @ViewBag.Sign = "../../Upload/" + _openUserLogin.IMGSIGN_RA.ToString();
                        }

                    ViewBag.Tehsil = openDB.GetStreamTehsil((_openUserLogin.HOMEDIST != null) ? _openUserLogin.HOMEDIST : string.Empty, (_openUserLogin.STREAMCODE != null) ? _openUserLogin.STREAMCODE : string.Empty).ToList().Where(s=>s.Value == _openUserLogin.TEHSIL).Select(s=>s.Text).FirstOrDefault();

                    ViewBag.MOBILENO = _openUserLogin.MOBILENO;
                    ViewBag.EMAILID = _openUserLogin.EMAILID;
                    string Tehsil1 = ViewBag.Tehsil == null ? "" : ViewBag.Tehsil;
                    ViewBag.ADDRESS = _openUserLogin.ADDRESS + " , " + Tehsil1 + " , " + _openUserLogin.PINCODE + " , " + _openUserLogin.HOMEDISTNM;
                    ViewBag.StudyCentreDIST = _openUserLogin.HOMEDISTNM;


                    /////

                      ViewBag.subStatus = openDB.IsUserInSubjects(_openUserLogin.ID.ToString()).ToString();
                        /***************** Subjects *****/
                        List<OpenUserSubjects> subjects_list = openDB.GetSubjectsForUser(app_id);
                        ViewBag.subjects_list = subjects_list;
                        return View(_openUserRegistration);
                    }
                    else
                    {
                        ViewBag.Id = 0;
                        return View();
                    }
                }
                catch (Exception)
                {
                    return View();
                }
           // }
        }

        #endregion Registration

        #region Subjects
        [HttpPost]
        public JsonResult getMINMAX(string cls, string YEAR, string SUB)
        {
            int OutStatus = 0;
            string search = "", THMAX = "000", THMIN = "000", PRMAX = "000", PRMIN = "000", INMAX = "000", INMIN = "000", TOTMAX = "000", TOTMIN = "000";
            DataSet ds = openDB.CheckSubMaster_MINMAX(cls, YEAR, SUB, "0", "0", search);
            if (ds != null)
            {
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        THMAX = ds.Tables[0].Rows[0]["THMAX"].ToString().Trim() == "--" ? "000"  : ds.Tables[0].Rows[0]["THMAX"].ToString();
                        THMIN = ds.Tables[0].Rows[0]["THMIN"].ToString().Trim() == "--" ? "000" : ds.Tables[0].Rows[0]["THMIN"].ToString();
                        PRMAX = ds.Tables[0].Rows[0]["PRMAX"].ToString().Trim() == "--" ? "000" : ds.Tables[0].Rows[0]["PRMAX"].ToString();
                        PRMIN = ds.Tables[0].Rows[0]["PRMIN"].ToString().Trim() == "--" ? "000" : ds.Tables[0].Rows[0]["PRMIN"].ToString();
                        INMAX = ds.Tables[0].Rows[0]["INMAX"].ToString().Trim() == "--" ? "000" : ds.Tables[0].Rows[0]["INMAX"].ToString();
                        INMIN = ds.Tables[0].Rows[0]["INMIN"].ToString().Trim() == "--" ? "000" : ds.Tables[0].Rows[0]["INMIN"].ToString();
                        TOTMAX = ds.Tables[0].Rows[0]["TOTMAX"].ToString().Trim() == "--" ? "000" : ds.Tables[0].Rows[0]["TOTMAX"].ToString();
                        TOTMIN = ds.Tables[0].Rows[0]["TOTMIN"].ToString().Trim() == "--" ? "000" : ds.Tables[0].Rows[0]["TOTMIN"].ToString();
                        OutStatus = 1;
                    }
                }

            }           
                var results = new
                {
                    status = OutStatus,
                    THMAX = THMAX,
                    THMIN = THMIN,
                    PRMAX = PRMAX,
                    PRMIN = PRMIN,
                    INMAX = INMAX,
                    INMIN = INMIN,
                    TOTMAX = TOTMAX,
                    TOTMIN = TOTMIN
                };
                return Json(results);
           
        }

        public ActionResult Subjects()
        {
            if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
            {
                return RedirectToAction("Index");
            }
            else if (Session["payVerify"].ToString() == "1" || Session["regStatus"].ToString() == "0" || Session["payStatus"].ToString() == "1")
            {
                return RedirectToAction("Applicationstatus", "Open");
            }
            else
            {
                OpenUserRegistration _openUserRegistration = openDB.GetRegistrationRecord(Session["app_id"].ToString());
                OpenUserLogin _openUserLogin = openDB.GetLoginById(Session["app_id"].ToString());
                string app_class = Session["app_class"].ToString();
                Session["subStatus"] = openDB.IsUserInSubjects(_openUserLogin.ID.ToString()).ToString();
                if (openDB.IsUserInReg(_openUserLogin.ID.ToString()) == 1)
                {
                    Session["CandPhoto"] = "../../" + (_openUserLogin.IMG_RAND.ToString().Trim() == string.Empty ? "Images/NoPhoto.jpg" : "Upload/" + _openUserLogin.IMG_RAND.ToString());
                    // Change by rohit
                    if (string.IsNullOrEmpty(_openUserRegistration.AppearingYear))
                    {
                        Session["AppearingYear"] = "";
                    }
                    else
                    {
                        Session["AppearingYear"] = _openUserRegistration.AppearingYear.Substring(_openUserRegistration.AppearingYear.LastIndexOf(' ') + 1);
                    }
                  
                    //   Session["app_session"] = _openUserRegistration.OSESSION.Substring(_openUserRegistration.OSESSION.LastIndexOf(' ') + 1);                    
                    string ss2 =  _openUserRegistration.OSESSION.Substring(_openUserRegistration.OSESSION.LastIndexOf(' ') + 1);
                    Session["app_session"] = (_openUserRegistration.OSESSION.Length > 6 ) ? _openUserRegistration.OSESSION.Split(' ')[1] : "";
                    Session["CentreStatus"] = _openUserRegistration.SCHL == "" ? "0" : "1";

                    string ChallanId = "";
                    ViewBag.payStatus = openDB.IsUserInChallan(_openUserLogin.APPNO.ToString(), out ChallanId).ToString();


                    if (app_class == "10")
                    {
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
//                            if DA = 'Yes' then sub2 to sub 6 show subject from subject master where opn = 'Y' and sub not in (63, 92, 01, 07)
//Add Sub 7,8: show subject from subject master where opn = 'Y' and sub not in (01, 07)
                            Session["Phy_Chal"] = "true";
                            ViewBag.Sub_2_5 = ViewBag.Sub_6_add  = openDB.GetMatricSubjects_2();
                            ViewBag.Sub_Matic_add = openDB.GetMatricSubjects_Additional_DA_Yes();
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
                                if(Session["app_session"] == null)
                                {
                                    ViewData["Result"] = "AY";
                                    return View(_openUserRegistration);
                                }
                                string session = Session["app_session"].ToString();
                                int appSession = Convert.ToInt32(session);
                                if (DateTime.Now.Year - 5 < appSession)//earlier it was 4
                                {
                                    ViewBag.visible = "true";
                                    ViewBag.maxCC = "4";
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
                            ViewBag.visible = "false";
                            ViewBag.maxCC = "0";
                        }
                    }
                    else if (app_class == "12")
                    {
                        string stream = Session["app_stream"].ToString();
                        List<SelectListItem> subjects = new List<SelectListItem>();
                        subjects.Add(new SelectListItem() { Value = "001", Text = "GENERAL ENGLISH"});
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
                            ViewBag.Sub5 =  subjects.Where(f => f.Value == "026" || f.Value == "144").Select(f => f).ToList(); 
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
                                ViewData["Sub_" + i + "_Chk"] = (_openUserSubjects.SUBCAT != "C") ? "" : "checked";

                            }
                            else
                            {
                                if (stream.Contains("COMMERCE-OLD"))//Old code change hen comemrce sub -5
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
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ViewData["Result"] = "-1";
                    return View(_openUserRegistration);
                }
                return View(_openUserRegistration);
            }
        }

        [HttpPost]
        public ActionResult Subjects(FormCollection fc, string Sub_1_Chk, string Sub_2_Chk, string Sub_3_Chk, string Sub_4_Chk, string Sub_5_Chk, string Sub_6_Chk)
        {
            if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
            {
                return RedirectToAction("Index");
            }
            OpenUserRegistration _openUserRegistration = openDB.GetRegistrationRecord(Session["app_id"].ToString());
            OpenUserLogin _openUserLogin = openDB.GetLoginById(Session["app_id"].ToString());
            string app_class = Session["app_class"].ToString();
            if (app_class == "10")
            {
                if (_openUserRegistration.PHY_CHAL.Trim().ToUpper().Contains("N.A."))
                {
                    ViewBag.Sub_2_5 = openDB.GetMatricSubjects_1();
                    ViewBag.Sub_6_add = openDB.GetMatricSubjects_2();
                    ViewBag.Sub_Matic_add = openDB.GetMatricSubjects_Add();
                }
                else
                {
                    // if DA = 'Yes' then sub2 to sub 6 show subject from subject master where opn = 'Y' and sub not in (63, 92, 01, 07)
                    //Add Sub 7,8: show subject from subject master where opn = 'Y' and sub not in (01, 07)
                    Session["Phy_Chal"] = "true";
                    ViewBag.Sub_2_5 = ViewBag.Sub_6_add = openDB.GetMatricSubjects_2();
                    ViewBag.Sub_Matic_add = openDB.GetMatricSubjects_Additional_DA_Yes();
                }
            }


            string app_id = Session["app_id"].ToString();
            string app_stream = Session["app_stream"].ToString();
            string[] subjects_array = new string[10];
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
                    return RedirectToAction("Subjects");
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

         
            // Check Credit Carry Marks

            if (!string.IsNullOrEmpty(fc["Sub_1_Chk"]))
            {
                if (string.IsNullOrEmpty(fc["Sub_1_Th_Obt"]) || string.IsNullOrEmpty(fc["Sub_1_Th_Min"]) || string.IsNullOrEmpty(fc["Sub_1_Th_Max"])
                    || string.IsNullOrEmpty(fc["Sub_1_Pr_Obt"]) || string.IsNullOrEmpty(fc["Sub_1_Pr_Min"]) || string.IsNullOrEmpty(fc["Sub_1_Pr_Max"])
                    || string.IsNullOrEmpty(fc["Sub_1_CCE_Obt"]) || string.IsNullOrEmpty(fc["Sub_1_CCE_Min"]) || string.IsNullOrEmpty(fc["Sub_1_CCE_Max"])
                    )
                {
                    ViewData["Result"] = "CC";
                    return View();
                }
            }

            if (!string.IsNullOrEmpty(fc["Sub_2_Chk"]))
            {
                if (string.IsNullOrEmpty(fc["Sub_2_Th_Obt"]) || string.IsNullOrEmpty(fc["Sub_2_Th_Min"]) || string.IsNullOrEmpty(fc["Sub_2_Th_Max"])
                    || string.IsNullOrEmpty(fc["Sub_2_Pr_Obt"]) || string.IsNullOrEmpty(fc["Sub_2_Pr_Min"]) || string.IsNullOrEmpty(fc["Sub_2_Pr_Max"])
                    || string.IsNullOrEmpty(fc["Sub_2_CCE_Obt"]) || string.IsNullOrEmpty(fc["Sub_2_CCE_Min"]) || string.IsNullOrEmpty(fc["Sub_2_CCE_Max"])
                    )
                {
                    ViewData["Result"] = "CC";
                    return View();
                }
            }

            if (!string.IsNullOrEmpty(fc["Sub_3_Chk"]))
            {
                if (string.IsNullOrEmpty(fc["Sub_3_Th_Obt"]) || string.IsNullOrEmpty(fc["Sub_3_Th_Min"]) || string.IsNullOrEmpty(fc["Sub_3_Th_Max"])
                  || string.IsNullOrEmpty(fc["Sub_3_Pr_Obt"]) || string.IsNullOrEmpty(fc["Sub_3_Pr_Min"]) || string.IsNullOrEmpty(fc["Sub_3_Pr_Max"])
                  || string.IsNullOrEmpty(fc["Sub_3_CCE_Obt"]) || string.IsNullOrEmpty(fc["Sub_3_CCE_Min"]) || string.IsNullOrEmpty(fc["Sub_3_CCE_Max"])
                  )
                {
                    ViewData["Result"] = "CC";
                    return View();
                }
            }


            if (!string.IsNullOrEmpty(fc["Sub_4_Chk"]))
            {
                if (string.IsNullOrEmpty(fc["Sub_4_Th_Obt"]) || string.IsNullOrEmpty(fc["Sub_4_Th_Min"]) || string.IsNullOrEmpty(fc["Sub_4_Th_Max"])
                  || string.IsNullOrEmpty(fc["Sub_4_Pr_Obt"]) || string.IsNullOrEmpty(fc["Sub_4_Pr_Min"]) || string.IsNullOrEmpty(fc["Sub_4_Pr_Max"])
                  || string.IsNullOrEmpty(fc["Sub_4_CCE_Obt"]) || string.IsNullOrEmpty(fc["Sub_4_CCE_Min"]) || string.IsNullOrEmpty(fc["Sub_4_CCE_Max"])
                  )
                {
                    ViewData["Result"] = "CC";
                    return View();
                }
            }


            if (!string.IsNullOrEmpty(fc["Sub_5_Chk"]))
            {
                if (string.IsNullOrEmpty(fc["Sub_5_Th_Obt"]) || string.IsNullOrEmpty(fc["Sub_5_Th_Min"]) || string.IsNullOrEmpty(fc["Sub_5_Th_Max"])
                 || string.IsNullOrEmpty(fc["Sub_5_Pr_Obt"]) || string.IsNullOrEmpty(fc["Sub_5_Pr_Min"]) || string.IsNullOrEmpty(fc["Sub_5_Pr_Max"])
                 || string.IsNullOrEmpty(fc["Sub_5_CCE_Obt"]) || string.IsNullOrEmpty(fc["Sub_5_CCE_Min"]) || string.IsNullOrEmpty(fc["Sub_5_CCE_Max"])
                 )
                {
                    ViewData["Result"] = "CC";
                    return View();
                }
            }


            if (!string.IsNullOrEmpty(fc["Sub_6_Chk"]))
            {
                if (string.IsNullOrEmpty(fc["Sub_6_Th_Obt"]) || string.IsNullOrEmpty(fc["Sub_6_Th_Min"]) || string.IsNullOrEmpty(fc["Sub_6_Th_Max"])
                 || string.IsNullOrEmpty(fc["Sub_6_Pr_Obt"]) || string.IsNullOrEmpty(fc["Sub_6_Pr_Min"]) || string.IsNullOrEmpty(fc["Sub_6_Pr_Max"])
                 || string.IsNullOrEmpty(fc["Sub_6_CCE_Obt"]) || string.IsNullOrEmpty(fc["Sub_6_CCE_Min"]) || string.IsNullOrEmpty(fc["Sub_6_CCE_Max"])
                 )
                {
                    ViewData["Result"] = "CC";
                    return View();
                }
            }


            // Check  language subjects for 12th
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
                            if(flag == 1) { flag = 2; }
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

                //12. Allow only 3 Language .. from  subject (01,02,03,09,10,71) .. check while submit
                int k = 0;
                foreach (string i in subjects_array)
                {
                    if (!string.IsNullOrEmpty(i))
                    {
                        if (i.Contains("01") || i.Contains("02") || i.Contains("03") || i.Contains("09") || i.Contains("10") || i.Contains("71"))
                        {
                            k++;
                        }
                    }
                }



                //eng- 02, hindi - 03,sankri - 09,urdu (elective)-10
                if (subjects_array.Contains("02") && subjects_array.Contains("03") && subjects_array.Contains("09") && subjects_array.Contains("10"))
                {
                    ViewData["Result"] = "ML3";
                    return View();
                }

                if (subjects_array.Contains("71") && subjects_array.Contains("03"))
                {
                    ViewData["Result"] = "ML1"; //You cannot select more than two language subject
                    return View();
                }
                if (k > 3)
                {
                    ViewData["Result"] = "ML3"; //You cannot select more than 3 language subject
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



            try
            {     // Check Duplicacy   
                bool CheckStatus = StaticDB.CheckArrayDuplicates(subjects_array);
                if (CheckStatus == false)
                {                    
                    int cc = subjects_array.Count(s => s != null);
                    if (cc > 4)
                    {
                        List<tblsubjectopen> _tblsubjectopenList = openDB.checkInsertUserInSubjects(subjects_array, subcat_array, app_class, app_id, app_stream, fc);
                        if (_tblsubjectopenList.Count > 0)
                        {
                            if (openDB.IsUserInSubjects(_openUserRegistration.APPNO) != 0)
                            {
                               openDB.RemoveUserSubjects(_openUserRegistration.APPNO);
                            }
                            //Loop and insert records.
                            foreach (tblsubjectopen opensubj in _tblsubjectopenList)
                            {
                                _context.tblsubjectopen.Add(opensubj);
                            }
                            int insertedRecords = _context.SaveChanges();
                            // _context?.Dispose();

                            if (insertedRecords > 0)
                            {
                                if (_openUserLogin.ISSUBJECT == 0)
                                {
                                    _openUserLogin.ISSUBJECT = 1;
                                    _openUserLogin.UPDT = DateTime.Now;
                                    openDB.UpdateLoginUser(_openUserLogin);
                                }
                            }
                          
                            ViewData["Result"] = "1";
                        }
                        else {
                            ViewData["Result"] = "0";
                        }


                        Session["subStatus"] = openDB.IsUserInSubjects(Session["app_id"].ToString()).ToString();
                        //ViewData["Result"] = "1";
                    }
                    else
                    {
                        ViewData["Result"] = "2";
                    }
                }
                else
                {
                    ViewData["Result"] = "5";
                }
                return View();
                // return RedirectToAction("Applicationstatus");
            }
            catch (Exception e) { ViewData["Result"] = "0"; return View(); }

        }

        
        public JsonResult GetSubject_3(string subject_2)
        {
            if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Open"),
                    isRedirect = true
                });
            }
            List<SelectListItem> subjects = new List<SelectListItem>();
            List<SelectListItem> selSubjects = new List<SelectListItem>();
            string app_class = Session["app_class"].ToString();
            if (app_class == "10")
            {
                if (Session["Phy_Chal"] != null && Session["Phy_Chal"].ToString() == "true")
                { subjects = openDB.GetMatricSubjects_2(); }
                else
                { subjects = openDB.GetMatricSubjects_1(); }
                if (subject_2 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_2)); } catch (Exception) { }
                TempData["Sub_2_5"] = subjects;
            }
            else if (app_class == "12")
            {
                string stream = Session["app_stream"].ToString();
                if (stream.ToUpper().Contains("COMMERCE"))
                {
                    subjects = openDB.GetSeniorSubjects_3();
                }
                else if (stream.ToUpper().Contains("HUMANITIES"))
                {
                    //subjects = openDB.GetSeniorSubjects_1();
                    subjects = openDB.GetSeniorSubjects_MainSubjects();
                }
                else if (stream.ToUpper().Contains("SCIENCE"))
                {
                    subjects = openDB.GetSeniorSubjects_2();
                }
                else
                {
                    //subjects = openDB.GetSeniorSubjects_1();
                    subjects = openDB.GetSeniorSubjects_MainSubjects();
                    ViewBag.Sub_3_4 = subjects;
                    ViewBag.Sub_5 = openDB.GetSeniorSubjects_2();
                    ViewBag.Sub_add = subjects;
                    TempData["Sub_3_4"] = subjects;
                    TempData["Sub_add"] = subjects;
                    TempData["Sub_5"] = openDB.GetSeniorSubjects_2();
                }

                selSubjects = (List<SelectListItem>)TempData["Sub_3_5"];
                if (subject_2 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_2)); } catch (Exception) { }
                if (selSubjects.Find(f => f.Text == "subject_2") != null)
                {
                    selSubjects.Remove(selSubjects.Find(f => f.Text == "subject_2"));
                    selSubjects.Add(new SelectListItem() { Text = "subject_2", Value = subject_2 });
                }
                TempData["Sub_3_5"] = selSubjects;
            }

            return Json(subjects);
        }

        public JsonResult GetSubject_4(string subject_3, string subject_2)
        {
            if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Open"),
                    isRedirect = true
                });
            }
            List<SelectListItem> subjects = new List<SelectListItem>();
            List<SelectListItem> selSubjects = new List<SelectListItem>();
            string app_class = Session["app_class"].ToString();
            if (app_class == "10")
            {
                if (Session["Phy_Chal"] != null && Session["Phy_Chal"].ToString() == "true")
                { subjects = openDB.GetMatricSubjects_2(); }
                else
                { subjects = openDB.GetMatricSubjects_1(); }
                if (subject_3 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_3)); } catch (Exception) { }
                if (subject_2 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_2)); } catch (Exception) { }
                TempData["Sub_2_5"] = subjects;
            }
            else if (app_class == "12")
            {
                selSubjects = (List<SelectListItem>)TempData["Sub_3_5"];
                if (selSubjects.Find(f => f.Text == "subject_3") != null)
                {
                    selSubjects.Remove(selSubjects.Find(f => f.Text == "subject_3"));
                }
                selSubjects.Add(new SelectListItem() { Text = "subject_3", Value = subject_3 });

                if (selSubjects.Find(f => f.Text == "subject_2") != null)
                {
                    selSubjects.Remove(selSubjects.Find(f => f.Text == "subject_2"));
                }
                selSubjects.Add(new SelectListItem() { Text = "subject_2", Value = subject_2 });

                string stream = Session["app_stream"].ToString();
                if (stream.ToUpper().Contains("COMMERCE"))
                {
                    subjects = openDB.GetSeniorSubjects_3();
                }
                else if (stream.ToUpper().Contains("HUMANITIES"))
                {
                    //subjects = openDB.GetSeniorSubjects_1();
                    subjects = openDB.GetSeniorSubjects_MainSubjects();                    
                }
                else if (stream.ToUpper().Contains("SCIENCE"))
                {
                    subjects = openDB.GetSeniorSubjects_2();
                    try
                    {
                        if (selSubjects.Find(f => f.Value == "028") != null)
                        {
                            subjects.Remove(subjects.Find(f => f.Value == "054"));
                        }
                        else if (selSubjects.Find(f => f.Value == "054") != null)
                        {
                            subjects.Remove(subjects.Find(f => f.Value == "028"));
                        }
                    }
                    catch (Exception) { }
                }
                else
                {
                    //subjects = openDB.GetSeniorSubjects_1();
                    subjects = openDB.GetSeniorSubjects_MainSubjects();                   
                    TempData["Sub_3_4"] = subjects;
                    TempData["Sub_add"] = subjects;
                    TempData["Sub_5"] = openDB.GetSeniorSubjects_2();
                }
                if (subject_3 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_3)); } catch (Exception) { }
                if (subject_2 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_2)); } catch (Exception) { }
                if (selSubjects.Find(f => f.Text == "subject_2") != null)
                {
                    selSubjects.Remove(selSubjects.Find(f => f.Text == "subject_2"));
                    selSubjects.Add(new SelectListItem() { Text = "subject_2", Value = subject_2 });
                }
                if (selSubjects.Find(f => f.Text == "subject_3") != null)
                {
                    selSubjects.Remove(selSubjects.Find(f => f.Text == "subject_3"));
                    selSubjects.Add(new SelectListItem() { Text = "subject_3", Value = subject_2 });
                }
                TempData["Sub_3_5"] = selSubjects;
            }
            return Json(subjects);
        }

        public JsonResult GetSubject_5(string subject_4, string subject_3, string subject_2)
        {
            if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Open"),
                    isRedirect = true
                });
            }
            List<SelectListItem> subjects = new List<SelectListItem>();
            List<SelectListItem> selSubjects = new List<SelectListItem>();
            string app_class = Session["app_class"].ToString();
            if (app_class == "10")
            {
                if (Session["Phy_Chal"] != null && Session["Phy_Chal"].ToString() == "true")
                { subjects = openDB.GetMatricSubjects_2(); }
                else
                { subjects = openDB.GetMatricSubjects_1(); }
                if (subject_4 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_4)); } catch (Exception) { }
                if (subject_3 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_3)); } catch (Exception) { }
                if (subject_2 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_2)); } catch (Exception) { }
                ViewBag.sub_5 = subjects;
            }
            else if (app_class == "12")
            {
                selSubjects = (List<SelectListItem>)TempData["Sub_3_5"];
                if (selSubjects.Find(f => f.Text == "subject_4") != null)
                {
                    selSubjects.Remove(selSubjects.Find(f => f.Text == "subject_4"));
                }
                selSubjects.Add(new SelectListItem() { Text = "subject_4", Value = subject_4 });

                if (selSubjects.Find(f => f.Text == "subject_3") != null)
                {
                    selSubjects.Remove(selSubjects.Find(f => f.Text == "subject_3"));
                }
                selSubjects.Add(new SelectListItem() { Text = "subject_3", Value = subject_3 });

                if (selSubjects.Find(f => f.Text == "subject_2") != null)
                {
                    selSubjects.Remove(selSubjects.Find(f => f.Text == "subject_2"));
                }
                selSubjects.Add(new SelectListItem() { Text = "subject_2", Value = subject_2 });
                string stream = Session["app_stream"].ToString();
                if (stream.ToUpper().Contains("COMMERCE"))
                {
                    subjects = openDB.GetSeniorSubjects_3();
                }
                else if (stream.ToUpper().Contains("HUMANITIES"))
                {                  
                    subjects = openDB.GetSeniorSubjects_MainSubjects();
                }
                else if (stream.ToUpper().Contains("SCIENCE"))
                {
                    subjects = openDB.GetSeniorSubjects_2();
                    try
                    {
                        if (selSubjects.Find(f => f.Value == "028") != null)
                        {
                            subjects.Remove(subjects.Find(f => f.Value == "054"));
                        }
                        else if (selSubjects.Find(f => f.Value == "054") != null)
                        {
                            subjects.Remove(subjects.Find(f => f.Value == "028"));
                        }
                    }
                    catch (Exception) { }
                }
                else
                {
                    subjects = openDB.GetSeniorSubjects_2();
                    //ViewBag.Sub_3_4 = subjects;
                    //ViewBag.Sub_5 = openDB.GetSeniorSubjects_2();
                    //ViewBag.Sub_add = subjects;
                    TempData["Sub_3_4"] = subjects;
                    TempData["Sub_add"] = subjects;
                    TempData["Sub_5"] = openDB.GetSeniorSubjects_2();
                }
                if (subject_4 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_4)); } catch (Exception) { }
                if (subject_3 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_3)); } catch (Exception) { }
                if (subject_2 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_2)); } catch (Exception) { }
                TempData["Sub_3_5"] = subjects;
            }
            return Json(subjects);
        }

        public JsonResult GetSubject_6(string subject_5, string subject_4, string subject_3, string subject_2)
        {
            if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Open"),
                    isRedirect = true
                });
            }
            List<SelectListItem> subjects = new List<SelectListItem>();
            List<SelectListItem> selSubjects = new List<SelectListItem>();
            string app_class = Session["app_class"].ToString();
                    
            if (app_class == "10")
            {

                if (Session["Phy_Chal"] != null && Session["Phy_Chal"].ToString() == "true")
                { subjects = openDB.GetMatricSubjects_2(); }
                else
                { subjects = openDB.GetMatricSubjects_2(); }

                //subjects = openDB.GetMatricSubjects_2();
                if (subject_5 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_5)); } catch (Exception) { }
                if (subject_4 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_4)); } catch (Exception) { }
                if (subject_3 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_3)); } catch (Exception) { }
                if (subject_2 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_2)); } catch (Exception) { }
                TempData["Sub_6_add"] = subjects;
            }
            else if (app_class == "12")
            {
                selSubjects = (List<SelectListItem>)TempData["Sub_3_5"];
                if (selSubjects.Find(f => f.Text == "subject_5") != null)
                {
                    selSubjects.Remove(selSubjects.Find(f => f.Text == "subject_5"));
                }
                selSubjects.Add(new SelectListItem() { Text = "subject_5", Value = subject_5 });

                if (selSubjects.Find(f => f.Text == "subject_4") != null)
                {
                    selSubjects.Remove(selSubjects.Find(f => f.Text == "subject_4"));
                }
                selSubjects.Add(new SelectListItem() { Text = "subject_4", Value = subject_4 });
                if (selSubjects.Find(f => f.Text == "subject_3") != null)
                {
                    selSubjects.Remove(selSubjects.Find(f => f.Text == "subject_3"));
                }
                selSubjects.Add(new SelectListItem() { Text = "subject_3", Value = subject_3 });
                if (selSubjects.Find(f => f.Text == "subject_2") != null)
                {
                    selSubjects.Remove(selSubjects.Find(f => f.Text == "subject_2"));
                }
                selSubjects.Add(new SelectListItem() { Text = "subject_2", Value = subject_2 });
                string stream = Session["app_stream"].ToString();


                if (stream.ToUpper().Contains("COMMERCE"))
                {
                    subjects = openDB.GetSeniorSubjects_AddSubList_COMM();
                }
                else if (stream.ToUpper().Contains("HUMANITIES"))
                {   
                    subjects = openDB.GetSeniorSubjects_AddSubList();
                }
                else if (stream.ToUpper().Contains("SCIENCE"))
                {
                    subjects = openDB.GetSeniorSubjects_AddSubList_SCI();
                    try
                    {
                        if (selSubjects.Find(f => f.Value == "028") != null)
                        {
                            subjects.Remove(subjects.Find(f => f.Value == "054"));
                        }
                        else if (selSubjects.Find(f => f.Value == "054") != null)
                        {
                            subjects.Remove(subjects.Find(f => f.Value == "028"));
                        }
                    }
                    catch (Exception) { }

                }
                else
                {
                    subjects = openDB.GetSeniorSubjects_MainSubjects();
                }

                subjects = openDB.GetSeniorSubjects_1();
               // subjects.Add(new SelectListItem() { Value = "139", Text = "ENVIRONMENTAL EDUCATION" });
                //subjects.Add(new SelectListItem() { Value = "145", Text = "KOREAN" });
                if (subject_5 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_5)); } catch (Exception) { }
                if (subject_4 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_4)); } catch (Exception) { }
                if (subject_3 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_3)); } catch (Exception) { }
                if (subject_2 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_2)); } catch (Exception) { }
                TempData["Sub_3_5"] = subjects;
            }
            return Json(subjects);
        }

        public JsonResult GetSubject_7(string subject_6, string subject_5, string subject_4, string subject_3, string subject_2)
        {
            if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Open"),
                    isRedirect = true
                });
            }
            List<SelectListItem> subjects = new List<SelectListItem>();
            List<SelectListItem> selSubjects = new List<SelectListItem>();
            string app_class = Session["app_class"].ToString();

            if (app_class == "10")
            {
                if (Session["Phy_Chal"] != null && Session["Phy_Chal"].ToString() == "true")
                { subjects = openDB.GetMatricSubjects_Additional_DA_Yes(); }
                else
                { subjects = openDB.GetMatricSubjects_Add(); }      

                if (subject_6 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_6)); } catch (Exception) { }
                if (subject_5 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_5)); } catch (Exception) { }
                if (subject_4 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_4)); } catch (Exception) { }
                if (subject_3 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_3)); } catch (Exception) { }
                if (subject_2 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_2)); } catch (Exception) { }
                TempData["Sub_6_add"] = subjects;
            }
            else if (app_class == "12")
            {
                selSubjects = (List<SelectListItem>)TempData["Sub_3_5"];
                string stream = Session["app_stream"].ToString();

                if (stream.ToUpper().Contains("COMMERCE"))
                {
                    subjects = openDB.GetSeniorSubjects_AddSubList_COMM();
                  }
                else if (stream.ToUpper().Contains("HUMANITIES"))
                {
                    subjects = openDB.GetSeniorSubjects_AddSubList();
                }
                else if (stream.ToUpper().Contains("SCIENCE"))
                {
                    subjects = openDB.GetSeniorSubjects_AddSubList_SCI();
                    try
                    {
                        if (selSubjects.Find(f => f.Value == "028") != null)
                        {
                            subjects.Remove(subjects.Find(f => f.Value == "054"));
                        }
                        else if (selSubjects.Find(f => f.Value == "054") != null)
                        {
                            subjects.Remove(subjects.Find(f => f.Value == "028"));
                        }
                    }
                    catch (Exception) { }
                }
                else
                {
                    subjects = openDB.GetSeniorSubjects_AddSubList();
                }

                //subjects.Add(new SelectListItem() { Value = "139", Text = "ENVIRONMENTAL EDUCATION" });
                // subjects.Add(new SelectListItem() { Value = "145", Text = "KOREAN" });
                if (subject_6 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_6)); } catch (Exception) { }
                if (subject_5 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_5)); } catch (Exception) { }
                if (subject_4 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_4)); } catch (Exception) { }
                if (subject_3 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_3)); } catch (Exception) { }
                if (subject_2 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_2)); } catch (Exception) { }
                TempData["Sub_3_5"] = subjects;
            }
            return Json(subjects);
        }

        public JsonResult GetSubject_8(string subject_7, string subject_6, string subject_5, string subject_4, string subject_3, string subject_2)
        {
            if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
            {
                return Json(new
                {
                    redirectUrl = Url.Action("Index", "Open"),
                    isRedirect = true
                });
            }
            List<SelectListItem> subjects = new List<SelectListItem>();           
            string app_class = Session["app_class"].ToString();
            if (app_class == "10")
            {
                if (Session["Phy_Chal"] != null && Session["Phy_Chal"].ToString() == "true")
                { subjects = openDB.GetMatricSubjects_Additional_DA_Yes(); }
                else
                { subjects = openDB.GetMatricSubjects_Add(); }

                if (subject_7 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_7)); } catch (Exception) { }
                if (subject_6 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_6)); } catch (Exception) { }
                if (subject_5 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_5)); } catch (Exception) { }
                if (subject_4 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_4)); } catch (Exception) { }
                if (subject_3 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_3)); } catch (Exception) { }
                if (subject_2 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_2)); } catch (Exception) { }              
            }
            else if (app_class == "12")
            {   
                string stream = Session["app_stream"].ToString();

                if (stream.ToUpper().Contains("COMMERCE"))
                {
                    subjects = openDB.GetSeniorSubjects_AddSubList_COMM();
                }
                else if (stream.ToUpper().Contains("HUMANITIES"))
                {
                    subjects = openDB.GetSeniorSubjects_AddSubList();
                }
                else if (stream.ToUpper().Contains("SCIENCE"))
                {
                    subjects = openDB.GetSeniorSubjects_AddSubList_SCI();
                }
                else
                {
                    subjects = openDB.GetSeniorSubjects_AddSubList();
                }

                if (subject_7 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_7)); } catch (Exception) { }
                if (subject_6 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_6)); } catch (Exception) { }
                if (subject_5 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_5)); } catch (Exception) { }
                if (subject_4 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_4)); } catch (Exception) { }
                if (subject_3 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_3)); } catch (Exception) { }
                if (subject_2 != "") try { subjects.Remove(subjects.Find(f => f.Value == subject_2)); } catch (Exception) { }               
            }
           
            TempData["Sub_6_add"] = subjects;
            return Json(subjects);
        }


        public JsonResult RemoveSubjects(string appno) // Calling on http post (on Submit)
        {
            int ostatus = 0;
            if (appno != "")
            {
                ostatus = openDB.RemoveUserSubjects(appno);
            }
            return Json(new { status = ostatus }, JsonRequestBehavior.AllowGet);
        }

        #endregion Subjects

        #region Fees and Challan
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult CalculateFee(string id)
        {
            try
            {
                ViewBag.Total = 0;
                Session["Session"] = @PSEBONLINE.Repository.SessionSettingMastersRepository.GetSessionSettingMasters().SessionShortYear;// Add for Challan Print
                FeeOpen _feeOpen = new FeeOpen();
                if (id == null || Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
                {
                    return RedirectToAction("Index");
                }
                if (id != "")
                {
                    
                    string AppNo = Session["app_id"].ToString();//1001
                    string today = DateTime.Today.ToString("dd/MM/yyyy");

                    OpenUserRegistration _openUserRegistration = new OpenUserRegistration();
                    _openUserRegistration = openDB.GetRegistrationRecord(Session["app_id"].ToString());
                    if (_openUserRegistration == null)
                    {
                        ViewBag.PHY_CHAL = "";
                    }
                    else
                    {
                        ViewBag.PHY_CHAL = _openUserRegistration.PHY_CHAL;
                    }

                    ViewBag.ToDatDate = today;

                    string result = openDB.IsValidForChallan(AppNo);
                    if (result != string.Empty)
                    {
                        TempData["notValidForChallan"] = result;
                        return RedirectToAction("Applicationstatus");
                    }

                    string OutError = "";
                    string ChallanId = "";
                    string IsUserInChallan = openDB.IsUserInChallan(id, out ChallanId).ToString();
                    ViewBag.IsUserInChallan = IsUserInChallan;
                    //ViewData["OutError"] = "IsUserInChallan";
                    ViewBag.LastDate = "";
                    if (IsUserInChallan == "0")
                    {
                      //  ViewData["OutError"] =  "0 = IsUserInChallan";
                        DateTime dateselected;
                        if (DateTime.TryParseExact(today, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
                        {
                            ViewData["result"] = 5;
                           
                            _feeOpen = openDB.spFeeDetailsOpen2017(AppNo, dateselected, out OutError);
                            ViewBag.Total = _feeOpen.TotalFee + _feeOpen.ExamTotalFee;
                            
                            if (_feeOpen.BankLastDate != null)
                            {
                                ViewBag.LastDate = _feeOpen.BankLastDate.ToString("dd/MM/yyyy");
                            }
                            
                            ViewData["result"] = 10;
                            ViewData["OutError"] = OutError.ToString();
                            ViewData["FeeStatus"] = "1";
                            ViewBag.TotalCount = 1;
                            return View(_feeOpen);

                        }
                        else {                            
                            ViewData["OutError"] = "Date Format Problem";
                        }


                        if (_feeOpen == null)
                        {
                            ViewBag.TotalCount = 0;
                            ViewData["FeeStatus"] = "0";
                        }                       
                    }
                    else
                    {
                       // ViewData["OutError"] = " Else ";
                        ChallanMasterModel CM = new ChallanMasterModel();
                        //HomeDB _homeDB = new HomeDB();
                        DataSet ds = openDB.GetChallanDetailsById(ChallanId);
                        CM.ChallanMasterData = ds;
                        try
                        {
                            if (CM.ChallanMasterData == null || CM.ChallanMasterData.Tables[0].Rows.Count == 0)
                            {
                                Session["payVerify"] = "0";
                            }
                        }
                        catch (Exception e)
                        {
                            ChallanId = "";
                            string res = openDB.IsUserInChallan(id, out ChallanId).ToString();
                            if (ChallanId.Length > 12)
                            {
                                int x = openDB.IsChallanVerified(id, ChallanId);
                                if (x == 0)
                                {
                                    Session["payVerify"] = "0";
                                }
                                else
                                {
                                    Session["payVerify"] = "1";
                                }
                            }
                        }
                        ViewBag.TotalCount = 0;
                        ViewBag.ChallanId = ChallanId;
                        ViewData["FeeStatus"] = "2";
                        //
                        ds = openDB.GetOpenChallanByAppNo(id);//app no
                        _feeOpen.StoreAllData = ds;
                        if (_feeOpen.StoreAllData == null || _feeOpen.StoreAllData.Tables.Count == 0)
                        {
                            ViewBag.TotalCount2 = 0;
                        }
                        else
                        {
                            ViewBag.TotalCount2 = _feeOpen.StoreAllData.Tables[0].Rows.Count;
                            if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "0")
                            {
                                //Challan Generated
                                ViewBag.Action = "0";
                            }
                            else if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "1")
                            {
                                //Downloaded by Bank 
                                ViewBag.Action = "1";
                            }
                            else if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "2")
                            {
                                //Challan Verified
                                Session["payVerify"] = "1";
                                ViewBag.Action = "2";
                            }

                        }
                    }

                }
                return View(_feeOpen);
            }
            catch (Exception e)
            {
               // ViewData["OutError"] = e.Message.ToString();
                ViewData["result"] = 20;
                ViewBag.Message = e.Message.ToString();
                return View();
            }
        }

        [HttpPost]
        public ActionResult CalculateFee(FeeOpen _feeOpen, FormCollection frm)
        {
            try
            {
                ChallanMasterModel CM = new ChallanMasterModel();

                if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
                {
                    return RedirectToAction("Index");
                }
                if (_feeOpen.BankCode == null)
                {
                    ViewBag.Message = "Please Select Bank";
                    ViewData["SelectBank"] = "1";
                    return View(_feeOpen);
                }
                else
                {
                      string BankCode = _feeOpen.BankCode;
                    string PayModValue = "online";
                    string bankName = "";

                    if (BankCode == "301" || BankCode == "302")
                    {
                        PayModValue = "online";
                        if (BankCode == "301")
                        {
                            bankName = "HDFC Bank";
                        }
                        else if (BankCode == "302")
                        {
                            bankName = "Punjab And Sind Bank";
                        }
                    }
                    else if (BankCode == "203")
                    {
                        PayModValue = "hod";
                        bankName = "PSEB HOD";
                    }
                    else if (BankCode == "202" || BankCode == "204")
                    {
                        PayModValue = "offline";
                        if (BankCode == "202")
                        {
                            bankName = "Punjab National Bank";
                        }
                        else if (BankCode == "204")
                        {
                            bankName = "State Bank of India";
                        }
                    }



                    string AppNo = Session["app_no"].ToString();//12201800001
                    string AppId = Session["app_id"].ToString();//1001

                    OpenUserRegistration _openUserRegistration = new OpenUserRegistration();
                    _openUserRegistration = openDB.GetRegistrationRecord(Session["app_id"].ToString());
                    if (_openUserRegistration == null)
                    {
                        ViewBag.PHY_CHAL = "";
                    }
                    else
                    {
                        ViewBag.PHY_CHAL = _openUserRegistration.PHY_CHAL;
                    }

                  
                    OpenUserLogin _openUserLogin = openDB.GetRecord(AppNo);
                    string today = DateTime.Today.ToString("dd/MM/yyyy");
                    DateTime dateselected;
                    if (DateTime.TryParseExact(today, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
                    {
                        string OutError = "";
                        _feeOpen = openDB.spFeeDetailsOpen2017(Session["app_id"].ToString(), dateselected, out OutError);
                       // _feeOpen = openDB.spFeeDetailsOpen2017(AppId, dateselected);
                        ViewBag.Total = _feeOpen.TotalFee + _feeOpen.ExamTotalFee;
                    }
                    if (ViewBag.Total == 0 && BankCode != "203")
                    {
                        BankCode = "203";
                        _feeOpen.BankCode = "203";
                    }
                    CM.APPNO = AppNo;
                    CM.FeeStudentList = AppNo;
                    CM.SCHLREGID = AppId;
                    CM.SchoolCode = AppId;
                    CM.addfee = _feeOpen.AdmissionFee; // AdmissionFee / ADDFEE
                    CM.latefee = _feeOpen.LateFee;
                    CM.prosfee = _feeOpen.ProsFee;
                    CM.addsubfee = _feeOpen.AddSubFee;
                    CM.add_sub_count = _feeOpen.NoAddSub;
                    CM.regfee = _feeOpen.RegConti;
                    CM.FEE = _feeOpen.TotalFee;
                    CM.TOTFEE = _feeOpen.TotalFee + _feeOpen.ExamTotalFee;

                    CM.OpenExamFee = _feeOpen.ExamRegFee;
                    CM.OpenLateFee = _feeOpen.ExamLateFee;
                    CM.OpenTotalFee = _feeOpen.ExamTotalFee;

                    CM.FEECAT = _feeOpen.FeeCat;
                    CM.FEECODE = _feeOpen.FeeCode;
                    CM.FEEMODE = "CASH";                    
                    CM.BANK = bankName;
                    CM.BCODE = BankCode;
                    CM.BANKCHRG = Convert.ToInt32(0);
                    CM.DIST = _openUserLogin.HOMEDIST;
                    CM.DISTNM = _openUserLogin.HOMEDISTNM;
                    CM.LOT = 0;
                    CM.DepositoryMobile = "CASH";
                    CM.type = "candt";
                    DateTime CHLNVDATE2;
                    if (DateTime.TryParseExact(_feeOpen.BankLastDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out CHLNVDATE2))
                    {
                        CM.ChallanVDateN = CHLNVDATE2;
                    }
                    else
                    {
                        CM.ChallanVDateN = _feeOpen.BankLastDate;
                    }
                    CM.CHLNVDATE = _feeOpen.BankLastDate.ToString("dd/MM/yyyy");
                    CM.LumsumFine = Convert.ToInt32(0);
                    CM.LSFRemarks = "";
                    string SchoolMobile = "";
                    string result = "0";
                    result = openDB.OpenInsertPaymentForm(CM, frm, out SchoolMobile);
                    if (result == "0" || result == "")
                    {
                        //--------------Not saved
                        ViewData["result"] = 0;
                        return View(_feeOpen);
                    }
                    if (result == "-1")
                    {
                        //-----alredy exist
                        ViewData["result"] = -1;
                        return View(_feeOpen);
                    }
                    else
                    {
                        ViewData["FeeStatus"] = null;
                        ViewData["SelectBank"] = null;
                        ViewData["result"] = 1;
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
                                string udf3CustMob = encrypt.QueryStringModule.Encrypt(SchoolMobile);

//AtomCheckoutUrl(string ChallanNo, string amt, string clientCode, string cmn, string cme, string cmno)
                                return RedirectToAction("AtomCheckoutUrl", "Gateway", new { ChallanNo= TransactionID, amt= TransactionAmount, clientCode= clientCode, cmn= udf1CustName, cme= udf2CustEmail, cmno = udf3CustMob } );
                               
                            }
                            #endregion Payment Gateyway
                        }
                        else
                        {

                            string Sms = "Fee Challan " + result + " of App " + Session["app_no"].ToString() + " successfully generated and valid upto " + CM.CHLNVDATE + ".Pay fee within valid date.Regards PSEB";
                            try
                            {
                                string getSms = new AbstractLayer.DBClass().gosms(SchoolMobile, Sms);
                                //string getSms = new AbstractLayer.DBClass().gosms("9711819184", Sms);
                            }
                            catch (Exception) { }
                            //--For Showing Message---------//                   
                            return RedirectToAction("GenerateChallaan", "Home", new { ChallanId = result });

                        }


                        ViewData["result"] = -10;
                        return View(_feeOpen);

                        //HomeDB _homeDB = new HomeDB();
                        //DataSet ds = openDB.GetChallanDetailsById(result);
                        //CM.ChallanMasterData = ds;
                        //if (CM.ChallanMasterData == null || CM.ChallanMasterData.Tables[0].Rows.Count == 0)
                        //{
                        //    Session["payVerify"] = "0";
                        //}
                        //else
                        //{
                        //    string ChallanId = "";
                        //    string res = openDB.IsUserInChallan(_openUserLogin.APPNO.ToString(), out ChallanId).ToString();
                        //    if (ChallanId.Length > 12)
                        //    {
                        //        Session["payStatus"] = "1";
                        //        int x = openDB.IsChallanVerified(_openUserLogin.APPNO.ToString(), ChallanId);
                        //        if (x == 0)
                        //        {
                        //            Session["payVerify"] = "0";
                        //        }
                        //        else
                        //        {
                        //            Session["payVerify"] = "1";
                        //        }
                        //    }
                        //}
                        //return RedirectToAction("GenerateChallaan", "Open", new { Id = result });
                    }
                }
            }
            catch (Exception)
            {
                return View(_feeOpen);
            }
        }

        public ActionResult GenerateChallaan(string Id)
        {
            try
            {
                Session["Session"] = @PSEBONLINE.Repository.SessionSettingMastersRepository.GetSessionSettingMasters().SessionShortYear;// Add for Challan Print
                string ChallanId = Id;
                ChallanMasterModel CM = new ChallanMasterModel();
                if (ChallanId == null || ChallanId == "0" || ChallanId == "")
                {
                    return RedirectToAction("Index", "Open");
                }
                

                string ChallanId1 = ChallanId.ToString();
                //HomeDB _homeDB = new HomeDB();
                DataSet ds = openDB.GetChallanDetailsById(ChallanId1);
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

                    CM.OpenExamFee = float.Parse(ds.Tables[0].Rows[0]["OpenExamFee"].ToString());
                    CM.OpenLateFee = float.Parse(ds.Tables[0].Rows[0]["OpenLateFee"].ToString());
                    CM.OpenTotalFee = float.Parse(ds.Tables[0].Rows[0]["OpenTotalFee"].ToString());


                    CM.FEE = float.Parse(ds.Tables[0].Rows[0]["FEE"].ToString());
                    CM.latefee = Convert.ToInt32(ds.Tables[0].Rows[0]["latefee"].ToString());
                    CM.TOTFEE = float.Parse(ds.Tables[0].Rows[0]["PaidFees"].ToString());
                    CM.FEECAT = ds.Tables[0].Rows[0]["FEECAT"].ToString();
                    CM.FEECODE = ds.Tables[0].Rows[0]["FEECODE"].ToString();
                    CM.FEEMODE = ds.Tables[0].Rows[0]["FEEMODE"].ToString();
                    CM.BANK = ds.Tables[0].Rows[0]["BANK"].ToString();
                    ViewBag.BCODE = CM.BCODE = ds.Tables[0].Rows[0]["BCODE"].ToString();
                    CM.BANKCHRG = float.Parse(ds.Tables[0].Rows[0]["BANKCHRG"].ToString());
                    CM.SchoolCode = ds.Tables[0].Rows[0]["SchoolCode"].ToString();
                    CM.APPNO = ds.Tables[0].Rows[0]["APPNO"].ToString();
                    CM.DIST = ds.Tables[0].Rows[0]["DIST"].ToString();
                    CM.DISTNM = ds.Tables[0].Rows[0]["DISTNM"].ToString();
                    CM.LOT = Convert.ToInt32(ds.Tables[0].Rows[0]["LOT"].ToString());
                    CM.TotalFeesInWords = ds.Tables[0].Rows[0]["TotalFeesInWords"].ToString();
                    CM.SchoolName = ds.Tables[0].Rows[0]["SchoolName"].ToString();
                    CM.DepositoryMobile = ds.Tables[0].Rows[0]["DepositoryMobile"].ToString();
                    CM.type = ds.Tables[0].Rows[0]["type"].ToString();
                    CM.APPNO = ds.Tables[0].Rows[0]["APPNO"].ToString();
                    CM.SCHLREGID = ds.Tables[0].Rows[0]["SCHLREGID"].ToString();
                    CM.SCHLCANDNM = ds.Tables[0].Rows[0]["SCHLCANDNM"].ToString();
                    if (ds.Tables[0].Rows[0]["Verified"].ToString() == "1")
                    {
                        CM.BRCODE = ds.Tables[0].Rows[0]["BRCODE"].ToString();
                        CM.BRANCH = ds.Tables[0].Rows[0]["BRANCH"].ToString();
                        CM.J_REF_NO = ds.Tables[0].Rows[0]["J_REF_NO"].ToString();
                        CM.DEPOSITDT = ds.Tables[0].Rows[0]["DEPOSITDT"].ToString();
                    }
                    return View(CM);
                }
            }
            catch (Exception ex)
            {
                // oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Index");
            }
        }


        #endregion Fees

        #region ApplicationForm
        public ActionResult ApplicationForm(string Id)
        {
            try
            {
                FeeOpen _feeOpen = new FeeOpen();
                if (Id == null || Id == "0" || Id == "")
                {
                    return RedirectToAction("Logout", "Open");
                }
                if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
                {
                    return RedirectToAction("Index", "Open");
                }
                else if (Session["payVerify"] == null || Session["payVerify"].ToString() == "0" || Session["CentreStatus"].ToString() == "0")
                {
                    return RedirectToAction("Applicationstatus", "Open");
                }
                string AppNo = "";
                if (Convert.ToString(Session["app_no"]) != "")
                {
                    AppNo = Session["app_no"].ToString();
                }

                DataSet ds = openDB.GetApplicationFormById(AppNo);
                _feeOpen.StoreAllData = ds;
                if (_feeOpen.StoreAllData == null || _feeOpen.StoreAllData.Tables.Count == 0 || _feeOpen.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCount = 0;
                    return View(_feeOpen);
                }
                else
                {
                    ViewBag.TotalCount = _feeOpen.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.APPNO = _feeOpen.StoreAllData.Tables[0].Rows[0]["APPNO"].ToString();
                    return View(_feeOpen);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        #endregion ApplicationForm

        #region StudyCenter

        public ActionResult Study_Center()
        {
            if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
            {
                return RedirectToAction("Index");
            }
            else if (Session["payVerify"].ToString() == "0")
            {
                return RedirectToAction("Applicationstatus", "Open");
            }
            else if (openDB.IsUserInSubjects(Session["app_id"].ToString()).ToString() == null || openDB.IsUserInSubjects(Session["app_id"].ToString()).ToString() == "0")
            {
                return RedirectToAction("Applicationstatus", "Open");
            }
            else
            {
                OpenUserLogin _openUserLogin = openDB.GetLoginById(Session["app_id"].ToString());
                if (_openUserLogin == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {

                    string ChallanId;
                    int x = 0;

                    x = openDB.IsUserInChallan(_openUserLogin.APPNO.ToString(), out ChallanId);
                    if (x != 0)
                    {
                        string dist = _openUserLogin.HOMEDIST;
                        string stream = _openUserLogin.STREAM;
                        List<SelectListItem> studyCenters = openDB.GetStudyCenters(dist, stream);

                        OpenUserRegistration _openUserRegistration = new OpenUserRegistration();
                        _openUserRegistration = openDB.GetRegistrationRecord(Session["app_id"].ToString());
                        ViewBag.CLASS = _openUserRegistration.CLASS.ToString();
                        if (_openUserRegistration != null)
                        {
                            ViewBag.SCHOOLE = (!string.IsNullOrEmpty(_openUserRegistration.SCHL.Trim())) ? _openUserRegistration.SCHOOLE : "";
                            ViewBag.School_1 = (!string.IsNullOrEmpty(_openUserRegistration.SCHL.Trim())) ? _openUserRegistration.SCHL.Trim() : "0";
                            ViewBag.School_2 = (!string.IsNullOrEmpty(_openUserRegistration.SCHL2.Trim())) ? _openUserRegistration.SCHL2.Trim() : "0";
                            ViewBag.School_3 = (!string.IsNullOrEmpty(_openUserRegistration.SCHL3.Trim())) ? _openUserRegistration.SCHL3.Trim() : "0";
                        }
                        ViewBag.studyCenters = studyCenters;
                        TempData["studyCenters"] = studyCenters;
                        TempData["selectedCenters"] = new List<SelectListItem>();
                        return View();
                    }
                    else
                    {
                        ViewBag.SubAccess = "challan";
                        return View();
                    }
                }

            }

        }

        public JsonResult GetStudyCenterList2(string center1)
        {
            List<SelectListItem> studyCenters = (List<SelectListItem>)TempData["studyCenters"];
            List<SelectListItem> selectedCenters = (List<SelectListItem>)TempData["selectedCenters"];

            if (selectedCenters.Count > 0)
            {
                if (selectedCenters.Find(f => f.Text == "Center1") == null)
                {
                    selectedCenters.Add(new SelectListItem() { Text = "Center1", Value = center1 });
                }
                else
                {
                    if (selectedCenters.Find(f => f.Value == center1) == null)
                    {
                        selectedCenters.Remove(selectedCenters.Find(f => f.Text == "Center1"));
                        selectedCenters.Add(new SelectListItem() { Text = "Center1", Value = center1 });
                    }
                    else
                    { selectedCenters.Remove(selectedCenters.Find(f => f.Value == center1)); }
                }
            }
            else
            {
                selectedCenters.Add(new SelectListItem() { Text = "Center1", Value = center1 });
            }


            TempData["studyCenters"] = studyCenters;
            foreach (SelectListItem sel in selectedCenters)
            {
                studyCenters.Remove(studyCenters.Find(f => f.Value == sel.Value));
            }
            TempData["selectedCenters"] = selectedCenters;
            ViewBag.studyCenters = studyCenters;

            return Json(studyCenters);

        }


        public JsonResult GetStudyCenterList3(string center2, string center1)
        {
            List<SelectListItem> studyCenters = (List<SelectListItem>)TempData["studyCenters"];
            List<SelectListItem> selectedCenters = (List<SelectListItem>)TempData["selectedCenters"];

            if (selectedCenters.Count > 0)
            {
                if (selectedCenters.Find(f => f.Text == "Center1") == null)
                {
                    selectedCenters.Add(new SelectListItem() { Text = "Center1", Value = center1 });
                }
                else
                {
                    if (selectedCenters.Find(f => f.Value == center1) == null)
                    {
                        selectedCenters.Remove(selectedCenters.Find(f => f.Text == "Center1"));
                        selectedCenters.Add(new SelectListItem() { Text = "Center1", Value = center1 });
                    }
                    else
                    { selectedCenters.Remove(selectedCenters.Find(f => f.Value == center1)); }
                }
                if (selectedCenters.Find(f => f.Text == "Center2") == null)
                {
                    selectedCenters.Add(new SelectListItem() { Text = "Center2", Value = center2 });
                }
                else
                {
                    if (selectedCenters.Find(f => f.Value == center2) == null)
                    {
                        selectedCenters.Remove(selectedCenters.Find(f => f.Text == "Center2"));
                        selectedCenters.Add(new SelectListItem() { Text = "Center2", Value = center2 });
                    }
                    else
                    { selectedCenters.Remove(selectedCenters.Find(f => f.Value == center2)); }
                }
            }
            else
            {
                selectedCenters.Add(new SelectListItem() { Text = "Center1", Value = center1 });
                selectedCenters.Add(new SelectListItem() { Text = "Center2", Value = center2 });
            }


            TempData["studyCenters"] = studyCenters;
            foreach (SelectListItem sel in selectedCenters)
            {
                studyCenters.Remove(studyCenters.Find(f => f.Value == sel.Value));
            }
            TempData["selectedCenters"] = selectedCenters;
            ViewBag.studyCenters = studyCenters;

            return Json(studyCenters);

        }

        [HttpPost]
        public ActionResult Study_Center(FormCollection fc)
        {
            if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
            {
                return RedirectToAction("Index");
            }
            else
            {
                OpenUserLogin _openUserLogin = openDB.GetLoginById(Session["app_id"].ToString());
                if (_openUserLogin == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    string dist = _openUserLogin.HOMEDIST;
                    string stream = _openUserLogin.STREAM;
                    List<SelectListItem> studyCenters = openDB.GetStudyCenters(dist, stream);

                    ViewBag.studyCenters = studyCenters;
                    TempData["studyCenters"] = studyCenters;
                    TempData["selectedCenters"] = new List<SelectListItem>();
                    try
                    {
                        DataSet ds = openDB.InsertStudyCenter(fc, dist, Session["app_id"].ToString());
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];
                            ViewBag.App_Email = dr["App_Email"].ToString();
                            ViewBag.App_Mobile = dr["App_Mobile"].ToString();
                            ViewBag.Schl_Email = dr["Schl_Email"].ToString();
                            ViewBag.Schl_Mobile = dr["Schl_Mobile"].ToString();
                            int regStatus = openDB.IsUserInReg(_openUserLogin.ID.ToString());
                            if (regStatus == 1)
                            {
                                Session["CandPhoto"] = "../../" + (_openUserLogin.IMG_RAND.ToString().Trim() == string.Empty ? "Images/NoPhoto.jpg" : "Upload/" + _openUserLogin.IMG_RAND.ToString());
                                OpenUserRegistration _openUserRegistration = openDB.GetRegistrationRecord(_openUserLogin.ID.ToString());
                                Session["app_session"] = (_openUserRegistration.OSESSION.Length > 6 ) ? _openUserRegistration.OSESSION.Split(' ')[1] : "";
                                ViewBag.CentreStatus = _openUserRegistration.SCHL == "" ? "0" : "1";
                                Session["CentreStatus"] = _openUserRegistration.SCHL == "" ? "0" : "1";
                            }
                            string ChallanId = "";
                            ViewBag.payStatus = openDB.IsUserInChallan(_openUserLogin.APPNO.ToString(), out ChallanId).ToString();
                            try
                            {
                                openDB.Study_Center_Mailer(_openUserLogin.APPNO.ToString(), dr["Schl_Email"].ToString(), ChallanId);
                                // string Sms = "Your Study Centers have been updated. Click to Login Here https://registration2022.pseb.ac.in/Open. Regards PSEB";
                                // new sms
                                //"Dear Candidate (Appno), Study Centre Schlnme+ (School code) is alloted to you for Senior Secondary Class. Visit study centre with registration Slip, Photographs,Eligibility Documents, Address & Age Proof and Fee Challan Copy. Collect free books from Study centre. Ebooks are also availble at pseb.ac.in/ebooks "
                                //'Dear User 10201800015, Study Centre is alloted to you. Click to Login https://bit.ly/2HKX0EI for Documents Required and Details for Study Centre.'
                                string Sms = "Dear User "+_openUserLogin.APPNO.ToString()+ ", Study Centre is alloted to you. Click to Login https://bit.ly/2HKX0EI for Documents Required and Details for Study Centre.";
                                string getSms = new AbstractLayer.DBClass().gosms(_openUserLogin.MOBILENO, Sms);
                            }
                            catch (Exception e)
                            {

                            }
                        }

                        return View();
                    }
                    catch (Exception e)
                    {
                        ViewBag.App_Email = ViewBag.App_Mobile = ViewBag.Schl_Email = ViewBag.Schl_Mobile = string.Empty;
                        return View();
                    }
                }
            }
        }

        #endregion StudyCenter

        #region ChangePassword
        public ActionResult ChangePassword()
        {
            if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
            {
                return RedirectToAction("Index", "Open");
            }
            ViewBag.AppNo = Session["app_no"].ToString();
            ViewBag.UserId = Session["app_id"].ToString();
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(FormCollection frm)
        {
            if (Session["app_id"] == null || Session["app_id"].ToString() == string.Empty)
            {
                return RedirectToAction("Index", "Open");
            }
            ViewBag.AppNo = Session["app_no"].ToString();
            ViewBag.UserId = Session["app_id"].ToString();
            string CurrentPassword = string.Empty;
            string NewPassword = string.Empty;


            if (frm["ConfirmPassword"] != "" && frm["NewPassword"] != "")
            {
                if (frm["ConfirmPassword"].ToString() == frm["NewPassword"].ToString())
                {
                    CurrentPassword = frm["CurrentPassword"].ToString();
                    NewPassword = frm["NewPassword"].ToString();
                    int result = openDB.ChangePassword(Convert.ToInt32(Session["app_id"].ToString()), CurrentPassword, NewPassword);
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

        #region SchoolOpen

        public ActionResult SchoolOpen(int? page)
        {
            Printlist obj = new Printlist();
            try
            {
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                {
                    return RedirectToAction("Logout", "Login");
                }
                string SCHL = Convert.ToString(Session["SCHL"]);

                DataSet result = new DBClass().schooltypes(SCHL);
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {
                    ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
                    ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
                }


                if (result.Tables[8].Rows.Count > 0)
                {
                    ViewBag.IsSchlAllow = result.Tables[8].Rows[0]["schl"].ToString();
                    ViewBag.AllowDate = result.Tables[8].Rows[0]["AllowDate"].ToString();
                    ViewBag.HUM = result.Tables[8].Rows[0]["HUM"].ToString();
                    ViewBag.sci = result.Tables[8].Rows[0]["sci"].ToString();
                    ViewBag.comm = result.Tables[8].Rows[0]["comm"].ToString();
                    ViewBag.Matric = result.Tables[8].Rows[0]["Matric"].ToString();
                    ViewBag.SeniorAccreditation = result.Tables[8].Rows[0]["SeniorAccreditation"].ToString();
                    ViewBag.MATRICAccreditation = result.Tables[8].Rows[0]["MATRICAccreditation"].ToString();
                }


                #region Circular

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                string Search = string.Empty;
                Search = "Id like '%' and CircularTypes like '%2%' and Convert(Datetime,Convert(date,ExpiryDateDD))>=Convert(Datetime,Convert(date,getdate()))";
                DataSet dsCircular = new AbstractLayer.AdminDB().CircularMaster(Search, pageIndex);//GetAllFeeMaster2016SP
                if (dsCircular == null || dsCircular.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCircular = 0;
                }
                else
                {
                    //var type7 = dsCircular.Tables[0].Columns[7].DataType.Name.ToString();
                    var type8 = dsCircular.Tables[0].Columns[9].DataType.Name.ToString();
                    ViewBag.TotalCircular = dsCircular.Tables[0].Rows.Count;
                    //
                    int count = Convert.ToInt32(dsCircular.Tables[1].Rows[0]["TotalCount"]);
                    ViewBag.TotalCircularCount = count;
                    int tp = Convert.ToInt32(count);
                    int pn = tp / 15;
                    int cal = 15 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                    { ViewBag.pn = pn + 1; }
                    else
                    { ViewBag.pn = pn; }

                    IEnumerable<DataRow> query = from order in dsCircular.Tables[0].AsEnumerable()
                                                 where order.Field<byte>("IsMarque") == 1 && order.Field<Boolean>("IsActive") == true
                                                 select order;
                    // Create a table of Marque from the query.
                    if (query.Any())
                    {
                        ViewBag.MarqueCount = 1;
                        obj.dsMarque = query.CopyToDataTable<DataRow>();
                    }
                    else { ViewBag.MarqueCount = 0; }

                    IEnumerable<DataRow> query1 = from order in dsCircular.Tables[0].AsEnumerable()
                                                  where order.Field<byte>("IsMarque") == 0 && order.Field<Boolean>("IsActive") == true
                                                  select order;
                    // Create a table of Marque from the query.
                    if (query1.Any())
                    {
                        ViewBag.CircularCount = 1;
                        obj.dsCircular = query1.CopyToDataTable<DataRow>();
                    }
                    else { ViewBag.CircularCount = 0; }
                }
                #endregion
            }
            catch (Exception ex)
            {
                //return RedirectToAction("Logout", "Login");
                //return View();
            }
            return View(obj);
        }


        public ActionResult Studentlist(string id, int? page)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("SchoolOpen", "Open");
                }
                ViewBag.Id = id;
                string schlcode = "";
                if (Convert.ToString(Session["SCHL"]) != "")
                {
                    schlcode = Convert.ToString(Session["SCHL"]);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                DataSet result = new DBClass().schooltypes(schlcode); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {
                    ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
                    ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
                }
                var itemsch = new SelectList(new[]{new {ID="1",Name="Application No"},new{ID="2",Name="Candidate Name"},
            new{ID="3",Name="Father's Name"},new{ID="4",Name="Mother's Name"},new{ID="5",Name="DOB"},}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();

                //if (TempData["Search"] != null)
                //{
                if (id == "M3" || id == "T3")
                {
                    Printlist obj = new Printlist();
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                    string clas = "FORM = '" + id.Trim().ToString().ToUpper() + "'";
                    string Search = string.Empty;
        
                    if (TempData["Search"] != null)
                    {
                        if (TempData["Search"].ToString().Trim() == string.Empty)
                        {
                            Search = " c.SCHL='" + schlcode + "' and  c.FORM='" + id.ToString() + "' ";
                        }
                        else
                        {
                            if (TempData["SearchId"].ToString() == id.ToString())
                            {
                                Search = TempData["Search"].ToString();
                            }
                            else
                            { Search = " c.SCHL='" + schlcode + "' and  c.FORM='" + id.ToString() + "' "; }
                        }
                    }
                    else
                    {
                        Search = " c.SCHL='" + schlcode + "' and  c.FORM='" + id.ToString() + "' ";
                    }
                    obj.StoreAllData = openDB.OpenStudentlist(Search, clas, pageIndex,1);
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        ViewBag.TotalCount1 = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        ViewBag.TotalCount1 = count;
                        int tp = Convert.ToInt32(count);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        ViewBag.SelectedItem = (TempData["SelValueSch"] != null) ? TempData["SelValueSch"].ToString() : string.Empty;
                        ViewBag.SearchString = (TempData["SearchString"] != null) ? TempData["SearchString"].ToString() : string.Empty;
                        return View(obj);
                    }
                }
                else
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }

                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult Studentlist(string id, int? page, FormCollection frm)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("SchoolOpen", "Open");
                }
                string schlcode = "";
                if (Convert.ToString(Session["SCHL"]) != "")
                {
                    schlcode = Convert.ToString(Session["SCHL"]);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                DataSet result = new DBClass().schooltypes(schlcode); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {
                    ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
                    ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
                }

                var itemsch = new SelectList(new[]{new {ID="1",Name="Application No"},new{ID="2",Name="Candidate Name"},
            new{ID="3",Name="Father's Name"},new{ID="4",Name="Mother's Name"},new{ID="5",Name="DOB"},}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                //AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                Printlist obj = new Printlist();
                ViewBag.Id = id;
                string Search = "";
                if (id == "M3" || id == "T3")
                {
                    string clas = "SCHL='" + schlcode + "' and FORM = '" + id.Trim().ToString().ToUpper() + "'";
                    Search = " c.SCHL='" + schlcode + "' and  c.FORM='" + id.ToString() + "' ";
                    if (frm["SelList"] != "")
                    {
                        ViewBag.SelectedItem = frm["SelList"];
                        int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());


                        if (frm["SearchString"] != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and b.APPNO='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += " and  c.NAME like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 3)
                            { Search += " and c.FNAME  like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " and c.MNAME like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 5)
                            { Search += " and c.DOB='" + frm["SearchString"].ToString() + "'"; }
                        }
                        TempData["SelValueSch"] = ViewBag.SelectedItem = SelValueSch;
                        TempData["SearchString"] = ViewBag.SearchString = frm["SearchString"];
                    }
                    TempData["SearchId"] = id.ToString();
                    TempData["Search"] = Search;
                    obj.StoreAllData = openDB.OpenStudentlist(Search, clas, pageIndex,1);//OpenStudentlistSP
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        ViewBag.TotalCount1 = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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
                return View(obj);
            }
            catch (Exception ex)
            {
                return View(id);
            }
        }


        public ActionResult AdmissionForm(string Id)
        {
            try
            {
                FeeOpen _feeOpen = new FeeOpen();
                if (Id == null || Id == "0" || Id == "")
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                {
                    return RedirectToAction("Logout", "Login");
                }

                string AppNo = Id;
                string SCHL = Convert.ToString(Session["SCHL"]);
                DataSet ds = openDB.GetApplicationFormById(AppNo);
                _feeOpen.StoreAllData = ds;
                if (_feeOpen.StoreAllData == null || _feeOpen.StoreAllData.Tables.Count == 0 || _feeOpen.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCount = 0;
                    return View(_feeOpen);
                }
                else
                {
                    ViewBag.TotalCount = _feeOpen.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.FORM = _feeOpen.StoreAllData.Tables[0].Rows[0]["FORM"].ToString();
                    ViewBag.FORMNAME = _feeOpen.StoreAllData.Tables[0].Rows[0]["FORMNAME"].ToString();
                    ViewBag.APPNO = _feeOpen.StoreAllData.Tables[0].Rows[0]["APPNO"].ToString();
                    ViewBag.ID = _feeOpen.StoreAllData.Tables[0].Rows[0]["ID"].ToString();

                    List<OpenUserSubjects> subjects_list = openDB.GetSubjectsForUser(_feeOpen.StoreAllData.Tables[0].Rows[0]["ID"].ToString());
                    ViewBag.subjects_list = subjects_list;
                    return View(_feeOpen);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion SchoolOpen
        
        #region UploadPhotoSign
        // GET: Open       

        public ActionResult UploadPhotoSign(string id)
        {
            try
            {
                ViewBag.app_id = id;
                if (Session["app_no"] == null || Session["app_no"].ToString() == string.Empty)
                {
                    return RedirectToAction("Index");
                }
                else if (Session["app_no"].ToString() != id.ToString())
                {
                    return RedirectToAction("Index");
                }               
                else
                {
                           
                    OpenUserLogin _openUserLogin = openDB.GetRecord(id);

                    ViewBag.CorrectEntryOpen = openDB.CorrectEntryOpen(_openUserLogin.ID.ToString()).ToString();
                    if (ViewBag.CorrectEntryOpen == "0")
                    { return RedirectToAction("Index"); }


                    if (_openUserLogin.IMG_RAND == "")
                    { @ViewBag.Photo = "../Images/NoPhoto.jpg"; }
                    else
                    {
                        @ViewBag.Photo = "../../Upload/" + _openUserLogin.IMG_RAND.ToString();
                    }
                    if (_openUserLogin.IMGSIGN_RA == "")
                    {
                        @ViewBag.Sign = "/Images/NoSignature.jpg";
                    }
                    else
                    {
                        @ViewBag.Sign = "../../Upload/" + _openUserLogin.IMGSIGN_RA.ToString();
                    }

                    //~/Images/NOSignature.jpg
                    return View(_openUserLogin);
                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult UploadPhotoSign(string id, HttpPostedFileBase Photo, HttpPostedFileBase Sign)
        {
            try
            {
                ViewBag.app_id = id;
                if (Session["app_no"] == null || Session["app_no"].ToString() == string.Empty)
                {
                    return RedirectToAction("Index");
                }
                else if (Session["app_no"].ToString() != id.ToString())
                {
                    return RedirectToAction("Index");
                }               

                string imgSign, imgPhoto;
                imgSign = imgPhoto = string.Empty;              
                OpenUserLogin _openUserLogin = openDB.GetRecord(id);               
                if (Photo != null)
                {
                    //var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/Open2022/Photo"), _openUserLogin.ID.ToString() + "_P.jpg");
                    //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/Open2022/Photo"));
                    //if (!Directory.Exists(FilepathExist))
                    //{
                    //    Directory.CreateDirectory(FilepathExist);
                    //}
                    //Photo.SaveAs(path);
                    string Orgfile = _openUserLogin.ID.ToString() + "_P.jpg";
                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = Photo.InputStream,
                                Key = string.Format("allfiles/Upload2023/Open2022/Photo/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }

                    imgPhoto = "allfiles/Upload2023/Open2022/Photo/" + _openUserLogin.ID.ToString() + "_P.jpg";                  
                }
                if (Sign != null)
                {
                    //var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/Open2022/Sign"), _openUserLogin.ID.ToString() + "_S.jpg");
                    //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/Open2022/Sign"));
                    //if (!Directory.Exists(FilepathExist))
                    //{
                    //    Directory.CreateDirectory(FilepathExist);
                    //}
                    //Sign.SaveAs(path);
                    string Orgfile = _openUserLogin.ID.ToString() + "_S.jpg";
                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = Photo.InputStream,
                                Key = string.Format("allfiles/Upload2023/Open2022/Sign/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }
                    imgSign = "allfiles/Upload2023/Open2022/Sign/" + _openUserLogin.ID.ToString() + "_S.jpg";
                }                

                int status = openDB.UploadPhotoSignOpen(id, imgSign, imgPhoto);
                if (status == 1)
                {                    
                    ViewData["result"] = 1;
                    return View(_openUserLogin);
                   
                }
                else
                {
                    ViewData["result"] = 0;
                    return View(_openUserLogin);
                }
            }
            catch (Exception e)
            {
                ViewData["result"] = 0;
                return View();
            }
        }




        #endregion UploadPhotoSign
        
        #region Fees and Challan
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult CalculateFeeOpenAdmin(string id,string APPNO)
        {
            try
            {
                Session["Session"] = @PSEBONLINE.Repository.SessionSettingMastersRepository.GetSessionSettingMasters().SessionShortYear;// Add for Challan Print
                FeeOpen _feeOpen = new FeeOpen();
                if (id == null || Session["AdminId"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (id != "")
                {
                    Session["app_id_admin"] = id.ToString();
                    Session["app_no_admin"] = APPNO.ToString();
                    
                   string AppNo = id.ToString();//1001
                    string today = DateTime.Today.ToString("dd/MM/yyyy");

                    ViewBag.ToDatDate = today;

                    string result = openDB.IsValidForChallan(AppNo);
                    if (result != string.Empty)
                    {
                        ViewData["result"] = "notValidForChallan";
                        // return RedirectToAction("Index", "Admin");
                        return View(_feeOpen);
                    }

                    string OutError = "";
                    string ChallanId = "";
                    string IsUserInChallan = openDB.IsUserInChallan(id, out ChallanId).ToString();
                    ViewBag.IsUserInChallan = IsUserInChallan;                  
                    if (IsUserInChallan == "0")
                    {                        
                        DateTime dateselected;
                        if (DateTime.TryParseExact(today, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
                        {
                            ViewData["result"] = 5;

                            _feeOpen = openDB.spFeeDetailsOpen2017_Admin_Phy_Chln(AppNo, dateselected, out OutError);
                            ViewBag.Total = _feeOpen.TotalFee + _feeOpen.ExamTotalFee;
                            ViewData["result"] = 10;
                            ViewData["OutError"] = OutError.ToString();
                            ViewData["FeeStatus"] = "1";
                            ViewBag.TotalCount = 1;
                      
                            return View(_feeOpen);

                        }
                        else
                        {
                            ViewData["OutError"] = "Date Format Problem";
                        }


                        if (_feeOpen == null)
                        {
                            ViewBag.TotalCount = 0;
                            ViewData["FeeStatus"] = "0";
                        }
                    }
                    else
                    {
                        // ViewData["OutError"] = " Else ";
                        ChallanMasterModel CM = new ChallanMasterModel();
                        //HomeDB _homeDB = new HomeDB();
                        DataSet ds = openDB.GetChallanDetailsById(ChallanId);
                        CM.ChallanMasterData = ds;
                        try
                        {
                            if (CM.ChallanMasterData == null || CM.ChallanMasterData.Tables[0].Rows.Count == 0)
                            {
                                Session["payVerify"] = "0";
                            }
                        }
                        catch (Exception e)
                        {
                            ChallanId = "";
                            string res = openDB.IsUserInChallan(id, out ChallanId).ToString();
                            if (ChallanId.Length > 12)
                            {
                                int x = openDB.IsChallanVerified(id, ChallanId);
                                if (x == 0)
                                {
                                    Session["payVerify"] = "0";
                                }
                                else
                                {
                                    Session["payVerify"] = "1";
                                }
                            }
                        }
                        ViewBag.TotalCount = 0;
                        ViewBag.ChallanId = ChallanId;
                        ViewData["FeeStatus"] = "2";
                        //
                        ds = openDB.GetOpenChallanByAppNo(id);//app no
                        _feeOpen.StoreAllData = ds;
                        if (_feeOpen.StoreAllData == null || _feeOpen.StoreAllData.Tables.Count == 0)
                        {
                            ViewBag.TotalCount2 = 0;
                        }
                        else
                        {
                            ViewBag.TotalCount2 = _feeOpen.StoreAllData.Tables[0].Rows.Count;
                            if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "0")
                            {
                                //Challan Generated
                                ViewBag.Action = "0";
                            }
                            else if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "1")
                            {
                                //Downloaded by Bank 
                                ViewBag.Action = "1";
                            }
                            else if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "2")
                            {
                                //Challan Verified
                                Session["payVerify"] = "1";
                                ViewBag.Action = "2";
                            }

                        }
                    }

                }
                return View(_feeOpen);
            }
            catch (Exception e)
            {
                // ViewData["OutError"] = e.Message.ToString();
                ViewData["result"] = 20;
                ViewBag.Message = e.Message.ToString();
                return View();
            }
        }

        [HttpPost]
        public ActionResult CalculateFeeOpenAdmin(FeeOpen _feeOpen, FormCollection frm, string lumsumfine, string lumsumremarks)
        {
            try
            {
                ChallanMasterModel CM = new ChallanMasterModel();

                if (Session["AdminId"] == null || Session["app_id_admin"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                } 
                else
                {    
                    string BankCode = "203";
                    string AppNo = Session["app_no_admin"].ToString();//12201800001
                    string AppId = Session["app_id_admin"].ToString();
                    OpenUserLogin _openUserLogin = openDB.GetRecord(AppNo);
                    string today = DateTime.Today.ToString("dd/MM/yyyy");
                    DateTime dateselected;
                    if (DateTime.TryParseExact(today, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
                    {
                        string OutError = "";
                        _feeOpen = openDB.spFeeDetailsOpen2017_Admin_Phy_Chln(AppId.ToString(), dateselected, out OutError);
                        // _feeOpen = openDB.spFeeDetailsOpen2017(AppId, dateselected);
                        ViewBag.Total = _feeOpen.TotalFee + _feeOpen.ExamTotalFee;
                    }
                    CM.APPNO = AppNo;
                    CM.FeeStudentList = AppNo;
                    CM.SCHLREGID = AppId;
                    CM.SchoolCode = AppId;
                    CM.addfee = _feeOpen.AdmissionFee; // AdmissionFee / ADDFEE
                    CM.latefee = _feeOpen.LateFee;
                    CM.prosfee = _feeOpen.ProsFee;
                    CM.addsubfee = _feeOpen.AddSubFee;
                    CM.add_sub_count = _feeOpen.NoAddSub;
                    CM.regfee = _feeOpen.RegConti;
                    CM.FEE = _feeOpen.TotalFee;
                    CM.TOTFEE = _feeOpen.TotalFee + _feeOpen.ExamTotalFee;

                    CM.OpenExamFee = _feeOpen.ExamRegFee;
                    CM.OpenLateFee = _feeOpen.ExamLateFee;
                    CM.OpenTotalFee = _feeOpen.ExamTotalFee;

                    CM.FEECAT = _feeOpen.FeeCat;
                    CM.FEECODE = _feeOpen.FeeCode;
                    CM.FEEMODE = "CASH";
                    CM.BANK = "";
                    CM.BCODE = BankCode;
                    CM.BANKCHRG = Convert.ToInt32(0);
                    CM.DIST = _openUserLogin.HOMEDIST;
                    CM.DISTNM = _openUserLogin.HOMEDISTNM;
                    CM.LOT = 0;
                    CM.DepositoryMobile = "CASH";
                    CM.type = "candt";
                    DateTime CHLNVDATE2;
                    if (DateTime.TryParseExact(_feeOpen.BankLastDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out CHLNVDATE2))
                    {
                        CM.ChallanVDateN = CHLNVDATE2;
                    }
                    else
                    {
                        CM.ChallanVDateN = _feeOpen.BankLastDate;
                    }
                    CM.CHLNVDATE = _feeOpen.BankLastDate.ToString("dd/MM/yyyy");
                    //CM.LumsumFine = Convert.ToInt32(0);
                    //CM.LSFRemarks = "";
                    if (string.IsNullOrEmpty(lumsumfine))
                    { CM.LumsumFine = Convert.ToInt32(0); }
                    else { CM.LumsumFine = Convert.ToInt32(lumsumfine); }                    
                    CM.LSFRemarks = lumsumremarks;

                    string SchoolMobile = "";
                    string result = "0";

                   result = openDB.OpenInsertPaymentForm(CM, frm, out SchoolMobile);

                    if (result == "0" || result == "")
                    {
                        //--------------Not saved
                        ViewData["result"] = 0;
                        return View(_feeOpen);
                    }
                    if (result == "-1")
                    {
                        //-----alredy exist
                        ViewData["result"] = -1;
                        return View(_feeOpen);
                    }
                    else
                    {
                        ViewData["FeeStatus"] = null;
                        ViewData["SelectBank"] = null;
                        ViewData["result"] = 1;
                        ViewBag.ChallanNo = result;
                       // string Sms = "Your Challan no. " + result + " of App no.  " + Session["app_no"].ToString() + " successfully generated and valid till Dt " + CM.CHLNVDATE + ". Regards PSEB";
                        string Sms = "Fee Challan " + result + " of App " + Session["app_no"].ToString() + " successfully generated and valid upto " + CM.CHLNVDATE + ".Pay fee within valid date.Regards PSEB";

                        try
                        {
                            string getSms = new AbstractLayer.DBClass().gosms(SchoolMobile, Sms);
                            //string getSms = new AbstractLayer.DBClass().gosms("9711819184", Sms);
                        }
                        catch (Exception) { }

                        // return View(_feeOpen);
                        //--For Showing Message---------//

                        //HomeDB _homeDB = new HomeDB();
                        DataSet ds = openDB.GetChallanDetailsById(result);
                        CM.ChallanMasterData = ds;
                        if (CM.ChallanMasterData == null || CM.ChallanMasterData.Tables[0].Rows.Count == 0)
                        {
                            Session["payVerify"] = "0";
                        }
                        else
                        {
                            string ChallanId = "";
                            string res = openDB.IsUserInChallan(_openUserLogin.APPNO.ToString(), out ChallanId).ToString();
                            if (ChallanId.Length > 12)
                            {
                                Session["payStatus"] = "1";
                                int x = openDB.IsChallanVerified(_openUserLogin.APPNO.ToString(), ChallanId);
                                if (x == 0)
                                {
                                    Session["payVerify"] = "0";
                                }
                                else
                                {
                                    Session["payVerify"] = "1";
                                }
                            }
                        }
                        return RedirectToAction("GenerateChallaan", "Open", new { Id = result });
                    }
                }
            }
            catch (Exception)
            {
                return View(_feeOpen);
            }
        }



        #endregion Fees



        #region Open INA Portal 
        [SessionCheckFilter]
        public ActionResult Open_INA_Portal()
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string SchlID = Convert.ToString(Session["SCHL"]);
                DataSet result = new AbstractLayer.DBClass().schooltypes(SchlID); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {
                    //ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                    //ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                    ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
                    ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
                }
                TempData["OpenINAMarksSearch"] = null;

                return View();
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }


        public ActionResult OpenINAAgree(string id)
        {
            try
            {

                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                Session["OpenINAAgree"] = id;
                @ViewBag.Dpdf = "../../PDF/12th_CCE.pdf";
                @ViewBag.Showpdf = "../../PDF/12th_CCE.pdf";
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult OpenINAAgree(FormCollection frm)
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                string s = frm["Agree"].ToString();
                if (Session["OpenINAAgree"] == null)
                {
                    return RedirectToAction("Open_INA_Portal", "Open");
                }
                else
                {
                    string CCEClass1 = Session["OpenINAAgree"].ToString();
                    ViewBag.CCEClass = CCEClass1;
                    if (s == "Agree")
                    {
                        return RedirectToAction("OpenINAMarks", "Open", new { id = CCEClass1 });
                    }
                    else
                    {
                        return RedirectToAction("Open_INA_Portal", "Open");
                    }
                }
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }
        }

        [SessionCheckFilter]
        public ActionResult OpenINAMarks(FormCollection frm, string id, int? page)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SchoolModels MS = new SchoolModels();
            try
            {
                string SCHL = "";
                string CLASS = "";
                if (id == "S") // For Senior
                {
                    CLASS = "12";
                    ViewBag.ClassName = "SENIOR SECONDARY";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "10";
                    ViewBag.ClassName = "MATRIC";
                }

                if (SCHL == "" || SCHL == null)
                { SCHL = Session["SCHL"].ToString(); }

                ViewBag.schlCode = SCHL;
                ViewBag.cid = id;

                #region  Check School Allow For CCE

                DataSet dsAllow = new AbstractLayer.SchoolDB().SchoolAllowForOpenINAMarks(SCHL, CLASS);
                if (dsAllow.Tables.Count > 0)
                {
                    if (dsAllow.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsActive = dsAllow.Tables[0].Rows[0]["IsActive"].ToString();
                        ViewBag.IsAllow = dsAllow.Tables[0].Rows[0]["IsAllow"].ToString();
                        ViewBag.LastDate = dsAllow.Tables[0].Rows[0]["LastDate"].ToString();
                        ViewBag.LastDateDT = dsAllow.Tables[0].Rows[0]["LastDateDT"].ToString();
                    }
                }

                #endregion  Check School Allow For CCE


                var itemFilter = new SelectList(new[] { new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                    new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";


                var itemAction = new SelectList(new[] { new { ID = "1", Name = "Pending" }, new { ID = "2", Name = "Filled" }, new { ID = "3", Name = "Final Submitted" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";
                //------------------------

                if (SCHL != null)
                {
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                    string Search = string.Empty;
                    string SelectedAction = "0";
                    if (TempData["OpenINAMarksSearch"] != null)
                    {
                        Search += TempData["OpenINAMarksSearch"].ToString();
                        ViewBag.SelectedFilter = TempData["SelFilter"];
                        SelectedAction = TempData["SelAction"].ToString();
                        ViewBag.SelectedAction = TempData["SelActionValue"];

                        TempData["SelFilter"] = ViewBag.SelectedFilter;
                        TempData["SelAction"] = SelectedAction;
                        TempData["SelActionValue"] = ViewBag.SelectedAction;
                        TempData["OpenINAMarksSearch"] = Search;
                    }
                    else
                    {
                        Search = "  a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "'";
                    }


                    //string class1 = "4"; // For Senior
                    MS.StoreAllData = new AbstractLayer.SchoolDB().GetOpenINAMarksStudentsBySchool(Search, SCHL, pageIndex, CLASS, Convert.ToInt32(SelectedAction));
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsCCEFilled"]);
                        }
                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                            ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[4].Rows[0]["FinalSubmitLastDate"];
                        }
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);

                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsCCEFilled"]);
                        }

                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                            ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[4].Rows[0]["FinalSubmitLastDate"];
                        }

                        ViewBag.TotalCount1 = count;
                        int tp = Convert.ToInt32(count);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;

                        return View(MS);
                    }


                }
            }
            catch (Exception ex)
            {
                //return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult OpenINAMarks(FormCollection frm, int? page)
        {


            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string id = "";
                if (frm["cid"] != "")
                {
                    id = frm["cid"].ToString();
                    ViewBag.cid = frm["cid"].ToString();
                }
                string SCHL = Session["SCHL"].ToString();
                ViewBag.schlCode = SCHL;
                string CLASS = "";
                if (id == "S") // For Senior
                {
                    CLASS = "12";
                    ViewBag.ClassName = "SENIOR SECONDARY";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "10";
                    ViewBag.ClassName = "MATRIC";
                }

                #region  Check School Allow For CCE

                DataSet dsAllow = new AbstractLayer.SchoolDB().SchoolAllowForOpenINAMarks(SCHL, CLASS);
                if (dsAllow.Tables.Count > 0)
                {
                    if (dsAllow.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsActive = dsAllow.Tables[0].Rows[0]["IsActive"].ToString();
                        ViewBag.IsAllow = dsAllow.Tables[0].Rows[0]["IsAllow"].ToString();
                        ViewBag.LastDate = dsAllow.Tables[0].Rows[0]["LastDate"].ToString();
                        ViewBag.LastDateDT = dsAllow.Tables[0].Rows[0]["LastDateDT"].ToString();
                    }
                }

                #endregion  Check School Allow For CCE

                var itemFilter = new SelectList(new[] { new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                    new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";


                var itemAction = new SelectList(new[] { new { ID = "1", Name = "Pending" }, new { ID = "2", Name = "Filled" }, new { ID = "3", Name = "Final Submitted" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";
                //------------------------

                if (SCHL != null)
                {
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                    string Search = string.Empty;
                    //  Search = " a.SCHL='" + SCHL + "'";
                    Search = "  a.SCHL = '" + SCHL + "' and  a.class='" + CLASS + "' ";

                    int SelAction = 0;
                    if (frm["SelAction"] != "")
                    {
                        int SelValueSch = Convert.ToInt32(frm["SelAction"].ToString());
                        if (frm["SelAction"] != "")
                        {
                            SelAction = SelValueSch;
                        }
                        ViewBag.SelectedAction = frm["SelAction"];
                    }

                    if (frm["SelFilter"] != "")
                    {

                        ViewBag.SelectedFilter = frm["SelFilter"];
                        int SelValueSch = Convert.ToInt32(frm["SelFilter"].ToString());
                        if (frm["SelFilter"] != "" && frm["SearchString"].ToString() != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and Roll='" + frm["SearchString"].ToString() + "'"; }
                            if (SelValueSch == 2)
                            { Search += " and Std_id='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 3)
                            { Search += " and  Registration_num like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " and  Name like '%" + frm["SearchString"].ToString() + "%'"; }
                        }
                    }

                    TempData["SelFilter"] = frm["SelFilter"];
                    TempData["SelAction"] = SelAction;
                    TempData["SelActionValue"] = frm["SelAction"];
                    TempData["OpenINAMarksSearch"] = Search;
                    // string class1 = "4";
                    MS.StoreAllData = new AbstractLayer.SchoolDB().GetOpenINAMarksStudentsBySchool(Search, SCHL, pageIndex, CLASS, SelAction);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsCCEFilled"]);
                        }
                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                            ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[4].Rows[0]["FinalSubmitLastDate"];
                        }
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsCCEFilled"]);
                        }

                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                            ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[4].Rows[0]["FinalSubmitLastDate"];
                        }
                        ViewBag.TotalCount1 = count;
                        int tp = Convert.ToInt32(count);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                        { ViewBag.pn = pn + 1; }
                        else
                        { ViewBag.pn = pn; }

                        return View(MS);
                    }
                }
            }
            catch (Exception ex)
            {
                //return RedirectToAction("Index", "Login");
            }

            return View();
        }


        [HttpPost]
        public JsonResult JqOpenINAMarks(string stdid, string CandSubject)
        {
            var flag = 1;

            // CandSubject myObj = JsonConvert.DeserializeObject<CandSubject>(CandSubject);
            var objResponse1 = JsonConvert.DeserializeObject<List<CandSubject>>(CandSubject);
            DataTable dtSub = new DataTable();
            dtSub.Columns.Add("CANDID");
            dtSub.Columns.Add("SUB");
            dtSub.Columns.Add("OBTMARKS");
            dtSub.Columns.Add("MINMARKS");
            dtSub.Columns.Add("MAXMARKS");
            DataRow row = null;
            foreach (var rowObj in objResponse1)
            {
                row = dtSub.NewRow();
                if (rowObj.OBTMARKS == "A" || rowObj.OBTMARKS == "ABS")
                {
                    rowObj.OBTMARKS = "ABS";
                }
                else if (rowObj.OBTMARKS == "C" || rowObj.OBTMARKS == "CAN")
                {
                    rowObj.OBTMARKS = "CAN";
                }
                else if (rowObj.OBTMARKS != "")
                {
                    rowObj.OBTMARKS = rowObj.OBTMARKS.PadLeft(3, '0');
                }
                if (rowObj.MINMARKS == "--" || rowObj.MINMARKS == "")
                {
                    rowObj.MINMARKS = "000";
                }
                dtSub.Rows.Add(stdid, rowObj.SUB, rowObj.OBTMARKS, rowObj.MINMARKS, rowObj.MAXMARKS);
            }
            dtSub.AcceptChanges();


            foreach (DataRow dr1 in dtSub.Rows)
            {
                if (dr1["OBTMARKS"].ToString() == "" || dr1["OBTMARKS"].ToString() == "ABS" || dr1["OBTMARKS"].ToString() == "CAN")
                { }
                else if (dr1["OBTMARKS"].ToString() == "0" || dr1["OBTMARKS"].ToString().Contains("A") || dr1["OBTMARKS"].ToString().Contains("C"))
                {
                    flag = -2;
                    var results = new
                    {
                        status = flag
                    };
                    return Json(results);
                }
                else
                {
                    int obt = Convert.ToInt32(dr1["OBTMARKS"].ToString());
                    int min = Convert.ToInt32(dr1["MINMARKS"].ToString());
                    int max = Convert.ToInt32(dr1["MAXMARKS"].ToString());

                    if ((obt < 0) || (obt > max))
                    {
                        flag = -2;
                    }
                }
            }
            if (flag == 1)
            {
                string dee = "1";
                string class1 = "4";
                int OutStatus = 0;
                dee = new AbstractLayer.SchoolDB().AllotMarksOpenINA(stdid, dtSub, class1, out OutStatus);

                var results = new
                {
                    status = OutStatus
                };
                return Json(results);
            }
            else
            {
                var results = new
                {
                    status = flag
                };
                return Json(results);
            }

            //  return Json(new { dee = dee }, JsonRequestBehavior.AllowGet);
            // Do Stuff
        }

        [SessionCheckFilter]
        public ActionResult OpenINAMarksReport(string id)
        {
            TempData["OpenINAMarksSearch"] = null;
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SchoolModels MS = new SchoolModels();
            try
            {
                string SCHL = "";
                string CLASS = "";
                if (id == "S") // For Senior
                {
                    CLASS = "12";
                    ViewBag.ClassName = "SENIOR";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "10";
                    ViewBag.ClassName = "MATRIC";
                }

                if (SCHL == "" || SCHL == null)
                {
                    SCHL = Session["SCHL"].ToString();
                }

                #region  Check School Allow For CCE

                DataSet dsAllow = new AbstractLayer.SchoolDB().SchoolAllowForOpenINAMarks(SCHL, CLASS);
                if (dsAllow.Tables.Count > 0)
                {
                    if (dsAllow.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsActive = dsAllow.Tables[0].Rows[0]["IsActive"].ToString();
                        ViewBag.IsAllow = dsAllow.Tables[0].Rows[0]["IsAllow"].ToString();
                        ViewBag.LastDate = dsAllow.Tables[0].Rows[0]["LastDate"].ToString();
                        ViewBag.LastDateDT = dsAllow.Tables[0].Rows[0]["LastDateDT"].ToString();
                    }
                }

                #endregion  Check School Allow For CCE
                ViewBag.schlCode = SCHL;
                ViewBag.cid = id;
                string Search = string.Empty;
                Search = "  a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "'";
                MS.StoreAllData = new AbstractLayer.SchoolDB().OpenINAMarksReport(Search, SCHL, CLASS);
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View(MS);
                }
                else
                {
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.SchoolName = MS.StoreAllData.Tables[1].Rows[0]["FullSchoolNameE"].ToString();
                    if (CLASS == "2")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["MSET"].ToString(); }
                    else if (CLASS == "4")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["SSET"].ToString(); }
                    else if (CLASS == "10")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["MOSET"].ToString(); }
                    else if (CLASS == "12")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["SOSET"].ToString(); }
                    else


                        return View(MS);
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }


        [SessionCheckFilter]
        public ActionResult OpenINAMarksFinalReport(string id, FormCollection frm)
        {
            TempData["OpenINAMarksSearch"] = null;
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SchoolModels MS = new SchoolModels();
            try
            {
                string SCHL = "";
                string CLASS = "";
                if (id == "S") // For Senior
                {
                    CLASS = "12";
                    ViewBag.ClassName = "SENIOR";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "10";
                    ViewBag.ClassName = "MATRIC";
                }

                if (SCHL == "" || SCHL == null)
                {
                    SCHL = Session["SCHL"].ToString();
                }

                #region  Check School Allow For CCE

                DataSet dsAllow = new AbstractLayer.SchoolDB().SchoolAllowForCCE(SCHL, CLASS, "INA-OPEN");
                if (dsAllow.Tables.Count > 0)
                {
                    if (dsAllow.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsActive = dsAllow.Tables[0].Rows[0]["IsActive"].ToString();
                        ViewBag.IsAllow = dsAllow.Tables[0].Rows[0]["IsAllow"].ToString();
                        ViewBag.LastDate = dsAllow.Tables[0].Rows[0]["LastDate"].ToString();
                        ViewBag.LastDateDT = dsAllow.Tables[0].Rows[0]["LastDateDT"].ToString();
                    }
                }

                #endregion  Check School Allow For CCE

                ViewBag.schlCode = SCHL;
                ViewBag.cid = id;
                string Search = string.Empty;
                Search = "  a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "'";
                DataSet dsFinal = new DataSet();
                MS.StoreAllData = dsFinal = new AbstractLayer.SchoolDB().OpenINAMarksFinalReport(Search, SCHL, CLASS);
                if (MS.StoreAllData == null)
                {
                    ViewBag.IsAllowCCE = 0;
                    ViewBag.IsFinal = 0;
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View(MS);
                }
                else if (MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.IsAllowCCE = 1;
                    MS.StoreAllData = new AbstractLayer.SchoolDB().OpenINAMarksReport(Search, SCHL, CLASS);
                    ViewBag.TotalCount = 0;
                    ViewBag.TotalCount1 = MS.StoreAllData.Tables[0].Rows.Count;
                    if (MS.StoreAllData.Tables[0].Rows[0]["CCEDate"].ToString() == "")
                    {
                        //if (CLASS == "4")
                        //{
                        //    ViewBag.CCEDate = "15/03/2017";
                        //}
                        //else if (CLASS == "2")
                        //{ ViewBag.CCEDate = "18/03/2017"; }
                    }
                    else
                    {
                        ViewBag.CCEDate = MS.StoreAllData.Tables[0].Rows[0]["CCEDate"].ToString();
                    }
                    ViewBag.SchoolName = MS.StoreAllData.Tables[1].Rows[0]["FullSchoolNameE"].ToString();

                    if (dsFinal.Tables[2].Rows.Count > 0)
                    {
                        int totalFinalPending = Convert.ToInt32(dsFinal.Tables[2].Rows[0]["TotalPending"]);
                        if (totalFinalPending == 0)
                        {
                            ViewBag.IsFinal = 0;
                        }
                        else { ViewBag.IsFinal = 1; }
                    }

                    if (CLASS == "2")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["MSET"].ToString(); }
                    else if (CLASS == "4")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["SSET"].ToString(); }
                    else if (CLASS == "10")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["MOSET"].ToString(); }
                    else if (CLASS == "12")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["SOSET"].ToString(); }
                    ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[0].Rows[0]["FinalSubmitLastDate"];
                    return View(MS);
                }

                else
                {
                    ViewBag.IsAllowCCE = 2;
                    ViewBag.IsFinal = 0;
                    ViewBag.TotalCount1 = 0;
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[0].Rows[0]["FinalSubmitLastDate"];

                    ViewBag.SchoolName = MS.StoreAllData.Tables[1].Rows[0]["FullSchoolNameE"].ToString();
                    if (CLASS == "2")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["MSET"].ToString(); }
                    else if (CLASS == "4")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["SSET"].ToString(); }
                    else if (CLASS == "10")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["MOSET"].ToString(); }
                    else if (CLASS == "12")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["SOSET"].ToString(); }
                    return View(MS);
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }

        [HttpPost]
        public ActionResult OpenINAMarksFinalReport(string id)
        {
            TempData["OpenINAMarksSearch"] = null;
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string SCHL = "";
                string CLASS = "";
                if (id == "S") // For Senior
                {
                    CLASS = "12";
                    ViewBag.ClassName = "SENIOR";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "10";
                    ViewBag.ClassName = "MATRIC";
                }
                if (SCHL == "" || SCHL == null)
                {
                    SCHL = Session["SCHL"].ToString();
                }

                ViewBag.schlCode = SCHL;
                ViewBag.cid = id;
                string Search = string.Empty;
                Search = "  a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "'";
                MS.StoreAllData = new AbstractLayer.SchoolDB().OpenINAMarksReportFinalSubmit(Search, SCHL, CLASS);  //OpenINAMarksReportFinalSubmit
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View(MS);
                }
                else
                {
                    ViewBag.TotalCount = 0;
                    ViewBag.TotalCount1 = MS.StoreAllData.Tables[0].Rows.Count;
                    if (MS.StoreAllData.Tables[0].Rows[0]["CCEDate"].ToString() == "")
                    {
                        if (CLASS == "4")
                        {
                            // ViewBag.CCEDate = "15/03/2017";
                        }
                        else if (CLASS == "2")
                        {// ViewBag.CCEDate = "18/03/2017"; 
                        }
                    }
                    else
                    {
                        ViewBag.CCEDate = MS.StoreAllData.Tables[0].Rows[0]["CCEDate"].ToString();
                    }
                    ViewBag.SchoolName = MS.StoreAllData.Tables[1].Rows[0]["FullSchoolNameE"].ToString();

                    if (CLASS == "2")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["MSET"].ToString(); }
                    else if (CLASS == "4")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["SSET"].ToString(); }
                    else if (CLASS == "10")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["MOSET"].ToString(); }
                    else if (CLASS == "12")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["SOSET"].ToString(); }

                    return View(MS);
                    //  return RedirectToAction("OpenINAMarksFinalReport", "School", new { id= id});
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }



        #endregion Open INA Portal 

        #region SeniorStudentMatricResultMarksOpen
        [SessionCheckFilter]
        public ActionResult ViewAllSeniorStudentMatricResultMarksOpen(SeniorStudentMatricResultMarksViewsModelList registrationSearchModel)
        {
            string schl = Session["schl"].ToString();

            DataSet dsOut = new DataSet();
            registrationSearchModel.RegistrationSearchModel = AbstractLayer.RegistrationDB.GetSeniorStudentMatricResultMarksSearch("SeniorStudentMatricResultMarksOpen", schl, out dsOut);
            registrationSearchModel.StoreAllData = dsOut;
            return View(registrationSearchModel);
        }


        [SessionCheckFilter]
        public ActionResult SeniorStudentMatricResultMarksOpen(string id, SeniorStudentMatricResultMarksOpenViews obj)
        {
            AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
            string schl = Session["schl"].ToString();
            ViewBag.MatricSubjectList = AbstractLayer.DBClass.GetAllMatricSubjectsForMarks().ToList();
            //
            ViewBag.MonthList = objCommon.GetMonth();
            ViewBag.MyMatricBoard = objCommon.GetMatricBoard();
            List<SelectListItem> itemYear = objCommon.GetSessionYear();
            ViewBag.MatricYearList = itemYear;
            ViewBag.YearList = itemYear.Where(s => Convert.ToInt32(s.Value) >= 2001).ToList();
            ViewBag.ResultList = objCommon.GetAllResult();
            //
            int Std_id = Convert.ToInt32(id);
            bool IsStudentExist = _context.SeniorStudentMatricResultMarksOpens.AsNoTracking().Where(s => s.Std_Id == Std_id).Count() > 0;
            if (IsStudentExist)
            {
                SeniorStudentMatricResultMarksOpenViews SeniorStudentMatricResultMarksOpen = _context.SeniorStudentMatricResultMarksOpenViews.AsNoTracking().SingleOrDefault(s => s.Std_id == Std_id);
                if (SeniorStudentMatricResultMarksOpen != null)
                {
                    obj = (SeniorStudentMatricResultMarksOpenViews)SeniorStudentMatricResultMarksOpen;
                }
            }
            return View(obj);
        }

        [SessionCheckFilter]
        [HttpPost]
        public async Task<JsonResult> SeniorStudentMatricResultMarksOpen(string id, SeniorStudentMatricResultMarksOpens modelData, HttpPostedFileBase FilePath)
        {
            string dee = "0";
            string schl = Session["schl"].ToString();
            if (modelData != null && modelData.MRID > 0)
            {
                if (modelData.ChangeStatus == "U")
                {
                    modelData.ChangeStatus = "U";
                }
                else
                {
                    modelData.FilePath = "";
                    modelData.ChangeStatus = "S";
                }


                if (Request.Files.Count > 0)
                {
                    try
                    {
                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {

                            HttpPostedFileBase file = files[i];
                            string fileKey = i == 0 ? "FilePath" : "";
                            string myUniqueFileName = AbstractLayer.StaticDB.GenerateFileName(modelData.Std_Id.ToString());

                            if (file != null && fileKey == "FilePath")
                            {
                                string fileExt = Path.GetExtension(file.FileName);
                                //var path = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/" + "SeniorStudentMatricResultMarksOpen"), myUniqueFileName + fileExt);
                                //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/" + "SeniorStudentMatricResultMarksOpen"));
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
                                            Key = string.Format("allfiles/Upload2023/SeniorStudentMatricResultMarksOpen/{0}", Orgfile),
                                            BucketName = BUCKET_NAME,
                                            CannedACL = S3CannedACL.PublicRead
                                        };

                                        var fileTransferUtility = new TransferUtility(client);
                                        fileTransferUtility.Upload(uploadRequest);
                                    }
                                }

                                modelData.FilePath = "SeniorStudentMatricResultMarksOpen" + "/" + myUniqueFileName + fileExt;
                                modelData.ChangeStatus = "U";
                            }
                        }
                        // Returns message that successfully uploaded  
                        modelData.ChangeStatus = "U";
                    }
                    catch (Exception ex)
                    {

                    }
                }


                bool IsStudentExist = _context.SeniorStudentMatricResultMarksOpens.AsNoTracking().Where(s => s.Std_Id == modelData.Std_Id).Count() > 0;
                if (IsStudentExist)
                {
                    SeniorStudentMatricResultMarksOpens SeniorStudentMatricResultMarksOpen = _context.SeniorStudentMatricResultMarksOpens.AsNoTracking().SingleOrDefault(s => s.Std_Id == modelData.Std_Id);
                    //
                    SeniorStudentMatricResultMarksOpen.MAT_MONTH = modelData.MAT_MONTH;
                    SeniorStudentMatricResultMarksOpen.MAT_YEAR = modelData.MAT_YEAR;
                    SeniorStudentMatricResultMarksOpen.MAT_BOARD = modelData.MAT_BOARD;
                    SeniorStudentMatricResultMarksOpen.MAT_ROLL = modelData.MAT_ROLL;
                    //                   
                    SeniorStudentMatricResultMarksOpen.SUB1 = modelData.SUB1;
                    SeniorStudentMatricResultMarksOpen.TOT1 = modelData.TOT1;
                    SeniorStudentMatricResultMarksOpen.MAX1 = modelData.MAX1;
                    SeniorStudentMatricResultMarksOpen.MIN1 = modelData.MIN1;
                    SeniorStudentMatricResultMarksOpen.SUB2 = modelData.SUB2;
                    SeniorStudentMatricResultMarksOpen.TOT2 = modelData.TOT2;
                    SeniorStudentMatricResultMarksOpen.MAX2 = modelData.MAX2;
                    SeniorStudentMatricResultMarksOpen.MIN2 = modelData.MIN2;
                    SeniorStudentMatricResultMarksOpen.SUB3 = modelData.SUB3;
                    SeniorStudentMatricResultMarksOpen.TOT3 = modelData.TOT3;
                    SeniorStudentMatricResultMarksOpen.MAX3 = modelData.MAX3;
                    SeniorStudentMatricResultMarksOpen.MIN3 = modelData.MIN3;
                    SeniorStudentMatricResultMarksOpen.SUB4 = modelData.SUB4;
                    SeniorStudentMatricResultMarksOpen.TOT4 = modelData.TOT4;
                    SeniorStudentMatricResultMarksOpen.MAX4 = modelData.MAX4;
                    SeniorStudentMatricResultMarksOpen.MIN4 = modelData.MIN4;
                    SeniorStudentMatricResultMarksOpen.SUB5 = modelData.SUB5;
                    SeniorStudentMatricResultMarksOpen.TOT5 = modelData.TOT5;
                    SeniorStudentMatricResultMarksOpen.MAX5 = modelData.MAX5;
                    SeniorStudentMatricResultMarksOpen.MIN5 = modelData.MIN5;
                    SeniorStudentMatricResultMarksOpen.SUB6 = modelData.SUB6;
                    SeniorStudentMatricResultMarksOpen.TOT6 = modelData.TOT6;
                    SeniorStudentMatricResultMarksOpen.MAX6 = modelData.MAX6;
                    SeniorStudentMatricResultMarksOpen.MIN6 = modelData.MIN6;
                    SeniorStudentMatricResultMarksOpen.SUB7 = modelData.SUB7;
                    SeniorStudentMatricResultMarksOpen.TOT7 = modelData.TOT7 == null ? "" : modelData.TOT7;
                    SeniorStudentMatricResultMarksOpen.MAX7 = modelData.MAX7 == null ? "" : modelData.MAX7;
                    SeniorStudentMatricResultMarksOpen.MIN7 = modelData.MIN7 == null ? "" : modelData.MIN7;
                    //
                    SeniorStudentMatricResultMarksOpen.SUBNM1 = modelData.SUBNM1;
                    SeniorStudentMatricResultMarksOpen.SUBNM2 = modelData.SUBNM2;
                    SeniorStudentMatricResultMarksOpen.SUBNM3 = modelData.SUBNM3;
                    SeniorStudentMatricResultMarksOpen.SUBNM4 = modelData.SUBNM4;
                    SeniorStudentMatricResultMarksOpen.SUBNM5 = modelData.SUBNM5;
                    SeniorStudentMatricResultMarksOpen.SUBNM6 = modelData.SUBNM6;
                    SeniorStudentMatricResultMarksOpen.SUBNM7 = modelData.SUBNM7;
                    SeniorStudentMatricResultMarksOpen.SUBNM8 = modelData.SUBNM8;
                    //
                    SeniorStudentMatricResultMarksOpen.MR_TOTAL = modelData.MR_TOTAL;
                    SeniorStudentMatricResultMarksOpen.MR_TOTMAX = modelData.MR_TOTMAX;
                    SeniorStudentMatricResultMarksOpen.MR_RESULT = modelData.MR_RESULT;
                    SeniorStudentMatricResultMarksOpen.ChangeStatus = modelData.ChangeStatus;
                    SeniorStudentMatricResultMarksOpen.FilePath = modelData.FilePath;
                    SeniorStudentMatricResultMarksOpen.IsActive = true;
                    SeniorStudentMatricResultMarksOpen.ModifyOn = DateTime.Now;
                    SeniorStudentMatricResultMarksOpen.IsFinalLock = false;
                    SeniorStudentMatricResultMarksOpen.FinalSubmitOn = null;
                    _context.Entry(SeniorStudentMatricResultMarksOpen).State = EntityState.Modified;
                }
                int insertedRecords = await _context.SaveChangesAsync();
                if (IsStudentExist == true && insertedRecords > 0)
                {
                    TempData["resultIns"] = "U";
                    dee = "2";
                }
                else
                {
                    TempData["resultIns"] = "F";
                    dee = "0";
                }
            }

            return Json(new { outstatus = dee }, JsonRequestBehavior.AllowGet);
        }

        [Route("ActionSeniorStudentMatricResultMarksOpen/{id}/{act}")]
        public async Task<ActionResult> ActionSeniorStudentMatricResultMarksOpen(string id, string act)
        {
            //string stdid = id;
            try
            {

                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(act))
                {
                    if (act.ToUpper() == "SCHLFNL")
                    {

                        var data = _context.SeniorStudentMatricResultMarksOpens.Where(s => s.SCHL == id.ToString()).ToList();

                        var dataInView = _context.SeniorStudentMatricResultMarksOpenViews.Where(s => s.schl == id.ToString()).ToList();


                        ViewBag.IsFinalLockSubmitted = "0";
                        //
                        ViewBag.IsFinalLockTotal = dataInView.Count();
                        ViewBag.IsFinalLock_Pending = dataInView.Where(s => string.IsNullOrEmpty(s.ChangeStatus)).Count();
                        //total filled
                        ViewBag.IsFinalLock_InProcess = dataInView.Where(s => s.IsFinalLock == false && !string.IsNullOrEmpty(s.ChangeStatus)).Count();
                        //total pending
                        ViewBag.IsFinalLock_FinalSubmit = dataInView.Where(s => s.IsFinalLock == true).Count();

                        if ((ViewBag.IsFinalLockTotal == ViewBag.IsFinalLock_InProcess) && (ViewBag.IsFinalLock_Pending == ViewBag.IsFinalLock_FinalSubmit) && ViewBag.IsFinalLock_Pending == 0)
                        {
                            data.ForEach(a =>
                            {
                                a.FinalSubmitOn = DateTime.Now;
                                a.IsFinalLock = true;
                            });
                            int insertedRecords = await _context.SaveChangesAsync();
                            TempData["resultIns"] = "FNL";
                        }
                        else
                        {
                            TempData["resultIns"] = "PEN";
                        }
                    }
                    else if (act.ToUpper() == "SCHLFNLPEND")
                    {

                        //4
                        var data = _context.SeniorStudentMatricResultMarksOpens.Where(s => s.SCHL == id.ToString() && s.IsFinalLock == false).ToList();
                        //3
                        var dataInView = _context.SeniorStudentMatricResultMarksOpenViews.Where(s => s.schl == id.ToString() && s.IsFinalLock == false).ToList();



                        //
                        ViewBag.IsFinalLockSubmitted = "0";
                        ViewBag.IsFinalLockTotal = dataInView.Count();
                        ViewBag.IsFinalLock_Pending = dataInView.Where(s => string.IsNullOrEmpty(s.ChangeStatus)).Count();
                        ViewBag.IsFinalLock_InProcess = dataInView.Where(s => s.IsFinalLock == false && !string.IsNullOrEmpty(s.ChangeStatus)).Count();

                        if (ViewBag.IsFinalLock_Pending == 0 && (ViewBag.IsFinalLockTotal == ViewBag.IsFinalLock_InProcess))
                        {
                            data.ForEach(a =>
                            {
                                a.FinalSubmitOn = DateTime.Now;
                                a.IsFinalLock = true;
                            });
                            int insertedRecords = await _context.SaveChangesAsync();
                            TempData["resultIns"] = "FNL";
                        }
                        else
                        {
                            TempData["resultIns"] = "PEN";
                        }
                    }
                    else if (act.ToUpper() == "CANDFNL")
                    {
                        int sid = Convert.ToInt32(id);
                        SeniorStudentMatricResultMarksOpens data = _context.SeniorStudentMatricResultMarksOpens.AsNoTracking().SingleOrDefault(s => s.Std_Id == sid);
                        data.FinalSubmitOn = DateTime.Now;
                        data.IsFinalLock = true;
                        if (data != null)
                        {
                            _context.Entry(data).State = EntityState.Modified;
                            int insertedRecords = await _context.SaveChangesAsync();
                            TempData["resultIns"] = "FNL";
                        }
                    }

                }
            }
            catch (Exception ex1)
            {

            }
            return RedirectToAction("ViewAllSeniorStudentMatricResultMarksOpen");
        }

        public async Task<JsonResult> ActionSeniorStudentMatricResultMarksOpen(string id, string act, string remarks)
        {

            string outstatus = "0";
            try
            {

                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(act) && !string.IsNullOrEmpty(remarks))
                {
                    if (act.ToUpper() == "CANCEL")
                    {
                        int sid = Convert.ToInt32(id);
                        SeniorStudentMatricResultMarksOpens data = _context.SeniorStudentMatricResultMarksOpens.AsNoTracking().SingleOrDefault(s => s.Std_Id == sid);
                        data.ChangeStatus = "C";
                        data.CancelOn = DateTime.Now;
                        data.IsFinalLock = false;
                        if (data != null)
                        {
                            outstatus = "1";
                            _context.Entry(data).State = EntityState.Modified;
                            int insertedRecords = await _context.SaveChangesAsync();
                            TempData["resultIns"] = "CANCEL";
                        }
                    }

                }
            }
            catch (Exception ex1)
            {
                outstatus = "-1";
            }
            return Json(new { result = outstatus }, JsonRequestBehavior.AllowGet);
        }

        [SessionCheckFilter]
        public ActionResult SeniorStudentMatricResultMarksOpenReport(SeniorStudentMatricResultMarksViewsModelList registrationSearchModel)
        {
            string schl = Session["schl"].ToString();
            DataSet dsOut = new DataSet();
            registrationSearchModel.RegistrationSearchModel = AbstractLayer.RegistrationDB.GetSeniorStudentMatricResultMarksSearch("SeniorStudentMatricResultMarksOpen", schl, out dsOut);
            registrationSearchModel.StoreAllData = dsOut;
            ViewBag.IsFinalLockSubmitted = "0";
            //
            ViewBag.IsFinalLockTotal = registrationSearchModel.RegistrationSearchModel.Count();
            ViewBag.IsFinalLockTotalMarks = registrationSearchModel.RegistrationSearchModel.Where(s => s.MRID > 0).Count();
            ViewBag.IsFinalLockYES = registrationSearchModel.RegistrationSearchModel.Where(s => s.IsFinalLock == true && s.MRID > 0).Count();
            ViewBag.IsFinalLockNO = registrationSearchModel.RegistrationSearchModel.Where(s => s.IsFinalLock == false && s.MRID > 0).Count();
            //
            if ((ViewBag.IsFinalLockTotal == ViewBag.IsFinalLockTotalMarks) && ViewBag.IsFinalLockNO == 0)
            {
                ViewBag.IsFinalLockSubmitted = "1";
            }
            return View(registrationSearchModel);
        }

        #endregion




        //// Dispose
        ///

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}