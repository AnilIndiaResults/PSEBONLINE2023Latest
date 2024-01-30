using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using PSEBONLINE.Models;
using System.IO;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Collections;
using System.Data.Odbc;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace PSEBONLINE.AbstractLayer
{

    public class EAffiliationDB
    {

        #region Check ConString

        private string CommonCon = "myDBConnection";
        public EAffiliationDB()
        {
            CommonCon = "myDBConnection";
        }
        #endregion  Check ConString

        #region ChangePassword

        public int ChangePassword(int UserId, string CurrentPassword, string NewPassword)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("EAffiliationChangePasswordSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@UserId", UserId);
                    cmd.Parameters.AddWithValue("@OldPwd", CurrentPassword);
                    cmd.Parameters.AddWithValue("@NewPwd", NewPassword);
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    return result;

                }
            }
            catch (Exception ex)
            {
                return result = -1;
            }
            finally
            {
                // con.Close();
            }
        }
        #endregion ChangePassword


        public SchoolModels SelectSchoolDatabyID_For_EAffiliation(string SCHL,string APPNO, out DataSet result)
        {
            SchoolModels sm = new SchoolModels();
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SelectSchoolDatabyID_For_EAffiliation", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@APPNO", APPNO);
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    con.Open();
                    if (ds.Tables.Count > 0)
                    {
                        if (SCHL.ToLower() == "new".ToLower())
                        {
                            sm.SCHL = APPNO;
                        }
                        else
                        {
                            if (ds.Tables[0].Rows.Count > 0)
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
                                sm.SCHLNME = ds.Tables[0].Rows[0]["SCHLEfull"].ToString();
                                sm.dist = ds.Tables[0].Rows[0]["DIST"].ToString();
                                sm.DISTE = ds.Tables[0].Rows[0]["DISTE"].ToString();
                                sm.STATIONE = ds.Tables[0].Rows[0]["STATIONE"].ToString();
                                sm.DISTNM = ds.Tables[0].Rows[0]["DISTNM"].ToString();

                                sm.SCHLP = ds.Tables[0].Rows[0]["SCHLP"].ToString();
                                sm.SCHLNMP = ds.Tables[0].Rows[0]["SCHLPfull"].ToString();
                                sm.DISTP = ds.Tables[0].Rows[0]["DISTP"].ToString();
                                sm.STATIONP = ds.Tables[0].Rows[0]["STATIONP"].ToString();

                                sm.PRINCIPAL = ds.Tables[0].Rows[0]["PRINCIPAL"].ToString();
                                sm.MOBILE = ds.Tables[0].Rows[0]["MOBILE"].ToString();
                                sm.mobile2 = ds.Tables[0].Rows[0]["mobile2"].ToString();
                                sm.ADDRESSE = ds.Tables[0].Rows[0]["ADDRESSE"].ToString();
                                sm.ADDRESSP = ds.Tables[0].Rows[0]["ADDRESSP"].ToString();
                                sm.CONTACTPER = ds.Tables[0].Rows[0]["CONTACTPER"].ToString();
                                sm.EMAILID = ds.Tables[0].Rows[0]["EMAILID"].ToString();
                                sm.STDCODE = ds.Tables[0].Rows[0]["STDCODE"].ToString();
                                sm.PHONE = ds.Tables[0].Rows[0]["PHONE"].ToString();
                                sm.OtContactno = ds.Tables[0].Rows[0]["OtContactno"].ToString();

                                sm.DOB = ds.Tables[0].Rows[0]["DOB"].ToString();
                                sm.DOJ = ds.Tables[0].Rows[0]["DOJ"].ToString();
                                sm.ExperienceYr = ds.Tables[0].Rows[0]["ExperienceYr"].ToString();
                                sm.PQualification = ds.Tables[0].Rows[0]["PQualification"].ToString();
                                sm.NSQF_flag = ds.Tables[0].Rows[0]["NSQF_flag"].ToString() == "Y" ? "YES" : "NO";


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

                                //---------------Ranjan------------
                                sm.HID_UTYPE = ds.Tables[0].Rows[0]["HID_UTYPE"].ToString();
                                sm.MID_UTYPE = ds.Tables[0].Rows[0]["MID_UTYPE"].ToString();
                                sm.H_UTYPE = ds.Tables[0].Rows[0]["H_UTYPE"].ToString();
                                sm.S_UTYPE = ds.Tables[0].Rows[0]["S_UTYPE"].ToString();
                                sm.C_UTYPE = ds.Tables[0].Rows[0]["C_UTYPE"].ToString();
                                sm.V_UTYPE = ds.Tables[0].Rows[0]["V_UTYPE"].ToString();
                                sm.A_UTYPE = ds.Tables[0].Rows[0]["A_UTYPE"].ToString();
                                sm.T_UTYPE = ds.Tables[0].Rows[0]["T_UTYPE"].ToString();

                                sm.HID_YR = sm.MATRIC == "YES" ? ds.Tables[0].Rows[0]["HID_YR"].ToString() : "XXX";
                                sm.MID_YR = sm.MATRIC == "YES" ? ds.Tables[0].Rows[0]["MID_YR"].ToString() : "XXX";
                                sm.HYR = sm.HUM == "YES" ? ds.Tables[0].Rows[0]["HYR"].ToString() : "XXX";
                                sm.SYR = sm.SCI == "YES" ? ds.Tables[0].Rows[0]["SYR"].ToString() : "XXX";
                                sm.CYR = sm.COMM == "YES" ? ds.Tables[0].Rows[0]["CYR"].ToString() : "XXX";
                                sm.VYR = sm.VOC == "YES" ? ds.Tables[0].Rows[0]["VYR"].ToString() : "XXX";
                                sm.TYR = sm.TECH == "YES" ? ds.Tables[0].Rows[0]["TYR"].ToString() : "XXX";
                                sm.AYR = sm.AGRI == "YES" ? ds.Tables[0].Rows[0]["AYR"].ToString() : "XXX";

                                sm.OHID_YR = sm.OMATRIC == "YES" ? ds.Tables[0].Rows[0]["HID_YR"].ToString() : "XXX";
                                sm.OMID_YR = sm.OMATRIC == "YES" ? ds.Tables[0].Rows[0]["MID_YR"].ToString() : "XXX";
                                sm.OHYR = sm.OHUM == "YES" ? ds.Tables[0].Rows[0]["HYR"].ToString() : "XXX";
                                sm.OSYR = sm.OSCI == "YES" ? ds.Tables[0].Rows[0]["SYR"].ToString() : "XXX";
                                sm.OCYR = sm.OCOMM == "YES" ? ds.Tables[0].Rows[0]["CYR"].ToString() : "XXX";
                                sm.OVYR = sm.OVOC == "YES" ? ds.Tables[0].Rows[0]["VYR"].ToString() : "XXX";
                                sm.OTYR = sm.OTECH == "YES" ? ds.Tables[0].Rows[0]["TYR"].ToString() : "XXX";
                                sm.OAYR = sm.OAGRI == "YES" ? ds.Tables[0].Rows[0]["AYR"].ToString() : "XXX";
                                //---Secsion ---------------
                                sm.HID_YR_SEC = sm.MATRIC == "YES" && sm.HID_UTYPE != "AFF" && sm.HID_UTYPE != "ASS" ? "N.A." : "XXX";
                                sm.MID_YR_SEC = sm.MATRIC == "YES" && sm.HID_UTYPE != "AFF" && sm.HID_UTYPE != "ASS" ? "N.A." : "XXX";
                                sm.HYR_SEC = sm.HUM == "YES" && sm.HID_UTYPE != "AFF" && sm.HID_UTYPE != "ASS" ? "N.A." : "XXX";
                                sm.SYR_SEC = sm.SCI == "YES" && sm.HID_UTYPE != "AFF" && sm.HID_UTYPE != "ASS" ? "N.A." : "XXX";
                                sm.CYR_SEC = sm.COMM == "YES" && sm.HID_UTYPE != "AFF" && sm.HID_UTYPE != "ASS" ? "N.A." : "XXX";
                                sm.VYR_SEC = sm.VOC == "YES" && sm.HID_UTYPE != "AFF" && sm.HID_UTYPE != "ASS" ? "N.A." : "XXX";
                                sm.TYR_SEC = sm.TECH == "YES" && sm.HID_UTYPE != "AFF" && sm.HID_UTYPE != "ASS" ? "N.A." : "XXX";
                                sm.AYR_SEC = sm.AGRI == "YES" && sm.HID_UTYPE != "AFF" && sm.HID_UTYPE != "ASS" ? "N.A." : "XXX";

                                sm.OHID_YR_SEC = sm.OMATRIC == "YES" ? "N.A." : "XXX";
                                sm.OMID_YR_SEC = sm.OMATRIC == "YES" ? "N.A." : "XXX";
                                sm.OHYR_SEC = sm.OHUM == "YES" ? "N.A." : "XXX";
                                sm.OSYR_SEC = sm.OSCI == "YES" ? "N.A." : "XXX";
                                sm.OCYR_SEC = sm.OCOMM == "YES" ? "N.A." : "XXX";
                                sm.OVYR_SEC = sm.OVOC == "YES" ? "N.A." : "XXX";
                                sm.OTYR_SEC = sm.OTECH == "YES" ? "N.A." : "XXX";
                                sm.OAYR_SEC = sm.OAGRI == "YES" ? "N.A." : "XXX";

                                sm.Tehsile = ds.Tables[0].Rows[0]["Tcode"].ToString();

                                sm.SchlEstd = ds.Tables[0].Rows[0]["SCHLESTD"].ToString();
                                sm.SchlType = ds.Tables[0].Rows[0]["SCHLTYPE"].ToString();
                                sm.Edublock = ds.Tables[0].Rows[0]["EDUBLOCK"].ToString();
                                sm.EduCluster = ds.Tables[0].Rows[0]["EDUCLUSTER"].ToString();
                                //
                                sm.omattype = ds.Tables[0].Rows[0]["omattype"].ToString();
                                sm.ohumtype = ds.Tables[0].Rows[0]["ohumtype"].ToString();
                                sm.oscitype = ds.Tables[0].Rows[0]["oscitype"].ToString();
                                sm.ocommtype = ds.Tables[0].Rows[0]["ocommtype"].ToString();
                                sm.Bank = ds.Tables[0].Rows[0]["bank"].ToString();
                                sm.IFSC = ds.Tables[0].Rows[0]["ifsc"].ToString();
                                sm.acno = ds.Tables[0].Rows[0]["acno"].ToString();

                                //FIFTH
                                sm.fifth = ds.Tables[0].Rows[0]["fifth"].ToString() == "Y" ? "YES" : "NO";
                                sm.FIF_YR = sm.fifth == "YES" ? ds.Tables[0].Rows[0]["FIF_YR"].ToString() : "XXX";
                                sm.FIF_UTYPE = ds.Tables[0].Rows[0]["FIF_UTYPE"].ToString();
                                sm.FIF_S = ds.Tables[0].Rows[0]["FIF_S"].ToString();
                                sm.lclass = ds.Tables[0].Rows[0]["lclass"].ToString();
                                sm.FIF_NO = ds.Tables[0].Rows[0]["FIF_NO"].ToString();
                                sm.udisecode = ds.Tables[0].Rows[0]["udisecode"].ToString();
                                sm.uclass = ds.Tables[0].Rows[0]["uclass"].ToString();
                            }
                        }
                    }
                    result = ds;
                    return sm;
                }
            }
            catch (Exception ex)
            {
                result = null;
                return null;
            }

        }


        public static List<SelectListItem> GetApplicationTypeList()
        {
            List<SelectListItem> itemOrder = new List<SelectListItem>();
            itemOrder.Add(new SelectListItem { Text = "E-Affiliation", Value = "AFF" });
            itemOrder.Add(new SelectListItem { Text = "Affiliation Continuation", Value = "AC" });
            itemOrder.Add(new SelectListItem { Text = "Additional Section", Value = "AS" });            
            return itemOrder;
        }
    
            public static List<EAffiliationClassMasters> GetEAffiliationClassMasterList()
        {
            List<EAffiliationClassMasters> eAffiliationClassMasters = new List<EAffiliationClassMasters>();
            try
            {
                using (DBContext context = new DBContext())
                {
                    //For Activation 5th and 8th for test2022.psebonline.in
                    eAffiliationClassMasters = context.EAffiliationClassMasters.Where(s => s.IsActive == true).ToList();
                }
            }
            catch (Exception ex)
            {
               
            }
           
            return eAffiliationClassMasters;
        }


        public DataSet CheckPinCodeMasterSP(int type, string pincode)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CheckPinCodeMasterSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@pincode", pincode);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }


        public EAffiliationDashBoardViewModel GetEAffiliationDashBoard(string appno)
        {
            EAffiliationDashBoardViewModel eAffiliationDashBoardViewModel = new EAffiliationDashBoardViewModel();
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            string search = "where APPNO = " + appno;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetEAffiliationDashBoardSP", con);  //SelectPrintList_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@appno", appno);
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    con.Open();
                    try
                    {

                       if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];                            
                            eAffiliationDashBoardViewModel.APPNO = dr["APPNO"].ToString();
                            eAffiliationDashBoardViewModel.SCHLNAME = dr["SCHLNAME"].ToString();
                            eAffiliationDashBoardViewModel.CREATEDDATE = dr["CREATEDDATE"].ToString();
                            eAffiliationDashBoardViewModel.FeePaidStatus = dr["FeePaidStatus"].ToString();
                            eAffiliationDashBoardViewModel.ForwardedToPSEB = dr["ForwardedToPSEB"].ToString();
                            eAffiliationDashBoardViewModel.ForwardedToInspection = dr["ForwardedToInspection"].ToString();

                            eAffiliationDashBoardViewModel.Objection = dr["Objection"].ToString();
                            eAffiliationDashBoardViewModel.FormUnlocked = dr["FormUnlocked"].ToString();
                            eAffiliationDashBoardViewModel.ForwardForApproval = dr["ForwardForApproval"].ToString();
                            eAffiliationDashBoardViewModel.ApprovedStatus = dr["ApprovedStatus"].ToString();


                            return eAffiliationDashBoardViewModel;
                        }
                        else
                        {
                            return new EAffiliationDashBoardViewModel();
                        }
                    }
                    catch (Exception e)
                    {
                        return new EAffiliationDashBoardViewModel();
                    }
                }
            }
            catch (Exception ex)
            {
                return new EAffiliationDashBoardViewModel();
            }
        }



        public EAffiliationModel GetEAff_NewschlList(string search)
        {
            EAffiliationModel _EAffiliationModel = new EAffiliationModel();
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            //string search = "where udise = " + udise;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetEAff_NewschlList", con);  //SelectPrintList_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    con.Open();
                    try
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = ds.Tables[0].Rows[0];                                                                            
                            _EAffiliationModel.DIST = dr["DIST"].ToString();
                            _EAffiliationModel.DISTNME = dr["distnm"].ToString();
                            _EAffiliationModel.UDISECODE = dr["udise"].ToString();
                            _EAffiliationModel.SCHLNME = dr["schlnm"].ToString();
                            _EAffiliationModel.PrincipalName = dr["principal"].ToString();

                            _EAffiliationModel.SCHLEMAIL = dr["emailid"].ToString();
                            _EAffiliationModel.SCHLMOBILE = dr["mobile"].ToString();

                            

                            return _EAffiliationModel;
                        }
                        else
                        {
                            return new EAffiliationModel();
                        }
                    }
                    catch (Exception e)
                    {
                        return new EAffiliationModel();
                    }
                }
            }
            catch (Exception ex)
            {
                return new EAffiliationModel();
            }
        }


        public EAffiliationModel GetEAffiliation(string appno)
        {
            EAffiliationModel _EAffiliationModel = new EAffiliationModel();
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            string search = "where APPNO = " + appno;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetEAffiliation", con);  //SelectPrintList_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@appno", appno);
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    con.Open();
                    try
                    {

                        _EAffiliationModel.StoreAllData = ds.Copy();
                        //
                        if (ds.Tables[0].Rows.Count > 0)
                        {                          
                             DataRow dr = ds.Tables[0].Rows[0];
                            _EAffiliationModel.EAffSession = Convert.ToString(dr["EAffSession"].ToString());
                            _EAffiliationModel.EAffClass = Convert.ToString(dr["EAffClass"].ToString());
                            _EAffiliationModel.IsCancelEAffiliation = Convert.ToInt32(dr["IsCancelEAffiliation"].ToString());
                            _EAffiliationModel.ID = Convert.ToInt32(dr["ID"].ToString());
                            _EAffiliationModel.EAffType = dr["EAffType"].ToString();
                            _EAffiliationModel.APPNO = dr["APPNO"].ToString();
                            _EAffiliationModel.SCHLNAME = dr["SCHLNAME"].ToString();
                            _EAffiliationModel.SCHLMOBILE = dr["SCHLMOBILE"].ToString();
                            _EAffiliationModel.SCHLEMAIL = dr["SCHLEMAIL"].ToString();
                            _EAffiliationModel.PWD = dr["PWD"].ToString();
                            _EAffiliationModel.SCHL = dr["SCHL"].ToString();
                            _EAffiliationModel.DIST = dr["DIST"].ToString();
                            _EAffiliationModel.UDISECODE = dr["UDISECODE"].ToString();
                            _EAffiliationModel.SCHLNME = dr["SCHLNME"].ToString();
                            _EAffiliationModel.SCHLNMP = dr["SCHLNMP"].ToString();
                            _EAffiliationModel.STATIONE = dr["STATIONE"].ToString();
                            _EAffiliationModel.STATIONP = dr["STATIONP"].ToString();
                            _EAffiliationModel.ADDRESSE = dr["ADDRESSE"].ToString();
                            _EAffiliationModel.ADDRESSP = dr["ADDRESSP"].ToString();
                            _EAffiliationModel.DISTNME = dr["DISTNME"].ToString();
                            _EAffiliationModel.PINCODE = dr["PINCODE"].ToString();
                            _EAffiliationModel.TehsilCode = dr["TehsilCode"].ToString();
                            _EAffiliationModel.PostOfficeCode = dr["PostOfficeCode"].ToString();
                            _EAffiliationModel.EducationType = dr["EducationType"].ToString();
                            _EAffiliationModel.Area = dr["Area"].ToString();
                            _EAffiliationModel.PrincipalName = dr["PrincipalName"].ToString();
                            _EAffiliationModel.Qualification = dr["Qualification"].ToString();
                            _EAffiliationModel.DOJ = dr["DOJ"].ToString();
                            _EAffiliationModel.OtherContactPerson = dr["OtherContactPerson"].ToString();
                            _EAffiliationModel.DOB = dr["DOB"].ToString();
                            _EAffiliationModel.Experience = dr["Experience"].ToString();
                            _EAffiliationModel.StdCode = dr["StdCode"].ToString();
                            _EAffiliationModel.PrincipalMobileNo = dr["PrincipalMobileNo"].ToString();
                            _EAffiliationModel.SocietyName = dr["SocietyName"].ToString();
                            _EAffiliationModel.SocietyRegNo = dr["SocietyRegNo"].ToString();
                            _EAffiliationModel.SocietyRegDate = Convert.ToString(dr["SocietyRegDate"].ToString());
                            _EAffiliationModel.SocietyNOM = dr["SocietyNOM"].ToString();
                            _EAffiliationModel.SocietyChairmanName = dr["SocietyChairmanName"].ToString();
                            _EAffiliationModel.SocietyChairmanMobile = dr["SocietyChairmanMobile"].ToString();
                            _EAffiliationModel.SocietyFile = dr["SocietyFile"].ToString();
                            _EAffiliationModel.BSFROM = dr["BSFROM"].ToString();
                            _EAffiliationModel.BSTO = dr["BSTO"].ToString();
                            _EAffiliationModel.BSIA = dr["BSIA"].ToString();
                            _EAffiliationModel.BSMEMO = dr["BSMEMO"].ToString();
                            _EAffiliationModel.BSIDATE = Convert.ToString(dr["BSIDATE"].ToString());
                            _EAffiliationModel.BSFILE = dr["BSFILE"].ToString();
                            _EAffiliationModel.FSFROM = dr["FSFROM"].ToString();
                            _EAffiliationModel.FSTO = dr["FSTO"].ToString();
                            _EAffiliationModel.FSIA = dr["FSIA"].ToString();
                            _EAffiliationModel.FSMEMO = dr["FSMEMO"].ToString();
                            _EAffiliationModel.FSIDATE = Convert.ToString(dr["FSIDATE"].ToString());
                            _EAffiliationModel.FSFILE = dr["FSFILE"].ToString();
                            _EAffiliationModel.MAPNAME = dr["MAPNAME"].ToString();
                            _EAffiliationModel.MAPREGNO = dr["MAPREGNO"].ToString();
                            _EAffiliationModel.MAPAUTH = dr["MAPAUTH"].ToString();
                            _EAffiliationModel.MAPMEMO = dr["MAPMEMO"].ToString();
                            _EAffiliationModel.MAPIDATE = Convert.ToString(dr["MAPIDATE"].ToString());
                            _EAffiliationModel.MAPFILE = dr["MAPFILE"].ToString();//
                            //
                            _EAffiliationModel.CLUAUTH = dr["CLUAUTH"].ToString();
                            _EAffiliationModel.CLUMEMO = dr["CLUMEMO"].ToString();
                            _EAffiliationModel.CLUIDATE = Convert.ToString(dr["CLUIDATE"].ToString());
                            _EAffiliationModel.CLUFILE = dr["CLUFILE"].ToString();
                            //
                            _EAffiliationModel.C1B = Convert.ToInt32(dr["C1B"].ToString());
                            _EAffiliationModel.C1G = Convert.ToInt32(dr["C1G"].ToString());
                            _EAffiliationModel.C1T = Convert.ToInt32(dr["C1T"].ToString());
                            _EAffiliationModel.C2B = Convert.ToInt32(dr["C2B"].ToString());
                            _EAffiliationModel.C2G = Convert.ToInt32(dr["C2G"].ToString());
                            _EAffiliationModel.C2T = Convert.ToInt32(dr["C2T"].ToString());
                            _EAffiliationModel.C3B = Convert.ToInt32(dr["C3B"].ToString());
                            _EAffiliationModel.C3G = Convert.ToInt32(dr["C3G"].ToString());
                            _EAffiliationModel.C3T = Convert.ToInt32(dr["C3T"].ToString());
                            _EAffiliationModel.C4B = Convert.ToInt32(dr["C4B"].ToString());
                            _EAffiliationModel.C4G = Convert.ToInt32(dr["C4G"].ToString());
                            _EAffiliationModel.C4T = Convert.ToInt32(dr["C4T"].ToString());
                            _EAffiliationModel.C5B = Convert.ToInt32(dr["C5B"].ToString());
                            _EAffiliationModel.C5G = Convert.ToInt32(dr["C5G"].ToString());
                            _EAffiliationModel.C5T = Convert.ToInt32(dr["C5T"].ToString());
                            _EAffiliationModel.C6B = Convert.ToInt32(dr["C6B"].ToString());
                            _EAffiliationModel.C6G = Convert.ToInt32(dr["C6G"].ToString());
                            _EAffiliationModel.C6T = Convert.ToInt32(dr["C6T"].ToString());
                            _EAffiliationModel.C7B = Convert.ToInt32(dr["C7B"].ToString());
                            _EAffiliationModel.C7G = Convert.ToInt32(dr["C7G"].ToString());
                            _EAffiliationModel.C7T = Convert.ToInt32(dr["C7T"].ToString());
                            _EAffiliationModel.C8B = Convert.ToInt32(dr["C8B"].ToString());
                            _EAffiliationModel.C8G = Convert.ToInt32(dr["C8G"].ToString());
                            _EAffiliationModel.C8T = Convert.ToInt32(dr["C8T"].ToString());
                            _EAffiliationModel.C9B = Convert.ToInt32(dr["C9B"].ToString());
                            _EAffiliationModel.C9G = Convert.ToInt32(dr["C9G"].ToString());
                            _EAffiliationModel.C9T = Convert.ToInt32(dr["C9T"].ToString());
                            _EAffiliationModel.C10B = Convert.ToInt32(dr["C10B"].ToString());
                            _EAffiliationModel.C10G = Convert.ToInt32(dr["C10G"].ToString());
                            _EAffiliationModel.C10T = Convert.ToInt32(dr["C10T"].ToString());
                            _EAffiliationModel.C11HB = Convert.ToInt32(dr["C11HB"].ToString());
                            _EAffiliationModel.C11HG = Convert.ToInt32(dr["C11HG"].ToString());
                            _EAffiliationModel.C11HT = Convert.ToInt32(dr["C11HT"].ToString());
                            _EAffiliationModel.C11SB = Convert.ToInt32(dr["C11SB"].ToString());
                            _EAffiliationModel.C11SG = Convert.ToInt32(dr["C11SG"].ToString());
                            _EAffiliationModel.C11ST = Convert.ToInt32(dr["C11ST"].ToString());
                            _EAffiliationModel.C11CB = Convert.ToInt32(dr["C11CB"].ToString());
                            _EAffiliationModel.C11CG = Convert.ToInt32(dr["C11CG"].ToString());
                            _EAffiliationModel.C11CT = Convert.ToInt32(dr["C11CT"].ToString());
                            _EAffiliationModel.C11VB = Convert.ToInt32(dr["C11VB"].ToString());
                            _EAffiliationModel.C11VG = Convert.ToInt32(dr["C11VG"].ToString());
                            _EAffiliationModel.C11VT = Convert.ToInt32(dr["C11VT"].ToString());
                            _EAffiliationModel.C12HB = Convert.ToInt32(dr["C12HB"].ToString());
                            _EAffiliationModel.C12HG = Convert.ToInt32(dr["C12HG"].ToString());
                            _EAffiliationModel.C12HT = Convert.ToInt32(dr["C12HT"].ToString());
                            _EAffiliationModel.C12SB = Convert.ToInt32(dr["C12SB"].ToString());
                            _EAffiliationModel.C12SG = Convert.ToInt32(dr["C12SG"].ToString());
                            _EAffiliationModel.C12ST = Convert.ToInt32(dr["C12ST"].ToString());
                            _EAffiliationModel.C12CB = Convert.ToInt32(dr["C12CB"].ToString());
                            _EAffiliationModel.C12CG = Convert.ToInt32(dr["C12CG"].ToString());
                            _EAffiliationModel.C12CT = Convert.ToInt32(dr["C12CT"].ToString());
                            _EAffiliationModel.C12VB = Convert.ToInt32(dr["C12VB"].ToString());
                            _EAffiliationModel.C12VG = Convert.ToInt32(dr["C12VG"].ToString());
                            _EAffiliationModel.C12VT = Convert.ToInt32(dr["C12VT"].ToString());
                            //
                            _EAffiliationModel.TOTALBOYS = Convert.ToInt32(dr["TOTALBOYS"].ToString());
                            _EAffiliationModel.TOTALGIRLS = Convert.ToInt32(dr["TOTALGIRLS"].ToString());
                            _EAffiliationModel.TOTALSTUDENTS= Convert.ToInt32(dr["TOTALSTUDENTS"].ToString());

                            //
                            _EAffiliationModel.OSTUDYMEDIUM = dr["OSTUDYMEDIUM"].ToString();
                            _EAffiliationModel.OLANDTYPE = dr["OLANDTYPE"].ToString();
                            _EAffiliationModel.OTOTALAREA = dr["OTOTALAREA"].ToString();
                            _EAffiliationModel.OCOVAREA = dr["OCOVAREA"].ToString();
                            _EAffiliationModel.OPLAYGROUNDSIZE = dr["OPLAYGROUNDSIZE"].ToString();
                            _EAffiliationModel.OCOURT = dr["OCOURT"].ToString();
                            _EAffiliationModel.OTRANSPORT = dr["OTRANSPORT"].ToString();
                            _EAffiliationModel.OSALARYEMP = dr["OSALARYEMP"].ToString();
                            _EAffiliationModel.OROWATER = dr["OROWATER"].ToString();
                            _EAffiliationModel.OTOILET = dr["OTOILET"].ToString();
                            _EAffiliationModel.OPLAYGROUND = dr["OPLAYGROUND"].ToString();
                            _EAffiliationModel.OBOARDSPLCAND = dr["OBOARDSPLCAND"].ToString();
                            _EAffiliationModel.OCOMPLAB = dr["OCOMPLAB"].ToString();
                            _EAffiliationModel.OSCILAB = dr["OSCILAB"].ToString();
                            _EAffiliationModel.OINTERNET = dr["OINTERNET"].ToString();
                            _EAffiliationModel.OSMARTCLS = dr["OSMARTCLS"].ToString();
                            _EAffiliationModel.OFIRESAFE = dr["OFIRESAFE"].ToString();
                            _EAffiliationModel.OFURNITURE = dr["OFURNITURE"].ToString();
                            _EAffiliationModel.ONOCLASSROOMS = dr["ONOCLASSROOMS"].ToString();
                            _EAffiliationModel.OActivitiesAchievements = dr["OActivitiesAchievements"].ToString();
                            _EAffiliationModel.ISACTIVE = Convert.ToBoolean(dr["ISACTIVE"].ToString());
                            _EAffiliationModel.CREATEDDATE = Convert.ToDateTime(dr["CREATEDDATE"].ToString());
                            //
                            _EAffiliationModel.PHONE = dr["PHONE"].ToString();
                            _EAffiliationModel.PrincipalExperienceFile = dr["PrincipalExperienceFile"].ToString();
                            _EAffiliationModel.IsPhysics = dr["IsPhysics"].ToString();
                            _EAffiliationModel.Islibrary = dr["Islibrary"].ToString();
                            _EAffiliationModel.IsBiology = dr["IsBiology"].ToString();
                            _EAffiliationModel.IsChemistry = dr["IsChemistry"].ToString();

                            _EAffiliationModel.IsFormLock = Convert.ToInt32(dr["IsFormLock"].ToString());
                            _EAffiliationModel.IsFormLockBy = dr["IsFormLockOn"].ToString();
                            _EAffiliationModel.IsFormLockOn = dr["IsFormLockOn"].ToString();
                            //new in 2021-22
                            _EAffiliationModel.IsCCTV = dr["IsCCTV"].ToString();

                            return _EAffiliationModel;
                        }
                        else
                        {
                            return new EAffiliationModel();
                        }
                    }
                    catch (Exception e)
                    {                       
                        return new EAffiliationModel();
                    }
                }
            }
            catch (Exception ex)
            {              
                return new EAffiliationModel();
            }
        }


        public DataSet EAffiliationList(string AdminUser, string search, string clas, int PageNumber, int type)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("EAffiliationListSP", con);  //SelectPrintList_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AdminUser", AdminUser); // O for Admin 1 for School else Openstudent
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@class", clas);
                    cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", 20);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }


        public List<EAffiliationApplicationStatusMaster> ForwardListByAppNoStatus(string AppType, int type, string Appno, string AdminUser)
        {
            List<EAffiliationApplicationStatusMaster> studentSchoolMigrationViewModel = new List<EAffiliationApplicationStatusMaster>();
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ForwardListByAppNoStatusSp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AppType", AppType);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@Appno", Appno);
                    cmd.Parameters.AddWithValue("@AdminUser", AdminUser);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    if (result.Tables[0].Rows.Count > 0)
                    {
                        var itemSubUType = StaticDB.DataTableToList<EAffiliationApplicationStatusMaster>(result.Tables[0]);
                        studentSchoolMigrationViewModel = itemSubUType.ToList();

                    }
                    return studentSchoolMigrationViewModel;
                }
            }
            catch (Exception ex)
            {
                return studentSchoolMigrationViewModel;
            }
        }



        public int EAffiliation(EAffiliationModel am, int action, out string OutError)
        {
            int result;
            OutError = "0";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("EAffiliationSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", action);
                    cmd.Parameters.AddWithValue("@EAffClass", am.EAffClass);
                    cmd.Parameters.AddWithValue("@EAffType", am.EAffType);

                    cmd.Parameters.AddWithValue("@ID", am.ID);// if ID=0 then Insert else Update
                    cmd.Parameters.AddWithValue("@APPNO", am.APPNO);
                    cmd.Parameters.AddWithValue("@SCHLNAME", am.SCHLNAME);
                    cmd.Parameters.AddWithValue("@SCHLMOBILE", am.SCHLMOBILE);
                    cmd.Parameters.AddWithValue("@SCHLEMAIL", am.SCHLEMAIL);
                    cmd.Parameters.AddWithValue("@PWD", am.PWD);
                    cmd.Parameters.AddWithValue("@SCHL", am.SCHL);
                    cmd.Parameters.AddWithValue("@DIST", am.DIST);
                    cmd.Parameters.AddWithValue("@UDISECODE", am.UDISECODE);
                    cmd.Parameters.AddWithValue("@SCHLNME", am.SCHLNME);
                    cmd.Parameters.AddWithValue("@SCHLNMP", am.SCHLNMP);
                    cmd.Parameters.AddWithValue("@STATIONE", am.STATIONE);
                    cmd.Parameters.AddWithValue("@STATIONP", am.STATIONP);
                    cmd.Parameters.AddWithValue("@ADDRESSE", am.ADDRESSE);
                    cmd.Parameters.AddWithValue("@ADDRESSP", am.ADDRESSP);
                    cmd.Parameters.AddWithValue("@DISTNME", am.DISTNME);
                    cmd.Parameters.AddWithValue("@PINCODE", am.PINCODE);
                    cmd.Parameters.AddWithValue("@TehsilCode", am.TehsilCode);
                    cmd.Parameters.AddWithValue("@PostOfficeCode", am.PostOfficeCode);
                    cmd.Parameters.AddWithValue("@EducationType", am.EducationType);
                    cmd.Parameters.AddWithValue("@Area", am.Area);
                    cmd.Parameters.AddWithValue("@PrincipalName", am.PrincipalName);
                    cmd.Parameters.AddWithValue("@Qualification", am.Qualification);
                    cmd.Parameters.AddWithValue("@DOJ", am.DOJ);
                    cmd.Parameters.AddWithValue("@OtherContactPerson", am.OtherContactPerson);
                    cmd.Parameters.AddWithValue("@DOB", am.DOB);
                    cmd.Parameters.AddWithValue("@Experience", am.Experience);
                    cmd.Parameters.AddWithValue("@StdCode", am.StdCode);
                    cmd.Parameters.AddWithValue("@PrincipalMobileNo", am.PrincipalMobileNo);
                    cmd.Parameters.AddWithValue("@SocietyName", am.SocietyName);
                    cmd.Parameters.AddWithValue("@SocietyRegNo", am.SocietyRegNo);         
                    cmd.Parameters.AddWithValue("@SocietyRegDate", am.SocietyRegDate);
                    cmd.Parameters.AddWithValue("@SocietyNOM", am.SocietyNOM);
                    cmd.Parameters.AddWithValue("@SocietyChairmanName", am.SocietyChairmanName);
                    cmd.Parameters.AddWithValue("@SocietyChairmanMobile", am.SocietyChairmanMobile);
                    cmd.Parameters.AddWithValue("@SocietyFile", am.SocietyFile);
                    cmd.Parameters.AddWithValue("@BSFROM", am.BSFROM);
                    cmd.Parameters.AddWithValue("@BSTO", am.BSTO);
                    cmd.Parameters.AddWithValue("@BSIA", am.BSIA);
                    cmd.Parameters.AddWithValue("@BSMEMO", am.BSMEMO);
                    cmd.Parameters.AddWithValue("@BSIDATE", am.BSIDATE);
                    cmd.Parameters.AddWithValue("@BSFILE", am.BSFILE);
                    cmd.Parameters.AddWithValue("@FSFROM", am.FSFROM);
                    cmd.Parameters.AddWithValue("@FSTO", am.FSTO);
                    cmd.Parameters.AddWithValue("@FSIA", am.FSIA);
                    cmd.Parameters.AddWithValue("@FSMEMO", am.FSMEMO);
                    cmd.Parameters.AddWithValue("@FSIDATE", am.FSIDATE);
                    cmd.Parameters.AddWithValue("@FSFILE", am.FSFILE);
                    cmd.Parameters.AddWithValue("@MAPNAME", am.MAPNAME);
                    cmd.Parameters.AddWithValue("@MAPREGNO", am.MAPREGNO);
                    cmd.Parameters.AddWithValue("@MAPAUTH", am.MAPAUTH);
                    cmd.Parameters.AddWithValue("@MAPMEMO", am.MAPMEMO);
                    cmd.Parameters.AddWithValue("@MAPIDATE", am.MAPIDATE);
                    cmd.Parameters.AddWithValue("@MAPFILE", am.MAPFILE);
                    //
                    cmd.Parameters.AddWithValue("@CLUAUTH", am.CLUAUTH);
                    cmd.Parameters.AddWithValue("@CLUMEMO", am.CLUMEMO);
                    cmd.Parameters.AddWithValue("@CLUIDATE", am.CLUIDATE);
                    cmd.Parameters.AddWithValue("@CLUFILE", am.CLUFILE);

                    cmd.Parameters.AddWithValue("@C1B", am.C1B);
                    cmd.Parameters.AddWithValue("@C1G", am.C1G);
                    cmd.Parameters.AddWithValue("@C1T", am.C1T);
                    cmd.Parameters.AddWithValue("@C2B", am.C2B);
                    cmd.Parameters.AddWithValue("@C2G", am.C2G);
                    cmd.Parameters.AddWithValue("@C2T", am.C2T);
                    cmd.Parameters.AddWithValue("@C3B", am.C3B);
                    cmd.Parameters.AddWithValue("@C3G", am.C3G);
                    cmd.Parameters.AddWithValue("@C3T", am.C3T);
                    cmd.Parameters.AddWithValue("@C4B", am.C4B);
                    cmd.Parameters.AddWithValue("@C4G", am.C4G);
                    cmd.Parameters.AddWithValue("@C4T", am.C4T);
                    cmd.Parameters.AddWithValue("@C5B", am.C5B);
                    cmd.Parameters.AddWithValue("@C5G", am.C5G);
                    cmd.Parameters.AddWithValue("@C5T", am.C5T);
                    cmd.Parameters.AddWithValue("@C6B", am.C6B);
                    cmd.Parameters.AddWithValue("@C6G", am.C6G);
                    cmd.Parameters.AddWithValue("@C6T", am.C6T);
                    cmd.Parameters.AddWithValue("@C7B", am.C7B);
                    cmd.Parameters.AddWithValue("@C7G", am.C7G);
                    cmd.Parameters.AddWithValue("@C7T", am.C7T);
                    cmd.Parameters.AddWithValue("@C8B", am.C8B);
                    cmd.Parameters.AddWithValue("@C8G", am.C8G);
                    cmd.Parameters.AddWithValue("@C8T", am.C8T);
                    cmd.Parameters.AddWithValue("@C9B", am.C9B);
                    cmd.Parameters.AddWithValue("@C9G", am.C9G);
                    cmd.Parameters.AddWithValue("@C9T", am.C9T);
                    cmd.Parameters.AddWithValue("@C10B", am.C10B);
                    cmd.Parameters.AddWithValue("@C10G", am.C10G);
                    cmd.Parameters.AddWithValue("@C10T", am.C10T);
                    cmd.Parameters.AddWithValue("@C11HB", am.C11HB);
                    cmd.Parameters.AddWithValue("@C11HG", am.C11HG);
                    cmd.Parameters.AddWithValue("@C11HT", am.C11HT);
                    cmd.Parameters.AddWithValue("@C11SB", am.C11SB);
                    cmd.Parameters.AddWithValue("@C11SG", am.C11SG);
                    cmd.Parameters.AddWithValue("@C11ST", am.C11ST);
                    cmd.Parameters.AddWithValue("@C11CB", am.C11CB);
                    cmd.Parameters.AddWithValue("@C11CG", am.C11CG);
                    cmd.Parameters.AddWithValue("@C11CT", am.C11CT);
                    cmd.Parameters.AddWithValue("@C11VB", am.C11VB);
                    cmd.Parameters.AddWithValue("@C11VG", am.C11VG);
                    cmd.Parameters.AddWithValue("@C11VT", am.C11VT);
                    cmd.Parameters.AddWithValue("@C12HB", am.C12HB);
                    cmd.Parameters.AddWithValue("@C12HG", am.C12HG);
                    cmd.Parameters.AddWithValue("@C12HT", am.C12HT);
                    cmd.Parameters.AddWithValue("@C12SB", am.C12SB);
                    cmd.Parameters.AddWithValue("@C12SG", am.C12SG);
                    cmd.Parameters.AddWithValue("@C12ST", am.C12ST);
                    cmd.Parameters.AddWithValue("@C12CB", am.C12CB);
                    cmd.Parameters.AddWithValue("@C12CG", am.C12CG);
                    cmd.Parameters.AddWithValue("@C12CT", am.C12CT);
                    cmd.Parameters.AddWithValue("@C12VB", am.C12VB);
                    cmd.Parameters.AddWithValue("@C12VG", am.C12VG);
                    cmd.Parameters.AddWithValue("@C12VT", am.C12VT);
                    cmd.Parameters.AddWithValue("@OSTUDYMEDIUM", am.OSTUDYMEDIUM);
                    cmd.Parameters.AddWithValue("@OLANDTYPE", am.OLANDTYPE);
                    cmd.Parameters.AddWithValue("@OTOTALAREA", am.OTOTALAREA);
                    cmd.Parameters.AddWithValue("@OCOVAREA", am.OCOVAREA);
                    cmd.Parameters.AddWithValue("@OPLAYGROUNDSIZE", am.OPLAYGROUNDSIZE);
                    cmd.Parameters.AddWithValue("@OCOURT", am.OCOURT);
                    cmd.Parameters.AddWithValue("@OTRANSPORT", am.OTRANSPORT);
                    cmd.Parameters.AddWithValue("@OSALARYEMP", am.OSALARYEMP);
                    cmd.Parameters.AddWithValue("@OROWATER", am.OROWATER);
                    cmd.Parameters.AddWithValue("@OTOILET", am.OTOILET);
                    cmd.Parameters.AddWithValue("@OPLAYGROUND", am.OPLAYGROUND);
                    cmd.Parameters.AddWithValue("@OBOARDSPLCAND", am.OBOARDSPLCAND);
                    cmd.Parameters.AddWithValue("@OCOMPLAB", am.OCOMPLAB);
                    cmd.Parameters.AddWithValue("@OSCILAB", am.OSCILAB);
                    cmd.Parameters.AddWithValue("@OINTERNET", am.OINTERNET);
                    cmd.Parameters.AddWithValue("@OSMARTCLS", am.OSMARTCLS);
                    cmd.Parameters.AddWithValue("@OFIRESAFE", am.OFIRESAFE);
                    cmd.Parameters.AddWithValue("@OFURNITURE", am.OFURNITURE);
                    cmd.Parameters.AddWithValue("@ONOCLASSROOMS", am.ONOCLASSROOMS);
                    cmd.Parameters.AddWithValue("@OActivitiesAchievements", am.OActivitiesAchievements);
                    cmd.Parameters.AddWithValue("@ISACTIVE", am.ISACTIVE);
                    cmd.Parameters.AddWithValue("@UpdatedBy", am.UpdatedBy);
                    cmd.Parameters.AddWithValue("@Remarks", am.Remarks);
                    //
                    cmd.Parameters.AddWithValue("@PHONE", am.PHONE);
                    cmd.Parameters.AddWithValue("@Islibrary", am.Islibrary);
                    cmd.Parameters.AddWithValue("@IsPhysics", am.IsPhysics);
                    cmd.Parameters.AddWithValue("@IsChemistry", am.IsChemistry);
                    cmd.Parameters.AddWithValue("@IsBiology", am.IsBiology);
                    cmd.Parameters.AddWithValue("@ObjectionLetter", am.ObjectionLetter);
                    cmd.Parameters.AddWithValue("@InspectionReport", am.InspectionReport);
                    cmd.Parameters.AddWithValue("@IsCCTV", am.IsCCTV);
                    cmd.Parameters.AddWithValue("@EmpUserId", am.EmpUserId);

                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                    return result;
                }
            }
            catch (Exception ex)
            {
                OutError = "-1";
                return result = -1;
            }
            finally
            {
                // con.Close();
            }
        }


        public DataSet GetEAffiliationPaymentDetails(string APPNO)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("EAffiliationPaymentDetailsSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@appno", APPNO);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public DataSet InsertEAffiliationPaymentDetails(EAffiliationPaymentDetailsModel SA, int type, out string OutError)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("InsertEAffiliationPaymentDetailsSP", con);//
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", type);
                    cmd.Parameters.AddWithValue("@id", SA.id);
                    cmd.Parameters.AddWithValue("@APPNO", SA.APPNO);
                    cmd.Parameters.AddWithValue("@refno", SA.refno);
                    cmd.Parameters.AddWithValue("@class", SA.cls);
                    cmd.Parameters.AddWithValue("@exam", SA.exam);                   
                    cmd.Parameters.AddWithValue("@fee", SA.fee);
                    cmd.Parameters.AddWithValue("@latefee", SA.latefee);
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                    return result;
                }
            }
            catch (Exception ex)
            {
                OutError = ex.Message;
                return result = null;
            }
            finally
            {
                // con.Close();
            }
        }


        public DataSet GetEAffiliationPayment(string appno)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetEAffiliationPaymentSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@appno", appno);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public DataSet GetEAffiliationStaffDetails(string search, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetEAffiliationStaffDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
                    cmd.Parameters.AddWithValue("@PageSize", 20);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }


        public int InsertEAffiliationStaffDetails(EAffiliationStaffDetailsModel SA, int action, out string OutError)
        {
            int result;
            OutError = "0";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("InsertEAffiliationStaffDetailsSP", con);//
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", action);
                    cmd.Parameters.AddWithValue("@eStaffId", SA.eStaffId);
                    cmd.Parameters.AddWithValue("@APPNO", SA.APPNO);
                    cmd.Parameters.AddWithValue("@Name", SA.Name);
                    cmd.Parameters.AddWithValue("@FName", SA.FName);
                    cmd.Parameters.AddWithValue("@DOB", SA.DOB);
                    cmd.Parameters.AddWithValue("@Gender", SA.Gender);
                    cmd.Parameters.AddWithValue("@AadharNo", SA.AadharNo);
                    cmd.Parameters.AddWithValue("@Qualification", SA.Qualification);
                    cmd.Parameters.AddWithValue("@Cadre", SA.Cadre);
                    cmd.Parameters.AddWithValue("@Subject", SA.Subject);
                    cmd.Parameters.AddWithValue("@ExpYear", SA.ExpYear);
                    cmd.Parameters.AddWithValue("@ExpMonth", SA.ExpMonth);
                    cmd.Parameters.AddWithValue("@MOBILENO", SA.MOBILENO);
                    cmd.Parameters.AddWithValue("@Salary", SA.Salary);
                    cmd.Parameters.AddWithValue("@SalaryMode", SA.SalaryMode);
                    cmd.Parameters.AddWithValue("@StaffFile", SA.StaffFile);
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                    return result;
                }
            }
            catch (Exception ex)
            {
                OutError = ex.Message;
                return result = -1;
            }
            finally
            {
                // con.Close();
            }
        }



        public int mailer(string email, string app_no, string pwd, string name)
        {

            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress(email));
            msg.Subject = "Confirm Your Registration - PSEB";
            msg.From = new MailAddress("noreply@psebonline.in", "psebonline.in");
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("ਪਿਆਰੇ <strong>" + name.ToUpper() + "</strong>");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("ਪੰਜਾਬ ਸਕੂਲ ਸਿੱਖਿਆ ਬੋਰਡ, ਓਪਨ ਸਕੂਲ ਪ੍ਰਣਾਲੀ ਅਧੀਨ ਆਨਲਾਈਨ ਦਾਖਲਾ/ਰਜਿਸਟਰੇਸ਼ਨ ਕਰਨ ਲਈ ਤੁਹਾਡਾ ਖਾਤਾ ਸਫਲਤਾਪੂਰਕ ਖੁਲ ਗਿਆ ਹੈ ਲਾਗਇਨ ਕਰਨ ਲਈ ਵੇਰਵੇ ਦਿੱਤੇ ਗਏ ਹਨ : -<br/>");
            sb.AppendLine("<center><table style='font-size:1.1em;font-weight:500;font-style:copperplate-gothic;'><tr><td>ਬਿਨੇ ਪੱਤਰ ਨੰ. / ਲਾਗਇਨ ਆਈ. ਡੀ</td><td>" + app_no + "</td></tr><tr><td>ਪਾਸਵਰਡ </td><td>" + pwd + "</td></tr></table>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<a href='https://registration2021.pseb.ac.in/Open'>ਲਾਗਇਨ ਕਰਨ ਲਈ ਇੱਥੇ ਕਲਿੱਕ ਕਰੋ</a></center><br/>");
            sb.AppendLine("<strong>ਜਰੂਰੀ ਨੋਟ :-</strong>");
            sb.AppendLine("<br/>");
            sb.AppendLine("1 ਆਨਲਾਇਨ ਫਾਰਮ ਭਰਨ ਤੋ ਪਹਿਲਾ ਬੋਰਡ ਵਲੋ ਜਾਰੀ ਦਾਖਲੇ ਸਬੰਧੀ ਹਦਾਇਤਾ ਜਰੁਰ ਪੜ ਲਈਆ ਜਾਣ ਅਤੇ ਫਾਰਮ ਭਰਨ ਸਮੇ ਵੇਰਵਿਆ ਨੂੰ ਧਿਆਨ ਨਾਲ ਚੈਕ ਕਰਕੇ ਹੀ ਸਬਮਿਟ ਕੀਤਾ ਜਾਵੇ, ਇਕ ਵਾਰ ਸਬਮਿਟ ਕਰਨ ਤੋ ਬਾਅਦ ਕੋਈ ਵੀ ਸੋਧ ਨਹੀ ਕੀਤੀ ਜਾ ਸਕੇਗੀ। ਇਸ ਸਬੰਧੀ ਸਾਰੀ ਜਿਮੇਵਾਰੀ ਪ੍ਰੀਖਿਆਰਥੀ ਦੀ ਹੋਵੇਗੀ।");
            sb.AppendLine("<br/>");
            sb.AppendLine("2 ਇਹ ਇੱਕ ਕੰਪਿਊਟਰ ਸਿਸਟਮ ਦੁਆਰਾ ਤਿਆਰ ਕੀਤੀ ਈ - ਮੇਲ ਹੈ ਅਤੇ ਇਸ ਈ - ਮੇਲ ਤੇ ਮੇਲ ਨਾ ਕੀਤੀ ਜਾਵੇ । ਕਿਰਪਾ ਕਰਕੇ ਇਸ ਈ - ਮੇਲ noreply@psebonline.in ਨੂੰ White List / Safe Sender List ਵਿੱਚ ਸ਼ਾਮਲ ਕਰੋ ਨਹੀ ਤਾ ISP (ਇੰਟਰਨੈੱਟ ਸਰਵਿਸ ਪ੍ਰੋਵਾਈਡਰ ) ਰਾਹੀ ਈ -ਮੇਲ ਪ੍ਰਾਪਤ ਕਰਨ ਤੋ ਰੋਕ ਲਗ ਸਕਦੀ ਹੈ.");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("<br/>ਤੁਹਾਡੀ ਸੇਵਾ ਵਿਚ,");
            sb.AppendLine("<br/>ਈ - ਓਪਨ ਸਕੂਲ ਟੀਮ ,");
            sb.AppendLine("<br/>ਪੰਜਾਬ ਸਕੂਲ ਸਿੱਖਿਆ ਬੋਰਡ");
            sb.AppendLine("<br/>ਮੋਹਾਲੀ।");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("ਓਪਨ ਸਕੂਲ ਸਬੰਧੀ ਕਿਸੇ ਵੀ ਜਾਣਕਾਰੀ ਲਈ ਸਾਡੇ ਨਾਲ ਸੰਪਰਕ ਕਰੋ : 0172-5227195, 5227197");
            sb.AppendLine("<br/><hr/>");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("Dear <strong>" + name.ToUpper() + "</strong>");
            sb.AppendLine("<br/><br/>");
            sb.AppendLine("Your Account For Punjab School Education Open School Online Registration has been Generated Successfully.<br/>");
            sb.AppendLine("Your Login Details are given Below :-");
            sb.AppendLine("<center><table style='font-size:1.1em;font-weight:500;'><tr><td><b>User ID / Login ID: </b></td><td>" + app_no + "</td></tr><tr><td><b>Password: </b></td><td>" + pwd + "</td></tr></table>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<a href='https://registration2021.pseb.ac.in/Open'><b>Click Here To Login</b></a></center><br/>");
            sb.AppendLine("<strong>Note:</strong> Please Read Instruction Carefully Before filling the Online Form");
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>This is a system generated e-mail and please do not reply. Add noreply@psebonline.in to your white list / safe sender list. Else, your mailbox filter or ISP (Internet Service Provider) may stop you from receiving e-mails.");
            sb.AppendLine("<br/>");
            sb.AppendLine("<br/>Regards,");
            sb.AppendLine("<br/>e-Open School Team,");
            sb.AppendLine("<br/>Punjab School Education Board");
            sb.AppendLine("<br/>Contact Us: 0172-5227195, 5227197");

            msg.IsBodyHtml = true;
            msg.Body = sb.ToString();

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "mail.smtp2go.com";
            smtp.Port = 2525;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("noreply@psebonline.in", "YWZtam9qZWtrNHRr");
            smtp.EnableSsl = true;
            smtp.Send(msg);

            return 0;
        }


        public string IsValidForChallan(string app_id)
        {
            string res = string.Empty;
            EAffiliationModel eam = GetEAffiliation(app_id);
               
                if (string.IsNullOrEmpty(eam.SCHLNME.Trim()) || string.IsNullOrEmpty(eam.SCHLNMP.Trim()) 
                || string.IsNullOrEmpty(eam.PINCODE.Trim()) || string.IsNullOrEmpty(eam.DIST.Trim()) || string.IsNullOrEmpty(eam.TehsilCode.Trim())
                || string.IsNullOrEmpty(eam.PrincipalName.Trim()) || string.IsNullOrEmpty(eam.Experience.Trim()) || string.IsNullOrEmpty(eam.DOJ.Trim())
                || string.IsNullOrEmpty(eam.PrincipalMobileNo.Trim()) || string.IsNullOrEmpty(eam.Area.Trim()) )
            {
                res += "Please fill all mandatory fields in School Profile, ";
            }
            if (string.IsNullOrEmpty(eam.SocietyName.Trim())|| string.IsNullOrEmpty(eam.SocietyChairmanName.Trim()) || string.IsNullOrEmpty(eam.SocietyChairmanMobile.Trim()) 
                || string.IsNullOrEmpty(eam.BSFROM.Trim()) || string.IsNullOrEmpty(eam.BSTO.Trim()) || string.IsNullOrEmpty(eam.BSIA.Trim())
                || string.IsNullOrEmpty(eam.FSFROM.Trim()) || string.IsNullOrEmpty(eam.FSTO.Trim()) || string.IsNullOrEmpty(eam.FSIA.Trim()) 
                || string.IsNullOrEmpty(eam.MAPREGNO.Trim())  || string.IsNullOrEmpty(eam.MAPNAME.Trim()) || string.IsNullOrEmpty(eam.MAPAUTH.Trim()))
            {
                res += "Please fill all mandatory fields in Society/Building/Fire/Map Details, ";
            }
            if (eam.C1T < 0 || eam.C2T < 0 || eam.C3T < 0 || eam.C4T < 0 )
            {
                res += "Please fill all mandatory fields in Student Details,  ";
            }
            if (string.IsNullOrEmpty(eam.OSTUDYMEDIUM.Trim()) || string.IsNullOrEmpty(eam.OCOURT.Trim())
                || string.IsNullOrEmpty(eam.OTRANSPORT.Trim()) || string.IsNullOrEmpty(eam.OSALARYEMP.Trim()) || string.IsNullOrEmpty(eam.OROWATER.Trim())
                || string.IsNullOrEmpty(eam.OTOILET.Trim()) || string.IsNullOrEmpty(eam.OPLAYGROUND.Trim()) || string.IsNullOrEmpty(eam.OBOARDSPLCAND.Trim())
                || string.IsNullOrEmpty(eam.OCOMPLAB.Trim()) || string.IsNullOrEmpty(eam.OSCILAB.Trim()) || string.IsNullOrEmpty(eam.OINTERNET.Trim())
                || string.IsNullOrEmpty(eam.OSMARTCLS.Trim()) || string.IsNullOrEmpty(eam.OFIRESAFE.Trim())
                        || string.IsNullOrEmpty(eam.IsBiology) || string.IsNullOrEmpty(eam.IsChemistry)
                                || string.IsNullOrEmpty(eam.IsPhysics) || string.IsNullOrEmpty(eam.Islibrary)

                  || string.IsNullOrEmpty(eam.OFURNITURE.Trim()) || string.IsNullOrEmpty(eam.ONOCLASSROOMS.Trim()) )
            {
                res += "Please fill all mandatory fields in Other Information, ";
            }

            // Document is not mandatory

            //if (string.IsNullOrEmpty(eam.SocietyFile)) { res += " Society Registration Document, "; }
            //if (string.IsNullOrEmpty(eam.BSFILE)) { res += " Building Safety Document, "; }
            //if (string.IsNullOrEmpty(eam.FSFILE)) { res += " Fire Safety Document, "; }
            //if (string.IsNullOrEmpty(eam.MAPFILE)) { res += " Building Map Document, "; }

            //if (eam.EAffClass == "10" || eam.EAffClass == "12")
            //{
            //    if (string.IsNullOrEmpty(eam.PrincipalExperienceFile)) { res += "Principal Experience Document, "; }

            //}          


            //if (string.IsNullOrEmpty(eam.OCOURT) && eam.OCOURT == "YES")
            //{
            //    res += "Court Case Document, ";
            //}

            string Search = "a.APPNO = '" + app_id + "' ";
            DataSet StoreAllData = GetEAffiliationStaffDetails(Search, 1);
            if (StoreAllData == null || StoreAllData.Tables[0].Rows.Count == 0)
            {
                res += "Please fill atleast 1 Staff Details, ";
            }             
            return res;
        }

        public List<EAffiliationDocumentMaster> EAffiliationDocumentMasterList(DataTable dataTable)
        {
            List<EAffiliationDocumentMaster> item = new List<EAffiliationDocumentMaster>();
            foreach (System.Data.DataRow dr in dataTable.Rows)
            {
                item.Add(new EAffiliationDocumentMaster { DocumentName = @dr["DocumentName"].ToString().Trim(), DocID = Convert.ToInt32(@dr["DocID"].ToString()) });
            }
            return item;
        }

        public DataSet GetEAffiliationDocumentDetails(int type,int eDocId, string appno, string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetEAffiliationDocumentDetailsSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@eDocId", eDocId);
                    cmd.Parameters.AddWithValue("@appno", appno);
                    cmd.Parameters.AddWithValue("@search", search);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int InsertEAffiliationDocumentDetails(EAffiliationDocumentDetailsModel model, int action, out string OutError)
        {
            int result;
            OutError = "0";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("InsertEAffiliationDocumentDetailsSP", con);//
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", action);
                    cmd.Parameters.AddWithValue("@eDocId", model.eDocId);
                    cmd.Parameters.AddWithValue("@APPNO", model.APPNO);
                    cmd.Parameters.AddWithValue("@DocID", model.DocID);
                    cmd.Parameters.AddWithValue("@DocFile", model.DocFile);                
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                    return result;
                }
            }
            catch (Exception ex)
            {
                OutError = ex.Message;
                return result = -1;
            }
            finally
            {
                // con.Close();
            }
        }



        public string EAffiliationForward(string EmpUserId, string ApplicationType,string ForwardList, string ApplicationStatus,string Remarks, int AdminId, out string OutError)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("EAffiliationForward", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                cmd.Parameters.AddWithValue("@ApplicationType", ApplicationType);
                cmd.Parameters.AddWithValue("@ForwardList", ForwardList);
                cmd.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
                cmd.Parameters.AddWithValue("@Remarks", Remarks);
                cmd.Parameters.AddWithValue("@AdminId", AdminId);
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                OutError = (string)cmd.Parameters["@OutError"].Value;
                return OutError;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                OutError = "0";
                return result = "-1";
            }
            finally
            {
                con.Close();
            }
        }



        public int UpdateSchoolMasterEAffiliation(EAffiliationSchoolMaster am,string EmpUserId, int AdminId, int Type, out string OutError)
        {
            int result;
            OutError = "0";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UpdateSchoolMasterEAffiliationSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.AddWithValue("@AppType", am.AppType);
                    cmd.Parameters.AddWithValue("@AppNo", am.AppNo);
                    cmd.Parameters.AddWithValue("@SchlCat", am.SchlCat);
                    cmd.Parameters.AddWithValue("@SCHL", am.SCHL);
                    cmd.Parameters.AddWithValue("@SCHLE", am.SCHLE);
                    cmd.Parameters.AddWithValue("@SCHLP", am.SCHLP);
                    cmd.Parameters.AddWithValue("@STATIONE", am.STATIONE);
                    cmd.Parameters.AddWithValue("@STATIONP", am.STATIONP);
                    cmd.Parameters.AddWithValue("@REMARKS", am.REMARKS);
                    cmd.Parameters.AddWithValue("@AdminId", AdminId);
                    cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);

                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                    return result;
                }
            }
            catch (Exception ex)
            {
                OutError = "-1";
                return result = -1;
            }
            finally
            {
                // con.Close();
            }
        }

        public DataSet GetAllApprovedeDocumentAgainstEaffiliationApplication(string Apptype, string appno)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAllApprovedeDocumentAgainstEaffiliationApplicationSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Apptype", Apptype);
                    cmd.Parameters.AddWithValue("@appno", appno);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // New 150322
        public static DataSet EAffiliation_AppType_Approval(string AppType, string AppNo, string ApprovalStatus, string ApprovalRemarks, string ApprovalBy, string ApprovalFileNo,
            string ApprovalIP, string EmpUserId, out string OutError)
        {
            DataSet result = new DataSet();
            try
            {
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "EAffiliation_AppType_ApprovalSP";
                cmd.Parameters.AddWithValue("@AppType", AppType);
                cmd.Parameters.AddWithValue("@AppNo", AppNo);
                cmd.Parameters.AddWithValue("@ApprovalStatus", ApprovalStatus);
                cmd.Parameters.AddWithValue("@ApprovalRemarks", ApprovalRemarks);
                cmd.Parameters.AddWithValue("@ApprovalBy", ApprovalBy);
                cmd.Parameters.AddWithValue("@ApprovalFileNo", ApprovalFileNo);
                cmd.Parameters.AddWithValue("@ApprovalIP", ApprovalIP);
                cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                result = db.ExecuteDataSet(cmd);
                OutError = (string)cmd.Parameters["@OutError"].Value;
                return result;
            }
            catch (Exception)
            {
                OutError = "-1";
                return null;
            }

        }


        public DataSet EAffiliationDashBoardSP(string AdminUser, string AdminType, string search, int type)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("EAffiliationDashBoardSP", con);  //SelectPrintList_sp
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AdminUser", AdminUser);
                    cmd.Parameters.AddWithValue("@AdminType", AdminType);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@search", search);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = null;
            }
        }
    }
}