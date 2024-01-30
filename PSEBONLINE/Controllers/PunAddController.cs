using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSEBONLINE.Models;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Globalization;
using System.Data.OleDb;
using PSEBONLINE.Filters;
using PsebPrimaryMiddle.Controllers;
using CCA.Util;
using System.Configuration;
using Amazon.S3;
using Amazon;
using Amazon.S3.IO;
using Amazon.S3.Transfer;
namespace PSEBONLINE.Controllers
{

    public class PunAddController : Controller
    {
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
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.HomeDB objDBHome = new AbstractLayer.HomeDB();
        AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
        private HomeModels Hm = new HomeModels();
        private const string BUCKET_NAME = "psebdata";

        public string stdPic, stdSign;
        // GET: PrivateCandidate
        public ActionResult Index()
        {
            return View();
        }
        #region Punjabi Additional Admin Pannel
        public ActionResult AdminPunAddHome()
        {
            try
            {
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Admin");
            }
        }
        public ActionResult PunAddRoll()
        {
            try
            {
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }

                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsModiFy = 1; ViewBag.IsView = 1; }
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
                        ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("PUNADD/ADMINCONFIRMATIONVIEW")).Count();
                        ViewBag.IsModiFy = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("PUNADD/ADMINCONFIRMATIONEDIT")).Count();
                    }
                }
                #endregion Action Assign Method


                AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
                PunAddModels MS = new PunAddModels();

                DataSet result = objDB.GetPunAddbatchYear(); // passing Value to DBClass from model
                ViewBag.MybatchYear = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MybatchYear.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["batchYear"].ToString(), Value = @dr["batchYear"].ToString() });
                }
                ViewBag.MybatchYear = new SelectList(items, "Value", "Text");
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult PunAddRoll(FormCollection frm, int? page, string cmd)
        {
            try
            {
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsModiFy = 1; ViewBag.IsView = 1; }
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
                        ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("PUNADD/ADMINCONFIRMATIONVIEW")).Count();
                        ViewBag.IsModiFy = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("PUNADD/ADMINCONFIRMATIONEDIT")).Count();
                    }
                }
                #endregion Action Assign Method
                AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
                PunAddModels MS = new PunAddModels();

                DataSet result = objDB.GetPunAddbatchYear(); // passing Value to DBClass from model
                ViewBag.MybatchYear = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MybatchYear.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["batchYear"].ToString(), Value = @dr["batchYear"].ToString() });
                }
                ViewBag.MybatchYear = new SelectList(items, "Value", "Text");

                MS.batchYear = frm["batchYear"].ToString();
                MS.SearchBy = frm["SearchBy"].ToString();
                MS.SearchString = frm["SearchString"].ToString();
                //ViewBag.MybatchYear = new SelectList(items, "Value", "Text");                
                ViewBag.MybatchYear = items.ToList();

                if (ModelState.IsValid)
                {
                    string Search = "refno is not null  ";
                    if (MS.batchYear != "")
                    {
                        Search += " and batch='" + MS.batchYear.Substring(0, 1) + "' and batchYear='" + MS.batchYear.Substring(2, 4) + "'";
                    }
                    if (MS.SearchBy != "0")
                    {
                        switch (MS.SearchBy)
                        {
                            case "refno": Search += " and refno='" + (MS.SearchString).Trim() + "'"; break;
                            case "roll": Search += " and roll='" + (MS.SearchString).Trim() + "'"; break;
                            case "name": Search += " and name like '%" + (MS.SearchString).Trim() + "%'"; break;
                            case "fname": Search += " and fname like '%" + (MS.SearchString).Trim() + "%'"; break;
                            case "mname": Search += " and mname like '%" + (MS.SearchString).Trim() + "%'"; break;
                            case "mobile": Search += " and mobile='" + (MS.SearchString).Trim() + "'"; break;
                            case "Rno": Search += " and Rno='" + (MS.SearchString).Trim() + "'"; break;
                        }
                    }
                    if (cmd == "Generate Roll No")
                    {
                        DataSet ds = objDB.GenerateRollPunAdd(Search, MS.batchYear.Substring(0, 1), MS.batchYear.Substring(2, 4));
                    }
                    MS.StoreAllData = objDB.GetPunAddRecordByBatch(Search, pageIndex);
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;

                    if (ViewBag.TotalCount > 0 && cmd == "Dwld Data")
                    {
                        DataSet ds = MS.StoreAllData;//objDB.GetPunAddRecordByBatch(Search, pageIndex);
                        DataTable dt = ds.Tables[0];
                        //string fname = DateTime.Now.ToString("ddMMyyyyHHmm");
                        //Response.Clear();
                        //Response.Buffer = true;
                        //Response.Charset = "";
                        //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        //Response.AddHeader("content-disposition", "attachment;filename=PbiAdd" + fname + ".xlsx");
                        //using (MemoryStream MyMemoryStream = new MemoryStream())
                        //{
                        //    XLWorkbook wb = new XLWorkbook();

                        //    var WS = wb.Worksheets.Add(dt, "Data" + fname);
                        //    WS.Tables.FirstOrDefault().ShowAutoFilter = false;
                        //    wb.SaveAs(MyMemoryStream);
                        //    MyMemoryStream.WriteTo(Response.OutputStream);
                        //    WS.AutoFilter.Enabled = false;
                        //    Response.Flush();
                        //    Response.End();
                        //}
                        objDB.DownloadFiles(Search);
                    }
                    if (ViewBag.TotalCount > 0 && cmd == "Dwld Image")
                    {
                        objDB.DownloadFilesImage(Search);

                    }

                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        MS.SearchString = string.Empty;
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {


                        //ViewBag.TotalCount1 = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["decount"]);
                        //int tp = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["decount"]);
                        //int pn = tp / 30;
                        //int cal = 30 * pn;
                        //int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        //if (res >= 1)
                        //    ViewBag.pn = pn + 1;
                        //else
                        //    ViewBag.pn = pn;
                        return View(MS);
                    }
                }
                else
                {
                    return PunAddRoll();
                }
            }
            catch (Exception ex)
            {
                return PunAddRoll();
                //return RedirectToAction("Index", "Admin");
            }
        }

        [AdminLoginCheckFilter]
        public ActionResult AdminConfirmationView(string id)
        {
            string Oroll = string.Empty;
            try
            {
                string refno = id;
                Session["refno"] = refno;
                if (Session["refno"] == null || Session["refno"].ToString() == "")
                {
                    return RedirectToAction("PunAddExamination", "PunAdd");
                }
                AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
                PunAddModels MS = new PunAddModels();
                //Oroll = Session["Oroll"].ToString();

                if (refno != null && refno != "")
                {
                    MS.StoreAllData = objDB.GetPunAddConfirmation(refno);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        return View();
                    }
                    else
                    {
                        MS.IsHardCopyCertificate = MS.StoreAllData.Tables[0].Rows[0]["IsHardCopyCertificate"].ToString();
                        MS.Board = MS.StoreAllData.Tables[0].Rows[0]["boardnm"].ToString();
                        MS.Other_Board = MS.StoreAllData.Tables[0].Rows[0]["OtherBoard"].ToString();

                        MS.batch = MS.StoreAllData.Tables[0].Rows[0]["batchMonth"].ToString();
                        MS.batchYear = MS.StoreAllData.Tables[0].Rows[0]["batchYear"].ToString();
                        //MS.Result = MS.StoreAllData.Tables[0].Rows[0]["Resultdtl"].ToString();
                        //MS.MatricMarks = MS.StoreAllData.Tables[0].Rows[0]["Resultdtl"].ToString().IndexOf('(',')');
                        string intval = MS.StoreAllData.Tables[0].Rows[0]["Resultdtl"].ToString();
                        MS.Result = intval.Substring(0, 4);// "PASS";
                        MS.MatricMarks = intval.Replace("PASS(", "").Replace(")", "");

                        MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["rp"].ToString();
                        if (MS.Exam_Type == "P")
                        {
                            MS.Exam_Type = "Private";
                        }

                        MS.category = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                        if (MS.category == "A")
                        {
                            MS.category = "Additional subject";
                        }
                        if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                        {
                            //Session["form"] = "Punjabi_Add_Spl_Jan";
                            Session["form"] = MS.StoreAllData.Tables[0].Rows[0]["feecat"].ToString();
                        }

                        MS.Class = MS.StoreAllData.Tables[0].Rows[0]["class"].ToString();
                        if (MS.Class == "10")
                        {
                            MS.Class = "Matriculation";
                            MS.MatricSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                            MS.MatricSub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                        }
                        MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                        MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
                        MS.Session = MS.SelMonth + ' ' + MS.SelYear;
                        MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                        Session["OROLL"] = MS.OROLL;
                        Session["category"] = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                        MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();
                        MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();
                        MS.refNo = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                        MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                        MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                        MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();
                        MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();
                        //MS.Result = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();
                        MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
                        MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
                        MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();
                        MS.EPname = MS.Candi_Name + '/' + MS.Pname;
                        MS.EPFname = MS.Father_Name + '/' + MS.PFname;
                        MS.EPMname = MS.Mother_Name + '/' + MS.PMname;
                        MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["sex"].ToString();
                        MS.CastList = MS.StoreAllData.Tables[0].Rows[0]["caste"].ToString();
                        MS.Area = MS.StoreAllData.Tables[0].Rows[0]["area"].ToString();
                        if (MS.Area == "U")
                        {
                            MS.Area = "Urban";
                        }
                        if (MS.Area == "R")
                        {
                            MS.Area = "Rural";
                        }
                        MS.Relist = MS.StoreAllData.Tables[0].Rows[0]["religion"].ToString();
                        if (MS.Relist == "H")
                        {
                            MS.Relist = "Hindu";
                        }
                        if (MS.Relist == "M")
                        {
                            MS.Relist = "Muslim";
                        }
                        if (MS.Relist == "S")
                        {
                            MS.Relist = "Sikh";
                        }
                        if (MS.Relist == "C")
                        {
                            MS.Relist = "Christian";
                        }
                        if (MS.Relist == "O")
                        {
                            MS.Relist = "Others";
                        }
                        MS.phyChal = MS.StoreAllData.Tables[0].Rows[0]["phy_chal"].ToString();
                        MS.rdoWantWriter = "0";
                        MS.IsPracExam = MS.StoreAllData.Tables[0].Rows[0]["prac"].ToString();
                        if (MS.IsPracExam == "True")
                        {
                            MS.IsPracExam = "1";
                        }
                        if (MS.IsPracExam == "False")
                        {
                            MS.IsPracExam = "0";
                        }
                        MS.address = MS.StoreAllData.Tables[0].Rows[0]["homeadd"].ToString();
                        MS.landmark = MS.StoreAllData.Tables[0].Rows[0]["LandMark"].ToString();
                        MS.block = MS.StoreAllData.Tables[0].Rows[0]["block"].ToString();
                        MS.SelExamDist = MS.StoreAllData.Tables[0].Rows[0]["dist"].ToString();
                        MS.SelDist = MS.StoreAllData.Tables[0].Rows[0]["homedistco"].ToString();
                        MS.tehsil = MS.StoreAllData.Tables[0].Rows[0]["tehsil"].ToString();
                        MS.pinCode = MS.StoreAllData.Tables[0].Rows[0]["pincode"].ToString();
                        DataSet result = objDB.SelectDist(); // passing Value to DBClass from model
                        ViewBag.MyDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> items = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                        {
                            items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                        }
                        // ViewBag.MyDist = items;
                        ViewBag.MyDist = new SelectList(items, "Value", "Text");

                        if (MS.SelDist == "")
                        {
                            MS.SelDist = "0";
                        }

                        ViewBag.MyExamDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> items1 = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyExamDist.Rows)
                        {
                            items1.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                        }
                        ViewBag.MyExamDist = new SelectList(items1, "Value", "Text");

                        if (MS.SelExamDist == "")
                        {
                            MS.SelExamDist = "0";
                        }

                        int dist = Convert.ToInt32(MS.SelDist);
                        DataSet result1 = objDB.SelectAllTehsil(dist);
                        ViewBag.MyTehsil = result1.Tables[0];
                        List<SelectListItem> TehList = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
                        {

                            TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                        }
                        ViewBag.MyTehsil = TehList;

                        if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                        {
                            MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                            MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                        }
                        @ViewBag.Photo = "../../upload/Upload2023/" + MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
                        @ViewBag.sign = "../../upload/Upload2023/" + MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();

                        MS.imgSign = MS.StoreAllData.Tables[0].Rows[0]["Sign_url"].ToString();
                        MS.imgPhoto = MS.StoreAllData.Tables[0].Rows[0]["Photo_url"].ToString();

                        Session["imgPhoto"] = MS.imgPhoto;
                        Session["imgSign"] = MS.imgSign;


                        MS.FormStatus = MS.StoreAllData.Tables[0].Rows[0]["FormStatus"].ToString();
                        return View(MS);
                    }
                }
                else
                {
                    return RedirectToAction("PunAddExamination", "PunAdd");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("PunAddExamination", "PunAdd");
            }
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult AdminConfirmationView(PunAddModels MS, FormCollection frm, HttpPostedFileBase eligibilitydoc1, HttpPostedFileBase eligibilitydoc2)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            try
            {
                string refno = frm["refno"].ToString();
                Session["refno"] = refno;
                if (Session["refno"] == null || Session["refno"].ToString() == "")
                {
                    return RedirectToAction("PunAddExamination", "PunAdd");
                }
                AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();

                DataSet result = objDB.SelectDist(); // passing Value to DBClass from model
                ViewBag.MyDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }
                ViewBag.MyDist = new SelectList(items, "Value", "Text");
                MS.Board = frm["Board"].ToString();
                if (MS.Board == "OTHER BOARD")
                {
                    MS.Other_Board = frm["Other_Board"];
                }
                else
                {
                    MS.Other_Board = null;
                }
                MS.Result = frm["Result"].ToString();
                MS.MatricMarks = frm["MatricMarks"].ToString();


                MS.Exam_Type = frm["Exam_Type"].ToString();
                MS.refNo = frm["refNo"].ToString();
                MS.Session = frm["Session"].ToString();
                //MS.Result = "";
                MS.Class = "10";
                MS.DOB = frm["DOB"].ToString();
                MS.Candi_Name = frm["Candi_Name"].ToString();
                MS.Father_Name = frm["Father_Name"].ToString();
                MS.Mother_Name = frm["Mother_Name"].ToString();

                MS.Pname = frm["PNAME"].ToString();
                MS.PFname = frm["PFNAME"].ToString();
                MS.PMname = frm["PMNAME"].ToString();
                MS.Board = frm["board"].ToString();
                if (MS.Board.Contains("P.S.E.B"))
                {
                    MS.RegNo = frm["RegNo"].ToString();
                }
                else
                {
                    MS.RegNo = null;
                }
                MS.OROLL = frm["OROLL"].ToString();
                Session["OROLL"] = MS.OROLL;
                Session["refno"] = MS.refNo;
                MS.mobileNo = frm["mobileNo"].ToString();
                MS.emailID = frm["emailID"].ToString();
                MS.Gender = frm["Gender"].ToString();
                MS.CastList = frm["CastList"].ToString();
                MS.Area = frm["Area"].ToString();
                if (MS.Area == "Urban")
                {
                    MS.Area = "U";
                }
                if (MS.Area == "Rural")
                {
                    MS.Area = "R";
                }
                MS.Relist = frm["Relist"].ToString();
                if (MS.Relist == "Hindu")
                {
                    MS.Relist = "H";
                }
                if (MS.Relist == "Muslim")
                {
                    MS.Relist = "M";
                }
                if (MS.Relist == "Sikh")
                {
                    MS.Relist = "S";
                }
                if (MS.Relist == "Christian")
                {
                    MS.Relist = "C";
                }
                if (MS.Relist == "Others")
                {
                    MS.Relist = "O";
                }

                MS.IsphysicalChall = frm["phyChal"].ToString();
                MS.rdoWantWriter = "0";
                MS.address = frm["address"].ToString();
                MS.landmark = frm["landmark"].ToString();
                MS.block = frm["block"].ToString();
                MS.SelDist = frm["SelDist"].ToString();

                int dist = Convert.ToInt32(MS.SelDist);
                DataSet result1 = objDB.SelectAllTehsil(dist);
                ViewBag.MyTehsil = result1.Tables[0];
                List<SelectListItem> TehList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
                {
                    TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });
                }
                ViewBag.MyTehsil = TehList;

                MS.tehsil = frm["tehsil"].ToString();
                MS.pinCode = frm["pinCode"].ToString();
                MS.category = frm["category"].ToString();

                if (MS.category == "Additional subject")
                {
                    MS.category = "A";
                }
                if (MS.category == "A" || MS.category == "D")
                {
                    MS.IsPracExam = "0";
                }
                else
                {
                    MS.IsPracExam = frm["IsPracExam"].ToString();
                    if (MS.IsPracExam == "Yes")
                    {
                        MS.IsPracExam = "1";
                    }
                    else
                    {
                        MS.IsPracExam = "0";
                    }
                }

                if (MS.std_Photo != null)
                {

                    string Orgfile = MS.refNo + "P" + ".jpg";
                    MS.PathPhoto = "PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/Photo/" + MS.refNo + "P" + ".jpg";
                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            ///file.CopyTo(newMemoryStream);

                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = MS.std_Photo.InputStream,
                                Key = string.Format("allfiles/Upload2023/PunAdd/Batch" + MS.refNo.Substring(3, 4) + "/Photo/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }
                }
                if (MS.std_Sign != null)
                {
                    string Orgfile = MS.refNo + "S" + ".jpg";
                    MS.PathPhoto = "PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/Photo/" + MS.refNo + "S" + ".jpg";
                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            ///file.CopyTo(newMemoryStream);

                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = MS.std_Photo.InputStream,
                                Key = string.Format("allfiles/Upload2023/PunAdd/Batch" + MS.refNo.Substring(3, 4) + "/Sign/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }
                }
                MS.imgPhoto = Session["imgPhoto"].ToString();
                MS.imgSign = Session["imgSign"].ToString();



                if ((MS.std_Photo == null || MS.std_Sign == null) && (MS.imgPhoto == null || MS.imgSign == null))
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Select Photo and Sign";
                    ViewData["SelectPhotoSign"] = "0";
                    return View(MS);
                }


                if (MS.Gender == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Select Gender";
                    ViewData["SelectGender"] = "0";
                    return View(MS);
                }

                if (MS.CastList == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Select Cast";
                    ViewData["SelectCast"] = "0";
                    return View(MS);
                }
                if (MS.Area == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Select Area";
                    ViewData["SelectArea"] = "0";
                    return View(MS);
                }
                if (MS.Relist == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Select Religion";
                    ViewData["SelectRelist"] = "0";
                    return View(MS);
                }
                if (MS.address == "")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Enter Address";
                    ViewData["Selectaddress"] = "0";
                    return View(MS);
                }
                if (MS.SelDist == "" || MS.tehsil == "" || MS.tehsil == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Select District & Tehsil";
                    ViewData["SelectDist"] = "0";
                    return View(MS);
                }
                if (MS.pinCode == "" || MS.pinCode.Count() != 6)
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Enter Pin Code";
                    ViewData["SelectPin"] = "0";
                    return View(MS);
                }


                if (eligibilitydoc1 != null)
                {
                    //string fname = Path.GetFileName(eligibilitydoc1.FileName);
                    //string fext = Path.GetExtension(eligibilitydoc1.FileName);
                    //string Filepath = Server.MapPath("~/Upload/Upload2023/PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/EligibilityDocument/");
                    //if (!Directory.Exists(Filepath))
                    //{
                    //    Directory.CreateDirectory(Filepath);
                    //}
                    //string pathPhoto = Path.Combine(Server.MapPath("~/Upload/Upload2023/PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/EligibilityDocument/"), MS.refNo + "D1" + fext);
                    //MS.EligibilityDoc1 = "PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/EligibilityDocument/" + MS.refNo + "D1" + fext;
                    //eligibilitydoc1.SaveAs(pathPhoto);

                    string Orgfile = MS.refNo + "D1" + ".jpg";
                    MS.PathPhoto = "PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/EligibilityDocument/" + MS.refNo + "D1" + ".jpg";
                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            ///file.CopyTo(newMemoryStream);

                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = MS.std_Photo.InputStream,
                                Key = string.Format("allfiles/Upload2023/PunAdd/Batch" + MS.refNo.Substring(3, 4) + "/EligibilityDocument/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }
                }
                else
                {

                    MS.EligibilityDoc1 = frm["EligibilityDoc1"];
                }

                if (eligibilitydoc2 != null)
                {
                    string Orgfile = MS.refNo + "D2" + ".jpg";
                    MS.PathPhoto = "PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/EligibilityDocument/" + MS.refNo + "D2" + ".jpg";
                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            ///file.CopyTo(newMemoryStream);

                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = MS.std_Photo.InputStream,
                                Key = string.Format("allfiles/Upload2023/PunAdd/Batch" + MS.refNo.Substring(3, 4) + "/EligibilityDocument/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }
                }
                else
                {
                    MS.EligibilityDoc2 = frm["EligibilityDoc2"];
                }


                MS.AdminId = adminLoginSession.AdminId.ToString();
                MS.EmpUserId = adminLoginSession.AdminEmployeeUserId;
                string OutError = "";
                DataSet result2 = AbstractLayer.PunAddDB.AdminInsertPrivateCandidateConfirmation(MS, out OutError);
                if (OutError == "1")
                {
                    ViewData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    ViewData["Status"] = "3";
                    ViewData["OutError"] = "";
                }
                else
                {
                    ViewData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    ViewData["Status"] = "ERR";
                    ViewData["OutError"] = OutError;
                }

                return View(frm);
            }
            catch (Exception ex)
            {
                return PunAddRoll();
            }
        }

        //public ActionResult AdminConfirmationEdit(string id)
        //{
        //    try
        //    {
        //        string refno = id;
        //        Session["refno"] = refno;
        //        AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
        //        PunAddModels MS = new PunAddModels();

        //        DataSet result = objDB.SelectDist(); // passing Value to DBClass from model               
        //        if (Session["refno"].ToString() == null || Session["refno"].ToString() == "")
        //        {
        //            return PunAddRoll();
        //        }
        //        //string Oroll = Session["Oroll"].ToString();
        //        string RefNo = Session["refno"].ToString();
        //        //MS.OROLL = Oroll;
        //        MS.refNo = RefNo;
        //        int result1 = 1;//objDB.EditPunAddConfirmation(MS);
        //        if (result1 == 1)
        //        {
        //            MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
        //            if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
        //            {
        //                ViewBag.Message = "Record Not Found";
        //                return View();
        //            }
        //            else
        //            {

        //                if (MS.StoreAllData.Tables[0].Rows[0]["EligibilityDoc1"] != null)
        //                {
        //                    MS.EligibilityDoc1 = MS.StoreAllData.Tables[0].Rows[0]["EligibilityDoc1"].ToString();
        //                }
        //                if (MS.StoreAllData.Tables[0].Rows[0]["EligibilityDoc2"] != null)
        //                {
        //                    MS.EligibilityDoc2 = MS.StoreAllData.Tables[0].Rows[0]["EligibilityDoc2"].ToString();
        //                }

        //                MS.IsHardCopyCertificate = MS.StoreAllData.Tables[0].Rows[0]["IsHardCopyCertificate"].ToString();

        //                MS.Board = MS.StoreAllData.Tables[0].Rows[0]["boardnm"].ToString();
        //                MS.Other_Board = MS.StoreAllData.Tables[0].Rows[0]["OtherBoard"].ToString();

        //                MS.batch = MS.StoreAllData.Tables[0].Rows[0]["batchMonth"].ToString();
        //                MS.batchYear = MS.StoreAllData.Tables[0].Rows[0]["batchYear"].ToString();
        //                //MS.Result = MS.StoreAllData.Tables[0].Rows[0]["Resultdtl"].ToString();
        //                //MS.MatricMarks = MS.StoreAllData.Tables[0].Rows[0]["Resultdtl"].ToString().IndexOf('(',')');
        //                string intval = MS.StoreAllData.Tables[0].Rows[0]["Resultdtl"].ToString();
        //                MS.Result = intval.Substring(0, 4);// "PASS";
        //                MS.MatricMarks = intval.Replace("PASS(", "").Replace(")", "");

        //                //MS.Board = MS.StoreAllData.Tables[0].Rows[0]["boardnm"].ToString();
        //                MS.batch = MS.StoreAllData.Tables[0].Rows[0]["batchMonth"].ToString();
        //                MS.batchYear = MS.StoreAllData.Tables[0].Rows[0]["batchYear"].ToString();
        //                // MS.Result = MS.StoreAllData.Tables[0].Rows[0]["Resultdtl"].ToString();

        //                MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["rp"].ToString();
        //                if (MS.Exam_Type == "P")
        //                {
        //                    MS.Exam_Type = "Private";
        //                }
        //                MS.category = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
        //                if (MS.category == "A")
        //                {
        //                    MS.category = "Additional subject";
        //                }


        //                MS.Class = MS.StoreAllData.Tables[0].Rows[0]["class"].ToString();
        //                if (MS.Class == "10")
        //                {
        //                    MS.Class = "Matriculation";
        //                    MS.MatricSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
        //                    MS.MatricSub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
        //                }
        //                MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
        //                MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
        //                MS.Session = MS.SelMonth + ' ' + MS.SelYear;
        //                MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
        //                Session["OROLL"] = MS.OROLL;
        //                Session["category"] = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();

        //                MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();
        //                MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();

        //                MS.refNo = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
        //                MS.SCHL = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();

        //                MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
        //                MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
        //                MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();

        //                MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
        //                MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();
        //                //MS.Result = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();

        //                MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
        //                MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
        //                MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();

        //                MS.EPname = MS.Candi_Name + '/' + MS.Pname;
        //                MS.EPFname = MS.Father_Name + '/' + MS.PFname;
        //                MS.EPMname = MS.Mother_Name + '/' + MS.PMname;

        //                MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["sex"].ToString();
        //                MS.CastList = MS.StoreAllData.Tables[0].Rows[0]["caste"].ToString();
        //                MS.Area = MS.StoreAllData.Tables[0].Rows[0]["area"].ToString();
        //                if (MS.Area == "U")
        //                {
        //                    MS.Area = "Urban";
        //                }
        //                if (MS.Area == "R")
        //                {
        //                    MS.Area = "Rural";
        //                }
        //                MS.Relist = MS.StoreAllData.Tables[0].Rows[0]["religion"].ToString();
        //                if (MS.Relist == "H")
        //                {
        //                    MS.Relist = "Hindu";
        //                }
        //                else if (MS.Relist == "M")
        //                {
        //                    MS.Relist = "Muslim";
        //                }
        //                else if (MS.Relist == "S")
        //                {
        //                    MS.Relist = "Sikh";
        //                }
        //                else if (MS.Relist == "C")
        //                {
        //                    MS.Relist = "Christian";
        //                }
        //                else if (MS.Relist == "O")
        //                {
        //                    MS.Relist = "Others";
        //                }

        //                MS.phyChal = MS.StoreAllData.Tables[0].Rows[0]["phy_chal"].ToString();
        //                MS.rdoWantWriter = MS.StoreAllData.Tables[0].Rows[0]["writer"].ToString();
        //                if (MS.rdoWantWriter == "True")
        //                {
        //                    MS.rdoWantWriter = "1";
        //                }
        //                if (MS.rdoWantWriter == "False")
        //                {
        //                    MS.rdoWantWriter = "0";
        //                }

        //                MS.IsPracExam = MS.StoreAllData.Tables[0].Rows[0]["prac"].ToString();
        //                if (MS.IsPracExam == "True")
        //                {
        //                    MS.IsPracExam = "1";
        //                }
        //                if (MS.IsPracExam == "False")
        //                {
        //                    MS.IsPracExam = "0";
        //                }

        //                MS.Choice1 = MS.StoreAllData.Tables[0].Rows[0]["cent_1"].ToString();
        //                MS.Choice2 = MS.StoreAllData.Tables[0].Rows[0]["cent_2"].ToString();
        //                MS.address = MS.StoreAllData.Tables[0].Rows[0]["homeadd"].ToString();

        //                MS.landmark = MS.StoreAllData.Tables[0].Rows[0]["LandMark"].ToString();
        //                MS.block = MS.StoreAllData.Tables[0].Rows[0]["block"].ToString();

        //                MS.SelDist = MS.StoreAllData.Tables[0].Rows[0]["homedistco"].ToString();
        //                MS.tehsil = MS.StoreAllData.Tables[0].Rows[0]["tehsil"].ToString();
        //                MS.pinCode = MS.StoreAllData.Tables[0].Rows[0]["pincode"].ToString();
        //                DataSet Distresult = objDB.SelectDist(); // passing Value to DBClass from model
        //                ViewBag.MyDist = Distresult.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
        //                List<SelectListItem> items = new List<SelectListItem>();
        //                foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
        //                {
        //                    items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
        //                }
        //                // ViewBag.MyDist = items;
        //                ViewBag.MyDist = new SelectList(items, "Value", "Text");

        //                MS.SelExamDist = MS.StoreAllData.Tables[0].Rows[0]["dist"].ToString();
        //                ViewBag.MyExamDist = result.Tables[0];// Edit Mode for dislaying message after saving storing output.
        //                List<SelectListItem> items1 = new List<SelectListItem>();
        //                foreach (System.Data.DataRow dr in ViewBag.MyExamDist.Rows)
        //                {
        //                    items1.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
        //                }
        //                // ViewBag.MyDist = items;
        //                ViewBag.MyExamDist = new SelectList(items1, "Value", "Text");

        //                if (MS.SelExamDist == "")
        //                {
        //                    MS.SelExamDist = "0";
        //                }
        //                //ViewBag.MyTehsil = MS.tehsil.ToString();

        //                int dist = Convert.ToInt32(MS.SelDist);
        //                DataSet Tehresult = objDB.SelectAllTehsil(dist);
        //                ViewBag.MyTehsil = Tehresult.Tables[0];
        //                List<SelectListItem> TehList = new List<SelectListItem>();
        //                foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
        //                {
        //                    TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });
        //                }
        //                ViewBag.MyTehsil = TehList;

        //                if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
        //                {
        //                    MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
        //                    MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
        //                }

        //                @ViewBag.Photo = "../../upload/Upload2023/" + MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
        //                //MS.imgSign= MS.StoreAllData.Tables[1].Rows[0]["PathSign"].ToString();                        
        //                @ViewBag.sign = "../../upload/Upload2023/" + MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();

        //                MS.imgSign = MS.StoreAllData.Tables[0].Rows[0]["Sign_url"].ToString();
        //                MS.imgPhoto = MS.StoreAllData.Tables[0].Rows[0]["Photo_url"].ToString();

        //                Session["imgPhoto"] = MS.imgPhoto;
        //                Session["imgSign"] = MS.imgSign;
        //                MS.FormStatus = MS.StoreAllData.Tables[0].Rows[0]["FormStatus"].ToString();
        //                return View(MS);
        //            }
        //        }
        //        else
        //        {
        //            return RedirectToAction("PunAddRoll", "PunAdd");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("PunAddRoll", "PunAdd");
        //    }
        //}

        #region Begin ExamErrorListMIS
        public ActionResult AdminPunAddErrorListMIS()
        {
            PunAddModels am = new PunAddModels();
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                am.StoreAllData = objDB.AdminPunAddErrorReport();
                if (am.StoreAllData != null)
                {
                    ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
                }
                else
                {
                    ViewBag.TotalCount = 0;
                }
                return View(am);
            }
        }

        [HttpPost]
        public ActionResult AdminPunAddErrorListMIS(PunAddModels AM) // HttpPostedFileBase file
        {
            try
            {
                // firm login // dist 
                if (Session["UserName"] == null)
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
                        AM.StoreAllData = objDB.AdminPunAddErrorReport();
                        if (AM.StoreAllData != null)
                        {
                            ViewBag.TotalCount = AM.StoreAllData.Tables[0].Rows.Count;
                        }
                        else
                        {
                            ViewBag.TotalCount = 0;
                        }
                        return View(AM);
                    }
                    DataSet ds = new DataSet();
                    if (AM.file.ContentLength > 0)  //(Request.Files["file"].ContentLength > 0
                    {
                        string fileName1 = "ErrorMIS_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

                        string fileExtension = System.IO.Path.GetExtension(AM.file.FileName);
                        if (fileExtension == ".xls" || fileExtension == ".xlsx")
                        {
                            // fileLocation = Server.MapPath("~/BankUpload/") + BM.file.FileName;
                            // fileLocation = Server.MapPath("~/BankUpload/") + BM.file.FileName;
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

                            string CheckMis = objDB.CheckExamMisExcel(ds);
                            if (CheckMis == "")
                            {
                                DataTable dt1 = ds.Tables[0];
                                string Result1 = "";
                                int OutStatus = 0;
                                DataTable dtResult = objDB.ExamErrorListMIS(dt1, AdminId, out OutStatus);// OutStatus mobile
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

                                AM.StoreAllData = objDB.AdminPunAddErrorReport();
                                if (AM.StoreAllData != null)
                                {
                                    ViewBag.TotalCount = AM.StoreAllData.Tables[0].Rows.Count;
                                }
                                else
                                {
                                    ViewBag.TotalCount = 0;
                                }
                                return View(AM);
                            }
                            else
                            {

                                ViewData["Result"] = "-1";
                                ViewBag.Message = CheckMis;

                                AM.StoreAllData = objDB.AdminPunAddErrorReport();
                                if (AM.StoreAllData != null)
                                {
                                    ViewBag.TotalCount = AM.StoreAllData.Tables[0].Rows.Count;
                                }
                                else
                                {
                                    ViewBag.TotalCount = 0;
                                }
                                return View(AM);
                            }
                        }
                        else
                        {

                            ViewData["Result"] = "-2";
                            ViewBag.Message = "Please Upload Only .xls file only";

                            AM.StoreAllData = objDB.AdminPunAddErrorReport();
                            if (AM.StoreAllData != null)
                            {
                                ViewBag.TotalCount = AM.StoreAllData.Tables[0].Rows.Count;
                            }
                            else
                            {
                                ViewBag.TotalCount = 0;
                            }

                            AM.StoreAllData = objDB.AdminPunAddErrorReport();
                            if (AM.StoreAllData != null)
                            {
                                ViewBag.TotalCount = AM.StoreAllData.Tables[0].Rows.Count;
                            }
                            else
                            {
                                ViewBag.TotalCount = 0;
                            }
                            return View(AM);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                AM.StoreAllData = objDB.AdminPunAddErrorReport();
                if (AM.StoreAllData != null)
                {
                    ViewBag.TotalCount = AM.StoreAllData.Tables[0].Rows.Count;
                }
                else
                {
                    ViewBag.TotalCount = 0;
                }
                return View(AM);
            }
            AM.StoreAllData = objDB.AdminPunAddErrorReport();
            if (AM.StoreAllData != null)
            {
                ViewBag.TotalCount = AM.StoreAllData.Tables[0].Rows.Count;
            }
            else
            {
                ViewBag.TotalCount = 0;
            }
            return View(AM);
        }
        #endregion  ExamErrorListMIS

        #region PunAdd CutList
        public ActionResult AdminPunAddCutList()
        {
            try
            {
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
                PunAddModels MS = new PunAddModels();

                DataSet result = objDB.GetPunAddbatchYear(); // passing Value to DBClass from model
                ViewBag.MybatchYear = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MybatchYear.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["batchYear"].ToString(), Value = @dr["batchYear"].ToString() });
                }
                ViewBag.MybatchYear = new SelectList(items, "Value", "Text");
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Admin");
            }

        }
        [HttpPost]
        public ActionResult AdminPunAddCutList(FormCollection frm)
        {
            try
            {
                //int pageIndex = 1;
                //pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                //ViewBag.pagesize = pageIndex;
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
                PunAddModels MS = new PunAddModels();

                DataSet result = objDB.GetPunAddbatchYear(); // passing Value to DBClass from model
                ViewBag.MybatchYear = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MybatchYear.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["batchYear"].ToString(), Value = @dr["batchYear"].ToString() });
                }
                ViewBag.MybatchYear = new SelectList(items, "Value", "Text");

                MS.batchYear = frm["batchYear"].ToString();
                MS.SearchBy = frm["SearchBy"].ToString();
                MS.SearchString = frm["SearchString"].ToString();
                //ViewBag.MybatchYear = new SelectList(items, "Value", "Text");                
                ViewBag.MybatchYear = items.ToList();

                if (ModelState.IsValid)
                {
                    string Search = string.Empty;
                    if (MS.batchYear != "")
                    {
                        Search = "a.batch='" + MS.batchYear.Substring(0, 1) + "' and a.batchYear='" + MS.batchYear.Substring(2, 4) + "'";
                    }
                    if (MS.SearchBy != "0")
                    {
                        switch (MS.SearchBy)
                        {
                            case "refno": Search += " and a.refno='" + (MS.SearchString).Trim() + "'"; break;
                            case "roll": Search += " and a.roll='" + (MS.SearchString).Trim() + "'"; break;
                            case "name": Search += " and a.name like '%" + (MS.SearchString).Trim() + "%'"; break;
                            case "fname": Search += " and a.fName like '%" + (MS.SearchString).Trim() + "%'"; break;
                            case "mname": Search += " and a.mname like '%" + (MS.SearchString).Trim() + "%'"; break;
                            case "mobile": Search += " and a.mobile='" + (MS.SearchString).Trim() + "'"; break;
                            case "Rno": Search += " and a.Rno='" + (MS.SearchString).Trim() + "'"; break;
                            case "Set": Search += " and a.[Set] like '%" + (MS.SearchString).Trim() + "%'"; break;
                        }
                    }
                    MS.StoreAllData = objDB.AdminPunAddCutList(Search);
                    ViewBag.TotalCount = MS.StoreAllData.Tables[1].Rows.Count;

                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        MS.SearchString = string.Empty;
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                    else
                    {
                        return View(MS);
                    }
                }
                else
                {
                    return PunAddRoll();
                }
            }
            catch (Exception ex)
            {
                return PunAddRoll();
                //return RedirectToAction("Index", "Admin");
            }
        }
        #endregion CutList
        public ActionResult AdminPunAddSignChart()
        {
            try
            {
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                DataSet result = objDB.GetPunAddbatchYear(); // passing Value to DBClass from model
                ViewBag.MybatchYear = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MybatchYear.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["batchYear"].ToString(), Value = @dr["batchYear"].ToString() });
                }
                ViewBag.MybatchYear = new SelectList(items, "Value", "Text");
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                ViewBag.MyCentre = "";// new SelectList(items, "Value", "Text");
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Admin");
            }

        }
        [HttpPost]
        public ActionResult AdminPunAddSignChart(FormCollection frm)
        {
            PunAddModels MS = new PunAddModels();
            try
            {
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                DataSet result = objDB.GetPunAddbatchYear(); // passing Value to DBClass from model
                ViewBag.MybatchYear = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MybatchYear.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["batchYear"].ToString(), Value = @dr["batchYear"].ToString() });
                }
                ViewBag.MybatchYear = new SelectList(items, "Value", "Text");

                MS.batchYear = frm["batchYear"].ToString();
                MS.SearchBy = frm["SearchBy"].ToString();
                MS.SearchString = frm["SearchString"].ToString();
                MS.setNo = frm["setNo"].ToString();

                //ViewBag.MybatchYear = new SelectList(items, "Value", "Text");                
                ViewBag.MybatchYear = items.ToList();

                MS.CList = frm["Clist"].ToString();
                ViewBag.MyCentre = Session["MyCentre"];
                if (ModelState.IsValid)
                {
                    string Search = string.Empty;
                    if (MS.batchYear != "")
                    {
                        Search = "a.batch='" + MS.batchYear.Substring(0, 1) + "' and a.batchYear='" + MS.batchYear.Substring(2, 4) + "'";
                    }
                    if (MS.CList != "" && MS.CList != "0")
                    {
                        Search += " and a.examcent='" + MS.CList + "'";
                    }
                    if (MS.SearchBy != "0")
                    {
                        switch (MS.SearchBy)
                        {
                            case "01": Search += " and a.rsub1code='01'"; break;
                            case "72": Search += " and a.rsub2code='72'"; break;
                        }
                    }
                    if (MS.setNo != "0")
                    {
                        switch (MS.setNo)
                        {
                            case "A": Search += " and a.[SET]='A'"; break;
                            case "B": Search += " and a.[SET]='B'"; break;
                        }
                    }
                    if (MS.SearchString.Trim() != "")
                    {
                        Search += " and a.Examroll in(" + MS.SearchString + ")";
                    }
                    MS.StoreAllData = objDB.AdminPunAddSignChart(Search, MS.SearchBy);
                    ViewBag.TotalCount = MS.StoreAllData.Tables[1].Rows.Count;
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                }
                if (ModelState.IsValid)
                { return View(MS); }
                else
                { return View(); }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Admin");
            }

        }

        public ActionResult AdminPunAddConfidentialList()
        {
            try
            {
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                DataSet result = objDB.GetPunAddbatchYear(); // passing Value to DBClass from model
                ViewBag.MybatchYear = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MybatchYear.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["batchYear"].ToString(), Value = @dr["batchYear"].ToString() });
                }
                ViewBag.MybatchYear = new SelectList(items, "Value", "Text");
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                ViewBag.MyCentre = "";
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Admin");
            }
        }
        [HttpPost]
        public ActionResult AdminPunAddConfidentialList(FormCollection frm)
        {
            PunAddModels MS = new PunAddModels();
            try
            {
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                DataSet result = objDB.GetPunAddbatchYear(); // passing Value to DBClass from model
                ViewBag.MybatchYear = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MybatchYear.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["batchYear"].ToString(), Value = @dr["batchYear"].ToString() });
                }
                ViewBag.MybatchYear = new SelectList(items, "Value", "Text");

                MS.batchYear = frm["batchYear"].ToString();
                MS.SearchBy = frm["SearchBy"].ToString();
                MS.SearchString = frm["SearchString"].ToString();
                MS.setNo = frm["setNo"].ToString();
                //ViewBag.MybatchYear = new SelectList(items, "Value", "Text");                
                ViewBag.MybatchYear = items.ToList();
                MS.CList = frm["Clist"].ToString();
                ViewBag.MyCentre = Session["MyCentre"];

                if (ModelState.IsValid)
                {
                    string Search = string.Empty;
                    if (MS.batchYear != "")
                    {
                        Search = "a.batch='" + MS.batchYear.Substring(0, 1) + "' and a.batchYear='" + MS.batchYear.Substring(2, 4) + "'";
                    }

                    if (MS.CList != "" && MS.CList != "0")
                    {
                        Search += " and a.examcent='" + MS.CList + "'";
                    }
                    if (MS.SearchBy != "0")
                    {
                        switch (MS.SearchBy)
                        {
                            case "01": Search += " and a.rsub1code='01'"; break;
                            case "72": Search += " and a.rsub2code='72'"; break;
                        }
                    }
                    if (MS.setNo != "0")
                    {
                        switch (MS.setNo)
                        {
                            case "A": Search += " and a.[SET]='A'"; break;
                            case "B": Search += " and a.[SET]='B'"; break;
                        }
                    }
                    if (MS.SearchString.Trim() != "")
                    {
                        //Search += " and a.Examroll='" + MS.SearchString.ToString().Trim() + "'";
                        Search += " and a.Examroll in (" + MS.SearchString.ToString().Trim() + ")";
                    }
                    //sm.StoreAllData = objDB.ConfidentialListMatric(sm);
                    MS.StoreAllData = objDB.AdminPunAddConfidentialList(Search, MS.SearchBy);
                    ViewBag.TotalCount = MS.StoreAllData.Tables[1].Rows.Count;
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                }
                if (ModelState.IsValid)
                { return View(MS); }
                else
                { return View(); }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Admin");
            }

        }
        #region Begin Admit Card
        public ActionResult PunAddAdmitCardSearch()
        {
            try
            {
                //if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                //{
                //    return RedirectToAction("Index", "Admin");
                //}
                //AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();

                PunAddModels MS = new PunAddModels();
                //DataSet result = objDB.GetPunAddbatchYear(); // passing Value to DBClass from model
                DataSet result = objDB.GetPunAddbatchYearAdmitCard(); // passing Value to DBClass from model
                ViewBag.MybatchYear = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MybatchYear.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["batchYear"].ToString(), Value = @dr["batchYear"].ToString() });
                }
                ViewBag.MybatchYear = new SelectList(items, "Value", "Text");
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Admin");
            }
        }
        [HttpPost]
        public ActionResult PunAddAdmitCardSearch(FormCollection frm, int? page, string cmd)
        {
            try
            {
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                //if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                //{
                //    return RedirectToAction("Index", "Admin");
                //}
                AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
                PunAddModels MS = new PunAddModels();

                //DataSet result = objDB.GetPunAddbatchYear(); // passing Value to DBClass from model
                DataSet result = objDB.GetPunAddbatchYearAdmitCard(); // passing Value to DBClass from model

                ViewBag.MybatchYear = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MybatchYear.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["batchYear"].ToString(), Value = @dr["batchYear"].ToString() });
                }
                ViewBag.MybatchYear = new SelectList(items, "Value", "Text");

                MS.batchYear = frm["batchYear"].ToString();
                MS.SearchBy = frm["SearchBy"].ToString();
                MS.SearchString = frm["SearchString"].ToString();
                //ViewBag.MybatchYear = new SelectList(items, "Value", "Text");                
                ViewBag.MybatchYear = items.ToList();

                if (ModelState.IsValid)
                {
                    string Search = string.Empty;
                    if (MS.batchYear != "")
                    {
                        //Search = "batch='" + MS.batchYear.Substring(0, 1) + "' and batchYear='" + MS.batchYear.Substring(2, 4) + "'";
                        Search = "c.month='" + MS.batchYear.Split('/')[0] + "' and batchYear='" + MS.batchYear.Split('/')[1] + "'";
                    }
                    if (MS.SearchBy != "0")
                    {
                        switch (MS.SearchBy)
                        {
                            case "refno": Search += " and refno='" + (MS.SearchString).Trim() + "'"; break;
                            case "roll": Search += " and roll='" + (MS.SearchString).Trim() + "'"; break;
                            case "name": Search += " and name like '%" + (MS.SearchString).Trim() + "%'"; break;
                            case "fname": Search += " and fname like '%" + (MS.SearchString).Trim() + "%'"; break;
                            case "mname": Search += " and mname like '%" + (MS.SearchString).Trim() + "%'"; break;
                            case "mobile": Search += " and mobile='" + (MS.SearchString).Trim() + "'"; break;
                            case "Rno": Search += " and Rno='" + (MS.SearchString).Trim() + "'"; break;
                        }
                    }
                    MS.StoreAllData = objDB.PunAddAdmitCardSearch(Search);

                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else { ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count; }
                }
                if (ModelState.IsValid)
                { return View(MS); }
                else
                { return View(); }
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult PunAddAdmitCard(string id)
        {
            PunAddModels MS = new PunAddModels();
            if (id != "" && id != null)
            {
                MS.refNo = id;
                MS.StoreAllData = objDB.GetPunAddAdmitCard(MS);
                ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message2 = "Record Not Found";
                    ViewBag.TotalCount2 = 0;
                }
            }
            return View(MS);
        }

        #endregion Admit Card 
        #region Result Sheet of Admin Punjabu Additional         
        public ActionResult AdminPunAddResultSheet()
        {
            try
            {
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }

                ViewBag.ResultBatchList = objDB.GetActivePBIResultBatchList();

                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                return View();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Admin");
            }

        }
        [HttpPost]
        public ActionResult AdminPunAddResultSheet(FormCollection frm)
        {
            PunAddModels MS = new PunAddModels();
            try
            {
                if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                ViewBag.ResultBatchList = objDB.GetActivePBIResultBatchList();


                MS.batchYear = frm["batchYear"].ToString();
                //MS.SearchBy = frm["SearchBy"].ToString();
                MS.SearchString = frm["SearchString"].ToString();
                MS.setNo = frm["setNo"].ToString();
                //ViewBag.MybatchYear = new SelectList(items, "Value", "Text");                
                //ViewBag.MybatchYear = items.ToList();

                string getbatchYear = MS.batchYear.Split('-')[0];

                string selBatch = MS.batchYear.Split('/')[0];
                string selBatchYr = MS.batchYear.Split('/')[1];
                selBatchYr = selBatchYr.Split('-')[0];

                if (ModelState.IsValid)
                {
                    string Search = string.Empty;
                    if (MS.batchYear != "")
                    {
                        Search = "a.batch='" + selBatch + "' and a.batchYear='" + selBatchYr + "'";
                    }

                    if (MS.setNo != "0")
                    {
                        switch (MS.setNo)
                        {
                            case "A": Search += " and a.[SET]='A'"; break;
                            case "B": Search += " and a.[SET]='B'"; break;
                        }
                    }
                    if (MS.SearchString.Trim() != "")
                    {
                        Search += " and a.Examroll='" + MS.SearchString + "'";
                    }
                    //sm.StoreAllData = objDB.ConfidentialListMatric(sm);
                    //MS.StoreAllData = objDB.AdminPunAddResultSheet(Search, MS.SearchBy);
                    MS.StoreAllData = objDB.AdminPunAddResultSheet(Search, MS.setNo);
                    ViewBag.TotalCount = MS.StoreAllData.Tables[1].Rows.Count;
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                }
                if (ModelState.IsValid)
                { return View(MS); }
                else
                { return View(); }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Admin");
            }

        }
        #endregion Result Sheet of Admin Punjabu Additional


        #endregion Admin Pannel



        public ActionResult PrivateRefrenceUnlockPage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PrivateRefrenceUnlockPage(FormCollection frc)
        {
            PunAddModels MS = new PunAddModels();
            AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
            MS.refNo = frc["refNo"];

            if (MS.refNo != null && MS.refNo.Length == 13)
            {
                MS.StoreAllData = objDB.PunAddUnlockPage(MS);
            }
            return View();
        }
        public ActionResult PunAddExamination()
        {

            return View();
        }

        [HttpPost]
        public ActionResult PunAddExamination(FormCollection frc)
        {
            try
            {
                PunAddModels MS = new PunAddModels();
                AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
                MS.refNo = frc["refNo"];
                MS.OROLL = frc["OROLL"];

                if (MS.refNo != null && MS.refNo.ToString() != "")
                {
                    MS.StoreAllData = objDB.GetDetailTblPunAdd(MS);
                    ViewBag.Message = "";
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Given Reference & Roll number not currect.";
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                    else
                    {

                        if (MS.refNo == MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString() && MS.OROLL == MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString())
                        {
                            Session["Oroll"] = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                            ViewData["refno"] = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                            Session["refno"] = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                            ViewData["roll"] = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                            ViewData["Status"] = "1";
                            return View(MS);
                            //return RedirectToAction("MigrationRec", "MigrateSchool");
                        }

                        return View(MS);
                    }
                }
                return View();
            }
            catch (Exception ex)
            {

                return RedirectToAction("PunAddExamination", "PunAdd");
            }

        }

        public ActionResult PunAddIntroduction()
        {
            AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
            PunAddModels MS = new PunAddModels();

            DataTable dt = new DataTable();
            dt = objDB.CurrentBatchYear();
            string hdText = "";
            List<SelectListItem> itemBatch = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                itemBatch.Add(new SelectListItem { Text = @dr["month"].ToString(), Value = @dr["batch"].ToString() });
                hdText = hdText + "" + itemBatch[0].Text;
            }
            ViewBag.Mybatch = itemBatch.ToList();

            List<SelectListItem> itemBatchYear = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                itemBatchYear.Add(new SelectListItem { Text = @dr["yr"].ToString(), Value = @dr["yr"].ToString() });
                hdText = hdText + "-" + itemBatchYear[0].Text;
            }
            ViewBag.MybatchYear = itemBatchYear.ToList();

            ViewBag.HeaderbatchYear = hdText;

            List<SelectListItem> Monthlist = objDB.GetMonth();
            ViewBag.MyMonth = Monthlist;

            List<SelectListItem> yearlist = objDB.GetSessionYear1();
            ViewBag.MyYear = yearlist;

            return View();
        }

        [HttpPost]
        public ActionResult PunAddIntroduction(FormCollection frm, PunAddModels MS)
        {
            AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();

            DataTable dt = new DataTable();
            dt = objDB.CurrentBatchYear();
            string hdText = "";
            List<SelectListItem> itemBatch = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                itemBatch.Add(new SelectListItem { Text = @dr["month"].ToString(), Value = @dr["batch"].ToString() });
                hdText = hdText + "" + itemBatch[0].Text;
            }
            ViewBag.Mybatch = itemBatch.ToList();

            List<SelectListItem> itemBatchYear = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                itemBatchYear.Add(new SelectListItem { Text = @dr["yr"].ToString(), Value = @dr["yr"].ToString() });
                hdText = hdText + "-" + itemBatchYear[0].Text;
            }
            ViewBag.MybatchYear = itemBatchYear.ToList();

            ViewBag.HeaderbatchYear = hdText;

            List<SelectListItem> Monthlist = objDB.GetMonth();
            ViewBag.MyMonth = Monthlist;

            List<SelectListItem> yearlist = objDB.GetSessionYear1();
            ViewBag.MyYear = yearlist;

            try
            {
                string batchYrNM = string.Empty;

                MS.Board = frm["Board"];
                MS.batch = frm["batch"];
                MS.batchYear = frm["batchYear"];
                MS.Class = frm["Class"];
                MS.category = frm["category"];
                MS.Exam_Type = frm["Exam_Type"];
                MS.SelMonth = frm["SelMonth"];
                MS.SelYear = frm["SelYear"];
                MS.Result = frm["Result"];
                MS.MatricMarks = frm["MatricMarks"];
                MS.OROLL = frm["OROLL"];
                MS.emailID = frm["emailID"];
                MS.mobileNo = frm["mobileNo"];
                if (MS.Board == "OTHER BOARD")
                {
                    MS.Other_Board = frm["Other_Board"];
                }
                else
                {
                    MS.Other_Board = null;
                }

                if (frm["OROLL"] != null && frm["OROLL"] != "")
                {
                    DataSet result2 = objDB.InsertTblPunAdd(MS);
                    if (result2.Tables[0].Rows.Count > 0)
                    {
                        if (result2.Tables[0].Rows[0]["result"].ToString() == "1")
                        {
                            switch (MS.batch)
                            {
                                case "1": batchYrNM = "April " + MS.batchYear; break;
                                case "2": batchYrNM = "July " + MS.batchYear; break;
                                case "3": batchYrNM = "October " + MS.batchYear; break;
                                case "4": batchYrNM = "January " + MS.batchYear; break;
                            }
                            string Oroll = frm["OROLL"].ToString();
                            string Clss = frm["Class"];
                            string Yar = frm["SelYear"];
                            string Mnth = frm["SelMonth"];
                            string Cat = frm["category"].ToString();
                            string refno = result2.Tables[0].Rows[0]["refno"].ToString();
                            ViewData["Status"] = result2.Tables[0].Rows[0]["result"].ToString();
                            Session["Oroll"] = Oroll;
                            Session["refno"] = refno;
                            TempData["refno"] = refno;
                            TempData["roll"] = Oroll;
                            //----------------- Email and SMS --------
                            try
                            {
                                AbstractLayer.DBClass dbclass = new AbstractLayer.DBClass();
                                if (MS.mobileNo != null || MS.mobileNo != "")
                                {
                                    string Sms = "You are registred for Class 10th " + batchYrNM + " with Ref No. " + refno + " is generated against old roll no. " + Oroll + ", save it till result declaration.";

                                    string getSms = dbclass.gosms(MS.mobileNo, Sms);
                                    // string getSms = objCommon.gosms("9711819184", Sms);
                                }
                                if (MS.emailID != null || MS.emailID != "")
                                {
                                    string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Dear " + ViewData["name"] + "</b>,</td></tr><tr><td height=" + 30 + ">As per your request Dated <b>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</b> Regarding Punjabi Additional Form</td></tr><tr><td><b>You are successfully registred for:-</b><br /><b>Class :</b> 10th, " + batchYrNM + " <br /><b> Reference No. :</b> " + refno + "<br /><b> Old Roll No. :</b> " + Oroll + " <br /><b> Keep this for further use till result declaration.</b> <br /></td></tr><tr><td height=" + 30 + "><b>Click Here To Login</b> <a href=https://registration2022.pseb.ac.in/PunAdd/PunAddExamination  target = _blank>https://registration2023.pseb.ac.in</a></td></tr><tr><td><b>Note:</b> Please Read Instruction Carefully Before filling the Online Form .</td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /><tr><td><b>Contact Us</b><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 8058911911<br>DISTRICTS:- BARNALA, FATEHGARH SAHIB, GURDASPUR, HOSHIARPUR, JALANDHAR, KAPURTHALA, SHRI MUKTSAR SAHIB, S.B.S. NAGAR, PATHANKOT, PATIALA, SANGRUR, CHANDIGARH &amp; OTHER STATES<br><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 8058911911<br>DISTRICTS:- AMRITSAR, BATHINDA, FARIDKOT, FAZILKA, FEROZEPUR, LUDHIANA, MANSA, MOGA, ROOP NAGAR, S.A.S NAGAR,TARN TARAN<br></td></tr>";

                                    string subject = "PSEB-Private Form Notification";
                                    bool result3 = dbclass.mail(subject, body, MS.emailID);
                                }

                            }
                            catch (Exception)
                            {
                                return View(MS);
                            }
                            return View(MS);
                        }
                        if (result2.Tables[0].Rows[0]["result"].ToString() == "2")
                        {
                            ViewData["roll"] = MS.OROLL;
                            ViewData["RefNo"] = result2.Tables[0].Rows[0]["refno"].ToString();
                            ViewData["Status"] = "2";
                            return View(MS);
                        }
                        if (result2.Tables[0].Rows[0]["result"].ToString() == "3")
                        {

                            ViewData["roll"] = MS.OROLL;
                            ViewData["Status"] = "3";
                            return View(MS);

                        }
                        if (result2.Tables[0].Rows[0]["result"].ToString() == "0")
                        {
                            ViewData["roll"] = MS.OROLL;
                            ViewData["Status"] = "0";
                            return View(MS);
                        }

                    }
                    else
                    {
                        ViewData["roll"] = MS.OROLL;
                        ViewData["Status"] = "0";
                        return View(MS);
                    }

                }
            }
            catch (Exception ex)
            {

                return View(MS);
            }


            return View(MS);
        }


        public ActionResult PunAddConfirmation(string id)
        {
            string refno = string.Empty;
            string Oroll = string.Empty;

            ViewBag.YesNoList = new AbstractLayer.DBClass().GetYesNoText();

            try
            {
                if (Session["refno"] == null || Session["refno"].ToString() == "")
                {
                    return RedirectToAction("PunAddExamination", "PunAdd");
                }
                AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
                PunAddModels MS = new PunAddModels();
                Oroll = Session["Oroll"].ToString();
                refno = Session["refno"].ToString();
                if (refno != null && refno != "")
                {
                    MS.StoreAllData = objDB.GetPunAddConfirmation(refno);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        return View();
                    }
                    else
                    {
                        if (MS.StoreAllData.Tables[0].Rows[0]["EligibilityDoc1"] != null)
                        {
                            MS.EligibilityDoc1 = MS.StoreAllData.Tables[0].Rows[0]["EligibilityDoc1"].ToString();
                        }
                        if (MS.StoreAllData.Tables[0].Rows[0]["EligibilityDoc2"] != null)
                        {
                            MS.EligibilityDoc2 = MS.StoreAllData.Tables[0].Rows[0]["EligibilityDoc2"].ToString();
                        }
                        MS.IsHardCopyCertificate = MS.StoreAllData.Tables[0].Rows[0]["IsHardCopyCertificate"].ToString();
                        MS.Board = MS.StoreAllData.Tables[0].Rows[0]["boardnm"].ToString();
                        MS.Other_Board = MS.StoreAllData.Tables[0].Rows[0]["OtherBoard"].ToString();

                        MS.batch = MS.StoreAllData.Tables[0].Rows[0]["batchMonth"].ToString();
                        MS.batchYear = MS.StoreAllData.Tables[0].Rows[0]["batchYear"].ToString();
                        //MS.Result = MS.StoreAllData.Tables[0].Rows[0]["Resultdtl"].ToString();
                        //MS.MatricMarks = MS.StoreAllData.Tables[0].Rows[0]["Resultdtl"].ToString().IndexOf('(',')');
                        string intval = MS.StoreAllData.Tables[0].Rows[0]["Resultdtl"].ToString();
                        MS.Result = intval.Substring(0, 4);// "PASS";
                        MS.MatricMarks = intval.Replace("PASS(", "").Replace(")", "");

                        MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["rp"].ToString();
                        if (MS.Exam_Type == "P")
                        {
                            MS.Exam_Type = "Private";
                        }

                        MS.category = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                        if (MS.category == "A")
                        {
                            MS.category = "Additional subject";
                        }
                        if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                        {
                            //Session["form"] = "Punjabi_Add_Spl_Jan";
                            Session["form"] = MS.StoreAllData.Tables[0].Rows[0]["feecat"].ToString();
                        }

                        MS.Class = MS.StoreAllData.Tables[0].Rows[0]["class"].ToString();
                        if (MS.Class == "10")
                        {
                            MS.Class = "Matriculation";
                            MS.MatricSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                            MS.MatricSub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                        }
                        MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                        MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
                        MS.Session = MS.SelMonth + ' ' + MS.SelYear;
                        MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                        Session["OROLL"] = MS.OROLL;
                        Session["category"] = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                        MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();
                        MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();
                        MS.refNo = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                        MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                        MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                        MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();
                        MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();
                        //MS.Result = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();
                        MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
                        MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
                        MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();
                        MS.EPname = MS.Candi_Name + '/' + MS.Pname;
                        MS.EPFname = MS.Father_Name + '/' + MS.PFname;
                        MS.EPMname = MS.Mother_Name + '/' + MS.PMname;
                        MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["sex"].ToString();
                        MS.CastList = MS.StoreAllData.Tables[0].Rows[0]["caste"].ToString();
                        MS.Area = MS.StoreAllData.Tables[0].Rows[0]["area"].ToString();
                        Session["ChallanID"] = MS.StoreAllData.Tables[0].Rows[0]["challanid"].ToString();
                        if (MS.Area == "U")
                        {
                            MS.Area = "Urban";
                        }
                        if (MS.Area == "R")
                        {
                            MS.Area = "Rural";
                        }
                        MS.Relist = MS.StoreAllData.Tables[0].Rows[0]["religion"].ToString();
                        if (MS.Relist == "H")
                        {
                            MS.Relist = "Hindu";
                        }
                        if (MS.Relist == "M")
                        {
                            MS.Relist = "Muslim";
                        }
                        if (MS.Relist == "S")
                        {
                            MS.Relist = "Sikh";
                        }
                        if (MS.Relist == "C")
                        {
                            MS.Relist = "Christian";
                        }
                        if (MS.Relist == "O")
                        {
                            MS.Relist = "Others";
                        }
                        MS.phyChal = MS.StoreAllData.Tables[0].Rows[0]["phy_chal"].ToString();
                        MS.rdoWantWriter = "0";
                        MS.IsPracExam = MS.StoreAllData.Tables[0].Rows[0]["prac"].ToString();
                        if (MS.IsPracExam == "True")
                        {
                            MS.IsPracExam = "1";
                        }
                        if (MS.IsPracExam == "False")
                        {
                            MS.IsPracExam = "0";
                        }
                        MS.address = MS.StoreAllData.Tables[0].Rows[0]["homeadd"].ToString();
                        MS.landmark = MS.StoreAllData.Tables[0].Rows[0]["LandMark"].ToString();
                        MS.block = MS.StoreAllData.Tables[0].Rows[0]["block"].ToString();
                        MS.SelExamDist = MS.StoreAllData.Tables[0].Rows[0]["dist"].ToString();
                        MS.SelDist = MS.StoreAllData.Tables[0].Rows[0]["homedistco"].ToString();
                        MS.tehsil = MS.StoreAllData.Tables[0].Rows[0]["tehsil"].ToString();
                        MS.pinCode = MS.StoreAllData.Tables[0].Rows[0]["pincode"].ToString();
                        DataSet result = objDB.SelectDist(); // passing Value to DBClass from model
                        ViewBag.MyDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> items = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                        {
                            items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                        }
                        // ViewBag.MyDist = items;
                        ViewBag.MyDist = new SelectList(items, "Value", "Text");

                        if (MS.SelDist == "")
                        {
                            MS.SelDist = "0";
                        }

                        ViewBag.MyExamDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> items1 = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyExamDist.Rows)
                        {
                            items1.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                        }
                        ViewBag.MyExamDist = new SelectList(items1, "Value", "Text");

                        if (MS.SelExamDist == "")
                        {
                            MS.SelExamDist = "0";
                        }

                        int dist = Convert.ToInt32(MS.SelDist);
                        DataSet result1 = objDB.SelectAllTehsil(dist);
                        ViewBag.MyTehsil = result1.Tables[0];
                        List<SelectListItem> TehList = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
                        {

                            TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                        }
                        ViewBag.MyTehsil = TehList;

                        if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                        {
                            MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                            MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                        }
                        @ViewBag.Photo = "../../upload/Upload2023/" + MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
                        @ViewBag.sign = "../../upload/Upload2023/" + MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();

                        MS.imgSign = MS.StoreAllData.Tables[0].Rows[0]["Sign_url"].ToString();
                        MS.imgPhoto = MS.StoreAllData.Tables[0].Rows[0]["Photo_url"].ToString();

                        Session["imgPhoto"] = MS.imgPhoto;
                        Session["imgSign"] = MS.imgSign;


                        MS.FormStatus = MS.StoreAllData.Tables[0].Rows[0]["FormStatus"].ToString();
                        return View(MS);
                    }
                }
                else
                {
                    return RedirectToAction("PunAddExamination", "PunAdd");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("PunAddExamination", "PunAdd");
            }
        }
        [HttpPost]
        public ActionResult PunAddConfirmation(PunAddModels MS, FormCollection frm, HttpPostedFileBase eligibilitydoc1, HttpPostedFileBase eligibilitydoc2)
        {

            ViewBag.YesNoList = new AbstractLayer.DBClass().GetYesNoText();
            try
            {

                if (Session["Oroll"] == null || Session["Oroll"].ToString() == "" || Session["refno"] == null || Session["refno"].ToString() == "")
                {
                    return RedirectToAction("PunAddExamination", "PunAdd");
                }
                AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();

                DataSet result = objDB.SelectDist(); // passing Value to DBClass from model
                ViewBag.MyDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }
                ViewBag.MyDist = new SelectList(items, "Value", "Text");
                MS.Board = frm["Board"].ToString();
                if (MS.Board == "OTHER BOARD")
                {
                    MS.Other_Board = frm["Other_Board"];
                }
                else
                {
                    MS.Other_Board = null;
                }
                MS.Result = frm["Result"].ToString();
                MS.MatricMarks = frm["MatricMarks"].ToString();


                MS.Exam_Type = frm["Exam_Type"].ToString();
                MS.refNo = frm["refNo"].ToString();
                MS.Session = frm["Session"].ToString();
                //MS.Result = "";
                MS.Class = "10";
                MS.DOB = frm["DOB"].ToString();
                MS.Candi_Name = frm["Candi_Name"].ToString();
                MS.Father_Name = frm["Father_Name"].ToString();
                MS.Mother_Name = frm["Mother_Name"].ToString();

                MS.Pname = frm["PNAME"].ToString();
                MS.PFname = frm["PFNAME"].ToString();
                MS.PMname = frm["PMNAME"].ToString();
                MS.Board = frm["board"].ToString();
                if (MS.Board.Contains("P.S.E.B"))
                {
                    MS.RegNo = frm["RegNo"].ToString();
                }
                else
                {
                    MS.RegNo = null;
                }
                MS.OROLL = frm["OROLL"].ToString();
                Session["OROLL"] = MS.OROLL;
                Session["refno"] = MS.refNo;
                MS.mobileNo = frm["mobileNo"].ToString();
                MS.emailID = frm["emailID"].ToString();
                MS.Gender = frm["Gender"].ToString();
                MS.CastList = frm["CastList"].ToString();
                MS.Area = frm["Area"].ToString();
                if (MS.Area == "Urban")
                {
                    MS.Area = "U";
                }
                if (MS.Area == "Rural")
                {
                    MS.Area = "R";
                }
                MS.Relist = frm["Relist"].ToString();
                if (MS.Relist == "Hindu")
                {
                    MS.Relist = "H";
                }
                if (MS.Relist == "Muslim")
                {
                    MS.Relist = "M";
                }
                if (MS.Relist == "Sikh")
                {
                    MS.Relist = "S";
                }
                if (MS.Relist == "Christian")
                {
                    MS.Relist = "C";
                }
                if (MS.Relist == "Others")
                {
                    MS.Relist = "O";
                }

                MS.IsphysicalChall = frm["phyChal"].ToString();
                MS.rdoWantWriter = "0";
                MS.address = frm["address"].ToString();
                MS.landmark = frm["landmark"].ToString();
                MS.block = frm["block"].ToString();
                MS.SelDist = frm["SelDist"].ToString();

                int dist = Convert.ToInt32(MS.SelDist);
                DataSet result1 = objDB.SelectAllTehsil(dist);
                ViewBag.MyTehsil = result1.Tables[0];
                List<SelectListItem> TehList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
                {
                    TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });
                }
                ViewBag.MyTehsil = TehList;

                MS.tehsil = frm["tehsil"].ToString();
                MS.pinCode = frm["pinCode"].ToString();
                MS.category = frm["category"].ToString();

                if (MS.category == "Additional subject")
                {
                    MS.category = "A";
                }
                if (MS.category == "A" || MS.category == "D")
                {
                    MS.IsPracExam = "0";
                }
                else
                {
                    MS.IsPracExam = frm["IsPracExam"].ToString();
                    if (MS.IsPracExam == "Yes")
                    {
                        MS.IsPracExam = "1";
                    }
                    else
                    {
                        MS.IsPracExam = "0";
                    }
                }



                if (MS.std_Photo != null)
                {
                    stdPic = Path.GetFileName(MS.std_Photo.FileName);
                    string Filepath = Server.MapPath("~/Upload/Upload2023/PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/Photo/");
                    if (!Directory.Exists(Filepath))
                    {
                        Directory.CreateDirectory(Filepath);
                    }
                    string pathPhoto = Path.Combine(Server.MapPath("~/Upload/Upload2023/PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/Photo/"), MS.refNo + "P" + ".jpg");
                    MS.PathPhoto = "PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/Photo/" + MS.refNo + "P" + ".jpg";

                    MS.std_Photo.SaveAs(pathPhoto);
                }
                if (MS.std_Sign != null)
                {
                    stdSign = Path.GetFileName(MS.std_Sign.FileName);
                    string Filepath = Server.MapPath("~/Upload/Upload2023/PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/Sign/");
                    if (!Directory.Exists(Filepath))
                    {
                        Directory.CreateDirectory(Filepath);
                    }
                    string pathSign = Path.Combine(Server.MapPath("~/Upload/Upload2023/PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/Sign/"), MS.refNo + "S" + ".jpg");
                    MS.PathSign = "PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/Sign/" + MS.refNo + "S" + ".jpg";
                    MS.std_Sign.SaveAs(pathSign);
                }
                MS.imgPhoto = Session["imgPhoto"].ToString();
                MS.imgSign = Session["imgSign"].ToString();

                if ((MS.std_Photo == null || MS.std_Sign == null) && (MS.imgPhoto == null || MS.imgSign == null))
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Select Photo and Sign";
                    ViewData["SelectPhotoSign"] = "0";
                    return View(MS);
                }


                if (MS.Gender == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Select Gender";
                    ViewData["SelectGender"] = "0";
                    return View(MS);
                }

                if (MS.CastList == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Select Cast";
                    ViewData["SelectCast"] = "0";
                    return View(MS);
                }
                if (MS.Area == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Select Area";
                    ViewData["SelectArea"] = "0";
                    return View(MS);
                }
                if (MS.Relist == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Select Religion";
                    ViewData["SelectRelist"] = "0";
                    return View(MS);
                }
                if (MS.address == "")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Enter Address";
                    ViewData["Selectaddress"] = "0";
                    return View(MS);
                }
                if (MS.SelDist == "" || MS.tehsil == "" || MS.tehsil == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Select District & Tehsil";
                    ViewData["SelectDist"] = "0";
                    return View(MS);
                }
                if (MS.pinCode == "" || MS.pinCode.Count() != 6)
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Enter Pin Code";
                    ViewData["SelectPin"] = "0";
                    return View(MS);
                }



                MS.IsHardCopyCertificate = frm["IsHardCopyCertificate"].ToString();
                if (string.IsNullOrEmpty(MS.IsHardCopyCertificate))
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                    ViewBag.Message = "Select Hard Copy of Certificate";
                    ViewData["SelectDist"] = "0";
                    return View(MS);
                }


                if (eligibilitydoc1 != null)
                {
                    string fname = Path.GetFileName(eligibilitydoc1.FileName);
                    string fext = Path.GetExtension(eligibilitydoc1.FileName);
                    string Filepath = Server.MapPath("~/Upload/Upload2023/PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/EligibilityDocument/");
                    if (!Directory.Exists(Filepath))
                    {
                        Directory.CreateDirectory(Filepath);
                    }
                    string pathPhoto = Path.Combine(Server.MapPath("~/Upload/Upload2023/PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/EligibilityDocument/"), MS.refNo + "D1" + fext);
                    MS.EligibilityDoc1 = "PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/EligibilityDocument/" + MS.refNo + "D1" + fext;
                    eligibilitydoc1.SaveAs(pathPhoto);
                }
                else
                {

                    MS.EligibilityDoc1 = frm["EligibilityDoc1"];
                }

                if (eligibilitydoc2 != null)
                {
                    string fname = Path.GetFileName(eligibilitydoc2.FileName);
                    string fext = Path.GetExtension(eligibilitydoc2.FileName);
                    string Filepath = Server.MapPath("~/Upload/Upload2023/PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/EligibilityDocument/");
                    if (!Directory.Exists(Filepath))
                    {
                        Directory.CreateDirectory(Filepath);
                    }
                    string pathPhoto = Path.Combine(Server.MapPath("~/Upload/Upload2023/PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/EligibilityDocument/"), MS.refNo + "D2" + fext);
                    MS.EligibilityDoc2 = "PunAdd/Batch" + MS.refNo.Substring(0, 5) + "/EligibilityDocument/" + MS.refNo + "D2" + fext;
                    eligibilitydoc2.SaveAs(pathPhoto);
                }
                else
                {
                    MS.EligibilityDoc2 = frm["EligibilityDoc2"];
                }




                string OutError = "";
                DataSet result2 = AbstractLayer.PunAddDB.InsertPrivateCandidateConfirmation(MS, out OutError);
                //if (result2.Tables[0].Rows[0]["RESULT"].ToString() == "1")
                if (OutError == "1")
                {
                    ViewData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    ViewData["Status"] = "3";
                    ViewData["OutError"] = "";
                }
                else
                {
                    ViewData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    ViewData["Status"] = "ERR";
                    ViewData["OutError"] = OutError;
                }
                MS.StoreAllData = objDB.GetPunAddConfirmation(MS.refNo);
                return View(MS);
            }
            catch (Exception ex)
            {
                Session["OROLL"] = null;
                return PunAddExamination();
            }
        }
        public JsonResult GetTehID(int DIST) // Calling on http post (on Submit)
        {
            //AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();

            AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
            PunAddModels MS = new PunAddModels();

            DataSet result = objDB.SelectAllTehsil(DIST); // passing Value to DBClass from model

            ViewBag.MyTeh = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> TehList = new List<SelectListItem>();
            //List<string> items = new List<string>();
            TehList.Add(new SelectListItem { Text = "---Select Tehsil---", Value = "0" });
            foreach (System.Data.DataRow dr in ViewBag.MyTeh.Rows)
            {

                TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

            }
            ViewBag.MyTeh = TehList;

            return Json(TehList);

        }

        //-----------------------------------

        //public JsonResult GetExamID(string CATE) // Calling on http post (on Submit)
        //{
        //    AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
        //    PrivateCandidateModels MS = new PrivateCandidateModels();
        //    if (CATE == "R")
        //    {
        //        List<SelectListItem> Examlist = objDB.GetMonth();
        //        ViewBag.MyMonth = Examlist;
        //        return Json(Examlist);
        //    }
        //    else
        //    {
        //        DataSet Monthlist1 = objDB.GetSessionMonth();
        //        ViewBag.MyMonth = Monthlist1.Tables[0];
        //        List<SelectListItem> Monthlist = new List<SelectListItem>();
        //        //Monthlist.Add(new SelectListItem { Text = "Select Month", Value = "0" });
        //        foreach (System.Data.DataRow dr in ViewBag.MyMonth.Rows)
        //        {
        //            Monthlist.Add(new SelectListItem { Text = @dr["sessionMonth"].ToString(), Value = @dr["sessionMonth"].ToString() });
        //        }
        //        ViewBag.MyMonth = Monthlist;
        //        return Json(Monthlist);
        //    }
        //}
        public JsonResult GetMonthID(string CATE) // Calling on http post (on Submit)
        {
            AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
            PunAddModels MS = new PunAddModels();
            if (CATE == "A")
            {
                List<SelectListItem> Monthlist = objDB.GetMonth();
                ViewBag.MyMonth = Monthlist;
                return Json(Monthlist);
            }
            else
            {
                DataSet Monthlist1 = objDB.GetSessionMonth();
                ViewBag.MyMonth = Monthlist1.Tables[0];
                List<SelectListItem> Monthlist = new List<SelectListItem>();
                //Monthlist.Add(new SelectListItem { Text = "Select Month", Value = "0" });
                foreach (System.Data.DataRow dr in ViewBag.MyMonth.Rows)
                {
                    Monthlist.Add(new SelectListItem { Text = @dr["sessionMonth"].ToString(), Value = @dr["sessionMonth"].ToString() });
                }
                ViewBag.MyMonth = Monthlist;
                return Json(Monthlist);
            }
        }
        public JsonResult GetYearID(string CATE) // Calling on http post (on Submit)
        {
            AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
            PunAddModels MS = new PunAddModels();
            if (CATE == "A")
            {
                List<SelectListItem> yearlist = objDB.GetSessionYear1();
                //yearlist.Add(new SelectListItem { Text = "Select Year", Value = "0" });
                ViewBag.MyYear = yearlist;
                return Json(yearlist);
            }
            else if (CATE == "D")
            {
                DataSet yearlist1 = objDB.GetSessionYear();
                ViewBag.MyYear = yearlist1.Tables[0];
                List<SelectListItem> yearlist = new List<SelectListItem>();
                //yearlist.Add(new SelectListItem { Text = "Select Year", Value = "0" });
                foreach (System.Data.DataRow dr in ViewBag.MyYear.Rows)
                {
                    yearlist.Add(new SelectListItem { Text = @dr["sessionYear"].ToString(), Value = @dr["sessionYear"].ToString() });
                }
                yearlist.RemoveAt(2);
                ViewBag.MyYear = yearlist;
                return Json(yearlist);
            }
            else
            {
                DataSet yearlist1 = objDB.GetSessionYear();
                ViewBag.MyYear = yearlist1.Tables[0];
                List<SelectListItem> yearlist = new List<SelectListItem>();
                //yearlist.Add(new SelectListItem { Text = "Select Year", Value = "0" });
                foreach (System.Data.DataRow dr in ViewBag.MyYear.Rows)
                {
                    yearlist.Add(new SelectListItem { Text = @dr["sessionYear"].ToString(), Value = @dr["sessionYear"].ToString() });
                }
                ViewBag.MyYear = yearlist;
                return Json(yearlist);
            }

        }

        public ActionResult PunAddConfirmationEdit()
        {
            try
            {
                AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
                PunAddModels MS = new PunAddModels();

                DataSet result = objDB.SelectDist(); // passing Value to DBClass from model               
                if (Session["refno"].ToString() == null || Session["refno"].ToString() == "")
                {
                    return PunAddExamination();
                }
                string Oroll = Session["Oroll"].ToString();
                string RefNo = Session["refno"].ToString();
                MS.OROLL = Oroll;
                MS.refNo = RefNo;
                int result1 = objDB.EditPunAddConfirmation(MS);
                return RedirectToAction("PunAddConfirmation", "PunAdd", new { id = MS.refNo });
            }
            catch (Exception ex)
            {
                return RedirectToAction("PunAddExamination", "PunAdd");
            }
        }
        public ActionResult PunAddFinalPrint(FormCollection frc)
        {
            PunAddModels MS = new PunAddModels();
            AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
            try
            {
                if (Session["OROLL"].ToString() != null || Session["OROLL"].ToString() != "")
                {
                    MS.OROLL = Session["OROLL"].ToString();
                    MS.refNo = Session["refNo"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmationPrint(MS.refNo);

                    ///----------------------------------------------------------------
                    MS.IsHardCopyCertificate = MS.StoreAllData.Tables[0].Rows[0]["IsHardCopyCertificate"].ToString();
                    MS.batch = MS.StoreAllData.Tables[0].Rows[0]["batch"].ToString();
                    MS.centrCode = MS.StoreAllData.Tables[0].Rows[0]["cent"].ToString();
                    MS.setNo = MS.StoreAllData.Tables[0].Rows[0]["set"].ToString();
                    MS.SelExamDist = MS.StoreAllData.Tables[0].Rows[0]["ExamDist"].ToString();
                    MS.distName = MS.StoreAllData.Tables[0].Rows[0]["DISTNM"].ToString();
                    MS.distCode = MS.StoreAllData.Tables[0].Rows[0]["DIST1"].ToString();
                    MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["utype"].ToString();
                    MS.category = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                    if (MS.category == "R")
                    {
                        MS.category = "Reappear/Compartment";
                    }
                    if (MS.category == "D")
                    {
                        MS.category = "Division improvement";
                    }
                    if (MS.category == "A")
                    {
                        MS.category = "Additional subject";
                    }
                    MS.phyChal = MS.StoreAllData.Tables[0].Rows[0]["phy_chal"].ToString();

                    MS.Choice1 = MS.StoreAllData.Tables[0].Rows[0]["cent_1"].ToString();
                    MS.Choice2 = MS.StoreAllData.Tables[0].Rows[0]["cent_2"].ToString();
                    @ViewBag.Photo = "../../upload/Upload2023/" + MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
                    @ViewBag.sign = "../../upload/Upload2023/" + MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();

                    MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                    MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
                    MS.Session = MS.SelMonth + '/' + MS.SelYear;
                    MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                    MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();

                    //MS.Fee = MS.StoreAllData.Tables[0].Rows[0]["FEE"].ToString();
                    //MS.challanNo = MS.StoreAllData.Tables[0].Rows[0]["challanid"].ToString();
                    //MS.BANK = MS.StoreAllData.Tables[0].Rows[0]["BANK"].ToString();
                    //MS.BRANCH = MS.StoreAllData.Tables[0].Rows[0]["BRANCH"].ToString();
                    //MS.DEPOSITDT = MS.StoreAllData.Tables[0].Rows[0]["DEPOSITDT"].ToString();

                    MS.refNo = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                    MS.SCHL = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                    MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                    MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                    MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();
                    MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
                    MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
                    MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();
                    MS.EPname = MS.Candi_Name + '/' + MS.Pname;
                    MS.EPFname = MS.Father_Name + '/' + MS.PFname;
                    MS.EPMname = MS.Mother_Name + '/' + MS.PMname;

                    MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["sex"].ToString();
                    MS.CastList = MS.StoreAllData.Tables[0].Rows[0]["caste"].ToString();
                    MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                    MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();
                    MS.Result = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();
                    MS.SearchResult = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();

                    MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();
                    MS.Relist = MS.StoreAllData.Tables[0].Rows[0]["religion"].ToString();
                    if (MS.Relist == "H")
                    {
                        MS.Relist = "Hindu";
                    }
                    if (MS.Relist == "M")
                    {
                        MS.Relist = "Muslim";
                    }
                    if (MS.Relist == "S")
                    {
                        MS.Relist = "Sikh";
                    }
                    if (MS.Relist == "C")
                    {
                        MS.Relist = "Christian";
                    }
                    if (MS.Relist == "O")
                    {
                        MS.Relist = "Others";
                    }

                    MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                    MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                    MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString();
                    MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString();
                    MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString();
                    MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString();

                    MS.sub1code = MS.StoreAllData.Tables[0].Rows[0]["rsub1abbr"].ToString();
                    MS.sub2code = MS.StoreAllData.Tables[0].Rows[0]["rsub2abbr"].ToString();
                    MS.sub3code = MS.StoreAllData.Tables[0].Rows[0]["rsub3abbr"].ToString();
                    MS.sub4code = MS.StoreAllData.Tables[0].Rows[0]["rsub4abbr"].ToString();
                    MS.sub5code = MS.StoreAllData.Tables[0].Rows[0]["rsub5abbr"].ToString();
                    MS.sub6code = MS.StoreAllData.Tables[0].Rows[0]["rsub6abbr"].ToString();

                    MS.address = MS.StoreAllData.Tables[0].Rows[0]["homeadd"].ToString();
                    MS.block = MS.StoreAllData.Tables[0].Rows[0]["block"].ToString();
                    MS.tehsil = MS.StoreAllData.Tables[0].Rows[0]["TEHSILENM"].ToString();
                    MS.pinCode = MS.StoreAllData.Tables[0].Rows[0]["pinCode"].ToString();

                    MS.addressfull = (MS.address + ',' + MS.block + ',' + MS.distName + ',' + MS.tehsil + ',' + MS.pinCode);




                    return View(MS);
                }
                else
                {
                    return PunAddExamination();
                }
            }
            catch (Exception ex)
            {
                return PunAddExamination();
            }
        }
        //public void SendEmailMobileMsg(string Email, string Mobile, string roll, string refNo, string info)
        //{
        //    ViewBag.SubmitValue = "Send";
        //    //string sid = lm.SchoolId;
        //    AbstractLayer.DBClass dbclass = new AbstractLayer.DBClass();

        //    DataSet ds = new DataSet();
        //    //ds = dbclass.SearchEmailID(sid);
        //    if (ds.Tables.Count > 0)
        //    {
        //        if (ds.Tables[0].Rows.Count > 0)
        //        {

        //            ///-------------------------------------------------------------
        //            //if (dsChallanDetails.Tables[0].Rows.Count > 0)
        //            //{
        //            //    int OutStatus;
        //            //    string Mobile = "0";
        //            //    BM.LOT = Convert.ToInt32(dsChallanDetails.Tables[0].Rows[0]["LOT"].ToString());
        //            //    //int UPLOADLOT = Convert.ToInt32(dsChallanDetails.Tables[0].Rows[0]["DOWNLDFLOT"].ToString());
        //            //    //int UPLOADLOT = Convert.ToInt32(dsChallanDetails.Tables[0].Rows[0]["UPLOADLOT"].ToString());
        //            //    //int UPLOADLOT = Convert.ToInt32(dsChallanDetails.Tables[1].Rows[0]["UPLOADLOT"].ToString()); //Changed By Amar Jnana
        //            //    //ImportBankMis
        //            //    DataTable dtResult = objDB.ImportBankMis(BM, UPLOADLOT, out OutStatus, out Mobile);// OutStatus mobile
        //            //    if (OutStatus > 0)
        //            //    {
        //            //        //  string Sms = "Your Challan no XXXXXXXXXX of  lot no XXX has been successfully verified. Kindly download your final print from school login.";// Thanks Team, Online Support, PSEB
        //            string Sms = "You are successfully registred for Class " + info + " March 2017 and your refrence no. " + refNo + " is generated against old roll no. " + roll + ", Keep this for further use till result declaration.";
        //            try
        //            {
        //                if (Mobile != "0" || Mobile != "")
        //                {
        //                    //string getSms = dbclass.gosms(Mobile, Sms);
        //                    // string getSms = objCommon.gosms("9711819184", Sms);
        //                }
        //            }
        //            catch (Exception) { }

        //            //}
        //            //else
        //            //{
        //            //    //ViewData["Result"] = "0";
        //            //    //return View();
        //            //    int RowNo = i + 2;
        //            //    Result1 += "Challan not/already uploaded, please check ChallanId " + BM.CHALLANID + " in row  " + RowNo + ",  ";
        //            //    // ViewBag.Result1 = Result1;
        //            //}
        //        }


        //        ///--------------------------------------------------------------


        //        //string SchoolNameWithCode = ds.Tables[0].Rows[0]["SCHLE"].ToString() + "(" + sid + ")";

        //        string password = ds.Tables[0].Rows[0]["PASSWORD"].ToString();
        //        string to = ds.Tables[0].Rows[0]["EMAILID"].ToString();

        //        string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Dear " + roll + "</b>,</td></tr><tr><td height=" + 30 + ">As per your request Dated <b>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</b> Regarding Private Form</td></tr><tr><td><b>Your Details are given Below:-</b><br /><b>Old Roll No. :</b> " + roll + "<br /><b>Reference No. :</b> " + refNo + "<br /></td></tr><tr><td height=" + 30 + "><b>Click Here To Login</b> <a href=https://https://registration2023.pseb.ac.in target = _blank>https://registration2023.pseb.ac.in</a></td></tr><tr><td><b>Note:</b> Please Read Instruction Carefully Before filling the Online Form .</td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /><tr><td><b>Contact Us</b><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 8058911911<br>DISTRICTS:- BARNALA, FATEHGARH SAHIB, GURDASPUR, HOSHIARPUR, JALANDHAR, KAPURTHALA, SHRI MUKTSAR SAHIB, S.B.S. NAGAR, PATHANKOT, PATIALA, SANGRUR, CHANDIGARH &amp; OTHER STATES<br><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 8058911911<br>DISTRICTS:- AMRITSAR, BATHINDA, FARIDKOT, FAZILKA, FEROZEPUR, LUDHIANA, MANSA, MOGA, ROOP NAGAR, S.A.S NAGAR,TARN TARAN<br></td></tr>";

        //        string subject = "PSEB-Private Form Notification";
        //        bool result = dbclass.mail(subject, body, Email);                
        //    }
        //    else
        //    {
        //        ViewData["result"] = "-1";
        //        ViewBag.Message = "Incorrect School Code ....";
        //    }
        //}

        //----------------------------------------- Challan ---------------------------------
        public ActionResult PaymentForm(string roll)
        {
            try
            {
                PunAddPaymentformViewModel pfvm = new PunAddPaymentformViewModel();
                AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
                if (Session["Oroll"] == null || Session["Oroll"].ToString() == "" || Session["form"] == null)
                {
                    return RedirectToAction("PunAddExamination", "PunAdd");
                }


                roll = Session["Oroll"].ToString();
                string RefNo = Session["refno"].ToString();

                string form = Session["form"].ToString();
                DataSet ds = objDB.GetPunAddDetailsPayment(RefNo, form);
                pfvm.PaymentFormData = ds;
                if (pfvm.PaymentFormData == null || pfvm.PaymentFormData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    //pfvm.LOTNo = Convert.ToInt32(ds.Tables[0].Rows[0]["LOT"].ToString());

                    pfvm.Class = "Matriculation";
                    pfvm.ExamType = "Private";
                    pfvm.category = "Punjabi Additional";

                    pfvm.Name = ds.Tables[0].Rows[0]["name"].ToString();
                    pfvm.RegNo = ds.Tables[0].Rows[0]["regno"].ToString();
                    pfvm.RefNo = ds.Tables[0].Rows[0]["refno"].ToString();
                    pfvm.roll = ds.Tables[0].Rows[0]["roll"].ToString();


                    pfvm.Dist = ds.Tables[0].Rows[0]["homedistco"].ToString();
                    pfvm.District = ds.Tables[0].Rows[0]["DISTNM"].ToString();

                    pfvm.SchoolCode = "";

                    ViewBag.TotalCount = pfvm.PaymentFormData.Tables[0].Rows.Count;

                    DataSet dscalFee = ds; //(DataSet)Session["CalculateFee"];
                    pfvm.TotalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["fee"].ToString());
                    pfvm.TotalLateFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["latefee"].ToString());
                    pfvm.TotalCertFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["certfee"].ToString());
                    pfvm.TotalFinalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["totfee"].ToString());

                    string rps = NumberToWords(Convert.ToInt32(dscalFee.Tables[1].Rows[0]["totfee"].ToString()));

                    pfvm.TotalFeesInWords = rps;


                    pfvm.FeeDate = Convert.ToDateTime(dscalFee.Tables[1].Rows[0]["banklastdate"].ToString());

                    //TotalCandidates

                    pfvm.FeeCode = dscalFee.Tables[1].Rows[0]["FEECODE"].ToString();
                    pfvm.FeeCategory = dscalFee.Tables[1].Rows[0]["FEECAT"].ToString();
                    pfvm.AllowBank = dscalFee.Tables[1].Rows[0]["AllowBanks"].ToString();

                    Session["PaymentForm"] = pfvm;


                    //string CheckFee = ds.Tables[1].Rows[0]["TotalFeeAmount"].ToString();
                    if (pfvm.TotalFinalFees == 0 && pfvm.TotalFees == 0)
                    {
                        ViewBag.CheckForm = 1; // only verify for M1 and T1 
                        Session["CheckFormFee"] = 0;
                    }
                    else
                    {
                        ViewBag.CheckForm = 0; // only verify for M1 and T1 
                        Session["CheckFormFee"] = 1;
                    }
                    return View(pfvm);
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("PunAddExamination", "PunAdd");
            }
        }
        [HttpPost]
        public ActionResult PaymentForm(PrivatePaymentformViewModel pfvm, FormCollection frm, string PayModValue, string AllowBanks)
        {
            try
            {
                PunAddChallanMasterModel CM = new PunAddChallanMasterModel();
                AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
                if (Session["Oroll"] == null || Session["Oroll"].ToString() == "")
                {
                    return RedirectToAction("PunAddExamination", "PunAdd");
                }
                string roll = Session["Oroll"].ToString();


                string bankName = "";
                AllowBanks = pfvm.BankCode;
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
                    PayModValue = "CASH";
                    bankName = "PSEB HOD";
                }
                else if (AllowBanks == "202" || AllowBanks == "204")
                {
                    PayModValue = "CASH";
                    if (AllowBanks == "202")
                    {
                        bankName = "Punjab National Bank";
                    }
                    else if (AllowBanks == "204")
                    {
                        bankName = "State Bank of India";
                    }
                }
                pfvm.BankName = bankName;

                if (ModelState.IsValid)
                {
                    PunAddPaymentformViewModel PFVMSession = (PunAddPaymentformViewModel)Session["PaymentForm"];
                    CM.roll = roll;
                    //CM.FEE = Convert.ToInt32(PFVMSession.TotalFinalFees);
                    CM.FEE = Convert.ToInt32(PFVMSession.TotalFees);
                    CM.latefee = Convert.ToInt32(PFVMSession.TotalLateFees);
                    CM.addsubfee = Convert.ToInt32(PFVMSession.TotalCertFees);
                    CM.TOTFEE = Convert.ToInt32(PFVMSession.TotalFinalFees);
                    CM.FEECAT = PFVMSession.FeeCategory;
                    CM.FEECODE = PFVMSession.FeeCode;
                    CM.FEEMODE = PayModValue.ToUpper();//payment mode
                    CM.BANK = pfvm.BankName;
                    CM.BCODE = pfvm.BankCode;
                    CM.BANKCHRG = PFVMSession.BankCharges;

                    CM.SchoolCode = PFVMSession.SchoolCode.ToString();
                    CM.DIST = PFVMSession.Dist.ToString();
                    CM.DISTNM = PFVMSession.District;
                    CM.LOT = 1;


                    CM.SCHLREGID = PFVMSession.roll.ToString();
                    CM.FeeStudentList = PFVMSession.RefNo.ToString();
                    CM.APPNO = PFVMSession.RefNo.ToString();


                    CM.type = "candt";
                    CM.CHLNVDATE = DateTime.Now.ToString("dd/MM/yyyy"); //PFVMSession.FeeDate;
                    CM.ChallanVDateN = PFVMSession.FeeDate; //PFVMSession.FeeDate;                


                    string CandiMobile = "";

                    if (pfvm.BankCode == null)
                    {
                        ViewBag.Message = "Please Select Bank";
                        ViewData["SelectBank"] = "1";
                        return View(pfvm);
                    }

                    if (Session["CheckFormFee"].ToString() == "0")
                    { pfvm.BankCode = "203"; }

                    string result = objDB.InsertPaymentFormPunAdd(CM, frm, out CandiMobile);
                    if (result == "0")
                    {
                        //--------------Not saved
                        ViewData["result"] = 0;
                    }
                    else if (result == "-1")
                    {
                        //-----alredy exist
                        ViewData["result"] = -1;
                        return RedirectToAction("PunAddConfirmation", "PunAdd");
                    }
                    else
                    {
                        //Session["PUNADDChallanID"] = result;
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
                                string udf3CustMob = encrypt.QueryStringModule.Encrypt(CandiMobile);

                                //AtomCheckoutUrl(string ChallanNo, string amt, string clientCode, string cmn, string cme, string cmno)
                                return RedirectToAction("AtomCheckoutUrl", "Gateway", new { ChallanNo = TransactionID, amt = TransactionAmount, clientCode = clientCode, cmn = udf1CustName, cme = udf2CustEmail, cmno = udf3CustMob });

                            }
                            #endregion Payment Gateyway
                        }
                        else
                        {
                            CM.CHLNVDATE = (Convert.ToString(PFVMSession.FeeDate)).Substring(0, 10);
                            //  string Sms = "Your Challan no. " + result + " generated  for Catg " + CM.category + " and Ref No. " + CM.FeeStudentList + " valid till Dt " + CM.CHLNVDATE + ". Regards PSEB";
                            try
                            {
                                //    string getSms = objCommon.gosms(CandiMobile, Sms);
                                // string getSms = objCommon.gosms("9711819184", Sms);
                            }
                            catch (Exception) { }

                            ModelState.Clear();
                            //--For Showing Message---------//                   
                            return RedirectToAction("GenerateChallaan", "PunAdd", new { ChallanId = result });
                        }
                        //--For Showing Message---------//                   

                    }
                }
                return View(pfvm);
            }
            catch (Exception)
            {
                return RedirectToAction("PunAddExamination", "PunAdd");
            }
        }
        public ActionResult GenerateChallaan()
        {
            try
            {
                Session["Session"] = "2023-2024";
                Session["RoleType"] = "PunAdd";
                string ChallanId = Session["ChallanID"].ToString();
                if (ChallanId == null || ChallanId == "0")
                {
                    return RedirectToAction("PunAddExamination", "PunAdd");
                }

                if (Session["Oroll"] == null || Session["Oroll"].ToString() == "")
                {
                    return RedirectToAction("PunAddExamination", "PunAdd");
                }
                //PrivateChallanMasterModel CM = new PrivateChallanMasterModel();
                ChallanMasterModel CM = new ChallanMasterModel();
                AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
                string schl = "";
                if (Convert.ToString(Session["SCHL"]) != "")
                {
                    schl = Session["SCHL"].ToString();
                }
                string ChallanId1 = ChallanId.ToString();

                DataSet ds = objDB.GetChallanDetailsById(ChallanId1);
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

                    CM.FEE = float.Parse(ds.Tables[0].Rows[0]["FEE"].ToString());
                    CM.latefee = int.Parse(ds.Tables[0].Rows[0]["latefee"].ToString());
                    CM.TOTFEE = float.Parse(ds.Tables[0].Rows[0]["PaidFees"].ToString());
                    CM.FEECAT = ds.Tables[0].Rows[0]["FEECAT"].ToString();
                    CM.FEECODE = ds.Tables[0].Rows[0]["FEECODE"].ToString();
                    CM.FEEMODE = ds.Tables[0].Rows[0]["FEEMODE"].ToString();
                    CM.BANK = ds.Tables[0].Rows[0]["BANK"].ToString();
                    ViewBag.BCODE = CM.BCODE = ds.Tables[0].Rows[0]["BCODE"].ToString();
                    CM.BANKCHRG = float.Parse(ds.Tables[0].Rows[0]["BANKCHRG"].ToString());
                    //CM.SchoolCode = ds.Tables[0].Rows[0]["SchoolCode"].ToString();
                    CM.SchoolCode = ds.Tables[0].Rows[0]["SCHLREGID"].ToString();
                    CM.SCHLREGID = ds.Tables[0].Rows[0]["SCHLREGID"].ToString();
                    CM.APPNO = ds.Tables[0].Rows[0]["APPNO"].ToString();// +" / "+ ds.Tables[0].Rows[0]["SCHLREGID"].ToString();
                                                                        // CM.SchoolCode = Session["SCHL"].ToString();
                    CM.DIST = ds.Tables[0].Rows[0]["DIST"].ToString();
                    CM.DISTNM = ds.Tables[0].Rows[0]["DISTNM"].ToString();
                    CM.LOT = Convert.ToInt32(ds.Tables[0].Rows[0]["LOT"].ToString());
                    CM.TotalFeesInWords = ds.Tables[0].Rows[0]["TotalFeesInWords"].ToString();
                    //CM.SchoolName = ds.Tables[0].Rows[0]["SchoolName"].ToString();
                    CM.SCHLCANDNM = ds.Tables[0].Rows[0]["candidateNM"].ToString();
                    CM.DepositoryMobile = ds.Tables[0].Rows[0]["DepositoryMobile"].ToString();
                    CM.type = ds.Tables[0].Rows[0]["type"].ToString();
                    if (ds.Tables[0].Rows[0]["Verified"].ToString() == "1")
                    {
                        CM.BRCODE = ds.Tables[0].Rows[0]["BRCODE"].ToString();
                        CM.BRANCH = ds.Tables[0].Rows[0]["BRANCH"].ToString();
                        CM.J_REF_NO = ds.Tables[0].Rows[0]["J_REF_NO"].ToString();
                        CM.DEPOSITDT = ds.Tables[0].Rows[0]["DEPOSITDT"].ToString();
                    }

                    Session["CalculateFee"] = null;
                    Session["PaymentForm"] = null;
                    Session["FeeStudentList"] = null;
                    return View(CM);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("PunAddExamination", "PunAdd");
            }
        }

        //public ActionResult ReGenerateChallaan(string ChallanId)
        //{
        //    if (ChallanId == null)
        //    {
        //        return RedirectToAction("PaymentForm", "Home");
        //    }
        //    ChallanMasterModel CM = new ChallanMasterModel();
        //    if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    string schl = Session["SCHL"].ToString();
        //    string ChallanId1 = ChallanId.ToString();
        //    string Usertype = "User";
        //    int OutStatus;
        //    DataSet ds = objDB.ReGenerateChallaanById(ChallanId1, Usertype, out OutStatus);
        //    if (OutStatus == 1)
        //    {
        //        Session["resultReGenerate"] = "1";
        //        ViewBag.Message = "Re Generate Challaan SuccessFully";
        //        return RedirectToAction("FinalPrint", "Home");
        //    }
        //    else
        //    {
        //        Session["resultReGenerate"] = "0";
        //        ViewBag.Message = "Re Generate Challaan Failure";
        //        return RedirectToAction("FinalPrint", "Home");
        //    }

        //}

        public string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

        #region Begin final Submit
        public ActionResult FinalSubmit(FormCollection frc)
        {
            PunAddModels MS = new PunAddModels();
            AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
            try
            {
                if (Session["OROLL"].ToString() != null || Session["OROLL"].ToString() != "")
                {
                    MS.OROLL = Session["OROLL"].ToString();
                    MS.refNo = Session["refNo"].ToString();
                    MS.StoreAllData = objDB.GetPrivateCandidateConfirmationFinalSubmit(MS.refNo);
                    //MS.StoreAllData = objDB.GetPrivateCandidateConfirmationPrint(MS.refNo);
                    if (MS.StoreAllData.Tables[0].Rows[0]["result"].ToString() == "1")
                    {
                        ViewData["roll"] = Session["OROLL"].ToString();
                        ViewData["refno"] = Session["refNo"].ToString();
                        ViewData["Status"] = "1";
                        //return RedirectToAction("PunAddFinalPrint", "PunAdd");
                        return RedirectToAction("PunAddConfirmation", "PunAdd");
                    }
                    else
                    {
                        return PunAddExamination();
                    }

                }
                else
                {
                    return PunAddExamination();
                }
            }
            catch (Exception ex)
            {
                return PunAddExamination();
            }
        }
        #endregion end final Submit

        //------------------------ Forgot Password
        public ActionResult PunAddForgotPassword()
        {

            AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
            PunAddModels MS = new PunAddModels();

            List<SelectListItem> yearlist = objDB.GetSessionYear1();
            ViewBag.MyYear = yearlist;


            List<SelectListItem> Monthlist = objDB.GetMonth();
            ViewBag.MyMonth = Monthlist;

            return View();
        }
        [HttpPost]
        public ActionResult PunAddForgotPassword(FormCollection frm, PunAddModels MS)
        {
            AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();
            //PrivateCandidateModels MS = new PrivateCandidateModels();
            try
            {
                List<SelectListItem> yearlist = objDB.GetSessionYear1();
                ViewBag.MyYear = yearlist;


                List<SelectListItem> Monthlist = objDB.GetMonth();
                ViewBag.MyMonth = Monthlist;

                MS.Class = frm["Class"];
                MS.category = frm["category"];
                MS.Exam_Type = frm["Exam_Type"];
                MS.SelMonth = frm["SelMonth"];
                MS.SelYear = frm["SelYear"];
                MS.OROLL = frm["OROLL"];
                MS.emailID = frm["emailID"];
                MS.mobileNo = frm["mobileNo"];

                if (frm["OROLL"] != null && frm["OROLL"] != "")
                {
                    MS.StoreAllData = objDB.getForgotPassword(MS);

                    if (MS.StoreAllData.Tables[0].Rows.Count > 0)
                    {

                        ViewData["refno"] = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                        ViewData["roll"] = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                        ViewData["Status"] = "1";

                        try
                        {
                            AbstractLayer.DBClass dbclass = new AbstractLayer.DBClass();
                            if (MS.mobileNo != null || MS.mobileNo != "")
                            {
                                string Sms = "Your refrence no. " + ViewData["refno"] + "  against old roll no. " + ViewData["roll"] + ", Keep this for further use till result declaration.";

                                //string getSms = dbclass.gosms(MS.mobileNo, Sms);
                                // string getSms = objCommon.gosms("9711819184", Sms);
                            }
                            //if (MS.emailID != null || MS.emailID != "")
                            //{
                            //    string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Dear " + ViewData["name"] + "</b>,</td></tr><tr><td height=" + 30 + ">As per your request Dated <b>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</b> Regarding Private Form</td></tr><tr><td><b>You are successfully registred for:-</b><br /><b>Class :</b> " + TempData["Classinfo"] + " March 2017 <br /><b> Reference No. :</b> " + ViewData["refno"] + "<br /><b> Old Roll No. :</b> " + ViewData["roll"] + "<br /><b> Keep this for further use till result declaration.</b> <br /></td></tr><tr><td height=" + 30 + "><b>Click Here To Login</b> <a href=https://registration2023.pseb.ac.in/PrivateCandidate/Private_Candidate_Examination_Form target = _blank>https://registration2023.pseb.ac.in</a></td></tr><tr><td><b>Note:</b> Please Read Instruction Carefully Before filling the Online Form .</td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /><tr><td><b>Contact Us</b><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 8058911911<br>DISTRICTS:- BARNALA, FATEHGARH SAHIB, GURDASPUR, HOSHIARPUR, JALANDHAR, KAPURTHALA, SHRI MUKTSAR SAHIB, S.B.S. NAGAR, PATHANKOT, PATIALA, SANGRUR, CHANDIGARH &amp; OTHER STATES<br><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 8058911911<br>DISTRICTS:- AMRITSAR, BATHINDA, FARIDKOT, FAZILKA, FEROZEPUR, LUDHIANA, MANSA, MOGA, ROOP NAGAR, S.A.S NAGAR,TARN TARAN<br></td></tr>";

                            //    string subject = "PSEB-Private Form Notification";
                            //    bool result = dbclass.mail(subject, body, MS.emailID);
                            //}

                        }
                        catch (Exception) { }
                        return View(MS);
                        //return RedirectToAction("MigrationRec", "MigrateSchool");
                    }

                    else
                    {
                        ViewData["roll"] = MS.OROLL;
                        ViewData["Status"] = "0";
                        return View(MS);
                    }

                }
            }
            catch (Exception ex)
            {

                return View(MS);
            }


            return View(MS);
        }


        #region Result Sheet of Candidate Punjabi Additional         
        public ActionResult PunAddResults()
        {

            ViewBag.ResultBatchList = objDB.GetActivePBIResultBatchList();
            ViewBag.Message = "Record Not Found";
            ViewBag.TotalCount = 0;
            return View();

        }
        [HttpPost]
        public ActionResult PunAddResults(FormCollection frm)
        {
            PunAddModels MS = new PunAddModels();
            try
            {

                ViewBag.ResultBatchList = objDB.GetActivePBIResultBatchList();

                MS.batchYear = frm["batchYear"].ToString();
                MS.SearchBy = frm["SearchBy"].ToString();
                MS.SearchString = frm["SearchString"].ToString();
                //MS.batch = frm["batch"].ToString();
                //ViewBag.MybatchYear = new SelectList(items, "Value", "Text");                
                //ViewBag.MybatchYear = items.ToList();

                string selBatch = MS.batchYear.Split('/')[0];
                string selBatchYr = MS.batchYear.Split('/')[1];
                selBatchYr = MS.batchYear.Split('-')[0];
                if (ModelState.IsValid)
                {
                    string Search = string.Empty;
                    if (MS.batchYear != "")
                    {
                        Search = "a.batch='" + selBatch + "' and a.batchYear='" + selBatchYr + "'";
                    }
                    if (MS.SearchBy != "0")
                    {
                        string sString = MS.SearchString.Trim();
                        switch (MS.SearchBy)
                        {
                            case "1": Search += " and a.ExamRoll    ='" + sString + "'"; break;
                            case "2": Search += " and a.refno       ='" + sString + "'"; break;
                            case "3": Search += " and a.name        ='" + sString + "'"; break;
                            case "4": Search += " and a.fname       ='" + sString + "'"; break;
                        }
                    }

                    MS.StoreAllData = objDB.PunAddResults(Search);
                    ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Enter Correct Refno/Roll no";
                        ViewBag.TotalCount = 0;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return View(MS);

        }
        #endregion Result Sheet of Candidate Punjabi Additional

        #region Punjabi Additional Certificate     
        public ActionResult PunAddResultCer()
        {
            return View();
        }
        [HttpPost]
        public ActionResult PunAddResultCer(FormCollection frm)
        {
            string id = frm["SearchString"].ToString();
            PunAddModels MS = new PunAddModels();
            if (id != "" && id != null)
            {
                MS.SearchString = id;
                MS.StoreAllData = objDB.PunAddResultCer(MS);
                ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
                if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message2 = "Record Not Found";
                    ViewBag.TotalCount2 = 0;
                }
            }
            return View(MS);
        }

        #endregion Punjabi Additional Certificate 

        ////public ActionResult PunAddCertPrint()
        ////{
        ////    return View();
        ////}
        #region Admin PunAdd Report
        public ActionResult AdminPunAddReport()
        {
            PunAddModels MS = new PunAddModels();
            ViewBag.ResultBatchList = objDB.GetActivePBIResultBatchList();

            return View();

        }
        [HttpPost]
        public ActionResult AdminPunAddReport(FormCollection frm)
        {
            PunAddModels MS = new PunAddModels();
            try
            {
                ViewBag.ResultBatchList = objDB.GetActivePBIResultBatchList();

                MS.batchYear = frm["batchYear"].ToString();
                MS.SearchBy = "";// frm["SearchBy"].ToString();
                MS.SearchString = "";// frm["SearchString"].ToString();
                MS.setNo = frm["setNo"].ToString();
                //ViewBag.MybatchYear = new SelectList(items, "Value", "Text");                
                ViewBag.MybatchYear = MS.batchYear;// items.ToList();


                string selBatch = MS.batchYear.Split('/')[0];
                string selBatchYr = MS.batchYear.Split('/')[1];
                selBatchYr = selBatchYr.Split('-')[0];

                if (ModelState.IsValid)
                {
                    string Search = string.Empty;
                    if (MS.batchYear != "")
                    {
                        Search = "a.batch='" + selBatch + "' and a.batchYear='" + selBatchYr + "'";
                    }
                    if (MS.SearchBy != "0")
                    {
                        switch (MS.SearchBy)
                        {
                            case "Roll": Search += " and a.ExamRoll='" + MS.SearchString.ToString().Trim() + "'"; break;
                            case "RefNo": Search += " and a.Refno='" + MS.SearchString.ToString().Trim() + "'"; break;
                        }
                    }
                    if (MS.setNo != "0")
                    {
                        switch (MS.setNo)
                        {
                            case "A": Search += " and a.[SET]='A'"; break;
                            case "B": Search += " and a.[SET]='B'"; break;
                        }
                    }
                    //sm.StoreAllData = objDB.ConfidentialListMatric(sm); 
                    MS.StoreAllData = objDB.AdminPunAddReport(Search, MS.SearchBy);
                    ViewBag.TotalCount = MS.StoreAllData.Tables[1].Rows.Count;
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                }
                return View(MS);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Admin");
            }

        }
        #endregion Admin PunAdd Report
        public JsonResult GetCentreID(string BATCH) // Calling on http post (on Submit)
        {
            AbstractLayer.PunAddDB objDB = new AbstractLayer.PunAddDB();

            DataSet result = objDB.SelectAllCentre(BATCH); // passing Value to DBClass from model

            ViewBag.MyCentre = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> CentList = new List<SelectListItem>();
            //List<string> items = new List<string>();
            CentList.Add(new SelectListItem { Text = "---All Centre---", Value = "0" });
            foreach (System.Data.DataRow dr in ViewBag.MyCentre.Rows)
            {

                CentList.Add(new SelectListItem { Text = @dr["ecent"].ToString(), Value = @dr["cent"].ToString() });

            }
            ViewBag.MyCentre = CentList;
            Session["MyCentre"] = ViewBag.MyCentre;
            return Json(CentList);

        }
    }
}