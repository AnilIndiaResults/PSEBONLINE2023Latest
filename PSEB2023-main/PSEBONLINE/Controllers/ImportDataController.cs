using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSEBONLINE.Models;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using PSEBONLINE.Filters;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon;

namespace PSEBONLINE.Controllers
{
    [SessionCheckFilter]
    public class ImportDataController : Controller
    {
        private const string BUCKET_NAME = "psebdata";

        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.ImportDB objDB = new AbstractLayer.ImportDB();
        AbstractLayer.RegistrationDB objDBR = new AbstractLayer.RegistrationDB();
        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        public string stdPic, stdSign;


        public JsonResult BindSearchListBySelfOtherType(string SelCat) // Calling on http post (on Submit)
        {

            List<SelectListItem> _list = new List<SelectListItem>();
            if (SelCat.ToUpper().Contains("OTHER"))
            {
                var itemsch = new SelectList(new[] { new { ID = "3", Name = "Registration Number" },
                    new { ID = "7", Name = "Aadhar Number" },new { ID = "8", Name = "TC Refno No." },
                    new { ID = "4", Name = "OLD ID" }, new { ID = "5", Name = "Old Roll" }, }, "ID", "Name", 1);
                _list = itemsch.ToList();
            }
            else
            {
                var itemsch = new SelectList(new[] { new { ID = "1", Name = "ALL" }, new { ID = "2", Name = "Candidate Name" }, new { ID = "3", Name = "Registration Number" },
                    new { ID = "7", Name = "Aadhar Number" },new { ID = "8", Name = "TC Refno No." },
                    new { ID = "4", Name = "OLD ID" }, new { ID = "5", Name = "Old Roll" }, }, "ID", "Name", 1);
                _list = itemsch.ToList();
            }
            ViewBag.MySch = _list.ToList();
            return Json(_list);
        }


        // GET: ImportData
        #region  ImportDataN3Form8thPass


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ImportDataN3Form8thPass(int? page)
        {
            try
            {
                int status = objDBR.CheckSchoolAssignForm(1, Session["SCHL"].ToString());
                if (status > 0)
                {
                    if (status == 0)
                    { return RedirectToAction("Index", "Home"); }
                    else
                    {
                        DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                        if (result1.Tables[2].Rows.Count > 0)
                        {
                            if (result1.Tables[2].Rows[0]["reclock9th"].ToString() == "0")
                            {
                                return RedirectToAction("AgreeResult", "RegistrationPortal", new { Form = "M1" });
                            }
                        }
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                //
                var sessionYear = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySessionYear = sessionYear.ToList();
                ViewBag.SelectedSessionYear = "0";

                var itemsch = AbstractLayer.ImportDB.GetImportSearchListMain();
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = new SelectList(new[] { new { ID = "Self", Name = "Self School" }, new { ID = "Other", Name = "Other School" } }, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
                    if (SchoolAssign == null || SchoolAssign == "")
                    { return RedirectToAction("Index", "Login"); }
                    else
                    {
                        if (SchoolAssign.Contains(','))
                        {
                            string[] s = SchoolAssign.Split(',');
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
                    ViewBag.MySchCode = schllist;
                }


                string Search = string.Empty;
                if (TempData["ImportDataN3Form8thPasssearch"] != null)
                {
                    Search = Convert.ToString(TempData["ImportDataN3Form8thPasssearch"]);

                    ViewBag.SelectedSessionYear = TempData["ImportDataN3Form8thPassSessionYear"];
                    ViewBag.SelectedItem = TempData["ImportDataN3Form8thPassSelList"];
                    ViewBag.Searchstring = TempData["ImportDataN3Form8thPassSearchString"];
                    ViewBag.SelectedSession = TempData["ImportDataN3Form8thPassSession"];
                    //SelAllImportDataN3Form8thPass_SpN1
                    //---------------Fill Data On page Load-----------------          
                    obj.StoreAllData = objDB.SelectAllImportN3Form8thPass(ViewBag.SelectedSessionYear, schl, pageIndex, Search);  //SelAllImportDataN3Form8thPass_SpN1
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        // ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                        ViewBag.TotalCount1 = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
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
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult ImportDataN3Form8thPass(string id, int? page, Import imp, FormCollection frm, string SessionYear, string cmd, string SelList, string SearchString, string Session1, string chkImportid)
        {
            try
            {
                //
                var sessionYear = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySessionYear = sessionYear.ToList();
                ViewBag.SelectedSessionYear = "0";

                var itemsch = AbstractLayer.ImportDB.GetImportSearchListMain();
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = new SelectList(new[] { new { ID = "Self", Name = "Self School" }, new { ID = "Other", Name = "Other School" } }, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();

                string result = "";
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                // Import rm = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    // obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                string Search = "";
                if (cmd == "Search")
                {
                    if (Session1.ToString().ToUpper() == "SELF")
                    {
                        Search = " schl ='" + imp.schoolcode + "'";
                    }
                    //Change by Harpal sir  Any student can  be imported either self or other
                    //else { Search = " schl !='" + imp.schoolcode + "'"; }

                    if (SelList != "")
                    {
                        if (Search != "")
                        {
                            Search += " and ";
                        }
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SelValueSch == 1)
                        {
                            Search += "  ROLL='" + frm["SearchString"].ToString() + "'";
                        }
                        else if (SelValueSch == 2)
                        {
                            Search += "  REGNO like '%" + SearchString.ToString().Trim() + "%'";
                        }
                        else if (SelValueSch == 7)
                        {
                            Search += "  Aadhar_num ='" + SearchString.ToString().Trim() + "'";

                        }
                    }

                    ViewBag.SelectedItem = SelList;
                    ViewBag.SelectedSessionYear = SessionYear;
                    TempData["ImportDataN3Form8thPassSessionYear"] = SessionYear;
                    TempData["ImportDataN3Form8thPasssearch"] = Search;
                    TempData["ImportDataN3Form8thPassSelList"] = SelList;
                    TempData["ImportDataN3Form8thPassSearchString"] = SearchString.ToString().Trim();
                    TempData["ImportDataN3Form8thPassSession"] = Session1;
                    ViewBag.SelectedSession = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    //SelAllImportDataN3Form8thPass_SpN1
                    imp.StoreAllData = objDB.SelectAllImportN3Form8thPass(SessionYear, schl, pageIndex, Search);//SelectAllImportN3Form8thPassSP


                    if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        return View(imp);
                    }
                }
                else
                {
                    /////-----------------------Import Begins-------

                    string importToSchl = Session["SCHL"].ToString();

                    string collectId = frm["cbImportSelected"];
                    int cnt = collectId.Count(x => x == ',');
                    TempData["TotImported"] = cnt + 1;
                    string CurrentSchl = Session["SCHL"].ToString();
                    DataTable dt = new DataTable();
                    collectId = "" + collectId + "";
                    string Sub = String.Empty;

                    if (SelList != "")
                    {
                        if (Session1.ToString().ToUpper() == "SELF")
                        {
                            Search = " schl ='" + imp.schoolcode + "'";
                        }
                        //Change by Harpal sir  Any student can  be imported either self or other
                        //else { Search = " schl !='" + imp.schoolcode + "'"; }

                        if (SelList != "")
                        {
                            if (Search != "")
                            {
                                Search += " and ";
                            }
                            ViewBag.SelectedItem = SelList;
                            int SelValueSch = Convert.ToInt32(SelList.ToString());
                            if (SelValueSch == 1)
                            {
                                Search += "  ROLL='" + frm["SearchString"].ToString() + "'";
                            }
                            else if (SelValueSch == 2)
                            {
                                Search += "  REGNO like '%" + SearchString.ToString().Trim() + "%'";
                            }
                        }
                    }
                    //ImportStudent10thFailN1
                    dt = objDB.ImportN3Form8thPass(SessionYear, importToSchl, CurrentSchl, collectId, Session1);//ImportStudent10thFailN1

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Not Imported";
                        ViewData["result"] = -1;
                        return View();
                    }
                    else
                    {

                        TempData["result"] = 1;
                        ViewData["result"] = 1;
                    }

                    /////
                    if (TempData["ImportDataN3Form8thPasssearch"] != null)
                    {
                        Search = Convert.ToString(TempData["ImportDataN3Form8thPasssearch"]);
                        ViewBag.SelectedItem = TempData["ImportDataN3Form8thPassSelList"];
                        ViewBag.Searchstring = TempData["ImportDataN3Form8thPassSearchString"];
                        ViewBag.SelectedSession = TempData["ImportDataN3Form8thPassSession"];

                        //---------------Fill Data On page Load-----------------          
                        imp.StoreAllData = objDB.SelectAllImportN3Form8thPass(SessionYear, schl, pageIndex, Search);
                        if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Data Doesn't Exist";
                            ViewBag.TotalCount = 0;
                        }
                        else
                        {
                            ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                            ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                            int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                            int pn = tp / 20;
                            int cal = 20 * pn;
                            int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                            if (res >= 1)
                                ViewBag.pn = pn + 1;
                            else
                                ViewBag.pn = pn;


                        }
                    }

                    return View(imp);
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        #endregion  ImportDataN3Form8thPass


        public ActionResult ImportData9thClass()
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("AgreeResult", "RegistrationPortal", new { Form = "T1" });
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                string schl = null;
                string session = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }

                string Search = string.Empty;
                //Search = "form='N1' and result='pass' ";
                Search = "form='N1' and result in ('pass','COMPARTMENT','reappear') ";
                Import obj = new Import();
                obj.StoreAllData = objDB.SelectAll9thPass(Search);
                ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                return View(obj);
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        #region importN3Data
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ImportDataN3Class(int? page)
        {
            try
            {
                //Import im = new Import();
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    //int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    int status = objDBR.CheckSchoolAssignForm(1, Session["SCHL"].ToString()); // changed by ranjan 17oct
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("AgreeResult", "RegistrationPortal", new { Form = "T1" });
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                var schoolTypeList = new SelectList(new[] { new { ID = "Self", Name = "Self School" }, new { ID = "Other", Name = "Other School" } }, "ID", "Name", 1);
                ViewBag.MySchoolType = schoolTypeList.ToList();

                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "ALL" }, new { ID = "2", Name = "Candidate Name" }, new { ID = "3", Name = "Registration Number" },
                    new { ID = "7", Name = "Aadhar Number" },new { ID = "8", Name = "TC Refno No." },
                    new { ID = "4", Name = "OLD ID" }, new { ID = "5", Name = "Old Roll" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();




                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;

                //Search = "form in('M1','M2') and result in ('pass','COMPARTMENT','reappear') and schl='"+ schl + "' and importflag is null";
                if (TempData["ImportDataN3Classsearch"] != null)
                {
                    Search = TempData["ImportDataN3Classsearch"].ToString();
                    pageIndex = Convert.ToInt32(page);
                    obj.Session1 = TempData["ImportDataN3ClassSession"].ToString();
                    obj.SelList = TempData["ImportDataN3ClasssSelList"].ToString();


                    //---------------------------//
                    //---------------Fill Data On pageing -----------------  
                    //Search = "schl ='" + obj.schoolcode + "' and CLASS = 1";

                    // ViewBag.SelectedSession = obj.Session1;
                    // ViewBag.Searchstring = SearchString.ToString().Trim();

                    obj.StoreAllData = objDB.SelectAllImportN3thPass(Search, session, pageIndex, obj.Session1);


                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);

                        int pn = tp / 30;
                        int cal = 30 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        //return View(rm);
                        //ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        obj.chkidList = new List<ImportIDModel>();

                        ImportIDModel chk = null;
                        for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            if (obj.StoreAllData.Tables[0].Rows.Count == 1)
                                ViewBag.impid = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Selected = false;
                            obj.chkidList.Add(chk);
                        }

                        return View(obj);


                    }
                    //---------------End Fill Data On pageing-----------------
                }

                return View(obj);

            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult ImportDataN3Class(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1, string chkImportid, string SchoolType)
        {
            try
            {
                //var itemsch = new SelectList(new[] { new { ID = "1", Name = "Candidate Name" }, new { ID = "2", Name = "Registration Number" }, new { ID = "3", Name = "Unique ID" }, new { ID = "4", Name = "ALL" }, }, "ID", "Name", 1);
                //ViewBag.MySch = itemsch.ToList();
                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "ALL" }, new { ID = "2", Name = "Candidate Name" }, new { ID = "3", Name = "Registration Number" },
                    new { ID = "7", Name = "Aadhar Number" },new { ID = "8", Name = "TC Refno No." },
                    new { ID = "4", Name = "OLD ID" }, new { ID = "5", Name = "Old Roll" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();

                var schoolTypeList = new SelectList(new[] { new { ID = "Self", Name = "Self School" }, new { ID = "Other", Name = "Other School" } }, "ID", "Name", 1);
                ViewBag.MySchoolType = schoolTypeList.ToList();


                string result = "";
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                // Import rm = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    // obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                string Search = "";
                if (cmd == "Search")
                {
                    Search = " CLASS = 1 ";

                    if (SchoolType.ToString().ToUpper() == "SELF")
                    {
                        Search += " and schl ='" + imp.schoolcode + "'";
                    }

                    //  if (SearchString != "")
                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SelValueSch == 1)
                        {
                            Search += " " + SearchString.ToString().Trim();

                        }
                        else if (SelValueSch == 2)
                        {
                            if (Convert.ToInt32(Session1) > 2019)
                            {
                                Search += " and Candi_Name like '%" + SearchString.ToString().Trim() + "%'";
                            }
                            else
                            {
                                Search += " and NAME like '%" + SearchString.ToString().Trim() + "%'";
                            }

                        }
                        else if (SelValueSch == 3)
                        {
                            if (Convert.ToInt32(Session1) > 2019)
                            {
                                Search += " and registration_num like '%" + SearchString.ToString().Trim() + "%'";
                            }
                            else
                            {
                                Search += " and REGNO like '%" + SearchString.ToString().Trim() + "%'";
                            }

                        }
                        else if (SelValueSch == 4)
                        {
                            if (Convert.ToInt32(Session1) > 2019)
                            {
                                Search += " and std_ID =" + SearchString.ToString().Trim();
                            }
                            else
                            {
                                Search += " and ID =" + SearchString.ToString().Trim();
                            }

                        }
                        else if (SelValueSch == 5)
                        {
                            if (Convert.ToInt32(Session1) > 2019)
                            {
                                Search += " and Class_Roll_Num_Section =" + SearchString.ToString().Trim();
                            }
                            else
                            {
                                Search += " and Current_ClassRoll =" + SearchString.ToString().Trim();
                            }

                        }
                        else if (SelValueSch == 7)
                        {
                            if (Convert.ToInt32(Session1) > 2019)
                            {
                                Search += " and Aadhar_num ='" + SearchString.ToString().Trim() + "'";
                            }
                            else
                            {
                                Search += " and Aadhar_num ='" + SearchString.ToString().Trim() + "'";
                            }

                        }
                        else if (SelValueSch == 8)
                        {
                            Search = " TCrefno='" + SearchString.ToString().Trim() + "'";
                        }

                    }


                    TempData["ImportDataN3Classsearch"] = Search;
                    TempData["ses"] = session;
                    TempData["ImportDataN3ClasssSelList"] = SelList;
                    TempData["ImportDataN3ClassSearchString"] = SearchString.ToString().Trim();
                    TempData["ImportDataN3ClassSession"] = Session1;
                    TempData["pageIndex"] = pageIndex;

                    ViewBag.SelectedSession = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    imp.StoreAllData = objDB.SelectAllImportN3thPass(Search, session, pageIndex, Session1);
                    int cn = imp.StoreAllData.Tables[0].Rows.Count;
                    if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Records Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);

                        int pn = tp / 30;
                        int cal = 30 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        //return View(rm);
                        //ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        imp.chkidList = new List<ImportIDModel>();

                        ImportIDModel chk = null;
                        for (int i = 0; i < imp.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = imp.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            if (imp.StoreAllData.Tables[0].Rows.Count == 1)
                                ViewBag.impid = imp.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Selected = false;
                            imp.chkidList.Add(chk);
                        }

                        return View(imp);
                    }
                }
                else
                {
                    /////-----------------------Import Begins-------

                    //string importToSchl = Session["SCHL"].ToString();
                    string importToSchl = imp.schoolcode;
                    if (importToSchl.ToString() == null || importToSchl.ToString() == "0")
                    {
                        TempData["result"] = "-2";
                        return RedirectToAction("ImportDataN3Class", "ImportData");
                    }
                    string CurrentSchl = Session["SCHL"].ToString();


                    string collectId = frm["ChkCNinthClass"];
                    int cnt = collectId.Count(x => x == ',');
                    TempData["TotImported"] = cnt + 1;

                    DataTable dt = new DataTable();
                    //dt = objDB.Select_All_Pass_Data(importToSchl, collectId);
                    if (chkImportid != "")
                    {
                        dt = objDB.Import_All_Pass_Absent_N3_Data(importToSchl, CurrentSchl, collectId, Session1);
                    }

                    else
                    {

                        dt = objDB.Import_All_Pass_Absent_N3_Data(importToSchl, CurrentSchl, collectId, Session1);
                    }

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        // return RedirectToAction("ImportData10thClass", "ImportData");
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        ViewData["result"] = -1;
                        return View();
                    }
                    else
                    {
                        //-------------- Updated----------
                        // ViewData["result"] = 1;
                        TempData["result"] = 1;


                    }


                    return View();

                }
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ImportDataN3passedTCRef(int? page)
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(1, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("AgreeResult", "RegistrationPortal", new { Form = "T1" });
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                    string Search = string.Empty;
                    string SearchString = string.Empty;

                    if (TempData["ImportDataN3TCREFSearch"] != null)
                    {
                        Search = TempData["ImportDataN3TCREFSearch"].ToString();
                        SearchString = TempData["ImportDataN3TCREFSearchString"].ToString();

                        //-------------------------------------------------------Page Load Start----------                     

                        obj.StoreAllData = objDB.SelectTCREFN3Students(Search, SearchString);   //SelectTCStudents9thPassed
                        if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                            return View();
                        }
                        else
                        {

                            ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                            obj.chkidList = new List<ImportIDModel>();
                            ImportIDModel chk = null;
                            //for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                            //{
                            //    chk = new ImportIDModel();
                            //    chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
                            //    chk.Name = "chkidList[" + i + "].id";
                            //    chk.Selected = false;
                            //    obj.chkidList.Add(chk);
                            //}
                            if (TempData["SelValueSch"].ToString() == "1")
                            {
                                //  Search = " TCrefno='" + SearchString.ToString().Trim() + "'";
                                for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                                {
                                    chk = new ImportIDModel();
                                    chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
                                    chk.Name = "chkidList[" + i + "].id";
                                    chk.Selected = false;
                                    obj.chkidList.Add(chk);
                                }
                            }
                            else if (TempData["SelValueSch"].ToString() == "2" || TempData["SelValueSch"].ToString() == "3" || TempData["SelValueSch"].ToString() == "7")
                            {
                                //   Search += " registration_num ='" + SearchString.ToString().Trim() + "'";
                                for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                                {
                                    chk = new ImportIDModel();
                                    chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                                    chk.Name = "chkidList[" + i + "].id";
                                    chk.Selected = false;
                                    obj.chkidList.Add(chk);
                                }
                            }

                            else
                            {
                                for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                                {
                                    chk = new ImportIDModel();
                                    chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
                                    chk.Name = "chkidList[" + i + "].id";
                                    chk.Selected = false;
                                    obj.chkidList.Add(chk);
                                }
                            }
                            return View(obj);

                        }

                        //--------------------------------End------------------------
                    }


                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }


                return View(obj);
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [HttpPost]
        public ActionResult ImportDataN3passedTCRef(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string SearchStringschl, string SearchStringfnm)

        {
            try
            {

                string session = null;
                string schl = null;
                Import obj = new Import();
                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    imp.schoolcode = schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    ViewBag.Searchstring = SearchString;
                    ViewBag.Searchstringschl = SearchStringschl;
                    ViewBag.Searchstringfnm = SearchStringfnm;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;
                if (cmd == "Search")
                {
                    ViewBag.SelectedItem = SelList;
                    int SelValueSch = Convert.ToInt32(SelList.ToString());
                    TempData["SelValueSch"] = SelValueSch;
                    if (SelValueSch == 1)
                    {
                        // Search += " Roll ='" + SearchString.ToString().Trim() + "'";
                        Search = " TCrefno='" + SearchString.ToString().Trim() + "'";
                    }
                    else if (SelValueSch == 2)
                    {
                        //Search += " registration_num like '%" + SearchString.ToString().Trim() + "%'";
                        Search += " registration_num ='" + SearchString.ToString().Trim() + "'";
                    }
                    else if (SelValueSch == 3)
                    {
                        Search += " schl ='" + SearchStringschl.ToString().Trim() + "' and candi_name like '%" + SearchString.ToString().Trim() + "%' and father_name like '%" + SearchStringfnm.ToString().Trim() + "%'";
                    }
                    else if (SelValueSch == 7)
                    {
                        Search += " Aadhar_num ='" + SearchString.ToString().Trim() + "'";

                    }


                    TempData["ImportDataN3TCREFSearch"] = Search;
                    TempData["ImportDataN3TCREFSearchString"] = SearchString.ToString().Trim();

                    obj.StoreAllData = objDB.SelectTCREFN3Students(Search, SearchString);   //SelectTCStudents9thPassed
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {

                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        obj.chkidList = new List<ImportIDModel>();
                        ImportIDModel chk = null;

                        //int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SelValueSch == 1)
                        {
                            //  Search = " TCrefno='" + SearchString.ToString().Trim() + "'";
                            for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                            {
                                chk = new ImportIDModel();
                                chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
                                chk.Name = "chkidList[" + i + "].id";
                                chk.Selected = false;
                                obj.chkidList.Add(chk);
                            }
                        }
                        else if (SelValueSch == 2 || SelValueSch == 3 || SelValueSch == 7)
                        {
                            //   Search += " registration_num ='" + SearchString.ToString().Trim() + "'";
                            for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                            {
                                chk = new ImportIDModel();
                                chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                                chk.Name = "chkidList[" + i + "].id";
                                chk.Selected = false;
                                obj.chkidList.Add(chk);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                            {
                                chk = new ImportIDModel();
                                chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
                                chk.Name = "chkidList[" + i + "].id";
                                chk.Selected = false;
                                obj.chkidList.Add(chk);
                            }
                        }

                        return View(obj);

                    }
                }
                /////-----------------------Import Begins-------
                // string importToSchl = imp.schoolcode;
                string importToSchl = Session["SCHL"].ToString();
                if (imp.chkidList == null)
                { return RedirectToAction("ImportDataN3passedTCRef", "ImportData"); }
                int selectedList = imp.chkidList.Where(t => t.Selected).Count();


                var selchklist = imp.chkidList.Where(t => t.Selected == true);

                var selchklistComma = imp.chkidList.Where(t => t.Selected == true).Select(s => s.id);
                var collectId = string.Join(",", selchklistComma);

                DataTable dt = new DataTable();
                TempData["TotImported"] = selectedList;
                string cls = "1";

                dt = objDB.Import_TCREF_N3_Students(importToSchl, collectId, SearchString, cls); //Select_All_9ThPassed_Continue_TC(importToSchl, collectId, session);

                string Sub = String.Empty;
                if (dt == null || dt.Rows.Count == 0)
                {
                    ViewBag.Message = "Data Doesn't Exist";
                    ViewBag.TotalCount = 0;
                    TempData["result"] = -1;
                    return View();
                }
                else
                {
                    //-------------- Updated----------
                    // ViewData["result"] = 1;
                    TempData["result"] = 1;
                }

                return View();
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        #endregion ImportN3Data



        #region E1importcontroller
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ImportData10thClass(int? page)
        {
            try
            {
                //Import im = new Import();
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    //int status = objDBR.CheckSchoolAssignForm(1, Session["SCHL"].ToString()); // changed by ranjan 17oct
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "ALL" }, new { ID = "2", Name = "Candidate Name" }, new { ID = "3", Name = "Registration Number" }, new { ID = "7", Name = "Aadhar Number" }, new { ID = "4", Name = "OLD ID" }, new { ID = "5", Name = "Old Roll" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();


                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;

                if (TempData["ImportData10thClassSearch"] != null)
                {
                    Search = TempData["ImportData10thClasssearch"].ToString();
                    if (TempData["Imported"] != null)
                    {
                        if (TempData["Imported"].ToString() == "1")
                        {
                            pageIndex = Convert.ToInt32(TempData["pageIndex"].ToString());
                        }

                    }
                    else
                    {
                        pageIndex = Convert.ToInt32(page);
                    }

                    obj.Session1 = TempData["ImportData10thClassSession"].ToString();
                    obj.SelList = TempData["ImportData10thClassSelList"].ToString();


                    string schlID = schl;
                    obj.StoreAllData = objDB.SelectAll10thPass(Search, schlID, obj.Session1, pageIndex);
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);

                        int pn = tp / 30;
                        int cal = 30 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        obj.chkidList = new List<ImportIDModel>();

                        ImportIDModel chk = null;
                        for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            if (obj.StoreAllData.Tables[0].Rows.Count == 1)
                                ViewBag.impid = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Selected = false;
                            obj.chkidList.Add(chk);
                        }

                        return View(obj);


                    }
                    //---------------End Fill Data On pageing-----------------
                }

                return View();

            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult ImportData10thClass(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1, string chkImportid)
        {
            try
            {
                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "ALL" }, new { ID = "2", Name = "Candidate Name" }, new { ID = "3", Name = "Registration Number" }, new { ID = "7", Name = "Aadhar Number" }, new { ID = "4", Name = "OLD ID" }, new { ID = "5", Name = "Old Roll" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();


                string result = "";
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                // Import rm = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    // obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                string Search = "";
                if (cmd == "Search")
                {
                    Search = "schl ='" + imp.schoolcode + "' ";


                    if (Session1 != "")
                    {

                    }

                    //  if (SearchString != "")
                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SelValueSch == 1)
                        {
                            Search += " " + SearchString.ToString().Trim();

                        }
                        else if (SelValueSch == 2)
                        {
                            if (Convert.ToInt32(Session1) > 2019)
                            {
                                Search += " and Candi_Name like '%" + SearchString.ToString().Trim() + "%'";
                            }
                            else
                            {
                                Search += " and NAME like '%" + SearchString.ToString().Trim() + "%'";
                            }

                        }
                        else if (SelValueSch == 3)
                        {
                            if (Convert.ToInt32(Session1) > 2019)
                            {
                                Search += " and registration_num like '%" + SearchString.ToString().Trim() + "%'";
                            }
                            else
                            {
                                Search += " and REGNO like '%" + SearchString.ToString().Trim() + "%'";
                            }

                        }
                        else if (SelValueSch == 4)
                        {
                            if (Convert.ToInt32(Session1) > 2019)
                            {
                                Search += " and std_ID =" + SearchString.ToString().Trim();
                            }
                            else
                            {
                                Search += " and ID =" + SearchString.ToString().Trim();
                            }

                        }
                        else if (SelValueSch == 5)
                        {
                            if (Convert.ToInt32(Session1) > 2019)
                            {
                                Search += " and Class_Roll_Num_Section =" + SearchString.ToString().Trim();
                            }
                            else
                            {
                                Search += " and Current_ClassRoll =" + SearchString.ToString().Trim();
                            }

                        }
                        else if (SelValueSch == 7)
                        {
                            Search += " and Aadhar_num ='" + SearchString.ToString().Trim() + "'";

                        }
                    }


                    TempData["ImportData10thClasssearch"] = Search;
                    TempData["ses"] = session;
                    TempData["ImportData10thClassSelList"] = SelList;
                    TempData["ImportData10thClassSearchString"] = SearchString.ToString().Trim();
                    TempData["ImportData10thClassSession"] = Session1;
                    TempData["pageIndex"] = pageIndex;

                    ViewBag.SelectedSession = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    string schlID = Session["SCHL"].ToString();//imp.schoolcode;
                    imp.StoreAllData = objDB.SelectAll10thPass(Search, schlID, Session1, pageIndex);
                    int cn = imp.StoreAllData.Tables[0].Rows.Count;
                    if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Records Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);

                        int pn = tp / 30;
                        int cal = 30 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        //return View(rm);
                        //ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        imp.chkidList = new List<ImportIDModel>();

                        ImportIDModel chk = null;
                        for (int i = 0; i < imp.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = imp.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            if (imp.StoreAllData.Tables[0].Rows.Count == 1)
                                ViewBag.impid = imp.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Selected = false;
                            imp.chkidList.Add(chk);
                        }

                        return View(imp);
                    }
                }
                else
                {
                    /////-----------------------Import Begins-------

                    string importToSchl = imp.schoolcode;
                    if (importToSchl.ToString() == null || importToSchl.ToString() == "0")
                    {
                        TempData["result"] = "-2";
                        return RedirectToAction("ImportData10thClass", "ImportData");
                    }
                    string CurrentSchl = Session["SCHL"].ToString();


                    string collectId = frm["ChkCNinthClass"];
                    int cnt = collectId.Count(x => x == ',');
                    TempData["TotImported"] = cnt + 1;

                    DataTable dt = new DataTable();
                    if (chkImportid != "")
                    {
                        dt = objDB.Import10thpass_Self(importToSchl, CurrentSchl, collectId, Session1);
                    }

                    else
                    {

                        dt = objDB.Import10thpass_Self(importToSchl, CurrentSchl, collectId, Session1);
                    }

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        // return RedirectToAction("ImportData10thClass", "ImportData");
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        ViewData["result"] = -1;
                        return View();
                    }
                    else
                    {
                        //-------------- Updated----------
                        // ViewData["result"] = 1;
                        TempData["result"] = 1;
                        TempData["Imported"] = "1";
                    }


                    return View();

                }
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ImportData10thClassReappear(int? page)
        {
            try
            {
                //Import im = new Import();
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    //int status = objDBR.CheckSchoolAssignForm(1, Session["SCHL"].ToString()); // changed by ranjan 17oct
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                //var itemsch = new SelectList(new[] { new { ID = "1", Name = "Candidate Name" }, new { ID = "2", Name = "Registration Number" }, new { ID = "3", Name = "Unique ID" }, new { ID = "4", Name = "ALL" }, }, "ID", "Name", 1);
                //ViewBag.MySch = itemsch.ToList();
                //var sessionsrc = new SelectList(new[]{new {ID="2016",Name="2016"},new{ID="2015",Name="2015"},
                //new{ID="2014",Name="2014"},}, "ID", "Name", 1);

                //var sessionsrc = new SelectList(new[] {  new { ID = "2016", Name = "2016" }, new { ID = "2015", Name = "2015" }, }, "ID", "Name", 1);
                var sessionsrc = new SelectList(new[] { new { ID = "2019", Name = "2019" }, }, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();

                //var itemsch = new SelectList(new[] { new { ID = "1", Name = "ALL" }, new { ID = "2", Name = "Candidate Name" }, new { ID = "3", Name = "Registration Number" },new { ID = "7", Name = "Aadhar Number" }, new { ID = "4", Name = "OLD ID" }, new { ID = "5", Name = "Old Roll" }, }, "ID", "Name", 1);
                var itemsch = AbstractLayer.ImportDB.GetImportSearchList();
                ViewBag.MySch = itemsch.ToList();


                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;

                //Search = "form in('M1','M2') and result in ('pass','COMPARTMENT','reappear') and schl='"+ schl + "' and importflag is null";
                if (TempData["ImportData10thClassReappearSearch"] != null)
                {
                    //--------------FillData-----------// 
                    //List<SelectListItem> SelYrlist = new List<SelectListItem>();
                    ////SelYrlist.Add(new SelectListItem { Text = TempData["ImportDataN3ClasssSelList"].ToString(), Value = TempData["ImportDataN3ClasssSelList"].ToString() });
                    //SelYrlist.Add(new SelectListItem { Text = TempData["ImportDataN3ClassSession"].ToString(), Value = TempData["ImportDataN3ClassSession"].ToString() });                 
                    Search = TempData["ImportData10thClassReappearsearch"].ToString();
                    //session=TempData["ses"].ToString();
                    //pageIndex = Convert.ToInt32(TempData["pageIndex"].ToString());
                    pageIndex = Convert.ToInt32(page);
                    obj.Session1 = TempData["ImportData10thClassReappearSession"].ToString();
                    obj.SelList = TempData["ImportData10thClassReappearSelList"].ToString();


                    //---------------------------//
                    //---------------Fill Data On pageing -----------------  
                    //Search = "schl ='" + obj.schoolcode + "' and CLASS = 1";

                    // ViewBag.SelectedSession = obj.Session1;
                    // ViewBag.Searchstring = SearchString.ToString().Trim();

                    //obj.StoreAllData = objDB.SelectAll10thPass(Search, session, pageIndex, obj.Session1);
                    string schlID = schl;
                    obj.StoreAllData = objDB.SelectAll10thReappear(Search, schlID, obj.Session1, pageIndex);

                    // obj.StoreAllData = objDB.SelectAllImportN3thPass(Search, session, pageIndex, TempData["ImportDataN3ClassSession"].ToString());
                    // obj.TotalCount = objDB.SelectAllImport9thPassCount(Search, session, pageIndex, ViewBag.SelectedSession);

                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);

                        int pn = tp / 30;
                        int cal = 30 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        //return View(rm);
                        //ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        obj.chkidList = new List<ImportIDModel>();

                        ImportIDModel chk = null;
                        for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            if (obj.StoreAllData.Tables[0].Rows.Count == 1)
                                ViewBag.impid = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Selected = false;
                            obj.chkidList.Add(chk);
                        }

                        return View(obj);


                    }
                    //---------------End Fill Data On pageing-----------------
                }

                return View();

            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult ImportData10thClassReappear(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1, string chkImportid)
        {
            try
            {
                //var itemsch = new SelectList(new[] { new { ID = "1", Name = "Candidate Name" }, new { ID = "2", Name = "Registration Number" }, new { ID = "3", Name = "Unique ID" }, new { ID = "4", Name = "ALL" }, }, "ID", "Name", 1);
                //ViewBag.MySch = itemsch.ToList();

                //var sessionsrc = new SelectList(new[] {  new { ID = "2016", Name = "2016" }, new { ID = "2015", Name = "2015" }, }, "ID", "Name", 1);
                var sessionsrc = new SelectList(new[] { new { ID = "2019", Name = "2019" }, }, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();

                //var itemsch = new SelectList(new[] { new { ID = "1", Name = "ALL" }, new { ID = "2", Name = "Candidate Name" }, new { ID = "3", Name = "Registration Number" },new { ID = "7", Name = "Aadhar Number" }, new { ID = "4", Name = "OLD ID" }, new { ID = "5", Name = "Old Roll" }, }, "ID", "Name", 1);
                var itemsch = AbstractLayer.ImportDB.GetImportSearchList();
                ViewBag.MySch = itemsch.ToList();


                string result = "";
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                // Import rm = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    // obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                string Search = "";
                if (cmd == "Search")
                {
                    //Search = "schl ='" + imp.schoolcode + "' ";
                    Search = "";

                    //}                   

                    if (Session1 != "")
                    {
                        //Search += " and Year='" + Session1.ToString().Trim() + "'";
                        // imp.chkidList = null;
                        // ModelState.Clear();
                        //ModelState.Remove("ImportIDModel");
                    }

                    //  if (SearchString != "")
                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SelValueSch == 1)
                        {
                            Search += " Roll ='" + SearchString.ToString().Trim() + "'";
                        }
                        else if (SelValueSch == 2)
                        {
                            Search += " registration_num like '%" + SearchString.ToString().Trim() + "%'";
                        }
                        else if (SelValueSch == 3)
                        {
                            Search += " std_ID =" + SearchString.ToString().Trim();
                        }

                        //  ModelState.Clear();
                        //ModelState.Remove("ImportIDModel");
                        // imp.chkidList = null;

                    }


                    TempData["ImportData10thClassReappearsearch"] = Search;
                    TempData["ses"] = session;
                    TempData["ImportData10thClassReappearSelList"] = SelList;
                    TempData["ImportData10thClassReappearSearchString"] = SearchString.ToString().Trim();
                    TempData["ImportData10thClassReappearSession"] = Session1;
                    TempData["pageIndex"] = pageIndex;

                    ViewBag.SelectedSession = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    string schlID = schl;
                    imp.StoreAllData = objDB.SelectAll10thReappear(Search, schlID, Session1, pageIndex);//SelectAllImport10threappear_any_Sp
                    int cn = imp.StoreAllData.Tables[0].Rows.Count;
                    if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Records Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);

                        int pn = tp / 30;
                        int cal = 30 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        //return View(rm);
                        //ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        imp.chkidList = new List<ImportIDModel>();

                        ImportIDModel chk = null;
                        for (int i = 0; i < imp.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = imp.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            if (imp.StoreAllData.Tables[0].Rows.Count == 1)
                                ViewBag.impid = imp.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Selected = false;
                            imp.chkidList.Add(chk);
                        }

                        return View(imp);
                    }
                }
                else
                {
                    /////-----------------------Import Begins-------

                    //string importToSchl = Session["SCHL"].ToString();
                    string importToSchl = imp.schoolcode = Session["SCHL"].ToString();
                    if (importToSchl.ToString() == null || importToSchl.ToString() == "0")
                    {
                        TempData["result"] = "-2";
                        return RedirectToAction("ImportData10thClassReappear", "ImportData");
                    }
                    string CurrentSchl = Session["SCHL"].ToString();


                    string collectId = frm["ChkCNinthClass"];
                    int cnt = collectId.Count(x => x == ',');
                    TempData["TotImported"] = cnt + 1;

                    DataTable dt = new DataTable();
                    //dt = objDB.Select_All_Pass_Data(importToSchl, collectId);
                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SelValueSch == 1)
                        {
                            Search += " Roll ='" + SearchString.ToString().Trim() + "'";
                        }
                        else if (SelValueSch == 2)
                        {
                            Search += " regno like '%" + SearchString.ToString().Trim() + "%'";
                        }
                        else if (SelValueSch == 3)
                        {
                            Search += " std_ID =" + SearchString.ToString().Trim();
                        }

                        //  ModelState.Clear();
                        //ModelState.Remove("ImportIDModel");
                        // imp.chkidList = null;

                    }
                    if (chkImportid != "")
                    {
                        dt = objDB.Import10thReappear_AnySchl(importToSchl, CurrentSchl, collectId, Search, Session1);
                    }

                    else
                    {
                        dt = objDB.Import10thReappear_AnySchl(importToSchl, CurrentSchl, collectId, Search, Session1);
                    }

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        // return RedirectToAction("ImportData10thClass", "ImportData");
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        ViewData["result"] = -1;
                        return View();
                    }
                    else
                    {
                        //-------------- Updated----------
                        // ViewData["result"] = 1;
                        TempData["result"] = 1;


                    }


                    return View();

                }
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ImportData10thpassedAnySchool(int? page)
        {
            try
            {
                //Import im = new Import();
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    //int status = objDBR.CheckSchoolAssignForm(1, Session["SCHL"].ToString()); // changed by ranjan 17oct
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();

                var itemsch = AbstractLayer.ImportDB.GetImportSearchList();
                ViewBag.MySch = itemsch.ToList();


                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;

                if (TempData["ImportData10thClassReappearSearch"] != null)
                {
                    Search = TempData["ImportData10thClassReappearsearch"].ToString();
                    pageIndex = Convert.ToInt32(page);
                    obj.Session1 = TempData["ImportData10thClassReappearSession"].ToString();
                    obj.SelList = TempData["ImportData10thClassReappearSelList"].ToString();

                    string schlID = schl;
                    obj.StoreAllData = objDB.SelectAll10thPassAnySchool(Search, schlID, obj.Session1, pageIndex);

                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);

                        int pn = tp / 30;
                        int cal = 30 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;

                        obj.chkidList = new List<ImportIDModel>();

                        ImportIDModel chk = null;
                        for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = obj.StoreAllData.Tables[0].Rows[i]["Current_ClassRoll"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            if (obj.StoreAllData.Tables[0].Rows.Count == 1)
                                ViewBag.impid = obj.StoreAllData.Tables[0].Rows[i]["Current_ClassRoll"].ToString();
                            chk.Selected = false;
                            obj.chkidList.Add(chk);
                        }

                        return View(obj);


                    }
                    //---------------End Fill Data On pageing-----------------
                }

                return View();

            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult ImportData10thpassedAnySchool(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1, string chkImportid)
        {
            try
            {

                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();

                var itemsch = AbstractLayer.ImportDB.GetImportSearchList();
                ViewBag.MySch = itemsch.ToList();


                string result = "";
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                // Import rm = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    // obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                string Search = "";
                if (SelList != "")
                {
                    ViewBag.SelectedItem = SelList;
                    int SelValueSch = Convert.ToInt32(SelList.ToString());
                    if (SelValueSch == 1)
                    {
                        // Search += " Roll ='" + SearchString.ToString().Trim() + "'";
                        Search += "Roll='" + SearchString.ToString().Trim() + "'";
                    }
                    else if (SelValueSch == 2)
                    {
                        Search += " regno like '%" + SearchString.ToString().Trim() + "%'";
                    }
                    else if (SelValueSch == 3)
                    {
                        Search += " std_ID ='" + SearchString.ToString().Trim() + "'";
                    }
                    else if (SelValueSch == 7)
                    {
                        Search += "  Aadhar_num ='" + SearchString.ToString().Trim() + "'";

                    }
                }


                TempData["ImportData10thClassReappearsearch"] = Search;
                TempData["ses"] = session;
                TempData["ImportData10thClassReappearSelList"] = SelList;
                TempData["ImportData10thClassReappearSearchString"] = SearchString.ToString().Trim();
                TempData["ImportData10thClassReappearSession"] = Session1;
                TempData["pageIndex"] = pageIndex;

                ViewBag.SelectedSession = Session1;
                ViewBag.Searchstring = SearchString.ToString().Trim();
                string schlID = schl;
                imp.StoreAllData = objDB.SelectAll10thPassAnySchool(Search, schlID, Session1, pageIndex);

                if (cmd == "Search")
                {


                    if (Session1 != "")
                    {
                    }

                    int cn = imp.StoreAllData.Tables[0].Rows.Count;
                    if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Records Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);

                        int pn = tp / 30;
                        int cal = 30 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        imp.chkidList = new List<ImportIDModel>();

                        ImportIDModel chk = null;
                        for (int i = 0; i < imp.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = imp.StoreAllData.Tables[0].Rows[i]["Current_ClassRoll"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            if (imp.StoreAllData.Tables[0].Rows.Count == 1)
                                ViewBag.impid = imp.StoreAllData.Tables[0].Rows[i]["Current_ClassRoll"].ToString();
                            chk.Selected = false;
                            imp.chkidList.Add(chk);
                        }

                        return View(imp);
                    }
                }
                else
                {
                    /////-----------------------Import Begins-------
                    string importToSchl = imp.StoreAllData.Tables[0].Rows[0]["schl"].ToString();
                    if (importToSchl == null || importToSchl == "" || importToSchl == "0")
                    {
                        importToSchl = Session["SCHL"].ToString();
                    }
                    else
                    {
                        importToSchl = imp.StoreAllData.Tables[0].Rows[0]["schl"].ToString();
                    }

                    //importToSchl = Session["SCHL"].ToString();
                    if (importToSchl.ToString() == null || importToSchl.ToString() == "0")
                    {
                        TempData["result"] = "-2";
                        return RedirectToAction("ImportData10thClassReappear", "ImportData");
                    }
                    string CurrentSchl = Session["SCHL"].ToString();


                    string collectId = frm["ChkCNinthClass"];
                    int cnt = collectId.Count(x => x == ',');
                    TempData["TotImported"] = cnt + 1;

                    DataTable dt = new DataTable();
                    if (chkImportid != "")
                    {

                        dt = objDB.Import10thPass_AnySchl(importToSchl, CurrentSchl, collectId, Session1);
                    }

                    else
                    {

                        dt = objDB.Import10thPass_AnySchl(importToSchl, CurrentSchl, collectId, Session1);
                    }

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        // return RedirectToAction("ImportData10thClass", "ImportData");
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        ViewData["result"] = -1;
                        return View();
                    }
                    else
                    {
                        if (dt.Rows[0]["val"].ToString() == "2627" || dt.Rows[0]["val"].ToString() == "2601")
                        {
                            ViewBag.Message = "Paticular Data Already Exist";
                            ViewBag.TotalCount = 0;
                            ViewData["result"] = -1;
                            return View();
                        }
                        else
                        {
                            //-------------- Updated----------
                            // ViewData["result"] = 1;
                            TempData["result"] = 1;


                        }
                    }


                    return View();

                }
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ImportData11THFailedSelfSchl(int? page)
        {
            try
            {
                //Import im = new Import();
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    //int status = objDBR.CheckSchoolAssignForm(1, Session["SCHL"].ToString()); // changed by ranjan 17oct
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "ALL" }, new { ID = "2", Name = "Candidate Name" }, new { ID = "3", Name = "Registration Number" }, new { ID = "7", Name = "Aadhar Number" }, new { ID = "4", Name = "OLD ID" }, new { ID = "5", Name = "Old Roll" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();


                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;

                if (TempData["ImportData10thClassSearch"] != null)
                {
                    Search = TempData["ImportData10thClasssearch"].ToString();
                    pageIndex = Convert.ToInt32(page);
                    obj.Session1 = TempData["ImportData10thClassSession"].ToString();
                    obj.SelList = TempData["ImportData10thClassSelList"].ToString();


                    string schlID = schl;
                    obj.StoreAllData = objDB.SelectAll11thfailselfschl(Search, schlID, obj.Session1, pageIndex);
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);

                        int pn = tp / 30;
                        int cal = 30 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        obj.chkidList = new List<ImportIDModel>();

                        ImportIDModel chk = null;
                        for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            if (obj.StoreAllData.Tables[0].Rows.Count == 1)
                                ViewBag.impid = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Selected = false;
                            obj.chkidList.Add(chk);
                        }

                        return View(obj);


                    }
                    //---------------End Fill Data On pageing-----------------
                }

                return View();

            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult ImportData11THFailedSelfSchl(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1, string chkImportid)
        {
            try
            {
                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "ALL" }, new { ID = "2", Name = "Candidate Name" }, new { ID = "3", Name = "Registration Number" }, new { ID = "7", Name = "Aadhar Number" }, new { ID = "4", Name = "OLD ID" }, new { ID = "5", Name = "Old Roll" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();


                string result = "";
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                // Import rm = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    // obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                    ViewBag.MySchCode = schllist;
                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }
                string Search = "";
                if (cmd == "Search")
                {
                    Search = "schl ='" + imp.schoolcode + "' and class=3 ";

                    if (Session1 != "")
                    {

                    }

                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SelValueSch == 1)
                        {
                            Search += " " + SearchString.ToString().Trim();

                        }
                        else if (SelValueSch == 2)
                        {
                            if (Convert.ToInt32(Session1) > 2019)
                            {
                                Search += " and Candi_Name like '%" + SearchString.ToString().Trim() + "%'";
                            }
                            else
                            {
                                Search += " and NAME like '%" + SearchString.ToString().Trim() + "%'";
                            }

                        }
                        else if (SelValueSch == 3)
                        {
                            if (Convert.ToInt32(Session1) > 2019)
                            {
                                Search += " and registration_num like '%" + SearchString.ToString().Trim() + "%'";
                            }
                            else
                            {
                                Search += " and REGNO like '%" + SearchString.ToString().Trim() + "%'";
                            }

                        }
                        else if (SelValueSch == 4)
                        {
                            if (Convert.ToInt32(Session1) > 2019)
                            {
                                Search += " and std_ID =" + SearchString.ToString().Trim();
                            }
                            else
                            {
                                Search += " and ID =" + SearchString.ToString().Trim();
                            }

                        }
                        else if (SelValueSch == 5)
                        {
                            if (Convert.ToInt32(Session1) > 2019)
                            {
                                Search += " and Class_Roll_Num_Section =" + SearchString.ToString().Trim();
                            }
                            else
                            {
                                Search += " and Current_ClassRoll =" + SearchString.ToString().Trim();
                            }

                        }
                        else if (SelValueSch == 7)
                        {
                            Search += " and Aadhar_num ='" + SearchString.ToString().Trim() + "'";

                        }
                    }

                    TempData["ImportData10thClasssearch"] = Search;
                    TempData["ses"] = session;
                    TempData["ImportData10thClassSelList"] = SelList;
                    TempData["ImportData10thClassSearchString"] = SearchString.ToString().Trim();
                    TempData["ImportData10thClassSession"] = Session1;
                    TempData["pageIndex"] = pageIndex;

                    ViewBag.SelectedSession = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    string schlID = imp.schoolcode;
                    imp.StoreAllData = objDB.SelectAll11thfailselfschl(Search, schlID, Session1, pageIndex);
                    int cn = imp.StoreAllData.Tables[0].Rows.Count;
                    if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Records Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);

                        int pn = tp / 30;
                        int cal = 30 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        imp.chkidList = new List<ImportIDModel>();

                        ImportIDModel chk = null;
                        for (int i = 0; i < imp.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = imp.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            if (imp.StoreAllData.Tables[0].Rows.Count == 1)
                                ViewBag.impid = imp.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Selected = false;
                            imp.chkidList.Add(chk);
                        }

                        return View(imp);
                    }
                }
                else
                {
                    /////-----------------------Import Begins-------

                    string importToSchl = imp.schoolcode;
                    if (importToSchl.ToString() == null || importToSchl.ToString() == "0")
                    {
                        TempData["result"] = "-2";
                        return RedirectToAction("ImportDataN3Class", "ImportData");
                    }
                    string CurrentSchl = Session["SCHL"].ToString();


                    string collectId = frm["ChkCNinthClass"];
                    int cnt = collectId.Count(x => x == ',');
                    TempData["TotImported"] = cnt + 1;

                    DataTable dt = new DataTable();
                    if (chkImportid != "")
                    {
                        dt = objDB.Import11thfail_SelfSchl(importToSchl, CurrentSchl, collectId, Session1);
                    }

                    else
                    {
                        dt = objDB.Import11thfail_SelfSchl(importToSchl, CurrentSchl, collectId, Session1);
                    }

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        // return RedirectToAction("ImportData10thClass", "ImportData");
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        ViewData["result"] = -1;
                        return View();
                    }
                    else
                    {
                        TempData["result"] = 1;


                    }


                    return View();

                }
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ImportData11thfailedTCRef(int? page)
        {
            try
            {

                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                    ViewBag.MySchCode = schllist;
                    string Search = string.Empty;
                    string SearchString = string.Empty;

                    if (TempData["ImportDataN3TCREFSearch"] != null)
                    {
                        Search = TempData["ImportDataN3TCREFSearch"].ToString();
                        SearchString = TempData["ImportDataN3TCREFSearchString"].ToString();

                        //-------------------------------------------------------Page Load Start----------                     

                        obj.StoreAllData = objDB.SelectAll11thfailtcref(Search, SearchString);   //SelectTCStudents9thPassed
                        if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Record Not Found";
                            ViewBag.TotalCount = 0;
                            return View();
                        }
                        else
                        {

                            ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                            obj.chkidList = new List<ImportIDModel>();
                            ImportIDModel chk = null;
                            if (TempData["SelValueSch"].ToString() == "1")
                            {
                                //  Search = " TCrefno='" + SearchString.ToString().Trim() + "'";
                                for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                                {
                                    chk = new ImportIDModel();
                                    chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
                                    chk.Name = "chkidList[" + i + "].id";
                                    chk.Selected = false;
                                    obj.chkidList.Add(chk);
                                }
                            }
                            else if (TempData["SelValueSch"].ToString() == "2" || TempData["SelValueSch"].ToString() == "3" || TempData["SelValueSch"].ToString() == "7")
                            {
                                //   Search += " registration_num ='" + SearchString.ToString().Trim() + "'";
                                for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                                {
                                    chk = new ImportIDModel();
                                    chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                                    chk.Name = "chkidList[" + i + "].id";
                                    chk.Selected = false;
                                    obj.chkidList.Add(chk);
                                }
                            }
                            else
                            {
                                for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                                {
                                    chk = new ImportIDModel();
                                    chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
                                    chk.Name = "chkidList[" + i + "].id";
                                    chk.Selected = false;
                                    obj.chkidList.Add(chk);
                                }
                            }

                            return View(obj);

                        }

                        //--------------------------------End------------------------
                    }


                }
                else
                {
                    return RedirectToAction("Index", "Login");
                }


                return View(obj);
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [HttpPost]
        public ActionResult ImportData11thfailedTCRef(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string SearchStringschl, string SearchStringfnm)
        {
            try
            {

                string session = null;
                string schl = null;
                Import obj = new Import();
                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    imp.schoolcode = schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    ViewBag.Searchstring = SearchString;
                    ViewBag.Searchstringschl = SearchStringschl;
                    ViewBag.Searchstringfnm = SearchStringfnm;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;
                if (cmd == "Search")
                {

                    ViewBag.SelectedItem = SelList;
                    int SelValueSch = Convert.ToInt32(SelList.ToString());
                    TempData["SelValueSch"] = SelValueSch;
                    if (SelValueSch == 1)
                    {
                        Search = " TCrefno='" + SearchString.ToString().Trim() + "'";
                    }
                    else if (SelValueSch == 2)
                    {
                        Search += " registration_num ='" + SearchString.ToString().Trim() + "'";
                    }
                    else if (SelValueSch == 3)
                    {
                        Search += " schl ='" + SearchStringschl.ToString().Trim() + "' and candi_name like '%" + SearchString.ToString().Trim() + "%' and father_name like '%" + SearchStringfnm.ToString().Trim() + "%'";
                    }
                    else if (SelValueSch == 7)
                    {
                        Search += "  Aadhar_num ='" + SearchString.ToString().Trim() + "'";

                    }

                    TempData["ImportDataN3TCREFSearch"] = Search;
                    TempData["ImportDataN3TCREFSearchString"] = SearchString.ToString().Trim();

                    obj.StoreAllData = objDB.SelectAll11thfailtcref(Search, SearchString);   //SelectTCStudents9thPassed
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        obj.chkidList = new List<ImportIDModel>();
                        ImportIDModel chk = null;

                        if (SelValueSch == 1)
                        {
                            for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                            {
                                chk = new ImportIDModel();
                                chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
                                chk.Name = "chkidList[" + i + "].id";
                                chk.Selected = false;
                                obj.chkidList.Add(chk);
                            }
                        }
                        else if (SelValueSch == 2 || SelValueSch == 3 || SelValueSch == 7)
                        {
                            for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                            {
                                chk = new ImportIDModel();
                                chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                                chk.Name = "chkidList[" + i + "].id";
                                chk.Selected = false;
                                obj.chkidList.Add(chk);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                            {
                                chk = new ImportIDModel();
                                chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
                                chk.Name = "chkidList[" + i + "].id";
                                chk.Selected = false;
                                obj.chkidList.Add(chk);
                            }
                        }

                        return View(obj);

                    }
                }
                /////-----------------------Import Begins-------
                // string importToSchl = imp.schoolcode;
                string importToSchl = Session["SCHL"].ToString();
                if (imp.chkidList == null)
                { return RedirectToAction("ImportData11thfailedTCRef", "ImportData"); }
                int selectedList = imp.chkidList.Where(t => t.Selected).Count();


                var selchklist = imp.chkidList.Where(t => t.Selected == true);

                var selchklistComma = imp.chkidList.Where(t => t.Selected == true).Select(s => s.id);
                var collectId = string.Join(",", selchklistComma);

                DataTable dt = new DataTable();
                TempData["TotImported"] = selectedList;
                string cls = "3";

                dt = objDB.Import_TCREF_11thfail_Students(importToSchl, collectId, SearchString, cls); //Select_All_9ThPassed_Continue_TC(importToSchl, collectId, session);

                string Sub = String.Empty;
                if (dt == null || dt.Rows.Count == 0)
                {
                    ViewBag.Message = "Data Doesn't Exist";
                    ViewBag.TotalCount = 0;
                    TempData["result"] = -1;
                    return View();
                }
                else
                {
                    //-------------- Updated----------
                    // ViewData["result"] = 1;
                    TempData["result"] = 1;
                }

                return View();
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        #endregion E1importcontroller

        [HttpPost]
        public ActionResult ImportDataSearching(Import imp, FormCollection frm)
        {
            try
            {


                var itemsch = AbstractLayer.ImportDB.GetImportSearchListMain();
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = new SelectList(new[]{new {ID="2016",Name="2016"},new{ID="2015",Name="2015"},
            new{ID="2014",Name="2014"},}, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();

                //string sBy = frm["SelList"].ToString();
                // string sText = frm["SearchString"].ToString();

                Import rm = new Import();
                if (ModelState.IsValid)
                {

                    string Search = string.Empty;
                    string schlid = "";
                    if (Session["SCHL"] != null)
                    {
                        schlid = Session["SCHL"].ToString();
                        //session = Session["Session"].ToString();
                        //schl = Session["SCHL"].ToString();
                        //obj.schoolcode = schl;
                        List<SelectListItem> schllist = new List<SelectListItem>();

                        string SchoolAssign = "";
                        SchoolAssign = objDB.GetImpschlOcode(3, schlid);
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
                                schllist.Add(new SelectListItem { Text = schlid, Value = schlid });
                            }
                        }
                        //schllist.Add(new SelectListItem { Text = "---Select Import TO--", Value = "0" });                
                        ViewBag.MySchCode = schllist;
                    }
                    else
                    {
                        return View(rm);
                    }
                    // Search = "form_Name='E1' and schl='" + schlid + "' and a.std_id like '%' ";
                    Search = "ImportFormNameTo='E1' and schl='" + schlid + "' ";
                    if (frm["SelList"] != "")
                    {
                        ViewBag.SelectedItem = frm["SelList"];
                        int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());


                        if (frm["SearchString"] != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and a.ROLL='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += " and  a.Name like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 3)
                            { Search += " and a.FName  like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " and a.MName like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 5)
                            { Search += " and a.DOB='" + frm["SearchString"].ToString() + "'"; }
                        }


                    }

                    rm.StoreAllData = objDB.GetStudentRecordsSearch_ImportData(Search);
                    // asm.StoreAllData = objDB.SearchSchoolDetails(Search);
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                        return RedirectToAction("ImportData10thClass", rm);



                    }
                }
                else
                {
                    return RedirectToAction("ImportData10thClass", "ImportData");
                }
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }

        }
        [HttpPost]
        public ActionResult CancelForm(SchoolModels sm, FormCollection fc)
        {
            try
            {


                TempData["ImportData10thClasssearch"] = null;
                TempData["ImportData10thClassSelList"] = null;
                TempData["ImportData10thClassSearchString"] = null;
                TempData["ImportData10thClassSession"] = null;
                return RedirectToAction("ImportData10thClass", "ImportData");
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [HttpPost]
        public ActionResult CancelForm1(SchoolModels sm, FormCollection fc)
        {
            try
            {


                TempData["ImportData11THFailedsearch1"] = null;
                TempData["ImportData11THFailedSelList1"] = null;
                TempData["ImportData11THFailedSearchString1"] = null;
                TempData["ImportData11THFailedSession1"] = null;
                return RedirectToAction("ImportData11THFailed", "ImportData");
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [HttpPost]
        public ActionResult CancelForm2(SchoolModels sm, FormCollection fc)
        {
            try
            {


                TempData["ImportDataViewAllRecordssearch2"] = null;
                TempData["ImportDataViewAllRecordsSelList2"] = null;
                TempData["ImportDataViewAllRecordsSession2"] = null;
                TempData["ImportDataViewAllRecordsSearchString2"] = null;
                return RedirectToAction("ImportDataViewAllRecords", "ImportData");
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [HttpPost]
        public ActionResult CancelForm3(SchoolModels sm, FormCollection fc)
        {
            try
            {


                TempData["ImportData10thpassed141516search3"] = null;
                TempData["ImportData10thpassed141516SelList3"] = null;
                TempData["ImportData10thpassed141516SearchString3"] = null;
                TempData["ImportData10thpassed141516Session3"] = null;
                return RedirectToAction("ImportData10thpassed141516", "ImportData");
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [HttpPost]
        public ActionResult CancelImport2017(SchoolModels sm, FormCollection fc)
        {
            try
            {
                TempData["search3"] = null;
                TempData["SelList3"] = null;
                TempData["SearchString3"] = null;
                TempData["Session3"] = null;
                return RedirectToAction("ImportedData2017", "ImportData");
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }



        [HttpPost]
        public ActionResult CancelForm4(SchoolModels sm, FormCollection fc)
        {
            try
            {


                TempData["ImportDataViewTo11thClasssearch4"] = null;
                TempData["ImportDataViewTo11thClassSelList4"] = null;
                TempData["ImportDataViewTo11thClassSearchString4"] = null;
                TempData["ImportDataViewTo11thClassSession4"] = null;
                return RedirectToAction("ImportDataViewTo11thClass", "ImportData");
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [HttpPost]
        public ActionResult CancelForm5(SchoolModels sm, FormCollection fc)
        {
            try
            {


                TempData["search5"] = null;
                TempData["SelList5"] = null;
                TempData["SearchString5"] = null;
                TempData["Session5"] = null;
                return RedirectToAction("ImportDataViewTo11thFailed", "ImportData");
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        public ActionResult MasterImport()
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        public ActionResult ImportDataViewAllRecords(int? page)
        {
            try
            {

                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                var itemsch = AbstractLayer.ImportDB.GetImportSearchListMain();
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = new SelectList(new[]{new {ID="2016",Name="2016"},new{ID="2015",Name="2015"},
            new{ID="2014",Name="2014"},}, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;



                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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


                string Search = string.Empty;

                //Search = "form in('M1','M2') and result in ('pass','COMPARTMENT','reappear') and schl='" + schl + "' and importflag is null";
                //Search = "importflag1617=0 and schl='" + schl + "' and OTID is not NULL";
                if (pageIndex == 1)
                {
                    TempData["ImportDataViewAllRecordssearch2"] = null;
                    TempData["ImportDataViewAllRecordsSelList2"] = null;
                    TempData["ImportDataViewAllRecordsSearchString2"] = null;
                    TempData["ImportDataViewAllRecordsSession2"] = null;
                }
                if (TempData["ImportDataViewAllRecordssearch2"] == null)
                    //Search = "form_Name='E1' and schl='" + schl + "' and oldid is not null and OROLL is not null ";
                    //Search = "form_Name='E1'";
                    Search = "form_Name='E1' and schl='" + schl + "' and oldid is not null and OROLL is not null ";
                else
                {
                    Search = Convert.ToString(TempData["ImportDataViewAllRecordssearch2"]);
                    ViewBag.SelectedItem = TempData["ImportDataViewAllRecordsSelList2"];
                    ViewBag.Searchstring = TempData["ImportDataViewAllRecordsSearchString2"];
                    ViewBag.SelectedSession = TempData["ImportDataViewAllRecordsSession2"];
                }
                obj.StoreAllData = objDB.SelectAllImportedData(Search, session, pageIndex);
                obj.TotalCount = objDB.AllImportDataCount(Search, session, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Data Doesn't Exist ";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int pn = tp / 10;
                    int cal = 10 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;

                    return View(obj);
                }

            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [HttpPost]
        public ActionResult ImportDataViewAllRecords(int? page, string cmd, string SelList, string SearchString, string Session1)
        {
            try
            {


                var itemsch = AbstractLayer.ImportDB.GetImportSearchListMain();
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = new SelectList(new[]{new {ID="2016",Name="2016"},new{ID="2015",Name="2015"},
            new{ID="2014",Name="2014"},}, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;



                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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


                string Search = string.Empty;

                //Search = "form in('M1','M2') and result in ('pass','COMPARTMENT','reappear') and schl='" + schl + "' and importflag is null";
                //Search = "importflag1617=0 and schl='" + schl + "' and OTID is not NULL";
                Search = "form_Name='E1' and schl='" + schl + "' and oldid is not null and OROLL is not null ";
                if (cmd == "Search")
                {
                    //Search = "result in ('P','C','R') and schl='" + schl + "' and CLASS='10' and importflag is null";
                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (Session1 != "")
                        {
                            Search += " and a.Year='" + Session1.ToString().Trim() + "'";
                        }

                        if (SearchString != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and a.OROLL='" + SearchString.ToString().Trim() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += " and  a.Registration_num like '%" + SearchString.ToString().Trim() + "%'"; }
                        }


                    }
                    else
                    {
                        if (Session1 != "")
                        {
                            Search += " and a.Year='" + Session1.ToString().Trim() + "'";
                        }
                    }

                    TempData["ImportDataViewAllRecordssearch2"] = Search;
                    TempData["ImportDataViewAllRecordsSelList2"] = SelList;
                    TempData["ImportDataViewAllRecordsSearchString2"] = SearchString.ToString().Trim();
                    TempData["ImportDataViewAllRecordsSession2"] = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    obj.StoreAllData = objDB.SelectAllImportedData(Search, session, pageIndex);
                    obj.TotalCount = objDB.AllImportDataCount(Search, session, pageIndex);
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist ";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                        int pn = tp / 10;
                        int cal = 10 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;

                        //return View(rm);
                        //ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        //rm.chkidList = new List<ImportIDModel>();

                        //ImportIDModel chk = null;
                        //for (int i = 0; i < rm.StoreAllData.Tables[0].Rows.Count; i++)
                        //{
                        //    chk = new ImportIDModel();
                        //    chk.id = rm.StoreAllData.Tables[0].Rows[i]["TID"].ToString();
                        //    chk.Selected = false;
                        //    rm.chkidList.Add(chk);
                        //}

                        return View(obj);
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }

        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ImportDataViewTo11thClass(int? page)
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                var itemsch = new SelectList(new[]{new {ID="1",Name="Roll Number"},new{ID="2",Name="Candidate Name"},
            new{ID="3",Name="Father's Name"},new{ID="4",Name="Mother's Name"},new{ID="5",Name="DOB"},}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;



                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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


                string Search = string.Empty;

                //Search = "form in('M1','M2') and result in ('pass','COMPARTMENT','reappear') and schl='" + schl + "' and importflag is null";
                if (pageIndex == 1)
                {
                    TempData["ImportDataViewTo11thClasssearch4"] = null;
                    TempData["ImportDataViewTo11thClassSelList4"] = null;
                    TempData["ImportDataViewTo11thClassSearchString4"] = null;
                }
                if (TempData["ImportDataViewTo11thClasssearch4"] == null)
                    Search = "importflag1617=0 and schl='" + schl + "' and OTID is not NULL";
                else
                {
                    Search = Convert.ToString(TempData["ImportDataViewTo11thClasssearch4"]);
                    ViewBag.SelectedItem = TempData["ImportDataViewTo11thClassSelList4"];
                    ViewBag.Searchstring = TempData["ImportDataViewTo11thClassSearchString4"];
                }
                obj.StoreAllData = objDB.SelectAllImportedData(Search, session, pageIndex);
                obj.TotalCount = objDB.AllImportDataCount(Search, session, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Data Doesn't Exist ";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int pn = tp / 10;
                    int cal = 10 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;

                    return View(obj);
                }
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [HttpPost]
        public ActionResult ImportDataViewTo11thClass(int? page, FormCollection frm, string cmd, string SelList, string SearchString)
        {
            try
            {


                var itemsch = new SelectList(new[]{new {ID="1",Name="Roll Number"},new{ID="2",Name="Candidate Name"},
            new{ID="3",Name="Father's Name"},new{ID="4",Name="Mother's Name"},new{ID="5",Name="DOB"},}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                Import rm = new Import();
                if (ModelState.IsValid)
                {

                    string Search = string.Empty;
                    string schlid = "";
                    string session = "";
                    if (Session["SCHL"] != null)
                    {
                        session = Session["Session"].ToString();
                        schlid = Session["SCHL"].ToString();
                        // obj.schoolcode = schl;
                        List<SelectListItem> schllist = new List<SelectListItem>();

                        string SchoolAssign = "";
                        SchoolAssign = objDB.GetImpschlOcode(3, schlid);
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
                                schllist.Add(new SelectListItem { Text = schlid, Value = schlid });
                            }
                        }
                        //schllist.Add(new SelectListItem { Text = "---Select Import TO--", Value = "0" });                
                        ViewBag.MySchCode = schllist;
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login");
                    }

                    //Search = "form in('M1','M2') and result in ('pass','COMPARTMENT','reappear') and schl='" + schl + "' and importflag is null";
                    //Search = "importflag1617=0 and schl='" + schl + "' and OTID is not NULL";
                    Search = "importflag1617=0 and schl='" + schlid + "' and OTID is not NULL";
                    if (cmd == "Search")
                    {
                        //Search = "result in ('P','C','R') and schl='" + schl + "' and CLASS='10' and importflag is null";
                        if (SelList != "")
                        {
                            ViewBag.SelectedItem = SelList;
                            int SelValueSch = Convert.ToInt32(SelList.ToString());


                            if (SearchString != "")
                            {
                                if (SelValueSch == 1)
                                { Search += " and a.OROLL='" + SearchString.ToString().Trim() + "'"; }
                                else if (SelValueSch == 2)
                                { Search += " and  a.Candi_Name like '%" + SearchString.ToString().Trim() + "%'"; }
                                else if (SelValueSch == 3)
                                { Search += " and a.Father_Name  like '%" + SearchString.ToString().Trim() + "%'"; }
                                else if (SelValueSch == 4)
                                { Search += " and a.Mother_Name like '%" + SearchString.ToString().Trim() + "%'"; }
                                else if (SelValueSch == 5)
                                { Search += " and a.DOB='" + SearchString.ToString().Trim() + "'"; }
                            }


                        }
                        TempData["ImportDataViewTo11thClasssearch4"] = Search;
                        TempData["ImportDataViewTo11thClassSelList4"] = SelList;
                        TempData["ImportDataViewTo11thClassSearchString4"] = SearchString.ToString().Trim();
                        ViewBag.Searchstring = SearchString.ToString().Trim();
                        rm.StoreAllData = objDB.SelectAllImportedData(Search, session, pageIndex);
                        rm.TotalCount = objDB.AllImportDataCount(Search, session, pageIndex);
                        if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Data Doesn't Exist ";
                            ViewBag.TotalCount = 0;
                            return View();
                        }
                        else
                        {
                            ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                            ViewBag.TotalCount1 = Convert.ToInt32(rm.TotalCount.Tables[0].Rows[0]["decount"]);
                            int tp = Convert.ToInt32(rm.TotalCount.Tables[0].Rows[0]["decount"]);
                            int pn = tp / 10;
                            int cal = 10 * pn;
                            int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                            if (res >= 1)
                                ViewBag.pn = pn + 1;
                            else
                                ViewBag.pn = pn;

                            return View(rm);
                        }

                    }
                    return View();
                }
                return View();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        public ActionResult ImportDataViewTo11thClassView(string id)
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (id == null)
                {
                    return RedirectToAction("ImportDataViewAllRecords", "ImportData");
                }

                RegistrationModels rm = new RegistrationModels();
                string formname = "E1";
                if (id != null)
                {
                    try
                    {
                        id = encrypt.QueryStringModule.Decrypt(id);
                        DataSet ds = objDB.SearchStudentGetByData_E_Import(id, formname);
                        if (ds == null)
                        {
                            return RedirectToAction("ImportDataViewAllRecords", "ImportData");
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            //DataSet ds = objDB.SearchStudentGetByData(id);
                            // rm.StoreAllData = objDB.SearchStudentGetByData(id);
                            //rm.Prev_School_Name = ds.Tables[0].Rows[0]["SchE"].ToString();
                            rm.Category = ds.Tables[0].Rows[0]["Category"].ToString();
                            rm.Board = ds.Tables[0].Rows[0]["Board"].ToString();
                            rm.Other_Board = ds.Tables[0].Rows[0]["Other_Board"].ToString();
                            rm.Registration_num = ds.Tables[0].Rows[0]["Registration_num"].ToString();
                            rm.Board_Roll_Num = ds.Tables[0].Rows[0]["OROLL"].ToString();
                            //rm.Board_Roll_Num = ds.Tables[0].Rows[0]["Board_Roll_Num"].ToString();
                            //rm.Prev_School_Name = ds.Tables[0].Rows[0]["Prev_School_Name"].ToString();
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
                            if (ds.Tables[0].Rows[0]["Section"].ToString() == "")
                            {
                                rm.Section = '0';
                            }
                            else
                            {
                                rm.Section = Convert.ToChar(ds.Tables[0].Rows[0]["Section"].ToString());
                            }


                            string formName = ds.Tables[0].Rows[0]["form_Name"].ToString();
                            //rm.MetricYear = ds.Tables[0].Rows[0]["MetricYear"].ToString();
                            //rm.MetricMonth = ds.Tables[0].Rows[0]["MetricMonth"].ToString();

                            rm.MetricYear = ds.Tables[0].Rows[0]["Year"].ToString();
                            rm.MetricMonth = ds.Tables[0].Rows[0]["Month"].ToString();
                            //rm.Metric_Roll_Num = ds.Tables[0].Rows[0]["MetricRollNum"].ToString();
                            rm.Metric_Roll_Num = ds.Tables[0].Rows[0]["OROLL"].ToString();

                            @ViewBag.Photo = "https://registration2022.pseb.ac.in/Upload2015Matric/" + rm.Metric_Roll_Num + ".jpg";
                            //@ViewBag.Photo = "../../upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                            @ViewBag.sign = "../../upload/" + ds.Tables[0].Rows[0]["std_Sign"].ToString();
                        }
                        else
                        {
                            return RedirectToAction("ImportDataViewAllRecords", "ImportData");
                        }
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("ImportDataViewAllRecords", "ImportData");
                    }

                }
                //return View(asm);
                return View(rm);
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        public ActionResult ImportDataViewTo11thClassModify(string id)
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                if (id == null)
                {
                    return RedirectToAction("ImportDataViewTo11thClassView", "ImportData");
                }

                RegistrationModels rm = new RegistrationModels();
                string formname = "E1";
                if (id != null)
                {
                    try
                    {
                        id = encrypt.QueryStringModule.Decrypt(id);
                        DataSet ds = objDB.SearchStudentGetByData_E_Import(id, formname);
                        if (ds == null)
                        {
                            return RedirectToAction("ImportDataViewTo11thClassView", "ImportData");
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            // DataSet ds = objDB.SearchStudentGetByData(id);
                            ViewBag.reap = ds.Tables[0].Rows[0]["Category"].ToString();
                            rm.Std_id = Convert.ToInt32(ds.Tables[0].Rows[0]["Std_id"].ToString());

                            rm.Category = ds.Tables[0].Rows[0]["Category"].ToString();
                            ViewBag.catg = objCommon.GetE2Category();

                            rm.Board = ViewBag.MyBoard2 = ds.Tables[0].Rows[0]["Board"].ToString();
                            ViewBag.MyBoard = objCommon.GetBoard();

                            rm.Other_Board = ds.Tables[0].Rows[0]["Other_Board"].ToString();
                            rm.Board_Roll_Num = ds.Tables[0].Rows[0]["OROLL"].ToString();
                            //rm.Board_Roll_Num = ds.Tables[0].Rows[0]["Board_Roll_Num"].ToString();
                            if (ds.Tables[0].Rows[0]["Prev_School_Name"].ToString() == "")
                            {
                                rm.Prev_School_Name = Session["SchlE"].ToString();
                                //rm.IsPrevSchoolSelf = true;
                            }
                            else
                            {
                                rm.Prev_School_Name = ds.Tables[0].Rows[0]["Prev_School_Name"].ToString();
                            }

                            //rm.Prev_School_Name = ds.Tables[0].Rows[0]["SchE"].ToString();

                            rm.Month = ds.Tables[0].Rows[0]["Month"].ToString();
                            rm.MetricMonth = ds.Tables[0].Rows[0]["Month"].ToString();
                            if (ds.Tables[0].Rows[0]["Month"].ToString() == "MARCH")
                            {
                                rm.Month = "MAR";
                                rm.MetricMonth = "MAR";
                            }
                            ViewBag.Mon = objCommon.GetMonth();

                            rm.MetricMonth = ds.Tables[0].Rows[0]["Month"].ToString();
                            if (ds.Tables[0].Rows[0]["Month"].ToString() == "MARCH")
                            {

                                rm.MetricMonth = "MAR";
                            }
                            ViewBag.MMon = objCommon.GetMonth();
                            rm.Year = ds.Tables[0].Rows[0]["Year"].ToString();
                            ViewBag.SessionYearList = objCommon.GetSessionYear();

                            rm.MetricYear = ds.Tables[0].Rows[0]["Year"].ToString();
                            ViewBag.SessionyYearList = objCommon.GetSessionYear();

                            //rm.MetricYear = ds.Tables[0].Rows[0]["MetricYear"].ToString();
                            //rm.MetricMonth = ds.Tables[0].Rows[0]["MetricMonth"].ToString();
                            rm.Metric_Roll_Num = ds.Tables[0].Rows[0]["OROLL"].ToString();


                            rm.MyGroup = ds.Tables[0].Rows[0]["Group_name"].ToString().Trim();
                            //ViewBag.MyGroup1 = objCommon.GroupName();
                            List<SelectListItem> MyGroupList = objCommon.GroupName();
                            // ViewBag.MyGroup = objCommon.GroupName();
                            DataTable dtAssignSubject = objCommon.GetAssignSubjectBySchool(Session["SCHL"].ToString());
                            if (dtAssignSubject != null && dtAssignSubject.Rows.Count > 0)
                            {
                                ViewBag.MyGroup1 = objCommon.GetSubjectsBySchool(dtAssignSubject, MyGroupList);
                            }

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
                            ViewBag.Caste = objCommon.GetCaste();

                            if (ds.Tables[0].Rows[0]["Gender"].ToString() == "M")
                            {
                                rm.Gender = "Male";
                            }
                            else
                            {
                                rm.Gender = "Female";
                            }


                            rm.Differently_Abled = ds.Tables[0].Rows[0]["Differently_Abled"].ToString();
                            ViewBag.DA = objCommon.GetDA();

                            rm.Religion = ds.Tables[0].Rows[0]["Religion"].ToString();
                            ViewBag.RE = objCommon.GetReligion();

                            rm.DOB = ds.Tables[0].Rows[0]["DOB"].ToString();
                            rm.Mobile = ds.Tables[0].Rows[0]["Mobile"].ToString();
                            rm.Belongs_BPL = ds.Tables[0].Rows[0]["Belongs_BPL"].ToString();
                            rm.E_punjab_Std_id = ds.Tables[0].Rows[0]["E_punjab_Std_id"].ToString();
                            rm.Aadhar_num = ds.Tables[0].Rows[0]["Aadhar_num"].ToString();
                            rm.Address = ds.Tables[0].Rows[0]["Address"].ToString();
                            rm.LandMark = ds.Tables[0].Rows[0]["LandMark"].ToString();
                            rm.Block = ds.Tables[0].Rows[0]["Block"].ToString();
                            string formName = ds.Tables[0].Rows[0]["form_Name"].ToString();
                            ViewBag.otid = ds.Tables[0].Rows[0]["otid"].ToString();

                            rm.MyDistrict = ds.Tables[0].Rows[0]["District"].ToString();
                            //rm.District = Convert.ToInt32(ds.Tables[0].Rows[0]["District"].ToString());
                            ViewBag.MyDist = objCommon.GetDistE();
                            if (ds.Tables[0].Rows[0]["Tehsil"].ToString() == "")
                            {
                                rm.Tehsil = 0;
                            }
                            else
                            {
                                rm.Tehsil = Convert.ToInt32(ds.Tables[0].Rows[0]["Tehsil"].ToString());
                            }


                            int dist = Convert.ToInt32(ds.Tables[0].Rows[0]["District"].ToString());

                            AbstractLayer.RegistrationDB objDBT = new AbstractLayer.RegistrationDB();
                            DataSet result1 = objDBT.SelectAllTehsil(dist);
                            ViewBag.MyTeh = result1.Tables[0];
                            List<SelectListItem> TehList = new List<SelectListItem>();

                            foreach (System.Data.DataRow dr in ViewBag.MyTeh.Rows)
                            {

                                TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                            }
                            ViewBag.MyTeh = TehList;

                            rm.PinCode = ds.Tables[0].Rows[0]["PinCode"].ToString();
                            rm.Registration_num = ds.Tables[0].Rows[0]["Registration_num"].ToString();

                            ////int IsPrevSchoolSelf = Convert.ToInt32(ds.Tables[0].Rows[0]["IsPrevSchoolSelf"].ToString());
                            ////if (IsPrevSchoolSelf == 1)
                            ////{
                            ////    rm.IsPrevSchoolSelf = true;
                            ////}
                            ////else
                            ////{
                            ////    rm.IsPrevSchoolSelf = false;
                            ////}
                            ////int IsPSEBRegNum = Convert.ToInt32(ds.Tables[0].Rows[0]["IsPSEBRegNum"].ToString());
                            ////if (IsPSEBRegNum == 1)
                            ////{
                            ////    rm.IsPSEBRegNum = true;
                            ////    rm.Registration_num = ds.Tables[0].Rows[0]["Registration_num"].ToString();
                            ////}
                            ////else
                            ////{
                            ////    rm.IsPSEBRegNum = false;
                            ////    rm.Registration_num = ds.Tables[0].Rows[0]["Registration_num"].ToString();
                            ////}
                            ////int IsPro = Convert.ToInt32(ds.Tables[0].Rows[0]["Provisional"].ToString());
                            ////if (IsPro == 1)
                            ////{
                            ////    rm.Provisional = true;
                            ////}
                            ////else
                            ////{
                            ////    rm.Provisional = false;
                            ////}
                            // rm.fname= ds.Tables[0].Rows[0]["std_Photo"].ToString();
                            // @ViewBag.ImageURL = "../../StdImages/Upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                            //rm.PhotoString= "../../upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();

                            string Oroll = ds.Tables[0].Rows[0]["OROLL"].ToString();
                            @ViewBag.Photo = "https://registration2022.pseb.ac.in/Upload2015Matric/" + Oroll + ".jpg";
                            //@ViewBag.Photo = "../../upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                            @ViewBag.sign = "../../upload/" + ds.Tables[0].Rows[0]["std_Sign"].ToString();
                            //  rm.std_Sign = "";
                            ////rm.Section = Convert.ToChar(ds.Tables[0].Rows[0]["Section"].ToString());
                            ViewBag.SectionList = objCommon.GetSection();
                        }
                        else
                        {
                            return RedirectToAction("ImportDataViewTo11thClassView", "ImportData");
                        }
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("ImportDataViewTo11thClassView", "ImportData");
                    }

                }
                //return View(asm);
                return View(rm);
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [HttpPost]
        public ActionResult ImportDataViewTo11thClassModify(RegistrationModels rm, FormCollection frm)
        {
            try
            {


                string schDist = "";
                // AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();

                ViewBag.catg = objCommon.GetE2Category();
                ViewBag.MyBoard = objCommon.GetN2Board();

                ViewBag.Mon = objCommon.GetMonth();

                // AbstractLayer.DBClass objDBC = new AbstractLayer.DBClass();
                ViewBag.SessionYearList = objCommon.GetSessionYear();

                ViewBag.Caste = objCommon.GetCaste();

                ViewBag.DA = objCommon.GetDA();

                ViewBag.RE = objCommon.GetReligion();
                ViewBag.MyDist = objCommon.GetDistE();


                ViewBag.SectionList = objCommon.GetSection();
                ViewBag.MyTeh = objCommon.GetAllTehsil();
                string dist = rm.MyDistrict;
                List<SelectListItem> MyGroupList = objCommon.GroupName();
                // ViewBag.MyGroup = objCommon.GroupName();
                DataTable dtAssignSubject = objCommon.GetAssignSubjectBySchool(Session["SCHL"].ToString());
                if (dtAssignSubject != null && dtAssignSubject.Rows.Count > 0)
                {
                    ViewBag.MyGroup = objCommon.GetSubjectsBySchool(dtAssignSubject, MyGroupList);
                }
                ViewBag.MyGroup = MyGroupList;

                if (ModelState.IsValid)
                {
                    // AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                    string id = rm.Std_id.ToString();
                    string formname = "E1";

                    DataSet ds = objDB.SearchStudentGetByData_E_Import(id, formname);
                    //DataSet ds = objDB.SearchStudentGetByData(id);
                    string distOld = ds.Tables[0].Rows[0]["District"].ToString();
                    rm.MyDistrict = distOld;
                    //string stdPic = null;
                    string formName = "E1";
                    //if (frm["file"].ToString() != "")
                    if (rm.file != null)
                    {
                        //stdPic = Path.GetFileName(frm["file"]);
                        stdPic = Path.GetFileName(rm.file.FileName);

                    }
                    else
                    {
                        stdPic = ds.Tables[0].Rows[0]["std_Photo"].ToString();



                        string Oroll = ds.Tables[0].Rows[0]["OROLL"].ToString();
                        string downPic = "https://registration2022.pseb.ac.in/Upload2015Matric/" + Oroll + ".jpg";

                        if (Session["SCHOOLDIST"] != null)
                        {
                            schDist = Session["SCHOOLDIST"].ToString();
                        }
                        else
                        {
                            return RedirectToAction("Index", "Login");
                        }

                        string PhotoName = "../Upload/" + "E1" + "/" + schDist + "/Photo" + "/" + rm.Std_id.ToString() + "P" + ".jpg";
                        string type = "P";
                        //if (!System.IO.Directory.Exists(PhotoName))
                        //{
                        //    System.IO.Directory.CreateDirectory(PhotoName);
                        //}
                        // System.IO.File.Copy(downPic, PhotoName);

                        //-----------------------------start----------

                        //string fileName = Oroll + ".jpg";
                        //string sourcePath = "https://registration2022.pseb.ac.in/Upload2015Matric/";
                        //string targetPath = Server.MapPath("../Upload/" + "E1" + "/" + schDist + "/Photo" + "/");

                        //string sourceFile = System.IO.Path.Combine(sourcePath, fileName);                    
                        //string destFile = System.IO.Path.Combine(targetPath, fileName);

                        //if (!System.IO.Directory.Exists(targetPath))
                        //{
                        //    System.IO.Directory.CreateDirectory(targetPath);
                        //}

                        //// To copy a file to another location and 
                        //// overwrite the destination file if it already exists.
                        //System.IO.File.Copy(sourceFile, destFile, true);

                        //-----------------------end----------------------




                        string UpdatePic = objDB.Updated_Pic_Data(rm.Std_id.ToString(), PhotoName, type);


                    }
                    if (rm.std_Sign != null)
                    {
                        //stdPic = Path.GetFileName(frm["file"]);
                        stdSign = Path.GetFileName(rm.std_Sign.FileName);

                    }
                    else
                    {

                        stdSign = ds.Tables[0].Rows[0]["std_Sign"].ToString();
                    }
                    //var stdPic = Path.GetFileName(rm.file.FileName);
                    //var stdSign = Path.GetFileName(rm.std_Sign.FileName);



                    string result = objDB.Update_E_Data(rm, frm, formName, id, stdPic, stdSign);
                    string filepathtosave = "";
                    if (result == "0")
                    {
                        //--------------Not updated
                        ViewData["resultUpdate"] = 0;
                    }
                    if (result == "-1")
                    {
                        //-----alredy exist
                        ViewData["resultUpdate"] = -1;
                    }
                    else
                    {
                        if (rm.file != null)
                        {
                            //var path = Path.Combine(Server.MapPath("allfiles/Upload2023/" + formName + "/" + Session["Dist"].ToString() + "/Photo"), result + "P" + ".jpg");
                            //string FilepathExist = Path.Combine(Server.MapPath("allfiles/Upload2023/" + formName + "/" + Session["Dist"].ToString() + "/Photo"));
                            //if (!Directory.Exists(FilepathExist))
                            //{
                            //    Directory.CreateDirectory(FilepathExist);
                            //}
                            //rm.file.SaveAs(path);
                            string Orgfile = result + "P" + ".jpg";
                            using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                            {
                                using (var newMemoryStream = new MemoryStream())
                                {
                                    var uploadRequest = new TransferUtilityUploadRequest
                                    {
                                        InputStream = rm.file.InputStream,
                                        Key = string.Format("allfiles/Upload2023/" + formName + "/" + Session["Dist"].ToString() + "/Photo/{0}", Orgfile),
                                        BucketName = BUCKET_NAME,
                                        CannedACL = S3CannedACL.PublicRead
                                    };

                                    var fileTransferUtility = new TransferUtility(client);
                                    fileTransferUtility.Upload(uploadRequest);
                                }
                            }


                            //filepathtosave = "../Upload/" + formName + "/" + Session["Dist"].ToString() + "/Photo/" + result + "P" + ".jpg";
                            filepathtosave = "allfiles/Upload2023/" + formName + "/" + Session["Dist"].ToString() + "/Photo/" + result + "P" + ".jpg";
                            ViewBag.ImageURL = filepathtosave;

                            //string PhotoName = formName + "/" + Session["Dist"].ToString() + "/Photo" + "/" + result + "P" + ".jpg";
                            string PhotoName = formName + "/" + Session["Dist"].ToString() + "/Photo" + "/" + result + "P" + ".jpg";
                            string type = "P";
                            string UpdatePic = objDB.Updated_Pic_Data(result, PhotoName, type);
                        }
                        if (rm.std_Sign != null)
                        {
                            //var path = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + Session["Dist"].ToString() + "/Sign"), result + "S" + ".jpg");
                            //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + Session["Dist"].ToString() + "/Sign"));
                            //if (!Directory.Exists(FilepathExist))
                            //{
                            //    Directory.CreateDirectory(FilepathExist);
                            //}
                            //rm.std_Sign.SaveAs(path);
                            string Orgfile = result + "S" + ".jpg";
                            using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                            {
                                using (var newMemoryStream = new MemoryStream())
                                {
                                    var uploadRequest = new TransferUtilityUploadRequest
                                    {
                                        InputStream = rm.file.InputStream,
                                        Key = string.Format("allfiles/Upload2023/" + formName + "/" + Session["Dist"].ToString() + "/Sign/{0}", Orgfile),
                                        BucketName = BUCKET_NAME,
                                        CannedACL = S3CannedACL.PublicRead
                                    };

                                    var fileTransferUtility = new TransferUtility(client);
                                    fileTransferUtility.Upload(uploadRequest);
                                }
                            }
                            filepathtosave = "allfiles/Upload2023/" + formName + "/" + Session["Dist"].ToString() + "/Sign/" + result + "S" + ".jpg";
                            ViewBag.ImageURL = filepathtosave;

                            string PhotoName = formName + "/" + Session["Dist"].ToString() + "/Sign" + "/" + result + "S" + ".jpg";
                            string type = "S";
                            string UpdatePic = objDB.Updated_Pic_Data(result, PhotoName, type);
                        }

                        ModelState.Clear();
                        //--For Showing Message---------//
                        ViewData["resultUpdate"] = result;
                        return RedirectToAction("ImportDataViewTo11thClassView", "ImportData");

                    }
                }
                return View(rm);
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        public ActionResult ImportDataViewTo11thClassDelete(string id)
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (id == null)
                {
                    return RedirectToAction("ImportDataViewTo11thClassView", "ImportData");
                }
                //calling class DBClass
                string formname = "E1";

                DataSet ds = objDB.SearchStudentGetByData_E(id, formname);
                //DataSet ds = objDB.SearchStudentGetByData(id);
                string dist = ds.Tables[0].Rows[0]["District"].ToString();
                string imgPhoto = ds.Tables[0].Rows[0]["std_Photo"].ToString();
                string imgSign = ds.Tables[0].Rows[0]["std_Sign"].ToString();
                string formName = ds.Tables[0].Rows[0]["form_Name"].ToString();
                string OTID = "";
                string tcref = "";
                if (ds.Tables[0].Rows[0]["OTID"].ToString() != "")
                {
                    OTID = ds.Tables[0].Rows[0]["OTID"].ToString();
                }
                else
                {
                    OTID = "";
                }
                if (ds.Tables[0].Rows[0]["TCrefno"].ToString() != "")
                {
                    tcref = ds.Tables[0].Rows[0]["TCrefno"].ToString();
                }
                else
                {
                    tcref = "";
                }
                string OLDID = ds.Tables[0].Rows[0]["oldid"].ToString();
                string result = objDB.Delete_E_FromData(id, OTID, OLDID, tcref); // passing Value to DBClass from model

                //--------Photo Delete------
                //RegistrationModels RM = new RegistrationModels();
                Import RM = new Import();
                var fileDesti = "";
                var fileDestiPic = "";



                var pathPhoto = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + dist + "/Photo"), imgPhoto);

                if (System.IO.File.Exists(pathPhoto))
                {
                    string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + dist + "/PhotoD"));
                    if (!Directory.Exists(FilepathExist))
                    {
                        Directory.CreateDirectory(FilepathExist);
                    }

                    //fileDesti = "../StdImages/DeletedPic/" + imgName;
                    fileDestiPic = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + dist + "/PhotoD"), imgPhoto);
                    System.IO.File.Move(pathPhoto, fileDestiPic);
                    System.IO.File.Delete(pathPhoto);

                }
                var pathSIGN = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + dist + "/Sign"), imgSign);
                if (System.IO.File.Exists(pathSIGN))
                {
                    string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + dist + "/SignD"));
                    if (!Directory.Exists(FilepathExist))
                    {
                        Directory.CreateDirectory(FilepathExist);
                    }
                    //fileDesti = "../StdImages/DeletedPic/" + imgName;
                    fileDesti = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + dist + "/SignD"), imgSign);
                    System.IO.File.Move(pathSIGN, fileDesti);
                    System.IO.File.Delete(pathSIGN);

                }
                //------------End photo Delete------

                ViewData["resultDelete"] = result; // for dislaying message after saving storing output.
                return RedirectToAction("ImportDataViewTo11thClassView", "ImportData");
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        public JsonResult SomeActionMethod(string id)
        {
            return Json(new { foo = "bar", baz = "Blech" }, JsonRequestBehavior.AllowGet);
        }



        //-----------------------11th Failed Candidates------------------
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ImportData11THFailed(int? page)
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                var itemsch = AbstractLayer.ImportDB.GetImportSearchListMain();
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = new SelectList(new[]{new {ID="2016",Name="2016"},new{ID="2015",Name="2015"},
            new{ID="2014",Name="2014"},}, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;



                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;

                //Search = "form in('M1','M2') and result in ('pass','COMPARTMENT','reappear') and schl='"+ schl + "' and importflag is null";
                if (pageIndex == 1)
                {
                    TempData["ImportData11THFailedsearch1"] = null;
                    TempData["ImportData11THFailedSelList1"] = null;
                    TempData["ImportData11THFailedSearchString1"] = null;
                    TempData["ImportData11THFailedSession1"] = null;
                }
                if (TempData["ImportData11THFailedsearch1"] == null)
                    // Search = "schl='" + schl + "' and form_name in ('E1','E2')  and importflag is null and result='FAIL'";
                    Search = "schl='" + schl + "' and form_name in ('E1','E2')  and result='FAIL'";
                else
                {
                    Search = Convert.ToString(TempData["ImportData11THFailedsearch1"]);
                    ViewBag.SelectedItem = TempData["ImportData11THFailedSelList1"];
                    ViewBag.Searchstring = TempData["ImportData11THFailedSearchString1"];
                    ViewBag.SelectedSession = TempData["ImportData11THFailedSession1"];
                }
                obj.StoreAllData = objDB.SelectAll11thFailed(Search, session, pageIndex);
                //rm.StoreAllData = objDB.SelectAll10thPass(Search, session, pageIndex);
                obj.TotalCount = objDB.GetAll11thFailedCount(Search, session, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST or allready imported. to check imported data click on \"Check Reg. No.of 2016 - 17\" link under Registration Menu.";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int pn = tp / 10;
                    int cal = 10 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;
                    obj.chkidList = new List<ImportIDModel>();



                }

                return View(obj);
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult ImportData11THFailed(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1)
        {
            try
            {


                var itemsch = AbstractLayer.ImportDB.GetImportSearchListMain();
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = new SelectList(new[]{new {ID="2016",Name="2016"},new{ID="2015",Name="2015"},
            new{ID="2014",Name="2014"},}, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();
                string session = null;
                string schl = null;
                string Search = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                Import rm = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    //obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                if (cmd == "Search")
                {
                    Search = "schl ='" + schl + "' and form_name in ('E1','E2') and (result='Fail' or result='ABSENT')  and importflag is null";
                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SearchString != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and a.OROLL='" + SearchString.ToString().Trim() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += " and  a.REGISTRATION_NUM like '%" + SearchString.ToString().Trim() + "%'"; }
                        }
                    }
                    //else
                    //{
                    //    Search = "schl='" + schl + "' and cat='11TH FAILED' and importflag is null and result='FAIL'";
                    //}

                    TempData["ImportData11THFailedsearch1"] = Search;
                    TempData["ImportData11THFailedSelList1"] = SelList;
                    TempData["ImportData11THFailedSearchString1"] = SearchString.ToString().Trim();
                    TempData["ImportData11THFailedSession1"] = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    rm.StoreAllData = objDB.SelectAll11thFailed(Search, session, pageIndex);
                    rm.TotalCount = objDB.GetAll11thFailedCount(Search, session, pageIndex);
                    //rm.StoreAllData = objDB.GetStudentRecordsSearch_ImportData(Search);
                    // asm.StoreAllData = objDB.SearchSchoolDetails(Search);
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "DATA DOESN'T EXIST or allready imported. to check imported data click on \"Check Reg. No.of 2016 - 17\" link under Registration Menu.";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(rm.TotalCount.Tables[0].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(rm.TotalCount.Tables[0].Rows[0]["decount"]);
                        int pn = tp / 10;
                        int cal = 10 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;

                        return View(rm);

                    }
                }
                /////-----------------------Import Begins-------
                // string importToSchl = imp.schoolcode;
                string importToSchl = Session["SCHL"].ToString();

                string collectId = frm["ChkCTenthClass"];
                int cnt = collectId.Count(x => x == ',');
                TempData["TotImported"] = cnt + 1;
                DataTable dt = new DataTable();
                dt = objDB.Select_All_11ThFailed_Data(importToSchl, collectId);
                if (dt == null || dt.Rows.Count == 0)
                {
                    // return RedirectToAction("ImportData10thClass", "ImportData");
                    ViewBag.Message = "Data Doesn't Exist";
                    ViewBag.TotalCount = 0;
                    TempData["result"] = -1;
                    return View();
                }
                else
                {
                    TempData["result"] = 1;
                }


                //string result = objDB.Update_ImportData_11Th_Failed(importToSchl, collectId);
                //if (result == "0")
                //{
                //    //--------------Not Updated
                //    TempData["result"] = 0;
                //}
                //else
                //{
                //    //-------------- Updated----------
                //    TempData["result"] = 1;
                //}

                return View();
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        public ActionResult ImportDataViewTo11thFailed(int? page)
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                var itemsch = new SelectList(new[]{new {ID="1",Name="Roll Number"},new{ID="2",Name="Candidate Name"},
            new{ID="3",Name="Father's Name"},new{ID="4",Name="Mother's Name"},new{ID="5",Name="DOB"},}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;



                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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


                string Search = string.Empty;

                //Search = "form in('M1','M2') and result in ('pass','COMPARTMENT','reappear') and schl='" + schl + "' and importflag is null";
                if (pageIndex == 1)
                {
                    TempData["search5"] = null;
                    TempData["SelList5"] = null;
                    TempData["SearchString5"] = null;
                }
                if (TempData["search5"] == null)
                    Search = "importflag1617=0 and schl='" + schl + "' and oldid is not NULL";

                else
                {
                    Search = Convert.ToString(TempData["search4"]);
                    ViewBag.SelectedItem = TempData["SelList4"];
                    ViewBag.Searchstring = TempData["SearchString4"];
                }
                obj.StoreAllData = objDB.SelectAllImportedData(Search, session, pageIndex);
                obj.TotalCount = objDB.AllImportDataCount(Search, session, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Data Doesn't Exist";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int pn = tp / 10;
                    int cal = 10 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;

                    return View(obj);
                }
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [HttpPost]
        public ActionResult ImportDataViewTo11thFailed(int? page, FormCollection frm, string cmd, string SelList, string SearchString)
        {
            try
            {


                var itemsch = new SelectList(new[]{new {ID="1",Name="Roll Number"},new{ID="2",Name="Candidate Name"},
            new{ID="3",Name="Father's Name"},new{ID="4",Name="Mother's Name"},new{ID="5",Name="DOB"},}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                Import rm = new Import();
                if (ModelState.IsValid)
                {

                    string Search = string.Empty;
                    string schlid = "";
                    string session = "";
                    if (Session["SCHL"] != null)
                    {
                        session = Session["Session"].ToString();
                        schlid = Session["SCHL"].ToString();
                        // obj.schoolcode = schl;
                        List<SelectListItem> schllist = new List<SelectListItem>();

                        string SchoolAssign = "";
                        SchoolAssign = objDB.GetImpschlOcode(3, schlid);
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
                                schllist.Add(new SelectListItem { Text = schlid, Value = schlid });
                            }
                        }
                        //schllist.Add(new SelectListItem { Text = "---Select Import TO--", Value = "0" });                
                        ViewBag.MySchCode = schllist;
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login");
                    }

                    //Search = "form in('M1','M2') and result in ('pass','COMPARTMENT','reappear') and schl='" + schl + "' and importflag is null";
                    //Search = "importflag1617=0 and schl='" + schl + "' and OTID is not NULL";
                    Search = "importflag1617=0 and schl='" + schlid + "' and OTID is not NULL";
                    if (cmd == "Search")
                    {
                        //Search = "result in ('P','C','R') and schl='" + schl + "' and CLASS='10' and importflag is null";
                        if (SelList != "")
                        {
                            ViewBag.SelectedItem = SelList;
                            int SelValueSch = Convert.ToInt32(SelList.ToString());


                            if (SearchString != "")
                            {
                                if (SelValueSch == 1)
                                { Search += " and a.OROLL='" + SearchString.ToString().Trim() + "'"; }
                                else if (SelValueSch == 2)
                                { Search += " and  a.Candi_Name like '%" + SearchString.ToString().Trim() + "%'"; }
                                else if (SelValueSch == 3)
                                { Search += " and a.Father_Name  like '%" + SearchString.ToString().Trim() + "%'"; }
                                else if (SelValueSch == 4)
                                { Search += " and a.Mother_Name like '%" + SearchString.ToString().Trim() + "%'"; }
                                else if (SelValueSch == 5)
                                { Search += " and a.DOB='" + SearchString.ToString().Trim() + "'"; }
                            }


                        }

                        TempData["search5"] = Search;
                        TempData["SelList5"] = SelList;
                        TempData["SearchString5"] = SearchString.ToString().Trim();
                        ViewBag.Searchstring = SearchString.ToString().Trim();
                        rm.StoreAllData = objDB.SelectAllImportedData(Search, session, pageIndex);
                        rm.TotalCount = objDB.AllImportDataCount(Search, session, pageIndex);
                        if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Data Doesn't Exist";
                            ViewBag.TotalCount = 0;
                            return View();
                        }
                        else
                        {
                            ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                            ViewBag.TotalCount1 = Convert.ToInt32(rm.TotalCount.Tables[0].Rows[0]["decount"]);
                            int tp = Convert.ToInt32(rm.TotalCount.Tables[0].Rows[0]["decount"]);
                            int pn = tp / 10;
                            int cal = 10 * pn;
                            int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                            if (res >= 1)
                                ViewBag.pn = pn + 1;
                            else
                                ViewBag.pn = pn;

                            return View(rm);
                        }

                    }
                    return View();
                }
                return View();
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        public ActionResult ImportDataViewTo11thFailedView(string id)
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (id == null)
                {
                    return RedirectToAction("ImportDataViewTo11thFailedView", "ImportData");
                }

                RegistrationModels rm = new RegistrationModels();
                string formname = "E1";
                if (id != null)
                {
                    try
                    {
                        id = encrypt.QueryStringModule.Decrypt(id);
                        DataSet ds = objDB.SearchStudentGetByData_E_Import(id, formname);
                        if (ds == null)
                        {
                            return RedirectToAction("ImportDataViewTo11thFailedView", "ImportData");
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            //DataSet ds = objDB.SearchStudentGetByData(id);
                            // rm.StoreAllData = objDB.SearchStudentGetByData(id);
                            // rm.Prev_School_Name = ds.Tables[0].Rows[0]["SchE"].ToString();
                            rm.Category = ds.Tables[0].Rows[0]["Category"].ToString();
                            rm.Board = ds.Tables[0].Rows[0]["Board"].ToString();
                            rm.Other_Board = ds.Tables[0].Rows[0]["Other_Board"].ToString();
                            rm.Registration_num = ds.Tables[0].Rows[0]["Registration_num"].ToString();
                            rm.Board_Roll_Num = ds.Tables[0].Rows[0]["OROLL"].ToString();
                            //rm.Board_Roll_Num = ds.Tables[0].Rows[0]["Board_Roll_Num"].ToString();
                            //rm.Prev_School_Name = ds.Tables[0].Rows[0]["Prev_School_Name"].ToString();
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
                            //rm.MyDistrict = ds.Tables[0].Rows[0]["distE"].ToString();
                            //rm.MYTehsil = ds.Tables[0].Rows[0]["tehE"].ToString();
                            rm.PinCode = ds.Tables[0].Rows[0]["PinCode"].ToString();
                            //rm.Section = Convert.ToChar(ds.Tables[0].Rows[0]["Section"].ToString());

                            string formName = ds.Tables[0].Rows[0]["form_Name"].ToString();
                            //rm.MetricYear = ds.Tables[0].Rows[0]["MetricYear"].ToString();
                            //rm.MetricMonth = ds.Tables[0].Rows[0]["MetricMonth"].ToString();

                            rm.MetricYear = ds.Tables[0].Rows[0]["Year"].ToString();
                            rm.MetricMonth = ds.Tables[0].Rows[0]["Month"].ToString();
                            //rm.Metric_Roll_Num = ds.Tables[0].Rows[0]["MetricRollNum"].ToString();
                            rm.Metric_Roll_Num = ds.Tables[0].Rows[0]["OROLL"].ToString();

                            // @ViewBag.ImageURL = "../../StdImages/Upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                            @ViewBag.Photo = "../../upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                            @ViewBag.sign = "../../upload/" + ds.Tables[0].Rows[0]["std_Sign"].ToString();
                        }
                        else
                        {
                            return RedirectToAction("ImportDataViewTo11thFailedView", "ImportData");
                        }
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("ImportDataViewTo11thFailedView", "ImportData");
                    }

                }
                //return View(asm);
                return View(rm);
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        public ActionResult ImportDataViewTo11thFailedModify(string id)
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (id == null)
                {
                    return RedirectToAction("ImportDataViewTo11thFailed", "ImportData");
                }

                RegistrationModels rm = new RegistrationModels();
                string formname = "E1";
                if (id != null)
                {
                    try
                    {
                        id = encrypt.QueryStringModule.Decrypt(id);
                        DataSet ds = objDB.SearchStudentGetByData_E_Import(id, formname);
                        if (ds == null)
                        {
                            return RedirectToAction("ImportDataViewTo11thFailed", "ImportData");
                        }
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            // DataSet ds = objDB.SearchStudentGetByData(id);
                            rm.Std_id = Convert.ToInt32(ds.Tables[0].Rows[0]["Std_id"].ToString());

                            rm.Category = ds.Tables[0].Rows[0]["Category"].ToString();
                            ViewBag.catg = objCommon.GetE2Category();

                            rm.Board = ViewBag.MyBoard2 = ds.Tables[0].Rows[0]["Board"].ToString();
                            ViewBag.MyBoard = objCommon.GetBoard();

                            rm.Other_Board = ds.Tables[0].Rows[0]["Other_Board"].ToString();
                            rm.Board_Roll_Num = ds.Tables[0].Rows[0]["OROLL"].ToString();
                            //rm.Board_Roll_Num = ds.Tables[0].Rows[0]["Board_Roll_Num"].ToString();
                            // rm.Prev_School_Name = ds.Tables[0].Rows[0]["Prev_School_Name"].ToString();
                            rm.Prev_School_Name = ds.Tables[0].Rows[0]["SchE"].ToString();

                            rm.Month = ds.Tables[0].Rows[0]["Month"].ToString();
                            ViewBag.Mon = objCommon.GetMonth();

                            rm.Year = ds.Tables[0].Rows[0]["Year"].ToString();
                            // AbstractLayer.DBClass objDBC = new AbstractLayer.DBClass();
                            ViewBag.SessionYearList = objCommon.GetSessionYear();

                            rm.MyGroup = ds.Tables[0].Rows[0]["Group_name"].ToString().Trim();
                            ViewBag.MyGroup1 = objCommon.GroupName();

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
                            ViewBag.Caste = objCommon.GetCaste();

                            if (ds.Tables[0].Rows[0]["Gender"].ToString() == "M")
                            {
                                rm.Gender = "Male";
                            }
                            else
                            {
                                rm.Gender = "Female";
                            }


                            rm.Differently_Abled = ds.Tables[0].Rows[0]["Differently_Abled"].ToString();
                            ViewBag.DA = objCommon.GetDA();

                            rm.Religion = ds.Tables[0].Rows[0]["Religion"].ToString();
                            ViewBag.RE = objCommon.GetReligion();

                            rm.DOB = ds.Tables[0].Rows[0]["DOB"].ToString();
                            rm.Mobile = ds.Tables[0].Rows[0]["Mobile"].ToString();
                            rm.Belongs_BPL = ds.Tables[0].Rows[0]["Belongs_BPL"].ToString();
                            rm.E_punjab_Std_id = ds.Tables[0].Rows[0]["E_punjab_Std_id"].ToString();
                            rm.Aadhar_num = ds.Tables[0].Rows[0]["Aadhar_num"].ToString();
                            rm.Address = ds.Tables[0].Rows[0]["Address"].ToString();
                            rm.LandMark = ds.Tables[0].Rows[0]["LandMark"].ToString();
                            rm.Block = ds.Tables[0].Rows[0]["Block"].ToString();
                            string formName = ds.Tables[0].Rows[0]["form_Name"].ToString();


                            rm.MyDistrict = ds.Tables[0].Rows[0]["District"].ToString();
                            //rm.District = Convert.ToInt32(ds.Tables[0].Rows[0]["District"].ToString());
                            ViewBag.MyDist = objCommon.GetDistE();

                            //rm.Tehsil = Convert.ToInt32(ds.Tables[0].Rows[0]["Tehsil"].ToString());

                            int dist = Convert.ToInt32(ds.Tables[0].Rows[0]["District"].ToString());

                            AbstractLayer.RegistrationDB objDBT = new AbstractLayer.RegistrationDB();
                            DataSet result1 = objDBT.SelectAllTehsil(dist);
                            ViewBag.MyTeh = result1.Tables[0];
                            List<SelectListItem> TehList = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in ViewBag.MyTeh.Rows)
                            {

                                TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                            }
                            ViewBag.MyTeh = TehList;

                            rm.PinCode = ds.Tables[0].Rows[0]["PinCode"].ToString();
                            rm.Registration_num = ds.Tables[0].Rows[0]["Registration_num"].ToString();

                            ////int IsPrevSchoolSelf = Convert.ToInt32(ds.Tables[0].Rows[0]["IsPrevSchoolSelf"].ToString());
                            ////if (IsPrevSchoolSelf == 1)
                            ////{
                            ////    rm.IsPrevSchoolSelf = true;
                            ////}
                            ////else
                            ////{
                            ////    rm.IsPrevSchoolSelf = false;
                            ////}
                            ////int IsPSEBRegNum = Convert.ToInt32(ds.Tables[0].Rows[0]["IsPSEBRegNum"].ToString());
                            ////if (IsPSEBRegNum == 1)
                            ////{
                            ////    rm.IsPSEBRegNum = true;
                            ////    rm.Registration_num = ds.Tables[0].Rows[0]["Registration_num"].ToString();
                            ////}
                            ////else
                            ////{
                            ////    rm.IsPSEBRegNum = false;
                            ////    rm.Registration_num = ds.Tables[0].Rows[0]["Registration_num"].ToString();
                            ////}
                            ////int IsPro = Convert.ToInt32(ds.Tables[0].Rows[0]["Provisional"].ToString());
                            ////if (IsPro == 1)
                            ////{
                            ////    rm.Provisional = true;
                            ////}
                            ////else
                            ////{
                            ////    rm.Provisional = false;
                            ////}
                            // rm.fname= ds.Tables[0].Rows[0]["std_Photo"].ToString();
                            // @ViewBag.ImageURL = "../../StdImages/Upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                            //rm.PhotoString= "../../upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();

                            rm.MetricYear = ds.Tables[0].Rows[0]["MetricYear"].ToString();
                            rm.MetricMonth = ds.Tables[0].Rows[0]["MetricMonth"].ToString();
                            //rm.Metric_Roll_Num = ds.Tables[0].Rows[0]["MetricRollNum"].ToString();
                            rm.Metric_Roll_Num = ds.Tables[0].Rows[0]["OROLL"].ToString();

                            @ViewBag.Photo = "../../upload/" + ds.Tables[0].Rows[0]["std_Photo"].ToString();
                            @ViewBag.sign = "../../upload/" + ds.Tables[0].Rows[0]["std_Sign"].ToString();
                            //  rm.std_Sign = "";
                            ////rm.Section = Convert.ToChar(ds.Tables[0].Rows[0]["Section"].ToString());
                            ViewBag.SectionList = objCommon.GetSection();
                        }
                        else
                        {
                            return RedirectToAction("ImportDataViewTo11thFailed", "ImportData");
                        }
                    }
                    catch (Exception ex)
                    {
                        return RedirectToAction("ImportDataViewTo11thFailed", "ImportData");
                    }

                }
                //return View(asm);
                return View(rm);
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [HttpPost]
        public ActionResult ImportDataViewTo11thFailedModify(RegistrationModels rm, FormCollection frm)
        {
            try
            {


                // AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();

                ViewBag.catg = objCommon.GetE2Category();
                ViewBag.MyBoard = objCommon.GetN2Board();

                ViewBag.Mon = objCommon.GetMonth();

                // AbstractLayer.DBClass objDBC = new AbstractLayer.DBClass();
                ViewBag.SessionYearList = objCommon.GetSessionYear();

                ViewBag.Caste = objCommon.GetCaste();

                ViewBag.DA = objCommon.GetDA();

                ViewBag.RE = objCommon.GetReligion();
                ViewBag.MyDist = objCommon.GetDistE();


                ViewBag.SectionList = objCommon.GetSection();
                ViewBag.MyTeh = objCommon.GetAllTehsil();
                string dist = rm.MyDistrict;

                if (ModelState.IsValid)
                {
                    // AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
                    string id = rm.Std_id.ToString();
                    string formname = "E1";

                    DataSet ds = objDB.SearchStudentGetByData_E_Import(id, formname);
                    //DataSet ds = objDB.SearchStudentGetByData(id);
                    string distOld = ds.Tables[0].Rows[0]["District"].ToString();
                    rm.MyDistrict = distOld;
                    //string stdPic = null;
                    string formName = "E1";
                    //if (frm["file"].ToString() != "")
                    if (rm.file != null)
                    {
                        //stdPic = Path.GetFileName(frm["file"]);
                        stdPic = Path.GetFileName(rm.file.FileName);

                    }
                    else
                    {

                        stdPic = ds.Tables[0].Rows[0]["std_Photo"].ToString();
                    }
                    if (rm.std_Sign != null)
                    {
                        //stdPic = Path.GetFileName(frm["file"]);
                        stdSign = Path.GetFileName(rm.std_Sign.FileName);

                    }
                    else
                    {

                        stdSign = ds.Tables[0].Rows[0]["std_Sign"].ToString();
                    }
                    //var stdPic = Path.GetFileName(rm.file.FileName);
                    //var stdSign = Path.GetFileName(rm.std_Sign.FileName);



                    string result = objDB.Update_E_Data(rm, frm, formName, id, stdPic, stdSign);
                    string filepathtosave = "";
                    if (result == "0")
                    {
                        //--------------Not updated
                        ViewData["resultUpdate"] = 0;
                    }
                    if (result == "-1")
                    {
                        //-----alredy exist
                        ViewData["resultUpdate"] = -1;
                    }
                    else
                    {
                        if (rm.file != null)
                        {
                            //var path = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + distOld + "/Photo"), result + "P" + ".jpg");
                            //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + distOld + "/Photo"));
                            //if (!Directory.Exists(FilepathExist))
                            //{
                            //    Directory.CreateDirectory(FilepathExist);
                            //}
                            //rm.file.SaveAs(path);
                            string Orgfile = result + "P" + ".jpg";
                            using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                            {
                                using (var newMemoryStream = new MemoryStream())
                                {
                                    var uploadRequest = new TransferUtilityUploadRequest
                                    {
                                        InputStream = rm.file.InputStream,
                                        Key = string.Format("allfiles/Upload2023/" + formName + "/" + distOld + "/Photo/{0}", Orgfile),
                                        BucketName = BUCKET_NAME,
                                        CannedACL = S3CannedACL.PublicRead
                                    };

                                    var fileTransferUtility = new TransferUtility(client);
                                    fileTransferUtility.Upload(uploadRequest);
                                }
                            }


                            filepathtosave = "allfiles/Upload2023/" + formName + "/" + distOld + "/Photo/" + result + "P" + ".jpg";
                            ViewBag.ImageURL = filepathtosave;

                            string PhotoName = formName + "/" + distOld + "/Photo" + "/" + result + "P" + ".jpg";
                            string type = "P";
                            string UpdatePic = objDB.Updated_Pic_Data(result, PhotoName, type);
                        }
                        if (rm.std_Sign != null)
                        {
                            //var path = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + distOld + "/Sign"), result + "S" + ".jpg");
                            //string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + distOld + "/Sign"));
                            //if (!Directory.Exists(FilepathExist))
                            //{
                            //    Directory.CreateDirectory(FilepathExist);
                            //}
                            //rm.std_Sign.SaveAs(path);
                            string Orgfile = result + "S" + ".jpg";
                            using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                            {
                                using (var newMemoryStream = new MemoryStream())
                                {
                                    var uploadRequest = new TransferUtilityUploadRequest
                                    {
                                        InputStream = rm.file.InputStream,
                                        Key = string.Format("allfiles/Upload2023/" + formName + "/" + distOld + "/Sign/{0}", Orgfile),
                                        BucketName = BUCKET_NAME,
                                        CannedACL = S3CannedACL.PublicRead
                                    };

                                    var fileTransferUtility = new TransferUtility(client);
                                    fileTransferUtility.Upload(uploadRequest);
                                }
                            }


                            filepathtosave = "../Upload/" + formName + "/" + distOld + "/Sign/" + result + "S" + ".jpg";
                            ViewBag.ImageURL = filepathtosave;

                            string PhotoName = formName + "/" + distOld + "/Sign" + "/" + result + "S" + ".jpg";
                            string type = "S";
                            string UpdatePic = objDB.Updated_Pic_Data(result, PhotoName, type);
                        }

                        ModelState.Clear();
                        //--For Showing Message---------//
                        ViewData["resultUpdate"] = result;
                        return RedirectToAction("ImportDataViewTo11thFailed", "ImportData");

                    }
                }
                return View(rm);
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        public ActionResult ImportDataViewTo11thFailedDelete(string id)
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (id == null)
                {
                    return RedirectToAction("ImportDataViewTo11thFailed", "ImportData");
                }
                //calling class DBClass
                string formname = "E1";

                DataSet ds = objDB.SearchStudentGetByData_E(id, formname);
                //DataSet ds = objDB.SearchStudentGetByData(id);
                string dist = ds.Tables[0].Rows[0]["District"].ToString();
                string imgPhoto = ds.Tables[0].Rows[0]["std_Photo"].ToString();
                string imgSign = ds.Tables[0].Rows[0]["std_Sign"].ToString();
                string formName = ds.Tables[0].Rows[0]["form_Name"].ToString();
                int OID = Convert.ToInt32(ds.Tables[0].Rows[0]["oldid"].ToString());
                string result = objDB.Delete_Import_11th_Failed_Data(id, OID); // passing Value to DBClass from model

                //--------Photo Delete------
                //RegistrationModels RM = new RegistrationModels();
                Import RM = new Import();
                var fileDesti = "";
                var fileDestiPic = "";



                var pathPhoto = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + dist + "/Photo"), imgPhoto);

                if (System.IO.File.Exists(pathPhoto))
                {
                    string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + dist + "/PhotoD"));
                    if (!Directory.Exists(FilepathExist))
                    {
                        Directory.CreateDirectory(FilepathExist);
                    }

                    //fileDesti = "../StdImages/DeletedPic/" + imgName;
                    fileDestiPic = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + dist + "/PhotoD"), imgPhoto);
                    System.IO.File.Move(pathPhoto, fileDestiPic);
                    System.IO.File.Delete(pathPhoto);

                }
                var pathSIGN = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + dist + "/Sign"), imgSign);
                if (System.IO.File.Exists(pathSIGN))
                {
                    string FilepathExist = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + dist + "/SignD"));
                    if (!Directory.Exists(FilepathExist))
                    {
                        Directory.CreateDirectory(FilepathExist);
                    }
                    //fileDesti = "../StdImages/DeletedPic/" + imgName;
                    fileDesti = Path.Combine(Server.MapPath("~/Upload/" + formName + "/" + dist + "/SignD"), imgSign);
                    System.IO.File.Move(pathSIGN, fileDesti);
                    System.IO.File.Delete(pathSIGN);

                }
                //------------End photo Delete------

                ViewData["resultDelete"] = result; // for dislaying message after saving storing output.
                return RedirectToAction("ImportDataViewTo11thFailed", "ImportData");
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }


        }

        //---------------------Import Data 2014-2015-2015----------
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ImportData10thpassed141516(int? page)
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }


                //var itemsch = new SelectList(new[]{new {ID="1",Name="Roll Number"},new{ID="2",Name="Candidate Name"},
                //new{ID="3",Name="Father's Name"},new{ID="4",Name="Mother's Name"},new{ID="5",Name="DOB"},}, "ID", "Name", 1);

                var itemsch = AbstractLayer.ImportDB.GetImportSearchListMain();
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = new SelectList(new[]{new {ID="2016",Name="2016"},new{ID="2015",Name="2015"},
            new{ID="2014",Name="2014"},}, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;



                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;


                //Search = "schl='" + schl + "' and CLASS='10' and importflag=0 and Category='10TH PASSED' and Year In('2014','2015','2016')";
                // Search = "schl='" + schl + "' and CLASS='10' and importflag is NULL and Category='10TH PASSED'";
                if (pageIndex == 1)
                {
                    TempData["ImportData10thpassed141516search3"] = null;
                    TempData["ImportData10thpassed141516SelList3"] = null;
                    TempData["ImportData10thpassed141516SearchString3"] = null;
                    TempData["ImportData10thpassed141516Session3"] = null;
                }
                if (TempData["ImportData10thpassed141516search3"] == null)
                    // Search = "schl='" + schl + "' and CLASS='10' and importflag is NULL and Category='10TH PASSED'";
                    //Search = "CLASS='10' and importflag is NULL and Category='10TH PASSED'";
                    Search = "CLASS='10' and Category='10TH PASSED'";
                else
                {
                    Search = Convert.ToString(TempData["ImportData10thpassed141516search3"]);
                    ViewBag.SelectedItem = TempData["ImportData10thpassed141516SelList3"];
                    ViewBag.Searchstring = TempData["ImportData10thpassed141516SearchString3"];
                    ViewBag.SelectedSession = TempData["ImportData10thpassed141516Session3"];
                }

                obj.StoreAllData = objDB.SelectAll10thPass1(Search, session, pageIndex);
                obj.TotalCount = objDB.GetAll10thPassCount1(Search, session, pageIndex);
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Data Doesn't Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {

                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int pn = tp / 10;
                    int cal = 10 * pn;
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
                return View();
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult ImportData10thpassed141516(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1)
        {
            try
            {


                var itemsch = AbstractLayer.ImportDB.GetImportSearchListMain();
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = new SelectList(new[]{new {ID="2016",Name="2016"},new{ID="2015",Name="2015"},
            new{ID="2014",Name="2014"},}, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;
                if (cmd == "Search")
                {
                    //Search = "((result in (''P'', ''C'', ''R'') and Year = ''2016'')or(result in (''P'') and Year in(''2015'', ''2014''))) and CLASS = '10'";
                    Search = "((result in ('P', 'C', 'R') and Year = '2016')or(result in ('P') and Year in('2015', '2014'))) and CLASS = '10'";
                    //Search = "((result in ('P', 'C', 'R') and Year = '2016') or(result in ('P') and Year in('2015', '2014') )) and CLASS = '10' and importflag is null";
                    // Search = "((result in ('P', 'C', 'R') and Year = '2016') or(result in ('P') and Year in('2015', '2014') )) and CLASS = '10' and importflag is null";
                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());

                        //if (Session1 != "")
                        //{
                        //    Search += " and a.Year='" + Session1.ToString().Trim() + "'";
                        //}

                        if (SearchString != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and a.ROLL='" + SearchString.ToString().Trim() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += " and  a.REGNO ='" + SearchString.ToString().Trim() + "'"; }
                            //else if (SelValueSch == 3)
                            //{ Search += " and a.FName  like '%" + SearchString.ToString().Trim() + "%'"; }
                            //else if (SelValueSch == 4)
                            //{ Search += " and a.MName like '%" + SearchString.ToString().Trim() + "%'"; }
                            //else if (SelValueSch == 5)
                            //{ Search += " and a.DOB='" + SearchString.ToString().Trim() + "'"; }
                        }


                    }
                    else
                    {
                        if (Session1 != "")
                        {
                            Search += " and a.Year='" + Session1.ToString().Trim() + "'";
                        }
                    }

                    TempData["ImportData10thpassed141516search3"] = Search;
                    TempData["ImportData10thpassed141516SelList3"] = SelList;
                    TempData["ImportData10thpassed141516SearchString3"] = SearchString.ToString().Trim();
                    TempData["ImportData10thpassed141516Session3"] = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    obj.StoreAllData = objDB.SelectAll10thPass1(Search, session, pageIndex);
                    obj.TotalCount = objDB.GetAll10thPassCount1(Search, session, pageIndex);
                    //rm.StoreAllData = objDB.GetStudentRecordsSearch_ImportData(Search);
                    // asm.StoreAllData = objDB.SearchSchoolDetails(Search);
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "DATA DOESN'T EXIST or allready imported. to check imported data click on \"Check Reg. No.of 2016 - 17\" link under Registration Menu.";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {

                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                        int pn = tp / 10;
                        int cal = 10 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        //return View(rm);
                        //ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        obj.chkidList = new List<ImportIDModel>();

                        ImportIDModel chk = null;
                        for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = obj.StoreAllData.Tables[0].Rows[i]["TID"].ToString(); // ID
                            chk.Name = "chkidList[" + i + "].id";
                            chk.Selected = false;
                            obj.chkidList.Add(chk);
                        }

                        return View(obj);

                    }
                }
                /////-----------------------Import Begins-------
                //string importToSchl = imp.schoolcode;
                string importToSchl = Session["SCHL"].ToString();
                if (imp.chkidList == null)
                { return RedirectToAction("ImportData10thpassed141516", "ImportData"); }
                int selectedList = imp.chkidList.Where(t => t.Selected).Count();
                TempData["TotImported"] = selectedList;
                DataTable dt = new DataTable();
                string collectId = "";
                //if (selectedList == 1)
                //{
                //    var selchklist = imp.chkidList.Where(t => t.Selected == true);
                //    var selchklistComma = imp.chkidList.Where(t => t.Selected == true).Select(s => s.id);
                //    collectId = selchklistComma.ToString();
                //   // string cl = collectId1.ToString();
                //    dt = objDB.Select_All_Pass_Data(importToSchl, collectId);
                //}
                //else
                //{
                var selchklist = imp.chkidList.Where(t => t.Selected == true);

                var selchklistComma = imp.chkidList.Where(t => t.Selected == true).Select(s => s.id);
                collectId = string.Join(",", selchklistComma);
                dt = objDB.Select_All_Pass_Data(importToSchl, collectId);
                //}


                if (dt == null || dt.Rows.Count == 0)
                {

                    ViewBag.Message = "Data Doesn't Exist";
                    ViewBag.TotalCount = 0;
                    TempData["result"] = -1;
                    return View();
                }
                else
                {
                    //-------------- Updated----------
                    // ViewData["result"] = 1;
                    TempData["result"] = 1;

                }

                return RedirectToAction("ImportData10thpassed141516", "ImportData");
                //return View(obj);
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        public ActionResult ImportDataSearch(Import imp, FormCollection frm)
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                var itemsch = new SelectList(new[]{new {ID="1",Name="Roll Number"},new{ID="2",Name="Candidate Name"},
            new{ID="3",Name="Father's Name"},new{ID="4",Name="Mother's Name"},new{ID="5",Name="DOB"},}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();

                //string sBy = frm["SelList"].ToString();
                // string sText = frm["SearchString"].ToString();

                Import rm = new Import();
                if (ModelState.IsValid)
                {

                    string Search = string.Empty;
                    string schlid = "";
                    if (Session["SCHL"] != null)
                    {
                        // session = Session["Session"].ToString();
                        schlid = Session["SCHL"].ToString();
                        // obj.schoolcode = schl;
                        List<SelectListItem> schllist = new List<SelectListItem>();

                        string SchoolAssign = "";
                        SchoolAssign = objDB.GetImpschlOcode(3, schlid);
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
                                schllist.Add(new SelectListItem { Text = schlid, Value = schlid });
                            }
                        }
                        //schllist.Add(new SelectListItem { Text = "---Select Import TO--", Value = "0" });                
                        ViewBag.MySchCode = schllist;
                    }
                    else
                    {
                        return View(rm);
                    }
                    // Search = "form_Name='E1' and schl='" + schlid + "' and a.std_id like '%' ";
                    Search = "ImportFormNameTo='E1' and schl='" + schlid + "' ";
                    if (frm["SelList"] != "")
                    {
                        ViewBag.SelectedItem = frm["SelList"];
                        int SelValueSch = Convert.ToInt32(frm["SelList"].ToString());


                        if (frm["SearchString"] != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and a.ROLL='" + frm["SearchString"].ToString() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += " and  a.Candi_Name like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 3)
                            { Search += " and a.Father_Name  like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 4)
                            { Search += " and a.Mother_Name like '%" + frm["SearchString"].ToString() + "%'"; }
                            else if (SelValueSch == 5)
                            { Search += " and a.DOB='" + frm["SearchString"].ToString() + "'"; }
                        }


                    }

                    rm.StoreAllData = objDB.GetStudentRecordsSearch_ImportData(Search);
                    // asm.StoreAllData = objDB.SearchSchoolDetails(Search);
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
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
                    return RedirectToAction("ImportData10thClass", "ImportData");
                }
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }

        }
        //-------------------------------TC REF-----------------

        //
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ImportData10thpassedTCRef(int? page)
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                var itemsch = new SelectList(new[]{new {ID="1",Name="Roll Number"},new{ID="2",Name="Candidate Name"},
            new{ID="3",Name="Father's Name"},new{ID="4",Name="Mother's Name"},new{ID="5",Name="DOB"},}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();


                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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


                return View(obj);
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [HttpPost]
        public ActionResult ImportData10thpassedTCRef(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString)
        {
            try
            {

                var itemsch = new SelectList(new[]{new {ID="1",Name="Roll Number"},new{ID="2",Name="Candidate Name"},
            new{ID="3",Name="Father's Name"},new{ID="4",Name="Mother's Name"},new{ID="5",Name="DOB"},}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                string session = null;
                string schl = null;

                Import obj = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    imp.schoolcode = schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;
                if (cmd == "Search")
                {

                    //Search = "TCrefno is not null and Tcstatus = 2  and ((result = 'PASS' and class='2') or (result in ('Fail','ABSENT') and class='3')) and  TCrefno = '" + SearchString+"'  ";               
                    // Search = "TCrefno is not null and Tcstatus = 2  and ((result = 'PASS' and class='2') or (result in ('Fail','ABSENT') and class='3')) and  (TCrefno = '" + SearchString + "' Or Registration_Num = '" + SearchString + "')  ";
                    Search = "TCrefno is not null  and ((result = 'PASS' and class='2') or (result in ('Fail','ABSENT') and class='3')) and  (TCrefno = '" + SearchString + "' Or Registration_Num = '" + SearchString + "')  ";
                    obj.StoreAllData = objDB.SelectTCStudents(Search, session);

                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "DATA DOESN'T EXIST or allready imported. to check imported data click on \"Check Reg. No.of 2016 - 17\" link under Registration Menu.";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {

                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        //return View(rm);
                        //ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        obj.chkidList = new List<ImportIDModel>();

                        ImportIDModel chk = null;
                        for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            chk.Selected = false;
                            obj.chkidList.Add(chk);
                        }

                        return View(obj);

                    }
                }
                /////-----------------------Import Begins-------
                // string importToSchl = imp.schoolcode;
                string importToSchl = Session["SCHL"].ToString();
                if (imp.chkidList == null)
                { return RedirectToAction("ImportData10thpassedTCRef", "ImportData"); }
                int selectedList = imp.chkidList.Where(t => t.Selected).Count();


                var selchklist = imp.chkidList.Where(t => t.Selected == true);

                var selchklistComma = imp.chkidList.Where(t => t.Selected == true).Select(s => s.id);
                var collectId = string.Join(",", selchklistComma);

                DataTable dt = new DataTable();
                TempData["TotImported"] = selectedList;
                dt = objDB.Select_All_11ThFailed_11THcontinue_TC(importToSchl, collectId);
                if (dt == null || dt.Rows.Count == 0)
                {
                    ViewBag.Message = "DATA DOESN'T EXIST or allready imported. to check imported data click on \"Check Reg. No.of 2016 - 17\" link under Registration Menu.";
                    ViewBag.TotalCount = 0;
                    TempData["result"] = -1;
                    return View();
                }
                else
                {
                    //-------------- Updated----------
                    // ViewData["result"] = 1;
                    TempData["result"] = 1;
                }

                return View();
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        //--------------------------------------Import Data 2017--------------
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult ImportedData2017(int? page)
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                //else
                //{
                //    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                //    if (status > 0)
                //    {
                //        if (status == 0)
                //        { return RedirectToAction("Index", "Home"); }
                //        else
                //        {
                //            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                //            if (result1.Tables[3].Rows.Count > 0)
                //            {
                //                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                //                {
                //                    return RedirectToAction("Index", "Home");
                //                }
                //            }
                //        }
                //    }
                //    else
                //    {
                //        return RedirectToAction("Index", "Home");
                //    }
                //}

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Roll Number" }, new { ID = "2", Name = "Registration Number" },
                new { ID = "3", Name = "TC Ref No" }, new { ID = "4", Name = "Aadhar Number" }, new { ID = "5", Name = "E-Punjab Id" }}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();

                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;



                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;


                //Search = "schl='" + schl + "' and CLASS='10' and importflag=0 and Category='10TH PASSED' and Year In('2014','2015','2016')";
                // Search = "schl='" + schl + "' and CLASS='10' and importflag is NULL and Category='10TH PASSED'";
                if (pageIndex == 1)
                {
                    TempData["search3"] = null;
                    TempData["SelList3"] = null;
                    TempData["SearchString3"] = null;
                    TempData["Session3"] = null;
                }
                if (TempData["search3"] == null) { }
                // Search = "schl='" + schl + "' and CLASS='10' and importflag is NULL and Category='10TH PASSED'";
                // Search = "CLASS='10' and importflag is NULL and Category='10TH PASSED'";
                else
                {
                    Search = Convert.ToString(TempData["search3"]);
                    ViewBag.SelectedItem = TempData["SelList3"];
                    ViewBag.Searchstring = TempData["SearchString3"];
                    ViewBag.SelectedSession = TempData["Session3"];
                }


                return View(obj);
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult ImportedData2017(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1)
        {
            try
            {


                //  var itemsch = new SelectList(new[] { new { ID = "1", Name = "Roll Number" }, new { ID = "2", Name = "Registration Number" }, }, "ID", "Name", 1);
                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Roll Number" }, new { ID = "2", Name = "Registration Number" },
                new { ID = "3", Name = "TC Ref No" }, new { ID = "4", Name = "Aadhar Number" }, new { ID = "5", Name = "E-Punjab Id" }}, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;
                if (cmd == "Search")
                {

                    //Search = "a.schl='"+schl+"' and a.oldSchlCode is Not null and a.OROLL is not Null";
                    //Search = "a.oldSchlCode is Not null and a.OROLL is not Null";
                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());

                        //if (Session1 != "")
                        //{
                        //    Search += " and a.Year='" + Session1.ToString().Trim() + "'";
                        //}

                        if (SearchString != "")
                        {
                            if (SelValueSch == 1)
                            //{ Search += " a.OROLL='" + SearchString.ToString().Trim() + "'"; }
                            { Search += " a.Board_Roll_Num='" + SearchString.ToString().Trim() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += "  a.Registration_num ='" + SearchString.ToString().Trim() + "'"; }
                            else if (SelValueSch == 3)
                            { Search += " a.tcrefno ='" + SearchString.ToString().Trim() + "'"; }
                            else if (SelValueSch == 4)
                            { Search += " a.Aadhar_num='" + SearchString.ToString().Trim() + "'"; }
                            else if (SelValueSch == 5)
                            { Search += " a.E_punjab_Std_id='" + SearchString.ToString().Trim() + "'"; }
                        }


                    }
                    else
                    {
                        if (Session1 != "")
                        {
                            Search += " and a.Year='" + Session1.ToString().Trim() + "'";
                        }
                    }

                    TempData["search3"] = Search;
                    TempData["SelList3"] = SelList;
                    TempData["SearchString3"] = SearchString.ToString().Trim();
                    TempData["Session3"] = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    obj.StoreAllData = objDB.SelectImportData2017(Search, session, pageIndex);//GetImportData2017
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Not Imported Yet.";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {

                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        return View(obj);

                    }
                }
                return View(obj);
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        //--------------------------------------------------Start 12 Th Import Data--------------
        #region  T1 Or 12Th Import Data

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Import11thCompartment(int? page)
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("AgreeResult", "RegistrationPortal", new { Form = "T1" });
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                var itemsch = AbstractLayer.ImportDB.GetImportSearchListMain();
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = new SelectList(new[]{new {ID="2016",Name="2016"},new{ID="2015",Name="2015"},
            new{ID="2014",Name="2014"},}, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;



                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;

                //Search = "form in('M1','M2') and result in ('pass','COMPARTMENT','reappear') and schl='"+ schl + "' and importflag is null";
                if (pageIndex == 1)
                {
                    TempData["ImportData11THFailedsearch1"] = null;
                    TempData["ImportData11THFailedSelList1"] = null;
                    TempData["ImportData11THFailedSearchString1"] = null;
                    TempData["ImportData11THFailedSession1"] = null;
                }
                if (TempData["ImportData11THFailedsearch1"] == null)
                    Search = "schl='" + schl + "' and form_name in ('E1','E2')  and result='COMPARTMENT'";
                else
                {
                    Search = Convert.ToString(TempData["ImportData11THFailedsearch1"]);
                    ViewBag.SelectedItem = TempData["ImportData11THFailedSelList1"];
                    ViewBag.Searchstring = TempData["ImportData11THFailedSearchString1"];
                    ViewBag.SelectedSession = TempData["ImportData11THFailedSession1"];
                }
                //----------------------------Pageload-----------------------
                //obj.StoreAllData = objDB.SelectAll11thCompartment(Search, session, pageIndex);
                ////rm.StoreAllData = objDB.SelectAll10thPass(Search, session, pageIndex);
                //obj.TotalCount = objDB.GetAll11thFailedCount(Search, session, pageIndex);
                //if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                //{
                //    ViewBag.Message = "DATA DOESN'T EXIST or allready imported. to check imported data click on \"Check Reg. No.of 2016 - 17\" link under Registration Menu.";
                //    ViewBag.TotalCount = 0;
                //}
                //else
                //{
                //    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                //    ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                //    int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                //    int pn = tp / 10;
                //    int cal = 10 * pn;
                //    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                //    if (res >= 1)
                //        ViewBag.pn = pn + 1;
                //    else
                //        ViewBag.pn = pn;
                //    obj.chkidList = new List<ImportIDModel>();

                //    ImportIDModel chk = null;
                //    for (int i = 0; i < ViewBag.TotalCount; i++)
                //    {
                //        chk = new ImportIDModel();
                //        chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                //        chk.Name = "chkidList[" + i + "].id";
                //        chk.Selected = false;
                //        obj.chkidList.Add(chk);
                //    }


                //}
                //---------------------End Pageload--------------------------
                return View(obj);
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult Import11thCompartment(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1)
        {
            try
            {


                var itemsch = AbstractLayer.ImportDB.GetImportSearchListMain();
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = new SelectList(new[]{new {ID="2016",Name="2016"},new{ID="2015",Name="2015"},
            new{ID="2014",Name="2014"},}, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();
                string session = null;
                string schl = null;
                string Search = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                Import rm = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    //obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                if (cmd == "Search")
                {
                    Search = "schl ='" + schl + "' and form_name in ('E1','E2') and (result='Compartment') ";
                    //Search = "schl ='" + schl + "' and class=3 and (result='Compartment') ";
                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SearchString != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " and a.OROLL='" + SearchString.ToString().Trim() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += " and  a.REGISTRATION_NUM like '%" + SearchString.ToString().Trim() + "%'"; }
                        }
                    }
                    //else
                    //{
                    //    Search = "schl='" + schl + "' and cat='11TH FAILED' and importflag is null and result='FAIL'";
                    //}

                    TempData["ImportData11THFailedsearch1"] = Search;
                    TempData["ImportData11THFailedSelList1"] = SelList;
                    TempData["ImportData11THFailedSearchString1"] = SearchString.ToString().Trim();
                    TempData["ImportData11THFailedSession1"] = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    rm.StoreAllData = objDB.SelectAll11thCompartment(Search, session, pageIndex);
                    rm.TotalCount = objDB.GetAll11thCompartmentCount(Search, session, pageIndex);
                    //rm.StoreAllData = objDB.GetStudentRecordsSearch_ImportData(Search);
                    // asm.StoreAllData = objDB.SearchSchoolDetails(Search);
                    if (rm.StoreAllData == null || rm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        //ViewBag.Message = "DATA DOESN'T EXIST or allready imported. to check imported data click on \"Check Reg. No.of 2016 - 17\" link under Registration Menu.";
                        ViewBag.Message = "This Data Is Not in Compartment.";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = rm.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(rm.TotalCount.Tables[0].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(rm.TotalCount.Tables[0].Rows[0]["decount"]);
                        int pn = tp / 10;
                        int cal = 10 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        //return View(rm);
                        //ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        rm.chkidList = new List<ImportIDModel>();

                        ImportIDModel chk = null;
                        for (int i = 0; i < rm.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = rm.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            chk.Selected = false;
                            rm.chkidList.Add(chk);
                        }

                        return View(rm);

                    }
                }
                /////-----------------------Import Begins-------
                // string importToSchl = imp.schoolcode;
                string importToSchl = Session["SCHL"].ToString();
                if (imp.chkidList == null)
                { return RedirectToAction("ImportData11THFailed", "ImportData"); }
                int selectedList = imp.chkidList.Where(t => t.Selected).Count();
                TempData["TotImported"] = selectedList;

                var selchklist = imp.chkidList.Where(t => t.Selected == true);

                var selchklistComma = imp.chkidList.Where(t => t.Selected == true).Select(s => s.id);
                var collectId = string.Join(",", selchklistComma);

                DataTable dt = new DataTable();
                dt = objDB.Select_All_11ThCompartment_Data(importToSchl, collectId);
                if (dt == null || dt.Rows.Count == 0)
                {
                    // return RedirectToAction("ImportData10thClass", "ImportData");
                    ViewBag.Message = "Data Doesn't Exist";
                    ViewBag.TotalCount = 0;
                    TempData["result"] = -1;
                    return View();
                }
                else
                {
                    //-------------- Updated----------
                    TempData["result"] = 1;
                }


                return View();
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        #region Import11thPass Self

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Import11thPass(int? page)
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("AgreeResult", "RegistrationPortal", new { Form = "T1" });
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }


                var schoolTypeList = new SelectList(new[] { new { ID = "Self", Name = "Self School" }, new { ID = "Other", Name = "Other School" } }, "ID", "Name", 1);
                ViewBag.MySchoolType = schoolTypeList.ToList();

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "ALL" }, new { ID = "2", Name = "Candidate Name" }, new { ID = "3", Name = "Registration Number" },
                    new { ID = "7", Name = "Aadhar Number" },new { ID = "8", Name = "TC Refno No." },
                    new { ID = "4", Name = "OLD ID" }, new { ID = "5", Name = "Old Roll" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();


                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                FormCollection frm = new FormCollection();
                string Search = string.Empty;
                if (TempData["Import11thPasssearch"] != null)
                {

                    Search = Convert.ToString(TempData["Import11thPasssearch"]);
                    ViewBag.SelectedItem = TempData["Import11thPassSelList"];
                    ViewBag.Searchstring = TempData["Import11thPassSearchString"];
                    ViewBag.SelectedSession = TempData["Import11thPassSession"];

                    //---------------Fill Data On page Load-----------------          
                    //obj.StoreAllData = objDB.SelectAllImport9thPass(Search, session, pageIndex, ViewBag.SelectedSession);
                    obj.StoreAllData = objDB.SelectAllImport11thPass(Search, session, pageIndex, ViewBag.SelectedSession);
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;


                    }
                }
                //---------------End Fill Data On page Load-----------------
                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult Import11thPass(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1, string chkImportid, string SchoolType)
        {
            try
            {

                var schoolTypeList = new SelectList(new[] { new { ID = "Self", Name = "Self School" }, new { ID = "Other", Name = "Other School" } }, "ID", "Name", 1);
                ViewBag.MySchoolType = schoolTypeList.ToList();

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "ALL" }, new { ID = "2", Name = "Candidate Name" }, new { ID = "3", Name = "Registration Number" },
                    new { ID = "7", Name = "Aadhar Number" },new { ID = "8", Name = "TC Refno No." },
                    new { ID = "4", Name = "OLD ID" }, new { ID = "5", Name = "Old Roll" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();



                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();


                string result = "";
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;


                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    // obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                string Search = "";
                if (cmd == "Search")
                {
                    Search = " CLASS = 3 ";

                    if (SchoolType.ToString().ToUpper() == "SELF")
                    {
                        Search += " and schl ='" + imp.schoolcode + "'";
                    }

                    //  if (SearchString != "")
                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SelValueSch == 1)
                        {
                            Search += " " + SearchString.ToString().Trim();

                        }
                        else if (SelValueSch == 2)
                        {
                            Search += " and Candi_Name like '%" + SearchString.ToString().Trim() + "%'";
                        }
                        else if (SelValueSch == 3)
                        {
                            Search += " and registration_num like '%" + SearchString.ToString().Trim() + "%'";
                        }
                        else if (SelValueSch == 4)
                        {
                            Search += " and std_ID =" + SearchString.ToString().Trim();
                        }
                        else if (SelValueSch == 5)
                        {
                            Search += " and Class_Roll_Num_Section =" + SearchString.ToString().Trim();

                        }
                        else if (SelValueSch == 7)
                        {
                            Search += " and Aadhar_num ='" + SearchString.ToString().Trim() + "'";

                        }
                        else if (SelValueSch == 8)
                        {
                            Search = " TCrefno='" + SearchString.ToString().Trim() + "'";
                        }

                    }



                    TempData["Import11thPasssearch"] = Search;
                    TempData["Import11thPassSelList"] = SelList;
                    TempData["Import11thPassSearchString"] = SearchString.ToString().Trim();
                    TempData["Import11thPassSession"] = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();

                    imp.StoreAllData = objDB.SelectAllImport11thPass(Search, session, pageIndex, Session1);//GetAllImport11thPassNew
                    int cn = imp.StoreAllData.Tables[0].Rows.Count;     //objDB.GetAll11thPassCount(Search, session, pageIndex);               
                    if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);

                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;


                        return View(imp);
                    }
                }
                else
                {
                    /////-----------------------Import Begins-------
                    string importToSchl = Session["SCHL"].ToString();

                    string collectId = frm["cbImportSelected"];
                    int cnt = collectId.Count(x => x == ',');
                    TempData["TotImported"] = cnt + 1;

                    string CurrentSchl = Session["SCHL"].ToString();
                    DataTable dt = new DataTable();

                    dt = objDB.Import11thPass(importToSchl, CurrentSchl, collectId, Session1);//Import11thPass                
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        // return RedirectToAction("ImportData10thClass", "ImportData");
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        ViewData["result"] = -1;
                        return View();
                    }
                    else
                    {
                        //-------------- Updated----------
                        // ViewData["result"] = 1;                       
                        TempData["result"] = 1;
                    }

                    if (TempData["Import11thPasssearch"] != null)
                    {
                        Search = Convert.ToString(TempData["Import11thPasssearch"]);
                        ViewBag.SelectedItem = TempData["Import11thPassSelList"];
                        ViewBag.Searchstring = TempData["Import11thPassSearchString"];
                        ViewBag.SelectedSession = TempData["Import11thPassSession"];
                        //---------------Fill Data On page Load-----------------          
                        //obj.StoreAllData = objDB.SelectAllImport9thPass(Search, session, pageIndex, ViewBag.SelectedSession);
                        imp.StoreAllData = objDB.SelectAllImport11thPass(Search, session, pageIndex, ViewBag.SelectedSession);
                        if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Data Doesn't Exist";
                            ViewBag.TotalCount = 0;
                        }
                        else
                        {
                            ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                            ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                            int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                            int pn = tp / 20;
                            int cal = 20 * pn;
                            int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                            if (res >= 1)
                                ViewBag.pn = pn + 1;
                            else
                                ViewBag.pn = pn;


                        }
                    }
                    return View(imp);
                    // return RedirectToAction("ImportData10thClass", "ImportData");
                }
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        #endregion Import11thPass

        #region Import12thFail
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Import12thFail(string id, int? page)
        {
            try
            {
                if (id == null || id == "")
                {
                    TempData["Import12thFailsearch"] = null;
                    return RedirectToAction("T1Master", "RegistrationPortal");
                }
                ViewBag.SO = id;
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("AgreeResult", "RegistrationPortal", new { Form = "T1" });
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                var itemsch = AbstractLayer.ImportDB.GetImportSearchListMain();
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;


                if (TempData["Import12thFailsearch"] != null)
                {
                    Search = Convert.ToString(TempData["Import12thFailsearch"]);
                    ViewBag.SelectedItem = TempData["Import12thFailSelList"];
                    ViewBag.Searchstring = TempData["Import12thFailSearchString"];
                    ViewBag.SelectedSession = TempData["Import12thFailSession"];

                    //---------------Fill Data On page Load-----------------          
                    // SelAllImport12thFail_SpN1
                    obj.StoreAllData = objDB.SelAllImport12thFail(schl, pageIndex, ViewBag.SelectedSession, Search);
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        // ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                        ViewBag.TotalCount1 = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
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

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult Import12thFail(string id, int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1, string chkImportid)
        {
            try
            {
                if (id == null || id == "")
                {
                    TempData["Import12thFailsearch"] = null;
                    return RedirectToAction("T1Master", "RegistrationPortal");
                }
                ViewBag.SO = id;
                var itemsch = AbstractLayer.ImportDB.GetImportSearchListMain();
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();
                string result = "";
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                // Import rm = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    // obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                string Search = "";
                if (cmd == "Search")
                {


                    if (id.ToString().ToUpper() == "SELF")
                    {
                        Search = " schl ='" + imp.schoolcode + "'";
                    }
                    if (SelList != "")
                    {
                        if (Search != "")
                        {
                            Search += " and ";
                        }
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SelValueSch == 1)
                        {
                            Search += "  ROLL='" + frm["SearchString"].ToString() + "'";
                        }

                        else if (SelValueSch == 2)
                        {
                            Search += "  REGNO like '%" + SearchString.ToString().Trim() + "%'";
                        }
                        else if (SelValueSch == 7)
                        {
                            Search += "  Aadhar_num ='" + SearchString.ToString().Trim() + "'";

                        }
                    }


                    TempData["Import12thFailsearch"] = Search;
                    TempData["Import12thFailSelList"] = SelList;
                    TempData["Import12thFailSearchString"] = SearchString.ToString().Trim();
                    TempData["Import12thFailSession"] = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    ViewBag.SelectedSession = Session1;
                    imp.StoreAllData = objDB.SelAllImport12thFail(schl, pageIndex, Session1, Search);//SelAllImport12thFail_Sp
                    if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        return View(imp);
                    }
                }
                else
                {

                    /////-----------------------Import Begins-------
                    // string importToSchl = Session["SCHL"].ToString();
                    string importToSchl = frm["ImportSchlTo"];
                    string collectId = frm["cbImportSelected"];
                    int cnt = collectId.Count(x => x == ',');
                    TempData["TotImported"] = cnt + 1;
                    string CurrentSchl = Session["SCHL"].ToString();
                    string Sub = String.Empty;
                    //   collectId = "'" + collectId + "'";
                    collectId = "" + collectId + "";
                    DataTable dt = new DataTable();

                    // dt = objDB.GetStudent12thFail(importToSchl, chkImportid, Session1);

                    //ImportStudent12thFailN1
                    if (SelList != "")
                    {
                        if (id.ToString().ToUpper() == "SELF")
                        {
                            Search = " schl ='" + imp.schoolcode + "'";
                        }
                        //Change by Harpal sir  Any student can  be imported either self or other
                        //else { Search = " schl !='" + imp.schoolcode + "'"; }

                        if (SelList != "")
                        {
                            if (Search != "")
                            {
                                Search += " and ";
                            }
                            ViewBag.SelectedItem = SelList;
                            int SelValueSch = Convert.ToInt32(SelList.ToString());
                            if (SelValueSch == 1)
                            {
                                Search += "  ROLL='" + frm["SearchString"].ToString() + "'";
                            }

                            else if (SelValueSch == 2)
                            {
                                Search += "  REGNO like '%" + SearchString.ToString().Trim() + "%'";
                            }
                            else if (SelValueSch == 7)
                            {
                                Search += "  Aadhar_num ='" + SearchString.ToString().Trim() + "'";

                            }
                        }
                    }
                    //ImportStudent12thFailN1
                    dt = objDB.ImportStudent12thFail(importToSchl, CurrentSchl, collectId, Session1);

                    // dt = objDB.ImportStudent12thFail(importToSchl, CurrentSchl, collectId, Session1);//ImportStudent12thFail
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        // return RedirectToAction("ImportData10thClass", "ImportData");
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        ViewData["result"] = -1;
                        return View();
                    }
                    else
                    {
                        TempData["result"] = 1;
                        ViewData["result"] = 1;
                    }

                    if (TempData["Import12thFailsearch"] != null)
                    {
                        Search = Convert.ToString(TempData["Import12thFailsearch"]);
                        ViewBag.SelectedItem = TempData["Import12thFailSelList"];
                        ViewBag.Searchstring = TempData["Import12thFailSearchString"];
                        ViewBag.SelectedSession = TempData["Import12thFailSession"];

                        //---------------Fill Data On page Load-----------------          
                        imp.StoreAllData = objDB.SelAllImport12thFail(schl, pageIndex, ViewBag.SelectedSession, Search);//SelAllImport12thFail_Sp
                        if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Data Doesn't Exist";
                            ViewBag.TotalCount = 0;
                        }
                        else
                        {
                            ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                            ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                            int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                            int pn = tp / 20;
                            int cal = 20 * pn;
                            int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                            if (res >= 1)
                                ViewBag.pn = pn + 1;
                            else
                                ViewBag.pn = pn;


                        }
                    }

                    return View(imp);
                }
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View(imp);
            }
        }
        #endregion Import12thFail
        //---------------------------------------------------End 12 Th Import Data-----------------
        //---------------------------------11th Pass TC Ref--------------------//
        //#region Import11thPassedTCRef Other School        
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //public ActionResult Import11thPassedTCRef(int? page)
        //{
        //    try
        //    {
        //        //var itemsch = new SelectList(new[] { new { ID = "1", Name = "TC No." }, new { ID = "2", Name = "Registration Number" }, new { ID = "3", Name = "Student Name" }, }, "ID", "Name", 1);
        //        //ViewBag.MySch = itemsch.ToList();

        //        if (Session["SCHL"] == null)
        //        {
        //            return RedirectToAction("Logout", "Login");
        //        }
        //        else
        //        {
        //            int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
        //            if (status > 0)
        //            {
        //                if (status == 0)
        //                { return RedirectToAction("Index", "Home"); }
        //                else
        //                {
        //                    DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
        //                    if (result1.Tables[3].Rows.Count > 0)
        //                    {
        //                        if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
        //                        {
        //                            return RedirectToAction("Index", "Home");
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                return RedirectToAction("Index", "Home");
        //            }
        //        }

        //        Import obj = new Import();
        //        string session = null;
        //        string schl = null;

        //        if (Session["Session"] != null)
        //        {
        //            session = Session["Session"].ToString();
        //            schl = Session["SCHL"].ToString();
        //            obj.schoolcode = schl;
        //            List<SelectListItem> schllist = new List<SelectListItem>();

        //            string SchoolAssign = "";
        //            SchoolAssign = objDB.GetImpschlOcode(3, schl);
        //            if (SchoolAssign == null || SchoolAssign == "")
        //            { return RedirectToAction("Index", "Login"); }
        //            else
        //            {
        //                if (SchoolAssign.Contains(','))
        //                {
        //                    string[] s = SchoolAssign.Split(',');
        //                    //  int Cs =    s.Count;
        //                    foreach (string schlcode in s)
        //                    {
        //                        schllist.Add(new SelectListItem { Text = schlcode, Value = schlcode });
        //                    }
        //                }
        //                else
        //                {
        //                    schllist.Add(new SelectListItem { Text = schl, Value = schl });
        //                }
        //            }
        //            //schllist.Add(new SelectListItem { Text = "---Select Import TO--", Value = "0" });                
        //            ViewBag.MySchCode = schllist;
        //            string Search = string.Empty;
        //            string SearchString = string.Empty;

        //            if (TempData["ImportDataT1TCREFSearch"] != null)
        //            {
        //                Search = TempData["ImportDataT1TCREFSearch"].ToString();
        //                SearchString = TempData["ImportDataT1TCREFSearchString"].ToString();

        //                //-------------------------------------------------------Page Load Start----------                     

        //                obj.StoreAllData = objDB.SelectImport11thPassedTCRef_N(Search, SearchString);   //SelectTCStudents9thPassed
        //                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
        //                {
        //                    ViewBag.Message = "Record Not Found";
        //                    ViewBag.TotalCount = 0;
        //                    return View();
        //                }
        //                else
        //                {

        //                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
        //                    obj.chkidList = new List<ImportIDModel>();
        //                    ImportIDModel chk = null;
        //                    //int SelValueSch = Convert.ToInt32(SelList.ToString());
        //                    if (TempData["SelValueSch"].ToString() == "1")
        //                    {
        //                        //  Search = " TCrefno='" + SearchString.ToString().Trim() + "'";
        //                        for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
        //                        {
        //                            chk = new ImportIDModel();
        //                            chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
        //                            chk.Name = "chkidList[" + i + "].id";
        //                            chk.Selected = false;
        //                            obj.chkidList.Add(chk);
        //                        }
        //                    }
        //                    else if (TempData["SelValueSch"].ToString() == "2" || TempData["SelValueSch"].ToString() == "3" || TempData["SelValueSch"].ToString() == "7")
        //                    {
        //                        //   Search += " registration_num ='" + SearchString.ToString().Trim() + "'";
        //                        for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
        //                        {
        //                            chk = new ImportIDModel();
        //                            chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
        //                            chk.Name = "chkidList[" + i + "].id";
        //                            chk.Selected = false;
        //                            obj.chkidList.Add(chk);
        //                        }
        //                    }                           
        //                    else
        //                    {
        //                        for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
        //                        {
        //                            chk = new ImportIDModel();
        //                            chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
        //                            chk.Name = "chkidList[" + i + "].id";
        //                            chk.Selected = false;
        //                            obj.chkidList.Add(chk);
        //                        }
        //                    }

        //                    return View(obj);

        //                }

        //                //--------------------------------End------------------------
        //            }


        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }


        //        return View(obj);
        //    }
        //    catch (Exception ex)
        //    {

        //        oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
        //        return View();
        //    }
        //}
        //[HttpPost]
        //public ActionResult Import11thPassedTCRef(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string SearchStringschl, string SearchStringfnm)
        //{
        //    try
        //    {
        //        //var itemsch = new SelectList(new[] { new { ID = "1", Name = "TC No." }, new { ID = "2", Name = "Registration Number" }, new { ID = "3", Name = "Student Name" }, }, "ID", "Name", 1);
        //        //ViewBag.MySch = itemsch.ToList();

        //        string session = null;
        //        string schl = null;
        //        Import obj = new Import();
        //        if (Session["Session"] != null)
        //        {
        //            session = Session["Session"].ToString();
        //            imp.schoolcode = schl = Session["SCHL"].ToString();
        //            obj.schoolcode = schl;
        //            ViewBag.Searchstring = SearchString;
        //            ViewBag.Searchstringschl = SearchStringschl;
        //            ViewBag.Searchstringfnm = SearchStringfnm;
        //            List<SelectListItem> schllist = new List<SelectListItem>();

        //            string SchoolAssign = "";
        //            SchoolAssign = objDB.GetImpschlOcode(3, schl);
        //            if (SchoolAssign == null || SchoolAssign == "")
        //            { return RedirectToAction("Index", "Login"); }
        //            else
        //            {
        //                if (SchoolAssign.Contains(','))
        //                {
        //                    string[] s = SchoolAssign.Split(',');
        //                    //  int Cs =    s.Count;
        //                    foreach (string schlcode in s)
        //                    {
        //                        schllist.Add(new SelectListItem { Text = schlcode, Value = schlcode });
        //                    }
        //                }
        //                else
        //                {
        //                    schllist.Add(new SelectListItem { Text = schl, Value = schl });
        //                }
        //            }
        //            //schllist.Add(new SelectListItem { Text = "---Select Import TO--", Value = "0" });                
        //            ViewBag.MySchCode = schllist;
        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }

        //        string Search = string.Empty;
        //        if (cmd == "Search")
        //        {
        //            ////Search = "TCrefno is not null  and ((result = 'PASS' and class='1')) and (Lot>0)  and  TCrefno = '" + SearchString + "'  ";
        //            ////Search = "TCrefno is not null and TCstatus=2 and SCHL!='"+schl+"'  and class='1' and  TCrefno = '" + SearchString + "'  ";
        //            //// Search = "SCHL!='" + schl + "'  and class='3' and  TCrefno = '" + SearchString + "'  ";
        //            //Search = "class='3' and  TCrefno = '" + SearchString + "'  ";

        //            ViewBag.SelectedItem = SelList;
        //            int SelValueSch = Convert.ToInt32(SelList.ToString());
        //            TempData["SelValueSch"] = SelValueSch;
        //            if (SelValueSch == 1)
        //            {
        //                // Search += " Roll ='" + SearchString.ToString().Trim() + "'";
        //                Search = " TCrefno='" + SearchString.ToString().Trim() + "'";
        //            }
        //            else if (SelValueSch == 2)
        //            {
        //                //Search += " registration_num like '%" + SearchString.ToString().Trim() + "%'";
        //                Search += " registration_num ='" + SearchString.ToString().Trim() + "'";
        //            }
        //            else if (SelValueSch == 3)
        //            {
        //                Search += " schl ='" + SearchStringschl.ToString().Trim() + "' and candi_name like '%" + SearchString.ToString().Trim() + "%' and father_name like '%" + SearchStringfnm.ToString().Trim() + "%'";
        //            }
        //            else if (SelValueSch == 7)
        //            {
        //                Search += "  Aadhar_num ='" + SearchString.ToString().Trim() + "'";

        //            }
        //            TempData["ImportDataT1TCREFSearch"] = Search;
        //            TempData["ImportDataT1TCREFSearchString"] = SearchString.ToString().Trim();

        //            obj.StoreAllData = objDB.SelectImport11thPassedTCRef_N(Search, SearchString);   //SelectTCStudents9thPassed
        //            if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
        //            {
        //                ViewBag.Message = "Record Not Found";
        //                ViewBag.TotalCount = 0;
        //                return View();
        //            }
        //            else
        //            {

        //                ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
        //                obj.chkidList = new List<ImportIDModel>();
        //                ImportIDModel chk = null;

        //                //int SelValueSch = Convert.ToInt32(SelList.ToString());
        //                if (SelValueSch == 1)
        //                {
        //                    //  Search = " TCrefno='" + SearchString.ToString().Trim() + "'";
        //                    for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
        //                    {
        //                        chk = new ImportIDModel();
        //                        chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
        //                        chk.Name = "chkidList[" + i + "].id";
        //                        chk.Selected = false;
        //                        obj.chkidList.Add(chk);
        //                    }
        //                }
        //                else if (SelValueSch == 2 || SelValueSch == 3 || SelValueSch == 7)
        //                {
        //                    //   Search += " registration_num ='" + SearchString.ToString().Trim() + "'";
        //                    for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
        //                    {
        //                        chk = new ImportIDModel();
        //                        chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
        //                        chk.Name = "chkidList[" + i + "].id";
        //                        chk.Selected = false;
        //                        obj.chkidList.Add(chk);
        //                    }
        //                }

        //                else
        //                {
        //                    for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
        //                    {
        //                        chk = new ImportIDModel();
        //                        chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
        //                        chk.Name = "chkidList[" + i + "].id";
        //                        chk.Selected = false;
        //                        obj.chkidList.Add(chk);
        //                    }
        //                }

        //                return View(obj);

        //            }
        //        }
        //        /////-----------------------Import Begins-------
        //        // string importToSchl = imp.schoolcode;
        //        string importToSchl = Session["SCHL"].ToString();
        //        if (imp.chkidList == null)
        //        { return RedirectToAction("ImportData11thfailedTCRef", "ImportData"); }
        //        int selectedList = imp.chkidList.Where(t => t.Selected).Count();


        //        var selchklist = imp.chkidList.Where(t => t.Selected == true);

        //        var selchklistComma = imp.chkidList.Where(t => t.Selected == true).Select(s => s.id);
        //        var collectId = string.Join(",", selchklistComma);

        //        DataTable dt = new DataTable();
        //        TempData["TotImported"] = selectedList;
        //        string cls = "3";

        //        dt = objDB.Import11thPassedTCRef_N(importToSchl, collectId, SearchString, cls); //Select_All_9ThPassed_Continue_TC(importToSchl, collectId, session);

        //        string Sub = String.Empty;
        //        if (dt == null || dt.Rows.Count == 0)
        //        {
        //            ViewBag.Message = "Data Doesn't Exist";
        //            ViewBag.TotalCount = 0;
        //            TempData["result"] = -1;
        //            return View();
        //        }
        //        else
        //        {
        //            //-------------- Updated----------
        //            // ViewData["result"] = 1;
        //            TempData["result"] = 1;
        //        }

        //        return View();
        //    }
        //    catch (Exception ex)
        //    {

        //        oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
        //        return View();
        //    }
        //}
        //[HttpPost]
        //public ActionResult CancelTc()
        //{

        //    TempData["SearchString"] = null;
        //    return RedirectToAction("Import11thPassedTCRef", "ImportData");
        //}
        //#endregion Import11thPassedTCRef Other School
        //------------------End 11th Pass TC Ref----------------------
        //---------------------------------11th Pass TC Ref--------------------//
        #region Import11thPassAnySchoolRegNum
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Import11thPassAnySchoolRegNum(int? page)
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[3].Rows.Count > 0)
                            {
                                if (result1.Tables[3].Rows[0]["reclock11th"].ToString() == "0")
                                {
                                    return RedirectToAction("AgreeResult", "RegistrationPortal", new { Form = "T1" });
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Registration Number" }, new { ID = "2", Name = "Unique ID" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                //var sessionsrc = new SelectList(new[]{new {ID="2016",Name="2016"},new{ID="2015",Name="2015"},
                //new{ID="2014",Name="2014"},}, "ID", "Name", 1);
                var sessionsrc = new SelectList(new[] { new { ID = "2019", Name = "2019" }, new { ID = "2016", Name = "2016" }, }, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;
                FormCollection frm = new FormCollection();
                //Search = "form in('M1','M2') and result in ('pass','COMPARTMENT','reappear') and schl='"+ schl + "' and importflag is null";
                if (pageIndex == 1)
                {

                    TempData["Import11thPasssearch"] = null;
                    TempData["Import11thPassSelList"] = null;
                    TempData["Import11thPassSearchString"] = null;
                    //TempData["Session"] = null;
                    TempData["Import11thPassSession"] = null;
                    //ViewBag.SelectedSession = frm["Session"].ToString();
                }
                if (TempData["search"] == null)
                    //Search = "((result in ('P', 'C', 'R') and Year = '2016') or(result in ('P') and Year in('2015', '2014') )) and schl = '" + schl + "' and CLASS = '10' and importflag is null";
                    Search = "schl ='" + schl + "' and CLASS = 3 and result ='PASS' and importflag is null";
                else
                {
                    Search = Convert.ToString(TempData["Import11thPasssearch"]);
                    ViewBag.SelectedItem = TempData["Import11thPassSelList"];
                    ViewBag.Searchstring = TempData["Import11thPassSearchString"];
                    ViewBag.SelectedSession = TempData["Import11thPassSession"];
                }
                //---------------Fill Data On page Load-----------------
                obj.StoreAllData = objDB.SelectAllImport11thPass(Search, session, pageIndex, ViewBag.SelectedSession);
                obj.TotalCount = objDB.SelectAllImport11thPassCount(Search, session, pageIndex, ViewBag.SelectedSession);
                //obj.TotalCount = objDB.GetAll11thPassCount(Search, session, pageIndex);
                //int totCN = obj.StoreAllData.Tables[0].Rows.Count;
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Data Doesn't Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                    ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int tp = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                    int pn = tp / 30;
                    int cal = 30 * pn;
                    int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                    if (res >= 1)
                        ViewBag.pn = pn + 1;
                    else
                        ViewBag.pn = pn;
                    //return View(rm);
                    //ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;

                    //obj.chkidList = new List<ImportIDModel>();

                    //ImportIDModel chk = null;
                    //for (int i = 0; i < ViewBag.TotalCount; i++)
                    //{
                    //    chk = new ImportIDModel();
                    //    //chk.id = obj.StoreAllData.Tables[0].Rows[i]["TID"].ToString();
                    //    chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                    //    chk.Name = "chkidList[" + i + "].id";
                    //    chk.Selected = false;
                    //    obj.chkidList.Add(chk);
                    //}


                    obj.chkidList = new List<ImportIDModel>();

                    ImportIDModel chk = null;
                    for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                    {
                        chk = new ImportIDModel();
                        chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                        chk.Name = "chkidList[" + i + "].id";
                        if (obj.StoreAllData.Tables[0].Rows.Count == 1)
                            ViewBag.impid = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                        chk.Selected = false;
                        obj.chkidList.Add(chk);
                    }
                }
                //---------------End Fill Data On page Load-----------------

                return View(obj);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [HttpPost]
        public ActionResult Import11thPassAnySchoolRegNum(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1, string chkImportid)
        {
            try
            {


                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Registration Number" }, new { ID = "2", Name = "Unique ID" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = new SelectList(new[] { new { ID = "2019", Name = "2019" }, new { ID = "2017", Name = "2017" }, new { ID = "2016", Name = "2016" }, }, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();
                string result = "";
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                // Import rm = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    // obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                string Search = "";
                string Search1 = "";
                if (cmd == "Search")
                {
                    Search = "CLASS = 3 and result ='PASS' and (Lot>0) ";


                    if (Session1 != "")
                    {
                        //Search += " and Year='" + Session1.ToString().Trim() + "'";
                        // imp.chkidList = null;
                        // ModelState.Clear();
                        //ModelState.Remove("ImportIDModel");
                    }

                    //  if (SearchString != "")
                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());

                        if (SelValueSch == 1)
                        {
                            Search1 = Search + " and Registration_Num='" + SearchString.ToString().Trim() + "'";
                            Search += " and REGNO='" + SearchString.ToString().Trim() + "'";

                        }
                        else if (SelValueSch == 2)
                        {
                            Search1 += Search + " and Std_ID='" + SearchString.ToString().Trim() + "'";
                            Search += " and ID='" + SearchString.ToString().Trim() + "'";

                            // Search += " and ID =" + SearchString.ToString().Trim();
                        }
                        //  ModelState.Clear();
                        //ModelState.Remove("ImportIDModel");
                        // imp.chkidList = null;

                    }


                    TempData["Import11thPasssearch"] = Search;
                    TempData["Import11thPassSelList"] = SelList;
                    TempData["Import11thPassSearchString"] = SearchString.ToString().Trim();
                    TempData["Import11thPassSession"] = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    string importToSchl = Session["SCHL"].ToString();
                    imp.StoreAllData = objDB.SelectAllImport11thPassOtherSchool(Search, Search1, session, Session1);
                    //imp.StoreAllData = objDB.SelectAllImport11thPass(Search, session, pageIndex, Session1);
                    int cn = imp.StoreAllData.Tables[0].Rows.Count;     //objDB.GetAll11thPassCount(Search, session, pageIndex);


                    // imp.TotalCount = objDB.SelectAllImport11thPassCount(Search, session, pageIndex, Session1);
                    //rm.StoreAllData = objDB.GetStudentRecordsSearch_ImportData(Search);
                    //asm.StoreAllData = objDB.SearchSchoolDetails(Search);

                    if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                        //ViewBag.TotalCount1 = Convert.ToInt32(imp.TotalCount.Tables[0].Rows[0]["decount"]);
                        //int tp = Convert.ToInt32(imp.TotalCount.Tables[0].Rows[0]["decount"]);
                        //int pn = tp / 30;
                        //int cal = 30 * pn;
                        //int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        //if (res >= 1)
                        //    ViewBag.pn = pn + 1;
                        //else
                        //    ViewBag.pn = pn;
                        //return View(rm);
                        //ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        imp.chkidList = new List<ImportIDModel>();

                        ImportIDModel chk = null;
                        for (int i = 0; i < imp.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = imp.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            if (imp.StoreAllData.Tables[0].Rows.Count == 1)
                                ViewBag.impid = imp.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Selected = false;
                            imp.chkidList.Add(chk);
                        }

                        return View(imp);
                    }
                }
                else
                {
                    /////-----------------------Import Begins-------
                    //  string importToSchl = imp.schoolcode;
                    string importToSchl = Session["SCHL"].ToString();
                    if (imp.chkidList == null)
                    { return RedirectToAction("Import11thPassAnySchoolRegNum", "ImportData"); }
                    int selectedList = imp.chkidList.Where(t => t.Selected).Count();
                    TempData["TotImported"] = selectedList;

                    var selchklist = imp.chkidList.Where(t => t.Selected == true);

                    var selchklistComma = imp.chkidList.Where(t => t.Selected == true).Select(s => s.id);
                    var collectId = string.Join(",", selchklistComma);

                    DataTable dt = new DataTable();
                    //dt = objDB.Select_All_Pass_Data(importToSchl, collectId);
                    if (chkImportid != "")
                    {
                        dt = objDB.Select_All_11ThPassed_Other_School(importToSchl, chkImportid, Session1);
                    }

                    else
                    {
                        dt = objDB.Select_All_11ThPassed_Other_School(importToSchl, collectId, Session1);
                    }

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        // return RedirectToAction("ImportData10thClass", "ImportData");
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        ViewData["result"] = -1;
                        return View();
                    }
                    else
                    {

                        //-------------- Updated----------
                        // ViewData["result"] = 1;                     
                        TempData["Import11thPasssearch"] = null;
                        TempData["result"] = 1;

                    }

                    return View();
                    // return RedirectToAction("ImportData10thClass", "ImportData");
                }
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [HttpPost]
        public ActionResult CancelRegNUm()
        {

            TempData["SearchString"] = null;
            return RedirectToAction("Import11thPassAnySchoolRegNum", "ImportData");
        }
        #endregion Import11thPassAnySchoolRegNum

        #region   
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Import12thRegNotExm(string id, int? page)
        {
            try
            {
                if (id == null || id == "")
                {
                    TempData["Import10thFailsearch"] = null;
                    return RedirectToAction("M1Master", "RegistrationPortal");
                }
                ViewBag.SO = id;
                if (Session["SCHL"] == null)
                {
                    TempData["Import10thFailsearch"] = null;
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[2].Rows.Count > 0)
                            {
                                if (result1.Tables[2].Rows[0]["reclock9th"].ToString() == "0")
                                {
                                    return RedirectToAction("AgreeResult", "RegistrationPortal", new { Form = "M1" });
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                var itemsch = new SelectList(new[] { new { ID = "2", Name = "Registration Number" }, new { ID = "7", Name = "Aadhar Number" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;

                //Search = "class=10 and (Result in ('F','A') or (year!='2017' and (result in ('R','C')))) and importflag is null";
                if (TempData["Import10thFailsearch"] != null)
                {
                    Search = Convert.ToString(TempData["Import10thFailsearch"]);
                    ViewBag.SelectedItem = TempData["Import10thFailSelList"];
                    ViewBag.Searchstring = TempData["Import10thFailSearchString"];
                    ViewBag.SelectedSession = TempData["Import10thFailSession"];
                    //SelAllImport10thFail_SpN1
                    //---------------Fill Data On page Load-----------------          
                    obj.StoreAllData = objDB.SelAllImport12thFail_NTE(schl, pageIndex, ViewBag.SelectedSession, Search);  //SelAllImport10thFail_SpN1
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        // ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                        ViewBag.TotalCount1 = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
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
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult Import12thRegNotExm(string id, int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1, string chkImportid)
        {
            try
            {
                if (id == null || id == "")
                {
                    TempData["Import10thFailsearch"] = null;
                    return RedirectToAction("M1Master", "RegistrationPortal");
                }
                ViewBag.SO = id;
                var itemsch = new SelectList(new[] { new { ID = "2", Name = "Registration Number" }, new { ID = "7", Name = "Aadhar Number" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();
                string result = "";
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                // Import rm = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    // obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                string Search = "";
                if (cmd == "Search")
                {
                    if (id.ToString().ToUpper() == "SELF")
                    {
                        Search = " schl ='" + imp.schoolcode + "'";
                    }
                    //Change by Harpal sir  Any student can  be imported either self or other
                    //else { Search = " schl !='" + imp.schoolcode + "'"; }

                    if (SelList != "")
                    {
                        if (Search != "")
                        {
                            Search += " and ";
                        }
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SelValueSch == 1)
                        {
                            Search += "  ROLL='" + frm["SearchString"].ToString() + "'";
                        }
                        else if (SelValueSch == 2)
                        {
                            Search += "  registration_num = '" + SearchString.ToString().Trim() + "'";
                        }
                        else if (SelValueSch == 7)
                        {
                            Search += "  Aadhar_num ='" + SearchString.ToString().Trim() + "'";

                        }
                    }

                    ViewBag.SelectedItem = SelList;
                    TempData["Import10thFailsearch"] = Search;
                    TempData["Import10thFailSelList"] = SelList;
                    TempData["Import10thFailSearchString"] = SearchString.ToString().Trim();
                    TempData["Import10thFailSession"] = Session1;
                    ViewBag.SelectedSession = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    //SelAllImport10thFail_SpN1
                    imp.StoreAllData = objDB.SelAllImport12thFail_NTE(schl, pageIndex, Session1, Search);//SelAllImport10thFail_Sp
                                                                                                         //  int cn = imp.StoreAllData.Tables[0].Rows.Count;     //objDB.GetAll11thPassCount(Search, session, pageIndex);

                    if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        return View(imp);
                    }
                }
                else
                {
                    /////-----------------------Import Begins-------
                    //  string importToSchl = imp.schoolcode;
                    //string importToSchl = Session["SCHL"].ToString();
                    string importToSchl = frm["ImportSchlTo"];
                    //string collectId =  frm["cbImportSelected"];
                    string collectId = frm["cbImportSelected"];
                    int cnt = collectId.Count(x => x == ',');
                    TempData["TotImported"] = cnt + 1;
                    string CurrentSchl = Session["SCHL"].ToString();
                    DataTable dt = new DataTable();
                    //collectId = "'" + collectId + "'";
                    collectId = "" + collectId + "";
                    string Sub = String.Empty;
                    // ImportStudent10thFailN1
                    if (SelList != "")
                    {
                        if (id.ToString().ToUpper() == "SELF")
                        {
                            Search = " schl ='" + imp.schoolcode + "'";
                        }
                        //Change by Harpal sir  Any student can  be imported either self or other
                        //else { Search = " schl !='" + imp.schoolcode + "'"; }

                        if (SelList != "")
                        {
                            if (Search != "")
                            {
                                Search += " and ";
                            }
                            ViewBag.SelectedItem = SelList;
                            int SelValueSch = Convert.ToInt32(SelList.ToString());
                            if (SelValueSch == 1)
                            {
                                Search += "  ROLL='" + frm["SearchString"].ToString() + "'";
                            }
                            else if (SelValueSch == 2)
                            {
                                Search += "  registration_num = '" + SearchString.ToString().Trim() + "'";
                            }
                        }
                    }
                    //ImportStudent10thFailN1
                    dt = objDB.ImportStudent12thFail_NTE(importToSchl, CurrentSchl, collectId, Session1);//ImportStudent10thFailN1

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        // return RedirectToAction("ImportData10thClass", "ImportData");
                        ViewBag.Message = "Data Not Imported";
                        ViewData["result"] = -1;
                        return View();
                    }
                    else
                    {
                        //TempData["Import10thFailsearch"] = null;
                        TempData["result"] = 1;
                        ViewData["result"] = 1;
                    }

                    /////
                    if (TempData["Import10thFailsearch"] != null)
                    {
                        Search = Convert.ToString(TempData["Import10thFailsearch"]);
                        ViewBag.SelectedItem = TempData["Import10thFailSelList"];
                        ViewBag.Searchstring = TempData["Import10thFailSearchString"];
                        ViewBag.SelectedSession = TempData["Import10thFailSession"];

                        //---------------Fill Data On page Load-----------------          
                        imp.StoreAllData = objDB.SelAllImport12thFail_NTE(schl, pageIndex, ViewBag.SelectedSession, Search);
                        if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Data Doesn't Exist";
                            ViewBag.TotalCount = 0;
                        }
                        else
                        {
                            ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                            // ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                            ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                            int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                            int pn = tp / 20;
                            int cal = 20 * pn;
                            int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                            if (res >= 1)
                                ViewBag.pn = pn + 1;
                            else
                                ViewBag.pn = pn;


                        }
                    }

                    return View(imp);
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        #endregion  Import12thRegNotExm
        #endregion  T1 Or 12Th Import Data
        //------------------End Import11thPassAnySchoolRegNum----------------------



        //---------------------------------------------
        //--------------------------------------------------Start M1 Or 10Th Import Data--------------


        #region  M1 Or 10Th Import Data


        #region  Import9thPass       
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Import9thPass(int? page)
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(1, Session["SCHL"].ToString()); // changed by ranjan 17oct
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[2].Rows.Count > 0)
                            {
                                if (result1.Tables[2].Rows[0]["reclock9th"].ToString() == "0")
                                {
                                    return RedirectToAction("AgreeResult", "RegistrationPortal", new { Form = "M1" });
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "ALL" }, new { ID = "2", Name = "Candidate Name" }, new { ID = "3", Name = "Registration Number" },
                    new { ID = "7", Name = "Aadhar Number" },new { ID = "8", Name = "TC Refno No." },
                    new { ID = "4", Name = "OLD ID" }, new { ID = "5", Name = "Old Roll" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();


                var schoolTypeList = new SelectList(new[] { new { ID = "Self", Name = "Self School" }, new { ID = "Other", Name = "Other School" } }, "ID", "Name", 1);
                ViewBag.MySchoolType = schoolTypeList.ToList();

                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;
                //Search = "schl ='" + schl + "' and CLASS = 1 and (lot>0 or LOT<>NULL) and result in('PASS','COMPARTMENT')  ";
                if (TempData["Import9thPasssearch"] != null)
                {

                    Search = Convert.ToString(TempData["Import9thPasssearch"]);
                    ViewBag.SelectedItem = TempData["Import9thPassSelList"];
                    ViewBag.Searchstring = TempData["Import9thPassSearchString"];
                    ViewBag.SelectedSession = TempData["Import9thPassSession"];

                    //---------------Fill Data On page Load-----------------          
                    obj.StoreAllData = objDB.SelectAllImport9thPass(Search, session, pageIndex, ViewBag.SelectedSession);
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        // ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                        ViewBag.TotalCount1 = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;


                    }
                }
                //---------------End Fill Data On page Load-----------------

                return View(obj);

            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult Import9thPass(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1, string chkImportid, string SchoolType)
        {
            try
            {


                var schoolTypeList = new SelectList(new[] { new { ID = "Self", Name = "Self School" }, new { ID = "Other", Name = "Other School" } }, "ID", "Name", 1);
                ViewBag.MySchoolType = schoolTypeList.ToList();

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "ALL" }, new { ID = "2", Name = "Candidate Name" }, new { ID = "3", Name = "Registration Number" },
                    new { ID = "7", Name = "Aadhar Number" },new { ID = "8", Name = "TC Refno No." },
                    new { ID = "4", Name = "OLD ID" }, new { ID = "5", Name = "Old Roll" }, }, "ID", "Name", 1);

                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();
                string result = "";
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                // Import rm = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    // obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                string Search = "";
                if (cmd == "Search")
                {
                    Search = " CLASS = 1 ";

                    if (SchoolType.ToString().ToUpper() == "SELF")
                    {
                        Search += " and schl ='" + imp.schoolcode + "'";
                    }

                    //  if (SearchString != "")
                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SelValueSch == 1)
                        {
                            Search += " " + SearchString.ToString().Trim();

                        }
                        else if (SelValueSch == 2)
                        {
                            Search += " and Candi_Name like '%" + SearchString.ToString().Trim() + "%'";
                        }
                        else if (SelValueSch == 3)
                        {
                            Search += " and registration_num like '%" + SearchString.ToString().Trim() + "%'";
                        }
                        else if (SelValueSch == 4)
                        {
                            Search += " and std_ID =" + SearchString.ToString().Trim();
                        }
                        else if (SelValueSch == 5)
                        {
                            Search += " and Class_Roll_Num_Section =" + SearchString.ToString().Trim();

                        }
                        else if (SelValueSch == 7)
                        {
                            Search += " and Aadhar_num ='" + SearchString.ToString().Trim() + "'";

                        }
                        else if (SelValueSch == 8)
                        {
                            Search = " TCrefno='" + SearchString.ToString().Trim() + "'";
                        }

                    }

                    TempData["Import9thPasssearch"] = Search;
                    TempData["ImpImport9thPassedTCRefort9thPassSelList"] = SelList;
                    TempData["Import9thPassSearchString"] = SearchString.ToString().Trim();
                    TempData["Import9thPassSession"] = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    imp.StoreAllData = objDB.SelectAllImport9thPass(Search, session, pageIndex, Session1);//GetAllImport9thPass
                    int cn = imp.StoreAllData.Tables[0].Rows.Count;     //objDB.GetAll11thPassCount(Search, session, pageIndex);

                    if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);

                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        return View(imp);
                    }
                }
                else
                {
                    /////-----------------------Import Begins-------                  

                    string importToSchl = Session["SCHL"].ToString();

                    string collectId = frm["ChkCTenthClass"];
                    int cnt = collectId.Count(x => x == ',');
                    TempData["TotImported"] = cnt + 1;

                    string CurrentSchl = Session["SCHL"].ToString();
                    DataTable dt = new DataTable();
                    //collectId = "'" + collectId + "'";   

                    //---- Import Selected Record ----------------
                    dt = objDB.Import9thPassSP(importToSchl, CurrentSchl, collectId, Session1);//GetStudentPassNinthN_sp_29042017

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        ViewData["result"] = -1;
                    }
                    else
                    {
                        TempData["result"] = 1;
                    }

                    if (TempData["Import9thPasssearch"] != null)
                    {

                        Search = Convert.ToString(TempData["Import9thPasssearch"]);
                        ViewBag.SelectedItem = TempData["Import9thPassSelList"];
                        ViewBag.Searchstring = TempData["Import9thPassSearchString"];
                        ViewBag.SelectedSession = TempData["Import9thPassSession"];

                        //---------------Fill Data On page Load-----------------          
                        imp.StoreAllData = objDB.SelectAllImport9thPass(Search, session, pageIndex, ViewBag.SelectedSession);
                        if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Data Doesn't Exist";
                            ViewBag.TotalCount = 0;
                        }
                        else
                        {
                            ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                            // ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                            ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                            int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                            int pn = tp / 20;
                            int cal = 20 * pn;
                            int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                            if (res >= 1)
                                ViewBag.pn = pn + 1;
                            else
                                ViewBag.pn = pn;


                        }
                    }
                    return View(imp);
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        #endregion  Import9thPass

        #region  Import9thReappear       
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Import9thReappear(int? page)
        {
            try
            {
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(1, Session["SCHL"].ToString()); // changed by ranjan 17oct
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[2].Rows.Count > 0)
                            {
                                if (result1.Tables[2].Rows[0]["reclock9th"].ToString() == "0")
                                {
                                    return RedirectToAction("AgreeResult", "RegistrationPortal", new { Form = "M1" });
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Roll" }, new { ID = "2", Name = "Registration Number" }, new { ID = "3", Name = "Candidate ID" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();

                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;
                //Search = "schl ='" + schl + "' and CLASS = 1 and (lot>0 or LOT<>NULL) and result in('PASS','COMPARTMENT')  ";
                if (TempData["Import9thPasssearchReappear"] != null)
                {

                    Search = Convert.ToString(TempData["Import9thPasssearchReappear"]);
                    ViewBag.SelectedItem = TempData["Import9thPassSelListReappear"];
                    ViewBag.Searchstring = TempData["Import9thPassSearchStringReappear"];
                    ViewBag.SelectedSession = TempData["Import9thPassSessionReappear"];

                    //---------------Fill Data On page Load-----------------          
                    obj.StoreAllData = objDB.GetAllImport9thReappear(Search, session, pageIndex, ViewBag.SelectedSession);//GetAllImport9thReappear
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        // ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                        ViewBag.TotalCount1 = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int pn = tp / 30;
                        int cal = 30 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;


                    }
                }
                //---------------End Fill Data On page Load-----------------

                return View(obj);

            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult Import9thReappear(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1, string chkImportid)
        {
            try
            {

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Roll" }, new { ID = "2", Name = "Registration Number" }, new { ID = "3", Name = "Candidate ID" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();

                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();
                string result = "";
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                // Import rm = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    // obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                string Search = "";
                if (cmd == "Search")
                {
                    ////////
                    Search = "schl ='" + imp.schoolcode + "' and Class=1 ";
                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SelValueSch == 1)
                        {
                            Search += " and  Roll ='" + SearchString.ToString().Trim() + "'";
                        }
                        else if (SelValueSch == 2)
                        {
                            Search += "and registration_num like '%" + SearchString.ToString().Trim() + "%'";
                        }
                        else if (SelValueSch == 3)
                        {
                            Search += "and Std_Id =" + SearchString.ToString().Trim();
                        }
                    }



                    TempData["Import9thPasssearchReappear"] = Search;
                    TempData["Import9thPassSelListReappear"] = SelList;
                    TempData["Import9thPassSearchStringReappear"] = SearchString.ToString().Trim();
                    TempData["Import9thPassSessionReappear"] = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    ////imp.StoreAllData = objDB.SelectAllImport9thReappear(Search, session, pageIndex, Session1); //GetAllImport9thReappear
                    imp.StoreAllData = objDB.GetAllImport9thReappear(Search, session, pageIndex, Session1);//GetAllImport9thReappear

                    int cn = imp.StoreAllData.Tables[0].Rows.Count;     //objDB.GetAll11thPassCount(Search, session, pageIndex);

                    if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);

                        int pn = tp / 30;
                        int cal = 30 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        //return View(rm);
                        //ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        imp.chkidList = new List<ImportIDModel>();

                        ImportIDModel chk = null;
                        for (int i = 0; i < imp.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = imp.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            if (imp.StoreAllData.Tables[0].Rows.Count == 1)
                                ViewBag.impid = imp.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Selected = false;
                            imp.chkidList.Add(chk);
                        }

                        return View(imp);
                    }
                }
                else
                {
                    /////-----------------------Import Begins-------                  

                    string importToSchl = Session["SCHL"].ToString();

                    string collectId = frm["ChkCTenthClass"];
                    int cnt = collectId.Count(x => x == ',');
                    TempData["TotImported"] = cnt + 1;

                    string CurrentSchl = Session["SCHL"].ToString();
                    DataTable dt = new DataTable();


                    dt = objDB.ImportStudent9thReappear(importToSchl, CurrentSchl, collectId, Session1);//ImportStudent9thReappear
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        ViewData["result"] = -1;
                        return View();
                    }
                    else
                    {
                        TempData["Import9thPasssearchReappear"] = null;
                        TempData["result"] = 1;
                        return View(imp);
                    }
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        #endregion  Import9thReappear

        #region  Import10thFail


        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Import10thFail(string id, int? page)
        {
            try
            {
                if (id == null || id == "")
                {
                    TempData["Import10thFailsearch"] = null;
                    return RedirectToAction("M1Master", "RegistrationPortal");
                }
                ViewBag.SO = id;
                if (Session["SCHL"] == null)
                {
                    TempData["Import10thFailsearch"] = null;
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(1, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[2].Rows.Count > 0)
                            {
                                if (result1.Tables[2].Rows[0]["reclock9th"].ToString() == "0")
                                {
                                    return RedirectToAction("AgreeResult", "RegistrationPortal", new { Form = "M1" });
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                var itemsch = AbstractLayer.ImportDB.GetImportSearchListMain();
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;

                //Search = "class=10 and (Result in ('F','A') or (year!='2017' and (result in ('R','C')))) and importflag is null";
                if (TempData["Import10thFailsearch"] != null)
                {
                    Search = Convert.ToString(TempData["Import10thFailsearch"]);
                    ViewBag.SelectedItem = TempData["Import10thFailSelList"];
                    ViewBag.Searchstring = TempData["Import10thFailSearchString"];
                    ViewBag.SelectedSession = TempData["Import10thFailSession"];
                    //SelAllImport10thFail_SpN1
                    //---------------Fill Data On page Load-----------------          
                    obj.StoreAllData = objDB.SelAllImport10thFail(schl, pageIndex, ViewBag.SelectedSession, Search);  //SelAllImport10thFail_SpN1
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        // ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                        ViewBag.TotalCount1 = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
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
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult Import10thFail(string id, int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1, string chkImportid)
        {
            try
            {
                if (id == null || id == "")
                {
                    TempData["Import10thFailsearch"] = null;
                    return RedirectToAction("M1Master", "RegistrationPortal");
                }
                ViewBag.SO = id;
                var itemsch = AbstractLayer.ImportDB.GetImportSearchListMain();
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();
                string result = "";
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                // Import rm = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    // obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                string Search = "";
                if (cmd == "Search")
                {
                    if (id.ToString().ToUpper() == "SELF")
                    {
                        Search = " schl ='" + imp.schoolcode + "'";
                    }
                    //Change by Harpal sir  Any student can  be imported either self or other
                    //else { Search = " schl !='" + imp.schoolcode + "'"; }

                    if (SelList != "")
                    {
                        if (Search != "")
                        {
                            Search += " and ";
                        }
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SelValueSch == 1)
                        {
                            Search += "  ROLL='" + frm["SearchString"].ToString() + "'";
                        }
                        else if (SelValueSch == 2)
                        {
                            Search += "  REGNO like '%" + SearchString.ToString().Trim() + "%'";
                        }
                    }

                    ViewBag.SelectedItem = SelList;
                    TempData["Import10thFailsearch"] = Search;
                    TempData["Import10thFailSelList"] = SelList;
                    TempData["Import10thFailSearchString"] = SearchString.ToString().Trim();
                    TempData["Import10thFailSession"] = Session1;
                    ViewBag.SelectedSession = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    //SelAllImport10thFail_SpN1
                    imp.StoreAllData = objDB.SelAllImport10thFail(schl, pageIndex, Session1, Search);//SelAllImport10thFail_Sp
                                                                                                     //  int cn = imp.StoreAllData.Tables[0].Rows.Count;     //objDB.GetAll11thPassCount(Search, session, pageIndex);

                    if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        return View(imp);
                    }
                }
                else
                {
                    /////-----------------------Import Begins-------
                    //  string importToSchl = imp.schoolcode;
                    //string importToSchl = Session["SCHL"].ToString();
                    string importToSchl = frm["ImportSchlTo"];
                    //string collectId =  frm["cbImportSelected"];
                    string collectId = frm["cbImportSelected"];
                    int cnt = collectId.Count(x => x == ',');
                    TempData["TotImported"] = cnt + 1;
                    string CurrentSchl = Session["SCHL"].ToString();
                    DataTable dt = new DataTable();
                    //collectId = "'" + collectId + "'";
                    collectId = "" + collectId + "";
                    string Sub = String.Empty;
                    // ImportStudent10thFailN1
                    if (SelList != "")
                    {
                        if (id.ToString().ToUpper() == "SELF")
                        {
                            Search = " schl ='" + imp.schoolcode + "'";
                        }
                        //Change by Harpal sir  Any student can  be imported either self or other
                        //else { Search = " schl !='" + imp.schoolcode + "'"; }

                        if (SelList != "")
                        {
                            if (Search != "")
                            {
                                Search += " and ";
                            }
                            ViewBag.SelectedItem = SelList;
                            int SelValueSch = Convert.ToInt32(SelList.ToString());
                            if (SelValueSch == 1)
                            {
                                Search += "  ROLL='" + frm["SearchString"].ToString() + "'";
                            }
                            else if (SelValueSch == 2)
                            {
                                Search += "  REGNO like '%" + SearchString.ToString().Trim() + "%'";
                            }
                            else if (SelValueSch == 7)
                            {
                                Search += "  Aadhar_num='" + frm["SearchString"].ToString() + "'";
                            }
                        }
                    }
                    //ImportStudent10thFailN1
                    dt = objDB.ImportStudent10thFail(importToSchl, CurrentSchl, collectId, Session1);//ImportStudent10thFailN1

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        // return RedirectToAction("ImportData10thClass", "ImportData");
                        ViewBag.Message = "Data Not Imported";
                        ViewData["result"] = -1;
                        return View();
                    }
                    else
                    {
                        //TempData["Import10thFailsearch"] = null;
                        TempData["result"] = 1;
                        ViewData["result"] = 1;
                    }

                    /////
                    if (TempData["Import10thFailsearch"] != null)
                    {
                        Search = Convert.ToString(TempData["Import10thFailsearch"]);
                        ViewBag.SelectedItem = TempData["Import10thFailSelList"];
                        ViewBag.Searchstring = TempData["Import10thFailSearchString"];
                        ViewBag.SelectedSession = TempData["Import10thFailSession"];

                        //---------------Fill Data On page Load-----------------          
                        imp.StoreAllData = objDB.SelAllImport10thFail(schl, pageIndex, ViewBag.SelectedSession, Search);
                        if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Data Doesn't Exist";
                            ViewBag.TotalCount = 0;
                        }
                        else
                        {
                            ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                            // ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                            ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                            int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                            int pn = tp / 20;
                            int cal = 20 * pn;
                            int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                            if (res >= 1)
                                ViewBag.pn = pn + 1;
                            else
                                ViewBag.pn = pn;


                        }
                    }

                    return View(imp);
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        #endregion  Import10thFail

        #region  Import10thRegNotExm 
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Import10thRegNotExm(string id, int? page)
        {
            try
            {
                if (id == null || id == "")
                {
                    TempData["Import10thFailsearch"] = null;
                    return RedirectToAction("M1Master", "RegistrationPortal");
                }
                ViewBag.SO = id;
                if (Session["SCHL"] == null)
                {
                    TempData["Import10thFailsearch"] = null;
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(1, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[2].Rows.Count > 0)
                            {
                                if (result1.Tables[2].Rows[0]["reclock9th"].ToString() == "0")
                                {
                                    return RedirectToAction("AgreeResult", "RegistrationPortal", new { Form = "M1" });
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                var itemsch = new SelectList(new[] { new { ID = "2", Name = "Registration Number" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();
                ViewBag.MySession = sessionsrc.ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;

                //Search = "class=10 and (Result in ('F','A') or (year!='2017' and (result in ('R','C')))) and importflag is null";
                if (TempData["Import10thFailsearch"] != null)
                {
                    Search = Convert.ToString(TempData["Import10thFailsearch"]);
                    ViewBag.SelectedItem = TempData["Import10thFailSelList"];
                    ViewBag.Searchstring = TempData["Import10thFailSearchString"];
                    ViewBag.SelectedSession = TempData["Import10thFailSession"];
                    //SelAllImport10thFail_SpN1
                    //---------------Fill Data On page Load-----------------          
                    obj.StoreAllData = objDB.SelAllImport10thFail_NTE(schl, pageIndex, ViewBag.SelectedSession, Search);  //SelAllImport10thFail_SpN1
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        // ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                        ViewBag.TotalCount1 = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(obj.StoreAllData.Tables[1].Rows[0]["decount"]);
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
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        [HttpPost]
        public ActionResult Import10thRegNotExm(string id, int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1, string chkImportid)
        {
            try
            {
                if (id == null || id == "")
                {
                    TempData["Import10thFailsearch"] = null;
                    return RedirectToAction("M1Master", "RegistrationPortal");
                }
                ViewBag.SO = id;
                var itemsch = new SelectList(new[] { new { ID = "2", Name = "Registration Number" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = AbstractLayer.ImportDB.GetImportSessionLast3();

                ViewBag.MySession = sessionsrc.ToList();
                string result = "";
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                // Import rm = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    // obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                string Search = "";
                if (cmd == "Search")
                {
                    if (id.ToString().ToUpper() == "SELF")
                    {
                        Search = " schl ='" + imp.schoolcode + "'";
                    }
                    //Change by Harpal sir  Any student can  be imported either self or other
                    //else { Search = " schl !='" + imp.schoolcode + "'"; }

                    if (SelList != "")
                    {
                        if (Search != "")
                        {
                            Search += " and ";
                        }
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SelValueSch == 1)
                        {
                            Search += "  ROLL='" + frm["SearchString"].ToString() + "'";
                        }
                        else if (SelValueSch == 2)
                        {
                            Search += "  registration_num = '" + SearchString.ToString().Trim() + "'";
                        }
                    }

                    ViewBag.SelectedItem = SelList;
                    TempData["Import10thFailsearch"] = Search;
                    TempData["Import10thFailSelList"] = SelList;
                    TempData["Import10thFailSearchString"] = SearchString.ToString().Trim();
                    TempData["Import10thFailSession"] = Session1;
                    ViewBag.SelectedSession = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    //SelAllImport10thFail_SpN1
                    imp.StoreAllData = objDB.SelAllImport10thFail_NTE(schl, pageIndex, Session1, Search);//SelAllImport10thFail_Sp
                                                                                                         //  int cn = imp.StoreAllData.Tables[0].Rows.Count;     //objDB.GetAll11thPassCount(Search, session, pageIndex);

                    if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                        int pn = tp / 20;
                        int cal = 20 * pn;
                        int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                        if (res >= 1)
                            ViewBag.pn = pn + 1;
                        else
                            ViewBag.pn = pn;
                        return View(imp);
                    }
                }
                else
                {
                    /////-----------------------Import Begins-------
                    //  string importToSchl = imp.schoolcode;
                    //string importToSchl = Session["SCHL"].ToString();
                    string importToSchl = frm["ImportSchlTo"];
                    //string collectId =  frm["cbImportSelected"];
                    string collectId = frm["cbImportSelected"];
                    int cnt = collectId.Count(x => x == ',');
                    TempData["TotImported"] = cnt + 1;
                    string CurrentSchl = Session["SCHL"].ToString();
                    DataTable dt = new DataTable();
                    //collectId = "'" + collectId + "'";
                    collectId = "" + collectId + "";
                    string Sub = String.Empty;
                    // ImportStudent10thFailN1
                    if (SelList != "")
                    {
                        if (id.ToString().ToUpper() == "SELF")
                        {
                            Search = " schl ='" + imp.schoolcode + "'";
                        }
                        //Change by Harpal sir  Any student can  be imported either self or other
                        //else { Search = " schl !='" + imp.schoolcode + "'"; }

                        if (SelList != "")
                        {
                            if (Search != "")
                            {
                                Search += " and ";
                            }
                            ViewBag.SelectedItem = SelList;
                            int SelValueSch = Convert.ToInt32(SelList.ToString());
                            if (SelValueSch == 1)
                            {
                                Search += "  ROLL='" + frm["SearchString"].ToString() + "'";
                            }
                            else if (SelValueSch == 2)
                            {
                                Search += "  registration_num = '" + SearchString.ToString().Trim() + "'";
                            }
                            else if (SelValueSch == 7)
                            {
                                Search += "  registration_num = '" + SearchString.ToString().Trim() + "'";
                            }
                        }
                    }
                    //ImportStudent10thFailN1
                    dt = objDB.ImportStudent10thFail_NTE(importToSchl, CurrentSchl, collectId, Session1);//ImportStudent10thFailN1

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        // return RedirectToAction("ImportData10thClass", "ImportData");
                        ViewBag.Message = "Data Not Imported";
                        ViewData["result"] = -1;
                        return View();
                    }
                    else
                    {
                        //TempData["Import10thFailsearch"] = null;
                        TempData["result"] = 1;
                        ViewData["result"] = 1;
                    }

                    /////
                    if (TempData["Import10thFailsearch"] != null)
                    {
                        Search = Convert.ToString(TempData["Import10thFailsearch"]);
                        ViewBag.SelectedItem = TempData["Import10thFailSelList"];
                        ViewBag.Searchstring = TempData["Import10thFailSearchString"];
                        ViewBag.SelectedSession = TempData["Import10thFailSession"];

                        //---------------Fill Data On page Load-----------------          
                        imp.StoreAllData = objDB.SelAllImport10thFail_NTE(schl, pageIndex, ViewBag.SelectedSession, Search);
                        if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Data Doesn't Exist";
                            ViewBag.TotalCount = 0;
                        }
                        else
                        {
                            ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;
                            // ViewBag.TotalCount1 = Convert.ToInt32(obj.TotalCount.Tables[0].Rows[0]["decount"]);
                            ViewBag.TotalCount1 = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                            int tp = Convert.ToInt32(imp.StoreAllData.Tables[1].Rows[0]["decount"]);
                            int pn = tp / 20;
                            int cal = 20 * pn;
                            int res = Convert.ToInt32(ViewBag.TotalCount1) - cal;
                            if (res >= 1)
                                ViewBag.pn = pn + 1;
                            else
                                ViewBag.pn = pn;


                        }
                    }

                    return View(imp);
                }
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        #endregion  Import10thRegNotExm

        //#region  Import9thPassedTCRef

        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        //public ActionResult Import9thPassedTCRef(int? page)
        //{
        //    try
        //    {
        //        if (Session["SCHL"] == null)
        //        {
        //            return RedirectToAction("Logout", "Login");
        //        }
        //        else
        //        {
        //            int status = objDBR.CheckSchoolAssignForm(1, Session["SCHL"].ToString());
        //            if (status > 0)
        //            {
        //                if (status == 0)
        //                { return RedirectToAction("Index", "Home"); }
        //                else
        //                {
        //                    DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
        //                    if (result1.Tables[2].Rows.Count > 0)
        //                    {
        //                        if (result1.Tables[2].Rows[0]["reclock9th"].ToString() == "0")
        //                        {
        //                            return RedirectToAction("AgreeResult", "RegistrationPortal", new { Form = "M1" });
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                return RedirectToAction("Index", "Home");
        //            }
        //        }

        //        Import obj = new Import();
        //        string session = null;
        //        string schl = null;

        //        if (Session["Session"] != null)
        //        {
        //            session = Session["Session"].ToString();
        //            schl = Session["SCHL"].ToString();
        //            obj.schoolcode = schl;
        //            List<SelectListItem> schllist = new List<SelectListItem>();

        //            string SchoolAssign = "";
        //            SchoolAssign = objDB.GetImpschlOcode(3, schl);
        //            if (SchoolAssign == null || SchoolAssign == "")
        //            { return RedirectToAction("Index", "Login"); }
        //            else
        //            {
        //                if (SchoolAssign.Contains(','))
        //                {
        //                    string[] s = SchoolAssign.Split(',');
        //                    //  int Cs =    s.Count;
        //                    foreach (string schlcode in s)
        //                    {
        //                        schllist.Add(new SelectListItem { Text = schlcode, Value = schlcode });
        //                    }
        //                }
        //                else
        //                {
        //                    schllist.Add(new SelectListItem { Text = schl, Value = schl });
        //                }
        //            }
        //            //schllist.Add(new SelectListItem { Text = "---Select Import TO--", Value = "0" });                
        //            ViewBag.MySchCode = schllist;
        //            string Search = string.Empty;
        //            string SearchString = string.Empty;

        //            if (TempData["ImportDataM1TCREFSearch"] != null)
        //            {
        //                Search = TempData["ImportDataM1TCREFSearch"].ToString();
        //                SearchString = TempData["ImportDataM1TCREFSearchString"].ToString();

        //                //-------------------------------------------------------Page Load Start----------                     

        //                obj.StoreAllData = objDB.SelectImport9thPassedTCRef(Search, SearchString);  //SelectTCREFN3Students //SelectTCStudents9thPassed
        //                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
        //                {
        //                    ViewBag.Message = "Record Not Found";
        //                    ViewBag.TotalCount = 0;
        //                    return View();
        //                }
        //                else
        //                {

        //                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
        //                    obj.chkidList = new List<ImportIDModel>();
        //                    ImportIDModel chk = null;                          
        //                    if (TempData["SelValueSch"].ToString() == "1")
        //                    {
        //                        //  Search = " TCrefno='" + SearchString.ToString().Trim() + "'";
        //                        for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
        //                        {
        //                            chk = new ImportIDModel();
        //                            chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
        //                            chk.Name = "chkidList[" + i + "].id";
        //                            chk.Selected = false;
        //                            obj.chkidList.Add(chk);
        //                        }
        //                    }
        //                    else if (TempData["SelValueSch"].ToString() == "2" || TempData["SelValueSch"].ToString() == "3" || TempData["SelValueSch"].ToString() == "7")
        //                    {
        //                        //   Search += " registration_num ='" + SearchString.ToString().Trim() + "'";
        //                        for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
        //                        {
        //                            chk = new ImportIDModel();
        //                            chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
        //                            chk.Name = "chkidList[" + i + "].id";
        //                            chk.Selected = false;
        //                            obj.chkidList.Add(chk);
        //                        }
        //                    }

        //                    else
        //                    {
        //                        for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
        //                        {
        //                            chk = new ImportIDModel();
        //                            chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
        //                            chk.Name = "chkidList[" + i + "].id";
        //                            chk.Selected = false;
        //                            obj.chkidList.Add(chk);
        //                        }
        //                    }
        //                    return View(obj);

        //                }

        //                //--------------------------------End------------------------
        //            }


        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }


        //        return View(obj);
        //    }
        //    catch (Exception ex)
        //    {

        //        oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
        //        return View();
        //    }
        //}
        //[HttpPost]
        //public ActionResult Import9thPassedTCRef(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string SearchStringschl, string SearchStringfnm)

        //{
        //    try
        //    {

        //        string session = null;
        //        string schl = null;
        //        Import obj = new Import();
        //        if (Session["Session"] != null)
        //        {
        //            session = Session["Session"].ToString();
        //            imp.schoolcode = schl = Session["SCHL"].ToString();
        //            obj.schoolcode = schl;
        //            ViewBag.Searchstring = SearchString;
        //            ViewBag.Searchstringschl = SearchStringschl;
        //            ViewBag.Searchstringfnm = SearchStringfnm;
        //            List<SelectListItem> schllist = new List<SelectListItem>();

        //            string SchoolAssign = "";
        //            SchoolAssign = objDB.GetImpschlOcode(3, schl);
        //            if (SchoolAssign == null || SchoolAssign == "")
        //            { return RedirectToAction("Index", "Login"); }
        //            else
        //            {
        //                if (SchoolAssign.Contains(','))
        //                {
        //                    string[] s = SchoolAssign.Split(',');
        //                    //  int Cs =    s.Count;
        //                    foreach (string schlcode in s)
        //                    {
        //                        schllist.Add(new SelectListItem { Text = schlcode, Value = schlcode });
        //                    }
        //                }
        //                else
        //                {
        //                    schllist.Add(new SelectListItem { Text = schl, Value = schl });
        //                }
        //            }
        //            //schllist.Add(new SelectListItem { Text = "---Select Import TO--", Value = "0" });                
        //            ViewBag.MySchCode = schllist;
        //        }
        //        else
        //        {
        //            return RedirectToAction("Index", "Login");
        //        }

        //        string Search = string.Empty;
        //        if (cmd == "Search")
        //        {

        //            ViewBag.SelectedItem = SelList;
        //            int SelValueSch = Convert.ToInt32(SelList.ToString());
        //            TempData["SelValueSch"] = SelValueSch;
        //            if (SelValueSch == 1)
        //            {
        //                // Search += " Roll ='" + SearchString.ToString().Trim() + "'";
        //                Search = " TCrefno='" + SearchString.ToString().Trim() + "'";
        //            }
        //            else if (SelValueSch == 2)
        //            {
        //                //Search += " registration_num like '%" + SearchString.ToString().Trim() + "%'";
        //                Search += " registration_num ='" + SearchString.ToString().Trim() + "'";
        //            }
        //            else if (SelValueSch == 3)
        //            {
        //                Search += " schl ='" + SearchStringschl.ToString().Trim() + "' and candi_name like '%" + SearchString.ToString().Trim() + "%' and father_name like '%" + SearchStringfnm.ToString().Trim() + "%'";
        //            }
        //            else if (SelValueSch == 7)
        //            {                        
        //                Search += " Aadhar_num ='" + SearchString.ToString().Trim() + "'";
        //            }
        //            TempData["ImportDataM1TCREFSearch"] = Search;
        //            TempData["ImportDataM1TCREFSearchString"] = SearchString.ToString().Trim();

        //            obj.StoreAllData = objDB.SelectImport9thPassedTCRef(Search, SearchString);   //SelectTCStudents9thPassed
        //            if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
        //            {
        //                ViewBag.Message = "Record Not Found";
        //                ViewBag.TotalCount = 0;
        //                return View();
        //            }
        //            else
        //            {

        //                ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
        //                obj.chkidList = new List<ImportIDModel>();
        //                ImportIDModel chk = null;

        //                //int SelValueSch = Convert.ToInt32(SelList.ToString());
        //                if (SelValueSch == 1)
        //                {
        //                    //  Search = " TCrefno='" + SearchString.ToString().Trim() + "'";
        //                    for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
        //                    {
        //                        chk = new ImportIDModel();
        //                        chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
        //                        chk.Name = "chkidList[" + i + "].id";
        //                        chk.Selected = false;
        //                        obj.chkidList.Add(chk);
        //                    }
        //                }
        //                else if (SelValueSch == 2 || SelValueSch == 3 || SelValueSch == 7)
        //                {
        //                    //   Search += " registration_num ='" + SearchString.ToString().Trim() + "'";
        //                    for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
        //                    {
        //                        chk = new ImportIDModel();
        //                        chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
        //                        chk.Name = "chkidList[" + i + "].id";
        //                        chk.Selected = false;
        //                        obj.chkidList.Add(chk);
        //                    }
        //                }
        //                else if (SelValueSch == 3)
        //                {
        //                    //  Search += " schl ='" + SearchStringschl.ToString().Trim() + "' and candi_name like '%" + SearchString.ToString().Trim() + "%'";
        //                    for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
        //                    {
        //                        chk = new ImportIDModel();
        //                        chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
        //                        chk.Name = "chkidList[" + i + "].id";
        //                        chk.Selected = false;
        //                        obj.chkidList.Add(chk);
        //                    }
        //                }
        //                else
        //                {
        //                    for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
        //                    {
        //                        chk = new ImportIDModel();
        //                        chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
        //                        chk.Name = "chkidList[" + i + "].id";
        //                        chk.Selected = false;
        //                        obj.chkidList.Add(chk);
        //                    }
        //                }

        //                return View(obj);

        //            }
        //        }
        //        /////-----------------------Import Begins-------
        //        // string importToSchl = imp.schoolcode;
        //        string importToSchl = Session["SCHL"].ToString();
        //        if (imp.chkidList == null)
        //        { return RedirectToAction("ImportData9thPassedTCRef", "ImportData"); }
        //        int selectedList = imp.chkidList.Where(t => t.Selected).Count();


        //        var selchklist = imp.chkidList.Where(t => t.Selected == true);

        //        var selchklistComma = imp.chkidList.Where(t => t.Selected == true).Select(s => s.id);
        //        var collectId = string.Join(",", selchklistComma);

        //        DataTable dt = new DataTable();
        //        TempData["TotImported"] = selectedList;
        //        string cls = "1";

        //        dt = objDB.Import9thPassedTCRef(importToSchl, collectId, SearchString, cls); //Select_All_9ThPassed_Continue_TC(importToSchl, collectId, session);

        //        string Sub = String.Empty;
        //        if (dt == null || dt.Rows.Count == 0)
        //        {
        //            ViewBag.Message = "Data Doesn't Exist";
        //            ViewBag.TotalCount = 0;
        //            TempData["result"] = -1;
        //            return View();
        //        }
        //        else
        //        {
        //            //-------------- Updated----------
        //            // ViewData["result"] = 1;
        //            TempData["result"] = 1;
        //        }

        //        return View();
        //    }
        //    catch (Exception ex)
        //    {

        //        oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
        //        return View();
        //    }
        //}
        //#endregion  Import9thPassedTCRef

        #region  Import9thReappearTCRef

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Import9thReappearTCRef(int? page)
        {
            try
            {


                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    int status = objDBR.CheckSchoolAssignForm(1, Session["SCHL"].ToString());
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[2].Rows.Count > 0)
                            {
                                if (result1.Tables[2].Rows[0]["reclock9th"].ToString() == "0")
                                {
                                    return RedirectToAction("AgreeResult", "RegistrationPortal", new { Form = "M1" });
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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


                string Search = string.Empty;
                //Search = "schl ='" + schl + "' and CLASS = 1 and (lot>0 or LOT<>NULL) and result in('PASS','COMPARTMENT')  ";
                if (TempData["Import9thReappearTCRefSearch"] != null)
                {

                    Search = TempData["Import9thReappearTCRefSearch"].ToString();
                    ViewBag.Searchstring = TempData["Import9thReappearTCRefSearchString"].ToString();

                    //---------------Fill Data On page Load-----------------          
                    obj.StoreAllData = objDB.SelectTCStudents9thReappear(Search, ViewBag.Searchstring);
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {

                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        obj.chkidList = new List<ImportIDModel>();
                        ImportIDModel chk = null;
                        for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            chk.Selected = false;
                            obj.chkidList.Add(chk);
                        }

                        return View(obj);

                    }
                }


                return View(obj);
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [HttpPost]
        public ActionResult Import9thReappearTCRef(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString)
        {
            try
            {
                string session = null;
                string schl = null;

                Import obj = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    imp.schoolcode = schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;
                if (cmd == "Search")
                {
                    Search = "SCHL!='" + schl + "'  and class='1' and  TCrefno = '" + SearchString + "'  ";
                    // Search = "TCrefno is not null  and ((result = 'PASS' and class='1')) and (Lot>0)  and  TCrefno = '" + SearchString + "'  ";
                    TempData["Import9thReappearTCRefSearch"] = Search;
                    TempData["Import9thReappearTCRefSearchString"] = SearchString;
                    ViewBag.Searchstring = SearchString;
                    obj.StoreAllData = objDB.SelectTCStudents9thReappear(Search, ViewBag.Searchstring);
                    if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;
                        obj.chkidList = new List<ImportIDModel>();
                        ImportIDModel chk = null;
                        for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = obj.StoreAllData.Tables[0].Rows[i]["TCrefno"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            chk.Selected = false;
                            obj.chkidList.Add(chk);
                        }

                        return View(obj);

                    }
                }
                /////-----------------------Import Begins-------
                ////rohit

                if (SearchString != "")
                {
                    string importToSchl = Session["SCHL"].ToString();
                    string collectId = frm["cbImportSelected"];
                    int cnt = collectId.Count(x => x == ',');
                    TempData["TotImported"] = cnt + 1;
                    string CurrentSchl = Session["SCHL"].ToString();
                    DataTable dt = new DataTable();

                    string Sub = String.Empty;
                    //dt = objDB.Select_All_9ThPassed_Continue_TC(importToSchl, CurrentSchl, SearchString, SearchString);
                    dt = objDB.ImportTCStudent9thReappear(importToSchl, CurrentSchl, SearchString, SearchString);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        // return RedirectToAction("ImportData10thClass", "ImportData");
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        ViewData["result"] = -1;
                        return View();
                    }
                    else
                    {
                        TempData["result"] = 1;
                    }
                }
                else
                {
                    ViewData["result"] = "5";
                    return View();
                }

                return View(imp);
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        #endregion  Import9thReappearTCRef

        #region Import9thPassAnySchoolRegNum
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        public ActionResult Import9thPassAnySchoolRegNum(int? page)
        {
            try
            {
                //Import im = new Import();
                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Logout", "Login");
                }
                else
                {
                    //int status = objDBR.CheckSchoolAssignForm(2, Session["SCHL"].ToString());
                    int status = objDBR.CheckSchoolAssignForm(1, Session["SCHL"].ToString()); // changed by ranjan 17oct
                    if (status > 0)
                    {
                        if (status == 0)
                        { return RedirectToAction("Index", "Home"); }
                        else
                        {
                            DataSet result1 = objDBR.schooltypes(Session["SCHL"].ToString());
                            if (result1.Tables[2].Rows.Count > 0)
                            {
                                if (result1.Tables[2].Rows[0]["reclock9th"].ToString() == "0")
                                {
                                    return RedirectToAction("AgreeResult", "RegistrationPortal", new { Form = "M1" });
                                }
                            }
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Registration Number" }, new { ID = "2", Name = "Unique ID" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                //var sessionsrc = new SelectList(new[]{new {ID="2016",Name="2016"},new{ID="2015",Name="2015"},
                //new{ID="2014",Name="2014"},}, "ID", "Name", 1);
                var sessionsrc = new SelectList(new[]{new{ID="2018",Name="2018"},new {ID="2017",Name="2017"},new{ID="2016",Name="2016"},
                }, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;
                Import obj = new Import();
                string session = null;
                string schl = null;

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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

                string Search = string.Empty;

                //Search = "form in('M1','M2') and result in ('pass','COMPARTMENT','reappear') and schl='"+ schl + "' and importflag is null";
                if (pageIndex == 1)
                {

                    TempData["Import9thPasssearch"] = null;
                    TempData["Import9thPassSelList"] = null;
                    TempData["Import9thPassSearchString"] = null;
                    //TempData["Session"] = null;
                    ViewBag.SelectedSession = TempData["Import9thPassSession"];

                }
                if (TempData["Import9thPasssearch"] == null)

                    Search = "CLASS = 1 and (lot>0 or LOT<>NULL) and result ='PASS' and importflag is null";
                else
                {
                    Search = Convert.ToString(TempData["Import9thPasssearch"]);
                    ViewBag.SelectedItem = TempData["Import9thPassSelList"];
                    ViewBag.Searchstring = TempData["Import9thPassSearchString"];
                    ViewBag.SelectedSession = TempData["Import9thPassSession"];
                }
                //---------------Fill Data On page Load-----------------          
                obj.StoreAllData = objDB.SelectAllImport9thPass(Search, session, pageIndex, ViewBag.SelectedSession);
                obj.TotalCount = objDB.SelectAllImport9thPassCount(Search, session, pageIndex, ViewBag.SelectedSession);
                //obj.TotalCount = objDB.GetAll11thPassCount(Search, session, pageIndex);
                //int totCN = obj.StoreAllData.Tables[0].Rows.Count;
                if (obj.StoreAllData == null || obj.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Data Doesn't Exist";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = obj.StoreAllData.Tables[0].Rows.Count;



                    obj.chkidList = new List<ImportIDModel>();

                    ImportIDModel chk = null;
                    for (int i = 0; i < obj.StoreAllData.Tables[0].Rows.Count; i++)
                    {
                        chk = new ImportIDModel();
                        chk.id = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                        chk.Name = "chkidList[" + i + "].id";
                        if (obj.StoreAllData.Tables[0].Rows.Count == 1)
                            ViewBag.impid = obj.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                        chk.Selected = false;
                        obj.chkidList.Add(chk);
                    }
                }
                //---------------End Fill Data On page Load-----------------

                return View(obj);

            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        [HttpPost]
        public ActionResult Import9thPassAnySchoolRegNum(int? page, Import imp, FormCollection frm, string cmd, string SelList, string SearchString, string Session1, string chkImportid)
        {
            try
            {


                var itemsch = new SelectList(new[] { new { ID = "1", Name = "Registration Number" }, new { ID = "2", Name = "Unique ID" }, }, "ID", "Name", 1);
                ViewBag.MySch = itemsch.ToList();
                var sessionsrc = new SelectList(new[] { new { ID = "2019", Name = "2019" }, new { ID = "2017", Name = "2017" }, new { ID = "2016", Name = "2016" }, }, "ID", "Name", 1);
                ViewBag.MySession = sessionsrc.ToList();
                string result = "";
                string session = null;
                string schl = null;
                int pageIndex = 1;
                pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
                ViewBag.pagesize = pageIndex;

                // Import rm = new Import();

                if (Session["Session"] != null)
                {
                    session = Session["Session"].ToString();
                    schl = Session["SCHL"].ToString();
                    // obj.schoolcode = schl;
                    List<SelectListItem> schllist = new List<SelectListItem>();

                    string SchoolAssign = "";
                    SchoolAssign = objDB.GetImpschlOcode(3, schl);
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
                string Search = "";
                string Search1 = "";
                if (cmd == "Search")
                {
                    Search = "CLASS = 1 and result ='PASS' and (Lot>0)";



                    if (Session1 != "")
                    {
                        //Search += " and Year='" + Session1.ToString().Trim() + "'";
                        // imp.chkidList = null;
                        // ModelState.Clear();
                        //ModelState.Remove("ImportIDModel");
                    }

                    //  if (SearchString != "")
                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());

                        if (SelValueSch == 1)
                        {
                            if (Session1 == "2017" || Session1 == "2018")
                            {
                                //Search += " and registration_Num='" + SearchString.ToString().Trim() + "'";
                                Search1 = Search + " and registration_Num='" + SearchString.ToString().Trim() + "'";
                                // Search += " and REGNO='" + SearchString.ToString().Trim() + "'";
                            }
                            else
                            {
                                Search += " and REGNO='" + SearchString.ToString().Trim() + "'";
                            }


                        }
                        else if (SelValueSch == 2)
                        { Search += " and ID =" + SearchString.ToString().Trim(); }
                        //  ModelState.Clear();
                        //ModelState.Remove("ImportIDModel");
                        // imp.chkidList = null;

                    }


                    TempData["Import9thPasssearch"] = Search;
                    TempData["Import9thPassSelList"] = SelList;
                    TempData["Import9thPassSearchString"] = SearchString.ToString().Trim();
                    TempData["Import9thPassSession"] = Session1;
                    ViewBag.Searchstring = SearchString.ToString().Trim();
                    string importToSchl = Session["SCHL"].ToString();
                    imp.StoreAllData = objDB.SelectAllImport9thPassOtherSchool(Search, Search1, session, Session1);
                    int cn = imp.StoreAllData.Tables[0].Rows.Count;     //objDB.SelectAllImport9thPassOtherSchool(Search, session, pageIndex);

                    if (imp.StoreAllData == null || imp.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        return View();
                    }
                    else
                    {
                        ViewBag.TotalCount = imp.StoreAllData.Tables[0].Rows.Count;

                        imp.chkidList = new List<ImportIDModel>();

                        ImportIDModel chk = null;
                        for (int i = 0; i < imp.StoreAllData.Tables[0].Rows.Count; i++)
                        {
                            chk = new ImportIDModel();
                            chk.id = imp.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Name = "chkidList[" + i + "].id";
                            if (imp.StoreAllData.Tables[0].Rows.Count == 1)
                                ViewBag.impid = imp.StoreAllData.Tables[0].Rows[i]["ID"].ToString();
                            chk.Selected = false;
                            imp.chkidList.Add(chk);
                        }

                        return View(imp);
                    }
                }
                else
                {
                    /////-----------------------Import Begins-------
                    //  string importToSchl = imp.schoolcode;
                    string importToSchl = Session["SCHL"].ToString();
                    if (imp.chkidList == null)
                    { return RedirectToAction("Import11thPassAnySchoolRegNum", "ImportData"); }
                    int selectedList = imp.chkidList.Where(t => t.Selected).Count();
                    TempData["TotImported"] = selectedList;

                    var selchklist = imp.chkidList.Where(t => t.Selected == true);

                    var selchklistComma = imp.chkidList.Where(t => t.Selected == true).Select(s => s.id);
                    var collectId = string.Join(",", selchklistComma);

                    DataTable dt = new DataTable();
                    //dt = objDB.Select_All_Pass_Data(importToSchl, collectId);
                    if (chkImportid != "")
                    {
                        //Select_All_9ThPassed_Other_School
                        dt = objDB.Select_All_9ThPassed_Other_School(importToSchl, chkImportid, Session1);
                    }

                    else
                    {
                        dt = objDB.Select_All_9ThPassed_Other_School(importToSchl, collectId, Session1);
                    }

                    if (dt == null || dt.Rows.Count == 0)
                    {
                        // return RedirectToAction("ImportData10thClass", "ImportData");
                        ViewBag.Message = "Data Doesn't Exist";
                        ViewBag.TotalCount = 0;
                        ViewData["result"] = -1;
                        return View();
                    }
                    else
                    {
                        //-------------- Updated----------
                        // ViewData["result"] = 1;
                        TempData["result"] = 1;

                    }
                    return View();

                }
            }
            catch (Exception ex)
            {

                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }
        #endregion Import9thPassAnySchoolRegNum

        #region CancelForm

        [HttpPost]
        public ActionResult CancelForm11Pass(SchoolModels sm, FormCollection fc)
        {
            TempData["Import11thPasssearch"] = null;
            TempData["Import11thPassSelList"] = null;
            TempData["Import11thPassSearchString"] = null;
            TempData["Import11thPassSession"] = null;
            return RedirectToAction("Import11thPass", "ImportData");
        }
        [HttpPost]
        public ActionResult CancelForm12thFail()
        {
            TempData["Import12thFailsearch"] = null;
            TempData["Import12thFailSelList"] = null;
            TempData["Import12thFailSearchString"] = null;
            TempData["Import12thFailSession"] = null;
            return RedirectToAction("Import12thFail", "ImportData");
        }

        [HttpPost]
        public ActionResult CancelTc11thPASS()
        {

            TempData["SearchString"] = null;
            return RedirectToAction("Import11thPassedTCRef", "ImportData");
        }
        [HttpPost]
        public ActionResult CancelRegNUm11thPassAnySchool()
        {

            TempData["SearchString"] = null;
            return RedirectToAction("Import11thPassAnySchoolRegNum", "ImportData");
        }

        [HttpPost]
        public ActionResult CancelForm9thPass(SchoolModels sm, FormCollection fc)
        {
            TempData["Import9thPasssearch"] = null;
            TempData["Import9thPassSelList"] = null;
            TempData["Import9thPassSearchString"] = null;
            TempData["Import9thPassSession"] = null;
            return RedirectToAction("Import9thPass", "ImportData");
        }
        [HttpPost]
        public ActionResult CancelForm10thFail()
        {
            TempData["Import10thFailsearch"] = null;
            TempData["Import10thFailSelList"] = null;
            TempData["Import10thFailSearchString"] = null;
            TempData["Import10thFailSession"] = null;
            return RedirectToAction("Import10thFail", "ImportData");
        }
        [HttpPost]
        public ActionResult CancelTc9thPASS()
        {

            TempData["SearchString"] = null;
            return RedirectToAction("Import9thPassedTCRef", "ImportData");
        }
        [HttpPost]
        public ActionResult CancelRegNUm9thPassAnySchool()
        {
            TempData["SearchString"] = null;
            return RedirectToAction("Import9thPassAnySchoolRegNum", "ImportData");
        }

        #endregion CancelForm


        #endregion  M1 Or 10Th Import Data
    }
}