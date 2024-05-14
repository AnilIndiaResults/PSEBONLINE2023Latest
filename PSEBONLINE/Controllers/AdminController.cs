﻿using System;
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
using Newtonsoft.Json;
using System.Web.Routing;
using System.Configuration;
using System.Reflection;
using PSEBONLINE.Filters;
using System.Diagnostics;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon;



namespace PSEBONLINE.Controllers
{
	//[AdminMenuFilter]
	public class AdminController : Controller
	{

		private const string BUCKET_NAME = "psebdata";
		#region SiteMenu       

		//Executes before every actionm
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

		private readonly DBContext _context = new DBContext();

		// GET: Admin
		AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
		AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
		AbstractLayer.HomeDB objDB1 = new AbstractLayer.HomeDB();
		AbstractLayer.BankDB objDB2 = new AbstractLayer.BankDB();
		AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
		string sp = System.Configuration.ConfigurationManager.AppSettings["upload"];
		private AdminUserModel aum = new AdminUserModel();


		[AdminLoginCheckFilter]
		public ActionResult PageNotAuthorized()
		{
			return View();
		}

		[AdminLoginCheckFilter]
		public ActionResult Welcome()
		{
			return View();
		}

		#region Admin Login 
		[Route("Admin")]
		[Route("Admin/login")]
		public ActionResult Index()
		{
			if (TempData["result"] != null)
			{
				ViewData["result"] = TempData["result"];
			}
			HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
			HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			HttpContext.Response.Cache.SetNoStore();
			Session.Clear();
			TempData.Clear();
			Session.Abandon();
			Session.RemoveAll();
			try
			{
				ViewBag.SessionList = objCommon.GetSession();
				return View();
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}

		[HttpPost]
		[Route("Admin")]
		[Route("Admin/login")]
		public ActionResult Index(LoginModel lm, FormCollection frm)
		{
			if (lm.Session == null) { ViewData["result"] = "0"; }
			ViewBag.SessionList = objCommon.GetSession();

			int empLoginStatus = 0;
			int adminmpLoginStatus = 0;

			try
			{
				AdminEmployeeAPIModel adminEmployeeAPIModel = AbstractLayer.AdminDB.CheckAdminEmployeeLogin(lm, out empLoginStatus);
				TempData["empLoginStatus"] = empLoginStatus;

				AdminLoginSession adminLoginSession = AbstractLayer.AdminDB.CheckAdminLogin(lm);
				adminLoginSession.CurrentSession = lm.Session;
				TempData["result"] = adminmpLoginStatus = adminLoginSession.LoginStatus;



				if (adminLoginSession.LoginStatus == 1 && empLoginStatus == 1)
				{
					//update employee data in admin emp master 
					int updateEmpLoginStatus = 0;
					AdminEmployeeMasters adminEmployeeMastersModel = new AbstractLayer.AdminDB().UpdateAdminEmployeeMastersByUserIdAPI(lm.AdminEmployeeUserId, lm.AdminEmployeePassword, adminEmployeeAPIModel, out updateEmpLoginStatus);
					if (updateEmpLoginStatus > 0)
					{
						adminLoginSession.AdminEmployeeUserId = adminEmployeeMastersModel.Userid;
						adminLoginSession.AdminEmployeeName = adminEmployeeMastersModel.Name;
						adminLoginSession.AdminEmployeePost = adminEmployeeMastersModel.Post;


						Session["AdminLoginSession"] = adminLoginSession;
						//
						HttpContext.Session["Session"] = adminLoginSession.CurrentSession.ToString();
						HttpContext.Session["AdminId"] = adminLoginSession.AdminId.ToString();
						HttpContext.Session["AdminUser"] = adminLoginSession.USER.ToString();
						HttpContext.Session["AdminType"] = adminLoginSession.AdminType.ToString();
						HttpContext.Session["UserName"] = adminLoginSession.USERNAME.ToString();
						HttpContext.Session["PAccessRight"] = adminLoginSession.PAccessRight.ToString();
						HttpContext.Session["DistAllow"] = adminLoginSession.Dist_Allow.ToString();
						HttpContext.Session["RoleType"] = adminLoginSession.RoleType.ToString();
						HttpContext.Session["ClassAssign"] = adminLoginSession.ActionRight.ToString();
						HttpContext.Session["AdminNumber"] = "2";

						if (adminLoginSession.PAccessRight == "Reg")
						{ return RedirectToAction("adminresultpage", "Admin"); }
						else if (adminLoginSession.AdminType.ToUpper() == "FIRM")
						{
							HttpContext.Session["AdminNumber"] = "1";
							return RedirectToAction("FirmCorrectionHome", "Admin");
						}
						else
						{
							// Change by Rohit for Admin User
							return RedirectToAction("Welcome", "Admin");
						}
					}
				}

				if (adminLoginSession.LoginStatus == 1 && empLoginStatus != 1)
				{
					TempData["result"] = "EMP";
				}
				else if (adminLoginSession.LoginStatus != 1 && empLoginStatus == 1)
				{
					TempData["result"] = "ADMIN";
				}
				else if (adminLoginSession.LoginStatus != 1 && empLoginStatus != 1)
				{
					TempData["result"] = "BOTH";
				}

				return RedirectToAction("Index", "Admin");
			}
			catch (Exception ex)
			{
				TempData["result"] = "ERR";
				TempData["resultMsg"] = ex.Message;
				return View();
			}
		}


		//[HttpPost]
		//[Route("Admin")]
		//[Route("Admin/login")]
		//public ActionResult Index(LoginModel lm, FormCollection frm)
		//{
		//    try
		//    {
		//        if (lm.Session == null) { ViewData["result"] = "0"; }
		//        ViewBag.SessionList = objCommon.GetSession();
		//        DataTable dt = objDB.CheckAdminLogin(lm); // passing Value to SchoolDB from model and Type 1 For regular
		//        if (dt.Rows.Count > 0)
		//        {
		//            if (dt.Rows[0]["STATUS"].ToString() == "1")
		//            {
		//                // HttpContext.Session["Session"] = lm.Session.ToString();
		//                HttpContext.Session["Session"] = lm.Session.ToString();
		//                ViewData["result"] = null;
		//                HttpContext.Session["AdminId"] = dt.Rows[0]["id"].ToString();
		//                HttpContext.Session["AdminUser"] = dt.Rows[0]["user"].ToString();
		//                HttpContext.Session["AdminType"] = dt.Rows[0]["Usertype"].ToString();
		//                HttpContext.Session["UserName"] = dt.Rows[0]["User_fullnm"].ToString();
		//                HttpContext.Session["PAccessRight"] = dt.Rows[0]["PAccessRight"].ToString();
		//                HttpContext.Session["DistAllow"] = dt.Rows[0]["Dist_Allow"].ToString();
		//                HttpContext.Session["RoleType"] = dt.Rows[0]["RoleType"].ToString();
		//                HttpContext.Session["ClassAssign"] = dt.Rows[0]["ActionRight"].ToString();
		//                // New added by Rohit for Session Management                      

		//                HttpContext.Session["AdminNumber"] = "2";
		//                if (dt.Rows[0]["PAccessRight"].ToString() == "Reg")
		//                { return RedirectToAction("adminresultpage", "Admin"); }
		//                ////else if (dt.Rows[0]["Usertype"].ToString() == "DM")
		//                ////{ return RedirectToAction("SchoolList", "DM"); }
		//                else if (dt.Rows[0]["Usertype"].ToString().ToUpper() == "FIRM")
		//                {
		//                    HttpContext.Session["AdminNumber"] = "1";
		//                    return RedirectToAction("FirmCorrectionHome", "Admin");
		//                }
		//                else
		//                {
		//                    // Change by Rohit for Admin User
		//                    return RedirectToAction("Welcome", "Admin");
		//                }
		//            }
		//            else
		//            {
		//                ViewData["result"] = "2";
		//                return View();
		//            }
		//        }
		//        else
		//        {
		//            ViewData["result"] = "0";
		//            return View();
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        ////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
		//        //return RedirectToAction("Logout", "Login");
		//        return View();
		//    }
		//}


		#endregion Admin Login

		#region AllowCCE
		public ActionResult AllowCCE(int? ID, SchoolAllowForCCE fm, int? page)
		{
			AbstractLayer.SchoolDB objSchoolDB = new AbstractLayer.SchoolDB();
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			var itemsch1 = new SelectList(new[] { new { ID = "1", Name = "School Code" }, new { ID = "2", Name = " Receipt No" }, new { ID = "3", Name = " Roll No" }, }, "ID", "Name", 1);
			ViewBag.MySearch = itemsch1.ToList();
			ViewBag.SelectedSearch = "0";

			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Login");
				}
				int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				ViewBag.Id = ID == null ? 0 : ID;
				string Search = string.Empty;
				Search = "Id like '%' ";
				int Outstatus = 0;



				if (ID > 0)
				{
					Search += " and Id=" + ID;
					//int pageIndex = 1;
					//pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					//ViewBag.pagesize = pageIndex;

					DataSet ds = objSchoolDB.ListingAllowCCE(2, 0, Search, out Outstatus);
					fm.StoreAllData = ds;
					if (fm.StoreAllData == null || fm.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						if (ID > 0)
						{
							fm.Id = Convert.ToInt32(ds.Tables[0].Rows[0]["Id"].ToString());
							fm.Schl = Convert.ToString(ds.Tables[0].Rows[0]["Schl"].ToString());
							fm.Cls = Convert.ToString(ds.Tables[0].Rows[0]["Cls"].ToString());
							fm.LastDate = Convert.ToString(ds.Tables[0].Rows[0]["LastDate"].ToString());
							fm.AllowTo = Convert.ToString(ds.Tables[0].Rows[0]["AllowTo"].ToString());
							fm.ReceiptNo = Convert.ToString(ds.Tables[0].Rows[0]["ReceiptNo"].ToString());
							fm.DepositDate = Convert.ToString(ds.Tables[0].Rows[0]["DepositDate"].ToString());
							fm.Amount = Convert.ToInt32(ds.Tables[0].Rows[0]["Amount"].ToString());
							fm.AllowRemarks = Convert.ToString(ds.Tables[0].Rows[0]["AllowRemarks"].ToString());
							fm.Panel = Convert.ToString(ds.Tables[0].Rows[0]["PanelName"].ToString());
							//fm.EmpDetails = Convert.ToString(ds.Tables[0].Rows[0]["EmpDetails"].ToString());
						}
						ViewBag.TotalCountId = fm.StoreAllData.Tables[0].Rows.Count;
					}
				}
				else
				{
					DataSet ds = objSchoolDB.ListingAllowCCE(2, 0, Search, out Outstatus);
					fm.StoreAllData = ds;
					if (fm.StoreAllData == null || fm.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						ViewBag.TotalCount = fm.StoreAllData.Tables[0].Rows.Count;
					}
					return View(fm);
				}
			}
			catch (Exception ex)
			{
				//////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));               
			}
			return View(fm);
		}

		[HttpPost]
		public ActionResult AllowCCE(SchoolAllowForCCE fm, FormCollection frm, string cmd, string SearchList, string SearchString)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			if (Session["AdminId"] == null)
			{
				return RedirectToAction("Index", "Login");
			}

			var itemsch1 = new SelectList(new[] { new { ID = "1", Name = "School Code" }, new { ID = "2", Name = " Receipt No" }, new { ID = "3", Name = " Roll No" }, }, "ID", "Name", 1);
			ViewBag.MySearch = itemsch1.ToList();
			ViewBag.SelectedSearch = "0";

			AbstractLayer.SchoolDB objSchoolDB = new AbstractLayer.SchoolDB();
			int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
			ViewBag.Id = fm.Id == null ? 0 : fm.Id;
			//Check server side validation using data annotation

			if (!cmd.ToLower().Contains("search"))
			{
				int type = 0;
				if (fm.Id == null)
				{
					fm.Id = 0;
					type = 0;
				}
				else { type = 1; }
				string SchlMobile = "";

				fm.EmpUserId = adminLoginSession.AdminEmployeeUserId;
				int result = objSchoolDB.InsertSchoolAllowForCCE(type, fm, out SchlMobile);//InsertFeeMaster2016SP          
				if (result > 0)
				{
					if (fm.Id == 0)
					{
						ViewData["result"] = 1;
						string cls1 = fm.Cls == "4" ? "Sr. Sec" : "Matric";
						if (!string.IsNullOrEmpty(SchlMobile))
						{
							string Sms = "";
							if (fm.Cls == "10")
							{
								Sms = "As per your school request, Elective Theory has been unlocked for " + cls1 + " upto date  " + fm.LastDate + ". Kindly fill & final submit.";
							}
							else
							{
								//As per your school request, CCE has been unlocked for +class+ upto date +allowedupto .Kindly fill &final submit CCE. 
								Sms = "As per your school request, CCE has been unlocked for " + cls1 + " upto date  " + fm.LastDate + ". Kindly fill &final submit CCE.";
							}
							string getSms = new AbstractLayer.DBClass().gosms(SchlMobile, Sms);
						}
					}
					else { ViewData["result"] = 2; }
				}
				else if (result == -1)
				{
					//-----alredy exist
					ViewData["result"] = -1;
				}
				else
				{
					//Not Saved                 
					ViewData["result"] = 0;
				}

			}
			else
			{
				string Search = string.Empty;
				Search = "Id like '%' ";
				int Outstatus = 0;

				if (!string.IsNullOrEmpty(SearchList))
				{
					if (!string.IsNullOrEmpty(SearchString))
					{
						if (SearchList == "1")
						{ Search += " and Schl ='" + SearchString.ToString() + "'"; }
						else if (SearchList == "2")
						{ Search += " and ReceiptNo ='" + SearchString.ToString() + "'"; }
						else if (SearchList == "3")
						{ Search += " and AllowTo like '%" + SearchString.ToString() + "%'"; }

						ViewBag.SearchString = SearchString;
						TempData["SearchString"] = SearchString;
					}
				}



				DataSet ds = objSchoolDB.ListingAllowCCE(2, 0, Search, out Outstatus);
				fm.StoreAllData = ds;
				if (fm.StoreAllData == null || fm.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = fm.StoreAllData.Tables[0].Rows.Count;
				}
				return View(fm);
			}
			return View(fm);
		}


		public ActionResult ListingCCE(int? Id, SchoolAllowForCCE fm)
		{
			AbstractLayer.SchoolDB objSchoolDB = new AbstractLayer.SchoolDB();
			if (Id > 0)
			{
				string Search = string.Empty;
				Search = "Id like '%' ";
				int Outstatus = 0;

				DataSet ds = objSchoolDB.ListingAllowCCE(0, Convert.ToInt32(Id), Search, out Outstatus);
				if (Outstatus > 0)
				{
					TempData["DeleteCCE"] = "1";
				}
				else
				{
					TempData["DeleteCCE"] = null;
				}
			}
			return RedirectToAction("AllowCCE");
		}
		#endregion AllowCCE



		#region Circular
		[AdminLoginCheckFilter]
		public ActionResult Circular(int? ID, CircularModels fm, int? page)
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Login");
				}
				int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				ViewBag.Id = ID == null ? 0 : ID;
				string Search = string.Empty;
				Search = "Id like '%' ";

				DataSet ds1 = objDB.CircularTypeMaster();
				if (ds1.Tables[0].Rows.Count > 0)
				{
					List<CircularTypeMaster> ci = new List<CircularTypeMaster>();
					ci = (from DataRow row in ds1.Tables[0].Rows

						  select new CircularTypeMaster
						  {
							  Id = Convert.ToInt32(row["Id"]),
							  CircularType = row["CircularType"].ToString(),
							  IsSelected = false
						  }).ToList();

					fm.CircularTypeMasterList = ci;
				}
				else { fm.CircularTypeMasterList = null; }


				if (ID > 0)
				{
					Search += " and Id=" + ID;
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;

					DataSet ds = objDB.CircularMaster(Search, pageIndex);//GetAllFeeMaster2016SP
					fm.StoreAllData = ds;
					if (fm.StoreAllData == null || fm.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						if (ID > 0)
						{
							fm.ID = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"].ToString());
							fm.CircularNo = ds.Tables[0].Rows[0]["CircularNo"].ToString();
							fm.Session = ds.Tables[0].Rows[0]["Session"].ToString();
							fm.Title = ds.Tables[0].Rows[0]["Title"].ToString();
							fm.Attachment = ds.Tables[0].Rows[0]["Attachment"].ToString();
							fm.UrlLink = ds.Tables[0].Rows[0]["UrlLink"].ToString();
							fm.UploadDate = ds.Tables[0].Rows[0]["UploadDate"].ToString();
							fm.ExpiryDate = ds.Tables[0].Rows[0]["ExpiryDate"].ToString();
							fm.Category = ds.Tables[0].Rows[0]["Category"].ToString();
							fm.IsMarque = Convert.ToInt32(ds.Tables[0].Rows[0]["IsMarque"].ToString());
							fm.IsActive = Convert.ToInt32(ds.Tables[0].Rows[0]["IsActive"].ToString().ToLower() == "true" ? "1" : "0");
							fm.UploadDate = ds.Tables[0].Rows[0]["UploadDate"].ToString();
							fm.SelectedCircularTypes = ds.Tables[0].Rows[0]["CircularTypes"].ToString();
							fm.CircularRemarks = ds.Tables[0].Rows[0]["CircularRemarks"].ToString();


							if (!string.IsNullOrEmpty(fm.SelectedCircularTypes))
							{
								List<CircularTypeMaster> ci = new List<CircularTypeMaster>();
								foreach (System.Data.DataRow dr in ds1.Tables[0].Rows)
								{
									if (AbstractLayer.StaticDB.ContainsValue(fm.SelectedCircularTypes, @dr["Id"].ToString()))
									{
										ci.Add(new CircularTypeMaster { CircularType = @dr["CircularType"].ToString(), Id = Convert.ToInt32(@dr["Id"]), IsSelected = true });
									}
									else
									{ ci.Add(new CircularTypeMaster { CircularType = @dr["CircularType"].ToString(), Id = Convert.ToInt32(@dr["Id"]) }); }
								}
								fm.CircularTypeMasterList = ci;
							}
						}
						ViewBag.TotalCount = fm.StoreAllData.Tables[0].Rows.Count;
						ViewBag.TotalCircularCount = fm.StoreAllData.Tables[1].Rows[0]["TotalCount"].ToString();
					}
				}
				else
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(fm);
				}
			}
			catch (Exception ex)
			{
				//////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));               
			}
			return View(fm);
		}

		[HttpPost]
		public ActionResult Circular(CircularModels fm, FormCollection frm, string cmd, string ChkId)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			if (Session["AdminId"] == null)
			{
				return RedirectToAction("Index", "Login");
			}
			int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
			ViewBag.Id = fm.ID == null ? 0 : fm.ID;
			//Check server side validation using data annotation

			if (ModelState.IsValid)
			{
				if (fm.ID == null)
				{
					fm.ID = 0;
					if (fm.file == null && string.IsNullOrEmpty(fm.UrlLink))
					{
						ViewData["result"] = 20;
						return View(fm);
					}
				}
				else
				{
					if (fm.ID > 0 && fm.file == null && !string.IsNullOrEmpty(fm.Attachment) && !string.IsNullOrEmpty(fm.UrlLink))
					{
						fm.Attachment = "";
					}
					else if (fm.ID > 0 && fm.file != null && !string.IsNullOrEmpty(fm.Attachment) && !string.IsNullOrEmpty(fm.UrlLink))
					{
						ViewData["result"] = 20;
						return View(fm);
					}
				}
				string fileExt, fileName = "";
				string outCircularNo = "";
				fm.CreatedBy = fm.UpdatedBy = AdminId;
				fm.IsMarque = (fm.Category.ToLower() == "marquee" ? 1 : 0);
				fm.Type = 0;
				fm.Session = Session["Session"].ToString();
				fm.SelectedCircularTypes = string.Join(",", fm.CircularTypeMasterList.Where(s => s.IsSelected == true).Select(s => s.Id.ToString()));




				int result = objDB.InsertCircularMaster(adminLoginSession.AdminEmployeeUserId, fm, out outCircularNo);//InsertFeeMaster2016SP
				ViewBag.outCircularNo = outCircularNo;
				if (result > 0)
				{
					if (fm.ID == 0) { ViewData["result"] = 1; }
					else { ViewData["result"] = 2; }

					if (fm.file != null)
					{
						fileExt = Path.GetExtension(fm.file.FileName).ToLower();
						fileName = outCircularNo + fileExt;

						using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
						{
							using (var newMemoryStream = new MemoryStream())
							{
								var uploadRequest = new TransferUtilityUploadRequest
								{
									InputStream = fm.file.InputStream,
									Key = string.Format("allfiles/Circular/{0}", fileName),
									BucketName = BUCKET_NAME,
									CannedACL = S3CannedACL.PublicRead
								};

								var fileTransferUtility = new TransferUtility(client);
								fileTransferUtility.Upload(uploadRequest);
							}
						}


						//var path = Path.Combine(Server.MapPath("~/Upload/" + "Circular"), fileName);
						//string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + "/Circular"));
						//if (!Directory.Exists(FilepathExist))
						//{
						//    Directory.CreateDirectory(FilepathExist);
						//}
						//fm.file.SaveAs(path);
						fm.Attachment = "https://psebdata.s3.ap-south-1.amazonaws.com/allfiles/Circular/" + fileName;
						fm.CircularNo = outCircularNo;
						fm.ID = result;
						fm.Type = 1;
						int result2 = objDB.InsertCircularMaster(adminLoginSession.AdminEmployeeUserId, fm, out outCircularNo);
						if (result > 0)
						{
							ModelState.Clear();
						}
					}
				}
				else if (result == -1)
				{
					//-----alredy exist
					ViewData["result"] = -1;
				}
				else
				{
					//Not Saved                 
					ViewData["result"] = 0;
				}

			}


			DataSet ds1 = objDB.CircularTypeMaster();
			if (ds1.Tables[0].Rows.Count > 0)
			{
				List<CircularTypeMaster> ci = new List<CircularTypeMaster>();
				ci = (from DataRow row in ds1.Tables[0].Rows

					  select new CircularTypeMaster
					  {
						  Id = Convert.ToInt32(row["Id"]),
						  CircularType = row["CircularType"].ToString(),
						  IsSelected = true
					  }).ToList();

				fm.CircularTypeMasterList = ci;
			}
			else { fm.CircularTypeMasterList = null; }
			return View(fm);
		}



		public ActionResult ViewCircular(int? ID, CircularModels fm, int? page)
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Login");
				}
				int AdminId = Convert.ToInt32(Session["AdminId"].ToString());

				DataSet ds1 = objDB.CircularTypeMaster();
				if (ds1.Tables[0].Rows.Count > 0)
				{
					List<CircularTypeMaster> ci = new List<CircularTypeMaster>();
					ci = (from DataRow row in ds1.Tables[0].Rows

						  select new CircularTypeMaster
						  {
							  Id = Convert.ToInt32(row["Id"]),
							  CircularType = row["CircularType"].ToString(),
							  IsSelected = true
						  }).ToList();

					fm.CircularTypeMasterList = ViewBag.CircularTypeMasterList = ci;

				}
				else { fm.CircularTypeMasterList = null; }

				string Search = string.Empty;
				Search = "Id like '%' ";
				if (ID > 0)
				{
					Search += " and Id=" + ID;
				}
				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;

				DataSet ds = objDB.CircularMaster(Search, pageIndex);//GetAllFeeMaster2016SP
				fm.StoreAllData = ds;
				if (fm.StoreAllData == null || fm.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = fm.StoreAllData.Tables[0].Rows.Count;
					//
					int count = Convert.ToInt32(fm.StoreAllData.Tables[1].Rows[0]["TotalCount"]);
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
			}
			catch (Exception ex)
			{
				//////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));               
			}
			return View(fm);
		}

		[HttpPost]
		public ActionResult ViewCircular(int? ID, CircularModels fm, int? page, string SearchString, string FromDate, string ToDate)
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Login");
				}
				int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;


				string Search = string.Empty;
				Search = "Id like '%' ";
				if (!string.IsNullOrEmpty(SearchString))
				{

					Search += " and Title like '%" + SearchString.ToString() + "%'";
					ViewBag.SearchString = SearchString;
					TempData["SearchString"] = SearchString;
				}
				if (FromDate != "")
				{
					ViewBag.FromDate = FromDate;
					TempData["FromDate"] = FromDate;
					Search += " and  CONVERT(DATETIME, DATEDIFF(DAY, 0, UploadDate)) >=convert(DATETIME,'" + FromDate.ToString() + "',103) ";
				}
				if (ToDate != "")
				{
					ViewBag.ToDate = ToDate;
					TempData["ToDate"] = ToDate;
					Search += "   and CONVERT(DATETIME, DATEDIFF(DAY, 0, UploadDate)) <=  convert(DATETIME,'" + ToDate.ToString() + "',103)";
				}

				DataSet ds = objDB.CircularMaster(Search, pageIndex);//GetAllFeeMaster2016SP
				fm.StoreAllData = ds;
				if (fm.StoreAllData == null || fm.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = fm.StoreAllData.Tables[0].Rows.Count;
					//
					int count = Convert.ToInt32(fm.StoreAllData.Tables[1].Rows[0]["TotalCount"]);
					ViewBag.TotalCircularCount = count;
					int tp = Convert.ToInt32(count);
					int pn = tp / 15;
					int cal = 15 * pn;
					int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
					if (res >= 1)
					{ ViewBag.pn = pn + 1; }
					else
					{ ViewBag.pn = pn; }

				}
			}
			catch (Exception ex)
			{
				//////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));               
			}
			return View(fm);
		}

		public ActionResult UpdateCircularStatus(int id)
		{
			try
			{
				if (Session["AdminType"] == null)
				{ return RedirectToAction("Index", "Admin"); }
				else
				{
					string AdminType = Session["AdminType"].ToString();
					int outstatus = 0;
					string result = objDB.ListingCircular(1, id, out outstatus);
					return RedirectToAction("ViewCircular", "Admin");
				}

			}
			catch (Exception)
			{
				return RedirectToAction("ViewUser", "Admin");
			}
		}
		#endregion




		#region Update Master Data

		public ActionResult UpdateMasterData(string id, AdminModels am)
		{
			if (Session["UserName"] == null)
			{ return RedirectToAction("Index", "Admin"); }

			string firmuser = Session["UserName"].ToString();
			ViewBag.SessionList = objCommon.GetSessionAdmin().Where(s => Convert.ToInt32(s.Value.Substring(0, 4)) <= 2019).ToList().Take(6); //GetSessionAdmin


			var itemsch = new SelectList(new[]{new {ID="1",Name="Regular"},new {ID="2",Name="Open"},
			new {ID="3",Name="Private"},}, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			ViewBag.SelectedItem = "0";

			var itemsch1 = new SelectList(new[] { new { ID = "1", Name = "Roll" }, new { ID = "2", Name = "RegNo" }, }, "ID", "Name", 1);
			ViewBag.MySearch = itemsch1.ToList();
			ViewBag.SelectedSearch = "0";


			return View(am);
		}

		[HttpPost]
		public ActionResult UpdateMasterData(string id, AdminModels am, FormCollection frm, string submit, string SelList, string SelYear, string SearchList, string SearchString)
		{
			if (Session["UserName"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			try
			{
				ViewBag.SessionList = objCommon.GetSessionAdmin().Where(s => Convert.ToInt32(s.Value.Substring(0, 4)) <= 2019).ToList().Take(6); //GetSessionAdmin


				string AdminType = Session["AdminType"].ToString();
				int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				string firmuser = Session["UserName"].ToString();

				var itemsch = new SelectList(new[]{new {ID="1",Name="Regular"},new {ID="2",Name="Open"},
			new {ID="3",Name="Private"},}, "ID", "Name", 1);
				ViewBag.MySch = itemsch.ToList();
				ViewBag.SelectedItem = "0";

				var itemsch1 = new SelectList(new[] { new { ID = "1", Name = "Roll" }, new { ID = "2", Name = "RegNo" }, }, "ID", "Name", 1);
				ViewBag.MySearch = itemsch1.ToList();
				ViewBag.SelectedSearch = "0";

				if (SelYear == null)
				{
					ViewData["Result"] = "-3";
					ViewBag.Message = "Please Select Session";
					return View(am);
				}

				string RP = string.Empty;
				if (SelList == null) { }
				else
				{
					RP = SelList == "1" ? "R" : SelList == "2" ? "O" : SelList == "3" ? "P" : "A";
				}

				if (submit == null)
				{ return RedirectToAction("UpdateMasterData", "Admin"); }
				else
				{
					if (submit.ToLower().Contains("reset"))
					{ return RedirectToAction("UpdateMasterData", "Admin"); }

					else if (submit.ToLower().Contains("search"))
					{
						#region searching
						ViewBag.searchYear = SelYear;
						ViewBag.searchRP = RP;
						string OutErr = string.Empty;
						DataSet ds1 = objDB.SearchMasterData(SelYear, Convert.ToInt32(1), RP, AdminId, SearchString, out OutErr); // UpdateMasterDataSPNew
						if (OutErr == "1")
						{
							if (ds1.Tables[0].Rows.Count > 0)
							{
								am.StoreAllData = ds1;
								ViewBag.TotalCount2 = ds1.Tables[0].Rows.Count;
								ViewData["Result"] = "20";
							}
							else
							{
								am.StoreAllData = ds1;
								ViewBag.TotalCount2 = ds1.Tables[0].Rows.Count;
								ViewData["Result"] = "25";
							}


						}
						else
						{
							ViewBag.TotalCount2 = 0;
							ViewBag.Message = OutErr;
							ViewData["Result"] = "0";

						}


						#endregion searching
					}
					else
					{
						#region Bulk upload
						string fileLocation = "";
						string filename = "";
						if (am.file != null)
						{
							filename = Path.GetFileName(am.file.FileName);

							DataSet ds = new DataSet();
							if (am.file.ContentLength > 0)  //(Request.Files["file"].ContentLength > 0
							{
								string fileName1 = "MasterData_" + firmuser + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
								string fileExtension = System.IO.Path.GetExtension(am.file.FileName);
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
									am.file.SaveAs(fileLocation);
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
									using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
									{
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
										string query = string.Format("Select * from [{0}]", excelSheets[0]);
										using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
										{
											dataAdapter.Fill(ds);
										}
									}

									DataTable dtexport;
									string CheckMis = objDB.CheckUpdateMasterDataMis(ds, out dtexport, RP);
									if (CheckMis == "")
									{
										DataTable dt1 = ds.Tables[0];
										if (dt1.Columns.Contains("Status"))
										{
											dt1.Columns.Remove("Status");
										}
										// UpdateData
										#region UpdateData


										string ErrStatus = string.Empty;
										DataSet ds1 = objDB.UpdateMasterData(SelYear, Convert.ToInt32(1), RP, AdminId, dt1, out ErrStatus); // UpdateMasterDataSPNew

										if (ErrStatus == "1")
										{
											ViewBag.Message = "Master Data Updated Successfully";
											ViewData["Result"] = "1";
										}
										else
										{
											ViewBag.Message = ErrStatus;
											ViewData["Result"] = "0";

										}
										#endregion UpdateData                          
										return View(am);
									}
									else
									{
										if (dtexport != null)
										{
											ExportDataFromDataTable(dtexport, "Error_MasterData");
										}
										ViewData["Result"] = "-1";
										ViewBag.Message = CheckMis;
										return View(am);
									}
								}
								else
								{

									ViewData["Result"] = "-2";
									ViewBag.Message = "Please Upload Only .xls file only";
									return View(am);
								}
							}

						}
						else
						{
							//ViewData["Result"] = "-4";
							// ViewBag.Message = "Please select .xls file only";
							//return View();
						}

						#endregion

					}

				}

				return View(am);
			}
			catch (Exception ex)
			{
				ViewBag.Message = "Error: " + ex.Message;
				ViewData["Result"] = "50";
				return View();
			}
		}


		[HttpPost]
		public ActionResult JqUpdateMasterDataSingle(string roll, string regno, string dob, string name, string pname, string fname, string pfname,
			string mname, string pmname, string remarks, string updt, string SelYear, string RP)
		{
			if (Session["AdminId"] == null)
			{
				return Json(new { status = "-1" }, JsonRequestBehavior.AllowGet);
			}
			string AdminType = Session["AdminType"].ToString().ToUpper();
			int AdminId = Convert.ToInt32(Session["AdminId"]);
			string status = "";
			string OutError = "1";
			if (string.IsNullOrEmpty(roll) || string.IsNullOrEmpty(regno) || string.IsNullOrEmpty(dob) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(fname)
				|| string.IsNullOrEmpty(pfname) || string.IsNullOrEmpty(mname) || string.IsNullOrEmpty(pmname) || string.IsNullOrEmpty(remarks) || string.IsNullOrEmpty(updt))
			{
				var results = new
				{
					status = "2"
				};
				return Json(results, JsonRequestBehavior.AllowGet);
			}
			else
			{
				#region update data
				DataTable dt = new DataTable();
				dt.Columns.Add("ROLL");
				dt.Columns.Add("NAME");
				dt.Columns.Add("PNAME");
				dt.Columns.Add("FNAME");
				dt.Columns.Add("PFNAME");
				dt.Columns.Add("MNAME");
				dt.Columns.Add("PMNAME");
				dt.Columns.Add("REGNO");
				dt.Columns.Add("DOB");
				dt.Columns.Add("REMARKS");
				dt.Columns.Add("UPDT");

				DataRow dr = null;
				dr = dt.NewRow(); // have new row on each iteration
				dr["ROLL"] = roll;
				dr["NAME"] = name;
				dr["PNAME"] = pname;
				dr["FNAME"] = fname;
				dr["PFNAME"] = pfname;
				dr["MNAME"] = mname;
				dr["PMNAME"] = pmname;
				dr["REGNO"] = regno;
				dr["DOB"] = dob;
				dr["REMARKS"] = remarks;
				dr["UPDT"] = updt;
				dt.Rows.Add(dr);
				dt.AcceptChanges();



				string ErrStatus = string.Empty;
				DataSet ds1 = objDB.UpdateMasterData(SelYear, Convert.ToInt32(1), RP, AdminId, dt, out ErrStatus); // UpdateMasterDataSPNew

				if (ErrStatus == "1")
				{
					OutError = "1";
				}
				else
				{
					OutError = "0";

				}
				#endregion
				var results = new
				{
					status = OutError,
				};
				return Json(results, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion Update Master Data


		#region Download Private Data
		[AdminLoginCheckFilter]
		public ActionResult DownloadPrivateData(string id, AdminModels am)
		{
			Session["downloadprintform"] = null;
			if (Session["UserName"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			string firmuser = Session["UserName"].ToString();
			string adminid = Session["Adminid"].ToString();
			string ErrStatus = string.Empty;


			#region Action Assign Method
			if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
			{ ViewBag.IsData = 1; ViewBag.IsBulk = 1; }
			else
			{

				string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
				string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
				if (aAct.Tables[0].Rows.Count > 0)
				{
					ViewBag.IsData = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/DOWNLOADPRIVATEDATA")).Count();
					ViewBag.IsBulk = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM")).Count();
				}
			}
			#endregion Action Assign Method




			List<SelectListItem> itemType = new List<SelectListItem>();
			DataSet dsType = new AbstractLayer.PrivateCandidateDB().GetAllBatch();//GetAllBatch
			if (dsType.Tables.Count > 0)
			{
				if (dsType.Tables[0].Rows.Count > 0)
				{
					foreach (System.Data.DataRow dr in dsType.Tables[0].Rows)
					{
						itemType.Add(new SelectListItem { Text = @dr["batchYear"].ToString(), Value = @dr["batch"].ToString() });
					}
					ViewBag.MySch = itemType.ToList();
				}
			}
			ViewBag.SelectedItem = "0";


			am.StoreAllData = objDB.DownloadPrivateData(Convert.ToInt32(1), "", adminid, "", out ErrStatus);
			if (am.StoreAllData == null || am.StoreAllData.Tables.Count == 0)
			{
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
			}
			else
			{
				ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
				ViewBag.TotalCount1 = am.StoreAllData.Tables[1].Rows[0]["Total"].ToString();

			}

			if (id == null)
			{
				// return RedirectToAction("DownloadPrivateData", "Admin");
			}
			else
			{
				DataSet ds = null;
				int type = 0;
				string fileName1 = string.Empty;
				string Search = string.Empty;
				int OutStatus = 0;
				string splitbatch = "9";
				if (id.ToLower() == "pending")
				{
					fileName1 = "Pending_" + firmuser;
					type = 5;
					Search = "refno like '%%' and DOWNLOT is null";
				}
				else
				{
					string FileExport = id.ToString();
					string splitFile = FileExport.Split('-')[0];
					splitbatch = FileExport.Split('-')[1];
					string splitLot = FileExport.Split('-')[2];
					fileName1 = "LOT" + splitLot;
					if (splitFile.ToLower().Contains("data"))
					{ type = 2; fileName1 += "_DATA_"; }
					else if (splitFile.ToLower().Contains("subject"))
					{ type = 3; fileName1 += "_SUB_"; }
					else if (splitFile.ToLower().Contains("photo"))
					{ type = 4; fileName1 += "_PHOTO_"; }
					Search = "refno like '%%'  and DOWNLOT=" + splitLot + "";
					fileName1 += firmuser + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
				}

				if (type > 0)
				{
					ds = objDB.DownloadPrivateData(Convert.ToInt32(type), splitbatch, adminid, Search, out ErrStatus); // DownloadPrivateDataSPNew
					#region Download Data or Subjects
					if (type == 2 || type == 3)
					{
						if (ds == null)
						{
							return RedirectToAction("DownloadPrivateData", "Admin");
						}
						else
						{
							if (ds.Tables[0].Rows.Count > 0)
							{
								bool ResultDownload;
								try
								{
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
									ResultDownload = true;
								}
								catch (Exception)
								{
									ResultDownload = false;
								}
							}
						}
					}
					#endregion Data

					#region Download Photo

					else if (type == 4)
					{
						if (ds == null)
						{
							return RedirectToAction("DownloadPrivateData", "Admin");
						}
						else
						{
							if (ds.Tables[0].Rows.Count > 0)
							{

								DataTable dt1 = ds.Tables[0];
								string result = objDB.DownloadZIPFileFirmExam(dt1);

							}
						}
					}
					#endregion Data

					#region Pending
					else if (type == 5)
					{
						if (ds == null)
						{
							return View(am);
						}
						else
						{
							if (ds.Tables[0].Rows.Count > 0)
							{
								ViewData["Result"] = "10";
								ViewBag.Message = ds.Tables[0].Rows[0]["DOWNLOT"].ToString();
							}
						}
					}
					#endregion Pending

				}

			}
			return View(am);
		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult DownloadPrivateData(string id, AdminModels am, FormCollection frm, string submit)
		{
			if (Session["UserName"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			try
			{

				int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				#region Action Assign Method
				if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{ ViewBag.IsData = 1; ViewBag.IsBulk = 1; }
				else
				{

					string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
					string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						ViewBag.IsData = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/DOWNLOADPRIVATEDATA")).Count();
						ViewBag.IsBulk = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM")).Count();
					}
				}
				#endregion Action Assign Method


				string adminid = Session["Adminid"].ToString();
				List<SelectListItem> itemType = new List<SelectListItem>();
				DataSet dsType = new AbstractLayer.PrivateCandidateDB().GetAllBatch();//GetAllBatch
				if (dsType.Tables.Count > 0)
				{
					if (dsType.Tables[0].Rows.Count > 0)
					{
						foreach (System.Data.DataRow dr in dsType.Tables[0].Rows)
						{
							itemType.Add(new SelectListItem { Text = @dr["batchYear"].ToString(), Value = @dr["batch"].ToString() });
						}
						ViewBag.MySch = itemType.ToList();
					}
				}
				ViewBag.SelectedItem = "0";
				string ErrStatus = string.Empty;
				string Search = string.Empty;
				DataSet ds1 = new DataSet();
				string firmuser = Session["UserName"].ToString();
				am.StoreAllData = objDB.DownloadPrivateData(Convert.ToInt32(1), "", adminid, "", out ErrStatus);
				if (am.StoreAllData == null || am.StoreAllData.Tables.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
					//ViewBag.TotalCount1 = am.StoreAllData.Tables[1].Rows.Count;
					ViewBag.TotalCount1 = am.StoreAllData.Tables[1].Rows[0]["Total"].ToString();
				}

				string Filevalue = string.Empty;
				if (frm["Filevalue"] == null) { }
				else
				{
					Filevalue = frm["Filevalue"].ToString();
				}

				//  Download Data by file
				string AdminType = Session["AdminType"].ToString();

				string fileLocation = "";
				string filename = "";
				if (Filevalue.ToUpper() == "STDIDMIS")
				{
					if (am.file != null)
					{
						filename = Path.GetFileName(am.file.FileName);

						DataSet ds = new DataSet();
						if (am.file.ContentLength > 0)  //(Request.Files["file"].ContentLength > 0
						{
							string fileName1 = "FirmExam_" + firmuser + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
							string fileExtension = System.IO.Path.GetExtension(am.file.FileName);
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
								am.file.SaveAs(fileLocation);
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
								using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
								{
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
									string query = string.Format("Select * from [{0}]", excelSheets[0]);
									using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
									{
										dataAdapter.Fill(ds);
									}
								}

								DataTable dtexport;
								string selbatch = "";
								if (frm["SelList"] != "")
								{
									selbatch = frm["SelList"];
								}
								string CheckMis = objDB.CheckDownloadPrivateDataMis(ds, out dtexport, selbatch);
								if (CheckMis == "")
								{
									DataTable dt1 = ds.Tables[0];
									if (dt1.Columns.Contains("Status"))
									{
										dt1.Columns.Remove("Status");
									}


									// string Result1 = "";
									int OutStatus = 0;
									// int REGNOLOT = 0;
									string commaString = "";
									for (int i = 0; i < dt1.Rows.Count; i++)
									{
										commaString = commaString + dt1.Rows[i]["refno"].ToString();
										//commaString += (i < dt1.Rows.Count) ? "," : string.Empty;
										commaString += (i < dt1.Rows.Count - 1) ? "," : string.Empty;
									}

									// Download
									#region DownloadFile

									if (submit.ToUpper().Contains("DOWNLOAD"))
									{
										string result = string.Empty;
										if (frm["SelList"] != "")
										{
											ViewBag.SelectedItem = frm["SelList"];
											TempData["SelectedItem"] = frm["SelList"];
											selbatch = frm["SelList"];
										}
										Search = "refno like '%%' and refno in (" + commaString + ")";

										if (submit.ToLower().Contains("data"))
										{
											ds1 = objDB.DownloadPrivateData(Convert.ToInt32(2), selbatch, adminid, Search, out ErrStatus); // DownloadPrivateDataSPNew
										}
										else if (submit.ToLower().Contains("subjects"))
										{
											ds1 = objDB.DownloadPrivateData(Convert.ToInt32(3), selbatch, adminid, Search, out ErrStatus); // DownloadPrivateDataSPNew
										}
										else if (submit.ToLower().Contains("photo"))
										{
											ds1 = objDB.DownloadPrivateData(Convert.ToInt32(4), selbatch, adminid, Search, out ErrStatus); // DownloadPrivateDataSPNew
										}
										else if (submit.ToLower().Contains("form"))
										{
											ds1 = objDB.DownloadPrivateData(Convert.ToInt32(10), selbatch, adminid, Search, out ErrStatus); // DownloadPrivateDataSPNew
										}
										// DataSet ds1 = objDB.DownloadRegNoAgainstID(commaString, "O", out OutStatus); // For all Regno alloted 
										fileName1 = submit + "_" + firmuser + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

										if (ds1.Tables[0].Rows.Count > 0)
										{
											ViewBag.Message = "Private Data Downloaded Successfully of Batch : " + selbatch;
											ViewData["Result"] = "1";
											ViewBag.TotalCount = ds1.Tables[0].Rows.Count;
											if (submit.ToLower().Contains("photo"))
											{
												if (ds1 == null)
												{
													return RedirectToAction("DownloadPrivateData", "Admin");
												}
												else
												{
													if (ds1.Tables[0].Rows.Count > 0)
													{
														result = objDB.DownloadZIPFileFirmExam(ds1.Tables[0]);
													}
												}
											}
											else if (submit.ToLower().Contains("form"))
											{
												Session["downloadprintform"] = ds1;
												return RedirectToAction("BulkPrivateCandidatePrintForm", "Admin");

											}
											else
											{
												//DataTable dt = ds1.Tables[0];
												using (XLWorkbook wb = new XLWorkbook())
												{
													wb.Worksheets.Add(ds1.Tables[0]);
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
											}
										}

									}
									#endregion DownloadFile                          
									return View(am);
								}
								else
								{
									if (dtexport != null)
									{
										ExportDataFromDataTable(dtexport, "Error_DataDownload");
									}
									ViewData["Result"] = "-1";
									ViewBag.Message = CheckMis;
									return View(am);
								}
							}
							else
							{

								ViewData["Result"] = "-2";
								ViewBag.Message = "Please Upload Only .xls file only";
								return View(am);
							}
						}

					}
					else
					{
						//ViewData["Result"] = "-4";
						// ViewBag.Message = "Please select .xls file only";
						//return View();
					}
				}
				else if (Filevalue.ToUpper() == "DATE")
				{
					#region DownloadFile

					if (submit.ToUpper().Contains("DOWNLOAD"))
					{
						string selbatch = string.Empty;
						Search = "a.refno like '%%' ";
						if (frm["SelList"] != "")
						{
							ViewBag.SelectedItem = frm["SelList"];
							TempData["SelectedItem"] = frm["SelList"];
							selbatch = frm["SelList"];
						}
						if (frm["FromDate"] != "")
						{
							ViewBag.FromDate = frm["FromDate"];
							TempData["FromDate"] = frm["FromDate"];
							Search += " and CONVERT(DATETIME, CONVERT(DATE, CONVERT(varchar,updt,103),103), 103)>=CONVERT(DATETIME,CONVERT(DATE, CONVERT(varchar,'" + frm["FromDate"].ToString() + "',103),103), 103)";
						}
						if (frm["ToDate"] != "")
						{
							ViewBag.ToDate = frm["ToDate"];
							TempData["ToDate"] = frm["ToDate"];
							Search += " and CONVERT(DATETIME, CONVERT(DATE, CONVERT(varchar,updt,103),103), 103)<=CONVERT(DATETIME,CONVERT(DATE, CONVERT(varchar,'" + frm["ToDate"].ToString() + "',103),103), 103)";
						}

						if (submit.ToLower().Contains("data"))
						{
							ds1 = objDB.DownloadPrivateData(Convert.ToInt32(2), selbatch, adminid, Search, out ErrStatus); // DownloadPrivateDataSPNew
						}
						else if (submit.ToLower().Contains("subjects"))
						{
							ds1 = objDB.DownloadPrivateData(Convert.ToInt32(3), selbatch, adminid, Search, out ErrStatus); // DownloadPrivateDataSPNew
						}
						else if (submit.ToLower().Contains("photo"))
						{
							ds1 = objDB.DownloadPrivateData(Convert.ToInt32(4), selbatch, adminid, Search, out ErrStatus); // DownloadPrivateDataSPNew
						}
						// DataSet ds1 = objDB.DownloadRegNoAgainstID(commaString, "O", out OutStatus); // For all Regno alloted 
						string fileName1 = submit + "_" + firmuser + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

						if (ds1 == null)
						{
							ViewBag.Message = ErrStatus.ToString();
							ViewData["Result"] = "5";
							ViewBag.TotalCount = 0;
						}
						else if (ds1.Tables[0].Rows.Count > 0)
						{
							ViewBag.Message = "Data Downloaded Successfully";
							ViewData["Result"] = "1";
							ViewBag.TotalCount = ds1.Tables[0].Rows.Count;
							if (submit.ToLower().Contains("photo"))
							{
								if (ds1 == null)
								{
									return RedirectToAction("DownloadPrivateData", "Admin");
								}
								else
								{
									if (ds1.Tables[0].Rows.Count > 0)
									{
										string result = objDB.DownloadZIPFileFirmExam(ds1.Tables[0]);
									}
								}
							}
							else
							{

								//DataTable dt = ds1.Tables[0];
								using (XLWorkbook wb = new XLWorkbook())
								{
									wb.Worksheets.Add(ds1.Tables[0]);
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
							}
						}

					}
					#endregion DownloadFile                          
				}
				return View(am);
			}
			catch (Exception ex)
			{
				// ////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewBag.Message = "Error: " + ex.Message;
				ViewData["Result"] = "50";
			}
			return View(am);
		}



		public ActionResult BulkPrivateCandidatePrintForm(DataSet dt)
		{
			if (Session["UserName"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			try
			{
				PrivateCandidateModels MS = new PrivateCandidateModels();
				AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
				if (Session["downloadprintform"] != null)
				{
					dt = (DataSet)Session["downloadprintform"];
					MS.StoreAllData = dt;
					ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
				}
				return View(MS);
			}
			catch (Exception ex)
			{
				return RedirectToAction("DownloadPrivateData", "Admin");
			}
		}


		#endregion Download Private Data

		#region DEO SCHOOL Data Download
		public ActionResult DownloadSchoolDistWise(AdminModels AM) // Download Admin DEO Data file
		{
			string district = null;
			try
			{
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				if (Session["DistAllow"].ToString() != "")
				{
					district = Session["DistAllow"].ToString();

					if (Session["UserName"] != null)
					{
						DataSet ds1 = new AbstractLayer.DEODB().DownloadDeoSchoolDistWise(district);
						if (ds1.Tables.Count > 0)
						{
							if (ds1.Tables[0].Rows.Count > 0)
							{
								if (ds1.Tables[0] != null)
								{
									ExportDataFromDataTable(ds1.Tables[0], Session["UserName"].ToString().ToUpper() + "_SchoolListDistwise".ToUpper());
								}
								ViewData["Result"] = "1";
								return RedirectToAction("Welcome", "Admin");
							}
							else
							{
								return RedirectToAction("Index", "Admin");
							}
						}
						else
						{
							ViewBag.Message = "Data Not Found";
							ViewData["Result"] = "0";
							return RedirectToAction("Index", "Admin");
						}
					}
				}
				else
				{ return RedirectToAction("Welcome", "Admin"); }
				return View(AM);
			}
			catch (Exception ex)
			{
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View(AM);
			}
		}
		#endregion DEO SCHOOL Data Download

		#region Cce Prac Marks Download

		public ActionResult CcePracMarksDownload(string id, AdminModels am)
		{
			if (Session["UserName"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			string firmuser = Session["UserName"].ToString();
			string ErrStatus = string.Empty;

			var itemsch = new SelectList(new[]{new {ID="1",Name="C.C.E"},new {ID="2",Name="Elective Theory"},
			new {ID="3",Name="Prac"},}, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			ViewBag.SelectedItem = "0";

			var itemET = new SelectList(new[]{new {ID="4",Name="ALL"},new {ID="1",Name="Regular"},new {ID="2",Name="Open"},
			new {ID="3",Name="Pvt"},}, "ID", "Name", 1);
			ViewBag.MyET = itemET.ToList();
			ViewBag.SelectedET = "0";

			var itemcls = new SelectList(new[] { new { ID = "4", Name = "Senior" }, new { ID = "2", Name = "Matric" }, }, "ID", "Name", 1);
			ViewBag.Mycls = itemcls.ToList();
			ViewBag.Selectedcls = "0";
			return View(am);
		}

		[HttpPost]
		public ActionResult CcePracMarksDownload(string id, AdminModels am, FormCollection frm, string submit)
		{
			if (Session["UserName"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			try
			{
				var itemsch = new SelectList(new[]{new {ID="1",Name="C.C.E"},new {ID="2",Name="Elective Theory"},
			new {ID="3",Name="Prac"},}, "ID", "Name", 1);
				ViewBag.MySch = itemsch.ToList();
				ViewBag.SelectedItem = "0";


				var itemET = new SelectList(new[]{new {ID="4",Name="ALL"},new {ID="1",Name="Regular"},new {ID="2",Name="Open"},
			new {ID="3",Name="Pvt"},}, "ID", "Name", 1);
				ViewBag.MyET = itemET.ToList();
				ViewBag.SelectedET = "0";


				var itemcls = new SelectList(new[] { new { ID = "4", Name = "Senior" }, new { ID = "2", Name = "Matric" }, }, "ID", "Name", 1);
				ViewBag.Mycls = itemcls.ToList();
				ViewBag.Selectedcls = "0";


				string ErrStatus = string.Empty;
				string Search = string.Empty;
				DataSet ds1 = new DataSet();
				string firmuser = Session["UserName"].ToString();

				string Filevalue = string.Empty;
				if (frm["Filevalue"] == null) { }
				else
				{
					Filevalue = frm["Filevalue"].ToString();
				}

				string SelList = string.Empty;
				if (frm["SelList"] == null) { }
				else
				{
					SelList = frm["SelList"].ToString();
				}

				string SelClass = string.Empty;
				if (frm["SelClass"] == null) { }
				else
				{
					SelClass = frm["SelClass"].ToString();
				}

				string SelET = string.Empty;
				string RP = string.Empty;
				if (frm["SelET"] == null) { }
				else
				{
					SelET = frm["SelET"].ToString();
					RP = frm["SelET"] == "1" ? "R" : frm["SelET"] == "2" ? "O" : frm["SelET"] == "3" ? "P" : "A";
				}

				//  Download Data by file
				string AdminType = Session["AdminType"].ToString();
				int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				string fileLocation = "";
				string filename = "";
				if (Filevalue.ToUpper() == "STDIDMIS")
				{
					if (am.file != null)
					{
						filename = Path.GetFileName(am.file.FileName);

						DataSet ds = new DataSet();
						if (am.file.ContentLength > 0)  //(Request.Files["file"].ContentLength > 0
						{
							string fileName1 = firmuser + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
							string fileExtension = System.IO.Path.GetExtension(am.file.FileName);
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
								am.file.SaveAs(fileLocation);
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
								using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
								{
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
									string query = string.Format("Select * from [{0}]", excelSheets[0]);
									using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
									{
										dataAdapter.Fill(ds);
									}
								}
								DataTable dtexport;
								string RP1 = "";
								if (frm["SelET"] != "")
								{
									RP1 = frm["SelET"] == "1" ? "CR" : frm["SelET"] == "2" ? "CO" : frm["SelET"] == "3" ? "CP" : "AP";
								}
								string CheckMis = objDB.CheckCcePracMarksDownloadMis(ds, out dtexport, RP1);
								if (CheckMis == "")
								{
									DataTable dt1 = ds.Tables[0];
									if (dt1.Columns.Contains("Status"))
									{
										dt1.Columns.Remove("Status");
									}


									// string Result1 = "";
									int OutStatus = 0;
									// int REGNOLOT = 0;
									string commaString = "";
									for (int i = 0; i < dt1.Rows.Count; i++)
									{
										commaString = commaString + dt1.Rows[i]["roll"].ToString();
										//commaString += (i < dt1.Rows.Count) ? "," : string.Empty;
										commaString += (i < dt1.Rows.Count - 1) ? "," : string.Empty;
									}

									// Download
									#region DownloadFile

									if (submit.ToUpper().Contains("DOWNLOAD"))
									{
										string result = string.Empty;
										Search = "roll like '%%' and roll in (" + commaString + ")";

										ds1 = objDB.CcePracMarksDownload(Convert.ToInt32(SelList), Convert.ToInt32(SelClass), RP, Convert.ToInt32(AdminId), Search, out ErrStatus); // CcePracMarksDownloadSPNew

										string type1 = frm["SelList"] == "1" ? "CCE" : frm["SelList"] == "2" ? "Theory" : "Prac";
										fileName1 = type1.ToUpper() + "_" + firmuser.ToUpper() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210


										if (ds1 == null || ds1.Tables[0].Rows.Count == 0)
										{
											ViewBag.Message = ErrStatus.ToString();
											ViewData["Result"] = "5";
											ViewBag.TotalCount = 0;
										}
										else if (ds1.Tables[0].Rows.Count > 0)
										{
											ViewBag.Message = " Data Downloaded Successfully";
											ViewData["Result"] = "1";
											ViewBag.TotalCount = ds1.Tables[0].Rows.Count;

											if (ds1.Tables[0] != null)
											{
												ExportDataFromDataTable(ds1.Tables[0], fileName1);
											}

										}

									}
									#endregion DownloadFile                          
									return View(am);
								}
								else
								{
									if (dtexport != null)
									{
										ExportDataFromDataTable(dtexport, "Error_MarksDataDownload");
									}
									ViewData["Result"] = "-1";
									ViewBag.Message = CheckMis;
									return View(am);
								}
							}
							else
							{

								ViewData["Result"] = "-2";
								ViewBag.Message = "Please Upload Only .xls file only";
								return View(am);
							}
						}

					}
					else
					{
						//ViewData["Result"] = "-4";
						// ViewBag.Message = "Please select .xls file only";
						//return View();
					}
				}
				else if (Filevalue.ToUpper() == "DATE")
				{
					#region DownloadFile

					if (submit.ToUpper().Contains("DOWNLOAD"))
					{
						Search = " e.roll like '%%' ";
						string type = string.Empty;

						if (SelList == "1" && RP == "R") //CCE Regular
						{

							if (frm["FromDate"] != "")
							{
								ViewBag.FromDate = frm["FromDate"];
								TempData["FromDate"] = frm["FromDate"];
								// Search += " and  CONVERT(DATETIME, DATEDIFF(DAY, 0, a.UPDT)) >=convert(DATETIME,'" + frm["FromDate"].ToString() + "',103) ";
								Search += " and   (CONVERT(DATETIME, CONVERT(varchar,INSERTDT,103), 103)>=CONVERT(DATETIME, CONVERT(varchar,'" + frm["FromDate"].ToString() + "',103), 103)  ";
								Search += " or   CONVERT(DATETIME, CONVERT(varchar,ccedate,103), 103)>=CONVERT(DATETIME, CONVERT(varchar,'" + frm["FromDate"].ToString() + "',103), 103) ) ";
							}
							if (frm["ToDate"] != "")
							{
								ViewBag.ToDate = frm["ToDate"];
								TempData["ToDate"] = frm["ToDate"];
								// Search += "   and CONVERT(DATETIME, DATEDIFF(DAY, 0, a.UPDT)) <=  convert(DATETIME,'" + frm["ToDate"].ToString() + "',103)";
								Search += "  and ( CONVERT(DATETIME, CONVERT(varchar,INSERTDT,103), 103)<=CONVERT(DATETIME, CONVERT(varchar,'" + frm["ToDate"].ToString() + "',103), 103)  ";
								Search += " or  CONVERT(DATETIME, CONVERT(varchar,ccedate,103), 103)<=CONVERT(DATETIME, CONVERT(varchar,'" + frm["ToDate"].ToString() + "',103), 103) ) ";

							}

						}

						else if (SelList == "2" && SelClass == "2") //theory matric
						{
							if (frm["FromDate"] != "")
							{
								ViewBag.FromDate = frm["FromDate"];
								TempData["FromDate"] = frm["FromDate"];
								Search += "  and ( CONVERT(DATETIME, CONVERT(varchar,GradingDT,103), 103)>=CONVERT(DATETIME, CONVERT(varchar,'" + frm["FromDate"].ToString() + "',103), 103)  ";
								Search += " or   CONVERT(DATETIME, CONVERT(varchar,graddate,103), 103)>=CONVERT(DATETIME, CONVERT(varchar,'" + frm["FromDate"].ToString() + "',103), 103) ) ";

							}
							if (frm["ToDate"] != "")
							{
								ViewBag.ToDate = frm["ToDate"];
								TempData["ToDate"] = frm["ToDate"];
								Search += "  and ( CONVERT(DATETIME, CONVERT(varchar,GradingDT,103), 103)<=CONVERT(DATETIME, CONVERT(varchar,'" + frm["ToDate"].ToString() + "',103), 103)  ";
								Search += " or   CONVERT(DATETIME, CONVERT(varchar,graddate,103), 103)<=CONVERT(DATETIME, CONVERT(varchar,'" + frm["ToDate"].ToString() + "',103), 103) ) ";
							}

						}

						else if (SelList == "3") //practical
						{
							if (frm["FromDate"] != "")
							{
								ViewBag.FromDate = frm["FromDate"];
								TempData["FromDate"] = frm["FromDate"];
								Search += " and   (CONVERT(DATETIME, CONVERT(varchar,PracInsDate,103), 103)>=CONVERT(DATETIME, CONVERT(varchar,'" + frm["FromDate"].ToString() + "',103), 103)  ";
								Search += " or   CONVERT(DATETIME, CONVERT(varchar,FinalPracDT,103), 103)>=CONVERT(DATETIME, CONVERT(varchar,'" + frm["FromDate"].ToString() + "',103), 103) ) ";


							}
							if (frm["ToDate"] != "")
							{
								ViewBag.ToDate = frm["ToDate"];
								TempData["ToDate"] = frm["ToDate"];
								Search += "  and ( CONVERT(DATETIME, CONVERT(varchar,PracInsDate,103), 103)<=CONVERT(DATETIME, CONVERT(varchar,'" + frm["ToDate"].ToString() + "',103), 103)  ";
								Search += " or   CONVERT(DATETIME, CONVERT(varchar,FinalPracDT,103), 103)<=CONVERT(DATETIME, CONVERT(varchar,'" + frm["ToDate"].ToString() + "',103), 103) ) ";
							}

						}


						ds1 = objDB.CcePracMarksDownload(Convert.ToInt32(SelList), Convert.ToInt32(SelClass), RP, Convert.ToInt32(AdminId), Search, out ErrStatus); // CcePracMarksDownloadSPNew


						string type1 = frm["SelList"] == "1" ? "CCE" : frm["SelList"] == "2" ? "Theory" : "Prac";
						string fileName1 = type1.ToUpper() + "_" + firmuser.ToUpper() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
						if (ds1 == null || ds1.Tables[0].Rows.Count == 0)
						{
							ViewBag.Message = ErrStatus.ToString();
							ViewData["Result"] = "5";
							ViewBag.TotalCount = 0;
						}
						else if (ds1.Tables[0].Rows.Count > 0)
						{
							ViewBag.Message = "Data Downloaded Successfully";
							ViewData["Result"] = "1";
							ViewBag.TotalCount = ds1.Tables[0].Rows.Count;

							if (ds1.Tables[0] != null)
							{
								ExportDataFromDataTable(ds1.Tables[0], fileName1);
							}

						}

					}
					#endregion DownloadFile                          
				}
				return View(am);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewBag.Message = "Error: " + ex.Message;
				ViewData["Result"] = "50";
				return View();
			}
		}

		#endregion Cce Prac Marks Download


		#region Practical Final Submission Unlock
		public ActionResult PracticalSubmissionUnlocked()
		{
			if (Session["UserName"] != null)
			{

				return View();
			}
			else
			{
				return RedirectToAction("Index", "Admin");
			}
		}

		[HttpPost]
		public ActionResult PracticalSubmissionUnlocked(AdminModels AM, FormCollection frm, string Category, string submit) // HttpPostedFileBase file
		{
			try
			{
				if (submit != null)
				{
					if (submit.ToUpper() == "DOWNLOAD")
					{
					}
				}


				string firm = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
				string id = frm["Filevalue"].ToString();
				Category = id;
				if (Session["UserName"] != null)
				{


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
						string fileName1 = "";

						if (id.ToString().ToUpper() == "PRAC")
						{
							fileName1 = "PracticalSubmissionUnlocked_" + "_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
						}
						//else if (id.ToString().ToUpper() == "STD")
						//{
						//    fileName1 = "PracCENT_STD_" + "_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
						//}
						else
						{
							return RedirectToAction("PracticalSubmissionUnlocked", "Admin");
						}
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
							using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
							{
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
								string query = string.Format("Select * from [{0}]", excelSheets[0]);
								using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
								{
									dataAdapter.Fill(ds);
								}
							}

							string CheckMis = "";
							DataTable dtexport = new DataTable();
							string ColNameCls = "";
							int flg = 0;
							if (id.ToString().ToUpper() == "PRAC")
							{
								flg = 1;
								CheckMis = objDB.CheckPracticalSubmissionUnlocked(ds, out dtexport);
							}

							if (CheckMis == "" && flg == 1)
							{
								DataSet ds1 = new DataSet();
								DataTable dt1 = ds.Tables[0];
								//if (dt1.Columns.Contains("STATUS"))
								//{
								//    dt1.Columns.Remove("STATUS");
								//}
								dt1.AcceptChanges();
								string Result1 = "";
								int OutStatus = 0;
								string OutResult = "0";
								string OutError = "0";
								if (id.ToString().ToUpper() == "PRAC")
								{
									for (int i = 0; i < dt1.Rows.Count; i++)
									{

										string CLASS = dt1.Rows[i][0].ToString();
										string RP = dt1.Rows[i][1].ToString();
										string PCENT = dt1.Rows[i][2].ToString();
										string SUB = dt1.Rows[i][3].ToString();
										string LOT = dt1.Rows[i][4].ToString();
										string LastDate = dt1.Rows[i][5].ToString();
										DataTable dtResult = objDB.PracticalSubmissionUnlocked(LastDate, CLASS, RP, PCENT, SUB, LOT, AdminId, out OutError);// OutStatus mobile
										if (OutError == "1")
										{ dt1.Rows[i]["Status"] = "Successfully Unlocked"; }
										else
										{ dt1.Rows[i]["Status"] = "Failure : " + OutError; }

									}

									ViewBag.Message = "File Uploaded Successfully";
									ViewData["Result"] = "1";
									if (dt1 != null)
									{
										ExportDataFromDataTable(dt1, "PracticalSubmissionUnlocked");
									}
									return View();
								}
								else
								{
									ViewBag.Message = "Please Check File Structure";
									ViewData["Result"] = "5";
									return View();
								}
							}
							else
							{
								// CheckMis = "Selected Class and File class not matched , please check ";
								if (dtexport != null)
								{
									ExportDataFromDataTable(dtexport, firm);
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
				else { return RedirectToAction("Index", "Admin"); }
			}
			catch (Exception ex)
			{
				//////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View();
			}
			return View();
		}

		#endregion  Practical Final Submission Unlock


		#region Practical Cent Update Master
		[AdminLoginCheckFilter]
		public ActionResult PracticalCentUpdateMaster()
		{
			if (Session["UserName"] != null)
			{
				var itemsch = new SelectList(new[] { new { ID = "1", Name = "Regular/Open" }, new { ID = "2", Name = "Pvt" }, }, "ID", "Name", 1);
				ViewBag.MySch = itemsch.ToList();
				ViewBag.SelectedItem = "0";

				var itemcls = new SelectList(new[] { new { ID = "3", Name = "Senior" }, new { ID = "2", Name = "Matric" }, }, "ID", "Name", 1);
				ViewBag.Mycls = itemcls.ToList();
				ViewBag.Selectedcls = "0";
				return View();
			}
			else
			{
				return RedirectToAction("Index", "Admin");
			}
		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult PracticalCentUpdateMaster(AdminModels AM, FormCollection frm, string Category, string submit) // HttpPostedFileBase file
		{
			try
			{
				if (submit != null)
				{
					if (submit.ToUpper() == "DOWNLOAD")
					{
					}
				}

				var itemsch = new SelectList(new[] { new { ID = "1", Name = "Regular/Open" }, new { ID = "2", Name = "Pvt" }, }, "ID", "Name", 1);
				ViewBag.MySch = itemsch.ToList();
				ViewBag.SelectedItem = "0";


				var itemcls = new SelectList(new[] { new { ID = "3", Name = "Senior" }, new { ID = "2", Name = "Matric" }, }, "ID", "Name", 1);
				ViewBag.Mycls = itemcls.ToList();
				ViewBag.Selectedcls = "0";

				string firm = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
				string id = frm["Filevalue"].ToString();
				Category = id;
				if (Session["UserName"] != null)
				{
					string SelList = string.Empty;
					if (frm["SelList"] == null) { }
					else
					{
						SelList = frm["SelList"].ToString();
					}


					string SelClass = string.Empty;
					if (frm["SelClass"] == null) { }
					else
					{
						SelClass = frm["SelClass"].ToString();
					}

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
						string fileName1 = "";

						if (id.ToString().ToUpper() == "CENT")
						{
							fileName1 = "PracCENT_" + SelList + "_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
						}
						else if (id.ToString().ToUpper() == "STD")
						{
							fileName1 = "PracCENT_STD_" + "_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
						}
						else
						{
							return RedirectToAction("PracticalCentUpdateMaster", "Admin");
						}
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
							using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
							{
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
								string query = string.Format("Select * from [{0}]", excelSheets[0]);
								using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
								{
									dataAdapter.Fill(ds);
								}
							}

							string CheckMis = "";
							DataTable dtexport = new DataTable();
							string ColNameCls = "";
							int flg = 0;
							if (id.ToString().ToUpper() == "STD")
							{
								flg = 1;
								CheckMis = objDB.CheckStdPracticalCentUpdateMaster(ds, out dtexport);
							}

							else
							{

								if (SelList.ToString() == "1")
								{
									ColNameCls = ds.Tables[0].Rows[0]["class"].ToString();
									if (SelClass == ColNameCls)
									{
										flg = 1;
										CheckMis = objDB.CheckRegOpenPracticalCentUpdateMaster(ds, ColNameCls, out dtexport);
									}
								}
								else if (SelList.ToString() == "2")
								{
									flg = 1;
									CheckMis = objDB.CheckPrivatePracticalCentUpdateMaster(ds, out dtexport);
								}
								else
								{
									ViewBag.Message = "Please Check File Structure";
									ViewData["Result"] = "5";
									return View();
								}

							}

							if (CheckMis == "" && flg == 1)
							{
								DataSet ds1 = new DataSet();
								DataTable dt1 = ds.Tables[0];
								if (dt1.Columns.Contains("STATUS"))
								{
									dt1.Columns.Remove("STATUS");
								}
								dt1.AcceptChanges();
								string Result1 = "";
								int OutStatus = 0;
								string OutResult = "0";
								string OutError = "0";
								if (id.ToString().ToUpper() == "STD")
								{
									DataTable dtResult = objDB.PracCentSTD(dt1, AdminId, SelList, SelClass, out OutError);// OutStatus mobile
								}
								else if (id.ToString().ToUpper() == "CENT")
								{
									DataTable dtResult = objDB.PracticalCentUpdateMaster(dt1, AdminId, SelList, SelClass, out OutError);// OutStatus mobile
								}
								else
								{
									ViewBag.Message = "Please Check File Structure";
									ViewData["Result"] = "5";
									return View();
								}

								if (OutError == "1")
								{
									ViewBag.Message = "File Uploaded Successfully";
									ViewData["Result"] = "1";
								}
								else
								{
									if (OutError.ToLower().Contains("update"))
									{
										ViewBag.Message = "File Not Uploaded Successfully- Duplicate Records";
									}
									else
									{ ViewBag.Message = "File Not Uploaded Successfully"; }
									//ViewBag.Message = "File Not Uploaded Successfully";
									ViewData["Result"] = "0";
								}
								return View();
							}
							else
							{

								//CheckMis = "Selected Class and File class not matched , please check ";
								if (dtexport != null)
								{
									ExportDataFromDataTable(dtexport, firm);
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
				else { return RedirectToAction("Index", "Admin"); }
			}
			catch (Exception ex)
			{
				//////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View();
			}
			return View();
		}

		#endregion Private Cent Update Master


		#region Common CalculateFee Admin

		[AdminLoginCheckFilter]
		public ActionResult CommonCalculateFeeAdmin(PrivatePaymentformViewModel pfvm)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");
			if (Session["Adminid"] == null || Session["Adminid"].ToString() == "")
			{ return RedirectToAction("Index", "Admin"); }
			Session["CommonPaymentForm"] = null;
			List<SelectListItem> itemType = new List<SelectListItem>();
			DataSet dsType = objDB.GetFeeCodeMaster(1, 0);//GetFeeCodeMaster
			if (dsType.Tables.Count > 0)
			{
				// fee code
				if (dsType.Tables[2].Rows.Count > 0)
				{
					foreach (System.Data.DataRow dr in dsType.Tables[2].Rows)
					{
						itemType.Add(new SelectListItem { Text = @dr["FeeCat"].ToString(), Value = @dr["FeeCode"].ToString() });
					}
					ViewBag.FeeCodeList = itemType.ToList();
				}
			}
			return View(pfvm);
		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult CommonCalculateFeeAdmin(string cmd, string AppNo, string lumsumfine, string lumsumremarks, string ValidDate, PrivatePaymentformViewModel pfvm, FormCollection frm)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			string id = "";
			ViewBag.date = DateTime.Now.ToString("dd/MM/yyyy");
			if (pfvm.RefNo == null || pfvm.RefNo == "")
			{ return RedirectToAction("CommonCalculateFeeAdmin", "Admin"); }
			else
			{
				List<SelectListItem> itemType = new List<SelectListItem>();
				DataSet dsType = objDB.GetFeeCodeMaster(1, 0);//for all feecode
				if (dsType.Tables.Count > 0)
				{
					// fee code
					if (dsType.Tables[2].Rows.Count > 0)
					{
						foreach (System.Data.DataRow dr in dsType.Tables[2].Rows)
						{
							itemType.Add(new SelectListItem { Text = @dr["FeeCat"].ToString(), Value = @dr["FeeCode"].ToString() });
						}
						ViewBag.FeeCodeList = itemType.ToList();
					}
				}

				ViewBag.AppNo = pfvm.RefNo.ToString();
				ViewBag.FeeCode = pfvm.FeeCode.ToString();
				if (cmd.ToLower().Contains("calculate"))
				{
					string RefNo = pfvm.RefNo.ToString();
					string form = pfvm.FeeCode.ToString();
					DataSet ds = objDB.CommonCalculateFeeAdmin(RefNo, form);
					pfvm.PaymentFormData = ds;
					if (pfvm.PaymentFormData == null || pfvm.PaymentFormData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						if (pfvm.FeeCode == "45" || pfvm.FeeCode == "59" || pfvm.FeeCode == "65" || pfvm.FeeCode == "66" || pfvm.FeeCode == "70")
						{
							pfvm.category = ds.Tables[0].Rows[0]["Category"].ToString();
							ViewBag.FormStatus = ds.Tables[0].Rows[0]["FormStatus"].ToString();
							pfvm.RefNo = ds.Tables[0].Rows[0]["RefNo"].ToString();
							pfvm.roll = ds.Tables[0].Rows[0]["Roll"].ToString();
							pfvm.Name = ds.Tables[0].Rows[0]["Name"].ToString();
							pfvm.Dist = ds.Tables[0].Rows[0]["Dist"].ToString();
							pfvm.District = ds.Tables[0].Rows[0]["DISTNM"].ToString();


							//fee details
							pfvm.TotalFees = Convert.ToInt32(pfvm.PaymentFormData.Tables[1].Rows[0]["fee"].ToString());
							pfvm.TotalLateFees = 0;
							pfvm.TotalFinalFees = Convert.ToInt32(pfvm.PaymentFormData.Tables[1].Rows[0]["totfee"].ToString());
							string AmountInWords = new AbstractLayer.DBClass().GetAmountInWords(pfvm.TotalFinalFees);
							pfvm.TotalFeesInWords = AmountInWords;

							pfvm.FeeCode = pfvm.PaymentFormData.Tables[1].Rows[0]["FEECODE"].ToString();
							pfvm.FeeCategory = pfvm.PaymentFormData.Tables[1].Rows[0]["FEECAT"].ToString();
							pfvm.BankLastDate = Convert.ToDateTime(pfvm.PaymentFormData.Tables[1].Rows[0]["BankLastdate"].ToString());
							pfvm.BankCode = "203";
							Session["CommonPaymentForm"] = pfvm;
							ViewData["FeeStatus"] = "1";
							return View(pfvm);
						}
						else
						{


							ViewBag.FormStatus = ds.Tables[0].Rows[0]["FormStatus"].ToString();

							pfvm.Class = ds.Tables[1].Rows[0]["class"].ToString();
							if (pfvm.Class == "10")
							{
								pfvm.Class = "Matriculation";
							}
							if (pfvm.Class == "12")
							{
								pfvm.Class = "Senior Secondary";
							}
							pfvm.ExamType = ds.Tables[1].Rows[0]["rp"].ToString();
							if (pfvm.ExamType == "R")
							{
								pfvm.ExamType = "Regular";
							}
							if (pfvm.ExamType == "O")
							{
								pfvm.ExamType = "Open";
							}
							if (pfvm.ExamType == "P")
							{
								pfvm.ExamType = "Private";
							}
							pfvm.category = ds.Tables[0].Rows[0]["cat"].ToString();
							if (pfvm.category == "R")
							{
								pfvm.category = "Reappear/Compartment";
							}
							if (pfvm.category == "D")
							{
								pfvm.category = "Division improvement";
							}
							if (pfvm.category == "A")
							{
								pfvm.category = "Additional subject";
							}

							pfvm.Name = ds.Tables[0].Rows[0]["name"].ToString();
							pfvm.RegNo = ds.Tables[0].Rows[0]["regno"].ToString();
							pfvm.RefNo = ds.Tables[0].Rows[0]["refno"].ToString();
							pfvm.roll = ds.Tables[0].Rows[0]["roll"].ToString();
							pfvm.Mobile = ds.Tables[0].Rows[0]["Mobile"].ToString();
							pfvm.Dist = ds.Tables[0].Rows[0]["homedistco"].ToString();
							pfvm.District = ds.Tables[0].Rows[0]["DISTNM"].ToString();
							pfvm.SchoolCode = ds.Tables[0].Rows[0]["schl"].ToString();
							pfvm.SchoolName = ds.Tables[0].Rows[0]["SCHLE"].ToString(); // Schollname with station and dist 
							ViewBag.TotalCount = pfvm.PaymentFormData.Tables[0].Rows.Count;
							DataSet dscalFee = ds; //(DataSet)Session["CalculateFee"];
							pfvm.TotalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["fee"].ToString());
							pfvm.TotalLateFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["latefee"].ToString());
							pfvm.TotalFinalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["totfee"].ToString());
							pfvm.TotalFeesInWords = dscalFee.Tables[1].Rows[0]["TotalFeesWords"].ToString();
							// string rps = NumberToWords(Convert.ToInt32(dscalFee.Tables[1].Rows[0]["totfee"].ToString()));                   
							//pfvm.TotalFeesInWords = rps;                   
							pfvm.FeeDate = Convert.ToDateTime(dscalFee.Tables[1].Rows[0]["banklastdate"].ToString());
							//TotalCandidates
							pfvm.FeeCode = dscalFee.Tables[1].Rows[0]["FEECODE"].ToString();
							pfvm.FeeCategory = dscalFee.Tables[1].Rows[0]["FEECAT"].ToString();
							pfvm.BankLastDate = Convert.ToDateTime(dscalFee.Tables[1].Rows[0]["BankLastdate"].ToString());
							pfvm.BankCode = "203";
							Session["CommonPaymentForm"] = pfvm;
							ViewData["FeeStatus"] = "1";
							return View(pfvm);
						}

					}
				}
				else
				{
					PrivateChallanMasterModel CM = new PrivateChallanMasterModel();
					//
					if (pfvm.RefNo != "")
					{
						CM.FeeStudentList = pfvm.RefNo;
						PrivatePaymentformViewModel PFVMSession = (PrivatePaymentformViewModel)Session["CommonPaymentForm"];
						CM.roll = PFVMSession.roll;
						CM.FEE = Convert.ToInt32(PFVMSession.TotalFees);
						CM.latefee = Convert.ToInt32(0);
						CM.TOTFEE = Convert.ToInt32(PFVMSession.TotalFinalFees);
						CM.FEECAT = PFVMSession.FeeCategory;
						CM.FEECODE = PFVMSession.FeeCode;
						CM.FEEMODE = "CASH";
						CM.BCODE = pfvm.BankCode = "203";
						CM.BANK = pfvm.BankName = "PSEB HOD";
						CM.BANKCHRG = PFVMSession.BankCharges;
						if (CM.FEECODE == "45")
						{ CM.SchoolCode = "4"; }
						else
						{
							CM.SchoolCode = "";
						}
						CM.DIST = PFVMSession.Dist.ToString();
						CM.DISTNM = PFVMSession.District;
						CM.LOT = 1;

						//CM.SCHLREGID = PFVMSession.roll.ToString();
						//CM.FeeStudentList = PFVMSession.RefNo.ToString();

						CM.SCHLREGID = PFVMSession.roll.ToString();
						CM.FeeStudentList = PFVMSession.RefNo.ToString();
						CM.APPNO = PFVMSession.RefNo.ToString();
						CM.category = PFVMSession.category;
						CM.type = "candt";
						DateTime CHLNVDATE2;
						if (DateTime.TryParseExact(ValidDate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out CHLNVDATE2))
						{
							CM.ChallanVDateN = CHLNVDATE2;
						}
						CM.CHLNVDATE = ValidDate;
						CM.LumsumFine = Convert.ToInt32(lumsumfine);
						CM.LSFRemarks = lumsumremarks;

						string CandiMobile = PFVMSession.Mobile;

						CM.EmpUserId = adminLoginSession.AdminEmployeeUserId;
						//  string result = "";
						string result = new AbstractLayer.PrivateCandidateDB().InsertPaymentFormPrivate(CM, frm, out CandiMobile);
						//string result = new AbstractLayer.HomeDB().InsertPaymentForm(CM, frm, out CandiMobile);
						if (result == "0" || result == "")
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
							string Sms = "Your Challan no. " + result + " generated  for Catg " + CM.category + " and Ref No. " + CM.FeeStudentList + " valid till Dt " + CM.CHLNVDATE + ". Regards PSEB";
							try
							{
								string getSms = objCommon.gosms(CandiMobile, Sms);
							}
							catch (Exception) { }
							ModelState.Clear();
							//--For Showing Message---------//                   
							return RedirectToAction("GenerateChallaan", "Home", new { ChallanId = result });
						}
					}
				}
				return View(pfvm);
			}
		}
		#endregion Common CalculateFee Admin

		#region DuplicateCertificate
		public ActionResult DuplicateCertificate(string id, DuplicateCertificate dc)
		{
			TempData["SearchDuplicateCertificate"] = null;
			try
			{

				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				else
				{
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
						ViewBag.MyDist = objCommon.GetDistE(); // All District
					}

					ViewBag.MyResult = objCommon.GetAllResult().ToList();
					ViewBag.MyYear = objCommon.GetSessionYearSchoolAdmin().ToList();
					ViewBag.MySession = objCommon.GetMonth().ToList();

					ViewBag.MyClass = objCommon.GetAllClass().ToList();
					ViewBag.MyType = objDB.GetDuplicateCertificateType(0).ToList();

					var itemStatus = new SelectList(new[] { new { ID = "1", Name = "Same As Per Record" }, new { ID = "2", Name = "Record Based Correction" },
					new { ID = "3", Name = "DOB Certificate Based Correction" }, new { ID = "4", Name = "FN/MN Correction (Due to Adoption)" },
					new { ID = "5", Name = "Change of Candidate Name with Alias/NEE" },  new { ID = "6", Name = "Duplicate Certificate After Correction" }}, "ID", "Name", 1);
					ViewBag.MyStatus = itemStatus.ToList();

					if (id != null)
					{
						try
						{
							id = encrypt.QueryStringModule.Decrypt(id);
							ViewBag.Id = id;
							string Search = string.Empty;
							Search = "a.id like '%' and a.id=" + id + "  ";
							DataSet ds = objDB.ViewDuplicateCertificate(Convert.ToInt32(id), 2, Search, 1, 20); //typ2 for user
							if (ds == null || ds.Tables[0].Rows.Count == 0)
							{
								return RedirectToAction("ViewDuplicateCertificate", "Admin");
							}
							else if (ds.Tables[0].Rows.Count > 0)
							{
								//dc.DairyNo = Convert.ToInt32(ds.Tables[0].Rows[0]["DairyNo"].ToString());
								//dc.DairyDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["DairyDate"].ToString());
								dc.Class = Convert.ToInt32(ds.Tables[0].Rows[0]["Class"].ToString());
								ViewBag.SelectedClass = ds.Tables[0].Rows[0]["Class"].ToString();
								dc.Session = ds.Tables[0].Rows[0]["Session"].ToString();
								ViewBag.SelectedSession = ds.Tables[0].Rows[0]["Session"].ToString();

								dc.Roll = ds.Tables[0].Rows[0]["Roll"].ToString();
								dc.Name = ds.Tables[0].Rows[0]["Name"].ToString();
								dc.Dist = ds.Tables[0].Rows[0]["Dist"].ToString();
								ViewBag.SelectedDist = ds.Tables[0].Rows[0]["Dist"].ToString();
								dc.Remarks = ds.Tables[0].Rows[0]["Remarks"].ToString();
								dc.Mobile = ds.Tables[0].Rows[0]["Mobile"].ToString();
								dc.ObjectionLetter = ds.Tables[0].Rows[0]["ObjectionLetter"].ToString();
								dc.Address = ds.Tables[0].Rows[0]["Address"].ToString();
								dc.Year = Convert.ToInt32(ds.Tables[0].Rows[0]["Year"].ToString());
								ViewBag.SelectedYear = ds.Tables[0].Rows[0]["Year"].ToString();
								dc.IsSameAsRecord = ds.Tables[0].Rows[0]["IsSameAsRecord"].ToString();
								dc.Before = ds.Tables[0].Rows[0]["Before1"].ToString();
								dc.After = ds.Tables[0].Rows[0]["After1"].ToString();
								ViewBag.SelectedType = dc.TypeOf = ds.Tables[0].Rows[0]["TypeOf"].ToString();
								dc.IsType = Convert.ToInt32(ds.Tables[0].Rows[0]["IsType"].ToString());

								dc.FNAME = ds.Tables[0].Rows[0]["FNAME"].ToString();
								dc.MNAME = ds.Tables[0].Rows[0]["MNAME"].ToString();
								dc.ReceiptNo = ds.Tables[0].Rows[0]["ReceiptNo"].ToString();
								dc.ReceiptDate = ds.Tables[0].Rows[0]["ReceiptDate"].ToString();
								dc.FeeAmount = Convert.ToInt32(ds.Tables[0].Rows[0]["FeeAmount"].ToString());
								dc.ScanFile = ds.Tables[0].Rows[0]["ScanFile"].ToString();
								dc.IsForward = Convert.ToInt32(ds.Tables[0].Rows[0]["IsForward"].ToString());
								dc.PrevCert = ds.Tables[0].Rows[0]["PrevCert"].ToString();

								dc.Result = ds.Tables[0].Rows[0]["Result"].ToString();
								dc.ResultDt = ds.Tables[0].Rows[0]["ResultDt"].ToString();
								dc.MaxMarks = ds.Tables[0].Rows[0]["MaxMarks"].ToString();
								dc.ObtMarks = ds.Tables[0].Rows[0]["ObtMarks"].ToString();
								dc.DOB = ds.Tables[0].Rows[0]["DOB"].ToString();
								dc.REGNO = ds.Tables[0].Rows[0]["REGNO"].ToString();


								if (ds.Tables[2].Rows.Count > 0)
								{
									dc.SameAsRecordDT = ds.Tables[2];

									List<SameAsRecord> ci = new List<SameAsRecord>();
									ci = (from DataRow row in dc.SameAsRecordDT.Rows

										  select new SameAsRecord
										  {
											  TypeOf = row["TypeOf"].ToString(),
											  Before = row["Before"].ToString(),
											  After = row["After"].ToString(),
											  IsSelected = true
										  }).ToList();

									dc.SameAsRecordList = ci;
								}
							}
						}
						catch (Exception)
						{
							throw;
						}
					}

					if (dc.SameAsRecordList == null)
					{
						List<SameAsRecord> ci = new List<SameAsRecord> { new SameAsRecord { TypeOf = "", Before = "", After = "" } };
						ViewBag.SameAsRecordList = ci;
						dc.SameAsRecordList = ci;
					}


				}
				return View(dc);
			}
			catch (Exception)
			{
				return View();
			}
		}

		[HttpPost]
		public ActionResult DuplicateCertificate(string id, DuplicateCertificate dc, string cmd, FormCollection frm)
		{
			if (Session["AdminId"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}
			else
			{
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
				//var sessionsrc = new SelectList(new[] { new { ID = "2017", Name = "2017" }, new { ID = "2016", Name = "2016" }, }, "ID", "Name", 1);
				//ViewBag.MySession = sessionsrc.ToList();
				ViewBag.MyResult = objCommon.GetAllResult().ToList();

				ViewBag.MyYear = objCommon.GetSessionYearSchoolAdmin().ToList();
				ViewBag.MySession = objCommon.GetMonth().ToList();

				ViewBag.MyClass = objCommon.GetAllClass().ToList();
				ViewBag.MyType = objDB.GetDuplicateCertificateType(0).ToList();

				var itemStatus = new SelectList(new[] { new { ID = "1", Name = "Same As Per Record" }, new { ID = "2", Name = "Record Based Correction" },
					new { ID = "3", Name = "DOB Certificate Based Correction" }, new { ID = "4", Name = "FN/MN Correction (Due to Adoption)" },
									   new { ID = "5", Name = "Change of Candidate Name with Alias/NEE" },  new { ID = "6", Name = "Duplicate Certificate After Correction" }}, "ID", "Name", 1);
				ViewBag.MyStatus = itemStatus.ToList();

				List<SameAsRecord> ci = new List<SameAsRecord> { new SameAsRecord { TypeOf = "", Before = "", After = "" } };
				ViewBag.SameAsRecordList = ci;

				// End Dist Allowed

			}

			if (cmd != null)
			{
				if (cmd.ToUpper() == "RESET")
				{
					TempData.Clear();
					return RedirectToAction("DuplicateCertificate", "Admin");
				}

			}

			if (dc.Address == "" || dc.Roll == "" || dc.Name == "" || dc.Mobile == "" || frm["Class1"] == "" || frm["Dist1"] == "" || frm["Year1"] == "" || frm["Session1"] == "")
			{ ViewData["result"] = "10"; return View(dc); }
			else
			{
				if (frm["Dist1"] != "")
				{
					ViewBag.SelectedDist = frm["Dist1"];
					TempData["SelectedDist"] = frm["Dist1"];
					dc.Dist = frm["Dist1"].ToString();
				}
				if (frm["Session1"] != "")
				{
					ViewBag.SelectedSession = frm["Session1"];
					dc.Session = frm["Session1"].ToString();
				}
				if (frm["Year1"] != "")
				{
					ViewBag.SelectedYear = frm["Year1"];
					dc.Year = Convert.ToInt32(frm["Year1"].ToString());
				}
				if (frm["Class1"] != "")
				{
					ViewBag.SelectedClass = frm["Class1"];
					dc.Class = Convert.ToInt32(frm["Class1"].ToString());
				}
				if (frm["Result1"] != "")
				{
					ViewBag.SelectedResult = frm["Result1"];
					dc.Result = frm["Result1"].ToString();
				}
				// dc.IsType = 0;             
				string MyStatus = dc.IsType.ToString();
				if (MyStatus == "1")
				{ dc.IsSameAsRecord = "YES"; }
				else
				{ dc.IsSameAsRecord = "NO"; }
				if (dc.IsSameAsRecord == "NO")
				{
					if (dc.SameAsRecordList == null || dc.SameAsRecordList.Count == 0)
					{
						ViewData["result"] = "11";
						return View(dc);
					}
					else
					{
						DataTable dt = AbstractLayer.StaticDB.ConvertListToDataTable<SameAsRecord>(dc.SameAsRecordList);
						if (dt == null || dt.Rows.Count == 0)
						{ }
						else
						{
							var duplicates = dt.AsEnumerable().GroupBy(r => r[0]).Where(gr => gr.Count() > 1).ToList();
							if (duplicates.Any())
							{
								ViewBag.Duplicate = "Duplicate : " + String.Join(", ", duplicates.Select(dupl => dupl.Key));
								ViewData["result"] = "20";
								return View(dc);
							}
							if (dt.Columns.Contains("IsSelected"))
							{ dt.Columns.Remove("IsSelected"); }
							dt.AcceptChanges();
							dc.SameAsRecordDT = dt;
						}
					}
				}
				else { dc.SameAsRecordDT = null; }
				// file name
				string fileName = "";
				string fileExt = "";
				//  string obj = Guid.NewGuid().ToString().Substring(0, 5);
				//if (dc.file != null)
				//{
				//    //stdPic = Path.GetFileName(frm["file"]);
				//    fileExt = (System.IO.Path.GetExtension(dc.file.FileName).ToLower());
				//    fileName = Path.GetFileNameWithoutExtension(dc.file.FileName) + '_' + obj + '_' + DateTime.Now.ToString("ddMMyyyy") + fileExt;
				//    dc.ScanFile = "Upload/DuplicateCertificate/" + fileName;
				//}
				//
				dc.ScanFile = "";
				string OutError = "0";
				dc.id = 0;
				dc.Adminid = Convert.ToInt32(Session["AdminId"]);
				///dc.DairyDate = new DateTime(dc.DairyDate.Value.Year, dc.DairyDate.Value.Day, dc.DairyDate.Value.Month);
				string res = "";
				if (id == null)
				{
					res = objDB.DuplicateCertificate(dc, 0, out OutError);
				}
				else
				{

					id = encrypt.QueryStringModule.Decrypt(id);
					dc.id = Convert.ToInt32(id);
					ViewBag.Id = id;
					res = objDB.DuplicateCertificate(dc, 1, out OutError);
				}


				if (Convert.ToString(OutError).Length == 8)
				{
					ViewBag.DairyNo = OutError;
					string DairyNo = OutError;
					ModelState.Clear();
					if (id == null)
					{
						ViewData["result"] = "1";
						//'Dr.No 0100001 is generated on 01/09/17 for duplicate certificate application. Fee Details #2541214-Rs600-dt 31/08/17.track status https://tinyurl.com/y7ruuksy'
						// Dairy No 0100001 is generated on dated 01/09/2017 against your duplicate certificate form application. Fee recieved against Rec. No 2541214, Amt 600 on dated 31/08/2017. Visit duplicate section under board's website to track status. 
						string Sms = "Dr.No" + DairyNo + " is generated on " + DateTime.Now.ToString("dd/MM/yyyy") + " for duplicate certificate application.Fee Details " + dc.ReceiptNo + "-Rs" + dc.FeeAmount + "-dt " + dc.ReceiptDate + ".trackstatus https://tinyurl.com/y7ruuksy";
						try
						{
							string getSms = objCommon.gosms(dc.Mobile, Sms);
							// string getSms = objCommon.gosms("9711819184", Sms);
						}
						catch (Exception) { }

					}
					else
					{ ViewData["result"] = "2"; }
					if (dc.file != null)
					{
						fileExt = (System.IO.Path.GetExtension(dc.file.FileName).ToLower());
						fileName = DairyNo + '_' + DateTime.Now.ToString("ddMMyyyy") + fileExt;

						//var path = Path.Combine(Server.MapPath("~/Upload/"+ formName + "/" + dist + "/Photo"), stdPic);
						var path = Path.Combine(Server.MapPath("~/Upload/" + "DuplicateCertificate"), fileName);
						string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + "/DuplicateCertificate"));
						if (!Directory.Exists(FilepathExist))
						{
							Directory.CreateDirectory(FilepathExist);
						}
						dc.file.SaveAs(path);
						dc.ScanFile = "Upload/DuplicateCertificate/" + fileName;
						dc.DairyNo = DairyNo;
						dc.id = 0;
						res = objDB.DuplicateCertificate(dc, 8, out OutError);
					}

				}
				else
				{
					ViewData["result"] = "0";
				}
			}
			return View(dc);
		}


		public ActionResult ViewDuplicateCertificate(DuplicateCertificate dc, int? page)
		{
			try
			{
				Session["AddressSlip"] = null;
				Session["DuplicateCertificateForm"] = null;
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				int AdminId = ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				string AdminUser = ViewBag.AdminUser = Session["UserName"].ToString().ToUpper();
				string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();

				#region Action Assign Method
				if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{ ViewBag.IsModiFy = 1; ViewBag.IsCert = 1; ViewBag.IsDisp = 1; ViewBag.IsAck = 1; ViewBag.IsOrder = 1; ViewBag.IsUpdate = 1; }
				else
				{

					string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
					string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
					//string AdminType = Session["AdminType"].ToString();
					//GetActionOfSubMenu(string cont, string act)
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						ViewBag.IsModiFy = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Equals("MODIFY")).Count();
						ViewBag.IsCert = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Equals("CERTIFICATE")).Count();
						ViewBag.IsDisp = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Equals("DISPATCH")).Count();
						ViewBag.IsAck = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("Action").ToUpper().Equals("DUPLICATECERTIFICATEACKNOWLEDGEMENT".ToUpper())).Count();
						ViewBag.IsOrder = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("Action").ToUpper().Equals("DUPLICATECERTIFICATEORDERSLIP".ToUpper())).Count();
						ViewBag.IsUpdate = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Equals("UPDATE")).Count();
					}
				}

				#endregion Action Assign Method

				//GetOrderBy
				ViewBag.MyOrder = objCommon.GetOrderBy();

				//GetAll District
				ViewBag.MyDist = objCommon.GetDistE();

				var itemType = new SelectList(new[] { new { ID = "1", Name = "Same As Per Record" }, new { ID = "2", Name = "Record Based Correction" },
					new { ID = "3", Name = "DOB Certificate Based Correction" }, new { ID = "4", Name = "FN/MN Correction (Due to Adoption)" },
					new { ID = "5", Name = "Change of Candidate Name with Alias/NEE" },  new { ID = "6", Name = "Duplicate Certificate After Correction" }}, "ID", "Name", 1);
				ViewBag.MyType = itemType.ToList();


				if (AdminType.ToUpper() == "COMPUTER-CELL" && Session["ClassAssign"].ToString().ToLower() == "1")
				{
					var itemBranch = new SelectList(new[] { new { ID = "1", Name = "Duplicate Branch" }, }, "ID", "Name", 1);
					ViewBag.MyBranch = itemBranch.ToList();
				}
				else if (AdminType.ToUpper() == "COMPUTER-CELL" && Session["ClassAssign"].ToString().ToLower() == "2")
				{
					var itemBranch = new SelectList(new[] { new { ID = "2", Name = "Certificate Branch" }, }, "ID", "Name", 1);
					ViewBag.MyBranch = itemBranch.ToList();
				}
				else
				{
					var itemBranch = new SelectList(new[] { new { ID = "1", Name = "Duplicate Branch" }, new { ID = "2", Name = "Certificate Branch" }, }, "ID", "Name", 1);
					ViewBag.MyBranch = itemBranch.ToList();
				}

				var itemStatus = new SelectList(new[] { new { ID = "1", Name = "Pending" }, new { ID = "2", Name = "In Process" },
					new { ID = "10", Name = "Printing" }, new { ID = "11", Name = "Dealing Set" },
					new { ID = "3", Name = "Certified Printed" },new { ID = "4", Name = "Objection" },
					  new { ID = "5", Name = "Dispatched" },}, "ID", "Name", 1);
				ViewBag.MyStatus = itemStatus.ToList();


				var itemsch = new SelectList(new[] { new { ID = "1", Name = "Dairy Number" }, new { ID = "2", Name = "Roll Number" },
					new { ID = "3", Name = "Dairy Date" },new { ID = "4", Name = "Name" },
					  new { ID = "5", Name = "Mobile" },new { ID = "6", Name = "Receipt Number " },}, "ID", "Name", 1);
				ViewBag.MySch = itemsch.ToList();

				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;
				if (TempData["SearchDuplicateCertificate"] != null)
				{
					Search += TempData["SearchDuplicateCertificate"].ToString();
					TempData["SelectedDairy"] = ViewBag.SelectedDairy = TempData["SelectedDairy"];
					TempData["SelectedDairyDate"] = ViewBag.SelectedDairyDate = TempData["SelectedDairyDate"];
					TempData["SelectedRoll"] = ViewBag.SelectedRoll = TempData["SelectedRoll"];
					TempData["SelectedBranch"] = ViewBag.SelectedBranch = TempData["SelectedBranch"];
					TempData["SelectedDist"] = ViewBag.SelectedDist = TempData["SelectedDist"];
					TempData["SelectedType"] = ViewBag.SelectedType = TempData["SelectedType"];
					dc.StoreAllData = objDB.ViewDuplicateCertificate(0, 1, Search, pageIndex, 20);
					TempData["SearchDuplicateCertificate"] = Search;
					if (dc.StoreAllData == null || dc.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.LastPageIndex = 0;
						ViewBag.TotalCount = 0;
						ViewBag.TotalCount1 = 0;
						return View();
					}
					else
					{

						ViewBag.TotalCount = dc.StoreAllData.Tables[0].Rows.Count;
						int count = Convert.ToInt32(dc.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
						ViewBag.TotalCount1 = count;
						int tp = Convert.ToInt32(count);
						int pn = tp / 20;
						int cal = 20 * pn;
						int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
						if (res >= 1)
							ViewBag.pn = pn + 1;
						else
							ViewBag.pn = pn;
						return View(dc);
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
		public ActionResult ViewDuplicateCertificate(DuplicateCertificate dc, FormCollection frm, int? page, string submit)
		{
			try
			{
				string ForwardList = string.Empty;
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}

				if (ViewBag.DistAllow == null)
				{ return RedirectToAction("Index", "Admin"); }
				else
				{
					ViewBag.UserDist = ViewBag.DistAllow;
				}
				int AdminId = ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				string AdminUser = ViewBag.AdminUser = Session["UserName"].ToString().ToUpper();
				string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();


				#region Action Assign Method
				if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{ ViewBag.IsModiFy = 1; ViewBag.IsCert = 1; ViewBag.IsDisp = 1; ViewBag.IsAck = 1; ViewBag.IsOrder = 1; ViewBag.IsUpdate = 1; }
				else
				{

					string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
					string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
					//string AdminType = Session["AdminType"].ToString();
					//GetActionOfSubMenu(string cont, string act)
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						ViewBag.IsUpdate = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Equals("UPDATE")).Count();
						ViewBag.IsModiFy = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Equals("MODIFY")).Count();
						ViewBag.IsCert = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Equals("CERTIFICATE")).Count();
						ViewBag.IsDisp = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Equals("DISPATCH")).Count();
						ViewBag.IsAck = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("Action").ToUpper().Equals("DUPLICATECERTIFICATEACKNOWLEDGEMENT".ToUpper())).Count();
						ViewBag.IsOrder = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("Action").ToUpper().Equals("DUPLICATECERTIFICATEORDERSLIP".ToUpper())).Count();
					}
				}
				#endregion Action Assign Method

				//GetOrderBy
				ViewBag.MyOrder = objCommon.GetOrderBy();

				//GetAll District
				ViewBag.MyDist = objCommon.GetDistE();

				var itemType = new SelectList(new[] { new { ID = "1", Name = "Same As Per Record" }, new { ID = "2", Name = "Record Based Correction" },
					new { ID = "3", Name = "DOB Certificate Based Correction" }, new { ID = "4", Name = "FN/MN Correction (Due to Adoption)" },
				   new { ID = "5", Name = "Change of Candidate Name with Alias/NEE" },  new { ID = "6", Name = "Duplicate Certificate After Correction" }}, "ID", "Name", 1);
				ViewBag.MyType = itemType.ToList();



				if (AdminType.ToUpper() == "COMPUTER-CELL" && Session["ClassAssign"].ToString().ToLower() == "1")
				{
					var itemBranch = new SelectList(new[] { new { ID = "1", Name = "Duplicate Branch" }, }, "ID", "Name", 1);
					ViewBag.MyBranch = itemBranch.ToList();
				}
				else if (AdminType.ToUpper() == "COMPUTER-CELL" && Session["ClassAssign"].ToString().ToLower() == "2")
				{
					var itemBranch = new SelectList(new[] { new { ID = "2", Name = "Certificate Branch" }, }, "ID", "Name", 1);
					ViewBag.MyBranch = itemBranch.ToList();
				}
				else
				{
					var itemBranch = new SelectList(new[] { new { ID = "1", Name = "Duplicate Branch" }, new { ID = "2", Name = "Certificate Branch" }, }, "ID", "Name", 1);
					ViewBag.MyBranch = itemBranch.ToList();
				}





				var itemStatus = new SelectList(new[] { new { ID = "1", Name = "Pending" }, new { ID = "2", Name = "In Process" },
					  new { ID = "10", Name = "Printing" }, new { ID = "11", Name = "Dealing Set" },
					new { ID = "3", Name = "Certified Printed" },new { ID = "4", Name = "Objection" },
					  new { ID = "5", Name = "Dispatched" },}, "ID", "Name", 1);
				ViewBag.MyStatus = itemStatus.ToList();

				var itemsch = new SelectList(new[] { new { ID = "1", Name = "Dairy Number" }, new { ID = "2", Name = "Roll Number" },
					new { ID = "3", Name = "Dairy Date" },new { ID = "4", Name = "Name" },
					  new { ID = "5", Name = "Mobile" },new { ID = "6", Name = "Receipt Number " },}, "ID", "Name", 1);
				ViewBag.MySch = itemsch.ToList();

				if (submit != null)
				{
					if (submit.ToUpper() == "RESET")
					{
						TempData.Clear();
						TempData["SearchDuplicateCertificate"] = null;
						return RedirectToAction("ViewDuplicateCertificate", "Admin");
					}
				}




				// SchoolModels asm = new SchoolModels();

				string Search = string.Empty;
				if (AdminType.ToUpper() == "ADMIN")
				{
					Search = "a.id like '%'";
				}
				else if (AdminType.ToUpper() == "DM")
				{
					Search = "a.id like '%' and CreatedBy=" + AdminId + "  ";
				}
				else
				{
					Search = "a.id like '%' and IsForward!=0 ";
				}



				if (AdminType.ToUpper() == "COMPUTER-CELL" && Session["ClassAssign"].ToString().ToLower() == "1")
				{
					Search += " and branch=1  ";
				}
				else if (AdminType.ToUpper() == "COMPUTER-CELL" && Session["ClassAssign"].ToString().ToLower() == "2")
				{
					Search += " and branch=2  ";
				}

				if (frm["Type1"] != "")
				{
					ViewBag.SelectedType = frm["Type1"];
					TempData["SelectedType"] = frm["Type1"];
					Search += " and isType=" + frm["Type1"].ToString();
				}

				if (frm["Branch"] != "")
				{
					ViewBag.SelectedBranch = frm["Branch"];
					TempData["SelectedBranch"] = frm["Branch"];
					Search += " and Branch=" + frm["Branch"].ToString();
				}

				if (frm["Dist1"] != "")
				{
					ViewBag.SelectedDist = frm["Dist1"];
					TempData["SelectedDist"] = frm["Dist1"];
					Search += " and a.dist=" + frm["Dist1"].ToString();
				}

				if (frm["SelStatus"] != "")
				{
					ViewBag.SelectedStatus = frm["SelStatus"];
					int Status1 = Convert.ToInt32(frm["SelStatus"].ToString());
					if (Status1 == 10)
					{
						Search += " and IsStatus=10";
					}
					else if (Status1 == 11)
					{
						Search += " and IsStatus=11 ";
					}
					else
					{
						Status1 = Status1 - 1;
						Search += " and IsStatus=" + Status1;
					}

				}


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
						{ Search += " and Roll='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 3)
						{ Search += " and format(DairyDate,'dd/MM/yyyy') ='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 4)
						{ Search += " and a.Name='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 5)
						{ Search += " and a.Mobile='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 6)
						{ Search += " and a.ReceiptNo='" + frm["SearchString"].ToString() + "'"; }
					}
				}

				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				TempData.Keep();
				TempData["SearchDuplicateCertificate"] = Search;
				dc.StoreAllData = objDB.ViewDuplicateCertificate(0, 1, Search, pageIndex, 20);
				if (dc.StoreAllData == null || dc.StoreAllData.Tables[0].Rows.Count == 0)
				{

					ViewBag.Message = "Record Not Found";
					ViewBag.LastPageIndex = 0;
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = dc.StoreAllData.Tables[0].Rows.Count;
					int count = Convert.ToInt32(dc.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
					ViewBag.TotalCount1 = count;
					int tp = Convert.ToInt32(count);
					int pn = tp / 20;
					int cal = 20 * pn;
					int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
					if (res >= 1)
						ViewBag.pn = pn + 1;
					else
						ViewBag.pn = pn;

					return View(dc);
				}

			}
			catch (Exception ex)
			{
				// ////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return View();
			}
		}

		public JsonResult JqDCForwardList(string storeid, string Action)
		{

			if (Action == "4")
			{
				Session["AddressSlip"] = storeid;
				return Json(new { redirectUrl = Url.Action("AddressSlip", "Admin"), isRedirect = true });
			}
			if (Action == "5")
			{
				Session["DuplicateCertificateForm"] = storeid;
				return Json(new { redirectUrl = Url.Action("DuplicateCertificateForm", "Admin"), isRedirect = true });
			}
			int userid = Convert.ToInt32(Session["AdminId"].ToString());
			// storeid = storeid.Remove(storeid.Length - 1);
			//string[] split1 = storeid.Split('^');
			string ForwardList = storeid;
			int cnt = ForwardList.Count(x => x == ',');
			string OutError = "0";
			string result = objDB.DuplicateCertificateForward(ForwardList, Action, Convert.ToInt32(Session["AdminId"]), out OutError);
			if (OutError == "1")
			{
				ViewData["ForwardResult"] = "1";
			}
			else
			{ ViewData["ForwardResult"] = "0"; }

			return Json(new { dee = OutError }, JsonRequestBehavior.AllowGet);
		}



		[HttpPost]
		public ActionResult ObjectionLetter(HttpPostedFileBase ObjectionLetter, FormCollection frm)
		{
			if (Session["AdminId"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}
			string OutError = "0";
			if (Convert.ToInt32(frm["HId"].ToString()) > 0 && frm["RemarksObjection"] != null)
			{
				if (ObjectionLetter != null && ObjectionLetter.ContentLength > 0 && !string.IsNullOrEmpty(ObjectionLetter.FileName))
				{
					string fileName = Path.GetFileNameWithoutExtension(ObjectionLetter.FileName);
					string fileExt = Path.GetExtension(ObjectionLetter.FileName);
					fileName = fileName + "_" + DateTime.Now.Date.ToString("dd_MMMM_yyyy") + fileExt;
					string filepath = "Upload/DuplicateCertificate/ObjectionLetter/" + fileName;
					//CSV.SaveAs(Server.MapPath(filepath));
					string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();
					string status = "";
					DuplicateCertificate dc = new DuplicateCertificate();
					dc.id = Convert.ToInt32(frm["HId"].ToString());
					dc.ObjectionLetter = filepath;
					dc.Adminid = Convert.ToInt32(Session["AdminId"]);
					dc.Remarks = frm["RemarksObjection"].ToString();
					dc.DairyNo = frm["HDairyNo"].ToString();
					dc.Mobile = frm["HMobile"].ToString();

					status = objDB.DuplicateCertificate(dc, 5, out OutError);
					if (OutError == "1")
					{
						if (Convert.ToInt32(frm["HId"].ToString()) > 0)
						{
							//var path = Path.Combine(Server.MapPath("~/Upload/"+ formName + "/" + dist + "/Photo"), stdPic);
							var path = Path.Combine(Server.MapPath("~/Upload/" + "DuplicateCertificate/ObjectionLetter"), fileName);
							string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + "/DuplicateCertificate/ObjectionLetter"));
							if (!Directory.Exists(FilepathExist))
							{
								Directory.CreateDirectory(FilepathExist);
							}
							ObjectionLetter.SaveAs(path);
							//Objection raised against your application (dairy no 101). Visit duplicate section under board's website to track status/Download objection letter
							string Sms = "Objection raised against your application (dairy no " + dc.DairyNo + ").successfully. Visit duplicate section under board's website to track status/Download objection letter";
							try
							{
								string getSms = objCommon.gosms(dc.Mobile, Sms);
								// string getSms = objCommon.gosms("9711819184", Sms);
							}
							catch (Exception) { }

						}


						return RedirectToAction("ViewDuplicateCertificate", "Admin");
					}

				}
			}
			return RedirectToAction("ViewDuplicateCertificate", "Admin");
		}

		[HttpPost]
		public ActionResult JqModifyDuplicateCertificate(string Data, string Id, string Type, string Byhand)
		{
			if (Session["AdminId"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}
			string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();
			string status = "";
			if (Data == null || Data == "" || Type == "")
			{
				var results = new
				{
					status = ""
				};
				return Json(results);
			}
			else
			{
				//var Jsonobject = JsonConvert.DeserializeObject<DuplicateCertificate>(Data);
				DuplicateCertificate oModel = JsonConvert.DeserializeObject<DuplicateCertificate>(Data);
				oModel.id = Convert.ToInt32(Id);
				oModel.Adminid = Convert.ToInt32(Session["AdminId"]);
				string OutError = "0";
				if (Type == "1")
				{
					status = objDB.DuplicateCertificate(oModel, 1, out OutError);
				}
				else { status = objDB.DuplicateCertificate(oModel, Convert.ToInt32(Type), out OutError); } //DuplicateCertificateSP


				if (Type == "3" && OutError == "1" && Byhand == "2")
				{
					string disn = "RP" + oModel.DispNo.ToString() + "IN";
					//Your certificate for Dr.No 0100001 has been dispatched on 10/09/17 vide registry no RP21412412IN.track status:(google tiny link).
					string Sms = "Your certificate for Dr.No " + oModel.DairyNo + " has been dispatched on " + Convert.ToDateTime(oModel.DispDate).ToString("dd/MM/yyyy") + " vide registry no " + disn + ".track status https://tinyurl.com/y7ruuksy";
					try
					{
						string getSms = objCommon.gosms(oModel.Mobile, Sms);
						// string getSms = objCommon.gosms("9711819184", Sms);
					}
					catch (Exception) { }
				}


				var results = new
				{
					status = OutError,
				};
				return Json(results);
			}
		}

		public ActionResult DuplicateCertificateReport(DuplicateCertificate dc, int? page)
		{
			TempData["SearchDuplicateCertificate"] = null;
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				int AdminId = ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();


				int Dcid = 0;
				if (AdminType.ToUpper() == "DM")
				{
					Dcid = AdminId;
				}
				ViewBag.UserDistNM = Session["UserName"].ToString().ToUpper();
				ViewBag.MyDist = objCommon.GetDistE();

				var itemBranch = new SelectList(new[] { new { ID = "1", Name = "Duplicate Branch" }, new { ID = "2", Name = "Certificate Branch" }, }, "ID", "Name", 1);
				ViewBag.MyBranch = itemBranch.ToList();


				var itemStatus = new SelectList(new[] { new { ID = "1", Name = "Pending" }, new { ID = "2", Name = "In Process" },
					  new { ID = "10", Name = "Printing" }, new { ID = "11", Name = "Dealing Set" },
					new { ID = "3", Name = "Certified Printed" },new { ID = "4", Name = "Objection" },
					  new { ID = "5", Name = "Dispatched" },}, "ID", "Name", 1);
				ViewBag.MySch = itemStatus.ToList();

				ViewBag.UserDistNM = "";
				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;
				if (TempData["ReportDuplicateCertificate"] != null)
				{
					Search += TempData["ReportDuplicateCertificate"].ToString();
					TempData["SelectedItem"] = ViewBag.SelectedItem = TempData["SelectedItem"];
					dc.StoreAllData = objDB.ViewDuplicateCertificate(Dcid, 3, Search, pageIndex, 20);
					TempData["ReportDuplicateCertificate"] = Search;
					if (dc.StoreAllData == null || dc.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.LastPageIndex = 0;
						ViewBag.TotalCount = 0;
						ViewBag.TotalCount1 = 0;
						return View();
					}
					else
					{

						ViewBag.TotalCount = dc.StoreAllData.Tables[0].Rows.Count;
						// ViewBag.UserDistNM = dc.StoreAllData.Tables[0].Rows[0]["UserDistNM"].ToString();                     
						int count = Convert.ToInt32(dc.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
						ViewBag.TotalCount1 = count;
						int tp = Convert.ToInt32(count);
						int pn = tp / 20;
						int cal = 20 * pn;
						int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
						if (res >= 1)
							ViewBag.pn = pn + 1;
						else
							ViewBag.pn = pn;
						return View(dc);
					}
				}
				else
				{
					if (AdminType.ToUpper() == "DM")
					{
						Search = "a.id like '%' and CreatedBy=" + AdminId + "  ";
					}
					else
					{
						Search = "a.id like '%' ";
					}
					ViewBag.SelectedName = "All";
					TempData.Keep();
					TempData["ReportDuplicateCertificate"] = Search;
					dc.StoreAllData = objDB.ViewDuplicateCertificate(Dcid, 3, Search, pageIndex, 20);
					if (dc.StoreAllData == null || dc.StoreAllData.Tables[0].Rows.Count == 0)
					{

						ViewBag.Message = "Record Not Found";
						ViewBag.LastPageIndex = 0;
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						ViewBag.TotalCount = 0;
						// ViewBag.UserDistNM = dc.StoreAllData.Tables[0].Rows[0]["UserDistNM"].ToString();
						return View(dc);
					}
				}

			}
			catch (Exception ex)
			{
				return View();
			}
		}

		[HttpPost]
		public ActionResult DuplicateCertificateReport(DuplicateCertificate dc, FormCollection frm, int? page, string submit)
		{
			try
			{
				string ForwardList = string.Empty;
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}

				ViewBag.MyDist = objCommon.GetDistE();

				var itemBranch = new SelectList(new[] { new { ID = "1", Name = "Duplicate Branch" }, new { ID = "2", Name = "Certificate Branch" }, }, "ID", "Name", 1);
				ViewBag.MyBranch = itemBranch.ToList();

				var itemStatus = new SelectList(new[] { new { ID = "1", Name = "Pending" }, new { ID = "2", Name = "In Process" },
					  new { ID = "10", Name = "Printing" }, new { ID = "11", Name = "Dealing Set" },
					new { ID = "3", Name = "Certified Printed" },new { ID = "4", Name = "Objection" },
					  new { ID = "5", Name = "Dispatched" },}, "ID", "Name", 1);
				ViewBag.MySch = itemStatus.ToList();

				if (submit != null)
				{
					if (submit.ToUpper() == "RESET")
					{
						TempData.Clear();
						TempData["ReportDuplicateCertificate"] = null;
						return RedirectToAction("DuplicateCertificateReport", "Admin");
					}
				}

				int AdminId = ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();

				ViewBag.UserDistNM = Session["UserName"].ToString().ToUpper();

				// SchoolModels asm = new SchoolModels();
				int Dcid = 0;
				if (AdminType.ToUpper() == "DM")
				{
					Dcid = AdminId;
				}

				string Search = string.Empty;
				if (AdminType.ToUpper() == "DM")
				{
					Search = "a.id like '%' and CreatedBy=" + AdminId + "  ";
				}
				else
				{
					Search = "a.id like '%'  ";
				}

				if (frm["Branch"] != "")
				{
					ViewBag.SelectedBranch = frm["Branch"];
					TempData["SelectedBranch"] = frm["Branch"];
					Search += " and Branch=" + frm["Branch"].ToString();

					ViewBag.SelectedBranchName = ViewBag.MyBranch[Convert.ToInt32(ViewBag.SelectedBranch) - 1].Text;

				}

				if (frm["Dist1"] != "")
				{
					ViewBag.SelectedDist = frm["Dist1"];
					TempData["SelectedDist"] = frm["Dist1"];
					Search += " and a.dist=" + frm["Dist1"].ToString();
				}

				if (frm["FromDate"] != "")
				{
					//Search += " and format(UpdatedDate, 'dd/MM/yyyy') >= '" + frm["FromDate"].ToString() + "'";
					Search += " and UpdatedDateDD >= '" + frm["FromDate"].ToString() + "'";
					ViewBag.FromDate = frm["FromDate"];
				}
				if (frm["ToDate"] != "")
				{
					//  Search += " and format(UpdatedDate, 'dd/MM/yyyy') <= '" + frm["ToDate"].ToString() + "'";
					Search += " and UpdatedDateDD <= '" + frm["ToDate"].ToString() + "'";
					ViewBag.ToDate = frm["ToDate"];
				}
				ViewBag.SelectedName = "All";
				if (frm["SelList"] != "")
				{
					ViewBag.SelectedItem = frm["SelList"];
					int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());
					ViewBag.SelectedName = itemStatus.ToList().Where(s => s.Value == SelValueSch.ToString()).First().Text;
					if (SelValueSch == 10)
					{
						Search += " and IsStatus=10";
					}
					else if (SelValueSch == 11)
					{
						Search += " and IsStatus=11 ";
					}
					else
					{


						SelValueSch = SelValueSch - 1;
						Search += " and IsStatus=" + SelValueSch;
					}
				}
				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				TempData.Keep();
				TempData["ReportDuplicateCertificate"] = Search;
				dc.StoreAllData = objDB.ViewDuplicateCertificate(Dcid, 3, Search, pageIndex, 20);
				if (dc.StoreAllData == null || dc.StoreAllData.Tables[0].Rows.Count == 0)
				{

					ViewBag.Message = "Record Not Found";
					ViewBag.LastPageIndex = 0;
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = dc.StoreAllData.Tables[0].Rows.Count;
					// ViewBag.UserDistNM = dc.StoreAllData.Tables[0].Rows[0]["UserDistNM"].ToString();
					int count = Convert.ToInt32(dc.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
					ViewBag.TotalCount1 = count;
					int tp = Convert.ToInt32(count);
					int pn = tp / 20;
					int cal = 20 * pn;
					int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
					if (res >= 1)
						ViewBag.pn = pn + 1;
					else
						ViewBag.pn = pn;

					return View(dc);
				}

			}
			catch (Exception ex)
			{
				// ////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return View();
			}
		}


		public ActionResult DuplicateCertificateSummary(DuplicateCertificate rp, string id)
		{
			try
			{
				TempData["ReportDuplicateCertificate"] = null;
				TempData["SearchDuplicateCertificate"] = null;
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}


				int AdminId = ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();

				#region Action Assign Method
				if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{ ViewBag.Summary1 = 1; ViewBag.Summary2 = 1; ViewBag.Summary3 = 1; }
				else
				{
					string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
					string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
					//string AdminType = Session["AdminType"].ToString();
					//GetActionOfSubMenu(string cont, string act)
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						ViewBag.Summary1 = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("Action").ToUpper().Equals("DuplicateCertificateSummary/1".ToUpper())).Count();
						ViewBag.Summary2 = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("Action").ToUpper().Equals("DuplicateCertificateSummary/2".ToUpper())).Count();
						ViewBag.Summary3 = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("Action").ToUpper().Equals("DuplicateCertificateSummary/3".ToUpper())).Count();
					}
				}

				#endregion Action Assign Method

				if (id == null)
				{
					return View(rp);
				}
				ViewBag.tab = id;
				int tid = Convert.ToInt32(id);
				string Search = string.Empty;
				DataSet ds = objDB.DuplicateCertificateSummary(Search, tid);  //1 for Total Registration by School Report
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
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}

		public ActionResult DuplicateCertificateForm()
		{
			DuplicateCertificate dc = new Models.DuplicateCertificate();

			if (Session["DuplicateCertificateForm"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			else
			{
				string id = Session["DuplicateCertificateForm"].ToString();
				ViewBag.Id = id;
				string Search = string.Empty;
				Search = "a.id like '%' and  a.id in ( " + id + " )";

				DataSet ds = objDB.ViewDuplicateCertificate(0, 4, Search, 1, 100);
				if (ds == null || ds.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.LastPageIndex = 0;
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = ds.Tables[0].Rows.Count;
					dc.StoreAllData = ds;
					return View(dc);
				}
			}
		}

		public ActionResult DuplicateCertificateAcknowledgement(string id, DuplicateCertificate dc)
		{
			if (Session["AdminId"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}
			else if (id == null)
			{ return RedirectToAction("ViewDuplicateCertificate", "Admin"); }
			else
			{
				try
				{
					//  id = encrypt.QueryStringModule.Decrypt(id);
					ViewBag.Id = id;
					string Search = string.Empty;
					// Search = "a.id like '%' and a.id=" + id + "  ";                  
					Search = "a.id like '%' and a.Dairyno='" + id + "'";
					dc.StoreAllData = objDB.ViewDuplicateCertificate(0, 4, Search, 1, 20);
					if (dc.StoreAllData == null || dc.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.LastPageIndex = 0;
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						ViewBag.TotalCount = dc.StoreAllData.Tables[0].Rows.Count;
						return View(dc);
					}
				}
				catch (Exception)
				{
					return RedirectToAction("ViewDuplicateCertificate", "Admin");
				}
			}
		}

		public ActionResult AddressSlip()
		{
			DuplicateCertificate dc = new Models.DuplicateCertificate();

			if (Session["AddressSlip"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			else
			{
				string id = Session["AddressSlip"].ToString();
				string Search = string.Empty;
				Search = "a.id like '%' and  a.id in ( " + id + " )";

				dc.StoreAllData = objDB.ViewDuplicateCertificate(0, 4, Search, 1, 100);
				if (dc.StoreAllData == null || dc.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.LastPageIndex = 0;
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = dc.StoreAllData.Tables[0].Rows.Count;
					return View(dc);
				}
			}

		}

		public ActionResult DuplicateCertificateOrderSlip(string id, DuplicateCertificate dc)
		{
			if (Session["AdminId"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}
			else if (id == null)
			{ return RedirectToAction("ViewDuplicateCertificate", "Admin"); }
			else
			{
				try
				{

					//GetOrderBy
					//  ViewBag.MyOrder = objCommon.GetOrderBy();

					ViewBag.Id = id;
					string Search = string.Empty;
					Search = "a.id like '%' and a.Dairyno='" + id + "'";
					dc.StoreAllData = objDB.ViewDuplicateCertificate(0, 4, Search, 1, 20);
					if (dc.StoreAllData == null || dc.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.LastPageIndex = 0;
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						ViewBag.TotalCount = dc.StoreAllData.Tables[0].Rows.Count;
						return View(dc);
					}
				}
				catch (Exception)
				{
					return RedirectToAction("ViewDuplicateCertificate", "Admin");
				}
			}
		}


		public ActionResult DuplicateCertificateStatus(DuplicateCertificate dc, int? page)
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				int AdminId = ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();

				int Dcid = 0;
				if (AdminType.ToUpper() == "DM")
				{
					Dcid = AdminId;
				}
				ViewBag.UserDistNM = Session["UserName"].ToString().ToUpper();

				ViewBag.UserDistNM = "";
				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;
				if (AdminType.ToUpper() == "DM")
				{
					Search = "a.id like '%' and CreatedBy=" + AdminId + "  ";
				}
				else
				{
					Search = "a.id like '%' ";
				}


				dc.StoreAllData = objDB.ViewDuplicateCertificate(Dcid, 5, Search, pageIndex, 20);
				if (dc.StoreAllData == null || dc.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = dc.StoreAllData.Tables[0].Rows.Count;
					return View(dc);
				}
			}
			catch (Exception ex)
			{
				return View();
			}
		}


		public ActionResult DuplicateCertificateRTSReport(DuplicateCertificate dc)
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				int AdminId = ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();
				ViewBag.UserDistNM = Session["UserName"].ToString().ToUpper();
				ViewBag.TotalCount = 0;
				return View(dc);
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		[HttpPost]
		public ActionResult DuplicateCertificateRTSReport(DuplicateCertificate dc, FormCollection frm, string submit)
		{
			try
			{
				string ForwardList = string.Empty;
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				if (submit != null)
				{
					if (submit.ToUpper() == "RESET")
					{
						TempData.Clear();
						TempData["RPTReportDuplicateCertificate"] = null;
						return RedirectToAction("DuplicateCertificateRTSReport", "Admin");
					}
				}

				int AdminId = ViewBag.AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();

				ViewBag.UserDistNM = Session["UserName"].ToString().ToUpper();

				// SchoolModels asm = new SchoolModels();
				int Dcid = 0;
				if (AdminType.ToUpper() == "DM")
				{
					Dcid = AdminId;
				}

				string Search = string.Empty;
				if (AdminType.ToUpper() == "DM")
				{
					Search = "a.id like '%' and CreatedBy=" + AdminId + "  ";
				}
				else
				{
					Search = "a.id like '%'  ";
				}

				if (frm["FromDate"] == "" || frm["ToDate"] == "")
				{
					ViewBag.TotalCount = 0;
					TempData["result"] = "10";
				}

				if (frm["FromDate"] != "")
				{
					//Search += " and UpdatedDateDD >= '" + frm["FromDate"].ToString() + "'";
					ViewBag.FromDate = frm["FromDate"].ToString();
				}
				if (frm["ToDate"] != "")
				{
					// Search += " and UpdatedDateDD <= '" + frm["ToDate"].ToString() + "'";
					ViewBag.ToDate = frm["ToDate"].ToString();
				}
				TempData["RPTReportDuplicateCertificate"] = Search;
				ViewBag.Id = "";
				string FileName = "DC_" + frm["FromDate"].ToString().Replace("/", "") + "_" + frm["ToDate"].ToString().Replace("/", "");
				if (submit.ToUpper().Contains("RTS"))
				{
					ViewBag.Id = "RTS";
					FileName += "_RTS";
					dc.StoreAllData = objDB.RPTDuplicateCertificate(1, "'" + frm["FromDate"].ToString() + "'", "'" + frm["ToDate"].ToString() + "'");//RTS
				}
				else if (submit.ToUpper().Contains("DATA"))
				{
					ViewBag.Id = "DATA";
					FileName += "_DATA";
					dc.StoreAllData = objDB.RPTDuplicateCertificate(2, "'" + frm["FromDate"].ToString() + "'", "'" + frm["ToDate"].ToString() + "'");//DATA
				}

				if (dc.StoreAllData == null || dc.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = dc.StoreAllData.Tables[0].Rows.Count;
					ExportDataFromDataTableAny(dc.StoreAllData.Tables[0], FileName);
					return View(dc);
				}

			}
			catch (Exception ex)
			{
				// ////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return View();
			}
		}

		public ActionResult ExportDataFromDataTableAny(DataTable dt, string filename)
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
						string fileName1 = filename + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";  //103_230820162209_347
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

				return RedirectToAction("Index", "Admin");
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Admin");
			}

		}


		#endregion DuplicateCertificate

		#region  Change_Password

		public ActionResult Change_Password()
		{
			string User = null;
			if (Session["AdminId"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}
			User = Session["UserName"].ToString();
			ViewBag.User = Session["UserName"].ToString();
			return View();
		}

		[HttpPost]
		public ActionResult Change_Password(FormCollection frm)
		{
			string User = null;
			if (Session["AdminId"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}
			User = Session["UserName"].ToString();
			ViewBag.User = Session["UserName"].ToString();

			string CurrentPassword = string.Empty;
			string NewPassword = string.Empty;

			if (frm["ConfirmPassword"] != "" && frm["NewPassword"] != "")
			{
				if (frm["ConfirmPassword"].ToString() == frm["NewPassword"].ToString())
				{
					CurrentPassword = frm["CurrentPassword"].ToString();
					NewPassword = frm["NewPassword"].ToString();
					int result = new AbstractLayer.DMDB().ChangePassword(Convert.ToInt32(Session["AdminId"].ToString()), CurrentPassword, NewPassword);
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
		#endregion  Change_Password

		#region Admin Inbox Master
		public MultiSelectList MenuAdminList(string sel, int type)
		{
			var AdminList = objCommon.GetAdminUser().ToList().Select(c => new
			{
				Text = c.Text,
				Value = c.Value
			}).OrderBy(s => s.Text).ToList();

			if (type == 0)
			{
				if (sel == "")
				{ return new MultiSelectList(AdminList, "Value", "Text"); }
				else
				{
					int[] myArray1 = AbstractLayer.StaticDB.StringToIntArray(sel, ',');
					return new MultiSelectList(AdminList, "Value", "Text", myArray1);
				}
			}
			else
			{
				if (sel == "")
				{ return new MultiSelectList(AdminList, "Value", "Text"); }
				else
				{

					int[] myArray1 = AbstractLayer.StaticDB.StringToIntArray(sel, ',');
					List<int> keepList = sel.Split(',').Select(int.Parse).ToList();
					// what you want
					AdminList = AdminList.Where(i => keepList.Contains(Convert.ToInt32(i.Value))).ToList();
					return new MultiSelectList(AdminList, "Value", "Text");
				}
			}

		}

		public ActionResult InboxMaster(InboxModel im)
		{
			if (Session["AdminType"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			else
			{
				string AdminId = Session["AdminId"].ToString();
				ViewBag.MyAdminList = MenuAdminList("", 0);
				return View(im);
			}

		}

		[HttpPost]
		public ActionResult InboxMaster(InboxModel im, FormCollection frm, IEnumerable<HttpPostedFileBase> files)
		{
			ViewBag.MyAdminList = MenuAdminList("", 0);


			if (Session["AdminType"] == null)
			{ return RedirectToAction("Index", "Admin"); }

			string AdminId = Session["AdminId"].ToString();

			string SelectedAdmin = "";
			if (frm["SelectedAdmin"] == "" || frm["SelectedAdmin"] == null)
			{
				ViewData["Result"] = 20;
				return View(im);
			}
			else
			{
				SelectedAdmin = frm["SelectedAdmin"].ToString();
				im.To = frm["SelectedAdmin"].ToString();
			}
			string SelFiles = "";
			string guidObj = Guid.NewGuid().ToString().Substring(0, 5);
			if (files != null)
			{
				if (files.Count() > 0)
				{
					if (files.Count() <= 10)
					{
						foreach (var file in files)
						{
							if (file != null && file.ContentLength > 0)
							{
								// file.SaveAs(Path.Combine(Server.MapPath("/UploadFiles"), im.Subject.ToUpper() + "_" + Path.GetFileNameWithoutExtension(file.FileName).ToUpper() + Path.GetExtension(file.FileName)));
								string fileName = "MailFiles/" + im.Subject.Replace("/", "_").ToUpper() + "_" + guidObj.ToString() + "_" + Path.GetFileNameWithoutExtension(file.FileName).Replace("/", "_").ToUpper() + Path.GetExtension(file.FileName);
								SelFiles += fileName + ",";
							}
						}
					}
					else
					{
						ViewData["Result"] = 10;
						return View(im);
					}
				}
			}

			if (SelFiles != "")
			{ SelFiles = SelFiles.Remove(SelFiles.LastIndexOf(","), 1); }
			im.Attachments = SelFiles.ToString();
			im.From = Session["AdminId"].ToString();
			string OutError = "0";
			DataTable dt1 = objDB.AddInboxMaster(im, out OutError);
			if (OutError == "1")
			{
				ViewData["Result"] = 1;
				if (files != null)
				{
					foreach (var file in files)
					{
						if (file != null && file.ContentLength > 0)
						{
							//file.SaveAs(Path.Combine(Server.MapPath("/MailFiles"), im.Subject.ToUpper() + "_" + Path.GetFileNameWithoutExtension(file.FileName).ToUpper() + Path.GetExtension(file.FileName)));
							var path = Path.Combine(Server.MapPath("~/Upload/MailFiles"), im.Subject.Replace("/", "_").ToUpper() + "_" + guidObj.ToString() + "_" + Path.GetFileNameWithoutExtension(file.FileName).Replace("/", "_").ToUpper() + Path.GetExtension(file.FileName));
							string FilepathExist = Path.Combine(Server.MapPath("~/Upload/MailFiles"));
							if (!Directory.Exists(FilepathExist))
							{
								Directory.CreateDirectory(FilepathExist);
							}
							file.SaveAs(path);
						}
					}
				}

			}
			else
			{
				ViewData["Result"] = 0;
			}
			return View(im);

		}


		public ActionResult ViewInbox(string id, InboxModel im, int? page)
		{
			ViewBag.TotalCount2 = 0;
			if (Session["AdminType"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			else
			{
				var itemType = new SelectList(new[] { new { ID = "1", Name = "Subject" }, new { ID = "2", Name = "Body" },
					new { ID = "3", Name = "Mail Id" }, new { ID = "4", Name = "Sender" },}, "ID", "Name", 1);
				ViewBag.MyType = itemType.ToList();

				ViewBag.MyAdminList = MenuAdminList("", 0);

				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;

				int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				string Search = string.Empty;


				// view  mail details
				if (id != null)
				{
					id = encrypt.QueryStringModule.Decrypt(id);
					Search = " a.id = '" + id + "' ";
					DataSet result = objDB.ViewInboxMaster(Convert.ToInt32(id), 3, AdminId, Search, pageIndex, 20);
					if (result == null)
					{
						ViewBag.TotalCount2 = 0;
					}
					else if (result.Tables[0].Rows.Count > 0)
					{
						if (Session["AdminType"].ToString().ToLower() == "admin")
						{
							string sentToUsers = result.Tables[0].Rows[0]["SentTo"].ToString();
							ViewBag.MyAdminList = MenuAdminList(sentToUsers, 1);
						}
						im.StoreAllData = result;
						ViewBag.TotalCount2 = im.StoreAllData.Tables[0].Rows.Count;
					}

				}
				else
				{

					Search = " d.SentToId = '" + AdminId + "' ";
					if (TempData["SearchInbox"] != null)
					{
						Search += TempData["SearchInbox"].ToString();
						TempData["SearchString"] = ViewBag.SearchString = TempData["SearchString"];
						TempData["SelectedType"] = ViewBag.SelectedType = TempData["SelectedType"];
						TempData["SearchDuplicateCertificate"] = Search;
						DataSet result = objDB.ViewInboxMaster(0, 1, AdminId, Search, pageIndex, 20);
						if (result == null)
						{
							ViewBag.LastPageIndex = 0;
							ViewBag.TotalCount = 0;
							ViewBag.TotalCount1 = 0;
						}
						else if (result.Tables[0].Rows.Count > 0)
						{

							im.StoreAllData = result;
							ViewBag.TotalCount = im.StoreAllData.Tables[0].Rows.Count;
							int count = Convert.ToInt32(im.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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

						DataSet result = objDB.ViewInboxMaster(0, 1, AdminId, Search, pageIndex, 20);
						if (result == null)
						{
							ViewBag.LastPageIndex = 0;
							ViewBag.TotalCount = 0;
							ViewBag.TotalCount1 = 0;
						}
						else if (result.Tables[0].Rows.Count > 0)
						{

							im.StoreAllData = result;
							ViewBag.TotalCount = im.StoreAllData.Tables[0].Rows.Count;
							int count = Convert.ToInt32(im.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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
				return View(im);
			}
		}

		[HttpPost]
		public ActionResult ViewInbox(string id, InboxModel im, FormCollection frm, int? page, string submit)
		{
			ViewBag.TotalCount2 = 0;
			if (Session["AdminType"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			else
			{


				var itemType = new SelectList(new[] { new { ID = "1", Name = "Subject" }, new { ID = "2", Name = "Body" },
					new { ID = "3", Name = "Mail Id" }, new { ID = "4", Name = "Sender" },}, "ID", "Name", 1);
				ViewBag.MyType = itemType.ToList();
				ViewBag.MyAdminList = MenuAdminList("", 0);

				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				if (submit != null)
				{
					if (submit.ToUpper() == "RESET")
					{
						TempData.Clear();
						TempData["SearchInbox"] = null;
						return RedirectToAction("ViewInbox", "Admin");
					}
				}



				string Search = string.Empty;
				Search = " d.SentToId = '" + AdminId + "' ";
				if (frm["MyType"] != "")
				{
					ViewBag.SelectedType = frm["MyType"];
					TempData["SelectedType"] = frm["MyType"];
					int SelValueSch = Convert.ToInt32(frm["MyType"].ToString());
					if (frm["SearchString"] != "")
					{
						TempData["SearchString"] = frm["SearchString"].ToString();
						ViewBag.SearchString = frm["SearchString"].ToString();
						if (SelValueSch == 1)
						{ Search += " and a.Subject  like '%" + frm["SearchString"].ToString() + "%'"; }
						else if (SelValueSch == 2)
						{ Search += " and a.Body like '%" + frm["SearchString"].ToString() + "%'"; }
						else if (SelValueSch == 3)
						{ Search += " and a.Id='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 4)
						{ Search += " and b.[user] like '%" + frm["SearchString"].ToString() + "%'"; }
					}
				}

				TempData["SearchInbox"] = Search;
				DataSet result = objDB.ViewInboxMaster(0, 1, AdminId, Search, pageIndex, 20);
				if (result == null)
				{
					ViewBag.LastPageIndex = 0;
					ViewBag.TotalCount = 0;
					ViewBag.TotalCount1 = 0;
				}
				else if (result.Tables[0].Rows.Count > 0)
				{
					im.StoreAllData = result;
					ViewBag.TotalCount = im.StoreAllData.Tables[0].Rows.Count;
					int count = Convert.ToInt32(im.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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
				return View(im);
			}
		}

		[HttpPost]
		public ActionResult JqReadInbox(string Id, string Type)
		{
			if (Session["AdminId"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}
			string AdminType = ViewBag.AdminType = Session["AdminType"].ToString().ToUpper();
			string status = "";
			if (Id == null || Id == "")
			{
				var results = new
				{
					status = ""
				};
				return Json(results);
			}
			else
			{
				string OutError = "0";
				status = objDB.ReadInbox(Convert.ToInt32(Id), Convert.ToInt32(Session["AdminId"]), Convert.ToInt32(Type), out OutError);
				var results = new
				{
					status = OutError,
				};
				return Json(results);
			}
		}




		public ActionResult SentMail(string id, InboxModel im, int? page)
		{
			ViewBag.TotalCount2 = 0;
			if (Session["AdminType"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			else
			{
				var itemType = new SelectList(new[] { new { ID = "1", Name = "Subject" }, new { ID = "2", Name = "Body" },
					new { ID = "3", Name = "Mail Id" }, new { ID = "4", Name = "Sender" },}, "ID", "Name", 1);
				ViewBag.MyType = itemType.ToList();

				ViewBag.MyAdminList = MenuAdminList("", 0);
				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;

				int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				string Search = string.Empty;


				if (id != null)
				{
					id = encrypt.QueryStringModule.Decrypt(id);
					Search = " a.id = '" + id + "' ";
					DataSet result = objDB.ViewInboxMaster(Convert.ToInt32(id), 3, AdminId, Search, pageIndex, 20);
					if (result == null)
					{
						ViewBag.TotalCount2 = 0;
					}
					else if (result.Tables[0].Rows.Count > 0)
					{
						if (Session["AdminType"].ToString().ToLower() == "admin")
						{
							string sentToUsers = result.Tables[0].Rows[0]["SentTo"].ToString();
							ViewBag.MyAdminList = MenuAdminList(sentToUsers, 1);
						}
						im.StoreAllData = result;
						ViewBag.TotalCount2 = im.StoreAllData.Tables[0].Rows.Count;
					}

				}
				else
				{
					Search = " SentFrom = '" + AdminId + "' ";
					if (TempData["SearchSentMail"] != null)
					{
						Search += TempData["SearchSentMail"].ToString();
						TempData["SearchString"] = ViewBag.SearchString = TempData["SearchString"];
						TempData["SelectedType"] = ViewBag.SelectedType = TempData["SelectedType"];
						TempData["SearchDuplicateCertificate"] = Search;
						DataSet result = objDB.ViewInboxMaster(0, 2, AdminId, Search, pageIndex, 20);
						if (result == null)
						{
							ViewBag.LastPageIndex = 0;
							ViewBag.TotalCount = 0;
							ViewBag.TotalCount1 = 0;
						}
						else if (result.Tables[0].Rows.Count > 0)
						{

							im.StoreAllData = result;
							ViewBag.TotalCount = im.StoreAllData.Tables[0].Rows.Count;
							int count = Convert.ToInt32(im.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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

						DataSet result = objDB.ViewInboxMaster(0, 2, AdminId, Search, pageIndex, 20);
						if (result == null)
						{
							ViewBag.LastPageIndex = 0;
							ViewBag.TotalCount = 0;
							ViewBag.TotalCount1 = 0;
						}
						else if (result.Tables[0].Rows.Count > 0)
						{

							im.StoreAllData = result;
							ViewBag.TotalCount = im.StoreAllData.Tables[0].Rows.Count;
							int count = Convert.ToInt32(im.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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
				return View(im);
			}
		}

		[HttpPost]
		public ActionResult SentMail(string id, InboxModel im, FormCollection frm, int? page, string submit)
		{
			ViewBag.TotalCount2 = 0;
			if (Session["AdminType"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			else
			{
				var itemType = new SelectList(new[] { new { ID = "1", Name = "Subject" }, new { ID = "2", Name = "Body" },
					new { ID = "3", Name = "Mail Id" }, new { ID = "4", Name = "Sender" },}, "ID", "Name", 1);
				ViewBag.MyType = itemType.ToList();
				ViewBag.MyAdminList = MenuAdminList("", 0);
				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				if (submit != null)
				{
					if (submit.ToUpper() == "RESET")
					{
						TempData.Clear();
						TempData["SearchInbox"] = null;
						return RedirectToAction("ViewInbox", "Admin");
					}
				}



				string Search = string.Empty;
				Search = " SentFrom = '" + AdminId + "' ";

				if (frm["MyType"] != "")
				{
					ViewBag.SelectedType = frm["MyType"];
					TempData["SelectedType"] = frm["MyType"];
					int SelValueSch = Convert.ToInt32(frm["MyType"].ToString());
					if (frm["SearchString"] != "")
					{
						TempData["SearchString"] = frm["SearchString"].ToString();
						ViewBag.SearchString = frm["SearchString"].ToString();
						if (SelValueSch == 1)
						{ Search += " and a.Subject  like '%" + frm["SearchString"].ToString() + "%'"; }
						else if (SelValueSch == 2)
						{ Search += " and a.Body like '%" + frm["SearchString"].ToString() + "%'"; }
						else if (SelValueSch == 3)
						{ Search += " and a.Id='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 4)
						{ Search += " and b.[user] like '%" + frm["SearchString"].ToString() + "%'"; }
					}
				}

				TempData["SearchSentMail"] = Search;
				DataSet result = objDB.ViewInboxMaster(0, 2, AdminId, Search, pageIndex, 20);
				if (result == null)
				{
					ViewBag.LastPageIndex = 0;
					ViewBag.TotalCount = 0;
					ViewBag.TotalCount1 = 0;
				}
				else if (result.Tables[0].Rows.Count > 0)
				{
					im.StoreAllData = result;
					ViewBag.TotalCount = im.StoreAllData.Tables[0].Rows.Count;
					int count = Convert.ToInt32(im.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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
				return View(im);
			}
		}


		[HttpPost]
		public ActionResult MailReplyMaster(InboxModel im, FormCollection frm, IEnumerable<HttpPostedFileBase> Replyfiles, string Reply)
		{
			//  ViewBag.MyAdminList = MenuAdminList("");
			if (Session["AdminType"] == null)
			{ return RedirectToAction("Index", "Admin"); }

			// HttpPostedFileBase ObjectionLetter,
			if (Convert.ToInt32(frm["MId"].ToString()) > 0 && frm["Subject"] != null)
			{
				string AdminId = Session["AdminId"].ToString();
				im.id = Convert.ToInt32(frm["MId"].ToString());
				im.To = frm["SentFrom"].ToString();
				im.Subject = frm["Subject"].ToString();
				im.From = Session["AdminId"].ToString();
				im.Reply = Reply.ToString();

				string SelectedAdmin = "";
				if (frm["SelectedAdmin"] == "" || frm["SelectedAdmin"] == null)
				{
					// ViewData["Result"] = 20;
					// return View(im);
				}
				else
				{
					if (Session["AdminType"].ToString().ToLower() == "admin")
					{
						SelectedAdmin = frm["SelectedAdmin"].ToString();
						im.To = frm["SelectedAdmin"].ToString();
					}
				}


				string SelFiles = "";
				string guidObj = Guid.NewGuid().ToString().Substring(0, 5);
				if (Replyfiles != null)
				{
					if (Replyfiles.Count() > 0)
					{
						if (Replyfiles.Count() <= 2)
						{
							foreach (var file in Replyfiles)
							{
								if (file != null && file.ContentLength > 0)
								{
									// file.SaveAs(Path.Combine(Server.MapPath("/UploadFiles"), im.Subject.ToUpper() + "_" + Path.GetFileNameWithoutExtension(file.FileName).ToUpper() + Path.GetExtension(file.FileName)));
									string fileName = "MailFiles/Reply/" + Path.GetFileNameWithoutExtension(file.FileName).Replace("/", "_").ToUpper() + "_M" + im.id + "_From" + im.From + "_To" + im.To + "_" + guidObj.ToString() + Path.GetExtension(file.FileName);
									SelFiles += fileName + ",";
								}
							}
						}
						else
						{
							ViewData["Result"] = 10;
							return View(im);
						}
					}
				}

				if (SelFiles != "")
				{ SelFiles = SelFiles.Remove(SelFiles.LastIndexOf(","), 1); }
				im.Attachments = SelFiles.ToString();

				string OutError = "0";
				DataTable dt1 = objDB.MailReplyMaster(im, out OutError);
				if (OutError == "1")
				{
					ViewData["Result"] = 1;
					if (Replyfiles != null)
					{
						foreach (var file in Replyfiles)
						{
							if (file != null && file.ContentLength > 0)
							{
								/// string fileName = "MailFiles/Reply/" + im.Subject.Replace("/", "_").ToUpper() + AdminId + "_To_" + SentFrom1 + "_" + guidObj.ToString() + "_" + Path.GetFileNameWithoutExtension(file.FileName).Replace("/", "_").ToUpper() + Path.GetExtension(file.FileName);
								//     string fileName = "MailFiles/Reply/" + Path.GetFileNameWithoutExtension(file.FileName).Replace("/", "_").ToUpper()+ "_M" + im.id + "_From" + im.From + "_To" + im.To + "_" + guidObj.ToString() + Path.GetExtension(file.FileName);

								//file.SaveAs(Path.Combine(Server.MapPath("/MailFiles"), im.Subject.ToUpper() + "_" + Path.GetFileNameWithoutExtension(file.FileName).ToUpper() + Path.GetExtension(file.FileName)));
								var path = Path.Combine(Server.MapPath("~/Upload/MailFiles/Reply/"), Path.GetFileNameWithoutExtension(file.FileName).Replace("/", "_").ToUpper() + "_M" + im.id + "_From" + im.From + "_To" + im.To + "_" + guidObj.ToString() + Path.GetExtension(file.FileName));
								string FilepathExist = Path.Combine(Server.MapPath("~/Upload/MailFiles/Reply/"));
								if (!Directory.Exists(FilepathExist))
								{
									Directory.CreateDirectory(FilepathExist);
								}
								file.SaveAs(path);
							}
						}
					}

				}
				else
				{
					ViewData["Result"] = 0;
				}
			}
			else
			{
				ViewData["Result"] = 5;
			}

			return RedirectToAction("ViewInbox", "Admin");
			//  return View(im);

		}


		#endregion Admin Inbox Master


		//
		#region Recheck Conduct Branch
		public ActionResult RecheckConductPrint()
		{
			FormCollection frm = new FormCollection();
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				if (!Session["UserName"].ToString().ToLower().Contains("conduct"))
				{
					if (!Session["PAccessRight"].ToString().Contains("Branch"))
					{
						return RedirectToAction("Index", "Admin");
					}
				}


				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "conduct2" ? "ReEvaluation" : "Rechecking";


				int pageIndex = 1;
				ViewBag.pagesize = pageIndex;

				var itemsch = new SelectList(new[]{new {ID="2",Name="Particular"},new {ID="1",Name="Subject"},new{ID="3",Name="Stream"},
			new{ID="4",Name="Image"},}, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();

				am.CorrectionType = "";
				am.CorrectionFromDate = "";
				am.CorrectionToDate = "";
				string UserName = Session["UserName"].ToString();
				am.StoreAllData = objDB.RecheckConductPrint(am, UserName, pageIndex);
				ViewBag.TotalCountToCreateLotSr = am.StoreAllData.Tables[1].Rows[0]["PRecordSr"].ToString();
				ViewBag.TotalCountToCreateLotMt = am.StoreAllData.Tables[2].Rows[0]["PRecordMt"].ToString();

				// ViewBag.TotalCount = 0;//am.StoreAllData.Tables[0].Rows.Count;
				ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(am);
				}
				else
				{
					ViewBag.TotalCount = 1;
					return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}
		[HttpPost]
		public ActionResult RecheckConductPrint(FormCollection frm, string cmd)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{
				//if (Session["UserName"] == null || Session["AdminType"].ToString().ToUpper() != "BRANCH")
				//{
				//    return RedirectToAction("Index", "Admin");
				//}
				if (!Session["UserName"].ToString().ToLower().Contains("conduct"))
				{
					if (!Session["PAccessRight"].ToString().Contains("Branch"))
					{
						return RedirectToAction("Index", "Admin");
					}
				}
				if (cmd == "Back")
				{
					return RedirectToAction("RecheckConductPrint", "Admin");
				}
				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "conduct2" ? "ReEvaluation" : "Rechecking";

				string Cls = "";
				string UserName = Session["UserName"].ToString();
				if (cmd == "Create Lot Senior Secondary")
				{
					Cls = "12";
					am.StoreAllData = objDB.RecheckConductCreateLot(am, UserName, Cls);
					ViewBag.TotalCountToCreateLotSr = am.StoreAllData.Tables[1].Rows[0]["PRecordSr"].ToString();
					ViewBag.TotalCountToCreateLotMt = am.StoreAllData.Tables[2].Rows[0]["PRecordMt"].ToString();
				}
				if (cmd == "Create Lot Matric")
				{
					Cls = "10";
					am.StoreAllData = objDB.RecheckConductCreateLot(am, UserName, Cls);
					ViewBag.TotalCountToCreateLotSr = am.StoreAllData.Tables[1].Rows[0]["PRecordSr"].ToString();
					ViewBag.TotalCountToCreateLotMt = am.StoreAllData.Tables[2].Rows[0]["PRecordMt"].ToString();
				}

				return RedirectToAction("RecheckConductPrint", "Admin");
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Admin");
			}
		}

		public ActionResult ConductPrintList(string cls, string lot)
		{
			FormCollection frm = new FormCollection();
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			var itemsch = new SelectList(new[] { new { ID = "1", Name = "SET" }, new { ID = "2", Name = "Ref No" },
				new { ID = "3", Name = "ROLL No" }, new { ID = "4", Name = "Dist" },
				new { ID = "5", Name = "AllDist" },new { ID = "6", Name = "RTI" }, }, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			try
			{
				//if (Session["UserName"] == null || Session["AdminType"].ToString().ToUpper() != "BRANCH")
				//{
				//    return RedirectToAction("Index", "Admin");
				//}
				if (!Session["UserName"].ToString().ToLower().Contains("conduct"))
				{
					if (!Session["PAccessRight"].ToString().Contains("Branch"))
					{
						return RedirectToAction("Index", "Admin");
					}
				}


				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "conduct2" ? "ReEvaluation" : "Rechecking";


				int pageIndex = 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;
				Search = "a.subnm like '%%'";
				string UserName = Session["UserName"].ToString();
				// am.StoreAllData = objDB.ConductPrintList(cls, lot, Search);
				//  ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
				ViewBag.lot = lot;
				HttpContext.Session["cls"] = cls;
				HttpContext.Session["lot"] = lot;
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
				ViewBag.IsMonth = "";
				return View(am);
				//if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				//{
				//    ViewBag.Message = "Record Not Found";
				//    ViewBag.TotalCount = 0;
				//    return View(am);
				//}
				//else
				//{
				//    ViewBag.TotalCount = 1;
				//    return View(am);
				//}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}

		[HttpPost]
		public ActionResult ConductPrintList(string cls, string lot, string cmd, string SelList, string SearchString, FormCollection frm)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			var itemsch = new SelectList(new[] { new { ID = "1", Name = "SET" }, new { ID = "2", Name = "Ref No" },
				new { ID = "3", Name = "ROLL No" }, new { ID = "4", Name = "Dist" },
				new { ID = "5", Name = "AllDist" },new { ID = "6", Name = "RTI" }, }, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			try
			{
				if (!Session["UserName"].ToString().ToLower().Contains("conduct"))
				{
					if (!Session["PAccessRight"].ToString().Contains("Branch"))
					{
						return RedirectToAction("Index", "Admin");
					}
				}
				if (Session["cls"].ToString() != null || Session["cls"].ToString() != "")
				{
					cls = Session["cls"].ToString();
					lot = Session["lot"].ToString();
				}

				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "conduct2" ? "ReEvaluation" : "Rechecking";

				ViewBag.IsMonth = "";
				string Search = "";
				Search = "a.subnm like '%%'";
				if (cmd == "Search")
				{
					if (SelList != "")
					{
						ViewBag.SelectedItem = SelList;
						int SelValueSch = Convert.ToInt32(SelList.ToString());
						if (SearchString != "")
						{
							if (SelValueSch == 1)
							{ Search += " and b.[set]='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 2)
							{ Search += "and a.refno='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  a.roll ='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 4)
							{ Search += " and  dst.Dist ='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  a.ISRTI=1 and  SECID ='" + SearchString.ToString().Trim() + "' "; }

						}
						else if (SelValueSch == 5)
						{ Search += " and  dst.Dist like '%" + SearchString.ToString().Trim() + "%'"; }
						else if (SelValueSch == 6)
						{ Search += " and  a.ISRTI=1  "; }
					}
				}
				int pageIndex = 1;
				ViewBag.pagesize = pageIndex;
				ViewBag.SearchList = frm["SelList"].ToString();
				ViewBag.Searchstring = SearchString.ToString().Trim();
				string UserName = Session["UserName"].ToString();
				am.StoreAllData = objDB.ConductPrintList(cls, lot, Search);
				ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
				ViewBag.lot = lot;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					ViewBag.IsMonth = "";
					return View(am);
				}
				else
				{
					ViewBag.IsMonth = am.StoreAllData.Tables[0].Rows[0]["month"].ToString();
					ViewBag.TotalCount = 1;
					return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}

		public ActionResult ConductPrintList_New(string cls, string lot)
		{
			FormCollection frm = new FormCollection();
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			var itemsch = new SelectList(new[] { new { ID = "1", Name = "SET" }, new { ID = "2", Name = "Ref No" },
				new { ID = "3", Name = "ROLL No" }, new { ID = "4", Name = "Dist" },
				new { ID = "5", Name = "AllDist" },new { ID = "6", Name = "RTI" }, }, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			ViewBag.IsMonth = "";
			try
			{
				//if (Session["UserName"] == null || Session["AdminType"].ToString().ToUpper() != "BRANCH")
				//{
				//    return RedirectToAction("Index", "Admin");
				//}
				if (!Session["UserName"].ToString().ToLower().Contains("conduct"))
				{
					if (!Session["PAccessRight"].ToString().Contains("Branch"))
					{
						return RedirectToAction("Index", "Admin");
					}
				}
				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "conduct2" ? "ReEvaluation" : "Rechecking";

				int pageIndex = 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;
				Search = "a.subnm like '%%'";
				string UserName = Session["UserName"].ToString();
				// am.StoreAllData = objDB.ConductPrintList_New(cls, lot, Search);
				//  ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
				ViewBag.lot = lot;
				HttpContext.Session["cls"] = cls;
				HttpContext.Session["lot"] = lot;
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
				return View(am);
				//if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				//{
				//    ViewBag.Message = "Record Not Found";
				//    ViewBag.TotalCount = 0;
				//    return View(am);
				//}
				//else
				//{
				//    ViewBag.TotalCount = 1;
				//    return View(am);
				//}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}

		[HttpPost]
		public ActionResult ConductPrintList_New(string cls, string lot, string cmd, string SelList, string SearchString, FormCollection frm)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			var itemsch = new SelectList(new[] { new { ID = "1", Name = "SET" }, new { ID = "2", Name = "Ref No" },
				new { ID = "3", Name = "ROLL No" }, new { ID = "4", Name = "Dist" },
				new { ID = "5", Name = "AllDist" }, new { ID = "6", Name = "RTI" }, }, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			ViewBag.IsMonth = "";
			try
			{
				if (!Session["UserName"].ToString().ToLower().Contains("conduct"))
				{
					if (!Session["PAccessRight"].ToString().Contains("Branch"))
					{
						return RedirectToAction("Index", "Admin");
					}
				}
				if (Session["cls"].ToString() != null || Session["cls"].ToString() != "")
				{
					cls = Session["cls"].ToString();
					lot = Session["lot"].ToString();
				}
				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "conduct2" ? "ReEvaluation" : "Rechecking";

				string Search = "";
				Search = "a.subnm like '%%'";
				if (cmd == "Search")
				{
					if (SelList != "")
					{
						ViewBag.SelectedItem = SelList;
						int SelValueSch = Convert.ToInt32(SelList.ToString());
						if (SearchString != "")
						{
							if (SelValueSch == 1)
							{ Search += " and b.[set]='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 2)
							{ Search += "and a.refno='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  a.roll ='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 4)
							{ Search += " and  dst.Dist ='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  a.ISRTI=1 and  SECID ='" + SearchString.ToString().Trim() + "' "; }
						}
						else if (SelValueSch == 5)
						{ Search += " and  dst.Dist like '%" + SearchString.ToString().Trim() + "%'"; }
						else if (SelValueSch == 6)
						{ Search += " and  a.ISRTI=1"; }
					}
				}
				int pageIndex = 1;
				ViewBag.pagesize = pageIndex;
				ViewBag.SearchList = frm["SelList"].ToString();
				ViewBag.Searchstring = SearchString.ToString().Trim();
				string UserName = Session["UserName"].ToString();
				am.StoreAllData = objDB.ConductPrintList_New(cls, lot, Search);
				ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
				ViewBag.lot = lot;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					ViewBag.IsMonth = "";
					return View(am);
				}
				else
				{
					ViewBag.IsMonth = am.StoreAllData.Tables[0].Rows[0]["month"].ToString();
					ViewBag.TotalCount = 1;
					return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}
		public ActionResult ConductUpdateList(string cls, string lot)
		{
			FormCollection frm = new FormCollection();
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			var itemsch = new SelectList(new[] { new { ID = "1", Name = "SET" }, new { ID = "2", Name = "Ref No" }, new { ID = "3", Name = "ROLL No" }, new { ID = "4", Name = "Pending" }, new { ID = "5", Name = "Dist" }, }, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			try
			{
				//if (Session["UserName"] == null || Session["AdminType"].ToString().ToUpper() != "BRANCH")
				//{
				//    return RedirectToAction("Index", "Admin");
				//}
				if (Session["UserName"].ToString() != "Conduct" && Session["UserName"].ToString() != "Pseb Admin")
				{
					if (!Session["PAccessRight"].ToString().Contains("Branch"))
					{
						return RedirectToAction("Index", "Admin");
					}
				}
				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "conduct2" ? "ReEvaluation" : "Rechecking";

				int pageIndex = 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;
				Search = "a.subnm like '%%'";
				string UserName = Session["UserName"].ToString();
				am.StoreAllData = null; // objDB.ConductPrintList(cls, lot, Search);
				ViewBag.TotalCountP = 0; // am.StoreAllData.Tables[0].Rows.Count;
				ViewBag.lot = lot;
				HttpContext.Session["cls"] = cls;
				HttpContext.Session["lot"] = lot;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(am);
				}
				else
				{
					ViewBag.TotalCount = 1;
					return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}

		[HttpPost]
		public ActionResult ConductUpdateList(string cls, string lot, string cmd, string SelList, string SearchString, FormCollection frm)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			var itemsch = new SelectList(new[] { new { ID = "1", Name = "SET" }, new { ID = "2", Name = "Ref No" }, new { ID = "3", Name = "ROLL No" }, new { ID = "4", Name = "Pending" }, new { ID = "5", Name = "Dist" }, }, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			try
			{
				//if (Session["UserName"] == null || Session["AdminType"].ToString().ToUpper() != "BRANCH")
				//{
				//    return RedirectToAction("Index", "Admin");
				//}
				if (Session["UserName"].ToString() != "Conduct" && Session["UserName"].ToString() != "Pseb Admin")
				{
					if (!Session["PAccessRight"].ToString().Contains("Branch"))
					{
						return RedirectToAction("Index", "Admin");
					}
				}
				if (Session["cls"].ToString() != null || Session["cls"].ToString() != "")
				{
					cls = Session["cls"].ToString();
					lot = Session["lot"].ToString();
				}
				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "conduct2" ? "ReEvaluation" : "Rechecking";

				string Search = "";
				Search = "a.subnm like '%%'";

				if (cmd == "Search")
				{
					if (SelList != "")
					{
						ViewBag.SelectedItem = SelList;
						int SelValueSch = Convert.ToInt32(SelList.ToString());
						if (SearchString != "")
						{
							if (SelValueSch == 1)
							{ Search += " and b.[set]='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 2)
							{ Search += "and a.refno='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  a.roll ='" + SearchString.ToString().Trim() + "'"; }

							else if (SelValueSch == 5)
							{ Search += " and  dst.dist ='" + SearchString.ToString().Trim() + "'"; }
							Session["Search"] = Search;
						}
						if (SelValueSch == 4)
						{
							Search += " and  a.UpdStatus is null";
							SearchString = "";
						}

					}
				}
				ViewBag.Searchstring = SearchString.ToString().Trim();
				//if (cmd == "Final submit")
				if (cmd == "Updates")
				{

					string Ychk = frm["AllChkId"];
					string Nchk = frm["NAllChkId"];
					if (Ychk != null || Nchk != null)
					{

						string updtStrY = string.Empty;
						string updtStrN = string.Empty;

						updtStrY = "ID in(" + Ychk + ")";
						updtStrN = "ID in(" + Nchk + ")";

						DataSet result = objDB.ConductUpdateList(updtStrY, updtStrN);

					}
					if (Session["Search"] != null)
					{
						Search = Session["Search"].ToString();
					}
				}
				if (cmd == "Final submit")
				{

					string Ychk = frm["AllChkId"];
					string Nchk = frm["NAllChkId"];
					if (Ychk != null || Nchk != null)
					{

						string updtStrY = string.Empty;
						string updtStrN = string.Empty;

						updtStrY = "ID in(" + Ychk + ")";
						updtStrN = "ID in(" + Nchk + ")";

						DataSet result = objDB.ConductUpdateListFS(updtStrY, updtStrN);

					}
					if (Session["Search"] != null)
					{
						Search = Session["Search"].ToString();
					}
				}
				int pageIndex = 1;
				ViewBag.pagesize = pageIndex;

				string UserName = Session["UserName"].ToString();
				am.StoreAllData = objDB.ConductPrintList(cls, lot, Search);
				ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
				ViewBag.lot = lot;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(am);
				}
				else
				{
					ViewBag.TotalCount = 1;
					return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}
		public ActionResult ConductUpdateListRemove(string cls, string lot)
		{
			FormCollection frm = new FormCollection();
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			var itemsch = new SelectList(new[] { new { ID = "1", Name = "SET" }, new { ID = "2", Name = "Ref No" }, new { ID = "3", Name = "ROLL No" }, new { ID = "4", Name = "Pending" }, new { ID = "5", Name = "Dist" }, }, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			try
			{
				//if (Session["UserName"] == null || Session["AdminType"].ToString().ToUpper() != "BRANCH")
				//{
				//    return RedirectToAction("Index", "Admin");
				//}
				if (Session["UserName"].ToString() != "Conduct" && Session["UserName"].ToString() != "Pseb Admin")
				{
					if (!Session["PAccessRight"].ToString().Contains("Branch"))
					{
						return RedirectToAction("Index", "Admin");
					}
				}
				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "conduct2" ? "ReEvaluation" : "Rechecking";
				int pageIndex = 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;
				Search = "a.subnm like '%%'";
				string UserName = Session["UserName"].ToString();
				am.StoreAllData = null; // objDB.ConductPrintList(cls, lot, Search);
				ViewBag.TotalCountP = 0; // am.StoreAllData.Tables[0].Rows.Count;
				ViewBag.lot = lot;
				HttpContext.Session["cls"] = cls;
				HttpContext.Session["lot"] = lot;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(am);
				}
				else
				{
					ViewBag.TotalCount = 1;
					return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}

		[HttpPost]
		public ActionResult ConductUpdateListRemove(string cls, string lot, string cmd, string SelList, string SearchString, FormCollection frm)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			var itemsch = new SelectList(new[] { new { ID = "1", Name = "SET" }, new { ID = "2", Name = "Ref No" }, new { ID = "3", Name = "ROLL No" }, new { ID = "4", Name = "Pending" }, new { ID = "5", Name = "Dist" }, }, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			try
			{
				//if (Session["UserName"] == null || Session["AdminType"].ToString().ToUpper() != "BRANCH")
				//{
				//    return RedirectToAction("Index", "Admin");
				//}
				if (Session["UserName"].ToString() != "Conduct" && Session["UserName"].ToString() != "Pseb Admin")
				{
					if (!Session["PAccessRight"].ToString().Contains("Branch"))
					{
						return RedirectToAction("Index", "Admin");
					}
				}
				if (Session["cls"].ToString() != null || Session["cls"].ToString() != "")
				{
					cls = Session["cls"].ToString();
					lot = Session["lot"].ToString();
				}
				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "conduct2" ? "ReEvaluation" : "Rechecking";
				string Search = "";
				Search = "a.subnm like '%%'";

				if (cmd == "Search")
				{
					if (SelList != "")
					{
						ViewBag.SelectedItem = SelList;
						int SelValueSch = Convert.ToInt32(SelList.ToString());
						if (SearchString != "")
						{
							if (SelValueSch == 1)
							{ Search += " and b.[set]='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 2)
							{ Search += "and a.refno='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  a.roll ='" + SearchString.ToString().Trim() + "'"; }

							else if (SelValueSch == 5)
							{ Search += " and  dst.dist ='" + SearchString.ToString().Trim() + "'"; }

							Session["Search"] = Search;
						}
						if (SelValueSch == 4)
						{ Search += " and  a.UpdStatus is null"; SearchString = ""; }

					}
				}
				ViewBag.Searchstring = SearchString.ToString().Trim();
				//if (cmd == "Final submit")
				if (cmd == "Remove Updates")
				{

					string Ychk = frm["AllChkIdRmv"];
					string Nchk = frm["NAllChkId"];
					if (Ychk != null || Nchk != null)
					{

						string updtStrY = string.Empty;
						string updtStrN = string.Empty;

						updtStrY = "ID in(" + Ychk + ")";
						updtStrN = "ID in(" + Nchk + ")";

						DataSet result = objDB.ConductUpdateListRemove(updtStrY, updtStrN);

					}
					if (Session["Search"] != null)
					{
						Search = Session["Search"].ToString();
					}
				}
				//if (cmd == "Final submit")
				//{

				//    string Ychk = frm["AllChkId"];
				//    string Nchk = frm["NAllChkId"];
				//    if (Ychk != null || Nchk != null)
				//    {

				//        string updtStrY = string.Empty;
				//        string updtStrN = string.Empty;

				//        updtStrY = "ID in(" + Ychk + ")";
				//        updtStrN = "ID in(" + Nchk + ")";

				//        DataSet result = objDB.ConductUpdateListFS(updtStrY, updtStrN);

				//    }
				//    if (Session["Search"] != null)
				//    {
				//        Search = Session["Search"].ToString();
				//    }
				//}
				int pageIndex = 1;
				ViewBag.pagesize = pageIndex;

				string UserName = Session["UserName"].ToString();
				am.StoreAllData = objDB.ConductPrintList(cls, lot, Search);
				ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
				ViewBag.lot = lot;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(am);
				}
				else
				{
					ViewBag.TotalCount = 1;
					return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}
		#endregion
		//
		//
		#region Recheck Secrecy Branch
		public ActionResult RecheckSecrecyPrint()
		{
			FormCollection frm = new FormCollection();
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{

				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString().ToUpper() != "SECRECY")
				//{
				//    return RedirectToAction("Index", "Admin");
				//}

				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "secrecy2" ? "ReEvaluation" : "Rechecking";
				int pageIndex = 1;
				ViewBag.pagesize = pageIndex;

				var itemsch = new SelectList(new[]{new {ID="2",Name="Particular"},new {ID="1",Name="Subject"},new{ID="3",Name="Stream"},
			new{ID="4",Name="Image"},}, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();

				am.CorrectionType = "";
				am.CorrectionFromDate = "";
				am.CorrectionToDate = "";
				string UserName = Session["UserName"].ToString();
				//am.StoreAllData = objDB.RecheckConductPrint(am, UserName, pageIndex);
				am.StoreAllData = objDB.RecheckSecrecyPrint(am, UserName, pageIndex);
				ViewBag.TotalCountToCreateLotSr = am.StoreAllData.Tables[1].Rows[0]["PRecordSr"].ToString();
				ViewBag.TotalCountToCreateLotMt = am.StoreAllData.Tables[2].Rows[0]["PRecordMt"].ToString();

				// ViewBag.TotalCount = 0;//am.StoreAllData.Tables[0].Rows.Count;
				ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(am);
				}
				else
				{
					ViewBag.TotalCount = 1;
					return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}
		[HttpPost]
		public ActionResult RecheckSecrecyPrint(FormCollection frm, string cmd)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{

				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "secrecy2" ? "ReEvaluation" : "Rechecking";
				//if (cmd == "Back")
				//{
				//    return RedirectToAction("FirmSchoolCorrectionFinalSubmit", "Admin");
				//}
				string Cls = "";
				if (cmd == "Create Lot Senior Secondary")
				{
					string UserName = Session["UserName"].ToString();
					Cls = "4";
					//am.StoreAllData = objDB.RecheckConductCreateLot(am, UserName, Cls);
					am.StoreAllData = objDB.RecheckSecrecyCreateLot(am, UserName, Cls);
					ViewBag.TotalCountToCreateLotSr = am.StoreAllData.Tables[1].Rows[0]["PRecordSr"].ToString();
					ViewBag.TotalCountToCreateLotMt = am.StoreAllData.Tables[2].Rows[0]["PRecordMt"].ToString();
				}
				if (cmd == "Create Lot Matric")
				{
					string UserName = Session["UserName"].ToString();
					Cls = "2";
					// am.StoreAllData = objDB.RecheckConductCreateLot(am, UserName, Cls);
					am.StoreAllData = objDB.RecheckSecrecyCreateLot(am, UserName, Cls);
					ViewBag.TotalCountToCreateLotSr = am.StoreAllData.Tables[1].Rows[0]["PRecordSr"].ToString();
					ViewBag.TotalCountToCreateLotMt = am.StoreAllData.Tables[2].Rows[0]["PRecordMt"].ToString();
				}

				var itemsch = new SelectList(new[]{new {ID="2",Name="Particular"},new {ID="1",Name="Subject"},new{ID="3",Name="Stream"},
			new{ID="4",Name="Image"},}, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();

				return RedirectToAction("RecheckSecrecyPrint", "Admin");
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Admin");
			}
		}

		public ActionResult SecrecyPrintList(string cls, string lot)
		{
			FormCollection frm = new FormCollection();
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			//var itemsch = new SelectList(new[] { new { ID = "1", Name = "SECID" }, }, "ID", "Name", 1);
			var itemsch = new SelectList(new[] { new { ID = "1", Name = "SET" }, new { ID = "2", Name = "SECID" },
				new { ID = "3", Name = "District Code" }, new { ID = "4", Name = "RTI" }, new { ID = "5", Name = "RTI BY DIST" }, }, "ID", "Name", 1);

			ViewBag.MySch = itemsch.ToList();
			ViewBag.IsMonth = "";
			try
			{
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "secrecy2" ? "ReEvaluation" : "Rechecking";
				int pageIndex = 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;
				Search = "SECID like '%%'";
				string UserName = Session["UserName"].ToString();
				ViewBag.Message = "Record Not Found";
				HttpContext.Session["Seccls"] = cls;
				HttpContext.Session["Seclot"] = lot;
				ViewBag.TotalCount = 0;
				return View(am);
			}
			catch (Exception)
			{
				return RedirectToAction("SecrecyPrintList", "Admin");
			}
		}

		[HttpPost]
		public ActionResult SecrecyPrintList(string cls, string lot, string cmd, string SelList, string SearchString, FormCollection frm)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			//var itemsch = new SelectList(new[] { new { ID = "1", Name = "SECID" },}, "ID", "Name", 1);
			var itemsch = new SelectList(new[] { new { ID = "1", Name = "SET" }, new { ID = "2", Name = "SECID" },
				new { ID = "3", Name = "District Code" }, new { ID = "4", Name = "RTI" }, new { ID = "5", Name = "RTI BY DIST" } }, "ID", "Name", 1);

			ViewBag.MySch = itemsch.ToList();
			ViewBag.IsMonth = "";
			try
			{
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				if (Session["Seccls"].ToString() != null || Session["Seccls"].ToString() != "")
				{
					cls = Session["Seccls"].ToString();
					lot = Session["Seclot"].ToString();
				}
				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "secrecy2" ? "ReEvaluation" : "Rechecking";
				string Search = "";
				Search = "SECID like '%%'";
				int SelValueSch = 0;
				if (cmd == "Search")
				{
					if (SelList != "")
					{
						ViewBag.SelectedItem = SelList;
						ViewBag.Searchstring = SearchString.ToString().Trim();
						SelValueSch = Convert.ToInt32(SelList.ToString());

						if (SearchString != "")
						{
							if (SelValueSch == 1)
							{ Search += " and b.[SET]='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 2)
							{ Search += " and [SECID]='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and dst.Dist='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 4)
							{ Search += " and  a.ISRTI=1 and  SECID ='" + SearchString.ToString().Trim() + "' "; }
							else if (SelValueSch == 5)
							{ Search += " and  a.ISRTI=1 and  dst.Dist ='" + SearchString.ToString().Trim() + "' "; }
						}
						else if (SelValueSch == 4 || SelValueSch == 5)
						{ Search += " and  a.ISRTI=1 "; }
					}
				}
				int pageIndex = 1;
				ViewBag.pagesize = pageIndex;
				string UserName = Session["UserName"].ToString();
				//am.StoreAllData = objDB.ConductPrintList(cls, lot, Search);
				am.StoreAllData = objDB.SecrecyPrintList(cls, lot, Search, SearchString.ToString().Trim());
				ViewBag.lot = lot;
				ViewBag.cls = cls;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.IsMonth = "";
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(am);
				}
				else
				{
					ViewBag.IsMonth = am.StoreAllData.Tables[0].Rows[0]["month"].ToString();
					ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
					ViewBag.TotalCount1 = am.StoreAllData.Tables[1].Rows.Count;
					ViewBag.SECID = am.StoreAllData.Tables[0].Rows[0]["SECID"].ToString().ToUpper();
					//SET
					ViewBag.SET = am.StoreAllData.Tables[0].Rows[0]["SET"].ToString().ToUpper();
					ViewBag.SecrecyLot = am.StoreAllData.Tables[0].Rows[0]["SecrecyLot"].ToString();
					ViewBag.SecrecyLotDT = am.StoreAllData.Tables[0].Rows[0]["SecrecyLotDT"].ToString();
					if (am.StoreAllData.Tables.Count > 2)
					{
						ViewBag.SCHLE = am.StoreAllData.Tables[2].Rows[0]["SCHLE"].ToString();
						ViewBag.SCHLP = am.StoreAllData.Tables[2].Rows[0]["SCHLP"].ToString();
						ViewBag.PRINCIPAL = am.StoreAllData.Tables[2].Rows[0]["PRINCIPAL"].ToString();
						ViewBag.MOBILE = am.StoreAllData.Tables[2].Rows[0]["MOBILE"].ToString();
						ViewBag.EMAILID = am.StoreAllData.Tables[2].Rows[0]["EMAILID"].ToString();

					}
					return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("SecrecyPrintList", "Admin");
			}
		}
		public ActionResult SecrecyUpdateListRemove(string cls, string lot)
		{
			FormCollection frm = new FormCollection();
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			var itemsch = new SelectList(new[] { new { ID = "1", Name = "SET" }, new { ID = "2", Name = "Ref No" }, new { ID = "3", Name = "ROLL No" }, new { ID = "4", Name = "Pending" }, new { ID = "5", Name = "Dist" }, }, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			try
			{
				//if (Session["UserName"] == null || Session["AdminType"].ToString().ToUpper() != "BRANCH")
				//{
				//    return RedirectToAction("Index", "Admin");
				//}
				if (Session["UserName"].ToString() != "Secrecy" && Session["UserName"].ToString() != "Pseb Admin")
				{
					if (!Session["PAccessRight"].ToString().Contains("Branch"))
					{
						return RedirectToAction("Index", "Admin");
					}
				}
				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "secrecy2" ? "ReEvaluation" : "Rechecking";
				int pageIndex = 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;
				Search = "";
				string UserName = Session["UserName"].ToString();
				am.StoreAllData = null; // objDB.ConductPrintList(cls, lot, Search);
				ViewBag.TotalCountP = 0; // am.StoreAllData.Tables[0].Rows.Count;
				ViewBag.lot = lot;
				HttpContext.Session["cls"] = cls;
				HttpContext.Session["lot"] = lot;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(am);
				}
				else
				{
					ViewBag.TotalCount = 1;
					return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}

		[HttpPost]
		public ActionResult SecrecyUpdateListRemove(string cls, string lot, string cmd, string SelList, string SearchString, FormCollection frm)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			var itemsch = new SelectList(new[] { new { ID = "1", Name = "SET" }, new { ID = "2", Name = "Ref No" }, new { ID = "3", Name = "ROLL No" }, new { ID = "4", Name = "Pending" }, new { ID = "5", Name = "Dist" }, }, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			try
			{
				//if (Session["UserName"] == null || Session["AdminType"].ToString().ToUpper() != "BRANCH")
				//{
				//    return RedirectToAction("Index", "Admin");
				//}
				if (Session["UserName"].ToString() != "Secrecy" && Session["UserName"].ToString() != "Pseb Admin")
				{
					if (!Session["PAccessRight"].ToString().Contains("Branch"))
					{
						return RedirectToAction("Index", "Admin");
					}
				}
				if (Session["cls"].ToString() != null || Session["cls"].ToString() != "")
				{
					cls = Session["cls"].ToString();
					lot = Session["lot"].ToString();
				}
				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "secrecy2" ? "ReEvaluation" : "Rechecking";
				string Search = "";
				Search = "b.refno is not null";

				if (cmd == "Search")
				{
					if (SelList != "")
					{
						ViewBag.SelectedItem = SelList;
						int SelValueSch = Convert.ToInt32(SelList.ToString());
						if (SearchString != "")
						{
							if (SelValueSch == 1)
							{ Search += " and b.[set]='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 2)
							{ Search += " and b.refno='" + SearchString.ToString().Trim() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and b.roll ='" + SearchString.ToString().Trim() + "'"; }

							else if (SelValueSch == 5)
							{ Search += " and  dst.dist ='" + SearchString.ToString().Trim() + "'"; }

							Session["Search"] = Search;
						}
						if (SelValueSch == 4)
						{ Search += " and  b.SecrecyLot is null"; SearchString = ""; }

					}
				}
				ViewBag.Searchstring = SearchString.ToString().Trim();
				//if (cmd == "Final submit")
				if (cmd == "Remove Updates")
				{

					string Ychk = frm["AllChkIdRmv"];
					string Nchk = frm["NAllChkId"];
					if (Ychk != null || Nchk != null)
					{

						string updtStrY = string.Empty;
						string updtStrN = string.Empty;

						updtStrY = "ID in(" + Ychk + ")";
						updtStrN = "ID in(" + Nchk + ")";

						DataSet result = objDB.SecrecyUpdateListRemove(updtStrY, updtStrN);

					}
					if (Session["Search"] != null)
					{
						Search = Session["Search"].ToString();
					}
				}
				//if (cmd == "Final submit")
				//{

				//    string Ychk = frm["AllChkId"];
				//    string Nchk = frm["NAllChkId"];
				//    if (Ychk != null || Nchk != null)
				//    {

				//        string updtStrY = string.Empty;
				//        string updtStrN = string.Empty;

				//        updtStrY = "ID in(" + Ychk + ")";
				//        updtStrN = "ID in(" + Nchk + ")";

				//        DataSet result = objDB.ConductUpdateListFS(updtStrY, updtStrN);

				//    }
				//    if (Session["Search"] != null)
				//    {
				//        Search = Session["Search"].ToString();
				//    }
				//}
				int pageIndex = 1;
				ViewBag.pagesize = pageIndex;

				string UserName = Session["UserName"].ToString();
				am.StoreAllData = objDB.GetSecrecyUpdateListRemove(cls, lot, Search);
				ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
				ViewBag.lot = lot;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(am);
				}
				else
				{
					ViewBag.TotalCount = 1;
					return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}


		public ActionResult SecrecyViewList(string cls, string lot)
		{
			FormCollection frm = new FormCollection();
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			//var itemsch = new SelectList(new[] { new { ID = "1", Name = "SECID" }, }, "ID", "Name", 1);
			var itemsch = new SelectList(new[] { new { ID = "1", Name = "SET" }, new { ID = "2", Name = "SECID" },
				new { ID = "3", Name = "District Code" }, new { ID = "4", Name = "RTI" }, }, "ID", "Name", 1);

			ViewBag.MySch = itemsch.ToList();

			try
			{
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				ViewBag.IsRecheck = Session["UserName"].ToString().ToLower() == "secrecy2" ? "ReEvaluation" : "Rechecking";
				int pageIndex = 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;
				Search = "SECID like '%%' and a.class='" + cls + "'  and a.SecrecyLot='" + lot + "'";
				string UserName = Session["UserName"].ToString();
				am.StoreAllData = objDB.SecrecyViewList(cls, lot, Search);
				ViewBag.lot = lot;
				ViewBag.cls = cls;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(am);
				}
				else
				{
					ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
					return View(am);
				}


			}
			catch (Exception)
			{
				return RedirectToAction("SecrecyPrintList", "Admin");
			}
		}


		#endregion
		//

		#region Open School Report

		public ActionResult SchoolReport()
		{
			string districts = string.Empty;
			// Dist Allowed
			string DistAllow = "";
			if (ViewBag.DistAllow != null)
			{
				DistAllow = ViewBag.DistAllow;
			}
			List<SelectListItem> OpenDistricts = objDB1.OpenSchoolDistricts();
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

		[HttpPost]
		public ActionResult SchoolReport(FormCollection fc)
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
				var obj = new AbstractLayer.OpenDB().GetSchoolRecords(fc["ddlDist"].ToString());
				ViewBag.data = obj;
			}
			return View();
		}

		#endregion Open School Report

		#region Open ApplicationDetails


		public ActionResult ApplicationDetails(int? page)
		{
			try
			{
				if (Session["AdminType"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				//ViewBag.Id = id;
				//Session["Id"] = id;
				if (Convert.ToString(Session["AdminId"]) != "")
				{

				}
				else
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
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						ViewBag.IsView = 1;
						ViewBag.IsModiFy = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/EDITREGISTRATION")).Count();
					}
				}
				#endregion Action Assign Method





				var itemsch = new SelectList(new[]{new {ID="6",Name="SCHL"},new {ID="1",Name="By Application Number"},new{ID="2",Name="Candidate Name"},
			new{ID="3",Name="Father's Name"},new{ID="4",Name="Mother's Name"},new{ID="5",Name="DOB"},
			new{ID="7",Name="Old Roll"},new{ID="8",Name="Mobile"},new{ID="9",Name="Aadhar No"},new{ID="10",Name="Email id"},
				new{ID="11",Name="Reg. No."},new{ID="12",Name="Student Id"},new{ID="13",Name="Challan Id"},
				}, "ID", "Name", 1);

				ViewBag.MySch = itemsch.ToList();

				List<SelectListItem> forms = new List<SelectListItem>() { new SelectListItem() { Text = "Matric", Value = "M3" }, new SelectListItem() { Text = "Senior", Value = "T3" } };
				ViewBag.FormType = forms;
				if (TempData["SearchString"] == null)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.SelectedForm = TempData["SelForm"] != null ? TempData["SelForm"].ToString() : string.Empty;
					ViewBag.SelectedItem = TempData["SelList"] != null ? TempData["SelList"].ToString() : string.Empty;
					ViewBag.SearchString = TempData["SearchString"] != null ? TempData["SearchString"].ToString() : string.Empty;
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;
					Printlist obj = new Printlist();
					string Search = "";
					string formType = (TempData["SelForm"] != null) ? TempData["SelForm"].ToString() : "";
					ViewBag.SelectedForm = formType;
					TempData["SelForm"] = formType;
					string dist = Session["DistAllow"].ToString();
					string clas = string.Empty;
					dist = "('" + dist.Replace(",", "','") + "')";
					if (formType.ToUpper().Trim() == "M3" || formType.ToUpper().Trim() == "T3" || formType.ToUpper().Trim() == string.Empty)
					{
						clas = (formType.Trim() == string.Empty) ? "FORM='M3' or FORM='T3'" : "FORM = '" + formType + "'";
						Search = "b.homedist in " + dist + ((formType.Trim() == string.Empty) ? " and (c.FORM='M3' or c.FORM='T3')" : " and c.FORM = '" + formType + "'");
						if (TempData["SelList"].ToString() != "")
						{
							TempData["SelList"] = TempData["SelList"].ToString();
							ViewBag.SelectedItem = TempData["SelList"];
							int SelValueSch = Convert.ToInt32(TempData["SelList"].ToString());


							if (TempData["SearchString"] != null && TempData["SearchString"].ToString() != "")
							{
								ViewBag.SearchString = TempData["SearchString"].ToString();
								TempData["SearchString"] = TempData["SearchString"].ToString();
								if (SelValueSch == 1)
								{
									Search += " and b.APPNO='" + TempData["SearchString"].ToString() + "'";
								}
								else if (SelValueSch == 2)
								{
									Search += " and  c.NAME like '%" + TempData["SearchString"].ToString() + "%'";
								}
								else if (SelValueSch == 3)
								{
									Search += " and c.FNAME  like '%" + TempData["SearchString"].ToString() + "%'";
								}
								else if (SelValueSch == 4)
								{
									Search += " and c.MNAME like '%" + TempData["SearchString"].ToString() + "%'";
								}
								else if (SelValueSch == 5)
								{
									Search += " and c.DOB='" + TempData["SearchString"].ToString() + "'";
								}
								else if (SelValueSch == 6)
								{
									Search += " and c.SCHL='" + TempData["SearchString"].ToString() + "'";
								}
								else if (SelValueSch == 7)
								{
									Search += " and c.OROLL='" + TempData["SearchString"].ToString() + "'";
								}
								else if (SelValueSch == 8)
								{
									Search += " and b.MOBILENO='" + TempData["SearchString"].ToString() + "'";
								}
								else if (SelValueSch == 9)
								{
									Search += " and b.AADHAR_NO='" + TempData["SearchString"].ToString() + "'";
								}
								else if (SelValueSch == 10)
								{
									Search += " and b.EMAILID='" + TempData["SearchString"].ToString() + "'";
								}

								else if (SelValueSch == 11)
								{
									Search += " and c.Regno='" + TempData["SearchString"].ToString() + "'";
								}
								else if (SelValueSch == 12)
								{
									Search += " and b.id='" + TempData["SearchString"].ToString() + "'";
								}
								else if (SelValueSch == 13)
								{
									Search += " and m.ChallanId='" + TempData["SearchString"].ToString() + "'";
								}
							}
						}

						obj.StoreAllData = new AbstractLayer.OpenDB().OpenStudentlist(Search, clas, pageIndex, 0);
						if (obj.StoreAllData == null || obj.StoreAllData.Tables.Count == 0 || obj.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.Message = "Record Not Found";
							ViewBag.TotalCount = 0;
							ViewBag.TotalCount1 = 0;
						}
						else
						{
							TempData["obj"] = obj;
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
					TempData["SelForm"] = ViewBag.SelectedForm;
					TempData["SelList"] = ViewBag.SelectedItem;
					TempData["SearchString"] = ViewBag.SearchString;
					//TempData["obj"] = obj;
					return View(obj);
				}


				return View();
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		[HttpPost]
		public ActionResult ApplicationDetails(int? page, FormCollection frm)
		{
			try
			{
				if (Session["AdminId"] == null)
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
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						ViewBag.IsView = 1;
						ViewBag.IsModiFy = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/EDITREGISTRATION")).Count();
					}
				}
				#endregion Action Assign Method

				var itemsch = new SelectList(new[]{new {ID="6",Name="SCHL"},new {ID="1",Name="By Application Number"},new{ID="2",Name="Candidate Name"},
			new{ID="3",Name="Father's Name"},new{ID="4",Name="Mother's Name"},new{ID="5",Name="DOB"},
			new{ID="7",Name="Old Roll"},new{ID="8",Name="Mobile"},new{ID="9",Name="Aadhar No"},new{ID="10",Name="Email id"},
				new{ID="11",Name="Reg. No."},new{ID="12",Name="Student Id"},new{ID="13",Name="Challan Id"},
				}, "ID", "Name", 1);

				ViewBag.MySch = itemsch.ToList();

				List<SelectListItem> forms = new List<SelectListItem>() { new SelectListItem() { Text = "Matric", Value = "M3" }, new SelectListItem() { Text = "Senior", Value = "T3" } };
				ViewBag.FormType = forms;

				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				//AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
				Printlist obj = new Printlist();
				//ViewBag.id = id;
				//Session["Id"] = id;
				string Search = "";
				string formType = (frm["SelForm"] != null) ? frm["SelForm"].ToString() : "";
				ViewBag.SelectedForm = formType;
				TempData["SelForm"] = TempData["SelList"] = TempData["SearchString"] = null;
				TempData["SelForm"] = formType;
				string dist = Session["DistAllow"].ToString();
				string clas = string.Empty;
				dist = "('" + dist.Replace(",", "','") + "')";
				if (formType.ToUpper().Trim() == "M3" || formType.ToUpper().Trim() == "T3" || formType.ToUpper().Trim() == string.Empty)
				{
					clas = (formType.Trim() == string.Empty) ? "FORM='M3' or FORM='T3'" : "FORM = '" + formType + "'";
					TempData["SelList"] = (frm["SelList"] == null || frm["SelList"].ToString() == string.Empty) ? string.Empty : frm["SelList"].ToString();
					ViewBag.SelectedItem = (frm["SelList"] == null || frm["SelList"].ToString() == string.Empty) ? string.Empty : frm["SelList"].ToString();
					ViewBag.SearchString = (frm["SearchString"] == null || frm["SearchString"].ToString() == string.Empty) ? string.Empty : frm["SearchString"].ToString();
					TempData["SearchString"] = (frm["SearchString"] == null || frm["SearchString"].ToString() == string.Empty) ? string.Empty : frm["SearchString"].ToString();

					Search = "b.homedist in " + dist + ((formType.Trim() == string.Empty) ? " and (b.FORM='M3' or b.FORM='T3')" : " and b.FORM = '" + formType + "'");
					if (frm["SelList"] != "")
					{
						int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());


						if (frm["SearchString"] != "")
						{
							if (SelValueSch == 1)
							{ Search += " and b.APPNO='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 2)
							{ Search += " and  c.NAME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 3)
							{ Search += " and c.FNAME  like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and c.MNAME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and c.DOB='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and c.SCHL='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 7)
							{
								Search += " and c.OROLL='" + TempData["SearchString"].ToString() + "'";
							}
							else if (SelValueSch == 8)
							{
								Search += " and b.MOBILENO='" + TempData["SearchString"].ToString() + "'";
							}
							else if (SelValueSch == 9)
							{
								Search += " and b.AADHAR_NO='" + TempData["SearchString"].ToString() + "'";
							}
							else if (SelValueSch == 10)
							{
								Search += " and b.EMAILID='" + TempData["SearchString"].ToString() + "'";
							}
							else if (SelValueSch == 11)
							{
								Search += " and c.Regno='" + TempData["SearchString"].ToString() + "'";
							}
							else if (SelValueSch == 12)
							{
								Search += " and b.id='" + TempData["SearchString"].ToString() + "'";
							}
							else if (SelValueSch == 13)
							{
								Search += " and m.ChallanId='" + TempData["SearchString"].ToString() + "'";
							}
						}
					}

					obj.StoreAllData = new AbstractLayer.OpenDB().OpenStudentlist(Search, clas, pageIndex, 0);
					if (obj.StoreAllData == null || obj.StoreAllData.Tables.Count == 0 || obj.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						ViewBag.TotalCount1 = 0;
					}
					else
					{
						//TempData["obj"] = obj;
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
				return View(obj);
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		[AdminLoginCheckFilter]
		public JsonResult jqCancelApplication(string cancelremarks, string appno)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
			try
			{
				string dee = "No";
				string result = "0";
				string outError = "0";
				string outstatus = "0";
				if (appno != "" && cancelremarks != "")
				{
					result = new AbstractLayer.OpenDB().CancelApplicationOpen(cancelremarks, appno, out outstatus, Convert.ToInt32(Session["AdminId"]), adminLoginSession.AdminEmployeeUserId);
					if (result == "1")
					{
						outError = result;
						dee = "Yes";
					}
					else
					{ dee = "No"; }
				}

				return Json(new { sn = dee, chid = outError }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return null;
			}
		}


		[AdminLoginCheckFilter]
		public JsonResult jqGenerateChlnOpenAdmin(float lumsumfine, string lumsumremarks, string BankCode, string appno, string gdate, string vdate)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
			try
			{
				FeeOpen _feeOpen = new FeeOpen();
				ChallanMasterModel CM = new ChallanMasterModel();
				string dee = "No";
				string outError = "0";
				int outstatus = 0;
				string Search = string.Empty;
				string UserType = "Admin";
				float fee = 0;
				DateTime dateG, dateV;
				OpenUserLogin _openUserLogin = new AbstractLayer.OpenDB().GetRecord(appno);
				if (DateTime.TryParseExact(gdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateG))
				{
					if (DateTime.TryParseExact(vdate, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateV))
					{
						if (dateV >= dateG)
						{

							_feeOpen = new AbstractLayer.OpenDB().spFeeDetailsOpen2017Admin(_openUserLogin.ID.ToString(), dateG);

							if (_feeOpen == null)
							{
								dee = "No";
							}
							else
							{
								BankCode = "203";
								ViewBag.Total = _feeOpen.TotalFee + _feeOpen.ExamTotalFee;


								CM.APPNO = appno;
								CM.FeeStudentList = appno;
								CM.SCHLREGID = _openUserLogin.ID.ToString();
								CM.SchoolCode = _openUserLogin.ID.ToString();
								CM.addfee = _feeOpen.AdmissionFee; // AdmissionFee / ADDFEE
								CM.latefee = _feeOpen.LateFee;
								CM.prosfee = _feeOpen.ProsFee;
								CM.addsubfee = _feeOpen.AddSubFee;
								CM.add_sub_count = _feeOpen.NoAddSub;
								CM.regfee = _feeOpen.RegConti;
								CM.FEE = _feeOpen.TotalFee;
								CM.TOTFEE = _feeOpen.TotalFee + _feeOpen.ExamTotalFee;

								CM.OpenExamFee = _feeOpen.ExamRegFee;
								CM.OpenLateFee = _feeOpen.ExamLateFee;
								CM.OpenTotalFee = _feeOpen.ExamTotalFee;
								CM.FEECAT = _feeOpen.FeeCat;
								CM.FEECODE = _feeOpen.FeeCode;
								CM.FEEMODE = "CASH";
								CM.BANK = "PSEB HOD";
								CM.BCODE = BankCode;
								CM.BANKCHRG = Convert.ToInt32(0);
								CM.DIST = _openUserLogin.HOMEDIST;
								CM.DISTNM = _openUserLogin.HOMEDISTNM;
								CM.LOT = 0;
								CM.DepositoryMobile = "CASH";
								CM.type = "candt";
								CM.ChallanVDateN = dateV;
								CM.ChallanGDateN = dateG;
								//DateTime CHLNVDATE2;
								//if (DateTime.TryParseExact(_feeOpen.BankLastDate.ToString("dd/MM/yyyy"), "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out CHLNVDATE2))
								//{
								//    CM.ChallanVDateN = dateV;
								//}
								//else
								//{
								//    CM.ChallanVDateN = _feeOpen.BankLastDate;
								//}
								CM.CHLNVDATE = _feeOpen.BankLastDate.ToString("dd/MM/yyyy");
								CM.LumsumFine = Convert.ToInt32(lumsumfine);
								CM.LSFRemarks = lumsumremarks;
								string SchoolMobile = "";
								string result = "0";
								FormCollection frm = new FormCollection();
								result = new AbstractLayer.OpenDB().OpenInsertPaymentForm(CM, frm, out SchoolMobile);
								if (result != "" && result != "0")
								{
									outError = result;
									dee = "Yes";
								}
								else
								{ dee = "No"; }
							}
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
				return null;
			}
		}


		public ActionResult ViewRegistration(string Id)
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}


				OpenUserLogin ol = new AbstractLayer.OpenDB().GetRecord(Id);
				if (ol.ID > 0)
				{
					ViewBag.Id = ol.ID;
					string app_id = ol.ID.ToString();
					OpenUserRegistration _openUserRegistration = new OpenUserRegistration();
					_openUserRegistration = new AbstractLayer.OpenDB().GetRegistrationRecord(app_id);

					if (_openUserRegistration == null)
					{
						_openUserRegistration = new OpenUserRegistration();
						_openUserRegistration.APPNO = app_id;
					}
					else
					{
						if (_openUserRegistration.OSESSION != null && _openUserRegistration.OSESSION != string.Empty)
						{
							string[] osession = _openUserRegistration.OSESSION.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
							ViewBag.month = osession[0];
							ViewBag.year = osession[1];
						}
					}
					_openUserRegistration.NATION = "INDIA";
					/***************** LOGIN *****/
					OpenUserLogin _openUserLogin = new AbstractLayer.OpenDB().GetLoginById(app_id);
					if (_openUserLogin.IMG_RAND == "" || _openUserLogin.IMG_RAND == null)
					{ @ViewBag.Photo = "/Images/NoPhoto.jpg"; }
					else
					{
						@ViewBag.Photo = ConfigurationManager.AppSettings["AWSURL"] + _openUserLogin.IMG_RAND.ToString();
					}
					if (_openUserLogin.IMGSIGN_RA == "" || _openUserLogin.IMGSIGN_RA == null)
					{
						@ViewBag.Sign = "/Images/NoSignature.jpg";
					}
					else
					{
						@ViewBag.Sign = ConfigurationManager.AppSettings["AWSURL"] + _openUserLogin.IMGSIGN_RA.ToString();
					}

					ViewBag.Tehsil = new AbstractLayer.OpenDB().GetStreamTehsil((_openUserLogin.HOMEDIST != null) ? _openUserLogin.HOMEDIST : string.Empty, (_openUserLogin.STREAMCODE != null) ? _openUserLogin.STREAMCODE : string.Empty).ToList().Where(s => s.Value == _openUserLogin.TEHSIL).Select(s => s.Text).FirstOrDefault();

					ViewBag.MOBILENO = _openUserLogin.MOBILENO;
					ViewBag.EMAILID = _openUserLogin.EMAILID;
					string Tehsil1 = ViewBag.Tehsil == null ? "" : ViewBag.Tehsil;
					ViewBag.ADDRESS = _openUserLogin.ADDRESS + " , " + Tehsil1 + " , " + _openUserLogin.PINCODE + " , " + _openUserLogin.HOMEDISTNM;
					ViewBag.StudyCentreDIST = _openUserLogin.HOMEDISTNM;


					/////

					ViewBag.subStatus = new AbstractLayer.OpenDB().IsUserInSubjects(_openUserLogin.ID.ToString()).ToString();
					/***************** Subjects *****/
					List<OpenUserSubjects> subjects_list = new AbstractLayer.OpenDB().GetSubjectsForUser(app_id);
					ViewBag.subjects_list = subjects_list;
					return View(_openUserRegistration);
				}
				else
				{
					ViewBag.Id = 0;
					return View();
				}
			}
			catch (Exception)
			{
				return View();
			}
			// }
		}


		[AdminLoginCheckFilter]
		public ActionResult EditRegistration(OpenUserRegistrationViewModel openUserRegistrationViewModel, string Id, string cmd)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			if (Session["AdminId"] == null || Session["AdminId"].ToString() == string.Empty)
			{
				return RedirectToAction("Index", "Admin");
			}
			else
			{
				openUserRegistrationViewModel.openUserLogin = new OpenUserLogin();
				openUserRegistrationViewModel.openUserRegistration = new OpenUserRegistration();

				ViewBag.Months = new AbstractLayer.OpenDB().GetMonths();
				ViewBag.Years = new AbstractLayer.OpenDB().GetYears();
				ViewBag.Boards = new AbstractLayer.OpenDB().GetN2Board();
				ViewBag.PhyChal = new AbstractLayer.DBClass().GetDA();
				ViewBag.Gender = new AbstractLayer.OpenDB().GetGenders();
				// ViewBag.Caste = dbClass.GetCaste();
				List<SelectListItem> casts = new AbstractLayer.DBClass().GetCaste();
				casts.RemoveAll(r => r.Text.Contains("SC("));
				ViewBag.Cast = casts;
				ViewBag.Religion = new AbstractLayer.DBClass().GetReligion();
				ViewBag.StudyMedium = new AbstractLayer.OpenDB().GetMedium();
				try
				{
					OpenUserLogin ol = new AbstractLayer.OpenDB().GetRecord(Id);
					/// openUserRegistrationViewModel.openUserLogin = ol;
					if (ol.ID > 0)
					{
						ViewBag.Form = ol.FORM.ToLower();
						if (ViewBag.Form == "m3")
						{
							ViewBag.categories = new AbstractLayer.OpenDB().GetMCategories();
						}
						else if (ViewBag.Form != null && ViewBag.Form.ToString() == "t3")
						{
							ViewBag.streams = new List<SelectListItem>() { new SelectListItem() { Text = "--Select--", Value = "" } };
							ViewBag.categories = new AbstractLayer.OpenDB().GetTCategories();
							if (ol.CATEGORY.ToUpper() == "12TH FAIL (REGULAR SCHOOL-SCIENCE GROUP)")
							{
								ViewBag.streams = new AbstractLayer.OpenDB().GetStreams_1();
							}
							else
							{
								ViewBag.streams = new AbstractLayer.OpenDB().GetStreams_2();
							}
						}

						ViewBag.CATEGORY = ol.CATEGORY;
						ViewBag.STREAM = ol.STREAMCODE;


						ViewBag.Id = ol.ID;
						string app_id = ol.ID.ToString();
						ViewBag.AppNo = ol.APPNO.ToString();
						int regStatus = new AbstractLayer.OpenDB().IsUserInReg(ol.ID.ToString());
						ViewBag.regStatus = regStatus.ToString();


						OpenUserRegistration _openUserRegistration = new OpenUserRegistration();
						_openUserRegistration = new AbstractLayer.OpenDB().GetRegistrationRecord(app_id);


						ViewBag.SCHL = _openUserRegistration.SCHL.Trim();
						//
						_openUserRegistration.CATEGORY = ol.CATEGORY;
						_openUserRegistration.STREAM = ol.STREAM;


						// chnage 2020
						//_openUserRegistration.STREAMCODE = ol.STREAMCODE;
						//_openUserRegistration.ADDRESS = ol.ADDRESS;
						//_openUserRegistration.TEHSIL = ol.TEHSIL;
						//_openUserRegistration.PINCODE = ol.PINCODE;
						//_openUserRegistration.HOMEDIST = ol.HOMEDIST;
						//_openUserRegistration.HOMEDISTNM = ol.HOMEDISTNM;
						//_openUserRegistration.EMAILID = ol.EMAILID;
						//_openUserRegistration.MOBILENO = ol.MOBILENO;
						//



						ViewBag.Dist = new AbstractLayer.OpenDB().GetDistrict();
						if (string.IsNullOrEmpty(_openUserRegistration.DIST))
						{
							_openUserRegistration.DIST = ol.HOMEDIST;
						}
						ViewBag.studyCenters = new AbstractLayer.OpenDB().GetStudyCenters(_openUserRegistration.DIST, ol.STREAM);


						if (_openUserRegistration == null)
						{
							_openUserRegistration = new OpenUserRegistration();
							_openUserRegistration.APPNO = app_id;
						}
						else
						{
							if (_openUserRegistration.OSESSION != null && _openUserRegistration.OSESSION != string.Empty)
							{
								string[] osession = _openUserRegistration.OSESSION.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
								if (osession.Count() == 2)
								{
									ViewBag.month = osession[0];
									ViewBag.year = osession[1];
								}

							}

							if (_openUserRegistration.AppearingYear != null && _openUserRegistration.AppearingYear.Trim() != string.Empty)
							{
								string[] oap = _openUserRegistration.AppearingYear.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
								ViewBag.AppearingMonth = oap[0];
								ViewBag.AppearingYr = oap[1];
							}
						}
						_openUserRegistration.NATION = "INDIA";
						/***************** LOGIN *****/
						OpenUserLogin _openUserLogin = new AbstractLayer.OpenDB().GetLoginById(app_id);
						if (_openUserLogin.IMG_RAND == "" || _openUserLogin.IMG_RAND == null)
						{ @ViewBag.Photo = "~/Images/NOSignature.jpg"; }
						else
						{
							@ViewBag.Photo = ConfigurationManager.AppSettings["AWSURL"] + _openUserLogin.IMG_RAND.ToString();
						}
						if (_openUserLogin.IMGSIGN_RA == "" || _openUserLogin.IMGSIGN_RA == null)
						{
							@ViewBag.Sign = "~/Images/NOSignature.jpg";
						}
						else
						{
							@ViewBag.Sign = ConfigurationManager.AppSettings["AWSURL"] + _openUserLogin.IMGSIGN_RA.ToString();
						}


						if (_openUserLogin.CATEGORY.ToLower().Contains("direct"))
						{
							ViewBag.disableBoard = "true";
						}
						else
						{
							ViewBag.disableBoard = "false";
						}

						//
						ViewBag.Tehsils = new AbstractLayer.OpenDB().GetStreamTehsil((_openUserLogin.HOMEDIST != null) ? _openUserLogin.HOMEDIST : string.Empty, (_openUserLogin.STREAMCODE != null) ? _openUserLogin.STREAMCODE : string.Empty);
						ViewBag.selTehsil = new AbstractLayer.OpenDB().GetStreamTehsil((_openUserLogin.HOMEDIST != null) ? _openUserLogin.HOMEDIST : string.Empty, (_openUserLogin.STREAMCODE != null) ? _openUserLogin.STREAMCODE : string.Empty).ToList().Where(s => s.Value == _openUserLogin.TEHSIL).Select(s => s.Text).FirstOrDefault();

						ViewBag.MOBILENO = _openUserLogin.MOBILENO;
						ViewBag.EMAILID = _openUserLogin.EMAILID;
						ViewBag.ADDRESS = _openUserLogin.ADDRESS + " , " + ViewBag.selTehsil == null ? "" : ViewBag.selTehsil + " , " + _openUserLogin.PINCODE + " , " + _openUserLogin.HOMEDISTNM;
						ViewBag.StudyCentreDIST = _openUserLogin.HOMEDISTNM;
						//
						ViewBag.subStatus = new AbstractLayer.OpenDB().IsUserInSubjects(_openUserLogin.ID.ToString()).ToString();
						/***************** Subjects *****/
						List<OpenUserSubjects> subjects_list = new AbstractLayer.OpenDB().GetSubjectsForUser(app_id);
						ViewBag.subjects_list = subjects_list;
						//chln
						string ChallanId = "";
						ViewBag.payStatus = new AbstractLayer.OpenDB().IsUserInChallan(_openUserLogin.APPNO.ToString(), out ChallanId).ToString();
						if (ChallanId.Length > 12)
						{
							ViewBag.ChallanId = ChallanId;
							int x = new AbstractLayer.OpenDB().IsChallanVerified(_openUserLogin.APPNO.ToString(), ChallanId);
							ViewBag.payVerify = x.ToString();
						}

						openUserRegistrationViewModel.openUserLogin = ol;
						openUserRegistrationViewModel.openUserRegistration = _openUserRegistration;
						return View(openUserRegistrationViewModel);
						// return View(_openUserRegistration);
					}
					else
					{
						ViewBag.Id = 0;
						return View();
					}
				}
				catch (Exception ex)
				{
					return View();
				}
			}
		}

		[HttpPost]
		public ActionResult EditRegistration(string Id, string cmd, FormCollection fc, OpenUserRegistrationViewModel openUserRegistrationViewModel, HttpPostedFileBase Photo, HttpPostedFileBase Sign)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			OpenUserRegistration _openUserRegistration = new OpenUserRegistration();
			_openUserRegistration = openUserRegistrationViewModel.openUserRegistration;
			if (Session["AdminId"] == null || Session["AdminId"].ToString() == string.Empty)
			{
				return RedirectToAction("Index", "Admin");
			}
			else
			{
				OpenUserLogin ol = new AbstractLayer.OpenDB().GetRecord(Id);
				ViewBag.Id = ol.ID;
				string app_id = ol.ID.ToString();
				ViewBag.AppNo = ol.APPNO.ToString();

				ViewBag.Form = ol.FORM.ToLower();
				if (ViewBag.Form == "m3")
				{
					ViewBag.categories = new AbstractLayer.OpenDB().GetMCategories();
				}
				else if (ViewBag.Form != null && ViewBag.Form.ToString() == "t3")
				{
					ViewBag.streams = new List<SelectListItem>() { new SelectListItem() { Text = "--Select--", Value = "" } };
					ViewBag.categories = new AbstractLayer.OpenDB().GetTCategories();
					if (ol.CATEGORY.ToUpper() == "12TH FAIL (REGULAR SCHOOL-SCIENCE GROUP)")
					{
						ViewBag.streams = new AbstractLayer.OpenDB().GetStreams_1();
					}
					else
					{
						ViewBag.streams = new AbstractLayer.OpenDB().GetStreams_2();
					}
				}



				ViewBag.Months = new AbstractLayer.OpenDB().GetMonths();
				ViewBag.Years = new AbstractLayer.OpenDB().GetYears();
				ViewBag.Boards = new AbstractLayer.DBClass().GetN2Board();
				ViewBag.PhyChal = new AbstractLayer.DBClass().GetDA();
				ViewBag.Gender = new AbstractLayer.OpenDB().GetGenders();
				// ViewBag.Caste = dbClass.GetCaste();
				List<SelectListItem> casts = new AbstractLayer.DBClass().GetCaste();
				casts.RemoveAll(r => r.Text.Contains("SC("));
				ViewBag.Cast = casts;
				ViewBag.Religion = new AbstractLayer.DBClass().GetReligion();
				ViewBag.StudyMedium = new AbstractLayer.OpenDB().GetMedium();
				ViewBag.Dist = new AbstractLayer.OpenDB().GetDistrict();
				if (ol.CATEGORY.ToLower().Contains("direct"))
				{
					ViewBag.disableBoard = "true";
				}
				else
				{
					ViewBag.disableBoard = "false";
				}




				int regStatus = new AbstractLayer.OpenDB().IsUserInReg(ol.ID.ToString());
				ViewBag.regStatus = regStatus.ToString();
				OpenUserRegistration _oUserRegistration = new OpenUserRegistration();
				if (regStatus == 1)
				{


					_oUserRegistration = new AbstractLayer.OpenDB().GetRegistrationRecord(_openUserRegistration.APPNO);
					if (_openUserRegistration.SCHL == null)
					{ ViewBag.SCHL = ""; }
					else
					{
						ViewBag.SCHL = _openUserRegistration.SCHL.Trim();
					}

					ViewBag.Dist = new AbstractLayer.OpenDB().GetDistrict();
					if (string.IsNullOrEmpty(_openUserRegistration.DIST))
					{
						_openUserRegistration.DIST = ol.HOMEDIST;
					}
					ViewBag.studyCenters = new AbstractLayer.OpenDB().GetStudyCenters(_openUserRegistration.DIST, ol.STREAM);
				}
				ViewBag.CATEGORY = ol.CATEGORY;
				ViewBag.STREAM = ol.STREAMCODE;
				//
				OpenUserLogin _openUserLogin = new AbstractLayer.OpenDB().GetRecord(Id);
				ViewBag.Tehsils = new AbstractLayer.OpenDB().GetStreamTehsil((_openUserLogin.HOMEDIST != null) ? _openUserLogin.HOMEDIST : string.Empty, (_openUserLogin.STREAMCODE != null) ? _openUserLogin.STREAMCODE : string.Empty);
				ViewBag.selTehsil = new AbstractLayer.OpenDB().GetStreamTehsil((_openUserLogin.HOMEDIST != null) ? _openUserLogin.HOMEDIST : string.Empty, (_openUserLogin.STREAMCODE != null) ? _openUserLogin.STREAMCODE : string.Empty).ToList().Where(s => s.Value == _openUserLogin.TEHSIL).Select(s => s.Text).FirstOrDefault();

				ViewBag.MOBILENO = _openUserLogin.MOBILENO;
				ViewBag.EMAILID = _openUserLogin.EMAILID;
				ViewBag.ADDRESS = _openUserLogin.ADDRESS + " , " + ViewBag.selTehsil == null ? "" : ViewBag.selTehsil + " , " + _openUserLogin.PINCODE + " , " + _openUserLogin.HOMEDISTNM;
				ViewBag.StudyCentreDIST = _openUserLogin.HOMEDISTNM;
				//


				if (cmd != null)
				{
					if (cmd.ToLower().Contains("login"))
					{


						//

						#region Login
						if (_openUserLogin.CLASS == "12")
						{
							List<SelectListItem> streams = new AbstractLayer.OpenDB().GetStreams_1();
							//if (fc["STREAM"].ToString().Contains(','))
							if (openUserRegistrationViewModel.openUserLogin.STREAMCODE.ToString().Contains(','))
							{
								//_openUserLogin.STREAMCODE = fc["STREAM"].ToString().Split(',')[0];
								_openUserLogin.STREAMCODE = openUserRegistrationViewModel.openUserLogin.STREAMCODE;
								_openUserLogin.STREAM = streams.Find(f => f.Value == _openUserLogin.STREAMCODE).Text;
							}
							else
							{
								//_openUserLogin.STREAMCODE = fc["STREAM"].ToString().Length > 1 ? fc["STREAM"].ToString().Substring(0, 1) : fc["STREAM"].ToString();
								_openUserLogin.STREAMCODE = openUserRegistrationViewModel.openUserLogin.STREAMCODE;
								_openUserLogin.STREAM = streams.Find(f => f.Value == _openUserLogin.STREAMCODE).Text;
							}


						}
						else
						{
							_openUserLogin.STREAM = "GENERAL";
							_openUserLogin.STREAMCODE = "G";
						}
						////
						_openUserLogin.ADDRESS = openUserRegistrationViewModel.openUserLogin.ADDRESS;
						_openUserLogin.LANDMARK = openUserRegistrationViewModel.openUserLogin.LANDMARK;
						_openUserLogin.PINCODE = openUserRegistrationViewModel.openUserLogin.PINCODE;
						_openUserLogin.TEHSIL = openUserRegistrationViewModel.openUserLogin.TEHSIL;
						_openUserLogin.HOMEDIST = openUserRegistrationViewModel.openUserLogin.HOMEDIST;
						_openUserLogin.HOMEDISTNM = new AbstractLayer.OpenDB().GetDistrict().Find(s => s.Value == openUserRegistrationViewModel.openUserLogin.HOMEDIST).Text;
						_openUserLogin.EMAILID = openUserRegistrationViewModel.openUserLogin.EMAILID;
						_openUserLogin.MOBILENO = openUserRegistrationViewModel.openUserLogin.MOBILENO;
						_openUserLogin.AADHAR_NO = openUserRegistrationViewModel.openUserLogin.AADHAR_NO;
						_openUserLogin.CATEGORY = openUserRegistrationViewModel.openUserRegistration.CAT;
						_openUserLogin.DOB = openUserRegistrationViewModel.openUserLogin.DOB;
						#endregion Login



						// Check Email / mobile / Aadhar duplicacy

						string EMAILID1 = _openUserLogin.EMAILID;
						string MOBILENO1 = _openUserLogin.MOBILENO;
						string AADHAR_NO1 = _openUserLogin.AADHAR_NO;


						string Search = "appno !='" + _openUserLogin.APPNO + "'";

						DataSet dschk = new AbstractLayer.OpenDB().OpenStudentlist(Search, "", 0, 4);
						if (dschk.Tables.Count > 0)
						{
							if (dschk.Tables[0].Rows.Count > 0)
							{
								//  bool dupEmail = dschk.Tables[0].AsEnumerable().Any(row => EMAILID1.ToUpper() == row.Field<string>("EMAILID").ToUpper());
								bool dupMobile = dschk.Tables[0].AsEnumerable().Any(row => MOBILENO1.ToUpper() == row.Field<string>("MOBILENO").ToUpper());
								bool dupAadhar = dschk.Tables[0].AsEnumerable().Any(row => AADHAR_NO1.ToUpper() == row.Field<string>("AADHAR_NO").ToUpper());

								if (dupMobile == true || dupAadhar == true)
								{
									int flag = 0;
									// if (dupEmail && flag == 0)
									//{
									//    flag = 1;
									//    ViewBag.error = "Duplicate Email Id";
									//    if (dupMobile)
									//    {
									//        flag = 2;
									//        ViewBag.error += " and Mobile Number";
									//    }
									//    if (dupAadhar)
									//    {
									//        flag = 3;
									//        ViewBag.flag = 3;
									//        ViewBag.error += " and Aadhar Number";
									//    }
									//}
									if (dupMobile && flag == 0)
									{
										flag = 2;
										ViewBag.error = "Duplicate Mobile Number";
										if (dupAadhar)
										{
											flag = 3;
											ViewBag.flag = 3;
											ViewBag.error += " and Aadhar Number";
										}
									}
									if (dupAadhar && flag == 0)
									{
										flag = 3;
										ViewBag.flag = 3;
										ViewBag.error = "Duplicate Aadhar Number";
									}


									ViewBag.val = "";
									return View();
								}
							}
						}
						//

						int status = new AbstractLayer.OpenDB().UpdateLoginUser(_openUserLogin);
						ViewBag.val = status.ToString();
						if (status == 1)
						{ ViewBag.val = openUserRegistrationViewModel.openUserLogin.APPNO; }

						return View(openUserRegistrationViewModel);
						// return View(_oUserRegistration);
					}

				}


				ViewBag.subStatus = new AbstractLayer.OpenDB().IsUserInSubjects(ol.ID.ToString()).ToString();
				/***************** Subjects *****/
				List<OpenUserSubjects> subjects_list = new AbstractLayer.OpenDB().GetSubjectsForUser(ol.ID.ToString());
				ViewBag.subjects_list = subjects_list;

				//chln
				string ChallanId = "";
				ViewBag.payStatus = new AbstractLayer.OpenDB().IsUserInChallan(ol.APPNO.ToString(), out ChallanId).ToString();
				if (ChallanId.Length > 12)
				{
					ViewBag.ChallanId = ChallanId;
					int x = new AbstractLayer.OpenDB().IsChallanVerified(ol.APPNO.ToString(), ChallanId);
					ViewBag.payVerify = x.ToString();
				}

				if (regStatus == 1)
				{
					////OpenUserLogin ol = new AbstractLayer.OpenDB().GetLoginById(_openUserRegistration.APPNO);                   
					// Update Data Reg
					string imgSign, imgPhoto;
					imgSign = imgPhoto = string.Empty;
					if (Photo != null)
					{

						var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/Open2022/Photo"), openUserRegistrationViewModel.openUserRegistration.APPNO.ToString() + "_P.jpg");
						string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/Open2022/Photo"));
						if (!Directory.Exists(FilepathExist))
						{
							Directory.CreateDirectory(FilepathExist);
						}
						//Photo.SaveAs(path);
						imgPhoto = "allfiles/Upload2023/Open2022/Photo/" + openUserRegistrationViewModel.openUserRegistration.APPNO.ToString() + "_P.jpg";

						string Orgfile = openUserRegistrationViewModel.openUserRegistration.APPNO.ToString() + "_P.jpg";

						using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
						{
							using (var newMemoryStream = new MemoryStream())
							{
								var uploadRequest = new TransferUtilityUploadRequest
								{
									InputStream = Photo.InputStream,
									Key = string.Format("allfiles/Upload2023/Open2022/Photo/{0}", Orgfile),

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
						imgPhoto = ol.IMG_RAND;
					}
					if (Sign != null)
					{
						var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/Open2022/Sign"), openUserRegistrationViewModel.openUserRegistration.APPNO.ToString() + "_S.jpg");
						string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/Open2022/Sign"));
						if (!Directory.Exists(FilepathExist))
						{
							Directory.CreateDirectory(FilepathExist);
						}
						//Sign.SaveAs(path);
						imgSign = "allfiles/Upload2023/Open2022/Sign/" + openUserRegistrationViewModel.openUserRegistration.APPNO.ToString() + "_S.jpg";
						string Orgfile = openUserRegistrationViewModel.openUserRegistration.APPNO.ToString() + "_S.jpg";
						using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
						{
							using (var newMemoryStream = new MemoryStream())
							{
								var uploadRequest = new TransferUtilityUploadRequest
								{
									InputStream = Sign.InputStream,
									Key = string.Format("allfiles/Upload2023/Open2022/Sign/{0}", Orgfile),

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
						imgSign = ol.IMGSIGN_RA;
					}

					#region Login
					if (_openUserLogin.CLASS == "12")
					{
						List<SelectListItem> streams = new AbstractLayer.OpenDB().GetStreams_1();
						if (openUserRegistrationViewModel.openUserLogin.STREAMCODE.ToString().Contains(','))
						{
							//_openUserLogin.STREAMCODE = fc["STREAM"].ToString().Split(',')[0];
							_openUserLogin.STREAMCODE = openUserRegistrationViewModel.openUserLogin.STREAMCODE;
							_openUserLogin.STREAM = streams.Find(f => f.Value == _openUserLogin.STREAMCODE).Text;
						}
						else
						{
							if (_openUserLogin.STREAMCODE.ToString().ToUpper() == "IG")
							{
								_openUserLogin.STREAMCODE = "IG";
								_openUserLogin.STREAMCODE = "INTIGRATED GROUP";
							}
							else
							{
								_openUserLogin.STREAMCODE = openUserRegistrationViewModel.openUserLogin.STREAMCODE;
								_openUserLogin.STREAM = streams.Find(f => f.Text == _openUserLogin.STREAMCODE).Text;
							}
						}
					}
					else
					{
						_openUserLogin.STREAM = "GENERAL";
						_openUserLogin.STREAMCODE = "G";
					}

					if (openUserRegistrationViewModel.openUserLogin.AADHAR_NO != _openUserLogin.AADHAR_NO)
					{
						_openUserRegistration.AADHAR_NO = _openUserLogin.AADHAR_NO = openUserRegistrationViewModel.openUserLogin.AADHAR_NO;
					}
					if (openUserRegistrationViewModel.openUserLogin.EMAILID != _openUserLogin.EMAILID)
					{
						_openUserLogin.EMAILID = openUserRegistrationViewModel.openUserLogin.EMAILID;
					}
					if (openUserRegistrationViewModel.openUserLogin.MOBILENO != _openUserLogin.MOBILENO)
					{
						_openUserLogin.MOBILENO = openUserRegistrationViewModel.openUserLogin.MOBILENO;
					}
					if (openUserRegistrationViewModel.openUserLogin.DOB != _openUserLogin.DOB)
					{
						_openUserRegistration.DOB = _openUserLogin.DOB = openUserRegistrationViewModel.openUserLogin.DOB;
					}
					_openUserLogin.ADDRESS = openUserRegistrationViewModel.openUserLogin.ADDRESS;
					_openUserLogin.LANDMARK = openUserRegistrationViewModel.openUserLogin.LANDMARK;
					_openUserLogin.PINCODE = openUserRegistrationViewModel.openUserLogin.PINCODE;
					_openUserLogin.TEHSIL = openUserRegistrationViewModel.openUserLogin.TEHSIL;
					_openUserLogin.HOMEDIST = openUserRegistrationViewModel.openUserLogin.HOMEDIST;
					_openUserLogin.HOMEDISTNM = new AbstractLayer.OpenDB().GetDistrict().Find(s => s.Value == openUserRegistrationViewModel.openUserLogin.HOMEDIST).Text;


					#endregion Login
					//UpdateOpenRegistrationByAdmin
					int status = new AbstractLayer.OpenDB().UpdateOpenRegistrationByAdmin(Session["AdminId"].ToString(), openUserRegistrationViewModel.openUserRegistration, imgSign, imgPhoto, _openUserLogin, adminLoginSession.AdminEmployeeUserId);
					if (status == 1)
					{
						ViewData["result"] = "1";
						ViewBag.updateStatus = status.ToString();
						if (_oUserRegistration.SCHL.Trim() != "")
						{
							if (fc["StudyCenter_1"].ToString().Trim() != "")
							{

								List<SelectListItem> studyCenters = new AbstractLayer.OpenDB().GetStudyCenters(_openUserRegistration.DIST, ol.STREAM);
								if (_oUserRegistration.DIST != fc["District"].Trim())
								{
									_oUserRegistration.DIST = fc["District"].Trim();
								}
								if (_oUserRegistration.SCHL1 != fc["StudyCenter_1"].Trim())
								{
									_oUserRegistration.SCHL = _oUserRegistration.SCHL1 = fc["StudyCenter_1"].Trim();
								}
								if (_oUserRegistration.SCHL2 != fc["StudyCenter_2"].Trim())
								{
									_oUserRegistration.SCHL2 = fc["StudyCenter_2"].Trim();
								}
								if (_oUserRegistration.SCHL3 != fc["StudyCenter_3"].Trim())
								{
									_oUserRegistration.SCHL3 = fc["StudyCenter_3"].Trim();
								}
								studyCenters.Remove(studyCenters.Find(f => f.Value == ""));
								switch (studyCenters.Count)
								{
									case 1:
										{
											_openUserRegistration.SCHL2 = _openUserRegistration.SCHL3 = "";
											if (_oUserRegistration.SCHL1 == _oUserRegistration.SCHL2)
											{
												ViewBag.updateStatus = "9";
												_oUserRegistration = new AbstractLayer.OpenDB().GetRegistrationRecord(_openUserRegistration.APPNO);
												// return View(_oUserRegistration);
											}
											break;
										}
									case 2:
										{
											_openUserRegistration.SCHL3 = "";
											if ((_oUserRegistration.SCHL1 == _oUserRegistration.SCHL2) || _oUserRegistration.SCHL2 == _oUserRegistration.SCHL3)
											{
												ViewBag.updateStatus = "9";
												_oUserRegistration = new AbstractLayer.OpenDB().GetRegistrationRecord(_openUserRegistration.APPNO);
												//  return View(_oUserRegistration);
											}
											break;
										}
									default:
										{
											if ((_oUserRegistration.SCHL1 == _oUserRegistration.SCHL2) || (_oUserRegistration.SCHL2) == _oUserRegistration.SCHL3 || _oUserRegistration.SCHL3 == _oUserRegistration.SCHL1)
											{
												ViewBag.updateStatus = "9";
												_oUserRegistration = new AbstractLayer.OpenDB().GetRegistrationRecord(_openUserRegistration.APPNO);
												// return View(_oUserRegistration);
											}
											break;
										}
								}
								int x = new AbstractLayer.OpenDB().UpdateStudyCenter(_oUserRegistration);
								ViewBag.updateStatus = x.ToString();
							}
						}
					}


				}
				return View(openUserRegistrationViewModel);
				// return RedirectToAction("ApplicationDetails", Session["AdminId"].ToString());
			}

		}

		public ActionResult AdmissionForm(string Id)
		{
			try
			{
				FeeOpen _feeOpen = new FeeOpen();
				if (Id == null || Id == "0" || Id == "")
				{
					return RedirectToAction("Index", "Admin");
				}
				string AppNo = Id;
				string SCHL = Convert.ToString(Session["AdminId"]);
				DataSet ds = new AbstractLayer.OpenDB().GetApplicationFormById(AppNo);
				_feeOpen.StoreAllData = ds;
				if (_feeOpen.StoreAllData == null || _feeOpen.StoreAllData.Tables.Count == 0 || _feeOpen.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.TotalCount = 0;
					return View(_feeOpen);
				}
				else
				{
					ViewBag.TotalCount = _feeOpen.StoreAllData.Tables[0].Rows.Count;
					ViewBag.FORM = _feeOpen.StoreAllData.Tables[0].Rows[0]["FORM"].ToString();
					ViewBag.FORMNAME = _feeOpen.StoreAllData.Tables[0].Rows[0]["FORMNAME"].ToString();
					ViewBag.APPNO = _feeOpen.StoreAllData.Tables[0].Rows[0]["APPNO"].ToString();
					ViewBag.ID = _feeOpen.StoreAllData.Tables[0].Rows[0]["ID"].ToString();

					List<OpenUserSubjects> subjects_list = new AbstractLayer.OpenDB().GetSubjectsForUser(_feeOpen.StoreAllData.Tables[0].Rows[0]["ID"].ToString());
					ViewBag.subjects_list = subjects_list;
					return View(_feeOpen);
				}
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Admin");
			}
		}

		public JsonResult GetStudyCenterList(string dist, string appno)
		{
			List<SelectListItem> StudyCenterList = new List<SelectListItem>();
			OpenUserLogin ol = new AbstractLayer.OpenDB().GetLoginById(appno);
			StudyCenterList = new AbstractLayer.OpenDB().GetStudyCenters(dist, ol.STREAM);

			return Json(StudyCenterList);

		}

		#endregion  Open ApplicationDetails

		#region Menu Master   

		public MultiSelectList MenuSessionList(string sel)
		{
			var SessionList = objCommon.GetSessionAdmin().ToList().Take(4).Select(c => new
			{
				Text = c.Text,
				Value = c.Value.Substring(0, 4)
			}).OrderByDescending(s => s.Value).ToList();
			if (sel == "")
			{ return new MultiSelectList(SessionList, "Value", "Text"); }
			else
			{
				int[] myArray1 = AbstractLayer.StaticDB.StringToIntArray(sel, ',');
				return new MultiSelectList(SessionList, "Value", "Text", myArray1);
			}
		}

		public ActionResult MenuMaster(int id = 0)
		{
			List<SelectListItem> menuList = new List<SelectListItem>();
			ViewBag.Id = id;
			ViewBag.SelMenu = menuList;
			ViewBag.SelectedSelMenu = 0;
			SiteMenu oModel = new SiteMenu();
			// oModel.MenuID = id;
			if (Session["AdminType"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			else if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
			{
				string AdminType = Session["AdminType"].ToString();

				//var itemyear = new MultiSelectList(new[] { new { ID = "2017", Name = "2017" }, new { ID = "2016", Name = "2016" },
				//     new { ID = "2015", Name = "2015" }, new { ID = "2014", Name = "2014" }, }, "ID", "Name");
				//ViewBag.MySession = itemyear.ToList();

				ViewBag.MySession = MenuSessionList("");



				//Parent
				var itemParent = new SelectList(new[] { new { ID = "1", Name = "Menu" }, new { ID = "2", Name = "SubMenu" }, new { ID = "3", Name = "Action" }, }, "ID", "Name", 1);
				ViewBag.Parent = itemParent.ToList();


				if (id == 0)
				{
					DataSet result = objDB.GetAllMenu(0);
					if (result == null)
					{
						return RedirectToAction("Index", "Admin");
					}
					else if (result.Tables[0].Rows.Count > 0)
					{
						oModel.StoreAllData = result;
						ViewBag.TotalCount = oModel.StoreAllData.Tables[0].Rows.Count;
					}
				}
				else
				{
					int outstatus = 0;
					DataSet result = objDB.ListingMenu(1, id, out outstatus);
					if (result == null)
					{
						return RedirectToAction("Index", "Admin");
					}
					else if (result.Tables[0].Rows.Count > 0)
					{
						oModel.StoreAllData = result;
						ViewBag.TotalCount = oModel.StoreAllData.Tables[0].Rows.Count;
						oModel.MenuName = oModel.StoreAllData.Tables[0].Rows[0]["MenuName"].ToString();
						oModel.MenuUrl = oModel.StoreAllData.Tables[0].Rows[0]["MenuUrl"].ToString();
						ViewBag.SelectedParent = oModel.StoreAllData.Tables[0].Rows[0]["SelRole"].ToString();
						ViewBag.SelectedSelMenu = oModel.StoreAllData.Tables[0].Rows[0]["ParentMenuId"].ToString();
						ViewBag.MySession = MenuSessionList(oModel.StoreAllData.Tables[0].Rows[0]["AssignYear"].ToString());

						if (ViewBag.SelectedParent == "1")
						{ }
						else
						{
							DataSet ds = objDB.GetAllMenu(Convert.ToInt32(oModel.StoreAllData.Tables[0].Rows[0]["SelRole"].ToString()));
							ViewBag.SelMenu = ds.Tables[0];

							// menuList.Add(new SelectListItem { Text = "---Select---", Value = "0" });
							foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
							{
								if (dr["MenuId"].ToString() == oModel.StoreAllData.Tables[0].Rows[0]["ParentMenuId"].ToString())
								{
									menuList.Add(new SelectListItem { Text = @dr["MenuName"].ToString(), Value = @dr["MenuId"].ToString(), Selected = true });
								}
								else
								{
									menuList.Add(new SelectListItem { Text = @dr["MenuName"].ToString(), Value = @dr["MenuId"].ToString() });
								}
							}
							ViewBag.SelMenu = menuList;
						}
					}
				}
				return View(oModel);
			}
			else
			{
				return RedirectToAction("Index", "Admin");
			}

		}

		[HttpPost]
		public ActionResult MenuMaster(SiteMenu oModel, FormCollection frm, int id = 0)
		{

			ViewBag.MySession = MenuSessionList("");

			//Parent
			var itemParent = new SelectList(new[] { new { ID = "1", Name = "Menu" }, new { ID = "2", Name = "SubMenu" }, new { ID = "3", Name = "Action" }, }, "ID", "Name", 1);
			ViewBag.Parent = itemParent.ToList();


			string SelectedSession = "";
			// int id = oModel.MenuID ;
			ViewBag.Id = id;
			List<SelectListItem> menuList = new List<SelectListItem>();
			ViewBag.SelMenu = menuList;
			ViewBag.SelectedSelMenu = 0;
			if (Session["AdminType"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			else if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
			{

				if (frm["SelectedSession"] == "" || frm["SelectedSession"] == null)
				{
					ViewData["Result"] = 20;
					return View(oModel);
				}
				else
				{ SelectedSession = frm["SelectedSession"].ToString(); }

				string AdminType = Session["AdminType"].ToString();


				if (id == 0)
				{

					int IsParent = 0, ParentMenuId = 0, IsMenu = 1;
					if (frm["Parent"] != "")
					{
						IsParent = Convert.ToInt32(frm["Parent"]);
						if (IsParent == 1)
						{ IsParent = 0; IsMenu = 1; }
						else if (IsParent == 2)
						{ IsMenu = 1; }
						else if (IsParent == 3)
						{ IsMenu = 0; }
					}
					if (frm["SelMenu"] != "")
					{ ParentMenuId = Convert.ToInt32(frm["SelMenu"]); }

					if (IsParent != 0 && ParentMenuId == 0)
					{
						ViewData["Result"] = -1;
					}
					else
					{
						// string result = "";
						string result = objDB.CreateMenu(oModel, IsParent, ParentMenuId, IsMenu, SelectedSession);
						if (Convert.ToInt32(result) > 0)
						{
							ViewData["Result"] = 1;
						}
						else
						{
							ViewData["Result"] = 0;
						}
					}
				}
				else
				{
					oModel.MenuID = id;
					int ParentMenuId = 0;
					if (frm["SelMenu"] != "")
					{ ParentMenuId = Convert.ToInt32(frm["SelMenu"]); }
					int OutStatus = 0;
					DataSet result = objDB.UpdateMenu(oModel, ParentMenuId, out OutStatus, SelectedSession); //UpdateMenu(SiteMenu model, int ParentMenuId, out int OutStatus)
					if (OutStatus > 0)
					{
						ViewData["Result"] = 11;
					}
					else
					{
						ViewData["Result"] = 10;
					}
				}

				DataSet ds = objDB.GetAllMenu(0);
				if (ds == null)
				{
					//return RedirectToAction("Index", "Admin");
				}
				else if (ds.Tables[0].Rows.Count > 0)
				{
					oModel.StoreAllData = ds;
					ViewBag.TotalCount = oModel.StoreAllData.Tables[0].Rows.Count;
				}

				return View(oModel);
			}
			else
			{
				return RedirectToAction("Index", "Admin");
			}

		}


		public JsonResult GetMenu(int Parent) // Calling on http post (on Submit)
		{

			DataSet ds = objDB.GetAllMenu(Parent);
			ViewBag.SelMenu = ds.Tables[0];
			List<SelectListItem> menuList = new List<SelectListItem>();
			menuList.Add(new SelectListItem { Text = "---Select---", Value = "0" });
			foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
			{
				menuList.Add(new SelectListItem { Text = @dr["MenuName"].ToString(), Value = @dr["MenuId"].ToString() });
			}
			ViewBag.SelMenu = menuList;
			return Json(menuList);
		}

		public ActionResult UpdateMenuStatus(int id)
		{
			try
			{
				if (Session["AdminType"] == null)
				{ return RedirectToAction("Index", "Admin"); }
				else if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{
					string AdminType = Session["AdminType"].ToString();
					int outstatus = 0;
					DataSet result = objDB.ListingMenu(0, id, out outstatus);
					return RedirectToAction("MenuMaster", "Admin");
				}
				else
				{
					return RedirectToAction("Index", "Admin");
				}
			}
			catch (Exception)
			{
				return RedirectToAction("MenuMaster", "Admin");
			}
		}





		#endregion Menu Master    

		//
		#region Admin User Master

		public MultiSelectList GetAllPSEBCLASSList(string sel)
		{
			var SessionList = objCommon.GetAllPSEBCLASS().ToList().Select(c => new
			{
				Text = c.Text,
				Value = c.Value
			}).OrderBy(s => s.Value).ToList();
			if (sel == "")
			{ return new MultiSelectList(SessionList, "Value", "Text"); }
			else
			{
				int[] myArray1 = AbstractLayer.StaticDB.StringToIntArray(sel, ',');
				return new MultiSelectList(SessionList, "Value", "Text", myArray1);
			}
		}



		public ActionResult CreateUser()
		{

			if (Session["AdminType"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			else if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
			{
				string AdminType = Session["AdminType"].ToString();

				//GetAllPSEBCLASS
				ViewBag.GetAllPSEBCLASS = GetAllPSEBCLASSList("");

				//GetAll District
				List<SelectListItem> DistList = new List<SelectListItem>();
				DataSet ds = objCommon.Fll_Dist_Details();
				foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
				{
					DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
				}
				aum.DistList = DistList;

				//Status
				//var itemStatus = new SelectList(new[] { new { ID = "1", Name = "ACTIVE" }, new { ID = "0", Name = "IN ACTIVE" }, }, "ID", "Name", 1);
				//ViewBag.Status = itemStatus.ToList();

				// End
				//Branch
				List<SelectListItem> allBranch = new List<SelectListItem>();
				foreach (System.Data.DataRow dr in ds.Tables[2].Rows)
				{
					allBranch.Add(new SelectListItem { Text = @dr["BranchName"].ToString(), Value = @dr["BranchName"].ToString() });

				}
				ViewBag.Branch = allBranch;

				// All Pages List               
				List<SiteMenu> all = new List<SiteMenu>();
				foreach (System.Data.DataRow dr in ds.Tables[1].Rows)
				{
					//all.Add(new SiteMenu { MenuID = Convert.ToInt32(@dr["MenuID"]), MenuName = @dr["MenuName"].ToString(), MenuUrl = @dr["MenuUrl"].ToString(), ParentMenuID = Convert.ToInt32(@dr["ParentMenuID"]) });
					all.Add(new SiteMenu { MenuID = Convert.ToInt32(@dr["MenuID"]), MenuName = @dr["MenuName"].ToString(), MenuUrl = @dr["MenuUrl"].ToString(), ParentMenuID = Convert.ToInt32(@dr["ParentMenuID"]), IsMenu = Convert.ToInt32(@dr["IsMenu"]) });

				}
				aum.SiteMenuModel = all;


				//GetAll Set
				List<SelectListItem> SetList = new List<SelectListItem>();
				foreach (System.Data.DataRow dr in ds.Tables[4].Rows)
				{
					SetList.Add(new SelectListItem { Text = @dr["AdminSet"].ToString(), Value = @dr["AdminSet"].ToString() });
				}
				aum.SetList = SetList;
				return View(aum);
			}
			else
			{
				return RedirectToAction("Index", "Admin");
			}


		}

		[HttpPost]
		public ActionResult JqCreateUser(string User, string distid, string pageid, string setid)
		{
			string status = "";
			if (User == null || User == "")
			{
				var results = new
				{
					status = ""
				};
				return Json(results);
			}
			else
			{
				var Jsonobject = JsonConvert.DeserializeObject<AdminUserModel>(User);
				AdminUserModel oModel = JsonConvert.DeserializeObject<AdminUserModel>(User);
				oModel.id = 0;
				oModel.Dist_Allow = distid;
				oModel.PAccessRight = pageid;
				if (setid != "")
				{
					oModel.Set_Allow = setid;
				}

				oModel.pass = Guid.NewGuid().ToString().Substring(0, 6); // Autogenerated password
				if (oModel.listOfActionRight.Count > 0)
				{
					oModel.ActionRight = string.Join(",", oModel.listOfActionRight);
				}
				int OutStatus = 0;
				status = objDB.CreateAdminUser(oModel, out OutStatus);
				if (OutStatus == 1)
				{
					string Sms = "Admin User Details, User Id: " + oModel.user + " and Password: " + oModel.pass + " for login. Regards PSEB";
					if (oModel.Mobno != "")
					{
						string getSms = objCommon.gosms(oModel.Mobno, Sms);
					}
					if (oModel.EmailID != "")
					{
						string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Dear " + oModel.User_fullnm + "</b>,</td></tr><tr><td><b>Your Admin User Details are given Below:-</b><br /><b>User Id :</b> " + oModel.user + "<br /><b>Password :</b> " + oModel.pass + "<br /></td></tr><tr><td height=" + 30 + "><b>Click Here To Login</b> <a href=https://registration2023.pseb.ac.in/Admin target = _blank>https://registration2023.pseb.ac.in/Admin</a></td></tr><tr><td><b>Note:</b> Please Read Instruction Carefully Before filling the Online Form .</td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /></td></tr>";
						// bool result = new AbstractLayer.DBClass().mail("PSEB - Admin User Details", body, "rohit.nanda@ethical.in");
						bool result = new AbstractLayer.DBClass().mail("PSEB - Admin User Details", body, oModel.EmailID);
					}
				}

				var results = new
				{
					status = OutStatus,
				};
				return Json(results);
			}
		}


		public ActionResult ViewUser()
		{
			if (Session["AdminType"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			string Search = string.Empty;
			Search = " id like '%%' ";
			DataSet result = objDB.GetAllAdminUser(0, Search);
			if (result == null)
			{
				return RedirectToAction("Index", "Admin");
			}
			if (result.Tables[0].Rows.Count > 0)
			{
				aum.StoreAllData = result;
				ViewBag.TotalCount = aum.StoreAllData.Tables[0].Rows.Count;
			}
			if (result.Tables[1].Rows.Count > 0)
			{
				//Branch
				List<SelectListItem> allBranch = new List<SelectListItem>();
				foreach (System.Data.DataRow dr in result.Tables[1].Rows)
				{
					allBranch.Add(new SelectListItem { Text = @dr["BranchName"].ToString(), Value = @dr["BranchName"].ToString() });

				}
				ViewBag.Branch = allBranch;
			}

			//ViewBag.Branch = new SelectList(result.Tables[1].Rows, "BranchName", "BranchName", 0);

			ViewBag.SelectedBranch = "";
			ViewBag.SearchUserId = "";
			ViewBag.SearchMobile = "";
			#region Action Assign Method
			if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
			{ ViewBag.IsModiFy = 1; ViewBag.IsDelete = 1; ViewBag.IsView = 1; ViewBag.IsModiFyOpen = 1; }
			else
			{

				string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
				string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string AdminType = Session["AdminType"].ToString();
				//GetActionOfSubMenu(string cont, string act)
				DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
				if (aAct.Tables[0].Rows.Count > 0)
				{
					bool exists = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("MODIFY")).Count() > 0;
					ViewBag.IsModiFy = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("MODIFY")).Count();
					ViewBag.IsDelete = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("DELETE")).Count();
					ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("VIEW")).Count();
					ViewBag.IsModiFyOpen = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("MODIFYOPEN")).Count();


				}
			}
			#endregion Action Assign Method          
			return View(aum);
		}


		[HttpPost]
		public ActionResult ViewUser(FormCollection frm)
		{
			ViewBag.SelectedBranch = "";
			ViewBag.SearchUserId = "";
			ViewBag.SearchMobile = "";
			if (Session["AdminType"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			else if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
			{
				string Search = string.Empty;
				Search = " id like '%%' ";

				string AdminType = Session["AdminType"].ToString();
				if (frm["Branch"] != "")
				{
					ViewBag.SelectedBranch = frm["Branch"];
					Search += " and Branch='" + frm["Branch"].ToString().Trim() + "'";
				}
				if (frm["SearchUserId"] != "")
				{
					ViewBag.SearchUserId = frm["SearchUserId"];
					Search += " and  [user] like '%" + frm["SearchUserId"].ToString().Trim() + "%'";
				}
				if (frm["SearchMobile"] != "")
				{
					ViewBag.SearchMobile = frm["SearchMobile"];
					Search += " and Mobno='" + frm["SearchMobile"].ToString().Trim() + "'";
				}


				DataSet result = objDB.GetAllAdminUser(0, Search);
				if (result == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				if (result.Tables[0].Rows.Count > 0)
				{
					aum.StoreAllData = result;
					ViewBag.TotalCount = aum.StoreAllData.Tables[0].Rows.Count;
				}
				if (result.Tables[1].Rows.Count > 0)
				{
					//Branch
					List<SelectListItem> allBranch = new List<SelectListItem>();
					foreach (System.Data.DataRow dr in result.Tables[1].Rows)
					{
						if (ViewBag.SelectedBranch == @dr["BranchName"].ToString())
						{ allBranch.Add(new SelectListItem { Text = @dr["BranchName"].ToString(), Value = @dr["BranchName"].ToString(), Selected = true }); }
						else
							allBranch.Add(new SelectListItem { Text = @dr["BranchName"].ToString(), Value = @dr["BranchName"].ToString() });

					}
					ViewBag.Branch = allBranch;
				}

				#region Action Assign Method
				if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{ ViewBag.IsModiFy = 1; ViewBag.IsDelete = 1; ViewBag.IsView = 1; ViewBag.IsModiFyOpen = 1; }
				else
				{

					string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
					string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
					int AdminId = Convert.ToInt32(Session["AdminId"]);
					// string AdminType = Session["AdminType"].ToString();
					//GetActionOfSubMenu(string cont, string act)
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						bool exists = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("MODIFY")).Count() > 0;
						ViewBag.IsModiFy = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("MODIFY")).Count();
						ViewBag.IsDelete = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("DELETE")).Count();
						ViewBag.IsView = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("VIEW")).Count();
						ViewBag.IsModiFyOpen = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").Equals("MODIFYOPEN")).Count();


					}
				}
				#endregion Action Assign Method          
			}
			else
			{
				return RedirectToAction("Index", "Admin");
			}
			return View(aum);
		}

		public JsonResult JqSendPassword(string storeid)
		{
			string dee = "1";
			storeid = storeid.Remove(storeid.Length - 1);
			string[] split1 = storeid.Split(',');
			int sCount = split1.Length;
			if (sCount > 0)
			{
				int i = 0;
				foreach (string s in split1)
				{
					i++;
					string userid = s.ToString();
					if (userid != "")
					{
						// dee = objDB.ErrorAllotRegno(stdid, errid, Convert.ToInt32(Action), userid);
						DataSet result = objCommon.GetAdminDetailsById(Convert.ToInt32(userid), Convert.ToInt32(Session["Session"].ToString().Substring(0, 4)));
						if (result.Tables[0].Rows.Count > 0)
						{
							dee = "1";
							string Mobile = result.Tables[0].Rows[0]["Mobno"].ToString();
							string Password = result.Tables[0].Rows[0]["pass"].ToString();
							string user = result.Tables[0].Rows[0]["user"].ToString();
							string Sms = "Admin User Details, User Id: " + user + " and Password: " + Password + " for login. Regards PSEB";
							if (Mobile != "")
							{
								string getSms = objCommon.gosms(Mobile, Sms);
							}
						}
					}
				}
			}

			return Json(new { dee = dee }, JsonRequestBehavior.AllowGet);
		}


		public JsonResult JqComposeSms(string storeid, string SMS)
		{
			string dee = "1";
			storeid = storeid.Remove(storeid.Length - 1);
			string[] split1 = storeid.Split(',');
			int sCount = split1.Length;
			if (sCount > 0)
			{
				int i = 0;
				foreach (string s in split1)
				{
					i++;
					string userid = s.ToString();
					if (userid != "")
					{
						// dee = objDB.ErrorAllotRegno(stdid, errid, Convert.ToInt32(Action), userid);
						DataSet result = objCommon.GetAdminDetailsById(Convert.ToInt32(userid), Convert.ToInt32(Session["Session"].ToString().Substring(0, 4)));
						if (result.Tables[0].Rows.Count > 0)
						{
							dee = "1";
							string Mobile = result.Tables[0].Rows[0]["Mobno"].ToString();
							string Sms = SMS + " Regards PSEB";
							if (Mobile != "" && SMS != "")
							{
								string getSms = objCommon.gosms(Mobile, Sms);
							}
						}
					}
				}
			}

			return Json(new { dee = dee }, JsonRequestBehavior.AllowGet);
		}


		public ActionResult UpdateUserStatus(int id)
		{
			try
			{
				if (Session["AdminType"] == null)
				{ return RedirectToAction("Index", "Admin"); }
				else if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{
					string AdminType = Session["AdminType"].ToString();
					int outstatus = 0;
					string result = objDB.ListingUser(1, id, out outstatus);
					return RedirectToAction("ViewUser", "Admin");
				}
				else
				{
					return RedirectToAction("Index", "Admin");
				}
			}
			catch (Exception)
			{
				return RedirectToAction("ViewUser", "Admin");
			}
		}


		public ActionResult ModifyUser(int id)
		{
			if (id == 0 || id.ToString() == null)
			{ return RedirectToAction("Index", "Admin"); }

			if (Session["AdminType"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			//else if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
			//{
			ViewBag.userid = id.ToString();
			string AdminType = Session["AdminType"].ToString();
			DataSet result = objDB.GetAllAdminUser(id, "");
			// string DistAllow, MenutAllow = "";
			if (result.Tables[0].Rows.Count > 0)
			{
				aum.user = result.Tables[0].Rows[0]["user"].ToString();
				aum.pass = result.Tables[0].Rows[0]["pass"].ToString();
				aum.Usertype = result.Tables[0].Rows[0]["Usertype"].ToString();
				aum.User_fullnm = result.Tables[0].Rows[0]["User_fullnm"].ToString();
				ViewBag.SelectedBranch = aum.Branch = result.Tables[0].Rows[0]["Branch"].ToString();
				aum.Designation = result.Tables[0].Rows[0]["Designation"].ToString();
				aum.EmailID = result.Tables[0].Rows[0]["EmailID"].ToString();
				aum.Mobno = result.Tables[0].Rows[0]["Mobno"].ToString();
				aum.Remarks = result.Tables[0].Rows[0]["Remarks"].ToString();
				aum.STATUS = Convert.ToInt32(result.Tables[0].Rows[0]["STATUS"].ToString().ToUpper() == "ACTIVE" ? 1 : 0);
				ViewBag.SelectedStatus = aum.STATUS;
				ViewBag.DistUser = result.Tables[0].Rows[0]["Dist_Allow"].ToString();
				ViewBag.MenuUser = result.Tables[0].Rows[0]["PAccessRight"].ToString();
				//Set
				ViewBag.Set_Allow = result.Tables[0].Rows[0]["Set_Allow"].ToString();

				if (result.Tables[0].Rows[0]["ActionRight"].ToString() == "")
				{ ViewBag.GetAllPSEBCLASS = GetAllPSEBCLASSList(""); }
				else
				{
					ViewBag.GetAllPSEBCLASS = GetAllPSEBCLASSList(result.Tables[0].Rows[0]["ActionRight"].ToString());
				}
			}



			//GetAll District
			List<SelectListItem> DistList = new List<SelectListItem>();
			DataSet ds = objCommon.Fll_Dist_Details();
			foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
			{
				if (AbstractLayer.StaticDB.ContainsValue(ViewBag.DistUser, @dr["DIST"].ToString()))
				{
					DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString(), Selected = true });
				}
				else
				{ DistList.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() }); }
			}
			aum.DistList = DistList;

			//Status
			//var itemStatus = new SelectList(new[] { new { ID = "1", Name = "ACTIVE" }, new { ID = "0", Name = "IN ACTIVE" }, }, "ID", "Name", 1);
			//ViewBag.Status = itemStatus.ToList();
			// End
			//Branch
			List<SelectListItem> allBranch = new List<SelectListItem>();
			foreach (System.Data.DataRow dr in ds.Tables[2].Rows)
			{
				// allBranch.Add(new SelectListItem { Text = @dr["BranchName"].ToString(), Value = @dr["BranchName"].ToString() });
				if (ViewBag.SelectedBranch == @dr["BranchName"].ToString())
				{ allBranch.Add(new SelectListItem { Text = @dr["BranchName"].ToString(), Value = @dr["BranchName"].ToString(), Selected = true }); }
				else
					allBranch.Add(new SelectListItem { Text = @dr["BranchName"].ToString(), Value = @dr["BranchName"].ToString() });
			}
			ViewBag.Branch = allBranch;

			// All Pages List               
			List<SiteMenu> all = new List<SiteMenu>();
			foreach (System.Data.DataRow dr in ds.Tables[1].Rows)
			{
				if (AbstractLayer.StaticDB.ContainsValue(ViewBag.MenuUser, @dr["MenuID"].ToString()))
				{
					// all.Add(new SiteMenu { MenuID = Convert.ToInt32(@dr["MenuID"]), MenuName = @dr["MenuName"].ToString(), MenuUrl = @dr["MenuUrl"].ToString(), ParentMenuID = Convert.ToInt32(@dr["ParentMenuID"]), IsSelected = true });
					all.Add(new SiteMenu { MenuID = Convert.ToInt32(@dr["MenuID"]), MenuName = @dr["MenuName"].ToString(), MenuUrl = @dr["MenuUrl"].ToString(), ParentMenuID = Convert.ToInt32(@dr["ParentMenuID"]), IsMenu = Convert.ToInt32(@dr["IsMenu"]), IsSelected = true });
				}
				else
				{
					//   all.Add(new SiteMenu { MenuID = Convert.ToInt32(@dr["MenuID"]), MenuName = @dr["MenuName"].ToString(), MenuUrl = @dr["MenuUrl"].ToString(), ParentMenuID = Convert.ToInt32(@dr["ParentMenuID"]) });
					all.Add(new SiteMenu { MenuID = Convert.ToInt32(@dr["MenuID"]), MenuName = @dr["MenuName"].ToString(), MenuUrl = @dr["MenuUrl"].ToString(), ParentMenuID = Convert.ToInt32(@dr["ParentMenuID"]), IsMenu = Convert.ToInt32(@dr["IsMenu"]) });

				}

			}
			aum.SiteMenuModel = all;

			//GetAll Set
			List<SelectListItem> SetList = new List<SelectListItem>();
			foreach (System.Data.DataRow dr in ds.Tables[4].Rows)
			{
				if (AbstractLayer.StaticDB.ContainsValue(ViewBag.Set_Allow, @dr["AdminSet"].ToString()))
				{
					SetList.Add(new SelectListItem { Text = @dr["AdminSet"].ToString(), Value = @dr["AdminSet"].ToString(), Selected = true });
				}
				else
				{ SetList.Add(new SelectListItem { Text = @dr["AdminSet"].ToString(), Value = @dr["AdminSet"].ToString() }); }
			}
			aum.SetList = SetList;

			return View(aum);

		}

		[HttpPost]
		public ActionResult JqModifyUser(string User, string distid, string pageid, string setid)
		{
			string status = "";
			if (User == null || User == "")
			{
				var results = new
				{
					status = ""
				};
				return Json(results);
			}
			else
			{
				var Jsonobject = JsonConvert.DeserializeObject<AdminUserModel>(User);
				AdminUserModel oModel = JsonConvert.DeserializeObject<AdminUserModel>(User);
				// oModel.id = 0;
				oModel.Dist_Allow = distid;
				oModel.PAccessRight = pageid;

				if (setid != "")
				{
					oModel.Set_Allow = setid;
				}
				if (oModel.user.ToString().ToUpper() == "ADMIN" || oModel.id == 1)
				{
					oModel.Usertype = "ADMIN";
				}

				if (oModel.listOfActionRight.Count > 0)
				{
					oModel.ActionRight = string.Join(",", oModel.listOfActionRight);
				}
				//  oModel.Usertype = "";
				int OutStatus = 0;
				status = objDB.CreateAdminUser(oModel, out OutStatus);
				var results = new
				{
					status = OutStatus,
				};
				return Json(results);
			}
		}

		public ActionResult DeleteUser(string id)
		{
			try
			{
				if (Session["AdminId"] == null)
				{ return RedirectToAction("Index", "Admin"); }
				else if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{
					string AdminType = Session["AdminType"].ToString();
					//string result = objDB.DeleteAdminUser(id);
					// ViewData["userDelete"] = result;
					//return RedirectToAction("ViewUser", "Admin");
					int outstatus = 0;
					string result = objDB.ListingUser(0, Convert.ToInt32(id), out outstatus);
					return RedirectToAction("ViewUser", "Admin");
				}
				else
				{
					return RedirectToAction("Index", "Admin");
				}
			}
			catch (Exception)
			{
				return RedirectToAction("ViewUser", "Admin");
			}
		}

		public ActionResult AssignMenuToUser()
		{
			if (Session["AdminType"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			else if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
			{
				string AdminType = Session["AdminType"].ToString();
				//GetAll District
				List<SelectListItem> AdminList = new List<SelectListItem>();
				DataSet ds = objCommon.Fll_Dist_Details();
				foreach (System.Data.DataRow dr in ds.Tables[3].Rows)
				{
					AdminList.Add(new SelectListItem { Text = @dr["AdminName"].ToString(), Value = @dr["id"].ToString() });
				}
				aum.AdminList = AdminList;

				// All Pages List                for multiple
				////List<SiteMenu> all = new List<SiteMenu>();
				////foreach (System.Data.DataRow dr in ds.Tables[1].Rows)
				////{
				////    all.Add(new SiteMenu { MenuID = Convert.ToInt32(@dr["MenuID"]), MenuName = @dr["MenuName"].ToString(), MenuUrl = @dr["MenuUrl"].ToString(), ParentMenuID = Convert.ToInt32(@dr["ParentMenuID"]), IsMenu = Convert.ToInt32(@dr["IsMenu"]) });
				////}
				////aum.SiteMenuModel = all;


				List<SelectListItem> itemMenu = new List<SelectListItem>();
				foreach (System.Data.DataRow dr in ds.Tables[1].Rows)
				{
					if (@dr["SubMenuText"].ToString().Trim().ToUpper() != "")
					{
						itemMenu.Add(new SelectListItem { Text = @dr["SubMenuText"].ToString().Trim(), Value = @dr["MenuID"].ToString().Trim() });
					}
				}
				ViewBag.MyMenu = itemMenu.ToList();

				return View(aum);
			}
			else
			{
				return RedirectToAction("Index", "Admin");
			}

		}

		[HttpPost]
		public ActionResult AssignMenuToUser(string adminlist, string pagelist)
		{
			string status = "";
			if (adminlist == null || adminlist == "")
			{
				var results = new
				{
					status = ""
				};
				return Json(results);
			}
			else if (pagelist == null || pagelist == "")
			{
				var results = new
				{
					status = ""
				};
				return Json(results);
			}
			else
			{
				string OutError = "0";
				status = objDB.AssignMenuToUser(Session["AdminId"].ToString(), adminlist, pagelist, out OutError);
				var results = new
				{
					status = OutError,
				};
				return Json(results);
			}
		}


		public JsonResult GetUserbyMenu(int Menu) // Calling on http post (on Submit)
		{
			string Usertd = string.Empty;
			DataSet result = objDB.GetAdminIdWithMenuId(Menu); // passing Value to DBClass from model

			if (result.Tables[0].Rows.Count > 0)
			{
				List<SelectListItem> _UserList = new List<SelectListItem>();
				foreach (System.Data.DataRow dr in result.Tables[0].Rows)
				{
					_UserList.Add(new SelectListItem { Text = @dr["User"].ToString(), Value = @dr["Id"].ToString() });
				}
				ViewBag.MyUser = _UserList;


				if (result.Tables[1].Rows.Count > 0)
				{
					Usertd = result.Tables[1].Rows[0]["UserList"].ToString();
				}

				var results = new
				{
					UserList = _UserList,
					Usertd = Usertd,
				};
				return Json(results);
			}
			else
			{
				var results = new
				{
					UserList = "",
					Usertd = "",
				};
				return Json(results);
			}

		}


		#endregion Admin User Master





		//
		#region Start Firm photo and sign upload
		public ActionResult FirmPhoto_Upload()
		{
			try
			{
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				////if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				////{
				////    return RedirectToAction("Index", "Admin");
				////}
				////if (Session["UserName"].ToString() != "admin")
				////{
				////    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				////    {
				////        return RedirectToAction("Index", "Admin");
				////    }
				////}
				var itemsch = new SelectList(new[]{new {ID="1",Name="Private Cand"},
														}, "ID", "Name", 1);
				ViewBag.MyForm = itemsch.ToList();
				return View();

			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}
		[HttpPost]
		public ActionResult FirmPhoto_Upload(HttpPostedFileBase FileUploadBulk, FormCollection frm)
		{
			try
			{
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}

				//if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				//{
				//    return RedirectToAction("Index", "Admin");
				//}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}
				var itemsch = new SelectList(new[]{new {ID="1",Name="Private Cand"},

												}, "ID", "Name", 1);
				ViewBag.MyForm = itemsch.ToList();

				ViewBag.SelectedItem = frm["FormNameList"];
				string FormName = frm["FormNameList"].ToString();
				if (frm["FormNameList"] != "")
				{
					if (FormName == "1") { FormName = "PrivateCand"; }
				}
				if (FileUploadBulk != null && FormName != "")
				{

					string FirmNM = Session["UserName"].ToString();
					string DistID = string.Empty;
					string Std_Id = string.Empty;
					string Filepath = string.Empty;
					string path;
					string SchlID = string.Empty;
					string filename = string.Empty;
					string SaveFile = string.Empty;
					string SaveFileN = string.Empty;
					string SaveFileNextn = string.Empty;
					string Incorrectfile = string.Empty;
					int countP = 0; int countS = 0;
					SchlID = Convert.ToString(Session["SCHL"]);
					//if (SchlID == null || SchlID == "")
					//{
					//    return RedirectToAction("Index", "Login");
					//}

					if (FileUploadBulk.ContentLength > 0)
					{

						HttpFileCollectionBase files = Request.Files;
						DataTable dt = new DataTable { Columns = { new DataColumn("Path") } };
						AdminModels sm = new AdminModels();
						AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();

						for (int i = 0; i < files.Count; i++)
						{
							HttpPostedFileBase file = files[i];
							string Prefix = string.Empty;
							if (file.FileName.Length <= 5)
							{
								int lname = file.FileName.Length;
								lname = 16 - lname;

							}
							string fname = Prefix + file.FileName.ToString();
							{
								FirmNM = Session["UserName"].ToString();



								if (fname.Substring(0, fname.IndexOf(".")).ToUpper().Contains("P"))
								{
									filename = fname.Substring(0, fname.IndexOf("P."));
									Std_Id = filename;
									string PhotoName = "PvtPhoto/" + FirmNM + "/Photo" + "/" + Std_Id + "P" + ".jpg";
									string type = "P";
									DataSet UpdatePic = objDB.FirmUpdated_Bulk_Pic_Data(Std_Id, PhotoName, type, FirmNM);
									if (UpdatePic.Tables[0].Rows[0]["FileStatus"].ToString() == "1" || UpdatePic.Tables[0].Rows[0]["FileStatus"].ToString() == "2")
									{
										// Filepath = Server.MapPath("~/Upload/PvtPhoto/"+ FirmNM + "/Photo/");
										Filepath = sp + "/" + "PvtPhoto/" + FirmNM + "/Photo" + "/";
										if (!Directory.Exists(Filepath))
										{
											Directory.CreateDirectory(Filepath);
										}
										SaveFile = SaveFile + file.FileName + ", ";
										path = sp + "/" + "PvtPhoto/" + FirmNM + "/Photo/" + Std_Id + "P" + ".jpg";
										dt.Rows.Add(file.FileName);
										file.SaveAs(path);
										countP = countP + 1;
									}
									else
									{
										Incorrectfile = Incorrectfile + UpdatePic.Tables[0].Rows[0]["FileStatus"].ToString() + ", ";
									}
								}
								else if (fname.Substring(0, fname.IndexOf(".")).ToUpper().Contains("S"))
								{
									filename = fname.Substring(0, fname.IndexOf("S."));
									Std_Id = filename;
									string PhotoName = "PvtPhoto/" + FirmNM + "/Sign" + "/" + Std_Id + "S" + ".jpg";
									string type = "S";
									DataSet UpdatePic = objDB.FirmUpdated_Bulk_Pic_Data(Std_Id, PhotoName, type, FirmNM);
									if (UpdatePic.Tables[0].Rows[0]["FileStatus"].ToString() == "1" || UpdatePic.Tables[0].Rows[0]["FileStatus"].ToString() == "2")
									{
										//Filepath = Server.MapPath("~/Upload/PvtPhoto/"+ FirmNM + "/Sign/");
										Filepath = sp + "/" + "PvtPhoto/" + FirmNM + "/Sign" + "/";
										if (!Directory.Exists(Filepath))
										{
											Directory.CreateDirectory(Filepath);
										}
										SaveFile = SaveFile + file.FileName + ",";
										path = sp + "/" + "PvtPhoto/" + FirmNM + "/Sign/" + Std_Id + "S" + ".jpg";
										dt.Rows.Add(file.FileName);
										file.SaveAs(path);
										countS = countS + 1;

									}
									else
									{
										Incorrectfile = Incorrectfile + UpdatePic.Tables[0].Rows[0]["FileStatus"].ToString() + ", ";
									}
								}

							}

						}
						if (Incorrectfile.ToString() != "")
						{
							@ViewBag.msgInC = "Student Roll No. not exist : [ " + Incorrectfile + " ]";
						}
						if (countP > 0 || countS > 0)
						{
							@ViewBag.msgOK = "Successfully uploaed: Photo = " + countP + " Sign = " + countS;
						}
					}
					else
					{

						SaveFileN = string.Empty;
						@ViewBag.msg = "Check Image";
					}

				}
				else
				{
					@ViewBag.msg = "Select Image and Form Name";
				}

				return View();
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}

		#endregion Firm photo and sign upload
		//
		//
		#region Open Image download     
		public ActionResult FirmDownloadStudentRecordOpen()
		{

			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				//{
				//    return RedirectToAction("Index", "Admin");
				//}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}
				return View();
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}
		[HttpPost]
		public ActionResult FirmDownloadStudentRecordOpen(FormCollection frm)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				//{
				//    return RedirectToAction("Index", "Admin");
				//}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}
				string UserName = Session["UserName"].ToString();
				string SearchString = frm["SdtID"].ToString();
				if (SearchString != "")
				{
					SearchString = Regex.Replace(SearchString, @"\s+", " ");
					SearchString = SearchString.Replace(",,", ",");
					objDB.FirmDownloadStudentRecordOpen(SearchString);
				}
				return RedirectToAction("FirmDownloadStudentRecordOpen", "Admin");

			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}
		#endregion Open Image download
		//
		//
		#region begin Admin CutList
		public ActionResult AdminCutList()
		{
			AdminModels am = new AdminModels();
			try
			{

				//if (Session["AdminType"].ToString().ToUpper() == "ADMIN" || Session["AdminType"].ToString().ToUpper() == "FIRM")
				//{
				//    string AdminType = Session["AdminType"].ToString();
				//}
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");

				}
				else
				{
					string AdminType = Session["AdminType"].ToString();
				}

			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
				// Response.Write(ex.Message);
			}
			return View(am);
		}

		[HttpPost]
		public ActionResult AdminCutList(FormCollection frm, string id)
		{
			AdminModels am = new AdminModels();
			try
			{

				ViewBag.Show = "0";
				ViewData["result"] = "0";
				if (id == "" || id == null)
				{
					if (frm["SchlCode"].ToString() == "")
					{
						return RedirectToAction("AdminCutList", "Admin");
					}
					else
					{
						id = frm["SchlCode"].ToString();
					}

				}
				////if (Session["AdminType"].ToString().ToUpper() == "ADMIN" || Session["AdminType"].ToString().ToUpper() == "FIRM")
				////{
				if (Session["UserName"] != null)
				{

					if (Session["UserName"].ToString().ToUpper() == "CREA" || Session["UserName"].ToString().ToUpper() == "PERF")
					{
						ViewBag.USERCLASS = "12";
					}
					else if (Session["UserName"].ToString().ToUpper() == "SAI" || Session["UserName"].ToString().ToUpper() == "DATA")
					{
						ViewBag.USERCLASS = "10";
					}
					else
					{
						ViewBag.USERCLASS = "Both";
					}

					am.SchlCode = id.ToString();
					Session["CutListSCHL"] = id.ToString();
					ViewBag.SCHL = id.ToString();
					string AdminType = Session["AdminType"].ToString();
					string SCHL = Session["CutListSCHL"].ToString();
					DataSet result = objCommon.schooltypes(SCHL); // passing Value to DBClass from model
					if (result == null)
					{
						return RedirectToAction("Index", "Admin");
					}
					if (result.Tables[1].Rows.Count > 0)
					{
						ViewBag.Show = "1";
						ViewData["result"] = "1";
						ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
						ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
						ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
						ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
					}
				}
				else
				{
					return RedirectToAction("Index", "Admin");
				}

			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
				// Response.Write(ex.Message);
			}
			return View(am);
		}

		public ActionResult CutList(string id)
		{
			try
			{

				//if (Session["AdminType"].ToString().ToUpper() == "ADMIN" || Session["AdminType"].ToString().ToUpper() == "FIRM")
				//{
				if (Session["UserName"] != null)
				{
					if (Session["CutListSCHL"] == null)
					{
						return RedirectToAction("AdminCutList", "Admin");
					}
					if (id == "")
					{
						return RedirectToAction("AdminCutList", "Admin");
					}
					string SCHL = Session["CutListSCHL"].ToString();
					ViewBag.SCHL = SCHL;
					ViewBag.CutlistClass = id;
					string class1 = "";
					string Type1 = "";
					if (id.Contains("E"))
					{
						ViewBag.Status = "0";
					}
					else
					{
						ViewBag.Status = "1";
						if (id == "M")
						{
							class1 = "2";
							Type1 = "REG";
						}
						else if (id == "MO")
						{
							class1 = "10";
							Type1 = "OPEN";
						}
						else if (id == "S")
						{
							class1 = "4";
							Type1 = "REG";
						}
						else if (id == "SO")
						{
							class1 = "12";
							Type1 = "OPEN";
						}

						int OutStatus = 0;
						DataSet result = objDB.UpdateCutListSubjects(SCHL, class1, Type1, out OutStatus); // passing Value to DBClass from model
						if (OutStatus == 0)
						{
							return RedirectToAction("Index", "Admin");
						}
					}
				}
				else
				{
					return RedirectToAction("Index", "Admin");
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
				// Response.Write(ex.Message);
			}
			return View();

		}
		#endregion Admin CutList
		//

		//
		#region Begin Download Compartment Data Firm
		public ActionResult DownloadCompartmentData(AdminModels AM)
		{
			if (Session["UserName"] != null)
			{
				FormCollection frm = new FormCollection();
				int adminid = Convert.ToInt32(Session["AdminId"].ToString());
				int OutStatus = 0;
				// Get Firm name 
				string UserNM = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
				//(int adminid, string firm, int DOWNLOT, out int OutStatus)
				AM.ID = 8;
				DataSet getData = objDB.GetPvtCompDataByFirmLOT(AM, adminid, UserNM, -1, out OutStatus);
				AM.StoreAllData = getData;
				if (AM.StoreAllData == null || AM.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = AM.StoreAllData.Tables[0].Rows.Count;
					return View(AM);
				}
			}
			else
			{
				return RedirectToAction("Index", "Admin");
			}

		}
		[HttpPost]
		public ActionResult DownloadCompartmentData(AdminModels AM, FormCollection frm, string cmd) // HttpPostedFileBase file
		{
			try
			{
				if (Session["UserName"] != null)
				{
					//HttpContext.Session["AdminType"]
					string AdminType = Session["AdminType"].ToString();
					int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
					AM.ID = Convert.ToInt32(frm["ID"]);
					string UserNM = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
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
						string fileName1 = "ComptData_" + UserNM + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

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
							using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
							{
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
								string query = string.Format("Select * from [{0}]", excelSheets[0]);
								using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
								{
									dataAdapter.Fill(ds);
								}
							}
							DataTable dtexport;
							string CheckMis = objDB.CheckComptDataMis(ds, out dtexport);
							if (CheckMis == "")
							{
								DataTable dt1 = ds.Tables[0];
								string Result1 = "";
								int OutStatus = 0;
								int REGNOLOT = 0;
								string commaString = "";
								for (int i = 0; i < dt1.Rows.Count; i++)
								{
									// commaString = commaString + dt1.Rows[i]["REFNO"].ToString();
									commaString = commaString + dt1.Rows[i][0].ToString();
									commaString += (i < dt1.Rows.Count - 1) ? "," : string.Empty;
								}
								//(AutomaticAllotRegnoOPENSP17 for 2017)
								// string dtResult = objDB.AutomaticAllotRegnoOPEN(commaString, out OutStatus, out REGNOLOT);// OutStatus mobile

								string Search = " isnull(a.firm,'') like '%'";
								Search += "and a.refno in ( " + commaString + ") ";
								if (cmd == "downloadimage")
								{
									objDB.DownloadZIPFileSPPvtREFNO(UserNM, Search);  //----PHOTO AND SIGN DOWNLOAD PARTICULAR REFNO
								}
								DataSet ds1 = objDB.GetPvtCompDataByFirmPending(AM, AdminId, UserNM, Search, 1, out OutStatus);



								if (OutStatus > 0 && ds1.Tables.Count > 0)
								{
									if (ds1.Tables[0].Rows.Count > 0)
									{
										//DataTable dt = ds1.Tables[0];
										using (XLWorkbook wb = new XLWorkbook())
										{
											wb.Worksheets.Add(ds1.Tables[0]);
											wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
											wb.Style.Font.Bold = true;
											Response.Clear();
											Response.Buffer = true;
											Response.Charset = "";
											Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
											Response.AddHeader("content-disposition", "attachment;filename=" + fileName1 + ".xls");
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
									else
									{
										return RedirectToAction("DownloadCompartmentData", "Admin");
									}

									return View(AM);
								}
								else
								{
									ViewBag.Message = "Download Failure";
									ViewData["Result"] = "0";
								}
								return View(AM);
							}
							else
							{
								if (dtexport != null)
								{
									ExportDataFromDataTable(dtexport, "REGNO");
								}
								ViewData["Result"] = "-1";
								ViewBag.Message = CheckMis;
								return View(AM);
							}
						}
						else
						{
							ViewData["Result"] = "-2";
							ViewBag.Message = "Please Upload Only .xls file only";
							return View(AM);
						}
					}
				}
				else
				{
					return RedirectToAction("Index", "Admin");
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View(AM);
			}
			return View(AM);
		}
		public ActionResult DownloadCompartmentDataFile(FormCollection frm, AdminModels AM)
		{
			AM = new AdminModels();
			//FormCollection frm = new FormCollection();
			try
			{
				if (Request.QueryString["File"] == null)
				{
					return RedirectToAction("DownloadCompartmentData", "Admin");
				}
				else
				{
					string FileExport = Request.QueryString["File"].ToString();
					DataSet ds = null;
					if (Session["UserName"] != null)
					{
						string fileName1 = string.Empty;
						string Search = string.Empty;
						int OutStatus = 0;
						// ds = objDB.GetPvtCompDataByFirmLOTSP(Convert.ToInt32(FileExport), out OutStatus); // For all Regno alloted                    
						int adminid = Convert.ToInt32(Session["AdminId"].ToString());
						// Get Firm name 
						string UserNM = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
						AM.ID = Convert.ToInt32(frm["ID"]);
						//(int adminid, string firm, int DOWNLOT, out int OutStatus)
						ds = objDB.GetPvtCompDataByFirmLOT(AM, adminid, UserNM, Convert.ToInt32(FileExport), out OutStatus);
						fileName1 = "CompData_" + UserNM + "_LOT" + FileExport;
						if (ds == null)
						{
							return RedirectToAction("DownloadCompartmentData", "Admin");
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
												Response.AddHeader("content-disposition", "attachment;filename=" + fileName1 + ".xls");
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
					else
					{
						return RedirectToAction("DownloadCompartmentData", "Admin");
					}
				}
				return RedirectToAction("DownloadCompartmentData", "Admin");
			}
			catch (Exception ex)
			{
				// ////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return RedirectToAction("DownloadCompartmentData", "Admin");
			}
		}
		public ActionResult DownloadCompartmentDataPendingFile(FormCollection frm)
		{
			try
			{
				AdminModels AM = new AdminModels();
				//FormCollection frm = new FormCollection();
				if (Request.QueryString["File"] == null)
				{
					return RedirectToAction("DownloadCompartmentData", "Admin");
				}
				else
				{
					string FileExport = Request.QueryString["File"].ToString();
					DataSet ds = null;

					////if (Session["AdminType"].ToString().ToUpper() == "ADMIN" || Session["AdminType"].ToString().ToUpper() == "FIRM")
					////{
					if (Session["UserName"] != null)
					{
						string AdminType = Session["AdminType"].ToString();
						int AdminId = Convert.ToInt32(Session["AdminId"].ToString());

						string Search = "a.firm like '%'";
						int OutStatus = 0;
						string UserNM = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
						AM.ID = 8; //Convert.ToInt32(frm["ID"]);
								   //GetPvtCompDataByFirmPending(int adminid,string firm, string search, int Type, out int OutStatus)
						ds = objDB.GetPvtCompDataByFirmPending(AM, AdminId, UserNM, Search, 0, out OutStatus); // GetPvtCompDataByFirmPendingSP      
						string fileName1 = "CompData_" + UserNM + '_' + DateTime.Now.ToString("ddMMyyyyHHmm");
						if (ds == null)
						{
							return RedirectToAction("DownloadCompartmentData", "Admin");
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
												Response.AddHeader("content-disposition", "attachment;filename=" + fileName1 + ".xls");
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
					else
					{
						return RedirectToAction("DownloadCompartmentData", "Admin");
					}
				}
				return RedirectToAction("DownloadCompartmentData", "Admin");
			}
			catch (Exception ex)
			{
				//////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return RedirectToAction("DownloadCompartmentData", "Admin");
			}
		}
		#endregion  Download Pvt Compartment Data Firm
		//

		//
		#region Begin AllotRegNo OPEN
		[AdminLoginCheckFilter]
		public ActionResult AllotRegNoOPEN(AdminModels AM)
		{
			if (Session["UserName"] != null)
			{
				string classAssign = string.Empty;
				if (Session["ClassAssign"] == null || Session["ClassAssign"] == "")
				{
					ViewBag.Result = "10";
					ViewBag.TotalCount = 0;
					return View(AM);
				}

				if (Session["ClassAssign"] != null)
				{
					classAssign = Session["ClassAssign"].ToString();
				}

				string AdminType = Session["AdminType"].ToString();
				int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				int OutStatus = 0;
				DataSet getData = objDB.GetAllotRegnoOPENByLOT(AdminId, "O", classAssign, -1, out OutStatus); // For all Regno alloted  (GetAllotRegnoOPENByLOTSP for 2017)
				AM.StoreAllData = getData;
				if (AM.StoreAllData == null || AM.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.PendingOpen = ViewBag.TotalCount = 0;
					return View();
				}
				else
				{

					ViewBag.TotalCount = AM.StoreAllData.Tables[0].Rows.Count;
					ViewBag.PendingOpen = AM.StoreAllData.Tables[1].Rows[0]["PendingOpen"].ToString();
					return View(AM);
				}
			}
			else
			{
				return RedirectToAction("Index", "Admin");
			}

		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult AllotRegNoOPEN(AdminModels AM, FormCollection frm, string submit) // HttpPostedFileBase file
		{
			try
			{
				////if (Session["AdminType"].ToString().ToUpper() == "ADMIN" || Session["AdminType"].ToString().ToUpper() == "FIRM")
				////{
				if (Session["UserName"] != null)
				{

					string classAssign = string.Empty;
					if (Session["ClassAssign"] == null || Session["ClassAssign"] == "")
					{
						ViewBag.Result = "10";
						ViewBag.TotalCount = 0;
						return View(AM);
					}

					if (Session["ClassAssign"] != null)
					{
						classAssign = Session["ClassAssign"].ToString();
					}

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
						string fileName1 = "AllotRegNoOPEN_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

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
							using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
							{
								try
								{
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
									string query = string.Format("Select * from [{0}]", excelSheets[0]);
									using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
									{
										dataAdapter.Fill(ds);
									}


									DataTable dtexport = new DataTable();
									string CheckMis = "";
									if (!submit.ToUpper().Contains("DOWNLOAD"))
									{
										CheckMis = objDB.CheckAllotRegNoOPENMis(submit.ToLower(), ds, out dtexport);
									}

									if (CheckMis == "")
									{
										DataTable dt1 = ds.Tables[0];
										string Result1 = "";
										int OutStatus = 0;
										int REGNOLOT = 0;
										string commaString = "";
										for (int i = 0; i < dt1.Rows.Count; i++)
										{
											commaString = commaString + dt1.Rows[i]["CANDID"].ToString();
											commaString += (i < dt1.Rows.Count) ? "," : string.Empty;
										}

										// Download
										#region DownloadFile

										if (submit.ToUpper().Contains("DOWNLOAD"))
										{
											DataSet ds1 = objDB.DownloadRegNoAgainstID(commaString, "O", out OutStatus); // For all Regno alloted 
											fileName1 = "Down" + AdminType + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
											if (OutStatus > 0 && ds1.Tables[0].Rows.Count > 0)
											{
												ViewBag.Message = "Registration Number Downloaded Successfully";
												ViewData["Result"] = "1";
												ViewBag.TotalCount = ds1.Tables[0].Rows.Count;
												//DataTable dt = ds1.Tables[0];
												using (XLWorkbook wb = new XLWorkbook())
												{
													wb.Worksheets.Add(ds1.Tables[0]);
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
											}

										}
										#endregion DownloadFile

										// Upload
										#region UploadFile
										else if (submit.ToLower().Contains("generate"))
										{
											string ErrorMsg = "";
											//(AutomaticAllotRegnoOPENSP17 for 2017)
											// string dtResult = objDB.AutomaticAllotRegnoOPEN(AdminId, commaString, out OutStatus, out REGNOLOT);// OutStatus mobile
											DataSet dtResult = objDB.AutomaticAllotRegnoOPEN(AdminId, commaString, out OutStatus, out REGNOLOT, out ErrorMsg);// OutStatus mobile
											if (OutStatus > 0 && REGNOLOT > 0 && ErrorMsg == "1")
											{
												// DataSet getData = objDB.GetAllotRegnoOPENByLOT(AdminId, "O", classAssign, -1, out OutStatus);
												int OutStatus2 = 0;
												DataSet getData = objDB.GetAllotRegnoOPENByLOT(AdminId, "O", classAssign, -1, out OutStatus2); // For all Regno alloted  (GetAllotRegnoOPENByLOTSP for 2017)
												AM.StoreAllData = getData;
												if (getData == null || getData.Tables[0].Rows.Count == 0)
												{
													ViewBag.Message = "Record Not Found";
													ViewBag.PendingOpen = ViewBag.TotalCount = 0;
												}
												else
												{
													ViewBag.TotalCount = getData.Tables[0].Rows.Count;
													ViewBag.PendingOpen = getData.Tables[1].Rows[0]["PendingOpen"].ToString();
												}


												ViewBag.Message = "Registration Number Alloted Successfully";
												ViewData["Result"] = "1";

												if (dtResult != null)
												{
													if (dtResult.Tables.Count > 0)
													{
														if (dtResult.Tables[0].Rows.Count > 1)
														{
															fileName1 = "WrongCandID";
															using (XLWorkbook wb = new XLWorkbook())
															{
																wb.Worksheets.Add(dtResult);
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
														}
													}
												}
											}
											else
											{

												ViewBag.Message = "Registration Number Alloted Failure : " + ErrorMsg;
												ViewData["Result"] = "0";
											}
										}
										#endregion UploadFile                                       
									}
									else
									{
										if (dtexport != null)
										{
											ExportDataFromDataTable(dtexport, "REGNO");
										}
										ViewData["Result"] = "-1";
										ViewBag.Message = CheckMis;
									}

								}
								catch (Exception ex)
								{
									//exception here
									ViewData["Result"] = "-3";
									ViewBag.Message = "Erorr : " + ex.Message;

								}
								finally
								{
									excelConnection.Close();
									excelConnection.Dispose();
								}
							}
							return View(AM);
						}
						else
						{

							ViewData["Result"] = "-2";
							ViewBag.Message = "Please Upload Only .xls file only";
							return View(AM);
						}
					}
				}
				else
				{
					return RedirectToAction("Index", "Admin");
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View(AM);
			}
			return View(AM);
		}

		public ActionResult DOWNLOADREGOPENFILE()
		{
			try
			{
				if (Request.QueryString["File"] == null)
				{
					return RedirectToAction("AllotRegNoOPEN", "Admin");
				}
				else
				{
					string AdminType = Session["AdminType"].ToString();
					int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
					string classAssign = string.Empty;
					if (Session["ClassAssign"] == null || Session["ClassAssign"] == "")
					{
						ViewBag.Result = "10";
						ViewBag.TotalCount = 0;
						return RedirectToAction("AllotRegNoOPEN", "Admin");
					}

					if (Session["ClassAssign"] != null)
					{
						classAssign = Session["ClassAssign"].ToString();
					}


					string FileExport = Request.QueryString["File"].ToString();
					DataSet ds = null;

					////if (Session["AdminType"].ToString().ToUpper() == "ADMIN" || Session["AdminType"].ToString().ToUpper() == "FIRM")
					////{
					if (Session["UserName"] != null)
					{
						// string fileName1 = string.Empty;
						string fileName1 = "DOWNLOADFILE_OPENREGNO_LOT_" + FileExport.ToString() + ".xls";
						string Search = string.Empty;
						int OutStatus = 0;
						ds = objDB.GetAllotRegnoOPENByLOT(AdminId, "0", classAssign, Convert.ToInt32(FileExport), out OutStatus); // For all Regno alloted                    
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
					else
					{
						return RedirectToAction("Index", "Admin");
					}
				}
				return RedirectToAction("AllotRegNoOPEN", "Admin");
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return RedirectToAction("AllotRegNoOPEN", "Admin");
			}
		}

		public ActionResult DOWNLOADPENDINGREGOPENFILE()
		{
			try
			{
				if (Request.QueryString["File"] == null)
				{
					return RedirectToAction("AllotRegNoOPEN", "Admin");
				}
				else
				{

					string FileExport = Request.QueryString["File"].ToString();
					DataSet ds = null;

					////if (Session["AdminType"].ToString().ToUpper() == "ADMIN" || Session["AdminType"].ToString().ToUpper() == "FIRM")
					////{
					if (Session["UserName"] != null)
					{
						string AdminType = Session["AdminType"].ToString();
						int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
						string classAssign = string.Empty;
						if (Session["ClassAssign"] == null || Session["ClassAssign"] == "")
						{
							ViewBag.Result = "10";
							ViewBag.TotalCount = 0;
							return RedirectToAction("AllotRegNoOPEN", "Admin");
						}

						if (Session["ClassAssign"] != null)
						{
							classAssign = Session["ClassAssign"].ToString();
						}
						string fileName1 = string.Empty;
						string Search = string.Empty;
						int OutStatus = 0;
						ds = objDB.GetPendingREGNOByAdminId(classAssign, AdminId, "O", out OutStatus); // GetPendingREGNOByAdminIdSP                  
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
									fileName1 = Session["UserName"].ToString().ToUpper() + "_PendingOpenRegNo";
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
						return RedirectToAction("Index", "Admin");
					}
				}
				return RedirectToAction("AllotRegNoOPEN", "Admin");
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return RedirectToAction("AllotRegNoOPEN", "Admin");
			}
		}


		#endregion  AllotRegNo OPEN
		//

		#region Begin StudentRollNoMIS
		[AdminLoginCheckFilter]
		public ActionResult StudentRollNoMIS()
		{
			if (Session["UserName"] != null)
			{
				return View();
			}
			else
			{
				return RedirectToAction("Index", "Admin");
			}
		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult StudentRollNoMIS(AdminModels AM, FormCollection frm, string Category, string submit) // HttpPostedFileBase file
		{
			try
			{
				if (submit != null)
				{
					if (submit.ToUpper() == "DOWNLOAD")
					{
					}
				}

				string firm = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
				string id = frm["Filevalue"].ToString();
				Category = id;
				if (Session["UserName"] != null)
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
						string fileName1 = "";


						if (id.ToString().ToUpper() == "ROLL" || id.ToString().ToUpper() == "ROLLONLY")
						{
							fileName1 = "StdRollNo_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
						}
						else if (id.ToString().ToUpper() == "RANGE")
						{
							fileName1 = "StdRollNoRange_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
						}
						else if (id.ToString().ToUpper() == "PVT" && submit.ToUpper() == "UPLOAD")
						{
							fileName1 = "StdRollNoPVT_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
						}
						else if (id.ToString().ToUpper() == "PVT" && submit.ToUpper() == "DOWNLOAD")
						{
							fileName1 = "StdRefNoDownPVT_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
						}
						else
						{
							return RedirectToAction("StudentRollNoMIS", "Admin");
						}

						// string fileName1 = "StdRollNo_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

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
							using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
							{
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
								string query = string.Format("Select * from [{0}]", excelSheets[0]);
								using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
								{
									dataAdapter.Fill(ds);
								}
							}

							string CheckMis = "";
							string ColName = "";
							if (id.ToString().ToUpper() == "PVT" && submit.ToUpper() == "DOWNLOAD")
							{ ColName = ds.Tables[0].Columns[0].ColumnName.ToString(); }
							else
							{ ColName = ds.Tables[0].Columns[1].ColumnName.ToString(); }


							DataTable dtexport = new DataTable();
							if (id.ToString().ToUpper() == "ROLL" && ColName.ToUpper() == "CANDID")
							{
								string[] arrayChln = ds.Tables[0].Rows.OfType<DataRow>().Select(k => k[1].ToString()).ToArray();
								bool CheckChln = AbstractLayer.StaticDB.CheckArrayDuplicates(arrayChln);
								if (CheckChln == true)
								{
									ViewData["Result"] = "11";
									ViewBag.Message = "Duplicate Candidate Id";
									return View();
								}
								CheckMis = objDB.CheckStdRollNoMis(ds, out dtexport);
							}
							else if (id.ToString().ToUpper() == "RANGE" && ColName.ToUpper() == "SROLL")
							{
								CheckMis = objDB.CheckStdRollNoRangeMis(ds, out dtexport);
							}
							else if (id.ToString().ToUpper() == "PVT" && submit.ToUpper() == "UPLOAD" && ColName.ToUpper() == "REFNO")
							{
								string[] arrayChln = ds.Tables[0].Rows.OfType<DataRow>().Select(k => k[1].ToString()).ToArray();
								bool CheckChln = AbstractLayer.StaticDB.CheckArrayDuplicates(arrayChln);
								if (CheckChln == true)
								{
									ViewData["Result"] = "11";
									ViewBag.Message = "Duplicate REF NO";
									return View();
								}

								CheckMis = objDB.CheckStdRollNoMisPvt(ds, firm, out dtexport);
							}
							else if (id.ToString().ToUpper() == "PVT" && submit.ToUpper() == "DOWNLOAD" && ColName.ToUpper() == "REFNO")
							{
								CheckMis = objDB.CheckStdRollNoMisPvtDown(ds, firm, out dtexport);
							}
							else if (id.ToString().ToUpper() == "ROLLONLY" && ColName.ToUpper() == "CANDID")
							{
								string[] arrayChln = ds.Tables[0].Rows.OfType<DataRow>().Select(k => k[1].ToString()).ToArray();
								bool CheckChln = AbstractLayer.StaticDB.CheckArrayDuplicates(arrayChln);
								if (CheckChln == true)
								{
									ViewData["Result"] = "11";
									ViewBag.Message = "Duplicate Candidate Id";
									return View();
								}
								if (ds.Tables[0].Columns.Count > 3)
								{
									ds.Tables[0].Columns.RemoveAt(3);
								}
								CheckMis = objDB.CheckStdRollNoMisOnly(ds, out dtexport);
							}
							else
							{
								ViewBag.Message = "Please Check File Structure";
								ViewData["Result"] = "5";
								return View();
							}
							if (CheckMis == "")
							{
								DataSet ds1 = new DataSet();
								DataTable dt1 = ds.Tables[0];
								if (dt1.Columns.Contains("STATUS"))
								{
									dt1.Columns.Remove("STATUS");
								}
								dt1.AcceptChanges();
								string Result1 = "";
								int OutStatus = 0;
								string OutResult = "0";

								if (id.ToString().ToUpper() == "ROLL")
								{
									DataTable dtResult = objDB.StudentRollNoMIS(dt1, AdminId, out OutStatus);// OutStatus mobile
								}
								else if (id.ToString().ToUpper() == "ROLLONLY")
								{
									DataTable dtResult = objDB.StudentRollNoMISONLY(dt1, AdminId, out OutStatus);// OutStatus mobile
								}
								else if (id.ToString().ToUpper() == "RANGE")
								{
									DataTable dtResult = objDB.StudentRollNoRangeMIS(dt1, AdminId, out OutStatus);// OutStatus mobile
								}
								else if (id.ToString().ToUpper() == "PVT" && submit.ToUpper() == "UPLOAD")
								{
									DataTable dtResult = objDB.StudentRollNoMISPVT(dt1, AdminId, out OutResult);// OutStatus mobile
								}
								else if (id.ToString().ToUpper() == "PVT" && submit.ToUpper() == "DOWNLOAD")
								{
									string commaString = "";
									for (int i = 0; i < dt1.Rows.Count; i++)
									{
										// commaString = commaString + dt1.Rows[i]["REFNO"].ToString();
										commaString = commaString + dt1.Rows[i][0].ToString();
										commaString += (i < dt1.Rows.Count - 1) ? "," : string.Empty;
									}
									//(AutomaticAllotRegnoOPENSP17 for 2017)
									// string dtResult = objDB.AutomaticAllotRegnoOPEN(commaString, out OutStatus, out REGNOLOT);// OutStatus mobile

									string Search = "a.firm like '%'";
									Search += "and a.refno in ( " + commaString + ") ";
									ds1 = objDB.GetPvtCompDataByFirmPending(AM, AdminId, firm, Search, 2, out OutStatus);
									if (OutStatus > 0 && ds1.Tables.Count > 0)
									{
										fileName1 = "PvtData" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
										if (ds1.Tables[0].Rows.Count > 0)
										{
											if (ds1.Tables[0] != null)
											{
												ExportDataFromDataTable(ds1.Tables[0], fileName1);
											}
										}

										ViewBag.Message = "File Uploaded Successfully";
										ViewData["Result"] = "1";
										return View();
									}
									else
									{
										ViewBag.Message = "Download Failure";
										ViewData["Result"] = "7";
									}
								}
								else
								{
									ViewBag.Message = "Please Check File Structure";
									ViewData["Result"] = "5";
									return View();
								}


								if (OutStatus > 0 || OutResult == "1")
								{
									ViewBag.Message = "File Uploaded Successfully";
									ViewData["Result"] = "1";
								}
								else
								{
									if (OutResult.ToLower().Contains("update"))
									{
										ViewBag.Message = "File Not Uploaded Successfully- Duplicate Records";
									}
									else
									{ ViewBag.Message = "File Not Uploaded Successfully"; }
									//ViewBag.Message = "File Not Uploaded Successfully";
									ViewData["Result"] = "0";
								}
								return View();
							}
							else
							{
								if (dtexport != null)
								{
									ExportDataFromDataTable(dtexport, firm);
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
				else { return RedirectToAction("Index", "Admin"); }
			}
			catch (Exception ex)
			{
				//////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View();
			}
			return View();
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
						string fileName1 = filename + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";  //103_230820162209_347
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

				return RedirectToAction("Index", "Admin");
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Admin");
			}

		}



		#endregion  StudentRollNoMIS
		//
		#region Begin DownloadZIPFile
		public ActionResult DownloadZIPFile(string id)
		{
			if (id == "perfectzip")
			{
				return View();
			}
			else
			{
				return RedirectToAction("Index", "Admin");

			}
		}
		[HttpPost]
		public ActionResult DownloadZIPFile(string id, FormCollection frm)
		{
			if (id == "perfectzip")
			{
				string tblname = "";
				string db = "";
				if (frm["tblname"] != "" && frm["db"] != "")
				{
					tblname = frm["tblname"].ToString();
					db = frm["db"] == "2016" ? "myConn" : frm["db"] == "2017" ? "myConn2017" : frm["db"] == "2018" ? "myConn2018" : "";

					//DownloadZIPFile(string filename,string tablename)
					string result = objDB.DownloadZIPFile(tblname, db);
					ViewBag.Result = result;
					ViewData["Result"] = result;
				}
				return View();
			}
			else
			{
				return RedirectToAction("Index", "Admin");
			}
		}
		#endregion DownloadZIPFile

		#region Begin ExamErrorListMIS

		public ActionResult DownloadErrorData()
		{
			try
			{
				if (Session["dtResultError"] == null)
				{
					return RedirectToAction("ErrorListMIS", "Admin");
				}
				if (Request.QueryString["File"] == null)
				{
					return RedirectToAction("ErrorListMIS", "Admin");
				}
				else
				{
					string file1 = Request.QueryString["File"].ToString();
					DataSet ds = (DataSet)Session["dtResultError"];
					DataTable dt;
					if (file1 == "P")
					{ dt = ds.Tables[0]; }
					else
					{ dt = ds.Tables[1]; }

					if (dt.Rows.Count == 0)
					{
						return RedirectToAction("ErrorListMIS", "Admin");
					}
					else
					{
						if (dt.Rows.Count > 0)
						{
							string fileName1 = "DownloadErrorData_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";  //103_230820162209_347
							using (XLWorkbook wb = new XLWorkbook())
							{
								// wb.Worksheets.Add(dt);
								wb.Worksheets.Add(dt, "DownloadErrorData");
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
				return RedirectToAction("ErrorListMIS", "Admin");
			}
			catch (Exception ex)
			{
				return RedirectToAction("ErrorListMIS", "Admin");
			}

		}

		[AdminLoginCheckFilter]
		public ActionResult ErrorListMIS()
		{
			if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
			{
				return RedirectToAction("Index", "Admin");
			}
			else
			{
				Session["dtResultError"] = null;
				int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				int type = 1; //for private compt
				string OutResult1 = "0";
				DataSet dtError = objDB.GetErrorListMISByFirmId(type, AdminId, out OutResult1);// OutStatus mobile
				if (OutResult1 == "1")
				{
					if (dtError.Tables.Count > 0)
					{
						ViewBag.ErrPvtTotal = dtError.Tables[0].Rows.Count; //pvt
						ViewBag.ErrRegTotal = dtError.Tables[1].Rows.Count;//reg
						Session["dtResultError"] = dtError;
					}
				}
				else { ViewBag.Errorcount = 0; }
				return View();
			}
		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult ErrorListMIS(AdminModels AM, FormCollection frm) // HttpPostedFileBase file
		{
			try
			{

				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				else
				{

					Session["dtResultError"] = null;
					int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
					int type = 1; //for private compt
					string OutResult1 = "0";
					DataSet dtError = objDB.GetErrorListMISByFirmId(type, AdminId, out OutResult1);// OutStatus mobile
					if (OutResult1 == "1")
					{
						if (dtError.Tables.Count > 0)
						{
							ViewBag.ErrPvtTotal = dtError.Tables[0].Rows.Count; //pvt
							ViewBag.ErrRegTotal = dtError.Tables[1].Rows.Count;//reg
							Session["dtResultError"] = dtError;
						}
					}
					else { ViewBag.Errorcount = 0; }

					string firm = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
					string id = frm["Filevalue"].ToString();
					//HttpContext.Session["AdminType"]
					string AdminType = Session["AdminType"].ToString();
					//int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
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
						// string fileName1 = "ErrorMIS_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

						string fileName1 = "ErrorMIS_" + id.ToString().ToUpper() + '_' + AdminType + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");

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
							//Create Connection to Excel work book and add oledb namespace
							using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
							{
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
								string query = string.Format("Select * from [{0}]", excelSheets[0]);
								using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
								{
									dataAdapter.Fill(ds);
								}
							}

							string CheckMis = "";



							DataTable dtexport = new DataTable();
							if (id.ToString().ToUpper() == "REG")
							{
								var duplicates = ds.Tables[0].AsEnumerable()
								  .GroupBy(i => new { Name = i.Field<string>("CANDID"), Subject = i.Field<string>("ERRCODE") })
								  .Where(g => g.Count() > 1)
								  .Select(g => new { g.Key.Name, g.Key.Subject }).ToList();
								if (duplicates.Count() > 0)
								{
									ViewData["Result"] = "11";
									ViewBag.Message = "Duplicate Record";
									return View();
								}
								CheckMis = objDB.CheckExamMisExcel(ds, out dtexport); // REG
							}
							else if (id.ToString().ToUpper() == "PVT")
							{

								var duplicates = ds.Tables[0].AsEnumerable()
									.GroupBy(i => new { Name = i.Field<string>("REFNO"), Subject = i.Field<string>("ERRCODE") })

									.Where(g => g.Count() > 1)
									.Select(g => new { g.Key.Name, g.Key.Subject }).ToList();
								if (duplicates.Count() > 0)
								{
									ViewData["Result"] = "11";
									ViewBag.Message = "Duplicate Record";
									return View();
								}

								CheckMis = objDB.CheckExamMisExcelPVT(ds, firm, out dtexport); // PVT
							}
							else { return RedirectToAction("ErrorListMIS", "Admin"); }

							if (CheckMis == "")
							{
								DataTable dt1 = ds.Tables[0];
								if (dt1.Columns.Contains("ErrStatus"))
								{
									dt1.Columns.Remove("ErrStatus");
								}
								dt1.AcceptChanges();
								string Result1 = "";
								int OutStatus = 0;
								string OutResult = "0";
								/// DataTable dtResult = objDB.ExamErrorListMIS(dt1, AdminId, out OutStatus);// OutStatus mobile

								if (id.ToString().ToUpper() == "REG")
								{
									DataTable dtResult = objDB.ExamErrorListMIS(dt1, AdminId, out OutStatus);// OutStatus mobile
								}
								else if (id.ToString().ToUpper() == "PVT")
								{
									DataTable dtResult = objDB.ExamErrorListMISPVT(dt1, AdminId, out OutResult);// OutStatus mobile
								}

								if (OutStatus > 0 || OutResult == "1")
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
									ExportDataFromDataTable(dtexport, id.ToString().ToUpper() + "_ErrorReport");
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
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View();
			}
			return View();
		}
		#endregion  ExamErrorListMIS

		public ActionResult StudentAllReport()
		{
			if (Session["UserName"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}
			else
			{
				return View();
			}
		}

		#region Firm Correction Start *****************************************

		public ActionResult DownloadZIPFilePS(string id)
		{
			return View();
		}
		[HttpPost]
		public ActionResult DownloadZIPFilePS(string id, FormCollection frm)
		{
			string tblname = "";
			if (frm["tblname"] != "")
			{
				tblname = frm["tblname"].ToString();
				//DownloadZIPFile(string filename,string tablename)
				string result = objDB.DownloadZIPFileSP(tblname);
				ViewBag.Result = result;
				ViewData["Result"] = result;
			}
			return View();

		}




		public ActionResult FirmSchoolErrorList(int? page)
		{
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}
				FormCollection frc = new FormCollection();
				var itemsch = new SelectList(new[]{new {ID="1",Name="Particular"},new {ID="2",Name="Subject"},new{ID="3",Name="Stream"},
			new{ID="4",Name="Image"},}, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				if (ModelState.IsValid)
				{
					string Search = string.Empty;
					string CrType = "2";
					Search = "err.[Status]='0'";
					//------ Paging Srt
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;
					//string Catg = CrType;                        

					//---- Paging end
					string FirmNM = Session["UserName"].ToString();
					am.StoreAllData = objDB.GetFirmSchoolErrorList(Search, FirmNM, CrType, pageIndex);
					ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;

					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View(am);
					}
					else
					{
						ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
						ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
						int tp = Convert.ToInt32(ViewBag.TotalCount);
						int pn = tp / 30;
						int cal = 30 * pn;
						int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
						if (res >= 1)
							ViewBag.pn = pn + 1;
						else
							ViewBag.pn = pn;

						return View(am);
					}
				}
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
				return View();
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}
		[HttpPost]
		public ActionResult FirmSchoolErrorList(FormCollection frc, int? page, string cmd)
		{
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				if (ModelState.IsValid)
				{
					string Search = string.Empty;
					string CrType = "2";
					Search = "err.[Status]='0'";
					am.SchlCode = frc["SchlCode"].ToString();
					am.SdtID = frc["SdtID"].ToString().Trim();
					am.ROLLexam = frc["ROLLexam"].ToString();
					am.ERRcode = frc["ERRcode"].ToString();
					if (am.SchlCode.ToString().Trim() != "" && am.SchlCode != null)
					{
						Search += " and err.SCHL in('" + am.SchlCode.ToString().Trim() + "')";
					}
					if (am.SdtID.ToString().Trim() != "" && am.SdtID != null)
					{
						Search += " and err.CANDID in('" + am.SdtID.ToString().Trim() + "')";
					}
					if (am.ROLLexam.ToString().Trim() != "" && am.ROLLexam != null)
					{
						Search += " and exm.Roll in('" + am.ROLLexam.ToString().Trim() + "')";
					}
					if (am.ERRcode.ToString().Trim() != "" && am.ERRcode != null)
					{
						Search += " and err.ERRcode in('" + am.ERRcode.ToString().Trim() + "')";
					}

					//------ Paging Srt
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;
					//string Catg = CrType;                        

					//---- Paging end
					string FirmNM = Session["UserName"].ToString();
					am.StoreAllData = objDB.GetFirmSchoolErrorList(Search, FirmNM, CrType, pageIndex);
					ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
					//----------------------

					//}



					if (ViewBag.TotalCountP > 0 && cmd == "Download Error List")
					{
						DataSet ds = objDB.GetFirmSchoolErrorListDownload(Search, FirmNM, CrType, pageIndex);
						DataTable dt = ds.Tables[0];
						string fname = DateTime.Now.ToString("ddMMyyyyHHmm");
						Response.Clear();
						Response.Buffer = true;
						Response.Charset = "";
						Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
						Response.AddHeader("content-disposition", "attachment;filename=ErrList" + fname + ".xlsx");
						using (MemoryStream MyMemoryStream = new MemoryStream())
						{
							XLWorkbook wb = new XLWorkbook();

							var WS = wb.Worksheets.Add(dt, "ErrList" + fname);
							WS.Tables.FirstOrDefault().ShowAutoFilter = false;
							wb.SaveAs(MyMemoryStream);
							MyMemoryStream.WriteTo(Response.OutputStream);
							WS.AutoFilter.Enabled = false;
							Response.Flush();
							Response.End();
						}
					}
					//----------------


					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View(am);
					}
					else
					{
						ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
						ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
						int tp = Convert.ToInt32(ViewBag.TotalCount);
						int pn = tp / 30;
						int cal = 30 * pn;
						int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
						if (res >= 1)
							ViewBag.pn = pn + 1;
						else
							ViewBag.pn = pn;

						return View(am);
					}
				}
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
				return View();
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}


		public ActionResult FirmSchoolCorrectionPending(int? page)
		{
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}
				FormCollection frc = new FormCollection();
				var itemsch = new SelectList(new[]{new {ID="1",Name="Particular"},new {ID="2",Name="Subject"},new{ID="3",Name="Stream"},
			new{ID="4",Name="Image"},}, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				if (ModelState.IsValid)
				{
					//if (Session["CorLot"] != null)
					//{
					//string CorLot2 = Session["CorLot"].ToString();
					//ViewBag.SelectedItemcode = Session["CorrectionType1"].ToString();
					//am.CorrectionLot = Session["CorLot"].ToString();
					string Search = string.Empty;
					string CrType = "2";
					//ViewBag.CorrectionType = am.CorrectionType;
					Search = "a.status is null";
					if (Session["DistAllow"].ToString() != "")
					{
						Search += " and b.DIST in(" + Session["DistAllow"].ToString() + ")";
					}
					if (Session["UserName"].ToString() == "CREA" || Session["UserName"].ToString() == "PERF")
					{
						Search += " and a.class in('Sr.Secondary Open','Sr.Secondary Regular')";
					}
					else if (Session["UserName"].ToString() == "DATA" || Session["UserName"].ToString() == "SAI")
					{
						Search += " and a.class in('Matriculation Regular','Sr.Secondary Regular')";
					}
					//------ Paging Srt
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;
					//string Catg = CrType;                        

					//---- Paging end
					am.StoreAllData = objDB.GetCorrectionDataFirmPending(Search, CrType, pageIndex);
					ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View(am);
					}
					else
					{
						ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
						ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
						int tp = Convert.ToInt32(ViewBag.TotalCount);
						int pn = tp / 30;
						int cal = 30 * pn;
						int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
						if (res >= 1)
							ViewBag.pn = pn + 1;
						else
							ViewBag.pn = pn;

						return View(am);
					}
				}
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
				return View();


				//}
				//else
				//{
				//    return View();
				//}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}
		[HttpPost]
		public ActionResult FirmSchoolCorrectionPending(int? page, FormCollection frc, string cmd)
		{
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}

				FormCollection frm = new FormCollection();
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				var itemsch = new SelectList(new[]{new {ID="1",Name="Particular"},new {ID="2",Name="Subject"},new{ID="3",Name="Stream"},
			new{ID="4",Name="Image"},}, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();

				if (ModelState.IsValid)
				{
					//------ Paging Srt
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;
					//string Catg = CrType;                        

					//---- Paging end                   
					#region View All Correction Pending Record
					//if (cmd == "View All Correction Pending Record")
					//{
					string Search = string.Empty;
					//am.CorrectionLot = frc["CorrectionLot"].ToString();
					string CrType = "2";
					Search = "a.status is null";
					//Search = "a.status is null and a.CorrectionLot= '"+am.CorrectionLot+"'";
					if (Session["DistAllow"].ToString() != "")
					{
						Search += " and b.DIST in(" + Session["DistAllow"].ToString() + ")";
					}
					am.StoreAllData = objDB.GetCorrectionDataFirmPending(Search, CrType, pageIndex);
					ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View(am);
					}
					else
					{
						ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
						ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
						int tp = Convert.ToInt32(ViewBag.TotalCount);
						int pn = tp / 30;
						int cal = 30 * pn;
						int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
						if (res >= 1)
							ViewBag.pn = pn + 1;
						else
							ViewBag.pn = pn;

						return View(am);
					}
				}

				#endregion View All Correction Pending Record

				return View(am);
				//}
				//else
				//{
				//    return View();
				//}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}
		public ActionResult FirmCorrectionHome()
		{
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}
				ViewBag.UserName = Session["UserName"].ToString();
				return View();
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}

		}




		public ActionResult VerifyFirmSchoolCorrection(FormCollection frc, string id, string CorLot)
		{

			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}

				FormCollection frm = new FormCollection();
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				if (ModelState.IsValid)
				{
					string Search = string.Empty;
					Search = "a.correctionlot like '%' ";
					if (Session["DistAllow"].ToString() != "")
					{
						Search += " and b.DIST in(" + Session["DistAllow"].ToString() + ")";
					}
					if (id != null)
					{
						Session["CorLot"] = CorLot;
						//Session["CorrectionType1"] = CorLottypr;
						// Session["CorLottypr"] = CorLottypr;
						id = encrypt.QueryStringModule.Decrypt(id);
						string userNM = Session["UserName"].ToString();
						DataSet ds = objDB.VerifyFirmSchoolCorrection(id, userNM);
						return RedirectToAction("FirmSchoolCorrection", "Admin");
					}
					return RedirectToAction("FirmSchoolCorrection", "Admin");

				}
				else
				{
					return View();
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}
		public ActionResult CancelFirmSchoolCorrection(FormCollection frc, string id, string CorLot)
		{

			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}

				FormCollection frm = new FormCollection();
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				if (ModelState.IsValid)
				{
					string Search = string.Empty;
					Search = "a.correctionlot like '%' ";
					if (Session["DistAllow"].ToString() != "")
					{
						Search += " and b.DIST in(" + Session["DistAllow"].ToString() + ")";
					}
					if (id != null)
					{
						Session["CorLot"] = CorLot;
						id = encrypt.QueryStringModule.Decrypt(id);
						string userNM = Session["UserName"].ToString();
						DataSet ds = objDB.CancelFirmSchoolCorrection(id, userNM);
						return RedirectToAction("FirmSchoolCorrection", "Admin");
					}
					return RedirectToAction("FirmSchoolCorrection", "Admin");

				}
				else
				{
					return View();
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}
		public ActionResult goforfinal()
		{
			return RedirectToAction("FirmSchoolCorrectionFinalSubmit", "Admin");
		}



		//public ActionResult FirmSchoolCorrectionFinalSubmit()
		//{
		//    try
		//    {

		//        if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
		//        {
		//            return RedirectToAction("Index", "Admin");
		//        }

		//        FormCollection frm = new FormCollection();
		//        var itemsch = new SelectList(new[]{new {ID="1",Name="Particular"},new {ID="2",Name="Subject"},new{ID="3",Name="Stream"},
		//    new{ID="4",Name="Image"},}, "ID", "Name", 1);
		//        ViewBag.CorrectionType = itemsch.ToList();
		//        AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
		//        AdminModels am = new AdminModels();
		//        if (ModelState.IsValid)
		//        {                    
		//            string UserName = Session["UserName"].ToString();
		//           am.StoreAllData = objDB.GetAllCorrectionDataFirm(UserName);
		//            ViewBag.TotalCount = 0;//am.StoreAllData.Tables[0].Rows.Count;
		//            if (am.StoreAllData == null || (am.StoreAllData.Tables[0].Rows.Count == 0 && am.StoreAllData.Tables[1].Rows.Count == 0 && am.StoreAllData.Tables[2].Rows.Count == 0 && am.StoreAllData.Tables[3].Rows.Count == 0))
		//            {
		//                ViewBag.Message = "Record Not Found";
		//                ViewBag.TotalCount = 0;
		//                return View(am);
		//            }
		//            else
		//            {
		//                ViewBag.TotalCount = 1;
		//                return View(am);
		//            }

		//        }
		//        else
		//        {
		//            return View();
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        ////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
		//        //return RedirectToAction("Logout", "Login");
		//        return View();
		//    }
		//}
		//[HttpPost]
		//public ActionResult FirmSchoolCorrectionFinalSubmit(FormCollection frm, string cmd)
		//{
		//    AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
		//    AdminModels am = new AdminModels();
		//    try
		//    {
		//        if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
		//        {
		//            return RedirectToAction("Index", "Admin");
		//        }
		//        //if (Session["UserName"].ToString() != "admin")
		//        //{
		//        //    if (!Session["PAccessRight"].ToString().Contains("Firm"))
		//        //    {
		//        //        return RedirectToAction("Index", "Admin");
		//        //    }
		//        //}

		//        if (cmd == "Back")
		//        {
		//            return RedirectToAction("FirmSchoolCorrection", "Admin");
		//        }
		//        else if (cmd == "ParticularFS")
		//        {
		//        string userNM = Session["UserName"].ToString();
		//        string CorType = "05";
		//        DataSet dsFeeDetailLot = objDB.GetCorrectionFeeDetailLot(userNM, CorType);
		//                    if (dsFeeDetailLot != null && dsFeeDetailLot.Tables[0].Rows.Count > 0)
		//                    {
		//                        for (int i = 0; i<dsFeeDetailLot.Tables[0].Rows.Count; i++)
		//                        {
		//                            ViewData["FeeDetailLot"] = ViewData["FeeDetailLot"] + dsFeeDetailLot.Tables[0].Rows[i]["CorrectionLot"].ToString() + ", ";
		//                        }
		//    string UserName = Session["UserName"].ToString();
		//    am.StoreAllData = null;
		//                        am.StoreAllData = objDB.GetAllCorrectionDataFirm(UserName);
		//                        ViewBag.TotalCount = 1;
		//                        ViewData["Status"] = "5";
		//                        return View(am);
		//}
		//                    else
		//                    {
		//                        am.StoreAllData = objDB.GetCorrectionDataFirmFinalSubmit(userNM, CorType);  // Final Submit Main Function
		//                        string UserName = Session["UserName"].ToString();
		//am.StoreAllData = null;
		//                        am.StoreAllData = objDB.GetAllCorrectionDataFirm(UserName);
		//                        ViewBag.TotalCount = 1;
		//                        ViewData["Status"] = "1";
		//                        return View(am);
		//                    }


		//        }
		//        else if (cmd == "SubjectFS")
		//        {
		//            string userNM = Session["UserName"].ToString();
		//            string CorType = "06";
		//            DataSet dsFeeDetailLot = objDB.GetCorrectionFeeDetailLot(userNM, CorType);
		//            if (dsFeeDetailLot != null && dsFeeDetailLot.Tables[0].Rows.Count > 0)
		//            {
		//                for (int i = 0; i < dsFeeDetailLot.Tables[0].Rows.Count; i++)
		//                {
		//                    ViewData["FeeDetailLot"] = ViewData["FeeDetailLot"] + dsFeeDetailLot.Tables[0].Rows[i]["CorrectionLot"].ToString() + ", ";
		//                }
		//                string UserName = Session["UserName"].ToString();
		//                am.StoreAllData = null;
		//                am.StoreAllData = objDB.GetAllCorrectionDataFirm(UserName);
		//                ViewBag.TotalCount = 1;
		//                ViewData["Status"] = "5";
		//                return View(am);
		//            }
		//            else
		//            {
		//                am.StoreAllData = objDB.GetCorrectionDataFirmFinalSubmit(userNM, CorType);

		//                string UserName = Session["UserName"].ToString();
		//                am.StoreAllData = null;
		//                am.StoreAllData = objDB.GetAllCorrectionDataFirm(UserName);
		//                ViewBag.TotalCount = 1;
		//                ViewData["Status"] = "2";
		//                return View(am);
		//            }


		//        }
		//        else if (cmd == "StreamFS")
		//        {
		//            string userNM = Session["UserName"].ToString();
		//            string CorType = "07";
		//            DataSet dsFeeDetailLot = objDB.GetCorrectionFeeDetailLot(userNM, CorType);
		//            if (dsFeeDetailLot != null && dsFeeDetailLot.Tables[0].Rows.Count > 0)
		//            {
		//                for (int i = 0; i < dsFeeDetailLot.Tables[0].Rows.Count; i++)
		//                {
		//                    ViewData["FeeDetailLot"] = ViewData["FeeDetailLot"] + dsFeeDetailLot.Tables[0].Rows[i]["CorrectionLot"].ToString() + ", ";
		//                }
		//                string UserName = Session["UserName"].ToString();
		//                am.StoreAllData = null;
		//                am.StoreAllData = objDB.GetAllCorrectionDataFirm(UserName);
		//                ViewBag.TotalCount = 1;
		//                ViewData["Status"] = "5";
		//                return View(am);
		//            }
		//            else
		//            {
		//                am.StoreAllData = objDB.GetCorrectionDataFirmFinalSubmit(userNM, CorType);

		//                string UserName = Session["UserName"].ToString();
		//                am.StoreAllData = null;
		//                am.StoreAllData = objDB.GetAllCorrectionDataFirm(UserName);
		//                ViewBag.TotalCount = 1;
		//                ViewData["Status"] = "3";
		//                return View(am);
		//            }



		//        }
		//        else if (cmd == "PhotoSignFS")
		//        {
		//            string userNM = Session["UserName"].ToString();
		//            string CorType = "08";
		//            DataSet dsFeeDetailLot = objDB.GetCorrectionFeeDetailLot(userNM, CorType);
		//            if (dsFeeDetailLot != null && dsFeeDetailLot.Tables[0].Rows.Count > 0)
		//            {
		//                for (int i = 0; i < dsFeeDetailLot.Tables[0].Rows.Count; i++)
		//                {
		//                    ViewData["FeeDetailLot"] = ViewData["FeeDetailLot"] + dsFeeDetailLot.Tables[0].Rows[i]["CorrectionLot"].ToString() + ", ";
		//                }
		//                string UserName = Session["UserName"].ToString();
		//                am.StoreAllData = null;
		//                am.StoreAllData = objDB.GetAllCorrectionDataFirm(UserName);
		//                ViewBag.TotalCount = 1;
		//                ViewData["Status"] = "5";
		//                return View(am);
		//            }
		//            else
		//            {
		//                am.StoreAllData = objDB.GetCorrectionDataFirmFinalSubmit(userNM, CorType);

		//                string UserName = Session["UserName"].ToString();
		//                am.StoreAllData = null;
		//                am.StoreAllData = objDB.GetAllCorrectionDataFirm(UserName);
		//                ViewBag.TotalCount = 1;
		//                ViewData["Status"] = "4";
		//                return View(am);
		//            }



		//        }
		//        #region View All Correction Record
		//        if (cmd == "View All Correction Final Submitted Records")
		//        {
		//            return RedirectToAction("FirmSchoolCorrectionAllCorrectionRecord", "Admin");
		//        }

		//        #endregion View All Correction Record 

		//        #region View All Correction Pending Record
		//        //if (cmd == "View All Correction Pending Record")
		//        //{
		//        //string Search = string.Empty;
		//        ////am.CorrectionLot = frc["CorrectionLot"].ToString();
		//        //Search = "a.status is null";
		//        //if (Session["DistAllow"].ToString() != "")
		//        //{
		//        //    Search += " and b.DIST in(" + Session["DistAllow"].ToString() + ")";
		//        //}

		//        //am.StoreAllData = objDB.GetCorrectionDataFirmPending(Search);
		//        //ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
		//        //if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
		//        //{
		//        //    ViewBag.Message = "Record Not Found";
		//        //    ViewBag.TotalCount = 0;
		//        //    return View(am);
		//        //}
		//        //else
		//        //{
		//        //    return View(am);
		//        //}
		//        //}

		//        #endregion View All Correction Pending Record         
		//        else
		//        {
		//            string UserName = Session["UserName"].ToString();
		//            am.StoreAllData = objDB.GetAllCorrectionDataFirm(UserName);
		//            ViewBag.TotalCount = 0;//am.StoreAllData.Tables[0].Rows.Count;
		//            if (am.StoreAllData == null || (am.StoreAllData.Tables[0].Rows.Count == 0 && am.StoreAllData.Tables[1].Rows.Count == 0 && am.StoreAllData.Tables[2].Rows.Count == 0 && am.StoreAllData.Tables[3].Rows.Count == 0))
		//            {
		//                ViewBag.Message = "Record Not Found";
		//                ViewBag.TotalCount = 0;
		//                return View(am);
		//            }
		//            else
		//            {
		//                ViewBag.TotalCount = 1;
		//                return View(am);
		//            }
		//            //return RedirectToAction("FirmSchoolCorrectionFinalSubmit", "Admin");
		//        }

		//    }
		//    catch (Exception ex)
		//    {
		//        return RedirectToAction("Index", "Admin");
		//    }

		//}


		public ActionResult FirmSchoolCorrectionAllCorrectionRecord(int? page)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}
				//string Search = string.Empty;
				////am.CorrectionLot = frc["CorrectionLot"].ToString();
				//Search = "a.FirmCorrectionLot is not null";
				//if (Session["DistAllow"].ToString() != "")
				//{
				//    Search += " and b.DIST in(" + Session["DistAllow"].ToString() + ")";
				//}
				//------ Paging Srt
				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				//string Catg = CrType;                        

				//---- Paging end
				string UserName = Session["UserName"].ToString();
				am.StoreAllData = objDB.FirmSchoolCorrectionAllCorrectionRecord(UserName, pageIndex);
				ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
				//ViewBag.TotalCountP = am.StoreAllData.Tables[1].Rows.Count;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(am);
				}
				else
				{
					//ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
					ViewBag.TotalCountP = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
					ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
					int tp = Convert.ToInt32(ViewBag.TotalCountp);
					int pn = tp / 30;
					int cal = 30 * pn;
					int res = Convert.ToInt32(ViewBag.TotalCountp) - cal;
					if (res >= 1)
						ViewBag.pn = pn + 1;
					else
						ViewBag.pn = pn;
					return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}

		}
		public ActionResult FirmSchoolCorrectionPendingCorrectionRecord(int? page)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}
				string Search = string.Empty;
				Search = "a.FirmCorrectionLot is null and a.status is not null";
				if (Session["DistAllow"].ToString() != "")
				{
					Search += " and b.DIST in(" + Session["DistAllow"].ToString() + ")";
				}
				//------ Paging Srt
				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				//string Catg = CrType;                        

				//---- Paging end
				string UserName = Session["UserName"].ToString();
				am.StoreAllData = objDB.FirmSchoolCorrectionPendingCorrectionRecord(UserName, pageIndex);
				ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(am);
				}
				else
				{
					//ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
					ViewBag.TotalCountP = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
					ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
					int tp = Convert.ToInt32(ViewBag.TotalCountp);
					int pn = tp / 30;
					int cal = 30 * pn;
					int res = Convert.ToInt32(ViewBag.TotalCountp) - cal;
					if (res >= 1)
						ViewBag.pn = pn + 1;
					else
						ViewBag.pn = pn;
					return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}

		}
		public ActionResult FirmFinalPrint()
		{
			FormCollection frm = new FormCollection();
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}

				int pageIndex = 1;
				ViewBag.pagesize = pageIndex;

				var itemsch = new SelectList(new[]{new {ID="2",Name="Particular"},new {ID="1",Name="Subject"},new{ID="3",Name="Stream"},
			new{ID="4",Name="Image"},}, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();

				am.CorrectionType = "";
				am.CorrectionFromDate = "";
				am.CorrectionToDate = "";
				string UserName = Session["UserName"].ToString();
				am.StoreAllData = objDB.GetFirmFinalPrint(am, UserName, pageIndex);
				ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
				ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(am);
				}
				else
				{
					return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}
		[HttpPost]
		public ActionResult FirmFinalPrint(FormCollection frm, string cmd)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}
				if (cmd == "Back")
				{
					return RedirectToAction("FirmSchoolCorrectionFinalSubmit", "Admin");
				}
				var itemsch = new SelectList(new[]{new {ID="2",Name="Particular"},new {ID="1",Name="Subject"},new{ID="3",Name="Stream"},
			new{ID="4",Name="Image"},}, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();

				if (frm["CorrectionFromDate"] == null)
				{
					am.CorrectionType = "";
					ViewBag.SelectedItemcode = "";
					am.CorrectionFromDate = "";
					am.CorrectionToDate = "";
				}
				else
				{
					am.CorrectionType = frm["CorrectionType1"].ToString();
					ViewBag.SelectedItemcode = frm["CorrectionType1"].ToString();
					am.CorrectionFromDate = frm["CorrectionFromDate"].ToString();
					am.CorrectionToDate = frm["CorrectionToDate"].ToString();
				}
				if (cmd == "Download All Checked")
				{
					string id = "aas";
					string CorType = "ALL";
					id = frm["AllChkId"].ToString();
					string UserName = Session["UserName"].ToString();
					am.StoreAllData = objDB.DownloadFirmFinalPrint(id, UserName, CorType.ToUpper());
					ViewBag.TotalCount = 0;//am.StoreAllData.Tables[0].Rows.Count;
					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						am.StoreAllData = objDB.GetFirmFinalPrint(am, UserName, 1);
						ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
						if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.Message = "Record Not Found";
							ViewBag.TotalCount = 0;
							return View(am);
						}
						else
						{
							return View(am);
						}
					}
					else
					{
						return View(am);
					}
				}

				else
				{
					//------ Paging Srt
					int pageIndex = 1;
					// pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;

					string UserName = Session["UserName"].ToString();
					am.StoreAllData = objDB.GetFirmFinalPrint(am, UserName, pageIndex);
					ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
					ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View(am);
					}
					else
					{
						return View(am);
					}

				}
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Admin");
			}
		}
		public ActionResult DownloadFirmFinalPrint(FormCollection frc, string id, string CorType)
		{

			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}
				id = encrypt.QueryStringModule.Decrypt(id);
				// id = "'''" + id.Replace(",", "'',''") + "'''";
				// string ctype = id.Substring(id.Length - 5);
				string UserName = Session["UserName"].ToString();
				am.StoreAllData = objDB.DownloadFirmFinalPrint(id, UserName, CorType.ToUpper());
				ViewBag.TotalCount = 0;//am.StoreAllData.Tables[0].Rows.Count;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return RedirectToAction("FirmFinalPrint", "Admin");
					//return View(am);
				}
				else
				{
					return RedirectToAction("FirmFinalPrint", "Admin");
					//return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}

		public ActionResult DownloadAllFirmFinalPrint(FormCollection frc)
		{
			string id = "aas";
			string CorType = "ALL";
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}
				id = frc["AllChkId"].ToString();
				//id = "'''"+id.Replace(",", "'',''")+"'''";

				// string ctype = id.Substring(id.Length - 5);
				string UserName = Session["UserName"].ToString();
				am.StoreAllData = objDB.DownloadFirmFinalPrint(id, UserName, CorType.ToUpper());
				ViewBag.TotalCount = 0;//am.StoreAllData.Tables[0].Rows.Count;
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					//return RedirectToAction("FirmFinalPrint", "Admin");
					return View(am);
				}
				else
				{
					return RedirectToAction("FirmFinalPrint", "Admin");
					//return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}
		public ActionResult FirmChangePassword()
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				am.CorrectionUserId = Session["UserName"].ToString();
				return View(am);
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}
		[HttpPost]
		public ActionResult FirmChangePassword(FormCollection frc)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				am.CorrectionUserId = Session["UserName"].ToString();
				am.CorrectionOldPwd = frc["CorrectionOldPwd"].ToString();
				am.CorrectionNewPwd = frc["CorrectionNewPwd"].ToString();
				int result = objDB.FirmChangePassword(am);
				if (result >= 1)
				{
					ViewData["Status"] = "1";
					return View();
				}
				else
				{
					ViewData["Status"] = "0";
					return View(am);
				}
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}


		}

		public ActionResult FirmDownloadStudentRecord()
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				return View();
			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}
		[HttpPost]
		public ActionResult FirmDownloadStudentRecord(FormCollection frm)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				string UserName = Session["UserName"].ToString();
				string SearchString = frm["SdtID"].ToString();
				if (SearchString != "")
				{
					SearchString = Regex.Replace(SearchString, @"\s+", " ");
					SearchString = SearchString.Replace(",,", ",");
					objDB.DownloadFirmFinalPrint("", UserName, "ALLstd", SearchString);
				}
				return RedirectToAction("FirmDownloadStudentRecord", "Admin");

			}
			catch (Exception)
			{
				return RedirectToAction("Index", "Admin");
			}
		}



		#endregion FIRM CORRECTION END *****************************************

		//--
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

		public ActionResult FeeEntry(int? ID, FeeModels fm)
		{
			try
			{
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Login");
				}
				List<SelectListItem> itemType = new List<SelectListItem>();
				List<SelectListItem> itemBank = new List<SelectListItem>();
				DataSet dsType = objDB.GetFeeCodeMaster(1, 0);//for all feecode
				if (dsType.Tables.Count > 0)
				{
					// fee code
					if (dsType.Tables[0].Rows.Count > 0)
					{
						foreach (System.Data.DataRow dr in dsType.Tables[0].Rows)
						{
							itemType.Add(new SelectListItem { Text = @dr["FeeCat"].ToString(), Value = @dr["FeeCode"].ToString() });
						}
						ViewBag.FeeCodeList = itemType.ToList();
					}
					// bank code
					if (dsType.Tables[1].Rows.Count > 0)
					{
						foreach (System.Data.DataRow dr in dsType.Tables[1].Rows)
						{
							itemBank.Add(new SelectListItem { Text = @dr["Bankname"].ToString(), Value = @dr["Bcode"].ToString() });
						}
						// ViewBag.BankCodeList = itemBank.ToList();

						ViewBag.BankCodeList = new MultiSelectList(itemBank.ToList(), "Value", "Text");
					}
				}


				ViewBag.FormNameList = objDB.GetAllFormName();

				string Search = string.Empty;
				Search = "Id like '%' ";
				if (ID > 0)
				{
					Search += " and Id=" + ID;
				}
				DataSet ds = objDB.GetAllFeeMaster2016(Search);//GetAllFeeMaster2016SP
				fm.StoreAllData = ds;
				if (fm.StoreAllData == null || fm.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					fm.IsActive = 1;
					if (ID > 0)
					{

						fm.FeeCode = Convert.ToInt32(ds.Tables[0].Rows[0]["FeeCode"].ToString());
						fm.FORM = ds.Tables[0].Rows[0]["FORM"].ToString();
						fm.StartDate = ds.Tables[0].Rows[0]["sDate"].ToString();
						fm.EndDate = ds.Tables[0].Rows[0]["eDate"].ToString();
						fm.BankLastDate = ds.Tables[0].Rows[0]["BankLastDate"].ToString();
						fm.Fee = Convert.ToInt32(ds.Tables[0].Rows[0]["Fee"].ToString());
						fm.LateFee = Convert.ToInt32(ds.Tables[0].Rows[0]["LateFee"].ToString());
						fm.RP = ds.Tables[0].Rows[0]["RP"].ToString();
						fm.AllowBanks = ds.Tables[0].Rows[0]["AllowBanks"].ToString();
						fm.Type = ds.Tables[0].Rows[0]["Type"].ToString();
						fm.IsActive = Convert.ToInt32(ds.Tables[0].Rows[0]["IsActive"].ToString());

						// ViewBag.BankCodeList = itemBank.ToList();
						int[] myArray1 = AbstractLayer.StaticDB.StringToIntArray(fm.AllowBanks, ',');
						ViewBag.BankCodeList = new MultiSelectList(itemBank.ToList(), "Value", "Text", myArray1);

					}
					ViewBag.TotalCount = fm.StoreAllData.Tables[0].Rows.Count;
				}
			}
			catch (Exception ex)
			{
				//////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));               
			}
			return View(fm);
		}

		[HttpPost]
		public ActionResult FeeEntry(FeeModels fm, FormCollection frm, string cmd)
		{
			if (Session["UserName"] == null)
			{
				return RedirectToAction("Index", "Login");
			}


			List<SelectListItem> itemType = new List<SelectListItem>();
			List<SelectListItem> itemBank = new List<SelectListItem>();
			DataSet dsType = objDB.GetFeeCodeMaster(1, 0);//for all feecode
			if (dsType.Tables.Count > 0)
			{
				// fee code
				if (dsType.Tables[0].Rows.Count > 0)
				{
					foreach (System.Data.DataRow dr in dsType.Tables[0].Rows)
					{
						itemType.Add(new SelectListItem { Text = @dr["FeeCat"].ToString(), Value = @dr["FeeCode"].ToString() });
					}
					ViewBag.FeeCodeList = itemType.ToList();
				}
				// bank code
				if (dsType.Tables[1].Rows.Count > 0)
				{
					foreach (System.Data.DataRow dr in dsType.Tables[1].Rows)
					{
						itemBank.Add(new SelectListItem { Text = @dr["Bankname"].ToString(), Value = @dr["Bcode"].ToString() });
					}
					//ViewBag.BankCodeList = itemBank.ToList();
					ViewBag.BankCodeList = new MultiSelectList(itemBank.ToList(), "Value", "Text");
				}
			}

			ViewBag.FormNameList = objDB.GetAllFormName();

			if (cmd.ToLower().Contains("upload"))
			{
				string fileLocation = "";
				string filename = "";
				DataSet ds = new DataSet();
				if (fm.file != null)
				{
					filename = Path.GetFileName(fm.file.FileName);
					if (fm.file.ContentLength > 0)  //(Request.Files["file"].ContentLength > 0
					{
						string fileName1 = "MIS_" + filename + '_' + DateTime.Now.ToString("ddMMyyyyHHmm");  //MIS_201_110720161210
						string fileExtension = System.IO.Path.GetExtension(fm.file.FileName);
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
							fm.file.SaveAs(fileLocation);
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
							using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
							{
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
								string query = string.Format("Select * from [{0}]", excelSheets[0]);
								using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
								{
									dataAdapter.Fill(ds);
								}
							}

							if (ds.Tables[0].Rows.Count > 0)
							{
								DataTable dt1 = ds.Tables[0];
								int OutStatus = 0;
								DataTable dtResult = objDB.BulkFeeMaster(dt1, Convert.ToInt32(Session["AdminId"]), out OutStatus);  // BulkFeeMaster
								if (OutStatus > 0)
								{
									ViewData["result"] = "11";
									ViewBag.Result1 = "All Fees Uploaded Successfully";
								}
								else
								{
									ViewData["result"] = "0";
									ViewBag.Result1 = "Fees Upload Failure";
								}
							}
							else
							{
								ViewData["result"] = "12";
								return View();
							}
						}
						else
						{
							ViewData["result"] = "13";
							return View();
						}
					}
				}
				else
				{
					//Not Saved                 
					ViewData["result"] = "14";
				}
				return View(fm);
			}


			if (cmd.ToLower().Contains("search"))
			{
				string Search = string.Empty;
				Search = "Id like '%' ";
				if (fm.FeeCode > 0)
				{
					Search += " and FeeCode=" + fm.FeeCode;
					if (fm.FeeCode == 20)
					{
						if (fm.FORM != null && fm.FORM != "" && fm.FORM != "0")
						{
							Search += " and FORM='" + fm.FORM + "'";
						}
					}
				}

				DataSet ds = objDB.GetAllFeeMaster2016(Search);//GetAllFeeMaster2016SP
				fm.StoreAllData = ds;
				if (fm.StoreAllData == null || fm.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.TotalCount = fm.StoreAllData.Tables[0].Rows.Count;
				}
				return View(fm);
			}

			//Check server side validation using data annotation
			if (ModelState.IsValid)
			{
				string SelectedSession = "";
				if (frm["SelectedSession"] == "" || frm["SelectedSession"] == null)
				{
					ViewData["Result"] = 20;
					return View(fm);
				}
				else
				{
					fm.AllowBanks = SelectedSession = frm["SelectedSession"].ToString();
				}

				fm.FeeCat = itemType.ToList().Where(x => x.Value == fm.FeeCode.ToString()).Select(x => x.Text).FirstOrDefault();

				if (fm.FeeCode == 20)
				{
					fm.Class = objDB.GetClassNumber(fm.FORM);
				}
				else
				{
					fm.FORM = fm.FeeCat;
					if (fm.FeeCat.ToLower().Contains("matric")) { fm.Class = 10; }
					else { fm.Class = 12; }
				}

				if (fm.ID == null) { fm.ID = 0; }
				int result = objDB.Insert_FeeMaster(fm);//InsertFeeMaster2016SP
				if (result > 0)
				{
					//----Data Inserted Successfully
					//ViewBag.Message = "File has been uploaded successfully";
					ModelState.Clear();
					//--For Showing Message---------//
					ViewData["result"] = 1;
				}
				else if (result == -1)
				{
					//-----alredy exist
					ViewData["result"] = -1;
				}
				else
				{
					//Not Saved                 
					ViewData["result"] = 0;
				}

			}
			return View(fm);
		}

		#region Begin Admin Result Page
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ActionResult adminresultpage()
		{
			try
			{

				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				#region Action Assign Method
				if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{ ViewBag.IsEdit = 1; ViewBag.IsFinalSubmit = 1; }
				else
				{

					string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
					string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
					int AdminId = Convert.ToInt32(Session["AdminId"]);
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						//ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(58)).Count();                        
						//ViewBag.IsFinalSubmit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(59)).Count();

						ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/ADMIN_RESULT_UPDATE")).Count();
						ViewBag.IsFinalSubmit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/ADMINRESULTPAGE")).Count();

					}
				}
				#endregion Action Assign Method

				FormCollection frm = new FormCollection();
				var itemsch = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
			new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
				ViewBag.MySch = itemsch.ToList();
				//ViewBag.SelectedYear = "2015";
				// var itemyear = new SelectList(new[] { new { ID = "2016", Name = "2016" }, new { ID = "2015", Name = "2015" }, new { ID = "2014", Name = "2014" }, }, "ID", "Name", 1);
				var itemyear = new SelectList(new[] { new { ID = "2016", Name = "2016-2017" }, new { ID = "2015", Name = "2015-2016" }, new { ID = "2014", Name = "2014-2015" }, }, "ID", "Name", 1);
				ViewBag.MyYear = itemyear.ToList();
				string year = ViewBag.SelectedYear;
				var itemschcode = new SelectList(new[] { new { ID = "1", Name = "SCHOOL CODE" } }, "ID", "Name", 1);
				ViewBag.MySchcode = itemschcode.ToList();

				var itemschform = new SelectList(new[] { new { ID = "1", Name = "N1" }, new { ID = "2", Name = "N2" }, new { ID = "3", Name = "N3" }, new { ID = "4", Name = "E1" }, new { ID = "5", Name = "E2" } }, "ID", "Name", 1);
				ViewBag.MyForm = itemschform.ToList();

				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				if (ModelState.IsValid)
				{
					string Search = string.Empty;
					string schlid = frm["TotalSearchString"];
					string srch = Convert.ToString(Session["Search"]);

					if (srch != null && srch != "")
					{
						Search = Session["Search"].ToString();
						year = Session["Year"].ToString();
						ViewBag.SelectedYear = year;
						am.TotalSearchString = Session["scode"].ToString();
						ViewBag.SelectedItemcode = "1";
						ViewBag.SelectedForm = Session["SelForm"].ToString();
						ViewBag.SelectedItem = Session["SelList"].ToString();
						//FormCollection frm = new FormCollection();
						frm["SearchByString"] = Session["SearchByString"].ToString();

						am.StoreAllData = objDB.GetSchoolRecordsSearch(Search, year);
						if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.Message = "Record Not Found";
							ViewBag.TotalCount = 0;
							return View();
						}
						else
						{
							// Session["Search"] = string.Empty;
							ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
							return View(am);
						}
					}
					if (Session["SCHL"] != null)
					{
						schlid = Session["SCHL"].ToString();
					}
					else
					{
						ViewBag.Message = "Record Not Found";
						return View(am);
					}
					Search = "schl='" + schlid + "' ";
					am.StoreAllData = objDB.GetSchoolRecordsSearch(Search, year);
					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
						return View(am);
					}
				}
				else
				{
					return adminresultpage();
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}
		[HttpPost]
		public ActionResult adminresultpage(FormCollection frm, string Year)
		{

			if (Session["UserName"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}

			#region Action Assign Method
			if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
			{ ViewBag.IsEdit = 1; ViewBag.IsFinalSubmit = 1; }
			else
			{

				string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
				string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
				if (aAct.Tables[0].Rows.Count > 0)
				{
					//ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(58)).Count();                        
					//ViewBag.IsFinalSubmit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(59)).Count();

					ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/ADMIN_RESULT_UPDATE")).Count();
					ViewBag.IsFinalSubmit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/ADMINRESULTPAGE")).Count();

				}
			}
			#endregion Action Assign Method


			var itemsch = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
			new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();

			var itemschcode = new SelectList(new[] { new { ID = "1", Name = "SCHOOL CODE" } }, "ID", "Name", 1);
			ViewBag.MySchcode = itemschcode.ToList();
			ViewBag.SelectedYear = Year;
			//var itemyear = new SelectList(new[] { new { ID = "2016", Name = "2016" }, new { ID = "2015", Name = "2015" }, new { ID = "2014", Name = "2014" }, }, "ID", "Name", 1);
			var itemyear = new SelectList(new[] { new { ID = "2016", Name = "2016-2017" }, new { ID = "2015", Name = "2015-2016" }, new { ID = "2014", Name = "2014-2015" }, }, "ID", "Name", 1);
			ViewBag.MyYear = itemyear.ToList();

			var itemschform = new SelectList(new[] { new { ID = "1", Name = "N1" }, new { ID = "2", Name = "N2" }, new { ID = "3", Name = "N3" }, new { ID = "4", Name = "E1" }, new { ID = "5", Name = "E2" } }, "ID", "Name", 1);
			ViewBag.MyForm = itemschform.ToList();

			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			if (ModelState.IsValid)
			{
				string Search = string.Empty;
				Search = "schl like '%' ";
				string schlid = "";
				//string SelTotalItem = frm["totalcountlist"];
				string TotalSearchString = Convert.ToString(frm["TotalSearchString"]);
				//ViewBag.SelectedForm = frm["SelForm"].ToString();
				am.TotalSearchString = TotalSearchString;
				//if (frm["totalcountlist"].ToString() != "" && TotalSearchString != "" && TotalSearchString != null)
				if (TotalSearchString != "")
				{
					if (Year == "2016")
					{
						if (frm["SelForm"].ToString() != "")
						{
							string FormName = frm["SelForm"].ToString();
							if (FormName == "1") { FormName = "N1"; }
							else if (FormName == "2") { FormName = "N2"; }
							else if (FormName == "3") { FormName = "N3"; }
							else if (FormName == "4") { FormName = "E1"; }
							else if (FormName == "5") { FormName = "E2"; }
							ViewBag.SelectedItemcode = frm["totalcountlist"];
							schlid = frm["TotalSearchString"];
							Search += " and FORM_Name='" + FormName + "'";
						}
						else
						{
							//ViewBag.SelectedItemcode = frm["totalcountlist"];
							Search += " and FORM_Name in ('E1','E2','N1','N2','N3') ";
						}
					}
					else if (Year == "2015" || Year == "2014")
					{
						if (frm["SelForm"].ToString() != "")
						{
							string FormName = frm["SelForm"].ToString();
							if (FormName == "1") { FormName = "N1"; }
							else if (FormName == "2") { FormName = "N2"; }
							else if (FormName == "3") { FormName = "N3"; }
							else if (FormName == "4") { FormName = "E1"; }
							else if (FormName == "5") { FormName = "E2"; }
							ViewBag.SelectedItemcode = frm["totalcountlist"];
							schlid = frm["TotalSearchString"];
							Search += " and FORM='" + FormName + "'";
						}
						else
						{
							ViewBag.SelectedItemcode = frm["totalcountlist"];
							Search += " and FORM in ('E1','E2','N1','N2','N3') ";
						}
					}
				}
				if (TotalSearchString != "")
				{
					schlid = frm["TotalSearchString"];
					if (schlid != "")
					{ Search += " and schl='" + schlid + "' "; }
					//Search = "schl like '%" + schlid + "%' ";
					//  Search = "and schl like '%" + schlid + "%' ";
				}

				if (frm["SelList"] != "")
				{
					ViewBag.SelectedItem = frm["SelList"];
					int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());
					if (frm["SearchByString"] != "" && frm["SearchByString"] != null && Year != "2016")
					{
						if (SelValueSch == 1)
						{ Search += " and id='" + frm["SearchByString"].ToString() + "'"; }
						else if (SelValueSch == 2)
						{ Search += " and  RegNo like '%" + frm["SearchByString"].ToString() + "%'"; }
						else if (SelValueSch == 3)
						{ Search += " and  name like '%" + frm["SearchByString"].ToString() + "%'"; }
						else if (SelValueSch == 4)
						{ Search += " and  fname  like '%" + frm["SearchByString"].ToString() + "%'"; }
						else if (SelValueSch == 5)
						{ Search += " and mname like '%" + frm["SearchByString"].ToString() + "%'"; }
						else if (SelValueSch == 6)
						{ Search += " and DOB='" + frm["SearchByString"].ToString() + "'"; }
					}
					if (frm["SearchByString"] != "" && frm["SearchByString"] != null && Year == "2016")
					{
						if (SelValueSch == 1)
						{ Search += " and Std_id='" + frm["SearchByString"].ToString() + "'"; }
						else if (SelValueSch == 2)
						{ Search += " and  Registration_Num like '%" + frm["SearchByString"].ToString() + "%'"; }
						else if (SelValueSch == 3)
						{ Search += " and  Candi_name like '%" + frm["SearchByString"].ToString() + "%'"; }
						else if (SelValueSch == 4)
						{ Search += " and  father_name  like '%" + frm["SearchByString"].ToString() + "%'"; }
						else if (SelValueSch == 5)
						{ Search += " and mother_name like '%" + frm["SearchByString"].ToString() + "%'"; }
						else if (SelValueSch == 6)
						{ Search += " and DOB='" + frm["SearchByString"].ToString() + "'"; }
					}
				}
				am.SelYear = frm["year"].ToString();
				am.SearchResult = Search;
				am.StoreAllData = objDB.GetSchoolRecordsSearch(Search, Year);
				Session["scode"] = frm["TotalSearchString"];
				Session["Search"] = Search;
				Session["Year"] = Year;
				Session["SelForm"] = frm["SelForm"];
				ViewBag.SelectedForm = frm["SelForm"];
				Session["SelList"] = frm["SelList"];
				Session["SearchByString"] = frm["SearchByString"];
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					Session["Search"] = Search.ToString();
					Session["txtSchoolcode"] = frm["TotalSearchString"];
					Session["ddlSchoolcode"] = frm["totalcountlist"];
					ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
					return View(am);
				}
			}
			else
			{
				return adminresultpage();
			}
		}

		public ActionResult Admin_Result_Update(string id, string year)
		{
			try
			{
				AdminModels am = new AdminModels();
				try
				{
					id = encrypt.QueryStringModule.Decrypt(id);
					year = encrypt.QueryStringModule.Decrypt(year);
					ViewBag.year = year;
				}
				catch (Exception)
				{
					Session["Search"] = null;
					return RedirectToAction("Index", "Admin");
				}

				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["AdminType"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Reg"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}

				string stdid = id;
				if (stdid != null)
				{
					ViewBag.MyEXM = objCommon.GroupName1();
					AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();

					string Search = string.Empty;
					if (year == "2016")
					{
						Search = "Std_ID='" + stdid + "' ";
					}
					else
					{
						Search = "ID='" + stdid + "' ";
					}
					am.StoreAllData = objDB.GetSchoolRecordsSearch(Search, year);
					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
						am.Candi_Name = am.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
						am.Father_Name = am.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
						am.Mother_Name = am.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();
						am.DOB = am.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
						am.Gender = am.StoreAllData.Tables[0].Rows[0]["SEX"].ToString();
						am.Result = am.StoreAllData.Tables[0].Rows[0]["result"].ToString();
						am.TotalMarks = am.StoreAllData.Tables[0].Rows[0]["totMarks"].ToString();
						am.ObtainedMarks = am.StoreAllData.Tables[0].Rows[0]["obtMarks"].ToString();
						am.reclock = am.StoreAllData.Tables[0].Rows[0]["reclock"].ToString();
						am.SdtID = am.StoreAllData.Tables[0].Rows[0]["ID"].ToString();
						am.FormName = am.StoreAllData.Tables[0].Rows[0]["FORM"].ToString();
						ViewBag.SelectedExam = am.StoreAllData.Tables[0].Rows[0]["EXAM"].ToString();
						return View(am);
					}
				}
				else
				{

					return Admin_Result_Update(id, year);
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}


		[HttpPost]
		public ActionResult Update_Result(AdminModels am, FormCollection fc, string exam, string year)
		{

			if (Session["UserName"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}
			try
			{
				string stdid = encrypt.QueryStringModule.Decrypt(fc["ID"]);
				if (stdid != null)
				{
					AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
					am.ID = Int32.Parse(stdid);
					am.TotalMarks = fc["TotalMarks"];
					am.ObtainedMarks = fc["ObtainedMarks"];
					am.Result = fc["resultlist"];
					am.reclock = fc["reclocklist"] == "TRUE" ? "1" : "0";
					am.EXAM = exam;

					int result = objDB.UpdateStudentRecords(am, year);
					// asm.StoreAllData = objDB.SearchSchoolDetails(Search);
					if (result > 0)
					{
						ViewBag.Message = "Record Updated Successfully";
						return RedirectToAction("adminresultpage", "Admin");
						//return adminresultpageReCall(fc);
					}
					else
					{
						return RedirectToAction("Admin_Result_Update", "Admin");
					}
				}
				else
				{
					return RedirectToAction("Index", "Admin");
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				Session["Search"] = null;
				return RedirectToAction("Index", "Admin");

			}
		}


		[HttpPost]
		public ActionResult FinalsubmitResult(FormCollection fc)
		{
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
				{
					return RedirectToAction("Index", "Admin");
				}

				AdminModels am = new AdminModels();
				if (fc["TotalSearchString"].ToString() != null)
				{
					AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();

					am.SchlCode = fc["TotalSearchString"].ToString();
					am.SelYear = fc["year"].ToString();
					int result = objDB.FinalsubmitResult(am);
					if (result > 1)
					{
						TempData["notice"] = "All Record Successfully Submitted";
						//return RedirectToAction("adminresultpage", "Admin");
					}
					else
					{
						TempData["notice"] = "Kindly update all student result then click Final Submit Result Button ";
					}
				}
				return RedirectToAction("adminresultpage", "Admin");
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}

		#endregion End Admin Result Page

		#region Admin Challan MasterDetail 
		public ActionResult ChallanDetail(string id)
		{
			if (Session["UserName"] == null)
			{
				return RedirectToAction("Index", "Login");
			}
			FormCollection frc = new FormCollection();
			var itemMyBankList = new SelectList(new[] { new { ID = "1", Name = "PNB" }, new { ID = "2", Name = "SBOP" }, new { ID = "3", Name = "PSEB" } }, "ID", "Name", 1);
			ViewBag.MyBankList = itemMyBankList.ToList();
			ViewBag.selecteditemcode = frc["MyBankList"];

			var itemMySchlList = new SelectList(new[] { new { ID = "1", Name = "School" } }, "ID", "Name", 1);
			//var itemMySchlList = new SelectList(new[] { new { ID = "1", Name = "Open" }, new { ID = "2", Name = "Re-Appear" }, new { ID = "3", Name = "Additional" }, new { ID = "4", Name = "Golden" }, new { ID = "5", Name = "School" } }, "ID", "Name", 1);
			ViewBag.MySchlList = itemMySchlList.ToList();
			ViewBag.selecteditemcode = frc["MySchlList"];

			var itemMyChallanList = new SelectList(new[]{new {ID="1",Name="Challan ID"},new {ID="2",Name="APP No/Roll No/Schl"},new{ID="3",Name="Download Lot"},
			new{ID="4",Name="Journal No"}}, "ID", "Name", 1);
			ViewBag.MyChallanList = itemMyChallanList.ToList();
			ViewBag.SelectedItem = frc["MyChallanList"];
			if (id != null)
			{
				string stdid = id;
				if (stdid != null)
				{
					AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
					AdminModels am = new AdminModels();
					string Search = string.Empty;
					Search = "bmstr.CHALLANID='" + stdid + "' ";
					am.StoreAllData = objDB.GetViewChallanDetail(Search);
					// asm.StoreAllData = objDB.SearchSchoolDetails(Search);
					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
						am.ChallanID = am.StoreAllData.Tables[0].Rows[0]["challanID"].ToString();
						am.SchlReffAppRll = am.StoreAllData.Tables[0].Rows[0]["SchoolCode"].ToString();
						am.Candi_Name = am.StoreAllData.Tables[0].Rows[0]["SCHLCANDNM"].ToString();
						am.Challan_Date = am.StoreAllData.Tables[0].Rows[0]["chlndate"].ToString();
						am.Challan_V_Date = am.StoreAllData.Tables[0].Rows[0]["chlnvdate"].ToString();
						am.FeeType = am.StoreAllData.Tables[0].Rows[0]["FEECAT"].ToString();
						am.BankName = am.StoreAllData.Tables[0].Rows[0]["BANK"].ToString();
						am.Fee = am.StoreAllData.Tables[0].Rows[0]["FEE"].ToString();
						am.Journal_No = am.StoreAllData.Tables[0].Rows[0]["J_REF_NO"].ToString();
						am.Branch = am.StoreAllData.Tables[0].Rows[0]["BRANCH"].ToString();
						am.Challan_Depst_Date = am.StoreAllData.Tables[0].Rows[0]["DEPOSITDT"].ToString();
						am.Challan_Dwld_Stats = am.StoreAllData.Tables[0].Rows[0]["DOWNLDDATE"].ToString();
						am.Challan_Verify_Stats = am.StoreAllData.Tables[0].Rows[0]["VERIFIED"].ToString();
						if (am.Challan_Verify_Stats == "1")
						{ am.Challan_Verify_Stats = "True"; }
						else { am.Challan_Verify_Stats = "False"; }
						ViewBag.schlid = 1;
						return View(am);
					}

				}
			}

			return View();

		}

		[HttpPost]
		public ActionResult ChallanDetail(FormCollection frc)
		{
			if (Session["UserName"] == null)
			{
				return RedirectToAction("Index", "Login");
			}
			var itemMyBankList = new SelectList(new[] { new { ID = "1", Name = "PNB" }, new { ID = "2", Name = "SBOP" }, new { ID = "3", Name = "PSEB" } }, "ID", "Name", 1);
			ViewBag.MyBankList = itemMyBankList.ToList();
			ViewBag.selecteditemBank = frc["bankddlist"];

			var itemMySchlList = new SelectList(new[] { new { ID = "1", Name = "School" } }, "ID", "Name", 1);
			//var itemMySchlList = new SelectList(new[] { new { ID = "1", Name = "Open" }, new { ID = "2", Name = "Re-Appear" }, new { ID = "3", Name = "Additional" }, new { ID = "4", Name = "Golden" }, new { ID = "5", Name = "School" } }, "ID", "Name", 1);
			ViewBag.MySchlList = itemMySchlList.ToList();
			ViewBag.selecteditemSchl = frc["Schlddlist"];

			var itemMyChallanList = new SelectList(new[]{new {ID="1",Name="Challan ID"},new {ID="2",Name="APP No/Roll No/Schl"},new{ID="3",Name="Download Lot"},
			new{ID="4",Name="Journal No"}}, "ID", "Name", 1);
			ViewBag.MyChallanList = itemMyChallanList.ToList();
			ViewBag.SelectedItemChln = frc["ChallanddList"];
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			if (ModelState.IsValid)
			{
				string Search = string.Empty;
				string schlid = Session["SCHL"].ToString();
				string SelItemBank = frc["bankddlist"];
				string SelItemSchl = frc["Schlddlist"];
				string SelItemChln = frc["ChallanddList"];

				if (SelItemBank != "0" && SelItemBank != "")
				{
					switch (SelItemBank)
					{
						case "2":
							Search = "bm.BCODE = '201' ";
							break;
						case "1":
							Search = "bm.BCODE = '202' ";
							break;
						case "3":
							Search = "bm.BCODE = '203' ";
							break;
						default:
							Search = "bm.BCODE like '%%' ";
							break;
					}
				}
				else
				{
					Search = "bm.BCODE like '%%' ";
				}

				if (SelItemSchl != "0" && SelItemSchl != "")
				{
					switch (SelItemSchl)
					{
						case "1":
							Search += "and cm.SchoolCode = '" + schlid + "' ";
							break;
						//case "2":
						//    Search += "and cm.SchoolCode = '" + schlid + "' ";
						//    break;
						//case "3":
						//    Search += "and cm.SchoolCode = '" + schlid + "' ";
						//    break;
						//case "4":
						//    Search += "and cm.SchoolCode = '" + schlid + "' ";
						//    break;
						//case "5":
						//    Search += "and cm.SchoolCode = '" + schlid + "' ";
						//    break;
						default:
							Search = "cm.SchoolCode like '%%' ";
							break;
					}
				}

				if (SelItemChln != "0" && SelItemChln != "")
				{
					if (frc["SearchByString"] != "")
					{
						if (SelItemChln == "1")
						{ Search += " and cm.CHALLANID='" + frc["SearchByString"].ToString() + "'"; }
						//else if (SelItemChln == "2")
						//{ Search += " and  RegNo like '%" + frc["SearchByString"].ToString() + "%'"; }
						else if (SelItemChln == "3")
						{ Search += " and  cm.DOWNLDFLOT = '" + frc["SearchByString"].ToString() + "'"; }
						else if (SelItemChln == "4")
						{ Search += " and  bmstr.J_REF_NO = '" + frc["SearchByString"].ToString() + "'"; }
					}
					else
					{
						return View();
					}
				}


				am.SearchResult = Search;
				am.StoreAllData = objDB.GetAdminChallanDetailSearch(Search);
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
					return View(am);
				}
			}
			else
			{
				return View(am);
			}
		}

		public ActionResult ViewChallanDetail(string id)
		{
			if (Session["UserName"] == null)
			{
				return RedirectToAction("Index", "Login");
			}
			string stdid = id;
			if (stdid != null)
			{
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				string Search = string.Empty;
				Search = "bmstr.CHALLANID='" + stdid + "' ";
				am.StoreAllData = objDB.GetViewChallanDetail(Search);
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
					am.ChallanID = am.StoreAllData.Tables[0].Rows[0]["challanID"].ToString();
					am.SchlReffAppRll = am.StoreAllData.Tables[0].Rows[0]["SchoolCode"].ToString();
					am.Candi_Name = am.StoreAllData.Tables[0].Rows[0]["SCHLCANDNM"].ToString();
					am.Challan_Date = am.StoreAllData.Tables[0].Rows[0]["chlndate"].ToString();
					am.Challan_V_Date = am.StoreAllData.Tables[0].Rows[0]["chlnvdate"].ToString();
					am.FeeType = am.StoreAllData.Tables[0].Rows[0]["FEECAT"].ToString();
					am.BankName = am.StoreAllData.Tables[0].Rows[0]["BANK"].ToString();
					am.Fee = am.StoreAllData.Tables[0].Rows[0]["FEE"].ToString();
					am.Journal_No = am.StoreAllData.Tables[0].Rows[0]["J_REF_NO"].ToString();
					am.Branch = am.StoreAllData.Tables[0].Rows[0]["BRANCH"].ToString();
					am.Challan_Depst_Date = am.StoreAllData.Tables[0].Rows[0]["DEPOSITDT"].ToString();
					am.Challan_Dwld_Stats = am.StoreAllData.Tables[0].Rows[0]["DOWNLDDATE"].ToString();
					am.Challan_Verify_Stats = am.StoreAllData.Tables[0].Rows[0]["VERIFIED"].ToString();
					if (am.Challan_Verify_Stats == "1")
					{ am.Challan_Verify_Stats = "True"; }
					else { am.Challan_Verify_Stats = "False"; }

					return View(am);
				}
			}
			else
			{
				return View();
			}
		}

		#endregion Admin Challan MasterDetail


		public ActionResult VerifyChallan(string id)
		{
			try
			{
				ViewBag.date = DateTime.Now.ToString("dd/mm/yyyy");
				ViewBag.schl = id;
				ChallanMasterModel CM = new ChallanMasterModel();
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Logout", "Admin");
				}
				else
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
				}

				string schl = id;

				DataSet ds = objDB1.GetFinalPrintChallan(schl);//School Cpde
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
					if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "0")
					{
						//Challan Generated
						CM.Action = "0";
					}
					else if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "1")
					{                                       //Downloaded by Bank 
						CM.Action = "1";
					}
					else if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "2")
					{
						//HttpRuntime.Cache.Remove("FinalPrintDataSet");
						//Challan Verified
						CM.Action = "2";
					}
				}
				//CM.LOT = Convert.ToInt32(ds.Tables[0].Rows[0]["LOT"].ToString());
				//CM.BANK = ds.Tables[0].Rows[0]["BANK"].ToString();
				//CM.CHALLANID = ds.Tables[0].Rows[0]["CHALLANID"].ToString();
				//CM.CHLNDATE = ds.Tables[0].Rows[0]["ChallanDate"].ToString();
				//CM.CHLNVDATE = ds.Tables[0].Rows[0]["ChallanVDate"].ToString();             
				//CM.FEECAT = ds.Tables[0].Rows[0]["Status"].ToString();                 
				//CM.SchoolName = ds.Tables[0].Rows[0]["SchoolName"].ToString();              
				return View(CM);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}

		[HttpPost]
		public ActionResult VerifyChallan(string txtschl, string dee)
		{
			try
			{
				ViewBag.date = DateTime.Now.ToString("dd/mm/yyyy");
				ViewBag.schl = txtschl;
				ChallanMasterModel CM = new ChallanMasterModel();
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Logout", "Admin");
				}
				string schl = txtschl;

				DataSet ds = objDB1.GetFinalPrintChallan(schl);//School Cpde
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
					if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "0")
					{
						//Challan Generated
						CM.Action = "0";
					}
					else if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "1")
					{                                       //Downloaded by Bank 
						CM.Action = "1";
					}
					else if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "2")
					{
						//HttpRuntime.Cache.Remove("FinalPrintDataSet");
						//Challan Verified
						CM.Action = "2";
					}
				}
				//CM.LOT = Convert.ToInt32(ds.Tables[0].Rows[0]["LOT"].ToString());
				//CM.BANK = ds.Tables[0].Rows[0]["BANK"].ToString();
				//CM.CHALLANID = ds.Tables[0].Rows[0]["CHALLANID"].ToString();
				//CM.CHLNDATE = ds.Tables[0].Rows[0]["ChallanDate"].ToString();
				//CM.CHLNVDATE = ds.Tables[0].Rows[0]["ChallanVDate"].ToString();             
				//CM.FEECAT = ds.Tables[0].Rows[0]["Status"].ToString();                 
				//CM.SchoolName = ds.Tables[0].Rows[0]["SchoolName"].ToString();              
				return View(CM);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}


		[HttpPost]
		public ActionResult Jqverify(string challanno, string totfee, string refno, string vdate)
		{
			if (Session["UserName"] == null)
			{
				return RedirectToAction("Logout", "Admin");
			}
			BankModels BM = new BankModels();
			//DateTime fromDateValue;
			//if (DateTime.TryParseExact(DateTime.Now.ToString(), "dd/MM/yyyy hh:mm:sstt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateValue))
			//{
			//}
			BM.CHALLANID = challanno;
			BM.TOTFEE = float.Parse(totfee);
			BM.BRCODE = "203";
			BM.BRANCH = "PSEB";
			BM.J_REF_NO = refno;
			//BM.DEPOSITDT = DateTime.Now.ToString("dd/MM/yyyy hh:mm:sstt");
			BM.DEPOSITDT = vdate;
			BM.MIS_FILENM = "";
			BM.BCODE = "203";
			int UPLOADLOT = 0;
			int OutStatus = 1;
			string Mobile = "";
			//ImportBankMis(BankModels BM, int UPLOADLOT, out int OutStatus, out string Mobile)
			DataTable dt = objDB2.ImportBankMis(BM, UPLOADLOT, out OutStatus, out Mobile);

			var results = new
			{
				status = OutStatus,
				//vdate= DateTime.Now.ToString("dd/MM/yyyy")

			};
			return Json(results);
		}

		[HttpPost]
		public ActionResult Jqchkduplicacyonregno(string regno, string GroupNM, int hdnid, string hdnyear, string remarks)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			int OutStatus = 0;
			objDB.Findduplicacyonregno(remarks, regno, GroupNM, hdnid, hdnyear, out OutStatus);

			var results = new
			{
				status = OutStatus

			};
			return Json(results);
		}

		//[HttpPost]
		//public ActionResult Jqchkduplicacyonregno(string regno, int hdnid, string hdnyear)
		//{
		//    AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
		//    int OutStatus = 0;
		//    objDB.Findduplicacyonregno(regno, hdnid, hdnyear, out OutStatus);

		//    var results = new
		//    {
		//        status = OutStatus

		//    };
		//    return Json(results);
		//}
		//[HttpPost] 
		//public ActionResult Jqchkduplicacyonregno(string regno, int hdnid, string YEAR)
		//{
		//    AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
		//    int OutStatus = 0;
		//    objDB.Findduplicacyonregno(regno, hdnid, YEAR, out OutStatus);

		//    var results = new
		//    {
		//        status = OutStatus

		//    };
		//    return Json(results);
		//}


		//public ActionResult adminregverifypage()
		//{
		//    try
		//    {
		//        if (Session["UserName"] == null )
		//        {
		//            return RedirectToAction("Index", "Admin");
		//        }

		//        #region Action Assign Method
		//        if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
		//        { ViewBag.IsEdit = 1; }
		//        else
		//        {

		//            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
		//            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
		//            int AdminId = Convert.ToInt32(Session["AdminId"]);
		//            DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
		//            if (aAct.Tables[0].Rows.Count > 0)
		//            {
		//                //ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(61)).Count();                        
		//                ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/ADMIN_REGNO_UPDATE")).Count();
		//            }
		//        }
		//        #endregion Action Assign Method
		//        //if (Session["AdminType"].ToString() != "admin")
		//        //{
		//        //    if (!Session["PAccessRight"].ToString().Contains("Reg"))
		//        //    {
		//        //        return RedirectToAction("Index", "Admin");
		//        //    }
		//        //}
		//        FormCollection frm = new FormCollection();
		//        var itemsch = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
		//    new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
		//        ViewBag.MySch = itemsch.ToList();
		//        ViewBag.SelectedYear = "2015";
		//        string year = "2015";
		//        var itemyear = new SelectList(new[] { new { ID = "2015", Name = "2015" }, new { ID = "2014", Name = "2014" }, }, "ID", "Name", 1);
		//        ViewBag.MyYear = itemyear.ToList();
		//        var itemschcode = new SelectList(new[] { new { ID = "1", Name = "SCHOOL CODE" } }, "ID", "Name", 1);
		//        ViewBag.MySchcode = itemschcode.ToList();

		//        var itemschform = new SelectList(new[] { new { ID = "1", Name = "N1" }, new { ID = "2", Name = "N2" }, new { ID = "3", Name = "N3" }, new { ID = "4", Name = "E1" }, new { ID = "5", Name = "E2" } }, "ID", "Name", 1);
		//        ViewBag.MyForm = itemschform.ToList();

		//        AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
		//        AdminModels am = new AdminModels();
		//        if (ModelState.IsValid)
		//        {
		//            string Search = string.Empty;
		//            string schlid = frm["TotalSearchString"];
		//            string srch = Convert.ToString(Session["Search"]);
		//            if (srch != null && srch != "")
		//            {
		//                //frm["TotalSearchString"] = Session["txtSchoolcode"].ToString();
		//                string list = Session["ddlSchoolcode"].ToString();
		//                if (list == "1")
		//                {
		//                    frm["totalcountlist"] = Session["ddlSchoolcode"].ToString();
		//                    ViewBag.SelectedItemcode = frm["totalcountlist"];
		//                    am.TotalSearchString = Session["txtSchoolcode"].ToString();

		//                }
		//                Search = Session["Search"].ToString();
		//                am.StoreAllData = objDB.GetSchoolRecordsSearch(Search, year);
		//                if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
		//                {
		//                    ViewBag.Message = "Record Not Found";
		//                    ViewBag.TotalCount = 0;
		//                    return View();
		//                }
		//                else
		//                {
		//                    // Session["Search"] = string.Empty;
		//                    ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
		//                    return View(am);
		//                }
		//            }
		//            if (Session["SCHL"] != null)
		//            {
		//                schlid = Session["SCHL"].ToString();
		//            }
		//            else
		//            {
		//                ViewBag.Message = "Record Not Found";
		//                return View(am);
		//            }
		//            Search = "schl='" + schlid + "' ";
		//            am.StoreAllData = objDB.GetSchoolRecordsSearch(Search, year);
		//            if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
		//            {
		//                ViewBag.Message = "Record Not Found";
		//                ViewBag.TotalCount = 0;
		//                return View();
		//            }
		//            else
		//            {
		//                ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
		//                return View(am);
		//            }
		//        }
		//        else
		//        {
		//            return adminregverifypage();
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        ////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
		//        //return RedirectToAction("Logout", "Login");
		//        return View();
		//    }
		//}

		//[HttpPost]
		//public ActionResult adminregverifypage(FormCollection frm, string Year)
		//{
		//    try
		//    {
		//        if (Session["UserName"] == null )
		//        {
		//            return RedirectToAction("Index", "Admin");
		//        }
		//        #region Action Assign Method
		//        if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
		//        { ViewBag.IsEdit = 1; }
		//        else
		//        {

		//            string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
		//            string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
		//            int AdminId = Convert.ToInt32(Session["AdminId"]);
		//            DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
		//            if (aAct.Tables[0].Rows.Count > 0)
		//            {
		//                //ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(61)).Count();                        
		//                ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/ADMIN_REGNO_UPDATE")).Count();
		//            }
		//        }
		//        #endregion Action Assign Method

		//        var itemsch = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
		//    new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
		//        ViewBag.MySch = itemsch.ToList();
		//        ViewBag.SelectedYear = Year;
		//        var itemyear = new SelectList(new[] { new { ID = "2015", Name = "2015" }, new { ID = "2014", Name = "2014" }, }, "ID", "Name", 1);
		//        ViewBag.MyYear = itemyear.ToList();
		//        var itemschcode = new SelectList(new[] { new { ID = "1", Name = "SCHOOL CODE" } }, "ID", "Name", 1);
		//        ViewBag.MySchcode = itemschcode.ToList();

		//        var itemschform = new SelectList(new[] { new { ID = "1", Name = "N1" }, new { ID = "2", Name = "N2" }, new { ID = "3", Name = "N3" }, new { ID = "4", Name = "E1" }, new { ID = "5", Name = "E2" } }, "ID", "Name", 1);
		//        ViewBag.MyForm = itemschform.ToList();

		//        AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
		//        AdminModels am = new AdminModels();
		//        if (ModelState.IsValid)
		//        {
		//            string Search = string.Empty;
		//            string schlid = "";
		//            string SelTotalItem = frm["totalcountlist"];
		//            string TotalSearchString = Convert.ToString(frm["TotalSearchString"]);
		//            ViewBag.SelectedForm = frm["SelForm"].ToString();
		//            am.TotalSearchString = TotalSearchString;
		//            if (frm["totalcountlist"].ToString() != "" && TotalSearchString != "" && TotalSearchString != null)
		//            {
		//                if (frm["SelForm"].ToString() != "")
		//                {
		//                    string FormName = frm["SelForm"].ToString();
		//                    if (FormName == "1") { FormName = "N1"; }
		//                    else if (FormName == "2") { FormName = "N2"; }
		//                    else if (FormName == "3") { FormName = "N3"; }
		//                    else if (FormName == "4") { FormName = "E1"; }
		//                    else if (FormName == "5") { FormName = "E2"; }
		//                    ViewBag.SelectedItemcode = frm["totalcountlist"];
		//                    schlid = frm["TotalSearchString"];
		//                    Search = "schl='" + schlid + "' and FORM='" + FormName + "'";
		//                }
		//                else
		//                {
		//                    ViewBag.SelectedItemcode = frm["totalcountlist"];
		//                    schlid = frm["TotalSearchString"];
		//                    Search = "schl='" + schlid + "' and FORM in ('E1','E2','N1','N2','N3') ";
		//                }

		//            }
		//            else if (TotalSearchString != "")
		//            {
		//                Search = "schl like '%" + schlid + "%' ";
		//            }

		//            if (frm["SelList"] != "")
		//            {
		//                ViewBag.SelectedItem = frm["SelList"];
		//                int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());
		//                if (frm["SearchByString"] != "" && frm["SearchByString"] != null)
		//                {
		//                    if (SelValueSch == 1)
		//                    { Search += " and id='" + frm["SearchByString"].ToString() + "'"; }
		//                    else if (SelValueSch == 2)
		//                    { Search += " and  RegNo like '%" + frm["SearchByString"].ToString() + "%'"; }
		//                    else if (SelValueSch == 3)
		//                    { Search += " and  name like '%" + frm["SearchByString"].ToString() + "%'"; }
		//                    else if (SelValueSch == 4)
		//                    { Search += " and  fname  like '%" + frm["SearchByString"].ToString() + "%'"; }
		//                    else if (SelValueSch == 5)
		//                    { Search += " and mname like '%" + frm["SearchByString"].ToString() + "%'"; }
		//                    else if (SelValueSch == 6)
		//                    { Search += " and DOB='" + frm["SearchByString"].ToString() + "'"; }
		//                }


		//            }

		//            am.SearchResult = Search;
		//            am.StoreAllData = objDB.GetSchoolRecordsSearch(Search, Year);
		//            if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
		//            {
		//                ViewBag.Message = "Record Not Found";
		//                ViewBag.TotalCount = 0;
		//                return View();
		//            }
		//            else
		//            {
		//                Session["Search"] = Search.ToString();
		//                Session["txtSchoolcode"] = frm["TotalSearchString"];
		//                Session["ddlSchoolcode"] = frm["totalcountlist"];
		//                ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
		//                return View(am);
		//            }
		//        }
		//        else
		//        {
		//            return adminresultpage();
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        ////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
		//        //return RedirectToAction("Logout", "Login");
		//        return View();
		//    }
		//}

		#region start admin reg verify
		public ActionResult adminregverifypage()
		{
			try
			{
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}

				#region Action Assign Method
				if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{ ViewBag.IsEdit = 1; }
				else
				{

					string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
					string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
					int AdminId = Convert.ToInt32(Session["AdminId"]);
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						//ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(61)).Count();                        
						ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/ADMIN_REGNO_UPDATE")).Count();
					}
				}
				#endregion Action Assign Method



				FormCollection frm = new FormCollection();
				var itemsch = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
				new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
				ViewBag.MySch = itemsch.ToList();
				ViewBag.SelectedYear = "2020";
				string year = "2020";

				var itemyear = new SelectList(new[] { new { ID = "2020", Name = "2020" }, new { ID = "2019", Name = "2019" }, new { ID = "2018", Name = "2018" }, new { ID = "2017", Name = "2017" }, new { ID = "2016", Name = "2016" }, new { ID = "2015", Name = "2015" }, new { ID = "2014", Name = "2014" }, }, "ID", "Name", 1);
				ViewBag.MyYear = itemyear.ToList();
				var itemschcode = new SelectList(new[] { new { ID = "1", Name = "SCHOOL CODE" } }, "ID", "Name", 1);
				ViewBag.MySchcode = itemschcode.ToList();

				//var itemschform = new SelectList(new[] { new { ID = "1", Name = "N1" }, new { ID = "2", Name = "N2" }, new { ID = "3", Name = "N3" }, new { ID = "4", Name = "E1" }, new { ID = "5", Name = "E2" } }, "ID", "Name", 1);
				//ViewBag.MyForm = itemschform.ToList();
				var itemschform = new SelectList(new[] { new { ID = "1", Name = "N1" }, new { ID = "2", Name = "N2" }, new { ID = "3", Name = "N3" },
					new { ID = "4", Name = "M1" }, new { ID = "5", Name = "M2" },new { ID = "6", Name = "E1" }, new { ID = "7", Name = "E2" },new { ID = "8", Name = "T1" }, new { ID = "9", Name = "T2" } }, "ID", "Name", 1);
				ViewBag.MyForm = itemschform.ToList();

				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				if (ModelState.IsValid)
				{
					string Search = string.Empty;
					string schlid = frm["TotalSearchString"];
					string srch = Convert.ToString(Session["Search"]);
					if (srch != null && srch != "")
					{
						//frm["TotalSearchString"] = Session["txtSchoolcode"].ToString();
						string list = Session["ddlSchoolcode"].ToString();
						if (list == "1")
						{
							frm["totalcountlist"] = Session["ddlSchoolcode"].ToString();
							ViewBag.SelectedItemcode = frm["totalcountlist"];
							am.TotalSearchString = Session["txtSchoolcode"].ToString();

						}
						Search = Session["Search"].ToString();
						am.StoreAllData = objDB.GetSchoolRecordsSearch(Search, year);
						if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.Message = "Record Not Found";
							ViewBag.TotalCount = 0;
							return View();
						}
						else
						{
							// Session["Search"] = string.Empty;
							ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
							return View(am);
						}
					}
					if (Session["SCHL"] != null)
					{
						schlid = Session["SCHL"].ToString();
					}
					else
					{
						ViewBag.Message = "Record Not Found";
						return View(am);
					}
					Search = "schl='" + schlid + "' ";
					am.StoreAllData = objDB.GetSchoolRecordsSearch(Search, year);
					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
						return View(am);
					}
				}
				else
				{
					return adminregverifypage();
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}

		[HttpPost]
		public ActionResult adminregverifypage(FormCollection frm, string Year)
		{
			try
			{
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				#region Action Assign Method
				if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{ ViewBag.IsEdit = 1; }
				else
				{

					string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
					string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
					int AdminId = Convert.ToInt32(Session["AdminId"]);
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						//ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(61)).Count();                        
						ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/ADMIN_REGNO_UPDATE")).Count();
					}
				}
				#endregion Action Assign Method

				var itemsch = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
			new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="DOB"},}, "ID", "Name", 1);
				ViewBag.MySch = itemsch.ToList();


				ViewBag.SelectedYear = Year;
				var itemyear = new SelectList(new[] { new { ID = "2020", Name = "2020" }, new { ID = "2019", Name = "2019" }, new { ID = "2018", Name = "2018" }, new { ID = "2017", Name = "2017" }, new { ID = "2016", Name = "2016" }, new { ID = "2015", Name = "2015" }, new { ID = "2014", Name = "2014" }, }, "ID", "Name", 1);
				ViewBag.MyYear = itemyear.ToList();
				var itemschcode = new SelectList(new[] { new { ID = "1", Name = "SCHOOL CODE" } }, "ID", "Name", 1);
				ViewBag.MySchcode = itemschcode.ToList();

				ViewBag.SelectedItemcode = frm["totalcountlist"];
				//var itemschform = new SelectList(new[] { new { ID = "1", Name = "N1" }, new { ID = "2", Name = "N2" }, new { ID = "3", Name = "N3" }, new { ID = "4", Name = "E1" }, new { ID = "5", Name = "E2" } }, "ID", "Name", 1);
				//ViewBag.MyForm = itemschform.ToList();

				var itemschform = new SelectList(new[] { new { ID = "1", Name = "N1" }, new { ID = "2", Name = "N2" }, new { ID = "3", Name = "N3" },
					new { ID = "4", Name = "M1" }, new { ID = "5", Name = "M2" },new { ID = "6", Name = "E1" }, new { ID = "7", Name = "E2" },new { ID = "8", Name = "T1" }, new { ID = "9", Name = "T2" } }, "ID", "Name", 1);
				ViewBag.MyForm = itemschform.ToList();

				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				if (ModelState.IsValid)
				{
					string Search = string.Empty;
					string schlid = "";
					string SelTotalItem = frm["totalcountlist"];
					string TotalSearchString = Convert.ToString(frm["TotalSearchString"]);
					ViewBag.SelectedForm = frm["SelForm"].ToString();
					string YR = frm["YEAR"].ToString();
					am.TotalSearchString = TotalSearchString;
					if (frm["totalcountlist"].ToString() != "" && TotalSearchString != "" && TotalSearchString != null)
					{
						if (frm["SelForm"].ToString() != "")
						{
							string FormName = frm["SelForm"].ToString();
							if (FormName == "1") { FormName = "N1"; }
							else if (FormName == "2") { FormName = "N2"; }
							else if (FormName == "3") { FormName = "N3"; }
							else if (FormName == "4") { FormName = "M1"; }
							else if (FormName == "5") { FormName = "M2"; }
							else if (FormName == "6") { FormName = "E1"; }
							else if (FormName == "7") { FormName = "E2"; }
							else if (FormName == "8") { FormName = "T1"; }
							else if (FormName == "9") { FormName = "T2"; }
							schlid = frm["TotalSearchString"];
							if (Convert.ToInt32(YR) >= 2016)
							{
								Search = "schl='" + schlid + "' and Form_Name='" + FormName + "'";
							}
							else
							{
								Search = "schl='" + schlid + "' and FORM='" + FormName + "'";
							}

						}
						else
						{
							ViewBag.SelectedItemcode = frm["totalcountlist"];
							schlid = frm["TotalSearchString"];
							if (Convert.ToInt32(YR) >= 2016)
							{
								Search = "schl='" + schlid + "' and Form_Name in ('E1','E2','N1','N2','N3','M1','M2','T1','T2') ";
							}
							else
							{
								Search = "schl='" + schlid + "' and FORM in ('E1','E2','N1','N2','N3','M1','M2','T1','T2') ";
							}

						}

					}
					else if (TotalSearchString != "")
					{
						Search = "schl like '%" + schlid + "%' ";
					}

					if (frm["SelList"] != "")
					{
						ViewBag.SelectedItem = frm["SelList"];
						int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());
						if (frm["SearchByString"] != "" && frm["SearchByString"] != null)
						{
							if (SelValueSch == 1)
							{
								if (Convert.ToInt32(YR) >= 2016)
								{
									Search += " and STD_ID='" + frm["SearchByString"].ToString() + "'";
								}
								else
								{
									Search += " and id='" + frm["SearchByString"].ToString() + "'";
								}

							}
							else if (SelValueSch == 2)
							{
								if (Convert.ToInt32(YR) >= 2016)
								{
									Search += " and  Registration_num like '%" + frm["SearchByString"].ToString() + "%'";
								}
								else
								{
									Search += " and  RegNo like '%" + frm["SearchByString"].ToString() + "%'";
								}

							}
							else if (SelValueSch == 3)
							{
								if (Convert.ToInt32(YR) >= 2016)
								{
									Search += " and  Candi_name like '%" + frm["SearchByString"].ToString() + "%'";
								}
								else
								{
									Search += " and  name like '%" + frm["SearchByString"].ToString() + "%'";
								}

							}
							else if (SelValueSch == 4)
							{
								if (Convert.ToInt32(YR) >= 2016)
								{
									Search += " and  Father_name  like '%" + frm["SearchByString"].ToString() + "%'";
								}
								else
								{
									Search += " and  fname  like '%" + frm["SearchByString"].ToString() + "%'";
								}

							}
							else if (SelValueSch == 5)
							{
								if (Convert.ToInt32(YR) >= 2016)
								{
									Search += " and  Mother_name  like '%" + frm["SearchByString"].ToString() + "%'";
								}
								else
								{
									Search += " and  mname  like '%" + frm["SearchByString"].ToString() + "%'";
								}
							}
							else if (SelValueSch == 6)
							{ Search += " and DOB='" + frm["SearchByString"].ToString() + "'"; }
						}


					}

					am.SearchResult = Search;
					am.StoreAllData = objDB.GetSchoolRecordsSearch(Search, Year);
					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View(am);
					}
					else
					{
						Session["Search"] = Search.ToString();
						Session["txtSchoolcode"] = frm["TotalSearchString"];
						Session["ddlSchoolcode"] = frm["totalcountlist"];
						ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
						return View(am);
					}
				}
				else
				{
					return adminresultpage();
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}
		#endregion end admin reg verify



		public ActionResult Admin_Regno_Update(string id, string year)
		{
			try
			{
				AdminModels am = new AdminModels();
				try
				{
					id = encrypt.QueryStringModule.Decrypt(id);
					year = encrypt.QueryStringModule.Decrypt(year);
					ViewBag.year = year;
				}
				catch (Exception)
				{
					Session["Search"] = null;
					return RedirectToAction("Index", "Admin");
				}

				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["AdminType"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Reg"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}

				string stdid = id;
				if (stdid != null)
				{
					AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();

					string Search = string.Empty;
					// Search = "ID='" + stdid + "' ";
					if (Convert.ToInt32(year) >= 2016)
					{
						Search = "STD_ID='" + stdid + "' ";
					}
					else
					{
						Search = "ID='" + stdid + "' ";
					}
					am.StoreAllData = objDB.GetSchoolRecordsSearch(Search, year);//GetSchoolRecordsSearchD
					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
						am.Candi_Name = am.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
						am.Father_Name = am.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
						am.Mother_Name = am.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();
						am.DOB = am.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
						am.Gender = am.StoreAllData.Tables[0].Rows[0]["SEX"].ToString();
						am.REGNO = am.StoreAllData.Tables[0].Rows[0]["REGNO"].ToString();
						am.SdtID = am.StoreAllData.Tables[0].Rows[0]["ID"].ToString();
						am.FormName = am.StoreAllData.Tables[0].Rows[0]["FORM"].ToString();
						Session["SCHL"] = am.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
						am.EXAM = am.StoreAllData.Tables[0].Rows[0]["Group_name"].ToString().Trim();
						am.Remarks = am.StoreAllData.Tables[0].Rows[0]["Remarks"].ToString().Trim();
						//  ViewBag.MyGroup1 = objCommon.GroupName();
						List<SelectListItem> MyGroupList = objCommon.GroupName();
						DataTable dtAssignSubject = objCommon.GetAssignSubjectBySchool(Session["SCHL"].ToString());//GetAssignSubjectBySchool
						if (dtAssignSubject != null && dtAssignSubject.Rows.Count > 0)
						{
							ViewBag.MyGroup1 = objCommon.GetSubjectsBySchool(dtAssignSubject, MyGroupList);
						}
						return View(am);
					}
				}
				else
				{

					return Admin_Result_Update(id, year);
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}
		//--------------------------------Examination Challan Verification-------------------------------------


		public ActionResult ExamChallanVerify(string id)
		{
			try
			{
				ViewBag.date = DateTime.Now.ToString("dd/mm/yyyy");
				ViewBag.schl = id;
				ChallanMasterModel CM = new ChallanMasterModel();
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Logout", "Admin");
				}
				else
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
				}

				string schl = id;

				DataSet ds = objDB1.GetFinalPrintChallanExam(schl);//School Cpde
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
					if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "0")
					{
						//Challan Generated
						CM.Action = "0";
					}
					else if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "1")
					{                                       //Downloaded by Bank 
						CM.Action = "1";
					}
					else if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "2")
					{
						//HttpRuntime.Cache.Remove("FinalPrintDataSet");
						//Challan Verified
						CM.Action = "2";
					}
				}
				//CM.LOT = Convert.ToInt32(ds.Tables[0].Rows[0]["LOT"].ToString());
				//CM.BANK = ds.Tables[0].Rows[0]["BANK"].ToString();
				//CM.CHALLANID = ds.Tables[0].Rows[0]["CHALLANID"].ToString();
				//CM.CHLNDATE = ds.Tables[0].Rows[0]["ChallanDate"].ToString();
				//CM.CHLNVDATE = ds.Tables[0].Rows[0]["ChallanVDate"].ToString();             
				//CM.FEECAT = ds.Tables[0].Rows[0]["Status"].ToString();                 
				//CM.SchoolName = ds.Tables[0].Rows[0]["SchoolName"].ToString();              
				return View(CM);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}
		[HttpPost]
		public ActionResult ExamChallanVerify(string txtschl, string dee)
		{
			try
			{
				ViewBag.date = DateTime.Now.ToString("dd/mm/yyyy");
				ViewBag.schl = txtschl;
				ChallanMasterModel CM = new ChallanMasterModel();
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Logout", "Admin");
				}
				string schl = txtschl;

				DataSet ds = objDB1.GetFinalPrintChallanExam(schl);//School Cpde
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
					if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "0")
					{
						//Challan Generated
						CM.Action = "0";
					}
					else if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "1")
					{                                       //Downloaded by Bank 
						CM.Action = "1";
					}
					else if (ds.Tables[0].Rows[0]["StatusNumber"].ToString() == "2")
					{
						//HttpRuntime.Cache.Remove("FinalPrintDataSet");
						//Challan Verified
						CM.Action = "2";
					}
				}

				return View(CM);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}
		[HttpPost]
		public ActionResult ExamJqverify(string challanno, string totfee, string refno, string vdate)
		{
			if (Session["UserName"] == null)
			{
				return RedirectToAction("Logout", "Admin");
			}
			BankModels BM = new BankModels();
			//DateTime fromDateValue;
			//if (DateTime.TryParseExact(DateTime.Now.ToString(), "dd/MM/yyyy hh:mm:sstt", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateValue))
			//{
			//}
			BM.CHALLANID = challanno;
			BM.TOTFEE = float.Parse(totfee);
			BM.BRCODE = "203";
			BM.BRANCH = "PSEB";
			BM.J_REF_NO = refno;
			//BM.DEPOSITDT = DateTime.Now.ToString("dd/MM/yyyy hh:mm:sstt");
			BM.DEPOSITDT = vdate;
			BM.MIS_FILENM = "";
			BM.BCODE = "203";
			int UPLOADLOT = 0;
			int OutStatus = 1;
			string Mobile = "";
			//ImportBankMis(BankModels BM, int UPLOADLOT, out int OutStatus, out string Mobile)
			DataTable dt = objDB2.ImportBankMis(BM, UPLOADLOT, out OutStatus, out Mobile);

			var results = new
			{
				status = OutStatus,
				//vdate= DateTime.Now.ToString("dd/MM/yyyy")

			};
			return Json(results);
		}

		//------------------------------------End Examination Challan Verification-------------------------------------
		/// <summary>
		/// Summary Performa Report in Admin Login
		/// </summary>
		/// <returns> Date wise Summary Performa Report</returns>
		public ActionResult SummaryPerformaReport()
		{
			try
			{
				if (Session["UserName"] == null)
				{ return RedirectToAction("Index", "Admin"); }
				//if (Session["AdminType"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Reg"))
				//    { return RedirectToAction("Index", "Admin"); }
				//}

				FormCollection frm = new FormCollection();
				//  AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				if (ModelState.IsValid)
				{
					am.StoreAllData = objDB.GetSummaryPerformaReport();

					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
						return View(am);
					}
				}
				else
				{
					return View();
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Admin");
				return View();
			}
		}

		#region Firm School CCE Grading Report Download
		public ActionResult FirmSchoolCCEGrading(int? page)
		{
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}
				FormCollection frc = new FormCollection();
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();

				DataSet result = objDB.SelectDistCCEGrading(Session["UserName"].ToString());
				ViewBag.MyExamDist = result.Tables[0];// Edit Mode for dislaying message after saving storing output.
				List<SelectListItem> items1 = new List<SelectListItem>();
				foreach (System.Data.DataRow dr in ViewBag.MyExamDist.Rows)
				{
					items1.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
				}
				ViewBag.MyExamDist = new SelectList(items1, "Value", "Text");

				//if (ModelState.IsValid)
				//{
				//    string Search = string.Empty;
				//    string CrType = "2";
				//    Search = "";
				//    //------ Paging Srt
				//    int pageIndex = 1;
				//    pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				//    ViewBag.pagesize = pageIndex;
				//    //string Catg = CrType;                        

				//    //---- Paging end
				//    string FirmNM = Session["UserName"].ToString();
				//    am.StoreAllData = objDB.FirmSchoolCCEGrading(Search, FirmNM, CrType, pageIndex);
				//    ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;

				//    if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				//    {
				//        ViewBag.Message = "Record Not Found";
				//        ViewBag.TotalCount = 0;
				//        return View(am);
				//    }
				//    else
				//    {
				//        ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
				//        ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
				//        int tp = Convert.ToInt32(ViewBag.TotalCount);
				//        int pn = tp / 30;
				//        int cal = 30 * pn;
				//        int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
				//        if (res >= 1)
				//            ViewBag.pn = pn + 1;
				//        else
				//            ViewBag.pn = pn;

				//        return View(am);
				//    }
				//}
				//ViewBag.Message = "Record Not Found";
				//ViewBag.TotalCount = 0;
				return View();
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}
		[HttpPost]
		public ActionResult FirmSchoolCCEGrading(FormCollection frc, int? page, string cmd)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}

				DataSet result = objDB.SelectDistCCEGrading(Session["UserName"].ToString());
				ViewBag.MyExamDist = result.Tables[0];// Edit Mode for dislaying message after saving storing output.
				List<SelectListItem> items1 = new List<SelectListItem>();
				foreach (System.Data.DataRow dr in ViewBag.MyExamDist.Rows)
				{
					items1.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
				}
				ViewBag.MyExamDist = new SelectList(items1, "Value", "Text");

				if (ModelState.IsValid && frc["SelExamDist"].ToString() != "")
				{
					string Search = string.Empty;
					string CrType = "2";
					Search = "";
					am.SchlCode = frc["SchlCode"].ToString();
					am.SdtID = frc["SdtID"].ToString().Trim();
					am.SelExamDist = frc["SelExamDist"].ToString();
					//am.ROLLexam = frc["ROLLexam"].ToString();
					//am.SelExamDist = frc["SelExamDist"].ToString();
					if (am.SchlCode.ToString().Trim() != "" && am.SchlCode != null)
					{
						Search += " and a.SCHL in('" + am.SchlCode.ToString().Trim() + "')";
					}
					if (am.SdtID.ToString().Trim() != "" && am.SdtID != null)
					{
						Search += " and a.std_id in('" + am.SdtID.ToString().Trim() + "')";
					}
					//if (am.ROLLexam.ToString().Trim() != "" && am.ROLLexam != null)
					//{
					//    Search += " and exm.Roll in('" + am.ROLLexam.ToString().Trim() + "')";
					//}
					if (am.SelExamDist.ToString().Trim() != "" && am.SelExamDist != null)
					{
						Search += " and c.dist in('" + am.SelExamDist.ToString().Trim() + "')";
					}

					//------ Paging Srt
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;
					//string Catg = CrType;                        

					//---- Paging end
					string FirmNM = Session["UserName"].ToString();
					am.StoreAllData = objDB.FirmSchoolCCEGrading(Search, FirmNM, CrType, pageIndex);
					ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
					//----------------------

					//}



					if (ViewBag.TotalCountP > 0 && cmd == "Download CCE Grading")
					{
						DataSet ds = objDB.FirmSchoolCCEGradingDownload(Search, FirmNM);
						DataTable dt = ds.Tables[0];
						string fname = DateTime.Now.ToString("ddMMyyyyHHmm");
						Response.Clear();
						Response.Buffer = true;
						Response.Charset = "";
						Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
						Response.AddHeader("content-disposition", "attachment;filename=CCEGrading" + fname + ".xlsx");
						using (MemoryStream MyMemoryStream = new MemoryStream())
						{
							XLWorkbook wb = new XLWorkbook();

							var WS = wb.Worksheets.Add(dt, "CCEGrading" + fname);
							WS.Tables.FirstOrDefault().ShowAutoFilter = false;
							wb.SaveAs(MyMemoryStream);
							MyMemoryStream.WriteTo(Response.OutputStream);
							WS.AutoFilter.Enabled = false;
							Response.Flush();
							Response.End();
						}
					}
					//----------------


					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View(am);
					}
					else
					{
						ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
						ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
						int tp = Convert.ToInt32(ViewBag.TotalCount);
						int pn = tp / 30;
						int cal = 30 * pn;
						int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
						if (res >= 1)
							ViewBag.pn = pn + 1;
						else
							ViewBag.pn = pn;

						return View(am);
					}
				}
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
				return View(am);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View(am);
			}
		}
		#endregion Firm School CCE Grading Report Download

		#region Begin Admin Result Update MIS
		public ActionResult AdminResultUpdateMIS()
		{
			try
			{
				if (Session["UserName"] == null && (Session["UserName"].ToString().ToUpper() != "ADMIN" || Session["UserName"].ToString().ToUpper() != "CREA" || Session["UserName"].ToString().ToUpper() != "DATA" || Session["UserName"].ToString().ToUpper() != "PERF" || Session["UserName"].ToString().ToUpper() != "SAI"))
				{
					return RedirectToAction("Index", "Admin");
				}
				else
				{
					return View();
				}
			}
			catch (Exception)
			{

				return RedirectToAction("Index", "Admin");
			}

		}

		[HttpPost]
		public ActionResult AdminResultUpdateMIS(PunAddModels AM) // HttpPostedFileBase file
		{
			try
			{
				// firm login // dist 
				if (Session["UserName"] == null && (Session["UserName"].ToString().ToUpper() != "ADMIN" || Session["UserName"].ToString().ToUpper() != "CREA" || Session["UserName"].ToString().ToUpper() != "DATA" || Session["UserName"].ToString().ToUpper() != "PERF" || Session["UserName"].ToString().ToUpper() != "SAI"))
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
						string fileName1 = "FirmResultMIS_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

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
							using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
							{
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
								string query = string.Format("Select * from [{0}]", excelSheets[0]);
								using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
								{
									dataAdapter.Fill(ds);
								}
							}
							string UserNM = Session["UserName"].ToString();
							switch (UserNM)
							{
								case "CREA": UserNM = "CIPL"; break;
								case "SAI": UserNM = "SAI"; break;
								case "DATA": UserNM = "DATA"; break;
								case "PERF": UserNM = "PERF"; break;
							}
							DataTable dt1 = ds.Tables[0];
							dt1.AcceptChanges();
							// Get Unique and  noe empty records
							dt1 = dt1.AsEnumerable().GroupBy(x => x.Field<string>("ROLL")).Select(g => g.First()).Where(r => r.ItemArray[1].ToString() != "").CopyToDataTable();

							DataTable dtexport;

							string CheckMis = objDB.CheckResultMisExcel(dt1, UserNM, out dtexport);
							if (CheckMis == "")
							{
								if (dt1.Columns.Contains("Status"))
								{
									dt1.Columns.Remove("Status");
								}

								string Result1 = "";
								string OutError = "";
								DataTable dtResult = objDB.AdminResultUpdateMIS(dt1, AdminId, out OutError);// OutStatus mobile
								if (OutError == "1")
								{
									ViewBag.Message = "File Uploaded Successfully";
									ViewData["Result"] = "1";
								}
								else
								{
									ViewBag.Message = "File Not Uploaded Successfully : " + OutError.ToString();
									ViewData["Result"] = "0";
								}
								return View();
							}
							else
							{
								if (dtexport != null)
								{
									ExportDataFromDataTable(dtexport, "Error_ResultUpdate");
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
				//////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View();
			}
			return View();
		}
		#endregion  Admin Result Update MIS


		#region Final Exam Result 2017
		public ActionResult FinalExamResult(FormCollection frm, int? page)
		{
			try
			{
				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;

				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				#region Action Assign Method
				if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{ ViewBag.IsEdit = 1; ViewBag.IsFinalSubmit = 1; }
				else
				{

					string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
					string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
					int AdminId = Convert.ToInt32(Session["AdminId"]);
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						//ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(58)).Count();                        
						//ViewBag.IsFinalSubmit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(59)).Count();

						//  ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/ADMIN_RESULT_UPDATE")).Count();
						//  ViewBag.IsFinalSubmit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/FinalExamResult")).Count();

					}
				}
				#endregion Action Assign Method

				var itemsch = new SelectList(new[]{new {ID="1",Name="By RollNo"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
			new{ID="4",Name="Father's Name"},}, "ID", "Name", 1);
				ViewBag.MySch = itemsch.ToList();

				// Year 
				var itemyear = new SelectList(new[] { new { ID = "2017", Name = "2017" }, }, "ID", "Name", 1);
				ViewBag.MyYear = itemyear.ToList();
				string year = ViewBag.SelectedYear;

				var itemschcode = new SelectList(new[] { new { ID = "1", Name = "SCHOOL CODE" } }, "ID", "Name", 1);
				ViewBag.MySchcode = itemschcode.ToList();

				// Class
				var itemschClass = new SelectList(new[] { new { ID = "2", Name = "Matric" }, new { ID = "4", Name = "Senior" } }, "ID", "Name", 1);
				ViewBag.MyClass = itemschClass.ToList();

				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				if (ModelState.IsValid)
				{
					string Search = string.Empty;
					string schlid = frm["TotalSearchString"];
					string srch = Convert.ToString(Session["Search"]);

					if (srch != null && srch != "")
					{
						Search = Session["Search"].ToString();
						year = Session["Year"].ToString();
						ViewBag.SelectedYear = year;
						am.TotalSearchString = Session["scode"].ToString();
						ViewBag.SelectedItemcode = "1";
						ViewBag.SelectedForm = Session["SelClass"].ToString();
						ViewBag.SelectedItem = Session["SelList"].ToString();
						frm["SearchByString"] = Session["SearchByString"].ToString();

						//am.StoreAllData = objDB.GetSchoolRecordsSearch(Search, year);
						am.StoreAllData = objDB.GetFinalExamResult(Convert.ToInt32(year), 1, Search, pageIndex, 20);
						if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.Message = "Record Not Found";
							ViewBag.LastPageIndex = 0;
							ViewBag.TotalCount = 0;
							ViewBag.TotalCount1 = 0;
							return View();
						}
						else
						{
							ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
							int count = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
							ViewBag.TotalCount1 = count;
							int tp = Convert.ToInt32(count);
							int pn = tp / 20;
							int cal = 20 * pn;
							int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
							if (res >= 1)
								ViewBag.pn = pn + 1;
							else
								ViewBag.pn = pn;
							return View(am);

						}
					}
				}
				else
				{
					return View(am);
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
			return View();
		}
		[HttpPost]
		public ActionResult FinalExamResult(FormCollection frm, string Year, int? page)
		{
			int pageIndex = 1;
			pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
			ViewBag.pagesize = pageIndex;
			if (Session["UserName"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}

			#region Action Assign Method
			if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
			{ ViewBag.IsEdit = 1; ViewBag.IsFinalSubmit = 1; }
			else
			{

				string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
				string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
				if (aAct.Tables[0].Rows.Count > 0)
				{
					//ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(58)).Count();                        
					//ViewBag.IsFinalSubmit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<int>("MenuId").Equals(59)).Count();

					// ViewBag.IsEdit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/ADMIN_RESULT_UPDATE")).Count();
					// ViewBag.IsFinalSubmit = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/FinalExamResult")).Count();

				}
			}
			#endregion Action Assign Method

			var itemsch = new SelectList(new[]{new {ID="1",Name="By RollNo"},new {ID="2",Name="REGNO"},new{ID="3",Name="Candidate Name"},
			new{ID="4",Name="Father's Name"},}, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();

			// Year 
			var itemyear = new SelectList(new[] { new { ID = "2017", Name = "2017" }, }, "ID", "Name", 1);
			ViewBag.MyYear = itemyear.ToList();
			string year = ViewBag.SelectedYear;

			var itemschcode = new SelectList(new[] { new { ID = "1", Name = "SCHOOL CODE" } }, "ID", "Name", 1);
			ViewBag.MySchcode = itemschcode.ToList();

			// Class
			var itemschClass = new SelectList(new[] { new { ID = "2", Name = "Matric" }, new { ID = "4", Name = "Senior" } }, "ID", "Name", 1);
			ViewBag.MyClass = itemschClass.ToList();

			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels am = new AdminModels();
			if (ModelState.IsValid)
			{
				string Search = string.Empty;
				Search = "a.schl like '%' ";
				string schlid = "";
				string TotalSearchString = Convert.ToString(frm["TotalSearchString"]);
				am.TotalSearchString = TotalSearchString;

				if (TotalSearchString != "")
				{
					schlid = frm["TotalSearchString"];
					if (schlid != "")
					{ Search += " and a.schl='" + schlid + "' "; }
				}

				if (frm["SelClass"].ToString() != "")
				{
					string ClassName = frm["SelClass"].ToString();
					ViewBag.SelectedItemcode = frm["totalcountlist"];
					schlid = frm["TotalSearchString"];
					Search += " and a.Class='" + ClassName + "'";
				}

				if (frm["SelList"] != "")
				{
					ViewBag.SelectedItem = frm["SelList"];
					int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());
					if (frm["SearchByString"] != "" && frm["SearchByString"] != null)
					{
						if (SelValueSch == 1)
						{ Search += " and r.Roll='" + frm["SearchByString"].ToString() + "'"; }
						else if (SelValueSch == 2)
						{ Search += " and  a.Registration_Num like '%" + frm["SearchByString"].ToString() + "%'"; }
						else if (SelValueSch == 3)
						{ Search += " and  a.Candi_name like '%" + frm["SearchByString"].ToString() + "%'"; }
						else if (SelValueSch == 4)
						{ Search += " and  a.father_name  like '%" + frm["SearchByString"].ToString() + "%'"; }
						else if (SelValueSch == 5)
						{ Search += " and a.mother_name like '%" + frm["SearchByString"].ToString() + "%'"; }
						else if (SelValueSch == 6)
						{ Search += " and a.DOB='" + frm["SearchByString"].ToString() + "'"; }
					}
				}
				am.SelYear = frm["year"].ToString();
				am.SearchResult = Search;
				am.StoreAllData = objDB.GetFinalExamResult(Convert.ToInt32(Year), 1, Search, pageIndex, 20);
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.LastPageIndex = 0;
					ViewBag.TotalCount = 0;
					ViewBag.TotalCount1 = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
					int count = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
					ViewBag.TotalCount1 = count;
					int tp = Convert.ToInt32(count);
					int pn = tp / 20;
					int cal = 20 * pn;
					int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
					if (res >= 1)
						ViewBag.pn = pn + 1;
					else
						ViewBag.pn = pn;
					return View(am);

				}
			}
			else
			{
				return View(am);
			}
		}

		public ActionResult ViewFinalResult(string id, string Resultyear)
		{
			try
			{
				FinalResultModels am = new FinalResultModels();
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}

				string Roll = id;
				if (Roll != "" && Resultyear != "")
				{

					AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
					am.StoreAllData = objDB.GetExamResultByRollNoNYear(Roll, Convert.ToInt32(Resultyear), 1);
					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View(am);
					}
					else
					{
						ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
						am.ROLL = am.StoreAllData.Tables[0].Rows[0]["ROLL"].ToString();
						am.CENT = am.StoreAllData.Tables[0].Rows[0]["CENT"].ToString();
						am.REGNO = am.StoreAllData.Tables[0].Rows[0]["REGNO"].ToString();
						am.Candi_Name = am.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
						am.Father_Name = am.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
						am.Mother_Name = am.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();
						am.Schl = am.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
						am.SchoolName = am.StoreAllData.Tables[0].Rows[0]["SchoolNameFull"].ToString();
						am.DOB = am.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
						am.Gender = am.StoreAllData.Tables[0].Rows[0]["SEX"].ToString();
						am.Result = am.StoreAllData.Tables[0].Rows[0]["RESULT"].ToString();
						am.TotalMarks = am.StoreAllData.Tables[0].Rows[0]["TOTMAX"].ToString();
						am.ObtainedMarks = am.StoreAllData.Tables[0].Rows[0]["TOTAL"].ToString();
						am.StdId = am.StoreAllData.Tables[0].Rows[0]["ID"].ToString();
						am.FormName = am.StoreAllData.Tables[0].Rows[0]["FORM"].ToString();
						am.DIST = am.StoreAllData.Tables[0].Rows[0]["DIST"].ToString();
						am.SET = am.StoreAllData.Tables[0].Rows[0]["SET"].ToString();
						am.PHONE = am.StoreAllData.Tables[0].Rows[0]["PHONE"].ToString();
						//am.DISTNM = am.StoreAllData.Tables[0].Rows[0]["DISTNM"].ToString();
						ViewBag.EXAM = am.EXAM = am.StoreAllData.Tables[0].Rows[0]["EXAM"].ToString();
						ViewBag.ClassName = am.StoreAllData.Tables[0].Rows[0]["ClassName"].ToString();
						return View(am);

					}
				}
				else
				{
					return RedirectToAction("FinalExamResult", "Admin");
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}

		#endregion Final Exam Result 2017

		#region  Download Pvt PHOTO SIGN
		public ActionResult NewDownloadZIPFilePSPvt()  //download photo and sign compartment student for batch8 
		{
			try
			{
				string UserNM = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
				if (UserNM != "")
				{
					string result = objDB.DownloadZIPFileSPPvt(UserNM);
					ViewBag.Result = result;
					ViewData["Result"] = result;
				}
				return RedirectToAction("DownloadCompartmentData", "Admin");
			}
			catch (Exception)
			{
				return RedirectToAction("DownloadCompartmentData", "Admin");
			}


		}
		public ActionResult DownloadZIPFilePSPvt()
		{
			return View();
		}
		[HttpPost]
		public ActionResult DownloadZIPFilePSPvt(string id, FormCollection frm)
		{
			string UserNM = AbstractLayer.StaticDB.GetFirmName(Session["UserName"].ToString());
			string tblname = "";
			if (UserNM != "")
			{
				//tblname = frm["tblname"].ToString();
				//DownloadZIPFile(string filename,string tablename)
				string result = objDB.DownloadZIPFileSPPvt(UserNM);
				ViewBag.Result = result;
				ViewData["Result"] = result;
			}
			return View();

		}
		#endregion

		#region update ImpDataUpdate 2015 and 2016 file
		public ActionResult ImpDataUpdate()
		{
			try
			{
				if (Session["UserName"] == null && (Session["UserName"].ToString().ToUpper() != "ADMIN"))
				{
					return RedirectToAction("Index", "Admin");
				}
				else
				{
					return View();
				}
			}
			catch (Exception)
			{

				return RedirectToAction("Index", "Admin");
			}

		}
		[HttpPost]
		public ActionResult ImpDataUpdate(PunAddModels AM) // HttpPostedFileBase ImpDataUpdate 2015 and 2016 file
		{
			try
			{
				if (Session["UserName"] == null && (Session["UserName"].ToString().ToUpper() != "ADMIN"))
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
						string fileName1 = "FirmResultMIS_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

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
							using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
							{
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
								string query = string.Format("Select * from [{0}]", excelSheets[0]);
								using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
								{
									dataAdapter.Fill(ds);
								}
							}
							string UserNM = Session["UserName"].ToString();
							switch (UserNM)
							{
								case "CREA": UserNM = "CIPL"; break;
								case "SAI": UserNM = "SAI"; break;
								case "DATA": UserNM = "DATA"; break;
								case "PERF": UserNM = "PERF"; break;
							}
							string CheckMis = objDB.CheckImpDataMisExcel(ds, UserNM);
							if (CheckMis == "")
							{
								DataTable dt1 = ds.Tables[0];
								string Result1 = "";
								int OutStatus = 0;
								DataTable dtResult = objDB.ImpDataUpdate(dt1, AdminId, out OutStatus);// OutStatus mobile
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
				//////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View();
			}
			return View();
		}
		#endregion update ImpDataUpdate 2015 and 2016 file
		#region Pvt 1. Roll & Ecentre Update 2. Error Add and Remove
		public ActionResult PvtRollEcentreUpdate()
		{
			try
			{
				if (Session["UserName"] == null && (Session["UserName"].ToString().ToUpper() != "ADMIN" || Session["UserName"].ToString().ToUpper() != "CREA" || Session["UserName"].ToString().ToUpper() != "DATA" || Session["UserName"].ToString().ToUpper() != "PERF" || Session["UserName"].ToString().ToUpper() != "SAI"))
				{
					return RedirectToAction("Index", "Admin");
				}
				else
				{
					return View();
				}
			}
			catch (Exception)
			{

				return RedirectToAction("Index", "Admin");
			}

		}
		[HttpPost]
		public ActionResult PvtRollEcentreUpdate(PunAddModels AM) // HttpPostedFileBase file
		{
			try
			{
				// firm login // dist 
				if (Session["UserName"] == null && (Session["UserName"].ToString().ToUpper() != "ADMIN" || Session["UserName"].ToString().ToUpper() != "CREA" || Session["UserName"].ToString().ToUpper() != "DATA" || Session["UserName"].ToString().ToUpper() != "PERF" || Session["UserName"].ToString().ToUpper() != "SAI"))
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
						string fileName1 = "FirmResultMIS_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

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
							using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
							{
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
								string query = string.Format("Select * from [{0}]", excelSheets[0]);
								using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
								{
									dataAdapter.Fill(ds);
								}
							}
							string UserNM = Session["UserName"].ToString();
							switch (UserNM)
							{
								case "CREA": UserNM = "CIPL"; break;
								case "SAI": UserNM = "SAI"; break;
								case "DATA": UserNM = "DATA"; break;
								case "PERF": UserNM = "PERF"; break;
							}
							string CheckMis = objDB.CheckRollEcentreMisExcel(ds, UserNM);
							if (CheckMis == "")
							{
								DataTable dt1 = ds.Tables[0];
								string Result1 = "";
								int OutStatus = 0;
								DataTable dtResult = objDB.PvtRollEcentreUpdate(dt1, UserNM, out OutStatus);// OutStatus mobile
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
				//////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View();
			}
			return View();
		}

		public ActionResult PvtErrUpdate()
		{
			try
			{
				if (Session["UserName"] == null && (Session["UserName"].ToString().ToUpper() != "ADMIN" || Session["UserName"].ToString().ToUpper() != "CREA" || Session["UserName"].ToString().ToUpper() != "DATA" || Session["UserName"].ToString().ToUpper() != "PERF" || Session["UserName"].ToString().ToUpper() != "SAI"))
				{
					return RedirectToAction("Index", "Admin");
				}
				else
				{
					return View();
				}
			}
			catch (Exception)
			{

				return RedirectToAction("Index", "Admin");
			}

		}
		[HttpPost]
		public ActionResult PvtErrUpdate(PunAddModels AM) // HttpPostedFileBase file
		{
			try
			{
				// firm login // dist 
				if (Session["UserName"] == null && (Session["UserName"].ToString().ToUpper() != "ADMIN" || Session["UserName"].ToString().ToUpper() != "CREA" || Session["UserName"].ToString().ToUpper() != "DATA" || Session["UserName"].ToString().ToUpper() != "PERF" || Session["UserName"].ToString().ToUpper() != "SAI"))
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
						string fileName1 = "FirmResultMIS_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

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
							using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
							{
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
								string query = string.Format("Select * from [{0}]", excelSheets[0]);
								using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
								{
									dataAdapter.Fill(ds);
								}
							}
							string UserNM = Session["UserName"].ToString();
							switch (UserNM)
							{
								case "CREA": UserNM = "CIPL"; break;
								case "SAI": UserNM = "SAI"; break;
								case "DATA": UserNM = "DATA"; break;
								case "PERF": UserNM = "PERF"; break;
							}
							string CheckMis = objDB.CheckPvtErrUpdateMisExcel(ds, UserNM);
							if (CheckMis == "")
							{
								DataTable dt1 = ds.Tables[0];
								string Result1 = "";
								int OutStatus = 0;
								DataTable dtResult = objDB.PvtErrUpdate(dt1, UserNM, out OutStatus);// OutStatus mobile
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
				//////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View();
			}
			return View();
		}

		[AdminLoginCheckFilter]
		public ActionResult FirmPrivateCandidateErrorList(int? page)
		{
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}

				FormCollection frc = new FormCollection();
				//var itemsch = new SelectList(new[] { new { ID = "1", Name = "Ref No." }, new { ID = "2", Name = "Roll Num" }, new { ID = "3", Name = "Error Code" }, }, "ID", "Name", 1);
				//ViewBag.CorrectionType = itemsch.ToList();
				//------ Paging Srt
				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				//string Catg = CrType;                        

				//---- Paging end
				string FirmNM = Session["UserName"].ToString();
				string UserNM = Session["UserName"].ToString();
				switch (UserNM)
				{
					case "CREA": UserNM = "CIPL"; break;
					case "SAI": UserNM = "SAI"; break;
					case "DATA": UserNM = "DATA"; break;
					case "PERF": UserNM = "PERF"; break;
				}
				string AdminId = Session["AdminId"].ToString();

				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				if (ModelState.IsValid)
				{
					if (page != null && page > 0 && Session["ExamErrorStatus"] != null)
					{
						string Search = Session["ExamErrorStatus"].ToString();
						am.StoreAllData = objDB.GetFirmPrivateCandidateErrorList(0, Search, UserNM, "", pageIndex);
						if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.Message = "Record Not Found";
							ViewBag.TotalCountP = ViewBag.TotalCount = 0;
							return View(am);
						}
						else
						{
							ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
							ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["TotalCount"].ToString());
							ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
							int tp = Convert.ToInt32(ViewBag.TotalCount);
							int pn = tp / 30;
							int cal = 30 * pn;
							int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
							if (res >= 1)
								ViewBag.pn = pn + 1;
							else
								ViewBag.pn = pn;
							return View(am);
						}
					}
				}
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
				return View();
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult FirmPrivateCandidateErrorList(FormCollection frc, int? page, string ERRcode, string cmd, string SearchString, string refno)
		{
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}


				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();

				//------ Paging Srt
				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				//string Catg = CrType;                        

				string FirmNM = Session["UserName"].ToString();
				string UserNM = Session["UserName"].ToString();
				switch (UserNM)
				{
					case "CREA": UserNM = "CIPL"; break;
					case "SAI": UserNM = "SAI"; break;
					case "DATA": UserNM = "DATA"; break;
					case "PERF": UserNM = "PERF"; break;

				}

				if (ModelState.IsValid)
				{
					string Search = "Refno like '%%' ";
					string drp = "";
					string SelType = "";
					int SelValueSch = 0;


					if (frc["SelType"] != null)
					{
						SelType = frc["SelType"].ToString();
						Search += " and ExamType= '" + SelType + "'";
					}

					if (frc["ERRcode"] != null)
					{
						drp = frc["ERRcode"].ToString();
						SelValueSch = Convert.ToInt32(drp.ToString());
						if (drp.ToString().Trim() != "" && drp != null)
						{
							if (SelValueSch == 1)
							{
								Search += " and refno='" + SearchString.ToString().Trim() + "'";
							}
							if (SelValueSch == 2)
							{
								Search += " and RollNo='" + SearchString.ToString().Trim() + "'";
							}
							if (SelValueSch == 3)
							{
								Search += " and Errcode='" + SearchString.ToString().Trim() + "'";
							}
							if (SelValueSch == 4)
							{
								Search += " and SCHL='" + SearchString.ToString().Trim() + "'";
							}
						}
					}

					if (cmd == "Search")
					{
						//---- Paging end
						am.ERRcode = ERRcode;
						ViewBag.Searchstring = SearchString;
						ViewBag.CorrectionType = frc["ERRcode"].ToString();
						string AdminId = Session["AdminId"].ToString();
						Session["ExamErrorStatus"] = Search;
						am.StoreAllData = objDB.GetFirmPrivateCandidateErrorList(0, Search, UserNM, "", pageIndex);

						if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.Message = "Record Not Found";
							ViewBag.TotalCountP = ViewBag.TotalCount = 0;
							return View(am);
						}
						else
						{
							ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
							ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["TotalCount"].ToString());
							ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
							int tp = Convert.ToInt32(ViewBag.TotalCount);
							int pn = tp / 30;
							int cal = 30 * pn;
							int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
							if (res >= 1)
								ViewBag.pn = pn + 1;
							else
								ViewBag.pn = pn;
							return View(am);
						}
					}

					if (cmd == "Download Error List")
					{
						if (Search == null || Search == "")
						{
							if (Session["AdminType"].ToString().ToLower() != "admin")
							{
								Search = " and CreatedBy='" + FirmNM + "'";
							}
						}
						string AdminId = Session["AdminId"].ToString();
						DataSet ds = objDB.GetFirmPrivateCandidateErrorList(1, Search, UserNM, "", pageIndex);
						if (ds.Tables.Count > 0)
						{
							if (ds.Tables[0].Rows.Count > 0)
							{
								DataTable dt = ds.Tables[0];
								string fname = DateTime.Now.ToString("ddMMyyyyHHmm");
								Response.Clear();
								Response.Buffer = true;
								Response.Charset = "";
								Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
								Response.AddHeader("content-disposition", "attachment;filename=ErrList" + fname + ".xlsx");
								using (MemoryStream MyMemoryStream = new MemoryStream())
								{
									XLWorkbook wb = new XLWorkbook();
									var WS = wb.Worksheets.Add(dt, "ErrList" + fname);
									WS.Tables.FirstOrDefault().ShowAutoFilter = false;
									wb.SaveAs(MyMemoryStream);
									MyMemoryStream.WriteTo(Response.OutputStream);
									WS.AutoFilter.Enabled = false;
									Response.Flush();
									Response.End();
								}
							}
						}

					}

				}
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
				return View();
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}


		}

		public ActionResult ADDErrorPrivateCandidate(string id)
		{
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//if (Session["UserName"].ToString() != "admin")
				//{
				//    if (!Session["PAccessRight"].ToString().Contains("Firm"))
				//    {
				//        return RedirectToAction("Index", "Admin");
				//    }
				//}
				var itemsch = new SelectList(new[] { new { ID = "1", Name = "Ref No." }, new { ID = "2", Name = "Roll Num" }, new { ID = "3", Name = "Error Code" }, }, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();

				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				// ViewBag.Searchstring = SearchString;
				if (ModelState.IsValid)
				{

					string FirmNM = Session["UserName"].ToString();
					am.StoreAllData = objDB.GetADDErrorPrivateCandidate(id, FirmNM);


					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View(am);
					}
					else
					{
						return RedirectToAction("FirmPrivateCandidateErrorList", "Admin");
					}
				}
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
				return View();
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}

		public ActionResult RemoveErrorPrivateCandidate(string id)
		{

			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}

				var itemsch = new SelectList(new[] { new { ID = "1", Name = "Ref No." }, new { ID = "2", Name = "Roll Num" }, new { ID = "3", Name = "Error Code" }, }, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();

				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				if (ModelState.IsValid)
				{

					string FirmNM = Session["UserName"].ToString();
					am.StoreAllData = objDB.GetRemovePrivateCandidateErrorList(id, FirmNM);
					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View(am);
					}
					else
					{
						return RedirectToAction("FirmPrivateCandidateErrorList", "Admin");
					}
				}
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
				return View();
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}

		#endregion Pvt 1. Roll & Ecentre Update 2. Error Add and Remove

		#region Final Submitted Records form All Forms For Admin
		[AdminLoginCheckFilter]
		public ActionResult FinalSubmittedRecordsAllAdmin(RegistrationModels rm, int? page)
		{

			AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
			if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
			{
				return RedirectToAction("Logout", "Admin");
			}

			#region Action Assign Method
			if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
			{ ViewBag.IsCancel = 1; ViewBag.IsRMVCancel = 1; ViewBag.IsShift = 1; ViewBag.IsModify = 1; }
			else
			{

				string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
				string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
				//string AdminType = Session["AdminType"].ToString();
				//GetActionOfSubMenu(string cont, string act)
				DataSet aAct = objCommon.GetActionOfSubMenu(Convert.ToInt32(Session["AdminId"]), controllerName, actionName);
				if (aAct.Tables[0].Rows.Count > 0)
				{
					ViewBag.IsCancel = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Equals("CANCEL")).Count();
					ViewBag.IsRMVCancel = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Contains("RMV")).Count();
					ViewBag.IsModify = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Contains("MODIFY".ToUpper())).Count();
					ViewBag.IsShift = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Contains("SHIFT".ToUpper())).Count();
				}
			}

			#endregion Action Assign Method

			var itemsch = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new{ID="2",Name="Candidate Name"}, new{ID="3",Name="Father's Name"},new{ID="4",Name="Mother's Name"},new{ID="5",Name="DOB"},
				new{ID="6",Name="Class Rollnum"},new{ID="7",Name="REGNO"},new{ID="8",Name="Section"},new{ID="9",Name="Aadhar Number"},new{ID="10",Name="Form"},}, "ID", "Name", 1); ViewBag.MySch = itemsch.ToList();

			DataSet result1 = new AbstractLayer.MigrateSchoolDB().GetAllFormName("");
			List<SelectListItem> empList = new List<SelectListItem>();
			foreach (System.Data.DataRow dr in result1.Tables[5].Rows)
			{
				empList.Add(new SelectListItem { Text = @dr["EmpDisplayName"].ToString(), Value = @dr["EmpId"].ToString() });
			}
			ViewBag.AllEmpList = empList;
			ViewBag.SelectedEmp = "0";

			string Search = string.Empty;
			string schlid = "";
			if (TempData["SCHL"] != null)
			{
				schlid = TempData["SCHL"].ToString();
			}
			else if (!string.IsNullOrEmpty(rm.SCHL))
			{
				schlid = rm.SCHL.ToString();
			}
			else
			{
				return View(rm);
			}
			if (schlid != "")
			{
				#region Group List School wise
				List<SelectListItem> MyGroupList = objCommon.GroupName();
				// ViewBag.MyGroup = objCommon.GroupName();
				DataTable dtAssignSubject = objCommon.GetAssignSubjectBySchool(schlid);
				if (dtAssignSubject != null && dtAssignSubject.Rows.Count > 0)
				{
					ViewBag.MyGroup = objCommon.GetSubjectsBySchool(dtAssignSubject, MyGroupList);
				}
				ViewBag.MyGroup = MyGroupList;
				#endregion
			}
			TempData["SCHL"] = rm.SCHL = schlid;

			int pageIndex = 1;
			pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
			ViewBag.pagesize = pageIndex;

			if (TempData["Search"] != null)
			{
				Search = TempData["Search"].ToString();
			}
			TempData["Search"] = Search;
			rm.StoreAllData = objDB.FinalSubmittedRecordsAll(Search, pageIndex);
			if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
			{
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
				return View();
			}
			else
			{
				ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
				ViewBag.TotalCount1 = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[0]["decount"]);
				int tp = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[0]["decount"]);
				int pn = tp / 30;
				int cal = 30 * pn;
				int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
				if (res >= 1)
					ViewBag.pn = pn + 1;
				else
					ViewBag.pn = pn;
				return View(rm);
			}
		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult FinalSubmittedRecordsAllAdmin(RegistrationModels rm, int? page, FormCollection frm)
		{

			var itemsch = new SelectList(new[]{new {ID="1",Name="By UniqueID"},new{ID="2",Name="Candidate Name"}, new{ID="3",Name="Father's Name"},new{ID="4",Name="Mother's Name"},new{ID="5",Name="DOB"},
				new{ID="6",Name="Class Rollnum"},new{ID="7",Name="REGNO"},new{ID="8",Name="Section"},new{ID="9",Name="Aadhar Number"},new{ID="10",Name="Form"},}, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			DataSet result1 = new AbstractLayer.MigrateSchoolDB().GetAllFormName("");
			List<SelectListItem> empList = new List<SelectListItem>();
			foreach (System.Data.DataRow dr in result1.Tables[5].Rows)
			{
				empList.Add(new SelectListItem { Text = @dr["EmpDisplayName"].ToString(), Value = @dr["EmpId"].ToString() });
			}
			ViewBag.AllEmpList = empList;
			ViewBag.SelectedEmp = "0";

			AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();

			if (ModelState.IsValid)
			{
				#region Action Assign Method
				if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{ ViewBag.IsCancel = 1; ViewBag.IsRMVCancel = 1; ViewBag.IsShift = 1; ViewBag.IsModify = 1; }
				else
				{

					string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
					string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
					//string AdminType = Session["AdminType"].ToString();
					//GetActionOfSubMenu(string cont, string act)
					DataSet aAct = objCommon.GetActionOfSubMenu(Convert.ToInt32(Session["AdminId"]), controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						ViewBag.IsCancel = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Equals("CANCEL")).Count();
						ViewBag.IsRMVCancel = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Contains("RMV")).Count();
						ViewBag.IsModify = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Contains("MODIFY".ToUpper())).Count();
						ViewBag.IsShift = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Contains("SHIFT".ToUpper())).Count();
					}
				}

				#endregion Action Assign Method

				#region Group List School wise
				List<SelectListItem> MyGroupList = objCommon.GroupName();
				// ViewBag.MyGroup = objCommon.GroupName();
				DataTable dtAssignSubject = objCommon.GetAssignSubjectBySchool(rm.SCHL);
				if (dtAssignSubject != null && dtAssignSubject.Rows.Count > 0)
				{
					ViewBag.MyGroup = objCommon.GetSubjectsBySchool(dtAssignSubject, MyGroupList);
				}
				ViewBag.MyGroup = MyGroupList;
				#endregion

				string Search = string.Empty;

				string schlid = rm.SCHL;
				TempData["SCHL"] = schlid;
				if (frm["SCHL"] != null && frm["SCHL"].ToString() != "")
				{
					schlid = frm["SCHL"].ToString().Trim();
				}
				else
				{
					return View(rm);
				}
				Search = " a.schl='" + schlid + "' and a.std_id is not null ";
				if (frm["SelList"] != "")
				{
					ViewBag.SelectedItem = frm["SelList"];
					int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());


					if (frm["SearchString"] != "")
					{
						ViewBag.SearchString = frm["SearchString"];
						if (SelValueSch == 1)
						{ Search += " and a.std_id='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 2)
						{ Search += " and  a.Candi_Name like '%" + frm["SearchString"].ToString() + "%'"; }
						else if (SelValueSch == 3)
						{ Search += " and a.Father_Name  like '%" + frm["SearchString"].ToString() + "%'"; }
						else if (SelValueSch == 4)
						{ Search += " and a.Mother_Name like '%" + frm["SearchString"].ToString() + "%'"; }
						else if (SelValueSch == 5)
						{ Search += " and a.DOB='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 6)
						{ Search += " and a.Class_Roll_Num_Section ='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 7)
						{ Search += " and a.Registration_num ='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 8)
						{ Search += " and a.section ='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 9)
						{ Search += " and a.Aadhar_num   ='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 10)
						{ Search += " and a.form_Name  ='" + frm["SearchString"].ToString() + "'"; }
					}

				}
				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;

				rm.StoreAllData = objDB.FinalSubmittedRecordsAll(Search, pageIndex);
				TempData["Search"] = Search;
				// asm.StoreAllData = objDB.SearchSchoolDetails(Search);
				if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{

					ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
					ViewBag.TotalCount1 = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[0]["decount"]);
					int tp = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[0]["decount"]);
					int pn = tp / 30;
					int cal = 30 * pn;
					int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
					if (res >= 1)
						ViewBag.pn = pn + 1;
					else
						ViewBag.pn = pn;
					return View(rm);
				}
			}
			else
			{
				return View(rm);
				//return N2Formgrid();
			}
		}
		public ActionResult CommanFormView(string id, string formNM)
		{
			AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
			RegistrationModels rm = new RegistrationModels();

			if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
			{
				return RedirectToAction("Logout", "Admin");
			}

			if (id == null)
			{
				return RedirectToAction("FinalSubmittedRecordsAllAdmin", "Admin");
			}
			string formname = "E1";
			if (id != null)
			{
				try
				{
					id = encrypt.QueryStringModule.Decrypt(id);
					//DataSet ds = objDB.CommanFormView(id);
					DataSet ds = objDB.CommanFormView(id, formNM);

					if (ds == null || ds.Tables[0].Rows.Count == 0)
					{
						return RedirectToAction("FinalSubmittedRecordsAllAdmin", "Admin");
					}
					if (ds.Tables[0].Rows.Count > 0)
					{
						//DataSet ds = objDB.SearchStudentGetByData(id);
						// rm.StoreAllData = objDB.SearchStudentGetByData(id);
						rm.form_Name = ds.Tables[0].Rows[0]["form_name"].ToString();
						Session["formName"] = ds.Tables[0].Rows[0]["form_name"].ToString();
						rm.Category = ds.Tables[0].Rows[0]["Category"].ToString();
						rm.Board = ds.Tables[0].Rows[0]["Board"].ToString();
						rm.Other_Board = ds.Tables[0].Rows[0]["Other_Board"].ToString();
						rm.Registration_num = ds.Tables[0].Rows[0]["Registration_num"].ToString();
						rm.Board_Roll_Num = ds.Tables[0].Rows[0]["Board_Roll_Num"].ToString();
						rm.MetricBoard = ds.Tables[0].Rows[0]["MatricBoard"].ToString();
						rm.MatricResult = ds.Tables[0].Rows[0]["MatricResult"].ToString();
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
						rm.Differently_Abled = ds.Tables[0].Rows[0]["disabilityPercent"].ToString();
						rm.DP = ds.Tables[0].Rows[0]["Differently_Abled"].ToString();
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
						rm.CandiMedium = ds.Tables[0].Rows[0]["CandStudyMedium"].ToString();
						// @ViewBag.ImageURL = "../../StdImages/Upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
						rm.PreNSQF = ds.Tables[0].Rows[0]["PRE_NSQF_SUB"].ToString();
						rm.NSQF = ds.Tables[0].Rows[0]["nsqf_flag"].ToString();

						if (rm.NSQF == "True")
						{
							@ViewBag.ns = "1";
							ViewData["NSQFSCHOOL"] = "1";
							rm.NSQF = "YES";
							string subNSQF = ds.Tables[0].Rows[0]["NSQF_SUB"].ToString();
							string PRENSQF = ds.Tables[0].Rows[0]["PRE_NSQF_SUB"].ToString();
							if (PRENSQF == "NO")
							{
								rm.NsqfsubS6 = "NO";
								rm.PreNSQF = "NO";
							}
							else
							{
								DataSet nsresult = objDB.GetNSQFVIEWSUBJECT11TH(subNSQF, PRENSQF);
								if (nsresult.Tables[0].Rows.Count > 0)
								{
									rm.NsqfsubS6 = nsresult.Tables[0].Rows[0]["NAME_ENG"].ToString();
								}
								else
								{
									rm.NsqfsubS6 = "NO";
								}

								if (nsresult.Tables[1].Rows.Count > 0)
								{
									rm.PreNSQF = nsresult.Tables[1].Rows[0]["NAME_ENG"].ToString();
								}
								else
								{
									rm.PreNSQF = "NO";
								}
							}
						}
						else
						{
							ViewData["NSQFSCHOOL"] = "0";
							rm.NSQF = "NO";
						}

						@ViewBag.Photo = ds.Tables[0].Rows[0]["std_Photo"].ToString();
						@ViewBag.sign = ds.Tables[0].Rows[0]["std_Sign"].ToString();
						// @ViewBag.Photo = "https://registration2022.pseb.ac.in/Upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
						//@ViewBag.sign = "https://registration2022.pseb.ac.in/Upload/" + ds.Tables[0].Rows[0]["std_Sign"].ToString();

						rm.MetricYear = ds.Tables[0].Rows[0]["MetricYear"].ToString();
						rm.MetricMonth = ds.Tables[0].Rows[0]["MetricMonth"].ToString();
						rm.Metric_Roll_Num = ds.Tables[0].Rows[0]["MetricRollNum"].ToString();
						//rm.Registration_num = ds.Tables[0].Rows[0]["Registration_num"].ToString();


						//------------------------------Fill Subjects----------------//

						if (ds.Tables[1].Rows.Count > 0)
						{
							for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
							{
								//if (rm.DA == "N.A.")
								//{
								if (i == 0)
								{
									rm.subS1 = ds.Tables[1].Rows[0]["SUBNM"].ToString();
									rm.subm1 = ds.Tables[1].Rows[0]["MEDIUM"].ToString();
									rm.subbS1 = ds.Tables[1].Rows[0]["SUB"].ToString();
								}
								else if (i == 1)
								{
									rm.subS2 = ds.Tables[1].Rows[1]["SUBNM"].ToString();
									rm.subM2 = ds.Tables[1].Rows[1]["MEDIUM"].ToString();
									rm.subbS2 = ds.Tables[1].Rows[1]["SUB"].ToString();
								}
								else if (i == 2)
								{
									rm.subS3 = ds.Tables[1].Rows[2]["SUBNM"].ToString();
									rm.subm3 = ds.Tables[1].Rows[2]["MEDIUM"].ToString();
									rm.subbS3 = ds.Tables[1].Rows[2]["SUB"].ToString();
								}
								else if (i == 3)
								{
									rm.subS4 = ds.Tables[1].Rows[3]["SUBNM"].ToString();
									rm.subM4 = ds.Tables[1].Rows[3]["MEDIUM"].ToString();
									rm.subbS4 = ds.Tables[1].Rows[3]["SUB"].ToString();
								}
								else if (i == 4)
								{
									rm.subS5 = ds.Tables[1].Rows[4]["SUBNM"].ToString();
									rm.subM5 = ds.Tables[1].Rows[4]["MEDIUM"].ToString();
									rm.subbS5 = ds.Tables[1].Rows[4]["SUB"].ToString();
								}
								else if (i == 5)
								{
									rm.subS6 = ds.Tables[1].Rows[5]["SUBNM"].ToString();
									rm.subM6 = ds.Tables[1].Rows[5]["MEDIUM"].ToString();
									rm.subbS6 = ds.Tables[1].Rows[5]["SUB"].ToString();
								}
								else if (i == 6)
								{
									rm.subS7 = ds.Tables[1].Rows[6]["SUBNM"].ToString();
									rm.subM7 = ds.Tables[1].Rows[6]["MEDIUM"].ToString();
									rm.subbS7 = ds.Tables[1].Rows[6]["SUB"].ToString();
								}
								else if (i == 7)
								{
									rm.subS8 = ds.Tables[1].Rows[7]["SUBNM"].ToString();
									rm.subM8 = ds.Tables[1].Rows[7]["MEDIUM"].ToString();
									rm.subbS8 = ds.Tables[1].Rows[7]["SUB"].ToString();
								}
								else if (i == 8)
								{
									rm.s9 = ds.Tables[1].Rows[8]["SUBNM"].ToString();
									rm.m9 = ds.Tables[1].Rows[8]["MEDIUM"].ToString();
									rm.subbS9 = ds.Tables[1].Rows[8]["SUB"].ToString();
								}
								else if (i == 9)
								{
									rm.s10 = ds.Tables[1].Rows[9]["SUBNM"].ToString();
									rm.m10 = ds.Tables[1].Rows[9]["MEDIUM"].ToString();
									rm.subbS10 = ds.Tables[1].Rows[9]["SUB"].ToString();
								}
								else if (i == 10)
								{
									rm.s11 = ds.Tables[1].Rows[10]["SUBNM"].ToString();
									rm.m11 = ds.Tables[1].Rows[10]["MEDIUM"].ToString();
									//rm.subbS11 = ds.Tables[1].Rows[10]["SUB"].ToString();
								}
								else if (i == 11)
								{
									rm.s12 = ds.Tables[1].Rows[11]["SUBNM"].ToString();
									rm.m12 = ds.Tables[1].Rows[11]["MEDIUM"].ToString();
									//rm.subbS12 = ds.Tables[1].Rows[11]["SUB"].ToString();
								}

							}
						}
					}
					else
					{
						return RedirectToAction("FinalSubmittedRecordsAllAdmin", "Admin");
					}

				}
				catch (Exception ex)
				{
					return RedirectToAction("FinalSubmittedRecordsAllAdmin", "Admin");
				}

			}
			//return View(asm);
			return View(rm);
		}

		[HttpPost]
		public JsonResult CancelStdRegNo(string Remarks, string stdid)
		{
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
			try
			{
				string dee = "";
				string res = null;
				DataSet result = objDB.CancelStdRegNo(Remarks, stdid, adminLoginSession.AdminEmployeeUserId);
				res = result.Tables[0].Rows.Count.ToString();
				dee = "Yes";
				return Json(new { sn = dee, chid = res }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return null;
			}
		}


		[HttpPost]
		public JsonResult SwitchForm(string Remarks, string OldRegNo, string stdid)
		{
			try
			{
				string UpdatedBy = "ADMIN-" + Session["AdminId"].ToString();
				string dee = "";
				int OutStatus = 0;
				int result = AbstractLayer.RegistrationDB.SwitchForm(Remarks, stdid, OldRegNo, UpdatedBy, out OutStatus);
				if (OutStatus == 0)
				{
					dee = "Yes";
				}
				else if (OutStatus == 0)
				{
					dee = "NotAllowed";
				}
				else
				{ dee = "No"; }

				return Json(new { sn = dee, chid = OutStatus }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return null;
			}
		}


		#endregion Final Submitted Records form All Forms For Admin

		#region Admin School Correction
		public ActionResult AdminSchoolCorrection()
		{
			try
			{
				ViewBag.DA = objCommon.GetDA();

				Session["SCHL"] = "9999999";
				//ViewBag.SelectedItem
				AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
				RegistrationModels rm = new RegistrationModels();
				if (Session["AdminType"] == null)
				{
					return RedirectToAction("Logout", "Admin");
				}

				string schid = Session["SCHL"].ToString();
				DataSet result = objDB.schooltypesCorrection(schid, ""); // passing Value to DBClass from model
				if (result == null)
				{
					return RedirectToAction("Index", "Home");
				}
				if (result.Tables[1].Rows.Count > 0)
				{

					ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
					ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
					ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
					ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
					ViewBag.N3M1threclock = result.Tables[1].Rows[0]["Nth"].ToString();
					ViewBag.E1T1threclock = result.Tables[1].Rows[0]["Eth"].ToString();

					DateTime sDateN = Convert.ToDateTime(result.Tables[6].Rows[0]["sDate"]);
					DateTime eDateN = Convert.ToDateTime(result.Tables[6].Rows[0]["eDate"]);

					DateTime sDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["sDate"]);
					DateTime eDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["eDate"]);

					DateTime sDateE = Convert.ToDateTime(result.Tables[6].Rows[2]["sDate"]);
					DateTime eDateE = Convert.ToDateTime(result.Tables[6].Rows[2]["eDate"]);

					DateTime sDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["sDate"]);
					DateTime eDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["eDate"]);

					DateTime sDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["sDate"]);
					DateTime eDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["eDate"]);

					DateTime sDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["sDate"]);
					DateTime eDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["eDate"]);

					DateTime dtTodate = Convert.ToDateTime(DateTime.Today);

					//List<SelectListItem> itemsch = new List<SelectListItem>();
					//if (ViewBag.N3M1threclock == "1" && dtTodate <= eDateN)
					//{
					//    itemsch.Add(new SelectListItem { Text = "9th Class", Value = "5" });
					//}
					//if (ViewBag.Matric == "1" && dtTodate <= eDateM)
					//{
					//    itemsch.Add(new SelectListItem { Text = "Matriculation Regular", Value = "1" });
					//}
					//if (ViewBag.OMatric == "1" && dtTodate <= eDateMO)
					//{
					//    itemsch.Add(new SelectListItem { Text = "Matriculation Open", Value = "2" });
					//}
					//if (ViewBag.E1T1threclock == "1" && dtTodate <= eDateE)
					//{
					//    itemsch.Add(new SelectListItem { Text = "11th Class", Value = "6" });
					//}
					//if (ViewBag.Senior == "1" && dtTodate <= eDateT)
					//{
					//    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Regular", Value = "3" });
					//}
					//if (ViewBag.OSenior == "1" && dtTodate <= eDateTO)
					//{
					//    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Open", Value = "4" });
					//}


					//if (ViewBag.Matric != "1" && ViewBag.OMatric != "1" && ViewBag.Senior != "1" && ViewBag.OSenior != "1" && ViewBag.N3M1threclock != "1" && ViewBag.E1T1threclock != "1")
					//{
					//    itemsch.Add(new SelectListItem { Text = "", Value = "" });
					//}
					// ViewBag.MySch = itemsch.ToList();
					//Bind Class asign by admin id
					ViewBag.MySch = AbstractLayer.DBClass.GetCorrectionClassAssignListByAdminId(Session["ClassAssign"].ToString()).ToList();

				}
				if (ModelState.IsValid)
				{ return View(rm); }
				else
				{ return View(); }
			}
			catch (Exception ex)
			{
				return RedirectToAction("Logout", "Login");
			}
		}

		[HttpPost]
		public ActionResult AdminSchoolCorrection(FormCollection frm, RegistrationModels rm, int? page, string cmd)
		{
			try
			{
				ViewBag.DA = objCommon.GetDA();

				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
				//RegistrationModels rm = new RegistrationModels();
				rm = new RegistrationModels();
				rm.SCHL = frm["SCHL"].ToString();
				Session["SCHL"] = frm["SCHL"].ToString();
				string schid = Session["SCHL"].ToString();

				DataSet result = objDB.schooltypesCorrection(schid, ""); // passing Value to DBClass from model
				if (result.Tables[1].Rows.Count > 0)
				{

					ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
					ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
					ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
					ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
					ViewBag.N3M1threclock = result.Tables[1].Rows[0]["Nth"].ToString();
					ViewBag.E1T1threclock = result.Tables[1].Rows[0]["Eth"].ToString();

					DateTime sDateN = Convert.ToDateTime(result.Tables[6].Rows[0]["sDate"]);
					DateTime eDateN = Convert.ToDateTime(result.Tables[6].Rows[0]["eDate"]);

					DateTime sDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["sDate"]);
					DateTime eDateM = Convert.ToDateTime(result.Tables[6].Rows[1]["eDate"]);

					DateTime sDateE = Convert.ToDateTime(result.Tables[6].Rows[2]["sDate"]);
					DateTime eDateE = Convert.ToDateTime(result.Tables[6].Rows[2]["eDate"]);

					DateTime sDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["sDate"]);
					DateTime eDateT = Convert.ToDateTime(result.Tables[6].Rows[3]["eDate"]);

					DateTime sDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["sDate"]);
					DateTime eDateMO = Convert.ToDateTime(result.Tables[6].Rows[4]["eDate"]);

					DateTime sDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["sDate"]);
					DateTime eDateTO = Convert.ToDateTime(result.Tables[6].Rows[5]["eDate"]);

					DateTime dtTodate = Convert.ToDateTime(DateTime.Today);

					//List<SelectListItem> itemsch = new List<SelectListItem>();
					//if (ViewBag.N3M1threclock == "1" && eDateN >= dtTodate)
					//{
					//    itemsch.Add(new SelectListItem { Text = "9th Class", Value = "5" });
					//}
					//if (ViewBag.Matric == "1" && dtTodate <= eDateM)
					//{
					//    itemsch.Add(new SelectListItem { Text = "Matriculation Regular", Value = "1" });
					//}
					//if (ViewBag.OMatric == "1" && dtTodate <= eDateMO)
					//{
					//    itemsch.Add(new SelectListItem { Text = "Matriculation Open", Value = "2" });
					//}
					//if (ViewBag.E1T1threclock == "1" && dtTodate <= eDateE)
					//{
					//    itemsch.Add(new SelectListItem { Text = "11th Class", Value = "6" });
					//}
					//if (ViewBag.Senior == "1" && dtTodate <= eDateT)
					//{
					//    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Regular", Value = "3" });
					//}
					//if (ViewBag.OSenior == "1" && dtTodate <= eDateTO)
					//{
					//    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Open", Value = "4" });
					//}


					//if (ViewBag.Matric != "1" && ViewBag.OMatric != "1" && ViewBag.Senior != "1" && ViewBag.OSenior != "1" && ViewBag.N3M1threclock != "1" && ViewBag.E1T1threclock != "1")
					//{
					//    itemsch.Add(new SelectListItem { Text = "", Value = "" });
					//}
					//ViewBag.MySch = itemsch.ToList();
					ViewBag.MySch = AbstractLayer.DBClass.GetCorrectionClassAssignListByAdminId(Session["ClassAssign"].ToString()).ToList();

				}

				if (ModelState.IsValid)
				{
					string Search = string.Empty;
					string Std_id = frm["SearchString"];
					string schlid = "";
					if (Session["SCHL"] != null)
					{
						schlid = Session["SCHL"].ToString();
					}

					else
					{
						return View(rm);
					}

					switch (frm["SelList"].ToString())
					{
						case "22":
							Search = " a.schl='" + schlid + "' and a.APPNO='" + Std_id + "'";
							break;
						case "44":
							Search = " a.schl='" + schlid + "' and a.APPNO='" + Std_id + "'";
							break;
						default:
							Search = "a.schl='" + schlid + "' and a.std_id='" + Std_id + "'";
							break;
					}

					//if (frm["SelList"] != null)
					//{
					//    Search = " a.schl='" + schlid + "' and a.std_id='" + Std_id + "'";
					//}
					ViewBag.SelectedItem = frm["SelList"];
					ViewBag.Searchstring = frm["SearchString"];
					ViewBag.SCHLstring = frm["SCHL"];
					int SelValueSch = 0;
					if (frm["SelList"].ToString() == "")
					{
						SelValueSch = 1;
						Search = "form_Name in ('')";
					}
					else
					{
						SelValueSch = Convert.ToInt32(frm["SelList"].ToString());
					}
					#region Add Record Region Begin
					if (cmd == "Add Record")
					{
						rm.Std_id = Convert.ToInt32(frm["SearchString"]);
						rm.Class = frm["SelList"];
						rm.Correctiontype = frm["SelListField"];
						rm.oldVal = frm["oldVal"];
						if (rm.Correctiontype == "Candi_Name_P" || rm.Correctiontype == "Father_Name_P" || rm.Correctiontype == "Mother_Name_P")
						{
							rm.newVal = frm["newValP"];
						}
						else if (rm.Correctiontype == "Caste")
						{
							rm.newVal = frm["newValCaste"];
						}
						else if (rm.Correctiontype == "DOB")
						{
							rm.newVal = frm["newValDOB"];
						}
						else if (rm.Correctiontype == "Gender")
						{
							rm.newVal = frm["newValGender"];
						}
						else if (rm.Correctiontype == "category")
						{
							rm.newVal = frm["newValcorCategory"];
						}
						else if (rm.Correctiontype == "CandStudyMedium")
						{
							rm.newVal = frm["newValcorCSM"];
						}
						else if (rm.Correctiontype == "Differently_Abled")
						{
							rm.newVal = frm["Differently_Abled"];
						}
						else if (rm.Correctiontype == "wantwriter")
						{
							rm.newVal = frm["wantwriter"];
						}
						else if (rm.Correctiontype == "Religion")
						{
							rm.newVal = frm["Relist"];
						}
						else if (rm.Correctiontype == "EXAM")
						{
							rm.newVal = frm["Grouplist"];
						}
						else if (rm.Correctiontype == "Month" || rm.Correctiontype == "MetricMonth")
						{
							rm.newVal = frm["Monthlist"].ToUpper();
						}
						else if (rm.Correctiontype == "Year" || rm.Correctiontype == "MetricYear")
						{
							rm.newVal = frm["Yearlist"].ToUpper();
						}
						else if (rm.Correctiontype == "Admission_Date")
						{
							rm.newVal = frm["Admission_Date"];
						}
						else if (rm.Correctiontype == "Belongs_BPL")
						{
							rm.newVal = frm["Belongs_BPL"].ToUpper();
						}
						else if (rm.Correctiontype == "Board")
						{
							rm.newVal = frm["Board"].ToUpper();
						}
						else if (rm.Correctiontype == "MatricBoard")
						{
							rm.newVal = frm["MatricBoard"].ToUpper();
						}
						else if (rm.Correctiontype == "MatricResult")
						{
							rm.newVal = frm["MatricResult"].ToUpper();
						}
						else if (rm.Correctiontype == "Tehsil")
						{
							rm.newVal = frm["SelAllTehsil"].ToUpper();
						}
						else if (rm.Correctiontype == "District")
						{
							rm.newVal = frm["SelAllDistrict"].ToUpper();
						}
						else if (rm.Correctiontype == "Section")
						{
							rm.newVal = frm["Section"].ToUpper();
						}


						else
						{
							rm.newVal = frm["newVal"];
						}

						rm.Remark = frm["Remark"];
						ViewBag.SelectedItem = frm["SelList"];
						ViewBag.Searchstring = frm["SearchString"];
						rm.SCHL = Session["SCHL"].ToString();

						DataSet result1 = objDB.InsertAdminSchoolCorrectionAdd(rm, frm);
						if (result1 != null)
						{
							ViewData["Status"] = result1.Tables[0].Rows[0]["res"].ToString();
							TempData["msg"] = result1.Tables[0].Rows[0]["msg"].ToString();
						}
						else
						{
							ViewData["Status"] = "-1";
						}

					}
					#endregion Add Record

					rm.StoreAllData = objDB.GetCorrectionStudentRecordsSearch_ED(Search, SelValueSch, pageIndex);
					rm.Correctiondata = null;
					ViewBag.TotalCountadded = "";
					rm.Correctiondata = objDB.GetStudentRecordsCorrectiondataPending(schlid);
					ViewBag.TotalCountadded = rm.Correctiondata.Tables[0].Rows.Count;

					if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;

						return View(rm);
					}
					else
					{

						ViewBag.TotalCountSearch = rm.StoreAllData.Tables[0].Rows.Count;
						string std_Class = rm.StoreAllData.Tables[0].Rows[0]["form_Name"].ToString();

						ViewBag.MyMatricBoard = objCommon.GetMatricBoard();
						ViewBag.MyMatricResult = objCommon.GetMatricResult();

						DataSet AllTehsil = objDB.getAllTehsil();
						ViewBag.MyAllTehsil = AllTehsil.Tables[0];
						List<SelectListItem> itemsTeh = new List<SelectListItem>();
						foreach (System.Data.DataRow dr in ViewBag.MyAllTehsil.Rows)
						{
							itemsTeh.Add(new SelectListItem { Text = @dr["TehsilName"].ToString(), Value = @dr["TehsilCode"].ToString() });
						}
						ViewBag.MyAllTehsil = new SelectList(itemsTeh, "Value", "Text");

						DataSet AllDistrict = objDB.getAllDistrict();
						ViewBag.MyAllDistrict = AllDistrict.Tables[0];
						List<SelectListItem> itemsDis = new List<SelectListItem>();
						foreach (System.Data.DataRow dr in ViewBag.MyAllDistrict.Rows)
						{
							itemsDis.Add(new SelectListItem { Text = @dr["DistName"].ToString(), Value = @dr["DistCode"].ToString() });
						}
						ViewBag.MyAllDistrict = new SelectList(itemsDis, "Value", "Text");

						DataSet CorrectionField = objDB.getCorrrectionField(std_Class);
						ViewBag.MySchField = CorrectionField.Tables[0];
						List<SelectListItem> items = new List<SelectListItem>();
						foreach (System.Data.DataRow dr in ViewBag.MySchField.Rows)
						{
							items.Add(new SelectListItem { Text = @dr["CorrectionFieldDisplayName"].ToString(), Value = @dr["CorrectionFieldName"].ToString() });
						}

						if (rm.StoreAllData.Tables[0].Rows[0]["aashirwardnumber"].ToString() == "")
						{
							for (int i = 1; i < CorrectionField.Tables[0].Rows.Count; i++)
							{
								if ((items[i]).Value == "aashirwardnumber")
								{
									items.Remove(items[i]);
									CorrectionField.Tables[0].Rows.RemoveAt(i);
								}

							}
						}
						ViewBag.MySchField = new SelectList(items, "Value", "Text");

						List<SelectListItem> MyGroupList = objCommon.GroupName();
						DataTable dtAssignSubject = objCommon.GetAssignSubjectBySchool(Session["SCHL"].ToString());
						if (dtAssignSubject != null && dtAssignSubject.Rows.Count > 0)
						{
							ViewBag.MyGrouplist = objCommon.GetSubjectsBySchool(dtAssignSubject, MyGroupList);
						}
						ViewBag.MyGrouplist = MyGroupList;

						List<SelectListItem> yearlist = objCommon.GetSessionYear();
						if (rm.StoreAllData.Tables[0].Rows[0]["category"].ToString().ToUpper().Contains("FAILED"))
						{
							yearlist.RemoveRange(3, 45);
							yearlist.Remove(yearlist[0]);
							//yearlist.Remove(yearlist[0]);
							// yearlist.ToList().Where(s => Convert.ToInt32(s.Value) >= 2013);
							ViewBag.SessionYearListPrev = yearlist;
						}
						else
						{
							ViewBag.SessionYearListPrev = yearlist;
						}


						ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
						ViewBag.TotalCount1 = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[0]["decount"]);
						int tp = Convert.ToInt32(rm.StoreAllData.Tables[1].Rows[0]["decount"]);
						int pn = tp / 30;
						int cal = 30 * pn;
						int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
						if (res >= 1)
							ViewBag.pn = pn + 1;
						else
							ViewBag.pn = pn;
						return View(rm);
					}
				}
				else
				{
					return View();
				}
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		public ActionResult AiddedCorrectionRecordDelete(string id)
		{
			AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
			RegistrationModels rm = new RegistrationModels();
			try
			{
				if (id == null)
				{
					return RedirectToAction("AdminSchoolCorrection", "Admin");
				}
				else
				{
					string result = objDB.AiddedCorrectionRecordDelete(id); // passing Value to DBClass from model
					if (result == "1")
					{
						ViewData["DeleteStatus"] = result;
						return RedirectToAction("AdminSchoolCorrection", "Admin");
					}
					else
					{
						ViewData["DeleteStatus"] = "";
						return RedirectToAction("AdminSchoolCorrection", "Admin");
					}


				}
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Admin");
			}
		}

		public ActionResult RejectSchoolCorrection(int? page)
		{
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				FormCollection frc = new FormCollection();
				var itemsch = new SelectList(new[] { new { ID = "1", Name = "Particular" }, new { ID = "2", Name = "Subject" }, new { ID = "4", Name = "Image" }, }, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();
				ViewBag.MySch = AbstractLayer.DBClass.GetCorrectionClassAssignListByAdminId(Session["ClassAssign"].ToString()).ToList();
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				string classAssign = "";
				if (ModelState.IsValid)
				{
					if (Session["CorLot"] != null)
					{
						string CorLot2 = Session["CorLot"].ToString();
						ViewBag.SelectedItemcode = Session["CorrectionType1"].ToString();
						am.CorrectionLot = Session["CorLot"].ToString();
						string Search = string.Empty;
						string CrType = Session["CorrectionType1"].ToString();
						//ViewBag.CorrectionType = am.CorrectionType;
						Search = "a.correctionlot='" + CorLot2 + "' ";
						if (Session["DistAllow"].ToString() != "")
						{
							Search += " and b.DIST in(" + Session["DistAllow"].ToString() + ")";
						}

						if (Session["ClassAssign"] != null)
						{
							classAssign = Session["ClassAssign"].ToString();
							string classAssignSearch = " Convert(varchar,a.class) in (";

							if (Session["ClassAssign"].ToString().Contains("12"))
							{
								classAssignSearch += "  'Sr.Secondary Open','Sr.Secondary Regular','4','44',";
							}
							if (Session["ClassAssign"].ToString().Contains("10"))
							{
								classAssignSearch += " 'Matriculation Open','Matriculation Regular','2','22',";
							}
							if (Session["ClassAssign"].ToString().Contains("11"))
							{
								classAssignSearch += " '11th Class',";
							}
							if (Session["ClassAssign"].ToString().Contains("9"))
							{
								classAssignSearch += " '9th Class',";
							}
							if (Session["ClassAssign"].ToString().Contains("5"))
							{
								classAssignSearch += " 'Primary','5',";
							}
							if (Session["ClassAssign"].ToString().Contains("8"))
							{
								classAssignSearch += " 'Middle', '8',";
							}

							classAssignSearch = classAssignSearch.Remove(classAssignSearch.Length - 1, 1);
							classAssignSearch += " )";
							Search += " and " + classAssignSearch;
						}


						//------ Paging Srt
						int pageIndex = 1;
						pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
						ViewBag.pagesize = pageIndex;
						//string Catg = CrType;                        

						//---- Paging end
						am.StoreAllData = objDB.GetCorrectionDataFirm(Search, CrType, pageIndex);
						ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
						if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.Message = "Record Not Found";
							ViewBag.TotalCount = 0;
							return View(am);
						}
						else
						{
							ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
							ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
							int tp = Convert.ToInt32(ViewBag.TotalCount);
							int pn = tp / 30;
							int cal = 30 * pn;
							int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
							if (res >= 1)
								ViewBag.pn = pn + 1;
							else
								ViewBag.pn = pn;

							return View(am);
						}
					}
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View();


				}
				else
				{
					return View();
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}
		[HttpPost]
		public ActionResult RejectSchoolCorrection(int? page, FormCollection frc, string cmd, string id, string SelList)
		{
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				FormCollection frm = new FormCollection();
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				var itemsch = new SelectList(new[] { new { ID = "1", Name = "Particular" }, new { ID = "2", Name = "Subject" }, new { ID = "4", Name = "Image" }, }, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();
				ViewBag.MySch = AbstractLayer.DBClass.GetCorrectionClassAssignListByAdminId(Session["ClassAssign"].ToString()).ToList();
				if (ModelState.IsValid)
				{
					//------ Paging Srt
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;
					//string Catg = CrType;                        

					//---- Paging end
					string classAssign = "";
					#region Search Record 
					ViewBag.SelectedItemcode = frc["CorrectionType1"].ToString();
					if (cmd == "Search" && frc["CorrectionType1"].ToString() != "")
					{
						string Search = string.Empty;
						string CrType = frc["CorrectionType1"].ToString();
						Session["CorrectionType1"] = ViewBag.SelectedItemcode;
						am.CorrectionLot = frc["CorrectionLot"].ToString();
						Session["CorLot"] = frc["CorrectionLot"].ToString();
						am.SearchResult = frc["SearchResult"].ToString();
						//ViewBag.CorrectionType = frc["CorrectionType"].ToString();
						if (frc["SearchResult"].ToString() != "0")
						{
							switch (am.SearchResult)
							{
								case "1": Search = "a.correctionlot ='" + am.CorrectionLot + "' "; break;
								case "2": Search = "a.correctionId ='" + am.CorrectionLot + "' "; break;
								case "3":
									if (CrType == "1") Search = "a.std_id ='" + am.CorrectionLot + "' ";
									if (CrType == "2") Search = "a.CANDID ='" + am.CorrectionLot + "' ";
									if (CrType == "4") Search = "a.std_id ='" + am.CorrectionLot + "' ";
									break;
							}

						}
						if (Session["DistAllow"].ToString() != "")
						{
							Search += " and b.DIST in(" + Session["DistAllow"].ToString() + ")";
						}
						if (Session["ClassAssign"] != null)
						{
							classAssign = Session["ClassAssign"].ToString();
							string classAssignSearch = " Convert(varchar,a.class) in (";

							if (Session["ClassAssign"].ToString().Contains("12") && (SelList == "4" || SelList == "44"))
							{
								classAssignSearch += "  'Sr.Secondary Open','Sr.Secondary Regular','4','44',";
							}
							if (Session["ClassAssign"].ToString().Contains("10") && (SelList == "2" || SelList == "22"))
							{
								classAssignSearch += " 'Matriculation Open','Matriculation Regular','2','22',";
							}
							if (Session["ClassAssign"].ToString().Contains("11") && SelList == "11")
							{
								classAssignSearch += " '11th Class',";
							}
							if (Session["ClassAssign"].ToString().Contains("9") && SelList == "9")
							{
								classAssignSearch += " '9th Class',";
							}
							if (Session["ClassAssign"].ToString().Contains("5") && SelList == "5")
							{
								classAssignSearch += " 'Primary','5',";
							}
							if (Session["ClassAssign"].ToString().Contains("8") && SelList == "8")
							{
								classAssignSearch += " 'Middle', '8',";
							}

							//classAssignSearch = classAssignSearch.Remove(classAssignSearch.Length - 1, 1);
							//classAssignSearch += " )";
							//Search += " and " + classAssignSearch;


							classAssignSearch = classAssignSearch.Remove(classAssignSearch.Length - 1, 1);
							classAssignSearch += " )";
							Search += " and " + classAssignSearch;
						}


						am.StoreAllData = objDB.GetCorrectionDataFirm(Search, CrType, pageIndex);
						ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
						if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.Message = "Record Not Found";
							ViewBag.TotalCount = 0;
							return View(am);
						}
						else
						{
							ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
							ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
							int tp = Convert.ToInt32(ViewBag.TotalCount);
							int pn = tp / 30;
							int cal = 30 * pn;
							int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
							if (res >= 1)
								ViewBag.pn = pn + 1;
							else
								ViewBag.pn = pn;

							return View(am);
						}
					}

					#endregion Search Record 

					#region Add All Checked

					//NAllChkId - 171007875
					if (cmd == "Reject All Checked")
					{
						string NAllchkid = frc["NAllChkId"];
						string Search = string.Empty;
						string CrType = frc["CorrectionType1"].ToString();
						if (NAllchkid != null)
						{
							NAllchkid = "CorrectionId in (" + NAllchkid + ")";
							string userNM = Session["UserName"].ToString();
							DataSet ds = objDB.AllCancelFirmSchoolCorrection(userNM, CrType, NAllchkid);
						}
						string Searchupdate = string.Empty;
						am.CorrectionLot = frc["CorrectionLot"].ToString();
						Search = "a.correctionlot ='" + am.CorrectionLot + "' ";
						if (Session["DistAllow"].ToString() != "")
						{
							Search += " and b.DIST in(" + Session["DistAllow"].ToString() + ")";
						}
						////------ Paging Srt
						//int pageIndex = 1;
						//pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
						//ViewBag.pagesize = pageIndex;
						//string Catg = CrType;                        

						//---- Paging end
						am.StoreAllData = objDB.GetCorrectionDataFirm(Search, CrType, pageIndex);
						ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
						if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.Message = "Record Not Found";
							ViewBag.TotalCount = 0;
							return View(am);
						}
						else
						{
							return View(am);
						}
					}
					#endregion Add All Checked
					return View(am);
				}
				else
				{
					return View();
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}

		#endregion Admin School Correction

		#region Firm Exam Data Download

		[AdminLoginCheckFilter]
		public ActionResult FirmExamDataDownload(string id, AdminModels am)
		{
			if (Session["UserName"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			string firmuser = Session["UserName"].ToString();
			string ErrStatus = string.Empty;

			var itemsch1 = new SelectList(new[] { new { ID = "1", Name = "Correction Data Only" }, new { ID = "2", Name = "All Updated Data" }, }, "ID", "Name", 1);
			ViewBag.MySchData = itemsch1.ToList();


			var itemsch = new SelectList(new[]{new {ID="1",Name="Regular"},new {ID="2",Name="Open"},
			new {ID="3",Name="Pvt"},}, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			ViewBag.SelectedItem = "0";
			am.StoreAllData = objDB.FirmExamDataDownload(Convert.ToInt32(1), "", firmuser, "", out ErrStatus);
			if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
			{
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
				ViewBag.TotalCount1 = am.StoreAllData.Tables[1].Rows[0]["Total"].ToString();
			}
			else
			{
				ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
				ViewBag.TotalCount1 = am.StoreAllData.Tables[1].Rows[0]["Total"].ToString();
			}
			// PVT

			if (am.StoreAllData == null || am.StoreAllData.Tables[2].Rows.Count == 0)
			{
				ViewBag.TotalCountPvt = 0;
				ViewBag.TotalCount1Pvt = am.StoreAllData.Tables[3].Rows[0]["TotalPvt"].ToString();
			}
			else
			{
				ViewBag.TotalCountPvt = am.StoreAllData.Tables[2].Rows.Count;
				ViewBag.TotalCount1Pvt = am.StoreAllData.Tables[3].Rows[0]["TotalPvt"].ToString();
			}

			if (string.IsNullOrEmpty(id))
			{
				// return RedirectToAction("FirmExamDataDownload", "Admin");
			}
			else
			{
				DataSet ds = null;
				int type = 0;
				string fileName1 = string.Empty;
				string Search = string.Empty;
				int OutStatus = 0;
				string RPLot = "";

				if (id.ToLower() == "pending")
				{
					fileName1 = "Pending_" + firmuser;
					type = 5;
					Search = "std_id like '%%' and DataDownloadLot is null";
				}
				else if (id.ToLower() == "pendingpvt")
				{
					fileName1 = "Pendingpvt_" + firmuser;
					type = 8;
					Search = "std_id like '%%' and DOWNLOT is null";
				}
				else
				{
					string FileExport = id.ToString();
					string splitFile = FileExport.Split('-')[0];
					string splitLot = FileExport.Split('-')[1];
					fileName1 = "LOT" + splitLot;
					if (splitFile.ToLower().Contains("pvt"))
					{
						RPLot = "P";
						if (splitFile.ToLower().Contains("data"))
						{ type = 2; fileName1 += "_DATA_"; }
						else if (splitFile.ToLower().Contains("photo"))
						{ type = 4; fileName1 += "_PHOTO_"; }
						Search = "std_id like '%%'  and DOWNLOT=" + splitLot + "";
					}
					else
					{
						if (splitFile.ToLower().Contains("data"))
						{ type = 2; fileName1 += "_DATA_"; }
						else if (splitFile.ToLower().Contains("subject"))
						{ type = 3; fileName1 += "_SUB_"; }
						else if (splitFile.ToLower().Contains("photo"))
						{ type = 4; fileName1 += "_PHOTO_"; }
						Search = "std_id like '%%'  and DataDownloadLot=" + splitLot + "";
					}
					fileName1 += firmuser + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
				}

				if (type > 0)
				{
					ds = objDB.FirmExamDataDownload(Convert.ToInt32(type), RPLot, firmuser, Search, out ErrStatus); // FirmExamDataDownloadSPNew
					#region Download Data or Subjects
					if (type == 2 || type == 3)
					{
						if (ds == null)
						{
							return RedirectToAction("FirmExamDataDownload", "Admin", new { id = "" });
						}
						else
						{
							if (ds.Tables[0].Rows.Count > 0)
							{
								bool ResultDownload;
								try
								{
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
									ResultDownload = true;
								}
								catch (Exception)
								{
									ResultDownload = false;
								}
							}
						}
					}
					#endregion Data

					#region Download Photo

					else if (type == 4)
					{
						if (ds == null)
						{
							return RedirectToAction("FirmExamDataDownload", "Admin", new { id = "" });
						}
						else
						{
							if (ds.Tables[0].Rows.Count > 0)
							{

								DataTable dt1 = ds.Tables[0];
								string result = objDB.DownloadZIPFileFirmExam(dt1);



							}
						}
					}
					#endregion Data

					#region Pending
					else if (type == 5)
					{
						if (ds == null)
						{
							return View(am);
						}
						else
						{
							if (ds.Tables[0].Rows.Count > 0)
							{
								ViewData["Result"] = "10";
								ViewBag.Message = ds.Tables[0].Rows[0]["DataDownloadLot"].ToString();
							}
						}
					}
					#endregion Pending

					#region Pending Private
					else if (type == 8)
					{
						if (ds == null)
						{
							return View(am);
						}
						else
						{
							if (ds.Tables[0].Rows.Count > 0)
							{
								ViewData["Result"] = "20";
								ViewBag.Message = ds.Tables[0].Rows[0]["DataDownloadLot"].ToString();
							}
						}
					}
					#endregion  Pending Private

				}

			}
			return View(am);
		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult FirmExamDataDownload(string id, AdminModels am, FormCollection frm, string submit)
		{
			if (Session["UserName"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			try
			{
				DataTable dtexport = new DataTable();
				var itemsch1 = new SelectList(new[] { new { ID = "1", Name = "Correction Data Only" }, new { ID = "2", Name = "All Updated Data" }, }, "ID", "Name", 1);
				ViewBag.MySchData = itemsch1.ToList();

				var itemsch = new SelectList(new[]{new {ID="1",Name="Regular"},new {ID="2",Name="Open"},
				new {ID="3",Name="Pvt"},}, "ID", "Name", 1);
				ViewBag.MySch = itemsch.ToList();
				ViewBag.SelectedItem = "0";
				string ErrStatus = string.Empty;
				string Search = string.Empty;
				DataSet ds1 = new DataSet();
				string firmuser = Session["UserName"].ToString();
				am.StoreAllData = objDB.FirmExamDataDownload(Convert.ToInt32(1), "", firmuser, "", out ErrStatus);
				if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
					ViewBag.TotalCount1 = am.StoreAllData.Tables[1].Rows[0]["Total"].ToString();
				}

				string Filevalue = string.Empty;
				if (frm["Filevalue"] == null) { }
				else
				{
					Filevalue = frm["Filevalue"].ToString();
				}

				//  Download Data by file
				string AdminType = Session["AdminType"].ToString();
				int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
				string fileLocation = "";
				string filename = "";
				if (Filevalue.ToUpper() == "STDIDMIS")
				{
					if (am.file != null)
					{
						filename = Path.GetFileName(am.file.FileName);

						DataSet ds = new DataSet();
						if (am.file.ContentLength > 0)  //(Request.Files["file"].ContentLength > 0
						{
							string fileName1 = "FirmExam_" + firmuser + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
							string fileExtension = System.IO.Path.GetExtension(am.file.FileName);
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
								am.file.SaveAs(fileLocation);
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
								using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
								{
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
									string query = string.Format("Select * from [{0}]", excelSheets[0]);
									using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
									{
										dataAdapter.Fill(ds);
									}
								}

								string RP1 = "";
								if (frm["SelList"] != "")
								{
									RP1 = frm["SelList"] == "1" ? "R" : frm["SelList"] == "2" ? "O" : frm["SelList"] == "3" ? "P" : "";
								}

								string CheckMis = "";
								dtexport = null;

								if (!submit.ToLower().Contains("photo"))
								{
									CheckMis = objDB.CheckFirmExamDataDownloadMis(ds, out dtexport, RP1);
								}


								if (CheckMis == "")
								{
									DataTable dt1 = ds.Tables[0];
									if (dt1.Columns.Contains("Status"))
									{
										dt1.Columns.Remove("Status");
									}


									// string Result1 = "";
									int OutStatus = 0;
									// int REGNOLOT = 0;
									string commaString = "";
									for (int i = 0; i < dt1.Rows.Count; i++)
									{

										commaString = commaString + "'" + dt1.Rows[i]["Std_id"].ToString() + "'";
										//commaString += (i < dt1.Rows.Count) ? "," : string.Empty;
										commaString += (i < dt1.Rows.Count - 1) ? "," : string.Empty;
									}

									// Download
									#region DownloadFile

									if (submit.ToUpper().Contains("DOWNLOAD"))
									{
										string result = string.Empty;
										string RP = string.Empty;
										if (frm["SelList"] != "")
										{
											ViewBag.SelectedItem = frm["SelList"];
											TempData["SelectedItem"] = frm["SelList"];
											RP = frm["SelList"] == "1" ? "R" : frm["SelList"] == "2" ? "O" : frm["SelList"] == "3" ? "P" : "";
										}
										Search = "std_id like '%%' and std_id in (" + commaString + ")";

										if (submit.ToLower().Contains("data"))
										{
											ds1 = objDB.FirmExamDataDownload(Convert.ToInt32(2), RP, firmuser, Search, out ErrStatus); // FirmExamDataDownloadSPNew
										}
										else if (submit.ToLower().Contains("subjects"))
										{
											ds1 = objDB.FirmExamDataDownload(Convert.ToInt32(6), RP, firmuser, Search, out ErrStatus); // FirmExamDataDownloadSPNew
										}
										else if (submit.ToLower().Contains("photo"))
										{
											ds1 = objDB.FirmExamDataDownload(Convert.ToInt32(7), RP, firmuser, Search, out ErrStatus); // FirmExamDataDownloadSPNew
										}
										// DataSet ds1 = objDB.DownloadRegNoAgainstID(commaString, "O", out OutStatus); // For all Regno alloted 
										fileName1 = submit.Replace(" ", "_") + "_" + firmuser + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

										if (ds1.Tables[0].Rows.Count > 0)
										{
											ViewBag.Message = "Exam Data Downloaded Successfully";
											ViewData["Result"] = "1";
											ViewBag.TotalCount12 = ds1.Tables[0].Rows.Count;
											if (submit.ToLower().Contains("photo"))
											{
												if (ds1 == null)
												{
													return RedirectToAction("FirmExamDataDownload", "Admin", new { id = "" });
												}
												else
												{
													if (ds1.Tables[0].Rows.Count > 0)
													{
														result = objDB.DownloadZIPFileFirmExam(ds1.Tables[0]);
													}
												}
											}
											else
											{

												//DataTable dt = ds1.Tables[0];
												fileName1 = fileName1 + ".xls";
												using (XLWorkbook wb = new XLWorkbook())
												{
													wb.Worksheets.Add(ds1);
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
									#endregion DownloadFile                          
									return View(am);
								}
								else
								{
									if (dtexport != null)
									{
										ExportDataFromDataTable(dtexport, "Error_FirmDataDownload");
									}
									ViewData["Result"] = "-1";
									ViewBag.Message = CheckMis;
									return View(am);
								}
							}
							else
							{

								ViewData["Result"] = "-2";
								ViewBag.Message = "Please Upload Only .xls file only";
								return View(am);
							}
						}

					}
					else
					{
						//ViewData["Result"] = "-4";
						// ViewBag.Message = "Please select .xls file only";
						//return View();
					}
				}
				else if (Filevalue.ToUpper() == "DATE")
				{
					#region DownloadFile

					if (submit.ToUpper().Contains("DOWNLOAD"))
					{
						string type1 = string.Empty;
						string RP = string.Empty;
						Search = "a.std_id like '%%' ";

						if (frm["SelData"] != "")
						{
							ViewBag.SelectedItemData = type1 = frm["SelData"];
							TempData["SelectedItemData"] = frm["SelData"];
						}


						if (frm["SelList"] != "")
						{
							ViewBag.SelectedItem = frm["SelList"];
							TempData["SelectedItem"] = frm["SelList"];
							RP = frm["SelList"] == "1" ? "R" : frm["SelList"] == "2" ? "O" : frm["SelList"] == "3" ? "P" : "";
						}
						// updt between convert(date,'21/07/2017',103) and convert(date,'01/08/2017',103)
						if (frm["FromDate"] != "")
						{
							ViewBag.FromDate = frm["FromDate"];
							TempData["FromDate"] = frm["FromDate"];
							if (frm["SelList"] == "3")
							{ Search += " and  CONVERT(DATETIME, DATEDIFF(DAY, 0, UPDT)) >=convert(DATETIME,'" + frm["FromDate"].ToString() + "',103) "; }
							else
							{
								if (type1 == "1")
								{
									Search += " and  CONVERT(DATETIME, DATEDIFF(DAY, 0, FirmCorrectionLotDT)) >=convert(DATETIME,'" + frm["FromDate"].ToString() + "',103) ";
								}
								else
								{ Search += " and  CONVERT(DATETIME, DATEDIFF(DAY, 0, UPDT)) >=convert(DATETIME,'" + frm["FromDate"].ToString() + "',103) "; }

							}
						}
						if (frm["ToDate"] != "")
						{
							ViewBag.ToDate = frm["ToDate"];
							TempData["ToDate"] = frm["ToDate"];
							// Search += " and  convert(date,'" + frm["ToDate"].ToString() + "',103)";
							//Search += " and  format(UPDT,'dd/MM/yyyy') <='" + frm["ToDate"].ToString() + "'";
							if (frm["SelList"] == "3")
							{
								Search += "   and CONVERT(DATETIME, DATEDIFF(DAY, 0,UPDT)) <=  convert(DATETIME,'" + frm["ToDate"].ToString() + "',103)";
							}
							else
							{

								if (type1 == "1")
								{
									Search += "   and CONVERT(DATETIME, DATEDIFF(DAY, 0,FirmCorrectionLotDT)) <=  convert(DATETIME,'" + frm["ToDate"].ToString() + "',103)";

								}
								else
								{ Search += "   and CONVERT(DATETIME, DATEDIFF(DAY, 0,UPDT)) <=  convert(DATETIME,'" + frm["ToDate"].ToString() + "',103)"; }

							}
						}

						if (type1 == "1")
						{
							if (submit.ToLower().Contains("data"))
							{
								ds1 = objDB.FirmExamDataDownload(Convert.ToInt32(12), RP, firmuser, Search, out ErrStatus); // FirmExamDataDownloadSPNew
							}
							else if (submit.ToLower().Contains("subjects"))
							{
								ds1 = objDB.FirmExamDataDownload(Convert.ToInt32(13), RP, firmuser, Search, out ErrStatus); // FirmExamDataDownloadSPNew
							}
							else if (submit.ToLower().Contains("photo"))
							{
								ds1 = objDB.FirmExamDataDownload(Convert.ToInt32(14), RP, firmuser, Search, out ErrStatus); // FirmExamDataDownloadSPNew
							}
						}
						else
						{
							if (submit.ToLower().Contains("data"))
							{
								ds1 = objDB.FirmExamDataDownload(Convert.ToInt32(2), RP, firmuser, Search, out ErrStatus); // FirmExamDataDownloadSPNew
							}
							else if (submit.ToLower().Contains("subjects"))
							{
								ds1 = objDB.FirmExamDataDownload(Convert.ToInt32(3), RP, firmuser, Search, out ErrStatus); // FirmExamDataDownloadSPNew
							}
							else if (submit.ToLower().Contains("photo"))
							{
								ds1 = objDB.FirmExamDataDownload(Convert.ToInt32(4), RP, firmuser, Search, out ErrStatus); // FirmExamDataDownloadSPNew
							}
						}
						// DataSet ds1 = objDB.DownloadRegNoAgainstID(commaString, "O", out OutStatus); // For all Regno alloted 
						string fileName1 = submit.Replace(" ", "_") + "_" + firmuser + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

						if (ds1 == null)
						{
							ViewBag.Message = ErrStatus.ToString();
							ViewData["Result"] = "5";
							ViewBag.TotalCount13 = 0;
						}
						else if (ds1.Tables[0].Rows.Count == 0)
						{
							ViewData["Result"] = "15";
							ViewBag.TotalCount13 = 0;
						}
						else if (ds1.Tables[0].Rows.Count > 0)
						{
							ViewBag.Message = "Exam Data Downloaded Successfully";
							ViewData["Result"] = "1";
							ViewBag.TotalCount13 = ds1.Tables[0].Rows.Count;
							if (submit.ToLower().Contains("photo"))
							{
								if (ds1 == null)
								{
									return RedirectToAction("FirmExamDataDownload", "Admin", new { id = "" });
								}
								else
								{
									if (ds1.Tables[0].Rows.Count > 0)
									{
										string result = objDB.DownloadZIPFileFirmExam(ds1.Tables[0]);
									}
								}
							}
							else
							{

								fileName1 = fileName1 + ".xls";
								using (XLWorkbook wb = new XLWorkbook())
								{
									wb.Worksheets.Add(ds1);
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
					#endregion DownloadFile                          
				}
				return View(am);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewBag.Message = "Error: " + ex.Message;
				ViewData["Result"] = "50";
				return View();
			}
		}

		#endregion Firm Exam Data Download



		#region Rohit FirmSchoolCorrection
		public ActionResult FirmSchoolCorrection(int? page)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
			try
			{
				string classAssign = "";
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				if (Session["ClassAssign"] == null || Session["ClassAssign"] == "")
				{
					ViewBag.Result = "10";
					ViewBag.TotalCount = 0;
					return View(am);
				}
				FormCollection frc = new FormCollection();
				var itemsch = new SelectList(new[]{new {ID="1",Name="Particular"},new {ID="2",Name="Subject"},
			new{ID="4",Name="Image"},}, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();

				//Bind Class asign by admin id
				ViewBag.MySch = AbstractLayer.DBClass.GetCorrectionClassAssignListByAdminId(Session["ClassAssign"].ToString()).ToList();

				if (ModelState.IsValid)
				{
					if (Session["CorLot"] != null)
					{
						string CorLot2 = Session["CorLot"].ToString();
						ViewBag.SelectedItemcode = Session["CorrectionType1"].ToString();
						am.CorrectionLot = Session["CorLot"].ToString();
						string Search = string.Empty;
						string CrType = Session["CorrectionType1"].ToString();
						//ViewBag.CorrectionType = am.CorrectionType;
						Search = "a.correctionlot='" + CorLot2 + "' ";
						if (Session["DistAllow"].ToString() != "")
						{
							Search += " and b.DIST in(" + Session["DistAllow"].ToString() + ")";
						}

						//if (Session["UserName"].ToString() == "CREA" || Session["UserName"].ToString() == "PERF")
						//{
						//    Search += " and a.class in('Sr.Secondary Open','Sr.Secondary Regular')";
						//}
						//else if (Session["UserName"].ToString() == "DATA" || Session["UserName"].ToString() == "SAI")
						//{
						//    Search += " and a.class in('Matriculation Open','Matriculation Regular')";
						//}
						//else

						if (Session["ClassAssign"] != null)
						{
							classAssign = Session["ClassAssign"].ToString();
							string classAssignSearch = " Convert(varchar,a.class) in (";

							if (Session["ClassAssign"].ToString().Contains("12"))
							{
								classAssignSearch += "  'Sr.Secondary Open','Sr.Secondary Regular','4','44',";
							}
							if (Session["ClassAssign"].ToString().Contains("10"))
							{
								classAssignSearch += " 'Matriculation Open','Matriculation Regular','2','22',";
							}
							if (Session["ClassAssign"].ToString().Contains("11"))
							{
								classAssignSearch += " '11th Class',";
							}
							if (Session["ClassAssign"].ToString().Contains("9"))
							{
								classAssignSearch += " '9th Class',";
							}
							if (Session["ClassAssign"].ToString().Contains("5"))
							{
								classAssignSearch += " 'Primary','5',";
							}
							if (Session["ClassAssign"].ToString().Contains("8"))
							{
								classAssignSearch += " 'Middle', '8',";
							}

							classAssignSearch = classAssignSearch.Remove(classAssignSearch.Length - 1, 1);
							classAssignSearch += " )";
							Search += " and " + classAssignSearch;
						}

						//------ Paging Srt
						int pageIndex = 1;
						pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
						ViewBag.pagesize = pageIndex;
						//string Catg = CrType;                        

						//---- Paging end
						am.StoreAllData = objDB.GetCorrectionDataFirm(Search, CrType, pageIndex);
						ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
						if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.Message = "Record Not Found";
							ViewBag.TotalCount = 0;
							return View(am);
						}
						else
						{
							ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
							ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
							int tp = Convert.ToInt32(ViewBag.TotalCount);
							int pn = tp / 30;
							int cal = 30 * pn;
							int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
							if (res >= 1)
								ViewBag.pn = pn + 1;
							else
								ViewBag.pn = pn;

							return View(am);
						}
					}
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View();


				}
				else
				{
					return View();
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}


		[HttpPost]
		public ActionResult FirmSchoolCorrection(int? page, FormCollection frc, string cmd, string id, string SelList)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
			try
			{


				FormCollection frm = new FormCollection();
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				string classAssign = "";
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}

				if (Session["ClassAssign"] == null || Session["ClassAssign"] == "")
				{
					ViewBag.Result = "10";
					ViewBag.TotalCount = 0;
					return View(am);
				}

				var itemsch = new SelectList(new[]{new {ID="1",Name="Particular"},new {ID="2",Name="Subject"},
			new{ID="4",Name="Image"},}, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();
				//Bind Class asign by admin id
				ViewBag.MySch = AbstractLayer.DBClass.GetCorrectionClassAssignListByAdminId(Session["ClassAssign"].ToString()).ToList();

				if (ModelState.IsValid)
				{
					//------ Paging Srt
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;
					//string Catg = CrType;                        

					//---- Paging end

					#region Search Record 
					ViewBag.SelectedItemcode = frc["CorrectionType1"].ToString();
					if (cmd == "Search" && frc["CorrectionType1"].ToString() != "")
					{
						string Search = string.Empty;
						string CrType = frc["CorrectionType1"].ToString();
						Session["CorrectionType1"] = ViewBag.SelectedItemcode;
						am.CorrectionLot = frc["CorrectionLot"].ToString();
						Session["CorLot"] = frc["CorrectionLot"].ToString();
						//ViewBag.CorrectionType = frc["CorrectionType"].ToString();

						Search = "a.correctionlot ='" + am.CorrectionLot + "' ";
						if (Session["DistAllow"].ToString() != "")
						{
							Search += " and b.DIST in(" + Session["DistAllow"].ToString() + ")";
						}

						//if (!string.IsNullOrEmpty(SelList))
						//{
						//    Search += " and  Convert(varchar,a.class) in ('" + SelList + "')";                          

						//}

						if (Session["ClassAssign"] != null)
						{
							classAssign = Session["ClassAssign"].ToString();
							string classAssignSearch = " Convert(varchar,a.class) in (";

							if (Session["ClassAssign"].ToString().Contains("12") && (SelList == "4" || SelList == "44"))
							{
								classAssignSearch += "  'Sr.Secondary Open','Sr.Secondary Regular','4','44',";
							}
							if (Session["ClassAssign"].ToString().Contains("10") && (SelList == "2" || SelList == "22"))
							{
								classAssignSearch += " 'Matriculation Open','Matriculation Regular','2','22',";
							}
							if (Session["ClassAssign"].ToString().Contains("11") && SelList == "11")
							{
								classAssignSearch += " '11th Class',";
							}
							if (Session["ClassAssign"].ToString().Contains("9") && SelList == "9")
							{
								classAssignSearch += " '9th Class',";
							}
							if (Session["ClassAssign"].ToString().Contains("5") && SelList == "5")
							{
								classAssignSearch += " 'Primary','5',";
							}
							if (Session["ClassAssign"].ToString().Contains("8") && SelList == "8")
							{
								classAssignSearch += " 'Middle', '8',";
							}

							classAssignSearch = classAssignSearch.Remove(classAssignSearch.Length - 1, 1);
							classAssignSearch += " )";
							Search += " and " + classAssignSearch;
						}

						am.StoreAllData = objDB.GetCorrectionDataFirm(Search, CrType, pageIndex);
						ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
						if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.Message = "Record Not Found";
							ViewBag.TotalCount = 0;
							return View(am);
						}
						else
						{
							ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
							ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
							int tp = Convert.ToInt32(ViewBag.TotalCount);
							int pn = tp / 30;
							int cal = 30 * pn;
							int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
							if (res >= 1)
								ViewBag.pn = pn + 1;
							else
								ViewBag.pn = pn;

							return View(am);
						}
					}

					#endregion Search Record                   
					return View(am);
				}
				else
				{
					return View();
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}

		[HttpPost]
		public ActionResult CorrLotAcceptReject(string correctionType, string correctionLot, string acceptid, string rejectid, string removeid, string remarksid)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
			string remstatus = "";
			string status = "";
			string adminid = Session["adminid"].ToString();
			string adminName = Session["UserName"].ToString();

			if (removeid != "")
			{
				string OutStatus = "0";
				status = objDB.CorrLotAcceptReject(adminLoginSession.AdminEmployeeUserId, correctionType, correctionLot, acceptid, rejectid, removeid, adminName, out OutStatus);
				var results = new
				{
					status = OutStatus,
				};

				return Json(results);
			}
			else
			{

				if (correctionLot == null || correctionLot == "")
				{
					var results = new
					{
						status = ""
					};
					return Json(results);
				}
				else
				{

					string OutStatus = "0";
					status = objDB.CorrLotAcceptReject(adminLoginSession.AdminEmployeeUserId, correctionType, correctionLot, acceptid, rejectid, removeid, adminName, out OutStatus);
					if (status == "1" && remarksid != "")
					{
						//123(rohit),456(mm)
						string[] split1 = remarksid.Split(',');
						int sCount = split1.Length;
						if (sCount > 0)
						{
							foreach (string s in split1)
							{
								string corid = s.Split('(')[0];
								string remark = s.Split('(', ')')[1];
								if (corid != "")
								{
									remstatus = objDB.CorrLotRejectRemarksSP(corid, remark, adminid);//CorrLotRejectRemarksSP
								}
							}
						}
					}
					var results = new
					{
						status = OutStatus,
					};
					return Json(results);
				}
			}
		}

		#endregion
		#region  Rohit Firm School Correction Updated 
		[HttpPost]
		public JsonResult GetCorrectionLot(string Prefix)
		{
			List<AdminModels> listResult = new List<AdminModels>();
			string Search = string.Empty;
			string CrType = "2";
			Search = "a.status is not null";
			string UserName = Session["UserName"].ToString();
			Search += " and a.FirmUser  = '" + Session["UserName"].ToString() + "'";
			if (Session["DistAllow"].ToString() != "")
			{
				Search += " and s.DIST in(" + Session["DistAllow"].ToString() + ")";
			}

			DataSet ds = objDB.GetCorrectionDataFirmUpdatedByAdmin(Search, CrType, 1);
			// DataSet ds = objDB.SelectSchoolDatabyID(schoolcode);    //SelectSchoolDatabyID 
			if (ds.Tables.Count > 0)
			{
				if (ds.Tables[3].Rows.Count > 0)
				{
					listResult = ds.Tables[3].AsEnumerable().Select(row => new AdminModels
					{
						CorrectionLot = String.IsNullOrEmpty(row.Field<string>("CorrectionLot")) ? "not found" : row.Field<string>("CorrectionLot"),
					}).ToList();
				}
			}
			var _list = (from N in listResult
						 where N.CorrectionLot.ToLower().StartsWith(Prefix.ToLower())
						 select new { N.CorrectionLot });
			return Json(_list, JsonRequestBehavior.AllowGet);
		}


		public ActionResult FirmSchoolCorrectionUpdated(int? page)
		{
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				var itemFee = new SelectList(new[] { new { ID = "1", Name = "With Fee" }, new { ID = "2", Name = "Without Fee" }, }, "ID", "Name", 1);
				ViewBag.FeeType = itemFee.ToList();
				ViewBag.SelectedFeeType = "1";

				FormCollection frc = new FormCollection();
				var itemsch = new SelectList(new[]{new {ID="1",Name="Particular"},new {ID="2",Name="Subject"},
				new{ID="4",Name="Image"},}, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();

				//Bind Class asign by admin id
				ViewBag.MySch = AbstractLayer.DBClass.GetCorrectionClassAssignListByAdminId(Session["ClassAssign"].ToString()).ToList();


				string classAssign = "";
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				if (ModelState.IsValid)
				{
					string Search = string.Empty;
					string CrType = "2";
					Search = "a.status is not null";
					string UserName = Session["UserName"].ToString();
					Search += " and a.FirmUser  = '" + Session["UserName"].ToString() + "'";
					if (Session["DistAllow"].ToString() != "")
					{
						Search += " and s.DIST in(" + Session["DistAllow"].ToString() + ")";
					}

					if (Session["ClassAssign"] != null)
					{
						classAssign = Session["ClassAssign"].ToString();
						string classAssignSearch = "Convert(varchar,a.class) in (";

						if (Session["ClassAssign"].ToString().Contains("12"))
						{
							classAssignSearch += "  'Sr.Secondary Open','Sr.Secondary Regular','4','44',";
						}
						if (Session["ClassAssign"].ToString().Contains("10"))
						{
							classAssignSearch += " 'Matriculation Open','Matriculation Regular','2','22',";
						}
						if (Session["ClassAssign"].ToString().Contains("11"))
						{
							classAssignSearch += " '11th Class',";
						}
						if (Session["ClassAssign"].ToString().Contains("9"))
						{
							classAssignSearch += " '9th Class',";
						}
						if (Session["ClassAssign"].ToString().Contains("5"))
						{
							classAssignSearch += " 'Primary','5',";
						}
						if (Session["ClassAssign"].ToString().Contains("8"))
						{
							classAssignSearch += " 'Middle', '8',";
						}

						classAssignSearch = classAssignSearch.Remove(classAssignSearch.Length - 1, 1);
						classAssignSearch += " )";
						Search += " and " + classAssignSearch;
					}

					//------ Paging Srt
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;
					//---- Paging end
					am.StoreAllData = objDB.GetCorrectionDataFirmUpdatedByAdmin(Search, CrType, pageIndex);
					// ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;

					if (am.StoreAllData == null)
					{
						ViewBag.TotalCountP = 0;
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						return View(am);
					}
					else
					{
						DataSet dschk = objDB.CheckFeeAllCorrectionDataByFirmSP(1, UserName, "");// check fee exist 
						ViewBag.TotalCount = 0;//am.StoreAllData.Tables[0].Rows.Count;
						if (dschk == null || dschk.Tables[0].Rows.Count == 0)
						{
							ViewBag.IsFeeExists = "0";
						}
						else
						{ ViewBag.IsFeeExists = "1"; }
						//
						if (am.StoreAllData.Tables[2].Rows.Count > 0)
						{
							ViewBag.CorrectionFeeDate = am.StoreAllData.Tables[2].Rows[0]["CorrectionFeeDate"].ToString();
							ViewBag.CorrectionFeeDateStatus = am.StoreAllData.Tables[2].Rows[0]["CorrectionFeeDateStatus"].ToString();
						}

						if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.TotalCountP = 0;
							ViewBag.Message = "Record Not Found";
							ViewBag.TotalCount = 0;
							return View(am);
						}
						else
						{
							ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
							ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
							ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);
							Session["ForFinalCorrectionLot"] = String.Join(",", am.StoreAllData.Tables[3].AsEnumerable().Select(x => x.Field<string>("CorrectionLot").ToString()).ToArray());
							int tp = Convert.ToInt32(ViewBag.TotalCount);
							int pn = tp / 30;
							int cal = 30 * pn;
							int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
							if (res >= 1)
								ViewBag.pn = pn + 1;
							else
								ViewBag.pn = pn;

							return View(am);
						}

					}

				}
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
				return View();


				//}
				//else
				//{
				//    return View();
				//}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Lout", "Login");
				return View();
			}
		}
		[HttpPost]
		public ActionResult FirmSchoolCorrectionUpdated(int? page, FormCollection frc, string cmd)
		{

			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				string UserName = Session["UserName"].ToString();
				FormCollection frm = new FormCollection();
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				var itemsch = new SelectList(new[] { new { ID = "1", Name = "Particular" }, new { ID = "2", Name = "Subject" }, new { ID = "4", Name = "Image" }, }, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();

				//Bind Class asign by admin id
				ViewBag.MySch = AbstractLayer.DBClass.GetCorrectionClassAssignListByAdminId(Session["ClassAssign"].ToString()).ToList();


				var itemFee = new SelectList(new[] { new { ID = "1", Name = "With Fee" }, new { ID = "2", Name = "Without Fee" }, }, "ID", "Name", 1);
				ViewBag.FeeType = itemFee.ToList();
				ViewBag.SelectedFeeType = "1";
				string classAssign = "";
				//------ Paging Srt
				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;

				#region Submit Fee  Only
				// if (cmd == "Submit Fee")
				if (cmd.ToLower() == "submit fee only")
				{

					if (Session["ForFinalCorrectionLot"] == null)
					{ ViewData["FeeUpdate"] = "10"; }

					string ForFinalCorrectionLot = Session["ForFinalCorrectionLot"].ToString();

					ViewBag.ForFinalCorrectionLot = am.CorrectionLot = ForFinalCorrectionLot;
					am.CorrectionRecieptNo = frc["CorrectionRecieptNo"].ToString();
					am.CorrectionRecieptDate = frc["CorrectionRecieptDate"].ToString();
					am.CorrectionNoCapproved = frc["CorrectionNoCapproved"].ToString();
					am.CorrectionAmount = frc["CorrectionAmount"].ToString();

					if (am.CorrectionRecieptNo != "")
					{
						DataSet dsFeeUpdate = objDB.SetCorrectionDataFirmFeeDetails(am, UserName, adminLoginSession.AdminEmployeeUserId);
						if (dsFeeUpdate == null || dsFeeUpdate.Tables[0].Rows.Count == 0)
						{
							ViewData["FeeUpdate"] = null;
						}
						else
						{
							ViewData["FeeUpdate"] = dsFeeUpdate.Tables[0].Rows[0]["InsUpt"].ToString();
						}
					}
					else { ViewData["FeeUpdate"] = null; }
				}

				#endregion Submit Fee  Only



				#region Submit Fee & Final Submission
				// if (cmd == "Submit Fee")
				if (cmd.ToLower().Contains("final"))
				{

					//DataSet dsFeeUpdate = objDB.CheckCorrectionFee(userNM, "");

					if (Session["ForFinalCorrectionLot"] == null)
					{ ViewData["FeeUpdate"] = "10"; }

					string ForFinalCorrectionLot = Session["ForFinalCorrectionLot"].ToString();
					ViewBag.ForFinalCorrectionLot = am.CorrectionLot = ForFinalCorrectionLot;
					am.CorrectionRecieptNo = frc["CorrectionRecieptNo"].ToString();
					am.CorrectionRecieptDate = frc["CorrectionRecieptDate"].ToString();
					am.CorrectionNoCapproved = frc["CorrectionNoCapproved"].ToString();
					am.CorrectionAmount = frc["CorrectionAmount"].ToString();

					if (am.CorrectionRecieptNo != "")
					{
						DataSet dsFeeUpdate = objDB.SetCorrectionDataFirmFeeDetails(am, UserName, adminLoginSession.AdminEmployeeUserId);
						if (dsFeeUpdate == null || dsFeeUpdate.Tables[0].Rows.Count == 0)
						{
							ViewData["FeeUpdate"] = null;
						}
						else
						{

							ViewData["FeeUpdate"] = dsFeeUpdate.Tables[0].Rows[0]["InsUpt"].ToString();
							// fee submit and final submit
							if (dsFeeUpdate.Tables[0].Rows[0]["InsUpt"].ToString() == "1" || dsFeeUpdate.Tables[0].Rows[0]["InsUpt"].ToString() == "2")
							{

								DataSet dschk = objDB.CheckFeeAllCorrectionDataByFirmSP(6, UserName, ForFinalCorrectionLot);// check fee exist 
								ViewBag.TotalCount = 0;//am.StoreAllData.Tables[0].Rows.Count;
								if (dschk == null || dschk.Tables[0].Rows.Count == 0)
								{
									ViewData["FeeUpdate"] = "5";
									ViewBag.commaCorrectionLot = ForFinalCorrectionLot;
								}
								else
								{
									// final submit here   
									string FirmCorrectionLot = string.Empty;
									string OutError = string.Empty;
									DataSet ds1 = objDB.CorrectionDataFirmFinalSubmitSPRNByCorrectionLot(adminLoginSession.AdminEmployeeUserId, ForFinalCorrectionLot, UserName, out FirmCorrectionLot, out OutError);  // Final Submit Main Function                       
									if (FirmCorrectionLot.Length > 2)
									{
										ViewBag.TotalCount = 1;
										ViewData["Status"] = "1";
										ViewData["FeeUpdate"] = "20";
										ViewData["Message"] = "";
									}
									else
									{
										ViewBag.TotalCount = 0;
										ViewData["Status"] = "0";
										ViewData["FeeUpdate"] = "21";
										ViewData["Message"] = OutError;
									}

								}
							}
							//

						}
					}
					else { ViewData["FeeUpdate"] = null; }
				}

				#endregion Submit Fee & Final Submission



				//---- Paging end                   
				#region View All Correction Pending Record                
				string Search = string.Empty;
				string CrType = "2";
				Search = "a.status is not null";

				if (frc["CorrectionLot"] != null)
				{
					ViewBag.CorrectionLot = am.CorrectionLot = frc["CorrectionLot"].ToString();
					Search += " and a.correctionlot ='" + am.CorrectionLot + "' ";

				}

				if (frc["FeeType"] != null)
				{
					ViewBag.SelectedFeeType = frc["FeeType"].ToString().Trim();
				}

				Search += " and a.FirmUser  ='" + Session["UserName"].ToString() + "'";

				//Search = "a.status is null and a.CorrectionLot= '"+am.CorrectionLot+"'";
				if (Session["DistAllow"].ToString() != "")
				{
					Search += " and s.DIST in(" + Session["DistAllow"].ToString() + ")";
				}
				if (Session["ClassAssign"] != null)
				{
					classAssign = Session["ClassAssign"].ToString();
					string classAssignSearch = "Convert(varchar,a.class) in (";

					if (Session["ClassAssign"].ToString().Contains("12"))
					{
						classAssignSearch += "  'Sr.Secondary Open','Sr.Secondary Regular','4','44',";
					}
					if (Session["ClassAssign"].ToString().Contains("10"))
					{
						classAssignSearch += " 'Matriculation Open','Matriculation Regular','2','22',";
					}
					if (Session["ClassAssign"].ToString().Contains("11"))
					{
						classAssignSearch += " '11th Class',";
					}
					if (Session["ClassAssign"].ToString().Contains("9"))
					{
						classAssignSearch += " '9th Class',";
					}
					if (Session["ClassAssign"].ToString().Contains("5"))
					{
						classAssignSearch += " 'Primary','5',";
					}
					if (Session["ClassAssign"].ToString().Contains("8"))
					{
						classAssignSearch += " 'Middle', '8',";
					}

					classAssignSearch = classAssignSearch.Remove(classAssignSearch.Length - 1, 1);
					classAssignSearch += " )";
					Search += " and " + classAssignSearch;
				}

				am.StoreAllData = objDB.GetCorrectionDataFirmUpdatedByAdmin(Search, CrType, pageIndex);
				//

				if (am.StoreAllData == null)
				{
					ViewBag.TotalCountP = 0;
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(am);
				}
				else
				{

					DataSet dschk = objDB.CheckFeeAllCorrectionDataByFirmSP(1, UserName, "");// check fee exist 
					ViewBag.TotalCount = 0;//am.StoreAllData.Tables[0].Rows.Count;
					if (dschk == null || dschk.Tables[0].Rows.Count == 0)
					{
						ViewBag.IsFeeExists = "0";
					}
					else
					{ ViewBag.IsFeeExists = "1"; }
					//
					if (am.StoreAllData.Tables[2].Rows.Count > 0)
					{
						ViewBag.CorrectionFeeDate = am.StoreAllData.Tables[2].Rows[0]["CorrectionFeeDate"].ToString();
						ViewBag.CorrectionFeeDateStatus = am.StoreAllData.Tables[2].Rows[0]["CorrectionFeeDateStatus"].ToString();
					}

					if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.TotalCountP = 0;
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
					}
					else
					{
						ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
						ViewBag.TotalCount = Convert.ToInt32(am.StoreAllData.Tables[1].Rows[0]["totalCount"].ToString());
						ViewBag.TotalCount1 = Convert.ToInt32(ViewBag.TotalCountP);

						if (frc["CorrectionLot"] != null)
						{
							ViewBag.FeeStatus = Convert.ToInt32(am.StoreAllData.Tables[0].Rows[0]["FeeStatus"].ToString());
							ViewBag.FirmCorrectionLot = Convert.ToString(am.StoreAllData.Tables[0].Rows[0]["FirmCorrectionLot"].ToString());

						}
						else
						{
							ViewBag.FeeStatus = null;
							ViewBag.FirmCorrectionLot = null;
						}

						Session["ForFinalCorrectionLot"] = String.Join(",", am.StoreAllData.Tables[3].AsEnumerable().Select(x => x.Field<string>("CorrectionLot").ToString()).ToArray());
						int tp = Convert.ToInt32(ViewBag.TotalCount);
						int pn = tp / 30;
						int cal = 30 * pn;
						int res = Convert.ToInt32(ViewBag.TotalCount) - cal;
						if (res >= 1)
							ViewBag.pn = pn + 1;
						else
							ViewBag.pn = pn;
					}
				}

				#endregion View All Correction Pending Record

				return View(am);

			}
			catch (Exception ex)
			{
				return View();
			}
		}

		#endregion Rohit Firm School Correction Updated 

		#region Firm Finaal Submit Rohit
		public ActionResult FirmSchoolCorrectionFinalSubmit(string id)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
			try
			{

				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				if (id == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				string UserName = Session["UserName"].ToString();
				FormCollection frm = new FormCollection();
				var itemsch = new SelectList(new[]{new {ID="1",Name="Particular"},new {ID="2",Name="Subject"},new{ID="3",Name="Stream"},
			   new{ID="4",Name="Image"},}, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch.ToList();
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels am = new AdminModels();
				if (ModelState.IsValid)
				{
					if (id.ToLower() == "finalsubmit")
					{
						DataSet dschk = objDB.CheckFeeAllCorrectionDataByFirmSP(1, UserName, "");// check fee exist 
						ViewBag.TotalCount = 0;//am.StoreAllData.Tables[0].Rows.Count;
						if (dschk == null || dschk.Tables[0].Rows.Count == 0)
						{
							// final submit here   

							string FirmCorrectionLot = string.Empty;
							string OutError = string.Empty;
							DataSet ds1 = objDB.CorrectionDataFirmFinalSubmitSPRN(adminLoginSession.AdminEmployeeUserId, UserName, out FirmCorrectionLot, out OutError);  // Final Submit Main Function                       
							if (FirmCorrectionLot.Length > 2)
							{
								ViewBag.TotalCount = 1;
								ViewData["Status"] = "1";
								ViewData["Message"] = "";

								am.StoreAllData = objDB.CheckFeeAllCorrectionDataByFirmSP(5, UserName, "");
								if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
								{
									ViewBag.TotalCount = 0;
									ViewData["Status"] = "10";
								}
								else
								{

									ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
									ViewData["Status"] = "11";
								}

							}
							else
							{
								ViewBag.TotalCount = 0;
								ViewData["Status"] = "0";
								ViewData["Message"] = OutError;
							}
							//  am.StoreAllData = objDB.GetAllCorrectionDataFirm(UserName);


							return View(am);

						}
						else
						{
							ViewBag.commaCorrectionLot = String.Join(",", dschk.Tables[0].AsEnumerable().Select(x => x.Field<string>("CorrectionLot").ToString()).ToArray());
							ViewBag.TotalCount = 1;
							ViewData["Status"] = "5";

						}
					}
					else if (id.ToLower() == "viewall")
					{
						am.StoreAllData = objDB.CheckFeeAllCorrectionDataByFirmSP(5, UserName, "");
						if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.TotalCount = 0;
							ViewData["Status"] = "10";
						}
						else
						{

							ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;
							ViewData["Status"] = "11";
						}
					}

					else if (id.ToLower().Contains("pendingfee"))
					{
						am.StoreAllData = objDB.CheckFeeAllCorrectionDataByFirmSP(7, UserName, "");
						if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.TotalCount = 0;
							ViewData["Status"] = "10";
						}
						else
						{

							ViewBag.TotalCount = am.StoreAllData.Tables[0].Rows.Count;

							using (XLWorkbook wb = new XLWorkbook())
							{
								wb.Worksheets.Add(am.StoreAllData.Tables[0]);
								wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
								wb.Style.Font.Bold = true;
								Response.Clear();
								Response.Buffer = true;
								Response.Charset = "";
								Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
								Response.AddHeader("content-disposition", "attachment;filename=" + "PendingFee_CorrectionLotList.xls");
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
					return View(am);
				}
				else
				{
					return View();
				}
			}
			catch (Exception ex)
			{
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}

		public ActionResult DownloadFirmCorrectionLot()
		{
			try
			{
				if (Request.QueryString["File"] == null)
				{
					return RedirectToAction("FirmSchoolCorrectionFinalSubmit", "Admin");
				}
				else
				{
					string FileExport = Request.QueryString["File"].ToString();
					DataSet ds = null;
					if (Session["UserName"] != null)
					{
						string UserName = Session["UserName"].ToString();
						string AdminType = Session["AdminType"].ToString();
						int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
						string fileName1 = string.Empty;
						string Search = string.Empty;
						int OutStatus = 0;
						ds = objDB.CheckFeeAllCorrectionDataByFirmSP(5, UserName, FileExport);
						// ds = objDB.GetPendingREGNOByAdminId(AdminId, "O", out OutStatus); // GetPendingREGNOByAdminIdSP                  
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
				return RedirectToAction("FirmSchoolCorrectionFinalSubmit", "Admin");
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return RedirectToAction("FirmSchoolCorrectionFinalSubmit", "Admin");
			}
		}


		public ActionResult SchoolCorrectionStatus(string id)
		{
			try
			{
				AdminModels rm = new AdminModels();
				DataSet result = new DataSet();
				string schlid = string.Empty;
				string Search = "schl like '%%' ";


				var itemsch1 = new SelectList(new[]{new{ID="1",Name="School Code"},new {ID="2",Name="Correction Lot"},new{ID="3",Name="Correction Id"},
				new{ID="4",Name="Student Id"},new{ID="5",Name="Receipt No"},}, "ID", "Name", 1);
				ViewBag.MySch = itemsch1.ToList();

				var itemsch3 = new SelectList(new[]{new {ID="1",Name="All"},new {ID="2",Name="Particular"},new {ID="3",Name="Subject"},
			new{ID="4",Name="Photo/Sign"},}, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch3.ToList();

				ViewBag.Id = id;
				if (!string.IsNullOrEmpty(id))
				{
					ViewBag.IsSchl = "1";
					Search += "and std_id ='" + id + "' ";
					rm.StoreAllData = objDB.SchoolCorrectionStatus(schlid, Search);
					if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message2 = "Record Not Found";
						ViewBag.TotalCount2 = 0;
						ViewBag.TotalCountadded = 0;
					}
					else
					{
						ViewBag.TotalCountadded = rm.StoreAllData.Tables[0].Rows.Count;

					}
				}
				else
				{


					if (Session["SCHL"] == null && Session["UserName"] == null)
					{
						return RedirectToAction("Logout", "Login");
					}
					else
					{
						rm.StoreAllData = null;
						ViewBag.TotalCountadded = "0";

						if (Session["UserName"] != null)
						{
							ViewBag.IsSchl = "0";
							ViewBag.Message2 = "Record Not Found";
							ViewBag.TotalCount2 = 0;
							ViewBag.TotalCountadded = 0;
						}
						else
						{
							schlid = Session["SCHL"].ToString();
							ViewBag.IsSchl = "1";
							Search += "and schl ='" + schlid + "' ";

							result = new AbstractLayer.RegistrationDB().schooltypes(schlid); // passing Value to DBClass from model

							if (result == null)
							{
								return View();
							}
							if (result.Tables[1].Rows.Count > 0)
							{
								ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
								ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
								ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
								ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();

								List<SelectListItem> itemsch = new List<SelectListItem>();
								if (ViewBag.Matric == "1")
								{
									itemsch.Add(new SelectListItem { Text = "Matriculation Regular", Value = "1" });
								}
								if (ViewBag.OMatric == "1")
								{
									itemsch.Add(new SelectListItem { Text = "Matriculation Open", Value = "2" });
								}
								if (ViewBag.Senior == "1")
								{
									itemsch.Add(new SelectListItem { Text = "Sr.Secondary Regular", Value = "3" });
								}
								if (ViewBag.OSenior == "1")
								{
									itemsch.Add(new SelectListItem { Text = "Sr.Secondary Open", Value = "4" });
								}
								if (ViewBag.Matric != "1" && ViewBag.OMatric != "1" && ViewBag.Senior != "1" && ViewBag.OSenior != "1")
								{
									itemsch.Add(new SelectListItem { Text = "", Value = "" });
								}
								ViewBag.MySch = itemsch.ToList();

							}

							rm.StoreAllData = objDB.SchoolCorrectionStatus(schlid, Search);
							if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
							{
								ViewBag.Message2 = "Record Not Found";
								ViewBag.TotalCount2 = 0;
								ViewBag.TotalCountadded = 0;
							}
							else
							{
								ViewBag.TotalCountadded = rm.StoreAllData.Tables[0].Rows.Count;

							}
						}

						//


					}
				}
				return View(rm);
			}
			catch (Exception ex)
			{
				return RedirectToAction("Logout", "Login");
			}
		}

		[HttpPost]
		public ActionResult SchoolCorrectionStatus(string id, FormCollection frm)
		{
			try
			{

				AdminModels rm = new AdminModels();
				DataSet result = new DataSet();
				string schlid = string.Empty;
				string Search = "schl is not null ";
				ViewBag.Id = id;

				var itemsch1 = new SelectList(new[]{new{ID="1",Name="School Code"},new {ID="2",Name="Correction Lot"},new{ID="3",Name="Correction Id"},
				new{ID="4",Name="Student Id"},new{ID="5",Name="Receipt No"},}, "ID", "Name", 1);
				ViewBag.MySch = itemsch1.ToList();

				var itemsch3 = new SelectList(new[]{new {ID="1",Name="All"},new {ID="2",Name="Particular"},new {ID="3",Name="Subject"},
			new{ID="4",Name="Photo/Sign"},}, "ID", "Name", 1);
				ViewBag.CorrectionType = itemsch3.ToList();

				if (Session["SCHL"] == null && Session["UserName"] == null)
				{
					return RedirectToAction("Logout", "Login");
				}
				else
				{
					rm.StoreAllData = null;
					ViewBag.TotalCountadded = "";

					if (Session["UserName"] != null)
					{
						ViewBag.IsSchl = "0";
					}
					else
					{
						schlid = Session["SCHL"].ToString();
						ViewBag.IsSchl = "1";
						Search = "and schl ='" + schlid + "' ";

						result = new AbstractLayer.RegistrationDB().schooltypes(schlid); // passing Value to DBClass from model

						if (result == null)
						{
							return View();
						}
						if (result.Tables[1].Rows.Count > 0)
						{
							ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
							ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
							ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
							ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();

							List<SelectListItem> itemsch = new List<SelectListItem>();
							if (ViewBag.Matric == "1")
							{
								itemsch.Add(new SelectListItem { Text = "Matriculation Regular", Value = "1" });
							}
							if (ViewBag.OMatric == "1")
							{
								itemsch.Add(new SelectListItem { Text = "Matriculation Open", Value = "2" });
							}
							if (ViewBag.Senior == "1")
							{
								itemsch.Add(new SelectListItem { Text = "Sr.Secondary Regular", Value = "3" });
							}
							if (ViewBag.OSenior == "1")
							{
								itemsch.Add(new SelectListItem { Text = "Sr.Secondary Open", Value = "4" });
							}
							if (ViewBag.Matric != "1" && ViewBag.OMatric != "1" && ViewBag.Senior != "1" && ViewBag.OSenior != "1")
							{
								itemsch.Add(new SelectListItem { Text = "", Value = "" });
							}
							ViewBag.MySch = itemsch.ToList();

						}
					}

					//

					if (frm["CorrectionType"] != null)
					{
						ViewBag.SelectedCorrectionType = frm["CorrectionType"].ToString();
						{ Search += " and   CorPanel='" + frm["CorrectionType"].ToString() + "'"; }

					}

					if (frm["Sch1"] != "")
					{
						ViewBag.SelectedItem = frm["Sch1"];
						TempData["SelectedItem"] = frm["Sch1"];
						int SelValueSch = Convert.ToInt32(frm["Sch1"].ToString());

						if (frm["SearchString"] != "")
						{
							if (SelValueSch == 1)
							{ Search += " and SCHL='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 2)
							{ Search += " and   CorrectionLot='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and CorrectionId='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 4)
							{ Search += " and std_id='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 5)
							{ Search += " and RecieptNo like '%" + frm["SearchString"].ToString() + "%'"; }
						}
					}
					rm.StoreAllData = objDB.SchoolCorrectionStatus(schlid, Search);
					if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message2 = "Record Not Found";
						ViewBag.TotalCount2 = 0;
						ViewBag.TotalCountadded = 0;
					}
					else
					{
						ViewBag.TotalCountadded = rm.StoreAllData.Tables[0].Rows.Count;

					}

				}
				return View(rm);
			}
			catch (Exception ex)
			{
				return RedirectToAction("Logout", "Login");
			}
		}


		#endregion Firm Finaal Submit Rohit


		#region DEO Profile Data Download
		public ActionResult DownloadDeoProfile() // Download Admin DEO Data file
		{
			AdminModels AM = new AdminModels();
			FormCollection frm = new FormCollection();
			try
			{
				if (Session["UserName"] != null)
				{
					DataSet ds1 = objDB.DownloadDeoProfile(AM);
					if (ds1.Tables.Count > 0)
					{
						if (ds1.Tables[0].Rows.Count > 0)
						{
							using (XLWorkbook wb = new XLWorkbook())
							{
								wb.Worksheets.Add(ds1.Tables[0]);
								wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
								wb.Style.Font.Bold = true;
								Response.Clear();
								Response.Buffer = true;
								Response.Charset = "";
								Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
								Response.AddHeader("content-disposition", "attachment;filename=" + "DeoProfileData" + ".xls");
								//Response.AddHeader("content-disposition", "attachment;filename= DownloadChallanReport.xlsx");

								using (MemoryStream MyMemoryStream = new MemoryStream())
								{
									wb.SaveAs(MyMemoryStream);
									MyMemoryStream.WriteTo(Response.OutputStream);
									Response.Flush();
									Response.End();
								}
							}
							ViewData["Result"] = "1";
							return RedirectToAction("Welcome", "Admin");
						}
						else
						{
							return RedirectToAction("Welcome", "Admin");
						}
					}
					else
					{
						ViewBag.Message = "Download Failure";
						ViewData["Result"] = "0";
						return RedirectToAction("Welcome", "Admin");
					}
				}
				else
				{
					return RedirectToAction("Index", "Admin");
				}
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View(AM);
			}
		}
		#endregion DEO Profile Data Download

		//Begin Admin Ninth and Eleventh Result Update
		#region Admin Ninth Result Page
		public ActionResult Admin_ResultUpdate()
		{
			try
			{
				if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
				{
					return RedirectToAction("Index", "Admin");
				}
				//string SchlID = Convert.ToString(Session["SCHL"]);
				//DataSet result = objCommon.schooltypes(SchlID); // passing Value to DBClass from model
				//if (result == null)
				//{
				//    return RedirectToAction("Index", "Home");
				//}
				//if (result.Tables[1].Rows.Count > 0)
				//{
				//    //ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
				//    //ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
				//    ViewBag.NApprovedSchl = result.Tables[6].Rows[0]["NApprovedSchl"].ToString();
				//    ViewBag.EApprovedSchl = result.Tables[7].Rows[0]["EApprovedSchl"].ToString();

				//}
				ViewBag.NApprovedSchl = "1";
				ViewBag.EApprovedSchl = "1";

				TempData["CCE_SeniorSearch"] = null;

				return View();
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}
		public ActionResult Admin_Ninth_Result_Page(FormCollection frm, string id, int? page)
		{
			if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
			{
				return RedirectToAction("Index", "Admin");
			}
			AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
			SchoolModels MS = new SchoolModels();
			try
			{
				string SCHL = MS.SchlCode = Convert.ToString(TempData["SchlCode"]);
				MS.dist = Convert.ToString(TempData["dist"]);
				TempData["SchlCode"] = MS.SchlCode;
				TempData["dist"] = MS.dist;
				string CLASS = "1";

				//if (SCHL == "" || SCHL == null)
				//{ SCHL = Session["SCHL"].ToString(); }

				ViewBag.schlCode = SCHL;
				ViewBag.cid = id;

				//Begin------------Dist Allow-------------------------
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string AdminType = Session["AdminType"].ToString();
				List<SiteMenu> all = new List<SiteMenu>();
				DataSet result = objCommon.GetAdminDetailsById(Convert.ToInt32(Session["AdminId"]), Convert.ToInt32(Session["Session"].ToString().Substring(0, 4)));
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

				//End------------Dist Allow-------------------------


				var itemFilter = new SelectList(new[] {  new { ID = "5", Name = "By Section" }, new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
					new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
				ViewBag.MyFilter = itemFilter.ToList();
				ViewBag.SelectedFilter = "0";

				var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";

				var resultList = new SelectList(new[] { new { ID = "1", Name = "Pass" }, new { ID = "2", Name = "Fail" }, new { ID = "3", Name = "Re-appear" }, new { ID = "4", Name = "RL" }, new { ID = "5", Name = "Cancel" },
					new { ID = "6", Name = "Absent" }, }, "ID", "Name", 1);
				ViewBag.rslist = resultList.ToList();


				//------------------------

				if (SCHL != null && SCHL != "")
				{
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;
					string Search = string.Empty;
					Search = "  a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "'";
					// Search = " a.schl = '" + SCHL + "' ";

					//if (TempData["CCE_SeniorSearch"] != null)
					//{
					//    Search += TempData["CCE_SeniorSearch"].ToString();
					//}
					int SelAction = 0;
					if (TempData["SelAction"] != null)
					{
						SelAction = Convert.ToInt32(TempData["SelAction"]);
					}
					else
					{
						SelAction = 0;
					}
					if (TempData["CCE_SeniorSearch"] != null)
					{
						Search = TempData["CCE_SeniorSearch"].ToString();
						ViewBag.SelectedFilter = TempData["SelFilter"];
						ViewBag.SelectedAction = TempData["SelAction"];
					}


					//string class1 = "4"; // For Senior
					MS.StoreAllData = objDB.Get_Ninth_Result_Page(Search, SCHL, pageIndex, CLASS, SelAction);
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
						ViewBag.MarksPending = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["MarksPending"]);
						ViewBag.FinalPending = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["FinalPending"]);

						string reclock = MS.StoreAllData.Tables[0].Rows[0]["reclock"].ToString();
						if (reclock == "1")
						{
							ViewBag.reclock = "1";
						}
						else
						{ ViewBag.reclock = "0"; }

						ViewBag.IsFinal = "0";//Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["IsMarksFilled"]);
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
				//return RedirectToAction("Index", "Login");
			}

			return View();
		}

		[HttpPost]
		public ActionResult Admin_Ninth_Result_Page(FormCollection frm, int? page, string cmd)
		{
			string Reslist = null;
			string totMarks = null;
			string obtainMarks = null;

			if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
			{
				return RedirectToAction("Index", "Admin");
			}
			AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
			SchoolModels MS = new SchoolModels();
			try
			{
				string id = "";
				//if (frm["cid"] != "")
				//{
				//    id = frm["cid"].ToString();
				//    ViewBag.cid = frm["cid"].ToString();
				//}
				string SCHL = MS.SchlCode = frm["SchlCode"];
				TempData["SchlCode"] = SCHL;
				ViewBag.schlCode = SCHL;
				string CLASS = "1";


				var itemFilter = new SelectList(new[] {  new { ID = "5", Name = "By Section" }, new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
					new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
				ViewBag.MyFilter = itemFilter.ToList();
				ViewBag.SelectedFilter = "0";

				var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";

				var resultList = new SelectList(new[] { new { ID = "1", Name = "Pass" }, new { ID = "2", Name = "Fail" }, new { ID = "3", Name = "Re-appear" }, new { ID = "4", Name = "RL" }, new { ID = "5", Name = "Cancel" },
					new { ID = "6", Name = "Absent" }, }, "ID", "Name", 1);
				ViewBag.rslist = resultList.ToList();

				//---------------------Final Submit-----------------
				if (cmd == "Click here to Final Submit")
				{
					DataSet result = objDB.FinalSubmitNinthResult(CLASS, SCHL);
					if (result.Tables[0].Rows.Count > 0)
					{
						if (result.Tables[0].Rows[0]["res"].ToString() == "0")
						{
							TempData["result"] = "0";
						}
						else
						{
							TempData["TotImported"] = result.Tables[0].Rows[0]["res"].ToString();
							TempData["result"] = "1";
						}

					}

					return RedirectToAction("Admin_Ninth_Result_Page", "admin");
				}
				//----------------------End Final Submit------------
				//------------------------

				if (SCHL != null)
				{
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;
					string Search = string.Empty;
					//  Search = " a.SCHL='" + SCHL + "'";
					Search = "  a.SCHL = '" + SCHL + "' and  a.class='" + CLASS + "' ";

					if (frm["Dist"] != "")
					{
						MS.dist = frm["Dist"];
						TempData["Dist"] = frm["Dist"];
						Search += " and a.SCHLDIST='" + frm["Dist"] + "' ";
					}

					int SelAction = 0;
					if (frm["SelAction"] != "")
					{

						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						int SelValueSch = Convert.ToInt32(frm["SelAction"].ToString());
						if (frm["SelAction"] != "")
						{
							if (SelValueSch == 1)
							{
								SelAction = 1; /// Action for All
							}
							if (SelValueSch == 2)
							{
								SelAction = 2;
							}
							//{ Search += " and (IsMarksFilled is null or IsMarksFilled=0) "; } // pending
							if (SelValueSch == 3)
							{
								SelAction = 3;
							}
							//  { Search += " and  IsMarksFilled=1 "; } // Filled

						}
					}

					if (frm["SelFilter"] != "")
					{
						TempData["SelFilter"] = frm["SelFilter"];
						ViewBag.SelectedFilter = frm["SelFilter"];
						int SelValueSch = Convert.ToInt32(frm["SelFilter"].ToString());
						if (frm["SelFilter"] != "" && frm["SearchString"].ToString() != "")
						{
							if (SelValueSch == 1)
							{ Search += " and a.Class_Roll_Num_Section='" + frm["SearchString"].ToString().Trim() + "'"; }
							if (SelValueSch == 2)
							{ Search += " and a.Std_id='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  a.Registration_num like '%" + frm["SearchString"].ToString().Trim() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  a.Candi_Name like '%" + frm["SearchString"].ToString().Trim() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and a.section='" + frm["SearchString"].ToString().Trim() + "'"; }
						}
					}

					TempData["CCE_SeniorSearch"] = Search;
					// string class1 = "4";
					TempData["SelAction"] = SelAction;

					if (cmd == "Challan Detail")
					{
						MS.StoreAllData = objDB.Get_Ninth_Result_ChallanDetails(Search, SCHL, pageIndex, CLASS, SelAction);
						if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.Message = "Record Not Found";
							ViewBag.TotalCount = 0;
							return View(MS);
						}
						else
						{
							ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
							//int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
							ViewBag.challandetail = 1;
							return View(MS);
						}
					}

					else
					{


						MS.StoreAllData = objDB.Get_Ninth_Result_Page(Search, SCHL, pageIndex, CLASS, SelAction);
						if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.Message = "Record Not Found";
							ViewBag.TotalCount = 0;
							return View(MS);
						}
						else
						{
							ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
							int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
							ViewBag.MarksPending = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["MarksPending"]);
							ViewBag.FinalPending = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["FinalPending"]);
							//string reclock = MS.StoreAllData.Tables[1].Rows[0]["reclock"].ToString();
							string reclock = MS.StoreAllData.Tables[0].Rows[0]["reclock"].ToString();
							ViewBag.challandetail = 0;
							if (reclock == "1")
							{
								ViewBag.reclock = "1";
							}
							else
							{ ViewBag.reclock = "0"; }
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
			}
			catch (Exception ex)
			{
				//return RedirectToAction("Index", "Login");
			}

			return View();
		}
		[HttpPost]
		public JsonResult UpdateMatricResult(string ResultList, string totmarks, string obtmarks, string stdid, string schl, string UPTREMARKS)//90
		{
			AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
			try
			{

				string dee = "";
				string outstatus = "";
				string Search = string.Empty;
				string res = null;
				//string UserType = "Admin";               
				//float fee = 0;              
				//DateTime date;              
				DataSet result = objDB.UpdNinthResult(ResultList, totmarks, obtmarks, stdid, schl, adminLoginSession.AdminEmployeeUserId, UPTREMARKS);//90
				res = result.Tables[0].Rows.Count.ToString();
				if (result.Tables[0].Rows.Count.ToString() != "0")
				{
					dee = "Yes";
				}
				else
					dee = "No";


				return Json(new { sn = dee, chid = res }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return null;
			}
		}
		public ActionResult Admin_Ninth_Result_Page_Report(string id)
		{

			if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
			{
				return RedirectToAction("Index", "Admin");
			}
			AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
			SchoolModels MS = new SchoolModels();
			try
			{
				if (TempData["SchlCode"] == null)
				{
					return RedirectToAction("Admin_Ninth_Result_Page", "admin");
				}

				string SCHL = MS.SchlCode = TempData["SchlCode"].ToString();
				Session["Schl"] = SCHL;
				string CLASS = "1";

				ViewBag.schlCode = SCHL;
				DataSet Dresult = objDB.GetSchoolSection(CLASS, SCHL); // passing Value to DBClass from model            
				List<SelectListItem> SecList = new List<SelectListItem>();
				//SecList.Add(new SelectListItem { Text = "ALL", Value = "ALL" });
				foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
				{

					SecList.Add(new SelectListItem { Text = @dr["section"].ToString(), Value = @dr["section"].ToString() });
				}

				ViewBag.sec = SecList;
				Session["rpt"] = id;
				ViewBag.TotalCount = 0;
			}
			catch (Exception ex)
			{

			}
			return View(MS);

		}
		[HttpPost]
		public ActionResult Admin_Ninth_Result_Page_Report(FormCollection frm)
		{
			TempData["CCE_SeniorSearch"] = null;
			if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
			{
				return RedirectToAction("Index", "Admin");
			}
			AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
			SchoolModels MS = new SchoolModels();
			try
			{
				if (Session["Schl"] == null)
				{
					return RedirectToAction("Admin_Ninth_Result_Page", "admin");
				}
				string SCHL = MS.SchlCode = Session["Schl"].ToString();
				string CLASS = "1";

				ViewBag.schlCode = SCHL;
				string id = Session["rpt"].ToString();
				string Section = frm["Selsec"].ToString();

				DataSet Dresult = objDB.GetSchoolSection(CLASS, SCHL); // passing Value to DBClass from model            
				List<SelectListItem> SecList = new List<SelectListItem>();
				// SecList.Add(new SelectListItem { Text = "ALL", Value = "ALL" });
				foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
				{
					SecList.Add(new SelectListItem { Text = @dr["Section"].ToString(), Value = @dr["Section"].ToString() });
				}
				ViewBag.sec = SecList;
				MS.Selsec = Section;
				ViewBag.SchlSec = Section;
				string Search = string.Empty;
				Search = "a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "' and  a.Section= '" + Section + "'";
				MS.StoreAllData = objDB.Get_Ninth_Result_Page_Report(Search, SCHL, CLASS, id);
				if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(MS);
				}
				else
				{
					ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
					ViewBag.SchoolName = MS.StoreAllData.Tables[1].Rows[0]["FullSchoolNameE"].ToString();
					ViewBag.Reportid = id;
					ViewBag.Fdate = MS.StoreAllData.Tables[0].Rows[0]["UpdateDT"].ToString();

					return View(MS);
				}
			}
			catch (Exception ex)
			{

			}
			return View(MS);

		}
		#endregion Admin Ninth Result Page

		#region Admin Eleventh Result Page
		public ActionResult Admin_Eleventh_Result_Page(FormCollection frm, string id, int? page)
		{
			if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
			{
				return RedirectToAction("Index", "Admin");
			}
			AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
			SchoolModels MS = new SchoolModels();

			var itemFee = new SelectList(new[] { new { ID = "1", Name = "With Fee" }, new { ID = "2", Name = "Without Fee" }, }, "ID", "Name", 1);
			ViewBag.FeeType = itemFee.ToList();
			ViewBag.SelectedFeeType = "1";


			try
			{
				string SCHL = MS.SchlCode = Convert.ToString(TempData["SchlCode"]);
				MS.dist = Convert.ToString(TempData["dist"]);
				TempData["SchlCode"] = MS.SchlCode;
				TempData["dist"] = MS.dist;
				string CLASS = "3";

				ViewBag.schlCode = SCHL;
				ViewBag.cid = id;

				//Begin------------Dist Allow-------------------------
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string AdminType = Session["AdminType"].ToString();
				List<SiteMenu> all = new List<SiteMenu>();
				DataSet result = objCommon.GetAdminDetailsById(Convert.ToInt32(Session["AdminId"]), Convert.ToInt32(Session["Session"].ToString().Substring(0, 4)));
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

				//End------------Dist Allow-------------------------

				var itemFilter = new SelectList(new[] {  new { ID = "5", Name = "By Section" }, new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
					new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
				ViewBag.MyFilter = itemFilter.ToList();
				ViewBag.SelectedFilter = "0";

				var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";

				var resultList = new SelectList(new[] { new { ID = "1", Name = "Pass" }, new { ID = "2", Name = "Fail" }, new { ID = "3", Name = "Compartment" }, new { ID = "4", Name = "RL" }, new { ID = "5", Name = "Cancel" },
					new { ID = "6", Name = "Absent" }, }, "ID", "Name", 1);
				ViewBag.rslist = resultList.ToList();


				//------------------------

				if (SCHL != null && SCHL != "")
				{
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;
					string Search = string.Empty;
					Search = "  a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "'";
					// Search = " a.schl = '" + SCHL + "' ";

					//if (TempData["CCE_SeniorSearch"] != null)
					//{
					//    Search += TempData["CCE_SeniorSearch"].ToString();
					//}
					int SelAction = 0;
					if (TempData["SelAction"] != null)
					{
						SelAction = Convert.ToInt32(TempData["SelAction"]);
					}
					else
					{
						SelAction = 0;
					}

					if (TempData["CCE_SeniorSearch"] != null)
					{
						Search = TempData["CCE_SeniorSearch"].ToString();
						ViewBag.SelectedFilter = TempData["SelFilter"];
						ViewBag.SelectedAction = TempData["SelAction"];
					}


					//string class1 = "4"; // For Senior
					MS.StoreAllData = objDB.Get_Eleventh_Result_Page(Search, SCHL, pageIndex, CLASS, SelAction);
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
						ViewBag.MarksPending = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["MarksPending"]);
						ViewBag.FinalPending = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["FinalPending"]);

						string reclock = MS.StoreAllData.Tables[0].Rows[0]["reclock"].ToString();
						if (reclock == "1")
						{
							ViewBag.reclock = "1";
						}
						else
						{ ViewBag.reclock = "0"; }

						ViewBag.IsFinal = "0";//Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["IsMarksFilled"]);
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
				//return RedirectToAction("Index", "Login");
			}

			return View();
		}

		[HttpPost]
		public ActionResult Admin_Eleventh_Result_Page(FormCollection frm, int? page, string cmd)
		{
			string Reslist = null;
			string totMarks = null;
			string obtainMarks = null;

			if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
			{
				return RedirectToAction("Index", "Admin");
			}
			AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
			SchoolModels MS = new SchoolModels();
			try
			{
				string id = "";
				string SCHL = MS.SchlCode = frm["SchlCode"];
				TempData["SchlCode"] = SCHL;
				ViewBag.schlCode = SCHL;
				string CLASS = "3";


				var itemFilter = new SelectList(new[] {  new { ID = "5", Name = "By Section" }, new { ID = "1", Name = "By RollNo" }, new { ID = "2", Name = "By UniqueID" }, new { ID = "3", Name = "By REGNO" },
					new { ID = "4", Name = "Candidate Name" }, }, "ID", "Name", 1);
				ViewBag.MyFilter = itemFilter.ToList();
				ViewBag.SelectedFilter = "0";

				var itemAction = new SelectList(new[] { new { ID = "1", Name = "All" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Filled" }, }, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";

				var resultList = new SelectList(new[] { new { ID = "1", Name = "Pass" }, new { ID = "2", Name = "Fail" }, new { ID = "3", Name = "Compartment" }, new { ID = "4", Name = "RL" }, new { ID = "5", Name = "Cancel" },
					new { ID = "6", Name = "Absent" }, }, "ID", "Name", 1);
				ViewBag.rslist = resultList.ToList();



				//---------------------Final Submit-----------------
				if (cmd == "Click here to Final Submit")
				{
					DataSet result = objDB.FinalSubmitEleventhResult(CLASS, SCHL);
					if (result.Tables[0].Rows.Count > 0)
					{
						if (result.Tables[0].Rows[0]["res"].ToString() == "0")
						{
							TempData["result"] = "0";
						}
						else
						{
							TempData["TotImported"] = result.Tables[0].Rows[0]["res"].ToString();
							TempData["result"] = "1";
						}

					}

					return RedirectToAction("Admin_Eleventh_Result_Page", "Admin");
				}
				//----------------------End Final Submit------------
				//------------------------

				if (SCHL != null)
				{
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;
					string Search = string.Empty;
					//  Search = " a.SCHL='" + SCHL + "'";
					Search = "  a.SCHL = '" + SCHL + "' and  a.class='" + CLASS + "' ";

					if (frm["Dist"] != "")
					{
						MS.dist = frm["Dist"];
						TempData["Dist"] = frm["Dist"];
						Search += " and a.SCHLDIST='" + frm["Dist"] + "' ";
					}
					int SelAction = 0;
					if (frm["SelAction"] != "")
					{

						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						int SelValueSch = Convert.ToInt32(frm["SelAction"].ToString());
						if (frm["SelAction"] != "")
						{
							if (SelValueSch == 1)
							{
								SelAction = 1; /// Action for All
							}
							if (SelValueSch == 2)
							{
								SelAction = 2;
							}
							//{ Search += " and (IsMarksFilled is null or IsMarksFilled=0) "; } // pending
							if (SelValueSch == 3)
							{
								SelAction = 3;
							}
							//  { Search += " and  IsMarksFilled=1 "; } // Filled

						}
					}

					if (frm["SelFilter"] != "")
					{
						TempData["SelFilter"] = frm["SelFilter"];
						ViewBag.SelectedFilter = frm["SelFilter"];
						int SelValueSch = Convert.ToInt32(frm["SelFilter"].ToString());
						if (frm["SelFilter"] != "" && frm["SearchString"].ToString() != "")
						{
							if (SelValueSch == 1)
							{ Search += " and a.Class_Roll_Num_Section='" + frm["SearchString"].ToString().Trim() + "'"; }
							if (SelValueSch == 2)
							{ Search += " and a.Std_id='" + frm["SearchString"].ToString().Trim() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  a.Registration_num like '%" + frm["SearchString"].ToString().Trim() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  a.Candi_Name like '%" + frm["SearchString"].ToString().Trim() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and a.section='" + frm["SearchString"].ToString().Trim() + "'"; }
						}
					}

					TempData["CCE_SeniorSearch"] = Search;
					// string class1 = "4";
					TempData["SelAction"] = SelAction;

					if (cmd == "Challan Detail")
					{
						MS.StoreAllData = objDB.Get_Ninth_Result_ChallanDetails(Search, SCHL, pageIndex, CLASS, SelAction);
						if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
						{
							ViewBag.Message = "Record Not Found";
							ViewBag.TotalCount = 0;
							ViewBag.challandetail = 1;
							return View(MS);
						}
						else
						{
							ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
							//int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
							ViewBag.challandetail = 1;
							return View(MS);
						}
					}
					else
					{
					MS.StoreAllData = objDB.Get_Eleventh_Result_Page(Search, SCHL, pageIndex, CLASS, SelAction);
					if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
						ViewBag.challandetail = 0;
							return View(MS);
					}
					else
					{
						ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
						int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
						ViewBag.MarksPending = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["MarksPending"]);
						ViewBag.FinalPending = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["FinalPending"]);
						//string reclock = MS.StoreAllData.Tables[1].Rows[0]["reclock"].ToString();
						string reclock = MS.StoreAllData.Tables[0].Rows[0]["reclock"].ToString();
						if (reclock == "1")
						{
							ViewBag.reclock = "1";
						}
						else
						{ ViewBag.reclock = "0"; }
							ViewBag.challandetail = 0;
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
			}
			catch (Exception ex)
			{
				//return RedirectToAction("Index", "Login");
			}

			return View();
		}
		[HttpPost]
		public JsonResult UpdateEleventhResult(string ResultList, string totmarks, string obtmarks, string stdid, string schl, string UPTREMARKS)
		{
			AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
			try
			{

				string dee = "";
				string outstatus = "";
				string Search = string.Empty;
				string res = null;
				//string UserType = "Admin";               
				//float fee = 0;              
				//DateTime date;              
				DataSet result = objDB.UpdateEleventhResult(ResultList, totmarks, obtmarks, stdid, schl, adminLoginSession.AdminEmployeeUserId, UPTREMARKS);
				res = result.Tables[0].Rows.Count.ToString();
				if (result.Tables[0].Rows.Count.ToString() != "0")
				{
					dee = "Yes";
				}
				else
					dee = "No";


				return Json(new { sn = dee, chid = res }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return null;
			}
		}
		public ActionResult Admin_Eleventh_Result_Page_Report(string id)
		{

			if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
			{
				return RedirectToAction("Index", "Admin");
			}
			AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
			SchoolModels MS = new SchoolModels();
			try
			{
				if (TempData["SchlCode"] == null)
				{
					return RedirectToAction("Admin_Eleventh_Result_Page", "admin");
				}

				string SCHL = MS.SchlCode = TempData["SchlCode"].ToString();
				Session["Schl"] = SCHL;
				string CLASS = "3";

				ViewBag.schlCode = SCHL;
				DataSet Dresult = objDB.GetSchoolSection(CLASS, SCHL); // passing Value to DBClass from model            
				List<SelectListItem> SecList = new List<SelectListItem>();
				//SecList.Add(new SelectListItem { Text = "ALL", Value = "ALL" });
				foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
				{

					SecList.Add(new SelectListItem { Text = @dr["section"].ToString(), Value = @dr["section"].ToString() });
				}

				ViewBag.sec = SecList;
				Session["rpt"] = id;
				ViewBag.TotalCount = 0;
			}
			catch (Exception ex)
			{

			}
			return View(MS);

		}
		[HttpPost]
		public ActionResult Admin_Eleventh_Result_Page_Report(FormCollection frm)
		{
			TempData["CCE_SeniorSearch"] = null;
			if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
			{
				return RedirectToAction("Index", "Admin");
			}
			AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
			SchoolModels MS = new SchoolModels();
			try
			{
				if (Session["Schl"] == null)
				{
					return RedirectToAction("Admin_Eleventh_Result_Page", "admin");
				}
				string SCHL = MS.SchlCode = Session["Schl"].ToString();
				string CLASS = "3";

				ViewBag.schlCode = SCHL;
				string id = Session["rpt"].ToString();
				string Section = frm["Selsec"].ToString();

				DataSet Dresult = objDB.GetSchoolSection(CLASS, SCHL); // passing Value to DBClass from model            
				List<SelectListItem> SecList = new List<SelectListItem>();
				// SecList.Add(new SelectListItem { Text = "ALL", Value = "ALL" });
				foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
				{
					SecList.Add(new SelectListItem { Text = @dr["Section"].ToString(), Value = @dr["Section"].ToString() });
				}
				ViewBag.sec = SecList;
				MS.Selsec = Section;
				ViewBag.SchlSec = Section;
				string Search = string.Empty;
				Search = "a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "' and  a.Section= '" + Section + "'";
				MS.StoreAllData = objDB.Get_Eleventh_Result_Page_Report(Search, SCHL, CLASS, id);
				if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(MS);
				}
				else
				{
					ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
					ViewBag.SchoolName = MS.StoreAllData.Tables[1].Rows[0]["FullSchoolNameE"].ToString();
					ViewBag.Reportid = id;
					ViewBag.Fdate = MS.StoreAllData.Tables[0].Rows[0]["UpdateDT"].ToString();

					return View(MS);
				}
			}
			catch (Exception ex)
			{

			}
			return View(MS);

		}
		public ActionResult Admin_Eleventh_Result_Page_Report_section(string id)
		{

			if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
			{
				return RedirectToAction("Index", "Admin");
			}
			AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
			SchoolModels MS = new SchoolModels();
			try
			{
				if (Session["Schl"] == null)
				{
					return RedirectToAction("Admin_Eleventh_Result_Page", "admin");
				}
				string SCHL = MS.SchlCode = Session["Schl"].ToString();
				string CLASS = "1";

				ViewBag.schlCode = SCHL;
				DataSet Dresult = objDB.GetSchoolSection(CLASS, SCHL); // passing Value to DBClass from model            
				List<SelectListItem> SecList = new List<SelectListItem>();
				//SecList.Add(new SelectListItem { Text = "ALL", Value = "ALL" });
				foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
				{

					SecList.Add(new SelectListItem { Text = @dr["section"].ToString(), Value = @dr["section"].ToString() });
				}

				ViewBag.sec = SecList;
				Session["rpt"] = id;
				//ViewBag.TotalCount = 0;
				string Search = string.Empty;

				Search = "a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "' ";
				MS.StoreAllData = objDB.Eleventh_Result_Page_Report_Section(Search, SCHL, CLASS, id);
				if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(MS);
				}
				else
				{
					ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
					ViewBag.SchoolName = MS.StoreAllData.Tables[1].Rows[0]["FullSchoolNameE"].ToString();
					ViewBag.Reportid = id;
					ViewBag.Fdate = MS.StoreAllData.Tables[0].Rows[0]["UpdateDT"].ToString();

					return View(MS);
				}
			}
			catch (Exception ex)
			{

			}
			return View(MS);

		}
		#endregion Admin Eleventh Result Page



		public JsonResult jq9th11thFinalSubmit(string cls, string feemode, string receiveno, string receivedate, string remarks, string receivefee, string hdselectedItems, string schl)
		{
			try
			{
				ChallanMasterModel CM = new ChallanMasterModel();
				string today = DateTime.Today.ToString("dd/MM/yyyy");
				AbstractLayer.SchoolDB objSchoolDB = new AbstractLayer.SchoolDB();
				string dee = "";
				string OutStatus = "0";
				string Search = string.Empty;
				string chid = "";
				if (string.IsNullOrEmpty(receivedate))
				{
					receivedate = DateTime.Now.ToString("dd/MM/yyyy");
				}
				DateTime dateselected;
				if (DateTime.TryParseExact(today, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dateselected))
				{
					CM.ChallanVDateN = dateselected;
					CM.CHLNVDATE = dateselected.ToString("dd/MM/yyyy");
					CM.SCHLREGID = schl.ToString();
					CM.SchoolCode = schl.ToString();
					CM.FeeStudentList = hdselectedItems.ToString();
					CM.FEECODE = "000";
					CM.TOTFEE = receivefee==""?0:(float)Convert.ToDouble(receivefee);
					CM.BCODE = "203";
					CM.BANK = "PSEB HOD";
					CM.LumsumFine = Convert.ToInt32(0);
					CM.LSFRemarks = receiveno.ToString();
					CM.FEEMODE = feemode.ToString();
					CM.FEECAT = "9th_11th_Result";
					string SchoolMobile = "";
					chid = objSchoolDB.InsertPaymentForm_For_9th11thResultUpdate(CM, out SchoolMobile);

					if (chid != "0")
					{
						string reslt = objSchoolDB.Admin9th11thFinalSubmit(feemode, Convert.ToInt32(Session["AdminId"].ToString()), cls, schl, receiveno, receivefee, remarks, hdselectedItems, out OutStatus, dateselected);
						if (OutStatus == "0" || OutStatus == "-1")
						{
							dee = "No";
						}
						else if (OutStatus == "-2")
						{
							dee = "DP";
						}
						else
						{
							dee = "Yes";
						}



					}

				}

				return Json(new { sn = dee, Rcno = OutStatus, chid = chid.ToString() }, JsonRequestBehavior.AllowGet);

			}
			catch (Exception ex)
			{
				return Json(new { sn = "0", Rcno = "", chid = "" }, JsonRequestBehavior.AllowGet);
			}
		}

		//End Admin Ninth and Eleventh Result Update


		#region Affiliation Certificate
		public ActionResult AffiliationCertificate(string id)
		{
			if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
			{
				return RedirectToAction("Index", "Admin");
			}
			AdminModels MS = new AdminModels();
			try
			{
				MS.SchlCode = id;
				//string Search = string.Empty;
				//Search = "a.schl = '" + SCHL + "' and  a.class= '" + CLASS + "' ";
				MS.StoreAllData = objDB.AffiliationCertificate(MS.SchlCode);
				if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View(MS);
				}
				else
				{
					ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
					MS.CertNo = MS.StoreAllData.Tables[0].Rows[0]["CertNo"].ToString();
					MS.CertDate = MS.StoreAllData.Tables[0].Rows[0]["CertDate"].ToString();
					// MS.Remarks = MS.StoreAllData.Tables[0].Rows[0]["Remarks"].ToString();
					return View(MS);
				}

			}
			catch (Exception ex)
			{

			}
			return View(MS);
		}
		[HttpPost]
		public ActionResult AffiliationCertificate(FormCollection frm)
		{
			if (Session["UserName"] == null || Session["RoleType"].ToString().ToUpper() != "ADMIN")
			{
				return RedirectToAction("Index", "Admin");
			}
			//  AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels MS = new AdminModels();
			try
			{
				MS.SchlCode = frm["SchlCode"].ToString();
				MS.CertNo = frm["CertNo"].ToString();
				MS.CertDate = frm["CertDate"].ToString();
				MS.Remarks = frm["Remarks"].ToString();
				if (MS.CertNo != "" && MS.CertDate != "" && MS.Remarks != "")
				{
					MS.StoreAllData = objDB.Upd_AffiliationCertificate(MS.SchlCode, MS.CertNo, MS.CertDate, MS.Remarks);
					MS.CertDate = "";
					MS.Remarks = "";
				}

				MS.StoreAllData = objDB.AffiliationCertificate(MS.SchlCode);

				if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Updated";
					ViewBag.TotalCount = 0;
					return View(MS);
				}
				else
				{
					ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
					ViewBag.TotalCount2 = MS.StoreAllData.Tables[0].Rows.Count;
					return View(MS);
				}

			}
			catch (Exception ex)
			{

			}
			return View(MS);
		}
		#endregion Affiliation Certificate


		#region Begin School Premises


		public ActionResult ViewSchoolPremisesInformation(SchoolPremisesInformation spi, int? page)
		{
			try
			{
				// string DistAllow = "";
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				ViewBag.MySch = objCommon.SearchSchoolItems().Where(s => Convert.ToInt32(s.Value) < 3);

				var itemAction = new SelectList(new[] { new { ID = "1", Name = "Pending" }, new { ID = "2", Name = "Submitted" }, }, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";

				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = "";
				if (TempData["SearchSchoolPemises"] != null)
				{
					Search += TempData["SearchSchoolPemises"].ToString();
					ViewBag.SelectedAction = TempData["SelAction"];
					ViewBag.SelectedItem = TempData["SelectedItem"];
					int outstatus = 0;
					spi.StoreAllData = new AbstractLayer.SchoolDB().ViewSchoolPremisesInformation(1, Search, "", pageIndex, out outstatus, AdminId);

					if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.LastPageIndex = 0;
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						TempData["SearchSchoolPemises"] = Search;
						TempData.Keep();
						ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
						int count = Convert.ToInt32(spi.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
						ViewBag.TotalCount1 = count;
						int tp = Convert.ToInt32(count);
						int pn = tp / 20;
						int cal = 20 * pn;
						int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
						if (res >= 1)
							ViewBag.pn = pn + 1;
						else
							ViewBag.pn = pn;
						return View(spi);
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
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}

		[HttpPost]
		public ActionResult ViewSchoolPremisesInformation(SchoolPremisesInformation spi, FormCollection frm, int? page)
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				// SchoolModels asm = new SchoolModels();
				if (ModelState.IsValid)
				{
					ViewBag.MySch = objCommon.SearchSchoolItems().Where(s => Convert.ToInt32(s.Value) < 3);

					var itemAction = new SelectList(new[] { new { ID = "1", Name = "Pending" }, new { ID = "2", Name = "Submitted" }, }, "ID", "Name", 1);
					ViewBag.MyAction = itemAction.ToList();
					ViewBag.SelectedAction = "0";

					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;
					string Search = string.Empty;
					Search = "SCHL like '%' ";

					int SelAction = 0;
					if (frm["SelAction"] != "")
					{
						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						SelAction = Convert.ToInt32(frm["SelAction"].ToString());
						if (SelAction == 1)
						{
							Search += " and FinalStatus  not like '%final%' ";
						}
						else if (SelAction == 2)
						{
							Search += " and FinalStatus  like '%final%' ";
						}
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
						}
					}
					TempData["SearchSchoolPemises"] = Search;
					TempData.Keep(); // to store search value for view
									 ////if (DistAllow != "")
									 ////{
									 ////    Search += " and DIST in (" + DistAllow + ")";
									 ////}
					int Outstatus = 0;
					spi.StoreAllData = new AbstractLayer.SchoolDB().ViewSchoolPremisesInformation(1, Search, "", pageIndex, out Outstatus, AdminId);

					if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.LastPageIndex = 0;
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
						int count = Convert.ToInt32(spi.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
						ViewBag.TotalCount1 = count;
						int tp = Convert.ToInt32(count);
						int pn = tp / 20;
						int cal = 20 * pn;
						int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
						if (res >= 1)
							ViewBag.pn = pn + 1;
						else
							ViewBag.pn = pn;
						return View(spi);
					}
				}
				return View(spi);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return View();
			}
		}


		// Cancel Challan
		public JsonResult UnlockFinalSchoolPremisesInformation(string cancelremarks, string schl)
		{
			try
			{
				string dee = "0";
				int outstatus = 0;
				if (schl != "")
				{
					int AdminId = Convert.ToInt32(Session["AdminId"]);
					DataSet ds = new AbstractLayer.SchoolDB().ViewSchoolPremisesInformation(2, cancelremarks, schl, 1, out outstatus, AdminId);
					dee = outstatus.ToString();
				}
				else
				{ dee = "2"; }
				return Json(new { sn = dee }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { sn = "-1" }, JsonRequestBehavior.AllowGet);
			}
		}



		public ActionResult DownloadFinalSubmitPendingSchoolList(string id)
		{
			try
			{
				//if (Request.QueryString["File"] == null)
				if (id == null)
				{
					return RedirectToAction("SchoolPremisesInformationReport", "Report");
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
							ds = new AbstractLayer.SchoolDB().ViewSchoolPremisesInformation(3, Search, "", 1, out OutStatus, AdminId);
						}
						else if (id.ToLower() == "pending")
						{
							//Search += " and isnull(IsFinalSubmit,0)=0 ";
							Search += " and   FinalStatus  not like '%final%' ";
							ds = new AbstractLayer.SchoolDB().ViewSchoolPremisesInformation(3, Search, "", 1, out OutStatus, AdminId);
						}
						else
						{
							//Search = " USERTYPE='" + id + "' and isnull(IsFinalSubmit,0)=0 ";
							Search = " USERTYPE='" + id + "'  and  FinalStatus  not like '%final%'";
							ds = new AbstractLayer.SchoolDB().ViewSchoolPremisesInformation(4, Search, "", 1, out OutStatus, AdminId);
						}
						if (ds == null)
						{
							return RedirectToAction("SchoolPremisesInformationReport", "Report");
						}
						else
						{
							if (ds.Tables[0].Rows.Count > 0)
							{
								bool ResultDownload;
								try
								{
									fileName1 = id.ToString().ToUpper() + "_DownloadFinalSubmitPendingSchoolList";
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
						return RedirectToAction("SchoolPremisesInformationReport", "Report");
					}
				}
				return RedirectToAction("SchoolPremisesInformationReport", "Report");
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return RedirectToAction("SchoolPremisesInformationReport", "Report");
			}
		}

		#endregion

		#region BookSalesMaster adminContrler
		public ActionResult BookSalesMaster()
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}

				AdminModels AM = new AdminModels();
				string CLS = "";
				//Session["AdminId"]= "17";
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				//Convert.ToInt32(Session["AdminId"]);
				AM.StoreAllData = objCommon.GetBookSalesMaster(CLS, AdminId);
				if (AM.StoreAllData == null || AM.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					DataSet Dresult = objCommon.GetBookSalesClass();
					List<SelectListItem> SecList = new List<SelectListItem>();
					//SecList.Add(new SelectListItem { Text = "ALL", Value = "ALL" });
					foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
					{

						SecList.Add(new SelectListItem { Text = @dr["class"].ToString(), Value = @dr["classid"].ToString() });
					}
					ViewBag.MyFormName = SecList;

					ViewBag.TotalCount = AM.StoreAllData.Tables[0].Rows.Count;
					return View(AM);
				}
			}
			catch (Exception ex)
			{
				return View();
			}
		}
		[HttpPost]
		public ActionResult BookSalesMaster(string updId, FormCollection fc)
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				AdminModels AM = new AdminModels();

				DataSet Dresult = objCommon.GetBookSalesClass();
				List<SelectListItem> SecList = new List<SelectListItem>();
				//SecList.Add(new SelectListItem { Text = "ALL", Value = "ALL" });
				foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
				{

					SecList.Add(new SelectListItem { Text = @dr["class"].ToString(), Value = @dr["classid"].ToString() });
				}
				ViewBag.MyFormName = SecList;

				AM.FormName = fc["FormName"];
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string CLS = AM.FormName;
				AM.StoreAllData = objCommon.GetBookSalesMaster(CLS, AdminId);
				if (AM.StoreAllData == null || AM.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = AM.StoreAllData.Tables[0].Rows.Count;
					return View(AM);
				}
			}
			catch (Exception ex)
			{
				return View();
			}
		}

		public JsonResult jqUpdateBookSalesMaster(string CLASS, string BOOKID, string SOS, string SALES, string SBALANCE, string AMOUNT, string DOS, string DOB, string DBALANCE)
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return null;
				}

				string dee = "";
				string outstatus = "";
				string Search = string.Empty;
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				//string res = objDB.UpdaadharEnrollmentNo(std_id, aadhar_num, SCHL);
				DataSet res1 = objCommon.UpdateBookSalesMaster(AdminId, CLASS, BOOKID, SOS, SALES, SBALANCE, AMOUNT, DOS, DOB, DBALANCE);
				string res = res1.Tables[0].Rows[0]["res"].ToString();
				if (res != "0")
				{
					//dee = res;
					dee = "Yes";
				}
				else
					dee = "No";

				return Json(new { sn = dee, chid = res }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return null;
			}
		}

		#endregion BookSalesMaster adminContrler


		public ActionResult BulkEmail()
		{
			if (Session["AdminId"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}
			ViewBag.Message = "";
			return View();
		}


		[HttpPost]
		public ActionResult BulkEmail(HttpPostedFileBase fileUploader)
		{
			try
			{
				ViewBag.Message = "";
				DataSet ds = objCommon.BulkMail();
				if (ds != null && ds.Tables[0].Rows.Count > 0)
				{
					//for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
					for (int i = 0; i < 1; i++)
					{
						string Appno = ds.Tables[0].Rows[i]["Appno"].ToString();
						// string EmailID = ds.Tables[0].Rows[i]["EMailId"].ToString();
						string EmailID = "rohit.nanda@ethical.in";
						string TotalFeePending = ds.Tables[0].Rows[i]["TotalFeePending"].ToString();
						string AddSubFees = ds.Tables[0].Rows[i]["AddSubFees"].ToString();
						string AddSubFeeCollected = ds.Tables[0].Rows[i]["AddSubFeeCollected"].ToString();
						string ExamAddSubFees = ds.Tables[0].Rows[i]["ExamAddSubFees"].ToString();

						if (EmailID != "" && fileUploader != null)
						{
							// string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Dear Candidate</b>,</td></tr><tr><td><b>Difference in Admission/Exam Fees of Open School of Application : </b> " + Appno + "<br /><b>Admission Additonal Fee : Rs</b>" + AddSubFees + "<br /><b>Exam Additonal Fee : Rs</b>" + ExamAddSubFees + "<br /><b>Total Additonal Submited Fee : Rs</b>" + AddSubFeeCollected + "<br /><b>Total Pending Fee : Rs</b>" + TotalFeePending + "<br /><br />NOTE: Last Date of Submission is <b>31st,December,2018</b>. Ignore if Already Paid.<br /><br /><b>Please find the attachment file for more details.</b><br /></td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:helpdeskpsebonline@gmail.com>helpdeskpsebonline@gmail.com</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /></td></tr>";
							string body = MailBodyFormat(ds.Tables[0]);
							//string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Dear Candidate</b>,</td></tr><tr><td><b>Difference in Admission/Exam Fees of Open School of Application : </b> " + Appno + "<br /><b>Admission Additonal Fee : Rs</b>" + AddSubFees + "<br /><b>Exam Additonal Fee : Rs</b>" + ExamAddSubFees + "<br /><b>Total Additonal Submited Fee : Rs</b>" + AddSubFeeCollected + "<br /><b>Total Pending Fee : Rs</b>" + TotalFeePending + "<br /><br /><b>Please find the attachment file for more details.</b><br /></td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:psebnoreply@gmail.com>psebnoreply@gmail.com</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /></td></tr>";
							// bool result = new AbstractLayer.DBClass().mail("PSEB - Admin User Details", body, "rohit.nanda@ethical.in");
							bool result = new AbstractLayer.DBClass().mailwithattachment("PSEB Reminder - Difference in Admission/Exam Fees of Open School of Application : " + Appno, body, EmailID, fileUploader);
							if (result)
							{
								ViewBag.Message = "1";
							}
						}
					}
				}
			}
			catch (Exception)
			{

			}
			return View();
		}


		public string MailBodyFormat(DataTable dsCandidateDtl)
		{
			StringBuilder _sbAppend = new StringBuilder();
			_sbAppend.Append("<table id='tblApplicationForm' runat='server' border='0' class='bgimg' style='width: 98%;'>");
			_sbAppend.Append("<table width='100%' border='0'>");
			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td width='10%' style='padding-top: 5px'>");
			_sbAppend.Append("<img src='https://registration2021.pseb.ac.in/Images/Printlogo.jpg' alt='PSEB' /></td>");
			_sbAppend.Append("<td width='90%' style='font-size: 16px; text-align: center; line-height: 25px; padding-top: 5px'>");
			_sbAppend.Append("<span style='font-size: 36px;'>Punjab School Education Board</span><br />");
			_sbAppend.Append("Recruitment of Clerk - Cum- Data Entry Operators</td>");
			_sbAppend.Append("</tr>");
			_sbAppend.Append("</table>");

			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td colspan='2'>");
			_sbAppend.Append("<fieldset style='border: 1px solid'>");
			_sbAppend.Append("<legend>Basic Information</legend>");
			_sbAppend.Append("<table style='width: 100%;' border='1' cellpadding='0' cellspacing='0'>");

			_sbAppend.Append("<tr>");

			_sbAppend.Append("<td>Application Number :   </td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append("<b>");
			//  _sbAppend.Append("<asp:Label ID='lblApplicationNo' runat='server' class='RightTextPrint'>" + dsCandidateDtl.Rows[0]["applicationNumber"].ToString() + "</asp:Label></b>");
			_sbAppend.Append("<asp:Label ID='lblApplicationNo' runat='server' class='RightTextPrint'>applicationNumber</asp:Label></b>");
			_sbAppend.Append(" </td>");
			_sbAppend.Append("<td rowspan='5' style='text-align: center'>");
			//_sbAppend.Append("<asp:Image ID='imgPhoto' Visible='false' runat='server' Height='130px' Width='130' />");
			_sbAppend.Append("</td>");
			_sbAppend.Append("</tr>");

			_sbAppend.Append(" <tr>");
			_sbAppend.Append("<td>User Id (email id) :</td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			//_sbAppend.Append("<asp:Label ID='lblEmailId' runat='server' class='RightTextPrint'>" + dsCandidateDtl.Rows[0]["emailID"].ToString() + "</asp:Label>");
			_sbAppend.Append("<asp:Label ID='lblEmailId' runat='server' class='RightTextPrint'>emailID</asp:Label>");
			_sbAppend.Append("</td>");

			_sbAppend.Append("</tr>");

			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td>Mobile No .:   </td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			//  _sbAppend.Append("<asp:Label ID='lblMobileNo' runat='server' class='RightTextPrint'>" + dsCandidateDtl.Rows[0]["mobileNumber"].ToString() + "</asp:Label>");
			_sbAppend.Append("<asp:Label ID='lblMobileNo' runat='server' class='RightTextPrint'>mobileNumber</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append(" </tr>");
			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td>Aadhar Number :   </td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			//_sbAppend.Append("<asp:Label ID='lblAadharNumber' runat='server' class='RightTextPrint'>" + dsCandidateDtl.Rows[0]["aadharNumber"].ToString() + "</asp:Label>");
			_sbAppend.Append("<asp:Label ID='lblAadharNumber' runat='server' class='RightTextPrint'>aadharNumber</asp:Label>");
			_sbAppend.Append("</td>");

			_sbAppend.Append("</tr>");
			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td>Punjab Domicile  :</td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			// _sbAppend.Append("<asp:Label ID='lblPunjabiDomicile' runat='server' class='RightTextPrint'>" + (dsCandidateDtl.Rows[0]["punjabDomicile"].ToString().Trim() == "1" ? "Yes" : "No") + "</asp:Label>");
			_sbAppend.Append("<asp:Label ID='lblPunjabiDomicile' runat='server' class='RightTextPrint'>Yes</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("</tr>");



			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td>Name :</td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append("<b>");
			_sbAppend.Append("<asp:Label ID='lblCandidateName' runat='server' class='RightTextPrint'>Dynamic Name</asp:Label></b>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("<td>Father&#39;s Name :   </td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append("<asp:Label ID='lblFatherName' runat='server' class='RightTextPrint'>Dynamic fatherName</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("</tr>");


			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td>Mother&#39;s Name :</td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append("<asp:Label ID='lblMotherName' runat='server' class='RightTextPrint'>Dynamic motherName</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("<td>Gender  :</td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append("<asp:Label ID='lblGender' runat='server' class='RightTextPrint'>Dynamic gender</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("</tr>");


			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td>Category :   </td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append("<asp:Label ID='lblCategory' runat='server' class='RightTextPrint'>Dynamic categotyName</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("<td id='trSelf' runat='server'>Ex.Serviceman Type :</td>");
			_sbAppend.Append("<td id='trSelf1' runat='server' class='LeftTextPrint'>");

			string strlblExServicemanType = "Yes";
			//if (dsCandidateDtl.Rows[0]["categotyName"].ToString() == "P0141"
			//         ||
			//           dsCandidateDtl.Rows[0]["categotyName"].ToString() == "P0242"
			//            ||
			//           dsCandidateDtl.Rows[0]["categotyName"].ToString() == "P0343"
			//              ||
			//           dsCandidateDtl.Rows[0]["categotyName"].ToString() == "P0444")
			//{
			//    if (Convert.ToInt32(dsCandidateDtl.Rows[0]["isSelf"].ToString()) == 1)
			//    {
			//        strlblExServicemanType = "Yes";
			//    }
			//    else
			//    {
			//        strlblExServicemanType = "No";
			//    }
			//}


			_sbAppend.Append("<asp:Label ID='lblExServicemanType' runat='server' class='RightTextPrint'>Dynamic strlblExServicemanType</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("</tr>");

			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td>Marital Status :   </td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append("<asp:Label ID='lblMaritalStatus' runat='server' class='RightTextPrint'Dynamic >maritalStatus</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("<td>Date of Birth :   </td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			//_sbAppend.Append("<asp:Label ID='lblDOB' runat='server' class='RightTextPrint'>" + Convert.ToDateTime(dsCandidateDtl.Rows[0]["dob"].ToString()).ToString("dd-MMM-yyyy") + "</asp:Label>");
			_sbAppend.Append("<asp:Label ID='lblDOB' runat='server' class='RightTextPrint'>Dynamic DOB</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("</tr>");
			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td>Punjabi Passed in Matriculation Level :   </td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append("<asp:Label ID='lblPunjabiPassed10th' runat='server' class='RightTextPrint'>Dynamic isMatriculationLevel</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("<td>Are You Employee of State / Center Govt. :</td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append("<asp:Label ID='lblPunjabiPassed12th' runat='server' class='RightTextPrint'>Dynamic isGovtEmployee</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("</tr>");




			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td>Bachelor's Degree :   </td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append("<asp:Label ID='lblBachelorsDegree' runat='server' class='RightTextPrint'>Dynamic isBachelorDegree</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("<td>Bachelor's Degree Name :</td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append("<asp:Label ID='lblBachelorsDegreName' runat='server' class='RightTextPrint'>Dynamic degreeName</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("</tr>");

			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td>Percentage in Bachelor's Degree :   </td>");
			_sbAppend.Append(" <td class='LeftTextPrint'>");
			_sbAppend.Append("<asp:Label ID='lblPercentageinBachelorsDegree' runat='server' class='RightTextPrint'>Dynamic percantageDegreeName</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("<td>University Name :</td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append(" <asp:Label ID='lblUniversityName' runat='server' class='RightTextPrint'>Dynamic universityName</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("</tr>");


			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td colspan='3'>Computer Eligibility (120 Hrs(DTP Course ) ISO 9001 Certified, O Level Certificate from DOEACC) :</td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append("<asp:Label ID='lbComputerEligibility' runat='server' class='RightTextPrint'>Dynamic isComputerLegibelity </asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("</tr>");
			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td>Passing Year / Session :   </td>");
			_sbAppend.Append("<td class='LeftTextPrint' colspan='3'>");
			_sbAppend.Append("<asp:Label ID='lblPassingYearwithSession' runat='server' class='RightTextPrint'>Dynamic passingYearWithSession</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("</tr>");

			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td>Computer Course Name : </td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append("<asp:Label ID='lblNameofTheCourse' runat='server' class='RightTextPrint'>Dynamic nameOfCourse</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("<td>Computer Course Passing Year :</td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append("<asp:Label ID='lblPassingYear' runat='server' class='RightTextPrint'>Dynamic passingYear</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("</tr>");

			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td>Computer Course Institute/Board /University :   </td>");
			_sbAppend.Append("<td class='LeftTextPrint' colspan='3'>");
			_sbAppend.Append("<asp:Label ID='lblInstituteBoardUniversity' runat='server' class='RightTextPrint'>Dynamic instituteBoardUniversity </asp:Label>");
			_sbAppend.Append("</td>");

			_sbAppend.Append("</tr>");
			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td>Candidate Address :</td>");
			_sbAppend.Append("<td class='LeftTextPrint' colspan='3'>");
			_sbAppend.Append("<asp:Label ID='lblAddress' runat='server' class='RightTextPrint'>Dynamic address</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("</tr>");

			//_sbAppend.Append("<tr>");
			//_sbAppend.Append("<td>Near By Place :</td>");
			//_sbAppend.Append("<td class='LeftTextPrint'>");
			//_sbAppend.Append("<asp:Label ID='lblNearByPlace' runat='server' class='RightTextPrint'>Dynamic" + dsCandidateDtl.Rows[0]["nearByPlace"].ToString() + "</asp:Label>");
			//_sbAppend.Append("</td>");
			//_sbAppend.Append("<td>District :</td>");
			//_sbAppend.Append("<td class='LeftTextPrint'>");
			//_sbAppend.Append("<asp:Label ID='lblDistrict' runat='server' class='RightTextPrint'>" + dsCandidateDtl.Rows[0]["DistrictName"].ToString() + "</asp:Label>");
			//_sbAppend.Append(" </td>");

			//_sbAppend.Append(" </tr>");

			//_sbAppend.Append(" <tr>");
			//_sbAppend.Append("<td>Tehsil :</td>");
			//_sbAppend.Append("<td class='LeftTextPrint'>");
			//_sbAppend.Append("<asp:Label ID='lblTehsil' runat='server' class='RightTextPrint'>" + dsCandidateDtl.Rows[0]["TehsilName"].ToString() + "</asp:Label>");
			//_sbAppend.Append("</td>");
			//_sbAppend.Append("<td>Pin Code :</td>");
			//_sbAppend.Append("<td class='LeftTextPrint'>");
			//_sbAppend.Append("<asp:Label ID='lblPinCode' runat='server' class='RightTextPrint'>" + dsCandidateDtl.Rows[0]["pinCode"].ToString() + "</asp:Label>");
			//_sbAppend.Append("</td>");

			//_sbAppend.Append("</tr>");

			_sbAppend.Append("</table>");
			_sbAppend.Append("</fieldset>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("</tr>");
			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td colspan='2'>");
			_sbAppend.Append("<fieldset style='border: 1px solid'>");
			_sbAppend.Append("<legend>Fee Details</legend>");
			_sbAppend.Append("<table style='width: 100%'>");
			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td>Fee Status :   </td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");

			string lblFeeStatus = "Paid";
			//if (dsCandidateDtl.Rows[0]["PaymentStatus"].ToString() == "1")
			//    lblFeeStatus = "Paid";
			//else
			//{
			//    if (dsCandidateDtl.Rows[0]["isSelf"].ToString() != "1")
			//        lblFeeStatus = "Pending";
			//}

			_sbAppend.Append("<asp:Label ID='lblFeeStatus' runat='server' class='RightTextPrint'>" + lblFeeStatus + "</asp:Label>");
			_sbAppend.Append("</td>");

			_sbAppend.Append("<td>Fee Deposited :   </td>");
			_sbAppend.Append(" <td class='LeftTextPrint'>");
			_sbAppend.Append("<asp:Label ID='lblFeeDeposited' runat='server' class='RightTextPrint'>" + dsCandidateDtl.Rows[0]["transactionAmount"].ToString() + "</asp:Label>");
			_sbAppend.Append("</td>");

			_sbAppend.Append(" </tr>");
			_sbAppend.Append("<tr>");
			_sbAppend.Append("<td>Deposit  Date :</td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append(" <asp:Label ID='lblDate' runat='server' class='RightTextPrint'>" + dsCandidateDtl.Rows[0]["trsanctionDate"].ToString() + "</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("<td>Bank Reference Number :   </td>");
			_sbAppend.Append("<td class='LeftTextPrint'>");
			_sbAppend.Append("<asp:Label ID='lblBankReferenceNumber' runat='server' class='RightTextPrint'>" + dsCandidateDtl.Rows[0]["transactionNumber"].ToString() + "</asp:Label>");
			_sbAppend.Append("</td>");
			_sbAppend.Append("</tr>");
			_sbAppend.Append("</table>");
			_sbAppend.Append("</fieldset>");
			_sbAppend.Append(" </td>");
			_sbAppend.Append("</tr>");

			//_sbAppend.Append("<tr>");
			//_sbAppend.Append("<td colspan='4' style='text-align: right;'>");
			//_sbAppend.Append("<asp:Image ID='imgSignature' Visible='false' runat='server' Height='60' Width='200' />");
			//_sbAppend.Append("<br />");
			//_sbAppend.Append("<br />");

			//_sbAppend.Append("Singnature of candidate &nbsp;&nbsp;");
			//_sbAppend.Append("</tr>");
			//_sbAppend.Append(" <tr>");
			//_sbAppend.Append("<td style='text-align: left;' colspan='4'>(Signature at the time of Counseling )");
			//_sbAppend.Append("</td>");
			//_sbAppend.Append("</tr>");

			//_sbAppend.Append("<tr>");
			//_sbAppend.Append("<td style='text-align: center;' colspan='4'>Print Date&Time : <%=DateTime.Now.ToString('dd-MMM-yyyy HH:MM') %>");
			//_sbAppend.Append("</td>");
			//_sbAppend.Append("</tr>");
			_sbAppend.Append("</table>");

			return _sbAppend.ToString();
		}

		#region Download Challan Master Data
		public ActionResult Download_ChallanMaster(string id, AdminModels am)
		{
			if (Session["UserName"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			string firmuser = Session["UserName"].ToString();
			string adminid = Session["Adminid"].ToString();
			string ErrStatus = string.Empty;

			ViewBag.feecat = objCommon.GetFeeCat();

			return View();
		}

		[HttpPost]
		public ActionResult Download_ChallanMaster(string id, AdminModels am, FormCollection frm, string submit)
		{
			if (Session["UserName"] == null)
			{ return RedirectToAction("Index", "Admin"); }
			try
			{
				string adminid = Session["Adminid"].ToString();

				ViewBag.feecat = objCommon.GetFeeCat();

				ViewBag.SelectedItem = "0";
				string ErrStatus = string.Empty;
				string Search = string.Empty;
				DataSet ds1 = new DataSet();
				string firmuser = Session["UserName"].ToString();

				#region DownloadFile

				if (submit.ToUpper().Contains("DOWNLOAD"))
				{
					string feecat = string.Empty;
					Search = "challanid like '%%' ";
					if (frm["feecat1"] != "")
					{
						ViewBag.SelectedItem = frm["feecat1"];
						feecat = frm["feecat1"];
						TempData["SelectedItem"] = frm["feecat1"];
					}
					if (frm["FromDate"] != "")
					{
						ViewBag.FromDate = frm["FromDate"];
						TempData["FromDate"] = frm["FromDate"];
						Search += " and CONVERT(DATETIME, CONVERT(varchar,Chlngdate,103), 103)>=CONVERT(DATETIME, CONVERT(varchar,'" + frm["FromDate"].ToString() + "',103), 103)";
					}
					if (frm["ToDate"] != "")
					{
						ViewBag.ToDate = frm["ToDate"];
						TempData["ToDate"] = frm["ToDate"];
						Search += " and CONVERT(DATETIME, CONVERT(varchar,Chlngdate,103), 103)<=CONVERT(DATETIME, CONVERT(varchar,'" + frm["ToDate"].ToString() + "',103), 103)";
					}

					if (submit.ToLower().Contains("data"))
					{
						ds1 = objDB.Download_ChallanMaster(feecat, adminid, Search, out ErrStatus); // DownloadPrivateDataSPNew
					}
					string fileName1 = "Challan_Data_Admin_" + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

					if (ds1 == null)
					{
						ViewBag.Message = ErrStatus.ToString();
						ViewData["Result"] = "5";
						ViewBag.TotalCount = 0;
					}
					else if (ds1.Tables[0].Rows.Count > 0)
					{
						ViewBag.Message = "Data Downloaded Successfully";
						ViewData["Result"] = "1";
						ViewBag.TotalCount = ds1.Tables[0].Rows.Count;
						{

							//DataTable dt = ds1.Tables[0];
							using (XLWorkbook wb = new XLWorkbook())
							{
								wb.Worksheets.Add(ds1.Tables[0]);
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
						}

					}
					#endregion DownloadFile                          

				}
				return View(am);
			}
			catch (Exception ex)
			{
				ViewBag.Message = "Error: " + ex.Message;
				ViewData["Result"] = "50";
				return View();
			}
		}
		#endregion Download Challan Master Data



		#region Late Admission Admin

		[AdminLoginCheckFilter]
		public ActionResult LateAdmissionAdmin(AdminModels rm)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			var itemsch = new SelectList(new[]{new {ID="1",Name="ALL"},new{ID="2",Name="Request ID"},new{ID="3",Name="Candidate Name"},
			new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="Regno"},new{ID="7",Name="Schl"}}, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();

			List<LateAdmissionStatusMasters> myApprovalStatusList = _context.LateAdmissionStatusMasters.AsNoTracking().Where(s => s.Statuscode == 1 || s.Statuscode == 2 || s.Statuscode == 3 || s.Statuscode == 4).ToList();
			ViewBag.MyApprovalStatusList = myApprovalStatusList;
			ViewBag.SelectedStatus = "";
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();

			string Search = string.Empty;



			#region Action Assign Method
			if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
			{ ViewBag.IsStatus = 1; }
			else
			{

				string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
				string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
				//string AdminType = Session["AdminType"].ToString();
				//GetActionOfSubMenu(string cont, string act)
				DataSet aAct = objCommon.GetActionOfSubMenu(adminLoginSession.AdminId, controllerName, actionName);
				if (aAct.Tables[0].Rows.Count > 0)
				{
					ViewBag.IsStatus = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Equals("MODIFY")).Count();
				}
			}
			#endregion Action Assign Method

			Search = "schl like '%' and [status] !='0' and  isnull(PanelType,'')='LA' ";
			rm.StoreAllData = objDB.GetLateAdmissionSchl(Search);
			if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
			{
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
			}
			else
			{
				ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
			}
			return View(rm);
		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult LateAdmissionAdmin(AdminModels rm, FormCollection frm, string ApprovalStatus)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			#region Action Assign Method
			if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
			{ ViewBag.IsStatus = 1; }
			else
			{

				string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
				string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
				//string AdminType = Session["AdminType"].ToString();
				//GetActionOfSubMenu(string cont, string act)
				DataSet aAct = objCommon.GetActionOfSubMenu(adminLoginSession.AdminId, controllerName, actionName);
				if (aAct.Tables[0].Rows.Count > 0)
				{
					ViewBag.IsStatus = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Equals("MODIFY")).Count();
				}
			}
			#endregion Action Assign Method

			try
			{
				var itemsch = new SelectList(new[]{new {ID="1",Name="ALL"},new{ID="2",Name="Request ID"},new{ID="3",Name="Candidate Name"},
			new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="Regno"},new{ID="7",Name="Schl"}}, "ID", "Name", 1);
				ViewBag.MySch = itemsch.ToList();

				List<LateAdmissionStatusMasters> myApprovalStatusList = _context.LateAdmissionStatusMasters.AsNoTracking().Where(s => s.Statuscode == 1 || s.Statuscode == 2 || s.Statuscode == 3 || s.Statuscode == 4).ToList();
				ViewBag.MyApprovalStatusList = myApprovalStatusList;
				ViewBag.SelectedStatus = "";
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();

				if (ModelState.IsValid)
				{

					string Search = string.Empty;
					Search = "schl like '%' and [status] !='0'  and  isnull(PanelType,'')='LA' ";

					if (!string.IsNullOrEmpty(ApprovalStatus))
					{
						Search += " and [Status]='" + ApprovalStatus.ToString().Trim() + "'";
						ViewBag.SelectedStatus = ApprovalStatus;
					}
					if (frm["SelList"] != "")
					{
						ViewBag.SelectedItem = frm["SelList"];
						int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());


						if (frm["SearchString"] != "")
						{
							if (SelValueSch == 1)
							{ Search += "and RequestID like '%' "; }
							else if (SelValueSch == 2)
							{ Search += " and RequestID='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and Candi_Name like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and Father_Name  like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and Mother_Name like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 6)
							{ Search += " and RegNo like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 7)
							{ Search += " and schl= '" + frm["SearchString"].ToString() + "'"; }
						}

					}

					rm.StoreAllData = objDB.GetLateAdmissionSchl(Search);
					// asm.StoreAllData = objDB.SearchSchoolDetails(Search);
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
				else
				{
					return View(rm);
				}
			}
			catch (Exception)
			{

				return RedirectToAction("Index", "Home");
			}

		}
		[HttpPost]
		public ActionResult AddLateAdmissionAdmin(AdminModels rm, FormCollection frm, string PanelType)
		{

			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			string fileNM = "";
			try
			{
				string schl = frm["schlAdd"];
				string RID = "";
				string cls = frm["class"];
				string formNM = frm["formNM"];
				string regno = frm["regno"];
				string name = frm["name"];
				string fname = frm["fname"];
				string mname = frm["mname"];
				string dob = frm["dob"];
				string mobileno = frm["mobileno"];
				string usertype = "Admin";
				//rm.file = frm["file"];
				string OBoard = "P.S.E.B BOARD";
				if (formNM == "N2" || formNM == "M2" || formNM == "E2" || formNM == "T2")
				{
					OBoard = frm["Board"];
				}
				if (rm.file != null)
				{
					fileNM = Path.GetFileName(rm.file.FileName);
				}

				if (string.IsNullOrEmpty(PanelType))
				{
					PanelType = "LA";
				}
				string refno = new AbstractLayer.RegistrationDB().SetLateAdmissionSchl(adminLoginSession.USER, adminLoginSession.AdminEmployeeUserId, PanelType, schl, RID, cls, formNM, regno, name, fname, mname, dob, mobileno, fileNM, usertype, OBoard);

				if (rm.file != null)
				{
					fileNM = refno + Path.GetExtension(rm.file.FileName);
					//var path = Path.Combine(Server.MapPath("~/Upload/"+ formName + "/" + dist + "/Photo"), stdPic);
					var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/LateAdmission/" + fileNM));
					string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/LateAdmission/"));
					if (!Directory.Exists(FilepathExist))
					{
						Directory.CreateDirectory(FilepathExist);
					}
					rm.file.SaveAs(path);
				}
				TempData["resultUpdate"] = "11";

				return RedirectToAction("LateAdmissionAdmin", "Admin");

			}
			catch (Exception)
			{

				return RedirectToAction("Index", "Home");
			}

		}
		[HttpPost]
		public ActionResult ModifyLateAdmissionAdmin(AdminModels rm, FormCollection frm, string cmd, string RequestID, string PanelType)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			string fileNM = "";
			try
			{
				string schl = frm["schl"];
				//var did = frm.AllKeys[8].ToString();
				string RID = frm["RID"];
				string cls = frm["class"];
				string formNM = frm["formNM"];
				string regno = frm["regno"];
				string name = frm["name"];
				string fname = frm["fname"];
				string mname = frm["mname"];
				string dob = frm["dob"];
				string mobileno = frm["mobileno"];
				string usertype = "Admin";
				//rm.file = frm["file"];
				string OBoard = "P.S.E.B BOARD";
				if (formNM == "N2" || formNM == "M2" || formNM == "E2" || formNM == "T2")
				{
					OBoard = frm["Board"];
				}
				if (rm.file != null)
				{
					fileNM = Path.GetFileName(rm.file.FileName);
				}

				string refno = new AbstractLayer.RegistrationDB().SetLateAdmissionSchl(adminLoginSession.USER, adminLoginSession.AdminEmployeeUserId, PanelType, schl, RID, cls, formNM, regno, name, fname, mname, dob, mobileno, fileNM, usertype, OBoard);

				if (rm.file != null)
				{
					fileNM = refno + Path.GetExtension(rm.file.FileName);
					//var path = Path.Combine(Server.MapPath("~/Upload/"+ formName + "/" + dist + "/Photo"), stdPic);
					var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/LateAdmission/" + fileNM));
					string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/LateAdmission/"));
					if (!Directory.Exists(FilepathExist))
					{
						Directory.CreateDirectory(FilepathExist);
					}
					rm.file.SaveAs(path);
				}
				TempData["resultUpdate"] = "12";

				return RedirectToAction("LateAdmissionSchl", "Admin");

			}
			catch (Exception)
			{

				return RedirectToAction("Index", "Home");
			}

		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public JsonResult UpdStsLateAdmissionAdmin(string RID, string status, string apprDate, string Remarks, string mobileno)
		{

			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels rm = new AdminModels();
			FormCollection frc = new FormCollection();
			ViewBag.updStatus = "";
			if (Session["UserName"] == null)
			{
				return Json(ViewBag.updStatus);
			}
			ViewBag.updStatus = objDB.UpdStsLateAdmissionAdmin(adminLoginSession.USER, adminLoginSession.AdminEmployeeUserId, adminLoginSession.USERNAME, RID, status, apprDate, Remarks);

			AbstractLayer.DBClass dbclass = new AbstractLayer.DBClass();
			if ((mobileno != null || mobileno != "") && status == "2" && ViewBag.updStatus != null)
			{
				//  string Sms = "Your late admission form Request No. " + RID + " has been Approve till date " + apprDate + " , from PSEB.";

				// NEW SMS

				//Dear Student, Your request for late admission permission is {#var#} by PSEB with Req ID : {#var#}. Kindly contact with concerned school. Thanks PSEB
				string Sms = "Dear Student, Your request for late admission permission is Approved by PSEB with Req ID : " + RID + ". Kindly contact with concerned school. Thanks PSEB";
				string getSms = dbclass.gosms(mobileno, Sms);
				// string getSms = objCommon.gosms("9711021501", Sms);
			}
			if ((mobileno != null || mobileno != "") && status == "3" && ViewBag.updStatus != null)
			{
				//string Sms = "Your late admission form Request No. " + RID + " has been Rejected, from PSEB";
				string Sms = "Dear Student, Your request for late admission permission is Rejected by PSEB with Req ID : " + RID + ". Kindly contact with concerned school. Thanks PSEB";

				string getSms = dbclass.gosms(mobileno, Sms);
				// string getSms = objCommon.gosms("9711021501", Sms);
			}
			TempData["updStatus"] = ViewBag.updStatus;
			// return Json(ViewBag.updStatus);
			return Json(new { sn = ViewBag.updStatus }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult LateAdmPrintLetter(string CID)
		{
			if (Session["SCHL"] == null && Session["UserName"] == null)
			{
				return RedirectToAction("Logout", "Login");
			}
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels rm = new AdminModels();
			string result = "";
			if (CID != "" || CID != null)
			{
				rm.StoreAllData = objDB.LateAdmPrintLetter(CID);
				if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;

				}
				return View(rm);
			}
			else
			{
				return RedirectToAction("LateAdmissionAdmin", "Admin");
			}
		}
		public ActionResult LateAdmHistory(string CID)
		{
			if (Session["UserName"] == null)
			{
				return RedirectToAction("Logout", "Admin");
			}
			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels rm = new AdminModels();
			string result = "";
			if (CID != "" || CID != null)
			{
				rm.StoreAllData = objDB.LateAdmHistory(CID);
				if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;

				}
				return View(rm);
			}
			else
			{
				return RedirectToAction("LateAdmissionAdmin", "Admin");
			}
		}
		#endregion Late Admission Admin

		#region InterBoard Migration Admin


		public ActionResult InterBoardMigrationLetter(string CID)
		{
			AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
			RegistrationModels rm = new RegistrationModels();
			string result = "";
			if (CID != "" || CID != null)
			{
				rm.StoreAllData = objDB.LateAdmPrintLetter(CID);
				if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
				}
				return View(rm);
			}
			else
			{
				return RedirectToAction("LateAdmissionSchl", "RegistrationPortal");
			}
		}


		[AdminLoginCheckFilter]
		public ActionResult InterBoardMigrationAdmin()
		{

			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			#region Action Assign Method
			if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
			{ ViewBag.IsStatus = 1; }
			else
			{

				string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
				string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
				//string AdminType = Session["AdminType"].ToString();
				//GetActionOfSubMenu(string cont, string act)
				DataSet aAct = objCommon.GetActionOfSubMenu(adminLoginSession.AdminId, controllerName, actionName);
				if (aAct.Tables[0].Rows.Count > 0)
				{
					ViewBag.IsStatus = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Equals("MODIFY")).Count();
				}
			}
			#endregion Action Assign Method



			ViewBag.MyDist = AbstractLayer.AdminDB.getAdminDistAllowList("admin", Convert.ToString(Session["AdminId"]));

			//Session["SVFFormName"] = Request.QueryString["Form"];            
			var itemsch = new SelectList(new[]{new {ID="1",Name="ALL"},new{ID="2",Name="Request ID"},new{ID="3",Name="Candidate Name"},
			new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="Regno"},new{ID="7",Name="Schl"},new{ID="8",Name="FORM"}}, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();

			AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
			AdminModels rm = new AdminModels();
			string Search = string.Empty;
			string schlid = "";
			if (Session["UserName"] == null)
			{
				return RedirectToAction("Logout", "Admin");
			}

			Search = "schl like '%' and [status] !='0' and  isnull(PanelType,'')='IB'  ";
			rm.StoreAllData = objDB.GetLateAdmissionSchl(Search);
			if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
			{
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
			}
			else
			{
				ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
			}
			return View(rm);
		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult InterBoardMigrationAdmin(FormCollection frm, string SelDist)
		{

			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			#region Action Assign Method
			if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
			{ ViewBag.IsStatus = 1; }
			else
			{

				string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
				string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
				//string AdminType = Session["AdminType"].ToString();
				//GetActionOfSubMenu(string cont, string act)
				DataSet aAct = objCommon.GetActionOfSubMenu(adminLoginSession.AdminId, controllerName, actionName);
				if (aAct.Tables[0].Rows.Count > 0)
				{
					ViewBag.IsStatus = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuName").ToUpper().Equals("MODIFY")).Count();
				}
			}
			#endregion Action Assign Method


			try
			{

				ViewBag.MyDist = AbstractLayer.AdminDB.getAdminDistAllowList("admin", Convert.ToString(Session["AdminId"]));


				var itemsch = new SelectList(new[]{new {ID="1",Name="ALL"},new{ID="2",Name="Request ID"},new{ID="3",Name="Candidate Name"},
			new{ID="4",Name="Father's Name"},new{ID="5",Name="Mother's Name"},new{ID="6",Name="Regno"},new{ID="7",Name="Schl"},new{ID="8",Name="FORM"}}, "ID", "Name", 1);
				ViewBag.MySch = itemsch.ToList();
				AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
				AdminModels rm = new AdminModels();
				if (ModelState.IsValid)
				{

					string Search = string.Empty;
					string schlid = "";
					if (Session["UserName"] == null)
					{
						return RedirectToAction("Logout", "Admin");
					}
					Search = "schl like '%' and [status] !='0'  and  isnull(PanelType,'')='IB'";

					if (!string.IsNullOrEmpty(SelDist))
					{
						Search += " and DIST='" + SelDist + "' ";
					}

					if (frm["SelList"] != "")
					{
						ViewBag.SelectedItem = frm["SelList"];
						int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());


						if (frm["SearchString"] != "")
						{
							if (SelValueSch == 1)
							{ Search += "and RequestID like '%' "; }
							else if (SelValueSch == 2)
							{ Search += " and RequestID='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and Candi_Name like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and Father_Name  like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and Mother_Name like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 6)
							{ Search += " and RegNo like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 7)
							{ Search += " and schl= '" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 8)
							{ Search += " and Form= '" + frm["SearchString"].ToString() + "'"; }
						}

					}

					rm.StoreAllData = objDB.GetLateAdmissionSchl(Search);
					// asm.StoreAllData = objDB.SearchSchoolDetails(Search);
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
				else
				{
					return View(rm);
				}
			}
			catch (Exception)
			{

				return RedirectToAction("Index", "Home");
			}

		}



		#endregion Inter Board Migration Admin



		#region ViewAllExamForm

		public ActionResult ViewAllExamForm(Printlist obj)
		{
			if (Session["AdminId"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}
			int AdminId = Convert.ToInt32(Session["AdminId"]);
			//Search By
			var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "School Code" }, new { ID = "2", Name = "Udise Code" }, }, "ID", "Name", 1);
			ViewBag.MySch = itemSearchBy.ToList();
			ViewBag.SelectedItem = "0";
			return View();
		}

		[HttpPost]
		public ActionResult ViewAllExamForm(Printlist obj, FormCollection frm, string Sch1, string SearchString)
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				int AdminId = Convert.ToInt32(Session["AdminId"]);

				//Search By
				var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "School Code" }, new { ID = "2", Name = "Udise Code" }, }, "ID", "Name", 1);
				ViewBag.MySch = itemSearchBy.ToList();
				ViewBag.SelectedItem = "0";

				string SchlCode = string.Empty;
				if (Sch1 != "" && SearchString != "")
				{
					//frm["SearchString"]
					//frm["Sch1"]
					ViewBag.SelectedItem = Sch1;
					//TempData["SelectedItem"] = Sch1;
					ViewBag.SearchString = SearchString.ToString();

					int SelValueSch = Convert.ToInt32(Sch1.ToString());
					if (SelValueSch == 1)
					{ SchlCode = SearchString.ToString(); }
					else if (SelValueSch == 2)
					{
						SchoolModels sm = new AbstractLayer.SchoolDB().getSchoolDetailsByUDISECode(SearchString.ToString());
						if (sm != null)
						{
							SchlCode = sm.SCHL;
						}
						else
						{
							SchlCode = SearchString;
						}
					}

					obj.StoreAllData = new AbstractLayer.AdminDB().GetExamFormCalFeeSPAdmin(AdminId, SchlCode);//GetExamFormCalFeeSP
					if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;
					}
					else
					{
						ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
					}
				}
				return View(obj);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return View();
			}
		}

		public ActionResult DownloadExamFinalReport(string ChallanId, string Filename)
		{
			try
			{
				ChallanMasterModel CM = new ChallanMasterModel();
				if (ChallanId == null)
				{
					return RedirectToAction("ViewAllExamForm", "Admin");
				}
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string ChallanId1 = ChallanId.ToString();
				int OutStatus;
				string Usertype = "User";
				Printlist objP = new Printlist();
				// objP.StoreAllData = objDB.DownloadExamFinalReport(schl, ChallanId);
				DataSet ds = new AbstractLayer.SchoolDB().DownloadExamFinalReport(ChallanId1, Usertype, out OutStatus);
				if (OutStatus == 1)
				{
					return File(Filename, "application/pdf");
				}
				else
				{
					ViewBag.Message = "File Not Download";
					ViewBag.TotalCount = 0;
					return RedirectToAction("ViewAllExamForm", "Admin");
				}
			}
			catch (Exception ex)
			{
				return RedirectToAction("ViewAllExamForm", "Admin");
			}

		}


		public ActionResult ViewCompleteExamFormFeeDetails(string schlcode, string ChallanId)//2012117535028
		{
			try
			{
				// AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();               
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				Printlist obj = new Printlist();
				obj.StoreAllData = new AbstractLayer.SchoolDB().CompleteExamFormFeeByChallan(schlcode, ChallanId);
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
				return RedirectToAction("ViewAllExamForm", "Admin");
				//return View();
			}
		}


		public ActionResult ViewSubjectWiseCountOfStudents(string schlcode, string ChallanId)//2012117535028
		{
			try
			{
				// AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();               
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				Printlist obj = new Printlist();
				obj.StoreAllData = new AbstractLayer.SchoolDB().CompleteExamFormFeeByChallan(schlcode, ChallanId);
				// obj.StoreAllData = new AbstractLayer.SchoolDB().ExamFormDataByChallan(2,schlcode, ChallanId);//tbl-2 for subject wise count
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
				return RedirectToAction("ViewAllExamForm", "Admin");
				//return View();
			}
		}


		#endregion

		//--------------------Capacity Letter For DM Login------------------------------//
		public ActionResult DMCapacityLetter(AdminModels AM)
		{
			AbstractLayer.AdminDB OBJDB = new AbstractLayer.AdminDB();
			string DistAllow = null;
			ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				List<SelectListItem> DistList = new List<SelectListItem>();
				if (Session["adminUSER"].ToString().ToUpper() == "ADMIN")
				{
					DataSet Dresult = OBJDB.DMgetallDistAllow(); //                  
					foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
					{
						DistList.Add(new SelectListItem { Text = @dr["USER"].ToString(), Value = @dr["Dist_Allow"].ToString() });
					}
					ViewBag.Dist = DistList;
				}
				else
				{
					DistList.Add(new SelectListItem { Text = Session["adminUSER"].ToString(), Value = Session["DistAllow"].ToString() });
					DistAllow = Session["DistAllow"].ToString();
					ViewBag.Dist = DistList;
				}
				AM.SelExamDist = DistAllow;
				DistAllow = Session["DistAllow"].ToString();
				AM.StoreAllData = OBJDB.DMCapacityLetter(DistAllow);
				if (AM.StoreAllData == null || AM.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "DATA DOESN'T EXIST";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = AM.StoreAllData.Tables[0].Rows.Count;
					return View(AM);
				}

			}
			catch (Exception ex)
			{
				return View();

			}
		}
		[HttpPost]
		public ActionResult DMCapacityLetter(AdminModels AM, FormCollection frc)
		{
			AbstractLayer.AdminDB OBJDB = new AbstractLayer.AdminDB();
			string DistAllow = null;
			ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				List<SelectListItem> DistList = new List<SelectListItem>();
				if (Session["adminUSER"].ToString().ToUpper() == "ADMIN")
				{
					DataSet Dresult = OBJDB.DMgetallDistAllow(); //                  
					foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
					{
						DistList.Add(new SelectListItem { Text = @dr["USER"].ToString(), Value = @dr["Dist_Allow"].ToString() });
					}
					ViewBag.Dist = DistList;
				}
				else
				{
					DistList.Add(new SelectListItem { Text = Session["adminUSER"].ToString(), Value = Session["DistAllow"].ToString() });
					DistAllow = Session["DistAllow"].ToString();
					ViewBag.Dist = DistList;
				}
				AM.SelExamDist = DistAllow;
				DistAllow = frc["SelExamDist"].ToString();

				AM.StoreAllData = OBJDB.DMCapacityLetter(DistAllow);
				if (AM.StoreAllData == null || AM.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "DATA DOESN'T EXIST";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = AM.StoreAllData.Tables[0].Rows.Count;
					return View(AM);
				}

			}
			catch (Exception ex)
			{
				//oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return View();

			}
		}

		public ActionResult DMCapacityLetterCentrewise(AdminModels AM)
		{
			AbstractLayer.AdminDB OBJDB = new AbstractLayer.AdminDB();
			string DistAllow = null;
			ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				List<SelectListItem> DistList = new List<SelectListItem>();
				if (Session["adminUSER"].ToString().ToUpper() == "ADMIN")
				{
					DataSet Dresult = OBJDB.DMgetallDistAllow(); //                  
					foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
					{
						DistList.Add(new SelectListItem { Text = @dr["USER"].ToString(), Value = @dr["Dist_Allow"].ToString() });
					}
					ViewBag.Dist = DistList;
				}
				else
				{
					DistList.Add(new SelectListItem { Text = Session["adminUSER"].ToString(), Value = Session["DistAllow"].ToString() });
					DistAllow = Session["DistAllow"].ToString();
					ViewBag.Dist = DistList;
				}
				AM.SelExamDist = DistAllow;
				DistAllow = Session["DistAllow"].ToString();
				AM.StoreAllData = null;//OBJDB.DMCapacityLetterCentrewise(DistAllow);
				if (AM.StoreAllData == null || AM.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "DATA DOESN'T EXIST";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = AM.StoreAllData.Tables[0].Rows.Count;
					return View(AM);
				}

			}
			catch (Exception ex)
			{
				return View();

			}
		}
		[HttpPost]
		public ActionResult DMCapacityLetterCentrewise(AdminModels AM, FormCollection frc)
		{
			AbstractLayer.AdminDB OBJDB = new AbstractLayer.AdminDB();
			string DistAllow = null;
			ViewBag.Date = DateTime.Now.ToString("dd/MM/yyyy");
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				List<SelectListItem> DistList = new List<SelectListItem>();
				if (Session["adminUSER"].ToString().ToUpper() == "ADMIN")
				{
					DataSet Dresult = OBJDB.DMgetallDistAllow(); //                  
					foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
					{
						DistList.Add(new SelectListItem { Text = @dr["USER"].ToString(), Value = @dr["Dist_Allow"].ToString() });
					}
					ViewBag.Dist = DistList;
				}
				else
				{
					DistList.Add(new SelectListItem { Text = Session["adminUSER"].ToString(), Value = Session["DistAllow"].ToString() });
					DistAllow = Session["DistAllow"].ToString();
					ViewBag.Dist = DistList;
				}
				AM.SelExamDist = DistAllow;
				DistAllow = frc["SelExamDist"].ToString();
				AM.SearchResult = frc["SearchResult"].ToString();
				ViewBag.Searchstring = frc["SearchResult"].ToString();
				AM.StoreAllData = OBJDB.DMCapacityLetterCentrewise(DistAllow, AM.SearchResult);
				if (AM.StoreAllData == null || AM.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "DATA DOESN'T EXIST";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = AM.StoreAllData.Tables[0].Rows.Count;
					return View(AM);
				}

			}
			catch (Exception ex)
			{
				//oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return View();

			}
		}
		//---------------------------End------------------------------//

		#region ViewEAffiliationAdmin
		[AdminLoginCheckFilter]
		public ActionResult ViewEAffiliationAdmin(EAffiliationModel spi, int? page, string AppType)
		{
			try
			{
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string AdminType = Session["AdminType"].ToString().ToUpper();
				string AdminUser = Session["AdminUser"].ToString().ToUpper();

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
					ViewBag.MyDist = ViewBag.DistUser; // User District
				}

				ViewBag.MyAppCls = new AbstractLayer.DBClass().GetAllClass().ToList();
				ViewBag.SelectedCls = "0";


				List<EAffiliationApplicationStatusMaster> eAffiliationApplicationStatusMasterList = new AbstractLayer.EAffiliationDB().ForwardListByAppNoStatus(AppType, 1, "", AdminUser);
				ViewBag.MyForwardList = eAffiliationApplicationStatusMasterList;
				//AppType
				ViewBag.MyAppType = AbstractLayer.EAffiliationDB.GetApplicationTypeList().ToList();
				ViewBag.SelectedAppType = "0";

				//Search By
				var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "Application No" }, new { ID = "6", Name = "School Code" }, new { ID = "2", Name = "UDISE Code" },
				new { ID = "3", Name = "School Name" },new { ID = "4", Name = "Station Name" },new { ID = "5", Name = "Mobile No" },new { ID = "7", Name = "EAffiliation Type" },}, "ID", "Name", 1);
				ViewBag.MySch = itemSearchBy.ToList();
				ViewBag.SelectedSearchBy = "0";

				var itemAction = new SelectList(new[] { new { ID = "1", Name = "ALL APPLICATION" },
					 new { ID = "2", Name = "APPLICATION REGISTERED" },
					  new { ID = "3", Name = "FEE PAID  " },
					   new { ID = "4", Name = "FORM UNLOCKED" },
					new { ID = "5", Name = "CLOSED APPLICATION" },
				 new { ID = "6", Name = "Pending In School Master" },}, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";

				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = "";
				if (TempData["SearchViewEAffiliationAdmin"] != null)
				{
					Search += TempData["SearchViewEAffiliationAdmin"].ToString();
					ViewBag.SelectedAction = TempData["SelAction"];
					ViewBag.SelectedItem = TempData["SelectedItem"];
					ViewBag.SelectedDist = TempData["SelectedDist"];
					ViewBag.SelectedAppType = TempData["SelAppType"];

					TempData["SelAppType"] = ViewBag.SelectedAppType;

					if (DistAllow != "")
					{
						Search += " and DIST in (" + DistAllow + ")";
					}

					if (ViewBag.SelectedAppType == "AFF")
					{
						spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser, Search, "", pageIndex, 4);
					}
					else if (ViewBag.SelectedAppType == "AC")
					{
						spi.StoreAllData = new AbstractLayer.AffiliationDB().AffiliationContinuationList(AdminUser, Search, "", pageIndex, 4);
					}
					else if (ViewBag.SelectedAppType == "AS")
					{
						spi.StoreAllData = new AbstractLayer.AdditionalSectionDB().AdditionalSectionList(AdminUser, Search, "", pageIndex, 4);
					}

					if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.LastPageIndex = 0;
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						TempData["SearchViewEAffiliationAdmin"] = Search;
						TempData.Keep();
						ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
						int count = Convert.ToInt32(spi.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
						ViewBag.TotalCount1 = count;
						int tp = Convert.ToInt32(count);
						int pn = tp / 20;
						int cal = 20 * pn;
						int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
						if (res >= 1)
							ViewBag.pn = pn + 1;
						else
							ViewBag.pn = pn;
						return View(spi);
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
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult ViewEAffiliationAdmin(SchoolPremisesInformation spi, FormCollection frm, int? page, string AppType)
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string AdminType = Session["AdminType"].ToString().ToUpper();
				string AdminUser = Session["AdminUser"].ToString().ToUpper();

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
					ViewBag.MyDist = ViewBag.DistUser; // User District
				}

				ViewBag.MyAppCls = new AbstractLayer.DBClass().GetAllClass().ToList();
				ViewBag.SelectedCls = "0";
				List<EAffiliationApplicationStatusMaster> eAffiliationApplicationStatusMasterList = new AbstractLayer.EAffiliationDB().ForwardListByAppNoStatus(AppType, 1, "", AdminUser);
				ViewBag.MyForwardList = eAffiliationApplicationStatusMasterList;
				//AppType
				ViewBag.MyAppType = AbstractLayer.EAffiliationDB.GetApplicationTypeList().ToList();
				ViewBag.SelectedAppType = "0";

				//Status
				var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "Application No" }, new { ID = "6", Name = "School Code" }, new { ID = "2", Name = "UDISE Code" },
				new { ID = "3", Name = "School Name" },new { ID = "4", Name = "Station Name" },new { ID = "5", Name = "Mobile No" },new { ID = "7", Name = "EAffiliation Type" },}, "ID", "Name", 1);
				ViewBag.MySch = itemSearchBy.ToList();
				ViewBag.SelectedSearchBy = "0";

				var itemAction = new SelectList(new[] { new { ID = "1", Name = "ALL APPLICATION" },
					 new { ID = "2", Name = "APPLICATION REGISTERED" },
					  new { ID = "3", Name = "FEE PAID  " },
					   new { ID = "4", Name = "FORM UNLOCKED" },
					new { ID = "5", Name = "CLOSED APPLICATION" },
				 new { ID = "6", Name = "Pending In School Master" },}, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";


				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;
				Search = "APPNO is not null ";

				int SelAction = 0;
				ViewBag.SelectedAppType = AppType;
				TempData["SelAppType"] = AppType;
				if (AppType == "AFF")
				{

					if (frm["AppCls"] != "")
					{
						ViewBag.SelectedCls = frm["AppCls"];
						TempData["SelectedCls"] = frm["AppCls"];
						Search += " and ClassLevel='" + frm["AppCls"].ToString() + "'";
					}
					if (frm["Dist1"] != "")
					{
						ViewBag.SelectedDist = frm["Dist1"];
						TempData["SelectedDist"] = frm["Dist1"];
						Search += " and Dist='" + frm["Dist1"].ToString() + "'";
					}

					if (frm["SelAction"] != "")
					{
						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						SelAction = Convert.ToInt32(frm["SelAction"].ToString());
						if (SelAction == 2)//Registered
						{
							Search += " and  HistoryStatus='APPLICATION REGISTERED'";
						}
						else if (SelAction == 3)//Registered
						{
							Search += " and  HistoryStatus='FEE PAID'";
						}
						else if (SelAction == 4)//FORM UNLOCKED
						{
							Search += " and IsFormLock='0'";
						}
						else if (SelAction == 5)
						{
							Search += " and  CurrentApplicationStatus=8";
						}
						else if (SelAction == 6)
						{ Search += "  and  FinalStatus='Fee Verified' and isnull(SchlMasterStatus,'')='' "; }
					}

					if (frm["Sch1"] != "")
					{
						ViewBag.SelectedItem = frm["Sch1"];
						TempData["SelectedItem"] = frm["Sch1"];
						int SelValueSch = Convert.ToInt32(frm["Sch1"].ToString());

						if (frm["SearchString"] != "")
						{
							if (SelValueSch == 1)
							{ Search += " and APPNO=" + frm["SearchString"].ToString(); }
							else if (SelValueSch == 2)
							{ Search += " and  UDISECODE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  SCHLNME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and  SCHLMOBILE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  SCHL='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 7)
							{ Search += " and  EAffType='" + frm["SearchString"].ToString() + "'"; }

						}
					}
					if (DistAllow != "")
					{
						Search += " and DIST in (" + DistAllow + ")";
					}

					TempData["SearchViewEAffiliationAdmin"] = Search;
					TempData.Keep(); // to store search value for view

					spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser, Search, "", pageIndex, 4);

				}
				else if (AppType == "AC")
				{

					if (frm["AppCls"] != "")
					{
						ViewBag.SelectedCls = frm["AppCls"];
						TempData["SelectedCls"] = frm["AppCls"];
						Search += " and ClassLevel='" + frm["AppCls"].ToString() + "'";
					}
					if (frm["Dist1"] != "")
					{
						ViewBag.SelectedDist = frm["Dist1"];
						TempData["SelectedDist"] = frm["Dist1"];
						Search += " and Dist='" + frm["Dist1"].ToString() + "'";
					}

					if (frm["SelAction"] != "")
					{
						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						SelAction = Convert.ToInt32(frm["SelAction"].ToString());
						if (SelAction == 2)//Registered
						{
							Search += "  and  ID is not null  ";
						}
						else if (SelAction == 3)//Fees Paid
						{
							Search += "  and  IsFinalSubmit='FINAL'  ";
						}
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
							{ Search += " and  UDISECODE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  SCHLNME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and  MOBILE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  SCHL='" + frm["SearchString"].ToString() + "'"; }
							//else if (SelValueSch == 7)
							//{ Search += " and  EAffType='" + frm["SearchString"].ToString() + "'"; }

						}
					}
					if (DistAllow != "")
					{
						Search += " and DIST in (" + DistAllow + ")";
					}

					TempData["SearchViewEAffiliationAdmin"] = Search;
					TempData.Keep(); // to store search value for view

					spi.StoreAllData = new AbstractLayer.AffiliationDB().AffiliationContinuationList(AdminUser, Search, "", pageIndex, 4);

				}
				else if (AppType == "AS")
				{
					if (frm["AppCls"] != "")
					{
						ViewBag.SelectedCls = frm["AppCls"];
						TempData["SelectedCls"] = frm["AppCls"];
						Search += " and ClassLevel='" + frm["AppCls"].ToString() + "'";
					}
					if (frm["Dist1"] != "")
					{
						ViewBag.SelectedDist = frm["Dist1"];
						TempData["SelectedDist"] = frm["Dist1"];
						Search += " and Dist='" + frm["Dist1"].ToString() + "'";
					}

					if (frm["SelAction"] != "")
					{
						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						SelAction = Convert.ToInt32(frm["SelAction"].ToString());
						if (SelAction == 2)//Registered
						{
							Search += "  and  ID is not null  ";
						}
						else if (SelAction == 3)//Fees Paid
						{
							Search += "  and  IsFinalSubmit='FINAL'  ";
						}
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
							{ Search += " and  UDISECODE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  SCHLNME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and  MOBILE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  SCHL='" + frm["SearchString"].ToString() + "'"; }
							//else if (SelValueSch == 7)
							//{ Search += " and  EAffType='" + frm["SearchString"].ToString() + "'"; }

						}
					}
					if (DistAllow != "")
					{
						Search += " and DIST in (" + DistAllow + ")";
					}

					TempData["SearchViewEAffiliationAdmin"] = Search;
					TempData.Keep(); // to store search value for view

					spi.StoreAllData = new AbstractLayer.AdditionalSectionDB().AdditionalSectionList(AdminUser, Search, "", pageIndex, 4);

				}



				if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.LastPageIndex = 0;
					ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
					int count = Convert.ToInt32(spi.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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

				return View(spi);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return View();
			}
		}


		// update Email-Mobile Affiliation
		public JsonResult updateEmailMobileEAffiliationAdmin(string Email, string Mobile, string APPNO)
		{
			try
			{
				string dee = "0";
				int outstatus = 0;
				string outError = "0";
				if (APPNO != "" && Email != "" && Mobile != "")
				{
					int AdminId = Convert.ToInt32(Session["AdminId"]);
					EAffiliationModel _EAffiliationModel = new EAffiliationModel();
					_EAffiliationModel.UpdatedBy = AdminId;
					_EAffiliationModel.APPNO = APPNO;
					_EAffiliationModel.SCHLMOBILE = Mobile;
					_EAffiliationModel.SCHLEMAIL = Email;
					_EAffiliationModel.Remarks = "Updated Email and Mobile Number by Admin";

					int result = new AbstractLayer.EAffiliationDB().EAffiliation(_EAffiliationModel, 10, out outError);
					// DataSet ds = new AbstractLayer.SchoolDB().ViewEAffiliationAdmin(2, cancelremarks, schl, 1, out outstatus, AdminId);
					dee = outError;
				}
				else
				{ dee = "2"; }
				return Json(new { sn = dee }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { sn = "-1" }, JsonRequestBehavior.AllowGet);
			}
		}

		public JsonResult JqSendPasswordEAffilication(string APPNO)
		{
			string AdminUser = Session["AdminUser"].ToString().ToUpper();

			string dee = "1";
			if (!string.IsNullOrEmpty(APPNO))
			{
				string Search = string.Empty;
				Search = "APPNO= '" + APPNO.ToString() + "'";
				DataSet result = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser, Search, "", 1, 4);
				if (result.Tables[0].Rows.Count > 0)
				{
					dee = "1";
					string Mobile = result.Tables[0].Rows[0]["SCHLMOBILE"].ToString();
					string SCHLEMAIL = result.Tables[0].Rows[0]["SCHLEMAIL"].ToString();
					string Password = result.Tables[0].Rows[0]["PWD"].ToString();
					string user = result.Tables[0].Rows[0]["APPNO"].ToString();
					string Sms = "EAffiliation Portal User Details, User Id: " + user + " and Password: " + Password + " for login. Regards PSEB";
					if (Mobile != "")
					{
						string getSms = objCommon.gosms(Mobile, Sms);
					}
				}
				return Json(new { dee = dee }, JsonRequestBehavior.AllowGet);
			}
			dee = "0";
			return Json(new { dee = dee }, JsonRequestBehavior.AllowGet);
		}


		public JsonResult unlockEAffiliationByAppNO(string UnlockRemarks, string APPNO, string AppType, string SCHLMOBILE)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			try
			{
				string dee = "0";
				int outstatus = 0;
				string outError = "0";

				if (AppType.ToUpper() == "AFF".ToUpper())
				{
					if (APPNO != "" && UnlockRemarks != "")
					{
						int AdminId = Convert.ToInt32(Session["AdminId"]);
						EAffiliationModel _EAffiliationModel = new EAffiliationModel();
						_EAffiliationModel.UpdatedBy = AdminId;
						_EAffiliationModel.APPNO = APPNO;
						_EAffiliationModel.Remarks = UnlockRemarks;
						_EAffiliationModel.EmpUserId = adminLoginSession.AdminEmployeeUserId;
						int result = new AbstractLayer.EAffiliationDB().EAffiliation(_EAffiliationModel, 20, out outError);
						dee = outError;

						if (outError == "1")
						{
							string Sms = "Your form for E-Affiliation with application " + APPNO + " is unlocked by PSEB. Kindly update form and submit again. You can check status of application on dashboard.Regards PSEB";
							try
							{
								string getSms = new AbstractLayer.DBClass().gosms(SCHLMOBILE, Sms);
								//string getSms = new AbstractLayer.DBClass().gosms("9711819184", Sms);
							}
							catch (Exception) { }
						}


					}
					else
					{ dee = "2"; }
				}
				else if (AppType.ToUpper() == "AC".ToUpper())
				{
					if (APPNO != "" && UnlockRemarks != "")
					{
						int AdminId = Convert.ToInt32(Session["AdminId"]);
						AffiliationModel _EAffiliationModel = new AffiliationModel();
						_EAffiliationModel.UpdatedBy = AdminId;
						_EAffiliationModel.SCHL = APPNO;
						_EAffiliationModel.Remarks = UnlockRemarks;
						_EAffiliationModel.EmpUserId = adminLoginSession.AdminEmployeeUserId;
						int result = new AbstractLayer.AffiliationDB().AffiliationContinuationAction(_EAffiliationModel, 20, out outError);
						dee = outError;

						if (outError == "1")
						{
							string Sms = "Your form for Annual Progress with application " + APPNO + " is unlocked by PSEB. Kindly update form and submit again. Regards PSEB";
							try
							{
								string getSms = new AbstractLayer.DBClass().gosms(SCHLMOBILE, Sms);
								//string getSms = new AbstractLayer.DBClass().gosms("9711819184", Sms);
							}
							catch (Exception) { }
						}


					}
					else
					{ dee = "2"; }
				}
				else if (AppType.ToUpper() == "AS".ToUpper())
				{
					if (APPNO != "" && UnlockRemarks != "")
					{
						int AdminId = Convert.ToInt32(Session["AdminId"]);
						AdditionalSectionModel _EAffiliationModel = new AdditionalSectionModel();
						_EAffiliationModel.UpdatedBy = AdminId;
						_EAffiliationModel.SCHL = APPNO;
						_EAffiliationModel.Remarks = UnlockRemarks;
						_EAffiliationModel.EmpUserId = adminLoginSession.AdminEmployeeUserId;
						int result = new AbstractLayer.AdditionalSectionDB().AdditionalSectionAction(_EAffiliationModel, 20, out outError);
						dee = outError;

						if (outError == "1")
						{
							string Sms = "Your form for Additional Section with application " + APPNO + " is unlocked by PSEB. Kindly update form and submit again.Regards PSEB";
							try
							{
								string getSms = new AbstractLayer.DBClass().gosms(SCHLMOBILE, Sms);
								//string getSms = new AbstractLayer.DBClass().gosms("9711819184", Sms);
							}
							catch (Exception) { }
						}


					}
					else
					{ dee = "2"; }
				}


				return Json(new { sn = dee }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { sn = "-1" }, JsonRequestBehavior.AllowGet);
			}
		}

		[HttpPost]
		public JsonResult updateSchoolMasterEAffiliationAdmin(string AppType, string APPNO, string modelData)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
			try
			{
				string dee = "0";
				int outstatus = 0;
				string outError = "0";
				if (!string.IsNullOrEmpty(AppType) && !string.IsNullOrEmpty(APPNO))
				{
					EAffiliationSchoolMaster oModel = JsonConvert.DeserializeObject<EAffiliationSchoolMaster>(modelData);
					int AdminId = Convert.ToInt32(Session["AdminId"]);
					oModel.AppType = AppType;
					if (oModel.SchlCat.ToLower() == "new")
					{
						// oModel.SCHL = oModel.SCHLE = oModel.SCHLP = oModel.STATIONE = oModel.STATIONP = "";
					}
					else if (oModel.SchlCat.ToLower() == "old")
					{
						if (string.IsNullOrEmpty(oModel.SCHL))
						{
							return Json(new { sn = "5" }, JsonRequestBehavior.AllowGet);
						}
					}

					int result = new AbstractLayer.EAffiliationDB().UpdateSchoolMasterEAffiliation(oModel, adminLoginSession.AdminEmployeeUserId, adminLoginSession.AdminId, 1, out outError);
					dee = outError;
				}
				else
				{ dee = "2"; }
				return Json(new { sn = dee }, JsonRequestBehavior.AllowGet);
			}
			catch (Exception ex)
			{
				return Json(new { sn = "-1" }, JsonRequestBehavior.AllowGet);
			}
		}

		#endregion ViewEAffiliationAdmin


		#region EAffiliation Admin Portal (New)

		[AdminLoginCheckFilter]
		public ActionResult InboxEAffiliation(EAffiliationModel spi, int? page, string AppType)
		{
			try
			{
				ViewBag.IsHistory = null;
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string AdminType = Session["AdminType"].ToString().ToUpper();
				string AdminUser = Session["AdminUser"].ToString().ToUpper();

				List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDLForMigration();
				ViewBag.MyApprovalStatusList = MyAcceptRejectList;

				string DistAllow = "";
				// Dist Allowed
				if (ViewBag.DistAllow == null)
				{ return RedirectToAction("Index", "Admin"); }
				else
				{
					DistAllow = ViewBag.DistAllow;
				}

				if (ViewBag.DistUser == null)
				{
					ViewBag.MyDist = null;
				}
				else
				{
					ViewBag.MyDist = ViewBag.DistUser; // User District

				}


				#region Action Assign Method
				if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{ ViewBag.IsObjection = 1; ViewBag.IsFillInspectionReport = 1; ViewBag.IsViewInspectionReport = 1; }
				else
				{

					string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
					string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						ViewBag.IsObjection = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("Admin/AffiliationObjectionLetter".ToUpper())).Count();
						ViewBag.IsFillInspectionReport = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/FillEAffiliationInspectionReport".ToUpper())).Count();
						ViewBag.IsViewInspectionReport = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/ViewEAffiliationInspectionReport".ToUpper())).Count();
						//ViewBag.IsCloseApplication = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
						// ViewBag.IsUpdateEmailMobile = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
						// ViewBag.IsUnlockForm = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
					}
				}
				#endregion Action Assign Method
				ViewBag.MyAppCls = new AbstractLayer.DBClass().GetAllClass().ToList();
				ViewBag.SelectedCls = "0";

				List<EAffiliationApplicationStatusMaster> eAffiliationApplicationStatusMasterList = new AbstractLayer.EAffiliationDB().ForwardListByAppNoStatus(AppType, 1, "", AdminUser);
				ViewBag.MyForwardList = eAffiliationApplicationStatusMasterList;
				//AppType
				ViewBag.MyAppType = AbstractLayer.EAffiliationDB.GetApplicationTypeList().ToList();
				ViewBag.SelectedAppType = "0";
				ViewBag.searchstatus = AbstractLayer.EAffiliationDB.GetApplicationStatusTypeList().ToList();
				ViewBag.Selectedsearchstatus = "0";



				//Search By
				//Status
				var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "Application No" }, new { ID = "6", Name = "School Code" }, new { ID = "2", Name = "UDISE Code" },
				new { ID = "3", Name = "School Name" },new { ID = "4", Name = "Station Name" },new { ID = "5", Name = "Mobile No" },new { ID = "7", Name = "EAffiliation Type" },}, "ID", "Name", 1);
				ViewBag.MySch = itemSearchBy.ToList();
				ViewBag.SelectedSearchBy = "0";

				var itemAction = new SelectList(new[] { new { ID = "1", Name = "ALL APPLICATION" },
					 new { ID = "2", Name = "APPLICATION REGISTERED" },
					  new { ID = "3", Name = "FEE PAID  " },
					   new { ID = "4", Name = "FORM UNLOCKED" },
					new { ID = "5", Name = "CLOSED APPLICATION" },
				 new { ID = "6", Name = "Pending In School Master" },}, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";


				//ViewBag.MyForwardList

				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = "";

				if (TempData["SearchInboxEAffiliation"] != null)
				{
					Search += TempData["SearchInboxEAffiliation"].ToString();
					ViewBag.SelectedAction = TempData["SelAction"];
					ViewBag.SelectedItem = TempData["SelectedItem"];
					ViewBag.SelectedDist = TempData["SelectedDist"];
					ViewBag.SelectedAppType = TempData["SelAppType"];


					TempData["SelAppType"] = ViewBag.SelectedAppType;
					//
					if (DistAllow != "")
					{
						Search += " and DIST in (" + DistAllow + ") ";
					}
					if (AdminType.ToUpper() != "ADMIN".ToUpper())
					{
						Search += " and upper(CurrentApplicationBranch)='" + AdminType + "'";
					}
					//


					// Search += " and  isnull(ApprovalStatus,'0')=''";  // blank

					if (ViewBag.SelectedAppType == "AFF")
					{
						spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser, Search, "", pageIndex, 11);
					}
					else if (ViewBag.SelectedAppType == "AC")
					{
						spi.StoreAllData = new AbstractLayer.AffiliationDB().AffiliationContinuationList(AdminUser, Search, "", pageIndex, 11);
					}
					else if (ViewBag.SelectedAppType == "AS")
					{
						spi.StoreAllData = new AbstractLayer.AdditionalSectionDB().AdditionalSectionList(AdminUser, Search, "", pageIndex, 11);
					}

					//spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser,Search, "", pageIndex, 11); // 11 -- INBOX

					if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.LastPageIndex = 0;
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						TempData["SearchInboxEAffiliation"] = Search;
						TempData.Keep();
						ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
						int count = Convert.ToInt32(spi.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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
				return View(spi);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}


		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult InboxEAffiliation(EAffiliationModel spi, FormCollection frm, int? page, string AppType)
		{
			try
			{
				ViewBag.IsHistory = null;
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string AdminType = Session["AdminType"].ToString().ToUpper();
				List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDLForMigration();
				ViewBag.MyApprovalStatusList = MyAcceptRejectList;
				string DistAllow = "";
				// Dist Allowed
				if (ViewBag.DistAllow == null)
				{ return RedirectToAction("Index", "Admin"); }
				else
				{
					DistAllow = ViewBag.DistAllow;
				}
				if (ViewBag.DistUser == null)
				{
					ViewBag.MyDist = null;

				}
				else
				{
					ViewBag.MyDist = ViewBag.DistUser; // User District
				}

				#region Action Assign Method
				if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{ ViewBag.IsObjection = 1; ViewBag.IsFillInspectionReport = 1; ViewBag.IsViewInspectionReport = 1; }
				else
				{

					string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
					string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						ViewBag.IsObjection = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("Admin/AffiliationObjectionLetter".ToUpper())).Count();
						ViewBag.IsFillInspectionReport = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/FillEAffiliationInspectionReport".ToUpper())).Count();
						ViewBag.IsViewInspectionReport = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/ViewEAffiliationInspectionReport".ToUpper())).Count();
						//ViewBag.IsCloseApplication = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
						// ViewBag.IsUpdateEmailMobile = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
						// ViewBag.IsUnlockForm = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
					}
				}
				#endregion Action Assign Method

				ViewBag.MyAppCls = new AbstractLayer.DBClass().GetAllClass().ToList();
				ViewBag.SelectedCls = "0";
				string AdminUser = Session["AdminUser"].ToString().ToUpper();


				List<EAffiliationApplicationStatusMaster> eAffiliationApplicationStatusMasterList = new AbstractLayer.EAffiliationDB().ForwardListByAppNoStatus(AppType, 1, "", AdminUser);
				ViewBag.MyForwardList = eAffiliationApplicationStatusMasterList;
				//AppType
				//AppType
				ViewBag.MyAppType = AbstractLayer.EAffiliationDB.GetApplicationTypeList().ToList();
				ViewBag.SelectedAppType = "0";
				ViewBag.searchstatus = AbstractLayer.EAffiliationDB.GetApplicationStatusTypeList().ToList();
				ViewBag.Selectedsearchstatus = "0";
				//Status
				var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "Application No" }, new { ID = "6", Name = "School Code" }, new { ID = "2", Name = "UDISE Code" },
				new { ID = "3", Name = "School Name" },new { ID = "4", Name = "Station Name" },new { ID = "5", Name = "Mobile No" },new { ID = "7", Name = "EAffiliation Type" },}, "ID", "Name", 1);
				ViewBag.MySch = itemSearchBy.ToList();
				ViewBag.SelectedSearchBy = "0";

				var itemAction = new SelectList(new[] { new { ID = "1", Name = "ALL APPLICATION" },
					 new { ID = "2", Name = "APPLICATION REGISTERED" },
					  new { ID = "3", Name = "FEE PAID  " },
					   new { ID = "4", Name = "FORM UNLOCKED" },
					new { ID = "5", Name = "CLOSED APPLICATION" },
				 new { ID = "6", Name = "Pending In School Master" },}, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";


				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;

				ViewBag.SelectedAppType = AppType;
				TempData["SelAppType"] = AppType;
				int SelAction = 0;
				Search = "APPNO is not null ";
				//Search += " and  isnull(ApprovalStatus,'')=''";  // blank

				if (AppType == "AFF")
				{
					if (frm["searchstatus"] != "")
					{
						ViewBag.SelectedStatus = frm["searchstatus"];
						if (ViewBag.SelectedStatus != "0")
						{
							Search += " and isnull(ApprovalStatus,'0')='" + ViewBag.SelectedStatus + "'";

						}
						//TempData["searchstatus"] = frm["searchstatus"];
					}

					if (frm["AppCls"] != "")
					{
						ViewBag.SelectedCls = frm["AppCls"];
						TempData["SelectedCls"] = frm["AppCls"];
						Search += " and ClassLevel='" + frm["AppCls"].ToString() + "'";
					}
					if (frm["Dist1"] != "")
					{
						ViewBag.SelectedDist = frm["Dist1"];
						TempData["SelectedDist"] = frm["Dist1"];
						Search += " and Dist='" + frm["Dist1"].ToString() + "'";
					}

					if (frm["SelAction"] != "")
					{
						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						SelAction = Convert.ToInt32(frm["SelAction"].ToString());
						if (SelAction == 2)//Registered
						{
							Search += " and  HistoryStatus='APPLICATION REGISTERED'";
						}
						else if (SelAction == 3)//Registered
						{
							Search += " and  HistoryStatus='FEE PAID'";
						}
						else if (SelAction == 4)//FORM UNLOCKED
						{
							Search += " and IsFormLock='0'";
						}
						else if (SelAction == 5)
						{
							Search += " and  CurrentApplicationStatus=8";
						}
						else if (SelAction == 6)
						{ Search += "  and  FinalStatus='Fee Verified' and isnull(SchlMasterStatus,'')='' "; }
					}

					if (frm["Sch1"] != "")
					{
						ViewBag.SelectedItem = frm["Sch1"];
						TempData["SelectedItem"] = frm["Sch1"];
						int SelValueSch = Convert.ToInt32(frm["Sch1"].ToString());

						if (frm["SearchString"] != "")
						{
							if (SelValueSch == 1)
							{ Search += " and APPNO=" + frm["SearchString"].ToString(); }
							else if (SelValueSch == 2)
							{ Search += " and  UDISECODE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  SCHLNME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and  SCHLMOBILE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  SCHL='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 7)
							{ Search += " and  EAffType='" + frm["SearchString"].ToString() + "'"; }

						}
					}


					//
					if (DistAllow != "")
					{
						if (AdminType.ToUpper() != "ADMIN".ToUpper() && !AdminType.ToUpper().Contains("INSPECTION"))
						{
							Search += " and DIST in (" + DistAllow + ") ";
						}
					}
					if (AdminType.ToUpper() != "ADMIN".ToUpper())
					{
						Search += " and upper(CurrentApplicationBranch)='" + AdminType + "'";
					}
					//
					spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser, Search, "", pageIndex, 11); // 11 - INBOX
					int VerifyStatus = Convert.ToInt32(spi.StoreAllData.Tables[0].Rows[0]["Verified"]);

					if (VerifyStatus == 1)
					{
						ViewBag.IsHistory = 1;
					}
					else
					{
						ViewBag.IsHistory = null;
					}


				}

				else if (AppType == "AC")
				{
					if (frm["searchstatus"] != "")
					{
						ViewBag.SelectedStatus = frm["searchstatus"];
						if (ViewBag.SelectedStatus != "0")
						{
							Search += " and isnull(ApprovalStatus,'0')='" + ViewBag.SelectedStatus + "'";

						}
						//TempData["searchstatus"] = frm["searchstatus"];
					}


					if (frm["Dist1"] != "")
					{
						ViewBag.SelectedDist = frm["Dist1"];
						TempData["SelectedDist"] = frm["Dist1"];
						Search += " and Dist='" + frm["Dist1"].ToString() + "'";
					}

					if (frm["SelAction"] != "")
					{
						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						SelAction = Convert.ToInt32(frm["SelAction"].ToString());
						if (SelAction == 2)//Registered
						{
							Search += "  and  ID is not null  ";
						}
						else if (SelAction == 3)//Fees Paid
						{
							Search += "  and  IsFinalSubmit='FINAL'  ";
						}
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
							{ Search += " and  UDISECODE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  SCHLNME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and  MOBILE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  SCHL='" + frm["SearchString"].ToString() + "'"; }
							//else if (SelValueSch == 7)
							//{ Search += " and  EAffType='" + frm["SearchString"].ToString() + "'"; }

						}
					}
					if (DistAllow != "")
					{
						if (AdminType.ToUpper() != "ADMIN".ToUpper() && !AdminType.ToUpper().Contains("INSPECTION"))
						{
							Search += " and DIST in (" + DistAllow + ") ";
						}
					}
					if (AdminType.ToUpper() != "ADMIN".ToUpper())
					{
						Search += " and upper(CurrentApplicationBranch)='" + AdminType + "'";
					}
					spi.StoreAllData = new AbstractLayer.AffiliationDB().AffiliationContinuationList(AdminUser, Search, "", pageIndex, 11);
					int VerifyStatus = Convert.ToInt32(spi.StoreAllData.Tables[0].Rows[0]["ChallanVerify"]);

					if (VerifyStatus == 1)
					{
						ViewBag.IsHistory = 1;
					}
					else
					{
						ViewBag.IsHistory = null;
					}

				}
				else if (AppType == "AS")
				{
					if (frm["searchstatus"] != "")
					{
						ViewBag.SelectedStatus = frm["searchstatus"];
						if (ViewBag.SelectedStatus != "0")
						{
							Search += " and isnull(ApprovalStatus,'0')='" + ViewBag.SelectedStatus + "'";

						}
						//TempData["searchstatus"] = frm["searchstatus"];
					}

					if (frm["AppCls"] != "")
					{
						ViewBag.SelectedCls = frm["AppCls"];
						TempData["SelectedCls"] = frm["AppCls"];
						Search += " and ClassLevel='" + frm["AppCls"].ToString() + "'";
					}
					if (frm["Dist1"] != "")
					{
						ViewBag.SelectedDist = frm["Dist1"];
						TempData["SelectedDist"] = frm["Dist1"];
						Search += " and Dist='" + frm["Dist1"].ToString() + "'";
					}

					if (frm["SelAction"] != "")
					{
						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						SelAction = Convert.ToInt32(frm["SelAction"].ToString());
						if (SelAction == 2)//Registered
						{
							Search += "  and  ID is not null  ";
						}
						else if (SelAction == 3)//Fees Paid
						{
							Search += "  and  IsFinalSubmit='FINAL'  ";
						}
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
							{ Search += " and  UDISECODE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  SCHLNME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and  MOBILE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  SCHL='" + frm["SearchString"].ToString() + "'"; }
							//else if (SelValueSch == 7)
							//{ Search += " and  EAffType='" + frm["SearchString"].ToString() + "'"; }

						}
					}
					if (DistAllow != "")
					{
						if (AdminType.ToUpper() != "ADMIN".ToUpper() && !AdminType.ToUpper().Contains("INSPECTION"))
						{
							Search += " and DIST in (" + DistAllow + ") ";
						}
					}
					if (AdminType.ToUpper() != "ADMIN".ToUpper())
					{
						Search += " and upper(CurrentApplicationBranch)='" + AdminType + "'";
					}
					spi.StoreAllData = new AbstractLayer.AdditionalSectionDB().AdditionalSectionList(AdminUser, Search, "", pageIndex, 11);
					int VerifyStatus = Convert.ToInt32(spi.StoreAllData.Tables[0].Rows[0]["ChallanVerify"]);

					if (VerifyStatus == 1)
					{
						ViewBag.IsHistory = 1;
					}
					else
					{
						ViewBag.IsHistory = null;
					}

				}



				TempData["SearchInboxEAffiliation"] = Search;
				TempData.Keep(); // to store search value for view


				if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.LastPageIndex = 0;
					ViewBag.TotalCount = 0;
				}
				else
				{
					//int VerifyStatus = Convert.ToInt32(spi.StoreAllData.Tables[0].Rows[0]["verified"]);
					//if (VerifyStatus == 1)
					//{
					//    ViewBag.IsHistory = 1;
					//}
					//else
					//{
					//    ViewBag.IsHistory = null;
					//}
					ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
					int count = Convert.ToInt32(spi.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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

				return View(spi);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return View();
			}
		}




		[AdminLoginCheckFilter]
		public ActionResult SentEAffiliation(EAffiliationModel spi, int? page, string AppType)
		{
			try
			{
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string AdminType = Session["AdminType"].ToString().ToUpper();

				string AdminUser = Session["AdminUser"].ToString().ToUpper();
				List<EAffiliationApplicationStatusMaster> eAffiliationApplicationStatusMasterList = new AbstractLayer.EAffiliationDB().ForwardListByAppNoStatus(AppType, 1, "", AdminUser);
				ViewBag.MyForwardList = eAffiliationApplicationStatusMasterList;
				//AppType
				//AppType
				ViewBag.MyAppType = AbstractLayer.EAffiliationDB.GetApplicationTypeList().ToList();
				ViewBag.SelectedAppType = "0";

				string DistAllow = "";
				// Dist Allowed
				if (ViewBag.DistAllow == null)
				{ return RedirectToAction("Index", "Admin"); }
				else
				{
					DistAllow = ViewBag.DistAllow;
				}

				if (ViewBag.DistUser == null)
				{
					ViewBag.MyDist = null;
				}
				else
				{
					ViewBag.MyDist = ViewBag.DistUser; // User District
				}
				ViewBag.MyAppCls = new AbstractLayer.DBClass().GetAllClass().ToList();
				ViewBag.SelectedCls = "0";
				//Status
				var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "Application No" }, new { ID = "6", Name = "School Code" }, new { ID = "2", Name = "UDISE Code" },
				new { ID = "3", Name = "School Name" },new { ID = "4", Name = "Station Name" },new { ID = "5", Name = "Mobile No" },new { ID = "7", Name = "EAffiliation Type" },}, "ID", "Name", 1);
				ViewBag.MySch = itemSearchBy.ToList();
				ViewBag.SelectedSearchBy = "0";

				var itemAction = new SelectList(new[] { new { ID = "1", Name = "ALL APPLICATION" },
					 new { ID = "2", Name = "APPLICATION REGISTERED" },
					  new { ID = "3", Name = "FEE PAID" },
					   new { ID = "4", Name = "FORM UNLOCKED" },
					new { ID = "5", Name = "CLOSED APPLICATION" },
				 new { ID = "6", Name = "Pending In School Master" },}, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";

				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = "";

				if (TempData["SearchSentEAffiliation"] != null)
				{
					Search += TempData["SearchSentEAffiliation"].ToString();
					ViewBag.SelectedAction = TempData["SelAction"];
					ViewBag.SelectedItem = TempData["SelectedItem"];
					ViewBag.SelectedDist = TempData["SelectedDist"];
					ViewBag.SelectedAppType = TempData["SelAppType"];

					TempData["SelAppType"] = ViewBag.SelectedAppType;

					//
					if (DistAllow != "")
					{
						Search += " and DIST in (" + DistAllow + ") ";
					}
					//if (AdminType.ToUpper() != "ADMIN".ToUpper())
					//{
					//    Search += " and upper(SentApplicationBranch)='" + AdminType + "'";
					//}
					//

					if (ViewBag.SelectedAppType == "AFF")
					{
						spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser, Search, "", pageIndex, 13);
					}
					else if (ViewBag.SelectedAppType == "AC")
					{
						spi.StoreAllData = new AbstractLayer.AffiliationDB().AffiliationContinuationList(AdminUser, Search, "", pageIndex, 13);
					}
					else if (ViewBag.SelectedAppType == "AS")
					{
						spi.StoreAllData = new AbstractLayer.AdditionalSectionDB().AdditionalSectionList(AdminUser, Search, "", pageIndex, 13);
					}
					//spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser,Search, "", pageIndex, 11); // 11 -- INBOX

					if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.LastPageIndex = 0;
						ViewBag.TotalCount = 0;
					}
					else
					{
						TempData["SearchSentEAffiliation"] = Search;
						TempData.Keep();
						ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
						int count = Convert.ToInt32(spi.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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
				return View(spi);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}


		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult SentEAffiliation(EAffiliationModel spi, FormCollection frm, int? page, string AppType)
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string AdminType = Session["AdminType"].ToString().ToUpper();

				string AdminUser = Session["AdminUser"].ToString().ToUpper();
				List<EAffiliationApplicationStatusMaster> eAffiliationApplicationStatusMasterList = new AbstractLayer.EAffiliationDB().ForwardListByAppNoStatus(AppType, 1, "", AdminUser);
				ViewBag.MyForwardList = eAffiliationApplicationStatusMasterList;



				//AppType
				ViewBag.MyAppType = AbstractLayer.EAffiliationDB.GetApplicationTypeList().ToList();
				ViewBag.SelectedAppType = "0";

				string DistAllow = "";
				// Dist Allowed
				if (ViewBag.DistAllow == null)
				{ return RedirectToAction("Index", "Admin"); }
				else
				{
					DistAllow = ViewBag.DistAllow;
				}
				if (ViewBag.DistUser == null)
				{
					ViewBag.MyDist = null;

				}
				else
				{
					ViewBag.MyDist = ViewBag.DistUser; // User District
				}
				ViewBag.MyAppCls = new AbstractLayer.DBClass().GetAllClass().ToList();
				ViewBag.SelectedCls = "0";
				//Status
				var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "Application No" }, new { ID = "6", Name = "School Code" }, new { ID = "2", Name = "UDISE Code" },
				new { ID = "3", Name = "School Name" },new { ID = "4", Name = "Station Name" },new { ID = "5", Name = "Mobile No" },new { ID = "7", Name = "EAffiliation Type" },}, "ID", "Name", 1);
				ViewBag.MySch = itemSearchBy.ToList();
				ViewBag.SelectedSearchBy = "0";

				var itemAction = new SelectList(new[] { new { ID = "1", Name = "ALL APPLICATION" },
					 new { ID = "2", Name = "APPLICATION REGISTERED" },
					  new { ID = "3", Name = "FEE PAID  " },
					   new { ID = "4", Name = "FORM UNLOCKED" },
					new { ID = "5", Name = "CLOSED APPLICATION" },
				 new { ID = "6", Name = "Pending In School Master" },}, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";


				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;
				ViewBag.SelectedAppType = AppType;
				TempData["SelAppType"] = AppType;
				int SelAction = 0;
				Search = "APPNO is not null ";

				if (AppType == "AFF")
				{


					if (frm["AppCls"] != "")
					{
						ViewBag.SelectedCls = frm["AppCls"];
						TempData["SelectedCls"] = frm["AppCls"];
						Search += " and ClassLevel='" + frm["AppCls"].ToString() + "'";
					}
					if (frm["Dist1"] != "")
					{
						ViewBag.SelectedDist = frm["Dist1"];
						TempData["SelectedDist"] = frm["Dist1"];
						Search += " and Dist='" + frm["Dist1"].ToString() + "'";
					}

					if (frm["SelAction"] != "")
					{
						ViewBag.SelectedAction = frm["SelAction"];
						SelAction = Convert.ToInt32(frm["SelAction"].ToString());
						if (SelAction == 2)//Registered
						{
							Search += " and  HistoryStatus='APPLICATION REGISTERED'";
						}
						else if (SelAction == 3)//Registered
						{
							Search += " and  HistoryStatus='FEE PAID'";
						}
						else if (SelAction == 4)//FORM UNLOCKED
						{
							Search += " and IsFormLock='0'";
						}
						else if (SelAction == 5)
						{
							Search += " and  CurrentApplicationStatus=8";
						}
						else if (SelAction == 6)
						{ Search += "  and  FinalStatus='Fee Verified' and isnull(SchlMasterStatus,'')='' "; }
					}

					if (frm["Sch1"] != "")
					{
						ViewBag.SelectedItem = frm["Sch1"];
						TempData["SelectedItem"] = frm["Sch1"];
						int SelValueSch = Convert.ToInt32(frm["Sch1"].ToString());

						if (frm["SearchString"] != "")
						{
							if (SelValueSch == 1)
							{ Search += " and APPNO=" + frm["SearchString"].ToString(); }
							else if (SelValueSch == 2)
							{ Search += " and  UDISECODE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  SCHLNME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and  SCHLMOBILE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  SCHL='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 7)
							{ Search += " and  EAffType='" + frm["SearchString"].ToString() + "'"; }

						}
					}

					if (DistAllow != "")
					{
						if (AdminType.ToUpper() != "ADMIN".ToUpper() && !AdminType.ToUpper().Contains("INSPECTION"))
						{
							Search += " and DIST in (" + DistAllow + ") ";
						}
					}
					if (AdminType.ToUpper() != "ADMIN".ToUpper())
					{
						Search += " and upper(SentApplicationBranch)='" + AdminType + "'";
					}
					spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser, Search, "", pageIndex, 11); // 11 - INBOX

				}

				else if (AppType == "AC")
				{

					if (frm["Dist1"] != "")
					{
						ViewBag.SelectedDist = frm["Dist1"];
						TempData["SelectedDist"] = frm["Dist1"];
						Search += " and Dist='" + frm["Dist1"].ToString() + "'";
					}

					if (frm["SelAction"] != "")
					{
						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						SelAction = Convert.ToInt32(frm["SelAction"].ToString());
						if (SelAction == 2)//Registered
						{
							Search += "  and  ID is not null  ";
						}
						else if (SelAction == 3)//Fees Paid
						{
							Search += "  and  IsFinalSubmit='FINAL'  ";
						}
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
							{ Search += " and  UDISECODE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  SCHLNME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and  MOBILE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  SCHL='" + frm["SearchString"].ToString() + "'"; }
							//else if (SelValueSch == 7)
							//{ Search += " and  EAffType='" + frm["SearchString"].ToString() + "'"; }

						}
					}
					if (DistAllow != "")
					{
						if (AdminType.ToUpper() != "ADMIN".ToUpper() && !AdminType.ToUpper().Contains("INSPECTION"))
						{
							Search += " and DIST in (" + DistAllow + ") ";
						}
					}
					//if (AdminType.ToUpper() != "ADMIN".ToUpper())
					//{
					//    Search += " and upper(SentApplicationBranch)='" + AdminType + "'";
					//}
					spi.StoreAllData = new AbstractLayer.AffiliationDB().AffiliationContinuationList(AdminUser, Search, "", pageIndex, 13);

				}
				else if (AppType == "AS")
				{
					if (frm["AppCls"] != "")
					{
						ViewBag.SelectedCls = frm["AppCls"];
						TempData["SelectedCls"] = frm["AppCls"];
						Search += " and ClassLevel='" + frm["AppCls"].ToString() + "'";
					}
					if (frm["Dist1"] != "")
					{
						ViewBag.SelectedDist = frm["Dist1"];
						TempData["SelectedDist"] = frm["Dist1"];
						Search += " and Dist='" + frm["Dist1"].ToString() + "'";
					}

					if (frm["SelAction"] != "")
					{
						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						SelAction = Convert.ToInt32(frm["SelAction"].ToString());
						if (SelAction == 2)//Registered
						{
							Search += "  and  ID is not null  ";
						}
						else if (SelAction == 3)//Fees Paid
						{
							Search += "  and  IsFinalSubmit='FINAL'  ";
						}
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
							{ Search += " and  UDISECODE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  SCHLNME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and  MOBILE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  SCHL='" + frm["SearchString"].ToString() + "'"; }
							//else if (SelValueSch == 7)
							//{ Search += " and  EAffType='" + frm["SearchString"].ToString() + "'"; }

						}
					}
					if (DistAllow != "")
					{
						if (AdminType.ToUpper() != "ADMIN".ToUpper() && !AdminType.ToUpper().Contains("INSPECTION"))
						{
							Search += " and DIST in (" + DistAllow + ") ";
						}
					}
					//if (AdminType.ToUpper() != "ADMIN".ToUpper())
					//{
					//    Search += " and upper(SentApplicationBranch)='" + AdminType + "'";
					//}
					spi.StoreAllData = new AbstractLayer.AdditionalSectionDB().AdditionalSectionList(AdminUser, Search, "", pageIndex, 13);

				}



				TempData["SearchSentEAffiliation"] = Search;
				TempData.Keep(); // to store search value for view

				if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.LastPageIndex = 0;
					ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
					int count = Convert.ToInt32(spi.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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

				return View(spi);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return View();
			}
		}


		public JsonResult JqEAffiliationForwardStatus(string storeid, string Remarks, string ApplicationStatus, string ApplicationType)
		{

			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			int userid = Convert.ToInt32(Session["AdminId"].ToString());
			string ForwardList = storeid;
			int cnt = ForwardList.Count(x => x == ',');
			string OutError = "0";
			string result = new AbstractLayer.EAffiliationDB().EAffiliationForward(adminLoginSession.AdminEmployeeUserId, ApplicationType, ForwardList, ApplicationStatus, Remarks, Convert.ToInt32(Session["AdminId"]), out OutError);
			if (OutError == "1")
			{
				ViewData["ForwardResult"] = "1";
			}
			else
			{ ViewData["ForwardResult"] = "0"; }

			return Json(new { dee = OutError }, JsonRequestBehavior.AllowGet);
		}



		[HttpPost]
		public ActionResult EAffiliation_ObjectionLetter(HttpPostedFileBase ObjectionLetter, FormCollection frm)
		{
			if (Session["AdminId"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}
			string AdminType = Session["AdminType"].ToString().ToUpper();
			string OutError = "0";
			if (!string.IsNullOrEmpty(frm["APPNO"].ToString()) && frm["RemarksObjection"] != null)
			{
				string APPNO = frm["APPNO"].ToString();
				string fileName = "";
				string filepath = "";
				if (ObjectionLetter != null && ObjectionLetter.ContentLength > 0 && !string.IsNullOrEmpty(ObjectionLetter.FileName))
				{

					string fileExt = Path.GetExtension(ObjectionLetter.FileName);
					fileName = APPNO + "_" + DateTime.Now.Date.ToString("ddMMyyyyhhmm") + fileExt;
					filepath = "Upload/Upload2023/EAffiliation2021/ObjectionLetter/" + fileName;
				}
				else
				{
					filepath = fileName = "";
				}



				EAffiliationModel _EAffiliationModel = new EAffiliationModel();
				_EAffiliationModel.UpdatedBy = Convert.ToInt32(Session["AdminId"]);
				_EAffiliationModel.APPNO = APPNO;
				_EAffiliationModel.Remarks = frm["RemarksObjection"].ToString();
				_EAffiliationModel.ObjectionLetter = filepath;
				_EAffiliationModel.SCHLMOBILE = frm["SCHLMOBILE"].ToString();



				//21 for objection
				int result = new AbstractLayer.EAffiliationDB().EAffiliation(_EAffiliationModel, 21, out OutError);
				if (OutError == "1")
				{
					if (!string.IsNullOrEmpty(frm["APPNO"].ToString()))
					{
						if (ObjectionLetter != null && ObjectionLetter.ContentLength > 0 && !string.IsNullOrEmpty(ObjectionLetter.FileName))
						{
							//var path = Path.Combine(Server.MapPath("~/Upload/"+ formName + "/" + dist + "/Photo"), stdPic);
							var path = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/EAffiliation2021/ObjectionLetter"), fileName);
							string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/EAffiliation2021/ObjectionLetter"));
							if (!Directory.Exists(FilepathExist))
							{
								Directory.CreateDirectory(FilepathExist);
							}
							ObjectionLetter.SaveAs(path);
						}
						//Objection raised against your application (dairy no 101). Visit duplicate section under board's website to track status/Download objection letter
						string Sms = "Objection raised against your EAffiliation application no " + _EAffiliationModel.APPNO + " successfully.";
						try
						{
							string getSms = objCommon.gosms(_EAffiliationModel.SCHLMOBILE, Sms);
							//string getSms = objCommon.gosms("9711819184", Sms);
						}
						catch (Exception) { }

					}


					return RedirectToAction("InboxEAffiliation", "Admin");
				}


			}
			return RedirectToAction("InboxEAffiliation", "Admin");
		}


		[HttpPost]
		public ActionResult EAffiliation_FillInspectionReport(HttpPostedFileBase InspectionReport, FormCollection frm)
		{
			if (Session["AdminId"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}


			string AdminType = Session["AdminType"].ToString().ToUpper();
			string OutError = "0";
			if (!string.IsNullOrEmpty(frm["APPNO"].ToString()))
			{
				if (InspectionReport != null && InspectionReport.ContentLength > 0 && !string.IsNullOrEmpty(InspectionReport.FileName))
				{
					string Remarks = frm["RemarksInspectionReport"].ToString();
					string APPNO = frm["APPNO"].ToString();
					string fileName = "";
					string fileExt = Path.GetExtension(InspectionReport.FileName);
					fileName = APPNO + "_" + DateTime.Now.Date.ToString("ddMMyyyyhhmm") + fileExt;
					string filepath = "Upload/Upload2023/EAffiliation2021/InspectionReport/" + fileName;
					if (string.IsNullOrEmpty(Remarks))
					{
						Remarks = "Inspection Report Submitted by Committee";
					}


					EAffiliationModel _EAffiliationModel = new EAffiliationModel();
					_EAffiliationModel.UpdatedBy = Convert.ToInt32(Session["AdminId"]);
					_EAffiliationModel.APPNO = APPNO;
					_EAffiliationModel.Remarks = Remarks;
					_EAffiliationModel.InspectionReport = filepath;
					_EAffiliationModel.SCHLMOBILE = frm["SCHLMOBILE"].ToString();



					//21 for objection
					int result = new AbstractLayer.EAffiliationDB().EAffiliation(_EAffiliationModel, 22, out OutError);
					if (OutError == "1")
					{
						TempData["result"] = "10";
						if (!string.IsNullOrEmpty(frm["APPNO"].ToString()))
						{
							var path = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/EAffiliation2021/InspectionReport"), fileName);
							string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/EAffiliation2021/InspectionReport"));
							if (!Directory.Exists(FilepathExist))
							{
								Directory.CreateDirectory(FilepathExist);
							}
							InspectionReport.SaveAs(path);
							//Objection raised against your application (dairy no 101). Visit duplicate section under board's website to track status/Download objection letter
							//string Sms = "Inspection Report raised against your EAffiliation application no " + _EAffiliationModel.APPNO + " successfully.";
							//try
							//{
							//    // string getSms = objCommon.gosms(_EAffiliationModel.SCHLMOBILE, Sms);
							//    //string getSms = objCommon.gosms("9711819184", Sms);
							//}
							//catch (Exception) { }

						}


						return RedirectToAction("InboxEAffiliation", "Admin");
					}

				}
			}
			return RedirectToAction("InboxEAffiliation", "Admin");
		}



		[AdminLoginCheckFilter]
		public ActionResult FillEAffiliationInspectionReport(string id, EAffiliationModel spi)
		{

			int AdminId = Convert.ToInt32(Session["AdminId"]);
			string AdminType = Session["AdminType"].ToString().ToUpper();

			try
			{
				if (string.IsNullOrEmpty(id))
				{
					return RedirectToAction("InboxEAffiliation", "Admin");
				}

			}
			catch (Exception ex)
			{
			}
			return View(spi);

		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult FillEAffiliationInspectionReport(string id, EAffiliationModel spi, FormCollection fc)
		{

			int AdminId = Convert.ToInt32(Session["AdminId"]);
			string AdminType = Session["AdminType"].ToString().ToUpper();

			try
			{
				if (string.IsNullOrEmpty(id))
				{
					return RedirectToAction("InboxEAffiliation", "Admin");
				}
			}
			catch (Exception ex)
			{
			}
			return View(spi);
		}


		[AdminLoginCheckFilter]
		public ActionResult ViewEAffiliationInspectionReport(string id, EAffiliationModel spi)
		{

			int AdminId = Convert.ToInt32(Session["AdminId"]);
			string AdminType = Session["AdminType"].ToString().ToUpper();

			try
			{
				if (string.IsNullOrEmpty(id))
				{
					return RedirectToAction("InboxEAffiliation", "Admin");
				}
			}
			catch (Exception ex)
			{
			}
			return View(spi);

		}

		[AdminLoginCheckFilter]
		public ActionResult ViewEAffiliationHistory(string id, string AppType, EAffiliationModel spi)
		{

			int AdminId = Convert.ToInt32(Session["AdminId"]);
			string AdminType = Session["AdminType"].ToString().ToUpper();
			string AdminUser = Session["AdminUser"].ToString().ToUpper();

			ViewBag.AppType = AbstractLayer.EAffiliationDB.GetApplicationTypeList().Where(s => s.Value.ToUpper() == AppType.ToUpper()).Select(s => s.Text).FirstOrDefault();
			ViewBag.APPNO = id.ToString();

			try
			{
				if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(AppType))
				{
					return RedirectToAction("InboxEAffiliation", "Admin");
				}
				else
				{

					string Search = " APPNO = '" + id + "'";
					if (AppType == "AFF")
					{
						spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser, Search, "", 1, 12); // 12- History by appno

					}
					else if (AppType == "AC")
					{
						spi.StoreAllData = new AbstractLayer.AffiliationDB().AffiliationContinuationList(AdminUser, Search, "", 1, 12); // 12- History by appno

					}
					else if (AppType == "AS")
					{
						spi.StoreAllData = new AbstractLayer.AdditionalSectionDB().AdditionalSectionList(AdminUser, Search, "", 1, 12); // 12- History by appno
					}

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



		[AdminLoginCheckFilter]
		public ActionResult AffiliationObjectionLetter(string id, string AppType, AffObjectionLettersViewModel spi)
		{

			int AdminId = Convert.ToInt32(Session["AdminId"]);
			string AdminType = Session["AdminType"].ToString().ToUpper();
			string AdminUser = Session["AdminUser"].ToString().ToUpper();
			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(AppType))
			{
				return RedirectToAction("InboxEAffiliation", "Admin");
			}
			spi.AffObjectionLetters = new AffObjectionLetters();

			spi.AffObjectionLetters.AppNo = id;
			spi.AffObjectionLetters.AppType = AppType;
			List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDL();
			ViewBag.MyApprovalStatusList = MyAcceptRejectList;


			spi.affObjectionListMastersList = new AbstractLayer.AdminDB().GetAffObjectionListMasters().ToList();
			try
			{
				string Search = " APPNO = '" + id + "'";
				if (AppType == "AFF")
				{
					spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser, Search, "", 1, 15); // 12- Objection by appno
				}
				else if (AppType == "AC")
				{
					spi.StoreAllData = new AbstractLayer.AffiliationDB().AffiliationContinuationList(AdminUser, Search, "", 1, 15); // 12- Objection by appno
				}
				else if (AppType == "AS")
				{
					spi.StoreAllData = new AbstractLayer.AdditionalSectionDB().AdditionalSectionList(AdminUser, Search, "", 1, 15); // 12- Objection by appno
				}

				if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
				}
			}
			catch (Exception ex)
			{
			}
			return View(spi);

		}


		[HttpPost]
		[AdminLoginCheckFilter]
		public ActionResult AffiliationObjectionLetter(string id, string AppType, AffObjectionLettersViewModel spi, string cmd,
			string OtherDocumentName, FormCollection frm, HttpPostedFileBase ObjectionFile)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			int AdminId = Convert.ToInt32(Session["AdminId"]);
			string AdminType = Session["AdminType"].ToString().ToUpper();
			string AdminUser = Session["AdminUser"].ToString().ToUpper();
			if (spi.AffObjectionLetters.AppNo != id)
			{
				return RedirectToAction("InboxEAffiliation", "Admin");
			}

			List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDL();
			ViewBag.MyApprovalStatusList = MyAcceptRejectList;

			id = spi.AffObjectionLetters.AppNo;
			AppType = spi.AffObjectionLetters.AppType;
			spi.affObjectionListMastersList = new AbstractLayer.AdminDB().GetAffObjectionListMasters().ToList();
			try
			{
				if (spi.selObjCode.Count() > 0)
				{
					//  string[] ObjCodeSplit = frm["AffObjectionLetters.ObjCode"].Split(',');

					foreach (string objcode in spi.selObjCode)
					{
						string DocName = objcode;
						//string DocName = spi.AffObjectionLetters.ObjCode;
						if (!string.IsNullOrEmpty(OtherDocumentName) && objcode == "aff_999")
						{
							DocName = "OtherDocument_" + OtherDocumentName;
							spi.AffObjectionLetters.Objection = OtherDocumentName;
						}

						if (!string.IsNullOrEmpty(cmd))
						{

							// Save file
							string filename = "";
							string FilepathExist = "", path = "";
							string exactPath = "~/Upload/Upload2023/AffObjecttonLetter/" + spi.AffObjectionLetters.AppType;
							if (ObjectionFile != null)
							{
								//Upload/AdditionalSection
								string ext = Path.GetExtension(ObjectionFile.FileName);
								filename = spi.AffObjectionLetters.AppNo + "_" + DocName.Replace(" ", "_") + ext;
								path = Path.Combine(Server.MapPath(exactPath), filename.ToUpper());
								FilepathExist = Path.Combine(Server.MapPath(exactPath));
								spi.AffObjectionLetters.ObjectionFile = "allfiles/Upload2023/AFFObjecttonLetter" + spi.AffObjectionLetters.AppType + "/" + filename.ToUpper();
							}

							if (cmd.ToLower() == "submit" || cmd.ToLower() == "save")
							{
								AffObjectionLetters affObjectionLettersData = new AffObjectionLetters()
								{

									AppType = spi.AffObjectionLetters.AppType,
									AppNo = spi.AffObjectionLetters.AppNo,
									ObjCode = objcode,
									Objection = spi.AffObjectionLetters.Objection,
									ObjectionFile = spi.AffObjectionLetters.ObjectionFile,
									ObjDate = DateTime.Now,
									ObjStatus = spi.AffObjectionLetters.ObjStatus,
									SubmitBy = AdminUser,
									ClearDate = DateTime.Now,
									Attachment = spi.AffObjectionLetters.Attachment,
									SchoolReply = spi.AffObjectionLetters.SchoolReply,
									SchoolReplyOn = spi.AffObjectionLetters.SchoolReplyOn,
									EmpUserId = adminLoginSession.AdminEmployeeUserId,

								};
								_context.AffObjectionLetters.Add(affObjectionLettersData);
								int insertedRecords = _context.SaveChanges();

								if (insertedRecords > 0)
								{
									ViewData["result"] = "1";
									if (ObjectionFile != null)
									{
										if (!Directory.Exists(FilepathExist))
										{
											Directory.CreateDirectory(FilepathExist);
										}

										string Orgfile = filename;

										using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
										{
											using (var newMemoryStream = new MemoryStream())
											{
												var uploadRequest = new TransferUtilityUploadRequest
												{
													InputStream = ObjectionFile.InputStream,
													Key = string.Format("allfiles/Upload2023/AFFObjecttonLetter/" + spi.AffObjectionLetters.AppType + "/{0}", Orgfile),

													BucketName = BUCKET_NAME,
													CannedACL = S3CannedACL.PublicRead
												};

												var fileTransferUtility = new TransferUtility(client);
												fileTransferUtility.Upload(uploadRequest);
											}
										}



										//ObjectionFile.SaveAs(path);
									}
								}
								else
								{
									ViewData["result"] = "2";
								}
							}
						}

					}

				}

			}
			catch (Exception ex)
			{
				ViewData["result"] = "ERR";
				ViewBag.Mesaage = ex.Message;
			}
			return View(spi);

		}


		[AdminLoginCheckFilter]
		public ActionResult EaffiliationCombinedDownloadDocuments(string id, string AppType, EAffiliationModel spi)
		{
			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(AppType))
			{
				return RedirectToAction("InboxEAffiliation", "Admin");
			}
			int AdminId = Convert.ToInt32(Session["AdminId"]);
			string AdminType = Session["AdminType"].ToString().ToUpper();
			string AdminUser = Session["AdminUser"].ToString().ToUpper();

			ViewBag.AppType = AbstractLayer.EAffiliationDB.GetApplicationTypeList().Where(s => s.Value.ToUpper() == AppType.ToUpper()).Select(s => s.Text).FirstOrDefault();
			ViewBag.APPNO = id.ToString();
			string combinedDocumentPath = "";
			try
			{
				string Search = " APPNO = '" + id + "'";
				spi.StoreAllData = new AbstractLayer.EAffiliationDB().GetAllApprovedeDocumentAgainstEaffiliationApplication(AppType, id.ToString()); // 12- History by appno
				if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
				}
				else
				{
					combinedDocumentPath = spi.StoreAllData.Tables[0].Rows[0]["CombinedDocumentPath"].ToString();
					if (!string.IsNullOrEmpty(combinedDocumentPath))
					{
						string AppTypeNM = ViewBag.AppType.Replace(" ", "_");
						string exactPath = "~/Upload/Upload2023/CombinedDocuments/" + AppTypeNM;
						string filename = AppTypeNM + "_" + id.ToString() + ".pdf";
						string downloadPath = Path.Combine(Server.MapPath(exactPath), filename.ToUpper());
						string path = Path.Combine(Server.MapPath(exactPath), filename.ToUpper());
						string FilepathExist = Path.Combine(Server.MapPath(exactPath));
						if (!Directory.Exists(FilepathExist))
						{
							Directory.CreateDirectory(FilepathExist);
						}



						// Save Merge File to Specific Path 
						// String[] files = @"C:\ENROLLDOCS\A1.pdf,C:\ENROLLDOCS\A2.pdf".Split(',');
						string destinationFile = path;
						String[] sourceFiles = combinedDocumentPath.Split(',');
						//for (int i = 0; i < sourceFiles.Length; i++)
						//{
						//    sourceFiles[i] = Path.Combine(Server.MapPath("~/Upload"), sourceFiles[i]);
						//}


						AbstractLayer.iTextSharpService.MergeFiles(destinationFile, sourceFiles);


						#region  Download File
						using (var req = new WebClient())
						{
							string saveFileName = DateTime.Now.ToString("ddMMyyyy") + "_" + filename;
							try
							{
								Response.Clear();
								Response.ClearContent();
								Response.ClearHeaders();
								Response.Buffer = true;
								Response.AddHeader("Content-Disposition", "attachment;filename=\"" + saveFileName + "\"");
								//if (!Directory.Exists(downloadPath))
								//{
								//	Directory.CreateDirectory(downloadPath);
								//}
								byte[] data = req.DownloadData(downloadPath);
								Response.BinaryWrite(data);
								Response.End();
							}
							catch (Exception ex1)
							{

							}
						}
						#endregion


					}
				}
			}
			catch (Exception ex)
			{
				//CustomExceptionHandlerFilter:FilterAttribute
				var st = new StackTrace(ex, true);
				// Get the top stack frame
				var frame = st.GetFrame(0);
				// Get the line number from the stack frame
				var line = frame.GetFileLineNumber();
				AbstractLayer.StaticDB.AddExceptionInDB(ex, "Admin", "EaffiliationCombinedDownloadDocuments", line.ToString(), "", "EaffiliationCombinedDownloadDocuments", "EXPT inner");

			}
			return RedirectToAction("InboxEAffiliation", "Admin");

		}




		[AdminLoginCheckFilter]
		public ActionResult ViewAllEAffiliationObjections(EAffiliationModel spi, int? page, string AppType)
		{
			try
			{
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string AdminType = Session["AdminType"].ToString().ToUpper();
				string AdminUser = Session["AdminUser"].ToString().ToUpper();

				List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDL();
				ViewBag.MyApprovalStatusList = MyAcceptRejectList;

				string DistAllow = "";
				// Dist Allowed
				if (ViewBag.DistAllow == null)
				{ return RedirectToAction("Index", "Admin"); }
				else
				{
					DistAllow = ViewBag.DistAllow;
				}

				if (ViewBag.DistUser == null)
				{
					ViewBag.MyDist = null;
				}
				else
				{
					ViewBag.MyDist = ViewBag.DistUser; // User District

				}


				ViewBag.MyAppCls = new AbstractLayer.DBClass().GetAllClass().ToList();
				ViewBag.SelectedCls = "0";

				//AppType
				ViewBag.MyAppType = AbstractLayer.EAffiliationDB.GetApplicationTypeList().ToList();
				ViewBag.SelectedAppType = "0";



				//Status
				var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "Application No" }, new { ID = "2", Name = "School Code" }, new { ID = "3", Name = "ID NO" },
				  new { ID = "4", Name = "Obj ID" },}, "ID", "Name", 1);
				ViewBag.MySch = itemSearchBy.ToList();
				ViewBag.SelectedSearchBy = "0";


				var itemAction = new SelectList(new[] { new { ID = "1", Name = "ALL" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Received" }, new { ID = "4", Name = "Completed " }, }, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";


				//ViewBag.MyForwardList

				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = "";

				if (TempData["SearchViewAllEAffiliationObjections"] != null)
				{
					Search += TempData["SearchViewAllEAffiliationObjections"].ToString();
					ViewBag.SelectedAction = TempData["SelAction"];
					//  ViewBag.SelectedItem = TempData["SelectedItem"];
					// ViewBag.SelectedDist = TempData["SelectedDist"];
					ViewBag.SelectedAppType = TempData["SelAppType"];


					TempData["SelAppType"] = ViewBag.SelectedAppType;
					//
					if (DistAllow != "")
					{
						Search += " and DIST in (" + DistAllow + ") ";
					}
					//if (AdminType.ToUpper() != "ADMIN".ToUpper())
					//{
					//    Search += " and upper(CurrentApplicationBranch)='" + AdminType + "'";
					//}
					//


					if (AppType == "AFF")
					{
						spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser, Search, "", 1, 15); // 12- Objection by appno
					}
					else if (AppType == "AC")
					{
						spi.StoreAllData = new AbstractLayer.AffiliationDB().AffiliationContinuationList(AdminUser, Search, "", 1, 15); // 12- Objection by appno
					}
					else if (AppType == "AS")
					{
						spi.StoreAllData = new AbstractLayer.AdditionalSectionDB().AdditionalSectionList(AdminUser, Search, "", 1, 15); // 12- Objection by appno
					}
					TempData["SearchViewAllEAffiliationObjections"] = Search;
					TempData.Keep();
					if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.LastPageIndex = 0;
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{

						ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
					}
				}
				return View(spi);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}


		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult ViewAllEAffiliationObjections(EAffiliationModel spi, FormCollection frm, int? page, string AppType)
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string AdminType = Session["AdminType"].ToString().ToUpper();
				List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDL();
				ViewBag.MyApprovalStatusList = MyAcceptRejectList;
				string DistAllow = "";
				// Dist Allowed
				if (ViewBag.DistAllow == null)
				{ return RedirectToAction("Index", "Admin"); }
				else
				{
					DistAllow = ViewBag.DistAllow;
				}
				if (ViewBag.DistUser == null)
				{
					ViewBag.MyDist = null;

				}
				else
				{
					ViewBag.MyDist = ViewBag.DistUser; // User District
				}

				ViewBag.MyAppCls = new AbstractLayer.DBClass().GetAllClass().ToList();
				ViewBag.SelectedCls = "0";
				string AdminUser = Session["AdminUser"].ToString().ToUpper();


				//List<EAffiliationApplicationStatusMaster> eAffiliationApplicationStatusMasterList = new AbstractLayer.EAffiliationDB().ForwardListByAppNoStatus(AppType, 1, "", AdminUser);
				//ViewBag.MyForwardList = eAffiliationApplicationStatusMasterList;

				//AppType
				ViewBag.MyAppType = AbstractLayer.EAffiliationDB.GetApplicationTypeList().ToList();
				ViewBag.SelectedAppType = "0";
				//Status
				var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "Application No" }, new { ID = "2", Name = "School Code" }, new { ID = "3", Name = "ID NO" },
				  new { ID = "4", Name = "Obj ID" },}, "ID", "Name", 1);
				ViewBag.MySch = itemSearchBy.ToList();
				ViewBag.SelectedSearchBy = "0";

				var itemAction = new SelectList(new[] { new { ID = "1", Name = "ALL" }, new { ID = "2", Name = "Pending" }, new { ID = "3", Name = "Received" }, new { ID = "4", Name = "Completed " }, }, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";


				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;

				ViewBag.SelectedAppType = AppType;
				TempData["SelAppType"] = AppType;
				int SelAction = 0;
				Search = "APPNO is not null ";



				if (frm["SelAction"] != "")
				{
					TempData["SelAction"] = frm["SelAction"];
					ViewBag.SelectedAction = frm["SelAction"];
					SelAction = Convert.ToInt32(frm["SelAction"].ToString());
					if (SelAction == 2)// Pending (Show which new objection added) 
					{
						Search += " and  AdminObjStatus='Pending'";
					}
					else if (SelAction == 3)// Received (Show which reply sent by school)
					{
						Search += " and  AdminObjStatus='Received'";
					}
					else if (SelAction == 4)// Completed (show which approved/rejected by dealing)
					{
						Search += " and AdminObjStatus='Completed'";
					}
				}

				if (DistAllow != "")
				{
					if (AdminType.ToUpper() != "ADMIN".ToUpper() && !AdminType.ToUpper().Contains("INSPECTION"))
					{
						Search += " and DIST in (" + DistAllow + ") ";
					}
				}


				if (frm["Sch1"] != "")
				{
					ViewBag.SelectedItem = frm["Sch1"];
					TempData["SelectedItem"] = frm["Sch1"];
					int SelValueSch = Convert.ToInt32(frm["Sch1"].ToString());

					if (frm["SearchString"] != "")
					{
						ViewBag.SearchString = frm["SearchString"].ToString();
						if (SelValueSch == 1)
						{ Search += " and APPNO=" + frm["SearchString"].ToString(); }
						else if (SelValueSch == 2)
						{ Search += " and  SCHL='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 3)
						{ Search += " and  IDNO='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 4)
						{ Search += " and  OLID='" + frm["SearchString"].ToString() + "'"; }

					}
				}

				if (AppType == "AFF")
				{
					spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser, Search, "", 1, 15); // 12- Objection by appno
				}
				else if (AppType == "AC")
				{
					spi.StoreAllData = new AbstractLayer.AffiliationDB().AffiliationContinuationList(AdminUser, Search, "", 1, 15); // 12- Objection by appno
				}
				else if (AppType == "AS")
				{
					spi.StoreAllData = new AbstractLayer.AdditionalSectionDB().AdditionalSectionList(AdminUser, Search, "", 1, 15); // 12- Objection by appno
				}


				TempData["SearchViewAllEAffiliationObjections"] = Search;
				TempData.Keep(); // to store search value for view


				if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.LastPageIndex = 0;
					ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
				}

				return View(spi);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return View();
			}
		}



		[AdminLoginCheckFilter]
		public ActionResult EAffiliationApprovedList(EAffiliationModel spi, int? page, string AppType)
		{
			try
			{
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string AdminType = Session["AdminType"].ToString().ToUpper();
				string AdminUser = Session["AdminUser"].ToString().ToUpper();

				List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDL();
				ViewBag.MyApprovalStatusList = MyAcceptRejectList;

				string DistAllow = "";
				// Dist Allowed
				if (ViewBag.DistAllow == null)
				{ return RedirectToAction("Index", "Admin"); }
				else
				{
					DistAllow = ViewBag.DistAllow;
				}

				if (ViewBag.DistUser == null)
				{
					ViewBag.MyDist = null;
				}
				else
				{
					ViewBag.MyDist = ViewBag.DistUser; // User District

				}


				#region Action Assign Method
				if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{ ViewBag.IsObjection = 1; ViewBag.IsFillInspectionReport = 1; ViewBag.IsViewInspectionReport = 1; }
				else
				{

					string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
					string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						ViewBag.IsObjection = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("Admin/AffiliationObjectionLetter".ToUpper())).Count();
						ViewBag.IsFillInspectionReport = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/FillEAffiliationInspectionReport".ToUpper())).Count();
						ViewBag.IsViewInspectionReport = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/ViewEAffiliationInspectionReport".ToUpper())).Count();
						//ViewBag.IsCloseApplication = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
						// ViewBag.IsUpdateEmailMobile = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
						// ViewBag.IsUnlockForm = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
					}
				}
				#endregion Action Assign Method
				ViewBag.MyAppCls = new AbstractLayer.DBClass().GetAllClass().ToList();
				ViewBag.SelectedCls = "0";

				List<EAffiliationApplicationStatusMaster> eAffiliationApplicationStatusMasterList = new AbstractLayer.EAffiliationDB().ForwardListByAppNoStatus(AppType, 1, "", AdminUser);
				ViewBag.MyForwardList = eAffiliationApplicationStatusMasterList;
				//AppType
				ViewBag.MyAppType = AbstractLayer.EAffiliationDB.GetApplicationTypeList().ToList();
				ViewBag.SelectedAppType = "0";



				//Search By
				//Status
				var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "Application No" }, new { ID = "6", Name = "School Code" }, new { ID = "2", Name = "UDISE Code" },
				new { ID = "3", Name = "School Name" },new { ID = "4", Name = "Station Name" },new { ID = "5", Name = "Mobile No" },new { ID = "7", Name = "EAffiliation Type" },}, "ID", "Name", 1);
				ViewBag.MySch = itemSearchBy.ToList();
				ViewBag.SelectedSearchBy = "0";

				var itemAction = new SelectList(new[] { new { ID = "1", Name = "ALL APPLICATION" },
					 new { ID = "2", Name = "APPLICATION REGISTERED" },
					  new { ID = "3", Name = "FEE PAID  " },
					   new { ID = "4", Name = "FORM UNLOCKED" },
					new { ID = "5", Name = "CLOSED APPLICATION" },
				 new { ID = "6", Name = "Pending In School Master" },}, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";


				//ViewBag.MyForwardList

				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = "";

				if (TempData["SearchEAffiliationApprovedList"] != null)
				{
					Search += TempData["SearchEAffiliationApprovedList"].ToString();
					ViewBag.SelectedAction = TempData["SelAction"];
					ViewBag.SelectedItem = TempData["SelectedItem"];
					ViewBag.SelectedDist = TempData["SelectedDist"];
					ViewBag.SelectedAppType = TempData["SelAppType"];


					TempData["SelAppType"] = ViewBag.SelectedAppType;
					//
					if (DistAllow != "")
					{
						Search += " and DIST in (" + DistAllow + ") ";
					}
					if (AdminType.ToUpper() != "ADMIN".ToUpper())
					{
						Search += " and upper(CurrentApplicationBranch)='" + AdminType + "'";
					}
					//


					Search += " and  isnull(ApprovalStatus,'')='A'";

					if (ViewBag.SelectedAppType == "AFF")
					{
						spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser, Search, "", pageIndex, 11);
					}
					else if (ViewBag.SelectedAppType == "AC")
					{
						spi.StoreAllData = new AbstractLayer.AffiliationDB().AffiliationContinuationList(AdminUser, Search, "", pageIndex, 11);
					}
					else if (ViewBag.SelectedAppType == "AS")
					{
						spi.StoreAllData = new AbstractLayer.AdditionalSectionDB().AdditionalSectionList(AdminUser, Search, "", pageIndex, 11);
					}

					//spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser,Search, "", pageIndex, 11); // 11 -- INBOX

					if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.LastPageIndex = 0;
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						TempData["SearchEAffiliationApprovedList"] = Search;
						TempData.Keep();
						ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
						int count = Convert.ToInt32(spi.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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
				return View(spi);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}


		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult EAffiliationApprovedList(EAffiliationModel spi, FormCollection frm, int? page, string AppType)
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string AdminType = Session["AdminType"].ToString().ToUpper();
				List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDL();
				ViewBag.MyApprovalStatusList = MyAcceptRejectList;
				string DistAllow = "";
				// Dist Allowed
				if (ViewBag.DistAllow == null)
				{ return RedirectToAction("Index", "Admin"); }
				else
				{
					DistAllow = ViewBag.DistAllow;
				}
				if (ViewBag.DistUser == null)
				{
					ViewBag.MyDist = null;

				}
				else
				{
					ViewBag.MyDist = ViewBag.DistUser; // User District
				}

				#region Action Assign Method
				if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{ ViewBag.IsObjection = 1; ViewBag.IsFillInspectionReport = 1; ViewBag.IsViewInspectionReport = 1; }
				else
				{

					string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
					string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						ViewBag.IsObjection = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("Admin/AffiliationObjectionLetter".ToUpper())).Count();
						ViewBag.IsFillInspectionReport = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/FillEAffiliationInspectionReport".ToUpper())).Count();
						ViewBag.IsViewInspectionReport = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/ViewEAffiliationInspectionReport".ToUpper())).Count();
						//ViewBag.IsCloseApplication = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
						// ViewBag.IsUpdateEmailMobile = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
						// ViewBag.IsUnlockForm = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
					}
				}
				#endregion Action Assign Method

				ViewBag.MyAppCls = new AbstractLayer.DBClass().GetAllClass().ToList();
				ViewBag.SelectedCls = "0";
				string AdminUser = Session["AdminUser"].ToString().ToUpper();


				List<EAffiliationApplicationStatusMaster> eAffiliationApplicationStatusMasterList = new AbstractLayer.EAffiliationDB().ForwardListByAppNoStatus(AppType, 1, "", AdminUser);
				ViewBag.MyForwardList = eAffiliationApplicationStatusMasterList;
				//AppType
				//AppType
				ViewBag.MyAppType = AbstractLayer.EAffiliationDB.GetApplicationTypeList().ToList();
				ViewBag.SelectedAppType = "0";
				//Status
				var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "Application No" }, new { ID = "6", Name = "School Code" }, new { ID = "2", Name = "UDISE Code" },
				new { ID = "3", Name = "School Name" },new { ID = "4", Name = "Station Name" },new { ID = "5", Name = "Mobile No" },new { ID = "7", Name = "EAffiliation Type" },}, "ID", "Name", 1);
				ViewBag.MySch = itemSearchBy.ToList();
				ViewBag.SelectedSearchBy = "0";

				var itemAction = new SelectList(new[] { new { ID = "1", Name = "ALL APPLICATION" },
					 new { ID = "2", Name = "APPLICATION REGISTERED" },
					  new { ID = "3", Name = "FEE PAID  " },
					   new { ID = "4", Name = "FORM UNLOCKED" },
					new { ID = "5", Name = "CLOSED APPLICATION" },
				 new { ID = "6", Name = "Pending In School Master" },}, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";


				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;

				ViewBag.SelectedAppType = AppType;
				TempData["SelAppType"] = AppType;
				int SelAction = 0;
				Search = "APPNO is not null and  isnull(ApprovalStatus,'')='A'";

				if (AppType == "AFF")
				{


					if (frm["AppCls"] != "")
					{
						ViewBag.SelectedCls = frm["AppCls"];
						TempData["SelectedCls"] = frm["AppCls"];
						Search += " and ClassLevel='" + frm["AppCls"].ToString() + "'";
					}
					if (frm["Dist1"] != "")
					{
						ViewBag.SelectedDist = frm["Dist1"];
						TempData["SelectedDist"] = frm["Dist1"];
						Search += " and Dist='" + frm["Dist1"].ToString() + "'";
					}

					if (frm["SelAction"] != "")
					{
						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						SelAction = Convert.ToInt32(frm["SelAction"].ToString());
						if (SelAction == 2)//Registered
						{
							Search += " and  HistoryStatus='APPLICATION REGISTERED'";
						}
						else if (SelAction == 3)//Registered
						{
							Search += " and  HistoryStatus='FEE PAID'";
						}
						else if (SelAction == 4)//FORM UNLOCKED
						{
							Search += " and IsFormLock='0'";
						}
						else if (SelAction == 5)
						{
							Search += " and  CurrentApplicationStatus=8";
						}
						else if (SelAction == 6)
						{ Search += "  and  FinalStatus='Fee Verified' and isnull(SchlMasterStatus,'')='' "; }
					}

					if (frm["Sch1"] != "")
					{
						ViewBag.SelectedItem = frm["Sch1"];
						TempData["SelectedItem"] = frm["Sch1"];
						int SelValueSch = Convert.ToInt32(frm["Sch1"].ToString());

						if (frm["SearchString"] != "")
						{
							if (SelValueSch == 1)
							{ Search += " and APPNO=" + frm["SearchString"].ToString(); }
							else if (SelValueSch == 2)
							{ Search += " and  UDISECODE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  SCHLNME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and  SCHLMOBILE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  SCHL='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 7)
							{ Search += " and  EAffType='" + frm["SearchString"].ToString() + "'"; }

						}
					}


					//
					if (DistAllow != "")
					{
						if (AdminType.ToUpper() != "ADMIN".ToUpper() && !AdminType.ToUpper().Contains("INSPECTION"))
						{
							Search += " and DIST in (" + DistAllow + ") ";
						}
					}
					if (AdminType.ToUpper() != "ADMIN".ToUpper())
					{
						Search += " and upper(CurrentApplicationBranch)='" + AdminType + "'";
					}
					//
					spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser, Search, "", pageIndex, 11); // 11 - INBOX

				}

				else if (AppType == "AC")
				{

					if (frm["Dist1"] != "")
					{
						ViewBag.SelectedDist = frm["Dist1"];
						TempData["SelectedDist"] = frm["Dist1"];
						Search += " and Dist='" + frm["Dist1"].ToString() + "'";
					}

					if (frm["SelAction"] != "")
					{
						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						SelAction = Convert.ToInt32(frm["SelAction"].ToString());
						if (SelAction == 2)//Registered
						{
							Search += "  and  ID is not null  ";
						}
						else if (SelAction == 3)//Fees Paid
						{
							Search += "  and  IsFinalSubmit='FINAL'  ";
						}
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
							{ Search += " and  UDISECODE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  SCHLNME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and  MOBILE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  SCHL='" + frm["SearchString"].ToString() + "'"; }
							//else if (SelValueSch == 7)
							//{ Search += " and  EAffType='" + frm["SearchString"].ToString() + "'"; }

						}
					}
					if (DistAllow != "")
					{
						if (AdminType.ToUpper() != "ADMIN".ToUpper() && !AdminType.ToUpper().Contains("INSPECTION"))
						{
							Search += " and DIST in (" + DistAllow + ") ";
						}
					}
					if (AdminType.ToUpper() != "ADMIN".ToUpper())
					{
						Search += " and upper(CurrentApplicationBranch)='" + AdminType + "'";
					}
					spi.StoreAllData = new AbstractLayer.AffiliationDB().AffiliationContinuationList(AdminUser, Search, "", pageIndex, 11);

				}
				else if (AppType == "AS")
				{
					if (frm["AppCls"] != "")
					{
						ViewBag.SelectedCls = frm["AppCls"];
						TempData["SelectedCls"] = frm["AppCls"];
						Search += " and ClassLevel='" + frm["AppCls"].ToString() + "'";
					}
					if (frm["Dist1"] != "")
					{
						ViewBag.SelectedDist = frm["Dist1"];
						TempData["SelectedDist"] = frm["Dist1"];
						Search += " and Dist='" + frm["Dist1"].ToString() + "'";
					}

					if (frm["SelAction"] != "")
					{
						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						SelAction = Convert.ToInt32(frm["SelAction"].ToString());
						if (SelAction == 2)//Registered
						{
							Search += "  and  ID is not null  ";
						}
						else if (SelAction == 3)//Fees Paid
						{
							Search += "  and  IsFinalSubmit='FINAL'  ";
						}
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
							{ Search += " and  UDISECODE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  SCHLNME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and  MOBILE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  SCHL='" + frm["SearchString"].ToString() + "'"; }
							//else if (SelValueSch == 7)
							//{ Search += " and  EAffType='" + frm["SearchString"].ToString() + "'"; }

						}
					}
					if (DistAllow != "")
					{
						if (AdminType.ToUpper() != "ADMIN".ToUpper() && !AdminType.ToUpper().Contains("INSPECTION"))
						{
							Search += " and DIST in (" + DistAllow + ") ";
						}
					}
					if (AdminType.ToUpper() != "ADMIN".ToUpper())
					{
						Search += " and upper(CurrentApplicationBranch)='" + AdminType + "'";
					}
					spi.StoreAllData = new AbstractLayer.AdditionalSectionDB().AdditionalSectionList(AdminUser, Search, "", pageIndex, 11);

				}



				TempData["SearchEAffiliationApprovedList"] = Search;
				TempData.Keep(); // to store search value for view


				if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.LastPageIndex = 0;
					ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
					int count = Convert.ToInt32(spi.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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

				return View(spi);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return View();
			}
		}




		[AdminLoginCheckFilter]
		public ActionResult EAffiliationRejectedList(EAffiliationModel spi, int? page, string AppType)
		{
			try
			{
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string AdminType = Session["AdminType"].ToString().ToUpper();
				string AdminUser = Session["AdminUser"].ToString().ToUpper();

				List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDL();
				ViewBag.MyApprovalStatusList = MyAcceptRejectList;

				string DistAllow = "";
				// Dist Allowed
				if (ViewBag.DistAllow == null)
				{ return RedirectToAction("Index", "Admin"); }
				else
				{
					DistAllow = ViewBag.DistAllow;
				}

				if (ViewBag.DistUser == null)
				{
					ViewBag.MyDist = null;
				}
				else
				{
					ViewBag.MyDist = ViewBag.DistUser; // User District

				}


				#region Action Assign Method
				if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{ ViewBag.IsObjection = 1; ViewBag.IsFillInspectionReport = 1; ViewBag.IsViewInspectionReport = 1; }
				else
				{

					string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
					string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						ViewBag.IsObjection = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("Admin/AffiliationObjectionLetter".ToUpper())).Count();
						ViewBag.IsFillInspectionReport = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/FillEAffiliationInspectionReport".ToUpper())).Count();
						ViewBag.IsViewInspectionReport = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/ViewEAffiliationInspectionReport".ToUpper())).Count();
						//ViewBag.IsCloseApplication = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
						// ViewBag.IsUpdateEmailMobile = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
						// ViewBag.IsUnlockForm = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
					}
				}
				#endregion Action Assign Method
				ViewBag.MyAppCls = new AbstractLayer.DBClass().GetAllClass().ToList();
				ViewBag.SelectedCls = "0";

				List<EAffiliationApplicationStatusMaster> eAffiliationApplicationStatusMasterList = new AbstractLayer.EAffiliationDB().ForwardListByAppNoStatus(AppType, 1, "", AdminUser);
				ViewBag.MyForwardList = eAffiliationApplicationStatusMasterList;
				//AppType
				ViewBag.MyAppType = AbstractLayer.EAffiliationDB.GetApplicationTypeList().ToList();
				ViewBag.SelectedAppType = "0";



				//Search By
				//Status
				var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "Application No" }, new { ID = "6", Name = "School Code" }, new { ID = "2", Name = "UDISE Code" },
				new { ID = "3", Name = "School Name" },new { ID = "4", Name = "Station Name" },new { ID = "5", Name = "Mobile No" },new { ID = "7", Name = "EAffiliation Type" },}, "ID", "Name", 1);
				ViewBag.MySch = itemSearchBy.ToList();
				ViewBag.SelectedSearchBy = "0";

				var itemAction = new SelectList(new[] { new { ID = "1", Name = "ALL APPLICATION" },
					 new { ID = "2", Name = "APPLICATION REGISTERED" },
					  new { ID = "3", Name = "FEE PAID  " },
					   new { ID = "4", Name = "FORM UNLOCKED" },
					new { ID = "5", Name = "CLOSED APPLICATION" },
				 new { ID = "6", Name = "Pending In School Master" },}, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";


				//ViewBag.MyForwardList

				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = "";

				if (TempData["SearchEAffiliationRejectedList"] != null)
				{
					Search += TempData["SearchEAffiliationRejectedList"].ToString();
					ViewBag.SelectedAction = TempData["SelAction"];
					ViewBag.SelectedItem = TempData["SelectedItem"];
					ViewBag.SelectedDist = TempData["SelectedDist"];
					ViewBag.SelectedAppType = TempData["SelAppType"];


					TempData["SelAppType"] = ViewBag.SelectedAppType;
					//
					if (DistAllow != "")
					{
						Search += " and DIST in (" + DistAllow + ") ";
					}
					if (AdminType.ToUpper() != "ADMIN".ToUpper())
					{
						Search += " and upper(CurrentApplicationBranch)='" + AdminType + "'";
					}
					//

					Search += " and  isnull(ApprovalStatus,'')='R'";  // Rejected

					if (ViewBag.SelectedAppType == "AFF")
					{
						spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser, Search, "", pageIndex, 11);
					}
					else if (ViewBag.SelectedAppType == "AC")
					{
						spi.StoreAllData = new AbstractLayer.AffiliationDB().AffiliationContinuationList(AdminUser, Search, "", pageIndex, 11);
					}
					else if (ViewBag.SelectedAppType == "AS")
					{
						spi.StoreAllData = new AbstractLayer.AdditionalSectionDB().AdditionalSectionList(AdminUser, Search, "", pageIndex, 11);
					}

					//spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser,Search, "", pageIndex, 11); // 11 -- INBOX

					if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.LastPageIndex = 0;
						ViewBag.TotalCount = 0;
						return View();
					}
					else
					{
						TempData["SearchEAffiliationRejectedList"] = Search;
						TempData.Keep();
						ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
						int count = Convert.ToInt32(spi.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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
				return View(spi);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				//return RedirectToAction("Logout", "Login");
				return View();
			}
		}


		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult EAffiliationRejectedList(EAffiliationModel spi, FormCollection frm, int? page, string AppType)
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				int AdminId = Convert.ToInt32(Session["AdminId"]);
				string AdminType = Session["AdminType"].ToString().ToUpper();
				List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDL();
				ViewBag.MyApprovalStatusList = MyAcceptRejectList;
				string DistAllow = "";
				// Dist Allowed
				if (ViewBag.DistAllow == null)
				{ return RedirectToAction("Index", "Admin"); }
				else
				{
					DistAllow = ViewBag.DistAllow;
				}
				if (ViewBag.DistUser == null)
				{
					ViewBag.MyDist = null;

				}
				else
				{
					ViewBag.MyDist = ViewBag.DistUser; // User District
				}

				#region Action Assign Method
				if (Session["AdminType"].ToString().ToUpper() == "ADMIN")
				{ ViewBag.IsObjection = 1; ViewBag.IsFillInspectionReport = 1; ViewBag.IsViewInspectionReport = 1; }
				else
				{

					string actionName = this.ControllerContext.RouteData.Values["action"].ToString();
					string controllerName = this.ControllerContext.RouteData.Values["controller"].ToString();
					DataSet aAct = objCommon.GetActionOfSubMenu(AdminId, controllerName, actionName);
					if (aAct.Tables[0].Rows.Count > 0)
					{
						ViewBag.IsObjection = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("Admin/AffiliationObjectionLetter".ToUpper())).Count();
						ViewBag.IsFillInspectionReport = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/FillEAffiliationInspectionReport".ToUpper())).Count();
						ViewBag.IsViewInspectionReport = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/ViewEAffiliationInspectionReport".ToUpper())).Count();
						//ViewBag.IsCloseApplication = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
						// ViewBag.IsUpdateEmailMobile = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
						// ViewBag.IsUnlockForm = aAct.Tables[0].AsEnumerable().Where(c => c.Field<string>("MenuUrl").ToUpper().Equals("ADMIN/BULKPRIVATECANDIDATEPRINTFORM".ToUpper())).Count();
					}
				}
				#endregion Action Assign Method

				ViewBag.MyAppCls = new AbstractLayer.DBClass().GetAllClass().ToList();
				ViewBag.SelectedCls = "0";
				string AdminUser = Session["AdminUser"].ToString().ToUpper();


				List<EAffiliationApplicationStatusMaster> eAffiliationApplicationStatusMasterList = new AbstractLayer.EAffiliationDB().ForwardListByAppNoStatus(AppType, 1, "", AdminUser);
				ViewBag.MyForwardList = eAffiliationApplicationStatusMasterList;
				//AppType
				//AppType
				ViewBag.MyAppType = AbstractLayer.EAffiliationDB.GetApplicationTypeList().ToList();
				ViewBag.SelectedAppType = "0";
				//Status
				var itemSearchBy = new SelectList(new[] { new { ID = "1", Name = "Application No" }, new { ID = "6", Name = "School Code" }, new { ID = "2", Name = "UDISE Code" },
				new { ID = "3", Name = "School Name" },new { ID = "4", Name = "Station Name" },new { ID = "5", Name = "Mobile No" },new { ID = "7", Name = "EAffiliation Type" },}, "ID", "Name", 1);
				ViewBag.MySch = itemSearchBy.ToList();
				ViewBag.SelectedSearchBy = "0";

				var itemAction = new SelectList(new[] { new { ID = "1", Name = "ALL APPLICATION" },
					 new { ID = "2", Name = "APPLICATION REGISTERED" },
					  new { ID = "3", Name = "FEE PAID  " },
					   new { ID = "4", Name = "FORM UNLOCKED" },
					new { ID = "5", Name = "CLOSED APPLICATION" },
				 new { ID = "6", Name = "Pending In School Master" },}, "ID", "Name", 1);
				ViewBag.MyAction = itemAction.ToList();
				ViewBag.SelectedAction = "0";


				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;

				ViewBag.SelectedAppType = AppType;
				TempData["SelAppType"] = AppType;
				int SelAction = 0;
				Search = "APPNO is not null ";


				Search += " and  isnull(ApprovalStatus,'')='R'";  // Rejected

				if (AppType == "AFF")
				{


					if (frm["AppCls"] != "")
					{
						ViewBag.SelectedCls = frm["AppCls"];
						TempData["SelectedCls"] = frm["AppCls"];
						Search += " and ClassLevel='" + frm["AppCls"].ToString() + "'";
					}
					if (frm["Dist1"] != "")
					{
						ViewBag.SelectedDist = frm["Dist1"];
						TempData["SelectedDist"] = frm["Dist1"];
						Search += " and Dist='" + frm["Dist1"].ToString() + "'";
					}

					if (frm["SelAction"] != "")
					{
						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						SelAction = Convert.ToInt32(frm["SelAction"].ToString());
						if (SelAction == 2)//Registered
						{
							Search += " and  HistoryStatus='APPLICATION REGISTERED'";
						}
						else if (SelAction == 3)//Registered
						{
							Search += " and  HistoryStatus='FEE PAID'";
						}
						else if (SelAction == 4)//FORM UNLOCKED
						{
							Search += " and IsFormLock='0'";
						}
						else if (SelAction == 5)
						{
							Search += " and  CurrentApplicationStatus=8";
						}
						else if (SelAction == 6)
						{ Search += "  and  FinalStatus='Fee Verified' and isnull(SchlMasterStatus,'')='' "; }
					}

					if (frm["Sch1"] != "")
					{
						ViewBag.SelectedItem = frm["Sch1"];
						TempData["SelectedItem"] = frm["Sch1"];
						int SelValueSch = Convert.ToInt32(frm["Sch1"].ToString());

						if (frm["SearchString"] != "")
						{
							if (SelValueSch == 1)
							{ Search += " and APPNO=" + frm["SearchString"].ToString(); }
							else if (SelValueSch == 2)
							{ Search += " and  UDISECODE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  SCHLNME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and  SCHLMOBILE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  SCHL='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 7)
							{ Search += " and  EAffType='" + frm["SearchString"].ToString() + "'"; }

						}
					}


					//
					if (DistAllow != "")
					{
						if (AdminType.ToUpper() != "ADMIN".ToUpper() && !AdminType.ToUpper().Contains("INSPECTION"))
						{
							Search += " and DIST in (" + DistAllow + ") ";
						}
					}
					if (AdminType.ToUpper() != "ADMIN".ToUpper())
					{
						Search += " and upper(CurrentApplicationBranch)='" + AdminType + "'";
					}
					//
					spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser, Search, "", pageIndex, 11); // 11 - INBOX

				}

				else if (AppType == "AC")
				{

					if (frm["Dist1"] != "")
					{
						ViewBag.SelectedDist = frm["Dist1"];
						TempData["SelectedDist"] = frm["Dist1"];
						Search += " and Dist='" + frm["Dist1"].ToString() + "'";
					}

					if (frm["SelAction"] != "")
					{
						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						SelAction = Convert.ToInt32(frm["SelAction"].ToString());
						if (SelAction == 2)//Registered
						{
							Search += "  and  ID is not null  ";
						}
						else if (SelAction == 3)//Fees Paid
						{
							Search += "  and  IsFinalSubmit='FINAL'  ";
						}
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
							{ Search += " and  UDISECODE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  SCHLNME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and  MOBILE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  SCHL='" + frm["SearchString"].ToString() + "'"; }
							//else if (SelValueSch == 7)
							//{ Search += " and  EAffType='" + frm["SearchString"].ToString() + "'"; }

						}
					}
					if (DistAllow != "")
					{
						if (AdminType.ToUpper() != "ADMIN".ToUpper() && !AdminType.ToUpper().Contains("INSPECTION"))
						{
							Search += " and DIST in (" + DistAllow + ") ";
						}
					}
					if (AdminType.ToUpper() != "ADMIN".ToUpper())
					{
						Search += " and upper(CurrentApplicationBranch)='" + AdminType + "'";
					}
					spi.StoreAllData = new AbstractLayer.AffiliationDB().AffiliationContinuationList(AdminUser, Search, "", pageIndex, 11);

				}
				else if (AppType == "AS")
				{
					if (frm["AppCls"] != "")
					{
						ViewBag.SelectedCls = frm["AppCls"];
						TempData["SelectedCls"] = frm["AppCls"];
						Search += " and ClassLevel='" + frm["AppCls"].ToString() + "'";
					}
					if (frm["Dist1"] != "")
					{
						ViewBag.SelectedDist = frm["Dist1"];
						TempData["SelectedDist"] = frm["Dist1"];
						Search += " and Dist='" + frm["Dist1"].ToString() + "'";
					}

					if (frm["SelAction"] != "")
					{
						TempData["SelAction"] = frm["SelAction"];
						ViewBag.SelectedAction = frm["SelAction"];
						SelAction = Convert.ToInt32(frm["SelAction"].ToString());
						if (SelAction == 2)//Registered
						{
							Search += "  and  ID is not null  ";
						}
						else if (SelAction == 3)//Fees Paid
						{
							Search += "  and  IsFinalSubmit='FINAL'  ";
						}
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
							{ Search += " and  UDISECODE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 3)
							{ Search += " and  SCHLNME like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 4)
							{ Search += " and  STATIONE like '%" + frm["SearchString"].ToString() + "%'"; }
							else if (SelValueSch == 5)
							{ Search += " and  MOBILE='" + frm["SearchString"].ToString() + "'"; }
							else if (SelValueSch == 6)
							{ Search += " and  SCHL='" + frm["SearchString"].ToString() + "'"; }
							//else if (SelValueSch == 7)
							//{ Search += " and  EAffType='" + frm["SearchString"].ToString() + "'"; }

						}
					}
					if (DistAllow != "")
					{
						if (AdminType.ToUpper() != "ADMIN".ToUpper() && !AdminType.ToUpper().Contains("INSPECTION"))
						{
							Search += " and DIST in (" + DistAllow + ") ";
						}
					}
					if (AdminType.ToUpper() != "ADMIN".ToUpper())
					{
						Search += " and upper(CurrentApplicationBranch)='" + AdminType + "'";
					}
					spi.StoreAllData = new AbstractLayer.AdditionalSectionDB().AdditionalSectionList(AdminUser, Search, "", pageIndex, 11);

				}



				TempData["SearchEAffiliationRejectedList"] = Search;
				TempData.Keep(); // to store search value for view


				if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.LastPageIndex = 0;
					ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
					int count = Convert.ToInt32(spi.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
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

				return View(spi);
			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return View();
			}
		}



		[AdminLoginCheckFilter]
		public ActionResult EAffiliationDashBoard(EAffiliationModel spi)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			string AdminUser = Session["AdminUser"].ToString().ToUpper();
			try
			{
				string DistAllow = "";
				// Dist Allowed
				if (ViewBag.DistAllow == null)
				{ return RedirectToAction("Index", "Admin"); }
				else
				{
					DistAllow = ViewBag.DistAllow;
				}
				string Search = "APPNO is not null ";
				if (DistAllow != "")
				{
					Search += " and DIST in (" + DistAllow + ") ";
				}

				spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationDashBoardSP(AdminUser, adminLoginSession.AdminType, Search, 1);

				if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;

				}

			}
			catch (Exception ex)
			{
			}
			return View(spi);
		}



		#region AddtionSectionAllow Admin

		[AdminLoginCheckFilter]
		public ActionResult AdditionalSectionAllow(string id, string AppType, AffObjectionLettersViewModel spi)
		{

			int AdminId = Convert.ToInt32(Session["AdminId"]);
			string AdminType = Session["AdminType"].ToString().ToUpper();
			string AdminUser = Session["AdminUser"].ToString().ToUpper();
			if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(AppType))
			{
				return RedirectToAction("InboxEAffiliation", "Admin");
			}
			spi.AffObjectionLetters = new AffObjectionLetters();

			spi.AffObjectionLetters.AppNo = id;
			spi.AffObjectionLetters.AppType = AppType;
			List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDL();
			ViewBag.MyApprovalStatusList = MyAcceptRejectList;


			spi.affObjectionListMastersList = new AbstractLayer.AdminDB().GetAffObjectionListMasters().ToList();
			try
			{
				string Search = " APPNO = '" + id + "'";
				if (AppType == "AFF")
				{
					spi.StoreAllData = new AbstractLayer.EAffiliationDB().EAffiliationList(AdminUser, Search, "", 1, 15); // 12- Objection by appno
				}
				else if (AppType == "AC")
				{
					spi.StoreAllData = new AbstractLayer.AffiliationDB().AffiliationContinuationList(AdminUser, Search, "", 1, 15); // 12- Objection by appno
				}
				else if (AppType == "AS")
				{
					spi.StoreAllData = new AbstractLayer.AdditionalSectionDB().AdditionalSectionList(AdminUser, Search, "", 1, 15); // 12- Objection by appno
				}

				if (spi.StoreAllData == null || spi.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.TotalCount = spi.StoreAllData.Tables[0].Rows.Count;
				}
			}
			catch (Exception ex)
			{
			}
			return View(spi);

		}


		[HttpPost]
		[AdminLoginCheckFilter]
		public ActionResult AdditionalSectionAllow(string id, string AppType, AffObjectionLettersViewModel spi, string cmd, string OtherDocumentName, FormCollection frm, HttpPostedFileBase objectionFile)
		{
			AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];

			int AdminId = Convert.ToInt32(Session["AdminId"]);
			string AdminType = Session["AdminType"].ToString().ToUpper();
			string AdminUser = Session["AdminUser"].ToString().ToUpper();
			if (spi.AffObjectionLetters.AppNo != id)
			{
				return RedirectToAction("InboxEAffiliation", "Admin");
			}

			List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDL();
			ViewBag.MyApprovalStatusList = MyAcceptRejectList;

			id = spi.AffObjectionLetters.AppNo;
			AppType = spi.AffObjectionLetters.AppType;
			spi.affObjectionListMastersList = new AbstractLayer.AdminDB().GetAffObjectionListMasters().ToList();
			try
			{
				if (spi.selObjCode.Count() > 0)
				{
					//  string[] ObjCodeSplit = frm["AffObjectionLetters.ObjCode"].Split(',');

					foreach (string objcode in spi.selObjCode)
					{
						string DocName = objcode;
						//string DocName = spi.AffObjectionLetters.ObjCode;
						if (!string.IsNullOrEmpty(OtherDocumentName) && objcode == "aff_999")
						{
							DocName = "OtherDocument_" + OtherDocumentName;
							spi.AffObjectionLetters.Objection = OtherDocumentName;
						}

						if (!string.IsNullOrEmpty(cmd))
						{

							// Save file
							string filename = "";
							string FilepathExist = "", path = "";
							string exactPath = "~/Upload/Upload2023/AffObjecttonLetter/" + spi.AffObjectionLetters.AppType;
							if (objectionFile != null)
							{
								//Upload/AdditionalSection
								string ext = Path.GetExtension(objectionFile.FileName);
								filename = spi.AffObjectionLetters.AppNo + "_" + DocName.Replace(" ", "_") + ext;
								path = Path.Combine(Server.MapPath(exactPath), filename.ToUpper());
								FilepathExist = Path.Combine(Server.MapPath(exactPath));
								spi.AffObjectionLetters.ObjectionFile = "allfiles/Upload2023/AFFObjecttonLetter/" + spi.AffObjectionLetters.AppType + filename.ToUpper();
							}

							if (cmd.ToLower() == "submit" || cmd.ToLower() == "save")
							{
								AffObjectionLetters affObjectionLettersData = new AffObjectionLetters()
								{

									AppType = spi.AffObjectionLetters.AppType,
									AppNo = spi.AffObjectionLetters.AppNo,
									ObjCode = objcode,
									Objection = spi.AffObjectionLetters.Objection,
									ObjectionFile = spi.AffObjectionLetters.ObjectionFile,
									ObjDate = DateTime.Now,
									ObjStatus = spi.AffObjectionLetters.ObjStatus,
									SubmitBy = AdminUser,
									ClearDate = DateTime.Now,
									Attachment = spi.AffObjectionLetters.Attachment,
									SchoolReply = spi.AffObjectionLetters.SchoolReply,
									SchoolReplyOn = spi.AffObjectionLetters.SchoolReplyOn,
									EmpUserId = adminLoginSession.AdminEmployeeUserId

								};
								_context.AffObjectionLetters.Add(affObjectionLettersData);
								int insertedRecords = _context.SaveChanges();

								if (insertedRecords > 0)
								{
									ViewData["result"] = "1";
									if (objectionFile != null)
									{
										if (!Directory.Exists(FilepathExist))
										{
											Directory.CreateDirectory(FilepathExist);
										}

										string Orgfile = filename;

										using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
										{
											using (var newMemoryStream = new MemoryStream())
											{
												var uploadRequest = new TransferUtilityUploadRequest
												{
													InputStream = objectionFile.InputStream,
													Key = string.Format("allfiles/Upload2023/AFFObjecttonLetter/" + spi.AffObjectionLetters.AppType + "/{0}", Orgfile),

													BucketName = BUCKET_NAME,
													CannedACL = S3CannedACL.PublicRead
												};

												var fileTransferUtility = new TransferUtility(client);
												fileTransferUtility.Upload(uploadRequest);
											}
										}

										//objectionFile.SaveAs(path);
									}
								}
								else
								{
									ViewData["result"] = "2";
								}
							}
						}

					}

				}

			}
			catch (Exception ex)
			{
				ViewData["result"] = "ERR";
				ViewBag.Mesaage = ex.Message;
			}
			return View(spi);

		}

		#endregion

		#region FinalPrintForm

		public ActionResult FinalPrintFormAffiliation(string id, AffiliationModel am)
		{
			try
			{
				//
				if (id == null)
				{
					return RedirectToAction("Index", "Admin");
				}

				DataSet outDs = new DataSet();
				am = new AbstractLayer.AffiliationDB().AffiliationContinuationBySchl(id, 2, out outDs);//ResultStatics
				if (am.ID > 0)
				{
					am.StoreAllData = outDs;
					ViewBag.Totalcount = 1;
					ViewBag.AID = am.ID;
					ViewBag.ChallanId = am.ChallanId;
					ViewBag.IsVerified = am.challanVerify;
					ViewBag.ChallanDt = am.ChallanDt;
				}
				else
				{
					ViewBag.AID = 0;
					ViewBag.IsVerified = 0;
					ViewBag.Totalcount = 0;
				}
			}
			catch (Exception)
			{

				throw;
			}
			return View(am);
		}

		public ActionResult FinalPrintFormAdditionalSection(string id, AdditionalSectionModel am)
		{
			try
			{

				if (id == null)
				{
					return RedirectToAction("Index", "Admin");
				}

				DataSet outDs = new DataSet();
				am = new AbstractLayer.AdditionalSectionDB().AdditionalSectionBySchl(id, 2, out outDs);//ResultStatics
				if (am.ID > 0)
				{
					am.StoreAllData = outDs;
					ViewBag.Totalcount = 1;
					ViewBag.AID = am.ID;

					if (am.StoreAllData.Tables[1].Rows.Count > 0)
					{
						ViewBag.ChallanId = am.StoreAllData.Tables[1].Rows[0]["ChallanId"].ToString();
						ViewBag.IsVerified = am.StoreAllData.Tables[1].Rows[0]["Verified"].ToString();
						ViewBag.ChallanDt = am.StoreAllData.Tables[1].Rows[0]["CHLNDATE"].ToString();

					}
					else
					{
						ViewBag.ChallanId = "1";
						ViewBag.IsVerified = am.challanVerify;
						ViewBag.ChallanDt = "";
					}
				}
				else
				{
					ViewBag.AID = 0;
					ViewBag.IsVerified = 0;
					ViewBag.Totalcount = 0;
				}
			}
			catch (Exception)
			{

				throw;
			}
			return View(am);
		}

		#endregion


		#region AdditionSection 
		[HttpPost]
		public ActionResult AdditionSection_ObjectionLetter(HttpPostedFileBase ObjectionLetter, FormCollection frm)
		{
			if (Session["AdminId"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}
			string AdminType = Session["AdminType"].ToString().ToUpper();
			string OutError = "0";
			if (!string.IsNullOrEmpty(frm["APPNO"].ToString()) && frm["RemarksObjection"] != null)
			{
				string APPNO = frm["APPNO"].ToString();
				string fileName = "";
				string filepath = "";
				string SCHLMOBILE = "";


				if (ObjectionLetter != null && ObjectionLetter.ContentLength > 0 && !string.IsNullOrEmpty(ObjectionLetter.FileName))
				{

					string fileExt = Path.GetExtension(ObjectionLetter.FileName);
					fileName = APPNO + "_" + DateTime.Now.Date.ToString("ddMMyyyyhhmm") + fileExt;
					filepath = "Upload/Upload2023/AdditionalSectionDocuments/ObjectionLetter/" + fileName;
				}
				else
				{
					filepath = fileName = "";
				}



				AdditionalSectionModel _EAffiliationModel = new AdditionalSectionModel();
				_EAffiliationModel.UpdatedBy = Convert.ToInt32(Session["AdminId"]);
				_EAffiliationModel.SCHL = APPNO;
				_EAffiliationModel.Remarks = frm["RemarksObjection"].ToString();
				_EAffiliationModel.ObjectionLetter = filepath;
				SCHLMOBILE = frm["SCHLMOBILE"].ToString();


				//21 for objection
				int result = new AbstractLayer.AdditionalSectionDB().AdditionalSectionAction(_EAffiliationModel, 21, out OutError);
				if (OutError == "1")
				{
					if (!string.IsNullOrEmpty(frm["APPNO"].ToString()))
					{
						if (ObjectionLetter != null && ObjectionLetter.ContentLength > 0 && !string.IsNullOrEmpty(ObjectionLetter.FileName))
						{
							//var path = Path.Combine(Server.MapPath("~/Upload/"+ formName + "/" + dist + "/Photo"), stdPic);
							var path = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/AdditionalSectionDocuments/ObjectionLetter"), fileName);
							string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/AdditionalSectionDocuments/ObjectionLetter"));
							if (!Directory.Exists(FilepathExist))
							{
								Directory.CreateDirectory(FilepathExist);
							}
							ObjectionLetter.SaveAs(path);
						}

						string Sms = "Objection raised against your Addition Section application no " + _EAffiliationModel.SCHL + " successfully.";
						try
						{
							string getSms = objCommon.gosms(SCHLMOBILE, Sms);
						}
						catch (Exception) { }

					}


					return RedirectToAction("InboxEAffiliation", "Admin");
				}


			}
			return RedirectToAction("InboxEAffiliation", "Admin");
		}

		#endregion

		#region AffiliationContinuation 
		[HttpPost]
		public ActionResult AffiliationContinuation_ObjectionLetter(HttpPostedFileBase ObjectionLetter, FormCollection frm)
		{
			if (Session["AdminId"] == null)
			{
				return RedirectToAction("Index", "Admin");
			}
			string AdminType = Session["AdminType"].ToString().ToUpper();
			string OutError = "0";
			if (!string.IsNullOrEmpty(frm["APPNO"].ToString()) && frm["RemarksObjection"] != null)
			{
				string APPNO = frm["APPNO"].ToString();
				string fileName = "";
				string filepath = "";
				string SCHLMOBILE = "";

				if (ObjectionLetter != null && ObjectionLetter.ContentLength > 0 && !string.IsNullOrEmpty(ObjectionLetter.FileName))
				{

					string fileExt = Path.GetExtension(ObjectionLetter.FileName);
					fileName = APPNO + "_" + DateTime.Now.Date.ToString("ddMMyyyyhhmm") + fileExt;
					filepath = "Upload/Upload2023/AffiliationDocuments/ObjectionLetter/" + fileName;
				}
				else
				{
					filepath = fileName = "";
				}



				AffiliationModel _EAffiliationModel = new AffiliationModel();
				_EAffiliationModel.UpdatedBy = Convert.ToInt32(Session["AdminId"]);
				_EAffiliationModel.SCHL = APPNO;
				_EAffiliationModel.Remarks = frm["RemarksObjection"].ToString();
				_EAffiliationModel.ObjectionLetter = filepath;
				SCHLMOBILE = frm["SCHLMOBILE"].ToString();


				//21 for objection
				int result = new AbstractLayer.AffiliationDB().AffiliationContinuationAction(_EAffiliationModel, 21, out OutError);
				if (OutError == "1")
				{
					if (!string.IsNullOrEmpty(frm["APPNO"].ToString()))
					{
						if (ObjectionLetter != null && ObjectionLetter.ContentLength > 0 && !string.IsNullOrEmpty(ObjectionLetter.FileName))
						{
							//var path = Path.Combine(Server.MapPath("~/Upload/"+ formName + "/" + dist + "/Photo"), stdPic);
							var path = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/AffiliationDocuments/ObjectionLetter"), fileName);
							string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + "Upload2023/AffiliationDocuments/ObjectionLetter"));
							if (!Directory.Exists(FilepathExist))
							{
								Directory.CreateDirectory(FilepathExist);
							}
							ObjectionLetter.SaveAs(path);
						}

						string Sms = "Objection raised against your Annual Progess application no " + _EAffiliationModel.SCHL + " successfully.";
						try
						{
							string getSms = objCommon.gosms(SCHLMOBILE, Sms);
						}
						catch (Exception) { }

					}


					return RedirectToAction("InboxEAffiliation", "Admin");
				}


			}
			return RedirectToAction("InboxEAffiliation", "Admin");
		}

		#endregion

		#endregion

		#region  ExecuteSqlQuery
		[AdminLoginCheckFilter]
		public ActionResult ExecuteSqlQuery()
		{
			return View();
		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult ExecuteSqlQuery(string SqlQuery)
		{
			if (string.IsNullOrEmpty(SqlQuery))
			{
				return View();
			}
			else
			{
				AbstractLayer.AdminDB OBJDB = new AbstractLayer.AdminDB();
				DataSet Dresult = OBJDB.ExecuteSqlQuery(SqlQuery);
				if (Dresult.Tables[0].Rows.Count > 0)
				{
					ExportDataFromDataTable(Dresult.Tables[0], "ExportData");
				}
			}
			return View();
		}
		#endregion  ExecuteSqlQuery

		#region School to School Migration


		[AdminLoginCheckFilter]
		public ActionResult ViewAllStudentSchoolMigration()
		{
			int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
			MigrateSchoolModels MS = new MigrateSchoolModels();
			try
			{
				var itemFilter = new SelectList(new[] { new { ID = "1", Name = "School Code" }, new { ID = "2", Name = "Candidate ID" }, new { ID = "3", Name = "Candidate Name" }, new { ID = "4", Name = "Father Name" }, new { ID = "5", Name = "Mother Name" }, }, "ID", "Name", 1);
				ViewBag.MySch = itemFilter.ToList();

				ViewBag.MyDist = AbstractLayer.AdminDB.getAdminDistAllowList("admin", Convert.ToString(Session["AdminId"]));


			}
			catch (Exception ex)
			{
			}

			return View();
		}


		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult ViewAllStudentSchoolMigration(FormCollection frm, string SelDist, string SelFilter, string SearchString)
		{

			MigrateSchoolModels MS = new MigrateSchoolModels();

			int AdminId = Convert.ToInt32(Session["AdminId"].ToString());

			var itemFilter = new SelectList(new[] { new { ID = "1", Name = "New School Code" },
				new { ID = "2", Name = "Candidate ID" }, new { ID = "3", Name = "Candidate Name" },
				new { ID = "4", Name = "Father Name" }, new { ID = "5", Name = "Migration Id" }, }, "ID", "Name", 1);
			ViewBag.MySch = itemFilter.ToList();

			ViewBag.MyDist = AbstractLayer.AdminDB.getAdminDistAllowList("admin", Convert.ToString(Session["AdminId"]));



			string Search = "MigrationId like '%%' ";
			if (!string.IsNullOrEmpty(SelDist))
			{
				Search += " and DIST='" + SelDist + "' ";
			}
			if (!string.IsNullOrEmpty(SelFilter) && !string.IsNullOrEmpty(SearchString))
			{
				ViewBag.SelectedItem = SelFilter;
				int SelValueSch = Convert.ToInt32(SelFilter.ToString());
				if (SearchString != "")
				{
					if (SelValueSch == 1)
					{ Search += " and NewSCHL='" + frm["SearchString"].ToString() + "'"; }
					else if (SelValueSch == 2)
					{ Search += " and StdId='" + frm["SearchString"].ToString() + "'"; }
					else if (SelValueSch == 3)
					{ Search += " and  Name like '%" + frm["SearchString"].ToString() + "%'"; }
					else if (SelValueSch == 4)
					{ Search += " and Fname like '%" + frm["SearchString"].ToString() + "%'"; }
					else if (SelValueSch == 5)
					{ Search += " and MigrationId ='" + frm["SearchString"].ToString() + "'"; }
				}
			}

			MS.StoreAllData = new AbstractLayer.SchoolDB().StudentSchoolMigrationsSearch(3, Search, "");// 3 for admin
			if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
			{
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
			}
			else
			{
				ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
			}
			return View(MS);
		}



		[AdminLoginCheckFilter]
		public ActionResult AdminAssignStudentSchoolMigration()
		{
			int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
			MigrateSchoolModels MS = new MigrateSchoolModels();
			try
			{

				var itemFilter = new SelectList(new[]{new {ID="1",Name="Student Unique ID"},new {ID="2",Name="Reg. No"},new{ID="3",Name="Aadhar No"},
			new{ID="4",Name="Migration Id"},new{ID="5",Name="By Name"},new{ID="6",Name="FORWARDED TO PSEB"},}, "ID", "Name", 1);
				ViewBag.MyFilter = itemFilter.ToList();


				//GetAllowedGroupListBySchool
				List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDL();
				ViewBag.MyGroup = MyAcceptRejectList;

				string Search = " IsAppByHOD=1 and MigrationStatusCode>=5";

				MS.StoreAllData = new AbstractLayer.SchoolDB().StudentSchoolMigrationsSearch(3, Search, "");// 3 for admin
				if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
				}
				return View(MS);

			}
			catch (Exception ex)
			{
			}

			return View();
		}


		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult AdminAssignStudentSchoolMigration(FormCollection frm, string SelFilter, string SearchString)
		{

			MigrateSchoolModels MS = new MigrateSchoolModels();

			int AdminId = Convert.ToInt32(Session["AdminId"].ToString());

			var itemFilter = new SelectList(new[]{new {ID="1",Name="Student Unique ID"},new {ID="2",Name="Reg. No"},new{ID="3",Name="Aadhar No"},
			new{ID="4",Name="Migration Id"},new{ID="5",Name="By Name"},new{ID="6",Name="FORWARDED TO PSEB"},}, "ID", "Name", 1);
			ViewBag.MyFilter = itemFilter.ToList();

			//GetAllowedGroupListBySchool
			List<SelectListItem> MyAcceptRejectList = AbstractLayer.DBClass.GetAcceptRejectDDL();
			ViewBag.MyGroup = MyAcceptRejectList;



			string Search = " IsAppByHOD=1 and MigrationStatusCode>= 5 ";
			if (!string.IsNullOrEmpty(SelFilter) && !string.IsNullOrEmpty(SearchString))
			{
				ViewBag.SelectedItem = SelFilter;
				int SelValueSch = Convert.ToInt32(SelFilter.ToString());
				if (SearchString != "")
				{
					if (SelValueSch == 1)
					{ Search += " and StdId='" + frm["SearchString"].ToString() + "'"; }
					else if (SelValueSch == 2)
					{ Search += " and  Registration_num ='" + frm["SearchString"].ToString() + "'"; }
					else if (SelValueSch == 3)
					{ Search += " and Aadhar_num ='" + frm["SearchString"].ToString() + "'"; }
					else if (SelValueSch == 4)
					{ Search += " and MigrationId ='" + frm["SearchString"].ToString() + "'"; }
					else if (SelValueSch == 5)
					{ Search += " and Name like '%" + frm["SearchString"].ToString() + "%'"; }
					else if (SelValueSch == 6)
					{ Search += " and MigrationStatusCode=5"; }
				}
			}

			MS.StoreAllData = new AbstractLayer.SchoolDB().StudentSchoolMigrationsSearch(3, Search, "");// 3 for admin
			if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
			{
				ViewBag.Message = "Record Not Found";
				ViewBag.TotalCount = 0;
			}
			else
			{
				ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
			}
			return View(MS);
		}



		#endregion School to school Migration

		#region Update MisMatch Private Photo Sign
		[AdminLoginCheckFilter]
		public ActionResult UpdateMisMatchPrivatePhotoSign(PrivateCandidateModels MS)
		{
			return View(MS);
		}


		[HttpPost]
		[AdminLoginCheckFilter]
		public ActionResult UpdateMisMatchPrivatePhotoSign(PrivateCandidateModels MS, FormCollection frm)
		{

			string AdminType = Session["AdminType"].ToString();
			int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
			string fileLocation = "";
			string filename = "";
			if (MS.file != null)
			{
				filename = Path.GetFileName(MS.file.FileName);

				DataSet ds = new DataSet();
				if (MS.file.ContentLength > 0)  //(Request.Files["file"].ContentLength > 0
				{

					string fileName1 = "MisMatchRefno" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
					string fileExtension = System.IO.Path.GetExtension(MS.file.FileName);
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
						MS.file.SaveAs(fileLocation);
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
						using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
						{
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
							string query = string.Format("Select * from [{0}]", excelSheets[0]);
							using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
							{
								dataAdapter.Fill(ds);
							}
						}
						if (ds.Tables[0].Rows.Count > 0)
						{
							string[] arrayChln = ds.Tables[0].Rows.OfType<DataRow>().Select(k => k[0].ToString()).ToArray();
							bool CheckChln = AbstractLayer.StaticDB.CheckArrayDuplicates(arrayChln);
							if (CheckChln == true)
							{
								ViewData["Result"] = "11";
								ViewBag.Message = "Duplicate REF NO";
								return View(MS);
							}
							else
							{
								string CommaSepRefno = string.Join(",", arrayChln);
								//
								string Search = " a.refno in ( " + CommaSepRefno + " )";
								DataSet dsData = new AbstractLayer.PrivateCandidateDB().AdminFPPrivateSearch(Search);
								MS.StoreAllData = dsData;
								if (dsData.Tables[0].Rows.Count > 0)
								{
									int k = 0;
									for (int i = 0; i < MS.StoreAllData.Tables[0].Rows.Count; i++)
									{

										string stdPic = "", stdSign = "";
										string filepathtosave = "", filepathtosaveSign = "";
										var filePath1 = "";
										var filePath1Sign = "";
										MS.refNo = MS.StoreAllData.Tables[0].Rows[i]["refno"].ToString().ToUpper();
										var rp = MS.StoreAllData.Tables[0].Rows[i]["rp"].ToString().ToUpper();
										var phtURL = MS.StoreAllData.Tables[0].Rows[i]["Photo_url"].ToString();
										var signURL = MS.StoreAllData.Tables[0].Rows[i]["Sign_url"].ToString();
										var cat = MS.StoreAllData.Tables[0].Rows[i]["cat"].ToString().ToUpper();
										string Oldpath = "", OldpathSign = "";

										string imgBatchPath = "Batch" + MS.refNo.Substring(3, 4);


										if (phtURL.Contains(imgBatchPath) == false)
										{
											if (rp == "R" && (phtURL.Contains("PvtPhoto/Photo") == true || phtURL.Contains("PvtPhoto/Batch0321/Photo") == true))
											{
												filePath1 = MS.imgPhoto = @"upload\Upload2020\" + phtURL;
												Oldpath = Path.Combine(Server.MapPath("~/Upload/Upload2020/" + phtURL));
											}
											else if (rp == "R" && phtURL.Contains("PvtPhoto") == false)
											{
												filePath1 = MS.imgPhoto = @"upload\Upload2020\" + phtURL;
												Oldpath = Path.Combine(Server.MapPath("~/Upload/Upload2020/" + phtURL));
											}
											else if (phtURL.Contains("PvtPhoto") == true && (phtURL.Contains("21/Photo") == true || phtURL.Contains("21/Photo") == true))
											{
												filePath1 = MS.imgPhoto = @"upload\Upload2023\" + phtURL;
												Oldpath = Path.Combine(Server.MapPath("~/Upload/Upload2023/" + phtURL));
											}
											else if (phtURL.Contains("PvtPhoto") == true && phtURL.Contains("20/Photo") == true)
											{
												filePath1 = MS.imgPhoto = @"upload\Upload2020\" + phtURL;
												Oldpath = Path.Combine(Server.MapPath("~/Upload/Upload2020/" + phtURL));
											}
											else if (rp == "O" && phtURL.Contains("PvtPhoto") == false)
											{
												filePath1 = MS.imgPhoto = @"upload\" + phtURL;
												Oldpath = Path.Combine(Server.MapPath("~/Upload/" + phtURL));
											}
											else if (cat == "SR" || cat.ToUpper() == "SD")
											{
												filePath1 = MS.imgPhoto = "~/Upload/Upload2023/" + phtURL;
												Oldpath = Path.Combine(Server.MapPath("~/Upload/Upload2023/" + phtURL));
											}

											if (filePath1 != null && filePath1 != "" && filePath1.Contains("Upload") && phtURL.Contains(imgBatchPath) == false)
											{
												var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo"), MS.refNo + "P" + ".jpg");
												string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo"));


												if (!Directory.Exists(FilepathExist))
												{
													Directory.CreateDirectory(FilepathExist);
												}
												if (System.IO.File.Exists(path))
												{
													System.IO.File.Delete(path);
												}

												if (System.IO.File.Exists(Oldpath))
												{
													System.IO.File.Copy(Oldpath, path);
													filepathtosave = "PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo/" + MS.refNo + "P" + ".jpg";
													MS.PathPhoto = filepathtosave;

													string PhotoName = MS.refNo + "P" + ".jpg";
													string type = "P";
													string UpdatePic = new AbstractLayer.PrivateCandidateDB().Updated_PrivateCandidate_PhotoSign_ByRefNo(MS.refNo, MS.PathPhoto, type);
													k++;
												}

											}

										}

										if (signURL.Contains(imgBatchPath) == false)
										{
											if (rp == "R" && (signURL.Contains("PvtPhoto/Sign") == true || signURL.Contains("PvtPhoto/Batch0321/Sign") == true))
											{
												filePath1Sign = MS.imgSign = @"upload\Upload2020\" + signURL;
												OldpathSign = Path.Combine(Server.MapPath("~/Upload/Upload2020/" + signURL));
											}
											else if (rp == "R" && signURL.Contains("PvtPhoto") == false)
											{
												filePath1Sign = MS.imgSign = @"upload\Upload2020\" + signURL;
												OldpathSign = Path.Combine(Server.MapPath("~/Upload/Upload2020/" + signURL));
											}

											else if (signURL.Contains("PvtPhoto") == true && (signURL.Contains("20/Sign") == true || signURL.Contains("21/Sign") == true))
											{
												filePath1Sign = MS.imgSign = @"upload\Upload2023\" + signURL;
												OldpathSign = Path.Combine(Server.MapPath("~/Upload/Upload2023/" + signURL));
											}
											else if (signURL.Contains("PvtPhoto") == true && signURL.Contains("19/Sign") == true)
											{
												filePath1Sign = MS.imgSign = @"upload\Upload2020\" + signURL;
												OldpathSign = Path.Combine(Server.MapPath("~/Upload/Upload2020/" + signURL));
											}
											else if (rp == "O" && signURL.Contains("PvtPhoto") == false)
											{
												filePath1Sign = MS.imgSign = @"upload\" + signURL;
												OldpathSign = Path.Combine(Server.MapPath("~/Upload/" + signURL));
											}
											if (cat == "SR" || cat == "SD")
											{
												filePath1Sign = MS.imgPhoto = "~/Upload/Upload2023/" + signURL;
												OldpathSign = Path.Combine(Server.MapPath("~/Upload/Upload2023/" + signURL));
											}
											if (signURL.Contains("Correction1819") == true)
											{
												filePath1Sign = MS.imgSign = @"upload\Upload2020\" + signURL;
												OldpathSign = Path.Combine(Server.MapPath("~/Upload/Upload2020/" + signURL));
											}

											if (filePath1Sign != null && filePath1Sign != "" && filePath1Sign.Contains("Upload") && signURL.Contains(imgBatchPath) == false)
											{
												var path = Path.Combine(Server.MapPath("~/Upload/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign"), MS.refNo + "S" + ".jpg");
												string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign"));

												if (!Directory.Exists(FilepathExist))
												{
													Directory.CreateDirectory(FilepathExist);
												}
												if (System.IO.File.Exists(path))
												{
													System.IO.File.Delete(path);
												}

												if (System.IO.File.Exists(OldpathSign))
												{
													System.IO.File.Copy(OldpathSign, path);

													//System.IO.File.Copy(filePath1, path);
													filepathtosaveSign = "PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign/" + MS.refNo + "S" + ".jpg";
													MS.PathSign = filepathtosaveSign;

													string PhotoName = MS.refNo + "S" + ".jpg";
													string type = "S";
													string UpdatePic = new AbstractLayer.PrivateCandidateDB().Updated_PrivateCandidate_PhotoSign_ByRefNo(MS.refNo, MS.PathSign, type);
												}
											}
										}
									}

									ViewData["Result"] = "1";
									ViewBag.Message = "Updated " + k;
									return View();
								}
								else
								{
									ViewData["Result"] = "0";
									ViewBag.Message = "No Data Found";
									return View();
								}
							}
						}



						return View(MS);
					}
				}
			}
			else
			{
				ViewData["Result"] = "-4";
				ViewBag.Message = "Please select .xls file only";
				return View();
			}
			return View(MS);

		}
		#endregion Update MisMatch Private Photo Sign

		#region  UnlockClusterTheoryMarks


		[AdminLoginCheckFilter]
		public ActionResult UnlockClusterTheoryMarks()
		{
			return View();

		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult UnlockClusterTheoryMarks(AdminModels AM, FormCollection frm) // HttpPostedFileBase file
		{
			string AdminType = Session["AdminType"].ToString();
			int AdminId = Convert.ToInt32(Session["AdminId"].ToString());
			try
			{

				string id = frm["Filevalue"].ToString();

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
					// string fileName1 = "ErrorMIS_" + AdminType + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

					string fileName1 = "UnlockClusterTheory_" + id.ToString().ToUpper() + '_' + AdminType + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");

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


						string CheckMis = "";
						if (ds.Tables[0].Rows.Count == 0)
						{
							ViewData["Result"] = "20";
							ViewBag.Message = "Empty Excel file";
							return View();
						}


						DataTable dtexport = new DataTable();
						var duplicates = ds.Tables[0].AsEnumerable()
							 .GroupBy(i => new { Name = i.Field<string>("SCHL") })
							 .Where(g => g.Count() > 1)
							 .Select(g => new { g.Key.Name }).ToList();
						if (duplicates.Count() > 0)
						{
							ViewData["Result"] = "11";
							ViewBag.Message = "Duplicate Record";
							return View();
						}
						CheckMis = AbstractLayer.AdminDB.CheckUnlockClusterTheoryExcel(ds, out dtexport); // REG

						if (CheckMis == "")
						{
							DataTable dt1 = ds.Tables[0];
							if (dt1.Columns.Contains("ErrStatus"))
							{
								dt1.Columns.Remove("ErrStatus");
							}
							dt1.AcceptChanges();
							int OutStatus = 0;
							string OutResult = "0";

							DataSet dtResult = AbstractLayer.AdminDB.UnlockClusterTheoryMarks(dt1, AdminId, out OutResult);// OutStatus mobile
							if (OutStatus > 0 || OutResult == "1")
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
								ExportDataFromDataTable(dtexport, id.ToString().ToUpper() + "_ErrorReport");
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
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View();
			}
			return View();
		}
		#endregion  UnlockClusterTheoryMarks


		#region  UnlockStudentPreviousYearMarks


		[AdminLoginCheckFilter]
		public ActionResult UnlockMarksPortalPanelWise()
		{
			var itemsch = new SelectList(new[] {
				new { ID = "1", Name = "Unlock Previous Year Marks" },
				new { ID = "2", Name = "Unlock Matriculation Marks for +2 Students" },
			  }, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			ViewBag.SelectedItem = "0";
			return View();

		}

		[AdminLoginCheckFilter]
		[HttpPost]
		public ActionResult UnlockMarksPortalPanelWise(AdminModels AM, FormCollection frm, string SelList) // HttpPostedFileBase file
		{
			string AdminType = Session["AdminType"].ToString();
			int AdminId = Convert.ToInt32(Session["AdminId"].ToString());

			var itemsch = new SelectList(new[] {
				new { ID = "1", Name = "Unlock Previous Year Marks" },
				new { ID = "2", Name = "Unlock Matriculation Marks for +2 Students" },
			  }, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			ViewBag.SelectedItem = "0";


			if (string.IsNullOrEmpty(SelList))
			{
				ViewData["Result"] = "15";
				ViewBag.Message = "Please Select Panel Type";
				return View();
			}
			try
			{
				string panelId = SelList;
				string panelNM = itemsch.SingleOrDefault(s => s.Value == panelId).Text.ToString().Replace(" ", "_");

				string id = frm["Filevalue"].ToString();

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

					string fileName1 = panelNM.ToUpper() + '_' + AdminType + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");

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


						string CheckMis = "";
						if (ds.Tables[0].Rows.Count == 0)
						{
							ViewData["Result"] = "20";
							ViewBag.Message = "Empty Excel file";
							return View();
						}


						#region Previous Year Marks
						if (panelId == "1")
						{
							DataTable dtexport = new DataTable();
							var duplicates = ds.Tables[0].AsEnumerable()
								 .GroupBy(i => new { Name = i.Field<string>("SCHL") })
								 .Where(g => g.Count() > 1)
								 .Select(g => new { g.Key.Name }).ToList();
							if (duplicates.Count() > 0)
							{
								ViewData["Result"] = "11";
								ViewBag.Message = "Duplicate Record";
								return View();
							}
							CheckMis = AbstractLayer.AdminDB.CheckUnlockStudentPreviousYearMarksExcel(ds, out dtexport); // REG

							if (CheckMis == "")
							{
								DataTable dt1 = ds.Tables[0];
								if (dt1.Columns.Contains("ErrStatus"))
								{
									dt1.Columns.Remove("ErrStatus");
								}
								dt1.AcceptChanges();
								int OutStatus = 0;
								string OutResult = "0";

								DataSet dtResult = AbstractLayer.AdminDB.UnlockStudentPreviousYearMarks(dt1, AdminId, out OutResult);// OutStatus mobile
								if (OutStatus > 0 || OutResult == "1")
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
									ExportDataFromDataTable(dtexport, id.ToString().ToUpper() + "_ErrorReport");
								}
								ViewData["Result"] = "-1";
								ViewBag.Message = CheckMis;
								return View();
							}
						}
						#endregion

						#region Matriculation Marks for +2 Students
						if (panelId == "2")
						{
							DataTable dtexport = new DataTable();
							var duplicates = ds.Tables[0].AsEnumerable()
								 .GroupBy(i => new { Name = i.Field<string>("SCHL") })
								 .Where(g => g.Count() > 1)
								 .Select(g => new { g.Key.Name }).ToList();
							if (duplicates.Count() > 0)
							{
								ViewData["Result"] = "11";
								ViewBag.Message = "Duplicate Record";
								return View();
							}
							CheckMis = AbstractLayer.AdminDB.CheckUnlockSeniorStudentMatricResultMarksExcel(ds, out dtexport); // REG

							if (CheckMis == "")
							{
								DataTable dt1 = ds.Tables[0];
								if (dt1.Columns.Contains("ErrStatus"))
								{
									dt1.Columns.Remove("ErrStatus");
								}
								dt1.AcceptChanges();
								int OutStatus = 0;
								string OutResult = "0";

								DataSet dtResult = AbstractLayer.AdminDB.UnlockSeniorStudentMatricResultMarks(dt1, AdminId, out OutResult);// OutStatus mobile
								if (OutStatus > 0 || OutResult == "1")
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
									ExportDataFromDataTable(dtexport, id.ToString().ToUpper() + "_ErrorReport");
								}
								ViewData["Result"] = "-1";
								ViewBag.Message = CheckMis;
								return View();
							}
						}
						#endregion
					}
					else
					{

						ViewData["Result"] = "-2";
						ViewBag.Message = "Please Upload Only .xls file only";
						return View();
					}
				}

			}
			catch (Exception ex)
			{
				////oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View();
			}
			return View();
		}
		#endregion  UnlockStudentPreviousYearMarks




		//// Dispose
		///

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_context.Dispose();
			}
			base.Dispose(disposing);
		}



		public ActionResult FinalPrintForAdmin(string schl)
		{
			try
			{

				ChallanMasterModel CM = new ChallanMasterModel();
				ViewBag.Message = "Record Not Found";
				return View();

			}
			catch (Exception ex)
			{
				oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return RedirectToAction("Logout", "Login");
			}
		}

		[HttpPost]
		public ActionResult FinalPrintForAdmin(FormCollection frm)
		{
			try
			{

				ChallanMasterModel CM = new ChallanMasterModel();
				string schl = frm["schl"];


				DataSet ds = objDB.GetFinalPrintChallan(schl);
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
				return View();
			}
		}


		public ActionResult UpdateExFile(string id)
		{
			try
			{
				AdminModels admModel = new AdminModels();
				string schl = (string)Session["schl"];


				DataSet ds = null; // objDB.GetExFileData(schl);
				admModel.StoreAllData = ds;
				if (admModel.StoreAllData == null || admModel.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.Message = "Record Not Found";
					ViewBag.TotalCount = 0;
					return View();
				}
				else
				{
					ViewBag.TotalCount = admModel.StoreAllData.Tables[0].Rows.Count;
					return View(admModel);
				}
			}
			catch (Exception ex)
			{
				oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return View();
			}

		}

		[HttpPost]
		public ActionResult UpdateExFile(AdminModels admModel, string id)
		{
			string DeoUser = null;
			string district = null;
			string fileLocation = "";
			string filename = "";
			string uid = "";
			try
			{


				if (admModel.file != null)
				{
					filename = Path.GetFileName(admModel.file.FileName);

					DataSet ds = new DataSet();
					if (admModel.file.ContentLength > 0)  //(Request.Files["file"].ContentLength > 0
					{
						string fileName1 = "EXDATA" + district + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210
						string fileExtension = System.IO.Path.GetExtension(admModel.file.FileName);
						if (fileExtension == ".xls" || fileExtension == ".xlsx")
						{
							fileLocation = Server.MapPath("~/EXUpload/" + fileName1 + fileExtension);

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
							admModel.file.SaveAs(fileLocation);
							string excelConnectionString = string.Empty;
							excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
								fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
							//connection String for xls file format.
							//if (Path.GetExtension(path).ToLower().Trim() == ".xls" && Environment.Is64BitOperatingSystem == false)
							//if (fileExtension == ".xls")
							//{
							//	excelConnectionString = "Provider=Microsoft.Jet.OLEDB.12.0;Data Source=" +
							//	fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
							//}
							////connection String for xlsx file format.
							//else if (fileExtension == ".xlsx")
							//{
							//	excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
							//	fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
							//}
							//Create Connection to Excel work book and add oledb namespace
							using (OleDbConnection excelConnection = new OleDbConnection(excelConnectionString))
							{
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
								string query = string.Format("Select * from [{0}]", excelSheets[0]);
								using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
								{
									dataAdapter.Fill(ds);
								}
							}

							DataTable dtexport;
							string CheckMis = "";
							//objDB.CheckUpdateMasterDataMis(ds, out dtexport, RP);
							if (CheckMis == "")
							{
								DataTable dt1 = ds.Tables[0];
								if (dt1.Columns.Contains("Status"))
								{
									dt1.Columns.Remove("Status");
								}
								// UpdateData
								#region UpdateData


								string ErrStatus = string.Empty;
								admModel.StoreAllData = objDB.UpdateEXFileData(ds.Tables[0], admModel.SchlCode, out ErrStatus); // UpdateMasterDataSPNew

								if (ErrStatus == "1")
								{
									ViewBag.Message = "EX File Data Updated Successfully";
									ViewData["Result"] = "1";
								}
								else
								{
									ViewBag.Message = ErrStatus;
									ViewData["Result"] = "0";

								}
								#endregion UpdateData                          
								return View(admModel);
							}
							else
							{
								//if (dtexport != null)
								//{
								//	ExportDataFromDataTable(dtexport, "Error_MasterData");
								//}
								//ViewData["Result"] = "-1";
								//ViewBag.Message = CheckMis;
								return View(admModel);
							}
						}
						else
						{

							ViewData["Result"] = "-2";
							ViewBag.Message = "Please Upload Only .xls file only";
							return View(admModel);
						}
					}

				}
				else
				{
					//ViewData["Result"] = "-4";
					// ViewBag.Message = "Please select .xls file only";
					//return View();
				}
			}
			catch (Exception ex)
			{
				oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				return View();

			}
			return View(admModel);
		}

		#region Download School Master for Firms
		public ActionResult DownloadSchoolSchoolMasterForFirm()
		{
			string district = null;
			try
			{
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}

				if (Session["UserName"] != null)
				{
					DataSet ds1 = new AbstractLayer.DEODB().DownloadSchoolMasterForFirm();
					if (ds1.Tables.Count > 0)
					{
						if (ds1.Tables[0].Rows.Count > 0)
						{
							if (ds1.Tables[0] != null)
							{
								ExportSchoolDataFromDataTable(ds1.Tables[0], Session["UserName"].ToString().ToUpper() + "_SchoolMasterList".ToUpper());
							}
							ViewData["Result"] = "1";
							return RedirectToAction("Welcome", "Admin");
						}
						else
						{
							return RedirectToAction("Index", "Admin");
						}
					}
					else
					{
						ViewBag.Message = "Data Not Found";
						ViewData["Result"] = "0";
						return RedirectToAction("Index", "Admin");
					}
				}

				else
				{ return RedirectToAction("Welcome", "Admin"); }
				return View();
			}
			catch (Exception ex)
			{
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View();
			}
		}

		public ActionResult ExportSchoolDataFromDataTable(DataTable dt, string filename)
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
						string fileName1 = filename + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";  //103_230820162209_347
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

				return RedirectToAction("Index", "Admin");
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Admin");
			}

		}

		#endregion Download School Master for Firms

		#region Download School Master for Admin
		public ActionResult DownloadSchoolSchoolMasterForAdmin()
		{
			string district = null;
			try
			{
				if (Session["UserName"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}

				if (Session["UserName"] != null)
				{
					DataSet ds1 = new AbstractLayer.DEODB().DownloadSchoolMasterForAdmin();
					if (ds1.Tables.Count > 0)
					{
						if (ds1.Tables[0].Rows.Count > 0)
						{
							if (ds1.Tables[0] != null)
							{
								ExportAdminSchoolDataFromDataTable(ds1.Tables[0], Session["UserName"].ToString().ToUpper() + "_SchoolMasterList".ToUpper());
							}
							ViewData["Result"] = "1";
							return RedirectToAction("Welcome", "Admin");
						}
						else
						{
							return RedirectToAction("Index", "Admin");
						}
					}
					else
					{
						ViewBag.Message = "Data Not Found";
						ViewData["Result"] = "0";
						return RedirectToAction("Index", "Admin");
					}
				}

				else
				{ return RedirectToAction("Welcome", "Admin"); }
				return View();
			}
			catch (Exception ex)
			{
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View();
			}
		}

		public ActionResult ExportAdminSchoolDataFromDataTable(DataTable dt, string filename)
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
						string fileName1 = filename + "_" + DateTime.Now.ToString("ddMMyyyyHHmm") + ".xls";  //103_230820162209_347
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

				return RedirectToAction("Index", "Admin");
			}
			catch (Exception ex)
			{
				return RedirectToAction("Index", "Admin");
			}

		}

		#endregion Download School Master for Firms

		#region Begin Admin Result Update MIS
		[AdminLoginCheckFilter]
		public ActionResult AdminUpdateCentreMaster()
		{
			try
			{
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				else
				{
					return View();
				}
			}
			catch (Exception)
			{

				return RedirectToAction("Index", "Admin");
			}

		}

		[HttpPost]
		[AdminLoginCheckFilter]
		public ActionResult AdminUpdateCentreMaster(DEOModel AM) // HttpPostedFileBase file
		{
			AbstractLayer.DEODB objDB = new AbstractLayer.DEODB();
			try
			{
				// firm login // dist 
				if (Session["AdminId"] == null)
				{
					return RedirectToAction("Index", "Admin");
				}
				else
				{
					//HttpContext.Session["AdminType"]
					string AdminUser = Session["AdminId"].ToString();
					int AdminId = 1;// Convert.ToInt32(Session["AdminId"].ToString());
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
						string fileName1 = "FirmResultMIS_" + AdminUser + AdminId.ToString() + '_' + DateTime.Now.ToString("ddMMyyyyHHmmss");  //MIS_201_110720161210

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
							string UserNM = Session["AdminId"].ToString();

							DataTable dt1 = ds.Tables[0];
							dt1.AcceptChanges();
							// Get Unique and  noe empty records
							// dt1 = dt1.AsEnumerable().GroupBy(x => x.Field<string>("cschl")).Select(g => g.First()).Where(r => r.ItemArray[1].ToString() != "").CopyToDataTable();

							DataTable dtexport;

							string CheckMis = objDB.CheckResultMisExcel(dt1, UserNM, out dtexport);
							if (CheckMis == "")
							{
								if (dt1.Columns.Contains("Status"))
								{
									dt1.Columns.Remove("Status");
								}

								string Result1 = "";
								string OutError = "";
								DataTable dtResult = objDB.AdminUpdateCentreMaster(dt1, AdminId, out OutError);// OutStatus mobile
								if (OutError == "1")
								{
									ViewBag.Message = "File Uploaded Successfully";
									ViewData["Result"] = "1";
								}
								else
								{
									ViewBag.Message = "File Not Uploaded Successfully : " + OutError.ToString();
									ViewData["Result"] = "0";
								}
								return View();
							}
							else
							{
								if (dtexport != null)
								{
									ExportDataFromDataTable(dtexport, "Error_ResultUpdate");
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
				//oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
				ViewData["Result"] = "-3";
				ViewBag.Message = ex.Message;
				return View();
			}
			return View();
		}


		#endregion  Admin Result Update MIS
	}
}



