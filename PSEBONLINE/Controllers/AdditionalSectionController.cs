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
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon;

namespace PSEBONLINE.Controllers
{
    [SessionCheckFilter]
    public class AdditionalSectionController : Controller
    {
        private const string BUCKET_NAME = "psebdata";
        private DBContext _context = new DBContext();

        public AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        public AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        public AbstractLayer.AdditionalSectionDB additionalSectionDB = new AbstractLayer.AdditionalSectionDB();
        public AbstractLayer.SchoolDB ObjSchoolDB = new AbstractLayer.SchoolDB();

               

        // GET: AdditionalSection
        public ActionResult Index(AdditionalSectionModel am)
        {
            ViewBag.AID = null;
            ViewBag.ChallanId = null;
            ViewBag.challanVerify = null;
            if (Request.UrlReferrer != null)
            {
                if (Request.UrlReferrer.ToString().Contains("AdditionalSection/CalculateFee"))
                {
                    return RedirectToAction("CalculateFee/"+ Session["SCHL"].ToString(), "AdditionalSection");
                    //("CalculateFee", "AdditionalSection")
                }
            }
            AdditionalSectionDashBoardViews additionalSectionDashBoardModel = new AdditionalSectionDashBoardViews();
            am.additionalSectionDashBoardViews = new AdditionalSectionDashBoardViews();

           string schl = Session["SCHL"].ToString();
            ViewBag.SCHL = schl;
           
            

            DataSet outDs = new DataSet();
            am = additionalSectionDB.AdditionalSectionBySchl(Session["SCHL"].ToString(), 1, out outDs);//ResultStatics
            if (am.ID > 0)
            {
                ViewBag.AID = am.ID;
                ViewBag.ChallanId = am.ChallanId;
                ViewBag.challanVerify = am.challanVerify;

                additionalSectionDashBoardModel = _context.AdditionalSectionDashBoardViews.Where(s => s.SCHL == schl).FirstOrDefault();
                am.additionalSectionDashBoardViews = additionalSectionDashBoardModel;

                am.affObjectionLettersViewList = _context.AffObjectionLettersViews.Where(s => s.AppNo == schl  && s.AppType == "AS").ToList();

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
                    return RedirectToAction("Index", "AdditionalSection");
                }

                ViewBag.SCHL = Session["SCHL"].ToString();
                if (id != ViewBag.SCHL)
                {
                    return RedirectToAction("Index", "AdditionalSection");
                }

                DataSet outDs = new DataSet();
                AdditionalSectionModel am1 = additionalSectionDB.AdditionalSectionBySchl(Session["SCHL"].ToString(), 1, out outDs);//ResultStatics
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
        public ActionResult ResultStatics(string id, AdditionalSectionModel am)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }

            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }

            DataSet outDs = new DataSet();
            am = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);//ResultStatics
            if (am.ID > 0)
            {
                ViewBag.Totalcount = 1;
                return View(am);
            }
            else
            {
                am = additionalSectionDB.GetAdditionalSectionBySchlAndType(id, 1, out outDs);//ResultStatics
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
        public ActionResult ResultStatics(string id, AdditionalSectionModel am, FormCollection fc)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            am.SCHL = id;
            DataSet outDs = new DataSet();
            AdditionalSectionModel amMain = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);//ResultStatics
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
            int result = additionalSectionDB.AdditionalSection(am, out outError);// if ID=0 then Insert else Update
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
        public ActionResult StudentFeeDetails(string id, AdditionalSectionModel am)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }

            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }
            DataSet ds = new DataSet();
            SchoolPremisesInformation sm = new AbstractLayer.SchoolDB().SchoolPremisesInformationBySchl(ViewBag.SCHL, out ds);
            if (sm.ID == 0)
            {
                DataSet outDs = new DataSet();
                am = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);
                if (am.ID > 0)
                {
                    return View(am);
                }
            }
            else
            {
                DataSet outDs = new DataSet();
                am = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);
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
        public ActionResult StudentFeeDetails(string id, AdditionalSectionModel am, FormCollection fc)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            if (id == null)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            am.SCHL = id;
            DataSet outDs = new DataSet();
            AdditionalSectionModel amMain = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);//ResultStatics
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

                // add three years

                amMain.SF1TC2019 = am.SF1TC2019;
                amMain.SF1TF2019 = am.SF1TF2019;
                amMain.SF2TC2019 = am.SF2TC2019;
                amMain.SF2TF2019 = am.SF2TF2019;
                amMain.SF3TC2019 = am.SF3TC2019;
                amMain.SF3TF2019 = am.SF3TF2019;
                amMain.SF4TC2019 = am.SF4TC2019;
                amMain.SF4TF2019 = am.SF4TF2019;
                amMain.SF5TC2019 = am.SF5TC2019;
                amMain.SF5TF2019 = am.SF5TF2019;
                amMain.SF6TC2019 = am.SF6TC2019;
                amMain.SF6TF2019 = am.SF6TF2019;
                amMain.SF7TC2019 = am.SF7TC2019;
                amMain.SF7TF2019 = am.SF7TF2019;
                amMain.SF8TC2019 = am.SF8TC2019;
                amMain.SF8TF2019 = am.SF8TF2019;
                amMain.SF9TC2019 = am.SF9TC2019;
                amMain.SF9TF2019 = am.SF9TF2019;
                amMain.SF10TC2019 = am.SF10TC2019;
                amMain.SF10TF2019 = am.SF10TF2019;
                amMain.SF11HTC2019 = am.SF11HTC2019;
                amMain.SF11HTF2019 = am.SF11HTF2019;
                amMain.SF11STC2019 = am.SF11STC2019;
                amMain.SF11STF2019 = am.SF11STF2019;
                amMain.SF11CTC2019 = am.SF11CTC2019;
                amMain.SF11CTF2019 = am.SF11CTF2019;
                amMain.SF11VTC2019 = am.SF11VTC2019;
                amMain.SF11VTF2019 = am.SF11VTF2019;
                amMain.SF12HTC2019 = am.SF12HTC2019;
                amMain.SF12HTF2019 = am.SF12HTF2019;
                amMain.SF12STC2019 = am.SF12STC2019;
                amMain.SF12STF2019 = am.SF12STF2019;
                amMain.SF12CTC2019 = am.SF12CTC2019;
                amMain.SF12CTF2019 = am.SF12CTF2019;
                amMain.SF12VTC2019 = am.SF12VTC2019;
                amMain.SF12VTF2019 = am.SF12VTF2019;
                //
                am = amMain;
            }


            string outError = "0";
            int result = additionalSectionDB.AdditionalSection(am, out outError);// if ID=0 then Insert else Update
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
        public ActionResult BalanceSheet(string id, AdditionalSectionModel am)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            if (id == null)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }


            ViewBag.SessionList = objCommon.GetSessionAdmin().Where(s => Convert.ToInt32(s.Value.Substring(0, 4)) <= 2018).ToList().Take(2); //GetSessionAdmin

            DataSet outDs = new DataSet();
            am = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);
            if (am.ID > 0)
            {
                if (am.BSDFILE == "" || am.BSDFILE == null)
                { @ViewBag.BSDFILE = "../Images/NoPhoto.jpg"; }
                else
                {
                    @ViewBag.BSDFILE = "../../Upload/AdditionalSection/BalanceSheet" + am.BSDFILE.ToString();
                }
            }
            return View(am);
        }

        [HttpPost]
        public ActionResult BalanceSheet(string id, AdditionalSectionModel am, FormCollection fc, HttpPostedFileBase bsdfile)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            if (id == null)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }
            ViewBag.SessionList = objCommon.GetSessionAdmin().Where(s => Convert.ToInt32(s.Value.Substring(0, 4)) <= 2018).ToList().Take(2); //GetSessionAdmin
            ViewBag.SCHL = Session["SCHL"].ToString();
            am.SCHL = id;
            string filename = "";
            if (bsdfile != null && bsdfile.ContentLength > 0)
            {
                string ext = Path.GetExtension(bsdfile.FileName);
                filename = am.SCHL + "_BSDFILE" + ext;
                var path = Path.Combine(Server.MapPath("~/Upload/AdditionalSection/BalanceSheet"), filename);
                string FilepathExist = Path.Combine(Server.MapPath("~/Upload/AdditionalSection/BalanceSheet"));
                if (!Directory.Exists(FilepathExist))
                {
                    Directory.CreateDirectory(FilepathExist);
                }
                bsdfile.SaveAs(path);
                am.BSDFILE = "AdditionalSection/BalanceSheet/" + filename;
            }

            DataSet outDs = new DataSet();
            AdditionalSectionModel amMain = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);//ResultStatics
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
            int result = additionalSectionDB.AdditionalSection(am, out outError);// if ID=0 then Insert else Update
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
        public ActionResult SafetyDetails(string id, AdditionalSectionModel am)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }


            // YesNo 
            //   ViewBag.YearList = objCommon.GetSessionYear();
            List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession = objCommon.GetSessionYear();
            itemSession.Add(new SelectListItem { Text = "2019", Value = "2019" });
            itemSession.Add(new SelectListItem { Text = "2020", Value = "2020" });
            itemSession.Add(new SelectListItem { Text = "2021", Value = "2021" });
            ViewBag.YearList = itemSession.Where(s => Convert.ToInt32(s.Value) >= 2016).OrderByDescending(s => s.Value);


            ViewBag.SelectedYear = "0";
            var itemsch = new SelectList(new[]{new {ID="1",Name="Punjab Public Works Department, B&R"},new {ID="2",Name="Department of Rural development and Panchayat"},
            }, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "0";

            DataSet outDs = new DataSet();
            am = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);//ResultStatics
            if (am.ID > 0)
            {
                if (am.BSFILE == "" || am.BSFILE == null)
                { @ViewBag.BSFILE = "../Images/NoPhoto.jpg"; }
                else
                {
                    @ViewBag.BSFILE = "../../Upload/AdditionalSection/SafetyDetails" + am.BSFILE.ToString();
                }

                if (am.FSFILE == "" || am.FSFILE == null)
                { @ViewBag.FSFILE = "../Images/NoPhoto.jpg"; }
                else
                {
                    @ViewBag.FSFILE = "../../Upload/AdditionalSection/SafetyDetails" + am.FSFILE.ToString();
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
        public ActionResult SafetyDetails(string id, AdditionalSectionModel am, FormCollection fc, HttpPostedFileBase bsfile, HttpPostedFileBase fsfile)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AdditionalSection");
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
                var path = Path.Combine(Server.MapPath("~/Upload/AdditionalSection/SafetyDetails"), filename);
                string FilepathExist = Path.Combine(Server.MapPath("~/Upload/AdditionalSection/SafetyDetails"));
                if (!Directory.Exists(FilepathExist))
                {
                    Directory.CreateDirectory(FilepathExist);
                }
                bsfile.SaveAs(path);
                am.BSFILE = "AdditionalSection/SafetyDetails/" + filename;
            }

            string filename1 = "";
            if (fsfile != null)
            {
                string ext = Path.GetExtension(bsfile.FileName);
                filename1 = am.SCHL + "_FSFILE" + ext;
                var path = Path.Combine(Server.MapPath("~/Upload/AdditionalSection/SafetyDetails"), filename1);
                string FilepathExist = Path.Combine(Server.MapPath("~/Upload/AdditionalSection/SafetyDetails"));
                if (!Directory.Exists(FilepathExist))
                {
                    Directory.CreateDirectory(FilepathExist);
                }
                fsfile.SaveAs(path);
                am.FSFILE = "AdditionalSection/SafetyDetails/" + filename1;
            }

            DataSet outDs = new DataSet();
            AdditionalSectionModel amMain = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);//ResultStatics
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
            int result = additionalSectionDB.AdditionalSection(am, out outError);// if ID=0 then Insert else Update
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
        public ActionResult BooksPurchaseDetails(string id, AdditionalSectionModel am)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }

            ViewBag.DistOfficerList = objCommon.GetDistE();
            DataSet outDs = new DataSet();
            am = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);//ResultStatics
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
        public ActionResult BooksPurchaseDetails(string id, AdditionalSectionModel am, FormCollection fc)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();

            ViewBag.DistOfficerList = objCommon.GetDistE();
            am.SCHL = id;
            DataSet outDs = new DataSet();
            AdditionalSectionModel amMain = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);//ResultStatics
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
            int result = additionalSectionDB.AdditionalSection(am, out outError);// if ID=0 then Insert else Update
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
        public ActionResult Activities(string id, AdditionalSectionModel am)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }
            ViewBag.YesNoList = objCommon.GetYesNoText();
            DataSet outDs = new DataSet();
            DataSet ds = new DataSet();
            SchoolPremisesInformation sm = new AbstractLayer.SchoolDB().SchoolPremisesInformationBySchl(ViewBag.SCHL, out ds);
            if (sm.ID == 0)
            {
                am = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);//ResultStatics
                if (am.ID > 0)
                {
                    return View(am);
                }
            }
            else
            {
                am = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);//ResultStatics
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
        public ActionResult Activities(string id, AdditionalSectionModel am, FormCollection fc)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            am.SCHL = id;
            ViewBag.YesNoList = objCommon.GetYesNoText();
            // Update Main Column Only
            DataSet outDs = new DataSet();
            AdditionalSectionModel amMain = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);//ResultStatics
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
            int result = additionalSectionDB.AdditionalSection(am, out outError);// if ID=0 then Insert else Update
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
        public ActionResult OtherInformation(string id, AdditionalSectionModel am)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }

            ViewBag.YesNoList = objCommon.GetYesNoText();
            DataSet outDs = new DataSet();
            DataSet ds = new DataSet();
            SchoolPremisesInformation sm = new AbstractLayer.SchoolDB().SchoolPremisesInformationBySchl(ViewBag.SCHL, out ds);
            if (sm.ID == 0)
            {
                am = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);//ResultStatics
                if (am.ID > 0)
                {
                    return View(am);
                }
            }
            else
            {
                am = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);//ResultStatics
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
        public ActionResult OtherInformation(string id, AdditionalSectionModel am, FormCollection fc)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }
            ViewBag.YesNoList = objCommon.GetYesNoText();
            am.SCHL = id;

            // Update Main Column Only
            DataSet outDs = new DataSet();
            AdditionalSectionModel amMain = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);//ResultStatics
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
            int result = additionalSectionDB.AdditionalSection(am, out outError);// if ID=0 then Insert else Update
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


        public JsonResult GetGroupByClass(string SelClass)
        {
            List<SelectListItem> objGroupList = new List<SelectListItem>();
         

            if (SelClass == "12")
            {
                DataSet dschk = new AbstractLayer.SchoolDB().GetAssignExamGroupBySchl(Session["SCHL"].ToString());
                if (dschk.Tables.Count > 0)
                {   
                    foreach (DataRow dr in dschk.Tables[1].Rows) // For addition Section
                    {
                        string GroupNM = dr["GroupNM"].ToString();                       
                        objGroupList.Add(new SelectListItem { Text = GroupNM, Value = GroupNM });                        
                    }                    
                }
            }
            else
            { objGroupList.Add(new SelectListItem { Text = "GENERAL", Value = "GENERAL" }); }
            ViewBag.GroupList = objGroupList;
            return Json(objGroupList);
        }

        public ActionResult CalculateFee(string id, AdditionalSectionPaymentDetailsModel eapdm)
        {         

            if (id == null)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }

            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }

            DataSet dsAllowBank = objCommon.CheckBankAllowByFeeCodeDate(66);// 59 for EAffiliation
            ViewBag.dsAllowBank = dsAllowBank;
            if (dsAllowBank == null || dsAllowBank.Tables[0].Rows.Count == 0)
            {
                ViewBag.IsAllowBank = 0;
            }
            else { ViewBag.IsAllowBank = 1; }

            List<SelectListItem> objGroupList = new List<SelectListItem>();
            ViewBag.GroupList = objGroupList;
            // ClassList
            ViewBag.ClassList  = AbstractLayer.AdditionalSectionDB.GetAdditionalSectionClassMasterList().ToList();
            //
            ViewBag.CountListOLD = new AbstractLayer.DBClass().GetCountList().Where(s=> Convert.ToInt32(s.Value) <=10).ToList();
            ViewBag.CountListNEW = new AbstractLayer.DBClass().GetCountList().Where(s => Convert.ToInt32(s.Value) > 0 && Convert.ToInt32(s.Value) <= 10).ToList();



            string result = additionalSectionDB.IsValidForChallan(Session["SCHL"].ToString());
            if (result != string.Empty)
            {
                TempData["notValidForChallan"] = result;
                ViewBag.RequiredData = 0;
            }
            else { ViewBag.RequiredData = 1; }


            DataSet outDsAm = new DataSet();
            AdditionalSectionModel am1 = new AdditionalSectionModel();
            am1 = additionalSectionDB.AdditionalSectionBySchl(Session["SCHL"].ToString(), 2, out outDsAm); 
            if (am1.SCHL != id)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }
            else
            {
                //Form lock status

                //if (Session["eAffiliationIsFormLock"].ToString() != eam.IsFormLock.ToString())
                //{
                //    Session["eAffiliationIsFormLock"] = eam.IsFormLock.ToString();

                //}

              

                ViewBag.AID = am1.ID;
                ViewBag.IsSchlSubmiited = 1;
                ViewBag.ChallanId = am1.ChallanId;
                ViewBag.challanVerify = am1.challanVerify;
                //ViewBag.RS10GPass2017 = am1.RS10GPass2017;
                //ViewBag.SF10Percent = am1.SF10Percent;
                //ViewBag.BSFROM = am1.BSFROM;
                //ViewBag.FSFROM = am1.FSFROM;
                //ViewBag.BPNAME = am1.BPNAME;
                //ViewBag.ASZONE = am1.ASZONE;
                //ViewBag.ASZONE = am1.ASZONE;
                //ViewBag.OI1 = am1.OI1;

                //int TotalStaff = outDsAm.Tables[3].Rows.Count;
                //string principal = outDsAm.Tables[2].Rows[0]["PRINCIPAL"].ToString();
                //string mobile = outDsAm.Tables[2].Rows[0]["MOBILE"].ToString();

                //int TotalStudent201819 = (am1.SF1TC2018 + am1.SF2TC2018 + am1.SF3TC2018 + am1.SF4TC2018 + am1.SF5TC2018 + am1.SF6TC2018 + am1.SF7TC2018 + am1.SF8TC2018 + am1.SF9TC2018 + am1.SF10TC2018 + am1.SF11HTC2018
                //                        + am1.SF11STC2018 + am1.SF11CTC2018 + am1.SF11VTC2018 + am1.SF12HTC2018 + am1.SF12STC2018 + am1.SF12CTC2018 + am1.SF12VTC2018);

                //ViewBag.TotalStudent201819 = 0;
                //ViewBag.TS1 = TotalStudent201819;
                //ViewBag.TS2 = am1.BPTS;
                //ViewBag.TotalStudent201819 = 1;

               // payment Details
                eapdm.StoreAllData = new AbstractLayer.AdditionalSectionDB().GetAdditionalSectionPaymentDetails(am1.SCHL);
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
        public ActionResult CalculateFee(string id, AdditionalSectionPaymentDetailsModel eapdm, FormCollection frm, string cmd,string SelOldSection,string SelNewSection)
        {

            if (id == null)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }

            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AdditionalSection");
            }

            DataSet dsAllowBank = objCommon.CheckBankAllowByFeeCodeDate(66);// 59 for EAffiliation
            ViewBag.dsAllowBank = dsAllowBank;
            if (dsAllowBank == null || dsAllowBank.Tables[0].Rows.Count == 0)
            {
                ViewBag.IsAllowBank = 0;
            }
            else { ViewBag.IsAllowBank = 1; }

            List<SelectListItem> objGroupList = new List<SelectListItem>();
            ViewBag.GroupList = objGroupList;
            // ClassList
            ViewBag.ClassList = AbstractLayer.AdditionalSectionDB.GetAdditionalSectionClassMasterList().ToList();
            //
            ViewBag.CountListOLD = new AbstractLayer.DBClass().GetCountList().Where(s => Convert.ToInt32(s.Value) <= 10).ToList();
            ViewBag.CountListNEW = new AbstractLayer.DBClass().GetCountList().Where(s => Convert.ToInt32(s.Value) > 0 && Convert.ToInt32(s.Value) <= 10).ToList();

            AdditionalSectionModel eam = new AdditionalSectionModel();

            if (!string.IsNullOrEmpty(cmd))
            {
                if (cmd.ToLower().Contains("add") && !string.IsNullOrEmpty(SelOldSection) &&  !string.IsNullOrEmpty(SelNewSection))
                {
                    eapdm.cls = frm["SelClass"].ToString();
                    eapdm.SCHL = id;
                    if (eapdm.cls == "12")
                    { eapdm.exam = frm["SelGroup"].ToString(); }
                    else { eapdm.exam = "GENERAL"; }

                    eapdm.OldSection = Convert.ToInt32(SelOldSection);
                    eapdm.NewSection = Convert.ToInt32(SelNewSection);

                    if (eapdm.NewSection <=0)
                    {
                        TempData["StatusCF"] = "21";
                        return RedirectToAction("CalculateFee", "AdditionalSection", new { id = ViewBag.SCHL });
                    }

                    eapdm.fee = eapdm.latefee = eapdm.id = 0;
                    string OutResult = "";
                    if (string.IsNullOrEmpty(eapdm.cls) || string.IsNullOrEmpty(eapdm.exam) || eapdm.NewSection <= 0)
                    {
                        TempData["StatusCF"] = "11";
                    }
                    else
                    {
                        DataSet dsAdd =  additionalSectionDB.InsertAdditionalSectionPaymentDetails(eapdm, 0, out OutResult);
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

            return RedirectToAction("CalculateFee", "AdditionalSection", new { id = ViewBag.SCHL });
            //return View(eapdm);
        }


        public ActionResult AdditionalSectionActions(string id, string act)
        {
            try
            {
                string OutResult = "";
                AdditionalSectionPaymentDetailsModel sam = new AdditionalSectionPaymentDetailsModel();
                if (id == null)
                {
                    return RedirectToAction("Index", "AdditionalSection");
                }
                else
                {
                    if (act == "D")
                    {
                        sam.id = Convert.ToInt32(id);
                        DataSet dsAdd = additionalSectionDB.InsertAdditionalSectionPaymentDetails(sam, 1, out OutResult);
                        if (OutResult == "1")
                        {
                            @ViewBag.result = "1";
                            TempData["StatusCF"] = "DEL";
                        }
                    }
                    else if (act == "FS")// Final Submit
                    {
                        sam.SCHL = Convert.ToString(id);
                        DataSet dsAdd = additionalSectionDB.InsertAdditionalSectionPaymentDetails(sam, 2, out OutResult);
                        if (OutResult == "1")
                        {
                            @ViewBag.result = "1";
                            TempData["StatusCF"] = "FS";
                        }
                    }
                    else if (act == "UF")// Unlock Final Submit
                    {
                        sam.SCHL = Convert.ToString(id);
                        DataSet dsAdd = additionalSectionDB.InsertAdditionalSectionPaymentDetails(sam, 3, out OutResult);
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
                return RedirectToAction("CalculateFee", "AdditionalSection", new { Id = id.ToString() });

            }
            catch (Exception)
            {
                return RedirectToAction("CalculateFee", "AdditionalSection", new { Id = id.ToString() });
            }
        }



        #region Challan and Payment Details AdditionalSection
        public ActionResult PaymentFormAdditionalSection(string id)
        {
            try
            {
                Session["Session"] = @PSEBONLINE.Repository.SessionSettingMastersRepository.GetSessionSettingMasters().SessionShortYear;
                if (string.IsNullOrEmpty(id) )
                {
                    return RedirectToAction("Index", "AdditionalSection");
                }

                AdditionalSectionFee _AdditionalSectionFee = new AdditionalSectionFee();

                string schl = id;
                string today = DateTime.Today.ToString("dd/MM/yyyy");
                DateTime dateselected;
                if (DateTime.TryParseExact(today, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
                {
                    ViewData["result"] = 5;
                    DataSet ds = additionalSectionDB.GetAdditionalSectionPayment(schl);
                    _AdditionalSectionFee.PaymentFormData = ds;
                    if (_AdditionalSectionFee.PaymentFormData == null || _AdditionalSectionFee.PaymentFormData.Tables[0].Rows.Count == 0)
                    { ViewBag.TotalCount = 0; Session["EAffiliationFee"] = null; }
                    else
                    {
                        Session["AdditionalSectionFee"] = ds;
                        //ViewBag.TotalFee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("fee")));
                        //ViewBag.TotalLateFee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("latefee")));
                        //ViewBag.TotalTotfee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("Totfee")));

                 
                        ViewBag.TotalFee = Convert.ToInt32(ds.Tables[1].Rows[0]["fee"].ToString());
                        ViewBag.TotalLateFee = Convert.ToInt32(ds.Tables[1].Rows[0]["latefee"].ToString());
                        ViewBag.TotalTotfee = Convert.ToInt32(ds.Tables[1].Rows[0]["totfee"].ToString());
                        ViewBag.Total = ViewBag.TotalTotfee;

                        ViewData["result"] = 10;
                        ViewData["FeeStatus"] = "1";
                        ViewBag.TotalCount = 1;
                        return View(_AdditionalSectionFee);
                    }

                }
                else
                {
                    ViewData["OutError"] = "Date Format Problem";
                }
                return View(_AdditionalSectionFee);
            }
            catch (Exception)
            {
                return View();
                /// return RedirectToAction("SchoolAccreditation", "School");
            }
        }
        [HttpPost]
        public ActionResult PaymentFormAdditionalSection(string id, FormCollection frm, string PayModValue, string AllowBanks)
        {
            try
            {

                EAffiliationFee pfvm = new EAffiliationFee();
                ChallanMasterModel CM = new ChallanMasterModel();

                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("Index", "AdditionalSection");
                }
                if (Session["AdditionalSectionFee"] == null)
                {
                    return RedirectToAction("CalculateFee", "AdditionalSection");
                }
                string schl = id;
                DataSet ds = (DataSet)Session["AdditionalSectionFee"];
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



                //ViewBag.TotalFee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("fee")));
                //ViewBag.TotalLateFee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("latefee")));
                //ViewBag.TotalTotfee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("Totfee")));

                ViewBag.TotalFee = Convert.ToInt32(ds.Tables[1].Rows[0]["fee"].ToString());
                ViewBag.TotalLateFee = Convert.ToInt32(ds.Tables[1].Rows[0]["latefee"].ToString());
                ViewBag.TotalTotfee = Convert.ToInt32(ds.Tables[1].Rows[0]["totfee"].ToString());


                ViewBag.Total = ViewBag.TotalTotfee;

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
                    CM.SchoolCode = schl;
                    CM.DIST = "";
                    CM.DISTNM = "";
                    CM.LOT = 1;
                    CM.SCHLREGID = schl;
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

                            string Sms = "Addition Section Challan no. " + result + " of Ref no  " + CM.APPNO + " successfully generated and valid till Dt " + bnkLastDate + ". Regards PSEB";
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
                return RedirectToAction("AdditionalSection", "Index");
            }
        }

        #endregion Challan and Payment Details

        #endregion


        #region PrintForm
        public ActionResult PrintForm(string id, AdditionalSectionModel am)
        {
            try
            {

                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Home"); }
                //
                if (id == null)
                {
                    return RedirectToAction("Index", "AdditionalSection");
                }

                ViewBag.SCHL = Session["SCHL"].ToString();
                if (id != ViewBag.SCHL)
                {
                    return RedirectToAction("Index", "AdditionalSection");
                }
                SchoolModels sm = new SchoolModels();
                DataSet outDs = new DataSet();
                am = additionalSectionDB.AdditionalSectionBySchl(id, 2, out outDs);//Type2 ( for staff details)
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
        public ActionResult FinalPrintForm(string id, AdditionalSectionModel am)
        {
            try
            {

                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Home"); }
                //
                if (id == null)
                {
                    return RedirectToAction("Index", "AdditionalSection");
                }

                ViewBag.SCHL = Session["SCHL"].ToString();
                if (id != ViewBag.SCHL)
                {
                    return RedirectToAction("Index", "AdditionalSection");
                }

                DataSet outDs = new DataSet();
                am = additionalSectionDB.AdditionalSectionBySchl(id, 2, out outDs);//ResultStatics
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



        #region Upload AdditionalSection Documents
        public ActionResult UploadAdditionalSectionDocuments(string id, AdditionalSectionDocumentDetailsModel adm)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "AdditionalSection");
            }

            adm.SCHL = id;
            DataSet outDs = new DataSet();
            AdditionalSectionModel am1 = additionalSectionDB.AdditionalSectionBySchl(id, 1, out outDs);//ResultStatics
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

                adm.StoreAllData = additionalSectionDB.GetAdditionalSectionDocumentDetails(2, 0, id, "");
                adm.AdditionalSectionDocumentMasterList = additionalSectionDB.AdditionalSectionDocumentMasterList(adm.StoreAllData.Tables[1]);//  Document List
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
        public ActionResult UploadAdditionalSectionDocuments(string id, AdditionalSectionDocumentDetailsModel adm, string cmd,string OtherDocumentName, FormCollection frm, HttpPostedFileBase docfile)
        {
            try
            {


                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("Index", "AdditionalSection");
                }
                adm.StoreAllData = additionalSectionDB.GetAdditionalSectionDocumentDetails(2, 0, id, "");
                adm.AdditionalSectionDocumentMasterList = additionalSectionDB.AdditionalSectionDocumentMasterList(adm.StoreAllData.Tables[1]);//  Document List
                adm.SCHL = id;

                string DocName = adm.AdditionalSectionDocumentMasterList.Where(s => s.DocID == adm.DocID).Select(s => s.DocumentName).FirstOrDefault();

                if (!string.IsNullOrEmpty(OtherDocumentName) && adm.DocID == 3)
                {
                    DocName = "OtherDocument_" + OtherDocumentName;
                }

                if (!string.IsNullOrEmpty(cmd))
                {
                    string outError = "0";
                    int result = 0;

                    // Save file
                    string filename = "";
                    string FilepathExist = "", path = "";
                    string exactPath = "~/Upload/Upload2023/AdditionalSectionDocuments";
                    if (docfile != null)
                    {
                        //Upload/AdditionalSection
                        string ext = Path.GetExtension(docfile.FileName);
                        filename = adm.SCHL + "_" + DocName.Replace(" ", "_") + ext;
                        path = Path.Combine(Server.MapPath(exactPath), filename);
                        FilepathExist = Path.Combine(Server.MapPath(exactPath));
                        adm.DocFile = "allfiles/Upload2023/AdditionalSectionDocuments/" + filename;
                    }


                    if (cmd.ToLower() == "submit" || cmd.ToLower() == "save")
                    {

                        result = additionalSectionDB.InsertAdditionalSectionDocumentDetails(adm, 0, out outError);  // 0 for insert
                       
                    }
                    else if (cmd.ToLower() == "delete")
                    {
                        adm.SCHL = Convert.ToString(id);
                        // adm.eDocId = Convert.ToInt32(eStaffId);
                        result = additionalSectionDB.InsertAdditionalSectionDocumentDetails(adm, 2, out outError); // 2 for delete
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

                        if (cmd.ToLower() == "submit" || cmd.ToLower() == "save")
                        {

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
                                        Key = string.Format("allfiles/Upload2023/AdditionalSectionDocuments/{0}", Orgfile),

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
                        ViewData["result"] = outError.ToString();
                        ViewBag.Mesaage = outError;
                    }
                }




                DataSet outDsAm = new DataSet();
                AdditionalSectionModel am1 = additionalSectionDB.AdditionalSectionBySchl(Session["SCHL"].ToString(), 1, out outDsAm);//ResultStatics
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

            }
            catch (Exception ex)
            {
                
            }
            return View(adm);
        }


        public ActionResult ActionUploadAdditionalSectionDocuments(string id, string eDocId, string act)
        {
            try
            {
                string outError = "0";
                int result = 0;
                AdditionalSectionDocumentDetailsModel easdm = new AdditionalSectionDocumentDetailsModel();
                if (id == null || eDocId == null)
                {
                    //return RedirectToAction("Index", "AdditionalSection");
                }
                else
                {
                    if (act == "D")
                    {
                        easdm.SCHL = Convert.ToString(id);
                        easdm.eDocId = Convert.ToInt32(eDocId);
                        result = additionalSectionDB.InsertAdditionalSectionDocumentDetails(easdm, 2, out outError); // 2 for delete
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
            return RedirectToAction("UploadAdditionalSectionDocuments", "AdditionalSection", new { id = id.ToString() });

        }


      
        #endregion




    }
}