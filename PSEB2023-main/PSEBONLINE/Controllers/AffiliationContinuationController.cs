using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSEBONLINE.Models;
using System.Data;
using System.Web.UI;
using System.IO;
using PSEBONLINE.Filters;
using System.Configuration;
using PsebPrimaryMiddle.Controllers;
using CCA.Util;
using FilterSkipping.Filters;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon;
using Amazon.S3.IO;

namespace PSEBONLINE.Controllers
{
    [SessionCheckFilter]
    
    public class AffiliationContinuationController : Controller
    {
        private readonly DBContext _context = new DBContext();
        public AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        public AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        public AbstractLayer.AffiliationDB affiliationDB = new AbstractLayer.AffiliationDB();
        public AbstractLayer.SchoolDB ObjSchoolDB = new AbstractLayer.SchoolDB();

        private const string BUCKET_NAME = "psebdata";

        // GET: AffiliationContinuation
        //public ActionResult Index(AffiliationModel am)
        //{

        //    ViewBag.SCHL = Session["SCHL"].ToString();

        //    DataSet outDs = new DataSet();
        //    am = affiliationDB.AffiliationContinuationBySchl(Session["SCHL"].ToString(), 1, out outDs);//ResultStatics
        //    if (am.ID > 0)
        //    {
        //        ViewBag.Totalcount = 1;         
        //        return View(am);
        //    }
        //    else {
        //        ViewBag.Totalcount = 0;               
        //        return View(am);
        //    }

        //}

        public ActionResult Index(AffiliationModel am)
        {
            AffiliationContinuationDashBoardViews _DashBoardModel = new AffiliationContinuationDashBoardViews();
            am.affiliationContinuationDashBoardViews = new AffiliationContinuationDashBoardViews();

            string schl = Session["SCHL"].ToString();
            ViewBag.SCHL = schl;

            DataSet outDs = new DataSet();
            am = affiliationDB.AffiliationContinuationBySchl(Session["SCHL"].ToString(), 1, out outDs);//ResultStatics
            if (am.ID > 0)
            {

                _DashBoardModel = _context.AffiliationContinuationDashBoardViews.Where(s => s.SCHL == schl).FirstOrDefault();
                am.affiliationContinuationDashBoardViews = _DashBoardModel;

                am.affObjectionLettersViewList = _context.AffObjectionLettersViews.Where(s => s.AppNo == schl && s.AppType == "AC").ToList();

                ViewBag.Totalcount = 1;
            }
            else
            {
                ViewBag.Totalcount = 0;

            }
            return View(am);
        }


        #region SchoolProfile
        public ActionResult SchoolProfile(string id, SchoolModels sm)
        {
            try
            {



                if (id == null)
                {
                    return RedirectToAction("Index", "AffiliationContinuation");
                }

                ViewBag.SCHL = Session["SCHL"].ToString();
                if (id != ViewBag.SCHL)
                {
                    return RedirectToAction("Index", "AffiliationContinuation");
                }

                DataSet outDs = new DataSet();
                AffiliationModel am1 = affiliationDB.AffiliationContinuationBySchl(Session["SCHL"].ToString(), 1, out outDs);//ResultStatics
                if (am1.ID > 0)
                {
                    ViewBag.AID = am1.ID;
                    ViewBag.ChallanId = am1.ChallanId;
                    ViewBag.challanVerify = am1.challanVerify;
                    ViewBag.RS10GPass2017 = am1.RS10GPass2017;
                    ViewBag.SF10Percent = am1.SF10Percent;
                    ViewBag.BSFROM = am1.BSFROM;
                    ViewBag.FSFROM = am1.FSFROM;
                    ViewBag.BPNAME = am1.BPNAME;
                    ViewBag.ASZONE = am1.ASZONE;
                    ViewBag.ASZONE = am1.ASZONE;
                    ViewBag.OI1 = am1.OI1;
                }
                else
                {
                    ViewBag.AID = 0;
                    ViewBag.ChallanId = 0;
                    ViewBag.challanVerify = 0;
                }

                DataSet ds = ObjSchoolDB.SelectSchoolDatabyID(id);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    ViewData["resultSVF"] = 2;
                }
                else
                {
                    sm.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                    sm.SCHL = ds.Tables[0].Rows[0]["schl"].ToString();
                    sm.idno = ds.Tables[0].Rows[0]["idno"].ToString();
                    sm.CLASS = ds.Tables[0].Rows[0]["CLASS"].ToString();
                    sm.OCODE = ds.Tables[0].Rows[0]["OCODE"].ToString();
                    sm.USERTYPE = ds.Tables[0].Rows[0]["USERTYPE"].ToString();
                    sm.session = ds.Tables[0].Rows[0]["SESSION"].ToString();
                    sm.status = ds.Tables[0].Rows[0]["status"].ToString();
                    sm.Approved = ds.Tables[0].Rows[0]["IsApproved"].ToString() == "Y" ? "YES" : "NO";
                    sm.vflag = ds.Tables[0].Rows[0]["IsVerified"].ToString() == "Y" ? "YES" : "NO";
                    sm.AREA = ds.Tables[0].Rows[0]["SchoolArea"].ToString();
                    sm.VALIDITY = ds.Tables[0].Rows[0]["VALIDITY"].ToString();
                    sm.PASSWORD = ds.Tables[0].Rows[0]["PASSWORD"].ToString();
                    sm.ACTIVE = ds.Tables[0].Rows[0]["ACTIVE"].ToString();
                    sm.ADDRESSE = ds.Tables[0].Rows[0]["ADDRESSE"].ToString();
                    sm.ADDRESSP = ds.Tables[0].Rows[0]["ADDRESSP"].ToString();
                    sm.AGRI = ds.Tables[0].Rows[0]["AGRI"].ToString();
                    sm.SCHLE = ds.Tables[0].Rows[0]["SCHLE"].ToString();
                    sm.dist = ds.Tables[0].Rows[0]["DIST"].ToString();
                    sm.DISTE = ds.Tables[0].Rows[0]["DISTE"].ToString();
                    sm.STATIONE = ds.Tables[0].Rows[0]["STATIONE"].ToString();
                    sm.SCHLP = ds.Tables[0].Rows[0]["SCHLP"].ToString();
                    sm.DISTP = ds.Tables[0].Rows[0]["DISTP"].ToString();
                    sm.STATIONP = ds.Tables[0].Rows[0]["STATIONP"].ToString();
                    sm.DISTNM = ds.Tables[0].Rows[0]["DISTNM"].ToString();
                    sm.DISTNMPun = ds.Tables[0].Rows[0]["DISTNMPun"].ToString();


                    sm.DOB = ds.Tables[0].Rows[0]["DOB"].ToString();
                    sm.DOJ = ds.Tables[0].Rows[0]["DOJ"].ToString();
                    sm.ExperienceYr = ds.Tables[0].Rows[0]["ExperienceYr"].ToString();
                    sm.PQualification = ds.Tables[0].Rows[0]["PQualification"].ToString();

                    sm.PRINCIPAL = ds.Tables[0].Rows[0]["PRINCIPAL"].ToString();
                    sm.STDCODE = ds.Tables[0].Rows[0]["STDCODE"].ToString();
                    sm.PHONE = ds.Tables[0].Rows[0]["PHONE"].ToString();
                    sm.MOBILE = ds.Tables[0].Rows[0]["MOBILE"].ToString();
                    sm.mobile2 = ds.Tables[0].Rows[0]["mobile2"].ToString();
                    sm.ADDRESSE = ds.Tables[0].Rows[0]["ADDRESSE"].ToString();
                    sm.ADDRESSP = ds.Tables[0].Rows[0]["ADDRESSP"].ToString();
                    sm.CONTACTPER = ds.Tables[0].Rows[0]["CONTACTPER"].ToString();
                    sm.CPPHONE = ds.Tables[0].Rows[0]["CONTACTPER"].ToString();
                    sm.CPSTD = ds.Tables[0].Rows[0]["CPSTD"].ToString();
                    sm.OtContactno = ds.Tables[0].Rows[0]["OtContactno"].ToString();
                    sm.EMAILID = ds.Tables[0].Rows[0]["EMAILID"].ToString();
                    sm.NSQF_flag = ds.Tables[0].Rows[0]["NSQF_flag"].ToString() == "Y" ? "YES" : "NO";

                    sm.REMARKS = ds.Tables[0].Rows[0]["REMARKS"].ToString();
                    sm.UDATE = ds.Tables[0].Rows[0]["UDATE"].ToString();
                    sm.correctionno = ds.Tables[0].Rows[0]["correctionno"].ToString();


                    sm.MSET = ViewBag.MSET = ds.Tables[0].Rows[0]["MSET"].ToString();
                    sm.MOSET = ViewBag.MOSET = ds.Tables[0].Rows[0]["MOSET"].ToString();
                    sm.SSET = ViewBag.SSET = ds.Tables[0].Rows[0]["SSET"].ToString();
                    sm.SOSET = ViewBag.SOSET = ds.Tables[0].Rows[0]["SOSET"].ToString();

                    //Regular

                    sm.middle = ds.Tables[0].Rows[0]["middle"].ToString() == "Y" ? "YES" : "NO";
                    sm.MATRIC = ds.Tables[0].Rows[0]["MATRIC"].ToString() == "Y" ? "YES" : "NO";
                    sm.HUM = ds.Tables[0].Rows[0]["HUM"].ToString() == "Y" ? "YES" : "NO";
                    sm.SCI = ds.Tables[0].Rows[0]["SCI"].ToString() == "Y" ? "YES" : "NO";
                    sm.COMM = ds.Tables[0].Rows[0]["COMM"].ToString() == "Y" ? "YES" : "NO";
                    sm.VOC = ds.Tables[0].Rows[0]["VOC"].ToString() == "Y" ? "YES" : "NO";
                    sm.TECH = ds.Tables[0].Rows[0]["TECH"].ToString() == "Y" ? "YES" : "NO";
                    sm.AGRI = ds.Tables[0].Rows[0]["AGRI"].ToString() == "Y" ? "YES" : "NO";

                    //OPen
                    sm.omiddle = ds.Tables[0].Rows[0]["omiddle"].ToString() == "Y" ? "YES" : "NO";
                    sm.OMATRIC = ds.Tables[0].Rows[0]["OMATRIC"].ToString() == "Y" ? "YES" : "NO";
                    sm.OHUM = ds.Tables[0].Rows[0]["OHUM"].ToString() == "Y" ? "YES" : "NO";
                    sm.OSCI = ds.Tables[0].Rows[0]["OSCI"].ToString() == "Y" ? "YES" : "NO";
                    sm.OCOMM = ds.Tables[0].Rows[0]["OCOMM"].ToString() == "Y" ? "YES" : "NO";
                    sm.OVOC = ds.Tables[0].Rows[0]["OVOC"].ToString() == "Y" ? "YES" : "NO";
                    sm.OTECH = ds.Tables[0].Rows[0]["OTECH"].ToString() == "Y" ? "YES" : "NO";
                    sm.OAGRI = ds.Tables[0].Rows[0]["OAGRI"].ToString() == "Y" ? "YES" : "NO";


                    //New

                    sm.MID_CR = ViewBag.MID_CR = ds.Tables[0].Rows[0]["MID_CR"].ToString();
                    sm.MID_NO = ViewBag.MID_NO = ds.Tables[0].Rows[0]["MID_NO"].ToString();
                    sm.MID_YR = ViewBag.MID_YR = ds.Tables[0].Rows[0]["MID_YR"].ToString();
                    sm.MID_S = ViewBag.MID_S = Convert.ToInt32(ds.Tables[0].Rows[0]["MID_S"].ToString());
                    sm.MID_DNO = ViewBag.MID_DNO = ds.Tables[0].Rows[0]["MID_DNO"].ToString();

                    sm.HID_CR = ViewBag.HID_CR = ds.Tables[0].Rows[0]["HID_CR"].ToString();
                    sm.HID_NO = ViewBag.HID_NO = ds.Tables[0].Rows[0]["HID_NO"].ToString();
                    sm.HID_YR = ViewBag.HID_YR = ds.Tables[0].Rows[0]["HID_YR"].ToString();
                    sm.HID_S = ViewBag.HID_S = Convert.ToInt32(ds.Tables[0].Rows[0]["HID_S"].ToString());
                    sm.HID_DNO = ViewBag.HID_DNO = ds.Tables[0].Rows[0]["HID_DNO"].ToString();

                    sm.SID_CR = ViewBag.SID_CR = ds.Tables[0].Rows[0]["SID_CR"].ToString();
                    sm.SID_NO = ViewBag.SID_NO = ds.Tables[0].Rows[0]["SID_NO"].ToString();
                    sm.SID_DNO = ViewBag.SID_DNO = ds.Tables[0].Rows[0]["SID_DNO"].ToString();
                    sm.H = ViewBag.H = ds.Tables[0].Rows[0]["H"].ToString();
                    sm.HYR = ViewBag.HYR = ds.Tables[0].Rows[0]["HYR"].ToString();

                    sm.H_S = ViewBag.H_S = Convert.ToInt32(ds.Tables[0].Rows[0]["H_S"].ToString());
                    sm.C = ViewBag.C = ds.Tables[0].Rows[0]["C"].ToString();
                    sm.CYR = ViewBag.CYR = ds.Tables[0].Rows[0]["CYR"].ToString();
                    sm.C_S = ViewBag.C_S = Convert.ToInt32(ds.Tables[0].Rows[0]["C_S"].ToString());
                    sm.S = ViewBag.S = ds.Tables[0].Rows[0]["S"].ToString();
                    sm.SYR = ViewBag.SYR = ds.Tables[0].Rows[0]["SYR"].ToString();
                    sm.S_S = ViewBag.S_S = Convert.ToInt32(ds.Tables[0].Rows[0]["S_S"].ToString());

                    sm.A = ViewBag.A = ds.Tables[0].Rows[0]["A"].ToString();
                    sm.AYR = ViewBag.AYR = ds.Tables[0].Rows[0]["AYR"].ToString();
                    sm.A_S = ViewBag.A_S = Convert.ToInt32(ds.Tables[0].Rows[0]["A_S"].ToString());

                    sm.V = ViewBag.V = ds.Tables[0].Rows[0]["V"].ToString();
                    sm.VYR = ViewBag.VYR = ds.Tables[0].Rows[0]["VYR"].ToString();
                    sm.V_S = ViewBag.V_S = Convert.ToInt32(ds.Tables[0].Rows[0]["V_S"].ToString());

                    sm.T = ViewBag.T = ds.Tables[0].Rows[0]["T"].ToString();
                    sm.TYR = ViewBag.TYR = ds.Tables[0].Rows[0]["TYR"].ToString();
                    sm.T_S = ViewBag.T_S = Convert.ToInt32(ds.Tables[0].Rows[0]["T_S"].ToString());


                    sm.MID_UTYPE = ViewBag.MID_UTYPE = ds.Tables[0].Rows[0]["MID_UTYPEFull"].ToString();
                    sm.HID_UTYPE = ViewBag.HID_UTYPE = ds.Tables[0].Rows[0]["HID_UTYPEFull"].ToString();
                    sm.H_UTYPE = ViewBag.H_UTYPE = ds.Tables[0].Rows[0]["H_UTYPEFull"].ToString();
                    sm.S_UTYPE = ViewBag.S_UTYPE = ds.Tables[0].Rows[0]["S_UTYPEFull"].ToString();
                    sm.C_UTYPE = ViewBag.C_UTYPE = ds.Tables[0].Rows[0]["C_UTYPEFull"].ToString();
                    sm.V_UTYPE = ViewBag.V_UTYPE = ds.Tables[0].Rows[0]["V_UTYPEFull"].ToString();
                    sm.A_UTYPE = ViewBag.A_UTYPE = ds.Tables[0].Rows[0]["A_UTYPEFull"].ToString();
                    sm.T_UTYPE = ViewBag.T_UTYPE = ds.Tables[0].Rows[0]["T_UTYPEFull"].ToString();

                    sm.Tcode = ViewBag.Tcode = ds.Tables[0].Rows[0]["Tcode"].ToString();
                    sm.Tehsile = ViewBag.omiddle = ds.Tables[0].Rows[0]["Tehsile"].ToString();
                    sm.Tehsilp = ViewBag.omiddle = ds.Tables[0].Rows[0]["Tehsilp"].ToString();



                    Session["TotalTeachers"] = null;
                    if (ds.Tables[4].Rows.Count > 0)
                    {
                        ViewBag.TotalTeachers = ds.Tables[4].Rows.Count;
                        Session["TotalTeachers"] = ds.Tables[4];
                    }
                    else
                    {
                        ViewBag.TotalTeachers = 0;
                    }
                }
                return View(sm);
            }
            catch (Exception ex)
            {

                return View();
            }
        }

        public ActionResult ViewTeacherList(string id, SchoolModels sm)
        {
            if (Session["SCHL"] != null && id == Session["SCHL"].ToString())
            {
                ViewBag.SCHL = Session["SCHL"].ToString();
                sm.StoreAllData = ObjSchoolDB.SelectSchoolDatabyID(id);
                if (sm.StoreAllData == null || sm.StoreAllData.Tables[4].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = sm.StoreAllData.Tables[4].Rows.Count;
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return View(sm);
        }
        #endregion

        #region ResultStatics
        public ActionResult ResultStatics(string id, AffiliationModel am)
        {

            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }

            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }

            DataSet outDs = new DataSet();
            am = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
            if (am.ID > 0)
            {
                ViewBag.Totalcount = 1;
                return View(am);
            }
            else
            {
                am = affiliationDB.GetAffiliationContinuationBySchlAndType(id, 1, out outDs);//ResultStatics
                if (am.SCHL != "")
                {
                    ViewBag.Totalcount = 1;
                    return View(am);
                }
                else
                {
                    ViewBag.Totalcount = 0;
                }
            }
            return View(am);
        }

        [HttpPost]
        public ActionResult ResultStatics(string id, AffiliationModel am, FormCollection fc)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            am.SCHL = id;
            DataSet outDs = new DataSet();
            AffiliationModel amMain = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
            if (amMain.ID > 0)
            {
                amMain.RS10GTotal2017 = am.RS10GTotal2017;
                amMain.RS10GPass2017 = am.RS10GPass2017;
                amMain.RS10GPercent2017 = am.RS10GPercent2017;
                amMain.RS10GTotal2018 = am.RS10GTotal2018;
                amMain.RS10GPass2018 = am.RS10GPass2018;
                amMain.RS10GPercent2018 = am.RS10GPercent2018;
                amMain.RS12HTotal2017 = am.RS12HTotal2017;
                amMain.RS12HPass2017 = am.RS12HPass2017;
                amMain.RS12HPercent2017 = am.RS12HPercent2017;
                amMain.RS12HTotal2018 = am.RS12HTotal2018;
                amMain.RS12HPass2018 = am.RS12HPass2018;
                amMain.RS12HPercent2018 = am.RS12HPercent2018;
                amMain.RS12STotal2017 = am.RS12STotal2017;
                amMain.RS12SPass2017 = am.RS12SPass2017;
                amMain.RS12SPercent2017 = am.RS12SPercent2017;
                amMain.RS12STotal2018 = am.RS12STotal2018;
                amMain.RS12SPass2018 = am.RS12SPass2018;
                amMain.RS12SPercent2018 = am.RS12SPercent2018;
                amMain.RS12CTotal2017 = am.RS12CTotal2017;
                amMain.RS12CPass2017 = am.RS12CPass2017;
                amMain.RS12CPercent2017 = am.RS12CPercent2017;
                amMain.RS12CTotal2018 = am.RS12CTotal2018;
                amMain.RS12CPass2018 = am.RS12CPass2018;
                amMain.RS12CPercent2018 = am.RS12CPercent2018;
                amMain.RS12VTotal2017 = am.RS12VTotal2017;
                amMain.RS12VPass2017 = am.RS12VPass2017;
                amMain.RS12VPercent2017 = am.RS12VPercent2017;
                amMain.RS12VTotal2018 = am.RS12VTotal2018;
                amMain.RS12VPass2018 = am.RS12VPass2018;
                amMain.RS12VPercent2018 = am.RS12VPercent2018;
                am = amMain;
            }
            string outError = "0";
            int result = affiliationDB.AffiliationContinuation(am, out outError);// if ID=0 then Insert else Update
            if (result > 0)
            {
                ViewBag.Totalcount = 1;
                ViewBag.AID = am.ID;
                ViewBag.result = result;
                ViewData["result"] = 1;
            }
            else
            {
                ViewBag.Totalcount = 0;
                ViewBag.AID = am.ID;
                ViewData["result"] = 0;
                ViewBag.Message = outError.ToString();
            }
            return View(am);
        }
        #endregion

        #region StudentFeeDetails
        public ActionResult StudentFeeDetails(string id, AffiliationModel am)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }

            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }
            DataSet ds = new DataSet();
            SchoolPremisesInformation sm = new AbstractLayer.SchoolDB().SchoolPremisesInformationBySchl(ViewBag.SCHL, out ds);
            if (sm.ID == 0)
            {
                DataSet outDs = new DataSet();
                am = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);
                if (am.ID > 0)
                {
                    return View(am);
                }
            }
            else
            {
                DataSet outDs = new DataSet();
                am = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);
                if (sm.ID > 0)
                {
                    am.SF1TC2018 = am.SF1TC2018 > 0 ? am.SF1TC2018 : sm.CWS26;
                    am.SF2TC2018 = am.SF2TC2018 > 0 ? am.SF2TC2018 : sm.CWS27;
                    am.SF3TC2018 = am.SF3TC2018 > 0 ? am.SF3TC2018 : sm.CWS28;
                    am.SF4TC2018 = am.SF4TC2018 > 0 ? am.SF4TC2018 : sm.CWS29;
                    am.SF5TC2018 = am.SF5TC2018 > 0 ? am.SF5TC2018 : sm.CWS30;
                    am.SF6TC2018 = am.SF6TC2018 > 0 ? am.SF6TC2018 : sm.CWS31;
                    am.SF7TC2018 = am.SF7TC2018 > 0 ? am.SF7TC2018 : sm.CWS32;
                    am.SF8TC2018 = am.SF8TC2018 > 0 ? am.SF8TC2018 : sm.CWS33;
                    am.SF9TC2018 = am.SF9TC2018 > 0 ? am.SF9TC2018 : sm.CWS34;
                    am.SF10TC2018 = am.SF10TC2018 > 0 ? am.SF10TC2018 : sm.CWS35;
                    am.SF11HTC2018 = am.SF11HTC2018 > 0 ? am.SF11HTC2018 : sm.CWS37;
                    am.SF11STC2018 = am.SF11STC2018 > 0 ? am.SF11STC2018 : sm.CWS38;
                    am.SF11CTC2018 = am.SF11CTC2018 > 0 ? am.SF11CTC2018 : sm.CWS39;
                    am.SF11VTC2018 = am.SF11VTC2018 > 0 ? am.SF11VTC2018 : sm.CWS40;
                    am.SF12HTC2018 = am.SF12HTC2018 > 0 ? am.SF12HTC2018 : sm.CWS41;
                    am.SF12STC2018 = am.SF12STC2018 > 0 ? am.SF12STC2018 : sm.CWS42;
                    am.SF12CTC2018 = am.SF12CTC2018 > 0 ? am.SF12CTC2018 : sm.CWS43;
                    am.SF12VTC2018 = am.SF12VTC2018 > 0 ? am.SF12VTC2018 : sm.CWS44;
                }
                return View(am);
            }

            return View(am);
        }

        [HttpPost]
        public ActionResult StudentFeeDetails(string id, AffiliationModel am, FormCollection fc)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            if (id == null)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            am.SCHL = id;
            DataSet outDs = new DataSet();
            AffiliationModel amMain = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
            if (amMain.ID > 0)
            {
                amMain.SF1TC2017 = am.SF1TC2017;
                amMain.SF1TF2017 = am.SF1TF2017;
                amMain.SF1TC2018 = am.SF1TC2018;
                amMain.SF1TF2018 = am.SF1TF2018;
                if (am.SF1TC2018 < am.SF1TF2018 && am.SF1TC2017 < am.SF1TF2017)
                {
                    int s1 = (am.SF1TF2018 / am.SF1TC2018);
                    int s2 = (am.SF1TF2017 / am.SF1TC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF1Percent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF1Percent = 0; }


                amMain.SF2TC2017 = am.SF2TC2017;
                amMain.SF2TF2017 = am.SF2TF2017;
                amMain.SF2TC2018 = am.SF2TC2018;
                amMain.SF2TF2018 = am.SF2TF2018;
                if (am.SF2TC2018 < am.SF2TF2018 && am.SF2TC2017 < am.SF2TF2017)
                {
                    int s1 = (am.SF2TF2018 / am.SF2TC2018);
                    int s2 = (am.SF2TF2017 / am.SF2TC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF2Percent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF2Percent = 0; }

                amMain.SF3TC2017 = am.SF3TC2017;
                amMain.SF3TF2017 = am.SF3TF2017;
                amMain.SF3TC2018 = am.SF3TC2018;
                amMain.SF3TF2018 = am.SF3TF2018;
                if (am.SF3TC2018 < am.SF3TF2018 && am.SF3TC2017 < am.SF3TF2017)
                {
                    int s1 = (am.SF3TF2018 / am.SF3TC2018);
                    int s2 = (am.SF3TF2017 / am.SF3TC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF3Percent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF3Percent = 0; }

                amMain.SF4TC2017 = am.SF4TC2017;
                amMain.SF4TF2017 = am.SF4TF2017;
                amMain.SF4TC2018 = am.SF4TC2018;
                amMain.SF4TF2018 = am.SF4TF2018;
                if (am.SF4TC2018 < am.SF4TF2018 && am.SF4TC2017 < am.SF4TF2017)
                {
                    int s1 = (am.SF4TF2018 / am.SF4TC2018);
                    int s2 = (am.SF4TF2017 / am.SF4TC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF4Percent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF4Percent = 0; }

                amMain.SF5TC2017 = am.SF5TC2017;
                amMain.SF5TF2017 = am.SF5TF2017;
                amMain.SF5TC2018 = am.SF5TC2018;
                amMain.SF5TF2018 = am.SF5TF2018;
                if (am.SF5TC2018 < am.SF5TF2018 && am.SF5TC2017 < am.SF5TF2017)
                {
                    int s1 = (am.SF5TF2018 / am.SF5TC2018);
                    int s2 = (am.SF5TF2017 / am.SF5TC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF5Percent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF5Percent = 0; }

                amMain.SF6TC2017 = am.SF6TC2017;
                amMain.SF6TF2017 = am.SF6TF2017;
                amMain.SF6TC2018 = am.SF6TC2018;
                amMain.SF6TF2018 = am.SF6TF2018;
                if (am.SF6TC2018 < am.SF6TF2018 && am.SF6TC2017 < am.SF6TF2017)
                {
                    int s1 = (am.SF6TF2018 / am.SF6TC2018);
                    int s2 = (am.SF6TF2017 / am.SF6TC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF6Percent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF6Percent = 0; }

                amMain.SF7TC2017 = am.SF7TC2017;
                amMain.SF7TF2017 = am.SF7TF2017;
                amMain.SF7TC2018 = am.SF7TC2018;
                amMain.SF7TF2018 = am.SF7TF2018;
                if (am.SF7TC2018 < am.SF7TF2018 && am.SF7TC2017 < am.SF7TF2017)
                {
                    int s1 = (am.SF7TF2018 / am.SF7TC2018);
                    int s2 = (am.SF7TF2017 / am.SF7TC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF7Percent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF7Percent = 0; }

                amMain.SF8TC2017 = am.SF8TC2017;
                amMain.SF8TF2017 = am.SF8TF2017;
                amMain.SF8TC2018 = am.SF8TC2018;
                amMain.SF8TF2018 = am.SF8TF2018;
                if (am.SF8TC2018 < am.SF8TF2018 && am.SF8TC2017 < am.SF8TF2017)
                {
                    int s1 = (am.SF8TF2018 / am.SF8TC2018);
                    int s2 = (am.SF8TF2017 / am.SF8TC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF8Percent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF8Percent = 0; }

                amMain.SF9TC2017 = am.SF9TC2017;
                amMain.SF9TF2017 = am.SF9TF2017;
                amMain.SF9TC2018 = am.SF9TC2018;
                amMain.SF9TF2018 = am.SF9TF2018;
                if (am.SF9TC2018 < am.SF9TF2018 && am.SF9TC2017 < am.SF9TF2017)
                {
                    int s1 = (am.SF9TF2018 / am.SF9TC2018);
                    int s2 = (am.SF9TF2017 / am.SF9TC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF9Percent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF9Percent = 0; }

                amMain.SF10TC2017 = am.SF10TC2017;
                amMain.SF10TF2017 = am.SF10TF2017;
                amMain.SF10TC2018 = am.SF10TC2018;
                amMain.SF10TF2018 = am.SF10TF2018;
                if (am.SF10TC2018 < am.SF10TF2018 && am.SF10TC2017 < am.SF10TF2017)
                {
                    int s1 = (am.SF10TF2018 / am.SF10TC2018);
                    int s2 = (am.SF10TF2017 / am.SF10TC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF10Percent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF10Percent = 0; }

                amMain.SF11HTC2017 = am.SF11HTC2017;
                amMain.SF11HTF2017 = am.SF11HTF2017;
                amMain.SF11HTC2018 = am.SF11HTC2018;
                amMain.SF11HTF2018 = am.SF11HTF2018;
                if (am.SF11HTC2018 < am.SF11HTF2018 && am.SF11HTC2017 < am.SF11HTF2017)
                {
                    int s1 = (am.SF11HTF2018 / am.SF11HTC2018);
                    int s2 = (am.SF11HTF2017 / am.SF11HTC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF11HPercent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF11HPercent = 0; }

                amMain.SF11STC2017 = am.SF11STC2017;
                amMain.SF11STF2017 = am.SF11STF2017;
                amMain.SF11STC2018 = am.SF11STC2018;
                amMain.SF11STF2018 = am.SF11STF2018;
                if (am.SF11STC2018 < am.SF11STF2018 && am.SF11STC2017 < am.SF11STF2017)
                {
                    int s1 = (am.SF11STF2018 / am.SF11STC2018);
                    int s2 = (am.SF11STF2017 / am.SF11STC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF11SPercent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF11SPercent = 0; }
                amMain.SF11CTC2017 = am.SF11CTC2017;
                amMain.SF11CTF2017 = am.SF11CTF2017;
                amMain.SF11CTC2018 = am.SF11CTC2018;
                amMain.SF11CTF2018 = am.SF11CTF2018;

                if (am.SF11CTC2018 < am.SF11CTF2018 && am.SF11CTC2017 < am.SF11CTF2017)
                {
                    int s1 = (am.SF11CTF2018 / am.SF11CTC2018);
                    int s2 = (am.SF11CTF2017 / am.SF11CTC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF11CPercent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF11CPercent = 0; }


                amMain.SF11VTC2017 = am.SF11VTC2017;
                amMain.SF11VTF2017 = am.SF11VTF2017;
                amMain.SF11VTC2018 = am.SF11VTC2018;
                amMain.SF11VTF2018 = am.SF11VTF2018;
                if (am.SF11VTC2018 < am.SF11VTF2018 && am.SF11VTC2017 < am.SF11VTF2017)
                {
                    int s1 = (am.SF11VTF2018 / am.SF11VTC2018);
                    int s2 = (am.SF11VTF2017 / am.SF11VTC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF11VPercent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF11VPercent = 0; }

                amMain.SF12HTC2017 = am.SF12HTC2017;
                amMain.SF12HTF2017 = am.SF12HTF2017;
                amMain.SF12HTC2018 = am.SF12HTC2018;
                amMain.SF12HTF2018 = am.SF12HTF2018;

                if (am.SF12HTC2018 < am.SF12HTF2018 && am.SF12HTC2017 < am.SF12HTF2017)
                {
                    int s1 = (am.SF12HTF2018 / am.SF12HTC2018);
                    int s2 = (am.SF12HTF2017 / am.SF12HTC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF12HPercent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF12HPercent = 0; }

                amMain.SF12STC2017 = am.SF12STC2017;
                amMain.SF12STF2017 = am.SF12STF2017;
                amMain.SF12STC2018 = am.SF12STC2018;
                amMain.SF12STF2018 = am.SF12STF2018;

                if (am.SF12STC2018 < am.SF12STF2018 && am.SF12STC2017 < am.SF12STF2017)
                {
                    int s1 = (am.SF12STF2018 / am.SF12STC2018);
                    int s2 = (am.SF12STF2017 / am.SF12STC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF12SPercent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF12SPercent = 0; }


                amMain.SF12CTC2017 = am.SF12CTC2017;
                amMain.SF12CTF2017 = am.SF12CTF2017;
                amMain.SF12CTC2018 = am.SF12CTC2018;
                amMain.SF12CTF2018 = am.SF12CTF2018;
                if (am.SF12CTC2018 < am.SF12CTF2018 && am.SF12CTC2017 < am.SF12CTF2017)
                {
                    int s1 = (am.SF12CTF2018 / am.SF12CTC2018);
                    int s2 = (am.SF12CTF2017 / am.SF12CTC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF12CPercent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF12CPercent = 0; }


                amMain.SF12VTC2017 = am.SF12VTC2017;
                amMain.SF12VTF2017 = am.SF12VTF2017;
                amMain.SF12VTC2018 = am.SF12VTC2018;
                amMain.SF12VTF2018 = am.SF12VTF2018;
                if (am.SF12VTC2018 < am.SF12VTF2018 && am.SF12VTC2017 < am.SF12VTF2017)
                {
                    int s1 = (am.SF12VTF2018 / am.SF12VTC2018);
                    int s2 = (am.SF12VTF2017 / am.SF12VTC2017);
                    float diff = s1 - s2;
                    float diff3 = (float.Parse(diff.ToString()) / float.Parse(s2.ToString())) * 100;
                    amMain.SF12VPercent = (int)Math.Ceiling(diff3);
                }
                else { amMain.SF12VPercent = 0; }

                am = amMain;
            }


            string outError = "0";
            int result = affiliationDB.AffiliationContinuation(am, out outError);// if ID=0 then Insert else Update
            if (result > 0)
            {
                ViewBag.AID = am.ID;
                ViewBag.result = result;
                ViewData["result"] = 1;
            }
            else
            {
                ViewBag.AID = am.ID;
                ViewData["result"] = 0;
                ViewBag.Message = outError.ToString();
            }
            return View(am);
        }
        #endregion

        #region BalanceSheet
        public ActionResult BalanceSheet(string id, AffiliationModel am)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            if (id == null)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }


            ViewBag.SessionList = objCommon.GetSessionAdmin().Where(s => Convert.ToInt32(s.Value.Substring(0, 4)) <= 2018).ToList().Take(2); //GetSessionAdmin

            DataSet outDs = new DataSet();
            am = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);
            if (am.ID > 0)
            {
                if (am.BSDFILE == "" || am.BSDFILE == null)
                { @ViewBag.BSDFILE = "../Images/NoPhoto.jpg"; }
                else
                {
                    @ViewBag.BSDFILE = "../../Upload/Affiliation/BalanceSheet" + am.BSDFILE.ToString();
                }
            }
            return View(am);
        }

        [HttpPost]
        public ActionResult BalanceSheet(string id, AffiliationModel am, FormCollection fc, HttpPostedFileBase bsdfile)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            if (id == null)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }
            ViewBag.SessionList = objCommon.GetSessionAdmin().Where(s => Convert.ToInt32(s.Value.Substring(0, 4)) <= 2018).ToList().Take(2); //GetSessionAdmin
            ViewBag.SCHL = Session["SCHL"].ToString();
            am.SCHL = id;
            string filename = "";
            if (bsdfile != null && bsdfile.ContentLength > 0)
            {
                string ext = Path.GetExtension(bsdfile.FileName);
                filename = am.SCHL + "_BSDFILE" + ext;
                var path = Path.Combine(Server.MapPath("~/Upload/Affiliation/BalanceSheet"), filename);
                string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Affiliation/BalanceSheet"));
                if (!Directory.Exists(FilepathExist))
                {
                    Directory.CreateDirectory(FilepathExist);
                }
                bsdfile.SaveAs(path);
                am.BSDFILE = "Affiliation/BalanceSheet/" + filename;
            }

            DataSet outDs = new DataSet();
            AffiliationModel amMain = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
            if (amMain.ID > 0)
            {
                amMain.BSDADD = am.BSDADD;
                amMain.BSDFILE = am.BSDFILE;
                amMain.BSDIDNO = am.BSDIDNO;
                amMain.BSDNAME = am.BSDNAME;
                amMain.BSDSES = am.BSDSES;
                amMain.BSDTEXP = am.BSDTEXP;                //
                amMain.BSDTINC = am.BSDTINC;
                am = amMain;
            }


            string outError = "0";
            int result = affiliationDB.AffiliationContinuation(am, out outError);// if ID=0 then Insert else Update
            if (result > 0)
            {
                ViewBag.AID = am.ID;
                ViewBag.result = result;
                ViewData["result"] = 1;
            }
            else
            {
                ViewBag.AID = am.ID;
                ViewData["result"] = 0;
                ViewBag.Message = outError.ToString();
            }
            return View(am);
        }
        #endregion

        #region SafetyDetails
        public ActionResult SafetyDetails(string id, AffiliationModel am)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }


            // YesNo 
            //   ViewBag.YearList = objCommon.GetSessionYear();
            List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession = objCommon.GetSessionYear();
            itemSession.Add(new SelectListItem { Text = "2019", Value = "2019" });
            itemSession.Add(new SelectListItem { Text = "2020", Value = "2020" });
            ViewBag.YearList = itemSession.Where(s => Convert.ToInt32(s.Value) >= 2016).OrderByDescending(s => s.Value);


            ViewBag.SelectedYear = "0";
            var itemsch = new SelectList(new[]{new {ID="1",Name="Punjab Public Works Department, B&R"},new {ID="2",Name="Department of Rural development and Panchayat"},
            }, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "0";

            DataSet outDs = new DataSet();
            am = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
            if (am.ID > 0)
            {
                if (am.BSFILE == "" || am.BSFILE == null)
                { @ViewBag.BSFILE = "../Images/NoPhoto.jpg"; }
                else
                {
                    @ViewBag.BSFILE = "../../Upload/Affiliation/SafetyDetails" + am.BSFILE.ToString();
                }

                if (am.FSFILE == "" || am.FSFILE == null)
                { @ViewBag.FSFILE = "../Images/NoPhoto.jpg"; }
                else
                {
                    @ViewBag.FSFILE = "../../Upload/Affiliation/SafetyDetails" + am.FSFILE.ToString();
                }

                if (string.IsNullOrEmpty(am.FSIDATE) || am.FSIDATE == "01/01/1900")
                {
                    am.FSIDATE = DateTime.Now.ToString("dd/MM/yyyy");
                }

                if (string.IsNullOrEmpty(am.BSIDATE) || am.BSIDATE == "01/01/1900")
                {
                    am.BSIDATE = DateTime.Now.ToString("dd/MM/yyyy");
                }

                return View(am);
            }
            return View(am);
        }

        [HttpPost]
        public ActionResult SafetyDetails(string id, AffiliationModel am, FormCollection fc, HttpPostedFileBase bsfile, HttpPostedFileBase fsfile)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            // YesNo 
            List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession = objCommon.GetSessionYear();
            itemSession.Add(new SelectListItem { Text = "2019", Value = "2019" });
            itemSession.Add(new SelectListItem { Text = "2020", Value = "2020" });
            ViewBag.YearList = itemSession.Where(s => Convert.ToInt32(s.Value) >= 2016).OrderByDescending(s => s.Value);

            ViewBag.SelectedYear = "0";
            var itemsch = new SelectList(new[]{new {ID="1",Name="Punjab Public Works Department, B&R"},new {ID="2",Name="Department of Rural development and Panchayat"},
            }, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "0";

            am.SCHL = id;
            string filename = "";
            if (bsfile != null)
            {
                string ext = Path.GetExtension(bsfile.FileName);
                filename = am.SCHL + "_BSFILE" + ext;
                var path = Path.Combine(Server.MapPath("~/Upload/Affiliation/SafetyDetails"), filename);
                string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Affiliation/SafetyDetails"));
                if (!Directory.Exists(FilepathExist))
                {
                    Directory.CreateDirectory(FilepathExist);
                }
                bsfile.SaveAs(path);
                am.BSFILE = "Affiliation/SafetyDetails/" + filename;
            }

            string filename1 = "";
            if (fsfile != null)
            {
                string ext = Path.GetExtension(bsfile.FileName);
                filename1 = am.SCHL + "_FSFILE" + ext;
                var path = Path.Combine(Server.MapPath("~/Upload/Affiliation/SafetyDetails"), filename1);
                string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Affiliation/SafetyDetails"));
                if (!Directory.Exists(FilepathExist))
                {
                    Directory.CreateDirectory(FilepathExist);
                }
                fsfile.SaveAs(path);
                am.FSFILE = "Affiliation/SafetyDetails/" + filename1;
            }

            DataSet outDs = new DataSet();
            AffiliationModel amMain = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
            if (amMain.ID > 0)
            {
                amMain.BSFROM = am.BSFROM;
                amMain.BSTO = am.BSTO;
                amMain.BSIA = am.BSIA;
                amMain.BSIDATE = am.BSIDATE;
                amMain.BSMEMO = am.BSMEMO;
                amMain.BSFILE = am.BSFILE;
                //
                amMain.FSFROM = am.FSFROM;
                amMain.FSTO = am.FSTO;
                amMain.FSIA = am.FSIA;
                amMain.FSIDATE = am.FSIDATE;
                amMain.FSMEMO = am.FSMEMO;
                amMain.FSFILE = am.FSFILE;
                am = amMain;
            }
            //

            string outError = "0";
            int result = affiliationDB.AffiliationContinuation(am, out outError);// if ID=0 then Insert else Update
            if (result > 0)
            {
                ViewBag.AID = am.ID;
                ViewBag.result = result;
                ViewData["result"] = 1;
            }
            else
            {
                ViewBag.AID = am.ID;
                ViewData["result"] = 0;
                ViewBag.Message = outError.ToString();
            }
            return View(am);
        }


        #endregion

        #region BooksPurchaseDetails
        public ActionResult BooksPurchaseDetails(string id, AffiliationModel am)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }

            ViewBag.DistOfficerList = objCommon.GetDistE();
            DataSet outDs = new DataSet();
            am = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
            if (am.ID > 0)
            {
                if (string.IsNullOrEmpty(am.BPBILLDATE) || am.BPBILLDATE == "01/01/1900")
                {
                    am.BPBILLDATE = DateTime.Now.ToString("dd/MM/yyyy");
                }
                return View(am);
            }

            if (string.IsNullOrEmpty(am.BPBILLDATE) || am.BPBILLDATE == "01/01/1900")
            {
                am.BPBILLDATE = DateTime.Now.ToString("dd/MM/yyyy");
            }
            return View(am);
        }

        [HttpPost]
        public ActionResult BooksPurchaseDetails(string id, AffiliationModel am, FormCollection fc)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();

            ViewBag.DistOfficerList = objCommon.GetDistE();
            am.SCHL = id;
            DataSet outDs = new DataSet();
            AffiliationModel amMain = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
            ViewBag.SID = 0;
            if (amMain.ID > 0)
            {
                if (am.BPTS > 0)
                { ViewBag.SID = 1; }
                amMain.BPAMOUNT = am.BPAMOUNT;
                amMain.BPBILL = am.BPBILL;
                amMain.BPBILLDATE = am.BPBILLDATE;
                amMain.BPBOOKPERCENT = am.BPBOOKPERCENT;
                amMain.BPFILE = am.BPFILE;
                amMain.BPNAME = am.BPNAME;
                amMain.BPTS = am.BPTS;
                am = amMain;
            }





            string outError = "0";
            int result = affiliationDB.AffiliationContinuation(am, out outError);// if ID=0 then Insert else Update
            if (result > 0)
            {
                ViewBag.AID = am.ID;
                ViewBag.result = result;
                ViewData["result"] = 1;
            }
            else
            {
                ViewBag.AID = am.ID;
                ViewData["result"] = 0;
                ViewBag.Message = outError.ToString();
            }
            return View(am);
        }

        #endregion

        #region Activities
        public ActionResult Activities(string id, AffiliationModel am)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }
            ViewBag.YesNoList = objCommon.GetYesNoText();
            DataSet outDs = new DataSet();
            DataSet ds = new DataSet();
            SchoolPremisesInformation sm = new AbstractLayer.SchoolDB().SchoolPremisesInformationBySchl(ViewBag.SCHL, out ds);
            if (sm.ID == 0)
            {
                am = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
                if (am.ID > 0)
                {
                    return View(am);
                }
            }
            else
            {
                am = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
                if (sm.ID > 0)
                {
                    am.ASDIST = sm.CSS45.ToString();
                    am.ASZONE = sm.CSS46.ToString();
                    am.ASNATIONAL = sm.CSS47.ToString();
                    am.ASINTER = sm.CSS48.ToString();
                    //Other
                    am.OI1 = sm.OTH74.ToString();
                    am.OI2 = sm.OTH75.ToString();
                    am.OI3 = sm.OTH76.ToString();
                    am.OI4 = sm.OTH80.ToString();
                    am.OI5 = sm.OTH81.ToString();
                    am.OI6 = sm.OTH82.ToString();
                    am.OI7 = sm.OTH83.ToString();
                }
                return View(am);

            }



            return View(am);
        }

        [HttpPost]
        public ActionResult Activities(string id, AffiliationModel am, FormCollection fc)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            am.SCHL = id;
            ViewBag.YesNoList = objCommon.GetYesNoText();
            // Update Main Column Only
            DataSet outDs = new DataSet();
            AffiliationModel amMain = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
            if (amMain.ID > 0)
            {
                amMain.ASDIST = am.ASDIST;
                amMain.ASINTER = am.ASINTER;
                amMain.ASNATIONAL = am.ASNATIONAL;
                amMain.ASZONE = am.ASZONE;
                amMain.ASSTATE = am.ASSTATE;
                amMain.AOTH = am.AOTH;
                // other
                amMain.OI1 = am.OI1;
                amMain.OI2 = am.OI2;
                amMain.OI3 = am.OI3;
                amMain.OI4 = am.OI4;
                amMain.OI5 = am.OI5;
                amMain.OI6 = am.OI6;
                amMain.OI7 = am.OI7;

                am = amMain;
            }
            //


            string outError = "0";
            int result = affiliationDB.AffiliationContinuation(am, out outError);// if ID=0 then Insert else Update
            if (result > 0)
            {
                ViewBag.AID = am.ID;
                ViewBag.result = result;
                ViewData["result"] = 1;
            }
            else
            {
                ViewBag.AID = am.ID;
                ViewData["result"] = 0;
                ViewBag.Message = outError.ToString();
            }
            return View(am);
        }


        #endregion

        #region OtherInformation
        public ActionResult OtherInformation(string id, AffiliationModel am)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }

            ViewBag.YesNoList = objCommon.GetYesNoText();
            DataSet outDs = new DataSet();
            DataSet ds = new DataSet();
            SchoolPremisesInformation sm = new AbstractLayer.SchoolDB().SchoolPremisesInformationBySchl(ViewBag.SCHL, out ds);
            if (sm.ID == 0)
            {
                am = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
                if (am.ID > 0)
                {
                    return View(am);
                }
            }
            else
            {
                am = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
                if (sm.ID > 0)
                {
                    am.OI1 = sm.OTH74.ToString();
                    am.OI2 = sm.OTH75.ToString();
                    am.OI3 = sm.OTH76.ToString();
                    am.OI4 = sm.OTH80.ToString();
                    am.OI5 = sm.OTH81.ToString();
                    am.OI6 = sm.OTH82.ToString();
                    am.OI7 = sm.OTH83.ToString();
                }
                return View(am);

            }
            return View(am);
        }

        [HttpPost]
        public ActionResult OtherInformation(string id, AffiliationModel am, FormCollection fc)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }
            ViewBag.YesNoList = objCommon.GetYesNoText();
            am.SCHL = id;

            // Update Main Column Only
            DataSet outDs = new DataSet();
            AffiliationModel amMain = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
            if (amMain.ID > 0)
            {
                amMain.OI1 = am.OI1;
                amMain.OI2 = am.OI2;
                amMain.OI3 = am.OI3;
                amMain.OI4 = am.OI4;
                amMain.OI5 = am.OI5;
                amMain.OI6 = am.OI6;
                amMain.OI7 = am.OI7;

                am = amMain;
            }
            //

            string outError = "0";
            int result = affiliationDB.AffiliationContinuation(am, out outError);// if ID=0 then Insert else Update
            if (result > 0)
            {
                ViewBag.AID = am.ID;
                ViewBag.result = result;
                ViewData["result"] = 1;
            }
            else
            {
                ViewBag.AID = am.ID;
                ViewData["result"] = 0;
                ViewBag.Message = outError.ToString();
            }
            return View(am);
        }

        #endregion

        #region CalculateFee
        public ActionResult CalculateFee(string id, AffiliationFee _affiliationFee)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }

            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }

            ViewBag.IsSchlSubmiited = 0;
            ViewBag.TotalStudent201819 = 0;
            ViewBag.RequiredData = 1;
            DataSet outDsAm = new DataSet();
            AffiliationModel am1 = affiliationDB.AffiliationContinuationBySchl(Session["SCHL"].ToString(), 2, out outDsAm);//ResultStatics
            if (am1.ID > 0)
            {
                ViewBag.AID = am1.ID;
                ViewBag.IsSchlSubmiited = 1;
                ViewBag.ChallanId = am1.ChallanId;
                ViewBag.challanVerify = am1.challanVerify;
                ViewBag.RS10GPass2017 = am1.RS10GPass2017;
                ViewBag.SF10Percent = am1.SF10Percent;
                ViewBag.BSFROM = am1.BSFROM;
                ViewBag.FSFROM = am1.FSFROM;
                ViewBag.BPNAME = am1.BPNAME;
                ViewBag.ASZONE = am1.ASZONE;
                ViewBag.ASZONE = am1.ASZONE;
                ViewBag.OI1 = am1.OI1;

                int TotalStaff = outDsAm.Tables[3].Rows.Count;
                string principal = outDsAm.Tables[2].Rows[0]["PRINCIPAL"].ToString();
                string mobile = outDsAm.Tables[2].Rows[0]["MOBILE"].ToString();

                int TotalStudent201819 = (am1.SF1TC2018 + am1.SF2TC2018 + am1.SF3TC2018 + am1.SF4TC2018 + am1.SF5TC2018 + am1.SF6TC2018 + am1.SF7TC2018 + am1.SF8TC2018 + am1.SF9TC2018 + am1.SF10TC2018 + am1.SF11HTC2018
                                        + am1.SF11STC2018 + am1.SF11CTC2018 + am1.SF11VTC2018 + am1.SF12HTC2018 + am1.SF12STC2018 + am1.SF12CTC2018 + am1.SF12VTC2018);

                ViewBag.TotalStudent201819 = 0;
                ViewBag.TS1 = TotalStudent201819;
                ViewBag.TS2 = am1.BPTS;
                //if (TotalStudent201819 == am1.BPTS && TotalStudent201819 > 0)
                //{

                //    ViewBag.TotalStudent201819 = 1;
                //}
                // Condition Removed by Mail : Remove Validation of Mandatory on Book Purchase Details Page from All fields On 12Sep2018
                ViewBag.TotalStudent201819 = 1;


                string result = affiliationDB.IsValidForChallan(Session["SCHL"].ToString());
                if (result != string.Empty)
                {
                    TempData["notValidForChallan"] = result;
                    ViewBag.RequiredData = 0;
                }
                else { ViewBag.RequiredData = 1; }

            }
            else
            {
                ViewBag.AID = 0;
                ViewBag.ChallanId = 0;
                ViewBag.challanVerify = 0;
            }




            var itemsch = new SelectList(new[]{new {ID="1",Name="Lumsum fee of 12 year or life time already paid"},
                new {ID="2",Name="Pay Fee of Current Year"},new {ID="3",Name="Pay Lumsum Fee for 12 Year"},
            new {ID="4",Name="Current Session Fee Paid Offline"},}, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "0";
            //
            string search = "a.challanid like '%%' and feecode=45 and StudentList='" + ViewBag.SCHL + "'";
            _affiliationFee.StoreAllData = new AbstractLayer.HomeDB().GetChallanDetailsBySearch(search);
            if (_affiliationFee.StoreAllData == null || _affiliationFee.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.TotalCount = 0;
                ViewData["ChlnStatus"] = null;
            }
            else
            {
                ViewBag.TotalCount = 1;
                ViewData["ChlnStatus"] = 1;
            }


            // check magazine
            MagazineSchoolRequirements magazineSchoolRequirements = new MagazineSchoolRequirements();
            magazineSchoolRequirements.Schl = Session["Schl"].ToString();
            magazineSchoolRequirements.MagazineSchoolRequirementsList = _context.MagazineSchoolRequirementsChallanViews.AsNoTracking().Where(s => s.Schl == magazineSchoolRequirements.Schl && s.ChallanVerify == 1).ToList();
            if (magazineSchoolRequirements.MagazineSchoolRequirementsList.Count > 0)
            {
                ViewData["MagazineStatus"] = 1;
            }
            else
            {
                ViewData["MagazineStatus"] = 0;
            }
            return View(_affiliationFee);
        }

        [HttpPost]
        public ActionResult CalculateFee(string id, AffiliationFee _affiliationFee, FormCollection frm, HttpPostedFileBase ReceiptScannedCopy, string submit, string PayModValue, string OldDepositDate, string OldAmount)
        {
            try
            {
                AffiliationModel am = new AffiliationModel();
                ChallanMasterModel CM = new ChallanMasterModel();

                DataSet outDsAm = new DataSet();
                AffiliationModel am1 = affiliationDB.AffiliationContinuationBySchl(Session["SCHL"].ToString(), 1, out outDsAm);//ResultStatics
                if (am1.ID > 0)
                {
                    ViewBag.AID = am1.ID;
                    ViewBag.ChallanId = am1.ChallanId;
                    ViewBag.challanVerify = am1.challanVerify;
                    ViewBag.RS10GPass2017 = am1.RS10GPass2017;
                    ViewBag.SF10Percent = am1.SF10Percent;
                    ViewBag.BSFROM = am1.BSFROM;
                    ViewBag.FSFROM = am1.FSFROM;
                    ViewBag.BPNAME = am1.BPNAME;
                    ViewBag.ASZONE = am1.ASZONE;
                    ViewBag.ASZONE = am1.ASZONE;
                    ViewBag.OI1 = am1.OI1;
                }
                else
                {
                    ViewBag.AID = 0;
                    ViewBag.ChallanId = 0;
                    ViewBag.challanVerify = 0;
                }

                // check magazine
                MagazineSchoolRequirements magazineSchoolRequirements = new MagazineSchoolRequirements();
                magazineSchoolRequirements.Schl = Session["Schl"].ToString();
                magazineSchoolRequirements.MagazineSchoolRequirementsList = _context.MagazineSchoolRequirementsChallanViews.AsNoTracking().Where(s => s.Schl == magazineSchoolRequirements.Schl && s.ChallanVerify == 1).ToList();
                if (magazineSchoolRequirements.MagazineSchoolRequirementsList.Count > 0)
                {
                    ViewData["MagazineStatus"] = 1;
                }
                else
                {
                    ViewData["MagazineStatus"] = 0;
                }


                // var itemsch = new SelectList(new[]{new {ID="1",Name="Lumsum fee of 12 year or life time already paid"},
                // new {ID="2",Name="Pay Fee of Current Year"},new {ID="3",Name="Pay Lumsum Fee for 12 Year"},
                //new {ID="4",Name="Current Session Fee Paid Offline"},}, "ID", "Name", 1);
                var itemsch = new SelectList(new[]{new {ID="1",Name="Lumsum fee of 12 year or life time already paid"},
                new {ID="2",Name="Pay Fee of Current Year"},new {ID="3",Name="Pay Lumsum Fee for 12 Year"},
               new {ID="4",Name="Current Session Fee Paid Offline"},}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Home"); }
                //
                if (id == null)
                {
                    return RedirectToAction("Index", "AffiliationContinuation");
                }
                ViewBag.SCHL = Session["SCHL"].ToString();
                if (submit != null)
                {
                    if (_affiliationFee.ChallanCategory == 4)
                    {
                        ViewData["ChallanCategory"] = _affiliationFee.ChallanCategory;
                        if (submit.ToLower() == "go" && !string.IsNullOrEmpty(OldDepositDate) && !string.IsNullOrEmpty(OldAmount))
                        {
                            ViewBag.OldAmount = OldAmount;
                            //string today = DateTime.Today.ToString("dd/MM/yyyy");
                            string today = OldDepositDate;
                            DateTime dateselected;
                            if (DateTime.TryParseExact(today, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
                            {
                                _affiliationFee = affiliationDB.AffiliationFee(_affiliationFee.ChallanCategory, id, dateselected);
                                ViewBag.Total = _affiliationFee.totfee;
                            }
                            if (_affiliationFee == null)
                            {
                                ViewBag.TotalCount = 0;
                                ViewData["FeeStatus"] = "0";
                                ViewBag.Balance = null;
                            }
                            else
                            {
                                ViewData["FeeStatus"] = "1";
                                ViewBag.TotalCount = 1;
                                if (_affiliationFee.totfee > 0)
                                {
                                    int Balance = _affiliationFee.totfee - Convert.ToInt32(OldAmount);
                                    ViewBag.Balance = Balance;
                                }
                            }
                            return View(_affiliationFee);

                        }
                        else if ((submit.ToLower().Contains("final") || submit.ToLower().Contains("online")) && ReceiptScannedCopy != null && !string.IsNullOrEmpty(OldDepositDate) && !string.IsNullOrEmpty(OldAmount))
                        {
                            ViewBag.OldAmount = OldAmount;
                            //string today = DateTime.Today.ToString("dd/MM/yyyy");
                            string today = OldDepositDate;
                            DateTime dateselected;
                            AffiliationFee _affiliationFee1 = new AffiliationFee();
                            if (DateTime.TryParseExact(today, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
                            {
                                // 2 ( for check late fee only for Lumsum fee of 12 years already paid
                                _affiliationFee1 = affiliationDB.AffiliationFee(_affiliationFee.ChallanCategory, id, dateselected);
                                ViewBag.totfee = _affiliationFee1.totfee;
                                if (_affiliationFee1.totfee == 0)
                                {
                                    _affiliationFee1.BankCode = _affiliationFee.BankCode = "203";
                                    _affiliationFee.SCHL = _affiliationFee1.SCHL;
                                    _affiliationFee.fee = _affiliationFee1.fee;
                                    _affiliationFee.latefee = _affiliationFee1.latefee;
                                    _affiliationFee.totfee = _affiliationFee1.totfee;
                                    _affiliationFee.FEECAT = _affiliationFee1.FEECAT;
                                    _affiliationFee.FEECODE = _affiliationFee1.FEECODE;

                                }
                                else
                                {

                                    int Balance = _affiliationFee1.totfee - Convert.ToInt32(OldAmount);
                                    ViewBag.Balance = Balance;
                                    _affiliationFee1.BankCode = _affiliationFee.BankCode = "203";
                                    //
                                    _affiliationFee.SCHL = _affiliationFee1.SCHL;
                                    _affiliationFee.fee = _affiliationFee1.fee;
                                    _affiliationFee.latefee = _affiliationFee1.latefee;
                                    _affiliationFee.totfee = _affiliationFee1.totfee;
                                    _affiliationFee.FEECAT = _affiliationFee1.FEECAT;
                                    _affiliationFee.FEECODE = _affiliationFee1.FEECODE;
                                    //_affiliationFee.sDate = _affiliationFee1.sDate;
                                    //_affiliationFee.eDate = _affiliationFee1.eDate;
                                    _affiliationFee.sDate = DateTime.Now.ToString("dd/MM/yyyy");
                                    _affiliationFee.eDate = DateTime.Now.ToString("dd/MM/yyyy");
                                    _affiliationFee.BankLastdate = _affiliationFee1.BankLastdate;
                                    _affiliationFee.AllowBanks = _affiliationFee1.AllowBanks;
                                    _affiliationFee.TotalFeesInWords = new AbstractLayer.DBClass().GetAmountInWords(_affiliationFee.totfee);

                                }
                            }


                            if ((submit.ToLower().Contains("final") || submit.ToLower().Contains("online")) && _affiliationFee.ChallanCategory == 4)
                            {

                                //
                                if (_affiliationFee.BankCode == null)
                                {
                                    ViewBag.Message = "Please Select Bank";
                                    ViewData["SelectBank"] = "1";
                                    return View(_affiliationFee);
                                }
                                else
                                {
                                    string BankCode = _affiliationFee.BankCode;
                                    string AllowBanks = _affiliationFee.BankCode;

                                    string bankName = "";
                                    if (AllowBanks == "203")
                                    {
                                        PayModValue = "hod";
                                        bankName = "PSEB HOD";
                                        CM.FEEMODE = "CASH";
                                        CM.DepositoryMobile = "CASH";
                                    }




                                    DataSet outDs = new DataSet();
                                    am = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
                                    if (am.ID > 0)
                                    {
                                        CM.APPNO = am.SCHL;
                                        CM.FeeStudentList = am.SCHL;
                                        CM.SCHLREGID = am.SCHL.ToString();
                                        CM.SchoolCode = _affiliationFee.ChallanCategory.ToString();
                                        // CM.addfee = 0; // AdmissionFee / ADDFEE
                                        CM.latefee = _affiliationFee1.latefee;
                                        // CM.prosfee = 0;
                                        CM.addsubfee = 0;
                                        CM.add_sub_count = 0;
                                        CM.regfee = _affiliationFee1.fee;
                                        CM.FEE = _affiliationFee1.fee;
                                        CM.TOTFEE = _affiliationFee1.totfee;
                                        CM.FEECAT = _affiliationFee1.FEECAT;
                                        CM.FEECODE = _affiliationFee1.FEECODE.ToString();
                                        CM.BANK = bankName;
                                        CM.BCODE = BankCode;
                                        CM.BANKCHRG = Convert.ToInt32(0);
                                        CM.DIST = am.SCHLDIST;
                                        CM.DISTNM = am.SCHLDISTNM;
                                        CM.LOT = 0;

                                        CM.prosfee = Convert.ToInt32(ViewBag.totfee);  // Total Amount till Deposit Date
                                        CM.addfee = Convert.ToInt32(OldAmount); // Old Amount


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
                                        string result = "0";
                                        result = new AbstractLayer.HomeDB().InsertPaymentForm(CM, frm, out SchoolMobile);
                                        if (result == "0" || result == "")
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
                                            string selCateory = CM.SchoolCode;
                                            string filename = "";
                                            if (ReceiptScannedCopy != null)
                                            {
                                                string ext = Path.GetExtension(ReceiptScannedCopy.FileName);
                                                filename = am.SCHL + "_" + selCateory + "_ReceiptScannedCopy" + ext;

                                                using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                                                {
                                                    using (var newMemoryStream = new MemoryStream())
                                                    {
                                                        ///file.CopyTo(newMemoryStream);

                                                        var uploadRequest = new TransferUtilityUploadRequest
                                                        {
                                                            InputStream = ReceiptScannedCopy.InputStream,
                                                            Key = string.Format("allfiles/Upload2023/Affiliation/ReceiptScannedCopy/{0}", filename),
                                                            BucketName = BUCKET_NAME,
                                                            CannedACL = S3CannedACL.PublicRead
                                                        };

                                                        var fileTransferUtility = new TransferUtility(client);
                                                        fileTransferUtility.Upload(uploadRequest);
                                                    }
                                                }

                                                //var path = Path.Combine(Server.MapPath("Upload2023/Affiliation/ReceiptScannedCopy"), filename);
                                                //string FilepathExist = Path.Combine(Server.MapPath("~/Upload2023/Affiliation/ReceiptScannedCopy"));
                                                //if (!Directory.Exists(FilepathExist))
                                                //{
                                                //    Directory.CreateDirectory(FilepathExist);
                                                //}
                                                //ReceiptScannedCopy.SaveAs(path);
                                                _affiliationFee.ReceiptScannedCopy = "Affiliation/ReceiptScannedCopy/" + filename;
                                            }

                                            ViewData["FeeStatus"] = null;
                                            ViewData["SelectBank"] = null;
                                            ViewData["result"] = 1;
                                            ViewBag.ChallanNo = result;
                                            am.ReceiptScannedCopy = _affiliationFee.ReceiptScannedCopy;
                                            am.OldAmount = _affiliationFee.OldAmount;
                                            am.oldChallanId = _affiliationFee.oldChallanId == null ? "" : _affiliationFee.oldChallanId;
                                            am.OldDepositDate = _affiliationFee.OldDepositDate;
                                            am.OldRecieptNo = _affiliationFee.OldRecieptNo;
                                            am.ChallanCategory = 4;

                                            string outError = "0";
                                            int result1 = affiliationDB.AffiliationContinuation(am, out outError);// if ID=0 then Insert else Update
                                            if (result1 > 0)
                                            {
                                                string TotfeePG = CM.TOTFEE.ToString();
                                                string paymenttype = CM.BCODE;
                                                if (result.Length > 5)
                                                {
                                                    string Sms = "Your Challan no. " + ViewBag.ChallanNo + " of Affiliation Continuation Fees is successfully generated and valid till Dt " + CM.CHLNVDATE + ". Regards PSEB";
                                                    try
                                                    {
                                                        string getSms = new AbstractLayer.DBClass().gosms(SchoolMobile, Sms);
                                                    }
                                                    catch (Exception) { }
                                                    return RedirectToAction("GenerateChallaan", "AffiliationContinuation", new { Id = ViewBag.ChallanNo });

                                                }
                                            }
                                        }
                                    }
                                }

                                //

                            }
                        }
                    }
                    else if (_affiliationFee.ChallanCategory > 1 && _affiliationFee.ChallanCategory <= 3)

                    {
                        ViewData["ChallanCategory"] = _affiliationFee.ChallanCategory;
                        if (submit.ToLower() == "submit")
                        {
                            string today = DateTime.Today.ToString("dd/MM/yyyy");
                            DateTime dateselected;
                            if (DateTime.TryParseExact(today, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
                            {
                                _affiliationFee = affiliationDB.AffiliationFee(_affiliationFee.ChallanCategory, id, dateselected);
                                ViewBag.Total = _affiliationFee.totfee;
                            }
                            if (_affiliationFee == null)
                            {
                                ViewBag.TotalCount = 0;
                                ViewData["FeeStatus"] = "0";
                            }
                            else
                            {
                                ViewData["FeeStatus"] = "1";
                                ViewBag.TotalCount = 1;
                            }
                            return View(_affiliationFee);

                        }
                        //else if (submit.ToLower().Contains("final"))
                        else if (submit.ToLower().Contains("online"))
                        {
                            //
                            if (_affiliationFee.BankCode == null)
                            {
                                ViewBag.Message = "Please Select Bank";
                                ViewData["SelectBank"] = "1";
                                return View(_affiliationFee);
                            }
                            else
                            {

                                string BankCode = _affiliationFee.BankCode;
                                string AllowBanks = _affiliationFee.BankCode;

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


                                DataSet outDs = new DataSet();
                                am = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
                                if (am.ID > 0)
                                {
                                    string today = DateTime.Today.ToString("dd/MM/yyyy");
                                    DateTime dateselected;
                                    if (DateTime.TryParseExact(today, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
                                    {
                                        _affiliationFee = affiliationDB.AffiliationFee(_affiliationFee.ChallanCategory, id, dateselected);
                                        ViewBag.Total = _affiliationFee.totfee;
                                    }

                                    CM.APPNO = am.SCHL;
                                    CM.FeeStudentList = am.SCHL;
                                    CM.SCHLREGID = am.SCHL.ToString();
                                    CM.SchoolCode = _affiliationFee.ChallanCategory.ToString();
                                    CM.addfee = 0; // AdmissionFee / ADDFEE
                                    CM.latefee = _affiliationFee.latefee;
                                    CM.prosfee = 0;
                                    CM.addsubfee = 0;
                                    CM.add_sub_count = 0;
                                    CM.regfee = _affiliationFee.fee;
                                    CM.FEE = _affiliationFee.fee;
                                    CM.TOTFEE = _affiliationFee.totfee;
                                    CM.FEECAT = _affiliationFee.FEECAT;
                                    CM.FEECODE = _affiliationFee.FEECODE.ToString();
                                    CM.FEEMODE = "ONLINE";
                                    CM.BANK = bankName;
                                    CM.BCODE = BankCode;
                                    CM.BANKCHRG = Convert.ToInt32(0);
                                    CM.DIST = am.SCHLDIST;
                                    CM.DISTNM = am.SCHLDISTNM;
                                    CM.LOT = 0;
                                    CM.DepositoryMobile = "ONLINE";
                                    CM.type = "schle";
                                    DateTime CHLNVDATE2;
                                    if (DateTime.TryParseExact(_affiliationFee.BankLastdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out CHLNVDATE2))
                                    {
                                        CM.ChallanVDateN = CHLNVDATE2;
                                    }
                                    CM.CHLNVDATE = _affiliationFee.BankLastdate;
                                    CM.LumsumFine = Convert.ToInt32(0);
                                    CM.LSFRemarks = "";
                                    string SchoolMobile = "";
                                    string result = "0";
                                    result = new AbstractLayer.HomeDB().InsertPaymentForm(CM, frm, out SchoolMobile);
                                    if (result == "0" || result == "")
                                    {
                                        //--------------Not saved
                                        ViewData["result"] = 0;
                                        return View(_affiliationFee);
                                    }
                                    if (result == "-1")
                                    {
                                        //-----alredy exist
                                        ViewData["result"] = -1;
                                        return View(_affiliationFee);
                                    }
                                    else
                                    {
                                        ViewData["FeeStatus"] = null;
                                        ViewData["SelectBank"] = null;
                                        ViewData["result"] = 1;
                                        ViewBag.ChallanNo = result;
                                        string TotfeePG = CM.TOTFEE.ToString();
                                        string paymenttype = CM.BCODE;
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
                                                string TransactionAmount = TotfeePG;
                                                string TransactionCurrency = "INR";
                                                string TransactionServiceCharge = "0";
                                                string TransactionID = ViewBag.ChallanNo;// Unique Challan Number
                                                string TransactionDateTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
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
                                                    return View(_affiliationFee);
                                                }
                                            }
                                            #endregion Payment Gateyway
                                        }
                                        else if (result.Length > 5)
                                        {

                                            string Sms = "Your Challan no. " + ViewBag.ChallanNo + " of Affiliation Continuation Fees is successfully generated and valid till Dt " + CM.CHLNVDATE + ". Regards PSEB";
                                            try
                                            {
                                                //string getSms = new AbstractLayer.DBClass().gosms(SchoolMobile, Sms);
                                            }
                                            catch (Exception) { }
                                            return RedirectToAction("GenerateChallaan", "AffiliationContinuation", new { Id = ViewBag.ChallanNo });

                                        }

                                    }
                                }
                                else { return View(_affiliationFee); }
                            }
                        }
                    }
                    else
                    {
                        ViewData["ChallanCategory"] = _affiliationFee.ChallanCategory;
                        ViewData["FeeStatus"] = null;
                        //int AmountPaid = Convert.ToInt32(OldAmount);
                        string today = DateTime.Today.ToString("dd/MM/yyyy");
                        DateTime dateselected;
                        AffiliationFee _affiliationFee1 = new AffiliationFee();
                        if (DateTime.TryParseExact(today, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
                        {
                            // 2 ( for check late fee only for Lumsum fee of 12 years already paid
                            _affiliationFee1 = affiliationDB.AffiliationFee(_affiliationFee.ChallanCategory, id, dateselected);
                            ViewBag.totfee = _affiliationFee1.totfee;
                            if (_affiliationFee1.totfee == 0)
                            {
                                _affiliationFee1.BankCode = _affiliationFee.BankCode = "203";

                                _affiliationFee.SCHL = _affiliationFee1.SCHL;
                                _affiliationFee.fee = _affiliationFee1.fee;
                                _affiliationFee.latefee = _affiliationFee1.latefee;
                                _affiliationFee.totfee = _affiliationFee1.totfee;
                                _affiliationFee.FEECAT = _affiliationFee1.FEECAT;
                                _affiliationFee.FEECODE = _affiliationFee1.FEECODE;
                                _affiliationFee.sDate = _affiliationFee1.sDate;
                                _affiliationFee.eDate = _affiliationFee1.eDate;
                                _affiliationFee.BankLastdate = _affiliationFee1.BankLastdate;
                                _affiliationFee.AllowBanks = _affiliationFee1.AllowBanks;
                                _affiliationFee.TotalFeesInWords = _affiliationFee1.TotalFeesInWords;
                            }
                            else
                            {
                                _affiliationFee1.BankCode = _affiliationFee.BankCode = "203";
                                _affiliationFee.SCHL = _affiliationFee1.SCHL;
                                _affiliationFee.fee = _affiliationFee1.fee;
                                _affiliationFee.latefee = _affiliationFee1.latefee;
                                _affiliationFee.totfee = _affiliationFee1.totfee;
                                _affiliationFee.FEECAT = _affiliationFee1.FEECAT;
                                _affiliationFee.FEECODE = _affiliationFee1.FEECODE;
                                _affiliationFee.sDate = _affiliationFee1.sDate;
                                _affiliationFee.eDate = _affiliationFee1.eDate;
                                _affiliationFee.BankLastdate = _affiliationFee1.BankLastdate;
                                _affiliationFee.AllowBanks = _affiliationFee1.AllowBanks;
                                _affiliationFee.TotalFeesInWords = _affiliationFee1.TotalFeesInWords;

                                // return View(_affiliationFee);

                            }
                        }

                        //if (submit.ToLower().Contains("final") && _affiliationFee.ChallanCategory == 1 && _affiliationFee1.BankCode == "203")
                        if ((submit.ToLower().Contains("final") || submit.ToLower().Contains("online")) && _affiliationFee.ChallanCategory == 1)
                        {
                            _affiliationFee1.BankCode = _affiliationFee.BankCode = "203";


                            //
                            if (_affiliationFee.BankCode == null)
                            {
                                ViewBag.Message = "Please Select Bank";
                                ViewData["SelectBank"] = "1";
                                return View(_affiliationFee);
                            }
                            else
                            {
                                string BankCode = _affiliationFee.BankCode;
                                string AllowBanks = _affiliationFee.BankCode;

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
                                    CM.FEEMODE = "ONLINE";
                                    CM.DepositoryMobile = "ONLINE";
                                }
                                else if (AllowBanks == "203")
                                {
                                    PayModValue = "hod";
                                    bankName = "PSEB HOD";
                                    CM.FEEMODE = "CASH";
                                    CM.DepositoryMobile = "CASH";
                                }




                                DataSet outDs = new DataSet();
                                am = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
                                if (am.ID > 0)
                                {
                                    CM.APPNO = am.SCHL;
                                    CM.FeeStudentList = am.SCHL;
                                    CM.SCHLREGID = am.SCHL.ToString();
                                    CM.SchoolCode = _affiliationFee.ChallanCategory.ToString();
                                    CM.addfee = 0; // AdmissionFee / ADDFEE
                                    CM.latefee = _affiliationFee1.latefee;
                                    CM.prosfee = 0;
                                    CM.addsubfee = 0;
                                    CM.add_sub_count = 0;
                                    CM.regfee = _affiliationFee1.fee;
                                    CM.FEE = _affiliationFee1.fee;
                                    CM.TOTFEE = _affiliationFee1.totfee;
                                    CM.FEECAT = _affiliationFee1.FEECAT;
                                    CM.FEECODE = _affiliationFee1.FEECODE.ToString();
                                    CM.BANK = bankName;
                                    CM.BCODE = BankCode;
                                    CM.BANKCHRG = Convert.ToInt32(0);
                                    CM.DIST = am.SCHLDIST;
                                    CM.DISTNM = am.SCHLDISTNM;
                                    CM.LOT = 0;

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
                                    string result = "0";
                                    result = new AbstractLayer.HomeDB().InsertPaymentForm(CM, frm, out SchoolMobile);
                                    if (result == "0" || result == "")
                                    {
                                        //--------------Not saved
                                        ViewData["result"] = 0;
                                        return View(_affiliationFee);
                                    }
                                    else if (result == "-1")
                                    {
                                        //-----alredy exist
                                        ViewData["result"] = -1;
                                        return View(_affiliationFee);
                                    }
                                    else
                                    {

                                        string selCateory = CM.SchoolCode;
                                        string filename = "";
                                        if (ReceiptScannedCopy != null)
                                        {
                                            string ext = Path.GetExtension(ReceiptScannedCopy.FileName);
                                            filename = am.SCHL + "_" + selCateory + "_ReceiptScannedCopy" + ext;

                                            using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                                            {
                                                using (var newMemoryStream = new MemoryStream())
                                                {
                                                    ///file.CopyTo(newMemoryStream);

                                                    var uploadRequest = new TransferUtilityUploadRequest
                                                    {
                                                        InputStream = ReceiptScannedCopy.InputStream,
                                                        Key = string.Format("allfiles/Upload2023/Affiliation/ReceiptScannedCopy/{0}", filename),
                                                        BucketName = BUCKET_NAME,
                                                        CannedACL = S3CannedACL.PublicRead
                                                    };

                                                    var fileTransferUtility = new TransferUtility(client);
                                                    fileTransferUtility.Upload(uploadRequest);
                                                }
                                            }
                                            _affiliationFee.ReceiptScannedCopy = "Affiliation/ReceiptScannedCopy/" + filename;
                                        }

                                        ViewData["FeeStatus"] = null;
                                        ViewData["SelectBank"] = null;
                                        ViewData["result"] = 1;
                                        ViewBag.ChallanNo = result;
                                        am.ReceiptScannedCopy = _affiliationFee.ReceiptScannedCopy;
                                        am.OldAmount = _affiliationFee.OldAmount;
                                        am.oldChallanId = _affiliationFee.oldChallanId == null ? "" : _affiliationFee.oldChallanId;
                                        am.OldDepositDate = _affiliationFee.OldDepositDate;
                                        am.OldRecieptNo = _affiliationFee.OldRecieptNo;
                                        am.ChallanCategory = 1;

                                        string outError = "0";
                                        int result1 = affiliationDB.AffiliationContinuation(am, out outError);// if ID=0 then Insert else Update
                                        if (result1 > 0)
                                        {
                                            string TotfeePG = CM.TOTFEE.ToString();
                                            string paymenttype = CM.BCODE;
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
                                                    string TransactionAmount = TotfeePG;
                                                    string TransactionCurrency = "INR";
                                                    string TransactionServiceCharge = "0";
                                                    string TransactionID = ViewBag.ChallanNo;// Unique Challan Number
                                                    string TransactionDateTime = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss");
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
                                                        return View(_affiliationFee);
                                                    }
                                                }
                                                #endregion Payment Gateyway
                                            }
                                            else if (result.Length > 5)
                                            {
                                                string Sms = "Your Challan no. " + ViewBag.ChallanNo + " of Affiliation Continuation Fees is successfully generated and valid till Dt " + CM.CHLNVDATE + ". Regards PSEB";
                                                try
                                                {
                                                    string getSms = new AbstractLayer.DBClass().gosms(SchoolMobile, Sms);
                                                }
                                                catch (Exception) { }
                                                return RedirectToAction("GenerateChallaan", "AffiliationContinuation", new { Id = ViewBag.ChallanNo });

                                            }
                                        }
                                    }
                                }
                            }

                            //

                        }

                    }
                }
                return View(_affiliationFee);

            }
            catch (Exception)
            {
                return View(_affiliationFee);
            }
        }

        public ActionResult GenerateChallaan(string id)
        {
            try
            {
                string ChallanId = id;
                ChallanMasterModel CM = new ChallanMasterModel();
                if (ChallanId == null || ChallanId == "0" || ChallanId == "")
                {
                    return RedirectToAction("Index", "AffiliationContinuation");
                }
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Home"); }
                //
                if (id == null)
                {
                    return RedirectToAction("Index", "AffiliationContinuation");
                }
                DataSet ds = new AbstractLayer.HomeDB().GetChallanDetailsById(ChallanId);
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
                return RedirectToAction("Index", "AffiliationContinuation");
            }
        }

        #endregion

        #region PrintForm
        public ActionResult PrintForm(string id, AffiliationModel am)
        {
            try
            {
                var itemsch = new SelectList(new[]{new {ID="1",Name="Lumsum fee of 12 year or life time already paid"},
                new {ID="2",Name="Pay Fee of Current Year"},new {ID="3",Name="Pay Lumsum Fee for 12 Year"},
                new {ID="4",Name="Current Session Fee Paid Offline"},}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();


                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Home"); }
                //
                if (id == null)
                {
                    return RedirectToAction("Index", "AffiliationContinuation");
                }

                ViewBag.SCHL = Session["SCHL"].ToString();
                if (id != ViewBag.SCHL)
                {
                    return RedirectToAction("Index", "AffiliationContinuation");
                }
                SchoolModels sm = new SchoolModels();
                DataSet outDs = new DataSet();
                am = affiliationDB.AffiliationContinuationBySchl(id, 2, out outDs);//Type2 ( for staff details)
                if (am.ID > 0)
                {
                    am.StoreAllData = outDs;
                    ViewBag.Totalcount = 1;
                    ViewBag.AID = am.ID;
                    ViewBag.ChallanId = am.ChallanId;
                    ViewBag.IsVerified = am.challanVerify;
                    ViewBag.ChallanDt = am.ChallanDt;
                }
                else
                {
                    ViewBag.AID = 0;
                    ViewBag.IsVerified = 0;
                    ViewBag.Totalcount = 0;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return View(am);
        }
        #endregion

        #region FinalPrintForm

        public ActionResult FinalPrintForm(string id, AffiliationModel am)
        {
            try
            {
                var itemsch = new SelectList(new[]{new {ID="1",Name="Lumsum fee of 12 year or life time already paid"},
                new {ID="2",Name="Pay Fee of Current Year"},new {ID="3",Name="Pay Lumsum Fee for 12 Year"},
                new {ID="4",Name="Current Session Fee Paid Offline"},}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();


                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Home"); }
                //
                if (id == null)
                {
                    return RedirectToAction("Index", "AffiliationContinuation");
                }

                ViewBag.SCHL = Session["SCHL"].ToString();
                if (id != ViewBag.SCHL)
                {
                    return RedirectToAction("Index", "AffiliationContinuation");
                }

                DataSet outDs = new DataSet();
                am = affiliationDB.AffiliationContinuationBySchl(id, 2, out outDs);//ResultStatics
                if (am.ID > 0)
                {
                    am.StoreAllData = outDs;
                    ViewBag.Totalcount = 1;
                    ViewBag.AID = am.ID;
                    ViewBag.ChallanId = am.ChallanId;
                    ViewBag.IsVerified = am.challanVerify;
                    ViewBag.ChallanDt = am.ChallanDt;
                    ViewBag.ChallanCategoryName = am.ChallanCategoryName = itemsch.ToList().Where(s => s.Value == am.ChallanCategory.ToString()).Select(s => s.Text).SingleOrDefault();
                }
                else
                {
                    ViewBag.AID = 0;
                    ViewBag.IsVerified = 0;
                    ViewBag.Totalcount = 0;
                }
            }
            catch (Exception)
            {

                throw;
            }
            return View(am);
        }
        #endregion



        #region Upload AffiliationContinuation Documents
        public ActionResult UploadAffiliationContinuationDocuments(string id, AffiliationDocumentDetailsModel adm)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }

            adm.SCHL = id;
            DataSet outDs = new DataSet();
            AffiliationModel am1 = affiliationDB.AffiliationContinuationBySchl(id, 1, out outDs);//ResultStatics
            if (am1.ID > 0)
            {
                ViewBag.AID = am1.ID;
                ViewBag.IsSchlSubmiited = 1;
                ViewBag.ChallanId = am1.ChallanId;
                ViewBag.challanVerify = am1.challanVerify;
                ViewBag.RS10GPass2017 = am1.RS10GPass2017;
                ViewBag.SF10Percent = am1.SF10Percent;
                ViewBag.BSFROM = am1.BSFROM;
                ViewBag.FSFROM = am1.FSFROM;
                ViewBag.BPNAME = am1.BPNAME;
                ViewBag.ASZONE = am1.ASZONE;
                ViewBag.ASZONE = am1.ASZONE;
                ViewBag.OI1 = am1.OI1;


                adm.StoreAllData = affiliationDB.GetAffiliationDocumentDetails(2, 0, id, "");
                adm.AffiliationDocumentMasterList = affiliationDB.AffiliationDocumentMasterList(adm.StoreAllData.Tables[1]);//  Document List
                if (adm.StoreAllData == null || adm.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = adm.StoreAllData.Tables[0].Rows.Count;
                }
            }
            else
            {
                ViewBag.AID = 0;
                ViewBag.ChallanId = 0;
            }
            return View(adm);
        }

        [HttpPost]
        public ActionResult UploadAffiliationContinuationDocuments(string id, AffiliationDocumentDetailsModel adm, string cmd, FormCollection frm, HttpPostedFileBase docfile)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "AffiliationContinuation");
            }
            adm.StoreAllData = affiliationDB.GetAffiliationDocumentDetails(2, 0, id, "");
            adm.AffiliationDocumentMasterList = affiliationDB.AffiliationDocumentMasterList(adm.StoreAllData.Tables[1]);//  Document List
            adm.SCHL = id;

            string DocName = adm.AffiliationDocumentMasterList.Where(s => s.DocID == adm.DocID).Select(s => s.DocumentName).FirstOrDefault();

            if (!string.IsNullOrEmpty(cmd))
            {
                string outError = "0";
                int result = 0;

                // Save file
                string filename = "";
                string FilepathExist = "", path = "";
                //string exactPath = "~/Upload/Upload2023/AffiliationDocuments";
                if (docfile != null)
                {
                    //Upload/Affiliation
                    //path = Path.Combine(Server.MapPath(exactPath), filename);
                    //FilepathExist = Path.Combine(Server.MapPath(exactPath));
                    string ext = Path.GetExtension(docfile.FileName);
                    filename = adm.SCHL + "_" + DocName.Replace(" ", "_") + ext;                    
                    adm.DocFile = "Upload2023/AffiliationDocuments/" + filename;


                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            ///file.CopyTo(newMemoryStream);

                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = docfile.InputStream,
                                Key = string.Format("allfiles/Upload2023/AffiliationDocuments/{0}", filename),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }
                }


                if (cmd.ToLower() == "submit" || cmd.ToLower() == "save")
                {

                    result = affiliationDB.InsertAffiliationDocumentDetails(adm, 0, out outError);  // 0 for insert

                    //if (!Directory.Exists(FilepathExist))
                    //{
                    //    Directory.CreateDirectory(FilepathExist);
                    //}
                    //docfile.SaveAs(path);
                }
                else if (cmd.ToLower() == "delete")
                {
                    adm.SCHL = Convert.ToString(id);
                    // adm.eDocId = Convert.ToInt32(eStaffId);
                    result = affiliationDB.InsertAffiliationDocumentDetails(adm, 2, out outError); // 2 for delete
                    if (outError == "1")
                    {
                        ViewBag.result = "1";
                        ViewData["Status"] = "DEL";
                        return View(adm);
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




            DataSet outDsAm = new DataSet();
            AffiliationModel am1 = affiliationDB.AffiliationContinuationBySchl(Session["SCHL"].ToString(), 1, out outDsAm);//ResultStatics
            if (am1.ID > 0)
            {
                ViewBag.AID = am1.ID;
                ViewBag.ChallanId = am1.ChallanId;
                ViewBag.challanVerify = am1.challanVerify;
                ViewBag.RS10GPass2017 = am1.RS10GPass2017;
                ViewBag.SF10Percent = am1.SF10Percent;
                ViewBag.BSFROM = am1.BSFROM;
                ViewBag.FSFROM = am1.FSFROM;
                ViewBag.BPNAME = am1.BPNAME;
                ViewBag.ASZONE = am1.ASZONE;
                ViewBag.ASZONE = am1.ASZONE;
                ViewBag.OI1 = am1.OI1;
            }
            else
            {
                ViewBag.AID = 0;
                ViewBag.ChallanId = 0;
            }

            return View(adm);
        }


        public ActionResult ActionUploadAffiliationDocuments(string id, string eDocId, string act)
        {
            try
            {
                string outError = "0";
                int result = 0;
                AffiliationDocumentDetailsModel easdm = new AffiliationDocumentDetailsModel();
                if (id == null || eDocId == null)
                {
                    //return RedirectToAction("Index", "AffiliationContinuation");
                }
                else
                {
                    if (act == "D")
                    {
                        easdm.SCHL = Convert.ToString(id);
                        easdm.eDocId = Convert.ToInt32(eDocId);
                        result = affiliationDB.InsertAffiliationDocumentDetails(easdm, 2, out outError); // 2 for delete
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
            return RedirectToAction("UploadAffiliationContinuationDocuments", "AffiliationContinuation", new { id = id.ToString() });

        }

        #endregion

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