using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSEBONLINE.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Web.Routing;
using PSEBONLINE.Filters;
using CCA.Util;
using System.Collections.Specialized;
using Newtonsoft.Json;
using System.Net;
using DocumentFormat.OpenXml.Bibliography;

namespace PSEBONLINE.Controllers
{
    public class ChallanController : Controller
    {
        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        private string CommonCon = "myDBConnection";
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
                             new RouteValueDictionary(new { controller = "Admin", action = "PageNotAuthorized" }));
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

        #region ShiftChallanDetails
        public ActionResult ShiftChallanDetails(ShiftChallanDetails obj)
        {

            if (Session["AdminId"] == null || Session["AdminId"].ToString() == "")
            {
                return RedirectToAction("Index", "Admin");
            }
            return View(obj);
        }

        [HttpPost]
        public ActionResult ShiftChallanDetails(int? page, ShiftChallanDetails scd, FormCollection frm, string cmd)
        {

            if (Session["AdminId"] == null || Session["AdminId"].ToString() == "")
            {
                return RedirectToAction("Index", "Admin");
            }
            try
            {
                scd.AdminId = Convert.ToInt32(Session["AdminId"]);

                if (cmd != null)
                {
                    scd.ActionType = 0;
                    string obj = Guid.NewGuid().ToString().Substring(0, 5);
                    if (cmd.ToUpper().Contains("SHIFT"))
                    {
                        scd.ActionType = 1;
                        scd.ShiftFile = scd.CorrectChallan + '_' + obj + '_' + DateTime.Now.ToString("ddMMyyyy") + (System.IO.Path.GetExtension(scd.file.FileName).ToLower());
                        string OutStatus = string.Empty;
                        int OutSID = 0;
                        DataSet getData = new AbstractLayer.AdminDB().ShiftChallanDetails(scd, out OutStatus, out OutSID);
                        scd.StoreAllData = getData;
                        if (OutStatus == "1")
                        {
                            ViewBag.ShiftId = scd.ShiftId = OutSID;
                            ViewData["result"] = "1";
                            ViewBag.CorrectChallan = scd.CorrectChallan;
                            if (scd.file != null)
                            {
                                var path = Path.Combine(Server.MapPath("~/Upload/" + "ShiftChallanDetails"), scd.ShiftFile);
                                string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + "/ShiftChallanDetails"));
                                if (!Directory.Exists(FilepathExist))
                                {
                                    Directory.CreateDirectory(FilepathExist);
                                }
                                scd.file.SaveAs(path);
                            }
                        }
                        else
                        {
                            ViewData["result"] = "0";
                        }
                        return View(scd);
                    }
                    else
                    {

                        string OutStatus = string.Empty;
                        int OutSID = 0;
                        DataSet getData = new AbstractLayer.AdminDB().ShiftChallanDetails(scd, out OutStatus, out OutSID);
                        scd.StoreAllData = getData;
                        if (scd.StoreAllData == null || scd.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                            return View();
                        }
                        else
                        {
                            ViewBag.TotalCount = scd.StoreAllData.Tables[0].Rows.Count;
                            if (scd.StoreAllData.Tables[0].Rows.Count > 0)
                            {
                                DataTable dt = scd.StoreAllData.Tables[0];
                                List<ShiftChallan> ci = new List<ShiftChallan>();
                                ci = (from DataRow row in dt.Rows

                                      select new ShiftChallan
                                      {
                                          challanid = row["CHALLANID"].ToString(),
                                          CHLNDATE = row["CHLNDATE"].ToString(),
                                          CHLNVDATE = row["CHLNVDATE"].ToString(),
                                          FEECODE = row["FEECODE"].ToString(),
                                          FEECAT = row["FEECAT"].ToString(),
                                          BCODE = row["BCODE"].ToString(),
                                          BANK = row["BANK"].ToString(),
                                          DIST = row["DIST"].ToString(),
                                          DISTNM = row["DISTNM"].ToString(),
                                          FEE = Convert.ToInt32(row["FEE"].ToString()),
                                          BANKCHRG = Convert.ToInt32(row["BANKCHRG"].ToString()),
                                          TOTFEE = Convert.ToInt32(row["TOTFEE"].ToString()),
                                          SCHLREGID = row["SCHLREGID"].ToString(),
                                          SCHLCANDNM = row["SCHLCANDNM"].ToString(),
                                          BRCODE = row["BRCODE"].ToString(),
                                          BRANCH = row["BRANCH"].ToString(),
                                          J_REF_NO = row["J_REF_NO"].ToString(),
                                          DEPOSITDT = row["DEPOSITDT"].ToString(),
                                          VERIFIED = row["VERIFIED"].ToString(),
                                          VERIFYDATE = row["VERIFYDATE"].ToString(),
                                          APPNO = row["APPNO"].ToString(),
                                          addfee = Convert.ToInt32(row["addfee"].ToString()),
                                          latefee = Convert.ToInt32(row["latefee"].ToString()),
                                          prosfee = Convert.ToInt32(row["prosfee"].ToString()),
                                          addsubfee = Convert.ToInt32(row["addsubfee"].ToString()),
                                          add_sub_count = Convert.ToInt32(row["add_sub_count"].ToString()),
                                          regfee = Convert.ToInt32(row["regfee"].ToString()),
                                          LOT = row["LOT"].ToString(),
                                          Mobile = row["Mobile"].ToString(),
                                          RP = row["RP"].ToString(),
                                          StudentList = row["StudentList"].ToString(),
                                      }).ToList();

                                scd.challanList = ci;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View(scd);
        }


        public ActionResult ShiftChallanDetailsSlip(string id, ShiftChallanDetails scd)
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            else if (id == null)
            { return RedirectToAction("ShiftChallanDetails", "Challan"); }
            else
            {
                try
                {
                    ViewBag.Id = id;
                    string OutStatus = string.Empty;
                    // Search = "a.id like '%' and a.id=" + id + "  ";                  
                    //Search = "a.id like '%' and a.Dairyno='" + id + "'";
                    string Sid = id.ToString();
                    scd.ActionType = 2;
                    scd.WrongChallan = scd.CorrectChallan = "";
                    scd.ActionType = 2;
                    scd.ShiftId = Convert.ToInt32(Sid);
                    int OutSID = 0;
                    scd.StoreAllData = new AbstractLayer.AdminDB().ShiftChallanDetails(scd, out OutStatus, out OutSID);
                    if (scd.StoreAllData == null || scd.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.LastPageIndex = 0;
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = scd.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.ShiftDate = scd.StoreAllData.Tables[0].Rows[0]["ShiftDate"].ToString();
                        if (scd.StoreAllData.Tables[0].Rows.Count > 0)
                        {
                            DataTable dt = scd.StoreAllData.Tables[0];
                            List<ShiftChallan> ci = new List<ShiftChallan>();
                            ci = (from DataRow row in dt.Rows

                                  select new ShiftChallan
                                  {
                                      challanid = row["CHALLANID"].ToString(),
                                      CHLNDATE = row["CHLNDATE"].ToString(),
                                      CHLNVDATE = row["CHLNVDATE"].ToString(),
                                      FEECODE = row["FEECODE"].ToString(),
                                      FEECAT = row["FEECAT"].ToString(),
                                      BCODE = row["BCODE"].ToString(),
                                      BANK = row["BANK"].ToString(),
                                      DIST = row["DIST"].ToString(),
                                      DISTNM = row["DISTNM"].ToString(),
                                      FEE = Convert.ToInt32(row["FEE"].ToString()),
                                      BANKCHRG = Convert.ToInt32(row["BANKCHRG"].ToString()),
                                      TOTFEE = Convert.ToInt32(row["TOTFEE"].ToString()),
                                      SCHLREGID = row["SCHLREGID"].ToString(),
                                      SCHLCANDNM = row["SCHLCANDNM"].ToString(),
                                      BRCODE = row["BRCODE"].ToString(),
                                      BRANCH = row["BRANCH"].ToString(),
                                      J_REF_NO = row["J_REF_NO"].ToString(),
                                      DEPOSITDT = row["DEPOSITDT"].ToString(),
                                      VERIFIED = row["VERIFIED"].ToString(),
                                      VERIFYDATE = row["VERIFYDATE"].ToString(),
                                      APPNO = row["APPNO"].ToString(),
                                      addfee = Convert.ToInt32(row["addfee"].ToString()),
                                      latefee = Convert.ToInt32(row["latefee"].ToString()),
                                      prosfee = Convert.ToInt32(row["prosfee"].ToString()),
                                      addsubfee = Convert.ToInt32(row["addsubfee"].ToString()),
                                      add_sub_count = Convert.ToInt32(row["add_sub_count"].ToString()),
                                      regfee = Convert.ToInt32(row["regfee"].ToString()),
                                      LumsumFine = Convert.ToInt32(row["LumsumFine"].ToString()),
                                      LOT = row["LOT"].ToString(),
                                      Mobile = row["Mobile"].ToString(),
                                      RP = row["RP"].ToString(),
                                      StudentList = row["StudentList"].ToString(),
                                      BoardBranch = row["BoardBranch"].ToString(),
                                  }).ToList();

                            scd.challanList = ci;
                        }
                        return View(scd);
                    }
                }
                catch (Exception)
                {
                    return RedirectToAction("ShiftChallanDetails", "Challan");
                }
            }
        }



        public ActionResult ViewShiftChallanDetails(ShiftChallanDetails scd, int? page)
        {
            try
            {
                if (Session["AdminId"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                int AdminId = ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "By ID" }, new { ID = "2", Name = "Correct Challan" },
                    new { ID = "3", Name = "By Date" },}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                string Search = string.Empty;
                if (TempData["SearchShiftChallanDetails"] != null)
                {
                    Search += TempData["SearchShiftChallanDetails"].ToString();
                    TempData["SelectedItem"] = ViewBag.SelectedItem = TempData["SelectedItem"];
                    scd.StoreAllData = new AbstractLayer.AdminDB().ViewShiftChallanDetails(0, 1, Search, pageIndex, 20);
                    TempData["SearchShiftChallanDetails"] = Search;
                    if (scd.StoreAllData == null || scd.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.LastPageIndex = 0;
                        ViewBag.TotalCount = 0;
                        ViewBag.TotalCount1 = 0;
                        return View();
                    }
                    else
                    {

                        ViewBag.TotalCount = scd.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(scd.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        ViewBag.TotalCount1 = count;
                        int tp = Convert.ToInt32(count);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        return View(scd);
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
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult ViewShiftChallanDetails(ShiftChallanDetails scd, FormCollection frm, int? page, string submit)
        {
            try
            {
                string ForwardList = string.Empty;
                if (Session["AdminId"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                int AdminId = ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "By ID" }, new { ID = "2", Name = "Correct Challan" },
                    new { ID = "3", Name = "By Date" },}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();

                if (submit != null)
                {
                    if (submit.ToUpper() == "RESET")
                    {
                        TempData.Clear();
                        TempData["SearchShiftChallanDetails"] = null;
                        return RedirectToAction("ViewShiftChallanDetails", "Challan");
                    }
                }
                string Search = string.Empty;
                Search = "Sid like '%'";
                if (frm["SelList"] != "")
                {
                    ViewBag.SelectedItem = frm["SelList"];
                    int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());
                    if (frm["SearchString"] != "")
                    {
                        ViewBag.SearchString = frm["SearchString"].ToString();
                        if (SelValueSch == 1)
                        { Search += " and Sid=" + frm["SearchString"].ToString(); }
                        else if (SelValueSch == 2)
                        { Search += " and CorrectChallan='" + frm["SearchString"].ToString() + "'"; }
                        else if (SelValueSch == 3)
                        { Search += " and format(CreatedDate,'dd/MM/yyyy') ='" + frm["SearchString"].ToString() + "'"; }
                    }
                }

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                TempData.Keep();
                TempData["SearchShiftChallanDetails"] = Search;
                scd.StoreAllData = new AbstractLayer.AdminDB().ViewShiftChallanDetails(0, 1, Search, pageIndex, 20);
                if (scd.StoreAllData == null || scd.StoreAllData.Tables[0].Rows.Count == 0)
                {

                    ViewBag.Message = "Record Not Found";
                    ViewBag.LastPageIndex = 0;
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = scd.StoreAllData.Tables[0].Rows.Count;
                    int count = Convert.ToInt32(scd.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                    ViewBag.TotalCount1 = count;
                    int tp = Convert.ToInt32(count);
                    int pn = tp / 20;
                    int cal = 20 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;

                    return View(scd);
                }

            }
            catch (Exception ex)
            {
                // oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        #endregion ShiftChallanDetails


        // GET: Challan

        #region ChallanDetails
        public ActionResult ChallanDetails(int? page)
        {
            try
            {
                if (Session["AdminId"] == null || Session["AdminId"].ToString() == "")
                {
                    return RedirectToAction("Index", "Admin");
                }

                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsPrint = 1; ViewBag.IsView = 1; ViewBag.IsRegenerate = 1; ViewBag.IsCancel = 1; ViewBag.IsVerify = 1; }
                else
                {

                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        //ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(61)).Count();                        
                        ViewBag.IsPrint = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("HOME/GENERATECHALLAAN")).Count();
                        ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/CHALLANDETAILS")).Count();
                        ViewBag.IsRegenerate = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/CHALLANDETAILSREGERATE")).Count();
                        ViewBag.IsCancel = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/CHALLANDETAILSCANCEL")).Count();
                        ViewBag.IsVerify = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/VERIFYPAYMENTINCHALLANDETAIL")).Count();

                    }
                }
                #endregion Action Assign Method

                ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");

                // By Rohit  -- select bank from database
                string BankAllow = string.Empty;
                DataSet dsBank = objCommon.GetAdminDetailsById(Convert.ToInt32(Session["AdminId"]), Convert.ToInt32(Session["Session"].ToString().Substring(0, 4)));
                if (dsBank != null)
                {
                    if (dsBank.Tables[3].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[3].Rows)
                        {
                            BankAllow += @dr["Bcode"].ToString().Trim() + ",";
                            itemBank.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }
                }
                // End 

                var itemsch1 = new SelectList(new[] { new { ID = "CHALLANID", Name = "Challan ID" }, new { ID = "APPNO", Name = "Appno/RefNo" }, new { ID = "SCHLREGID", Name = "SCHLREGID" }, new { ID = "LOT", Name = "downloaded lot" }, new { ID = "J_REF_NO", Name = "Journal No" },
                new { ID = "RefundStatus", Name = "Refund Transactions" }}, "ID", "Name", 1);

                ViewBag.feecat = objCommon.GetFeeCat();

                ViewBag.MySch1 = itemsch1;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Challan obj = new Challan();
                string session = null;
                string schl = null;
                AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
                string Search = string.Empty;

                if (BankAllow != "")
                {
                    BankAllow = BankAllow.Remove(BankAllow.LastIndexOf(","), 1);
                    // Search += " and a.Bcode in (" + BankAllow.ToString().Trim() + ")";
                }

                if (TempData["GetChallanDetailsSearch"] != null)
                {
                    Search += TempData["GetChallanDetailsSearch"].ToString();
                    TempData["SelectedItem"] = ViewBag.SelectedItem = TempData["SelectedItem"];
                    TempData["feecatselect"] = ViewBag.feecatselect = TempData["feecatselect"];
                    TempData["srhfld"] = ViewBag.srhfld = TempData["srhfld"];
                    TempData["ChallanDetailsSearchString"] = ViewBag.Searchstring = TempData["ChallanDetailsSearchString"];
                    TempData["GetChallanDetailsSearch"] = Search;
                    obj.StoreAllData = objDB.GetChallanDetails(Search, session, pageIndex);//GetChallanDetails               

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
                else
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    ViewBag.TotalCount1 = 0;
                }
                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        [HttpPost]
        public ActionResult ChallanDetails(int? page, Challan obj, FormCollection frm, string cmd, string BankName, string srhfld, string SearchString, string feecat1)
        {
            if (Session["AdminId"] == null || Session["AdminId"].ToString() == "")
            {
                return RedirectToAction("Index", "Admin");
            }
            try
            {
                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsPrint = 1; ViewBag.IsView = 1; ViewBag.IsRegenerate = 1; ViewBag.IsCancel = 1; ViewBag.IsVerify = 1; }
                else
                {

                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        //ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(61)).Count();                        
                        ViewBag.IsPrint = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("HOME/GENERATECHALLAAN")).Count();
                        ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/CHALLANDETAILS")).Count();
                        ViewBag.IsRegenerate = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/CHALLANDETAILSREGERATE")).Count();
                        ViewBag.IsCancel = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/CHALLANDETAILSCANCEL")).Count();
                        ViewBag.IsVerify = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/VERIFYPAYMENTINCHALLANDETAIL")).Count();

                    }
                }
                #endregion Action Assign Method
                ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");

                // By Rohit  -- select bank from database
                string BankAllow = string.Empty;
                DataSet dsBank = objCommon.GetAdminDetailsById(Convert.ToInt32(Session["AdminId"]), Convert.ToInt32(Session["Session"].ToString().Substring(0, 4)));
                if (dsBank != null)
                {
                    if (dsBank.Tables[3].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[3].Rows)
                        {
                            BankAllow += @dr["Bcode"].ToString().Trim() + ",";
                            itemBank.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }
                }
                // End 

                //var itemsch = new SelectList(new[] { new { ID = "Punjab National Bank", Name = "PNB" }, new { ID = "State Bank Of Patiala", Name = "SBOP" }, }, "ID", "Name", 1);
                //ViewBag.MySch = itemsch.ToList();


                var itemsch1 = new SelectList(new[] { new { ID = "CHALLANID", Name = "Challan ID" }, new { ID = "APPNO", Name = "Appno/RefNo" }, new { ID = "SCHLREGID", Name = "SCHLREGID" }, new { ID = "LOT", Name = "downloaded lot" }, new { ID = "J_REF_NO", Name = "Journal No" },
                new { ID = "RefundStatus", Name = "Refund Transactions" }}, "ID", "Name", 1);
                ViewBag.MySch1 = itemsch1.ToList();
                ViewBag.feecat = objCommon.GetFeeCat();
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
                Challan rm = new Challan();

                string Search = "";
                if (cmd == "Search")
                {
                    string dee = "%";
                    Search = "SCHLREGID like '" + dee + "'";
                    if (BankName != "")
                    {
                        Search += " and a.Bcode='" + BankName.ToString().Trim() + "'";
                        ViewBag.SelectedItem = BankName;
                    }
                    if (feecat1 != "")
                    {
                        Search += " and a.FEECAT like '%" + feecat1.ToString().Trim() + "%'";
                        ViewBag.feecatselect = feecat1;
                    }
                    if (srhfld != "" && SearchString != "")
                    {
                        ViewBag.srhfld = srhfld;
                        if (srhfld == "CHALLANID")
                            Search += " and a.CHALLANID='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "APPNO")
                            Search += " and a.APPNO='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "SCHLREGID")
                            Search += " and a.SCHLREGID='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "LOT")
                            Search += " and a.DOWNLDFLOT='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "J_REF_NO")
                            Search += " and a.J_REF_NO='" + SearchString.ToString().Trim() + "'";
                        //else if (srhfld.ToLower() == "RefundStatus".ToLower())
                        //    Search += " and a.Verified=2 ";
                    }

                    if (srhfld != "")
                    {
                        if (srhfld.ToLower() == "RefundStatus".ToLower())
                        {
                            Search += " and a.Verified=2 ";
                        }
                    }

                    TempData["GetChallanDetailsSearch"] = Search;
                    TempData["SelectedItem"] = BankName;
                    TempData["feecatselect"] = feecat1;
                    TempData["srhfld"] = srhfld;
                    TempData["ChallanDetailsSearchString"] = SearchString.ToString().Trim();
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    rm.StoreAllData = objDB.GetChallanDetails(Search, session, pageIndex);

                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                        int count = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                        ViewBag.TotalCount1 = count;
                        int tp = Convert.ToInt32(count);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        return View(rm);

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
        public ActionResult CancelForm(SchoolModels sm, FormCollection fc)
        {
            TempData["GetChallanDetailsSearch"] = null;
            TempData["SelectedItem"] = null;
            TempData["feecatselect"] = null;
            TempData["srhfld"] = null;
            TempData["ChallanDetailsSearchString"] = null;
            return RedirectToAction("ChallanDetails", "Challan");
        }


        public JsonResult ChallanDetailsRegerate(float lumsumfine, string lumsumremarks, string challanid, string gdate, string schl, string vdate)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            try
            {
                string dee = "No";
                string outError = "0";
                int outstatus = 0;
                string Search = string.Empty;
                string UserType = "Admin";
                float fee = 0;
                DateTime date, dateV;
                if (DateTime.TryParseExact(gdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    if (DateTime.TryParseExact(vdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateV))
                    {
                        if (dateV >= date)
                        {
                            // objCommon.ReGenerateChallaanByIdSPAdminNew(lumsumfine, lumsumremarks, challanid, out outstatus, date, fee, dateV, out outError);
                            //ReGenerateChallaanByIdPSEB
                            outstatus = objCommon.ReGenerateChallaanByIdSPAdminNew(adminLoginSession.AdminEmployeeUserId, lumsumfine, lumsumremarks, challanid, out outError, date, fee, dateV);
                            if (Convert.ToInt32(outstatus) > 0)
                            {
                                dee = "Yes";
                            }
                            else
                            { dee = "No"; }
                        }
                        else
                        {
                            outError = "-5";
                            dee = "date";
                        }
                    }
                }
                return Json(new { sn = dee, chid = outError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return null;
            }
        }

        // Refund Challan

        public JsonResult ChallanDetailsRefund(string RefundRefno, string RefundDate, string RefundRemarks, string challanid, string Type)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            try
            {
                string dee = "";
                string outstatus = "";
                int AdminId = Convert.ToInt32(Session["AdminId"]);
                objCommon.ChallanDetailsRefund(RefundRefno, RefundDate, RefundRemarks, challanid, out outstatus, AdminId, Type, adminLoginSession.AdminEmployeeUserId);//ChallanDetailsCancelSP

                dee = outstatus;
                return Json(new { sn = dee, chid = outstatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        // Cancel Challan
        public JsonResult ChallanDetailsCancel(string cancelremarks, string challanid, string Type)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            try
            {
                string dee = "";
                string outstatus = "";
                int AdminId = Convert.ToInt32(Session["AdminId"]);
                objCommon.ChallanDetailsCancel(cancelremarks, challanid, out outstatus, AdminId, Type, adminLoginSession.AdminEmployeeUserId);//ChallanDetailsCancelSP
                //if (outstatus == "1")
                //{
                //    dee = outstatus;
                //}               
                //else
                //{ dee = "No"; }
                dee = outstatus;
                return Json(new { sn = dee, chid = outstatus }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //// Exam Challan Cancel------ Begin---------///
        public ActionResult ExamChallanCancel(int? page, string CancelledStatus)
        {
            if (Session["AdminId"] == null || Session["AdminId"].ToString() == "")
            {
                return RedirectToAction("Index", "Admin");
            }
            try
            {
                ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");
                //var itemsch = new SelectList(new[] { new { ID = "Punjab National Bank", Name = "PNB" }, new { ID = "State Bank Of Patiala", Name = "SBOP" }, }, "ID", "Name", 1);
                //ViewBag.MySch = itemsch.ToList();
                DataSet dsBank = objCommon.GetAdminDetailsById(Convert.ToInt32(Session["AdminId"]), Convert.ToInt32(Session["Session"].ToString().Substring(0, 4)));
                if (dsBank != null)
                {
                    if (dsBank.Tables[3].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[3].Rows)
                        {
                            itemBank.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }
                }
                var itemsch1 = new SelectList(new[] { new { ID = "CHALLANID", Name = "Challan ID" }, new { ID = "SCHLREGID", Name = "SCHLREGID" } }, "ID", "Name", 1);
                ViewBag.feecat = objCommon.GetFeeCat();
                ViewBag.MySch1 = itemsch1;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Challan obj = new Challan();
                string session = null;
                string schl = null;
                AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
                string Search = string.Empty;
                if (CancelledStatus == "SuccessFully" || CancelledStatus == "Failure")
                {
                    ViewData["CancelStatus"] = CancelledStatus;
                    return View();
                }
                else
                {
                    if (pageIndex == 1)
                    {

                        TempData["search"] = null;
                        TempData["SelList"] = null;
                        TempData["SearchString"] = null;
                    }
                    string dee = "%";
                    if (TempData["search"] == null)
                        Search = "SCHLREGID like '" + dee + "' and Feecode=21";
                    else
                    {
                        Search = Convert.ToString(TempData["search"]);
                        ViewBag.SelectedItem = TempData["SelList"];
                        ViewBag.Searchstring = TempData["SearchString"];
                    }
                    obj.StoreAllData = objDB.GetChallanDetails(Search, session, pageIndex);
                    //obj.TotalCount = objDB.GetAll10thPassCount(Search, session, pageIndex);
                    int count = 0;
                    for (int i = 0; i <= obj.StoreAllData.Tables[1].Rows.Count - 1; i++)
                    {
                        count = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[i].ItemArray[0]);

                    }
                    //string count = obj.StoreAllData.Tables[1].Columns[0].ToString();
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = count;
                        int tp = Convert.ToInt32(count);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        //return View(rm);
                        //ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;



                    }
                }
                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        [HttpPost]
        public ActionResult ExamChallanCancel(int? page, Challan obj, FormCollection frm, string cmd, string BankName, string srhfld, string SearchString, string feecat1)
        {
            try
            {
                ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");
                //var itemsch = new SelectList(new[] { new { ID = "Punjab National Bank", Name = "PNB" }, new { ID = "State Bank Of Patiala", Name = "SBOP" }, }, "ID", "Name", 1);
                //ViewBag.MySch = itemsch.ToList();
                DataSet dsBank = objCommon.GetAdminDetailsById(Convert.ToInt32(Session["AdminId"]), Convert.ToInt32(Session["Session"].ToString().Substring(0, 4)));
                if (dsBank != null)
                {
                    if (dsBank.Tables[3].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[3].Rows)
                        {
                            itemBank.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }
                }
                var itemsch1 = new SelectList(new[] { new { ID = "CHALLANID", Name = "Challan ID" }, new { ID = "SCHLREGID", Name = "SCHLREGID" } }, "ID", "Name", 1);
                ViewBag.MySch1 = itemsch1.ToList();
                ViewBag.feecat = objCommon.GetFeeCat();
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
                Challan rm = new Challan();
                string Search = "";
                if (cmd == "Search")
                {
                    string dee = "%";
                    Search = "SCHLREGID like '" + dee + "' and feecode=21";
                    if (BankName != "")
                    {
                        Search += " and a.BANK='" + BankName.ToString().Trim() + "'";
                        ViewBag.SelectedItem = BankName;
                    }
                    //if (feecat1 != "")
                    //{
                    //    Search += " and a.FEECAT like '%" + feecat1.ToString().Trim() + "%'";
                    //    ViewBag.SelectedItem = BankName;
                    //}
                    if (srhfld != "" && SearchString != "")
                    {
                        ViewBag.srhfld = srhfld;
                        if (srhfld == "CHALLANID")
                            Search += " and a.CHALLANID='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "SCHLREGID")
                            Search += " and a.SCHLREGID='" + SearchString.ToString().Trim() + "'";
                        //else if (srhfld == "LOT")
                        //    Search += " and a.DOWNLDFLOT='" + SearchString.ToString().Trim() + "'";
                        //else if (srhfld == "J_REF_NO")
                        //    Search += " and a.J_REF_NO='" + SearchString.ToString().Trim() + "'";
                        //else
                        //    Search += " and a.SCHLREGID='" + SearchString.ToString().Trim() + "'";

                    }

                    TempData["search"] = Search;
                    TempData["SelList"] = BankName;
                    TempData["srhfld"] = srhfld;
                    TempData["SearchString"] = SearchString.ToString().Trim();
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    rm.StoreAllData = objDB.GetChallanDetails(Search, session, pageIndex);
                    int count = 0;
                    for (int i = 0; i <= rm.StoreAllData.Tables[1].Rows.Count - 1; i++)
                    {
                        count = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[i].ItemArray[0]);

                    }
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(count);
                        int tp = Convert.ToInt32(count);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        //return View(rm);
                        //ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;


                        return View(rm);

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

        public ActionResult ChallanCancel(string id)
        {
            string challanid = id;
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
            try
            {
                if (challanid == null || challanid == "")
                {
                    return RedirectToAction("ExamChallanCancel", "Challan");
                }
                else
                {
                    string userName = Session["UserName"].ToString();
                    string result = objDB.ChallanCancel(challanid, userName); // passing Value to DBClass from model
                    int STATUS = Convert.ToInt32(result);
                    if (STATUS > 0)
                    {
                        return RedirectToAction("ExamChallanCancel", "Challan", new { CancelledStatus = "SuccessFully" });
                    }
                    else
                    {
                        return RedirectToAction("ExamChallanCancel", "Challan", new { CancelledStatus = "Failure" });
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                Session["Search"] = null;
                return RedirectToAction("Index", "Admin");
            }
        }
        #endregion ChallanDetails

        #region BankMISPage
        public ActionResult BankMISPage(int? page)
        {
            if (Session["AdminId"] == null || Session["AdminId"].ToString() == "")
            {
                return RedirectToAction("Index", "Admin");
            }

            return View();
        }

        [HttpPost]
        public ActionResult BankMISPage(int? page, Challan obj, FormCollection frm, string cmd, string searchby, string SearchString)
        {
            if (Session["AdminId"] == null || Session["AdminId"].ToString() == "")
            {
                return RedirectToAction("Index", "Admin");
            }
            try
            {
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
                Challan rm = new Challan();
                ViewBag.Searchstring = SearchString;
                //if (Session["Session"] != null)
                //{
                //    session = Session["Session"].ToString();
                //    schl = Session["SCHL"].ToString();
                //    obj.SCHLREGID = Convert.ToDouble(schl);
                //}
                //else
                //{
                //    return RedirectToAction("Index", "Login");
                //}
                string Search = "";
                string session = "";
                if (cmd == "Search" && searchby != "")
                {
                    if (searchby == "challanid")
                    {
                        Search += " a.challanid='" + SearchString.ToString().Trim() + "'";
                    }
                    else if (searchby == "J_REF_NO")
                    {
                        Search += " a.J_REF_NO='" + SearchString.ToString().Trim() + "'";
                    }
                    rm.StoreAllData = objDB.GetChallanDetails(Search, session, pageIndex);
                    int count = 0;
                    for (int i = 0; i <= rm.StoreAllData.Tables[1].Rows.Count - 1; i++)
                    {
                        count = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[i].ItemArray[0]);

                    }
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;


                        return View(rm);

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

        #endregion BankMISPage

        //// Exam Challan Cancel------ End---------///


        #region PSEBHOD ChallanDetails
        public ActionResult PSEBChallanDetails(int? page)
        {
            try
            {
                if (Session["AdminId"] == null || Session["AdminId"].ToString() == "")
                {
                    return RedirectToAction("Index", "Admin");
                }


                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsView = 1; ViewBag.IsVerified = 1; }
                else
                {
                    ViewBag.IsVerified = 1;
                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/CHALLANDETAILS")).Count();
                    }
                }
                #endregion Action Assign Method
                ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");
                // By Rohit  -- select bank from database
                string BankAllow = string.Empty;
                DataSet dsBank = objCommon.GetAdminDetailsById(Convert.ToInt32(Session["AdminId"]), Convert.ToInt32(Session["Session"].ToString().Substring(0, 4)));
                if (dsBank != null)
                {
                    if (dsBank.Tables[3].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[3].Rows)
                        {
                            BankAllow += @dr["Bcode"].ToString().Trim() + ",";
                            itemBank.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }

                    if (dsBank.Tables[1].Rows.Count > 0)
                    {
                        List<SelectListItem> itemDist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[1].Rows)
                        {
                            itemDist.Add(new SelectListItem { Text = @dr["DISTNM"].ToString().Trim(), Value = @dr["DIST"].ToString().Trim() });
                        }
                        ViewBag.MyDist = itemDist.ToList();
                    }
                }
                // End 

                var itemsch1 = new SelectList(new[] { new { ID = "CHALLANID", Name = "Challan ID" }, new { ID = "APPNO", Name = "Appno/RefNo" }, new { ID = "SCHLREGID", Name = "SCHLREGID" }, new { ID = "LOT", Name = "downloaded lot" }, new { ID = "J_REF_NO", Name = "Journal No" } }, "ID", "Name", 1);

                ViewBag.feecat = objCommon.GetFeeCat();

                ViewBag.MySch1 = itemsch1;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Challan obj = new Challan();
                string session = null;
                string schl = null;
                AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
                string Search = string.Empty;
                if (pageIndex == 1)
                {
                    TempData["search"] = null;
                    TempData["SelList"] = null;
                    TempData["SearchString"] = null;
                }
                string dee = "%";
                if (TempData["search"] == null)
                    // Search = "SCHLREGID like '" + dee + "' and Bcode=203 ";
                    Search = "SCHLREGID like '" + dee + "'  ";
                else
                {
                    Search = Convert.ToString(TempData["search"]);
                    ViewBag.SelectedItem = TempData["SelList"];
                    ViewBag.Searchstring = TempData["SearchString"];
                }

                if (BankAllow != "")
                {
                    BankAllow = BankAllow.Remove(BankAllow.LastIndexOf(","), 1);
                    Search += " and a.Bcode in (" + BankAllow.ToString().Trim() + ")";
                }


                Search += " and Verified!=5  and a.Bcode  in ('203')";
                obj.StoreAllData = objDB.GetChallanDetails(Search, session, pageIndex);//GetChallanDetails
                //obj.TotalCount = objDB.GetAll10thPassCount(Search, session, pageIndex);
                int count = 0;
                for (int i = 0; i <= obj.StoreAllData.Tables[1].Rows.Count - 1; i++)
                {
                    count = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[i].ItemArray[0]);

                }
                //string count = obj.StoreAllData.Tables[1].Columns[0].ToString();
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = count;
                    int tp = Convert.ToInt32(count);
                    int pn = tp / 20;
                    int cal = 20 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;
                    //return View(rm);
                    //ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;



                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        [HttpPost]
        public ActionResult PSEBChallanDetails(int? page, Challan obj, FormCollection frm, string cmd, string BankName, string srhfld, string SearchString, string feecat1)
        {
            if (Session["AdminId"] == null || Session["AdminId"].ToString() == "")
            {
                return RedirectToAction("Index", "Admin");
            }
            try
            {

                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsView = 1; ViewBag.IsVerified = 1; }
                else
                {
                    ViewBag.IsVerified = 1;
                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/CHALLANDETAILS")).Count();
                    }
                }
                #endregion Action Assign Method
                ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");
                // By Rohit  -- select bank from database
                string BankAllow = string.Empty;
                DataSet dsBank = objCommon.GetAdminDetailsById(Convert.ToInt32(Session["AdminId"]), Convert.ToInt32(Session["Session"].ToString().Substring(0, 4)));
                if (dsBank != null)
                {
                    if (dsBank.Tables[3].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[3].Rows)
                        {
                            BankAllow += @dr["Bcode"].ToString().Trim() + ",";
                            itemBank.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }

                    if (dsBank.Tables[1].Rows.Count > 0)
                    {
                        List<SelectListItem> itemDist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[1].Rows)
                        {
                            itemDist.Add(new SelectListItem { Text = @dr["DISTNM"].ToString().Trim(), Value = @dr["DIST"].ToString().Trim() });
                        }
                        ViewBag.MyDist = itemDist.ToList();
                    }
                }
                // End 

                //var itemsch = new SelectList(new[] { new { ID = "Punjab National Bank", Name = "PNB" }, new { ID = "State Bank Of Patiala", Name = "SBOP" }, }, "ID", "Name", 1);
                //ViewBag.MySch = itemsch.ToList();

                var itemsch1 = new SelectList(new[] { new { ID = "CHALLANID", Name = "Challan ID" }, new { ID = "APPNO", Name = "Appno/RefNo" }, new { ID = "SCHLREGID", Name = "SCHLREGID" }, new { ID = "LOT", Name = "downloaded lot" }, new { ID = "J_REF_NO", Name = "Journal No" } }, "ID", "Name", 1);
                ViewBag.MySch1 = itemsch1.ToList();
                ViewBag.feecat = objCommon.GetFeeCat();
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
                Challan rm = new Challan();

                string Search = "";
                if (cmd == "Search")
                {
                    string dee = "%";
                    // Search = "SCHLREGID like '" + dee + "'  and Bcode=203 ";
                    Search = "SCHLREGID like '" + dee + "'  ";
                    if (BankName != "")
                    {
                        Search += " and a.Bcode='" + BankName.ToString().Trim() + "'";
                        ViewBag.SelectedItem = BankName;
                    }
                    if (feecat1 != "")
                    {
                        Search += " and a.FEECAT like '%" + feecat1.ToString().Trim() + "%'";
                        ViewBag.SelectedItem = BankName;
                    }
                    if (srhfld != "" && SearchString != "")
                    {
                        ViewBag.srhfld = srhfld;
                        if (srhfld == "CHALLANID")
                            Search += " and a.CHALLANID='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "APPNO")
                            Search += " and a.APPNO='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "SCHLREGID")
                            Search += " and a.SCHLREGID='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "LOT")
                            Search += " and a.DOWNLDFLOT='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "J_REF_NO")
                            Search += " and a.J_REF_NO='" + SearchString.ToString().Trim() + "'";
                    }

                    TempData["search"] = Search;
                    TempData["SelList"] = BankName;
                    TempData["srhfld"] = srhfld;
                    TempData["SearchString"] = SearchString.ToString().Trim();
                    ViewBag.Searchstring = SearchString.ToString().Trim();

                    Search += " and Verified!=5  and a.Bcode  in ('203')";
                    rm.StoreAllData = objDB.GetChallanDetails(Search, session, pageIndex);
                    int count = 0;
                    for (int i = 0; i <= rm.StoreAllData.Tables[1].Rows.Count - 1; i++)
                    {
                        count = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[i].ItemArray[0]);

                    }
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(count);
                        int tp = Convert.ToInt32(count);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        return View(rm);

                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [AdminLoginCheckFilter]
        public JsonResult VerifyPSEBHOD(string feedistrict, string receiptnumber, string depositdate, string challanid, string challandateV, string brcode, string depositremarks)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            try
            {
                BankModels BM = new BankModels();
                string dee = "No";
                string outError = "0";
                int outstatus = 0;
                string Search = string.Empty;
                string UserType = "Admin";
                float fee = 0;
                DateTime date, dateV;
                if (DateTime.TryParseExact(depositdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                {
                    if (DateTime.TryParseExact(challandateV, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateV))
                    {
                        if (dateV >= date)
                        {
                            DataSet ds1 = new AbstractLayer.BankDB().GetChallanDetailsByIdSPBank(challanid);
                            if (ds1.Tables.Count > 0)
                            {
                                if (ds1.Tables[0].Rows.Count > 0)
                                {
                                    BM.BCODE = ds1.Tables[0].Rows[0]["BCODE"].ToString();
                                    BM.TOTFEE = Convert.ToInt32(ds1.Tables[0].Rows[0]["TOTFEE"].ToString());
                                }
                            }

                            BM.CHALLANID = challanid;
                            if (brcode == "203")
                            {
                                BM.BRCODE = "203";
                                BM.BRANCH = "PSEB";
                                BM.BCODE = "203";
                            }
                            else
                            {
                                BM.BRCODE = brcode;
                                BM.BRANCH = new AbstractLayer.DBClass().GetDistE().Where(s => s.Value == brcode).Select(s => s.Text).FirstOrDefault();
                            }

                            BM.J_REF_NO = receiptnumber;
                            BM.DEPOSITDT = depositdate;
                            BM.MIS_FILENM = "";

                            int UPLOADLOT = 0;
                            string Mobile;
                            DataSet Bmis = new AbstractLayer.BankDB().GetTotBankMIS();
                            UPLOADLOT = Convert.ToInt32(Bmis.Tables[0].Rows[0][0].ToString());
                            BM.DepositRemarks = depositremarks;

                            string FeeDistrict = feedistrict;
                            DataTable dt = new AbstractLayer.BankDB().ImportBankMisSP_PSEB(BM, UPLOADLOT, out outstatus, out Mobile, adminLoginSession.AdminEmployeeUserId, feedistrict);
                            if (Convert.ToInt32(outstatus) > 0)
                            {
                                dee = "Yes";
                            }
                            else
                            { dee = "No"; }
                        }
                        else
                        {
                            outError = "-5";
                            dee = "date";
                        }
                    }
                }
                return Json(new { sn = dee, chid = outError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return null;
            }
        }

        #endregion PSEBHOD ChallanDetails



        #region registration CalculateFeeAdminMiddlePrimary

        [AdminLoginCheckFilter]
        public ActionResult CalculateFeeAdminMiddlePrimary(string Status)
        {
            FeeHomeViewModel fhvm = new FeeHomeViewModel();
            TempData["calfeeschl"] = null;
            ViewData["Status"] = ViewBag.Schl = ViewBag.SchlName = "";
            ViewData["FeeStatus"] = null;
            ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");
            return View(fhvm);
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult CalculateFeeAdminMiddlePrimary(string Status, string SearchString, string cmd, string schl, FormCollection frc, FeeHomeViewModel fhvm, string selectedClass)
        {
            try
            {
                ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");
                DateTime date = DateTime.ParseExact(SearchString, "dd/MM/yyyy", null);
                ViewBag.Searchstring = SearchString;

                string UserType = fhvm.Type;

                if (schl == null || schl == "")
                { return RedirectToAction("CalculateFeeAdminMiddlePrimary", "Challan"); }

                if (string.IsNullOrEmpty(selectedClass))
                {
                    ViewData["FeeStatus"] = "NA";
                    return View();
                }


                TempData["calfeeschl"] = ViewBag.Schl = schl.ToString();

                if (Status == "Successfully" || Status == "Failure")
                {
                    //ViewData["Status"] = Status;
                    ViewData["FeeStatus"] = Status;
                    return View();
                }
                else
                {
                    // FormCollection frc = new FormCollection();
                    string FormNM = frc["ChkId"];
                    if (FormNM == null || FormNM == "")
                    {
                        return View();
                    }



                    ViewData["Status"] = "";
                    //string UserType = "User";


                    if (selectedClass == "5" && FormNM.Contains("A"))
                    {
                        ViewData["FeeStatus"] = "WF";
                        return View();

                    }
                    else if (selectedClass == "8" && FormNM.Contains("F"))
                    {
                        ViewData["FeeStatus"] = "WF";
                        return View();
                    }




                    string Search = string.Empty;
                    Search = "SCHL='" + schl.ToString() + "'";
                    FormNM = "'" + FormNM.Replace(",", "','") + "'";
                    Search += "  and type='" + UserType + "' and form_name in(" + FormNM + ")";



                    DataSet dsCheckFee = new AbstractLayer.HomeDB().CheckFeeStatusJunior(schl.ToString(), UserType, FormNM.ToUpper(), date);//CheckFeeStatusSPByView

                    //CheckFeeStatus
                    if (dsCheckFee == null)
                    {
                        // return RedirectToAction("Index", "Home");
                        ViewData["FeeStatus"] = "11";
                        return View();
                    }
                    else
                    {
                        if (dsCheckFee.Tables[0].Rows.Count > 0)
                        {
                            if (dsCheckFee.Tables[0].Rows[0]["Outstatus"].ToString() == "0")
                            {
                                //Not Exist (if lot '0' not xist)
                                ViewData["FeeStatus"] = "0";
                                ViewBag.Message = "All Fees are submmited/ Data Not Available for Fee Calculation...";
                                ViewBag.TotalCount = 0;
                            }
                            else if (dsCheckFee.Tables[0].Rows[0]["Outstatus"].ToString() == "2")
                            {
                                //Not Allowed Some Form are not in Fee
                                ViewData["FeeStatus"] = "2";
                                // ViewBag.Message = "Not Allowed,Some FORM are not in Fee Structure..please contact Punjab School Education Board";
                                // ViewBag.Message = "Calculate fee is allowed only for M1 and T1 Form only";
                                DataSet ds = dsCheckFee;
                                fhvm.StoreAllData = dsCheckFee;
                                ViewBag.TotalCount = dsCheckFee.Tables[0].Rows.Count;
                                // return View(fhvm);
                            }
                            else if (dsCheckFee.Tables[0].Rows[0]["Outstatus"].ToString() == "3" || dsCheckFee.Tables[0].Rows[0]["Outstatus"].ToString() == "5")
                            {
                                int CountTable = dsCheckFee.Tables.Count;
                                ViewBag.CountTable = CountTable;
                                ViewBag.OutStatus = dsCheckFee.Tables[0].Rows[0]["Outstatus"].ToString();
                                //Not Allowed Some Form are not in Fee
                                ViewData["FeeStatus"] = "2";
                                // ViewBag.Message = "Not Allowed, Some Mandatory Fields are not Filled.";
                                // ViewBag.Message = "Some Mandatory Fields Like Subject's/Photograph's,Signature's of Listed Form wise Candidate's are Missing or Duplicate Records. Please Update These Details Then Try Again to Calculate Fee & Final Submission";
                                DataSet ds = dsCheckFee;
                                fhvm.StoreAllData = dsCheckFee;
                                if (CountTable > 1)
                                {
                                    ViewBag.TotalCount = dsCheckFee.Tables[0].Rows.Count;
                                    if (dsCheckFee.Tables[1].Rows.Count > 0)
                                    {
                                        if (dsCheckFee.Tables[1].Rows[0]["Outstatus"].ToString() == "5")
                                        {
                                            ViewBag.TotalCountDuplicate = dsCheckFee.Tables[1].Rows.Count;
                                        }
                                        else
                                        { ViewBag.TotalCountDuplicate = 0; }
                                    }
                                    else
                                    { ViewBag.TotalCountDuplicate = 0; }

                                }


                                // return View(fhvm);
                            }
                            else if (dsCheckFee.Tables[0].Rows[0]["Outstatus"].ToString() == "1")
                            {
                                string cls = selectedClass;
                                DataSet ds = new AbstractLayer.HomeDB().GetCalculateFeeBySchoolJunior(cls, Search, schl.ToString(), date);
                                // DataSet ds = _challanRepository.GetCalculateFeeBySchool(cls, Search, loginSession.SCHL, date);
                                fhvm.StoreAllData = ds;
                                if (fhvm.StoreAllData == null || fhvm.StoreAllData.Tables[0].Rows.Count == 0)
                                {
                                    ViewBag.Message = "Record Not Found";
                                    ViewBag.TotalCount = 0;
                                    ViewData["FeeStatus"] = "3";
                                    return View();
                                }
                                else
                                {
                                    ViewData["FeeStatus"] = "1";
                                    TempData["CalculateFee"] = ds;
                                    ViewBag.TotalCount = fhvm.StoreAllData.Tables[0].Rows.Count;
                                    TempData["AllowBanks"] = ds.Tables[0].Rows[0]["AllowBanks"].ToString();
                                    fhvm.TotalFeesInWords = ds.Tables[1].Rows[0]["TotalFeesInWords"].ToString();
                                    fhvm.EndDate = ds.Tables[0].Rows[0]["EndDateDay"].ToString() + " " + ds.Tables[0].Rows[0]["FeeValidDate"].ToString();
                                }
                            }
                        }
                        else
                        {
                            //return RedirectToAction("Index", "Home");
                            ViewData["FeeStatus"] = "22";
                            return View();
                        }
                    }
                    return View(fhvm);
                }
            }
            catch (Exception ex)
            {
                //ErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }

        }


        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult GenerateLumsumFineChallan(string lumsumfine, string lumsumremarks, FormCollection frm, string ValidDate)
        {

            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];


            PaymentformViewModel pfvm = new PaymentformViewModel();

            if (TempData["CalculateFee"] == null || TempData["CalculateFee"].ToString() == "")
            {
                return RedirectToAction("CalculateFeeAdminMiddlePrimary", "Challan");
            }

            if (TempData["calfeeschl"] == null || TempData["calfeeschl"].ToString() == "")
            {
                return RedirectToAction("CalculateFeeAdminMiddlePrimary", "Challan");
            }


            string schl = string.Empty;
            schl = TempData["calfeeschl"].ToString();
            DataSet ds = new AbstractLayer.HomeDB().GetSchoolLotDetails(schl);

            pfvm.PaymentFormData = ds;
            if (pfvm.PaymentFormData == null || pfvm.PaymentFormData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                return View();
            }
            else
            {
                pfvm.LOTNo = Convert.ToInt32(ds.Tables[0].Rows[0]["LOT"].ToString());
                pfvm.Dist = ds.Tables[0].Rows[0]["Dist"].ToString();
                pfvm.District = ds.Tables[0].Rows[0]["diste"].ToString();
                pfvm.DistrictFull = ds.Tables[0].Rows[0]["DistrictFull"].ToString();
                //pfvm.SchoolCode = Convert.ToInt32(ds.Tables[0].Rows[0]["schl"].ToString());
                pfvm.SchoolCode = ds.Tables[0].Rows[0]["schl"].ToString();
                pfvm.SchoolName = ds.Tables[0].Rows[0]["SchoolFull"].ToString(); // Schollname with station and dist 
                ViewBag.TotalCount = pfvm.PaymentFormData.Tables[0].Rows.Count;

                DataSet dscalFee = (DataSet)TempData["CalculateFee"];
                pfvm.totaddsubfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["HardCopyCertificateFee"].ToString());/// TOTAL  CERT FEE
                pfvm.totaddfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalExamFee"].ToString());/// EXAM FEE
                pfvm.TotalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalFee"].ToString()); // REG FEE
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

                TempData["PaymentForm"] = pfvm;

                //string CheckFee = ds.Tables[1].Rows[0]["TotalFeeAmount"].ToString();
                if (pfvm.TotalFinalFees == 0 && pfvm.TotalFees == 0)
                {
                    ViewBag.CheckForm = 1; // only verify for M1 and T1 
                    TempData["CheckFormFeeAdmin"] = 0;
                }
                else
                {
                    ViewBag.CheckForm = 0; // only verify for M1 and T1 
                    TempData["CheckFormFeeAdmin"] = 1;
                }
            }
            //post here

            ChallanMasterModel CM = new ChallanMasterModel();
            pfvm.BankCode = "203";
            if (TempData["FeeStudentList"] == null || TempData["FeeStudentList"].ToString() == "")
            {
                return RedirectToAction("CalculateFeeAdminMiddlePrimary", "Challan");
            }
            if (ModelState.IsValid)
            {
                string SCHL = TempData["calfeeschl"].ToString();
                string FeeStudentList = TempData["FeeStudentList"].ToString();
                CM.FeeStudentList = FeeStudentList.Remove(FeeStudentList.LastIndexOf(","), 1);

                CM.FEE = Convert.ToInt32(pfvm.TotalFinalFees);
                CM.TOTFEE = Convert.ToInt32(pfvm.TotalFinalFees);
                CM.addfee = Convert.ToInt32(pfvm.totaddfee);// exam fee
                CM.regfee = Convert.ToInt32(pfvm.TotalFees);
                CM.latefee = Convert.ToInt32(pfvm.TotalLateFees);
                CM.addsubfee = Convert.ToInt32(pfvm.totaddsubfee);// cert fee

                CM.FEECAT = pfvm.FeeCategory;
                CM.FEECODE = pfvm.FeeCode;
                CM.FEEMODE = "CASH";
                CM.BANK = "PSEB HOD";
                CM.BCODE = pfvm.BankCode;
                CM.BANKCHRG = pfvm.BankCharges;
                CM.SchoolCode = pfvm.SchoolCode.ToString();
                CM.DIST = pfvm.Dist.ToString();
                CM.DISTNM = pfvm.District;
                CM.LOT = pfvm.LOTNo;
                CM.SCHLREGID = pfvm.SchoolCode.ToString();
                CM.APPNO = pfvm.SchoolCode.ToString();
                CM.type = "schle";
                DateTime CHLNVDATE2;
                if (DateTime.TryParseExact(ValidDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out CHLNVDATE2))
                {
                    CM.ChallanVDateN = CHLNVDATE2;
                }
                //CM.CHLNVDATE = pfvm.FeeDate;
                CM.CHLNVDATE = ValidDate;
                CM.LumsumFine = Convert.ToInt32(lumsumfine);
                CM.LSFRemarks = lumsumremarks;

                string SchoolMobile = "";
                string result = "0";

                CM.EmpUserId = adminLoginSession.AdminEmployeeUserId;
                //InsertPaymentForm(CM, frm, out SchoolMobile);

                result = new AbstractLayer.HomeDB().InsertPaymentForm(CM, frm, out SchoolMobile);
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
                    string Sms = "Your Challan no. " + result + " of Lot no  " + CM.LOT + " successfully generated and valid till Dt " + CM.CHLNVDATE + ". Regards PSEB";
                    try
                    {
                        string getSms = objCommon.gosms(SchoolMobile, Sms);
                        // string getSms = objCommon.gosms("9711819184", Sms);
                    }
                    catch (Exception) { }
                    ModelState.Clear();
                    //--For Showing Message---------//                   
                    return RedirectToAction("GenerateChallaan", "Home", new { ChallanId = result });
                }
            }
            return View();
        }


        #endregion registration CalculateFeeAdminMiddlePrimary

        #region Challan Affiliation Report
        public ActionResult ChallanAffiliationReport(int? page)
        {
            try
            {
                if (Session["AdminId"] == null || Session["AdminId"].ToString() == "")
                {
                    return RedirectToAction("Index", "Admin");
                }


                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsPrint = 1; ViewBag.IsView = 1; ViewBag.IsRegenerate = 1; ViewBag.IsCancel = 1; }
                else
                {

                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        //ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(61)).Count();                        
                        ViewBag.IsPrint = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("HOME/GENERATECHALLAAN")).Count();
                        ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/CHALLANDETAILS")).Count();
                        ViewBag.IsRegenerate = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/CHALLANDETAILSREGERATE")).Count();
                        ViewBag.IsCancel = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/CHALLANDETAILSCANCEL")).Count();
                    }
                }
                #endregion Action Assign Method

                ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");

                // By Rohit  -- select bank from database
                string BankAllow = string.Empty;
                DataSet dsBank = objCommon.GetAdminDetailsById(Convert.ToInt32(Session["AdminId"]), Convert.ToInt32(Session["Session"].ToString().Substring(0, 4)));
                if (dsBank != null)
                {
                    if (dsBank.Tables[3].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[3].Rows)
                        {
                            BankAllow += @dr["Bcode"].ToString().Trim() + ",";
                            itemBank.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }
                }
                // End 

                var itemsch1 = new SelectList(new[] { new { ID = "CHALLANID", Name = "Challan ID" }, new { ID = "APPNO", Name = "Appno/RefNo" }, new { ID = "SCHLREGID", Name = "SCHLREGID" }, new { ID = "LOT", Name = "downloaded lot" }, new { ID = "J_REF_NO", Name = "Journal No" },
                new { ID = "RefundStatus", Name = "Refund Transactions" }}, "ID", "Name", 1);

                ViewBag.feecat = objCommon.GetFeeCat();

                ViewBag.MySch1 = itemsch1;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Challan rm = new Challan();
                string session = null;
                AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
                string Search = string.Empty;

                string dee = "%";
                if (TempData["SearchChallanAffiliationReport"] == null)
                {
                    Search = "SCHLREGID like '" + dee + "'";
                }
                else if (TempData["SearchChallanAffiliationReport"] != null)
                {
                    Search += TempData["SearchChallanAffiliationReport"].ToString();
                    ViewBag.SelectedItem = TempData["SelBankName"];
                    ViewBag.feecatselect = TempData["feecatselect"];
                    ViewBag.srhfld = TempData["srhfld"];
                }

                if (BankAllow != "")
                {
                    BankAllow = BankAllow.Remove(BankAllow.LastIndexOf(","), 1);
                    Search += " and a.Bcode in (" + BankAllow.ToString().Trim() + ")";
                }


                TempData["feecatselect"] = ViewBag.feecatselect;
                TempData["SelBankName"] = ViewBag.SelectedItem;
                TempData["srhfld"] = ViewBag.srhfld;
                TempData["SearchChallanAffiliationReport"] = Search;

                TempData.Keep(); // to store search value for view
                rm.StoreAllData = objDB.ChallanAffiliationReport(Search, session, pageIndex);//GetChallanDetails               
                if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                    int tp = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[0]["totalcount"]);
                    ViewBag.TotalCount1 = tp;
                    int pn = tp / 20;
                    int cal = 20 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;

                }

                return View(rm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        [HttpPost]
        public ActionResult ChallanAffiliationReport(int? page, Challan obj, FormCollection frm, string cmd, string BankName, string srhfld, string SearchString, string feecat1)
        {
            if (Session["AdminId"] == null || Session["AdminId"].ToString() == "")
            {
                return RedirectToAction("Index", "Admin");
            }
            try
            {
                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsPrint = 1; ViewBag.IsView = 1; ViewBag.IsRegenerate = 1; ViewBag.IsCancel = 1; }
                else
                {

                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        //ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(61)).Count();                        
                        ViewBag.IsPrint = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("HOME/GENERATECHALLAAN")).Count();
                        ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/CHALLANDETAILS")).Count();
                        ViewBag.IsRegenerate = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/CHALLANDETAILSREGERATE")).Count();
                        ViewBag.IsCancel = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/CHALLANDETAILSCANCEL")).Count();
                    }
                }
                #endregion Action Assign Method
                ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");

                // By Rohit  -- select bank from database
                string BankAllow = string.Empty;
                DataSet dsBank = objCommon.GetAdminDetailsById(Convert.ToInt32(Session["AdminId"]), Convert.ToInt32(Session["Session"].ToString().Substring(0, 4)));
                if (dsBank != null)
                {
                    if (dsBank.Tables[3].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[3].Rows)
                        {
                            BankAllow += @dr["Bcode"].ToString().Trim() + ",";
                            itemBank.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }
                }
                // End 

                //var itemsch = new SelectList(new[] { new { ID = "Punjab National Bank", Name = "PNB" }, new { ID = "State Bank Of Patiala", Name = "SBOP" }, }, "ID", "Name", 1);
                //ViewBag.MySch = itemsch.ToList();


                var itemsch1 = new SelectList(new[] { new { ID = "CHALLANID", Name = "Challan ID" }, new { ID = "APPNO", Name = "Appno/RefNo" }, new { ID = "SCHLREGID", Name = "SCHLREGID" }, new { ID = "LOT", Name = "downloaded lot" }, new { ID = "J_REF_NO", Name = "Journal No" },
                new { ID = "RefundStatus", Name = "Refund Transactions" }}, "ID", "Name", 1);
                ViewBag.MySch1 = itemsch1.ToList();
                ViewBag.feecat = objCommon.GetFeeCat();
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
                Challan rm = new Challan();

                string Search = "";
                if (cmd == "Search")
                {
                    string dee = "%";
                    Search = "SCHLREGID like '" + dee + "'";
                    if (BankName != "")
                    {
                        Search += " and a.Bcode='" + BankName.ToString().Trim() + "'";
                        ViewBag.SelectedItem = BankName;
                    }
                    if (feecat1 != "")
                    {
                        Search += " and a.FEECAT like '%" + feecat1.ToString().Trim() + "%'";
                        ViewBag.feecatselect = feecat1;
                    }

                    if (srhfld != "" && SearchString != "")
                    {
                        ViewBag.srhfld = srhfld;
                        if (srhfld == "CHALLANID")
                            Search += " and a.CHALLANID='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "APPNO")
                            Search += " and a.APPNO='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "SCHLREGID")
                            Search += " and a.SCHLREGID='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "LOT")
                            Search += " and a.DOWNLDFLOT='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "J_REF_NO")
                            Search += " and a.J_REF_NO='" + SearchString.ToString().Trim() + "'";
                        //else if (srhfld.ToLower() == "RefundStatus".ToLower())
                        //    Search += " and a.Verified=2 ";
                    }

                    if (srhfld != "")
                    {
                        if (srhfld.ToLower() == "RefundStatus".ToLower())
                        {
                            Search += " and a.Verified=2 ";
                        }
                    }


                    TempData["feecatselect"] = ViewBag.feecatselect;
                    TempData["SelBankName"] = ViewBag.SelectedItem;
                    TempData["srhfld"] = ViewBag.srhfld;
                    TempData["SearchChallanAffiliationReport"] = Search;

                    TempData.Keep(); // to store search value for view
                    rm.StoreAllData = objDB.ChallanAffiliationReport(Search, session, pageIndex);
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                        int tp = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[0]["totalcount"]);
                        ViewBag.TotalCount1 = tp;
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        return View(rm);

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


        #endregion Challan Affiliation Report



        #region ViewManualChallanDetails

        [AdminLoginCheckFilter]
        public ActionResult ViewManualChallanDetails(int? page)
        {
            try
            {
                if (Session["AdminId"] == null || Session["AdminId"].ToString() == "")
                {
                    return RedirectToAction("Index", "Admin");
                }
                List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDL().Where(s => s.Value != "C").ToList();
                ViewBag.MyApprovalStatusList = MyAcceptRejectList;

                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsView = 1; ViewBag.IsVerified = 1; }
                else
                {
                    ViewBag.IsVerified = 1;
                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/CHALLANDETAILS")).Count();
                    }
                }
                #endregion Action Assign Method
                ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");
                // By Rohit  -- select bank from database
                string BankAllow = string.Empty;
                DataSet dsBank = objCommon.GetAdminDetailsById(Convert.ToInt32(Session["AdminId"]), Convert.ToInt32(Session["Session"].ToString().Substring(0, 4)));
                if (dsBank != null)
                {
                    if (dsBank.Tables[3].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[3].Rows)
                        {
                            BankAllow += @dr["Bcode"].ToString().Trim() + ",";
                            itemBank.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }

                    if (dsBank.Tables[1].Rows.Count > 0)
                    {
                        List<SelectListItem> itemDist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[1].Rows)
                        {
                            itemDist.Add(new SelectListItem { Text = @dr["DISTNM"].ToString().Trim(), Value = @dr["DIST"].ToString().Trim() });
                        }
                        ViewBag.MyDist = itemDist.ToList();
                    }
                }
                // End 

                var itemsch1 = new SelectList(new[] { new { ID = "CHALLANID", Name = "Challan ID" }, new { ID = "APPNO", Name = "Appno/RefNo" }, new { ID = "SCHLREGID", Name = "SCHLREGID" }, new { ID = "LOT", Name = "downloaded lot" }, new { ID = "J_REF_NO", Name = "Journal No" } }, "ID", "Name", 1);
                ViewBag.MySch1 = itemsch1;

                var itemAppList = new SelectList(new[] { new { ID = "Verified", Name = "Verified" }, new { ID = "Rejected", Name = "Rejected" }, new { ID = "Pending", Name = "Pending" }, }, "ID", "Name", 1);
                ViewBag.MyStatus = itemAppList;
                ViewBag.SelectedStatus = "0";

                ViewBag.feecat = objCommon.GetFeeCat();


                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Challan obj = new Challan();
                string session = null;
                string schl = null;
                AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
                string Search = string.Empty;
                if (pageIndex == 1)
                {
                    TempData["search"] = null;
                    TempData["SelList"] = null;
                    TempData["SearchString"] = null;
                }
                string dee = "%";
                if (TempData["search"] == null)
                    // Search = "SCHLREGID like '" + dee + "' and Bcode=203 ";
                    Search = "SCHLREGID like '" + dee + "'  ";
                else
                {
                    Search = Convert.ToString(TempData["search"]);
                    ViewBag.SelectedItem = TempData["SelList"];
                    ViewBag.Searchstring = TempData["SearchString"];
                }

                if (BankAllow != "")
                {
                    BankAllow = BankAllow.Remove(BankAllow.LastIndexOf(","), 1);
                    Search += " and Bcode in (" + BankAllow.ToString().Trim() + ")";
                }

                Search += " and Verified=5 ";
                obj.StoreAllData = AbstractLayer.BankDB.Get_VerifyManualPaidFee_ChallanDetails(Search, session, pageIndex);//GetChallanDetails



                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
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

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        [HttpPost]
        public ActionResult ViewManualChallanDetails(int? page, Challan obj, FormCollection frm, string ApprovalStatus, string cmd, string BankName, string srhfld, string SearchString, string feecat1)
        {
            if (Session["AdminId"] == null || Session["AdminId"].ToString() == "")
            {
                return RedirectToAction("Index", "Admin");
            }
            try
            {
                List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDL().Where(s => s.Value != "C").ToList();
                ViewBag.MyApprovalStatusList = MyAcceptRejectList;

                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsView = 1; ViewBag.IsVerified = 1; }
                else
                {
                    ViewBag.IsVerified = 1;
                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("CHALLAN/CHALLANDETAILS")).Count();
                    }
                }
                #endregion Action Assign Method
                ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");
                // By Rohit  -- select bank from database
                string BankAllow = string.Empty;
                DataSet dsBank = objCommon.GetAdminDetailsById(Convert.ToInt32(Session["AdminId"]), Convert.ToInt32(Session["Session"].ToString().Substring(0, 4)));
                if (dsBank != null)
                {
                    if (dsBank.Tables[3].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[3].Rows)
                        {
                            BankAllow += @dr["Bcode"].ToString().Trim() + ",";
                            itemBank.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }

                    if (dsBank.Tables[1].Rows.Count > 0)
                    {
                        List<SelectListItem> itemDist = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[1].Rows)
                        {
                            itemDist.Add(new SelectListItem { Text = @dr["DISTNM"].ToString().Trim(), Value = @dr["DIST"].ToString().Trim() });
                        }
                        ViewBag.MyDist = itemDist.ToList();
                    }
                }
                // End 


                var itemsch1 = new SelectList(new[] { new { ID = "CHALLANID", Name = "Challan ID" }, new { ID = "APPNO", Name = "Appno/RefNo" }, new { ID = "SCHLREGID", Name = "SCHLREGID" }, new { ID = "LOT", Name = "downloaded lot" }, new { ID = "J_REF_NO", Name = "Journal No" } }, "ID", "Name", 1);
                ViewBag.MySch1 = itemsch1.ToList();

                var itemAppList = new SelectList(new[] { new { ID = "Verified", Name = "Verified" }, new { ID = "Rejected", Name = "Rejected" }, new { ID = "Pending", Name = "Pending" }, }, "ID", "Name", 1);
                ViewBag.MyStatus = itemAppList;
                ViewBag.SelectedStatus = "0";


                ViewBag.feecat = objCommon.GetFeeCat();
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
                Challan rm = new Challan();

                string Search = "";
                if (cmd == "Search")
                {
                    string dee = "%";
                    // Search = "SCHLREGID like '" + dee + "'  and Bcode=203 ";
                    Search = "SCHLREGID like '" + dee + "'  ";

                    if (ApprovalStatus != "")
                    {
                        Search += " and ManualFinalStatus='" + ApprovalStatus.ToString().Trim() + "'";
                        ViewBag.SelectedStatus = ApprovalStatus;
                    }

                    if (BankName != "")
                    {
                        Search += " and Bcode='" + BankName.ToString().Trim() + "'";
                        ViewBag.SelectedItem = BankName;
                    }
                    if (feecat1 != "")
                    {
                        Search += " and FEECAT like '%" + feecat1.ToString().Trim() + "%'";
                        ViewBag.SelectedItem = BankName;
                    }

                    if (srhfld != "" && SearchString != "")
                    {
                        ViewBag.srhfld = srhfld;
                        if (srhfld == "CHALLANID")
                            Search += " and CHALLANID='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "APPNO")
                            Search += " and APPNO='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "SCHLREGID")
                            Search += " and SCHLREGID='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "LOT")
                            Search += " and DOWNLDFLOT='" + SearchString.ToString().Trim() + "'";
                        else if (srhfld == "J_REF_NO")
                            Search += " and J_REF_NO='" + SearchString.ToString().Trim() + "'";
                    }

                    TempData["search"] = Search;
                    TempData["SelList"] = BankName;
                    TempData["srhfld"] = srhfld;
                    TempData["SearchString"] = SearchString.ToString().Trim();
                    ViewBag.Searchstring = SearchString.ToString().Trim();

                    Search += " and Verified=5 ";
                    obj.StoreAllData = AbstractLayer.BankDB.Get_VerifyManualPaidFee_ChallanDetails(Search, session, pageIndex);//GetChallanDetails

                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
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
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
            }
            return View(obj);
        }


        [AdminLoginCheckFilter]
        public JsonResult VerifyManualPSEBHOD(string feedistrict, string approvalstatus, string receiptnumber, string depositdate, string challanid, string challandateV, string brcode, string depositremarks,
          string OldReceiptNo, string OldDepositDate, string OldChallanId, string OldAmount)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

            if (string.IsNullOrEmpty(approvalstatus))
            {
                return Json(new { sn = "-10", chid = "-10" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                BankModels BM = new BankModels();
                string dee = "No";
                string outError = "0";
                int outstatus = 0;
                string Search = string.Empty;
                string UserType = "Admin";
                float fee = 0;
                DateTime date, dateV;

                DataSet ds1 = new AbstractLayer.BankDB().GetChallanDetailsByIdSPBank(challanid);
                if (ds1.Tables.Count > 0)
                {
                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        BM.BCODE = ds1.Tables[0].Rows[0]["BCODE"].ToString();
                        BM.TOTFEE = Convert.ToInt32(ds1.Tables[0].Rows[0]["TOTFEE"].ToString());
                    }
                }

                BM.CHALLANID = challanid;
                if (brcode == "203")
                {
                    BM.BRCODE = "203";
                    BM.BRANCH = "PSEB";
                    BM.BCODE = "203";
                }
                else
                {
                    BM.BRCODE = brcode;
                    BM.BRANCH = new AbstractLayer.DBClass().GetDistE().Where(s => s.Value == brcode).Select(s => s.Text).FirstOrDefault();
                }

                BM.J_REF_NO = receiptnumber;
                BM.DEPOSITDT = depositdate;
                BM.MIS_FILENM = "";

                int UPLOADLOT = 0;
                string Mobile;
                DataSet Bmis = new AbstractLayer.BankDB().GetTotBankMIS();
                UPLOADLOT = Convert.ToInt32(Bmis.Tables[0].Rows[0][0].ToString());
                BM.DepositRemarks = depositremarks;
                BM.ApprovalStatus = approvalstatus;

                string FeeDistrict = feedistrict;
                string result = AbstractLayer.BankDB.ImportBankMisSP_PSEB_ManualChallanSP(feedistrict, OldReceiptNo, OldDepositDate, OldChallanId, OldAmount, adminLoginSession.AdminEmployeeUserId, BM, UPLOADLOT, out outstatus, out Mobile);
                if (Convert.ToInt32(outstatus) > 0)
                {
                    dee = "Yes";
                }
                else
                { dee = "No"; }

                return Json(new { sn = dee, chid = outError }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return null;
            }
        }

        #endregion PSEBHOD ChallanDetails


        #region verfilyPayment


        public ActionResult VerifyPayment()
        {
            ViewBag.SuccessVerifyMsg = "NONE";
            return View();
        }

        [HttpPost]
        public ActionResult VerifyPayment(string SearchString)
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable result = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("getChallanList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SchlCode", SearchString);
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(result);

                    con.Open();
                    ViewBag.SuccessVerifyMsg = "Fail";
                    foreach (DataRow row in result.Rows)
                    {
                        
                        callBackAPI(row["challanid"].ToString(), Convert.ToInt32(row["totfee"].ToString()));
                    }

                }
            }
            catch {; }
            //callBackAPI("3019224116276", 1000);
            //
            return View();

        }

        public ActionResult VerifyOpenPayment()
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable result = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("select  challanid,totfee from challanmaster where feecode=40  and APPNO in (select APPNO from tblloginopen) and Convert(date,chlndate,103)>=getdate()-3 and VERIFIED=0", con);
                    cmd.CommandType = CommandType.Text;
                    dataAdapter.SelectCommand = cmd;
                    dataAdapter.Fill(result);

                    con.Open();
                    ViewBag.SuccessVerifyMsg = "Fail";
                    foreach (DataRow row in result.Rows)
                    {
                        callBackAPI(row["challanid"].ToString(), Convert.ToInt32(row["totfee"].ToString()));
                    }

                }
            }
            catch {; }
            //callBackAPI("3019224116276", 1000);
            //
            return RedirectToAction("VerifyPayment");

        }

        [HttpPost]
        public ActionResult VerifyPaymentInChallanDetail(string SearchString, int TotFee)
        {

            ViewBag.SuccessVerifyMsg = "Fail";
            try
            {
                callBackAPI(SearchString, TotFee);
            }
            catch
            {
                return Json(new { success = false });
            }
            if (ViewBag.SuccessVerifyMsg == "Success")
            {
                return Json(new { success = true });
            }


            return Json(new { success = false }); // Return a JSON response
        }

        public bool callBackAPI(string orderid, int amount)
        {
            try
            {
                string accessCode = ConfigurationManager.AppSettings["CcAvenueAccessCode"];// ConfigurationManager.AppSettings["AWSAccessKey"];//from avenues
                string workingKey = ConfigurationManager.AppSettings["CcAvenueWorkingKey"];// from avenues

                string orderStatusQueryJson = "{ \"order_no\":" + orderid + " }"; //Ex. { "reference_no":"CCAvenue_Reference_No" , "order_no":"123456"} 
                string encJson = "";

                //string queryUrl = "https://login.ccavenue.com/apis/servlet/DoWebTrans";
                string queryUrl = "https://api.ccavenue.com/apis/servlet/DoWebTrans?";
                CCACrypto ccaCrypto = new CCACrypto();
                encJson = ccaCrypto.Encrypt(orderStatusQueryJson, workingKey);
                //ViewBag.result = encJson;
                // make query for the status of the order to ccAvenues change the command param as per your need
                //  string authQueryUrlParam = "enc_request=" + encJson + "&access_code=" + accessCode + "&command=orderStatusTracker&request_type=JSON&response_type=JSON&version=1.2";
                string authQueryUrlParam = "enc_request=" + encJson + "&access_code=" + accessCode + "&request_type=JSON&response_type=JSON&command=orderStatusTracker&version=1.2";
                // Url Connection
                String message = PaymentRequestToGateway(queryUrl, authQueryUrlParam);
                //ViewBag.result = message; 
                //Response.Write(message);
                NameValueCollection param = ResponseMap(message);
                ViewBag.result = "A" + encJson + "<br>" + message + "<br>" + param.Count;
                String status = "";
                String encResJson = "";
                if (param != null && param.Count == 2)
                {
                    for (int i = 0; i < param.Count; i++)
                    {
                        if ("status".Equals(param.Keys[i]))
                        {
                            status = param[i];
                        }
                        if ("enc_response".Equals(param.Keys[i]))
                        {
                            encResJson += param[i];
                            ViewBag.result = "Final Result::" + ccaCrypto.Decrypt(encResJson, workingKey);
                            //Response.Write(encResXML);
                        }
                    }
                    if (!"".Equals(status) && status.Equals("0"))
                    {
                        String ResJson = ccaCrypto.Decrypt(encResJson, workingKey);
                        // Response.Write(ResJson);
                        // List<Item> items = JsonConvert.DeserializeObject<List<Item>>(ResJson);
                        string str = "";
                        dynamic jsonObj = JsonConvert.DeserializeObject(ResJson);
                        string reference_no = jsonObj.reference_no;

                        string order_no = jsonObj.order_no;

                        string order_capt_amt = jsonObj.order_capt_amt;
                        string order_card_name = jsonObj.order_card_name;
                        string order_status = jsonObj.order_status;
                        int vamt = 0;
                        try
                        {
                            vamt = Convert.ToInt32(order_capt_amt);
                        }
                        catch
                        {
                            vamt = 0;
                        }
                        if (vamt != amount && vamt != 0)
                        {
                            ViewBag.result = "Amount Mismatched";
                        }
                        else if (order_status == "Shipped")
                        {
                            DateTime order_date_time = jsonObj.order_date_time;
                            DataTable dt1 = new DataTable();

                            dt1.Columns.Add("CHALLANID");
                            dt1.Columns.Add("TOTFEE");
                            dt1.Columns.Add("BRCODE");
                            dt1.Columns.Add("BRANCH");
                            dt1.Columns.Add("PAYMETHOD");
                            dt1.Columns.Add("J_REF_NO");
                            dt1.Columns.Add("DEPOSITDT");
                            dt1.Columns.Add("PAYSTATUS");
                            dt1.AcceptChanges();
                            DataRow myDataRow = dt1.NewRow();
                            myDataRow["CHALLANID"] = order_no;
                            myDataRow["TOTFEE"] = order_capt_amt.ToString();
                            myDataRow["BRCODE"] = "301";
                            myDataRow["BRANCH"] = "HDFC";
                            myDataRow["PAYMETHOD"] = order_card_name;
                            myDataRow["J_REF_NO"] = reference_no;
                            myDataRow["DEPOSITDT"] = order_date_time.ToString("dd/MM/yyyy hh:mm:ss");
                            myDataRow["PAYSTATUS"] = "Success";
                            dt1.Rows.Add(myDataRow);
                            dt1.AcceptChanges();

                            AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
                            BankModels BM = new BankModels();
                            BM.BCODE = "301";
                            BM.MIS_FILENM = "D"; int OutStatus = 0; string OutError = "";
                            DataTable dtResult = objDB.BulkOnlinePayment(dt1, 0, 0, BM, out OutStatus, out OutError);  // 
                            if (OutStatus > 0)
                            {
                                ViewBag.SuccessVerifyMsg = "Success";
                                return true;
                            }
                            else
                                return false;
                        }
                        ViewBag.result = str + "<br>" + ResJson;
                        return false;
                        // return ResJson;
                    }
                    else if (!"".Equals(status) && status.Equals("1"))
                    {
                        Console.WriteLine("failure response from ccAvenues: " + encResJson);
                        ViewBag.result = "failure response from ccAvenues: " + encResJson;
                    }

                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private string PaymentRequestToGateway(String queryUrl, String urlParam)
        {

            String message = "";
            //  try
            {
                StreamWriter myWriter = null;// it will open a http connection with provided url
                WebRequest objRequest = WebRequest.Create(queryUrl);//send data using objxmlhttp object
                objRequest.Method = "POST";
                //objRequest.ContentLength = TranRequest.Length;
                objRequest.ContentType = "application/x-www-form-urlencoded";//to set content type
                myWriter = new System.IO.StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(urlParam);//send data
                myWriter.Close();//closed the myWriter object

                // Getting Response
                System.Net.HttpWebResponse objResponse = (System.Net.HttpWebResponse)objRequest.GetResponse();//receive the responce from objxmlhttp object 
                using (System.IO.StreamReader sr = new System.IO.StreamReader(objResponse.GetResponseStream()))
                {
                    message = sr.ReadToEnd();
                    //Response.Write(message);
                }
            }
            //catch (Exception exception)
            //{
            //    Console.Write("Exception occured while connection." + exception);
            //}
            return message;

        }

        private NameValueCollection ResponseMap(String message)
        {
            NameValueCollection Params = new NameValueCollection();
            if (message != null || !"".Equals(message))
            {
                string[] segments = message.Split('&');
                foreach (string seg in segments)
                {
                    string[] parts = seg.Split('=');
                    if (parts.Length > 0)
                    {
                        string Key = parts[0].Trim();
                        string Value = parts[1].Trim();
                        Params.Add(Key, Value);
                    }
                }
            }
            return Params;
        }
        public ActionResult MisUpdate()
        {
            if (Session["AdminId"] == null || Session["AdminId"].ToString() == "")
            {
                return RedirectToAction("Index", "Admin");
            }
            try
            {
                AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
                Challan rm = new Challan();
                rm.StoreAllData = objDB.GetPendingChallanDetails();

                if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                    int count = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
                    //ViewBag.TotalCount1 = count;
                    return View(rm);

                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
            }

            //ViewBag.SuccessVerifyMsg = "NONE";
            return View();
        }

        [HttpPost]
        public ActionResult MisUpdate(int? id)
        {
            AbstractLayer.BankDB objDB = new AbstractLayer.BankDB();
            Challan rm = new Challan();
            try
            {

                rm.StoreAllData = objDB.GetPendingChallanDetails();

                if (rm.StoreAllData.Tables[0].Rows != null || rm.StoreAllData.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in rm.StoreAllData.Tables[0].Rows)
                    {
                        callBackAPI(row["challanid"].ToString(), Convert.ToInt32(row["totfee"].ToString()));
                    }
                }
            }

            catch {; }


            return RedirectToAction("MisUpdate");
        }

        #endregion
    }
}