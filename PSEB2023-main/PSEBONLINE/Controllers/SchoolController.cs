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
using System.Threading.Tasks;
using PSEBONLINE.Filters;
using CCA.Util;
using System.Configuration;
using PsebPrimaryMiddle.Controllers;
using System.Data.Entity;
using PSEBONLINE.Repository;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon;

namespace PSEBONLINE.Controllers
{
    [RoutePrefix("School")]
    public class SchoolController : Controller
    {
        private const string BUCKET_NAME = "psebdata";

        private readonly DBContext _context = new DBContext();
        string sp = System.Configuration.ConfigurationManager.AppSettings["upload"];
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        AbstractLayer.RegistrationDB objDBReg = new AbstractLayer.RegistrationDB();
        AbstractLayer.DEODB OBJDB = new AbstractLayer.DEODB();
        string sp1 = System.Configuration.ConfigurationManager.AppSettings["ImagePathCor"];


        private readonly ISchoolRepository _schoolRepository;

        public SchoolController(ISchoolRepository schoolRepository)
        {
            _schoolRepository = schoolRepository;
        }


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
                    //SiteMenu model = new SiteMenu();
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
                             new RouteValueDictionary(new { controller = "Admin", action = "PageNotAuthorized" }));
                            // context.Result = new RedirectResult("~/Admin/PageNotAuthorized"); // Page not Authorized
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
                            new RouteValueDictionary(new { controller = "Admin", action = "PageNotAuthorized" }));
                        return;
                    }
                    ViewBag.SiteMenu = all;
                }
            }
            catch (Exception)
            {
                context.Result = new RedirectToRouteResult(
                             new RouteValueDictionary(new { controller = "Admin", action = "Index" }));
                return;
            }
        }


        ////Executes after every action
        //protected override void OnActionExecuted(ActionExecutedContext context)
        //{

        //    base.OnActionExecuted(context);           
        //}
        #endregion SiteMenu


        #region SchoolAccreditation
        public JsonResult GetGroupByClass(string SelClass)
        {
            DataSet result = objDBReg.schooltypes(Session["SCHL"].ToString()); // passing Value to DBClass from model  
            List<SelectListItem> objGroupList = new List<SelectListItem>();
            if (result.Tables[8].Rows.Count > 0)
            {

                if (SelClass == "10")
                {
                    if (result.Tables[8].Rows[0]["Matric"].ToString() == "1") { objGroupList.Add(new SelectListItem { Text = "GENERAL", Value = "GENERAL" }); }

                }
                else if (SelClass == "12")
                {
                    if (result.Tables[8].Rows[0]["HUM"].ToString() == "1") { objGroupList.Add(new SelectListItem { Text = "HUMANITIES", Value = "HUMANITIES" }); }
                    if (result.Tables[8].Rows[0]["sci"].ToString() == "1") { objGroupList.Add(new SelectListItem { Text = "SCIENCE", Value = "SCIENCE" }); }
                    if (result.Tables[8].Rows[0]["comm"].ToString() == "1") { objGroupList.Add(new SelectListItem { Text = "COMMERCE", Value = "COMMERCE" }); }
                }
            }
            ViewBag.GroupList = objGroupList;
            return Json(objGroupList);
        }


        public JsonResult CheckNewReNewByGroupByClass(string SelClass, string SelGroup)
        {
            DataSet result = objDBReg.schooltypes(Session["SCHL"].ToString()); // passing Value to DBClass from model  
            List<SelectListItem> objList = new List<SelectListItem>();
            if (result.Tables[8].Rows.Count > 0)
            {
                DateTime todayDate = DateTime.Today;
                DateTime lastDT_OPEN_ACC_RENEW = Convert.ToDateTime(result.Tables[8].Rows[0]["OPEN_ACC_RENEW"].ToString());
                if (todayDate > lastDT_OPEN_ACC_RENEW)
                {
                    objList.Add(new SelectListItem { Text = "New", Value = "New", Selected = true });
                }
                else
                {


                    if (SelClass == "10")
                    {
                        if (result.Tables[1].Rows[0]["OMatric"].ToString() == "1")
                        { objList.Add(new SelectListItem { Text = "Renewal", Value = "Renewal", Selected = true }); }
                        else
                        { objList.Add(new SelectListItem { Text = "New", Value = "New", Selected = true }); }

                    }
                    else if (SelClass == "12")
                    {
                        if (result.Tables[1].Rows[0]["GovtType"].ToString().ToLower() == "govt")
                        {
                            if (result.Tables[1].Rows[0]["OSenior"].ToString() == "1")
                            {
                                objList.Add(new SelectListItem { Text = "Renewal", Value = "Renewal", Selected = true });
                            }
                            else
                            { objList.Add(new SelectListItem { Text = "New", Value = "New", Selected = true }); }
                        }
                        else if (result.Tables[1].Rows[0]["GovtType"].ToString().ToLower() == "nongovt")
                        {
                            if (SelGroup == "HUMANITIES" && result.Tables[1].Rows[0]["OHUM"].ToString() == "Y")
                            {
                                objList.Add(new SelectListItem { Text = "Renewal", Value = "Renewal", Selected = true });
                            }
                            else if (SelGroup == "SCIENCE" && result.Tables[1].Rows[0]["Osci"].ToString() == "Y")
                            {
                                objList.Add(new SelectListItem { Text = "Renewal", Value = "Renewal", Selected = true });
                            }
                            else if (SelGroup == "COMMERCE" && result.Tables[1].Rows[0]["Ocomm"].ToString() == "Y")
                            {
                                objList.Add(new SelectListItem { Text = "Renewal", Value = "Renewal", Selected = true });
                            }
                            else
                            { objList.Add(new SelectListItem { Text = "New", Value = "New", Selected = true }); }
                        }
                    }
                }
            }
            return Json(objList);
        }

        public ActionResult SchoolAccreditation(SchoolAccreditationCombinedModel sacm)
        {
            try
            {
                string SCHL = string.Empty;
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                { return RedirectToAction("Index", "Home"); }
                else
                {
                    SCHL = Session["SCHL"].ToString();
                }
                ViewBag.SCHL = SCHL;
                var itemsch = new SelectList(new[] { new { ID = "New", Name = "New" }, new { ID = "Renewal", Name = "Renewal" }, }, "ID", "Name", 1);
                ViewBag.AccreditationList = itemsch.ToList();
                ViewBag.SelAccreditation = "0";

                List<SelectListItem> objGroupList = new List<SelectListItem>();
                ViewBag.GroupList = objGroupList;


                DataSet result = objDBReg.schooltypes(Session["SCHL"].ToString()); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {

                    ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                    ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
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
                DataSet dsAllowBank = objCommon.CheckBankAllowByFeeCodeDate(53);// 53 fror school acc
                ViewBag.dsAllowBank = dsAllowBank;
                if (dsAllowBank == null || dsAllowBank.Tables[0].Rows.Count == 0)
                {
                    ViewBag.IsAllowBank = 0;
                }
                else { ViewBag.IsAllowBank = 1; }


                List<SelectListItem> bindClass = new List<SelectListItem>();
                if (ViewBag.MATRICAccreditation == "1" && ViewBag.SeniorAccreditation == "1")
                { bindClass = objCommon.GetAllPSEBCLASS().Where(s => s.Value == "10" || s.Value == "12").ToList(); }
                else if (ViewBag.MATRICAccreditation == "1" && ViewBag.SeniorAccreditation == "0")
                { bindClass = objCommon.GetAllPSEBCLASS().Where(s => s.Value == "10").ToList(); }
                else if (ViewBag.MATRICAccreditation == "0" && ViewBag.SeniorAccreditation == "1")
                { bindClass = objCommon.GetAllPSEBCLASS().Where(s => s.Value == "12").ToList(); }
                ViewBag.ClassList = bindClass;
                // ClassList

                // SchoolAccreditationCombinedModel sacm = new SchoolAccreditationCombinedModel();
                SchoolAccreditationModel sam = new SchoolAccreditationModel();
                DataSet ds = new DataSet();
                SchoolModels sm = objDB.GetSchoolDataBySchl(SCHL, out ds);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    ViewData["Status"] = "NF";
                    ViewBag.IsFinal = ViewBag.TotalCount1 = 0;
                    return View();
                }
                else
                {
                    if (string.IsNullOrEmpty(sm.Bank) || string.IsNullOrEmpty(sm.acno))
                    {
                        ViewData["Status"] = "EMPTYBANK";
                        ViewBag.IsFinal = ViewBag.TotalCount1 = 0;
                        return View(sacm);
                    }

                    sacm.schlmodel = sm;
                    sacm.sam = new SchoolAccreditationModel();
                    sam.StoreAllData = objDB.GetSchoolAccreditation(SCHL);
                    sacm.sam.StoreAllData = sam.StoreAllData.Copy();
                    if (sam.StoreAllData == null || sam.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        // return RedirectToAction("SchoolAccreditation", "School");
                        ViewBag.IsFinal = ViewBag.TotalCount = 0;
                    }
                    else
                    {

                        ViewBag.IsFinal = Convert.ToInt32(sacm.sam.StoreAllData.Tables[0].Rows[0]["IsFinal"].ToString());
                        ViewBag.TotalCount = sacm.sam.StoreAllData.Tables[0].Rows.Count;
                    }


                    // Challan Generated
                    if (sam.StoreAllData == null || sacm.sam.StoreAllData.Tables[1].Rows.Count == 0)
                    {
                        ViewBag.IsChallan = ViewBag.TotalCount1 = 0;
                    }
                    else
                    {
                        ViewBag.IsChallan = Convert.ToInt32(sacm.sam.StoreAllData.Tables[1].Rows[0]["ChallanVerify"].ToString());
                        ViewBag.TotalCount1 = sacm.sam.StoreAllData.Tables[1].Rows.Count;
                    }


                    if (sam.StoreAllData == null || sacm.sam.StoreAllData.Tables[2].Rows.Count == 0)
                    {
                        ViewBag.TotalCount2 = 0;
                    }
                    else
                    {

                        ViewBag.TotalCount2 = sacm.sam.StoreAllData.Tables[2].Rows.Count;
                    }

                }
                return View(sacm);
            }
            catch (Exception ex)
            {
                return View();
            }
        }


		[HttpPost]
        public ActionResult SchoolAccreditation(SchoolAccreditationCombinedModel sacm, FormCollection frm, string cmd)
        {

            try
            {
                sacm.schlmodel = new SchoolModels();
                sacm.sam = new SchoolAccreditationModel();

                string SCHL = string.Empty;
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                { return RedirectToAction("Index", "Home"); }
                else
                {
                    SCHL = Session["SCHL"].ToString();
                    sacm.schlmodel.SCHL = SCHL;
                }
                ViewBag.SCHL = SCHL;
                var itemsch = new SelectList(new[] { new { ID = "New", Name = "New" }, new { ID = "Renewal", Name = "Renewal" }, }, "ID", "Name", 1);
                ViewBag.AccreditationList = itemsch.ToList();
                ViewBag.SelAccreditation = "0";


                DataSet dsAllowBank = objCommon.CheckBankAllowByFeeCodeDate(53);// 53 fror school acc
                ViewBag.dsAllowBank = dsAllowBank;
                if (dsAllowBank == null || dsAllowBank.Tables[0].Rows.Count == 0)
                {
                    ViewBag.IsAllowBank = 0;
                }
                else { ViewBag.IsAllowBank = 1; }


                // GroupList               
                // List<SelectListItem> objGroupList = objCommon.GroupName();
                // objGroupList.Add(new SelectListItem { Text = "GENERAL", Value = "GENERAL" });
                List<SelectListItem> objGroupList = new List<SelectListItem>();
                ViewBag.GroupList = objGroupList;

                // ClassList
                ViewBag.ClassList = objCommon.GetAllPSEBCLASS().Where(s => s.Value == "10" || s.Value == "12").ToList();
                //sacm.sam = new SchoolAccreditationModel();

                if (!string.IsNullOrEmpty(cmd))
                {
                    if (cmd.ToLower().Contains("add"))
                    {
                        sacm.sam.cls = frm["SelClass"].ToString();
                        sacm.sam.schl = SCHL;
                        sacm.sam.exam = frm["SelGroup"].ToString();
                        sacm.sam.Acrtype = frm["SelAccreditation"].ToString();

                        sacm.sam.fee = sacm.sam.latefee = sacm.sam.id = 0;
                        string OutResult = "";
                        if (string.IsNullOrEmpty(sacm.sam.cls) || string.IsNullOrEmpty(sacm.sam.exam) || string.IsNullOrEmpty(sacm.sam.Acrtype))
                        {
                            ViewData["Status"] = "11";
                        }
                        else
                        {

                            DataSet dsAdd = objDB.InsertSchoolAccreditation(sacm.sam, 0, out OutResult);
                            if (OutResult == "1")
                            {
                                ViewData["Status"] = "1";
                                //ViewData["Status"] = MS.StoreAllData.Tables[0].Rows[0]["result"].ToString();
                            }
                            else
                            {
                                ViewData["Status"] = OutResult;
                                //ViewData["Status"] = MS.StoreAllData.Tables[0].Rows[0]["result"].ToString();
                            }
                        }
                    }
                }
                // Get Data 

                #region Get Data 

                SchoolAccreditationModel sam = new SchoolAccreditationModel();
                DataSet ds = new DataSet();
                SchoolModels sm = objDB.GetSchoolDataBySchl(SCHL, out ds);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    ViewData["result"] = 2;
                    ViewBag.IsFinal = ViewBag.TotalCount1 = 0;
                    return View();
                }
                else
                {
                    sacm.schlmodel = sm;
                    sacm.sam = new SchoolAccreditationModel();
                    sam.StoreAllData = objDB.GetSchoolAccreditation(SCHL);
                    sacm.sam.StoreAllData = sam.StoreAllData.Copy();
                    if (sam.StoreAllData == null || sam.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        // return RedirectToAction("SchoolAccreditation", "School");
                        ViewBag.IsFinal = ViewBag.TotalCount = 0;
                    }
                    else
                    {

                        ViewBag.IsFinal = Convert.ToInt32(sacm.sam.StoreAllData.Tables[0].Rows[0]["IsFinal"].ToString());
                        ViewBag.TotalCount = sacm.sam.StoreAllData.Tables[0].Rows.Count;
                    }


                    // Challan Generated
                    if (sam.StoreAllData == null || sacm.sam.StoreAllData.Tables[1].Rows.Count == 0)
                    {
                        ViewBag.IsChallan = ViewBag.TotalCount1 = 0;
                    }
                    else
                    {
                        ViewBag.IsChallan = Convert.ToInt32(sacm.sam.StoreAllData.Tables[1].Rows[0]["ChallanVerify"].ToString());
                        ViewBag.TotalCount1 = sacm.sam.StoreAllData.Tables[1].Rows.Count;
                    }


                    if (sam.StoreAllData == null || sacm.sam.StoreAllData.Tables[2].Rows.Count == 0)
                    {
                        ViewBag.TotalCount2 = 0;
                    }
                    else
                    {

                        ViewBag.TotalCount2 = sacm.sam.StoreAllData.Tables[2].Rows.Count;
                    }

                }

                DataSet result = objDBReg.schooltypes(Session["SCHL"].ToString()); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {

                    ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                    ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
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


                List<SelectListItem> bindClass = new List<SelectListItem>();
                if (ViewBag.MATRICAccreditation == "1" && ViewBag.SeniorAccreditation == "1")
                { bindClass = objCommon.GetAllPSEBCLASS().Where(s => s.Value == "10" || s.Value == "12").ToList(); }
                else if (ViewBag.MATRICAccreditation == "1" && ViewBag.SeniorAccreditation == "0")
                { bindClass = objCommon.GetAllPSEBCLASS().Where(s => s.Value == "10").ToList(); }
                else if (ViewBag.MATRICAccreditation == "0" && ViewBag.SeniorAccreditation == "1")
                { bindClass = objCommon.GetAllPSEBCLASS().Where(s => s.Value == "12").ToList(); }
                ViewBag.ClassList = bindClass;
                #endregion  Get Data 

                return View(sacm);

            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        public ActionResult AccreditationActions(string id, string act)
        {
            try
            {
                string OutResult = "";
                SchoolAccreditationModel sam = new SchoolAccreditationModel();
                if (id == null)
                {
                    return RedirectToAction("SchoolAccreditation", "School");
                }
                else
                {

                    if (act == "D")
                    {

                        sam.id = Convert.ToInt32(id);
                        DataSet dsAdd = objDB.InsertSchoolAccreditation(sam, 1, out OutResult);
                        if (OutResult == "1")
                        {
                            @ViewBag.result = "1";
                            ViewData["Status"] = "DEL";
                        }
                    }
                    else if (act == "FS")// Final Submit
                    {
                        sam.schl = Convert.ToString(id);
                        DataSet dsAdd = objDB.InsertSchoolAccreditation(sam, 2, out OutResult);
                        if (OutResult == "1")
                        {
                            @ViewBag.result = "1";
                            ViewData["Status"] = "FS";
                        }
                    }
                    else if (act == "UF")// Unlock Final Submit
                    {
                        sam.schl = Convert.ToString(id);
                        DataSet dsAdd = objDB.InsertSchoolAccreditation(sam, 3, out OutResult);
                        if (OutResult == "1")
                        {
                            @ViewBag.result = "1";
                            ViewData["Status"] = "UF";
                        }
                    }

                }
                return RedirectToAction("SchoolAccreditation", "School");
            }
            catch (Exception)
            {
                return RedirectToAction("SchoolAccreditation", "School");
            }
        }


        #region Challan and Payment Details Accreditation
        public ActionResult PaymentFormAccreditation(string id)
        {
            try
            {
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                { return RedirectToAction("Index", "Home"); }

                FeeMasterAccreditation _FeeMasterAccreditation = new FeeMasterAccreditation();
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("SchoolAccreditation", "School");
                }
                string Schl = id;
                string today = DateTime.Today.ToString("dd/MM/yyyy");
                //string today = "14/01/2020";
                DateTime dateselected;
                if (DateTime.TryParseExact(today, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
                {
                    ViewData["result"] = 5;
                    DataSet ds = objDB.GetSchoolAccreditationPayment(Schl);
                    _FeeMasterAccreditation.PaymentFormData = ds;
                    if (_FeeMasterAccreditation.PaymentFormData == null || _FeeMasterAccreditation.PaymentFormData.Tables[0].Rows.Count == 0)
                    { ViewBag.TotalCount = 0; Session["FeeMasterAccreditation"] = null; }
                    else
                    {
                        Session["FeeMasterAccreditation"] = ds;
                        ViewBag.TotalNew = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("New")));
                        ViewBag.TotalReNew = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("ReNew")));
                        ViewBag.TotalLateFee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("latefee")));
                        ViewBag.TotalTotfee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("Totfee")));

                        ViewBag.Total = ViewBag.TotalNew + ViewBag.TotalReNew + ViewBag.TotalLateFee;
                        ViewData["result"] = 10;
                        ViewData["FeeStatus"] = "1";
                        ViewBag.TotalCount = 1;
                        return View(_FeeMasterAccreditation);
                    }

                }
                else
                {
                    ViewData["OutError"] = "Date Format Problem";
                }
                return View(_FeeMasterAccreditation);
            }
            catch (Exception)
            {
                return View();
                /// return RedirectToAction("SchoolAccreditation", "School");
            }
        }
        [HttpPost]
        public ActionResult PaymentFormAccreditation(string id, FormCollection frm, string BankAllow, string PayModValue)
        {
            try
            {
                FeeMasterAccreditation pfvm = new FeeMasterAccreditation();
                ChallanMasterModel CM = new ChallanMasterModel();
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                { return RedirectToAction("Index", "Home"); }

                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("SchoolAccreditation", "School");
                }
                if (Session["FeeMasterAccreditation"] == null)
                {
                    return RedirectToAction("SchoolAccreditation", "School");
                }
                string Schl = id;
                DataSet ds = (DataSet)Session["FeeMasterAccreditation"];
                pfvm.PaymentFormData = ds;
                ViewBag.TotalNew = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("New")));
                ViewBag.TotalReNew = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("ReNew")));
                ViewBag.TotalLateFee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("latefee")));
                ViewBag.TotalTotfee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("Totfee")));


                if (string.IsNullOrEmpty(BankAllow))
                {
                    ViewBag.Message = "Please Select Bank";
                    ViewData["SelectBank"] = "1";
                    return View(pfvm);
                }
                string bankName = "";

                string AllowBanks = BankAllow;
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
                }
                else if (AllowBanks == "203")
                {
                    PayModValue = "hod";
                    bankName = "PSEB HOD";
                    CM.FEEMODE = "CASH";
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
                    CM.FEEMODE = "CASH";
                }
                //pfvm.BankName = bankName;
                //

                if (ModelState.IsValid)
                {
                    CM.FEE = Convert.ToInt32(ViewBag.TotalNew) + Convert.ToInt32(ViewBag.TotalReNew);
                    CM.latefee = Convert.ToInt32(ViewBag.TotalLateFee);
                    CM.TOTFEE = Convert.ToInt32(ViewBag.TotalTotfee);
                    CM.FEECAT = ds.Tables[0].Rows[0]["FeeCat"].ToString();
                    CM.FEECODE = ds.Tables[0].Rows[0]["FeeCode"].ToString();
                    //CM.FEEMODE = "CASH";
                    CM.BCODE = BankAllow;
                    CM.BANK = bankName;
                    CM.BANKCHRG = 0;
                    CM.SchoolCode = Schl;
                    CM.DIST = "";
                    CM.DISTNM = "";
                    CM.LOT = 1;
                    CM.SCHLREGID = Schl;
                    CM.FeeStudentList = ds.Tables[0].Rows[0]["Refno"].ToString();
                    CM.APPNO = ds.Tables[0].Rows[0]["Refno"].ToString();

                    CM.type = "schle";
                    CM.CHLNVDATE = Convert.ToString(ds.Tables[0].Rows[0]["BankEndDate"].ToString());
                    DateTime BankLastDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["BankEndDate"].ToString());
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
                                return RedirectToAction("AtomCheckoutUrl", "Gateway", new { ChallanNo = TransactionID, amt = TransactionAmount, clientCode = clientCode, cmn = udf1CustName, cme = udf2CustEmail, cmno = udf3CustMob });

                            }
                            #endregion Payment Gateyway
                        }
                        else
                        {
                            string bnkLastDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["BankEndDate"].ToString()).ToString("dd/MM/yyyy");
                            string Sms = "School Accreditation Challan no. " + result + " of Ref no  " + CM.APPNO + " successfully generated and valid till Dt " + bnkLastDate + ". Regards PSEB";
                            try
                            {
                                string getSms = objCommon.gosms(SchoolMobile, Sms);
                                //string getSms = objCommon.gosms("9711819184", Sms);
                            }
                            catch (Exception) { }

                            ModelState.Clear();
                            //--For Showing Message---------//                   
                            return RedirectToAction("GenerateChallaan", "Home", new { ChallanId = result });
                        }
                    }
                }
                return View(pfvm);
            }
            catch (Exception ex)
            {
                return RedirectToAction("SchoolAccreditation", "school");
            }
        }

        public ActionResult SchoolAccreditationReport(string id)
        {
            try
            {
                SchoolAccreditationCombinedModel sacm = new SchoolAccreditationCombinedModel();
                //SchoolAccreditationModel sam = new SchoolAccreditationModel();
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                { return RedirectToAction("Index", "Home"); }

                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("SchoolAccreditation", "School");
                }

                DataSet ds = new DataSet();
                SchoolModels sm = objDB.GetSchoolDataBySchl(Session["SCHL"].ToString(), out ds);
                sacm.schlmodel = sm;
                DataSet dsAdd = objDB.GetSchoolAccreditationReport(Session["SCHL"].ToString(), id);
                sacm.sam = new SchoolAccreditationModel();
                sacm.sam.StoreAllData = dsAdd.Copy();
                if (dsAdd == null || dsAdd.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = dsAdd.Tables[0].Rows.Count;
                }
                return View(sacm);
            }
            catch (Exception)
            {
                return RedirectToAction("SchoolAccreditation", "School");
            }
        }
        #endregion Challan and Payment Details

        #endregion SchoolAccreditation

        public JsonResult CheckSchoolCodeAndGetAllDetails(string schoolcode, string APPNO)
        {
            EAffiliationExamGroupApplyView eAffiliationExamGroupApplyView = new EAffiliationExamGroupApplyView();
            string outid = "0";
            DataSet ds;
            SchoolModels schoolModels = new AbstractLayer.EAffiliationDB().SelectSchoolDatabyID_For_EAffiliation(schoolcode, APPNO, out ds);    //SelectSchoolDatabyID 
            if (!string.IsNullOrEmpty(schoolModels.SCHL))
            {
                outid = "1";
                if (ds.Tables[1].Rows.Count > 0)
                {
                    var itemSubUType = ds.Tables[1].AsEnumerable().Select(dataRow => new EAffiliationExamGroupApplyView
                    {
                        APPNO = dataRow.Field<string>("APPNO").ToString(),
                        SCHL = dataRow.Field<string>("SCHL").ToString(),
                        NEW_PRIMARY = dataRow.Field<string>("NEW_PRIMARY").ToString(),
                        NEW_MIDDLE = dataRow.Field<string>("NEW_MIDDLE").ToString(),
                        NEW_MATRIC = dataRow.Field<string>("NEW_MATRIC").ToString(),
                        NEW_HUM = dataRow.Field<string>("NEW_HUM").ToString(),
                        NEW_COMM = dataRow.Field<string>("NEW_COMM").ToString(),
                        NEW_SCI = dataRow.Field<string>("NEW_SCI").ToString(),
                    }).ToList();
                    eAffiliationExamGroupApplyView = itemSubUType.Where(s => s.SCHL.ToLower().Trim() == schoolcode.ToLower().Trim()).FirstOrDefault<EAffiliationExamGroupApplyView>();
                }
            }
            return Json(new { sm = schoolModels, eam = eAffiliationExamGroupApplyView, oid = outid }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult JqSendPasswordEmail(string Schl, string Type, string SentTo)
        {
            string outid = "0";
            if (Schl != "")
            {
                DataSet ds = objDB.SelectSchoolDatabyID(Schl);    //SelectSchoolDatabyID 
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        string mobile = ds.Tables[0].Rows[0]["MOBILE"].ToString().Trim();
                        string emailid = ds.Tables[0].Rows[0]["EMAILID"].ToString().Trim();
                        string Password = ds.Tables[0].Rows[0]["PASSWORD"].ToString().Trim();
                        string PRINCIPAL = ds.Tables[0].Rows[0]["PRINCIPAL"].ToString().Trim();
                        if (Type == "1")
                        {
                            string Sms = "Respected " + PRINCIPAL + ", Your School User Id: " + Schl + " and Password: " + Password + " . Kindly change Password after first login for security reason.";
                            //  string Sms = "Your Login details are School Code:: " + Schl + " and Password: " + Password + ". Click to Login Here https://registration2021.pseb.ac.in/Login. Regards PSEB";
                            string getSms = new AbstractLayer.DBClass().gosms(mobile, Sms);
                            if (getSms.ToLower().Contains("success"))
                            {
                                outid = "1";
                            }

                        }
                        else if (Type == "2")
                        {
                            string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Respected " + PRINCIPAL + "</b>,</td></tr><tr><td><b>Your School Login Details are given Below for Punjab School Education Board Web Portal :-</b><br /><b>School Code :</b> " + Schl + "<br /><b>School Name :</b> " + Password + "<br /></td></tr><tr><td><b>Path :</b><a href=https://www.registration.pseb.ac.in target = _blank>www.registration.pseb.ac.in</a><br /><b>UserId :</b> " + Schl + "<br /><b>Password :</b> " + Password + "<br /></td></tr><tr><td><b>Note:</b>Make sure change password after first login for Security reason.</td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /><tr><td><b>Contact Us</b><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 18002700280<br>DISTRICTS:- BARNALA, FATEHGARH SAHIB, GURDASPUR, HOSHIARPUR, JALANDHAR, KAPURTHALA, SHRI MUKTSAR SAHIB, S.B.S. NAGAR, PATHANKOT, PATIALA, SANGRUR, CHANDIGARH &amp; OTHER STATES<br><br><b>Email Id:</b> <a href=mailto:Contact2@psebonline.in target=_blank>contact2@psebonline.in</a><br><b>Toll Free Help Line No. :</b> 18004190690<br>DISTRICTS:- AMRITSAR, BATHINDA, FARIDKOT, FAZILKA, FEROZEPUR, LUDHIANA, MANSA, MOGA, ROOP NAGAR, S.A.S NAGAR,TARN TARAN<br></td></tr>";
                            string subject = "School Login Details for PSEB Portal.";
                            bool result = new AbstractLayer.DBClass().mail(subject, body, emailid);
                            if (result == true)
                            {
                                outid = "1";
                            }

                        }
                    }
                }
            }

            return Json(new { status = outid }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewSchoolhistory(string id, SchoolModels sm)
        {
            if (id != null)
            {
                ViewBag.SCHL = id.ToString();
                sm.StoreAllData = new AbstractLayer.SchoolDB().SelectSchoolDatabyID(id);
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

        public JsonResult CheckSchoolCode(string schoolcode)
        {
            string schoolname = "";
            string districtname = "";
            string outid = "0";
            string verifylogin = "";
            DataSet ds = objDB.SelectSchoolDatabyID(schoolcode);    //SelectSchoolDatabyID 
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    outid = "1";
                    ViewBag.schoolname = schoolname = ds.Tables[0].Rows[0]["SCHLEfull"].ToString();
                    ViewBag.districtname = districtname = ds.Tables[0].Rows[0]["DISTE"].ToString().Trim();
                    verifylogin = ds.Tables[0].Rows[0]["IsVerified"].ToString().Trim();
                }
            }
            return Json(new { sn = schoolname, dn = districtname, vl = verifylogin, oid = outid }, JsonRequestBehavior.AllowGet);
        }


        #region Reg School List of Status Done
        public ActionResult RegSchoolList(SchoolModels asm, int? page)
        {
            try
            {
                if (Session["AdminId"] == null)
                { return RedirectToAction("Index", "Admin"); }
                else
                {
                    // Dist Allowed
                    string DistAllow = "";
                    if (ViewBag.DistAllow == null)
                    { return RedirectToAction("Index", "Admin"); }
                    else
                    { DistAllow = ViewBag.DistAllow; }
                    if (ViewBag.DistUser == null)
                    { ViewBag.MyDist = null; }
                    else
                    {
                        ViewBag.MyDist = ViewBag.DistUser;
                    }

                    #region Action Assign Method
                    if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                    { ViewBag.IsView = 1; }
                    else
                    {

                        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                        string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                        int AdminId = Convert.ToInt32(Session["AdminId"]);
                        //string AdminType = Session["AdminType"].ToString();
                        //GetActionOfSubMenu(string cont, string act)
                        DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                        if (aAct.Tables[0].Rows.Count > 0)
                        {
                            ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("SCHOOL/SCHOOL_VIEW_FORM")).Count();
                        }
                    }
                    #endregion Action Assign Method


                    AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                    ViewBag.MySch = objCommon.SearchSchoolItems();
                    ViewBag.MySchoolType = objCommon.GetSchool();
                    ViewBag.MyClassType = objCommon.GetClass().Where(s => s.Value != "1").ToList();

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;

                    if (ViewBag.MyDist == null)
                    {
                        ModelState.AddModelError("", "District Not Found");
                        return View();
                    }
                    else
                    {
                        ViewBag.SelectedDist = "";
                        ViewBag.SelectedItem = "";
                        ViewBag.SelectedSchoolType = "";
                        ViewBag.SelectedClassType = "";
                        SchoolModels ASM = new SchoolModels();
                        string Search = string.Empty;
                        if (TempData["SearchRegSchoolList"] != null)
                        {
                            Search += TempData["SearchRegSchoolList"].ToString();
                            TempData["SelectedItem"] = ViewBag.SelectedItem = TempData["SelectedItem"];
                            TempData["SelectedClassType"] = ViewBag.SelectedClassType = TempData["SelectedClassType"];
                            TempData["SelectedSchoolType"] = ViewBag.SelectedSchoolType = TempData["SelectedSchoolType"];
                            TempData["SelectedDist"] = ViewBag.SelectedDist = TempData["SelectedDist"];

                            ASM.StoreAllData = objDB.RegSchoolList(Search, pageIndex, 20);//RegSchoolListSP
                            if (ASM.StoreAllData != null)
                            {
                                ViewBag.TotalCount = ASM.StoreAllData.Tables[0].Rows.Count;
                                int count = Convert.ToInt32(ASM.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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
                            else
                            {
                                ViewBag.LastPageIndex = 0;
                                ViewBag.TotalCount = 0;
                                ViewBag.TotalCount1 = 0;
                            }
                        }
                        else
                        {
                            Search = "sm.Id like '%' and sm.status='Done' and sm.Class!='1' ";
                            if (DistAllow != "")
                            {
                                Search += " and sm.DIST in (" + DistAllow + ")";
                            }
                            TempData["SearchRegSchoolList"] = Search;
                            ASM.StoreAllData = objDB.RegSchoolList(Search, pageIndex, 20);//RegSchoolListSP
                            if (ASM.StoreAllData != null)
                            {
                                ViewBag.TotalCount = ASM.StoreAllData.Tables[0].Rows.Count;
                                int count = Convert.ToInt32(ASM.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                                ViewBag.TotalCount1 = count;
                                int tp = Convert.ToInt32(count);
                                int pn = tp / 20;
                                int cal = 20 * pn;
                                int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                                if (res >= 1)
                                    ViewBag.pn = pn + 1;
                                else
                                    ViewBag.pn = pn;
                                return View(ASM);

                            }
                            else
                            {
                                ViewBag.TotalCount = 0;
                                ViewBag.TotalCount1 = 0;
                            }

                        }
                        return View(ASM);

                    }

                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [HttpPost]
        public ActionResult RegSchoolList(FormCollection frm, int? page)
        {
            try
            {

                // Dist Allowed
                string DistAllow = "";
                if (ViewBag.DistAllow == null)
                { return RedirectToAction("Index", "Admin"); }
                else
                { DistAllow = ViewBag.DistAllow; }
                if (ViewBag.DistUser == null)
                { ViewBag.MyDist = null; }
                else
                {
                    ViewBag.MyDist = ViewBag.DistUser;
                }
                // End Dist Allowed

                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsView = 1; }
                else
                {

                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    //string AdminType = Session["AdminType"].ToString();
                    //GetActionOfSubMenu(string cont, string act)
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("SCHOOL/SCHOOL_VIEW_FORM")).Count();
                    }
                }
                #endregion Action Assign Method

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                SchoolModels ASM = new SchoolModels();
                if (ModelState.IsValid)
                {
                    ViewBag.MySchoolType = objCommon.GetSchool();
                    ViewBag.MyClassType = objCommon.GetClass().Where(s => s.Value != "1").ToList();
                    // bind Dist 
                    ////  ViewBag.MyDist = objCommon.GetDistE();
                    ViewBag.MySch = objCommon.SearchSchoolItems();
                    string Search = string.Empty;
                    Search = "sm.Id like '%' and sm.status='Done' and sm.Class!='1' ";
                    if (frm["Dist1"] != "")
                    {
                        ViewBag.SelectedDist = frm["Dist1"];
                        TempData["SelectedDist"] = frm["Dist1"];
                        Search += " and sm.dist=" + frm["Dist1"].ToString();
                    }

                    if (frm["SchoolType"] != "")
                    {
                        ViewBag.SelectedSchoolType = frm["SchoolType"];
                        TempData["SelectedSchoolType"] = frm["SchoolType"];
                        Search += " and st.schooltype='" + frm["SchoolType"].ToString() + "'";
                    }

                    if (frm["ClassType"] != "")
                    {
                        ViewBag.SelectedClassType = frm["ClassType"];
                        TempData["SelectedClassType"] = frm["ClassType"];
                        Search += " and sm.class=" + frm["ClassType"].ToString();
                    }

                    if (frm["Sch1"] != "")
                    {
                        ViewBag.SelectedItem = frm["Sch1"];
                        TempData["SelectedItem"] = frm["Sch1"];
                        int SelValueSch = Convert.ToInt32(frm["Sch1"].ToString());

                        if (frm["SearchString"] != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and sm.SCHL=" + frm["SearchString"].ToString(); }
                            else if (SelValueSch == 2)
                            { Search += " and  sm.SCHLE like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 3)
                            { Search += " and sm.IDNO='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 4)
                            { Search += " and sm.STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 5)
                            { Search += " and sm.SCHLE=" + frm["SearchString"].ToString(); }
                        }

                    }
                    ViewBag.PreviousPageIndex = 0;
                    ViewBag.CurrentPageIndex = FirstPageIndex;
                    TempData["SearchRegSchoolList"] = Search;
                    TempData.Keep(); // to store search value for view
                    if (DistAllow != "")
                    {
                        Search += " and sm.DIST in (" + DistAllow + ")";
                    }
                    ASM.StoreAllData = objDB.RegSchoolList(Search, pageIndex, 20);//RegSchoolListSP
                    if (ASM.StoreAllData != null)
                    {
                        ViewBag.TotalCount = ASM.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(ASM.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.TotalCount1 = 0;
                    }
                }
                return View(ASM);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }


        #endregion

        #region Book Demand
        public ActionResult BookAssessmentForm(BookAssessmentForm rft)
        {
            try
            {

                //if (rft.BookRequestList.Count > 0)
                //{
                //    rft.BookRequestList.Clear();
                //}
                string schl = string.Empty;
                if (Session["SCHL"] != null && Session["SCHL"].ToString() != string.Empty)
                {
                    schl = Session["SCHL"].ToString();
                }
                else
                {
                    return RedirectToAction("Logout", "Login");
                }
                DataSet ds = objDB.SelectSchoolDatabyID(schl);
                rft.StoreAllData = ds;
                ViewBag.schoolname = rft.SCHL = ds.Tables[0].Rows[0]["SCHLEfull"].ToString();
                ViewBag.PrincipalName = rft.PrincipalName = ds.Tables[0].Rows[0]["Principal"].ToString().Trim();
                ViewBag.SchoolMobile = rft.SchoolMobile = ds.Tables[0].Rows[0]["Mobile"].ToString();
                ViewBag.schl = rft.SCHL = schl;
                //GetAllBookClass
                ViewBag.Class = objDB.GetAllBookClass();
                ViewBag.TotalCount = 0;
                return View(rft);
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult BookAssessmentForm(BookAssessmentForm rft, string id, string cmd, FormCollection frm)
        {
            if (Session["SCHL"] == null || Session["SCHL"].ToString().Trim() == string.Empty)
            {
                return RedirectToAction("Logout", "Login");
            }
            ////
            var skipped = ModelState.Keys.Where(key => key.StartsWith("BookRequestList")).ToList();
            foreach (var key in skipped)
            { ModelState.Remove(key); }
            ////


            string schl = Session["SCHL"].ToString();
            DataSet ds = objDB.SelectSchoolDatabyID(schl);
            ViewBag.schoolname = rft.SCHL = ds.Tables[0].Rows[0]["SCHLEfull"].ToString();
            ViewBag.PrincipalName = rft.PrincipalName = ds.Tables[0].Rows[0]["Principal"].ToString().Trim();
            ViewBag.SchoolMobile = rft.SchoolMobile = ds.Tables[0].Rows[0]["Mobile"].ToString();
            ViewBag.schl = rft.SCHL = schl;
            //GetAllBookClass
            ViewBag.Class = objDB.GetAllBookClass();

            ViewBag.TotalCount = 0;
            string class1 = "";
            if (frm["Class"] != null)
            {
                class1 = frm["Class"].ToString();
            }


            if ((rft.SCHL == "" || rft.SCHL == null) && (rft.TOT_STUD.ToString() == ""))
            {
                ViewData["result"] = "10";

            }
            else if (cmd.ToUpper().Contains("SAVE"))
            {
                if (rft.BookRequestList == null || rft.BookRequestList.Count == 0)
                {
                    ViewData["result"] = "11";
                    return View(rft);
                }
                else
                {
                    rft.Class = Convert.ToInt32(class1);
                    rft.TOT_STUD = Convert.ToInt32(rft.TOT_STUD);
                    DataTable dt = AbstractLayer.StaticDB.ConvertListToDataTable<BookRequest>(rft.BookRequestList);
                    if (dt == null || dt.Rows.Count == 0)
                    { }
                    else
                    {
                        var duplicates = dt.AsEnumerable().GroupBy(r => r[0]).Where(gr => gr.Count() > 1).ToList();
                        if (duplicates.Any())
                        {
                            ViewBag.Duplicate = "Duplicate : " + String.Join(", ", duplicates.Select(dupl => dupl.Key));
                            ViewData["result"] = "20";
                            return View(rft);
                        }

                        dt.AcceptChanges();
                        // dt = dt.AsEnumerable().Where(r => r.ItemArray[3].ToString() != "" && r.ItemArray[3].ToString() != "0").CopyToDataTable();
                        var rows = dt.AsEnumerable().Where(r => r.ItemArray[3].ToString() != "" && r.ItemArray[3].ToString() != "0"); // optionally include .ToList();
                        dt = rows.Any() ? rows.CopyToDataTable() : dt.Clone();

                        if (dt.Columns.Contains("Flag"))
                        { dt.Columns.Remove("Flag"); }


                        dt.AcceptChanges();
                        rft.BookRequestDT = dt;
                    }
                }

                string res = "";
                string OutError = "0";
                rft.id = 0;
                if (id == null)
                {
                    res = objDB.BookAssessmentForm(rft, 0, out OutError);

                }

                if (Convert.ToInt32(OutError) > 1)
                {
                    ViewData["result"] = "1";
                    rft.BookRequestList = null;
                    ViewBag.TotalCount = 0;
                    ModelState.Clear();
                    return View();
                }

                else
                {
                    ViewData["result"] = "0";
                }

            }
            else if (cmd.ToUpper().Contains("GET"))
            {
                // ModelState.Remove("BookRequestList");
                rft.BookRequestList = null;
                ViewBag.SelectedClass = class1;
                ViewBag.TOT_STUD = rft.TOT_STUD;
                string Search = "";
                Search = "BOOKID like '%%' and Class= " + class1 + "";
                List<BookRequest> _BookRequest = new List<BookRequest>();
                string OutError = "0";
                DataSet dsBD1 = objDB.BookDemand(6, Search, schl, out OutError);
                rft.StoreAllData = dsBD1;
                ViewBag.TotalCount = 0;
                if (dsBD1.Tables.Count > 0)
                {
                    if (dsBD1.Tables[0].Rows.Count > 0)
                    {
                        foreach (System.Data.DataRow dr in dsBD1.Tables[0].Rows)
                        {
                            _BookRequest.Add(new BookRequest { BookId = Convert.ToInt32(@dr["BookId"]), BookNM = @dr["BOOK_NM"].ToString(), NOS = @dr["TOT_BOOK"].ToString(), Class = Convert.ToInt32(@dr["class"]) });
                        }
                        rft.BookRequestList = _BookRequest;
                        ViewBag.TotalCount = dsBD1.Tables[0].Rows.Count;
                    }
                    else
                    {
                        DataSet dsBD = objDB.BookDemand(2, Search, "", out OutError);
                        rft.StoreAllData = dsBD;
                        if (dsBD.Tables.Count > 0)
                        {
                            if (dsBD.Tables[0].Rows.Count > 0)
                            {
                                foreach (System.Data.DataRow dr in dsBD.Tables[0].Rows)
                                {
                                    _BookRequest.Add(new BookRequest { BookId = Convert.ToInt32(@dr["BookId"]), BookNM = @dr["BOOK_NM"].ToString(), NOS = @dr["NOS"].ToString(), Class = Convert.ToInt32(@dr["class"]) });
                                }
                                rft.BookRequestList = _BookRequest;
                                ViewBag.TotalCount = dsBD.Tables[0].Rows.Count;

                            }
                        }
                    }
                }
            }
            else if (cmd.ToUpper().Contains("VIEW"))
            {
                string Search = "";
                Search = "BOOKID like '%%' and schl='" + schl + "'";
                if (class1 != "")
                {
                    Search += " and Class= " + class1 + "";
                    ViewBag.SelectedClass = class1;
                }
                List<BookRequest> _BookRequest = new List<BookRequest>();
                string OutError = "0";
                DataSet dsBD = objDB.BookDemand(5, Search, schl, out OutError);
                rft.StoreAllData = dsBD;

                if (dsBD.Tables.Count > 0)
                {
                    if (dsBD.Tables[0].Rows.Count > 0)
                    {
                        foreach (System.Data.DataRow dr in dsBD.Tables[0].Rows)
                        {
                            _BookRequest.Add(new BookRequest { BookId = Convert.ToInt32(@dr["BookId"]), BookNM = @dr["BOOK_NM"].ToString(), NOS = @dr["TOT_BOOK"].ToString(), Class = Convert.ToInt32(@dr["class"]), Flag = @dr["Flag"].ToString() });
                        }
                        rft.BookRequestList = _BookRequest;
                        ViewBag.TotalCount1 = dsBD.Tables[0].Rows.Count;
                        ViewBag.TotalCount = 0;
                    }
                }
                else
                {
                    ViewBag.TotalCount = 0;
                    ViewBag.TotalCount1 = 0;
                }
            }
            return View(rft);
        }


        public ActionResult ViewBookAssessmentForm(BookAssessmentForm rft, string id, string cmd, FormCollection frm)
        {
            try
            {
                //GetAllBookClass
                ViewBag.Class = objDB.GetAllBookClass();

                string schl = string.Empty;
                if (Session["SCHL"] != null && Session["SCHL"].ToString() != string.Empty)
                {
                    schl = Session["SCHL"].ToString();
                }
                else
                {
                    return RedirectToAction("Logout", "Login");
                }

                ViewBag.Id = id;
                string class1 = "";
                if (frm["Class"] != null)
                {
                    class1 = frm["Class"].ToString();
                }


                int Printlot = 0;
                if (Request.QueryString["Printlot"] == null || Request.QueryString["Printlot"] == "")
                { }
                else
                {
                    Printlot = Convert.ToInt32(Request.QueryString["Printlot"]);
                }



                DataSet ds = objDB.SelectSchoolDatabyID(schl);
                rft.StoreAllData = ds;
                ViewBag.schoolname = rft.SCHL = ds.Tables[0].Rows[0]["SCHLEfull"].ToString();
                ViewBag.schoolnameP = rft.SCHL = ds.Tables[0].Rows[0]["SCHLPfull"].ToString();
                ViewBag.PrincipalName = rft.PrincipalName = ds.Tables[0].Rows[0]["Principal"].ToString().Trim();
                ViewBag.SchoolMobile = rft.SchoolMobile = ds.Tables[0].Rows[0]["Mobile"].ToString();
                ViewBag.Email = ds.Tables[0].Rows[0]["EMAILID"].ToString().Trim();
                ViewBag.Phone = ds.Tables[0].Rows[0]["STDCODE"].ToString() + " " + ds.Tables[0].Rows[0]["PHONE"].ToString();
                ViewBag.Address = ds.Tables[0].Rows[0]["ADDRESSEfull"].ToString().Trim();
                ViewBag.IDNO = ds.Tables[0].Rows[0]["IDNO"].ToString();

                ViewBag.schl = rft.SCHL = schl;
                //GetAllBookClass
                ViewBag.Class = objDB.GetAllBookClass();


                //
                string Search = "";
                string OutError = "0";
                Search = "BOOKID like '%%' and schl='" + schl + "'";
                ViewBag.SelectedClass = "0";
                if (class1 != "")
                {
                    Search += " and Class= " + class1 + "";
                    ViewBag.SelectedClass = class1;
                }

                DataSet dsBD;
                if (id.ToUpper() == "ROUGHPRINT")
                { dsBD = objDB.BookDemand(5, Search, schl, out OutError); }
                else
                {
                    if (Printlot == 0) { dsBD = objDB.BookDemand(3, Search, schl, out OutError); }
                    else
                    {
                        Search += " and lot= " + Printlot + "";
                        dsBD = objDB.BookDemand(7, Search, schl, out OutError);
                    }

                }


                if (Printlot == 0)
                {
                    ViewBag.IsPrint = "1";
                    ViewBag.PrintOn = "";
                }

                //  DataSet dsBD = objDB.BookDemand(3, Search, schl, out OutError);
                rft.StoreAllData = dsBD;
                List<BookRequest> _BookRequest = new List<BookRequest>();
                if (dsBD.Tables.Count > 0)
                {
                    if (dsBD.Tables[0].Rows.Count > 0)
                    {
                        foreach (System.Data.DataRow dr in dsBD.Tables[0].Rows)
                        {
                            _BookRequest.Add(new BookRequest { BookId = Convert.ToInt32(@dr["BookId"]), BookNM = @dr["BOOK_NM"].ToString(), NOS = @dr["TOT_BOOK"].ToString(), Class = Convert.ToInt32(@dr["class"]) });
                        }
                        rft.BookRequestList = _BookRequest;
                        ViewBag.TotalCount = dsBD.Tables[0].Rows.Count;
                        if (class1 != "" && Printlot == 0)
                        {
                            ViewBag.IsPrint = dsBD.Tables[0].Rows[0]["Flag"].ToString();
                            ViewBag.PrintOn = dsBD.Tables[0].Rows[0]["PrintOn"].ToString();

                        }

                        if (Printlot > 0)
                        {
                            ViewBag.IsPrint = dsBD.Tables[0].Rows[0]["Flag"].ToString();
                            ViewBag.PrintOn = dsBD.Tables[0].Rows[0]["PrintOn"].ToString();
                        }
                    }
                    if (dsBD.Tables[1].Rows.Count > 0)
                    {
                        ViewBag.Summary = dsBD.Tables[1].Rows[0]["Summary"].ToString();
                    }
                }


                return View(rft);
            }
            catch (Exception)
            {
                return View();
            }
        }

        public JsonResult JqUpdatePrint(string storeid, string class1, string Action)
        {
            int schl = Convert.ToInt32(Session["Schl"].ToString());
            string OutError = "0";
            string Search = "";
            Search = "schl='" + storeid + "'";
            if (Convert.ToInt32(class1) > 0)
            {
                Search += " and Class= " + class1 + "";
            }

            DataSet dsBD = objDB.BookDemand(4, Search, storeid, out OutError);
            if (OutError == "1")
            {
                OutError = "1";
            }
            else
            { OutError = "0"; }

            return Json(new { dee = OutError }, JsonRequestBehavior.AllowGet);
        }

        #endregion Book Demand


        #region School Grievance Management System for Support

        public ActionResult RegistrationForTraining(RegistrationForTraining rft)
        {
            try
            {
                string schl = string.Empty;
                if (Session["SCHL"] != null && Session["SCHL"].ToString() != string.Empty)
                {
                    schl = Session["SCHL"].ToString();
                }
                else
                {
                    return RedirectToAction("Logout", "Login");
                }
                DataSet ds = objDB.SelectSchoolDatabyID(schl);
                rft.StoreAllData = ds;
                ViewBag.schoolname = rft.schl = ds.Tables[0].Rows[0]["SCHLEfull"].ToString();
                ViewBag.PrincipalName = rft.PrincipalName = ds.Tables[0].Rows[0]["Principal"].ToString().Trim();
                ViewBag.SchoolMobile = rft.SchoolMobile = ds.Tables[0].Rows[0]["Mobile"].ToString();
                ViewBag.schl = rft.schl = schl;
                if (ds.Tables[2].Rows.Count > 0)
                {
                    ViewBag.TotalCount = ds.Tables[2].Rows.Count;
                    //ViewBag.SchoolRepresentative = rft.SchoolRepresentative = ds.Tables[2].Rows[0]["Name"].ToString();
                    //rft.Designation = ds.Tables[2].Rows[0]["Designation"].ToString().Trim();
                    //rft.cpmobile = ds.Tables[2].Rows[0]["Mobile"].ToString();
                    //rft.cpemail = ds.Tables[2].Rows[0]["Email"].ToString();
                }
                else
                { ViewBag.TotalCount = 0; }
                return View(rft);
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult RegistrationForTraining(RegistrationForTraining rft, int? id, string cmd)
        {
            if (Session["SCHL"] == null || Session["SCHL"].ToString().Trim() == string.Empty)
            {
                return RedirectToAction("Logout", "Login");
            }
            string schl = Session["SCHL"].ToString();
            DataSet ds = objDB.SelectSchoolDatabyID(schl);
            ViewBag.schoolname = rft.schl = ds.Tables[0].Rows[0]["SCHLEfull"].ToString();
            ViewBag.PrincipalName = rft.PrincipalName = ds.Tables[0].Rows[0]["Principal"].ToString().Trim();
            ViewBag.SchoolMobile = rft.SchoolMobile = ds.Tables[0].Rows[0]["Mobile"].ToString();
            ViewBag.schl = rft.schl = schl;

            if ((rft.SchoolRepresentative == "" || rft.SchoolRepresentative == null) && (rft.cpmobile == "" || rft.cpmobile == null) &&
                (rft.cpemail == "" || rft.cpemail == null) && (rft.Designation == "" || rft.Designation == null))
            { ViewData["result"] = "10"; }
            else
            {
                string OutError = "0";
                string res = objDB.RegistrationForTraining(rft, out OutError);
                if (OutError == "1")
                {
                    ModelState.Clear();
                    ViewData["result"] = "1";
                }
                else
                {
                    ViewData["result"] = "0";
                }
            }
            return View(rft);
        }



        public ActionResult Grievances()
        {
            string schl = string.Empty;
            if (Session["SCHL"] != null && Session["SCHL"].ToString() != string.Empty)
            {
                schl = Session["SCHL"].ToString();
                try
                {
                    DataSet ds = objDB.SelectSchoolDatabyID(schl);
                    string principal = ds.Tables[0].Rows[0]["PRINCIPAL"].ToString();
                    ViewBag.PRINCIPAL = string.IsNullOrEmpty(principal) ? string.Empty : principal;
                }
                catch (Exception e) { }
            }
            else
            {
                return RedirectToAction("Logout", "Login");
            }
            return View();
        }

        public ActionResult PostGrievance()
        {
            string schl = string.Empty;
            if (Session["SCHL"] != null && Session["SCHL"].ToString() != string.Empty)
            {
                schl = Session["SCHL"].ToString();
            }
            else
            {
                return RedirectToAction("Logout", "Login");
            }
            List<SelectListItem> forms = objDB.FormsList(0);
            ViewBag.forms = forms;
            string schoolname = "";
            string districtname = "";
            string outid = "";
            string verifylogin = "";
            objDB.pro_checkloginstatUsercodeDist(schl, out schoolname, out districtname, out verifylogin, out outid);
            List<SelectListItem> classes = objDB.GetClasses(schl);
            ViewBag.schoolname = schoolname;
            ViewBag.districtname = districtname;
            ViewBag.schl = schl;
            ViewBag.classes = classes;
            DataSet ds = objCommon.Fll_Dist_Details();
            District objDis = new District();// create the object of class Employee 
            List<District> disList = new List<District>();
            // int table = Convert.ToInt32(ds.Tables.Count);// count the number of table in dataset
            int table = Convert.ToInt32(1);
            //foreach (DataRow dr in ds.Tables[0].Rows)
            //{
            //    disList.Add(new District { DIST = Convert.ToInt32(dr["DIST"]), DISTNM = Convert.ToString(dr["DISTNM"]) });
            //}

            //ViewBag.district1 = disList.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult PostGrievance(CallCenter obj, int? id, string cmd, string remarksname, IEnumerable<HttpPostedFileBase> files)
        {
            if (Session["SCHL"] == null || Session["SCHL"].ToString().Trim() == string.Empty)
            {
                return RedirectToAction("Logout", "Login");
            }
            string schl = Session["SCHL"].ToString();
            DataSet ds = objCommon.Fll_Dist_Details();
            District objDis = new District();// create the object of class Employee 
            List<District> disList = new List<District>();
            int table = Convert.ToInt32(ds.Tables.Count);// count the number of table in dataset
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                disList.Add(new District { DIST = Convert.ToInt32(dr["DIST"]), DISTNM = Convert.ToString(dr["DISTNM"]) });
            }
            ViewBag.district1 = disList.ToList();
            string schoolname = "";
            string districtname = "";
            string outid = "";
            string verifylogin = "";
            string dt = DateTime.Now.ToString("dd/MMM/yyyy hh:MM:ss:ff").Replace(' ', '_').Replace('/', '_').Replace('.', '_').Replace(':', '_');


            objDB.pro_checkloginstatUsercodeDist(schl, out schoolname, out districtname, out verifylogin, out outid);
            obj.schoolcode = schl;
            obj.schoolname = schoolname;
            obj.district = districtname;
            obj.CreatedBy = schl;
            obj.dist = Convert.ToInt32(disList.Find(f => f.DISTNM.ToUpper().Trim() == districtname.ToUpper().Trim()).DIST.ToString());
            ViewBag.schoolname = schoolname;
            ViewBag.districtname = districtname;
            ViewBag.schl = schl;
            if (string.IsNullOrEmpty(schl))
            {
                List<SelectListItem> classes = objDB.GetClasses("9999999");
                ViewBag.classes = classes;
            }
            else
            {
                List<SelectListItem> classes = objDB.GetClasses(schl);
                ViewBag.classes = classes;
            }

            obj.schoolcode = "00" + schl;
            obj.schoolcode = obj.schoolcode.Substring(obj.schoolcode.Length - 7, 7);
            if (string.IsNullOrEmpty(obj.classname))
            {
                List<SelectListItem> forms = objDB.FormsList(0);
                ViewBag.forms = forms;
            }
            else
            {
                List<SelectListItem> forms = new List<SelectListItem>();
                try { /*forms = objDB.FormsList(Convert.ToInt32(obj.classname));*/  forms = objDB.FormsList(0); }
                catch { forms = objDB.FormsList(0); }
                ViewBag.forms = forms;
            }
            var errors = ModelState.Values.SelectMany(e => e.Errors);
            //if (ModelState.IsValid)
            //{
            if (id != null)
            {
                obj.ccfid = Convert.ToInt32(id);
                obj.UpdatedBy = Convert.ToString(Session["SCHL"]);
                obj.remarks = remarksname;
            }
            else
            {
                obj.CreatedBy = Convert.ToString(Session["SCHL"]);
            }
            string OutTicket = "";
            obj.district = disList.Find(f => f.DISTNM.ToUpper().Contains(obj.district.ToUpper().Trim())).DIST.ToString();
            obj.district = "0" + obj.district;
            obj.district = obj.district.Substring(obj.district.Length - 3, 3);

            // Multiple Files
            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg" };
            string SelFiles = "";
            if (files != null)
            {
                if (files.Count() > 0)
                {
                    if (files.Count() <= 3)
                    {
                        foreach (var file in files)
                        {
                            if (file != null && file.ContentLength > 0)
                            {
                                var extension = Path.GetExtension(file.FileName);
                                if (!allowedExtensions.Contains(extension))
                                {
                                    // Not allowed
                                    ViewData["Result"] = "20";
                                    return View(obj);
                                }
                                else if (file.ContentLength > 50000 || file.ContentLength < 5000)
                                {
                                    ViewData["Result"] = "20";
                                    return View(obj);
                                }
                                else
                                {
                                    // string dt1 = DateTime.Now.ToString("dd/MM/yyyy hh:MM:ss").Replace(' ', '_').Replace('/', '_').Replace('.', '_').Replace(':', '_');
                                    //var path = Path.Combine(Server.MapPath("~/Upload/Grievances/Images/"), schl.ToString() + "_" + dt1 + "_" + Path.GetFileNameWithoutExtension(file.FileName).ToUpper() + Path.GetExtension(file.FileName));
                                    //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Grievances/Images"));
                                    //if (!Directory.Exists(FilepathExist))
                                    //{
                                    //    Directory.CreateDirectory(FilepathExist);
                                    //}
                                    //file.SaveAs(path);

                                    string fileName = "Grievances/Images/" + schl.ToString() + "_" + Path.GetFileNameWithoutExtension(file.FileName).ToUpper() + "_G" + Path.GetExtension(file.FileName);
                                    SelFiles += fileName + ",";
                                }
                            }
                        }
                    }
                    else
                    {
                        ViewData["Result"] = 10;
                        return View(obj);
                    }
                }
            }

            if (SelFiles != "")
            {
                SelFiles = SelFiles.Remove(SelFiles.LastIndexOf(","), 1);
            }
            obj.pdf = SelFiles.ToString();
            // end


            string res = objDB.insertUpdateCallCenterForm(obj, out OutTicket);
            if (res == "2")
            {
                ModelState.Clear();
                ViewBag.Message = "Data Saved Successfully.... Your Grievance Number is " + OutTicket;
                foreach (var file in files)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        //var path = Path.Combine(Server.MapPath("~/Upload/Grievances/Images"), schl.ToString() + "_" + Path.GetFileNameWithoutExtension(file.FileName).ToUpper() + "_G" + Path.GetExtension(file.FileName));
                        //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Grievances/Images"));
                        //if (!Directory.Exists(FilepathExist))
                        //{
                        //    Directory.CreateDirectory(FilepathExist);
                        //}
                        //file.SaveAs(path);
                        string Orgfile = schl.ToString() + "_G" + Path.GetExtension(file.FileName);

                        using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                        {
                            using (var newMemoryStream = new MemoryStream())
                            {
                                var uploadRequest = new TransferUtilityUploadRequest
                                {
                                    InputStream = file.InputStream,
                                    Key = string.Format("allfiles/Upload2023/Grievances/Images/{0}", Orgfile),
                                    BucketName = BUCKET_NAME,
                                    CannedACL = S3CannedACL.PublicRead
                                };

                                var fileTransferUtility = new TransferUtility(client);
                                fileTransferUtility.Upload(uploadRequest);
                            }
                        }

                        ////string fileName = "Grievances/Images/" + schl.ToString() + "_" + dt1 + "_G" +  Path.GetExtension(file.FileName));
                        ////SelFiles += fileName + ",";
                    }
                }

            }
            else if (res == "1")
            {
                ModelState.Clear();
                ViewBag.Message = "Data Updated Successfully Of Grievance Number : " + OutTicket;
            }
            else if (res == "3")
            {
                ViewBag.Message = "School Code is Invalid.";
            }
            ViewBag.OutTicket = OutTicket;
            ViewData["result"] = res;
            //}
            return View();
        }


        //[HttpPost]
        //public ActionResult PostGrievance(CallCenter obj, int? id, string cmd, string remarksname, HttpPostedFileBase Gri_pdf, HttpPostedFileBase Gri_Img)
        //{
        //    if (Session["SCHL"] == null || Session["SCHL"].ToString().Trim() == string.Empty)
        //    {
        //        return RedirectToAction("Logout", "Login");
        //    }
        //    string schl = Session["SCHL"].ToString();
        //    DataSet ds = objCommon.Fll_Dist_Details();
        //    District objDis = new District();// create the object of class Employee 
        //    List<District> disList = new List<District>();
        //    int table = Convert.ToInt32(ds.Tables.Count);// count the number of table in dataset
        //    foreach (DataRow dr in ds.Tables[0].Rows)
        //    {
        //        disList.Add(new District { DIST = Convert.ToInt32(dr["DIST"]), DISTNM = Convert.ToString(dr["DISTNM"]) });
        //    }
        //    ViewBag.district1 = disList.ToList();
        //    string schoolname = "";
        //    string districtname = "";
        //    string outid = "";
        //    string verifylogin = "";
        //    string dt = DateTime.Now.ToString("dd/MMM/yyyy hh:MM:ss:ff").Replace(' ', '_').Replace('/', '_').Replace('.', '_').Replace(':', '_');
        //    if (Gri_Img != null && (Gri_Img.ContentType == "image/jpeg" || Gri_Img.ContentType == "image/jpg"))
        //    {
        //        var path = Path.Combine(Server.MapPath("~/Upload/Grievances/Images/") + schl.ToString() + "_" + dt + "_I.jpg");
        //        string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Grievances/Images"));
        //        if (!Directory.Exists(FilepathExist))
        //        {
        //            Directory.CreateDirectory(FilepathExist);
        //        }
        //        Gri_Img.SaveAs(path);
        //        obj.photo = "Grievances/Images/" + schl.ToString() + "_" + dt + "_I.jpg";
        //    }
        //    if (Gri_pdf != null && Gri_pdf.ContentType == "application/pdf")
        //    {
        //        var path = Path.Combine(Server.MapPath("~/Upload/Grievances/PDF/") + schl.ToString() + "_" + dt + "_P.pdf");
        //        string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Grievances/PDF"));
        //        if (!Directory.Exists(FilepathExist))
        //        {
        //            Directory.CreateDirectory(FilepathExist);
        //        }
        //        Gri_pdf.SaveAs(path);
        //        obj.pdf = "Grievances/PDF/" + schl.ToString() + "_" + dt + "_P.jpg";
        //    }

        //    objDB.pro_checkloginstatUsercodeDist(schl, out schoolname, out districtname, out verifylogin, out outid);
        //    obj.schoolcode = schl;
        //    obj.schoolname = schoolname;
        //    obj.district = districtname;
        //    obj.CreatedBy = schl;
        //    obj.dist = Convert.ToInt32(disList.Find(f => f.DISTNM.ToUpper().Trim() == districtname.ToUpper().Trim()).DIST.ToString());
        //    ViewBag.schoolname = schoolname;
        //    ViewBag.districtname = districtname;
        //    ViewBag.schl = schl;
        //    if (string.IsNullOrEmpty(schl))
        //    {
        //        List<SelectListItem> classes = objDB.GetClasses("9999999");
        //        ViewBag.classes = classes;
        //    }
        //    else
        //    {
        //        List<SelectListItem> classes = objDB.GetClasses(schl);
        //        ViewBag.classes = classes;
        //    }

        //    obj.schoolcode = "00" + schl;
        //    obj.schoolcode = obj.schoolcode.Substring(obj.schoolcode.Length - 7, 7);
        //    if (string.IsNullOrEmpty(obj.classname))
        //    {
        //        List<SelectListItem> forms = objDB.FormsList(0);
        //        ViewBag.forms = forms;
        //    }
        //    else
        //    {
        //        List<SelectListItem> forms = new List<SelectListItem>();
        //        try { /*forms = objDB.FormsList(Convert.ToInt32(obj.classname));*/  forms = objDB.FormsList(0); }
        //        catch { forms = objDB.FormsList(0); }
        //        ViewBag.forms = forms;
        //    }
        //    var errors = ModelState.Values.SelectMany(e => e.Errors);
        //    //if (ModelState.IsValid)
        //    //{
        //    if (id != null)
        //    {
        //        obj.ccfid = Convert.ToInt32(id);
        //        obj.UpdatedBy = Convert.ToString(Session["SCHL"]);
        //        obj.remarks = remarksname;
        //    }
        //    else
        //    {
        //        obj.CreatedBy = Convert.ToString(Session["SCHL"]);
        //    }
        //    string OutTicket = "";
        //    obj.district = disList.Find(f => f.DISTNM.ToUpper().Contains(obj.district.ToUpper().Trim())).DIST.ToString();
        //    obj.district = "0" + obj.district;
        //    obj.district = obj.district.Substring(obj.district.Length - 3, 3);
        //    string res = objDB.insertUpdateCallCenterForm(obj, out OutTicket);
        //    if (res == "2")
        //    {
        //        ModelState.Clear();
        //        ViewBag.Message = "Data Saved Successfully.... Your Grievance Number is " + OutTicket;
        //    }
        //    else if (res == "1")
        //    {
        //        ModelState.Clear();
        //        ViewBag.Message = "Data Updated Successfully Of Grievance Number : " + OutTicket;
        //    }
        //    else if (res == "3")
        //    {
        //        ViewBag.Message = "School Code is Invalid.";
        //    }
        //    ViewBag.OutTicket = OutTicket;
        //    ViewData["result"] = res;
        //    //}
        //    return View();
        //}

        public JsonResult GetForms(string clas)
        {
            List<SelectListItem> forms = objDB.FormsList(Convert.ToInt32(clas));
            return Json(forms, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DisplayGrievances(int? page, string command = "")
        {
            string schl = string.Empty;
            if (Session["SCHL"] != null && Session["SCHL"].ToString() != string.Empty)
            {
                schl = Session["SCHL"].ToString();
            }
            else
            {
                return RedirectToAction("Logout", "Login");
            }
            List<District> disList = new List<District>();
            DataSet ds = objCommon.Fll_Dist_Details();
            int table = Convert.ToInt32(ds.Tables.Count);// count the number of table in dataset
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                disList.Add(new District { DIST = Convert.ToInt32(dr["DIST"]), DISTNM = Convert.ToString(dr["DISTNM"]) });
            }
            ViewBag.district1 = disList.ToList();

            if (command.Trim().ToLower() == "reset")
            {
                Session["search"] = null;
            }
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.pagesize = pageIndex;
            List<SelectListItem> forms = objDB.FormsList(0);
            ViewBag.forms = forms;
            CallCenter rm = new CallCenter();
            string Search = Convert.ToString(Session["search"]);
            if (page != null && Convert.ToString(Session["Search"]) != "")
            {
                ViewBag.classn = Convert.ToString(Session["classname"]);
                ViewBag.sell = Convert.ToString(Session["SelList"]);
                ViewBag.stat = Convert.ToString(Session["status1"]);
                ViewBag.datesub = Convert.ToString(Session["dateSubmitted"]);
                ViewBag.selu = Convert.ToString(Session["seluser"]);
                ViewBag.ticket = Session["ticket"].ToString().Trim();
                rm.StoreAllData = objDB.GetStudentCallCenterRecordsSearchSCHL(schl, Search, pageIndex);
                //  rm.TotalCount = objDB.GetStudentRecordsSearchCallCenterCount(Search);
                if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View(rm);
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
            else
            {
                Search = " schoolcode='" + schl + "' ";
                Session["classname"] = null;
                Session["SelList"] = null;
                Session["status1"] = null;
                Session["dateSubmitted"] = null;
                Session["seluser"] = null;
                Session["ticket"] = null;
                rm.StoreAllData = objDB.GetStudentCallCenterRecordsSearchSCHL(schl, Search, pageIndex);
                //  rm.TotalCount = objDB.GetStudentRecordsSearchCallCenterCount(Search);
                if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    //return View();
                }
                else
                {
                    ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                    int tp = rm.StoreAllData.Tables[0].Rows.Count;
                    int pn = tp / 50;
                    ViewBag.pn = pn;
                }
                return View(rm);
            }

        }

        [HttpPost]
        public ActionResult DisplayGrievances(int? page, string id, FormCollection fc)
        {
            string schl = string.Empty;
            if (Session["SCHL"] != null && Session["SCHL"].ToString() != string.Empty)
            {
                schl = Session["SCHL"].ToString();
            }
            else
            {
                return RedirectToAction("Logout", "Login");
            }
            List<District> disList = new List<District>();
            DataSet ds = objCommon.Fll_Dist_Details();
            int table = Convert.ToInt32(ds.Tables.Count);// count the number of table in dataset
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                disList.Add(new District { DIST = Convert.ToInt32(dr["DIST"]), DISTNM = Convert.ToString(dr["DISTNM"]) });
            }
            ViewBag.district1 = disList.ToList();

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.pagesize = pageIndex;
            List<SelectListItem> forms = objDB.FormsList(0);
            ViewBag.forms = forms;
            CallCenter rm = new CallCenter();
            string Search = string.Empty;
            if (!string.IsNullOrEmpty(schl))
            { Search += "schoolcode='" + schl + "' "; }

            if (fc["SelList"] != null && fc["SelList"].ToString() != string.Empty)
            {
                Session["SelList"] = fc["SelList"].ToString();
                Search += "and district like '" + fc["SelList"].ToString() + "' ";
            }
            if (fc["SelUser"] != null && fc["SelUser"].ToString() != string.Empty)
            {
                Session["seluser"] = fc["SelUser"].ToString();
                Search += "and CreatedBy like '" + fc["SelUser"].ToString() + "' ";
            }
            if (fc["classname"] != null && fc["classname"].ToString() != string.Empty)
            {
                Session["classname"] = fc["classname"].ToString();
                Search += "and classname like '" + fc["classname"].ToString() + "' ";
            }
            if (fc["status1"] != null && fc["status1"].ToString() != string.Empty)
            {
                Session["status1"] = fc["status1"].ToString();
                Search += "and status like '" + fc["status1"].ToString() + "' ";
                ViewBag.stat = fc["status1"].ToString();
            }
            if (fc["formname"] != null && fc["formname"].ToString() != string.Empty)
            {
                Session["formname"] = fc["formname"].ToString();
                Search += "and formname like '" + fc["formname"].ToString() + "' ";
                ViewBag.formname = fc["formname"].ToString();
            }
            if (fc["dateSubmitted"] != null && fc["dateSubmitted"].ToString() != string.Empty)
            {
                Session["dateSubmitted"] = fc["dateSubmitted"].ToString();
                Search += "and  convert(varchar,createddate,103) ='" + fc["dateSubmitted"].ToString() + "'";
                ViewBag.datef = fc["dateSubmitted"].ToString();
            }
            if (fc["ticket"] != null && fc["ticket"].ToString() != string.Empty)
            {
                Session["ticket"] = fc["ticket"].ToString();
                Search += "and ticketno = '" + fc["ticket"].ToString() + "' ";
                ViewBag.ticket = fc["ticket"].ToString();
            }
            //if (datefrom != "" && dateto != "")
            //{
            //    Session["datefrom"] = datefrom;
            //    Session["dateto"] = dateto;
            //    if (datefrom == dateto)
            //        Search += "and CONVERT(VARCHAR(25), a.CreatedDate, 126) LIKE '" + datefrom + "%'";
            //    else
            //        Search += "and CreatedDate between '" + datefrom + "' and '" + dateto + "' ";
            //}
            rm.StoreAllData = objDB.GetStudentCallCenterRecordsSearchSCHL(schl, Search, pageIndex);
            //rm.TotalCount = objDB.GetStudentRecordsSearchCallCenterCount(Search);
            if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                return View();
            }
            else
            {
                Session["search"] = Search;
                ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                int count = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[0]["decount"]);
                ViewBag.TotalCount1 = count;
                int tp = Convert.ToInt32(count);
                int pn = tp / 50;
                int cal = 50 * pn;
                int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                if (res >= 1)
                    ViewBag.pn = pn + 1;
                else
                    ViewBag.pn = pn;
                return View(rm);
            }
            //return View();
        }

        #endregion School Grievance Management System for Support


        #region MenuMaster



        #endregion MenuMaster

        #region Exam Centre
        public ActionResult ExamCentre(int? page)
        {
            string schl = null;
            if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
            {
                Session.Clear();
                return RedirectToAction("Logout", "Login");
            }

            Printlist obj = new Printlist();
            #region Circular

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.pagesize = pageIndex;

            string Search = string.Empty;
            Search = "Id like '%' and CircularTypes like '%4%' and Convert(Datetime,Convert(date,ExpiryDateDD))>=Convert(Datetime,Convert(date,getdate()))";
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

            //
            schl = Session["SCHL"].ToString();
            obj.StoreAllData = new AbstractLayer.DEODB().CentreForExam(schl);
            if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "0";
                ViewBag.TotalCount = 0;
                ViewBag.Matric = ViewBag.OMatric = ViewBag.Senior = ViewBag.OSenior = "0";
            }
            else
            {
                ViewBag.Message = "1";
                ViewBag.TotalCount = 1;
                if (obj.StoreAllData.Tables[1].Rows.Count > 0)
                {
                    ViewBag.Matric = obj.StoreAllData.Tables[1].Rows[0]["Matric"].ToString();
                    ViewBag.OMatric = obj.StoreAllData.Tables[1].Rows[0]["OMatric"].ToString();
                    ViewBag.Senior = obj.StoreAllData.Tables[1].Rows[0]["Senior"].ToString();
                    ViewBag.OSenior = obj.StoreAllData.Tables[1].Rows[0]["OSenior"].ToString();
                }
            }
            return View(obj);
        }
        #endregion ExamCentre


        public ActionResult Welcome()
        {            // start Admin Menu

            if (Session["AdminId"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }

        #region Private Signature Chart, Confidential List and Admit Card Both
        public ActionResult SignatureChartPrivate(string id)
        {
            if (id == null || id == "")
            { return RedirectToAction("Index", "Login"); }

            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                string CLASS1 = "";
                if (id.ToUpper() == "S") // For Senior
                {
                    CLASS1 = "12";
                    ViewBag.ClassName = "Senior Secondary";
                }
                else if (id.ToUpper() == "M") // For MAtric
                {
                    CLASS1 = "10";
                    ViewBag.ClassName = "Matric";
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                ViewBag.cid = id;
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = Session["cent"].ToString();
                    if (Cent != "")
                    {
                        DataSet Dresult = objDB.GetCentcode(Schl);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        sm.ExamCent = Cent;
                        sm.ExamSub = "";
                        sm.ExamRoll = "";

                        DataSet SetResult = objDB.SignatureChartPvtSet(sm, Convert.ToInt32(CLASS1)); //SignatureChartMatricSub
                        List<SelectListItem> Setlist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in SetResult.Tables[0].Rows)
                        {
                            Setlist.Add(new SelectListItem { Text = @dr["set"].ToString(), Value = @dr["set"].ToString() });
                        }
                        ViewBag.MySelSet = Setlist;

                        // DataSet Subresult = objDB.SignatureChartPvt(sm,Convert.ToInt32(CLASS1)); //SignatureChartMatricSub
                        DataSet Subresult = objDB.SignatureChartPvtSub(sm, Convert.ToInt32(CLASS1)); //SignatureChartMatricSub
                        List<SelectListItem> Sublist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Subresult.Tables[0].Rows)
                        {
                            Sublist.Add(new SelectListItem { Text = @dr["subnm"].ToString(), Value = @dr["sub"].ToString() });
                        }

                        ViewBag.MyExamSub = Sublist;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }
        [HttpPost]
        public ActionResult SignatureChartPrivate(FormCollection frc, string id)
        {
            if (id == null || id == "")
            { return RedirectToAction("SignatureChartPrivate", "School"); }

            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string CLASS1 = "";
                    if (id.ToUpper() == "S") // For Senior
                    {
                        CLASS1 = "12";
                        ViewBag.ClassName = "Senior Secondary";
                    }
                    else if (id.ToUpper() == "M") // For MAtric
                    {
                        CLASS1 = "10";
                        ViewBag.ClassName = "Matric";
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login");
                    }
                    ViewBag.cid = id;
                    string Schl = Session["SCHL"].ToString();
                    string Cent = frc["ExamCent"].ToString();
                    string roll = frc["ExamRoll"].ToString();
                    if (Cent != "")
                    {
                        sm.ExamCent = Cent;
                        sm.SelSet = frc["SelSet"].ToString();
                        sm.ExamSub = frc["ExamSub"].ToString();
                        sm.ExamRoll = frc["ExamRoll"].ToString();

                        DataSet Dresult = objDB.GetCentcode(Schl);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;

                        DataSet SetResult = objDB.SignatureChartPvtSet(sm, Convert.ToInt32(CLASS1)); //SignatureChartMatricSub
                        List<SelectListItem> Setlist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in SetResult.Tables[0].Rows)
                        {
                            Setlist.Add(new SelectListItem { Text = @dr["set"].ToString(), Value = @dr["set"].ToString() });
                        }
                        ViewBag.MySelSet = Setlist;

                        DataSet Subresult = objDB.SignatureChartPvtSub(sm, Convert.ToInt32(CLASS1)); //SignatureChartMatricSub
                        List<SelectListItem> Sublist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Subresult.Tables[0].Rows)
                        {
                            Sublist.Add(new SelectListItem { Text = @dr["subnm"].ToString(), Value = @dr["sub"].ToString() });
                        }

                        ViewBag.MyExamSub = Sublist;

                        if (sm.ExamCent != "" && sm.SelSet != "" && sm.ExamSub != "")
                        {
                            sm.StoreAllData = objDB.GetSignatureChartPvt(sm, Convert.ToInt32(CLASS1));
                        }
                        sm.ExamCent = Cent;
                        // ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        //ViewBag.SearchMsg = sm.StoreAllData.Tables[0].Rows.Count;
                        //if (ViewBag.SearchMsg == 0)
                        //{
                        //    ViewBag.Message = "No Record Found";
                        //}
                        if (sm.StoreAllData == null || sm.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                            return View(sm);
                        }
                        else
                        {
                            ViewBag.SearchMsg = ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                            return View(sm);
                        }
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }

        public ActionResult ConfidentialListPrivate(string id)
        {
            if (id == null || id == "")
            { return RedirectToAction("Index", "Login"); }


            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                string CLASS1 = "";
                if (id.ToUpper() == "S") // For Senior
                {
                    CLASS1 = "12";
                    ViewBag.ClassName = "Senior Secondary";
                }
                else if (id.ToUpper() == "M") // For MAtric
                {
                    CLASS1 = "10";
                    ViewBag.ClassName = "Matric";
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                ViewBag.cid = id;
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = Session["cent"].ToString();
                    if (Cent != "")
                    {

                        sm.ExamCent = Cent;

                        DataSet Dresult = objDB.GetCentcode(Schl);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }
        [HttpPost]
        public ActionResult ConfidentialListPrivate(FormCollection frc, string id)
        {
            if (id == null || id == "")
            { return RedirectToAction("ConfidentialListPrivate", "School"); }

            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                string CLASS1 = "";
                if (id.ToUpper() == "S") // For Senior
                {
                    CLASS1 = "12";
                    ViewBag.ClassName = "Senior Secondary";
                }
                else if (id.ToUpper() == "M") // For MAtric
                {
                    CLASS1 = "10";
                    ViewBag.ClassName = "Matric";
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                ViewBag.cid = id;
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = frc["ExamCent"].ToString();
                    if (Cent != "")
                    {

                        sm.ExamCent = Cent;
                        DataSet Dresult = objDB.GetCentcode(Schl);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                        }
                        ViewBag.MySchCode = schllist;
                        // string search;
                        ////if (frc["ExamCent"].ToString() != "")
                        ////{
                        ////    sm.ExamCent = frc["ExamCent"].ToString();                           
                        ////}                       
                        sm.StoreAllData = objDB.ConfidentialListPvt(sm, Convert.ToInt32(CLASS1));
                        // sm.ExamCent = Session["cent"].ToString();
                        // ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        if (sm.StoreAllData == null || sm.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                        }
                        else
                        {
                            ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        }
                        return View(sm);
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }

        public ActionResult AdmitCardPrivate(string id)
        {
            try
            {
                string CLASS1 = "";
                if (id.ToUpper() == "S") // For Senior
                {
                    CLASS1 = "12";
                    ViewBag.ClassName = "Senior Secondary";
                }
                else if (id.ToUpper() == "M") // For MAtric
                {
                    CLASS1 = "10";
                    ViewBag.ClassName = "Matric";
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                ViewBag.cid = id;
                SchoolModels rm = new SchoolModels();
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    string schlid = Session["SCHL"].ToString();
                    //string ClsType = "2";
                    rm.StoreAllData = null;
                    ViewBag.TotalCountadded = "";
                }
                if (ModelState.IsValid)
                { return View(rm); }
                else
                { return View(); }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }
        }

        [HttpPost]
        public ActionResult AdmitCardPrivate(FormCollection frc, string id)
        {
            try
            {
                string CLASS1 = "";
                if (id.ToUpper() == "S") // For Senior
                {
                    CLASS1 = "12";
                    ViewBag.ClassName = "Senior Secondary";
                }
                else if (id.ToUpper() == "M") // For MAtric
                {
                    CLASS1 = "10";
                    ViewBag.ClassName = "Matric";
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                SchoolModels rm = new SchoolModels();
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    string schlid = Session["SCHL"].ToString();
                    string ClsType = "2";
                    rm.StoreAllData = null;
                    ViewBag.TotalCountadded = "";

                    rm.CandId = frc["CandId"].ToString();
                    rm.ExamRoll = frc["ExamRoll"].ToString();
                    rm.SelList = frc["SelList"].ToString();

                    string search = "";
                    if (rm.SelList.ToUpper() == "ALL")
                    {
                        search = " pc.schl='" + schlid + "'";
                    }
                    if (rm.CandId.Trim() != "" || rm.ExamRoll.Trim() != "")
                    {

                        search = " pc.examroll is not null ";
                        if (rm.CandId.Trim() != "")
                        {
                            search += " and exm.reg16id='" + rm.CandId + "'";
                        }
                        if (rm.ExamRoll.Trim() != "")
                        {
                            search += " and pc.examROLL='" + rm.ExamRoll + "'";
                        }

                    }

                    rm.StoreAllData = objDB.AdmitCardPrivate(search, schlid, Convert.ToInt32(CLASS1));

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
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }
        }

        #endregion  Private  Signature Chart and Confidential List and Admit Card Both


        #region School Result Page
        public ActionResult Schoolresultpage()
        {
            try
            {

                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                {
                    return RedirectToAction("Logout", "Login");
                }
                FormCollection frm = new FormCollection();
                var itemsch = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
            new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedYear = "2016";
                var itemyear = new SelectList(new[] { new { ID = "2016", Name = "2016" }, }, "ID", "Name", 1);
                ViewBag.MyYear = itemyear.ToList();
                string year = ViewBag.SelectedYear;
                var itemschcode = new SelectList(new[] { new { ID = "1", Name = "SCHOOL CODE" } }, "ID", "Name", 1);
                ViewBag.MySchcode = itemschcode.ToList();

                //var itemschform = new SelectList(new[] { new { ID = "1", Name = "N1" }, new { ID = "2", Name = "N2" }, new { ID = "3", Name = "N3" }, new { ID = "4", Name = "E1" }, new { ID = "5", Name = "E2" } }, "ID", "Name", 1);
                //ViewBag.MyForm = itemschform.ToList();

                DataSet result = objDBReg.schooltypes(Session["SCHL"].ToString()); // passing Value to DBClass from model
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
                    ViewBag.N3M1threclock = result.Tables[4].Rows[0]["Nth"].ToString();
                    ViewBag.E1T1threclock = result.Tables[5].Rows[0]["Eth"].ToString();
                }


                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                SchoolModels am = new SchoolModels();
                if (ModelState.IsValid)
                {
                    string Search = string.Empty;
                    string schlid = frm["TotalSearchString"];
                    string srch = Convert.ToString(Session["Search"]);
                    if (srch != null && srch != "")
                    {
                        //frm["SelForm"] = Session["SelForm"].ToString();
                        am.SelForm = Session["SelForm"].ToString();
                        am.SelList = Session["SelList"].ToString();
                        am.SearchByString = Session["SearchByString"].ToString();



                        //ViewBag.MySch = Session["SelList"];
                        // Session["SearchByString"]

                        //frm["TotalSearchString"] = Session["txtSchoolcode"].ToString();
                        //string list = Session["ddlSchoolcode"].ToString();
                        //if (list == "1")
                        //{
                        //    frm["totalcountlist"] = Session["ddlSchoolcode"].ToString();
                        //    ViewBag.SelectedItemcode = frm["totalcountlist"];
                        //    am.TotalSearchString = Session["txtSchoolcode"].ToString();

                        //}
                        Search = Session["Search"].ToString();

                        am.StoreAllData = objDB.GetSchoolRecordsSearch(Search, year);
                        if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                            return View();
                        }
                        else
                        {
                            // Session["Search"] = string.Empty;
                            ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
                            return View(am);
                        }
                    }
                    if (Session["SCHL"] != null)
                    {
                        schlid = Session["SCHL"].ToString();
                    }
                    else
                    {
                        ViewBag.Message = "Record Not Found";
                        return View(am);
                    }
                    Search = "schl='" + schlid + "' ";
                    am.StoreAllData = objDB.GetSchoolRecordsSearch(Search, year);
                    if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
                        return View(am);
                    }
                }
                else
                {
                    return Schoolresultpage();
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
        public ActionResult Schoolresultpage(FormCollection frm, string Year)
        {

            if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
            {
                return RedirectToAction("Logout", "Login");
            }
            var itemsch = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
            new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();

            var itemschcode = new SelectList(new[] { new { ID = "1", Name = "SCHOOL CODE" } }, "ID", "Name", 1);
            ViewBag.MySchcode = itemschcode.ToList();
            ViewBag.SelectedYear = Year;
            var itemyear = new SelectList(new[] { new { ID = "2016", Name = "2016" }, }, "ID", "Name", 1);
            ViewBag.MyYear = itemyear.ToList();

            //var itemschform = new SelectList(new[] { new { ID = "1", Name = "N1" }, new { ID = "2", Name = "N2" }, new { ID = "3", Name = "N3" }, new { ID = "4", Name = "E1" }, new { ID = "5", Name = "E2" } }, "ID", "Name", 1);
            //ViewBag.MyForm = itemschform.ToList();

            DataSet result = objDBReg.schooltypes(Session["SCHL"].ToString()); // passing Value to DBClass from model
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
                ViewBag.N3M1threclock = result.Tables[4].Rows[0]["Nth"].ToString();
                ViewBag.E1T1threclock = result.Tables[5].Rows[0]["Eth"].ToString();
            }

            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels am = new SchoolModels();
            if (ModelState.IsValid)
            {
                string Search = string.Empty;
                Search = "schl ='" + Session["SCHL"].ToString() + "' ";
                //string schlid = "";
                string SelTotalItem = frm["totalcountlist"];
                // string TotalSearchString = Convert.ToString(frm["TotalSearchString"]);
                ViewBag.SelectedForm = frm["SelForm"].ToString();
                //am.TotalSearchString = TotalSearchString;
                //if (frm["totalcountlist"].ToString() != "" && TotalSearchString != "" && TotalSearchString != null)
                if (frm["totalcountlist"] != "")
                {
                    if (frm["SelForm"].ToString() != "")
                    {
                        string FormName = frm["SelForm"].ToString();
                        if (FormName == "1") { FormName = "N1"; }
                        else if (FormName == "2") { FormName = "N2"; }
                        else if (FormName == "3") { FormName = "N3"; }
                        else if (FormName == "4") { FormName = "E1"; }
                        else if (FormName == "5") { FormName = "E2"; }
                        ViewBag.SelectedItemcode = frm["totalcountlist"];
                        // schlid = frm["TotalSearchString"];
                        Search += " and form_Name ='" + FormName + "'";
                    }
                    else
                    {
                        ViewBag.SelectedItemcode = frm["totalcountlist"];
                        // schlid = frm["TotalSearchString"];
                        //if (schlid != "")
                        //{ Search = "schl='" + schlid + "' and FORM in ('E1','E2','N1','N2','N3') "; }
                        //else
                        //{
                        Search += " and form_Name in ('E1','E2','N1','N2','N3') ";
                        // } 
                    }

                }
                //if (TotalSearchString != "")
                //{
                //schlid = frm["TotalSearchString"];
                //if (schlid != "")
                //{ Search += " and schl='" + schlid + "' "; }
                //Search = "schl like '%" + schlid + "%' ";
                //  Search = "and schl like '%" + schlid + "%' ";
                //}

                if (frm["SelList"] != "")
                {
                    ViewBag.SelectedItem = frm["SelList"];
                    int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());
                    if (frm["SearchByString"] != "" && frm["SearchByString"] != null)
                    {
                        if (SelValueSch == 1)
                        { Search += " and Std_id='" + frm["SearchByString"].ToString() + "'"; }
                        else if (SelValueSch == 2)
                        { Search += " and  RegNo like '%" + frm["SearchByString"].ToString() + "%'"; }
                        else if (SelValueSch == 3)
                        { Search += " and  name like '%" + frm["SearchByString"].ToString() + "%'"; }
                        else if (SelValueSch == 4)
                        { Search += " and  fname  like '%" + frm["SearchByString"].ToString() + "%'"; }
                        else if (SelValueSch == 5)
                        { Search += " and mname like '%" + frm["SearchByString"].ToString() + "%'"; }
                        else if (SelValueSch == 6)
                        { Search += " and DOB='" + frm["SearchByString"].ToString() + "'"; }
                    }
                }
                am.SelForm = frm["SelForm"];
                am.SelList = frm["SelList"];
                am.SearchByString = frm["SearchByString"];
                am.SearchResult = Search;
                am.StoreAllData = objDB.GetSchoolRecordsSearch(Search, Year);
                if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {

                    Session["Search"] = Search.ToString();
                    Session["SelForm"] = frm["SelForm"];
                    Session["SelList"] = frm["SelList"];
                    Session["SearchByString"] = frm["SearchByString"];

                    ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
                    return View(am);
                }
            }
            else
            {
                return Schoolresultpage();
            }
        }
        public ActionResult SchoolResultUpdate(string id, string year)
        {
            try
            {
                SchoolModels am = new SchoolModels();
                try
                {
                    id = encrypt.QueryStringModule.Decrypt(id);
                    year = encrypt.QueryStringModule.Decrypt(year);
                    ViewBag.year = year;
                }
                catch (Exception)
                {
                    Session["Search"] = null;
                    return RedirectToAction("Index", "Login");
                }

                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                {
                    return RedirectToAction("Logout", "Login");
                }

                string stdid = id;
                if (stdid != null)
                {
                    ViewBag.MyEXM = objCommon.GroupName1();
                    // ViewBag.MyEXM = new string[] { "NA", "NA" };
                    AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

                    string Search = string.Empty;
                    Search = "std_id='" + stdid + "' ";
                    am.StoreAllData = objDB.GetSchoolRecordsSearch(Search, year);
                    if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
                        am.Candi_Name = am.StoreAllData.Tables[0].Rows[0]["Candi_Name"].ToString();
                        am.Father_Name = am.StoreAllData.Tables[0].Rows[0]["Father_Name"].ToString();
                        am.Mother_Name = am.StoreAllData.Tables[0].Rows[0]["Mother_Name"].ToString();
                        am.DOB = am.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        am.Gender = am.StoreAllData.Tables[0].Rows[0]["Gender"].ToString();
                        am.Result = am.StoreAllData.Tables[0].Rows[0]["result"].ToString();
                        am.TotalMarks = am.StoreAllData.Tables[0].Rows[0]["totMarks"].ToString();
                        am.ObtainedMarks = am.StoreAllData.Tables[0].Rows[0]["obtMarks"].ToString();
                        am.reclock = am.StoreAllData.Tables[0].Rows[0]["reclock"].ToString();
                        am.SdtID = am.StoreAllData.Tables[0].Rows[0]["std_id"].ToString();
                        am.FormName = am.StoreAllData.Tables[0].Rows[0]["Form_Name"].ToString();
                        ViewBag.SelectedExam = am.StoreAllData.Tables[0].Rows[0]["EXAM"].ToString();
                        return View(am);
                    }
                }
                else
                {

                    return SchoolResultUpdate(id, year);
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
        public ActionResult SchoolResultUpdate(FormCollection fc)
        {
            if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
            {
                return RedirectToAction("Logout", "Login");
            }
            try
            {
                SchoolModels am = new SchoolModels();
                string stdid = encrypt.QueryStringModule.Decrypt(fc["ID"]);

                if (stdid != null)
                {
                    AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                    am.id = Int32.Parse(stdid);
                    am.TotalMarks = fc["TotalMarks"];
                    am.ObtainedMarks = fc["ObtainedMarks"];
                    am.Result = fc["resultlist"];
                    am.reclock = fc["reclocklist"] == "TRUE" ? "1" : "0";
                    am.EXAM = fc["exam"];
                    string year = "2016";

                    am.SCHL = Session["SCHL"].ToString();

                    int result = objDB.UpdateStudentRecords(am, year);
                    // asm.StoreAllData = objDB.SearchSchoolDetails(Search);
                    if (result > 0)
                    {
                        ViewBag.Message = "Record Updated Successfully";
                        return RedirectToAction("Schoolresultpage", "School");
                        //return adminresultpageReCall(fc);
                    }
                    else
                    {
                        return RedirectToAction("SchoolResultUpdate", "School");
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Login");
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                Session["Search"] = null;
                return RedirectToAction("Logout", "Login");

            }
        }

        [HttpPost]
        public ActionResult FinalsubmitResult(FormCollection fc)
        {
            try
            {
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                {
                    return RedirectToAction("Logout", "Login");
                }

                SchoolModels am = new SchoolModels();
                if (fc["TotalSearchString"].ToString() != null)
                {
                    AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

                    am.SchlCode = fc["TotalSearchString"].ToString();

                    int result = objDB.FinalsubmitResult(am);
                    if (result > 1)
                    {
                        TempData["notice"] = "All Record Successfully Submitted";
                        //return RedirectToAction("adminresultpage", "Admin");
                    }
                    else
                    {
                        TempData["notice"] = "Kindly update all student result then click Final Submit Result Button ";
                    }
                }
                return RedirectToAction("Schoolresultpage", "School");
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        #endregion School Result Page

        #region Begin CCE Senior  
        [SessionCheckFilter]
        public ActionResult CCE_Grading_Portal()
        {

            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            ViewBag.Senior = loginSession.Senior;
            ViewBag.Matric = loginSession.Matric;
            TempData["CCE_SeniorSearch"] = null;
            return View();
        }


        [SessionCheckFilter]
        public ActionResult CCE_Senior(string id, int? page)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            string SCHL = loginSession.SCHL.ToString();
            ViewBag.schlCode = SCHL;
            ViewBag.cid = id;
            try
            {
                string CLASS = "";
                if (id == "S") // For Senior
                {
                    CLASS = "4";
                    ViewBag.ClassName = "SENIOR SECONDARY";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "2";
                    ViewBag.ClassName = "MATRIC";
                }




                #region  Check School Allow For CCE

                DataSet dsAllow = objDB.SchoolAllowForCCE(SCHL, CLASS, "CCE");
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

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
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
                    if (TempData["CCE_SeniorSearch"] != null)
                    {
                        Search += TempData["CCE_SeniorSearch"].ToString();
                        ViewBag.SelectedFilter = TempData["SelFilter"];
                        SelectedAction = TempData["SelAction"].ToString();
                        ViewBag.SelectedAction = TempData["SelActionValue"];

                        TempData["SelFilter"] = ViewBag.SelectedFilter;
                        TempData["SelAction"] = SelectedAction;
                        TempData["SelActionValue"] = ViewBag.SelectedAction;
                        TempData["CCE_SeniorSearch"] = Search;
                    }
                    else
                    {
                        Search = "  a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "'";
                    }


                    //string class1 = "4"; // For Senior
                    MS.StoreAllData = objDB.GetDataBySCHL(Search, SCHL, pageIndex, CLASS, Convert.ToInt32(SelectedAction));
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
        public ActionResult CCE_Senior(string id, FormCollection frm, int? page)
        {

            LoginSession loginSession = (LoginSession)Session["LoginSession"];

            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            string SCHL = loginSession.SCHL.ToString();
            ViewBag.schlCode = SCHL;
            ViewBag.cid = id;
            try
            {
                if (frm["cid"] != "")
                {
                    id = frm["cid"].ToString();
                    ViewBag.cid = frm["cid"].ToString();
                }


                string CLASS = "";
                if (id == "S") // For Senior
                {
                    CLASS = "4";
                    ViewBag.ClassName = "SENIOR SECONDARY";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "2";
                    ViewBag.ClassName = "MATRIC";
                }

                #region  Check School Allow For CCE

                DataSet dsAllow = objDB.SchoolAllowForCCE(SCHL, CLASS, "CCE");
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

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
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
                            if (SelValueSch == 3)
                            {
                                SelAction = 2;
                            }
                            //  { Search += " and  IsCCEFilled=1 "; } // Filled
                            else if (SelValueSch == 2)
                            { SelAction = 1; }
                            //{ Search += " and (IsCCEFilled is null or IsCCEFilled=0) "; } // pending
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
                            { Search += " and b.Roll='" + frm["SearchString"].ToString() + "'"; }
                            if (SelValueSch == 2)
                            { Search += " and Std_id='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 3)
                            { Search += " and  Registration_num like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " and  Candi_Name like '%" + frm["SearchString"].ToString() + "%'"; }
                        }
                    }

                    TempData["SelFilter"] = frm["SelFilter"];
                    TempData["SelAction"] = SelAction;
                    TempData["SelActionValue"] = frm["SelAction"];
                    TempData["CCE_SeniorSearch"] = Search;
                    // string class1 = "4";
                    MS.StoreAllData = objDB.GetDataBySCHL(Search, SCHL, pageIndex, CLASS, SelAction);
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

        [SessionCheckFilter]
        [HttpPost]
        public JsonResult JqCCESenior(string stdid, string CandSubject)
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

                    if ((obt < 1) || (obt > max))
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
                dee = objDB.AllotCCESenior(stdid, dtSub, class1, out OutStatus);

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
        public ActionResult CCEReport(string id)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];

            TempData["CCE_SeniorSearch"] = null;
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string SCHL = "";
                string CLASS = "";
                if (id == "S") // For Senior
                {
                    CLASS = "4";
                    ViewBag.ClassName = "SENIOR";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "2";
                    ViewBag.ClassName = "MATRIC";
                }

                if (SCHL == "" || SCHL == null)
                {
                    SCHL = Session["SCHL"].ToString();
                }

                #region  Check School Allow For CCE

                DataSet dsAllow = objDB.SchoolAllowForCCE(SCHL, CLASS, "CCE");
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
                MS.StoreAllData = objDB.CCEREPORT(Search, SCHL, CLASS);
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
        public ActionResult CCEFinalReport(string id, FormCollection frm)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            TempData["CCE_SeniorSearch"] = null;

            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string SCHL = "";
                string CLASS = "";
                if (id == "S") // For Senior
                {
                    CLASS = "4";
                    ViewBag.ClassName = "SENIOR";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "2";
                    ViewBag.ClassName = "MATRIC";
                }

                if (SCHL == "" || SCHL == null)
                {
                    SCHL = Session["SCHL"].ToString();
                }

                #region  Check School Allow For CCE

                DataSet dsAllow = objDB.SchoolAllowForCCE(SCHL, CLASS, "CCE");
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
                MS.StoreAllData = dsFinal = objDB.CCEFinalReport(Search, SCHL, CLASS);
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
                    MS.StoreAllData = objDB.CCEREPORT(Search, SCHL, CLASS);
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

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult CCEFinalReport(string id)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            TempData["CCE_SeniorSearch"] = null;

            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string SCHL = "";
                string CLASS = "";
                if (id == "S") // For Senior
                {
                    CLASS = "4";
                    ViewBag.ClassName = "SENIOR";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "2";
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
                MS.StoreAllData = objDB.CCEREPORTFinalSubmit(Search, SCHL, CLASS);  //CCEREPORTFinalSubmit
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
                    //ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["IsCCEFilled"]);
                    //if (MS.StoreAllData.Tables[2].Rows.Count > 0)
                    //{
                    //    int totalFinalPending = Convert.ToInt32(MS.StoreAllData.Tables[2].Rows[0]["TotalPending"]);
                    //    if (totalFinalPending == 0)
                    //    {
                    //        ViewBag.IsFinal = 0;
                    //    }
                    //    else { ViewBag.IsFinal = 1; }
                    //}
                    if (CLASS == "2")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["MSET"].ToString(); }
                    else if (CLASS == "4")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["SSET"].ToString(); }
                    else if (CLASS == "10")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["MOSET"].ToString(); }
                    else if (CLASS == "12")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["SOSET"].ToString(); }

                    return View(MS);
                    //  return RedirectToAction("CCEFinalReport", "School", new { id= id});
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }



        public ActionResult Agree(string id)
        {
            try
            {

                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                Session["CCEClass"] = id;
                // string fpdf = ViewBag.FormName = Request.QueryString["Form"];
                //// string fpdf = Session["FormName"].ToString();
                if (id == "S")
                {
                    @ViewBag.Dpdf = "../../PDF/12th_CCE.pdf";
                    @ViewBag.Showpdf = "../../PDF/12th_CCE.pdf";
                }
                else if (id == "M")
                {
                    @ViewBag.Dpdf = "../../PDF/10th_CCE.pdf";
                    @ViewBag.Showpdf = "../../PDF/10th_CCE.pdf";
                }
                else if (id == "SG")
                {
                    @ViewBag.Dpdf = "../../PDF/12th_Grading.pdf";
                    @ViewBag.Showpdf = "../../PDF/12th_Grading.pdf";
                }
                else if (id == "MG")
                {
                    @ViewBag.Dpdf = "../../PDF/10th_Grading.pdf";
                    @ViewBag.Showpdf = "../../PDF/10th_Grading.pdf";
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult CheckAgree(FormCollection frm)
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                string s = frm["Agree"].ToString();
                if (Session["CCEClass"] == null)
                {
                    return RedirectToAction("CCE_Grading_Portal", "School");
                }
                else
                {
                    string CCEClass1 = Session["CCEClass"].ToString();
                    ViewBag.CCEClass = Session["CCEClass"].ToString();
                    if (s == "Agree")
                    {
                        if (ViewBag.CCEClass == "S" || ViewBag.CCEClass == "M")
                        { return RedirectToAction("CCE_Senior", "School", new { id = CCEClass1 }); }
                        else if (ViewBag.CCEClass == "SG" || ViewBag.CCEClass == "MG")
                        {
                            CCEClass1 = CCEClass1.Substring(0, 1);
                            return RedirectToAction("Grading", "School", new { id = CCEClass1 });

                        }

                    }
                    else
                    {
                        return RedirectToAction("CCE_Grading_Portal", "School");
                    }
                }
                return RedirectToAction("CCE_Grading_Portal", "School");
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }
        }

        #endregion  End CCE Senior 


        #region Begin Grading
        public ActionResult Grading(FormCollection frm, string id, int? page)
        {
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
                    CLASS = "4";
                    ViewBag.ClassName = "SENIOR";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "2";
                    ViewBag.ClassName = "MATRIC";
                }

                if (SCHL == "" || SCHL == null)
                { SCHL = Session["SCHL"].ToString(); }

                ViewBag.schlCode = SCHL;
                ViewBag.cid = id;

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

                    #region  Check School Allow For CCE

                    DataSet dsAllow = objDB.SchoolAllowForCCE(SCHL, CLASS, "ELECTIVE");
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

                    string SelectedAction = "0";
                    if (TempData["GradingSearch"] != null)
                    {
                        Search += TempData["GradingSearch"].ToString();
                        ViewBag.SelectedFilter = TempData["SelFilter"];
                        SelectedAction = TempData["SelAction"].ToString();
                        ViewBag.SelectedAction = TempData["SelActionValue"];

                        TempData["SelFilter"] = ViewBag.SelectedFilter;
                        TempData["SelAction"] = SelectedAction;
                        TempData["SelActionValue"] = ViewBag.SelectedAction;
                        TempData["GradingSearch"] = Search;
                    }
                    else
                    {
                        Search = "  a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "'";
                    }


                    //string class1 = "4"; // For Senior
                    MS.StoreAllData = objDB.GetDataBySCHLGrading(Search, SCHL, pageIndex, CLASS, 0);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.FinalCount = ViewBag.TotalCount = 0;
                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.FinalCount = MS.StoreAllData.Tables[3].Rows.Count;
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsMarksGrading"]);
                        }
                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                        }
                        return View();
                    }
                    else
                    {
                        ViewBag.FinalCount = 0;
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        //ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["IsMarksGrading"]);

                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.FinalCount = MS.StoreAllData.Tables[3].Rows.Count;
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsMarksGrading"]);
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
        public ActionResult Grading(FormCollection frm, int? page)
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
                    CLASS = "4";
                    ViewBag.ClassName = "SENIOR";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "2";
                    ViewBag.ClassName = "MATRIC";
                }
                var itemFilter = new SelectList(new[] { new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                    new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "Pending" }, new { ID = "2", Name = "Filled" }, new { ID = "3", Name = "Final Submitted" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";
                //------------------------


                #region  Check School Allow For Grading

                DataSet dsAllow = objDB.SchoolAllowForCCE(SCHL, CLASS, "ELECTIVE");
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
                #endregion  Check School Allow For Grading


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
                        ViewBag.SelectedAction = frm["SelAction"];
                        int SelValueSch = Convert.ToInt32(frm["SelAction"].ToString());
                        if (frm["SelAction"] != "")
                        {

                            SelAction = SelValueSch;
                            //if (SelValueSch == 4)
                            //{
                            //    SelAction = 3;
                            //}
                            //else if (SelValueSch == 3)
                            //{
                            //    SelAction = 2;
                            //}
                            ////  { Search += " and  IsMarksFilled=1 "; } // Filled
                            //else if (SelValueSch == 2)
                            //{ SelAction = 1; }
                            //{ Search += " and (IsMarksFilled is null or IsMarksFilled=0) "; } // pending
                        }
                    }



                    if (frm["SelFilter"] != "")
                    {
                        //TempData["SelFilter"] = frm["SelFilter"];
                        ViewBag.SelectedFilter = frm["SelFilter"];
                        int SelValueSch = Convert.ToInt32(frm["SelFilter"].ToString());
                        if (frm["SelFilter"] != "" && frm["SearchString"].ToString() != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and b.Roll='" + frm["SearchString"].ToString() + "'"; }
                            if (SelValueSch == 2)
                            { Search += " and Std_id='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 3)
                            { Search += " and  Registration_num like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " and  Candi_Name like '%" + frm["SearchString"].ToString() + "%'"; }
                        }
                    }

                    TempData["SelFilter"] = frm["SelFilter"];
                    TempData["SelAction"] = SelAction;
                    TempData["SelActionValue"] = frm["SelAction"];
                    TempData["GradingSearch"] = Search;
                    // string class1 = "4";                  
                    MS.StoreAllData = objDB.GetDataBySCHLGrading(Search, SCHL, pageIndex, CLASS, SelAction);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        ViewBag.FinalCount = 0;
                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.FinalCount = MS.StoreAllData.Tables[3].Rows.Count;
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsMarksGrading"]);
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
                        ViewBag.FinalCount = 0;


                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.FinalCount = MS.StoreAllData.Tables[3].Rows.Count;
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsMarksGrading"]);
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
        public JsonResult JqGrading(string stdid, string CandSubject)
        {
            var objResponse1 = JsonConvert.DeserializeObject<List<CandSubject>>(CandSubject);
            DataTable dtSub = new DataTable();
            dtSub.Columns.Add("CANDID");
            dtSub.Columns.Add("SUB");
            dtSub.Columns.Add("OBTMARKS");
            dtSub.Columns.Add("MINMARKS");
            dtSub.Columns.Add("MAXMARKS");
            DataRow row = null;
            //foreach (var rowObj in objResponse1)
            //{
            //    row = dtSub.NewRow();
            //    dtSub.Rows.Add(stdid, rowObj.SUB, rowObj.OBTMARKS.ToUpper(), rowObj.MINMARKS, rowObj.MAXMARKS);
            //}
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
                else if (rowObj.OBTMARKS == "H" || rowObj.OBTMARKS == "HHH")
                {
                    rowObj.OBTMARKS = "HHH";
                }
                else if (rowObj.OBTMARKS != "")
                {
                    rowObj.OBTMARKS = rowObj.OBTMARKS.PadLeft(3, '0');
                }

                dtSub.Rows.Add(stdid, rowObj.SUB, rowObj.OBTMARKS, rowObj.MINMARKS, rowObj.MAXMARKS);
            }
            dtSub.AcceptChanges();

            var flag = 1;
            //foreach (DataRow dr1 in dtSub.Rows)
            //{
            //    if (dr1["OBTMARKS"].ToString() == "" || dr1["OBTMARKS"].ToString().ToUpper() == "A+" || dr1["OBTMARKS"].ToString().ToUpper() == "A" || dr1["OBTMARKS"].ToString().ToUpper() == "B"
            //        || dr1["OBTMARKS"].ToString().ToUpper() == "C" || dr1["OBTMARKS"].ToString().ToUpper() == "D" || dr1["OBTMARKS"].ToString().ToUpper() == "E"
            //        || dr1["OBTMARKS"].ToString().ToUpper() == "Z" || dr1["OBTMARKS"].ToString().ToUpper() == "X")
            //    { }
            //    else
            //    {
            //        flag = -2;
            //        var results = new
            //        {
            //            status = flag
            //        };
            //        return Json(results);
            //    }               
            //}

            //dr1["OBTMARKS"].ToString() == "0" || nhj

            foreach (DataRow dr1 in dtSub.Rows)
            {
                if (dr1["OBTMARKS"].ToString() == "" || dr1["OBTMARKS"].ToString() == "ABS" || dr1["OBTMARKS"].ToString() == "CAN" || dr1["OBTMARKS"].ToString() == "HHH")
                { }
                else if (dr1["OBTMARKS"].ToString().Contains("A") || dr1["OBTMARKS"].ToString().Contains("C") || dr1["OBTMARKS"].ToString().Contains("H"))
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
                    //(obt < 1) ||

                    if ((obt > max))
                    {
                        flag = -2;
                    }
                }
            }
            if (flag == 1)
            {
                string dee = "1";
                string class1 = "2";
                int OutStatus = 0;
                dee = objDB.AllotGrading(stdid, dtSub, class1, out OutStatus); // allot sub9 marks for matric only

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



        public ActionResult GradingReport(string id)
        {
            TempData["Grading_SeniorSearch"] = null;
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
                    CLASS = "4";
                    ViewBag.ClassName = "SENIOR";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "2";
                    ViewBag.ClassName = "MATRIC";
                }

                if (SCHL == "" || SCHL == null)
                {
                    SCHL = Session["SCHL"].ToString();
                }


                #region  Check School Allow For Grading

                DataSet dsAllow = objDB.SchoolAllowForCCE(SCHL, CLASS, "ELECTIVE");
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
                #endregion  Check School Allow For Grading


                ViewBag.schlCode = SCHL;
                ViewBag.cid = id;
                string Search = string.Empty;
                Search = "  a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "'";
                MS.StoreAllData = objDB.GradingREPORT(Search, SCHL, CLASS);
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


        public ActionResult GradingFinalReport(string id, FormCollection frm)
        {
            TempData["Grading_SeniorSearch"] = null;
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
                    CLASS = "4";
                    ViewBag.ClassName = "SENIOR";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "2";
                    ViewBag.ClassName = "MATRIC";
                }

                if (SCHL == "" || SCHL == null)
                {
                    SCHL = Session["SCHL"].ToString();
                }


                #region  Check School Allow For Grading

                DataSet dsAllow = objDB.SchoolAllowForCCE(SCHL, CLASS, "ELECTIVE");
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
                #endregion  Check School Allow For Grading

                ViewBag.schlCode = SCHL;
                ViewBag.cid = id;
                string Search = string.Empty;
                Search = "  a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "'";
                DataSet dsFinal = new DataSet();
                MS.StoreAllData = dsFinal = objDB.GradingFinalReport(Search, SCHL, CLASS);
                if (MS.StoreAllData == null)
                {
                    ViewBag.IsAllowTheory = 0;
                    ViewBag.IsFinal = 0;
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View(MS);
                }
                else if (MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.IsAllowTheory = 1;
                    Search = "  a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "'  ";
                    MS.StoreAllData = objDB.GradingREPORT(Search, SCHL, CLASS);
                    ViewBag.TotalCount = 0;
                    ViewBag.TotalCount1 = MS.StoreAllData.Tables[0].Rows.Count;
                    if (MS.StoreAllData.Tables[0].Rows[0]["GradDate"].ToString() == "")
                    {
                        ViewBag.GradDate = "";
                    }
                    else
                    {
                        ViewBag.GradDate = MS.StoreAllData.Tables[0].Rows[0]["GradDate"].ToString();
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

                    // ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["IsMarksGrading"]);
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

                else
                {
                    ViewBag.IsAllowTheory = 2;
                    ViewBag.IsFinal = 0;
                    ViewBag.TotalCount1 = 0;
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
                    return View(MS);
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }

        [HttpPost]
        public ActionResult GradingFinalReport(string id)
        {
            TempData["CCE_SeniorSearch"] = null;
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
                    CLASS = "4";
                    ViewBag.ClassName = "SENIOR";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "2";
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
                MS.StoreAllData = objDB.GradingREPORTFinalSubmit(Search, SCHL, CLASS);  //CCEREPORTFinalSubmit
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
                    if (MS.StoreAllData.Tables[0].Rows[0]["GradDate"].ToString() == "")
                    {
                        ViewBag.GradDate = "";
                    }
                    else
                    {
                        ViewBag.GradDate = MS.StoreAllData.Tables[0].Rows[0]["GradDate"].ToString();
                    }
                    ViewBag.SchoolName = MS.StoreAllData.Tables[1].Rows[0]["FullSchoolNameE"].ToString();
                    ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["IsMarksGrading"]);
                    if (CLASS == "2")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["MSET"].ToString(); }
                    else if (CLASS == "4")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["SSET"].ToString(); }
                    else if (CLASS == "10")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["MOSET"].ToString(); }
                    else if (CLASS == "12")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["SOSET"].ToString(); }

                    return View(MS);
                    //  return RedirectToAction("CCEFinalReport", "School", new { id= id});
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }


        #endregion  End Grading

        #region Signature Chart and Confidential List Matric
        public ActionResult SignatureChartMatric()
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = Session["cent"].ToString();
                    if (Cent != "")
                    {
                        DataSet Dresult = objDB.GetCentcode(Schl);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        sm.ExamCent = Cent;
                        sm.ExamSub = "";
                        sm.ExamRoll = "";
                        sm.CLASS = "2";

                        DataSet Subresult = objDB.SignatureChartMatricSub(sm);
                        List<SelectListItem> Sublist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Subresult.Tables[0].Rows)
                        {
                            Sublist.Add(new SelectListItem { Text = @dr["subnm"].ToString(), Value = @dr["sub"].ToString() });
                        }

                        ViewBag.MyExamSub = Sublist;

                        // sm.StoreAllData = objDB.SignatureChartSr(sm);
                        //sm.ExamCent= Session["cent"].ToString();
                        //ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }
        [HttpPost]
        public ActionResult SignatureChartMatric(FormCollection frc)
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = frc["ExamCent"].ToString();
                    //string sub = frc["ExamSub"].ToString();
                    string roll = frc["ExamRoll"].ToString();
                    if (Cent != "")
                    {

                        sm.ExamCent = Cent;
                        sm.ExamSub = frc["ExamSub"].ToString();
                        sm.ExamRoll = frc["ExamRoll"].ToString();
                        sm.CLASS = "2";
                        DataSet Dresult = objDB.GetCentcode(Schl);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        DataSet Subresult = objDB.SignatureChartMatricSub(sm);
                        List<SelectListItem> Sublist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Subresult.Tables[0].Rows)
                        {
                            Sublist.Add(new SelectListItem { Text = @dr["subnm"].ToString(), Value = @dr["sub"].ToString() });
                        }

                        ViewBag.MyExamSub = Sublist;

                        sm.StoreAllData = objDB.SignatureChartMatric(sm);
                        sm.ExamCent = Cent;
                        ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.SearchMsg = sm.StoreAllData.Tables[0].Rows.Count;
                        if (ViewBag.SearchMsg == 0)
                        {
                            ViewBag.Message = "No Record Found";
                        }
                        return View(sm);
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }


        public ActionResult ConfidentialListMatric()
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = Session["cent"].ToString();
                    if (Cent != "")
                    {

                        sm.ExamCent = Cent;
                        sm.CLASS = "2";

                        DataSet Dresult = objDB.GetCentcode(Schl);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }
        [HttpPost]
        public ActionResult ConfidentialListMatric(FormCollection frc)
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = frc["ExamCent"].ToString();
                    if (Cent != "")
                    {

                        sm.ExamCent = Cent;

                        DataSet Dresult = objDB.GetCentcode(Schl);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        string search;
                        if (frc["ExamCent"].ToString() != "")
                        {
                            sm.ExamCent = frc["ExamCent"].ToString();
                            sm.CLASS = "2";
                            //search = search + "'" + sm.ExamCent + "'";
                        }
                        //if (frc["ExamRoll"].ToString().Trim() !="")
                        //{
                        //    sm.ExamRoll = frc["ExamRoll"].ToString();
                        //    //search = search + "'" + sm.ExamCent + "'";
                        //}

                        sm.StoreAllData = objDB.ConfidentialListMatric(sm);
                        sm.ExamCent = Session["cent"].ToString();
                        ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        return View(sm);
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }
        #endregion  Signature Chart and Confidential List Matric

        #region Signature Chart and Confidential List 
        public ActionResult SignatureChartSr()
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = Session["cent"].ToString();
                    if (Cent != "")
                    {
                        DataSet Dresult = objDB.GetCentcode(Schl);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        sm.ExamCent = Cent;
                        sm.ExamSub = "";
                        sm.ExamRoll = "";


                        DataSet Subresult = objDB.SignatureChartSrSub(sm);
                        List<SelectListItem> Sublist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Subresult.Tables[0].Rows)
                        {
                            Sublist.Add(new SelectListItem { Text = @dr["subnm"].ToString(), Value = @dr["sub"].ToString() });
                        }

                        ViewBag.MyExamSub = Sublist;

                        // sm.StoreAllData = objDB.SignatureChartSr(sm);
                        //sm.ExamCent= Session["cent"].ToString();
                        //ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }

        [HttpPost]
        public ActionResult SignatureChartSr(FormCollection frc)
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = frc["ExamCent"].ToString();
                    //string sub = frc["ExamSub"].ToString();
                    string roll = frc["ExamRoll"].ToString();
                    if (Cent != "")
                    {

                        sm.ExamCent = Cent;
                        sm.ExamSub = frc["ExamSub"].ToString();
                        sm.ExamRoll = frc["ExamRoll"].ToString();
                        sm.CLASS = "4";
                        DataSet Dresult = objDB.GetCentcode(Schl);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        DataSet Subresult = objDB.SignatureChartSrSub(sm);
                        List<SelectListItem> Sublist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Subresult.Tables[0].Rows)
                        {
                            Sublist.Add(new SelectListItem { Text = @dr["subnm"].ToString(), Value = @dr["sub"].ToString() });
                        }

                        ViewBag.MyExamSub = Sublist;

                        sm.StoreAllData = objDB.SignatureChartSr(sm);
                        sm.ExamCent = Cent;
                        ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.SearchMsg = sm.StoreAllData.Tables[0].Rows.Count;
                        if (ViewBag.SearchMsg == 0)
                        {
                            ViewBag.Message = "No Record Found";
                        }
                        return View(sm);
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }
        public ActionResult ConfidentialListSenior()
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = Session["cent"].ToString();
                    if (Cent != "")
                    {

                        sm.ExamCent = Cent;
                        sm.CLASS = "4";
                        DataSet Dresult = objDB.GetCentcode(Schl);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }

        [HttpPost]
        public ActionResult ConfidentialListSenior(FormCollection frc)
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = frc["ExamCent"].ToString();
                    if (Cent != "")
                    {

                        sm.ExamCent = Cent;
                        sm.CLASS = "4";
                        DataSet Dresult = objDB.GetCentcode(Schl);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        string search;
                        if (frc["ExamCent"].ToString() != "")
                        {
                            sm.ExamCent = frc["ExamCent"].ToString();
                            //search = search + "'" + sm.ExamCent + "'";
                        }
                        //if (frc["ExamRoll"].ToString().Trim() !="")
                        //{
                        //    sm.ExamRoll = frc["ExamRoll"].ToString();
                        //    //search = search + "'" + sm.ExamCent + "'";
                        //}

                        sm.StoreAllData = objDB.ConfidentialListSenior(sm);
                        sm.ExamCent = Session["cent"].ToString();
                        ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        return View(sm);
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }
        #endregion  Signature Chart and Confidential List

        #region Start Bulk Photo Upload Open
        public ActionResult Photo_Upload_Open()
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                else
                {

                    var itemsch = new SelectList(new[]{new {ID="1",Name="N1"},
                                                       new {ID="2",Name="N2"},
                                                       new {ID="3",Name="N3"},
                                                       new {ID="4",Name="M1"},
                                                       new {ID="5",Name="M2"},
                                                       new {ID="6",Name="E1"},
                                                       new {ID="7",Name="E2"},
                                                       new {ID="8",Name="T1"},
                                                       new {ID="9",Name="T2"},
                                                       new {ID="10",Name="Open"},
                                                        }, "ID", "Name", 1);
                    ViewBag.MyForm = itemsch.ToList();
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
        [HttpPost]
        public ActionResult Photo_Upload_Open(HttpPostedFileBase FileUploadBulk, FormCollection frm)
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                var itemsch = new SelectList(new[]{new {ID="1",Name="N1"},
                                               new {ID="2",Name="N2"},
                                               new {ID="3",Name="N3"},
                                               new {ID="4",Name="M1"},
                                               new {ID="5",Name="M2"},
                                               new {ID="6",Name="E1"},
                                               new {ID="7",Name="E2"},
                                               new {ID="8",Name="T1"},
                                               new {ID="9",Name="T2"},
                                               new {ID="10",Name="Open"},
                                                }, "ID", "Name", 1);
                ViewBag.MyForm = itemsch.ToList();

                ViewBag.SelectedItem = frm["FormNameList"];
                string FormName = frm["FormNameList"].ToString();
                if (frm["FormNameList"] != "")
                {
                    if (FormName == "1") { FormName = "N1"; }
                    else if (FormName == "2") { FormName = "N2"; }
                    else if (FormName == "3") { FormName = "N3"; }
                    else if (FormName == "4") { FormName = "M1"; }
                    else if (FormName == "5") { FormName = "M2"; }
                    else if (FormName == "6") { FormName = "E1"; }
                    else if (FormName == "7") { FormName = "E2"; }
                    else if (FormName == "8") { FormName = "T1"; }
                    else if (FormName == "9") { FormName = "T2"; }
                    else if (FormName == "10") { FormName = "Open"; }
                }
                if (FileUploadBulk != null && FormName != "")
                {

                    string DistID = string.Empty;
                    string Std_Id = string.Empty;
                    string Filepath = string.Empty;
                    string path;
                    string SchlID = string.Empty;
                    string filename = string.Empty;
                    string SaveFile = string.Empty;
                    string SaveFileN = string.Empty;
                    string SaveFileNextn = string.Empty;
                    string Incorrectfile = string.Empty;
                    int countP = 0; int countS = 0;
                    SchlID = Convert.ToString(Session["SCHL"]);
                    if (SchlID == null || SchlID == "")
                    {
                        return RedirectToAction("Index", "Login");
                    }

                    if (FileUploadBulk.ContentLength > 0)
                    {

                        HttpFileCollectionBase files = Request.Files;
                        DataTable dt = new DataTable { Columns = { new DataColumn("Path") } };
                        SchoolModels sm = new SchoolModels();
                        AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            string Prefix = string.Empty;
                            if (file.FileName.Length <= 14)
                            {
                                int lname = file.FileName.Length;
                                lname = 14 - lname;
                                switch (lname)
                                {
                                    case 1: Prefix = "0"; break;
                                    case 2: Prefix = "00"; break;
                                    case 3: Prefix = "000"; break;
                                    case 4: Prefix = "0000"; break;
                                    case 5: Prefix = "00000"; break;
                                    case 6: Prefix = "000000"; break;
                                }


                            }
                            string fname = Prefix + file.FileName.ToString();
                            if (file.ContentType != "image/jpeg" || fname.Length > 14 || fname.Length < 6 || (fname.Substring(9, 1).ToUpper() != "P" && fname.Substring(9, 1).ToUpper() != "S"))
                            {
                                SaveFileNextn = SaveFileNextn + file.FileName + ", ";
                            }

                            //else if (file.ContentLength > 50000 || file.ContentLength < 5000)
                            //{
                            //    SaveFileN = SaveFileN + file.FileName + ", ";
                            //}
                            else
                            {
                                filename = file.FileName.Substring(0, 5);
                                //filename = fname.Substring(0, fname.Length);
                                //DistID = "150";
                                DistID = Session["SCHOOLDIST"].ToString();   // Add Dist ID
                                                                             //Std_Id = FormName + DistID + filename;
                                Std_Id = filename;
                                if (fname.Substring(9, 1).ToUpper() == "P")
                                {
                                    //string PhotoName = FormName + "/" + DistID + "/Photo" + "/" + Std_Id + "P" + ".jpg";
                                    string PhotoName = "Correction/" + FormName + "/" + Std_Id + "P" + ".jpg";
                                    string type = "P";
                                    string UpdatePic = objDB.Updated_Bulk_Pic_Data_Open(Std_Id, PhotoName, type, SchlID);
                                    if (UpdatePic.ToString() == "1")
                                    {
                                        // Filepath = Server.MapPath("~/Upload/" + FormName + "/" + DistID + "/Photo/");
                                        //Filepath = sp1 + "/" + FormName + "/" + DistID + "/Photo/";
                                        Filepath = sp1 + "Correction/" + FormName;
                                        if (!Directory.Exists(Filepath))
                                        {
                                            Directory.CreateDirectory(Filepath);
                                        }
                                        SaveFile = SaveFile + file.FileName + ", ";
                                        //path = Filepath + (FormName + DistID + file.FileName);
                                        // path = Path.Combine(Server.MapPath("~/Upload/" + FormName + "/" + DistID + "/Photo"), Std_Id + "P" + ".jpg");
                                        //path = sp + "/" + FormName + "/" + DistID + "/Photo" + "/" + Std_Id + "P" + ".jpg";
                                        path = sp1 + "Correction/" + FormName + "/" + Std_Id + "P" + ".jpg";
                                        dt.Rows.Add(file.FileName);
                                        file.SaveAs(path);
                                        countP = countP + 1;
                                    }
                                    else
                                    {
                                        Incorrectfile = Incorrectfile + file.FileName + ", ";
                                    }
                                }
                                else if (fname.Substring(9, 1).ToUpper() == "S")
                                {
                                    //string PhotoName = FormName + "/" + DistID + "/Sign" + "/" + Std_Id + "S" + ".jpg";
                                    //string PhotoName = FormName + "/Sign" + "/" + Std_Id + "S" + ".jpg";
                                    string PhotoName = "Correction/" + FormName + "/" + Std_Id + "S" + ".jpg";
                                    string type = "S";
                                    string UpdatePic = objDB.Updated_Bulk_Pic_Data_Open(Std_Id, PhotoName, type, SchlID);
                                    if (UpdatePic.ToString() == "1")
                                    {
                                        //Filepath = Server.MapPath("~/Upload/" + FormName + "/" + DistID + "/Sign/");
                                        //Filepath = sp + "/" + FormName + "/" + DistID + "/Photo/";
                                        //Filepath = sp1 + "/" + FormName + "/Sign/";
                                        Filepath = sp1 + "Correction/" + FormName;
                                        if (!Directory.Exists(Filepath))
                                        {
                                            Directory.CreateDirectory(Filepath);
                                        }
                                        SaveFile = SaveFile + file.FileName + ",";
                                        //path = Filepath + (FormName + DistID + file.FileName);
                                        //path = Path.Combine(Server.MapPath("~/Upload/" + FormName + "/" + DistID + "/Sign"), Std_Id + "S" + ".jpg");
                                        //path = sp + "/" + FormName + "/" + DistID + "/Photo" + "/" + Std_Id + "S" + ".jpg";
                                        //path = sp1 + "/" + FormName + "/Sign" + "/" + Std_Id + "S" + ".jpg";
                                        path = sp1 + "Correction/" + FormName + "/" + Std_Id + "S" + ".jpg";
                                        dt.Rows.Add(file.FileName);
                                        file.SaveAs(path);
                                        countS = countS + 1;
                                    }
                                    else
                                    {
                                        Incorrectfile = Incorrectfile + file.FileName + ", ";
                                    }
                                }

                            }

                        }
                        if (SaveFileN != "" && SaveFileNextn != "")
                        {
                            @ViewBag.msgSE = "Image not uploaded : Size [ " + SaveFileN + " ]  Extension [ " + SaveFileNextn + " ];";
                        }
                        else if (SaveFileN != "")
                        {
                            @ViewBag.msgS = "Image size not ok : File Name [ " + SaveFileN + " ]";
                        }
                        if (SaveFileNextn != "")
                        {
                            @ViewBag.msgE = "File name & Extension not ok : File Name [ " + SaveFileNextn + " ]";
                        }
                        if (Incorrectfile.ToString() != "")
                        {
                            @ViewBag.msgInC = "Student details not exist : File Name [ " + Incorrectfile + " ]";
                        }
                        if (countP > 0 || countS > 0)
                        {
                            @ViewBag.msgOK = "Successfully uploaed: Photo = " + countP + " Sign = " + countS;
                        }
                    }
                    else
                    {

                        SaveFileN = string.Empty;
                        @ViewBag.msg = "Check Image";
                    }

                }
                else
                {
                    @ViewBag.msg = "Select Image and Form Name";
                }

                HttpPostedFileBase FileUploadBulk2 = FileUploadBulk;
                if (FileUploadBulk2 != null && FormName != "" && FormName != "Open")
                {

                    string DistID = string.Empty;
                    string Std_Id = string.Empty;
                    string Filepath = string.Empty;
                    string path;
                    string SchlID = string.Empty;
                    string filename = string.Empty;
                    string SaveFile = string.Empty;
                    string SaveFileN = string.Empty;
                    string SaveFileNextn = string.Empty;
                    string Incorrectfile = string.Empty;
                    int countP = 0; int countS = 0;
                    SchlID = Convert.ToString(Session["SCHL"]);
                    if (SchlID == null || SchlID == "")
                    {
                        return RedirectToAction("Index", "Login");
                    }

                    if (FileUploadBulk2.ContentLength > 0)
                    {

                        HttpFileCollectionBase files = Request.Files;
                        DataTable dt = new DataTable { Columns = { new DataColumn("Path") } };
                        SchoolModels sm = new SchoolModels();
                        AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            string Prefix = string.Empty;
                            if (file.FileName.Length <= 14)
                            {
                                int lname = file.FileName.Length;
                                lname = 14 - lname;
                                switch (lname)
                                {
                                    case 1: Prefix = "0"; break;
                                    case 2: Prefix = "00"; break;
                                    case 3: Prefix = "000"; break;
                                    case 4: Prefix = "0000"; break;
                                    case 5: Prefix = "00000"; break;
                                    case 6: Prefix = "000000"; break;
                                }


                            }
                            string fname = Prefix + file.FileName.ToString();
                            if (file.ContentType != "image/jpeg" || fname.Length > 14 || fname.Length < 6 || (fname.Substring(9, 1).ToUpper() != "P" && fname.Substring(9, 1).ToUpper() != "S"))
                            {
                                SaveFileNextn = SaveFileNextn + file.FileName + ", ";
                            }

                            else if (file.ContentLength > 50000 || file.ContentLength < 5000)
                            {
                                SaveFileN = SaveFileN + file.FileName + ", ";
                            }
                            else
                            {
                                //filename = file.FileName.Substring(0, 7);
                                filename = fname.Substring(0, 9);
                                //DistID = "150";
                                DistID = Session["SCHOOLDIST"].ToString();   // Add Dist ID
                                Std_Id = FormName + DistID + filename;
                                if (fname.Substring(9, 1).ToUpper() == "P")
                                {
                                    int OutStatus = 0;
                                    string PhotoName = FormName + "/" + DistID + "/Photo" + "/" + Std_Id + "P" + ".jpg";
                                    string type = "P";
                                    string UpdatePic = objDB.Updated_Bulk_Pic_Data(Std_Id, PhotoName, type, SchlID, out OutStatus);
                                    if (OutStatus.ToString() == "1")
                                    {
                                        // Filepath = Server.MapPath("~/Upload/" + FormName + "/" + DistID + "/Photo/");
                                        Filepath = sp + "/" + FormName + "/" + DistID + "/Photo/";
                                        if (!Directory.Exists(Filepath))
                                        {
                                            Directory.CreateDirectory(Filepath);
                                        }
                                        SaveFile = SaveFile + file.FileName + ", ";
                                        //path = Filepath + (FormName + DistID + file.FileName);
                                        // path = Path.Combine(Server.MapPath("~/Upload/" + FormName + "/" + DistID + "/Photo"), Std_Id + "P" + ".jpg");
                                        path = sp + "/" + FormName + "/" + DistID + "/Photo" + "/" + Std_Id + "P" + ".jpg";
                                        dt.Rows.Add(file.FileName);
                                        file.SaveAs(path);
                                        countP = countP + 1;
                                    }
                                    else
                                    {
                                        Incorrectfile = Incorrectfile + file.FileName + ", ";
                                    }
                                }
                                else if (fname.Substring(9, 1).ToUpper() == "S")
                                {
                                    int OutStatus = 0;
                                    string PhotoName = FormName + "/" + DistID + "/Sign" + "/" + Std_Id + "S" + ".jpg";
                                    string type = "S";
                                    string UpdatePic = objDB.Updated_Bulk_Pic_Data(Std_Id, PhotoName, type, SchlID, out OutStatus);
                                    if (OutStatus.ToString() == "1")
                                    {
                                        // Filepath = Server.MapPath("~/Upload/" + FormName + "/" + DistID + "/Sign/");
                                        Filepath = sp + "/" + FormName + "/" + DistID + "/Photo/";
                                        if (!Directory.Exists(Filepath))
                                        {
                                            Directory.CreateDirectory(Filepath);
                                        }
                                        SaveFile = SaveFile + file.FileName + ",";
                                        //path = Filepath + (FormName + DistID + file.FileName);
                                        //path = Path.Combine(Server.MapPath("~/Upload/" + FormName + "/" + DistID + "/Sign"), Std_Id + "S" + ".jpg");
                                        path = sp + "/" + FormName + "/" + DistID + "/Photo" + "/" + Std_Id + "S" + ".jpg";
                                        dt.Rows.Add(file.FileName);
                                        file.SaveAs(path);
                                        countS = countS + 1;
                                    }
                                    else
                                    {
                                        Incorrectfile = Incorrectfile + file.FileName + ", ";
                                    }
                                }

                            }

                        }
                        if (SaveFileN != "" && SaveFileNextn != "")
                        {
                            @ViewBag.msgSE = "Image not uploaded : Size [ " + SaveFileN + " ]  Extension [ " + SaveFileNextn + " ];";
                        }
                        else if (SaveFileN != "")
                        {
                            @ViewBag.msgS = "Image size not ok : File Name [ " + SaveFileN + " ]";
                        }
                        if (SaveFileNextn != "")
                        {
                            @ViewBag.msgE = "File name & Extension not ok : File Name [ " + SaveFileNextn + " ]";
                        }
                        if (Incorrectfile.ToString() != "")
                        {
                            @ViewBag.msgInC = "Student details not exist : File Name [ " + Incorrectfile + " ]";
                        }
                        if (countP > 0 || countS > 0)
                        {
                            @ViewBag.msgOK = "Successfully uploaed: Photo = " + countP + " Sign = " + countS;
                        }
                    }
                    else
                    {

                        SaveFileN = string.Empty;
                        @ViewBag.msg = "Check Image";
                    }

                }
                else
                {
                    @ViewBag.msg = "Select Image and Form Name";
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

        #endregion Bulk Photo Upload Open

        #region Private Members        
        private const int PageSize = 100;
        private const int PageSize1 = 10;
        private const int FirstPageIndex = 1;
        private int? _fullCategoryCount;
        #endregion

        #region Public Properties
        //public int FullCategoryCount
        //{
        //    get
        //    {
        //        if (_fullCategoryCount == null)
        //        {
        //            string Search = string.Empty;
        //            Search = "sm.Id like '%' ";
        //            _fullCategoryCount = objCommon.CountTableRowsMaster(1); // 1 for schoolmaster                 
        //        }

        //        return (int)_fullCategoryCount;
        //    }
        //}
        #endregion

        #region Private Methods

        private int getLastPageIndex(int FullCategoryCount)
        {
            // int lastPageIndex = this.FullCategoryCount / PageSize;
            int lastPageIndex = FullCategoryCount / PageSize;
            if (FullCategoryCount % PageSize > 0)
                lastPageIndex += 1;
            return lastPageIndex;
        }

        #endregion

        public JsonResult getPunjabiName(string text)
        {
            ViewBag.Name = objCommon.getPunjabiName(text);
            return Json(ViewBag.Name);
        }

        [HttpPost]
        public ActionResult ViewSchoollist(RegistrationModels sm)
        {
            return RedirectToAction("Schoollist", "School");
        }

        public ActionResult Schoollist(SchoolModels asm)
        {
            try
            {
                //{
                TempData["schoollist"] = "schoolist";
                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                ViewBag.MyDist = objCommon.GetDistE();
                ViewBag.MySch = objCommon.SearchSchoolItems();
                ViewBag.MySchoolType = objCommon.GetSchool();
                //ViewBag.MyClassType = objCommon.GetClass();

                ViewBag.PreviousPageIndex = 0;
                ViewBag.CurrentPageIndex = FirstPageIndex;
                if (ViewBag.MyDist == null)
                {
                    ModelState.AddModelError("", "District Not Found");
                    return View();
                }
                else
                {
                    ViewBag.SelectedDist = "";
                    ViewBag.SelectedItem = "";
                    ViewBag.SelectedSchoolType = "";
                    ViewBag.SelectedClassType = "";
                    SchoolModels ASM = new SchoolModels();
                    ViewBag.TotalCount = 0;
                    return View(asm);
                    //if (asm.StoreAllData == null)
                    //{
                    //    string Search = string.Empty;
                    //    Search = "sm.Id like '%' ";
                    //    ASM.StoreAllData = objDB.SearchSchoolDetailsPaging(Search, FirstPageIndex, PageSize1);
                    //    if (ASM.StoreAllData != null)
                    //    {
                    //        // ASM.StoreAllData = objDB.SearchSchoolDetails(Search);
                    //        // ViewBag.TotalCount = ASM.StoreAllData.Tables[0].Rows.Count;
                    //        ViewBag.TotalCount = Convert.ToInt32(ASM.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]);
                    //        ViewBag.LastPageIndex = this.getLastPageIndex(Convert.ToInt32(ASM.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]));
                    //    }
                    //    return View(ASM);
                    //}
                    //else
                    //{
                    //    ViewBag.TotalCount = asm.StoreAllData.Tables[0].Rows.Count;
                    //    return View(asm);
                    //}

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
        public ActionResult Schoollist(FormCollection frm)
        {
            try
            {
                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                SchoolModels asm = new SchoolModels();
                if (ModelState.IsValid)
                {
                    ViewBag.MySchoolType = objCommon.GetSchool();
                    ViewBag.MyClassType = objCommon.GetClass();
                    // bind Dist 
                    ViewBag.MyDist = objCommon.GetDistE();
                    ViewBag.MySch = objCommon.SearchSchoolItems();
                    string Search = string.Empty;
                    Search = "sm.Id like '%' ";
                    TempData["schoollist"] = "schoollist";
                    if (frm["Dist1"] != "")
                    {
                        ViewBag.SelectedDist = frm["Dist1"];
                        TempData["SelectedDist"] = frm["Dist1"];
                        Search += " and sm.dist=" + frm["Dist1"].ToString();
                    }

                    if (frm["schooltype"] != "")
                    {
                        ViewBag.selectedschooltype = frm["schooltype"];
                        TempData["selectedschooltype"] = frm["schooltype"];
                        Search += " and st.schooltype='" + frm["schooltype"].ToString() + "'";
                    }

                    //if (frm["ClassType"] != "")
                    //{
                    //    ViewBag.SelectedClassType = frm["ClassType"];
                    //    TempData["SelectedClassType"] = frm["ClassType"];
                    //    Search += " and sm.class=" + frm["ClassType"].ToString();
                    //}

                    if (frm["Sch1"] != "")
                    {
                        ViewBag.SelectedItem = frm["Sch1"];
                        TempData["SelectedItem"] = frm["Sch1"];
                        int SelValueSch = Convert.ToInt32(frm["Sch1"].ToString());

                        if (frm["SearchString"] != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and sm.SCHL=" + frm["SearchString"].ToString(); }
                            else if (SelValueSch == 2)
                            { Search += " and  sm.SCHLE like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 3)
                            { Search += "and sm.IDNO='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 4)
                            { Search += " and sm.STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 5)
                            { Search += " and sm.SCHLE=" + frm["SearchString"].ToString(); }
                        }

                    }
                    ViewBag.PreviousPageIndex = 0;
                    ViewBag.CurrentPageIndex = FirstPageIndex;
                    TempData["SearchSchoolData"] = Search;
                    TempData.Keep(); // to store search value for view

                    asm.StoreAllData = objDB.SearchSchoolDetailsPaging(Search, FirstPageIndex, PageSize1);

                    if (asm.StoreAllData == null || asm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.LastPageIndex = 0;
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        // ViewBag.TotalCount = asm.StoreAllData.Tables[0].Rows.Count;                                           
                        ViewBag.TotalCount = Convert.ToInt32(asm.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]);
                        ViewBag.LastPageIndex = this.getLastPageIndex(Convert.ToInt32(asm.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]));

                        return View(asm);
                    }
                }
                else
                {
                    return View(asm);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        [Route("Schoollist/Next")]
        public ActionResult N(int currentPageIndex)
        {
            try
            {
                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                SchoolModels ASM = new SchoolModels();
                ViewBag.MyDist = objCommon.GetDistE();
                ViewBag.MySch = objCommon.SearchSchoolItems();
                ViewBag.MySchoolType = objCommon.GetSchool();
                ViewBag.MyClassType = objCommon.GetClass();
                int startIndex = (currentPageIndex * PageSize1) + 1;
                int endIndex = startIndex + PageSize1 - 1;
                ViewBag.CurrentPageIndex = currentPageIndex + 1;
                ViewBag.PreviousPageIndex = currentPageIndex;

                string Search = string.Empty;
                if (TempData["SearchSchoolData"] == null)
                { Search = "sm.Id like '%' "; }
                else
                {
                    ViewBag.SelectedDist = TempData["SelectedDist"];
                    ViewBag.SelectedItem = TempData["SelectedItem"];
                    ViewBag.SelectedSchoolType = TempData["SelectedSchoolType"];
                    ViewBag.SelectedClassType = TempData["SelectedClassType"];
                    Search = TempData["SearchSchoolData"].ToString();
                    TempData.Keep();
                }

                ASM.StoreAllData = objDB.SearchSchoolDetailsPaging(Search, startIndex, endIndex);
                if (ASM.StoreAllData != null)
                {
                    ViewBag.TotalCount = Convert.ToInt32(ASM.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]);
                    ViewBag.LastPageIndex = this.getLastPageIndex(Convert.ToInt32(ASM.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]));
                }
                return View("schoollist", ASM);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        [Route("Schoollist/Previous")]
        public ActionResult P(int previousPageIndex)
        {
            try
            {
                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                SchoolModels ASM = new SchoolModels();
                ViewBag.MyDist = objCommon.GetDistE();
                ViewBag.MySch = objCommon.SearchSchoolItems();
                ViewBag.MySchoolType = objCommon.GetSchool();
                ViewBag.MyClassType = objCommon.GetClass();

                int endIndex = previousPageIndex * PageSize1;
                int startIndex = endIndex - (PageSize1 - 1);
                ViewBag.CurrentPageIndex = previousPageIndex;
                ViewBag.PreviousPageIndex = previousPageIndex - 1;

                string Search = string.Empty;
                if (TempData["SearchSchoolData"] == null)
                { Search = "sm.Id like '%' "; }
                else
                {
                    ViewBag.SelectedDist = TempData["SelectedDist"];
                    ViewBag.SelectedItem = TempData["SelectedItem"];
                    ViewBag.SelectedSchoolType = TempData["SelectedSchoolType"];
                    ViewBag.SelectedClassType = TempData["SelectedClassType"];
                    Search = TempData["SearchSchoolData"].ToString();
                    TempData.Keep();
                }
                ASM.StoreAllData = objDB.SearchSchoolDetailsPaging(Search, startIndex, endIndex);
                if (ASM.StoreAllData != null)
                {
                    // ViewBag.TotalCount = ASM.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount = Convert.ToInt32(ASM.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]);
                    ViewBag.LastPageIndex = this.getLastPageIndex(Convert.ToInt32(ASM.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]));
                }
                return View("schoollist", ASM);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }


        // GET: School 
        public ActionResult Index()
        {
            //return View();
            return RedirectToAction("Admin_School_Master", "School");
        }


        [Route("Admin_School_Master/Next")]
        public ActionResult Next(int currentPageIndex)
        {
            try
            {
                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                SchoolModels ASM = new SchoolModels();
                ViewBag.MyDist = objCommon.GetDistE();
                ViewBag.MySch = objCommon.SearchSchoolItems();
                ViewBag.MySchoolType = objCommon.GetSchool();
                ViewBag.MyClassType = objCommon.GetClass();
                int startIndex = (currentPageIndex * PageSize) + 1;
                int endIndex = startIndex + PageSize - 1;
                ViewBag.CurrentPageIndex = currentPageIndex + 1;
                ViewBag.PreviousPageIndex = currentPageIndex;

                string Search = string.Empty;
                if (TempData["SearchSchoolData"] == null)
                { Search = "sm.Id like '%' "; }
                else
                {
                    ViewBag.SelectedDist = TempData["SelectedDist"];
                    ViewBag.SelectedItem = TempData["SelectedItem"];
                    ViewBag.SelectedSchoolType = TempData["SelectedSchoolType"];
                    ViewBag.SelectedClassType = TempData["SelectedClassType"];
                    Search = TempData["SearchSchoolData"].ToString();
                    TempData.Keep();
                }


                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsModiFy = 1; ViewBag.IsDelete = 1; ViewBag.IsView = 1; ViewBag.IsModiFyOpen = 1; }
                else
                {

                    string actionName = "Admin_School_Master";
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    string AdminType = Session["AdminType"].ToString();
                    //GetActionOfSubMenu(string cont, string act)
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        bool exists = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("MODIFY")).Count() > 0;
                        ViewBag.IsModiFy = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("MODIFY")).Count();
                        ViewBag.IsDelete = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("DELETE")).Count();
                        ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("VIEW")).Count();
                        ViewBag.IsModiFyOpen = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("MODIFYOPEN")).Count();


                    }
                }
                #endregion Action Assign Method



                ASM.StoreAllData = objDB.SearchSchoolDetailsPaging(Search, startIndex, endIndex);
                if (ASM.StoreAllData != null)
                {
                    ViewBag.TotalCount = Convert.ToInt32(ASM.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]);
                    ViewBag.LastPageIndex = this.getLastPageIndex(Convert.ToInt32(ASM.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]));
                }
                return View("Admin_School_Master", ASM);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        [Route("Admin_School_Master/Previous")]
        public ActionResult Previous(int previousPageIndex)
        {
            try
            {
                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                SchoolModels ASM = new SchoolModels();
                ViewBag.MyDist = objCommon.GetDistE();
                ViewBag.MySch = objCommon.SearchSchoolItems();
                ViewBag.MySchoolType = objCommon.GetSchool();
                ViewBag.MyClassType = objCommon.GetClass();

                int endIndex = previousPageIndex * PageSize;
                int startIndex = endIndex - (PageSize - 1);
                ViewBag.CurrentPageIndex = previousPageIndex;
                ViewBag.PreviousPageIndex = previousPageIndex - 1;

                string Search = string.Empty;
                if (TempData["SearchSchoolData"] == null)
                { Search = "sm.Id like '%' "; }
                else
                {
                    ViewBag.SelectedDist = TempData["SelectedDist"];
                    ViewBag.SelectedItem = TempData["SelectedItem"];
                    ViewBag.SelectedSchoolType = TempData["SelectedSchoolType"];
                    ViewBag.SelectedClassType = TempData["SelectedClassType"];
                    Search = TempData["SearchSchoolData"].ToString();
                    TempData.Keep();
                }

                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsModiFy = 1; ViewBag.IsDelete = 1; ViewBag.IsView = 1; ViewBag.IsModiFyOpen = 1; ViewBag.IsPrint = 1; }
                else
                {

                    string actionName = "Admin_School_Master";
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    string AdminType = Session["AdminType"].ToString();
                    //GetActionOfSubMenu(string cont, string act)
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        bool exists = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("MODIFY")).Count() > 0;
                        ViewBag.IsModiFy = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("MODIFY")).Count();
                        ViewBag.IsDelete = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("DELETE")).Count();
                        ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("VIEW")).Count();
                        ViewBag.IsModiFyOpen = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("MODIFYOPEN")).Count();
                    }
                }
                #endregion Action Assign Method



                ASM.StoreAllData = objDB.SearchSchoolDetailsPaging(Search, startIndex, endIndex);
                if (ASM.StoreAllData != null)
                {
                    // ViewBag.TotalCount = ASM.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount = Convert.ToInt32(ASM.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]);
                    ViewBag.LastPageIndex = this.getLastPageIndex(Convert.ToInt32(ASM.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]));
                }
                return View("Admin_School_Master", ASM);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }



        public ActionResult Admin_School_Master(SchoolModels asm, int? page)
        {
            if (page == null)
            {
                TempData["SearchSchoolData"] = null;
            }
            try
            {
                if (Session["AdminId"] == null)
                { return RedirectToAction("Index", "Admin"); }
                else
                {
                    // Dist Allowed
                    string DistAllow = "";
                    if (ViewBag.DistAllow == null)
                    { return RedirectToAction("Index", "Admin"); }
                    else
                    { DistAllow = ViewBag.DistAllow; }
                    if (ViewBag.DistUser == null)
                    { ViewBag.MyDist = null; }
                    else
                    {
                        ViewBag.MyDist = ViewBag.DistUser;
                    }

                    #region Action Assign Method
                    if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                    { ViewBag.IsLink = 1; ViewBag.IsModiFy = 1; ViewBag.IsDelete = 1; ViewBag.IsView = 1; ViewBag.IsModiFyOpen = 1; ViewBag.IsPrint = 1; }
                    else
                    {

                        string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                        string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                        int AdminId = Convert.ToInt32(Session["AdminId"]);
                        //string AdminType = Session["AdminType"].ToString();
                        //GetActionOfSubMenu(string cont, string act)
                        DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                        if (aAct.Tables[0].Rows.Count > 0)
                        {
                            ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("SCHOOL/SCHOOL_VIEW_FORM")).Count();
                            ViewBag.IsModiFy = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("SCHOOL/SCHOOL_MODIFICATION_FORM")).Count();
                            ViewBag.IsModiFyOpen = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("SCHOOL/OPEN_SCHOOL_MODIFICATION_FORM")).Count();
                            ViewBag.IsPrint = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/AFFILIATIONCERTIFICATE")).Count();
                            ViewBag.IsLink = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("SCHOOL/LINKOTHERSCHOOL")).Count();
                        }
                    }
                    #endregion Action Assign Method




                    AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                    //// ViewBag.MyDist = objCommon.GetDistE();
                    ViewBag.MySch = objCommon.SearchSchoolItems();
                    ViewBag.MySchoolType = objCommon.GetSchool();
                    ViewBag.MyClassType = objCommon.GetClass();

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;

                    if (ViewBag.MyDist == null)
                    {
                        ModelState.AddModelError("", "District Not Found");
                        return View();
                    }
                    else
                    {
                        ViewBag.SelectedDist = "";
                        ViewBag.SelectedItem = "";
                        ViewBag.SelectedSchoolType = "";
                        ViewBag.SelectedClassType = "";
                        SchoolModels ASM = new SchoolModels();


                        TempData.Keep();
                        if (TempData["SearchSchoolData"] != null)
                        {
                            string Search = TempData["SearchSchoolData"].ToString();

                            ViewBag.SelectedDist = TempData["SelectedDist"];
                            ViewBag.SelectedSchoolType = TempData["SelectedSchoolType"];
                            ViewBag.SelectedClassType = TempData["SelectedClassType"];
                            ViewBag.SelectedItem = TempData["SelectedItem"];


                            if (DistAllow != "")
                            {
                                Search += " and sm.DIST in (" + DistAllow + ")";
                            }
                            ASM.StoreAllData = objDB.SearchSchoolDetailsPaging(Search, pageIndex, 30);
                            if (ASM.StoreAllData == null || ASM.StoreAllData.Tables[0].Rows.Count == 0)
                            {
                                ViewBag.Message = "Record Not Found";
                                ViewBag.TotalCount1 = ViewBag.TotalCount = 0;
                                return View();
                            }
                            else
                            {

                                ViewBag.TotalCount = ASM.StoreAllData.Tables[0].Rows.Count;
                                ViewBag.TotalCount1 = Convert.ToInt32(ASM.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]);
                                int tp = Convert.ToInt32(ASM.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]);
                                int pn = tp / 30;
                                int cal = 30 * pn;
                                int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                                if (res >= 1)
                                    ViewBag.pn = pn + 1;
                                else
                                    ViewBag.pn = pn;

                            }
                            return View(ASM);
                        }
                        else
                        {
                            //ViewBag.TotalCount = asm.StoreAllData.Tables[0].Rows.Count;
                            ViewBag.TotalCount = 0;
                            return View(asm);
                        }
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
        public ActionResult Admin_School_Master(FormCollection frm, int? page)
        {
            try
            {

                // Dist Allowed
                string DistAllow = "";
                if (ViewBag.DistAllow == null)
                { return RedirectToAction("Index", "Admin"); }
                else
                { DistAllow = ViewBag.DistAllow; }
                if (ViewBag.DistUser == null)
                { ViewBag.MyDist = null; }
                else
                {
                    ViewBag.MyDist = ViewBag.DistUser;
                }
                // End Dist Allowed

                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsLink = 1; ViewBag.IsModiFy = 1; ViewBag.IsDelete = 1; ViewBag.IsView = 1; ViewBag.IsModiFyOpen = 1; ViewBag.IsPrint = 1; }
                else
                {

                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    //string AdminType = Session["AdminType"].ToString();
                    //GetActionOfSubMenu(string cont, string act)
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("SCHOOL/SCHOOL_VIEW_FORM")).Count();
                        ViewBag.IsModiFy = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("SCHOOL/SCHOOL_MODIFICATION_FORM")).Count();
                        ViewBag.IsModiFyOpen = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("SCHOOL/OPEN_SCHOOL_MODIFICATION_FORM")).Count();
                        ViewBag.IsPrint = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/AFFILIATIONCERTIFICATE")).Count();
                        ViewBag.IsLink = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("SCHOOL/LINKOTHERSCHOOL")).Count();
                    }
                }
                #endregion Action Assign Method



                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                SchoolModels asm = new SchoolModels();
                if (ModelState.IsValid)
                {
                    ViewBag.MySchoolType = objCommon.GetSchool();
                    ViewBag.MyClassType = objCommon.GetClass();
                    // bind Dist 
                    ////  ViewBag.MyDist = objCommon.GetDistE();
                    ViewBag.MySch = objCommon.SearchSchoolItems();
                    string Search = string.Empty;
                    //Search = "sm.Id like '%' ";
                    Search = "sm.Id >0 ";
                    if (frm["Dist1"] != "")
                    {
                        ViewBag.SelectedDist = frm["Dist1"];
                        TempData["SelectedDist"] = frm["Dist1"];
                        Search += " and sm.dist=" + frm["Dist1"].ToString();
                    }

                    if (frm["SchoolType"] != "")
                    {
                        ViewBag.SelectedSchoolType = frm["SchoolType"];
                        TempData["SelectedSchoolType"] = frm["SchoolType"];
                        Search += " and st.schooltype='" + frm["SchoolType"].ToString() + "'";
                    }

                    if (frm["ClassType"] != "")
                    {
                        ViewBag.SelectedClassType = frm["ClassType"];
                        TempData["SelectedClassType"] = frm["ClassType"];
                        Search += " and sm.class=" + frm["ClassType"].ToString();
                    }

                    if (frm["Sch1"] != "")
                    {
                        ViewBag.SelectedItem = frm["Sch1"];
                        TempData["SelectedItem"] = frm["Sch1"];
                        int SelValueSch = Convert.ToInt32(frm["Sch1"].ToString());

                        if (frm["SearchString"] != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and sm.SCHL=" + frm["SearchString"].ToString(); }
                            else if (SelValueSch == 2)
                            { Search += " and  sm.SCHLE like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 3)
                            { Search += " and sm.IDNO='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 4)
                            { Search += " and sm.STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 5)
                            { Search += " and sm.SCHLE like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 6)
                            { Search += " and sm.udisecode='" + frm["SearchString"].ToString() + "'"; }
                        }

                    }


                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;


                    TempData["SearchSchoolData"] = Search;
                    TempData.Keep(); // to store search value for view
                    if (DistAllow != "")
                    {
                        Search += " and sm.DIST in (" + DistAllow + ")";
                    }
                    asm.StoreAllData = objDB.SearchSchoolDetailsPaging(Search, pageIndex, 30);

                    if (asm.StoreAllData == null || asm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        // ViewBag.TotalCount = asm.StoreAllData.Tables[0].Rows.Count;                                           
                        //ViewBag.TotalCount = Convert.ToInt32(asm.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]);
                        //ViewBag.LastPageIndex = this.getLastPageIndex(Convert.ToInt32(asm.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]));


                        ViewBag.TotalCount = asm.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(asm.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]);
                        int tp = Convert.ToInt32(asm.StoreAllData.Tables[1].Rows[0]["LastPageIndex"]);
                        int pn = tp / 30;
                        int cal = 30 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        return View(asm);
                    }
                }
                else
                {
                    return View(asm);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }



        //
        #region begin CutList
        public ActionResult SchoolCutList()
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            string SCHL = Session["SCHL"].ToString();
            DataSet result = objCommon.schooltypes(SCHL); // passing Value to DBClass from model
            if (result == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (result.Tables[1].Rows.Count > 0)
            {
                ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
                ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
            }
            return View();
        }

        public ActionResult CutList(string id)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            if (id == "")
            {
                return RedirectToAction("SchoolCutList", "School");
            }

            ViewBag.CutlistClass = id;
            string SCHL = Session["SCHL"].ToString();
            string class1 = "";
            string Type1 = "";
            if (id.Contains("E"))
            {
                ViewBag.Status = "0";
            }
            else
            {
                ViewBag.Status = "1";
                if (id == "M")
                {
                    class1 = "2";
                    Type1 = "REG";
                }
                else if (id == "MO")
                {
                    class1 = "10";
                    Type1 = "OPEN";
                }
                else if (id == "S")
                {
                    class1 = "4";
                    Type1 = "REG";
                }
                else if (id == "SO")
                {
                    class1 = "12";
                    Type1 = "OPEN";
                }

                int OutStatus = 0;
                DataSet result = objDB.UpdateCutListSubjects(SCHL, class1, Type1, out OutStatus); // passing Value to DBClass from model
                if (OutStatus == 0)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
        #endregion  CutList

        //

        public ActionResult School_Master()
        {
            return View();
        }

        [AdminLoginCheckFilter]
        public ActionResult Add_School()
        {
            try
            {
                AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                SchoolModels sm = new SchoolModels();
                ViewBag.CountList = objCommon.GetCountList();
                // Area 
                ViewBag.AREAList = objCommon.GetArea();
                // YesNo 
                ViewBag.YesNoList = objCommon.GetYesNo();
                // Status
                ViewBag.StatusList = objCommon.GetStatus();
                // Session 
                // ViewBag.SessionList = objCommon.GetSessionAdmin();
                ViewBag.SessionList = objCommon.GetEstalimentYearList();
                // Class 
                ViewBag.ClassTypeList = objCommon.GetClass();
                // School 
                ViewBag.SchoolTypeList = objCommon.GetSchool();

                //new 
                ViewBag.SubUserTypeList = objCommon.GetSubUserType();
                ViewBag.EstalimentYearList = objCommon.GetEstalimentYearList();
                //


                ViewBag.SchoolType = objCommon.GetSchoolAbbr();
                // English Dist 
                ViewBag.DistEList = objCommon.GetDistE();
                // Punjabi   Dist         
                ViewBag.DistPList = objCommon.GetDistP();
                // SchoolYear 
                // ViewBag.SchoolYear = objCommon.GetSessionYearSchoolAdmin();
                ViewBag.SchoolYear = objCommon.GetEstalimentYearListSingle();

                ViewBag.SchoolSetMset = objDB.GetSchoolSetByType(1).ToList();
                ViewBag.SchoolSetMoSet = objDB.GetSchoolSetByType(2).ToList();
                ViewBag.SchoolSetSSet = objDB.GetSchoolSetByType(3).ToList();
                ViewBag.SchoolSetSoset = objDB.GetSchoolSetByType(4).ToList();


                string mydist = "0";
                DataSet result2 = objDB.SelectBlock(mydist);
                ViewBag.MyEdublock = result2.Tables[0];
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


                //List<SelectListItem> itemSession = objCommon.GetSessionYear().ToList();
                //itemSession.Add(new SelectListItem { Text = "2017", Value = "2017" });
                // ViewBag.SchoolYear = itemSession.ToList();
                // Tehsil
                // ViewBag.MyTeh = objCommon.GetAllTehsil();
                ViewBag.MyTeh = "";

                return View(sm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult Add_School(SchoolModels sm, FormCollection frm)
        {
            try
            {
                AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

                //new 
                ViewBag.SubUserTypeList = objCommon.GetSubUserType();
                ViewBag.EstalimentYearList = objCommon.GetEstalimentYearList();
                //

                ViewBag.CountList = objCommon.GetCountList();
                // Area 
                ViewBag.AREAList = objCommon.GetArea();
                // YesNo 
                ViewBag.YesNoList = objCommon.GetYesNo();
                // Status
                ViewBag.StatusList = objCommon.GetStatus();
                // Session 
                // ViewBag.SessionList = objCommon.GetSessionAdmin();
                ViewBag.SessionList = objCommon.GetEstalimentYearList();
                // Class 
                ViewBag.ClassTypeList = objCommon.GetClass();
                // School 
                ViewBag.SchoolTypeList = objCommon.GetSchool();
                ViewBag.SchoolType = objCommon.GetSchoolAbbr();
                // English Dist 
                ViewBag.DistEList = objCommon.GetDistE();
                // Punjabi   Dist         
                ViewBag.DistPList = objCommon.GetDistP();
                // SchoolYear 
                // ViewBag.SchoolYear = objCommon.GetSessionYearSchoolAdmin();
                ViewBag.SchoolYear = objCommon.GetEstalimentYearListSingle();
                // Tehsil
                ViewBag.MyTeh = objCommon.GetAllTehsil();

                ViewBag.SchoolSetMset = objDB.GetSchoolSetByType(1).ToList();
                ViewBag.SchoolSetMoSet = objDB.GetSchoolSetByType(2).ToList();
                ViewBag.SchoolSetSSet = objDB.GetSchoolSetByType(3).ToList();
                ViewBag.SchoolSetSoset = objDB.GetSchoolSetByType(4).ToList();

                sm.DISTNM = objCommon.GetDistE().Where(i => i.Value == frm["DIST"].ToString()).Single().Text;

                sm.Tcode = sm.Tehsilp = sm.Tehsile = sm.Tehsile;


                string mydist = sm.dist.ToString();
                DataSet result2 = objDB.SelectBlock(mydist);
                ViewBag.MyEdublock = result2.Tables[0];
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


                if (frm["NSQF_flag"] != null)
                {
                    sm.NSQF_flag = frm["NSQF_flag"].ToString();
                    //sm.NSQF_flag = frm["NSQF_flag"].ToString() == "Y" ? true : false;
                }
                else { sm.NSQF_flag = ""; }

                if (Session["UserName"] != null)
                {
                    sm.username = Session["UserName"].ToString();
                }


                ViewBag.newschlcode = "";
                // DataTable ds= objCommon.GetAllSchoolType()
                string newschlcode;


                if (!string.IsNullOrEmpty(sm.SameAsSchl))
                {
                    if (sm.SameAsSchl.ToLower() == "yes")
                    {
                        sm.idno = "yes";
                    }
                }



                //#region Call API to insert school master details
                //string apiStatus = "";
                //SchoolApiViewModel savm = new SchoolApiViewModel();
                //try
                //{
                //    sm.userip = AbstractLayer.StaticDB.GetFullIPAddress();
                //    savm = new AbstractLayer.PsebAPIServiceDB().InsertSMFPSEBMainToPsebJunior(sm, out newschlcode);
                //    apiStatus = savm.statusCode;
                //    ViewBag.newschlcode = newschlcode;
                //    ViewBag.ApiStatus = apiStatus;
                //}
                //catch (Exception ex)
                //{
                //    ViewBag.newschlcode = "Error: " +  ex.Message;
                //    ViewBag.ApiStatus = apiStatus;
                //}
                //#endregion

                //int result = apiStatus == "200" ? 1 : 0;
                string EmpUserId = adminLoginSession.AdminEmployeeUserId;
                int result = objDB.InsertSMF(sm, 1, out newschlcode, EmpUserId); // passing Value to SchoolDB from model and Type 1 For regular
                if (result > 0)
                {
                    // ViewData["resultStatus"] = savm.Object.SCHL;
                    ViewData["resultStatus"] = newschlcode;
                    // ViewData["resultSMF"] = 1;
                    // return RedirectToAction("Admin_School_Master", "School");
                    ViewData["resultSMF"] = 1;
                    ViewBag.Message = "Your school information is Added successfully. Your correction number is " + result;
                    return View(sm);
                }
                else
                {
                    ViewData["resultStatus"] = result;
                    ViewData["resultSMF"] = 0;
                    ModelState.AddModelError("", "Not Inserted");
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


        [AdminLoginCheckFilter]
        public ActionResult School_Modification_Form(string id)
        {
            try
            {
                AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

                if (id == null)
                {
                    return RedirectToAction("Admin_School_Master", "School");
                }

                //new 
                ViewBag.SubUserTypeList = objCommon.GetSubUserType();
                ViewBag.EstalimentYearList = objCommon.GetEstalimentYearList();
                //

                ViewBag.CountList = objCommon.GetCountList();

                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB(); //calling class SchoolDB           
                                                                             // Area 
                ViewBag.AREAList = objCommon.GetArea();
                // YesNo 
                ViewBag.YesNoList = objCommon.GetYesNo();
                // Status
                ViewBag.StatusList = objCommon.GetStatus();
                // Session 
                // ViewBag.SessionList = objCommon.GetSessionAdmin();
                ViewBag.SessionList = objCommon.GetEstalimentYearList();
                // Class 
                ViewBag.ClassTypeList = objCommon.GetClass();
                // School 
                ViewBag.SchoolTypeList = objCommon.GetSchool();
                ViewBag.SchoolType = objCommon.GetSchoolAbbr();
                // English Dist 
                ViewBag.DistEList = objCommon.GetDistE();
                // Punjabi   Dist         
                ViewBag.DistPList = objCommon.GetDistP();

                // SchoolYear 
                // ViewBag.SchoolYear = objCommon.GetSessionYearSchoolAdmin();
                ViewBag.SchoolYear = objCommon.GetEstalimentYearListSingle();

                ViewBag.SchoolSetMset = objDB.GetSchoolSetByType(1).ToList();
                ViewBag.SchoolSetMoSet = objDB.GetSchoolSetByType(2).ToList();
                ViewBag.SchoolSetSSet = objDB.GetSchoolSetByType(3).ToList();
                ViewBag.SchoolSetSoset = objDB.GetSchoolSetByType(4).ToList();


                DataSet ds = objDB.SelectSchoolDatabyID(id);
                SchoolModels sm = new SchoolModels();
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    ViewData["resultSMF"] = 2;
                    return View();
                }
                else
                {

                    if (Session["Session"].ToString() != "2016-2017")
                    {
                        sm.dist = ds.Tables[0].Rows[0]["dist"].ToString();
                        string mydist = sm.dist.ToString();
                        DataSet result = objDB.SelectBlock(mydist);
                        ViewBag.MyEdublock = result.Tables[0];
                        List<SelectListItem> BlockList = new List<SelectListItem>();
                        //BlockList.Add(new SelectListItem { Text = "---Edu Block---", Value = "0" });
                        foreach (System.Data.DataRow dr in ViewBag.MyEdublock.Rows)
                        {
                            BlockList.Add(new SelectListItem { Text = @dr["Edu Block Name"].ToString(), Value = @dr["Edu Block Name"].ToString() });
                        }
                        ViewBag.MyEdublock = BlockList;
                    }
                    List<SelectListItem> EduClusterList = new List<SelectListItem>();
                    EduClusterList.Add(new SelectListItem { Text = "---Edu Cluster---", Value = "0" });
                    ViewBag.MyEduCluster = EduClusterList;



                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        sm.CorrectionNoOld = ds.Tables[1].Rows[0]["CorrectionNoOld"].ToString();
                        sm.RemarksOld = ds.Tables[1].Rows[0]["RemarksOld"].ToString();
                        sm.RemarkDateOld = ds.Tables[1].Rows[0]["RemarkDateOld"].ToString();
                        sm.REMARKS = string.Empty;
                    }
                    sm.Approved = ViewBag.Approved = ds.Tables[0].Rows[0]["IsApproved"].ToString();
                    sm.vflag = ViewBag.vflag = ds.Tables[0].Rows[0]["IsVerified"].ToString();

                    sm.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                    sm.SCHL = ds.Tables[0].Rows[0]["schl"].ToString();
                    sm.udisecode = ds.Tables[0].Rows[0]["udisecode"].ToString();
                    sm.idno = ds.Tables[0].Rows[0]["idno"].ToString();
                    sm.OCODE = ds.Tables[0].Rows[0]["OCODE"].ToString();
                    sm.AREA = ViewBag.AREA = ds.Tables[0].Rows[0]["Area"].ToString();
                    // sm.AREA =  ds.Tables[0].Rows[0]["SchoolArea"].ToString();                
                    sm.VALIDITY = ds.Tables[0].Rows[0]["VALIDITY"].ToString();
                    sm.dist = ds.Tables[0].Rows[0]["Dist"].ToString();
                    sm.session = ViewBag.Session = ds.Tables[0].Rows[0]["session"].ToString();
                    sm.status = ViewBag.Status = ds.Tables[0].Rows[0]["STATUS"].ToString();
                    sm.CLASS = ViewBag.Class = ds.Tables[0].Rows[0]["ClassID"].ToString();
                    // ViewBag.ClassID = ds.Tables[0].Rows[0]["ClassID"].ToString();
                    //   ViewBag.ClassID = new SelectList(itemClass, "Value", "Text", ds.Tables[0].Rows[0]["ClassID"].ToString());
                    // ViewBag.Session = new SelectList(itemSession, "Value", "Text", ds.Tables[0].Rows[0]["session"].ToString());
                    // ViewBag.Status = new SelectList(ds.Tables[0].Rows[0]["STATUS"].ToString(), ds.Tables[0].Rows[0]["STATUS"].ToString());
                    //ViewBag.USERTYPE = new SelectList(itemSchool, "Value", "Text", ds.Tables[0].Rows[0]["SchoolTypeID"].ToString());
                    //   sm.USERTYPE = ViewBag.USERTYPE = ds.Tables[0].Rows[0]["SchoolTypeID"].ToString();
                    sm.USERTYPE = ViewBag.USERTYPE = ds.Tables[0].Rows[0]["USERTYPE"].ToString();


                    sm.PASSWORD = ds.Tables[0].Rows[0]["PASSWORD"].ToString();
                    sm.SCHLE = ds.Tables[0].Rows[0]["SCHLE"].ToString();
                    sm.STATIONE = ds.Tables[0].Rows[0]["STATIONE"].ToString();
                    ViewBag.DIST = ds.Tables[0].Rows[0]["DIST"].ToString();
                    sm.DISTE = ViewBag.DISTE = ds.Tables[0].Rows[0]["DISTE"].ToString();
                    //   ViewBag.ODISTE = new SelectList(itemDist, "Value", "Text", ds.Tables[0].Rows[0]["DISTE"].ToString());
                    //ViewBag.ODISTP = new SelectList(itemDistP, "Value", "Text", ds.Tables[0].Rows[0]["DISTP"].ToString());
                    sm.SCHLP = ds.Tables[0].Rows[0]["SCHLP"].ToString();
                    sm.STATIONP = ds.Tables[0].Rows[0]["STATIONP"].ToString();
                    sm.DISTP = ds.Tables[0].Rows[0]["DISTP"].ToString();
                    sm.DISTNMPun = ds.Tables[0].Rows[0]["DISTNMPun"].ToString();
                    // ViewBag.ODISTP = new SelectList(itemDistP, "Value", "Text", ds.Tables[0].Rows[0]["DISTP"].ToString());

                    sm.PRINCIPAL = ds.Tables[0].Rows[0]["PRINCIPAL"].ToString();
                    sm.STDCODE = ds.Tables[0].Rows[0]["STDCODE"].ToString();
                    sm.PHONE = ds.Tables[0].Rows[0]["PHONE"].ToString();
                    sm.MOBILE = ds.Tables[0].Rows[0]["MOBILE"].ToString();
                    sm.EMAILID = ds.Tables[0].Rows[0]["EMAILID"].ToString();
                    sm.CONTACTPER = ds.Tables[0].Rows[0]["CONTACTPER"].ToString();
                    sm.OtContactno = ds.Tables[0].Rows[0]["OtContactno"].ToString();
                    sm.mobile2 = ds.Tables[0].Rows[0]["mobile2"].ToString();
                    //if (ds.Tables[0].Rows[0]["NSQF_flag"].ToString() == "false")
                    //{ sm.NSQF_flag = ViewBag.NSQF_flag =  "N"; }
                    //else
                    //{ sm.NSQF_flag = ViewBag.NSQF_flag = "0"; }

                    sm.NSQF_flag = ViewBag.NSQF_flag = ds.Tables[0].Rows[0]["NSQF_flag"].ToString();


                    sm.ADDRESSE = ds.Tables[0].Rows[0]["ADDRESSE"].ToString();
                    sm.ADDRESSP = ds.Tables[0].Rows[0]["ADDRESSP"].ToString();
                    // sm.ADDRESSE = ds.Tables[0].Rows[0]["ADDRESSEFUll"].ToString();
                    // sm.ADDRESSP = ds.Tables[0].Rows[0]["ADDRESSPFUll"].ToString();



                    //sm.REMARKS = ds.Tables[0].Rows[0]["Remarks"].ToString();
                    // ViewBag.OApproved = ds.Tables[0].Rows[0]["omiddle"].ToString());
                    ViewBag.OApproved = new SelectList(ds.Tables[0].Rows[0]["omiddle"].ToString(), ds.Tables[0].Rows[0]["omiddle"].ToString());
                    ViewBag.OVerified = new SelectList(ds.Tables[0].Rows[0]["omiddle"].ToString(), ds.Tables[0].Rows[0]["omiddle"].ToString());
                    // ViewBag.omiddle = new SelectList(ds.Tables[0].Rows[0]["omiddle"].ToString(), ds.Tables[0].Rows[0]["omiddle"].ToString());

                    sm.MSET = ViewBag.MSET = ds.Tables[0].Rows[0]["MSET"].ToString();
                    sm.MOSET = ViewBag.MOSET = ds.Tables[0].Rows[0]["MOSET"].ToString();
                    sm.SSET = ViewBag.SSET = ds.Tables[0].Rows[0]["SSET"].ToString();
                    sm.SOSET = ViewBag.SOSET = ds.Tables[0].Rows[0]["SOSET"].ToString();

                    //Regular

                    sm.middle = ViewBag.middle = ds.Tables[0].Rows[0]["middle"].ToString();
                    sm.MATRIC = ViewBag.MATRIC = ds.Tables[0].Rows[0]["MATRIC"].ToString();
                    sm.HUM = ViewBag.HUM = ds.Tables[0].Rows[0]["HUM"].ToString();
                    sm.SCI = ViewBag.SCI = ds.Tables[0].Rows[0]["SCI"].ToString();
                    sm.COMM = ViewBag.COMM = ds.Tables[0].Rows[0]["COMM"].ToString();
                    sm.VOC = ViewBag.VOC = ds.Tables[0].Rows[0]["VOC"].ToString();
                    sm.TECH = ViewBag.TECH = ds.Tables[0].Rows[0]["TECH"].ToString();
                    sm.AGRI = ViewBag.AGRI = ds.Tables[0].Rows[0]["AGRI"].ToString();

                    //Open
                    sm.omiddle = ViewBag.OMIDDLE = ds.Tables[0].Rows[0]["omiddle"].ToString();
                    sm.OMATRIC = ViewBag.OMATRIC = ds.Tables[0].Rows[0]["OMATRIC"].ToString();
                    sm.OHUM = ViewBag.OHUM = ds.Tables[0].Rows[0]["OHUM"].ToString();
                    sm.OSCI = ViewBag.OSCI = ds.Tables[0].Rows[0]["OSCI"].ToString();
                    sm.OCOMM = ViewBag.OCOMM = ds.Tables[0].Rows[0]["OCOMM"].ToString();
                    sm.OVOC = ViewBag.OVOC = ds.Tables[0].Rows[0]["OVOC"].ToString();
                    sm.OTECH = ViewBag.OTECH = ds.Tables[0].Rows[0]["OTECH"].ToString();
                    sm.OAGRI = ViewBag.OAGRI = ds.Tables[0].Rows[0]["OAGRI"].ToString();


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

                    sm.MID_UTYPE = ViewBag.MID_UTYPE = ds.Tables[0].Rows[0]["MID_UTYPE"].ToString();
                    sm.HID_UTYPE = ViewBag.HID_UTYPE = ds.Tables[0].Rows[0]["HID_UTYPE"].ToString();
                    sm.H_UTYPE = ViewBag.H_UTYPE = ds.Tables[0].Rows[0]["H_UTYPE"].ToString();
                    sm.S_UTYPE = ViewBag.S_UTYPE = ds.Tables[0].Rows[0]["S_UTYPE"].ToString();
                    sm.C_UTYPE = ViewBag.C_UTYPE = ds.Tables[0].Rows[0]["C_UTYPE"].ToString();
                    sm.V_UTYPE = ViewBag.V_UTYPE = ds.Tables[0].Rows[0]["V_UTYPE"].ToString();
                    sm.A_UTYPE = ViewBag.A_UTYPE = ds.Tables[0].Rows[0]["A_UTYPE"].ToString();
                    sm.T_UTYPE = ViewBag.T_UTYPE = ds.Tables[0].Rows[0]["T_UTYPE"].ToString();

                    sm.Tcode = ViewBag.Tcode = ds.Tables[0].Rows[0]["Tcode"].ToString();
                    sm.Tehsile = ViewBag.omiddle = ds.Tables[0].Rows[0]["Tehsile"].ToString();
                    sm.Tehsilp = ViewBag.omiddle = ds.Tables[0].Rows[0]["Tehsilp"].ToString();


                    //FIFTH
                    sm.fifth = ViewBag.fifth = ds.Tables[0].Rows[0]["fifth"].ToString();
                    sm.FIF_YR = ViewBag.FIF_YR = ds.Tables[0].Rows[0]["FIF_YR"].ToString();
                    sm.FIF_UTYPE = ds.Tables[0].Rows[0]["FIF_UTYPE"].ToString();
                    sm.FIF_S = ViewBag.FIF_S = Convert.ToString(ds.Tables[0].Rows[0]["FIF_S"].ToString());
                    sm.lclass = ViewBag.lclass = ds.Tables[0].Rows[0]["lclass"].ToString();
                    sm.uclass = ViewBag.uclass = ds.Tables[0].Rows[0]["uclass"].ToString();

                    // rm.MyDistrict = ds.Tables[0].Rows[0]["District"].ToString();               
                    ViewBag.MyDist = objCommon.GetDistE();

                    // rm.Tehsil = Convert.ToInt32(ds.Tables[0].Rows[0]["Tehsil"].ToString());

                    // sm.dist = ds.Tables[0].Rows[0]["Dist"].ToString();

                    int dist = Convert.ToInt32(ds.Tables[0].Rows[0]["Dist"].ToString());
                    DataSet result1 = objCommon.SelectAllTehsil(dist);
                    // ViewBag.MyTeh = result1.Tables[0]; 

                    List<SelectListItem> TehList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result1.Tables[0].Rows)
                    {

                        TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCode"].ToString() });
                    }


                    List<SelectListItem> TehListP = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result1.Tables[0].Rows)
                    {
                        TehListP.Add(new SelectListItem { Text = @dr["TEHSILP"].ToString(), Value = @dr["TCode"].ToString() });
                    }

                    ViewBag.MyTeh = TehList;
                    ViewBag.MyTehP = TehListP;


                    if (Session["Session"].ToString() != "2016-2017")
                    {
                        sm.SchlEstd = ds.Tables[0].Rows[0]["SCHLESTD"].ToString();
                        sm.SchlType = ds.Tables[0].Rows[0]["SCHLTYPE"].ToString();
                        sm.Edublock = ds.Tables[0].Rows[0]["EDUBLOCK"].ToString();
                        string block = ds.Tables[0].Rows[0]["EDUBLOCK"].ToString();
                        DataSet Eduresult = new AbstractLayer.RegistrationDB().Select_CLUSTER_NAME(block);
                        ViewBag.MyEduCluster = Eduresult.Tables[0];
                        //List<SelectListItem> EduClusterList = new List<SelectListItem>();
                        // EduClusterList.Add(new SelectListItem { Text = "---Edu Cluster---", Value = "0" });
                        foreach (System.Data.DataRow dr in ViewBag.MyEduCluster.Rows)
                        {
                            EduClusterList.Add(new SelectListItem { Text = @dr["CLUSTER_NAME"].ToString(), Value = @dr["CLUSTER_NAME"].ToString() });
                        }

                        ViewBag.MyEduCluster = EduClusterList;
                        sm.EduCluster = ds.Tables[0].Rows[0]["EDUCLUSTER"].ToString();

                        ViewBag.ESTDYR = ds.Tables[0].Rows[0]["SCHLESTD"].ToString();
                    }

                }

                return View(sm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Admin_School_Master", "School");
                //return View();
            }
            // return View();
        }


        [AdminLoginCheckFilter]
        [HttpPost]
        public async Task<ActionResult> School_Modification_Form(SchoolModels sm, FormCollection frm)
        {
            try
            {
                AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
                //new 
                ViewBag.SubUserTypeList = objCommon.GetSubUserType();
                ViewBag.EstalimentYearList = objCommon.GetEstalimentYearList();
                //
                //SchoolModels sm = new  SchoolModels();
                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

                ViewBag.CountList = objCommon.GetCountList();
                // Area 
                ViewBag.AREAList = objCommon.GetArea();
                // YesNo 
                ViewBag.YesNoList = objCommon.GetYesNo();
                // Status
                ViewBag.StatusList = objCommon.GetStatus();
                // Session 
                // ViewBag.SessionList = objCommon.GetSessionAdmin();
                ViewBag.SessionList = objCommon.GetEstalimentYearList();
                // Class 
                ViewBag.ClassTypeList = objCommon.GetClass();
                // School 
                ViewBag.SchoolTypeList = objCommon.GetSchool();
                ViewBag.SchoolType = objCommon.GetSchoolAbbr();
                // English Dist 
                ViewBag.DistEList = objCommon.GetDistE();
                // Punjabi   Dist         
                ViewBag.DistPList = objCommon.GetDistP();
                // SchoolYear 
                // ViewBag.SchoolYear = objCommon.GetSessionYearSchoolAdmin();
                ViewBag.SchoolYear = objCommon.GetEstalimentYearListSingle();

                ViewBag.SchoolSetMset = objDB.GetSchoolSetByType(1).ToList();
                ViewBag.SchoolSetMoSet = objDB.GetSchoolSetByType(2).ToList();
                ViewBag.SchoolSetSSet = objDB.GetSchoolSetByType(3).ToList();
                ViewBag.SchoolSetSoset = objDB.GetSchoolSetByType(4).ToList();


                if (Session["Session"].ToString() != "2016-2017")
                {
                    string mydist = sm.dist.ToString();
                    DataSet result2 = objDB.SelectBlock(mydist);
                    ViewBag.MyEdublock = result2.Tables[0];
                    List<SelectListItem> BlockList = new List<SelectListItem>();
                    //BlockList.Add(new SelectListItem { Text = "---Edu Block---", Value = "0" });
                    foreach (System.Data.DataRow dr in ViewBag.MyEdublock.Rows)
                    {
                        BlockList.Add(new SelectListItem { Text = @dr["Edu Block Name"].ToString(), Value = @dr["Edu Block Name"].ToString() });
                    }
                    ViewBag.MyEdublock = BlockList;
                }
                List<SelectListItem> EduClusterList = new List<SelectListItem>();
                EduClusterList.Add(new SelectListItem { Text = "---Edu Cluster---", Value = "0" });
                ViewBag.MyEduCluster = EduClusterList;



                int dist = Convert.ToInt32(sm.dist);
                DataSet result1 = objCommon.SelectAllTehsil(dist);
                // ViewBag.MyTeh = result1.Tables[0]; 


                List<SelectListItem> TehList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in result1.Tables[0].Rows)
                {

                    TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCode"].ToString() });
                }


                List<SelectListItem> TehListP = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in result1.Tables[0].Rows)
                {
                    TehListP.Add(new SelectListItem { Text = @dr["TEHSILP"].ToString(), Value = @dr["TCode"].ToString() });
                }

                ViewBag.MyTeh = TehList;
                ViewBag.MyTehP = TehListP;

                if (frm["Tehsil"] != null)
                {
                    sm.Tehsile = frm["Tehsil"].ToString();
                    sm.Tehsilp = frm["Tehsil"].ToString();
                    sm.Tcode = frm["Tehsil"].ToString();
                }
                else
                {
                    if (sm.Tehsile != null)
                    {
                        sm.Tcode = sm.Tehsile;
                    }
                }

                if (frm["NSQF_flag"] != null)
                {
                    sm.NSQF_flag = frm["NSQF_flag"].ToString();
                    //sm.NSQF_flag = frm["NSQF_flag"].ToString() == "Y" ? true : false;
                }
                else
                {
                    sm.NSQF_flag = "";
                    //sm.NSQF_flag = false; 
                }
                // int DistNo = Convert.ToInt32(frm["DIST"].ToString());
                sm.DISTNM = objCommon.GetDistE().Where(i => i.Value == frm["DIST"].ToString()).Single().Text;
                if (Session["UserName"] != null)
                {
                    sm.username = Session["UserName"].ToString();
                }
                //  sm.DISTNM = frm["DIST"].ToString();

                //#region Call API to update school master details


                //ViewBag.ApiStatus = "1";
                //ViewBag.ErrStatus = "1";

                //if (!string.IsNullOrEmpty(sm.SameAsSchl))
                //{
                //    if (sm.SameAsSchl.ToLower() == "yes")
                //    {
                //        sm.idno = sm.SCHL;
                //    }
                //}


                //string apiStatus = "";
                //SchoolApiViewModel savm = new SchoolApiViewModel();


                //try
                //{
                //    sm.userip = AbstractLayer.StaticDB.GetFullIPAddress();
                //    //apiStatus = await new AbstractLayer.PsebAPIServiceDB().UpdateSMFPSEBMainToPsebJunior(sm );
                //    savm = await new AbstractLayer.PsebAPIServiceDB().UpdateSMFPSEBMainToPsebJunior(sm);
                //    apiStatus = savm.statusCode;
                //    ViewBag.ApiStatus = apiStatus;
                //    ViewBag.ErrStatus = "200";
                //}
                //catch (Exception ex)
                //{
                //    ViewBag.ApiStatus = apiStatus;
                //    ViewBag.ErrStatus = ex.Message;
                //}
                //#endregion

                // int result = savm.statusCode == "200" ? 1 : 0;
                string EmpUserId = adminLoginSession.AdminEmployeeUserId;
                int result = objDB.UpdateSMF(sm, 1, EmpUserId); // passing Value to SchoolDB from model and Type 1 For regular
                if (result > 0)
                {
                    ViewData["resultSMF"] = 1;
                    ViewBag.Message = "Your school information is updated successfully. Your correction number is " + result;
                    // ViewBag.Message = "School information is updated successfully.";
                    //ViewBag.Message = "Your school information is updated successfully. Your correction number is " + savm.Object.CorrectionNo;
                    return View(sm);
                }
                else
                {
                    ViewData["resultSMF"] = result;
                    ModelState.AddModelError("", "Not Update");
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ApiStatus = "catch";
                ViewBag.ErrStatus = ex.Message;
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }

        }

        [HttpPost]
        public ActionResult CancelForm(SchoolModels sm)
        {
            return RedirectToAction("Admin_School_Master", "School");
        }

        public ActionResult School_View_Form(string id)
        {
            if (Session["AdminId"] == null)
            { return RedirectToAction("Index", "Admin"); }
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Admin_School_Master", "School");
                }

                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB(); //calling class SchoolDB           
                DataSet ds = objDB.SelectSchoolDatabyID(id);
                SchoolModels sm = new SchoolModels();
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    ViewData["resultSVF"] = 2;
                }
                else
                {
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        sm.CorrectionNoOld = ds.Tables[1].Rows[0]["CorrectionNoOld"].ToString();
                        sm.RemarksOld = ds.Tables[1].Rows[0]["RemarksOld"].ToString();
                        sm.RemarkDateOld = ds.Tables[1].Rows[0]["RemarkDateOld"].ToString();
                    }
                    sm.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                    sm.SCHL = ds.Tables[0].Rows[0]["schl"].ToString();
                    sm.udisecode = ds.Tables[0].Rows[0]["udisecode"].ToString();
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

                    sm.PRINCIPAL = ds.Tables[0].Rows[0]["PRINCIPAL"].ToString();
                    sm.STDCODE = ds.Tables[0].Rows[0]["STDCODE"].ToString();
                    sm.PHONE = ds.Tables[0].Rows[0]["PHONE"].ToString();
                    sm.MOBILE = ds.Tables[0].Rows[0]["MOBILE"].ToString();
                    sm.mobile2 = ds.Tables[0].Rows[0]["mobile2"].ToString();
                    sm.ADDRESSE = ds.Tables[0].Rows[0]["ADDRESSE"].ToString();
                    sm.ADDRESSP = ds.Tables[0].Rows[0]["ADDRESSP"].ToString();
                    sm.CONTACTPER = ds.Tables[0].Rows[0]["CONTACTPER"].ToString();
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

                    if (Session["Session"].ToString() != "2016-2017")
                    {
                        sm.SchlEstd = ds.Tables[0].Rows[0]["SCHLESTD"].ToString();
                        sm.SchlType = ds.Tables[0].Rows[0]["SCHLTYPE"].ToString();
                        sm.Edublock = ds.Tables[0].Rows[0]["EDUBLOCK"].ToString();
                    }

                    //FIFTH
                    sm.fifth = ds.Tables[0].Rows[0]["fifth"].ToString() == "Y" ? "YES" : "NO";
                    sm.FIF_YR = sm.fifth == "YES" ? ds.Tables[0].Rows[0]["FIF_YR"].ToString() : "XXX";
                    sm.FIF_UTYPE = ViewBag.FIF_UTYPE = ds.Tables[0].Rows[0]["FIF_UTYPEFull"].ToString();
                    sm.FIF_S = ViewBag.FIF_S = Convert.ToString(ds.Tables[0].Rows[0]["FIF_S"].ToString());
                    sm.lclass = ds.Tables[0].Rows[0]["lclass"].ToString();
                    sm.uclass = ds.Tables[0].Rows[0]["uclass"].ToString();
                }
                return View(sm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }


        public ActionResult School_Password()
        {
            return View();
        }

        public ActionResult Open_School_Master()
        {
            return View();
        }

        public JsonResult MyEdublock(string DIST) // Calling on http post (on Submit)
        {
            DataSet result = objDB.SelectBlock(DIST);
            ViewBag.MyEdublock = result.Tables[0];
            List<SelectListItem> BlockList = new List<SelectListItem>();
            //BlockList.Add(new SelectListItem { Text = "---Edu Block---", Value = "0" });
            foreach (System.Data.DataRow dr in ViewBag.MyEdublock.Rows)
            {
                BlockList.Add(new SelectListItem { Text = @dr["Edu Block Name"].ToString(), Value = @dr["Edu Block Name"].ToString() });
            }
            ViewBag.MyEdublock = BlockList;
            return Json(BlockList);
        }


        public JsonResult GetTehID(int DIST) // Calling on http post (on Submit)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            DataSet result = objDB.SelectAllTehsil(DIST);
            List<SelectListItem> TehList = new List<SelectListItem>();
            TehList.Add(new SelectListItem { Text = "--Select Tehsil--", Value = "0" });
            foreach (System.Data.DataRow dr in result.Tables[0].Rows)
            {
                TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCode"].ToString() });
            }
            ViewBag.MyTeh = TehList;
            return Json(TehList);
        }

        public JsonResult GetTehIDP(int DIST) // Calling on http post (on Submit)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            DataSet result = objDB.SelectAllTehsil(DIST);
            List<SelectListItem> TehListP = new List<SelectListItem>();
            TehListP.Add(new SelectListItem { Text = "--Select Tehsil--", Value = "0" });
            foreach (System.Data.DataRow dr in result.Tables[0].Rows)
            {
                TehListP.Add(new SelectListItem { Text = @dr["TEHSILP"].ToString(), Value = @dr["TCode"].ToString() });
            }
            ViewBag.MyTehP = TehListP;
            return Json(TehListP);
        }

        public JsonResult GetTehsilP(int Teh) // Calling on http post (on Submit)
        {
            string TehPText = Teh.ToString();
            //string DistText = "";
            //if (Teh.ToString().Length == 2)
            //{ DistText = "0" + Teh.ToString(); }
            //else
            //{
            //    DistText = Teh.ToString();
            //}

            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            List<SelectListItem> TehP = objCommon.GetAllTehsilP();

            if (TehP.Any(i => i.Value == TehPText))
            {
                TehPText = ViewBag.MyTehP = TehP.Where(i => i.Value == TehPText).Single().Text;
            }
            return Json(TehPText);
        }

        public JsonResult GetDistP(int DIST) // Calling on http post (on Submit)
        {
            string DistPText = "";
            string DistText = "";
            if (DIST.ToString().Length == 2)
            { DistText = "0" + DIST.ToString(); }
            else
            {
                DistText = DIST.ToString();
            }

            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            List<SelectListItem> DistP = objCommon.GetDistP();

            if (DistP.Any(i => i.Value == DistText))
            {
                DistPText = ViewBag.MyTehP = DistP.Where(i => i.Value == DistText).Single().Text;
            }
            return Json(DistPText);
        }



        #region LinkOtherSchool


        public ActionResult LinkOtherSchool(string id, SchoolModels sm)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Admin_School_Master", "School");
                }
                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB(); //calling class SchoolDB           
                DataSet ds = objDB.SelectSchoolDatabyID(id);
                sm.StoreAllData = ds;
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCount = 0;
                    ViewData["resultSVF"] = 2;
                }
                else
                {
                    sm.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                    ViewBag.Schl = sm.SCHL = ds.Tables[0].Rows[0]["schl"].ToString();
                    sm.SCHLE = ds.Tables[0].Rows[0]["SCHLEfull"].ToString();
                    sm.DISTNM = ds.Tables[0].Rows[0]["DISTNM"].ToString();
                    sm.ImpschlOcode = ds.Tables[0].Rows[0]["ImpschlOcode"].ToString();
                    ViewBag.TotalCount = 0;
                    if (ds.Tables[5].Rows.Count > 0)
                    { ViewBag.TotalCount = 1; }
                }

                return View(sm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        [HttpPost]
        public ActionResult LinkOtherSchool(string id, SchoolModels sm, string LinkSchool)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            try
            {

                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("Admin_School_Master", "School");
                }

                else if (!string.IsNullOrEmpty(LinkSchool))
                {
                    sm.SCHL = id;
                    sm.ImpschlOcode = LinkSchool;
                    int result = objDB.UpdateSMF(sm, 3, adminLoginSession.AdminEmployeeUserId); // passing Value to SchoolDB from model and Type 1 For regular
                    if (result > 0)
                    {
                        ViewData["result"] = 1;
                    }
                    else
                    {
                        ViewData["result"] = 0;
                    }
                }
                else
                { ViewData["result"] = 3; }


                DataSet ds = objDB.SelectSchoolDatabyID(id);
                sm.StoreAllData = ds;
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCount = 0;
                    ViewData["resultSVF"] = 2;
                }
                else
                {
                    sm.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                    ViewBag.Schl = sm.SCHL = ds.Tables[0].Rows[0]["schl"].ToString();
                    sm.SCHLE = ds.Tables[0].Rows[0]["SCHLEfull"].ToString();
                    sm.DISTNM = ds.Tables[0].Rows[0]["DISTNM"].ToString();
                    sm.ImpschlOcode = ds.Tables[0].Rows[0]["ImpschlOcode"].ToString();
                    ViewBag.TotalCount = 0;
                    if (ds.Tables[5].Rows.Count > 0)
                    { ViewBag.TotalCount = 1; }
                }
                return View(sm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }


        public ActionResult RemoveLinkOtherSchool(string id, string Schl)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            SchoolModels sm = new SchoolModels();
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("LinkOtherSchool", "School");
            }
            else if (string.IsNullOrEmpty(Schl))
            {
                return RedirectToAction("LinkOtherSchool", "School");
            }
            else
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(Schl))
                {
                    sm.SCHL = Schl;
                    sm.ImpschlOcode = id;
                    int result = objDB.UpdateSMF(sm, 4, adminLoginSession.AdminEmployeeUserId);
                    if (result > 0)
                    {
                        ViewData["result"] = 1;
                    }
                    else
                    {
                        ViewData["result"] = 0;
                    }
                }
            }
            return RedirectToAction("LinkOtherSchool", "School", new { id = Schl });
        }

        #endregion LinkOtherSchool

        public ActionResult Open_School_Modification_Form(string id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Admin_School_Master", "School");
                }
                ////if (Session["AdminType"].ToString() != "admin")
                ////{
                ////    if (!Session["PAccessRight"].ToString().Contains("Sch"))
                ////    {
                ////        return RedirectToAction("Index", "Admin");
                ////    }
                ////}
                ViewBag.YesNoList = objCommon.GetYesNo();

                // SCHL = "0064583";
                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB(); //calling class SchoolDB
                DataSet ds = objDB.SelectSchoolDatabyID(id);
                SchoolModels sm = new SchoolModels();
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    ViewData["resultOSMF"] = 2;
                    return View();
                }
                else
                {
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        sm.CorrectionNoOld = ds.Tables[1].Rows[0]["CorrectionNoOld"].ToString();
                        sm.RemarksOld = ds.Tables[1].Rows[0]["RemarksOld"].ToString();
                        sm.RemarkDateOld = ds.Tables[1].Rows[0]["RemarkDateOld"].ToString();
                    }
                    sm.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                    sm.SCHL = ds.Tables[0].Rows[0]["schl"].ToString();
                    sm.udisecode = ds.Tables[0].Rows[0]["udisecode"].ToString();
                    sm.idno = ds.Tables[0].Rows[0]["idno"].ToString();
                    string classnum = ds.Tables[0].Rows[0]["CLASS"].ToString();
                    sm.CLASS = ds.Tables[0].Rows[0]["CLASS"].ToString();
                    sm.OCODE = ds.Tables[0].Rows[0]["OCODE"].ToString();
                    sm.USERTYPE = ds.Tables[0].Rows[0]["USERTYPE"].ToString();
                    sm.session = ds.Tables[0].Rows[0]["SESSION"].ToString();
                    sm.status = ds.Tables[0].Rows[0]["status"].ToString();
                    sm.Approved = ds.Tables[0].Rows[0]["IsApproved"].ToString();
                    sm.vflag = ds.Tables[0].Rows[0]["IsVerified"].ToString();
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


                    sm.PRINCIPAL = ds.Tables[0].Rows[0]["PRINCIPAL"].ToString();
                    sm.MOBILE = ds.Tables[0].Rows[0]["MOBILE"].ToString();
                    sm.mobile2 = ds.Tables[0].Rows[0]["mobile2"].ToString();
                    sm.ADDRESSE = ds.Tables[0].Rows[0]["ADDRESSE"].ToString();
                    sm.ADDRESSP = ds.Tables[0].Rows[0]["ADDRESSP"].ToString();
                    sm.CONTACTPER = ds.Tables[0].Rows[0]["CONTACTPER"].ToString();
                    sm.EMAILID = ds.Tables[0].Rows[0]["EMAILID"].ToString();

                    //sm.REMARKS = ds.Tables[0].Rows[0]["REMARKS"].ToString();
                    sm.UDATE = ds.Tables[0].Rows[0]["UDATE"].ToString();
                    sm.correctionno = ds.Tables[0].Rows[0]["correctionno"].ToString();

                    //Regular

                    sm.middle = ViewBag.middle = "N";
                    sm.MATRIC = ViewBag.MATRIC = "N";
                    sm.HUM = ViewBag.HUM = "N";
                    sm.SCI = ViewBag.SCI = "N";
                    sm.COMM = ViewBag.COMM = "N";
                    sm.VOC = ViewBag.VOC = "N";
                    sm.TECH = ViewBag.TECH = "N";
                    sm.AGRI = ViewBag.AGRI = "N";

                    //OPen
                    sm.omiddle = ViewBag.omiddle = ds.Tables[0].Rows[0]["omiddle"].ToString();
                    sm.OMATRIC = ViewBag.OMATRIC = ds.Tables[0].Rows[0]["OMATRIC"].ToString();
                    sm.OHUM = ViewBag.OHUM = ds.Tables[0].Rows[0]["OHUM"].ToString();
                    sm.OSCI = ViewBag.OSCI = ds.Tables[0].Rows[0]["OSCI"].ToString();
                    sm.OCOMM = ViewBag.OCOMM = ds.Tables[0].Rows[0]["OCOMM"].ToString();
                    sm.OVOC = ViewBag.OVOC = ds.Tables[0].Rows[0]["OVOC"].ToString();
                    sm.OTECH = ViewBag.OTECH = ds.Tables[0].Rows[0]["OTECH"].ToString();
                    sm.OAGRI = ViewBag.OAGRI = ds.Tables[0].Rows[0]["OAGRI"].ToString();

                    //ViewBag.omiddle = new SelectList(ds.Tables[0].Rows[0]["omiddle"].ToString(), ds.Tables[0].Rows[0]["omiddle"].ToString());
                    //ViewBag.Ometric = new SelectList(ds.Tables[0].Rows[0]["OMATRIC"].ToString(), ds.Tables[0].Rows[0]["OMATRIC"].ToString());

                    //ViewBag.OHum = new SelectList(ds.Tables[0].Rows[0]["OHUM"].ToString(), ds.Tables[0].Rows[0]["OHUM"].ToString());
                    //ViewBag.OSci = new SelectList(ds.Tables[0].Rows[0]["OSCI"].ToString(), ds.Tables[0].Rows[0]["OSCI"].ToString());
                    //ViewBag.OComm = new SelectList(ds.Tables[0].Rows[0]["OCOMM"].ToString(), ds.Tables[0].Rows[0]["OCOMM"].ToString());
                    //ViewBag.Ovoc = new SelectList(ds.Tables[0].Rows[0]["OVOC"].ToString(), ds.Tables[0].Rows[0]["OVOC"].ToString());
                    //ViewBag.Otech = new SelectList(ds.Tables[0].Rows[0]["OTECH"].ToString(), ds.Tables[0].Rows[0]["OTECH"].ToString());
                    //ViewBag.OAgri = new SelectList(ds.Tables[0].Rows[0]["OAGRI"].ToString(), ds.Tables[0].Rows[0]["OAGRI"].ToString());


                    //ViewBag.OHum = ds.Tables[0].Rows[0]["OHUM"].ToString();
                    // sm.STATIONE = ds.Tables[0].Rows[0]["STATIONE"].ToString();

                    //mRegister.imagename = ds.Tables[0].Rows[0]["std_pic"].ToString();
                    // ViewBag.ImageURL = "../../StdImages/Upload/" + ds.Tables[0].Rows[0]["std_pic"].ToString();

                    sm.Tcode = ViewBag.Tcode = ds.Tables[0].Rows[0]["Tcode"].ToString();
                    sm.Tehsile = ViewBag.Tehsile = ds.Tables[0].Rows[0]["Tehsile"].ToString();
                    sm.Tehsilp = ViewBag.Tehsilp = ds.Tables[0].Rows[0]["Tehsilp"].ToString();


                    int dist = Convert.ToInt32(sm.dist);
                    DataSet result1 = objCommon.SelectAllTehsil(dist);
                    List<SelectListItem> TehList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result1.Tables[0].Rows)
                    {

                        TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCode"].ToString() });
                    }


                    List<SelectListItem> TehListP = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result1.Tables[0].Rows)
                    {
                        TehListP.Add(new SelectListItem { Text = @dr["TEHSILP"].ToString(), Value = @dr["TCode"].ToString() });
                    }

                    ViewBag.MyTeh = TehList;
                    ViewBag.MyTehP = TehListP;

                }
                return View(sm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        [HttpPost]
        public ActionResult Open_School_Modification_Form(SchoolModels sm)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            try
            {
                // YesNo 

                int dist = Convert.ToInt32(sm.dist);
                DataSet result1 = objCommon.SelectAllTehsil(dist);
                List<SelectListItem> TehList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in result1.Tables[0].Rows)
                {

                    TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCode"].ToString() });
                }


                List<SelectListItem> TehListP = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in result1.Tables[0].Rows)
                {
                    TehListP.Add(new SelectListItem { Text = @dr["TEHSILP"].ToString(), Value = @dr["TCode"].ToString() });
                }

                ViewBag.MyTeh = TehList;
                ViewBag.MyTehP = TehListP;


                if (sm.Tcode != null)
                {
                    sm.Tehsile = sm.Tehsilp;
                    sm.Tehsilp = sm.Tehsile;
                    sm.Tcode = sm.Tcode;
                }


                ViewBag.YesNoList = objCommon.GetYesNo();
                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                int result = objDB.UpdateSMF(sm, 2, adminLoginSession.AdminEmployeeUserId); // passing Value to SchoolDB from model and Type 1 For regular
                if (result > 0)
                {
                    ViewData["resultOSMF"] = 1;
                    return RedirectToAction("Admin_School_Master", "School");
                }
                else
                {
                    ViewData["resultOSMF"] = result;
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


        #region Start Bulk Photo Upload

        public bool ValidateFileDimensions(Stream ipStream)
        {
            using (System.Drawing.Image myImage = System.Drawing.Image.FromStream(ipStream))
            {
                //  return (myImage.Height == 145 && myImage.Width == 218); // by Patra Sir
                return (myImage.Height == 252 && myImage.Width == 324); // approval by Harpal/Patra Sir
                                                                        //if(myImage.Height < 130 || myImage.Width < 100)
            }
            //3.5*4.5 - > 252x324
            //4.5*5.5 - > 324x424
        }

        public ActionResult Photo_Upload()
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                else
                {

                    var itemsch = new SelectList(new[]{new {ID="1",Name="N1"},
                                                       new {ID="2",Name="N2"},
                                                       new {ID="3",Name="N3"},
                                                       new {ID="4",Name="M1"},
                                                       new {ID="5",Name="M2"},
                                                       new {ID="6",Name="E1"},
                                                       new {ID="7",Name="E2"},
                                                       new {ID="8",Name="T1"},
                                                       new {ID="9",Name="T2"},
                                                        }, "ID", "Name", 1);

                    //var itemsch = new SelectList(new[]{
                    //                                   new {ID="4",Name="M1"},
                    //                                   new {ID="5",Name="M2"},                                                      
                    //                                   new {ID="8",Name="T1"},
                    //                                   new {ID="9",Name="T2"},
                    //                                    }, "ID", "Name", 1);

                    ViewBag.MyForm = itemsch.ToList();
                    return View();
                }
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }
        [HttpPost]
        public ActionResult Photo_Upload(HttpPostedFileBase FileUploadBulk, HttpPostedFileBase FileUploadBulk2, FormCollection frm)
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                var itemsch = new SelectList(new[]{new {ID="1",Name="N1"},
                                               new {ID="2",Name="N2"},
                                               new {ID="3",Name="N3"},
                                               new {ID="4",Name="M1"},
                                               new {ID="5",Name="M2"},
                                               new {ID="6",Name="E1"},
                                               new {ID="7",Name="E2"},
                                               new {ID="8",Name="T1"},
                                               new {ID="9",Name="T2"},
                                                }, "ID", "Name", 1);
                //var itemsch = new SelectList(new[]{
                //                                       new {ID="4",Name="M1"},
                //                                       new {ID="5",Name="M2"},
                //                                       new {ID="8",Name="T1"},
                //                                       new {ID="9",Name="T2"},
                //                                        }, "ID", "Name", 1);
                ViewBag.MyForm = itemsch.ToList();

                ViewBag.SelectedItem = frm["FormNameList"];
                string FormName = frm["FormNameList"].ToString();
                if (frm["FormNameList"] != "")
                {
                    if (FormName == "1") { FormName = "N1"; }
                    else if (FormName == "2") { FormName = "N2"; }
                    else if (FormName == "3") { FormName = "N3"; }
                    else if (FormName == "4") { FormName = "M1"; }
                    else if (FormName == "5") { FormName = "M2"; }
                    else if (FormName == "6") { FormName = "E1"; }
                    else if (FormName == "7") { FormName = "E2"; }
                    else if (FormName == "8") { FormName = "T1"; }
                    else if (FormName == "9") { FormName = "T2"; }
                }
                if (FileUploadBulk != null && FormName != "")
                {
                    string DistID = string.Empty;
                    string Std_Id = string.Empty;
                    string Filepath = string.Empty;
                    string path;
                    string SchlID = string.Empty;
                    string filename = string.Empty;
                    string SaveFile = string.Empty;
                    string SaveFileN = string.Empty;
                    string SaveFileD = string.Empty;
                    string SaveFileNextn = string.Empty;
                    string Incorrectfile = string.Empty;
                    int countP = 0; int countS = 0;
                    SchlID = Convert.ToString(Session["SCHL"]);
                    if (SchlID == null || SchlID == "")
                    {
                        return RedirectToAction("Index", "Login");
                    }

                    if (FileUploadBulk.ContentLength > 0)
                    {

                        HttpFileCollectionBase files = Request.Files;
                        DataTable dt = new DataTable { Columns = { new DataColumn("Path") } };
                        SchoolModels sm = new SchoolModels();
                        AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            string Prefix = string.Empty;
                            //if (file.FileName.Length <= 14) by rohit

                            if (file.FileName.Length > 0)
                            {
                                if (file.FileName.Length <= 14)
                                {
                                    int lname = file.FileName.Length;
                                    lname = 14 - lname;
                                    switch (lname)
                                    {
                                        case 1: Prefix = "0"; break;
                                        case 2: Prefix = "00"; break;
                                        case 3: Prefix = "000"; break;
                                        case 4: Prefix = "0000"; break;
                                        case 5: Prefix = "00000"; break;
                                        case 6: Prefix = "000000"; break;
                                    }


                                }
                                string fname = Prefix + file.FileName.ToString();
                                string chkf = fname.Substring(9, 1).ToUpper();
                                //string chkf1 = fname.Substring(9, 1).ToUpper();
                                //  if (file.ContentType != "image/jpeg" || fname.Length > 14 || fname.Length < 6 || (fname.Substring(9, 1).ToUpper() != "P" && fname.Substring(9, 1).ToUpper() != "S"))
                                if (file.ContentType != "image/jpeg" || fname.Length > 14 || fname.Length < 6 || (fname.Substring(9, 1).ToUpper() != "P" && fname.Substring(9, 1).ToUpper() != "S"))
                                {
                                    SaveFileNextn = SaveFileNextn + file.FileName + ", ";
                                }
                                else if (file.ContentLength > 50000 || file.ContentLength < 4000)
                                {
                                    SaveFileN = SaveFileN + file.FileName + ", ";
                                }
                                //else if (ValidateFileDimensions(file.InputStream) == false)
                                //{
                                //    SaveFileD = SaveFileD + file.FileName + ", ";
                                //}
                                else
                                {

                                    //filename = file.FileName.Substring(0, 7);
                                    filename = fname.Substring(0, 9);
                                    //DistID = "150";
                                    DistID = Session["SCHOOLDIST"].ToString();   // Add Dist ID
                                    Std_Id = FormName + DistID + filename;
                                    if (fname.Substring(9, 1).ToUpper() == "P")
                                    {
                                        int OutStatus = 0;
                                        string PhotoName = FormName + "/" + DistID + "/Photo" + "/" + Std_Id + "P" + ".jpg";
                                        string type = "P";
                                        string UpdatePic = objDB.Updated_Bulk_Pic_Data(Std_Id, PhotoName, type, SchlID, out OutStatus);
                                        if (OutStatus.ToString() == "1")
                                        {
                                            ////  Filepath = Server.MapPath("~/Upload/"+ "Upload2017/" + FormName + "/" + DistID + "/Photo/");
                                            Filepath = sp + "/" + "Upload2023/" + FormName + "/" + DistID + "/Photo/";
                                            if (!Directory.Exists(Filepath))
                                            {
                                                Directory.CreateDirectory(Filepath);
                                            }
                                            SaveFile = SaveFile + file.FileName + ", ";
                                            ////  path = Path.Combine(Server.MapPath("~/Upload/"+ "Upload2017/" + FormName + "/" + DistID + "/Photo"), Std_Id + "P" + ".jpg");
                                            path = sp + "/" + "Upload2023/" + FormName + "/" + DistID + "/Photo" + "/" + Std_Id + "P" + ".jpg";
                                            dt.Rows.Add(file.FileName);
                                            file.SaveAs(path);
                                            countP = countP + 1;
                                        }
                                        else
                                        {
                                            Incorrectfile = Incorrectfile + file.FileName + ", ";
                                        }
                                    }
                                    //else if (fname.Substring(9, 1).ToUpper() == "S")
                                    //{
                                    //    int OutStatus = 0;
                                    //    string PhotoName = FormName + "/" + DistID + "/Sign" + "/" + Std_Id + "S" + ".jpg";
                                    //    string type = "S";
                                    //    string UpdatePic = objDB.Updated_Bulk_Pic_Data(Std_Id, PhotoName, type, SchlID, out OutStatus);
                                    //    if (OutStatus.ToString() == "1")
                                    //    {
                                    //        //// Filepath = Server.MapPath("~/Upload/"+ "Upload2017/" + FormName + "/" + DistID + "/Sign/");
                                    //        Filepath = sp + "/" + "Upload2017/" + FormName + "/" + DistID + "/Sign/";
                                    //        if (!Directory.Exists(Filepath))
                                    //        {
                                    //            Directory.CreateDirectory(Filepath);
                                    //        }
                                    //        SaveFile = SaveFile + file.FileName + ",";
                                    //        //// path = Path.Combine(Server.MapPath("~/Upload/"+ "Upload2017/" + FormName + "/" + DistID + "/Sign"), Std_Id + "S" + ".jpg");
                                    //        path = sp + "/" + "Upload2017/" + FormName + "/" + DistID + "/Sign" + "/" + Std_Id + "S" + ".jpg";
                                    //        dt.Rows.Add(file.FileName);
                                    //        file.SaveAs(path);
                                    //        countS = countS + 1;
                                    //    }
                                    //    else
                                    //    {
                                    //        Incorrectfile = Incorrectfile + file.FileName + ", ";
                                    //    }
                                    //}

                                }
                            }

                        }
                        if (SaveFileN != "" && SaveFileNextn != "")
                        {
                            @ViewBag.msgSE = "Image not uploaded : Size [ " + SaveFileN + " ]  Extension [ " + SaveFileNextn + " ];";
                        }
                        else if (SaveFileN != "")
                        {
                            @ViewBag.msgS = "Image size not ok : File Name [ " + SaveFileN + " ]";
                        }
                        else if (SaveFileD != "")
                        {
                            @ViewBag.msgS = "Image dimension should be 252 x 324 : File Name [ " + SaveFileD + " ]";
                        }
                        if (SaveFileNextn != "")
                        {
                            @ViewBag.msgE = "File name & Extension not ok : File Name [ " + SaveFileNextn + " ]";
                        }
                        if (Incorrectfile.ToString() != "")
                        {
                            @ViewBag.msgInC = "Student details not exist : File Name [ " + Incorrectfile + " ]";
                        }
                        if (countP > 0 || countS > 0)
                        {
                            @ViewBag.msgOK = "Successfully uploaded: Photo = " + countP + " Sign = " + countS;
                        }
                    }
                    else
                    {

                        SaveFileN = string.Empty;
                        @ViewBag.msg = "Check Image";
                    }

                }
                else
                {
                    @ViewBag.msg = "Select Image and Form Name";
                }

                // HttpPostedFileBase FileUploadBulk2 = FileUploadBulk;
                if (FileUploadBulk2 != null && FormName != "")
                {

                    string DistID = string.Empty;
                    string Std_Id = string.Empty;
                    string Filepath = string.Empty;
                    string path;
                    string SchlID = string.Empty;
                    string filename = string.Empty;
                    string SaveFile = string.Empty;
                    string SaveFileN = string.Empty;
                    string SaveFileD = string.Empty;
                    string SaveFileNextn = string.Empty;
                    string Incorrectfile = string.Empty;
                    int countP = 0; int countS = 0;
                    SchlID = Convert.ToString(Session["SCHL"]);
                    if (SchlID == null || SchlID == "")
                    {
                        return RedirectToAction("Index", "Login");
                    }

                    if (FileUploadBulk2.ContentLength > 0)
                    {

                        HttpFileCollectionBase files = Request.Files;
                        DataTable dt = new DataTable { Columns = { new DataColumn("Path") } };
                        SchoolModels sm = new SchoolModels();
                        AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            string Prefix = string.Empty;
                            if (file.FileName.Length > 0)
                            {
                                if (file.FileName.Length <= 14)
                                {
                                    int lname = file.FileName.Length;
                                    lname = 14 - lname;
                                    switch (lname)
                                    {
                                        case 1: Prefix = "0"; break;
                                        case 2: Prefix = "00"; break;
                                        case 3: Prefix = "000"; break;
                                        case 4: Prefix = "0000"; break;
                                        case 5: Prefix = "00000"; break;
                                        case 6: Prefix = "000000"; break;
                                    }


                                }
                                string fname = Prefix + file.FileName.ToString();
                                if (file.ContentType != "image/jpeg" || fname.Length > 14 || fname.Length < 6 || (fname.Substring(9, 1).ToUpper() != "P" && fname.Substring(9, 1).ToUpper() != "S"))
                                {
                                    SaveFileNextn = SaveFileNextn + file.FileName + ", ";
                                }

                                else if (file.ContentLength > 50000 || file.ContentLength < 4000)
                                {
                                    SaveFileN = SaveFileN + file.FileName + ", ";
                                }
                                //else if (ValidateFileDimensions(file.InputStream) == false)
                                //{
                                //    SaveFileD = SaveFileD + file.FileName + ", ";
                                //}
                                else
                                {
                                    //filename = file.FileName.Substring(0, 7);
                                    filename = fname.Substring(0, 9);
                                    //DistID = "150";
                                    DistID = Session["SCHOOLDIST"].ToString();   // Add Dist ID
                                    Std_Id = FormName + DistID + filename;

                                    if (fname.Substring(9, 1).ToUpper() == "S")
                                    {
                                        int OutStatus = 0;
                                        string PhotoName = FormName + "/" + DistID + "/Sign" + "/" + Std_Id + "S" + ".jpg";
                                        string type = "S";
                                        string UpdatePic = objDB.Updated_Bulk_Pic_Data(Std_Id, PhotoName, type, SchlID, out OutStatus);//Uploaded_Bulk_Photo_Sign
                                        if (OutStatus.ToString() == "1")
                                        {
                                            //// Filepath = Server.MapPath("~/Upload/" + "Upload2017/" + FormName + "/" + DistID + "/Sign/");
                                            Filepath = sp + "/" + "Upload2023/" + FormName + "/" + DistID + "/Sign/";
                                            if (!Directory.Exists(Filepath))
                                            {
                                                Directory.CreateDirectory(Filepath);
                                            }
                                            SaveFile = SaveFile + file.FileName + ",";
                                            //// path = Path.Combine(Server.MapPath("~/Upload/"+ "Upload2017/" + FormName + "/" + DistID + "/Sign"), Std_Id + "S" + ".jpg");
                                            path = sp + "/" + "Upload2023/" + FormName + "/" + DistID + "/Sign" + "/" + Std_Id + "S" + ".jpg";
                                            dt.Rows.Add(file.FileName);
                                            file.SaveAs(path);
                                            countS = countS + 1;
                                        }
                                        else
                                        {
                                            Incorrectfile = Incorrectfile + file.FileName + ", ";
                                        }
                                    }

                                }
                            }
                        }
                        if (SaveFileN != "" && SaveFileNextn != "")
                        {
                            @ViewBag.msgSE = "Image not uploaded : Size [ " + SaveFileN + " ]  Extension [ " + SaveFileNextn + " ];";
                        }
                        else if (SaveFileN != "")
                        {
                            @ViewBag.msgS = "Image size not ok : File Name [ " + SaveFileN + " ]";
                        }
                        else if (SaveFileD != "")
                        {
                            @ViewBag.msgS = "Image dimension should be 252 x 324 : File Name [ " + SaveFileD + " ]";
                        }
                        if (SaveFileNextn != "")
                        {
                            @ViewBag.msgE = "File name & Extension not ok : File Name [ " + SaveFileNextn + " ]";
                        }
                        if (Incorrectfile.ToString() != "")
                        {
                            @ViewBag.msgInC = "Student details not exist : File Name [ " + Incorrectfile + " ]";
                        }
                        if (countP > 0 || countS > 0)
                        {
                            @ViewBag.msgOK = "Successfully uploaded: Photo = " + countP + " Sign = " + countS;
                        }
                    }
                    else
                    {

                        SaveFileN = string.Empty;
                        @ViewBag.msg = "Check Sign";
                    }

                }
                else
                {
                    @ViewBag.msg = "Select Sign and Form Name";
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

        #endregion Bulk Photo Upload

        //Dee Comment

        public JsonResult FindSchoolName(string schoolcode)
        {
            //string retval = "";
            string schoolname = "";
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            objDB.FindSchoolName(schoolcode, out schoolname);
            string dee = schoolname;

            return Json(new { sn = dee }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ApplyToImport(string schoolcode, string schoolreqcode)
        {
            //string retval = "";
            string schoolname = "";
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            objDB.ApplyToImport(schoolcode, schoolreqcode, out schoolname);
            string dee = schoolname;
            if (dee != "")
                dee = "Allowed Successfully.";

            return Json(new { sn = dee }, JsonRequestBehavior.AllowGet);
        }


        //#region TC Request

        //public ActionResult TCRequest(string id)
        //{


        //    if (Convert.ToString(Session["RoleType"]) != "School")
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }
        //    FormCollection frm = new FormCollection();
        //    var itemsch = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
        //    new{ID="4",Name="Father's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
        //    ViewBag.MySch = itemsch.ToList();
        //    AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
        //    SchoolModels sm = new SchoolModels();
        //    if (ModelState.IsValid)
        //    {
        //        string Search = string.Empty;
        //        string schlid = Convert.ToString(Session["SCHL"]);
        //        string srch = schlid;//Convert.ToString(Session["Search"]);
        //        if (srch != null && srch != "")
        //        {
        //            if (schlid != null || schlid != "")
        //            {
        //                if (id != null)
        //                {

        //                    string stdid = encrypt.QueryStringModule.Decrypt(id);
        //                    Search = "SCHL='" + schlid + "' and ID='" + stdid + "' ";
        //                    sm.StoreAllData = objDB.SearchSchoolDetailsTC(Search);
        //                    string Tcstatus = sm.StoreAllData.Tables[0].Rows[0]["Tcstatus"].ToString();
        //                    if (Tcstatus == "2")
        //                    {
        //                        ViewData["Tcstatus"] = Tcstatus;
        //                        return View(sm);
        //                    }
        //                    else
        //                    {
        //                        ViewData["Tcstatus"] = Tcstatus;
        //                        return View(sm);
        //                    }
        //                }
        //                else
        //                {

        //                    Search = "SCHL='" + schlid + "'"; //Session["Search"].ToString();
        //                    sm.StoreAllData = objDB.SearchSchoolDetailsTC(Search);
        //                    if (sm.StoreAllData == null || sm.StoreAllData.Tables[0].Rows.Count == 0)
        //                    {
        //                        ViewBag.Message = "Record Not Found";
        //                        ViewBag.TotalCount = 0;
        //                        return View();
        //                    }
        //                    else
        //                    {
        //                        ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
        //                        return View(sm);
        //                    }

        //                }
        //            }

        //        }
        //        return View(sm);

        //    }
        //    else
        //    {

        //        return TCRequest(id);
        //    }
        //}
        //[HttpPost]
        //public ActionResult TCRequest(FormCollection frm)
        //{

        //    if (Session["RoleType"].ToString() != "School")
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }
        //    var itemsch = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
        //    new{ID="4",Name="Father's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
        //    ViewBag.MySch = itemsch.ToList();
        //    AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
        //    SchoolModels sm = new SchoolModels();
        //    if (ModelState.IsValid)
        //    {
        //        string Search = string.Empty;
        //        string schlid = Convert.ToString(Session["SCHL"]);
        //        //string SelTotalItem = frm["totalcountlist"];
        //        //string TotalSearchString = Convert.ToString(frm["TotalSearchString"]);
        //        if (schlid != null || schlid != "")
        //        {
        //            Search = "SCHL='" + schlid + "'";
        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }
        //        if (frm["SelList"] != "")
        //        {
        //            ViewBag.SelectedItem = frm["SelList"];
        //            int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());
        //            string SearchBy = frm["SearchByString"].ToString();
        //            if (SearchBy != "" && SearchBy != null)
        //            {
        //                if (SelValueSch == 1)
        //                { Search += " and id='" + frm["SearchByString"].ToString() + "'"; }
        //                else if (SelValueSch == 2)
        //                { Search += " and  RegNo like '%" + frm["SearchByString"].ToString() + "%'"; }
        //                else if (SelValueSch == 3)
        //                { Search += " and  name like '%" + frm["SearchByString"].ToString() + "%'"; }
        //                else if (SelValueSch == 4)
        //                { Search += " and  fname  like '%" + frm["SearchByString"].ToString() + "%'"; }
        //                else if (SelValueSch == 5)
        //                { Search += " and mname like '%" + frm["SearchByString"].ToString() + "%'"; }
        //                else if (SelValueSch == 6)
        //                { Search += " and DOB='" + frm["SearchByString"].ToString() + "'"; }
        //            }


        //        }

        //        sm.SearchResult = Search;
        //        sm.StoreAllData = objDB.SearchSchoolDetailsTC(Search);
        //        if (sm.StoreAllData == null || sm.StoreAllData.Tables[0].Rows.Count == 0)
        //        {
        //            ViewBag.Message = "Record Not Found";
        //            ViewBag.TotalCount = 0;
        //            return View();
        //        }
        //        else
        //        {
        //            Session["Search"] = Search.ToString();
        //            //Session["txtSchoolcode"] = frm["TotalSearchString"];
        //            //Session["ddlSchoolcode"] = frm["totalcountlist"];
        //            ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
        //            return View(sm);
        //        }
        //    }
        //    else
        //    {
        //        return TCRequest(frm);
        //    }
        //}

        ////public ActionResult TCRequestDone(string id)
        ////{
        ////    SchoolModelsTC sm = new SchoolModelsTC();
        ////    try
        ////    {
        ////        id = encrypt.QueryStringModule.Decrypt(id);
        ////        if (Convert.ToString(Session["SCHL"]) != null || Convert.ToString(Session["SCHL"]) != "")
        ////        { }
        ////        else { return RedirectToAction("Index", "Login"); }
        ////        if (Session["RoleType"].ToString() != "School")
        ////        { return RedirectToAction("Index", "Login"); }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Session["Search"] = null;
        ////        return RedirectToAction("Index", "Login");
        ////    }
        ////    string stdid = id;
        ////    if (stdid != null)
        ////    {
        ////        AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

        ////        string Search = string.Empty;
        ////        Search = "SCHL='" + Session["SCHL"] + "' and ID='" + stdid + "' ";
        ////        sm.StoreAllData = objDB.SearchSchoolDetailsTC(Search);
        ////        if (sm.StoreAllData == null || sm.StoreAllData.Tables[0].Rows.Count == 0)
        ////        {
        ////            ViewBag.Message = "Record Not Found";
        ////            ViewBag.TotalCount = 0;
        ////            return View();
        ////        }
        ////        else
        ////        {
        ////            ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
        ////            sm.Candi_Name = sm.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
        ////            sm.Father_Name = sm.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
        ////            sm.Mother_Name = sm.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();
        ////            sm.DOB = sm.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
        ////            sm.Gender = sm.StoreAllData.Tables[0].Rows[0]["SEX"].ToString();
        ////            sm.SdtID = sm.StoreAllData.Tables[0].Rows[0]["ID"].ToString();
        ////            sm.FormName = sm.StoreAllData.Tables[0].Rows[0]["FORM"].ToString();
        ////            sm.regno = sm.StoreAllData.Tables[0].Rows[0]["REGNO"].ToString();
        ////            return View(sm);
        ////        }
        ////    }
        ////    else
        ////    {

        ////        return TCRequestDone(id);
        ////    }
        ////}

        ////[HttpPost]
        ////public ActionResult Update_Result(FormCollection fc)
        ////{
        ////    SchoolModelsTC sm = new SchoolModelsTC();
        ////    if (Convert.ToString(Session["SCHL"]) == null || Convert.ToString(Session["SCHL"]) == "")
        ////    {
        ////        return RedirectToAction("Index", "Login");
        ////    }
        ////    if (Session["RoleType"].ToString() != "School")
        ////    {
        ////        return RedirectToAction("Index", "Login");
        ////    }

        ////    try
        ////    {
        ////        string stdid = encrypt.QueryStringModule.Decrypt(fc["ID"]);
        ////        if (stdid != null)
        ////        {
        ////            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
        ////            sm.ID = Int32.Parse(stdid);
        ////            sm.SCHL = Session["SCHL"].ToString();
        ////            sm.dispatchNo = fc["dispatchNo"];
        ////            sm.attendanceTot = fc["attendanceTot"];
        ////            sm.attendancePresnt = fc["attendancePresnt"];
        ////            sm.struckOff = fc["struckOff"];
        ////            sm.reasonFrSchoolLeav = fc["reasonFrSchoolLeav"];
        ////            //am.reclock = fc["reclocklist"] == "TRUE" ? "1" : "0";

        ////            int result = objDB.GenerateTC(sm);
        ////            // asm.StoreAllData = objDB.SearchSchoolDetails(Search);
        ////            if (result > 0)
        ////            {
        ////                ViewBag.Message = "Record Updated Successfully";
        ////                return RedirectToAction("TCRequest", "School");
        ////                //return adminresultpageReCall(fc);
        ////            }
        ////            else
        ////            {
        ////                ViewBag.Message = "Record not Updated please try again";
        ////                return RedirectToAction("TCRequest", "School");
        ////            }

        ////        }
        ////        else
        ////        {
        ////            return RedirectToAction("Index", "Login");
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        Session["Search"] = null;
        ////        return RedirectToAction("Index", "Login");
        ////    }

        ////   // return View();

        ////}
        //#endregion

        public ActionResult UploadIndPhotoSignature()
        {
            try
            {
                string SchlID = Convert.ToString(Session["SCHL"]);
                if (SchlID == null || SchlID == "")
                {
                    return RedirectToAction("Index", "Login");
                }
                var itemsch = new SelectList(new[]{new {ID="1",Name="N1"},new {ID="2",Name="N2"},new{ID="3",Name="N3"},
            new{ID="4",Name="M2"},new{ID="5",Name="E1"},new{ID="6",Name="E2"},new{ID="7",Name="T2"},}, "ID", "Name", 1);
                ViewBag.MyForm = itemsch.ToList();

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
        public ActionResult UploadIndPhotoSignature(string foname, int uniqueid, string distcode, UploadImgndSignature obj)
        {
            try
            {
                var itemsch = new SelectList(new[]{new {ID="1",Name="N1"},new {ID="2",Name="N2"},new{ID="3",Name="N3"},
            new{ID="4",Name="M2"},new{ID="5",Name="E1"},new{ID="6",Name="E2"},new{ID="7",Name="T2"},}, "ID", "Name", 1);
                string fname = (foname == "1") ? "N1" : (foname == "2") ? "N2" : (foname == "3") ? "N3" : (foname == "4") ? "M2" : (foname == "5") ? "E1" : (foname == "6") ? "E2" : foname = "T2";
                ViewBag.MyForm = itemsch.ToList();
                string stdPic = "";
                string stdSign = "";
                if (obj.std_Photo != null)
                {
                    stdPic = Path.GetFileName(obj.std_Photo.FileName);
                }
                if (obj.std_Sign != null)
                {
                    stdSign = Path.GetFileName(obj.std_Sign.FileName);
                }
                // AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                //string formName = "", dist = "", result = "";
                // int status = 0;
                string result = fname + distcode + uniqueid;
                string filepathtosave = "";
                if (obj.std_Photo != null)
                {
                    //string type = "photo";
                    //var path = Path.Combine(Server.MapPath("~/Upload/"+ formName + "/" + dist + "/Photo"), stdPic);
                    //var path = Path.Combine(Server.MapPath("~/Upload/" + fname + "/" + distcode + "/Photo"), result + "P" + ".jpg");
                    //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + fname + "/" + distcode + "/Photo"));
                    //if (!Directory.Exists(FilepathExist))
                    //{
                    //    Directory.CreateDirectory(FilepathExist);
                    //}
                    //obj.std_Photo.SaveAs(path);
                    string Orgfile = result + "P" + ".jpg";
                                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                                    {
                                        using (var newMemoryStream = new MemoryStream())
                                        {                                            
                                            var uploadRequest = new TransferUtilityUploadRequest
                                            {
                                                InputStream = obj.std_Photo.InputStream,
                                                Key = string.Format("allfiles/Upload2023/" + fname + "/" + distcode + "/Photo/{0}", Orgfile),
                                                BucketName = BUCKET_NAME,
                                                CannedACL = S3CannedACL.PublicRead
                                            };

                                            var fileTransferUtility = new TransferUtility(client);
                                            fileTransferUtility.Upload(uploadRequest);
                                        }
                                    }


                    //filepathtosave = "../Upload/" + fname + "/" + distcode + "/Photo/" + result + "P" + ".jpg";
                    filepathtosave = "allfiles/Upload2023/" + fname + "/" + distcode + "/Photo/" + result + "P" + ".jpg";
                    ViewBag.ImageURL1 = filepathtosave;

                    string PhotoName = fname + "/" + distcode + "/Photo" + "/" + result + "P" + ".jpg";
                    string type = "P";
                    string UpdatePic = objDB.DummyUpdate_Uploaded_Photo_Sign(uniqueid, PhotoName, type);


                }
                if (obj.std_Sign != null)
                {
                    //var path = Path.Combine(Server.MapPath("~/Upload/" + fname + "/" + distcode + "/Sign"), result + "S" + ".jpg");
                    //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + fname + "/" + distcode + "/Sign"));
                    //if (!Directory.Exists(FilepathExist))
                    //{
                    //    Directory.CreateDirectory(FilepathExist);
                    //}
                    //obj.std_Sign.SaveAs(path);

                    string Orgfile = result + "S" + ".jpg";
                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = obj.std_Sign.InputStream,
                                Key = string.Format("allfiles/Upload2023/" + fname + "/" + distcode + "/Sign/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }
                    //filepathtosave = "../Upload/" + fname + "/" + distcode + "/Sign/" + result + "S" + ".jpg";
                    filepathtosave = "allfiles/Upload2023" + fname + "/" + distcode + "/Sign/" + result + "S" + ".jpg";
                    ViewBag.ImageURL2 = filepathtosave;

                    string PhotoName = fname + "/" + distcode + "/Sign" + "/" + result + "S" + ".jpg";
                    string type = "S";
                    string UpdatePic = objDB.DummyUpdate_Uploaded_Photo_Sign(uniqueid, PhotoName, type);
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
        public ActionResult Jqfindimageandsignature(int uniqueid, string form_Name)
        {
            //AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
            int OutStatus = 0;
            string photo = "", signature = "", District = "";
            string SchlID = Convert.ToString(Session["SCHL"]);
            objDB.findimageandsignature(form_Name, uniqueid, SchlID, out photo, out signature, out District, out OutStatus);

            var results = new
            {
                status = OutStatus,
                photo = photo,
                signature = signature,
                District = District
            };
            return Json(results);
        }

        public ActionResult School_UDISE_Code()
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string SchlID = Convert.ToString(Session["SCHL"]);
                string udisecode = "";
                int outstatus = 0;
                objDB.getUDISECode(SchlID, out udisecode, out outstatus);
                ViewBag.Udise = udisecode;
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
        public ActionResult School_UDISE_Code(SchoolModels obj)
        {
            try
            {
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                {
                    return RedirectToAction("Logout", "Login");
                }
                string SCHL = Convert.ToString(Session["SCHL"]);
                string udisecode = obj.udisecode;
                string outstatus = "";
                objDB.insertUdisecode(SCHL, udisecode, out outstatus);
                ViewBag.Udise = udisecode;
                if (outstatus == "1")
                    ViewBag.msg = "UDISE Code Updated Successfully.";
                else if (outstatus == "2")
                    ViewBag.err = "This UDISE Code is already in Used by others.";
                else
                    ViewBag.err = "Something Went Wrong.";
                return View();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        //---------------------------------------School Ninth Result Page--------------//
        //---------------------------------------School Ninth Result Page--------------//
        #region School Ninth Result Page
        public ActionResult ResultUpate()
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string SchlID = Convert.ToString(Session["SCHL"]);
                DataSet result = objCommon.schooltypes(SchlID); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {
                    //ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                    //ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                    ViewBag.NApprovedSchl = result.Tables[6].Rows[0]["NApprovedSchl"].ToString();
                    ViewBag.EApprovedSchl = result.Tables[7].Rows[0]["EApprovedSchl"].ToString();

                }
                TempData["CCE_SeniorSearch"] = null;

                return View();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }
        public ActionResult Ninth_Result_Page(FormCollection frm, string id, int? page)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string SCHL = "";
                string CLASS = "1";

                if (SCHL == "" || SCHL == null)
                { SCHL = Session["SCHL"].ToString(); }

                ViewBag.schlCode = SCHL;
                ViewBag.cid = id;


                var itemFilter = new SelectList(new[] {  new { ID = "5", Name = "By Section" }, new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                    new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";

                var resultList = new SelectList(new[] { new { ID = "1", Name = "Pass" }, new { ID = "2", Name = "Fail" }, new { ID = "3", Name = "Re-appear" }, new { ID = "4", Name = "RL" }, new { ID = "5", Name = "Cancel" },
                    new { ID = "6", Name = "Absent" }, }, "ID", "Name", 1);
                ViewBag.rslist = resultList.ToList();


                //------------------------

                if (SCHL != null)
                {
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                    string Search = string.Empty;
                    Search = "  a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "'";
                    // Search = " a.schl = '" + SCHL + "' ";

                    //if (TempData["CCE_SeniorSearch"] != null)
                    //{
                    //    Search += TempData["CCE_SeniorSearch"].ToString();
                    //}
                    int SelAction = 0;
                    if (TempData["SelAction"] != null)
                    {
                        SelAction = Convert.ToInt32(TempData["SelAction"]);
                    }
                    else
                    {
                        SelAction = 0;
                    }
                    if (TempData["CCE_SeniorSearch"] != null)
                    {
                        Search = TempData["CCE_SeniorSearch"].ToString();
                        ViewBag.SelectedFilter = TempData["SelFilter"];
                        ViewBag.SelectedAction = TempData["SelAction"];
                    }


                    //string class1 = "4"; // For Senior
                    MS.StoreAllData = objDB.Get_Ninth_Result_Page(Search, SCHL, pageIndex, CLASS, SelAction);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        string reclock = MS.StoreAllData.Tables[0].Rows[0]["reclock"].ToString();
                        if (reclock == "1")
                        {
                            ViewBag.reclock = "1";
                        }
                        else
                        { ViewBag.reclock = "0"; }

                        ViewBag.IsFinal = "0";//Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["IsMarksFilled"]);
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
        public ActionResult Ninth_Result_Page(FormCollection frm, int? page, string cmd)
        {
            string Reslist = null;
            string totMarks = null;
            string obtainMarks = null;

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string id = "";
                string SCHL = Session["SCHL"].ToString();
                ViewBag.schlCode = SCHL;
                string CLASS = "1";


                var itemFilter = new SelectList(new[] {  new { ID = "5", Name = "By Section" }, new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                    new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";

                var resultList = new SelectList(new[] { new { ID = "1", Name = "Pass" }, new { ID = "2", Name = "Fail" }, new { ID = "3", Name = "Re-appear" }, new { ID = "4", Name = "RL" }, new { ID = "5", Name = "Cancel" },
                    new { ID = "6", Name = "Absent" }, }, "ID", "Name", 1);
                ViewBag.rslist = resultList.ToList();

                //---------------------Unlock Final Submit Ninth and Eleventh Result-----------------
                if (cmd.ToLower().Contains("unlock"))
                {
                    // CLASS = 1 (Ninth)
                    DataSet result = objDB.UnlockFinalSubmitNinthandEleventhResult(CLASS, SCHL);
                    if (result.Tables[0].Rows.Count > 0)
                    {

                        TempData["result"] = result.Tables[0].Rows[0]["res"].ToString();
                    }
                    return RedirectToAction("Ninth_Result_Page", "school");
                }
                //----------------------Unlock Final Submit Ninth and Eleventh Result------------


                //---------------------Final Submit-----------------
                if (cmd == "Click here to Final Submit")
                {
                    DataSet result = objDB.FinalSubmitNinthResult(CLASS, SCHL);
                    if (result.Tables[0].Rows.Count > 0)
                    {
                        if (result.Tables[0].Rows[0]["res"].ToString() == "0")
                        {
                            TempData["result"] = "0";
                        }
                        else
                        {
                            TempData["TotImported"] = result.Tables[0].Rows[0]["res"].ToString();
                            TempData["result"] = "1";
                        }

                    }

                    return RedirectToAction("Ninth_Result_Page", "school");
                }
                //----------------------End Final Submit------------
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

                        TempData["SelAction"] = frm["SelAction"];
                        ViewBag.SelectedAction = frm["SelAction"];
                        int SelValueSch = Convert.ToInt32(frm["SelAction"].ToString());
                        if (frm["SelAction"] != "")
                        {
                            if (SelValueSch == 1)
                            {
                                SelAction = 1; /// Action for All
							}
                            if (SelValueSch == 2)
                            {
                                SelAction = 2;
                            }
                            //{ Search += " and (IsMarksFilled is null or IsMarksFilled=0) "; } // pending
                            if (SelValueSch == 3)
                            {
                                SelAction = 3;
                            }
                            //  { Search += " and  IsMarksFilled=1 "; } // Filled

                        }
                    }

                    if (frm["SelFilter"] != "")
                    {
                        TempData["SelFilter"] = frm["SelFilter"];
                        ViewBag.SelectedFilter = frm["SelFilter"];
                        int SelValueSch = Convert.ToInt32(frm["SelFilter"].ToString());
                        if (frm["SelFilter"] != "" && frm["SearchString"].ToString() != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and a.Class_Roll_Num_Section='" + frm["SearchString"].ToString() + "'"; }
                            if (SelValueSch == 2)
                            { Search += " and a.Std_id='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 3)
                            { Search += " and  a.Registration_num like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " and  a.Candi_Name like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 5)
                            { Search += " and a.section='" + frm["SearchString"].ToString() + "'"; }
                        }
                    }

                    TempData["CCE_SeniorSearch"] = Search;
                    // string class1 = "4";
                    TempData["SelAction"] = SelAction;
                    MS.StoreAllData = objDB.Get_Ninth_Result_Page(Search, SCHL, pageIndex, CLASS, SelAction);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        //string reclock = MS.StoreAllData.Tables[1].Rows[0]["reclock"].ToString();
                        string reclock = MS.StoreAllData.Tables[0].Rows[0]["reclock"].ToString();
                        if (reclock == "1")
                        {
                            ViewBag.reclock = "1";
                        }
                        else
                        { ViewBag.reclock = "0"; }
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
        public JsonResult UpdateMatricResult(string ResultList, string totmarks, string obtmarks, string stdid, string schl, string UPTREMARKS)
        {
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

            try
            {

                string dee = "";
                string outstatus = "";
                string Search = string.Empty;
                string res = null;
                //string UserType = "Admin";               
                //float fee = 0;              
                //DateTime date;              
                DataSet result = objDB.UpdNinthResult(ResultList, totmarks, obtmarks, stdid, schl, "SCHL",UPTREMARKS);
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
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return null;
            }
        }
        public ActionResult Ninth_Result_Page_Report(string id)
        {

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string SCHL = "";
                string CLASS = "1";


                if (SCHL == "" || SCHL == null)
                {
                    SCHL = Session["SCHL"].ToString();
                }

                ViewBag.schlCode = SCHL;
                DataSet Dresult = objDB.GetSchoolSection(CLASS, SCHL); // passing Value to DBClass from model            
                List<SelectListItem> SecList = new List<SelectListItem>();
                //SecList.Add(new SelectListItem { Text = "ALL", Value = "ALL" });
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {

                    SecList.Add(new SelectListItem { Text = @dr["section"].ToString(), Value = @dr["section"].ToString() });
                }

                ViewBag.sec = SecList;
                Session["rpt"] = id;
                ViewBag.TotalCount = 0;
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }
        [HttpPost]
        public ActionResult Ninth_Result_Page_Report(FormCollection frm)
        {
            TempData["CCE_SeniorSearch"] = null;
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if (Session["rpt"] == null)
            {
                return RedirectToAction("ResultUpate", "School");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string SCHL = "";
                string CLASS = "1";


                if (SCHL == "" || SCHL == null)
                {
                    SCHL = Session["SCHL"].ToString();
                }

                ViewBag.schlCode = SCHL;
                string id = Session["rpt"].ToString();
                string Section = frm["Selsec"].ToString();

                DataSet Dresult = objDB.GetSchoolSection(CLASS, SCHL); // passing Value to DBClass from model            
                List<SelectListItem> SecList = new List<SelectListItem>();
                // SecList.Add(new SelectListItem { Text = "ALL", Value = "ALL" });
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {
                    SecList.Add(new SelectListItem { Text = @dr["Section"].ToString(), Value = @dr["Section"].ToString() });
                }
                ViewBag.sec = SecList;
                MS.Selsec = Section;
                ViewBag.SchlSec = Section;
                string Search = string.Empty;
                Search = "a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "' and  a.Section= '" + Section + "'";
                MS.StoreAllData = objDB.Get_Ninth_Result_Page_Report(Search, SCHL, CLASS, id);
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
                    ViewBag.Reportid = id;
                    ViewBag.Fdate = MS.StoreAllData.Tables[0].Rows[0]["UpdateDT"].ToString();

                    return View(MS);
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }
        #endregion School Ninth Result Page

        //---------------------------------------School Eleventh Result Page--------------//
        //---------------------------------------School Eleventh Result Page--------------//
        #region School Eleventh Result Page
        public ActionResult Eleventh_Result_Page(FormCollection frm, string id, int? page)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string SCHL = "";
                string CLASS = "3";

                if (SCHL == "" || SCHL == null)
                { SCHL = Session["SCHL"].ToString(); }

                ViewBag.schlCode = SCHL;
                ViewBag.cid = id;


                var itemFilter = new SelectList(new[] {  new { ID = "5", Name = "By Section" }, new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                    new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";

                var resultList = new SelectList(new[] { new { ID = "1", Name = "Pass" }, new { ID = "2", Name = "Fail" }, new { ID = "3", Name = "Compartment" }, new { ID = "4", Name = "RL" }, new { ID = "5", Name = "Cancel" },
                    new { ID = "6", Name = "Absent" }, }, "ID", "Name", 1);
                ViewBag.rslist = resultList.ToList();


                //------------------------

                if (SCHL != null)
                {
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                    string Search = string.Empty;
                    Search = "  a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "'";
                    // Search = " a.schl = '" + SCHL + "' ";

                    //if (TempData["CCE_SeniorSearch"] != null)
                    //{
                    //    Search += TempData["CCE_SeniorSearch"].ToString();
                    //}
                    int SelAction = 0;
                    if (TempData["SelAction"] != null)
                    {
                        SelAction = Convert.ToInt32(TempData["SelAction"]);
                    }
                    else
                    {
                        SelAction = 0;
                    }

                    if (TempData["CCE_SeniorSearch"] != null)
                    {
                        Search = TempData["CCE_SeniorSearch"].ToString();
                        ViewBag.SelectedFilter = TempData["SelFilter"];
                        ViewBag.SelectedAction = TempData["SelAction"];
                    }


                    //string class1 = "4"; // For Senior
                    MS.StoreAllData = objDB.Get_Eleventh_Result_Page(Search, SCHL, pageIndex, CLASS, SelAction);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        string reclock = MS.StoreAllData.Tables[0].Rows[0]["reclock"].ToString();
                        if (reclock == "1")
                        {
                            ViewBag.reclock = "1";
                        }
                        else
                        { ViewBag.reclock = "0"; }

                        ViewBag.IsFinal = "0";//Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["IsMarksFilled"]);
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
        public ActionResult Eleventh_Result_Page(FormCollection frm, int? page, string cmd)
        {
            string Reslist = null;
            string totMarks = null;
            string obtainMarks = null;

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string id = "";
                //if (frm["cid"] != "")
                //{
                //    id = frm["cid"].ToString();
                //    ViewBag.cid = frm["cid"].ToString();
                //}
                string SCHL = Session["SCHL"].ToString();
                ViewBag.schlCode = SCHL;
                string CLASS = "3";


                var itemFilter = new SelectList(new[] {  new { ID = "5", Name = "By Section" }, new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                    new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";

                var resultList = new SelectList(new[] { new { ID = "1", Name = "Pass" }, new { ID = "2", Name = "Fail" }, new { ID = "3", Name = "Compartment" }, new { ID = "4", Name = "RL" }, new { ID = "5", Name = "Cancel" },
                    new { ID = "6", Name = "Absent" }, }, "ID", "Name", 1);
                ViewBag.rslist = resultList.ToList();


                //---------------------Unlock Final Submit Ninth and Eleventh Result-----------------
                if (cmd.ToLower().Contains("unlock"))
                {
                    // CLASS = 3 (Eleventh)
                    DataSet result = objDB.UnlockFinalSubmitNinthandEleventhResult(CLASS, SCHL);
                    if (result.Tables[0].Rows.Count > 0)
                    {

                        TempData["result"] = result.Tables[0].Rows[0]["res"].ToString();
                    }
                    return RedirectToAction("Eleventh_Result_Page", "school");
                }
                //----------------------Unlock Final Submit Ninth and Eleventh Result------------


                //---------------------Final Submit-----------------
                if (cmd == "Click here to Final Submit")
                {
                    DataSet result = objDB.FinalSubmitEleventhResult(CLASS, SCHL);
                    if (result.Tables[0].Rows.Count > 0)
                    {
                        if (result.Tables[0].Rows[0]["res"].ToString() == "0")
                        {
                            TempData["result"] = "0";
                        }
                        else
                        {
                            TempData["TotImported"] = result.Tables[0].Rows[0]["res"].ToString();
                            TempData["result"] = "1";
                        }

                    }

                    return RedirectToAction("Eleventh_Result_Page", "school");
                }
                //----------------------End Final Submit------------
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

                        TempData["SelAction"] = frm["SelAction"];
                        ViewBag.SelectedAction = frm["SelAction"];
                        int SelValueSch = Convert.ToInt32(frm["SelAction"].ToString());
                        if (frm["SelAction"] != "")
                        {
                            if (SelValueSch == 1)
                            {
                                SelAction = 1; /// Action for All
							}
                            if (SelValueSch == 2)
                            {
                                SelAction = 2;
                            }
                            //{ Search += " and (IsMarksFilled is null or IsMarksFilled=0) "; } // pending
                            if (SelValueSch == 3)
                            {
                                SelAction = 3;
                            }
                            //  { Search += " and  IsMarksFilled=1 "; } // Filled

                        }
                    }

                    if (frm["SelFilter"] != "")
                    {
                        TempData["SelFilter"] = frm["SelFilter"];
                        ViewBag.SelectedFilter = frm["SelFilter"];
                        int SelValueSch = Convert.ToInt32(frm["SelFilter"].ToString());
                        if (frm["SelFilter"] != "" && frm["SearchString"].ToString() != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and a.Class_Roll_Num_Section='" + frm["SearchString"].ToString() + "'"; }
                            if (SelValueSch == 2)
                            { Search += " and a.Std_id='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 3)
                            { Search += " and  a.Registration_num like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " and  a.Candi_Name like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 5)
                            { Search += " and a.section='" + frm["SearchString"].ToString() + "'"; }
                        }
                    }

                    TempData["CCE_SeniorSearch"] = Search;
                    // string class1 = "4";
                    TempData["SelAction"] = SelAction;
                    MS.StoreAllData = objDB.Get_Eleventh_Result_Page(Search, SCHL, pageIndex, CLASS, SelAction);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        //string reclock = MS.StoreAllData.Tables[1].Rows[0]["reclock"].ToString();
                        string reclock = MS.StoreAllData.Tables[0].Rows[0]["reclock"].ToString();
                        if (reclock == "1")
                        {
                            ViewBag.reclock = "1";
                        }
                        else
                        { ViewBag.reclock = "0"; }
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
        public JsonResult UpdateEleventhResult(string ResultList, string totmarks, string obtmarks, string stdid, string schl, string UPTREMARKS)
        {
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

            try
            {

                string dee = "";
                string outstatus = "";
                string Search = string.Empty;
                string res = null;
                //string UserType = "Admin";               
                //float fee = 0;              
                //DateTime date;              
                DataSet result = objDB.UpdateEleventhResult(ResultList, totmarks, obtmarks, stdid, schl, "SCHL", UPTREMARKS);
                res = result.Tables[0].Rows.Count.ToString();
                if (result.Tables[0].Rows.Count.ToString() != "0")
                {
                    dee = "Yes";
                }
                else
                { dee = "No"; }


                return Json(new { sn = dee, chid = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return null;
            }
        }
        public ActionResult Eleventh_Result_Page_Report(string id)
        {

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string SCHL = "";
                string CLASS = "3";


                if (SCHL == "" || SCHL == null)
                {
                    SCHL = Session["SCHL"].ToString();
                }

                ViewBag.schlCode = SCHL;
                DataSet Dresult = objDB.GetSchoolSection(CLASS, SCHL); // passing Value to DBClass from model            
                List<SelectListItem> SecList = new List<SelectListItem>();
                //SecList.Add(new SelectListItem { Text = "ALL", Value = "ALL" });
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {

                    SecList.Add(new SelectListItem { Text = @dr["section"].ToString(), Value = @dr["section"].ToString() });
                }

                ViewBag.sec = SecList;
                Session["rpt"] = id;
                ViewBag.TotalCount = 0;
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }
        [HttpPost]
        public ActionResult Eleventh_Result_Page_Report(FormCollection frm)
        {
            TempData["CCE_SeniorSearch"] = null;
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string SCHL = "";
                string CLASS = "3";


                if (SCHL == "" || SCHL == null)
                {
                    SCHL = Session["SCHL"].ToString();
                }

                ViewBag.schlCode = SCHL;
                string id = Session["rpt"].ToString();
                string Section = frm["Selsec"].ToString();

                DataSet Dresult = objDB.GetSchoolSection(CLASS, SCHL); // passing Value to DBClass from model            
                List<SelectListItem> SecList = new List<SelectListItem>();
                // SecList.Add(new SelectListItem { Text = "ALL", Value = "ALL" });
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {
                    SecList.Add(new SelectListItem { Text = @dr["Section"].ToString(), Value = @dr["Section"].ToString() });
                }
                ViewBag.sec = SecList;
                MS.Selsec = Section;
                ViewBag.SchlSec = Section;
                string Search = string.Empty;
                Search = "a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "' and  a.Section= '" + Section + "'";
                MS.StoreAllData = objDB.Get_Eleventh_Result_Page_Report(Search, SCHL, CLASS, id);
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
                    ViewBag.Reportid = id;
                    ViewBag.Fdate = MS.StoreAllData.Tables[0].Rows[0]["UpdateDT"].ToString();

                    return View(MS);
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }
        public ActionResult Eleventh_Result_Page_Report_section(string id)
        {

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string SCHL = "";
                string CLASS = "1";


                if (SCHL == "" || SCHL == null)
                {
                    SCHL = Session["SCHL"].ToString();
                }

                ViewBag.schlCode = SCHL;
                DataSet Dresult = objDB.GetSchoolSection(CLASS, SCHL); // passing Value to DBClass from model            
                List<SelectListItem> SecList = new List<SelectListItem>();
                //SecList.Add(new SelectListItem { Text = "ALL", Value = "ALL" });
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {

                    SecList.Add(new SelectListItem { Text = @dr["section"].ToString(), Value = @dr["section"].ToString() });
                }

                ViewBag.sec = SecList;
                Session["rpt"] = id;
                //ViewBag.TotalCount = 0;
                string Search = string.Empty;

                Search = "a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "' ";
                MS.StoreAllData = objDB.Eleventh_Result_Page_Report_Section(Search, SCHL, CLASS, id);
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
                    ViewBag.Reportid = id;
                    ViewBag.Fdate = MS.StoreAllData.Tables[0].Rows[0]["UpdateDT"].ToString();

                    return View(MS);
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }
        #endregion School Eleventh Result Page


        //---------------------------------------------Result Declare----------------------------//
        #region Result Declare 2016-17
        public ActionResult SchoolResultDeclare()
        {
            RegistrationModels rm = new RegistrationModels();
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            string schid = Session["SCHL"].ToString();
            DataSet result = objDB.schoolResultType(schid); // passing Value to DBClass from model
            if (result == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (result.Tables[0].Rows.Count > 0)
            {
                if (result.Tables[0].Rows.Count == 2)
                {
                    ViewBag.Matric = result.Tables[0].Rows[0]["MRP"].ToString();
                    ViewBag.OMatric = result.Tables[0].Rows[1]["MRP"].ToString();
                }
                else if (result.Tables[0].Rows.Count == 1)
                {
                    if (result.Tables[0].Rows[0]["MRP"].ToString() == "R")
                    {
                        ViewBag.Matric = result.Tables[0].Rows[0]["MRP"].ToString();
                    }
                    else if (result.Tables[0].Rows[0]["MRP"].ToString() == "O")
                    {
                        ViewBag.OMatric = result.Tables[0].Rows[0]["MRP"].ToString();
                    }


                }
            }
            if (result.Tables[1].Rows.Count > 0)
            {
                if (result.Tables[1].Rows.Count == 2)
                {
                    ViewBag.Senior = result.Tables[1].Rows[0]["SRP"].ToString();
                    ViewBag.OSenior = result.Tables[1].Rows[1]["SRP"].ToString();
                }
                else if (result.Tables[1].Rows.Count == 1)
                {
                    if (result.Tables[1].Rows[0]["SRP"].ToString() == "R")
                    {
                        ViewBag.Senior = result.Tables[1].Rows[0]["SRP"].ToString();
                    }
                    else if (result.Tables[1].Rows[0]["SRP"].ToString() == "O")
                    {
                        ViewBag.OSenior = result.Tables[1].Rows[0]["SRP"].ToString();
                    }

                }
            }
            return View();
        }
        public ActionResult ResultDeclareSchoolWise(string Rtype)
        {
            string SCHL = "";
            var itemFilter = new SelectList(new[] { new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                 new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
            ViewBag.MyFilter = itemFilter.ToList();
            ViewBag.SelectedFilter = "0";

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            MS.status = Rtype;
            ViewBag.status = Rtype;
            string Type = Rtype;
            string Myclass = "";
            string rp = "";
            if (Type == "MR")
            {
                @ViewBag.Res = "Matric Regular";
                Myclass = "10";
                rp = "R";
                @ViewBag.rp = "Regular";
            }
            else if (Type == "MO")
            {
                @ViewBag.Res = "Matric Open";
                Myclass = "10";
                rp = "O";
                @ViewBag.rp = "Open";
            }
            else if (Type == "SR")
            {
                @ViewBag.Res = "SR.Secondary Regular";
                Myclass = "12";
                rp = "R";
                @ViewBag.rp = "Regular";
            }
            else if (Type == "SO")
            {
                @ViewBag.Res = "SR.Secondary Open";
                Myclass = "12";
                rp = "O";
                @ViewBag.rp = "Open";
            }

            SCHL = Session["SCHL"].ToString();
            string Search = string.Empty;
            Search = "a.SCHL Like '%' ";
            ViewBag.schlCode = SCHL;
            try
            {
                MS.StoreAllData = objDB.GetSchoolResultDetails(Search, SCHL, Myclass, rp);
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    Session["SchoolResultDetails"] = null;
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    Session["SchoolResultDetails"] = MS.StoreAllData;
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.SCHLNCODE = MS.StoreAllData.Tables[0].Rows[0]["SCHLNCODE"];
                    ViewBag.SCHLSET = MS.StoreAllData.Tables[0].Rows[0]["SCHLSET"];
                    //int count = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["TotalCnt"]);                  

                    return View(MS);
                }

            }
            catch (Exception ex)
            {
                //return RedirectToAction("Index", "Login");
            }


            return View();
        }
        [HttpPost]
        public ActionResult ResultDeclareSchoolWise(string Rtype, FormCollection frm, string cmd, string submit)
        {

            string SCHL = "";
            var itemFilter = new SelectList(new[] { new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                 new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
            ViewBag.MyFilter = itemFilter.ToList();
            ViewBag.SelectedFilter = "0";

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();

            string Type = frm["status"];
            //MS.status= Type;
            string Myclass = "";
            string rp = "";
            if (Type == "MR")
            {
                @ViewBag.Res = "Matric Regular";
                Myclass = "10";
                rp = "R";
                @ViewBag.rp = "Regular";
            }
            else if (Type == "MO")
            {
                @ViewBag.Res = "Matric Open";
                Myclass = "10";
                rp = "O";
                @ViewBag.rp = "Open";
            }
            else if (Type == "SR")
            {
                @ViewBag.Res = "SR.Secondary Regular";
                Myclass = "12";
                rp = "R";
                @ViewBag.rp = "Regular";
            }
            else if (Type == "SO")
            {
                @ViewBag.Res = "SR.Secondary Open";
                Myclass = "12";
                rp = "O";
                @ViewBag.rp = "Open";
            }

            SCHL = Session["SCHL"].ToString();
            string Search = string.Empty;



            if (frm["SelFilter"] != "")
            {
                TempData["SelFilter"] = frm["SelFilter"];
                ViewBag.SelectedFilter = frm["SelFilter"];
                int SelValueSch = Convert.ToInt32(frm["SelFilter"].ToString());
                if (frm["SelFilter"] != "" && frm["SearchString"].ToString() != "")
                {
                    if (SelValueSch == 1)
                    { Search += "a.roll='" + frm["SearchString"].ToString() + "'"; }
                    if (SelValueSch == 2)
                    { Search += "d.Std_id='" + frm["SearchString"].ToString() + "'"; }
                    else if (SelValueSch == 3)
                    { Search += "d.Regno like '%" + frm["SearchString"].ToString() + "%'"; }
                    else if (SelValueSch == 4)
                    { Search += "d.Name like '%" + frm["SearchString"].ToString() + "%'"; }
                    //if (SelValueSch == 2)
                    //{ Search += "b.Std_id='" + frm["SearchString"].ToString() + "'"; }
                    //else if (SelValueSch == 3)
                    //{ Search += "b.Registration_num like '%" + frm["SearchString"].ToString() + "%'"; }
                    //else if (SelValueSch == 4)
                    //{ Search += "b.Candi_Name like '%" + frm["SearchString"].ToString() + "%'"; }

                }
                try
                {

                    MS.StoreAllData = objDB.GetSchoolResultDetails(Search, SCHL, Myclass, rp);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        Session["SchoolResultDetails"] = null;
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        Session["SchoolResultDetails"] = MS.StoreAllData;
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.SCHLNCODE = MS.StoreAllData.Tables[0].Rows[0]["SCHLNCODE"];
                        ViewBag.SCHLSET = MS.StoreAllData.Tables[0].Rows[0]["SCHLSET"];
                        //int count = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["TotalCnt"]);                  

                        return View(MS);
                    }

                }
                catch (Exception ex)
                {
                    //return RedirectToAction("Index", "Login");
                }
            }


            return View(MS);
        }

        public ActionResult DownloadSchoolResultDetails()
        {
            try
            {
                if (Request.QueryString["File"] == null)
                {
                    return RedirectToAction("SchoolResultDeclare", "School");
                }
                else
                {
                    string UserType = "";
                    UserType = "User";
                    string FileExport = Request.QueryString["File"].ToString();
                    DataSet ds = null;
                    if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                    {
                        return RedirectToAction("Logout", "Login");
                    }
                    else
                    {
                        string fileName1 = string.Empty;
                        ds = (DataSet)Session["SchoolResultDetails"];
                        if (ds == null)
                        {
                            return RedirectToAction("SchoolResultDeclare", "School");
                        }
                        else
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                fileName1 = "SchoolResultDetails" + '_' + DateTime.Now.ToString("ddMMyyyy");
                                ExportDataFromDataTable(ds.Tables[0], fileName1);
                            }
                        }
                    }
                }
                return RedirectToAction("SchoolResultDeclare", "School");
            }
            catch (Exception ex)
            {
                return RedirectToAction("SchoolResultDeclare", "School");
            }
        }



        public ActionResult ExportDataFromDataTable(DataTable dt, string filename)
        {
            try
            {
                if (dt.Rows.Count == 0)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    if (dt.Rows.Count > 0)
                    {
                        //string fileName1 = "ERRORPVT_" + firm + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";  //103_230820162209_347
                        string fileName1 = filename + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xlsx";  //103_230820162209_347
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            wb.Worksheets.Add(dt);
                            wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                            wb.Style.Font.Bold = true;
                            Response.Clear();
                            Response.Buffer = true;
                            Response.Charset = "";
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.AddHeader("content-disposition", "attachment;filename=" + fileName1 + "");
                            //Response.AddHeader("content-disposition", "attachment;filename= DownloadChallanReport.xlsx");

                            using (MemoryStream MyMemoryStream = new MemoryStream())
                            {
                                wb.SaveAs(MyMemoryStream);
                                MyMemoryStream.WriteTo(Response.OutputStream);
                                Response.Flush();
                                Response.End();
                            }
                        }

                    }
                }

                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Admin");
            }

        }

        #endregion Result Declare 2016-17
        //--------------------------------------------- End Result Declare----------------------------//

        #region Rohit School Correction

        public JsonResult GetCorrectionTypeByClass(string cls)
        {
            List<SelectListItem> SecList = AbstractLayer.DBClass.GetCorrectionTypeByClass(cls);
            ViewBag.CorrectionType = SecList.ToList();
            return Json(SecList);
        }

        public ActionResult SchoolCorrection(string id, int? page)
        {
            ViewBag.nsfqPatternList = objCommon.GetNsqfPatternList();
            try
            {
                string classAssign = "";
                AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
                AdminModels am = new AdminModels();
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                string SchlID = Convert.ToString(Session["SCHL"]);
                DataSet result = objCommon.schooltypes(SchlID); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {
                    ViewBag.NApprovedSchl = result.Tables[4].Rows[0]["Nth"].ToString();
                    ViewBag.EApprovedSchl = result.Tables[5].Rows[0]["Eth"].ToString();
                }


                FormCollection frc = new FormCollection();
                //    var itemsch = new SelectList(new[]{new {ID="1",Name="Particular"},new {ID="2",Name="Subject"},
                //new{ID="4",Name="Image"},}, "ID", "Name", 1);
                List<SelectListItem> itemsch = new List<SelectListItem>();
                ViewBag.CorrectionType = itemsch.ToList();

                //
                var itemClass = new SelectList(new[] { new { ID = "1", Name = "9th Class" }, new { ID = "2", Name = "11th Class" }, }, "ID", "Name", 1);
                ViewBag.CorrectionClass = itemClass.ToList();

                if (ModelState.IsValid)
                {
                    if (Session["SchlCorLot"] != null)
                    {
                        string CorLot2 = Session["SchlCorLot"].ToString();
                        ViewBag.SelectedItemcode = Session["SchlCorrectionType1"].ToString();
                        am.CorrectionLot = Session["SchlCorLot"].ToString();
                        string Search = string.Empty;
                        Search = " a.SCHL  ='" + Session["SCHL"].ToString() + "'";
                        string CrType = Session["SchlCorrectionType1"].ToString();
                        Search += " and a.correctionlot='" + CorLot2 + "' ";

                        if (Session["SchlCorrectionClass"] != null)
                        {
                            classAssign = Session["SchlCorrectionClass"].ToString();
                            if (classAssign == "2")
                            {
                                Search += " and a.class in ('11th Class')";
                            }
                            else if (classAssign == "1")
                            {
                                Search += " and a.class in ('9th Class')";
                            }
                            else if (classAssign == "10")
                            {
                                Search += " and a.class in ('Matriculation Regular')";
                            }
                            else if (classAssign == "12")
                            {
                                Search += " and a.class in ('Sr.Secondary Regular')";
                            }

                            else if (classAssign == "22")
                            {
                                Search += " and a.class in ('Matriculation Open')";
                            }
                            else if (classAssign == "44")
                            {
                                Search += " and a.class in ('Sr.Secondary Open')";
                            }
                        }

                        //------ Paging Srt
                        int pageIndex = 1;
                        pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        ViewBag.pagesize = pageIndex;
                        //string Catg = CrType;                        

                        //---- Paging end
                        am.StoreAllData = objDB.GetCorrectionDataFirm(Search, CrType, pageIndex);
                        if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                            ViewBag.TotalCountP = 0;
                            return View(am);
                        }
                        else
                        {
                            ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
                            ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
                            ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
                            int tp = Convert.ToInt32(ViewBag.TotalCount);
                            int pn = tp / 30;
                            int cal = 30 * pn;
                            int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
                            if (res >= 1)
                                ViewBag.pn = pn + 1;
                            else
                                ViewBag.pn = pn;

                            return View(am);
                        }
                    }
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View();


                }
                else
                {
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
        [HttpPost]
        public ActionResult SchoolCorrection(int? page, FormCollection frc, string cmd, string id, string CorrectionClass)
        {

            ViewBag.nsfqPatternList = objCommon.GetNsqfPatternList();
            try
            {
                AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
                AdminModels am = new AdminModels();
                string classAssign = "";
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                string SchlID = Convert.ToString(Session["SCHL"]);
                DataSet result = objCommon.schooltypes(SchlID); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {
                    ViewBag.NApprovedSchl = result.Tables[4].Rows[0]["Nth"].ToString();
                    ViewBag.EApprovedSchl = result.Tables[5].Rows[0]["Eth"].ToString();
                }


                //if (result.Tables[9].Rows.Count > 0)
                //{
                //    ViewBag.NLastDate = TempData["NLastDate"] = result.Tables[9].AsEnumerable().Where(x => x.Field<int>("Class") == 9).Select(x => x.Field<DateTime>("VerifyLastDateBySchl")).FirstOrDefault();
                //    ViewBag.ELastDate = TempData["ELastDate"] = result.Tables[9].AsEnumerable().Where(x => x.Field<int>("Class") == 11).Select(x => x.Field<DateTime>("VerifyLastDateBySchl")).FirstOrDefault();
                //}

                //    var itemsch = new SelectList(new[]{new {ID="1",Name="Particular"},new {ID="2",Name="Subject"},
                //new{ID="4",Name="Image"},}, "ID", "Name", 1);
                List<SelectListItem> SecList = AbstractLayer.DBClass.GetCorrectionTypeByClass(CorrectionClass);
                ViewBag.CorrectionType = SecList.ToList();


                //
                //var itemClass = new SelectList(new[] { new { ID = "1", Name = "9th Class" }, new { ID = "2", Name = "11th Class" }, }, "ID", "Name", 1);
                //ViewBag.CorrectionClass = itemClass.ToList();


                if (ModelState.IsValid)
                {
                    //------ Paging Srt
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                    //string Catg = CrType;                        

                    //---- Paging end

                    #region Search Record 

                    if (cmd == "Search" && frc["CorrectionType1"] != null && frc["CorrectionClass"] != null)
                    {
                        ViewBag.CorrectionClass = CorrectionClass;
                        ViewBag.SelectedItemcode = frc["CorrectionType1"].ToString();
                        string Search = string.Empty;
                        Search = " a.SCHL  ='" + Session["SCHL"].ToString() + "'";
                        string CrType = frc["CorrectionType1"].ToString();
                        Session["SchlCorrectionType1"] = ViewBag.SelectedItemcode;
                        Session["SchlCorLot"] = am.CorrectionLot = frc["CorrectionLot"].ToString();

                        Search += "  and a.correctionlot ='" + am.CorrectionLot + "' ";

                        if (frc["CorrectionClass"] != null)
                        {
                            classAssign = frc["CorrectionClass"].ToString();
                            if (classAssign == "2")
                            {
                                Search += " and a.class in ('11th Class')";
                            }
                            else if (classAssign == "1")
                            {
                                Search += " and a.class in ('9th Class')";
                            }
                            else if (classAssign == "10")
                            {
                                Search += " and a.class in ('Matriculation Regular')";
                            }
                            else if (classAssign == "12")
                            {
                                Search += " and a.class in ('Sr.Secondary Regular')";
                            }
                            else if (classAssign == "22")
                            {
                                Search += " and a.class in ('Matriculation Open')";
                            }
                            else if (classAssign == "44")
                            {
                                Search += " and a.class in ('Sr.Secondary Open')";
                            }

                        }
                        ViewBag.CorrectionFinalSubmitDt = "";
                        am.StoreAllData = objDB.GetCorrectionDataFirm(Search, CrType, pageIndex);
                        if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                            ViewBag.TotalCountP = 0;
                            return View(am);
                        }
                        else
                        {
                            ViewBag.CorrectionFinalSubmitDt = am.StoreAllData.Tables[0].Rows[0]["CorrectionFinalSubmitDt"].ToString();
                            ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
                            ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
                            ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
                            int tp = Convert.ToInt32(ViewBag.TotalCount);
                            int pn = tp / 30;
                            int cal = 30 * pn;
                            int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
                            if (res >= 1)
                                ViewBag.pn = pn + 1;
                            else
                                ViewBag.pn = pn;

                            return View(am);
                        }
                    }

                    #endregion Search Record                   
                    return View(am);
                }
                else
                {
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

        [HttpPost]
        public ActionResult CorrLotAcceptReject(string correctionType, string correctionLot, string acceptid, string rejectid, string removeid, string remarksid)
        {
            string remstatus = "";
            string status = "";
            string schl = Session["schl"].ToString();
            if (removeid != "")
            {
                string OutStatus = "0";
                status = new AbstractLayer.AdminDB().CorrLotAcceptReject("", correctionType, correctionLot, acceptid, rejectid, removeid, schl, out OutStatus);
                var results = new
                {
                    status = OutStatus,
                };

                return Json(results);
            }
            else
            {

                if (correctionLot == null || correctionLot == "")
                {
                    var results = new
                    {
                        status = ""
                    };
                    return Json(results);
                }
                else
                {
                    string OutStatus = "0";
                    status = new AbstractLayer.AdminDB().CorrLotAcceptReject("", correctionType, correctionLot, acceptid, rejectid, removeid, schl, out OutStatus);
                    if (Convert.ToInt32(status) > 0 && remarksid != "")
                    {
                        //123(rohit),456(mm)
                        string[] split1 = remarksid.Split(',');
                        int sCount = split1.Length;
                        if (sCount > 0)
                        {
                            foreach (string s in split1)
                            {
                                string corid = s.Split('(')[0];
                                string remark = s.Split('(', ')')[1];
                                if (corid != "")
                                {
                                    remstatus = new AbstractLayer.AdminDB().CorrLotRejectRemarksSP(corid, remark, schl);//CorrLotRejectRemarksSP
                                }
                            }
                        }
                    }
                    var results = new
                    {
                        status = OutStatus,
                    };
                    return Json(results);
                }
            }
        }

        #endregion

        #region  Rohit  School Correction Updated 



        public ActionResult SchoolCorrectionUpdated(int? page)
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                string SchlID = Convert.ToString(Session["SCHL"]);
                DataSet result = objCommon.schooltypes(SchlID); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {
                    ViewBag.NApprovedSchl = result.Tables[4].Rows[0]["Nth"].ToString();
                    ViewBag.EApprovedSchl = result.Tables[5].Rows[0]["Eth"].ToString();
                }

                //if (result.Tables[9].Rows.Count > 0)
                //{
                //    ViewBag.NLastDate = TempData["NLastDate"] = result.Tables[9].AsEnumerable().Where(x => x.Field<int>("Class") == 9).Select(x => x.Field<DateTime>("VerifyLastDateBySchl")).FirstOrDefault();
                //    ViewBag.ELastDate = TempData["ELastDate"] = result.Tables[9].AsEnumerable().Where(x => x.Field<int>("Class") == 11).Select(x => x.Field<DateTime>("VerifyLastDateBySchl")).FirstOrDefault();
                //}

                var itemFee = new SelectList(new[] { new { ID = "1", Name = "With Fee" }, new { ID = "2", Name = "Widout Fee" }, }, "ID", "Name", 1);
                ViewBag.FeeType = itemFee.ToList();
                FormCollection frc = new FormCollection();
                var itemsch = new SelectList(new[]{new {ID="1",Name="Particular"},new {ID="2",Name="Subject"},
              new{ID="4",Name="Image"},}, "ID", "Name", 1);
                ViewBag.CorrectionType = itemsch.ToList();
                AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
                AdminModels am = new AdminModels();
                if (ModelState.IsValid)
                {
                    string Search = string.Empty;
                    string CrType = "2";
                    Search = "a.status is not null";
                    Search += " and a.schl  = '" + Session["SCHL"].ToString() + "'";

                    //------ Paging Srt
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                    //---- Paging end
                    am.StoreAllData = objDB.GetCorrectionDataFirmUpdated(Search, CrType, pageIndex);
                    // ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;

                    if (am.StoreAllData == null)
                    {
                        ViewBag.TotalCountP = 0;
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(am);
                    }
                    else
                    {
                        DataSet dschk = objDB.CheckFeeAllCorrectionDataByFirmSP(1, "SCHL", "");// check fee exist 
                        ViewBag.TotalCount = 0;//am.StoreAllData.Tables[0].Rows.Count;
                        if (dschk == null || dschk.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.IsFeeExists = "0";
                        }
                        else
                        { ViewBag.IsFeeExists = "1"; }

                        if (am.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.CorrectionFeeDate = am.StoreAllData.Tables[4].Rows[0]["CorrectionFeeDate"].ToString();
                            ViewBag.CorrectionFeeDateStatus = am.StoreAllData.Tables[4].Rows[0]["CorrectionFeeDateStatus"].ToString();
                        }

                        if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.TotalCountP = 0;
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                            return View(am);
                        }
                        else
                        {
                            ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
                            ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
                            ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
                            Session["ForFinalCorrectionLot"] = String.Join(",", am.StoreAllData.Tables[3].AsEnumerable().Select(x => x.Field<string>("CorrectionLot").ToString()).ToArray());
                            int tp = Convert.ToInt32(ViewBag.TotalCount);
                            int pn = tp / 30;
                            int cal = 30 * pn;
                            int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
                            if (res >= 1)
                                ViewBag.pn = pn + 1;
                            else
                                ViewBag.pn = pn;

                            return View(am);
                        }

                    }

                }
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                return View();


                //}
                //else
                //{
                //    return View();
                //}
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Lout", "Login");
                return View();
            }
        }
        [HttpPost]
        public ActionResult SchoolCorrectionUpdated(int? page, FormCollection frc, string cmd)
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                string SchlID = Convert.ToString(Session["SCHL"]);
                DataSet result = objCommon.schooltypes(SchlID); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {
                    ViewBag.NApprovedSchl = result.Tables[4].Rows[0]["Nth"].ToString();
                    ViewBag.EApprovedSchl = result.Tables[5].Rows[0]["Eth"].ToString();
                }
                //if (result.Tables[9].Rows.Count > 0)
                //{
                //    ViewBag.NLastDate = TempData["NLastDate"] = result.Tables[9].AsEnumerable().Where(x => x.Field<int>("Class") == 9).Select(x => x.Field<DateTime>("VerifyLastDateBySchl")).FirstOrDefault();
                //    ViewBag.ELastDate = TempData["ELastDate"] = result.Tables[9].AsEnumerable().Where(x => x.Field<int>("Class") == 11).Select(x => x.Field<DateTime>("VerifyLastDateBySchl")).FirstOrDefault();
                //}

                AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
                AdminModels am = new AdminModels();
                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Particular" }, new { ID = "2", Name = "Subject" }, new { ID = "4", Name = "Image" }, }, "ID", "Name", 1);
                ViewBag.CorrectionType = itemsch.ToList();

                var itemFee = new SelectList(new[] { new { ID = "1", Name = "With Fee" }, new { ID = "2", Name = "Widout Fee" }, }, "ID", "Name", 1);
                ViewBag.FeeType = itemFee.ToList();
                //------ Paging Srt
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                //---- Paging end                   
                #region View All Correction Pending Record                
                string Search = string.Empty;
                string CrType = "2";
                Search = "a.status is not null";

                if (frc["CorrectionLot"] != null)
                {
                    ViewBag.CorrectionLot = am.CorrectionLot = frc["CorrectionLot"].ToString();
                    Search += " and a.correctionlot ='" + am.CorrectionLot + "' ";

                }
                Search += " and a.SCHL  ='" + Session["SCHL"].ToString() + "'";
                am.StoreAllData = objDB.GetCorrectionDataFirmUpdated(Search, CrType, pageIndex);

                if (am.StoreAllData == null)
                {
                    ViewBag.TotalCountP = 0;
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View(am);
                }
                else
                {

                    DataSet dschk = objDB.CheckFeeAllCorrectionDataByFirmSP(1, "SCHL", "");// check fee exist 
                    ViewBag.TotalCount = 0;//am.StoreAllData.Tables[0].Rows.Count;
                    if (dschk == null || dschk.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.IsFeeExists = "0";
                    }
                    else
                    { ViewBag.IsFeeExists = "1"; }
                    //
                    if (am.StoreAllData.Tables[4].Rows.Count > 0)
                    {
                        ViewBag.CorrectionFeeDate = am.StoreAllData.Tables[4].Rows[0]["CorrectionFeeDate"].ToString();
                        ViewBag.CorrectionFeeDateStatus = am.StoreAllData.Tables[4].Rows[0]["CorrectionFeeDateStatus"].ToString();
                    }
                    ViewBag.CorrectionFinalSubmitDt = "";
                    if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.TotalCountP = 0;
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.CorrectionFinalSubmitDt = am.StoreAllData.Tables[0].Rows[0]["CorrectionFinalSubmitDt"].ToString();
                        ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
                        ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
                        Session["SchlForFinalCorrectionLot"] = String.Join(",", am.StoreAllData.Tables[3].AsEnumerable().Select(x => x.Field<string>("CorrectionLot").ToString()).ToArray());
                        int tp = Convert.ToInt32(ViewBag.TotalCount);
                        int pn = tp / 30;
                        int cal = 30 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                    }
                }

                #endregion View All Correction Pending Record

                return View(am);

            }
            catch (Exception ex)
            {
                return View();
            }
        }

        #endregion Rohit Firm School Correction Updated 

        #region  Final Submit Rohit
        public ActionResult SchoolCorrectionFinalSubmit(string id)
        {
            try
            {

                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string SchlID = Convert.ToString(Session["SCHL"]);
                DataSet result = objCommon.schooltypes(SchlID); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {
                    ViewBag.NApprovedSchl = result.Tables[4].Rows[0]["Nth"].ToString();
                    ViewBag.EApprovedSchl = result.Tables[5].Rows[0]["Eth"].ToString();
                }

                if (id == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                string UserName = Session["SCHL"].ToString();
                FormCollection frm = new FormCollection();
                var itemsch = new SelectList(new[]{new {ID="1",Name="Particular"},new {ID="2",Name="Subject"},new{ID="3",Name="Stream"},
            new{ID="4",Name="Image"},}, "ID", "Name", 1);
                ViewBag.CorrectionType = itemsch.ToList();
                AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
                AdminModels am = new AdminModels();
                if (ModelState.IsValid)
                {
                    if (id == "FinalSubmit")
                    {
                        DataSet dschk = objDB.CheckFeeAllCorrectionDataByFirmSP(1, "SCHL", "");// check fee exist 
                        ViewBag.TotalCount = 0;//am.StoreAllData.Tables[0].Rows.Count;
                        if (dschk == null || dschk.Tables[0].Rows.Count == 0)
                        {
                            // final submit here   

                            string FirmCorrectionLot = string.Empty;
                            string OutError = string.Empty;
                            DataSet ds1 = objDB.CorrectionDataFirmFinalSubmitSPRN("", UserName, out FirmCorrectionLot, out OutError);  // Final Submit Main Function                       
                            if (FirmCorrectionLot.Length > 2)
                            {
                                ViewBag.TotalCount = 1;
                                ViewData["Status"] = "1";
                                ViewData["Message"] = "";

                                am.StoreAllData = objDB.CheckFeeAllCorrectionDataByFirmSP(5, UserName, "");
                                if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
                                {
                                    ViewBag.TotalCount = 0;
                                    ViewData["Status"] = "10";
                                }
                                else
                                {

                                    ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
                                    ViewData["Status"] = "11";
                                }

                            }
                            else
                            {
                                ViewBag.TotalCount = 0;
                                ViewData["Status"] = "0";
                                ViewData["Message"] = OutError;
                            }
                            //  am.StoreAllData = objDB.GetAllCorrectionDataFirm(UserName);


                            return View(am);
                        }
                        else
                        {
                            ViewBag.commaCorrectionLot = String.Join(",", dschk.Tables[0].AsEnumerable().Select(x => x.Field<string>("CorrectionLot").ToString()).ToArray());
                            ViewBag.TotalCount = 1;
                            ViewData["Status"] = "5";

                        }
                    }
                    else if (id == "ViewAll")
                    {
                        am.StoreAllData = objDB.CheckFeeAllCorrectionDataByFirmSP(5, UserName, "");
                        if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.TotalCount = 0;
                            ViewData["Status"] = "10";
                        }
                        else
                        {

                            ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
                            ViewData["Status"] = "11";
                        }
                    }
                    return View(am);
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        #endregion Firm Final Submit Rohit



        #region CutList School
        public ActionResult CutList_Schl(string id)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();

            try
            {
                if (id == "")
                {
                    return RedirectToAction("SchoolCutList", "School");
                }

                ViewBag.CutlistClass = id;
                string SCHL = Session["SCHL"].ToString();
                string class1 = "";
                string Type1 = "";

                if (id.Contains("E"))
                {
                    ViewBag.Status = "0";
                }
                else
                {
                    ViewBag.Status = "1";
                }
                if (ViewBag.Status != null)
                {
                    ViewBag.Status = "1";
                    if (id == "M" || id == "ME")
                    {
                        class1 = "2";
                        Type1 = "REG";
                        DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);
                        List<SelectListItem> SecList = new List<SelectListItem>();
                        SecList.Add(new SelectListItem { Text = "No Centre", Value = "No Centre" });
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {

                            SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                        }
                        ViewBag.Ecent = SecList;
                    }
                    else if (id == "MO" || id == "MOE")
                    {
                        class1 = "2";
                        Type1 = "OPEN";
                        DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);
                        List<SelectListItem> SecList = new List<SelectListItem>();
                        SecList.Add(new SelectListItem { Text = "No Centre", Value = "No Centre" });
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {

                            SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                        }
                        ViewBag.Ecent = SecList;
                    }
                    else if (id == "S" || id == "SE")
                    {
                        class1 = "4";
                        Type1 = "REG";
                        DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);
                        List<SelectListItem> SecList = new List<SelectListItem>();
                        SecList.Add(new SelectListItem { Text = "No Centre", Value = "No Centre" });
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {

                            SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                        }
                        ViewBag.Ecent = SecList;
                    }
                    else if (id == "SO" || id == "SOE")
                    {
                        class1 = "4";
                        Type1 = "OPEN";
                        DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);
                        List<SelectListItem> SecList = new List<SelectListItem>();
                        SecList.Add(new SelectListItem { Text = "No Centre", Value = "No Centre" });
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {

                            SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                        }
                        ViewBag.Ecent = SecList;
                    }

                    MS.StoreAllData = null; //objDB.GetCutList_Schl(SCHL, class1, Type1, ViewBag.Status);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        return View(MS);
                    }
                }
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult CutList_Schl(FormCollection frm, string id)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();

            try
            {
                if (id == "")
                {
                    return RedirectToAction("SchoolCutList", "School");
                }

                ViewBag.CutlistClass = id;
                string SCHL = Session["SCHL"].ToString();
                string class1 = "";
                string Type1 = "";

                if (id.Contains("E"))
                {
                    ViewBag.Status = "0";
                }
                else
                {
                    ViewBag.Status = "1";
                }
                if (ViewBag.Status != null)
                {
                    if (id == "M" || id == "ME")
                    {
                        class1 = "2";
                        Type1 = "REG";
                        DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);
                        List<SelectListItem> SecList = new List<SelectListItem>();
                        SecList.Add(new SelectListItem { Text = "No Centre", Value = "No Centre" });
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {

                            SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                        }
                        ViewBag.Ecent = SecList;
                    }
                    else if (id == "MO" || id == "MOE")
                    {
                        class1 = "2";
                        Type1 = "OPEN";
                        DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);
                        List<SelectListItem> SecList = new List<SelectListItem>();
                        SecList.Add(new SelectListItem { Text = "No Centre", Value = "No Centre" });
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {

                            SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                        }
                        ViewBag.Ecent = SecList;
                    }
                    else if (id == "S" || id == "SE")
                    {
                        class1 = "4";
                        Type1 = "REG";
                        DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);
                        List<SelectListItem> SecList = new List<SelectListItem>();
                        SecList.Add(new SelectListItem { Text = "No Centre", Value = "No Centre" });
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {

                            SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                        }
                        ViewBag.Ecent = SecList;
                    }
                    else if (id == "SO" || id == "SOE")
                    {
                        class1 = "4";
                        Type1 = "OPEN";
                        DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);
                        List<SelectListItem> SecList = new List<SelectListItem>();
                        SecList.Add(new SelectListItem { Text = "No Centre", Value = "No Centre" });
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {

                            SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                        }
                        ViewBag.Ecent = SecList;
                    }
                    MS.ExamCent = frm["ExamCent"].ToString();

                    string Search = string.Empty;
                    if (frm["EXAM"] != "0")
                    {

                        if (id == "ME" || id == "MOE" || id == "SE" || id == "SOE")
                        {
                            if (Type1.ToUpper() == "REG")
                            {

                                if (!string.IsNullOrEmpty(MS.ExamCent))
                                {
                                    MS.EXAM = frm["EXAM"];
                                    if (frm["EXAM"] == "N")// for senior
                                    {
                                        Search = " isnull(NSQF_SUB,'')!='' and NSQF_SUB!='NO'";
                                    }
                                    else if ((id == "M") && frm["EXAM"] == "V")
                                    {
                                        Search = " Exam='" + frm["EXAM"] + "' ";
                                    }
                                    else
                                    {
                                        Search = " Exam='" + frm["EXAM"] + "'  and isnull(NSQF_SUB,'') in ('','NO') ";
                                    }
                                }
                                else
                                {

                                    if (frm["EXAM"] == "N")
                                    {
                                        Search = " isnull(NSQF_SUB,'')!='' and NSQF_SUB!='NO'  ";
                                        MS.EXAM = frm["EXAM"];
                                    }
                                    else if ((id == "M") && frm["EXAM"] == "V")
                                    {
                                        Search = " Exam='" + frm["EXAM"] + "' ";
                                    }
                                    else
                                    {
                                        Search = " Exam='" + frm["EXAM"] + "' and isnull(NSQF_SUB,'') in ('','NO')  ";
                                        MS.EXAM = frm["EXAM"];
                                    }
                                }
                            }
                            else
                            {
                                if (MS.ExamCent == "No Centre")
                                {
                                    Search = " Exam='" + frm["EXAM"] + "' ";
                                    MS.EXAM = frm["EXAM"];

                                }
                                else
                                {
                                    Search = " Exam='" + frm["EXAM"] + "' ";
                                    MS.EXAM = frm["EXAM"];
                                }

                            }
                        }
                        else
                        {
                            if (Type1.ToUpper() == "REG")
                            {

                                if (MS.ExamCent == "No Centre")
                                {
                                    MS.EXAM = frm["EXAM"];
                                    if (frm["EXAM"] == "N")// for senior
                                    {
                                        Search = " isnull(NSQF_SUB,'')!='' and NSQF_SUB!='NO'";
                                    }
                                    else if ((id == "M") && frm["EXAM"] == "V")
                                    {
                                        Search = " Exam='" + frm["EXAM"] + "' ";
                                    }
                                    else
                                    {
                                        Search = " Exam='" + frm["EXAM"] + "'  and isnull(NSQF_SUB,'') in ('','NO') ";
                                    }


                                    Search += "  and isnull(ER.Cent,'')='' ";
                                }
                                else
                                {

                                    if (frm["EXAM"] == "N")
                                    {
                                        Search = " isnull(NSQF_SUB,'')!='' and NSQF_SUB!='NO'  and ER.Cent= '" + frm["ExamCent"] + "' ";
                                        MS.EXAM = frm["EXAM"];
                                    }
                                    else if ((id == "M") && frm["EXAM"] == "V")
                                    {
                                        Search = " Exam='" + frm["EXAM"] + "' and ER.Cent= '" + frm["ExamCent"] + "' ";
                                    }
                                    else
                                    {
                                        Search = " Exam='" + frm["EXAM"] + "' and isnull(NSQF_SUB,'') in ('','NO')  and ER.Cent= '" + frm["ExamCent"] + "' ";
                                        MS.EXAM = frm["EXAM"];
                                    }
                                }
                            }
                            else
                            {
                                if (MS.ExamCent == "No Centre")
                                {
                                    Search = " Exam='" + frm["EXAM"] + "' ";
                                    MS.EXAM = frm["EXAM"];
                                    Search += "  and isnull(ER.Cent,'')='' ";
                                }
                                else
                                {
                                    //// Search = " Exam='" + frm["EXAM"] + "' ";
                                    Search = " Exam='" + frm["EXAM"] + "' and ER.Cent= '" + frm["ExamCent"] + "' ";
                                    MS.EXAM = frm["EXAM"];
                                }

                            }


                        }




                    }
                    if (frm["EXAM"] != "0" && frm["SelList"] != "0")
                    {
                        Search = Search + " and ";
                    }

                    if (frm["SelList"] != "0")
                    {
                        MS.SelList = frm["SelList"];
                        //ViewBag.SelectedItem = frm["SelList"];
                        int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());


                        if (frm["SearchString"] != "" && (id == "M" || id == "S"))
                        {
                            if (SelValueSch == 0)
                            { Search += "  std_id like '%' "; }
                            else if (SelValueSch == 1)
                            { Search += "  std_id='" + frm["SearchByString"].ToString() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += "  Candi_Name like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 3)
                            { Search += "  Father_Name  like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += "  Mother_Name like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 5)
                            { Search += "  registration_Num like '%" + frm["SearchByString"].ToString() + "%'"; }

                        }
                        else if (frm["SearchString"] != "" && (id == "MO" || id == "SO"))
                        {
                            if (SelValueSch == 0)
                            { Search += "  R.appno like '%' "; }
                            else if (SelValueSch == 1)
                            { Search += "  R.appno='" + frm["SearchByString"].ToString() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += "  R.Name like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 3)
                            { Search += "  R.FName  like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += "  R.MName like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 5)
                            { Search += "  R.regno like '%" + frm["SearchByString"].ToString() + "%'"; }

                        }


                    }


                    MS.StoreAllData = objDB.GetCutList_Schl(Search, SCHL, class1, Type1, ViewBag.Status);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        return View(MS);
                    }
                }
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }


        public ActionResult SchoolCutListAdmin()
        {
            if (Session["AdminType"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }
        public ActionResult CutList_Schl_Admin(string id)
        {
            FormCollection frm = new FormCollection();
            if (Session["AdminType"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();

            try
            {
                if (id == "" || id == null)
                {
                    return RedirectToAction("SchoolCutListAdmin", "School");
                }

                ViewBag.CutlistClass = id;
                MS.hdnFlag = id;
                string SCHL = frm["SCHL"];// "";// Session["SCHL"].ToString();
                string class1 = "";
                string Type1 = "";
                List<SelectListItem> SecList = new List<SelectListItem>();

                if (id.Contains("E"))
                {
                    ViewBag.Status = "0";
                }
                else
                {
                    ViewBag.Status = "1";
                }
                if (ViewBag.Status != null)
                {
                    if (id == "M" || id == "ME")
                    {
                        class1 = "2";
                        Type1 = "REG";

                        if (SCHL != null)
                        {
                            DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);

                            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                            {

                                SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                            }
                            ViewBag.Ecent = SecList;
                        }
                        else
                        {
                            ViewBag.Ecent = SecList;
                        }
                    }
                    else if (id == "MO" || id == "MOE")
                    {
                        class1 = "2";
                        Type1 = "OPEN";
                        if (SCHL != null)
                        {
                            DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);

                            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                            {

                                SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                            }
                            ViewBag.Ecent = SecList;
                        }
                        else
                        {
                            ViewBag.Ecent = SecList;
                        }
                    }
                    else if (id == "S" || id == "SE")
                    {
                        class1 = "4";
                        Type1 = "REG";
                        if (SCHL != null)
                        {
                            DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);

                            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                            {

                                SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                            }
                            ViewBag.Ecent = SecList;
                        }
                        else
                        {
                            ViewBag.Ecent = SecList;
                        }
                    }
                    else if (id == "SO" || id == "SOE")
                    {
                        class1 = "4";
                        Type1 = "OPEN";
                        if (SCHL != null)
                        {
                            DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);

                            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                            {

                                SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                            }
                            ViewBag.Ecent = SecList;
                        }
                        else
                        {
                            ViewBag.Ecent = SecList;
                        }
                    }
                    //----------- SET Binding-----
                    DataSet result = objDB.GetSCHLSET(class1, Session["Adminid"].ToString()); // passing Value to DBClass from model
                    ViewBag.MySelSet = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                    List<SelectListItem> items = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.MySelSet.Rows)
                    {
                        items.Add(new SelectListItem { Text = @dr["AdminSet"].ToString(), Value = @dr["AdminSet"].ToString() });
                    }
                    ViewBag.MySelSet = new SelectList(items, "Value", "Text");
                    //--------------


                    MS.StoreAllData = null; //objDB.GetCutList_Schl(SCHL, class1, Type1, ViewBag.Status);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        return View(MS);
                    }
                }
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult CutList_Schl_Admin(FormCollection frm, string id)
        {
            //if (Session["SCHL"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();

            try
            {
                if (id == "")
                {
                    return RedirectToAction("SchoolCutList", "School");
                }

                ViewBag.CutlistClass = id;
                ViewData["CutlistClass"] = id;
                MS.SelSet = frm["SelSet"];
                string SCHL = frm["SCHL"]; //Session["SCHL"].ToString();
                MS.SCHL = SCHL;
                MS.EXAM = frm["EXAM"];
                MS.SelList = frm["SelList"];
                MS.SearchByString = frm["SearchByString"];
                string class1 = "";
                string Type1 = "";
                string SetSearch = "";

                List<SelectListItem> SecList = new List<SelectListItem>();
                //DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);
                //List<SelectListItem> SecList = new List<SelectListItem>();
                //foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                //{

                //    SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                //}
                //ViewBag.Ecent = SecList;

                if (id.Contains("E"))
                {
                    ViewBag.Status = "0";
                }
                else
                {
                    ViewBag.Status = "1";
                }
                if (ViewBag.Status != null)
                {
                    if (id == "M" || id == "ME")
                    {
                        class1 = "2";
                        Type1 = "REG";
                        SetSearch = " and SM.[MSET] = '" + frm["SelSet"];

                        if (SCHL != null)
                        {
                            DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);

                            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                            {

                                SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                            }
                            ViewBag.Ecent = SecList;
                        }
                        else
                        {
                            ViewBag.Ecent = SecList;
                        }
                    }
                    else if (id == "MO" || id == "MOE")
                    {
                        class1 = "2";
                        Type1 = "OPEN";
                        SetSearch = " and SM.[MOSET] = '" + frm["SelSet"];

                        if (SCHL != null)
                        {
                            DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);

                            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                            {

                                SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                            }
                            ViewBag.Ecent = SecList;
                        }
                        else
                        {
                            ViewBag.Ecent = SecList;
                        }
                    }
                    else if (id == "S" || id == "SE")
                    {
                        class1 = "4";
                        Type1 = "REG";
                        SetSearch = " and SM.[SSET] = '" + frm["SelSet"];
                        if (SCHL != null)
                        {
                            DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);

                            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                            {

                                SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                            }
                            ViewBag.Ecent = SecList;
                        }
                        else
                        {
                            ViewBag.Ecent = SecList;
                        }
                    }
                    else if (id == "SO" || id == "SOE")
                    {
                        class1 = "4";
                        Type1 = "OPEN";
                        SetSearch = " and SM.[SOSET] = '" + frm["SelSet"];
                        if (SCHL != null)
                        {
                            DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);

                            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                            {

                                SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                            }
                            ViewBag.Ecent = SecList;
                        }
                        else
                        {
                            ViewBag.Ecent = SecList;
                        }
                    }

                    MS.ExamCent = frm["ExamCent"].ToString();
                    string Search = string.Empty;
                    //Search = " Exam='" + frm["EXAM"] + "' " + SetSearch + "' ";
                    Search = " Exam='" + frm["EXAM"] + "' and ER.Cent= '" + frm["ExamCent"] + "' ";

                    if (frm["SelList"] != "")
                    {
                        //ViewBag.SelectedItem = frm["SelList"];
                        int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());


                        if (frm["SearchString"] != "" && (id == "M" || id == "S"))
                        {
                            if (SelValueSch == 0)
                            { Search += " and std_id like '%' "; }
                            else if (SelValueSch == 1)
                            { Search += " and std_id='" + frm["SearchByString"].ToString() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += " and Candi_Name like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 3)
                            { Search += " and Father_Name  like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " and Mother_Name like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 5)
                            { Search += " and registration_Num like '%" + frm["SearchByString"].ToString() + "%'"; }

                        }
                        else if (frm["SearchString"] != "" && (id == "MO" || id == "SO"))
                        {
                            if (SelValueSch == 0)
                            { Search += " and R.appno like '%' "; }
                            else if (SelValueSch == 1)
                            { Search += " and R.appno='" + frm["SearchByString"].ToString() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += " and R.Name like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 3)
                            { Search += " and R.FName  like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " and R.MName like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 5)
                            { Search += " and R.regno like '%" + frm["SearchByString"].ToString() + "%'"; }

                        }


                    }

                    //----------- SET Binding-----
                    DataSet result = objDB.GetSCHLSET(class1, Session["Adminid"].ToString());  // passing Value to DBClass from model
                    ViewBag.MySelSet = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                    List<SelectListItem> items = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.MySelSet.Rows)
                    {
                        items.Add(new SelectListItem { Text = @dr["AdminSet"].ToString(), Value = @dr["AdminSet"].ToString() });
                    }
                    // ViewBag.MyDist = items;
                    ViewBag.MySelSet = new SelectList(items, "Value", "Text");
                    //--------------

                    MS.StoreAllData = objDB.GetCutList_Schl_Admin(Search, SCHL, class1, Type1, ViewBag.Status);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        return View(MS);
                    }
                }
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult CutList_Schl_AdminN(string id)
        {
            FormCollection frm = new FormCollection();
            if (Session["AdminType"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();

            try
            {
                if (id == "" || id == null)
                {
                    return RedirectToAction("SchoolCutListAdmin", "School");
                }

                ViewBag.CutlistClass = id;
                MS.hdnFlag = id;
                string SCHL = frm["SCHL"];// "";// Session["SCHL"].ToString();
                string class1 = "";
                string Type1 = "";
                List<SelectListItem> SecList = new List<SelectListItem>();

                if (id.Contains("E"))
                {
                    ViewBag.Status = "0";
                }
                else
                {
                    ViewBag.Status = "1";
                }
                if (ViewBag.Status != null)
                {
                    if (id == "M" || id == "ME")
                    {
                        class1 = "2";
                        Type1 = "REG";

                        if (SCHL != null)
                        {
                            DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);

                            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                            {

                                SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                            }
                            ViewBag.Ecent = SecList;
                        }
                        else
                        {
                            ViewBag.Ecent = SecList;
                        }
                    }
                    else if (id == "MO" || id == "MOE")
                    {
                        class1 = "2";
                        Type1 = "OPEN";
                        if (SCHL != null)
                        {
                            DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);

                            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                            {

                                SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                            }
                            ViewBag.Ecent = SecList;
                        }
                        else
                        {
                            ViewBag.Ecent = SecList;
                        }
                    }
                    else if (id == "S" || id == "SE")
                    {
                        class1 = "4";
                        Type1 = "REG";
                        if (SCHL != null)
                        {
                            DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);

                            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                            {

                                SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                            }
                            ViewBag.Ecent = SecList;
                        }
                        else
                        {
                            ViewBag.Ecent = SecList;
                        }
                    }
                    else if (id == "SO" || id == "SOE")
                    {
                        class1 = "4";
                        Type1 = "OPEN";
                        if (SCHL != null)
                        {
                            DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);

                            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                            {

                                SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                            }
                            ViewBag.Ecent = SecList;
                        }
                        else
                        {
                            ViewBag.Ecent = SecList;
                        }
                    }
                    //----------- SET Binding-----
                    DataSet result = objDB.GetSCHLSET(class1, Session["Adminid"].ToString()); // passing Value to DBClass from model
                    ViewBag.MySelSet = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                    List<SelectListItem> items = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.MySelSet.Rows)
                    {
                        items.Add(new SelectListItem { Text = @dr["AdminSet"].ToString(), Value = @dr["AdminSet"].ToString() });
                    }
                    ViewBag.MySelSet = new SelectList(items, "Value", "Text");
                    //--------------


                    MS.StoreAllData = null; //objDB.GetCutList_Schl(SCHL, class1, Type1, ViewBag.Status);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        return View(MS);
                    }
                }
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        public ActionResult CutList_Schl_AdminN(FormCollection frm, string id)
        {
            //if (Session["SCHL"] == null)
            //{
            //    return RedirectToAction("Index", "Login");
            //}
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();

            try
            {
                if (id == "")
                {
                    return RedirectToAction("SchoolCutList", "School");
                }

                ViewBag.CutlistClass = id;
                ViewData["CutlistClass"] = id;
                MS.SelSet = frm["SelSet"];
                string SCHL = frm["SCHL"]; //Session["SCHL"].ToString();
                MS.SCHL = SCHL;
                MS.EXAM = frm["EXAM"];
                MS.SelList = frm["SelList"];
                MS.SearchByString = frm["SearchByString"];
                string class1 = "";
                string Type1 = "";
                string SetSearch = "";

                List<SelectListItem> SecList = new List<SelectListItem>();


                if (id.Contains("E"))
                {
                    ViewBag.Status = "0";
                }
                else
                {
                    ViewBag.Status = "1";
                }

                if (SCHL != null)
                {
                    DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);

                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {

                        SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                    }
                    ViewBag.Ecent = SecList;
                }
                else
                {
                    ViewBag.Ecent = SecList;
                }

                string Search = string.Empty;
                Search = " Exam like '%%'";
                //Search = " Exam='" + frm["EXAM"] + "' and ER.Cent= '" + frm["ExamCent"] + "' ";
                if (ViewBag.Status != null)
                {
                    if (SCHL != "")
                    {
                        Search += " and R.SCHL ='" + SCHL + "'";
                    }
                    if (frm["EXAM"] != "" && frm["EXAM"] != "0")
                    {
                        Search += " and Exam='" + frm["EXAM"] + "'";
                    }
                    if (frm["ExamCent"] != "" && frm["ExamCent"] != "0")
                    {
                        Search += " and ER.Cent='" + frm["ExamCent"] + "'";
                    }


                    if (id == "M" || id == "ME")
                    { class1 = "2"; Type1 = "REG"; }
                    else if (id == "MO" || id == "MOE")
                    { class1 = "2"; Type1 = "OPEN"; }
                    else if (id == "S" || id == "SE")
                    { class1 = "4"; Type1 = "REG"; }
                    else if (id == "SO" || id == "SOE")
                    { class1 = "4"; Type1 = "OPEN"; }


                    if ((id == "M" || id == "ME") && frm["SelSet"] != "")
                    {
                        Search += " and SM.[MSET] = '" + frm["SelSet"] + "'";
                    }
                    else if ((id == "MO" || id == "MOE") && frm["SelSet"] != "")
                    {
                        Search += " and SM.[MOSET] = '" + frm["SelSet"] + "'";
                    }
                    else if ((id == "S" || id == "SE") && frm["SelSet"] != "")
                    {
                        Search += " and SM.[SSET] = '" + frm["SelSet"] + "'";
                    }
                    else if ((id == "SO" || id == "SOE") && frm["SelSet"] != "")
                    {
                        Search += " and SM.[SOSET] = '" + frm["SelSet"] + "'";
                    }
                    MS.ExamCent = frm["ExamCent"].ToString();

                    //Search = " Exam='" + frm["EXAM"] + "' " + SetSearch + "' ";
                    //Search = " Exam='" + frm["EXAM"] + "' and ER.Cent= '" + frm["ExamCent"] + "' ";

                    if (frm["SelList"] != "")
                    {
                        //ViewBag.SelectedItem = frm["SelList"];
                        int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());

                        if (frm["SearchString"] != "" && (id == "M" || id == "S" || id == "SE"))
                        {
                            if (SelValueSch == 0)
                            { Search += " and std_id like '%' "; }
                            else if (SelValueSch == 1)
                            { Search += " and std_id='" + frm["SearchByString"].ToString() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += " and Candi_Name like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 3)
                            { Search += " and Father_Name  like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " and Mother_Name like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 5)
                            { Search += " and registration_Num like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 6)
                            { Search += " and ER.ROLL ='" + frm["SearchByString"].ToString() + "'"; }

                        }
                        else if (frm["SearchString"] != "" && (id == "MO" || id == "SO" || id == "SOE"))
                        {
                            if (SelValueSch == 0)
                            { Search += " and R.appno like '%' "; }
                            else if (SelValueSch == 1)
                            { Search += " and R.appno='" + frm["SearchByString"].ToString() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += " and R.Name like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 3)
                            { Search += " and R.FName  like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " and R.MName like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 5)
                            { Search += " and R.regno like '%" + frm["SearchByString"].ToString() + "%'"; }
                            else if (SelValueSch == 6)
                            { Search += " and ER.ROLL ='" + frm["SearchByString"].ToString() + "'"; }
                        }


                    }

                    //----------- SET Binding-----
                    DataSet result = objDB.GetSCHLSET(class1, Session["Adminid"].ToString());  // passing Value to DBClass from model
                    ViewBag.MySelSet = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                    List<SelectListItem> items = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.MySelSet.Rows)
                    {
                        items.Add(new SelectListItem { Text = @dr["AdminSet"].ToString(), Value = @dr["AdminSet"].ToString() });
                    }
                    // ViewBag.MyDist = items;
                    ViewBag.MySelSet = new SelectList(items, "Value", "Text");
                    //--------------

                    MS.StoreAllData = objDB.CutList_Schl_AdminN(Search, SCHL, class1, Type1, ViewBag.Status);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        return View(MS);
                    }
                }
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion CutList School

        #region pvt cutlist
        public ActionResult pvtcutlist_admin()
        {
            if (Session["AdminType"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }
        public ActionResult Pvtcutlist(string id)
        {
            if (Session["AdminType"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            ViewBag.MyDist = objCommon.GetDistE();
            try
            {
                if (id == "" || id == null)
                {
                    return RedirectToAction("SchoolCutListAdmin", "School");
                }

                ViewBag.CutlistClass = id;
                string SCHL = "";// Session["SCHL"].ToString();
                string class1 = "";
                string Type1 = "";
                if (id.Contains("E"))
                {
                    ViewBag.Status = "0";
                }
                else
                {
                    ViewBag.Status = "1";
                }
                if (ViewBag.Status != null)
                {
                    if (id == "M" || id == "ME")
                    {
                        class1 = "10";
                        Type1 = "REG";
                    }
                    else if (id == "S" || id == "SE")
                    {
                        class1 = "12";
                        Type1 = "REG";
                    }

                    //----------- SET Binding-----
                    DataSet result = objDB.GetSCHLSET(class1, Session["Adminid"].ToString()); // passing Value to DBClass from model
                    ViewBag.MySelSet = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                    List<SelectListItem> items = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.MySelSet.Rows)
                    {
                        items.Add(new SelectListItem { Text = @dr["AdminSet"].ToString(), Value = @dr["AdminSet"].ToString() });
                    }
                    ViewBag.MySelSet = new SelectList(items, "Value", "Text");
                    //--------------



                    ViewBag.MyDISTRESULT = objCommon.GetDistE();
                    //List<SelectListItem> Distitems = new List<SelectListItem>();
                    //foreach (System.Data.DataRow dr in ViewBag.MyDISTRESULT.Rows)
                    //{
                    //    Distitems.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    //}
                    ViewBag.MyDist = new SelectList(ViewBag.MyDISTRESULT, "Value", "Text");


                    MS.StoreAllData = null; //objDB.GetCutList_Schl(SCHL, class1, Type1, ViewBag.Status);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        return View(MS);
                    }
                }
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public ActionResult Pvtcutlist(FormCollection frm, string id)
        {
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();

            try
            {
                if (id == "" || id == null)
                {
                    return RedirectToAction("SchoolCutListAdmin", "School");
                }
                ViewBag.MyDist = objCommon.GetDistE();
                ViewBag.CutlistClass = id;
                MS.SelSet = frm["SelSet"];
                string SCHL = frm["SCHL"]; //Session["SCHL"].ToString();
                MS.SCHL = SCHL;
                MS.EXAM = frm["EXAM"];
                MS.SelList = frm["SelList"];
                MS.SearchByString = frm["SearchByString"];
                string class1 = "";
                string Type1 = "";
                string SetSearch = "";
                if (id.Contains("E"))
                {
                    ViewBag.Status = "0";
                }
                else
                {
                    ViewBag.Status = "1";
                }
                if (ViewBag.Status != null)
                {
                    if (id == "M" || id == "ME")
                    {
                        class1 = "10";
                        Type1 = "PVT";
                        SetSearch = " and SM.[MSET] = '" + frm["SelSet"];
                    }
                    else
                    {
                        class1 = "12";
                        Type1 = "PVT";
                    }


                    string Search = string.Empty;
                    string Myset = string.Empty;
                    string Mycategory = string.Empty;
                    string MyExam_Type = string.Empty;
                    string SelDistSearch = string.Empty;
                    //Search = " Exam='" + frm["EXAM"] + "' " + SetSearch + "' ";
                    if (frm["SelSet"] != "")
                    {
                        if (class1 == "10")
                        {
                            Myset = frm["SelSet"];
                        }
                        else if (class1 == "12")
                        {
                            Myset = frm["SelSet"];
                        }
                    }
                    if (frm["category"] != "")
                    {
                        MS.category = frm["category"];
                    }
                    if (frm["Exam_Type"] != "")
                    {
                        MS.Exam_Type = frm["Exam_Type"];
                    }
                    if (frm["dist"] != "")
                    {
                        SelDistSearch = frm["dist"];
                    }

                    if (frm["SelList"] != "")
                    {
                        //int SelValueSch = 0;// Convert.ToInt32(frm["SelList"].ToString());
                        //if (frm["SearchString"] != "" && (id == "PVT"))
                        int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());
                        if ((id.ToUpper() == "M") || (id.ToUpper() == "S") || (id.ToUpper() == "PVT"))
                        {
                            if (SelValueSch == 0)
                            {
                                Search += " appno like '%' and R.cat='" + MS.category + "' and R.rp='" + MS.Exam_Type + "'";
                            }
                            else if (SelValueSch == 1)
                            { Search += " R.refno='" + frm["SearchByString"].ToString().Trim() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += " R.Name like '%" + frm["SearchByString"].ToString().Trim() + "%'"; }
                            else if (SelValueSch == 3)
                            { Search += " R.FName  like '%" + frm["SearchByString"].ToString().Trim() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " R.MName like '%" + frm["SearchByString"].ToString().Trim() + "%'"; }
                            else if (SelValueSch == 5)
                            { Search += " R.regno like '%" + frm["SearchByString"].ToString().Trim() + "'"; }
                            else if (SelValueSch == 6)
                            { Search += " R.SCHL='" + frm["SearchByString"].ToString().Trim() + "'"; }
                            else if (SelValueSch == 7)
                            { Search += " R.ROLL='" + frm["SearchByString"].ToString().Trim() + "'"; }

                        }


                    }

                    //----------- SET Binding-----
                    DataSet result = objDB.GetSCHLSET(class1, Session["Adminid"].ToString());  // passing Value to DBClass from model
                    ViewBag.MySelSet = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                    List<SelectListItem> items = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.MySelSet.Rows)
                    {
                        items.Add(new SelectListItem { Text = @dr["AdminSet"].ToString(), Value = @dr["AdminSet"].ToString() });
                    }
                    // ViewBag.MyDist = items;
                    ViewBag.MySelSet = new SelectList(items, "Value", "Text");
                    //--------------
                    MS.dist = SelDistSearch;

                    MS.StoreAllData = objDB.Pvtcutlist(Search, SCHL, class1, Type1, ViewBag.Status, Myset, SelDistSearch);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        return View(MS);
                    }
                }
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion pvt cutlist

        public JsonResult GetCentreList(string SCHL, string cls)
        {
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            string class1 = null;
            if (cls == "M" || cls == "ME" || cls == "MO" || cls == "MOE")
            {
                class1 = "2";
            }
            else if (cls == "S" || cls == "SE" || cls == "SO" || cls == "SOE")
            {
                class1 = "4";
            }

            DataSet Dresult = objDB.GetCentreSchl(SCHL, class1);
            List<SelectListItem> SecList = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
            {
                SecList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
            }
            ViewBag.Ecent = SecList;
            return Json(SecList);
        }

        #region SupervisoryStaffList
        //public ActionResult SupervisoryStaffList(DEOModel DEO)
        //{
        //    string DeoUser = null;
        //    string district = null;
        //    string Catg = null;
        //    ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");

        //    try
        //    {
        //        DataSet result = OBJDB.GetDeoDISTSCHL(Session["SCHL"].ToString()); // passing Value to DBClass from model            
        //        List<SelectListItem> DList = new List<SelectListItem>();
        //        foreach (System.Data.DataRow dr in result.Tables[0].Rows)
        //        {
        //            DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
        //        }
        //        ViewBag.Dist = DList;

        //    }
        //    catch (Exception ex)
        //    {
        //        oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
        //        return View();

        //    }
        //    return View(DEO);
        //}
        public ActionResult SupervisoryStaffList(SchoolModels SM)
        {
            string DeoUser = null;
            //string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");

            DataSet district = objDB.GetDeoDISTSCHL(Session["SCHL"].ToString());
            SM.StoreAllData = objDB.SupervisoryStaffList(district.Tables[0].Rows[0][0].ToString());
            if (SM.StoreAllData == null || SM.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "Staff Dose not Exist";
                ViewBag.TotalCount = 0;
            }
            else
            {
                ViewBag.TotalCount = SM.StoreAllData.Tables[0].Rows.Count;
            }

            //try
            //{
            //    DataSet result = OBJDB.GetDeoDISTSCHL(Session["SCHL"].ToString()); // passing Value to DBClass from model            
            //    List<SelectListItem> DList = new List<SelectListItem>();
            //    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
            //    {
            //        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
            //    }
            //    ViewBag.Dist = DList;

            //}
            //catch (Exception ex)
            //{
            //    oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
            //    return View();

            //}
            return View(SM);
        }

        [HttpPost]
        public ActionResult SupervisoryStaffList(DEOModel DEO, FormCollection frm)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                district = frm["SelDist"];
                DEO.StoreAllData = OBJDB.SupervisoryStaffList(district);
                if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Staff Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                }

                DataSet result = OBJDB.GetDeoDIST(district); // passing Value to DBClass from model            
                List<SelectListItem> DList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                {
                    DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }

                ViewBag.Dist = DList;
                return View(DEO);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View(DEO);

            }

        }
        #endregion SupervisoryStaffListv



        #region PracticalChart

        public ActionResult PracticalChart(string id)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SchoolModels MS = new SchoolModels();
            string SchlID = Convert.ToString(Session["SCHL"]);

            DataSet result = objCommon.schooltypes(SchlID); // passing Value to DBClass from model
            if (result == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (result.Tables[1].Rows.Count > 0)
            {
                ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
                ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
            }
            ViewBag.Cls = "";
            ViewBag.cid = id;
            string class1 = "";
            if (id == null)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ViewBag.Status = "1";
                if (id.ToUpper() == "M")
                {
                    ViewBag.Cls = class1 = "2";
                }
                else if (id.ToUpper() == "S")
                {
                    ViewBag.Cls = class1 = "3";
                }

                MS.StoreAllData = objDB.GetpracticalMarks_Schl(SchlID, class1);
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                }
            }


            return View(MS);

            //return View();
        }

        public ActionResult PracticalChartLink()
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SchoolModels MS = new SchoolModels();
            string SchlID = Convert.ToString(Session["SCHL"]);

            DataSet result = objCommon.schooltypes(SchlID); // passing Value to DBClass from model
            if (result == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (result.Tables[1].Rows.Count > 0)
            {
                ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
                ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
            }

            return View();
        }

        #endregion PracticalChart

        #region Practical Exam Marks

        public ActionResult PracticalAgree(string id)
        {
            try
            {

                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                Session["PracticalAgree"] = "1";
                @ViewBag.Dpdf = "../../PDF/PracticalAgree.pdf";
                @ViewBag.Showpdf = "../../PDF/PracticalAgree.pdf";
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult CheckPracticalAgree(FormCollection frm)
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                string s = frm["Agree"].ToString();
                if (Session["PracticalAgree"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (s == "Agree")
                    {
                        return RedirectToAction("PracticalExamMarks", "School");

                    }
                    else
                    {
                        return RedirectToAction("PracticalChartLink", "School");
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }
        }



        // [OutputCache(Duration = 180, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        [OutputCache(Duration = 180, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult PracticalExamMarks(string id, int? page)
        {

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            TempData["PracExamViewListSearch"] = null;
            TempData["PracExamEnterMarksSearch"] = null;
            TempData["ViewPracExamFinalSubmitSearch"] = null;

            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                ViewBag.cid = "";
                string SCHL = Session["SCHL"].ToString();
                ViewBag.schlCode = SCHL;

                //var itemRP = new SelectList(new[] { new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                //  var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" }, }, "ID", "Name", 1);
                var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" }, new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                ViewBag.MyRP = itemRP.ToList();

                //Subject Code, Subject Name,Pending
                var itemAction = new SelectList(new[] { new { ID = "1", Name = "Subject Code" }, new { ID = "2", Name = " Subject Name" }, new { ID = "3", Name = "Pending" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";


                var itemClass = new SelectList(new[] { new { ID = "4", Name = "SrSec" }, new { ID = "2", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.MyClass = itemClass.ToList();
                ViewBag.SelectedClass = "0";



                if (SCHL != null)
                {
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                    string Search = "Reg.schl like '%%'";
                    string SelectedAction = "0";
                    if (TempData["PracticalExamMarksSearch"] != null)
                    {
                        Search = TempData["PracticalExamMarksSearch"].ToString();
                        ViewBag.SelectedClass = TempData["SelClassPE"];
                        ViewBag.SelectedRP = TempData["SelRPPE"].ToString();
                        SelectedAction = TempData["SelAction"].ToString();
                        ViewBag.SelectedAction = TempData["SelActionValue"];

                        TempData["SelClassPE"] = ViewBag.SelectedClass;
                        TempData["SelRPPE"] = ViewBag.SelectedRP;
                        TempData["SelAction"] = SelectedAction;
                        TempData["SelActionValue"] = ViewBag.SelectedAction;
                        TempData["PracticalExamMarksSearch"] = Search;


                        string class1 = ViewBag.SelectedClass; string rp = ViewBag.SelectedRP;
                        string cent = SCHL;

                        MS.StoreAllData = objDB.PracExamEnterMarks(class1, rp, cent, Search, Convert.ToInt32(SelectedAction), pageIndex);
                        if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount1 = ViewBag.TotalCount = 0;
                            //if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                            //{
                            //    ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsMarksFilled"]);
                            //}
                            return View();
                        }
                        else
                        {
                            ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                            int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                            //if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                            //{
                            //    ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsMarksFilled"]);
                            //}
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
                    else
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
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
        [OutputCache(Duration = 180, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult PracticalExamMarks(FormCollection frm, string id, int? page)
        {

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                ViewBag.cid = "";

                string SCHL = Session["SCHL"].ToString();
                ViewBag.schlCode = SCHL;

                var itemClass = new SelectList(new[] { new { ID = "4", Name = "SrSec" }, new { ID = "2", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.MyClass = itemClass.ToList();
                ViewBag.SelectedClass = "0";

                // var itemRP = new SelectList(new[] { new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                // var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" }, }, "ID", "Name", 1);
                var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" }, new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                ViewBag.MyRP = itemRP.ToList();

                //Subject Code, Subject Name,Pending
                var itemAction = new SelectList(new[] { new { ID = "1", Name = "Subject Code" }, new { ID = "2", Name = " Subject Name" }, new { ID = "3", Name = "Pending" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";
                //------------------------
                string SelClass = "";
                string RP = "";
                string Cent = "";


                if (SCHL != null)
                {
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                    string Search = "Reg.schl like '%%'";
                    //  Search = " a.SCHL='" + SCHL + "'";
                    Search += " and sb.pcent = '" + SCHL + "'";
                    Cent = SCHL;
                    if (frm["SelClass"] != "")
                    {
                        //SelClass = frm["SelClass"].ToString() == "1" ? "4" : "2";
                        SelClass = frm["SelClass"].ToString();
                        ViewBag.SelectedClass = frm["SelClass"].ToString();
                        Search += " and Reg.class='" + SelClass + "'";
                    }

                    if (frm["SelRP"] != "" && frm["SelRP"] != null)
                    {
                        //  RP = frm["SelRP"].ToString() == "1" ? "R" : frm["SelRP"].ToString() == "2" ?  "O" :  "P";
                        RP = frm["SelRP"].ToString();
                        ViewBag.SelectedRP = frm["SelRP"].ToString();
                        Search += " and Reg.rp='" + RP + "'";
                    }
                    int SelAction = 0;
                    if (frm["SelAction"] != "")
                    {
                        ViewBag.SelectedAction = frm["SelAction"];
                        SelAction = Convert.ToInt32(frm["SelAction"].ToString());
                        if (frm["SelAction"] != "" && frm["SearchString"].ToString() != "")
                        {
                            if (SelAction == 1)
                            { Search += " and sm.Sub='" + frm["SearchString"].ToString() + "'"; }
                            if (SelAction == 2)
                            { Search += " and sm.subnm='%" + frm["SearchString"].ToString() + "%'"; }
                            if (SelAction == 3)
                            { Search += " and  isnull(fplot,'')='' "; }
                        }
                    }

                    TempData["SelClassPE"] = SelClass;
                    TempData["SelRPPE"] = RP;
                    TempData["SelAction"] = SelAction;
                    TempData["SelActionValue"] = frm["SelAction"];
                    TempData["PracticalExamMarksSearch"] = Search;

                    MS.StoreAllData = objDB.PracExamEnterMarks(SelClass, RP, Cent, Search, Convert.ToInt32(SelAction), pageIndex);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        Session["PracticalExamMarks"] = null;
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount1 = ViewBag.TotalCount = 0;
                        //if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        //{
                        //    ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsMarksFilled"]);
                        //}
                        return View();
                    }
                    else
                    {
                        Session["PracticalExamMarks"] = "1";
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        //if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        //{
                        //    ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsMarksFilled"]);
                        //}
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
        #endregion Practical Exam Marks

        #region PracExamViewList
        [OutputCache(Duration = 180, VaryByParam = "id", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult PracExamViewList(string id, int? page)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["PracticalExamMarks"] == null)
            {
                return RedirectToAction("PracticalExamMarks", "School");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            TempData.Keep();
            if (TempData["PracticalExamMarksSearch"] != null)
            { }

            try
            {
                string SelClass = "";
                string RP = "";
                string Cent = "";
                string SubCode = "";
                string SelectedStatus = "0";
                if (id != null)
                {
                    string[] split = id.Split('-');
                    //var parm = Class + "-" + ExamType + "-" + Centre + "-" + SUB;
                    ViewBag.SelClass = SelClass = split[0].ToString();
                    ViewBag.SelRP = RP = split[1].ToString();
                    ViewBag.SelCent = Cent = split[2].ToString();
                    SubCode = split[3].ToString();
                    TempData["cid"] = ViewBag.cid = id.ToString();
                }
                string SCHL = Session["SCHL"].ToString();
                ViewBag.schlCode = SCHL;
                var itemStatus = new SelectList(new[] { new { ID = "1", Name = "Pending" }, new { ID = "2", Name = "Submitted" }, }, "ID", "Name", 1);
                ViewBag.MyStatus = itemStatus.ToList();
                ViewBag.SelectedStatus = SelectedStatus = "0";
                //          
                // var itemRP = new SelectList(new[] { new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" }, new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                ViewBag.MyRP = itemRP.ToList();

                //var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" } }, "ID", "Name", 1);
                //ViewBag.MyRP = itemRP.ToList();

                //Subject Code, Subject Name,Pending
                var itemAction = new SelectList(new[] { new { ID = "1", Name = "Stdid/Refno" }, new { ID = "2", Name = "Roll No" },
                    new { ID = "3", Name = "RegNo" }, new { ID = "3", Name = "Name" },  }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";
                //------------------------

                if (SCHL != null)
                {
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                    string Search = "Reg.schl like '%%'";
                    string SelectedAction = "0";
                    if (TempData["PracExamViewListSearch"] != null)
                    {
                        Search = TempData["PracExamViewListSearch"].ToString();
                        ViewBag.SelectedStatus = TempData["SelStatus"];
                        SelectedAction = TempData["SelAction"].ToString();
                        ViewBag.SelectedAction = TempData["SelActionValue"];

                        TempData["SelStatus"] = ViewBag.SelectedStatus;
                        TempData["SelRP"] = ViewBag.SelectedRP;
                        TempData["SelAction"] = SelectedAction;
                        TempData["SelActionValue"] = ViewBag.SelectedAction;
                        TempData["PracExamViewListSearch"] = Search;
                    }

                    MS.StoreAllData = objDB.PracExamViewList(SelClass, RP, Cent, Search, Convert.ToInt32(SelectedStatus), pageIndex, SubCode);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.LastDateofSub = null;
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount1 = ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {

                        ViewBag.LastDateofSub = MS.StoreAllData.Tables[0].Rows[0]["LastDate"];

                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;

                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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
                    if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                    {
                        ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcent"].ToString());
                        ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcentNM"].ToString());

                        ViewBag.SubCode = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["sub"].ToString());
                        ViewBag.SubName = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["subnm"].ToString());

                        ViewBag.PrMin = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["prmin"].ToString());
                        ViewBag.PrMax = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["prmax"].ToString());
                    }

                    return View(MS);
                }
            }
            catch (Exception ex)
            {
                //return RedirectToAction("Index", "Login");
            }
            return View(MS);
        }

        [HttpPost]
        [OutputCache(Duration = 180, VaryByParam = "id", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult PracExamViewList(FormCollection frm, string id, int? page)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {

                string SelClass = "";
                string RP = "";
                string Cent = "";
                string SubCode = "";
                string SelectedStatus = "";
                if (frm["cid"] != null)
                {
                    id = frm["cid"].ToString();
                    string[] split = id.Split('-');
                    //var parm = Class + "-" + ExamType + "-" + Centre + "-" + SUB;
                    ViewBag.SelClass = SelClass = split[0].ToString();
                    ViewBag.SelRP = RP = split[1].ToString();
                    ViewBag.SelCent = Cent = split[2].ToString();
                    SubCode = split[3].ToString();
                    TempData["cid"] = ViewBag.cid = id.ToString();
                }
                string SCHL = Session["SCHL"].ToString();
                ViewBag.schlCode = SCHL;
                var itemStatus = new SelectList(new[] { new { ID = "1", Name = "Pending" }, new { ID = "2", Name = "Submitted" }, }, "ID", "Name", 1);
                ViewBag.MyStatus = itemStatus.ToList();
                ViewBag.SelectedStatus = SelectedStatus = "0";
                //
                // var itemRP = new SelectList(new[] { new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" }, new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                ViewBag.MyRP = itemRP.ToList();

                //var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" }, }, "ID", "Name", 1);
                //ViewBag.MyRP = itemRP.ToList();

                //Subject Code, Subject Name,Pending
                var itemAction = new SelectList(new[] { new { ID = "1", Name = "Stdid/Refno" }, new { ID = "2", Name = "Roll No" },
                    new { ID = "3", Name = "RegNo" }, new { ID = "4", Name = "Name" },  }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";
                //------------------------



                if (SCHL != null)
                {
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                    string Search = "Reg.schl like '%%'";
                    //  Search = " a.SCHL='" + SCHL + "'";
                    Search += " and sb.pcent = '" + SCHL + "'";
                    if (frm["SelStatus"] != "" && frm["SelStatus"] != null)
                    {
                        SelectedStatus = frm["SelStatus"].ToString();
                        ViewBag.SelectedStatus = frm["SelStatus"].ToString();
                        if (SelectedStatus == "1")
                        { Search += " and isnull(OBTMARKSP,'')='' "; }
                        else if (SelectedStatus == "2")
                        { Search += " and OBTMARKSP is not null and fplot is not null "; }

                    }

                    if (frm["SelRP"] != "" && frm["SelRP"] != null)
                    {
                        // RP = frm["SelRP"].ToString() == "1" ? "R" : frm["SelRP"].ToString() == "2" ? "O" : "P";
                        ViewBag.SelectedRP = RP = frm["SelRP"].ToString();
                        Search += " and Reg.rp='" + RP + "'";
                    }
                    int SelAction = 0;
                    if (frm["SelAction"] != "")
                    {
                        ViewBag.SelectedAction = frm["SelAction"];
                        SelAction = Convert.ToInt32(frm["SelAction"].ToString());
                        if (frm["SelAction"] != "" && frm["SearchString"].ToString() != "")
                        {
                            if (SelAction == 1)
                            { Search += " and reg.std_id='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelAction == 2)
                            { Search += " and reg.roll='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelAction == 3)
                            { Search += " and reg.regno='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelAction == 4)
                            { Search += " and reg.name like '%" + frm["SearchString"].ToString() + "%'"; }
                        }
                    }

                    TempData["SelStatus"] = frm["SelStatus"];
                    TempData["SelRP"] = frm["SelRP"];
                    TempData["SelAction"] = SelAction;
                    TempData["SelActionValue"] = frm["SelAction"];
                    TempData["PracExamViewListSearch"] = Search;

                    MS.StoreAllData = objDB.PracExamViewList(SelClass, RP, Cent, Search, Convert.ToInt32(SelectedStatus), pageIndex, SubCode);

                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.LastDateofSub = null;
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount1 = ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.LastDateofSub = MS.StoreAllData.Tables[0].Rows[0]["LastDate"];
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;

                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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

                    if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                    {
                        ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcent"].ToString());
                        ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcentNM"].ToString());

                        ViewBag.SubCode = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["sub"].ToString());
                        ViewBag.SubName = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["subnm"].ToString());

                        ViewBag.PrMin = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["prmin"].ToString());
                        ViewBag.PrMax = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["prmax"].ToString());
                    }

                    return View(MS);
                }
            }
            catch (Exception ex)
            {
                //return RedirectToAction("Index", "Login");
            }
            return View(MS);
        }

        #endregion PracExamViewList


        #region PracExamEnterMarks
        [OutputCache(Duration = 180, VaryByParam = "id", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult PracExamEnterMarks(string id, int? page)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["PracticalExamMarks"] == null)
            {
                return RedirectToAction("PracticalExamMarks", "School");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string SelClass = "";
                string RP = "";
                string Cent = "";
                string SubCode = "";
                string SelectedStatus = "0";
                if (id != null)
                {
                    string[] split = id.Split('-');
                    //var parm = Class + "-" + ExamType + "-" + Centre + "-" + SUB;
                    ViewBag.Class = ViewBag.SelClass = SelClass = split[0].ToString();
                    ViewBag.RP = ViewBag.SelRP = RP = split[1].ToString();
                    ViewBag.SelCent = Cent = split[2].ToString();
                    SubCode = split[3].ToString();
                    TempData["cid"] = ViewBag.cid = id.ToString();
                }
                string SCHL = Session["SCHL"].ToString();
                ViewBag.schlCode = SCHL;
                var itemStatus = new SelectList(new[] { new { ID = "1", Name = "Pending" }, new { ID = "2", Name = "Submitted" }, }, "ID", "Name", 1);
                ViewBag.MyStatus = itemStatus.ToList();
                ViewBag.SelectedStatus = SelectedStatus = "0";
                //
                //var itemRP = new SelectList(new[] { new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" }, new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                ViewBag.MyRP = itemRP.ToList();

                //var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" }, }, "ID", "Name", 1);
                //ViewBag.MyRP = itemRP.ToList();

                //Subject Code, Subject Name,Pending
                var itemAction = new SelectList(new[] { new { ID = "1", Name = "Stdid/Refno" }, new { ID = "2", Name = "Roll No" },
                    new { ID = "3", Name = "RegNo" }, new { ID = "3", Name = "Name" },  }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";
                //------------------------

                if (SCHL != null)
                {
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;

                    string Search = "Reg.schl like '%%'  and PracFlg is null ";
                    string SelectedAction = "0";
                    if (TempData["PracExamEnterMarksSearch"] != null)
                    {
                        Search = TempData["PracExamEnterMarksSearch"].ToString();
                        ViewBag.SelectedStatus = TempData["SelStatus"];
                        SelectedAction = TempData["SelAction"].ToString();
                        ViewBag.SelectedAction = TempData["SelActionValue"];

                        TempData["SelStatus"] = ViewBag.SelectedStatus;
                        TempData["SelRP"] = ViewBag.SelectedRP;
                        TempData["SelAction"] = SelectedAction;
                        TempData["SelActionValue"] = ViewBag.SelectedAction;
                        TempData["PracExamEnterMarksSearch"] = Search;
                    }
                    //PracExamEnterMarks
                    MS.StoreAllData = objDB.ViewPracExamEnterMarks(SelClass, RP, Cent, Search, Convert.ToInt32(SelectedStatus), pageIndex, SubCode);

                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.LastDateofSub = null;
                        ViewBag.Message = "Record Not Found";
                        ViewBag.Unlocked = ViewBag.fsCount = ViewBag.TotalCount1 = ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.LastDateofSub = MS.StoreAllData.Tables[0].Rows[0]["LastDate"];
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.Unlocked = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["Unlocked"].ToString());

                        ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcent"].ToString());
                        ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcentNM"].ToString());

                        ViewBag.fsCount = Convert.ToInt32(MS.StoreAllData.Tables[2].Rows[0]["fsCount"]);

                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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

                    if (MS.StoreAllData.Tables[2].Rows.Count > 0)
                    {
                        ViewBag.fsCount = Convert.ToInt32(MS.StoreAllData.Tables[2].Rows[0]["fsCount"]);
                    }
                    if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                    {
                        ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcent"].ToString());
                        ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcentNM"].ToString());

                        ViewBag.SubCode = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["sub"].ToString());
                        ViewBag.SubName = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["subnm"].ToString());

                        ViewBag.PrMin = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["prmin"].ToString());
                        ViewBag.PrMax = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["prmax"].ToString());
                    }

                    return View(MS);
                }
            }
            catch (Exception ex)
            {
                //return RedirectToAction("Index", "Login");
            }
            return View(MS);
        }

        [HttpPost]
        public ActionResult PracExamEnterMarks(FormCollection frm, string id, int? page)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                // string id = "";
                string SelClass = "";
                string RP = "";
                string Cent = "";
                string SubCode = "";
                string SelectedStatus = "";
                if (frm["cid"] != null)
                {
                    id = frm["cid"].ToString();
                    string[] split = id.Split('-');
                    //var parm = Class + "-" + ExamType + "-" + Centre + "-" + SUB;
                    ViewBag.SelClass = SelClass = split[0].ToString();
                    ViewBag.SelectedRP = ViewBag.SelRP = RP = split[1].ToString();
                    ViewBag.SelCent = Cent = split[2].ToString();
                    SubCode = split[3].ToString();
                    TempData["cid"] = ViewBag.cid = id.ToString();
                }
                string SCHL = Session["SCHL"].ToString();
                ViewBag.schlCode = SCHL;
                var itemStatus = new SelectList(new[] { new { ID = "1", Name = "Pending" }, new { ID = "2", Name = "Submitted" }, }, "ID", "Name", 1);
                ViewBag.MyStatus = itemStatus.ToList();
                ViewBag.SelectedStatus = SelectedStatus = "0";
                //
                //  var itemRP = new SelectList(new[] { new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" }, new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                ViewBag.MyRP = itemRP.ToList();

                //var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" }, }, "ID", "Name", 1);
                //ViewBag.MyRP = itemRP.ToList();

                //Subject Code, Subject Name,Pending
                var itemAction = new SelectList(new[] { new { ID = "1", Name = "Stdid/Refno" }, new { ID = "2", Name = "Roll No" },
                    new { ID = "3", Name = "RegNo" }, new { ID = "4", Name = "Name" },  }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";
                //------------------------



                if (SCHL != null)
                {
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                    string Search = "Reg.schl like '%%' and PracFlg is null";
                    //  Search = " a.SCHL='" + SCHL + "'";
                    Search += " and sb.pcent = '" + SCHL + "'";
                    if (frm["SelStatus"] != "" && frm["SelStatus"] != null)
                    {
                        SelectedStatus = frm["SelStatus"].ToString();
                        ViewBag.SelectedStatus = frm["SelStatus"].ToString();
                        if (SelectedStatus == "1")
                        { Search += " and isnull(OBTMARKSP,'')='' "; }
                        else if (SelectedStatus == "2")
                        { Search += " and OBTMARKSP is not null and PracFlg=1 and FPLot is null "; }

                    }

                    //if (frm["SelRP"] != "" || frm["SelRP"] !=  null)
                    //{
                    //    RP = frm["SelRP"].ToString() == "1" ? "R" : frm["SelRP"].ToString() == "2" ? "O" : "P";
                    //    ViewBag.SelectedRP = frm["SelRP"].ToString();
                    //    Search += " and Reg.rp='" + RP + "'";
                    //}
                    int SelAction = 0;
                    if (frm["SelAction"] != "")
                    {
                        ViewBag.SelectedAction = frm["SelAction"];
                        SelAction = Convert.ToInt32(frm["SelAction"].ToString());
                        if (frm["SelAction"] != "" && frm["SearchString"].ToString() != "")
                        {
                            if (SelAction == 1)
                            { Search += " and reg.std_id='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelAction == 2)
                            { Search += " and reg.roll='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelAction == 3)
                            { Search += " and reg.regno='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelAction == 4)
                            { Search += " and reg.name like '%" + frm["SearchString"].ToString() + "%'"; }
                        }
                    }

                    TempData["SelStatus"] = frm["SelStatus"];
                    TempData["SelRP"] = frm["SelRP"];
                    TempData["SelAction"] = SelAction;
                    TempData["SelActionValue"] = frm["SelAction"];
                    TempData["PracExamEnterMarksSearch"] = Search;



                    //PracExamEnterMarks(string class1, string rp, string cent, string Search,int SelectedAction, int pageNumber, string sub)
                    MS.StoreAllData = objDB.ViewPracExamEnterMarks(SelClass, RP, Cent, Search, Convert.ToInt32(SelectedStatus), pageIndex, SubCode);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.LastDateofSub = null;
                        ViewBag.Message = "Record Not Found";
                        ViewBag.Unlocked = ViewBag.fsCount = ViewBag.TotalCount1 = ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.LastDateofSub = MS.StoreAllData.Tables[0].Rows[0]["LastDate"];
                        ViewBag.Unlocked = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["Unlocked"].ToString());

                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;

                        ViewBag.fsCount = Convert.ToInt32(MS.StoreAllData.Tables[2].Rows[0]["fsCount"]);

                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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

                    if (MS.StoreAllData.Tables[2].Rows.Count > 0)
                    {
                        ViewBag.fsCount = Convert.ToInt32(MS.StoreAllData.Tables[2].Rows[0]["fsCount"]);
                    }

                    if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                    {
                        ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcent"].ToString());
                        ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcentNM"].ToString());

                        ViewBag.SubCode = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["sub"].ToString());
                        ViewBag.SubName = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["subnm"].ToString());

                        ViewBag.PrMin = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["prmin"].ToString());
                        ViewBag.PrMax = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["prmax"].ToString());
                    }

                    return View(MS);


                }
            }
            catch (Exception ex)
            {
                //return RedirectToAction("Index", "Login");
            }
            return View(MS);
        }


        [HttpPost]
        public JsonResult JqPracExamEnterMarks(string SelClass, string RP, string CandSubjectPrac)
        {
            var flag = 1;

            // CandSubject myObj = JsonConvert.DeserializeObject<CandSubject>(CandSubject);
            var objResponse1 = JsonConvert.DeserializeObject<List<CandSubjectPrac>>(CandSubjectPrac);
            DataTable dtSub = new DataTable();
            dtSub.Columns.Add("CANDID");
            dtSub.Columns.Add("SUB");
            dtSub.Columns.Add("OBTMARKSP");
            dtSub.Columns.Add("MINMARKSP");
            dtSub.Columns.Add("MAXMARKSP");
            dtSub.Columns.Add("PRACDATE");
            dtSub.Columns.Add("ACCEPT");

            DataRow row = null;
            foreach (var rowObj in objResponse1)
            {
                rowObj.OBTMARKSP = rowObj.OBTMARKSP == null ? "" : rowObj.OBTMARKSP;

                row = dtSub.NewRow();
                if (rowObj.OBTMARKSP.ToUpper() == "A" || rowObj.OBTMARKSP.ToUpper() == "ABS")
                {
                    rowObj.OBTMARKSP = "ABS";
                }
                else if (rowObj.OBTMARKSP.ToUpper() == "C" || rowObj.OBTMARKSP.ToUpper() == "CAN")
                {
                    rowObj.OBTMARKSP = "CAN";
                }
                else if (rowObj.OBTMARKSP.ToUpper() == "U" || rowObj.OBTMARKSP.ToUpper() == "UMC")
                {
                    rowObj.OBTMARKSP = "UMC";
                }
                else if (rowObj.OBTMARKSP.ToUpper() == "H" || rowObj.OBTMARKSP.ToUpper() == "HHH")
                {
                    rowObj.OBTMARKSP = "HHH";
                }
                else if (rowObj.OBTMARKSP != "")
                {
                    rowObj.OBTMARKSP = rowObj.OBTMARKSP.PadLeft(3, '0');
                }

                if (rowObj.PRACDATE != "" && rowObj.OBTMARKSP != "" && rowObj.ACCEPT.ToString().ToLower() == "true")
                {
                    dtSub.Rows.Add(rowObj.CANDID, rowObj.SUB, rowObj.OBTMARKSP, rowObj.MINMARKSP, rowObj.MAXMARKSP, rowObj.PRACDATE);
                }
            }
            dtSub.AcceptChanges();


            foreach (DataRow dr1 in dtSub.Rows)
            {


                if (dr1["OBTMARKSP"].ToString() == "" || dr1["OBTMARKSP"].ToString() == "HHH" || dr1["OBTMARKSP"].ToString() == "ABS" || dr1["OBTMARKSP"].ToString() == "CAN" || dr1["OBTMARKSP"].ToString() == "UMC")
                { }
                else if (dr1["OBTMARKSP"].ToString() == "0" || dr1["OBTMARKSP"].ToString().ToUpper().Contains("A") || dr1["OBTMARKSP"].ToString().ToUpper().Contains("C") || dr1["OBTMARKSP"].ToString().ToUpper().Contains("U"))
                {
                    flag = -1;
                    var results = new
                    {
                        status = flag
                    };
                    return Json(results);
                }
                else
                {
                    int obt = Convert.ToInt32(dr1["OBTMARKSP"].ToString());
                    int min = Convert.ToInt32(dr1["MINMARKSP"].ToString());
                    int max = Convert.ToInt32(dr1["MAXMARKSP"].ToString());

                    if ((obt < 0) || (obt > max))
                    {
                        flag = -2;
                    }
                }
            }
            if (flag == 1 && dtSub.Rows.Count > 0)
            {
                if (dtSub.Columns.Contains("ACCEPT"))
                {
                    dtSub.Columns.Remove("ACCEPT");
                }
                string dee = "1";
                string class1 = "4";
                int OutStatus = 0;
                string OutError = string.Empty;
                dee = objDB.AllotPracMarks(RP, dtSub, SelClass, out OutStatus, out OutError);
                var results = new
                {
                    status = OutError
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


        #endregion PracExamEnterMarks


        #region ViewPracExamFinalSubmit


        public ActionResult ViewPracExamFinalSubmit(FormCollection frm, string id, int? page)
        {

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SchoolModels MS = new SchoolModels();
            try
            {
                string SelClass = "";
                string RP = "";
                string Cent = "";
                string SubCode = "";
                string SelectedStatus = "0";
                if (id != null)
                {
                    string[] split = id.Split('-');
                    //var parm = Class + "-" + ExamType + "-" + Centre + "-" + SUB;
                    ViewBag.Class = ViewBag.SelClass = SelClass = split[0].ToString();
                    ViewBag.RP = ViewBag.SelRP = RP = split[1].ToString();
                    ViewBag.SelCent = Cent = split[2].ToString();
                    SubCode = split[3].ToString();
                    TempData["cid"] = ViewBag.cid = id.ToString();
                }
                string SCHL = Session["SCHL"].ToString();
                ViewBag.schlCode = SCHL;

                //------------------------

                if (SCHL != null)
                {
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;

                    string Search = "Reg.schl like '%%' and FPLot is null and OBTMARKSP is not null and PracFlg=1";
                    string SelectedAction = "0";
                    if (TempData["ViewPracExamFinalSubmitSearch"] != null)
                    {
                        Search = TempData["ViewPracExamFinalSubmitSearch"].ToString();
                        TempData["ViewPracExamFinalSubmitSearch"] = Search;
                    }


                    MS.StoreAllData = objDB.ViewPracExamFinalSubmit(SelClass, RP, Cent, Search, Convert.ToInt32(SelectedStatus), pageIndex, SubCode);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.LastDateofSub = null;
                        ViewBag.Message = "Record Not Found";
                        ViewBag.Unlocked = ViewBag.TotalCount1 = ViewBag.TotalCount = 0;

                    }
                    else
                    {
                        ViewBag.LastDateofSub = MS.StoreAllData.Tables[0].Rows[0]["LastDate"];
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.Unlocked = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["Unlocked"].ToString());

                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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

                    if (MS.StoreAllData.Tables[2].Rows.Count > 0)
                    {
                        ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcent"].ToString());
                        ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcentNM"].ToString());

                        ViewBag.SubCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["sub"].ToString());
                        ViewBag.SubName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["subnm"].ToString());

                        ViewBag.PrMin = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmin"].ToString());
                        ViewBag.PrMax = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmax"].ToString());
                    }

                    return View(MS);
                }
            }
            catch (Exception ex)
            {
                //return RedirectToAction("Index", "Login");
            }
            return View(MS);
        }

        [HttpPost]
        public ActionResult ViewPracExamFinalSubmit(FormCollection frm, int? page)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            SchoolModels MS = new SchoolModels();
            try
            {
                string id = "";
                string SelClass = "";
                string RP = "";
                string Cent = "";
                string SubCode = "";
                string SelectedStatus = "";
                if (frm["cid"] != null)
                {
                    id = frm["cid"].ToString();
                    string[] split = id.Split('-');
                    //var parm = Class + "-" + ExamType + "-" + Centre + "-" + SUB;
                    ViewBag.SelClass = SelClass = split[0].ToString();
                    ViewBag.SelRP = RP = split[1].ToString();
                    ViewBag.SelCent = Cent = split[2].ToString();
                    SubCode = split[3].ToString();
                    TempData["cid"] = ViewBag.cid = id.ToString();
                }
                string SCHL = Session["SCHL"].ToString();
                ViewBag.schlCode = SCHL;

                //------------------------



                if (SCHL != null)
                {
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                    string Search = "Reg.schl like '%%' and FPLot is null and OBTMARKSP is not null and PracFlg=1 ";
                    //  Search = " a.SCHL='" + SCHL + "'";
                    Search += " and sb.pcent = '" + SCHL + "'";

                    TempData["ViewPracExamFinalSubmitSearch"] = Search;
                    //PracExamFinalSubmit(string class1, string rp, string cent, string Search,int SelectedAction, int pageNumber, string sub)

                    MS.StoreAllData = objDB.ViewPracExamFinalSubmit(SelClass, RP, Cent, Search, Convert.ToInt32(SelectedStatus), pageIndex, SubCode);

                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.LastDateofSub = null;
                        ViewBag.Message = "Record Not Found";
                        ViewBag.Unlocked = ViewBag.TotalCount1 = ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.LastDateofSub = MS.StoreAllData.Tables[0].Rows[0]["LastDate"];
                        ViewBag.Unlocked = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["Unlocked"].ToString());

                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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

                    if (MS.StoreAllData.Tables[2].Rows.Count > 0)
                    {
                        ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcent"].ToString());
                        ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcentNM"].ToString());

                        ViewBag.SubCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["sub"].ToString());
                        ViewBag.SubName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["subnm"].ToString());

                        ViewBag.PrMin = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmin"].ToString());
                        ViewBag.PrMax = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmax"].ToString());
                    }

                    return View(MS);
                }
            }
            catch (Exception ex)
            {
                //return RedirectToAction("Index", "Login");
            }


            return View(MS);
        }


        [HttpPost]
        public JsonResult JqRemovePracMarks(string SelClass, string RP, string CandSubjectPrac)
        {
            var flag = 1;

            // CandSubject myObj = JsonConvert.DeserializeObject<CandSubject>(CandSubject);
            var objResponse1 = JsonConvert.DeserializeObject<List<CandSubjectPrac>>(CandSubjectPrac);
            DataTable dtSub = new DataTable();
            dtSub.Columns.Add("CANDID");
            dtSub.Columns.Add("SUB");
            dtSub.Columns.Add("OBTMARKSP");
            dtSub.Columns.Add("MINMARKSP");
            dtSub.Columns.Add("MAXMARKSP");
            dtSub.Columns.Add("PRACDATE");
            dtSub.Columns.Add("ACCEPT");

            DataRow row = null;
            foreach (var rowObj in objResponse1)
            {
                rowObj.OBTMARKSP = rowObj.OBTMARKSP == null ? "" : rowObj.OBTMARKSP;

                row = dtSub.NewRow();

                if (rowObj.PRACDATE != "" && rowObj.OBTMARKSP != "" && rowObj.ACCEPT.ToString().ToLower() == "true")
                {
                    dtSub.Rows.Add(rowObj.CANDID, rowObj.SUB, rowObj.OBTMARKSP, rowObj.MINMARKSP, rowObj.MAXMARKSP, rowObj.PRACDATE);
                }
            }
            dtSub.AcceptChanges();

            if (flag == 1 && dtSub.Rows.Count > 0)
            {
                if (dtSub.Columns.Contains("ACCEPT"))
                {
                    dtSub.Columns.Remove("ACCEPT");
                }
                string dee = "1";
                string class1 = "4";
                int OutStatus = 0;
                string OutError = string.Empty;
                dee = objDB.RemovePracMarks(RP, dtSub, SelClass, out OutStatus, out OutError);
                var results = new
                {
                    status = OutError
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

        #endregion PracExamFinalSubmit


        #region PracExamRoughReport
        [OutputCache(Duration = 180, VaryByParam = "id", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult PracExamRoughReport(string id, int? page)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["PracticalExamMarks"] == null)
            {
                return RedirectToAction("PracticalExamMarks", "School");
            }
            SchoolModels MS = new SchoolModels();
            try
            {
                string SelClass = "";
                string RP = "";
                string Cent = "";
                string SubCode = "";
                string SelectedStatus = "0";
                if (id != null)
                {
                    string[] split = id.Split('-');
                    //var parm = Class + "-" + ExamType + "-" + Centre + "-" + SUB;
                    ViewBag.Class = ViewBag.SelClass = SelClass = split[0].ToString();
                    ViewBag.RP = ViewBag.SelRP = RP = split[1].ToString();
                    ViewBag.SelCent = Cent = split[2].ToString();
                    SubCode = split[3].ToString();
                    ViewBag.cid = id.ToString();
                }
                string SCHL = Session["SCHL"].ToString();
                ViewBag.schlCode = SCHL;

                //------------------------

                if (SCHL != null)
                {
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                    string Search = "Reg.schl like '%%'  ";
                    Search += " and sb.pcent = '" + SCHL + "'";

                    MS.StoreAllData = objDB.ViewPracExamFinalSubmit(SelClass, RP, Cent, Search, Convert.ToInt32(SelectedStatus), pageIndex, SubCode);

                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;

                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;

                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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

                    if (MS.StoreAllData.Tables[2].Rows.Count > 0)
                    {
                        ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcent"].ToString());
                        ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcentNM"].ToString());

                        ViewBag.SubCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["sub"].ToString());
                        ViewBag.SubName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["subnm"].ToString());

                        ViewBag.PrMin = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmin"].ToString());
                        ViewBag.PrMax = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmax"].ToString());
                    }

                    return View(MS);
                }
            }
            catch (Exception ex)
            {
                //return RedirectToAction("Index", "Login");
            }
            return View(MS);
        }

        #endregion PracExamRoughReport

        #region PracExamFinalReport      
        [OutputCache(Duration = 180, VaryByParam = "id", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult PracExamFinalReport(string id, SchoolModels MS, string SelLot, FormCollection frm)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["PracticalExamMarks"] == null)
            {
                return RedirectToAction("PracticalExamMarks", "School");
            }
            try
            {
                string SelClass = "";
                string RP = "";
                string Cent = "";
                string SubCode = "";
                string SelectedStatus = "0";
                if (id != null)
                {
                    string[] split = id.Split('-');
                    //var parm = Class + "-" + ExamType + "-" + Centre + "-" + SUB;
                    ViewBag.Class = ViewBag.SelClass = SelClass = split[0].ToString();
                    ViewBag.RP = ViewBag.SelRP = RP = split[1].ToString();
                    ViewBag.SelCent = Cent = split[2].ToString();
                    SubCode = split[3].ToString();
                    TempData["cid"] = ViewBag.cid = id.ToString();
                }
                string SCHL = Session["SCHL"].ToString();
                ViewBag.schlCode = SCHL;
                ViewBag.SelectedLot = SelLot == null ? "0" : SelLot;
                //------------------------

                if (SCHL != null)
                {

                    string Search = "Reg.schl like '%%' and FPLot is not null and OBTMARKSP is not null and PracFlg=1";

                    if (frm["SelLot"] != "" && frm["SelLot"] != null)
                    {
                        ViewBag.SelectedLot = frm["SelLot"].ToString();
                        Search += " and FPLot='" + ViewBag.SelectedLot + "'  ";
                    }

                    MS.StoreAllData = objDB.ViewPracExamFinalSubmit(SelClass, RP, Cent, Search, Convert.ToInt32(5), 0, SubCode);

                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.Unlocked = ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.fplot = MS.StoreAllData.Tables[0].Rows[0]["fplot"].ToString();
                        ViewBag.fplot2 = MS.StoreAllData.Tables[0].Rows[0]["fplot2"].ToString();
                        ViewBag.PracInsDate = MS.StoreAllData.Tables[0].Rows[0]["PracInsDate"].ToString();
                        ViewBag.Unlocked = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["Unlocked"].ToString());

                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        ViewBag.TotalCount1 = count;
                    }

                    if (MS.StoreAllData.Tables[2].Rows.Count > 0)
                    {
                        ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcent"].ToString());
                        ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcentNM"].ToString());

                        ViewBag.SubCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["sub"].ToString());
                        ViewBag.SubName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["subnm"].ToString());

                        ViewBag.PrMin = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmin"].ToString());
                        ViewBag.PrMax = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmax"].ToString());
                    }
                    if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                    {
                        DataTable dtLot = MS.StoreAllData.Tables[3];
                        // English
                        List<SelectListItem> itemLot = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dtLot.Rows)
                        {
                            itemLot.Add(new SelectListItem { Text = @dr["fplot"].ToString().Trim(), Value = @dr["fplot"].ToString().Trim() });
                        }
                        ViewBag.itemLot = itemLot;
                    }

                    return View(MS);
                }
            }
            catch (Exception ex)
            {
                //return RedirectToAction("Index", "Login");
            }
            return View(MS);
        }

        [HttpPost]
        public JsonResult JqPracExamFinalReport(string ExamCent, string SelClass, string RP, string CandPracExaminer)
        {
            var flag = 1;
            string SCHL = Session["SCHL"].ToString();
            string SUB = "";
            string CENT = "";
            DataTable dtSub = new DataTable();
            if (CandPracExaminer != null)
            {
                // CandSubject myObj = JsonConvert.DeserializeObject<CandSubject>(CandSubject);
                var objResponse1 = JsonConvert.DeserializeObject<List<CandPracExaminer>>(CandPracExaminer);
                dtSub.Columns.Add("SUB");
                dtSub.Columns.Add("CENT");
                dtSub.Columns.Add("EXAMINER");
                dtSub.Columns.Add("SCHOOL");
                dtSub.Columns.Add("TEACHER");
                dtSub.Columns.Add("MOBILE");

                DataRow row = null;
                foreach (var rowObj in objResponse1)
                {
                    //rowObj.OBTMARKSP = rowObj.OBTMARKSP == null ? "" : rowObj.OBTMARKSP;

                    row = dtSub.NewRow();

                    if (rowObj.SUB != "" && rowObj.CENT != "" && rowObj.EXAMINER != "" && rowObj.SCHOOL != "" && rowObj.TEACHER != "" && rowObj.MOBILE != "")
                    {
                        SUB = rowObj.SUB;
                        CENT = rowObj.CENT;
                        dtSub.Rows.Add(rowObj.SUB, rowObj.CENT, rowObj.EXAMINER, rowObj.SCHOOL, rowObj.TEACHER, rowObj.MOBILE);
                    }
                }
                dtSub.AcceptChanges();
            }

            //if (flag == 1 && dtSub.Rows.Count > 0 && SUB!="" && CENT != "" && SCHL != "")
            if (flag == 1 && SUB != "" && CENT != "" && SCHL != "")
            {
                if (string.IsNullOrEmpty(ExamCent))
                {
                    ExamCent = "";
                }

                string dee = "0";
                int OutStatus = 0;
                string OutError = string.Empty;
                dee = objDB.PracExamFinalSubmit(ExamCent, SelClass, RP, CENT, SUB, SCHL, dtSub, out OutStatus, out OutError);
                var results = new
                {
                    status = OutError
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


        #endregion PracExamFinalReport


        #region Prac Exam Final Submit      

        public ActionResult PracExamFinalSubmit(string id, SchoolModels MS)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            if (Session["PracticalExamMarks"] == null)
            {
                return RedirectToAction("PracticalExamMarks", "School");
            }
            try
            {
                string SelClass = "";
                string RP = "";
                string Cent = "";
                string SubCode = "";
                string SelectedStatus = "0";
                if (id != null)
                {
                    string[] split = id.Split('-');
                    //var parm = Class + "-" + ExamType + "-" + Centre + "-" + SUB;
                    ViewBag.Class = ViewBag.SelClass = SelClass = split[0].ToString();
                    ViewBag.RP = ViewBag.SelRP = RP = split[1].ToString();
                    ViewBag.SelCent = Cent = split[2].ToString();
                    SubCode = split[3].ToString();
                    TempData["cid"] = ViewBag.cid = id.ToString();
                }
                string SCHL = Session["SCHL"].ToString();
                ViewBag.schlCode = SCHL;

                //------------------------

                if (SCHL != null)
                {

                    string Search = "Reg.schl like '%%' and FPLot is null and OBTMARKSP is not null and PracFlg=1";
                    MS.StoreAllData = objDB.ViewPracExamFinalSubmit(SelClass, RP, Cent, Search, Convert.ToInt32(SelectedStatus), 0, SubCode);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.LastDateofSub = null;
                        ViewBag.Message = "Record Not Found";
                        ViewBag.Unlocked = ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.LastDateofSub = MS.StoreAllData.Tables[0].Rows[0]["LastDate"];
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        ViewBag.Unlocked = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["Unlocked"].ToString());
                        ViewBag.TotalCount1 = count;
                    }

                    if (MS.StoreAllData.Tables[2].Rows.Count > 0)
                    {
                        ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcent"].ToString());
                        ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcentNM"].ToString());

                        ViewBag.SubCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["sub"].ToString());
                        ViewBag.SubName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["subnm"].ToString());

                        ViewBag.PrMin = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmin"].ToString());
                        ViewBag.PrMax = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmax"].ToString());
                    }

                    return View(MS);
                }
            }
            catch (Exception ex)
            {
                //return RedirectToAction("Index", "Login");
            }
            return View(MS);
        }



        #endregion Prac Exam Final Submit


        #region Signature Chart and Confidential List 

        #region Prac SignatureChart Matric
        public ActionResult PracSignatureChartMatric()
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = Session["cent"].ToString();
                    if (Schl != "")
                    {
                        //DataSet Dresult = objDB.GetCentcode(Schl);
                        DataSet Dresult = objDB.GetPracCentcodeByClass(Schl, 2);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["pcentNM"].ToString(), Value = @dr["pcent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        sm.ExamCent = Cent;
                        sm.ExamSub = "";
                        sm.ExamRoll = "";
                        sm.CLASS = "2";

                        DataSet Subresult = objDB.GetSubFromSubMasters(2, "P", Schl, Cent); // for matric prac sub
                        List<SelectListItem> Sublist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Subresult.Tables[0].Rows)
                        {
                            Sublist.Add(new SelectListItem { Text = @dr["subnm"].ToString(), Value = @dr["sub"].ToString() });
                        }

                        ViewBag.MyExamSub = Sublist;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }
        [HttpPost]
        public ActionResult PracSignatureChartMatric(FormCollection frc)
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = frc["ExamCent"].ToString();
                    //string sub = frc["ExamSub"].ToString();
                    string roll = frc["ExamRoll"].ToString();
                    if (Cent != "")
                    {

                        sm.ExamCent = Cent;
                        sm.ExamSub = frc["ExamSub"].ToString();
                        sm.ExamRoll = frc["ExamRoll"].ToString();
                        sm.CLASS = "2";
                        //DataSet Dresult = objDB.GetCentcode(Schl);
                        DataSet Dresult = objDB.GetPracCentcodeByClass(Schl, 2);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["pcentNM"].ToString(), Value = @dr["pcent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        //GetSubFromSubMasters(int cls , string type)
                        DataSet Subresult = objDB.GetSubFromSubMasters(2, "P", Schl, Cent); // for matric prac sub
                        List<SelectListItem> Sublist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Subresult.Tables[0].Rows)
                        {
                            Sublist.Add(new SelectListItem { Text = @dr["subnm"].ToString(), Value = @dr["sub"].ToString() });
                        }

                        ViewBag.MyExamSub = Sublist;

                        sm.StoreAllData = objDB.PracSignatureChart(sm);
                        sm.ExamCent = Cent;
                        ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.SearchMsg = sm.StoreAllData.Tables[0].Rows.Count;
                        if (ViewBag.SearchMsg == 0)
                        {
                            ViewBag.Message = "No Record Found";
                        }
                        return View(sm);
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }


        public ActionResult PracConfidentialListMatric()
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = Session["cent"].ToString();
                    if (Schl != "")
                    {

                        sm.ExamCent = Cent;
                        sm.CLASS = "2";
                        //DataSet Dresult = objDB.GetCentcode(Schl);
                        DataSet Dresult = objDB.GetPracCentcodeByClass(Schl, 2);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["pcentNM"].ToString(), Value = @dr["pcent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }
        [HttpPost]
        public ActionResult PracConfidentialListMatric(FormCollection frc)
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = frc["ExamCent"].ToString();
                    if (Cent != "")
                    {

                        sm.ExamCent = Cent;

                        //DataSet Dresult = objDB.GetCentcode(Schl);
                        DataSet Dresult = objDB.GetPracCentcodeByClass(Schl, 2);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["pcentNM"].ToString(), Value = @dr["pcent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        string search;
                        if (frc["ExamCent"].ToString() != "")
                        {
                            sm.ExamCent = frc["ExamCent"].ToString();
                            sm.CLASS = "2";
                            //search = search + "'" + sm.ExamCent + "'";
                        }
                        //if (frc["ExamRoll"].ToString().Trim() !="")
                        //{
                        //    sm.ExamRoll = frc["ExamRoll"].ToString();
                        //    //search = search + "'" + sm.ExamCent + "'";
                        //}

                        sm.StoreAllData = objDB.PracConfidentialList(sm);
                        sm.ExamCent = Session["cent"].ToString();
                        ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        return View(sm);
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }
        #endregion  Prac SignatureChart Matric

        #region Prac SignatureChart Senior
        public ActionResult PracSignatureChartSr()
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = Session["cent"].ToString();
                    if (Schl != "")
                    {
                        //DataSet Dresult = objDB.GetCentcode(Schl);
                        DataSet Dresult = objDB.GetPracCentcodeByClass(Schl, 4);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["pcentNM"].ToString(), Value = @dr["pcent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        sm.ExamCent = Cent;
                        sm.ExamSub = "";
                        sm.ExamRoll = "";


                        //GetSubFromSubMasters(int cls , string type)
                        DataSet Subresult = objDB.GetSubFromSubMasters(4, "P", Schl, Cent); // for matric prac sub
                        List<SelectListItem> Sublist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Subresult.Tables[0].Rows)
                        {
                            Sublist.Add(new SelectListItem { Text = @dr["subnm"].ToString(), Value = @dr["sub"].ToString() });
                        }

                        ViewBag.MyExamSub = Sublist;

                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }

        [HttpPost]
        public ActionResult PracSignatureChartSr(FormCollection frc)
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = frc["ExamCent"].ToString();
                    //string sub = frc["ExamSub"].ToString();
                    string roll = frc["ExamRoll"].ToString();
                    if (Cent != "")
                    {

                        sm.ExamCent = Cent;
                        sm.ExamSub = frc["ExamSub"].ToString();
                        sm.ExamRoll = frc["ExamRoll"].ToString();
                        sm.CLASS = "4";
                        //DataSet Dresult = objDB.GetCentcode(Schl);
                        DataSet Dresult = objDB.GetPracCentcodeByClass(Schl, 4);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["pcentNM"].ToString(), Value = @dr["pcent"].ToString() });
                        }
                        ViewBag.MySchCode = schllist;

                        //GetSubFromSubMasters(int cls , string type)
                        DataSet Subresult = objDB.GetSubFromSubMasters(4, "P", Schl, Cent); // for matric prac sub
                        List<SelectListItem> Sublist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Subresult.Tables[0].Rows)
                        {
                            Sublist.Add(new SelectListItem { Text = @dr["subnm"].ToString(), Value = @dr["sub"].ToString() });
                        }

                        ViewBag.MyExamSub = Sublist;

                        sm.StoreAllData = objDB.PracSignatureChart(sm);
                        sm.ExamCent = Cent;
                        ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.SearchMsg = sm.StoreAllData.Tables[1].Rows.Count;
                        if (ViewBag.SearchMsg == 0)
                        {
                            ViewBag.TotalCount = 0;
                            ViewBag.Message = "No Record Found";
                        }
                        return View(sm);
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }
        public ActionResult PracConfidentialListSenior()
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = Session["cent"].ToString();
                    if (Schl != "")
                    {

                        sm.ExamCent = Cent;
                        sm.CLASS = "4";
                        //DataSet Dresult = objDB.GetCentcode(Schl);
                        DataSet Dresult = objDB.GetPracCentcodeByClass(Schl, 4);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["pcentNM"].ToString(), Value = @dr["pcent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }

        [HttpPost]
        public ActionResult PracConfidentialListSenior(FormCollection frc)
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = frc["ExamCent"].ToString();
                    if (Cent != "")
                    {

                        sm.ExamCent = Cent;
                        sm.CLASS = "4";
                        //DataSet Dresult = objDB.GetCentcode(Schl);
                        DataSet Dresult = objDB.GetPracCentcodeByClass(Schl, 4);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["pcentNM"].ToString(), Value = @dr["pcent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        string search;
                        if (frc["ExamCent"].ToString() != "")
                        {
                            sm.ExamCent = frc["ExamCent"].ToString();
                            //search = search + "'" + sm.ExamCent + "'";
                        }
                        //if (frc["ExamRoll"].ToString().Trim() !="")
                        //{
                        //    sm.ExamRoll = frc["ExamRoll"].ToString();
                        //    //search = search + "'" + sm.ExamCent + "'";
                        //}

                        sm.StoreAllData = objDB.PracConfidentialList(sm);
                        sm.ExamCent = Session["cent"].ToString();
                        ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        return View(sm);
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }
        #endregion  Prac SignatureChart Senior


        #endregion  Signature Chart and Confidential List 

        public JsonResult UnlockCCE(string Schl, string Type)
        {
            if (Session["AdminId"] == null)
            {
                return Json(new { status = -1 }, JsonRequestBehavior.AllowGet);
            }
            int AdminId = Convert.ToInt32(Session["AdminId"]);

            int outid = 0;
            if (!string.IsNullOrEmpty(Schl) && !string.IsNullOrEmpty(Type))
            {
                DataSet ds = objDB.UnlockCCE(Schl, Type, AdminId, out outid);
            }
            return Json(new { status = outid }, JsonRequestBehavior.AllowGet);
        }

        #region Attendance Supervisory Staff 
        public ActionResult AttendanceSupervisoryStaff()
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = Session["cent"].ToString();
                    if (Cent != "")
                    {
                        DataSet Dresult = objDB.GetCentcode(Schl);
                        List<SelectListItem> schllist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                        }

                        ViewBag.MySchCode = schllist;
                        sm.ExamCent = Cent;

                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                        ViewBag.Message = "Data Not Found";

                        //DataSet Subresult = objDB.SignatureChartSrSub(sm);
                        //List<SelectListItem> Sublist = new List<SelectListItem>();
                        //foreach (System.Data.DataRow dr in Subresult.Tables[0].Rows)
                        //{
                        //    Sublist.Add(new SelectListItem { Text = @dr["subnm"].ToString(), Value = @dr["sub"].ToString() });
                        //}

                        //ViewBag.MyExamSub = Sublist;

                        //// sm.StoreAllData = objDB.SignatureChartSr(sm);
                        ////sm.ExamCent= Session["cent"].ToString();
                        ////ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.msg = "Data Not Found";
                        ViewBag.Message = "Data Not Found";
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }

        [HttpPost]
        public ActionResult AttendanceSupervisoryStaff(FormCollection frc, string cmd)
        {
            SchoolModels sm = new SchoolModels();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            try
            {
                if (Session["SCHL"] == null)
                { return RedirectToAction("Index", "Login"); }
                else
                {
                    string Schl = Session["SCHL"].ToString();
                    string Cent = frc["ExamCent"].ToString();
                    string Class = frc["Class"].ToString();
                    string txtadmisndate = frc["IDATE"].ToString();

                    DataSet Dresult = objDB.GetCentcode(Schl);
                    List<SelectListItem> schllist = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                    }

                    ViewBag.MySchCode = schllist;

                    if (txtadmisndate != "")
                    {
                        string dd = txtadmisndate.Substring(0, 2);
                        string mm = txtadmisndate.Substring(3, 2);
                        string yy = txtadmisndate.Substring(6, 4);
                        txtadmisndate = mm + '/' + dd + '/' + yy;
                    }
                    #region if (Cent != "" && cmd == "Search")
                    if (Cent != "" && cmd == "Search")
                    {
                        txtadmisndate = frc["IDATE"].ToString();
                        sm.ExamCent = Cent;
                        sm.CLASS = Class;
                        sm.IDATE = txtadmisndate;

                        sm.StoreAllData = objDB.GetAttendanceSupervisoryStaff(sm, txtadmisndate);
                        sm.ExamCent = Cent;
                        ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.SearchMsg = sm.StoreAllData.Tables[0].Rows.Count;
                        if (ViewBag.SearchMsg == 0)
                        {
                            ViewBag.Message = "No Record Found";
                        }
                        return View(sm);
                    }
                    #endregion

                    #region else if (cmd == "Present" || cmd == "Not Present")
                    else if (cmd == "Present" || cmd == "Not Present")
                    {
                        sm.ExamCent = Cent;
                        sm.CLASS = Class;



                        string attendance = (cmd == "Present" ? "P" : "A");

                        string EpunSearch = frc["ChkStaffID"].ToString();

                        string[] split1 = EpunSearch.Split(',');
                        int sCount = split1.Length;
                        if (sCount > 0)
                        {
                            foreach (string s in split1)
                            {
                                string epunjid = s.Split('(')[0];
                                //string remark = s.Split('(', ')')[1];
                                if (epunjid != "")
                                {
                                    sm.StoreAllData = objDB.SetAttendanceSupervisoryStaff(sm, epunjid, attendance, txtadmisndate);
                                }
                            }
                        }

                        //--------- Search Fill Grid Data --------
                        sm.IDATE = frc["IDATE"].ToString();
                        txtadmisndate = frc["IDATE"].ToString();
                        //if (txtadmisndate != "")
                        //{
                        //    string dd = txtadmisndate.Substring(0, 2);
                        //    string mm = txtadmisndate.Substring(3, 2);
                        //    string yy = txtadmisndate.Substring(6, 4);
                        //    txtadmisndate = mm + '/' + dd + '/' + yy;
                        //}

                        sm.StoreAllData = objDB.GetAttendanceSupervisoryStaff(sm, txtadmisndate);
                        sm.ExamCent = Cent;
                        ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.SearchMsg = sm.StoreAllData.Tables[0].Rows.Count;
                        if (ViewBag.SearchMsg == 0)
                        {
                            ViewBag.Message = "No Record Found";
                        }
                    }
                    #endregion

                    #region else if (cmd == "Delete Staff")
                    else if (cmd == "Delete Staff")
                    {
                        sm.ExamCent = Cent;
                        sm.CLASS = Class;

                        string EpunSearch = frc["ChkStaffID"].ToString();

                        string[] split1 = EpunSearch.Split(',');
                        int sCount = split1.Length;
                        if (sCount > 0)
                        {
                            foreach (string s in split1)
                            {
                                string epunjid = s.Split('(')[0];
                                //string remark = s.Split('(', ')')[1];
                                if (epunjid != "")
                                {
                                    sm.StoreAllData = objDB.DelAttendanceSupervisoryStaff(sm, epunjid);
                                }
                            }
                        }

                        //--------- Search Fill Grid Data --------
                        sm.IDATE = frc["IDATE"].ToString();
                        txtadmisndate = frc["IDATE"].ToString();
                        //if (txtadmisndate != "")
                        //{
                        //    string dd = txtadmisndate.Substring(0, 2);
                        //    string mm = txtadmisndate.Substring(3, 2);
                        //    string yy = txtadmisndate.Substring(6, 4);
                        //    txtadmisndate = mm + '/' + dd + '/' + yy;
                        //}

                        sm.StoreAllData = objDB.GetAttendanceSupervisoryStaff(sm, txtadmisndate);
                        sm.ExamCent = Cent;
                        ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.SearchMsg = sm.StoreAllData.Tables[0].Rows.Count;
                        if (ViewBag.SearchMsg == 0)
                        {
                            ViewBag.Message = "No Record Found";
                        }
                    }
                    #endregion

                    #region else if (cmd == "Final Submit")
                    else if (Cent != "" && Class != "" && cmd == "Final Submit")
                    {
                        sm.ExamCent = Cent;
                        sm.CLASS = Class;
                        sm.StoreAllData = objDB.FSAttendanceSupervisoryStaff(sm);

                        ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.SearchMsg = sm.StoreAllData.Tables[0].Rows.Count;
                    }
                    #endregion

                    #region else if (cmd == "Print")


                    else if (Cent != "" && Class != "" && (cmd == "Rough Print" || cmd == "Final Print"))
                    {

                        string finalflg = (cmd == "Final Print" ? "1" : "0");
                        sm.StoreAllData = objDB.GetAttendanceSupervisoryStaffReport(Cent, Class, finalflg);
                        if (sm.StoreAllData.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt = sm.StoreAllData.Tables[0];
                            DataTable dt2 = sm.StoreAllData.Tables[1];
                            //dt.Merge(dt2);
                            string fname = DateTime.Now.ToString("ddMMyyyyHHmm");
                            Response.Clear();
                            Response.Buffer = true;
                            Response.Charset = "";
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.AddHeader("content-disposition", "attachment;filename=AttendenceList" + fname + ".xlsx");
                            using (MemoryStream MyMemoryStream = new MemoryStream())
                            {
                                XLWorkbook wb = new XLWorkbook();

                                var WS = wb.Worksheets.Add(dt, "AttendanceList" + fname);
                                //WS = wb.Worksheets.Add(dt2, "AttendenceList" + fname);
                                WS.Tables.FirstOrDefault().ShowAutoFilter = false;
                                wb.SaveAs(MyMemoryStream);
                                MyMemoryStream.WriteTo(Response.OutputStream);
                                WS.AutoFilter.Enabled = false;
                                Response.Flush();
                                Response.End();
                            }
                        }

                    }
                    #endregion

                    else
                    {
                        ViewBag.TotalCount = 0;
                        ViewBag.Message = "No Record Found";
                    }
                    return View(sm);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }

        }
        //$.post("/School/UpdateBankAcStaff", { "bank": bank, "acno": acno, "ifsc": ifsc, "epunjabid": k, "cent": cent },
        [HttpPost]
        public JsonResult UpdateBankAcStaff(string bankname, string acno, string ifsc, string epunjabid, string cent)
        {
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

            try
            {

                string dee = "";
                string outstatus = "";
                string Search = string.Empty;
                string res = null;
                //string UserType = "Admin";               
                //float fee = 0;              
                //DateTime date;              
                DataSet result = objDB.UpdateBankAcStaff(bankname, acno, ifsc, epunjabid, cent);
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
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return null;
            }
        }

        [HttpPost]
        public JsonResult CheckifscVal(string ifsc)
        {
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

            try
            {

                string dee = "";
                string outstatus = "";
                string Search = string.Empty;
                string res = string.Empty;
                //string UserType = "Admin";               
                //float fee = 0;              
                //DateTime date;              
                DataSet result = objDB.CheckifscVal(ifsc);
                res = result.Tables[0].Rows[0]["res"].ToString();
                if (result.Tables[0].Rows.Count > 0)
                {
                    //dee = result.Tables[0].Rows[0]["res"].ToString(); //"Yes";
                    return Json(res);
                }
                return Json(res);
                //return Json(new { sn = dee, chid = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return null;
            }
        }

        #endregion Attendance Supervisory Staff 


        #region  View Student Certificate

        [SessionCheckFilter]
        public ActionResult SchoolResultCertificate()
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];

            string Schl = Session["SCHL"].ToString();
            DataSet result = objCommon.schooltypes(Schl); // passing Value to DBClass from model
            if (result == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Senior = loginSession.Senior;
            ViewBag.Matric = loginSession.Matric;
            ViewBag.OSenior = loginSession.OSenior;
            ViewBag.OMATRIC = loginSession.OMATRIC;
            //if (result.Tables[1].Rows.Count > 0)
            //{
            //    ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
            //    ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
            //}
            return View();
        }



        [SessionCheckFilter]
        public ActionResult StudentResultCertificate(string id)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];

            SchoolModels MS = new SchoolModels();
            string Schl = Session["SCHL"].ToString();
            try
            {
                string CLASS = id == "Matric" ? "10" : id == "Senior" ? "12" : id == "MatricOpen" ? "10" : id == "SeniorOpen" ? "12" : "";
                string RP = id == "Matric" ? "R" : id == "Senior" ? "R" : id == "MatricOpen" ? "O" : id == "SeniorOpen" ? "O" : "";


                ViewBag.schlCode = Session["SCHL"].ToString();
                ViewBag.cid = id;

                var itemFilter = new SelectList(new[] { new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                 new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";

                string Search = string.Empty;
                Search = "a.SCHL Like '%' ";

                MS.StoreAllData = new AbstractLayer.SchoolDB().GetSchoolResultDetails(Search, Session["SCHL"].ToString(), CLASS, RP);
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    Session["StudentResultCertificate"] = null;
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    Session["StudentResultCertificate"] = MS.StoreAllData.Tables[0];
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;


                    return View(MS);
                }

            }
            catch (Exception ex)
            {
                //return RedirectToAction("Index", "Login");
            }


            return View();
        }

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult StudentResultCertificate(FormCollection frm, string id, string cmd, string submit)
        {

            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            SchoolModels MS = new SchoolModels();
            try
            {
                string CLASS = id == "Matric" ? "10" : id == "Senior" ? "12" : id == "MatricOpen" ? "10" : id == "SeniorOpen" ? "12" : "";
                string RP = id == "Matric" ? "R" : id == "Senior" ? "R" : id == "MatricOpen" ? "O" : id == "SeniorOpen" ? "O" : "";

                ViewBag.schlCode = Session["SCHL"].ToString();
                ViewBag.cid = id;

                var itemFilter = new SelectList(new[] { new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                 new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";


                string Search = string.Empty;
                Search = "a.SCHL Like '%' ";


                if (frm["SelFilter"] != "")
                {
                    TempData["SelFilter"] = frm["SelFilter"];
                    ViewBag.SelectedFilter = frm["SelFilter"];
                    int SelValueSch = Convert.ToInt32(frm["SelFilter"].ToString());
                    if (frm["SelFilter"] != "" && frm["SearchString"].ToString() != "")
                    {
                        if (SelValueSch == 1)
                        { Search += " and a.roll='" + frm["SearchString"].ToString() + "'"; }
                        if (SelValueSch == 2)
                        { Search += "and d.Std_id='" + frm["SearchString"].ToString() + "'"; }
                        else if (SelValueSch == 3)
                        { Search += " and d.Regno like '%" + frm["SearchString"].ToString() + "%'"; }
                        else if (SelValueSch == 4)
                        { Search += "and d.Name like '%" + frm["SearchString"].ToString() + "%'"; }
                    }
                }

                MS.StoreAllData = new AbstractLayer.SchoolDB().GetSchoolResultDetails(Search, Session["SCHL"].ToString(), CLASS, RP);
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    Session["StudentResultCertificate"] = null;
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    Session["StudentResultCertificate"] = MS.StoreAllData.Tables[0];
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;


                    return View(MS);
                }

            }
            catch (Exception ex)
            {
                //return RedirectToAction("Index", "Login");
            }

            return View(MS);
        }




        #endregion  StudentResultCertificate 



        #region School to School Migration


        [SessionCheckFilter]
        public ActionResult ApplyStudentSchoolMigration()
        {
            string Schl = Session["SCHL"].ToString();
            MigrateSchoolModels MS = new MigrateSchoolModels();
            try
            {

                var itemFilter = new SelectList(new[]{new {ID="1",Name="Student Unique ID"},new {ID="2",Name="Reg. No"},new{ID="3",Name="Aadhar No"},
            new{ID="4",Name="Mobile No"},new{ID="5",Name="E-Punjab ID"},}, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();


                //GetAllowedGroupListBySchool
                List<SelectListItem> MyGroupList = new List<SelectListItem>();
                ViewBag.MyGroup = MyGroupList;

            }
            catch (Exception ex)
            {
            }

            return View();
        }


        [SessionCheckFilter]
        [HttpPost]
        public ActionResult ApplyStudentSchoolMigration(FormCollection frm, string SelFilter, string SearchString)
        {


            MigrateSchoolModels MS = new MigrateSchoolModels();

            string Schl = Session["SCHL"].ToString();

            var itemFilter = new SelectList(new[]{new {ID="1",Name="Student Unique ID"},new {ID="2",Name="Reg. No"},new{ID="3",Name="Aadhar No"},
            new{ID="4",Name="Mobile No"},new{ID="5",Name="E-Punjab ID"},}, "ID", "Name", 1);
            ViewBag.MyFilter = itemFilter.ToList();

            //GetAllowedGroupListBySchool
            List<SelectListItem> MyGroupList = objCommon.GetAllowedGroupListBySchool(Session["SCHL"].ToString());
            ViewBag.MyGroup = MyGroupList;


            string Search = "SCHL!='" + Schl + "' and class not in (5,8) ";
            if (string.IsNullOrEmpty(SelFilter) || string.IsNullOrEmpty(SearchString))
            {
                ViewBag.TotalCount = -1;
            }
            else
            {

                ViewBag.SelectedItem = SelFilter;
                int SelValueSch = Convert.ToInt32(SelFilter.ToString());
                if (SearchString != "")
                {
                    if (SelValueSch == 1)
                    { Search += " and std_id='" + frm["SearchString"].ToString() + "'"; }
                    else if (SelValueSch == 2)
                    { Search += " and  Registration_num ='" + frm["SearchString"].ToString() + "'"; }
                    else if (SelValueSch == 3)
                    { Search += " and Aadhar_num ='" + frm["SearchString"].ToString() + "'"; }
                    else if (SelValueSch == 4)
                    { Search += " and Mobile ='" + frm["SearchString"].ToString() + "'"; }
                    else if (SelValueSch == 5)
                    { Search += " and E_punjab_Std_id ='" + frm["SearchString"].ToString() + "'"; }
                }
            }

            MS.StoreAllData = objDB.ApplyStudentSchoolMigrationSearch(0, Search, Schl);
            if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
            }
            else
            {
                ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
            }
            return View(MS);
        }

        [HttpPost]
        public ActionResult AddStudentMigration(StudentSchoolMigrations studentSchoolMigration)
        {
            string Schl = Session["SCHL"].ToString();
            string outid = "0";
            string outStatus = "0";
            string filename = "";
            string FilepathExist = "", path = "";
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                var submitDataModel = Request.Files;

                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.                     

                        string ext = Path.GetExtension(file.FileName);
                        filename = studentSchoolMigration.StdId + "_" + studentSchoolMigration.NewSCHL + "_SchoolMigration" + ext;
                        //path = Path.Combine(Server.MapPath("~/Upload/Upload2023/SchoolMigration/StudentMigrationLetter"), filename);
                        //FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/SchoolMigration/StudentMigrationLetter"));
                        //studentSchoolMigration.StudentMigrationLetter = "Upload2023/SchoolMigration/StudentMigrationLetter/" + filename;
                        //if (!Directory.Exists(FilepathExist))
                        //{
                        //    Directory.CreateDirectory(FilepathExist);
                        //}
                        //file.SaveAs(path);
                       
                        using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                        {
                            using (var newMemoryStream = new MemoryStream())
                            {
                                var uploadRequest = new TransferUtilityUploadRequest
                                {
                                    InputStream = file.InputStream,
                                    Key = string.Format("allfiles/Upload2023/SchoolMigration/StudentMigrationLetter/{0}", filename),
                                    BucketName = BUCKET_NAME,
                                    CannedACL = S3CannedACL.PublicRead
                                };

                                var fileTransferUtility = new TransferUtility(client);
                                fileTransferUtility.Upload(uploadRequest);
                            }
                        }

                    }


                    studentSchoolMigration.MigrationId = 0;
                    //                    studentSchoolMigration.RegNo = "";
                    studentSchoolMigration.IsActive = true;
                    studentSchoolMigration.CreatedDate = DateTime.Now;
                    studentSchoolMigration.UpdatedDate = DateTime.Now;
                    studentSchoolMigration.IsDeleted = false;
                    studentSchoolMigration.userName = Schl;
                    if (studentSchoolMigration.NewSCHL.Length < 7)
                    {
                        studentSchoolMigration.NewSCHL = Schl;
                    }

                    //studentSchoolMigration.AppLevel = "SCHL";
                    studentSchoolMigration.MigrationStatusCode = 1; //Applied & Forward to Old School
                    if (studentSchoolMigration.AppLevel.ToUpper() == "SCHL")
                    {
                        studentSchoolMigration.IsAppBySchl = 1;
                        studentSchoolMigration.IsAppByHOD = 0;
                    }
                    else if (studentSchoolMigration.AppLevel.ToUpper() == "HOD")
                    {
                        studentSchoolMigration.IsAppBySchl = 0;
                        studentSchoolMigration.IsAppByHOD = 1;
                    }
                    else
                    {
                        studentSchoolMigration.IsAppBySchl = 0;
                        studentSchoolMigration.IsAppByHOD = 0;
                    }

                    studentSchoolMigration.IsCancel = 0;
                    studentSchoolMigration.UserIP = AbstractLayer.StaticDB.GetFullIPAddress();
                    _context.StudentSchoolMigrations.Add(studentSchoolMigration);
                    int insertedRecords = _context.SaveChanges();
                    if (insertedRecords > 0)
                    {
                        // Returns message that successfully uploaded  
                        outid = "1";
                        outStatus = "Successfully";
                        //DataSet ds = new DataSet();
                        //SchoolModels sm = new AbstractLayer.SchoolDB().GetSchoolDataBySchl(studentSchoolMigration.CurrentSCHL, out ds);

                        if (!string.IsNullOrEmpty(Session["SchoolMobile"].ToString()))
                        {
                            string SchoolMobile = Session["SchoolMobile"].ToString();
                            string Sms = "School to School Migration of Student " + studentSchoolMigration.StdId + " of Class is applied by new school. Check there request under School Migration -> Received List and take necessary action. Regards PSEB";
                            try
                            {
                                string getSms = objCommon.gosms(SchoolMobile, Sms);
                            }
                            catch (Exception) { }
                        }
                    }
                    else
                    {
                        outStatus = "Failure";
                        outid = "-1";
                    }

                    return Json(new { migid = outid, status = outStatus }, JsonRequestBehavior.AllowGet);
                    // return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    outid = "err";
                    outStatus = ex.Message;
                    return Json(new { migid = outid, status = outStatus }, JsonRequestBehavior.AllowGet);

                }
            }
            else
            {
                outStatus = "No files selected.";
                outid = "-1";
                return Json(new { migid = outid, status = outStatus }, JsonRequestBehavior.AllowGet);
            }
        }


        #region  Applied 
        [SessionCheckFilter]
        public ActionResult StudentSchoolMigrationApplied()
        {
            string Schl = Session["SCHL"].ToString();
            MigrateSchoolModels MS = new MigrateSchoolModels();
            try
            {

                var itemFilter = new SelectList(new[]{new {ID="1",Name="Student Unique ID"},new {ID="2",Name="Reg. No"},new{ID="3",Name="Aadhar No"},
            new{ID="4",Name="Migration Id"},new{ID="5",Name="By Name"},}, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();


                //GetAllowedGroupListBySchool
                List<SelectListItem> MyGroupList = new List<SelectListItem>();
                ViewBag.MyGroup = MyGroupList;

                string Search = "MigrationId like '%%' and class not in (5,8) ";
                MS.StoreAllData = objDB.StudentSchoolMigrationsSearch(0, Search, Schl);
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                }
                return View(MS);

            }
            catch (Exception ex)
            {
            }

            return View();
        }


        [SessionCheckFilter]
        [HttpPost]
        public ActionResult StudentSchoolMigrationApplied(FormCollection frm, string SelFilter, string SearchString)
        {


            MigrateSchoolModels MS = new MigrateSchoolModels();

            string Schl = Session["SCHL"].ToString();

            var itemFilter = new SelectList(new[]{new {ID="1",Name="Student Unique ID"},new {ID="2",Name="Reg. No"},new{ID="3",Name="Aadhar No"},
            new{ID="4",Name="Migration Id"},new{ID="5",Name="By Name"},}, "ID", "Name", 1);
            ViewBag.MyFilter = itemFilter.ToList();

            //GetAllowedGroupListBySchool
            List<SelectListItem> MyGroupList = new List<SelectListItem>();
            ViewBag.MyGroup = MyGroupList;


            string Search = "MigrationId like '%%' and class not in (5,8) ";
            if (string.IsNullOrEmpty(SelFilter) || string.IsNullOrEmpty(SearchString))
            {
                ViewBag.TotalCount = -1;
            }
            else
            {

                ViewBag.SelectedItem = SelFilter;
                int SelValueSch = Convert.ToInt32(SelFilter.ToString());
                if (SearchString != "")
                {
                    if (SelValueSch == 1)
                    { Search += " and StdId='" + frm["SearchString"].ToString() + "'"; }
                    else if (SelValueSch == 2)
                    { Search += " and  Registration_num ='" + frm["SearchString"].ToString() + "'"; }
                    else if (SelValueSch == 3)
                    { Search += " and Aadhar_num ='" + frm["SearchString"].ToString() + "'"; }
                    else if (SelValueSch == 4)
                    { Search += " and MigrationId ='" + frm["SearchString"].ToString() + "'"; }
                    else if (SelValueSch == 5)
                    { Search += " and Name like '%" + frm["SearchString"].ToString() + "%'"; }
                }
            }

            MS.StoreAllData = objDB.StudentSchoolMigrationsSearch(0, Search, Schl);
            if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
            }
            else
            {
                ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
            }
            return View(MS);
        }


        #endregion

        #region Received


        [SessionCheckFilter]
        public ActionResult StudentSchoolMigrationReceived()
        {
            string Schl = Session["SCHL"].ToString();
            MigrateSchoolModels MS = new MigrateSchoolModels();
            try
            {

                var itemFilter = new SelectList(new[]{new {ID="1",Name="Student Unique ID"},new {ID="2",Name="Reg. No"},new{ID="3",Name="Aadhar No"},
            new{ID="4",Name="Migration Id"},new{ID="5",Name="By Name"},}, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();


                //GetAllowedGroupListBySchool
                List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDL();
                ViewBag.MyGroup = MyAcceptRejectList;

                string Search = "MigrationId like '%%' and class not in (5,8) ";
                MS.StoreAllData = objDB.StudentSchoolMigrationsSearch(1, Search, Schl); // type=1 for receieved
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                }
                return View(MS);

            }
            catch (Exception ex)
            {
            }

            return View();
        }


        [SessionCheckFilter]
        [HttpPost]
        public ActionResult StudentSchoolMigrationReceived(FormCollection frm, string SelFilter, string SearchString)
        {


            MigrateSchoolModels MS = new MigrateSchoolModels();

            string Schl = Session["SCHL"].ToString();

            var itemFilter = new SelectList(new[]{new {ID="1",Name="Student Unique ID"},new {ID="2",Name="Reg. No"},new{ID="3",Name="Aadhar No"},
            new{ID="4",Name="Migration Id"},new{ID="5",Name="By Name"},}, "ID", "Name", 1);
            ViewBag.MyFilter = itemFilter.ToList();

            //GetAllowedGroupListBySchool
            List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDL();
            ViewBag.MyGroup = MyAcceptRejectList;


            string Search = "MigrationId like '%%' and class not in (5,8) ";
            if (string.IsNullOrEmpty(SelFilter) || string.IsNullOrEmpty(SearchString))
            {
                ViewBag.TotalCount = -1;
            }
            else
            {

                ViewBag.SelectedItem = SelFilter;
                int SelValueSch = Convert.ToInt32(SelFilter.ToString());
                if (SearchString != "")
                {
                    if (SelValueSch == 1)
                    { Search += " and StdId='" + frm["SearchString"].ToString() + "'"; }
                    else if (SelValueSch == 2)
                    { Search += " and  Registration_num ='" + frm["SearchString"].ToString() + "'"; }
                    else if (SelValueSch == 3)
                    { Search += " and Aadhar_num ='" + frm["SearchString"].ToString() + "'"; }
                    else if (SelValueSch == 4)
                    { Search += " and MigrationId ='" + frm["SearchString"].ToString() + "'"; }
                    else if (SelValueSch == 5)
                    { Search += " and Name like '%" + frm["SearchString"].ToString() + "%'"; }
                }
            }

            MS.StoreAllData = objDB.StudentSchoolMigrationsSearch(1, Search, Schl); // type=1 for receieved
            if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
            }
            else
            {
                ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
            }
            return View(MS);
        }

        #endregion



        #region Challan and Payment Details 

        [SessionCheckFilter]
        public ActionResult StudentMigrationPayFee(string id)
        {
            string Schl = Session["SCHL"].ToString();
            StudentSchoolMigrationViewModel studentSchoolMigrationViewModel = new StudentSchoolMigrationViewModel();
            EAffiliationFee _EAffiliationFee = new EAffiliationFee();

            try
            {
                string Search = "MigrationId =" + id;
                List<StudentSchoolMigrationViewModel> studentSchoolMigrationViewModelList = objDB.StudentSchoolMigrationsSearchModel(2, Search, Schl); // type=1 for receieved
                if (studentSchoolMigrationViewModelList.Count() > 0)
                {
                    ViewBag.TotalCount = studentSchoolMigrationViewModelList.Count;
                    studentSchoolMigrationViewModel = studentSchoolMigrationViewModelList.Where(s => s.MigrationId == Convert.ToInt32(id)).FirstOrDefault();

                    //

                    DataSet ds = AbstractLayer.SchoolDB.GetStudentSchoolMigrationsPayment(studentSchoolMigrationViewModel.MigrationId, studentSchoolMigrationViewModel.StdId);
                    _EAffiliationFee.PaymentFormData = ds;
                    if (_EAffiliationFee.PaymentFormData == null || _EAffiliationFee.PaymentFormData.Tables[0].Rows.Count == 0)
                    { ViewBag.TotalCount = 0; Session["StudentSchoolMigrationFee"] = null; }
                    else
                    {
                        Session["StudentSchoolMigrationFee"] = ds;
                        ViewBag.TotalFee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("fee")));
                        ViewBag.TotalLateFee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("latefee")));
                        ViewBag.TotalTotfee = Convert.ToInt32(ds.Tables[0].AsEnumerable().Sum(x => x.Field<int>("Totfee")));

                        ViewBag.Total = ViewBag.TotalFee + ViewBag.TotalLateFee;
                        ViewData["result"] = 10;
                        ViewData["FeeStatus"] = "1";
                        ViewBag.TotalCount = 1;
                        return View(_EAffiliationFee);
                    }
                }
                else
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }

            }
            catch (Exception ex)
            {
            }


            return View(_EAffiliationFee);
        }



        [HttpPost]
        [SessionCheckFilter]
        public ActionResult StudentMigrationPayFee(string id, FormCollection frm, string PayModValue, string AllowBanks)
        {
            string Schl = Session["SCHL"].ToString();
            try
            {

                EAffiliationFee pfvm = new EAffiliationFee();
                ChallanMasterModel CM = new ChallanMasterModel();
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("Index", "EAffiliation");
                }
                if (Session["StudentSchoolMigrationFee"] == null)
                {
                    return RedirectToAction("CalculateFee", "EAffiliation");
                }
                string appno = id;
                DataSet ds = (DataSet)Session["StudentSchoolMigrationFee"];
                pfvm.PaymentFormData = ds;

                string bankName = "";

                if (AllowBanks == "301" || AllowBanks == "302")
                {
                    PayModValue = "online";
                    CM.FEEMODE = "ONLINE";
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
                    CM.FEEMODE = "CASH";
                    PayModValue = "hod";
                    bankName = "PSEB HOD";
                }
                else if (AllowBanks == "202" || AllowBanks == "204")
                {
                    CM.FEEMODE = "CASH";
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

                string stdid = ds.Tables[0].Rows[0]["StdId"].ToString();

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
                    CM.BCODE = AllowBanks;
                    CM.BANK = bankName;
                    CM.BANKCHRG = 0;
                    CM.SchoolCode = ds.Tables[0].Rows[0]["StdId"].ToString();
                    CM.DIST = "";
                    CM.DISTNM = "";
                    CM.LOT = 1;
                    CM.SCHLREGID = Schl;
                    CM.FeeStudentList = ds.Tables[0].Rows[0]["MigrationId"].ToString();
                    CM.APPNO = ds.Tables[0].Rows[0]["MigrationId"].ToString();

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

                            //string Sms = "Challan no. " + result + " of Ref no  " + CM.APPNO + " successfully generated and valid till Dt " + bnkLastDate + ". Regards PSEB";
                            //try
                            //{
                            //    string getSms = objCommon.gosms(SchoolMobile, Sms);
                            //}
                            //catch (Exception) { }
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




        public ActionResult StudentMigrationView(string id)
        {
            // string Schl = Session["SCHL"].ToString();          
            StudentSchoolMigrationViewModel studentSchoolMigrationViewModel = new StudentSchoolMigrationViewModel();
            try
            {
                string Search = "MigrationId =" + id;
                List<StudentSchoolMigrationViewModel> studentSchoolMigrationViewModelList = objDB.StudentSchoolMigrationsSearchModel(2, Search, ""); // type=2 for search any
                if (studentSchoolMigrationViewModelList.Count() > 0)
                {
                    ViewBag.TotalCount = studentSchoolMigrationViewModelList.Count;
                    studentSchoolMigrationViewModel = studentSchoolMigrationViewModelList.Where(s => s.MigrationId == Convert.ToInt32(id)).FirstOrDefault();
                }
                else
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }

            }
            catch (Exception ex)
            {
            }

            return View(studentSchoolMigrationViewModel);
        }



        public ActionResult StudentMigrationPrintCertificate(string id)
        {
            // string Schl = Session["SCHL"].ToString();
            StudentSchoolMigrationViewModel studentSchoolMigrationViewModel = new StudentSchoolMigrationViewModel();
            try
            {
                string Search = "MigrationId =" + id;
                List<StudentSchoolMigrationViewModel> studentSchoolMigrationViewModelList = objDB.StudentSchoolMigrationsSearchModel(2, Search, ""); // type=2 for any
                if (studentSchoolMigrationViewModelList.Count() > 0)
                {
                    ViewBag.TotalCount = studentSchoolMigrationViewModelList.Count;
                    studentSchoolMigrationViewModel = studentSchoolMigrationViewModelList.Where(s => s.MigrationId == Convert.ToInt32(id)).FirstOrDefault();
                }
                else
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }

            }
            catch (Exception ex)
            {
            }

            return View(studentSchoolMigrationViewModel);
        }


        #endregion



        #region Begin PreBoardExamTheory
        public ActionResult PreBoardExamTheory_Portal()
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                string SchlID = Convert.ToString(Session["SCHL"]);
                DataSet result = objCommon.schooltypes(SchlID); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {
                    ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                    ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                }
                TempData["PreBoardExamTheorySearch"] = null;

                return View();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        public ActionResult PreBoardExamTheory(FormCollection frm, string id, int? page)
        {
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
                    CLASS = "4";
                    ViewBag.ClassName = "SENIOR SECONDARY";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "2";
                    ViewBag.ClassName = "MATRIC";
                }

                if (SCHL == "" || SCHL == null)
                { SCHL = Session["SCHL"].ToString(); }

                ViewBag.schlCode = SCHL;
                ViewBag.cid = id;

                #region  Check School Allow For CCE,PreBoard

                DataSet dsAllow = objDB.SchoolAllowForCCE(SCHL, CLASS, "PREBOARD");
                if (dsAllow.Tables.Count > 0)
                {
                    if (dsAllow.Tables[1].Rows.Count > 0) // table 1 for pre board
                    {
                        ViewBag.IsActive = dsAllow.Tables[1].Rows[0]["IsActive"].ToString();
                        ViewBag.IsAllow = dsAllow.Tables[1].Rows[0]["IsAllow"].ToString();
                        ViewBag.LastDate = dsAllow.Tables[1].Rows[0]["LastDate"].ToString();
                        ViewBag.LastDateDT = dsAllow.Tables[1].Rows[0]["LastDateDT"].ToString();
                    }
                }

                #endregion  Check School Allow For CCE,PreBoard


                var itemFilter = new SelectList(new[] { new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                    new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
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
                    if (TempData["PreBoardExamTheorySearch"] != null)
                    {
                        Search += TempData["PreBoardExamTheorySearch"].ToString();
                        ViewBag.SelectedFilter = TempData["SelFilter"];
                        SelectedAction = TempData["SelAction"].ToString();
                        ViewBag.SelectedAction = TempData["SelActionValue"];

                        TempData["SelFilter"] = ViewBag.SelectedFilter;
                        TempData["SelAction"] = SelectedAction;
                        TempData["SelActionValue"] = ViewBag.SelectedAction;
                        TempData["PreBoardExamTheorySearch"] = Search;
                    }
                    else
                    {
                        Search = "  a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "'";
                    }


                    //string class1 = "4"; // For Senior
                    MS.StoreAllData = objDB.GetPreBoardExamTheoryBySCHL(Search, SCHL, pageIndex, CLASS, Convert.ToInt32(SelectedAction));
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsPreBoardMarksFilled"]);
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
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsPreBoardMarksFilled"]);
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
        public ActionResult PreBoardExamTheory(FormCollection frm, int? page)
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
                    CLASS = "4";
                    ViewBag.ClassName = "SENIOR SECONDARY";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "2";
                    ViewBag.ClassName = "MATRIC";
                }

                #region  Check School Allow For CCE,PreBoard

                DataSet dsAllow = objDB.SchoolAllowForCCE(SCHL, CLASS, "PREBOARD");
                if (dsAllow.Tables.Count > 0)
                {
                    if (dsAllow.Tables[1].Rows.Count > 0) // table 1 for pre board
                    {
                        ViewBag.IsActive = dsAllow.Tables[1].Rows[0]["IsActive"].ToString();
                        ViewBag.IsAllow = dsAllow.Tables[1].Rows[0]["IsAllow"].ToString();
                        ViewBag.LastDate = dsAllow.Tables[1].Rows[0]["LastDate"].ToString();
                        ViewBag.LastDateDT = dsAllow.Tables[1].Rows[0]["LastDateDT"].ToString();
                    }
                }

                #endregion  Check School Allow For CCE,PreBoard

                var itemFilter = new SelectList(new[] { new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                    new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
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
                            if (SelValueSch == 3)
                            {
                                SelAction = 2;
                            }
                            //  { Search += " and  IsMarksFilled=1 "; } // Filled
                            else if (SelValueSch == 2)
                            { SelAction = 1; }
                            //{ Search += " and (IsMarksFilled is null or IsMarksFilled=0) "; } // pending
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
                            { Search += " and b.Roll='" + frm["SearchString"].ToString() + "'"; }
                            if (SelValueSch == 2)
                            { Search += " and Std_id='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 3)
                            { Search += " and  Registration_num like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " and  Candi_Name like '%" + frm["SearchString"].ToString() + "%'"; }
                        }
                    }

                    TempData["SelFilter"] = frm["SelFilter"];
                    TempData["SelAction"] = SelAction;
                    TempData["SelActionValue"] = frm["SelAction"];
                    TempData["PreBoardExamTheorySearch"] = Search;
                    // string class1 = "4";
                    MS.StoreAllData = objDB.GetPreBoardExamTheoryBySCHL(Search, SCHL, pageIndex, CLASS, SelAction);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsPreBoardMarksFilled"]);
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
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsPreBoardMarksFilled"]);
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
        public JsonResult JqPreBoardExamTheory(string stdid, string CandSubject)
        {
            var flag = 1;

            // CandSubject myObj = JsonConvert.DeserializeObject<CandSubject>(CandSubject);
            var objResponse1 = JsonConvert.DeserializeObject<List<CandSubjectPreBoard>>(CandSubject);
            DataTable dtSub = new DataTable();
            dtSub.Columns.Add("CANDID");
            dtSub.Columns.Add("SUB");
            dtSub.Columns.Add("OBTMARKS");
            dtSub.Columns.Add("MINMARKS");
            dtSub.Columns.Add("MAXMARKS");
            dtSub.Columns.Add("PROBTMARKS");
            dtSub.Columns.Add("PRMINMARKS");
            dtSub.Columns.Add("PRMAXMARKS");
            dtSub.Columns.Add("INOBTMARKS");
            dtSub.Columns.Add("INMINMARKS");
            dtSub.Columns.Add("INMAXMARKS");
            DataRow row = null;
            foreach (var rowObj in objResponse1)
            {
                row = dtSub.NewRow();
                //OBTMARKS
                if (rowObj.OBTMARKS == "A" || rowObj.OBTMARKS == "ABS")
                {
                    rowObj.OBTMARKS = "ABS";
                }
                else if (rowObj.OBTMARKS == "C" || rowObj.OBTMARKS == "CAN")
                {
                    rowObj.OBTMARKS = "CAN";
                }
                else if (string.IsNullOrEmpty(rowObj.OBTMARKS))
                {
                    rowObj.OBTMARKS = "";
                }
                else if (rowObj.OBTMARKS != "")
                {
                    rowObj.OBTMARKS = rowObj.OBTMARKS.PadLeft(3, '0');
                }

                //PROBTMARKS
                if (rowObj.PROBTMARKS == "A" || rowObj.PROBTMARKS == "ABS")
                {
                    rowObj.PROBTMARKS = "ABS";
                }
                else if (rowObj.PROBTMARKS == "C" || rowObj.PROBTMARKS == "CAN")
                {
                    rowObj.PROBTMARKS = "CAN";
                }
                else if (string.IsNullOrEmpty(rowObj.PROBTMARKS))
                {
                    rowObj.PROBTMARKS = "";
                }
                else if (rowObj.PROBTMARKS != "")
                {
                    rowObj.PROBTMARKS = rowObj.PROBTMARKS.PadLeft(3, '0');
                }

                //INOBTMARKS
                if (rowObj.INOBTMARKS == "A" || rowObj.INOBTMARKS == "ABS")
                {
                    rowObj.INOBTMARKS = "ABS";
                }
                else if (rowObj.INOBTMARKS == "C" || rowObj.INOBTMARKS == "CAN")
                {
                    rowObj.INOBTMARKS = "CAN";
                }
                else if (string.IsNullOrEmpty(rowObj.INOBTMARKS))
                {
                    rowObj.INOBTMARKS = "";
                }
                else if (rowObj.INOBTMARKS != "")
                {
                    rowObj.INOBTMARKS = rowObj.INOBTMARKS.PadLeft(3, '0');
                }

                if (string.IsNullOrEmpty(rowObj.PRMINMARKS) || rowObj.PRMINMARKS == "--")
                {
                    rowObj.PRMINMARKS = "000";
                }
                if (string.IsNullOrEmpty(rowObj.PRMAXMARKS) || rowObj.PRMAXMARKS == "--")
                {
                    rowObj.PRMAXMARKS = "000";
                }
                if (string.IsNullOrEmpty(rowObj.INMINMARKS) || rowObj.INMINMARKS == "--")
                {
                    rowObj.INMINMARKS = "000";
                }
                if (string.IsNullOrEmpty(rowObj.INMAXMARKS) || rowObj.INMAXMARKS == "--")
                {
                    rowObj.INMAXMARKS = "000";
                }

                if (!string.IsNullOrEmpty(rowObj.SUB))
                {
                    dtSub.Rows.Add(stdid, rowObj.SUB, rowObj.OBTMARKS, rowObj.MINMARKS, rowObj.MAXMARKS,
                         rowObj.PROBTMARKS, rowObj.PRMINMARKS, rowObj.PRMAXMARKS,
                          rowObj.INOBTMARKS, rowObj.INMINMARKS, rowObj.INMAXMARKS);
                }
            }
            dtSub.AcceptChanges();
            //dtSub =  AbstractLayer.StaticDB.RemoveEmptyAndDuplicateRowFromDataTable(dtSub, "SUB");
            // dtSub.AcceptChanges();
            foreach (DataRow dr1 in dtSub.Rows)
            {
                if (dr1["OBTMARKS"].ToString() == "" || dr1["OBTMARKS"].ToString() == "ABS" || dr1["OBTMARKS"].ToString() == "CAN")
                { }
                else if (dr1["OBTMARKS"].ToString().Contains("A") || dr1["OBTMARKS"].ToString().Contains("C"))
                {
                    flag = -2;
                    var results = new
                    {
                        status = flag
                    };
                    return Json(results);
                }
                else if (dr1["PROBTMARKS"].ToString().Contains("A") || dr1["PROBTMARKS"].ToString().Contains("C"))
                {
                    flag = -2;
                    var results = new
                    {
                        status = flag
                    };
                    return Json(results);
                }
                else if (dr1["INOBTMARKS"].ToString().Contains("A") || dr1["INOBTMARKS"].ToString().Contains("C"))
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
                    string SUB = dr1["SUB"].ToString();
                    if (!string.IsNullOrEmpty(SUB))
                    {
                        string OBTMARKS = dr1["OBTMARKS"].ToString() == "" ? "000" : dr1["OBTMARKS"].ToString();
                        string MINMARKS = dr1["MINMARKS"].ToString() == "" ? "000" : dr1["MINMARKS"].ToString();
                        string MAXMARKS = dr1["MAXMARKS"].ToString() == "" ? "000" : dr1["MAXMARKS"].ToString();


                        string PROBTMARKS = dr1["PROBTMARKS"].ToString() == "" ? "000" : dr1["PROBTMARKS"].ToString();
                        string PRMINMARKS = dr1["PRMINMARKS"].ToString() == "" ? "000" : dr1["PRMINMARKS"].ToString();
                        string PRMAXMARKS = dr1["PRMAXMARKS"].ToString() == "" ? "000" : dr1["PRMAXMARKS"].ToString();

                        string INOBTMARKS = dr1["INOBTMARKS"].ToString() == "" ? "000" : dr1["INOBTMARKS"].ToString();
                        string INMINMARKS = dr1["INMINMARKS"].ToString() == "" ? "000" : dr1["INMINMARKS"].ToString();
                        string INMAXMARKS = dr1["INMAXMARKS"].ToString() == "" ? "000" : dr1["INMAXMARKS"].ToString();


                        int obt = Convert.ToInt32(OBTMARKS);
                        int min = Convert.ToInt32(MINMARKS);
                        int max = Convert.ToInt32(MAXMARKS);

                        if ((obt < 0) || (obt > max))
                        {
                            flag = -2;
                        }

                        int PRobt = Convert.ToInt32(PROBTMARKS);
                        int PRmin = Convert.ToInt32(PRMINMARKS);
                        int PRmax = Convert.ToInt32(PRMAXMARKS);

                        if ((PRobt < 0) || (PRobt > PRmax))
                        {
                            flag = -2;
                        }


                        int INobt = Convert.ToInt32(INOBTMARKS);
                        int INmin = Convert.ToInt32(INMINMARKS);
                        int INmax = Convert.ToInt32(INMAXMARKS);

                        if ((INobt < 0) || (INobt > INmax))
                        {
                            flag = -2;
                        }
                    }
                }
            }
            if (flag == 1)
            {
                string dee = "1";
                string class1 = "4";
                int OutStatus = 0;



                dee = objDB.AllotPreBoardExamTheory(stdid, dtSub, class1, out OutStatus);

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

        public ActionResult PreBoardExamTheoryReport(string id)
        {
            TempData["PreBoardExamTheorySearch"] = null;
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
                    CLASS = "4";
                    ViewBag.ClassName = "SENIOR";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "2";
                    ViewBag.ClassName = "MATRIC";
                }

                if (SCHL == "" || SCHL == null)
                {
                    SCHL = Session["SCHL"].ToString();
                }

                #region  Check School Allow For CCE,PreBoard

                DataSet dsAllow = objDB.SchoolAllowForCCE(SCHL, CLASS, "PREBOARD");
                if (dsAllow.Tables.Count > 0)
                {
                    if (dsAllow.Tables[1].Rows.Count > 0) // table 1 for pre board
                    {
                        ViewBag.IsActive = dsAllow.Tables[1].Rows[0]["IsActive"].ToString();
                        ViewBag.IsAllow = dsAllow.Tables[1].Rows[0]["IsAllow"].ToString();
                        ViewBag.LastDate = dsAllow.Tables[1].Rows[0]["LastDate"].ToString();
                        ViewBag.LastDateDT = dsAllow.Tables[1].Rows[0]["LastDateDT"].ToString();
                    }
                }

                #endregion  Check School Allow For CCE,PreBoard
                ViewBag.schlCode = SCHL;
                ViewBag.cid = id;
                string Search = string.Empty;
                Search = "  a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "'";
                MS.StoreAllData = objDB.PreBoardExamTheoryREPORT(Search, SCHL, CLASS);
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


        public ActionResult PreBoardExamTheoryFinalReport(string id, FormCollection frm)
        {
            TempData["PreBoardExamTheorySearch"] = null;
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
                    CLASS = "4";
                    ViewBag.ClassName = "SENIOR";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "2";
                    ViewBag.ClassName = "MATRIC";
                }

                if (SCHL == "" || SCHL == null)
                {
                    SCHL = Session["SCHL"].ToString();
                }

                #region  Check School Allow For CCE,PreBoard

                DataSet dsAllow = objDB.SchoolAllowForCCE(SCHL, CLASS, "PREBOARD");
                if (dsAllow.Tables.Count > 0)
                {
                    if (dsAllow.Tables[1].Rows.Count > 0) // table 1 for pre board
                    {
                        ViewBag.IsActive = dsAllow.Tables[1].Rows[0]["IsActive"].ToString();
                        ViewBag.IsAllow = dsAllow.Tables[1].Rows[0]["IsAllow"].ToString();
                        ViewBag.LastDate = dsAllow.Tables[1].Rows[0]["LastDate"].ToString();
                        ViewBag.LastDateDT = dsAllow.Tables[1].Rows[0]["LastDateDT"].ToString();
                    }
                }

                #endregion  Check School Allow For CCE,PreBoard

                ViewBag.schlCode = SCHL;
                ViewBag.cid = id;
                string Search = string.Empty;
                Search = "  a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "'";
                DataSet dsFinal = new DataSet();
                MS.StoreAllData = dsFinal = objDB.PreBoardExamTheoryFinalReport(Search, SCHL, CLASS);
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
                    MS.StoreAllData = objDB.PreBoardExamTheoryREPORT(Search, SCHL, CLASS);
                    ViewBag.TotalCount = 0;
                    ViewBag.TotalCount1 = MS.StoreAllData.Tables[0].Rows.Count;
                    if (MS.StoreAllData.Tables[0].Rows[0]["PreBoardDate"].ToString() == "")
                    {
                        ViewBag.PreBoardDate = "15/01/2021";
                        //if (CLASS == "4")
                        //{
                        //    ViewBag.PreBoardDate = "15/03/2017";
                        //}
                        //else if (CLASS == "2")
                        //{ ViewBag.CCEDate = "18/03/2017"; }
                    }
                    else
                    {
                        ViewBag.PreBoardDate = MS.StoreAllData.Tables[0].Rows[0]["PreBoardDate"].ToString();
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
        public ActionResult PreBoardExamTheoryFinalReport(string id)
        {
            TempData["PreBoardExamTheorySearch"] = null;
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
                    CLASS = "4";
                    ViewBag.ClassName = "SENIOR";
                }
                else if (id == "M") // For MAtric
                {
                    CLASS = "2";
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
                MS.StoreAllData = objDB.PreBoardExamTheoryREPORTFinalSubmit(Search, SCHL, CLASS);  //CCEREPORTFinalSubmit
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
                    if (MS.StoreAllData.Tables[0].Rows[0]["PreBoardDate"].ToString() == "")
                    {
                        ViewBag.PreBoardDate = "15/01/2021";
                        //if (CLASS == "4")
                        //{
                        //    ViewBag.CCEDate = "15/03/2017";
                        //}
                        //else if (CLASS == "2")
                        //{ ViewBag.CCEDate = "18/03/2017"; }
                    }
                    else
                    {
                        ViewBag.PreBoardDate = MS.StoreAllData.Tables[0].Rows[0]["PreBoardDate"].ToString();
                    }
                    ViewBag.SchoolName = MS.StoreAllData.Tables[1].Rows[0]["FullSchoolNameE"].ToString();
                    //ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["IsMarksFilled"]);
                    //if (MS.StoreAllData.Tables[2].Rows.Count > 0)
                    //{
                    //    int totalFinalPending = Convert.ToInt32(MS.StoreAllData.Tables[2].Rows[0]["TotalPending"]);
                    //    if (totalFinalPending == 0)
                    //    {
                    //        ViewBag.IsFinal = 0;
                    //    }
                    //    else { ViewBag.IsFinal = 1; }
                    //}
                    if (CLASS == "2")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["MSET"].ToString(); }
                    else if (CLASS == "4")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["SSET"].ToString(); }
                    else if (CLASS == "10")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["MOSET"].ToString(); }
                    else if (CLASS == "12")
                    { ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["SOSET"].ToString(); }

                    return View(MS);
                    //  return RedirectToAction("CCEFinalReport", "School", new { id= id});
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }



        public ActionResult PreBoardExamTheoryAgree(string id)
        {
            try
            {

                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                TempData["PreBoardExamTheoryClass"] = id;

            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
            }

            return View();
        }

        [HttpPost]
        public ActionResult PreBoardExamTheoryAgree(FormCollection frm, string Agree)
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                string s = frm["Agree"].ToString();
                if (TempData["PreBoardExamTheoryClass"] == null)
                {
                    return RedirectToAction("PreBoardExamTheory_Portal", "School");
                }
                else
                {
                    string CCEClass1 = TempData["PreBoardExamTheoryClass"].ToString();
                    ViewBag.CCEClass = CCEClass1;
                    if (s == "Agree")
                    {
                        if (ViewBag.CCEClass == "S" || ViewBag.CCEClass == "M")
                        { return RedirectToAction("PreBoardExamTheory", "School", new { id = CCEClass1 }); }


                    }
                    else
                    {
                        return RedirectToAction("PreBoardExamTheory_Portal", "School");
                    }
                }
                return RedirectToAction("PreBoardExamTheory_Portal", "School");
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }
        }

        #endregion  End PreBoardExamTheory


        #region MagazineSchoolRequirements
        [SessionCheckFilter]
        [HttpGet]
        public ActionResult MagazineSchoolRequirements(MagazineSchoolRequirements magazineSchoolRequirements)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];

            magazineSchoolRequirements.Schl = loginSession.SCHL;
            string usertype = Session["SchType"].ToString();

            //Pay Current Year Fee
            var itemsch = new SelectList(new[] { new { ID = "1", Name = "Pay Current Year Fee" }, new { ID = "2", Name = "Current Year Fee Paid Offline" }, }, "ID", "Name", 1);

            //

            //ADD,GOV,MRS,R&A
            if ((new[] { "ADD", "GOV", "MRS", "R&A" }).Contains(Session["SchType"].ToString().ToUpper(), StringComparer.OrdinalIgnoreCase))
            {
                magazineSchoolRequirements.FixedMagazine = 2;
            }
            else
            {
                magazineSchoolRequirements.FixedMagazine = 4;
            }
            //
            magazineSchoolRequirements.ExtraRate = magazineSchoolRequirements.FixedRate = 20; // fixrate
            magazineSchoolRequirements.ExtraMonth = magazineSchoolRequirements.FixedMonth = 12;


            // check any challan verified
            magazineSchoolRequirements.MagazineSchoolRequirementsList = _context.MagazineSchoolRequirementsChallanViews.AsNoTracking().Where(s => s.Schl == magazineSchoolRequirements.Schl).ToList();

            if (magazineSchoolRequirements.MagazineSchoolRequirementsList.Where(s => s.ChallanShowStatus == "1").Count() > 0)
            {
                magazineSchoolRequirements.FixedMagazine = 0;
                magazineSchoolRequirements.FixedTotal = 0;

                ViewBag.MySch = itemsch.ToList().Where(s => s.Value == "1").ToList();
                ViewBag.SelectedItem = "0";
            }
            else
            {
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";
                magazineSchoolRequirements.FixedTotal = (magazineSchoolRequirements.FixedMagazine * magazineSchoolRequirements.FixedRate * magazineSchoolRequirements.FixedMonth);
            }

            magazineSchoolRequirements.TotalAmount = magazineSchoolRequirements.FixedTotal;
            return View(magazineSchoolRequirements);
        }


        [SessionCheckFilter]
        public async Task<ActionResult> MagazineSchoolRequirements(MagazineSchoolRequirements magazineSchoolRequirements, HttpPostedFileBase ReceiptScannedCopy, string submit, string OldDepositDate, string OldAmount, string PayModValue, string AllowBanks, FormCollection fc)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            //loginSession.SCHL = Session["Schl"].ToString();
            //loginSession.CurrentSession = "2021-2022";

            ViewBag.SCHL = loginSession.SCHL;
            try
            {

                //Pay Current Year Fee
                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Pay Current Year Fee" }, new { ID = "2", Name = "Current Year Fee Paid Offline" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";
                //

                AffiliationFee _affiliationFee = new AffiliationFee();
                _affiliationFee.ChallanCategory = Convert.ToInt32(magazineSchoolRequirements.ChallanCategory);
                if (submit != null)
                {


                    magazineSchoolRequirements.Schl = loginSession.SCHL;
                    magazineSchoolRequirements.FY = loginSession.CurrentSession;
                    //magazineSchoolRequirements.MagazineSchoolRequirementsList = _context.MagazineSchoolRequirementsChallanViews.AsNoTracking().Where(s => s.Schl == magazineSchoolRequirements.Schl).ToList();
                    magazineSchoolRequirements.MagazineSchoolRequirementsList = _context.MagazineSchoolRequirementsChallanViews.AsNoTracking().Where(s => s.Schl == magazineSchoolRequirements.Schl).ToList();

                    if (magazineSchoolRequirements.MagazineSchoolRequirementsList.Where(s => s.ChallanShowStatus == "1").Count() > 0)
                    {
                        magazineSchoolRequirements.FixedMagazine = 0;
                        magazineSchoolRequirements.FixedTotal = 0;
                        ViewBag.MySch = itemsch.ToList().Where(s => s.Value == "1").ToList();

                    }
                    else
                    {
                        ViewBag.MySch = itemsch.ToList();
                        magazineSchoolRequirements.FixedTotal = (magazineSchoolRequirements.FixedMagazine * magazineSchoolRequirements.FixedRate * magazineSchoolRequirements.FixedMonth);
                    }



                    int NOS_Entry = _context.MagazineSchoolRequirements.Where(s => s.Schl == magazineSchoolRequirements.Schl).Count();
                    magazineSchoolRequirements.RefNo = "MZ" + magazineSchoolRequirements.Schl + string.Format("{0:D3}", NOS_Entry + 1);

                    magazineSchoolRequirements.ExtraTotal = (magazineSchoolRequirements.ExtraMagazine * magazineSchoolRequirements.ExtraRate * magazineSchoolRequirements.ExtraMonth);
                    magazineSchoolRequirements.TotalAmount = magazineSchoolRequirements.FixedTotal + magazineSchoolRequirements.ExtraTotal;

                    int AmountToBePaid = magazineSchoolRequirements.TotalAmount;
                    ViewBag.AmountToBePaid = AmountToBePaid;
                    // ViewBag.Total = AmountToBePaid;

                    if (submit.ToLower() == "submit")
                    {
                        ViewData["ChallanCategory"] = _affiliationFee.ChallanCategory;
                        return View(magazineSchoolRequirements);
                    }
                    else
                    {

                        if (_affiliationFee.ChallanCategory == 1)
                        {

                            ViewData["ChallanCategory"] = _affiliationFee.ChallanCategory;
                            if (magazineSchoolRequirements.MagazineId == 0 && (submit.ToLower().Contains("final") || submit.ToLower().Contains("online")))
                            {
                                if (string.IsNullOrEmpty(AllowBanks))
                                {
                                    ViewBag.Message = "Please Select Bank";
                                    ViewData["SelectBank"] = "1";
                                    return View(magazineSchoolRequirements);
                                }

                                MagazineSchoolRequirements magazineSchool = new MagazineSchoolRequirements()
                                {
                                    MagazineId = 0,
                                    Schl = magazineSchoolRequirements.Schl,
                                    RefNo = magazineSchoolRequirements.RefNo,
                                    FY = magazineSchoolRequirements.FY,
                                    FixedMagazine = magazineSchoolRequirements.FixedMagazine,
                                    FixedRate = magazineSchoolRequirements.FixedRate,
                                    FixedMonth = magazineSchoolRequirements.FixedMonth,
                                    FixedTotal = magazineSchoolRequirements.FixedTotal,
                                    ExtraMagazine = magazineSchoolRequirements.ExtraMagazine,
                                    ExtraRate = magazineSchoolRequirements.ExtraRate,
                                    ExtraMonth = magazineSchoolRequirements.ExtraMonth,
                                    ExtraTotal = magazineSchoolRequirements.ExtraTotal,
                                    TotalAmount = magazineSchoolRequirements.TotalAmount,
                                    //ChallanId = "",
                                    //ChallanDT ="",
                                    //ChallanVerify = "",
                                    //ChallanPaidOn = "",
                                    SubmitOn = DateTime.Now,
                                    ModifyOn = DateTime.Now,
                                    IsActive = true,
                                    ChallanCategory = "1",
                                };

                                _context.MagazineSchoolRequirements.Add(magazineSchool);
                                int insertedRecords = await _context.SaveChangesAsync();
                                // _context?.Dispose();

                                if (insertedRecords > 0)
                                {
                                    TempData["resultIns"] = "S";

                                    #region Payment Gateway Integration



                                    ChallanMasterModel CM = new ChallanMasterModel();
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


                                    CM.FEE = magazineSchoolRequirements.TotalAmount;
                                    CM.latefee = 0;
                                    CM.TOTFEE = magazineSchoolRequirements.TotalAmount;
                                    string TotfeePG = (CM.TOTFEE).ToString();
                                    CM.FEECAT = "MagazineSchoolFee";
                                    CM.FEECODE = "72";
                                    CM.FEEMODE = "ONLINE";
                                    CM.BCODE = AllowBanks;
                                    CM.BANK = bankName;
                                    CM.BANKCHRG = 0;
                                    CM.SchoolCode = magazineSchoolRequirements.Schl;
                                    CM.DIST = "";
                                    CM.DISTNM = "";
                                    CM.LOT = NOS_Entry + 1;
                                    CM.SCHLREGID = magazineSchoolRequirements.Schl;
                                    CM.FeeStudentList = magazineSchoolRequirements.RefNo;
                                    CM.APPNO = magazineSchoolRequirements.RefNo;
                                    CM.type = "schle";
                                    //
                                    CM.CHLNVDATE = DateTime.Now.ToString("dd/MM/yyyy");
                                    DateTime BankLastDate = DateTime.Now;
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
                                    string result = new AbstractLayer.HomeDB().InsertPaymentForm(CM, fc, out SchoolMobile);
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
                                        string bnkLastDate = DateTime.Now.ToString("dd/MM/yyyy");

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
                                                    return RedirectToAction("MagazineSchoolRequirements", "School");
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

                                    #endregion

                                }
                                else
                                {
                                    TempData["resultIns"] = "F";
                                }
                            }

                            return View(magazineSchoolRequirements);
                        }

                        else if (_affiliationFee.ChallanCategory == 2)
                        {
                            ViewData["ChallanCategory"] = _affiliationFee.ChallanCategory;



                            if (submit.ToLower() == "go" && !string.IsNullOrEmpty(OldDepositDate) && !string.IsNullOrEmpty(OldAmount))
                            {
                                ViewBag.FEECAT = "MagazineSchoolFee";
                                ViewBag.OldAmount = OldAmount;
                                string today = OldDepositDate;

                                ViewBag.Total = magazineSchoolRequirements.TotalAmount;
                                int Balance = magazineSchoolRequirements.TotalAmount - Convert.ToInt32(OldAmount);
                                ViewBag.Balance = Balance;
                                ViewData["FeeStatus"] = "1";
                                ViewBag.TotalCount = 1;

                                return View(magazineSchoolRequirements);

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
                                    _affiliationFee1 = new AbstractLayer.SchoolDB().MagazineSchoolRequirementsFeeDetails(_affiliationFee.ChallanCategory, loginSession.SCHL, "", dateselected);


                                    ViewBag.FEECAT = "MagazineSchoolFee";
                                    ViewBag.OldAmount = OldAmount;

                                    ViewBag.Total = magazineSchoolRequirements.TotalAmount;
                                    int Balance = magazineSchoolRequirements.TotalAmount - Convert.ToInt32(OldAmount);
                                    ViewBag.Balance = Balance;
                                    ViewData["FeeStatus"] = "1";
                                    ViewBag.TotalCount = 1;

                                    //if (Balance > 0)
                                    //{
                                    //    _affiliationFee1.fee = Balance;
                                    //    _affiliationFee1.latefee = 0;
                                    //    _affiliationFee1.totfee = Balance;
                                    //}
                                    //else
                                    //{
                                    //    _affiliationFee1.fee = 0;
                                    //    _affiliationFee1.latefee = 0;
                                    //    _affiliationFee1.totfee = 0;
                                    //    _affiliationFee1.BankCode = _affiliationFee.BankCode = "203";
                                    //     AllowBanks = "203"; 
                                    //}

                                    _affiliationFee1.fee = magazineSchoolRequirements.TotalAmount;
                                    _affiliationFee1.latefee = 0;
                                    _affiliationFee1.totfee = magazineSchoolRequirements.TotalAmount;
                                    _affiliationFee1.BankCode = _affiliationFee.BankCode = "203";
                                    AllowBanks = "203";
                                    _affiliationFee1.AllowBanks = AllowBanks;
                                    _affiliationFee1.FEECAT = "MagazineSchoolFee";
                                    _affiliationFee1.FEECODE = 72;
                                    _affiliationFee1.SCHL = magazineSchoolRequirements.Schl;
                                    _affiliationFee1.BankLastdate = DateTime.Now.ToString("dd/MM/yyyy");

                                    //
                                    _affiliationFee.SCHL = _affiliationFee1.SCHL;
                                    _affiliationFee.fee = _affiliationFee1.fee;
                                    _affiliationFee.latefee = _affiliationFee1.latefee;
                                    _affiliationFee.totfee = _affiliationFee1.totfee;
                                    _affiliationFee.FEECAT = _affiliationFee1.FEECAT;
                                    _affiliationFee.FEECODE = _affiliationFee1.FEECODE;
                                    _affiliationFee.sDate = DateTime.Now.ToString("dd/MM/yyyy");
                                    _affiliationFee.eDate = DateTime.Now.ToString("dd/MM/yyyy");
                                    _affiliationFee.BankLastdate = _affiliationFee1.BankLastdate;
                                    _affiliationFee.AllowBanks = _affiliationFee1.AllowBanks;
                                    _affiliationFee.TotalFeesInWords = new AbstractLayer.DBClass().GetAmountInWords(_affiliationFee.totfee);


                                }


                                if ((submit.ToLower().Contains("final") || submit.ToLower().Contains("online")) && _affiliationFee.ChallanCategory == 2)
                                {

                                    using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                                    {
                                        try
                                        {
                                            //
                                            ChallanMasterModel CM = new ChallanMasterModel();
                                            if (string.IsNullOrEmpty(AllowBanks))
                                            {
                                                ViewBag.Message = "Please Select Bank";
                                                ViewData["SelectBank"] = "1";
                                                return View(magazineSchoolRequirements);
                                            }
                                            else
                                            {
                                                _affiliationFee.BankCode = AllowBanks;
                                                string BankCode = _affiliationFee.BankCode;
                                                // string AllowBanks = _affiliationFee.BankCode;

                                                string bankName = "";
                                                if (AllowBanks == "203")
                                                {
                                                    PayModValue = "hod";
                                                    bankName = "PSEB HOD";
                                                    CM.FEEMODE = "CASH";
                                                    CM.DepositoryMobile = "CASH";
                                                }

                                                //Balance
                                                magazineSchoolRequirements.TotalAmount = _affiliationFee1.totfee;
                                                string filename = "", path = "";
                                                string selCateory = _affiliationFee.ChallanCategory.ToString();
                                                if (ReceiptScannedCopy != null)
                                                {
                                                    string ext = Path.GetExtension(ReceiptScannedCopy.FileName);
                                                    filename = magazineSchoolRequirements.Schl + "_" + selCateory + "_ReceiptScannedCopy" + ext;
                                                    path = Path.Combine(Server.MapPath("~/Upload/Upload2023/MagazineSchoolRequirements/ReceiptScannedCopy"), filename);
                                                    string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/MagazineSchoolRequirements/ReceiptScannedCopy"));
                                                    _affiliationFee.ReceiptScannedCopy = "Upload2023/MagazineSchoolRequirements/ReceiptScannedCopy/" + filename;
                                                    magazineSchoolRequirements.ReceiptScannedCopy = _affiliationFee.ReceiptScannedCopy;
                                                }
                                                MagazineSchoolRequirements magazineSchool = new MagazineSchoolRequirements()
                                                {
                                                    MagazineId = 0,
                                                    Schl = magazineSchoolRequirements.Schl,
                                                    RefNo = magazineSchoolRequirements.RefNo,
                                                    FY = magazineSchoolRequirements.FY,
                                                    FixedMagazine = magazineSchoolRequirements.FixedMagazine,
                                                    FixedRate = magazineSchoolRequirements.FixedRate,
                                                    FixedMonth = magazineSchoolRequirements.FixedMonth,
                                                    FixedTotal = magazineSchoolRequirements.FixedTotal,
                                                    ExtraMagazine = magazineSchoolRequirements.ExtraMagazine,
                                                    ExtraRate = magazineSchoolRequirements.ExtraRate,
                                                    ExtraMonth = magazineSchoolRequirements.ExtraMonth,
                                                    ExtraTotal = magazineSchoolRequirements.ExtraTotal,
                                                    TotalAmount = magazineSchoolRequirements.TotalAmount,
                                                    SubmitOn = DateTime.Now,
                                                    ModifyOn = DateTime.Now,
                                                    IsActive = true,
                                                    OldAmount = magazineSchoolRequirements.OldAmount,
                                                    oldChallanId = magazineSchoolRequirements.oldChallanId == null ? "" : magazineSchoolRequirements.oldChallanId,
                                                    OldDepositDate = magazineSchoolRequirements.OldDepositDate,
                                                    OldReceiptNo = magazineSchoolRequirements.OldReceiptNo,
                                                    ChallanCategory = "2",
                                                    ReceiptScannedCopy = magazineSchoolRequirements.ReceiptScannedCopy
                                                };

                                                _context.MagazineSchoolRequirements.Add(magazineSchool);
                                                int insertedRecords = await _context.SaveChangesAsync();
                                                transaction.Commit();


                                                if (magazineSchoolRequirements.TotalAmount >= 0 && insertedRecords > 0)
                                                {

                                                    CM.FEE = magazineSchoolRequirements.TotalAmount;
                                                    CM.latefee = 0;
                                                    CM.TOTFEE = magazineSchoolRequirements.TotalAmount;
                                                    string TotfeePG = (CM.TOTFEE).ToString();
                                                    CM.FEECAT = _affiliationFee1.FEECAT;
                                                    CM.FEECODE = _affiliationFee1.FEECODE.ToString();
                                                    //CM.FEEMODE = "ONLINE";
                                                    CM.BCODE = AllowBanks;
                                                    CM.BANK = bankName;
                                                    CM.BANKCHRG = 0;
                                                    CM.SchoolCode = magazineSchoolRequirements.ChallanCategory;
                                                    CM.DIST = "";
                                                    CM.DISTNM = "";
                                                    CM.LOT = NOS_Entry + 1;
                                                    CM.SCHLREGID = magazineSchoolRequirements.Schl;
                                                    CM.FeeStudentList = magazineSchoolRequirements.RefNo;
                                                    CM.APPNO = magazineSchoolRequirements.RefNo;


                                                    CM.prosfee = Convert.ToInt32(AmountToBePaid);  // Total Amount till Deposit Date
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
                                                    result = new AbstractLayer.HomeDB().InsertPaymentForm(CM, fc, out SchoolMobile);
                                                    if (result == "0" || result == "")
                                                    {
                                                        //--------------Not saved
                                                        ViewData["result"] = 0;
                                                        return View(magazineSchoolRequirements);
                                                    }
                                                    else if (result == "-1")
                                                    {
                                                        //-----alredy exist
                                                        ViewData["result"] = -1;
                                                        return View(magazineSchoolRequirements);
                                                    }
                                                    else
                                                    {
                                                        ViewData["FeeStatus"] = null;
                                                        ViewData["SelectBank"] = null;
                                                        ViewData["result"] = 1;
                                                        ViewBag.ChallanNo = result;

                                                        if (ReceiptScannedCopy != null)
                                                        {
                                                            try
                                                            {

                                                                //MagazineSchoolRequirements magazineSchoolRequirementsUpdate = _context.MagazineSchoolRequirements.Where(s => s.RefNo == magazineSchoolRequirements.RefNo).SingleOrDefault();
                                                                //magazineSchoolRequirementsUpdate.ReceiptScannedCopy = magazineSchoolRequirements.ReceiptScannedCopy;
                                                                //_context.Entry(magazineSchoolRequirementsUpdate).State = EntityState.Modified;
                                                                //_context.SaveChanges();

                                                                string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/MagazineSchoolRequirements/ReceiptScannedCopy"));
                                                                if (!Directory.Exists(FilepathExist))
                                                                {
                                                                    Directory.CreateDirectory(FilepathExist);
                                                                }
                                                                ReceiptScannedCopy.SaveAs(path);

                                                            }
                                                            catch (Exception)
                                                            {

                                                            }
                                                        }
                                                        transaction.Commit();//transaction commit
                                                        if (result.Length > 7)
                                                        {
                                                            TotfeePG = CM.TOTFEE.ToString();
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
                                                                        return View(magazineSchoolRequirements);
                                                                    }
                                                                }
                                                                #endregion Payment Gateyway
                                                            }
                                                            else if (result.Length > 5)
                                                            {
                                                                return RedirectToAction("MagazineSchoolRequirements", "School");

                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            //
                                            transaction.Commit();//transaction commit
                                        }
                                        catch (Exception ex1)
                                        {

                                            transaction.Rollback();
                                        }

                                    }
                                }
                            }


                        }
                    }
                }

            }
            catch (Exception ex)
            {
                TempData["resultIns"] = "Error : " + ex.Message;
            }
            return RedirectToAction("MagazineSchoolRequirements", "School");
            //return View(magazineSchoolRequirements);
        }


        [SessionCheckFilter]
        [HttpGet]
        public ActionResult MagazineSchoolRequirementsPrint(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("MagazineSchoolRequirements", "School");
            }

            MagazineSchoolRequirements magazineSchoolRequirements = _context.MagazineSchoolRequirements.Single(s => s.RefNo == id);
            return View(magazineSchoolRequirements);
        }
        #endregion


        #region Meritorious Exam Centre
        [SessionCheckFilter]
        public ActionResult MeritoriousExamCentre(int? page)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            if (loginSession.IsMeritoriousSchool == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            Printlist obj = new Printlist();
            #region Circular

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.pagesize = pageIndex;

            string Search = string.Empty;
            Search = "Id like '%' and CircularTypes like '%10%' and Convert(Datetime,Convert(date,ExpiryDateDD))>=Convert(Datetime,Convert(date,getdate()))";
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


            obj.StoreAllData = AbstractLayer.SchoolDB.MeritoriousCentreMaster(loginSession.SCHL);
            if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "0";
                ViewBag.TotalCount = 0;
            }
            else
            {
                ViewBag.Message = "1";
                ViewBag.TotalCount = 1;
            }
            return View(obj);
        }


        #region  Signature Chart and Confidential Meritorious 
        [SessionCheckFilter]
        public ActionResult ConfidentialListMeritorious(SchoolModels sm)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            if (loginSession.IsMeritoriousSchool == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            List<SelectListItem> schllist = new List<SelectListItem>();
            ViewBag.MySchCode = schllist;
            try
            {

                DataSet Dresult = AbstractLayer.SchoolDB.GetMeritoriousCentCodeBySchl(loginSession.SCHL);
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {
                    schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                }
                ViewBag.MySchCode = schllist;
            }
            catch (Exception ex)
            {
            }
            return View(sm);
        }

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult ConfidentialListMeritorious(SchoolModels sm, FormCollection frc, string ExamCent)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            if (loginSession.IsMeritoriousSchool == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            List<SelectListItem> schllist = new List<SelectListItem>();
            ViewBag.MySchCode = schllist;
            try
            {

                DataSet Dresult = AbstractLayer.SchoolDB.GetMeritoriousCentCodeBySchl(loginSession.SCHL);
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {
                    schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                }
                ViewBag.MySchCode = schllist;

                if (!string.IsNullOrEmpty(ExamCent))
                {
                    sm.ExamCent = ExamCent;
                    sm.CLASS = "";
                }
                sm.StoreAllData = AbstractLayer.SchoolDB.GetMeritoriousConfidentialList(sm);
                //sm.ExamCent = Session["cent"].ToString();
                ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
            }
            catch (Exception ex)
            {
            }
            return View(sm);

        }


        [SessionCheckFilter]
        public ActionResult SignatureChartMeritorious(SchoolModels sm)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            if (loginSession.IsMeritoriousSchool == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            List<SelectListItem> schllist = new List<SelectListItem>();
            ViewBag.MySchCode = schllist;
            try
            {

                DataSet Dresult = AbstractLayer.SchoolDB.GetMeritoriousCentCodeBySchl(loginSession.SCHL);
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {
                    schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                }
                ViewBag.MySchCode = schllist;

            }
            catch (Exception ex)
            {
            }
            return View(sm);

        }


        [SessionCheckFilter]
        [HttpPost]
        public ActionResult SignatureChartMeritorious(SchoolModels sm, FormCollection frc, string ExamCent, string ExamRoll)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            if (loginSession.IsMeritoriousSchool == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            List<SelectListItem> schllist = new List<SelectListItem>();
            ViewBag.MySchCode = schllist;
            try
            {

                DataSet Dresult = AbstractLayer.SchoolDB.GetMeritoriousCentCodeBySchl(loginSession.SCHL);
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {
                    schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                }
                ViewBag.MySchCode = schllist;

                if (!string.IsNullOrEmpty(ExamCent))
                {
                    sm.ExamCent = ExamCent;
                }
                if (!string.IsNullOrEmpty(ExamRoll))
                {
                    sm.ExamRoll = ExamRoll;
                }
                sm.StoreAllData = AbstractLayer.SchoolDB.GetMeritoriousConfidentialList(sm);
                //sm.ExamCent = Session["cent"].ToString();
                ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
            }

            catch (Exception ex)
            {
            }
            return View(sm);

        }

        #endregion  Signature Chart and Confidential Meritorious 





        #endregion Meritorious Exam Centre


        #region PrivateStudents Signature Chart and Confidential List 
        [SessionCheckFilter]
        public ActionResult SignatureChartPrivateStudents(string id, SchoolModels sm)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Logout", "Login");
            }
            if (loginSession.IsPrivateExam == 0)
            {
                return RedirectToAction("Logout", "Login");
            }

            ViewBag.cid = id;
            sm.CLASS = id.ToString().ToLower().Trim() == "senior" ? "12" : id.ToString().ToLower().Trim() == "matric" ? "10" : "";
            ViewBag.Cls = sm.CLASS;

            string Schl = loginSession.SCHL.ToString();
            string Cent = "";
            try
            {
                if (Schl != "")
                {
                    DataSet Dresult = _schoolRepository.SignatureChartSP_PrivateStudents(1, sm.CLASS, Schl, Cent);
                    List<SelectListItem> schllist = new List<SelectListItem>();
                    List<SelectListItem> Sublist = new List<SelectListItem>();

                    if (Dresult.Tables[0].Rows.Count > 0)
                    {
                        Cent = Dresult.Tables[0].Rows[0]["cent"].ToString();
                        sm.ExamCent = Cent;
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                        }

                    }

                    if (Dresult.Tables[1].Rows.Count > 0)
                    {
                        foreach (System.Data.DataRow dr in Dresult.Tables[1].Rows)
                        {
                            Sublist.Add(new SelectListItem { Text = @dr["subnm"].ToString(), Value = @dr["sub"].ToString() });
                        }

                    }

                    ViewBag.MySchCode = schllist;
                    ViewBag.MyExamSub = Sublist;

                    sm.ExamCent = Cent;
                    sm.ExamRoll = sm.ExamSub = "";

                }
                else
                {
                    ViewBag.TotalCount = 0;
                    ViewBag.msg = "Data Not Found";
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult SignatureChartPrivateStudents(string id, SchoolModels sm, FormCollection frc)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Logout", "Login");
            }
            if (loginSession.IsPrivateExam == 0)
            {
                return RedirectToAction("Logout", "Login");
            }
            ViewBag.cid = id;
            sm.CLASS = id.ToString().ToLower().Trim() == "senior" ? "12" : id.ToString().ToLower().Trim() == "matric" ? "10" : "";
            ViewBag.Cls = sm.CLASS;

            try
            {

                string Schl = loginSession.SCHL.ToString();
                string Cent = frc["ExamCent"].ToString();
                string roll = frc["ExamRoll"].ToString();
                if (Cent != "")
                {

                    sm.ExamCent = Cent;
                    sm.ExamSub = frc["ExamSub"].ToString();
                    sm.ExamRoll = frc["ExamRoll"].ToString();

                    DataSet Dresult = _schoolRepository.SignatureChartSP_PrivateStudents(1, sm.CLASS, Schl, Cent);
                    List<SelectListItem> schllist = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                    }

                    ViewBag.MySchCode = schllist;
                    List<SelectListItem> Sublist = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[1].Rows)
                    {
                        Sublist.Add(new SelectListItem { Text = @dr["subnm"].ToString(), Value = @dr["sub"].ToString() });
                    }

                    ViewBag.MyExamSub = Sublist;

                    sm.StoreAllData = _schoolRepository.GetSignatureChartSP_PrivateStudents(sm);
                    sm.ExamCent = Cent;
                    ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                    if (ViewBag.SearchMsg == 0)
                    {
                        ViewBag.Message = "No Record Found";
                    }
                }
                else
                {
                    ViewBag.TotalCount = 0;
                    ViewBag.msg = "Data Not Found";
                }

            }
            catch (Exception ex)
            {

            }
            return View(sm);
        }

        [SessionCheckFilter]
        public ActionResult ConfidentialListPrivateStudents(string id, SchoolModels sm)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Logout", "Login");
            }
            if (loginSession.IsPrivateExam == 0)
            {
                return RedirectToAction("Logout", "Login");
            }
            ViewBag.cid = id;
            sm.CLASS = id.ToString().ToLower().Trim() == "senior" ? "12" : id.ToString().ToLower().Trim() == "matric" ? "10" : "";
            ViewBag.Cls = sm.CLASS;

            try
            {

                string Schl = loginSession.SCHL.ToString();
                string Cent = "";
                if (Schl != "")
                {

                    DataSet Dresult = _schoolRepository.SignatureChartSP_PrivateStudents(1, sm.CLASS, Schl, Cent);
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    if (Dresult.Tables[0].Rows.Count > 0)
                    {
                        Cent = Dresult.Tables[0].Rows[0]["cent"].ToString();
                        sm.ExamCent = Cent;
                        foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                        {
                            schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                        }

                    }
                    ViewBag.MySchCode = schllist;
                }
                else
                {
                    ViewBag.TotalCount = 0;
                    ViewBag.msg = "Data Not Found";
                }

            }
            catch (Exception ex)
            {

            }
            return View(sm);
        }

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult ConfidentialListPrivateStudents(string id, SchoolModels sm, FormCollection frc)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("SignatureChart", "School");
            }

            ViewBag.cid = id;
            sm.CLASS = id.ToString().ToLower().Trim() == "senior" ? "12" : id.ToString().ToLower().Trim() == "matric" ? "10" : "";
            ViewBag.Cls = sm.CLASS;
            try
            {

                string Schl = loginSession.SCHL.ToString();
                string Cent = frc["ExamCent"].ToString();
                if (Cent != "")
                {

                    sm.ExamCent = Cent;
                    DataSet Dresult = _schoolRepository.SignatureChartSP_PrivateStudents(1, sm.CLASS, Schl, Cent);
                    List<SelectListItem> schllist = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                    }

                    ViewBag.MySchCode = schllist;
                    if (frc["ExamCent"].ToString() != "")
                    {
                        sm.ExamCent = frc["ExamCent"].ToString();
                    }

                    sm.StoreAllData = _schoolRepository.GetConfidentialListSP_PrivateStudents(sm);
                    ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;

                }
                else
                {
                    ViewBag.TotalCount = 0;
                    ViewBag.msg = "Data Not Found";
                }


            }
            catch (Exception ex)
            {

            }
            return View(sm);
        }
        #endregion PrivateStudents  Signature Chart and Confidential List Matric



        #region  Online Centre Creation

        [SessionCheckFilter]
        public ActionResult OnlineCentreCreation(SchoolMasterForOnlineCentreCreationViews schoolMasterForOnlineCentreCreationViews)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            schoolMasterForOnlineCentreCreationViews = _context.SchoolMasterForOnlineCentreCreationViews.Where(s => s.SCHL == loginSession.SCHL).SingleOrDefault();

            ViewBag.IsExistsinCentre2017Master = schoolMasterForOnlineCentreCreationViews.IsExistsinCentre2017Master;
            ViewBag.IsExistsInOnlineCentreCreationsTERM1 = schoolMasterForOnlineCentreCreationViews.IsExistsInOnlineCentreCreationsTERM1;

            if (TempData["resultOnlineCentreCreation"] != null)
            {
                ViewData["resultOnlineCentreCreation"] = TempData["resultOnlineCentreCreation"];
            }


            // check any challan verified
            schoolMasterForOnlineCentreCreationViews.OnlineCentreCreationsChallanList = _context.OnlineCentreCreationsChallanViews.AsNoTracking().Where(s => s.SCHL == loginSession.SCHL).ToList();


            return View(schoolMasterForOnlineCentreCreationViews);

        }


        [SessionCheckFilter]
        [HttpPost]
        public async Task<ActionResult> OnlineCentreCreation(SchoolMasterForOnlineCentreCreationViews schoolMasterForOnlineCentreCreationViews, FormCollection frc, HttpPostedFileBase ScanCopy, string submit)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];


            ViewBag.IsExistsinCentre2017Master = schoolMasterForOnlineCentreCreationViews.IsExistsinCentre2017Master;
            ViewBag.IsExistsInOnlineCentreCreationsTERM1 = schoolMasterForOnlineCentreCreationViews.IsExistsInOnlineCentreCreationsTERM1;
            // SchoolMasterForOnlineCentreCreationViews schoolMasterForOnlineCentreCreationViews1 = _context.SchoolMasterForOnlineCentreCreationViews.Where(s => s.SCHL == loginSession.SCHL).SingleOrDefault();





            if (string.IsNullOrEmpty(submit))
            { return RedirectToAction("OnlineCentreCreation", "School"); }
            else
            {

                if (schoolMasterForOnlineCentreCreationViews.SENIOR_COUNT >= 35 || schoolMasterForOnlineCentreCreationViews.MATRIC_COUNT >= 35 || schoolMasterForOnlineCentreCreationViews.MIDDLE_COUNT >= 35)
                {

                    if (schoolMasterForOnlineCentreCreationViews.OnlineCentreCreationId > 0)
                    {
                        using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                        {
                            try
                            {
                                OnlineCentreCreations onlineCentreCreations1 = _context.OnlineCentreCreations.Find(schoolMasterForOnlineCentreCreationViews.OnlineCentreCreationId);
                                onlineCentreCreations1.TotalRooms = schoolMasterForOnlineCentreCreationViews.TotalRooms;
                                onlineCentreCreations1.TotalCapacity = schoolMasterForOnlineCentreCreationViews.TotalCapacity;
                                onlineCentreCreations1.TotalRoomsOther = schoolMasterForOnlineCentreCreationViews.TotalRoomsOther;
                                onlineCentreCreations1.SingleBenchFurniture = schoolMasterForOnlineCentreCreationViews.SingleBenchFurniture;
                                onlineCentreCreations1.DoubleBenchFurniture = schoolMasterForOnlineCentreCreationViews.DoubleBenchFurniture;
                                onlineCentreCreations1.ModifyBy = loginSession.SCHL;
                                onlineCentreCreations1.ModifyOn = DateTime.Now;

                                // Save file
                                string filename = "";
                                string FilepathExist = "", path = "";
                                if (ScanCopy != null)
                                {
                                    string ext = Path.GetExtension(ScanCopy.FileName);
                                    filename = onlineCentreCreations1.CentreAppNo + ext;
                                    path = Path.Combine(Server.MapPath("~/Upload/Upload2023/OnlineCentreCreations"), filename);
                                    FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/OnlineCentreCreations"));
                                    onlineCentreCreations1.ScanCopy = "Upload2023/OnlineCentreCreations/" + filename;
                                }

                                _context.Entry(onlineCentreCreations1).State = EntityState.Modified;
                                // _context.SaveChanges();
                                int insertedRecords = await _context.SaveChangesAsync();

                                if (insertedRecords > 0)
                                {
                                    TempData["resultOnlineCentreCreation"] = "U";

                                    if (ScanCopy != null)
                                    {
                                        if (!Directory.Exists(FilepathExist))
                                        {
                                            Directory.CreateDirectory(FilepathExist);
                                        }
                                        ScanCopy.SaveAs(path);
                                    }
                                }
                                else
                                {
                                    TempData["resultOnlineCentreCreation"] = "UF";
                                }
                                transaction.Commit();//transaction commit
                            }
                            catch (Exception ex1)
                            {
                                transaction.Rollback();
                            }
                        }
                    }
                    else if (schoolMasterForOnlineCentreCreationViews.OnlineCentreCreationId == 0)
                    {
                        using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                        {

                            int NOS_Entry = _context.OnlineCentreCreations.Where(s => s.SCHL == loginSession.SCHL).Count();
                            string CentreAppNo = "TERM2" + loginSession.SCHL + string.Format("{0:D3}", NOS_Entry + 1);

                            OnlineCentreCreations onlineCentreCreation = new OnlineCentreCreations()
                            {

                                OnlineCentreCreationId = 0,
                                CentreAppNo = CentreAppNo,
                                SCHL = loginSession.SCHL,
                                // CENT = schoolMasterForOnlineCentreCreationViews.CENT,
                                //IsNew = schoolMasterForOnlineCentreCreationViews.CAT,
                                USERTYPE = schoolMasterForOnlineCentreCreationViews.USERTYPE,
                                MATREG = schoolMasterForOnlineCentreCreationViews.MATRIC_COUNT,
                                MATOPN = 0,
                                SSREG = schoolMasterForOnlineCentreCreationViews.SENIOR_COUNT,
                                SSOPN = 0,
                                MATPVT = 0,
                                SSPVT = 0,
                                MIDREG = schoolMasterForOnlineCentreCreationViews.MIDDLE_COUNT,
                                TotalRooms = schoolMasterForOnlineCentreCreationViews.TotalRooms,
                                TotalCapacity = schoolMasterForOnlineCentreCreationViews.TotalCapacity,
                                TotalRoomsOther = schoolMasterForOnlineCentreCreationViews.TotalRoomsOther,
                                SingleBenchFurniture = schoolMasterForOnlineCentreCreationViews.SingleBenchFurniture,
                                DoubleBenchFurniture = schoolMasterForOnlineCentreCreationViews.DoubleBenchFurniture,
                                SubmitOn = DateTime.Now,
                                ModifyOn = DateTime.Now,
                                ChallanVerify = 0,
                                SchoolCategoryMain = schoolMasterForOnlineCentreCreationViews.SchoolCategoryMain,
                            };

                            //if (schoolMasterForOnlineCentreCreationViews.IsExistsinCentre2017Master == 0)
                            if (schoolMasterForOnlineCentreCreationViews.IsExistsInOnlineCentreCreationsTERM1 == 0) //for term 2
                            {
                                //  create new centre
                                onlineCentreCreation.IsNew = "NEW";
                                onlineCentreCreation.CENT = "";
                            }
                            else
                            {
                                onlineCentreCreation.IsNew = "RENEW";
                                onlineCentreCreation.CENT = "";
                            }



                            // Save file
                            string filename = "";
                            string FilepathExist = "", path = "";
                            if (ScanCopy != null)
                            {
                                string ext = Path.GetExtension(ScanCopy.FileName);
                                filename = onlineCentreCreation.CentreAppNo + ext;
                                //path = Path.Combine(Server.MapPath("~/Upload/Upload2023/OnlineCentreCreations"), filename);
                                //FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/OnlineCentreCreations"));
                                //onlineCentreCreation.ScanCopy = "Upload2023/OnlineCentreCreations/" + filename;
                                path = Path.Combine(Server.MapPath("allfiles/Upload2023/OnlineCentreCreations"), filename);
                                FilepathExist = Path.Combine(Server.MapPath("allfiles/Upload2023/OnlineCentreCreations"));
                                onlineCentreCreation.ScanCopy = "allfiles/Upload2023/OnlineCentreCreations/" + filename;
                            }


                            _context.OnlineCentreCreations.Add(onlineCentreCreation);
                            int insertedRecords = await _context.SaveChangesAsync();
                            // _context?.Dispose();
                            transaction.Commit();//transaction commit
                            if (insertedRecords > 0)
                            {
                                TempData["resultOnlineCentreCreation"] = "S";

                                if (ScanCopy != null)
                                {
                                    //if (!Directory.Exists(FilepathExist))
                                    //{
                                    //    Directory.CreateDirectory(FilepathExist);
                                    //}
                                    //ScanCopy.SaveAs(path);
                                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                                    {
                                        using (var newMemoryStream = new MemoryStream())
                                        {
                                            var uploadRequest = new TransferUtilityUploadRequest
                                            {
                                                InputStream = ScanCopy.InputStream,
                                                Key = string.Format("allfiles/Upload2023/OnlineCentreCreations/{0}", filename),
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
                                TempData["resultOnlineCentreCreation"] = "SF";
                            }
                        }

                    }

                }
                else
                {
                    TempData["resultOnlineCentreCreation"] = "NA";

                }
            }

            return RedirectToAction("OnlineCentreCreation", "School");

        }




        #region Challan and Payment Details 

        [SessionCheckFilter]
        public ActionResult OnlineCentreCreation_PaymentForm(string id)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            OnlineCentreCreationsPaymentForm onlineCentreCreationsPaymentForm = new OnlineCentreCreationsPaymentForm();

            onlineCentreCreationsPaymentForm = AbstractLayer.SchoolDB.GetOnlineCentreCreationsPayment(id);

            if (onlineCentreCreationsPaymentForm.SCHL == null)
            { ViewBag.TotalCount = 0; Session["StudentSchoolMigrationFee"] = null; }
            else
            {
                Session["OnlineCentreCreationPaymentFees"] = onlineCentreCreationsPaymentForm;
                ViewData["FeeStatus"] = "1";
                ViewBag.TotalCount = 1;
            }

            return View(onlineCentreCreationsPaymentForm);
        }



        [HttpPost]
        [SessionCheckFilter]
        public ActionResult OnlineCentreCreation_PaymentForm(string id, OnlineCentreCreationsPaymentForm onlineCentreCreationsPaymentForm, FormCollection frm, string PayModValue, string AllowBanks)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            // OnlineCentreCreationsPaymentForm onlineCentreCreationsPaymentForm = new OnlineCentreCreationsPaymentForm();

            try
            {

                ChallanMasterModel CM = new ChallanMasterModel();
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("OnlineCentreCreation", "School");
                }
                if (Session["OnlineCentreCreationPaymentFees"] == null)
                {
                    return RedirectToAction("OnlineCentreCreation_PaymentForm", "School");
                }
                string appno = id;
                onlineCentreCreationsPaymentForm = AbstractLayer.SchoolDB.GetOnlineCentreCreationsPayment(id);


                string bankName = "";

                if (AllowBanks == "301" || AllowBanks == "302")
                {
                    PayModValue = "online";
                    CM.FEEMODE = "ONLINE";
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
                    CM.FEEMODE = "CASH";
                    PayModValue = "hod";
                    bankName = "PSEB HOD";
                }
                else if (AllowBanks == "202" || AllowBanks == "204")
                {
                    CM.FEEMODE = "CASH";
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






                if (string.IsNullOrEmpty(AllowBanks))
                {
                    ViewBag.Message = "Please Select Bank";
                    ViewData["SelectBank"] = "1";
                    return RedirectToAction("OnlineCentreCreation_PaymentForm", "School");
                }


                if (ModelState.IsValid)
                {


                    CM.FEECAT = onlineCentreCreationsPaymentForm.FEECAT;
                    CM.FEECODE = onlineCentreCreationsPaymentForm.FEECODE.ToString();
                    CM.BCODE = AllowBanks;
                    CM.BANK = bankName;
                    CM.BANKCHRG = 0;
                    CM.SchoolCode = onlineCentreCreationsPaymentForm.SCHL;
                    CM.DIST = "";
                    CM.DISTNM = "";
                    CM.LOT = 1;
                    CM.SCHLREGID = onlineCentreCreationsPaymentForm.SCHL;
                    CM.FeeStudentList = onlineCentreCreationsPaymentForm.CentreAppNo;
                    CM.APPNO = onlineCentreCreationsPaymentForm.CentreAppNo;

                    CM.add_sub_count = Convert.ToInt32(onlineCentreCreationsPaymentForm.Add_Count);
                    CM.addfee = Convert.ToInt32(onlineCentreCreationsPaymentForm.ExtraFees);
                    CM.regfee = Convert.ToInt32(onlineCentreCreationsPaymentForm.ContiFee);

                    CM.FEE = Convert.ToInt32(onlineCentreCreationsPaymentForm.totfee);
                    CM.latefee = Convert.ToInt32(onlineCentreCreationsPaymentForm.latefee);
                    CM.TOTFEE = Convert.ToInt32(onlineCentreCreationsPaymentForm.totfee);
                    string TotfeePG = (CM.TOTFEE).ToString();

                    CM.type = "schle";
                    CM.CHLNVDATE = Convert.ToString(onlineCentreCreationsPaymentForm.BankLastdate);
                    DateTime BankLastDate = Convert.ToDateTime(onlineCentreCreationsPaymentForm.BankLastdate);
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
                        string bnkLastDate = Convert.ToDateTime(onlineCentreCreationsPaymentForm.BankLastdate).ToString("dd/MM/yyyy");
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
                                    return RedirectToAction("OnlineCentreCreation_PaymentForm", "School");
                                }
                            }
                            #endregion Payment Gateyway
                        }
                        else if (result.Length > 5)
                        {

                            //string Sms = "Challan no. " + result + " of Ref no  " + CM.APPNO + " successfully generated and valid till Dt " + bnkLastDate + ". Regards PSEB";
                            //try
                            //{
                            //    string getSms = objCommon.gosms(SchoolMobile, Sms);
                            //}
                            //catch (Exception) { }
                            return RedirectToAction("GenerateChallaan", "Home", new { ChallanId = result });

                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("OnlineCentreCreation", "School");
        }

        #endregion Challan and Payment Details



        [AllowAnonymous]
        public ActionResult OnlineCentreCreationPrint(string id)
        {
            OnlineCentreCreationsChallanViews onlineCentreCreationsChallanViews = new OnlineCentreCreationsChallanViews();

            // check any challan verified
            onlineCentreCreationsChallanViews = _context.OnlineCentreCreationsChallanViews.AsNoTracking().Where(s => s.CentreAppNo == id).SingleOrDefault();


            return View(onlineCentreCreationsChallanViews);

        }


        #endregion  Online Centre Creation



        #region Begin PhyChlMarksEntry   


        [SessionCheckFilter]
        public ActionResult PhyChl_Portal()
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            TempData["PhyChl_MarksSearch"] = null;
            return View();
        }


        [SessionCheckFilter]
        public ActionResult PhyChlMarksEntry_Agree(string id)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            TempData["PhyChlMarksEntryClass"] = id;
            return View();
        }

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult PhyChlMarksEntry_Agree(string id, FormCollection frm)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            try
            {
                string cc = id;
                string s = frm["Agree"].ToString();
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("PhyChlMarksEntry_Portal", "School");
                }
                else
                {
                    if (s == "Agree")
                    {
                        return RedirectToAction("PhyChlMarksEntryPanel", "School", new { id = cc });
                    }
                }
                return RedirectToAction("PhyChlMarksEntry_Portal", "School");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }
        }


        [SessionCheckFilter]
        public ActionResult PhyChlMarksEntryPanel(FormCollection frm, string id, int? page)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            SchoolModels MS = new SchoolModels();
            try
            {
                string CLASS = id == "Matric" ? "2" : id == "Senior" ? "4" : "";
                ViewBag.schlCode = loginSession.SCHL;
                ViewBag.cid = id;

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                string Search = string.Empty;
                string SelectedAction = "0";


                var itemFilter = new SelectList(new[] { new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                    new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";
                //------------------------

                if (loginSession.SCHL != null)
                {

                    if (TempData["PhyChlMarksEntryPanelSearch"] != null)
                    {
                        Search += TempData["PhyChlMarksEntryPanelSearch"].ToString();
                        ViewBag.SelectedFilter = TempData["SelFilter"];
                        SelectedAction = TempData["SelAction"].ToString();
                        ViewBag.SelectedAction = TempData["SelActionValue"];

                        TempData["SelFilter"] = ViewBag.SelectedFilter;
                        TempData["SelAction"] = SelectedAction;
                        TempData["SelActionValue"] = ViewBag.SelectedAction;
                        TempData["PhyChlMarksEntryPanelSearch"] = Search;
                    }
                    else
                    {
                        Search = "  a.schl = '" + loginSession.SCHL + "' and  a.class= '" + CLASS + "'";
                    }



                    MS.StoreAllData = _schoolRepository.GetPhyChlMarksEntryMarksDataBySCHL(Search, loginSession.SCHL, pageIndex, CLASS, Convert.ToInt32(SelectedAction));
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsMarksFilled"]);
                        }
                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                            ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[4].Rows[0]["FinalSubmitLastDate"];
                        }
                        if (MS.StoreAllData.Tables[5].Rows.Count > 0)
                        {
                            MS.schoolAllowForMarksEntry = new SchoolAllowForMarksEntry()
                            {
                                Cls = MS.StoreAllData.Tables[5].Rows[0]["Cls"].ToString(),
                                IsActive = Convert.ToInt32(MS.StoreAllData.Tables[5].Rows[0]["IsActive"].ToString()),
                                IsAllow = MS.StoreAllData.Tables[5].Rows[0]["IsAllow"].ToString(),
                                LastDate = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["LastdateDT"].ToString()),
                                Panel = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["Panel"].ToString())
                            };
                        }
                        return View(MS);
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);

                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsMarksFilled"]);
                        }

                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                            ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[4].Rows[0]["FinalSubmitLastDate"];
                        }
                        if (MS.StoreAllData.Tables[5].Rows.Count > 0)
                        {
                            MS.schoolAllowForMarksEntry = new SchoolAllowForMarksEntry()
                            {
                                Cls = MS.StoreAllData.Tables[5].Rows[0]["Cls"].ToString(),
                                IsActive = Convert.ToInt32(MS.StoreAllData.Tables[5].Rows[0]["IsActive"].ToString()),
                                IsAllow = MS.StoreAllData.Tables[5].Rows[0]["IsAllow"].ToString(),
                                LastDate = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["LastdateDT"].ToString()),
                                Panel = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["Panel"].ToString())
                            };
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

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult PhyChlMarksEntryPanel(string id, FormCollection frm, int? page)
        {


            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            SchoolModels MS = new SchoolModels();
            try
            {
                string CLASS = id == "Matric" ? "2" : id == "Senior" ? "4" : "";
                ViewBag.schlCode = loginSession.SCHL;
                ViewBag.cid = id;

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                var itemFilter = new SelectList(new[] { new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                    new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";
                //------------------------

                if (loginSession.SCHL != null)
                {
                    string Search = "";
                    Search = "  a.SCHL = '" + loginSession.SCHL + "' and  a.class='" + CLASS + "' ";

                    int SelAction = 0;
                    if (frm["SelAction"] != "")
                    {
                        int SelValueSch = Convert.ToInt32(frm["SelAction"].ToString());
                        if (frm["SelAction"] != "")
                        {
                            if (SelValueSch == 3)
                            {
                                SelAction = 2;
                            }
                            //  { Search += " and  IsMarksFilled=1 "; } // Filled
                            else if (SelValueSch == 2)
                            { SelAction = 1; }
                            //{ Search += " and (IsMarksFilled is null or IsMarksFilled=0) "; } // pending
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
                            { Search += " and b.Roll='" + frm["SearchString"].ToString() + "'"; }
                            if (SelValueSch == 2)
                            { Search += " and Std_id='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 3)
                            { Search += " and  Registration_num like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " and  Candi_Name like '%" + frm["SearchString"].ToString() + "%'"; }
                        }
                    }

                    TempData["SelFilter"] = frm["SelFilter"];
                    TempData["SelAction"] = SelAction;
                    TempData["SelActionValue"] = frm["SelAction"];
                    TempData["PhyChlMarksEntryPanelSearch"] = Search;
                    // string class1 = "4";
                    MS.StoreAllData = _schoolRepository.GetPhyChlMarksEntryMarksDataBySCHL(Search, loginSession.SCHL, pageIndex, CLASS, SelAction);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsMarksFilled"]);
                        }
                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                            ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[4].Rows[0]["FinalSubmitLastDate"];
                        }
                        if (MS.StoreAllData.Tables[5].Rows.Count > 0)
                        {
                            MS.schoolAllowForMarksEntry = new SchoolAllowForMarksEntry()
                            {
                                Cls = MS.StoreAllData.Tables[5].Rows[0]["Cls"].ToString(),
                                IsActive = Convert.ToInt32(MS.StoreAllData.Tables[5].Rows[0]["IsActive"].ToString()),
                                IsAllow = MS.StoreAllData.Tables[5].Rows[0]["IsAllow"].ToString(),
                                LastDate = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["LastdateDT"].ToString()),
                                Panel = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["Panel"].ToString())
                            };
                        }
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsMarksFilled"]);
                        }

                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                            ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[4].Rows[0]["FinalSubmitLastDate"];
                        }
                        if (MS.StoreAllData.Tables[5].Rows.Count > 0)
                        {
                            MS.schoolAllowForMarksEntry = new SchoolAllowForMarksEntry()
                            {
                                Cls = MS.StoreAllData.Tables[5].Rows[0]["Cls"].ToString(),
                                IsActive = Convert.ToInt32(MS.StoreAllData.Tables[5].Rows[0]["IsActive"].ToString()),
                                IsAllow = MS.StoreAllData.Tables[5].Rows[0]["IsAllow"].ToString(),
                                LastDate = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["LastdateDT"].ToString()),
                                Panel = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["Panel"].ToString())
                            };
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

        [SessionCheckFilter]
        [HttpPost]
        public JsonResult JqPhyChlMarksEntryMarks(string stdid, string CandSubject, string cls)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            var flag = 1;

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
                else if (rowObj.OBTMARKS == "O" || rowObj.OBTMARKS == "OC")
                {
                    rowObj.OBTMARKS = "OC";
                }
                else if (rowObj.OBTMARKS != "")
                {
                    rowObj.OBTMARKS = rowObj.OBTMARKS.PadLeft(3, '0');
                }
                else if (string.IsNullOrEmpty(rowObj.OBTMARKS))
                {
                    rowObj.OBTMARKS = "";
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
                if (dr1["OBTMARKS"].ToString() == "" || dr1["OBTMARKS"].ToString() == "OC" || dr1["OBTMARKS"].ToString() == "ABS" || dr1["OBTMARKS"].ToString() == "CAN")
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

                    if ((obt < 1) || (obt > max))
                    {
                        flag = -2;
                    }
                }
            }
            if (flag == 1)
            {
                string dee = "1";
                // string class1 = "5";
                string class1 = cls == "Primary" ? "5" : cls == "Middle" ? "8" : "5";
                int OutStatus = 0;
                dee = _schoolRepository.AllotPhyChlMarksEntryMarks(loginSession.SCHL, stdid, dtSub, class1, out OutStatus);
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
        public ActionResult PhyChlMarksEntryReport(string id)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            TempData["PhyChlMarksEntryPanelSearch"] = null;
            SchoolModels MS = new SchoolModels();
            try
            {
                string CLASS = id == "Matric" ? "2" : id == "Senior" ? "4" : "";
                ViewBag.schlCode = loginSession.SCHL;
                ViewBag.cid = id;

                string Search = string.Empty;
                Search = "  a.schl = '" + loginSession.SCHL + "' and  a.class= '" + CLASS + "'";
                string OutError = "";
                MS.StoreAllData = _schoolRepository.PhyChlMarksEntryMarksEntryReport(loginSession.SCHL, 0, Search, loginSession.SCHL, CLASS, out OutError);
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
                    ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["FIFSET"].ToString();
                    return View(MS);
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }

        [SessionCheckFilter]
        public ActionResult PhyChlMarksEntryFinalReport(string id, FormCollection frm)
        {
            TempData["PhyChlMarksEntryPanelSearch"] = null;
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            SchoolModels MS = new SchoolModels();
            try
            {
                string CLASS = id == "Matric" ? "2" : id == "Senior" ? "4" : "";
                ViewBag.schlCode = loginSession.SCHL;
                ViewBag.cid = id;

                string Search = string.Empty;
                Search = "  a.schl = '" + loginSession.SCHL + "' and  a.class= '" + CLASS + "'";
                string OutError = "";
                DataSet dsFinal = new DataSet();
                MS.StoreAllData = dsFinal = _schoolRepository.PhyChlMarksEntryMarksEntryReport(loginSession.SCHL, 1, Search, loginSession.SCHL, CLASS, out OutError);
                if (MS.StoreAllData == null)
                {
                    ViewBag.IsAllowPhyChlMarksEntry = 0;
                    ViewBag.IsFinal = 0;
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View(MS);
                }
                else if (MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.IsAllowPhyChlMarksEntry = 1;
                    MS.StoreAllData = _schoolRepository.PhyChlMarksEntryMarksEntryReport(loginSession.SCHL, 0, Search, loginSession.SCHL, CLASS, out OutError);
                    ViewBag.TotalCount = 0;
                    ViewBag.TotalCount1 = MS.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.MarksFilledDate = MS.StoreAllData.Tables[0].Rows[0]["MarksFilledDate"].ToString();
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

                    if (dsFinal.Tables[3].Rows.Count > 0)
                    {
                        MS.schoolAllowForMarksEntry = new SchoolAllowForMarksEntry()
                        {
                            Cls = dsFinal.Tables[3].Rows[0]["Cls"].ToString(),
                            IsActive = Convert.ToInt32(dsFinal.Tables[3].Rows[0]["IsActive"].ToString()),
                            IsAllow = dsFinal.Tables[3].Rows[0]["IsAllow"].ToString(),
                            LastDate = Convert.ToString(dsFinal.Tables[3].Rows[0]["LastdateDT"].ToString()),
                            Panel = Convert.ToString(dsFinal.Tables[3].Rows[0]["Panel"].ToString())
                        };
                    }
                    ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["FIFSET"].ToString();
                    ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[0].Rows[0]["FinalSubmitLastDate"];
                    return View(MS);
                }

                else
                {
                    ViewBag.IsAllowPhyChlMarksEntry = 2;
                    ViewBag.IsFinal = 0;
                    ViewBag.TotalCount1 = 0;
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[0].Rows[0]["FinalSubmitLastDate"];

                    ViewBag.SchoolName = MS.StoreAllData.Tables[1].Rows[0]["FullSchoolNameE"].ToString();
                    ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["FIFSET"].ToString();
                    return View(MS);
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult PhyChlMarksEntryFinalReport(string id)
        {
            TempData["PhyChlMarksEntryPanelSearch"] = null;
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            SchoolModels MS = new SchoolModels();
            try
            {
                string CLASS = id == "Matric" ? "2" : id == "Senior" ? "4" : "";
                ViewBag.schlCode = loginSession.SCHL;
                ViewBag.cid = id;
                string Search = string.Empty;
                string OutError = "";
                MS.StoreAllData = _schoolRepository.PhyChlMarksEntryMarksEntryReport(loginSession.SCHL, 2, Search, loginSession.SCHL, CLASS, out OutError);
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
                    ViewBag.MarksFilledDate = MS.StoreAllData.Tables[0].Rows[0]["MarksFilledDate"].ToString();
                    ViewBag.SchoolName = MS.StoreAllData.Tables[1].Rows[0]["FullSchoolNameE"].ToString();
                    ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["FIFSET"].ToString();
                    return View(MS);
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }


        #endregion  End PhyChlMarksEntry  


        #region Re-Exam For Absent Student in Term-1

        [SessionCheckFilter]
        public ActionResult ViewReExamTermStudentList(string id, ReExamTermStudentsModelList onDemandCertificateSearchModel)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("Index", "Home");
                }
                string Search = "", RP = "", cls = "";
                ViewBag.id = id;

                string SCHL = loginSession.SCHL;
                ViewBag.Senior = loginSession.Senior.ToString();
                ViewBag.Matric = loginSession.Matric.ToString();
                ViewBag.OSenior = loginSession.OSenior.ToString();
                ViewBag.OMatric = loginSession.OMATRIC.ToString();

                switch (id)
                {
                    case "S":
                        RP = "R";
                        cls = "4";
                        break;
                    case "SO":
                        RP = "O";
                        cls = "4";
                        break;
                    case "M":
                        RP = "R";
                        cls = "2";
                        break;
                    case "MO":
                        RP = "O";
                        cls = "2";
                        break;
                    default:
                        RP = "";
                        cls = "";
                        break;
                }
                ViewBag.RP = RP;
                ViewBag.cls = cls;

                //Search,
                DataSet dsOut = new DataSet();
                onDemandCertificateSearchModel.ReExamTermStudentsSearchModel = AbstractLayer.SchoolDB.GetReExamTermStudentList("GET", RP, cls, SCHL, Search, out dsOut);
                onDemandCertificateSearchModel.StoreAllData = dsOut;
                return View(onDemandCertificateSearchModel);
            }
            catch (Exception ex)
            {
                return View(id);
            }
        }


        [SessionCheckFilter]
        public JsonResult JqReExamTermApplyStudents(string studentlist, string cls)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];

            int result = 0;
            if (!string.IsNullOrEmpty(studentlist))
            {
                studentlist = studentlist.Remove(studentlist.Length - 1);


                List<int> listComma = studentlist.Split(',').Select(int.Parse).ToList();
                List<ReExamTermStudents> list = new List<ReExamTermStudents>();

                foreach (var stdid in listComma)
                {
                    list.Add(new ReExamTermStudents { ReExamId = 0, Std_id = stdid, Schl = loginSession.SCHL, Cls = Convert.ToInt32(cls), IsActive = 1, IsPrinted = 0, SubmitOn = DateTime.Now, SubmitBy = "SCHL" });
                }

                if (list.Count() > 0)
                {
                    result = new AbstractLayer.SchoolDB().InsertReExamTermStudentList(list);
                }
                else
                {
                    result = -1;
                }
            }
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }


        [SessionCheckFilter]
        public ActionResult ReExamTermAppliedStudentList(string id, ReExamTermStudentsModelList onDemandCertificateSearchModel)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("Index", "Home");
                }
                string Search = "", RP = "", cls = "";
                ViewBag.id = id;

                string SCHL = loginSession.SCHL;
                ViewBag.Senior = loginSession.Senior.ToString();
                ViewBag.Matric = loginSession.Matric.ToString();
                ViewBag.OSenior = loginSession.OSenior.ToString();
                ViewBag.OMatric = loginSession.OMATRIC.ToString();

                switch (id)
                {
                    case "S":
                        RP = "R";
                        cls = "4";
                        break;
                    case "SO":
                        RP = "O";
                        cls = "4";
                        break;
                    case "M":
                        RP = "R";
                        cls = "2";
                        break;
                    case "MO":
                        RP = "O";
                        cls = "2";
                        break;
                    default:
                        RP = "";
                        cls = "";
                        break;
                }
                ViewBag.RP = RP;
                ViewBag.cls = cls;

                //Search,
                DataSet dsOut = new DataSet();
                onDemandCertificateSearchModel.ReExamTermStudentsSearchModel = AbstractLayer.SchoolDB.GetReExamTermStudentList("ADDED", RP, cls, SCHL, Search, out dsOut);
                onDemandCertificateSearchModel.StoreAllData = dsOut;
                return View(onDemandCertificateSearchModel);
            }
            catch (Exception ex)
            {
                return View(id);
            }
        }


        [SessionCheckFilter]
        public JsonResult JqRemoveReExamTermApplyStudents(string demandIdList, string cls)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];

            int result = 0;
            if (!string.IsNullOrEmpty(demandIdList))
            {
                demandIdList = demandIdList.Remove(demandIdList.Length - 1);


                List<int> listComma = demandIdList.Split(',').Select(int.Parse).ToList();
                List<ReExamTermStudents> list = new List<ReExamTermStudents>();

                foreach (var did in listComma)
                {
                    list.Add(new ReExamTermStudents { ReExamId = did });
                }

                if (list.Count() > 0)
                {
                    result = new AbstractLayer.SchoolDB().RemoveRangeOnDemandCertificateStudentList(list);
                }
                else
                {
                    result = -1;
                }
            }
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }


        [SessionCheckFilter]
        public ActionResult ReExamTermStudents_ChallanList(ReExamTermStudents_ChallanDetailsViewsModelList obj)
        {
            try
            {
                LoginSession loginSession = (LoginSession)Session["LoginSession"];
                DataSet dsOut = new DataSet();
                obj.ReExamTermStudents_ChallanDetailsViews = AbstractLayer.SchoolDB.ReExamTermStudents_ChallanList(loginSession.SCHL, out dsOut);
                obj.StoreAllData = dsOut;
                return View(obj);
            }
            catch (Exception ex)
            {
                return View();
            }
        }



        #region Calculate Fee
        [SessionCheckFilter]
        public ActionResult ReExamTermStudentCalculateFee(string id, string D)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];

            try
            {
                Session["ReExamTermStudentCalculateFee"] = null;
                FeeHomeViewModel fhvm = new FeeHomeViewModel();

                string Search = string.Empty;
                Search = "SCHL=" + loginSession.SCHL.ToString();
                DataSet ds = new DataSet();
                ds = AbstractLayer.SchoolDB.ReExamTermStudentsCountRecordsClassWise(Search, loginSession.SCHL.ToString());
                if ((ds == null) || (ds.Tables[0].Rows.Count == 0 && ds.Tables[1].Rows.Count == 0))
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.SR = ViewBag.MR = ViewBag.TotalCount = 0;
                    return View(fhvm);
                }
                else
                {
                    ViewBag.MR = ds.Tables[0].Rows[0]["MR"].ToString();
                    ViewBag.SR = ds.Tables[1].Rows[0]["SR"].ToString();
                }

                ViewBag.SearchId = id;
                string date = DateTime.Now.ToString("dd/MM/yyyy");
                if (!string.IsNullOrEmpty(id))
                {
                    fhvm.StoreAllData = AbstractLayer.SchoolDB.ReExamTermStudentCalculateFee(id, date, Search, loginSession.SCHL.ToString());

                    if (fhvm.StoreAllData == null || fhvm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        ViewData["FeeStatus"] = "3";
                    }
                    else
                    {
                        ViewData["FeeStatus"] = "1";
                        if (Session["ReExamTermStudentCalculateFee"] != null)
                        {
                            Session["ReExamTermStudentCalculateFee"] = null;
                        }

                        Session["ReExamTermStudentCalculateFee"] = fhvm.StoreAllData;
                        ViewBag.TotalCount = fhvm.StoreAllData.Tables[0].Rows.Count;
                        fhvm.TotalFeesInWords = fhvm.StoreAllData.Tables[1].Rows[0]["TotalFeesInWords"].ToString();
                        fhvm.EndDate = fhvm.StoreAllData.Tables[0].Rows[0]["EndDateDay"].ToString() + " " + fhvm.StoreAllData.Tables[0].Rows[0]["FeeValidDate"].ToString();
                    }
                }
                else
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    ViewData["FeeStatus"] = "5";
                }
                return View(fhvm);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }
        }


        [SessionCheckFilter]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ReExamTermStudentPaymentForm()
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];

            try
            {

                PaymentformViewModel pfvm = new PaymentformViewModel();
                if (Session["ReExamTermStudentCalculateFee"] == null || Session["ReExamTermStudentCalculateFee"].ToString() == "")
                {
                    return RedirectToAction("ReExamTermStudentCalculateFee", " School");
                }

                // ViewBag.BankList = objCommon.GetBankList();
                string schl = loginSession.SCHL;

                pfvm.LOTNo = Convert.ToInt32(1);
                pfvm.Dist = loginSession.DIST.ToString();
                pfvm.District = loginSession.DIST.ToString(); ;
                pfvm.DistrictFull = loginSession.DIST.ToString();
                pfvm.SchoolCode = loginSession.SCHL.ToString();
                pfvm.SchoolName = loginSession.SCHLNME.ToString();
                ViewBag.TotalCount = 1;

                DataSet dscalFee = (DataSet)Session["ReExamTermStudentCalculateFee"];
                pfvm.TotalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalFee"].ToString());
                pfvm.TotalLateFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalLateFee"].ToString());
                pfvm.TotalFinalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalFeeAmount"].ToString());
                pfvm.TotalFeesInWords = dscalFee.Tables[1].Rows[0]["TotalFeesWords"].ToString();
                //  pfvm.FeeDate = Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy");
                pfvm.FeeDate = dscalFee.Tables[0].Rows[0]["FeeValidDateFormat"].ToString();
                pfvm.OfflineLastDate = dscalFee.Tables[0].Rows[0]["OfflineLastDate"].ToString();
                pfvm.StartDate = dscalFee.Tables[0].Rows[0]["FeeStartDate"].ToString();
                //TotalCandidates
                pfvm.TotalCandidates = Convert.ToInt32(dscalFee.Tables[0].AsEnumerable().Sum(x => x.Field<int>("CountStudents")));
                pfvm.FeeCode = dscalFee.Tables[0].Rows[0]["FeeCode"].ToString();
                pfvm.FeeCategory = dscalFee.Tables[0].Rows[0]["FEECAT"].ToString();
                Session["ReExamTermStudentPaymentForm"] = pfvm;

                // new add AllowBanks by rohit
                pfvm.AllowBanks = dscalFee.Tables[0].Rows[0]["AllowBanks"].ToString();
                string[] bls = pfvm.AllowBanks.Split(',');
                // BankList bl = new BankList();
                BankModels BM = new BankModels();
                pfvm.bankList = new List<BankListModel>();
                for (int b = 0; b < bls.Count(); b++)
                {
                    int OutStatus;
                    BM.BCODE = bls[b].ToString();
                    DataSet ds1 = new AbstractLayer.BankDB().GetBankDataByBCODE(BM, out OutStatus);
                    BM.BANKNAME = ds1.Tables[0].Rows[0]["BANKNAME"].ToString();
                    pfvm.bankList.Add(new BankListModel { BCode = BM.BCODE, BankName = BM.BANKNAME, Img = "" });
                }
                ///////////////



                if (pfvm.TotalFinalFees == 0 && pfvm.TotalFees == 0)
                {
                    ViewBag.CheckForm = 1; // only verify for M1 and T1 
                    TempData["ReExamTermStudentCheckFormFee"] = 0;
                }
                else
                {
                    ViewBag.CheckForm = 0; // only verify for M1 and T1 
                    TempData["ReExamTermStudentCheckFormFee"] = 1;
                }
                return View(pfvm);

            }
            catch (Exception ex)
            {
                return RedirectToAction("ReExamTermStudentCalculateFee", "School");
            }
        }


        [SessionCheckFilter]
        [HttpPost]
        public ActionResult ReExamTermStudentPaymentForm(PaymentformViewModel pfvm, FormCollection frm, string PayModValue, string AllowBanks, string IsOnline)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];

            try
            {
                ChallanMasterModel CM = new ChallanMasterModel();
                if (AllowBanks == null)
                {
                    AllowBanks = pfvm.BankCode;
                }
                else
                {
                    pfvm.BankCode = AllowBanks;
                }

                if (pfvm.BankCode == null)
                {
                    ViewBag.Message = "Please Select Bank";
                    ViewData["SelectBank"] = "1";
                    return View(pfvm);
                }
                //if (Session["ReExamTermStudentCheckFormFee"].ToString() == "0")
                //{ pfvm.BankCode = "203"; }


                if (Session["ReExamTermStudentPaymentForm"] == null || Session["ReExamTermStudentPaymentForm"].ToString() == "")
                {
                    return RedirectToAction("ReExamTermStudentPaymentForm", "School");
                }
                if (Session["ReExamTermStudent_FeeStudentList"] == null || Session["ReExamTermStudent_FeeStudentList"].ToString() == "")
                {
                    return RedirectToAction("ReExamTermStudentPaymentForm", "School");
                }

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
                }
                else if (AllowBanks == "203")
                {
                    PayModValue = "hod";
                    bankName = "PSEB HOD";
                    CM.FEEMODE = "CASH";
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
                    CM.FEEMODE = "CASH";
                }
                pfvm.BankName = bankName;


                if (ModelState.IsValid)
                {
                    string SCHL = loginSession.SCHL;
                    string FeeStudentList = Session["ReExamTermStudent_FeeStudentList"].ToString();
                    CM.FeeStudentList = FeeStudentList.Remove(FeeStudentList.LastIndexOf(","), 1);
                    PaymentformViewModel PFVMSession = (PaymentformViewModel)Session["ReExamTermStudentPaymentForm"];
                    // new add AllowBanks by rohit
                    //pfvm.AllowBanks = dscalFee.Tables[0].Rows[0]["AllowBanks"].ToString();
                    string[] bls = PFVMSession.AllowBanks.Split(',');
                    // BankList bl = new BankList();
                    BankModels BM = new BankModels();
                    PFVMSession.bankList = new List<BankListModel>();
                    for (int b = 0; b < bls.Count(); b++)
                    {
                        int OutStatus;
                        BM.BCODE = bls[b].ToString();
                        DataSet ds1 = new AbstractLayer.BankDB().GetBankDataByBCODE(BM, out OutStatus);
                        BM.BANKNAME = ds1.Tables[0].Rows[0]["BANKNAME"].ToString();
                        PFVMSession.bankList.Add(new BankListModel { BCode = BM.BCODE, BankName = BM.BANKNAME, Img = "" });
                    }
                    ///////////////


                    CM.FEE = Convert.ToInt32(PFVMSession.TotalFinalFees);
                    CM.TOTFEE = Convert.ToInt32(PFVMSession.TotalFinalFees);
                    CM.regfee = Convert.ToInt32(PFVMSession.TotalFees);
                    CM.latefee = Convert.ToInt32(PFVMSession.TotalLateFees);
                    CM.FEECAT = PFVMSession.FeeCategory;
                    CM.FEECODE = PFVMSession.FeeCode;
                    //CM.FEEMODE = "CASH";
                    CM.BANK = pfvm.BankName;
                    CM.BCODE = pfvm.BankCode;
                    CM.BANKCHRG = PFVMSession.BankCharges;
                    CM.SchoolCode = PFVMSession.SchoolCode.ToString();
                    CM.DIST = PFVMSession.Dist.ToString();
                    CM.DISTNM = PFVMSession.District;
                    CM.LOT = PFVMSession.LOTNo;
                    //
                    CM.SCHLREGID = PFVMSession.SchoolCode.ToString();
                    CM.APPNO = PFVMSession.SchoolCode.ToString();
                    //
                    CM.type = "schle";
                    DateTime CHLNVDATE2;
                    CM.CHLNVDATE = PFVMSession.FeeDate;
                    if (DateTime.TryParseExact(PFVMSession.FeeDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out CHLNVDATE2))
                    {
                        CM.ChallanVDateN = CHLNVDATE2;
                    }


                    if (AllowBanks == "202" || AllowBanks == "204")
                    {
                        if (Convert.ToDateTime(PFVMSession.OfflineLastDate).Date >= DateTime.Now.Date)
                        {
                            //  $("#divOffline").show();
                        }
                        else
                        {
                            ViewData["result"] = 20;
                            return RedirectToAction("ReExamTermStudentPaymentForm", "School");
                        }
                    }

                    string SchoolMobile = "";
                    // string result = "0";
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
                                return RedirectToAction("AtomCheckoutUrl", "Gateway", new { ChallanNo = TransactionID, amt = TransactionAmount, clientCode = clientCode, cmn = udf1CustName, cme = udf2CustEmail, cmno = udf3CustMob });

                            }
                            #endregion Payment Gateyway
                        }
                        else
                        {
                            ////{#var#} Challan no. {#var#} of Ref no. {#var#} successfully generated and valid till Dt {#var#}. Regards PSEB
                            string Sms = "Your Challan no. " + result + " of Ref no  " + CM.LOT + " successfully generated and valid till Dt " + CM.CHLNVDATE + ". Regards PSEB";
                            try
                            {
                                string getSms = new AbstractLayer.DBClass().gosms(SchoolMobile, Sms);
                                //string getSms = objCommon.gosms("9711819184", Sms);
                            }
                            catch (Exception) { }

                            ModelState.Clear();
                            //--For Showing Message---------//                   
                            return RedirectToAction("GenerateChallaan", "Home", new { ChallanId = result });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
            }
            return RedirectToAction("ReExamTermStudentPaymentForm", "School");
        }

        #endregion


        #endregion



        #region Begin SchoolBasedExams Portal
        [SessionCheckFilter]
        public ActionResult SchoolBasedExamsPortal()
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            TempData["ReAppearCCEMarksSearch"] = null;
            return View();
        }
        [SessionCheckFilter]
        public ActionResult SchoolBasedExamsAgree(string id)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            TempData["SchoolBasedExamsClass"] = id;
            return View();
        }

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult SchoolBasedExamsAgree(string id, FormCollection frm)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            try
            {
                string cc = id;
                string s = frm["Agree"].ToString();
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("SchoolBasedExamsPortal", "School");
                }
                else
                {
                    if (s == "Agree")
                    {
                        if (cc == "OMatric" || cc == "OSenior")
                        {
                            return RedirectToAction("SchoolBasedExamsMarksOpen", "School", new { id = cc });
                        }
                        else
                        {
                            return RedirectToAction("SchoolBasedExamsMarks", "School", new { id = cc });
                        }
                    }
                }
                return RedirectToAction("SchoolBasedExamsPortal", "School");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }
        }


        [SessionCheckFilter]
        public ActionResult SchoolBasedExamsMarks(FormCollection frm, string id, int? page)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            SchoolModels MS = new SchoolModels();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("SchoolBasedExamsPortal", "School");
                }

                string CLASS = id == "Senior" ? "4" : id == "Matric" ? "2" : "";
                ViewBag.schlCode = loginSession.SCHL;
                ViewBag.cid = id;

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                string Search = string.Empty;
                string SelectedAction = "0";


                var itemFilter = new SelectList(new[] { new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                    new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";
                //------------------------

                if (loginSession.SCHL != null)
                {

                    if (TempData["SchoolBasedExamsMarksSearch"] != null)
                    {
                        Search += TempData["SchoolBasedExamsMarksSearch"].ToString();
                        ViewBag.SelectedFilter = TempData["SelFilter"];
                        SelectedAction = TempData["SelAction"].ToString();
                        ViewBag.SelectedAction = TempData["SelActionValue"];

                        TempData["SelFilter"] = ViewBag.SelectedFilter;
                        TempData["SelAction"] = SelectedAction;
                        TempData["SelActionValue"] = ViewBag.SelectedAction;
                        TempData["SchoolBasedExamsMarksSearch"] = Search;
                    }
                    else
                    {
                        Search = "  a.schl = '" + loginSession.SCHL + "' and  a.class= '" + CLASS + "'";
                    }



                    MS.StoreAllData = _schoolRepository.GetSchoolBasedExamsMarksDataBySCHL(Search, loginSession.SCHL, pageIndex, CLASS, Convert.ToInt32(SelectedAction));
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsSchoolBasedExamsFilled"]);
                        }
                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                            ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[4].Rows[0]["FinalSubmitLastDate"];
                        }
                        if (MS.StoreAllData.Tables[5].Rows.Count > 0)
                        {
                            MS.schoolAllowForMarksEntry = new SchoolAllowForMarksEntry()
                            {

                                Cls = MS.StoreAllData.Tables[5].Rows[0]["Cls"].ToString(),
                                IsActive = Convert.ToInt32(MS.StoreAllData.Tables[5].Rows[0]["IsActive"].ToString()),
                                IsAllow = MS.StoreAllData.Tables[5].Rows[0]["IsAllow"].ToString(),
                                LastDate = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["LastdateDT"].ToString()),
                                Panel = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["Panel"].ToString())
                            };
                        }
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);

                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsSchoolBasedExamsFilled"]);
                        }

                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                            ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[4].Rows[0]["FinalSubmitLastDate"];
                        }
                        if (MS.StoreAllData.Tables[5].Rows.Count > 0)
                        {
                            MS.schoolAllowForMarksEntry = new SchoolAllowForMarksEntry()
                            {
                                Cls = MS.StoreAllData.Tables[5].Rows[0]["Cls"].ToString(),
                                IsActive = Convert.ToInt32(MS.StoreAllData.Tables[5].Rows[0]["IsActive"].ToString()),
                                IsAllow = MS.StoreAllData.Tables[5].Rows[0]["IsAllow"].ToString(),
                                LastDate = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["LastdateDT"].ToString()),
                                Panel = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["Panel"].ToString())
                            };
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

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult SchoolBasedExamsMarks(string id, FormCollection frm, int? page)
        {


            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            SchoolModels MS = new SchoolModels();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("SchoolBasedExamsPortal", "School");
                }
                string CLASS = id == "Senior" ? "4" : id == "Matric" ? "2" : "";
                ViewBag.schlCode = loginSession.SCHL;
                ViewBag.cid = id;

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                var itemFilter = new SelectList(new[] { new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                    new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";
                //------------------------

                if (loginSession.SCHL != null)
                {
                    string Search = "";
                    Search = "  a.SCHL = '" + loginSession.SCHL + "' and  a.class='" + CLASS + "' ";

                    int SelAction = 0;
                    if (frm["SelAction"] != "")
                    {
                        int SelValueSch = Convert.ToInt32(frm["SelAction"].ToString());
                        if (frm["SelAction"] != "")
                        {
                            if (SelValueSch == 3)
                            {
                                SelAction = 2;
                            }
                            //  { Search += " and  IsSchoolBasedExamsFilled=1 "; } // Filled
                            else if (SelValueSch == 2)
                            { SelAction = 1; }
                            //{ Search += " and (IsSchoolBasedExamsFilled is null or IsSchoolBasedExamsFilled=0) "; } // pending
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
                            { Search += " and b.Roll='" + frm["SearchString"].ToString() + "'"; }
                            if (SelValueSch == 2)
                            { Search += " and Std_id='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 3)
                            { Search += " and  Registration_num like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " and  Candi_Name like '%" + frm["SearchString"].ToString() + "%'"; }
                        }
                    }

                    TempData["SelFilter"] = frm["SelFilter"];
                    TempData["SelAction"] = SelAction;
                    TempData["SelActionValue"] = frm["SelAction"];
                    TempData["SchoolBasedExamsMarksSearch"] = Search;
                    // string class1 = "4";
                    MS.StoreAllData = _schoolRepository.GetSchoolBasedExamsMarksDataBySCHL(Search, loginSession.SCHL, pageIndex, CLASS, SelAction);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsSchoolBasedExamsFilled"]);
                        }
                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                            ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[4].Rows[0]["FinalSubmitLastDate"];
                        }
                        if (MS.StoreAllData.Tables[5].Rows.Count > 0)
                        {
                            MS.schoolAllowForMarksEntry = new SchoolAllowForMarksEntry()
                            {
                                Cls = MS.StoreAllData.Tables[5].Rows[0]["Cls"].ToString(),
                                IsActive = Convert.ToInt32(MS.StoreAllData.Tables[5].Rows[0]["IsActive"].ToString()),
                                IsAllow = MS.StoreAllData.Tables[5].Rows[0]["IsAllow"].ToString(),
                                LastDate = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["LastdateDT"].ToString()),
                                Panel = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["Panel"].ToString())
                            };
                        }
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsSchoolBasedExamsFilled"]);
                        }

                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                            ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[4].Rows[0]["FinalSubmitLastDate"];
                        }
                        if (MS.StoreAllData.Tables[5].Rows.Count > 0)
                        {
                            MS.schoolAllowForMarksEntry = new SchoolAllowForMarksEntry()
                            {
                                Cls = MS.StoreAllData.Tables[5].Rows[0]["Cls"].ToString(),
                                IsActive = Convert.ToInt32(MS.StoreAllData.Tables[5].Rows[0]["IsActive"].ToString()),
                                IsAllow = MS.StoreAllData.Tables[5].Rows[0]["IsAllow"].ToString(),
                                LastDate = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["LastdateDT"].ToString()),
                                Panel = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["Panel"].ToString())
                            };
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

        [SessionCheckFilter]
        [HttpPost]
        public JsonResult JqSchoolBasedExamsMarks(string stdid, string CandSubject, string cls)
        {

            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            var flag = 1;

            var objResponse1 = JsonConvert.DeserializeObject<List<CandSubjectReExamTermStudents>>(CandSubject);
            DataTable dtSub = new DataTable();
            dtSub.Columns.Add("CANDID");
            dtSub.Columns.Add("SUB");
            dtSub.Columns.Add("OBTTEST1");
            dtSub.Columns.Add("MAXTEST1");
            dtSub.Columns.Add("OBTTEST2");
            dtSub.Columns.Add("MAXTEST2");
            dtSub.Columns.Add("OBTTEST3");
            dtSub.Columns.Add("MAXTEST3");
            dtSub.Columns.Add("OBTTEST4");
            dtSub.Columns.Add("MAXTEST4");
            dtSub.Columns.Add("OBTTEST5");
            dtSub.Columns.Add("MAXTEST5");
            DataRow row = null;
            foreach (var rowObj in objResponse1)
            {
                row = dtSub.NewRow();
                if (rowObj.OBTTEST1 == "A" || rowObj.OBTTEST1 == "ABS")
                {
                    rowObj.OBTTEST1 = "ABS";
                }
                else if (rowObj.OBTTEST1 == "C" || rowObj.OBTTEST1 == "CAN")
                {
                    rowObj.OBTTEST1 = "CAN";
                }
                else if (!string.IsNullOrEmpty(rowObj.OBTTEST1))
                {
                    rowObj.OBTTEST1 = rowObj.OBTTEST1.PadLeft(3, '0');
                }

                if (rowObj.OBTTEST2 == "A" || rowObj.OBTTEST2 == "ABS")
                {
                    rowObj.OBTTEST2 = "ABS";
                }
                else if (rowObj.OBTTEST2 == "C" || rowObj.OBTTEST2 == "CAN")
                {
                    rowObj.OBTTEST2 = "CAN";
                }
                else if (!string.IsNullOrEmpty(rowObj.OBTTEST2))
                {
                    rowObj.OBTTEST2 = rowObj.OBTTEST2.PadLeft(3, '0');
                }
                //
                if (rowObj.OBTTEST3 == "A" || rowObj.OBTTEST3 == "ABS")
                {
                    rowObj.OBTTEST3 = "ABS";
                }
                else if (rowObj.OBTTEST3 == "C" || rowObj.OBTTEST3 == "CAN")
                {
                    rowObj.OBTTEST3 = "CAN";
                }
                else if (!string.IsNullOrEmpty(rowObj.OBTTEST3))
                {
                    rowObj.OBTTEST3 = rowObj.OBTTEST3.PadLeft(3, '0');
                }
                //
                if (rowObj.OBTTEST4 == "A" || rowObj.OBTTEST4 == "ABS")
                {
                    rowObj.OBTTEST4 = "ABS";
                }
                else if (rowObj.OBTTEST4 == "C" || rowObj.OBTTEST4 == "CAN")
                {
                    rowObj.OBTTEST4 = "CAN";
                }
                else if (!string.IsNullOrEmpty(rowObj.OBTTEST4))
                {
                    rowObj.OBTTEST4 = rowObj.OBTTEST4.PadLeft(3, '0');
                }
                //
                if (rowObj.OBTTEST5 == "A" || rowObj.OBTTEST5 == "ABS")
                {
                    rowObj.OBTTEST5 = "ABS";
                }
                else if (rowObj.OBTTEST5 == "C" || rowObj.OBTTEST5 == "CAN")
                {
                    rowObj.OBTTEST5 = "CAN";
                }
                else if (!string.IsNullOrEmpty(rowObj.OBTTEST5))
                {
                    rowObj.OBTTEST5 = rowObj.OBTTEST5.PadLeft(3, '0');
                }
                //if (rowObj.MINMARKS == "--" || rowObj.MINMARKS == "")
                //{
                //    rowObj.MINMARKS = "000";
                //}
                dtSub.Rows.Add(stdid, rowObj.SUB, rowObj.OBTTEST1, rowObj.MAXTEST1, rowObj.OBTTEST2, rowObj.MAXTEST2, rowObj.OBTTEST3, rowObj.MAXTEST3, rowObj.OBTTEST4, rowObj.MAXTEST4, rowObj.OBTTEST5, rowObj.MAXTEST5);
            }
            dtSub.AcceptChanges();


            foreach (DataRow dr1 in dtSub.Rows)
            {
                if (dr1["OBTTEST1"].ToString() == "" || dr1["OBTTEST1"].ToString() == "ABS" || dr1["OBTTEST1"].ToString() == "CAN")
                { }
                else if (dr1["OBTTEST1"].ToString() == "0" || dr1["OBTTEST1"].ToString().Contains("A") || dr1["OBTTEST1"].ToString().Contains("C"))
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
                    int obt = Convert.ToInt32(dr1["OBTTEST1"].ToString());
                    int max = Convert.ToInt32(dr1["MAXTEST1"].ToString());
                    if ((obt < 1) || (obt > max))
                    {
                        flag = -2;
                    }
                }
                //
                if (dr1["OBTTEST2"].ToString() == "" || dr1["OBTTEST2"].ToString() == "ABS" || dr1["OBTTEST2"].ToString() == "CAN")
                { }
                else if (dr1["OBTTEST2"].ToString() == "0" || dr1["OBTTEST2"].ToString().Contains("A") || dr1["OBTTEST2"].ToString().Contains("C"))
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
                    int obt = Convert.ToInt32(dr1["OBTTEST2"].ToString());
                    int max = Convert.ToInt32(dr1["MAXTEST2"].ToString());
                    if ((obt < 1) || (obt > max))
                    {
                        flag = -2;
                    }
                }
                //
                if (dr1["OBTTEST3"].ToString() == "" || dr1["OBTTEST3"].ToString() == "ABS" || dr1["OBTTEST3"].ToString() == "CAN")
                { }
                else if (dr1["OBTTEST3"].ToString() == "0" || dr1["OBTTEST3"].ToString().Contains("A") || dr1["OBTTEST3"].ToString().Contains("C"))
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
                    int obt = Convert.ToInt32(dr1["OBTTEST3"].ToString());
                    int max = Convert.ToInt32(dr1["MAXTEST3"].ToString());
                    if ((obt < 1) || (obt > max))
                    {
                        flag = -2;
                    }
                }
                //
                if (dr1["OBTTEST4"].ToString() == "" || dr1["OBTTEST4"].ToString() == "ABS" || dr1["OBTTEST4"].ToString() == "CAN")
                { }
                else if (dr1["OBTTEST4"].ToString() == "0" || dr1["OBTTEST4"].ToString().Contains("A") || dr1["OBTTEST4"].ToString().Contains("C"))
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
                    int obt = Convert.ToInt32(dr1["OBTTEST4"].ToString());
                    int max = Convert.ToInt32(dr1["MAXTEST4"].ToString());
                    if ((obt < 1) || (obt > max))
                    {
                        flag = -2;
                    }
                }
                //
                if (dr1["OBTTEST5"].ToString() == "" || dr1["OBTTEST5"].ToString() == "ABS" || dr1["OBTTEST5"].ToString() == "CAN")
                { }
                else if (dr1["OBTTEST5"].ToString() == "0" || dr1["OBTTEST5"].ToString().Contains("A") || dr1["OBTTEST5"].ToString().Contains("C"))
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
                    int obt = Convert.ToInt32(dr1["OBTTEST5"].ToString());
                    int max = Convert.ToInt32(dr1["MAXTEST5"].ToString());
                    if ((obt < 1) || (obt > max))
                    {
                        flag = -2;
                    }
                }
            }
            if (flag == 1)
            {
                string dee = "1";
                // string class1 = "5";
                string class1 = cls == "Senior" ? "4" : cls == "Matric" ? "2" : "5";
                int OutStatus = 0;
                dee = _schoolRepository.AllotSchoolBasedExamsMarks(loginSession.SCHL, stdid, dtSub, class1, out OutStatus);
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
        public ActionResult SchoolBasedExamsReport(string id)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            TempData["SchoolBasedExamsMarksSearch"] = null;
            SchoolModels MS = new SchoolModels();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("SchoolBasedExamsPortal", "School");
                }
                string CLASS = id == "Senior" ? "4" : id == "Matric" ? "2" : "";
                ViewBag.schlCode = loginSession.SCHL;
                ViewBag.cid = id;

                string Search = string.Empty;
                Search = "  a.schl = '" + loginSession.SCHL + "' and  a.class= '" + CLASS + "'";
                string OutError = "";
                MS.StoreAllData = _schoolRepository.SchoolBasedExamsMarksEntryReport(loginSession.SCHL, 0, Search, loginSession.SCHL, CLASS, out OutError);
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
                    ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["FIFSET"].ToString();
                    return View(MS);
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }

        [SessionCheckFilter]
        public ActionResult SchoolBasedExamsFinalReport(string id, FormCollection frm)
        {
            TempData["SchoolBasedExamsMarksSearch"] = null;
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            SchoolModels MS = new SchoolModels();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("SchoolBasedExamsPortal", "School");
                }
                string CLASS = id == "Senior" ? "4" : id == "Matric" ? "2" : "";
                ViewBag.schlCode = loginSession.SCHL;
                ViewBag.cid = id;

                string Search = string.Empty;
                Search = "  a.schl = '" + loginSession.SCHL + "' and  a.class= '" + CLASS + "'";
                string OutError = "";
                DataSet dsFinal = new DataSet();
                MS.StoreAllData = dsFinal = _schoolRepository.SchoolBasedExamsMarksEntryReport(loginSession.SCHL, 1, Search, loginSession.SCHL, CLASS, out OutError);
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
                    MS.StoreAllData = _schoolRepository.SchoolBasedExamsMarksEntryReport(loginSession.SCHL, 0, Search, loginSession.SCHL, CLASS, out OutError);
                    ViewBag.TotalCount = 0;
                    ViewBag.TotalCount1 = MS.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.SchoolBasedExamsDate = MS.StoreAllData.Tables[0].Rows[0]["SchoolBasedExamsDate"].ToString();
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
                    if (dsFinal.Tables[3].Rows.Count > 0)
                    {
                        MS.schoolAllowForMarksEntry = new SchoolAllowForMarksEntry()
                        {
                            Cls = dsFinal.Tables[3].Rows[0]["Cls"].ToString(),
                            IsActive = Convert.ToInt32(dsFinal.Tables[3].Rows[0]["IsActive"].ToString()),
                            IsAllow = dsFinal.Tables[3].Rows[0]["IsAllow"].ToString(),
                            LastDate = Convert.ToString(dsFinal.Tables[3].Rows[0]["LastdateDT"].ToString()),
                            Panel = Convert.ToString(dsFinal.Tables[3].Rows[0]["Panel"].ToString())
                        };
                    }
                    ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["FIFSET"].ToString();
                    ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[0].Rows[0]["FinalSubmitLastDate"];
                    return View(MS);
                }

                else
                {
                    ViewBag.IsAllowCCE = 2;
                    ViewBag.IsFinal = 0;
                    ViewBag.TotalCount1 = 0;
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;

                    ViewBag.SchoolName = MS.StoreAllData.Tables[1].Rows[0]["FullSchoolNameE"].ToString();
                    ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["FIFSET"].ToString();
                    ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[0].Rows[0]["FinalSubmitLastDate"];
                    return View(MS);
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult SchoolBasedExamsFinalReport(string id)
        {
            TempData["SchoolBasedExamsMarksSearch"] = null;
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            SchoolModels MS = new SchoolModels();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("SchoolBasedExamsPortal", "School");
                }
                string CLASS = id == "Senior" ? "4" : id == "Matric" ? "2" : "";
                ViewBag.schlCode = loginSession.SCHL;
                ViewBag.cid = id;
                string Search = string.Empty;
                string OutError = "";
                MS.StoreAllData = _schoolRepository.SchoolBasedExamsMarksEntryReport(loginSession.SCHL, 2, Search, loginSession.SCHL, CLASS, out OutError);
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
                    ViewBag.SchoolBasedExamsDate = MS.StoreAllData.Tables[0].Rows[0]["SchoolBasedExamsDate"].ToString();
                    ViewBag.SchoolName = MS.StoreAllData.Tables[1].Rows[0]["FullSchoolNameE"].ToString();
                    ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["FIFSET"].ToString();
                    return View(MS);
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }



        #region  Open

        [SessionCheckFilter]
        public ActionResult SchoolBasedExamsMarksOpen(FormCollection frm, string id, int? page)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            SchoolModels MS = new SchoolModels();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("SchoolBasedExamsPortal", "School");
                }

                string CLASS = id == "OSenior" ? "12" : id == "OMatric" ? "10" : "";
                ViewBag.schlCode = loginSession.SCHL;
                ViewBag.cid = id;

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                string Search = string.Empty;
                string SelectedAction = "0";


                var itemFilter = new SelectList(new[] { new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                    new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";
                //------------------------

                if (loginSession.SCHL != null)
                {

                    if (TempData["SchoolBasedExamsMarksSearch"] != null)
                    {
                        Search += TempData["SchoolBasedExamsMarksSearch"].ToString();
                        ViewBag.SelectedFilter = TempData["SelFilter"];
                        SelectedAction = TempData["SelAction"].ToString();
                        ViewBag.SelectedAction = TempData["SelActionValue"];

                        TempData["SelFilter"] = ViewBag.SelectedFilter;
                        TempData["SelAction"] = SelectedAction;
                        TempData["SelActionValue"] = ViewBag.SelectedAction;
                        TempData["SchoolBasedExamsMarksSearch"] = Search;
                    }
                    else
                    {
                        Search = "  a.schl = '" + loginSession.SCHL + "' and  a.class= '" + CLASS + "'";
                    }



                    MS.StoreAllData = _schoolRepository.GetSchoolBasedExamsMarksDataBySCHLOpen(Search, loginSession.SCHL, pageIndex, CLASS, Convert.ToInt32(SelectedAction));
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsSchoolBasedExamsFilled"]);
                        }
                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                            ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[4].Rows[0]["FinalSubmitLastDate"];
                        }
                        if (MS.StoreAllData.Tables[5].Rows.Count > 0)
                        {
                            MS.schoolAllowForMarksEntry = new SchoolAllowForMarksEntry()
                            {

                                Cls = MS.StoreAllData.Tables[5].Rows[0]["Cls"].ToString(),
                                IsActive = Convert.ToInt32(MS.StoreAllData.Tables[5].Rows[0]["IsActive"].ToString()),
                                IsAllow = MS.StoreAllData.Tables[5].Rows[0]["IsAllow"].ToString(),
                                LastDate = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["LastdateDT"].ToString()),
                                Panel = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["Panel"].ToString())
                            };
                        }
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);

                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsSchoolBasedExamsFilled"]);
                        }

                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                            ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[4].Rows[0]["FinalSubmitLastDate"];
                        }
                        if (MS.StoreAllData.Tables[5].Rows.Count > 0)
                        {
                            MS.schoolAllowForMarksEntry = new SchoolAllowForMarksEntry()
                            {
                                Cls = MS.StoreAllData.Tables[5].Rows[0]["Cls"].ToString(),
                                IsActive = Convert.ToInt32(MS.StoreAllData.Tables[5].Rows[0]["IsActive"].ToString()),
                                IsAllow = MS.StoreAllData.Tables[5].Rows[0]["IsAllow"].ToString(),
                                LastDate = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["LastdateDT"].ToString()),
                                Panel = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["Panel"].ToString())
                            };
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

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult SchoolBasedExamsMarksOpen(string id, FormCollection frm, int? page)
        {


            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            SchoolModels MS = new SchoolModels();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("SchoolBasedExamsPortal", "School");
                }

                string CLASS = id == "OSenior" ? "12" : id == "OMatric" ? "10" : "";
                ViewBag.schlCode = loginSession.SCHL;
                ViewBag.cid = id;

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                var itemFilter = new SelectList(new[] { new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
                    new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                ViewBag.SelectedFilter = "0";

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";
                //------------------------

                if (loginSession.SCHL != null)
                {
                    string Search = "";
                    Search = "  a.SCHL = '" + loginSession.SCHL + "' and  a.class='" + CLASS + "' ";

                    int SelAction = 0;
                    if (frm["SelAction"] != "")
                    {
                        int SelValueSch = Convert.ToInt32(frm["SelAction"].ToString());
                        if (frm["SelAction"] != "")
                        {
                            if (SelValueSch == 3)
                            {
                                SelAction = 2;
                            }
                            //  { Search += " and  IsSchoolBasedExamsFilled=1 "; } // Filled
                            else if (SelValueSch == 2)
                            { SelAction = 1; }
                            //{ Search += " and (IsSchoolBasedExamsFilled is null or IsSchoolBasedExamsFilled=0) "; } // pending
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
                            { Search += " and b.Roll='" + frm["SearchString"].ToString() + "'"; }
                            if (SelValueSch == 2)
                            { Search += " and Std_id='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 3)
                            { Search += " and  Registration_num like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " and  Candi_Name like '%" + frm["SearchString"].ToString() + "%'"; }
                        }
                    }

                    TempData["SelFilter"] = frm["SelFilter"];
                    TempData["SelAction"] = SelAction;
                    TempData["SelActionValue"] = frm["SelAction"];
                    TempData["SchoolBasedExamsMarksSearch"] = Search;
                    // string class1 = "4";
                    MS.StoreAllData = _schoolRepository.GetSchoolBasedExamsMarksDataBySCHLOpen(Search, loginSession.SCHL, pageIndex, CLASS, SelAction);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsSchoolBasedExamsFilled"]);
                        }
                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                            ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[4].Rows[0]["FinalSubmitLastDate"];
                        }
                        if (MS.StoreAllData.Tables[5].Rows.Count > 0)
                        {
                            MS.schoolAllowForMarksEntry = new SchoolAllowForMarksEntry()
                            {
                                Cls = MS.StoreAllData.Tables[5].Rows[0]["Cls"].ToString(),
                                IsActive = Convert.ToInt32(MS.StoreAllData.Tables[5].Rows[0]["IsActive"].ToString()),
                                IsAllow = MS.StoreAllData.Tables[5].Rows[0]["IsAllow"].ToString(),
                                LastDate = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["LastdateDT"].ToString()),
                                Panel = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["Panel"].ToString())
                            };
                        }
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        if (MS.StoreAllData.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.IsFinal = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsSchoolBasedExamsFilled"]);
                        }

                        if (MS.StoreAllData.Tables[4].Rows.Count > 0)
                        {
                            ViewBag.TotalPen = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["TotalPen"]);
                            ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[4].Rows[0]["FinalSubmitLastDate"];
                        }
                        if (MS.StoreAllData.Tables[5].Rows.Count > 0)
                        {
                            MS.schoolAllowForMarksEntry = new SchoolAllowForMarksEntry()
                            {
                                Cls = MS.StoreAllData.Tables[5].Rows[0]["Cls"].ToString(),
                                IsActive = Convert.ToInt32(MS.StoreAllData.Tables[5].Rows[0]["IsActive"].ToString()),
                                IsAllow = MS.StoreAllData.Tables[5].Rows[0]["IsAllow"].ToString(),
                                LastDate = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["LastdateDT"].ToString()),
                                Panel = Convert.ToString(MS.StoreAllData.Tables[5].Rows[0]["Panel"].ToString())
                            };
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

        [SessionCheckFilter]
        [HttpPost]
        public JsonResult JqSchoolBasedExamsMarksOpen(string stdid, string CandSubject, string cls)
        {

            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            var flag = 1;

            var objResponse1 = JsonConvert.DeserializeObject<List<CandSubjectReExamTermStudents>>(CandSubject);
            DataTable dtSub = new DataTable();
            dtSub.Columns.Add("CANDID");
            dtSub.Columns.Add("SUB");
            dtSub.Columns.Add("OBTTEST1");
            dtSub.Columns.Add("MAXTEST1");
            dtSub.Columns.Add("OBTTEST2");
            dtSub.Columns.Add("MAXTEST2");
            dtSub.Columns.Add("OBTTEST3");
            dtSub.Columns.Add("MAXTEST3");
            dtSub.Columns.Add("OBTTEST4");
            dtSub.Columns.Add("MAXTEST4");
            dtSub.Columns.Add("OBTTEST5");
            dtSub.Columns.Add("MAXTEST5");
            DataRow row = null;
            foreach (var rowObj in objResponse1)
            {
                row = dtSub.NewRow();
                if (rowObj.OBTTEST1 == "A" || rowObj.OBTTEST1 == "ABS")
                {
                    rowObj.OBTTEST1 = "ABS";
                }
                else if (rowObj.OBTTEST1 == "C" || rowObj.OBTTEST1 == "CAN")
                {
                    rowObj.OBTTEST1 = "CAN";
                }
                else if (!string.IsNullOrEmpty(rowObj.OBTTEST1))
                {
                    rowObj.OBTTEST1 = rowObj.OBTTEST1.PadLeft(3, '0');
                }

                if (rowObj.OBTTEST2 == "A" || rowObj.OBTTEST2 == "ABS")
                {
                    rowObj.OBTTEST2 = "ABS";
                }
                else if (rowObj.OBTTEST2 == "C" || rowObj.OBTTEST2 == "CAN")
                {
                    rowObj.OBTTEST2 = "CAN";
                }
                else if (!string.IsNullOrEmpty(rowObj.OBTTEST2))
                {
                    rowObj.OBTTEST2 = rowObj.OBTTEST2.PadLeft(3, '0');
                }
                //
                if (rowObj.OBTTEST3 == "A" || rowObj.OBTTEST3 == "ABS")
                {
                    rowObj.OBTTEST3 = "ABS";
                }
                else if (rowObj.OBTTEST3 == "C" || rowObj.OBTTEST3 == "CAN")
                {
                    rowObj.OBTTEST3 = "CAN";
                }
                else if (!string.IsNullOrEmpty(rowObj.OBTTEST3))
                {
                    rowObj.OBTTEST3 = rowObj.OBTTEST3.PadLeft(3, '0');
                }
                //
                if (rowObj.OBTTEST4 == "A" || rowObj.OBTTEST4 == "ABS")
                {
                    rowObj.OBTTEST4 = "ABS";
                }
                else if (rowObj.OBTTEST4 == "C" || rowObj.OBTTEST4 == "CAN")
                {
                    rowObj.OBTTEST4 = "CAN";
                }
                else if (!string.IsNullOrEmpty(rowObj.OBTTEST4))
                {
                    rowObj.OBTTEST4 = rowObj.OBTTEST4.PadLeft(3, '0');
                }
                //
                if (rowObj.OBTTEST5 == "A" || rowObj.OBTTEST5 == "ABS")
                {
                    rowObj.OBTTEST5 = "ABS";
                }
                else if (rowObj.OBTTEST5 == "C" || rowObj.OBTTEST5 == "CAN")
                {
                    rowObj.OBTTEST5 = "CAN";
                }
                else if (!string.IsNullOrEmpty(rowObj.OBTTEST5))
                {
                    rowObj.OBTTEST5 = rowObj.OBTTEST5.PadLeft(3, '0');
                }
                //if (rowObj.MINMARKS == "--" || rowObj.MINMARKS == "")
                //{
                //    rowObj.MINMARKS = "000";
                //}
                dtSub.Rows.Add(stdid, rowObj.SUB, rowObj.OBTTEST1, rowObj.MAXTEST1, rowObj.OBTTEST2, rowObj.MAXTEST2, rowObj.OBTTEST3, rowObj.MAXTEST3, rowObj.OBTTEST4, rowObj.MAXTEST4, rowObj.OBTTEST5, rowObj.MAXTEST5);
            }
            dtSub.AcceptChanges();


            foreach (DataRow dr1 in dtSub.Rows)
            {
                if (dr1["OBTTEST1"].ToString() == "" || dr1["OBTTEST1"].ToString() == "ABS" || dr1["OBTTEST1"].ToString() == "CAN")
                { }
                else if (dr1["OBTTEST1"].ToString() == "0" || dr1["OBTTEST1"].ToString().Contains("A") || dr1["OBTTEST1"].ToString().Contains("C"))
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
                    int obt = Convert.ToInt32(dr1["OBTTEST1"].ToString());
                    int max = Convert.ToInt32(dr1["MAXTEST1"].ToString());
                    if ((obt < 1) || (obt > max))
                    {
                        flag = -2;
                    }
                }
                //
                if (dr1["OBTTEST2"].ToString() == "" || dr1["OBTTEST2"].ToString() == "ABS" || dr1["OBTTEST2"].ToString() == "CAN")
                { }
                else if (dr1["OBTTEST2"].ToString() == "0" || dr1["OBTTEST2"].ToString().Contains("A") || dr1["OBTTEST2"].ToString().Contains("C"))
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
                    int obt = Convert.ToInt32(dr1["OBTTEST2"].ToString());
                    int max = Convert.ToInt32(dr1["MAXTEST2"].ToString());
                    if ((obt < 1) || (obt > max))
                    {
                        flag = -2;
                    }
                }
                //
                if (dr1["OBTTEST3"].ToString() == "" || dr1["OBTTEST3"].ToString() == "ABS" || dr1["OBTTEST3"].ToString() == "CAN")
                { }
                else if (dr1["OBTTEST3"].ToString() == "0" || dr1["OBTTEST3"].ToString().Contains("A") || dr1["OBTTEST3"].ToString().Contains("C"))
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
                    int obt = Convert.ToInt32(dr1["OBTTEST3"].ToString());
                    int max = Convert.ToInt32(dr1["MAXTEST3"].ToString());
                    if ((obt < 1) || (obt > max))
                    {
                        flag = -2;
                    }
                }
                //
                if (dr1["OBTTEST4"].ToString() == "" || dr1["OBTTEST4"].ToString() == "ABS" || dr1["OBTTEST4"].ToString() == "CAN")
                { }
                else if (dr1["OBTTEST4"].ToString() == "0" || dr1["OBTTEST4"].ToString().Contains("A") || dr1["OBTTEST4"].ToString().Contains("C"))
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
                    int obt = Convert.ToInt32(dr1["OBTTEST4"].ToString());
                    int max = Convert.ToInt32(dr1["MAXTEST4"].ToString());
                    if ((obt < 1) || (obt > max))
                    {
                        flag = -2;
                    }
                }
                //
                if (dr1["OBTTEST5"].ToString() == "" || dr1["OBTTEST5"].ToString() == "ABS" || dr1["OBTTEST5"].ToString() == "CAN")
                { }
                else if (dr1["OBTTEST5"].ToString() == "0" || dr1["OBTTEST5"].ToString().Contains("A") || dr1["OBTTEST5"].ToString().Contains("C"))
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
                    int obt = Convert.ToInt32(dr1["OBTTEST5"].ToString());
                    int max = Convert.ToInt32(dr1["MAXTEST5"].ToString());
                    if ((obt < 1) || (obt > max))
                    {
                        flag = -2;
                    }
                }
            }
            if (flag == 1)
            {
                string dee = "1";
                // string class1 = "5";
                string class1 = cls == "OSenior" ? "12" : cls == "OMatric" ? "10" : "";
                int OutStatus = 0;
                dee = _schoolRepository.AllotSchoolBasedExamsMarksOpen(loginSession.SCHL, stdid, dtSub, class1, out OutStatus);
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
        public ActionResult SchoolBasedExamsReportOpen(string id)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            TempData["SchoolBasedExamsMarksSearch"] = null;
            SchoolModels MS = new SchoolModels();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("SchoolBasedExamsPortal", "School");
                }

                string CLASS = id == "OSenior" ? "12" : id == "OMatric" ? "10" : "";
                ViewBag.schlCode = loginSession.SCHL;
                ViewBag.cid = id;

                string Search = string.Empty;
                Search = "  a.schl = '" + loginSession.SCHL + "' and  a.class= '" + CLASS + "'";
                string OutError = "";
                MS.StoreAllData = _schoolRepository.SchoolBasedExamsMarksEntryReportOpen(loginSession.SCHL, 0, Search, loginSession.SCHL, CLASS, out OutError);
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
                    ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["FIFSET"].ToString();
                    return View(MS);
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }

        [SessionCheckFilter]
        public ActionResult SchoolBasedExamsFinalReportOpen(string id, FormCollection frm)
        {
            TempData["SchoolBasedExamsMarksSearch"] = null;
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            SchoolModels MS = new SchoolModels();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("SchoolBasedExamsPortal", "School");
                }

                string CLASS = id == "OSenior" ? "12" : id == "OMatric" ? "10" : "";
                ViewBag.schlCode = loginSession.SCHL;
                ViewBag.cid = id;

                string Search = string.Empty;
                Search = "  a.schl = '" + loginSession.SCHL + "' and  a.class= '" + CLASS + "'";
                string OutError = "";
                DataSet dsFinal = new DataSet();
                MS.StoreAllData = dsFinal = _schoolRepository.SchoolBasedExamsMarksEntryReportOpen(loginSession.SCHL, 1, Search, loginSession.SCHL, CLASS, out OutError);
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
                    MS.StoreAllData = _schoolRepository.SchoolBasedExamsMarksEntryReportOpen(loginSession.SCHL, 0, Search, loginSession.SCHL, CLASS, out OutError);
                    ViewBag.TotalCount = 0;
                    ViewBag.TotalCount1 = MS.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.SchoolBasedExamsDate = MS.StoreAllData.Tables[0].Rows[0]["SchoolBasedExamsDate"].ToString();
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
                    if (dsFinal.Tables[3].Rows.Count > 0)
                    {
                        MS.schoolAllowForMarksEntry = new SchoolAllowForMarksEntry()
                        {
                            Cls = dsFinal.Tables[3].Rows[0]["Cls"].ToString(),
                            IsActive = Convert.ToInt32(dsFinal.Tables[3].Rows[0]["IsActive"].ToString()),
                            IsAllow = dsFinal.Tables[3].Rows[0]["IsAllow"].ToString(),
                            LastDate = Convert.ToString(dsFinal.Tables[3].Rows[0]["LastdateDT"].ToString()),
                            Panel = Convert.ToString(dsFinal.Tables[3].Rows[0]["Panel"].ToString())
                        };
                    }
                    ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["FIFSET"].ToString();
                    ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[0].Rows[0]["FinalSubmitLastDate"];
                    return View(MS);
                }

                else
                {
                    ViewBag.IsAllowCCE = 2;
                    ViewBag.IsFinal = 0;
                    ViewBag.TotalCount1 = 0;
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;

                    ViewBag.SchoolName = MS.StoreAllData.Tables[1].Rows[0]["FullSchoolNameE"].ToString();
                    ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["FIFSET"].ToString();
                    ViewBag.FinalSubmitLastDate = MS.StoreAllData.Tables[0].Rows[0]["FinalSubmitLastDate"];
                    return View(MS);
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult SchoolBasedExamsFinalReportOpen(string id)
        {
            TempData["SchoolBasedExamsMarksSearch"] = null;
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            SchoolModels MS = new SchoolModels();
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("SchoolBasedExamsPortal", "School");
                }

                string CLASS = id == "OSenior" ? "12" : id == "OMatric" ? "10" : "";
                ViewBag.schlCode = loginSession.SCHL;
                ViewBag.cid = id;
                string Search = string.Empty;
                string OutError = "";
                MS.StoreAllData = _schoolRepository.SchoolBasedExamsMarksEntryReportOpen(loginSession.SCHL, 2, Search, loginSession.SCHL, CLASS, out OutError);
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
                    ViewBag.SchoolBasedExamsDate = MS.StoreAllData.Tables[0].Rows[0]["SchoolBasedExamsDate"].ToString();
                    ViewBag.SchoolName = MS.StoreAllData.Tables[1].Rows[0]["FullSchoolNameE"].ToString();
                    ViewBag.SET = MS.StoreAllData.Tables[1].Rows[0]["FIFSET"].ToString();
                    return View(MS);
                }
            }
            catch (Exception ex)
            {

            }
            return View(MS);

        }
        #endregion


        #endregion  End SchoolBasedExams Portal

        #region Infrasture Performa

     

        public ActionResult Admin_School_Infrastructure()
        {
            try
            {
                if (Session["AdminId"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    // Dist Allowed
                    string DistAllow = "";
                    if (ViewBag.DistAllow == null)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        DistAllow = ViewBag.DistAllow;
                    }
                    if (ViewBag.DistUser == null)
                    { ViewBag.MyDist = null; }
                    else
                    {
                        ViewBag.MyDist = ViewBag.DistUser;
                    }
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

        [HttpPost]
        public ActionResult Admin_School_Infrastructure(FormCollection frm)
        {
            try
            {
                if (Session["AdminId"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    string DistAllow = "";
                    if (ViewBag.DistAllow == null)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        DistAllow = ViewBag.DistAllow;
                    }
                    if (ViewBag.DistUser == null)
                    { ViewBag.MyDist = null; }
                    else
                    {
                        ViewBag.MyDist = ViewBag.DistUser;
                    }
                    string sSCHL = frm["SearchString"];
                    string sDIST = frm["Dist1"];
                    if (sSCHL == "")
                    {
                        sSCHL = "all";
                    }
                    if (sDIST.ToLower() == "")
                    {
                        sDIST = "all";
                    }

                    if (frm["Dist1"] != "")
                    {
                        ViewBag.SelectedDist = frm["Dist1"];
                    }

                    DataTable dt = new DataTable();
                    dt = new AbstractLayer.SchoolDB().GetInfrasturePerformaBySCHLListSearch(sSCHL, sDIST);

                    SchoolModels asm = new SchoolModels();

                    asm.StoreAllData = new DataSet();
                    asm.StoreAllData.Tables.Add(dt);
                    ViewBag.TotalCount = dt.Rows.Count;
                    string SCHL = Convert.ToString(Session["SCHL"]);
                    if (SCHL != "")
                    {
                        //LoginSession loginSession = (LoginSession)Session["LoginSession"];

                        //oInfrasturePerformas = await new AbstractLayer.SchoolDB().GetInfrasturePerformaBySCHLList(loginSession);
                        //ipm.ipf = oInfrasturePerformas;
                        //DataSet ds = new DataSet();
                        //SchoolModels sm = objDB.GetSchoolDataBySchl(SCHL, out ds);
                        //ipm.schlmodel = sm;
                    }

                    return View(asm);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        public async Task<ActionResult> ViewInfrasturePerformaAdminView(string SCHL)
        {
            InfrasturePerformasviewModel ipm = new InfrasturePerformasviewModel();
            try
            {
                if (Session["AdminId"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (SCHL != "")
                {
                    LoginSession loginSession = new LoginSession();
                    loginSession.SCHL = SCHL;
                    InfrasturePerformasList oInfrasturePerformas = new InfrasturePerformasList();
                    oInfrasturePerformas = await new AbstractLayer.SchoolDB().GetInfrasturePerformaBySCHLList(loginSession);
                    ipm.ipf = oInfrasturePerformas;
                    DataSet ds = new DataSet();
                    SchoolModels sm = objDB.GetSchoolDataBySchl(SCHL, out ds);
                    ipm.schlmodel = sm;
                }
            }
            catch (Exception ex)
            {

            }
            return View(ipm);
        }


        public async Task<ActionResult> BulkPrintInfrastructureForm()
        {
            try
            {
                if (Session["AdminId"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    // Dist Allowed
                    string DistAllow = "";
                    if (ViewBag.DistAllow == null)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        DistAllow = ViewBag.DistAllow;
                    }
                    if (ViewBag.DistUser == null)
                    { ViewBag.MyDist = null; }
                    else
                    {
                        ViewBag.MyDist = ViewBag.DistUser;
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
            ////IEnumerable<MedicalVending.Model.Models.Patient>
            //string SCHL = "";
            //string DIST = "";
            //         if (SCHL == "")
            //         {
            //             SCHL = "all";
            //         }
            //         if (DIST == "")
            //         {
            //             DIST = "all";
            //         }
            //         //IEnumerable<MedicalVending.Model.Models.Patient>
            //         List<InfrasturePerformasListWithSchool> ipm = new List<InfrasturePerformasListWithSchool>();
            //         try
            //         {
            //             //ipm = await new AbstractLayer.SchoolDB().GetInfrasturePerformaWithSchool(DIST, SCHL);
            //         }
            //         catch (Exception ex)
            //         {

            //         }
            //         return View(ipm);
        }

        [HttpPost]
        public async Task<ActionResult> BulkPrintInfrastructureForm(FormCollection frm)
        {

            string SCHL = "";
            string DIST = "";
            string TCODE = "";
            if(frm["SelectedTcode"] !=null)
            {
                ViewBag.SelectedTcode = frm["SelectedTcode"].ToString();
                TCODE = frm["SelectedTcode"].ToString();
            }
            if (frm["Dist1"] != null)
            {
                ViewBag.SelectedDist = frm["Dist1"].ToString();
                DIST = frm["Dist1"].ToString();
            }
            if (frm["SearchString"] != null)
            {
                SCHL = frm["SearchString"].ToString();
            }
            if (TCODE == "")
            {
                TCODE = "all";
            }
            if (SCHL == "")
            {
                SCHL = "all";
            }
            if (DIST == "")
            {
                DIST = "all";
            }
            //IEnumerable<MedicalVending.Model.Models.Patient>
            List<InfrasturePerformasListWithSchool> ipm = new List<InfrasturePerformasListWithSchool>();
            try
            {
                string DistAllow = "";
                if (ViewBag.DistAllow == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    DistAllow = ViewBag.DistAllow;
                }
                if (ViewBag.DistUser == null)
                { ViewBag.MyDist = null; }
                else
                {
                    ViewBag.MyDist = ViewBag.DistUser;
                }
                ipm = await new AbstractLayer.SchoolDB().GetInfrasturePerformaWithSchool(DIST, SCHL, TCODE);
            }
            catch (Exception ex)
            {

            }
            return View(ipm);
        }


        [SessionCheckFilter]
        public async Task<ActionResult> ViewInfrasturePerforma(InfrasturePerformasviewModel ipm)
        {
            try
            {
                string SCHL = Convert.ToString(Session["SCHL"]);
                if (SCHL != "")
                {
                    LoginSession loginSession = (LoginSession)Session["LoginSession"];
                    InfrasturePerformasList oInfrasturePerformas = new InfrasturePerformasList();
                    oInfrasturePerformas = await new AbstractLayer.SchoolDB().GetInfrasturePerformaBySCHLList(loginSession);
                    ipm.ipf = oInfrasturePerformas;
                    DataSet ds = new DataSet();
                    SchoolModels sm = objDB.GetSchoolDataBySchl(SCHL, out ds);
                    ipm.schlmodel = sm;
                }

            }
            catch (Exception ex)
            {

            }
            return View(ipm);
        }

        [SessionCheckFilter]
        public async Task<ActionResult> InfrasturePerforma(InfrasturePerformas ipm)
        {
            try
            {
                string SCHL = Convert.ToString(Session["SCHL"]);
                if (SCHL != "")
                {
                    LoginSession loginSession = (LoginSession)Session["LoginSession"];
                    ipm = await new AbstractLayer.SchoolDB().GetInfrasturePerformaBySCHL(loginSession);
                    DataSet ds = new DataSet();
                    ds = new AbstractLayer.SchoolDB().PanelEntryLastDate("InfrasturePerforma");
                    if (ds.Tables[1].Rows.Count > 0)
                    {
                        ViewData["lastDate"] = ds.Tables[1].Rows[0]["LastDate"].ToString();
                    }
                    else
                    {
                        ViewData["lastDate"] = "";
                    }
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        ViewData["lastDateOver"] = 1;
                    }
                    else
                    {
                        ViewData["lastDateOver"] = 0;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View(ipm);
        }

        [SessionCheckFilter]
        [HttpPost]
        public async Task<ActionResult> InfrasturePerforma(InfrasturePerformas ipm, string cmd)
        {
            try
            {
                DataSet ds = new DataSet();
                ds = new AbstractLayer.SchoolDB().PanelEntryLastDate("InfrasturePerforma");
                if (ds.Tables[1].Rows.Count > 0)
                {
                    ViewData["lastDate"] = ds.Tables[1].Rows[0]["LastDate"].ToString();
                }
                else
                {
                    ViewData["lastDate"] = "";
                }
                if (ds.Tables[0].Rows.Count == 0)
                {
                    ViewData["lastDateOver"] = 1;
                }
                else
                {
                    ViewData["lastDateOver"] = 0;
                }

                bool bCheck = true;
                if (ipm.IFSC1 == "")
                {
                    ViewData["result"] = "IFSC 1 is required";
                    bCheck = false;
                }
                else if (ipm.BankBranch1 == "")
                {
                    ViewData["result"] = "Bank Branch 1 is required";
                    bCheck = false;
                }

                if (new AbstractLayer.SchoolDB().IFSCCheck(ipm.IFSC1, ipm.Bank1) == false)
                {
                    ViewData["result"] = "IFSC 1 is not correct.";
                    bCheck = false;
                }
                if (ipm.IFSC2 != null)
                {
                    if (ipm.IFSC1.ToLower() == ipm.IFSC2.ToLower())
                    {
                        ViewData["result"] = "IFSC 1 and IFSC 2 should be diffrent.";
                        bCheck = false;
                    }
                    else if (new AbstractLayer.SchoolDB().IFSCCheck(ipm.IFSC2, ipm.Bank2) == false)
                    {
                        ViewData["result"] = "IFSC 2 is not correct.";
                        bCheck = false;
                    }
                }
                if (ipm.IFSC3 != null)
                {
                    if (ipm.IFSC3.ToLower() == ipm.IFSC2.ToLower())
                    {
                        ViewData["result"] = "IFSC 2 and IFSC 3 should be diffrent.";
                        bCheck = false;
                    }
                    else if (new AbstractLayer.SchoolDB().IFSCCheck(ipm.IFSC3, ipm.Bank3) == false)
                    {
                        ViewData["result"] = "IFSC 3 is not correct.";
                        bCheck = false;
                    }
                }

                if (bCheck == true)
                {
                    if (cmd == null)
                    {
                        return RedirectToAction("ViewInfrasturePerforma", "School");
                    }
                    else if (cmd.ToLower() == "save")
                    {
                        string SCHL = Convert.ToString(Session["SCHL"]);
                        if (SCHL != "")
                        {
                            int a = 0;
                            ipm.FinalSubmitStatus = 0;
                            ipm.FinalSubmitDate = null;
                            ipm = await new AbstractLayer.SchoolDB().UpdateInfrasturePerformaBySCHL(ipm, out a);
                            ViewData["result"] = a;
                        }
                    }
                    else if (cmd.ToLower() == "final submit")
                    {
                        string SCHL = Convert.ToString(Session["SCHL"]);
                        if (SCHL != "")
                        {
                            int a = 0;
                            ipm.FinalSubmitStatus = 1;
                            ipm.FinalSubmitDate = DateTime.Now.ToString();
                            ipm = await new AbstractLayer.SchoolDB().UpdateInfrasturePerformaBySCHL(ipm, out a);
                            ViewData["result"] = a;
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return View(ipm);
        }

        #endregion


        #region ExamCentreResources
        [SessionCheckFilter]
        public ActionResult ExamCentreResources(ExamCentreResources model)
        {
            ViewBag.YesNoList = objCommon.GetYesNoText();
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            model = new ExamCentreResources();

            if (string.IsNullOrEmpty(loginSession.EXAMCENT))
            {
                return RedirectToAction("Index", "Home");
            }    

            bool isExists = _context.ExamCentreResources.Where(s => s.schl == loginSession.SCHL).Count() > 0 ? true : false ;
            if (!isExists)
            {
                model.id = 0;
                model.schl = loginSession.SCHL;
            }
            else
            {
                model =  _context.ExamCentreResources.Where(s => s.schl == loginSession.SCHL).FirstOrDefault();
            }

            if (TempData["resultIns"] != null)
            {
                ViewData["resultIns"] = TempData["resultIns"];
            }

            return View(model);
        }


        [SessionCheckFilter]
        [HttpPost]
        public async Task<ActionResult> ExamCentreResources(ExamCentreResources model, string cmd, FormCollection fc)
        {
            ViewBag.YesNoList = objCommon.GetYesNoText();

            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            if (string.IsNullOrEmpty(loginSession.EXAMCENT))
            {
                return RedirectToAction("Index", "Home");
            }

            bool isExists = _context.ExamCentreResources.Where(s => s.schl == loginSession.SCHL).Count() > 0 ? true : false ;
            if (!isExists)
            {
                model.id = 0;
                model.schl = loginSession.SCHL;
            }

            try
            {

                if (cmd.ToLower().Contains("save") && model.id == 0 && !isExists)
                {

                    ExamCentreResources examCentreResources = new ExamCentreResources()
                    {
                         id = 0,
                         schl = model.schl,
                        internetAvailability = model.internetAvailability,
                        printerAvailability = model.printerAvailability,
                        perMinutePrintingSpeed = model.perMinutePrintingSpeed,
                        powerBackup = model.powerBackup,
                        photostateAvailability = model.photostateAvailability,
                        submitBy = model.schl,                       
                        submitOn = DateTime.Now   
                    };

                    _context.ExamCentreResources.Add(examCentreResources);
                    int insertedRecords = await _context.SaveChangesAsync();

                    if (insertedRecords > 0)
                    {
                        TempData["resultIns"] = "S";                       
                    }
                }

                else if (cmd.ToLower().Contains("update") && model.id > 0)
                {
                    using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                    {
                        try
                        {
                            ExamCentreResources examCentreResources = _context.ExamCentreResources.Find(model.id);                          
                            examCentreResources.internetAvailability = model.internetAvailability;
                            examCentreResources.printerAvailability = model.printerAvailability;
                            examCentreResources.perMinutePrintingSpeed = model.perMinutePrintingSpeed;
                            examCentreResources.powerBackup = model.powerBackup;
                            examCentreResources.photostateAvailability = model.photostateAvailability;
                            examCentreResources.modifiedBy = model.schl;
                            examCentreResources.modifiedOn = DateTime.Now; 
                            _context.Entry(examCentreResources).State = EntityState.Modified;
                            _context.SaveChanges();
                            TempData["resultIns"] = "M";

                            transaction.Commit();//transaction commit
                        }
                        catch (Exception ex1)
                        {

                            transaction.Rollback();
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                TempData["resultIns"] = "Error : " + ex.Message;
            }
            return RedirectToAction("ExamCentreResources", "School");
        }

        #endregion


        #region Exam Centre Confidential Resources
        [SessionCheckFilter]
        public ActionResult ExamCentreConfidentialResources(ExamCentreConfidentialResources model)
        {
            ViewBag.YesNoList = objCommon.GetYesNoText();
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            model = new ExamCentreConfidentialResources();

            if (string.IsNullOrEmpty(loginSession.EXAMCENT))
            {
                return RedirectToAction("Index", "Home");
            }

            model.id = 0;
            model.schl = loginSession.SCHL;
            ViewBag.schlnme = loginSession.SCHLNME;
            model.principal = loginSession.PRINCIPAL;
            model.mobile = loginSession.MOBILE;

            if (string.IsNullOrEmpty(loginSession.MOBILE))
            { ViewBag.MOBILE = loginSession.MOBILE; }
            else
            {              
                ViewBag.MOBILE = "xxxxxx" + loginSession.MOBILE.Substring(Math.Max(0, loginSession.MOBILE.Length - 4));
            }

            string FilepathExist = Path.Combine(Server.MapPath("~/ConfidentialFiles"));
            if (!Directory.Exists(FilepathExist))
            {
                Directory.CreateDirectory(FilepathExist);
            }

            string targetDirectory = Server.MapPath("~/ConfidentialFiles/");          
            string[] fileEntries = Directory.GetFiles(targetDirectory, "*.pdf", SearchOption.AllDirectories);
            model.confidentialFiles = fileEntries;

            if (TempData["resultIns"] != null)
            {
                ViewData["resultIns"] = TempData["resultIns"];
            }
            return View(model);
        }

        [HttpGet]
        public virtual ActionResult DownloadConfidentialFiles(string fileName)
        {
            if (Path.GetExtension(fileName) == ".pdf")
            {
                string targetDirectory = Server.MapPath("~/ConfidentialFiles/");
                string fullPath = Path.Combine(targetDirectory, fileName);

                return File(fullPath, "application/pdf", fileName);
            }
            else
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden);
            }           
        }
        #endregion


        #region Attendance Supervisory Staff Unlock
        public ActionResult AttendanceSupervisoryStaffUnlock()
        {

            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
            //ViewBag.Message = "Your application description page.";
            //return View();
        }
        [HttpPost]
        public ActionResult AttendanceSupervisoryStaffUnlock(ReportModel rp, string Class, string examDate, string Center)
        {
            try
            {
                //if (Session["UserName"] == null)
                //{
                //	return RedirectToAction("Logout", "Admin");
                //}
                ViewBag.ExamDate = examDate;
                ViewBag.Center = Center;
                examDate = " a.ExamDate='" + examDate + "' ";

                DataSet ds = new AbstractLayer.SchoolDB().AttendanceSupervisoryStaffUnlock(0, examDate, Center); // type=4 for summary report
                rp.StoreAllData = ds;
                if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                }
                return View(rp);
            }
            catch (Exception ex)
            {
                return View();
            }
            //ViewBag.Message = "Your application description page.";
            //return View();
        }
        #endregion

        #region REGOPENPracticalExamCentre

        public ActionResult PracticalExamCentreList()
        {

            var itemAction = new SelectList(new[] { new { ID = "1", Name = "Candid" }, new { ID = "2", Name = "Roll No" }, new { ID = "3", Name = "School Code" }, }, "ID", "Name", 1);
            ViewBag.MyAction = itemAction.ToList();
            ViewBag.SelectedAction = "0";
            
            //------------------------
            return View();
        }

        [HttpPost]
        public ActionResult PracticalExamCentreList(String Class, FormCollection frm, string cmd)
        {
            try
            {
                ViewBag.Class = Class;

                var itemAction = new SelectList(new[] { new { ID = "1", Name = "Candid" }, new { ID = "2", Name = "Roll No" }, new { ID = "3", Name = "School Code" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";
                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                SchoolModels rm = new SchoolModels();
                string Search = string.Empty;
                if (cmd == "Search")
                {
                    Search += "Roll is not null";

                    int SelAction = 0;
                    if (frm["SelAction"] != "")
                    {
                        ViewBag.SelectedAction = frm["SelAction"];
                        SelAction = Convert.ToInt32(frm["SelAction"].ToString());
                        if (frm["SelAction"] != "" && frm["SearchString"].ToString() != "")
                        {
                            if (SelAction == 1)
                            { Search += " and Candid='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelAction == 2)
                            { Search += " and Roll='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelAction == 3)
                            { Search += " and Pcent='" + frm["SearchString"].ToString() + "'"; }

                        }
                    }

                    rm.StoreAllData = objDB.PracticalExamCentreList(Class,Search);
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

        #endregion REGOPENPracticalExamCentre

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
