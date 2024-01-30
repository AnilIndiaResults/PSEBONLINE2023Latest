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
using PSEBONLINE.Filters;

namespace PSEBONLINE.Controllers
{
    public class ExaminationPortalController : Controller
    {
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();

        // GET: ExaminationPortal
        [SessionCheckFilter]
        public ActionResult Printlist(string id, int? page)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            ViewBag.Senior = loginSession.Senior;
            ViewBag.Matric = loginSession.Matric;
            ViewBag.OSenior = loginSession.OSenior;
            ViewBag.OMatric = loginSession.OMATRIC;
            try
            {
                string schlcode = loginSession.SCHL;
         

             

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
              
                Printlist obj = new Printlist();

                string Search = "";
                ViewBag.id = id;
                if (id == "M" && ViewBag.Matric == "1")
                {                 
                    Search = " a.SCHL='" + schlcode + "' and form_name in('M1','M2')  and challanVerify=1 and lot!='0' and (emr17flag is null or emr17flag=0) ";
                    obj.StoreAllData = objDB.SelectPrintList(Search, id, pageIndex);
                }
                else if (id == "S" && ViewBag.Senior == "1")
                {                   
                    Search = " a.SCHL='" + schlcode + "' and form_name in('T1','T2') and challanVerify=1 and lot!='0' and (emr17flag is null or emr17flag=0)";
                    obj.StoreAllData = objDB.SelectPrintList(Search, id, pageIndex);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

                
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
                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                // return RedirectToAction("Logout", "Login");
               return View(id);
            }
        }


        [HttpPost]
        [SessionCheckFilter]
        public ActionResult Printlist(string id, int? page, string cmd,string SearchString)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            ViewBag.Senior = loginSession.Senior;
            ViewBag.Matric = loginSession.Matric;
            ViewBag.OSenior = loginSession.OSenior;
            ViewBag.OMatric = loginSession.OMATRIC;
            try
            {
                string schlcode = loginSession.SCHL;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                Printlist obj = new Printlist();

                string Search = "";
                ViewBag.id = id;

                

                if (id == "M" && ViewBag.Matric == "1")
                {
                    Search = " a.SCHL='" + schlcode + "' and form_name in('M1','M2')  and challanVerify=1 and lot!='0' and (emr17flag is null or emr17flag=0) ";
                  
                }
                else if (id == "S" && ViewBag.Senior == "1")
                {
                    Search = " a.SCHL='" + schlcode + "' and form_name in('T1','T2') and challanVerify=1 and lot!='0' and (emr17flag is null or emr17flag=0)";                   
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

                if (cmd == "Search" && !string.IsNullOrEmpty(SearchString))
                {
                    ViewBag.SearchString = SearchString;
                    Search += " and Std_id='" + SearchString.ToString() + "'";
                   
                }

                obj.StoreAllData = objDB.SelectPrintList(Search, id, pageIndex);

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
                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                // return RedirectToAction("Logout", "Login");
                return View(id);
            }
        }


        [SessionCheckFilter]
        public ActionResult ImportedPrintList(string id, int? page)
        {
            try
            {
                LoginSession loginSession = (LoginSession)Session["LoginSession"];
                ViewBag.Senior = loginSession.Senior;
                ViewBag.Matric = loginSession.Matric;
                ViewBag.OSenior = loginSession.OSenior;
                ViewBag.OMatric = loginSession.OMATRIC;

                string schlcode = loginSession.SCHL;

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
               
                Printlist obj = new Printlist();
                string Search = "";
                Search = "a.SCHL='" + schlcode + "'";
                ViewBag.id = id;
                if (id == "M")
                {
                    Search = " a.SCHL='" + schlcode + "' and b.form_name in('M1','M2')";
                }
                else if (id == "S")
                {

                    Search = " a.SCHL='" + schlcode + "' and b.form_name in('T1','T2')";
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                obj.StoreAllData = objDB.SelectImportedPrintList_sp(Search, pageIndex);
               
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
            
            return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View(id);
            }
        }

        public JsonResult JqPrintlist(string storeid, string storescid, string storeaashirwardno)
        {
            
            storeid = storeid.Remove(storeid.Length - 1);
            if (storescid != "")
                storescid = storescid.Remove(storescid.Length - 1);
            if (storeaashirwardno != "")
                storeaashirwardno = storeaashirwardno.Remove(storeaashirwardno.Length - 1);
            //string retval = "";
            //string schoolname = "";
            // string districtname = "";
            //string outid = "";
            //string verifylogin = "";
            string dee = "";
            AbstractLayer.ImportDB objDB = new AbstractLayer.ImportDB();
            dee = objDB.insertinbulkexammasterregular2017(storeid, storescid, storeaashirwardno);


            return Json(new { dee = dee }, JsonRequestBehavior.AllowGet);
        }

        //// Duplicate removed by rohit under supervison of Gulab sir/Manvinder Singh
        public JsonResult JqCheckDuplicateAashirwardNumber(string storeaashirwardno, string storeid, string classR)
        {
            int ClassNo = 0;
            if (classR == "M")
            { ClassNo = 2; }
            else { ClassNo = 4; }

            if (storeaashirwardno != "")
                storeaashirwardno = storeaashirwardno.Remove(storeaashirwardno.Length - 1);
            string duplicateaashirwardno = "";
            string duplicateid = "";
            AbstractLayer.ImportDB objDB = new AbstractLayer.ImportDB();
            objDB.CheckDuplicateAashirwardNumber(storeaashirwardno, storeid, out duplicateaashirwardno, out duplicateid, ClassNo);

            if (duplicateid != "")
                duplicateid = duplicateid.Remove(duplicateid.Length - 1);
            return Json(new { dee = duplicateaashirwardno, dee1 = duplicateid }, JsonRequestBehavior.AllowGet);

        }    
       
  

        public JsonResult JqResendPrintlist(string storeid, string storescid)
        {
            storeid = storeid.Remove(storeid.Length - 1);
            storescid = storescid.Remove(storescid.Length - 1);
            //string retval = "";
            //string schoolname = "";
            // string districtname = "";
            //string outid = "";
            //string verifylogin = "";
            string dee = "";
            AbstractLayer.ImportDB objDB = new AbstractLayer.ImportDB();
            dee = objDB.resendinsertinbulkexammasterregular2017(storeid, storescid);


            return Json(new { dee = dee }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReGenerateFinalPrint(string ChallanId)
        {
            try
            {
                if (ChallanId == null)
                {
                    return RedirectToAction("ExamFormCalFee", "Home");
                }
                ChallanMasterModel CM = new ChallanMasterModel();
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                {
                    return RedirectToAction("Index", "Home");
                }
                string schl = Session["SCHL"].ToString();
                string ChallanId1 = ChallanId.ToString();
                string Usertype = "User";
                int OutStatus;                                
                  
                DataSet ds = objDB.ReGenerateFinalPrint(ChallanId1, Usertype, out OutStatus);
                if (OutStatus == 1)
                {
                    ViewBag.Message = "Re Generate Final Print SuccessFully";
                }
                else
                {
                    ViewBag.Message = "Re Generate Final Print Failure";
                    return RedirectToAction("ExamFormCalFee", "ExaminationPortal");
                }
                return RedirectToAction("ExamFormCalFee", "ExaminationPortal");
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }

        }

        [SessionCheckFilter]
        public ActionResult ExamFormCalFee()
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            try
            {
                Printlist obj = new Printlist();
                obj.StoreAllData = objDB.GetExamFormCalFee(loginSession.SCHL);//GetExamFormCalFeeSP
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
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
                //return RedirectToAction("Logout", "Login");
                return View();
            }
        }

        public ActionResult DownloadExamFinalReport(string ChallanId,string Filename)
        {
            try
            {
                if (ChallanId == null)
                {
                    return RedirectToAction("ExamFormCalFee", "Home");
                }
                ChallanMasterModel CM = new ChallanMasterModel();
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                {
                    return RedirectToAction("Index", "Home");
                }
                string schl = Session["SCHL"].ToString();
                string ChallanId1 = ChallanId.ToString();               
                int OutStatus;
                string Usertype = "User";
                Printlist objP = new Printlist();
               // objP.StoreAllData = objDB.DownloadExamFinalReport(schl, ChallanId);
                DataSet ds = objDB.DownloadExamFinalReport(ChallanId1, Usertype, out OutStatus);
                if (OutStatus == 1)
                {
                    return File(Filename, "application/pdf");
                }
                 else  
                {
                    ViewBag.Message = "File Not Download";
                    ViewBag.TotalCount = 0;
                    return RedirectToAction("ExamFormCalFee", "ExaminationPortal");
                }   
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }

        }


       

            // New 17 
        public ActionResult ExamReGenerateChallaan(string ChallanId)
        {
            try
            {
                if (ChallanId == null)
                {
                    return RedirectToAction("ExamFormCalFee", "Home");
                }
                ChallanMasterModel CM = new ChallanMasterModel();
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                {
                    return RedirectToAction("Index", "Home");
                }
                string schl = Session["SCHL"].ToString();
                string ChallanId1 = ChallanId.ToString();
                string Usertype = "User";
                int OutStatus;
                Printlist objP = new Printlist();
                objP.StoreAllData = objDB.CompleteExamFormFeeByChallan(schl, ChallanId);
                if (objP.StoreAllData == null || objP.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Challan Not Found";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    string SchoolType = objP.StoreAllData.Tables[0].Rows[0]["SchoolType"].ToString();
                    string ClassP = objP.StoreAllData.Tables[0].Rows[0]["Class"].ToString();
                    string outCHALLANID = "";
                    DataSet ds = objDB.ExamReGenerateChallaanById(ChallanId1, Usertype, out OutStatus, out outCHALLANID);
                    if (OutStatus == 1)
                    {
                        Session["resultExamReGenerate"] = "1";
                        //ExamCalculateFee(string id)
                        ViewBag.Message = "Re Generate Challaan SuccessFully";
                        return RedirectToAction("GenerateChallaan", "Home", new { ChallanId = outCHALLANID });
                    }
                    else
                    {
                        Session["resultExamReGenerate"] = "0";
                        ViewBag.Message = "Re Generate Challaan Failure";
                        return RedirectToAction("ExamFormCalFee", "ExaminationPortal");
                    }
                }
                return RedirectToAction("ExamFormCalFee", "ExaminationPortal");
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }

        }

        public JsonResult jqReGenerateChallaanNew(string ChallanId, string BCODE)
        {
           
            string outCHALLANID = "";
            string dee = "0";
            try
            {
                if (ChallanId == null || ChallanId == "")
                {
                    dee = "-1";
                }
                if (BCODE == null || BCODE == "")
                {
                    dee = "-1";
                }
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                {
                    dee = "-1";
                }
                ChallanMasterModel CM = new ChallanMasterModel();
                string ChallanId1 = ChallanId.ToString();
                string Usertype = "User";
                int OutStatus = 0;
                string schl = Session["SCHL"].ToString();
                Printlist objP = new Printlist();
                objP.StoreAllData = objDB.CompleteExamFormFeeByChallan(schl, ChallanId);
                if (objP.StoreAllData == null || objP.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Challan Not Found";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    string SchoolType = objP.StoreAllData.Tables[0].Rows[0]["SchoolType"].ToString();
                    string ClassP = objP.StoreAllData.Tables[0].Rows[0]["Class"].ToString();                   
                    DataSet ds = objDB.ExamReGenerateChallaanById(ChallanId1, Usertype, out OutStatus, out outCHALLANID);                    
                    if (OutStatus == 1)
                    {
                        dee = "1";                       
                    }
                    else
                    {
                        dee = "0";
                    }
                }
            }
            catch (Exception ex)
            {
                dee = "-3";
            }
            return Json(new { dee = dee, chid = outCHALLANID }, JsonRequestBehavior.AllowGet);

        }


        [SessionCheckFilter]
        public ActionResult CompleteExamFormFeeDetails(string ChallanId)//2012117535028
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            try
            {               
                Printlist obj = new Printlist();
                obj.StoreAllData = objDB.CompleteExamFormFeeByChallan(loginSession.SCHL, ChallanId);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
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
                return RedirectToAction("Logout", "Login");
                //return View();
            }
        }

        [HttpPost]
        public ActionResult UpdateAashirwardNo(int id, int reg16id, string aashirwardno)
        {
            int status = -1;
            objDB.updateAashirwardNo(id, reg16id, aashirwardno, out status);
            var results = new
            {
                outstatus = status
            };

            return Json(results);
        }


        /********************OPeN************/
        [SessionCheckFilter]
        public ActionResult PrintlistOpen(string id, int? page)
        {
            try
            {
                LoginSession loginSession = (LoginSession)Session["LoginSession"];
                ViewBag.Senior = loginSession.Senior;
                ViewBag.Matric = loginSession.Matric;
                ViewBag.OSenior = loginSession.OSenior;
                ViewBag.OMatric = loginSession.OMATRIC;


                string schlcode = loginSession.SCHL;

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                //AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                Printlist obj = new Printlist();
                string Search = "";
                ViewBag.id = id;
                if (id == "MO" && ViewBag.OMatric == "1")
                {
                    //Search = " form_name in('M1','M2') and challanVerify=1 and SCHL='"+schlcode+"'";
                    Search = " R.SCHL='" + schlcode + "' and R.FORM in ('M3')  and R.Schl is not null  and (emr17flag is null or emr17flag=0)";
                    obj.StoreAllData = objDB.SelectPrintListSPOpen(Search, id, pageIndex);
                }
                else if (id == "SO" && ViewBag.OSenior == "1")
                {
                    //Search = " form_name in('T1','T2') and challanVerify=1 and SCHL='" + schlcode + "'";
                    Search = " R.SCHL='" + schlcode + "' and R.FORM  in ('T3') and R.Schl is not null and (emr17flag is null or emr17flag=0)";
                    obj.StoreAllData = objDB.SelectPrintListSPOpen(Search, id, pageIndex);
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

               
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

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
               // return RedirectToAction("Logout", "Login");
               return View(id);
            }
        }

        [SessionCheckFilter]
        public ActionResult ImportedPrintListOpen(string id, int? page)
        {
            try
            {
                LoginSession loginSession = (LoginSession)Session["LoginSession"];
                ViewBag.Senior = loginSession.Senior;
                ViewBag.Matric = loginSession.Matric;
                ViewBag.OSenior = loginSession.OSenior;
                ViewBag.OMatric = loginSession.OMATRIC;

               
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                //AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                Printlist obj = new Printlist();
                string Search = "";
                //Search = "a.SCHL='" + schlcode + "'";
                ViewBag.id = id;
                if (id == "MO")
                {
                    Search = " a.SCHL='" + loginSession.SCHL + "' and R.Form in('M3')";
                }
                else if (id == "SO")
                {

                    Search = " a.SCHL='" + loginSession.SCHL + "' and R.Form in('T3')";
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                obj.StoreAllData = objDB.SelectImportedPrintListOpen(Search, pageIndex);
               
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

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View(id);
            }
        }
        
        public JsonResult JqPrintlistOpen(string class1, string storeid, string storescid, string storeaashirwardno)
        {
            int cls = class1 == "MO" ? 10 : 12;

            storeid = storeid.Remove(storeid.Length - 1);
            if (storescid != "")
                storescid = storescid.Remove(storescid.Length - 1);
            if (storeaashirwardno != "")
                storeaashirwardno = storeaashirwardno.Remove(storeaashirwardno.Length - 1);          
            string dee = "";
            AbstractLayer.ImportDB objDB = new AbstractLayer.ImportDB();
            dee = objDB.insertinbulkexammasterregular2017NEwOpen(cls, storeid, storescid, storeaashirwardno);
            return Json(new { dee = dee }, JsonRequestBehavior.AllowGet);
        }    

        public JsonResult JqResendPrintlistOpen(string storeid, string storescid)
        {
            storeid = storeid.Remove(storeid.Length - 1);
            storescid = storescid.Remove(storescid.Length - 1);          
            string dee = "";
            AbstractLayer.ImportDB objDB = new AbstractLayer.ImportDB();
            dee = objDB.resendinsertinbulkexammasterregular2017Open(storeid, storescid);
            return Json(new { dee = dee }, JsonRequestBehavior.AllowGet);
        }


        #region viewAllExamCandidate

        [SessionCheckFilter]
        public ActionResult ViewAllExamCandidate(int? page)
        {
            try
            {
                LoginSession loginSession = (LoginSession)Session["LoginSession"];
     

             
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                //AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                Printlist obj = new Printlist();
                string Search = "";
                Search = "a.SCHL='" + loginSession.SCHL + "'";

                obj.StoreAllData = objDB.SelectAView_All_Exam_Candidate(Search, pageIndex);
               
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

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View(page);
            }
        }

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult ViewAllExamCandidate(int? page, FormCollection frm)
        {
            try
            {
                LoginSession loginSession = (LoginSession)Session["LoginSession"];
                string schlcode = loginSession.SCHL;

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                //AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
                Printlist obj = new Printlist();
                string Search = "";
                if (frm["Category"] != "0")
                {
                    ViewBag.Category = frm["Category"];
                    int SelValueSch = Convert.ToInt32(frm["Category"].ToString());

                    if (frm["SearchString"] != "")
                    {
                        if (SelValueSch == 1)
                        {
                            Search = "a.SCHL='" + schlcode + "' and a.reg16id='" + frm["SearchString"].ToString().Trim() + "'";
                        }
                        if (SelValueSch == 2)
                        {
                            Search = "a.SCHL='" + schlcode + "' and b.Candi_Name like '%" + frm["SearchString"].ToString() + "%'";
                        }
                        if (SelValueSch == 3)
                        {
                            Search = "a.SCHL='" + schlcode + "' and a.Type='OPEN' and a.Class=2 and (Convert(varchar,a.reg16id)='" + frm["SearchString"].ToString().Trim() + "' or b.Candi_Name like '%" + frm["SearchString"].ToString() + "%')  ";
                        }
                        if (SelValueSch == 4)
                        {
                            Search = "a.SCHL='" + schlcode + "' and a.Type='REG' and a.Class=2 and (Convert(varchar,a.reg16id)='" + frm["SearchString"].ToString().Trim() + "' or b.Candi_Name like '%" + frm["SearchString"].ToString() + "%')  ";
                        }
                        if (SelValueSch == 5)
                        {
                            Search = "a.SCHL='" + schlcode + "' and a.Type='OPEN' and a.Class=4 and (Convert(varchar,a.reg16id)='" + frm["SearchString"].ToString().Trim() + "' or b.Candi_Name like '%" + frm["SearchString"].ToString() + "%')  ";
                        }
                        if (SelValueSch == 6)
                        {
                            Search = "a.SCHL='" + schlcode + "' and a.Type='REG' and a.Class=4 and (Convert(varchar,a.reg16id)='" + frm["SearchString"].ToString().Trim() + "' or b.Candi_Name like '%" + frm["SearchString"].ToString() + "%')  ";
                        }
                        if (SelValueSch == 7)
                        {
                            Search = "a.SCHL='" + schlcode + "' and a.printlotnew ='" + frm["SearchString"].ToString().Trim() + "'";
                        }
                    }
                    else
                    {
                        if (SelValueSch == 3)
                        {
                            Search = "a.SCHL='" + schlcode + "' and a.Type='OPEN' and a.Class=2 ";
                        }
                        if (SelValueSch == 4)
                        {
                            Search = "a.SCHL='" + schlcode + "' and a.Type='REG' and a.Class=2 ";
                        }
                        if (SelValueSch == 5)
                        {
                            Search = "a.SCHL='" + schlcode + "' and a.Type='OPEN' and a.Class=4 ";
                        }
                        if (SelValueSch == 6)
                        {
                            Search = "a.SCHL='" + schlcode + "' and a.Type='REG' and a.Class=4 ";
                        }
                    }
                }
                else if (frm["Category"] == "0")
                {
                    Search = "a.SCHL='" + schlcode + "'";
                }

                obj.StoreAllData = objDB.SelectAView_All_Exam_Candidate(Search, pageIndex);
               
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

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                //return RedirectToAction("Logout", "Login");
                return View(page);
            }
        }
        #endregion viewAllExamCandidate






    }
}
