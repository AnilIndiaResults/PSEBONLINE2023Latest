using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PSEBONLINE.Filters;
using PSEBONLINE.Models;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using PSEBONLINE.Repository;
using System.IO;
using System.Data.Entity;

namespace PSEBONLINE.Controllers
{
    public class SelfDeclarationController : Controller
    {
        private readonly DBContext _context = new DBContext();
        private readonly ISelfDeclarationRepository _selfDeclarationRepository;

        public SelfDeclarationController(ISelfDeclarationRepository selfDeclarationRepository)
        {
            _selfDeclarationRepository = selfDeclarationRepository;
        }
        // GET: SelfDeclaration

        
        #region  Login      
        [Route("SelfDeclaration")]
        [Route("SelfDeclaration/login")]
        public ActionResult Index()
        {
            List<SelectListItem> rpList = AbstractLayer.DBClass.GetRPType().ToList();
            ViewBag.MyRPList = rpList.ToList();
            SelfDeclarationLoginModel selfDeclarationLoginModel = new SelfDeclarationLoginModel();
 
            if (TempData["result"] != null)
            {
                ViewData["result"] = TempData["result"];
            }            
            return View(selfDeclarationLoginModel);
        }


        [Route("SelfDeclaration")]
        [Route("SelfDeclaration/login")]
        [HttpPost]
        public async Task<ActionResult> Index(SelfDeclarationLoginModel lm)
        {
            try
            {
                List<SelectListItem> rpList = AbstractLayer.DBClass.GetRPType().ToList();
                ViewBag.MyRPList = rpList.ToList();
                SelfDeclarationLoginSession loginSession = new SelfDeclarationLoginSession();
                if (lm.RP == "A")
                {
                    loginSession = await _selfDeclarationRepository.CheckLoginAdditionSubject(lm); // passing Value to _schoolRepository.from model and Type 1 For regular              
                }
                else
                {
                    loginSession = await _selfDeclarationRepository.CheckLogin(lm); // passing Value to _schoolRepository.from model and Type 1 For regular              
                }
                
                if (loginSession != null)
                {
                    loginSession.CurrentSession = "2023-2024";
                    TempData["result"] = loginSession.LoginStatus.ToString();

                    if (loginSession.LoginStatus == 1)
                    {
                        Session["SelfDeclarationLoginSession"] = loginSession;                       
                        return RedirectToAction("SelfDeclarationCandidateDetails", "SelfDeclaration");
                    }
                }
                return RedirectToAction("Index", "SelfDeclaration");
            }
            catch (Exception ex)
            {               
                TempData["result"] = "Error : " + ex.Message;
                return RedirectToAction("Index", "SelfDeclaration");
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
            return RedirectToAction("Index", "SelfDeclaration");
        }

        #endregion School Login 

        [SelfDeclarationLoginCheckFilter]
        public async Task<ActionResult> SelfDeclarationCandidateDetails(SelfDeclarations selfDeclaration)
        {
            SelfDeclarationLoginSession selfDeclarationLoginSession = (SelfDeclarationLoginSession)Session["SelfDeclarationLoginSession"];

            if (selfDeclarationLoginSession.CAT.ToLower() == "Additional".ToLower())
            {
                selfDeclaration = await _selfDeclarationRepository.GetDataByLoginDetailsAdditionSubject(selfDeclarationLoginSession); // passing Value to _schoolRepository.from model and Type 1 For regular              
            }
            else
            {
                selfDeclaration = await _selfDeclarationRepository.GetDataByLoginDetails(selfDeclarationLoginSession); // passing Value to _schoolRepository.from model and Type 1 For regular              
            }
            

            if (selfDeclaration == null)
            {

                selfDeclaration = new SelfDeclarations()
                {
                    CAT = selfDeclarationLoginSession.CAT,
                    Class = selfDeclarationLoginSession.CLASS,
                    ApplyYear = selfDeclarationLoginSession.YEAR,
                    ApplyMonth = selfDeclarationLoginSession.MONTH,
                    RP = selfDeclarationLoginSession.RP,
                    Roll = selfDeclarationLoginSession.ROLL,
                    RegNo = selfDeclarationLoginSession.REGNO,

                    Name = selfDeclarationLoginSession.NAME,
                    FName = selfDeclarationLoginSession.FNAME,
                    MName = selfDeclarationLoginSession.MNAME,
                    Result = selfDeclarationLoginSession.RESULT,
                    Resultdtl = selfDeclarationLoginSession.RESULTDTL
                };
           }
           
            if (TempData["resultIns"] != null)
            {
                ViewData["resultIns"] = TempData["resultIns"];
            }
            return View(selfDeclaration);
        }


        [SelfDeclarationLoginCheckFilter]
        [HttpPost]
        public async Task<ActionResult> SelfDeclarationCandidateDetails(SelfDeclarations selfDeclaration, string cmd ,FormCollection fc, HttpPostedFileBase SelfDeclarationDocument)
        {
            SelfDeclarationLoginSession selfDeclarationLoginSession = (SelfDeclarationLoginSession)Session["SelfDeclarationLoginSession"];

            try
            {
                // Save file
                string filename = "";
                string FilepathExist = "", path = "";
                if (SelfDeclarationDocument != null)
                {
                    if (selfDeclarationLoginSession.CAT.ToLower() == "Additional".ToLower())
                    {
                        string ext = Path.GetExtension(SelfDeclarationDocument.FileName);
                        filename = selfDeclaration.Roll + "_SignedSelfDeclaration" + ext;
                        path = Path.Combine(Server.MapPath("~/Upload/Upload2023/SelfDeclarationAdditionSubject"), filename);
                        FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/SelfDeclarationAdditionSubject"));
                        selfDeclaration.SelfDeclarationDocument = "Upload2023/SelfDeclarationAdditionSubject/" + filename;
                    }
                    else
                    {
                        string ext = Path.GetExtension(SelfDeclarationDocument.FileName);
                        filename = selfDeclaration.Roll + "_SignedSelfDeclaration" + ext;
                        path = Path.Combine(Server.MapPath("~/Upload/Upload2023/SelfDeclaration"), filename);
                        FilepathExist = Path.Combine(Server.MapPath("~/Upload/Upload2023/SelfDeclaration"));
                        selfDeclaration.SelfDeclarationDocument = "Upload2023/SelfDeclaration/" + filename;
                    }
                }



                if (cmd == null)
                {
                    if (selfDeclaration.SelfDeclarationId > 0)
                    {
                        using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                        {
                            try
                            {
                                SelfDeclarations selfDeclarations1 = _context.SelfDeclarations.Find(selfDeclaration.SelfDeclarationId);
                                selfDeclarations1.IsFinalSubmit = 1;
                                selfDeclarations1.IsFinalSubmitOn = DateTime.Now;
                                _context.Entry(selfDeclarations1).State = EntityState.Modified;
                                _context.SaveChanges();
                                TempData["resultIns"] = "F";


                                if (SelfDeclarationDocument != null)
                                {
                                    if (!Directory.Exists(FilepathExist))
                                    {
                                        Directory.CreateDirectory(FilepathExist);
                                    }


                                    SelfDeclarationDocument.SaveAs(path);
                                }

                                transaction.Commit();//transaction commit
                            }
                            catch (Exception ex1)
                            {

                                transaction.Rollback();
                            }
                        }
                    }
                }
                else
                {

                    if (cmd.ToLower().Contains("submit") && selfDeclaration.SelfDeclarationId == 0)
                    {

                        SelfDeclarations selfDeclaration1 = new SelfDeclarations()
                        {
                            CAT = selfDeclarationLoginSession.CAT,
                            Class = selfDeclaration.Class,
                            ApplyYear = selfDeclarationLoginSession.YEAR,
                            ApplyMonth = selfDeclarationLoginSession.MONTH,
                            RP = selfDeclaration.RP,
                            Roll = selfDeclaration.Roll,
                            RegNo = selfDeclaration.RegNo,

                            Name = selfDeclaration.Name,
                            FName = selfDeclaration.FName,
                            MName = selfDeclaration.MName,
                            Result = selfDeclaration.Result,
                            Resultdtl = selfDeclaration.Resultdtl,
                            Remarks = selfDeclaration.Remarks,
                            CreatedBy = selfDeclaration.Roll,
                            CreatedOn = DateTime.Now,
                            ModifyOn = DateTime.Now,
                            IsActive = true,
                            IsFinalSubmit = 0,
                            SelfDeclarationDocument = selfDeclaration.SelfDeclarationDocument
                        };




                        _context.SelfDeclarations.Add(selfDeclaration1);
                        int insertedRecords = await _context.SaveChangesAsync();
                        // _context?.Dispose();

                        if (insertedRecords > 0)
                        {
                            TempData["resultIns"] = "S";

                            if (SelfDeclarationDocument != null)
                            {
                                if (!Directory.Exists(FilepathExist))
                                {
                                    Directory.CreateDirectory(FilepathExist);
                                }
                                SelfDeclarationDocument.SaveAs(path);
                            }
                        }
                    }

                    else if (cmd.ToLower().Contains("update") && selfDeclaration.SelfDeclarationId > 0)
                    {
                        using (DbContextTransaction transaction = _context.Database.BeginTransaction())
                        {
                            try
                            {
                                SelfDeclarations selfDeclarations1 = _context.SelfDeclarations.Find(selfDeclaration.SelfDeclarationId);
                                selfDeclarations1.IsFinalSubmit = 0;
                                selfDeclarations1.Remarks = selfDeclaration.Remarks;
                                selfDeclarations1.ModifyBy = selfDeclaration.Roll;
                                selfDeclarations1.ModifyOn = DateTime.Now;
                                selfDeclarations1.IsActive = true;
                                _context.Entry(selfDeclarations1).State = EntityState.Modified;
                                _context.SaveChanges();
                                TempData["resultIns"] = "M";


                                if (SelfDeclarationDocument != null)
                                {
                                    if (!Directory.Exists(FilepathExist))
                                    {
                                        Directory.CreateDirectory(FilepathExist);
                                    }
                                    SelfDeclarationDocument.SaveAs(path);
                                }

                                transaction.Commit();//transaction commit
                            }
                            catch (Exception ex1)
                            {

                                transaction.Rollback();
                            }
                        }

                    }

                }
               

            }
            catch (Exception ex)
            {
                TempData["resultIns"] = "Error : " + ex.Message;
            }
            return RedirectToAction("SelfDeclarationCandidateDetails", "SelfDeclaration");
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