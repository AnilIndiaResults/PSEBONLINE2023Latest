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
using PSEBONLINE.AbstractLayer;

namespace PSEBONLINE.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        AbstractLayer.AdminDB objCommon1 = new AbstractLayer.AdminDB();
        AbstractLayer.ErrorLog oErrorLog = new AbstractLayer.ErrorLog();
        [Route("login1")]
        public ActionResult Index1(string id)
        {
            ViewBag.SessionList = objCommon.GetSession().ToList();
            try
            {

                if (id == null) /*-- for test*/
                {
                    string myIP;
                    myIP = Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(':')[0];
                    if (myIP == "" || myIP == null)
                        myIP = Request.ServerVariables["REMOTE_ADDR"].Split(':')[0];
                    if (myIP == "112.196.84.226")
                    {
                        // ViewBag.myIP = myIP;
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Home", "Home");
                    }
                }
                else if (id.ToUpper() == "ROHIT") /*-- for test*/
                {                   
                    return View();
                }
                else
                {
                    return RedirectToAction("Home", "Home");
                }
            }
            catch (Exception)
            {                
                return RedirectToAction("Home", "Home");
            }
        }

        [Route("login1")]
        [HttpPost]
        public ActionResult Index1(LoginModel2 lm)
        {
            ViewBag.SessionList = objCommon.GetSession().ToList();
            LoginModel obj = new LoginModel();
            obj.username = lm.username;
            obj.Password = lm.Password;
          
            
            AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
            DataSet ds = objDB.CheckLogin(obj); // passing Value to SchoolDB from model and Type 1 For regular
            DataTable dt = ds.Tables[0];
            LoginModel obj1 = new LoginModel();
            obj1.username = lm.admusername;
            obj1.Password = lm.admPassword;
            // DataTable dt1 = objCommon1.CheckAdminLogin(obj1);
            DataTable dt1 = null;
            if (dt == null || dt1 == null)
            {
                ViewData["result"] = "-1";
                return View();
            }
            if (dt.Rows.Count > 0 && dt1.Rows.Count > 0)
            {
                //if (dt.Rows[0]["RoleType"].ToString() != "Admin")
                //{
                if (dt.Rows[0]["Active"].ToString() != "")
                {
                    ViewData["result"] = null;
                    HttpContext.Session["SchoolLogin"] = dt.Rows[0]["schl"].ToString();
                    HttpContext.Session["myIP"] = "Yes";
                    HttpContext.Session["RoleType"] = dt.Rows[0]["RoleType"].ToString();
                    HttpContext.Session["SchlE"] = dt.Rows[0]["SchlE"].ToString();
                    HttpContext.Session["SchlP"] = dt.Rows[0]["SchlP"].ToString();
                    HttpContext.Session["IDNO"] = dt.Rows[0]["IDNO"].ToString();
                    HttpContext.Session["SCHL"] = dt.Rows[0]["schl"].ToString();
                    HttpContext.Session["STATION"] = dt.Rows[0]["STATIONE"].ToString();
                    HttpContext.Session["STATIONP"] = dt.Rows[0]["STATIONP"].ToString();
                    HttpContext.Session["SCHOOLDIST"] = dt.Rows[0]["DIST"].ToString();
                    HttpContext.Session["DISTE"] = dt.Rows[0]["DISTE"].ToString();
                    HttpContext.Session["DISTP"] = dt.Rows[0]["DISTP"].ToString();
                    // HttpContext.Session["Session"] = "No Session";
                    HttpContext.Session["Session"] = lm.Session;
                    HttpContext.Session["Class"] = dt.Rows[0]["Class"].ToString();
                    Session["SchType"] = dt.Rows[0]["Abbr"].ToString();
                    HttpContext.Session["FullSchoolNameE"] = dt.Rows[0]["FullSchoolNameE"].ToString();
                    HttpContext.Session["FullSchoolNameP"] = dt.Rows[0]["FullSchoolNameP"].ToString();
                    HttpContext.Session["cent"] = dt.Rows[0]["cent"].ToString();
                    HttpContext.Session["status"] = dt.Rows[0]["Status"].ToString();
                    HttpContext.Session["schlNSQF"] = dt.Rows[0]["NSQF_flag"].ToString();
                    string dist1 = "010,020,030,045,050,100,110,120,160,165,175";
                    bool status = dist1.Split(',').Contains(dt.Rows[0]["DIST"].ToString());
                    if (status == true)
                    {
                        HttpContext.Session["TollFreeStatus"] = "2";
                        //  HttpContext.Session["TollFreeEmail"] = lm.TollFreeNumber = "Email Id: contact2@psebonline.in";
                        //  HttpContext.Session["TollFreeNumber"] = lm.TollFreeEmail = "Toll Free Help Line No. : 18004190690";
                    }
                    else
                    {
                        HttpContext.Session["TollFreeStatus"] = "1";
                        // HttpContext.Session["TollFreeEmail"] = lm.TollFreeNumber = "psebhelpdesk@gmail.com";
                        // HttpContext.Session["TollFreeNumber"] = lm.TollFreeEmail = "Toll Free Help Line No. : 18002700280";
                    }
                    //  return RedirectToAction("Choose_Session", "Login");
                    // return RedirectToAction("Index", "Home");
                    return RedirectToAction("ExamCalculateFeeAdmin", "Home");

                }
                else
                {
                    ViewData["result"] = "2";
                    return View();
                }
            }
            else
            {
                ViewData["result"] = "0";
                return View();
            }
        }
        public ActionResult Logout()
        {
            foreach (System.Collections.DictionaryEntry entry in HttpContext.Cache)
            {
                HttpContext.Cache.Remove((string)entry.Key);
            }
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            TempData.Clear();
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");
            // return RedirectToAction("Index", "Home");

        }


        [Route("login")]
        public ActionResult Index()
        {

            if (TempData["result"] != null)
            {
                ViewData["result"] = TempData["result"];
            }
            Session["LoginSession"] = null;
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            Session.Clear();
            TempData.Clear();
            Session.Abandon();
            Session.RemoveAll();
            try
            {

                ViewBag.SessionList = objCommon.GetSession().ToList();               
                return View();
            }
            catch (Exception ex)
            {
               // oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
                return View();
            }
        }

        [Route("login")]
        [HttpPost]
        public ActionResult Index(LoginModel lm)
        {
            try
            {
                LoginSession loginSession = AbstractLayer.SchoolDB.LoginSenior(lm); // passing Value to _schoolRepository.from model and Type 1 For regular   
                if (loginSession != null)
                {
                    loginSession.CurrentSession = lm.Session;
                    TempData["result"] = loginSession.LoginStatus;
                    Session["Session"] = lm.Session.ToString();

                    if (loginSession.LoginStatus == 1)
                    {
                        Session["LoginSession"] = loginSession;

                        DataSet ds = new AbstractLayer.RegistrationDB().CheckLogin(lm);
                        if (ds.Tables.Count > 0)
                        {
                            DataTable dt = ds.Tables[0];

                            DataTable dt1; // Check Staff
                            if (ds.Tables.Count > 1)
                            {
                                dt1 = ds.Tables[1];
                                HttpContext.Session["IsStaff"] = dt1.Rows[0]["IsStaff"].ToString();
                            }

                            if (dt.Rows[0]["Active"].ToString() != "")
                            {                              
                                HttpContext.Session["SchoolLogin"] = dt.Rows[0]["schl"].ToString();
                                HttpContext.Session["SchoolMobile"] = dt.Rows[0]["Mobile"].ToString();
                                HttpContext.Session["RoleType"] = dt.Rows[0]["RoleType"].ToString();
                                HttpContext.Session["SchlE"] = dt.Rows[0]["SchlE"].ToString();
                                HttpContext.Session["SchlP"] = dt.Rows[0]["SchlP"].ToString();
                                HttpContext.Session["IDNO"] = dt.Rows[0]["IDNO"].ToString();
                                HttpContext.Session["SCHL"] = dt.Rows[0]["schl"].ToString();
                                HttpContext.Session["STATION"] = dt.Rows[0]["STATIONE"].ToString();
                                HttpContext.Session["STATIONP"] = dt.Rows[0]["STATIONP"].ToString();
                                HttpContext.Session["SCHOOLDIST"] = dt.Rows[0]["DIST"].ToString();
                                HttpContext.Session["DISTE"] = dt.Rows[0]["DISTE"].ToString();
                                HttpContext.Session["DISTP"] = dt.Rows[0]["DISTP"].ToString();                              
                                HttpContext.Session["Class"] = dt.Rows[0]["Class"].ToString();
                                HttpContext.Session["SchType"] = dt.Rows[0]["Abbr"].ToString();
                                HttpContext.Session["FullSchoolNameE"] = dt.Rows[0]["FullSchoolNameE"].ToString();
                                HttpContext.Session["FullSchoolNameP"] = dt.Rows[0]["FullSchoolNameP"].ToString();
                                HttpContext.Session["cent"] = dt.Rows[0]["cent"].ToString();
                                HttpContext.Session["status"] = dt.Rows[0]["Status"].ToString();
                                HttpContext.Session["schlInfoUpdFlag"] = dt.Rows[0]["schlInfoUpdFlag"].ToString();
                                HttpContext.Session["Schtype4form"] = dt.Rows[0]["Schtype4form"].ToString();
                                HttpContext.Session["12Supt"] = dt.Rows[0]["12Supt"].ToString();
                                HttpContext.Session["10Supt"] = dt.Rows[0]["10Supt"].ToString();

                                HttpContext.Session["EXAMCENTSCHLN"] = dt.Rows[0]["cent"].ToString();
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    if (dt.Rows[i]["cent"].ToString().Length == 5)
                                    {
                                        HttpContext.Session["EXAMCENTSCHLN"] = dt.Rows[i]["cent"].ToString();
                                    }
                                }



                                string dist1 = "010,020,030,045,050,100,110,120,160,165,175";
                                bool status = dist1.Split(',').Contains(dt.Rows[0]["DIST"].ToString());
                                if (status == true)
                                {
                                    HttpContext.Session["TollFreeStatus"] = "2";
                                }
                                else
                                {
                                    HttpContext.Session["TollFreeStatus"] = "1";
                                }
                              
                            }
                            else
                            {
                                ViewData["result"] = "2";                             
                            }

                        }

                        return RedirectToAction("Index", "Home");
                    }
                }
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {               
                TempData["result"] = "Error : " + ex.Message;
                return RedirectToAction("Index", "Login");
            }
        }


      //  [Route("login")]
      //  [HttpPost]
      //  public ActionResult Index_Old(LoginModel lm)
      //  {
      //      try
      //      {
      //          if (lm.Session == null) { ViewData["result"] = "0"; }
      //          else
      //          {
      //              HttpContext.Session["Session"] = lm.Session.ToString();
      //          }

      //          ViewBag.SessionList = objCommon.GetSession().ToList();
      //          AbstractLayer.RegistrationDB objDB = new AbstractLayer.RegistrationDB();
      //          DataSet ds = objDB.CheckLogin(lm); // passing Value to SchoolDB from model and Type 1 For regular

      //          if (ds == null)
      //          {
      //              ViewData["result"] = "-1";
      //              return View();
      //          }


      //          if (ds.Tables.Count > 0)
      //          {
      //              DataTable dt = ds.Tables[0];

      //              DataTable dt1; // Check Staff
      //              if (ds.Tables.Count > 1)
      //              {
      //                  dt1 = ds.Tables[1];
      //                  HttpContext.Session["IsStaff"] = dt1.Rows[0]["IsStaff"].ToString();
      //              }

      //              if (dt.Rows[0]["Active"].ToString() != "")
      //              {
      //                 // oErrorLog.WriteErrorLog(dt.Rows[0]["schl"].ToString(), Path.GetFileName(Request.Path));
      //                  ViewData["result"] = null;
      //                  HttpContext.Session["SchoolLogin"] = dt.Rows[0]["schl"].ToString();
      //                  HttpContext.Session["SchoolMobile"] = dt.Rows[0]["Mobile"].ToString();
      //                  HttpContext.Session["RoleType"] = dt.Rows[0]["RoleType"].ToString();
      //                  HttpContext.Session["SchlE"] = dt.Rows[0]["SchlE"].ToString();
      //                  HttpContext.Session["SchlP"] = dt.Rows[0]["SchlP"].ToString();
      //                  HttpContext.Session["IDNO"] = dt.Rows[0]["IDNO"].ToString();
      //                  HttpContext.Session["SCHL"] = dt.Rows[0]["schl"].ToString();
      //                  HttpContext.Session["STATION"] = dt.Rows[0]["STATIONE"].ToString();
      //                  HttpContext.Session["STATIONP"] = dt.Rows[0]["STATIONP"].ToString();
      //                  HttpContext.Session["SCHOOLDIST"] = dt.Rows[0]["DIST"].ToString();
      //                  HttpContext.Session["DISTE"] = dt.Rows[0]["DISTE"].ToString();
      //                  HttpContext.Session["DISTP"] = dt.Rows[0]["DISTP"].ToString();
      //                  HttpContext.Session["Session"] = lm.Session;
      //                  HttpContext.Session["Class"] = dt.Rows[0]["Class"].ToString();
      //                  HttpContext.Session["SchType"] = dt.Rows[0]["Abbr"].ToString();
      //                  HttpContext.Session["FullSchoolNameE"] = dt.Rows[0]["FullSchoolNameE"].ToString();
      //                  HttpContext.Session["FullSchoolNameP"] = dt.Rows[0]["FullSchoolNameP"].ToString();
      //                  HttpContext.Session["cent"] = dt.Rows[0]["cent"].ToString();
      //                  HttpContext.Session["status"] = dt.Rows[0]["Status"].ToString();
						//HttpContext.Session["schlInfoUpdFlag"] = dt.Rows[0]["schlInfoUpdFlag"].ToString();
      //                  HttpContext.Session["Schtype4form"] = dt.Rows[0]["Schtype4form"].ToString();               
      //                  HttpContext.Session["12Supt"] = dt.Rows[0]["12Supt"].ToString();
      //                  HttpContext.Session["10Supt"] = dt.Rows[0]["10Supt"].ToString();
                                

      //                  string dist1 = "010,020,030,045,050,100,110,120,160,165,175";
      //                  bool status = dist1.Split(',').Contains(dt.Rows[0]["DIST"].ToString());
      //                  if (status == true)
      //                  {
      //                      HttpContext.Session["TollFreeStatus"] = "2";
      //                  }
      //                  else
      //                  {
      //                      HttpContext.Session["TollFreeStatus"] = "1";
      //                  }

      //                  LoginSession loginSession = AbstractLayer.SchoolDB.LoginSenior(lm); // passing Value to _schoolRepository.from model and Type 1 For regular   
      //                  if (loginSession != null)
      //                  {
      //                      loginSession.CurrentSession = lm.Session;
      //                      TempData["result"] = loginSession.LoginStatus;
      //                      if (loginSession.LoginStatus == 1)
      //                      {
      //                          Session["LoginSession"] = loginSession;
      //                          return RedirectToAction("Index", "Home");
      //                      }
      //                  }
      //                  return RedirectToAction("Index", "Login");
      //              }
      //              else
      //              {
      //                  ViewData["result"] = "2";
      //                  return View();
      //              }

      //          }
      //          else
      //          {
      //              ViewData["result"] = "0";
      //              return View();
      //          }
      //      }
      //      catch (Exception ex)
      //      {
      //          oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
      //          ViewData["result"] = "ERR";
      //          ViewData["resultMsg"] = ex.Message;
      //          return View();
      //      }
      //  }

        [HttpGet]
        public ActionResult Choose_Session()
        {
            return View();
        }
        

        public ActionResult ForgotPassword()
        {
            ViewBag.SubmitValue = "Send";
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(LoginModel lm)
        {
            try
            {
                ViewBag.SubmitValue = "Send";
                string sid = lm.SchoolId;
                AbstractLayer.DBClass dbclass = new AbstractLayer.DBClass();

                DataSet ds = new DataSet();
                ds = dbclass.SearchEmailID(sid);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        string IsApporved = ViewBag.IsApporved = ds.Tables[0].Rows[0]["approved"].ToString();
                        string SchlStatus = ViewBag.SchlStatus = ds.Tables[0].Rows[0]["Status"].ToString();
                       
                        if (IsApporved == "1" && SchlStatus.ToUpper() == "DONE")
                        {

                            string SchoolNameWithCode = ds.Tables[0].Rows[0]["SCHLE"].ToString() + "(" + sid + ")";
                            string password = ds.Tables[0].Rows[0]["PASSWORD"].ToString();
                            string to = ds.Tables[0].Rows[0]["EMAILID"].ToString();
                            string MOBILENO = ds.Tables[0].Rows[0]["Mobile"].ToString();



                            if (!string.IsNullOrEmpty(MOBILENO) && MOBILENO.Length == 10 )
                            {
                                  string Sms = "Your Login details are School Code: " + sid + " and Password: " + password + ". Click to Login Here https://registration2021.pseb.ac.in/Login. Regards PSEB";
                                //string Sms = "Your Login details are School Code: " + sid + " and Password: " + password + ". Click to Login Here https://registration2021.pseb.ac.in/Login. Regards PSEB";
                                string getSms = new AbstractLayer.DBClass().gosms(MOBILENO, Sms);
                                if (getSms.ToLower().Contains("success"))
                                {
                                    ViewBag.SubmitValue = "Resend";
                                    ViewData["result"] = "11";
                                    ViewBag.Message = "SMS Has Been Successfully Send to your Registered Mobile Number : " + MOBILENO + "";
                                    ModelState.Clear();
                                }
                            }


                            if (!string.IsNullOrEmpty(to))
                            {

                                string body = "<table width=" + 600 + " cellpadding=" + 4 + " cellspacing=" + 4 + " border=" + 0 + "><tr><td><b>Dear " + SchoolNameWithCode + "</b>,</td></tr><tr><td height=" + 30 + ">As per your request Dated <b>" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "</b> Regarding Forget Password</td></tr><tr><td><b>Your Login Details are given Below:-</b><br /><b>School Login Id (School Code) :</b> " + sid + "<br /><b>Password :</b> " + password + "<br /></td></tr><tr><td height=" + 30 + "><b>Click Here To Login</b> <a href=https://www.registration.pseb.ac.in target = _blank>www.registration.pseb.ac.in</a></td></tr><tr><td><b>Note:</b> Please Read Instruction Carefully Before filling the Online Form .</td></tr><tr><td>This is a system generated e-mail and please do not reply. Add <a target=_blank href=mailto:noreply@psebonline.in>noreply@psebonline.in</a> to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.</td></tr><tr><td><b><i>Regards</b><i>,<br /> Tech Team, <br />Punjab School Education Board<br /><tr><td><b>Contact Us</b><br><b>Email Id:</b> <a href=mailto:psebhelpdesk@gmail.com target=_blank>psebhelpdesk@gmail.com</a><br><b>Toll Free Help Line No. :</b> 18002700280<br>DISTRICTS:- BARNALA, FATEHGARH SAHIB, GURDASPUR, HOSHIARPUR, JALANDHAR, KAPURTHALA, SHRI MUKTSAR SAHIB, S.B.S. NAGAR, PATHANKOT, PATIALA, SANGRUR, CHANDIGARH &amp; OTHER STATES<br><br><b>Email Id:</b> <a href=mailto:Contact2@psebonline.in target=_blank>contact2@psebonline.in</a><br><b>Toll Free Help Line No. :</b> 18004190690<br>DISTRICTS:- AMRITSAR, BATHINDA, FARIDKOT, FAZILKA, FEROZEPUR, LUDHIANA, MANSA, MOGA, ROOP NAGAR, S.A.S NAGAR,TARN TARAN<br></td></tr>";
                                string subject = "PSEB-Forgot Password Notification";
                                bool result = dbclass.mail(subject, body, to);
                                if (result == true)
                                {
                                    ViewBag.SubmitValue = "Resend";
                                    ViewData["result"] = "1";
                                    //ViewBag.Message = "Thank You, Your Password Send To Your "+ to + " EmailId Successfully....";
                                    ViewBag.Message = "Password Has Been Successfully Send to your Registered Email Id  " + to + "";
                                    ModelState.Clear();
                                }
                                else
                                {
                                    ViewData["result"] = "0";
                                    ViewBag.Message = "Password Not Sent....";
                                }
                            }

                        }
                        else
                        {
                           
                            ViewData["result"] = "5";
                            ViewBag.Message = "Incorrect School Code ....";
                        }
                    }
                    else
                    {
                        ViewData["result"] = "-1";
                        ViewBag.Message = "Incorrect School Code ....";
                    }
                }
                else
                {
                    ViewData["result"] = "-1";
                    ViewBag.Message = "Incorrect School Code ....";
                }
            }
            catch (Exception ex)
            {
                oErrorLog.WriteErrorLog(ex.ToString(), Path.GetFileName(Request.Path));
            }
            return View();
        }

    }
}