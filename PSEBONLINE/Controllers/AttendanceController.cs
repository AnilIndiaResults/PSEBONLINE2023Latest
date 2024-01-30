using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSEBONLINE.Models;
using System.Data;
using System.IO;
using System.Web.Services;
using Newtonsoft.Json;
using System.Web.Routing;
using ClosedXML.Excel;
using System.Web.Caching;
using System.Web.UI;
using System.Threading.Tasks;
using PSEBONLINE.Filters;
using CCA.Util;
using System.Configuration;
using PsebPrimaryMiddle.Controllers;
using System.Data.Entity;
using PSEBONLINE.Repository;
using PSEBONLINE.AbstractLayer;

using System.Web.Helpers;
using System.Data.Entity.Core.Mapping;

namespace PSEBONLINE.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly DBContext _context = new DBContext();
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.AttendanceDB objAttendance = new AbstractLayer.AttendanceDB();
        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        // GET: Attendance


        [SessionCheckFilter]
        public ActionResult Index()
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            SchoolModels MS = new SchoolModels();

            DataSet result = objCommon.schooltypes(loginSession.SCHL);
            if (result == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (result.Tables[1].Rows.Count > 0)
            {
                ViewBag.Senior = result.Tables[1].Rows[0]["Senior"].ToString();
                ViewBag.Matric = result.Tables[1].Rows[0]["Matric"].ToString();
                ViewBag.OSenior = result.Tables[1].Rows[0]["OSenior"].ToString();
                ViewBag.OMatric = result.Tables[1].Rows[0]["OMatric"].ToString();
            }

            return View();
        }


        [SessionCheckFilter]
        public ActionResult AttendanceAgree()
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            Session["AttendanceAgree"] = "1";
            @ViewBag.Dpdf = "../../PDF/PracticalAgree.pdf";
            @ViewBag.Showpdf = "../../PDF/PracticalAgree.pdf";
            return View();
        }

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult CheckAttendanceAgree(FormCollection frm)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];

            try
            {
                string s = frm["Agree"].ToString();
                if (Session["AttendanceAgree"] == null)
                {
                    return RedirectToAction("Index", "Attendance");
                }
                else
                {
                    if (s == "Agree")
                    {
                        return RedirectToAction("AttendenceSummary", "Attendance");

                    }
                    else
                    {
                        return RedirectToAction("Index", "Attendance");
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Logout", "Login");
            }
        }

        [SessionCheckFilter]
        public ActionResult AttendenceSummary(List<AttendenceSummaryDetail> obj)
        {

            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string SCHL = loginSession.SCHL.ToString();
                ViewBag.schlCode = SCHL;

                var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" }, new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                ViewBag.MyRP = itemRP.ToList();

                //var itemClass = new SelectList(new[] { new { ID = "12", Name = "SrSec" }, new { ID = "10", Name = "Matric" }, new { ID = "8", Name = "Middle" },}, "ID", "Name", 1);
                var itemClass = new SelectList(new[] { new { ID = "12", Name = "SrSec" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.MyClass = itemClass.ToList();
                ViewBag.SelectedClass = "0";

                var itemAttendanceStatus = new SelectList(new[] { new { ID = "P", Name = "Present" }, new { ID = "A", Name = "Absent" }, new { ID = "UMC", Name = "UMC" }, new { ID = "Cancel", Name = "Cancel" }, }, "ID", "Name", 1);
                ViewBag.AttendanceStatus = itemAttendanceStatus.ToList();

                DataSet Dresult = AbstractLayer.SchoolDB.GetCentreBySchl(SCHL);
                List<SelectListItem> centreList = new List<SelectListItem>();
                //SecList.Add(new SelectListItem { Text = "No Centre", Value = "No Centre" });
                foreach (DataRow dr in Dresult.Tables[0].Rows)
                {
                    centreList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                }
                ViewBag.Ecent = centreList;
                ViewBag.LastDate = "27/04/2023";
                
                DataSet ds = new DataSet();
                ds = new AbstractLayer.SchoolDB().PanelEntryLastDate("StudentAttendance");
                if (ds.Tables[1].Rows.Count > 0)
                {
                    ViewBag.LastDate = ds.Tables[1].Rows[0]["LastDate"].ToString();
                }
            }
            catch
            {

            }
            return View(obj);
        }

        [SessionCheckFilter]
        [HttpPost]
        public ActionResult AttendenceSummary(string cmd, string ExamCent, string SelExamBatch, string SelClass, string SelCategory, string SearchList, string SubCode, List<AttendenceSummaryDetail> obj, FormCollection frm)
        {

            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                string SCHL = loginSession.SCHL.ToString();
                ViewBag.schlCode = SCHL;

                var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" }, new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                ViewBag.MyRP = itemRP.ToList();

                //var itemClass = new SelectList(new[] { new { ID = "12", Name = "SrSec" }, new { ID = "10", Name = "Matric" }, new { ID = "8", Name = "Middle" },}, "ID", "Name", 1);
                var itemClass = new SelectList(new[] { new { ID = "12", Name = "SrSec" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.MyClass = itemClass.ToList();
                ViewBag.SelectedClass = "0";

                var itemAttendanceStatus = new SelectList(new[] { new { ID = "P", Name = "Present" }, new { ID = "A", Name = "Absent" }, new { ID = "UMC", Name = "UMC" }, new { ID = "Cancel", Name = "Cancel" }, }, "ID", "Name", 1);
                ViewBag.AttendanceStatus = itemAttendanceStatus.ToList();

                // get exam cent by school
                DataSet Dresult = AbstractLayer.SchoolDB.GetCentreBySchl(SCHL);
                List<SelectListItem> centreList = new List<SelectListItem>();
                //SecList.Add(new SelectListItem { Text = "No Centre", Value = "No Centre" });
                foreach (DataRow dr in Dresult.Tables[0].Rows)
                {
                    centreList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                }
                ViewBag.Ecent = centreList;

                string centercode = ExamCent;
                string cls = frm["SelClass"].ToString();
                string category = frm["SelRP"].ToString();
                string examDate = frm["examDate"].ToString();
                ViewBag.SelectedClass = cls;
                ViewBag.SelectedRP = category;
                ViewBag.SelectedExamCent = ExamCent;
                ViewBag.ExamDate = examDate;

                ViewBag.RPname = category == "R" ? "Regular" : category == "O" ? "Open" : "Private";

                ViewBag.LastDate = "27/04/2023";
                DataSet ds = new DataSet();
                ds = new AbstractLayer.SchoolDB().PanelEntryLastDate("StudentAttendance");
                if (ds.Tables[1].Rows.Count > 0)
                {
                    ViewBag.LastDate = ds.Tables[1].Rows[0]["LastDate"].ToString();
                }
                List<AttendenceSummaryDetail> subjectAttendancedetail = new List<AttendenceSummaryDetail>();
                //subjectAttendancedetail = _context.attendenceSummaryDetail.Where(s => s.center == centercode && s.cls == cls.ToString() && s.category.ToLower() == category.ToLower()).ToList();
                subjectAttendancedetail = AbstractLayer.AttendanceDB.AttendenceSummaryDetails(centercode, cls, category);
                if (examDate != "")
                {
                    subjectAttendancedetail = subjectAttendancedetail.Where(s => s.examdate == examDate).ToList();
                }

                return View(subjectAttendancedetail);
            }
            catch (Exception ex)
            {

            }
            return View();
        }


        //AttendanceEntry

        [SessionCheckFilter]
        [OutputCache(Duration = 180, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult AttendanceEntry(string id)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            vmAttendanceModel obj = new vmAttendanceModel();
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("AttendanceSummary", "Attendance");
            }

            string SelClass = "";
            string RP = "";
            string centrecode = "";
            string SubCode = "";
            string ExamDate = "";
            if (id != null)
            {
                string[] split = id.Split('-');
                //var parm = Class + "-" + ExamType + "-" + Centre + "-" + SUB;
                ViewBag.SelClass = SelClass = split[0].ToString();
                SelClass = SelClass = split[0].ToString();
                ViewBag.SelRP = RP = split[1].ToString();
                ViewBag.SelCent = centrecode = split[2].ToString();
                SubCode = split[3].ToString();
                ExamDate = split[4].ToString();
                ViewBag.examDate = ExamDate.Replace("_", "/");
                TempData["cid"] = ViewBag.cid = id.ToString();
            }


            TempData["AttendanceViewListSearch"] = null;
            TempData["AttendanceEnterSearch"] = null;
            TempData["ViewAttendanceFinalSubmitSearch"] = null;


            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            SchoolModels MS = new SchoolModels();
            try
            {
                ViewBag.SelectedExamCent = centrecode;
                string SCHL = loginSession.SCHL.ToString();
                ViewBag.schlCode = SCHL;

                var itemRP = new SelectList(new[] { new { ID = "R", Name = "REG" }, new { ID = "O", Name = "OPEN" }, new { ID = "P", Name = "PVT" }, }, "ID", "Name", 1);
                ViewBag.MyRP = itemRP.ToList();

                //Subject Code, Subject Name,Pending
                var itemAction = new SelectList(new[] { new { ID = "1", Name = "Subject Code" }, new { ID = "2", Name = " Subject Name" }, }, "ID", "Name", 1);
                ViewBag.MyAction = itemAction.ToList();
                ViewBag.SelectedAction = "0";


                //var itemClass = new SelectList(new[] { new { ID = "12", Name = "SrSec" }, new { ID = "10", Name = "Matric" }, new { ID = "8", Name = "Middle" },}, "ID", "Name", 1);
                var itemClass = new SelectList(new[] { new { ID = "12", Name = "SrSec" }, new { ID = "10", Name = "Matric" }, }, "ID", "Name", 1);
                ViewBag.MyClass = itemClass.ToList();
                ViewBag.SelectedClass = "0";

                var itemAttendanceStatus = new SelectList(new[] { new { ID = "P", Name = "Present" }, new { ID = "A", Name = "Absent" }, new { ID = "UMC", Name = "UMC" }, new { ID = "Cancel", Name = "Cancel" }, }, "ID", "Name", 1);
                ViewBag.AttendanceStatus = itemAttendanceStatus.ToList();

                DataSet Dresult = AbstractLayer.SchoolDB.GetCentreBySchl(SCHL);
                List<SelectListItem> centreList = new List<SelectListItem>();
                //SecList.Add(new SelectListItem { Text = "No Centre", Value = "No Centre" });
                foreach (DataRow dr in Dresult.Tables[0].Rows)
                {
                    centreList.Add(new SelectListItem { Text = @dr["centE"].ToString(), Value = @dr["CENT"].ToString() });
                }
                ViewBag.Ecent = centreList;


                ViewBag.centrecode = centrecode;
                ViewBag.centernm = centreList.Where(s => s.Value == centrecode).Select(s => s.Text).SingleOrDefault();
                ViewBag.schl = loginSession.SCHL;
                ViewBag.schlnme = loginSession.SCHLNME;

                ViewBag.LastDate = "27/04/2023";
                DataSet ds = new DataSet();
                ds = new AbstractLayer.SchoolDB().PanelEntryLastDate("StudentAttendance");
                if (ds.Tables[1].Rows.Count > 0)
                {
                    ViewBag.LastDate = ds.Tables[1].Rows[0]["LastDate"].ToString();
                }
                string exambatch = "MAR23";
                ViewBag.examBatch = exambatch;

                obj.attendanceMemoDetail = _context.attendanceMemoDetail.AsNoTracking().Where(s => s.cls == SelClass && s.subCode == SubCode && s.centrecode == centrecode && s.rp == RP && s.examBatch == exambatch).FirstOrDefault();

                if (RP.ToLower() == "r")
                {
                    if (SelClass == "4" || SelClass == "12")
                    {
                        obj.subjectAttendanceSeniorRegulars = _context.subjectAttendanceSeniorRegular.AsNoTracking().Where(s => s.rp.ToLower() == RP.ToLower() && s.cent == centrecode && s.cls == SelClass && s.subCode.ToLower() == SubCode.ToLower()).ToList();
                    }
                    else if (SelClass == "2" || SelClass == "10")
                    {
                        obj.subjectAttendanceMatricRegulars = _context.subjectAttendanceMatricRegular.AsNoTracking().Where(s => s.rp.ToLower() == RP.ToLower() && s.cent == centrecode && s.cls == SelClass && s.subCode.ToLower() == SubCode.ToLower()).ToList();
                    }

                }
                else if (RP.ToLower() == "o")
                {
                    obj.subjectAttendanceOpens = _context.subjectAttendanceOpen.AsNoTracking().Where(s => s.rp.ToLower() == RP.ToLower() && s.cent == centrecode && s.cls == SelClass && s.subCode.ToLower() == SubCode.ToLower()).ToList();
                }
                else
                {
                    obj.subjectAttendancePrivates = _context.subjectAttendancePrivate.AsNoTracking().Where(s => s.rp.ToLower() == RP.ToLower() && s.cent == centrecode && s.cls == SelClass && s.subCode.ToLower() == SubCode.ToLower()).ToList();
                }




                if (obj.attendanceMemoDetail != null)
                {
                    ViewBag.memoNumber = obj.attendanceMemoDetail.memoNumber;
                }
                var count = 0;

                if (obj.subjectAttendancePrivates != null)
                {
                    count += obj.subjectAttendancePrivates.Count;
                    obj.attendanceList = (from o in obj.subjectAttendancePrivates
                                          select new SubjectAttendance
                                          {
                                              memonumber = o.memonumber,
                                              attendance = o.attendance,
                                              rp = o.rp,
                                              cent = o.cent,
                                              schl = o.schl,
                                              cls = o.cls,
                                              rollNo = o.rollNo,
                                              subCode = o.subCode,
                                              subName = o.subName,
                                              studentId = o.studentId,
                                              candidateName = o.candidateName,
                                              motherName = o.motherName,
                                              fatherName = o.fatherName,
                                              dob = o.dob,
                                              differentlyAbled = o.differentlyAbled,
                                              attendanceStatus = o.attendanceStatus,
                                          }).ToList();
                }
                if (obj.subjectAttendanceMatricRegulars != null)
                {
                    count += obj.subjectAttendanceMatricRegulars.Count;
                    obj.attendanceList = (from o in obj.subjectAttendanceMatricRegulars
                                          select new SubjectAttendance
                                          {
                                              memonumber = o.memonumber,
                                              attendance = o.attendance,
                                              rp = o.rp,
                                              cent = o.cent,
                                              schl = o.schl,
                                              cls = o.cls,
                                              rollNo = o.rollNo,
                                              subCode = o.subCode,
                                              subName = o.subName,
                                              studentId = o.studentId,
                                              candidateName = o.candidateName,
                                              motherName = o.motherName,
                                              fatherName = o.fatherName,
                                              dob = o.dob,
                                              differentlyAbled = o.differentlyAbled,
                                              attendanceStatus = o.attendanceStatus,
                                          }).ToList();
                }
                if (obj.subjectAttendanceEighthRegulars != null)
                {
                    count += obj.subjectAttendanceEighthRegulars.Count;
                    obj.attendanceList = (from o in obj.subjectAttendanceEighthRegulars
                                          select new SubjectAttendance
                                          {
                                              memonumber = o.memonumber,
                                              attendance = o.attendance,
                                              rp = o.rp,
                                              cent = o.cent,
                                              schl = o.schl,
                                              cls = o.cls,
                                              rollNo = o.rollNo,
                                              subCode = o.subCode,
                                              subName = o.subName,
                                              studentId = o.studentId,
                                              candidateName = o.candidateName,
                                              motherName = o.motherName,
                                              fatherName = o.fatherName,
                                              dob = o.dob,
                                              differentlyAbled = o.differentlyAbled,
                                              attendanceStatus = o.attendanceStatus,
                                          }).ToList();
                }
                if (obj.subjectAttendanceSeniorRegulars != null)
                {
                    count += obj.subjectAttendanceSeniorRegulars.Count;
                    obj.attendanceList = (from o in obj.subjectAttendanceSeniorRegulars
                                          select new SubjectAttendance
                                          {
                                              memonumber = o.memonumber,
                                              attendance = o.attendance,
                                              rp = o.rp,
                                              cent = o.cent,
                                              schl = o.schl,
                                              cls = o.cls,
                                              rollNo = o.rollNo,
                                              subCode = o.subCode,
                                              subName = o.subName,
                                              studentId = o.studentId,
                                              candidateName = o.candidateName,
                                              motherName = o.motherName,
                                              fatherName = o.fatherName,
                                              dob = o.dob,
                                              differentlyAbled = o.differentlyAbled,
                                              attendanceStatus = o.attendanceStatus,
                                          }).ToList();
                }
                if (obj.subjectAttendanceOpens != null)
                {
                    count += obj.subjectAttendanceOpens.Count;
                    obj.attendanceList = (from o in obj.subjectAttendanceOpens
                                          select new SubjectAttendance
                                          {
                                              memonumber = o.memonumber,
                                              attendance = o.attendance,
                                              rp = o.rp,
                                              cent = o.cent,
                                              schl = o.schl,
                                              cls = o.cls,
                                              rollNo = o.rollNo,
                                              subCode = o.subCode,
                                              subName = o.subName,
                                              studentId = o.studentId,
                                              candidateName = o.candidateName,
                                              motherName = o.motherName,
                                              fatherName = o.fatherName,
                                              dob = o.dob,
                                              differentlyAbled = o.differentlyAbled,
                                              attendanceStatus = o.attendanceStatus,
                                          }).ToList();
                }

                ViewBag.TotalCount = count;
                ViewBag.LastDate = "27/04/2023";
                DataSet ds1 = new DataSet();
                ds1 = new AbstractLayer.SchoolDB().PanelEntryLastDate("StudentAttendance");
                if (ds1.Tables[1].Rows.Count > 0)
                {
                    ViewBag.LastDate = ds1.Tables[1].Rows[0]["LastDate"].ToString();
                }
                ViewBag.Total = count;
                ViewBag.Present = 0;
                ViewBag.Absent = 0;
                ViewBag.Cancel = 0;
                ViewBag.UMC = 0;
                ViewBag.Pending = count;
                if (count > 0)
                {
                    // variables
                    ViewBag.RPname = RP == "R" ? "Regular" : RP == "O" ? "Open" : "Private";
                    ViewBag.SubCode = obj.attendanceList.Select(s => s.subCode).Distinct().FirstOrDefault();
                    ViewBag.SubNm = obj.attendanceList.Select(s => s.subName).Distinct().FirstOrDefault();
                    ViewBag.clsName = SelClass == "12" ? "Senior Secondary" : SelClass == "10" ? "Matric" : "Middle";

                    ViewBag.Total = obj.attendanceList.Count;
                    ViewBag.Present = obj.attendanceList.Where(s => s.attendance == "P").ToList().Count;
                    ViewBag.Absent = obj.attendanceList.Where(s => s.attendance == "A").ToList().Count;
                    ViewBag.Cancel = obj.attendanceList.Where(s => s.attendance == "Cancel").ToList().Count;
                    ViewBag.UMC = obj.attendanceList.Where(s => s.attendance == "UMC").ToList().Count;

                    int submmitedCount = (ViewBag.Present + ViewBag.Absent + ViewBag.Cancel + ViewBag.UMC);
                    ViewBag.Pending = obj.attendanceList.Count - submmitedCount;

                }

            }
            catch (Exception ex)
            {
            }

            return View(obj);
        }




        [SessionCheckFilter]
        [HttpPost]
        public JsonResult AttendanceSave(vmAttendanceModel ovmAttendanceModel)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            AttendanceResponse response = new AttendanceResponse();

            if (string.IsNullOrEmpty(ovmAttendanceModel.centrecode))
            {
                response.returncode = "-5";
                response.returnmessage = "centre";
                response.memonumber = "0";
                return Json(response, JsonRequestBehavior.AllowGet);
            }


            string id = ovmAttendanceModel.cid;
            string SelClass = "";
            string RP = "";
            string centrecode = ovmAttendanceModel.centrecode;
            string SubCode = "";
            if (id != null)
            {
                string[] split = id.Split('-');
                //var parm = Class + "-" + ExamType + "-" + Centre + "-" + SUB;
                ViewBag.SelClass = SelClass = split[0].ToString();
                ViewBag.SelRP = RP = split[1].ToString();
                ViewBag.SelCent = centrecode = split[2].ToString();
                SubCode = split[3].ToString();
                TempData["cid"] = ViewBag.cid = id.ToString();
            }

            //if (action == "1")
            //{
            //	action = "subcode";
            //}
            //else
            //{
            //	action = "subname";
            //}
            DataTable StudentAttendance = new DataTable();
            StudentAttendance.Clear();
            StudentAttendance.Columns.Add("studentId");
            StudentAttendance.Columns.Add("attendanceStatus");

            foreach (var item in ovmAttendanceModel.attendanceList)
            {
                if (item.attendanceStatus.ToUpper() != "P")
                {
                    DataRow _st = StudentAttendance.NewRow();
                    _st["studentId"] = item.studentId;
                    _st["attendanceStatus"] = item.attendanceStatus;
                    StudentAttendance.Rows.Add(_st);
                }
            }

            string createdby = loginSession.SCHL.ToString();
            string empuserid = loginSession.SCHL.ToString();

            string OutMemoNumber = "0";
            string OutStatus = "0";

            DataSet dt = AttendanceDB.StudentAttendanceSave(StudentAttendance, ovmAttendanceModel.centrecode, SubCode, loginSession.SCHL.ToString(), SelClass, RP,
                "", "MAR23", ovmAttendanceModel.status, createdby, null, empuserid, out OutMemoNumber, out OutStatus);
            //response.returncode = dt.Tables[0].Rows[0]["returncode"].ToString();
            //response.returnmessage = dt.Tables[0].Rows[0]["returnmessage"].ToString();
            //response.memonumber = dt.Tables[0].Rows[0]["memonumber"].ToString();

            response.returncode = OutStatus;
            response.returnmessage = OutStatus;
            response.memonumber = OutMemoNumber;
            return Json(response, JsonRequestBehavior.AllowGet);

        }





        [SessionCheckFilter]
        [HttpPost]
        public JsonResult AttendanceEntryFinal(vmAttendanceModel ovmAttendanceModel)
        {
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            AttendanceResponse response = new AttendanceResponse();
            if (string.IsNullOrEmpty(ovmAttendanceModel.memmoNumber))
            {
                response.returncode = "-5";
                response.returnmessage = "memo";
                response.memonumber = "0";
                return Json(response, JsonRequestBehavior.AllowGet);
            }


            DataTable StudentAttendance = new DataTable();
            StudentAttendance.Clear();
            StudentAttendance.Columns.Add("studentId");
            StudentAttendance.Columns.Add("attendanceStatus");

            //foreach (var item in ovmAttendanceModel.attendanceList)
            //{
            //	DataRow _st = StudentAttendance.NewRow();
            //	_st["studentId"] = item.studentId;
            //	_st["attendanceStatus"] = item.attendanceStatus;
            //	StudentAttendance.Rows.Add(_st);
            //}
            string finalsubmitBy = loginSession.SCHL.ToString();
            string empuserid = loginSession.SCHL.ToString();


            DataSet dt = AttendanceDB.StudentAttendanceFinalSubmit(StudentAttendance, ovmAttendanceModel.memmoNumber, ovmAttendanceModel.status, finalsubmitBy, empuserid);
            response.returncode = dt.Tables[0].Rows[0]["returncode"].ToString();
            response.returnmessage = dt.Tables[0].Rows[0]["returnmessage"].ToString();
            response.memonumber = dt.Tables[0].Rows[0]["memonumber"].ToString();
            return Json(response, JsonRequestBehavior.AllowGet);
        }


        public List<SubjectAttendanceSeniorRegular> GetsubjectAttendanceSeniorRegular(string sMemoNo)
        {
            var query = "select * from fun_AttendanceRoughandFinalReportSeniorRegular('" + sMemoNo + "')";
            object[] parameters = new object[] { };

            var response = _context.Database.SqlQuery<SubjectAttendanceSeniorRegular>(query, parameters).ToList();

            return response;
        }

        public List<SubjectAttendanceMatricRegular> GetsubjectAttendanceAttendanceMatricRegular(string sMemoNo)
        {
            var query = "select * from fun_AttendanceRoughandFinalReportMatricRegulars('" + sMemoNo + "')";
            object[] parameters = new object[] { };

            var response = _context.Database.SqlQuery<SubjectAttendanceMatricRegular>(query, parameters).ToList();

            return response;
        }
        public List<SubjectAttendanceOpen> GetsubjectAttendanceAttendanceOpen(string sMemoNo)
        {
            var query = "select * from fun_AttendanceRoughandFinalReportOpens('" + sMemoNo + "')";
            object[] parameters = new object[] { };

            var response = _context.Database.SqlQuery<SubjectAttendanceOpen>(query, parameters).ToList();

            return response;
        }

        public List<SubjectAttendancePrivate> GetsubjectAttendanceAttendancePrivate(string sMemoNo)
        {
            var query = "select * from fun_AttendanceRoughandFinalReportPrivate('" + sMemoNo + "')";
            object[] parameters = new object[] { };

            var response = _context.Database.SqlQuery<SubjectAttendancePrivate>(query, parameters).ToList();

            return response;
        }
        //

        [AllowAnonymous]
        [OutputCache(Duration = 180, VaryByParam = "none", Location = OutputCacheLocation.Client, NoStore = true)]
        public ActionResult AttendancePrint(string id)
        {
            ViewBag.Total = 0;
            ViewBag.Present = 0;
            ViewBag.Absent = 0;
            ViewBag.Cancel = 0;
            ViewBag.UMC = 0;
            LoginSession loginSession = (LoginSession)Session["LoginSession"];
            vmAttendanceModel obj = new vmAttendanceModel();
            try
            {
                if (id != null)
                {
                    obj.attendanceMemoDetail = _context.attendanceMemoDetail.AsNoTracking().Where(s => s.memoNumber.ToLower() == id.ToLower()).FirstOrDefault();
                    var RP = obj.attendanceMemoDetail.rp;
                    var SelClass = obj.attendanceMemoDetail.cls;
                    if (RP.ToLower() == "r")
                    {
                        if (SelClass == "4" || SelClass == "12")
                        {

                            obj.subjectAttendanceSeniorRegulars = GetsubjectAttendanceSeniorRegular(id.ToLower());
                            //obj.subjectAttendanceSeniorRegulars = _context.subjectAttendanceSeniorRegular.AsNoTracking().Where(s => s.memonumber.ToLower() == id.ToLower()).ToList();
                            obj.attendanceList = (from o in obj.subjectAttendanceSeniorRegulars
                                                  select new SubjectAttendance
                                                  {
                                                      memonumber = o.memonumber,
                                                      attendance = o.attendance,
                                                      rp = o.rp,
                                                      cent = o.cent,
                                                      schl = o.schl,
                                                      cls = o.cls,
                                                      rollNo = o.rollNo,
                                                      subCode = o.subCode,
                                                      subName = o.subName,
                                                      studentId = o.studentId,
                                                      candidateName = o.candidateName,
                                                      motherName = o.motherName,
                                                      fatherName = o.fatherName,
                                                      dob = o.dob,
                                                      differentlyAbled = o.differentlyAbled,
                                                      attendanceStatus = o.attendanceStatus,
                                                  }).ToList();

                        }
                        else if (SelClass == "2" || SelClass == "10")
                        {
                            obj.subjectAttendanceMatricRegulars = GetsubjectAttendanceAttendanceMatricRegular(id.ToLower());
                            //obj.subjectAttendanceMatricRegulars = _context.subjectAttendanceMatricRegular.AsNoTracking().Where(s => s.memonumber.ToLower() == id.ToLower()).ToList();
                            obj.attendanceList = (from o in obj.subjectAttendanceMatricRegulars
                                                  select new SubjectAttendance
                                                  {
                                                      memonumber = o.memonumber,
                                                      attendance = o.attendance,
                                                      rp = o.rp,
                                                      cent = o.cent,
                                                      schl = o.schl,
                                                      cls = o.cls,
                                                      rollNo = o.rollNo,
                                                      subCode = o.subCode,
                                                      subName = o.subName,
                                                      studentId = o.studentId,
                                                      candidateName = o.candidateName,
                                                      motherName = o.motherName,
                                                      fatherName = o.fatherName,
                                                      dob = o.dob,
                                                      differentlyAbled = o.differentlyAbled,
                                                      attendanceStatus = o.attendanceStatus,
                                                  }).ToList();
                        }
                    }
                    else if (RP.ToLower() == "o")
                    {
                        //fun_AttendanceRoughandFinalReportOpens
                        //GetsubjectAttendanceAttendancePrivate
                        obj.subjectAttendanceOpens = GetsubjectAttendanceAttendanceOpen(id.ToLower());
                        //obj.subjectAttendanceOpens = _context.subjectAttendanceOpen.AsNoTracking().Where(s => s.memonumber.ToLower() == id.ToLower()).ToList();
                        obj.attendanceList = (from o in obj.subjectAttendanceOpens
                                              select new SubjectAttendance
                                              {
                                                  memonumber = o.memonumber,
                                                  attendance = o.attendance,
                                                  rp = o.rp,
                                                  cent = o.cent,
                                                  schl = o.schl,
                                                  cls = o.cls,
                                                  rollNo = o.rollNo,
                                                  subCode = o.subCode,
                                                  subName = o.subName,
                                                  studentId = o.studentId,
                                                  candidateName = o.candidateName,
                                                  motherName = o.motherName,
                                                  fatherName = o.fatherName,
                                                  dob = o.dob,
                                                  differentlyAbled = o.differentlyAbled,
                                                  attendanceStatus = o.attendanceStatus,
                                              }).ToList();
                    }
                    else
                    {
                        //obj.subjectAttendancePrivates = _context.subjectAttendancePrivate.AsNoTracking().Where(s => s.memonumber.ToLower() == id.ToLower()).ToList();
                        obj.subjectAttendancePrivates = GetsubjectAttendanceAttendancePrivate(id.ToLower());
                        obj.attendanceList = (from o in obj.subjectAttendancePrivates
                                              select new SubjectAttendance
                                              {
                                                  memonumber = o.memonumber,
                                                  attendance = o.attendance,
                                                  rp = o.rp,
                                                  cent = o.cent,
                                                  schl = o.schl,
                                                  cls = o.cls,
                                                  rollNo = o.rollNo,
                                                  subCode = o.subCode,
                                                  subName = o.subName,
                                                  studentId = o.studentId,
                                                  candidateName = o.candidateName,
                                                  motherName = o.motherName,
                                                  fatherName = o.fatherName,
                                                  dob = o.dob,
                                                  differentlyAbled = o.differentlyAbled,
                                                  attendanceStatus = o.attendanceStatus,
                                              }).ToList();
                    }
                    obj.attendanceListPresent = obj.attendanceList.Where(s => s.attendance == "P").ToList();
                    obj.attendanceListAbsent = obj.attendanceList.Where(s => s.attendance == "A").ToList();
                    obj.attendanceListCancel = obj.attendanceList.Where(s => s.attendance == "Cancel").ToList();
                    obj.attendanceListUMC = obj.attendanceList.Where(s => s.attendance == "UMC").ToList();

                    List<AttendenceSummaryDetail> subjectAttendancedetail = new List<AttendenceSummaryDetail>();
                    //subjectAttendancedetail = _context.attendenceSummaryDetail.Where(s => s.center == centercode && s.cls == cls.ToString() && s.category.ToLower() == category.ToLower()).ToList();
                    //try
                    //{
                    //	subjectAttendancedetail = AbstractLayer.AttendanceDB.AttendenceSummaryDetails(obj.attendanceList.FirstOrDefault().cent, obj.attendanceMemoDetail.cls, obj.attendanceMemoDetail.rp);
                    //}
                    //catch
                    //{
                    subjectAttendancedetail = AbstractLayer.AttendanceDB.AttendenceSummaryDetails(obj.attendanceMemoDetail.centrecode, obj.attendanceMemoDetail.cls, obj.attendanceMemoDetail.rp);
                    //}
                    ViewBag.Total = subjectAttendancedetail.Where(s => s.memonumber == id).FirstOrDefault().total;
                    ViewBag.Present = obj.attendanceListPresent.Count;
                    ViewBag.Absent = obj.attendanceListAbsent.Count;
                    ViewBag.Cancel = obj.attendanceListCancel.Count;
                    ViewBag.UMC = obj.attendanceListUMC.Count;


                    int submmitedCount = (ViewBag.Absent + ViewBag.Cancel + ViewBag.UMC);
                    ViewBag.Pending = "0";
                    ViewBag.Present = (ViewBag.Total - submmitedCount);

                }
                return View(obj);
            }
            catch (Exception ex)
            {
                return View(obj);
            }

        }




        [AdminLoginCheckFilter]
        public ActionResult AdminAttendanceReport(vmAttendanceModel obj)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            return View(obj);
        }

        [AdminLoginCheckFilter]
        [HttpPost]
        public ActionResult AdminAttendanceReport(string cmd, string SelExamBatch, string SelClass, string SelCategory, string SearchList, string SubCode, vmAttendanceModel obj, FormCollection frm)
        {
            AdminLoginSession adminLoginSession = (AdminLoginSession)Session["AdminLoginSession"];
            return View(obj);
        }

    }
}