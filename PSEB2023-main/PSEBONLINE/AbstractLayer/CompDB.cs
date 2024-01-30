using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using PSEBONLINE.Models;
using System.Web.Mvc;

namespace PSEBONLINE.AbstractLayer
{
    public class CompDB
    {
        public DataSet SelectDist()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
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
                return result = null;
            }
        }
        public DataSet SelectAllTehsil(int DISTID)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {                 
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
        //----------------------------------------------- On Additional selection get month and session year---------------------
        public DataSet GetSessionYear()
        {

            // SqlConnection con = null;
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();

            try
            {

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
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

                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
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
        public List<SelectListItem> GetMonth()
        {
            List<SelectListItem> MonthList = new List<SelectListItem>();
            //MonthList.Add(new SelectListItem { Text = "Select Month", Value = "0" });
            MonthList.Add(new SelectListItem { Text = "JANUARY", Value = "JANUARY" });
            MonthList.Add(new SelectListItem { Text = "FEBUARY", Value = "FEBUARY" });
            MonthList.Add(new SelectListItem { Text = "MARCH", Value = "MARCH" });
            MonthList.Add(new SelectListItem { Text = "APRIL", Value = "APRIL" });
            MonthList.Add(new SelectListItem { Text = "MAY", Value = "MAY" });
            MonthList.Add(new SelectListItem { Text = "JUN", Value = "JUN" });
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
            return itemSession;
        }

        //internal DataSet InsertCompCandidateForm(PrivateCandidateModels mS)
        //{
        //    throw new NotImplementedException();
        //}

        public DataTable SessionMaster()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
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
        public DataSet GetDetailCCM(CompartmentCandidatesModel MS)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetDetailCCM_sp", con);
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
        public DataSet InsertCompCandidateForm(CompartmentCandidatesModel MS)  // Type 1=Regular, 2=Open
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("InsertCompCandidateForm_SP", con);//InsertSMFSP   //InsertCompCandidateForm_SP
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
                //return result = -1;
                return result = null;
            }
            finally
            {
                // con.Close();
            }
        }
        public DataSet GetCompCandidateDetails(string oroll, string Clss, string Yar, string Mnth, string Cat)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCompCandidateDetails_sp", con);
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
        public DataSet GenerateCompCandidateFormRefno(CompartmentCandidatesModel MS)  // Type 1=Regular, 2=Open
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("GenerateCompCandidateFormRefno_SP", con);//InsertSMFSP   //InsertSMFSPNew
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

        public DataSet GetCompCandidateConfirmation(string refno)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCompCandidateConfirmation_sp", con);
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
        public DataSet CHKCompCand(string refno,string roll)  
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("CHKCompCand_sp", con);//InsertSMFSP   //InsertSMFSPNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", refno);
                    cmd.Parameters.AddWithValue("@roll", roll);
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
        public DataSet InsertCompCandidateConfirmation(CompartmentCandidatesModel MS)  // Type 1=Regular, 2=Open
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("InsertCompCandidateConfirmation_SP29nov", con);//InsertSMFSP   //InsertSMFSPNew
                    cmd.CommandType = CommandType.StoredProcedure;
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
                //return result = -1;
                return result = null;
            }
            finally
            {
                // con.Close();
            }
        }

        public int EditCompCandidateConfirmation(CompartmentCandidatesModel MS)  // Type 1=Regular, 2=Open
        {
            int result;
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("EditCompCandidateConfirmation_SP", con);
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
        public DataSet GetPCompCandidateConfirmationPrint(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetompConfirmationPrint_sp", con);
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

        public DataSet GetCompCandidateConfirmationFinalSubmit(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCompCandidateConfirmationFinalSubmit_sp", con);
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
        //-------------------------------------For Challan---------------------------//
        public DataSet BankStatus()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BankStatusSP", con);
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
        public DataSet BankStatusCode(string bcode)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("BankStatusName_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@BCODE", bcode);
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
        public DataSet GetCompCandidateDetailsPayment(string RefNo, string form)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetCompCandidateDetailsPaymentSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", RefNo);                   
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
        public string InsertPaymentFormComp(CompChallanMasterModel CM, FormCollection frm, out string CandiMobile)
        {
            SqlConnection con = null;
            string result = "";
            try
            {
                con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
                SqlCommand cmd = new SqlCommand("InsertPaymentFormCompSP", con);   //InsertPaymentFormSP_Test  // [InsertPaymentFormSP_Rohit]
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@roll", CM.roll);
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
                // cmd.Parameters.AddWithValue("@FeeStudentList", CM.FeeStudentList);
                // cmd.Parameters.AddWithValue("@ChallanGDateN", CM.ChallanGDateN);
                cmd.Parameters.AddWithValue("@ChallanVDateN", CM.ChallanVDateN);
                //  cmd.Parameters.Add("@CHALLANID", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
                SqlParameter outPutParameter = new SqlParameter();
                outPutParameter.ParameterName = "@CHALLANID";
                outPutParameter.Size = 100;
                outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
                outPutParameter.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(outPutParameter);
                SqlParameter outPutParameter1 = new SqlParameter();
                outPutParameter1.ParameterName = "@CandiMobile";
                outPutParameter1.Size = 20;
                outPutParameter1.SqlDbType = System.Data.SqlDbType.VarChar;
                outPutParameter1.Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(outPutParameter1);
                con.Open();
                result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = (string)cmd.Parameters["@CHALLANID"].Value;
                CandiMobile = (string)cmd.Parameters["@CandiMobile"].Value;
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

        public DataSet GetChallanDetailsByIdComp(string ChallanId)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetChallanDetailsByIdCompSP", con);
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
        public DataSet getForgotPassword(PrivateCandidateModels MS)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("getForgotPasswordCompCand_sp", con);
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


    }
}