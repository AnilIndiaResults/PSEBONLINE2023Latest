using PSEBONLINE.AbstractLayer;
using PSEBONLINE.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PSEBONLINE.Controllers
{
    public class AssociateCommuniController : Controller

    {
        private readonly DBContext _context = new DBContext();
        public AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        public AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        public AbstractLayer.AffiliationDB affiliationDB = new AbstractLayer.AffiliationDB();
        public AbstractLayer.SchoolDB ObjSchoolDB = new AbstractLayer.SchoolDB();
        // GET: AssociateCommuni
        public ActionResult Index()
        {
            if (Session["SCHL"].ToString()==null)
            {
                return RedirectToAction("Index", "Login");
            }

            return RedirectToAction("schoolProfile");
        }

        [HttpGet]
        public ActionResult RoomDetails(string id)
        {
            AssociateModel ViewModel = new AssociateModel();
            ViewModel.RoomDetailsModelList = new List<RoomDetailsModel>();
          
            ViewModel.RoomDetailsModel = new RoomDetailsModel();
           

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            var roomtype = new SelectList(new[]{
        new { ID="Class Room", Name="Class Room" },
        new { ID="Store", Name="Store" },
        new { ID="Principal Room", Name="Principal Room" },
        new { ID="4", Name="Science Lab" },
        new { ID="Science Lab", Name="Computer Lab" },
        new { ID="Other Room", Name="Other Room" },
    }, "ID", "Name", 1);
            ViewBag.roomtype = roomtype.ToList();
            ViewBag.Selectedroomtype = "0";

            var floorName = new SelectList(new[]{
        new { ID="Ground Floor", Name="Ground Floor" },
        new { ID="1st Floor", Name="1st Floor" },
        new { ID="2nd Floor", Name="2nd Floor" },
        new { ID="3rd Floor", Name="3rd Floor" },
        new { ID="4th Floor", Name="4th Floor" },
        new { ID="5th Floor", Name="5th Floor" },
        new { ID="6th Floor", Name="6th Floor" },
        new { ID="7th Floor", Name="7th Floor" },
    }, "ID", "Name", 1);
            ViewBag.floorName = floorName.ToList();
            ViewBag.SelectedfloorName = "0";

            ViewModel.RoomDetailsModelList = AssociateDB.GetAssociateRoomDetails();
           

            return View(ViewModel);
        }


        [HttpPost]
        public ActionResult RoomDetails(string id,AssociateModel roomModel)
        {
            //AssociateModel ViewModel = new AssociateModel();
            //ViewModel.RoomDetailsModelList = new List<RoomDetailsModel>();
            //ViewModel.RoomDetailsModel = new RoomDetailsModel();
            string outError = "0";
            string rslt = AssociateDB.SaveRoomDetails(roomModel);


            if (rslt == "OK")
            {
                ViewData["result"] = "1";
                ViewBag.Mesaage = outError;
            }
            else
            {
                ViewData["result"] = outError.ToString();
                ViewBag.Mesaage = outError;
            }
            return RoomDetails(id);


            //if (rslt == "OK")
            //{
               
            //    roomModel = new AssociateModel();
            //    return RoomDetails(id);
            //}
            //else
            //{
            //    AssociateModel NewroomModel = new AssociateModel();
            //    return View();
            //}
        }

        public int calculateArea(AssociateModel roomModel)
        {
            roomModel.RoomDetailsModel.Area = (roomModel.RoomDetailsModel.Height * roomModel.RoomDetailsModel.width) * roomModel.RoomDetailsModel.Quantity;
            return roomModel.RoomDetailsModel.Area;
        }

        #region SchoolProfile

        [HttpGet]
        public ActionResult schoolProfile(string id, SchoolModels sm)
        {
            try
            {

                if (Session["SCHL"] == null)
                {
                    return RedirectToAction("Index", "Login");
                }

                ViewBag.SCHL = Session["SCHL"].ToString();
                DataSet outDs = new DataSet();
                AssociateModel am1 = AssociateDB.AssociateContinuationBySchlTemp(Session["SCHL"].ToString(), 1, out outDs);//ResultStatics
                if (am1.ID > 0)
                {
                    ViewBag.AID = am1.ID;
                    ViewBag.ChallanId = am1.ChallanId;
                    ViewBag.challanVerify = am1.challanVerify;
                    ViewBag.RS10GPass2017 = am1.RS10GPass2017;
                    ViewBag.SF10Percent = am1.SF10Percent;
                    ViewBag.BSFROM = am1.BSFROM;
                    ViewBag.FSFROM = am1.FSFROM;
                    ViewBag.BPNAME = am1.BPNAME;
                    ViewBag.ASZONE = am1.ASZONE;
                    ViewBag.ASZONE = am1.ASZONE;
                    ViewBag.OI1 = am1.OI1;
                }
                else
                {
                    ViewBag.AID = 0;
                    ViewBag.ChallanId = 0;
                    ViewBag.challanVerify = 0;
                }

                DataSet ds = ObjSchoolDB.SelectSchoolDatabyID(Session["SCHL"].ToString());
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    ViewData["resultSVF"] = 2;
                }
                else
                {
                    sm.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
                    sm.SCHL = ds.Tables[0].Rows[0]["schl"].ToString();
                    sm.idno = ds.Tables[0].Rows[0]["idno"].ToString();
                    sm.CLASS = ds.Tables[0].Rows[0]["CLASS"].ToString();
                    sm.OCODE = ds.Tables[0].Rows[0]["OCODE"].ToString();
                    sm.USERTYPE = ds.Tables[0].Rows[0]["USERTYPE"].ToString();
                    sm.session = ds.Tables[0].Rows[0]["SESSION"].ToString();
                    sm.status = ds.Tables[0].Rows[0]["status"].ToString();
                    sm.Approved = ds.Tables[0].Rows[0]["IsApproved"].ToString() == "Y" ? "YES" : "NO";
                    sm.vflag = ds.Tables[0].Rows[0]["IsVerified"].ToString() == "Y" ? "YES" : "NO";
                    sm.AREA = ds.Tables[0].Rows[0]["SchoolArea"].ToString();
                    sm.VALIDITY = ds.Tables[0].Rows[0]["VALIDITY"].ToString();
                    sm.PASSWORD = ds.Tables[0].Rows[0]["PASSWORD"].ToString();
                    sm.ACTIVE = ds.Tables[0].Rows[0]["ACTIVE"].ToString();
                    sm.ADDRESSE = ds.Tables[0].Rows[0]["ADDRESSE"].ToString();
                    sm.ADDRESSP = ds.Tables[0].Rows[0]["ADDRESSP"].ToString();
                    sm.AGRI = ds.Tables[0].Rows[0]["AGRI"].ToString();
                    sm.SCHLE = ds.Tables[0].Rows[0]["SCHLE"].ToString();
                    sm.dist = ds.Tables[0].Rows[0]["DIST"].ToString();
                    sm.DISTE = ds.Tables[0].Rows[0]["DISTE"].ToString();
                    sm.STATIONE = ds.Tables[0].Rows[0]["STATIONE"].ToString();
                    sm.SCHLP = ds.Tables[0].Rows[0]["SCHLP"].ToString();
                    sm.DISTP = ds.Tables[0].Rows[0]["DISTP"].ToString();
                    sm.STATIONP = ds.Tables[0].Rows[0]["STATIONP"].ToString();
                    sm.DISTNM = ds.Tables[0].Rows[0]["DISTNM"].ToString();
                    sm.DISTNMPun = ds.Tables[0].Rows[0]["DISTNMPun"].ToString();


                    sm.DOB = ds.Tables[0].Rows[0]["DOB"].ToString();
                    sm.DOJ = ds.Tables[0].Rows[0]["DOJ"].ToString();
                    sm.ExperienceYr = ds.Tables[0].Rows[0]["ExperienceYr"].ToString();
                    sm.PQualification = ds.Tables[0].Rows[0]["PQualification"].ToString();

                    sm.PRINCIPAL = ds.Tables[0].Rows[0]["PRINCIPAL"].ToString();
                    sm.STDCODE = ds.Tables[0].Rows[0]["STDCODE"].ToString();
                    sm.PHONE = ds.Tables[0].Rows[0]["PHONE"].ToString();
                    sm.MOBILE = ds.Tables[0].Rows[0]["MOBILE"].ToString();
                    sm.mobile2 = ds.Tables[0].Rows[0]["mobile2"].ToString();
                    sm.ADDRESSE = ds.Tables[0].Rows[0]["ADDRESSE"].ToString();
                    sm.ADDRESSP = ds.Tables[0].Rows[0]["ADDRESSP"].ToString();
                    sm.CONTACTPER = ds.Tables[0].Rows[0]["CONTACTPER"].ToString();
                    sm.CPPHONE = ds.Tables[0].Rows[0]["CONTACTPER"].ToString();
                    sm.CPSTD = ds.Tables[0].Rows[0]["CPSTD"].ToString();
                    sm.OtContactno = ds.Tables[0].Rows[0]["OtContactno"].ToString();
                    sm.EMAILID = ds.Tables[0].Rows[0]["EMAILID"].ToString();
                    sm.NSQF_flag = ds.Tables[0].Rows[0]["NSQF_flag"].ToString() == "Y" ? "YES" : "NO";

                    sm.REMARKS = ds.Tables[0].Rows[0]["REMARKS"].ToString();
                    sm.UDATE = ds.Tables[0].Rows[0]["UDATE"].ToString();
                    sm.correctionno = ds.Tables[0].Rows[0]["correctionno"].ToString();


                    sm.MSET = ViewBag.MSET = ds.Tables[0].Rows[0]["MSET"].ToString();
                    sm.MOSET = ViewBag.MOSET = ds.Tables[0].Rows[0]["MOSET"].ToString();
                    sm.SSET = ViewBag.SSET = ds.Tables[0].Rows[0]["SSET"].ToString();
                    sm.SOSET = ViewBag.SOSET = ds.Tables[0].Rows[0]["SOSET"].ToString();

                    //Regular

                    sm.middle = ds.Tables[0].Rows[0]["middle"].ToString() == "Y" ? "YES" : "NO";
                    sm.MATRIC = ds.Tables[0].Rows[0]["MATRIC"].ToString() == "Y" ? "YES" : "NO";
                    sm.HUM = ds.Tables[0].Rows[0]["HUM"].ToString() == "Y" ? "YES" : "NO";
                    sm.SCI = ds.Tables[0].Rows[0]["SCI"].ToString() == "Y" ? "YES" : "NO";
                    sm.COMM = ds.Tables[0].Rows[0]["COMM"].ToString() == "Y" ? "YES" : "NO";
                    sm.VOC = ds.Tables[0].Rows[0]["VOC"].ToString() == "Y" ? "YES" : "NO";
                    sm.TECH = ds.Tables[0].Rows[0]["TECH"].ToString() == "Y" ? "YES" : "NO";
                    sm.AGRI = ds.Tables[0].Rows[0]["AGRI"].ToString() == "Y" ? "YES" : "NO";

                    //OPen
                    sm.omiddle = ds.Tables[0].Rows[0]["omiddle"].ToString() == "Y" ? "YES" : "NO";
                    sm.OMATRIC = ds.Tables[0].Rows[0]["OMATRIC"].ToString() == "Y" ? "YES" : "NO";
                    sm.OHUM = ds.Tables[0].Rows[0]["OHUM"].ToString() == "Y" ? "YES" : "NO";
                    sm.OSCI = ds.Tables[0].Rows[0]["OSCI"].ToString() == "Y" ? "YES" : "NO";
                    sm.OCOMM = ds.Tables[0].Rows[0]["OCOMM"].ToString() == "Y" ? "YES" : "NO";
                    sm.OVOC = ds.Tables[0].Rows[0]["OVOC"].ToString() == "Y" ? "YES" : "NO";
                    sm.OTECH = ds.Tables[0].Rows[0]["OTECH"].ToString() == "Y" ? "YES" : "NO";
                    sm.OAGRI = ds.Tables[0].Rows[0]["OAGRI"].ToString() == "Y" ? "YES" : "NO";


                    //New

                    sm.MID_CR = ViewBag.MID_CR = ds.Tables[0].Rows[0]["MID_CR"].ToString();
                    sm.MID_NO = ViewBag.MID_NO = ds.Tables[0].Rows[0]["MID_NO"].ToString();
                    sm.MID_YR = ViewBag.MID_YR = ds.Tables[0].Rows[0]["MID_YR"].ToString();
                    sm.MID_S = ViewBag.MID_S = Convert.ToInt32(ds.Tables[0].Rows[0]["MID_S"].ToString());
                    sm.MID_DNO = ViewBag.MID_DNO = ds.Tables[0].Rows[0]["MID_DNO"].ToString();

                    sm.HID_CR = ViewBag.HID_CR = ds.Tables[0].Rows[0]["HID_CR"].ToString();
                    sm.HID_NO = ViewBag.HID_NO = ds.Tables[0].Rows[0]["HID_NO"].ToString();
                    sm.HID_YR = ViewBag.HID_YR = ds.Tables[0].Rows[0]["HID_YR"].ToString();
                    sm.HID_S = ViewBag.HID_S = Convert.ToInt32(ds.Tables[0].Rows[0]["HID_S"].ToString());
                    sm.HID_DNO = ViewBag.HID_DNO = ds.Tables[0].Rows[0]["HID_DNO"].ToString();

                    sm.SID_CR = ViewBag.SID_CR = ds.Tables[0].Rows[0]["SID_CR"].ToString();
                    sm.SID_NO = ViewBag.SID_NO = ds.Tables[0].Rows[0]["SID_NO"].ToString();
                    sm.SID_DNO = ViewBag.SID_DNO = ds.Tables[0].Rows[0]["SID_DNO"].ToString();
                    sm.H = ViewBag.H = ds.Tables[0].Rows[0]["H"].ToString();
                    sm.HYR = ViewBag.HYR = ds.Tables[0].Rows[0]["HYR"].ToString();

                    sm.H_S = ViewBag.H_S = Convert.ToInt32(ds.Tables[0].Rows[0]["H_S"].ToString());
                    sm.C = ViewBag.C = ds.Tables[0].Rows[0]["C"].ToString();
                    sm.CYR = ViewBag.CYR = ds.Tables[0].Rows[0]["CYR"].ToString();
                    sm.C_S = ViewBag.C_S = Convert.ToInt32(ds.Tables[0].Rows[0]["C_S"].ToString());
                    sm.S = ViewBag.S = ds.Tables[0].Rows[0]["S"].ToString();
                    sm.SYR = ViewBag.SYR = ds.Tables[0].Rows[0]["SYR"].ToString();
                    sm.S_S = ViewBag.S_S = Convert.ToInt32(ds.Tables[0].Rows[0]["S_S"].ToString());

                    sm.A = ViewBag.A = ds.Tables[0].Rows[0]["A"].ToString();
                    sm.AYR = ViewBag.AYR = ds.Tables[0].Rows[0]["AYR"].ToString();
                    sm.A_S = ViewBag.A_S = Convert.ToInt32(ds.Tables[0].Rows[0]["A_S"].ToString());

                    sm.V = ViewBag.V = ds.Tables[0].Rows[0]["V"].ToString();
                    sm.VYR = ViewBag.VYR = ds.Tables[0].Rows[0]["VYR"].ToString();
                    sm.V_S = ViewBag.V_S = Convert.ToInt32(ds.Tables[0].Rows[0]["V_S"].ToString());

                    sm.T = ViewBag.T = ds.Tables[0].Rows[0]["T"].ToString();
                    sm.TYR = ViewBag.TYR = ds.Tables[0].Rows[0]["TYR"].ToString();
                    sm.T_S = ViewBag.T_S = Convert.ToInt32(ds.Tables[0].Rows[0]["T_S"].ToString());


                    sm.MID_UTYPE = ViewBag.MID_UTYPE = ds.Tables[0].Rows[0]["MID_UTYPEFull"].ToString();
                    sm.HID_UTYPE = ViewBag.HID_UTYPE = ds.Tables[0].Rows[0]["HID_UTYPEFull"].ToString();
                    sm.H_UTYPE = ViewBag.H_UTYPE = ds.Tables[0].Rows[0]["H_UTYPEFull"].ToString();
                    sm.S_UTYPE = ViewBag.S_UTYPE = ds.Tables[0].Rows[0]["S_UTYPEFull"].ToString();
                    sm.C_UTYPE = ViewBag.C_UTYPE = ds.Tables[0].Rows[0]["C_UTYPEFull"].ToString();
                    sm.V_UTYPE = ViewBag.V_UTYPE = ds.Tables[0].Rows[0]["V_UTYPEFull"].ToString();
                    sm.A_UTYPE = ViewBag.A_UTYPE = ds.Tables[0].Rows[0]["A_UTYPEFull"].ToString();
                    sm.T_UTYPE = ViewBag.T_UTYPE = ds.Tables[0].Rows[0]["T_UTYPEFull"].ToString();

                    sm.Tcode = ViewBag.Tcode = ds.Tables[0].Rows[0]["Tcode"].ToString();
                    sm.Tehsile = ViewBag.omiddle = ds.Tables[0].Rows[0]["Tehsile"].ToString();
                    sm.Tehsilp = ViewBag.omiddle = ds.Tables[0].Rows[0]["Tehsilp"].ToString();



                    Session["TotalTeachers"] = null;
                    if (ds.Tables[4].Rows.Count > 0)
                    {
                        ViewBag.TotalTeachers = ds.Tables[4].Rows.Count;
                        Session["TotalTeachers"] = ds.Tables[4];
                    }
                    else
                    {
                        ViewBag.TotalTeachers = 0;
                    }
                }
                return View(sm);
            }
            catch (Exception ex)
            {

                return View();
            }
        }

        public ActionResult ViewTeacherList(string id, SchoolModels sm)
        {
            if (Session["SCHL"] != null && id == Session["SCHL"].ToString())
            {
                ViewBag.SCHL = Session["SCHL"].ToString();
                sm.StoreAllData = ObjSchoolDB.SelectSchoolDatabyID(id);
                if (sm.StoreAllData == null || sm.StoreAllData.Tables[4].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = sm.StoreAllData.Tables[4].Rows.Count;
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            return View(sm);
        }

        [HttpPost]
        public ActionResult schoolProfile(SchoolModels sm)
        {
            return schoolProfile(Session["SCHL"].ToString(),sm);
        }
        #endregion

        #region BuildingSafty
        [HttpGet]
        public ActionResult BuildingSafty(string id, AssociateModel am)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AssociateCommuni");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            if (id != ViewBag.SCHL)
            {
                return RedirectToAction("Index", "AssociateCommuni");
            }


            // YesNo 
            //   ViewBag.YearList = objCommon.GetSessionYear();
            List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession = objCommon.GetSessionYear();
            itemSession.Add(new SelectListItem { Text = "2019", Value = "2019" });
            itemSession.Add(new SelectListItem { Text = "2020", Value = "2020" });
            ViewBag.YearList = itemSession.Where(s => Convert.ToInt32(s.Value) >= 2016).OrderByDescending(s => s.Value);


            ViewBag.SelectedYear = "0";
            var itemsch = new SelectList(new[]{new {ID="1",Name="Punjab Public Works Department, B&R"},new {ID="2",Name="Department of Rural development and Panchayat"},
            }, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "0";

            DataSet outDs = new DataSet();
            am = AssociateDB.AssociateContinuationBySchlTemp(id, 1, out outDs);//ResultStatics
            if (am.ID > 0)
            {
                if (am.BSFILE == "" || am.BSFILE == null)
                { @ViewBag.BSFILE = "../Images/NoPhoto.jpg"; }
                else
                {
                    @ViewBag.BSFILE = "../../Upload/Affiliation/SafetyDetails" + am.BSFILE.ToString();
                }

                if (am.FSFILE == "" || am.FSFILE == null)
                { @ViewBag.FSFILE = "../Images/NoPhoto.jpg"; }
                else
                {
                    @ViewBag.FSFILE = "../../Upload/Affiliation/SafetyDetails" + am.FSFILE.ToString();
                }

                if (string.IsNullOrEmpty(am.FSIDATE) || am.FSIDATE == "01/01/1900")
                {
                    am.FSIDATE = DateTime.Now.ToString("dd/MM/yyyy");
                }

                if (string.IsNullOrEmpty(am.BSIDATE) || am.BSIDATE == "01/01/1900")
                {
                    am.BSIDATE = DateTime.Now.ToString("dd/MM/yyyy");
                }

                return View(am);
            }
            return View(am);
        }

        [HttpPost]
        public ActionResult BuildingSafty(string id, AssociateModel am, FormCollection fc, HttpPostedFileBase bsfile, HttpPostedFileBase fsfile)
        {
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Home"); }
            //
            if (id == null)
            {
                return RedirectToAction("Index", "AssociateCommuni");
            }
            ViewBag.SCHL = Session["SCHL"].ToString();
            // YesNo 
            List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession = objCommon.GetSessionYear();
            itemSession.Add(new SelectListItem { Text = "2019", Value = "2019" });
            itemSession.Add(new SelectListItem { Text = "2020", Value = "2020" });
            ViewBag.YearList = itemSession.Where(s => Convert.ToInt32(s.Value) >= 2016).OrderByDescending(s => s.Value);

            ViewBag.SelectedYear = "0";
            var itemsch = new SelectList(new[]{new {ID="1",Name="Punjab Public Works Department, B&R"},new {ID="2",Name="Department of Rural development and Panchayat"},
            }, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            ViewBag.SelectedItem = "0";

            am.SCHL = id;
            string filename = "";
            if (bsfile != null)
            {
                string ext = Path.GetExtension(bsfile.FileName);
                filename = am.SCHL + "_BSFILE" + ext;
                var path = Path.Combine(Server.MapPath("~/Upload/Affiliation/SafetyDetails"), filename);
                string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Affiliation/SafetyDetails"));
                if (!Directory.Exists(FilepathExist))
                {
                    Directory.CreateDirectory(FilepathExist);
                }
                bsfile.SaveAs(path);
                am.BSFILE = "Affiliation/SafetyDetails/" + filename;
            }

            string filename1 = "";
            if (fsfile != null)
            {
                string ext = Path.GetExtension(bsfile.FileName);
                filename1 = am.SCHL + "_FSFILE" + ext;
                var path = Path.Combine(Server.MapPath("~/Upload/Affiliation/SafetyDetails"), filename1);
                string FilepathExist = Path.Combine(Server.MapPath("~/Upload/Affiliation/SafetyDetails"));
                if (!Directory.Exists(FilepathExist))
                {
                    Directory.CreateDirectory(FilepathExist);
                }
                fsfile.SaveAs(path);
                am.FSFILE = "Affiliation/SafetyDetails/" + filename1;
            }

            DataSet outDs = new DataSet();
            AssociateModel amMain = AssociateDB.AssociateContinuationBySchlTemp(id, 1, out outDs);//ResultStatics
            if (amMain.ID > 0)
            {
                amMain.BSFROM = am.BSFROM;
                amMain.BSTO = am.BSTO;
                amMain.BSIA = am.BSIA;
                amMain.BSIDATE = am.BSIDATE;
                amMain.BSMEMO = am.BSMEMO;
                amMain.BSFILE = am.BSFILE;
                //
                amMain.FSFROM = am.FSFROM;
                amMain.FSTO = am.FSTO;
                amMain.FSIA = am.FSIA;
                amMain.FSIDATE = am.FSIDATE;
                amMain.FSMEMO = am.FSMEMO;
                amMain.FSFILE = am.FSFILE;
                am = amMain;
            }
            //

            string outError = "0";
            int result = AssociateDB.AssociateContinuation(am, out outError);// if ID=0 then Insert else Update
            if (result > 0)
            {
                ViewBag.AID = am.ID;
                ViewBag.result = result;
                ViewData["result"] = 1;
            }
            else
            {
                ViewBag.AID = am.ID;
                ViewData["result"] = 0;
                ViewBag.Message = outError.ToString();
            }
            return View(am);
        }

        #endregion
        //public ActionResult BuildingSafty()
        //{
        //    ViewBag.Years = GetYears();
        //    return View();
        //}


        //private List<int> GetYears()
        //{
        //    var currentYear = DateTime.Now.Year;
        //    var years = new List<int>();

        //    for (int year = currentYear; year >= currentYear - 10; year--)
        //    {
        //        years.Add(year);
        //    }

        //    return years;
        //}


        //public ActionResult AddBuildingSafty(AssociateContinuationBuildingSafty AssSaftyModel)
        //{

        //    if (Session["SCHL"].ToString() == null)
        //    {
        //        return RedirectToAction("Index", "Login");
        //    }



        //    string schl = Session["SCHL"].ToString();
        //    ViewBag.SCHL = schl;

        //    DataSet outDs = new DataSet();
        //    String result = AssociateDB.AssociateContinuationBySafty(AssSaftyModel);//ResultStatics
        //    if (AssSaftyModel.Id>0)
        //    {

        //        ViewBag.Totalcount = 1;
        //    }
        //    else
        //    {
        //        ViewBag.Totalcount = 0;

        //    }

        //    return BuildingSafty();
        //}

        [HttpPost]
        public ActionResult studentCount(string cmd,StudentCountModel studentCountModel)
        {
            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }



            string outError = "0";
            int result = 0;


            if (cmd.ToLower() == "submit" || cmd.ToLower() == "save")
            {
                studentCountModel.SCHL = Session["SCHL"].ToString();
                result = AssociateDB.InsertAssociationStudentCount(studentCountModel, 2, out outError);  // 0 for insert

                if (result > 0)
                {
                    ViewData["result"] = "1";
                    ViewBag.Mesaage = outError;
                }
                else
                {
                    ViewData["result"] = outError.ToString();
                    ViewBag.Mesaage = outError;
                }
            }

            return studentCount(studentCountModel);
        
        }


        [HttpGet]
        public ActionResult studentCount(StudentCountModel studentModel)
        {

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
           

            //ViewModel.StudentCountModelList = AssociateDB.GetAssociateStudentCount(Session["SCHL"].ToString());


            return View(studentModel);
        }

        [HttpGet]
        public ActionResult UploadDocument(string id,AssociationDocumentDetailsModel adm)

            
        {

            AssociateModel ViewModel = new AssociateModel();
            ViewModel.AssociationDocumentDetailsModelList =   new List<AssociationDocumentDetailsModel>();
            ViewModel.AssociationDocumentDetailsModel = new AssociationDocumentDetailsModel();
             

            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
          
            try
            {
                id = id==null? Session["SCHL"].ToString():id;
            }
            catch
            {
                adm.SCHL = Session["SCHL"].ToString();
            }

            adm.SCHL = id;
            DataSet outDs = new DataSet();
            AssociateModel am1 = AssociateDB.AssociateContinuationBySchlTemp(Session["SCHL"].ToString(), 1, out outDs);//ResultStatics
            if (am1.ID > 0 || am1.ID == 0)
            {
                ViewBag.AID = am1.ID;
                ViewBag.IsSchlSubmiited = 1;
                ViewBag.ChallanId = am1.ChallanId;
                ViewBag.challanVerify = am1.challanVerify;
                ViewBag.RS10GPass2017 = am1.RS10GPass2017;
                ViewBag.SF10Percent = am1.SF10Percent;
                ViewBag.BSFROM = am1.BSFROM;
                ViewBag.FSFROM = am1.FSFROM;
                ViewBag.BPNAME = am1.BPNAME;
                ViewBag.ASZONE = am1.ASZONE;
                ViewBag.ASZONE = am1.ASZONE;
                ViewBag.OI1 = am1.OI1;


                adm.StoreAllData = AssociateDB.GetAssociationDocumentDetails(2, 0, id, "");
                adm.AssociationDocumentMasterList = new List< AssociationDocumentMaster>();

                adm.AssociationDocumentMasterList = AssociateDB.AssociationDocumentMasterList(adm.StoreAllData.Tables[1]);//  Document List
                if (adm.StoreAllData == null || adm.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                }
                else
                {
                    ViewBag.TotalCount = adm.StoreAllData.Tables[0].Rows.Count;
                }
            }
            else
            {
                ViewBag.AID = 0;
                ViewBag.ChallanId = 0;
            }
            return View(adm);
        }


        [HttpPost]
        public ActionResult UploadDocument(string id, AssociationDocumentDetailsModel adm, string cmd, FormCollection frm, HttpPostedFileBase docfile)
        {

            

            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "AssociationCommuni");
            }
            adm.StoreAllData = AssociateDB.GetAssociationDocumentDetails(2, 0, id, "");
            adm.AssociationDocumentMasterList = AssociateDB.AssociationDocumentMasterList(adm.StoreAllData.Tables[1]);//  Document List
            adm.SCHL = id;

            string DocName = adm.AssociationDocumentMasterList.Where(s => s.DocID == adm.DocID).Select(s => s.DocumentName).FirstOrDefault();

            if (!string.IsNullOrEmpty(cmd))
            {
                string outError = "0";
                int result = 0;

                // Save file
                string filename = "";
                string FilepathExist = "", path = "";
                string exactPath = "~/Upload/Upload2023/AssociateDocuments";
                if (docfile != null)
                {
                    //Upload/Affiliation
                    string ext = Path.GetExtension(docfile.FileName);
                    filename = adm.SCHL + "_" + DocName.Replace(" ", "_") + ext;
                    path = Path.Combine(Server.MapPath(exactPath), filename);
                    FilepathExist = Path.Combine(Server.MapPath(exactPath));
                    adm.DocFile = "Upload2023/AssociateDocuments/" + filename;
                }


                if (cmd.ToLower() == "submit" || cmd.ToLower() == "save")
                {

                    result = AssociateDB.InsertAssociationDocumentDetails(adm, 0, out outError);  // 0 for insert

                    if (!Directory.Exists(FilepathExist))
                    {
                        Directory.CreateDirectory(FilepathExist);
                    }
                    docfile.SaveAs(path);
                }
                else if (cmd.ToLower() == "delete")
                {
                    adm.SCHL = Convert.ToString(id);
                    // adm.eDocId = Convert.ToInt32(eStaffId);
                    result = AssociateDB.InsertAssociationDocumentDetails(adm, 2, out outError); // 2 for delete
                    if (outError == "1")
                    {
                        ViewBag.result = "1";
                        ViewData["Status"] = "DEL";
                        return View(adm);
                    }
                }

                if (result > 0)
                {
                    ViewData["result"] = "1";
                    ViewBag.Mesaage = outError;
                }
                else
                {
                    ViewData["result"] = outError.ToString();
                    ViewBag.Mesaage = outError;
                }
            }




            DataSet outDsAm = new DataSet();
            AssociateModel am1 = AssociateDB.AssociateContinuationBySchlTemp(Session["SCHL"].ToString(), 1, out outDsAm);//ResultStatics
            if (am1.ID > 0)
            {
                ViewBag.AID = am1.ID;
                ViewBag.ChallanId = am1.ChallanId;
                ViewBag.challanVerify = am1.challanVerify;
                ViewBag.RS10GPass2017 = am1.RS10GPass2017;
                ViewBag.SF10Percent = am1.SF10Percent;
                ViewBag.BSFROM = am1.BSFROM;
                ViewBag.FSFROM = am1.FSFROM;
                ViewBag.BPNAME = am1.BPNAME;
                ViewBag.ASZONE = am1.ASZONE;
                ViewBag.ASZONE = am1.ASZONE;
                ViewBag.OI1 = am1.OI1;
            }
            else
            {
                ViewBag.AID = 0;
                ViewBag.ChallanId = 0;
            }

            return View(adm);
        }


        public ActionResult ActionUploadAssociateDocuments(string id, string eDocId, string act)
        {
            try
            {
                string outError = "0";
                int result = 0;
                AssociationDocumentDetailsModel easdm = new AssociationDocumentDetailsModel();
                if (id == null || eDocId == null)
                {
                    //return RedirectToAction("Index", "AffiliationContinuation");
                }
                else
                {
                    if (act == "D")
                    {
                        easdm.SCHL = Convert.ToString(id);
                        easdm.eDocId = Convert.ToInt32(eDocId);
                        result = AssociateDB.InsertAssociationDocumentDetails(easdm, 2, out outError); // 2 for delete
                        if (outError == "1")
                        {
                            ViewBag.result = "1";
                            ViewData["Status"] = "DEL";
                        }
                    }
                }

            }
            catch (Exception)
            {
            }
            return RedirectToAction("UploadDocument", "AssociateCommuni", new { id = id.ToString() });

        }

        

        [HttpGet]
        public ActionResult SchoolInfra(SchoolInfraModel schoolInfraModel)
        {


            AssociateModel ViewModel = new AssociateModel();
            ViewModel.AssociationDocumentDetailsModelList = new List<AssociationDocumentDetailsModel>();
            ViewModel.AssociationDocumentDetailsModel = new AssociationDocumentDetailsModel();


            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

           

            
            //for yes no dropdown
            var YesnoItemList = new SelectList(new[] { new { ID = "Yes", Name = "Yes" }, new { ID = "No", Name = "No" }, }, "ID", "Name", 1);
            ViewBag.YesNoItem = YesnoItemList.ToList();
            ViewBag.SelectItem = "0";

            //for Play Ground Location dropdown
            var PglList = new SelectList(new[] { new { ID = "NA", Name = "NA" }, new { ID = "Within Premises", Name = "Within Premises" }, new { ID = "Out from Premises", Name = "Out from Premises" } }, "ID", "Name", 1);
            ViewBag.PglListItem = PglList.ToList();
            ViewBag.SelectPglItem = "0";

            //for Type Of internet Available dropdown
            var InternetList = new SelectList(new[] { new { ID = "NA", Name = "NA" }, new { ID = "Broadband Connection", Name = "Broadband Connection" }, new { ID = "Wireless Connecton", Name = "Wireless Connecton" }, new { ID = "Mobile Connecton", Name = "Mobile Connecton" } }, "ID", "Name", 1);
            ViewBag.InternetListListItem = InternetList.ToList();
            ViewBag.SelectInternetListtem = "0";

            return View(schoolInfraModel);
        }

        [HttpPost]
        public ActionResult SchoolInfra(string cmd, SchoolInfraModel schoolInfraModel)
        {


            if (Session["SCHL"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

           

            string outError = "0";
            int result = 0;
            //for yes no dropdown
            


            if (cmd.ToLower() == "submit" || cmd.ToLower() == "save")
            {
                 schoolInfraModel.SCHL = Session["SCHL"].ToString();
                result = AssociateDB.InsertAssociationSchoolInfrastructure(schoolInfraModel, 2, out outError);  // 0 for insert

                if (result > 0)
                {
                    ViewData["result"] = "1";
                    ViewBag.Mesaage = outError;
                }
                else
                {
                    ViewData["result"] = outError.ToString();
                    ViewBag.Mesaage = outError;
                }
            }

            return SchoolInfra(schoolInfraModel);
        }

       
    }
}

