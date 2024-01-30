using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSEBONLINE.Models;
using System.Data;

using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Globalization;
using Amazon.S3;
using Amazon;
using Amazon.S3.IO;
using Amazon.S3.Transfer;
using System.Configuration;

namespace PSEBONLINE.Controllers
{
    public class CompartmentCandidatesController : Controller
    {
        CompartmentCandidatesModel MS = new CompartmentCandidatesModel();
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.CompDB objDB = new AbstractLayer.CompDB();
        public string stdPic, stdSign;
        private const string BUCKET_NAME = "psebdata";
        // GET: CompartmentCandidates
        //--
        public ActionResult CompartmentStatus()
        {
            FormCollection frm = new FormCollection();
            AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
            AdminModels am = new AdminModels();
            var itemsch = new SelectList(new[] { new { ID = "1", Name = "Old Roll No" }, new { ID = "2", Name = "Ref No" }, new { ID = "3", Name = "Candidate Name" }, }, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();

            return View();

        }

        [HttpPost]
        public ActionResult CompartmentStatus(string cmd, string SelList, string SearchString, FormCollection frm)
        {
            AbstractLayer.AdminDB objDB = new AbstractLayer.AdminDB();
            AdminModels am = new AdminModels();
            var itemsch = new SelectList(new[] { new { ID = "1", Name = "Old Roll No" }, new { ID = "2", Name = "Ref No" }, new { ID = "3", Name = "Candidate Name" }, }, "ID", "Name", 1);
            ViewBag.MySch = itemsch.ToList();
            try
            {
                string Search = "";
                Search = "";

                if (cmd == "Search")
                {
                    if (SelList != "")
                    {
                        ViewBag.SelectedItem = SelList;
                        int SelValueSch = Convert.ToInt32(SelList.ToString());
                        if (SearchString != "")
                        {
                            if (SelValueSch == 1)
                            { Search += " [roll]='" + SearchString.ToString().Trim() + "'"; }
                            else if (SelValueSch == 2)
                            { Search += " refno='" + SearchString.ToString().Trim() + "'"; }
                            else if (SelValueSch == 3)
                            { Search += " Name like '%" + SearchString.ToString().Trim() + "%'"; }
                            Session["Search"] = Search;
                        }
                        ViewBag.Searchstring = SearchString.ToString().Trim();

                        am.StoreAllData = objDB.CompartmentStatus(Search);
                        ViewBag.TotalCountP = am.StoreAllData.Tables[0].Rows.Count;
                        ViewBag.lot = SearchString.ToString().Trim();
                        if (am.StoreAllData == null || am.StoreAllData.Tables[0].Rows.Count == 0)
                        {
                            ViewBag.Message = "Please Contact PSEB Head Office With Copy of Deposited Fee Challan and Admission Form.";
                            ViewBag.TotalCount = 0;
                            return View(am);
                        }
                        else
                        {
                            ViewBag.TotalCount = 1;
                            return View(am);
                        }

                    }
                    else
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(am);
                    }
                }
                return View(am);

            }
            catch (Exception)
            {
                return View(am);
            }
        }
        //-------
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetMonthID(string CATE) // Calling on http post (on Submit)
        {          
         
            if (CATE == "A")
            {
                List<SelectListItem> Monthlist = objDB.GetMonth();
                ViewBag.MyMonth = Monthlist;
                return Json(Monthlist);
            }
            else
            {
                DataSet Monthlist1 = objDB.GetSessionMonth();
                ViewBag.MyMonth = Monthlist1.Tables[0];
                List<SelectListItem> Monthlist = new List<SelectListItem>();
                //Monthlist.Add(new SelectListItem { Text = "Select Month", Value = "0" });
                foreach (System.Data.DataRow dr in ViewBag.MyMonth.Rows)
                {
                    Monthlist.Add(new SelectListItem { Text = @dr["sessionMonth"].ToString(), Value = @dr["sessionMonth"].ToString() });
                }
                ViewBag.MyMonth = Monthlist;
                return Json(Monthlist);
            }
        }
        public JsonResult GetYearID(string CATE) // Calling on http post (on Submit)
        {
            AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            PrivateCandidateModels MS = new PrivateCandidateModels();
            if (CATE == "A")
            {
                List<SelectListItem> yearlist = objDB.GetSessionYear1();
                //yearlist.Add(new SelectListItem { Text = "Select Year", Value = "0" });
                ViewBag.MyYear = yearlist;
                return Json(yearlist);
            }
            //else if (CATE == "D")
            //{
            //    DataSet yearlist1 = objDB.GetSessionYear();
            //    ViewBag.MyYear = yearlist1.Tables[0];
            //    List<SelectListItem> yearlist = new List<SelectListItem>();
            //    //yearlist.Add(new SelectListItem { Text = "Select Year", Value = "0" });
            //    foreach (System.Data.DataRow dr in ViewBag.MyYear.Rows)
            //    {
            //        yearlist.Add(new SelectListItem { Text = @dr["sessionYear"].ToString(), Value = @dr["sessionYear"].ToString() });
            //    }
            //    yearlist.RemoveAt(2);
            //    ViewBag.MyYear = yearlist;
            //    return Json(yearlist);
            //}
            else
            {
                //DataSet yearlist1 = objDB.GetSessionYear();
                //ViewBag.MyYear = yearlist1.Tables[0];
                //List<SelectListItem> yearlist = new List<SelectListItem>();

                //foreach (System.Data.DataRow dr in ViewBag.MyYear.Rows)
                //{
                //    //FOR pRIVATE
                //    //   yearlist.Add(new SelectListItem { Text = @dr["sessionYear"].ToString(), Value = @dr["sessionYear"].ToString() });

                //}

                List<SelectListItem> yearlist = new List<SelectListItem>();
                yearlist.Add(new SelectListItem { Text = "2018", Value = "2018" });
                //yearlist.Add(new SelectListItem { Text = "2017", Value = "2017" });
                //yearlist.Add(new SelectListItem { Text = "2016", Value = "2016" });
                //yearlist.Add(new SelectListItem { Text = "2015", Value = "2015" });
                //yearlist.Add(new SelectListItem { Text = "2014", Value = "2014" });
                ViewBag.MyYear = yearlist;
                return Json(yearlist);
            }

        }
        public ActionResult CompCandidateExamForm()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CompCandidateExamForm(FormCollection frc)
        {
            try
            {              
                MS.refNo = frc["refNo"];
                MS.OROLL = frc["OROLL"];

                if (MS.refNo != null && MS.refNo.Length == 13)
                {
                    MS.StoreAllData = objDB.GetDetailCCM(MS);
                    ViewBag.Message = "";
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Given Reference & Roll number not currect.";
                        ViewBag.TotalCount = 0;
                        return View(MS);
                    }
                    else
                    {

                        if (MS.refNo == MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString() && MS.OROLL == MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString())
                        {
                            Session["Oroll"] = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                            ViewData["refno"] = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                            Session["refno"] = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                            ViewData["roll"] = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                            ViewData["Status"] = "1";

                            string Clss = MS.StoreAllData.Tables[0].Rows[0]["class"].ToString();
                            if (Clss == "10")
                            {
                                Session["ClassStatus"] = "Matriculation Examination Form For Re-Appear (Regular) , JUNE-2017";
                            }
                            else
                            {
                                Session["ClassStatus"] = "Sr.Secondary Examination Form For Compartment (Regular) , JUNE-2017";
                            }


                            return View(MS);
                            //return RedirectToAction("MigrationRec", "MigrateSchool");
                        }
                        
                        return View(MS);
                    }
                }
                return View();
            }
            catch (Exception ex)
            {

                return RedirectToAction("CompCandidateExamForm", "CompartmentCandidates");
            }

        }
        public ActionResult CompCandidateIntroForm()
      {
            
            //List<SelectListItem> yearlist = objDB.GetSessionYear1();
            //ViewBag.MyYear = yearlist;


            //List<SelectListItem> Monthlist = objDB.GetMonth();
            //ViewBag.MyMonth = Monthlist;

            return View();
            
        }
        [HttpPost]
        public ActionResult CompCandidateIntroForm(FormCollection frm, CompartmentCandidatesModel MS)
        {
            
            DataSet result = objDB.GetSessionYear(); // passing Value to DBClass from model
            ViewBag.MyYear = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.MyYear.Rows)
            {
                items.Add(new SelectListItem { Text = @dr["sessionYear"].ToString(), Value = @dr["sessionYear"].ToString() });
            }
            ViewBag.MyYear = new SelectList(items, "Value", "Text");

            DataSet result1 = objDB.GetSessionMonth(); // passing Value to DBClass from model
            ViewBag.MyMonth = result1.Tables[0];
            List<SelectListItem> items1 = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in ViewBag.MyMonth.Rows)
            {
                items1.Add(new SelectListItem { Text = @dr["sessionMonth"].ToString(), Value = @dr["sessionMonth"].ToString() });
            }
            ViewBag.MyMonth = new SelectList(items1, "Value", "Text");

            try
            {
                MS.Class = frm["Class"];
                MS.category = frm["category"];
                MS.Exam_Type = frm["Exam_Type"];
                MS.SelMonth = frm["SelMonth"];
                MS.SelYear = frm["SelYear"];
                MS.OROLL = frm["OROLL"];
                MS.emailID = frm["emailID"];
                MS.mobileNo = frm["mobileNo"];

                if (frm["OROLL"] != null && frm["OROLL"] != "")
                {
                  
                    DataSet result2 = objDB.InsertCompCandidateForm(MS);
                    if (result2.Tables[0].Rows.Count > 0)
                    {
                        if (result2.Tables[0].Rows[0]["result"].ToString() == "1")
                        {
                            string Oroll = frm["OROLL"].ToString();
                            string Clss = frm["Class"];
                            string Yar = frm["SelYear"];
                            string Mnth = frm["SelMonth"];
                            string Cat = frm["category"].ToString();
                            ViewData["Status"] = "1";
                            if(Clss=="10")
                            {
                                Session["ClassStatus"] = "Matriculation Examination Form For Re-Appear (Regular) , JUNE-2017";
                            }
                            else
                            {
                                Session["ClassStatus"] = "Sr.Secondary Examination Form For Compartment (Regular) , JUNE-2017";
                            }
                            
                            return RedirectToAction("PreCompCandidateIntroForm", new { id = Oroll, Clss, Yar, Mnth, Cat });
                            
                        }
                        if (result2.Tables[0].Rows[0]["result"].ToString() == "2")
                        {
                            ViewData["roll"] = MS.OROLL;
                            ViewData["RefNo"] = result2.Tables[0].Rows[0]["refno"].ToString();
                            ViewData["Status"] = "2";
                            return View(MS);
                        }
                        if (result2.Tables[0].Rows[0]["result"].ToString() == "3")
                        {

                            ViewData["roll"] = MS.OROLL;
                            ViewData["Status"] = "3";
                            return View(MS);

                        }
                        if (result2.Tables[0].Rows[0]["result"].ToString() == "0")
                        {
                            ViewData["roll"] = MS.OROLL;
                            ViewData["Status"] = "0";
                            return View(MS);
                        }

                    }
                    else
                    {
                        ViewData["roll"] = MS.OROLL;
                        ViewData["Status"] = "0";
                        return View(MS);
                    }

                }
            }
            catch (Exception ex)
            {

                return View(MS);
            }


            return View(MS);
        }
        public ActionResult PreCompCandidateIntroForm(string id, string Clss, string Yar, string Mnth, string Cat)
        {
            try
            {
               
                if (id != null && Clss != null && Yar != null && Mnth != null && Cat != null)
                {
                    MS.StoreAllData = objDB.GetCompCandidateDetails(id, Clss, Yar, Mnth, Cat);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        //ViewBag.cnt = 0;
                        ViewBag.Message = "Record Not Found";
                        return View();
                    }
                    if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A")
                    {
                        MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["rp"].ToString();
                        if (MS.Exam_Type == "R")
                        {
                            MS.Exam_Type = "Regular";
                        }
                        if (MS.Exam_Type == "O")
                        {
                            MS.Exam_Type = "Open";
                        }
                        if (MS.Exam_Type == "P")
                        {
                            MS.Exam_Type = "Private";
                        }
                        //MS.category = MS.StoreAllData.Tables[1].Rows[0]["category"].ToString();
                        MS.category = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                        Session["category"] = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                        if (MS.category == "R")
                        {
                            MS.category = "Reappear/Compartment";
                        }
                        if (MS.category == "D")
                        {
                            MS.category = "Division improvement";
                        }
                        if (MS.category == "A")
                        {
                            MS.category = "Additional subject";
                        }

                        MS.Class = MS.StoreAllData.Tables[0].Rows[0]["class"].ToString();
                        if (MS.Class == "10")
                        {
                            MS.Class = "Matriculation";
                        }
                        if (MS.Class == "12")
                        {
                            MS.Class = "Senior Secondary";
                        }

                        MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                        MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
                        MS.Session = MS.SelMonth + ' ' + MS.SelYear;
                        MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                        MS.Result = ""; //MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();
                        MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();
                        MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();

                        MS.SCHL = "";// MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                        MS.Candi_Name = "";// MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                        MS.Father_Name = "";//MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                        MS.Mother_Name = "";//MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();

                        MS.Pname = "";//MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
                        MS.PFname = "";//MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
                        MS.PMname = "";//MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();
                        MS.EPname = MS.Candi_Name + '/' + MS.Pname;
                        MS.EPFname = MS.Father_Name + '/' + MS.PFname;
                        MS.EPMname = MS.Mother_Name + '/' + MS.PMname;

                        MS.DOB = "";//MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        MS.RegNo = "";//MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();


                        return View(MS);
                    }
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        return View();
                    }
                    else
                    {
                        //MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["exam"].ToString();
                        MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["RP"].ToString();
                        if (MS.Exam_Type == "R")
                        {
                            MS.Exam_Type = "Regular";
                        }
                        if (MS.Exam_Type == "O")
                        {
                            MS.Exam_Type = "Open";
                        }
                        if (MS.Exam_Type == "P")
                        {
                            MS.Exam_Type = "Private";
                        }
                        //MS.category = MS.StoreAllData.Tables[1].Rows[0]["category"].ToString();
                        MS.category = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                        Session["category"] = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                        if (MS.category == "R")
                        {
                            MS.category = "Reappear/Compartment";
                        }
                        if (MS.category == "D")
                        {
                            MS.category = "Division improvement";
                        }
                        if (MS.category == "A")
                        {
                            MS.category = "Additional subject";
                        }

                        MS.Class = MS.StoreAllData.Tables[0].Rows[0]["class"].ToString();
                        if (MS.Class == "10")
                        {
                            MS.Class = "Matriculation";
                        }
                        if (MS.Class == "12")
                        {
                            MS.Class = "Senior Secondary";
                        }

                        MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                        MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
                        MS.Session = MS.SelMonth + ' ' + MS.SelYear;
                        MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                        MS.Result = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();
                        MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();
                        MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();

                        MS.SCHL = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                        MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                        MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                        MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();

                        MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
                        MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
                        MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();
                        MS.EPname = MS.Candi_Name + '/' + MS.Pname;
                        MS.EPFname = MS.Father_Name + '/' + MS.PFname;
                        MS.EPMname = MS.Mother_Name + '/' + MS.PMname;

                        MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();


                        return View(MS);
                        //return Private_Candidate_Examination_Form();
                        //return Pre_Private_Candidate_Introduction_Form(frm);
                    }
                }
                else
                {
                    //return View(MS);

                    //return Private_Candidate_Examination_Form();
                    return RedirectToAction("CompCandidateExamForm", "CompartmentCandidates");
                }
            }
            catch (Exception ex)
            {
                //return Private_Candidate_Examination_Form();
                return RedirectToAction("CompCandidateExamForm", "CompartmentCandidates");
            }
        }
        [HttpPost]
        public ActionResult PreCompCandidateIntroForm(FormCollection frm)
        {
            try
            {
                
                MS.Class = frm["class"].ToString();
                if (MS.Class == "Matriculation")
                {
                    MS.Class = "10";
                    MS.DOB = frm["DOB"].ToString();
                }
                if (MS.Class == "Senior Secondary")
                {
                    MS.Class = "12";
                    MS.DOB = "";
                }

                MS.category = frm["category"].ToString();
                if (MS.category == "Reappear/Compartment")
                {
                    MS.category = "R";
                }
                if (MS.category == "Division improvement")
                {
                    MS.category = "D";
                }
                if (MS.category == "Additional subject")
                {
                    MS.category = "A";
                }
                MS.Exam_Type = frm["Exam_Type"].ToString();

                MS.SelMonth = frm["SelMonth"];
                MS.SelYear = frm["SelYear"];

                //MS.Session = frm["Session"].ToString();
                MS.OROLL = frm["OROLL"].ToString();
                Session["Oroll"] = MS.OROLL;
                MS.emailID = frm["emailID"].ToString();
                MS.mobileNo = frm["mobileNo"].ToString();

                MS.Result = frm["Result"].ToString();
                MS.RegNo = frm["RegNo"].ToString();
                MS.Candi_Name = frm["Candi_Name"].ToString();
                MS.Pname = frm["Pname"].ToString();
                MS.Father_Name = frm["Father_Name"].ToString();
                MS.PFname = frm["PFname"].ToString();
                MS.Mother_Name = frm["Mother_Name"].ToString();
                MS.PMname = frm["PMname"].ToString();

                if (MS.OROLL != null)
                {
                    MS.StoreAllData = objDB.GetCompCandidateDetails(MS.OROLL, MS.Class, MS.SelYear, MS.SelMonth, MS.category);
                    
                    MS.Class = MS.StoreAllData.Tables[0].Rows[0]["class"].ToString();
                   
                    MS.category = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                    if (MS.category == "R")
                    {
                        MS.category = "Reappear/Compartment";
                    }
                    if (MS.category == "D")
                    {
                        MS.category = "Division improvement";
                    }
                    if (MS.category == "A")
                    {
                        MS.category = "Additional subject";
                    }
                }
                else
                {
                    return CompCandidateExamForm();
                }

                DataSet result2 = objDB.GenerateCompCandidateFormRefno(MS);

                if (result2.Tables[0].Rows[0]["RESULT"].ToString() == "1")
                {
                    //string clss=string.Empty;
                    if (MS.Class == "10") { MS.Class = "Matriculation"; }
                    if (MS.Class == "12") { MS.Class = "Senior Secondary"; }

                    ViewData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    Session["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    TempData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    ViewData["roll"] = result2.Tables[0].Rows[0]["roll"].ToString();
                    TempData["roll"] = result2.Tables[0].Rows[0]["roll"].ToString();
                    ViewData["name"] = result2.Tables[0].Rows[0]["name"].ToString();
                    //TempData["ClassDetails"] = result2.Tables[0].Rows[0]["class"].ToString();                    

                    TempData["Classinfo"] = MS.Class + " (" + MS.category + " , " + MS.Exam_Type + " )";
                    ViewData["Status"] = "1";
                    //------------------
                    try
                    {
                        AbstractLayer.DBClass dbclass = new AbstractLayer.DBClass();
                        if (MS.mobileNo != null || MS.mobileNo != "")
                        {
                            string Sms = "You are successfully registred for Class " + TempData["Classinfo"] + " JULY 2017 and your refrence no. " + ViewData["refno"] + " is generated against old roll no. " + ViewData["roll"] + ", Keep this for further use till result declaration.";

                            string getSms = dbclass.gosms(MS.mobileNo, Sms);
                            // string getSms = objCommon.gosms("9711819184", Sms);
                        }
                        if (MS.emailID != null || MS.emailID != "")
                        {
                            string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Dear " + ViewData["name"] + "</b>,</td></tr><tr><td height=" + 30 + ">As per your request Dated <b>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</b> Regarding Private Form</td></tr><tr><td><b>You are successfully registred for:-</b><br /><b>Class :</b> " + TempData["Classinfo"] + " March 2017 <br /><b> Reference No. :</b> " + ViewData["refno"] + "<br /><b> Old Roll No. :</b> " + ViewData["roll"] + "<br /><b> Keep this for further use till result declaration.</b> <br /></td></tr><tr><td height=" + 30 + "><b>Click Here To Login</b> <a href=https://registration2023.pseb.ac.in/PrivateCandidate/Private_Candidate_Examination_Form target = _blank>https://registration2023.pseb.ac.in</a></td></tr><tr><td><b>Note:</b> Please Read Instruction Carefully Before filling the Online Form .</td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /><tr><td><b>Contact Us</b><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 8058911911<br>DISTRICTS:- BARNALA, FATEHGARH SAHIB, GURDASPUR, HOSHIARPUR, JALANDHAR, KAPURTHALA, SHRI MUKTSAR SAHIB, S.B.S. NAGAR, PATHANKOT, PATIALA, SANGRUR, CHANDIGARH &amp; OTHER STATES<br><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 8058911911<br>DISTRICTS:- AMRITSAR, BATHINDA, FARIDKOT, FAZILKA, FEROZEPUR, LUDHIANA, MANSA, MOGA, ROOP NAGAR, S.A.S NAGAR,TARN TARAN<br></td></tr>";

                            string subject = "PSEB-Compartment Form Notification";
                            bool result = dbclass.mail(subject, body, MS.emailID);
                        }

                    }
                    catch (Exception) { }

                    return View(MS);                    
                }
                if (result2.Tables[0].Rows[0]["RESULT"].ToString() == "2")
                {
                    ViewData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    ViewData["roll"] = result2.Tables[0].Rows[0]["roll"].ToString();
                    ViewData["Status"] = "2";
                    return View(MS);
                    
                }
                if (result2.Tables[0].Rows[0]["RESULT"].ToString() == "0")
                {
                    ViewData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    ViewData["roll"] = result2.Tables[0].Rows[0]["roll"].ToString();
                    ViewData["Status"] = "0";
                    return View(MS);
                    
                }

                return View();
            }
            catch (Exception ex)
            {
                return CompCandidateExamForm();
            }
        }
        public ActionResult CompCandidateConfirmation(string refno)
        {
          
            string start = Request.QueryString["Oroll"];
            string Oroll = string.Empty;
            try
            {
                
                //Oroll = "1014819040";
                Oroll = Session["Oroll"].ToString();
                refno = Session["refno"].ToString();

                DataSet result2 = objDB.CHKCompCand(refno, Oroll);
                if (result2.Tables[0].Rows[0]["RESULT1"].ToString() == "1")
                {

                    ViewData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    ViewData["FormS"] = "FS";
                    //return View(MS);

                }
                else if (result2.Tables[0].Rows[0]["RESULT1"].ToString() == "0")
                {

                    ViewData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    ViewData["FormS"] = "NFS";
                    //return View(MS);

                }

                if (refno != null && refno != "")
                {                    
                    MS.StoreAllData = objDB.GetCompCandidateConfirmation(refno);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        return View();
                    }
                    else
                    {

                        MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["rp"].ToString();
                        //MS.category = MS.StoreAllData.Tables[1].Rows[0]["category"].ToString();
                        if (MS.Exam_Type == "R")
                        {
                            MS.Exam_Type = "Regular";
                        }
                        if (MS.Exam_Type == "O")
                        {
                            MS.Exam_Type = "Open";
                        }
                        if (MS.Exam_Type == "P")
                        {
                            MS.Exam_Type = "Private";
                        }
                        //MS.category = MS.StoreAllData.Tables[1].Rows[0]["category"].ToString();
                        MS.category = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                        if (MS.category == "R")
                        {
                            MS.category = "Reappear/Compartment";
                        }
                        if (MS.category == "D")
                        {
                            MS.category = "Division improvement";
                        }
                        if (MS.category == "A")
                        {
                            MS.category = "Additional subject";
                        }

                        if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "R" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                        {
                            //Session["form"] = "MatricReapMarch2017";
                            Session["form"] = "MatricReapJune";
                        }
                        else if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "D" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                        {
                            Session["form"] = "MatricDICMarch2017";
                        }
                        else if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                        {
                            Session["form"] = "MatricAdditionalMarch2017";
                        }
                        else if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "R" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "12")
                        {
                            Session["form"] = "SrSecReapJune";
                        }
                        else if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "D" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "12")
                        {
                            Session["form"] = "SrSecDICMarch2017";
                        }
                        else if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "12")
                        {
                            Session["form"] = "SrSecAdditionalMarch2017";
                        }
                        
                        MS.Class = MS.StoreAllData.Tables[0].Rows[0]["class"].ToString();
                        if (MS.Class == "10")
                        {
                            MS.Class = "Matriculation";
                            MS.MatricSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                            MS.MatricSub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                        }
                        if (MS.Class == "12")
                        {
                            MS.TwelveSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                            MS.Class = "Senior Secondary";

                        }
                        MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                        MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
                        MS.Session = MS.SelMonth + ' ' + MS.SelYear;

                        MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                        Session["OROLL"] = MS.OROLL;
                        Session["category"] = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();

                        MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();
                        MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();

                        MS.refNo = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                        MS.SCHL = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();

                        MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                        MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                        MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();

                        MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();
                        MS.Result = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();

                        MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
                        MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
                        MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();

                        MS.EPname = MS.Candi_Name + '/' + MS.Pname;
                        MS.EPFname = MS.Father_Name + '/' + MS.PFname;
                        MS.EPMname = MS.Mother_Name + '/' + MS.PMname;

                        MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["sex"].ToString();
                        MS.CastList = MS.StoreAllData.Tables[0].Rows[0]["caste"].ToString();
                        MS.Area = MS.StoreAllData.Tables[0].Rows[0]["area"].ToString();
                        if (MS.Area == "U")
                        {
                            MS.Area = "Urban";
                        }
                        if (MS.Area == "R")
                        {
                            MS.Area = "Rural";
                        }
                        MS.Relist = MS.StoreAllData.Tables[0].Rows[0]["religion"].ToString();
                        if (MS.Relist == "H")
                        {
                            MS.Relist = "Hindu";
                        }
                        if (MS.Relist == "M")
                        {
                            MS.Relist = "Muslim";
                        }
                        if (MS.Relist == "S")
                        {
                            MS.Relist = "Sikh";
                        }
                        if (MS.Relist == "C")
                        {
                            MS.Relist = "Christian";
                        }
                        if (MS.Relist == "O")
                        {
                            MS.Relist = "Others";
                        }

                        
                        MS.phyChal = MS.StoreAllData.Tables[0].Rows[0]["phy_chal"].ToString();                                             

                        MS.rdoWantWriter = MS.StoreAllData.Tables[0].Rows[0]["writer"].ToString();
                        if (MS.rdoWantWriter == "True")
                        {
                            MS.rdoWantWriter = "1";
                        }
                        if (MS.rdoWantWriter == "False")
                        {
                            MS.rdoWantWriter = "0";
                        }

                        MS.IsPracExam = MS.StoreAllData.Tables[0].Rows[0]["prac"].ToString();
                        if (MS.IsPracExam == "True")
                        {
                            MS.IsPracExam = "1";
                        }
                        if (MS.IsPracExam == "False")
                        {
                            MS.IsPracExam = "0";
                        }

                        MS.Choice1 = MS.StoreAllData.Tables[0].Rows[0]["cent_1"].ToString();
                        MS.Choice2 = MS.StoreAllData.Tables[0].Rows[0]["cent_2"].ToString();
                        MS.address = MS.StoreAllData.Tables[0].Rows[0]["homeadd"].ToString();

                        MS.landmark = MS.StoreAllData.Tables[0].Rows[0]["LandMark"].ToString();
                        MS.block = MS.StoreAllData.Tables[0].Rows[0]["block"].ToString();                        
                        MS.SelExamDist = MS.StoreAllData.Tables[0].Rows[0]["dist"].ToString();
                        MS.SelDist = MS.StoreAllData.Tables[0].Rows[0]["homedistco"].ToString();
                        MS.tehsil = MS.StoreAllData.Tables[0].Rows[0]["tehsil"].ToString();
                        MS.pinCode = MS.StoreAllData.Tables[0].Rows[0]["pincode"].ToString();
                        DataSet result = objDB.SelectDist(); // passing Value to DBClass from model
                        ViewBag.MyDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> items = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                        {
                            items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                        }
                        // ViewBag.MyDist = items;
                        ViewBag.MyDist = new SelectList(items, "Value", "Text");

                        if (MS.SelDist == "")
                        {
                            MS.SelDist = "0";
                        }

                        ViewBag.MyExamDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> items1 = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyExamDist.Rows)
                        {
                            items1.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                        }
                        // ViewBag.MyDist = items;
                        ViewBag.MyExamDist = new SelectList(items1, "Value", "Text");

                        if (MS.SelExamDist == "")
                        {
                            MS.SelExamDist = "0";
                        }
                        //----------------------------ExamDist Tehsil-------------
                        int examdist = Convert.ToInt32(MS.SelExamDist);
                        DataSet ExamTehresult = objDB.SelectAllTehsil(examdist);
                        ViewBag.MyTehsil = ExamTehresult.Tables[0];
                        List<SelectListItem> ExamTehList = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
                        {
                            ExamTehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });
                        }
                        ViewBag.MyExamTehsil = ExamTehList;
                        //-------------------------------Dist-------------------
                        int dist = Convert.ToInt32(MS.SelDist);
                        DataSet result1 = objDB.SelectAllTehsil(dist);
                        ViewBag.MyTehsil = result1.Tables[0];
                        List<SelectListItem> TehList = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
                        {
                            TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });
                        }
                        ViewBag.MyTehsil = TehList;
                        if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "R")
                        {
                            MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                            MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                            MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString();
                            MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString();
                            MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString();
                            MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString();
                        }
                        else if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "D")
                        {
                            MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                            MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                            MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString();
                            MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString();
                            MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString();
                            MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString();
                        }
                        else if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                        {
                            List<SelectListItem> Mitems = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[1].Rows)
                            {
                                Mitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.SubMatric = new SelectList(Mitems, "Value", "Text");


                        }
                        else if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "12")
                        {
                            List<SelectListItem> sitems = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[2].Rows)
                            {
                                sitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.SubTwelve = new SelectList(sitems, "Value", "Text");
                        }

                        //@ViewBag.Photo = "../../upload/" + MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();                                         
                        //@ViewBag.sign = "../../upload/" + MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();
                        @ViewBag.Photo = "https://registration2022.pseb.ac.in/upload/" + MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
                        @ViewBag.sign = "https://registration2022.pseb.ac.in/upload/" + MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();

                        MS.imgSign = MS.StoreAllData.Tables[0].Rows[0]["Sign_url"].ToString();
                        MS.imgPhoto = MS.StoreAllData.Tables[0].Rows[0]["Photo_url"].ToString();

                        Session["imgPhoto"] = MS.imgPhoto;
                        Session["imgSign"] = MS.imgSign;

                        MS.FormStatus = MS.StoreAllData.Tables[0].Rows[0]["FormStatus"].ToString();

                        return View(MS);
                       
                    }
                }
                else
                {
                   
                    return RedirectToAction("CompCandidateExamForm", "CompartmentCandidates");
                }
            }
            catch (Exception ex)
            {
                
                return RedirectToAction("CompCandidateExamForm", "CompartmentCandidates");
            }
        }
        [HttpPost]
        public ActionResult CompCandidateConfirmation(CompartmentCandidatesModel MS, FormCollection frm)
        {
            try
            {
                
                DataSet result = objDB.SelectDist(); // passing Value to DBClass from model
                ViewBag.MyDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }
                
                ViewBag.MyDist = new SelectList(items, "Value", "Text");

                ViewBag.MyExamDist = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items1 = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyExamDist.Rows)
                {
                    items1.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                }
                
                ViewBag.MyExamDist = new SelectList(items1, "Value", "Text");

                if (MS.SelExamDist == "")
                {
                    MS.SelExamDist = "0";
                }


                if (frm["SelExamDist"] != null)
                {
                    MS.SelExamDist = frm["SelExamDist"].ToString();
                }
                else
                {
                    MS.SelExamDist = "0";
                }

              
                MS.Exam_Type = frm["Exam_Type"].ToString();
                MS.refNo = frm["refNo"].ToString();
                MS.Session = frm["Session"].ToString();
                MS.Result = frm["result"].ToString();
                MS.Class = frm["Class"].ToString();
                if (MS.Class == "Matriculation")
                {
                    MS.Class = "10";
                    MS.DOB = frm["DOB"].ToString();
                }
                if (MS.Class == "Senior Secondary")
                {
                    MS.Class = "12";
                    MS.DOB = "";
                }
                MS.Candi_Name = frm["Candi_Name"].ToString();
                MS.Father_Name = frm["Father_Name"].ToString();
                MS.Mother_Name = frm["Mother_Name"].ToString();

                MS.Pname = frm["PNAME"].ToString();
                MS.PFname = frm["PFNAME"].ToString();
                MS.PMname = frm["PMNAME"].ToString();
                MS.RegNo = frm["RegNo"].ToString();
                MS.OROLL = frm["OROLL"].ToString();
                Session["OROLL"] = MS.OROLL;
                Session["refno"] = MS.refNo;
                MS.mobileNo = frm["mobileNo"].ToString();
                MS.emailID = frm["emailID"].ToString();

                MS.Gender = frm["Gender"].ToString();

                MS.CastList = frm["CastList"].ToString();
                MS.Area = frm["Area"].ToString();
                if (MS.Area == "Urban")
                {
                    MS.Area = "U";
                }
                if (MS.Area == "Rural")
                {
                    MS.Area = "R";
                }
                MS.Relist = frm["Relist"].ToString();
                if (MS.Relist == "Hindu")
                {
                    MS.Relist = "H";
                }
                if (MS.Relist == "Muslim")
                {
                    MS.Relist = "M";
                }
                if (MS.Relist == "Sikh")
                {
                    MS.Relist = "S";
                }
                if (MS.Relist == "Christian")
                {
                    MS.Relist = "C";
                }
                if (MS.Relist == "Others")
                {
                    MS.Relist = "O";
                }

                MS.IsphysicalChall = frm["phyChal"].ToString();
                if (MS.IsphysicalChall != "N.A." && frm["rdoWantWriter"] != null)
                {
                    //MS.IsphysicalChall = frm["phyChal"].ToString();
                    MS.rdoWantWriter = frm["rdoWantWriter"].ToString();
                    if (MS.rdoWantWriter == "Yes")
                    {
                        MS.rdoWantWriter = "1";
                    }
                    else
                    {
                        MS.rdoWantWriter = "0";
                    }
                }
                else
                {
                    MS.IsphysicalChall = "N.A.";
                    MS.rdoWantWriter = "0";
                }
                if (MS.Class == "12")
                {
                    MS.Choice1 = frm["Choice1"].ToString();
                    MS.Choice2 = frm["Choice2"].ToString();
                    MS.address = frm["address"].ToString();

                    MS.landmark = frm["landmark"].ToString();
                    MS.block = frm["block"].ToString();
                }
                else
                {
                    MS.Choice1 = frm["Choice1"].ToString();                   
                    MS.address = frm["address"].ToString();
                    MS.landmark = frm["landmark"].ToString();
                    MS.block = frm["block"].ToString();
                }


                //----------------------------ExamDist Tehsil-------------
                int examdist = Convert.ToInt32(MS.SelExamDist);
                DataSet ExamTehresult = objDB.SelectAllTehsil(examdist);
                ViewBag.MyTehsil = ExamTehresult.Tables[0];
                List<SelectListItem> ExamTehList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
                {
                    ExamTehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });
                }
                ViewBag.MyExamTehsil = ExamTehList;
                //-------------------------------Dist-------------------

                MS.SelDist = frm["SelDist"].ToString();

                int dist = Convert.ToInt32(MS.SelDist);
                DataSet result1 = objDB.SelectAllTehsil(dist);
                ViewBag.MyTehsil = result1.Tables[0];
                List<SelectListItem> TehList = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
                {

                    TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });

                }
                ViewBag.MyTehsil = TehList;

                MS.tehsil = frm["tehsil"].ToString();
                MS.pinCode = frm["pinCode"].ToString();
                MS.category = frm["category"].ToString();

                if (MS.category == "Reappear/Compartment")
                {
                    MS.category = "R";
                }
                if (MS.category == "Division improvement")
                {
                    MS.category = "D";
                }
                if (MS.category == "Additional subject")
                {
                    MS.category = "A";
                }
                if (MS.category == "A" || MS.category == "D")
                {
                    MS.IsPracExam = "0";
                }
                else
                {
                    MS.IsPracExam = frm["IsPracExam"].ToString();
                    if (MS.IsPracExam == "Yes")
                    {
                        MS.IsPracExam = "1";
                    }
                    else
                    {
                        MS.IsPracExam = "0";
                    }
                }

                if (MS.category == "R" || MS.category == "D")
                {
                    if (frm["sub1"] != null)
                    { MS.sub1 = frm["sub1"].ToString(); }
                    else { MS.sub1 = ""; }
                    if (frm["sub2"] != null) { MS.sub2 = frm["sub2"].ToString(); }
                    else { MS.sub2 = ""; }
                    if (frm["sub3"] != null) { MS.sub3 = frm["sub3"].ToString(); }
                    else { MS.sub3 = ""; }
                    if (frm["sub4"] != null) { MS.sub4 = frm["sub4"].ToString(); }
                    else { MS.sub4 = ""; }
                    if (frm["sub5"] != null) { MS.sub5 = frm["sub5"].ToString(); }
                    else { MS.sub5 = ""; }
                    if (frm["sub6"] != null) { MS.sub6 = frm["sub6"].ToString(); }
                    else { MS.sub6 = ""; }

                }
               
                else if (MS.category == "A" & MS.Class == "10")
                {
                    MS.sub1 = frm["MatricSub"].ToString();
                    //MS.sub2 = frm["sub2"].ToString();
                    if (frm["MatricSub2"] != null && frm["MatricSub2"] != "0") { MS.sub2 = frm["MatricSub2"].ToString(); }
                    else { MS.sub2 = ""; }
                    MS.sub3 = "";
                    MS.sub4 = "";
                    MS.sub5 = "";
                    MS.sub6 = "";
                }
                else if (MS.category == "A" & MS.Class == "12")
                {
                    MS.sub1 = frm["TwelveSub"].ToString();
                    MS.sub2 = "";
                    MS.sub3 = "";
                    MS.sub4 = "";
                    MS.sub5 = "";
                    MS.sub6 = "";
                }
                
                if (MS.std_Photo != null)
                {
                    //stdPic = Path.GetFileName(MS.std_Photo.FileName);
                    //string Filepath = Server.MapPath("~/Upload/PvtPhoto/Photo/");
                    //if (!Directory.Exists(Filepath))
                    //{
                    //    Directory.CreateDirectory(Filepath);
                    //}
                    string Orgfile = MS.refNo + "P" + ".jpg";
                    MS.PathPhoto = "PvtPhoto/Photo/" + MS.refNo + "P" + ".jpg";

                    //MS.std_Photo.SaveAs(pathPhoto);

                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            ///file.CopyTo(newMemoryStream);

                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = MS.std_Photo.InputStream,
                                Key = string.Format("allfiles/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Photo/{0}", Orgfile),
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
                    //Response.Write("<script>alert('Please Upload Photo')</script>");
                    //return View();
                }
                if (MS.std_Sign != null)
                {
                   

                    string Orgfile = MS.refNo + "S" + ".jpg";
                    MS.PathSign = "PvtPhoto/Sign/" + MS.refNo + "S" + ".jpg";                   

                    using (var client = new AmazonS3Client(ConfigurationManager.AppSettings["AWSKey"], ConfigurationManager.AppSettings["AWSValue"], RegionEndpoint.APSouth1))
                    {
                        using (var newMemoryStream = new MemoryStream())
                        {
                            ///file.CopyTo(newMemoryStream);

                            var uploadRequest = new TransferUtilityUploadRequest
                            {
                                InputStream = MS.std_Photo.InputStream,
                                Key = string.Format("allfiles/Upload2023/PvtPhoto/Batch" + MS.refNo.Substring(3, 4) + "/Sign/{0}", Orgfile),
                                BucketName = BUCKET_NAME,
                                CannedACL = S3CannedACL.PublicRead
                            };

                            var fileTransferUtility = new TransferUtility(client);
                            fileTransferUtility.Upload(uploadRequest);
                        }
                    }

                }
                MS.imgPhoto = Session["imgPhoto"].ToString();
                MS.imgSign = Session["imgSign"].ToString();




                if ((MS.std_Photo == null || MS.std_Sign == null) && (MS.imgPhoto == null || MS.imgSign == null))
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetCompCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select Photo and Sign";
                    ViewData["SelectPhotoSign"] = "0";
                    return View(MS);
                }


                if (MS.Gender == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetCompCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select Gender";
                    ViewData["SelectGender"] = "0";
                    return View(MS);
                }

                if (MS.CastList == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetCompCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select Cast";
                    ViewData["SelectCast"] = "0";
                    return View(MS);
                }
                if (MS.Area == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetCompCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select Area";
                    ViewData["SelectArea"] = "0";
                    return View(MS);
                }
                if (MS.Relist == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetCompCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select Religion";
                    ViewData["SelectRelist"] = "0";
                    return View(MS);
                }
                
                if (MS.address == "")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetCompCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Enter Address";
                    ViewData["Selectaddress"] = "0";
                    return View(MS);
                }
                if (MS.SelDist == "" || MS.tehsil == "" || MS.tehsil == "0")
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetCompCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Select District & Tehsil";
                    ViewData["SelectDist"] = "0";
                    return View(MS);
                }
                if (MS.pinCode == "" || MS.pinCode.Count() != 6)
                {
                    MS.category = frm["category"].ToString();
                    MS.Class = frm["Class"].ToString();
                    MS.Gender = frm["Gender"].ToString();
                    MS.Area = frm["Area"].ToString();
                    MS.Relist = frm["Relist"].ToString();
                    MS.StoreAllData = objDB.GetCompCandidateConfirmation(MS.refNo);
                    ViewBag.Message = "Enter Pin Code";
                    ViewData["SelectPin"] = "0";
                    return View(MS);
                }

                DataSet result2 = objDB.InsertCompCandidateConfirmation(MS);
                MS.StoreAllData = objDB.GetCompCandidateConfirmation(MS.refNo);

                if ((result2.Tables[0].Rows[0]["RESULT"].ToString() == "1") && (result2.Tables[0].Rows[0]["RESULT1"].ToString() == "1"))
                {
                    
                    ViewData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    ViewData["Status"] = "3";
                    ViewData["FormS"] = "FS";
                    return View(MS);
                   
                }
                if ((result2.Tables[0].Rows[0]["RESULT"].ToString() == "1") && (result2.Tables[0].Rows[0]["RESULT1"].ToString() == "0"))
                {
                    ViewData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    ViewData["Status"] = "NFS";
                    return View(MS);
                }
                if (result2.Tables[0].Rows[0]["RESULT1"].ToString() == "1")
                {

                    ViewData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    ViewData["Status"] = "3";
                    ViewData["Status"] = "FS";
                    return View(MS);

                }
                else if (result2.Tables[0].Rows[0]["RESULT1"].ToString() == "0")
                {

                    ViewData["refno"] = result2.Tables[0].Rows[0]["refno"].ToString();
                    ViewData["FormS"] = "NFS";
                    return View(MS);

                }


                return View(frm);
            }
            catch (Exception ex)
            {
                return CompCandidateExamForm();
            }
        }

        public ActionResult CompCandidateConfirmationEdit()
        {
            try
            {
                
                DataSet result = objDB.SelectDist(); 

                if (Session["refno"].ToString() == null || Session["refno"].ToString() == "")
                {
                    return CompCandidateExamForm();
                }
                string Oroll = Session["Oroll"].ToString();
                string RefNo = Session["refno"].ToString();
                MS.OROLL = Oroll;
                MS.refNo = RefNo;
                int result1 = objDB.EditCompCandidateConfirmation(MS);
                if (result1 == 1)
                {
                   
                    MS.StoreAllData = objDB.GetCompCandidateConfirmation(MS.refNo);
                    if (MS.StoreAllData == null || MS.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        return View();
                    }
                    else
                    {

                        MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["rp"].ToString();
                       if (MS.Exam_Type == "R")
                        {
                            MS.Exam_Type = "Regular";
                        }
                        if (MS.Exam_Type == "O")
                        {
                            MS.Exam_Type = "Open";
                        }
                        if (MS.Exam_Type == "P")
                        {
                            MS.Exam_Type = "Private";
                        }
                        
                        MS.category = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                        if (MS.category == "R")
                        {
                            MS.category = "Reappear/Compartment";
                        }
                        if (MS.category == "D")
                        {
                            MS.category = "Division improvement";
                        }
                        if (MS.category == "A")
                        {
                            MS.category = "Additional subject";
                        }


                        MS.Class = MS.StoreAllData.Tables[0].Rows[0]["class"].ToString();
                        if (MS.Class == "10")
                        {
                            MS.Class = "Matriculation";
                            MS.MatricSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            MS.MatricSub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString();
                           
                        }
                        if (MS.Class == "12")
                        {
                            MS.TwelveSub = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                            MS.Class = "Senior Secondary";
                        }
                        MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                        MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
                        MS.Session = MS.SelMonth + ' ' + MS.SelYear;

                        MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                        Session["OROLL"] = MS.OROLL;
                        Session["category"] = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();

                        MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();
                        MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();

                        MS.refNo = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                        MS.SCHL = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();

                        MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                        MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                        MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();

                        MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                        MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();
                        MS.Result = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();

                        MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
                        MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
                        MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();

                        MS.EPname = MS.Candi_Name + '/' + MS.Pname;
                        MS.EPFname = MS.Father_Name + '/' + MS.PFname;
                        MS.EPMname = MS.Mother_Name + '/' + MS.PMname;

                        MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["sex"].ToString();
                        MS.CastList = MS.StoreAllData.Tables[0].Rows[0]["caste"].ToString();
                        MS.Area = MS.StoreAllData.Tables[0].Rows[0]["area"].ToString();
                        if (MS.Area == "U")
                        {
                            MS.Area = "Urban";
                        }
                        if (MS.Area == "R")
                        {
                            MS.Area = "Rural";
                        }
                        MS.Relist = MS.StoreAllData.Tables[0].Rows[0]["religion"].ToString();
                        if (MS.Relist == "H")
                        {
                            MS.Relist = "Hindu";
                        }
                        if (MS.Relist == "M")
                        {
                            MS.Relist = "Muslim";
                        }
                        if (MS.Relist == "S")
                        {
                            MS.Relist = "Sikh";
                        }
                        if (MS.Relist == "C")
                        {
                            MS.Relist = "Christian";
                        }
                        if (MS.Relist == "O")
                        {
                            MS.Relist = "Others";
                        }

                        MS.phyChal = MS.StoreAllData.Tables[0].Rows[0]["phy_chal"].ToString();

                        MS.rdoWantWriter = MS.StoreAllData.Tables[0].Rows[0]["writer"].ToString();
                        if (MS.rdoWantWriter == "True")
                        {
                            MS.rdoWantWriter = "1";
                        }
                        if (MS.rdoWantWriter == "False")
                        {
                            MS.rdoWantWriter = "0";
                        }

                        MS.IsPracExam = MS.StoreAllData.Tables[0].Rows[0]["prac"].ToString();
                        if (MS.IsPracExam == "True")
                        {
                            MS.IsPracExam = "1";
                        }
                        if (MS.IsPracExam == "False")
                        {
                            MS.IsPracExam = "0";
                        }

                        MS.Choice1 = MS.StoreAllData.Tables[0].Rows[0]["cent_1"].ToString();
                        MS.Choice2 = MS.StoreAllData.Tables[0].Rows[0]["cent_2"].ToString();
                        MS.address = MS.StoreAllData.Tables[0].Rows[0]["homeadd"].ToString();

                        MS.landmark = MS.StoreAllData.Tables[0].Rows[0]["LandMark"].ToString();
                        MS.block = MS.StoreAllData.Tables[0].Rows[0]["block"].ToString();
                        //MS.SelDist = MS.StoreAllData.Tables[0].Rows[0]["distName"].ToString();
                        //MS.tehsil = MS.StoreAllData.Tables[0].Rows[0]["tehsilName"].ToString();

                        MS.SelDist = MS.StoreAllData.Tables[0].Rows[0]["homedistco"].ToString();
                        MS.tehsil = MS.StoreAllData.Tables[0].Rows[0]["tehsil"].ToString();
                        MS.pinCode = MS.StoreAllData.Tables[0].Rows[0]["pincode"].ToString();
                        DataSet Distresult = objDB.SelectDist(); // passing Value to DBClass from model
                        ViewBag.MyDist = Distresult.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                        List<SelectListItem> items = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyDist.Rows)
                        {
                            items.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                        }
                        // ViewBag.MyDist = items;
                        ViewBag.MyDist = new SelectList(items, "Value", "Text");

                        MS.SelExamDist = MS.StoreAllData.Tables[0].Rows[0]["dist"].ToString();
                        ViewBag.MyExamDist = result.Tables[0];// Edit Mode for dislaying message after saving storing output.
                        List<SelectListItem> items1 = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyExamDist.Rows)
                        {
                            items1.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
                        }
                        // ViewBag.MyDist = items;
                        ViewBag.MyExamDist = new SelectList(items1, "Value", "Text");

                        if (MS.SelExamDist == "")
                        {
                            MS.SelExamDist = "0";
                        }
                        //----------------------------ExamDist Tehsil-------------
                        int examdist= Convert.ToInt32(MS.SelExamDist);
                        DataSet ExamTehresult = objDB.SelectAllTehsil(examdist);
                        ViewBag.MyTehsil = ExamTehresult.Tables[0];
                        List<SelectListItem> ExamTehList = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
                        {
                            ExamTehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });
                        }
                        ViewBag.MyExamTehsil = ExamTehList;
                        //-------------------------------Dist-------------------

                        int dist = Convert.ToInt32(MS.SelDist);
                        DataSet Tehresult = objDB.SelectAllTehsil(dist);
                        ViewBag.MyTehsil = Tehresult.Tables[0];
                        List<SelectListItem> TehList = new List<SelectListItem>();
                        foreach (System.Data.DataRow dr in ViewBag.MyTehsil.Rows)
                        {
                            TehList.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });
                        }
                        ViewBag.MyTehsil = TehList;



                        if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "R")
                        {
                            MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                            MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                            MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString();
                            MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString();
                            MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString();
                            MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString();
                        }
                        else if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "D")
                        {
                            MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                            MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                            MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString();
                            MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString();
                            MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString();
                            MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString();
                        }
                        else if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "10")
                        {
                            List<SelectListItem> Mitems = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[1].Rows)
                            {
                                Mitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.SubMatric = new SelectList(Mitems, "Value", "Text");
                        }
                        else if (MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString() == "A" && MS.StoreAllData.Tables[0].Rows[0]["class"].ToString() == "12")
                        {
                            List<SelectListItem> sitems = new List<SelectListItem>();
                            foreach (System.Data.DataRow dr in MS.StoreAllData.Tables[2].Rows)
                            {
                                sitems.Add(new SelectListItem { Text = @dr["name_eng"].ToString(), Value = @dr["sub"].ToString() });
                            }
                            ViewBag.SubTwelve = new SelectList(sitems, "Value", "Text");
                        }

                        @ViewBag.Photo = "https://registration2022.pseb.ac.in/upload/" + MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
                        @ViewBag.sign = "https://registration2022.pseb.ac.in/upload/" + MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();

                        //@ViewBag.Photo = "../../upload/" + MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
                        ////MS.imgSign= MS.StoreAllData.Tables[1].Rows[0]["PathSign"].ToString();                        
                        //@ViewBag.sign = "../../upload/" + MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();

                        MS.imgSign = MS.StoreAllData.Tables[0].Rows[0]["Sign_url"].ToString();
                        MS.imgPhoto = MS.StoreAllData.Tables[0].Rows[0]["Photo_url"].ToString();

                        Session["imgPhoto"] = MS.imgPhoto;
                        Session["imgSign"] = MS.imgSign;


                        MS.FormStatus = MS.StoreAllData.Tables[0].Rows[0]["FormStatus"].ToString();

                        return View(MS);
                        
                    }
                }
                else
                {
                    return RedirectToAction("CompCandidateExamForm", "CompartmentCandidates");
                }
            }
            catch (Exception ex)
            {
                //return CompCandidateExamForm();
                return RedirectToAction("CompCandidateExamForm", "CompartmentCandidates");
            }
        }

        public ActionResult CompCandidateFinalPrint(FormCollection frc)
        {
            
            try
            {
                if (Session["OROLL"].ToString() != null || Session["OROLL"].ToString() != "")
                {
                    MS.OROLL = Session["OROLL"].ToString();
                    MS.refNo = Session["refNo"].ToString();
                    MS.StoreAllData = objDB.GetPCompCandidateConfirmationPrint(MS.refNo);

                    ///----------------------------------------------------------------
                    MS.Class = MS.StoreAllData.Tables[0].Rows[0]["class"].ToString();
                    MS.centrCode = MS.StoreAllData.Tables[0].Rows[0]["cent"].ToString();
                    MS.setNo = MS.StoreAllData.Tables[0].Rows[0]["set"].ToString();
                    MS.SelExamDist = MS.StoreAllData.Tables[0].Rows[0]["ExamDist"].ToString();
                    MS.distName = MS.StoreAllData.Tables[0].Rows[0]["DISTNM"].ToString();
                    MS.Exam_Type = MS.StoreAllData.Tables[0].Rows[0]["utype"].ToString();
                    MS.category = MS.StoreAllData.Tables[0].Rows[0]["cat"].ToString();
                    if (MS.category == "R")
                    {
                        MS.category = "Reappear/Compartment";
                    }
                    if (MS.category == "D")
                    {
                        MS.category = "Division improvement";
                    }
                    if (MS.category == "A")
                    {
                        MS.category = "Additional subject";
                    }
                    MS.phyChal = MS.StoreAllData.Tables[0].Rows[0]["phy_chal"].ToString();

                    @ViewBag.ExamCen = MS.StoreAllData.Tables[0].Rows[0]["matricExamCenter"].ToString();
                    MS.Choice1 = MS.StoreAllData.Tables[0].Rows[0]["cent_1"].ToString();
                    MS.Choice2 = MS.StoreAllData.Tables[0].Rows[0]["cent_2"].ToString();
                    //@ViewBag.Photo = "../../upload/" + MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
                    //@ViewBag.sign = "../../upload/" + MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();

                    @ViewBag.Photo = "https://registration2022.pseb.ac.in/upload/" + MS.StoreAllData.Tables[0].Rows[0]["photo_url"].ToString();
                    @ViewBag.sign = "https://registration2022.pseb.ac.in/upload/" + MS.StoreAllData.Tables[0].Rows[0]["sign_url"].ToString();

                    MS.SelYear = MS.StoreAllData.Tables[0].Rows[0]["year"].ToString();
                    MS.SelMonth = MS.StoreAllData.Tables[0].Rows[0]["session"].ToString();
                    MS.Session = MS.SelMonth + '/' + MS.SelYear;
                    MS.OROLL = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                    MS.emailID = MS.StoreAllData.Tables[0].Rows[0]["emailid"].ToString();

                  
                    MS.refNo = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                    MS.SCHL = MS.StoreAllData.Tables[0].Rows[0]["SCHL"].ToString();
                    MS.Candi_Name = MS.StoreAllData.Tables[0].Rows[0]["NAME"].ToString();
                    MS.Father_Name = MS.StoreAllData.Tables[0].Rows[0]["FNAME"].ToString();
                    MS.Mother_Name = MS.StoreAllData.Tables[0].Rows[0]["MNAME"].ToString();
                    MS.Pname = MS.StoreAllData.Tables[0].Rows[0]["PNAME"].ToString();
                    MS.PFname = MS.StoreAllData.Tables[0].Rows[0]["PFNAME"].ToString();
                    MS.PMname = MS.StoreAllData.Tables[0].Rows[0]["PMNAME"].ToString();
                    MS.EPname = MS.Candi_Name + '/' + MS.Pname;
                    MS.EPFname = MS.Father_Name + '/' + MS.PFname;
                    MS.EPMname = MS.Mother_Name + '/' + MS.PMname;

                    MS.Gender = MS.StoreAllData.Tables[0].Rows[0]["sex"].ToString();
                    MS.CastList = MS.StoreAllData.Tables[0].Rows[0]["caste"].ToString();
                    MS.DOB = MS.StoreAllData.Tables[0].Rows[0]["DOB"].ToString();
                    MS.RegNo = MS.StoreAllData.Tables[0].Rows[0]["regno"].ToString();
                    MS.Result = MS.StoreAllData.Tables[0].Rows[0]["result"].ToString();
                    MS.SearchResult = MS.StoreAllData.Tables[0].Rows[0]["resultdtl"].ToString();

                    MS.mobileNo = MS.StoreAllData.Tables[0].Rows[0]["mobile"].ToString();
                    MS.Relist = MS.StoreAllData.Tables[0].Rows[0]["religion"].ToString();
                    if (MS.Relist == "H")
                    {
                        MS.Relist = "Hindu";
                    }
                    if (MS.Relist == "M")
                    {
                        MS.Relist = "Muslim";
                    }
                    if (MS.Relist == "S")
                    {
                        MS.Relist = "Sikh";
                    }
                    if (MS.Relist == "C")
                    {
                        MS.Relist = "Christian";
                    }
                    if (MS.Relist == "O")
                    {
                        MS.Relist = "Others";
                    }

                    MS.sub1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1"].ToString();
                    MS.sub2 = MS.StoreAllData.Tables[0].Rows[0]["rsub2"].ToString();
                    MS.sub3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3"].ToString();
                    MS.sub4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4"].ToString();
                    MS.sub5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5"].ToString();
                    MS.sub6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6"].ToString();

                    MS.sub1code = MS.StoreAllData.Tables[0].Rows[0]["rsub1abbr"].ToString();
                    MS.sub2code = MS.StoreAllData.Tables[0].Rows[0]["rsub2abbr"].ToString();
                    MS.sub3code = MS.StoreAllData.Tables[0].Rows[0]["rsub3abbr"].ToString();
                    MS.sub4code = MS.StoreAllData.Tables[0].Rows[0]["rsub4abbr"].ToString();
                    MS.sub5code = MS.StoreAllData.Tables[0].Rows[0]["rsub5abbr"].ToString();
                    MS.sub6code = MS.StoreAllData.Tables[0].Rows[0]["rsub6abbr"].ToString();


                    ViewBag.SCode1 = MS.StoreAllData.Tables[0].Rows[0]["rsub1code"].ToString();
                    ViewBag.SCode2= MS.StoreAllData.Tables[0].Rows[0]["rsub2code"].ToString();
                    ViewBag.SCode3 = MS.StoreAllData.Tables[0].Rows[0]["rsub3code"].ToString();
                    ViewBag.SCode4 = MS.StoreAllData.Tables[0].Rows[0]["rsub4code"].ToString();
                    ViewBag.SCode5 = MS.StoreAllData.Tables[0].Rows[0]["rsub5code"].ToString();
                    ViewBag.SCode6 = MS.StoreAllData.Tables[0].Rows[0]["rsub6code"].ToString();

                    MS.address = MS.StoreAllData.Tables[0].Rows[0]["homeadd"].ToString();
                    MS.block = MS.StoreAllData.Tables[0].Rows[0]["block"].ToString();
                    MS.tehsil = MS.StoreAllData.Tables[0].Rows[0]["TEHSILENM"].ToString();
                    MS.pinCode = MS.StoreAllData.Tables[0].Rows[0]["pinCode"].ToString();
                    MS.addressfull = (MS.address + ',' + MS.block + ',' + MS.distName + ',' + MS.tehsil + ',' + MS.pinCode);
                    //if (MS.Class=="10")
                    //{
                    //    MS.addressfull = ( MS.distName + ',' + MS.tehsil + ',' + MS.pinCode);
                    //}
                    //else
                    //{
                    //    MS.addressfull = (MS.address + ',' + MS.block + ',' + MS.distName + ',' + MS.tehsil + ',' + MS.pinCode);
                    //}

                    ViewBag.challanno= MS.StoreAllData.Tables[0].Rows[0]["ChallanID"].ToString();
                    ViewBag.amt = MS.StoreAllData.Tables[0].Rows[0]["TOTFEE"].ToString();
                    ViewBag.bank = MS.StoreAllData.Tables[0].Rows[0]["BANK"].ToString();
                    ViewBag.cvdate = MS.StoreAllData.Tables[0].Rows[0]["CHLNVDATE"].ToString();
                    ViewBag.Brefno = MS.StoreAllData.Tables[0].Rows[0]["J_REF_NO"].ToString();
                    ViewBag.CDDT = MS.StoreAllData.Tables[0].Rows[0]["DEPOSITDT"].ToString();
                    ViewBag.Branch = MS.StoreAllData.Tables[0].Rows[0]["BRANCH"].ToString();
                    //ViewBag.challanno = MS.StoreAllData.Tables[0].Rows[0]["ChallanID"].ToString();

                    return View(MS);
                }
                else
                {
                    return RedirectToAction("CompCandidateExamForm", "CompartmentCandidates");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("CompCandidateExamForm", "CompartmentCandidates");
            }
        }


        #region Begin final Submit
        public ActionResult FinalSubmit(FormCollection frc)
        {            
            try
            {
                if (Session["OROLL"].ToString() != null || Session["OROLL"].ToString() != "")
                {
                    MS.OROLL = Session["OROLL"].ToString();
                    MS.refNo = Session["refNo"].ToString();
                    MS.StoreAllData = objDB.GetCompCandidateConfirmationFinalSubmit(MS.refNo);                   
                    if (MS.StoreAllData.Tables[0].Rows[0]["result"].ToString() == "1")
                    {
                        ViewData["roll"] = Session["OROLL"].ToString();
                        ViewData["refno"] = Session["refNo"].ToString();
                        ViewData["Status"] = "1";
                        // return RedirectToAction("CompCandidateFinalPrint", "CompartmentCandidates");
                        return RedirectToAction("CompCandidateConfirmation", "CompartmentCandidates"); //By ranjan
                    }
                    else
                    {
                        return CompCandidateExamForm();
                    }

                }
                else
                {
                    return CompCandidateExamForm();
                }
            }
            catch (Exception ex)
            {
                return CompCandidateExamForm();
            }
        }
        #endregion end final Submit

        #region Generate Challan
        //----------------------------------------- Challan ---------------------------------
        public ActionResult PaymentForm(string roll)
        {
            CompPaymentformViewModel pfvm = new CompPaymentformViewModel();
            //AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            if (Session["Oroll"] == null || Session["Oroll"].ToString() == "" || Session["form"] == null)
            {
                return RedirectToAction("CompCandidateConfirmation", "CompartmentCandidates");
            }
            DataSet Bds = objDB.BankStatus();
            pfvm.StoreAllData = Bds;
            if (pfvm.StoreAllData == null || Bds.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "No Bank Found";
                ViewBag.TotalCount = 0;
                return View();
            }
            else
            {
                ViewBag.TotalCount = pfvm.StoreAllData.Tables[0].Rows.Count;
            }

            roll = Session["Oroll"].ToString();
            string RefNo = Session["refno"].ToString();           
            string form = Session["form"].ToString();
            DataSet ds = objDB.GetCompCandidateDetailsPayment(RefNo, form);
            pfvm.PaymentFormData = ds;
            if (pfvm.PaymentFormData == null || pfvm.PaymentFormData.Tables[0].Rows.Count == 0)
            {
                ViewBag.Message = "Record Not Found";
                ViewBag.TotalCount = 0;
                return View();
            }
            else
            {
                pfvm.Class = ds.Tables[0].Rows[0]["class"].ToString();
                if (pfvm.Class == "10")
                {
                    pfvm.Class = "Matriculation";

                }
                if (pfvm.Class == "12")
                {
                    pfvm.Class = "Senior Secondary";
                }
                pfvm.ExamType = ds.Tables[0].Rows[0]["exam"].ToString();
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
                pfvm.Dist = ds.Tables[0].Rows[0]["homedistco"].ToString();
                pfvm.District = ds.Tables[0].Rows[0]["DISTNM"].ToString();               
                pfvm.SchoolCode = ds.Tables[0].Rows[0]["schl"].ToString();
                pfvm.SchoolName = ds.Tables[0].Rows[0]["SCHLE"].ToString(); // Schollname with station and dist 
                ViewBag.TotalCount = pfvm.PaymentFormData.Tables[0].Rows.Count;

                DataSet dscalFee = ds; //(DataSet)Session["CalculateFee"];
                pfvm.TotalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["fee"].ToString());
                pfvm.TotalLateFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["latefee"].ToString());
                pfvm.TotalFinalFees = Convert.ToInt32(dscalFee.Tables[1].Rows[0]["totfee"].ToString());

                string rps = NumberToWords(Convert.ToInt32(dscalFee.Tables[1].Rows[0]["totfee"].ToString()));
                //pfvm.TotalFeesInWords = dscalFee.Tables[1].Rows[0]["totfee"].ToString();
                pfvm.TotalFeesInWords = rps;

                
                pfvm.FeeDate = Convert.ToDateTime(dscalFee.Tables[1].Rows[0]["eDate"].ToString());               
                pfvm.FeeCode = dscalFee.Tables[1].Rows[0]["FEECODE"].ToString();
                pfvm.FeeCategory = dscalFee.Tables[1].Rows[0]["FEECAT"].ToString();

                Session["PaymentForm"] = pfvm;                
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

        [HttpPost]
        public ActionResult PaymentForm(CompPaymentformViewModel pfvm, FormCollection frm)
        {
            CompChallanMasterModel CM = new CompChallanMasterModel();            
            if (Session["Oroll"] == null || Session["Oroll"].ToString() == "")
            {
                return RedirectToAction("CompCandidateExamForm", "CompartmentCandidates");
            }
            string roll = Session["Oroll"].ToString();

            if (Session["Oroll"] == null || Session["Oroll"].ToString() == "")
            {
                return RedirectToAction("CompCandidateExamForm", "CompartmentCandidates");
            }

           
            //if (ModelState.IsValid)
            //{
                
                CM.FeeStudentList = "1";
                CompPaymentformViewModel PFVMSession = (CompPaymentformViewModel)Session["PaymentForm"];
                CM.roll = roll;
                CM.FEE = Convert.ToInt32(PFVMSession.TotalFinalFees);
                CM.TOTFEE = Convert.ToInt32(PFVMSession.TotalFinalFees);
                CM.FEECAT = PFVMSession.FeeCategory;
                CM.FEECODE = PFVMSession.FeeCode;
                CM.FEEMODE = "CASH";
                
                CM.BANK = pfvm.BankName;
                CM.BCODE = pfvm.BankCode;
                string bcode= pfvm.BankCode;
                DataSet Bds = objDB.BankStatusCode(bcode);
                pfvm.StoreAllData = Bds;
                if (pfvm.StoreAllData == null || Bds.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "No Bank Found";
                    ViewBag.TotalCount = 0;
                    return View();
                }
                else
                {
                    CM.BANK = Bds.Tables[0].Rows[0]["BankName"].ToString();
                }

                CM.BANKCHRG = PFVMSession.BankCharges;
                CM.SchoolCode = PFVMSession.SchoolCode.ToString();
                CM.DIST = PFVMSession.Dist.ToString();
                CM.DISTNM = PFVMSession.District;
                //CM.LOT = PFVMSession.LOTNo;
                CM.LOT = 1;
                CM.SCHLREGID = PFVMSession.roll.ToString();
                CM.type = "candt";
               
                CM.CHLNVDATE = DateTime.Now.ToString("dd/MM/yyyy"); //PFVMSession.FeeDate;
                CM.ChallanVDateN = PFVMSession.FeeDate; //PFVMSession.FeeDate;                


                string CandiMobile = "";
                // string result = "0";

                if (pfvm.BankCode == null)
                {
                    ViewBag.Message = "Please Select Bank";
                    ViewData["SelectBank"] = "1";
                    return View(pfvm);
                }

                if (Session["CheckFormFee"].ToString() == "0")
                { pfvm.BankCode = "203"; }

                string result = objDB.InsertPaymentFormComp(CM, frm, out CandiMobile);
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
                    CM.CHLNVDATE = (Convert.ToString(PFVMSession.FeeDate)).Substring(0, 10);
                    //ViewBag.Message = "File has been uploaded successfully";
                    //Your Challan no. XXXXXXXXXX of Lot no  XX successfully generated and valid till Dt XXXXXXXXXXX. Regards PSEB
                    string Sms = "Your Challan no. " + result + " of Lot no  " + CM.LOT + " successfully generated and valid till Dt " + CM.CHLNVDATE + ". Regards PSEB";
                    try
                    {
                        string getSms = objCommon.gosms(CandiMobile, Sms);
                        // string getSms = objCommon.gosms("9711819184", Sms);
                    }
                    catch (Exception) { }

                    ModelState.Clear();
                    //--For Showing Message---------//                   
                    return RedirectToAction("GenerateChallaan", "CompartmentCandidates", new { ChallanId = result });
                }
           // }
            return View(pfvm);
        }

        public ActionResult GenerateChallaan(string ChallanId)
        {
            if (ChallanId == null || ChallanId == "0")
            {
                return RedirectToAction("CompCandidateExamForm", "CompartmentCandidates");
            }

            if (Session["Oroll"] == null || Session["Oroll"].ToString() == "")
            {
                return RedirectToAction("CompCandidateExamForm", "CompartmentCandidates");
            }
            CompChallanMasterModel CM = new CompChallanMasterModel();           
            string schl = "";
            if (Convert.ToString(Session["SCHL"]) != "")
            {
                schl = Session["SCHL"].ToString();
            }
            
            string ChallanId1 = ChallanId.ToString();

            DataSet ds = objDB.GetChallanDetailsByIdComp(ChallanId1);
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
                CM.TOTFEE = float.Parse(ds.Tables[0].Rows[0]["PaidFees"].ToString());
                CM.FEECAT = ds.Tables[0].Rows[0]["FEECAT"].ToString();
                CM.FEECODE = ds.Tables[0].Rows[0]["FEECODE"].ToString();
                CM.FEEMODE = ds.Tables[0].Rows[0]["FEEMODE"].ToString();
                CM.BANK = ds.Tables[0].Rows[0]["BANK"].ToString();
                ViewBag.BCODE = CM.BCODE = ds.Tables[0].Rows[0]["BCODE"].ToString();
                CM.BANKCHRG = float.Parse(ds.Tables[0].Rows[0]["BANKCHRG"].ToString());
                //CM.SchoolCode = ds.Tables[0].Rows[0]["SchoolCode"].ToString();
                CM.SchoolCode = ds.Tables[0].Rows[0]["SCHLREGID"].ToString();
                CM.APPNO = ds.Tables[0].Rows[0]["APPNO"].ToString();
                // CM.SchoolCode = Session["SCHL"].ToString();
                CM.DIST = ds.Tables[0].Rows[0]["DIST"].ToString();
                CM.DISTNM = ds.Tables[0].Rows[0]["DISTNM"].ToString();
                CM.LOT = Convert.ToInt32(ds.Tables[0].Rows[0]["LOT"].ToString());
                CM.TotalFeesInWords = ds.Tables[0].Rows[0]["TotalFeesInWords"].ToString();
                //CM.SchoolName = ds.Tables[0].Rows[0]["SchoolName"].ToString();
                CM.SCHLCANDNM = ds.Tables[0].Rows[0]["candidateNM"].ToString();
                CM.DepositoryMobile = ds.Tables[0].Rows[0]["DepositoryMobile"].ToString();
                CM.type = ds.Tables[0].Rows[0]["type"].ToString();
                if (ds.Tables[0].Rows[0]["Verified"].ToString() == "1")
                {
                    CM.BRCODE = ds.Tables[0].Rows[0]["BRCODE"].ToString();
                    CM.BRANCH = ds.Tables[0].Rows[0]["BRANCH"].ToString();
                    CM.J_REF_NO = ds.Tables[0].Rows[0]["J_REF_NO"].ToString();
                    CM.DEPOSITDT = ds.Tables[0].Rows[0]["DEPOSITDT"].ToString();
                }

                Session["CalculateFee"] = null;
                Session["PaymentForm"] = null;
                Session["FeeStudentList"] = null;
                return View(CM);
            }
        }
        
        public string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

        #endregion end Generate Challan

        #region Forgot Password
        //------------------------ Forgot Password

        public ActionResult ForgotPassword()
        {
            
            List<SelectListItem> yearlist = objDB.GetSessionYear1();
            ViewBag.MyYear = yearlist;


            List<SelectListItem> Monthlist = objDB.GetMonth();
            ViewBag.MyMonth = Monthlist;

            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(FormCollection frm, PrivateCandidateModels MS)
        {
            //AbstractLayer.PrivateCandidateDB objDB = new AbstractLayer.PrivateCandidateDB();
            //PrivateCandidateModels MS = new PrivateCandidateModels();
            try
            {
                DataSet result = objDB.GetSessionYear(); // passing Value to DBClass from model
                ViewBag.MyYear = result.Tables[0];// ViewData["result"] = result; // for dislaying message after saving storing output.
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyYear.Rows)
                {
                    items.Add(new SelectListItem { Text = @dr["sessionYear"].ToString(), Value = @dr["sessionYear"].ToString() });
                }
                ViewBag.MyYear = new SelectList(items, "Value", "Text");

                DataSet result1 = objDB.GetSessionMonth(); // passing Value to DBClass from model
                ViewBag.MyMonth = result1.Tables[0];
                List<SelectListItem> items1 = new List<SelectListItem>();
                foreach (System.Data.DataRow dr in ViewBag.MyMonth.Rows)
                {
                    items1.Add(new SelectListItem { Text = @dr["sessionMonth"].ToString(), Value = @dr["sessionMonth"].ToString() });
                }
                ViewBag.MyMonth = new SelectList(items1, "Value", "Text");


                MS.Class = frm["Class"];
                MS.category = frm["category"];
                MS.Exam_Type = frm["Exam_Type"];
                MS.SelMonth = frm["SelMonth"];
                MS.SelYear = frm["SelYear"];
                MS.OROLL = frm["OROLL"];
                MS.emailID = frm["emailID"];
                MS.mobileNo = frm["mobileNo"];

                if (frm["OROLL"] != null && frm["OROLL"] != "")
                {
                    MS.StoreAllData = objDB.getForgotPassword(MS);

                    if (MS.StoreAllData.Tables[0].Rows.Count > 0)
                    {

                        ViewData["refno"] = MS.StoreAllData.Tables[0].Rows[0]["refno"].ToString();
                        ViewData["roll"] = MS.StoreAllData.Tables[0].Rows[0]["roll"].ToString();
                        ViewData["Status"] = "1";

                        try
                        {
                            AbstractLayer.DBClass dbclass = new AbstractLayer.DBClass();
                            if (MS.mobileNo != null || MS.mobileNo != "")
                            {
                                string Sms = "Your refrence no. " + ViewData["refno"] + "  against old roll no. " + ViewData["roll"] + ", Keep this for further use till result declaration.";

                                string getSms = dbclass.gosms(MS.mobileNo, Sms);
                                
                            }
                          

                        }
                        catch (Exception) { }
                        return View(MS);
                       
                    }

                    else
                    {
                        ViewData["roll"] = MS.OROLL;
                        ViewData["Status"] = "0";
                        return View(MS);
                    }

                }
            }
            catch (Exception ex)
            {

                return View(MS);
            }


            return View(MS);
        }

        #endregion end Forgot Password
    }
}