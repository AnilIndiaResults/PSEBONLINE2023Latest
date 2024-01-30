using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.IO;
using System.Web.Security;
using PSEBONLINE.AbstractLayer;
using PSEBONLINE.Models;
using System.Threading.Tasks;
using PSEBONLINE.Repository;
using PSEBONLINE.Filters;
using Newtonsoft.Json;
using System.Web.Caching;
using System.Data.Entity;
using ClosedXML.Excel;

namespace PSEBONLINE.Controllers
{
	public class AgencyController : Controller
	{
		// GET: Agency
		private readonly DBContext _context = new DBContext();
		private readonly IAgencyRepository _agencyRepository;

		public AgencyController(IAgencyRepository agencyRepository)
		{
			_agencyRepository = agencyRepository;
		}

		[AllowAnonymous]
		public JsonResult GetClassSubjectByAgencyId(string cls)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];
			List<AgencyAllowSubjectModel> agencyAllowSubjectModelList = new List<AgencyAllowSubjectModel>();

			DataSet ds = _agencyRepository.ClassSubjectByAgencyId(1, AgencyLoginSession.AgencyId.ToString(), cls, "", "");

			if (ds.Tables.Count > 0)
			{
				if (ds.Tables[1].Rows.Count > 0)
				{
					foreach (DataRow dr in ds.Tables[1].Rows)
					{
						agencyAllowSubjectModelList.Add(new AgencyAllowSubjectModel { Sub = dr["Sub"].ToString(), SubNM = dr["SubNM"].ToString() });
					}
				}
			}

			return Json(agencyAllowSubjectModelList);
		}

		#region Agency Login 
		[Route("Agency")]
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
			Session.Abandon();
			Session.RemoveAll();
			try
			{
				ViewBag.SessionList = new AbstractLayer.DBClass().GetSession().ToList();
				return View();
			}
			catch (Exception ex)
			{

				return View();
			}
		}


		[Route("Agency")]
		[HttpPost]
		public async Task<ActionResult> Index(LoginModel lm)
		{
			try
			{
				AgencyLoginSession AgencyLoginSession = await _agencyRepository.CheckAgencyLogin(lm); // passing Value to _agencyRepository.from model and Type 1 For regular              
				AgencyLoginSession.CurrentSession = lm.Session;
				TempData["result"] = AgencyLoginSession.LoginStatus;
				if (AgencyLoginSession.LoginStatus == 1)
				{
					Session["AgencyLoginSession"] = AgencyLoginSession;
					return RedirectToAction("Welcome", "Agency");
				}
				return RedirectToAction("Index", "Agency");
			}
			catch (Exception ex)
			{
				TempData["result"] = "Error : " + ex.Message;
				return RedirectToAction("Index", "Agency");
			}
		}


		public ActionResult Logout()
		{
			HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
			HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			HttpContext.Response.Cache.SetNoStore();
			TempData.Clear();
			FormsAuthentication.SignOut();
			Session.Clear();
			Session.Abandon();
			return RedirectToAction("Index", "Agency");
		}

		#endregion Agency Login

		[AgencyLoginCheckFilter]
		public ActionResult Welcome(AgencyModel AgencyModel, int? page)
		{

			Printlist obj = new Printlist();
			#region Circular

			int pageIndex = 1;
			pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
			ViewBag.pagesize = pageIndex;

			string Search = string.Empty;
			Search = "Id like '%' and CircularTypes like '%7%' and Convert(Datetime,Convert(date,ExpiryDateDD))>=Convert(Datetime,Convert(date,getdate()))";


			// Cache
			DataSet dsCircular = new DataSet();
			DataSet cacheData = HttpContext.Cache.Get("AgencyCircular") as DataSet;

			if (cacheData == null)
			{
				dsCircular = new AbstractLayer.AdminDB().CircularMaster(Search, pageIndex);
				cacheData = dsCircular;
				HttpContext.Cache.Insert("AgencyCircular", cacheData, null, DateTime.Now.AddMinutes(5), Cache.NoSlidingExpiration);

			}
			else
			{
				dsCircular = cacheData;
			}
			// Cache end 

			if (dsCircular == null || dsCircular.Tables[0].Rows.Count == 0)
			{
				ViewBag.TotalCircular = 0;
			}
			else
			{
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



		[AgencyLoginCheckFilter]
		public ActionResult NSQFAssesmentDataFormat(string id)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];

			DataSet ds = _agencyRepository.GetNSQFAssesmentDataFormat(0, AgencyLoginSession.UserName, id, "");
			if (ds == null || ds.Tables[0].Rows.Count == 0)
			{
				TempData["ExportToExcelDataFromDataTable"] = null;
				ViewData["exportStatus"] = 0;
			}
			else
			{
				ViewData["exportStatus"] = 1;
				TempData["ExportToExcelDataFromDataTable"] = ds.Tables[0];
				string fileName1 = ds.Tables[0].Rows[0]["Class"].ToString();
				ExportToExcelDataFromDataTable(fileName1, "NSQFAssesment");
			}

			return RedirectToAction("Welcome", "Agency");
		}

		#region  Change_Password

		[AgencyLoginCheckFilter]
		public ActionResult Change_Password()
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];
			ViewBag.User = AgencyLoginSession.UserName.ToString();
			return View();
		}

		[AgencyLoginCheckFilter]
		[HttpPost]
		public ActionResult Change_Password(FormCollection frm)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];
			ViewBag.User = AgencyLoginSession.UserName.ToString();

			string CurrentPassword = string.Empty;
			string NewPassword = string.Empty;

			if (frm["ConfirmPassword"] != "" && frm["NewPassword"] != "")
			{
				if (frm["ConfirmPassword"].ToString() == frm["NewPassword"].ToString())
				{
					CurrentPassword = frm["CurrentPassword"].ToString();
					NewPassword = frm["NewPassword"].ToString();
					int result = _agencyRepository.ChangePassword(AgencyLoginSession.UserName.ToString(), CurrentPassword, NewPassword);
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

		#region Agency Profile
		[AgencyLoginCheckFilter]
		public ActionResult AgencyProfileDetails(TblNsqfMaster nsqfMasterModel)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];
			List<TblNsqfMaster> nsqfMasterList = new List<TblNsqfMaster>();

			DataSet PDesi = new AbstractLayer.DEODB().GetDesignation(); // passing Value to DeoClass from model            
			List<SelectListItem> PDList = new List<SelectListItem>();
			foreach (System.Data.DataRow dr in PDesi.Tables[0].Rows)
			{
				PDList.Add(new SelectListItem { Text = @dr["POSTNM"].ToString(), Value = @dr["POSTNM"].ToString() });
			}
			ViewBag.PDList = PDList;

			nsqfMasterList = _agencyRepository.TblNsqfMasterSP(1, AgencyLoginSession.AgencyId, "");
			if (nsqfMasterList == null || nsqfMasterList.Count == 0)
			{
				ViewBag.Message = "Record Dose not Exist";
				ViewBag.TotalCount = 0;
			}
			else
			{
				ViewBag.TotalCount = 1;
				nsqfMasterModel = new TblNsqfMaster()
				{
					AgencyId = nsqfMasterList[0].AgencyId,
					AgCode = nsqfMasterList[0].AgCode,
					PWD = nsqfMasterList[0].PWD,
					Sector = nsqfMasterList[0].Sector,
					AgencyNM = nsqfMasterList[0].AgencyNM,
					AgencyAdd = nsqfMasterList[0].AgencyAdd,
					AgencyMob = nsqfMasterList[0].AgencyMob,
					AgencyEmail = nsqfMasterList[0].AgencyEmail,
					IsActive = nsqfMasterList[0].IsActive,
					AllowClass = nsqfMasterList[0].AllowClass,
					AllowSubject = nsqfMasterList[0].AllowSubject,
					PNAME1 = nsqfMasterList[0].PNAME1,
					PDESI1 = nsqfMasterList[0].PDESI1,
					PMOBILE1 = nsqfMasterList[0].PMOBILE1,
					PNAME2 = nsqfMasterList[0].PNAME2,
					PDESI2 = nsqfMasterList[0].PDESI2,
					PMOBILE2 = nsqfMasterList[0].PMOBILE2,
					PNAME3 = nsqfMasterList[0].PNAME3,
					PDESI3 = nsqfMasterList[0].PDESI3,
					PMOBILE3 = nsqfMasterList[0].PMOBILE3,
					PNAME4 = nsqfMasterList[0].PNAME4,
					PDESI4 = nsqfMasterList[0].PDESI4,
					PMOBILE4 = nsqfMasterList[0].PMOBILE4,
					PNAME5 = nsqfMasterList[0].PNAME5,
					PDESI5 = nsqfMasterList[0].PDESI5,
					PMOBILE5 = nsqfMasterList[0].PMOBILE5,

				};
			}
			return View(nsqfMasterModel);
		}

		[AgencyLoginCheckFilter]
		[HttpPost]
		public ActionResult AgencyProfileDetails(TblNsqfMaster nsqfMasterModel, FormCollection frm)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];
			try
			{
				if (nsqfMasterModel != null)
				{
					nsqfMasterModel.AllowClass = AgencyLoginSession.AllowClass;
					nsqfMasterModel.AllowSubject = AgencyLoginSession.AllowSubject;
					nsqfMasterModel.UPDDATE = DateTime.Now;
					nsqfMasterModel.UserType = AgencyLoginSession.UserType;
					_context.Entry(nsqfMasterModel).State = EntityState.Modified;
					int insertedRecords = _context.SaveChanges();
					if (insertedRecords > 0)
					{
						TempData["resultProfile"] = "1";
					}
					else { TempData["resultProfile"] = "0"; }
				}
			}
			catch (Exception ex)
			{

			}

			return RedirectToAction("AgencyProfileDetails", "Agency");
		}

		#endregion




		[AgencyLoginCheckFilter]
		public ActionResult ViewAllSchoolsAllowed(AgencySchoolModelList centerSchoolModelList)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];
			//
			var itemsch = new SelectList(new[]{new {ID="1",Name="By School Code"},new{ID="2",Name="By UDISE Code"},
			new{ID="3",Name="School Name"},}, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();

			List<AgencyAllowClassModel> agencyAllowClassModelList = new List<AgencyAllowClassModel>();
			foreach (var cls1 in AgencyLoginSession.AllowClass.Split(','))
			{
				agencyAllowClassModelList.Add(new AgencyAllowClassModel { Id = cls1, Name = DBClass.GetClassNameByClass(cls1) });
			}
			centerSchoolModelList.AgencyAllowClassModels = agencyAllowClassModelList.ToList();

			List<AgencyAllowSubjectModel> agencyAllowSubjectModelList = new List<AgencyAllowSubjectModel>();
			centerSchoolModelList.AgencyAllowSubjectModels = agencyAllowSubjectModelList.ToList();
			//
			centerSchoolModelList.AgencySchoolModels = null;
			//centerSchoolModelList.AgencySchoolModels = _agencyRepository.AgencyMasterSP(4, AgencyLoginSession.AgencyId, "");
			return View(centerSchoolModelList);
		}


		[AgencyLoginCheckFilter]
		[HttpPost]
		public ActionResult ViewAllSchoolsAllowed(AgencySchoolModelList centerSchoolModelList, FormCollection frm)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];
			//
			var itemsch = new SelectList(new[]{new {ID="1",Name="By School Code"},new{ID="2",Name="By UDISE Code"},
			new{ID="3",Name="School Name"},}, "ID", "Name", 1);
			ViewBag.MySch = itemsch.ToList();
			List<AgencyAllowClassModel> agencyAllowClassModelList = new List<AgencyAllowClassModel>();
			foreach (var cls1 in AgencyLoginSession.AllowClass.Split(','))
			{
				agencyAllowClassModelList.Add(new AgencyAllowClassModel { Id = cls1, Name = DBClass.GetClassNameByClass(cls1) });
			}
			centerSchoolModelList.AgencyAllowClassModels = agencyAllowClassModelList.ToList();

			List<AgencyAllowSubjectModel> agencyAllowSubjectModelList = new List<AgencyAllowSubjectModel>();


			DataSet ds = _agencyRepository.ClassSubjectByAgencyId(1, AgencyLoginSession.AgencyId.ToString(), centerSchoolModelList.AgencyAllowClass, "", "");

			if (ds.Tables.Count > 0)
			{
				if (ds.Tables[1].Rows.Count > 0)
				{
					foreach (DataRow dr in ds.Tables[1].Rows)
					{
						agencyAllowSubjectModelList.Add(new AgencyAllowSubjectModel { Sub = dr["Sub"].ToString(), SubNM = dr["SubNM"].ToString() });
					}
				}
			}


			centerSchoolModelList.AgencyAllowSubjectModels = agencyAllowSubjectModelList.ToList();

			string search = "";
			if (frm["SelList"] != "")
			{
				search = " AgencyId='" + AgencyLoginSession.AgencyId.ToString() + "'";
				ViewBag.SelectedFilter = frm["SelList"];
				int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());
				if (frm["SearchString"].ToString() != "")
				{
					if (SelValueSch == 1)
					{ search += " and Schl='" + frm["SearchString"].ToString() + "'"; }
					else if (SelValueSch == 2)
					{ search += " and udisecode='" + frm["SearchString"].ToString() + "'"; }
					else if (SelValueSch == 4)
					{ search += " and SchlNME like '%" + frm["SearchString"].ToString() + "%'"; }
				}
			}
			DataSet ds1 = new DataSet();
			ViewBag.AgencyAllowSubject = centerSchoolModelList.AgencyAllowSubject;
			ViewBag.AgencyAllowClass = centerSchoolModelList.AgencyAllowClass;
			centerSchoolModelList.AgencySchoolModels = _agencyRepository.AgencyMasterSP(4, AgencyLoginSession.AgencyId, centerSchoolModelList.AgencyAllowClass, centerSchoolModelList.AgencyAllowSubject, search, out ds1);
			return View(centerSchoolModelList);
		}

		#region  Marks Entry Panel 

		//[Route("MarksEntryPanel/{id}/{schl}/{sub}")]
		[AgencyLoginCheckFilter]
		[AgencyAfterLoginFilter]
		public ActionResult MarksEntryPanel(string id, string schl, string sub, int? page)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];
			SchoolModels MS = new SchoolModels();
			ViewBag.cid = id;
			ViewBag.schlCode = schl;
			ViewBag.sub = sub;
			//string cls = id == "Primary" ? "5" : id == "Middle" ? "8" : "";
			string cls = DBClass.GetClassByName(id);
			ViewBag.Class = ViewBag.SelClass = cls.ToString();
			var itemFilter = new SelectList(new[] { new { ID = "1", Name = "Stdid/Refno" }, new { ID = "2", Name = "Roll No" },
					new { ID = "3", Name = "RegNo" }, new { ID = "4", Name = "Name" },  }, "ID", "Name", 1);
			ViewBag.MyFilter = itemFilter.ToList();
			ViewBag.SelectedFilter = "0";

			var itemAction = new SelectList(new[] { new { ID = "1", Name = "Pending" }, new { ID = "2", Name = "Filled" }, }, "ID", "Name", 1);
			ViewBag.MyAction = itemAction.ToList();
			ViewBag.SelectedAction = "0";

			#region  Check School Allow For MarksEntry
			// SchoolAllowForMarksEntry schoolAllowForMarksEntry = _agencyRepository.SchoolAllowForMarksEntry(loginSession.SCHL, cls);
			#endregion  Check School Allow For MarksEntry

			int pageIndex = 1;
			pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
			ViewBag.pagesize = pageIndex;

			string Search = string.Empty;
			string SelectedAction = "0";
			Search = " Reg.SCHL='" + schl + "' ";

			//MS.StoreAllData = _agencyRepository.GetNSQFMarksEntryDataBySCHL(Search, sub, AgencyLoginSession.UserName, pageIndex, cls, Convert.ToInt32(SelectedAction), schl);
			MS.StoreAllData = _agencyRepository.GetNSQFMarksEntryDataBySCHL(Search, sub, schl, pageIndex, cls, Convert.ToInt32(SelectedAction), schl);
			if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
			{
				ViewBag.LastDateofSub = null;
				ViewBag.Message = "Record Not Found";
				ViewBag.Unlocked = ViewBag.fsCount = ViewBag.TotalCount1 = ViewBag.TotalCount = 0;
			}
			else
			{
				ViewBag.LastDateofSub = MS.StoreAllData.Tables[0].Rows[0]["LastDate"];
				ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
				ViewBag.Unlocked = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["Unlocked"].ToString());

				ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcent"].ToString());
				ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcentNM"].ToString());

				ViewBag.fsCount = Convert.ToInt32(MS.StoreAllData.Tables[2].Rows[0]["fsCount"]);

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


			}

			if (MS.StoreAllData.Tables[2].Rows.Count > 0)
			{
				ViewBag.fsCount = Convert.ToInt32(MS.StoreAllData.Tables[2].Rows[0]["fsCount"]);
			}
			if (MS.StoreAllData.Tables[3].Rows.Count > 0)
			{
				ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcent"].ToString());
				ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcentNM"].ToString());

				ViewBag.SubCode = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["sub"].ToString());
				ViewBag.SubName = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["subnm"].ToString());

				ViewBag.PrMin = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["prmin"].ToString());
				ViewBag.PrMax = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["prmax"].ToString());
			}

			if (MS.StoreAllData.Tables[4].Rows.Count > 0)
			{
				MS.schoolAllotedToAgency = new SchoolAllotedToAgency()
				{
					IsActive = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["IsActive"].ToString()),
					LastDate = Convert.ToDateTime(MS.StoreAllData.Tables[4].Rows[0]["LastDate"].ToString()),
					SCHLNME = Convert.ToString(MS.StoreAllData.Tables[4].Rows[0]["schlnme"].ToString()),
					Schl = Convert.ToString(MS.StoreAllData.Tables[4].Rows[0]["SCHL"].ToString())
				};
			}


			return View(MS);
		}


		//[Route("MarksEntryPanel/{id}/{schl}/{sub}")]
		[AgencyLoginCheckFilter]
		[AgencyAfterLoginFilter]
		[HttpPost]
		public ActionResult MarksEntryPanel(string id, string schl, string sub, int? page, FormCollection frm)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];
			SchoolModels MS = new SchoolModels();
			ViewBag.cid = id;
			ViewBag.schlCode = schl;
			ViewBag.sub = sub;
			// string cls = id == "Primary" ? "5" : id == "Middle" ? "8" : "";
			string cls = DBClass.GetClassByName(id);
			ViewBag.Class = ViewBag.SelClass = cls.ToString();

			var itemFilter = new SelectList(new[] { new { ID = "1", Name = "Stdid/Refno" }, new { ID = "2", Name = "Roll No" },
					new { ID = "3", Name = "RegNo" }, new { ID = "4", Name = "Name" },  }, "ID", "Name", 1);
			ViewBag.MyFilter = itemFilter.ToList();
			ViewBag.SelectedFilter = "0";

			var itemAction = new SelectList(new[] { new { ID = "1", Name = "Pending" }, new { ID = "2", Name = "Filled" }, }, "ID", "Name", 1);
			ViewBag.MyAction = itemAction.ToList();
			ViewBag.SelectedAction = "0";

			if (AgencyLoginSession.SchoolAllows != null && schl != "")
			{
				int pageIndex = 1;
				pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				ViewBag.pagesize = pageIndex;
				string Search = string.Empty;
				Search = " Reg.SCHL='" + schl + "' ";

				int SelAction = 0;
				if (frm["SelAction"] != "")
				{
					int SelValueSch = Convert.ToInt32(frm["SelAction"].ToString());
					if (frm["SelAction"] != "")
					{
						if (SelValueSch == 1)
						{ Search += " and isnull(OBTMARKSP,'')='' "; }
						else if (SelValueSch == 2)
						{ Search += " and OBTMARKSP is not null and PracFlg=1 and FPLot is null "; }
					}
					ViewBag.SelectedAction = frm["SelAction"];
				}

				if (frm["SelFilter"] != "")
				{
					ViewBag.SelectedFilter = frm["SelFilter"];
					int SelValueSch = Convert.ToInt32(frm["SelFilter"].ToString());
					if (frm["SelFilter"] != "" && frm["SearchString"].ToString() != "")
					{
						if (SelValueSch == 1)
						{ Search += " and reg.std_id='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 2)
						{ Search += " and reg.roll='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 3)
						{ Search += " and reg.regno='" + frm["SearchString"].ToString() + "'"; }
						else if (SelValueSch == 4)
						{ Search += " and reg.name like '%" + frm["SearchString"].ToString() + "%'"; }
					}
				}

				TempData["SelFilter"] = frm["SelFilter"];
				TempData["SelAction"] = SelAction;
				TempData["SelActionValue"] = frm["SelAction"];
				TempData["AgencySearch"] = Search;
				// string class1 = "4";
				MS.StoreAllData = _agencyRepository.GetNSQFMarksEntryDataBySCHL(Search, sub, AgencyLoginSession.UserName, pageIndex, cls, SelAction, schl);
				if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
				{
					ViewBag.LastDateofSub = null;
					ViewBag.Message = "Record Not Found";
					ViewBag.Unlocked = ViewBag.fsCount = ViewBag.TotalCount1 = ViewBag.TotalCount = 0;
				}
				else
				{
					ViewBag.LastDateofSub = MS.StoreAllData.Tables[0].Rows[0]["LastDate"];
					ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
					ViewBag.Unlocked = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["Unlocked"].ToString());

					ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcent"].ToString());
					ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcentNM"].ToString());

					ViewBag.fsCount = Convert.ToInt32(MS.StoreAllData.Tables[2].Rows[0]["fsCount"]);

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


				}

				if (MS.StoreAllData.Tables[2].Rows.Count > 0)
				{
					ViewBag.fsCount = Convert.ToInt32(MS.StoreAllData.Tables[2].Rows[0]["fsCount"]);
				}
				if (MS.StoreAllData.Tables[3].Rows.Count > 0)
				{
					ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcent"].ToString());
					ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["pcentNM"].ToString());

					ViewBag.SubCode = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["sub"].ToString());
					ViewBag.SubName = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["subnm"].ToString());

					ViewBag.PrMin = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["prmin"].ToString());
					ViewBag.PrMax = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["prmax"].ToString());
				}

				if (MS.StoreAllData.Tables[4].Rows.Count > 0)
				{
					MS.schoolAllotedToAgency = new SchoolAllotedToAgency()
					{
						IsActive = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["IsActive"].ToString()),
						LastDate = Convert.ToDateTime(MS.StoreAllData.Tables[4].Rows[0]["LastDate"].ToString()),
						SCHLNME = Convert.ToString(MS.StoreAllData.Tables[4].Rows[0]["schlnme"].ToString()),
						Schl = Convert.ToString(MS.StoreAllData.Tables[4].Rows[0]["SCHL"].ToString())
					};
				}

			}

			return View(MS);
		}



		[HttpPost]
		public JsonResult JqPracExamEnterMarks(string SelClass, string RP, string CandSubjectPrac)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];

			var flag = 1;

			// CandSubject myObj = JsonConvert.DeserializeObject<CandSubject>(CandSubject);
			var objResponse1 = JsonConvert.DeserializeObject<List<CandSubjectPrac>>(CandSubjectPrac);
			DataTable dtSub = new DataTable();
			dtSub.Columns.Add("CANDID");
			dtSub.Columns.Add("SUB");
			dtSub.Columns.Add("OBTMARKSP");
			dtSub.Columns.Add("MINMARKSP");
			dtSub.Columns.Add("MAXMARKSP");
			dtSub.Columns.Add("PRACDATE");
			dtSub.Columns.Add("ACCEPT");

			DataRow row = null;
			foreach (var rowObj in objResponse1)
			{
				rowObj.OBTMARKSP = rowObj.OBTMARKSP == null ? "" : rowObj.OBTMARKSP;

				row = dtSub.NewRow();
				if (rowObj.OBTMARKSP.ToUpper() == "A" || rowObj.OBTMARKSP.ToUpper() == "ABS")
				{
					rowObj.OBTMARKSP = "ABS";
				}
				else if (rowObj.OBTMARKSP.ToUpper() == "C" || rowObj.OBTMARKSP.ToUpper() == "CAN")
				{
					rowObj.OBTMARKSP = "CAN";
				}
				else if (rowObj.OBTMARKSP.ToUpper() == "U" || rowObj.OBTMARKSP.ToUpper() == "UMC")
				{
					rowObj.OBTMARKSP = "UMC";
				}
				else if (rowObj.OBTMARKSP.ToUpper() == "H" || rowObj.OBTMARKSP.ToUpper() == "HHH")
				{
					rowObj.OBTMARKSP = "HHH";
				}
				else if (rowObj.OBTMARKSP != "")
				{
					rowObj.OBTMARKSP = rowObj.OBTMARKSP.PadLeft(3, '0');
				}

				if (rowObj.PRACDATE != "" && rowObj.OBTMARKSP != "" && rowObj.ACCEPT.ToString().ToLower() == "true")
				{
					dtSub.Rows.Add(rowObj.CANDID, rowObj.SUB, rowObj.OBTMARKSP, rowObj.MINMARKSP, rowObj.MAXMARKSP, rowObj.PRACDATE);
				}
			}
			dtSub.AcceptChanges();


			foreach (DataRow dr1 in dtSub.Rows)
			{


				if (dr1["OBTMARKSP"].ToString() == "" || dr1["OBTMARKSP"].ToString() == "HHH" || dr1["OBTMARKSP"].ToString() == "ABS" || dr1["OBTMARKSP"].ToString() == "CAN" || dr1["OBTMARKSP"].ToString() == "UMC")
				{ }
				else if (dr1["OBTMARKSP"].ToString() == "0" || dr1["OBTMARKSP"].ToString().ToUpper().Contains("A") || dr1["OBTMARKSP"].ToString().ToUpper().Contains("C") || dr1["OBTMARKSP"].ToString().ToUpper().Contains("U"))
				{
					flag = -1;
					var results = new
					{
						status = flag
					};
					return Json(results);
				}
				else
				{
					int obt = Convert.ToInt32(dr1["OBTMARKSP"].ToString());
					int min = Convert.ToInt32(dr1["MINMARKSP"].ToString());
					int max = Convert.ToInt32(dr1["MAXMARKSP"].ToString());

					if ((obt < 1) || (obt > max))
					{
						flag = -2;
					}
				}
			}
			if (flag == 1 && dtSub.Rows.Count > 0)
			{
				if (dtSub.Columns.Contains("ACCEPT"))
				{
					dtSub.Columns.Remove("ACCEPT");
				}
				string dee = "1";
				int OutStatus = 0;
				string OutError = string.Empty;
				RP = "R";
				dee = _agencyRepository.AllotNSQFMarksEntry(AgencyLoginSession.UserName, RP, dtSub, SelClass, out OutStatus, out OutError);
				var results = new
				{
					status = OutError
				};
				return Json(results);
			}
			else
			{
				var results = new
				{
					status = flag
				};
				return Json(results);
			}

			//  return Json(new { dee = dee }, JsonRequestBehavior.AllowGet);
			// Do Stuff
		}

		#endregion

		#region ViewNSQFPracExamFinalSubmit

		[AgencyLoginCheckFilter]
		[AgencyAfterLoginFilter]
		public ActionResult ViewNSQFPracExamFinalSubmit(string id, string schl, string sub, int? page)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];
			SchoolModels MS = new SchoolModels();
			try
			{
				string SelClass = "";
				string RP = "";
				string Cent = "";
				string SubCode = sub;
				string SelectedStatus = "0";
				string cls = DBClass.GetClassByName(id);
				if (id != null)
				{
					ViewBag.cid = id;
					ViewBag.schlCode = schl;
					ViewBag.Class = ViewBag.SelClass = SelClass = cls.ToString();
					ViewBag.RP = ViewBag.SelRP = RP = "R";
					ViewBag.SelCent = Cent = schl.ToString();
				}


				//------------------------

				if (schl != null)
				{
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;

					string Search = "Reg.schl='" + schl + "'  and FPLot is null and OBTMARKSP is not null and PracFlg=1 ";
					Search += " and sb.pcent = '" + AgencyLoginSession.UserName + "'";

					string SelectedAction = "0";
					if (TempData["ViewPracExamFinalSubmitSearch"] != null)
					{
						Search = TempData["ViewPracExamFinalSubmitSearch"].ToString();
						TempData["ViewPracExamFinalSubmitSearch"] = Search;
					}


					MS.StoreAllData = _agencyRepository.ViewNSQFMarksEntryFinalSubmit(SelClass, RP, AgencyLoginSession.UserName, Search, Convert.ToInt32(SelectedStatus), pageIndex, SubCode, schl);
					if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.LastDateofSub = null;
						ViewBag.Message = "Record Not Found";
						ViewBag.Unlocked = ViewBag.TotalCount1 = ViewBag.TotalCount = 0;

					}
					else
					{
						ViewBag.LastDateofSub = MS.StoreAllData.Tables[0].Rows[0]["LastDate"];
						ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
						ViewBag.Unlocked = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["Unlocked"].ToString());

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


					}

					if (MS.StoreAllData.Tables[2].Rows.Count > 0)
					{
						ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcent"].ToString());
						ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcentNM"].ToString());

						ViewBag.SubCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["sub"].ToString());
						ViewBag.SubName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["subnm"].ToString());

						ViewBag.PrMin = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmin"].ToString());
						ViewBag.PrMax = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmax"].ToString());
					}

					if (MS.StoreAllData.Tables[3].Rows.Count > 0)
					{
						MS.schoolAllotedToAgency = new SchoolAllotedToAgency()
						{
							IsActive = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsActive"].ToString()),
							LastDate = Convert.ToDateTime(MS.StoreAllData.Tables[3].Rows[0]["LastDate"].ToString()),
							SCHLNME = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["schlnme"].ToString()),
							Schl = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["SCHL"].ToString())
						};
					}
					return View(MS);
				}
			}
			catch (Exception ex)
			{
				//return RedirectToAction("Index", "Login");
			}
			return View(MS);
		}


		[AgencyLoginCheckFilter]
		[AgencyAfterLoginFilter]
		[HttpPost]
		public ActionResult ViewNSQFPracExamFinalSubmit(string id, string schl, string sub, FormCollection frm, int? page)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];

			SchoolModels MS = new SchoolModels();
			try
			{
				string SelClass = "";
				string RP = "";
				string Cent = "";
				string SubCode = sub;
				string SelectedStatus = "0";
				string cls = DBClass.GetClassByName(id);
				if (id != null)
				{
					ViewBag.cid = id;
					ViewBag.schlCode = schl;
					ViewBag.Class = ViewBag.SelClass = SelClass = cls.ToString();
					ViewBag.RP = ViewBag.SelRP = RP = "R";
					ViewBag.SelCent = Cent = schl.ToString();
				}

				//------------------------



				if (schl != null)
				{
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;

					string Search = "Reg.schl='" + schl + "'  and FPLot is null and OBTMARKSP is not null and PracFlg=1 ";
					Search += " and sb.pcent = '" + AgencyLoginSession.UserName + "'";

					TempData["ViewPracExamFinalSubmitSearch"] = Search;
					//PracExamFinalSubmit(string class1, string rp, string cent, string Search,int SelectedAction, int pageNumber, string sub)

					MS.StoreAllData = _agencyRepository.ViewNSQFMarksEntryFinalSubmit(SelClass, RP, AgencyLoginSession.UserName, Search, Convert.ToInt32(SelectedStatus), pageIndex, SubCode, schl);

					if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.LastDateofSub = null;
						ViewBag.Message = "Record Not Found";
						ViewBag.Unlocked = ViewBag.TotalCount1 = ViewBag.TotalCount = 0;
					}
					else
					{
						ViewBag.LastDateofSub = MS.StoreAllData.Tables[0].Rows[0]["LastDate"];
						ViewBag.Unlocked = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["Unlocked"].ToString());

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


					}

					if (MS.StoreAllData.Tables[2].Rows.Count > 0)
					{
						ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcent"].ToString());
						ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcentNM"].ToString());

						ViewBag.SubCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["sub"].ToString());
						ViewBag.SubName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["subnm"].ToString());

						ViewBag.PrMin = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmin"].ToString());
						ViewBag.PrMax = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmax"].ToString());
					}
					if (MS.StoreAllData.Tables[3].Rows.Count > 0)
					{
						MS.schoolAllotedToAgency = new SchoolAllotedToAgency()
						{
							IsActive = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsActive"].ToString()),
							LastDate = Convert.ToDateTime(MS.StoreAllData.Tables[3].Rows[0]["LastDate"].ToString()),
							SCHLNME = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["schlnme"].ToString()),
							Schl = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["SCHL"].ToString())
						};
					}
					return View(MS);
				}
			}
			catch (Exception ex)
			{
				//return RedirectToAction("Index", "Login");
			}


			return View(MS);
		}


		[HttpPost]
		public JsonResult JqRemovePracMarks(string SelClass, string RP, string CandSubjectPrac)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];

			var flag = 1;

			// CandSubject myObj = JsonConvert.DeserializeObject<CandSubject>(CandSubject);
			var objResponse1 = JsonConvert.DeserializeObject<List<CandSubjectPrac>>(CandSubjectPrac);
			DataTable dtSub = new DataTable();
			dtSub.Columns.Add("CANDID");
			dtSub.Columns.Add("SUB");
			dtSub.Columns.Add("OBTMARKSP");
			dtSub.Columns.Add("MINMARKSP");
			dtSub.Columns.Add("MAXMARKSP");
			dtSub.Columns.Add("PRACDATE");
			dtSub.Columns.Add("ACCEPT");

			DataRow row = null;
			foreach (var rowObj in objResponse1)
			{
				rowObj.OBTMARKSP = rowObj.OBTMARKSP == null ? "" : rowObj.OBTMARKSP;

				row = dtSub.NewRow();

				if (rowObj.PRACDATE != "" && rowObj.OBTMARKSP != "" && rowObj.ACCEPT.ToString().ToLower() == "true")
				{
					dtSub.Rows.Add(rowObj.CANDID, rowObj.SUB, rowObj.OBTMARKSP, rowObj.MINMARKSP, rowObj.MAXMARKSP, rowObj.PRACDATE);
				}
			}
			dtSub.AcceptChanges();

			if (flag == 1 && dtSub.Rows.Count > 0)
			{
				if (dtSub.Columns.Contains("ACCEPT"))
				{
					dtSub.Columns.Remove("ACCEPT");
				}
				string dee = "1";
				string class1 = "4";
				int OutStatus = 0;
				string OutError = string.Empty;
				dee = _agencyRepository.RemoveNSQFPracMarks(AgencyLoginSession.UserName, RP, dtSub, SelClass, out OutStatus, out OutError);
				var results = new
				{
					status = OutError
				};
				return Json(results);
			}
			else
			{
				var results = new
				{
					status = flag
				};
				return Json(results);
			}

			//  return Json(new { dee = dee }, JsonRequestBehavior.AllowGet);
			// Do Stuff
		}

		#endregion ViewNSQFPracExamFinalSubmit

		#region PracExamRoughReport

		[AgencyLoginCheckFilter]
		[AgencyAfterLoginFilter]
		public ActionResult NSQFPracExamRoughReport(string id, string schl, string sub, int? page)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];
			SchoolModels MS = new SchoolModels();
			try
			{
				string SelClass = "";
				string RP = "";
				string Cent = "";
				string SubCode = sub;
				string SelectedStatus = "0";
				string cls = DBClass.GetClassByName(id);
				if (id != null)
				{
					ViewBag.cid = id;
					ViewBag.schlCode = schl;
					ViewBag.Class = ViewBag.SelClass = SelClass = cls.ToString();
					ViewBag.RP = ViewBag.SelRP = RP = "R";
					ViewBag.SelCent = Cent = schl.ToString();
				}


				//------------------------

				if (schl != null)
				{
					int pageIndex = 1;
					pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
					ViewBag.pagesize = pageIndex;
					string Search = " reg.schl = '" + schl + "'  ";
					// Search += " and sb.pcent = '" + AgencyLoginSession.UserName + "'";

					MS.StoreAllData = _agencyRepository.ViewNSQFMarksEntryFinalSubmit(SelClass, RP, AgencyLoginSession.UserName, Search, Convert.ToInt32(SelectedStatus), pageIndex, SubCode, schl);

					if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.TotalCount = 0;

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


					}

					if (MS.StoreAllData.Tables[2].Rows.Count > 0)
					{
						ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcent"].ToString());
						ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcentNM"].ToString());

						ViewBag.SubCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["sub"].ToString());
						ViewBag.SubName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["subnm"].ToString());

						ViewBag.PrMin = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmin"].ToString());
						ViewBag.PrMax = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmax"].ToString());
					}
					if (MS.StoreAllData.Tables[3].Rows.Count > 0)
					{
						MS.schoolAllotedToAgency = new SchoolAllotedToAgency()
						{
							IsActive = Convert.ToInt32(MS.StoreAllData.Tables[3].Rows[0]["IsActive"].ToString()),
							LastDate = Convert.ToDateTime(MS.StoreAllData.Tables[3].Rows[0]["LastDate"].ToString()),
							SCHLNME = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["schlnme"].ToString()),
							Schl = Convert.ToString(MS.StoreAllData.Tables[3].Rows[0]["SCHL"].ToString())
						};
					}
					return View(MS);
				}
			}
			catch (Exception ex)
			{
				//return RedirectToAction("Index", "Login");
			}
			return View(MS);
		}

		#endregion PracExamRoughReport


		#region PracExamFinalReport      
		[AgencyLoginCheckFilter]
		[AgencyAfterLoginFilter]
		public ActionResult NSQFPracExamFinalReport(string id, string schl, string sub, string SelLot, int? page)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];
			SchoolModels MS = new SchoolModels();
			try
			{
				string SelClass = "";
				string RP = "";
				string Cent = "";
				string SubCode = sub;
				string SelectedStatus = "0";
				string cls = DBClass.GetClassByName(id);
				if (id != null)
				{
					ViewBag.cid = id;
					ViewBag.schlCode = schl;
					ViewBag.Class = ViewBag.SelClass = SelClass = cls.ToString();
					ViewBag.RP = ViewBag.SelRP = RP = "R";
					ViewBag.SelCent = Cent = schl.ToString();
				}
				ViewBag.SelectedLot = SelLot == null ? "0" : SelLot;
				//------------------------

				if (schl != null)
				{

					string Search = "reg.schl = '" + schl + "' and FPLot is not null and OBTMARKSP is not null and PracFlg=1";
					Search += " and sb.pcent = '" + AgencyLoginSession.UserName + "'";

					if (!string.IsNullOrEmpty(SelLot))
					{
						ViewBag.SelectedLot = SelLot.ToString();
						Search += " and FPLot='" + SelLot + "'  ";
					}

					MS.StoreAllData = _agencyRepository.ViewNSQFMarksEntryFinalSubmit(SelClass, RP, AgencyLoginSession.UserName, Search, Convert.ToInt32(5), 0, SubCode, schl);

					if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
					{
						ViewBag.Message = "Record Not Found";
						ViewBag.Unlocked = ViewBag.TotalCount = 0;
					}
					else
					{
						ViewBag.TotalCount = MS.StoreAllData.Tables[0].Rows.Count;
						ViewBag.fplot = MS.StoreAllData.Tables[0].Rows[0]["fplot"].ToString();
						ViewBag.fplot2 = MS.StoreAllData.Tables[0].Rows[0]["fplot2"].ToString();
						ViewBag.PracInsDate = MS.StoreAllData.Tables[0].Rows[0]["PracInsDate"].ToString();
						ViewBag.Unlocked = Convert.ToInt32(MS.StoreAllData.Tables[0].Rows[0]["Unlocked"].ToString());

						int count = Convert.ToInt32(MS.StoreAllData.Tables[1].Rows[0]["TotalCnt"]);
						ViewBag.TotalCount1 = count;
					}
					if (MS.StoreAllData != null)
					{

						if (MS.StoreAllData.Tables[2].Rows.Count > 0)
						{
							ViewBag.CentreCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcent"].ToString());
							ViewBag.CentreName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["pcentNM"].ToString());

							ViewBag.SubCode = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["sub"].ToString());
							ViewBag.SubName = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["subnm"].ToString());

							ViewBag.PrMin = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmin"].ToString());
							ViewBag.PrMax = Convert.ToString(MS.StoreAllData.Tables[2].Rows[0]["prmax"].ToString());
						}
						if (MS.StoreAllData.Tables[3].Rows.Count > 0)
						{
							DataTable dtLot = MS.StoreAllData.Tables[3];
							// English
							List<SelectListItem> itemLot = new List<SelectListItem>();
							foreach (System.Data.DataRow dr in dtLot.Rows)
							{
								itemLot.Add(new SelectListItem { Text = @dr["fplot"].ToString().Trim(), Value = @dr["fplot"].ToString().Trim() });
							}
							ViewBag.itemLot = itemLot;
						}
						if (MS.StoreAllData.Tables[4].Rows.Count > 0)
						{
							MS.schoolAllotedToAgency = new SchoolAllotedToAgency()
							{
								IsActive = Convert.ToInt32(MS.StoreAllData.Tables[4].Rows[0]["IsActive"].ToString()),
								LastDate = Convert.ToDateTime(MS.StoreAllData.Tables[4].Rows[0]["LastDate"].ToString()),
								SCHLNME = Convert.ToString(MS.StoreAllData.Tables[4].Rows[0]["schlnme"].ToString()),
								Schl = Convert.ToString(MS.StoreAllData.Tables[4].Rows[0]["SCHL"].ToString())
							};
						}
					}
					return View(MS);
				}
			}
			catch (Exception ex)
			{
				//return RedirectToAction("Index", "Login");
			}
			return View(MS);
		}

		#endregion PracExamFinalReport


		[AgencyLoginCheckFilter]
		public ActionResult NSQFPracExamAllSchoolsFinalSubmit(string cls, string sub, AgencySchoolModelList centerSchoolModelList)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];
			centerSchoolModelList.AgencyAllowClass = cls;
			centerSchoolModelList.AgencyAllowSubject = sub;
			DataSet ds1 = new DataSet();
			centerSchoolModelList.AgencySchoolModels = _agencyRepository.AgencyMasterSP(10, AgencyLoginSession.AgencyId, centerSchoolModelList.AgencyAllowClass, centerSchoolModelList.AgencyAllowSubject, "", out ds1);
			if (ds1.Tables.Count > 0)
			{
				if (ds1.Tables[1].Rows.Count > 0)
				{
					ViewBag.NOMS = Convert.ToInt32(ds1.Tables[1].Rows[0]["NOMS"].ToString());
					ViewBag.LastDate = Convert.ToString(ds1.Tables[1].Rows[0]["LastDate"].ToString());
				}
			}

			return View(centerSchoolModelList);
		}

		[HttpPost]
		[AgencyLoginCheckFilter]
		public ActionResult NSQFPracExamAllSchoolsFinalSubmit(string cls, string sub, AgencySchoolModelList centerSchoolModelList, FormCollection fc)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];
			centerSchoolModelList.AgencyAllowClass = cls;
			centerSchoolModelList.AgencyAllowSubject = sub;
			DataSet ds1 = new DataSet();
			centerSchoolModelList.AgencySchoolModels = _agencyRepository.AgencyMasterSP(10, AgencyLoginSession.AgencyId, centerSchoolModelList.AgencyAllowClass, centerSchoolModelList.AgencyAllowSubject, "", out ds1);
			if (ds1.Tables.Count > 0)
			{
				if (ds1.Tables[1].Rows.Count > 0)
				{
					ViewBag.NOMS = Convert.ToInt32(ds1.Tables[1].Rows[0]["NOMS"].ToString());
					ViewBag.LastDate = Convert.ToString(ds1.Tables[1].Rows[0]["LastDate"].ToString());
				}

				if (ViewBag.NOMS == 0)
				{
					string resultFS = _agencyRepository.NSQFPracExamAllSchoolsFinalSubmit(centerSchoolModelList.AgencyAllowClass, AgencyLoginSession.UserName, centerSchoolModelList.AgencyAllowSubject, out string OutError);
					if (resultFS == "1")
					{
						ViewData["resultFS"] = "1";
					}
					else
					{
						ViewData["resultFS"] = resultFS;
					}
				}
			}

			return View(centerSchoolModelList);
		}


		//AgencyMarksPendingSchoolList
		[AgencyLoginCheckFilter]
		public ActionResult NSQFPracExamMarksPendingSchoolList(AgencyModel agencyModel)
		{
			AgencyLoginSession AgencyLoginSession = (AgencyLoginSession)Session["AgencyLoginSession"];
			ViewBag.TotalCount = 0;
			agencyModel.StoreAllData = _agencyRepository.NSQFPracExamMarksPendingSchoolList(0, "");
			if (agencyModel.StoreAllData.Tables.Count > 0)
			{
				if (agencyModel.StoreAllData.Tables[0].Rows.Count > 0)
				{
					ViewBag.TotalCount = agencyModel.StoreAllData.Tables[0].Rows.Count;
					TempData["ExportToExcelDataFromDataTable"] = agencyModel.StoreAllData.Tables[0];
					string fileName1 = "Export";
					ExportToExcelDataFromDataTable(fileName1, "NSQFPracExamMarksPendingSchoolList");
				}
			}


			return View(agencyModel);
		}
		public ActionResult ExportToExcelDataFromDataTable(string File, string PageType)
		{
			try
			{

				DataTable dt = new DataTable();

				if (TempData["ExportToExcelDataFromDataTable"] == null)
				{
					return RedirectToAction("Welcome", "Agency");
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

			return RedirectToAction("Welcome", "Agency");
		}


	}
}