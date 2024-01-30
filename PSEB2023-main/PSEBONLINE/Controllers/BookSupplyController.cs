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


namespace PSEBONLINE.Controllers
{
    public class BookSupplyController : Controller
    {
        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.BookSupplyDB objDB = new AbstractLayer.BookSupplyDB();
        private PrinterModel PM = new PrinterModel();

        // GET: Printer
        public ActionResult Index()
        {
            return View();
        }


        #region  Login BookSupply
        [Route("BookSupply")]
        [Route("BookSupply/Login")]
        public ActionResult Login()
        {
            try
            {
                ViewBag.SessionList = objCommon.GetSessionAdmin().ToList().Take(1);
                return View();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        [Route("BookSupply")]
        [Route("BookSupply/Login")]
        [HttpPost]
        public ActionResult Login(LoginModel lm)
        {
            try
            {
                ViewBag.SessionList = objCommon.GetSessionAdmin().ToList().Take(2);
                int OutStatus;
                PM = objDB.BookSupplyLogin(lm, out OutStatus); // PrinterLoginSP
                if (OutStatus > 0)
                {
                    if (OutStatus == 1)
                    {
                        if (PM.Id > 0)
                        {                           
                            HttpContext.Session["BookSupplyType"] =PM.UserType;
                            HttpContext.Session["PrinterName"] =PM.PrinterName;
                            HttpContext.Session["PrinterUserId"] = PM.UserId;
                            HttpContext.Session["PrinterId"] = PM.PrinterId;
                            HttpContext.Session["Session"] = lm.Session;
                            return RedirectToAction("Welcome", "BookSupply");
                        }
                        else
                        {
                            ViewData["result"] = "0";
                        }
                    }
                    else
                    {
                        ViewData["result"] = "2";
                    }
                }
                else
                {
                    ViewData["result"] = "0";
                }
                return View();
            }
            catch (Exception ex)
            {
               // oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("login", "BookSupply");
            }
        }
     

        public ActionResult Logout()
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            FormsAuthentication.SignOut();
            return RedirectToAction("login", "BookSupply");

        }
        #endregion  Login BookSupply

        public ActionResult Welcome(PrinterModel BM)
        {
            try
            {
                if (Session["PrinterUserId"] == null)
                {
                    return RedirectToAction("login", "BookSupply");
                }              
                return View(BM);
            }
            catch (Exception ex)
            {
                //oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }



        #region ViewProfile
        public ActionResult ViewProfile(string id, PrinterModel PM)
        {
            try
            {
                int OutStatus;
                if (Session["PrinterName"] == null)
                {
                    return RedirectToAction("login", "BookSupply");
                }
                else
                {

                    PM = objDB.GetDataByPrinterUserId(id, out OutStatus);                
                    if (PM == null)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {                      
                        ViewBag.TotalCount = 1;
                    }

                }
                return View(PM);
            }
            catch (Exception ex)
            {             
                return View();
            }
        }

        [HttpPost]
        public ActionResult ViewProfile(string id, PrinterModel PM, FormCollection frm)
        {
            try
            {
                int OutStatus;
                if (Session["PrinterName"] == null)
                {
                    return RedirectToAction("login", "BookSupply");
                }
                else
                {                   
                    DataSet ds = objDB.UpdatePrinterDataByUserId(PM, out OutStatus);
                    if (OutStatus == 1)
                    {
                        ViewData["Result"] = "1";                       
                    }
                    else
                    {
                        ViewData["Result"] = "0";                       
                    }
                }
                return View(PM);
            }
            catch (Exception ex)
            {                
                return RedirectToAction("ViewProfile", "BookSupply");
            }
        }

        #endregion ViewProfile


        #region BookPrintMaster
        public ActionResult BookPrintMaster(string id, BookPrintMaster PM)
        {
            try
            {
                ViewBag.Id = id;
                int OutStatus;
                if (Session["PrinterName"] == null || Session["BookSupplyType"].ToString() != "A")
                {
                    return RedirectToAction("login", "BookSupply");
                }
                else
                {
                    if (string.IsNullOrEmpty(id))
                    {
                        ViewBag.Id = 0;
                         DataSet result = objDB.GetBookPrintMaster("0", out OutStatus);
                        if (result == null)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount1 = 0;
                            return View();
                        }
                        else if (result.Tables[0].Rows.Count > 0)
                        {
                            PM.StoreAllData = result;
                            ViewBag.TotalCount = PM.StoreAllData.Tables[0].Rows.Count;
                        }

                    }
                    else
                    {
                        PM = objDB.GetBookPrintMasterByBookId(id, out OutStatus);
                        if (PM == null)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount1 = 0;
                            return View();
                        }
                        else
                        {
                            ViewBag.TotalCount1 = 1;
                        }
                    }

                }
                return View(PM);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult BookPrintMaster(string id, BookPrintMaster PM, FormCollection frm)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    ViewBag.Id = 0;
                }
                else { ViewBag.Id = id; }

                int OutStatus;
                if (Session["PrinterName"] == null || Session["BookSupplyType"].ToString() != "A")
                {
                    return RedirectToAction("login", "BookSupply");
                }
                else
                {
                    PM.Id = Convert.ToInt32(ViewBag.Id);

                    DataSet ds = objDB.BookPrintMaster(PM, out OutStatus);
                    if (OutStatus == 1)
                    {
                        ViewData["Result"] = "1";
                    }
                    else
                    {
                        ViewData["Result"] = "0";
                    }
                }
                return View(PM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("BookPrintMaster", "BookSupply");
            }
        }

        #endregion BookPrintMaster


        #region BooksForPrinting (AssignmentofBooksforprinting)
        public ActionResult BooksForPrinting(string id, AssignmentofBooksforprinting PM)
        {
            try
            {
                int OutStatus1;                
                ViewBag.SelPrinters = objDB.GetPrintMasterList("0", out OutStatus1);
                ViewBag.SelBooks = objDB.GetBookPrintMasterList("0", out OutStatus1);
                ViewBag.SelectedPrinter = 0;
                ViewBag.SelectedBok = 0;

                //Parent
                var itemType = new SelectList(new[] { new { ID = "1", Name = "SSA" },  new { ID = "2", Name = "General Sale" }, }, "ID", "Name", 1);
                ViewBag.selType = itemType.ToList();
                ViewBag.SelectedType = 0;

                ViewBag.Id = id;
                int OutStatus;
                if (Session["PrinterName"] == null)
                {
                    return RedirectToAction("login", "BookSupply");
                }
                else
                {


                    string Search = Search = "a.Id like '%'";
                    if (string.IsNullOrEmpty(id))
                    {
                        ViewBag.Id = 0;
                        if (Session["BookSupplyType"].ToString() == "A")
                        {
                            DataSet result = objDB.GetAssignmentofBooksforprinting(0, "0", out OutStatus, Search);
                            if (result == null)
                            {
                                return View(PM);
                            }
                            else if (result.Tables[0].Rows.Count > 0)
                            {
                                PM.StoreAllData = result;
                                ViewBag.TotalCount = PM.StoreAllData.Tables[0].Rows.Count;
                            }
                        }
                        else if (Session["BookSupplyType"].ToString() == "P")
                        {
                            string printerId = Session["PrinterId"].ToString();
                            DataSet result = objDB.GetAssignmentofBooksforprinting(2, printerId, out OutStatus, Search);
                            if (result == null)
                            {
                                return View(PM);
                            }
                            else if (result.Tables[0].Rows.Count > 0)
                            {
                                PM.StoreAllData = result;
                                ViewBag.TotalCount = PM.StoreAllData.Tables[0].Rows.Count;
                            }
                        }


                    }
                    else
                    {

                        PM = objDB.GetAssignmentofBooksforprintingById(0, id, out OutStatus);
                        if (PM == null)
                        {                           
                            return View();
                        }
                        else
                        {
                            ViewBag.TotalCount1 = 1;
                        }
                    }

                }
                return View(PM);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult BooksForPrinting(string id, AssignmentofBooksforprinting PM, FormCollection frm, string selBook, string selPrinter, string seltype1)
        {
            try
            {

                int OutStatus1;
                ViewBag.SelPrinters = objDB.GetPrintMasterList("0", out OutStatus1);
                ViewBag.SelBooks = objDB.GetBookPrintMasterList("0", out OutStatus1);
                ViewBag.SelectedPrinter = 0;
                ViewBag.SelectedBok = 0;

                //Parent
                var itemType = new SelectList(new[] { new { ID = "1", Name = "SSA" },  new { ID = "2", Name = "General Sale" }, }, "ID", "Name", 1);
                ViewBag.selType = itemType.ToList();
                ViewBag.SelectedType = 0;
                if (string.IsNullOrEmpty(id))
                {
                    ViewBag.Id = 0;
                    PM.Id = 0;                    
                }
                else {
                    ViewBag.Id = id;
                    PM.Id = 1;
                }

                #region Check Total Qty Of Books/No. Of Books To Be Printed  
                int OutStatus2 = 0;
                int BookPrintCount = 0;
                int TotalBookToBePrinted = 0;
                int flag = 0;

               
                DataSet result = objDB.GetAssignmentofBooksforprinting(1,PM.BookId, out OutStatus2,"");
                if (result == null)
                {
                    BookPrintCount = 0;
                    ViewBag.TotalBookToBePrinted = TotalBookToBePrinted = Convert.ToInt32(PM.StoreAllData.Tables[1].Rows[0]["Numberofbookstobeprinted"].ToString());
                }
                else if (result.Tables[0].Rows.Count > 0)
                {
                    PM.StoreAllData = result;
                    ViewBag.TotalBookToBePrinted = TotalBookToBePrinted = Convert.ToInt32(PM.StoreAllData.Tables[1].Rows[0]["Numberofbookstobeprinted"].ToString());

                    if(PM.Id == 0)
                    { ViewBag.BookPrintCount = BookPrintCount = PM.StoreAllData.Tables[0].AsEnumerable().Sum(x => x.Field<int>("QtyofBooksforprinting")); }
                     else{ ViewBag.BookPrintCount = BookPrintCount = PM.StoreAllData.Tables[0].AsEnumerable().Where(x => x.Field<string>("TransId") != PM.TransId).Sum(x => x.Field<int>("QtyofBooksforprinting")); }
                    
                }

                //TotalBookToBePrinted >= (PM.QtyofBooksforprinting + BookPrintCount)
                //50>=(60+0)                    
                if (TotalBookToBePrinted >= (PM.QtyofBooksforprinting + BookPrintCount))
                {
                    flag = 0;
                }
                else
                {
                    flag = 1;
                    ViewData["Result"] = "10";
                    return View(PM);
                }
              
                #endregion Check Total Qty Of Books/No. Of Books To Be Printed  



                if (Session["PrinterName"] == null || Session["BookSupplyType"].ToString() != "A")
                {
                    return RedirectToAction("login", "BookSupply");
                }
                else
                {
                    int OutStatus;                  
                    DataSet ds = objDB.AssignmentofBooksforprinting(PM, out OutStatus);
                    if (OutStatus == 1)
                    {
                        ViewData["Result"] = "1";
                    }
                    else
                    {
                        ViewData["Result"] = "0";
                    }
                }
                return View(PM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("BooksForPrinting", "BookSupply");
            }
        }


        public ActionResult ViewBooksForPrinting(string id, AssignmentofBooksforprinting PM)
        {
            try
            {
                if (Session["PrinterName"] == null)
                {
                    return RedirectToAction("login", "BookSupply");
                }
                int OutStatus1;
                ViewBag.SelPrinters = objDB.GetPrintMasterList("0", out OutStatus1);
                ViewBag.SelBooks = objDB.GetBookPrintMasterList("0", out OutStatus1);
                ViewBag.SelectedPrinter = 0;
                ViewBag.SelectedBok = 0;

                //Parent
                var itemType = new SelectList(new[] { new { ID = "1", Name = "SSA" },  new { ID = "2", Name = "General Sale" }, }, "ID", "Name", 1);
                ViewBag.selType = itemType.ToList();
                ViewBag.SelectedType = 0;

                ViewBag.Id = id;
                int OutStatus;
                if (Session["PrinterName"] == null)
                {
                    return RedirectToAction("login", "BookSupply");
                }
                else
                {

                    string Search =  "a.Id like '%'";
                    
                        ViewBag.Id = 0;
                    if (Session["BookSupplyType"].ToString() == "A")
                    {
                        DataSet result = objDB.GetAssignmentofBooksforprinting(0, "0", out OutStatus, Search);
                        if (result == null)
                        {
                            return View(PM);
                        }
                        else if (result.Tables[0].Rows.Count > 0)
                        {
                            PM.StoreAllData = result;
                            ViewBag.TotalCount = PM.StoreAllData.Tables[0].Rows.Count;
                        }
                    }
                    else if (Session["BookSupplyType"].ToString() == "P")
                    {
                        string printerId = Session["PrinterId"].ToString();
                        ViewBag.SelPrinters = objDB.GetPrintMasterList("0", out OutStatus1).Where(item => item.Value == printerId).ToList();
                        ViewBag.SelectedPrinter = printerId;
                        PM.PrinterId = Convert.ToString(printerId);


                       // List<SelectListItem> _list = new List<SelectListItem>();

                        DataSet result = objDB.GetAssignmentofBooksforprinting(2, printerId, out OutStatus, Search);
                        if (result == null)
                        {
                            return View(PM);
                        }
                        else if (result.Tables[0].Rows.Count > 0)
                        {
                            PM.StoreAllData = result;
                            ViewBag.TotalCount = PM.StoreAllData.Tables[0].Rows.Count;
                            //ViewBag.SelBooks = objDB.GetTransIdList(2, printerId, out OutStatus1);
                        }

                        List<SelectListItem> _list = new List<SelectListItem>();
                        if (PM.StoreAllData.Tables[1].Rows.Count > 0)
                        {
                            foreach (System.Data.DataRow dr in PM.StoreAllData.Tables[1].Rows)
                            {
                                _list.Add(new SelectListItem { Text = @dr["BookName"].ToString(), Value = @dr["BookId"].ToString() });
                            }
                            ViewBag.SelBooks = _list.GroupBy(h => h.Value).Select(g => new SelectListItem { Value = g.Key.ToString(), Text = g.First().Text }).ToList();
                        }
                    }
                }
                return View(PM);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult ViewBooksForPrinting(string id, AssignmentofBooksforprinting PM, FormCollection frm, string selBook, string selPrinter, string seltype1)
        {
            try
            {
                if (Session["PrinterName"] == null)
                {
                    return RedirectToAction("login", "BookSupply");
                }

                int OutStatus1;
                int OutStatus;
                ViewBag.SelPrinters = objDB.GetPrintMasterList("0", out OutStatus1);
                ViewBag.SelBooks = objDB.GetBookPrintMasterList("0", out OutStatus1);
                ViewBag.SelectedPrinter = 0;
                ViewBag.SelectedBok = 0;

                //Parent
                var itemType = new SelectList(new[] { new { ID = "1", Name = "SSA" },  new { ID = "2", Name = "General Sale" }, }, "ID", "Name", 1);
                ViewBag.selType = itemType.ToList();
                ViewBag.SelectedType = 0;
             

                if (Session["PrinterName"] == null)
                {
                    return RedirectToAction("login", "BookSupply");
                }
                else
                {

                    string Search = Search = "a.Id like '%'";
                    if (!string.IsNullOrEmpty(PM.PrinterId))
                    {                       
                        Search += " and a.PrinterId='" + PM.PrinterId.ToString() + "'";
                    }
                    if (!string.IsNullOrEmpty(PM.BookId))
                    {
                        Search += " and a.BookId='" + PM.BookId.ToString()+"'";
                    }
                    if (!string.IsNullOrEmpty(PM.DateStamp))
                    {
                        Search += " and a.DateStamp='" + PM.DateStamp.ToString() + "'";
                    }
                    if (!string.IsNullOrEmpty(PM.Typeofbooks))
                    {
                        Search += " and a.Typeofbooks='" + PM.Typeofbooks.ToString() + "'";
                    }

                  
                        ViewBag.Id = 0;
                    if (Session["BookSupplyType"].ToString() == "A")
                    {                       
                        DataSet result = objDB.GetAssignmentofBooksforprinting(0, "0", out OutStatus, Search);
                        if (result == null)
                        {
                            return View(PM);
                        }
                        else if (result.Tables[0].Rows.Count > 0)
                        {
                            PM.StoreAllData = result;
                            ViewBag.TotalCount = PM.StoreAllData.Tables[0].Rows.Count;
                        }
                    }
                    else if (Session["BookSupplyType"].ToString() == "P")
                    {
                        string printerId = Session["PrinterId"].ToString();
                        ViewBag.SelPrinters = objDB.GetPrintMasterList("0", out OutStatus1).Where(item => item.Value == printerId).ToList();
                        ViewBag.SelectedPrinter = printerId;
                        PM.PrinterId = Convert.ToString(printerId);

                        DataSet result = objDB.GetAssignmentofBooksforprinting(2, printerId, out OutStatus, Search);
                        if (result == null)
                        {
                            return View(PM);
                        }
                        else if (result.Tables[0].Rows.Count > 0)
                        {
                            PM.StoreAllData = result;
                            ViewBag.TotalCount = PM.StoreAllData.Tables[0].Rows.Count;
                        }

                        List<SelectListItem> _list = new List<SelectListItem>();
                        if (result.Tables[1].Rows.Count > 0)
                        {
                            foreach (System.Data.DataRow dr in result.Tables[1].Rows)
                            {
                                _list.Add(new SelectListItem { Text = @dr["BookName"].ToString(), Value = @dr["BookId"].ToString() });
                            }
                            ViewBag.SelBooks = _list.GroupBy(h => h.Value).Select(g => new SelectListItem { Value = g.Key.ToString(), Text = g.First().Text }).ToList();
                        }

                        //DataSet result = objDB.GetAssignmentofBooksforprinting(2, printerId, out OutStatus, Search);
                        //if (result == null)
                        //{
                        //    return View(PM);
                        //}
                        //else if (result.Tables[0].Rows.Count > 0)
                        //{
                        //    PM.StoreAllData = result;
                        //    ViewBag.TotalCount = PM.StoreAllData.Tables[0].Rows.Count;
                        //}
                    }
                }
                return View(PM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("ViewBooksForPrinting", "BookSupply");
            }
        }

        #endregion BooksForPrinting


        #region SupplyofBooks

        public JsonResult BookSupplySummaryByTransId(string PrinterId, string TransId)
        {
            int OutStatus = 0;          
            string TotalDemand = "";
            string TotalPending = "0";
            string TotalSuppied = "";            
            if (!string.IsNullOrEmpty(TransId))
            {
                DataSet ds = objDB.BookSupplySummaryByTransId(1, PrinterId, TransId, out OutStatus);
                if (OutStatus > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        OutStatus = 1;
                        ViewBag.TotalDemand = TotalDemand = ds.Tables[0].Rows[0]["totalDemand"].ToString();
                        ViewBag.TotalPending = TotalPending = ds.Tables[0].Rows[0]["TotalPending"].ToString();
                        ViewBag.TotalSuppied= TotalSuppied = ds.Tables[0].Rows[0]["TotalSuppied"].ToString();
                     }
                   
                }
            }
            return Json(new { tid = TransId, td = TotalDemand, tp = TotalPending, ts = TotalSuppied, oid = OutStatus }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SupplyofBooks(string id, SupplyofBooks PM)
        {
            try
            {               

                int OutStatus1;
                ViewBag.SelDepots = objDB.GetDepotList("0");
                ViewBag.SelectedDepot = 0;
                //Parent
                var itemType = new SelectList(new[] { new { ID = "1", Name = "Partial" }, new { ID = "2", Name = "Full" },}, "ID", "Name", 1);
                ViewBag.selType = itemType.ToList();
                ViewBag.SelectedType = 0;


                ViewBag.Id = id;
                int OutStatus;
                if (Session["PrinterName"] == null || Session["BookSupplyType"].ToString() != "P")
                {
                    return RedirectToAction("login", "BookSupply");
                }
                else
                {
                    string printerId = Session["PrinterId"].ToString();
                    ViewBag.SelPrinters = objDB.GetPrintMasterList("0", out OutStatus1).Where(item => item.Value == printerId).ToList();
                    ViewBag.SelectedPrinter = printerId;
                    PM.PrinterId = Convert.ToString(printerId);
                    ViewBag.SelTrans = objDB.GetTransIdList(2, printerId, out OutStatus1);
                    ViewBag.SelectedTran = 0;

                    string Search = "a.SupplyId like '%'";
                    if (string.IsNullOrEmpty(id))
                    {
                        ViewBag.Id = 0;
                        ViewBag.Finalcount = 0;
                        DataSet result = objDB.GetSupplyofBooks(0,"0", out OutStatus,Search);
                        if (result == null)
                        {
                              return RedirectToAction("login", "BookSupply");
                        }
                        else if (result.Tables[0].Rows.Count > 0)
                        {
                            PM.StoreAllData = result;
                            ViewBag.TotalCount = PM.StoreAllData.Tables[0].Rows.Count;

                            if (result.Tables.Count > 1)
                            {
                                if (result.Tables[1].Rows.Count > 0)
                                {
                                    ViewBag.Finalcount = PM.StoreAllData.Tables[1].Rows.Count;
                                    ViewBag.DepotUserId = PM.StoreAllData.Tables[1].Rows[0]["DepotUserId"].ToString();
                                    ViewBag.SelDepots = objDB.GetDepotList("0").Where(item => item.Value.ToLower().Trim() == ViewBag.DepotUserId.ToLower().Trim()).ToList();
                                }
                            }
                        }
                    }
                    else
                    {
                        PM = objDB.GetSupplyofBooksById(0,id, out OutStatus);
                        if (PM == null)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                            return View();
                        }
                        else
                        {
                            ViewBag.TotalCount1 = 1;
                        }
                    }

                }
                return View(PM);
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        [HttpPost]
        public ActionResult SupplyofBooks(string id, SupplyofBooks PM, FormCollection frm, string selDepot, string selTran, string selPrinter,string seltype1)
        {
            try
            {
                if (Session["PrinterName"] == null || Session["BookSupplyType"].ToString() != "P")
                {
                    return RedirectToAction("login", "BookSupply");
                }
                
                int OutStatus1;
                ViewBag.SelDepots = objDB.GetDepotList("0");
                ViewBag.SelectedDepot = 0;


                string printerId = Session["PrinterId"].ToString();

                ViewBag.SelPrinters = objDB.GetPrintMasterList("0", out OutStatus1).Where(item => item.Value == printerId).ToList();
                ViewBag.SelectedPrinter = printerId;
                PM.PrinterId = Convert.ToString(printerId);

                ViewBag.SelTrans = objDB.GetTransIdList(2, printerId, out OutStatus1);
                ViewBag.SelectedTran = 0;

                //Parent
                var itemType = new SelectList(new[] { new { ID = "1", Name = "Partial" }, new { ID = "2", Name = "Full" }, }, "ID", "Name", 1);
                ViewBag.selType = itemType.ToList();
                ViewBag.SelectedType = 0;

                if (string.IsNullOrEmpty(id))
                {
                    ViewBag.Id = 0;
                }
                else { ViewBag.Id = id; }

                #region Check No. Of Books To Be Printed/NumberofSuppliedbooks
                int OutStatus2 = 0;
                int NumberofSuppliedbooksCount = 0;
                int QtyofBooksforprintingCount = 0;
                int flag = 0;
                DataSet result = objDB.GetSupplyofBooks(1, PM.TransId, out OutStatus2,"");
                if (result == null)
                {
                    NumberofSuppliedbooksCount = 0;
                    ViewBag.QtyofBooksforprinting = QtyofBooksforprintingCount = Convert.ToInt32(PM.StoreAllData.Tables[1].Rows[0]["QtyofBooksforprinting"].ToString());
                }
                else if (result.Tables[0].Rows.Count > 0)
                {
                    PM.StoreAllData = result;
                    ViewBag.QtyofBooksforprinting = QtyofBooksforprintingCount = Convert.ToInt32(PM.StoreAllData.Tables[1].Rows[0]["QtyofBooksforprinting"].ToString());

                    if (PM.SupplyId == 0)
                    {
                        ViewBag.NumberofSuppliedbooksCount = NumberofSuppliedbooksCount = PM.StoreAllData.Tables[0].AsEnumerable().Sum(x => x.Field<int>("NumberofSuppliedbooks"));
                    }
                    else
                    {

                        ViewBag.NumberofSuppliedbooksCount = NumberofSuppliedbooksCount = PM.StoreAllData.Tables[0].AsEnumerable().Where(x => x.Field<int>("SupplyId") != PM.SupplyId).Sum(x => x.Field<int>("NumberofSuppliedbooks"));
                    }
                }

                //TotalBookToBePrinted >= (PM.QtyofBooksforprinting + BookPrintCount)
                //50>=(60+0)
               
                if (QtyofBooksforprintingCount >= (PM.NumberofSuppliedbooks + NumberofSuppliedbooksCount))
                {
                    flag = 0;
                }
                else
                {
                    flag = 1;
                    ViewData["Result"] = "10";
                    return View(PM);
                }

                #endregion No. Of Books To Be Printed/NumberofSuppliedbooks


                int OutStatus;
                if (Session["PrinterName"] == null || Session["BookSupplyType"].ToString() != "P")
                {
                    return RedirectToAction("login", "BookSupply");
                }
                else
                {
                    PM.SupplyId = Convert.ToInt32(ViewBag.Id);
                    DataSet ds = objDB.SupplyofBooks(PM, out OutStatus);
                    if (OutStatus == 1)
                    {
                        ViewData["Result"] = "1";
                    }
                    else
                    {
                        ViewData["Result"] = "0";
                    }
                }
                return View(PM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("SupplyofBooks", "BookSupply");
            }
        }




        public ActionResult SupplyofBooksFinalSubmit(string id)
        {
            if (Session["PrinterName"] == null || Session["BookSupplyType"].ToString() != "P")
            {
                return RedirectToAction("login", "BookSupply");
            }

            int OutStatus = 0;
            if (!string.IsNullOrEmpty(id))
            {             
                DataSet result = objDB.SupplyofBooksFinalSubmit(id, out OutStatus);
                if (OutStatus > 0)
                {
                    Session["isSupplyofBooksFinalSubmit"] = "1";
                }
                else
                { Session["isSupplyofBooksFinalSubmit"] = "0"; }
            }
            return RedirectToAction("SupplyofBooks");
        }



        public ActionResult ViewSupplyofBooks(string id)
        {

            if (Session["PrinterName"] == null || Session["BookSupplyType"].ToString() != "P")
            {
                return RedirectToAction("login", "BookSupply");
            }
            int OutStatus = 0;
            string printerId = Session["PrinterId"].ToString();
            int supplylot = 0;

            if (string.IsNullOrEmpty(id))
            {

                ViewBag.Supplylot = supplylot = 0;
                DataSet result = objDB.ViewSupplyofBooks(1, printerId, supplylot, out OutStatus);
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
                DataSet result = objDB.ViewSupplyofBooks(2, printerId, supplylot, out OutStatus);
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


        public ActionResult DeleteSupplyofBooks(string id)
        {
            if (Session["PrinterName"] == null || Session["BookSupplyType"].ToString() != "P")
            {
                return RedirectToAction("login", "BookSupply");
            }

            int OutStatus = 0;
            int supplyId = 0;
            if (!string.IsNullOrEmpty(id))
            {
                supplyId = Convert.ToInt32(id);
                DataSet result = objDB.ActionBySupplyId(0, supplyId, out OutStatus);
            }
            return RedirectToAction("SupplyofBooks");
        }



        #endregion SupplyofBooks

        public ActionResult ChangePassword(PrinterModel PM)
        {          
            try
            {
                if (Session["PrinterName"] == null)
                {
                      return RedirectToAction("login", "BookSupply");
                }
                PM.UserId = Session["PrinterUserId"].ToString();
                PM.PrinterName = Session["PrinterName"].ToString();
                return View(PM);
            }
            catch (Exception)
            {
                  return RedirectToAction("login", "BookSupply");
            }
        }
        [HttpPost]
        public ActionResult ChangePassword(PrinterModel PM, FormCollection frc)
        {          
            try
            {
                if (Session["PrinterName"] == null)
                {
                      return RedirectToAction("login", "BookSupply");
                }
                PM.UserId = frc["UserId"].ToString();
                PM.OldPassword = frc["OldPassword"].ToString();
                PM.Newpassword = frc["Newpassword"].ToString();
                int OutStatus = 0;
                DataSet result = objDB.ChangePasswordPrinter(PM, out OutStatus);
                if (OutStatus >= 1)
                {
                    ViewData["Status"] = "1";                   
                }
                else
                {
                    ViewData["Status"] = "0";                   
                }
                return View(PM);
            }
            catch (Exception)
            {
                  return RedirectToAction("login", "BookSupply");
            }


        }


    }
}