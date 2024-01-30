using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using PSEBONLINE.Models;
using System.Web.Mvc;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace PSEBONLINE.AbstractLayer
{
    public class PrivateCandidateDB
    {
        #region Check ConString

        private string CommonCon = "myDBConnection";
        public PrivateCandidateDB()
        {
            if (HttpContext.Current.Session["Session"] == null)
            {
                CommonCon = "myDBConnection";
            }

            else
            {
                CommonCon = "myDBConnection";
            }
        }
        #endregion  Check ConString


        public static List<PrivateCandidateCategoryMasters> GetPrivateCandidateCategoryMasterListByBatchForAdmin(int type, string SelBatch, string SelBatchYear)
        {
            List<PrivateCandidateCategoryMasters> PrivateCandidateCategoryMasters = new List<PrivateCandidateCategoryMasters>();
            try
            {
                using (DBContext context = new DBContext())
                {
                    DateTime today = DateTime.Now.Date;
                    if (type == 1)// active list
                    {

                        PrivateCandidateCategoryMasters = context.PrivateCandidateCategoryMasters
                           .Where(s => s.Batch == SelBatch && s.BatchYear == SelBatchYear &&
                           s.IsActive == true).ToList();


                    }
                    else
                    {
                        PrivateCandidateCategoryMasters = context.PrivateCandidateCategoryMasters
                            .Where(s => s.Batch == SelBatch && s.BatchYear == SelBatchYear && s.IsActive == true).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return PrivateCandidateCategoryMasters;
        }

        public static List<PrivateCandidateCategoryMasters> GetPrivateCandidateCategoryMasterListByBatch(int type, string SelBatch, string SelBatchYear)
        {
            List<PrivateCandidateCategoryMasters> PrivateCandidateCategoryMasters = new List<PrivateCandidateCategoryMasters>();
            try
            {
                using (DBContext context = new DBContext())
                {
                    DateTime today = DateTime.Now.Date;
                    if (type == 1)// active list
                    {
                        PrivateCandidateCategoryMasters = context.PrivateCandidateCategoryMasters
                           .Where(s => s.Batch == SelBatch && s.BatchYear == SelBatchYear &&
                           s.IsActive == true).ToList();

                        //PrivateCandidateCategoryMasters = context.PrivateCandidateCategoryMasters
                        //.Where(s => s.Batch == SelBatch && s.BatchYear == SelBatchYear &&
                        //s.IsActive == true && System.Data.Entity.DbFunctions.TruncateTime(s.LastDate) >= today).ToList();
                    }
                    else
                    {
                        PrivateCandidateCategoryMasters = context.PrivateCandidateCategoryMasters
                            .Where(s => s.Batch == SelBatch && s.BatchYear == SelBatchYear && s.IsActive == true).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return PrivateCandidateCategoryMasters;
        }


        public static List<PrivateCandidateCategoryMasters> GetPrivateCandidateCategoryMasterList(int type)
        {
            List<PrivateCandidateCategoryMasters> PrivateCandidateCategoryMasters = new List<PrivateCandidateCategoryMasters>();
            try
            {
                using (DBContext context = new DBContext())
                {
                    DateTime today = DateTime.Now.Date;

                    if (type == 1)// active list
                    {

                        PrivateCandidateCategoryMasters = context.PrivateCandidateCategoryMasters
                           .Where(s => s.IsActive == true && System.Data.Entity.DbFunctions.TruncateTime(s.LastDate) >= today).ToList();
                    }
                    else
                    {
                        PrivateCandidateCategoryMasters = context.PrivateCandidateCategoryMasters.Where(s => s.IsActive == true).ToList();
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return PrivateCandidateCategoryMasters;
        }



        public DataSet GetAllBatch()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetAllBatch", con);
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

        #region Private Admit Card (Compartment)
        public DataSet AdmitCardPrivateSearch(string Search, int cls)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AdmitCardPrivateSearch", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@class", cls);
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
        public DataSet GetCompartmentPrivateAdmitCardByRefNo(PrivateCandidateModels rm)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCompartmentPrivateAdmitCardByRefNo", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", rm.refNo);
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

        #endregion Private Admit Card (Compartment)

        #region Private Admit Card Matric
        public DataSet GetFinalPrintPrivateMatricAdmitCardSearch(string Search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetFinalPrintPrivateCandMatricAdmitCardSearch_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    //cmd.Parameters.AddWithValue("@refno", rm.refNo);
                    //cmd.Parameters.AddWithValue("@CandType", rm.Exam_Type);

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
        public DataSet GetFinalPrintPrivateMatricAdmitCard(PrivateCandidateModels rm)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetFinalPrintPrivateCandMatricAdmitCard_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", rm.refNo);
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

        #endregion Private Admit Card Matric

        #region Private Admit Card 
        public DataSet GetFinalPrintPrivateAdmitCardSearch(string Search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetFinalPrintPrivateCandAdmitCardSearch_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    //cmd.Parameters.AddWithValue("@refno", rm.refNo);
                    //cmd.Parameters.AddWithValue("@CandType", rm.Exam_Type);

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
        public DataSet GetFinalPrintPrivateAdmitCard(PrivateCandidateModels rm)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetFinalPrintPrivateCandAdmitCard_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", rm.refNo);
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

        #endregion Private Admit Card 
        public DataSet SelectDist()
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
                    SqlCommand cmd = new SqlCommand("GetAllDistrict", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@SCHL", schid);

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
        //---------------Select AllTehsil
        public DataSet SelectAllTehsilEC(int DISTID)
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
                    SqlCommand cmd = new SqlCommand("GetAllTehsilEC", con);
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
        public DataSet SelectForMigrateSchools(string Search)
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
                    SqlCommand cmd = new SqlCommand("SelectForMigrateSchools_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);

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

        public DataSet GetAllFormName(string SCHL)
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
                    SqlCommand cmd = new SqlCommand("GetAllFormName_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);

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
        public DataSet GetAllLot(string SCHL)
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
                    SqlCommand cmd = new SqlCommand("GetAllLot_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);

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
        public DataSet GetRegEntryviewMigrate(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetRegEntryviewMigrate_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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
        public DataSet GetRegEntryviewMigrate_Search(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetRegEntryviewMigrate_Search_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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
        public DataSet SearchSchoolDetailsRA(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SearchSchoolDetailsRA_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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

        public DataSet GetStudentDetailsSchoolRA(string stdID)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetStudentDetailsSchoolRA_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@stdID", stdID);
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


        //----------------------------------------------1-Oct-2016--------------------------

        public DataSet PrivateRefrenceUnlockPage(PrivateCandidateModels MS)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PrivateRefrenceUnlockPage_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refNo", MS.refNo);
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
        public DataSet GetDetailPC(PrivateCandidateModels MS)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetDetailPC_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refNo", MS.refNo);
                    cmd.Parameters.AddWithValue("@oroll", MS.OROLL);
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
        public DataSet GetSessionYear()
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetSessionYear_sp", con);
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

        public DataSet GetSessionMonth()
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetSessionMonth_sp", con);
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

        //----------------------------------------------- On Additional selection get month and session year---------------------

        public List<SelectListItem> GetMonth()
        {
            List<SelectListItem> MonthList = new List<SelectListItem>();
            //MonthList.Add(new SelectListItem { Text = "Select Month", Value = "0" });
            MonthList.Add(new SelectListItem { Text = "JANUARY", Value = "JANUARY" });
            MonthList.Add(new SelectListItem { Text = "FEBUARY", Value = "FEBUARY" });
            MonthList.Add(new SelectListItem { Text = "MARCH", Value = "MARCH" });
            MonthList.Add(new SelectListItem { Text = "APRIL", Value = "APRIL" });
            MonthList.Add(new SelectListItem { Text = "MAY", Value = "MAY" });
            MonthList.Add(new SelectListItem { Text = "JUNE", Value = "JUNE" });
            MonthList.Add(new SelectListItem { Text = "JULY", Value = "JULY" });
            MonthList.Add(new SelectListItem { Text = "AUGUST", Value = "AUGUST" });
            MonthList.Add(new SelectListItem { Text = "SEPTEMBER", Value = "SEPTEMBER" });
            MonthList.Add(new SelectListItem { Text = "OCTOBER", Value = "OCTOBER" });
            MonthList.Add(new SelectListItem { Text = "NOVEMBER", Value = "NOVEMBER" });
            MonthList.Add(new SelectListItem { Text = "DECEMBER", Value = "DECEMBER" });
            return MonthList;
        }

        public List<SelectListItem> GetSessionYear1()
        {
            //DataTable dsSession = SessionMaster(); // passing Value to SchoolDB from model
            //List<SelectListItem> itemSession = new List<SelectListItem>();
            //foreach (System.Data.DataRow dr in dsSession.Rows)
            //{
            //    itemSession.Add(new SelectListItem { Text = @dr["yearfrom"].ToString(), Value = @dr["yearfrom"].ToString() });
            //}
            //return itemSession;
            List<SelectListItem> itemSession = new List<SelectListItem>();
            itemSession.Add(new SelectListItem { Text = "1950", Value = "1950" });
            itemSession.Add(new SelectListItem { Text = "1951", Value = "1951" });
            itemSession.Add(new SelectListItem { Text = "1952", Value = "1952" });
            itemSession.Add(new SelectListItem { Text = "1953", Value = "1953" });
            itemSession.Add(new SelectListItem { Text = "1954", Value = "1954" });
            itemSession.Add(new SelectListItem { Text = "1955", Value = "1955" });
            itemSession.Add(new SelectListItem { Text = "1956", Value = "1956" });
            itemSession.Add(new SelectListItem { Text = "1957", Value = "1957" });
            itemSession.Add(new SelectListItem { Text = "1958", Value = "1958" });
            itemSession.Add(new SelectListItem { Text = "1959", Value = "1959" });
            itemSession.Add(new SelectListItem { Text = "1960", Value = "1960" });

            itemSession.Add(new SelectListItem { Text = "1961", Value = "1961" });
            itemSession.Add(new SelectListItem { Text = "1962", Value = "1962" });
            itemSession.Add(new SelectListItem { Text = "1963", Value = "1963" });
            itemSession.Add(new SelectListItem { Text = "1964", Value = "1964" });
            itemSession.Add(new SelectListItem { Text = "1965", Value = "1965" });
            itemSession.Add(new SelectListItem { Text = "1966", Value = "1966" });
            itemSession.Add(new SelectListItem { Text = "1967", Value = "1967" });
            itemSession.Add(new SelectListItem { Text = "1968", Value = "1968" });
            itemSession.Add(new SelectListItem { Text = "1969", Value = "1969" });
            itemSession.Add(new SelectListItem { Text = "1970", Value = "1970" });

            itemSession.Add(new SelectListItem { Text = "1971", Value = "1971" });
            itemSession.Add(new SelectListItem { Text = "1972", Value = "1972" });
            itemSession.Add(new SelectListItem { Text = "1973", Value = "1973" });
            itemSession.Add(new SelectListItem { Text = "1974", Value = "1974" });
            itemSession.Add(new SelectListItem { Text = "1975", Value = "1975" });
            itemSession.Add(new SelectListItem { Text = "1976", Value = "1976" });
            itemSession.Add(new SelectListItem { Text = "1977", Value = "1977" });
            itemSession.Add(new SelectListItem { Text = "1978", Value = "1978" });
            itemSession.Add(new SelectListItem { Text = "1979", Value = "1979" });
            itemSession.Add(new SelectListItem { Text = "1980", Value = "1980" });

            itemSession.Add(new SelectListItem { Text = "1981", Value = "1981" });
            itemSession.Add(new SelectListItem { Text = "1982", Value = "1982" });
            itemSession.Add(new SelectListItem { Text = "1983", Value = "1983" });
            itemSession.Add(new SelectListItem { Text = "1984", Value = "1984" });
            itemSession.Add(new SelectListItem { Text = "1985", Value = "1985" });
            itemSession.Add(new SelectListItem { Text = "1986", Value = "1986" });
            itemSession.Add(new SelectListItem { Text = "1987", Value = "1987" });
            itemSession.Add(new SelectListItem { Text = "1988", Value = "1988" });
            itemSession.Add(new SelectListItem { Text = "1989", Value = "1989" });
            itemSession.Add(new SelectListItem { Text = "1990", Value = "1990" });

            itemSession.Add(new SelectListItem { Text = "1991", Value = "1991" });
            itemSession.Add(new SelectListItem { Text = "1992", Value = "1992" });
            itemSession.Add(new SelectListItem { Text = "1993", Value = "1993" });
            itemSession.Add(new SelectListItem { Text = "1994", Value = "1994" });
            itemSession.Add(new SelectListItem { Text = "1995", Value = "1995" });
            itemSession.Add(new SelectListItem { Text = "1996", Value = "1996" });
            itemSession.Add(new SelectListItem { Text = "1997", Value = "1997" });
            itemSession.Add(new SelectListItem { Text = "1998", Value = "1998" });
            itemSession.Add(new SelectListItem { Text = "1999", Value = "1999" });
            itemSession.Add(new SelectListItem { Text = "2000", Value = "2000" });

            itemSession.Add(new SelectListItem { Text = "2001", Value = "2001" });
            itemSession.Add(new SelectListItem { Text = "2002", Value = "2002" });
            itemSession.Add(new SelectListItem { Text = "2003", Value = "2003" });
            itemSession.Add(new SelectListItem { Text = "2004", Value = "2004" });
            itemSession.Add(new SelectListItem { Text = "2005", Value = "2005" });
            itemSession.Add(new SelectListItem { Text = "2006", Value = "2006" });
            itemSession.Add(new SelectListItem { Text = "2007", Value = "2007" });
            itemSession.Add(new SelectListItem { Text = "2008", Value = "2008" });
            itemSession.Add(new SelectListItem { Text = "2009", Value = "2009" });
            itemSession.Add(new SelectListItem { Text = "2010", Value = "2010" });
            itemSession.Add(new SelectListItem { Text = "2011", Value = "2011" });
            itemSession.Add(new SelectListItem { Text = "2012", Value = "2012" });
            itemSession.Add(new SelectListItem { Text = "2013", Value = "2013" });
            itemSession.Add(new SelectListItem { Text = "2014", Value = "2014" });
            itemSession.Add(new SelectListItem { Text = "2015", Value = "2015" });
            itemSession.Add(new SelectListItem { Text = "2016", Value = "2016" });
            itemSession.Add(new SelectListItem { Text = "2017", Value = "2017" });
            itemSession.Add(new SelectListItem { Text = "2018", Value = "2018" });
            itemSession.Add(new SelectListItem { Text = "2019", Value = "2019" });
            itemSession.Add(new SelectListItem { Text = "2020", Value = "2020" });
            itemSession.Add(new SelectListItem { Text = "2021", Value = "2021" });
            itemSession.Add(new SelectListItem { Text = "2022", Value = "2022" });
            itemSession.Add(new SelectListItem { Text = "2023", Value = "2023" });
            return itemSession;
        }
        public DataTable SessionMaster()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SessionMasterPrivateSP", con);
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

        public DataSet GetPrivateCandidateDetails(string oroll, string Clss, string Yar, string Mnth, string Cat)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetPrivateCandidateDetails_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OROLL", oroll);
                    cmd.Parameters.AddWithValue("@Class", Clss);
                    cmd.Parameters.AddWithValue("@SelYear", Yar);
                    cmd.Parameters.AddWithValue("@SelMonth", Mnth);
                    cmd.Parameters.AddWithValue("@Cat", Cat);
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

        public DataSet GetPrivateCandidateDetailsNew(string oroll, string Clss, string Yar, string Mnth, string Cat, string batch, string batchYear)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetPrivateCandidateDetails_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@OROLL", oroll);
                    cmd.Parameters.AddWithValue("@Class", Clss);
                    cmd.Parameters.AddWithValue("@SelYear", Yar);
                    cmd.Parameters.AddWithValue("@SelMonth", Mnth);
                    cmd.Parameters.AddWithValue("@Cat", Cat);
                    cmd.Parameters.AddWithValue("@batch", batch);
                    cmd.Parameters.AddWithValue("@batchYear", batchYear);
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
        public List<SelectListItem> GetDA()
        {
            List<SelectListItem> DList = new List<SelectListItem>();
            DList.Add(new SelectListItem { Text = "N.A.", Value = "N.A." });
            DList.Add(new SelectListItem { Text = "Blindness", Value = "Blindness" });
            DList.Add(new SelectListItem { Text = "Low-Vision", Value = "Low-Vision" });
            DList.Add(new SelectListItem { Text = "Leprosy Cured Person", Value = "Leprosy Cured Person" });
            DList.Add(new SelectListItem { Text = "Hearing Imparment (deaf and hard of hearing)", Value = "Hearing Imparment (deaf and hard of hearing)" });
            DList.Add(new SelectListItem { Text = "Locomotor Disability", Value = "Locomotor Disability" });
            DList.Add(new SelectListItem { Text = "Dwarfism", Value = "Dwarfism" });
            DList.Add(new SelectListItem { Text = "Intellectual Disability", Value = "Intellectual Disability" });
            DList.Add(new SelectListItem { Text = "Mental Illness", Value = "Mental Illness" });
            DList.Add(new SelectListItem { Text = "Autism Spectrum Disorder", Value = "Autism Spectrum Disorder" });
            DList.Add(new SelectListItem { Text = "Cerebral Palsy", Value = "Cerebral Palsy" });
            DList.Add(new SelectListItem { Text = "Muscular Dystrophy", Value = "Muscular Dystrophy" });
            DList.Add(new SelectListItem { Text = "Chronic Neurological Conditions", Value = "Chronic Neurological Conditions" });
            DList.Add(new SelectListItem { Text = "Specific Learning Disabilities", Value = "Specific Learning Disabilities" });
            DList.Add(new SelectListItem { Text = "Multiple Sclerosis", Value = "Multiple Sclerosis" });
            DList.Add(new SelectListItem { Text = "Speech and Language disability", Value = "Speech and Language disability" });
            DList.Add(new SelectListItem { Text = "Thalassemia", Value = "Thalassemia" });
            DList.Add(new SelectListItem { Text = "Hemophilia", Value = "Hemophilia" });
            DList.Add(new SelectListItem { Text = "Sickle Cell disease", Value = "Sickle Cell disease" });
            DList.Add(new SelectListItem { Text = "Multiple Disabilities including deafblindness", Value = "Multiple Disabilities including deafblindness" });
            DList.Add(new SelectListItem { Text = "Acid Attack victim", Value = "Acid Attack victim								" });
            DList.Add(new SelectListItem { Text = "Parkinsons disease", Value = "Parkinsons disease" });
            //DList.Add(new SelectListItem { Text = "Blind/Visually Impaired", Value = "Blind" });
            //DList.Add(new SelectListItem { Text = "Deaf & Dumb/Hearing Impaired", Value = "Deaf&Dumb" });
            //DList.Add(new SelectListItem { Text = "Mentally Retired", Value = "Mental" });
            //DList.Add(new SelectListItem { Text = "Physically Handicapped", Value = "Physically" });
            //DList.Add(new SelectListItem { Text = "Others", Value = "Others" });

            return DList;
        }
        public DataSet InsertPrivateCandidateForm(PrivateCandidateModels MS)  // Type 1=Regular, 2=Open
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    //SqlCommand cmd = new SqlCommand("InsertPrivateCandidateForm_SP", con);// for October-2017 batch-10
                    SqlCommand cmd = new SqlCommand("InsertPrivateCandidateForm_SP_MAR2018", con);//for March-2018 batch-3

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Exam_Type", MS.Exam_Type);
                    cmd.Parameters.AddWithValue("@category", MS.category);
                    cmd.Parameters.AddWithValue("@Class", MS.Class);
                    cmd.Parameters.AddWithValue("@SelYear", MS.SelYear);
                    cmd.Parameters.AddWithValue("@SelMonth", MS.SelMonth);
                    cmd.Parameters.AddWithValue("@OROLL", MS.OROLL);
                    cmd.Parameters.AddWithValue("@emailID", MS.emailID);
                    cmd.Parameters.AddWithValue("@mobileNo", MS.mobileNo);
                    cmd.Parameters.AddWithValue("@batch", MS.batch);
                    cmd.Parameters.AddWithValue("@batchYear", MS.batchYear);
                    cmd.Parameters.AddWithValue("@OtherBoard", MS.Other_Board);
                    cmd.Parameters.AddWithValue("@board", MS.Board);
                    cmd.CommandTimeout = 600;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;
                }
            }
            catch (Exception ex)
            {
                //return result = -1;
                return result = null;
            }
            finally
            {
                // con.Close();
            }
        }


        public DataSet GeneratePrivateCandidateFormRefno(PrivateCandidateModels MS)  // Type 1=Regular, 2=Open
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("GeneratePrivateCandidateFormRefno_SP", con);//InsertSMFSP   //InsertSMFSPNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Exam_Type", MS.Exam_Type);
                    cmd.Parameters.AddWithValue("@category", MS.category);
                    cmd.Parameters.AddWithValue("@Class", MS.Class);
                    cmd.Parameters.AddWithValue("@SelYear", MS.SelYear);
                    cmd.Parameters.AddWithValue("@SelMonth", MS.SelMonth);
                    //cmd.Parameters.AddWithValue("@OROLL", MS.OROLL);

                    cmd.Parameters.AddWithValue("@resultdtl", MS.Result);
                    cmd.Parameters.AddWithValue("@regno", MS.RegNo);
                    cmd.Parameters.AddWithValue("@name", MS.Candi_Name);
                    cmd.Parameters.AddWithValue("@pname", MS.Pname);
                    cmd.Parameters.AddWithValue("@fname", MS.Father_Name);
                    cmd.Parameters.AddWithValue("@pfname", MS.PFname);
                    cmd.Parameters.AddWithValue("@mname", MS.Mother_Name);
                    cmd.Parameters.AddWithValue("@pmname", MS.PMname);
                    cmd.Parameters.AddWithValue("@dob", MS.DOB);

                    cmd.Parameters.AddWithValue("@emailID", MS.emailID);
                    cmd.Parameters.AddWithValue("@mobileNo", MS.mobileNo);
                    cmd.Parameters.AddWithValue("@OROLL", MS.OROLL);
                    cmd.Parameters.AddWithValue("@batch", MS.batch);
                    cmd.Parameters.AddWithValue("@batchYear", MS.batchYear);

                    //con.Open();
                    //result = cmd.ExecuteNonQuery();
                    ////result = (int)cmd.Parameters["@GetCorrectionNo"].Value;
                    //return result;
                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;

                }
            }
            catch (Exception ex)
            {
                //return result = -1;
                return result = null;
            }
            finally
            {
                // con.Close();
            }
        }

        public DataSet GetPrivateCandidateConfirmation(string refno)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetPrivateCandidateConfirmation_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", refno);
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

        public static DataSet InsertPrivateCandidateConfirmation(PrivateCandidateModels MS, out string OutError)
        {
            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.CommandText = "InsertPrivateCandidateConfirmationMain";
                cmd.Parameters.AddWithValue("@DisabilityPercent", MS.DisabilityPercent);
                cmd.Parameters.AddWithValue("@IsHardCopyCertificate", MS.IsHardCopyCertificate);
                cmd.Parameters.AddWithValue("@SelExamDist", MS.SelExamDist);
                cmd.Parameters.AddWithValue("@Gender", MS.Gender);
                cmd.Parameters.AddWithValue("@CastList", MS.CastList);
                cmd.Parameters.AddWithValue("@Area", MS.Area);
                cmd.Parameters.AddWithValue("@Relist", MS.Relist);
                cmd.Parameters.AddWithValue("@IsphysicalChall", MS.IsphysicalChall);
                cmd.Parameters.AddWithValue("@rdoWantWriter", MS.rdoWantWriter);
                cmd.Parameters.AddWithValue("@IsPracExam", MS.IsPracExam);
                cmd.Parameters.AddWithValue("@Choice1", MS.Choice1);
                cmd.Parameters.AddWithValue("@Choice2", MS.Choice2);

                cmd.Parameters.AddWithValue("@regno", MS.RegNo);
                cmd.Parameters.AddWithValue("@name", MS.Candi_Name);
                cmd.Parameters.AddWithValue("@pname", MS.Pname);
                cmd.Parameters.AddWithValue("@fname", MS.Father_Name);
                cmd.Parameters.AddWithValue("@pfname", MS.PFname);
                cmd.Parameters.AddWithValue("@mname", MS.Mother_Name);
                cmd.Parameters.AddWithValue("@pmname", MS.PMname);
                cmd.Parameters.AddWithValue("@dob", MS.DOB);

                cmd.Parameters.AddWithValue("@mobileNo", MS.mobileNo);
                cmd.Parameters.AddWithValue("@emailID", MS.emailID);
                cmd.Parameters.AddWithValue("@OROLL", MS.OROLL);
                cmd.Parameters.AddWithValue("@refno", MS.refNo);
                cmd.Parameters.AddWithValue("@Addess", MS.address);
                cmd.Parameters.AddWithValue("@landmark", MS.landmark);
                cmd.Parameters.AddWithValue("@block", MS.block);
                cmd.Parameters.AddWithValue("@SelDist", MS.SelDist);
                cmd.Parameters.AddWithValue("@tehsil", MS.tehsil);
                cmd.Parameters.AddWithValue("@pinCode", MS.pinCode);

                cmd.Parameters.AddWithValue("@cat", MS.category);
                cmd.Parameters.AddWithValue("@Class", MS.Class);

                cmd.Parameters.AddWithValue("@sub1", MS.sub1);
                cmd.Parameters.AddWithValue("@sub2", MS.sub2);
                cmd.Parameters.AddWithValue("@sub3", MS.sub3);
                cmd.Parameters.AddWithValue("@sub4", MS.sub4);
                cmd.Parameters.AddWithValue("@sub5", MS.sub5);
                cmd.Parameters.AddWithValue("@sub6", MS.sub6);
                cmd.Parameters.AddWithValue("@sub7", MS.sub7);
                cmd.Parameters.AddWithValue("@sub8", MS.sub8);
                cmd.Parameters.AddWithValue("@PathPhoto", MS.PathPhoto);
                cmd.Parameters.AddWithValue("@PathSign", MS.PathSign);

                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                //
                ds = db.ExecuteDataSet(cmd);
                OutError = (string)cmd.Parameters["@OutError"].Value;

                return ds;

            }
            catch (Exception ex)
            {

                OutError = "-1";
                return null;
            }

        }



        public DataSet UpdateAdminPrivateCandidateConfirmation(PrivateCandidateModels MS)  // Type 1=Regular, 2=Open
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    //SqlCommand cmd = new SqlCommand("InsertPrivateCandidateConfirmation_SP29nov", con);//InsertSMFSP   //InsertSMFSPNew
                    //SqlCommand cmd = new SqlCommand("InsertPrivateCandidateConfirmation_SP30Dec", con);
                    SqlCommand cmd = new SqlCommand("UpdateAdminPrivateCandidateConfirmation_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Exam_Type", MS.Exam_Type);
                    cmd.Parameters.AddWithValue("@Stream", MS.Stream);

                    cmd.Parameters.AddWithValue("@refno", MS.refNo);
                    cmd.Parameters.AddWithValue("@OROLL", MS.OROLL);
                    cmd.Parameters.AddWithValue("@Session", MS.SelMonth);
                    cmd.Parameters.AddWithValue("@year", MS.SelYear);
                    cmd.Parameters.AddWithValue("@Result", MS.Result);
                    cmd.Parameters.AddWithValue("@Remarks", MS.SearchString);

                    cmd.Parameters.AddWithValue("@SelExamDist", MS.SelExamDist);
                    cmd.Parameters.AddWithValue("@Gender", MS.Gender);
                    cmd.Parameters.AddWithValue("@CastList", MS.CastList);
                    cmd.Parameters.AddWithValue("@Area", MS.Area);
                    cmd.Parameters.AddWithValue("@Relist", MS.Relist);
                    cmd.Parameters.AddWithValue("@IsphysicalChall", MS.IsphysicalChall);
                    cmd.Parameters.AddWithValue("@rdoWantWriter", MS.rdoWantWriter);
                    cmd.Parameters.AddWithValue("@IsPracExam", MS.IsPracExam);
                    cmd.Parameters.AddWithValue("@Choice1", MS.Choice1);
                    cmd.Parameters.AddWithValue("@Choice2", MS.Choice2);

                    cmd.Parameters.AddWithValue("@regno", MS.RegNo);
                    cmd.Parameters.AddWithValue("@name", MS.Candi_Name);
                    cmd.Parameters.AddWithValue("@pname", MS.Pname);
                    cmd.Parameters.AddWithValue("@fname", MS.Father_Name);
                    cmd.Parameters.AddWithValue("@pfname", MS.PFname);
                    cmd.Parameters.AddWithValue("@mname", MS.Mother_Name);
                    cmd.Parameters.AddWithValue("@pmname", MS.PMname);
                    cmd.Parameters.AddWithValue("@dob", MS.DOB);

                    cmd.Parameters.AddWithValue("@mobileNo", MS.mobileNo);
                    cmd.Parameters.AddWithValue("@emailID", MS.emailID);
                    cmd.Parameters.AddWithValue("@Addess", MS.address);
                    cmd.Parameters.AddWithValue("@landmark", MS.landmark);
                    cmd.Parameters.AddWithValue("@block", MS.block);
                    cmd.Parameters.AddWithValue("@SelDist", MS.SelDist);
                    cmd.Parameters.AddWithValue("@tehsil", MS.tehsil);
                    cmd.Parameters.AddWithValue("@pinCode", MS.pinCode);

                    cmd.Parameters.AddWithValue("@cat", MS.category);
                    cmd.Parameters.AddWithValue("@Class", MS.Class);

                    cmd.Parameters.AddWithValue("@sub1", MS.sub1);
                    cmd.Parameters.AddWithValue("@sub2", MS.sub2);
                    cmd.Parameters.AddWithValue("@sub3", MS.sub3);
                    cmd.Parameters.AddWithValue("@sub4", MS.sub4);
                    cmd.Parameters.AddWithValue("@sub5", MS.sub5);
                    cmd.Parameters.AddWithValue("@sub6", MS.sub6);
                    cmd.Parameters.AddWithValue("@sub7", MS.sub7);
                    cmd.Parameters.AddWithValue("@sub8", MS.sub8);
                    cmd.Parameters.AddWithValue("@PathPhoto", MS.PathPhoto);
                    cmd.Parameters.AddWithValue("@PathSign", MS.PathSign);
                    cmd.Parameters.AddWithValue("@IsHardCopyCertificate", MS.IsHardCopyCertificate);
                    cmd.Parameters.AddWithValue("@AdminId", MS.AdminId);
                    cmd.Parameters.AddWithValue("@EmpUserId", MS.EmpUserId);

                    ad.SelectCommand = cmd;
                    ad.Fill(result);
                    con.Open();
                    return result;

                }
            }
            catch (Exception ex)
            {
                //return result = -1;
                return result = null;
            }
            finally
            {
                // con.Close();
            }
        }

        public int EditPrivateCandidateConfirmation(PrivateCandidateModels MS)  // Type 1=Regular, 2=Open
        {
            int result;
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("EditPrivateCandidateConfirmation_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", MS.refNo);
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

        public DataSet UnlockFinalSubmitPrivateCandidate(string refno)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("UnlockFinalSubmitPrivateCandidateSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", refno);
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


        public DataSet GetPrivateCandidateConfirmationFinalSubmit(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetPrivateCandidateConfirmationFinalSubmit_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", search);
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
        public DataSet GetPrivateCandidateConfirmationPrint(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetPrivateCandidateConfirmationPrint_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", search);
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

        public DataSet SetUpdateSyncResultDetails(string refno, string oRoll)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("SetUpdateSyncResultDetails_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", refno);
                    cmd.Parameters.AddWithValue("@oRoll", oRoll);
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

        //--------------------------------------Challan-------------


        public DataSet GetPrivateCandidateDetailsPayment(string RefNo, string form)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetPrivateCandidateDetailsPaymentSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", RefNo);
                    //cmd.Parameters.AddWithValue("@roll", roll);
                    cmd.Parameters.AddWithValue("@form", form);

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
        public DataSet CheckFeeStatus(string SCHL, string type)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CheckFeeStatusSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@type", type);
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

        //public DataSet GetCalculateFeeBySchool(string search)
        public DataSet GetCalculateFeeBySchool(string search, string schl)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCalculateFeeBySchoolSPNew", con);//GetCalculateFeeBySchoolSP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@Schl", schl);
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

        public string InsertPaymentFormPrivate_OLD(PrivateChallanMasterModel CM, FormCollection frm, out string CandiMobile)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("InsertPaymentFormSP", con);   //InsertPaymentFormSP // InsertPaymentFormSP_Rohit
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SchoolCode", CM.SchoolCode);
                cmd.Parameters.AddWithValue("@CHLNDATE", CM.CHLNDATE);
                cmd.Parameters.AddWithValue("@CHLNVDATE", CM.CHLNVDATE);
                cmd.Parameters.AddWithValue("@FEEMODE", CM.FEEMODE);
                cmd.Parameters.AddWithValue("@FEECODE", CM.FEECODE);
                cmd.Parameters.AddWithValue("@FEECAT", CM.FEECAT);
                cmd.Parameters.AddWithValue("@BCODE", CM.BCODE);
                cmd.Parameters.AddWithValue("@BANK", CM.BANK);
                cmd.Parameters.AddWithValue("@ACNO", CM.ACNO);
                cmd.Parameters.AddWithValue("@FEE", CM.FEE);
                cmd.Parameters.AddWithValue("@BANKCHRG", CM.BANKCHRG);
                cmd.Parameters.AddWithValue("@TOTFEE", CM.TOTFEE);
                cmd.Parameters.AddWithValue("@SCHLREGID", CM.SCHLREGID);
                cmd.Parameters.AddWithValue("@DIST", CM.DIST);
                cmd.Parameters.AddWithValue("@DISTNM", CM.DISTNM);
                cmd.Parameters.AddWithValue("@SCHLCANDNM", CM.SCHLCANDNM);
                cmd.Parameters.AddWithValue("@BRCODE", CM.BRCODE);
                cmd.Parameters.AddWithValue("@BRANCH", CM.BRANCH);
                //cmd.Parameters.AddWithValue("@DEPOSITDT", CM.DEPOSITDT);
                cmd.Parameters.AddWithValue("@addfee", CM.addfee);
                cmd.Parameters.AddWithValue("@latefee", CM.latefee);
                cmd.Parameters.AddWithValue("@prosfee", CM.prosfee);
                cmd.Parameters.AddWithValue("@addsubfee", CM.addsubfee);
                cmd.Parameters.AddWithValue("@add_sub_count", CM.add_sub_count);
                cmd.Parameters.AddWithValue("@regfee", CM.regfee);
                cmd.Parameters.AddWithValue("@type", CM.type);
                cmd.Parameters.AddWithValue("@LOT", CM.LOT);
                cmd.Parameters.AddWithValue("@FeeStudentList", CM.FeeStudentList);
                if (CM.LSFRemarks != null && CM.LSFRemarks != "")
                {
                    cmd.Parameters.AddWithValue("@LumsumFine", CM.LumsumFine);
                    cmd.Parameters.AddWithValue("@LSFRemarks", CM.LSFRemarks);
                }
                //cmd.Parameters.AddWithValue("@ChallanGDateN", CM.ChallanGDateN);
                cmd.Parameters.AddWithValue("@ChallanVDateN", CM.ChallanVDateN);
                //  cmd.Parameters.Add("@CHALLANID", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                SqlParameter outPutParameter = new SqlParameter();
                outPutParameter.ParameterName = "@CHALLANID";
                outPutParameter.Size = 100;
                outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                outPutParameter.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(outPutParameter);
                SqlParameter outPutParameter1 = new SqlParameter();
                outPutParameter1.ParameterName = "@SchoolMobile";
                outPutParameter1.Size = 20;
                outPutParameter1.SqlDbType = System.Data.SqlDbType.VarChar;
                outPutParameter1.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(outPutParameter1);
                // cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = (string)cmd.Parameters["@CHALLANID"].Value;
                CandiMobile = (string)cmd.Parameters["@SchoolMobile"].Value;
                // string OutError = (string)cmd.Parameters["@OutError"].Value;
                return outuniqueid;
            }
            catch (Exception ex)
            {
                //mbox(ex);
                CandiMobile = "";
                return result = "";

            }
            finally
            {
                con.Close();
            }
        }

        public string InsertPaymentFormPrivate(PrivateChallanMasterModel CM, FormCollection frm, out string CandiMobile)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("InsertPaymentFormMainSP", con);   //InsertPaymentFormSP //InsertPaymentFormSPTest  // [InsertPaymentFormSP_Rohit]
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 300;
                cmd.Parameters.AddWithValue("@EmpUserId", CM.EmpUserId);
                cmd.Parameters.AddWithValue("@SchoolCode", CM.SchoolCode);
                cmd.Parameters.AddWithValue("@CHLNDATE", CM.CHLNDATE);
                cmd.Parameters.AddWithValue("@CHLNVDATE", CM.CHLNVDATE);
                cmd.Parameters.AddWithValue("@FEEMODE", CM.FEEMODE);
                cmd.Parameters.AddWithValue("@FEECODE", CM.FEECODE);
                cmd.Parameters.AddWithValue("@FEECAT", CM.FEECAT);
                cmd.Parameters.AddWithValue("@BCODE", CM.BCODE);
                cmd.Parameters.AddWithValue("@BANK", CM.BANK);
                cmd.Parameters.AddWithValue("@ACNO", CM.ACNO);
                cmd.Parameters.AddWithValue("@FEE", CM.FEE);
                cmd.Parameters.AddWithValue("@BANKCHRG", CM.BANKCHRG);
                cmd.Parameters.AddWithValue("@TOTFEE", CM.TOTFEE);
                cmd.Parameters.AddWithValue("@SCHLREGID", CM.SCHLREGID);
                cmd.Parameters.AddWithValue("@DIST", CM.DIST);
                cmd.Parameters.AddWithValue("@DISTNM", CM.DISTNM);
                cmd.Parameters.AddWithValue("@SCHLCANDNM", CM.SCHLCANDNM);
                cmd.Parameters.AddWithValue("@BRCODE", CM.BRCODE);
                cmd.Parameters.AddWithValue("@BRANCH", CM.BRANCH);
                //cmd.Parameters.AddWithValue("@DEPOSITDT", CM.DEPOSITDT);
                cmd.Parameters.AddWithValue("@addfee", CM.addfee);
                cmd.Parameters.AddWithValue("@latefee", CM.latefee);
                cmd.Parameters.AddWithValue("@prosfee", CM.prosfee);
                cmd.Parameters.AddWithValue("@addsubfee", CM.addsubfee);
                cmd.Parameters.AddWithValue("@add_sub_count", CM.add_sub_count);
                cmd.Parameters.AddWithValue("@regfee", CM.regfee);
                cmd.Parameters.AddWithValue("@type", CM.type);
                cmd.Parameters.AddWithValue("@LOT", CM.LOT);
                cmd.Parameters.AddWithValue("@FeeStudentList", CM.FeeStudentList);
                if (CM.LSFRemarks != null && CM.LSFRemarks != "")
                {
                    cmd.Parameters.AddWithValue("@LumsumFine", CM.LumsumFine);
                    cmd.Parameters.AddWithValue("@LSFRemarks", CM.LSFRemarks);
                }
                //cmd.Parameters.AddWithValue("@ChallanGDateN", CM.ChallanGDateN);
                cmd.Parameters.AddWithValue("@ChallanVDateN", CM.ChallanVDateN);
                cmd.Parameters.Add("@CHALLANID", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@SchoolMobile", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string OutError = (string)cmd.Parameters["@OutError"].Value;
                string outuniqueid = (string)cmd.Parameters["@CHALLANID"].Value;
                CandiMobile = (string)cmd.Parameters["@SchoolMobile"].Value;

                return outuniqueid;

            }
            catch (Exception ex)
            {
                //mbox(ex);
                CandiMobile = ex.Message;
                return result = "0";

            }
            finally
            {
                con.Close();
            }
        }


        public string ReGenerateChallaanById(string ChallanId, string usertype, string BCODE)  // ReGenerateChallaan
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            string outuniqueid = "";
            string result = "";

            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ReGenerateChallaanByIdSPNew_PVT", con);//ReGenerateChallaanByIdSPNew_amar //ReGenerateChallaanByIdSPNew ReGenerateChallaanByIdSP
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CHALLANID", ChallanId);
                    cmd.Parameters.AddWithValue("@type", usertype);
                    cmd.Parameters.AddWithValue("@BANKCODE", BCODE);
                    SqlParameter outPutParameter = new SqlParameter();
                    outPutParameter.ParameterName = "@OutStatus";
                    outPutParameter.Size = 20;
                    outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                    outPutParameter.Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add(outPutParameter);

                    con.Open();
                    result = cmd.ExecuteNonQuery().ToString();
                    outuniqueid = (string)cmd.Parameters["@OutStatus"].Value;
                    return outuniqueid;

                }
            }
            catch (Exception ex)
            {
                outuniqueid = "-1";
                return null;
            }
        }

        public DataSet GetChallanDetailsById(string ChallanId)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetChallanDetailsByIdPrivateSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CHALLANID", ChallanId);

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


        public DataSet GetFinalPrintChallan(string SchoolCode)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetFinalPrintChallanSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SchoolCode", SchoolCode);
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

        public DataSet CheckChallanIsVerified(string SCHL, int LOT)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CheckChallanIsVerified", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SCHL", SCHL);
                    cmd.Parameters.AddWithValue("@LOT", LOT);
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


        public DataSet getForgotPassword(PrivateCandidateModels MS)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("getForgotPassword_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Exam_Type", MS.Exam_Type);
                    cmd.Parameters.AddWithValue("@category", MS.category);
                    cmd.Parameters.AddWithValue("@Class", MS.Class);
                    cmd.Parameters.AddWithValue("@SelYear", MS.SelYear);
                    cmd.Parameters.AddWithValue("@SelMonth", MS.SelMonth);
                    cmd.Parameters.AddWithValue("@OROLL", MS.OROLL);
                    cmd.Parameters.AddWithValue("@emailID", MS.emailID);
                    cmd.Parameters.AddWithValue("@mobileNo", MS.mobileNo);
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

        #region Photo and Image Upload and Email 
        public DataSet InsertimgUpdPvt(PrivateCandidateModels MS)  // photo and sign update by candidates using email link
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //SqlCommand cmd = new SqlCommand("InsertPrivateCandidateConfirmation_SP29nov", con);//InsertSMFSP   //InsertSMFSPNew
                    SqlCommand cmd = new SqlCommand("InsertimgUpdPvt_SP", con);//InsertSMFSP   //InsertSMFSPNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refNo", MS.refNo);
                    cmd.Parameters.AddWithValue("@PathPhoto", MS.PathPhoto);
                    cmd.Parameters.AddWithValue("@PathSign", MS.PathSign);
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
        public DataSet imgUpdPvtemail(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("imgUpdPvtemail_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
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

        #endregion Photo and Image Upload and Email 

        #region Private Final Print using Admin Login
        public DataSet AdminFPPrivateSearch(string Search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AdminFPPrivateSearch_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
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
        public DataSet AdminFPPrivate(PrivateCandidateModels rm)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AdminFPPrivate_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", rm.refNo);
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

        #endregion Private Final Print using Admin Login

        #region PvtPracticalExamCentre
        public DataSet PvtPracticalExamCentre(string Search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PvtPracticalExamCentre", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
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
        #endregion PvtPracticalExamCentre

        #region Private DIC pricatical subject update DB
        public DataSet PrivateSearchDIC_Prac(string Search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //SqlCommand cmd = new SqlCommand("AdmitCardPrivateSearch", con);
                    SqlCommand cmd = new SqlCommand("PrivateSearchDIC_Prac", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    //cmd.Parameters.AddWithValue("@class", cls);
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
        public string upd_PrivateSearchDIC_Prac(string refno, string prac)
        {
            string result = "";
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //SqlCommand cmd = new SqlCommand("CreateClusterNew_Sp", con);//ReGenerateChallaanByIdSPAdminNew                    
                    SqlCommand cmd = new SqlCommand("upd_PrivateSearchDIC_Prac", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", refno);
                    cmd.Parameters.AddWithValue("@prac", prac);

                    con.Open();
                    result = cmd.ExecuteNonQuery().ToString();
                    if (con.State == ConnectionState.Open)
                        con.Close();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }
        #endregion Private DIC pricatical subject update DB

        #region Candidates to View Practical Exam Centre Details. Regular,Open, Re-Appear/Additional/DIC,
        public DataSet CandViewPracExamCentDtl(string Search, string Exam_Type)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CandViewPracExamCentDtl_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@Exam_Type", Exam_Type);
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


        #endregion Candidates to View Practical Exam Centre Details. Regular,Open, Re-Appear/Additional/DIC,

        public string Updated_PrivateCandidate_PhotoSign_ByRefNo(string refno, string PhotoSignName, string Type)
        {
            string result = "";
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("Updated_PrivateCandidate_PhotoSign_ByRefNoSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", refno);
                    cmd.Parameters.AddWithValue("@PhotoSignName", PhotoSignName);
                    cmd.Parameters.AddWithValue("@Type", Type);
                    con.Open();
                    result = cmd.ExecuteNonQuery().ToString();
                    return result;
                }
            }
            catch (Exception ex)
            {
                return result = "";
            }
        }


        public DataSet PrivateCandidateStatusCheck(string Search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PrivateCandidateStatusCheckSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
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