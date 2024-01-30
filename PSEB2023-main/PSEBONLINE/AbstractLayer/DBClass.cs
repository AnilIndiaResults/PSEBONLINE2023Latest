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
using System.Data.Entity;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace PSEBONLINE.AbstractLayer
{
    public class DBClass
    {
        private readonly DBContext _context = new DBContext();
        #region Check ConString
        private string CommonCon = "myDBConnection";
        public DBClass()
        {
            CommonCon = "myDBConnection";
        }

        public static List<SelectListItem> GetCorrectionTypeByClass(string cls)
        {
            List<SelectListItem> SecList = new List<SelectListItem>();
            string class1 = cls == "1" ? "9" : cls == "2" ? "11" : cls == "10" ? "10" : cls == "22" ? "10" : cls == "12" ? "12" : cls == "44" ? "12" : "0";
            if (class1 == "9")
            {
                SecList.Add(new SelectListItem { Text = "Particular", Value = "1" });
                // SecList.Add(new SelectListItem { Text = "Subject", Value = "2" });
                SecList.Add(new SelectListItem { Text = "Image", Value = "4" });
            }
            else if (class1 == "11")
            {
                SecList.Add(new SelectListItem { Text = "Particular", Value = "1" });
                // SecList.Add(new SelectListItem { Text = "Subject", Value = "2" });
                //SecList.Add(new SelectListItem { Text = "Image", Value = "4" });
            }
            else if (class1 == "10")
            {
                SecList.Add(new SelectListItem { Text = "Particular", Value = "1" });
                SecList.Add(new SelectListItem { Text = "Subject", Value = "2" });
               // SecList.Add(new SelectListItem { Text = "Image", Value = "4" });
            }
            else if (class1 == "12")
            {
                SecList.Add(new SelectListItem { Text = "Particular", Value = "1" });
                //SecList.Add(new SelectListItem { Text = "Subject", Value = "2" });
                // SecList.Add(new SelectListItem { Text = "Image", Value = "4" });
            }
            return SecList;
        }

        public List<ExamCategoryMasters> GetExamCategoryMastersActiveList()
        {
            return _context.ExamCategoryMasters.Where(s => s.IsActive == true).ToList();
        }
        public static List<SelectListItem> GetRPTypeRegOpenOnly()
        {
            List<SelectListItem> itemOrder = new List<SelectListItem>();
            itemOrder.Add(new SelectListItem { Text = "Regular", Value = "R" });
            itemOrder.Add(new SelectListItem { Text = "Open", Value = "O" });
            itemOrder.Add(new SelectListItem { Text = "Private", Value = "P" });
            return itemOrder;
        }
        public static List<SelectListItem> GetCorrectionClassAssignListByAdminId(string ClassAssign)
        {
            List<SelectListItem> itemsch = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(ClassAssign))
            {
                if (ClassAssign.ToString().Contains("12"))
                {
                    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Regular", Value = "4" });
                    itemsch.Add(new SelectListItem { Text = "Sr.Secondary Open", Value = "44" });
                }
                if (ClassAssign.ToString().Contains("10"))
                {
                    itemsch.Add(new SelectListItem { Text = "Matriculation Regular", Value = "2" });
                    itemsch.Add(new SelectListItem { Text = "Matriculation Open", Value = "22" });
                }
                if (ClassAssign.ToString().Contains("11"))
                {
                    itemsch.Add(new SelectListItem { Text = "11th Class", Value = "11" });
                }
                if (ClassAssign.ToString().Contains("9"))
                {
                    itemsch.Add(new SelectListItem { Text = "9th Class", Value = "9" });
                }
                if (ClassAssign.ToString().Contains("5"))
                {
                    itemsch.Add(new SelectListItem { Text = "Primary", Value = "5" });
                }
                if (ClassAssign.ToString().Contains("8"))
                {
                    itemsch.Add(new SelectListItem { Text = "Middle", Value = "8" });
                }
            }
           return  itemsch.ToList();
        }

            public static string GetSubjectNameBysubCode(string subCode)
        {
            string SubNM = "";
            if (subCode.Length == 2)
            {
                DataSet dsSub = new AbstractLayer.RegistrationDB().GetNAmeAndAbbrbySub(subCode.ToString());
                if (dsSub.Tables[0].Rows.Count > 0)
                {
                    SubNM = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                }
            }
            else if (subCode.Length == 3)
            {
                DataSet dsSub = new AbstractLayer.RegistrationDB().GetNAmeAndAbbrbySubFromSSE(subCode.ToString());
                if (dsSub.Tables[0].Rows.Count > 0)
                {
                    SubNM = dsSub.Tables[0].Rows[0]["NAME_ENG"].ToString();
                }
            }            
            return SubNM;
        }

        public static string GetClassNameByClass(string Cls)
        {
            string ClsName = Cls == "5" ? "Primary" : Cls == "8" ? "Middle" :
                 Cls == "9" ? "Ninth" : Cls == "10" ? "Matric" :
                 Cls == "11" ? "Eleventh" : Cls == "12" ? "Senior" : "";
            return ClsName;
        }
        public static string GetClassByName(string ClsName)
        {
            string cls = ClsName == "Primary" ? "5" : ClsName == "Middle" ? "8" :
                 ClsName == "Ninth" ? "9"  : ClsName == "Matric" ? "10"  :
                 ClsName == "Eleventh" ? "11" :  ClsName == "Senior" ? "12" : "";

            return cls;
        }
        public static List<SelectListItem> GetAllMatricSubjectsForMarks()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            try
            {
                DataSet ds = new DataSet();
                Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetAllMatricSubjectsForMarks"; //InsertOnlinePaymentMISSP // InsertOnlinePaymentMISSPNEW
                ds = db.ExecuteDataSet(cmd);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return subjects;
        }

        public static List<SelectListItem> GetAllMatricSubjects()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            try
            {
                DataSet ds = new DataSet();
                Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select sub,name_eng from matric order by sub";
                ds = db.ExecuteDataSet(cmd);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                    }
                }
                return subjects;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static List<SelectListItem> GetAllSeniorSubjects()
        {
            List<SelectListItem> subjects = new List<SelectListItem>();
            try
            {
                DataSet ds = new DataSet();
                Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select sub,name_eng from ssnew order by sub";
                ds = db.ExecuteDataSet(cmd);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        subjects.Add(new SelectListItem() { Text = dr["name_eng"].ToString(), Value = dr["sub"].ToString() });
                    }
                }
                return subjects;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string GenerateOTP()
        {
            string _numbers = "0123456789";
            StringBuilder builder = new StringBuilder(6);
            string numberAsString = "";
            Random random = new Random();
            for (var i = 0; i < 6; i++)
            {
                builder.Append(_numbers[random.Next(0, _numbers.Length)]);
            }
            numberAsString = builder.ToString();
            return numberAsString;
        }
        public static List<SelectListItem> GetRPType()
        {
            List<SelectListItem> itemOrder = new List<SelectListItem>();
            itemOrder.Add(new SelectListItem { Text = "Regular", Value = "R" });
            //itemOrder.Add(new SelectListItem { Text = "Open", Value = "O" });
            itemOrder.Add(new SelectListItem { Text = "Private", Value = "P" });
           // itemOrder.Add(new SelectListItem { Text = "Additional Subject", Value = "A" });
            return itemOrder;
        }

        public static List<SelectListItem> GetPvtUType()
        {
            List<SelectListItem> itemOrder = new List<SelectListItem>();
            itemOrder.Add(new SelectListItem { Text = "Additional Subject", Value = "A" });
            return itemOrder;
        }




        public List<SelectListItem> GetSubUserType()
        {
            DataTable dsSchool = GetAllSchoolType(); // passing Value to SchoolDB from model

            var itemSubUType = dsSchool.AsEnumerable().Where(s => s.Field<string>("subtype").ToString() == "Y")
            .Select(dataRow => new SelectListItem
            {
                Text = dataRow.Field<string>("schooltype").ToString(),
                Value = dataRow.Field<string>("abbr").ToString(),
            }).ToList();
            return itemSubUType.ToList();
        }

        public List<SelectListItem> GetEstalimentYearList()
        {
            List<SelectListItem> itemSession = new List<SelectListItem>();
            int currentYear = DateTime.Now.Year;
            int nextYear = currentYear + 1;
            int nextToNextYear = currentYear + 2;
            itemSession.Add(new SelectListItem { Text = nextYear.ToString() + "-" + nextToNextYear.ToString(), Value = nextYear.ToString() + "-" + nextToNextYear.ToString() });

            for (int i = currentYear; i > 1800; i--)
            {
                int a = i;
                int b = i + 1;
                itemSession.Add(new SelectListItem { Text = a.ToString() + "-" + b.ToString(), Value = a.ToString() + "-" + b.ToString() });
            }

          
           
            return itemSession;
        }

        public List<SelectListItem> GetEstalimentYearListSingle()
        {
            List<SelectListItem> itemSession = new List<SelectListItem>();
            int currentYear = DateTime.Now.Year;
            int nextYear = currentYear + 1;
            itemSession.Add(new SelectListItem { Text = nextYear.ToString(), Value = nextYear.ToString() });

            for (int i = currentYear; i > 1800; i--)
            {
                itemSession.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
            }
           return itemSession;
        }


      

        public List<SelectListItem> GetCountList()
        {
            List<SelectListItem> _list = new List<SelectListItem>();       
            for (int i = 0; i <= 12; i++)
            {
                if (i < 10)
                {
                    _list.Add(new SelectListItem { Text = "0" + i.ToString(), Value = "0" + i.ToString() });
                }
                else
                { _list.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() }); }
            }
            return _list;
        }


        public List<SelectListItem> GetOrderBy()
        {
            List<SelectListItem> itemOrder = new List<SelectListItem>();
            itemOrder.Add(new SelectListItem { Text = "Superintendent ", Value = "1" });
            itemOrder.Add(new SelectListItem { Text = "Assistant Secretary", Value = "2" });
            itemOrder.Add(new SelectListItem { Text = "Deputy Secretary", Value = "3" });
            itemOrder.Add(new SelectListItem { Text = "Director Computer", Value = "4" });
            itemOrder.Add(new SelectListItem { Text = "Secretary", Value = "5" });
            itemOrder.Add(new SelectListItem { Text = "Vice Chairman", Value = "6" });
            itemOrder.Add(new SelectListItem { Text = "Chairman", Value = "7" });
            return itemOrder;
        }

        public List<SelectListItem> GetAllResult()
        {
            List<SelectListItem> iList = new List<SelectListItem>();
            iList.Add(new SelectListItem { Text = "PASS", Value = "PASS" });
            iList.Add(new SelectListItem { Text = "FAIL", Value = "FAIL" });
            iList.Add(new SelectListItem { Text = "RE-APPEAR", Value = "RE-APPEAR" });
            iList.Add(new SelectListItem { Text = "ABSENT", Value = "ABSENT" });
            iList.Add(new SelectListItem { Text = "CANCEL", Value = "CANCEL" });
            return iList;
        }

        public List<SelectListItem> GetAllClass()
        {
            List<SelectListItem> classList = new List<SelectListItem>();
            classList.Add(new SelectListItem { Text = "5", Value = "5" });
            classList.Add(new SelectListItem { Text = "8", Value = "8" });
            classList.Add(new SelectListItem { Text = "10", Value = "10" });
            classList.Add(new SelectListItem { Text = "12", Value = "12" });
            return classList;
        }

        public DataSet SchoolDist(string schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SchoolDist_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@schl", schl);
                    //  cmd.Parameters.AddWithValue("@Schl", Schl);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception)
            {
                return result = null;
            }
        }
        #endregion  Check ConString




        public static string base64Decode(string sData) //Decode    
        {
            try
            {
                var encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();
                byte[] todecodeByte = Convert.FromBase64String(sData);
                int charCount = utf8Decode.GetCharCount(todecodeByte, 0, todecodeByte.Length);
                char[] decodedChar = new char[charCount];
                utf8Decode.GetChars(todecodeByte, 0, todecodeByte.Length, decodedChar, 0);
                string result = new String(decodedChar);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Decode" + ex.Message);
            }
        }

        public DataSet CheckBankAllowByFeeCodeDate(int feecode)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CheckBankAllowByFeeCodeDate", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@feecode", feecode);
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


        public static List<SelectListItem> GetAcceptRejectDDL()
        {
            List<SelectListItem> itemStatus = new List<SelectListItem>();
            itemStatus.Add(new SelectListItem { Text = "Accept", Value = "A" });
            itemStatus.Add(new SelectListItem { Text = "Reject", Value = "R" });
            itemStatus.Add(new SelectListItem { Text = "Cancel", Value = "C" });
            return itemStatus;
        }


        public List<SelectListItem> GetEAffType()
        {
            List<SelectListItem> itemStatus = new List<SelectListItem>();
            itemStatus.Add(new SelectListItem { Text = "Apply for Fresh Affiliation", Value = "NEW" });
            itemStatus.Add(new SelectListItem { Text = "Apply For Another Class/Stream", Value = "RENEW" });
            return itemStatus;
        }


        public List<SelectListItem> GetPropertyStatus()
        {
            List<SelectListItem> itemStatus = new List<SelectListItem>();
            itemStatus.Add(new SelectListItem { Text = "GOVT", Value = "GOVT" });
            itemStatus.Add(new SelectListItem { Text = "OWN", Value = "OWN" });
            itemStatus.Add(new SelectListItem { Text = "RENT", Value = "RENT" });
            itemStatus.Add(new SelectListItem { Text = "LEASE", Value = "LEASE" });
            return itemStatus;
        }

        public List<SelectListItem> GetPropertyFloor()
        {
            List<SelectListItem> itemStatus = new List<SelectListItem>();
            itemStatus.Add(new SelectListItem { Text = "Ground Floor", Value = "GF" });
            itemStatus.Add(new SelectListItem { Text = "First Floor", Value = "1F" });
            itemStatus.Add(new SelectListItem { Text = "Second Floor", Value = "2F" });
            itemStatus.Add(new SelectListItem { Text = "Third Floor", Value = "3F" });
            return itemStatus;
        }


        public List<SelectListItem> GetFeeCatRP(string RP)
        {
            DataSet dsFEECAT = Get_feeCodesRP(RP);
            // English
            List<SelectListItem> itemDist = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in dsFEECAT.Tables[0].Rows)
            {
                itemDist.Add(new SelectListItem { Text = @dr["FEECAT"].ToString().Trim(), Value = @dr["FEECAT"].ToString().Trim() });
            }
            return itemDist;
        }
        public DataSet Get_feeCodesRP(string RP)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("sp_Get_feeCodesRP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@RP", RP);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception)
            {
                return result = null;
            }
        }



        public List<SelectListItem> GetWritter()
        {
            List<SelectListItem> WantWritter = new List<SelectListItem>();
            WantWritter.Add(new SelectListItem { Text = "---SELECT--", Value = "0" });
            WantWritter.Add(new SelectListItem { Text = "NO", Value = "NO" });
            WantWritter.Add(new SelectListItem { Text = "SCRIBE", Value = "SCRIBE" });
            WantWritter.Add(new SelectListItem { Text = "READER", Value = "READER" });
            WantWritter.Add(new SelectListItem { Text = "LAB ASSISTANT", Value = "LAB ASSISTANT" });
            return WantWritter;
        }

        public List<SelectListItem> GetMatricResult()
        {
            List<SelectListItem> MatricResult = new List<SelectListItem>();
            MatricResult.Add(new SelectListItem { Text = "---Select Matric Result--", Value = "0" });
            MatricResult.Add(new SelectListItem { Text = "Pass", Value = "PASS" });
            MatricResult.Add(new SelectListItem { Text = "Re-Appear", Value = "RE-APPEAR" });
            //MatricResult.Add(new SelectListItem { Text = "Fail", Value = "FAIL" });
            //MatricResult.Add(new SelectListItem { Text = "Result Late Due To Complaint", Value = "RLC" });
            //MatricResult.Add(new SelectListItem { Text = "Result Late Due To Eligibility", Value = "RLE" });
            //MatricResult.Add(new SelectListItem { Text = "Result Late Due To Old Posting", Value = "RLP" });
            //MatricResult.Add(new SelectListItem { Text = "Result Late Due To Award Wanting", Value = "RLA" });
            //MatricResult.Add(new SelectListItem { Text = "ABS Absent", Value = "ABS" });
            //MatricResult.Add(new SelectListItem { Text = "Cancel", Value = "CAN" });
            //MatricResult.Add(new SelectListItem { Text = "Unfair Means Cases", Value = "UMC" });
            //MatricResult.Add(new SelectListItem { Text = "Previous Result Stand", Value = "PRS" });
            //MatricResult.Add(new SelectListItem { Text = "Result Late Due To INA Wanting", Value = "RLI" });
            //MatricResult.Add(new SelectListItem { Text = "Result Late Due To Grade Wanting", Value = "RLG" });
            return MatricResult;
        }
        public List<SelectListItem> GetAdminUser()
        {
            List<SelectListItem> adminUser = new List<SelectListItem>();
            DataSet dsUser = new AbstractLayer.AdminDB().GetAllAdminUser(0, "id like '%%'");
            foreach (System.Data.DataRow dr in dsUser.Tables[0].Rows)
            {
                adminUser.Add(new SelectListItem { Text = @dr["user"].ToString(), Value = @dr["id"].ToString() });
            }
            return adminUser;
        }


        public string GetAmountInWords(int amount)
        {
            string AmountInWords = "";
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAmountInWords", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@amount", amount);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    if (result.Tables.Count > 0)
                    {
                        if (result.Tables[0].Rows.Count > 0)
                        {
                            AmountInWords = result.Tables[0].Rows[0]["AmountInWords"].ToString();
                        }
                    }
                    return AmountInWords;
                }
            }
            catch (Exception ex)
            {
                return AmountInWords = "";
            }
        }

        public DataSet GetAdminDetailsById(int adminid, int year)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAdminDetailsByIdSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@year", year);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception)
            {
                return result = null;
            }
        }

        public DataSet GetActionOfSubMenu(int AdminId, string cont, string act)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetActionOfSubMenuSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AdminId", AdminId);
                    cmd.Parameters.AddWithValue("@Controller", cont);
                    cmd.Parameters.AddWithValue("@Action", act);
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception)
            {
                return result = null;
            }
        }



        #region Get All Session Year Admin
        public DataTable SessionMasterSPAdmin()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SessionMasterSPAdmin", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result.Tables[0];
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<SelectListItem> GetSessionYearSchoolAdmin()
        {
            DataTable dsSession = SessionMasterSPAdmin(); // SessionMasterSPAdmin
            List<SelectListItem> itemSession = new List<SelectListItem>();
            // itemSession.Add(new SelectListItem { Text = "2017", Value = "2017" });
            foreach (System.Data.DataRow dr in dsSession.Rows)
            {
                itemSession.Add(new SelectListItem { Text = @dr["yearto"].ToString(), Value = @dr["yearto"].ToString() });
            }
            itemSession.Add(new SelectListItem { Text = "1969", Value = "1969" });
            return itemSession;
        }

        public List<SelectListItem> GetSessionAdmin()
        {
            DataTable dsSession = SessionMasterSPAdmin(); // passing Value to SchoolDB from model
            List<SelectListItem> itemSession = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in dsSession.Rows)
            {
                itemSession.Add(new SelectListItem { Text = @dr["YearFull"].ToString(), Value = @dr["YearFull"].ToString() });
            }
            return itemSession;
        }

        #endregion Get All Session Year Admin




        public List<SelectListItem> GroupName1()
        {
            List<SelectListItem> GroupList = new List<SelectListItem>();
            GroupList.Add(new SelectListItem { Text = "--Select--", Value = "" });
            GroupList.Add(new SelectListItem { Text = "SCIENCE", Value = "S" });
            GroupList.Add(new SelectListItem { Text = "COMMERCE", Value = "C" });
            GroupList.Add(new SelectListItem { Text = "HUMANITIES", Value = "H" });
            GroupList.Add(new SelectListItem { Text = "VOCATIONAL", Value = "V" });
            //GroupList.Add(new SelectListItem { Text = "TECHNICAL", Value = "T" });
            GroupList.Add(new SelectListItem { Text = "AGRICULTURE", Value = "A" });

            return GroupList;
        }

        public DataSet schooltypes(string schid)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {

                    SqlCommand cmd = new SqlCommand("GetSchoolType", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", schid);

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


        //-----------------For SMS-----------------
        StringBuilder xml = new StringBuilder();

        public string gosmsPSEB(string mobileno, string message)
        {
            if (mobileno == "0000000000")
            {
                return "Not Valid";
            }
            else
            {
                string setmobileno = mobileno;

                string answer = "";
                string Apistatus = "";
                try
                {
                    int count = 0;
                    bool IsHindi;
                    string mobno = string.Empty;
                    string url = string.Empty;
                    String surl = string.Empty;
                    string sSendrnumber = string.Empty;
                    string Adddigit91 = "";
                    try
                    {
                        DataSet ds = new DataSet();
                        ds = getsmsSetup();
                        // RunProcedure("Udp_getActiveSmsSetUp", out ds);

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            surl = "https://3.111.144.132/api.php?username=PSEBCC&password=1125&sender=PSEBCC&sendto=@MN@&message=@MText@";
                            sSendrnumber = "PSEBCC";
                            Adddigit91 = "N";
                            //surl = ds.Tables[0].Rows[0]["URL"].ToString();
                            //sSendrnumber = ds.Tables[0].Rows[0]["Sender"].ToString();
                            //Adddigit91 = ds.Tables[0].Rows[0]["AddDigit91"].ToString();
                        }
                    }
                    catch (Exception ex) { }
                    int chkcount = 0;
                    int checksms = 0;
                    xml.Append("<root>");
                    string[] mob = mobileno.ToString().Trim().Split(',');

                    //********Start Changes of Hindi
                    IsHindi = IsUnicode(message);
                    if (IsHindi == true)
                    {
                        surl = surl.Insert(surl.Length, "&type=1");
                    }
                    //**********End

                    if (mob.Length > 0)
                    {
                        for (int ln = 0; ln < mob.Length; ln++)
                        {
                            checksms = 1;
                            chkcount = chkcount + 1;
                            mobileno = mob[ln].Trim();//dr["MobileNo"].ToString();
                            int j = mobileno.Length;
                            int counts = mobileno.Length;
                            int k = j - 10;
                            if (j < 10)
                            {
                                mobileno = "";
                            }
                            if (j == 10)
                            {
                                if (Adddigit91 == "Y")
                                    mobno = "91" + "" + mobileno;
                                else
                                    mobno = mobileno;
                            }
                            if (j > 10)
                            {
                                mobileno = mobileno.Remove(0, k);
                                if (Adddigit91 == "Y")
                                    mobno = "91" + "" + mobileno;
                                else
                                    mobno = mobileno;
                            }

                            //  message = message.Replace("&", "%26");
                            if (mobileno != "&nbsp;" && mobileno != "")
                            {
                                xml.Append("<SMS>");
                                xml.Append("<subject>" + message + "</subject>");
                                xml.Append("<Mobile_No>" + mobileno + "</Mobile_No>");
                                xml.Append("<Sender>" + sSendrnumber + "</Sender>");
                                xml.Append("<Update_Datetime>" + System.DateTime.Now + "</Update_Datetime>");


                                #region Genrate URL
                                url = surl;
                                url = url.Replace("@MN@", mobno);
                                url = url.Replace("@MText@", message);
                                string status = readHtmlPage(url);
                                Apistatus = status;
                                #endregion
                                count = count + 1;
                                int length = message.Length;
                                int divlength = length / 157;
                                decimal remilngth = length % 157;
                                if (divlength == 0)
                                {
                                    length = 1;
                                }
                                else
                                {
                                    length = divlength;
                                    if (remilngth != 0)
                                    {
                                        length = length + 1;
                                    }
                                }
                                xml.Append("</SMS>");
                                int legthxml = xml.Length;
                                if (legthxml > 7000 && legthxml < 7950)
                                {
                                    xml.Append("</root>");
                                    xml = new StringBuilder();
                                    xml.Append("<root>");
                                }
                            }

                        }
                    }

                    xml.Append("</root>");
                    try
                    {

                        if (count == 0 && chkcount != 0)
                        {
                            answer = "Applicant mobile no. not available.";
                        }
                        if (count == 0 && chkcount != 0 && mobileno == "")
                        {
                            answer = "Applicant mobile no. not valid.";
                        }
                        if (count > 0)
                        {
                            answer = count + " SMS send successfully to your Applicant.";
                        }
                    }
                    catch (Exception)
                    {
                        answer = "Sorry ! your information is not send, please contact administrator";
                    }
                }
                catch (Exception e)
                {
                    answer = "Sorry ! your information is not send, please contact administrator";
                }

                //try
                //{
                //    SqlParameter[] sqlparam1 = new SqlParameter[4];
                //    sqlparam1[0] = MakeInParameter("@mobile", SqlDbType.VarChar, 60, setmobileno);
                //    sqlparam1[1] = MakeInParameter("@sms", SqlDbType.NVarChar, 500, message);
                //    sqlparam1[2] = MakeInParameter("@status", SqlDbType.VarChar, 200, answer);
                //    sqlparam1[3] = MakeInParameter("@Apistatus", SqlDbType.VarChar, 4000, Apistatus);
                //    //RunProcedure("sp_sms_history", sqlparam1);
                //}
                //catch (Exception ex)
                //{

                //}

                try
                {
                    string hostName = Dns.GetHostName(); // Retrive the Name of HOST
                    string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                    int result;
                    SqlDataAdapter ad = new SqlDataAdapter();
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand("sp_sms_history", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@mobile", setmobileno);
                        cmd.Parameters.AddWithValue("@sms", message);
                        cmd.Parameters.AddWithValue("@status", answer);
                        cmd.Parameters.AddWithValue("@Apistatus", Apistatus);
                        cmd.Parameters.AddWithValue("@Ip", myIP);
                        con.Open();
                        result = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                }
                return answer;
            }
        }
        public string gosms(string mobileno, string message)
        {
            if (mobileno == "0000000000")
            {
                return "Not Valid";
            }
            else
            {
                string setmobileno = mobileno;

                string answer = "";
                string Apistatus = "";
                try
                {
                    int count = 0;
                    bool IsHindi;
                    string mobno = string.Empty;
                    string url = string.Empty;
                    String surl = string.Empty;
                    string sSendrnumber = string.Empty;
                    string Adddigit91 = "";
                    try
                    {
                        DataSet ds = new DataSet();
                        ds = getsmsSetup();
                        // RunProcedure("Udp_getActiveSmsSetUp", out ds);

                        if (ds.Tables[0].Rows.Count > 0)
                        {                            
                            surl = ds.Tables[0].Rows[0]["URL"].ToString();
                            sSendrnumber = ds.Tables[0].Rows[0]["Sender"].ToString();
                            Adddigit91 = ds.Tables[0].Rows[0]["AddDigit91"].ToString();
                        }
                    }
                    catch (Exception ex) { }
                    int chkcount = 0;
                    int checksms = 0;
                    xml.Append("<root>");
                    string[] mob = mobileno.ToString().Trim().Split(',');

                    //********Start Changes of Hindi
                    IsHindi = IsUnicode(message);
                    if (IsHindi == true)
                    {
                        surl = surl.Insert(surl.Length, "&type=1");
                    }
                    //**********End

                    if (mob.Length > 0)
                    {
                        for (int ln = 0; ln < mob.Length; ln++)
                        {
                            checksms = 1;
                            chkcount = chkcount + 1;
                            mobileno = mob[ln].Trim();//dr["MobileNo"].ToString();
                            int j = mobileno.Length;
                            int counts = mobileno.Length;
                            int k = j - 10;
                            if (j < 10)
                            {
                                mobileno = "";
                            }
                            if (j == 10)
                            {
                                if (Adddigit91 == "Y")
                                    mobno = "91" + "" + mobileno;
                                else
                                    mobno = mobileno;
                            }
                            if (j > 10)
                            {
                                mobileno = mobileno.Remove(0, k);
                                if (Adddigit91 == "Y")
                                    mobno = "91" + "" + mobileno;
                                else
                                    mobno = mobileno;
                            }

                            //  message = message.Replace("&", "%26");
                            if (mobileno != "&nbsp;" && mobileno != "")
                            {
                                xml.Append("<SMS>");
                                xml.Append("<subject>" + message + "</subject>");
                                xml.Append("<Mobile_No>" + mobileno + "</Mobile_No>");
                                xml.Append("<Sender>" + sSendrnumber + "</Sender>");
                                xml.Append("<Update_Datetime>" + System.DateTime.Now + "</Update_Datetime>");

                                
                                #region Genrate URL
                                url = surl;
                                url = url.Replace("@MN@", mobno);
                                url = url.Replace("@MText@", message);
                                string status = readHtmlPage(url);
                                Apistatus = status;
                                #endregion
                                count = count + 1;
                                int length = message.Length;
                                int divlength = length / 157;
                                decimal remilngth = length % 157;
                                if (divlength == 0)
                                {
                                    length = 1;
                                }
                                else
                                {
                                    length = divlength;
                                    if (remilngth != 0)
                                    {
                                        length = length + 1;
                                    }
                                }
                                xml.Append("</SMS>");
                                int legthxml = xml.Length;
                                if (legthxml > 7000 && legthxml < 7950)
                                {
                                    xml.Append("</root>");
                                    xml = new StringBuilder();
                                    xml.Append("<root>");
                                }
                            }

                        }
                    }

                    xml.Append("</root>");
                    try
                    {

                        if (count == 0 && chkcount != 0)
                        {
                            answer = "Applicant mobile no. not available.";
                        }
                        if (count == 0 && chkcount != 0 && mobileno == "")
                        {
                            answer = "Applicant mobile no. not valid.";
                        }
                        if (count > 0)
                        {
                            answer = count + " SMS send successfully to your Applicant.";
                        }
                    }
                    catch (Exception)
                    {
                        answer = "Sorry ! your information is not send, please contact administrator";
                    }
                }
                catch (Exception e)
                {
                    answer = "Sorry ! your information is not send, please contact administrator";
                }

                //try
                //{
                //    SqlParameter[] sqlparam1 = new SqlParameter[4];
                //    sqlparam1[0] = MakeInParameter("@mobile", SqlDbType.VarChar, 60, setmobileno);
                //    sqlparam1[1] = MakeInParameter("@sms", SqlDbType.NVarChar, 500, message);
                //    sqlparam1[2] = MakeInParameter("@status", SqlDbType.VarChar, 200, answer);
                //    sqlparam1[3] = MakeInParameter("@Apistatus", SqlDbType.VarChar, 4000, Apistatus);
                //    //RunProcedure("sp_sms_history", sqlparam1);
                //}
                //catch (Exception ex)
                //{

                //}

                try
                {
                    string hostName = Dns.GetHostName(); // Retrive the Name of HOST
                    string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                    int result;
                    SqlDataAdapter ad = new SqlDataAdapter();
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                    {
                        SqlCommand cmd = new SqlCommand("sp_sms_history", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@mobile", setmobileno);
                        cmd.Parameters.AddWithValue("@sms", message);
                        cmd.Parameters.AddWithValue("@status", answer);
                        cmd.Parameters.AddWithValue("@Apistatus", Apistatus);
                        cmd.Parameters.AddWithValue("@Ip", myIP);
                        con.Open();
                        result = cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                }
                return answer;
            }
        }
        public static String readHtmlPage(string url)
        {
            String result = "";
            String sResult = "";
            String strPost = "x=1&y=2&z=YouPostedOk";
            StreamWriter myWriter = null;
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            // HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create("");
            objRequest.Method = "POST";
            objRequest.ContentLength = strPost.Length;
            objRequest.ContentType = "application/x-www-form-urlencoded";
            try
            {
                myWriter = new StreamWriter(objRequest.GetRequestStream());
                myWriter.Write(strPost);
            }
            catch (Exception e)
            {
                sResult = "0";
                return sResult;
            }
            finally
            {
                myWriter.Close();
            }
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            using (StreamReader sr =
            new StreamReader(objResponse.GetResponseStream()))
            {

                result = sr.ReadToEnd();
                sr.Close();
            }

            return result;
        }
        public static bool IsUnicode(string input)
        {
            var asciiBytesCount = Encoding.ASCII.GetByteCount(input);
            var unicodBytesCount = Encoding.UTF8.GetByteCount(input);
            return asciiBytesCount != unicodBytesCount;
        }

        //----------------End SMS---------------
        public bool mail(string subject, string body, string to)
        {
            string Status = "";
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                string mail_from = ("noreply@psebonline.in");
                mail.From = new MailAddress(mail_from, "psebonline.in");
                if (to == "")
                {

                }
                else
                {
                    mail.To.Add(to);
                }

                string[] multi = to.Split(',');
                foreach (string MultiMail in multi)
                {
                    mail.To.Add(new MailAddress(MultiMail));
                }

                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.smtp2go.com";
                smtp.Port = 2525;
                //smtp.Credentials = new System.Net.NetworkCredential("noreply@psebonline.in", "YWZtam9qZWtrNHRr");

                try
                {
                    smtp.Send(mail);
                    mail.Dispose();
                    Status = "true";

                    try
                    {
                        string hostName = Dns.GetHostName(); // Retrive the Name of HOST
                        string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                        int result;
                        SqlDataAdapter ad = new SqlDataAdapter();
                        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                        {
                            SqlCommand cmd = new SqlCommand("sp_email_history", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@email", to);
                            cmd.Parameters.AddWithValue("@text", body);
                            cmd.Parameters.AddWithValue("@status", Status);
                            cmd.Parameters.AddWithValue("@subject", subject);
                            cmd.Parameters.AddWithValue("@IP", myIP);
                            con.Open();
                            result = cmd.ExecuteNonQuery();
                            return true;
                        }

                    }
                    catch (Exception ex)
                    {
                        return true;
                    }

                }
                catch (Exception)
                {
                    Status = "false";
                    return false;
                }

            }
            catch (Exception ex)
            {
                Status = "false";
                return false;
            }



        }


        public bool mailwithattachment(string subject, string body, string to, HttpPostedFileBase fileUploader)
        {
            string Status = "";
            try
            {
                System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage();
                string from_name = "helpdeskpsebonline@gmail.com";
                mail.From = new MailAddress(from_name, "PSEB");
                if (to == "")
                {

                }
                else
                {
                    mail.To.Add(to);
                }


                string[] multi = to.Split(',');
                foreach (string MultiMail in multi)
                {
                    mail.To.Add(new MailAddress(MultiMail));
                }

                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                //if (fileUploader != null)
                //{
                //    string fileName = Path.GetFileName(fileUploader.FileName);
                //    mail.Attachments.Add(new Attachment(fileUploader.InputStream, fileName));
                //}

                System.Net.Mail.Attachment attachment;
                attachment = new System.Net.Mail.Attachment("E:/Difference in Admission and Exam Fees of Open School.pdf");
                mail.Attachments.Add(attachment);

                SmtpClient smtp = new SmtpClient();
                //smtp.Host = "mail.smtp2go.com";
                // smtp.Port = 2525;
                // smtp.Credentials = new System.Net.NetworkCredential("noreply@psebonline.in", "YWZtam9qZWtrNHRr");
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 25;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;

                smtp.Credentials = new System.Net.NetworkCredential("helpdeskpsebonline@gmail.com", "helpdesk@26");
                try
                {
                    smtp.Send(mail);
                    mail.Dispose();
                    Status = "true";

                    try
                    {
                        string hostName = Dns.GetHostName(); // Retrive the Name of HOST
                        string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                        int result;
                        SqlDataAdapter ad = new SqlDataAdapter();
                        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                        {
                            SqlCommand cmd = new SqlCommand("sp_email_history", con);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@email", to);
                            cmd.Parameters.AddWithValue("@text", body);
                            cmd.Parameters.AddWithValue("@status", Status);
                            cmd.Parameters.AddWithValue("@subject", subject);
                            cmd.Parameters.AddWithValue("@IP", myIP);
                            con.Open();
                            result = cmd.ExecuteNonQuery();
                            return true;
                        }

                    }
                    catch (Exception ex)
                    {
                        return true;
                    }
                    //  return true;
                }
                catch (Exception ex)
                {
                    Status = "false";
                    return false;
                }

            }
            catch (Exception ex)
            {
                Status = "false";
                return false;
            }

        }




        public DataSet getsmsSetup()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Udp_getActiveSmsSetUp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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

        public DataSet SearchEmailID(string sid)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetEmailForgotpassword", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@sid", sid);
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


        public DataSet GetBankNameList(int type, string Bank, string IfscCode)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetBankNameListSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@Bank", Bank);
                    cmd.Parameters.AddWithValue("@IfscCode", IfscCode);
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    con.Open();
                    return ds;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<SelectListItem> GetNsqfPatternList()
        {
            List<SelectListItem> _List = new List<SelectListItem>();
            _List.Add(new SelectListItem { Text = "2 Years", Value = "2" });
           // _List.Add(new SelectListItem { Text = "4 Years", Value = "4" });      
            return _List;
        }

        public List<SelectListItem> GetBankList()
        {
            List<SelectListItem> BankList = new List<SelectListItem>();
            BankList.Add(new SelectListItem { Text = "State Bank of Patiala", Value = "101" });
            BankList.Add(new SelectListItem { Text = "Punjab National Bank", Value = "102" });
            BankList.Add(new SelectListItem { Text = "PSEB HOD", Value = "103" });
            return BankList;
        }
        public List<SelectListItem> GetN2Board()
        {
            List<SelectListItem> BoardN2List = new List<SelectListItem>();
            BoardN2List.Add(new SelectListItem { Text = "---Select Board--", Value = "0" });
            BoardN2List.Add(new SelectListItem { Text = "CBSE BOARD", Value = "CBSE BOARD" });
            //BoardN2List.Add(new SelectListItem { Text = "P.S.E.B BOARD", Value = "P.S.E.B BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "I.C.S.E BOARD", Value = "ICSE BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "HARYANA BOARD", Value = "HARYANA BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "HIMACHAL BOARD", Value = "HIMACHAL BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "J&K BOARD", Value = "J&K BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "RAJASTHAN BOARD", Value = "RAJASTHAN BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "OTHER BOARD", Value = "OTHER BOARD" });
            return BoardN2List;
        }
        public List<SelectListItem> GetMatricBoard()
        {
            List<SelectListItem> BoardN2List = new List<SelectListItem>();
            BoardN2List.Add(new SelectListItem { Text = "---Select Board--", Value = "0" });
            BoardN2List.Add(new SelectListItem { Text = "CBSE BOARD", Value = "CBSE BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "P.S.E.B BOARD", Value = "P.S.E.B BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "I.C.S.E BOARD", Value = "ICSE BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "HARYANA BOARD", Value = "HARYANA BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "HIMACHAL BOARD", Value = "HIMACHAL BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "J&K BOARD", Value = "J&K BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "RAJASTHAN BOARD", Value = "RAJASTHAN BOARD" });
            BoardN2List.Add(new SelectListItem { Text = "OTHER BOARD", Value = "OTHER BOARD" });
            return BoardN2List;
        }
        public List<SelectListItem> GetGroupMedium()
        {
            List<SelectListItem> TM = new List<SelectListItem>();
            TM.Add(new SelectListItem { Text = "Medium", Value = "Medium" });
            TM.Add(new SelectListItem { Text = "PUNJABI", Value = "PUNJABI" });
            TM.Add(new SelectListItem { Text = "HINDI", Value = "HINDI" });
            TM.Add(new SelectListItem { Text = "ENGLISH", Value = "ENGLISH" });

            return TM;
        }
        public List<SelectListItem> GetMediumAll()
        {
            List<SelectListItem> BM = new List<SelectListItem>();
            BM.Add(new SelectListItem { Text = "Medium", Value = "Medium" });
            BM.Add(new SelectListItem { Text = "PUNJABI", Value = "PUNJABI" });
            BM.Add(new SelectListItem { Text = "HINDI", Value = "HINDI" });
            BM.Add(new SelectListItem { Text = "ENGLISH", Value = "ENGLISH" });
            BM.Add(new SelectListItem { Text = "SANSKRIT", Value = "SANSKRIT" });
            BM.Add(new SelectListItem { Text = "URDU", Value = "URDU" });
            BM.Add(new SelectListItem { Text = "PERSIAN", Value = "PERSIAN" });
            BM.Add(new SelectListItem { Text = "ARABIC", Value = "ARABIC" });
            BM.Add(new SelectListItem { Text = "FRENCH", Value = "FRENCH" });
            BM.Add(new SelectListItem { Text = "GERMAN", Value = "GERMAN" });
            BM.Add(new SelectListItem { Text = "RUSSIAN", Value = "RUSSIAN" });

            return BM;
        }
        public List<SelectListItem> GroupName()
        {
            List<SelectListItem> GroupList = new List<SelectListItem>();
            GroupList.Add(new SelectListItem { Text = "--Select--", Value = "0" });
            GroupList.Add(new SelectListItem { Text = "SCIENCE", Value = "SCIENCE" });
            GroupList.Add(new SelectListItem { Text = "COMMERCE", Value = "COMMERCE" });
            GroupList.Add(new SelectListItem { Text = "HUMANITIES", Value = "HUMANITIES" });
            GroupList.Add(new SelectListItem { Text = "VOCATIONAL", Value = "VOCATIONAL" });
            //GroupList.Add(new SelectListItem { Text = "TECHNICAL", Value = "TECHNICAL" });
            GroupList.Add(new SelectListItem { Text = "AGRICULTURE", Value = "AGRICULTURE" });

            return GroupList;
        }
        public List<SelectListItem> getGroupT()
        {
            List<SelectListItem> GroupTList = new List<SelectListItem>();
            GroupTList.Add(new SelectListItem { Text = "--Select--", Value = "0" });
            GroupTList.Add(new SelectListItem { Text = "AGRICULTURE", Value = "A" });
            //GroupTList.Add(new SelectListItem { Text = "TECHNICAL", Value = "T" });
            GroupTList.Add(new SelectListItem { Text = "VOCATIONAL", Value = "V" });
            GroupTList.Add(new SelectListItem { Text = "COMMERCE", Value = "C" });
            GroupTList.Add(new SelectListItem { Text = "SCIENCE", Value = "S" });
            GroupTList.Add(new SelectListItem { Text = "HUMANITIES", Value = "H" });

            return GroupTList;
        }
        //---------------------------

        public DataTable GetAssignSubjectBySchool(string Schl)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAssignSubjectBySchool", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Schl", Schl);
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    con.Open();
                    return ds.Tables[0];
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        public int CountTableRowsMaster(int table)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CountTableRowsMasterSp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@table", table);
                    cmd.Parameters.Add("@totalcount", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    result = (int)cmd.Parameters["@totalcount"].Value;
                    return result;

                }
            }
            catch (Exception)
            {
                return result = -1;
            }
        }

        public string getPunjabiName(string text)
        {
            int i;
            //String en = Request.QueryString["id"].ToString();
            string en = text;
            char[] seps = { ' ' };
            string[] en1 = en.Split(seps);
            string pn = "";

            for (i = 0; i < en1.Length; i++)
            {
                // string mqry = "Select  * From dictonary where name='" + en1[i].ToString() + "'";  //t2
                //  DataSet ds = SqlHelper.ExecuteDataset(obj.getconnectionstring, CommandType.Text, mqry);
                DataTable ds = GetPunjabiNameSP(en1[i].ToString());
                if (ds.Rows.Count > 0)
                {
                    if (i == 0)
                    {
                        pn += ds.Rows[0]["pnbname"].ToString().Trim();
                    }
                    else
                    {
                        pn += " " + ds.Rows[0]["pnbname"].ToString().Trim();
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        pn += en1[i].ToString();
                    }
                    else
                    {
                        pn += " " + en1[i].ToString();
                    }
                }
            }
            //txt_candnm_pun.Text = pn;
            // Response.Write(pn);
            return pn;
        }


        public DataTable GetPunjabiNameSP(string name)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetPunjabiNameSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@name", name);
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    con.Open();
                    return ds.Tables[0];
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<SelectListItem> GetAllowedGroupListBySchool(string SCHL)
        {
            List<SelectListItem> MyGroupList = GroupName();
            DataTable dtAssignSubject = GetAssignSubjectBySchool(SCHL.ToString());
            if (dtAssignSubject != null && dtAssignSubject.Rows.Count > 0)
            {
                MyGroupList = GetSubjectsBySchool(dtAssignSubject, MyGroupList);
            }            
            return MyGroupList;
        }


        public List<SelectListItem> GetSubjectsBySchool(DataTable dtAssignSubject, List<SelectListItem> MyGroupList)
        {

            if (dtAssignSubject.Rows.Count > 0)
            {
                // SCI,COMM,HUM,VOC,TECH,AGRI
                if (dtAssignSubject.Rows[0]["SCI"].ToString() == "N")
                {
                    MyGroupList.Remove(MyGroupList.Single(x => x.Value == "SCIENCE"));
                }
                if (dtAssignSubject.Rows[0]["COMM"].ToString() == "N")
                {
                    MyGroupList.Remove(MyGroupList.Single(x => x.Value == "COMMERCE"));
                }
                if (dtAssignSubject.Rows[0]["HUM"].ToString() == "N")
                {
                    MyGroupList.Remove(MyGroupList.Single(x => x.Value == "HUMANITIES"));
                }
                if (dtAssignSubject.Rows[0]["VOC"].ToString() == "N")
                {
                    MyGroupList.Remove(MyGroupList.Single(x => x.Value == "VOCATIONAL"));
                }
                //if (dtAssignSubject.Rows[0]["TECH"].ToString() == "N")
                //{
                //    MyGroupList.Remove(MyGroupList.Single(x => x.Value == "TECHNICAL"));
                //}
                if (dtAssignSubject.Rows[0]["AGRI"].ToString() == "N")
                {
                    MyGroupList.Remove(MyGroupList.Single(x => x.Value == "AGRICULTURE"));
                }
            }

            return MyGroupList;
        }


        public List<SelectListItem> GetGroupListByGroup(List<SelectListItem> MyGroupList, string Group, string Form)
        {
            if (Group != "")
            {

                if (Form.Trim() == "T1")
                {
                    if (Group.ToUpper().Trim() == "HUMANITIES")
                    {
                        //MyGroupList = MyGroupList.Where(item => item.Text.ToUpper() == Group.ToUpper() || item.Value == "0").ToList();
                        //// MyGroupList.Remove(MyGroupList.Where(c => c.Value.ToUpper() != "HUMANITIES").SingleOrDefault());
                      MyGroupList = MyGroupList.Where(item => item.Text.ToUpper().Trim() == Group.ToUpper().Trim() || item.Text.ToUpper().Trim() == "HUMANITIES" || item.Text.ToUpper().Trim() == "VOCATIONAL" || item.Value == "0").ToList();
                    }
                    else if (Group.ToUpper().Trim() == "SCIENCE" || Group.ToUpper().Trim() == "COMMERCE")
                    {
                        MyGroupList = MyGroupList.Where(item => item.Text.ToUpper().Trim() == Group.ToUpper().Trim() || item.Text.ToUpper().Trim() == "HUMANITIES" || item.Text.ToUpper().Trim() == "VOCATIONAL" || item.Value == "0").ToList();
                    }
                    else
                    {
                        MyGroupList = MyGroupList.Where(item => item.Text.ToUpper().Trim() == Group.ToUpper().Trim() || item.Text.ToUpper().Trim() == "HUMANITIES" || item.Value == "0").ToList();
                    }
                }
            }
            return MyGroupList;
        }
        //---------------Get All School Types------------

        public DataTable TehsilMaster()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("TehsilMasterSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result.Tables[0];
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        //---------------Get All Session------------
        public DataTable SessionMaster()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SessionMasterSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result.Tables[0];
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //---------------Get All School Types------------
        public DataTable GetAllClassType()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAllClassType", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result.Tables[0];
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        //---------------Get All School Types------------
        public DataTable GetAllSchoolType()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAllSchoolType", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result.Tables[0];
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        public List<SelectListItem> GetNonGovtSchoolTypeList()
        {
            DataSet dsSchool = GetAllSchoolTypeNew(); // passing Value to SchoolDB from model

            var itemSubUType = dsSchool.Tables[1].AsEnumerable()
            .Select(dataRow => new SelectListItem
            {
                Text = dataRow.Field<string>("schooltype").ToString(),
                Value = dataRow.Field<string>("abbr").ToString(),
            }).ToList();
            return itemSubUType.ToList();
        }


        public DataSet GetAllSchoolTypeNew()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAllSchoolType", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        //-------------Drp Fill District Details-----------//
        public DataSet Fll_Dist_Details()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAllDistrict", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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

        //Commmo Region
        public List<SelectListItem> GetAllTehsil()
        {
            DataTable dsTehsilSession = TehsilMaster(); // passing Value to SchoolDB from model
            List<SelectListItem> itemTeh = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in dsTehsilSession.Rows)
            {
                itemTeh.Add(new SelectListItem { Text = @dr["TEHSIL"].ToString(), Value = @dr["TCODE"].ToString() });


            }
            return itemTeh;
        }

        public List<SelectListItem> GetAllTehsilP()
        {
            DataTable dsTehsilSession = TehsilMaster(); // passing Value to SchoolDB from model
            List<SelectListItem> itemTeh = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in dsTehsilSession.Rows)
            {
                itemTeh.Add(new SelectListItem { Text = @dr["TEHSILP"].ToString(), Value = @dr["TCODE"].ToString() });


            }
            return itemTeh;
        }
        public static List<SelectListItem> GetSessionSingle()
        {
            List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession.Add(new SelectListItem { Text = "2021", Value = "2021" });
            itemSession.Add(new SelectListItem { Text = "2022", Value = "2022" });
            itemSession.Add(new SelectListItem { Text = "2023", Value = "2023" });
            return itemSession;
        }

        public static List<SelectListItem> GetMonthFullNameBYNumber()
        {
            List<SelectListItem> MonthList = new List<SelectListItem>();
            MonthList.Add(new SelectListItem { Text = "January", Value = "01" });
            MonthList.Add(new SelectListItem { Text = "February", Value = "02" });
            MonthList.Add(new SelectListItem { Text = "March", Value = "03" });
            MonthList.Add(new SelectListItem { Text = "April", Value = "04" });
            MonthList.Add(new SelectListItem { Text = "May", Value = "05" });
            MonthList.Add(new SelectListItem { Text = "June", Value = "06" });
            MonthList.Add(new SelectListItem { Text = "July", Value = "07" });
            MonthList.Add(new SelectListItem { Text = "August", Value = "08" });
            MonthList.Add(new SelectListItem { Text = "September", Value = "09" });
            MonthList.Add(new SelectListItem { Text = "October", Value = "10" });
            MonthList.Add(new SelectListItem { Text = "November", Value = "11" });
            MonthList.Add(new SelectListItem { Text = "December", Value = "12" });
            return MonthList;
        }

        public static List<SelectListItem> GetMonthNameNumber()
        {
            List<SelectListItem> MonthList = new List<SelectListItem>();
            MonthList.Add(new SelectListItem { Text = "Jan", Value = "01" });
            MonthList.Add(new SelectListItem { Text = "Feb", Value = "02" });
            MonthList.Add(new SelectListItem { Text = "Mar", Value = "03" });
            MonthList.Add(new SelectListItem { Text = "April", Value = "04" });
            MonthList.Add(new SelectListItem { Text = "May", Value = "05" });
            MonthList.Add(new SelectListItem { Text = "Jun", Value = "06" });
            MonthList.Add(new SelectListItem { Text = "July", Value = "07" });
            MonthList.Add(new SelectListItem { Text = "Aug", Value = "08" });
            MonthList.Add(new SelectListItem { Text = "Sept", Value = "09" });
            MonthList.Add(new SelectListItem { Text = "Oct", Value = "10" });
            MonthList.Add(new SelectListItem { Text = "Nov", Value = "11" });
            MonthList.Add(new SelectListItem { Text = "Dec", Value = "12" });
            return MonthList;
        }

        public List<SelectListItem> GetMonth()
        {
            List<SelectListItem> MonthList = new List<SelectListItem>();
            //MonthList.Add(new SelectListItem { Text = "--Select Month--", Value = "0" });
            MonthList.Add(new SelectListItem { Text = "Jan", Value = "Jan" });
            MonthList.Add(new SelectListItem { Text = "Feb", Value = "Feb" });
            MonthList.Add(new SelectListItem { Text = "Mar", Value = "Mar" });
            MonthList.Add(new SelectListItem { Text = "April", Value = "April" });
            MonthList.Add(new SelectListItem { Text = "May", Value = "May" });
            MonthList.Add(new SelectListItem { Text = "Jun", Value = "Jun" });
            MonthList.Add(new SelectListItem { Text = "July", Value = "July" });
            MonthList.Add(new SelectListItem { Text = "Aug", Value = "Aug" });
            MonthList.Add(new SelectListItem { Text = "Sept", Value = "Sept" });
            MonthList.Add(new SelectListItem { Text = "Oct", Value = "Oct" });
            MonthList.Add(new SelectListItem { Text = "Nov", Value = "Nov" });
            MonthList.Add(new SelectListItem { Text = "Dec", Value = "Dec" });
            return MonthList;
        }
        public List<SelectListItem> GetCaste()
        {
            List<SelectListItem> CastList = new List<SelectListItem>();
            if (HttpContext.Current.Session["Session"] == null)
            {
                CastList.Add(new SelectListItem { Text = "General", Value = "General" });
                CastList.Add(new SelectListItem { Text = "BC", Value = "BC" });
                CastList.Add(new SelectListItem { Text = "OBC", Value = "OBC" });
                CastList.Add(new SelectListItem { Text = "SC", Value = "SC" });
                CastList.Add(new SelectListItem { Text = "ST", Value = "ST" });
                //CastList.Add(new SelectListItem { Text = "Others", Value = "Others" });
            }
            else if (HttpContext.Current.Session["Session"].ToString() == "2016-2017")
            {
                CastList.Add(new SelectListItem { Text = "General", Value = "General" });
                CastList.Add(new SelectListItem { Text = "BC", Value = "BC" });
                CastList.Add(new SelectListItem { Text = "OBC", Value = "OBC" });
                CastList.Add(new SelectListItem { Text = "SC(R&O)", Value = "SC(R&O)" });
                CastList.Add(new SelectListItem { Text = "SC(M&B)", Value = "SC(M&B)" });
                CastList.Add(new SelectListItem { Text = "SC", Value = "SC" });
                CastList.Add(new SelectListItem { Text = "ST", Value = "ST" });
                CastList.Add(new SelectListItem { Text = "Others", Value = "Others" });
            }
            else
            {
                CastList.Add(new SelectListItem { Text = "General", Value = "General" });
                CastList.Add(new SelectListItem { Text = "BC", Value = "BC" });
                CastList.Add(new SelectListItem { Text = "OBC", Value = "OBC" });
                CastList.Add(new SelectListItem { Text = "SC", Value = "SC" });
                CastList.Add(new SelectListItem { Text = "ST", Value = "ST" });
                // CastList.Add(new SelectListItem { Text = "Others", Value = "Others" });
            }

            return CastList;
        }
        public DataTable GetDA_DB()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetDA_DB_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result.Tables[0];
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<SelectListItem> GetDA()
        {
            DataTable dsGetDA_DB = GetDA_DB();
            List<SelectListItem> DList = new List<SelectListItem>();
            foreach (DataRow dr in dsGetDA_DB.Rows)
            {
                DList.Add(new SelectListItem { Text = @dr["dacat"].ToString(), Value = @dr["dacat"].ToString() });
            }
            return DList;
        }
        public List<SelectListItem> GetCategory()
        {
            List<SelectListItem> catgilist = new List<SelectListItem>();
            catgilist.Add(new SelectListItem { Text = "8th PASSED", Value = "8th PASSED" });
            //catgilist.Add(new SelectListItem { Text = "8TH REAPPEAR", Value = "8TH REAPPEAR" });
            catgilist.Add(new SelectListItem { Text = "9th FAILED", Value = "9th FAILED" });
            return catgilist;
        }


        public List<SelectListItem> GetE2Category()
        {
            List<SelectListItem> catgilist = new List<SelectListItem>();
            catgilist.Add(new SelectListItem { Text = "10TH PASSED", Value = "10TH PASSED" });
            catgilist.Add(new SelectListItem { Text = "10TH REAPPEAR", Value = "10TH REAPPEAR" });
            catgilist.Add(new SelectListItem { Text = "11TH FAILED", Value = "11TH FAILED" });
            catgilist.Add(new SelectListItem { Text = "11TH ABSENT", Value = "11TH ABSENT" });
            catgilist.Add(new SelectListItem { Text = "11th COMPARTMENT", Value = "11th COMPARTMENT" });
            catgilist.Add(new SelectListItem { Text = "12th REAPPEAR", Value = "12th REAPPEAR" });
            catgilist.Add(new SelectListItem { Text = "12TH FAILED", Value = "12TH FAILED" });
            catgilist.Add(new SelectListItem { Text = "12TH ABSENT", Value = "12TH ABSENT" });
            catgilist.Add(new SelectListItem { Text = "11TH PASSED", Value = "11TH PASSED" });
            catgilist.Add(new SelectListItem { Text = "12th COMPARTMENT", Value = "12th COMPARTMENT" });
            catgilist.Add(new SelectListItem { Text = "12th CANCELLED", Value = "12th CANCELLED" });
            return catgilist;
        }
        public List<SelectListItem> GetMCategories()
        {
            List<SelectListItem> catgilist = new List<SelectListItem>();
            catgilist.Add(new SelectListItem { Text = "9th COMPARTMENT", Value = "9th COMPARTMENT" });
            catgilist.Add(new SelectListItem { Text = "9TH PASSED", Value = "9TH PASSED" });
            catgilist.Add(new SelectListItem { Text = "10TH FAILED", Value = "10TH FAILED" });
            catgilist.Add(new SelectListItem { Text = "10TH REAPPEAR", Value = "10TH REAPPEAR" });
            catgilist.Add(new SelectListItem { Text = "10TH ABSENT", Value = "10TH ABSENT" });
            catgilist.Add(new SelectListItem { Text = "10TH CANCELLED", Value = "10TH CANCELLED" });
            return catgilist;
        }
        public List<SelectListItem> GetECategories()
        {
            List<SelectListItem> catgilist = new List<SelectListItem>();
            catgilist.Add(new SelectListItem { Text = "10th FAIL", Value = "10th FAIL" });
            catgilist.Add(new SelectListItem { Text = "10th COMPARTMENT", Value = "10th COMPARTMENT" });
            catgilist.Add(new SelectListItem { Text = "10th REAPPEAR", Value = "10th REAPPEAR" });
            catgilist.Add(new SelectListItem { Text = "10TH PASSED", Value = "10TH PASSED" });
            catgilist.Add(new SelectListItem { Text = "11TH FAILED", Value = "11TH FAILED" });
            catgilist.Add(new SelectListItem { Text = "11TH ABSENT", Value = "11TH ABSENT" });

            return catgilist;
        }
        public List<SelectListItem> GetBoard()
        {
            List<SelectListItem> BoardList = new List<SelectListItem>();
            BoardList.Add(new SelectListItem { Text = "Board", Value = "Board" });
            BoardList.Add(new SelectListItem { Text = "School", Value = "School" });
            return BoardList;
        }
        public List<SelectListItem> GetReligion()
        {
            List<SelectListItem> Relist = new List<SelectListItem>();
            Relist.Add(new SelectListItem { Text = "Hindu", Value = "Hindu" });
            Relist.Add(new SelectListItem { Text = "Muslim", Value = "Muslim" });
            Relist.Add(new SelectListItem { Text = "Sikh", Value = "Sikh" });
            Relist.Add(new SelectListItem { Text = "Christian", Value = "Christian" });
            Relist.Add(new SelectListItem { Text = "Others", Value = "Others" });
            return Relist;
        }
        public List<SelectListItem> GetSection()
        {
            List<SelectListItem> itemSection = new List<SelectListItem>();
            itemSection.Add(new SelectListItem { Text = "Select", Value = "0" });
            itemSection.Add(new SelectListItem { Text = "A", Value = "A" });
            itemSection.Add(new SelectListItem { Text = "B", Value = "B" });
            itemSection.Add(new SelectListItem { Text = "C", Value = "C" });
            itemSection.Add(new SelectListItem { Text = "D", Value = "D" });
            itemSection.Add(new SelectListItem { Text = "E", Value = "E" });
            itemSection.Add(new SelectListItem { Text = "F", Value = "F" });
            itemSection.Add(new SelectListItem { Text = "G", Value = "G" });
            itemSection.Add(new SelectListItem { Text = "H", Value = "H" });
            itemSection.Add(new SelectListItem { Text = "I", Value = "I" });
            itemSection.Add(new SelectListItem { Text = "J", Value = "J" });
            itemSection.Add(new SelectListItem { Text = "K", Value = "K" });
            itemSection.Add(new SelectListItem { Text = "L", Value = "L" });
            itemSection.Add(new SelectListItem { Text = "M", Value = "M" });
            itemSection.Add(new SelectListItem { Text = "N", Value = "N" });
            itemSection.Add(new SelectListItem { Text = "O", Value = "O" });
            itemSection.Add(new SelectListItem { Text = "P", Value = "P" });
            itemSection.Add(new SelectListItem { Text = "Q", Value = "Q" });
            itemSection.Add(new SelectListItem { Text = "R", Value = "R" });
            itemSection.Add(new SelectListItem { Text = "S", Value = "S" });
            itemSection.Add(new SelectListItem { Text = "T", Value = "T" });
            itemSection.Add(new SelectListItem { Text = "U", Value = "U" });
            itemSection.Add(new SelectListItem { Text = "V", Value = "V" });
            itemSection.Add(new SelectListItem { Text = "W", Value = "W" });
            itemSection.Add(new SelectListItem { Text = "X", Value = "X" });
            itemSection.Add(new SelectListItem { Text = "Y", Value = "Y" });
            itemSection.Add(new SelectListItem { Text = "Z", Value = "Z" });
            return itemSection;
        }
        public List<SelectListItem> GetArea()
        {
            List<SelectListItem> itemArea = new List<SelectListItem>();
            itemArea.Add(new SelectListItem { Text = "URBAN", Value = "U" });
            itemArea.Add(new SelectListItem { Text = "RURAL", Value = "R" });
            return itemArea;
        }

        public List<SelectListItem> GetTerm()
        {
            List<SelectListItem> itemArea = new List<SelectListItem>();
            itemArea.Add(new SelectListItem { Text = "Term1", Value = "Term1" });
            itemArea.Add(new SelectListItem { Text = "Term2", Value = "Term2" });
            return itemArea;
        }

        public List<SelectListItem> GetYesNo()
        {
            List<SelectListItem> itemYesNo = new List<SelectListItem>();
            itemYesNo.Add(new SelectListItem { Text = "NO", Value = "N" });
            itemYesNo.Add(new SelectListItem { Text = "YES", Value = "Y" });
            return itemYesNo;
        }

        public List<SelectListItem> GetYesNoText()
        {
            List<SelectListItem> itemYesNo = new List<SelectListItem>();
            itemYesNo.Add(new SelectListItem { Text = "NO", Value = "NO" });
            itemYesNo.Add(new SelectListItem { Text = "YES", Value = "YES" });
            return itemYesNo;
        }
        public List<SelectListItem> GetStatus()
        {
            List<SelectListItem> itemStatus = new List<SelectListItem>();
            itemStatus.Add(new SelectListItem { Text = "DONE", Value = "DONE" , Selected = true});
            itemStatus.Add(new SelectListItem { Text = "CANCEL", Value = "CANCEL" });
            return itemStatus;
        }

        public List<SelectListItem> GetSession()
        {
           List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession.Add(new SelectListItem { Text = "2023-2024", Value = "2023-2024" });
            return itemSession;
        }


        public List<SelectListItem> GetSessionAll()
        {
            DataTable dsSession = SessionMaster(); // passing Value to SchoolDB from model
            List<SelectListItem> itemSession = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in dsSession.Rows)
            {
                itemSession.Add(new SelectListItem { Text = @dr["YearFull"].ToString(), Value = @dr["YearFull"].ToString() });
            }
            return itemSession;
        }

        public List<SelectListItem> GetSessionYear()
        {
            DataTable dsSession = SessionMaster(); // passing Value to SchoolDB from model
            List<SelectListItem> itemSession = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in dsSession.Rows)
            {
                itemSession.Add(new SelectListItem { Text = @dr["yearfrom"].ToString(), Value = @dr["yearfrom"].ToString() });
            }
            return itemSession;
        }

        public List<SelectListItem> GetSessionYearSchool()
        {
            DataTable dsSession = SessionMaster(); // passing Value to SchoolDB from model
            List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession.Add(new SelectListItem { Text = "2018", Value = "2018" });
            foreach (System.Data.DataRow dr in dsSession.Rows)
            {
                itemSession.Add(new SelectListItem { Text = @dr["yearfrom"].ToString(), Value = @dr["yearfrom"].ToString() });
            }
            return itemSession;
        }
        public List<SelectListItem> GetClass()
        {
            DataTable dsClass = GetAllClassType(); // passing Value to SchoolDB from model
            List<SelectListItem> itemClass = new List<SelectListItem>();
            // itemClass.Add(new SelectListItem { Text = "--Select Class--", Value = "0" });
            foreach (System.Data.DataRow dr in dsClass.Rows)
            {
                itemClass.Add(new SelectListItem { Text = @dr["class"].ToString(), Value = @dr["Id"].ToString() });
            }
            return itemClass;
        }

        public List<SelectListItem> GetNovGovtSchoolType()
        {
            DataTable dsSchool = GetAllSchoolType(); // passing Value to SchoolDB from model
            List<SelectListItem> itemSchool = new List<SelectListItem>();
            //itemSchool.Add(new SelectListItem { Text = "--Select School Type--", Value = "0" });
            foreach (System.Data.DataRow dr in dsSchool.Rows)
            {
                itemSchool.Add(new SelectListItem { Text = @dr["schooltype"].ToString(), Value = @dr["schooltype"].ToString() });
            }
            // itemSchool.Add(new SelectListItem { Text = "Select", Value = "0" });
            return itemSchool;
        }


        public List<SelectListItem> GetSchool()
        {
            DataTable dsSchool = GetAllSchoolType(); // passing Value to SchoolDB from model
            List<SelectListItem> itemSchool = new List<SelectListItem>();
            //itemSchool.Add(new SelectListItem { Text = "--Select School Type--", Value = "0" });
            foreach (System.Data.DataRow dr in dsSchool.Rows)
            {
                itemSchool.Add(new SelectListItem { Text = @dr["schooltype"].ToString(), Value = @dr["schooltype"].ToString() });
            }
            // itemSchool.Add(new SelectListItem { Text = "Select", Value = "0" });
            return itemSchool;
        }

        public List<SelectListItem> GetSchoolAbbr()
        {
            DataTable dsSchool = GetAllSchoolType(); // passing Value to SchoolDB from model
            List<SelectListItem> itemSchool = new List<SelectListItem>();
            // itemSchool.Add(new SelectListItem { Text = "--Select School Type--", Value = "0" });
            foreach (System.Data.DataRow dr in dsSchool.Rows)
            {
                itemSchool.Add(new SelectListItem { Text = @dr["schooltype"].ToString(), Value = @dr["abbr"].ToString() });
            }
            //itemSchool.Add(new SelectListItem { Text = "Select", Value = "0" });
            //itemSchool.Insert(0, new SelectListItem { Text = "Select", Value = "0" });
            return itemSchool;
        }

        public List<SelectListItem> GetDistE()
        {
            DataSet dsDist = Fll_Dist_Details();
            // English
            List<SelectListItem> itemDist = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in dsDist.Tables[0].Rows)
            {
                itemDist.Add(new SelectListItem { Text = @dr["DISTNM"].ToString(), Value = @dr["DIST"].ToString() });
            }
            return itemDist;
        }
        public List<SelectListItem> GetDistP()
        {
            DataSet dsDist = Fll_Dist_Details();
            // Punjabi
            List<SelectListItem> itemDistP = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in dsDist.Tables[0].Rows)
            {
                itemDistP.Add(new SelectListItem { Text = @dr["DISTNMP"].ToString(), Value = @dr["DIST"].ToString() });
            }
            return itemDistP;
        }

        public List<SelectListItem> SearchSchoolItems()
        {
            var itemsch = new SelectList(new[]{new {ID="1",Name="School Code"},new{ID="2",Name="School Name"},
                     new{ID="3",Name="School IDNO"},new{ID="4",Name="School Station"},new{ID="5",Name="School Center Code"},new{ID="6",Name="UDISE Code"},}, "ID", "Name", 1);
            return itemsch.ToList();
        }


        //---------------Select AllTehsil
        public DataSet SelectAllTehsil(int DISTID)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("GetAllTehsil", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@DISTID", DISTID);

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



        public string GetSubjectName(int id)
        {
            SqlConnection con = null;
            string result = "";
            try
            {


                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("GetSubjectName", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.Add("@subjectname", SqlDbType.NVarChar, 900).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                result = Convert.ToString(cmd.Parameters["@subjectname"].Value);
                return result;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                return "";
            }
            finally
            {
                con.Close();
            }

        }

        public DataSet Fll_Subject_Details(int id)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("pro_GetAllSubjectById", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id);
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

        public List<SelectListItem> GetSubject(int id)
        {
            DataSet dsDist = Fll_Subject_Details(id);
            // English
            List<SelectListItem> itemDist = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in dsDist.Tables[0].Rows)
            {
                itemDist.Add(new SelectListItem { Text = @dr["subjectname"].ToString(), Value = @dr["id"].ToString() });
            }
            return itemDist;
        }

        public List<SelectListItem> GetCadre()
        {
            DataSet dsDist = Fll_Cadre_Details();
            // English
            List<SelectListItem> itemDist = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in dsDist.Tables[0].Rows)
            {
                itemDist.Add(new SelectListItem { Text = @dr["cadrename"].ToString(), Value = @dr["id"].ToString() });
            }
            return itemDist;
        }

        public DataSet Fll_Cadre_Details()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("pro_GetAllCadre", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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
        public List<SelectListItem> GetFeeCat()
        {
            DataSet dsDist = Fll_FeeCat_Details();
            // English
            List<SelectListItem> itemDist = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in dsDist.Tables[0].Rows)
            {
                itemDist.Add(new SelectListItem { Text = @dr["FEECAT"].ToString().Trim(), Value = @dr["FEECAT"].ToString().Trim() });
            }
            return itemDist;
        }
        public DataSet Fll_FeeCat_Details()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("pro_GetAllFeeCat", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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
        public DataTable GetAssignSubjectBySchoolandStudent(string Schl, string stdsub)
        {
            DataSet ds = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAssignSubjectBySchoolandStudent_Sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Schl", Schl);
                    cmd.Parameters.AddWithValue("@stdsub", stdsub);
                    ad.SelectCommand = cmd;
                    ad.Fill(ds);
                    con.Open();
                    return ds.Tables[0];
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public List<SelectListItem> GetSubjectsBySchoolByCandiDate(DataTable dtAssignSubject, List<SelectListItem> mg)
        {

            if (dtAssignSubject.Rows.Count > 0)
            {
                // SCI,COMM,HUM,VOC,TECH,AGRI

                //mg.Remove(new SelectListItem { Text = "SCIENCE", Value = "SCIENCE" });
                //mg.Remove(new SelectListItem { Text = "COMMERCE", Value = "COMMERCE" });
                //mg.Remove(new SelectListItem { Text = "HUMANITIES", Value = "HUMANITIES" });
                //mg.Remove(new SelectListItem { Text = "VOCATIONAL", Value = "VOCATIONAL" });
                //mg.Remove(new SelectListItem { Text = "TECHNICAL", Value = "TECHNICAL" });
                //mg.Remove(new SelectListItem { Text = "AGRICULTURE", Value = "AGRICULTURE" });

                if (Convert.ToString(dtAssignSubject.Rows[0]["COMM"]) == "Y")
                {
                    mg.Remove(mg.Single(x => x.Value == "SCIENCE"));
                    mg.Remove(mg.Single(x => x.Value == "VOCATIONAL"));
                    //mg.Remove(mg.Single(x => x.Value == "TECHNICAL"));
                    mg.Remove(mg.Single(x => x.Value == "AGRICULTURE"));

                }
                else if (Convert.ToString(dtAssignSubject.Rows[0]["SCI"]) == "Y")
                {
                    mg.Remove(mg.Single(x => x.Value == "COMMERCE"));
                    mg.Remove(mg.Single(x => x.Value == "VOCATIONAL"));
                    //mg.Remove(mg.Single(x => x.Value == "TECHNICAL"));
                    mg.Remove(mg.Single(x => x.Value == "AGRICULTURE"));

                }
                else if (dtAssignSubject.Rows[0]["HUM"].ToString() == "Y")
                {
                    mg.Remove(mg.Single(x => x.Value == "SCIENCE"));
                    mg.Remove(mg.Single(x => x.Value == "COMMERCE"));
                    mg.Remove(mg.Single(x => x.Value == "VOCATIONAL"));
                    //mg.Remove(mg.Single(x => x.Value == "TECHNICAL"));
                    mg.Remove(mg.Single(x => x.Value == "AGRICULTURE"));

                }
                else if (dtAssignSubject.Rows[0]["VOC"].ToString() == "Y")
                {
                    mg.Remove(mg.Single(x => x.Value == "COMMERCE"));
                    mg.Remove(mg.Single(x => x.Value == "SCIENCE"));
                    //mg.Remove(mg.Single(x => x.Value == "TECHNICAL"));
                    mg.Remove(mg.Single(x => x.Value == "AGRICULTURE"));

                }
                else if (dtAssignSubject.Rows[0]["TECH"].ToString() == "Y")
                {
                    mg.Remove(mg.Single(x => x.Value == "COMMERCE"));
                    mg.Remove(mg.Single(x => x.Value == "VOCATIONAL"));
                    mg.Remove(mg.Single(x => x.Value == "SCIENCE"));
                    mg.Remove(mg.Single(x => x.Value == "AGRICULTURE"));

                }
                else if (dtAssignSubject.Rows[0]["AGRI"].ToString() == "Y")
                {
                    mg.Remove(mg.Single(x => x.Value == "COMMERCE"));
                    mg.Remove(mg.Single(x => x.Value == "VOCATIONAL"));
                    //mg.Remove(mg.Single(x => x.Value == "TECHNICAL"));
                    mg.Remove(mg.Single(x => x.Value == "SCIENCE"));

                }
            }

            return mg;
        }


        public void ReGenerateChallaanByIdSPAdmin(float lumsumfine, string lumsumremarks, string challanid, float fee, out int outstatus)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ReGenerateChallaanByIdSPAdmin", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CHALLANID", challanid);
                    cmd.Parameters.AddWithValue("@type", "Admin");
                    cmd.Parameters.AddWithValue("@lumsumfine", lumsumfine);
                    cmd.Parameters.AddWithValue("@FEE", fee);
                    cmd.Parameters.AddWithValue("@lumsumremarks", lumsumremarks);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    outstatus = (int)cmd.Parameters["@OutStatus"].Value;

                }
            }
            catch (Exception)
            {
                outstatus = -1;
            }
        }

        public int ReGenerateChallaanByIdSPAdminNew(string EmpUserId,float lumsumfine, string lumsumremarks, string challanid, out string OutError, DateTime? date = null, float fee = 0, DateTime? dateV = null)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    // updated for regerated any challan with lumsumfee of any category
                    //SqlCommand cmd = new SqlCommand("ReGenerateChallaanByIdPSEBNEw1", con);//ReGenerateChallaanByIdSPAdminNewD// ReGenerateChallaanByIdSPAdminNew
                    SqlCommand cmd = new SqlCommand("ReGenerateChallaanByIdPSEB", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                    cmd.Parameters.AddWithValue("@CHALLANID", challanid);
                    cmd.Parameters.AddWithValue("@type", "Admin");
                    cmd.Parameters.AddWithValue("@lumsumfine", lumsumfine);
                    cmd.Parameters.AddWithValue("@FEE", fee);
                    cmd.Parameters.AddWithValue("@lumsumremarks", lumsumremarks);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@dateV", dateV);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    result = Convert.ToInt32(cmd.Parameters["@OutStatus"].Value);
                    OutError = (string)cmd.Parameters["@OutError"].Value;
                }
            }
            catch (Exception ex)
            {
                OutError = "";
                result = -2;
            }
            return result;
        }

        public void CancelOfflineChallanBySchl(string cancelremarks, string challanid, out string outstatus, string Schl, string Type)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CancelOfflineChallanBySchlSP", con);//ChallanDetailsCancelSP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CHALLANID", challanid);
                    cmd.Parameters.AddWithValue("@Schl", Schl);
                    cmd.Parameters.AddWithValue("@cancelremarks", cancelremarks);
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    outstatus = Convert.ToString(cmd.Parameters["@OutStatus"].Value);

                }
            }
            catch (Exception ex)
            {
                outstatus = "-1";
            }
        }

        public void ChallanDetailsRefund(string RefundRefno, string RefundDate, string RefundRemarks, string challanid, out string outstatus, int AdminId, string Type,string EmpUserId)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ChallanDetailsRefundSP", con);//ChallanDetailsCancelSP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                    cmd.Parameters.AddWithValue("@CHALLANID", challanid);
                    cmd.Parameters.AddWithValue("@AdminId", AdminId);
                    cmd.Parameters.AddWithValue("@RefundRefno", RefundRefno);
                    cmd.Parameters.AddWithValue("@RefundDate", RefundDate);
                    cmd.Parameters.AddWithValue("@RefundRemarks", RefundRemarks);
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    outstatus = Convert.ToString(cmd.Parameters["@OutStatus"].Value);

                }
            }
            catch (Exception ex)
            {
                outstatus = "-1";
            }
        }


        public void ChallanDetailsCancel(string cancelremarks, string challanid, out string outstatus, int AdminId, string Type,string EmpUserId)
        {
            int result;
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ChallanDetailsCancelSP", con);//ChallanDetailsCancelSP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
                    cmd.Parameters.AddWithValue("@CHALLANID", challanid);
                    cmd.Parameters.AddWithValue("@AdminId", AdminId);
                    cmd.Parameters.AddWithValue("@cancelremarks", cancelremarks);
                    cmd.Parameters.AddWithValue("@Type", Type);
                    cmd.Parameters.Add("@OutStatus", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
                    con.Open();
                    result = cmd.ExecuteNonQuery();
                    outstatus = Convert.ToString(cmd.Parameters["@OutStatus"].Value);

                }
            }
            catch (Exception ex)
            {
                outstatus = "-1";
            }
        }
        public List<SelectListItem> GetAllPSEBCLASS()
        {
            List<SelectListItem> iList = new List<SelectListItem>();
            iList.Add(new SelectListItem { Text = "None", Value = "0" });
            iList.Add(new SelectListItem { Text = "9th", Value = "9" });
            iList.Add(new SelectListItem { Text = "10th", Value = "10" });
            iList.Add(new SelectListItem { Text = "11th", Value = "11" });
            iList.Add(new SelectListItem { Text = "12th", Value = "12" });
            return iList;
        }

        public List<SelectListItem> GetAllPSEBCLASS_5to12()
        {
            List<SelectListItem> iList = new List<SelectListItem>();
            iList.Add(new SelectListItem { Text = "None", Value = "0" });
            iList.Add(new SelectListItem { Text = "5th Class Level", Value = "5" });
            iList.Add(new SelectListItem { Text = "8th Class Level", Value = "8" });
            iList.Add(new SelectListItem { Text = "9th Class Level", Value = "9" });
            iList.Add(new SelectListItem { Text = "10th Class Level", Value = "10" });
            iList.Add(new SelectListItem { Text = "11th Class Level", Value = "11" });
            iList.Add(new SelectListItem { Text = "12th Class Level", Value = "12" });
            return iList;
        }


        #region BookSalesMaster DBclass
        public DataSet GetBookSalesMaster(string CLS, int AdminId)
        {
            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());//
                    //SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
                    SqlCommand cmd = new SqlCommand("BookSalesMaster_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Class", CLS);
                    cmd.Parameters.AddWithValue("@USERID", AdminId);

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
        public DataSet GetBookSalesClass()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetBookSalesClass_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public DataSet UpdateBookSalesMaster(int AdminId,string CLASS, string BOOKID, string SOS, string SALES, string SBALANCE, string AMOUNT, string DOS, string DOB, string DBALANCE)
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UpdateBookSalesMaster_Sp", con);//ReGenerateChallaanByIdSPAdminNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CLASSID", CLASS);
                    cmd.Parameters.AddWithValue("@BOOKID", BOOKID);
                    cmd.Parameters.AddWithValue("@SOS", SOS);
                    cmd.Parameters.AddWithValue("@SALES", SALES);
                    cmd.Parameters.AddWithValue("@SBALANCE", SBALANCE);
                    cmd.Parameters.AddWithValue("@AMOUNT", AMOUNT);
                    cmd.Parameters.AddWithValue("@DOS", DOS);
                    cmd.Parameters.AddWithValue("@DOB", DOB);
                    cmd.Parameters.AddWithValue("@DBALANCE", DBALANCE);
                    cmd.Parameters.AddWithValue("@IPNM", myIP);
                    cmd.Parameters.AddWithValue("@USERID", AdminId);
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
        #endregion BookSalesMaster DBclass

        public DataSet BulkMail()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Open_AddtionalFees_Issue_Data_13092018SP", con);//
                    cmd.CommandType = CommandType.StoredProcedure;
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



        public DataSet jsCheckAadharDuplicate(string AadharNo)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("jsCheckAadharDuplicate_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AadharNo", AadharNo);
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
    }
}