using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSEBONLINE.Models;
using System.Data;
using System.Web.UI;
using System.IO;
using System.Web.Routing;
using ClosedXML;
using ClosedXML.Excel;
using System.Data.OleDb;
using PSEBONLINE.Filters;
using System.Reflection;

namespace PSEBONLINE.Controllers
{

    public class ReportController : Controller
    {
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        #region SiteMenu
        //Executes before every action
        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            string actionName = context.ActionDescriptor.ActionName;
            string controllerName = context.ActionDescriptor.ControllerDescriptor.ControllerName;
            if (actionName == "SummaryReports")
            { actionName = "Report"; controllerName = "Admin"; }
            else if (actionName == "PaymentSummary")
            { actionName = "PaymentSummary"; controllerName = "Admin"; }
            else if (actionName == "CCEGradingReports")
            { actionName = "CCEGradingReport"; controllerName = "Admin"; }
            else if (actionName == "NinthEleventhResultReport")
            { actionName = "NinthEleventhResultReport"; controllerName = "Admin"; }


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

        private void GetRouteActionController(string Act, string cont, out string NewAct, out string NewCont)
        {
            if (Act == "SummaryReports")
            { NewAct = "Report"; NewCont = "Admin"; }
            else if (Act == "PaymentSummary")
            { NewAct = "PaymentSummary"; NewCont = "Admin"; }
            else if (Act == "CCEGradingReports")
            { NewAct = "CCEGradingReport"; NewCont = "Admin"; }
            else
            { NewAct = "Report"; NewCont = "Admin"; }
        }

        #endregion SiteMenu
        private readonly DBContext _context = new DBContext();
        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.ReportDB objDB = new AbstractLayer.ReportDB();
        // GET: Report

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExportToExcelDataFromDataTable(string File, string PageType)
        {
            try
            {

                DataTable dt = new DataTable();

                if (TempData["ExportToExcelDataFromDataTable"] == null)
                {
                    if (PageType.ToLower() == "ClassWiseSchoolWiseReport".ToLower()) { return RedirectToAction("ClassWiseSchoolWiseReport", "Report"); }
                    else if (PageType.ToLower() == "clustersubjectstatusreport") { return RedirectToAction("ClusterSubjectStatusReport", "Report"); }
                    else if (PageType.ToLower() == "subjectwisereport") { return RedirectToAction("ExaminationAllClassWiseSubjectWiseReport", "Report"); }
                    else
                    { return RedirectToAction("Welcome", "Admin"); }
                }
                else
                {
                    dt = (DataTable)TempData["ExportToExcelDataFromDataTable"];
                }

                string fileName1 = "";
                if (dt.Rows.Count > 0)
                {


                    fileName1 = File.Replace(" ", "") + "_" + PageType + "_Data_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";
                    // dt.TableName = fileName1;
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
            catch (Exception ex)
            {

            }

            if (PageType.ToLower() == "ClassWiseSchoolWiseReport".ToLower())
            {
                TempData["ExportToExcel"] = "1";
                return RedirectToAction("ClassWiseSchoolWiseReport", "Report");
            }
            else if (PageType.ToLower() == "clustersubjectstatusreport")
            {
                TempData["ExportToExcel"] = "1";
                return RedirectToAction("ClusterSubjectStatusReport", "Report");
            }
            else if (PageType.ToLower() == "subjectwisereport")
            {
                TempData["ExportToExcel"] = "1";
                return RedirectToAction("ExaminationAllClassWiseSubjectWiseReport", "Report");
            }
            else if (PageType.ToLower() == "MonthWiseFeeCollectionDetails")
            {
                TempData["ExportToExcel"] = "1";
                return RedirectToAction("MonthWiseCategoryWiseFeeCollectionDetails", "Report");
            }
            else
            { return RedirectToAction("Welcome", "Admin"); }


        }

        [Route("Admin/Report")]
        public ActionResult SummaryReports(ReportModel rp, string id = "0", string fromDate = "", string toDate = "")
        {
            //_RegExamFormFeeSummary
            try
            {
                if (Session["AdminId"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }

                ViewBag.tab = id;
                int tid = Convert.ToInt32(id);
                DataSet ds = new DataSet();
                if (id == "11" && fromDate != "" && toDate != "")
                {
                    ds = objDB.RegandExamFormFeeSummarywithDate(tid, fromDate, toDate);
                }
                else
                {
                    ds = objDB.PSEBReport(tid);  //1 for Total Registration by School Report
                }
                rp.StoreAllData = ds;
                if (Request.IsAjaxRequest())
                {
                    //var query = GetCategoryPaginatedProducts(categoryid, page1);
                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                    if (tid == 1)
                        return PartialView("_summaryreport", rp);
                    else if (tid == 2)
                        return PartialView("_formwisereport", rp);
                    else if (tid == 3)
                        return PartialView("_schoolwisereport", rp);
                    else if (tid == 4)
                        return PartialView("_paymentsummary", rp);
                    else if (tid == 5)
                        return PartialView("_opensummaryreport", rp);
                    else if (tid == 6)
                        return PartialView("_openusertypewisereport", rp);
                    else if (tid == 7)
                        return PartialView("_opendistwisereport", rp);
                    else if (tid == 8)
                        return PartialView("_openschoolreport", rp);
                    else if (tid == 9)
                        return PartialView("_regnoerrorreport", rp);
                    else if (tid == 10)
                        return PartialView("_usertypereport", rp);
                    else if (tid == 11)
                        return PartialView("_RegExamFormFeeSummary", rp);
                }
                if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                    return View(rp);
                }
            }
            catch (Exception ex)
            {
                // oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        [Route("Admin/PaymentSummary")]
        public ActionResult PaymentSummary(ReportModel rp)
        {
            try
            {
                if (Session["AdminId"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                //if (Session["AdminType"].ToString() != "admin")
                //{
                //    return RedirectToAction("Index", "Admin");
                //}
                DataSet ds = objDB.PSEBPaymentSummaryReport();  //1 for Total Registration by School Report
                rp.StoreAllData = ds;
                if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                    return View(rp);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        [Route("Admin/CCEGradingReport")]
        public ActionResult CCEGradingReports(ReportModel rp, string id = "0")
        {
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }

                #region Action Assign Method
                if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
                { ViewBag.IsGRADE12 = 1; ViewBag.IsCCE12 = 1; ViewBag.IsGRADE10 = 1; ViewBag.IsCCE10 = 1; }
                else
                {
                    string actionName = "CCEGradingReport";
                    string controllerName = "Admin";
                    int AdminId = Convert.ToInt32(Session["AdminId"]);
                    DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
                    if (aAct.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.IsGRADE12 = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(50)).Count();
                        ViewBag.IsCCE12 = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(51)).Count();
                        ViewBag.IsGRADE10 = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(52)).Count();
                        ViewBag.IsCCE10 = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(53)).Count();

                    }
                }
                #endregion Action Assign Method          

                ViewBag.tab = id;
                int tid = Convert.ToInt32(id);
                DataSet ds = objDB.CCEGradingReport(tid);  //1 for Total Registration by School Report
                rp.StoreAllData = ds;
                if (Request.IsAjaxRequest())
                {
                    //var query = GetCategoryPaginatedProducts(categoryid, page1);
                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                    return PartialView("_ccegradingeport", rp);
                }
                else if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {

                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                    return View(rp);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [Route("Admin/DownloadCCEGradingReport")]
        public ActionResult DownloadCCEGradingReport()
        {
            try
            {
                if (Request.QueryString["File"] == null)
                {
                    return RedirectToAction("CCEGradingReport", "Admin");
                }
                else
                {
                    string FileExport = Request.QueryString["File"].ToString();
                    DataSet ds = null;
                    if (Session["UserName"] != null)
                    {
                        string AdminType = Session["AdminType"].ToString();
                        int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                        string fileName1 = FileExport == "1" ? "SetWise_CCE_Senior_Report" : FileExport == "2" ? "SetWise_Grading_Senior_Report" : FileExport == "3" ? "SetWise_CCE_Matric_Report" : FileExport == "4" ? "SetWise_Grading_Matric_Report" : "Report";
                        string Search = string.Empty;
                        ds = objDB.CCEGradingPendingSchoolList(Convert.ToInt32(FileExport)); // CCEGradingPendingSchoolList                  
                        if (ds == null)
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                bool ResultDownload;
                                try
                                {
                                    switch (FileExport)
                                    {
                                        case "Excel":
                                            using (XLWorkbook wb = new XLWorkbook())
                                            {
                                                ////// wb.Worksheets.Add("PNB-TTAmarEN");//PNB-TTAmarEN for Punjabi                                               
                                                wb.Worksheets.Add(ds);
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
                                            break;
                                        default:
                                            using (XLWorkbook wb = new XLWorkbook())
                                            {
                                                wb.Worksheets.Add(ds);
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
                                            break;

                                    }
                                    ResultDownload = true;
                                }
                                catch (Exception)
                                {
                                    ResultDownload = false;
                                }

                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                }
                return RedirectToAction("CCEGradingReport", "Admin");
            }
            catch (Exception ex)
            {
                // oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("CCEGradingReport", "Admin");
            }
        }

        #region Result 9th 11th Report
        [Route("Admin/NinthEleventhResultReport")]
        public ActionResult NinthEleventhResultReport(ReportModel rp)
        {
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }

                DataSet ds = objDB.NinthEleventhReport();
                rp.StoreAllData = ds;
                if (rp.StoreAllData.Tables[0].Rows.Count > 0 && rp.StoreAllData != null)
                {

                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                    //return View(rp);
                }
                //else if (rp.StoreAllData.Tables[1].Rows.Count > 0 && rp.StoreAllData != null)
                //{

                //    ViewBag.TotalCount = rp.StoreAllData.Tables[1].Rows.Count;
                //    // return View(rp);
                //}
                else
                {
                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.Message = "No records Found";
                    return View(rp);
                }
                return View(rp);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }



        #endregion Result 9th 11th Report

        #region  CategoryWiseFeeCollectionDetails

        public ActionResult CategoryWiseFeeCollectionDetails()
        {
            ReportModel rp = new ReportModel();
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                // By Rohit  -- select bank from database
                DataSet dsBank = objCommon.Fll_FeeCat_Details();
                if (dsBank != null)
                {
                    if (dsBank.Tables[0].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[0].Rows)
                        {
                            //itemBank.Add(new SelectListItem { Text = @dr["FEECAT"].ToString().Trim(), Value = @dr["FEECAT"].ToString().Trim() });
                            itemBank.Add(new SelectListItem { Text = @dr["FEECAT"].ToString().Trim(), Value = @dr["FEECODE"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Admin");
                }

                // End 

                string Search = string.Empty;
                Search = "a.FEECODE like '%' ";

                string outError = "";
                DataSet ds = objDB.CategoryWiseFeeCollectionDetails(Search, out outError);  //1 for Total Registration by School Report
                rp.StoreAllData = ds;
                if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    ViewBag.Total = 0;
                    return View(rp);
                }
                else
                {
                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                    string typeName = rp.StoreAllData.Tables[0].Columns["TOTFEE"].DataType.Name;
                    ViewBag.Total = rp.StoreAllData.Tables[0].AsEnumerable().Sum(x => Convert.ToInt32(x.Field<Double>("TOTFEE")));
                    ViewBag.AmountInWords = objCommon.GetAmountInWords(Convert.ToInt32(ViewBag.Total));
                    return View(rp);
                }
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [HttpPost]
        public ActionResult CategoryWiseFeeCollectionDetails(FormCollection frm)
        {
            ReportModel rp = new ReportModel();
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                // By Rohit  -- select bank from database
                DataSet dsBank = objCommon.Fll_FeeCat_Details();
                if (dsBank != null)
                {
                    if (dsBank.Tables[0].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[0].Rows)
                        {
                            itemBank.Add(new SelectListItem { Text = @dr["FEECAT"].ToString().Trim(), Value = @dr["FEECODE"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Admin");
                }

                // End 
                string Search = string.Empty;
                Search = "a.FEECODE like '%' ";

                if (!string.IsNullOrEmpty(frm["FEECAT"]))
                {
                    Search += " and a.FEECODE in (" + frm["FEECAT"].ToString().Trim() + ")";
                    ViewBag.SelectedItem = frm["FEECAT"];
                    TempData["FEECAT"] = frm["FEECAT"];
                }

                if (frm["Branch"] != "")
                {
                    // Search += " and a.Branch like '%" + frm["Branch"].ToString().Trim() + "%'";
                    Search += " and b.NODAL_BRANCH like '" + frm["Branch"].ToString().Trim() + "'";
                    ViewBag.Branch = frm["Branch"];
                    TempData["Branch"] = frm["Branch"];
                }

                string outError = "";
                DataSet ds = objDB.CategoryWiseFeeCollectionDetails(Search, out outError);  //1 for Total Registration by School Report
                rp.StoreAllData = ds;
                if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    ViewBag.Total = 0;
                    return View(rp);
                }
                else
                {
                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                    string typeName = rp.StoreAllData.Tables[0].Columns["TOTFEE"].DataType.Name;
                    ViewBag.Total = rp.StoreAllData.Tables[0].AsEnumerable().Sum(x => Convert.ToInt32(x.Field<Double>("TOTFEE")));
                    ViewBag.AmountInWords = objCommon.GetAmountInWords(Convert.ToInt32(ViewBag.Total));
                    return View(rp);
                }
            }
            catch (Exception ex)
            {
                // oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        #endregion CategoryWiseFeeCollectionDetails

        #region  BankWiseFeeCollectionDetails

        public ActionResult BankWiseFeeCollectionDetails()
        {
            ReportModel rp = new ReportModel();
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                // By Rohit  -- select bank from database
                DataSet dsBank = objCommon.Fll_FeeCat_Details();
                if (dsBank != null)
                {
                    if (dsBank.Tables[1].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[1].Rows)
                        {
                            itemBank.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Admin");
                }

                // End 

                string Search = string.Empty;
                Search = "a.bcode like '%' ";

                string outError = "";
                DataSet ds = objDB.BankWiseFeeCollectionDetails(Search, out outError);  //1 for Total Registration by School Report
                rp.StoreAllData = ds;
                if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    ViewBag.Total = 0;
                    return View(rp);
                }
                else
                {
                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                    string typeName = rp.StoreAllData.Tables[0].Columns["TOTFEE"].DataType.Name;
                    ViewBag.Total = rp.StoreAllData.Tables[0].AsEnumerable().Sum(x => Convert.ToInt32(x.Field<Double>("TOTFEE")));
                    ViewBag.AmountInWords = objCommon.GetAmountInWords(Convert.ToInt32(ViewBag.Total));
                    return View(rp);
                }
            }
            catch (Exception ex)
            {
                // oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));                
                return View();
            }
        }

        [HttpPost]
        public ActionResult BankWiseFeeCollectionDetails(FormCollection frm)
        {
            ReportModel rp = new ReportModel();
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                // By Rohit  -- select bank from database
                DataSet dsBank = objCommon.Fll_FeeCat_Details();
                if (dsBank != null)
                {
                    if (dsBank.Tables[1].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[1].Rows)
                        {
                            itemBank.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Admin");
                }

                // End 
                string Search = string.Empty;
                Search = "a.bcode like '%' ";

                if (frm["BankName"] != "")
                {
                    Search += " and a.Bcode='" + frm["BankName"].ToString().Trim() + "'";
                    ViewBag.SelectedItem = frm["BankName"];
                    TempData["BankName"] = frm["BankName"];
                }

                if (frm["Branch"] != "")
                {
                    Search += " and b.NODAL_BRANCH like '%" + frm["Branch"].ToString().Trim() + "%'";
                    ViewBag.Branch = frm["Branch"];
                    TempData["Branch"] = frm["Branch"];
                }

                string outError = "";
                DataSet ds = objDB.BankWiseFeeCollectionDetails(Search, out outError);  //1 for Total Registration by School Report
                rp.StoreAllData = ds;
                if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    ViewBag.Total = 0;
                    return View(rp);
                }
                else
                {
                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                    string typeName = rp.StoreAllData.Tables[0].Columns["TOTFEE"].DataType.Name;
                    ViewBag.Total = rp.StoreAllData.Tables[0].AsEnumerable().Sum(x => Convert.ToInt32(x.Field<Double>("TOTFEE")));
                    ViewBag.AmountInWords = objCommon.GetAmountInWords(Convert.ToInt32(ViewBag.Total));
                    return View(rp);
                }
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        #endregion BankWiseFeeCollectionDetails

        //------------------------Report Controller------------
        public ActionResult PrivateExamFormsStatus()
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            ReportModel rp = new ReportModel();
            rp.StoreAllData = objDB.getPrivateExamFormsStatus();
            ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
            return View(rp);
        }
        //-----------------End----------------------


        #region RegistrationReport

        public ActionResult RegistrationReport()
        {
            if (Session["AdminId"] == null || Session["AdminId"].ToString().Trim() == string.Empty)
            {
                return RedirectToAction("Index", "Admin");
            }


            var itemschUserType = new SelectList(new[] { new { ID = "G", Name = "Goverment"  },
                 new { ID = "P", Name = "Private" }, }, "ID", "Name", 1);
            ViewBag.MyUserType = itemschUserType.ToList();
            ViewBag.SelectedItemUserType = "0";



            string adminId = Session["AdminId"].ToString().Trim();
            DataSet ds = new DataSet();
            // ds = objDB.AllStudentReport(adminId);
            ds = objDB.RegistrationReport(adminId);
            List<SelectListItem> forms = new List<SelectListItem>();
            List<SelectListItem> districts = new List<SelectListItem>();
            List<SelectListItem> category = new List<SelectListItem>();
            // category.Add(new SelectListItem { Text = "All", Value = "0", Selected = true });
            category.Add(new SelectListItem { Text = "Reg Pending", Value = "1" });
            category.Add(new SelectListItem { Text = "Reg Error", Value = "2" });
            category.Add(new SelectListItem { Text = "Reg Descrepancy", Value = "3" });
            ViewBag.cat = category;
            TempData["category"] = category;

            if (ds.Tables.Count > 1)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        forms.Add(new SelectListItem { Text = dr["form_name"].ToString(), Value = dr["form_name"].ToString() });
                    }
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        districts.Add(new SelectListItem { Text = dr["DISTNM"].ToString(), Value = dr["DIST"].ToString() });
                    }
                }
            }
            // districts.Insert(0, new SelectListItem { Text = "-- Select District --", Value = "0" });
            ViewBag.districts = districts;
            TempData["districts"] = districts;
            //forms.Insert(0, new SelectListItem { Text = "-- Select Form --", Value = "0" });
            ViewBag.forms = forms;
            TempData["forms"] = forms;
            ViewBag.TotalCount = 0;
            return View();
        }

        [HttpPost]
        public ActionResult RegistrationReport(FormCollection fc, string SelUserType)
        {
            if (Session["AdminId"] == null || Session["AdminId"].ToString().Trim() == string.Empty)
            {
                return RedirectToAction("Index", "Admin");
            }

            var itemschUserType = new SelectList(new[] { new { ID = "G", Name = "Goverment"  },
                 new { ID = "P", Name = "Private" }, }, "ID", "Name", 1);
            ViewBag.MyUserType = itemschUserType.ToList();
            ViewBag.SelectedItemUserType = "0";


            string adminId = Session["AdminId"].ToString().Trim();
            DataSet ds = new DataSet();
            // ds = objDB.AllStudentReport(adminId);
            ds = objDB.RegistrationReport(adminId);
            List<SelectListItem> forms = new List<SelectListItem>();
            List<SelectListItem> districts = new List<SelectListItem>();
            List<SelectListItem> category = new List<SelectListItem>();
            // category.Add(new SelectListItem { Text = "All", Value = "0", Selected = true });
            category.Add(new SelectListItem { Text = "Reg Pending", Value = "1" });
            category.Add(new SelectListItem { Text = "Reg Error", Value = "2" });
            category.Add(new SelectListItem { Text = "Reg Descrepancy", Value = "3" });
            ViewBag.cat = category;
            TempData["category"] = category;

            //category = (List<SelectListItem>)TempData["category"];
            //ViewBag.cat = category;
            //TempData["category"] = category;

            forms = (List<SelectListItem>)TempData["forms"];
            districts = (List<SelectListItem>)TempData["districts"];


            ViewBag.districts = districts;
            TempData["districts"] = districts;

            ViewBag.forms = forms;
            TempData["forms"] = forms;

            ViewBag.SelectedDist = "0";
            ViewBag.SelectedForm = "0";
            ViewBag.SelectedCategory = "0";

            string search = "R.std_id is not null ";
            if (!string.IsNullOrEmpty(SelUserType))
            {
                ViewBag.SelectedItemUserType = SelUserType;
            }

            if (fc["district"] != null && fc["district"].ToString() != "0" && fc["district"].ToString() != "")
            {
                search += " and R.DIST='" + fc["district"].ToString() + "'";
                ViewBag.SelectedDist = fc["district"].ToString();
            }

            if (fc["category"] != null && fc["category"].ToString() != "0" && fc["category"].ToString() != "")
            {
                switch (fc["category"].ToString().Trim())
                {
                    case "1": search += " and  isnull(R.regno,'')='' "; break;
                    case "2": search += " and R.regno like 'ERR%'"; break;
                    case "3": search += " and R.regno like '%:ERR%'"; break;
                }
                ViewBag.SelectedCategory = fc["category"].ToString();

                List<SelectListItem> allcat = (List<SelectListItem>)TempData["category"];
                foreach (var i in allcat)
                {
                    if (i.Value.ToUpper() == ViewBag.SelectedCategory.ToUpper())
                    {
                        i.Selected = true;
                        break;
                    }
                }
                ViewBag.cat = allcat;
                TempData["category"] = allcat;

            }
            if (fc["schl"] != null && fc["schl"].ToString().Trim() != string.Empty)
            {
                search += " and R.SCHL ='" + fc["schl"].ToString().Trim() + "'";
                ViewBag.SCHL = fc["schl"].ToString();
            }


            //if (fc["form"] != null && fc["form"].ToString() != "0" && fc["form"].ToString() != "")
            //{
            //    search += " and R.Form_Name ='" + fc["form"].ToString() + "'";
            //    ViewBag.SelectedForm = fc["form"].ToString();
            //}

            if (!string.IsNullOrEmpty(fc["form"]))
            {
                string formList1 = fc["form"];
                //  string formlistspilt = formList1.Split(',');
                string formList = String.Join(",", formList1.Split(',').Select(k => "'" + k.ToString() + "'").ToArray());


                //  string formList = String.Join(",", dataTable.Rows.OfType<DataRow>().Select(k => "'" + k[0].ToString() + "'").ToArray());

                search += " and R.Form_Name in (" + formList + ")";
                ViewBag.SelectedForm = fc["form"];
                TempData["SelectedForm"] = fc["form"];
            }

            ds = new DataSet();
            ds = objDB.RegistrationReportSearch(search, Session["Session"].ToString(), SelUserType);
            ViewBag.data = ds;
            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                ViewBag.TotalCount = 0;
            }
            else
            {
                ViewBag.TotalCount = ds.Tables[0].Rows.Count;
            }

            return View();
        }

        #endregion RegistrationReport

        #region ChallanFormReceivingStatus
        public ActionResult ChallanFormReceivingStatus()
        {
            //ReportModel rp = new ReportModel();
            //List<SelectListItem> feeCodes = new List<SelectListItem>();

            //feeCodes.Add(new SelectListItem { Text = "--All--", Value = "31,32" });
            //feeCodes.Add(new SelectListItem { Text = "SrSec_Reap_June", Value = "31" });
            //feeCodes.Add(new SelectListItem { Text = "Matric_Reap_June", Value = "32" });
            //ViewBag.feecodes = feeCodes;
            //rp.StoreAllData = objDB.Get_ChallanFormReceivingStatus(string.Empty);
            //int totalCount = 0;
            //try { totalCount = rp.StoreAllData.Tables[0].Rows.Count; } catch { }
            //ViewBag.TotalCount = totalCount;
            //return View(rp);

            ReportModel rp = new ReportModel();
            List<SelectListItem> feeCodes = new List<SelectListItem>();
            DataSet result = objDB.Get_feeCodes();
            ViewBag.feecodes = result.Tables[0];
            //feeCodes.Add(new SelectListItem { Text = "--All--", Value = "31,32" });
            foreach (System.Data.DataRow dr in ViewBag.feecodes.Rows)
            {
                feeCodes.Add(new SelectListItem { Text = @dr["feecat"].ToString(), Value = @dr["feecode"].ToString() });
            }
            ViewBag.feecodes = feeCodes;
            // Session["selected_feecode"] = "(" + result.Tables[0].Rows[0]["feecode"].ToString() + ")" + result.Tables[0].Rows[0]["feecat"].ToString();
            Session["selected_feecode"] = "";//"("+ result.Tables[0].Rows[0]["feecode"].ToString() +")"+ result.Tables[0].Rows[0]["feecat"].ToString();
            return View(rp);
        }

        [HttpPost]
        public ActionResult ChallanFormReceivingStatus(FormCollection fc)
        {
            ReportModel rp = new ReportModel();

            string selected_feecode = string.Empty;
            try { selected_feecode = fc["feeCode"].ToString(); } catch { }

            List<SelectListItem> feeCodes = new List<SelectListItem>();
            //feeCodes.Add(new SelectListItem { Text = "--All--", Value = "31,32" });
            //feeCodes.Add(new SelectListItem { Text = "SrSec_Reap_June", Value = "31", Selected = (selected_feecode.Trim() == "31") ? true : false });
            //feeCodes.Add(new SelectListItem { Text = "Matric_Reap_June", Value = "32", Selected = (selected_feecode.Trim() == "32") ? true : false });
            //ViewBag.feecodes = feeCodes;

            DataSet result = objDB.Get_feeCodes();
            ViewBag.feecodes = result.Tables[0];
            //feeCodes.Add(new SelectListItem { Text = "--All--", Value = "31,32" });
            foreach (System.Data.DataRow dr in ViewBag.feecodes.Rows)
            {
                feeCodes.Add(new SelectListItem { Text = @dr["feecat"].ToString(), Value = @dr["feecode"].ToString() });
            }
            ViewBag.feecodes = feeCodes;
            if (selected_feecode.Trim() != string.Empty && selected_feecode.Trim() != "31,32")
            { Session["selected_feecode"] = "(" + selected_feecode + ")" + feeCodes.Find(f => f.Value == selected_feecode).Text; }
            else { Session["selected_feecode"] = string.Empty; }
            rp.feecode = selected_feecode;
            //ViewBag.feecodes = selected_feecode;
            rp.StoreAllData = objDB.Get_ChallanFormReceivingStatus(selected_feecode);
            int totalCount = 0;
            try { totalCount = rp.StoreAllData.Tables[0].Rows.Count; } catch { }
            ViewBag.TotalCount = totalCount;
            return View(rp);
        }
        #endregion ChallanFormReceivingStatus

        #region PrivateCountReport       

        public ActionResult PrivateCountReport(ReportModel rp)
        {
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                ViewBag.SelectedItem = "0";
                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Download Report of Distwise & Tehsilwise Count" }, new { ID = "2", Name = "Download Report of Distwise, Tehsilwise & Subjectwise Count" }, new { ID = "3", Name = "Download Report of Subject Wise Count" }, }, "ID", "Name", 1);
                ViewBag.MyList = itemsch.ToList();
                // By Rohit  -- select bank from database
                ViewBag.FEECAT = objCommon.GetFeeCatRP("P");
                // End

                return View(rp);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }


        [HttpPost]
        public ActionResult PrivateCountReport(ReportModel rp, FormCollection frm)
        {
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Download Report of Distwise & Tehsilwise Count" }, new { ID = "2", Name = "Download Report of Distwise, Tehsilwise & Subjectwise Count" }, new { ID = "3", Name = "Download Report of Subject Wise Count" }, }, "ID", "Name", 1);
                ViewBag.MyList = itemsch.ToList();

                // By Rohit  -- select bank from database
                ViewBag.FEECAT = objCommon.GetFeeCatRP("P");

                // End

                int type = 0;
                if (frm["SelList"] != "")
                {
                    type = Convert.ToInt32(frm["SelList"]);
                    ViewBag.SelectedItem = frm["SelList"];
                }

                string Search = string.Empty;
                Search = "x.dist like '%' ";
                if (frm["FEECAT"] != "")
                {
                    Search += " and FeeCat='" + frm["FEECAT"].ToString().Trim() + "'";
                    ViewBag.SelectedFeeCat = frm["FEECAT"];
                    TempData["FEECAT"] = frm["FEECAT"];

                    List<SelectListItem> allfee = objCommon.GetFeeCatRP("P");
                    foreach (var i in allfee)
                    {
                        if (i.Value.ToUpper() == ViewBag.SelectedFeeCat.ToUpper())
                        {
                            i.Selected = true;
                            break;
                        }
                    }
                    ViewBag.FEECAT = allfee;
                }

                string outError = "";
                string batch = "8";
                DataSet ds = objDB.PrivateCountReport(type, batch, Search, out outError);  //PrivateCountReportSP
                rp.StoreAllData = ds;
                if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    ViewBag.Total = 0;
                }
                else
                {
                    Session["PrivateCountReport"] = ds;
                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                }
                return View(rp);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }


        public ActionResult DownloadPrivateCountReport()
        {
            try
            {
                if (Request.QueryString["File"] == null)
                {
                    return RedirectToAction("PrivateCountReport", "Report");
                }
                if (Session["PrivateCountReport"] == null || Session["PrivateCountReport"].ToString() == "")
                {
                    return RedirectToAction("PrivateCountReport", "Report");
                }

                else
                {

                    string FileExport = Request.QueryString["File"].ToString();
                    DataSet ds = (DataSet)Session["PrivateCountReport"];
                    string fileName1 = string.Empty;
                    fileName1 = "PrivateCountReport_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";  //103_230820162209_347

                    if (Session["UserName"] == null)
                    {
                        return RedirectToAction("Logout", "Admin");
                    }
                    else
                    {
                        if (ds == null)
                        {
                            return RedirectToAction("PrivateCountReport", "Report");
                        }
                        else
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                bool ResultDownload;
                                try
                                {
                                    switch (FileExport)
                                    {
                                        case "Excel":
                                            using (XLWorkbook wb = new XLWorkbook())
                                            {
                                                ////// wb.Worksheets.Add("PNB-TTAmarEN");//PNB-TTAmarEN for Punjabi                                               
                                                wb.Worksheets.Add(ds);
                                                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                                wb.Style.Font.Bold = true;
                                                Response.Clear();
                                                Response.Buffer = true;
                                                Response.Charset = "";
                                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                                Response.AddHeader("content-disposition", "attachment;filename=" + fileName1 + "");
                                                //string style = @"<style> .textmode {PNB-TTAmarEN:\@; } </style>";
                                                //Response.Output.Write("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\">");
                                                //Response.Write(style);
                                                using (MemoryStream MyMemoryStream = new MemoryStream())
                                                {
                                                    wb.SaveAs(MyMemoryStream);
                                                    MyMemoryStream.WriteTo(Response.OutputStream);
                                                    Response.Flush();
                                                    Response.End();
                                                }
                                            }
                                            break;
                                        default:
                                            //  string fileName2 = Session["SCHL"].ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";  //103_230820162209_347
                                            using (XLWorkbook wb = new XLWorkbook())
                                            {
                                                //wb.Worksheets.Add(dt);
                                                wb.Worksheets.Add(ds);
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
                                            break;

                                    }
                                    ResultDownload = true;
                                }
                                catch (Exception)
                                {
                                    ResultDownload = false;
                                }

                            }
                        }
                    }
                }
                return RedirectToAction("PrivateCountReport", "Report");
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("PrivateCountReport", "Report");
            }
        }

        #endregion PrivateCountReport

        #region ExamReport
        public ActionResult SummaryOfExaminationReport()
        {
            if (Session["AdminId"] == null || Session["AdminId"].ToString().Trim() == string.Empty)
            {
                return RedirectToAction("Index", "Admin");
            }
            string adminId = Session["AdminId"].ToString().Trim();
            DataSet ds = new DataSet();
            ReportModel rp = new ReportModel();
            ds = objDB.SummaryOfExaminationReport(adminId);
            rp.StoreAllData = ds;
            if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                ViewBag.Total = 0;
            }
            else
            {

                ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
            }
            return View(rp);

        }
        #endregion ExamReport

        #region Regular School List Report      
        public ActionResult RegSchoolReport(SchoolModels asm)
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
                    AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                    ViewBag.MySch = objCommon.SearchSchoolItems();
                    ViewBag.MySchoolType = objCommon.GetSchool();
                    ViewBag.MyClassType = objCommon.GetClass().Where(s => s.Value != "0").ToList();


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
                        ViewBag.TotalCount = 0;

                        //if (TempData["SearchRegSchoolReport"] != null)
                        //{
                        //    Search += TempData["SearchRegSchoolRecords"].ToString();
                        //    TempData["SelectedItem"] = ViewBag.SelectedItem = TempData["SelectedItem"];
                        //    TempData["SelectedClassType"] = ViewBag.SelectedClassType = TempData["SelectedClassType"];
                        //    TempData["SelectedSchoolType"] = ViewBag.SelectedSchoolType = TempData["SelectedSchoolType"];
                        //    TempData["SelectedDist"] = ViewBag.SelectedDist = TempData["SelectedDist"];

                        //    ASM.StoreAllData = new AbstractLayer.SchoolDB().RegSchoolReport(Search);//RegSchoolListSP
                        //    if (ASM.StoreAllData != null)
                        //    {
                        //        ViewBag.TotalCount = ASM.StoreAllData.Tables[0].Rows.Count;
                        //    }
                        //    else
                        //    {
                        //        ViewBag.TotalCount = 0;
                        //    }
                        //}
                        //else
                        //{
                        //    Search = "sm.Id like '%' and sm.status='Done' and sm.Class!='1' ";
                        //    if (DistAllow != "")
                        //    {
                        //        Search += " and sm.DIST in (" + DistAllow + ")";
                        //    }
                        //    TempData["SearchRegSchoolReport"] = Search;                          
                        //    ASM.StoreAllData = new AbstractLayer.SchoolDB().RegSchoolReport(Search);//RegSchoolListSP
                        //    if (ASM.StoreAllData != null)
                        //    {
                        //        ViewBag.TotalCount = ASM.StoreAllData.Tables[0].Rows.Count;
                        //    }
                        //    else
                        //    {
                        //        ViewBag.TotalCount = 0;
                        //    }

                        //}
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
        public ActionResult RegSchoolReport(FormCollection frm, int? page)
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

                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                SchoolModels ASM = new SchoolModels();
                if (ModelState.IsValid)
                {
                    ViewBag.MySchoolType = objCommon.GetSchool();
                    ViewBag.MyClassType = objCommon.GetClass().Where(s => s.Value != "0").ToList();
                    // bind Dist 
                    ////  ViewBag.MyDist = objCommon.GetDistE(); 
                    ViewBag.MySch = objCommon.SearchSchoolItems();
                    string Search = string.Empty;
                    Search = "sm.Id like '%' and sm.status='Done' and sm.Class!='0' ";
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
                    if (DistAllow != "")
                    {
                        Search += " and sm.DIST in (" + DistAllow + ")";
                    }
                    TempData["SearchRegSchoolReport"] = Search;
                    TempData.Keep(); // to store search value for view
                    ASM.StoreAllData = new AbstractLayer.SchoolDB().RegSchoolReport(Search);//RegSchoolListSP
                    if (ASM.StoreAllData != null)
                    {
                        ViewBag.data = ASM.StoreAllData;
                        ViewBag.TotalCount = ASM.StoreAllData.Tables[0].Rows.Count;
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
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


        #region Regular School List Report      
        public ActionResult RegSchoolReportSummary(SchoolModels asm)
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
                    AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

                    var itemsch = new SelectList(new[]{new {ID="1",Name="School Code"},new{ID="2",Name="School Name"},
                     new{ID="3",Name="School IDNO"},new{ID="4",Name="School Station"},new{ID="5",Name="School Center Code"},new{ID="6",Name="Tehsil"},}, "ID", "Name", 1);
                    ViewBag.MySch = itemsch.ToList();

                    ViewBag.MySchoolType = objCommon.GetSchool();
                    ViewBag.MyClassType = objCommon.GetClass().ToList();


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
                        ViewBag.TotalCount = 0;
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
        public ActionResult RegSchoolReportSummary(FormCollection frm, int? page)
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

                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                SchoolModels ASM = new SchoolModels();
                if (ModelState.IsValid)
                {
                    ViewBag.MySchoolType = objCommon.GetSchool();
                    //ViewBag.MyClassType = objCommon.GetClass().Where(s => s.Value != "1").ToList();
                    ViewBag.MyClassType = objCommon.GetClass().ToList();
                    // bind Dist 
                    ////  ViewBag.MyDist = objCommon.GetDistE();
                    var itemsch = new SelectList(new[]{new {ID="1",Name="School Code"},new{ID="2",Name="School Name"},
                     new{ID="3",Name="School IDNO"},new{ID="4",Name="School Station"},new{ID="5",Name="School Center Code"},new{ID="6",Name="Tehsil"},}, "ID", "Name", 1);
                    ViewBag.MySch = itemsch.ToList();
                    string Search = string.Empty;
                    Search = "sm.Id like '%' and sm.status='Done' ";
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
                            {
                                if (frm["SearchString"].ToString().ToLower().Trim() == "blank")
                                {
                                    Search += " and (sm.STATIONE is null or sm.STATIONE='' ) ";
                                }
                                else
                                {
                                    Search += " and sm.STATIONE like '%" + frm["SearchString"].ToString() + "%'";
                                }
                            }

                            else if (SelValueSch == 5)
                            { Search += " and sm.SCHLE=" + frm["SearchString"].ToString(); }
                            else if (SelValueSch == 6)
                            {
                                if (frm["SearchString"].ToString().ToLower().Trim() == "blank")
                                { Search += " and (sm.tcode is null or sm.tcode='' ) "; }
                                else
                                { Search += " and sm.tcode=" + frm["SearchString"].ToString(); }

                            }
                        }

                    }
                    if (DistAllow != "")
                    {
                        Search += " and sm.DIST in (" + DistAllow + ")";
                    }
                    TempData["SearchRegSchoolReport"] = Search;
                    TempData.Keep(); // to store search value for view
                    ASM.StoreAllData = new AbstractLayer.SchoolDB().RegSchoolReport(Search);//RegSchoolListSP
                    if (ASM.StoreAllData != null)
                    {
                        ViewBag.data = ASM.StoreAllData;
                        ViewBag.TotalCount = ASM.StoreAllData.Tables[0].Rows.Count;
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
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



        #region RegNoStatus
        public ActionResult RegNoStatus()
        {
            if (Session["AdminId"] == null || Session["AdminId"].ToString().Trim() == string.Empty)
            {
                return RedirectToAction("Index", "Admin");
            }
            string adminId = Session["AdminId"].ToString().Trim();
            DataSet ds = new DataSet();
            ReportModel rp = new ReportModel();
            ds = objDB.RegNoStatusReport();
            rp.StoreAllData = ds;
            if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                ViewBag.Total = 0;
            }
            else
            {

                ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
            }
            return View(rp);

        }
        #endregion RegNoStatus

        //#region StatusofCorrection

        //        public ActionResult StatusofCorrection()
        //        {
        //            return View();
        //        }
        //#endregion StatusofCorrection

        public ActionResult StatusofCorrection()
        {
            try
            {
                if (Session["AdminId"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                else
                {
                    ReportModel RM = new ReportModel();
                    AbstractLayer.ReportDB objDB = new AbstractLayer.ReportDB();
                    RM.StoreAllData = objDB.StatusofCorrection();
                    if (RM.StoreAllData != null)
                    {
                        ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                        return View(RM);
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                        return View(RM);
                    }
                }
            }
            catch (Exception)
            {

                throw;
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




        #region CCE Summary Report
        public ActionResult CCESummaryReport()
        {
            if (Session["UserName"] != null)
            {
                var itemsch = new SelectList(new[] { new { ID = "1", Name = "C.C.E" }, new { ID = "2", Name = "Theory" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "5", Name = "Primary" }, new { ID = "8", Name = "Middle" },
                new { ID = "2", Name = "Matric" }, new { ID = "4", Name = "Senior" },}, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult CCESummaryReport(ReportModel RM, FormCollection frm, string Category, string submit) // HttpPostedFileBase file
        {
            try
            {


                var itemsch = new SelectList(new[] { new { ID = "1", Name = "C.C.E" }, new { ID = "2", Name = "Theory" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "5", Name = "Primary" }, new { ID = "8", Name = "Middle" },
                new { ID = "2", Name = "Matric" }, new { ID = "4", Name = "Senior" },}, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                string firm = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
                string id = frm["Filevalue"].ToString();
                Category = id;
                if (Session["UserName"] != null)
                {
                    string AdminType = Session["AdminType"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                    string Search = string.Empty;
                    if (frm["SelList"] != "" && frm["SelClass"] != "")
                    {
                        ViewBag.SelectedItem = frm["SelList"];
                        ViewBag.Selectedcls = frm["SelClass"];
                        //TempData["SearchDuplicateCertificate"] = Search;
                        RM.StoreAllData = objDB.CCESummaryReport(Convert.ToInt32(frm["SelList"]), Convert.ToInt32(frm["SelClass"]));
                        if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                        {

                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                            return View();
                        }
                        else
                        {
                            if (submit != null)
                            {
                                if (submit.ToUpper().Contains("DOWNLOAD"))
                                {
                                    if (RM.StoreAllData.Tables[0] != null)
                                    {
                                        string Type = "";
                                        if (ViewBag.SelectedItem == "1") { Type = "CCE"; }
                                        else { Type = "Theory"; }

                                        string fileName1 = Type.ToUpper() + "_SummaryReport";
                                        ExportDataFromDataTable(RM.StoreAllData.Tables[0], fileName1);
                                    }
                                }
                            }
                            ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                            return View(RM);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "2";
                        ViewBag.TotalCount = 0;
                        return View();
                    }



                }
                else { return RedirectToAction("Index", "Admin"); }
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }

        }

        #endregion CCE Summary Report



        #region Practical Summary Report
        public ActionResult PracticalSummaryReport()
        {
            if (Session["UserName"] != null)
            {
                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Regular" }, new { ID = "2", Name = "Open" }, new { ID = "3", Name = "Pvt" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "4", Name = "Senior" }, new { ID = "2", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult PracticalSummaryReport(ReportModel RM, FormCollection frm, string Category, string submit) // HttpPostedFileBase file
        {
            try
            {

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Regular" }, new { ID = "2", Name = "Open" }, new { ID = "3", Name = "Pvt" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "4", Name = "Senior" }, new { ID = "2", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                string firm = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
                string id = frm["Filevalue"].ToString();
                Category = id;
                if (Session["UserName"] != null)
                {
                    string AdminType = Session["AdminType"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                    string Search = string.Empty;


                    if (submit.ToUpper().Contains("DOWNLOAD"))
                    {
                        if (Session["PracticalSummaryReportDT"] != null)
                        {
                            DataTable dt = new DataTable();
                            dt = (DataTable)Session["PracticalSummaryReportDT"];
                            ViewBag.SelectedItem = frm["SelList"];
                            ViewBag.Selectedcls = frm["SelClass"];

                            string Type = "";
                            string Cls1 = "";
                            if (ViewBag.SelectedItem == "1") { Type = "REG"; }
                            else if (ViewBag.SelectedItem == "2") { Type = "OPEN"; }
                            else { Type = "PVT"; }

                            if (ViewBag.Selectedcls == "2") { Cls1 = "SENIOR"; }
                            else { Cls1 = "MATRIC"; }

                            string fileName1 = Type.ToUpper() + Cls1.ToUpper() + "_PracSummaryReport";
                            ExportDataFromDataTable(dt, fileName1);
                        }
                    }
                    else
                    {

                        if (frm["SelList"] != "" && frm["SelClass"] != "")
                        {
                            ViewBag.SelectedItem = frm["SelList"];
                            ViewBag.Selectedcls = frm["SelClass"];
                            //TempData["SearchDuplicateCertificate"] = Search;
                            RM.StoreAllData = objDB.PracticalSummaryReport(Convert.ToInt32(frm["SelList"]), Convert.ToInt32(frm["SelClass"]));
                            if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                            {

                                ViewBag.Message = "Record Not Found";
                                ViewBag.TotalCount = 0;
                                return View();
                            }
                            else
                            {
                                Session["PracticalSummaryReportDT"] = RM.StoreAllData.Tables[0];
                                //if (submit != null)
                                //{
                                //    if (submit.ToUpper().Contains("DOWNLOAD"))
                                //    {
                                //        if (RM.StoreAllData.Tables[0] != null)
                                //        {
                                //            string Type = "";
                                //            string Cls1 = "";
                                //            if (ViewBag.SelectedItem == "1") { Type = "REG"; }
                                //            else if (ViewBag.SelectedItem == "2") { Type = "OPEN"; }
                                //            else { Type = "PVT"; }

                                //            if (ViewBag.Selectedcls == "2") { Cls1 = "SENIOR"; }                                       
                                //            else { Cls1 = "MATRIC"; }

                                //            string fileName1 = Type.ToUpper() + Cls1.ToUpper() + "_PracSummaryReport" + '_' + DateTime.Now.ToString("ddMMyyyy");
                                //            ExportDataFromDataTable(RM.StoreAllData.Tables[0], fileName1);
                                //        }
                                //    }
                                //}
                                ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                                return View(RM);
                            }
                        }
                        else
                        {
                            ViewBag.Message = "2";
                            ViewBag.TotalCount = 0;
                            return View();
                        }


                    }

                    return View(RM);



                }
                else { return RedirectToAction("Index", "Admin"); }
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }

        }

        #endregion Practical Summary Report


        #region  DateWiseFeeCollectionDetails

        public ActionResult DateWiseFeeCollectionDetails()
        {
            ReportModel rp = new ReportModel();
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                // By Rohit  -- select bank from database
                DataSet dsBank = objCommon.Fll_FeeCat_Details();
                if (dsBank != null)
                {
                    if (dsBank.Tables[0].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[0].Rows)
                        {
                            itemBank.Add(new SelectListItem { Text = @dr["FEECAT"].ToString().Trim(), Value = @dr["FEECODE"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }

                    if (dsBank.Tables[1].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank1 = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[1].Rows)
                        {
                            itemBank1.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MyBank = itemBank1.ToList();
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Admin");
                }

                var itemcls = new SelectList(new[] { new { ID = "1", Name = "Date Fee Head and Bank Wise " }, new { ID = "2", Name = "Date and Fee Head Wise " },
                    new { ID = "3", Name = "Date Wise" }, new { ID = "4", Name = "Fee Head and Bank Wise" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                var itemDateType = new SelectList(new[] { new { ID = "S", Name = "Settlement" },
                    new { ID = "T", Name = "Transaction " },}, "ID", "Name", 1);
                ViewBag.MyDateType = itemDateType.ToList();
                ViewBag.SelectedDateType = "0";

                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                ViewBag.Total = 0;
                return View(rp);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult DateWiseFeeCollectionDetails(FormCollection frm, string submit)
        {
            ReportModel rp = new ReportModel();
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                // By Rohit  -- select bank from database
                DataSet dsBank = objCommon.Fll_FeeCat_Details();
                if (dsBank != null)
                {
                    if (dsBank.Tables[0].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[0].Rows)
                        {
                            itemBank.Add(new SelectListItem { Text = @dr["FEECAT"].ToString().Trim(), Value = @dr["FEECODE"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }

                    if (dsBank.Tables[1].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank1 = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[1].Rows)
                        {
                            itemBank1.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MyBank = itemBank1.ToList();
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Admin");
                }
                var itemcls = new SelectList(new[] { new { ID = "1", Name = "Date Fee Head and Bank Wise " }, new { ID = "2", Name = "Date and Fee Head Wise " },
                    new { ID = "3", Name = "Date Wise" }, new { ID = "4", Name = "Fee Head and Bank Wise" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                var itemDateType = new SelectList(new[] { new { ID = "S", Name = "Settlement" },
                    new { ID = "T", Name = "Transaction " },}, "ID", "Name", 1);
                ViewBag.MyDateType = itemDateType.ToList();
                ViewBag.SelectedDateType = "0";
                // End 
                string Search = string.Empty;
                Search = "a.FEECODE like '%' ";
                string SelDateType = "T";

                if (frm["SelClass"] != "")
                {
                    ViewBag.Selectedcls = frm["SelClass"];
                    TempData["SelClass"] = frm["SelClass"];
                }



                if (!string.IsNullOrEmpty(frm["FEECAT"]))
                {
                    Search += " and a.FEECODE in (" + frm["FEECAT"].ToString().Trim() + ")";
                    ViewBag.SelectedItem = frm["FEECAT"];
                    TempData["FEECAT"] = frm["FEECAT"];
                }
                if (frm["Bank"] != "")
                {
                    Search += " and a.bcode='" + frm["Bank"].ToString().Trim() + "'";
                    ViewBag.SelectedBank = frm["Bank"];
                    TempData["Bank"] = frm["Bank"];
                }


                if (frm["DateType"] != "")
                {
                    SelDateType = frm["DateType"];
                    ViewBag.SelectedDateType = SelDateType;
                    if (SelDateType == "T")
                    {
                        if (frm["FromDate"] != "")
                        {
                            ViewBag.FromDate = frm["FromDate"];
                            TempData["FromDate"] = frm["FromDate"];
                            Search += " and CONVERT(DATETIME, CONVERT(varchar(10),DEPOSITDT,103), 103)>=CONVERT(DATETIME, CONVERT(varchar(10),'" + frm["FromDate"].ToString() + "',103), 103)";
                        }
                        if (frm["ToDate"] != "")
                        {
                            ViewBag.ToDate = frm["ToDate"];
                            TempData["ToDate"] = frm["ToDate"];
                            Search += " and CONVERT(DATETIME, CONVERT(varchar(10),DEPOSITDT,103), 103)<=CONVERT(DATETIME, CONVERT(varchar(10),'" + frm["ToDate"].ToString() + "',103), 103)";
                        }
                    }
                    else if (SelDateType == "S")
                    {
                        if (frm["FromDate"] != "")
                        {
                            ViewBag.FromDate = frm["FromDate"];
                            TempData["FromDate"] = frm["FromDate"];
                            Search += " and CONVERT(DATETIME, CONVERT(varchar(10),SettlementDate,103), 103)>=CONVERT(DATETIME, CONVERT(varchar(10),'" + frm["FromDate"].ToString() + "',103), 103)";
                        }
                        if (frm["ToDate"] != "")
                        {
                            ViewBag.ToDate = frm["ToDate"];
                            TempData["ToDate"] = frm["ToDate"];
                            Search += " and CONVERT(DATETIME, CONVERT(varchar(10),SettlementDate,103), 103)<=CONVERT(DATETIME, CONVERT(varchar(10),'" + frm["ToDate"].ToString() + "',103), 103)";
                        }
                    }
                }

                if (!string.IsNullOrEmpty(submit))
                {
                    string bankcode = frm["Bank"];
                    string feecat = frm["FEECAT"];
                    string FromDate = frm["FromDate"];
                    string ToDate = frm["ToDate"];
                    if (submit.ToLower().Contains("download") || submit.ToLower().Contains("excel"))
                    {
                        string outError1 = "";
                        rp.StoreAllData = objDB.BankWiseChallanMasterData(SelDateType, Search, bankcode, feecat, "0", out outError1);
                        if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                            return View(rp);
                        }
                        else
                        {
                            ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                            string fileName1 = bankcode + "_" + FromDate + "_" + ToDate + "_ChallanMasterData";
                            ExportDataFromDataTable(rp.StoreAllData.Tables[0], fileName1);
                            return View(rp);
                        }
                    }
                }

                string outError = "";
                DataSet ds = objDB.DateWiseFeeCollectionDetails(SelDateType, Search, ViewBag.Selectedcls, out outError);  //1 for Total Registration by School Report
                rp.StoreAllData = ds;
                if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    ViewBag.Total = 0;
                    return View(rp);
                }
                else
                {
                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                    string typeName = rp.StoreAllData.Tables[0].Columns["TOTFEE"].DataType.Name;
                    ViewBag.Total = rp.StoreAllData.Tables[0].AsEnumerable().Sum(x => Convert.ToInt32(x.Field<Double>("TOTFEE")));
                    ViewBag.AmountInWords = objCommon.GetAmountInWords(Convert.ToInt32(ViewBag.Total));
                    return View(rp);
                }
            }
            catch (Exception ex)
            {
                // oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        #endregion DateWiseFeeCollectionDetails


        #region RegistrationReportAllSession

        [AdminLoginCheckFilter]
        public ActionResult RegistrationReportAllSession(ReportModel rm)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            if (Session["AdminId"] == null || Session["AdminId"].ToString().Trim() == string.Empty)
            {
                return RedirectToAction("Index", "Admin");
            }
            ViewBag.SessionList = objCommon.GetSessionAll().Where(s => Convert.ToInt32(s.Value.Substring(0, 4)) <= 2022).ToList().Take(7);

            var itemschUserType = new SelectList(new[] { new { ID = "G", Name = "Goverment"  },
                 new { ID = "P", Name = "Private" }, }, "ID", "Name", 1);
            ViewBag.MyUserType = itemschUserType.ToList();
            ViewBag.SelectedItemUserType = "0";

            string adminId = Session["AdminId"].ToString().Trim();
            DataSet ds = new DataSet();
            // ds = objDB.AllStudentReport(adminId);
            ds = objDB.RegistrationReport(adminId);
            List<SelectListItem> forms = new List<SelectListItem>();
            List<SelectListItem> districts = new List<SelectListItem>();
            List<SelectListItem> category = new List<SelectListItem>();
            // category.Add(new SelectListItem { Text = "All", Value = "0", Selected = true });
            category.Add(new SelectListItem { Text = "Reg Pending", Value = "1" });
            category.Add(new SelectListItem { Text = "Reg Error", Value = "2" });
            category.Add(new SelectListItem { Text = "Reg Descrepancy", Value = "3" });
            ViewBag.cat = category;
            TempData["category"] = category;

            if (ds.Tables.Count > 1)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        forms.Add(new SelectListItem { Text = dr["form_name"].ToString(), Value = dr["form_name"].ToString() });
                    }
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[1].Rows)
                    {
                        districts.Add(new SelectListItem { Text = dr["DISTNM"].ToString(), Value = dr["DIST"].ToString() });
                    }
                }
            }
            // districts.Insert(0, new SelectListItem { Text = "-- Select District --", Value = "0" });
            ViewBag.districts = districts;
            TempData["districts"] = districts;
            //forms.Insert(0, new SelectListItem { Text = "-- Select Form --", Value = "0" });
            ViewBag.forms = forms;
            TempData["forms"] = forms;
            ViewBag.TotalCount = 0;
            return View(rm);
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult RegistrationReportAllSession(FormCollection fc, ReportModel rm, string SelUserType)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            if (Session["AdminId"] == null || Session["AdminId"].ToString().Trim() == string.Empty)
            {
                return RedirectToAction("Index", "Admin");
            }

            ViewBag.SessionList = objCommon.GetSessionAll().Where(s => Convert.ToInt32(s.Value.Substring(0, 4)) <= 2022).ToList().Take(7);


            var itemschUserType = new SelectList(new[] { new { ID = "G", Name = "Goverment"  },
                 new { ID = "P", Name = "Private" }, }, "ID", "Name", 1);
            ViewBag.MyUserType = itemschUserType.ToList();
            ViewBag.SelectedItemUserType = "0";

            string adminId = Session["AdminId"].ToString().Trim();
            DataSet ds = new DataSet();
            // ds = objDB.AllStudentReport(adminId);
            ds = objDB.RegistrationReport(adminId);
            List<SelectListItem> forms = new List<SelectListItem>();
            List<SelectListItem> districts = new List<SelectListItem>();
            List<SelectListItem> category = new List<SelectListItem>();
            // category.Add(new SelectListItem { Text = "All", Value = "0", Selected = true });
            category.Add(new SelectListItem { Text = "Reg Pending", Value = "1" });
            category.Add(new SelectListItem { Text = "Reg Error", Value = "2" });
            category.Add(new SelectListItem { Text = "Reg Descrepancy", Value = "3" });
            ViewBag.cat = category;
            TempData["category"] = category;

            //category = (List<SelectListItem>)TempData["category"];
            //ViewBag.cat = category;
            //TempData["category"] = category;

            forms = (List<SelectListItem>)TempData["forms"];
            districts = (List<SelectListItem>)TempData["districts"];


            ViewBag.districts = districts;
            TempData["districts"] = districts;

            ViewBag.forms = forms;
            TempData["forms"] = forms;

            ViewBag.SelectedDist = "0";
            ViewBag.SelectedForm = "0";
            ViewBag.SelectedCategory = "0";

            string search = "R.std_id is not null ";

            if (!string.IsNullOrEmpty(SelUserType))
            {
                ViewBag.SelectedItemUserType = SelUserType;
            }
            if (fc["Session"] != null && fc["Session"].ToString() != "0" && fc["Session"].ToString() != "")
            {
                ViewBag.SelectedSession = fc["Session"].ToString();
            }

            if (fc["district"] != null && fc["district"].ToString() != "0" && fc["district"].ToString() != "")
            {
                search += " and R.DIST='" + fc["district"].ToString() + "'";
                ViewBag.SelectedDist = fc["district"].ToString();
            }
            if (fc["form"] != null && fc["form"].ToString() != "0" && fc["form"].ToString() != "")
            {
                search += " and R.Form_Name ='" + fc["form"].ToString() + "'";
                ViewBag.SelectedForm = fc["form"].ToString();
            }
            if (fc["category"] != null && fc["category"].ToString() != "0" && fc["category"].ToString() != "")
            {
                switch (fc["category"].ToString().Trim())
                {
                    case "1": search += " and R.regno is null"; break;
                    case "2": search += " and R.regno like 'ERR%'"; break;
                    case "3": search += " and R.regno like '%:ERR%'"; break;
                }
                ViewBag.SelectedCategory = fc["category"].ToString();

                List<SelectListItem> allcat = (List<SelectListItem>)TempData["category"];
                foreach (var i in allcat)
                {
                    if (i.Value.ToUpper() == ViewBag.SelectedCategory.ToUpper())
                    {
                        i.Selected = true;
                        break;
                    }
                }
                ViewBag.cat = allcat;
                TempData["category"] = allcat;

            }
            if (fc["schl"] != null && fc["schl"].ToString().Trim() != string.Empty)
            {
                search += " and R.SCHL ='" + fc["schl"].ToString().Trim() + "'";
                ViewBag.SCHL = fc["schl"].ToString();
            }

            rm.StoreAllData = ds = objDB.RegistrationReportSearch(search, ViewBag.SelectedSession, SelUserType);

            ViewBag.data = ds;
            ViewBag.TotalCount1 = 0;
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = ds.Tables[0].Rows.Count;
                }

                if (ds.Tables.Count > 1)
                {
                    if (ds.Tables[1].Rows.Count == 0)
                    {

                        ViewBag.TotalCount1 = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount1 = 1;
                    }
                }
            }
            return View(rm);
        }

        #endregion RegistrationReportAllSession

        #region SchoolPremisesInformationReport
        public ActionResult SchoolPremisesInformationReport()
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            ReportModel rp = new ReportModel();
            rp.StoreAllData = objDB.SchoolPremisesInformationReport();
            ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
            return View(rp);
        }
        #endregion SchoolPremisesInformationReport


        #region AffiliationContinuationReport
        public ActionResult AffiliationContinuationReport()
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            ReportModel rp = new ReportModel();
            rp.StoreAllData = new AbstractLayer.AffiliationDB().AffiliationContinuationReport();
            ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
            return View(rp);
        }

        public ActionResult DownloadAffiliationContinuationData(string id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("AffiliationContinuationReport", "Report");
                }
                else
                {

                    string FileExport = id;
                    DataSet ds = null;

                    if (Session["UserName"] != null)
                    {
                        string AdminType = Session["AdminType"].ToString();
                        int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                        string fileName1 = string.Empty;
                        int OutStatus = 0;
                        string Search = "SCHL like '%%'";
                        if (id.ToLower() == "complete" || id.ToLower() == "")
                        {
                            ds = new AbstractLayer.AffiliationDB().ViewAffiliationContinuation(3, Search, "", 1, out OutStatus, AdminId);
                        }
                        else if (id.ToLower() == "pending")
                        {
                            Search += " and IsFinalSubmit!='FINAL' ";
                            ds = new AbstractLayer.AffiliationDB().ViewAffiliationContinuation(3, Search, "", 1, out OutStatus, AdminId);
                        }
                        else
                        {
                            Search = " USERTYPE='" + id + "'   and IsFinalSubmit!='FINAL' ";
                            ds = new AbstractLayer.AffiliationDB().ViewAffiliationContinuation(4, Search, "", 1, out OutStatus, AdminId);
                        }
                        if (ds == null)
                        {
                            return RedirectToAction("AffiliationContinuationReport", "Report");
                        }
                        else
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                bool ResultDownload;
                                try
                                {
                                    fileName1 = id.ToString().ToUpper() + "_DownloadAffiliationContinuationData";
                                    if (ds.Tables[0] != null)
                                    {
                                        ExportDataFromDataTable(ds.Tables[0], fileName1);
                                    }
                                    ResultDownload = true;
                                }
                                catch (Exception)
                                {
                                    ResultDownload = false;
                                }

                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("AffiliationContinuationReport", "Report");
                    }
                }
                return RedirectToAction("AffiliationContinuationReport", "Report");
            }
            catch (Exception ex)
            {
                return RedirectToAction("AffiliationContinuationReport", "Report");
            }
        }


        #endregion AffiliationContinuationReport


        #region School List Report     
        public ActionResult SchoolListReport(SchoolModels asm)
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
                    AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

                    var itemsch = new SelectList(new[]{new {ID="1",Name="School Code"},new{ID="2",Name="School Name"},
                     new{ID="3",Name="School IDNO"},new{ID="4",Name="School Station"},new{ID="5",Name="School Center Code"},new{ID="6",Name="Tehsil"},}, "ID", "Name", 1);
                    ViewBag.MySch = itemsch.ToList();

                    ViewBag.MySchoolType = objCommon.GetSchool();
                    ViewBag.MyClassType = objCommon.GetClass().Where(s => s.Value != "1").ToList();


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
                        ViewBag.TotalCount = 0;

                        //if (TempData["SearchRegSchoolReport"] != null)
                        //{
                        //    Search += TempData["SearchRegSchoolRecords"].ToString();
                        //    TempData["SelectedItem"] = ViewBag.SelectedItem = TempData["SelectedItem"];
                        //    TempData["SelectedClassType"] = ViewBag.SelectedClassType = TempData["SelectedClassType"];
                        //    TempData["SelectedSchoolType"] = ViewBag.SelectedSchoolType = TempData["SelectedSchoolType"];
                        //    TempData["SelectedDist"] = ViewBag.SelectedDist = TempData["SelectedDist"];

                        //    ASM.StoreAllData = new AbstractLayer.SchoolDB().RegSchoolReport(Search);//RegSchoolListSP
                        //    if (ASM.StoreAllData != null)
                        //    {
                        //        ViewBag.TotalCount = ASM.StoreAllData.Tables[0].Rows.Count;
                        //    }
                        //    else
                        //    {
                        //        ViewBag.TotalCount = 0;
                        //    }
                        //}
                        //else
                        //{
                        //    Search = "sm.Id like '%' and sm.status='Done' and sm.Class!='1' ";
                        //    if (DistAllow != "")
                        //    {
                        //        Search += " and sm.DIST in (" + DistAllow + ")";
                        //    }
                        //    TempData["SearchRegSchoolReport"] = Search;                          
                        //    ASM.StoreAllData = new AbstractLayer.SchoolDB().RegSchoolReport(Search);//RegSchoolListSP
                        //    if (ASM.StoreAllData != null)
                        //    {
                        //        ViewBag.TotalCount = ASM.StoreAllData.Tables[0].Rows.Count;
                        //    }
                        //    else
                        //    {
                        //        ViewBag.TotalCount = 0;
                        //    }

                        //}
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
        public ActionResult SchoolListReport(FormCollection frm, int? page)
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

                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                SchoolModels ASM = new SchoolModels();
                if (ModelState.IsValid)
                {
                    ViewBag.MySchoolType = objCommon.GetSchool();
                    ViewBag.MyClassType = objCommon.GetClass().Where(s => s.Value != "1").ToList();
                    // bind Dist 
                    ////  ViewBag.MyDist = objCommon.GetDistE();
                    var itemsch = new SelectList(new[]{new {ID="1",Name="School Code"},new{ID="2",Name="School Name"},
                     new{ID="3",Name="School IDNO"},new{ID="4",Name="School Station"},new{ID="5",Name="School Center Code"},new{ID="6",Name="Tehsil"},}, "ID", "Name", 1);
                    ViewBag.MySch = itemsch.ToList();
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
                            {
                                if (frm["SearchString"].ToString().ToLower().Trim() == "blank")
                                {
                                    Search += " and (sm.STATIONE is null or sm.STATIONE='' ) ";
                                }
                                else
                                {
                                    Search += " and sm.STATIONE like '%" + frm["SearchString"].ToString() + "%'";
                                }
                            }

                            else if (SelValueSch == 5)
                            { Search += " and sm.SCHLE=" + frm["SearchString"].ToString(); }
                            else if (SelValueSch == 6)
                            {
                                if (frm["SearchString"].ToString().ToLower().Trim() == "blank")
                                { Search += " and (sm.tcode is null or sm.tcode='' ) "; }
                                else
                                { Search += " and sm.tcode=" + frm["SearchString"].ToString(); }
                            }
                        }

                    }
                    if (DistAllow != "")
                    {
                        Search += " and sm.DIST in (" + DistAllow + ")";
                    }
                    TempData["SearchRegSchoolReport"] = Search;
                    TempData.Keep(); // to store search value for view
                    ASM.StoreAllData = new AbstractLayer.SchoolDB().RegSchoolReport(Search);//RegSchoolListSP
                    if (ASM.StoreAllData != null)
                    {
                        ViewBag.data = ASM.StoreAllData;
                        ViewBag.TotalCount = ASM.StoreAllData.Tables[0].Rows.Count;
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
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

        #region Migration Count contoller
        public ActionResult MigrationCount(ReportModel rm)
        {
            try
            {
                FormCollection frm = new FormCollection();
                rm = new ReportModel();
                if (Session["AdminId"] == null)
                { return RedirectToAction("Index", "Admin"); }
                else
                {
                    AbstractLayer.ReportDB objDB = new AbstractLayer.ReportDB();
                    string search = "";
                    string outError = "";
                    rm.StoreAllData = objDB.MigrationCountReport(search, out outError);
                    if (rm.StoreAllData.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                    }
                    return View(rm);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        #endregion Migration Count contoller

        #region SchoolStaffSummaryReport
        public ActionResult SchoolStaffSummaryReport()
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            ReportModel rp = new ReportModel();
            rp.StoreAllData = objDB.SchoolStaffSummaryReport();
            ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
            return View(rp);
        }
        #endregion SchoolStaffSummaryReport

        #region StoppedRollSummaryReport
        public ActionResult StoppedRollSummaryReport()
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            ReportModel rp = new ReportModel();
            rp.StoreAllData = objDB.StoppedRollSummaryReport();
            if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.TotalCount = 0;
            }
            else
            {
                ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
            }
            return View(rp);
        }
        #endregion StoppedRollSummaryReport



        #region PracticalExamPendingSchoolReport
        public ActionResult PracticalExamPendingSchoolReport()
        {
            if (Session["UserName"] != null)
            {
                var itemsch = new SelectList(new[] { new { ID = "R", Name = "Regular" }, new { ID = "O", Name = "Open" }, new { ID = "P", Name = "Pvt" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "12", Name = "Senior" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult PracticalExamPendingSchoolReport(ReportModel RM, FormCollection frm, string Category, string submit) // HttpPostedFileBase file
        {
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                var itemsch = new SelectList(new[] { new { ID = "R", Name = "Regular" }, new { ID = "O", Name = "Open" }, new { ID = "P", Name = "Pvt" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "12", Name = "Senior" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";


                if (submit != null)
                {

                    string firm = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
                    string id = frm["Filevalue"].ToString();
                    Category = id;
                    if (Session["UserName"] != null)
                    {
                        string AdminType = Session["AdminType"].ToString();
                        int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                        string Search = string.Empty;
                        if (frm["SelList"] != "" && frm["SelClass"] != "")
                        {
                            ViewBag.SelectedItem = frm["SelList"];
                            ViewBag.Selectedcls = frm["SelClass"];
                            //TempData["SearchDuplicateCertificate"] = Search;
                            RM.StoreAllData = objDB.PracticalExamPendingSchoolReport(frm["SelList"], Convert.ToInt32(frm["SelClass"]));
                            if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                            {

                                ViewBag.Message = "Record Not Found";
                                ViewBag.TotalCount = 0;
                                return View();
                            }
                            else
                            {
                                Session["PracticalExamPendingSchoolReport"] = RM.StoreAllData.Tables[0];
                                ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                                if (submit.ToUpper().Contains("DOWNLOAD") || submit.ToUpper().Contains("EXCEL"))
                                {
                                    string Type = "";
                                    string Cls1 = "";
                                    if (ViewBag.SelectedItem == "R") { Type = "REG"; }
                                    else if (ViewBag.SelectedItem == "O") { Type = "OPEN"; }
                                    else { Type = "PVT"; }

                                    if (ViewBag.Selectedcls == "12") { Cls1 = "SENIOR"; }
                                    else { Cls1 = "MATRIC"; }

                                    string fileName1 = Type.ToUpper() + Cls1.ToUpper() + "_PracPendingSchoolReport";
                                    ExportDataFromDataTable(RM.StoreAllData.Tables[0], fileName1);
                                }
                                return View(RM);
                            }
                        }
                        else
                        {
                            ViewBag.Message = "2";
                            ViewBag.TotalCount = 0;
                        }
                    }
                    else { return RedirectToAction("Index", "Admin"); }

                }

                return View(RM);

            }
            catch (Exception ex)
            {
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }

        }

        #endregion Practical Exam Pending School Report


        #region Practical Pending Candidates Report
        public ActionResult PracticalPendingCandidatesReport()
        {
            if (Session["UserName"] != null)
            {
                var itemsch = new SelectList(new[] { new { ID = "All", Name = "All" }, new { ID = "R", Name = "Regular" }, new { ID = "O", Name = "Open" }, new { ID = "P", Name = "Pvt" }, new { ID = "T", Name = "ElectiveTheory" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "12", Name = "Senior" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult PracticalPendingCandidatesReport(ReportModel RM, FormCollection frm, string SelList, string Category, string submit) // HttpPostedFileBase file
        {
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                var itemsch = new SelectList(new[] { new { ID = "All", Name = "All" }, new { ID = "R", Name = "Regular" }, new { ID = "O", Name = "Open" }, new { ID = "P", Name = "Pvt" }, new { ID = "T", Name = "ElectiveTheory" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "12", Name = "Senior" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";


                if (submit != null)
                {

                    string firm = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
                    string id = frm["Filevalue"].ToString();
                    Category = id;
                    if (Session["UserName"] != null)
                    {
                        string AdminType = Session["AdminType"].ToString();
                        int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                        string Search = string.Empty;

                        if (!string.IsNullOrEmpty(SelList))
                        {
                            ViewBag.SelectedItem = SelList;
                            string listname = itemsch.ToList().Where(s => s.Value.ToString().ToUpper() == SelList.ToString().ToUpper()).Select(s => s.Text).FirstOrDefault();
                            int cls = 0;
                            RM.StoreAllData = objDB.PracticalPendingCandidatesReport("", SelList, cls);
                            if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                            {
                                ViewBag.Message = "Record Not Found";
                                ViewBag.TotalCount = 0;
                                return View();
                            }
                            else
                            {
                                ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                                string fileName1 = listname.ToUpper() + "_Practical_PendingCandidates";
                                ExportDataFromDataTable(RM.StoreAllData.Tables[0], fileName1);
                                return View(RM);
                            }
                        }
                        else
                        {
                            ViewBag.Message = "2";
                            ViewBag.TotalCount = 0;
                        }
                    }
                    else { return RedirectToAction("Index", "Admin"); }

                }

                return View(RM);

            }
            catch (Exception ex)
            {
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }

        }

        #endregion

        #region RecheckSummaryReport

        public ActionResult RecheckSummaryReport()
        {
            if (Session["UserName"] != null)
            {
                var itemcls = new SelectList(new[] { new { ID = "12", Name = "Senior" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                var batch = new SelectList(new[] { new { ID = "March", Name = "March" }, new { ID = "July", Name = "July" }, new { ID = "September", Name = "September" }, new { ID = "November", Name = "November" }, }, "ID", "Name", 1);
                ViewBag.Mybatch = batch.ToList();
                ViewBag.Month = "0";
                ViewBag.Year = "2019";

                var itemRP = new SelectList(new[] { new { ID = "1", Name = "Regular" }, new { ID = "2", Name = "Open" }, new { ID = "3", Name = "Pvt" }, }, "ID", "Name", 1);
                ViewBag.MyRP = itemRP.ToList();
                ViewBag.SelectedRP = "0";

                ViewBag.TotalCount = 0;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult RecheckSummaryReport(ReportModel RM, FormCollection frm)
        {
            if (Session["AdminId"] == null || Session["AdminId"].ToString().Trim() == string.Empty)
            {
                return RedirectToAction("Index", "Admin");
            }

            var itemcls = new SelectList(new[] { new { ID = "12", Name = "Senior" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
            ViewBag.Mycls = itemcls.ToList();
            ViewBag.Selectedcls = "0";

            var batch = new SelectList(new[] { new { ID = "March", Name = "March" }, new { ID = "July", Name = "July" }, new { ID = "September", Name = "September" }, new { ID = "November", Name = "November" }, }, "ID", "Name", 1);
            ViewBag.Mybatch = batch.ToList();
            ViewBag.Month = "0";
            ViewBag.Year = "2019";

            var itemRP = new SelectList(new[] { new { ID = "1", Name = "Regular" }, new { ID = "2", Name = "Open" }, new { ID = "3", Name = "Pvt" }, }, "ID", "Name", 1);
            ViewBag.MyRP = itemRP.ToList();
            ViewBag.SelectedRP = "0";

            if (frm["Class"] != "" && frm["batch"] != "" && frm["batchYear"] != "")
            {
                string cls = string.Empty;
                ViewBag.Selectedcls = cls = frm["Class"];
                ViewBag.Month = frm["batch"];
                ViewBag.Year = frm["batchYear"];
                string Rp = ViewBag.SelectedRP = frm["RP"];

                string adminId = Session["AdminId"].ToString().Trim();
                RM.StoreAllData = objDB.RecheckSummaryReport(cls, ViewBag.Month, ViewBag.Year, Rp);
                if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.Total = RM.StoreAllData.Tables[0].AsEnumerable().Sum(x => x.Field<int>("Total"));
                    ViewBag.Rechecking = RM.StoreAllData.Tables[0].AsEnumerable().Sum(x => x.Field<int>("Rechecking"));
                    ViewBag.RCFS = RM.StoreAllData.Tables[0].AsEnumerable().Sum(x => x.Field<int>("RCFS"));
                    ViewBag.RCFP = RM.StoreAllData.Tables[0].AsEnumerable().Sum(x => x.Field<int>("RCFP"));
                    ViewBag.Revaluation = RM.StoreAllData.Tables[0].AsEnumerable().Sum(x => x.Field<int>("Revaluation"));
                    ViewBag.REFS = RM.StoreAllData.Tables[0].AsEnumerable().Sum(x => x.Field<int>("REFS"));
                    ViewBag.REFP = RM.StoreAllData.Tables[0].AsEnumerable().Sum(x => x.Field<int>("REFP"));


                }
            }
            else
            {
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
            }

            return View(RM);

        }
        #endregion ExamReport



        #region EAffiliationReport
        public ActionResult EAffiliationReport()
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction("Index", "Admin");
            }
            ReportModel rp = new ReportModel();
            rp.StoreAllData = objDB.EAffiliationReport();
            if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.TotalCount = 0;
            }
            else
            {
                ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
            }
            return View(rp);
        }
        #endregion StoppedRollSummaryReport


        // Class and Subject wise Count of Candidates of Matric & Senior Secondary class, Session 2020-21
        #region ClassSubjectWiseExaminationReport
        public ActionResult ClassSubjectWiseExaminationReport()
        {
            if (Session["UserName"] != null)
            {
                var itemsch = new SelectList(new[] { new { ID = "A", Name = "Regular and Open" }, new { ID = "R", Name = "Regular" }, new { ID = "O", Name = "Open" },
                        new { ID = "PROA", Name = "Private [Regular/Open/Add ]" },new { ID = "DRO", Name = "Differentlyabled [Regular/Open]" },
                    new { ID = "DAR", Name = "Differentlyabled [Regular]" },new { ID = "DAO", Name = "Differentlyabled [Open]" },
                    new { ID = "DPROA", Name = "Differentlyabled Private [Regular/Open/Add]" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "12", Name = "Senior" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult ClassSubjectWiseExaminationReport(ReportModel RM, FormCollection frm, string Category, string submit) // HttpPostedFileBase file
        {
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                var itemsch = new SelectList(new[] { new { ID = "A", Name = "Regular and Open" }, new { ID = "R", Name = "Regular" }, new { ID = "O", Name = "Open" },
                        new { ID = "PROA", Name = "Private [Regular/Open/Add ]" },new { ID = "DRO", Name = "Differentlyabled [Regular/Open]" },
                    new { ID = "DAR", Name = "Differentlyabled [Regular]" },new { ID = "DAO", Name = "Differentlyabled [Open]" },
                    new { ID = "DPROA", Name = "Differentlyabled Private [Regular/Open/Add]" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "12", Name = "Senior" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";


                if (submit != null)
                {
                    if (submit.ToUpper() == "DOWNLOAD")
                    {
                        if (Session["ClassSubjectWiseExaminationReport"] != null)
                        {
                            DataTable dt = new DataTable();
                            dt = (DataTable)Session["ClassSubjectWiseExaminationReport"];
                            ViewBag.SelectedItem = frm["SelList"];
                            ViewBag.Selectedcls = frm["SelClass"];

                            string Type = "";
                            string Cls1 = "";
                            if (ViewBag.SelectedItem == "R") { Type = "REG"; }
                            else if (ViewBag.SelectedItem == "O") { Type = "OPEN"; }
                            else { Type = "REG-OPEN"; }

                            if (ViewBag.Selectedcls == "12") { Cls1 = "SENIOR"; }
                            else { Cls1 = "MATRIC"; }

                            string fileName1 = Type.ToUpper() + Cls1.ToUpper() + "_ClassSubjectWiseExaminationReport" + '_' + DateTime.Now.ToString("ddMMyyyy");
                            ExportDataFromDataTable(dt, fileName1);
                        }
                    }
                    else
                    {
                        string firm = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
                        string id = frm["Filevalue"].ToString();
                        Category = id;
                        if (Session["UserName"] != null)
                        {
                            string AdminType = Session["AdminType"].ToString();
                            int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                            string Search = string.Empty;
                            if (frm["SelList"] != "" && frm["SelClass"] != "")
                            {
                                ViewBag.SelectedItem = frm["SelList"];
                                ViewBag.Selectedcls = frm["SelClass"];
                                RM.StoreAllData = objDB.ClassSubjectWiseExaminationReport(0, frm["SelList"], "", Convert.ToInt32(frm["SelClass"]));
                                if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                                {

                                    ViewBag.Message = "Record Not Found";
                                    ViewBag.TotalCount = 0;
                                    return View();
                                }
                                else
                                {
                                    Session["ClassSubjectWiseExaminationReport"] = RM.StoreAllData.Tables[0];
                                    ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                                    return View(RM);
                                }
                            }
                            else
                            {
                                ViewBag.Message = "2";
                                ViewBag.TotalCount = 0;
                            }
                        }
                        else { return RedirectToAction("Index", "Admin"); }
                    }
                }

                return View(RM);

            }
            catch (Exception ex)
            {
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }

        }

        #endregion ClassSubjectWiseExaminationReport



        #region DistClassSubjectWiseExaminationReport
        public ActionResult DistClassSubjectWiseExaminationReport()
        {
            if (Session["UserName"] != null)
            {
                //var itemsch = new SelectList(new[] { new { ID = "A", Name = "Regular and Open" }, new { ID = "R", Name = "Regular" }, new { ID = "O", Name = "Open" }, }, "ID", "Name", 1);

                var itemsch = new SelectList(new[] { new { ID = "A", Name = "Regular and Open" }, new { ID = "R", Name = "Regular" }, new { ID = "O", Name = "Open" },
                        new { ID = "PROA", Name = "Private [Regular/Open/Add ]" },new { ID = "DRO", Name = "Differentlyabled [Regular/Open]" },
                    new { ID = "DAR", Name = "Differentlyabled [Regular]" },new { ID = "DAO", Name = "Differentlyabled [Open]" },
                    new { ID = "DPROA", Name = "Differentlyabled Private [Regular/Open/Add]" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "12", Name = "Senior" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                // English Dist 
                ViewBag.DistEList = objCommon.GetDistE();
                ViewBag.SelectedDIST = "0";

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult DistClassSubjectWiseExaminationReport(ReportModel RM, FormCollection frm, string Category, string submit) // HttpPostedFileBase file
        {
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }
                // var itemsch = new SelectList(new[] { new { ID = "A", Name = "Regular and Open" }, new { ID = "R", Name = "Regular" }, new { ID = "O", Name = "Open" }, }, "ID", "Name", 1);
                var itemsch = new SelectList(new[] { new { ID = "A", Name = "Regular and Open" }, new { ID = "R", Name = "Regular" }, new { ID = "O", Name = "Open" },
                        new { ID = "PROA", Name = "Private [Regular/Open/Add ]" },new { ID = "DRO", Name = "Differentlyabled [Regular/Open]" },
                    new { ID = "DAR", Name = "Differentlyabled [Regular]" },new { ID = "DAO", Name = "Differentlyabled [Open]" },
                    new { ID = "DPROA", Name = "Differentlyabled Private [Regular/Open/Add]" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "12", Name = "Senior" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                // English Dist 
                ViewBag.DistEList = objCommon.GetDistE();
                ViewBag.SelectedDIST = "0";

                if (submit != null)
                {
                    if (submit.ToUpper() == "DOWNLOAD")
                    {
                        if (Session["DISTClassSubjectWiseExaminationReport"] != null)
                        {
                            DataTable dt = new DataTable();
                            dt = (DataTable)Session["DISTClassSubjectWiseExaminationReport"];
                            ViewBag.SelectedItem = frm["SelList"];
                            ViewBag.Selectedcls = frm["SelClass"];

                            string Type = "";
                            string Cls1 = "";
                            if (ViewBag.SelectedItem == "R") { Type = "REG"; }
                            else if (ViewBag.SelectedItem == "O") { Type = "OPEN"; }
                            else { Type = "REG-OPEN"; }

                            if (ViewBag.Selectedcls == "12") { Cls1 = "SENIOR"; }
                            else { Cls1 = "MATRIC"; }

                            string fileName1 = Type.ToUpper() + Cls1.ToUpper() + "_DISTClassSubjectWiseExaminationReport" + '_' + DateTime.Now.ToString("ddMMyyyy");
                            ExportDataFromDataTable(dt, fileName1);
                        }
                    }
                    else
                    {
                        string firm = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
                        string id = frm["Filevalue"].ToString();
                        Category = id;
                        if (Session["UserName"] != null)
                        {
                            string AdminType = Session["AdminType"].ToString();
                            int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                            string Search = string.Empty;
                            if (frm["SelList"] != "" && frm["SelClass"] != "")
                            {
                                ViewBag.SelectedItem = frm["SelList"];
                                ViewBag.Selectedcls = frm["SelClass"];
                                ViewBag.SelectedDIST = frm["SelDIST"];
                                ViewBag.DIST = ViewBag.DISTNM = "";
                                RM.StoreAllData = objDB.ClassSubjectWiseExaminationReport(1, frm["SelList"], ViewBag.SelectedDIST, Convert.ToInt32(frm["SelClass"]));
                                if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                                {

                                    ViewBag.Message = "Record Not Found";
                                    ViewBag.TotalCount = 0;
                                    return View();
                                }
                                else
                                {
                                    Session["DISTClassSubjectWiseExaminationReport"] = RM.StoreAllData.Tables[0];
                                    ViewBag.DIST = RM.StoreAllData.Tables[0].Rows[0]["DIST"].ToString();
                                    ViewBag.DISTNM = RM.StoreAllData.Tables[0].Rows[0]["DISTNM"].ToString();
                                    ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                                    return View(RM);
                                }
                            }
                            else
                            {
                                ViewBag.Message = "2";
                                ViewBag.TotalCount = 0;
                            }
                        }
                        else { return RedirectToAction("Index", "Admin"); }
                    }
                }

                return View(RM);

            }
            catch (Exception ex)
            {
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }

        }

        #endregion DISTClassSubjectWiseExaminationReport



        #region EAffiliationApplicationsReceivedReport
        public ActionResult EAffiliationApplicationsReceivedReport()
        {
            if (Session["UserName"] != null)
            {
                var itemcls = new SelectList(new[] { new { ID = "5", Name = "5th" }, new { ID = "8", Name = "8th" }
                , new { ID = "10", Name = "10th" }, new { ID = "12", Name = "12th" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                // English Dist 
                ViewBag.DistEList = objCommon.GetDistE();
                ViewBag.SelectedDIST = "0";

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        [HttpPost]
        public ActionResult EAffiliationApplicationsReceivedReport(ReportModel RM, FormCollection frm, string Category, string submit) // HttpPostedFileBase file
        {
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Index", "Admin");
                }

                var itemcls = new SelectList(new[] { new { ID = "5", Name = "5th" }, new { ID = "8", Name = "8th" }
                , new { ID = "10", Name = "10th" }, new { ID = "12", Name = "12th" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                // English Dist 
                ViewBag.DistEList = objCommon.GetDistE();
                ViewBag.SelectedDIST = "0";

                if (submit != null)
                {

                    string firm = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
                    string id = frm["Filevalue"].ToString();
                    Category = id;
                    if (Session["UserName"] != null)
                    {
                        string AdminType = Session["AdminType"].ToString();
                        int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                        string Search = "APPNO like '%%'";
                        if (frm["SelList"] != "" && frm["SelClass"] != "")
                        {
                            ViewBag.SelectedItem = frm["SelList"];
                            ViewBag.Selectedcls = frm["SelClass"];
                            ViewBag.SelectedDIST = frm["SelDIST"];
                            ViewBag.DIST = ViewBag.DISTNM = "";
                            //EAffiliationApplicationsReceivedReport(int type,string DIST, int cls, string Search)

                            if (frm["SelDIST"] != "")
                            {
                                ViewBag.SelectedDIST = frm["SelDIST"];
                                TempData["SelDIST"] = frm["SelDIST"];
                                Search += " and DIST ='" + ViewBag.SelectedDIST.ToString() + "'";

                            }

                            if (frm["SelClass"] != "")
                            {
                                ViewBag.Selectedcls = frm["SelClass"];
                                TempData["SelClass"] = frm["SelClass"];
                                Search += " and Class ='" + ViewBag.Selectedcls.ToString() + "'";
                            }

                            if (frm["FromDate"] != "")
                            {
                                ViewBag.FromDate = frm["FromDate"];
                                TempData["FromDate"] = frm["FromDate"];
                                Search += " and  CONVERT(DATETIME, DATEDIFF(DAY, 0, CREATEDDATE)) >=convert(DATETIME,'" + frm["FromDate"].ToString() + "',103) ";

                            }
                            if (frm["ToDate"] != "")
                            {
                                ViewBag.ToDate = frm["ToDate"];
                                TempData["ToDate"] = frm["ToDate"];
                                Search += "   and CONVERT(DATETIME, DATEDIFF(DAY, 0,CREATEDDATE)) <=  convert(DATETIME,'" + frm["ToDate"].ToString() + "',103)";
                            }
                            RM.StoreAllData = objDB.EAffiliationApplicationsReceivedReport(0, ViewBag.SelectedDIST, Convert.ToInt32(ViewBag.Selectedcls), Search);
                            if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                            {

                                ViewBag.Message = "Record Not Found";
                                ViewBag.TotalCount = 0;
                                return View();
                            }
                            else
                            {
                                Session["EAffiliationApplicationsReceivedReport"] = RM.StoreAllData.Tables[0];
                                ViewBag.DIST = RM.StoreAllData.Tables[0].Rows[0]["DIST"].ToString();
                                ViewBag.DISTNM = RM.StoreAllData.Tables[0].Rows[0]["DISTNME"].ToString();
                                ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                                return View(RM);
                            }
                        }
                        else
                        {
                            ViewBag.Message = "2";
                            ViewBag.TotalCount = 0;
                        }
                    }
                    else { return RedirectToAction("Index", "Admin"); }

                }
                return View(RM);

            }
            catch (Exception ex)
            {
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }

        }

        #endregion EAffiliationApplicationsReceivedReport


        public ActionResult RegExaminationCountandFeeSummaryReport()
        {
            return View();
        }

        public ActionResult ClusterRegisterReport(ReportModel rm)
        {

            if (Session["AdminId"] != null)
            {
                rm.DistList = AbstractLayer.AdminDB.getAdminDistAllowList("admin", Convert.ToString(Session["AdminId"]));
            }
            else if (Session["USER"] != null)
            {
                rm.DistList = AbstractLayer.AdminDB.getAdminDistAllowList("deo", Convert.ToString(Session["USER"]));
            }
            else
            {
                return View();
            }





            var itemsch1 = new SelectList(new[] { new { ID = "1", Name = "School Code" },
                new { ID = "2", Name = "Cluster Code" }, new { ID = "3", Name = "UDISE Code" }, }, "ID", "Name", 1);
            ViewBag.MySearch = itemsch1.ToList();
            ViewBag.SelectedSearch = "0";

            try
            {
                ViewBag.TotalCount = 0;
                return View(rm);

            }
            catch (Exception ex)
            {

                return View();
            }
        }

        [HttpPost]
        public ActionResult ClusterRegisterReport(ReportModel rm, FormCollection frm, string SearchList, string SearchString)
        {
            var itemsch1 = new SelectList(new[] { new { ID = "1", Name = "School Code" },
                new { ID = "2", Name = "Cluster Code" }, new { ID = "3", Name = "UDISE Code" }, }, "ID", "Name", 1);
            ViewBag.MySearch = itemsch1.ToList();
            ViewBag.SelectedSearch = "0";

            if (Session["AdminId"] != null)
            {
                rm.DistList = AbstractLayer.AdminDB.getAdminDistAllowList("admin", Convert.ToString(Session["AdminId"]));
            }
            else if (Session["USER"] != null)
            {
                rm.DistList = AbstractLayer.AdminDB.getAdminDistAllowList("deo", Convert.ToString(Session["USER"]));
            }
            try
            {
                string outError = "";

                string Search = string.Empty;
                Search = "ccode like '%' ";

                if (!string.IsNullOrEmpty(rm.Dist))
                {
                    Search += " and dist ='" + rm.Dist.ToString() + "' ";
                }

                if (!string.IsNullOrEmpty(SearchList))
                {
                    ViewBag.SelectedSearch = SearchList;
                    if (!string.IsNullOrEmpty(SearchString))
                    {
                        if (SearchList == "1")
                        { Search += " and schl ='" + SearchString.ToString() + "'"; }
                        else if (SearchList == "2")
                        { Search += " and ccode ='" + SearchString.ToString() + "'"; }
                        else if (SearchList == "3")
                        { Search += " and chtudise ='" + SearchString.ToString() + "'"; }

                        ViewBag.SearchString = SearchString;
                        TempData["SearchString"] = SearchString;
                    }
                }

                rm.StoreAllData = new AbstractLayer.ReportDB().ClusterRegisterReport(Search, out outError);
                if (rm.StoreAllData.Tables[0].Rows.Count > 0)
                {
                    ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                }
                else
                {
                    ViewBag.TotalCount = 0;
                }
                return View(rm);

            }
            catch (Exception ex)
            {

                return View();
            }
        }



        #region OpenSchoolAccreditationReport

        public ActionResult OpenSchoolAccreditationReport()
        {
            string districts = string.Empty;
            // Dist Allowed
            string DistAllow = "";
            if (ViewBag.DistAllow != null)
            {
                DistAllow = ViewBag.DistAllow;
            }

            List<SelectListItem> OpenDistricts = new AbstractLayer.HomeDB().OpenSchoolDistricts();
            if (ViewBag.DistUser == null || ViewBag.DistAllow == null)
            {
                ViewBag.Districts = new AbstractLayer.DBClass().GetDistE();
            }
            else
            {
                ViewBag.Districts = ViewBag.DistUser;
            }

            List<SelectListItem> dist = new List<SelectListItem>();
            List<SelectListItem> dist1 = new List<SelectListItem>();
            dist1 = (List<SelectListItem>)ViewBag.Districts;
            foreach (SelectListItem sel in dist1)
            {
                if (OpenDistricts.Find(f => f.Value == sel.Value) != null)
                {
                    dist.Add(sel);
                }
            }

            ViewBag.Districts = dist;
            return View();
        }

        [HttpPost]
        public ActionResult OpenSchoolAccreditationReport(FormCollection fc, string cmd)
        {
            string DistAllow = "";
            if (ViewBag.DistAllow != null)
            {
                DistAllow = ViewBag.DistAllow;
            }
            if (ViewBag.DistUser == null || ViewBag.DistAllow == null)
            {
                ViewBag.Districts = new AbstractLayer.DBClass().GetDistE();
            }
            else
            {
                ViewBag.Districts = ViewBag.DistUser;
            }
            ViewBag.SelectedDist = fc["ddlDist"] != null ? fc["ddlDist"].ToString() : string.Empty;

            string distcode = "";

            if (fc["ddlDist"] != null)
            {
                distcode = fc["ddlDist"].ToString();
            }
            var obj = new AbstractLayer.ReportDB().OpenSchoolAccreditationReport(distcode);
            ViewBag.data = obj;
            TempData["ExportToExcelDataFromDataTable"] = obj.Tables[0];
            if (!string.IsNullOrEmpty(cmd))
            {
                if (cmd.ToLower().Contains("excel") || cmd.ToLower().Contains("download"))
                {
                    TempData["ExportToExcel"] = "1";
                    string filename = "OpenSchoolAccreditationReport";
                    ExportToExcelDataFromDataTable(filename, "SubjectWiseReport");
                }
            }

            return View();
        }

        #endregion OpenSchoolAccreditationReport


        [Route("NonGovtSchoolListReport")]
        #region NonGovtSchoolListReport 
        public ActionResult NonGovtSchoolListReport(SchoolModels asm)
        {
            try
            {
                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

                // Dist Allowed
                string DistAllow = "";
                if (ViewBag.DistAllow != null)
                {
                    DistAllow = ViewBag.DistAllow;
                }

                ViewBag.MyDist = new AbstractLayer.DBClass().GetDistE();

                var itemsch = new SelectList(new[]{new {ID="1",Name="School Code"},new{ID="2",Name="School Name"},
                     new{ID="3",Name="School IDNO"},new{ID="4",Name="School Station"},new{ID="5",Name="School Center Code"},new{ID="6",Name="Tehsil"},}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();

                ViewBag.MySchoolType = objCommon.GetNonGovtSchoolTypeList();
                ViewBag.MyClassType = objCommon.GetClass().ToList();


                ViewBag.SelectedDist = "";
                ViewBag.SelectedItem = "";
                ViewBag.SelectedSchoolType = "";
                ViewBag.SelectedClassType = "";

                string Search = string.Empty;
                ViewBag.TotalCount = 0;
                return View(asm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [Route("NonGovtSchoolListReport")]
        [HttpPost]
        public ActionResult NonGovtSchoolListReport(SchoolModels asm, FormCollection frm, int? page)
        {
            try
            {
                AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();

                // Dist Allowed
                string DistAllow = "";
                if (ViewBag.DistAllow != null)
                {
                    DistAllow = ViewBag.DistAllow;
                }

                ViewBag.MyDist = new AbstractLayer.DBClass().GetDistE();

                var itemsch = new SelectList(new[]{new {ID="1",Name="School Code"},new{ID="2",Name="School Name"},
                     new{ID="3",Name="School IDNO"},new{ID="4",Name="School Station"},new{ID="5",Name="School Center Code"},new{ID="6",Name="Tehsil"},}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();

                ViewBag.MySchoolType = objCommon.GetNonGovtSchoolTypeList();
                ViewBag.MyClassType = objCommon.GetClass().ToList();



                if (ModelState.IsValid)
                {

                    string Search = string.Empty;
                    Search = "sm.Id like '%' and sm.status='Done' ";
                    if (frm["Dist1"] != "")
                    {
                        ViewBag.SelectedDist = frm["Dist1"];
                        TempData["SelectedDist"] = frm["Dist1"];
                        Search += " and sm.dist='" + frm["Dist1"].ToString() + "'";
                    }
                    if (frm["SchoolType"] != "")
                    {
                        ViewBag.SelectedSchoolType = frm["SchoolType"];
                        TempData["SelectedSchoolType"] = frm["SchoolType"];
                        Search += " and st.abbr ='" + frm["SchoolType"].ToString() + "'";
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
                            {
                                if (frm["SearchString"].ToString().ToLower().Trim() == "blank")
                                {
                                    Search += " and (sm.STATIONE is null or sm.STATIONE='' ) ";
                                }
                                else
                                {
                                    Search += " and sm.STATIONE like '%" + frm["SearchString"].ToString() + "%'";
                                }
                            }

                            else if (SelValueSch == 5)
                            { Search += " and sm.SCHLE=" + frm["SearchString"].ToString(); }
                            else if (SelValueSch == 6)
                            {
                                if (frm["SearchString"].ToString().ToLower().Trim() == "blank")
                                { Search += " and (sm.tcode is null or sm.tcode='' ) "; }
                                else
                                { Search += " and sm.tcode=" + frm["SearchString"].ToString(); }
                            }
                        }

                    }
                    if (DistAllow != "")
                    {
                        Search += " and sm.DIST in (" + DistAllow + ")";
                    }
                    TempData["SearchRegSchoolReport"] = Search;
                    TempData.Keep(); // to store search value for view
                    asm.StoreAllData = new AbstractLayer.SchoolDB().RegSchoolReport(Search);//RegSchoolListSP
                    if (asm.StoreAllData != null)
                    {
                        ViewBag.data = asm.StoreAllData;
                        ViewBag.TotalCount = asm.StoreAllData.Tables[0].Rows.Count;
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                    }
                }
                return View(asm);
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        #endregion


        public ActionResult EAffiliationSummaryReport()
        {
            ReportModel rp = new ReportModel();
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }


                string Search = string.Empty;
                // Search = "a.bcode like '%' ";

                string outError = "";
                DataSet ds = objDB.EAffiliationSummaryReport(Search, out outError);  //1 for Total Registration by School Report
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
                // oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));                
                return View();
            }
        }

        public ActionResult SchoolToSchoolMigrationSummary()
        {
            ReportModel rp = new ReportModel();
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }

                DataSet ds = new AbstractLayer.SchoolDB().StudentSchoolMigrationsSearch(4, "", ""); // type=4 for summary report
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
        }


        #region Middle-Primary Report

        [AdminLoginCheckFilter]
        public ActionResult ExaminationAllClassWiseSubjectWiseReport(ReportModel RM)
        {

            try
            {
                var itemsch = new SelectList(new[] {
                new { ID = "1", Name = "Subject Wise" }, new { ID = "2", Name = "District Subject Wise" },
                new { ID = "3", Name = "School Wise" },new { ID = "4", Name = "School Subject Wise" },
                new { ID = "5", Name = "District School Subject Wise" },new { ID = "6", Name = "District School Wise" },
                new { ID = "7", Name = "Subject Wise - Study Medium Wise" },
               // new { ID = "8", Name = "School Wise - Subject Wise - Study Medium Wise" },
                }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "5", Name = "Primary" }, new { ID = "8", Name = "Middle" },
                new { ID = "10", Name = "Matric" }, new { ID = "12", Name = "Senior" },}, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                ViewBag.DistEList = new AbstractLayer.DBClass().GetDistE();
                ViewBag.SelectedDIST = "0";

                return View(RM);
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult ExaminationAllClassWiseSubjectWiseReport(ReportModel RM, FormCollection frm, string submit)
        {


            try
            {
                var itemsch = new SelectList(new[] {
                new { ID = "1", Name = "Subject Wise" }, new { ID = "2", Name = "District Subject Wise" },
                new { ID = "3", Name = "School Wise" },new { ID = "4", Name = "School Subject Wise" },
                new { ID = "5", Name = "District School Subject Wise" },new { ID = "6", Name = "District School Wise" },
                new { ID = "7", Name = "Subject Wise - Study Medium Wise" },
               //new { ID = "8", Name = "School Wise - Subject Wise - Study Medium Wise" },
                }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "5", Name = "Primary" }, new { ID = "8", Name = "Middle" },
                new { ID = "10", Name = "Matric" }, new { ID = "12", Name = "Senior" },}, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                ViewBag.DistEList = new AbstractLayer.DBClass().GetDistE();
                ViewBag.SelectedDIST = "0";

                string Search = string.Empty;
                if (!string.IsNullOrEmpty(frm["SelList"]) && !string.IsNullOrEmpty(frm["SelClass"]))
                {
                    ViewBag.SelectedItem = frm["SelList"] == null ? "ALL" : frm["SelList"];
                    ViewBag.SelectedItemText = frm["SelList"] == null ? "ALL" : itemsch.ToList().Where(s => s.Value == ViewBag.SelectedItem).Select(s => s.Text).FirstOrDefault();
                    ViewBag.Selectedcls = frm["SelClass"] == null ? "ALL" : frm["SelClass"];
                    ViewBag.SelectedDIST = frm["SelDist"] == null ? "ALL" : frm["SelDist"];
                    RM.StoreAllData = AbstractLayer.ReportDB.ExaminationAllClassWiseSubjectWiseReport(ViewBag.SelectedItem, Convert.ToInt32(ViewBag.Selectedcls), ViewBag.SelectedDIST);
                    if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        TempData["ExportToExcelDataFromDataTable"] = RM.StoreAllData.Tables[0];
                        if (submit != null)
                        {
                            if (submit.ToLower().Contains("excel") || submit.ToLower().Contains("download"))
                            {
                                TempData["ExportToExcel"] = "1";
                                ExportToExcelDataFromDataTable(ViewBag.SelectedItemText, "SubjectWiseReport");
                            }
                        }
                        ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                    }
                }
                else
                {
                    ViewBag.Message = "2";
                    ViewBag.TotalCount = 0;
                }
            }
            catch (Exception ex)
            {
                return View();
            }
            return View(RM);
        }

        #endregion

        #region Panel Wise Mark Pending Summary Report
        [AdminLoginCheckFilter]
        [AdminMenuFilter]
        public ActionResult PanelWiseMarkPendingSummaryReport()
        {

            var itemsch = new SelectList(new[] { new { ID = "1", Name = "C.C.E" }, new { ID = "2", Name = "Practical Summary Report" },
                new { ID = "3", Name = "Elective Theory" }, new { ID = "4", Name = " Differently Abled" },
            new { ID = "5", Name = "Pre-Board" }, new { ID = "6", Name = "Practical Marks Entry" },}, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "0";

            var itemcls = new SelectList(new[] { new { ID = "5", Name = "Primary" }, new { ID = "8", Name = "Middle" },
                new { ID = "10", Name = "Matric" }, new { ID = "12", Name = "Senior" },}, "ID", "Name", 1);
            ViewBag.Mycls = itemcls.ToList();
            ViewBag.Selectedcls = "0";
            return View();
        }


        [AdminLoginCheckFilter]
        [AdminMenuFilter]
        [HttpPost]
        public ActionResult PanelWiseMarkPendingSummaryReport(ReportModel RM, FormCollection frm, string Category, string submit) // HttpPostedFileBase file
        {
            try
            {

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "C.C.E" }, new { ID = "2", Name = "Practical Summary Report" },
                new { ID = "3", Name = "Elective Theory" }, new { ID = "4", Name = " Differently Abled" },
            new { ID = "5", Name = "Pre-Board" }, new { ID = "6", Name = "Practical Marks Entry" },}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "5", Name = "Primary" }, new { ID = "8", Name = "Middle" },
                new { ID = "10", Name = "Matric" }, new { ID = "12", Name = "Senior" },}, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";


                string id = frm["Filevalue"].ToString();
                Category = id;

                string Search = string.Empty;
                if (frm["SelList"] != "" && frm["SelClass"] != "")
                {
                    ViewBag.SelectedItem = frm["SelList"];
                    ViewBag.Selectedcls = frm["SelClass"];
                    //TempData["SearchDuplicateCertificate"] = Search;
                    RM.StoreAllData = AbstractLayer.ReportDB.PanelWiseMarkPendingSummaryReport(Convert.ToInt32(frm["SelList"]), Convert.ToInt32(frm["SelClass"]));
                    if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                    {

                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        TempData["ExportDataFromDataTable"] = RM.StoreAllData;
                        if (submit != null)
                        {
                            if (submit.ToUpper().Contains("DOWNLOAD") || submit.ToUpper().Contains("EXCEL"))
                            {
                                if (RM.StoreAllData.Tables[0] != null)
                                {
                                    string Type = itemsch.ToList().Where(s => s.Value == ViewBag.SelectedItem).Select(s => s.Text).SingleOrDefault();
                                    string fileName1 = Type.Replace(" ", "").ToUpper().Trim() + "_SummaryReport";
                                    ExportDataFromDataTable(RM.StoreAllData.Tables[0], fileName1);
                                }
                            }
                        }
                        ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                        return View(RM);
                    }
                }
                else
                {
                    ViewBag.Message = "2";
                    ViewBag.TotalCount = 0;
                    return View();
                }


            }
            catch (Exception ex)
            {

                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }

        }

        #endregion Panel Wise Mark Pending Summary Report

        #region Middle-Primary Report

        [AdminLoginCheckFilter]
        public ActionResult ClassWiseSchoolWiseReport(ReportModel RM)
        {

            try
            {
                var itemsch = new SelectList(new[] {new { ID = "1", Name = "School Wise Regular Student Count" },
                new { ID = "2", Name = "Pending UserType-Dist Wise School List Regular Student Count With SubjectList" },
                    new { ID = "3", Name = "UserType Wise School List Regular Student Count" },
                }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "5", Name = "Primary" }, new { ID = "8", Name = "Middle" },
                new { ID = "10", Name = "Matric" }, new { ID = "12", Name = "Senior" },}, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                //ViewBag.DistEList = new AbstractLayer.DBClass().GetDistE();
                //ViewBag.SelectedDIST = "0";

                return View(RM);
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult ClassWiseSchoolWiseReport(ReportModel RM, FormCollection frm, string submit)
        {


            try
            {
                var itemsch = new SelectList(new[] {new { ID = "1", Name = "School Wise Regular Student Count" },
                new { ID = "2", Name = "Pending UserType-Dist Wise School List Regular Student Count With SubjectList" },
                    new { ID = "3", Name = "UserType Wise School List Regular Student Count" },
                }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "5", Name = "Primary" }, new { ID = "8", Name = "Middle" },
                new { ID = "10", Name = "Matric" }, new { ID = "12", Name = "Senior" },}, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                //ViewBag.DistEList = new AbstractLayer.DBClass().GetDistE();
                //ViewBag.SelectedDIST = "0";

                string Search = string.Empty;
                if (!string.IsNullOrEmpty(frm["SelList"]) && !string.IsNullOrEmpty(frm["SelClass"]))
                {
                    ViewBag.SelectedItem = frm["SelList"] == null ? "ALL" : frm["SelList"];
                    ViewBag.SelectedItemText = frm["SelList"] == null ? "ALL" : itemsch.ToList().Where(s => s.Value == ViewBag.SelectedItem).Select(s => s.Text).FirstOrDefault();
                    ViewBag.Selectedcls = frm["SelClass"] == null ? "ALL" : frm["SelClass"];
                    //ViewBag.SelectedDIST = frm["SelDist"] == null ? "ALL" : frm["SelDist"];
                    RM.StoreAllData = AbstractLayer.ReportDB.ClassWiseSchoolWiseReport(ViewBag.SelectedItem, Convert.ToInt32(ViewBag.Selectedcls), "");
                    if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        TempData["ExportToExcelDataFromDataTable"] = RM.StoreAllData.Tables[0];
                        if (submit != null)
                        {
                            if (submit.ToLower().Contains("excel") || submit.ToLower().Contains("download"))
                            {
                                TempData["ExportToExcel"] = "1";
                                ExportToExcelDataFromDataTable(ViewBag.SelectedItemText, ViewBag.Selectedcls);
                            }
                        }
                        ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                    }
                }
                else
                {
                    ViewBag.Message = "2";
                    ViewBag.TotalCount = 0;
                }
            }
            catch (Exception ex)
            {
                return View();
            }
            return View(RM);
        }

        #endregion

        #region Cluster/CenterHead Reports 

        [AdminLoginCheckFilter]
        public ActionResult ClusterMarkingStatusReport(ReportModel rm)
        {



            var itemsch1 = new SelectList(new[] { new { ID = "1", Name = "School Code" },
                new { ID = "2", Name = "Cluster Code" }, new { ID = "3", Name = "UDISE Code" }, }, "ID", "Name", 1);
            ViewBag.MySearch = itemsch1.ToList();
            ViewBag.SelectedSearch = "0";

            string AdminId = ViewBag.AdminId = Session["AdminId"].ToString();
            string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();

            string userNM = "admin";

            rm.DistList = AbstractLayer.AdminDB.getAdminDistAllowList(userNM, AdminId);
            var itemReportType = new SelectList(new[] {
                new { ID = "1", Name = "School Marking Status" }, new { ID = "2", Name = "Marking Status Pending School" },
                 new { ID = "4", Name = "District Wise Count Report" },}, "ID", "Name", 1);
            ViewBag.MyReportType = itemReportType.ToList();
            ViewBag.SelectedReportType = "0";



            try
            {

                ViewBag.TotalCount = 0;
                return View(rm);

            }
            catch (Exception ex)
            {

                return View();
            }
        }

        [HttpPost]
        public ActionResult ClusterMarkingStatusReport(ReportModel rm, FormCollection frm, string SearchReportType, string SearchList, string SearchString)
        {


            var itemsch1 = new SelectList(new[] { new { ID = "1", Name = "School Code" },
                new { ID = "2", Name = "Cluster Code" }, new { ID = "3", Name = "UDISE Code" }, }, "ID", "Name", 1);
            ViewBag.MySearch = itemsch1.ToList();
            ViewBag.SelectedSearch = "0";
            string userNM = "admin";
            string userId = "";

            string AdminId = ViewBag.AdminId = Session["AdminId"].ToString();
            string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();



            if (Session["AdminId"] != null)
            {
                userNM = "admin";
                userId = AdminId.ToString();
                rm.DistList = AbstractLayer.AdminDB.getAdminDistAllowList(userNM, AdminId.ToString());

                var itemReportType = new SelectList(new[] {
            new { ID = "1", Name = "School Marking Status" }, new { ID = "2", Name = "Marking Status Pending School" },
                new { ID = "4", Name = "District Wise Count Report" },}, "ID", "Name", 1);
                ViewBag.MyReportType = itemReportType.ToList();
                ViewBag.SelectedReportType = "0";

            }

            try
            {
                string outError = "";
                string Search = string.Empty;
                Search = "ccode like '%' ";

                if (!string.IsNullOrEmpty(rm.Dist))
                {
                    Search += " and dist ='" + rm.Dist.ToString() + "' ";
                }

                if (!string.IsNullOrEmpty(SearchReportType))
                {
                    ViewBag.SelectedReportType = SearchReportType;
                    TempData["SearchReportType"] = SearchReportType;

                    if (!string.IsNullOrEmpty(SearchList) && SearchReportType != "4")
                    {
                        ViewBag.SelectedSearch = SearchList;
                        if (!string.IsNullOrEmpty(SearchString))
                        {
                            if (SearchList == "1")
                            { Search += " and schl ='" + SearchString.ToString() + "'"; }
                            else if (SearchList == "2")
                            { Search += " and ccode ='" + SearchString.ToString() + "'"; }
                            else if (SearchList == "3")
                            { Search += " and chtudise ='" + SearchString.ToString() + "'"; }

                            ViewBag.SearchString = SearchString;
                            TempData["SearchString"] = SearchString;
                        }
                    }

                }


                rm.StoreAllData = AbstractLayer.ReportDB.ClusterMarkingStatusReport(Convert.ToInt32(ViewBag.SelectedReportType), userId, userNM, Search, out outError);
                if (rm.StoreAllData.Tables[0].Rows.Count > 0)
                {
                    ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                }
                else
                {
                    ViewBag.TotalCount = 0;
                }
                return View(rm);

            }
            catch (Exception ex)
            {

                return View();
            }
        }




        public ActionResult ClusterSubjectStatusReport(ClusterReportModel rm)
        {

            var itemReportType = new SelectList(new[] { new { ID = "3", Name = "Subject Status" },
                new { ID = "5", Name = "Final Submit Pending Status" },}, "ID", "Name", 1);
            ViewBag.MyReportType = itemReportType.ToList();
            ViewBag.SelectedReportType = "0";


            var itemsch1 = new SelectList(new[] { new { ID = "1", Name = "Pending" }, }, "ID", "Name", 1);
            //     new { ID = "2", Name = "Marks  Entered" },
            ViewBag.MySearch = itemsch1.ToList();
            ViewBag.SelectedSearch = "0";





            ClusterReportModel clusterReportModel = new ClusterReportModel();
            clusterReportModel = AbstractLayer.ReportDB.BindAllListofClusterReport();
            rm.ClusterList = new List<ClusterModel>();
            rm.SubList = clusterReportModel.SubList;

            string AdminId = ViewBag.AdminId = Session["AdminId"].ToString();
            string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();
            string userNM = "admin";
            if (Session["AdminId"] != null)
            {
                userNM = "admin";
                rm.DistList = AbstractLayer.AdminDB.getAdminDistAllowList(userNM, AdminId.ToString());
            }


            try
            {

                ViewBag.TotalCount = 0;
                return View(rm);

            }
            catch (Exception ex)
            {

                return View();
            }
        }

        [HttpPost]
        public ActionResult ClusterSubjectStatusReport(ClusterReportModel rm, FormCollection frm, string cmd, string SearchReportType, string SearchList)
        {

            var itemReportType = new SelectList(new[] { new { ID = "3", Name = "Subject Status" },
                new { ID = "5", Name = "Final Submit Pending Status" },}, "ID", "Name", 1);
            ViewBag.MyReportType = itemReportType.ToList();
            ViewBag.SelectedReportType = "0";



            var itemsch1 = new SelectList(new[] { new { ID = "1", Name = "Pending" }, }, "ID", "Name", 1);
            //     new { ID = "2", Name = "Marks  Entered" },

            ViewBag.MySearch = itemsch1.ToList();
            ViewBag.SelectedSearch = "0";


            ClusterReportModel clusterReportModel = new ClusterReportModel();
            clusterReportModel = AbstractLayer.ReportDB.BindAllListofClusterReport();
            rm.ClusterList = new List<ClusterModel>();
            rm.SubList = clusterReportModel.SubList;

            string userNM = "admin";
            string userId = "";
            string AdminId = ViewBag.AdminId = Session["AdminId"].ToString();
            string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();
            if (Session["AdminId"] != null)
            {
                userNM = "admin";
                userId = AdminId;
                rm.DistList = AbstractLayer.AdminDB.getAdminDistAllowList(userNM, AdminId.ToString());
            }

            try
            {
                string outError = "";
                string Search = string.Empty;
                Search = "ccode is not null ";

                if (!string.IsNullOrEmpty(SearchReportType))
                {
                    ViewBag.SelectedReportType = SearchReportType;
                    TempData["SearchReportType"] = SearchReportType;
                    ViewBag.SelectedItemText = SearchReportType == null ? "ALL" : itemReportType.ToList().Where(s => s.Value == ViewBag.SelectedReportType).Select(s => s.Text).FirstOrDefault();



                    if (!string.IsNullOrEmpty(rm.sub))
                    {
                        Search += " and sub ='" + rm.sub.ToString() + "' ";
                    }

                    if (!string.IsNullOrEmpty(rm.ccode))
                    {
                        if (rm.ccode != "0")
                        { Search += " and ccode ='" + rm.ccode.ToString() + "' "; }

                    }

                    if (!string.IsNullOrEmpty(rm.Dist))
                    {
                        Search += " and dist ='" + rm.Dist.ToString() + "' ";

                        rm.ClusterList = clusterReportModel.ClusterList.Where(s => s.dist == rm.Dist).ToList();

                    }

                    if (!string.IsNullOrEmpty(SearchList) && SearchReportType != "5")
                    {
                        ViewBag.SelectedSearch = SearchList;
                        if (SearchList == "1")
                        { Search += " and NOCP > 0"; }
                        else if (SearchList == "2")
                        { Search += " and  NOCM > 0"; }
                    }
                    rm.StoreAllData = AbstractLayer.ReportDB.ClusterMarkingStatusReport(Convert.ToInt32(SearchReportType), userId, userNM, Search, out outError);
                    //  rm.StoreAllData = AbstractLayer.ReportDB.ClusterMarkingStatusReport(3, userId, userNM, Search, out outError);
                    if (rm.StoreAllData.Tables[0].Rows.Count > 0)
                    {
                        TempData["ExportToExcelDataFromDataTable"] = rm.StoreAllData.Tables[0];
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                        if (cmd != null)
                        {
                            if (cmd.ToLower().Contains("excel") || cmd.ToLower().Contains("download"))
                            {
                                ViewBag.SelectedItemText = "Export_" + ViewBag.SelectedItemText;
                                TempData["ExportToExcel"] = "1";
                                ExportToExcelDataFromDataTable(ViewBag.SelectedItemText, "ClusterSubjectStatusReport");
                            }
                        }
                    }
                    else
                    {
                        ViewBag.TotalCount = 0;
                    }
                }
                return View(rm);

            }
            catch (Exception ex)
            {

                return View();
            }
        }



        [AdminLoginCheckFilter]
        [AdminMenuFilter]
        public ActionResult TheoryMarksStatusReport(ReportModel RM)
        {
            try
            {
                var itemsch = new SelectList(new[] {
                    new { ID = "1", Name = "Marks Entry Status" }, new { ID = "2", Name = "Overall Marks Entry Status" },}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "5", Name = "Primary" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "5";

                return View(RM);
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        [AdminLoginCheckFilter]
        [AdminMenuFilter]
        [HttpPost]
        public ActionResult TheoryMarksStatusReport(ReportModel RM, FormCollection frm, string submit)
        {
            try
            {
                var itemsch = new SelectList(new[] {
                    new { ID = "1", Name = "Marks Entry Status" }, new { ID = "2", Name = "Overall Marks Entry Status" },}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "5", Name = "Primary" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                string Search = string.Empty;
                if (!string.IsNullOrEmpty(frm["SelList"]) && !string.IsNullOrEmpty(frm["SelClass"]))
                {
                    ViewBag.SelectedItem = frm["SelList"] == null ? "ALL" : frm["SelList"];
                    ViewBag.SelectedItemText = frm["SelList"] == null ? "ALL" : itemsch.ToList().Where(s => s.Value == ViewBag.SelectedItem).Select(s => s.Text).FirstOrDefault();
                    ViewBag.Selectedcls = frm["SelClass"] == null ? "ALL" : frm["SelClass"];
                    RM.StoreAllData = AbstractLayer.ReportDB.TheoryMarksStatusReport(ViewBag.SelectedItem, Convert.ToInt32(ViewBag.Selectedcls));
                    if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        TempData["ExportToExcelDataFromDataTable"] = RM.StoreAllData.Tables[0];
                        if (submit != null)
                        {
                            if (submit.ToLower().Contains("excel") || submit.ToLower().Contains("download"))
                            {
                                TempData["ExportToExcel"] = "1";
                                ExportToExcelDataFromDataTable(ViewBag.SelectedItemText, "TheoryMarksStatusReport");
                            }
                        }
                        ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                    }
                }
                else
                {
                    ViewBag.Message = "2";
                    ViewBag.TotalCount = 0;
                }
            }
            catch (Exception ex)
            {
                return View();
            }
            return View(RM);
        }

        #endregion Cluster/CenterHead Reports 


        #region PanelWiseClassWiseSummaryReport
        [AdminLoginCheckFilter]
        public ActionResult PanelWiseClassWiseSummaryReport()
        {
            var itemsch = new SelectList(new[] { new { ID = "1", Name = "C.C.E/I.N.A" },new { ID = "2", Name = "Pre-Board" },
            new { ID = "3", Name = "Re-Appear INA Summary Report" }, new { ID = "4", Name = "OPEN-INA Summary Report" },
            }, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "1";

            var itemcls = new SelectList(new[] { new { ID = "5", Name = "Primary" }, new { ID = "8", Name = "Middle" },
                new { ID = "2", Name = "Matric" }, new { ID = "4", Name = "Senior Secondary" },}, "ID", "Name", 1);
            ViewBag.Mycls = itemcls.ToList();
            ViewBag.Selectedcls = "0";
            return View();
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult PanelWiseClassWiseSummaryReport(ReportModel RM, FormCollection frm, string Category, string submit) // HttpPostedFileBase file
        {
            try
            {


                var itemsch = new SelectList(new[] { new { ID = "1", Name = "C.C.E/I.N.A" },new { ID = "2", Name = "Pre-Board" },
                 new { ID = "3", Name = "Re-Appear INA Summary Report" },
                new { ID = "4", Name = "OPEN-INA Summary Report" },}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "1";

                var itemcls = new SelectList(new[] { new { ID = "5", Name = "Primary" }, new { ID = "8", Name = "Middle" },
                new { ID = "2", Name = "Matric" }, new { ID = "4", Name = "Senior Secondary" },}, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                //  string firm = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
                //  string id = frm["Filevalue"].ToString();
                //   Category = id;
                ViewBag.ClassName = "";


                if (frm["SelList"] != "" && frm["SelClass"] != "")
                {
                    ViewBag.SelectedItem = frm["SelList"];
                    ViewBag.Selectedcls = frm["SelClass"];
                    ViewBag.ClassName = itemsch.ToList().Where(s => s.Value == frm["SelClass"].ToString()).Select(s => s.Text).SingleOrDefault();

                    string AdminType = Session["AdminType"].ToString();
                    int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                    string Search = string.Empty;
                    if (frm["SelList"] != "" && frm["SelClass"] != "")
                    {
                        ViewBag.SelectedItem = frm["SelList"];
                        ViewBag.Selectedcls = frm["SelClass"];
                        RM.StoreAllData = AbstractLayer.ReportDB.PanelWiseClassWiseSummaryReport(Convert.ToInt32(frm["SelList"]), Convert.ToInt32(frm["SelClass"]));
                        if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                        {

                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                            return View();
                        }
                        else
                        {
                            if (submit != null)
                            {
                                if (submit.ToUpper().Contains("DOWNLOAD") || submit.ToUpper().Contains("EXCEL"))
                                {
                                    if (RM.StoreAllData.Tables[0] != null)
                                    {
                                        string Type = "";
                                        if (ViewBag.SelectedItem == "1") { Type = "CCE"; }

                                        string fileName1 = Type.ToUpper() + "_SummaryReport";
                                        ExportDataFromDataTable(RM.StoreAllData.Tables[0], fileName1);
                                    }
                                }
                            }
                            ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                            return View(RM);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "2";
                        ViewBag.TotalCount = 0;
                        return View();
                    }



                }
                else { return RedirectToAction("Index", "Admin"); }
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }

        }

        #endregion PanelWiseClassWiseSummaryReport

        #region EAffiliationModuleWiseSummaryReport
        [AdminLoginCheckFilter]
        public ActionResult EAffiliationModuleWiseSummaryReport()
        {
            var itemsch = new SelectList(new[] { new { ID = "1", Name = "Annual Progress Summary Report" }, }, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "1";


            return View();
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult EAffiliationModuleWiseSummaryReport(ReportModel RM, FormCollection frm, string SelList, string submit) // HttpPostedFileBase file
        {
            try
            {


                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Annual Progress Summary Report" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "1";


                string AdminType = Session["AdminType"].ToString();
                int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                string Search = string.Empty;

                if (!string.IsNullOrEmpty(SelList))
                {
                    ViewBag.SelectedItem = SelList;
                    RM.StoreAllData = objDB.EAffiliationModuleWiseSummaryReport(Convert.ToInt32(frm["SelList"]), Convert.ToInt32(frm["SelClass"]));
                    if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                    {

                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        if (submit != null)
                        {
                            if (submit.ToUpper().Contains("DOWNLOAD") || submit.ToUpper().Contains("EXCEL"))
                            {
                                if (RM.StoreAllData.Tables[1] != null)
                                {
                                    string Type = "";
                                    if (ViewBag.SelectedItem == "1") { Type = "AnnualProgress"; }

                                    string fileName1 = Type.ToUpper() + "_SummaryReport" + '_' + DateTime.Now.ToString("ddMMyyyy");
                                    ExportDataFromDataTable(RM.StoreAllData.Tables[1], fileName1);
                                }
                            }
                        }
                        ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                    }
                }
                else
                {
                    ViewBag.Message = "2";
                    ViewBag.TotalCount = 0;

                }
                return View(RM);
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }

        }

        #endregion EAffiliationModuleWiseSummaryReport

        #region  MonthWiseCategoryWiseFeeCollectionDetails

        public ActionResult MonthWiseCategoryWiseFeeCollectionDetails()
        {
            ReportModel rp = new ReportModel();
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                // By Rohit  -- select bank from database
                DataSet dsBank = objCommon.Fll_FeeCat_Details();
                if (dsBank != null)
                {
                    if (dsBank.Tables[0].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[0].Rows)
                        {
                            //itemBank.Add(new SelectListItem { Text = @dr["FEECAT"].ToString().Trim(), Value = @dr["FEECAT"].ToString().Trim() });
                            itemBank.Add(new SelectListItem { Text = @dr["FEECAT"].ToString().Trim(), Value = @dr["FEECODE"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }

                    if (dsBank.Tables[1].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank1 = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[1].Rows)
                        {
                            itemBank1.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MyBank = itemBank1.ToList();
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Admin");
                }

                // End 

                List<SelectListItem> itemMonthYear = new List<SelectListItem>();
                ViewBag.YearList = itemMonthYear;
                ViewBag.SelDisplayYearMonth = "0";

                string Search = string.Empty;
                Search = "";
                string outError = "";
                DataSet ds = objDB.MonthWiseCategoryWiseFeeCollectionDetails(Search, out outError);  //1 for Total Registration by School Report
                rp.StoreAllData = ds;
                if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    ViewBag.TotalCount1 = 0;
                    ViewBag.Total = 0;
                }
                else
                {

                    foreach (System.Data.DataRow dr in rp.StoreAllData.Tables[0].Rows)
                    {
                        itemMonthYear.Add(new SelectListItem { Text = @dr["DisplayYearMonth"].ToString().Trim(), Value = @dr["DisplayYearMonth"].ToString().Trim() });
                    }
                    ViewBag.YearList = itemMonthYear;

                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.Total = 0;
                    ViewBag.TotalCount1 = 0;
                    ViewBag.AmountInWords = "";

                }
                return View(rp);

            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [HttpPost]
        public ActionResult MonthWiseCategoryWiseFeeCollectionDetails(FormCollection frm, string submit, string YearMonth)
        {
            ReportModel rp = new ReportModel();
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                // By Rohit  -- select bank from database
                DataSet dsBank = objCommon.Fll_FeeCat_Details();
                if (dsBank != null)
                {
                    if (dsBank.Tables[0].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[0].Rows)
                        {
                            itemBank.Add(new SelectListItem { Text = @dr["FEECAT"].ToString().Trim(), Value = @dr["FEECODE"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }

                    if (dsBank.Tables[1].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank1 = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[1].Rows)
                        {
                            itemBank1.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MyBank = itemBank1.ToList();
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Admin");
                }

                List<SelectListItem> itemMonthYear = new List<SelectListItem>();
                ViewBag.YearList = itemMonthYear;
                ViewBag.SelDisplayYearMonth = "";

                // End 
                string Search = string.Empty;
                Search = "a.FEECODE is not null ";

                if (!string.IsNullOrEmpty(YearMonth))
                {
                    string mn = YearMonth.Split('-')[0];
                    string yr = YearMonth.Split('-')[1];
                    Search += " and datename(mm,SettlementDate)='" + mn + "' and YEAR(SettlementDate)='" + yr + "' ";
                    ViewBag.SelDisplayYearMonth = YearMonth;
                    TempData["SelDisplayYearMonth"] = YearMonth;
                }
                if (!string.IsNullOrEmpty(frm["FEECAT"]))
                {
                    Search += " and a.FEECODE in (" + frm["FEECAT"].ToString().Trim() + ")";
                    ViewBag.SelectedItem = frm["FEECAT"];
                    TempData["FEECAT"] = frm["FEECAT"];
                }
                if (frm["Bank"] != "")
                {
                    Search += " and a.bcode='" + frm["Bank"].ToString().Trim() + "'";
                    ViewBag.SelectedBank = frm["Bank"];
                    TempData["Bank"] = frm["Bank"];
                }

                string outError = "";
                DataSet ds = objDB.MonthWiseCategoryWiseFeeCollectionDetails(Search, out outError);  //1 for Total Registration by School Report
                rp.StoreAllData = ds;
                if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    ViewBag.TotalCount1 = 0;
                    ViewBag.Total = 0;
                    return View(rp);
                }
                else
                {
                    foreach (System.Data.DataRow dr in rp.StoreAllData.Tables[0].Rows)
                    {
                        itemMonthYear.Add(new SelectListItem { Text = @dr["DisplayYearMonth"].ToString().Trim(), Value = @dr["DisplayYearMonth"].ToString().Trim() });
                    }
                    ViewBag.YearList = itemMonthYear;

                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = rp.StoreAllData.Tables[1].Rows.Count;

                    TempData["ExportToExcelDataFromDataTable"] = rp.StoreAllData.Tables[1];
                    if (submit != null)
                    {
                        if (submit.ToLower().Contains("excel") || submit.ToLower().Contains("download"))
                        {

                            ViewBag.SelectedItemText = "Export_" + ViewBag.SelDisplayYearMonth;
                            TempData["ExportToExcel"] = "1";
                            ExportToExcelDataFromDataTable(ViewBag.SelectedItemText, "MonthWiseFeeCollectionDetails");
                        }
                    }

                    return View(rp);
                }
            }
            catch (Exception ex)
            {
                // oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        #endregion MonthWiseCategoryWiseFeeCollectionDetails

        #region MagazineSchoolRequirementsReport
        [AdminLoginCheckFilter]
        public ActionResult MagazineSchoolRequirementsReport()
        {
            var itemsch = new SelectList(new[] {
            new { ID = "1", Name = "School Requirement Report" },
            new { ID = "2", Name = "Summary Report" },
            new { ID = "3", Name = "Download Pending School Data" },
             new { ID = "4", Name = "Download Data" },}, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "1";

            return View();
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult MagazineSchoolRequirementsReport(ReportModel RM, FormCollection frm, string SelList, string submit) // HttpPostedFileBase file
        {
            try
            {

                var itemsch = new SelectList(new[] {
            new { ID = "1", Name = "School Requirement Report" },
            new { ID = "2", Name = "Summary Report" },
            new { ID = "3", Name = "Download Pending School Data" },
                 new { ID = "4", Name = "Download Data" },}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "1";


                string AdminType = Session["AdminType"].ToString();
                int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                string Search = string.Empty;

                if (!string.IsNullOrEmpty(SelList))
                {
                    ViewBag.SelectedItem = SelList;
                    RM.StoreAllData = objDB.MagazineSchoolRequirementsReport(Convert.ToInt32(SelList));
                    if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        if (submit != null)
                        {
                            if (submit.ToUpper().Contains("DOWNLOAD") || submit.ToUpper().Contains("EXCEL"))
                            {
                                if (RM.StoreAllData.Tables[0] != null)
                                {
                                    string Type = itemsch.ToList().Where(s => s.Value == SelList).Select(s => s.Text).SingleOrDefault();
                                    string fileName1 = Type.ToUpper().Replace(" ", "_") + "_Magazine";
                                    ExportDataFromDataTable(RM.StoreAllData.Tables[0], fileName1);
                                }
                            }
                        }
                        ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                    }
                }
                else
                {
                    ViewBag.Message = "2";
                    ViewBag.TotalCount = 0;

                }
                return View(RM);
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }

        }

        #endregion MagazineSchoolRequirementsReport


        #region UndertakingOfQuestionPapersReport
        [AdminLoginCheckFilter]
        public ActionResult UndertakingOfQuestionPapersReport()
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            //
            var itemsch = new SelectList(new[] {
            new { ID = "1", Name = "Pending List Report" },
            new { ID = "2", Name = "Not-Satisfactory List Report" },
            new { ID = "3", Name = "Submitted List Report" },}, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "0";
            //
            ViewBag.MonthList = AbstractLayer.DBClass.GetMonthNameNumber();
            ViewBag.SelMonth = "0";
            ViewBag.YearList = AbstractLayer.DBClass.GetSessionSingle();
            ViewBag.SelYear = "0";
            ViewBag.SelMonthNM = "";
            return View();
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult UndertakingOfQuestionPapersReport(ReportModel RM, FormCollection frm, string Month, string Year, string SelList, string submit) // HttpPostedFileBase file
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            //
            var itemsch = new SelectList(new[] {
            new { ID = "1", Name = "Pending List Report" },
            new { ID = "2", Name = "Not-Satisfactory List Report" },
            new { ID = "3", Name = "Submitted List Report" },}, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "0";
            //
            ViewBag.MonthList = AbstractLayer.DBClass.GetMonthNameNumber();
            ViewBag.SelMonth = Month;
            ViewBag.YearList = AbstractLayer.DBClass.GetSessionSingle();
            ViewBag.SelYear = Year;
            ViewBag.SelMonthNM = AbstractLayer.DBClass.GetMonthNameNumber().Where(s => s.Value == Month).Select(s => s.Text).SingleOrDefault();
            try
            {



                string Search = string.Empty;

                if (!string.IsNullOrEmpty(SelList))
                {
                    ViewBag.SelectedItem = SelList;
                    RM.StoreAllData = AbstractLayer.ReportDB.UndertakingOfQuestionPapersReport(Convert.ToInt32(SelList), Month, Year);
                    if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        if (submit != null)
                        {
                            if (submit.ToUpper().Contains("DOWNLOAD") || submit.ToUpper().Contains("EXCEL"))
                            {
                                if (RM.StoreAllData.Tables[0] != null)
                                {
                                    string Type = itemsch.ToList().Where(s => s.Value == SelList).Select(s => s.Text).SingleOrDefault();
                                    string fileName1 = Type.ToUpper().Replace(" ", "_") + "_QuestionPapers";
                                    ExportDataFromDataTable(RM.StoreAllData.Tables[0], fileName1);
                                }
                            }
                        }
                        ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                    }
                }
                else
                {
                    ViewBag.Message = "2";
                    ViewBag.TotalCount = 0;

                }
                return View(RM);
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }

        }

        #endregion UndertakingOfQuestionPapersReport


        #region PreviousClassMarksOfSeniorPanelWiseSummaryReport
        [AdminLoginCheckFilter]
        public ActionResult PreviousClassMarksOfSeniorPanelWiseSummaryReport()
        {
            var itemsch = new SelectList(new[] { new { ID = "1", Name = "Previous Year Marks Report"  },
                 new { ID = "2", Name = "Matriculation Marks for +2 Students Report" },
            new { ID = "3", Name = "Re-Appear INA Pending Report" },
            new { ID = "4", Name = "Open INA Report" },
              new { ID = "5", Name = "Open Matriculation Marks for +2 Students Report" },}, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "0";

            var itemcls = new SelectList(new[] {new { ID = "8", Name = "Middle" },  new { ID = "10", Name = "Matric" },
            new { ID = "12", Name = "Senior Secondary" },}, "ID", "Name", 1);
            ViewBag.Mycls = itemcls.ToList();
            ViewBag.Selectedcls = "1";
            return View();


        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult PreviousClassMarksOfSeniorPanelWiseSummaryReport(ReportModel RM, FormCollection frm, string Category, string submit) // HttpPostedFileBase file
        {
            try
            {


                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Previous Year Marks Report"  },
                new { ID = "2", Name = "Matriculation Marks for +2 Students Report" },
                new { ID = "3", Name = "Re-Appear INA Pending Report" },
                new { ID = "4", Name = "Open INA  Report" },
                new { ID = "5", Name = "Open Matriculation Marks for +2 Students Report" },}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] {new { ID = "8", Name = "Middle" },  new { ID = "10", Name = "Matric" },
               new { ID = "12", Name = "Senior Secondary" },}, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();

                ViewBag.ClassName = "";


                if (frm["SelList"] != "" && frm["SelClass"] != "")
                {
                    ViewBag.SelectedItem = frm["SelList"];
                    ViewBag.Selectedcls = frm["SelClass"];
                    ViewBag.ClassName = itemcls.ToList().Where(s => s.Value == frm["SelClass"].ToString()).Select(s => s.Text).SingleOrDefault();
                    ViewBag.PanelName = itemsch.ToList().Where(s => s.Value == frm["SelList"].ToString()).Select(s => s.Text).SingleOrDefault();

                    RM.StoreAllData = objDB.PreviousClassMarksOfSeniorPanelWiseSummaryReport(Convert.ToInt32(frm["SelList"]), Convert.ToInt32(frm["SelClass"]));
                    if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        if (submit != null)
                        {
                            if (submit.ToUpper().Contains("DOWNLOAD") || submit.ToUpper().Contains("EXCEL"))
                            {
                                if (RM.StoreAllData.Tables[0] != null)
                                {
                                    string fileName1 = ViewBag.PanelName;
                                    ExportDataFromDataTable(RM.StoreAllData.Tables[0], fileName1);
                                }
                            }
                        }
                        ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                        return View(RM);
                    }
                }
                else
                {
                    ViewBag.Message = "2";
                    ViewBag.TotalCount = 0;
                    return View();
                }
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }

        }

        #endregion PreviousClassMarksOfSeniorPanelWiseSummaryReport


        #region  OverAllVerifiedFeeCollectionDetails

        public ActionResult OverAllVerifiedFeeCollectionDetails()
        {
            ReportModel rp = new ReportModel();
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                // By Rohit  -- select bank from database
                DataSet dsBank = objCommon.Fll_FeeCat_Details();
                if (dsBank != null)
                {
                    if (dsBank.Tables[0].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[0].Rows)
                        {
                            itemBank.Add(new SelectListItem { Text = @dr["FEECAT"].ToString().Trim(), Value = @dr["FEECODE"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }

                    if (dsBank.Tables[1].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank1 = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[1].Rows)
                        {
                            itemBank1.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MyBank = itemBank1.ToList();
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Admin");
                }

                var itemcls = new SelectList(new[] { new { ID = "1", Name = "Date Fee Head and Bank Wise " }, new { ID = "2", Name = "Date and Fee Head Wise " },
                    new { ID = "3", Name = "Date Wise" }, new { ID = "4", Name = "Fee Head and Bank Wise" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                var itemDateType = new SelectList(new[] { new { ID = "S", Name = "Settlement" },
                    new { ID = "T", Name = "Transaction " },}, "ID", "Name", 1);
                ViewBag.MyDateType = itemDateType.ToList();
                ViewBag.SelectedDateType = "0";

                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                ViewBag.Total = 0;
                return View(rp);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult OverAllVerifiedFeeCollectionDetails(FormCollection frm, string submit)
        {
            ReportModel rp = new ReportModel();
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                // By Rohit  -- select bank from database
                DataSet dsBank = objCommon.Fll_FeeCat_Details();
                if (dsBank != null)
                {
                    if (dsBank.Tables[0].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[0].Rows)
                        {
                            itemBank.Add(new SelectListItem { Text = @dr["FEECAT"].ToString().Trim(), Value = @dr["FEECODE"].ToString().Trim() });
                        }
                        ViewBag.MySch = itemBank.ToList();
                    }

                    if (dsBank.Tables[1].Rows.Count > 0)
                    {
                        List<SelectListItem> itemBank1 = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dsBank.Tables[1].Rows)
                        {
                            itemBank1.Add(new SelectListItem { Text = @dr["Bank"].ToString().Trim(), Value = @dr["Bcode"].ToString().Trim() });
                        }
                        ViewBag.MyBank = itemBank1.ToList();
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Admin");
                }
                var itemcls = new SelectList(new[] { new { ID = "1", Name = "Date Fee Head and Bank Wise " }, new { ID = "2", Name = "Date and Fee Head Wise " },
                    new { ID = "3", Name = "Date Wise" }, new { ID = "4", Name = "Fee Head and Bank Wise" }, }, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";

                var itemDateType = new SelectList(new[] { new { ID = "S", Name = "Settlement" },
                    new { ID = "T", Name = "Transaction " },}, "ID", "Name", 1);
                ViewBag.MyDateType = itemDateType.ToList();
                ViewBag.SelectedDateType = "0";
                // End 
                string Search = string.Empty;
                Search = "a.FEECODE like '%' ";
                string SelDateType = "T";

                if (frm["SelClass"] != "")
                {
                    ViewBag.Selectedcls = frm["SelClass"];
                    TempData["SelClass"] = frm["SelClass"];
                }



                if (!string.IsNullOrEmpty(frm["FEECAT"]))
                {
                    Search += " and a.FEECODE in (" + frm["FEECAT"].ToString().Trim() + ")";
                    ViewBag.SelectedItem = frm["FEECAT"];
                    TempData["FEECAT"] = frm["FEECAT"];
                }
                if (frm["Bank"] != "")
                {
                    Search += " and a.bcode='" + frm["Bank"].ToString().Trim() + "'";
                    ViewBag.SelectedBank = frm["Bank"];
                    TempData["Bank"] = frm["Bank"];
                }


                if (frm["DateType"] != "")
                {
                    SelDateType = frm["DateType"];
                    ViewBag.SelectedDateType = SelDateType;
                    if (SelDateType == "T")
                    {
                        if (frm["FromDate"] != "")
                        {
                            ViewBag.FromDate = frm["FromDate"];
                            TempData["FromDate"] = frm["FromDate"];
                            Search += " and CONVERT(DATETIME, CONVERT(varchar(10),DEPOSITDT,103), 103)>=CONVERT(DATETIME, CONVERT(varchar(10),'" + frm["FromDate"].ToString() + "',103), 103)";
                        }
                        if (frm["ToDate"] != "")
                        {
                            ViewBag.ToDate = frm["ToDate"];
                            TempData["ToDate"] = frm["ToDate"];
                            Search += " and CONVERT(DATETIME, CONVERT(varchar(10),DEPOSITDT,103), 103)<=CONVERT(DATETIME, CONVERT(varchar(10),'" + frm["ToDate"].ToString() + "',103), 103)";
                        }
                    }
                    else if (SelDateType == "S")
                    {
                        if (frm["FromDate"] != "")
                        {
                            ViewBag.FromDate = frm["FromDate"];
                            TempData["FromDate"] = frm["FromDate"];
                            Search += " and CONVERT(DATETIME, CONVERT(varchar(10),SettlementDate,103), 103)>=CONVERT(DATETIME, CONVERT(varchar(10),'" + frm["FromDate"].ToString() + "',103), 103)";
                        }
                        if (frm["ToDate"] != "")
                        {
                            ViewBag.ToDate = frm["ToDate"];
                            TempData["ToDate"] = frm["ToDate"];
                            Search += " and CONVERT(DATETIME, CONVERT(varchar(10),SettlementDate,103), 103)<=CONVERT(DATETIME, CONVERT(varchar(10),'" + frm["ToDate"].ToString() + "',103), 103)";
                        }
                    }
                }

                //if (!string.IsNullOrEmpty(submit))
                //{
                //    string bankcode = frm["Bank"];
                //    string feecat = frm["FEECAT"];
                //    string FromDate = frm["FromDate"];
                //    string ToDate = frm["ToDate"];
                //    if (submit.ToLower().Contains("download") || submit.ToLower().Contains("excel"))
                //    {
                //        string outError1 = "";
                //        rp.StoreAllData = objDB.BankWiseChallanMasterData(SelDateType, Search, bankcode, feecat, "0", out outError1);
                //        if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
                //        {
                //            ViewBag.Message = "Record Not Found";
                //            ViewBag.TotalCount = 0;
                //            return View(rp);
                //        }
                //        else
                //        {
                //            ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                //            string fileName1 = bankcode + "_" + FromDate + "_" + ToDate + "_ChallanMasterData";
                //            ExportDataFromDataTable(rp.StoreAllData.Tables[0], fileName1);
                //            return View(rp);
                //        }
                //    }
                //}

                string outError = "";
                // DataSet ds = objDB.DateWiseFeeCollectionDetails(SelDateType, Search, ViewBag.Selectedcls, out outError);  
                DataSet ds = AbstractLayer.ReportDB.OverAllVerifiedFeeCollectionDetails(SelDateType, Search, ViewBag.Selectedcls, out outError);
                rp.StoreAllData = ds;
                if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    ViewBag.Total = 0;
                    return View(rp);
                }
                else
                {
                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                    string typeName = rp.StoreAllData.Tables[0].Columns["TOTFEE"].DataType.Name;
                    ViewBag.Total = rp.StoreAllData.Tables[0].AsEnumerable().Sum(x => Convert.ToInt32(x.Field<Double>("TOTFEE")));
                    ViewBag.AmountInWords = objCommon.GetAmountInWords(Convert.ToInt32(ViewBag.Total));
                    return View(rp);
                }
            }
            catch (Exception ex)
            {
                // oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        #endregion OverAllVerifiedFeeCollectionDetails


        #region  OpenSchoolAdmissionCandidateReport

        public ActionResult OpenSchoolAdmissionCandidateReport(ReportModel rp)
        {
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                //All, Schl Code, Dist Code, App No, Aadhar No
                var itemcls = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Schl Code" },
                    new { ID = "3", Name = "Dist Code" }, new { ID = "4", Name = "App No" }, new { ID = "5", Name = "Aadhar No" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemcls.ToList();
                ViewBag.SelectedItem = "0";


                ViewBag.Searchstring = null;
                ViewBag.Search = null;

                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                ViewBag.Total = 0;
                return View(rp);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult OpenSchoolAdmissionCandidateReport(FormCollection frm, string submit, string SelList, string SearchString, string FromDate, string ToDate)
        {
            ReportModel rp = new ReportModel();
            try
            {
                //All, Schl Code, Dist Code, App No, Aadhar No
                var itemcls = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Schl Code" },
                    new { ID = "3", Name = "Dist Code" }, new { ID = "4", Name = "App No" }, new { ID = "5", Name = "Aadhar No" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemcls.ToList();
                ViewBag.SelectedItem = "0";

                ViewBag.Searchstring = null;
                ViewBag.Search = null;
                // End 
                string Search = string.Empty;
                Search = "b.appno is not null ";

                if (frm["FromDate"] != "")
                {
                    ViewBag.FromDate = frm["FromDate"];
                    TempData["FromDate"] = frm["FromDate"];
                    Search += " and CONVERT(DATETIME, CONVERT(varchar(10),b.DEPOSITDT,103), 103)>=CONVERT(DATETIME, CONVERT(varchar(10),'" + frm["FromDate"].ToString() + "',103), 103)";
                }
                if (frm["ToDate"] != "")
                {
                    ViewBag.ToDate = frm["ToDate"];
                    TempData["ToDate"] = frm["ToDate"];
                    Search += " and CONVERT(DATETIME, CONVERT(varchar(10),b.DEPOSITDT,103), 103)<=CONVERT(DATETIME, CONVERT(varchar(10),'" + frm["ToDate"].ToString() + "',103), 103)";
                }


                if (!string.IsNullOrEmpty(SelList))
                {

                    ViewBag.SelectedItem = SelList;
                    TempData["SelList"] = SelList;
                    int SelValueSch = Convert.ToInt32(SelList);

                    ViewBag.Search = itemcls.SingleOrDefault(s => s.Value == SelList).Text;

                    if (frm["SearchString"] != "")
                    {
                        ViewBag.Searchstring = frm["SearchString"].ToString();
                        if (SelValueSch == 2)
                        { Search += " and  b.schl ='" + frm["SearchString"].ToString() + "'"; }
                        else if (SelValueSch == 3)
                        { Search += " and b.dist= '" + frm["SearchString"].ToString() + "'"; }
                        else if (SelValueSch == 4)
                        { Search += " and b.appno = '" + frm["SearchString"].ToString() + "'"; }
                        else if (SelValueSch == 5)
                        { Search += " and b.AADHAR_NO='" + frm["SearchString"].ToString() + "'"; }

                    }

                }





                string outError = "";
                int SelType = 0;
                DataSet ds = AbstractLayer.ReportDB.OpenSchoolAdmissionCandidateReport(SelType, Search, out outError);
                rp.StoreAllData = ds;
                if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    ViewBag.Total = 0;
                }
                else
                {
                    ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                }
            }
            catch (Exception ex)
            {
                // oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));               
            }
            return View(rp);
        }
        #endregion OpenSchoolAdmissionCandidateReport



        #region ModuleWisePendingSummaryReports
        [AdminLoginCheckFilter]
        public ActionResult ModuleWisePendingSummaryReports()
        {
            var itemsch = new SelectList(new[] {
            new { ID = "1", Name = "Pending Class Wise School List Of Registration" },
            new { ID = "5", Name = "Pending Class Wise School List Of Examination" },
            new { ID = "2", Name = "Late Fees Summary Report" },
            new { ID = "3", Name = "Late Fees UserType-School Wise Summary Report" },
            new { ID = "4", Name = "Late Fees UserType Wise Summary Report" },
            new { ID = "7", Name = "Open Students Step Wise Summary Report" },
            new { ID = "8", Name = "School Based Exam Portal Regular" },
            new { ID = "9", Name = "School Based Exam Portal Open" },
            new { ID = "10", Name = "Center Head Wise Pending Summary Report" },
                }, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "0";

            var itemcls = new SelectList(new[] {  new { ID = "0", Name = "All Classes" }, new { ID = "5", Name = "Primary" }, new { ID = "8", Name = "Middle" },
            new { ID = "10", Name = "Matric" }, new { ID = "12", Name = "Senior Secondary" },}, "ID", "Name", 1);
            ViewBag.Mycls = itemcls.ToList();
            ViewBag.Selectedcls = "0";

            return View();
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult ModuleWisePendingSummaryReports(ReportModel RM, FormCollection frm, string SelList, string SelClass, string submit) // HttpPostedFileBase file
        {
            try
            {
                var itemsch = new SelectList(new[] {
            new { ID = "1", Name = "Pending Class Wise School List Of Registration" },
            new { ID = "5", Name = "Pending Class Wise School List Of Examination" },
            new { ID = "2", Name = "Late Fees Summary Report" },
            new { ID = "3", Name = "Late Fees UserType-School Wise Summary Report" },
            new { ID = "4", Name = "Late Fees UserType Wise Summary Report" },
            new { ID = "7", Name = "Open Students Step Wise Summary Report" },
            new { ID = "8", Name = "School Based Exam Portal Regular" },
            new { ID = "9", Name = "School Based Exam Portal Open" },
            new { ID = "10", Name = "Center Head Wise Pending Summary Report" },
                }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "0", Name = "All Classes" },  new { ID = "5", Name = "Primary" }, new { ID = "8", Name = "Middle" },
                new { ID = "10", Name = "Matric" }, new { ID = "12", Name = "Senior Secondary" },}, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();
                ViewBag.Selectedcls = "0";


                string AdminType = Session["AdminType"].ToString();
                int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
                string Search = string.Empty;

                if (!string.IsNullOrEmpty(SelList) && !string.IsNullOrEmpty(SelClass))
                {
                    ViewBag.SelectedItem = SelList;
                    ViewBag.Selectedcls = SelClass;
                    RM.StoreAllData = AbstractLayer.ReportDB.ModuleWisePendingSummaryReport(Convert.ToInt32(SelList), Convert.ToInt32(SelClass));
                    if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        if (submit != null)
                        {
                            if (submit.ToUpper().Contains("DOWNLOAD") || submit.ToUpper().Contains("EXCEL"))
                            {
                                if (RM.StoreAllData.Tables[0] != null)
                                {
                                    string Type = itemsch.ToList().Where(s => s.Value == SelList).Select(s => s.Text).SingleOrDefault();
                                    string SelClass1 = itemcls.ToList().Where(s => s.Value == SelClass).Select(s => s.Text).SingleOrDefault();

                                    string fileName1 = Type.ToUpper().Replace(" ", "_") + "_" + SelClass1.ToUpper().Replace(" ", "_");
                                    ExportDataFromDataTable(RM.StoreAllData.Tables[0], fileName1);
                                }
                            }
                        }
                        ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                    }
                }
                else
                {
                    ViewBag.Message = "2";
                    ViewBag.TotalCount = 0;

                }
                return View(RM);
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }

        }

        #endregion MagazineSchoolRequirementsReport



        #region RegistrationReport



        [AdminLoginCheckFilter]
        public ActionResult CenterCreationReport(ReportModel sm)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

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
                ViewBag.MyTerm = objCommon.GetTerm();
                ViewBag.MySchoolType = objCommon.GetSchool();


                return View(sm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult CenterCreationReport(ReportModel sm, FormCollection frm, string SearchString)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

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

                ViewBag.MyTerm = objCommon.GetTerm();
                ViewBag.MySchoolType = objCommon.GetSchool();




                if (ModelState.IsValid)
                {

                    string Search = string.Empty;
                    Search = "CentreAppNo is not null  ";

                    if (frm["Term"] != "")
                    {
                        ViewBag.SelectedTerm = frm["Term"];
                        TempData["SelectedTermt"] = frm["Term"];
                    }


                    if (frm["Dist1"] != "")
                    {
                        ViewBag.SelectedDist = frm["Dist1"];
                        TempData["SelectedDist"] = frm["Dist1"];
                        Search += " and DIST='" + frm["Dist1"].ToString() + "'";
                    }

                    if (frm["SchoolType"] != "")
                    {
                        ViewBag.SelectedSchoolType = frm["SchoolType"];
                        TempData["SelectedSchoolType"] = frm["SchoolType"];
                        Search += " and USERTYPE='" + frm["SchoolType"].ToString() + "'";
                    }


                    if (!string.IsNullOrEmpty(SearchString))
                    {
                        Search += " and  SCHL='" + SearchString + "'";
                        ViewBag.SearchString = SearchString;
                        TempData["SearchString"] = SearchString;
                    }

                    if (DistAllow != "")
                    {
                        Search += " and DIST in (" + DistAllow + ")";
                    }


                    TempData["SearchCenterCreationReport"] = Search;
                    TempData.Keep(); // to store search value for view
                    sm.StoreAllData = AbstractLayer.SchoolDB.OnlineCenterCreationReport(ViewBag.SelectedTerm, 1, Search);
                    ViewBag.DISTNM = ViewBag.DIST = "";
                    if (sm.StoreAllData == null || sm.StoreAllData.Tables[0].Rows.Count == 0)
                    {

                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        Session["CenterCreationReport"] = sm.StoreAllData.Tables[0];
                        ViewBag.TotalCount = sm.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.DIST = sm.StoreAllData.Tables[0].Rows[0]["DIST"].ToString();
                        ViewBag.DISTNM = sm.StoreAllData.Tables[0].Rows[0]["DISTNM"].ToString();

                    }
                }
                return View(sm);
            }
            catch (Exception ex)
            {
                return View();
            }
        }


        #endregion RegistrationReport

        #region OnDemandCertificatesSummaryReport
        [AdminLoginCheckFilter]
        public ActionResult OnDemandCertificatesSummaryReport()
        {
            var itemsch = new SelectList(new[] { new { ID = "1", Name = "Class Wise Summary Report"  },
                 new { ID = "2", Name = "Class Wise School Wise Summary Report" }, }, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "0";

            var itemcls = new SelectList(new[] { new { ID = "5", Name = "Primary" },new { ID = "8", Name = "Middle" },  new { ID = "10", Name = "Matric" },
               new { ID = "12", Name = "Senior Secondary" },}, "ID", "Name", 1);
            ViewBag.Mycls = itemcls.ToList();
            ViewBag.Selectedcls = "1";
            return View();
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult OnDemandCertificatesSummaryReport(ReportModel RM, FormCollection frm, string Category, string submit) // HttpPostedFileBase file
        {
            try
            {
                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Class Wise Summary Report"  },
                 new { ID = "2", Name = "Class Wise School Wise Summary Report" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = "0";

                var itemcls = new SelectList(new[] { new { ID = "5", Name = "Primary" },new { ID = "8", Name = "Middle" },  new { ID = "10", Name = "Matric" },
               new { ID = "12", Name = "Senior Secondary" },}, "ID", "Name", 1);
                ViewBag.Mycls = itemcls.ToList();

                ViewBag.ClassName = "";


                if (frm["SelList"] != "")
                {
                    ViewBag.SelectedItem = frm["SelList"];
                    ViewBag.PanelName = itemsch.ToList().Where(s => s.Value == frm["SelList"].ToString()).Select(s => s.Text).SingleOrDefault();

                    if (frm["SelClass"] != "")
                    {
                        ViewBag.Selectedcls = frm["SelClass"];
                        ViewBag.ClassName = itemcls.ToList().Where(s => s.Value == frm["SelClass"].ToString()).Select(s => s.Text).SingleOrDefault();
                    }

                    RM.StoreAllData = AbstractLayer.ReportDB.OnDemandCertificatesSummaryReport(Convert.ToInt32(frm["SelList"]), Convert.ToString(frm["SelClass"]));
                    if (RM.StoreAllData == null || RM.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        if (submit != null)
                        {
                            if (submit.ToUpper().Contains("DOWNLOAD") || submit.ToUpper().Contains("EXCEL"))
                            {
                                if (RM.StoreAllData.Tables[0] != null)
                                {
                                    string fileName1 = ViewBag.PanelName + ViewBag.ClassName;
                                    ExportDataFromDataTable(RM.StoreAllData.Tables[0], fileName1);
                                }
                            }
                        }
                        ViewBag.TotalCount = RM.StoreAllData.Tables[0].Rows.Count;
                        return View(RM);
                    }
                }
                else
                {
                    ViewBag.Message = "2";
                    ViewBag.TotalCount = 0;
                    return View();
                }
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                ViewData["Result"] = "-3";
                ViewBag.Message = ex.Message;
                return View();
            }

        }

        #endregion PreviousClassMarksOfSeniorPanelWiseSummaryReport

        #region EAffiliationDownloadData
        [AdminLoginCheckFilter]
        public ActionResult EAffiliationDownloadData(ReportModel rp)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

            //AppType
            ViewBag.MyAppType = AbstractLayer.EAffiliationDB.GetApplicationTypeList().ToList();
            ViewBag.SelectedAppType = "0";

            return View(rp);
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult EAffiliationDownloadData(ReportModel rp, string AppType, string submit)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

            //AppType
            ViewBag.MyAppType = AbstractLayer.EAffiliationDB.GetApplicationTypeList().ToList();


            rp.StoreAllData = AbstractLayer.ReportDB.EAffiliation_AppType_DownloadData(AppType);
            if (rp.StoreAllData == null || rp.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.TotalCount = 0;
            }
            else
            {
                ViewBag.TotalCount = rp.StoreAllData.Tables[0].Rows.Count;
                if (submit != null)
                {
                    if (submit.ToUpper().Contains("DOWNLOAD") || submit.ToUpper().Contains("EXCEL"))
                    {
                        if (rp.StoreAllData.Tables[0] != null)
                        {
                            string AppTypeNM = AbstractLayer.EAffiliationDB.GetApplicationTypeList().ToList().SingleOrDefault(s => s.Value == AppType).Text.ToUpper().Replace(" ", "");

                            string fileName1 = AppTypeNM + "_DownloadData";
                            ExportDataFromDataTable(rp.StoreAllData.Tables[0], fileName1);
                        }
                    }
                }

            }
            return View(rp);
        }
        #endregion EAffiliationDownloadData

        #region CentreExamDateView
        public ActionResult CentreExamDate()
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
        public ActionResult CentreExamDate(ReportModel rp, string Class, string examDate, string Center)
        {
            try
            {
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                ViewBag.ExamDate = examDate;
                ViewBag.Center = Center;
                examDate = " a.ExamDate='" + examDate + "' ";

                DataSet ds = new AbstractLayer.SchoolDB().CentreExamDateView(0, examDate, Center); // type=4 for summary report
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


        #region Attendance Admin Panel
        [AdminLoginCheckFilter]
        public ActionResult AttendanceReport()
        {
            try
            {
                AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
                if (Session["AdminLoginSession"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }
                

                var itemClass = new SelectList(new[] { new { ID = "12", Name = "SrSec" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.MyClass = itemClass.ToList();
                
                var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" }, new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                ViewBag.MyRP = itemRP.ToList();
                //List<AttendanceAdminReport> obj = new List<AttendanceAdminReport>();
                //obj = _context.AttendanceAdmin.ToList();
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }

        }
        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult AttendanceReport(FormCollection frm)
        {

            try
            {
                AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
                if (Session["AdminLoginSession"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                string centercode = frm["centercode"].ToString();
                string cls = frm["Class"].ToString();
                string category = frm["SelRP"].ToString();
                ViewBag.SelectedClass = cls;
                ViewBag.SelectedRP = category;
                ViewBag.centercode = centercode;

                var itemClass = new SelectList(new[] { new { ID = "12", Name = "SrSec" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.MyClass = itemClass.ToList();

                var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" }, new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                ViewBag.MyRP = itemRP.ToList();
                ViewBag.ClassSelected = frm["Class"].ToString();
                string sCls = frm["Class"].ToString();
                List<AttendanceAdminReport> obj = new List<AttendanceAdminReport>();
                //obj = _context.AttendanceAdmin.Where(s => s.cls == (sCls == "" ? s.cls : sCls) && s.centrecode == (centercode == "" ? s.centrecode : centercode) && s.rp == (category == "" ? s.rp : category)).ToList();
                obj = AbstractLayer.AttendanceDB.AttendenceSummaryDetailsAdmin(centercode, sCls, category);
                //AttendenceSummaryDetailsSPAdmin
                if (frm["Export"] != null)
                {
                    string fileName1 = "AttendanceReport_Data_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        wb.Worksheets.Add(ToDataTable(obj));
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

                return View(obj);
            }
            catch (Exception ex)
            {
                return View();
            }

        }


        #endregion
    }
}