using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using CCA.Util;
using ClosedXML.Excel;
using PSEBONLINE.Filters;
using PSEBONLINE.Models;
using PsebPrimaryMiddle.Controllers;

namespace PSEBONLINE.Controllers
{
   
    public class OnDemandCertificateController : Controller
    {
        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        private DBContext _context = new DBContext();
        // GET: OnDemandCertificate


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
                    DataSet result = new AbstractLayer.DBClass().GetAdminDetailsById(Convert.ToInt32(Session["AdminId"]), Convert.ToInt32(Session["Session"].ToString().Substring(0, 4)));
                    if (result.Tables[2].Rows.Count > 0)
                    {
                        bool exists = true;
                        DataSet dsIsExists = new AbstractLayer.DBClass().GetActionOfSubMenu(0, controllerName, actionName);
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


        #region  Ondemand Certificate for School

        [SessionCheckFilter]
        public ActionResult ViewStudentList(string id, OnDemandCertificateModelList onDemandCertificateSearchModel)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("OnDemandCertificate", "RegistrationPortal");
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
                onDemandCertificateSearchModel.OnDemandCertificateSearchModel = AbstractLayer.OnDemandCertificateDB.GetOnDemandCertificateStudentList("GET",RP, cls, SCHL, Search, out dsOut);
                onDemandCertificateSearchModel.StoreAllData = dsOut;
                return View(onDemandCertificateSearchModel);       
            }
            catch (Exception ex)
            {               
                return View(id);
            }
        }

        [SessionCheckFilter]
        public JsonResult JqOnDemandCertificateApplyStudents(string studentlist,string cls)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
          
            int result = 0;
            if (!string.IsNullOrEmpty(studentlist))
            {
                studentlist = studentlist.Remove(studentlist.Length - 1);


                List<int> listComma = studentlist.Split(',').Select(int.Parse).ToList();
                List<OnDemandCertificates> list = new List<OnDemandCertificates>();

                foreach (var stdid in listComma)
                {
                    list.Add(new OnDemandCertificates { DemandId=0, Std_id = stdid, Schl = loginSession.SCHL, Cls = Convert.ToInt32(cls),IsActive=1,IsPrinted=0,SubmitOn=DateTime.Now,SubmitBy="SCHL" });
                }

                if (list.Count() > 0)
                {
                    result = new AbstractLayer.OnDemandCertificateDB().InsertOnDemandCertificateStudentList(list);
                }
                else
                {
                    result = -1;
                }
            }
                return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }

        [SessionCheckFilter]
        public ActionResult OnDemandCertificateAppliedStudentList(string id, OnDemandCertificateModelList onDemandCertificateSearchModel)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return RedirectToAction("OnDemandCertificate", "RegistrationPortal");
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
                onDemandCertificateSearchModel.OnDemandCertificateSearchModel = AbstractLayer.OnDemandCertificateDB.GetOnDemandCertificateStudentList("ADDED", RP, cls, SCHL, Search, out dsOut);
                onDemandCertificateSearchModel.StoreAllData = dsOut;
                return View(onDemandCertificateSearchModel);
            }
            catch (Exception ex)
            {
                return View(id);
            }
        }

        [SessionCheckFilter]
        public JsonResult JqRemoveOnDemandCertificateApplyStudents(string demandIdList, string cls)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];

            int result = 0;
            if (!string.IsNullOrEmpty(demandIdList))
            {
                demandIdList = demandIdList.Remove(demandIdList.Length - 1);


                List<int> listComma = demandIdList.Split(',').Select(int.Parse).ToList();
                List<OnDemandCertificates> list = new List<OnDemandCertificates>();
          
                foreach (var did in listComma)
                {
                    list.Add(new OnDemandCertificates { DemandId = did });
                }

                if (list.Count() > 0)
                {
                    result = new AbstractLayer.OnDemandCertificateDB().RemoveRangeOnDemandCertificateStudentList(list);
                }
                else
                {
                    result = -1;
                }
            }
            return Json(new { Result = result }, JsonRequestBehavior.AllowGet);
        }

        [SessionCheckFilter]
        public ActionResult OnDemandCertificate_ChallanList(OnDemandCertificate_ChallanDetailsViewsModelList obj)
        {
            try
            {
                LoginSession loginSession = (LoginSession)Session["LoginSession"];               
                DataSet dsOut = new DataSet();
                obj.OnDemandCertificate_ChallanDetailsViews = AbstractLayer.OnDemandCertificateDB.OnDemandCertificate_ChallanList(loginSession.SCHL, out dsOut);
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
        public ActionResult OnDemandCertificateCalculateFee(string id, string D)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];

            try
            {
                Session["OnDemandCertificateCalculateFee"] = null;
                FeeHomeViewModel fhvm = new FeeHomeViewModel();

                string Search = string.Empty;
                Search = "SCHL=" + loginSession.SCHL.ToString();
                DataSet ds = new DataSet();
                ds = AbstractLayer.OnDemandCertificateDB.OnDemandCertificatesCountRecordsClassWise(Search, loginSession.SCHL.ToString());
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
                    fhvm.StoreAllData = AbstractLayer.OnDemandCertificateDB.OnDemandCertificateCalculateFee(id, date, Search, Session["SCHL"].ToString());

                    if (fhvm.StoreAllData == null || fhvm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        ViewData["FeeStatus"] = "3";
                    }
                    else
                    {
                        ViewData["FeeStatus"] = "1";
                        if (Session["OnDemandCertificateCalculateFee"] != null)
                        {
                            Session["OnDemandCertificateCalculateFee"] = null;
                        }

                        Session["OnDemandCertificateCalculateFee"] = fhvm.StoreAllData;
                        ViewBag.TotalCount = fhvm.StoreAllData.Tables[0].Rows.Count;
                        fhvm.TotalFeesInWords = fhvm.StoreAllData.Tables[1].Rows[0]["TotalFeesInWords"].ToString();
                        fhvm.EndDate = fhvm.StoreAllData.Tables[0].Rows[0]["EndDateDay"].ToString() + " " + fhvm.StoreAllData.Tables[0].Rows[0]["FeeValidDate"].ToString();
                    }
                }
                else {
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
        public ActionResult OnDemandCertificatePaymentForm()
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];

            try
            {

                PaymentformViewModel pfvm = new PaymentformViewModel();
                if (Session["OnDemandCertificateCalculateFee"] == null || Session["OnDemandCertificateCalculateFee"].ToString() == "")
                {
                    return RedirectToAction("OnDemandCertificateCalculateFee", " OnDemandCertificate");
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

                DataSet dscalFee = (DataSet)Session["OnDemandCertificateCalculateFee"];
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
                Session["OnDemandCertificatePaymentForm"] = pfvm;

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
                    TempData["OnDemandCertificateCheckFormFee"] = 0;
                }
                else
                {
                    ViewBag.CheckForm = 0; // only verify for M1 and T1 
                   TempData["OnDemandCertificateCheckFormFee"] = 1;
                }
                return View(pfvm);

            }
            catch (Exception ex)
            {
                return RedirectToAction("OnDemandCertificateCalculateFee", " OnDemandCertificate");
            }
        }


        [SessionCheckFilter]
        [HttpPost]
        public ActionResult OnDemandCertificatePaymentForm(PaymentformViewModel pfvm, FormCollection frm, string PayModValue, string AllowBanks, string IsOnline)
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
                //if (Session["OnDemandCertificateCheckFormFee"].ToString() == "0")
                //{ pfvm.BankCode = "203"; }


                if (Session["OnDemandCertificatePaymentForm"] == null || Session["OnDemandCertificatePaymentForm"].ToString() == "")
                {
                    return RedirectToAction("OnDemandCertificatePaymentForm", "OnDemandCertificate");
                }
                if (Session["OnDemandCertificate_FeeStudentList"] == null || Session["OnDemandCertificate_FeeStudentList"].ToString() == "")
                {
                    return RedirectToAction("OnDemandCertificatePaymentForm", "OnDemandCertificate");
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
                    string FeeStudentList = Session["OnDemandCertificate_FeeStudentList"].ToString();
                    CM.FeeStudentList = FeeStudentList.Remove(FeeStudentList.LastIndexOf(","), 1);
                    PaymentformViewModel PFVMSession = (PaymentformViewModel)Session["OnDemandCertificatePaymentForm"];
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
                            return RedirectToAction("OnDemandCertificatePaymentForm", "OnDemandCertificate");
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
            return RedirectToAction("OnDemandCertificatePaymentForm", "OnDemandCertificate");
        }

        #endregion


        #endregion


      


        #region  OnDemand Certificate Admin Panel

        [AdminLoginCheckFilter]
        public ActionResult Welcome()
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];



            #region Action Assign Method
            if (adminLoginSession.AdminType.ToUpper() == "ADMIN")
            { ViewBag.IsVIEW = 1; ViewBag.IsDOWNLOAD = 1; ViewBag.IsPREVIOUS = 1; ViewBag.IsDISPATCH = 1;  }
            else
            {

                string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
                string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
                //string AdminType = Session["AdminType"].ToString();
                //GetActionOfSubMenu(string cont, string act)
                DataSet aAct = new AbstractLayer.DBClass().GetActionOfSubMenu(adminLoginSession.AdminId, controllerName, actionName);
                if (aAct.Tables[0].Rows.Count > 0)
                {
                    ViewBag.IsVIEW = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("Action").ToUpper().Equals("ONDEMANDCERTIFICATEVERIFIEDSTUDENTLIST")).Count();
                    ViewBag.IsDOWNLOAD = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("Action").ToUpper().Equals("ONDEMANDCERTIFICATEDOWNLOADDATA")).Count();
                    ViewBag.IsPREVIOUS = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("Action").ToUpper().Equals("ONDEMANDCERTIFICATEDOWNLOADPREVIOUSDATA")).Count();
                    ViewBag.IsDISPATCH = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("Action").ToUpper().Equals("REGISTRYMIS".ToUpper())).Count();
                    
                   
                }
            }

            #endregion Action Assign Method

            return View();
        }


        #region Verified Student List

        [AdminLoginCheckFilter]
        public ActionResult OnDemandCertificateVerifiedStudentList(OnDemandCertificatesVerifiedStudentCompleteDetailsViewsModelList onDemandCertificateSearchModel)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            try
            {
                var itemsch = new SelectList(new[]{new {ID="1",Name="School Code"},new{ID="2",Name="Unique ID"},new{ID="3",Name="Candidate Name"},
                new{ID="4",Name="Roll Number"},new{ID="7",Name="Reg No"}}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                ViewBag.SelectedItem = 0;

                //Search,               
                DataSet dsOut = new DataSet();
                onDemandCertificateSearchModel.OnDemandCertificatesVerifiedStudentCompleteDetailsViewsSearchModel = null;
                onDemandCertificateSearchModel.StoreAllData = dsOut;
            }
            catch (Exception ex)
            {
            }
            return View(onDemandCertificateSearchModel);
        }


        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult OnDemandCertificateVerifiedStudentList(OnDemandCertificatesVerifiedStudentCompleteDetailsViewsModelList onDemandCertificateSearchModel, FormCollection fc, string cmd, string FromDate, string ToDate, string SelList, string SearchString)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            var itemsch = new SelectList(new[]{new {ID="1",Name="School Code"},new{ID="2",Name="Unique ID"},new{ID="3",Name="Candidate Name"},
                new{ID="4",Name="Roll Number"},new{ID="7",Name="Reg No"}}, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = 0;

            try
            {

                //Search,
                string Search = "Demandid is not null";
                // Search += "form_Name='E1' and schl='" + schlid + "' and a.std_id like '%' ";

                if (!string.IsNullOrEmpty(FromDate))
                {
                    ViewBag.FromDate = FromDate;
                    TempData["FromDate"] = FromDate;
                    Search += " and CONVERT(DATETIME, CONVERT(varchar(10),b.SubmitOn,103), 103)>=CONVERT(DATETIME, CONVERT(varchar(10),'" + FromDate.ToString() + "',103), 103)";
                }

                if (!string.IsNullOrEmpty(ToDate))
                {
                    ViewBag.ToDate = ToDate;
                    TempData["ToDate"] = ToDate;
                    Search += " and CONVERT(DATETIME, CONVERT(varchar(10),b.SubmitOn,103), 103)<=CONVERT(DATETIME, CONVERT(varchar(10),'" + ToDate.ToString() + "',103), 103)";

                }


                if (!string.IsNullOrEmpty(SelList))
                {
                    ViewBag.SelectedItem = SelList;
                    int SelValueSch = Convert.ToInt32(SelList.ToString());


                    if (!string.IsNullOrEmpty(SearchString))
                    {
                        if (SelValueSch == 1)
                        { Search += " and b.SCHL='" + SearchString.ToString() + "'"; }
                        else if (SelValueSch == 2)
                        { Search += " and b.Std_id='" + SearchString.ToString() + "'"; }
                        else if (SelValueSch == 3)
                        { Search += " and b.name like '%" + SearchString.ToString() + "%'"; }
                        else if (SelValueSch == 4)
                        { Search += " and b.Roll='" + SearchString.ToString() + "'"; }
                        else if (SelValueSch == 7)
                        { Search += " and b.regno='" + SearchString.ToString() + "'"; }
                    }

                }

                DataSet dsOut = new DataSet();
                onDemandCertificateSearchModel.OnDemandCertificatesVerifiedStudentCompleteDetailsViewsSearchModel = AbstractLayer.OnDemandCertificateDB.GetCompleteOnDemandCertificateStudentList("GET", Search, out dsOut);
                onDemandCertificateSearchModel.StoreAllData = dsOut;

            }
            catch (Exception ex)
            {

            }
            return View(onDemandCertificateSearchModel);
        }

        #endregion


        #region DownloadData
        [AdminLoginCheckFilter]
        public ActionResult OnDemandCertificateDownloadData(OnDemandCertificates BM)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
        
            try
            {
                int OutStatus;
                string SelType = "SEARCH";
                string Search = "Demandid is not null ";
                DataSet ds = AbstractLayer.OnDemandCertificateDB.OnDemandCertificateDownloadData(SelType, adminLoginSession.AdminEmployeeUserId,adminLoginSession.USER, 0, Search, out OutStatus);
                BM.StoreAllData = ds;
                if (ds == null || OutStatus == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.CurrentDownloadedLot = ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.TotalCount = ds.Tables[0].Rows.Count;
                        //  ViewBag.CurrentDownloadedLot = ds.Tables[1].Rows[0]["TotalDownloadedLot"].ToString();
                        ViewBag.CurrentDownloadedLot = ds.Tables[2].Rows[0]["LotNew"].ToString();  // harpal sir 
                    }
                    else if (ds.Tables[1].Rows.Count > 0)
                    {
                        ViewBag.CurrentDownloadedLot = ViewBag.TotalCount = 0;
                        ViewBag.Message = "Record Not Found";
                        // ViewBag.CurrentDownloadedLot = ds.Tables[1].Rows[0]["TotalDownloadedLot"].ToString();
                    }
                    else
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.CurrentDownloadedLot = ViewBag.TotalCount = 0;
                    }
                }
              
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));              
              
            }

            return View(BM);
        }


        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult OnDemandCertificateDownloadData(OnDemandCertificates BM,FormCollection fc,string cmd)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];    
            try
            {
                int OutStatus;
             
                string Search = "Demandid is not null ";

                string SelType = "GENERATE";
                int DOWNLOADLOT = 0;
                if (!string.IsNullOrEmpty(cmd))
                {
                    if (cmd.ToLower().Contains("excel"))
                    {
                        //DOWNLOADLOT = 3;
                        SelType = "GENERATE";
                    }
                }

                DataSet ds = AbstractLayer.OnDemandCertificateDB.OnDemandCertificateDownloadData(SelType, adminLoginSession.AdminEmployeeUserId,adminLoginSession.USER, DOWNLOADLOT, Search,out OutStatus);
                BM.StoreAllData = ds;
                if (ds == null || OutStatus == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.CurrentDownloadedLot = ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (ds.Tables[0].Columns.Contains("CertNo"))
                        { ds.Tables[0].Columns.Remove("CertNo"); }
                        if (ds.Tables[0].Columns.Contains("RegistryON"))
                        { ds.Tables[0].Columns.Remove("RegistryON"); }
                        if (ds.Tables[0].Columns.Contains("DOWNLOADON"))
                        { ds.Tables[0].Columns.Remove("DOWNLOADON"); }
                        if (ds.Tables[0].Columns.Contains("DOWNLOADLOT"))
                        { ds.Tables[0].Columns.Remove("DOWNLOADLOT"); }
                        if (ds.Tables[0].Columns.Contains("RegistryNumber"))
                        { ds.Tables[0].Columns.Remove("RegistryNumber"); }
                        //
                        string LotNew = ds.Tables[1].Rows[0]["LotNew"].ToString();
                       // string LotNew = "1";
                        bool ResultDownload;
                        string FileExport = "Excel";

                        try
                        {
                            switch (FileExport)
                            {
                                case "Excel":
                                    string fileName1 = "OnDemandCertificate" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_LOT" + LotNew + ".xls";  //103_230820162209_347
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
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
             
            }
            return RedirectToAction("OnDemandCertificateDownloadData", "OnDemandCertificate");
        }


        [AdminLoginCheckFilter]
        public ActionResult OnDemandCertificateDownloadPreviousData(OnDemandCertificates BM)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

            try
            {
                     int OutStatus;
           
                    TempData["DownloadPreviousData"] = null;
                    int prevLot = 1;
                    string Search = "Demandid is not null";
                    DataSet ds = AbstractLayer.OnDemandCertificateDB.OnDemandCertificateDownloadData("PREV", adminLoginSession.AdminEmployeeUserId,adminLoginSession.USER, prevLot, Search, out OutStatus);

                    BM.StoreAllData = ds;
                    List<SelectListItem> itemLotList = new List<SelectListItem>();
                    itemLotList.Add(new SelectListItem { Text = "--Select Lot--", Value = "0" });
                    ViewBag.LotList = itemLotList;

                    if (ds == null || OutStatus == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.CurrentDownloadedLot = ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        // ViewBag.TotalCount = ds.Tables[0].Rows.Count;

                        if (ds.Tables[3].Rows.Count > 0)
                        {
                            ViewBag.TotalCount = 0;
                            //  ViewBag.TotalCount = ds.Tables[0].Rows.Count;
                            ViewBag.CurrentDownloadedLot = ds.Tables[3].Rows.Count;
                            foreach (System.Data.DataRow dr in ds.Tables[3].Rows)
                            {
                                itemLotList.Add(new SelectListItem { Text = @dr["DOWNLDLOT"].ToString(), Value = @dr["DOWNLDLOT"].ToString() });
                            }
                            ViewBag.LotList = itemLotList;
                            ViewBag.Message = "Record Not Found";
                        }
                        else
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount1 = 0;
                        }
                    }
              
                return View(BM);
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
        public ActionResult OnDemandCertificateDownloadPreviousData(OnDemandCertificates BM, FormCollection frm, string cmd)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

            try
            {
                int OutStatus;

                TempData["DownloadPreviousData"] = null;
                int prevLot = 1;
               // DataSet ds = AbstractLayer.OnDemandCertificateDB.OnDemandCertificateDownloadData("PREV", adminLoginSession.AdminEmployeeUserId,adminLoginSession.USER, prevLot, out OutStatus);

                string[] lotsearch = frm["LOT"].ToString().Split('-');
                prevLot = Convert.ToInt32(lotsearch[0].ToString());
                TempData["DownloadPreviousData"] = prevLot;

                string Search = "Demandid is not null";
                DataSet ds = AbstractLayer.OnDemandCertificateDB.OnDemandCertificateDownloadData("PREV", adminLoginSession.AdminEmployeeUserId,adminLoginSession.USER, prevLot, Search, out OutStatus);
                BM.StoreAllData = ds;
                List<SelectListItem> itemLotList = new List<SelectListItem>();
                itemLotList.Add(new SelectListItem { Text = "--Select Lot--", Value = "0" });
                ViewBag.LotList = itemLotList;

                if (ds == null || OutStatus == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.CurrentDownloadedLot = ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {                    
                    if (ds.Tables[3].Rows.Count > 0)
                    {
                        ViewBag.TotalCount = ds.Tables[0].Rows.Count;
                        //  ViewBag.TotalCount = ds.Tables[0].Rows.Count;
                        ViewBag.CurrentDownloadedLot = ds.Tables[3].Rows.Count;
                        foreach (System.Data.DataRow dr in ds.Tables[3].Rows)
                        {
                            itemLotList.Add(new SelectListItem { Text = @dr["DOWNLDLOT"].ToString(), Value = @dr["DOWNLDLOT"].ToString() });
                        }
                        ViewBag.LotList = itemLotList;
                        ViewBag.Message = "Record Not Found";
                    }
                    else
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount1 = 0;
                    }
                }

                return View(BM);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("DownloadPreviousData", "OnDemandCertificate");
            }
        }



       
        [AdminLoginCheckFilter]
        public ActionResult PreviousExportData(OnDemandCertificates BM, FormCollection frm)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

            try
            {
                if (Request.QueryString["File"] == null)
                {
                    return RedirectToAction("Welcome", "OnDemandCertificate");
                }
                else
                {
                    string FileExport = Request.QueryString["File"].ToString();                  
                    DataSet ds = null;
                    int OutStatus;
                    string DownloadPreviousData =  TempData["DownloadPreviousData"].ToString();

                    int prevlot = Convert.ToInt32(DownloadPreviousData);
                    string Search = "Demandid is not null";
                    ds = AbstractLayer.OnDemandCertificateDB.OnDemandCertificateDownloadData("PREV", adminLoginSession.AdminEmployeeUserId,adminLoginSession.USER, prevlot, Search,out OutStatus);

                    if (OutStatus == 1 && ds.Tables[0].Rows.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            if (ds.Tables[0].Columns.Contains("CertNo"))
                            { ds.Tables[0].Columns.Remove("CertNo"); }
                            if (ds.Tables[0].Columns.Contains("RegistryON"))
                            { ds.Tables[0].Columns.Remove("RegistryON"); }
                            if (ds.Tables[0].Columns.Contains("DOWNLOADON"))
                            { ds.Tables[0].Columns.Remove("DOWNLOADON"); }
                            if (ds.Tables[0].Columns.Contains("DOWNLOADLOT"))
                            { ds.Tables[0].Columns.Remove("DOWNLOADLOT"); }
                            if (ds.Tables[0].Columns.Contains("RegistryNumber"))
                            { ds.Tables[0].Columns.Remove("RegistryNumber"); }
                            //
                            string LotNew = prevlot.ToString();
                            // string LotNew = "1";
                            bool ResultDownload;
                            
                            try
                            {
                                switch (FileExport)
                                {
                                    case "Excel":
                                        string fileName1 = "OnDemandCertificate" + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + "_LOT" + LotNew + ".xls";  //103_230820162209_347
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

                return RedirectToAction("Welcome", "OnDemandCertificate");
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("OnDemandCertificateDownloadPreviousChallan", "OnDemandCertificate");
            }

        }

        #endregion DownloadData


        #region Verify Registry 
        [AdminLoginCheckFilter]
        public ActionResult RegistryMIS()
        {            
            return View();
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult RegistryMIS(OnDemandCertificates BM) // HttpPostedFileBase file
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

            try
            {               
                    string fileLocation = "";
                    string filename = "";
                 
                    if (BM.file != null)
                    {
                        filename = Path.GetFileName(BM.file.FileName);
                    }
                    DataSet ds = new DataSet();
                    if (BM.file.ContentLength > 0)  //(Request.Files["file"].ContentLength > 0
                    {
                        string fileName1 = "RegistryMIS_" + adminLoginSession.AdminEmployeeUserId + '_' + DateTime.Now.ToString("ddMMyyyyHHmm");  //MIS_201_110720161210
                        string fileExtension = System.IO.Path.GetExtension(BM.file.FileName);
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
                        BM.file.SaveAs(fileLocation);
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

                        if (ds.Tables[0].Rows.Count > 5000)
                        {
                            ViewData["Result"] = "6";
                            ViewBag.Message = "Please Upload less than 5000 Challans";
                            return View();
                        }

                        string[] arrayChln = ds.Tables[0].Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray();
                        bool CheckChln = AbstractLayer.StaticDB.CheckArrayDuplicates(arrayChln);
                        if (CheckChln == true)
                        {
                            ViewData["Result"] = "11";
                            ViewBag.Message = "Duplicate Challan Number";
                            return View();
                        }




                        DataTable dtexport = new DataTable();
                        string CheckMis = new  AbstractLayer.OnDemandCertificateDB().CheckRegistryMISExcelExport(ds, out dtexport);//CheckMisExcelExport
                        if (CheckMis == "")
                        {                           
                            if (ds.Tables.Count > 0)
                            {
                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    DataTable dt1 = ds.Tables[0];
                                    if (dt1.Columns.Contains("STATUS"))
                                    {
                                        dt1.Columns.Remove("STATUS");
                                    }
                                    dt1.AcceptChanges();
                                   
                                    int OutStatus = 0;
                                    // DataTable dtResult = objDM.BulkChallanEntry(dt1, AdminId, out OutStatus);// OutStatus mobile
                                    int BankId = 0;
                                    string OutError = "";
                                    int dtResult = AbstractLayer.OnDemandCertificateDB.BulkRegistryMIS(dt1, BankId, adminLoginSession.AdminEmployeeUserId, fileName1, out OutStatus, out OutError);  // BulkChallanBank
                                    if (OutStatus > 0)
                                    {
                                        ViewData["Result"] = "1";
                                        ViewBag.Message = "File Uploaded Successfully";
                                    }
                                    else
                                    {
                                        ViewData["Result"] = "5";
                                        ViewBag.Message = "File Upload Failure: " + OutError;
                                    }

                                }
                            }
                            // End Bulk Code by Rohit                               



                            return View();
                        }
                        else
                        {
                            if (dtexport != null)
                            {
                                ExportDataFromDataTable(1, dtexport, adminLoginSession.AdminEmployeeUserId);
                            }
                            ViewData["Result"] = "-1";
                            ViewBag.Message = CheckMis;
                            return View();
                        }



                    }
                    else
                    {

                        ViewData["Result"] = "2";
                        ViewBag.Message = "Please Upload Only .xls and .xlsx only";
                        return View();
                    }
                    }
              
            }
            catch (Exception ex)
            {
                //  oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                ViewData["Result"] = "0";
                // ViewBag.Message = "File not Uploaded..plz try again";
                ViewBag.Message = ex.Message;
                return View();
            }
            return View();
        }

        #endregion



        #region ViewRegistryDetails

        [AdminLoginCheckFilter]
        public ActionResult ViewRegistryDetails(OnDemandCertificates onDemandCertificates)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

            var itemsch = new SelectList(new[]{
            new {ID="1",Name="Roll Number"},new{ID="2",Name="Registry Number"},new{ID="3",Name="Certificate Number"}}, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();

            string Search = string.Empty;
            //Search = "EmpUserO='" + BM.BCODE + "' ";
            DataSet ds = AbstractLayer.OnDemandCertificateDB.ViewRegistryMISSP(1,Search);


            List<SelectListItem> itemLotList = new List<SelectListItem>();
            itemLotList.Add(new SelectListItem { Text = "--Select Lot--", Value = "0" });
            foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
            {
                itemLotList.Add(new SelectListItem { Text = @dr["RegistryLot"].ToString(), Value = @dr["RegistryLot"].ToString() });
            }
            ViewBag.LotList = itemLotList;

            return View(onDemandCertificates);
        }

        [HttpPost]
        public ActionResult ViewRegistryDetails(OnDemandCertificates onDemandCertificates,string RegistryLot, FormCollection frm)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

            var itemsch = new SelectList(new[]{
                new {ID="1",Name="Roll Number"},new{ID="2",Name="Registry Number"},new{ID="3",Name="Certificate Number"}}, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            try
            {

                string Search = string.Empty;              
                Search = "RegistryNumber is not null ";
                if (onDemandCertificates.RegistryLot > 0)
                {
                    Search += " and RegistryLot='" + onDemandCertificates.RegistryLot + "'";
                }
                if (frm["SelList"] != "")
                {
                    ViewBag.SelectedItem = frm["SelList"];
                    int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());
                    if (frm["SearchString"] != "")
                    {
                        if (SelValueSch == 1)
                        { Search += " and Roll='" + frm["SearchString"].ToString() + "'"; }
                        else if (SelValueSch == 2)
                        { Search += " and  RegistryNumber='" + frm["SearchString"].ToString() + "'"; }
                        else if (SelValueSch == 3)
                        { Search += " and CertNo='" + frm["SearchString"].ToString() + "'"; }
                    }
                }
                DataSet ds = AbstractLayer.OnDemandCertificateDB.ViewRegistryMISSP(2,Search);
                onDemandCertificates.StoreAllData = ds;



                //
                List<SelectListItem> itemLotList = new List<SelectListItem>();
                itemLotList.Add(new SelectListItem { Text = "--Select Lot--", Value = "0" });
                foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                {
                    itemLotList.Add(new SelectListItem { Text = @dr["RegistryLot"].ToString(), Value = @dr["RegistryLot"].ToString() });
                }
                ViewBag.LotList = itemLotList;


                if (ds == null || ds.Tables[1].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.CurrentDownloadedLot = ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = ds.Tables[1].Rows.Count;
                }               
            }
            catch (Exception ex)
            {                
            }
            return View(onDemandCertificates);
        }


        #endregion ViewRegistryDetails

        #endregion


        public ActionResult ExportDataFromDataTable(int tt, DataTable dt, string EmpUserId)
        {
            try
            {
                if (dt.Rows.Count == 0)
                {
                    return RedirectToAction("RegistryMIS", "OnDemandCertificate");
                }
                else
                {
                    string fileName1 = "";
                    if (dt.Rows.Count > 0)
                    {
                        if (tt == 1)
                        {
                            fileName1 = "ERROR_" + EmpUserId + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";
                        }
                        else
                        { fileName1 = EmpUserId + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls"; }
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

                return RedirectToAction("RegistryMIS", "OnDemandCertificate");
            }
            catch (Exception ex)
            {
                return RedirectToAction("RegistryMIS", "OnDemandCertificate");
            }

        }


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