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
using System.Data.SqlClient;
using ClosedXML;
using ClosedXML.Excel;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Runtime.Serialization.Formatters.Binary;

namespace PSEBONLINE.Controllers
{
    public class TCController : Controller
    {
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
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

        #region Begin TCcontroller For Admin
        //[Route("MigrateSchool")]
        //[Route("MigrateSchool/admin")]


        public ActionResult TCRegProcess()
        {
            try
            {
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsTC = 1; }
                else
                {
                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        // ViewBag.IsTC = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(60)).Count();
                        ViewBag.IsTC = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("TC/TCREQUEST")).Count();
                    }
                }
                #endregion Action Assign Method

                AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
                MigrateSchoolModels MS = new MigrateSchoolModels();

                /***********************Change by Rohit */
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

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "School Code" }, new { ID = "2", Name = "School ID" } }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();

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
        public ActionResult TCRegProcess(FormCollection frm)
        {
            try
            {
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsTC = 1; }
                else
                {

                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        // ViewBag.IsTC = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(60)).Count();
                        ViewBag.IsTC = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("TC/TCREQUEST")).Count();
                    }
                }
                #endregion Action Assign Method
                AbstractLayer.TCDB objDB = new AbstractLayer.TCDB();
                TCModels MS = new TCModels();



                string distID = frm["SelDist"].ToString();
                MS.SelDist = frm["SelDist"].ToString();
                MS.SelList = frm["SelList"].ToString();



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


                var itemsch = new SelectList(new[] { new { ID = "1", Name = "School Code" }, new { ID = "2", Name = "School ID" } }, "ID", "Name", 1);

                ViewBag.MySch = itemsch.ToList();

                if (ModelState.IsValid)
                {
                    string Search = string.Empty;

                    if (distID != "")
                    {
                        Search = "DIST='" + distID + "' ";
                        //ViewBag.MyDist = frm["SelDist"];
                    }
                    else
                    {

                        Search = "DIST like '%%' ";
                    }
                    if (frm["SelList"] != "")
                    {
                        //ViewBag.select = frm["SelList"];
                        int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());
                        if (frm["SearchString"] != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and SCHL='" + frm["SearchString"].ToString() + "'"; }
                            if (SelValueSch == 2)
                            { Search += " and IDNO='" + frm["SearchString"].ToString() + "'"; }
                        }
                    }
                    else
                    {
                        MS.SearchString = string.Empty;
                    }
                    MS.StoreAllData = objDB.GetAllSchoolsByDistTC(Search);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        MS.SearchString = string.Empty;
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        return View(MS);
                    }
                }
                else
                {
                    return TCRegProcess(frm);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }
        #endregion Begin TCcontroller For Admin


        #region TCRequest
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult TCRequest(FormCollection frm, string id)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            AbstractLayer.TCDB objDB = new AbstractLayer.TCDB();
            TCModels MS = new TCModels();

            string SCHL = string.Empty;
            try
            {
                string SelYear = "";
                if (Session["id"] == null)
                {
                    Session["id"] = id;
                    Session["year"] = Session["session"].ToString().Substring(0, 4);
                    SelYear = Session["year"].ToString();
                }
                else
                {
                    id = Session["id"].ToString();
                    SelYear = Session["year"].ToString();
                }

                SCHL = id;
                DataSet result1 = objDB.GetAllFormName(id);
                ViewBag.MyForm = result1.Tables[0];
                List<SelectListItem> FormList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyForm.Rows)
                {
                    FormList.Add(new SelectListItem { Text = @dr["FORM"].ToString(), Value = @dr["FORM"].ToString() });
                }
                ViewBag.MyForm = FormList;

                DataSet result2 = objDB.GetAllLot(id);
                ViewBag.MyLot = result2.Tables[0];
                List<SelectListItem> LotList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyLot.Rows)
                {
                    LotList.Add(new SelectListItem { Text = @dr["LOT"].ToString(), Value = @dr["LOT"].ToString() });
                }
                ViewBag.MyLot = LotList;


                var itemFilter = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
            new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();


                //------------------------

                string schlid = string.Empty;

                MS.SelYear = SelYear;

                DataSet result = objDB.getSCHLstatusTC(id, SelYear);
                if (result.Tables[0].Rows[0]["reclock"].ToString() != "1")
                {
                    ViewData["ComResult"] = "0";
                    return View();
                }


                if (id != null)
                {
                    Session["schl"] = id;
                    string Search = string.Empty;
                    Search = SCHL;
                    MS.StoreAllData = objDB.GetRegEntryviewTC(Search, SelYear);

                    ViewBag.schlCode = MS.StoreAllData.Tables[1].Rows[0]["schlCode"].ToString();
                    ViewBag.schlID = MS.StoreAllData.Tables[1].Rows[0]["schlID"].ToString();
                    ViewBag.schlName = MS.StoreAllData.Tables[1].Rows[0]["schlNM"].ToString();

                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;

                        MS.idno = MS.StoreAllData.Tables[0].Rows[0]["Std_ID"].ToString();
                        MS.SchlCode = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                        MS.Sno = MS.StoreAllData.Tables[0].Rows[0]["SRL"].ToString();
                        MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["REGNO"].ToString();
                        MS.FormName = MS.StoreAllData.Tables[0].Rows[0]["FORM"].ToString();
                        MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                        MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                        MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();
                        MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["Gender"].ToString();
                        // MS.RegDate = MS.StoreAllData.Tables[0].Rows[0]["REGDATE"].ToString();
                        MS.AdmDate = MS.StoreAllData.Tables[0].Rows[0]["ADMDATE"].ToString();
                        MS.Fee = MS.StoreAllData.Tables[0].Rows[0]["Totfee"].ToString();
                        MS.Lot = MS.StoreAllData.Tables[0].Rows[0]["LOT"].ToString();
                        //MS.Std_Sub = MS.StoreAllData.Tables[0].Rows[0]["StdSub"].ToString();

                        return View(MS);
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                Session["Search"] = null;
                Session["id"] = null;
                Session["year"] = null;
                return RedirectToAction("TCRegProcess", "TC");
            }

            return View();
        }
        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult TCRequest(FormCollection frm)
        {
            try
            {
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                string id = string.Empty;
                AbstractLayer.TCDB objDB = new AbstractLayer.TCDB();
                TCModels MS = new TCModels();
                string schlid = string.Empty;
                string SCHL = string.Empty;
                try
                {
                    //schlid = "TUF6NkWFx0oBg-QnwhytFQ%3d%3d";
                    //id = encrypt.QueryStringModule.Decrypt(schlid);

                    SCHL = Session["schl"].ToString();
                    frm["SchlCode"] = SCHL;
                    id = frm["SchlCode"].ToString();
                    DataSet result1 = objDB.GetAllFormName(id);
                    ViewBag.MyForm = result1.Tables[0];
                    List<SelectListItem> FormList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.MyForm.Rows)
                    {
                        FormList.Add(new SelectListItem { Text = @dr["FORM"].ToString(), Value = @dr["FORM"].ToString() });
                    }
                    ViewBag.MyForm = FormList;

                    MS.SelForm = frm["SelForm"].ToString();
                    MS.SelLot = frm["SelLot"].ToString();
                    ViewBag.SelLot = frm["SelLot"].ToString();
                    MS.SelFilter = frm["SelFilter"].ToString();

                    //DataSet result2 = objDB.GetAllLot(id); change by rohit 2020
                    DataTable result2 = result1.Tables[1];
                    ViewBag.MyLot = result2;
                    List<SelectListItem> LotList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.MyLot.Rows)
                    {
                        LotList.Add(new SelectListItem { Text = @dr["LOT"].ToString(), Value = @dr["LOT"].ToString() });
                    }
                    ViewBag.MyLot = LotList;
                }
                catch (Exception ex)
                {
                    Session["Search"] = null;
                    return RedirectToAction("Index", "Admin");
                }

                var itemFilter = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
            new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();

                ////------------------------          
                if (id != null && id != "")
                {
                    Session["schl"] = id;
                    MS.SelYear = frm["SelYear"].ToString();
                    string Search = string.Empty;
                    if (Convert.ToInt32(MS.SelYear) > 2016)
                    {
                        Search = "SCHL='" + id + "' and Registration_num is not Null";
                        if (frm["SelForm"] != "")
                        {
                            Search += " and FORM_Name='" + frm["SelForm"].ToString() + "' ";
                        }
                        if (frm["SelLot"] != "")
                        {
                            Search += " and LOT='" + frm["SelLot"].ToString() + "' ";
                        }
                        if (frm["SelFilter"] != "")
                        {
                            int SelValueSch = Convert.ToInt32(frm["SelFilter"].ToString());
                            if (frm["SelFilter"] != "" && frm["SearchString"].ToString() != "")
                            {
                                if (SelValueSch == 1)
                                { Search += " and Std_ID='" + frm["SearchString"].ToString() + "'"; }
                                else if (SelValueSch == 2)
                                { Search += " and  Registration_num like '%" + frm["SearchString"].ToString() + "%'"; }
                                else if (SelValueSch == 3)
                                { Search += " and  Candi_NAME like '%" + frm["SearchString"].ToString() + "%'"; }
                                else if (SelValueSch == 4)
                                { Search += " and  Father_NAME  like '%" + frm["SearchString"].ToString() + "%'"; }
                                else if (SelValueSch == 5)
                                { Search += " and Mother_NAME like '%" + frm["SearchString"].ToString() + "%'"; }
                                else if (SelValueSch == 6)
                                { Search += " and DOB='" + frm["SearchString"].ToString() + "'"; }
                            }
                        }
                    }
                    else if (MS.SelYear == "2015" || MS.SelYear == "2014")
                    {
                        Search = "SCHL='" + id + "' and REGNO is not Null";
                        if (frm["SelForm"] != "")
                        {
                            Search += " and FORM='" + frm["SelForm"].ToString() + "' ";
                        }
                        if (frm["SelLot"] != "")
                        {
                            Search += " and LOT='" + frm["SelLot"].ToString() + "' ";
                        }
                        if (frm["SelFilter"] != "")
                        {
                            int SelValueSch = Convert.ToInt32(frm["SelFilter"].ToString());
                            if (frm["SelFilter"] != "" && frm["SearchString"].ToString() != "")
                            {
                                if (SelValueSch == 1)
                                { Search += " and ID='" + frm["SearchString"].ToString() + "'"; }
                                else if (SelValueSch == 2)
                                { Search += " and  REGNO like '%" + frm["SearchString"].ToString() + "%'"; }
                                else if (SelValueSch == 3)
                                { Search += " and  NAME like '%" + frm["SearchString"].ToString() + "%'"; }
                                else if (SelValueSch == 4)
                                { Search += " and  FNAME  like '%" + frm["SearchString"].ToString() + "%'"; }
                                else if (SelValueSch == 5)
                                { Search += " and MNAME like '%" + frm["SearchString"].ToString() + "%'"; }
                                else if (SelValueSch == 6)
                                { Search += " and DOB='" + frm["SearchString"].ToString() + "'"; }
                            }
                        }
                    }
                    Session["id"] = id;
                    Session["year"] = MS.SelYear;
                    // MS.StoreAllData = objDB.GetRegEntryviewTC(Search);
                    MS.StoreAllData = objDB.GetRegEntryviewTC_Search(Search, id, MS.SelYear);
                    ViewBag.schlCode = MS.StoreAllData.Tables[1].Rows[0]["schlCode"].ToString();
                    ViewBag.schlID = MS.StoreAllData.Tables[1].Rows[0]["schlID"].ToString();
                    ViewBag.schlName = MS.StoreAllData.Tables[1].Rows[0]["schlNM"].ToString();

                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        MS.idno = MS.StoreAllData.Tables[0].Rows[0]["Std_ID"].ToString();
                        MS.SchlCode = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                        MS.Sno = MS.StoreAllData.Tables[0].Rows[0]["SRL"].ToString();
                        MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["REGNO"].ToString();
                        MS.FormName = MS.StoreAllData.Tables[0].Rows[0]["FORM"].ToString();
                        MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                        MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                        MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();
                        MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["Gender"].ToString();
                        // MS.RegDate = MS.StoreAllData.Tables[0].Rows[0]["REGDATE"].ToString();
                        MS.AdmDate = MS.StoreAllData.Tables[0].Rows[0]["ADMDATE"].ToString();
                        MS.Fee = MS.StoreAllData.Tables[0].Rows[0]["Totfee"].ToString();
                        MS.Lot = MS.StoreAllData.Tables[0].Rows[0]["LOT"].ToString();
                        //MS.Std_Sub = MS.StoreAllData.Tables[0].Rows[0]["StdSub"].ToString();

                        return View(MS);
                    }
                }
                return View(frm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }
        public ActionResult TCRequestDone(string id, string year)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            TCModels sm = new TCModels();
            try
            {
                id = encrypt.QueryStringModule.Decrypt(id);
                year = encrypt.QueryStringModule.Decrypt(year);
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    string stdid = id;
                    if (stdid != null)
                    {
                        AbstractLayer.TCDB objDB = new AbstractLayer.TCDB();

                        string Search = string.Empty;
                        if (year == "2015" || year == "2014")
                        {
                            Search = "ID='" + stdid + "' ";
                        }
                        else
                        {
                            Search = "std_id='" + stdid + "' ";
                        }
                        sm.StoreAllData = objDB.SearchSchoolDetailsTC(Search, year);
                        if (sm.StoreAllData == null || sm.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                            return View();
                        }
                        else
                        {
                            ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                            sm.SCHL = sm.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                            sm.Candi_Name = sm.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                            sm.Father_Name = sm.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                            sm.Mother_Name = sm.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();
                            sm.DOB = sm.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                            sm.Gender = sm.StoreAllData.Tables[0].Rows[0]["Gender"].ToString();
                            sm.SdtID = sm.StoreAllData.Tables[0].Rows[0]["Std_ID"].ToString();
                            sm.FormName = sm.StoreAllData.Tables[0].Rows[0]["FORM"].ToString();
                            sm.regno = sm.StoreAllData.Tables[0].Rows[0]["REGNO"].ToString();
                            sm.SelYear = year;
                            return View(sm);
                        }
                    }
                    else
                    {

                        return TCRequestDone(id, year);
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                Session["Search"] = null;
                return RedirectToAction("TCRegProcess", "TC");
            }

        }
        [HttpPost]
        public ActionResult TCRequestDone(FormCollection fc)
        {
            TCModels sm = new TCModels();

            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            try
            {
                string stdid = encrypt.QueryStringModule.Decrypt(fc["ID"]);
                if (stdid != null)
                {
                    AbstractLayer.TCDB objDB = new AbstractLayer.TCDB();
                    sm.ID = Int32.Parse(stdid);
                    sm.SCHL = fc["SCHL"].ToString();
                    sm.dispatchNo = fc["dispatchNo"].ToString();
                    sm.attendanceTot = fc["attendanceTot"].ToString();
                    sm.attendancePresnt = fc["attendancePresnt"].ToString();
                    sm.struckOff = fc["struckOff"].ToString();
                    sm.reasonFrSchoolLeav = fc["reasonFrSchoolLeav"].ToString();
                    //am.reclock = fc["reclocklist"] == "TRUE" ? "1" : "0";
                    sm.SelYear = fc["SelYear"].ToString();

                    int result = objDB.GenerateTC(sm);
                    // asm.StoreAllData = objDB.SearchSchoolDetails(Search);
                    if (result > 0)
                    {
                        ViewData["SCHL"] = sm.SCHL;
                        ViewData["TCStatus"] = "1";
                        return View();
                        //ViewBag.Message = "Record Updated Successfully";
                        //return RedirectToAction("TCRequest", "School");

                    }
                    else
                    {
                        ViewData["SCHL"] = sm.SCHL;
                        ViewData["TCStatus"] = "0";
                        return View();
                        //ViewBag.Message = "Record not Updated please try again";
                        //return RedirectToAction("TCRequest", "School");
                    }

                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                Session["Search"] = null;
                return RedirectToAction("TCRegProcess", "TC");
            }

            // return View();

        }
        public ActionResult TCPrint(string id, string year)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            try
            {
                TCModels MS = new TCModels();
                AbstractLayer.TCDB objDB = new AbstractLayer.TCDB();
                id = encrypt.QueryStringModule.Decrypt(id);
                year = encrypt.QueryStringModule.Decrypt(year);

                MS.StoreAllData = objDB.SelectTCSchools_Print(id, year);
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    MS.SearchString = string.Empty;
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["SelYear"].ToString();
                    MS.dispatchNo = MS.StoreAllData.Tables[0].Rows[0]["dispatch_no"].ToString();
                    MS.idno = MS.StoreAllData.Tables[0].Rows[0]["Std_ID"].ToString();
                    MS.TCrefno = MS.StoreAllData.Tables[0].Rows[0]["TCrefno"].ToString();
                    MS.TCdate = MS.StoreAllData.Tables[0].Rows[0]["TCdate_N"].ToString();
                    MS.SchlCode = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                    MS.distCode = MS.StoreAllData.Tables[0].Rows[0]["DISTcode"].ToString();
                    MS.distName = MS.StoreAllData.Tables[0].Rows[0]["DISTNM"].ToString();
                    MS.distNameP = MS.StoreAllData.Tables[0].Rows[0]["DISTP"].ToString();
                    MS.SCHLType = MS.StoreAllData.Tables[0].Rows[0]["SCHLType"].ToString();
                    MS.SCHLfullNM_E = MS.StoreAllData.Tables[0].Rows[0]["SCHLfullNM_E"].ToString();
                    MS.SCHLfullNM_P = MS.StoreAllData.Tables[0].Rows[0]["SCHLfullNM_P"].ToString();

                    MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["Regno"].ToString();
                    MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["Name"].ToString();
                    MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FName"].ToString();
                    MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MName"].ToString();
                    MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PName"].ToString();
                    MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFName"].ToString();
                    MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMName"].ToString();


                    MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                    MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["Gender"].ToString();
                    MS.Caste = MS.StoreAllData.Tables[0].Rows[0]["CASTE"].ToString();

                    MS.Religion = MS.StoreAllData.Tables[0].Rows[0]["RELIGION"].ToString();
                    MS.Nation = MS.StoreAllData.Tables[0].Rows[0]["NATION"].ToString();
                    MS.admno = MS.StoreAllData.Tables[0].Rows[0]["Admno"].ToString();
                    MS.AdmDate = MS.StoreAllData.Tables[0].Rows[0]["AdmDate"].ToString();
                    MS.Section = MS.StoreAllData.Tables[0].Rows[0]["Section"].ToString();
                    MS.FormName = MS.StoreAllData.Tables[0].Rows[0]["Form"].ToString();
                    MS.Group_Name = MS.StoreAllData.Tables[0].Rows[0]["Group_Name"].ToString();
                    MS.NSQF_flag = MS.StoreAllData.Tables[0].Rows[0]["NSQF_flag"].ToString();
                    MS.AWRegisterNo = MS.StoreAllData.Tables[0].Rows[0]["AWRegisterNo"].ToString();


                    if (MS.FormName.Contains("N"))
                    {
                        MS.FormName = "9th";
                    }
                    else if (MS.FormName.Contains("M"))
                    {
                        MS.FormName = "10th";
                    }
                    else if (MS.FormName.Contains("E"))
                    {
                        MS.FormName = "11th";
                    }
                    else if (MS.FormName.Contains("T"))
                    {
                        MS.FormName = "12th";
                    }

                    MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["Class_Roll_Num_Section"].ToString();
                    //---------------------------Name Punjabi Start--------------

                    MS.struckOffdt = MS.StoreAllData.Tables[0].Rows[0]["struck_Off_dt"].ToString();
                    MS.Result = MS.StoreAllData.Tables[0].Rows[0]["result"].ToString();
                    MS.attendanceTot = MS.StoreAllData.Tables[0].Rows[0]["Tot_Atd"].ToString();
                    MS.attendancePresnt = MS.StoreAllData.Tables[0].Rows[0]["Present_Atd"].ToString();

                    MS.Totmark = MS.StoreAllData.Tables[0].Rows[0]["totmarks"].ToString();
                    MS.obtmark = MS.StoreAllData.Tables[0].Rows[0]["obtmarks"].ToString();
                    MS.aadhar = MS.StoreAllData.Tables[0].Rows[0]["Aadhar_num"].ToString();
                    return View(MS);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                Session["Search"] = null;
                return RedirectToAction("TCRegProcess", "TC");
            }
        }
        public ActionResult TCCancel(FormCollection fc, string id, string year)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            try
            {
                TCModels MS = new TCModels();
                AbstractLayer.TCDB objDB = new AbstractLayer.TCDB();
                id = encrypt.QueryStringModule.Decrypt(id);
                year = encrypt.QueryStringModule.Decrypt(year);

                if (id != null)
                {
                    MS.SCHL = Session["schl"].ToString();
                    string result = objDB.TCCancel(id, year);
                    if (Convert.ToInt32(result) > 0)
                    {
                        //ViewData["TCCancelStatus"] = result;
                        //return TCRequest(fc, MS.SCHL);
                        return RedirectToAction("TCRequest", "TC");
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("TCRegProcess", "TC");
            }
            return View();
        }


        #endregion TCRequest

        #region TCcontroller For School
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult TCRequestSchl()
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            FormCollection frm = new FormCollection();
            string id = Session["SCHL"].ToString();
            AbstractLayer.TCDB objDB = new AbstractLayer.TCDB();
            TCModels MS = new TCModels();

            string SCHL = string.Empty;
            try
            {
                SCHL = id;
                DataSet result1 = objDB.GetAllFormNameSchl(id);
                ViewBag.MyForm = result1.Tables[0];
                List<SelectListItem> FormList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyForm.Rows)
                {
                    FormList.Add(new SelectListItem { Text = @dr["FORM"].ToString() + "th", Value = @dr["class"].ToString() });
                }
                ViewBag.MyForm = FormList;

                var itemRec = new SelectList(new[] { new { ID = "1", Name = "All TC Generated" }, new { ID = "2", Name = "All TC Pending" }, }, "ID", "Name", 1);
                ViewBag.MyRec = itemRec.ToList();

                var itemFilter = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
            new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();
                //---------------------------------------------
                Import obj = new Import();
                string session = null;
                string schl = null;
                AbstractLayer.ImportDB objDBn = new AbstractLayer.ImportDB();
                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDBn.GetImpschlOcode(3, schl);
                    if (SchoolAssign == null || SchoolAssign == "")
                    { return RedirectToAction("Index", "Login"); }
                    else
                    {
                        if (SchoolAssign.Contains(','))
                        {
                            string[] s = SchoolAssign.Split(',');
                            //  int Cs =    s.Count;
                            foreach (string schlcode in s)
                            {
                                schllist.Add(new SelectListItem { Text = schlcode, Value = schlcode });
                            }
                        }
                        else
                        {
                            schllist.Add(new SelectListItem { Text = schl, Value = schl });
                        }
                    }
                    //schllist.Add(new SelectListItem { Text = "---Select Import TO--", Value = "0" });                
                    ViewBag.MySchCode = schllist;
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }


                //------------------------

                string schlid = string.Empty;

                DataSet result = objDB.getSCHLstatusTCSchl(id);
                if (result.Tables[0].Rows[0]["reclock"].ToString() != "1")
                {
                    ViewData["ComResult"] = "0";
                    return View();
                }

                if (Session["Search"] != null && Session["Search"].ToString() != "")
                {
                    string Search = Session["Search"].ToString();
                    string SelYear = Session["SelYear"].ToString();
                    string SelForm = Session["SelForm"].ToString();
                    string Sid = Session["id"].ToString();

                    MS.SelYear = SelYear;
                    MS.SelForm = SelForm;
                    MS.SchlCode = Sid;
                    MS.StoreAllData = objDB.GetRegEntryviewTCSchl_Search(Search, id, SelYear);
                    ViewBag.schlCode = MS.StoreAllData.Tables[1].Rows[0]["schlCode"].ToString();
                    ViewBag.schlID = MS.StoreAllData.Tables[1].Rows[0]["schlID"].ToString();
                    ViewBag.schlName = MS.StoreAllData.Tables[1].Rows[0]["schlNM"].ToString();
                    ViewBag.StuckOffLastDate = MS.StoreAllData.Tables[0].Rows[0]["StuckOffLastDate"].ToString();
                    ViewBag.TCGenerateLastDate = MS.StoreAllData.Tables[0].Rows[0]["TCGenerateLastDate"].ToString();
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
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                Session["Search"] = null;
                return RedirectToAction("Logout", "Login");
            }

            return View();
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult TCRequestSchl(FormCollection frm)
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                string id = string.Empty;
                AbstractLayer.TCDB objDB = new AbstractLayer.TCDB();
                TCModels MS = new TCModels();
                string schlid = string.Empty;
                string SCHL = string.Empty;
                try
                {
                    //schlid = "TUF6NkWFx0oBg-QnwhytFQ%3d%3d";
                    //id = encrypt.QueryStringModule.Decrypt(schlid);

                    SCHL = id;
                    id = Session["SCHL"].ToString();
                    DataSet result1 = objDB.GetAllFormNameSchl(id);
                    ViewBag.MyForm = result1.Tables[0];
                    List<SelectListItem> FormList = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in ViewBag.MyForm.Rows)
                    {
                        FormList.Add(new SelectListItem { Text = @dr["FORM"].ToString() + "th", Value = @dr["class"].ToString() });
                    }
                    ViewBag.MyForm = FormList;

                    MS.SelForm = frm["SelForm"].ToString();
                    MS.SelRec = frm["SelRec"].ToString();
                    MS.SelYear = frm["SelYear"].ToString();
                    MS.SelFilter = frm["SelFilter"].ToString();

                    var itemRec = new SelectList(new[] { new { ID = "1", Name = "All TC Generated" }, new { ID = "2", Name = "All TC Pending" }, }, "ID", "Name", 1);
                    ViewBag.MyRec = itemRec.ToList();
                }
                catch (Exception ex)
                {
                    Session["Search"] = null;
                    return RedirectToAction("Logout", "Login");
                }

                //var itemFilter = new SelectList(new[] { new { ID = "1", Name = "Student Name" }, new { ID = "2", Name = "Roll No" }, }, "ID", "Name", 1);
                var itemFilter = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
            new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
                ViewBag.MyFilter = itemFilter.ToList();

                //---------------------------------------------
                Import obj = new Import();
                string session = null;
                string schl = null;
                AbstractLayer.ImportDB objDBn = new AbstractLayer.ImportDB();
                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDBn.GetImpschlOcode(3, schl);
                    if (SchoolAssign == null || SchoolAssign == "")
                    { return RedirectToAction("Index", "Login"); }
                    else
                    {
                        if (SchoolAssign.Contains(','))
                        {
                            string[] s = SchoolAssign.Split(',');
                            //  int Cs =    s.Count;
                            foreach (string schlcode in s)
                            {
                                schllist.Add(new SelectListItem { Text = schlcode, Value = schlcode });
                            }
                        }
                        else
                        {
                            schllist.Add(new SelectListItem { Text = schl, Value = schl });
                        }
                    }
                    //schllist.Add(new SelectListItem { Text = "---Select Import TO--", Value = "0" });                
                    ViewBag.MySchCode = schllist;
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }


                //------------------------

                ////------------------------          
                if (id != null && id != "")
                {

                    Session["schl"] = id;
                    id = frm["SchlCode"].ToString();
                    MS.SchlCode = id;
                    string Search = string.Empty;
                    Search = "SCHL='" + id + "'";
                    if (frm["SelForm"] == "")
                    {
                        return View(MS);
                    }
                    else
                    {
                        //if (frm["SelLot"] != "")
                        //{
                        //    Search += " and LOT='" + frm["SelLot"].ToString() + "' ";
                        //}
                        Search += " and class='" + frm["SelForm"].ToString() + "' ";

                        if (frm["SelRec"] == "1")
                        {
                            Search += " and TCrefno is not null ";
                        }
                        if (frm["SelRec"] == "2")
                        {
                            Search += " and TCrefno is null ";
                        }
                        if (frm["SelFilter"] != "")
                        {
                            int SelValueSch = Convert.ToInt32(frm["SelFilter"].ToString());
                            if (frm["SelFilter"] != "" && frm["SearchString"].ToString() != "")
                            {
                                if (MS.SelYear.ToString() == "2015" || MS.SelYear.ToString() == "2014")
                                {
                                    if (SelValueSch == 1)
                                    { Search += " and id='" + frm["SearchString"].ToString() + "'"; }
                                    else if (SelValueSch == 2)
                                    { Search += " and  Regno like '%" + frm["SearchString"].ToString() + "%'"; }
                                    else if (SelValueSch == 3)
                                    { Search += " and  Name like '%" + frm["SearchString"].ToString() + "%'"; }
                                    else if (SelValueSch == 4)
                                    { Search += " and  FName  like '%" + frm["SearchString"].ToString() + "%'"; }
                                    else if (SelValueSch == 5)
                                    { Search += " and MName like '%" + frm["SearchString"].ToString() + "%'"; }
                                    else if (SelValueSch == 6)
                                    { Search += " and DOB='" + frm["SearchString"].ToString() + "'"; }
                                }
                                //if (MS.SelYear.ToString() == "2017" || MS.SelYear.ToString() == "2016")
                                else
                                {
                                    if (SelValueSch == 1)
                                    { Search += " and std_id='" + frm["SearchString"].ToString() + "'"; }
                                    else if (SelValueSch == 2)
                                    { Search += " and  Registration_num like '%" + frm["SearchString"].ToString() + "%'"; }
                                    else if (SelValueSch == 3)
                                    { Search += " and  Candi_Name like '%" + frm["SearchString"].ToString() + "%'"; }
                                    else if (SelValueSch == 4)
                                    { Search += " and  Father_Name  like '%" + frm["SearchString"].ToString() + "%'"; }
                                    else if (SelValueSch == 5)
                                    { Search += " and Mother_Name like '%" + frm["SearchString"].ToString() + "%'"; }
                                    else if (SelValueSch == 6)
                                    { Search += " and DOB='" + frm["SearchString"].ToString() + "'"; }
                                }
                            }
                        }

                        // MS.StoreAllData = objDB.GetRegEntryviewTC(Search);
                        string SelYear = MS.SelYear.ToString();
                        string SelForm = MS.SelForm.ToString();
                        MS.StoreAllData = objDB.GetRegEntryviewTCSchl_Search(Search, id, SelYear);
                        Session["Search"] = Search;
                        Session["SelYear"] = SelYear;
                        Session["SelForm"] = SelForm;
                        Session["id"] = id;
                        ViewBag.schlCode = MS.StoreAllData.Tables[1].Rows[0]["schlCode"].ToString();
                        ViewBag.schlID = MS.StoreAllData.Tables[1].Rows[0]["schlID"].ToString();
                        ViewBag.schlName = MS.StoreAllData.Tables[1].Rows[0]["schlNM"].ToString();
                        ViewBag.StuckOffLastDate = MS.StoreAllData.Tables[0].Rows[0]["StuckOffLastDate"].ToString();
                        ViewBag.TCGenerateLastDate = MS.StoreAllData.Tables[0].Rows[0]["TCGenerateLastDate"].ToString();
                    }
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
                return View(frm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        public ActionResult TCRequestDoneSchl(string id, string year)
        {
            TCModels sm = new TCModels();
            try
            {
                id = encrypt.QueryStringModule.Decrypt(id);
                year = encrypt.QueryStringModule.Decrypt(year);
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    string stdid = id;
                    if (stdid != null)
                    {
                        AbstractLayer.TCDB objDB = new AbstractLayer.TCDB();

                        string Search = string.Empty;
                        if (year == "2015" || year == "2014")
                        {
                            Search = "ID='" + stdid + "' ";
                        }
                        else
                        {
                            Search = "std_id='" + stdid + "' ";
                        }

                        sm.StoreAllData = objDB.SearchSchoolDetailsTCSchl(Search, year);
                        if (sm.StoreAllData == null || sm.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                            return RedirectToAction("TCRequestSchl", "TC");
                        }
                        else
                        {
                            ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                            sm.SCHL = sm.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                            sm.Candi_Name = sm.StoreAllData.Tables[0].Rows[0]["Name"].ToString();
                            sm.Father_Name = sm.StoreAllData.Tables[0].Rows[0]["FName"].ToString();
                            sm.Mother_Name = sm.StoreAllData.Tables[0].Rows[0]["MName"].ToString();
                            sm.DOB = sm.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                            sm.Gender = sm.StoreAllData.Tables[0].Rows[0]["Gender"].ToString();
                            sm.SdtID = sm.StoreAllData.Tables[0].Rows[0]["Std_ID"].ToString();
                            //sm.FormName = sm.StoreAllData.Tables[0].Rows[0]["FORM"].ToString();
                            sm.FormName = sm.StoreAllData.Tables[0].Rows[0]["class"].ToString();
                            sm.SelYear = year;
                            switch (sm.FormName)
                            {
                                case "1": sm.FormName = "9th"; break;
                                case "2": sm.FormName = "10th"; break;
                                case "3": sm.FormName = "11th"; break;
                                case "4": sm.FormName = "12th"; break;
                                default:
                                    break;
                            }
                            sm.regno = sm.StoreAllData.Tables[0].Rows[0]["regno"].ToString();
                            //

                            sm.TCdate = sm.StoreAllData.Tables[0].Rows[0]["TodayDate"].ToString();

                            ViewBag.StuckOffLastDate = sm.StoreAllData.Tables[0].Rows[0]["StuckOffLastDate"].ToString();
                            ViewBag.TCGenerateLastDate = sm.StoreAllData.Tables[0].Rows[0]["TCGenerateLastDate"].ToString();
                            return View(sm);
                        }
                    }
                    else
                    {

                        return TCRequestDoneSchl(id, year);
                    }
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
        public ActionResult TCRequestDoneSchl(FormCollection fc)
        {
            TCModels sm = new TCModels();
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                string stdid = encrypt.QueryStringModule.Decrypt(fc["ID"]);
                if (stdid != null)
                {
                    AbstractLayer.TCDB objDB = new AbstractLayer.TCDB();
                    sm.ID = Int32.Parse(stdid);
                    //sm.SCHL = Session["SCHL"].ToString();
                    sm.SCHL = fc["SCHL"].ToString();
                    sm.dispatchNo = fc["dispatchNo"].ToString();
                    sm.attendanceTot = fc["attendanceTot"].ToString();
                    sm.attendancePresnt = fc["attendancePresnt"].ToString();
                    sm.struckOff = fc["struckOff"].ToString();
                    sm.reasonFrSchoolLeav = fc["reasonFrSchoolLeav"].ToString();
                    sm.SelYear = fc["SelYear"].ToString();
                    sm.TCdate = fc["TCdate"].ToString();

                    int result = objDB.GenerateTCSchl(sm);
                    if (result > 0)
                    {
                        ViewData["SCHL"] = sm.SCHL;
                        ViewData["TCStatus"] = "1";
                        return View();
                    }
                    else
                    {
                        ViewData["SCHL"] = sm.SCHL;
                        ViewData["TCStatus"] = "0";
                        return View();

                    }

                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                Session["Search"] = null;
                return RedirectToAction("Index", "Login");
            }

            // return View();

        }
        public ActionResult TCPrintSchl(string id, string year)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                TCModels MS = new TCModels();
                AbstractLayer.TCDB objDB = new AbstractLayer.TCDB();

                //string Search = string.Empty;
                // Search = "StdID=" + stdID;
                //Search = "std_id= '" + id + "'";
                id = encrypt.QueryStringModule.Decrypt(id);
                year = encrypt.QueryStringModule.Decrypt(year);

                MS.StoreAllData = objDB.SelectTCSchoolsSchl_Print(id, year);
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    MS.SearchString = string.Empty;
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {

                    MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["SelYear"].ToString();
                    MS.dispatchNo = MS.StoreAllData.Tables[0].Rows[0]["dispatch_no"].ToString();
                    MS.idno = MS.StoreAllData.Tables[0].Rows[0]["Std_ID"].ToString();
                    MS.TCrefno = MS.StoreAllData.Tables[0].Rows[0]["TCrefno"].ToString();
                    MS.TCdate = MS.StoreAllData.Tables[0].Rows[0]["TCdate_N"].ToString();
                    MS.SchlCode = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                    MS.distCode = MS.StoreAllData.Tables[0].Rows[0]["DISTcode"].ToString();
                    MS.distName = MS.StoreAllData.Tables[0].Rows[0]["DISTNM"].ToString();
                    MS.distNameP = MS.StoreAllData.Tables[0].Rows[0]["DISTP"].ToString();
                    MS.SCHLType = MS.StoreAllData.Tables[0].Rows[0]["SCHLType"].ToString();
                    MS.SCHLfullNM_E = MS.StoreAllData.Tables[0].Rows[0]["SCHLfullNM_E"].ToString();
                    MS.SCHLfullNM_P = MS.StoreAllData.Tables[0].Rows[0]["SCHLfullNM_P"].ToString();

                    MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["Regno"].ToString();
                    MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["Name"].ToString();
                    MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FName"].ToString();
                    MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MName"].ToString();
                    MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PName"].ToString();
                    MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFName"].ToString();
                    MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMName"].ToString();


                    MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                    MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["Gender"].ToString();
                    MS.Caste = MS.StoreAllData.Tables[0].Rows[0]["CASTE"].ToString();

                    MS.Religion = MS.StoreAllData.Tables[0].Rows[0]["RELIGION"].ToString();
                    MS.Nation = MS.StoreAllData.Tables[0].Rows[0]["NATION"].ToString();
                    MS.admno = MS.StoreAllData.Tables[0].Rows[0]["Admno"].ToString();
                    MS.AdmDate = MS.StoreAllData.Tables[0].Rows[0]["AdmDate"].ToString();
                    MS.Section = MS.StoreAllData.Tables[0].Rows[0]["Section"].ToString();
                    MS.FormName = MS.StoreAllData.Tables[0].Rows[0]["Form"].ToString();
                    MS.Group_Name = MS.StoreAllData.Tables[0].Rows[0]["Group_Name"].ToString();
                    MS.NSQF_flag = MS.StoreAllData.Tables[0].Rows[0]["NSQF_flag"].ToString();
                    MS.AWRegisterNo = MS.StoreAllData.Tables[0].Rows[0]["AWRegisterNo"].ToString();


                    if (MS.FormName.Contains("N"))
                    {
                        MS.FormName = "9th";
                    }
                    else if (MS.FormName.Contains("M"))
                    {
                        MS.FormName = "10th";
                    }
                    else if (MS.FormName.Contains("E"))
                    {
                        MS.FormName = "11th";
                    }
                    else if (MS.FormName.Contains("T"))
                    {
                        MS.FormName = "12th";
                    }

                    MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["Class_Roll_Num_Section"].ToString();
                    //---------------------------Name Punjabi Start--------------

                    MS.struckOffdt = MS.StoreAllData.Tables[0].Rows[0]["struck_Off_dt"].ToString();
                    MS.Result = MS.StoreAllData.Tables[0].Rows[0]["result"].ToString();
                    MS.attendanceTot = MS.StoreAllData.Tables[0].Rows[0]["Tot_Atd"].ToString();
                    MS.attendancePresnt = MS.StoreAllData.Tables[0].Rows[0]["Present_Atd"].ToString();

                    MS.Totmark = MS.StoreAllData.Tables[0].Rows[0]["totmarks"].ToString();
                    MS.obtmark = MS.StoreAllData.Tables[0].Rows[0]["obtmarks"].ToString();
                    MS.aadhar = MS.StoreAllData.Tables[0].Rows[0]["Aadhar_num"].ToString();

                    return View(MS);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                Session["Search"] = null;
                return RedirectToAction("Index", "Login");
            }
        }
        public ActionResult TCCancelSchl(FormCollection fc, string id)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            try
            {
                TCModels MS = new TCModels();
                AbstractLayer.TCDB objDB = new AbstractLayer.TCDB();
                if (id != null)
                {
                    MS.SCHL = Session["schl"].ToString();
                    string result = objDB.TCCancelSchl(id);
                    if (Convert.ToInt32(result) > 0)
                    {
                        //ViewData["TCCancelStatus"] = result;
                        //return TCRequest(fc, MS.SCHL);
                        return RedirectToAction("TCRequestSchl/" + Session["schl"].ToString(), "TC");
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Index", "Login");
            }
            return View();
        }

        #endregion TCcontroller For School
    }
}