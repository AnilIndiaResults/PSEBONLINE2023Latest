using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using PSEBONLINE.Models;
using System.IO;
using System.Web.Routing;
using PSEBONLINE.Filters;

namespace PSEBONLINE.Controllers
{
    public class MigrateSchoolController : Controller
    {

        #region SiteMenu
        //Executes before every action
        protected override void OnActionExecuting(ActionExecutingContext context)
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
        #endregion SiteMenu
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        // GET: MigrateSchool
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CancelForm()
        {
            return RedirectToAction("SchoolRegProcess", "MigrateSchool");
        }
        public ActionResult MigrationList()
        {
            return RedirectToAction("MigrationDetailView", "MigrateSchool");
        }
        //[Route("MigrateSchool")]
        //[Route("MigrateSchool/SchoolRegProcess")]

        [Route("MigrateSchool")]
        [Route("MigrateSchool/SchoolRegProcess")]
        public ActionResult SchoolRegProcess()
        {
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsMigrate = 1; ViewBag.IsMigrationList = 1; }
                else
                {

                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        //ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(61)).Count();                        
                        ViewBag.IsMigrationList = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("MIGRATESCHOOL/MIGRATIONREC")).Count();
                        ViewBag.IsMigrate = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("MIGRATESCHOOL/REGENTRYVIEW")).Count();
                    }
                }
                #endregion Action Assign Method
                ViewBag.AdminType = Session["AdminType"].ToString();
                AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
                MigrateSchoolModels MS = new MigrateSchoolModels();

                //
                List<SelectListItem> items = new List<SelectListItem>();

                int adminid = Convert.ToInt32(Session["AdminId"].ToString());
                ////string Dist_Allow = "";
                ////DataSet dsAdmin = objCommon.GetAdminDetailsById(Convert.ToInt32(Session["AdminId"]), Convert.ToInt32(Session["AdminSession"].ToString().Substring(0, 4)));
                ////if (dsAdmin.Tables.Count > 0)
                ////{
                ////    if (dsAdmin.Tables[0].Rows.Count > 0)
                ////    {
                ////        Dist_Allow = dsAdmin.Tables[0].Rows[0]["Dist_Allow"].ToString();
                ////    }
                ////    if (dsAdmin.Tables[1].Rows.Count > 0)
                ////    {
                ////        foreach (System.Data.DataRow dr in dsAdmin.Tables[1].Rows)
                ////        {
                ////            items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                ////        }
                ////        ViewBag.MyDist = new SelectList(items, "Value", "Text");
                ////    }
                ////}

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
                //

                ////DataSet result = objDB.SelectDist(); 
                ////ViewBag.MyDist = result.Tables[0];             
                ////foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                ////{
                ////    items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                ////}
                ////ViewBag.MyDist = new SelectList(items, "Value", "Text");


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
        [Route("MigrateSchool")]
        [Route("MigrateSchool/SchoolRegProcess")]
        public ActionResult SchoolRegProcess(FormCollection frm)
        {
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsMigrate = 1; ViewBag.IsMigrationList = 1; }
                else
                {

                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        //ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(61)).Count();                        
                        ViewBag.IsMigrationList = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("MIGRATESCHOOL/MIGRATIONREC")).Count();
                        ViewBag.IsMigrate = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("MIGRATESCHOOL/REGENTRYVIEW")).Count();
                    }
                }
                #endregion Action Assign Method
                AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
                MigrateSchoolModels MS = new MigrateSchoolModels();

                //// DataSet result = objDB.SelectDist(); // passing Value to DBClass from model
                ////   ViewBag.MyDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                string distID = frm["SelDist"].ToString();
                ////foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                ////{
                ////    items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                ////}
                MS.SelDist = frm["SelDist"].ToString();
                MS.SelList = frm["SelList"].ToString();
                //// ViewBag.MyDist = new SelectList(items, "Value", "Text");


                int adminid = Convert.ToInt32(Session["AdminId"].ToString());
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
                //MS.SelList = frm["SelList"].ToString();
                ViewBag.MySch = itemsch.ToList();

                if (ModelState.IsValid)
                {
                    string Search = string.Empty;
                    //string schlid = "";
                    //if (Session["SCHL"] != null)
                    //{
                    //    schlid = Session["SCHL"].ToString();
                    //}
                    //else
                    //{
                    //    return View(MS);
                    //}
                    if (distID != "")
                    {
                        Search = "District='" + distID + "' ";
                        //ViewBag.MyDist = frm["SelDist"];
                    }
                    else
                    {

                        Search = "District like '%%' ";
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
                    MS.StoreAllData = objDB.SelectForMigrateSchools(Search);
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
                    return SchoolRegProcess(frm);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }


        #region Begin RegEntryview
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult RegEntryview(FormCollection frm, string id)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
            MigrateSchoolModels MS = new MigrateSchoolModels();

            string SCHL = string.Empty;
            try
            {
                SCHL = id;
                DataSet result1 = objDB.GetAllFormName(id);
                ViewBag.MyForm = result1.Tables[0];
                List<SelectListItem> FormList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyForm.Rows)
                {
                    FormList.Add(new SelectListItem { Text = @dr["form_name"].ToString(), Value = @dr["form_name"].ToString() });
                }
                ViewBag.MyForm = FormList;

                //var itemLot = new SelectList(new[] { new { ID = "1", Name = "1" }, new { ID = "2", Name = "2" }, new { ID = "3", Name = "3" } }, "ID", "Name", 1);
                //ViewBag.MyLot = itemLot.ToList();

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

                if (id != null)
                {
                    string Search = string.Empty;
                    Search = SCHL;
                    MS.StoreAllData = objDB.GetRegEntryviewMigrate(Search);

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

                        MS.idno = MS.StoreAllData.Tables[0].Rows[0]["Std_id"].ToString();
                        MS.SchlCode = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                        MS.Sno = MS.StoreAllData.Tables[0].Rows[0]["Class_Roll_Num_Section"].ToString();
                        MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["Registration_num"].ToString();
                        MS.FormName = MS.StoreAllData.Tables[0].Rows[0]["form_Name"].ToString();
                        MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["Candi_Name"].ToString();
                        MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["Father_Name"].ToString();
                        MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["Mother_Name"].ToString();
                        MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["Gender"].ToString();
                        // MS.RegDate = MS.StoreAllData.Tables[0].Rows[0]["REGDATE"].ToString();
                        MS.AdmDate = MS.StoreAllData.Tables[0].Rows[0]["Admission_Date"].ToString();
                        MS.Fee = MS.StoreAllData.Tables[0].Rows[0]["REGFEE"].ToString();
                        MS.Lot = MS.StoreAllData.Tables[0].Rows[0]["LOT"].ToString();
                        MS.Std_Sub = MS.StoreAllData.Tables[0].Rows[0]["StdSub"].ToString();

                        //MS.idno = MS.StoreAllData.Tables[0].Rows[0]["ID"].ToString();
                        //MS.Sno = MS.StoreAllData.Tables[0].Rows[0]["SRL"].ToString();
                        //MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["REGNO"].ToString();
                        //MS.FormName = MS.StoreAllData.Tables[0].Rows[0]["FORM"].ToString();
                        //MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                        //MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                        //MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();
                        //MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        //MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["SEX"].ToString();
                        //MS.RegDate = MS.StoreAllData.Tables[0].Rows[0]["REGDATE"].ToString();
                        //MS.AdmDate = MS.StoreAllData.Tables[0].Rows[0]["ADMDATE"].ToString();
                        //MS.Fee = MS.StoreAllData.Tables[0].Rows[0]["REGFEE"].ToString();
                        //MS.Lot = MS.StoreAllData.Tables[0].Rows[0]["LOT"].ToString();

                        //ViewBag.MyLot = MS.Lot;

                        return View(MS);
                    }
                }
            }
            catch (Exception ex)
            {
                Session["Search"] = null;
                return RedirectToAction("Index", "Admin");
            }

            return View();
        }


        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult RegEntryview(FormCollection frm)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            string id = string.Empty;
            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
            MigrateSchoolModels MS = new MigrateSchoolModels();
            string schlid = string.Empty;
            string SCHL = string.Empty;
            try
            {
                //schlid = "TUF6NkWFx0oBg-QnwhytFQ%3d%3d";
                //id = encrypt.QueryStringModule.Decrypt(schlid);


                id = frm["SchlCode"].ToString();
                MS.SchlCode = id;
                SCHL = id;
                DataSet result1 = objDB.GetAllFormName(id);
                ViewBag.MyForm = result1.Tables[0];
                List<SelectListItem> FormList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyForm.Rows)
                {
                    FormList.Add(new SelectListItem { Text = @dr["form_name"].ToString(), Value = @dr["form_name"].ToString() });
                }
                ViewBag.MyForm = FormList;

                MS.SelForm = frm["SelForm"].ToString();
                MS.SelLot = frm["SelLot"].ToString();
                ViewBag.SelLot = frm["SelLot"].ToString();
                MS.SelFilter = frm["SelFilter"].ToString();

                DataSet result2 = objDB.GetAllLot(id);
                ViewBag.MyLot = result2.Tables[0];
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



            //var itemFilter = new SelectList(new[] { new { ID = "1", Name = "Student Name" }, new { ID = "2", Name = "Roll No" }, }, "ID", "Name", 1);
            var itemFilter = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
            new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
            ViewBag.MyFilter = itemFilter.ToList();

            ////------------------------



            //if (Convert.ToString(Session["RoleType"]) != "Admin")
            //{
            //    return RedirectToAction("Index", "Admin");
            //}            
            if (id != null && id != "")
            {
                string Search = string.Empty;
                Search = "SCHL='" + id + "' and Registration_Num is not Null";
                if (frm["SelForm"] != "")
                {
                    Search += " and form_name='" + frm["SelForm"].ToString() + "' ";
                }
                if (frm["SelLot"] != "")
                {
                    Search += " and LOT='" + frm["SelLot"].ToString() + "' ";
                }
                else
                {
                    Search += " and LOT >0 ";
                }
                if (frm["SelFilter"] != "")
                {
                    int SelValueSch = Convert.ToInt32(frm["SelFilter"].ToString());
                    if (frm["SelFilter"] != "" && frm["SearchString"].ToString() != "")
                    {
                        //if (frm["SelFilter"] == "1") {Search += "and Candi_Name like '" + frm["SearchString"].ToString() + "%' ";}
                        //else if (frm["SelFilter"] == "2") {Search += "and OROLL='" + frm["SearchString"].ToString() + "' ";}

                        if (SelValueSch == 1)
                        { Search += " and Std_id='" + frm["SearchString"].ToString() + "'"; }
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


                MS.StoreAllData = objDB.GetRegEntryviewMigrate_Search(Search);
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.schlCode = id;
                    ViewBag.schlID = id;
                    ViewBag.schlName = MS.StoreAllData.Tables[1].Rows[0]["schlNM"].ToString();

                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.schlCode = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                    ViewBag.schlID = MS.StoreAllData.Tables[1].Rows[0]["IDNO"].ToString();
                    ViewBag.schlName = MS.StoreAllData.Tables[1].Rows[0]["schlNM"].ToString();

                    MS.idno = MS.StoreAllData.Tables[0].Rows[0]["Std_id"].ToString();
                    MS.SchlCode = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                    MS.Sno = MS.StoreAllData.Tables[0].Rows[0]["Class_Roll_Num_Section"].ToString();
                    MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["Registration_num"].ToString();
                    MS.FormName = MS.StoreAllData.Tables[0].Rows[0]["form_Name"].ToString();
                    MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["Candi_Name"].ToString();
                    MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["Father_Name"].ToString();
                    MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["Mother_Name"].ToString();
                    MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                    MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["Gender"].ToString();
                    // MS.RegDate = MS.StoreAllData.Tables[0].Rows[0]["REGDATE"].ToString();
                    MS.AdmDate = MS.StoreAllData.Tables[0].Rows[0]["Admission_Date"].ToString();
                    MS.Fee = MS.StoreAllData.Tables[0].Rows[0]["REGFEE"].ToString();
                    MS.Lot = MS.StoreAllData.Tables[0].Rows[0]["LOT"].ToString();
                    MS.Std_Sub = MS.StoreAllData.Tables[0].Rows[0]["StdSub"].ToString();

                    return View(MS);
                }
            }
            return View();
        }
        #endregion End RegEntryview

        #region Begin MigrationForm

        public ActionResult MigrationForm(string id)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
            MigrateSchoolModels MS = new MigrateSchoolModels();
            string stdid = string.Empty;
            try
            {
                //id = encrypt.QueryStringModule.Decrypt(stdid);
                // id = "2687544";
                stdid = id;
            }
            catch (Exception ex)
            {
                Session["Search"] = null;
                return RedirectToAction("Index", "Admin");
            }

            if (id != null)
            {
                string Search = string.Empty;
                //Search = "Std_id='" + stdid + "' ";
                Search = stdid;
                MS.StoreAllData = objDB.GetMigrationForm(Search);//GetMigrationForm_SP
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
                    //ViewBag.schlCode = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                    //ViewBag.schlID = MS.StoreAllData.Tables[0].Rows[0]["IDNO"].ToString();
                    //ViewBag.schlName = MS.StoreAllData.Tables[0].Rows[0]["SCHOOLE"].ToString();

                    //MS.SchlCode= MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                    //MS.idno = MS.StoreAllData.Tables[0].Rows[0]["ID"].ToString();
                    //MS.Sno = MS.StoreAllData.Tables[0].Rows[0]["SRL"].ToString();
                    //MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["REGNO"].ToString();
                    //MS.FormName = MS.StoreAllData.Tables[0].Rows[0]["FORM"].ToString();
                    //MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                    //MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                    //MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();
                    //MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                    //MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["SEX"].ToString();
                    //MS.RegDate = MS.StoreAllData.Tables[0].Rows[0]["REGDATE"].ToString();
                    //MS.AdmDate = MS.StoreAllData.Tables[0].Rows[0]["ADMDATE"].ToString();
                    //MS.Fee = MS.StoreAllData.Tables[0].Rows[0]["REGFEE"].ToString();
                    //MS.Lot = MS.StoreAllData.Tables[0].Rows[0]["LOT"].ToString();
                    MS.idno = MS.StoreAllData.Tables[0].Rows[0]["Std_id"].ToString();
                    MS.SchlCode = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                    MS.Sno = MS.StoreAllData.Tables[0].Rows[0]["Class_Roll_Num_Section"].ToString();
                    MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["Registration_num"].ToString();
                    MS.FormName = MS.StoreAllData.Tables[0].Rows[0]["form_Name"].ToString();
                    Session["Class"] = MS.StoreAllData.Tables[0].Rows[0]["Class"].ToString();
                    Session["FormName"] = MS.StoreAllData.Tables[0].Rows[0]["Form_Name"].ToString();
                    MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["Candi_Name"].ToString();
                    MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["Father_Name"].ToString();
                    MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["Mother_Name"].ToString();
                    MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                    MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["Gender"].ToString();
                    // MS.RegDate = MS.StoreAllData.Tables[0].Rows[0]["REGDATE"].ToString();
                    MS.AdmDate = MS.StoreAllData.Tables[0].Rows[0]["Admission_Date"].ToString();
                    MS.Fee = MS.StoreAllData.Tables[0].Rows[0]["REGFEE"].ToString();
                    MS.Lot = MS.StoreAllData.Tables[0].Rows[0]["LOT"].ToString();
                    MS.Std_SubOld = MS.StoreAllData.Tables[0].Rows[0]["Group_Name"].ToString();
                    MS.Std_Sub = MS.StoreAllData.Tables[0].Rows[0]["Group_Name"].ToString();


                    List<SelectListItem> MyGroupList = objCommon.GroupName();
                    //DataTable dtAssignSubject = objCommon.GetAssignSubjectBySchool(Session["SCHL"].ToString());
                    //if (dtAssignSubject != null && dtAssignSubject.Rows.Count > 0)
                    //{
                    //    ViewBag.MyGroup = objCommon.GetSubjectsBySchool(dtAssignSubject, MyGroupList);
                    //}

                    ViewBag.groupNM = MyGroupList;
                    //ViewBag.groupNM = MS.StoreAllData.Tables[0].Rows[0]["Group_Name"].ToString();


                    if (MS.FormName != null)
                    {
                        ViewBag.FormName = "1";
                    }
                    else
                    {
                        ViewBag.FormName = "0";
                    }


                    return View(MS);
                }
            }
            else
            {
                ViewData["MigrateStatus"] = "1";
                return View();
            }

            //return View(frm);
        }
        public JsonResult GetTehID(int SCHL)
        {
            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();

            DataSet rsch = objDB.GetSchl(SCHL);
            if (rsch.Tables[0].Rows.Count > 0)
            {
                ViewBag.schlName = rsch.Tables[0].Rows[0]["SCHOOLE"].ToString();
                Session["DSTName"] = rsch.Tables[0].Rows[0]["DISTNME"].ToString();
                Session["DSTid"] = rsch.Tables[0].Rows[0]["DIST"].ToString();
                ViewBag.schlName = ViewBag.schlName + " (" + Session["DSTName"] + ")";
            }
            else
            {
                ViewBag.schlName = "School Name Not Found";
            }
            return Json(ViewBag.schlName);
        }
        public JsonResult GetGroupList(int SCHL, string sid) // Calling on http post (on Submit)
        {
            string schcd = Convert.ToString(SCHL);
            string Prefix = string.Empty;
            if (schcd.Length <= 7)
            {
                int lname = schcd.Length;
                lname = 7 - lname;
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
            schcd = Prefix + schcd.ToString();

            List<SelectListItem> MyGroupList = objCommon.GroupName();
            DataTable dtAssignSubject = objCommon.GetAssignSubjectBySchool(schcd);
            if (dtAssignSubject != null && dtAssignSubject.Rows.Count > 0)
            {
                ViewBag.MyGroup = objCommon.GetSubjectsBySchool(dtAssignSubject, MyGroupList);
            }


            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
            DataSet ds = objDB.GetStudentRecordsByID(sid);
            string Tgroup = ds.Tables[0].Rows[0]["Group_Name"].ToString();
            if (ds.Tables[0].Rows[0]["class"].ToString() == "3")
            {
                MyGroupList = ViewBag.MyGroup;
                ViewBag.groupNM = MyGroupList;
            }
            else
            {
                string frmNM = "T1";// ds.Tables[0].Rows[0]["form_Name"].ToString();

                ViewBag.MyGroup = objCommon.GetGroupListByGroup(ViewBag.MyGroup, Tgroup, frmNM);
                MyGroupList = ViewBag.MyGroup;
                ViewBag.groupNM = MyGroupList;
            }
            return Json(MyGroupList);
        }

        [HttpPost]
        public ActionResult MigrationForm(FormCollection frm)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
            MigrateSchoolModels MS = new MigrateSchoolModels();
            string stdid = string.Empty;
            string id = string.Empty;
            try
            {
                //stdid = "n_d1Sa6B9rkk2Wyl-QBuBg%3d%3d";
                //id = encrypt.QueryStringModule.Decrypt(stdid);
                id = frm["idno"]; //"2687544";

            }
            catch (Exception ex)
            {
                Session["Search"] = null;
                return RedirectToAction("Index", "Admin");
            }

            if (id != null)
            {
                MS.StdId = id;
                MS.StdId = frm["idno"];
                MS.SchlCode = frm["SCHLCode"];
                MS.RegNo = frm["RegNo"];
                MS.Candi_Name = frm["Candi_Name"];
                MS.Father_Name = frm["Father_Name"];
                MS.Mother_Name = frm["Mother_Name"];
                MS.SchlCodeNew = frm["SchlCodeNew"];
                MS.DistName = Session["DSTName"].ToString();
                MS.Std_Sub = frm["Std_Sub"];
                //MS.rdoDD = frm["rdoDD"];
                //MS.rdoBrdRcpt = frm["rdoBrdRcpt"];
                MS.DDRcptNo = frm["DDRcptNo"];
                MS.Amount = frm["Amount"];
                MS.DepositDt = frm["DepositDt"];
                MS.BankName = frm["BankName"];

                MS.DiryOrderNo = frm["DiryOrderNo"];
                MS.OrderDt = frm["OrderDt"];
                MS.OrderBy = frm["OrderBy"];
                MS.Remark = frm["Remark"];
                MS.FormName = frm["FormName"];


                string scode = MS.SchlCode;
                MS.SchlName = ViewBag.schlName;
                MS.UserName = Session["UserName"].ToString();


                if (MS.FormName != null)
                {
                    //--------- check subject 
                    string sub = MS.Std_Sub;
                    if (sub != null && sub != "")
                    {

                        switch (sub)
                        {
                            case "AGRICULTURE": sub = "AGRI"; break;
                            case "COMMERCE": sub = "COMM"; break;
                            case "HUMANITIES": sub = "HUM"; break;
                            case "SCIENCE": sub = "SCI"; break;
                            case "TECHNICAL": sub = "TECH"; break;
                            case "VOCATIONAL": sub = "VOC"; break;

                        }

                        DataSet SubResult = objDB.ChekResultCompairSubjects(frm["SchlCodeNew"], sub);
                        string ComResult = SubResult.Tables[0].Rows[0][0].ToString();
                        if (ComResult == "0")
                        {
                            ViewData["ComResult"] = "0";
                            return View();
                        }
                        //----end
                    }
                }


                DataSet result = objDB.Insert_MigrationForm(MS, frm);
                //if (result == 2)

                // MIGRATE SCHL NOT TO BE SAME
                if (result.Tables[0].Rows[0]["RESULT"].ToString() == "0")
                {
                    ViewData["StdID"] = id;
                    ViewData["MigrateStatus"] = "0";
                    return View();
                    //return RedirectToAction("MigrationRec", "MigrateSchool");
                }
                // MIGRATE SCHL Std ALREADY
                else if (result.Tables[0].Rows[0]["RESULT"].ToString() == "1")
                {
                    ViewData["StdID"] = id;
                    ViewData["MigrateStatus"] = "1";
                    return View();
                    //return RedirectToAction("MigrationRec", "MigrateSchool");
                }
                // MIGRATE SCHL SUCCESSFULLY
                if (result.Tables[0].Rows[0]["RESULT"].ToString() == "2")
                {
                    ViewData["StdID"] = id;
                    ViewData["MigrateStatus"] = "2";
                    return View();
                    //return RedirectToAction("MigrationRec", "MigrateSchool");
                }
                // MIGRATE SCHL NOT DONE TRY AGAIN
                else
                {
                    ViewData["MigrateStatus"] = result;
                    return RedirectToAction("SchoolRegProcess", "MigrateSchool");
                }
            }
            return View(frm);
        }

        public ActionResult MigrationRec()
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            try
            {
                FormCollection frm = new FormCollection();
                AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
                MigrateSchoolModels MS = new MigrateSchoolModels();

                // DataSet result = objDB.SelectDist(); // passing Value to DBClass from model
                // ViewBag.MyDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                string distID = Convert.ToString(frm["SelDist"]);
                //foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                //{
                //    items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                //}
                // ViewBag.MyDist = new SelectList(items, "Value", "Text");

                int adminid = Convert.ToInt32(Session["AdminId"].ToString());
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

                //var itemsch = new SelectList(new[] { new { ID = "1", Name = "School Code" }, }, "ID", "Name", 1);
                //ViewBag.MySch = itemsch.ToList();

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "School Code" }, new { ID = "2", Name = "Candidate ID" }, new { ID = "3", Name = "Candidate Name" }, new { ID = "4", Name = "Father Name" }, new { ID = "5", Name = "Mother Name" }, }, "ID", "Name", 1);
                //MS.SelList = frm["SelList"].ToString();
                ViewBag.MySch = itemsch.ToList();

                if (ModelState.IsValid)
                {
                    string Search = string.Empty;
                    Search = "smm.DIST like '%%' ";
                    MS.StoreAllData = objDB.SelectMigrateSchools(Search);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        MS.SearchString = string.Empty;
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        if (Session["AdminType"].ToString() == "admin")
                        { ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count; }
                        else { ViewBag.TotalCount = 0; }
                        return View(MS);
                    }
                }
            }
            catch (Exception ex)
            {
                Session["Search"] = null;
                return RedirectToAction("Index", "Admin");
            }
            return View();
        }

        [HttpPost]
        public ActionResult MigrationRec(FormCollection frm)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            try
            {
                AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
                MigrateSchoolModels MS = new MigrateSchoolModels();

                //  DataSet result = objDB.SelectDist(); // passing Value to DBClass from model
                //  ViewBag.MyDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                string distID = frm["SelDist"].ToString();
                //  foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                //  {
                //     items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                //}
                MS.SelDist = frm["SelDist"].ToString();
                MS.SelList = frm["SelList"].ToString();
                // ViewBag.MyDist = new SelectList(items, "Value", "Text");

                int adminid = Convert.ToInt32(Session["AdminId"].ToString());
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

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "School Code" }, new { ID = "2", Name = "Candidate ID" }, new { ID = "3", Name = "Candidate Name" }, new { ID = "4", Name = "Father Name" }, new { ID = "5", Name = "Mother Name" }, }, "ID", "Name", 1);
                //MS.SelList = frm["SelList"].ToString();
                ViewBag.MySch = itemsch.ToList();

                if (ModelState.IsValid)
                {
                    string Search = string.Empty;
                    if (distID != "")
                    {
                        Search = "smm.DIST='" + distID + "' ";
                    }
                    else
                    {
                        Search = "smm.DIST like '%%' ";
                    }
                    if (frm["SelList"] != "")
                    {
                        ViewBag.SelectedItem = frm["SelList"];
                        int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());
                        if (frm["SearchString"] != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and smm.SchlCodeNew='" + frm["SearchString"].ToString() + "'"; }
                            if (SelValueSch == 2)
                            { Search += " and smm.StdId='" + frm["SearchString"].ToString() + "'"; }
                            if (SelValueSch == 3)
                            { Search += " and smm.Candi_Name like '%" + frm["SearchString"].ToString() + "%'"; }
                            if (SelValueSch == 4)
                            { Search += " and smm.Father_Name like '%" + frm["SearchString"].ToString() + "%'"; }
                            if (SelValueSch == 5)
                            { Search += " and smm.Mother_Name like '%" + frm["SearchString"].ToString() + "%'"; }
                        }
                    }
                    else
                    {
                        MS.SearchString = string.Empty;
                    }
                    MS.StoreAllData = objDB.SelectMigrateSchools(Search);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        MS.SearchString = string.Empty;
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                    else
                    {
                        ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        return View(MS);
                    }
                }
                else
                {
                    return MigrationRec(frm);
                }
            }
            catch (Exception ex)
            {
                Session["Search"] = null;
                return RedirectToAction("Index", "Admin");
            }
        }

        #endregion End MigrationForm

        #region View Migration Details
        public ActionResult MigrationDetailView(string id)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            try
            {
                MigrateSchoolModels MS = new MigrateSchoolModels();
                AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();

                string Search = string.Empty;
                Search = "smm.ID= '" + id + "'";
                //Search = "StdID= '" + id + "'";
                MS.StoreAllData = objDB.SelectMigrateSchools(Search);//SelectMigrateSchools_sp
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

                    MS.SchlCode = MS.StoreAllData.Tables[0].Rows[0]["SchlCode"].ToString();
                    MS.idno = MS.StoreAllData.Tables[0].Rows[0]["StdId"].ToString();
                    MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["RegNo"].ToString();
                    MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["Candi_Name"].ToString();
                    MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["Father_Name"].ToString();
                    MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["Mother_Name"].ToString();
                    MS.SchlCodeNew = MS.StoreAllData.Tables[0].Rows[0]["SchlCodeNew"].ToString();
                    //MS.SchlName= MS.StoreAllData.Tables[0].Rows[0]["SchlName"].ToString();

                    MS.DDRcptNo = MS.StoreAllData.Tables[0].Rows[0]["DDRcptNo"].ToString();
                    MS.Amount = MS.StoreAllData.Tables[0].Rows[0]["Amount"].ToString();
                    MS.DepositDt = MS.StoreAllData.Tables[0].Rows[0]["DepositDt"].ToString();
                    MS.BankName = MS.StoreAllData.Tables[0].Rows[0]["BankName"].ToString();

                    MS.MigrateNo = MS.StoreAllData.Tables[0].Rows[0]["ID"].ToString();
                    MS.DiryOrderNo = MS.StoreAllData.Tables[0].Rows[0]["DiryOrderNo"].ToString();
                    MS.OrderDt = MS.StoreAllData.Tables[0].Rows[0]["OrderDt"].ToString();
                    MS.OrderBy = MS.StoreAllData.Tables[0].Rows[0]["OrderBy"].ToString();
                    MS.Remark = MS.StoreAllData.Tables[0].Rows[0]["Remark"].ToString();

                    return View(MS);
                }
            }
            catch (Exception ex)
            {
                Session["Search"] = null;
                return RedirectToAction("Index", "Admin");
            }

            //return View();
        }
        #endregion View Migration Details

        public ActionResult MigrationPrint(string id)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            try
            {
                MigrateSchoolModels MS = new MigrateSchoolModels();
                AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();

                string Search = string.Empty;
                // Search = "StdID=" + stdID;
                Search = "StdID= '" + id + "'";
                MS.StoreAllData = objDB.SelectMigrateSchools_Print(id);
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

                    MS.MigrateNo = MS.StoreAllData.Tables[0].Rows[0]["ID"].ToString();
                    MS.RegSet = MS.StoreAllData.Tables[0].Rows[0]["RegSet"].ToString();
                    MS.Migrationdate = MS.StoreAllData.Tables[0].Rows[0]["MigrationDate"].ToString();

                    MS.SchlCode = MS.StoreAllData.Tables[0].Rows[0]["SchlCode"].ToString();
                    MS.newSchlDetail = MS.StoreAllData.Tables[0].Rows[0]["newSchlDetail"].ToString();
                    MS.newForm = MS.StoreAllData.Tables[0].Rows[0]["Form"].ToString();
                    if (MS.newForm == "N1" || MS.newForm == "N2" || MS.newForm == "N3")
                    {
                        MS.newForm = "9th";
                    }
                    else if (MS.newForm == "M1" || MS.newForm == "M2")
                    {
                        MS.newForm = "10th";
                    }
                    else if (MS.newForm == "E1" || MS.newForm == "E2")
                    {
                        MS.newForm = "11th";
                    }
                    else if (MS.newForm == "T1" || MS.newForm == "T2")
                    {
                        MS.newForm = "12th";
                    }

                    MS.DistNameP = MS.StoreAllData.Tables[0].Rows[0]["DISTNMP"].ToString();
                    MS.SchlCodeNew = MS.StoreAllData.Tables[0].Rows[0]["SchlCodeNew"].ToString();
                    MS.OldSchlDetail = MS.StoreAllData.Tables[0].Rows[0]["OldSchlDetail"].ToString();
                    MS.idno = MS.StoreAllData.Tables[0].Rows[0]["StdId"].ToString();
                    MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["RegNo"].ToString();
                    MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["Candi_Name"].ToString();
                    MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["Father_Name"].ToString();
                    MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["Mother_Name"].ToString();

                    //MS.SchlName= MS.StoreAllData.Tables[0].Rows[0]["SchlName"].ToString();

                    MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["Pname"].ToString();
                    MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFname"].ToString();
                    MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMname"].ToString();

                    MS.DDRcptNo = MS.StoreAllData.Tables[0].Rows[0]["DDRcptNo"].ToString();
                    MS.Amount = MS.StoreAllData.Tables[0].Rows[0]["Amount"].ToString();
                    MS.DepositDt = MS.StoreAllData.Tables[0].Rows[0]["DepositDt"].ToString();
                    MS.BankName = MS.StoreAllData.Tables[0].Rows[0]["BankName"].ToString();

                    MS.MigrateNo = MS.StoreAllData.Tables[0].Rows[0]["ID"].ToString();
                    MS.DiryOrderNo = MS.StoreAllData.Tables[0].Rows[0]["DiryOrderNo"].ToString();
                    MS.OrderDt = MS.StoreAllData.Tables[0].Rows[0]["OrderDt"].ToString();
                    MS.OrderBy = MS.StoreAllData.Tables[0].Rows[0]["OrderBy"].ToString();
                    MS.Remark = MS.StoreAllData.Tables[0].Rows[0]["Remark"].ToString();

                    return View(MS);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                Session["Search"] = null;
                return RedirectToAction("Index", "Admin");
            }
        }

        public ActionResult MigrationDelete(string id)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB(); //calling class DBClass
            try
            {
                if (id == null)
                {
                    return RedirectToAction("MigrationRec", "MigrateSchool");
                }
                else
                {
                    string result = objDB.DeleteFromData(id); // passing Value to DBClass from model                    

                    ViewData["MigrateDeleteStatus"] = result;
                    return RedirectToAction("MigrationRec", "MigrateSchool");



                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                Session["Search"] = null;
                return RedirectToAction("Index", "Admin");
            }
        }

        public ActionResult N1formview(string id)
        {
            try
            {
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                RegistrationModels rm = new RegistrationModels();

                string formname = "N1";
                if (id != null)
                {
                    try
                    {
                        id = encrypt.QueryStringModule.Decrypt(id);

                        DataSet ds = objDB.SearchStudentGetByData(id, formname);
                        if (ds == null || ds.Tables[0].Rows.Count == 0)
                        {
                            return RedirectToAction("N1Formgrid", "RegistrationPortal");
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            //DataSet ds = objDB.SearchStudentGetByData(id);
                            // rm.StoreAllData = objDB.SearchStudentGetByData(id);
                            rm.Category = ds.Tables[0].Rows[0]["Category"].ToString();
                            rm.Board = ds.Tables[0].Rows[0]["Board"].ToString();
                            rm.Board_Roll_Num = ds.Tables[0].Rows[0]["Board_Roll_Num"].ToString();
                            rm.Prev_School_Name = ds.Tables[0].Rows[0]["Prev_School_Name"].ToString();
                            rm.Month = ds.Tables[0].Rows[0]["Month"].ToString();
                            rm.Year = ds.Tables[0].Rows[0]["Year"].ToString();
                            rm.AWRegisterNo = ds.Tables[0].Rows[0]["AWRegisterNo"].ToString();
                            rm.Admission_Num = ds.Tables[0].Rows[0]["Admission_Num"].ToString();
                            rm.Admission_Date = ds.Tables[0].Rows[0]["Admission_Date"].ToString();
                            rm.Class_Roll_Num_Section = ds.Tables[0].Rows[0]["Class_Roll_Num_Section"].ToString();
                            rm.Candi_Name = ds.Tables[0].Rows[0]["Candi_Name"].ToString();
                            rm.Candi_Name_P = ds.Tables[0].Rows[0]["Candi_Name_P"].ToString();
                            rm.Father_Name = ds.Tables[0].Rows[0]["Father_Name"].ToString();
                            rm.Father_Name_P = ds.Tables[0].Rows[0]["Father_Name_P"].ToString();
                            rm.Mother_Name = ds.Tables[0].Rows[0]["Mother_Name"].ToString();
                            rm.Mother_Name_P = ds.Tables[0].Rows[0]["Mother_Name_P"].ToString();
                            rm.Caste = ds.Tables[0].Rows[0]["Caste"].ToString();
                            rm.Gender = ds.Tables[0].Rows[0]["Gender"].ToString();
                            rm.Differently_Abled = ds.Tables[0].Rows[0]["Differently_Abled"].ToString();
                            rm.Religion = ds.Tables[0].Rows[0]["Religion"].ToString();
                            rm.DOB = ds.Tables[0].Rows[0]["DOB"].ToString();
                            rm.Mobile = ds.Tables[0].Rows[0]["Mobile"].ToString();
                            rm.Belongs_BPL = ds.Tables[0].Rows[0]["Belongs_BPL"].ToString();
                            rm.E_punjab_Std_id = ds.Tables[0].Rows[0]["E_punjab_Std_id"].ToString();
                            rm.Aadhar_num = ds.Tables[0].Rows[0]["Aadhar_num"].ToString();
                            rm.Address = ds.Tables[0].Rows[0]["Address"].ToString();
                            rm.LandMark = ds.Tables[0].Rows[0]["LandMark"].ToString();
                            rm.Block = ds.Tables[0].Rows[0]["Block"].ToString();
                            rm.MyDistrict = ds.Tables[0].Rows[0]["distE"].ToString();
                            rm.MYTehsil = ds.Tables[0].Rows[0]["tehE"].ToString();
                            rm.PinCode = ds.Tables[0].Rows[0]["PinCode"].ToString();
                            rm.Section = Convert.ToChar(ds.Tables[0].Rows[0]["Section"].ToString());
                            string formName = ds.Tables[0].Rows[0]["form_Name"].ToString();
                            // @ViewBag.ImageURL = "../../StdImages/Upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                            @ViewBag.Photo = "../../upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                            @ViewBag.sign = "../../upload/" + ds.Tables[0].Rows[0]["std_Sign"].ToString();
                        }
                        else
                        {
                            return RedirectToAction("N1Formgrid", "RegistrationPortal");
                        }

                    }
                    catch (Exception ex)
                    {

                        //throw;
                        return RedirectToAction("N1Formgrid", "RegistrationPortal");
                    }

                }
                //return View(asm);
                return View(rm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }
        public ActionResult E1formview(string id)
        {
            try
            {
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                RegistrationModels rm = new RegistrationModels();
                string formname = "E1";
                if (id != null)
                {
                    try
                    {
                        id = encrypt.QueryStringModule.Decrypt(id);
                        DataSet ds = objDB.SearchStudentGetByData_E(id, formname);
                        if (ds == null || ds.Tables[0].Rows.Count == 0)
                        {
                            return RedirectToAction("E1Formgrid", "RegistrationPortal");
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            //DataSet ds = objDB.SearchStudentGetByData(id);
                            // rm.StoreAllData = objDB.SearchStudentGetByData(id);
                            rm.Category = ds.Tables[0].Rows[0]["Category"].ToString();
                            rm.Board = ds.Tables[0].Rows[0]["Board"].ToString();
                            rm.Other_Board = ds.Tables[0].Rows[0]["Other_Board"].ToString();
                            rm.Registration_num = ds.Tables[0].Rows[0]["Registration_num"].ToString();
                            rm.Board_Roll_Num = ds.Tables[0].Rows[0]["Board_Roll_Num"].ToString();
                            rm.Prev_School_Name = ds.Tables[0].Rows[0]["Prev_School_Name"].ToString();
                            rm.Month = ds.Tables[0].Rows[0]["Month"].ToString();
                            rm.Year = ds.Tables[0].Rows[0]["Year"].ToString();
                            rm.AWRegisterNo = ds.Tables[0].Rows[0]["AWRegisterNo"].ToString();
                            rm.Admission_Num = ds.Tables[0].Rows[0]["Admission_Num"].ToString();
                            rm.Admission_Date = ds.Tables[0].Rows[0]["Admission_Date"].ToString();
                            rm.Class_Roll_Num_Section = ds.Tables[0].Rows[0]["Class_Roll_Num_Section"].ToString();
                            rm.Candi_Name = ds.Tables[0].Rows[0]["Candi_Name"].ToString();
                            rm.Candi_Name_P = ds.Tables[0].Rows[0]["Candi_Name_P"].ToString();
                            rm.Father_Name = ds.Tables[0].Rows[0]["Father_Name"].ToString();
                            rm.Father_Name_P = ds.Tables[0].Rows[0]["Father_Name_P"].ToString();
                            rm.Mother_Name = ds.Tables[0].Rows[0]["Mother_Name"].ToString();
                            rm.Mother_Name_P = ds.Tables[0].Rows[0]["Mother_Name_P"].ToString();
                            rm.Caste = ds.Tables[0].Rows[0]["Caste"].ToString();
                            rm.Gender = ds.Tables[0].Rows[0]["Gender"].ToString();
                            rm.Differently_Abled = ds.Tables[0].Rows[0]["Differently_Abled"].ToString();
                            rm.Religion = ds.Tables[0].Rows[0]["Religion"].ToString();
                            rm.DOB = ds.Tables[0].Rows[0]["DOB"].ToString();
                            rm.Mobile = ds.Tables[0].Rows[0]["Mobile"].ToString();
                            rm.Belongs_BPL = ds.Tables[0].Rows[0]["Belongs_BPL"].ToString();
                            rm.E_punjab_Std_id = ds.Tables[0].Rows[0]["E_punjab_Std_id"].ToString();
                            rm.Aadhar_num = ds.Tables[0].Rows[0]["Aadhar_num"].ToString();
                            rm.MyGroup = ds.Tables[0].Rows[0]["Group_name"].ToString();
                            rm.Address = ds.Tables[0].Rows[0]["Address"].ToString();
                            rm.LandMark = ds.Tables[0].Rows[0]["LandMark"].ToString();
                            rm.Block = ds.Tables[0].Rows[0]["Block"].ToString();
                            rm.MyDistrict = ds.Tables[0].Rows[0]["distE"].ToString();
                            rm.MYTehsil = ds.Tables[0].Rows[0]["tehE"].ToString();
                            rm.PinCode = ds.Tables[0].Rows[0]["PinCode"].ToString();
                            if (ds.Tables[0].Rows[0]["Section"].ToString() != "")
                            {
                                rm.Section = Convert.ToChar(ds.Tables[0].Rows[0]["Section"].ToString());
                            }
                            string formName = ds.Tables[0].Rows[0]["form_Name"].ToString();
                            rm.MetricYear = ds.Tables[0].Rows[0]["MetricYear"].ToString();
                            rm.MetricMonth = ds.Tables[0].Rows[0]["MetricMonth"].ToString();
                            rm.Metric_Roll_Num = ds.Tables[0].Rows[0]["MetricRollNum"].ToString();
                            // @ViewBag.ImageURL = "../../StdImages/Upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();

                            if (ds.Tables[0].Rows[0]["Cat"].ToString() == null || ds.Tables[0].Rows[0]["Cat"].ToString() == "")
                            {
                                //  not imported
                                rm.checkCategory = "";
                                @ViewBag.Photo = "../../upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                            }
                            else
                            {
                                rm.checkCategory = ds.Tables[0].Rows[0]["Cat"].ToString();
                                if (rm.checkCategory == "P")
                                {
                                    if (ds.Tables[0].Rows[0]["StudentUniqueId"].ToString() == null || ds.Tables[0].Rows[0]["StudentUniqueId"].ToString() == "")
                                    {
                                        string Oroll = ds.Tables[0].Rows[0]["OROLL"].ToString();
                                        @ViewBag.Photo = "https://registration2022.pseb.ac.in/Upload2015Matric/" + Oroll + ".jpg";
                                    }
                                    else
                                    {
                                        @ViewBag.Photo = "../../upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                                    }
                                }
                                else if (rm.checkCategory == "F")
                                {
                                    if (ds.Tables[0].Rows[0]["StudentUniqueId"].ToString() == null || ds.Tables[0].Rows[0]["StudentUniqueId"].ToString() == "")
                                    {
                                        string Oroll = ds.Tables[0].Rows[0]["OROLL"].ToString();
                                        @ViewBag.Photo = "https://registration2022.pseb.ac.in/Upload2015/" + Oroll + ".jpg";
                                    }
                                    else
                                    {
                                        @ViewBag.Photo = "../../upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                                    }
                                }
                                else
                                {
                                    @ViewBag.Photo = "../../upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                                }
                            }

                            // 
                            @ViewBag.sign = "../../upload/" + ds.Tables[0].Rows[0]["std_Sign"].ToString();
                            rm.MetricYear = ds.Tables[0].Rows[0]["MetricYear"].ToString();
                            rm.MetricMonth = ds.Tables[0].Rows[0]["MetricMonth"].ToString();
                            rm.Metric_Roll_Num = ds.Tables[0].Rows[0]["MetricRollNum"].ToString();
                            //rm.Registration_num = ds.Tables[0].Rows[0]["Registration_num"].ToString();
                        }
                        else
                        {
                            return RedirectToAction("E1Formgrid", "RegistrationPortal");
                        }
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("E1Formgrid", "RegistrationPortal");
                    }

                }
                //return View(asm);
                return View(rm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }



        /****************Start Allot Reg by Rohit*/

        #region Begin AllotRegNo

        public ActionResult AdminAllotRegNo()
        {
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                //tuple
                //var t = AbstractLayer.StaticDB.GetCurrentSessionYear(Session["Session"].ToString());
                //int CurrentYear = t.Item1;
                //string CurrentSession = t.Item2;
                //


                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsREG = 1; }
                else
                {
                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsREG = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("MIGRATESCHOOL/ALLOTREGNO")).Count();
                    }
                }
                #endregion Action Assign Method
                ViewBag.AdminType = Session["AdminType"].ToString();
                AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
                MigrateSchoolModels MS = new MigrateSchoolModels();                //
                List<SelectListItem> items = new List<SelectListItem>();
                int adminid = Convert.ToInt32(Session["AdminId"].ToString());

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

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "School Code" }, new { ID = "2", Name = "School ID" } }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();

                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult AdminAllotRegNo(FormCollection frm)
        {
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsREG = 1; }
                else
                {

                    string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                    string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsREG = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("MIGRATESCHOOL/AllotRegNo")).Count();
                    }
                }
                #endregion Action Assign Method
                AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
                MigrateSchoolModels MS = new MigrateSchoolModels();
                List<SelectListItem> items = new List<SelectListItem>();
                string distID = frm["SelDist"].ToString();
                MS.SelDist = frm["SelDist"].ToString();
                MS.SelList = frm["SelList"].ToString();
                int adminid = Convert.ToInt32(Session["AdminId"].ToString());
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
                        Search = "District='" + distID + "' ";
                    }
                    else
                    {

                        Search = "District like '%%' ";
                    }
                    if (frm["SelList"] != "")
                    {
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
                    MS.StoreAllData = objDB.SelectForMigrateSchools(Search);
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
                    return SchoolRegProcess(frm);
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [AdminLoginCheckFilter]
        public ActionResult AllotRegNo(FormCollection frm, string id, int? page)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
            {
                return RedirectToAction("Index", "Admin");
            }

            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
            MigrateSchoolModels MS = new MigrateSchoolModels();
            string SCHL = string.Empty;
            try
            {

                SCHL = id;
                DataSet result1 = objDB.GetAllFormName(id);
                ViewBag.AllEmpList = result1.Tables[5];
                ViewBag.MyForm = result1.Tables[1];
                ViewBag.AllErrorList = result1.Tables[2];
                /******* By Rohit Error List */
                List<SelectListItem> ErrorList5 = new List<SelectListItem>();
                List<SelectListItem> ErrorList8 = new List<SelectListItem>();
                List<SelectListItem> ErrorList9 = new List<SelectListItem>();
                List<SelectListItem> ErrorList10 = new List<SelectListItem>();
                List<SelectListItem> ErrorList11 = new List<SelectListItem>();
                List<SelectListItem> ErrorList12 = new List<SelectListItem>();
				List<SelectListItem> ErrorList27 = new List<SelectListItem>();
				foreach (System.Data.DataRow dr in result1.Tables[2].Rows)
                {
                    if (dr["FORM"].ToString() == "F2")
                    {
                        ErrorList5.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "A2")
                    {
                        ErrorList8.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "N2")
                    {
                        ErrorList9.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "M2")
                    {
                        ErrorList10.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "E2")
                    {
                        ErrorList11.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "T2")
                    {
                        ErrorList12.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "AL")
                    {
                        ErrorList9.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                        ErrorList10.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                        ErrorList11.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                        ErrorList12.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
						ErrorList27.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
					}
                }
                ViewBag.ErrorList5 = ErrorList5;
                ViewBag.ErrorList8 = ErrorList8;
                ViewBag.ErrorList9 = ErrorList9;
                ViewBag.ErrorList10 = ErrorList10;
                ViewBag.ErrorList11 = ErrorList11;
                ViewBag.ErrorList12 = ErrorList12;
				ViewBag.ErrorList27 = ErrorList27;

				// MS.ErrorList = ErrorList9;
				/*******/

				List<SelectListItem> empList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.AllEmpList.Rows)
                {
                    empList.Add(new SelectListItem { Text = @dr["EmpDisplayName"].ToString(), Value = @dr["EmpId"].ToString() });
                }
                ViewBag.AllEmpList = empList;
                ViewBag.SelectedEmp = "0";

                List<SelectListItem> FormList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyForm.Rows)
                {
                    FormList.Add(new SelectListItem { Text = @dr["form_name"].ToString(), Value = @dr["form_name"].ToString() });
                }
                ViewBag.MyForm = FormList;


                //var itemLot = new SelectList(new[] { new { ID = "1", Name = "1" }, new { ID = "2", Name = "2" }, new { ID = "3", Name = "3" } }, "ID", "Name", 1);
                //ViewBag.MyLot = itemLot.ToList();

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

                var itemAction = new SelectList(new[] { new { ID = "0", Name = "All" }, new { ID = "1", Name = "Allot Descrepancy" }, new { ID = "2", Name = "Allot Regno" } }, "ID", "Name", 0);
                ViewBag.MyAction = itemAction.ToList();
				


				//------------------------

				string schlid = string.Empty;

                if (id != null)
                {
                    int pageIndex = 1;
                    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                    ViewBag.pagesize = pageIndex;


                    string Search = string.Empty;
                    //Search = "SCHL='" + id + "'";

                    //

                    if (TempData["AllotRegnoSearch"] != null)
                    {
                        Search += TempData["AllotRegnoSearch"].ToString();
                        ViewBag.SelectedFilter = TempData["SelFilter"];
                        ViewBag.SelectedAction = TempData["SelAction"];
                        ViewBag.SelectedForm = TempData["SelForm"];
                        ViewBag.SelectedLot = TempData["SelLot"];
                        ViewBag.SelectedEmp = TempData["SelEmp"];
                        //
                        TempData["AllotRegnoSearch"] = Search;
                        TempData["SelFilter"] = ViewBag.SelectedFilter;
                        TempData["SelAction"] = ViewBag.SelectedAction;
                        TempData["SelForm"] = ViewBag.SelectedForm;
                        TempData["SelLot"] = ViewBag.SelectedLot;
                        TempData["SelEmp"] = ViewBag.SelectedEmp;
                    }
                    else
                    {
                        Search = "SCHL='" + id + "'";
                        ViewBag.SelectedFilter = 0;
                        ViewBag.SelectedAction = 0;
                        ViewBag.SelectedForm = 0;
                        ViewBag.SelectedLot = 0;
                        ViewBag.SelectedEmp = 0;
                    }
                    // 


                    MS.StoreAllData = objDB.GetStudentRegNoNotAlloted(Search, SCHL, pageIndex);

                    ViewBag.schlCode = MS.StoreAllData.Tables[2].Rows[0]["schlCode"].ToString();
                    ViewBag.schlID = MS.StoreAllData.Tables[2].Rows[0]["schlID"].ToString();
                    ViewBag.schlName = MS.StoreAllData.Tables[2].Rows[0]["schlNM"].ToString();

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
                Session["Search"] = null;
                return RedirectToAction("AdminAllotRegNo", "MigrateSchool");
            }

            return View();
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult AllotRegNo(FormCollection frm, int? page)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            string id = string.Empty;
            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
            MigrateSchoolModels MS = new MigrateSchoolModels();
            string schlid = string.Empty;
            string SCHL = string.Empty;
            try
            {
                //schlid = "TUF6NkWFx0oBg-QnwhytFQ%3d%3d";
                //id = encrypt.QueryStringModule.Decrypt(schlid);


                id = frm["SchlCode"].ToString();
                SCHL = id;
                DataSet result1 = objDB.GetAllFormName(id);
                ViewBag.AllEmpList = result1.Tables[5];
                ViewBag.MyForm = result1.Tables[1];
                /******* By Rohit Error List */
                List<SelectListItem> ErrorList5 = new List<SelectListItem>();
                List<SelectListItem> ErrorList8 = new List<SelectListItem>();
                List<SelectListItem> ErrorList9 = new List<SelectListItem>();
                List<SelectListItem> ErrorList10 = new List<SelectListItem>();
                List<SelectListItem> ErrorList11 = new List<SelectListItem>();
                List<SelectListItem> ErrorList12 = new List<SelectListItem>();
				List<SelectListItem> ErrorList27 = new List<SelectListItem>();
				foreach (System.Data.DataRow dr in result1.Tables[2].Rows)
                {
                    if (dr["FORM"].ToString() == "F2")
                    {
                        ErrorList5.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "A2")
                    {
                        ErrorList8.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "N2")
                    {
                        ErrorList9.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "M2")
                    {
                        ErrorList10.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "E2")
                    {
                        ErrorList11.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "T2")
                    {
                        ErrorList12.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "AL")
                    {
                        ErrorList9.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                        ErrorList10.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                        ErrorList11.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                        ErrorList12.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
						ErrorList27.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
					}
                }
                ViewBag.ErrorList5 = ErrorList5;
                ViewBag.ErrorList8 = ErrorList8;
                ViewBag.ErrorList9 = ErrorList9;
                ViewBag.ErrorList10 = ErrorList10;
                ViewBag.ErrorList11 = ErrorList11;
                ViewBag.ErrorList12 = ErrorList12;
                ViewBag.ErrorList27 = ErrorList27;

                // MS.ErrorList = ErrorList9;
                /*******/
                List<SelectListItem> empList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.AllEmpList.Rows)
                {
                    empList.Add(new SelectListItem { Text = @dr["EmpDisplayName"].ToString(), Value = @dr["EmpId"].ToString() });
                }
                ViewBag.AllEmpList = empList;
                ViewBag.SelectedEmp = "0";
                List<SelectListItem> FormList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyForm.Rows)
                {
                    FormList.Add(new SelectListItem { Text = @dr["form_name"].ToString(), Value = @dr["form_name"].ToString() });
                }
                ViewBag.MyForm = FormList;

                MS.SelForm = frm["SelForm"].ToString();
                MS.SelLot = frm["SelLot"].ToString();
                ViewBag.SelLot = frm["SelLot"].ToString();
                MS.SelFilter = frm["SelFilter"].ToString();

                DataSet result2 = objDB.GetAllLot(id);
                ViewBag.MyLot = result2.Tables[0];
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
                return RedirectToAction("AdminAllotRegNo", "MigrateSchool");
            }

            //var itemFilter = new SelectList(new[] { new { ID = "1", Name = "Student Name" }, new { ID = "2", Name = "Roll No" }, }, "ID", "Name", 1);
            var itemFilter = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
            new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
            ViewBag.MyFilter = itemFilter.ToList();

			var itemAction = new SelectList(new[] { new { ID = "0", Name = "All" }, new { ID = "1", Name = "Allot Descrepancy" }, new { ID = "2", Name = "Allot Regno" } }, "ID", "Name", 0);
			ViewBag.MyAction = itemAction.ToList();
			////------------------------
			ViewBag.SelectedFilter = "0";
            ViewBag.SelectedAction = "0";
            ViewBag.SelectedForm = "0";
            ViewBag.SelectedLot = "0";
            ViewBag.SelectedEmp = "0";

            TempData["SelAction"] = "0";
            TempData["SelForm"] = "0";
            TempData["SelLot"] = "0";
            TempData["SelFilter"] = "0";
            TempData["SelEmp"] = "0";

            if (id != null && id != "")
            {
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                string Search = string.Empty;
                Search = "SCHL='" + id + "'";
                if (frm["SelAction"] != "")
                {
                    TempData["SelAction"] = frm["SelAction"];
                    ViewBag.SelectedAction = frm["SelAction"];
                    int SelValueSch = Convert.ToInt32(frm["SelAction"].ToString());
                    if (frm["SelAction"] != "0")
                    {
                        if (SelValueSch == 1)
                        { Search += " and  Registration_num!='' and  Registration_num not like 'BS/23%' "; }
                        else
                        { Search += "  and (Registration_num is not null or Registration_num!='' and Registration_num not like '%ERR%') "; }
                    }
                }



                if (frm["SelForm"] != "")
                {
                    TempData["SelForm"] = frm["SelForm"];
                    ViewBag.SelectedForm = frm["SelForm"];
                    Search += " and form_name='" + frm["SelForm"].ToString() + "' ";
                }
                if (frm["SelLot"] != "")
                {
                    TempData["SelLot"] = frm["SelLot"];
                    ViewBag.SelectedLot = frm["SelLot"];
                    Search += " and LOT='" + frm["SelLot"].ToString() + "' ";
                }
                //else
                //{
                //    ViewBag.SelectedLot = "0";
                //    Search += " and LOT >0 ";
                //}
                if (frm["SelFilter"] != "")
                {
                    TempData["SelFilter"] = frm["SelFilter"];
                    ViewBag.SelectedFilter = frm["SelFilter"];
                    int SelValueSch = Convert.ToInt32(frm["SelFilter"].ToString());
                    if (frm["SelFilter"] != "" && frm["SearchString"].ToString() != "")
                    {
                        ViewBag.SearchString = frm["SearchString"].ToString();
                        if (SelValueSch == 1)
                        { Search += " and Std_id='" + frm["SearchString"].ToString() + "'"; }
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

                // Session["AllotRegnoSearch"] = Search;
                TempData["AllotRegnoSearch"] = Search;

                //[GetStudentRegNoNotAllotedSP]
                MS.StoreAllData = objDB.GetStudentRegNoNotAlloted(Search, SCHL, pageIndex);//GetStudentRegNoNotAllotedSPPaging
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCount = 0;
                    ViewBag.schlCode = id;
                    ViewBag.schlID = id;
                    ViewBag.schlName = MS.StoreAllData.Tables[2].Rows[0]["schlNM"].ToString();
                    //ViewBag.schlName = 0;
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount1 = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.schlCode = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                    ViewBag.schlID = MS.StoreAllData.Tables[0].Rows[0]["IDNO"].ToString();
                    ViewBag.schlName = MS.StoreAllData.Tables[2].Rows[0]["schlNM"].ToString();

                    // ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
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

                    return View(MS);
                }
            }
            return View();
        }

        [AdminLoginCheckFilter]
        public ActionResult ViewOtherBoardAdditionalDocumentsDateWise(string id)
        {
            MigrateSchoolModels spi = new MigrateSchoolModels();

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("AllotRegNo", "MigrateSchool");
                }
                return View(spi);
            }
            catch (Exception ex)
            {
            }
            return View(spi);

        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult ViewOtherBoardAdditionalDocumentsDateWise(string id, string FromDate, string ToDate, MigrateSchoolModels spi)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("AllotRegNo", "MigrateSchool");
                }
                else
                {

                    string Search = "Stdid like '%%' ";
                    if (!string.IsNullOrEmpty(FromDate))
                    {
                        ViewBag.FromDate = FromDate;
                        TempData["FromDate"] = FromDate;
                        Search += " and CONVERT(DATETIME, CONVERT(varchar(10),SubmitOn,103), 103)>=CONVERT(DATETIME, CONVERT(varchar(10),'" + FromDate.ToString() + "',103), 103)";
                    }
                    if (!string.IsNullOrEmpty(ToDate))
                    {
                        ViewBag.ToDate = ToDate;
                        TempData["ToDate"] = ToDate;
                        Search += " and CONVERT(DATETIME, CONVERT(varchar(10),SubmitOn,103), 103)<=CONVERT(DATETIME, CONVERT(varchar(10),'" + ToDate.ToString() + "',103), 103)";
                    }
                    spi.StoreAllData = AbstractLayer.AdminDB.OtherBoardDocumentsBySchoolSP(1, Search);
                    if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
                    }

                    return View(spi);
                }
            }
            catch (Exception ex)
            {
            }
            return View(spi);

        }

        #endregion End AllotRegNo


        // public JsonResult JqErrorAllotRegNo(string storeid)
        [AdminLoginCheckFilter]
        public JsonResult JqAllotRegNo(string storeid, string Action, string selEmp)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            int userid = Convert.ToInt32(Session["AdminId"].ToString());
            //455651(1,2,),103559(9,19,),
            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
            string dee = "1";
            //std(id1,id2),
            storeid = storeid.Remove(storeid.Length - 1);
            //string[] split1 = storeid.Split('^');
            string[] split1 = storeid.Split('$');
            int sCount = split1.Length;
            if (sCount > 0)
            {
                foreach (string s in split1)
                {
                    //180000500()^(test)$180231608()^(rohit)$
                    string[] s2 = s.Split('^');
                    string stdid = s2[0].Split('(')[0];
                    string errid = s2[0].Split('(', ')')[1];
                    string remarks = s2[1].Split('(', ')')[1];

                    //string stdid = s.Split('(')[0];
                    //string errid = s.Split('(', ')')[1];                   
                    if (stdid != "")
                    {
                        dee = objDB.ErrorAllotRegno(stdid, errid, Convert.ToInt32(Action), userid, remarks, selEmp, adminLoginSession.AdminEmployeeUserId);//ErrorAllotRegnoSP
                    }
                }
            }

            return Json(new { dee = dee }, JsonRequestBehavior.AllowGet);
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult JqAllotManualRegNo(string stdid, string regno, string remarks, string selEmp)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

            int userid = Convert.ToInt32(Session["AdminId"].ToString());
            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            //if (string.IsNullOrEmpty(remarks))
            //{
            //    var results = new
            //    {
            //        status = 5
            //    };
            //    return Json(results);
            //}
            else
            {
                int OutStatus = 0;
                DataTable dt = objDB.ManualAllotRegno(stdid, regno, out OutStatus, userid, remarks, selEmp, adminLoginSession.AdminEmployeeUserId);
                var results = new
                {
                    status = OutStatus
                };
                return Json(results);
            }
        }




        [AdminLoginCheckFilter]
        public JsonResult JqApprovedOtherBoardDocumentStudent(string storeid, string Action, string selEmp)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            int userid = Convert.ToInt32(Session["AdminId"].ToString());
            //455651(1,2,),103559(9,19,),
            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
            string dee = "1";
            //std(id1,id2),
            storeid = storeid.Remove(storeid.Length - 1);
            int sCount = storeid.Length;
            if (sCount > 0)
            {
                if (storeid != "")
                {
                    dee = objDB.ApprovedOtherBoardDocumentStudent(storeid, "", Convert.ToInt32(Action), userid, "", selEmp, adminLoginSession.AdminEmployeeUserId);//ErrorAllotRegnoSP
                }
            }

            return Json(new { dee = dee }, JsonRequestBehavior.AllowGet);
        }


        /***********End Allot RegNo**************/


        #region Begin ViewAllotRegNo
        [AdminLoginCheckFilter]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ViewAllotRegNo(FormCollection frm, string id)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }

            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
            MigrateSchoolModels MS = new MigrateSchoolModels();
            string SCHL = string.Empty;
            try
            {
                SCHL = id;
                DataSet result1 = objDB.GetAllFormName(id);
                ViewBag.AllEmpList = result1.Tables[5];
                ViewBag.MyForm = result1.Tables[3];
                ViewBag.AllErrorList = result1.Tables[2];

                List<SelectListItem> empList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.AllEmpList.Rows)
                {
                    empList.Add(new SelectListItem { Text = @dr["EmpDisplayName"].ToString(), Value = @dr["EmpId"].ToString() });
                }
                ViewBag.AllEmpList = empList;
                ViewBag.SelectedEmp = "0";
                /******* By Rohit Error List */
                List<SelectListItem> ErrorList5 = new List<SelectListItem>();
                List<SelectListItem> ErrorList8 = new List<SelectListItem>();
                List<SelectListItem> ErrorList9 = new List<SelectListItem>();
                List<SelectListItem> ErrorList10 = new List<SelectListItem>();
                List<SelectListItem> ErrorList11 = new List<SelectListItem>();
                List<SelectListItem> ErrorList12 = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in result1.Tables[2].Rows)
                {
                    if (dr["FORM"].ToString() == "F2")
                    {
                        ErrorList5.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "A2")
                    {
                        ErrorList8.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "N2")
                    {
                        ErrorList9.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "M2")
                    {
                        ErrorList10.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "E2")
                    {
                        ErrorList11.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "T2")
                    {
                        ErrorList12.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "AL")
                    {
                        ErrorList9.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                        ErrorList10.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                        ErrorList11.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                        ErrorList12.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                }
                ViewBag.ErrorList5 = ErrorList5;
                ViewBag.ErrorList8 = ErrorList8;
                ViewBag.ErrorList9 = ErrorList9;
                ViewBag.ErrorList10 = ErrorList10;
                ViewBag.ErrorList11 = ErrorList11;
                ViewBag.ErrorList12 = ErrorList12;               // MS.ErrorList = ErrorList9;
                /*******/

                List<SelectListItem> FormList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyForm.Rows)
                {
                    FormList.Add(new SelectListItem { Text = @dr["form_name"].ToString(), Value = @dr["form_name"].ToString() });
                }
                ViewBag.MyForm = FormList;


                //var itemLot = new SelectList(new[] { new { ID = "1", Name = "1" }, new { ID = "2", Name = "2" }, new { ID = "3", Name = "3" } }, "ID", "Name", 1);
                //ViewBag.MyLot = itemLot.ToList();

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

                var itemAction = new SelectList(new[] { new { ID = "0", Name = "All" },new { ID = "1", Name = "Allotted Regno" },new { ID = "2", Name = " Error list" }, new { ID = "3", Name = "Descrepancy List" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();

                ViewBag.SelectedFilter = "0";
                ViewBag.SelectedAction = "0";
                ViewBag.SelectedForm = "0";
                ViewBag.SelectedLot = "0";
                //------------------------

                string schlid = string.Empty;

                if (id != null)
                {
                    string Search = string.Empty;
                    Search = "";
                    MS.StoreAllData = objDB.ViewAllotRegNo(Search, SCHL);

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
                        // ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                }
            }
            catch (Exception ex)
            {
                Session["Search"] = null;
                // return RedirectToAction("Index", "Admin");
            }

            return View();
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ViewAllotRegNo(FormCollection frm)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            string id = string.Empty;
            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
            MigrateSchoolModels MS = new MigrateSchoolModels();
            string schlid = string.Empty;
            string SCHL = string.Empty;
            try
            {
                //schlid = "TUF6NkWFx0oBg-QnwhytFQ%3d%3d";
                //id = encrypt.QueryStringModule.Decrypt(schlid);


                id = frm["SchlCode"].ToString();
                SCHL = id;
                DataSet result1 = objDB.GetAllFormName(id);
                ViewBag.AllEmpList = result1.Tables[5];
                ViewBag.MyForm = result1.Tables[3];
                List<SelectListItem> empList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.AllEmpList.Rows)
                {
                    empList.Add(new SelectListItem { Text = @dr["EmpDisplayName"].ToString(), Value = @dr["EmpId"].ToString() });
                }
                ViewBag.AllEmpList = empList;
                ViewBag.SelectedEmp = "0";
                /******* By Rohit Error List */
                List<SelectListItem> ErrorList5 = new List<SelectListItem>();
                List<SelectListItem> ErrorList8 = new List<SelectListItem>();
                List<SelectListItem> ErrorList9 = new List<SelectListItem>();
                List<SelectListItem> ErrorList10 = new List<SelectListItem>();
                List<SelectListItem> ErrorList11 = new List<SelectListItem>();
                List<SelectListItem> ErrorList12 = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in result1.Tables[2].Rows)
                {
                    if (dr["FORM"].ToString() == "F2")
                    {
                        ErrorList5.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "A2")
                    {
                        ErrorList8.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "N2")
                    {
                        ErrorList9.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "M2")
                    {
                        ErrorList10.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "E2")
                    {
                        ErrorList11.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "T2")
                    {
                        ErrorList12.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                    if (dr["FORM"].ToString() == "AL")
                    {
                        ErrorList9.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                        ErrorList10.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                        ErrorList11.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                        ErrorList12.Add(new SelectListItem { Text = @dr["Error_Code"].ToString(), Value = @dr["Error_desc"].ToString() });
                    }
                }
                ViewBag.ErrorList5 = ErrorList5;
                ViewBag.ErrorList8 = ErrorList8;
                ViewBag.ErrorList9 = ErrorList9;
                ViewBag.ErrorList10 = ErrorList10;
                ViewBag.ErrorList11 = ErrorList11;
                ViewBag.ErrorList12 = ErrorList12;               // MS.ErrorList = ErrorList9;
                /*******/

                List<SelectListItem> FormList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyForm.Rows)
                {
                    FormList.Add(new SelectListItem { Text = @dr["form_name"].ToString(), Value = @dr["form_name"].ToString() });
                }
                ViewBag.MyForm = FormList;

                MS.SelForm = frm["SelForm"].ToString();
                MS.SelLot = frm["SelLot"].ToString();
                ViewBag.SelLot = frm["SelLot"].ToString();
                ViewBag.SelLot = frm["SelLot"].ToString();
                MS.SelFilter = frm["SelFilter"].ToString();

                DataSet result2 = objDB.GetAllLot(id);
                ViewBag.MyLot = result2.Tables[0];
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
                //  return RedirectToAction("Index", "Admin");
            }



            //var itemFilter = new SelectList(new[] { new { ID = "1", Name = "Student Name" }, new { ID = "2", Name = "Roll No" }, }, "ID", "Name", 1);
            var itemFilter = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
            new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
            ViewBag.MyFilter = itemFilter.ToList();


			var itemAction = new SelectList(new[] { new { ID = "0", Name = "All" }, new { ID = "1", Name = "Allotted Regno" }, new { ID = "2", Name = " Error list" }, new { ID = "3", Name = "Descrepancy List" }, }, "ID", "Name", 1);
			ViewBag.MyAction = itemAction.ToList();

			ViewBag.SelectedFilter = ViewBag.SelectedFilter;
            ViewBag.SelectedAction = ViewBag.SelectedAction;
            ViewBag.SelectedForm = ViewBag.SelectedForm;
            ViewBag.SelectedLot = ViewBag.SelectedLot;
            ////------------------------

            if (id != null && id != "")
            {
                string Search = string.Empty;
                Search = "a.SCHL='" + id + "'";
                if (frm["SelAction"] != "")
                {
                    ViewBag.SelectedAction = frm["SelAction"];
                    int SelValueSch = Convert.ToInt32(frm["SelAction"].ToString());
                    if (frm["SelAction"] != "" && frm["SelAction"] != "0")
                    {

						if (SelValueSch == 1)
						{ Search += " and  Registration_num !='' and Registration_num not like '%ERR%'"; }
                        else if(SelValueSch == 2)
							{ Search += " and  Registration_num !='' and Registration_num like '%ERR%'"; }
                        else if (SelValueSch == 3)
                        { Search += " and  Registration_num like '%:ERR%'"; }
                    }
                }
                if (frm["SelAction"] == "" || frm["SelAction"] == "0")
                {
                    ViewBag.SelectedAction = "0";
                    //Search += " and  Registration_num not like '%ERR%'";
					Search += "";
				}
                //else { ViewBag.SelectedAction = "0"; }

                if (frm["SelForm"] != "")
                {
                    ViewBag.SelectedForm = frm["SelForm"];
                    Search += " and form_name='" + frm["SelForm"].ToString() + "' ";
                }
                if (frm["SelLot"] != "")
                {
                    ViewBag.SelectedLot = frm["SelLot"];
                    Search += " and LOT='" + frm["SelLot"].ToString() + "' ";
                }
                else
                {
                    Search += " and LOT >0 ";
                }
                if (frm["SelFilter"] != "")
                {
                    ViewBag.SelectedFilter = frm["SelFilter"];
                    int SelValueSch = Convert.ToInt32(frm["SelFilter"].ToString());
                    if (frm["SelFilter"] != "" && frm["SearchString"].ToString() != "")
                    {
                        //if (frm["SelFilter"] == "1") {Search += "and Candi_Name like '" + frm["SearchString"].ToString() + "%' ";}
                        //else if (frm["SelFilter"] == "2") {Search += "and OROLL='" + frm["SearchString"].ToString() + "' ";}

                        if (SelValueSch == 1)
                        { Search += " and Std_id='" + frm["SearchString"].ToString() + "'"; }
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

                //[GetStudentRegNoNotAllotedSP]
                MS.StoreAllData = objDB.ViewAllotRegNo(Search, SCHL);//ViewAllotRegNoSP
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.schlCode = id;
                    ViewBag.schlID = id;
                    ViewBag.schlName = MS.StoreAllData.Tables[1].Rows[0]["schlNM"].ToString();

                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.schlCode = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                    ViewBag.schlID = MS.StoreAllData.Tables[0].Rows[0]["IDNO"].ToString();
                    ViewBag.schlName = MS.StoreAllData.Tables[1].Rows[0]["schlNM"].ToString();
                    return View(MS);
                }
            }
            return View();
        }
        #endregion End AllotRegNo

        [AdminLoginCheckFilter]
        public JsonResult JqRemoveRegNo(string storeid, string Action, string selEmp)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

            string dee = "";
            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
            int userid = Convert.ToInt32(Session["AdminId"].ToString());
            storeid = storeid.Remove(storeid.Length - 1);
            // string[] split1 = storeid.Split('^');
            string[] split1 = storeid.Split(',');
            int sCount = split1.Length;
            if (sCount > 0)
            {
                foreach (string s in split1)
                {
                    string stdid = s.ToString();
                    //string errid = s.Split('(', ')')[1];
                    if (stdid != "")
                    {
                        string AdminEmployeeUserId = adminLoginSession.AdminEmployeeUserId;
                       
                        try
                        {
							dee = objDB.RemoveRegno(stdid, Convert.ToInt16(Action), userid, selEmp, AdminEmployeeUserId);

						}
                        catch(Exception ex)
                        {
                            
                        }


						//dee = objDB.RemoveRegno(stdid, Convert.ToInt16(Action), userid, selEmp, AdminEmployeeUserId);
					}
                }
            }
            string Message = "";
			if (Convert.ToInt32(Action) == 0)
			{
				Message = "Registration Number Removed Successfully";
			}
			if (Convert.ToInt32(Action) == 1)
            {
                Message = "Registration Number Removed Successfully";
            }
            if (Convert.ToInt32(Action) == 2)
            {
                Message = "Error Removed Successfully";
            }
            if (Convert.ToInt32(Action) == 3)
            {
                Message = "Descrepancy Removed Successfully";
            }
            return Json(new { dee = Message }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult SendReg(string Schl, string Act)
        {
            if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
            MigrateSchoolModels MS = new MigrateSchoolModels();
            try
            {
                DataSet ds = new DataSet();
                ds = objCommon.SearchEmailID(Schl);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        string SchoolNameWithCode = ds.Tables[0].Rows[0]["SCHLE"].ToString() + "(" + Schl + ")";
                        //  string to = ds.Tables[0].Rows[0]["EMAILID"].ToString();
                        string to = "rohit.nanda@ethical.in";
                        if (Act == "E")
                        {
                            //  string Search = string.Empty;
                            //  Search = "";
                            //  MS.StoreAllData = objDB.ViewAllotRegNo(Search, Schl);

                            ////string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Dear " + SchoolNameWithCode + "</b>,</td></tr><tr><td height=" + 30 + ">Registration No for Form's (N2/M2/E2/T2) of Schl Code " + Schl + " is Updated on " + DateTime.Now.ToString("dd/MM/yyyy") + "</b>.</td></tr><tr><td><b>List of Students is attached with the email:-</b><br /><b>Show Grid </b> " + 1 + "<br /><br /></td></tr><tr><td height=" + 30 + "></td></tr><tr><td><b>Note:</b> For Detail Kindly Check your School Login Under->Registration Portal->View Registration of N2/M2/E2/T2</td></tr><tr><td></td></tr><tr><td><b><i>Thanks & Regards</b><i>,<br /> Registration Branch, <br />Punjab School Education Board Mohali<br /></td></tr>";
                            string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Dear " + SchoolNameWithCode + "</b>,</td></tr><tr><td height=" + 30 + ">Registration No for Form's (N2/M2/E2/T2) of Schl Code " + Schl + " is Updated on " + DateTime.Now.ToString("dd/MM/yyyy") + "</b>.</td></tr><tr><td height=" + 10 + "></td></tr><tr><td><b>Note:</b> For Detail Kindly Check your School Login Under->Registration Portal->View Registration of N2/M2/E2/T2</td></tr><tr><td></td></tr><tr><td><b><i>Thanks & Regards</b><i>,<br /> Registration Branch, <br />Punjab School Education Board Mohali<br /></td></tr>";
                            //DataSet dsEmail = new DataSet();
                            //dsEmail = MS.StoreAllData;                          
                            //body = "<table>";
                            //foreach (DataRow Title in dsEmail.Tables[0].Rows)
                            //{
                            //    body += "<tr>";
                            //    body += "<td>" + Title[0] + "</td>";
                            //    body += "<td>" + String.Format("{0:c}", Title[1])
                            //       + "</td>";
                            //    body += "</tr>";
                            //}
                            //body += "</table>";


                            string subject = "PSEB-View Registration of N2/M2/E2/T2";
                            bool result = objCommon.mail(subject, body, to);
                            if (result == true)
                            {
                                ViewData["result"] = "1";
                                ViewBag.Message = "Email Sent Successfully....";
                                // ModelState.Clear();
                            }
                            else
                            {
                                ViewData["result"] = "0";
                                ViewBag.Message = "Email Not Sent....";
                            }
                        }
                        else if (Act == "S")
                        {
                            string Mobile = ds.Tables[0].Rows[0]["Mobile"].ToString();
                            string Sms = "Registration No for Form's (N2/M2/E2/T2) of Schl Code " + Schl + " is Updated on " + DateTime.Now.ToString("dd/MM/yyyy") + ".For Detail Kindly Check your School Login Under->Registration Portal->View Registration of N2/M2/E2/T2.";
                            if (Mobile != "0" || Mobile != "")
                            {
                                // string getSms = objCommon.gosms(Mobile, Sms);
                                string getSms = objCommon.gosms("9711819184", Sms);
                                if (getSms != "")
                                {
                                    ViewData["result"] = "1";
                                    ViewBag.Message = "SMS Sent Successfully....";
                                    // ModelState.Clear();
                                }
                                else
                                {
                                    ViewData["result"] = "0";
                                    ViewBag.Message = "SMS Not Sent....";
                                }
                            }
                        }
                        else
                        {
                            ViewData["result"] = "-1";
                            ViewBag.Message = "Invalid Action ...";
                        }

                    }
                    else
                    {
                        ViewData["result"] = "-2";
                        ViewBag.Message = "School Not Found....";
                    }
                }

            }
            catch (Exception)
            {
            }
            return RedirectToAction("ViewAllotRegNo", "MigrateSchool", new { id = Schl });
        }



        public JsonResult jqSendReg(string Schl, string Act)
        {
            string status = "";
            AbstractLayer.MigrateSchoolDB objDB = new AbstractLayer.MigrateSchoolDB();
            MigrateSchoolModels MS = new MigrateSchoolModels();
            try
            {
                DataSet ds = new DataSet();
                ds = objCommon.SearchEmailID(Schl);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        string SchoolNameWithCode = ds.Tables[0].Rows[0]["SCHLE"].ToString() + "(" + Schl + ")";
                        string to = ds.Tables[0].Rows[0]["EMAILID"].ToString();
                        //  string to = "rohit.nanda@ethical.in";
                        if (Act == "E")
                        {
                            if (to != "")
                            {
                                string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Dear " + SchoolNameWithCode + "</b>,</td></tr><tr><td height=" + 10 + ">Registration No for Form's (N2/M2/E2/T2) of Schl Code " + Schl + " is Updated on " + DateTime.Now.ToString("dd/MM/yyyy") + "</b>.</td></tr><tr><td height=" + 10 + "></td></tr><tr><td><b>Note:</b> For Detail Kindly Check your School Login Under->Registration Portal->View Registration of N2/M2/E2/T2</td></tr><tr><td></td></tr><tr><td><b><i>Thanks & Regards</b><i>,<br /> Registration Branch, <br />Punjab School Education Board Mohali<br /></td></tr>";
                                string subject = "PSEB-View Registration of N2/M2/E2/T2";
                                bool result = objCommon.mail(subject, body, to);
                                if (result == true)
                                {
                                    status = "1";
                                }
                            }
                        }
                        else if (Act == "S")
                        {
                            string Mobile = ds.Tables[0].Rows[0]["Mobile"].ToString();
                            string Sms = "Registration No for Form's (N2/M2/E2/T2) of Schl Code " + Schl + " is Updated on " + DateTime.Now.ToString("dd/MM/yyyy") + ".For Detail Kindly Check your School Login Under->Registration Portal->View Registration of N2/M2/E2/T2.";
                            if (Mobile != "0" || Mobile != "")
                            {
                                string getSms = objCommon.gosms(Mobile, Sms);
                                //  string getSms = objCommon.gosms("9711819184", Sms);
                                if (getSms != "")
                                {
                                    status = "1";
                                }
                            }
                        }

                    }
                }

            }
            catch (Exception)
            {
            }
            return Json(new { status = status }, JsonRequestBehavior.AllowGet);
        }
    }
}