using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using PSEBONLINE.Models;
using System.Text;
using System.Data;
using System.Net;
using System.Web.Security;
using System.Collections.Generic;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Data.OleDb;
using ClosedXML.Excel;
using PSEBONLINE.Filters;
using System.Security.Cryptography;

namespace PSEBONLINE.Controllers
{
    public class DeoPortalController : Controller
    {

        private readonly DBContext _context = new DBContext();
        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.DEODB OBJDB = new AbstractLayer.DEODB();
        // GET: Test
        [Route("DeoPortal")]
        public ActionResult Index()
        {
            ViewBag.ExamCentreList = _context.BindDeoCentreMasterViews.ToList();
            return View();
        }
        [Route("DeoPortal")]
        [HttpPost]
        public ActionResult Index(DEOModel DM, string ExamCentre)
        {
            try
            {
                ViewBag.ExamCentreList = _context.BindDeoCentreMasterViews.ToList();

                if (!string.IsNullOrEmpty(ExamCentre))
                {
                    DataSet ds = OBJDB.CheckLogin(DM, ExamCentre); // 
                    if (ds == null)
                    {
                        ViewData["result"] = "-1";
                        return View();
                    }

                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        if (ds.Tables[1].Rows.Count > 0)
                        {
                            List<DeoMonthYearModel> list = new List<DeoMonthYearModel>();
                            list = AbstractLayer.StaticDB.DataTableToList<DeoMonthYearModel>(ds.Tables[1]).ToList();
                            Session["DeoMonthYear"] = list;
                        }

                        HttpContext.Session["NAME"] = dt.Rows[0]["NAME"].ToString();
                        HttpContext.Session["User"] = dt.Rows[0]["USER"].ToString();
                        HttpContext.Session["Dist"] = dt.Rows[0]["DIST"].ToString();
                        HttpContext.Session["UID"] = dt.Rows[0]["USERID"].ToString();
                        HttpContext.Session["DISTNM"] = dt.Rows[0]["DISTNM"].ToString();
                        HttpContext.Session["UTYPE"] = dt.Rows[0]["UTYPE"].ToString();
                        HttpContext.Session["DeoSession"] = dt.Rows[0]["DeoSession"].ToString();
                        HttpContext.Session["DeoLoginExamCentre"] = ExamCentre.ToString();

                        var DeoSessionMonthYear = _context.BindDeoCentreMasterViews.Where(s => s.ExamMonth.Trim() == ExamCentre).SingleOrDefault().DeoSessionMonthYear;
                        HttpContext.Session["DeoSessionMonthYear"] = DeoSessionMonthYear.ToString();


                        int deoFlag = Convert.ToInt32(dt.Rows[0]["ProfileFlag"].ToString());
                        if (deoFlag == 0)
                        {
                            return RedirectToAction("DeoProfileDetails", "DeoPortal");
                        }
                        else
                        {
                            Session["UTYPE"] = dt.Rows[0]["UTYPE"].ToString();
                            return RedirectToAction("DeoHome", "DeoPortal");
                        }
                    }
                    else
                    {
                        ViewData["result"] = "0";
                        return View();
                    }
                }
                else
                {
                    ViewData["result"] = "20";
                    return View();
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        //[Route("DeoPortal")]
        public ActionResult DeoHome()
        {
            return View();
        }
        public ActionResult Logout()
        {
            foreach (System.Collections.DictionaryEntry entry in HttpContext.Cache)
            {
                HttpContext.Cache.Remove((string)entry.Key);
            }
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            TempData.Clear();
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "DeoPortal");
            // return RedirectToAction("Index", "Home");

        }
        public ActionResult ExaminerSummaryReport(FormCollection frm)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN" || DeoUser == "admin")
                {
                    //string[] ArrInvoice = { "010","020","025","030","040","045","050","060","070","080" };
                    //string[] ArrInvoice = { "Invoice # 120", "Invoice # 121", "Invoice # 122", "Invoice # 123" };
                    //List<int> lstInvice = new List<int>();
                    //List<SelectListItem> DList = new List<SelectListItem>();
                    //foreach (string item in ArrInvoice)
                    //{
                    //    //int intInvocie = 0;
                    //    //Int32.TryParse(item.Split(',')[0], out intInvocie);
                    //    //if (intInvocie > 0)
                    //    //{
                    //    //    //lstInvice.Add(intInvocie);
                    //    //    DList.Add(new SelectListItem { Text = "yiy", Value = Convert.ToString(intInvocie)});
                    //    //}


                    //}
                    //string DC = "010,020,025,030,040,045,050,060,070,080";
                    //// Split string on spaces.
                    //// ... This will separate all the words.
                    //string[] DistCodes = DC.Split(',');
                    //foreach (string DistCode in DistCodes)
                    //{
                    //    DList.Add(new SelectListItem { Text = "yiy", Value = DistCode });
                    //}

                    //DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    //List<SelectListItem> DistList = new List<SelectListItem>();
                    //foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    //{
                    //    DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    //}

                    //ViewBag.Dist = DistList;

                    DEOModel obj = new DEOModel();
                    obj.StoreAllData = OBJDB.ExaminerSummaryReport();
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Examiner Dose not Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    }
                    return View(obj);
                }
                else
                {
                    //DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    //List<SelectListItem> DList = new List<SelectListItem>();
                    //foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    //{
                    //    DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    //}
                    //ViewBag.Dist = DList;
                }


                return View();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }
        public ActionResult DeoSummaryReport(FormCollection frm)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN" || DeoUser == "admin")
                {
                    //string[] ArrInvoice = { "010","020","025","030","040","045","050","060","070","080" };
                    //string[] ArrInvoice = { "Invoice # 120", "Invoice # 121", "Invoice # 122", "Invoice # 123" };
                    //List<int> lstInvice = new List<int>();
                    //List<SelectListItem> DList = new List<SelectListItem>();
                    //foreach (string item in ArrInvoice)
                    //{
                    //    //int intInvocie = 0;
                    //    //Int32.TryParse(item.Split(',')[0], out intInvocie);
                    //    //if (intInvocie > 0)
                    //    //{
                    //    //    //lstInvice.Add(intInvocie);
                    //    //    DList.Add(new SelectListItem { Text = "yiy", Value = Convert.ToString(intInvocie)});
                    //    //}


                    //}
                    //string DC = "010,020,025,030,040,045,050,060,070,080";
                    //// Split string on spaces.
                    //// ... This will separate all the words.
                    //string[] DistCodes = DC.Split(',');
                    //foreach (string DistCode in DistCodes)
                    //{
                    //    DList.Add(new SelectListItem { Text = "yiy", Value = DistCode });
                    //}

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;

                    DEOModel obj = new DEOModel();
                    obj.StoreAllData = OBJDB.DeoSummaryReport(Session["DeoLoginExamCentre"].ToString());
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Center Dose not Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    }
                    return View(obj);
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }


                return View();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }

        public ActionResult Change_Password()
        {
            string DeoUser = null;
            string district = null;

            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }
            DeoUser = Session["USER"].ToString();
            district = Session["Dist"].ToString();
            return View();
        }
        [HttpPost]
        public ActionResult Change_Password(FormCollection frm)
        {
            string DeoUser = null;
            string district = null;

            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }
            DeoUser = Session["USER"].ToString();
            district = Session["Dist"].ToString();

            string CurrentPassword = string.Empty;
            string NewPassword = string.Empty;


            if (frm["ConfirmPassword"] != "" && frm["NewPassword"] != "")
            {
                if (frm["ConfirmPassword"].ToString() == frm["NewPassword"].ToString())
                {
                    CurrentPassword = frm["CurrentPassword"].ToString();
                    NewPassword = frm["NewPassword"].ToString();


                    int result = OBJDB.DeoChangePassword(district, DeoUser, CurrentPassword, NewPassword); // passing Value to SchoolDB from model and Type 1 For regular
                    if (result > 0)
                    {
                        ViewData["resultDCP"] = 1;
                        return View();
                        // return RedirectToAction("Index", "DeoPortal");
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
        public ActionResult DeoProfileDetails(DEOModel DEO)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }
            HttpContext.Session["deoFlag"] = "";
            DeoUser = Session["USER"].ToString();
            district = Session["Dist"].ToString();

            DEO.StoreAllData = OBJDB.CHKDeoFlag(DeoUser, district);
            if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "DEO Dose not Exist";
                ViewBag.TotalCount = 0;
            }
            else
            {
                //int Pflag= Convert.ToInt32(DEO.StoreAllData.Tables[0].Rows[0]["ProfileFlag"]);
                //if(Pflag==1)
                //{
                HttpContext.Session["deoFlag"] = Convert.ToInt32(DEO.StoreAllData.Tables[0].Rows[0]["ProfileFlag"]);

                DEO.NAME = DEO.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                DEO.mailid = DEO.StoreAllData.Tables[0].Rows[0]["EMAILID"].ToString();
                DEO.Mobile = DEO.StoreAllData.Tables[0].Rows[0]["MOBILE"].ToString();
                DEO.STD = DEO.StoreAllData.Tables[0].Rows[0]["STD"].ToString();
                DEO.PHONE = DEO.StoreAllData.Tables[0].Rows[0]["PHONE"].ToString();

                DEO.PNAME1 = DEO.StoreAllData.Tables[0].Rows[0]["PNAME1"].ToString();
                DEO.PNAME2 = DEO.StoreAllData.Tables[0].Rows[0]["PNAME2"].ToString();
                DEO.PNAME3 = DEO.StoreAllData.Tables[0].Rows[0]["PNAME3"].ToString();
                DEO.PNAME4 = DEO.StoreAllData.Tables[0].Rows[0]["PNAME4"].ToString();
                DEO.PNAME5 = DEO.StoreAllData.Tables[0].Rows[0]["PNAME5"].ToString();

                DEO.PDESI1 = DEO.StoreAllData.Tables[0].Rows[0]["PDESI1"].ToString();
                DEO.PDESI2 = DEO.StoreAllData.Tables[0].Rows[0]["PDESI2"].ToString();
                DEO.PDESI3 = DEO.StoreAllData.Tables[0].Rows[0]["PDESI3"].ToString();
                DEO.PDESI4 = DEO.StoreAllData.Tables[0].Rows[0]["PDESI4"].ToString();
                DEO.PDESI5 = DEO.StoreAllData.Tables[0].Rows[0]["PDESI5"].ToString();

                //string a= DEO.StoreAllData.Tables[0].Rows[0]["PMOBILE1"].ToString();
                DEO.PMOBILE1 = DEO.StoreAllData.Tables[0].Rows[0]["PMOBILE1"].ToString();
                DEO.PMOBILE2 = DEO.StoreAllData.Tables[0].Rows[0]["PMOBILE2"].ToString();
                DEO.PMOBILE3 = DEO.StoreAllData.Tables[0].Rows[0]["PMOBILE3"].ToString();
                DEO.PMOBILE4 = DEO.StoreAllData.Tables[0].Rows[0]["PMBOILE4"].ToString();
                DEO.PMOBILE5 = DEO.StoreAllData.Tables[0].Rows[0]["PMOBILE5"].ToString();



                //}
                //else
                //{

                //}
            }

            try
            {

                //--------------------------FILL Designation----------
                DataSet PDesi = OBJDB.GetDesignation(); // passing Value to DeoClass from model            
                List<SelectListItem> PDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in PDesi.Tables[0].Rows)
                {
                    PDList.Add(new SelectListItem { Text = @dr["POSTNM"].ToString(), Value = @dr["POSTNM"].ToString() });
                }
                ViewBag.PDESIOne = PDList;
                ViewBag.PDESITwo = PDList;
                ViewBag.PDESIThree = PDList;
                ViewBag.PDESIFour = PDList;
                ViewBag.PDESIFive = PDList;
                //--------------------------End Designation----------

            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
            return View(DEO);
        }
        [HttpPost]
        public ActionResult DeoProfileDetails(DEOModel DEO, FormCollection frm)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;

            //id = encrypt.QueryStringModule.Decrypt(id);
            //CCODE = encrypt.QueryStringModule.Decrypt(CCODE);

            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }


                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();


                //--------------------------FILL Designation----------
                DataSet PDesi = OBJDB.GetDesignation(); // passing Value to DeoClass from model            
                List<SelectListItem> PDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in PDesi.Tables[0].Rows)
                {
                    PDList.Add(new SelectListItem { Text = @dr["POSTNM"].ToString(), Value = @dr["POSTNM"].ToString() });
                }
                ViewBag.PDESIOne = PDList;
                ViewBag.PDESITwo = PDList;
                ViewBag.PDESIThree = PDList;
                ViewBag.PDESIFour = PDList;
                ViewBag.PDESIFive = PDList;
                //--------------------------End Designation----------

                string name = DEO.NAME;
                string Email = DEO.mailid;
                string mob = DEO.Mobile;
                string std = DEO.STD;
                string phone = DEO.PHONE;

                string op1 = DEO.PNAME1;
                string op2 = DEO.PNAME2;
                string op3 = DEO.PNAME3;
                string op4 = DEO.PNAME4;
                string op5 = DEO.PNAME5;

                string Pdesi1 = DEO.PDESI1;
                string Pdesi2 = DEO.PDESI2;
                string Pdesi3 = DEO.PDESI3;
                string Pdesi4 = DEO.PDESI4;
                string Pdesi5 = DEO.PDESI5;

                string mob1 = DEO.PMOBILE1;
                string mob2 = DEO.PMOBILE2;
                string mob3 = DEO.PMOBILE3;
                string mob4 = DEO.PMOBILE4;
                string mob5 = DEO.PMOBILE5;




                string Staffres = OBJDB.UPDATEDEOPROFILE(DeoUser, district, name, Email, mob, std, phone, op1, op2, op3, op4, op5, Pdesi1, Pdesi2, Pdesi3, Pdesi4, Pdesi5, mob1, mob2, mob3, mob4, mob5);
                if (Staffres != "0")
                {
                    ViewData["result"] = "1";
                    HttpContext.Session["NAME"] = DEO.NAME;
                }
                else
                {
                    ViewData["result"] = "-1";
                }

                //if (DeoUser == "ADMIN")
                //{
                //    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                //    List<SelectListItem> DistList = new List<SelectListItem>();
                //    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                //    {
                //        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                //    }

                //    ViewBag.Dist = DistList;
                //}
                //else {
                //    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                //    List<SelectListItem> DList = new List<SelectListItem>();
                //    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                //    {
                //        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                //    }
                //    ViewBag.Dist = DList;
                //}


            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

            return View(DEO);
        }
        public ActionResult ExaminersReport(int? page)
        {
            //return View();
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    //string[] ArrInvoice = { "010","020","025","030","040","045","050","060","070","080" };
                    //string[] ArrInvoice = { "Invoice # 120", "Invoice # 121", "Invoice # 122", "Invoice # 123" };
                    //List<int> lstInvice = new List<int>();
                    //List<SelectListItem> DList = new List<SelectListItem>();
                    //foreach (string item in ArrInvoice)
                    //{
                    //    //int intInvocie = 0;
                    //    //Int32.TryParse(item.Split(',')[0], out intInvocie);
                    //    //if (intInvocie > 0)
                    //    //{
                    //    //    //lstInvice.Add(intInvocie);
                    //    //    DList.Add(new SelectListItem { Text = "yiy", Value = Convert.ToString(intInvocie)});
                    //    //}


                    //}
                    //string DC = "010,020,025,030,040,045,050,060,070,080";
                    //// Split string on spaces.
                    //// ... This will separate all the words.
                    //string[] DistCodes = DC.Split(',');
                    //foreach (string DistCode in DistCodes)
                    //{
                    //    DList.Add(new SelectListItem { Text = "yiy", Value = DistCode });
                    //}

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    //Search = "a.DIST = '" + district + "'";
                    Search = "DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }
                obj.StoreAllData = OBJDB.ExaminerReport(Search, Catg, pageIndex);
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Examiner Does not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    //ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    //int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    //int pn = tp / 10;
                    //int cal = 10 * pn;
                    //int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    //if (res >= 1)
                    //    ViewBag.pn = pn + 1;
                    //else
                    //    ViewBag.pn = pn;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }

        public ActionResult FinalPrint()
        {
            string DeoUser = null;
            string district = null;
            int udid = 0;
            string udid1 = null;
            string Catg = null;

            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }


                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                udid = Convert.ToInt32(Session["UID"].ToString());
                udid1 = Session["UID"].ToString();

                DataSet Fprint = OBJDB.FillFinalPrintLot(district, udid); // passing Value to DeoClass from model            
                List<SelectListItem> lotList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in Fprint.Tables[0].Rows)
                {
                    lotList.Add(new SelectListItem { Text = @dr["DateLot"].ToString(), Value = @dr["Lot"].ToString() });
                }
                ViewBag.Flot = lotList;
                //if (lotList.Count==0)
                //{
                //    ViewBag.Flot = "0";
                //}
                //else
                //{
                //    ViewBag.Flot = lotList;
                //}

                DEOModel obj = new DEOModel();
                obj.StoreAllData = OBJDB.FinalSubmitDeoPortalGrid(udid1);
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }

        [HttpPost]
        public ActionResult FinalPrint(DEOModel DEO, FormCollection frm, string cmd)
        {
            string DeoUser = null;
            string district = null;
            int udid = 0;
            string SelLot = frm["SelLot"];
            Session["SelLot"] = SelLot;
            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }
            DeoUser = Session["USER"].ToString();
            district = Session["Dist"].ToString();
            if (cmd == "Block Wise Staff List Report")
            {
                //ClusterWiseStaffListPrintLot(SelLot);
                return RedirectToAction("ClusterWiseStaffListPrintLot", "DeoPortal");

            }
            //if (cmd == "Cluster Wise Staff List Report")
            //{

            //    return RedirectToAction("ClusterWiseStaffListPrintLot", "DeoPortal");

            //}
            if (cmd == "Block Wise Centre List Report")
            {

                return RedirectToAction("ClusterWiseCentreListPrintLot", "DeoPortal");

            }
            if (cmd == "Cluster Report")
            {

                return RedirectToAction("ClusterListReportPrintlot", "DeoPortal");

            }
            return View(DEO);
        }
        public ActionResult ClusterListReportPrintlot()
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            int udid = 0;
            string SLot = null;
            if (Session["SelLot"].ToString() != null || Session["SelLot"].ToString() != "")
            {
                SLot = Session["SelLot"].ToString();
            }

            DEOModel obj = new DEOModel();
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                udid = Convert.ToInt32(Session["UID"].ToString());

                obj.StoreAllData = OBJDB.ClusterListReportPrintlot(district, SLot);
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.lot = obj.StoreAllData.Tables[0].Rows[0]["lot"].ToString();
                    ViewBag.fdate = obj.StoreAllData.Tables[0].Rows[0]["finaldate"].ToString();
                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }
        public ActionResult ClusterWiseStaffListPrintLot()
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            int udid = 0;
            string SLot = null;
            if (Session["SelLot"].ToString() != null || Session["SelLot"].ToString() != "")
            {
                SLot = Session["SelLot"].ToString();
            }

            DEOModel obj = new DEOModel();
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                udid = Convert.ToInt32(Session["UID"].ToString());

                DataSet Fprint = OBJDB.FillFinalPrintLot(district, udid); // passing Value to DeoClass from model            
                List<SelectListItem> lotList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in Fprint.Tables[0].Rows)
                {
                    lotList.Add(new SelectListItem { Text = @dr["DateLot"].ToString(), Value = @dr["Lot"].ToString() });
                }
                ViewBag.Flot = lotList;
                if (DeoUser.ToUpper() == "ADMIN")
                {

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                obj.StoreAllData = OBJDB.SelectClusterWiseStaffListReportLot(district, SLot);
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.lot = obj.StoreAllData.Tables[1].Rows[0]["lot"].ToString();
                    ViewBag.fdate = obj.StoreAllData.Tables[1].Rows[0]["finaldate"].ToString();
                    ViewBag.Month = obj.StoreAllData.Tables[0].Rows[0]["Month"].ToString();
                    ViewBag.Year = obj.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }
        public ActionResult ClusterWiseCentreListPrintLot()
        {
            string DeoUser = null;
            string district = null;
            string SLot = null;
            if (Session["SelLot"].ToString() != null || Session["SelLot"].ToString() != "")
            {
                SLot = Session["SelLot"].ToString();
            }
            DEOModel obj = new DEOModel();
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                obj.StoreAllData = OBJDB.SelectClusterWiseCentreReportLot(district, SLot);
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.lot = obj.StoreAllData.Tables[0].Rows[0]["lot"].ToString();
                    ViewBag.fdate = obj.StoreAllData.Tables[0].Rows[0]["finaldate"].ToString();
                    ViewBag.Month = obj.StoreAllData.Tables[0].Rows[0]["Month"].ToString();
                    ViewBag.Year = obj.StoreAllData.Tables[0].Rows[0]["year"].ToString();

                }


                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }
        public ActionResult FinalSubmitDeoPortal()
        {
            string DeoUser = null;
            string district = null;
            int udid = 0;
            string Catg = null;

            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                udid = Convert.ToInt32(Session["UID"].ToString());

                DEOModel obj = new DEOModel();
                obj.StoreAllData = OBJDB.FinalSubmitDeoPortalGrid(district);
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message1 = "Completed";
                    ViewBag.Succ1 = "1";
                    ViewBag.TotalCount1 = 0;
                }
                else
                {
                    ViewBag.TotalCount1 = obj.StoreAllData.Tables[0].Rows.Count;

                }
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[1].Rows.Count == 0)
                {
                    ViewBag.Message2 = "Completed";
                    ViewBag.Succ2 = "1";
                    ViewBag.TotalCount2 = 0;
                }
                else
                {
                    ViewBag.TotalCount2 = obj.StoreAllData.Tables[1].Rows.Count;

                }
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[2].Rows.Count == 0)
                {
                    ViewBag.Message3 = "Completed";
                    ViewBag.Succ3 = "1";
                    ViewBag.TotalCount3 = 0;
                }
                else
                {
                    ViewBag.TotalCount3 = obj.StoreAllData.Tables[2].Rows.Count;

                }
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[3].Rows.Count == 0)
                {
                    ViewBag.Message4 = "Completed";
                    ViewBag.Succ4 = "1";
                    ViewBag.TotalCount4 = 0;
                }
                else
                {
                    ViewBag.TotalCount4 = obj.StoreAllData.Tables[3].Rows.Count;

                }
                //-------if Above 4 Viewbag like ViewBag.Succ1,ViewBag.Succ2,ViewBag.Succ3,ViewBag.Succ4 is Equals To 1------Then One More Validation is Following//
                DataSet FinalValidation = OBJDB.FinalSubmitDeoPortalGridValidation(district);
                if (FinalValidation.Tables[0].Rows.Count > 0)
                {
                    if (FinalValidation == null || FinalValidation.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.FVMessage = "There is no record for final Submition..";
                        ViewBag.TotalCountFV = 0;
                    }
                    else if ((FinalValidation.Tables[0].Rows[0]["clst"].ToString() == "0") && (FinalValidation.Tables[0].Rows[0]["cnt"].ToString() == "0") && (FinalValidation.Tables[0].Rows[0]["supdt"].ToString() == "0") && (FinalValidation.Tables[0].Rows[0]["dsupdt"].ToString() == "0") && (FinalValidation.Tables[0].Rows[0]["invg"].ToString() == "0") && (FinalValidation.Tables[0].Rows[0]["staff"].ToString() == "0"))
                    {
                        ViewBag.Message1 = "No Records";
                        ViewBag.Message2 = "No Records";
                        ViewBag.Message3 = "No Records";
                        ViewBag.Message4 = "No Records";
                        ViewBag.FVMessage = "There is no record for final Submition..";
                        ViewBag.TotalCountFV = 0;
                    }
                    else
                    {
                        ViewBag.TotalCountFV = FinalValidation.Tables[0].Rows.Count;

                        ViewBag.clust = FinalValidation.Tables[0].Rows[0]["clst"].ToString();
                        ViewBag.centre = FinalValidation.Tables[0].Rows[0]["cnt"].ToString();
                        ViewBag.supdt = FinalValidation.Tables[0].Rows[0]["supdt"].ToString();
                        ViewBag.dsupdt = FinalValidation.Tables[0].Rows[0]["dsupdt"].ToString();
                        ViewBag.invg = FinalValidation.Tables[0].Rows[0]["invg"].ToString();
                        ViewBag.staff = FinalValidation.Tables[0].Rows[0]["staff"].ToString();

                        ViewBag.Obsr = FinalValidation.Tables[0].Rows[0]["Obsr"].ToString();
                        ViewBag.FlyS = FinalValidation.Tables[0].Rows[0]["FlyS"].ToString();
                        ViewBag.DupCntlr = FinalValidation.Tables[0].Rows[0]["DupCntlr"].ToString();
                    }
                }


                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }
        [HttpPost]
        public ActionResult FinalSubmitDeoPortal(DEOModel DEO)
        {
            string DeoUser = null;
            string district = null;
            string udid = null;
            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");

            }
            DeoUser = Session["USER"].ToString();
            district = Session["Dist"].ToString();
            udid = Session["UID"].ToString();
            string result = OBJDB.FinalSubmitDeoPortal(district);
            if (result == "FinalSubmitted")
            {
                // @ViewBag.result = "1";
                ViewData["result"] = "1";

            }
            if (result == "NotSubmitted")
            {
                // @ViewBag.result = "1";
                ViewData["result"] = "-1";

            }

            return View(DEO);
        }
        //public ActionResult FinalSubmitionDeoPortal(DEOModel DEO,string CCODEID)
        //{
        //    string DeoUser = null;
        //    string district = null;
        //    string udid = null;           
        //    if (Session["USER"] == null && Session["Name"] == null)
        //    {
        //       return RedirectToAction("Index", "DeoPortal");

        //    }
        //    DeoUser = Session["USER"].ToString();
        //    district = Session["Dist"].ToString();
        //    udid = Session["UID"].ToString();

        //    if (CCODEID == null && CCODEID == "0")
        //    {
        //        return RedirectToAction("Index", "DeoPortal");
        //    }
        //    else
        //    {
        //        CCODEID = encrypt.QueryStringModule.Decrypt(CCODEID);
        //        string result = OBJDB.FinalSubmitDeoPortal(CCODEID, district);
        //        if (result == "FinalSubmitted")
        //        {
        //            // @ViewBag.result = "1";
        //            ViewData["result"] = "1";

        //        }

        //    }
        //    return RedirectToAction("FinalSubmitDeoPortal", "DeoPortal");
        //    //return View(DEO);
        //}
        public ActionResult ClusterWiseStaffListPrint(int? page, string SelLot)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    //string[] ArrInvoice = { "010","020","025","030","040","045","050","060","070","080" };
                    //string[] ArrInvoice = { "Invoice # 120", "Invoice # 121", "Invoice # 122", "Invoice # 123" };
                    //List<int> lstInvice = new List<int>();
                    //List<SelectListItem> DList = new List<SelectListItem>();
                    //foreach (string item in ArrInvoice)
                    //{
                    //    //int intInvocie = 0;
                    //    //Int32.TryParse(item.Split(',')[0], out intInvocie);
                    //    //if (intInvocie > 0)
                    //    //{
                    //    //    //lstInvice.Add(intInvocie);
                    //    //    DList.Add(new SelectListItem { Text = "yiy", Value = Convert.ToString(intInvocie)});
                    //    //}


                    //}
                    //string DC = "010,020,025,030,040,045,050,060,070,080";
                    //// Split string on spaces.
                    //// ... This will separate all the words.
                    //string[] DistCodes = DC.Split(',');
                    //foreach (string DistCode in DistCodes)
                    //{
                    //    DList.Add(new SelectListItem { Text = "yiy", Value = DistCode });
                    //}

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    //Search = "a.DIST = '" + district + "'";
                    Search = "b.DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }

                // Search += " and  ExamMonth = '" + Session["DeoLoginExamCentre"].ToString() + "'";
                obj.StoreAllData = OBJDB.SelectClusterWiseStaffListReport(Search, Catg, pageIndex, Session["DeoLoginExamCentre"].ToString());
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.Month = obj.StoreAllData.Tables[0].Rows[0]["Month"].ToString();
                    ViewBag.Year = obj.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                    //ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    //int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    //int pn = tp / 10;
                    //int cal = 10 * pn;
                    //int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    //if (res >= 1)
                    //    ViewBag.pn = pn + 1;
                    //else
                    //    ViewBag.pn = pn;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }
        public ActionResult ClusterWiseCentreList(int? page)
        {


            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    //string[] ArrInvoice = { "010","020","025","030","040","045","050","060","070","080" };
                    //string[] ArrInvoice = { "Invoice # 120", "Invoice # 121", "Invoice # 122", "Invoice # 123" };
                    //List<int> lstInvice = new List<int>();
                    //List<SelectListItem> DList = new List<SelectListItem>();
                    //foreach (string item in ArrInvoice)
                    //{
                    //    //int intInvocie = 0;
                    //    //Int32.TryParse(item.Split(',')[0], out intInvocie);
                    //    //if (intInvocie > 0)
                    //    //{
                    //    //    //lstInvice.Add(intInvocie);
                    //    //    DList.Add(new SelectListItem { Text = "yiy", Value = Convert.ToString(intInvocie)});
                    //    //}


                    //}
                    //string DC = "010,020,025,030,040,045,050,060,070,080";
                    //// Split string on spaces.
                    //// ... This will separate all the words.
                    //string[] DistCodes = DC.Split(',');
                    //foreach (string DistCode in DistCodes)
                    //{
                    //    DList.Add(new SelectListItem { Text = "yiy", Value = DistCode });
                    //}

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    //Search = "a.DIST = '" + district + "'";
                    Search = "b.DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }

                //Search += " and  ExamMonth = '" + Session["DeoLoginExamCentre"].ToString() + "'";

                obj.StoreAllData = OBJDB.SelectClusterWiseCentreReport(Search, Catg, pageIndex, Session["DeoLoginExamCentre"].ToString());
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.Month = obj.StoreAllData.Tables[0].Rows[0]["Month"].ToString();
                    ViewBag.Year = obj.StoreAllData.Tables[0].Rows[0]["Year"].ToString();
                    //ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    //int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    //int pn = tp / 10;
                    //int cal = 10 * pn;
                    //int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    //if (res >= 1)
                    //    ViewBag.pn = pn + 1;
                    //else
                    //    ViewBag.pn = pn;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }

        public ActionResult ClusterListReportPrint(int? page)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    //string[] ArrInvoice = { "010","020","025","030","040","045","050","060","070","080" };
                    //string[] ArrInvoice = { "Invoice # 120", "Invoice # 121", "Invoice # 122", "Invoice # 123" };
                    //List<int> lstInvice = new List<int>();
                    //List<SelectListItem> DList = new List<SelectListItem>();
                    //foreach (string item in ArrInvoice)
                    //{
                    //    //int intInvocie = 0;
                    //    //Int32.TryParse(item.Split(',')[0], out intInvocie);
                    //    //if (intInvocie > 0)
                    //    //{
                    //    //    //lstInvice.Add(intInvocie);
                    //    //    DList.Add(new SelectListItem { Text = "yiy", Value = Convert.ToString(intInvocie)});
                    //    //}


                    //}
                    //string DC = "010,020,025,030,040,045,050,060,070,080";
                    //// Split string on spaces.
                    //// ... This will separate all the words.
                    //string[] DistCodes = DC.Split(',');
                    //foreach (string DistCode in DistCodes)
                    //{
                    //    DList.Add(new SelectListItem { Text = "yiy", Value = DistCode });
                    //}

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    //Search = "a.DIST = '" + district + "'";
                    Search = "DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }

                //  Search += " and  ExamMonth = '" + Session["DeoLoginExamCentre"].ToString() + "'";
                obj.StoreAllData = OBJDB.SelectClusterListByUserReport(Search, Catg, pageIndex, Session["DeoLoginExamCentre"].ToString());
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.Month = obj.StoreAllData.Tables[0].Rows[0]["Month"].ToString();
                    ViewBag.Year = obj.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                    //ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    //int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    //int pn = tp / 10;
                    //int cal = 10 * pn;
                    //int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    //if (res >= 1)
                    //    ViewBag.pn = pn + 1;
                    //else
                    //    ViewBag.pn = pn;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

        }
        public ActionResult CentreReportPrint(FormCollection frm)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["Utype"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (Session["USER"].ToString().ToUpper() == "ADMIN")
                {
                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                    district = "0";
                    //DeoUser = Session["Utype"].ToString().ToUpper();
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }
                DEOModel obj = new DEOModel();
                string Search = string.Empty;
                int pageIndex = 1;
                //pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                Catg = obj.Category;

                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    Search = "a.DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }

                Search += " and  ExamMonth = '" + Session["DeoLoginExamCentre"].ToString() + "'";
                obj.StoreAllData = OBJDB.SelectCenterListByUserReportPrint(Search, Catg, pageIndex);
                obj.TotalCount = OBJDB.SelectCenterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    ViewBag.Month = obj.StoreAllData.Tables[0].Rows[0]["Month"].ToString();
                    ViewBag.Year = obj.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                    //int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    //int pn = tp / 10;
                    //int cal = 10 * pn;
                    //int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    //if (res >= 1)
                    //    ViewBag.pn = pn + 1;
                    //else
                    //    ViewBag.pn = pn;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }
        [HttpPost]
        public ActionResult CentreReportPrint(FormCollection frm, string id)
        {
            DEOModel obj = new DEOModel();
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (Session["USER"].ToString().ToUpper() == "ADMIN")
                {
                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                    district = frm["SelDist"].ToString();
                    //DeoUser = Session["Utype"].ToString().ToUpper();
                    obj.SelDist = frm["SelDist"].ToString();
                    Session["Dist"] = frm["SelDist"].ToString();
                    Session["DISTNM"] = frm["SelDist"];// frm["hiddistnm"];
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                string Search = string.Empty;
                int pageIndex = 1;
                //pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                Catg = obj.Category;

                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    Search = "a.DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }

                Search += " and ExamMonth = '" + Session["DeoLoginExamCentre"].ToString() + "'";

                obj.StoreAllData = OBJDB.SelectCenterListByUserReportPrint(Search, Catg, pageIndex);
                obj.TotalCount = OBJDB.SelectCenterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    ViewBag.Month = obj.StoreAllData.Tables[0].Rows[0]["Month"].ToString();
                    ViewBag.Year = obj.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                    //int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    //int pn = tp / 10;
                    //int cal = 10 * pn;
                    //int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    //if (res >= 1)
                    //    ViewBag.pn = pn + 1;
                    //else
                    //    ViewBag.pn = pn;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }

        //[Route("DeoPortal")]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult CentreReport(int? page)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    //string[] ArrInvoice = { "010","020","025","030","040","045","050","060","070","080" };
                    //string[] ArrInvoice = { "Invoice # 120", "Invoice # 121", "Invoice # 122", "Invoice # 123" };
                    //List<int> lstInvice = new List<int>();
                    //List<SelectListItem> DList = new List<SelectListItem>();
                    //foreach (string item in ArrInvoice)
                    //{
                    //    //int intInvocie = 0;
                    //    //Int32.TryParse(item.Split(',')[0], out intInvocie);
                    //    //if (intInvocie > 0)
                    //    //{
                    //    //    //lstInvice.Add(intInvocie);
                    //    //    DList.Add(new SelectListItem { Text = "yiy", Value = Convert.ToString(intInvocie)});
                    //    //}


                    //}
                    //string DC = "010,020,025,030,040,045,050,060,070,080";
                    //// Split string on spaces.
                    //// ... This will separate all the words.
                    //string[] DistCodes = DC.Split(',');
                    //foreach (string DistCode in DistCodes)
                    //{
                    //    DList.Add(new SelectListItem { Text = "yiy", Value = DistCode });
                    //}

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    Search = "a.DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }
                obj.StoreAllData = OBJDB.SelectCenterListByUserReport(Search, Catg, pageIndex);
                obj.TotalCount = OBJDB.SelectCenterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int pn = tp / 10;
                    int cal = 10 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult CentreReport(int? page, DEOModel DEO)
        {
            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }
            string Search = Session["Dist"].ToString();
            string imgurl = "../Images/pseb_logo.png";
            //DataTable dt = OBJDB.ReportCenterListByUser(Search);
            //ExportToPdf(dt);

            //------------------------------------Start-----------------
            string DistName = Session["DISTNM"].ToString();
            //int orderNo = 2303;
            //DataTable dt = new DataTable();
            //dt.Columns.AddRange(new DataColumn[5] {
            //                new DataColumn("ProductId", typeof(string)),
            //                new DataColumn("Product", typeof(string)),
            //                new DataColumn("Price", typeof(int)),
            //                new DataColumn("Quantity", typeof(int)),
            //                new DataColumn("Total", typeof(int))});
            //dt.Rows.Add(101, "Sun Glasses", 200, 5, 1000);
            //dt.Rows.Add(102, "Jeans", 400, 2, 800);
            //dt.Rows.Add(103, "Trousers", 300, 3, 900);
            //dt.Rows.Add(104, "Shirts", 550, 2, 1100);
            DataTable dt = OBJDB.ReportCenterListByUser(Search);

            using (StringWriter sw = new StringWriter())
            {
                using (System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(sw))
                {
                    StringBuilder sb = new StringBuilder();

                    //Generate reort .
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Punjab School Education Board<br/>List of Examination Centre Of March 2017</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    //sb.Append("<tr><td><b>Image: </b>");
                    sb.Append("<tr><td><img src='D:\\PSEB_20_12_2016\\PSEBONLINE\\PSEBONLINE\\Images\\pseblogo.jpg' />");
                    //sb.Append(@"<img src='D:\PSEB_20_12_2016\PSEBONLINE\PSEBONLINE\Images\pseblogo.jpg' />");
                    sb.Append("</td><td align = 'right'><b>Date: </b>");
                    sb.Append(DateTime.Now);
                    sb.Append(" </td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>District Name: </b>");
                    sb.Append(DistName);
                    sb.Append("</td></tr>");
                    sb.Append("</table>");
                    sb.Append("<br />");

                    //Generate Report Items Grid.
                    sb.Append("<table border = '1'>");
                    sb.Append("<tr>");
                    //foreach (DataColumn column in dt.Columns)
                    //{
                    //    sb.Append("<th style = 'background-color: #D20B0C;color:#ffffff'>");
                    //    sb.Append(column.ColumnName);
                    //    sb.Append("</th>");
                    //}
                    sb.Append("<th>S.No</ th >");
                    sb.Append("<th>Centre</th>");
                    sb.Append("<th>Centre Name</ th>");
                    sb.Append("<th>Building Name</th>");
                    sb.Append("<th>Cluster Code</th>");
                    sb.Append("<th>Matric</th>");
                    sb.Append("<th>Sr.Sec</th>");
                    sb.Append("</tr>");
                    foreach (DataRow row in dt.Rows)
                    {
                        sb.Append("<tr>");
                        foreach (DataColumn column in dt.Columns)
                        {
                            sb.Append("<td>");
                            sb.Append(row[column]);
                            sb.Append("</td>");
                        }
                        sb.Append("</tr>");
                    }
                    //sb.Append("<tr><td align = 'right' colspan = '");
                    //sb.Append(dt.Columns.Count - 1);
                    //sb.Append("'>Total</td>");
                    //sb.Append("<td>");
                    //sb.Append(dt.Compute("sum(Total)", ""));
                    //sb.Append("</td>");
                    //sb.Append("</tr></table>");
                    sb.Append("</table>");


                    //Export HTML String as PDF.
                    StringReader sr = new StringReader(sb.ToString());
                    Document pdfDoc = new Document(PageSize.A4.Rotate(), 4f, 4f, 4f, 0f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, Response.OutputStream);
                    pdfDoc.Open();
                    htmlparser.Parse(sr);
                    pdfDoc.Close();
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", "attachment;filename=" + DistName + " Centre report .pdf");
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);

                    //var outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "" + DistName + " Centre report .pdf");
                    //using (var fs = new FileStream(outputFile, FileMode.Create, FileAccess.Write, FileShare.None))
                    //{
                    //    using (var doc = new Document())
                    //    {
                    //        using (writer = PdfWriter.GetInstance(doc, fs))
                    //        {
                    //            doc.Open();
                    //            for (var i = 0; i < 1000; i++)
                    //            {
                    //                doc.Add(new Paragraph(String.Format("This is paragraph #{0}", i)));
                    //            }
                    //            doc.Close();
                    //        }
                    //    }
                    //}

                    Response.Write(pdfDoc);
                    Response.End();
                }
            }
            //-------------------------End--------------------


            return View(DEO);


        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult CenterList(int? page)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    //string[] ArrInvoice = { "010","020","025","030","040","045","050","060","070","080" };
                    //string[] ArrInvoice = { "Invoice # 120", "Invoice # 121", "Invoice # 122", "Invoice # 123" };
                    //List<int> lstInvice = new List<int>();
                    //List<SelectListItem> DList = new List<SelectListItem>();
                    //foreach (string item in ArrInvoice)
                    //{
                    //    //int intInvocie = 0;
                    //    //Int32.TryParse(item.Split(',')[0], out intInvocie);
                    //    //if (intInvocie > 0)
                    //    //{
                    //    //    //lstInvice.Add(intInvocie);
                    //    //    DList.Add(new SelectListItem { Text = "yiy", Value = Convert.ToString(intInvocie)});
                    //    //}


                    //}
                    //string DC = "010,020,025,030,040,045,050,060,070,080";
                    //// Split string on spaces.
                    //// ... This will separate all the words.
                    //string[] DistCodes = DC.Split(',');
                    //foreach (string DistCode in DistCodes)
                    //{
                    //    DList.Add(new SelectListItem { Text = "yiy", Value = DistCode });
                    //}

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                    district = "0";
                    //ViewBag.SelectedItem =
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    Search = "a.DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }


                Search += " and a.ExamMonth = '" + Session["DeoLoginExamCentre"].ToString() + "'";
				//obj.StoreAllData = OBJDB.CenterList(Search, Catg, pageIndex);
				// obj.TotalCount = OBJDB.CenterCount(Search, Catg, pageIndex);

				obj.StoreAllData = OBJDB.SelectCenterListByUser(Search, Catg, pageIndex);
				obj.TotalCount = OBJDB.SelectCenterListByUserCount(Search, Catg, pageIndex);


				if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
					int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
					int pn = tp / 10;
                    int cal = 10 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult CenterList(int? page, DEOModel DEO, FormCollection frm, string cmd, string SelDist, string Category, string SearchString)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                    //district = "0";
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                Catg = DEO.Category;

                string Search = "";
                if (cmd == "Search")
                {
                    if (SelDist == "0")
                    {
                        Search += "a.Dist like '%'";

                    }
                    else if (SelDist != "")
                    {
                        Search += "a.Dist='" + SelDist.ToString().Trim() + "'";

                    }
                    if (SelDist != "")
                    {
                        ViewBag.SelectedItem = SelDist;
                        int SelValueSch = Convert.ToInt32(Category.ToString());
                        if (SelValueSch == 1)
                        { Search += " and a.Cent='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueSch == 2)
                        { Search += " and  a.ecentre like '%" + SearchString.ToString().Trim() + "%'"; }
                        else if (SelValueSch == 3)
                        { Search += " and  a.schoole like '%" + SearchString.ToString().Trim() + "%'"; }
                        else if (SelValueSch == 4)
                        { Search += " and a.CCODE='" + SearchString.ToString().Trim() + "'"; }
                    }


                    TempData["CenterListSelDist"] = Search;
                    TempData["CenterListSelectList"] = Category;
                    TempData["CenterListSearchString"] = SearchString.ToString().Trim();

                    ViewBag.Searchstring = SearchString.ToString().Trim();

                    Search += " and a.ExamMonth = '" + Session["DeoLoginExamCentre"].ToString() + "'";
                    DEO.StoreAllData = OBJDB.SelectCenterListByUser(Search, Catg, pageIndex);
                    DEO.TotalCount = OBJDB.SelectCenterListByUserCount(Search, Catg, pageIndex);
                    //rm.StoreAllData = objDB.GetStudentRecordsSearch_ImportData(Search);
                    // asm.StoreAllData = objDB.SearchSchoolDetails(Search);
                    if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "DATA DOESN'T EXIST";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(DEO.TotalCount.Tables[0].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(DEO.TotalCount.Tables[0].Rows[0]["decount"]);
                        int pn = tp / 10;
                        int cal = 10 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;


                        return View(DEO);
                    }
                }

                return View(DEO);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ClusterListReport(int? page)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    //string[] ArrInvoice = { "010","020","025","030","040","045","050","060","070","080" };
                    //string[] ArrInvoice = { "Invoice # 120", "Invoice # 121", "Invoice # 122", "Invoice # 123" };
                    //List<int> lstInvice = new List<int>();
                    //List<SelectListItem> DList = new List<SelectListItem>();
                    //foreach (string item in ArrInvoice)
                    //{
                    //    //int intInvocie = 0;
                    //    //Int32.TryParse(item.Split(',')[0], out intInvocie);
                    //    //if (intInvocie > 0)
                    //    //{
                    //    //    //lstInvice.Add(intInvocie);
                    //    //    DList.Add(new SelectListItem { Text = "yiy", Value = Convert.ToString(intInvocie)});
                    //    //}


                    //}
                    //string DC = "010,020,025,030,040,045,050,060,070,080";
                    //// Split string on spaces.
                    //// ... This will separate all the words.
                    //string[] DistCodes = DC.Split(',');
                    //foreach (string DistCode in DistCodes)
                    //{
                    //    DList.Add(new SelectListItem { Text = "yiy", Value = DistCode });
                    //}

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    //Search = "a.DIST = '" + district + "'";
                    Search = "DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }


                //                Search += " and  ExamMonth = '" + Session["DeoLoginExamCentre"].ToString() + "'";
                obj.StoreAllData = OBJDB.SelectClusterListByUserReport(Search, Catg, pageIndex, Session["DeoLoginExamCentre"].ToString());
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;


                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ClusterList(int? page)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN")
                {

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                    district = "0";
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                DataSet DistwiseSCHL = OBJDB.GetDeoClusterSchlDISTWise(district); // passing Value to DBClass from model            
                List<SelectListItem> SDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in DistwiseSCHL.Tables[0].Rows)
                {
                    SDList.Add(new SelectListItem { Text = @dr["Address"].ToString(), Value = @dr["cschl"].ToString() });
                }
                ViewBag.schl = SDList;

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    Search = "DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }


                // Search += " and  ExamMonth = '" + Session["DeoLoginExamCentre"].ToString() + "'";
                obj.StoreAllData = OBJDB.SelectClusterListByUser(Search, Catg, pageIndex, Session["DeoLoginExamCentre"].ToString());
                obj.TotalCount = OBJDB.SelectClusterListByUserCount(Search, Catg, pageIndex);

                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int pn = tp / 10;
                    int cal = 10 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult ClusterList(int? page, DEOModel DEO, FormCollection frm, string cmd, string SelDist, string Category, string SearchString)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }
                DataSet DistwiseSCHL = OBJDB.GetDeoClusterSchlDISTWise(district); // passing Value to DeoClass from model            
                List<SelectListItem> SDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in DistwiseSCHL.Tables[0].Rows)
                {
                    SDList.Add(new SelectListItem { Text = @dr["Address"].ToString(), Value = @dr["cschl"].ToString() });
                }

                ViewBag.schl = SDList;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                Catg = DEO.Category;

                string Search = "";
                if (cmd == "Search")
                {
                    if (SelDist == "0")
                    {
                        Search += "a.Dist like '%'";

                    }
                    else if (SelDist != "")
                    {
                        if (DeoUser.ToUpper() == "ADMIN")
                        {
                            district = frm["SelDist"];
                            Search += "Dist='" + SelDist.ToString().Trim() + "'";
                        }
                        else
                            Search += "Dist='" + SelDist.ToString().Trim() + "'";

                    }
                    if (SelDist != "")
                    {
                        ViewBag.SelectedItem = SelDist;
                        int SelValueSch = Convert.ToInt32(Category.ToString());
                        if (SelValueSch == 1)
                        { Search += " and a.cCent='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueSch == 2)
                        { Search += " and  a.clusternam like '%" + SearchString.ToString().Trim() + "%'"; }
                        else if (SelValueSch == 3)
                        { Search += " and  a.Address like '%" + SearchString.ToString().Trim() + "%'"; }
                        else if (SelValueSch == 4)
                        { Search += " and a.CCODE='" + SearchString.ToString().Trim() + "'"; }
                    }


                    TempData["CenterListSelDist"] = Search;
                    TempData["CenterListSelectList"] = Category;
                    TempData["CenterListSearchString"] = SearchString.ToString().Trim();

                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    DEO.StoreAllData = OBJDB.SelectClusterListByUser(Search, Catg, pageIndex, Session["DeoLoginExamCentre"].ToString());
                    DEO.TotalCount = OBJDB.SelectClusterListByUserCount(Search, Catg, pageIndex);
                    //rm.StoreAllData = objDB.GetStudentRecordsSearch_ImportData(Search);
                    // asm.StoreAllData = objDB.SearchSchoolDetails(Search);
                    if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "DATA DOESN'T EXIST";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(DEO.TotalCount.Tables[0].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(DEO.TotalCount.Tables[0].Rows[0]["decount"]);
                        int pn = tp / 10;
                        int cal = 10 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;


                        return View(DEO);
                    }
                }
                //if (cmd == "ADD CLUSTER")
                //{
                //    return RedirectToAction("ADDCLUSTER", "Deoportal");
                //}

                return View(DEO);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

        }
        //public JsonResult SchoolSelectList()
        //{
        //    string district = "010";
        //    DataSet DistwiseSCHL = OBJDB.GetDeoSchlDISTWise(district); // passing Value to DeoClass from model            
        //    List<SelectListItem> SDList = new List<SelectListItem>();
        //    foreach (System.Data.DataRow dr in DistwiseSCHL.Tables[0].Rows)
        //    {
        //        SDList.Add(new SelectListItem { Text = @dr["Address"].ToString(), Value = @dr["cschl"].ToString() });
        //    }

        //    ViewBag.schl = SDList;

        //    return Json(SDList);
        //}
        public JsonResult CreateCluster(string ClusterName, string Person1, string Mobile1, string Person2, string Mobile2, string Pincode, string CENT, string DISTCODE, string USERID)
        {

            try
            {
                string dee = "";
                string outstatus = "";
                string Search = string.Empty;
                //string UserType = "Admin";               
                //float fee = 0;              
                //DateTime date;
                //
                string ExamMonth = Session["DeoLoginExamCentre"].ToString();
                string res = OBJDB.CreateClusterNew(ClusterName, Person1, Mobile1, Person2, Mobile2, Pincode, CENT, DISTCODE, USERID, ExamMonth);
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
        //-----------------------------------------------------------------------STAFF DETAILS-------------------------------
        public ActionResult BulkUploadStaff()
        {
            return View();
        }
        //[HttpPost]
        //public ActionResult BulkUploadStaff(DEOModel DEO)
        //{
        //    string DeoUser = null;
        //    string district = null;
        //    string fileLocation = "";
        //    string filename = "";
        //    string uid = "";
        //    try {
        //        if (Session["USER"] == null && Session["Name"] == null)
        //        {
        //            return RedirectToAction("Index", "DeoPortal");
        //        }              
        //        DeoUser = Session["USER"].ToString();
        //        district = Session["Dist"].ToString();
        //        uid = Session["UID"].ToString();
        //        if (DEO.file != null)
        //        {
        //            filename = Path.GetFileName(DEO.file.FileName);
        //        }
        //        DataSet ds = new DataSet();
        //        if (DEO.file.ContentLength > 0)  //(Request.Files["file"].ContentLength > 0
        //        {
        //            string fileName1 = "MIS_" + district + '_' + DateTime.Now.ToString("ddMMyyyyHHmm");  //MIS_201_110720161210
        //            string fileExtension = System.IO.Path.GetExtension(DEO.file.FileName);
        //            if (fileExtension == ".xls" || fileExtension == ".xlsx")
        //            {
        //                fileLocation = Server.MapPath("~/StaffUpload/" + fileName1 + fileExtension);

        //                if (System.IO.File.Exists(fileLocation))
        //                {
        //                    try
        //                    {
        //                        System.IO.File.Delete(fileLocation);
        //                    }
        //                    catch (Exception)
        //                    {

        //                    }
        //                }
        //                DEO.file.SaveAs(fileLocation);
        //                string excelConnectionString = string.Empty;
        //                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
        //                if (fileExtension == ".xls")
        //                {
        //                    excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
        //                    fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
        //                }
        //                //connection String for xlsx file format.
        //                else if (fileExtension == ".xlsx")
        //                {
        //                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
        //                    fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
        //                }
        //                //Create Connection to Excel work book and add oledb namespace
        //                OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
        //                excelConnection.Open();
        //                DataTable dt = new DataTable();
        //                dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
        //                if (dt == null)
        //                {
        //                    return null;
        //                }

        //                String[] excelSheets = new String[dt.Rows.Count];
        //                int t = 0;
        //                //excel data saves in temp file here.
        //                foreach (DataRow row in dt.Rows)
        //                {
        //                    excelSheets[t] = row["TABLE_NAME"].ToString(); // bank_mis     TABLE_NAME
        //                    t++;
        //                }
        //                OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
        //                //string query = string.Format("Select * from [{0}]", excelSheets[0]);
        //                string query = string.Format("Select * from [{0}]", excelSheets[1]);
        //                using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
        //                {
        //                    dataAdapter.Fill(ds);
        //                }
        //                string MIS_FILENM = fileName1;
        //                string CheckMis = OBJDB.CheckMisExcel(ds, MIS_FILENM);

        //                if (CheckMis == "")
        //                {

        //                    string Result1 = "";
        //                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                    {

        //                        string CCODE = ViewBag.ClusterCode;
        //                        string TD = ds.Tables[0].Rows[i][0].ToString();
        //                        string Epunjabid = ds.Tables[0].Rows[i][1].ToString();
        //                        string Aadharnum = ds.Tables[0].Rows[i][2].ToString();
        //                        string Name = ds.Tables[0].Rows[i][3].ToString();
        //                        string fname = ds.Tables[0].Rows[i][4].ToString();
        //                        string SCHL = ds.Tables[0].Rows[i][5].ToString();
        //                        string cadre = ds.Tables[0].Rows[i][6].ToString();
        //                        string desi = ds.Tables[0].Rows[i][7].ToString();
        //                        string mob = ds.Tables[0].Rows[i][8].ToString();
        //                        string gen = ds.Tables[0].Rows[i][9].ToString();
        //                        string expe = ds.Tables[0].Rows[i][10].ToString();
        //                        string Month = ds.Tables[0].Rows[i][11].ToString();
        //                        string IFSC = ds.Tables[0].Rows[i][12].ToString();
        //                        string Accno = ds.Tables[0].Rows[i][13].ToString();
        //                        string Schoolname = null;
        //                        // string OtherSchool = DEO.Otherschl;
        //                        //if (Schoolname == "Others")
        //                        //{
        //                        //    Schoolname = OtherSchool;
        //                        //    SCHL = "*******";
        //                        //}
        //                        //DEO.MIS_FILENM = fileName1;       // "MIS_101_210620161659.xls"  

        //                        string Staffres = OBJDB.ADDSTAFFDETAILS(uid, district, CCODE, TD, fname, Name, expe, Month, gen, mob, SCHL, Schoolname, Aadharnum, Epunjabid, cadre, desi, IFSC, Accno);
        //                        if (Staffres != "0")
        //                        {
        //                            ViewData["result"] = "1";

        //                        }
        //                        else
        //                        {
        //                            ViewData["result"] = "-1";
        //                            ViewBag.Message = "Please Upload Only .xls and .xlsx only";
        //                            return View();
        //                        }

        //                    }

        //                }

        //            }

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
        //        return View();

        //    }
        //    return View(DEO);
        //}


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ADDSTAFF(DEOModel DEO, string CCODEID)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            @ViewBag.DA = objCommon.GetDA();
            @ViewBag.exp = OBJDB.GetStaffExp();
            try
            {

                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (CCODEID != null && CCODEID != "")
                {
                    Session["vcclsCode"] = CCODEID;

                }
                else
                {
                    CCODEID = Session["vcclsCode"].ToString();
                }
                //CCODEID = encrypt.QueryStringModule.Decrypt(CCODEID);
                CCODEID = encrypt.QueryStringModule.Decrypt(CCODEID);
                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                DataSet Dresult = OBJDB.GetClusterSCHOOLSTAFF(CCODEID, Session["DeoLoginExamCentre"].ToString());
                //DataSet Dresult = OBJDB.GetClusterSCHOOLSTAFF(CCODEID); 
                if (Dresult.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;
                    return View(DEO);
                }
                else
                {
                    //ViewBag.ClusterCode = Dresult.Tables[0].Rows[0][0].ToString();
                    //ViewBag.ClusterName = Dresult.Tables[0].Rows[0][1].ToString();
                    //ViewBag.ClusterSchoolCode = Dresult.Tables[0].Rows[0][2].ToString();
                    //ViewBag.Address = Dresult.Tables[0].Rows[0][4].ToString();
                    //ViewBag.TotalCount = Dresult.Tables[0].Rows.Count;
                    ViewBag.ClusterCode = Dresult.Tables[0].Rows[0]["ccode"].ToString();
                    ViewBag.ClusterName = Dresult.Tables[0].Rows[0]["clusternam"].ToString();
                    ViewBag.Address = Dresult.Tables[0].Rows[0]["address"].ToString();
                    ViewBag.rstaff = Dresult.Tables[0].Rows[0]["rstaff"].ToString();
                    ViewBag.staff = Dresult.Tables[0].Rows[0]["staff"].ToString();
                    ViewBag.diff = Dresult.Tables[0].Rows[0]["diff"].ToString();
                    ViewBag.TotalCount = Dresult.Tables[0].Rows.Count;
                }
                DataSet Staffresult = OBJDB.GetSTAFFClusterWise(CCODEID, Session["DeoLoginExamCentre"].ToString());
                if (Staffresult.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    //ViewBag.TotalCount = 0;
                    //return View(DEO);
                }
                else
                {
                    //SLNO, CCODE, TeacherName, FatherName, Designation, TypeOfDuty, Mobile
                    @ViewBag.message = "1";
                    ViewBag.SLNO = Staffresult.Tables[0].Rows[0][0].ToString();
                    ViewBag.CCODE = Staffresult.Tables[0].Rows[0][1].ToString();
                    ViewBag.TeacherName = Staffresult.Tables[0].Rows[0][2].ToString();
                    ViewBag.FatherName = Staffresult.Tables[0].Rows[0][3].ToString();
                    ViewBag.Designation = Staffresult.Tables[0].Rows[0][4].ToString();
                    ViewBag.TypeOfDuty = Staffresult.Tables[0].Rows[0][5].ToString();
                    ViewBag.Mobile = Staffresult.Tables[0].Rows[0][6].ToString();
                    //ViewBag.totcnt = Staffresult.Tables[0].Rows.Count;

                }

                DataSet result = OBJDB.SelectDist(); // passing Value to DBClass from model            
                List<SelectListItem> DList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                {
                    DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }
                ViewBag.SelDist = DList;
                ViewBag.homedist = DList;

                //if (DeoUser == "ADMIN")
                //{
                //    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                //    List<SelectListItem> DistList = new List<SelectListItem>();
                //    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                //    {
                //        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                //    }

                //    ViewBag.Dist = DistList;
                //}
                //else {
                //    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                //    List<SelectListItem> DList = new List<SelectListItem>();
                //    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                //    {
                //        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                //    }
                //    ViewBag.Dist = DList;
                //}
                DataSet DistwiseSCHL = OBJDB.GetDeoSchlDISTWise(district); // passing Value to DeoClass from model            
                List<SelectListItem> SDList = new List<SelectListItem>();
                SDList.Add(new SelectListItem { Text = "Others", Value = "Others" });
                foreach (System.Data.DataRow dr in DistwiseSCHL.Tables[0].Rows)
                {
                    SDList.Add(new SelectListItem { Text = @dr["schlnm"].ToString(), Value = @dr["schl"].ToString() });

                }

                ViewBag.schl = SDList;
                //--------------------------FILL CADRE----------
                DataSet RCADRE = OBJDB.GetCADRE(); // passing Value to DeoClass from model            
                List<SelectListItem> CDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in RCADRE.Tables[0].Rows)
                {
                    CDList.Add(new SelectListItem { Text = @dr["CADRE"].ToString(), Value = @dr["CADRE"].ToString() });
                }

                ViewBag.Dcadre = CDList;
                // --------------------------------End CADRE---------------------
                //--------------------------FILL State----------
                //DataSet RState = OBJDB.GetCADRE(); // passing Value to DeoClass from model            
                //List<SelectListItem> RLState = new List<SelectListItem>();
                //foreach (System.Data.DataRow dr in RState.Tables[0].Rows)
                //{
                //    RLState.Add(new SelectListItem { Text = @dr["StateName"].ToString(), Value = @dr["Scode"].ToString() });
                //}

                //ViewBag.hstate = RLState;
                // --------------------------------End State---------------------
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
            return View(DEO);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult ADDSTAFF(DEOModel DEO, string CCODEID, FormCollection frm)
        {
            string DeoUser = null;
            string district = null;
            string uid = null;
            string Catg = null;
            @ViewBag.DA = objCommon.GetDA();
            ViewBag.exp = OBJDB.GetStaffExp();
            try
            {
                //if (CCODEID == null && CCODEID == null)
                //{
                //   return RedirectToAction("Index", "DeoPortal");
                //}
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                string CCID = frm["hdnccode"];
                // CCODEID = encrypt.QueryStringModule.Decrypt(CCODEID);
                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                uid = Session["UID"].ToString();

                DataSet Dresult = OBJDB.GetClusterSCHOOLSTAFF(CCID, Session["DeoLoginExamCentre"].ToString());
                if (Dresult.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;
                    return View(DEO);
                }
                else
                {
                    ViewBag.ClusterCode = Dresult.Tables[0].Rows[0][0].ToString();
                    ViewBag.ClusterName = Dresult.Tables[0].Rows[0][1].ToString();
                    ViewBag.ClusterSchoolCode = Dresult.Tables[0].Rows[0][2].ToString();
                    ViewBag.CCENT = Dresult.Tables[0].Rows[0][3].ToString();
                    ViewBag.Address = Dresult.Tables[0].Rows[0][4].ToString();
                    ViewBag.TotalCount = Dresult.Tables[0].Rows.Count;
                }
                DataSet DistwiseSCHL = OBJDB.GetDeoSchlDISTWise(district); // passing Value to DeoClass from model            
                List<SelectListItem> SDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in DistwiseSCHL.Tables[0].Rows)
                {
                    SDList.Add(new SelectListItem { Text = @dr["schlnm"].ToString(), Value = @dr["schl"].ToString() });
                }

                ViewBag.schl = SDList;
                //--------------------------FILL CADRE----------
                DataSet RCADRE = OBJDB.GetCADRE(); // passing Value to DeoClass from model            
                List<SelectListItem> CDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in RCADRE.Tables[0].Rows)
                {
                    CDList.Add(new SelectListItem { Text = @dr["CADRE"].ToString(), Value = @dr["CADRE"].ToString() });
                }

                ViewBag.Dcadre = CDList;
                // --------------------------------End CADRE---------------------

                DataSet result = OBJDB.SelectDist(); // passing Value to DBClass from model            
                List<SelectListItem> DList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                {
                    DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }
                ViewBag.SelDist = DList;
                ViewBag.homedist = DList;

                //if (DeoUser.ToUpper() == "ADMIN")
                //{
                //    DataSet DStresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                //    List<SelectListItem> DSistList = new List<SelectListItem>();
                //    foreach (System.Data.DataRow dr in DStresult.Tables[0].Rows)
                //    {
                //        DSistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                //    }

                //    ViewBag.Dist = DSistList;
                //}
                //else {
                //    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                //    List<SelectListItem> DList = new List<SelectListItem>();
                //    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                //    {
                //        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                //    }
                //    ViewBag.Dist = DList;
                //}



                //string CENT = ViewBag.CCENT;
                string CCODE = ViewBag.ClusterCode;

                string TD = DEO.typeDuty;
                string fname = DEO.StaffFatherName;
                string Name = DEO.StaffName;
                string expe = DEO.experience;
                string Month = DEO.expmonth;
                string gen = DEO.gender;
                string mob = DEO.Mobile;
                string SCHL = DEO.schlcode;
                string Schoolname = DEO.Selschool;
                string Aadharnum = DEO.adharno;
                string Epunjabid = DEO.teacherepunjabid;
                string cadre = DEO.cadre;

                string desi = frm["desi"];
                string IFSC = DEO.ifsccode;
                string Accno = DEO.bankaccno;
                string OtherSchool = DEO.Otherschl;
                if (Schoolname == "Others")
                {
                    Schoolname = OtherSchool;
                    SCHL = "*******";
                }
                string phy = DEO.physicallydisablity;
                string DOB = DEO.DOB;
                string SelDist = DEO.SelDist;
                string homeaddress = DEO.homeaddress;
                string homedist = DEO.homedist;
                string bankname = DEO.bankname;

                //string selected12 = frm["Selschool"];
                //string dor = DEO.DOR;
                //string remarks = DEO.remarks;

                //string Staffres = OBJDB.ADDSTAFFDETAILS(uid, district,CCODE, TD, fname, Name, expe, Month, gen, mob, SCHL, Schoolname, Aadharnum, Epunjabid, cadre, desi, IFSC, Accno,dor,remarks);
                string Staffres = OBJDB.ADDSTAFFDETAILS(DeoUser, uid, district, CCODE, TD, fname, Name, expe, Month, gen, mob, SCHL, Schoolname, Aadharnum, Epunjabid, cadre, desi, IFSC, Accno,
                    phy, DOB, SelDist, homeaddress, homedist, bankname);
                if (Staffres == "1")
                {
                    ViewData["result"] = "1";

                }
                else if (Staffres == "-1")
                {
                    ViewData["result"] = "-1";
                }
                else if (Staffres == "0")
                {
                    ViewData["result"] = "0";
                }



            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
            return View(DEO);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult VIEWSTAFF(DEOModel DEO, string CCODEID, FormCollection frm)
        {

            string DeoUser = null;
            string district = null;
            string Catg = null;
            string id = null;
            id = CCODEID;
            @ViewBag.DA = objCommon.GetDA();
            @ViewBag.exp = OBJDB.GetStaffExp();
            try
            {
                //if (id == null && id == null)
                //{
                //    return RedirectToAction("Index", "DeoPortal");
                //}
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (CCODEID != null && CCODEID != "")
                {
                    Session["vcclsCode"] = id;


                }
                else
                {
                    id = Session["vcclsCode"].ToString();
                }
                ViewBag.ClusterCode = id;

                id = encrypt.QueryStringModule.Decrypt(id);
                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                List<SelectListItem> ClsList = new List<SelectListItem>();
                DataSet DCls = OBJDB.GetClusterSTAFFWise(id, district);
                if (DCls.Tables[0].Rows.Count > 0)
                {
                    ViewBag.TotalCount = DCls.Tables[0].Rows.Count;
                    //--------------------------FILL ClusterSTAFFWise----------                           
                    foreach (System.Data.DataRow dr in DCls.Tables[0].Rows)
                    {
                        ClsList.Add(new SelectListItem { Text = @dr["clusternam"].ToString(), Value = @dr["ccode"].ToString() });
                    }

                }
                else
                {
                    ViewBag.Message = "There Is No Cluster";
                    //return View(DEO);
                }

                ViewBag.ClstCode = ClsList;

                DataSet Dresult = OBJDB.GetClusterSCHOOLSTAFF(id, Session["DeoLoginExamCentre"].ToString());
                if (Dresult.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;
                    return View(DEO);
                }
                else
                {
                    //tt.ccode,tt.clusternam,tt.[address],centres,
                    ViewBag.ViewClsid = Dresult.Tables[0].Rows[0]["ccode"].ToString();
                    ViewBag.ClusterCode = Dresult.Tables[0].Rows[0]["ccode"].ToString();
                    ViewBag.ClusterName = Dresult.Tables[0].Rows[0]["clusternam"].ToString();
                    ViewBag.Address = Dresult.Tables[0].Rows[0]["address"].ToString();
                    ViewBag.rstaff = Dresult.Tables[0].Rows[0]["rstaff"].ToString();
                    ViewBag.staff = Dresult.Tables[0].Rows[0]["staff"].ToString();
                    ViewBag.diff = Dresult.Tables[0].Rows[0]["diff"].ToString();
                    // ViewBag.ClusterName = Dresult.Tables[0].Rows[0][""].ToString();
                    //ViewBag.ClusterSchoolCode = Dresult.Tables[0].Rows[0][2].ToString();
                    // ViewBag.Address = Dresult.Tables[0].Rows[0][4].ToString();
                    ViewBag.TotalCount = Dresult.Tables[0].Rows.Count;
                }
                DEO.StoreAllData = OBJDB.GetSTAFFClusterWise(id, Session["DeoLoginExamCentre"].ToString());
                if (DEO.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                }
                else
                {
                    //SLNO, CCODE, TeacherName, FatherName, Designation, TypeOfDuty, Mobile
                    @ViewBag.message = "1";
                    ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
            return View(DEO);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult VIEWSTAFF(DEOModel DEO, FormCollection frm, string cmd)
        {
            string district = null;
            string uid = null;
            string fileLocation = "";
            string filename = "";
            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }
            district = Session["Dist"].ToString();
            uid = Session["UID"].ToString();
            string CLSCODE = frm["ClusterCode"];
            string StaffChk = frm["StaffName"];
            string OldClsid = frm["hdnccode"];
            DEO.ClusterCode = OldClsid;
            string StaffClusterresult = OBJDB.Update_Cluster_To_StaffShift(CLSCODE, district, StaffChk, Session["DeoLoginExamCentre"].ToString());
            if (StaffClusterresult == "0")
            {
                //--------------Not Updated
                // ViewData["result"] = 0;
                TempData["result"] = 0;
            }
            else
            {
                //-------------- Updated----------
                // ViewData["result"] = 1;
                TempData["result"] = 1;
                TempData["TotImported"] = CLSCODE;
            }
            if (cmd == "Upload")
            {
                string UpdateFilePath = System.Configuration.ConfigurationManager.AppSettings["StaffFile"];
                DataSet ds = new DataSet();
                if (DEO.file.ContentLength > 0)  //(Request.Files["file"].ContentLength > 0
                {
                    string fileName1 = "MIS_" + district + '_' + DateTime.Now.ToString("ddMMyyyyHHmm");  //MIS_201_110720161210

                    string fileExtension = System.IO.Path.GetExtension(DEO.file.FileName);
                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        //fileLocation = Server.MapPath(UpdateFilePath + fileName1 + fileExtension);                     
                        fileLocation = UpdateFilePath + fileName1 + fileExtension;
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
                        DEO.file.SaveAs(fileLocation);
                        System.Threading.Thread.Sleep(10000);

                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
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
                        //string query = string.Format("Select * from [{0}]", excelSheets[0]);
                        string query = string.Format("Select * from [{0}]", excelSheets[1]);
                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(ds);
                        }
                        excelConnection.Close();
                        excelConnection.Dispose();

                        //oWB.Close(true, null, null);
                        //oXL.Quit();
                        string MIS_FILENM = fileName1;
                        string CheckMis = OBJDB.CheckMisExcel(ds, MIS_FILENM);
                        if (CheckMis == "")
                        {

                            string Result1 = "";
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {

                                //string CCODE = ViewBag.ClusterCode;
                                string CCODE = frm["hdnccode"];
                                string TD = ds.Tables[0].Rows[i][0].ToString();
                                string Epunjabid = ds.Tables[0].Rows[i][1].ToString();
                                string Aadharnum = ds.Tables[0].Rows[i][2].ToString();
                                string Name = ds.Tables[0].Rows[i][3].ToString();
                                string fname = ds.Tables[0].Rows[i][4].ToString();
                                string SCHL = ds.Tables[0].Rows[i][5].ToString();
                                string cadre = ds.Tables[0].Rows[i][6].ToString();
                                string desi = ds.Tables[0].Rows[i][7].ToString();
                                string mob = ds.Tables[0].Rows[i][8].ToString();
                                string gen = ds.Tables[0].Rows[i][9].ToString();
                                string expe = ds.Tables[0].Rows[i][10].ToString();
                                string Month = ds.Tables[0].Rows[i][11].ToString();
                                string IFSC = ds.Tables[0].Rows[i][12].ToString();
                                string Accno = ds.Tables[0].Rows[i][13].ToString();
                                string Schoolname = null;

                                //
                                string Staffres = OBJDB.BulkUploadADDSTAFFDETAILS(uid, district, CCODE, TD, fname, Name, expe, Month, gen, mob, SCHL, Schoolname, Aadharnum, Epunjabid, cadre, desi, IFSC, Accno, Session["DeoLoginExamCentre"].ToString());

                                if (Staffres == "1")
                                {
                                    TempData["Uploadresult"] = "1";
                                    // return RedirectToAction("VIEWSTAFF", "DeoPortal");
                                }
                                else if (Staffres == "-1")
                                {
                                    TempData["Uploadresult"] = "-1";
                                    ViewBag.Message = "Please Upload Only .xls and .xlsx only";

                                }
                                else if (Staffres == "0")
                                {
                                    TempData["Uploadresult"] = "0";
                                    ViewBag.Message = "Mobile Number Already Exist";

                                }

                            }
                            if (Result1 == "")
                            {
                                TempData["chkMISResult"] = "All Staffs Uploaded Successfully";
                            }
                            else { TempData["chkMISResult"] = Result1 + ", Rest of the Staffs Uploaded Successfully"; }
                            ViewData["Result"] = "1";

                        }
                        else
                        {

                            TempData["chkMIS"] = "-1";
                            TempData["chkMISMessage"] = CheckMis;


                        }
                    }
                    else
                    {

                        TempData["chkExcelSheet"] = "2";
                        ViewBag.Message = "Please Upload Only .xls and .xlsx only";

                    }
                }
            }
            if (cmd == "Download")
            {
                string UpdateFilePath = System.Configuration.ConfigurationManager.AppSettings["StaffFile"];
                string DownFlname = frm["ErrorList"];
                //string file = @"D:\PSEBONLINE_23012017\PSEBONLINE\PSEBONLINE\StaffUpload\" + DownFlname + '.' + "xls";
                // string file = "https://registration2022.pseb.ac.in/StaffUpload/" + DownFlname + '.' + "xls";
                //string file = UpdateFilePath + DownFlname + '.' + "xls";
                string file = UpdateFilePath + DownFlname + "Copy.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(file, contentType, Path.GetFileName(file));

            }
            //return View(DEO);
            return RedirectToAction("VIEWSTAFF", "DeoPortal");
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult UPDATESTAFF(DEOModel DEO, string id, string CCODE)
        {

            string DeoUser = null;
            string district = null;
            string Catg = null;
            @ViewBag.DA = objCommon.GetDA();
            @ViewBag.exp = OBJDB.GetStaffExp();

            //id = encrypt.QueryStringModule.Decrypt(id);
            //CCODE = encrypt.QueryStringModule.Decrypt(CCODE);

            try
            {
                if (id == null && id == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (CCODE == null && CCODE == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                id = encrypt.QueryStringModule.Decrypt(id);
                CCODE = encrypt.QueryStringModule.Decrypt(CCODE);
                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();

                //--------------------------FILL CADRE----------
                DataSet RCADRE = OBJDB.GetCADRE(); // passing Value to DeoClass from model            
                List<SelectListItem> CDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in RCADRE.Tables[0].Rows)
                {
                    CDList.Add(new SelectListItem { Text = @dr["CADRE"].ToString(), Value = @dr["CADRE"].ToString() });
                }

                ViewBag.Dcadre = CDList;
                // --------------------------------End CADRE---------------------
                //--------------------------------------------Schol Start------------------
                DataSet DistwiseSCHL = OBJDB.GetDeoSchlDISTWise(district); // passing Value to DeoClass from model            
                List<SelectListItem> SDList = new List<SelectListItem>();
                SDList.Add(new SelectListItem { Text = "Others", Value = "Others" });
                foreach (System.Data.DataRow dr in DistwiseSCHL.Tables[0].Rows)
                {
                    SDList.Add(new SelectListItem { Text = @dr["schlnm"].ToString(), Value = @dr["schl"].ToString() });
                }

                ViewBag.schl = SDList;
                //--------------------------------------------Schol End------------------

                DataSet Dresult = OBJDB.GetClusterSCHOOLSTAFF(CCODE, Session["DeoLoginExamCentre"].ToString());
                if (Dresult.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;
                    return View(DEO);
                }
                else
                {
                    ViewBag.ClusterCode = Dresult.Tables[0].Rows[0][0].ToString();
                    ViewBag.ClusterName = Dresult.Tables[0].Rows[0][1].ToString();
                    ViewBag.ClusterSchoolCode = Dresult.Tables[0].Rows[0][2].ToString();
                    ViewBag.Address = Dresult.Tables[0].Rows[0][4].ToString();
                    ViewBag.TotalCount = Dresult.Tables[0].Rows.Count;
                }
                DataSet Staffresult = OBJDB.SelectSTAFFClusterANDStaffWise(id, CCODE, Session["DeoLoginExamCentre"].ToString());
                if (Staffresult.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    //ViewBag.TotalCount = 0;
                    //return View(DEO);
                }
                else
                {
                    //SLNO, CCODE, TeacherName, FatherName, Designation, TypeOfDuty, Mobile
                    @ViewBag.message = "1";

                    DEO.typeDuty = Staffresult.Tables[0].Rows[0]["dcode"].ToString();
                    DEO.StaffName = Staffresult.Tables[0].Rows[0]["name"].ToString();
                    DEO.StaffFatherName = Staffresult.Tables[0].Rows[0]["fname"].ToString();
                    DEO.experience = Staffresult.Tables[0].Rows[0]["expyear"].ToString();
                    DEO.expmonth = Staffresult.Tables[0].Rows[0]["expmonth"].ToString();
                    DEO.gender = Staffresult.Tables[0].Rows[0]["Gender"].ToString();
                    DEO.Mobile = Staffresult.Tables[0].Rows[0]["Mobile"].ToString();
                    DEO.schlcode = Staffresult.Tables[0].Rows[0]["schl"].ToString();
                    //DEO.Selschool = Staffresult.Tables[0].Rows[0]["SchoolName"].ToString();
                    DEO.Selschool = Staffresult.Tables[0].Rows[0]["schl"].ToString();
                    if (DEO.schlcode == "*******")
                    {
                        DEO.Otherschl = Staffresult.Tables[0].Rows[0]["schlnm"].ToString();
                        DEO.Selschool = "Others";
                    }


                    DEO.adharno = Staffresult.Tables[0].Rows[0]["adharno"].ToString();
                    DEO.teacherepunjabid = Staffresult.Tables[0].Rows[0]["epunjabid"].ToString();
                    DEO.cadre = Staffresult.Tables[0].Rows[0]["Cadre"].ToString();
                    DEO.desi = Staffresult.Tables[0].Rows[0]["subject"].ToString();

                    DataSet MDESI = OBJDB.SelectAllDESIGCADREWISE(DEO.cadre);
                    List<SelectListItem> DesiList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in MDESI.Tables[0].Rows)
                    {
                        DesiList.Add(new SelectListItem { Text = @dr["SUBJECT"].ToString(), Value = @dr["SUBJECT"].ToString() });
                    }
                    ViewBag.Mydesi = DesiList;

                    //ViewBag.Mydesi = Staffresult.Tables[0].Rows[0]["Designation"].ToString();
                    DEO.ifsccode = Staffresult.Tables[0].Rows[0]["ifsc"].ToString();
                    DEO.bankaccno = Staffresult.Tables[0].Rows[0]["acno"].ToString();

                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

            return View(DEO);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult UPDATESTAFF(DEOModel DEO, string id, string CCODE, FormCollection frm)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            @ViewBag.DA = objCommon.GetDA();
            @ViewBag.exp = OBJDB.GetStaffExp();

            //id = encrypt.QueryStringModule.Decrypt(id);
            //CCODE = encrypt.QueryStringModule.Decrypt(CCODE);

            try
            {
                if (id == null && id == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                id = encrypt.QueryStringModule.Decrypt(id);
                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();

                //--------------------------FILL CADRE and Design----------
                DataSet RCADRE = OBJDB.GetCADRE(); // passing Value to DeoClass from model            
                List<SelectListItem> CDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in RCADRE.Tables[0].Rows)
                {
                    CDList.Add(new SelectListItem { Text = @dr["CADRE"].ToString(), Value = @dr["CADRE"].ToString() });
                }

                ViewBag.Dcadre = CDList;

                DataSet MDESI = OBJDB.SelectAllDESIGCADREWISE(DEO.cadre);
                List<SelectListItem> DesiList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in MDESI.Tables[0].Rows)
                {
                    DesiList.Add(new SelectListItem { Text = @dr["SUBJECT"].ToString(), Value = @dr["SUBJECT"].ToString() });
                }
                ViewBag.Mydesi = DesiList;

                // --------------------------------End CADRE and Design---------------------
                //--------------------------------------------Schol Start------------------
                DataSet DistwiseSCHL = OBJDB.GetDeoSchlDISTWise(district); // passing Value to DeoClass from model            
                List<SelectListItem> SDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in DistwiseSCHL.Tables[0].Rows)
                {
                    SDList.Add(new SelectListItem { Text = @dr["schlnm"].ToString(), Value = @dr["schl"].ToString() });
                }
                SDList.Add(new SelectListItem { Text = "Others", Value = "Others" });
                ViewBag.schl = SDList;
                //--------------------------------------------Schol End------------------

                string TD = DEO.typeDuty;
                string fname = DEO.StaffFatherName;
                string Name = DEO.StaffName;
                string expe = DEO.experience;
                string Month = DEO.expmonth;
                string gen = DEO.gender;
                string mob = DEO.Mobile;
                string SCHL = DEO.schlcode;
                string Schoolname = DEO.Selschool;
                string Aadharnum = DEO.adharno;
                string Epunjabid = DEO.teacherepunjabid;
                string cadre = DEO.cadre;

                string desi = frm["desi"];
                string IFSC = DEO.ifsccode;
                string Accno = DEO.bankaccno;
                //string selected = Request.Form["Selschool"];
                //string selected12 = frm["Selschool"];
                string OtherSchool = DEO.Otherschl;
                if (Schoolname == "Others")
                {
                    Schoolname = OtherSchool;
                    SCHL = "*******";
                }

                string Staffres = OBJDB.UPDATESTAFFDETAILS(id, TD, fname, Name, expe, Month, gen, mob, SCHL, Schoolname, Aadharnum, Epunjabid, cadre, desi, IFSC, Accno, Session["DeoLoginExamCentre"].ToString());
                if (Staffres == "1")
                {
                    ViewData["result"] = "1";

                }
                else if (Staffres == "-1")
                {
                    ViewData["result"] = "-1";
                }
                else if (Staffres == "0")
                {
                    ViewData["result"] = "0";
                }

            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

            return View(DEO);
        }
        public ActionResult STAFFDELETE(string id)
        {

            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (id == null && id == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                else
                {
                    string result = OBJDB.DeleteStaffData(id);
                    if (result == "Deleted")
                    {
                        // @ViewBag.result = "1";
                        ViewData["result"] = "1";
                        return RedirectToAction("VIEWSTAFF", "DeoPortal");
                        //return View(id);
                    }

                }


                //id = encrypt.QueryStringModule.Decrypt(id);
                //DeoUser = Session["USER"].ToString();
                //district = Session["Dist"].ToString();
                //uid = Session["UID"].ToString();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }

            return RedirectToAction("VIEWSTAFF", "DeoPortal");
        }
        public JsonResult GETExcelErrorlist(string errl) // Calling on http post (on Submit)
        {
            string DeoUser = null;
            string district = null;
            if (Session["USER"] == null && Session["Name"] == null)
            {
                return Json(new { Url = "DeoPortal/Index" });
                //return Json("Index", "DeoPortal");
            }
            DeoUser = Session["USER"].ToString();
            district = Session["Dist"].ToString();
            DataSet result = OBJDB.GetBulkStaffErrorList(errl, district);
            ViewBag.bs = null;
            List<SelectListItem> ErrList = new List<SelectListItem>();
            //List<string> items = new List<string>();
            // ErrList.Add(new SelectListItem { Text = "---Select DESIGNATION---", Value = "0" });

            if (result != null)
            {
                ViewBag.bs = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                foreach (System.Data.DataRow dr in ViewBag.bs.Rows)
                {
                    ErrList.Add(new SelectListItem { Text = @dr["FN"].ToString(), Value = @dr["MIS_FILENM"].ToString() });
                }
                ViewBag.StaffBulkErrorList = ErrList;
            }
            return Json(ErrList);

        }
        public JsonResult GETSchoolName(string SCHL) // Calling on http post (on Submit)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            DataSet result = OBJDB.txtGETSchoolName(SCHL);
            ViewBag.schl = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> SCHList = new List<SelectListItem>();
            //SCHList.Add(new SelectListItem { Text = "---Select School---", Value = "0" });
            foreach (System.Data.DataRow dr in ViewBag.schl.Rows)
            {

                SCHList.Add(new SelectListItem { Text = @dr["schlnm"].ToString(), Value = @dr["SCHL"].ToString() });

            }
            if (result.Tables[0].Rows.Count == 0)
            {
                SCHList.Add(new SelectListItem { Text = "---Select School---", Value = "" });
                //SCHList.Add(new SelectListItem { Text = "Others", Value = "Others" });
            }

            ViewBag.schl = SCHList;
            return Json(SCHList);


        }
        public JsonResult GETDSTAFFESIGNATION(string CAD) // Calling on http post (on Submit)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            DataSet result = OBJDB.SelectAllDESIGCADREWISE(CAD);
            ViewBag.designation = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> desiList = new List<SelectListItem>();
            //List<string> items = new List<string>();
            desiList.Add(new SelectListItem { Text = "---Select DESIGNATION---", Value = "0" });
            foreach (System.Data.DataRow dr in ViewBag.designation.Rows)
            {

                desiList.Add(new SelectListItem { Text = @dr["SUBJECT"].ToString(), Value = @dr["SUBJECT"].ToString() });

            }
            ViewBag.designation = desiList;

            return Json(desiList);

        }
        //public JsonResult GETDSTAFFESIGNATION(string CAD) // Calling on http post (on Submit)
        //{
        //    AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
        //    DataSet result = OBJDB.SelectAllDESIGCADREWISE(CAD);
        //    ViewBag.MyTeh = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
        //    List<SelectListItem> TehList = new List<SelectListItem>();
        //    //List<string> items = new List<string>();
        //    TehList.Add(new SelectListItem { Text = "---Select Tehsil---", Value = "0" });
        //    foreach (System.Data.DataRow dr in ViewBag.MyTeh.Rows)
        //    {

        //        TehList.Add(new SelectListItem { Text = @dr["SUBJECT"].ToString(), Value = @dr["SUBJECT"].ToString() });

        //    }
        //    ViewBag.MyTeh = TehList;

        //    return Json(TehList);

        //}
        public ActionResult ClusterDELETE(string id)
        {

            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (id == null && id == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                else
                {
                    string result = OBJDB.DeleteClusterListData(id);
                    if (result == "Deleted")
                    {
                        TempData["Deleteresult"] = "1";
                    }
                    else if (result == "NotDeleted")
                    {
                        TempData["Deleteresult"] = "-1";
                        //@ViewBag.result = "-1";
                        //Cluster not deleted, due to staff exist in this cluster, Kindly Delete or Shift staff before delete this cluster
                    }

                }


                //id = encrypt.QueryStringModule.Decrypt(id);
                //DeoUser = Session["USER"].ToString();
                //district = Session["Dist"].ToString();
                //uid = Session["UID"].ToString();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }

            return RedirectToAction("ClusterList", "DeoPortal");
        }
        public ActionResult CentreDELETE(string id)
        {

            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (id == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                else
                {
                    string result = OBJDB.DeleteCentreListData(id);
                    if (result == "Deleted")
                    {
                        @ViewBag.result = "1";
                        return RedirectToAction("ClusterList", "DeoPortal");
                    }

                }


                //id = encrypt.QueryStringModule.Decrypt(id);
                //DeoUser = Session["USER"].ToString();
                //district = Session["Dist"].ToString();
                //uid = Session["UID"].ToString();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
            }

            return RedirectToAction("ClusterList", "DeoPortal");
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ADDCENT(int? page, string CCODEID, FormCollection frm)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            try
            {
                if (page >= 1 && Session["CCODEID"] != null)
                {
                    CCODEID = Session["CCODEID"].ToString();

                }
                else
                {
                    CCODEID = encrypt.QueryStringModule.Decrypt(CCODEID);
                    Session["CCODEID"] = CCODEID;
                }
                if (CCODEID == null && CCODEID == "")
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    district = frm["SelDist"];
                    //string[] ArrInvoice = { "010","020","025","030","040","045","050","060","070","080" };
                    //string[] ArrInvoice = { "Invoice # 120", "Invoice # 121", "Invoice # 122", "Invoice # 123" };
                    //List<int> lstInvice = new List<int>();
                    //List<SelectListItem> DList = new List<SelectListItem>();
                    //foreach (string item in ArrInvoice)
                    //{
                    //    //int intInvocie = 0;
                    //    //Int32.TryParse(item.Split(',')[0], out intInvocie);
                    //    //if (intInvocie > 0)
                    //    //{
                    //    //    //lstInvice.Add(intInvocie);
                    //    //    DList.Add(new SelectListItem { Text = "yiy", Value = Convert.ToString(intInvocie)});
                    //    //}


                    //}
                    //string DC = "010,020,025,030,040,045,050,060,070,080";
                    //// Split string on spaces.
                    //// ... This will separate all the words.
                    //string[] DistCodes = DC.Split(',');
                    //foreach (string DistCode in DistCodes)
                    //{
                    //    DList.Add(new SelectListItem { Text = "yiy", Value = DistCode });
                    //}

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                //-----------------------------------------------------------Cluster View Page Starts---------------
                DataSet Cresult = OBJDB.GetClusterNameBuldingCount(CCODEID, Session["DeoLoginExamCentre"].ToString());
                if (Cresult.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;

                }
                else
                {
                    ViewBag.ClusterCode = Cresult.Tables[0].Rows[0][0].ToString();
                    ViewBag.ClusterName = Cresult.Tables[0].Rows[0][1].ToString();
                    ViewBag.ClusterSchoolCode = Cresult.Tables[0].Rows[0][2].ToString();
                    ViewBag.CCENT = Cresult.Tables[0].Rows[0][3].ToString();
                    ViewBag.Address = Cresult.Tables[0].Rows[0][4].ToString();
                    ViewBag.centrecnt = Cresult.Tables[0].Rows[0][5].ToString();
                    ViewBag.TotalCountCluster = Cresult.Tables[0].Rows.Count;
                }
                //----------------------- Page End-------------------------

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    if (DeoUser.ToUpper() == "ADMIN")
                    {
                        district = frm["SelDist"];
                        Search += "a.Dist='" + district + "'";
                    }
                    else
                        Search = "a.DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }

                Search += " and  ExamMonth = '" + Session["DeoLoginExamCentre"].ToString() + "'";
                obj.StoreAllData = OBJDB.Select_ADD_CenterListByUser(Search, Catg, pageIndex);
                obj.TotalCount = OBJDB.Select_ADD_CenterListByUserCount(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int pn = tp / 10;
                    int cal = 10 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;
                    obj.chkidList = new List<CntreIDModel>();

                    CntreIDModel chk = null;
                    for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                    {
                        chk = new CntreIDModel();
                        chk.id = obj.StoreAllData.Tables[0].Rows[i]["CENT"].ToString();
                        chk.Name = "chkidList[" + i + "].CENT";
                        if (obj.StoreAllData.Tables[0].Rows.Count == 1)
                            ViewBag.impid = obj.StoreAllData.Tables[0].Rows[i]["CENT"].ToString();
                        chk.Selected = false;
                        obj.chkidList.Add(chk);
                    }
                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult ADDCENT(int? page, string CCODEID, DEOModel DEO, FormCollection frm, string cmd, string SelDist, string Category, string SearchString)
        {

            string DeoUser = null;
            string district = null;
            string Catg = null;
            try
            {
                //if (CCODEID == null && CCODEID == "")
                //{
                //    return RedirectToAction("Index", "DeoPortal");
                //}
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                CCODEID = Session["CCODEID"].ToString();
                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    //string[] ArrInvoice = { "010","020","025","030","040","045","050","060","070","080" };
                    //string[] ArrInvoice = { "Invoice # 120", "Invoice # 121", "Invoice # 122", "Invoice # 123" };
                    //List<int> lstInvice = new List<int>();
                    //List<SelectListItem> DList = new List<SelectListItem>();
                    //foreach (string item in ArrInvoice)
                    //{
                    //    //int intInvocie = 0;
                    //    //Int32.TryParse(item.Split(',')[0], out intInvocie);
                    //    //if (intInvocie > 0)
                    //    //{
                    //    //    //lstInvice.Add(intInvocie);
                    //    //    DList.Add(new SelectListItem { Text = "yiy", Value = Convert.ToString(intInvocie)});
                    //    //}


                    //}
                    //string DC = "010,020,025,030,040,045,050,060,070,080";
                    //// Split string on spaces.
                    //// ... This will separate all the words.
                    //string[] DistCodes = DC.Split(',');
                    //foreach (string DistCode in DistCodes)
                    //{
                    //    DList.Add(new SelectListItem { Text = "yiy", Value = DistCode });
                    //}

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;

                }

                //-----------------------------------------------------------Cluster View Page Starts---------------
                DataSet Cresult = OBJDB.GetClusterNameBuldingCount(CCODEID, Session["DeoLoginExamCentre"].ToString());
                if (Cresult.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;

                }
                else
                {
                    ViewBag.ClusterCode = Cresult.Tables[0].Rows[0][0].ToString();
                    ViewBag.ClusterName = Cresult.Tables[0].Rows[0][1].ToString();
                    ViewBag.ClusterSchoolCode = Cresult.Tables[0].Rows[0][2].ToString();
                    ViewBag.CCENT = Cresult.Tables[0].Rows[0][3].ToString();
                    ViewBag.Address = Cresult.Tables[0].Rows[0][4].ToString();
                    ViewBag.centrecnt = Cresult.Tables[0].Rows[0][5].ToString();
                    ViewBag.TotalCountCluster = Cresult.Tables[0].Rows.Count;
                }
                //----------------------- Page End-------------------------

                ////-----------------------------------------------------------Cluster View Page Starts---------------
                //DataSet Cresult = OBJDB.GetClusterSCHOOLSTAFF(CCODEID);
                //if (Cresult.Tables[0].Rows.Count == 0)
                //{
                //    ViewBag.Message = "DATA DOESN'T EXIST";
                //    ViewBag.TotalCount = 0;

                //}
                //else
                //{
                //    ViewBag.ClusterCode = Cresult.Tables[0].Rows[0][0].ToString();
                //    ViewBag.ClusterName = Cresult.Tables[0].Rows[0][1].ToString();
                //    ViewBag.ClusterSchoolCode = Cresult.Tables[0].Rows[0][2].ToString();
                //    ViewBag.CCENT = Cresult.Tables[0].Rows[0][3].ToString();
                //    ViewBag.Address = Cresult.Tables[0].Rows[0][4].ToString();
                //    ViewBag.TotalCount = Cresult.Tables[0].Rows.Count;
                //}
                ////----------------------- Page End-------------------------

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    if (DeoUser.ToUpper() == "ADMIN")
                    {
                        district = frm["SelDist"];
                        Search += "a.Dist='" + SelDist.ToString().Trim() + "'";
                    }
                    else
                        Search = "a.DIST = '" + district + "'";
                //Search = "a.DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }
                obj.StoreAllData = OBJDB.Select_ADD_CenterListByUser(Search, Catg, pageIndex);
                obj.TotalCount = OBJDB.Select_ADD_CenterListByUserCount(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int pn = tp / 10;
                    int cal = 10 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;


                    //return View(obj);
                }
                //----------------- Import Buttion Click---------------------------
                if (cmd == "ADD SELECTED CENTER TO THIS BLOCK")
                {
                    string[] IDs = frm["CenterName"].Split(new Char[] { ',' });
                    string chkid = frm["CenterName"];
                    TempData["TotImported"] = IDs.Count();
                    try
                    {
                        string Centresult = OBJDB.Update_CCODE_To_CENTRE(CCODEID, chkid);
                        if (Centresult == "0")
                        {
                            //--------------Not Updated
                            // ViewData["result"] = 0;
                            TempData["result"] = 0;
                        }
                        else
                        {
                            //-------------- Updated----------
                            // ViewData["result"] = 1;
                            TempData["result"] = 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                        return RedirectToAction("Index", "DeoPortal");
                    }

                }
                if (cmd == "Search")
                {

                    if (SelDist != "")
                    {
                        Search = "a.Dist='" + SelDist.ToString().Trim() + "'";

                    }
                    if (SelDist != "")
                    {
                        ViewBag.SelectedItem = SelDist;
                        int SelValueSch = Convert.ToInt32(Category.ToString());
                        if (SelValueSch == 1)
                        { Search += " and a.Cent='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueSch == 2)
                        { Search += " and  a.ecentre like '%" + SearchString.ToString().Trim() + "%'"; }
                        else if (SelValueSch == 3)
                        { Search += " and  a.schoole like '%" + SearchString.ToString().Trim() + "%'"; }
                        else if (SelValueSch == 4)
                        { Search += " and a.CCODE='" + SearchString.ToString().Trim() + "'"; }
                    }



                }
                TempData["CenterListSelDist"] = Search;
                TempData["CenterListSelectList"] = Category;
                TempData["CenterListSearchString"] = SearchString.ToString().Trim();

                ViewBag.Searchstring = SearchString.ToString().Trim();
                DEO.StoreAllData = OBJDB.Select_ADD_CenterListByUser(Search, Catg, pageIndex);
                DEO.TotalCount = OBJDB.Select_ADD_CenterListByUserCount(Search, Catg, pageIndex);
                //rm.StoreAllData = objDB.GetStudentRecordsSearch_ImportData(Search);
                // asm.StoreAllData = objDB.SearchSchoolDetails(Search);
                if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(DEO.TotalCount.Tables[0].Rows[0]["decount"]);
                    int tp = Convert.ToInt32(DEO.TotalCount.Tables[0].Rows[0]["decount"]);
                    int pn = tp / 10;
                    int cal = 10 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;


                    return View(DEO);
                }
                //-----------------------------------------------------------------

                //return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult VIEWCENTRE(int? page, string CCODEID)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (CCODEID != null && CCODEID != "")
                {
                    Session["vcclsCode"] = CCODEID;

                }
                else
                {
                    CCODEID = Session["vcclsCode"].ToString();
                }
                CCODEID = encrypt.QueryStringModule.Decrypt(CCODEID);

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();


                DataSet DCls = OBJDB.GetClusterSTAFFWise(CCODEID, district);
                if (DCls.Tables[0].Rows.Count > 0)
                {
                    ViewBag.TotalCount = DCls.Tables[0].Rows.Count;
                    //--------------------------FILL ClusterSTAFFWise----------                           
                    List<SelectListItem> ClsList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in DCls.Tables[0].Rows)
                    {
                        ClsList.Add(new SelectListItem { Text = @dr["clusternam"].ToString(), Value = @dr["ccode"].ToString() });
                    }

                    ViewBag.ClstCode = ClsList;
                    // --------------------------------End ClusterSTAFFWise---------------------


                }
                else
                {
                    ViewBag.Message = "There Is No Cluster";
                    return View();
                }
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    //string[] ArrInvoice = { "010","020","025","030","040","045","050","060","070","080" };
                    //string[] ArrInvoice = { "Invoice # 120", "Invoice # 121", "Invoice # 122", "Invoice # 123" };
                    //List<int> lstInvice = new List<int>();
                    //List<SelectListItem> DList = new List<SelectListItem>();
                    //foreach (string item in ArrInvoice)
                    //{
                    //    //int intInvocie = 0;
                    //    //Int32.TryParse(item.Split(',')[0], out intInvocie);
                    //    //if (intInvocie > 0)
                    //    //{
                    //    //    //lstInvice.Add(intInvocie);
                    //    //    DList.Add(new SelectListItem { Text = "yiy", Value = Convert.ToString(intInvocie)});
                    //    //}


                    //}
                    //string DC = "010,020,025,030,040,045,050,060,070,080";
                    //// Split string on spaces.
                    //// ... This will separate all the words.
                    //string[] DistCodes = DC.Split(',');
                    //foreach (string DistCode in DistCodes)
                    //{
                    //    DList.Add(new SelectListItem { Text = "yiy", Value = DistCode });
                    //}

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                //-----------------------------------------------------------Cluster View Page Starts---------------
                //DataSet Cresult = OBJDB.GetClusterSCHOOLSTAFF(CCODEID);
                DataSet Cresult = OBJDB.GetClusterNameBuldingCount(CCODEID, Session["DeoLoginExamCentre"].ToString());
                if (Cresult.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;

                }
                else
                {
                    ViewBag.massage = 0;
                    ViewBag.ClusterCode = Cresult.Tables[0].Rows[0][0].ToString();
                    ViewBag.ClusterName = Cresult.Tables[0].Rows[0][1].ToString();
                    ViewBag.ClusterSchoolCode = Cresult.Tables[0].Rows[0][2].ToString();
                    ViewBag.CCENT = Cresult.Tables[0].Rows[0][3].ToString();
                    ViewBag.Address = Cresult.Tables[0].Rows[0][4].ToString();
                    ViewBag.centrecnt = Cresult.Tables[0].Rows[0][5].ToString();
                    ViewBag.TotalCountCluster = Cresult.Tables[0].Rows.Count;
                }
                //----------------------- Page End-------------------------
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)

                {
                    Search = "a.DIST = '" + district + "'";
                    Search += " and a.CCODE='" + CCODEID + "'";
                }

                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }

                Search += " and ExamMonth = '" + Session["DeoLoginExamCentre"].ToString() + "'";
                obj.StoreAllData = OBJDB.ViewClusterCentre(Search, Catg, pageIndex);
                obj.TotalCount = OBJDB.SelectCenterListByUserCount(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int pn = tp / 10;
                    int cal = 10 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult VIEWCENTRE(int? page, DEOModel DEO, FormCollection frm, string cmd, string SelDist, string Category, string SearchString)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                string CCID = frm["hdnccode"];
                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                DataSet DCls = OBJDB.GetClusterSTAFFWise(CCID, district);
                if (DCls.Tables[0].Rows.Count > 0)
                {
                    ViewBag.TotalCount = DCls.Tables[0].Rows.Count;
                    //--------------------------FILL ClusterSTAFFWise----------                           
                    List<SelectListItem> ClsList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in DCls.Tables[0].Rows)
                    {
                        ClsList.Add(new SelectListItem { Text = @dr["clusternam"].ToString(), Value = @dr["ccode"].ToString() });
                    }

                    ViewBag.ClstCode = ClsList;
                    // --------------------------------End ClusterSTAFFWise---------------------


                }
                else
                {
                    ViewBag.Message = "There Is No Cluster";
                    return View();
                }
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                //-----------------------------------------------------------Cluster View Page Starts---------------
                //DataSet Cresult = OBJDB.GetClusterSCHOOLSTAFF(CCID);
                DataSet Cresult = OBJDB.GetClusterNameBuldingCount(CCID, Session["DeoLoginExamCentre"].ToString());
                if (Cresult.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;

                }
                else
                {
                    ViewBag.ClusterCode = Cresult.Tables[0].Rows[0][0].ToString();
                    ViewBag.ClusterName = Cresult.Tables[0].Rows[0][1].ToString();
                    ViewBag.ClusterSchoolCode = Cresult.Tables[0].Rows[0][2].ToString();
                    ViewBag.CCENT = Cresult.Tables[0].Rows[0][3].ToString();
                    ViewBag.Address = Cresult.Tables[0].Rows[0][4].ToString();
                    ViewBag.centrecnt = Cresult.Tables[0].Rows[0][5].ToString();
                    ViewBag.TotalCountCluster = Cresult.Tables[0].Rows.Count;
                }
                //----------------------- Page End-------------------------
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                Catg = DEO.Category;

                string Search = "";
                if (cmd == "Search")
                {

                    if (SelDist != "")
                    {
                        Search += "a.Dist='" + SelDist.ToString().Trim() + "' and a.CCODE='" + CCID + "'";


                    }
                    if (SelDist != "")
                    {
                        ViewBag.SelectedItem = SelDist;
                        int SelValueSch = Convert.ToInt32(Category.ToString());
                        if (SelValueSch == 1)
                        { Search += " and a.Cent='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueSch == 2)
                        { Search += " and  a.ecentre like '%" + SearchString.ToString().Trim() + "%'"; }
                        else if (SelValueSch == 3)
                        { Search += " and  a.schoole like '%" + SearchString.ToString().Trim() + "%'"; }
                        //else if (SelValueSch == 4)
                        //{ Search += " and a.CCODE='" + SearchString.ToString().Trim() + "'"; }
                    }


                    TempData["CenterListSelDist"] = Search;
                    TempData["CenterListSelectList"] = Category;
                    TempData["CenterListSearchString"] = SearchString.ToString().Trim();

                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    DEO.StoreAllData = OBJDB.ViewClusterCentre(Search, Catg, pageIndex);
                    DEO.TotalCount = OBJDB.SelectCenterListByUserCount(Search, Catg, pageIndex);
                    //rm.StoreAllData = objDB.GetStudentRecordsSearch_ImportData(Search);
                    // asm.StoreAllData = objDB.SearchSchoolDetails(Search);
                    if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "DATA DOESN'T EXIST";
                        ViewBag.TotalCount = 0;

                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(DEO.TotalCount.Tables[0].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(DEO.TotalCount.Tables[0].Rows[0]["decount"]);
                        int pn = tp / 10;
                        int cal = 10 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;


                        return View(DEO);
                    }
                }
                //---------------------------------Shift Centre To Other Cluster---------------------
                if (cmd == "Shift Centre To Block")
                {
                    //string CCODEID= frm["hdnccode"];
                    string CLSCODE = frm["ClusterCode"];
                    string CHKCent = frm["CentreName"];
                    string OldClsid = frm["hdnccode"];
                    DEO.ClusterCode = OldClsid;
                    string StaffClusterresult = OBJDB.Update_Cluster_To_CentreShift(CLSCODE, district, CHKCent, OldClsid);
                    if (StaffClusterresult == "0")
                    {
                        //--------------Not Updated
                        // ViewData["result"] = 0;
                        TempData["Centreresult"] = 0;
                    }
                    else
                    {
                        //-------------- Updated----------
                        // ViewData["result"] = 1;
                        TempData["Centreresult"] = 1;
                        TempData["ClusterImported"] = CLSCODE;
                    }
                    return RedirectToAction("VIEWCENTRE", "DeoPortal");

                }
                //----------------------------------End Shift Centre To Other Cluster-----------

                return View(DEO);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ADDCLUSTER(int? page, FormCollection frm, string cmd)
        {
            DEOModel obj = new DEOModel();

            string DeoUser = null;
            string district = null;
            string Catg = null;
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    //string[] ArrInvoice = { "010","020","025","030","040","045","050","060","070","080" };
                    //string[] ArrInvoice = { "Invoice # 120", "Invoice # 121", "Invoice # 122", "Invoice # 123" };
                    //List<int> lstInvice = new List<int>();
                    //List<SelectListItem> DList = new List<SelectListItem>();
                    //foreach (string item in ArrInvoice)
                    //{
                    //    //int intInvocie = 0;
                    //    //Int32.TryParse(item.Split(',')[0], out intInvocie);
                    //    //if (intInvocie > 0)
                    //    //{
                    //    //    //lstInvice.Add(intInvocie);
                    //    //    DList.Add(new SelectListItem { Text = "yiy", Value = Convert.ToString(intInvocie)});
                    //    //}


                    //}
                    //string DC = "010,020,025,030,040,045,050,060,070,080";
                    //// Split string on spaces.
                    //// ... This will separate all the words.
                    //string[] DistCodes = DC.Split(',');
                    //foreach (string DistCode in DistCodes)
                    //{
                    //    DList.Add(new SelectListItem { Text = "yiy", Value = DistCode });
                    //}

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    Search = "a.DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }
                //obj.StoreAllData = OBJDB.SelectCenterListByUser(Search, Catg, pageIndex);

                Search += " and ExamMonth = '" + Session["DeoLoginExamCentre"].ToString() + "'";
                obj.StoreAllData = OBJDB.Select_ADD_CenterListByUser(Search, Catg, pageIndex);
                obj.TotalCount = OBJDB.Select_ADD_CenterListByUserCount(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int pn = tp / 10;
                    int cal = 10 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult ADDCLUSTER(int? page, DEOModel DEO, FormCollection frm, string cmd, string SelDist, string Category, string SearchString)
        {
            DEOModel obj = new DEOModel();

            string DeoUser = null;
            string district = null;
            string Catg = null;
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    //string[] ArrInvoice = { "010","020","025","030","040","045","050","060","070","080" };
                    //string[] ArrInvoice = { "Invoice # 120", "Invoice # 121", "Invoice # 122", "Invoice # 123" };
                    //List<int> lstInvice = new List<int>();
                    //List<SelectListItem> DList = new List<SelectListItem>();
                    //foreach (string item in ArrInvoice)
                    //{
                    //    //int intInvocie = 0;
                    //    //Int32.TryParse(item.Split(',')[0], out intInvocie);
                    //    //if (intInvocie > 0)
                    //    //{
                    //    //    //lstInvice.Add(intInvocie);
                    //    //    DList.Add(new SelectListItem { Text = "yiy", Value = Convert.ToString(intInvocie)});
                    //    //}


                    //}
                    //string DC = "010,020,025,030,040,045,050,060,070,080";
                    //// Split string on spaces.
                    //// ... This will separate all the words.
                    //string[] DistCodes = DC.Split(',');
                    //foreach (string DistCode in DistCodes)
                    //{
                    //    DList.Add(new SelectListItem { Text = "yiy", Value = DistCode });
                    //}

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                Catg = DEO.Category;

                string Search = "";
                if (cmd == "Search")
                {

                    if (SelDist != "")
                    {
                        Search += "a.Dist='" + SelDist.ToString().Trim() + "'";


                    }
                    if (SelDist != "")
                    {
                        ViewBag.SelectedItem = SelDist;
                        int SelValueSch = Convert.ToInt32(Category.ToString());
                        if (SelValueSch == 1)
                        { Search += " and a.Cent='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueSch == 2)
                        { Search += " and  a.ecentre like '%" + SearchString.ToString().Trim() + "%'"; }
                        else if (SelValueSch == 3)
                        { Search += " and  a.schoole like '%" + SearchString.ToString().Trim() + "%'"; }
                        //else if (SelValueSch == 4)
                        //{ Search += " and a.CCODE='" + SearchString.ToString().Trim() + "'"; }
                    }


                    TempData["CenterListSelDist"] = Search;
                    TempData["CenterListSelectList"] = Category;
                    TempData["CenterListSearchString"] = SearchString.ToString().Trim();

                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    // DEO.StoreAllData = OBJDB.ViewClusterCentre(Search, Catg, pageIndex);
                    //DEO.StoreAllData = OBJDB.SelectCenterListByUser(Search, Catg, pageIndex);
                    // DEO.TotalCount = OBJDB.SelectCenterListByUserCount(Search, Catg, pageIndex);
                    //rm.StoreAllData = objDB.GetStudentRecordsSearch_ImportData(Search);
                    // asm.StoreAllData = objDB.SearchSchoolDetails(Search);

                    Search += " and ExamMonth = '" + Session["DeoLoginExamCentre"].ToString() + "'";
                    DEO.StoreAllData = OBJDB.Select_ADD_CenterListByUser(Search, Catg, pageIndex);
                    DEO.TotalCount = OBJDB.Select_ADD_CenterListByUserCount(Search, Catg, pageIndex);
                    if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "DATA DOESN'T EXIST";
                        ViewBag.TotalCount = 0;

                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(DEO.TotalCount.Tables[0].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(DEO.TotalCount.Tables[0].Rows[0]["decount"]);
                        int pn = tp / 10;
                        int cal = 10 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;


                        return View(DEO);
                    }
                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

        }
        //---------------------------------Examiner Portal--------------------------ADDExaminer
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ADDExaminer(DEOModel DEO)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            //if (id == null && id == null)
            //{
            //    return RedirectToAction("Index", "DeoPortal");
            //}
            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }

            // id = encrypt.QueryStringModule.Decrypt(id);
            DeoUser = Session["USER"].ToString();
            district = Session["Dist"].ToString();
            // string cls = "1";// DEO.Class;
            // string cls = DEO.Class;


            DataSet DistwiseSCHL = OBJDB.GetDeoSchlDISTWise(district); // passing Value to DeoClass from model            
            List<SelectListItem> SDList = new List<SelectListItem>();
            SDList.Add(new SelectListItem { Text = "Others", Value = "Others" });
            foreach (System.Data.DataRow dr in DistwiseSCHL.Tables[0].Rows)
            {
                SDList.Add(new SelectListItem { Text = @dr["schlnm"].ToString(), Value = @dr["schl"].ToString() });

            }
            ViewBag.Myschl = SDList;


            DataSet result = OBJDB.SelectDist();
            ViewBag.MyDist = result.Tables[0];
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
            {
                items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
            }
            ViewBag.MyDist = new SelectList(items, "Value", "Text");



            @ViewBag.texp = OBJDB.GetStaffExp();
            @ViewBag.evaexp = OBJDB.GetStaffExp();

            //--------------------------FILL CADRE----------
            DataSet RCADRE = OBJDB.GetCADRE(); // passing Value to DeoClass from model            
            List<SelectListItem> CDList = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in RCADRE.Tables[0].Rows)
            {
                CDList.Add(new SelectListItem { Text = @dr["CADRE"].ToString(), Value = @dr["CADRE"].ToString() });
            }

            ViewBag.Dcadre = CDList;
            // --------------------------------End CADRE---------------------
            DataSet Desi = OBJDB.GetSubDesi();
            ViewBag.Mydesi = Desi.Tables[0];
            List<SelectListItem> Desiitems = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.Mydesi.Rows)
            {
                Desiitems.Add(new SelectListItem { Text = @dr["SUBJECT"].ToString(), Value = @dr["SUBJECT"].ToString() });
            }
            ViewBag.Mydesi = new SelectList(Desiitems, "Value", "Text");
            //--------------------------Select last Entry---------------------
            DataSet seleLastCan = OBJDB.SelectlastEntryExaminer(district);
            if (seleLastCan.Tables[0].Rows.Count > 0)
            {

                @ViewBag.message = "1";
                @ViewBag.MyClass = seleLastCan.Tables[0].Rows[0]["Class"].ToString();
                @ViewBag.SubjectList = seleLastCan.Tables[0].Rows[0]["SubjectList"].ToString();
                @ViewBag.Name = seleLastCan.Tables[0].Rows[0]["Name"].ToString();
                @ViewBag.Fname = seleLastCan.Tables[0].Rows[0]["fname"].ToString();
                @ViewBag.degi = seleLastCan.Tables[0].Rows[0]["Designation"].ToString();
                @ViewBag.schoolname = seleLastCan.Tables[0].Rows[0]["SchoolCode"].ToString();
                @ViewBag.email = seleLastCan.Tables[0].Rows[0]["EmailID"].ToString();
                @ViewBag.mob = seleLastCan.Tables[0].Rows[0]["Mobile"].ToString();
                @ViewBag.Quali = seleLastCan.Tables[0].Rows[0]["Quali"].ToString();
                @ViewBag.Exp = seleLastCan.Tables[0].Rows[0]["Teachingexp"].ToString();
            }
            else
            {
                @ViewBag.message = "Record Not Found";
            }
            //----------------------------

            return View(DEO);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult ADDExaminer(DEOModel DEO, FormCollection frm)
        {
            string DeoUser = null;
            string district = null;
            int udid = 0;


            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }

            // id = encrypt.QueryStringModule.Decrypt(id);
            DeoUser = Session["USER"].ToString();
            district = Session["Dist"].ToString();
            udid = Convert.ToInt32(Session["UID"].ToString());

            DataSet DistwiseSCHL = OBJDB.GetDeoSchlDISTWise(district); // passing Value to DeoClass from model            
            List<SelectListItem> SDList = new List<SelectListItem>();
            SDList.Add(new SelectListItem { Text = "Others", Value = "Others" });
            foreach (System.Data.DataRow dr in DistwiseSCHL.Tables[0].Rows)
            {
                SDList.Add(new SelectListItem { Text = @dr["schlnm"].ToString(), Value = @dr["schl"].ToString() });

            }
            ViewBag.Myschl = SDList;


            DataSet result = OBJDB.SelectDist();
            ViewBag.MyDist = result.Tables[0];
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
            {
                items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
            }
            ViewBag.MyDist = new SelectList(items, "Value", "Text");

            DataSet Desi = OBJDB.GetSubDesi();
            ViewBag.Mydesi = Desi.Tables[0];

            //--------------------------FILL CADRE----------
            DataSet RCADRE = OBJDB.GetCADRE(); // passing Value to DeoClass from model            
            List<SelectListItem> CDList = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in RCADRE.Tables[0].Rows)
            {
                CDList.Add(new SelectListItem { Text = @dr["CADRE"].ToString(), Value = @dr["CADRE"].ToString() });
            }

            ViewBag.Dcadre = CDList;
            // --------------------------------End CADRE---------------------
            //List<SelectListItem> Desiitems = new List<SelectListItem>();
            //foreach (System.Data.DataRow dr in ViewBag.Mydesi.Rows)
            //{
            //    Desiitems.Add(new SelectListItem { Text = @dr["SUBJECT"].ToString(), Value = @dr["SUBJECT"].ToString() });
            //}
            //ViewBag.Mydesi = new SelectList(Desiitems, "Value", "Text");


            @ViewBag.texp = OBJDB.GetStaffExp();
            @ViewBag.evaexp = OBJDB.GetStaffExp();


            try
            {
                string cls = DEO.Class;
                string SelListItem = frm["Subjects"];
                string[] SplitText = SelListItem.Substring(0, 5).Split(',');
                string subcode = String.Join(",", SplitText);
                //string subcode= String.Join(",", SelListItem.Split(',').Select(c => Convert.ToInt32(c).ToString()));
                string name = DEO.StaffName;
                string Fname = DEO.StaffFatherName;
                string desi = DEO.desi;
                string Schoolcode = DEO.Selschool;
                string emailid = DEO.mailid;
                string mobno = DEO.Mobile;
                string Quali = DEO.Quali;
                string Adrs = DEO.homeaddress;
                string hdist = DEO.homedist;
                string hteh = DEO.hometehsil;
                string pincode = DEO.homepincode;
                string texp = DEO.teachingexp;
                string eexp = DEO.Evaluationexp;
                string remark = DEO.remarks;
                string cadre = DEO.cadre;
                string DOR = DEO.DOR;

                string OtherSchool = DEO.Otherschl;
                if (Schoolcode == "Others")
                {
                    Schoolcode = OtherSchool;
                    //SCHL = "*******";
                }


                //string Staffres = OBJDB.ADDSTAFFDETAILS(uid, district, CCODE, TD, fname, Name, expe, Month, gen, mob, SCHL, Schoolname, Aadharnum, Epunjabid, cadre, desi, IFSC, Accno);
                string AddExaminer = OBJDB.AddExaminerDETAILS(cls, SelListItem, name, Fname, desi, Schoolcode, emailid, mobno, Quali, Adrs, hdist, hteh, pincode, texp, eexp, remark, udid, district, subcode, cadre, DOR);
                if (AddExaminer != "0")
                {
                    ViewData["result"] = "1";

                    //--------------------------Select last Entry---------------------
                    DataSet seleLastCan = OBJDB.SelectlastEntryExaminer(district);
                    if (seleLastCan.Tables[0].Rows.Count > 0)
                    {

                        @ViewBag.message = "1";
                        @ViewBag.Class = seleLastCan.Tables[0].Rows[0]["Class"].ToString();
                        @ViewBag.SubjectList = seleLastCan.Tables[0].Rows[0]["SubjectList"].ToString();
                        @ViewBag.Name = seleLastCan.Tables[0].Rows[0]["Name"].ToString();
                        @ViewBag.Fname = seleLastCan.Tables[0].Rows[0]["fname"].ToString();
                        @ViewBag.degi = seleLastCan.Tables[0].Rows[0]["Designation"].ToString();
                        @ViewBag.schoolname = seleLastCan.Tables[0].Rows[0]["SchoolCode"].ToString();
                        @ViewBag.email = seleLastCan.Tables[0].Rows[0]["EmailID"].ToString();
                        @ViewBag.mob = seleLastCan.Tables[0].Rows[0]["Mobile"].ToString();
                        @ViewBag.Quali = seleLastCan.Tables[0].Rows[0]["Quali"].ToString();
                        @ViewBag.Exp = seleLastCan.Tables[0].Rows[0]["Teachingexp"].ToString();
                    }
                    else
                    {
                        @ViewBag.message = "Record Not Found";
                    }
                    //----------------------------
                }
                else
                {
                    ViewData["result"] = "-1";
                }



            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }


            return View(DEO);
        }
        public JsonResult GetTehID(int DIST) // Calling on http post (on Submit)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            DataSet result = objDB.SelectAllTehsil(DIST);
            ViewBag.MyTeh = result.Tables[0];
            List<SelectListItem> TehList = new List<SelectListItem>();
            TehList.Add(new SelectListItem { Text = "---Select Tehsil---", Value = "0" });
            foreach (System.Data.DataRow dr in ViewBag.MyTeh.Rows)
            {
                TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });
            }
            ViewBag.MyTeh = TehList;
            return Json(TehList);

        }
        public JsonResult GetSub(string cls) // Calling on http post (on Submit)
        {
            DataSet SubClass = OBJDB.GetLassWiseSubjects(cls); // passing Value to DeoClass from model            
            List<SelectListItem> SubList = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in SubClass.Tables[0].Rows)
            {
                SubList.Add(new SelectListItem { Text = @dr["SubENG"].ToString(), Value = @dr["SubENG"].ToString() });
            }
            ViewBag.lstsubs = SubList;
            return Json(SubList);
        }

        public ActionResult Examiner(DEOModel DEO, FormCollection frm)
        {

            string DeoUser = null;
            string district = null;
            string Catg = null;
            //if (id == null && id == null)
            //{
            //    return RedirectToAction("Index", "DeoPortal");
            //}
            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }

            // id = encrypt.QueryStringModule.Decrypt(id);
            DeoUser = Session["USER"].ToString();
            district = Session["Dist"].ToString();

            DEO.StoreAllData = OBJDB.ViewAllExaminerDistwise(district);
            if (DEO.StoreAllData == null) //DEO.StoreAllData.Tables[0].Rows.Count == 0 ||
            {
                ViewBag.Message = "DATA DOESN'T EXIST";
                ViewBag.TotalCount = 0;
                //ViewBag.TotalCount = 0;
                //return View(DEO);
            }
            else
            {
                //SLNO, CCODE, TeacherName, FatherName, Designation, TypeOfDuty, Mobile
                @ViewBag.message = "1";
                ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                ViewBag.cnt10 = DEO.StoreAllData.Tables[1].Rows[0]["cnt10"].ToString();
                ViewBag.cnt12 = DEO.StoreAllData.Tables[1].Rows[0]["cnt12"].ToString();
                ViewBag.Class10WiseSub = DEO.StoreAllData.Tables[2].Rows[0]["sub10"].ToString();
                ViewBag.Class12WiseSub = DEO.StoreAllData.Tables[3].Rows[0]["sub12"].ToString();
                ViewBag.TotalRecords = DEO.StoreAllData.Tables[1].Rows[0]["cntTotal"].ToString();

            }
            return View(DEO);
        }
        [HttpPost]
        public ActionResult Examiner(DEOModel DEO, FormCollection frm, string cmd, string Category, string SearchString)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            //if (id == null && id == null)
            //{
            //    return RedirectToAction("Index", "DeoPortal");
            //}
            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }

            // id = encrypt.QueryStringModule.Decrypt(id);
            DeoUser = Session["USER"].ToString();
            district = Session["Dist"].ToString();
            string Search = "";
            if (cmd == "Search")
            {


                if (Category != "")
                {
                    ViewBag.SelectedItem = Category;
                    int SelValueSch = Convert.ToInt32(Category.ToString());
                    if (SelValueSch == 1)
                    { Search = " Examinerid='" + SearchString.ToString().Trim() + "'"; }
                    else if (SelValueSch == 2)
                    { Search = " Name like '%" + SearchString.ToString().Trim() + "%'"; }
                    else if (SelValueSch == 3)
                    { Search = " Designation like '%" + SearchString.ToString().Trim() + "%'"; }
                    else if (SelValueSch == 4)
                    { Search = " Class='" + SearchString.ToString().Trim() + "'"; }
                    else if (SelValueSch == 5)
                    { Search = " Mobile='" + SearchString.ToString().Trim() + "'"; }
                }

                ViewBag.Searchstring = SearchString.ToString().Trim();
                DEO.StoreAllData = OBJDB.ViewAllExaminer(Search, district);
                if (DEO.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    //ViewBag.TotalCount = 0;
                    //return View(DEO);
                }
                else
                {
                    //SLNO, CCODE, TeacherName, FatherName, Designation, TypeOfDuty, Mobile
                    @ViewBag.message = "1";
                    ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.cnt10 = DEO.StoreAllData.Tables[1].Rows[0]["cnt10"].ToString();
                    ViewBag.cnt12 = DEO.StoreAllData.Tables[1].Rows[0]["cnt12"].ToString();
                    ViewBag.Class10WiseSub = DEO.StoreAllData.Tables[2].Rows[0]["sub10"].ToString();
                    ViewBag.Class12WiseSub = DEO.StoreAllData.Tables[3].Rows[0]["sub12"].ToString();
                    ViewBag.TotalRecords = DEO.StoreAllData.Tables[1].Rows[0]["cntTotal"].ToString();
                    return View(DEO);
                }
            }


            return View(DEO);
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult UpdateExaminer(DEOModel DEO, string Exid)
        {
            string DeoUser = null;
            string district = null;
            int udid = 0;


            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }

            // id = encrypt.QueryStringModule.Decrypt(id);
            DeoUser = Session["USER"].ToString();
            district = Session["Dist"].ToString();
            udid = Convert.ToInt32(Session["UID"].ToString());

            DataSet result = OBJDB.SelectDist();
            ViewBag.MyDist = result.Tables[0];
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
            {
                items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
            }
            ViewBag.MyDist = new SelectList(items, "Value", "Text");

            DataSet Desi = OBJDB.GetSubDesi();
            ViewBag.Mydesi = Desi.Tables[0];
            List<SelectListItem> Desiitems = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.Mydesi.Rows)
            {
                Desiitems.Add(new SelectListItem { Text = @dr["SUBJECT"].ToString(), Value = @dr["SUBJECT"].ToString() });
            }
            ViewBag.Mydesi = new SelectList(Desiitems, "Value", "Text");

            @ViewBag.texp = OBJDB.GetStaffExp();
            @ViewBag.evaexp = OBJDB.GetStaffExp();

            Exid = encrypt.QueryStringModule.Decrypt(Exid);
            //--------------------------Select Show Data---------------------
            DataSet seleLastCan = OBJDB.SelectEntryExaminer(Convert.ToInt32(Exid));
            if (seleLastCan.Tables[0].Rows.Count > 0)
            {

                @ViewBag.message = "1";
                @ViewBag.Exid = seleLastCan.Tables[0].Rows[0]["Examinerid"].ToString();
                DEO.Exid = Convert.ToInt32(seleLastCan.Tables[0].Rows[0]["Examinerid"].ToString());
                DEO.Class = seleLastCan.Tables[0].Rows[0]["Class"].ToString();
                DEO.Subjects = seleLastCan.Tables[0].Rows[0]["SubjectList"].ToString();
                //DEO.lstSubjects = seleLastCan.Tables[0].Rows[0]["SubjectList"].ToString();
                string SL = seleLastCan.Tables[0].Rows[0]["SubjectList"].ToString();
                List<string> lstSubjects = new List<string>(SL.Split(','));
                DEO.StaffName = seleLastCan.Tables[0].Rows[0]["Name"].ToString();
                DEO.StaffFatherName = seleLastCan.Tables[0].Rows[0]["fname"].ToString();
                //DEO.desi = seleLastCan.Tables[0].Rows[0]["Designation"].ToString();
                DEO.Selschool = seleLastCan.Tables[0].Rows[0]["SchoolCode"].ToString();
                DEO.Selschool = seleLastCan.Tables[0].Rows[0]["SCHL"].ToString();
                DEO.schlcode = seleLastCan.Tables[0].Rows[0]["SCHL"].ToString();
                DEO.mailid = seleLastCan.Tables[0].Rows[0]["EmailID"].ToString();
                DEO.Mobile = seleLastCan.Tables[0].Rows[0]["Mobile"].ToString();
                DEO.Quali = seleLastCan.Tables[0].Rows[0]["Quali"].ToString();
                DEO.remarks = seleLastCan.Tables[0].Rows[0]["remarks"].ToString();
                DEO.homeaddress = seleLastCan.Tables[0].Rows[0]["HomeAddress"].ToString();
                DEO.homepincode = seleLastCan.Tables[0].Rows[0]["homepincode"].ToString();
                DEO.teachingexp = seleLastCan.Tables[0].Rows[0]["Teachingexp"].ToString();
                DEO.Evaluationexp = seleLastCan.Tables[0].Rows[0]["Evaexp"].ToString();
                DEO.homedist = seleLastCan.Tables[0].Rows[0]["HomeDist"].ToString();
                DEO.hometehsil = seleLastCan.Tables[0].Rows[0]["HomeTehsil"].ToString();
                DEO.DOR = seleLastCan.Tables[0].Rows[0]["DOR"].ToString();
                DEO.remarks = seleLastCan.Tables[0].Rows[0]["remarks"].ToString();

                string SchlC = seleLastCan.Tables[0].Rows[0]["SCHL"].ToString();
                if (SchlC == "*******")
                {
                    DEO.Otherschl = seleLastCan.Tables[0].Rows[0]["SchoolCode"].ToString();
                    DEO.Selschool = "Others";
                }



                AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                DataSet Tresult = objDB.SelectAllTehsil(Convert.ToInt32(DEO.homedist));
                ViewBag.MyTeh = Tresult.Tables[0];
                List<SelectListItem> TehList = new List<SelectListItem>();
                //TehList.Add(new SelectListItem { Text = "---Select Tehsil---", Value = "0" });
                foreach (System.Data.DataRow dr in ViewBag.MyTeh.Rows)
                {
                    TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });
                }
                ViewBag.MyTeh = TehList;

                DEO.cadre = seleLastCan.Tables[0].Rows[0]["Cadre"].ToString();
                //--------------------------FILL CADRE----------
                DataSet RCADRE = OBJDB.GetCADRE(); // passing Value to DeoClass from model            
                List<SelectListItem> CDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in RCADRE.Tables[0].Rows)
                {
                    CDList.Add(new SelectListItem { Text = @dr["CADRE"].ToString(), Value = @dr["CADRE"].ToString() });
                }

                ViewBag.Dcadre = CDList;
                // --------------------------------End CADRE---------------------
                DEO.desi = seleLastCan.Tables[0].Rows[0]["Designation"].ToString();

                DataSet MDESI = OBJDB.SelectAllDESIGCADREWISE(DEO.cadre);
                List<SelectListItem> DesiList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in MDESI.Tables[0].Rows)
                {
                    DesiList.Add(new SelectListItem { Text = @dr["SUBJECT"].ToString(), Value = @dr["SUBJECT"].ToString() });
                }
                ViewBag.Mydesi = DesiList;

                DataSet SubClass = OBJDB.GetLassWiseSubjects(DEO.Class); // passing Value to DeoClass from model            
                List<SelectListItem> SubList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in SubClass.Tables[0].Rows)
                {
                    SubList.Add(new SelectListItem { Text = @dr["SubENG"].ToString(), Value = @dr["SubENG"].ToString() });
                }
                ViewBag.lstsubs = SubList;
                DEO.lstSubjects = lstSubjects;
                //ViewData["lstsubs"] = SubList;

                DataSet DistwiseSCHL = OBJDB.GetDeoSchlDISTWise(district); // passing Value to DeoClass from model            
                List<SelectListItem> SDList = new List<SelectListItem>();
                SDList.Add(new SelectListItem { Text = "Others", Value = "Others" });
                foreach (System.Data.DataRow dr in DistwiseSCHL.Tables[0].Rows)
                {
                    SDList.Add(new SelectListItem { Text = @dr["schlnm"].ToString(), Value = @dr["schl"].ToString() });

                }
                ViewBag.Myschl = SDList;

            }
            else
            {
                @ViewBag.message = "Record Not Found";
            }
            //----------------------------

            return View(DEO);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult UpdateExaminer(DEOModel DEO, FormCollection frm)
        {
            string DeoUser = null;
            string district = null;
            int udid = 0;


            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }

            // id = encrypt.QueryStringModule.Decrypt(id);
            DeoUser = Session["USER"].ToString();
            district = Session["Dist"].ToString();
            udid = Convert.ToInt32(Session["UID"].ToString());

            try
            {
                int id = Convert.ToInt32(frm["hdnccode"]);

                string cls = DEO.Class;
                string SelListItem = frm["lstSubjects"];
                List<string> lstSubjects = new List<string>(SelListItem.Split(','));
                string subcode = null;
                string name = DEO.StaffName;
                string Fname = DEO.StaffFatherName;
                string desi = DEO.desi;
                string Schoolcode = DEO.Selschool;
                string emailid = DEO.mailid;
                string mobno = DEO.Mobile;
                string Quali = DEO.Quali;
                string Adrs = DEO.homeaddress;
                string hdist = DEO.homedist;
                string hteh = DEO.hometehsil;
                string pincode = DEO.homepincode;
                string texp = DEO.teachingexp;
                string eexp = DEO.Evaluationexp;
                string remark = DEO.remarks;
                string cadre = DEO.cadre;
                string DOR = DEO.DOR;

                string OtherSchool = DEO.Otherschl;
                if (Schoolcode == "Others")
                {
                    Schoolcode = OtherSchool;
                    //SCHL = "*******";
                }

                string UpdateExaminer = OBJDB.UpdateExaminerDETAILS(id, cls, SelListItem, name, Fname, desi, Schoolcode, emailid, mobno, Quali, Adrs, hdist, hteh, pincode, texp, eexp, remark, subcode, cadre, DOR);
                if (UpdateExaminer != "0")
                {
                    ViewData["result"] = "1";

                    //--------------------------FILL CADRE----------
                    DataSet RCADRE = OBJDB.GetCADRE(); // passing Value to DeoClass from model            
                    List<SelectListItem> CDList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in RCADRE.Tables[0].Rows)
                    {
                        CDList.Add(new SelectListItem { Text = @dr["CADRE"].ToString(), Value = @dr["CADRE"].ToString() });
                    }

                    ViewBag.Dcadre = CDList;
                    // --------------------------------End CADRE---------------------

                    AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                    DataSet result = OBJDB.SelectDist();
                    ViewBag.MyDist = result.Tables[0];
                    List<SelectListItem> items = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                    {
                        items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.MyDist = new SelectList(items, "Value", "Text");

                    DataSet Tresult = objDB.SelectAllTehsil(Convert.ToInt32(DEO.homedist));
                    ViewBag.MyTeh = Tresult.Tables[0];
                    List<SelectListItem> TehList = new List<SelectListItem>();
                    //TehList.Add(new SelectListItem { Text = "---Select Tehsil---", Value = "0" });
                    foreach (System.Data.DataRow dr in ViewBag.MyTeh.Rows)
                    {
                        TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });
                    }
                    ViewBag.MyTeh = TehList;


                    DataSet SubClass = OBJDB.GetLassWiseSubjects(DEO.Class); // passing Value to DeoClass from model            
                    List<SelectListItem> SubList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in SubClass.Tables[0].Rows)
                    {
                        SubList.Add(new SelectListItem { Text = @dr["SubENG"].ToString(), Value = @dr["SubENG"].ToString() });
                    }
                    ViewBag.lstsubs = SubList;
                    DEO.lstSubjects = lstSubjects;
                    //ViewData["lstsubs"] = SubList;

                    DataSet DistwiseSCHL = OBJDB.GetDeoSchlDISTWise(district); // passing Value to DeoClass from model            
                    List<SelectListItem> SDList = new List<SelectListItem>();
                    SDList.Add(new SelectListItem { Text = "Others", Value = "Others" });
                    foreach (System.Data.DataRow dr in DistwiseSCHL.Tables[0].Rows)
                    {
                        SDList.Add(new SelectListItem { Text = @dr["schlnm"].ToString(), Value = @dr["schl"].ToString() });

                    }
                    ViewBag.Myschl = SDList;

                    DataSet Desi = OBJDB.GetSubDesi();
                    ViewBag.Mydesi = Desi.Tables[0];
                    List<SelectListItem> Desiitems = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.Mydesi.Rows)
                    {
                        Desiitems.Add(new SelectListItem { Text = @dr["SUBJECT"].ToString(), Value = @dr["SUBJECT"].ToString() });
                    }
                    ViewBag.Mydesi = new SelectList(Desiitems, "Value", "Text");

                    @ViewBag.texp = OBJDB.GetStaffExp();
                    @ViewBag.evaexp = OBJDB.GetStaffExp();
                    ////--------------------------Select last Entry---------------------
                    //DataSet seleLastCan = OBJDB.SelectlastEntryExaminer();
                    //if (seleLastCan.Tables[0].Rows.Count > 0)
                    //{

                    //    @ViewBag.message = "1";
                    //    @ViewBag.Class = seleLastCan.Tables[0].Rows[0]["Class"].ToString();
                    //    @ViewBag.SubjectList = seleLastCan.Tables[0].Rows[0]["SubjectList"].ToString();
                    //    @ViewBag.Name = seleLastCan.Tables[0].Rows[0]["Name"].ToString();
                    //    @ViewBag.Fname = seleLastCan.Tables[0].Rows[0]["fname"].ToString();
                    //    @ViewBag.degi = seleLastCan.Tables[0].Rows[0]["Designation"].ToString();
                    //    @ViewBag.schoolname = seleLastCan.Tables[0].Rows[0]["SchoolCode"].ToString();
                    //    @ViewBag.email = seleLastCan.Tables[0].Rows[0]["EmailID"].ToString();
                    //    @ViewBag.mob = seleLastCan.Tables[0].Rows[0]["Mobile"].ToString();
                    //    @ViewBag.Quali = seleLastCan.Tables[0].Rows[0]["Quali"].ToString();
                    //    @ViewBag.Exp = seleLastCan.Tables[0].Rows[0]["Teachingexp"].ToString();
                    //}
                    //else
                    //{
                    //    @ViewBag.message = "Record Not Found";
                    //}
                    ////----------------------------
                }
                else
                {
                    ViewData["result"] = "-1";

                }


                //string Staffres = OBJDB.ADDSTAFFDETAILS(uid, district, CCODE, TD, fname, Name, expe, Month, gen, mob, SCHL, Schoolname, Aadharnum, Epunjabid, cadre, desi, IFSC, Accno);



            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

            return View(DEO);
        }
        public ActionResult ExaminerDELETE(string Exid)
        {

            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (Exid == null && Exid == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                else
                {
                    string result = OBJDB.DeleteExaminerData(Exid);
                    if (result == "Deleted")
                    {
                        // @ViewBag.result = "1";
                        ViewData["result"] = "1";
                        // return View(Exid);
                        return RedirectToAction("Examiner", "DeoPortal");
                    }

                }


                //id = encrypt.QueryStringModule.Decrypt(id);
                //DeoUser = Session["USER"].ToString();
                //district = Session["Dist"].ToString();
                //uid = Session["UID"].ToString();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }

            return RedirectToAction("Examiner", "DeoPortal");
        }
        public ActionResult FinalSubmitExaminer(DEOModel DEO, FormCollection frm)
        {

            string DeoUser = null;
            string district = null;
            string Catg = null;
            //if (id == null && id == null)
            //{
            //    return RedirectToAction("Index", "DeoPortal");
            //}
            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }

            // id = encrypt.QueryStringModule.Decrypt(id);
            DeoUser = Session["USER"].ToString();
            district = Session["Dist"].ToString();

            DEO.StoreAllData = OBJDB.GoForFinalSubmitExaminer(district);
            //DEO.StoreAllData = OBJDB.ViewAllExaminerDistwise(district);
            if (DEO.StoreAllData.Tables[0].Rows[0]["cntTotal"].ToString() == "0")
            {
                ViewBag.Message = "There is no record pending final submission.";
                ViewBag.TotalCount = 0;
                //ViewBag.TotalCount = 0;
                //return View(DEO);
            }
            else
            {
                @ViewBag.message = "1";
                ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                ViewBag.cnt10 = DEO.StoreAllData.Tables[0].Rows[0]["cnt10"].ToString();
                ViewBag.cnt12 = DEO.StoreAllData.Tables[0].Rows[0]["cnt12"].ToString();
                ViewBag.Class10WiseSub = DEO.StoreAllData.Tables[1].Rows[0]["sub10"].ToString();
                ViewBag.Class12WiseSub = DEO.StoreAllData.Tables[2].Rows[0]["sub12"].ToString();
                ViewBag.TotalRecords = DEO.StoreAllData.Tables[0].Rows[0]["cntTotal"].ToString();
            }
            return View(DEO);
        }

        public ActionResult ExaminerFinalSubmit(DEOModel DEO, FormCollection frm, string Dist)
        {
            string DeoUser = null;
            string district = null;
            int udid = 0;

            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");

            }
            DeoUser = Session["USER"].ToString();
            district = Dist;
            udid = Convert.ToInt32(Session["UID"].ToString());

            if (Dist == null && Dist == "0")
            {
                return RedirectToAction("Index", "DeoPortal");
            }
            else
            {
                string result = OBJDB.FinalSubmitExaminer(district, udid);
                if (result == "FinalSubmitted")
                {
                    // @ViewBag.result = "1";
                    ViewData["result"] = "1";

                }
                else
                {
                    ViewData["result"] = "0";
                }

            }
            // return View();
            return RedirectToAction("FinalSubmitExaminer", "DeoPortal");
        }
        //-----------------------------------End Examinor-----------------------------
        //--------------------------------------Forgot Password--------------------------------
        public ActionResult ForgotPassword()
        {
            ViewBag.SubmitValue = "Send";
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(DEOModel DEO)
        {
            try
            {

                ViewBag.SubmitValue = "Send";
                string DEOid = DEO.username;
                AbstractLayer.DBClass dbclass = new AbstractLayer.DBClass();

                DataSet ds = new DataSet();
                ds = OBJDB.GetEmailForgotpasswordDeoportal(DEOid);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {

                        string password = ds.Tables[0].Rows[0]["PWD"].ToString();
                        string to = ds.Tables[0].Rows[0]["EMAILID"].ToString();

                        string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Dear " + DEOid + "</b>,</td></tr><tr><td height=" + 30 + ">As per your request Dated <b>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</b> Regarding Forget Password</td></tr><tr><td><b>Your Login Details are given Below:-</b><br /><b>USER NAME:</b> " + DEOid + "<br /><b>Password :</b> " + password + "<br /></td></tr><tr><td height=" + 30 + "><b>Click Here To Login</b> <a href=https://registration2023.pseb.ac.in/DeoPortal target = _blank>https://registration2023.pseb.ac.in</a></td></tr><tr><td><b>Note:</b> Please Read Instruction Carefully Before filling the Online Form .</td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br />";

                        //string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 1 + "><tr><td><b>Dear " + SchoolNameWithCode + "</b>,</td></tr><tr><td height=" + 30 + ">As per your request Dated <b>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</b> Regarding Forget Password</td></tr><tr><td><b>Your Login Details are given Below:-</b><br /><b>School Login Id (School Code) :</b> " + sid + "<br /><b>Password :</b> " + password + "<br /></td></tr><tr><td height=" + 30 + "><b>Click Here To Login</b> <a href=https://https://registration2023.pseb.ac.in target = _blank>https://registration2023.pseb.ac.in</a></td></tr><tr><td><b>Note:</b> Pleare Read Instruction Carefully Before filling the Online Form .</td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /><tr><td><b>Contact Us</b><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 8058911911<br>DISTRICTS:- BARNALA, FATEHGARH SAHIB, GURDASPUR, HOSHIARPUR, JALANDHAR, KAPURTHALA, SHRI MUKTSAR SAHIB, S.B.S. NAGAR, PATHANKOT, PATIALA, SANGRUR, CHANDIGARH &amp; OTHER STATES<br><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 8058911911<br>DISTRICTS:- AMRITSAR, BATHINDA, FARIDKOT, FAZILKA, FEROZEPUR, LUDHIANA, MANSA, MOGA, ROOP NAGAR, S.A.S NAGAR,TARN TARAN<br></td></tr>";
                        string subject = "Deo Portal-Forgot Password Notification";
                        bool result = dbclass.mail(subject, body, to);
                        if (result == true)
                        {
                            ViewBag.SubmitValue = "Resend";
                            ViewData["result"] = "1";
                            //ViewBag.Message = "Thank You, Your Password Send To Your "+ to + " EmailId Successfully....";
                            ViewBag.Message = "Password Has Been Successfully Send to your Registered Email Id  " + to + "";
                            ModelState.Clear();
                        }
                        else
                        {
                            ViewData["result"] = "0";
                            ViewBag.Message = "Password Not Sent....";
                        }
                    }
                    else
                    {
                        ViewData["result"] = "-1";
                        ViewBag.Message = "Incorrect User Name....";
                    }
                }
                else
                {
                    ViewData["result"] = "-1";
                    ViewBag.Message = "Incorrect School Code ....";
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
            }
            return View();
        }

        //-------------------------------------End Password-------------------------------

        //----------------------Mail to Deo----------------------//
        public ActionResult MailToDeo()
        {
            ViewBag.SubmitValue = "Send";
            return View();
        }
        [HttpPost]
        public ActionResult MailToDeo(DEOModel DEO)
        {
            try
            {

                ViewBag.SubmitValue = "Send";
                string Distid = DEO.username;
                AbstractLayer.DBClass dbclass = new AbstractLayer.DBClass();

                DataSet ds = new DataSet();
                ds = OBJDB.GetEmailMailCentreNotAdded(Distid);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {

                        string name = ds.Tables[0].Rows[0]["Name"].ToString();
                        string centreCode = ds.Tables[0].Rows[0]["cent"].ToString();
                        string CenterName = ds.Tables[0].Rows[0]["ecentre"].ToString();
                        // string Remarks = ds.Tables[0].Rows[0]["Remarks"].ToString();
                        string to = ds.Tables[0].Rows[0]["EMAILID"].ToString();
                        DataTable dt = new DataTable();
                        dt = ds.Tables[1];
                        //string strtable = "<table>< tr >< th > Company </ th >< th > Contact </ th >< th > Country </ th ></ tr >< tr >< td > Alfreds Futterkiste </ td >< td > Maria Anders </ td >< td > Germany </ td ></ tr ></ table > ";

                        //string strtable = null;
                        string strCent = null;
                        string strCentName = null;
                        string Remarks = null;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            //strCent = "<td>"+strCent + ds.Tables[1].Rows[i]["cent"].ToString()+" < br />" +"</td>";
                            //strCentName = "<td>" + strCentName + ds.Tables[1].Rows[i]["ecentre"].ToString() + " < br />" + "</td>";
                            //Remarks = "<td>" + Remarks + ds.Tables[1].Rows[i]["Remarks"].ToString() + " < br />" + "</td>";
                            strCent = strCent + ds.Tables[1].Rows[i]["cent"].ToString() + " <br />" + "<hr/>";
                            strCentName = strCentName + ds.Tables[1].Rows[i]["ecentre"].ToString() + " <br />" + "<hr/>";
                            Remarks = Remarks + ds.Tables[1].Rows[i]["Remarks"].ToString() + " <br />" + "<hr/>";
                            //strtable = strtable + ds.Tables[1].Rows[i]["cent"].ToString() + " (" + ds.Tables[1].Rows[i]["ecentre"].ToString() + ") <br/>";
                        }
                        string body = "<table width = " + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td colspan = " + 2 + " ><b> Dear " + name + " ji</b>,</td></tr><tr><td height = " + 30 + " colspan=" + 2 + ">As per your request Dated<b>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</b> <br/><br/> </td></tr><tr><td colspan = " + 2 + " ><b> Your Centre Code And Centre name are given Below:-</b><br /></td></tr><table border=" + 1 + "><tr><td>Center Code</td> <td>Center Name</td> <td>Remarks</td></tr><tr><td>" + strCent + "</td><td>" + strCentName + "</td><td>" + Remarks + "</td></tr></table><tr><td height = " + 30 + " colspan= " + 2 + " ><b> Click Here To Login</b> <a href = https://registration2023.pseb.ac.in/DeoPortal target = _blank>https://registration2023.pseb.ac.in</a></td></tr><tr><td colspan=" + 2 + "><b>Note:</b> Please Read Instruction Carefully Before filling the Online Form .</td></tr><tr><td colspan=" + 2 + ">This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td colspan=" + 2 + "><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /></td></tr></table>";
                        //string body = "<table width = " + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td colspan = " + 2 + " ><b> Dear " + name + " ji</b>,</td></tr><tr><td height = " + 30 + " colspan=" + 2 + ">As per your request Dated<b>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</b> <br/><br/> </td></tr><tr><td colspan = " + 2 + " ><b> Your Centre Code And Centre name are given Below:-</b><br /></td></tr><table border=" + 1 + "><tr><td>Center Code</td> <td>Center Name</td> <td>Remarks</td></tr><tr>" + strCent + "" + strCentName + "" + Remarks + "</tr></table><tr><td height = " + 30 + " colspan= " + 2 + " ><b> Click Here To Login</b> <a href = https://registration2023.pseb.ac.in/DeoPortal target = _blank>https://registration2023.pseb.ac.in</a></td></tr><tr><td colspan=" + 2 + "><b>Note:</b> Please Read Instruction Carefully Before filling the Online Form .</td></tr><tr><td colspan=" + 2 + ">This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td colspan=" + 2 + "><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /></td></tr></table>"; 
                        // string body = "<table width = " + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td colspan = " + 2 + " ><b> Dear " + name + " ji</b>,</td></tr><tr><td height = " + 30 + " colspan=" + 2 + ">As per your request Dated<b>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</b> <br/><br/><b>" + Remarks + "</b> </td></tr><tr><td colspan = " + 2 + " ><b> Your Centre Code And Centre name are given Below:-</b><br /></td></tr><tr><td>Center Code</td> <td>Center Name</td></tr><tr><td>" + strCent + "</td><td>"+strCentName+"</td></tr><tr><td height = " + 30 + " colspan= "+2+" ><b> Click Here To Login</b> <a href = https://registration2023.pseb.ac.in/DeoPortal target = _blank>https://registration2023.pseb.ac.in</a></td></tr><tr><td colspan=" + 2 + "><b>Note:</b> Please Read Instruction Carefully Before filling the Online Form .</td></tr><tr><td colspan=" + 2 + ">This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td colspan=" + 2 + "><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /></td></tr></table>";
                        // string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Dear " + name + " ji</b>,</td></tr><tr><td height=" + 30 + ">As per your request Dated <b>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</b> <br/><br/><b>" + Remarks + "</b> </td></tr><tr><td><b>Your Centre Code And Centre name are given Below:-</b><br /><br />" + strtable + "<br /></td></tr><tr><td height=" + 30 + "><b>Click Here To Login</b> <a href=https://registration2023.pseb.ac.in/DeoPortal target = _blank>https://registration2023.pseb.ac.in</a></td></tr><tr><td><b>Note:</b> Please Read Instruction Carefully Before filling the Online Form .</td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br />";

                        //string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 1 + "><tr><td><b>Dear " + SchoolNameWithCode + "</b>,</td></tr><tr><td height=" + 30 + ">As per your request Dated <b>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</b> Regarding Forget Password</td></tr><tr><td><b>Your Login Details are given Below:-</b><br /><b>School Login Id (School Code) :</b> " + sid + "<br /><b>Password :</b> " + password + "<br /></td></tr><tr><td height=" + 30 + "><b>Click Here To Login</b> <a href=https://https://registration2023.pseb.ac.in target = _blank>https://registration2023.pseb.ac.in</a></td></tr><tr><td><b>Note:</b> Pleare Read Instruction Carefully Before filling the Online Form .</td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /><tr><td><b>Contact Us</b><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 8058911911<br>DISTRICTS:- BARNALA, FATEHGARH SAHIB, GURDASPUR, HOSHIARPUR, JALANDHAR, KAPURTHALA, SHRI MUKTSAR SAHIB, S.B.S. NAGAR, PATHANKOT, PATIALA, SANGRUR, CHANDIGARH &amp; OTHER STATES<br><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 8058911911<br>DISTRICTS:- AMRITSAR, BATHINDA, FARIDKOT, FAZILKA, FEROZEPUR, LUDHIANA, MANSA, MOGA, ROOP NAGAR, S.A.S NAGAR,TARN TARAN<br></td></tr>";
                        string subject = "Regarding Examination Centre March 2017";
                        bool result = dbclass.mail(subject, body, to);
                        if (result == true)
                        {
                            ViewBag.SubmitValue = "Resend";
                            ViewData["result"] = "1";
                            //ViewBag.Message = "Thank You, Your Password Send To Your "+ to + " EmailId Successfully....";
                            ViewBag.Message = "Centre Code And Center Name Has Been Successfully Send to your Registered Email Id  " + to + "";
                            ModelState.Clear();
                        }
                        else
                        {
                            ViewData["result"] = "0";
                            ViewBag.Message = "Password Not Sent....";
                        }
                    }
                    else
                    {
                        ViewData["result"] = "-1";
                        ViewBag.Message = "Incorrect User Name....";
                    }
                }
                else
                {
                    ViewData["result"] = "-1";
                    ViewBag.Message = "Incorrect School Code ....";
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
            }
            return View();
        }
        //------------------------ End-------------------------//

        //-----------------------Admin View Staff----------------//
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AdminViewStaff(int? page)
        {
            DEOModel DEO = new DEOModel();
            string DeoUser = null;
            string district = null;
            string Catg = null;
            string id = null;
            @ViewBag.DA = objCommon.GetDA();
            @ViewBag.exp = OBJDB.GetStaffExp();
            try
            {
                //if (id == null && id == null)
                //{
                //    return RedirectToAction("Index", "DeoPortal");
                //}
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }


                DeoUser = Session["USER"].ToString();
                DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                List<SelectListItem> DistList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {
                    DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }

                ViewBag.Dist = DistList;
                //SLNO, CCODE, TeacherName, FatherName, Designation, TypeOfDuty, Mobile
                //@ViewBag.message = "1";
                //ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Catg = DEO.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                //    if (TempData["CenterListSelDist"] == null)
                //        Search += "DIST is not Null";
                //    else
                //    {

                //        Search = Convert.ToString(TempData["CenterListSelDist"]);
                //        ViewBag.SelectedItem = TempData["CenterListSelectList"];
                //        ViewBag.Searchstring = TempData["CenterListSearchString"];

                //        // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                //}


                if (TempData["CenterListSelDist"] != null)
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];

                    TempData["CenterListSelDist"] = Search;
                    TempData["CenterListSelectList"] = ViewBag.SelectedItem;
                    TempData["CenterListSearchString"] = ViewBag.Searchstring;

                    string ExamMonth = Session["DeoLoginExamCentre"].ToString();
                    DEO.StoreAllData = OBJDB.AdminViewStaff(Search, Catg, pageIndex, ExamMonth);
                    if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Staff Dose not Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        @ViewBag.message = "1";
                        ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(DEO.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(DEO.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;

                    }
                }


            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
            return View(DEO);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult AdminViewStaff(int? page, DEOModel DEO, FormCollection frm, string cmd, string SelDist, string Category, string StaffList, string SearchString)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                //DeoUser = Session["USER"].ToString();
                //district = Session["Dist"].ToString();
                DeoUser = Session["USER"].ToString();
                DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                List<SelectListItem> DistList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {
                    DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }

                ViewBag.Dist = DistList;

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                Catg = DEO.Category;

                string Search = "";
                if (cmd == "Search")
                {

                    if (SelDist != "")
                    {
                        Search += "Dist='" + SelDist.ToString().Trim() + "'";

                    }
                    if (SelDist != "")
                    {
                        ViewBag.SelectedItem = SelDist;
                        int SelValueSch = Convert.ToInt32(Category.ToString());
                        int SelValueStaff = Convert.ToInt32(StaffList.ToString());
                        //if (SelValueSch == 1)
                        //{ Search += " and Cent='" + SearchString.ToString().Trim() + "'"; }
                        //else if (SelValueSch == 2)
                        //{ Search += " and  ecentre like '%" + SearchString.ToString().Trim() + "%'"; }
                        //else if (SelValueSch == 3)
                        if (SelValueSch == 3)
                        { Search += " and  schlnm like '%" + SearchString.ToString().Trim() + "%'"; }
                        else if (SelValueSch == 4)
                        { Search += " and CCODE='" + SearchString.ToString().Trim() + "'"; }


                    }
                    if (Catg == "" || Catg == "0")
                    {
                        // ViewBag.SelectedItem = SelDist;                        
                        int SelValueStaff = Convert.ToInt32(StaffList.ToString());

                        if (SelValueStaff == 9)
                        { Search += " and epunjabid='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueStaff == 5)
                        { Search += " and staffid='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueStaff == 6)
                        { Search += " and  name like '%" + SearchString.ToString().Trim() + "%'"; }
                        else if (SelValueStaff == 7)
                        { Search += " and mobile like '%" + SearchString.ToString().Trim() + "%'"; }
                        else if (SelValueStaff == 8)
                        { Search += " and adharno='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueStaff == 10)
                        { Search += " and Cent='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueStaff == 11)
                        { Search += " and  schlnm like '%" + SearchString.ToString().Trim() + "%'"; }
                    }


                    TempData["CenterListSelDist"] = Search;
                    TempData["CenterListSelectList"] = Category;
                    TempData["CenterListSearchString"] = SearchString.ToString().Trim();

                    ViewBag.Searchstring = SearchString.ToString().Trim();

                    string ExamMonth = Session["DeoLoginExamCentre"].ToString();
                    DEO.StoreAllData = OBJDB.AdminViewStaff(Search, Catg, pageIndex, ExamMonth);
                    if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "DATA DOESN'T EXIST";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        @ViewBag.message = "1";
                        ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(DEO.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(DEO.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;


                        return View(DEO);
                    }
                }

                return View(DEO);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AdminStaffReplacement(int? page)
        {
            return View();

            //DEOModel DEO = new DEOModel();
            //string DeoUser = null;
            //string district = null;
            //string Catg = null;
            //string id = null;
            //@ViewBag.DA = objCommon.GetDA();
            //@ViewBag.exp = OBJDB.GetStaffExp();
            //try
            //{
            //    //if (id == null && id == null)
            //    //{
            //    //    return RedirectToAction("Index", "DeoPortal");
            //    //}
            //    if (Session["USER"] == null && Session["Name"] == null)
            //    {
            //        return RedirectToAction("Index", "DeoPortal");
            //    }


            //    DeoUser = Session["USER"].ToString();
            //    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
            //    List<SelectListItem> DistList = new List<SelectListItem>();
            //    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
            //    {
            //        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
            //    }

            //    ViewBag.Dist = DistList;
            //    //SLNO, CCODE, TeacherName, FatherName, Designation, TypeOfDuty, Mobile
            //    //@ViewBag.message = "1";
            //    //ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;

            //    int pageIndex = 1;
            //    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            //    ViewBag.pagesize = pageIndex;
            //    Catg = DEO.Category;
            //    string Search = string.Empty;
            //    if (pageIndex == 1)
            //    {

            //        TempData["CenterListSelDist"] = null;
            //        TempData["CenterListSelectList"] = null;
            //        TempData["CenterListSearchString"] = null;

            //    }
            //    if (TempData["CenterListSelDist"] == null)
            //        Search += "DIST is not Null";
            //    else
            //    {

            //        Search = Convert.ToString(TempData["CenterListSelDist"]);
            //        ViewBag.SelectedItem = TempData["CenterListSelectList"];
            //        ViewBag.Searchstring = TempData["CenterListSearchString"];

            //        // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
            //    }
            //    DEO.StoreAllData = OBJDB.GetSTAFFAllDistStaffAdmin(Search, Catg, pageIndex);
            //    //DEO.TotalCount = DEO.StoreAllData.Tables[1].Rows[0]["decount"]);
            //    if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
            //    {
            //        ViewBag.Message = "Staff Dose not Exist";
            //        ViewBag.TotalCount = 0;
            //    }
            //    else
            //    {
            //        @ViewBag.message = "1";
            //        ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
            //        ViewBag.TotalCount1 = Convert.ToInt32(DEO.StoreAllData.Tables[1].Rows[0]["decount"]);
            //        int tp = Convert.ToInt32(DEO.StoreAllData.Tables[1].Rows[0]["decount"]);
            //        int pn = tp / 20;
            //        int cal = 20 * pn;
            //        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
            //        if (res >= 1)
            //            ViewBag.pn = pn + 1;
            //        else
            //            ViewBag.pn = pn;

            //    }


            //}
            //catch (Exception ex)
            //{
            //    oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
            //    return View();

            //}
            //return View(DEO);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult AdminStaffReplacement(int? page, DEOModel DEO, FormCollection frm, string cmd, string StaffList, string SearchString, string remarksid)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                //DeoUser = Session["USER"].ToString();
                //district = Session["Dist"].ToString();
                DeoUser = Session["USER"].ToString();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                Catg = DEO.Category;

                string Search = "";

                if (cmd == "Search")
                {
                    // ViewBag.SelectedItem = SelDist;                        
                    int SelValueStaff = Convert.ToInt32(StaffList.ToString());

                    if (SelValueStaff == 9)
                    { Search += " epunjabid='" + SearchString.ToString().Trim() + "'"; }
                    TempData["CenterListSearchString"] = SearchString.ToString().Trim();
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    string ExamMonth = Session["DeoLoginExamCentre"].ToString();
                    DEO.StoreAllData = OBJDB.GetSTAFFAllDistStaffAdmin(Search, Catg, pageIndex, ExamMonth);
                    if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "DATA DOESN'T EXIST";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        @ViewBag.message = "1";
                        ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                        return View(DEO);
                    }
                }

                else if (cmd == "Staff Replace" && TempData["CenterListSearchString"].ToString() == SearchString.ToString())
                {

                    string MainEpunID = SearchString.ToString();

                    string RplcEpunID = "";
                    string resultlist = "";
                    string remarks = "";
                    if (frm["AllChkId"] != null)
                    {
                        RplcEpunID = frm["AllChkId"].ToString();
                        resultlist = frm["resultlist"].ToString();
                        remarks = frm["remarks"].ToString();
                        //remarksid = RplcEpunID;
                        //string[] split1 = remarksid.Split(',');
                        //int sCount = split1.Length;
                        //if (sCount > 0)
                        //{
                        //    foreach (string s in split1)
                        //    {
                        //        string corid = s.Split('(')[0];                                
                        //    }
                        //}
                    }

                    if (RplcEpunID != "")
                    {
                        DataSet result = OBJDB.DeoStaffReplaceByAdmin(MainEpunID, RplcEpunID, DeoUser, resultlist, remarks, Session["DeoLoginExamCentre"].ToString());
                        if (result == null || result.Tables[0].Rows.Count == 0)
                        { ViewData["Result"] = null; }
                        else
                        { ViewData["Result"] = result.Tables[0].Rows[0]["res"].ToString(); }
                    }

                    int SelValueStaff = Convert.ToInt32(StaffList.ToString());
                    if (SelValueStaff == 9)
                    { Search += " epunjabid='" + SearchString.ToString().Trim() + "'"; }
                    TempData["CenterListSearchString"] = SearchString.ToString().Trim();
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    string ExamMonth = Session["DeoLoginExamCentre"].ToString();
                    DEO.StoreAllData = OBJDB.GetSTAFFAllDistStaffAdmin(Search, Catg, pageIndex, ExamMonth);
                    if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "DATA DOESN'T EXIST";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        @ViewBag.message = "1";
                        ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                        return View(DEO);
                    }
                }
                //if (cmd == "Staff Replace Report")
                //{
                //    string Seldist = "";
                //    DataSet ds1= OBJDB.DeoStaffReplaceData(Seldist, DeoUser);
                //    //DataTable dt = ds1.Tables[0];
                //    string fileName1 = "StaffReplaceList_" + DeoUser + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");
                //    using (XLWorkbook wb = new XLWorkbook())
                //    {
                //        wb.Worksheets.Add(ds1.Tables[0]);
                //        wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                //        wb.Style.Font.Bold = true;
                //        Response.Clear();
                //        Response.Buffer = true;
                //        Response.Charset = "";
                //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //        Response.AddHeader("content-disposition", "attachment;filename=" + fileName1 + ".xls");
                //        using (MemoryStream MyMemoryStream = new MemoryStream())
                //        {
                //            wb.SaveAs(MyMemoryStream);
                //            MyMemoryStream.WriteTo(Response.OutputStream);
                //            Response.Flush();
                //            Response.End();
                //        }
                //    }
                //}

                return View(DEO);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

        }
        public ActionResult AdminStaffReplaceReport()
        {
            DEOModel DEO = new DEOModel();
            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }
            string DeoUser = Session["USER"].ToString();
            DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
            List<SelectListItem> DistList = new List<SelectListItem>();
            // DistList.Add(new SelectListItem { Text = "ALL", Value = "00" });
            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
            {
                DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
            }

            ViewBag.Dist = DistList;
            return View();
        }
        [HttpPost]
        public ActionResult AdminStaffReplaceReport(FormCollection frc, string id, string cmd)
        {
            DEOModel DEO = new DEOModel();
            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }
            string DeoUser = Session["USER"].ToString();
            DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
            List<SelectListItem> DistList = new List<SelectListItem>();
            // DistList.Add(new SelectListItem { Text = "ALL", Value = "00" });
            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
            {
                DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
            }

            ViewBag.Dist = DistList;
            DEO.SelDist = frc["SelDist"].ToString();
            if (cmd == "Search")
            {

                DEO.StoreAllData = OBJDB.DeoStaffReplaceData(DEO.SelDist, DeoUser, Session["DeoLoginExamCentre"].ToString());
                @ViewBag.message = "1";
                ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                return View(DEO);
            }
            else if (cmd == "Download Staff Replace Report")
            {
                string ExamMonth = Session["DeoLoginExamCentre"].ToString();
                DEO.StoreAllData = OBJDB.DeoStaffReplaceData(DEO.SelDist, DeoUser, ExamMonth);
                DataSet ds1 = OBJDB.DeoStaffReplaceDataDownload(DEO.SelDist, DeoUser, ExamMonth);

                @ViewBag.message = "1";
                ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;


                //DataTable dt = ds1.Tables[0];
                string fileName1 = "StaffReplaceList_" + DeoUser + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");
                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(ds1.Tables[0]);
                    wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    wb.Style.Font.Bold = true;
                    Response.Clear();
                    Response.Buffer = true;
                    Response.Charset = "";
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment;filename=" + fileName1 + ".xls");
                    using (MemoryStream MyMemoryStream = new MemoryStream())
                    {
                        wb.SaveAs(MyMemoryStream);
                        MyMemoryStream.WriteTo(Response.OutputStream);
                        Response.Flush();
                        Response.End();
                    }
                }
            }
            return View(DEO);
        }
        public ActionResult NewAppointmentLetter(FormCollection frc, string id)
        {
            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }
            DEOModel DEO = new DEOModel();
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");

            string ExamMonth = Session["DeoLoginExamCentre"].ToString();
            DEO.StoreAllData = OBJDB.DeoNewAppointmentLetter(id, ExamMonth);
            ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
            return View(DEO);
        }
        //------------------End--------------------------------------//

        //--------------------------------------Centre Wise Staff List------------//
        public ActionResult SupervisoryStaffList(DEOModel DEO)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                ViewBag.DeoMonthYear = (List<DeoMonthYearModel>)Session["DeoMonthYear"];
                ViewBag.SelectedDeoMonthYear = "0";

                DeoUser = Session["USER"].ToString();
                if (DeoUser.ToUpper() == "ADMIN" || DeoUser == "admin")
                {

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
            return View(DEO);
        }
        [HttpPost]
        public ActionResult SupervisoryStaffList(DEOModel DEO, FormCollection frm)
        {

            string DeoUser = null;
            string district = null;
            string Catg = null;
            string month = "", year = "";
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                DeoUser = Session["USER"].ToString();
                ViewBag.DeoMonthYear = (List<DeoMonthYearModel>)Session["DeoMonthYear"];
                ViewBag.SelectedDeoMonthYear = "0";

                if (!string.IsNullOrEmpty(DEO.DeoMonthYear))
                {
                    ViewBag.SelectedDeoMonthYear = DEO.DeoMonthYear;
                    TempData["SelectedDeoMonthYear"] = DEO.DeoMonthYear;
                    month = DEO.DeoMonthYear.Split('-')[0];
                    year = DEO.DeoMonthYear.Split('-')[1];
                }

                if (DeoUser.ToUpper() == "ADMIN" || DeoUser == "admin")
                {

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                    district = frm["selDist"].ToString();
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                    district = frm["selDist"].ToString();
                }

                //DeoUser = Session["USER"].ToString();
                //district = Session["Dist"].ToString();

                DEO.StoreAllData = OBJDB.SupervisoryStaffList_SpByMonthYear(month, year, district, Session["DeoLoginExamCentre"].ToString());
                if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Staff Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                }
            }
            catch (Exception ex)
            {
            }
            return View(DEO);
        }
        //--------------------------End-------------------------//
        //----------------------StaffLetter-----------------------------------//
        public ActionResult StaffLetter(DEOModel DEO)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                DeoUser = Session["USER"].ToString();
                DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                List<SelectListItem> DistList = new List<SelectListItem>();
                DistList.Add(new SelectListItem { Text = "ALL", Value = "ALL" });
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {

                    DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }

                ViewBag.Dist = DistList;

            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
            return View(DEO);
        }
        [HttpPost]
        public ActionResult StaffLetter(int? page, DEOModel DEO, FormCollection frm, string cmd, string SelDist, string Category, string SearchString)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                //if (DeoUser == "ADMIN")
                //{

                //}

                DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                List<SelectListItem> DistList = new List<SelectListItem>();
                DistList.Add(new SelectListItem { Text = "ALL", Value = "ALL" });
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {
                    DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }

                ViewBag.Dist = DistList;
                //int pageIndex = 1;
                //pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                //ViewBag.pagesize = pageIndex;

                //Catg = DEO.Category;

                string Search = "";
                if (cmd == "Search")
                {

                    if (SelDist != "")
                    {
                        Search += "s.Dist='" + SelDist.ToString().Trim() + "'";

                    }
                    if (SelDist != "")
                    {
                        ViewBag.SelectedItem = SelDist;
                        int SelValueSch = Convert.ToInt32(Category.ToString());
                        if (SelValueSch == 1)
                        { Search += " and a.Cent='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueSch == 2)
                        { Search += " and a.ccode='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueSch == 3)
                        { Search += " and s.Staffid='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueSch == 4)
                        { Search += " and b.mobile='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueSch == 5)
                        { Search += " and s.adharno='" + SearchString.ToString().Trim() + "'"; }

                        //else if (SelValueSch == 3)
                        //{ Search += " and  a.schoole like '%" + SearchString.ToString().Trim() + "%'"; }
                    }
                    if (SelDist == "ALL")
                    {
                        Search = "s.Dist is not null";
                        ViewBag.SelectedItem = SelDist;
                        int SelValueSch = Convert.ToInt32(Category.ToString());
                        if (SelValueSch == 1)
                        { Search += " and a.Cent='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueSch == 2)
                        { Search += " and a.ccode='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueSch == 3)
                        { Search += " and s.Staffid='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueSch == 4)
                        { Search += " and b.mobile='" + SearchString.ToString().Trim() + "'"; }
                        else if (SelValueSch == 5)
                        { Search += " and s.adharno='" + SearchString.ToString().Trim() + "'"; }

                        //else if (SelValueSch == 3)
                        //{ Search += " and  a.schoole like '%" + SearchString.ToString().Trim() + "%'"; }
                    }


                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    DEO.StoreAllData = OBJDB.StaffLetterList(Search);

                    if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "DATA DOESN'T EXIST";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                        return View(DEO);
                    }
                }

                return View(DEO);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

        }
        //----------------End-----------------------------------------------------//

        //--------------------Capacity Letter For School Login------------------------------//
        public ActionResult CapacityLetter(DEOModel DEO)
        {
            string schl = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if ((Session["SCHL"] == null || Session["SCHL"].ToString() == "") && Session["USER"].ToString().ToUpper() != "ADMIN")
                {
                    Session.Clear();
                    return RedirectToAction("Logout", "Login");
                }

                //if(Session["USER"].ToString().ToUpper() == "ADMIN")
                //{
                //    DataSet Dresult = OBJDB.AdminGetALLSCHL(); //
                //    List<SelectListItem> DistList = new List<SelectListItem>();
                //    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                //    {
                //        DistList.Add(new SelectListItem { Text = @dr["schoole"].ToString(), Value = @dr["CSCHL"].ToString() });
                //    }
                //    ViewBag.Dist = DistList;


                //} 
                //else
                //{
                //    schl = Session["SCHL"].ToString();
                //}

                schl = Session["SCHL"].ToString();
                DEO.StoreAllData = OBJDB.CapacityLetter(schl);
                if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                    return View(DEO);
                }

            }
            catch (Exception ex)
            {
                return View();

            }
        }
        [HttpPost]
        public ActionResult CapacityLetter(DEOModel DEO, FormCollection frc)
        {
            string schl = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if ((Session["SCHL"] == null || Session["SCHL"].ToString() == "") && Session["USER"].ToString().ToUpper() != "ADMIN")
                {
                    Session.Clear();
                    return RedirectToAction("Logout", "Login");
                }

                if (Session["USER"].ToString().ToUpper() == "ADMIN")
                {
                    DataSet Dresult = OBJDB.AdminGetALLSCHL(); //
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["schoole"].ToString(), Value = @dr["CSCHL"].ToString() });
                    }
                    ViewBag.Dist = DistList;



                }
                schl = frc["SelDist"].ToString();

                DEO.StoreAllData = OBJDB.CapacityLetter(schl);
                if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                    return View(DEO);
                }

            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }
        //---------------------------End------------------------------//
        //--------------------Exam Center------------------------------//
        public ActionResult ExamCentre(DEOModel DEO)
        {
            string schl = null;
            if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
            {
                Session.Clear();
                return RedirectToAction("Logout", "Login");
            }
            schl = Session["SCHL"].ToString();
            DEO.StoreAllData = OBJDB.CentreForExam(schl);
            if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "0";
                ViewBag.TotalCount = 0;
                return View();
            }
            else
            {
                ViewBag.Message = "1";
                return View();

            }
        }
        //----------------------------End-------------------------------//
        //--------------------Noticeboard Center------------------------------//
        public ActionResult NoticeBoard(int? page)
        {
            Printlist obj = new Printlist();
            #region Circular

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.pagesize = pageIndex;

            string Search = string.Empty;
            Search = "Id like '%' and CircularTypes like '%5%' and Convert(Datetime,Convert(date,ExpiryDateDD))>=Convert(Datetime,Convert(date,getdate()))";
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



            return View(obj);
        }
        //----------------------------End-------------------------------//
        //------------------Admin Staff-------------------------------------//
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AdminStaff(int? page)
        {
            DEOModel DEO = new DEOModel();
            string DeoUser = null;
            string district = null;
            string Catg = null;
            string id = null;
            string ccode = null;

            @ViewBag.DA = objCommon.GetDA();
            @ViewBag.exp = OBJDB.GetStaffExp();
            try
            {
                //if (id == null && id == null)
                //{
                //    return RedirectToAction("Index", "DeoPortal");
                //}
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }


                DeoUser = Session["USER"].ToString();
                DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                List<SelectListItem> DistList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {
                    DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }

                ViewBag.Dist = DistList;


                DataSet clsresult = OBJDB.ShowAllCluster();
                List<SelectListItem> cresult = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in clsresult.Tables[0].Rows)
                {
                    cresult.Add(new SelectListItem { Text = @dr["clusternam"].ToString(), Value = @dr["ccode"].ToString() });
                }

                ViewBag.cls = cresult;

                ccode = DEO.ClusterCode;
                DataSet DCls = OBJDB.GetCentreSTAFFWise(ccode);
                List<SelectListItem> ClsList = new List<SelectListItem>();
                if (DCls == null)
                {
                    ClsList.Add(new SelectListItem { Text = "Select Centre", Value = "0" });
                    ViewBag.Cent = ClsList;
                }
                //if (DCls.Tables[0].Rows.Count > 0)
                //{
                //    ViewBag.TotalCount = DCls.Tables[0].Rows.Count;
                //    //--------------------------FILL Centre---------- 
                //    foreach (System.Data.DataRow dr in DCls.Tables[0].Rows)
                //    {
                //        ClsList.Add(new SelectListItem { Text = @dr["CentreName"].ToString(), Value = @dr["cent"].ToString() });
                //    }                  
                //    ViewBag.Cent = ClsList;
                //    // --------------------------------End Centre --------------------
                //}


            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
            return View(DEO);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult AdminStaff(int? page, DEOModel DEO, FormCollection frm, string cmd, string SelDist, string ClusterCode, string Category, string StaffList, string SearchString)
        {
            string district = null;
            int uid = 0;
            string fileLocation = "";
            string filename = "";
            string DeoUser = null;
            string id = null;
            string Catg = null;
            string ccode = null;
            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }

            DeoUser = Session["USER"].ToString();
            DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
            List<SelectListItem> DistList = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
            {
                DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
            }

            ViewBag.Dist = DistList;
            DataSet clsresult = OBJDB.ShowAllCluster();
            List<SelectListItem> cresult = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in clsresult.Tables[0].Rows)
            {
                cresult.Add(new SelectListItem { Text = @dr["clusternam"].ToString(), Value = @dr["ccode"].ToString() });
            }

            ViewBag.cls = cresult;
            ccode = DEO.ClusterCode;

            DataSet DCls = OBJDB.GetCentreSTAFFWise(ccode);
            List<SelectListItem> ClsList = new List<SelectListItem>();
            if (DCls == null)
            {
                ClsList.Add(new SelectListItem { Text = "Select Centre", Value = "0" });
                ViewBag.Cent = ClsList;
            }
            if (DCls != null)
            {
                if (DCls.Tables[0].Rows.Count > 0)
                {
                    ViewBag.TotalCount = DCls.Tables[0].Rows.Count;
                    //--------------------------FILL centre----------  
                    foreach (System.Data.DataRow dr in DCls.Tables[0].Rows)
                    {
                        ClsList.Add(new SelectListItem { Text = @dr["CentreName"].ToString(), Value = @dr["cent"].ToString() });
                    }
                    ViewBag.Cent = ClsList;
                    // --------------------------------End centre---------------------
                }
            }

            //--------------------------------------------------------- Paging Start--------------
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.pagesize = pageIndex;
            Catg = DEO.Category;
            // ccode = DEO.ClusterCode;
            string Choose = StaffList.ToString();
            string Search = "";

            if (cmd == "Search")
            {

                if (SelDist != "" && (ccode != "" || ccode != "0"))
                {
                    if (Choose == "ALL")
                    {
                        Search += "Dist='" + SelDist.ToString().Trim() + "'";
                        Search += " and CCODE='" + ccode.ToString().Trim() + "'";
                    }
                    else
                    {
                        Search += "Dist='" + SelDist.ToString().Trim() + "'";
                        Search += " and CCODE='" + ccode.ToString().Trim() + "'";
                        Search += " and (Cent is null Or cent='')";
                    }

                }
                //if (SelDist != "")
                //{
                //    ViewBag.SelectedItem = SelDist;
                //    int SelValueSch = Convert.ToInt32(Category.ToString());
                //    int SelValueStaff = Convert.ToInt32(StaffList.ToString());
                //    //if (SelValueSch == 1)
                //    //{ Search += " and Cent='" + SearchString.ToString().Trim() + "'"; }
                //    //else if (SelValueSch == 2)
                //    //{ Search += " and  ecentre like '%" + SearchString.ToString().Trim() + "%'"; }
                //    //else if (SelValueSch == 3)
                //    if (SelValueSch == 3)
                //    { Search += " and  schlnm like '%" + SearchString.ToString().Trim() + "%'"; }
                //    else if (SelValueSch == 4)
                //    { Search += " and CCODE='" + SearchString.ToString().Trim() + "'"; }


                //}
                //if (Catg == "" || Catg == "0")
                //{
                //    // ViewBag.SelectedItem = SelDist;                        
                //    int SelValueStaff = Convert.ToInt32(StaffList.ToString());

                //    if (SelValueStaff == 5)
                //    { Search += " and staffid='" + SearchString.ToString().Trim() + "'"; }
                //    else if (SelValueStaff == 6)
                //    { Search += " and  name like '%" + SearchString.ToString().Trim() + "%'"; }
                //    else if (SelValueStaff == 7)
                //    { Search += " and mobile like '%" + SearchString.ToString().Trim() + "%'"; }
                //    else if (SelValueStaff == 8)
                //    { Search += " and adharno='" + SearchString.ToString().Trim() + "'"; }
                //}


                TempData["CenterListSelDist"] = Search;
                TempData["CenterListSelectList"] = ccode;
                //TempData["CenterListSearchString"] = SearchString.ToString().Trim();

                //ViewBag.Searchstring = SearchString.ToString().Trim();

                DEO.StoreAllData = OBJDB.GetAdminSTAFFDetails(Search, Catg, pageIndex);
                if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    @ViewBag.message = "1";
                    ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(DEO.StoreAllData.Tables[1].Rows[0]["decount"]);
                    int tp = Convert.ToInt32(DEO.StoreAllData.Tables[1].Rows[0]["decount"]);
                    int pn = tp / 20;
                    int cal = 20 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;


                    return View(DEO);
                }
            }
            //-----------------------------------------------------------------------------End------------

            //------------------------------------------------------------Allot To Center Start--------------------
            if (cmd == "Submit")
            {
                district = Session["Dist"].ToString();
                //uid = Session["UID"].ToString();
                string cencode = frm["CentreCode"];
                string StaffChk = frm["StaffName"];
                uid = Convert.ToInt32(Session["UID"].ToString());
                //string OldClsid = frm["hdnccode"];
                // DEO.ClusterCode = OldClsid;
                string updcentdate = frm["centreDate"];
                string StaffClusterresult = OBJDB.Update_Centre_To_StaffShift(cencode, uid, StaffChk, updcentdate);
                if (StaffClusterresult == "0")
                {
                    //--------------Not Updated
                    ViewData["result"] = 0;

                }
                else
                {
                    //-------------- Updated----------
                    ViewData["result"] = 1;


                }
            }


            return View(DEO);
            //return RedirectToAction("ADMINSTAFF", "DeoPortal");
        }

        public JsonResult GETDistWiseCluster(string CAD) // Calling on http post (on Submit)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            DataSet result = OBJDB.SelectAllClusterDistWISE(CAD);
            ViewBag.cls = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> desiList = new List<SelectListItem>();
            //List<string> items = new List<string>();
            desiList.Add(new SelectListItem { Text = "---Select Cluster---", Value = "0" });
            foreach (System.Data.DataRow dr in ViewBag.cls.Rows)
            {

                desiList.Add(new SelectListItem { Text = @dr["clusternam"].ToString(), Value = @dr["ccode"].ToString() });

            }
            ViewBag.cls = desiList;
            return Json(desiList);

        }
        public JsonResult GETClusterWiseCentre(string cls) // Calling on http post (on Submit)
        {
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            DataSet result = OBJDB.GetCentreSTAFFWise(cls);
            ViewBag.Cent = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> desiList = new List<SelectListItem>();
            //List<string> items = new List<string>();
            desiList.Add(new SelectListItem { Text = "---Select centre---", Value = "0" });
            foreach (System.Data.DataRow dr in ViewBag.Cent.Rows)
            {
                desiList.Add(new SelectListItem { Text = @dr["CentreName"].ToString(), Value = @dr["cent"].ToString() });
            }
            ViewBag.Cent = desiList;
            return Json(desiList);
        }
        //----------------------End-----------------------------------------//

        //-----------------------Replacemnet Staff---------------------------//
        public ActionResult ReplacementStaff(int? page, DEOModel DEO, FormCollection frm, string SearchString)
        {

            int udid = 0;
            udid = Convert.ToInt32(Session["UID"].ToString());
            string DeoUser = null;
            string district = null;
            string Catg = null;
            //if (id == null && id == null)
            //{
            //    return RedirectToAction("Index", "DeoPortal");
            //}
            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }

            // id = encrypt.QueryStringModule.Decrypt(id);
            DeoUser = Session["USER"].ToString();
            district = Session["Dist"].ToString();
            if (SearchString == null || SearchString == "")
            {

                List<SelectListItem> StaffList = new List<SelectListItem>();
                StaffList.Add(new SelectListItem { Text = "---Select Staff---", Value = "0" });
                ViewBag.Staff = StaffList;
            }
            else
            {
                @ViewBag.Searchstring = SearchString;
                DEO.StoreAllData = OBJDB.ViewReplacementStaff(SearchString.Trim(), district, udid, Session["DeoLoginExamCentre"].ToString());
                if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "STAFF ID DOESN'T EXIST";
                    ViewBag.TotalCount = 0;
                    //ViewBag.TotalCount = 0;
                    //return View(DEO);
                    List<SelectListItem> StaffList = new List<SelectListItem>();
                    StaffList.Add(new SelectListItem { Text = "---Select Staff---", Value = "0" });
                    ViewBag.Staff = StaffList;
                }
                else
                {
                    //SLNO, CCODE, TeacherName, FatherName, Designation, TypeOfDuty, Mobile
                    //@ViewBag.message = "1";
                    ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.ccode = DEO.StoreAllData.Tables[0].Rows[0]["ccode"].ToString();
                    ViewBag.dutytype = DEO.StoreAllData.Tables[0].Rows[0]["dutytype"].ToString();
                    ViewBag.ClusterDatails = DEO.StoreAllData.Tables[0].Rows[0]["ClusterDatails"].ToString();
                    ViewBag.centreDetails = DEO.StoreAllData.Tables[0].Rows[0]["centreDetails"].ToString();
                    ViewBag.OldStaffDetails = DEO.StoreAllData.Tables[0].Rows[0]["OldStaffDetails"].ToString();


                    ViewBag.Staff = DEO.StoreAllData.Tables[1];// ViewData["result"] = result; // for dislaying message after saving storing output.
                    List<SelectListItem> StaffList = new List<SelectListItem>();
                    //StaffList.Add(new SelectListItem { Text = "---Select Staff---", Value = "0" });
                    foreach (System.Data.DataRow dr in ViewBag.Staff.Rows)
                    {
                        StaffList.Add(new SelectListItem { Text = @dr["NewStaffDetails"].ToString(), Value = @dr["Staffid"].ToString() });
                    }
                    ViewBag.Staff = StaffList;
                }
            }
            //---------------------------- Show Grid Data-----------------------//
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.pagesize = pageIndex;

            DEO.StoreAllStaffData = OBJDB.ViewReplacementALLShiftStaff(district, pageIndex, udid, Session["DeoLoginExamCentre"].ToString());
            if (DEO.StoreAllStaffData == null || DEO.StoreAllStaffData.Tables[0].Rows.Count == 0)
            {
                // ViewBag.Message = "Staff Dose not Exist";
                ViewBag.TotalCount = 0;
                List<SelectListItem> StaffList = new List<SelectListItem>();
                StaffList.Add(new SelectListItem { Text = "---Select Staff---", Value = "0" });
                ViewBag.Staff = StaffList;
            }
            else
            {
                @ViewBag.message = "1";
                ViewBag.TotalCount = DEO.StoreAllStaffData.Tables[0].Rows.Count;
                ViewBag.TotalCount1 = Convert.ToInt32(DEO.StoreAllStaffData.Tables[1].Rows[0]["decount"]);
                int tp = Convert.ToInt32(DEO.StoreAllStaffData.Tables[1].Rows[0]["decount"]);
                int pn = tp / 20;
                int cal = 20 * pn;
                int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                if (res >= 1)
                    ViewBag.pn = pn + 1;
                else
                    ViewBag.pn = pn;

                List<SelectListItem> StaffList = new List<SelectListItem>();
                StaffList.Add(new SelectListItem { Text = "---Select Staff---", Value = "0" });
                ViewBag.Staff = StaffList;
            }
            //----------------------------------End--------------------------------//

            return View(DEO);
        }

        [HttpPost]
        public ActionResult ReplacementStaff(int? page, DEOModel DEO, FormCollection frm, string SearchString, string cmd, string Reason)
        {
            int udid = 0;
            udid = Convert.ToInt32(Session["UID"].ToString());
            string DeoUser = null;
            string district = null;
            string Catg = null;
            //if (id == null && id == null)
            //{
            //    return RedirectToAction("Index", "DeoPortal");
            //}
            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }

            // id = encrypt.QueryStringModule.Decrypt(id);
            DeoUser = Session["USER"].ToString();
            district = Session["Dist"].ToString();
            if (cmd == "Search")
            {



                if (SearchString == null || SearchString == "")
                {
                    List<SelectListItem> StaffList = new List<SelectListItem>();
                    StaffList.Add(new SelectListItem { Text = "---Select Staff---", Value = "0" });
                    ViewBag.Staff = StaffList;
                    ViewBag.TotalCount = 0;
                    ViewBag.Message = "Please Enter Staff ID";
                }
                else
                {
                    @ViewBag.Searchstring = SearchString;
                    DEO.StoreAllData = OBJDB.ViewReplacementStaff(SearchString.Trim(), district, udid, Session["DeoLoginExamCentre"].ToString());
                    if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "STAFF ID DOESN'T EXIST";
                        ViewBag.TotalCount = 0;
                        //ViewBag.TotalCount = 0;
                        //return View(DEO);
                        List<SelectListItem> StaffList = new List<SelectListItem>();
                        StaffList.Add(new SelectListItem { Text = "---Select Staff---", Value = "0" });
                        ViewBag.Staff = StaffList;
                    }
                    else
                    {
                        //SLNO, CCODE, TeacherName, FatherName, Designation, TypeOfDuty, Mobile
                        //@ViewBag.message = "1";
                        ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.ccode = DEO.StoreAllData.Tables[0].Rows[0]["ccode"].ToString();
                        ViewBag.dutytype = DEO.StoreAllData.Tables[0].Rows[0]["dutytype"].ToString();
                        ViewBag.ClusterDatails = DEO.StoreAllData.Tables[0].Rows[0]["ClusterDatails"].ToString();
                        ViewBag.centreDetails = DEO.StoreAllData.Tables[0].Rows[0]["centreDetails"].ToString();
                        ViewBag.OldStaffDetails = DEO.StoreAllData.Tables[0].Rows[0]["OldStaffDetails"].ToString();


                        ViewBag.Staff = DEO.StoreAllData.Tables[1];// ViewData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> StaffList = new List<SelectListItem>();
                        //StaffList.Add(new SelectListItem { Text = "---Select Staff---", Value = "0" });
                        foreach (System.Data.DataRow dr in ViewBag.Staff.Rows)
                        {
                            StaffList.Add(new SelectListItem { Text = @dr["NewStaffDetails"].ToString(), Value = @dr["Staffid"].ToString() });
                        }
                        ViewBag.Staff = StaffList;

                        //---------------------------- Show Grid Data-----------------------//
                        int pageIndex = 1;
                        pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                        ViewBag.pagesize = pageIndex;

                        DEO.StoreAllStaffData = OBJDB.ViewReplacementALLShiftStaff(district, pageIndex, udid, Session["DeoLoginExamCentre"].ToString());
                        if (DEO.StoreAllStaffData == null || DEO.StoreAllStaffData.Tables[0].Rows.Count == 0)
                        {
                            //ViewBag.Message = "No Staff For Grid";
                            ViewBag.TotalCount = 0;
                            //List<SelectListItem> StaffList = new List<SelectListItem>();
                            //StaffList.Add(new SelectListItem { Text = "---Select Staff---", Value = "0" });
                            //ViewBag.Staff = StaffList;
                        }
                        else
                        {
                            @ViewBag.message = "1";
                            ViewBag.TotalCount = DEO.StoreAllStaffData.Tables[0].Rows.Count;
                            ViewBag.TotalCount1 = Convert.ToInt32(DEO.StoreAllStaffData.Tables[1].Rows[0]["decount"]);
                            int tp = Convert.ToInt32(DEO.StoreAllStaffData.Tables[1].Rows[0]["decount"]);
                            int pn = tp / 20;
                            int cal = 20 * pn;
                            int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                            if (res >= 1)
                                ViewBag.pn = pn + 1;
                            else
                                ViewBag.pn = pn;

                            //List<SelectListItem> StaffList = new List<SelectListItem>();
                            //StaffList.Add(new SelectListItem { Text = "---Select Staff---", Value = "0" });
                            //ViewBag.Staff = StaffList;
                        }
                        //----------------------------------End--------------------------------//
                    }
                }
            }
            if (cmd == "Submit")
            {
                try
                {

                    string oldstffid = DEO.SearchString;
                    string newStaffid = DEO.StaffName;
                    string cendate = DEO.centreDate;
                    //int udid = Convert.ToInt32(Session["UID"].ToString());
                    string reason = Reason;
                    string ChangeShiftStaff = OBJDB.Ins_Shift_Staff(oldstffid, district, newStaffid, cendate, udid, Reason);
                    if (ChangeShiftStaff != "0")
                    {
                        ViewData["result"] = "1";
                    }
                    else
                    {
                        ViewData["result"] = "-1";
                    }
                    //---------------------------- Show Grid Data-----------------------//
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;

                    DEO.StoreAllStaffData = OBJDB.ViewReplacementALLShiftStaff(district, pageIndex, udid, Session["DeoLoginExamCentre"].ToString());
                    if (DEO.StoreAllStaffData == null || DEO.StoreAllStaffData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Staff Dose not Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        @ViewBag.message = "1";
                        ViewBag.TotalCount = DEO.StoreAllStaffData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(DEO.StoreAllStaffData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(DEO.StoreAllStaffData.Tables[1].Rows[0]["decount"]);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;


                    }
                    //----------------------------------End--------------------------------//
                    List<SelectListItem> StaffList = new List<SelectListItem>();
                    StaffList.Add(new SelectListItem { Text = "---Select Staff---", Value = "0" });
                    ViewBag.Staff = StaffList;

                }
                catch (Exception ex)
                {
                    oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                    return View();

                    List<SelectListItem> StaffList = new List<SelectListItem>();
                    StaffList.Add(new SelectListItem { Text = "---Select Staff---", Value = "0" });
                    ViewBag.Staff = StaffList;

                }
            }


            return View(DEO);
        }
        //----------------------------------------------End---------------------------//
        //----------------------------------------------Deoletter---------------------------//
        public ActionResult DeoLetter(DEOModel DEO, string shiftid)
        {
            int udid = 0;
            udid = Convert.ToInt32(Session["UID"].ToString());
            ViewBag.Deouid = udid;
            string DeoUser = null;
            string district = null;

            if (Session["USER"] == null && Session["Name"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }

            // id = encrypt.QueryStringModule.Decrypt(id);
            DeoUser = Session["USER"].ToString();
            district = Session["Dist"].ToString();

            DEO.StoreAllData = OBJDB.DeoLetter(district, shiftid, Session["DeoLoginExamCentre"].ToString());

            if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "STAFF ID DOESN'T EXIST";
                ViewBag.TotalCount = 0;

            }
            else
            {
                //SLNO, CCODE, TeacherName, FatherName, Designation, TypeOfDuty, Mobile
                //@ViewBag.message = "1";
                //Select dist,ostaffid,dcode,oname,oschlnm,omobile,ccode,cent from ShiftStaff where dist='010'
                //select ndist, nstaffid, ndcode, nname, nschlnm, nmobile, noccode, nocent, ecentre, schoole, distnm, centredate from ShiftStaff where dist = '010'
                ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                ViewBag.dist = DEO.StoreAllData.Tables[0].Rows[0]["dist"].ToString();
                ViewBag.ostaffid = DEO.StoreAllData.Tables[0].Rows[0]["ostaffid"].ToString();
                string dcode = DEO.StoreAllData.Tables[0].Rows[0]["dcode"].ToString();
                if (dcode == "1")
                {
                    ViewBag.dcode = "Superintendent";
                }
                if (dcode == "2")
                {
                    ViewBag.dcode = "Deputy Superintendent";
                }
                if (dcode == "3")
                {
                    ViewBag.dcode = "Invigilator";
                }
                ViewBag.id = DEO.StoreAllData.Tables[0].Rows[0]["id"].ToString();
                ViewBag.oname = DEO.StoreAllData.Tables[0].Rows[0]["oname"].ToString();
                ViewBag.ofname = DEO.StoreAllData.Tables[0].Rows[0]["Ofname"].ToString();
                ViewBag.ocadre = DEO.StoreAllData.Tables[0].Rows[0]["OCadreDesi"].ToString();
                ViewBag.oschlnm = DEO.StoreAllData.Tables[0].Rows[0]["oschlnm"].ToString();
                ViewBag.omobile = DEO.StoreAllData.Tables[0].Rows[0]["omobile"].ToString();
                ViewBag.ccode = DEO.StoreAllData.Tables[0].Rows[0]["ccode"].ToString();
                ViewBag.cent = DEO.StoreAllData.Tables[0].Rows[0]["cent"].ToString();

                ViewBag.nstaffid = DEO.StoreAllData.Tables[0].Rows[0]["nstaffid"].ToString();
                ViewBag.nfname = DEO.StoreAllData.Tables[0].Rows[0]["nfname"].ToString();
                ViewBag.nname = DEO.StoreAllData.Tables[0].Rows[0]["nname"].ToString();
                ViewBag.Ncadre = DEO.StoreAllData.Tables[0].Rows[0]["NCadreDesi"].ToString();
                ViewBag.nschlnm = DEO.StoreAllData.Tables[0].Rows[0]["nschlnm"].ToString();
                ViewBag.nmobile = DEO.StoreAllData.Tables[0].Rows[0]["nmobile"].ToString();

                ViewBag.ecentre = DEO.StoreAllData.Tables[0].Rows[0]["ecentre"].ToString();
                ViewBag.schoole = DEO.StoreAllData.Tables[0].Rows[0]["schoole"].ToString();
                ViewBag.distnm = DEO.StoreAllData.Tables[0].Rows[0]["distnm"].ToString();
                ViewBag.clustername = DEO.StoreAllData.Tables[0].Rows[0]["clustername"].ToString();
                ViewBag.PrintDate = DEO.StoreAllData.Tables[0].Rows[0]["PrintDate"].ToString();
                ViewBag.centredate = DEO.StoreAllData.Tables[0].Rows[0]["centredate"].ToString();
            }
            return View(DEO);
        }
        //----------------------------------------------End---------------------------//


        public ActionResult ClusterWiseExtraStaffListPrint(int? page, string SelLot)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser.ToUpper() == "ADMIN")
                {
                    //string[] ArrInvoice = { "010","020","025","030","040","045","050","060","070","080" };
                    //string[] ArrInvoice = { "Invoice # 120", "Invoice # 121", "Invoice # 122", "Invoice # 123" };
                    //List<int> lstInvice = new List<int>();
                    //List<SelectListItem> DList = new List<SelectListItem>();
                    //foreach (string item in ArrInvoice)
                    //{
                    //    //int intInvocie = 0;
                    //    //Int32.TryParse(item.Split(',')[0], out intInvocie);
                    //    //if (intInvocie > 0)
                    //    //{
                    //    //    //lstInvice.Add(intInvocie);
                    //    //    DList.Add(new SelectListItem { Text = "yiy", Value = Convert.ToString(intInvocie)});
                    //    //}


                    //}
                    //string DC = "010,020,025,030,040,045,050,060,070,080";
                    //// Split string on spaces.
                    //// ... This will separate all the words.
                    //string[] DistCodes = DC.Split(',');
                    //foreach (string DistCode in DistCodes)
                    //{
                    //    DList.Add(new SelectListItem { Text = "yiy", Value = DistCode });
                    //}

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    //Search = "a.DIST = '" + district + "'";
                    Search = "b.DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }
                obj.StoreAllData = OBJDB.SelectClusterWiseExtraStaffListReport(Search, Catg, pageIndex);
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Center Dose not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    //ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    //int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    //int pn = tp / 10;
                    //int cal = 10 * pn;
                    //int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    //if (res >= 1)
                    //    ViewBag.pn = pn + 1;
                    //else
                    //    ViewBag.pn = pn;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }


        //----------------------Send Sms-----------------------//
        public ActionResult sendsms()
        {
            string Mobile = "0";
            DataSet dtResult = OBJDB.sendSmsToschl();// OutStatus mobile
            if (dtResult != null || dtResult.Tables[0].Rows.Count != 0)
            {
                // for (int i = 0; i < dtResult.Tables[0].Rows.Count; i++)
                //{
                Mobile = "9718803833";
                //  Mobile = dtResult.Tables[0].Rows[0]["Mobile"].ToString();                    
                string Sms = "Signature chart and Confidential List are uploaded in your School Login.  You can print these lists From School Login under Exam Centre if required..";
                try
                {
                    if (Mobile != "0" || Mobile != "")
                    {
                        string getSms = objCommon.gosms(Mobile, Sms);
                        // string getSms = objCommon.gosms("9711819184", Sms);
                    }

                }
                catch (Exception) { }
                // }

                @ViewBag.msg = "message Send SuccessFully";

            }
            else
            {

            }
            return View();
        }

        //---------------------End------------------------------//

        //------------------Admin Staff-------------------------------------//
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult EntryAdminStaff(int? page)
        {
            DEOModel DEO = new DEOModel();
            string DeoUser = null;
            string district = null;
            string Catg = null;
            string uid = null;
            string ccode = null;

            @ViewBag.DA = objCommon.GetDA();
            @ViewBag.exp = OBJDB.GetStaffExp();
            try
            {
                //if (id == null && id == null)
                //{
                //    return RedirectToAction("Index", "DeoPortal");
                //}
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                district = Session["Dist"].ToString();
                DeoUser = Session["USER"].ToString();
                uid = Session["UID"].ToString();
                DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                List<SelectListItem> DistList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {
                    DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }

                ViewBag.Dist = DistList;


                DataSet clsresult = OBJDB.ShowAllCluster();
                List<SelectListItem> cresult = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in clsresult.Tables[0].Rows)
                {
                    cresult.Add(new SelectListItem { Text = @dr["clusternam"].ToString(), Value = @dr["ccode"].ToString() });
                }

                ViewBag.cls = cresult;

                //ccode = DEO.ClusterCode;
                //DataSet DCls = OBJDB.GetCentreSTAFFWise(ccode);
                //List<SelectListItem> ClsList = new List<SelectListItem>();
                //if (DCls == null)
                //{
                //    ClsList.Add(new SelectListItem { Text = "Select Centre", Value = "0" });
                //    ViewBag.Cent = ClsList;
                //}

                DataSet DistwiseSCHL = OBJDB.GetDeoSchlDISTWise(district); // passing Value to DeoClass from model            
                List<SelectListItem> SDList = new List<SelectListItem>();
                SDList.Add(new SelectListItem { Text = "Others", Value = "Others" });
                foreach (System.Data.DataRow dr in DistwiseSCHL.Tables[0].Rows)
                {
                    SDList.Add(new SelectListItem { Text = @dr["schlnm"].ToString(), Value = @dr["schl"].ToString() });

                }

                ViewBag.schl = SDList;
                //--------------------------FILL CADRE----------
                DataSet RCADRE = OBJDB.GetCADRE(); // passing Value to DeoClass from model            
                List<SelectListItem> CDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in RCADRE.Tables[0].Rows)
                {
                    CDList.Add(new SelectListItem { Text = @dr["CADRE"].ToString(), Value = @dr["CADRE"].ToString() });
                }

                ViewBag.Dcadre = CDList;
                // --------------------------------End CADRE---------------------

            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
            DataSet Staffresult = OBJDB.GetAdminSTAFFUserWise(uid);
            if (Staffresult.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "DATA DOESN'T EXIST";
                //ViewBag.TotalCount = 0;
                //return View(DEO);
            }
            else
            {
                //SLNO, CCODE, TeacherName, FatherName, Designation, TypeOfDuty, Mobile
                @ViewBag.message = "1";
                ViewBag.SLNO = Staffresult.Tables[0].Rows[0][0].ToString();
                ViewBag.CCODE = Staffresult.Tables[0].Rows[0][1].ToString();
                ViewBag.TeacherName = Staffresult.Tables[0].Rows[0][2].ToString();
                ViewBag.FatherName = Staffresult.Tables[0].Rows[0][3].ToString();
                ViewBag.Designation = Staffresult.Tables[0].Rows[0][4].ToString();
                ViewBag.TypeOfDuty = Staffresult.Tables[0].Rows[0][5].ToString();
                ViewBag.Mobile = Staffresult.Tables[0].Rows[0][6].ToString();
                //ViewBag.totcnt = Staffresult.Tables[0].Rows.Count;
            }
            return View(DEO);
        }
        [HttpPost]
        public ActionResult EntryAdminStaff(DEOModel DEO, string CCODEID, FormCollection frm)
        {
            string DeoUser = null;
            string district = null;
            string uid = null;
            string Catg = null;
            @ViewBag.DA = objCommon.GetDA();
            ViewBag.exp = OBJDB.GetStaffExp();
            try
            {
                //if (CCODEID == null && CCODEID == null)
                //{
                //   return RedirectToAction("Index", "DeoPortal");
                //}
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                string CCID = frm["hdnccode"];
                // CCODEID = encrypt.QueryStringModule.Decrypt(CCODEID);
                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                uid = Session["UID"].ToString();


                DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                List<SelectListItem> DistList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {
                    DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }

                ViewBag.Dist = DistList;


                DataSet clsresult = OBJDB.ShowAllCluster();
                List<SelectListItem> cresult = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in clsresult.Tables[0].Rows)
                {
                    cresult.Add(new SelectListItem { Text = @dr["clusternam"].ToString(), Value = @dr["ccode"].ToString() });
                }

                ViewBag.cls = cresult;

                DataSet DistwiseSCHL = OBJDB.GetDeoSchlDISTWise(district); // passing Value to DeoClass from model            
                List<SelectListItem> SDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in DistwiseSCHL.Tables[0].Rows)
                {
                    SDList.Add(new SelectListItem { Text = @dr["schlnm"].ToString(), Value = @dr["schl"].ToString() });
                }

                ViewBag.schl = SDList;
                //--------------------------FILL CADRE----------
                DataSet RCADRE = OBJDB.GetCADRE(); // passing Value to DeoClass from model            
                List<SelectListItem> CDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in RCADRE.Tables[0].Rows)
                {
                    CDList.Add(new SelectListItem { Text = @dr["CADRE"].ToString(), Value = @dr["CADRE"].ToString() });
                }

                ViewBag.Dcadre = CDList;



                //string CENT = ViewBag.CCENT;
                //string CCODE = ViewBag.ClusterCode;
                string Admindistrict = DEO.SelDist;
                string AdminCCODE = DEO.ClusterCode;

                string TD = DEO.typeDuty;
                string fname = DEO.StaffFatherName;
                string Name = DEO.StaffName;
                string expe = DEO.experience;
                string Month = DEO.expmonth;
                string gen = DEO.gender;
                string mob = DEO.Mobile;
                string SCHL = DEO.schlcode;
                string Schoolname = DEO.Selschool;
                string Aadharnum = DEO.adharno;
                string Epunjabid = DEO.teacherepunjabid;
                string cadre = DEO.cadre;

                string desi = frm["desi"];
                string IFSC = DEO.ifsccode;
                string Accno = DEO.bankaccno;
                string OtherSchool = DEO.Otherschl;
                if (Schoolname == "Others")
                {
                    Schoolname = OtherSchool;
                    SCHL = "*******";
                }

                string phy = DEO.physicallydisablity;
                string DOB = DEO.DOB;
                string SelDist = DEO.SelDist;
                string homeaddress = DEO.homeaddress;
                string homedist = DEO.homedist;
                string bankname = DEO.bankname;
                //string selected12 = frm["Selschool"];
                //string dor = DEO.DOR;
                //string remarks = DEO.remarks;

                //string Staffres = OBJDB.ADDSTAFFDETAILS(uid, district,CCODE, TD, fname, Name, expe, Month, gen, mob, SCHL, Schoolname, Aadharnum, Epunjabid, cadre, desi, IFSC, Accno,dor,remarks);
                string AdminStaffres = OBJDB.ADDSTAFFDETAILS(DeoUser, uid, Admindistrict, AdminCCODE, TD, fname, Name, expe, Month, gen, mob, SCHL, Schoolname, Aadharnum, Epunjabid, cadre, desi, IFSC, Accno,
                     phy, DOB, SelDist, homeaddress, homedist, bankname);
                if (AdminStaffres == "1")
                {
                    ViewData["result"] = "1";

                }
                else if (AdminStaffres == "-1")
                {
                    ViewData["result"] = "-1";
                }
                else if (AdminStaffres == "0")
                {
                    ViewData["result"] = "0";
                }


                DataSet Staffresult = OBJDB.GetAdminSTAFFUserWise(uid);
                if (Staffresult.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    //ViewBag.TotalCount = 0;
                    //return View(DEO);
                }
                else
                {
                    //SLNO, CCODE, TeacherName, FatherName, Designation, TypeOfDuty, Mobile
                    @ViewBag.message = "1";
                    ViewBag.SLNO = Staffresult.Tables[0].Rows[0][0].ToString();
                    ViewBag.CCODE = Staffresult.Tables[0].Rows[0][1].ToString();
                    ViewBag.TeacherName = Staffresult.Tables[0].Rows[0][2].ToString();
                    ViewBag.FatherName = Staffresult.Tables[0].Rows[0][3].ToString();
                    ViewBag.Designation = Staffresult.Tables[0].Rows[0][4].ToString();
                    ViewBag.TypeOfDuty = Staffresult.Tables[0].Rows[0][5].ToString();
                    ViewBag.Mobile = Staffresult.Tables[0].Rows[0][6].ToString();
                    //ViewBag.totcnt = Staffresult.Tables[0].Rows.Count;
                }
            }

            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
            return View(DEO);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult VIEWADMINSTAFF(DEOModel DEO, FormCollection frm)
        {

            string DeoUser = null;
            string district = null;
            string uid = null;
            try
            {

                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                uid = Session["UID"].ToString();
                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();

                DEO.StoreAllData = OBJDB.GetAdminSTAFFUserWise(uid);
                if (DEO.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    //ViewBag.TotalCount = 0;
                    //return View(DEO);
                }
                else
                {
                    //SLNO, CCODE, TeacherName, FatherName, Designation, TypeOfDuty, Mobile
                    @ViewBag.message = "1";
                    ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;

                }



            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
            return View(DEO);
        }

        //-----------------------------------End------------------------------------------------------

        //-----------------------------------Start------------------------------------------------------

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ImportStaff(DEOModel DEO, string CCODEID, int? page)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            string id = null;
            id = CCODEID;

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.pagesize = pageIndex;
            string sstring = "";


            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (CCODEID != null && CCODEID != "")
                {
                    Session["vcclsCode"] = id;

                }
                else
                {
                    id = Session["vcclsCode"].ToString();
                }
                id = encrypt.QueryStringModule.Decrypt(id);
                ViewBag.ViewClsid = id;
                DataSet Dresult = OBJDB.GetALLDIST();
                if (Dresult.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;
                    return View(DEO);
                }
                else
                {
                    ViewBag.Dist = Dresult.Tables[0];
                    List<SelectListItem> DISTLIST = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.Dist.Rows)
                    {
                        DISTLIST.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = new SelectList(DISTLIST, "Value", "Text");
                }

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "UDISE CODE" }, new { ID = "2", Name = "SCHL NAME" }, new { ID = "3", Name = "FATHER NAME" }, new { ID = "4", Name = "EMPLOYEE CODE" }, new { ID = "5", Name = "AADHAR NO." }, new { ID = "6", Name = "MOBILE NO" }, }, "ID", "Name", 1);
                ViewBag.SBy = itemsch.ToList();

            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
            if (TempData["sstring"] != null)
            {
                TempData.Keep("sstring");
                sstring = TempData["sstring"].ToString();
                //pageIndex = Convert.ToInt32(TempData["pageIndex"].ToString());
                pageIndex = Convert.ToInt32(page);
                DEO.StoreAllData = OBJDB.SEARCHSTAFFDATA(sstring, pageIndex);
                if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                }
                else
                {
                    @ViewBag.message = "1";
                    ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;

                    ViewBag.TotalCount1 = Convert.ToInt32(DEO.StoreAllData.Tables[1].Rows[0]["decount"]);
                    int tp = Convert.ToInt32(DEO.StoreAllData.Tables[1].Rows[0]["decount"]);

                    int pn = tp / 30;
                    int cal = 30 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;

                }
            }
            return View(DEO);
        }
        [HttpPost]
        public ActionResult ImportStaff(DEOModel DEO, string cmd, string CCODEID, FormCollection frc, int? page)
        {
            string DeoUser = null;
            string district = null;
            string Catg = null;
            string id = null;
            id = CCODEID;

            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.pagesize = pageIndex;


            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (CCODEID != null && CCODEID != "")
                {
                    Session["vcclsCode"] = id;

                }
                else
                {
                    id = Session["vcclsCode"].ToString();
                }
                id = encrypt.QueryStringModule.Decrypt(id);
                ViewBag.ViewClsid = id;
                DataSet Dresult = OBJDB.GetALLDIST();
                if (Dresult.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;
                    return View(DEO);
                }
                else
                {
                    ViewBag.Dist = Dresult.Tables[0];
                    List<SelectListItem> DISTLIST = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.Dist.Rows)
                    {
                        DISTLIST.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = new SelectList(DISTLIST, "Value", "Text");
                }

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "UDISE CODE" }, new { ID = "2", Name = "SCHL NAME" }, new { ID = "3", Name = "FATHER NAME" }, new { ID = "4", Name = "EMPLOYEE CODE" }, new { ID = "5", Name = "AADHAR NO." }, new { ID = "6", Name = "MOBILE NO" }, }, "ID", "Name", 1);
                ViewBag.SBy = itemsch.ToList();

            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
            string sstring = "";
            if (cmd == "Search")
            {

                string dist = frc["SelDist"];
                string block = frc["Edublock"];
                string cluster = frc["EduCluster"];
                string sby = frc["SearchBy"];
                string stext = frc["SearchString"];
                if (dist != "" && dist != null)
                {
                    sstring = "DIST= '" + dist + "'";
                    if (block != "" && block != null && block != "0")
                    {
                        //sstring += " and [Edu Block Name]='"+ block+"' ";
                        sstring += " and [EduBlock]='" + block + "' ";
                    }
                    if (cluster != "" && cluster != null && cluster != "0")
                    {
                        //sstring += " and [CLUSTER_NAME]='" + cluster + "' ";
                        sstring += " and [EDUCLUSTER]='" + cluster + "' ";
                    }
                    if (sby != "" && sby != null && stext != "")
                    {

                        if (sby == "1")
                        {
                            //sstring += " and [Udise Code]='" + stext + "'   ";
                            sstring += " and [Udise]='" + stext + "'   ";
                        }
                        if (sby == "2")
                        {
                            //sstring += " and [School Name] like '%" + stext.ToString().Trim() + "%'";
                            sstring += " and [SCHLNM] like '%" + stext.ToString().Trim() + "%'";

                        }
                        if (sby == "3")
                        {
                            //sstring += " and [Father Name] like '%" + stext.ToString().Trim() + "%'";
                            sstring += " and [Fname] like '%" + stext.ToString().Trim() + "%'";
                        }
                        if (sby == "4")
                        {
                            //sstring += " and [Employee Code]='" + stext + "' ";
                            sstring += " and [epunjabid]='" + stext + "' ";
                        }
                        if (sby == "5")
                        {
                            //sstring += " and [UID]='" + stext + "' ";
                            sstring += " and [adharno]='" + stext + "' ";
                        }
                        if (sby == "6")
                        {
                            sstring += " and [Mobile]='" + stext + "' ";
                            //sstring += " and [Mobile No]='" + stext + "' ";
                        }
                    }
                }
                TempData["sstring"] = sstring;
                TempData["pageIndex"] = pageIndex;
                //ViewBag.Dist = dist;
                DEO.Edublock = block;
                DEO.EduCluster = cluster;

                DEO.StoreAllData = OBJDB.SEARCHSTAFFDATA(sstring, pageIndex);
                if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                }
                else
                {
                    @ViewBag.message = "1";
                    ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;

                    ViewBag.TotalCount1 = Convert.ToInt32(DEO.StoreAllData.Tables[1].Rows[0]["decount"]);
                    int tp = Convert.ToInt32(DEO.StoreAllData.Tables[1].Rows[0]["decount"]);

                    int pn = tp / 30;
                    int cal = 30 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;

                }
            }
            if (cmd == "Update Cluster To STAFF")
            {
                string dcode = frc["typeDuty"];
                string hcluster = frc["hdnccode"];
                string staffidchk = frc["StaffName"];

                string StaffClusterresult = OBJDB.UpdateClusterToStaff(dcode, hcluster, staffidchk);
                if (StaffClusterresult == "0")
                {
                    //--------------Not Updated
                    // ViewData["result"] = 0;
                    TempData["result"] = 0;
                }
                else
                {
                    //-------------- Updated----------
                    // ViewData["result"] = 1;
                    TempData["result"] = 1;
                    TempData["TotImported"] = hcluster;

                    sstring = TempData["sstring"].ToString();
                    DEO.StoreAllData = OBJDB.SEARCHSTAFFDATA(sstring, pageIndex);
                    if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "DATA DOESN'T EXIST";
                    }
                    else
                    {
                        @ViewBag.message = "1";
                        ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(DEO.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(DEO.StoreAllData.Tables[1].Rows[0]["decount"]);

                        int pn = tp / 30;
                        int cal = 30 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                    }
                }
            }
            return View(DEO);
        }
        public JsonResult GetBlock(string DIST) // Calling on http post (on Submit)
        {
            DataSet result = OBJDB.SelectBlock(DIST);
            ViewBag.Edublock = result.Tables[0];
            List<SelectListItem> BlockList = new List<SelectListItem>();
            BlockList.Add(new SelectListItem { Text = "---Edu Block---", Value = "0" });
            foreach (System.Data.DataRow dr in ViewBag.Edublock.Rows)
            {
                BlockList.Add(new SelectListItem { Text = @dr["Edu Block Name"].ToString(), Value = @dr["Edu Block Name"].ToString() });
            }
            ViewBag.Edublock = BlockList;
            return Json(BlockList);

        }
        public JsonResult GetEduCluster(string BLOCK) // Calling on http post (on Submit)
        {
            DataSet result = OBJDB.Select_CLUSTER_NAME(BLOCK);
            ViewBag.EduCluster = result.Tables[0];
            List<SelectListItem> EduClusterList = new List<SelectListItem>();
            EduClusterList.Add(new SelectListItem { Text = "---Edu Cluster---", Value = "0" });
            foreach (System.Data.DataRow dr in ViewBag.EduCluster.Rows)
            {
                EduClusterList.Add(new SelectListItem { Text = @dr["CLUSTER_NAME"].ToString(), Value = @dr["CLUSTER_NAME"].ToString() });
            }
            ViewBag.Edublock = EduClusterList;
            return Json(EduClusterList);

        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ADDSTAFFNew(DEOModel DEO, string CCODEID)
        {
            CCODEID = "101003";
            string DeoUser = null;
            string district = null;
            string Catg = null;
            @ViewBag.DA = objCommon.GetDA();
            @ViewBag.exp = OBJDB.GetStaffExp();
            try
            {

                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                if (CCODEID != null && CCODEID != "")
                {
                    Session["vcclsCode"] = CCODEID;

                }
                else
                {
                    CCODEID = Session["vcclsCode"].ToString();
                }

                //CCODEID = encrypt.QueryStringModule.Decrypt(CCODEID);
                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                DataSet Dresult = OBJDB.GetClusterSCHOOLSTAFF(CCODEID, Session["DeoLoginExamCentre"].ToString());

                if (Dresult.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;
                    return View(DEO);
                }
                else
                {

                    ViewBag.ClusterCode = Dresult.Tables[0].Rows[0]["ccode"].ToString();
                    ViewBag.ClusterName = Dresult.Tables[0].Rows[0]["clusternam"].ToString();
                    ViewBag.Address = Dresult.Tables[0].Rows[0]["address"].ToString();
                    ViewBag.rstaff = Dresult.Tables[0].Rows[0]["rstaff"].ToString();
                    ViewBag.staff = Dresult.Tables[0].Rows[0]["staff"].ToString();
                    ViewBag.diff = Dresult.Tables[0].Rows[0]["diff"].ToString();
                    ViewBag.TotalCount = Dresult.Tables[0].Rows.Count;
                }
                DataSet Staffresult = OBJDB.GetSTAFFClusterWise(CCODEID, Session["DeoLoginExamCentre"].ToString());
                if (Staffresult.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";

                }
                else
                {
                    //SLNO, CCODE, TeacherName, FatherName, Designation, TypeOfDuty, Mobile
                    @ViewBag.message = "1";
                    ViewBag.SLNO = Staffresult.Tables[0].Rows[0][0].ToString();
                    ViewBag.CCODE = Staffresult.Tables[0].Rows[0][1].ToString();
                    ViewBag.TeacherName = Staffresult.Tables[0].Rows[0][2].ToString();
                    ViewBag.FatherName = Staffresult.Tables[0].Rows[0][3].ToString();
                    ViewBag.Designation = Staffresult.Tables[0].Rows[0][4].ToString();
                    ViewBag.TypeOfDuty = Staffresult.Tables[0].Rows[0][5].ToString();
                    ViewBag.Mobile = Staffresult.Tables[0].Rows[0][6].ToString();
                    //ViewBag.totcnt = Staffresult.Tables[0].Rows.Count;
                }

                DataSet DistwiseSCHL = OBJDB.GetDeoSchlDISTWise(district); // passing Value to DeoClass from model            
                List<SelectListItem> SDList = new List<SelectListItem>();
                SDList.Add(new SelectListItem { Text = "Others", Value = "Others" });
                foreach (System.Data.DataRow dr in DistwiseSCHL.Tables[0].Rows)
                {
                    SDList.Add(new SelectListItem { Text = @dr["schlnm"].ToString(), Value = @dr["schl"].ToString() });

                }

                ViewBag.schl = SDList;
                //--------------------------FILL CADRE----------
                DataSet RCADRE = OBJDB.GetCADRE(); // passing Value to DeoClass from model            
                List<SelectListItem> CDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in RCADRE.Tables[0].Rows)
                {
                    CDList.Add(new SelectListItem { Text = @dr["CADRE"].ToString(), Value = @dr["CADRE"].ToString() });
                }

                ViewBag.Dcadre = CDList;
                // --------------------------------End CADRE---------------------

            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
            return View(DEO);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult ADDSTAFFNew(DEOModel DEO, string CCODEID, FormCollection frm)
        {
            string DeoUser = null;
            string district = null;
            string uid = null;
            string Catg = null;
            @ViewBag.DA = objCommon.GetDA();
            ViewBag.exp = OBJDB.GetStaffExp();
            try
            {

                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                string CCID = frm["hdnccode"];
                // CCODEID = encrypt.QueryStringModule.Decrypt(CCODEID);
                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                uid = Session["UID"].ToString();

                DataSet Dresult = OBJDB.GetClusterSCHOOLSTAFF(CCID, Session["DeoLoginExamCentre"].ToString());
                if (Dresult.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;
                    return View(DEO);
                }
                else
                {
                    ViewBag.ClusterCode = Dresult.Tables[0].Rows[0][0].ToString();
                    ViewBag.ClusterName = Dresult.Tables[0].Rows[0][1].ToString();
                    ViewBag.ClusterSchoolCode = Dresult.Tables[0].Rows[0][2].ToString();
                    ViewBag.CCENT = Dresult.Tables[0].Rows[0][3].ToString();
                    ViewBag.Address = Dresult.Tables[0].Rows[0][4].ToString();
                    ViewBag.TotalCount = Dresult.Tables[0].Rows.Count;
                }
                DataSet DistwiseSCHL = OBJDB.GetDeoSchlDISTWise(district); // passing Value to DeoClass from model            
                List<SelectListItem> SDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in DistwiseSCHL.Tables[0].Rows)
                {
                    SDList.Add(new SelectListItem { Text = @dr["schlnm"].ToString(), Value = @dr["schl"].ToString() });
                }

                ViewBag.schl = SDList;
                //--------------------------FILL CADRE----------
                DataSet RCADRE = OBJDB.GetCADRE(); // passing Value to DeoClass from model            
                List<SelectListItem> CDList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in RCADRE.Tables[0].Rows)
                {
                    CDList.Add(new SelectListItem { Text = @dr["CADRE"].ToString(), Value = @dr["CADRE"].ToString() });
                }

                ViewBag.Dcadre = CDList;
                // --------------------------------End CADRE---------------------

                string CCODE = ViewBag.ClusterCode;

                string TD = DEO.typeDuty;
                string fname = DEO.StaffFatherName;
                string Name = DEO.StaffName;
                string expe = DEO.experience;
                string Month = DEO.expmonth;
                string gen = DEO.gender;
                string mob = DEO.Mobile;
                string SCHL = DEO.schlcode;
                string Schoolname = DEO.Selschool;
                string Aadharnum = DEO.adharno;
                string Epunjabid = DEO.teacherepunjabid;
                string cadre = DEO.cadre;

                string desi = frm["desi"];
                string IFSC = DEO.ifsccode;
                string Accno = DEO.bankaccno;
                string OtherSchool = DEO.Otherschl;
                if (Schoolname == "Others")
                {
                    Schoolname = OtherSchool;
                    SCHL = "*******";
                }

                string block = DEO.Edublock;
                string cluster = DEO.EduCluster;
                string UDISE = DEO.UdiseCode;
                string schlType = DEO.schlType;
                string schlmgmt = DEO.schlMGMT;
                string SCHLCAT = DEO.SCHLCAT;
                string dob = DEO.DOB;


                //string selected12 = frm["Selschool"];
                //string dor = DEO.DOR;
                //string remarks = DEO.remarks;

                //string Staffres = OBJDB.ADDSTAFFDETAILS(uid, district,CCODE, TD, fname, Name, expe, Month, gen, mob, SCHL, Schoolname, Aadharnum, Epunjabid, cadre, desi, IFSC, Accno,dor,remarks);
                string Staffres = OBJDB.ADDSTAFFDETAILS_NEW(uid, district, CCODE, TD, fname, Name, expe, Month, gen, mob, SCHL, Schoolname, Aadharnum, Epunjabid, cadre, desi, IFSC, Accno, block, cluster, UDISE, schlType, schlmgmt, SCHLCAT, dob);
                if (Staffres == "1")
                {
                    ViewData["result"] = "1";

                }
                else if (Staffres == "-1")
                {
                    ViewData["result"] = "-1";
                }
                else if (Staffres == "0")
                {
                    ViewData["result"] = "0";
                }



            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
            return View(DEO);
        }
        //----------- Close New Add Staff---------------------------------------------------
        #region DEO SCHOOL Data Download
        public ActionResult DownloadDeoSchoolDistWise() // Download Admin DEO Data file
        {
            DEOModel AM = new DEOModel();
            FormCollection frm = new FormCollection();
            string district = null;
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                district = Session["Dist"].ToString();

                if (Session["USER"] != null)
                {
                    DataSet ds1 = OBJDB.DownloadDeoSchoolDistWise(district);
                    if (ds1.Tables.Count > 0)
                    {
                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                wb.Worksheets.Add(ds1.Tables[0]);
                                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                wb.Style.Font.Bold = true;
                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("content-disposition", "attachment;filename=" + "DeoSchoolListDistwise" + ".xls");
                                //Response.AddHeader("content-disposition", "attachment;filename= DownloadChallanReport.xlsx");

                                using (MemoryStream MyMemoryStream = new MemoryStream())
                                {
                                    wb.SaveAs(MyMemoryStream);
                                    MyMemoryStream.WriteTo(Response.OutputStream);
                                    Response.Flush();
                                    Response.End();
                                }
                            }
                            ViewData["Result"] = "1";
                            return RedirectToAction("DeoHome", "DeoPortal");
                        }
                        else
                        {
                            return RedirectToAction("DeoHome", "DeoPortal");
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Download Failure";
                        ViewData["Result"] = "0";
                        return RedirectToAction("DeoHome", "DeoPortal");
                    }
                }
                else
                {
                    return RedirectToAction("DeoHome", "DeoPortal");
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View(AM);
            }
        }
        #endregion DEO SCHOOL Data Download
        //-----------------------------------End------------------------------------------------------

        #region Begin Admin Result Update MIS
        public ActionResult AdminUpdateCentreMaster()
        {
            try
            {
                if (Session["User"] == null && (Session["User"].ToString().ToUpper() != "ADMIN"))
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                else
                {
                    return View();
                }
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "DeoPortal");
            }

        }

        [HttpPost]
        public ActionResult AdminUpdateCentreMaster(DEOModel AM) // HttpPostedFileBase file
        {
            AbstractLayer.DEODB objDB = new AbstractLayer.DEODB();
            try
            {
                // firm login // dist 
                if ((Session["User"].ToString().ToUpper() != "ADMIN"))
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                else
                {
                    //HttpContext.Session["AdminType"]
                    string AdminUser = Session["User"].ToString();
                    int AdminId = 1;// Convert.ToInt32(Session["AdminId"].ToString());
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
                        string fileName1 = "FirmResultMIS_" + AdminUser + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

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
                            string UserNM = Session["User"].ToString();

                            DataTable dt1 = ds.Tables[0];
                            dt1.AcceptChanges();
                            // Get Unique and  noe empty records
                            // dt1 = dt1.AsEnumerable().GroupBy(x => x.Field<string>("cschl")).Select(g => g.First()).Where(r => r.ItemArray[1].ToString() != "").CopyToDataTable();

                            DataTable dtexport;

                            string CheckMis = objDB.CheckResultMisExcel(dt1, UserNM, out dtexport);
                            if (CheckMis == "")
                            {
                                if (dt1.Columns.Contains("Status"))
                                {
                                    dt1.Columns.Remove("Status");
                                }

                                string Result1 = "";
                                string OutError = "";
                                DataTable dtResult = objDB.AdminUpdateCentreMaster(dt1, AdminId, out OutError);// OutStatus mobile
                                if (OutError == "1")
                                {
                                    ViewBag.Message = "File Uploaded Successfully";
                                    ViewData["Result"] = "1";
                                }
                                else
                                {
                                    ViewBag.Message = "File Not Uploaded Successfully : " + OutError.ToString();
                                    ViewData["Result"] = "0";
                                }
                                return View();
                            }
                            else
                            {
                                if (dtexport != null)
                                {
                                    ExportDataFromDataTable(dtexport, "Error_ResultUpdate");
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
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }
            return View();
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
                        string fileName1 = filename + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";  //103_230820162209_347
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

                return RedirectToAction("Index", "DeoPortal");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "DeoPortal");
            }

        }
        #endregion  Admin Result Update MIS

        public JsonResult UpdateCentStaff(string staffid, string cent)
        {
            try
            {
                string dee = "";
                string outstatus = "";
                string Search = string.Empty;
                string res = OBJDB.UpdateCentStaff(staffid, cent);
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


        public ActionResult AdminDeoUser(string id, string cmd)
        {
            try
            {
                DEOModel DEO = new DEOModel();
                if (Session["USER"] == null || Session["Name"].ToString() != "ADMIN")
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                string Search = "";
                if (id != "" && id != null)
                {
                    DataSet ds = OBJDB.DeoUserUnlock(id);
                    ViewData["result"] = "1";
                }
                else
                {
                    ViewData["result"] = null;
                }

                DEO.StoreAllData = OBJDB.GetAdminDeoUser(Search);
                if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;
                    return View(DEO);
                }
                else
                {
                    @ViewBag.message = "1";
                    ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                    return View(DEO);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }

        }

        public ActionResult ObserversReport(int? page)
        {
            //return View();
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser == "ADMIN")
                {

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    //Search = "a.DIST = '" + district + "'";
                    Search = "DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }
                obj.StoreAllData = OBJDB.ExaminerReport(Search, Catg, pageIndex);
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Observer Does not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }

        [HttpPost]
        public ActionResult ObserversReport(FormCollection fc, int? page)
        {
            //return View();
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser == "ADMIN")
                {

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    //Search = "a.DIST = '" + district + "'";
                    Search = "DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }

                obj.SelDist = fc["SelDist"];
                obj.Class = fc["Class"];
                obj.centreDate = fc["centreDate"];

                obj.StoreAllData = OBJDB.ObserversReport(obj, pageIndex);
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Observer Does not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }

        public ActionResult OldStaffAppointmentReport(int? page)
        {
            //return View();
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser == "ADMIN")
                {

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    //Search = "a.DIST = '" + district + "'";
                    Search = "DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }
                obj.StoreAllData = OBJDB.ExaminerReport(Search, Catg, pageIndex);
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Data Does not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }

        [HttpPost]
        public ActionResult OldStaffAppointmentReport(FormCollection fc, int? page)
        {
            //return View();
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser == "ADMIN")
                {

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    //Search = "a.DIST = '" + district + "'";
                    Search = "DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }

                obj.SelDist = fc["SelDist"];
                obj.Class = fc["Class"];
                obj.centreDate = fc["centreDate"];

                obj.StoreAllData = OBJDB.OldStaffAppointmentReport(obj, pageIndex);
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Data Does not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }
        public ActionResult PreviousStaffReport(int? page)
        {
            //return View();
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser == "ADMIN")
                {

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    //Search = "a.DIST = '" + district + "'";
                    Search = "DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }
                obj.StoreAllData = OBJDB.ExaminerReport(Search, Catg, pageIndex);
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Previous Staff Does not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }

        [HttpPost]
        public ActionResult PreviousStaffReport(FormCollection fc, int? page)
        {
            //return View();
            string DeoUser = null;
            string district = null;
            string Catg = null;
            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {
                if (Session["USER"] == null && Session["Name"] == null)
                {
                    return RedirectToAction("Index", "DeoPortal");
                }

                DeoUser = Session["USER"].ToString();
                district = Session["Dist"].ToString();
                if (DeoUser == "ADMIN")
                {

                    DataSet Dresult = OBJDB.AdminGetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }

                    ViewBag.Dist = DistList;
                }
                else
                {
                    DataSet result = OBJDB.GetDeoDIST(DeoUser); // passing Value to DBClass from model            
                    List<SelectListItem> DList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in result.Tables[0].Rows)
                    {
                        DList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DList;
                }

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                DEOModel obj = new DEOModel();
                Catg = obj.Category;
                string Search = string.Empty;
                if (pageIndex == 1)
                {

                    TempData["CenterListSelDist"] = null;
                    TempData["CenterListSelectList"] = null;
                    TempData["CenterListSearchString"] = null;

                }
                if (TempData["CenterListSelDist"] == null)
                    //Search = "a.DIST = '" + district + "'";
                    Search = "DIST = '" + district + "'";
                else
                {
                    Search = Convert.ToString(TempData["CenterListSelDist"]);
                    // ViewBag.SelectedItem = TempData["CenterListSelectList"];
                    // ViewBag.Searchstring = TempData["CenterListSearchString"];
                    // ViewBag.SelectedSession = TempData["ImportData10thClassSession"];
                }

                obj.SelDist = fc["SelDist"];
                //obj.Class = fc["Class"];
                //obj.centreDate = fc["centreDate"];

                obj.StoreAllData = OBJDB.PreviousStaffReport(obj, pageIndex);
                //obj.TotalCount = OBJDB.SelectClusterListByUserCountReport(Search, Catg, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Previous Staff Does not Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;

                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }

        #region Sports Marks Entry 
        public ActionResult SportsMarksEntry()
        {
            try
            {
                if (Session["User"] == null && (Session["User"].ToString().ToUpper() != "ADMIN"))
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                AbstractLayer.DEODB objDB = new AbstractLayer.DEODB();
                DEOModel MS = new DEOModel();
                string currentUser = Session["User"].ToString().ToUpper();
                MS.StoreAllData2 = objDB.GetSportsMarksViewList(currentUser);
                if (MS.StoreAllData2 == null || MS.StoreAllData2.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount2 = 0;
                    return View(MS);
                }
                else
                {
                    ViewBag.TotalCount2 = MS.StoreAllData2.Tables[0].Rows.Count;

                    List<SelectListItem> CDList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in MS.StoreAllData2.Tables[1].Rows)
                    {
                        CDList.Add(new SelectListItem { Text = @dr["Name"].ToString(), Value = @dr["Name"].ToString() });
                    }
                    ViewBag.rslist = CDList;
                    return View(MS);
                }


            }
            catch (Exception)
            {
                return RedirectToAction("Index", "DeoPortal");
            }

        }

        [HttpPost]
        public ActionResult SportsMarksEntry(FormCollection frm, string cmd)
        {
            try
            {
                if (Session["User"] == null && (Session["User"].ToString().ToUpper() != "ADMIN"))
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                AbstractLayer.DEODB objDB = new AbstractLayer.DEODB();
                DEOModel MS = new DEOModel();
                string currentUser = Session["User"].ToString().ToUpper();

                if (cmd == "Final Submit")
                {
                    MS.StoreAllData = objDB.DeoFSSportsMarksEntry(currentUser, "0");
                }

                #region "Download Inserted Data"

                else if (cmd == "Download Inserted Data")
                {
                    MS.StoreAllData = objDB.GetDeoAllDataSportsMarksEntry(currentUser, "0");
                    DataSet ds1 = MS.StoreAllData;
                    if (ds1.Tables.Count > 0)
                    {
                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                wb.Worksheets.Add(ds1.Tables[0]);
                                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                wb.Style.Font.Bold = true;
                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("content-disposition", "attachment;filename=" + "DeoInsertedData" + ".xls");
                                //Response.AddHeader("content-disposition", "attachment;filename= DownloadChallanReport.xlsx");

                                using (MemoryStream MyMemoryStream = new MemoryStream())
                                {
                                    wb.SaveAs(MyMemoryStream);
                                    MyMemoryStream.WriteTo(Response.OutputStream);
                                    Response.Flush();
                                    Response.End();
                                }
                            }
                            ViewData["Result"] = "11";
                        }
                    }

                }
                #endregion

                //-BEGIN INSERTED RECORD
                MS.StoreAllData2 = objDB.GetSportsMarksViewList(currentUser);
                if (MS.StoreAllData2 == null || MS.StoreAllData2.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount2 = 0;
                    //return View(MS);
                }
                else
                {
                    ViewBag.TotalCount2 = MS.StoreAllData2.Tables[0].Rows.Count;

                    //List<SelectListItem> CDList = new List<SelectListItem>();
                    //foreach (System.Data.DataRow dr in MS.StoreAllData2.Tables[1].Rows)
                    //{
                    //    CDList.Add(new SelectListItem { Text = @dr["Name"].ToString(), Value = @dr["Name"].ToString() });
                    //}
                    //ViewBag.rslist = CDList;
                    //return View(MS);
                }
                //-BEGIN INSERTED RECORD

                MS.RollNo = frm["RollNo"].ToString();
                MS.StoreAllData = objDB.GetDeoStudentDetails(MS.RollNo, currentUser);
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View(MS);
                }
                else
                {
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;

                    List<SelectListItem> CDList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[1].Rows)
                    {
                        CDList.Add(new SelectListItem { Text = @dr["Name"].ToString(), Value = @dr["Name"].ToString() });
                    }

                    ViewBag.rslist = CDList;

                    string reclock = MS.StoreAllData.Tables[0].Rows[0]["reclock"].ToString();
                    if (reclock == "1")
                    {
                        ViewBag.reclock = "1";
                    }
                    else
                    { ViewBag.reclock = "0"; }
                    return View(MS);
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "DeoPortal");
            }
        }

        [HttpPost]
        public JsonResult UpdateMatricResult(string refno, string selSport, string roll, string ccode, string ag, string ses, string pos)
        {
            try
            {
                if (Session["User"] == null && (Session["User"].ToString().ToUpper() != "ADMIN"))
                {
                    //return RedirectToAction("Index", "DeoPortal");
                    return Json(new { sn = "", chid = "" }, JsonRequestBehavior.AllowGet);
                }
                AbstractLayer.DEODB objDB = new AbstractLayer.DEODB();
                string dee = "No";
                string res = "No";
                string currentUser = Session["User"].ToString().ToUpper();
                if (selSport != "" && ccode != "" && ag != "" && ses != "" && pos != "")
                {

                    string result = objDB.UpdateSportMarkEntry(roll, refno, selSport, currentUser, ccode, ag, ses, pos);
                    res = result;
                    if (result != "0" && result != "-1" && result != "")
                    {
                        dee = "Yes";
                    }

                }

                return Json(new { sn = dee, chid = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return null;
            }
        }

        [HttpPost]
        public JsonResult DeleteSportEntry(string roll)
        {
            try
            {
                if (Session["User"] == null && (Session["User"].ToString().ToUpper() != "ADMIN"))
                {
                    //return RedirectToAction("Index", "DeoPortal");
                    return Json(new { sn = "", chid = "" }, JsonRequestBehavior.AllowGet);
                }
                AbstractLayer.DEODB objDB = new AbstractLayer.DEODB();
                string dee = "No";
                string res = "No";
                string currentUser = Session["User"].ToString().ToUpper();
                if (roll != "")
                {

                    string result = objDB.DeleteSportEntry(roll, currentUser);
                    res = result;
                    if (result != "0" && result != "-1" && result != "")
                    {
                        dee = "Yes";
                    }

                }

                return Json(new { sn = dee, chid = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return null;
            }
        }

        [HttpPost]
        public JsonResult BackToRecheck(string roll)
        {
            try
            {
                if (Session["User"] == null && (Session["User"].ToString().ToUpper() != "ADMIN"))
                {
                    //return RedirectToAction("Index", "DeoPortal");
                    return Json(new { sn = "", chid = "" }, JsonRequestBehavior.AllowGet);
                }
                AbstractLayer.DEODB objDB = new AbstractLayer.DEODB();
                string dee = "No";
                string res = "No";
                string currentUser = Session["User"].ToString().ToUpper();
                if (roll != "")
                {

                    string result = objDB.BackToRecheck(roll, currentUser);
                    res = result;
                    if (result != "0" && result != "-1" && result != "")
                    {
                        dee = "Yes";
                    }

                }

                return Json(new { sn = dee, chid = res }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return null;
            }
        }

        public ActionResult SportsMarksViewList()
        {
            try
            {
                AbstractLayer.DEODB objDB = new AbstractLayer.DEODB();
                DEOModel MS = new DEOModel();
                string currentUser = Session["User"].ToString().ToUpper();

                if (Session["User"] == null && (Session["User"].ToString().ToUpper() != "ADMIN"))
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                else
                {
                    DataSet Dresult = OBJDB.AdminGetDeoDIST(currentUser); // passing Value to DBClass from model            
                    List<SelectListItem> DistList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                    }
                    ViewBag.Dist = DistList;

                    MS.StoreAllData = null;// objDB.GetSportsMarksViewList(currentUser);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                    else
                    {
                        List<SelectListItem> CDList = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[1].Rows)
                        {
                            CDList.Add(new SelectListItem { Text = @dr["Name"].ToString(), Value = @dr["Name"].ToString() });
                        }

                        ViewBag.rslist = CDList;

                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        return View(MS);
                    }
                }
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "DeoPortal");
            }
        }

        [HttpPost]
        public ActionResult SportsMarksViewList(FormCollection frm, string cmd)
        {
            try
            {
                if (Session["User"] == null && (Session["User"].ToString().ToUpper() != "ADMIN"))
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                AbstractLayer.DEODB objDB = new AbstractLayer.DEODB();
                DEOModel MS = new DEOModel();
                string currentUser = Session["User"].ToString().ToUpper();

                DataSet Dresult = OBJDB.AdminGetDeoDIST(currentUser); // passing Value to DBClass from model            
                List<SelectListItem> DistList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                {
                    DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }
                ViewBag.Dist = DistList;

                MS.SelDist = frm["SelDist"].ToString();

                if (cmd == "Final Submit")
                {
                    MS.StoreAllData = objDB.DeoFSSportsMarksEntry(currentUser, MS.SelDist);
                }
                if (cmd == "Return to Check All")
                {
                    string res = objDB.BackToRecheckAll(currentUser, MS.SelDist);
                }

                #region "Download Inserted Data"

                else if (cmd == "Download Inserted Data")
                {
                    MS.StoreAllData = objDB.GetDeoAllDataSportsMarksEntry(currentUser, MS.SelDist);
                    DataSet ds1 = MS.StoreAllData;
                    if (ds1.Tables.Count > 0)
                    {
                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                wb.Worksheets.Add(ds1.Tables[0]);
                                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                wb.Style.Font.Bold = true;
                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("content-disposition", "attachment;filename=" + "DeoInsertedData" + ".xls");
                                //Response.AddHeader("content-disposition", "attachment;filename= DownloadChallanReport.xlsx");

                                using (MemoryStream MyMemoryStream = new MemoryStream())
                                {
                                    wb.SaveAs(MyMemoryStream);
                                    MyMemoryStream.WriteTo(Response.OutputStream);
                                    Response.Flush();
                                    Response.End();
                                }
                            }
                            ViewData["Result"] = "11";
                        }
                    }

                }
                #endregion


                //MS.RollNo = frm["RollNo"].ToString();

                MS.StoreAllData = objDB.GetSportsMarksViewList(MS.SelDist);//objDB.GetDeoStudentDetails(MS.RollNo, currentUser);
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View(MS);
                }
                else
                {
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;

                    List<SelectListItem> CDList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[1].Rows)
                    {
                        CDList.Add(new SelectListItem { Text = @dr["Name"].ToString(), Value = @dr["Name"].ToString() });
                    }

                    ViewBag.rslist = CDList;

                    string reclock = MS.StoreAllData.Tables[0].Rows[0]["reclock"].ToString();
                    if (reclock == "1")
                    {
                        ViewBag.reclock = "1";
                    }
                    else
                    { ViewBag.reclock = "0"; }
                    return View(MS);
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "DeoPortal");
            }
        }

        public ActionResult SportsMarksViewListAdmin()
        {
            try
            {
                AbstractLayer.DEODB objDB = new AbstractLayer.DEODB();
                DEOModel MS = new DEOModel();
                string currentUser = Session["User"].ToString().ToUpper();

                if (Session["User"] == null && (Session["User"].ToString().ToUpper() != "ADMIN"))
                {
                    return RedirectToAction("Index", "DeoPortal");
                }
                else
                {
                    MS.StoreAllData = objDB.GetSportsMarksViewList(currentUser);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                    else
                    {
                        List<SelectListItem> CDList = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[1].Rows)
                        {
                            CDList.Add(new SelectListItem { Text = @dr["Name"].ToString(), Value = @dr["Name"].ToString() });
                        }

                        ViewBag.rslist = CDList;

                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        return View(MS);
                    }
                }
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "DeoPortal");
            }
        }
        #endregion Sports Marks Entry



        //--------------------PSTET For School Login------------------------------//
        [SessionCheckFilter]
        public ActionResult PSTET(DEOModel DEO)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];

            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {

                DEO.StoreAllData = OBJDB.Get_PSTET_Master(loginSession.SCHL);
                if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                    return View(DEO);
                }

            }
            catch (Exception ex)
            {
                return View();

            }
        }

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult PSTET(DEOModel DEO, FormCollection frc)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];

            ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
            try
            {


                DEO.StoreAllData = OBJDB.Get_PSTET_Master(loginSession.SCHL);
                if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;
                    return View(DEO);
                }

            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();

            }
        }
        //---------------------------End------------------------------//

    }
}