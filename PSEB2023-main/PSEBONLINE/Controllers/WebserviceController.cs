using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using PSEBONLINE.Models;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace PSEBONLINE.Controllers
{
    public class WebserviceController : Controller
    {
        AbstractLayer.WebSerDB objDB = new AbstractLayer.WebSerDB();
        AbstractLayer.DBClass objCommon = new AbstractLayer.DBClass();
        epunjabschool.service_pseb abc = new epunjabschool.service_pseb();
        // GET: Webservice
        public ActionResult Index()
        {
            string schlid = "";
            if (Session["SCHL"] != null)
            {
                schlid = Session["SCHL"].ToString();
            }
            else
            {
                return View();
            }
            string udic = objDB.GetUdiCode(schlid);
            webSerModel wm = new webSerModel();
            wm.UdiseCode = udic;

            if (udic != "" || udic != null)
            {
                wm.StoreAllData = objDB.GetudiCodeDetails(udic);
                if (wm.StoreAllData == null || wm.StoreAllData.Tables[0].Rows.Count == 0)
                {
                    ViewBag.Message = "Record Not Found";
                    ViewBag.TotalCount = 0;
                    return View(wm);
                }
                else
                {
                    ViewBag.TotalCount = wm.StoreAllData.Tables[0].Rows.Count;
                    return View(wm);
                }
            }

            return View(wm);
        }


        [HttpPost]
        public ActionResult Index(FormCollection frc, webSerModel wm, string cmd, string importBy, string SearchString, string chkImportid)
        {
            
            string EpunSearch = "";
            string aadharnoSearch = "";
            string res = null;
            string CurrentSchl = Session["SCHL"].ToString();
            if (cmd == "Import Records")
            {
                try
                {
                    epunjabschool.service_pseb abc = new epunjabschool.service_pseb();
                    if (importBy != "")
                    {
                        ViewBag.SelectedItem = importBy;
                        int SelValueSch = Convert.ToInt32(importBy.ToString());
                        if (SelValueSch == 1)
                        {
                            EpunSearch = SearchString.ToString().Trim();
                            try
                            {
                                string strParameter = Encrypt("PSEB4488~08059585PSEB~" + EpunSearch);
                                string JsonString = Decrypt(abc.get9thClassStudentDetails_ByStudents(strParameter));
                                DataTable dt1 = new DataTable();
                                dt1 = JsonStringToDataTable(JsonString);
                                string epunid = dt1.Rows[0]["studentdID"].ToString();
                                string aadhar = dt1.Rows[0]["studentUID"].ToString();
                                string Name = dt1.Rows[0]["studentName"].ToString();
                                string Pname = objCommon.getPunjabiName(Name);
                                string fname = dt1.Rows[0]["fatherName"].ToString();
                                string Pfname = objCommon.getPunjabiName(fname);
                                string mname = dt1.Rows[0]["motherName"].ToString();
                                string Pmname = objCommon.getPunjabiName(mname);
                                string dob = "";
                                string inputString = string.Format("{0:d/MM/yyyy}", dt1.Rows[0]["dob"]).ToString();
                                DateTime dDate;
                                if (DateTime.TryParse(inputString, out dDate))
                                {
                                    dob = Convert.ToDateTime(dt1.Rows[0]["dob"]).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    dob = "";
                                    //Console.WriteLine("Invalid"); // <-- Control flow goes here
                                }
                                //string dob = Convert.ToDateTime(dt1.Rows[0]["dob"]).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                string sex = (dt1.Rows[0]["genderID"].ToString() == "1") ? "MALE" : "FEMALE"; // dt1.Rows[0]["genderID"].ToString();
                                string caste = (dt1.Rows[0]["casteCategoryCode"].ToString() == "1") ? "SC" : (dt1.Rows[0]["casteCategoryCode"].ToString() == "2") ? "ST" : (dt1.Rows[0]["casteCategoryCode"].ToString() == "3") ? "OBC" : (dt1.Rows[0]["casteCategoryCode"].ToString() == "4") ? "GENERAL" : "GENERAL";
                                string reli = (dt1.Rows[0]["religionCode"].ToString() == "1") ? "HINDU" : (dt1.Rows[0]["religionCode"].ToString() == "2") ? "MUSLIM" : (dt1.Rows[0]["religionCode"].ToString() == "3") ? "CHRISTIAN" : (dt1.Rows[0]["religionCode"].ToString() == "4") ? "SIKH" : "OTHERS";
                                string DA = (dt1.Rows[0]["disabilityType"].ToString() == "1") ? "Blind/Visually Impaired" : (dt1.Rows[0]["disabilityType"].ToString() == "2") ? "Blind/Visually Impaired" : (dt1.Rows[0]["disabilityType"].ToString() == "3") ? "Deaf & Dumb/Hearing Impaired" : (dt1.Rows[0]["disabilityType"].ToString() == "4") ? "Deaf & Dumb/Hearing Impaired" : (dt1.Rows[0]["disabilityType"].ToString() == "11") ? "Physically Handicapped" : "OTHERS";//dt1.Rows[0]["disabilityType"].ToString();
                                string mob = dt1.Rows[0]["contactNo"].ToString();
                                string admno = dt1.Rows[0]["admissionNo"].ToString();
                                string Address = dt1.Rows[0]["village"].ToString() + ',' + dt1.Rows[0]["gramPanchayat"].ToString();
                                string pincode = dt1.Rows[0]["pinCode"].ToString();
                                int OutStatus = 0;
                                res = objDB.Import_All_N1_Data(out OutStatus,CurrentSchl, epunid, aadhar, Name, Pname, fname, Pfname, mname, Pmname, dob, sex, caste, reli, DA, mob, admno, Address, pincode);
                                ViewData["OutStatus"] = OutStatus;
                            }
                            catch (Exception)
                            {
                                TempData["result"] = 5;
                                return View(wm);
                                throw;
                            }
                            if (res == "0" || res == null)
                            {
                                //--------------Not saved
                                TempData["result"] = 0;
                                return View(wm);
                            }
                            if (res == "-1")
                            {
                                //-----alredy exist
                                TempData["result"] = -1;
                                return View(wm);
                            }
                            if (res == "-3")
                            {
                                //-----alredy exist
                                TempData["result"] = -3;
                                return View(wm);
                            }
                            if (res == "3")
                            {
                                //-----alredy exist
                                TempData["TotImported"] = 1;
                                TempData["result"] = 1;
                                return View(wm);
                            }
                            else
                            {
                                TempData["result"] = 1;
                                return View(wm);
                            }

                        }
                        if (SelValueSch == 2)
                        {
                            aadharnoSearch = SearchString.ToString().Trim();
                            try
                            {
                                string strParameter = Encrypt("PSEB4488~08059585PSEB~" + aadharnoSearch);
                                string JsonString = Decrypt(abc.get9thClassStudentDetails_ByUID(strParameter));
                                DataTable dt1 = new DataTable();
                                dt1 = JsonStringToDataTable(JsonString);
                                string epunid = dt1.Rows[0]["studentdID"].ToString();
                                string aadhar = dt1.Rows[0]["studentUID"].ToString();
                                string Name = dt1.Rows[0]["studentName"].ToString();
                                string Pname = objCommon.getPunjabiName(Name);

                                string fname = dt1.Rows[0]["fatherName"].ToString();
                                string Pfname = objCommon.getPunjabiName(fname);

                                string mname = dt1.Rows[0]["motherName"].ToString();
                                string Pmname = objCommon.getPunjabiName(mname);
                                string dob = "";
                                string inputString = string.Format("{0:d/MM/yyyy}", dt1.Rows[0]["dob"]).ToString();
                                DateTime dDate;
                                if (DateTime.TryParse(inputString, out dDate))
                                {
                                    dob = Convert.ToDateTime(dt1.Rows[0]["dob"]).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    dob = "";
                                    //Console.WriteLine("Invalid"); // <-- Control flow goes here
                                }

                                //string dob = Convert.ToDateTime(dt1.Rows[0]["dob"]).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                string sex = (dt1.Rows[0]["genderID"].ToString() == "1") ? "MALE" : "FEMALE"; // dt1.Rows[0]["genderID"].ToString();
                                string caste = (dt1.Rows[0]["casteCategoryCode"].ToString() == "1") ? "SC" : (dt1.Rows[0]["casteCategoryCode"].ToString() == "2") ? "ST" : (dt1.Rows[0]["casteCategoryCode"].ToString() == "3") ? "OBC" : (dt1.Rows[0]["casteCategoryCode"].ToString() == "4") ? "GENERAL" : "GENERAL";
                                string reli = (dt1.Rows[0]["religionCode"].ToString() == "1") ? "HINDU" : (dt1.Rows[0]["religionCode"].ToString() == "2") ? "MUSLIM" : (dt1.Rows[0]["religionCode"].ToString() == "3") ? "CHRISTIAN" : (dt1.Rows[0]["religionCode"].ToString() == "4") ? "SIKH" : "OTHERS";
                                string DA = (dt1.Rows[0]["disabilityType"].ToString() == "1") ? "Blind/Visually Impaired" : (dt1.Rows[0]["disabilityType"].ToString() == "2") ? "Blind/Visually Impaired" : (dt1.Rows[0]["disabilityType"].ToString() == "3") ? "Deaf & Dumb/Hearing Impaired" : (dt1.Rows[0]["disabilityType"].ToString() == "4") ? "Deaf & Dumb/Hearing Impaired" : (dt1.Rows[0]["disabilityType"].ToString() == "11") ? "Physically Handicapped" : "OTHERS";//dt1.Rows[0]["disabilityType"].ToString();
                                string mob = dt1.Rows[0]["contactNo"].ToString();
                                string admno = dt1.Rows[0]["admissionNo"].ToString();
                                string Address = dt1.Rows[0]["village"].ToString() + ',' + dt1.Rows[0]["gramPanchayat"].ToString();
                                string pincode = dt1.Rows[0]["pinCode"].ToString();
                                int OutStatus = 0;
                                res = objDB.Import_All_N1_Data(out OutStatus, CurrentSchl, epunid, aadhar, Name, Pname, fname, Pfname, mname, Pmname, dob, sex, caste, reli, DA, mob, admno, Address, pincode);
                                ViewData["OutStatus"] = OutStatus;
                            }
                            catch (Exception)
                            {
                                TempData["result"] = 5;
                                return View(wm);
                                throw;
                            }
                            if (res == "0" || res == null)
                            {
                                //--------------Not saved
                                TempData["result"] = 0;
                                return View(wm);
                            }
                            if (res == "-1")
                            {
                                //-----alredy exist
                                TempData["result"] = -1;
                                return View(wm);
                            }
                            if (res == "-3")
                            {
                                //-----alredy exist
                                TempData["result"] = -3;
                                return View(wm);
                            }
                            if (res == "3")
                            {
                                //-----alredy exist
                                TempData["TotImported"] = 1;
                                TempData["result"] = 1;
                                return View(wm);
                            }
                            else
                            {
                                TempData["result"] = 1;
                                return View(wm);
                            }

                        }
                    }


                }
                catch (Exception)
                {

                    throw;
                }

            }
            if (cmd == "Submit")
            {
                try
                {
                    string UdiseCode = frc["UdiseCode"].ToString(); //03010307002;
                    string strParameter = Encrypt("PSEB4488~08059585PSEB~" + UdiseCode);
                    epunjabschool.service_pseb abc = new epunjabschool.service_pseb();
                    string JsonString = Decrypt(abc.get9thClassAllStudents(strParameter));
                    DataTable dt = new DataTable();
                    dt = JsonStringToDataTable(JsonString);
                    //for(int i=0; dt.Rows.Count>i; i++)
                    //{

                    //}

                    string result = objDB.Ins_Temp_Data(wm, frc, dt);
                    if (result == "0" || result == null)
                    {
                        //--------------Not saved
                        TempData["result"] = 0;
                    }
                    else if (result == "-1")
                    {
                        //-----alredy exist
                        TempData["result"] = -1;
                    }
                    else
                    {
                        TempData["result"] = "Inserted";
                    }

                    //ViewBag.TotalCount = dt.Rows.Count;
                    //wm.StoreAllDataTable = dt;
                    wm.StoreAllData = objDB.GetudiCodeDetails(UdiseCode);
                    if (wm.StoreAllData == null || wm.StoreAllData.Tables[0].Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(wm);
                    }
                    else
                    {
                        ViewBag.TotalCount = wm.StoreAllData.Tables[0].Rows.Count;
                        return View(wm);
                    }
                }
                catch (Exception)
                {
                    TempData["result"] = 5;
                    return View(wm);
                    throw;
                }


            }
            //else
            if (cmd == "Import Selected Record")
            {
                //--------------Start Import Selected Data here----------------//
                string importToSchl = Session["SCHL"].ToString();
                if (importToSchl.ToString() == null || importToSchl.ToString() == "0")
                {
                    TempData["result"] = "-2";
                    return RedirectToAction("Index", "WebService");
                }
                // string CurrentSchl = Session["SCHL"].ToString();

                string collectId = frc["ChkCNinthClass"];
                if (collectId == null || collectId == "")
                {
                    return View(wm);
                }
                int cnt = collectId.Count(x => x == ',');
                //TempData["TotImported"] = cnt + 1;
                TempData["TotImported"] = 0;
                int recr = 0;

                DataTable dt = new DataTable();
                //dt = objDB.Select_All_Pass_Data(importToSchl, collectId);
                if (chkImportid != "")
                {
                    // dt = objDB.Select_All_Pass_Data_For_N(importToSchl, chkImportid, Session1);
                    //string strParameter = Encrypt("PSEB4488~08059585PSEB~" + collectId);
                    //string res = null;
                    epunjabschool.service_pseb abc = new epunjabschool.service_pseb();
                    string[] words = collectId.Split(',');
                    foreach (string word in words)
                    {
                        try
                        {
                            string strParameter = Encrypt("PSEB4488~08059585PSEB~" + word);
                            string JsonString = Decrypt(abc.get9thClassStudentDetails_ByStudents(strParameter));
                            DataTable dt1 = new DataTable();
                            dt1 = JsonStringToDataTable(JsonString);
                            if (dt1 != null)
                            {

                                string epunid = dt1.Rows[0]["studentdID"].ToString();
                                string aadhar = dt1.Rows[0]["studentUID"].ToString();
                                string Name = dt1.Rows[0]["studentName"].ToString();
                                string Pname = objCommon.getPunjabiName(Name);

                                string fname = dt1.Rows[0]["fatherName"].ToString();
                                string Pfname = objCommon.getPunjabiName(fname);

                                string mname = dt1.Rows[0]["motherName"].ToString();
                                string Pmname = objCommon.getPunjabiName(mname);


                                string dob = "";
                                string inputString = string.Format("{0:d/MM/yyyy}", dt1.Rows[0]["dob"]).ToString();
                                DateTime dDate;
                                if (DateTime.TryParse(inputString, out dDate))
                                {
                                    dob = Convert.ToDateTime(dt1.Rows[0]["dob"]).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    dob = "";
                                    //Console.WriteLine("Invalid"); // <-- Control flow goes here
                                }



                                //string dob = Convert.ToDateTime(dt1.Rows[0]["dob"]).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                string sex = (dt1.Rows[0]["genderID"].ToString() == "1") ? "MALE" : "FEMALE"; // dt1.Rows[0]["genderID"].ToString();
                                string caste = (dt1.Rows[0]["casteCategoryCode"].ToString() == "1") ? "SC" : (dt1.Rows[0]["casteCategoryCode"].ToString() == "2") ? "ST" : (dt1.Rows[0]["casteCategoryCode"].ToString() == "3") ? "OBC" : (dt1.Rows[0]["casteCategoryCode"].ToString() == "4") ? "GENERAL" : "GENERAL";
                                string reli = (dt1.Rows[0]["religionCode"].ToString() == "1") ? "HINDU" : (dt1.Rows[0]["religionCode"].ToString() == "2") ? "MUSLIM" : (dt1.Rows[0]["religionCode"].ToString() == "3") ? "CHRISTIAN" : (dt1.Rows[0]["religionCode"].ToString() == "4") ? "SIKH" : "OTHERS";
                                string DA = (dt1.Rows[0]["disabilityType"].ToString() == "1") ? "Blind/Visually Impaired" : (dt1.Rows[0]["disabilityType"].ToString() == "2") ? "Blind/Visually Impaired" : (dt1.Rows[0]["disabilityType"].ToString() == "3") ? "Deaf & Dumb/Hearing Impaired" : (dt1.Rows[0]["disabilityType"].ToString() == "4") ? "Deaf & Dumb/Hearing Impaired" : (dt1.Rows[0]["disabilityType"].ToString() == "11") ? "Physically Handicapped" : "OTHERS";//dt1.Rows[0]["disabilityType"].ToString();
                                string mob = dt1.Rows[0]["contactNo"].ToString();
                                string admno = dt1.Rows[0]["admissionNo"].ToString();
                                string Address = dt1.Rows[0]["village"].ToString() + ',' + dt1.Rows[0]["gramPanchayat"].ToString();
                                string pincode = dt1.Rows[0]["pinCode"].ToString();
                                int OutStatus = 0;
                                res = objDB.Import_All_N1_Data(out OutStatus, CurrentSchl, epunid, aadhar, Name, Pname, fname, Pfname, mname, Pmname, dob, sex, caste, reli, DA, mob, admno, Address, pincode);
                                ViewData["OutStatus"] = OutStatus;
                                if (res != "-1")
                                {
                                    TempData["TotImported"] = recr + 1;
                                    recr = recr + 1;
                                }

                            }
                        }
                        catch (Exception)
                        {
                            TempData["result"] = 5;
                            return View(wm);
                            throw;
                        }

                    }
                    if (res == "0" || res == null)
                    {
                        //--------------Not saved
                        TempData["result"] = 0;
                    }
                    if (res == "-1")
                    {
                        //-----alredy exist
                        TempData["result"] = -1;
                    }
                    if (res == "-3")
                    {
                        //-----alredy exist
                        TempData["result"] = -3;
                    }

                    else
                    {
                        TempData["result"] = 1;
                    }

                }

                //return View(wm);
                //---------------End Import Selected Data here----------------//
            }
            return View(wm);
        }


        public DataTable JsonStringToDataTable(string jsonString)
        {
            DataTable dt1 = (DataTable)JsonConvert.DeserializeObject(jsonString, (typeof(DataTable)));
            return dt1;
            //DataTable dt = new DataTable();
            //string[] jsonStringArray = Regex.Split(jsonString.Replace("[", "").Replace("]", ""), "},{");
            //List<string> ColumnsName = new List<string>();
            //foreach (string jSA in jsonStringArray)
            //{
            //    string[] jsonStringData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
            //    foreach (string ColumnsNameData in jsonStringData)
            //    {
            //        try
            //        {
            //            int idx = ColumnsNameData.IndexOf(":");
            //            string ColumnsNameString = ColumnsNameData.Substring(0, idx - 1).Replace("\"", "").Replace(",","");
            //            if (!ColumnsName.Contains(ColumnsNameString))
            //            {
            //                ColumnsName.Add(ColumnsNameString);
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            throw new Exception(string.Format("Error Parsing Column Name : {0}", ColumnsNameData));
            //        }
            //    }
            //    break;
            //}
            //foreach (string AddColumnName in ColumnsName)
            //{
            //    dt.Columns.Add(AddColumnName);
            //}
            //foreach (string jSA in jsonStringArray)
            //{
            //    string[] RowData = Regex.Split(jSA.Replace("{", "").Replace("}", ""), ",");
            //    DataRow nr = dt.NewRow();
            //    foreach (string rowData in RowData)
            //    {
            //        try
            //        {
            //            int idx = rowData.IndexOf(":");
            //            string RowColumns = rowData.Substring(0, idx - 1).Replace("\"", "");
            //            string RowDataString = rowData.Substring(idx + 1).Replace("\"", "");
            //            nr[RowColumns] = RowDataString;
            //        }
            //        catch (Exception ex)
            //        {
            //            continue;
            //        }
            //    }
            //    dt.Rows.Add(nr);
            //}
            //return dt;
        }

        public static string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText.Replace(' ', '+'));
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public static string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }


        // GET: /Webservice/
        //public DataTable GetSchoolDetailsByUdiseCode(string UdiseCode)
        //{                
        //    string strParameter = Encrypt("PSEB4488~08059585PSEB~" + UdiseCode);
        //    string JsonString = Decrypt(abc.getSchoolDetailsByUdiseCode(strParameter));
        //    string[] result = JsonString.Split(new string[] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
        //    DataTable dataTable = JsonConvert.DeserializeObject<DataTable>(result[0]);
        //    return dataTable;          
        //}


        #region Deo-Portal Staff Master 
        public ActionResult Indexdeo(string CCODEID)
        {
            webSerModel wm = new webSerModel();
            if (CCODEID == "" || CCODEID == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }

            CCODEID = encrypt.QueryStringModule.Decrypt(CCODEID);
            Session["CCODEID"] = CCODEID;
            string BlockID = "";
            if (Session["CCODEID"] != null)
            {
                BlockID = Session["CCODEID"].ToString();
            }


            if (Session["DeoLoginExamCentre"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }


            return View();
        }
        [HttpPost]
        public ActionResult Indexdeo(FormCollection frc, webSerModel wm, string cmd, string importBy, string SearchString, string chkImportid)
        {
            if (Session["DeoLoginExamCentre"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }
            string BlockID = "";
            string DeoUser = Session["USER"].ToString();
            string userdist = Session["Dist"].ToString();
            string uid = Session["UID"].ToString();

            if (Session["USER"] == null || Session["Dist"] == null || Session["UID"] == null)
            {
                return RedirectToAction("Index", "DeoPortal");
            }

            if (Session["CCODEID"] != null)
            {
                BlockID = Session["CCODEID"].ToString();
            }
            else
            {
                return RedirectToAction("Index", "DeoPortal");
            }
          
            string EpunSearch = "";
            string aadharnoSearch = "";
            string res = null;
            string TD = frc["typeDuty"];// Session["SCHL"].ToString();
            string ImportBy = frc["importBy"].ToString();
            string strParameter = "";

            wm.SearchString = frc["SearchString"].ToString().Trim();
            ViewBag.Searchstring = frc["SearchString"].ToString().Trim();

            #region search statements
            if (cmd == "Search")
            {
                try
                {

                    EpunSearch = SearchString.ToString().Trim();
                    if (ImportBy == "1") 
                    {
                        strParameter = Encrypt("PSEB4488~08059585PSEB~STAFFID~" + EpunSearch);
                    }
                    else if (ImportBy == "2")
                    {
                        strParameter = Encrypt("PSEB4488~08059585PSEB~UID~" + EpunSearch);
                    }
                    else if (ImportBy == "3")
                    {
                        strParameter = Encrypt("PSEB4488~08059585PSEB~UDISE~" + EpunSearch);
                    }


                    string JsonString = Decrypt(abc.getStaffDetails(strParameter));
                    DataTable dt1 = new DataTable();
                    dt1 = JsonStringToDataTable(JsonString);

                    //ViewBag.TotalCount = dt.Rows.Count;
                    wm.StoreAllDataTable = dt1;
                    //wm.StoreAllData = objDB.GetudiCodeDetails(UdiseCode);
                    //if (wm.StoreAllData == null || wm.StoreAllData.Tables[0].Rows.Count == 0)
                    if (wm.StoreAllDataTable == null || wm.StoreAllDataTable.Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(wm);
                    }
                    else
                    {
                        ViewBag.TotalCount = wm.StoreAllDataTable.Rows.Count;
                        return View(wm);
                    }
                }
                catch (Exception e)
                {
                    TempData["result"] = e.Message.ToString();
                    return View(wm);
                    throw;
                }


            }

            #endregion search

            #region Import Selected Record

            if (cmd == "Import Selected Record")
            {
                try
                {
                    epunjabschool.service_pseb abc = new epunjabschool.service_pseb();
                    if (importBy != "")
                    {
                        ViewBag.SelectedItem = importBy;
                        int SelValueSch = Convert.ToInt32(importBy.ToString());
                        EpunSearch = SearchString.ToString().Trim();

                        EpunSearch = frc["ChkStaffID"].ToString();

                        if (EpunSearch != "")
                        {
                            int count = 0;
                            string[] words = EpunSearch.Split(',');
                            foreach (string word in words)
                            {
                                try
                                {

                                    if (ImportBy == "1" || ImportBy == "2" || ImportBy == "3")
                                    {

                                        strParameter = Encrypt("PSEB4488~08059585PSEB~STAFFID~" + word);
                                    }
                                    //else if (ImportBy == "2")
                                    //{
                                    //EpunSearch = frc["UID"].ToString();
                                    //strParameter = Encrypt("PSEB4488~08059585PSEB~UID~" + EpunSearch);
                                    //}
                                    //else if (ImportBy == "3")
                                    //{
                                    //EpunSearch = frc["UDISE_Code"].ToString();
                                    //strParameter = Encrypt("PSEB4488~08059585PSEB~UDISE~" + EpunSearch);
                                    //}
                                    string dutytype = "typeDuty" + word;
                                    TD = frc[dutytype];
                                    
                                    string JsonString = Decrypt(abc.getStaffDetails(strParameter));
                                    DataTable dt1 = new DataTable();
                                    dt1 = JsonStringToDataTable(JsonString);
                                    if (dt1 != null)
                                    {
                                        string epunjabid = dt1.Rows[0]["epunjabid"].ToString();
                                        string adharno = dt1.Rows[0]["adharno"].ToString();
                                        string schl = dt1.Rows[0]["schl"].ToString();
                                        string name = dt1.Rows[0]["name"].ToString();
                                        string fname = dt1.Rows[0]["fname"].ToString();
                                        string cadre = dt1.Rows[0]["cadre"].ToString();
                                        string subject = dt1.Rows[0]["subject"].ToString();
                                        string schlnm = dt1.Rows[0]["schlnm"].ToString();
                                        string DISTNM = dt1.Rows[0]["DISTNM"].ToString();
                                        string mobile = dt1.Rows[0]["mobile"].ToString();
                                        string gender = dt1.Rows[0]["gender"].ToString();
                                        string DOB = dt1.Rows[0]["DOB"].ToString();// Convert.ToDateTime(dt1.Rows[0]["DOB"]).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                        string Disability = dt1.Rows[0]["Disability"].ToString();
                                        string expyear = dt1.Rows[0]["expyear"].ToString();
                                        string expmonth = dt1.Rows[0]["expmonth"].ToString();
                                        string bank = dt1.Rows[0]["bank"].ToString();
                                        string ifsc = dt1.Rows[0]["ifsc"].ToString();
                                        string acno = dt1.Rows[0]["acno"].ToString();
                                        string Updatedate = dt1.Rows[0]["Updatedate"].ToString();
                                        string EDUBLOCK = dt1.Rows[0]["EDUBLOCK"].ToString();
                                        string EDUCLUSTER = dt1.Rows[0]["EDUCLUSTER"].ToString();
                                        string UDISE = dt1.Rows[0]["UDISE"].ToString();
                                        string ADDRESS = dt1.Rows[0]["ADDRESS"].ToString();
                                        string HOME_DIST = dt1.Rows[0]["HOME_DIST"].ToString();
                                        string Staff_Status = dt1.Rows[0]["Staff_Status"].ToString();
                                        string ImportStaffFlag = "1";
                                        res = objDB.Import_All_deostaff_Data(ImportStaffFlag, DeoUser, userdist, uid, BlockID, TD, epunjabid, adharno, schl, name, fname, cadre, subject, schlnm, DISTNM, mobile, gender, DOB, Disability, expyear, expmonth, bank, ifsc, acno, Updatedate, EDUBLOCK, EDUCLUSTER, UDISE, ADDRESS, HOME_DIST, Staff_Status, "", Session["DeoLoginExamCentre"].ToString());
                                        count++;
                                    }
                                }

                                catch (Exception e)
                                {
                                    TempData["result"] = e.Message.ToString();
                                    return View(wm);
                                    throw;
                                }
                            } ///--- forEach function end

                            if (res != null)
                            {
                                //--------------saved successfully-------
                                TempData["TotImported"] = count;
                                TempData["result"] = res;
                                ViewData["result"] = res;
                                return View(wm);
                            }
                        }                        
                    }
                }
                catch (Exception e)
                {
                    TempData["result"] = e.Message.ToString();
                    return View(wm);
                }

            }

            #endregion Import Selected Record
            return View(wm);
        }

        #endregion Deo-Portal Staff Master 


        #region School Import Staff from Deo
        public ActionResult IndexStaffImport()
        {          

            webSerModel wm = new webSerModel();
            AbstractLayer.SchoolDB objDB = new AbstractLayer.SchoolDB();
            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Login"); }
            //string CCODEID = Session["SCHL"].ToString();
            else
            {
                string Schl = Session["SCHL"].ToString();
                string Cent = Session["cent"].ToString();
                if (Cent != "")
                {
                    DataSet Dresult = objDB.GetCentcode(Schl);
                    List<SelectListItem> schllist = new List<SelectListItem>();
                    foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
                    {
                        schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
                    }

                    ViewBag.MySchCode = schllist;

                    ViewBag.TotalCount = null;
                    ViewBag.msg = "Data Not Found";
                    ViewBag.Message = "Data Not Found";
                    return View();
                }      
                }
            return View();
        }
        [HttpPost]
        public ActionResult IndexStaffImport(FormCollection frc, webSerModel wm, string cmd, string importBy, string SearchString, string chkImportid)
        {
            string BlockID = "";
            string DeoUser = "";
            string userdist = "";
            string uid = "";

            if (Session["SCHL"] == null)
            { return RedirectToAction("Index", "Login"); }

            AbstractLayer.SchoolDB objDB1 = new AbstractLayer.SchoolDB();
            string Schl = Session["SCHL"].ToString();
            DeoUser = Session["SCHL"].ToString();
            DataSet Dresult = objDB1.GetCentcode(Schl);
            List<SelectListItem> schllist = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in Dresult.Tables[0].Rows)
            {
                schllist.Add(new SelectListItem { Text = @dr["block"].ToString(), Value = @dr["cent"].ToString() });
            }

            ViewBag.MySchCode = schllist;
            wm.ExamCent = frc["ExamCent"].ToString();

            string EpunSearch = "";
            string res = null;
            string TD = frc["typeDuty"];// Session["SCHL"].ToString();
            string ImportBy = frc["importBy"].ToString();
            string strParameter = "";

            wm.SearchString = frc["SearchString"].ToString().Trim();
            ViewBag.Searchstring = frc["SearchString"].ToString().Trim();
            res = objDB.getStaffDetailsDeo(wm.ExamCent, wm.SearchString);
            if (res != "")
            {
                TempData["result"] = res;
                ViewData["result"] = res;
                return View(wm);
            }
            else
            {
         
            #region search statements
            if (cmd == "Search")
            {
                try
                {

                    EpunSearch = SearchString.ToString().Trim();
                    if (ImportBy == "1")
                    {
                        strParameter = Encrypt("PSEB4488~08059585PSEB~STAFFID~" + EpunSearch);
                    }
                    else if (ImportBy == "2")
                    {
                        strParameter = Encrypt("PSEB4488~08059585PSEB~UID~" + EpunSearch);
                    }
                    else if (ImportBy == "3")
                    {
                        strParameter = Encrypt("PSEB4488~08059585PSEB~UDISE~" + EpunSearch);
                    }


                    string JsonString = Decrypt(abc.getStaffDetails(strParameter));
                    DataTable dt1 = new DataTable();
                    dt1 = JsonStringToDataTable(JsonString);

                    //ViewBag.TotalCount = dt.Rows.Count;
                    wm.StoreAllDataTable = dt1;
                    //wm.StoreAllData = objDB.GetudiCodeDetails(UdiseCode);
                    //if (wm.StoreAllData == null || wm.StoreAllData.Tables[0].Rows.Count == 0)
                    if (wm.StoreAllDataTable == null || wm.StoreAllDataTable.Rows.Count == 0)
                    {
                        ViewBag.Message = "Record Not Found";
                        ViewBag.TotalCount = 0;
                        return View(wm);
                    }
                    else
                    {
                        ViewBag.TotalCount = wm.StoreAllDataTable.Rows.Count;
                        return View(wm);
                    }
                }
                catch (Exception e)
                {
                    TempData["result"] = e.Message.ToString();
                    return View(wm);
                    throw;
                }


            }

            #endregion search

            #region Import Selected Record

            if (cmd == "Import Selected Record")
            {
                try
                {
                    epunjabschool.service_pseb abc = new epunjabschool.service_pseb();
                    if (importBy != "")
                    {
                        ViewBag.SelectedItem = importBy;
                        int SelValueSch = Convert.ToInt32(importBy.ToString());
                        EpunSearch = SearchString.ToString().Trim();

                        EpunSearch = frc["ChkStaffID"].ToString();

                        if (EpunSearch != "")
                        {
                            int count = 0;
                            string[] words = EpunSearch.Split(',');
                            foreach (string word in words)
                            {
                                    try
                                    {

                                        if (ImportBy == "1" || ImportBy == "2" || ImportBy == "3")
                                        {

                                            strParameter = Encrypt("PSEB4488~08059585PSEB~STAFFID~" + word);
                                        }
                                        string dutytype = "typeDuty" + word;
                                        TD = frc[dutytype];

                                        string JsonString = Decrypt(abc.getStaffDetails(strParameter));
                                        DataTable dt1 = new DataTable();
                                        dt1 = JsonStringToDataTable(JsonString);
                                        if (dt1 != null)
                                        { 
                                        string epunjabid = dt1.Rows[0]["epunjabid"].ToString();
                                        string adharno = dt1.Rows[0]["adharno"].ToString();
                                        string schl = dt1.Rows[0]["schl"].ToString();
                                        string name = dt1.Rows[0]["name"].ToString();
                                        string fname = dt1.Rows[0]["fname"].ToString();
                                        string cadre = dt1.Rows[0]["cadre"].ToString();
                                        string subject = dt1.Rows[0]["subject"].ToString();
                                        string schlnm = dt1.Rows[0]["schlnm"].ToString();
                                        string DISTNM = dt1.Rows[0]["DISTNM"].ToString();
                                        string mobile = dt1.Rows[0]["mobile"].ToString();
                                        string gender = dt1.Rows[0]["gender"].ToString();
                                        string DOB = dt1.Rows[0]["DOB"].ToString();// Convert.ToDateTime(dt1.Rows[0]["DOB"]).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                        string Disability = dt1.Rows[0]["Disability"].ToString();
                                        string expyear = dt1.Rows[0]["expyear"].ToString();
                                        string expmonth = dt1.Rows[0]["expmonth"].ToString();
                                        string bank = dt1.Rows[0]["bank"].ToString();
                                        string ifsc = dt1.Rows[0]["ifsc"].ToString();
                                        string acno = dt1.Rows[0]["acno"].ToString();
                                        string Updatedate = dt1.Rows[0]["Updatedate"].ToString();
                                        string EDUBLOCK = dt1.Rows[0]["EDUBLOCK"].ToString();
                                        string EDUCLUSTER = dt1.Rows[0]["EDUCLUSTER"].ToString();
                                        string UDISE = dt1.Rows[0]["UDISE"].ToString();
                                        string ADDRESS = dt1.Rows[0]["ADDRESS"].ToString();
                                        string HOME_DIST = dt1.Rows[0]["HOME_DIST"].ToString();
                                        string Staff_Status = dt1.Rows[0]["Staff_Status"].ToString();
                                        string ImportStaffFlag = "2";
                                            string DeoLoginExamCentre = "";

                                            if (Session["DeoLoginExamCentre"] != null)
                                            { 
                                                DeoLoginExamCentre = Session["DeoLoginExamCentre"].ToString();

                                            }
                                            else
                                            { 
                                                DeoLoginExamCentre = "";
                                            }
                                            res = objDB.Import_All_deostaff_Data(ImportStaffFlag, DeoUser, userdist, uid, BlockID, TD, epunjabid, adharno, schl, name, fname, cadre, subject, schlnm, DISTNM, mobile, gender, DOB, Disability, expyear, expmonth, bank, ifsc, acno, Updatedate, EDUBLOCK, EDUCLUSTER, UDISE, ADDRESS, HOME_DIST, Staff_Status, wm.ExamCent, DeoLoginExamCentre);
                                            res = objDB.IndexStaffImport(TD, epunjabid, wm.ExamCent);




                                            count++;
                                    }
                                }

                                catch (Exception e)
                                {
                                    TempData["result"] = e.Message.ToString();
                                    return View(wm);
                                    throw;
                                }
                            } ///--- forEach function end

                            if (res != null)
                            {
                                //--------------saved successfully-------
                                TempData["TotImported"] = count;
                                TempData["result"] = res;
                                ViewData["result"] = res;
                                return View(wm);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    TempData["result"] = e.Message.ToString();
                    return View(wm);
                }

            }

            #endregion Import Selected Record
            return View(wm);
            }
        }

        #endregion School Import Staff from Deo
    }
}