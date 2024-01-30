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
using System.Text.RegularExpressions;
using ClosedXML;
using ClosedXML.Excel;
using System.Data.OleDb;
using System.Reflection;
using System.Web.Routing;

namespace PSEBONLINE.Controllers
{

    public class DMController : Controller
    {

        #region SiteMenu
        //Executes before every action
        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                // Start ********* Get all ActionName of all Controller by return type;
                ////string actionname1 = "";
                ////actionname1 = AbstractLayer.StaticDB.GetActionsOfController();
                //End

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
                             new RouteValueDictionary(new { controller = "Admin", action = "PageNotAuthorized" }));
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

        #endregion SiteMenu

        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.DMDB objDM = new AbstractLayer.DMDB();     
        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        AbstractLayer.SchoolDB objSCHL = new AbstractLayer.SchoolDB();

        #region ChangePassword & LogOut
        public ActionResult Logout()
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Admin");

        }

        public ActionResult Change_Password()
        {
            string DMUser = null;           
            if (Session["AdminId"] == null )
            {
                return RedirectToAction("Index", "Admin");
            }
            DMUser = Session["UserName"].ToString();
            ViewBag.DMUser = Session["UserName"].ToString();            
            return View();
        }
        [HttpPost]
        public ActionResult Change_Password(FormCollection frm)
        {
            string DMUser = null;           
            if (Session["AdminId"] == null )
            {
                return RedirectToAction("Index", "Admin");
            }
            DMUser = Session["UserName"].ToString();
            ViewBag.DMUser = Session["UserName"].ToString();          

            string CurrentPassword = string.Empty;
            string NewPassword = string.Empty;


            if (frm["ConfirmPassword"] != "" && frm["NewPassword"] != "")
            {
                if (frm["ConfirmPassword"].ToString() == frm["NewPassword"].ToString())
                {
                    CurrentPassword = frm["CurrentPassword"].ToString();
                    NewPassword = frm["NewPassword"].ToString();
                    int result = objDM.ChangePassword(Convert.ToInt32(Session["AdminId"].ToString()), CurrentPassword, NewPassword); // passing Value to SchoolDB from model and Type 1 For regular
                    if (result > 0)
                    {
                        ViewData["resultDCP"] = 1;
                        return View();
                        // return RedirectToAction("Index", "DM");
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

        #endregion ChangePassword & LogOut

        //
        #region Begin DM Panel
        public ActionResult SchoolList(DMModel asm, int? page)
        {
            try
            {
               // string DistAllow = "";
                if (Session["AdminId"] == null )
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (Session["DistAllow"].ToString() == "")
                {
                    return RedirectToAction("Index", "Admin");
                }               

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


                //  ViewBag.MyDist = objCommon.GetDistE().Where(s=>s.Value.Contains(DistAllow));
                ViewBag.MySch = objCommon.SearchSchoolItems().Where(s => Convert.ToInt32(s.Value) < 3);
                ViewBag.MySchoolType = objCommon.GetSchool();
                ViewBag.MyClassType = objCommon.GetClass().Where(s=>s.Value != "1");
               
                var itemAction = new SelectList(new[] { new { ID = "1", Name = "Pending" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";

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
                    DMModel ASM = new DMModel();
                    if (asm.StoreAllData == null)
                    {
                        string Search = string.Empty;
                        Search = "SCHL like '%' ";
                        if (DistAllow != "")
                        {
                            Search += " and DIST in (" + DistAllow + ")";
                        }

                        if (TempData["SearchSchoolData"] != null)
                        {
                            Search += TempData["SearchSchoolData"].ToString();
                            ViewBag.SelectedClassType = TempData["SelectedClassType"];
                            ViewBag.SelectedAction = TempData["SelAction"];
                            ViewBag.SelectedItem = TempData["SelectedItem"];



                            ASM.StoreAllData = objDM.DMSchoolList(Search, pageIndex, 20);

                            if (asm.StoreAllData == null || asm.StoreAllData.Tables[0].Rows.Count == 0)
                            {
                                ViewBag.Message = "Record Not Found";
                                ViewBag.LastPageIndex = 0;
                                ViewBag.TotalCount = 0;
                                return View();
                            }
                            else
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
                        }
                        else
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.LastPageIndex = 0;
                            ViewBag.TotalCount = 0;
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.TotalCount = asm.StoreAllData.Tables[0].Rows.Count;                       
                        return View(asm);
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
        public ActionResult SchoolList(DMModel asm, FormCollection frm, int? page)
        {
            try
            {
                //string DistAllow = "";
                if (Session["AdminId"] == null )
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (Session["DistAllow"].ToString() == "")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    //if (Session["DistAllow"].ToString().Contains(","))
                    //{ DistAllow = Session["DistAllow"].ToString().Remove(Session["DistAllow"].ToString().LastIndexOf(","), 1); }
                    //else
                    //{
                    //    DistAllow = Session["DistAllow"].ToString();
                    //}
                }

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

                // SchoolModels asm = new SchoolModels();
                if (ModelState.IsValid)
                {
                    ViewBag.MySchoolType = objCommon.GetSchool();
                    ViewBag.MyClassType = objCommon.GetClass().Where(s=>s.Value != "1");

                    var itemAction = new SelectList(new[] { new { ID = "1", Name = "Pending" }, }, "ID", "Name", 1);
                    ViewBag.MyAction = itemAction.ToList();
                    ViewBag.SelectedAction = "0";
                    // bind Dist 
                   // ViewBag.MyDist = objCommon.GetDistE().Where(s => s.Value.Contains(DistAllow));
                    ViewBag.MySch = objCommon.SearchSchoolItems().Where(s => Convert.ToInt32(s.Value) < 3);
                    string Search = string.Empty;
                    Search = "SCHL like '%' ";
                    if (frm["Dist1"] != "")
                    {
                        ViewBag.SelectedDist = frm["Dist1"];
                        TempData["SelectedDist"] = frm["Dist1"];
                        Search += " and dist=" + frm["Dist1"].ToString();
                    }

                    int SelAction = 0;
                    if (frm["SelAction"] != "")
                    {
                        TempData["SelAction"] = frm["SelAction"];
                        ViewBag.SelectedAction = frm["SelAction"];
                        SelAction = Convert.ToInt32(frm["SelAction"].ToString());
                        if (SelAction == 1)
                        {
                            Search += " and (SeniorReceiveNo is null or MatricReceiveNo is null) ";
                        }
                    }

                    //if (frm["SchoolType"] != "")
                    //{
                    //    ViewBag.SelectedSchoolType = frm["SchoolType"];
                    //    TempData["SelectedSchoolType"] = frm["SchoolType"];
                    //    Search += " and st.schooltype='" + frm["SchoolType"].ToString() + "'";
                    //}

                    if (frm["ClassType"] != "")
                    {
                        ViewBag.SelectedClassType = frm["ClassType"];
                        TempData["SelectedClassType"] = frm["ClassType"];
                        Search += " and classid=" + frm["ClassType"].ToString();
                    }

                    if (frm["Sch1"] != "")
                    {
                        ViewBag.SelectedItem = frm["Sch1"];
                        TempData["SelectedItem"] = frm["Sch1"];
                        int SelValueSch = Convert.ToInt32(frm["Sch1"].ToString());

                        if (frm["SearchString"] != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and SCHL=" + frm["SearchString"].ToString(); }
                            else if (SelValueSch == 2)
                            { Search += " and  SCHLE like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 3)
                            { Search += " and IDNO='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 4)
                            { Search += " and STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 5)
                            { Search += " and SCHLE=" + frm["SearchString"].ToString(); }
                        }

                    }
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                    TempData["SearchSchoolData"] = Search;
                    TempData.Keep(); // to store search value for view
                    if (DistAllow != "")
                    {
                        Search += " and DIST in (" + DistAllow + ")";
                    }
                    asm.StoreAllData = objDM.DMSchoolList(Search, pageIndex, 20);

                    if (asm.StoreAllData == null || asm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.LastPageIndex = 0;
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = asm.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(asm.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        ViewBag.TotalCount1 = count;
                        int tp = Convert.ToInt32(count);
                        int pn = tp / 20;
                        int cal = 20 * pn;
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
                    return RedirectToAction("SchoolList", "DM");
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));               
                return View();
            }
        }

        public JsonResult ReceiveCCE(string class1,string cceremarks, string receivedate, string schl)
        {
            try
            {
                string dee = "";
                 string OutReceiveNo = "";               
               // string OutReceiveNo = "CCE" + schl.ToString();
                string Search = string.Empty;  
                DateTime date;
                if (DateTime.TryParseExact(receivedate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    objDM.ReceiveCCE(Convert.ToInt32(Session["AdminId"].ToString()), Convert.ToInt32(class1),"C", cceremarks, schl, out OutReceiveNo, date);
                    if (OutReceiveNo == "0")
                    {
                        dee = "No";
                    }
                    else if (OutReceiveNo == "-1")
                    {
                        dee = "NF";
                    }
                    else if (OutReceiveNo != "0")
                    { dee = "Yes"; }
                    else
                    {
                        dee = "No";
                    }
                }
                return Json(new { sn = dee, Rcno = OutReceiveNo }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));               
                return null;
            }
        }

        public ActionResult GenerateChallan(string id, DMModel ASM, FormCollection frm)
        {
            try
            {

                int class1 = 4;
                if (id == "M")
                { class1 = 2; }
                else if (id == "S")
                { class1 = 4; }
                else if (id == "MG")
                { class1 = 2; }
                else if (id == "SG")
                { class1 = 4; }

                ViewBag.SearchId = id;


                string DistAllow = "";
                if (Session["AdminId"] == null )
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (Session["DistAllow"].ToString() == "")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    if (Session["DistAllow"].ToString().Contains(","))
                    { DistAllow = Session["DistAllow"].ToString().Remove(Session["DistAllow"].ToString().LastIndexOf(","), 1); }
                    else
                    {
                        DistAllow = Session["DistAllow"].ToString();
                    }
                }
                if (DistAllow == "")
                {
                    ModelState.AddModelError("", "District Not Found");
                    return View();
                }
                else
                {
                    if (id != null)
                    {
                        if (ASM.StoreAllData == null)
                        {
                            string Search = string.Empty;
                            Search = "c.Rno like '%' and DairyNo is null and CCEReceiveNo is not null   and c.receivedby =" + Session["AdminId"].ToString() + " ";
                            if (DistAllow != "")
                            {
                                Search += " and sm.DIST in (" + DistAllow + ")";
                            }
                            if (class1 > 0)
                            {
                                Search += " and c.class=" + class1 + "";
                            }

                            // and b.class=4 and DairyNo is null
                            ASM.StoreAllData = objDM.GetReceivedCCE(Search);
                            if (ASM.StoreAllData != null)
                            {
                                ViewBag.Message = "";
                                ViewData["ReceiveStatus"] = "1";
                                ViewBag.TotalCount = ASM.StoreAllData.Tables[0].Rows.Count;
                            }
                            else
                            {
                                ViewBag.TotalCount = 0;
                                ViewData["ReceiveStatus"] = "3";
                                ViewBag.Message = "Record Not Found";
                            }
                            return View(ASM);
                        }
                        else
                        {
                            ViewBag.TotalCount = ASM.StoreAllData.Tables[0].Rows.Count;
                            return View(ASM);
                        }
                    }
                    else
                    {                        
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
       public ActionResult GenerateChallan(string id)
        {
            try
            {
                int class1 = 4;
                if (id == "M")
                {class1 = 2; }
                else if (id == "S")
                { class1 = 4; }
                else if (id == "MG")
                { class1 = 2; }
                else if (id == "SG")
                { class1 = 4; }

                ViewBag.SearchId = id;

                string DistAllow = "";
                if (Session["AdminId"] == null )
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (Session["DistAllow"].ToString() == "")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    if (Session["DistAllow"].ToString().Contains(","))
                    { DistAllow = Session["DistAllow"].ToString().Remove(Session["DistAllow"].ToString().LastIndexOf(","), 1); }
                    else
                    {
                        DistAllow = Session["DistAllow"].ToString();
                    }
                }               
                if (DistAllow == "")
                {
                    ModelState.AddModelError("", "District Not Found");
                    return View();
                }
                else
                {
                    string OutDairyNo = "0";

                    objDM.GenerateChallanCCE(Convert.ToInt32(class1), "C", DistAllow,  out OutDairyNo);

                    if (OutDairyNo != "0")
                    {
                        return RedirectToAction("Challaan", "DM", new { id = OutDairyNo });
                    }
                    else
                    {
                        return View();
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

        public ActionResult Challaan(string id)
        {
            try
            {
                string DairyNo = id;              
                if (DairyNo == null || DairyNo == "0")
                {
                    return RedirectToAction("Home", "DM");
                }
                DMModel DM = new DMModel();  
                if (Session["AdminId"] == null )
                {
                    return RedirectToAction("Logout", "Admin");
                }   
                DataSet ds = objDM.GetChallanDetailsByDairyNo(DairyNo);
                DM.StoreAllData = ds;
                if (DM.StoreAllData != null)
                {
                    ViewBag.TotalCount = DM.StoreAllData.Tables[0].Rows.Count;
                    DM.DairyNo = ds.Tables[0].Rows[0]["DairyNo"].ToString();
                    DM.DairyDate = ds.Tables[0].Rows[0]["DairyDate"].ToString();
                    DM.UserName = ds.Tables[0].Rows[0]["User_fullnm"].ToString();
                    DM.DistName = ds.Tables[0].Rows[0]["DistName"].ToString();
                    DM.OfficeName = ds.Tables[0].Rows[0]["Branch"].ToString();

                }
                return View(DM);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Admin");
            }
        }
    
        public ActionResult CompleteChallanDetails(string id)
        {
            try
            {
               string  DairyNo = id;
                if (DairyNo == null)
                { ViewBag.DairyNo = "0";
                    DairyNo = "";
                }
                else
                {  ViewBag.DairyNo = DairyNo;} 
                DMModel DM = new DMModel();
                if (Session["AdminId"] == null )
                {
                    return RedirectToAction("Logout", "Admin");
                }
                DataSet ds = objDM.GetChallanDetailsByDairyNo(DairyNo);
                DM.StoreAllData = ds;
                if (DM.StoreAllData != null)
                {
                    ViewBag.TotalCount = DM.StoreAllData.Tables[0].Rows.Count;
                    if (DairyNo!="")
                    {
                        DM.DairyNo = ds.Tables[0].Rows[0]["DairyNo"].ToString();
                        DM.DairyDate = ds.Tables[0].Rows[0]["DairyDate"].ToString();
                        DM.UserName = ds.Tables[0].Rows[0]["User_fullnm"].ToString();
                        DM.DistName = ds.Tables[0].Rows[0]["DistName"].ToString();
                        DM.OfficeName = ds.Tables[0].Rows[0]["Branch"].ToString();
                    }

                }
                return View(DM);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Admin");
            }
        }

        [HttpPost]
        public JsonResult DeleteChallan(string DairyNo)
        {
            try
            {               
                if (DairyNo == null)
                {
                    var results1 = new
                    {
                        status = 0
                    };
                    return Json(results1);
                }                       
                int Outstatus = 0;
                objDM.DeleteDairyNo(DairyNo, Convert.ToInt32(Session["AdminId"].ToString()), out Outstatus);
                var  results = new
                {
                    status = Outstatus
                };
                return Json(results);
            }
            catch (Exception)
            {
                var results = new
                {
                    status = -2
                };
                return Json(results);
            }
        }
        #endregion Begin DM Panel

        //

        #region Begin Challan Panel
        public ActionResult ChallanList(DMModel asm, int? page)
        {
            try
            {
                // string DistAllow = "";
                if (Session["AdminId"] == null )
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (Session["DistAllow"].ToString() == "")
                {
                    return RedirectToAction("Index", "Admin");
                }

                string DistAllow = "";               
                // Dist Allowed
                
                if (ViewBag.DistAllow == null)
                { return RedirectToAction("Index", "Admin"); }
                else
                {
                    DistAllow = ViewBag.DistAllow;
                }
                if (ViewBag.DistUser == null)
                { ViewBag.MyDist = null; }
                else
                {
                    //    ViewBag.MyDist = ViewBag.DistUser;
                    ViewBag.MyDist = objCommon.GetDistE(); // All District
                }
                // End Dist Allowed
                int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                //feecat          
                ViewBag.feecat = objCommon.GetFeeCat();
                ViewBag.SelectedFeeCat = "0";
                // action
                var itemAction = new SelectList(new[] { new { ID = "1", Name = "Received" }, new { ID = "2", Name = "Pending" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";               
                   // itemSearchBy
                var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "Schlregid" }, new { ID = "2", Name = "Appno / Refno" },
                 new { ID = "3", Name = "ChallanId" }, new { ID = "4", Name = "Receiving No" }, new { ID = "5", Name = "Receiving Date" },}, "ID", "Name", 1);
                ViewBag.MySearchBy = itemSearchBy.ToList();
                ViewBag.SelectedSearchBy = "0";

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
                    string Search = string.Empty;                    
                    if (TempData["SearchChallanData"] != null)
                    {
                        Search += TempData["SearchChallanData"].ToString();
                        TempData["SelectedDist"] = ViewBag.SelectedDist = TempData["SelectedDist"];
                        TempData["SelAction"] = ViewBag.SelectedAction = TempData["SelAction"];
                        TempData["SelectedFeeCat"] = ViewBag.SelectedFeeCat = TempData["SelectedFeeCat"];
                        TempData["SelectedSearchBy"] = ViewBag.SelectedSearchBy = TempData["SelectedSearchBy"];
                        asm.StoreAllData = objDM.DMChallanList(1, Search, pageIndex, 20);
                        TempData["SearchChallanData"] = Search;

                        List<SelectListItem> allfee = objCommon.GetFeeCat();
                        foreach (var i in allfee)
                        {
                            if (i.Value.ToUpper() == ViewBag.SelectedFeeCat.ToUpper())
                            {
                                i.Selected = true;
                                break;
                            }
                        }
                        ViewBag.feecat = allfee;

                        if (asm.StoreAllData == null || asm.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.LastPageIndex = 0;
                            ViewBag.TotalCount = 0;
                            ViewBag.TotalCount1 = 0;
                            return View();
                        }
                        else
                        {

                            ViewBag.TotalCount = asm.StoreAllData.Tables[0].Rows.Count;
                            int count = Convert.ToInt32(asm.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                            ViewBag.TotalCount1 = count;
                            int tp = Convert.ToInt32(count);
                            int pn = tp / 20;
                            int cal = 20 * pn;
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
                        ViewBag.Message = "Record Not Found";
                        ViewBag.LastPageIndex = 0;
                        ViewBag.TotalCount = 0;
                        ViewBag.TotalCount1 = 0;
                        return View();
                    }
                    //ViewBag.SelectedDist = "";
                    //ViewBag.Message = "Record Not Found";
                    //ViewBag.LastPageIndex = 0;
                    //ViewBag.TotalCount = 0;
                    //ViewBag.TotalCount1 = 0;
                  //  return View();                    

                }
            }
            catch (Exception ex)
            {               
                return View();
            }
        }

        [HttpPost]
        public ActionResult ChallanList(DMModel asm, FormCollection frm, int? page, string submit)
        {
            try
            {
                if (submit != null)
                {
                    if (submit.ToUpper() == "RESET")
                    {
                        TempData.Clear();
                        TempData["SearchChallanData"] = null;
                        Session["DownloadPendingChallanData"] = null;
                        return RedirectToAction("ChallanList", "DM");
                    }

                }

                Session["DownloadPendingChallanData"] = null;
                //string DistAllow = "";
                if (Session["AdminId"] == null )
                {   
                    return RedirectToAction("Index", "Admin");
                }
                if (Session["DistAllow"].ToString() == "")
                {
                    return RedirectToAction("Index", "Admin");
                }

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
                    // ViewBag.MyDist = ViewBag.DistUser;
                    ViewBag.MyDist = objCommon.GetDistE(); // All District
                }
                // End Dist Allowed
                int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                // SchoolModels asm = new SchoolModels();
                if (ModelState.IsValid)
                {                    
                    //feecat          
                    ViewBag.feecat = objCommon.GetFeeCat(); 
                    ViewBag.SelectedFeeCat = "0";
                    // action
                    var itemAction = new SelectList(new[] { new { ID = "1", Name = "Received" }, new { ID = "2", Name = "Pending" }, }, "ID", "Name", 1);
                    ViewBag.MyAction = itemAction.ToList();
                    ViewBag.SelectedAction = "0";

                    // itemSearchBy
                    var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "Schlregid" }, new { ID = "2", Name = "Appno / Refno" },
                 new { ID = "3", Name = "ChallanId" }, new { ID = "4", Name = "Receiving No" }, new { ID = "5", Name = "Receiving Date" },}, "ID", "Name", 1);
                    ViewBag.MySearchBy = itemSearchBy.ToList();
                    ViewBag.SelectedSearchBy = "0";

                  
                    string Search = string.Empty;
                    Search = "id like '%' ";
                    if (frm["Dist1"] != "")
                    {
                        ViewBag.SelectedDist = frm["Dist1"];
                        TempData["SelectedDist"] = frm["Dist1"];
                        Search += " and dist=" + frm["Dist1"].ToString();
                    }

                    int SelAction = 0;
                    if (frm["SelAction"] != "")
                    {
                        TempData["SelAction"] = frm["SelAction"];
                        ViewBag.SelectedAction = frm["SelAction"];
                        SelAction = Convert.ToInt32(frm["SelAction"].ToString());
                        if (SelAction == 1)
                        {
                            Search += " and DMReceiveNo is not null ";
                        }
                        else if (SelAction == 2)
                        {
                            Search += " and  DMReceiveNo is  null ";
                        }
                    }


                    if (frm["feecat"] != "")
                    {
                        Search += " and FEECAT like '%" + frm["feecat"].ToString().Trim() + "%'";
                        ViewBag.SelectedFeeCat = frm["feecat"].ToString().Trim();
                        TempData["SelectedFeeCat"] = frm["feecat"].ToString().Trim();
                        List<SelectListItem> allfee = objCommon.GetFeeCat();
                        foreach (var i in allfee)
                        {
                            if (i.Value.ToUpper() == ViewBag.SelectedFeeCat.ToUpper())
                            {
                                i.Selected = true;
                                break;
                            }
                        }
                        ViewBag.feecat = allfee;                    
                    }

                    if (frm["SelBy"] != "")
                    {
                        ViewBag.SelectedSearchBy = frm["SelBy"];
                        TempData["SelectedSearchBy"] = frm["SelBy"];
                        int SelValueSch = Convert.ToInt32(frm["SelBy"].ToString());

                        if (frm["SearchString"] != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and Schlregid=" + frm["SearchString"].ToString(); }
                            else if (SelValueSch == 2)
                            { Search += " and Appno=" + frm["SearchString"].ToString(); }
                             else if (SelValueSch == 3)
                            { Search += " and a.ChallanId='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 4)
                            { Search += " and DMReceiveNo=" + frm["SearchString"].ToString() + ""; }
                            else if (SelValueSch == 5)
                            { Search += " and DMReceiveDate='" + frm["SearchString"].ToString() + "'"; }
                        }
                    }

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                    
                    TempData.Keep(); // to store search value for view
                    
                    //if (DistAllow != "")
                    //{
                    //    Search += " and DIST in (" + DistAllow + ")";
                    //}
                    TempData["SearchChallanData"] = Search;
                    asm.StoreAllData = objDM.DMChallanList(1,Search, pageIndex, 20);//DMChallanList

                    if (asm.StoreAllData == null || asm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                       
                        ViewBag.Message = "Record Not Found";
                        ViewBag.LastPageIndex = 0;
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        Session["DownloadPendingChallanData"] = asm.StoreAllData.Tables[0];
                        ViewBag.TotalCount = asm.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(asm.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        ViewBag.TotalCount1 = count;
                        int tp = Convert.ToInt32(count);
                        int pn = tp / 20;
                        int cal = 20 * pn;
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
                    return RedirectToAction("ChallanList", "DM");
                }
            }
            catch (Exception ex)
            {
               // oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        public ActionResult DownloadPendingChallanData()
        {
            try
            {
                if (Session["DownloadPendingChallanData"] == null)
                {
                    return RedirectToAction("ChallanList", "DM");
                }
                if (Request.QueryString["File"] == null)
                {
                    return RedirectToAction("ChallanList", "DM");
                }
                else
                {
                    DataTable dt = (DataTable)Session["DownloadPendingChallanData"];
                    if (dt.Rows.Count == 0)
                    {
                        return RedirectToAction("ChallanList", "DM");
                    }
                    else
                    {
                        if (dt.Rows.Count > 0)
                        {
                            if (dt.Columns.Contains("CHLNDATE"))
                            { dt.Columns.Remove("CHLNDATE"); }
                            if (dt.Columns.Contains("CHLNVDATE"))
                            { dt.Columns.Remove("CHLNVDATE"); }
                            if (dt.Columns.Contains("ReceivedBy"))
                            { dt.Columns.Remove("ReceivedBy"); }
                            if (dt.Columns.Contains("FEEMODE"))
                            { dt.Columns.Remove("FEEMODE"); }
                            if (dt.Columns.Contains("Mobile"))
                            { dt.Columns.Remove("Mobile"); }
                            if (dt.Columns.Contains("DOWNLDDATE"))
                            { dt.Columns.Remove("DOWNLDDATE"); }
                            if (dt.Columns.Contains("DOWNLDFLOT"))
                            { dt.Columns.Remove("DOWNLDFLOT"); }
                            if (dt.Columns.Contains("VERIFIED"))
                            { dt.Columns.Remove("VERIFIED"); }
                            if (dt.Columns.Contains("VERIFYDATE"))
                            { dt.Columns.Remove("VERIFYDATE"); }
                            if (dt.Columns.Contains("DOWNLDFLG"))
                            { dt.Columns.Remove("DOWNLDFLG"); }
                            if (dt.Columns.Contains("J_REF_NO"))
                            { dt.Columns.Remove("J_REF_NO"); }
                            if (dt.Columns.Contains("BRCODE"))
                            { dt.Columns.Remove("BRCODE"); }
                            if (dt.Columns.Contains("type"))
                            { dt.Columns.Remove("type"); }
                            if (dt.Columns.Contains("BANKCHRG"))
                            { dt.Columns.Remove("BANKCHRG"); }
                            if (dt.Columns.Contains("FEE"))
                            { dt.Columns.Remove("FEE"); }
                            if (dt.Columns.Contains("TOTFEE"))
                            { dt.Columns.Remove("TOTFEE"); }
                            if (dt.Columns.Contains("ACNO"))
                            { dt.Columns.Remove("ACNO"); }

                            string fileName1 = "DownloadPendingChallanData_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";  //103_230820162209_347
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
                }
                return RedirectToAction("ChallanList", "DM");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ChallanList", "DM");
            }

        }

        public JsonResult ReceiveChln(string receiveno, string receivedate, string remarks, string challanid)
        {
            try
            {
                string dee = "";
                string OutReceiveNo = "";
                // string OutReceiveNo = "CCE" + schl.ToString();
                string Search = string.Empty;
                DateTime date;
                if (DateTime.TryParseExact(receivedate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    objDM.ReceiveChln(Convert.ToInt32(Session["AdminId"].ToString()), challanid, receiveno, remarks, out OutReceiveNo, date);
                    if (OutReceiveNo == "0" || OutReceiveNo == "-1")
                    {
                        dee = "No";
                    } 
                    else if (OutReceiveNo == "-2")
                    {
                        dee = "DP";
                    }
                    else
                    {
                        dee = "Yes";
                    }                
                }
                return Json(new { sn = dee, Rcno = OutReceiveNo }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
               // oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return null;
            }
        }

        public ActionResult ChallanReceivedDetails(DMModel asm, int? page)
        {
            try
            {
                // string DistAllow = "";
                if (Session["AdminId"] == null )
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (Session["DistAllow"].ToString() == "")
                {
                    return RedirectToAction("Index", "Admin");
                }

                string DistAllow = "";
                // Dist Allowed
                int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());

                if (ViewBag.DistAllow == null)
                { return RedirectToAction("Index", "Admin"); }
                else
                { DistAllow = ViewBag.DistAllow; }
                if (ViewBag.DistUser == null)
                { ViewBag.MyDist = null; }
                else
                {
                   // ViewBag.MyDist = ViewBag.DistUser;
                    ViewBag.MyDist = objCommon.GetDistE(); // All District
                }
                // End Dist Allowed


                //feecat          
                ViewBag.feecat = objCommon.GetFeeCat();
                ViewBag.SelectedFeeCat = "0";  
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
                    string Search = string.Empty;                  

                    if (TempData["SearchChallanReceive"] != null)
                    {                      

                        Search += TempData["SearchChallanReceive"].ToString();
                        ViewBag.SelectedFeeCat = TempData["SelectedFeeCat"];
                        ViewBag.FromDate = TempData["FromDate"];
                        ViewBag.ToDate = TempData["ToDate"];
                        ViewBag.FromReceiving = TempData["FromReceiving"];
                        ViewBag.ToReceiving = TempData["ToReceiving"];
                        asm.StoreAllData = objDM.DMChallanList(2, Search, pageIndex, 20);
                        TempData["SearchChallanReceive"] = Search;
                        if (asm.StoreAllData == null || asm.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.LastPageIndex = 0;
                            ViewBag.TotalCount = 0;
                            ViewBag.TotalCount1 = 0;
                            return View();
                        }
                        else
                        {
                            ViewBag.TotalCount = asm.StoreAllData.Tables[0].Rows.Count;
                            int count = Convert.ToInt32(asm.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                            ViewBag.TotalCount1 = count;
                            int tp = Convert.ToInt32(count);
                            int pn = tp / 20;
                            int cal = 20 * pn;
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
                        ViewBag.Message = "Record Not Found";
                        ViewBag.LastPageIndex = 0;
                        ViewBag.TotalCount = 0;
                        ViewBag.TotalCount1 = 0;
                        return View();
                    }

                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult ChallanReceivedDetails(DMModel asm, FormCollection frm, int? page, string submit)
        {
            try
            {
                if (submit != null)
                {
                    if (submit.ToUpper() == "RESET")
                    {
                        TempData.Clear();
                        TempData["SearchChallanReceive"] = null;
                        Session["DownloadChallanReceivedData"] = null;
                        return RedirectToAction("ChallanReceivedDetails", "DM");
                    }

                }
                Session["DownloadChallanReceivedData"] = null;
                //string DistAllow = "";
                if (Session["AdminId"] == null )
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (Session["DistAllow"].ToString() == "")
                {
                    return RedirectToAction("Index", "Admin");
                }
                int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());

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
                    //ViewBag.MyDist = ViewBag.DistUser;
                    ViewBag.MyDist = objCommon.GetDistE(); // All District
                }
                // End Dist Allowed

                // SchoolModels asm = new SchoolModels();
                if (ModelState.IsValid)
                {
                    //feecat          
                    ViewBag.feecat = objCommon.GetFeeCat();
                    ViewBag.SelectedFeeCat = "0";
                  
                    string Search = string.Empty;
                    // Search = "id like '%' ";
                    Search = "id like '%' and ReceivedBy = " + Session["AdminId"].ToString().Trim() + "";

                    if (frm["feecat"] != "")
                    {
                        Search += " and FEECAT like '%" + frm["feecat"].ToString().Trim() + "%'";
                        ViewBag.SelectedFeeCat = frm["feecat"].ToString().Trim();
                        TempData["SelectedFeeCat"] = frm["feecat"].ToString().Trim();                       
                        List<SelectListItem> allfee = objCommon.GetFeeCat();
                        foreach (var i in allfee)
                        {
                            if (i.Value.ToUpper() == ViewBag.SelectedFeeCat.ToUpper())
                            {
                                i.Selected = true;
                                break;
                            }
                        }
                        ViewBag.feecat = allfee;
                    }

                    if (frm["FromDate"] != "")
                    {
                        ViewBag.FromDate = frm["FromDate"];
                        TempData["FromDate"] = frm["FromDate"];
                        Search += " and  format(DMReceiveDate,'dd/MM/yyyy') >='" + frm["FromDate"].ToString() + "'";
                    }
                    if (frm["ToDate"] != "")
                    {
                        ViewBag.ToDate = frm["ToDate"];
                        TempData["ToDate"] = frm["ToDate"];
                        Search += " and  format(DMReceiveDate,'dd/MM/yyyy') <='" + frm["ToDate"].ToString() + "'";
                    }
                    if (frm["FromReceiving"] != "")
                    {
                        ViewBag.FromReceiving = frm["FromReceiving"];
                        TempData["FromReceiving"] = frm["FromReceiving"];
                        Search += " and DMReceiveNo >= " + frm["FromReceiving"].ToString() + "";
                    }
                    if (frm["ToReceiving"] != "")
                    {
                        ViewBag.ToReceiving = frm["ToReceiving"];
                        TempData["ToReceiving"] = frm["ToReceiving"];
                        Search += " and DMReceiveNo <= " + frm["ToReceiving"].ToString() + "";
                    }

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;
                   
                    TempData.Keep(); // to store search value for view
                    //if (DistAllow != "")
                    //{
                    //    Search += " and DIST in (" + DistAllow + ")";
                    //}                   
                    TempData["SearchChallanReceive"] = Search;
                    asm.StoreAllData = objDM.DMChallanList(2,Search, pageIndex, 20);

                    if (asm.StoreAllData == null || asm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        Session["DownloadChallanReceivedData"] = null;
                        ViewBag.Message = "Record Not Found";
                        ViewBag.LastPageIndex = 0;
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        Session["DownloadChallanReceivedData"] = asm.StoreAllData.Tables[0];
                        ViewBag.TotalCount = asm.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.FEECODE1 = asm.StoreAllData.Tables[0].Rows[0]["FEECODE"].ToString();
                        ViewBag.FEECAT1 = asm.StoreAllData.Tables[0].Rows[0]["FEECAT"].ToString();
                        ViewBag.DISTNM1 = asm.StoreAllData.Tables[0].Rows[0]["DMDISTNM"].ToString();
                        ViewBag.DIST1 = asm.StoreAllData.Tables[0].Rows[0]["DMDIST"].ToString();
                        int count = Convert.ToInt32(asm.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        ViewBag.TotalCount1 = count;
                        int tp = Convert.ToInt32(count);
                        int pn = tp / 20;
                        int cal = 20 * pn;
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
                // oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        public ActionResult DownloadChallanReceivedData()
        {
            try
            {

                if (Session["DownloadChallanReceivedData"] == null)
                {
                    return RedirectToAction("ChallanReceivedDetails", "DM");
                }
                if (Request.QueryString["File"] == null)
                {
                    return RedirectToAction("ChallanReceivedDetails", "DM");
                }
                else
                {
                    DataTable dt = (DataTable)Session["DownloadChallanReceivedData"];
                    if (dt.Rows.Count == 0)
                    {
                        return RedirectToAction("ChallanReceivedDetails", "DM");
                    }
                    else
                    {
                        if (dt.Rows.Count > 0)
                        {
                            if (dt.Columns.Contains("CHLNDATE"))
                            { dt.Columns.Remove("CHLNDATE");}
                            if (dt.Columns.Contains("CHLNVDATE"))
                            { dt.Columns.Remove("CHLNVDATE"); }
                            if (dt.Columns.Contains("ReceivedBy"))
                            { dt.Columns.Remove("ReceivedBy"); }
                            if (dt.Columns.Contains("FEEMODE"))
                            { dt.Columns.Remove("FEEMODE"); }
                            if (dt.Columns.Contains("Mobile"))
                            { dt.Columns.Remove("Mobile"); }
                            if (dt.Columns.Contains("DOWNLDDATE"))
                            { dt.Columns.Remove("DOWNLDDATE"); }
                            if (dt.Columns.Contains("DOWNLDFLOT"))
                            { dt.Columns.Remove("DOWNLDFLOT"); }                         
                            if (dt.Columns.Contains("VERIFIED"))
                            { dt.Columns.Remove("VERIFIED"); }
                            if (dt.Columns.Contains("VERIFYDATE"))
                            { dt.Columns.Remove("VERIFYDATE"); }
                            if (dt.Columns.Contains("DOWNLDFLG"))
                            { dt.Columns.Remove("DOWNLDFLG"); }
                            if (dt.Columns.Contains("J_REF_NO"))
                            { dt.Columns.Remove("J_REF_NO"); }
                            if (dt.Columns.Contains("BRCODE"))
                            { dt.Columns.Remove("BRCODE"); }    
                            if (dt.Columns.Contains("type"))
                            { dt.Columns.Remove("type"); }                          
                            if (dt.Columns.Contains("BANKCHRG"))
                            { dt.Columns.Remove("BANKCHRG"); }
                            if (dt.Columns.Contains("FEE"))
                            { dt.Columns.Remove("FEE"); }
                            if (dt.Columns.Contains("TOTFEE"))
                            { dt.Columns.Remove("TOTFEE"); }
                            if (dt.Columns.Contains("ACNO"))
                            { dt.Columns.Remove("ACNO"); }                      

                            string fileName1 = "DownloadChallanReceivedData_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";  //103_230820162209_347
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
                }
                return RedirectToAction("ChallanReceivedDetails", "DM");
            }
            catch (Exception ex)
            {
                return RedirectToAction("ChallanReceivedDetails", "DM");
            }

        }

        public ActionResult CancelReceiving(string id)
        {
            try
            {               
                int OutReceiveNo = 0;
                if (id != "")
                {
                    objDM.DMCancelReceiving(Convert.ToInt32(Session["AdminId"].ToString()), id, out OutReceiveNo);
                    ViewData["CancelReceiveNo"] = OutReceiveNo;
                }
                else
                {
                    ViewData["CancelReceiveNo"] = 0;
                }               
            }
            catch (Exception ex)
            {
                ViewData["CancelReceiveNo"] = 0;
            }
            return RedirectToAction("ChallanList", "DM");
        }
        #endregion Begin Challan Panel

        

        #region Begin BulkChallanEntry
        public ActionResult BulkChallanEntry()
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult BulkChallanEntry(AdminModels AM)
        {
            try
            {
                if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    //HttpContext.Session["AdminType"]
                    string AdminType = Session["AdminType"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
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
                        string fileName1 = "BulkChallanEntry_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

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

                            string[] array = ds.Tables[0].Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray();
                            string[] arrayChln = ds.Tables[0].Rows.OfType<DataRow>().Select(k => k[1].ToString()).ToArray();
                           // bool CheckStatus = AbstractLayer.StaticDB.CheckArrayDuplicates(array);
                            bool CheckChln = AbstractLayer.StaticDB.CheckArrayDuplicates(arrayChln);
                            //if (CheckStatus == true)
                            //{
                            //    ViewData["Result"] = "10";
                            //    ViewBag.Message = "Duplicate Receiving Number";
                            //    return View();
                            //}

                            if (CheckChln == true)
                            {
                                ViewData["Result"] = "11";
                                ViewBag.Message = "Duplicate Challan Number";
                                return View();
                            }
                            DataTable dtexport = new DataTable();
                            string CheckMis = objDM.CheckBulkChallanExcelExport(ds, AdminId, out dtexport);
                            if (CheckMis == "")
                            {
                                DataTable dt1 = ds.Tables[0];
                                if (dt1.Columns.Contains("STATUS"))
                                {
                                    dt1.Columns.Remove("STATUS");
                                }
                                string Result1 = "";
                                int OutStatus = 0;

                                dt1.AcceptChanges();
                                DataTable dtResult = objDM.BulkChallanEntry(dt1, AdminId, out OutStatus);// OutStatus mobile
                                if (OutStatus > 0)
                                {
                                    ViewBag.Message = "File Uploaded Successfully";
                                    ViewData["Result"] = "1";
                                }
                                else
                                {
                                    ViewBag.Message = "File Not Uploaded Successfully";
                                    ViewData["Result"] = "0";
                                }
                                return View();
                            }
                            else
                            {
                                if (dtexport != null)
                                {
                                    ExportDataFromDataTable(dtexport, Session["UserName"].ToString());
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
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }
            return View();
        }

        public ActionResult ExportDataFromDataTable(DataTable dt, string UserName)
        {
            try
            {
                if (dt.Rows.Count == 0)
                {
                    return RedirectToAction("BulkChallanEntry", "DM");
                }
                else
                {
                    if (dt.Rows.Count > 0)
                    {
                        string fileName1 = "BulkChallanEntry_ERROR_" + UserName + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";  //103_230820162209_347
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

                return RedirectToAction("BulkChallanEntry", "DM");
            }
            catch (Exception ex)
            {
                return RedirectToAction("BulkChallanEntry", "DM");
            }

        }

        #endregion  ExamBulkChallanEntry


        #region Book Assessment 

        public ActionResult BookAssessmentFormList(DMModel asm, int? page)
        {
            try
            {
                // string DistAllow = "";
                if (Session["AdminId"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (Session["DistAllow"].ToString() == "")
                {
                    return RedirectToAction("Index", "Admin");
                }

                string DistAllow = "";
                // Dist Allowed

                if (ViewBag.DistAllow == null)
                { return RedirectToAction("Index", "Admin"); }
                else
                {
                    DistAllow = ViewBag.DistAllow;
                }
                if (ViewBag.DistUser == null)
                { ViewBag.MyDist = null; }
                else
                {
                    //    ViewBag.MyDist = ViewBag.DistUser;
                    ViewBag.MyDist = objCommon.GetDistE(); // All District
                }
                // End Dist Allowed
                int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());                            
              
                // itemSearchBy
                var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "School Code" },}, "ID", "Name", 1);
                ViewBag.MySearchBy = itemSearchBy.ToList();
                ViewBag.SelectedSearchBy = "0";

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
                    string Search = string.Empty;
                    if (TempData["SearchBookAssessmentFormList"] != null)
                    {
                        Search += TempData["SearchBookAssessmentFormList"].ToString();
                        TempData["SelectedDist"] = ViewBag.SelectedDist = TempData["SelectedDist"];
                        TempData["SelectedSearchBy"] = ViewBag.SelectedSearchBy = TempData["SelectedSearchBy"];
                        asm.StoreAllData = objDM.BookAssessmentFormList(1, Search, pageIndex, 20);
                        TempData["SearchBookAssessmentFormList"] = Search;

                        //List<SelectListItem> allfee = objCommon.GetFeeCat();
                        //foreach (var i in allfee)
                        //{
                        //    if (i.Value.ToUpper() == ViewBag.SelectedFeeCat.ToUpper())
                        //    {
                        //        i.Selected = true;
                        //        break;
                        //    }
                        //}
                        //ViewBag.feecat = allfee;

                        if (asm.StoreAllData == null || asm.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.LastPageIndex = 0;
                            ViewBag.TotalCount = 0;
                            ViewBag.TotalCount1 = 0;
                            return View();
                        }
                        else
                        {

                            ViewBag.TotalCount = asm.StoreAllData.Tables[0].Rows.Count;
                            int count = Convert.ToInt32(asm.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                            ViewBag.TotalCount1 = count;
                            int tp = Convert.ToInt32(count);
                            int pn = tp / 20;
                            int cal = 20 * pn;
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
                        ViewBag.Message = "Record Not Found";
                        ViewBag.LastPageIndex = 0;
                        ViewBag.TotalCount = 0;
                        ViewBag.TotalCount1 = 0;
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult BookAssessmentFormList(DMModel asm, FormCollection frm, int? page, string submit)
        {
            try
            {
                if (submit != null)
                {
                    if (submit.ToUpper() == "RESET")
                    {
                        TempData.Clear();
                        TempData["SearchChallanData"] = null;
                       // Session["DownloadPendingChallanData"] = null;
                        return RedirectToAction("ChallanList", "DM");
                    }

                }

               // Session["DownloadPendingChallanData"] = null;
                //string DistAllow = "";
                if (Session["AdminId"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (Session["DistAllow"].ToString() == "")
                {
                    return RedirectToAction("Index", "Admin");
                }

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
                    // ViewBag.MyDist = ViewBag.DistUser;
                    ViewBag.MyDist = objCommon.GetDistE(); // All District
                }
                // End Dist Allowed
                int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                // SchoolModels asm = new SchoolModels();
                if (ModelState.IsValid)
                {

                    // itemSearchBy
                    var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "School Code" }, }, "ID", "Name", 1);
                    ViewBag.MySearchBy = itemSearchBy.ToList();
                    ViewBag.SelectedSearchBy = "0";

                    string Search = string.Empty;
                    Search = "Schl like '%' ";
                    if (frm["Dist1"] != "")
                    {
                        ViewBag.SelectedDist = frm["Dist1"];
                        TempData["SelectedDist"] = frm["Dist1"];
                        Search += " and Dist=" + frm["Dist1"].ToString();
                    }             

                    if (frm["SelBy"] != "")
                    {
                        ViewBag.SelectedSearchBy = frm["SelBy"];
                        TempData["SelectedSearchBy"] = frm["SelBy"];
                        int SelValueSch = Convert.ToInt32(frm["SelBy"].ToString());

                        if (frm["SearchString"] != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and Schl='" + frm["SearchString"].ToString() + "'"; }                           
                        }
                    }

                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;

                    TempData.Keep(); // to store search value for view

                    if (DistAllow != "")
                    {
                        Search += " and Dist in (" + DistAllow + ")";
                    }
                    TempData["SearchBookAssessmentFormList"] = Search;

                   
                    asm.StoreAllData = objDM.BookAssessmentFormList(1, Search, pageIndex, 20);//DMChallanList

                    if (asm.StoreAllData == null || asm.StoreAllData.Tables[0].Rows.Count == 0)
                    {

                        ViewBag.Message = "Record Not Found";
                        ViewBag.LastPageIndex = 0;
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = asm.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(asm.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        ViewBag.TotalCount1 = count;
                        int tp = Convert.ToInt32(count);
                        int pn = tp / 20;
                        int cal = 20 * pn;
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
                    return RedirectToAction("BookAssessmentFormList", "DM");
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        public ActionResult ViewBookAssessmentFormList(string Dist, string Schl)
        {
            try
            {
                
                BookAssessmentForm rft = new BookAssessmentForm();  
                if (Session["AdminId"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                if (Session["DistAllow"].ToString() == "")
                {
                    return RedirectToAction("Index", "Admin");
                }
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
                    // ViewBag.MyDist = ViewBag.DistUser;
                    ViewBag.MyDist = objCommon.GetDistE(); // All District
                }
                // End Dist Allowed
                int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                
                string Search = "";
                string OutError = "0";
                Search = "SCHL like '%%'";
                if (!string.IsNullOrEmpty(Dist))
                { Search += " and Dist='" + Dist + "' "; }
                if (!string.IsNullOrEmpty(Schl))
                { Search += " and Schl='" + Schl + "' "; }

               
                DataSet dsBD = objDM.ViewBookAssessmentFormList(8, Search, Schl, out OutError);
                rft.StoreAllData = dsBD;
                if (rft.StoreAllData == null || rft.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = rft.StoreAllData.Tables[0].Rows.Count;
                }
                return View(rft);
            }
            catch (Exception)
            {
                return View();
            }
        }


        #endregion Book Assessment 


        #region View SupplyofBooks
        AbstractLayer.BookSupplyDB objDBSupply = new AbstractLayer.BookSupplyDB();
        private PrinterModel PM = new PrinterModel();

        public ActionResult ViewSupplyofBooks(string id)
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            if (Session["DistAllow"].ToString() == "")
            {
                return RedirectToAction("Index", "Admin");
            }

            //string DistAllow = "";
            //// Dist Allowed

            //if (ViewBag.DistAllow == null)
            //{ return RedirectToAction("Index", "Admin"); }
            //else
            //{
            //    DistAllow = ViewBag.DistAllow;
            //}
            //if (ViewBag.DistUser == null)
            //{ ViewBag.MyDist = null; }
            //else
            //{
            //    //    ViewBag.MyDist = ViewBag.DistUser;
            //    ViewBag.MyDist = objCommon.GetDistE(); // All District
            //}
            int OutStatus = 0;
            string printerId = Session["AdminUser"].ToString();
            int supplylot = 0;

            if (string.IsNullOrEmpty(id))
            {

                ViewBag.Supplylot = supplylot = 0;
                DataSet result = objDBSupply.ViewSupplyofBooks(3, printerId, supplylot, out OutStatus); // 3 for Depot
                if (result == null || result.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCount = 0;
                }
                else if (result.Tables[0].Rows.Count > 0)
                {
                    PM.StoreAllData = result;
                    ViewBag.TotalCount = PM.StoreAllData.Tables[0].Rows.Count;
                }
            }
            else
            {
                ViewBag.Supplylot = supplylot = Convert.ToInt32(id);
                DataSet result = objDBSupply.ViewSupplyofBooks(4, printerId, supplylot, out OutStatus); // 3 for Depot particular supply
                if (result == null || result.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCount = 0;
                }
                else if (result.Tables[0].Rows.Count > 0)
                {
                    PM.StoreAllData = result;
                    ViewBag.TotalCount = PM.StoreAllData.Tables[0].Rows.Count;
                }
            }
            return View(PM);
        }

        public JsonResult ReceiveSupplyBookDetails(string remarks, string NORB, string SupplyId, string Type)
        {
            try
            {
                string dee = "";
                string outstatus = "";
                int AdminId = Convert.ToInt32(Session["AdminId"]);
                objDBSupply.ReceiveSupplyBookDetails(remarks, NORB, SupplyId, out outstatus, AdminId, Convert.ToInt32(Type));//ChallanDetailsCancelSP
               
                dee = outstatus;
                return Json(new { sn = dee, chid = outstatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        #endregion View SupplyofBooks

    }
}