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
using System.Web.Routing;
using System.Reflection;
using System.Web.Caching;
using PSEBONLINE.Filters;
using PsebPrimaryMiddle.Controllers;


using CCA.Util;
using PSEBONLINE.AbstractLayer;
using System.Data.Entity;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon;
namespace PSEBONLINE.Controllers
{
    public class HomeController : Controller
    {
        private const string BUCKET_NAME = "psebdata";
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

        ////Executes after every action
        //protected override void OnActionExecuted(ActionExecutedContext context)
        //{
        //    base.OnActionExecuted(context);
        //}
        #endregion SiteMenu


        private readonly DBContext _context = new DBContext();
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.HomeDB objDB = new AbstractLayer.HomeDB();
        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        HomeModels Hm = new HomeModels();


        public ActionResult RegPhotoFromOldPathToNewPath()
        {
            try
            {
                List<RegPhotoFromOldPathToNewPathViews> regPhotoFromOldPathToNewPathViewsList = _context.RegPhotoFromOldPathToNewPathViews.ToList();

                if (regPhotoFromOldPathToNewPathViewsList.Count() > 0)
                {
                    int result1 = 0;
                    foreach (RegPhotoFromOldPathToNewPathViews data in regPhotoFromOldPathToNewPathViewsList)
                    {

                        string studentid = data.std_id.ToString();

                        try
                        {

                       
                        // DataSet ds = new AbstractLayer.RegistrationDB().GetStudentDataById(studentid); // std_id in ('+@studentid+')
                        if (!string.IsNullOrEmpty(studentid))
                        {

                            string std_id = data.std_id.ToString();
                            string formName = data.form_name.ToString();
                            string result = data.StudentUniqueIdNEW.ToString();
                            string sldist = data.SCHLDIST.ToString();


                            string std_Photo = data.std_Photo.ToString();
                            string std_Sign = data.std_Sign.ToString();

                            string Old_std_Photo = data.Old_std_Photo.ToString();
                            string Old_std_Sign = data.Old_std_Sign.ToString();
                            string oldyear = data.oldyear.ToString();

                            string Upload = System.Configuration.ConfigurationManager.AppSettings["Upload"];

                            result1 += 1;

                            string filepathtosave = "";

                            #region Photo and Sign
                            string filePath = "";
                            string filePathSign = "";

                            if (std_Photo != null && std_Photo != "")
                            {
                                @ViewBag.Photo = "../../upload/Upload2023/" + std_Photo;
                            }
                            else
                            {


                                    if (oldyear.ToString() == "2022")
                                    {
                                        if (std_Photo == null || std_Photo == "")
                                        {
                                            if (Old_std_Photo != null && Old_std_Photo != "")
                                            {
                                                @ViewBag.Photo = Old_std_Photo;
                                                filePath = @"C:\ClusterStorage\Volume1\upload\upload2022\" + Old_std_Photo.Replace("https://registration2022.pseb.ac.in/upload/Upload2022/", "");
                                             
                                            }
                                        }
                                        else
                                        {
                                            @ViewBag.Photo = "../../upload/Upload2023/" + std_Photo;
                                        }
                                    }
                                    else if (oldyear.ToString() == "2021")
                                    {
                                        if (std_Photo == null || std_Photo == "")
                                        {
                                            if (Old_std_Photo != null && Old_std_Photo != "")
                                            {

                                                @ViewBag.Photo = Old_std_Photo;
                                                filePath = @"C:\ClusterStorage\Volume1\upload\upload2020\" + Old_std_Photo.Replace("https://registration2022.pseb.ac.in/upload/Upload2020/", "");

                                            }
                                        }
                                        else
                                        {
                                            @ViewBag.Photo = "../../upload/Upload2023/" + std_Photo;
                                        }
                                    }
                                    else if (oldyear.ToString() == "2020")
                                    {
                                        if (std_Photo == null || std_Photo == "")
                                        {
                                            if (Old_std_Photo != null && Old_std_Photo != "")
                                            {

                                                @ViewBag.Photo = Old_std_Photo;
                                                filePath = @"C:\ClusterStorage\Volume1\upload\upload2019\" + Old_std_Photo.Replace("https://registration2022.pseb.ac.in/upload/Upload2019/", "");

                                            }
                                        }
                                        else
                                        {
                                            @ViewBag.Photo = "../../upload/Upload2023/" + std_Photo;
                                        }
                                    }
                                    else if (oldyear.ToString() == "2019")
                                    {
                                        if (std_Photo == null || std_Photo == "")
                                        {
                                            if (Old_std_Photo != null && Old_std_Photo != "")
                                            {

                                                @ViewBag.Photo = Old_std_Photo;
                                                filePath = @"C:\ClusterStorage\Volume1\upload\upload2018\" + Old_std_Photo.Replace("https://registration2022.pseb.ac.in/upload/Upload2018/", "");


                                            }
                                        }
                                        else
                                        {
                                            @ViewBag.Photo = "../../upload/Upload2023/" + std_Photo;
                                        }
                                    }
                                    else
                                    {
                                        @ViewBag.Photo = "../../upload/Upload2023/" + std_Photo;
                                    }
                            }
                            //---------------------------------------------------------For Sign------------------------------------------

                            if (std_Sign != null && std_Sign != "")
                            {
                                //  not imported

                                @ViewBag.sign = "../../upload/Upload2023/" + std_Sign;
                            }
                            else
                            {

                                    if (oldyear.ToString() == "2022")
                                    {
                                        if (std_Sign == null || std_Sign == "")
                                        {
                                            if (Old_std_Sign != null && Old_std_Sign != "")
                                            {

                                                @ViewBag.sign = Old_std_Sign;
                                                filePathSign = @"C:\ClusterStorage\Volume1\upload\upload2022\" + Old_std_Sign.Replace("https://registration2022.pseb.ac.in/upload/Upload2022/", "");

                                            }
                                        }
                                        else
                                        {
                                            @ViewBag.sign = "../../upload/Upload2023/" + std_Sign;
                                        }
                                    }
                                    else if (oldyear.ToString() == "2021")
                                    {
                                        if (std_Sign == null || std_Sign == "")
                                        {
                                            if (Old_std_Sign != null && Old_std_Sign != "")
                                            {
                                                @ViewBag.sign = Old_std_Sign;
                                                filePathSign = @"C:\ClusterStorage\Volume1\upload\upload2020\" + Old_std_Sign.Replace("https://registration2022.pseb.ac.in/upload/Upload2020/", "");

                                            }
                                        }
                                        else
                                        {
                                            @ViewBag.sign = "../../upload/Upload2023/" + std_Sign;
                                        }
                                    }
                                    else if (oldyear.ToString() == "2020")
                                    {
                                        if (std_Sign == null || std_Sign == "")
                                        {
                                            if (Old_std_Sign != null && Old_std_Sign != "")
                                            {

                                                @ViewBag.sign = Old_std_Sign;
                                                filePathSign = @"C:\ClusterStorage\Volume1\upload\upload2019\" + Old_std_Sign.Replace("https://registration2022.pseb.ac.in/upload/Upload2019/", "");

                                            }
                                        }
                                        else
                                        {
                                            @ViewBag.sign = "Upload2023/" + std_Sign;
                                        }
                                    }
                                    else if (oldyear.ToString() == "2019")
                                    {
                                        if (std_Sign == null || std_Sign == "")
                                        {
                                            if (Old_std_Sign != null && Old_std_Sign != "")
                                            {
                                                @ViewBag.sign = Old_std_Sign;
                                                filePathSign = @"C:\ClusterStorage\Volume1\upload\upload2018\" + Old_std_Sign.Replace("https://registration2022.pseb.ac.in/upload/Upload2018/", "");

                                            }
                                        }
                                        else
                                        {
                                            @ViewBag.sign = "../../upload/Upload2023/" + std_Sign;
                                        }
                                    }

                                    else
                                    {
                                        @ViewBag.sign = "../../upload/Upload2023/" + std_Sign;
                                    }
                            }

                            #endregion



                            if (!string.IsNullOrEmpty(result))
                            {
                                    //-----------------------------------Session 2023-2024------------
                                    if (filePath != null)
                                    {
                                        var filePath1 = filePath;
                                        if ((std_Photo == null || std_Photo == "") && (filePath != null && filePath != ""))
                                        {
                                            //var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/" + formName + "/" + sldist + "/Photo"), result + "P" + ".jpg");
                                            //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/" + formName + "/" + sldist + "/Photo"));
                                            //if (!Directory.Exists(FilepathExist))
                                            //{
                                            //    Directory.CreateDirectory(FilepathExist);
                                            //}
                                            //if (System.IO.File.Exists(path))
                                            //{
                                            //    System.IO.File.Delete(path);
                                            //}
                                            //System.IO.File.Copy(filePath1, path);
                                            //filepathtosave = "../Upload/Upload2023/" + formName + "/" + sldist + "/Photo/" + result + "P" + ".jpg";
                                            //ViewBag.ImageURL = filepathtosave;

                                            string PhotoName = formName + "/" + sldist + "/Photo" + "/" + result + "P" + ".jpg";
                                            string type = "P";
                                            string IsNew = "NO";
                                            string UpdatePic = new AbstractLayer.RegistrationDB().Updated_PhotoSign_ByStudentId(std_id, PhotoName, type, IsNew);

                                          


                                            
                                        }
                                    }
                                    if (filePathSign != null)
                                    {
                                        var filePathS = filePathSign;
                                        if ((std_Sign == null || std_Sign == "") && (filePathSign != null && filePathSign != ""))
                                        {
                                            var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/" + formName + "/" + sldist + "/Sign"), result + "S" + ".jpg");
                                            string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/" + formName + "/" + sldist + "/Sign"));
                                            if (!Directory.Exists(FilepathExist))
                                            {
                                                Directory.CreateDirectory(FilepathExist);
                                            }
                                            if (System.IO.File.Exists(path))
                                            {
                                                System.IO.File.Delete(path);
                                            }
                                            System.IO.File.Copy(filePathS, path);
                                            filepathtosave = "../Upload/Upload2023/" + formName + "/" + sldist + "/Sign/" + result + "S" + ".jpg";
                                            ViewBag.ImageURL = filepathtosave;

                                            string PhotoName = formName + "/" + sldist + "/Sign" + "/" + result + "S" + ".jpg";
                                            string type = "S";
                                            string IsNew = "NO";
                                            string UpdatePic = new AbstractLayer.RegistrationDB().Updated_PhotoSign_ByStudentId(std_id, PhotoName, type, IsNew);


                                        }
                                    }
                                // //-----------------------------------End  Session 2023-2024------------
                            }

                        }

                        }
                        catch (Exception ex1)
                        {

                        }

                    }
                    //forwach close
                }

            }
            catch (Exception ex)
            {

            }

            return View();
        }


        #region Open School Report
        [Route("OpenSchoolReport")]
        [Route("Home/OpenSchoolReport")]
        public ActionResult OpenSchoolReport()
        {
            string districts = string.Empty;
            // Dist Allowed
            string DistAllow = "";
            if (ViewBag.DistAllow != null)
            {
                DistAllow = ViewBag.DistAllow;
            }
            List<SelectListItem> OpenDistricts = objDB.OpenSchoolDistricts();
            if (ViewBag.DistUser == null || ViewBag.DistAllow == null)
            {
                ViewBag.Districts = new AbstractLayer.OpenDB().GetDistrict();
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
        [Route("OpenSchoolReport")]
        [Route("Home/OpenSchoolReport")]
        [HttpPost]
        public ActionResult OpenSchoolReport(FormCollection fc)
        {
            string DistAllow = "";
            if (ViewBag.DistAllow != null)
            {
                DistAllow = ViewBag.DistAllow;
            }
            if (ViewBag.DistUser == null || ViewBag.DistAllow == null)
            {
                ViewBag.Districts = new AbstractLayer.OpenDB().GetDistrict();
            }
            else
            {
                ViewBag.Districts = ViewBag.DistUser;
            }
            ViewBag.SelectedDist = fc["ddlDist"] != null ? fc["ddlDist"].ToString() : string.Empty;

            if (fc["ddlDist"] != null)
            {
                var obj = new AbstractLayer.AdminDB().GetSchoolRecords1819(fc["ddlDist"].ToString());
                ViewBag.data = obj;


            }
            return View();
        }

        #endregion Open School Report

        #region DuplicateCertificate Status
        [Route("DuplicateCertificateStatus")]
        [Route("Home/DuplicateCertificateStatus")]
        public ActionResult DuplicateCertificateStatus(DuplicateCertificate dc)
        {
            var itemsch = new SelectList(new[] { new { ID = "1", Name = "Dairy Number" },new { ID = "2", Name = "Receipt Number " },}, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            return View();
        }

        [HttpPost]
        [Route("DuplicateCertificateStatus")]
        [Route("Home/DuplicateCertificateStatus")]
        public ActionResult DuplicateCertificateStatus(DuplicateCertificate dc, FormCollection frm, string submit)
        {
            var itemsch = new SelectList(new[] { new { ID = "1", Name = "Dairy Number" }, new { ID = "2", Name = "Receipt Number " }, }, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            try
            {
                if (submit != null)
                {
                    if (submit.ToUpper() == "RESET")
                    {                       
                        return RedirectToAction("DuplicateCertificateStatus", "Home");
                    }
                }   
               
                if (ModelState.IsValid)
                {
                    string Search = string.Empty;                  
                    Search = "a.id like '%' ";
                    if (frm["SelList"] != "")
                    {
                        ViewBag.SelectedItem = frm["SelList"];
                        int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());
                        if (frm["SearchString"] != "")
                        {
                            ViewBag.SearchString = frm["SearchString"].ToString();
                            if (SelValueSch == 1)
                            { Search += " and DairyNo=" + frm["SearchString"].ToString(); }                         
                            else if (SelValueSch == 2)
                            { Search += " and a.ReceiptNo='" + frm["SearchString"].ToString() + "'"; }
                        }
                    }
                    dc.StoreAllData = new AbstractLayer.AdminDB().ViewDuplicateCertificate(0,1, Search, 1, 20);
                    if (dc.StoreAllData == null || dc.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";                     
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = dc.StoreAllData.Tables[0].Rows.Count;
                        //if (dc.StoreAllData.Tables[0].Rows[0]["DispNo"].ToString() != "")
                        //{
                        //    ViewBag.Status = 2;
                        //    dc.DispNo = dc.StoreAllData.Tables[0].Rows[0]["DispNo"].ToString();
                        //    dc.DispDate = Convert.ToDateTime(dc.StoreAllData.Tables[0].Rows[0]["DispDate"].ToString());
                        //    ViewBag.DispDate = dc.StoreAllData.Tables[0].Rows[0]["DispDate"].ToString();
                        //    dc.CertNo = dc.StoreAllData.Tables[0].Rows[0]["CertNo"].ToString();
                        //    dc.CertDate = Convert.ToDateTime(dc.StoreAllData.Tables[0].Rows[0]["CertDate"].ToString());
                        //    ViewBag.CertDate = dc.StoreAllData.Tables[0].Rows[0]["CertDate"].ToString();
                        //}
                        //else if (dc.StoreAllData.Tables[0].Rows[0]["ObjectionLetter"].ToString() != "")
                        //{
                        //    ViewBag.Status = 3;
                        //    //dc.CertNo = dc.StoreAllData.Tables[0].Rows[0]["CertNo"].ToString();
                        //    //dc.CertDate = Convert.ToDateTime(dc.StoreAllData.Tables[0].Rows[0]["CertDate"].ToString());
                        //   // ViewBag.CertDate = dc.StoreAllData.Tables[0].Rows[0]["CertDate"].ToString();
                        //    ViewBag.ObjectionLetter = dc.StoreAllData.Tables[0].Rows[0]["ObjectionLetter"].ToString();
                        //    dc.ObjectionLetter = dc.StoreAllData.Tables[0].Rows[0]["ObjectionLetter"].ToString();
                        //}
                        //else if (dc.StoreAllData.Tables[0].Rows[0]["CertNo"].ToString() == "" || dc.StoreAllData.Tables[0].Rows[0]["DispNo"].ToString() == "")
                        //{
                        //    ViewBag.Status = 1; 
                        //}

                   
                        //dc.Name = dc.StoreAllData.Tables[0].Rows[0]["Name"].ToString();
                        //dc.FNAME = dc.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                        //dc.MNAME = dc.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();
                        //dc.DairyDate = Convert.ToDateTime(dc.StoreAllData.Tables[0].Rows[0]["DairyDate"].ToString());
                        //dc.DairyNo = Convert.ToString(dc.StoreAllData.Tables[0].Rows[0]["DairyNo"].ToString());
                        //dc.Roll = Convert.ToString(dc.StoreAllData.Tables[0].Rows[0]["Roll"].ToString());
                        //ViewBag.IsStatus  = dc.StoreAllData.Tables[0].Rows[0]["IsStatus"].ToString();
                        //ViewBag.FinalStatus = Convert.ToString(dc.StoreAllData.Tables[0].Rows[0]["Status"].ToString());
                        return View(dc);
                    }
                }
                else
                {
                    return RedirectToAction("DuplicateCertificateStatus", "Home");
                }
            }
            catch (Exception ex)
            {               
                return View();
            }
        }

        #endregion DuplicateCertificate Status

        #region Challan Deposit Details 


        public ActionResult FeeDepositDetails(string id)
        {
            if (Session["Session"] == null || Session["Session"].ToString() == "")
            {
                return RedirectToAction("Logout", "Login");
            }
            if (id == null || id.ToString() == "")
            {
                return RedirectToAction("FinalPrint", "Home");
            }
            DataSet dst = new AbstractLayer.BankDB().GetChallanDetailsByIdSPBank(id);
            if (dst == null || dst.Tables[0].Rows.Count == 0)
            {
                return RedirectToAction("FinalPrint", "Home");
            }
            else
            {
                string StudentList = dst.Tables[0].Rows[0]["StudentList"].ToString().Trim();
                if (StudentList != "")
                {
                    Session["StudentList"] = StudentList;
                    return RedirectToAction("ChallanDepositDetails", "Home");
                }
            }
            return RedirectToAction("FinalPrint", "Home");
        }



        public ActionResult ChallanDepositDetails(string id)
        {
            //Session["StudentList"] = "1210718243230";
            if (Session["Session"] == null || Session["Session"].ToString() == "")
            {             
                return RedirectToAction("Logout", "Login");
            }
            if (Session["StudentList"] == null || Session["StudentList"].ToString() == "")
            {
                return RedirectToAction("FinalPrint", "Home");
            }
            //if (Session["StudentList"] == null)
            //{ return RedirectToAction("Logout", "Login"); }           
             id = Session["StudentList"].ToString();           
            ///id = encrypt.QueryStringModule.Decrypt(id);            
            //id = Session["StudentList"].ToString();
            ChallanDepositDetailsModel cdm = new ChallanDepositDetailsModel();
            if (id == null || id.ToString() == "")
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                ViewBag.cid = id.ToString();
                ViewBag.SelectedChallanId = "";
                // For all challans either cancel or not. by student list
                DataSet dst = objDB.GetChallanDetailsByStudentList(id);
                if (dst == null || dst.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = 1;
                    if (dst.Tables[0].Rows.Count > 0)
                    {
                        List<SelectListItem> itemCHALLANID = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in dst.Tables[0].Rows)
                        {
                            itemCHALLANID.Add(new SelectListItem { Text = @dr["CHALLANID"].ToString().Trim(), Value = @dr["CHALLANID"].ToString().Trim() });
                        }
                        ViewBag.MyChallanId = itemCHALLANID.ToList();
                    }
                }
            }
            return View(cdm);
        }

        [HttpPost]       
        public ActionResult ChallanDepositDetails(string id,ChallanDepositDetailsModel cdm, FormCollection frm, string submit)
        {
            if (Session["Session"] == null || Session["Session"].ToString() == "")
            {
                return RedirectToAction("Logout", "Login");
            }
            if (Session["StudentList"] == null || Session["StudentList"].ToString() == "")
            {
                return RedirectToAction("FinalPrint", "Home");
            }
            //if (Session["StudentList"] == null)
            //{ return RedirectToAction("Logout", "Login"); }
            id = Session["StudentList"].ToString();
            ///// id = encrypt.QueryStringModule.Decrypt(id);
      

            try
            {
                if (id == null || id.ToString() == "")
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    ViewBag.cid = id.ToString();
                    // For all challans either cancel or not. by student list
                    DataSet dst = objDB.GetChallanDetailsByStudentList(id);                   
                    if (dst == null || dst.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        if (dst.Tables[0].Rows.Count > 0)
                        {
                            List<SelectListItem> itemCHALLANID = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in dst.Tables[0].Rows)
                            {
                                itemCHALLANID.Add(new SelectListItem { Text = @dr["CHALLANID"].ToString().Trim(), Value = @dr["CHALLANID"].ToString().Trim() });
                            }
                            ViewBag.MyChallanId = itemCHALLANID.ToList();
                        }
                    }
                }


                if (frm["CHALLANID"] != "")
                {
                    if (submit != null)
                    {
                        if (submit.ToUpper() == "CANCEL" && Session["AdminType"].ToString().ToUpper() == "ADMIN")
                        {
                            if (frm["CHALLANID"] != "")
                            {
                                string dee = "";
                                string outstatus = "";
                                int AdminId = Convert.ToInt32(Session["AdminId"]);
                                string challanid = cdm.CHALLANID;
                                objDB.ChallanDepositDetailsCancel("Cancel", frm["CHALLANID"].ToString(), out outstatus, AdminId);//ChallanDetailsCancelSP
                                if (outstatus == "1")
                                {
                                    ViewData["Result"] = "10";
                                }
                                else 
                                {
                                    ViewData["Result"] = "11";
                                }                             
                            }
                            return View(cdm);
                        }
                    }

                    DataSet ds1 = new AbstractLayer.BankDB().GetChallanDetailsByIdSPBank(frm["CHALLANID"].ToString());
                    cdm.dsData = ds1;
                    if (cdm.dsData == null || cdm.dsData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        cdm.CHALLANID = ds1.Tables[0].Rows[0]["CHALLANID"].ToString();
                        cdm.SCHLREGID = ds1.Tables[0].Rows[0]["SCHLREGID"].ToString();
                        cdm.LOT = Convert.ToInt32(ds1.Tables[0].Rows[0]["LOT"].ToString());
                        cdm.APPNO = ds1.Tables[0].Rows[0]["APPNO"].ToString();
                        cdm.SCHLCANDNM = ds1.Tables[0].Rows[0]["SCHLCANDNM"].ToString();
                        cdm.CHLNDATE = ds1.Tables[0].Rows[0]["CHLNDATE"].ToString();
                        cdm.CHLVDATE = ds1.Tables[0].Rows[0]["CHLNVDATE"].ToString();
                        cdm.BANK = ds1.Tables[0].Rows[0]["BANK"].ToString();
                        cdm.FEE = ds1.Tables[0].Rows[0]["FEE"].ToString();
                        cdm.DOWNLDDATE = ds1.Tables[0].Rows[0]["DOWNLDDATE"].ToString();
                        cdm.DOWNLDFLOT = ds1.Tables[0].Rows[0]["DOWNLDFLOT"].ToString();
                    }

                    DataSet ds = new DataSet();
                    string OutError = "0";
                    if (cdm.CHALLANID != "")
                    {
                        ds = objDB.ChallanDepositDetails(cdm, out OutError);
                        if (OutError == "1")
                        {
                            ViewData["Result"] = "1";
                        }
                        else
                        {
                            ViewData["Result"] = "0";
                        }

                    }
                }
                return View(cdm);
             
            }
            catch (Exception ex)
            {               
                return RedirectToAction("Logout", "Login");
            }
        }

        // Get Challan
        public JsonResult GetChallanDetails(string challanid)
        {
            try
            {
                ChallanDepositDetailsModel cdm = new ChallanDepositDetailsModel();
                int OutStatus = 0;
                //string  CHALLANID = "", FEE="", BANK="", SCHLREGID = "", LOT = "", CHLNVDATE="", BRANCHCAND = "",
                //    BRCODECAND = "", J_REF_NOCAND = "", DEPOSITDTCAND = "";
                int AdminId = Convert.ToInt32(Session["AdminId"]);
                DataSet ds1 = new AbstractLayer.BankDB().GetChallanDetailsByIdSPBank(challanid);
                if (ds1 == null || ds1.Tables[0].Rows.Count == 0)
                {
                    OutStatus = 0;          
                }
                else
                {
                    OutStatus =1;
                    //CHALLANID = ds1.Tables[0].Rows[0]["CHALLANID"].ToString();
                    //LOT = ds1.Tables[0].Rows[0]["LOT"].ToString();
                    //SCHLREGID = ds1.Tables[0].Rows[0]["SCHLREGID"].ToString();
                    //FEE = ds1.Tables[0].Rows[0]["FEE"].ToString();
                    //BANK = ds1.Tables[0].Rows[0]["BANK"].ToString();                          
                    //CHLNVDATE = ds1.Tables[0].Rows[0]["CHLNVDATE"].ToString();
                    cdm.CHALLANID = ds1.Tables[0].Rows[0]["CHALLANID"].ToString();
                    cdm.SCHLREGID = ds1.Tables[0].Rows[0]["SCHLREGID"].ToString();
                    cdm.LOT = Convert.ToInt32(ds1.Tables[0].Rows[0]["LOT"].ToString());
                    cdm.APPNO = ds1.Tables[0].Rows[0]["APPNO"].ToString();
                    cdm.SCHLCANDNM = ds1.Tables[0].Rows[0]["SCHLCANDNM"].ToString();
                    cdm.CHLNDATE = ds1.Tables[0].Rows[0]["CHLNDATE"].ToString().Split(' ')[0].ToString();
                    cdm.CHLVDATE = ds1.Tables[0].Rows[0]["CHLNVDATE"].ToString().Split(' ')[0].ToString();
                    cdm.BANK = ds1.Tables[0].Rows[0]["BANK"].ToString();
                    cdm.FEE = ds1.Tables[0].Rows[0]["FEE"].ToString();
                    cdm.DOWNLDDATE = ds1.Tables[0].Rows[0]["DOWNLDDATE"].ToString();
                    cdm.DOWNLDFLOT = ds1.Tables[0].Rows[0]["DOWNLDFLOT"].ToString();

                    cdm.BRANCHCAND = ds1.Tables[0].Rows[0]["BRANCHCAND"].ToString();
                    cdm.BRCODECAND = ds1.Tables[0].Rows[0]["BRCODECAND"].ToString();
                    cdm.J_REF_NOCAND = ds1.Tables[0].Rows[0]["J_REF_NOCAND"].ToString();
                    cdm.DEPOSITDTCAND = ds1.Tables[0].Rows[0]["DEPOSITDTCAND"].ToString();
                    cdm.DOWNLDFLG = Convert.ToInt32(ds1.Tables[0].Rows[0]["DOWNLDFLG"].ToString());
                }
                 var results = new
                {
                    status = OutStatus,
                    chid = cdm.CHALLANID,
                    SCHLREGID = cdm.SCHLREGID,
                    LOT = cdm.LOT,
                    APPNO = cdm.APPNO,
                    SCHLCANDNM = cdm.SCHLCANDNM,
                    CHLNDATE = cdm.CHLNDATE,
                    CHLVDATE = cdm.CHLVDATE,
                    BANK = cdm.BANK,
                    FEE = cdm.FEE,
                    DOWNLDDATE = cdm.DOWNLDDATE,
                    DOWNLDFLOT = cdm.DOWNLDFLOT,
                     DOWNLDFLG = cdm.DOWNLDFLG,
                     DOWNLDFSTATUS = Convert.ToInt32(cdm.DOWNLDFLG) == 1 ? "Downloaded on : " + cdm.DOWNLDDATE + "" : "Not Downloaded",
                     BRANCHCAND = cdm.BRANCHCAND,
                    BRCODECAND = cdm.BRCODECAND,
                    J_REF_NOCAND = cdm.J_REF_NOCAND,
                    DEPOSITDTCAND = cdm.DEPOSITDTCAND
                };
                return Json(results, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        #endregion Challan Deposit Details

        public ActionResult Index(string Length, int? page)
        {
            try
            {


                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                {
                    return RedirectToAction("Logout", "Login");
                }
                Printlist obj = new Printlist();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;


                #region Circular

                string Search = string.Empty;
                Search = "Id like '%' and  CircularTypeName like '%SCHOOL-HOME%'  and Convert(Datetime,Convert(date,ExpiryDateDD))>=Convert(Datetime,Convert(date,getdate()))";


                // Cache
                DataSet dsCircular = new DataSet();
                DataSet cacheData = HttpContext.Cache.Get("HomeCircular") as DataSet;

                if (cacheData == null)
                {
                    dsCircular = new AbstractLayer.AdminDB().CircularMaster(Search, pageIndex);
                    cacheData = dsCircular;
                    HttpContext.Cache.Insert("HomeCircular", cacheData, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);

                }
                else
                {
                    dsCircular = cacheData;
                }
                // Cache end 

                // DataSet dsCircular = new AbstractLayer.AdminDB().CircularMaster(Search, pageIndex);//GetAllFeeMaster2016SP
                if (dsCircular == null || dsCircular.Tables[0].Rows.Count == 0)
                {
                    ViewBag.TotalCircular = 0;
                }
                else
                {
                    //var type7 = dsCircular.Tables[0].Columns[7].DataType.Name.ToString();
                    var type8 = dsCircular.Tables[0].Columns[9].DataType.Name.ToString();
                    ViewBag.TotalCircular = dsCircular.Tables[0].Rows.Count;

                    //MarQue
                    IEnumerable<DataRow> query = from order in dsCircular.Tables[0].AsEnumerable()
                                                 where order.Field<byte>("IsMarque") == 1 && order.Field<Boolean>("IsActive") == true
                                                 select order;
                    // Create a table of Marque from the query.
                    if (query.Any())
                    {
                        obj.dsMarque = query.CopyToDataTable<DataRow>();
                        ViewBag.MarqueCount = obj.dsMarque.Rows.Count;
                    }
                    else { ViewBag.MarqueCount = 0; }

                    // circular
                    IEnumerable<DataRow> query1 = from order in dsCircular.Tables[0].AsEnumerable()
                                                  where order.Field<byte>("IsMarque") == 0 && order.Field<Boolean>("IsActive") == true
                                                  select order;
                    // Create a table of Marque from the query.
                    if (query1.Any())
                    {
                        obj.dsCircular = query1.CopyToDataTable<DataRow>();
                        ViewBag.CircularCount = obj.dsCircular.Rows.Count;

                        //
                        int count = Convert.ToInt32(dsCircular.Tables[2].Rows[0]["CircularCount"]);
                        ViewBag.TotalCircularCount = count;
                        int tp = Convert.ToInt32(count);
                        int pn = tp / 15;
                        int cal = 15 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCircularCount) - cal;
                        if (res >= 1)
                        { ViewBag.pn = pn + 1; }
                        else
                        { ViewBag.pn = pn; }


                    }
                    else
                    {
                        ViewBag.CircularCount = 0;
                        ViewBag.TotalCircularCount = 0;
                    }
                }
                #endregion

                string SCHL = Convert.ToString(Session["SCHL"]);
                string IsOpen = objDB.CheckOpenSchool(SCHL);
                Session["IsOpen"] = IsOpen;
                string outstatus = "";
                if (Length == "2")
                { ViewBag.msg = "Sorry Your UDISE Code is an Invalid,So try with different Code."; }
                if (Length == "1")
                { ViewBag.msg = "Thanks, For Submission."; }
                objDB.findUdisecode(SCHL, out outstatus);
                ViewBag.popupstatus = outstatus;

                DataSet ds = objDB.findUdisecodeWithDetails(SCHL);

                obj.udisecode = ds.Tables[0].Rows[0]["udisecode"].ToString();
                obj.TECH = ds.Tables[0].Rows[0]["TECH"].ToString();
                obj.VOC = ds.Tables[0].Rows[0]["VOC"].ToString();
                obj.SCI = ds.Tables[0].Rows[0]["SCI"].ToString();
                obj.AGRI = ds.Tables[0].Rows[0]["AGRI"].ToString();
                obj.COMM = ds.Tables[0].Rows[0]["COMM"].ToString();
                obj.HUM = ds.Tables[0].Rows[0]["HUM"].ToString();
                obj.MATRIC = ds.Tables[0].Rows[0]["MATRIC"].ToString();
                Session["IsNSQF"] = obj.NSQF_flag = Convert.ToBoolean(ds.Tables[0].Rows[0]["NSQF_flag"].ToString());

                obj.numofcomm = ds.Tables[0].Rows[0]["numofcomm"].ToString();
                obj.numofhum = ds.Tables[0].Rows[0]["numofhum"].ToString();
                obj.numofsci = ds.Tables[0].Rows[0]["numofsci"].ToString();
                obj.numoftech = ds.Tables[0].Rows[0]["numoftech"].ToString();
                obj.numofagri = ds.Tables[0].Rows[0]["numofagri"].ToString();
                obj.numofvoc = ds.Tables[0].Rows[0]["numofvoc"].ToString();
                obj.numofregular = ds.Tables[0].Rows[0]["numofregular"].ToString();
                obj.numofnsqf = ds.Tables[0].Rows[0]["numofnsqf"].ToString();
                ViewBag.popupstatus = outstatus;
                Session["IsAffiliation"] = ds.Tables[0].Rows[0]["IsAffiliation"].ToString() == "1" ? "1" : null;
                Session["IsAdditionalSection"] = ds.Tables[0].Rows[0]["IsAdditionalSection"].ToString() == "1" ? "1" : null;

                if (ds.Tables[1].Rows.Count > 0)
                {
                    Session["IsPracticalCent"] = ds.Tables[1].Rows[0]["IsPracticalCent"].ToString() == "1" ? "1" : null;
                }


                //if (ds.Tables[3].Rows.Count > 0)
                //{
                //    Session["IsAffiliation"] = ds.Tables[3].Rows[0]["IsAffiliation"].ToString() == "1" ? "1" : null;
                //}

                if (ds.Tables[4].Rows.Count > 0)
                {
                    Session["IsInfrastructureAllowed"] = ds.Tables[4].Rows[0]["IsInfrastructureAllowed"].ToString() == "Y" ? "Y" : null;
                }



                DataSet result = objCommon.schooltypes(SCHL); // passing Value to DBClass from model
                if (result == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (result.Tables[1].Rows.Count > 0)
                {
                    ViewBag.SchoolType = result.Tables[0].Rows[0]["H_UTYPE"].ToString();
                    ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                    ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                    ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
                    ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();

                    LoginSessionSchl loginSessionSchl = new LoginSessionSchl()
                    { 
                        SCHL = result.Tables[1].Rows[0]["SCHL"].ToString(),
                        SENIOR = result.Tables[1].Rows[0]["Senior"].ToString(),
                        OSENIOR = result.Tables[1].Rows[0]["OSENIOR"].ToString(),
                        MATRIC = result.Tables[1].Rows[0]["MATRIC"].ToString(),
                        OMATRIC = result.Tables[1].Rows[0]["OMATRIC"].ToString(),
                        PRIMARY = result.Tables[1].Rows[0]["Fifth"].ToString(),
                        MIDDLE = result.Tables[1].Rows[0]["Eighth"].ToString(),
                    };

                    Session["loginSessionSchl"] = loginSessionSchl;

                }

                DataSet OpenSchlds = objDB.findOPENSCHOOL(SCHL);
                if (OpenSchlds == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (OpenSchlds.Tables[0].Rows.Count > 0)
                    {
                        ViewBag.OpenSchool = OpenSchlds.Tables[0].Rows[0]["OpenSchool"].ToString();

                    }
                }


                Session["IsSchoolPremises"] = ds.Tables[0].Rows[0]["IsSchoolPremises"].ToString();


                if (ds.Tables[0].Rows[0]["IsSchoolPremises"].ToString() == "0")
                {
                    if (Session["IsInfrastructureAllowed"] != null && Session["IsInfrastructureAllowed"].ToString() == "Y")
                    {
                        return RedirectToAction("SchoolPremisesInformation", "SchoolProfile");
                    }
                }

                if (ds.Tables[5].Rows.Count > 0)
                {
                    Session["IsNewSchl"] = ds.Tables[5].Rows[0]["IsNewSchl"].ToString() == "Y" ? "Y" : null;
                    if (ds.Tables[5].Rows[0]["IsNewSchl"].ToString() == "Y")
                    {
                        return RedirectToAction("Change_Password", "SchoolProfile");
                    }
                }

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                ViewData["result"] = "ERR";
                ViewData["resultMsg"] = ex.Message;
                return View();
            }
        }



        [HttpPost]
        public ActionResult udise(Printlist obj)
        {
            if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
            {
                return RedirectToAction("Logout", "Login");
            }
            string SCHL = Convert.ToString(Session["SCHL"]);
            string udisecode = obj.udisecode;
            string outstatus = "";
            objDB.insertUdisecodeD1(SCHL, obj, out outstatus);
            if (outstatus == "2")
            { outstatus = "No"; }
            return RedirectToAction("Index", "Home", outstatus);
        }
        public ActionResult Home()
        {

            #region Circular
            Printlist obj = new Printlist(); 
            string Search = string.Empty;
            Search = "Id like '%' and CircularTypes like '%6%' and Convert(Datetime,Convert(date,ExpiryDateDD))>=Convert(Datetime,Convert(date,getdate()))";
            // Cache
            DataSet dsCircular = new DataSet();
            DataSet cacheData = HttpContext.Cache.Get("HomeLinkCircular") as DataSet;

            if (cacheData == null)
            {
                dsCircular = new AbstractLayer.AdminDB().CircularMaster(Search, 0);
                cacheData = dsCircular;
                if (cacheData != null)
                {
                    HttpContext.Cache.Insert("HomeLinkCircular", cacheData, null, DateTime.Now.AddMinutes(20), Cache.NoSlidingExpiration);
                }
            }
            else
            {
                dsCircular = cacheData;
            }
            // Cache end 

            // DataSet dsCircular = new AbstractLayer.AdminDB().CircularMaster(Search, pageIndex);//GetAllFeeMaster2016SP
            if (dsCircular == null || dsCircular.Tables[0].Rows.Count == 0)
            {
                ViewBag.TotalCircular = 0;
                 ViewBag.MarqueCount = 0; 
                ViewBag.CircularCount = 0;
                ViewBag.TotalCircularCount = 0;
            }
            else
            {
                //var type7 = dsCircular.Tables[0].Columns[7].DataType.Name.ToString();
                var type8 = dsCircular.Tables[0].Columns[9].DataType.Name.ToString();
                ViewBag.TotalCircular = dsCircular.Tables[0].Rows.Count;

                //MarQue
                IEnumerable<DataRow> query = from order in dsCircular.Tables[0].AsEnumerable()
                                             where order.Field<byte>("IsMarque") == 1 && order.Field<Boolean>("IsActive") == true
                                             select order;
                // Create a table of Marque from the query.
                if (query.Any())
                {
                    obj.dsMarque = query.CopyToDataTable<DataRow>();
                    ViewBag.MarqueCount = obj.dsMarque.Rows.Count;
                }
                else { ViewBag.MarqueCount = 0; }

                // circular
                IEnumerable<DataRow> query1 = from order in dsCircular.Tables[0].AsEnumerable()
                                              where order.Field<byte>("IsMarque") == 0 && order.Field<Boolean>("IsActive") == true
                                              select order;
                // Create a table of Marque from the query.
                if (query1.Any())
                {
                    obj.dsCircular = query1.CopyToDataTable<DataRow>();
                    ViewBag.CircularCount = obj.dsCircular.Rows.Count;

                    //
                    int count = Convert.ToInt32(dsCircular.Tables[2].Rows[0]["CircularCount"]);
                    ViewBag.TotalCircularCount = count;
                    int tp = Convert.ToInt32(count);
                    int pn = tp / 15;
                    int cal = 15 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCircularCount) - cal;
                    if (res >= 1)
                    { ViewBag.pn = pn + 1; }
                    else
                    { ViewBag.pn = pn; }


                }
                else
                {
                    ViewBag.CircularCount = 0;
                    ViewBag.TotalCircularCount = 0;
                }
            }
            #endregion
            return View(obj);
        }

        public ActionResult test1()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }


        public ActionResult DownloadMissingReport(string cls)
        {
            try
            {
                if (Request.QueryString["File"] == null)
                {
                    return RedirectToAction("CalculateFee", "Home");
                }                
                else
                {
                    string UserType = "";
                    UserType = "User";
                    string FileExport = Request.QueryString["File"].ToString();
                    DataSet ds = null;
                    if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                    {
                        return RedirectToAction("Logout", "Login");
                    }
                    else
                    {

                      

                        string fileName1 = string.Empty;
                        string Search = string.Empty;
                        Search = "SCHL=" + Session["SCHL"].ToString();
                        Search += "  and type='" + UserType + "'";
                        if (FileExport == "Excel")
                        {
                            fileName1 = Session["SCHL"].ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";  //103_230820162209_347
                        }
                        else
                        { Search += "  and form_name='" + FileExport + "'";
                            fileName1 = FileExport + '_' + Session["SCHL"].ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";
                        }

                        if (Request.QueryString["cls"] != null)
                        {
                            string selectedcls = Request.QueryString["cls"].ToString();

                            selectedcls = selectedcls == "9" ? "1" : selectedcls == "10" ? "2" : selectedcls == "11" ? "3" : selectedcls == "12" ? "4" : selectedcls;

                            Search += "  and a.class='" + selectedcls + "'";
                        }

                        //CheckFeeStatus
                        // ds = objDB.GetMissingCheckFeeStatus(Session["SCHL"].ToString(), UserType); //GetMissingCheckFeeStatusSP
                        ds = objDB.GetMissingCheckFeeStatus(Search); //GetMissingCheckFeeStatusSP
                        if (ds == null)
                        {
                            return RedirectToAction("Index", "Home");
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
                return RedirectToAction("CalculateFee", "Home");
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("CalculateFee", "Home");
            }
        }

        public ActionResult DownloadMissingReportAdmin()
        {
            try
            {
                if (Request.QueryString["File"] == null)
                {
                    return RedirectToAction("CalculateFeeAdmin", "Home");
                }
                else
                {
                    string UserType = "";
                    UserType = "User";
                    string FileExport = Request.QueryString["File"].ToString();
                    DataSet ds = null;
                    Session["SCHL"] = Request.QueryString["schl"].ToString();
                    if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                    {
                        return RedirectToAction("Logout", "Admin");
                    }
                    else
                    {
                        string fileName1 = string.Empty;
                        string Search = string.Empty;
                        Search = "SCHL=" + Session["SCHL"].ToString();
                        Search += "  and type='" + UserType + "'";
                        if (FileExport == "Excel")
                        {
                            fileName1 = Session["SCHL"].ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";  //103_230820162209_347
                        }
                        else
                        {
                            Search += "  and form_name='" + FileExport + "'";
                            fileName1 = FileExport + '_' + Session["SCHL"].ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";
                        }

                        //CheckFeeStatus
                        // ds = objDB.GetMissingCheckFeeStatus(Session["SCHL"].ToString(), UserType); //GetMissingCheckFeeStatusSP
                        ds = objDB.GetMissingCheckFeeStatus(Search); //GetMissingCheckFeeStatusSP
                        if (ds == null)
                        {
                            return RedirectToAction("Index", "Home");
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
                return RedirectToAction("CalculateFeeAdmin", "Home");
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("CalculateFeeAdmin", "Home");
            }
        }


        #region Registration CalculateFee ONLINE

        [SessionCheckFilter]
        public ActionResult CalculateFee(string id, string Status, FormCollection frc)
        {
            try
            {
                FeeHomeViewModel fhvm = new FeeHomeViewModel();
                LoginSession loginSession = (LoginSession)Session["LoginSession"];

                TempData["CalculateFee"] = null;
                TempData["AllowBanks"] = null;
                TempData["PaymentForm"] = null;
                TempData["FeeStudentList"] = null;

                ViewBag.selectedClass = "";
                return View(fhvm);
            }
            catch (Exception ex)
            {                
                return RedirectToAction("Logout", "Login");
            }

        }

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult CalculateFee(string id, string Status, FormCollection frc, string aa, string ChkId, string selectedClass)
        {
            try
            {
                FeeHomeViewModel fhvm = new FeeHomeViewModel();
                LoginSession loginSession = (LoginSession)Session["LoginSession"];

                if (Status == "Successfully" || Status == "Failure")
                {
                    //ViewData["Status"] = Status;
                    ViewData["FeeStatus"] = Status;
                    return View();
                }               
                else
                {

                    ViewBag.selectedClass = selectedClass;
                    // FormCollection frc = new FormCollection();
                    string FormNM = frc["ChkId"];
                    TempData["ChkId"] = frc["ChkId"];

                    if (FormNM == null || FormNM == "")
                    {
                        return View();
                    }

                    ViewData["Status"] = "";
                    string UserType = "User";

                    string Search = string.Empty;
                    Search = "SCHL='" + loginSession.SCHL + "'";
                    FormNM = "'" + FormNM.Replace(",", "','") + "'";
                    Search += "  and type='" + UserType + "' and form_name in(" + FormNM + ")";                    
                    string SearchString = DateTime.Now.ToString("dd/MM/yyyy");
                    DateTime date = DateTime.ParseExact(SearchString, "dd/MM/yyyy", null);


                    DataSet dsCheckFee = objDB.CheckFeeStatus(Session["SCHL"].ToString(), UserType, FormNM.ToUpper(), date);//CheckFeeStatusSPByView

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
                                ViewBag.Message = "Calculate fee is allowed only for M1 and T1 Form only";
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
                                if (dsCheckFee.Tables[1].Rows.Count > 0)
                                {
                                    string CheckExists = "";

                                    //changes date 11 Sep 2021
                                    for (int p = 0; p < dsCheckFee.Tables[1].Rows.Count; p++)
                                    {
                                        string Form_Name = dsCheckFee.Tables[1].Rows[p]["form_name"].ToString();
                                        string std_id = dsCheckFee.Tables[1].Rows[p]["std_id"].ToString();
                                        string std_Photo = dsCheckFee.Tables[1].Rows[p]["std_Photo"].ToString();
                                        string std_Sign = dsCheckFee.Tables[1].Rows[p]["std_Sign"].ToString();
                                        string Form_name = dsCheckFee.Tables[1].Rows[p]["Form_name"].ToString();
                                        string CLASS = dsCheckFee.Tables[1].Rows[p]["CLASS"].ToString();
                                        //N1/175/Photo/N1175170066311P.jpg
                                        //https://registration2022.pseb.ac.in/Upload/Upload2017/N1/175/Photo/N1175170066311P.jpg

                                        //if (!string.IsNullOrEmpty(CLASS))
                                        //{ }

                                        // if (CLASS == "2" || CLASS == "4")
                                        //if (!string.IsNullOrEmpty(CLASS))
                                        // {

                                        //     string PhotoExist = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/" + std_Photo));
                                        //     if (!System.IO.File.Exists(PhotoExist))
                                        //     {
                                        //         //Photo Not Exist
                                        //         CheckExists += std_id + ",";
                                        //     }

                                        //     string SignExist = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/" + std_Sign));
                                        //     if (!System.IO.File.Exists(SignExist))
                                        //     {
                                        //         CheckExists += std_id + ",";
                                        //         // Sign Not Exist
                                        //     }
                                        // }
                                                                       

                                        string CandPhotoTempPath = dsCheckFee.Tables[1].Rows[p]["CandPhotoTempPath"].ToString();
                                        string CandSignTempPath = dsCheckFee.Tables[1].Rows[p]["CandSignTempPath"].ToString();

                                        string PhotoExist = Path.Combine(Server.MapPath("~/Upload/"  + CandPhotoTempPath));                                    
                                        if (!System.IO.File.Exists(PhotoExist))
                                        {
                                            //Photo Not Exist
                                            CheckExists += std_id + ",";
                                        }

                                        string SignExist = Path.Combine(Server.MapPath("~/Upload/" + CandSignTempPath));                                       
                                        if (!System.IO.File.Exists(SignExist))
                                        {
                                            CheckExists += std_id + ",";
                                            // Sign Not Exist
                                        }


                                    }
                                    //changes date 11 Sep 2021

                                    if (CheckExists != "")
                                    {
                                        ViewData["FeeStatus"] = "10";
                                        string[] myArray = CheckExists.Trim().Split(',').Distinct().ToArray();
                                        ViewBag.Message = myArray;
                                        fhvm.StoreAllData = dsCheckFee;
                                        ViewBag.TotalCount = dsCheckFee.Tables[0].Rows.Count;
                                        return View(fhvm);
                                    }


                                    string cls = selectedClass;
                                    // Change after  MERITORIOUS latefee 0
                                    DataSet ds = objDB.GetCalculateFeeBySchool(cls,Search, loginSession.SCHL, date);
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
                                        Session["CalculateFee"] = ds;
                                        ViewBag.TotalCount = fhvm.StoreAllData.Tables[0].Rows.Count;
                                        Session["AllowBanks"] = ds.Tables[0].Rows[0]["AllowBanks"].ToString();
                                        fhvm.TotalFeesInWords = ds.Tables[1].Rows[0]["TotalFeesInWords"].ToString();
                                        fhvm.EndDate = ds.Tables[0].Rows[0]["EndDateDay"].ToString() + " " + ds.Tables[0].Rows[0]["FeeValidDate"].ToString();
                                        fhvm.StartDate = ds.Tables[0].Rows[0]["FeeStartDate"].ToString();
                                        //  return View(fhvm);   
                                    }
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
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }

        }



        [SessionCheckFilter]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult PaymentForm()
        {
            try
            {

                PaymentformViewModel pfvm = new PaymentformViewModel();
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                {
                    return RedirectToAction("Logout", "Login");
                }
                if (Session["CalculateFee"] == null || Session["CalculateFee"].ToString() == "")
                {
                    return RedirectToAction("CalculateFee", "Home");
                }

                // ViewBag.BankList = objCommon.GetBankList();
                string schl = string.Empty;
                schl = Session["SCHL"].ToString();
                DataSet ds = objDB.GetSchoolLotDetails(schl);
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

                    DataSet dscalFee = (DataSet)Session["CalculateFee"];
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
                    Session["PaymentForm"] = pfvm;


                    if (Convert.ToDateTime(pfvm.OfflineLastDate).Date >= DateTime.Now.Date)
                    {

                        ViewData["IsOfflineAllow"] = "1";

                    }
                    else {
                        ViewData["IsOfflineAllow"] = "0";
                    }
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
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }
        }

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult PaymentForm(PaymentformViewModel pfvm, FormCollection frm, string PayModValue, string AllowBanks, string IsOnline)
        {
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
                if (Session["CheckFormFee"].ToString() == "0")
                { pfvm.BankCode = "203"; }
               

                if (Session["PaymentForm"] == null || Session["PaymentForm"].ToString() == "")
                {
                    return RedirectToAction("PaymentForm", "Home");
                }
                if (Session["FeeStudentList"] == null || Session["FeeStudentList"].ToString() == "")
                {
                    return RedirectToAction("PaymentForm", "Home");
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
                    string SCHL = Session["SCHL"].ToString();
                    string FeeStudentList = Session["FeeStudentList"].ToString();
                    CM.FeeStudentList = FeeStudentList.Remove(FeeStudentList.LastIndexOf(","), 1);
                    PaymentformViewModel PFVMSession = (PaymentformViewModel)Session["PaymentForm"];
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

                    if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                    { CM.type = "candt"; }
                    else { CM.type = "schle"; }
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
                            return RedirectToAction("PaymentForm", "Home");
                        }
                    }

                        string SchoolMobile = "";
                    // string result = "0";
                    string result = objDB.InsertPaymentForm(CM, frm, out SchoolMobile);
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
                            //Your Challan no. {#var#} of Lot no {#var#} successfully generated and valid till Dt {#var#}. Regards PSEB
                            string Sms = "Your Challan no. " + result + " of Lot no  " + CM.LOT + " successfully generated and valid till Dt " + CM.CHLNVDATE + ". Regards PSEB";
                            //string Sms = "Your Challan no. " + result + " of Lot no  " + CM.LOT + " successfully generated and valid till Dt " + CM.CHLNVDATE + ". Regards PSEB";
                            try
                            {
                                string getSms = objCommon.gosms(SchoolMobile, Sms);
                                //string getSms = objCommon.gosms("9711819184", Sms);
                            }
                            catch (Exception) { }

                            ModelState.Clear();
                            //--For Showing Message---------//                   
                            return RedirectToAction("GenerateChallaan", "Home", new { ChallanId = result });
                        }
                    }
                }
                return RedirectToAction("PaymentForm", "Home");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                //return View(pfvm);
                return RedirectToAction("PaymentForm", "Home");
            }
        }

        public ActionResult GenerateChallaan(string ChallanId)
        {
            try
            {
                if (ChallanId == null || ChallanId == "0")
                {
                    return RedirectToAction("Index", "Home");
                }
                ChallanMasterModel CM = new ChallanMasterModel();
                // if (Session["SCHL"] == null || Session["SCHL"].ToString() == "") 
                //if ((Session["SCHL"] == null || Session["SCHL"].ToString() == "") && Convert.ToString(Session["RoleType"]).ToLower() != "admin") //-----update by Deepak
                //{
                //    return RedirectToAction("Logout", "Login");
                //}
                //if (Session["PaymentForm"] == null || Session["PaymentForm"].ToString() == "")
                //{
                //    return RedirectToAction("PaymentForm", "Home");
                //}
                //-----------Update By Deepak
                string schl = "";
                //if (Convert.ToString(Session["SCHL"]) != "")
                //{
                //    schl = Session["SCHL"].ToString();
                //}
                //-------End----------
                //string schl = Session["SCHL"].ToString();
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
                    CM.latefee = Convert.ToInt32(ds.Tables[0].Rows[0]["latefee"].ToString());
                    CM.TOTFEE = float.Parse(ds.Tables[0].Rows[0]["PaidFees"].ToString());
                    CM.FEECAT = ds.Tables[0].Rows[0]["FEECAT"].ToString();
                    CM.FEECODE = ds.Tables[0].Rows[0]["FEECODE"].ToString();
                    CM.FEEMODE = ds.Tables[0].Rows[0]["FEEMODE"].ToString();
                    CM.BANK = ds.Tables[0].Rows[0]["BANK"].ToString();
                    ViewBag.BCODE = CM.BCODE = ds.Tables[0].Rows[0]["BCODE"].ToString();
                    CM.BANKCHRG = float.Parse(ds.Tables[0].Rows[0]["BANKCHRG"].ToString());
                    CM.SchoolCode = ds.Tables[0].Rows[0]["SchoolCode"].ToString();
                    // CM.SchoolCode = Session["SCHL"].ToString();
                    CM.DIST = ds.Tables[0].Rows[0]["DIST"].ToString();
                    CM.DISTNM = ds.Tables[0].Rows[0]["DISTNM"].ToString();
                    CM.LOT = Convert.ToInt32(ds.Tables[0].Rows[0]["LOT"].ToString());
                    CM.TotalFeesInWords = ds.Tables[0].Rows[0]["TotalFeesInWords"].ToString();
                    CM.SchoolName = ds.Tables[0].Rows[0]["SchoolName"].ToString();
                    CM.DepositoryMobile = ds.Tables[0].Rows[0]["DepositoryMobile"].ToString();
                    CM.type = ds.Tables[0].Rows[0]["type"].ToString();
                    CM.APPNO = ds.Tables[0].Rows[0]["APPNO"].ToString();
                    CM.SCHLREGID = ds.Tables[0].Rows[0]["SCHLREGID"].ToString();
                    CM.SCHLCANDNM = ds.Tables[0].Rows[0]["SCHLCANDNM"].ToString();
                    if (ds.Tables[0].Rows[0]["Verified"].ToString() == "1")
                    {
                        CM.BRCODE = ds.Tables[0].Rows[0]["BRCODE"].ToString();
                        CM.BRANCH = ds.Tables[0].Rows[0]["BRANCH"].ToString();
                        CM.J_REF_NO = ds.Tables[0].Rows[0]["J_REF_NO"].ToString();
                        CM.DEPOSITDT = ds.Tables[0].Rows[0]["DEPOSITDT"].ToString();
                    }
                    //if (CM.BCODE == "203" && CM.TOTFEE == 0)
                    //{
                    //    return RedirectToAction("ExaminationPortal", "RegistrationPortal");
                    //}
                    Session["CalculateFee"] = null;
                    Session["PaymentForm"] = null;
                    Session["FeeStudentList"] = null;
                    Session["FeeStudentList"] = null;

                    return View(CM);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }
        }



        public ActionResult PaymentFormM1T1(PaymentformViewModel pfvm, FormCollection frm)
        {
            try
            {

                ChallanMasterModel CM = new ChallanMasterModel();
                pfvm.BankCode = "203";
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                {
                    return RedirectToAction("Logout", "Login");
                }
              
                if (Session["FeeStudentList"] == null || Session["FeeStudentList"].ToString() == "")
                {
                    return RedirectToAction("PaymentForm", "Home");
                }
                if (ModelState.IsValid)
                {
                    string SCHL = Session["SCHL"].ToString();
                    string FeeStudentList = Session["FeeStudentList"].ToString();
                    DataSet ds = objDB.GetSchoolLotDetails(SCHL);
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
                        pfvm.SchoolCode = ds.Tables[0].Rows[0]["schl"].ToString();
                        pfvm.SchoolName = ds.Tables[0].Rows[0]["SchoolFull"].ToString(); // Schollname with station and dist 
                        ViewBag.TotalCount = pfvm.PaymentFormData.Tables[0].Rows.Count;
                        DataSet dscalFee = (DataSet)Session["CalculateFee"];
                        pfvm.TotalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalFee"].ToString());
                        pfvm.TotalLateFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalLateFee"].ToString());
                        pfvm.TotalFinalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalFeeAmount"].ToString());
                        pfvm.TotalFeesInWords = dscalFee.Tables[1].Rows[0]["TotalFeesWords"].ToString();
                        //  pfvm.FeeDate = Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy");
                        pfvm.FeeDate = dscalFee.Tables[0].Rows[0]["FeeValidDateFormat"].ToString();
                        //TotalCandidates
                        pfvm.TotalCandidates = Convert.ToInt32(dscalFee.Tables[0].AsEnumerable().Sum(x => x.Field<int>("CountStudents")));
                        pfvm.FeeCode = dscalFee.Tables[0].Rows[0]["FeeCode"].ToString();
                        pfvm.FeeCategory = dscalFee.Tables[0].Rows[0]["FEECAT"].ToString();
                        // Session["PaymentForm"] = pfvm;  
                        CM.FeeStudentList = FeeStudentList.Remove(FeeStudentList.LastIndexOf(","), 1);
                        //PaymentformViewModel PFVMSession = (PaymentformViewModel)Session["PaymentForm"];
                        PaymentformViewModel PFVMSession = pfvm;
                        CM.FEE = Convert.ToInt32(PFVMSession.TotalFinalFees);
                        CM.TOTFEE = Convert.ToInt32(PFVMSession.TotalFinalFees);
                        CM.FEECAT = PFVMSession.FeeCategory;
                        CM.FEECODE = PFVMSession.FeeCode;
                        CM.FEEMODE = "CASH";
                        CM.BANK = pfvm.BankName;
                        CM.BCODE = pfvm.BankCode;
                        CM.BANKCHRG = PFVMSession.BankCharges;
                        CM.SchoolCode = PFVMSession.SchoolCode.ToString();
                        CM.DIST = PFVMSession.Dist.ToString();
                        CM.DISTNM = PFVMSession.District;
                        CM.LOT = PFVMSession.LOTNo;
                        CM.SCHLREGID = PFVMSession.SchoolCode.ToString();
                        if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                        { CM.type = "candt"; }
                        else { CM.type = "schle"; }
                        DateTime CHLNVDATE2;
                        if (DateTime.TryParseExact(PFVMSession.FeeDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out CHLNVDATE2))
                        {
                            CM.ChallanVDateN = CHLNVDATE2;
                        }
                        CM.CHLNVDATE = PFVMSession.FeeDate;
                        string SchoolMobile = "";
                        // string result = "0";
                        string result = objDB.InsertPaymentForm(CM, frm, out SchoolMobile);
                        if (result == "0")
                        {
                            //--------------Not saved
                            ViewData["result"] = 0;
                            return RedirectToAction("CalculateFee", "Home", new { Status = "Failure" });
                        }
                        if (result == "-1")
                        {
                            //-----alredy exist
                            ViewData["result"] = -1;
                            return RedirectToAction("CalculateFee", "Home", new { Status = "Failure" });
                        }
                        else
                        {
                            //ViewBag.Message = "File has been uploaded successfully";
                           // Your Challan no.XXXXXXXXXX of Lot no  XX successfully generated and valid till Dt XXXXXXXXXXX. Regards PSEB
                            string Sms = "Your Challan no. " + result + " of Lot no  " + CM.LOT + " successfully generated and valid till Dt " + CM.CHLNVDATE + ". Regards PSEB";
                            try
                            {
                                string getSms = objCommon.gosms(SchoolMobile, Sms);
                                // string getSms = objCommon.gosms("9711819184", Sms);
                            }
                            catch (Exception) { }

                            ModelState.Clear();
                            //--For Showing Message---------//                   
                            return RedirectToAction("CalculateFee", "Home", new { Status = "Successfully" });
                        }
                    }
                }
                //return View(pfvm);
                return View("~/Views/Home/CalculateFee.cshtml");
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }
        }

        #endregion Registration CalculateFee ONLINE

        [SessionCheckFilter]
        public ActionResult FinalPrint()
        {
            try
            {

                ChallanMasterModel CM = new ChallanMasterModel();                
                string schl = Session["SCHL"].ToString();

                DataSet ds = objDB.GetFinalPrintChallan(schl);//GetFinalPrintChallanSP
                CM.ChallanMasterData = ds;
                if (CM.ChallanMasterData == null || CM.ChallanMasterData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = CM.ChallanMasterData.Tables[0].Rows.Count;                             
                    return View(CM);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }
        }
        [SessionCheckFilter]
        public ActionResult ReGenerateChallaan(string ChallanId)
        {
            try
            {
                if (ChallanId == null)
                {
                    return RedirectToAction("PaymentForm", "Home");
                }
                ChallanMasterModel CM = new ChallanMasterModel();
                
                string schl = Session["SCHL"].ToString();
                string ChallanId1 = ChallanId.ToString();
                string Usertype = "User";
                int OutStatus;
                DataSet ds = objDB.ReGenerateChallaanById(ChallanId1, Usertype, out OutStatus);
                if (OutStatus == 1)
                {
                    Session["resultReGenerate"] = "1";
                    ViewBag.Message = "Re Generate Challaan SuccessFully";
                    return RedirectToAction("FinalPrint", "Home");
                }
                else
                {
                    Session["resultReGenerate"] = "0";
                    ViewBag.Message = "Re Generate Challaan Failure";
                    return RedirectToAction("FinalPrint", "Home");
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }

        }

        public JsonResult jqReGenerateChallaanNew(string ChallanId,string BCODE)
        {
            string ChallanIdOut = "";
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
                ChallanMasterModel CM = new ChallanMasterModel();
                //if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                //{
                //    dee = "-2";
                //    //return RedirectToAction("Index", "Home");
                //}
                //string schl = Session["SCHL"].ToString();
                string ChallanId1 = ChallanId.ToString();
                string Usertype = "User";
                int OutStatus = 0;
                
               DataSet ds = objDB.ReGenerateChallaanByIdBank(ChallanId1, BCODE, Usertype, out OutStatus, out ChallanIdOut);//ReGenerateChallaanByIdBankSP
                if (OutStatus == 1)
                {
                    dee = "1";
                }
                else
                {
                    dee = "0";
                }
            }
            catch (Exception ex)
            {              
                dee = "-3";
            }           
            return Json(new { dee = dee, chid = ChallanIdOut }, JsonRequestBehavior.AllowGet);

        }


        //Final Print strat
        #region Final Report 2017-18

        [SessionCheckFilter]
        public ActionResult FinalReport(string schl, string lot,string Challanid)
        {            
            var itemsch = new SelectList(new[]{new {ID="1",Name="ALL"},new{ID="2",Name="Unique ID"},new{ID="3",Name="Candidate Name"},
            new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="Section"},new{ID="7",Name="Regno"},new{ID="8",Name="Class RollNo."}}, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();

            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            FeeHomeViewModel FM = new FeeHomeViewModel();
            FM.StoreAllData = objDB.GetStudentFinalPrint(schl, lot, Challanid);          
            if (FM.StoreAllData == null || FM.StoreAllData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                return View();
            }
            else
            {
                ViewBag.TotalCount = FM.StoreAllData.Tables[0].Rows.Count;
                if (FM.StoreAllData.Tables[1].Rows.Count > 0)
                {
                    ViewBag.SCHL = FM.StoreAllData.Tables[1].Rows[0]["SCHL"];
                    ViewBag.IDNO = FM.StoreAllData.Tables[1].Rows[0]["IDNO"];
                    ViewBag.SSE =  FM.StoreAllData.Tables[1].Rows[0]["SchoolStationE"];
                    ViewBag.SSP =  FM.StoreAllData.Tables[1].Rows[0]["SchoolStationP"];
                    ViewBag.Principal = FM.StoreAllData.Tables[1].Rows[0]["PRINCIPAL"];
                    ViewBag.phno = FM.StoreAllData.Tables[1].Rows[0]["PHONE"];
                    ViewBag.mob = FM.StoreAllData.Tables[1].Rows[0]["MOBILE"];
                    ViewBag.DC = FM.StoreAllData.Tables[1].Rows[0]["DIST"];
                    ViewBag.DN = FM.StoreAllData.Tables[1].Rows[0]["DISTE"];
                }

                if (FM.StoreAllData.Tables[2].Rows.Count > 0)
                {
                    ViewBag.table2Count = "1";
                    ViewBag.regdt = FM.StoreAllData.Tables[2].Rows[0]["regdt"].ToString();
                }
                else
                {
                    ViewBag.table2Count = "0";
                }
                if (FM.StoreAllData.Tables[3].Rows.Count > 0)
                {
                    ViewBag.table3Count = "1";
                    ViewBag.bcode = FM.StoreAllData.Tables[3].Rows[0]["bcode"];
                    ViewBag.branch = FM.StoreAllData.Tables[3].Rows[0]["branch"];
                    ViewBag.Bank = FM.StoreAllData.Tables[3].Rows[0]["bank"].ToString();
                    ViewBag.amt = FM.StoreAllData.Tables[3].Rows[0]["fee"];
                    ViewBag.challanid = FM.StoreAllData.Tables[3].Rows[0]["challanid"];
                    ViewBag.depositdt = FM.StoreAllData.Tables[3].Rows[0]["DEPOSITDT"];
                    ViewBag.lot = FM.StoreAllData.Tables[3].Rows[0]["lot"];
                }
                else
                {
                    ViewBag.table3Count = "0";
                }

                return View(FM);
            }


            // return View();
        }

        #endregion Final Report 2017-18
        //Final Prit End
        [SessionCheckFilter]
        public ActionResult FinalPrintReport(string schl, int lot)
        {
            try
            {
                if (Session["SCHL"].ToString() != schl)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    //DataSet ds = objDB.CheckChallanIsVerified(schl, lot);
                    DataSet ds = objDB.CheckChallanIsVerifiedByFeeCode(schl, lot, 20);// 20 Feecode for REG_CONTI FEE 2016-17
                    if (ds == null || ds.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        Session["PrintLot"] = null;
                    }
                    else
                    {
                        if (ds.Tables[0].Rows[0]["VERIFIED"].ToString() == "1")
                        {
                            ViewBag.Message = "Final Print Report Downloaded SuccessFully";
                            Session["PrintLot"] = lot;
                        }
                        else
                        {
                            ViewBag.Message = "Challaan Not Verified";
                            Session["PrintLot"] = null;
                        }
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }
        }


        //Exam Portal Calculare Fee and Generate Challan

        //public ActionResult ExamCalculateFee()
        //{          
        //    if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
        //    {
        //        return RedirectToAction("Logout", "Login");
        //    }           
        //    return View();
        //}

        public ActionResult ExamCalculateFeeAdmin()
        {
            Session["ExamCalculateFee"] = null;
            if (Session["myIP"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                ViewData["FeeStatus"] = "3";

                return View();
            }
        }

        [SessionCheckFilter]
        [HttpPost]
       public ActionResult ExamCalculateFeeAdmin(FeeHomeViewModel fhvm, FormCollection frm)
        {
            try
            {
                // string id = frm["Category"].ToString();             
                string id = fhvm.formSelected;
                string dt1 = fhvm.dateSelected;
                string Search = string.Empty;
                Search = "SCHL=" + Session["SCHL"].ToString();
                DataSet ds = new DataSet();
                ViewBag.SearchId = id;

                if (id == "M" || id == "S" || id == "MO" || id == "SO")
                {
                    DateTime dateselected;
                    if (DateTime.TryParseExact(fhvm.dateSelected, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
                    {
                        //ExamCalculateFeeAdmin
                        ds = objDB.ExamCalculateFeeAdmin(Search, Session["SCHL"].ToString(), id, dateselected);
                    }
                    else
                    {
                        ViewBag.Message = "Please Selected Valid Date";
                        ViewBag.TotalCount = 0;
                        ViewData["FeeStatus"] = "3";
                        return View();
                    }

                }
                else
                {
                    return View(fhvm);
                    //return RedirectToAction("Index", "Login");
                }
                //DataSet ds = objDB.ExamReportMatricCalculateFee(Search, Session["SCHL"].ToString());
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
                    if (Session["ExamCalculateFee"] != null)
                    {
                        Session["ExamCalculateFee"] = null;
                    }

                    Session["ExamCalculateFee"] = ds;
                    ViewBag.TotalCount = fhvm.StoreAllData.Tables[0].Rows.Count;
                    fhvm.TotalFeesInWords = ds.Tables[1].Rows[0]["TotalFeesInWords"].ToString();
                    fhvm.EndDate = ds.Tables[0].Rows[0]["EndDateDay"].ToString() + " " + ds.Tables[0].Rows[0]["FeeValidDate"].ToString();
                    //  return View(fhvm);                          
                }
                return View(fhvm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }
        }


        public ActionResult ExamCalculateFeeByDate()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("Logout", "Admin");
            }

            Session["ExamCalculateFeeAdmin"] = null;
            Session["examcalfeeschl"] = null;
            Session["ExamSearchId"] = null;
            ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.Message = "Record Not Found";
            ViewBag.TotalCount = 0;
            ViewData["FeeStatus"] = "3";
            return View();
        }

        [HttpPost]        
        public ActionResult ExamCalculateFeeByDate(FeeHomeViewModel fhvm, FormCollection frm)
        {
            try
            {
                // string id = frm["Category"].ToString();    
                string schl = fhvm.schl;
                Session["examcalfeeschl"] = fhvm.schl;
                string id = fhvm.formSelected;
                string dt1 = fhvm.dateSelected;
                if (Session["UserName"] == null)
                {
                    return RedirectToAction("Logout", "Admin");
                }
                if (schl == null || schl == "")
                {
                    return RedirectToAction("ExamCalculateFeeByDate", "Home");
                }

                if (fhvm.insertDate == null || fhvm.insertDate == "")
                {
                    fhvm.insertDate = DateTime.Now.ToString("dd/MM/yyyy");
                }
                string Search = string.Empty;
                Search = "SCHL=" + schl.ToString();
                DataSet ds = new DataSet();
                ViewBag.SearchId = id;
                Session["ExamSearchId"] = id;
                if (id == "M" || id == "S" || id == "MO" || id == "SO")
                { 
                    DateTime dateselected;
                    DateTime insertdate;
                    
                    if (DateTime.TryParseExact(fhvm.dateSelected, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
                    {
                        if (DateTime.TryParseExact(fhvm.insertDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out insertdate))
                        {
                            ds = objDB.ExamReportCalculateFeeSPByDate(Search, schl.ToString(), id, dateselected, insertdate);
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Please Selected Valid Date";
                        ViewBag.TotalCount = 0;
                        ViewData["FeeStatus"] = "3";
                        return View();
                    }
                   
                }             
                else
                {
                    return View(fhvm);
                    //return RedirectToAction("Index", "Login");
                }
                //DataSet ds = objDB.ExamReportMatricCalculateFee(Search, Session["SCHL"].ToString());
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
                    if (Session["ExamCalculateFeeAdmin"] != null)
                    {
                        Session["ExamCalculateFeeAdmin"] = null;
                    }
                    Session["ExamCalculateFeeAdmin"] = ds;
                    ViewBag.TotalCount = fhvm.StoreAllData.Tables[0].Rows.Count;
                    fhvm.TotalFeesInWords = ds.Tables[1].Rows[0]["TotalFeesInWords"].ToString();
                    fhvm.EndDate = ds.Tables[0].Rows[0]["EndDateDay"].ToString() + " " + ds.Tables[0].Rows[0]["FeeValidDate"].ToString();
                    //  return View(fhvm);                          
                }
                return View(fhvm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }
        }


        [HttpPost]
        public ActionResult GenerateExamChallanByDate(string lumsumfine, string lumsumremarks, FormCollection frm, string ValidDate,string BankCode)
        {

            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];


            PaymentformViewModel pfvm = new PaymentformViewModel();
            if (Session["adminid"] == null || Session["adminid"].ToString() == "")
            {
                return RedirectToAction("Logout", "Login");
            }
            if (Session["ExamCalculateFeeAdmin"] == null || Session["ExamCalculateFeeAdmin"].ToString() == "")
            {
                return RedirectToAction("ExamCalculateFeeByDate", "Home");
            }
            if (Session["examcalfeeschl"] == null || Session["examcalfeeschl"].ToString() == "")
            {
                return RedirectToAction("ExamCalculateFeeByDate", "Home");
            }
            if (Session["ExamSearchId"] == null || Session["ExamSearchId"].ToString() == "")
            {
                return RedirectToAction("ExamCalculateFeeByDate", "Home");
            }

            string ExamSearchId = Session["ExamSearchId"].ToString();
            string schl = string.Empty;
            schl = Session["examcalfeeschl"].ToString();
            DataSet ds = objDB.GetSchoolPrintLotDetails(schl);
            pfvm.PaymentFormData = ds;
            if (pfvm.PaymentFormData == null || pfvm.PaymentFormData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                return View();
            }
            else
            {
                pfvm.ExamForm = ExamSearchId;
                pfvm.PrintLot = Convert.ToInt32(ds.Tables[0].Rows[0]["printlot"].ToString());
                pfvm.Dist = ds.Tables[0].Rows[0]["Dist"].ToString();
                pfvm.District = ds.Tables[0].Rows[0]["diste"].ToString();
                pfvm.DistrictFull = ds.Tables[0].Rows[0]["DistrictFull"].ToString();
                //pfvm.SchoolCode = Convert.ToInt32(ds.Tables[0].Rows[0]["schl"].ToString());
                pfvm.SchoolCode = ds.Tables[0].Rows[0]["schl"].ToString();
                pfvm.SchoolName = ds.Tables[0].Rows[0]["SchoolFull"].ToString(); // Schollname with station and dist 
                ViewBag.TotalCount = pfvm.PaymentFormData.Tables[0].Rows.Count;
                // Exam Calculate Fee
                DataSet dscalFee = (DataSet)Session["ExamCalculateFeeAdmin"];
                pfvm.TotalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalFee"].ToString());
                pfvm.TotalLateFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalLateFee"].ToString());
                pfvm.TotalFinalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalFeeAmount"].ToString());
                pfvm.TotalFeesInWords = dscalFee.Tables[1].Rows[0]["TotalFeesWords"].ToString();
                //  pfvm.FeeDate = Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy");
                pfvm.FeeDate = dscalFee.Tables[0].Rows[0]["FeeValidDateFormat"].ToString();
                //TotalCandidates
                // pfvm.TotalCandidates = Convert.ToInt32(dscalFee.Tables[0].AsEnumerable().Sum(x => x.Field<int>("CountStudents")));
                pfvm.TotalCandidates = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalNOS"].ToString());
                pfvm.FeeCode = dscalFee.Tables[0].Rows[0]["FeeCode"].ToString();
                pfvm.FeeCategory = dscalFee.Tables[0].Rows[0]["FEECAT"].ToString();

                // Student wise Fee
                // pfvm.totaddfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["Totalfee"].ToString());
                pfvm.totaddfee = 0;
                pfvm.totregfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["Totalfee"].ToString());
                pfvm.totfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["Totalfee"].ToString());
                pfvm.totlatefee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["Totallatefee"].ToString());
                pfvm.totpracfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalPrSubFee"].ToString());
                pfvm.totaddsubfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalAddSubFee"].ToString());
                pfvm.totadd_sub_count = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalNOAS"].ToString());
                pfvm.totprac_sub_count = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalNOPS"].ToString());
                // pfvm.StudentId = Convert.ToInt32(dscalFee.Tables[2].Rows[0]["reg16id"].ToString());
                pfvm.StudentwiseFeesDT = dscalFee.Tables[2];
                Session["ExamPaymentFormByDate"] = pfvm;

                if (pfvm.TotalFinalFees == 0 && pfvm.TotalFees == 0)
                {
                    ViewBag.CheckForm = 1; // only verify for M1 and T1 
                    Session["ExamCheckFormFeeByDate"] = 0;
                }
                else
                {
                    ViewBag.CheckForm = 0; // only verify for M1 and T1 
                    Session["ExamCheckFormFeeByDate"] = 1;
                }
            }
            //post here

            ChallanMasterModel CM = new ChallanMasterModel();
            if (string.IsNullOrEmpty(BankCode))
            {
                // pfvm.BankCode = "203";
                return RedirectToAction("ExamCalculateFeeByDate", "Home");
            }
            else
            {
                pfvm.BankCode = BankCode;
            }
            if (Session["ExamFeeStudentListAdmin"] == null || Session["ExamFeeStudentListAdmin"].ToString() == "")
            {
                return RedirectToAction("ExamCalculateFeeByDate", "Home");
            }
            if (ModelState.IsValid)
            {
                string SCHL = Session["examcalfeeschl"].ToString();
                string FeeStudentList = Session["ExamFeeStudentListAdmin"].ToString();
                CM.FeeStudentList = FeeStudentList.Remove(FeeStudentList.LastIndexOf(","), 1);
                PaymentformViewModel PFVMSession = (PaymentformViewModel)Session["ExamPaymentFormByDate"];
                CM.FEECAT = PFVMSession.FeeCategory;
                CM.FEECODE = PFVMSession.FeeCode;
                CM.FEEMODE = "CASH";
                CM.BANK = pfvm.BankName;
                CM.BCODE = pfvm.BankCode;
                CM.BANKCHRG = PFVMSession.BankCharges;
                CM.SchoolCode = PFVMSession.SchoolCode.ToString();
                CM.DIST = PFVMSession.Dist.ToString();
                CM.DISTNM = PFVMSession.District;
                CM.LOT = PFVMSession.PrintLot;  //PrintLot
                CM.SCHLREGID = PFVMSession.SchoolCode.ToString();
                // Add Fee Details                
                CM.addfee = Convert.ToInt32(PFVMSession.totaddfee);
                CM.latefee = Convert.ToInt32(PFVMSession.totlatefee);
                CM.pracfee = Convert.ToInt32(PFVMSession.totpracfee);
                CM.addsubfee = Convert.ToInt32(PFVMSession.totaddsubfee);
                CM.add_sub_count = Convert.ToInt32(PFVMSession.totadd_sub_count);
                CM.prac_sub_count = Convert.ToInt32(PFVMSession.totprac_sub_count);
                CM.regfee = Convert.ToInt32(PFVMSession.totregfee);
                CM.FEE = Convert.ToInt32(PFVMSession.TotalFinalFees);
                CM.TOTFEE = Convert.ToInt32(PFVMSession.TotalFinalFees);
                CM.APPNO = PFVMSession.SchoolCode.ToString();
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
                if (PFVMSession.ExamForm == "M")
                {
                    CM.Class = 2;
                    CM.FormType = "REG";
                }
                else if (PFVMSession.ExamForm == "MO")
                {
                    CM.Class = 2;
                    CM.FormType = "OPEN";
                }
                else if (PFVMSession.ExamForm == "S")
                {
                    CM.Class = 4;
                    CM.FormType = "REG";
                }
                else if (PFVMSession.ExamForm == "SO")
                {
                    CM.Class = 4;
                    CM.FormType = "OPEN";
                }
                else
                {
                    return RedirectToAction("ExamPaymentForm", "Home");
                }
                string result = "0";

                CM.EmpUserId = adminLoginSession.AdminEmployeeUserId;
                result = objDB.ExamInsertPaymentForm(CM, frm, out SchoolMobile, PFVMSession.StudentwiseFeesDT);
                //ExamInsertPaymentFormSP
                //string result = objDB.ExamInsertPaymentFormSPByDate(CM, frm, out SchoolMobile, PFVMSession.StudentwiseFeesDT);
                //result = objDB.InsertPaymentForm(CM, frm, out SchoolMobile);
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
                    //ViewBag.Message = "File has been uploaded successfully";
                    //Your Challan no. XXXXXXXXXX of Lot no  XX successfully generated and valid till Dt XXXXXXXXXXX. Regards PSEB
                    //string Sms = "Your Challan no. " + result + " of Lot no  " + CM.LOT + " successfully generated and valid till Dt " + CM.CHLNVDATE + ". Regards PSEB";
                    //try
                    //{
                    //    string getSms = objCommon.gosms(SchoolMobile, Sms);
                    //    // string getSms = objCommon.gosms("9711819184", Sms);
                    //}
                    //catch (Exception) { }

                    Session["ExamCalculateFeeAdmin"] = null;

                    ModelState.Clear();
                    //--For Showing Message---------//                   
                    return RedirectToAction("GenerateChallaan", "Home", new { ChallanId = result });
                }
            }
            return View();
        }



        [SessionCheckFilter]
        public ActionResult ExamCalculateFee(string id, string D)
        {
            try
            {
                Session["ExamCalculateFee"] = null;
                FeeHomeViewModel fhvm = new FeeHomeViewModel();
                
                string Search = string.Empty;
                Search = "SCHL=" + Session["SCHL"].ToString();
                DataSet ds = new DataSet();
                ds = objDB.ExamReportCountRecordsClassWise(Search, Session["SCHL"].ToString());
                if ((ds == null) || (ds.Tables[0].Rows.Count == 0 && ds.Tables[1].Rows.Count == 0 && ds.Tables[2].Rows.Count == 0 && ds.Tables[3].Rows.Count == 0))
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View(fhvm);
                }
                else
                {
                    ViewBag.MR = ds.Tables[0].Rows[0]["MR"].ToString();
                    ViewBag.SR = ds.Tables[1].Rows[0]["SR"].ToString();
                    ViewBag.MO = ds.Tables[2].Rows[0]["MO"].ToString();
                    ViewBag.SO = ds.Tables[3].Rows[0]["SO"].ToString();
                    if (ds.Tables[4].Rows[0]["DTGap"].ToString() == "0")
                    {
                        ViewBag.DateMessage = "Today, You Are Not Authorize To Calcualate Fee";
                    }
                }

                ViewBag.SearchId = id;

                if (id == "M")
                {
                    ds = objDB.ExamReportMatricCalculateFee(Search, Session["SCHL"].ToString());
                    if (D == "1")
                    {
                        Downloadexcelfile(ds);
                        ds = objDB.ExamReportMatricCalculateFee(Search, Session["SCHL"].ToString());
                    }
                }
                else if (id == "S")
                {
                    ds = objDB.ExamReportSeniorCalculateFee(Search, Session["SCHL"].ToString());
                    if (D == "1")
                    {
                        Downloadexcelfile(ds);
                        ds = objDB.ExamReportSeniorCalculateFee(Search, Session["SCHL"].ToString());
                    }
                }
                else if (id == "MO")
                {
                    ds = objDB.ExamReportMatricCalculateFeeOPEN(Search, Session["SCHL"].ToString());
                    if (D == "1")
                    {
                        Downloadexcelfile(ds);
                        ds = objDB.ExamReportMatricCalculateFeeOPEN(Search, Session["SCHL"].ToString());
                    }
                }
                else if (id == "SO")
                {
                    ds = objDB.ExamReportSeniorCalculateFeeOPEN(Search, Session["SCHL"].ToString());
                    if (D == "1")
                    {
                        Downloadexcelfile(ds);
                        ds = objDB.ExamReportSeniorCalculateFeeOPEN(Search, Session["SCHL"].ToString());
                    }

                }
                else
                {
                    return View(fhvm);
                    //return RedirectToAction("Index", "Login");
                }
                //DataSet ds = objDB.ExamReportMatricCalculateFee(Search, Session["SCHL"].ToString());
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
                    if (Session["ExamCalculateFee"] != null)
                    {
                        Session["ExamCalculateFee"] = null;
                    }

                    Session["ExamCalculateFee"] = ds;
                    ViewBag.TotalCount = fhvm.StoreAllData.Tables[0].Rows.Count;
                    fhvm.TotalFeesInWords = ds.Tables[1].Rows[0]["TotalFeesInWords"].ToString();
                    fhvm.EndDate = ds.Tables[0].Rows[0]["EndDateDay"].ToString() + " " + ds.Tables[0].Rows[0]["FeeValidDate"].ToString();
                    if (id == "MO" || id == "SO")
                    {
                        GenerateExamHODChallan(id, Session["SCHL"].ToString(), ds, null);
                    }
                    //  return View(fhvm);                          
                }
                return View(fhvm);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }
        }

        public void Downloadexcelfile(DataSet ds)
        {
            if (ds.Tables[2].Rows.Count > 0)
            {
                DataTable dt = ds.Tables[2];
                dt.Columns["reg16id"].SetOrdinal(0); dt.Columns["reg16id"].ColumnName = "UNIQUE ID";
                dt.Columns["candi_name"].SetOrdinal(1); dt.Columns["candi_name"].ColumnName = "NAME OF STUDENT";
                dt.Columns["Section"].SetOrdinal(2); dt.Columns["Section"].ColumnName = "CLASS SECTION";
                dt.Columns["classroll"].SetOrdinal(3); dt.Columns["classroll"].ColumnName = "ROLLNUMBER";
                dt.Columns["form_name"].SetOrdinal(4); dt.Columns["form_name"].ColumnName = "FORM";
                dt.Columns["Fee"].SetOrdinal(5); dt.Columns["Fee"].ColumnName = "FEE";
                dt.Columns["NOAS"].SetOrdinal(6); dt.Columns["NOAS"].ColumnName = "NO. OF ADD SUB";
                dt.Columns["AddSubFee"].SetOrdinal(7); dt.Columns["AddSubFee"].ColumnName = "ADDITIONAL FEE";
                dt.Columns["NOPS"].SetOrdinal(8); dt.Columns["NOPS"].ColumnName = "NO. OF PR SUB";
                dt.Columns["PrSubFee"].SetOrdinal(9); dt.Columns["PrSubFee"].ColumnName = "PRACTICAL FEE";
                dt.Columns["latefee"].SetOrdinal(10); dt.Columns["latefee"].ColumnName = "LATE FEE";
                dt.Columns["Totfee"].SetOrdinal(11); dt.Columns["Totfee"].ColumnName = "TOTAL FEE";
                //dt.Merge(dt2);
                string fname = DateTime.Now.ToString("ddMMyyyyHHmm");
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=ExamCalList" + fname + ".xlsx");
                using (MemoryStream MyMemoryStream = new MemoryStream())
                {
                    XLWorkbook wb = new XLWorkbook();

                    var WS = wb.Worksheets.Add(dt, "ExamCalList" + fname);
                    //WS = wb.Worksheets.Add(dt2, "AttendenceList" + fname);
                    WS.Tables.FirstOrDefault().ShowAutoFilter = false;
                    wb.SaveAs(MyMemoryStream);
                    MyMemoryStream.WriteTo(Response.OutputStream);
                    WS.AutoFilter.Enabled = false;
                    Response.Flush();
                    Response.End();
                }
            }
        }







        [SessionCheckFilter]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ExamPaymentForm(string id)
        {
            try
            {

                ViewBag.PayFormId = id;
                PaymentformViewModel pfvm = new PaymentformViewModel();
                
                if (Session["ExamCalculateFee"] == null || Session["ExamCalculateFee"].ToString() == "")
                {
                    return RedirectToAction("ExamCalculateFee", "Home");
                }
                string schl = string.Empty;
                schl = Session["SCHL"].ToString();
                DataSet ds = objDB.GetSchoolPrintLotDetails(schl);
                pfvm.PaymentFormData = ds;
                if (pfvm.PaymentFormData == null || pfvm.PaymentFormData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    pfvm.ExamForm = id;
                    pfvm.PrintLot = Convert.ToInt32(ds.Tables[0].Rows[0]["printlot"].ToString());
                    pfvm.Dist = ds.Tables[0].Rows[0]["Dist"].ToString();
                    pfvm.District = ds.Tables[0].Rows[0]["diste"].ToString();
                    pfvm.DistrictFull = ds.Tables[0].Rows[0]["DistrictFull"].ToString();
                    //pfvm.SchoolCode = Convert.ToInt32(ds.Tables[0].Rows[0]["schl"].ToString());
                    pfvm.SchoolCode = ds.Tables[0].Rows[0]["schl"].ToString();
                    pfvm.SchoolName = ds.Tables[0].Rows[0]["SchoolFull"].ToString(); // Schollname with station and dist 
                    ViewBag.TotalCount = pfvm.PaymentFormData.Tables[0].Rows.Count;
                    // Exam Calculate Fee
                    DataSet dscalFee = (DataSet)Session["ExamCalculateFee"];
                    pfvm.TotalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalFee"].ToString());
                    pfvm.TotalLateFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalLateFee"].ToString());
                    pfvm.TotalFinalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalFeeAmount"].ToString());
                    pfvm.TotalFeesInWords = dscalFee.Tables[1].Rows[0]["TotalFeesWords"].ToString();
                    //  pfvm.FeeDate = Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy");
                    pfvm.FeeDate = dscalFee.Tables[0].Rows[0]["FeeValidDateFormat"].ToString();
                    pfvm.OfflineLastDate = dscalFee.Tables[0].Rows[0]["OfflineLastDate"].ToString();
                    pfvm.StartDate = dscalFee.Tables[0].Rows[0]["FeeStartDate"].ToString();
                    //TotalCandidates
                    // pfvm.TotalCandidates = Convert.ToInt32(dscalFee.Tables[0].AsEnumerable().Sum(x => x.Field<int>("CountStudents")));
                    pfvm.TotalCandidates = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalNOS"].ToString());
                    pfvm.FeeCode = dscalFee.Tables[0].Rows[0]["FeeCode"].ToString();
                    pfvm.FeeCategory = dscalFee.Tables[0].Rows[0]["FEECAT"].ToString();

                    // Student wise Fee
                    // pfvm.totaddfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["Totalfee"].ToString());
                    pfvm.totaddfee = 0;
                    pfvm.totregfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["Totalfee"].ToString());
                    pfvm.totfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["Totalfee"].ToString());
                    pfvm.totlatefee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["Totallatefee"].ToString());
                    pfvm.totpracfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalPrSubFee"].ToString());
                    pfvm.totaddsubfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalAddSubFee"].ToString());
                    pfvm.totadd_sub_count = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalNOAS"].ToString());
                    pfvm.totprac_sub_count = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalNOPS"].ToString());
                    // pfvm.StudentId = Convert.ToInt32(dscalFee.Tables[2].Rows[0]["reg16id"].ToString());
                    pfvm.StudentwiseFeesDT = dscalFee.Tables[2];
                    Session["ExamPaymentForm"] = pfvm;

                    if (pfvm.TotalFinalFees == 0 && pfvm.TotalFees == 0)
                    {
                        ViewBag.CheckForm = 1; // only verify for M1 and T1 
                        Session["ExamCheckFormFee"] = 0;
                    }
                    else
                    {
                        ViewBag.CheckForm = 0; // only verify for M1 and T1 
                        Session["ExamCheckFormFee"] = 1;
                    }
                    return View(pfvm);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }
        }

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult ExamPaymentForm(PaymentformViewModel pfvm, FormCollection frm, string PayModValue, string AllowBanks, string IsOnline)
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
            if (Session["ExamCheckFormFee"].ToString() == "0")
            { pfvm.BankCode = "203"; }


           
            if (Session["ExamPaymentForm"] == null || Session["ExamPaymentForm"].ToString() == "")
            {
                return RedirectToAction("ExamPaymentForm", "Home");
            }
            if (Session["ExamStudentList"] == null || Session["ExamStudentList"].ToString() == "")
            {
                return RedirectToAction("ExamPaymentForm", "Home");
            }


            //
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
            //

            if (ModelState.IsValid)
            {
                string SCHL = Session["SCHL"].ToString();
                string FeeStudentList = Session["ExamStudentList"].ToString();
                CM.FeeStudentList = FeeStudentList.Remove(FeeStudentList.LastIndexOf(","), 1);
                PaymentformViewModel PFVMSession = (PaymentformViewModel)Session["ExamPaymentForm"];               
                CM.FEECAT = PFVMSession.FeeCategory;
                CM.FEECODE = PFVMSession.FeeCode;
              //  CM.FEEMODE = "CASH";
                CM.BANK = pfvm.BankName;
                CM.BCODE = pfvm.BankCode;
                CM.BANKCHRG = PFVMSession.BankCharges;
                CM.SchoolCode = PFVMSession.SchoolCode.ToString();
                CM.DIST = PFVMSession.Dist.ToString();
                CM.DISTNM = PFVMSession.District;
                CM.LOT = PFVMSession.PrintLot;  //PrintLot
                CM.SCHLREGID = PFVMSession.SchoolCode.ToString();
                // Add Fee Details
                CM.addfee = Convert.ToInt32(PFVMSession.totaddfee);
                CM.latefee = Convert.ToInt32(PFVMSession.totlatefee);
                CM.pracfee = Convert.ToInt32(PFVMSession.totpracfee);
                CM.addsubfee = Convert.ToInt32(PFVMSession.totaddsubfee);
                CM.add_sub_count = Convert.ToInt32(PFVMSession.totadd_sub_count);
                CM.prac_sub_count = Convert.ToInt32(PFVMSession.totprac_sub_count);
                CM.regfee = Convert.ToInt32(PFVMSession.totregfee);
                CM.FEE = Convert.ToInt32(PFVMSession.TotalFinalFees);
                CM.TOTFEE = Convert.ToInt32(PFVMSession.TotalFinalFees);
                CM.APPNO = PFVMSession.SchoolCode.ToString();
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                { CM.type = "candt"; }
                else { CM.type = "schle"; }
                DateTime CHLNVDATE2;
                if (DateTime.TryParseExact(PFVMSession.FeeDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out CHLNVDATE2))
                {
                    CM.ChallanVDateN = CHLNVDATE2;
                }
                CM.CHLNVDATE = PFVMSession.FeeDate;
                string SchoolMobile = "";

                if (PFVMSession.ExamForm == "M")
                {
                    CM.Class = 2;
                    CM.FormType = "REG";
                }
                else if (PFVMSession.ExamForm == "MO")
                {
                    CM.Class = 2;
                    CM.FormType = "OPEN";
                }
                else if (PFVMSession.ExamForm == "S")
                {
                    CM.Class = 4;
                    CM.FormType = "REG";
                }
                else if (PFVMSession.ExamForm == "SO")
                {
                    CM.Class = 4;
                    CM.FormType = "OPEN";
                }
                else
                {
                    return RedirectToAction("ExamPaymentForm", "Home");
                }
                

                //string result = "";
                string result = objDB.ExamInsertPaymentForm(CM, frm, out SchoolMobile, PFVMSession.StudentwiseFeesDT);
                if (result == "0")
                {
                    //--------------Not saved
                    ViewData["result"] = 0;
                }
                else if (result == "-1")
                {
                    //-----alredy exist
                    ViewData["result"] = -1;
                }
                else
                {
                    Session["ExamCalculateFee"] = null;
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


                    
                    
                    //--For Showing Message---------//                   
                    return RedirectToAction("GenerateChallaan", "Home", new { ChallanId = result });
                }
            }
            return View(pfvm);
        }

        [SessionCheckFilter]
        public ActionResult FinalExamPortalReport(string schl, int? printlot, int? Class,string Type)
        {
            try
            {
            if (Session["SCHL"].ToString() != schl)
            {
                return RedirectToAction("Logout", "Login");
            }
            else
            {
                DataSet ds = objDB.CheckChallanIsVerifiedByFeeCode(schl, Convert.ToInt32(printlot),21);// 21 Feecode for 
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    // Session["ExamPrintLot"] = null;
                    ViewBag.PrintLot = 0;
                }
                else
                {
                    if (ds.Tables[0].Rows[0]["VERIFIED"].ToString() == "1")
                    {
                        ViewBag.Message = "Final Exam Print Report Downloaded SuccessFully";
                       // Session["ExamPrintLot"] = printlot;
                        ViewBag.PrintLot = printlot;
                        ViewBag.Class = Class;
                        ViewBag.Type = Type;
                        
                    }
                    else
                    {
                        ViewBag.Message = "Challaan Not Verified";
                        //  Session["ExamPrintLot"] = null;
                        ViewBag.PrintLot = 0;
                    }
                }
                return View();
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return RedirectToAction("Logout", "Login");
            }
        }


        public ActionResult GenerateExamHODChallan(string id, string SCHL, DataSet ds1, FormCollection frm)
        {
            ChallanMasterModel CM = new ChallanMasterModel();
            Session["ExamCalculateFee"] = ds1;

            var StudentList = "";
            for (int i = 0; i < ds1.Tables[0].Rows.Count; i++)
            {
                StudentList += ds1.Tables[0].Rows[i]["StudentList"].ToString() + ",";
            }
            Session["ExamStudentList"] = StudentList.ToString();

            PaymentformViewModel pfvm = new PaymentformViewModel();
            DataSet ds = objDB.GetSchoolPrintLotDetails(SCHL);
            pfvm.PaymentFormData = ds;
            if (pfvm.PaymentFormData == null || pfvm.PaymentFormData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                return View();
            }
            else
            {
                pfvm.ExamForm = id;
                pfvm.PrintLot = Convert.ToInt32(ds.Tables[0].Rows[0]["printlot"].ToString());
                pfvm.Dist = ds.Tables[0].Rows[0]["Dist"].ToString();
                pfvm.District = ds.Tables[0].Rows[0]["diste"].ToString();
                pfvm.DistrictFull = ds.Tables[0].Rows[0]["DistrictFull"].ToString();
                //pfvm.SchoolCode = Convert.ToInt32(ds.Tables[0].Rows[0]["schl"].ToString());
                pfvm.SchoolCode = ds.Tables[0].Rows[0]["schl"].ToString();
                pfvm.SchoolName = ds.Tables[0].Rows[0]["SchoolFull"].ToString(); // Schollname with station and dist 
                ViewBag.TotalCount = pfvm.PaymentFormData.Tables[0].Rows.Count;
                // Exam Calculate Fee
                DataSet dscalFee = (DataSet)Session["ExamCalculateFee"];
                pfvm.TotalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalFee"].ToString());
                pfvm.TotalLateFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalLateFee"].ToString());
                pfvm.TotalFinalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalFeeAmount"].ToString());
                pfvm.TotalFeesInWords = dscalFee.Tables[1].Rows[0]["TotalFeesWords"].ToString();
                //  pfvm.FeeDate = Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy");
                pfvm.FeeDate = dscalFee.Tables[0].Rows[0]["FeeValidDateFormat"].ToString();
                //TotalCandidates
                // pfvm.TotalCandidates = Convert.ToInt32(dscalFee.Tables[0].AsEnumerable().Sum(x => x.Field<int>("CountStudents")));
                pfvm.TotalCandidates = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalNOS"].ToString());
                pfvm.FeeCode = dscalFee.Tables[0].Rows[0]["FeeCode"].ToString();
                pfvm.FeeCategory = dscalFee.Tables[0].Rows[0]["FEECAT"].ToString();

                // Student wise Fee
                // pfvm.totaddfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["Totalfee"].ToString());
                pfvm.totaddfee = 0;
                pfvm.totregfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["Totalfee"].ToString());
                pfvm.totfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["Totalfee"].ToString());
                pfvm.totlatefee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["Totallatefee"].ToString());
                pfvm.totpracfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalPrSubFee"].ToString());
                pfvm.totaddsubfee = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalAddSubFee"].ToString());
                pfvm.totadd_sub_count = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalNOAS"].ToString());
                pfvm.totprac_sub_count = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalNOPS"].ToString());
                // pfvm.StudentId = Convert.ToInt32(dscalFee.Tables[2].Rows[0]["reg16id"].ToString());
                pfvm.StudentwiseFeesDT = dscalFee.Tables[2];
                Session["ExamPaymentForm"] = pfvm;

                if (pfvm.TotalFinalFees == 0 && pfvm.TotalFees == 0)
                {
                    ViewBag.CheckForm = 1; // only verify for M1 and T1 
                    Session["ExamCheckFormFee"] = 0;
                }
                else
                {
                    ViewBag.CheckForm = 0; // only verify for M1 and T1 
                    Session["ExamCheckFormFee"] = 1;
                }                
            }


            pfvm.BankCode = "203";

            
            if (Session["ExamPaymentForm"] == null || Session["ExamPaymentForm"].ToString() == "")
            {
                return RedirectToAction("ExamPaymentForm", "Home");
            }
            if (Session["ExamStudentList"] == null || Session["ExamStudentList"].ToString() == "")
            {
                return RedirectToAction("ExamPaymentForm", "Home");
            }
             
                string FeeStudentList = Session["ExamStudentList"].ToString();
                CM.FeeStudentList = FeeStudentList.Remove(FeeStudentList.LastIndexOf(","), 1);
                PaymentformViewModel PFVMSession = (PaymentformViewModel)Session["ExamPaymentForm"];
                CM.FEECAT = PFVMSession.FeeCategory;
                CM.FEECODE = PFVMSession.FeeCode;
                CM.FEEMODE = "CASH";
                CM.BANK = pfvm.BankName;
                CM.BCODE = pfvm.BankCode;
                CM.BANKCHRG = 0;
            CM.SchoolCode = PFVMSession.SchoolCode.ToString();
                CM.DIST = PFVMSession.Dist.ToString();
                CM.DISTNM = PFVMSession.District;
                CM.LOT = PFVMSession.PrintLot;  //PrintLot
                CM.SCHLREGID = PFVMSession.SchoolCode.ToString();
                // Add Fee Details
                CM.addfee = Convert.ToInt32(0);
                CM.latefee = Convert.ToInt32(0);
            CM.pracfee = Convert.ToInt32(0);
            CM.addsubfee = Convert.ToInt32(0);
            CM.add_sub_count = Convert.ToInt32(0);
            CM.prac_sub_count = Convert.ToInt32(0);
            CM.regfee = Convert.ToInt32(0);
            CM.FEE = Convert.ToInt32(0);
            CM.TOTFEE = Convert.ToInt32(0);
            //
                if (Session["SCHL"] == null || Session["SCHL"].ToString() == "")
                { CM.type = "candt"; }
                else { CM.type = "schle"; }

                DateTime CHLNVDATE2;
                if (DateTime.TryParseExact(PFVMSession.FeeDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out CHLNVDATE2))
                {
                    CM.ChallanVDateN = CHLNVDATE2;
                }
                CM.CHLNVDATE = PFVMSession.FeeDate;
                string SchoolMobile = "";

                if (PFVMSession.ExamForm == "M")
                {
                    CM.Class = 2;
                    CM.FormType = "REG";
                }
                else if (PFVMSession.ExamForm == "MO")
                {
                    CM.Class = 2;
                    CM.FormType = "OPEN";
                }
                else if (PFVMSession.ExamForm == "S")
                {
                    CM.Class = 4;
                    CM.FormType = "REG";
                }
                else if (PFVMSession.ExamForm == "SO")
                {
                    CM.Class = 4;
                    CM.FormType = "OPEN";
                }
                else
                {
                    return RedirectToAction("ExamPaymentForm", "Home");
                }

                //string result = "";
                string result = objDB.ExamInsertPaymentForm(CM, frm, out SchoolMobile, PFVMSession.StudentwiseFeesDT);
                if (result == "0")
                {
                Session["OpenExamCalculateFee"] = result;
                //--------------Not saved
                ViewData["result"] = 0;
                }
                else if (result == "-1")
                {
                Session["OpenExamCalculateFee"] = result;
                //-----alredy exist
                ViewData["result"] = -1;
                }
                else
                {
                    Session["ExamCalculateFee"] = null;
                    Session["OpenExamCalculateFee"] = 1;
                    //ViewBag.Message = "File has been uploaded successfully";
                    //Your Challan no. XXXXXXXXXX of Lot no  XX successfully generated and valid till Dt XXXXXXXXXXX. Regards PSEB

                    string Sms = "Your Challan no. " + result + " of Lot no  " + CM.LOT + " successfully generated and valid till Dt " + CM.CHLNVDATE + ". Regards PSEB";
                    try
                    {
                        // string getSms = objCommon.gosms(SchoolMobile, Sms);
                        //string getSms = objCommon.gosms("9711819184", Sms);
                    }
                    catch (Exception) { }
                return RedirectToAction("GenerateChallaan", "Home", new { ChallanId = result });

            }
            return RedirectToAction("GenerateChallaan", "Home", new { ChallanId = result });
      }

        #region registration CalculateFeeAdmin
      
        public ActionResult CalculateFeeAdmin(string Status)
        {
            Session["calfeeschl"] = null;
            ViewBag.Schl = "";
            ViewBag.SchlName = "";
            ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");
            if (Session["Adminid"] == null || Session["Adminid"].ToString() == "")
            { return RedirectToAction("Logout", "Login"); }
            ViewData["Status"] = "";
            FeeHomeViewModel fhvm = new FeeHomeViewModel();
            ViewData["FeeStatus"] = null;
            return View(fhvm);
        }

        [HttpPost]
        public ActionResult CancelFormForA(string id)
        {
            return RedirectToAction("CalculateFeeAdmin", "Home");
        }

        [HttpPost]
        public ActionResult CalculateFeeAdmin(string Status, string SearchString, string cmd, string schl, FormCollection frc, FeeHomeViewModel fhvm)
        {
            string id = "";

          
            ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");
            DateTime date = DateTime.ParseExact(SearchString, "dd/MM/yyyy", null);
            ViewBag.Searchstring = SearchString;
            if (schl == null || schl == "")
            { return RedirectToAction("CalculateFeeAdmin", "Home"); }
            if (Status == "Successfully")
            {
                //ViewData["Status"] = Status;
                ViewData["FeeStatus"] = Status;
                return View();
            }
            else
            {

                string FormNM = frc["ChkId"];
                if (FormNM == null || FormNM == "")
                {
                    return View();
                }


                ViewBag.Schl = schl.ToString();             
                Session["calfeeschl"] = schl.ToString();
                ViewData["Status"] = "";
                ViewData["FeeStatus"] = "";
                string UserType = "";
             
                UserType = fhvm.Type;
                

                string Search = string.Empty;
                Search = "SCHL=" + schl.ToString();
                FormNM = "'" + FormNM.Replace(",", "','") + "'";
                Search += "  and type='" + UserType + "' and form_name in(" + FormNM + ")";
                 DataSet dsCheckFee = objDB.CheckFeeStatus(schl.ToString(), UserType, FormNM.ToUpper(), date);


                //string Search = string.Empty;
                //Search = "SCHL=" + schl.ToString();
                //Search += "  and type='" + UserType + "'";             
                //CheckFeeStatus
                //DataSet dsCheckFee = objDB.CheckFeeStatus(schl.ToString(), UserType, id, date);
                if (dsCheckFee == null)
                {
                    return RedirectToAction("Index", "Home");
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
                            ViewBag.Message = "Calculate fee is allowed only for M1 and T1 Form only";
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
                            //Allowed
                            //Generate Fee
                            // DataSet ds = objDB.GetCalculateFeeBySchool(Search);

                            if (dsCheckFee.Tables[1].Rows.Count > 0)
                            {
                                string CheckExists = "";

                                //changes date 11 Sep 2021
                                for (int p = 0; p < dsCheckFee.Tables[1].Rows.Count; p++)
                                {
                                    string Form_Name = dsCheckFee.Tables[1].Rows[p]["form_name"].ToString();
                                    string std_id = dsCheckFee.Tables[1].Rows[p]["std_id"].ToString();
                                    string std_Photo = dsCheckFee.Tables[1].Rows[p]["std_Photo"].ToString();
                                    string std_Sign = dsCheckFee.Tables[1].Rows[p]["std_Sign"].ToString();
                                    string Form_name = dsCheckFee.Tables[1].Rows[p]["Form_name"].ToString();
                                    string cls = dsCheckFee.Tables[1].Rows[p]["CLASS"].ToString();
                                    //N1/175/Photo/N1175170066311P.jpg
                                    //https://registration2022.pseb.ac.in/Upload/Upload2017/N1/175/Photo/N1175170066311P.jpg

                                    //if (!string.IsNullOrEmpty(CLASS))
                                    //{ }

                                    // if (CLASS == "2" || CLASS == "4")
                                    //if (!string.IsNullOrEmpty(CLASS))
                                    // {

                                    //     string PhotoExist = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/" + std_Photo));
                                    //     if (!System.IO.File.Exists(PhotoExist))
                                    //     {
                                    //         //Photo Not Exist
                                    //         CheckExists += std_id + ",";
                                    //     }

                                    //     string SignExist = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/" + std_Sign));
                                    //     if (!System.IO.File.Exists(SignExist))
                                    //     {
                                    //         CheckExists += std_id + ",";
                                    //         // Sign Not Exist
                                    //     }
                                    // }


                                    string CandPhotoTempPath = dsCheckFee.Tables[1].Rows[p]["CandPhotoTempPath"].ToString();
                                    string CandSignTempPath = dsCheckFee.Tables[1].Rows[p]["CandSignTempPath"].ToString();

                                    // string PhotoExist1 = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/" + std_Photo));

                                    string PhotoExist = Path.Combine(Server.MapPath("~/Upload/" + CandPhotoTempPath));
                                    //  string PhotoExist = CandPhotoFullPath;
                                    if (!System.IO.File.Exists(PhotoExist))
                                    {
                                        //Photo Not Exist
                                        CheckExists += std_id + ",";
                                    }

                                    string SignExist = Path.Combine(Server.MapPath("~/Upload/" + CandSignTempPath));

                                    if (!System.IO.File.Exists(SignExist))
                                    {
                                        CheckExists += std_id + ",";
                                        // Sign Not Exist
                                    }


                                }
                                //changes date 11 Sep 2021




                                if (CheckExists != "")
                                {
                                    ViewData["FeeStatus"] = "10";
                                    string[] myArray = CheckExists.Trim().Split(',').Distinct().ToArray();
                                    ViewBag.Message = myArray;
                                    fhvm.StoreAllData = dsCheckFee;
                                    ViewBag.TotalCount = dsCheckFee.Tables[0].Rows.Count;
                                    return View(fhvm);
                                }
                                // Change after  MERITORIOUS latefee 0
                                DataSet ds = objDB.GetCalculateFeeBySchoolAdmin(id, Search, schl.ToString(), date);
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
                                    Session["CalculateFeeAdmin"] = ds;
                                    ViewBag.TotalCount = fhvm.StoreAllData.Tables[0].Rows.Count;
                                    fhvm.TotalFeesInWords = ds.Tables[1].Rows[0]["TotalFeesInWords"].ToString();
                                    fhvm.EndDate = ds.Tables[0].Rows[0]["EndDateDay"].ToString() + " " + ds.Tables[0].Rows[0]["FeeValidDate"].ToString();
                                    //  return View(fhvm);     
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                return View(fhvm);
            }
        }

        [HttpPost]
        public ActionResult DeeTest(string lumsumfine, string lumsumremarks, FormCollection frm, string ValidDate)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];


            PaymentformViewModel pfvm = new PaymentformViewModel();
            if (Session["adminid"] == null || Session["adminid"].ToString() == "")
            {
                return RedirectToAction("Logout", "Login");
            }
            if (Session["CalculateFeeAdmin"] == null || Session["CalculateFeeAdmin"].ToString() == "")
            {
                return RedirectToAction("CalculateFeeAdmin", "Home");
            }
            if (Session["calfeeschl"] == null || Session["calfeeschl"].ToString() == "")
            {
                return RedirectToAction("CalculateFeeAdmin", "Home");
            }
            
            // ViewBag.BankList = objCommon.GetBankList();
            string schl = string.Empty;
            schl = Session["calfeeschl"].ToString();
            DataSet ds = objDB.GetSchoolLotDetails(schl);
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
                DataSet dscalFee = (DataSet)Session["CalculateFeeAdmin"];
                pfvm.TotalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalFee"].ToString());
                pfvm.TotalLateFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalLateFee"].ToString());
                pfvm.TotalFinalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["TotalFeeAmount"].ToString());
                pfvm.TotalFeesInWords = dscalFee.Tables[1].Rows[0]["TotalFeesWords"].ToString();
                //  pfvm.FeeDate = Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy");
                pfvm.FeeDate = dscalFee.Tables[0].Rows[0]["FeeValidDateFormat"].ToString();
                //TotalCandidates
                pfvm.TotalCandidates = Convert.ToInt32(dscalFee.Tables[0].AsEnumerable().Sum(x => x.Field<int>("CountStudents")));
                pfvm.FeeCode = dscalFee.Tables[0].Rows[0]["FeeCode"].ToString();
                pfvm.FeeCategory = dscalFee.Tables[0].Rows[0]["FEECAT"].ToString();
                //Session["PaymentForm"] = pfvm;

                //string CheckFee = ds.Tables[1].Rows[0]["TotalFeeAmount"].ToString();
                if (pfvm.TotalFinalFees == 0 && pfvm.TotalFees == 0)
                {
                    ViewBag.CheckForm = 1; // only verify for M1 and T1 
                    Session["CheckFormFeeAdmin"] = 0;
                }
                else
                {
                    ViewBag.CheckForm = 0; // only verify for M1 and T1 
                    Session["CheckFormFeeAdmin"] = 1;
                }
            }
            //post here

            ChallanMasterModel CM = new ChallanMasterModel();
            pfvm.BankCode = "203";           
            if (Session["FeeStudentListAdmin"] == null || Session["FeeStudentListAdmin"].ToString() == "")
            {
                return RedirectToAction("CalculateFeeAdmin", "Home");
            }
            if (ModelState.IsValid)
            {
                string SCHL = Session["calfeeschl"].ToString();
                string FeeStudentList = Session["FeeStudentListAdmin"].ToString();
                CM.FeeStudentList = FeeStudentList.Remove(FeeStudentList.LastIndexOf(","), 1);                
                CM.FEE = Convert.ToInt32(pfvm.TotalFinalFees);
                CM.TOTFEE = Convert.ToInt32(pfvm.TotalFinalFees);
                CM.regfee = Convert.ToInt32(pfvm.TotalFees);
                CM.latefee = Convert.ToInt32(pfvm.TotalLateFees); 
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
                result = objDB.InsertPaymentForm(CM, frm, out SchoolMobile);
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
                    //ViewBag.Message = "File has been uploaded successfully";
                    //Your Challan no. XXXXXXXXXX of Lot no  XX successfully generated and valid till Dt XXXXXXXXXXX. Regards PSEB
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
        #endregion registration CalculateFeeAdmin

        //--------------------Exam Center------------------------------//
        public ActionResult SampleDocuments()
        {
            return View();
        }
        //----------------------------End-------------------------------//
        // Cancel Challan
        public JsonResult CancelOfflineChallanBySchl(string cancelremarks, string challanid, string Type)
        {
            try
            {
                string dee = "";
                string outstatus = "";
                string SCHL = Convert.ToString(Session["SCHL"]);
                objCommon.CancelOfflineChallanBySchl(cancelremarks, challanid, out outstatus, SCHL, Type);//ChallanDetailsCancelSP               
                dee = outstatus;
                return Json(new { sn = dee, chid = challanid }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { sn = "-1", chid = challanid }, JsonRequestBehavior.AllowGet);
            }
        }



        #region OpenSchoolAccreditation User Report
        [Route("OpenSchoolAccreditationUserReport")]
        [Route("Home/OpenSchoolAccreditationUserReport")]
        public ActionResult OpenSchoolAccreditationUserReport()
        {
            string districts = string.Empty;
            // Dist Allowed
            string DistAllow = "";
            if (ViewBag.DistAllow != null)
            {
                DistAllow = ViewBag.DistAllow;
            }
            List<SelectListItem> OpenDistricts = objDB.OpenSchoolDistricts();
            if (ViewBag.DistUser == null || ViewBag.DistAllow == null)
            {
                ViewBag.Districts = new AbstractLayer.OpenDB().GetDistrict();
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
        [Route("OpenSchoolAccreditationUserReport")]
        [Route("Home/OpenSchoolAccreditationUserReport")]
        [HttpPost]
        public ActionResult OpenSchoolAccreditationUserReport(FormCollection fc)
        {
            string DistAllow = "";
            if (ViewBag.DistAllow != null)
            {
                DistAllow = ViewBag.DistAllow;
            }
            if (ViewBag.DistUser == null || ViewBag.DistAllow == null)
            {
                ViewBag.Districts = new AbstractLayer.OpenDB().GetDistrict();
            }
            else
            {
                ViewBag.Districts = ViewBag.DistUser;
            }
            ViewBag.SelectedDist = fc["ddlDist"] != null ? fc["ddlDist"].ToString() : string.Empty;

            if (fc["ddlDist"] != null)
            {
                //var obj = new AbstractLayer.AdminDB().GetSchoolRecords1819(fc["ddlDist"].ToString());
                var obj = new AbstractLayer.ReportDB().OpenSchoolAccreditationReport(fc["ddlDist"].ToString());
                ViewBag.data = obj;


            }
            return View();
        }

        #endregion OpenSchoolAccreditation User Report


        #region UndertakingOfQuestionPapers

        [HttpGet]
        public ActionResult UndertakingOfQuestionPapers(UndertakingOfQuestionPapers undertakingOfQuestionPapers)
        {
            if (Session["LoginSession"] == null && Session["CenterHeadLoginSession"] == null)
            {
                return RedirectToAction("Logout", "Login");
            }

            if (Session["LoginSession"] != null)
            {
                LoginSession loginSession = (LoginSession)Session["LoginSession"];
                undertakingOfQuestionPapers.CentCode = loginSession.SCHL;
                undertakingOfQuestionPapers.ClassList = new AbstractLayer.DBClass().GetAllPSEBCLASS().Where(s => s.Value == "8").ToList();
                undertakingOfQuestionPapers.CentNM = loginSession.SCHLNME;
                undertakingOfQuestionPapers.CentHeadNM = loginSession.PRINCIPAL;
                undertakingOfQuestionPapers.CentMobile = loginSession.MOBILE;
            }

            undertakingOfQuestionPapers.StatusList = new AbstractLayer.DBClass().GetYesNoText();

            int currentMonth = DateTime.Now.Month;
            int currentYear = DateTime.Now.Year;

            if (currentMonth > 3)
            {
                undertakingOfQuestionPapers.MonthList = DBClass.GetMonthNameNumber().Where(s => Convert.ToInt32(s.Value) < currentMonth).ToList();
            }
            else
            {
                undertakingOfQuestionPapers.MonthList = DBClass.GetMonthNameNumber().Where(s => Convert.ToInt32(s.Value) <= currentMonth).ToList();
            }



            if (currentMonth > 3)
            {
                undertakingOfQuestionPapers.YearList = DBClass.GetSessionSingle().Where(s => Convert.ToInt32(s.Value) == currentYear).ToList();
            }
            else
            {
                undertakingOfQuestionPapers.YearList = DBClass.GetSessionSingle().Where(s => Convert.ToInt32(s.Value) <= currentYear).ToList();
            }

            undertakingOfQuestionPapers.UndertakingOfQuestionPapersList = _context.UndertakingOfQuestionPapers.AsNoTracking().Where(s => s.CentCode == undertakingOfQuestionPapers.CentCode && s.IsActive == true).OrderByDescending(s => s.QPID).ToList();

            return View(undertakingOfQuestionPapers);
        }


        public async Task<ActionResult> UndertakingOfQuestionPapers(UndertakingOfQuestionPapers undertakingOfQuestionPapers, FormCollection fc)
        {
            try
            {

                if (string.IsNullOrEmpty(undertakingOfQuestionPapers.QP_Month) || string.IsNullOrEmpty(undertakingOfQuestionPapers.QP_Year) ||
                    string.IsNullOrEmpty(undertakingOfQuestionPapers.QP_Class))
                {
                    TempData["resultIns"] = "FILL";
                    return View(undertakingOfQuestionPapers);
                }

                if (Session["LoginSession"] != null)
                {
                    LoginSession loginSession = (LoginSession)Session["LoginSession"];
                    undertakingOfQuestionPapers.CentCode = loginSession.SCHL;
                    undertakingOfQuestionPapers.ClassList = new AbstractLayer.DBClass().GetAllPSEBCLASS().Where(s => s.Value == "8").ToList();
                }



                int NOS_Entry = _context.UndertakingOfQuestionPapers.Where(s => s.CentCode == undertakingOfQuestionPapers.CentCode
                && s.QP_Class == undertakingOfQuestionPapers.QP_Class
                && s.QP_Month == undertakingOfQuestionPapers.QP_Month
                && s.QP_Year == undertakingOfQuestionPapers.QP_Year).Count();

                if (NOS_Entry == 0)
                {
                    undertakingOfQuestionPapers.Refno = undertakingOfQuestionPapers.CentCode
                        + "C" + undertakingOfQuestionPapers.QP_Class
                        + undertakingOfQuestionPapers.QP_Month + undertakingOfQuestionPapers.QP_Year;

                    UndertakingOfQuestionPapers dataToSave = new UndertakingOfQuestionPapers()
                    {
                        QPID = 0,
                        CentCode = undertakingOfQuestionPapers.CentCode,
                        Refno = undertakingOfQuestionPapers.Refno.ToUpper(),
                        QP_Class = undertakingOfQuestionPapers.QP_Class,
                        QP_Month = undertakingOfQuestionPapers.QP_Month,
                        QP_Year = undertakingOfQuestionPapers.QP_Year,
                        QP_Description1 = undertakingOfQuestionPapers.QP_Description1,
                        QP_Status1 = undertakingOfQuestionPapers.QP_Status1,
                        QP_Description2 = undertakingOfQuestionPapers.QP_Description2,
                        QP_Remarks1 = undertakingOfQuestionPapers.QP_Remarks1,
                        QP_Status2 = undertakingOfQuestionPapers.QP_Status2,
                        QP_Remarks2 = undertakingOfQuestionPapers.QP_Remarks2,
                        QP_Description3 = undertakingOfQuestionPapers.QP_Description3,
                        QP_Status3 = undertakingOfQuestionPapers.QP_Status3,
                        QP_Remarks3 = undertakingOfQuestionPapers.QP_Remarks3,
                        SubmitBy = undertakingOfQuestionPapers.CentCode,
                        SubmitOn = DateTime.Now,
                        IsActive = true,
                        IsFinalLock = false
                    };

                    _context.UndertakingOfQuestionPapers.Add(dataToSave);
                    int insertedRecords = await _context.SaveChangesAsync();

                    if (insertedRecords > 0)
                    {
                        TempData["resultIns"] = "S";
                    }
                    else
                    {
                        TempData["resultIns"] = "F";
                    }
                }

                else
                {

                    TempData["resultIns"] = "DUP";
                    undertakingOfQuestionPapers.StatusList = new AbstractLayer.DBClass().GetYesNoText();
                    undertakingOfQuestionPapers.ClassList = new AbstractLayer.DBClass().GetAllPSEBCLASS().Where(s => s.Value == "8").ToList();
                    undertakingOfQuestionPapers.MonthList = DBClass.GetMonthNameNumber();
                    undertakingOfQuestionPapers.YearList = new AbstractLayer.DBClass().GetSession();
                    return View(undertakingOfQuestionPapers);
                }
                return RedirectToAction("UndertakingOfQuestionPapers", "Home");
            }
            catch (Exception ex)
            {
                TempData["resultIns"] = "Error : " + ex.Message;
            }
            return RedirectToAction("UndertakingOfQuestionPapers", "Home");
            //return View(undertakingOfQuestionPapers);
        }


        [Route("ActionUndertakingOfQuestionPapers/{id}/{act}")]
        public async Task<ActionResult> ActionUndertakingOfQuestionPapers(string id, string act)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(act))
            {
                if (act.ToUpper() == "D")
                {
                    UndertakingOfQuestionPapers data = _context.UndertakingOfQuestionPapers.SingleOrDefault(s => s.Refno.ToUpper() == id.ToUpper());
                    if (data != null && data.IsFinalLock == false)
                    {
                        _context.Entry(data).State = EntityState.Deleted;
                        int insertedRecords = await _context.SaveChangesAsync();
                        TempData["resultIns"] = "DEL";
                    }
                }
                else if (act.ToUpper() == "F")
                {
                    UndertakingOfQuestionPapers data = _context.UndertakingOfQuestionPapers.SingleOrDefault(s => s.Refno.ToUpper() == id.ToUpper());
                    data.FinalSubmitOn = DateTime.Now;
                    data.IsFinalLock = true;
                    if (data != null)
                    {
                        _context.Entry(data).State = EntityState.Modified;
                        int insertedRecords = await _context.SaveChangesAsync();
                        TempData["resultIns"] = "FNL";
                    }
                }

            }
            return RedirectToAction("UndertakingOfQuestionPapers", "Home");
        }

        [Route("UndertakingOfQuestionPapersPrint/{id}")]
        public ActionResult UndertakingOfQuestionPapersPrint(string id)
        {
            UndertakingOfQuestionPapersViews undertakingOfQuestionPapers = new UndertakingOfQuestionPapersViews();
            if (!string.IsNullOrEmpty(id))
            {
                undertakingOfQuestionPapers = _context.UndertakingOfQuestionPapersViews.SingleOrDefault(s => s.Refno.ToUpper() == id.ToUpper());
            }   
            return View(undertakingOfQuestionPapers);
        }
        #endregion



        #region  

        [Route("CentreList")]
        public ActionResult CentreList()
        {
            List<SelectListItem> DistList = new AbstractLayer.DBClass().GetDistE();
            ViewBag.Dist = DistList;

            ViewBag.ExamCategoryMastersActiveList = new AbstractLayer.DBClass().GetExamCategoryMastersActiveList();
            


            ViewBag.TotalCount = 0;
            return View();

        }
        


        [Route("CentreList")]
        [HttpPost]
        public ActionResult CentreList(int? page, DEOModel DEO, FormCollection frm, string cmd, string SelExamCategoryMonth, string SelDist, string Category, string SearchString)
        {
          
            try
            {

                List<SelectListItem> DistList = new AbstractLayer.DBClass().GetDistE();
                ViewBag.Dist = DistList;

                ViewBag.ExamCategoryMastersActiveList = new AbstractLayer.DBClass().GetExamCategoryMastersActiveList();


                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

               

                string Search = "";
                if (cmd == "Search" && !string.IsNullOrEmpty(SelExamCategoryMonth))
                {
                   
                    if (SelDist == "0" || SelDist == "")
                    {
                        Search += "a.Dist is not null ";

                    }
                    else if (SelDist != "")
                    {
                        Search += "a.Dist='" + SelDist.ToString().Trim() + "'";
                        ViewBag.SelectedItem = SelDist;

                    }


                   

                    if (!string.IsNullOrEmpty(Category) && !string.IsNullOrEmpty(SearchString))
                    {
     
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
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                
                    DEO.StoreAllData =  DEODB.GetCenterListSP(Search, SelExamCategoryMonth);

                    if (DEO.StoreAllData == null || DEO.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "DATA DOESN'T EXIST";
                        ViewBag.TotalCount = 0;                      
                    }
                    else
                    {
                        ViewBag.TotalCount = DEO.StoreAllData.Tables[0].Rows.Count;                      
                        
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

        #endregion



    }
}