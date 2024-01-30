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
using System.Threading;

using PSEBONLINE.Repository;
using System.Threading.Tasks;
using System.Reflection;

namespace PSEBONLINE.AbstractLayer
{
	public class SchoolDB : ISchoolRepository
	{

		private Database _database;

		private DBContext context;

		#region Check ConString
		private string CommonCon = "myDBConnection";        
        public SchoolDB()
		{
			context = new DBContext();
			CommonCon = "myDBConnection";
		}

		#endregion  Check ConString


		public static DataSet GetCentreBySchl(string schl)
		{
			try
			{
				DataSet ds = new DataSet();
				Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = 300;
				cmd.CommandText = "GetCentreBySchlSp";
				cmd.Parameters.AddWithValue("@SCHL", schl);
				ds = db.ExecuteDataSet(cmd);
				return ds;
			}
			catch (Exception ex)
			{
				return null;
			}
		}


		public static LoginSession LoginSenior(LoginModel LM)  // Type 1=Regular, 2=Open
		{
			LoginSession loginSession = new LoginSession();
			try
			{
				Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "LoginSeniorSP";// LoginSP(old)
				cmd.Parameters.AddWithValue("@UserName", LM.username);
				cmd.Parameters.AddWithValue("@Password", LM.Password);
				using (IDataReader reader = db.ExecuteReader(cmd))
				{
					if (reader.Read())
					{
						loginSession.PRINCIPAL = DBNull.Value != reader["PRINCIPAL"] ? (string)reader["PRINCIPAL"] : default(string);
						loginSession.STATUS = DBNull.Value != reader["STATUS"] ? (string)reader["STATUS"] : default(string);
						loginSession.DIST = DBNull.Value != reader["DIST"] ? (string)reader["DIST"] : default(string);
						loginSession.SCHL = DBNull.Value != reader["SCHL"] ? (string)reader["SCHL"] : default(string);
						//
						loginSession.middle = DBNull.Value != reader["middle"] ? (string)reader["middle"] : default(string);
						loginSession.fifth = DBNull.Value != reader["fifth"] ? (string)reader["fifth"] : default(string);
						//
						loginSession.Senior = DBNull.Value != reader["middle"] ? (string)reader["Senior"] : default(string);
						loginSession.OSenior = DBNull.Value != reader["fifth"] ? (string)reader["OSenior"] : default(string);
						loginSession.Matric = DBNull.Value != reader["middle"] ? (string)reader["Matric"] : default(string);
						loginSession.OMATRIC = DBNull.Value != reader["fifth"] ? (string)reader["OMATRIC"] : default(string);
						//
						loginSession.Approved = DBNull.Value != reader["Approved"] ? (bool)reader["Approved"] : default(bool);
						loginSession.MOBILE = DBNull.Value != reader["MOBILE"] ? (string)reader["MOBILE"] : default(string);
						loginSession.EMAILID = DBNull.Value != reader["EMAILID"] ? (string)reader["EMAILID"] : default(string);
						loginSession.LoginStatus = DBNull.Value != reader["LoginStatus"] ? (int)reader["LoginStatus"] : default(int);
						loginSession.DateFirstLogin = DBNull.Value != reader["DateFirstLogin"] ? (DateTime)reader["DateFirstLogin"] : default(DateTime);
						loginSession.SCHLNME = DBNull.Value != reader["SCHLNME"] ? (string)reader["SCHLNME"] : default(string);
						loginSession.SCHLNMP = DBNull.Value != reader["SCHLNMP"] ? (string)reader["SCHLNMP"] : default(string);
						//
						//loginSession.EXAMCENTSCHLN = DBNull.Value != reader["EXAMCENTSCHLN"] ? (string)reader["EXAMCENTSCHLN"] : default(string);
						loginSession.EXAMCENT = DBNull.Value != reader["EXAMCENT"] ? (string)reader["EXAMCENT"] : default(string);
						loginSession.PRACCENT = DBNull.Value != reader["PRACCENT"] ? (string)reader["PRACCENT"] : default(string);
						loginSession.USERTYPE = DBNull.Value != reader["SCHLNME"] ? (string)reader["USERTYPE"] : default(string);
						loginSession.CLUSTERDETAILS = DBNull.Value != reader["CLUSTERDETAILS"] ? (string)reader["CLUSTERDETAILS"] : default(string);
						//
						loginSession.IsMeritoriousSchool = DBNull.Value != reader["IsMeritoriousSchool"] ? (int)reader["IsMeritoriousSchool"] : default(int);
						loginSession.IsPrivateExam = DBNull.Value != reader["IsPrivateExam"] ? (int)reader["IsPrivateExam"] : default(int);
						loginSession.IsAllowPSTET = DBNull.Value != reader["IsAllowPSTET"] ? (int)reader["IsAllowPSTET"] : default(int);

						loginSession.DealingBranchContact = DBNull.Value != reader["DealingBranchContact"] ? (string)reader["DealingBranchContact"] : default(string);
					}
				}
				Thread.Sleep(2000);
			}
			catch (Exception ex)
			{
				loginSession = null;
			}
			return loginSession;
		}

		public static DataSet MeritoriousCentreMaster(string schl)
		{
			try
			{
				DataSet ds = new DataSet();
				Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = 300;
				cmd.CommandText = "MeritoriousCentreMasterSp"; //InsertOnlinePaymentMISSP // InsertOnlinePaymentMISSPNEW
				cmd.Parameters.AddWithValue("@schl", schl);
				ds = db.ExecuteDataSet(cmd);
				//result = db.ExecuteNonQuery(cmd);                
				return ds;

			}
			catch (Exception ex)
			{

				return null;
			}

		}

		public AffiliationFee MagazineSchoolRequirementsFeeDetails(int Cat, string schl, string RefNo, DateTime dt1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myDBConnection"].ToString()))
				{
					SqlCommand cmd = new SqlCommand("MagazineSchoolRequirementsFeeDetailsSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.Clear();
					cmd.Parameters.AddWithValue("@Cat", Cat);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@RefNo", RefNo);
					cmd.Parameters.AddWithValue("@Challandt", dt1);
					ad.SelectCommand = cmd;
					ad.Fill(result);
					con.Open();
					try
					{
						if (result.Tables[0].Rows.Count > 0)
						{
							AffiliationFee _affiliationFee = new AffiliationFee();
							DataRow dr = result.Tables[0].Rows[0];
							_affiliationFee.SCHL = dr["SCHL"].ToString();
							_affiliationFee.Class = dr["Class"].ToString();
							_affiliationFee.Form = dr["Form"].ToString();
							_affiliationFee.sDate = dr["sDate"].ToString();
							_affiliationFee.eDate = Convert.ToString(dr["eDate"].ToString());
							_affiliationFee.fee = Convert.ToInt32(dr["fee"].ToString());
							_affiliationFee.latefee = Convert.ToInt32(dr["latefee"].ToString());
							_affiliationFee.totfee = Convert.ToInt32(dr["totfee"].ToString());
							_affiliationFee.FEECODE = Convert.ToInt32(dr["FEECODE"].ToString());
							_affiliationFee.FEECAT = Convert.ToString(dr["FEECAT"].ToString());
							_affiliationFee.BankLastdate = Convert.ToString(dr["BankLastDate"].ToString());
							_affiliationFee.Type = dr["Type"].ToString();
							_affiliationFee.AllowBanks = dr["AllowBanks"].ToString();
							_affiliationFee.TotalFeesInWords = dr["TotalFeesInWords"].ToString();
							_affiliationFee.ChallanCategory = Cat;
							return _affiliationFee;
						}
						else
						{
							return new AffiliationFee();
						}
					}
					catch (Exception ex)
					{
						return new AffiliationFee();
					}
				}
			}
			catch (Exception ex)
			{
				return new AffiliationFee();
			}
		}


		public DataSet GetAssignExamGroupBySchl(string Schl)
		{
			DataSet ds = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetAssignExamGroupBySchlSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Schl", Schl);
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


		public DataSet GetpracticalMarks_Schl(string schl, string cls)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				//using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("PracticalChart_SP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@cls", cls);
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


		public DataSet RegSchoolList(string search, int PageNumber, int PageSize)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("RegSchoolListSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@PageNumber", PageNumber);
					cmd.Parameters.AddWithValue("@PageSize", PageSize);
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

		public DataSet RegSchoolReport(string search)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("RegSchoolReport", con);
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



		#region Book Demand
		public List<SelectListItem> GetAllBookClass()
		{
			string OutError = "";
			DataSet dsBD = BookDemand(1, "BOOKID like '%%'", "", out OutError);
			// English
			List<SelectListItem> classList = new List<SelectListItem>();
			foreach (System.Data.DataRow dr in dsBD.Tables[0].Rows)
			{
				classList.Add(new SelectListItem { Text = @dr["ClassName"].ToString().Trim(), Value = @dr["Class"].ToString().Trim() });
			}
			return classList;
		}

		public DataSet BookDemand(int type, string Search, string schl, out string OutError)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("BookDemandSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					// cmd.CommandTimeout = 120;
					cmd.Parameters.AddWithValue("@type", type);
					cmd.Parameters.AddWithValue("@Search", Search);
					cmd.Parameters.AddWithValue("@schl", schl);
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
				OutError = "-1";
				return null;
			}
		}

		public string BookAssessmentForm(BookAssessmentForm obj, int type1, out string OutError)
		{
			SqlConnection con = null;
			string result = "";
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("BookAssessmentFormSP", con); //DuplicateCertificateSP
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@Id", obj.id);
				cmd.Parameters.AddWithValue("@SCHL", obj.SCHL);
				cmd.Parameters.AddWithValue("@Class", obj.Class);
				//cmd.Parameters.AddWithValue("@BOOKID", obj.BOOKID);
				//cmd.Parameters.AddWithValue("@BOOK_NM", obj.BOOK_NM);
				//cmd.Parameters.AddWithValue("@TOT_BOOK", obj.TOT_BOOK);
				cmd.Parameters.AddWithValue("@TOT_STUD", obj.TOT_STUD);
				cmd.Parameters.AddWithValue("@Type", type1);
				cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
				cmd.Parameters.AddWithValue("@BookRequestDT", obj.BookRequestDT);
				cmd.Parameters.AddWithValue("@UpdatedBy", 0);
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

		#endregion Book Demand



		#region  School Grievance Management System for Support

		public string RegistrationForTraining(RegistrationForTraining obj, out string OutError)
		{
			SqlConnection con = null;
			string result = "";
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("RegistrationForTrainingSP", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@Schl", obj.schl);
				cmd.Parameters.AddWithValue("@Name", obj.SchoolRepresentative.Trim());
				cmd.Parameters.AddWithValue("@Designation", obj.Designation.Trim());
				cmd.Parameters.AddWithValue("@Mobile", obj.cpmobile.Trim());
				cmd.Parameters.AddWithValue("@Email", obj.cpemail.Trim());
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



		public List<SelectListItem> GetClasses(string schl)
		{
			string classes = string.Empty;
			schl = "00" + schl;
			SqlConnection con = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["myConn2017"].ConnectionString);
			SqlCommand cmd = new SqlCommand("GetSchoolType", con);
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@schl", schl.Substring(schl.Length - 7, 7).ToString());
			SqlDataAdapter adp = new SqlDataAdapter(cmd);
			DataSet ds = new DataSet();
			adp.Fill(ds);
			try
			{
				string matric = ds.Tables[1].Rows[0]["Matric"].ToString();
				string senior = ds.Tables[1].Rows[0]["Senior"].ToString();
				if (senior.Trim() == "1")
				{
					if (matric.Trim() == "1")
					{
						classes = "3";      //both matric and senior
					}
					else
					{
						classes = "2";      //only senior
					}
				}
				else
				{
					if (matric.Trim() == "1")
					{
						classes = "1";      //only matric
					}
					else
					{
						classes = "0";      //neither matric nor senior
					}
				}
			}
			catch (Exception e)
			{
				classes = "0";
			}
			List<SelectListItem> clases = new List<SelectListItem>();
			clases.Add(new SelectListItem { Text = "--Select Class Name--", Value = "" });
			if (classes.Trim() == "1")
			{
				clases.Add(new SelectListItem { Text = "9", Value = "9" });
				clases.Add(new SelectListItem { Text = "10", Value = "10" });
				clases.Add(new SelectListItem { Text = "Others", Value = "Others" });
			}
			else if (classes == "2")
			{
				clases.Add(new SelectListItem { Text = "11", Value = "11" });
				clases.Add(new SelectListItem { Text = "12", Value = "12" });
				clases.Add(new SelectListItem { Text = "Others", Value = "Others" });
			}
			else if (classes == "3")
			{
				clases.Add(new SelectListItem { Text = "9", Value = "9" });
				clases.Add(new SelectListItem { Text = "10", Value = "10" });
				clases.Add(new SelectListItem { Text = "11", Value = "11" });
				clases.Add(new SelectListItem { Text = "12", Value = "12" });
				clases.Add(new SelectListItem { Text = "Others", Value = "Others" });
			}
			else
			{
				clases.Add(new SelectListItem { Text = "Others", Value = "Others" });
			}
			return clases;
		}

		public DataTable GrievanceMaster()
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn2017"].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GrievanceMasterSP", con);
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

		public List<SelectListItem> FormsList(int? num)
		{
			DataTable dsGrievance = GrievanceMaster(); // passing Value to SchoolDB from model
			List<SelectListItem> listItems1 = new List<SelectListItem>();
			listItems1.Add(new SelectListItem { Text = "--Select Grievance--", Value = "" });
			foreach (System.Data.DataRow dr in dsGrievance.Rows)
			{
				listItems1.Add(new SelectListItem { Text = @dr["Grievance"].ToString(), Value = @dr["Grievance"].ToString() });
				//listItems1.Add(new SelectListItem { Text = @dr["Grievance"].ToString(), Value = @dr["Id"].ToString() });
			}
			return listItems1;
		}

		public string insertUpdateCallCenterForm(CallCenter obj, out string OutTicket)
		{
			SqlConnection con = null;
			string result = "";
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn2017"].ToString());
				SqlCommand cmd = new SqlCommand("pro_callcenterform", con);
				cmd.CommandType = CommandType.StoredProcedure;
				if (obj.ccfid != 0)
				{
					cmd.Parameters.AddWithValue("@ccfid", obj.ccfid);
					cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
					cmd.Parameters.AddWithValue("@UpdatedBy", obj.UpdatedBy);
					cmd.Parameters.AddWithValue("@remarks", obj.remarks);
					cmd.Parameters.AddWithValue("@status", obj.status);
				}
				else
				{
					cmd.Parameters.AddWithValue("@CreatedBy", obj.schoolcode);
				}
				cmd.Parameters.AddWithValue("@schoolcode", obj.schoolcode);
				cmd.Parameters.AddWithValue("@schoolname", obj.schoolname);
				cmd.Parameters.AddWithValue("@district", obj.district);
				cmd.Parameters.AddWithValue("@classname", (obj.classname == null) ? string.Empty : obj.classname);
				cmd.Parameters.AddWithValue("@formname", obj.formname);
				cmd.Parameters.AddWithValue("@cpname", obj.cpname);
				cmd.Parameters.AddWithValue("@cpmobile", obj.cpmobile);
				cmd.Parameters.AddWithValue("@cpemail", obj.cpemail);
				cmd.Parameters.AddWithValue("@description", obj.description);
				cmd.Parameters.AddWithValue("@photo", obj.photo);
				cmd.Parameters.AddWithValue("@pdf", obj.pdf);
				cmd.Parameters.Add("@OutId", SqlDbType.Int).Direction = ParameterDirection.Output;
				cmd.Parameters.AddWithValue("@sessionyear", obj.sessionyear);
				cmd.Parameters.Add("@OutTicket", SqlDbType.VarChar, 15).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery().ToString();
				string outuniqueid = Convert.ToString(cmd.Parameters["@OutId"].Value);
				OutTicket = Convert.ToString(cmd.Parameters["@OutTicket"].Value);
				return outuniqueid;

			}
			catch (Exception ex)
			{
				//mbox(ex);
				OutTicket = "";
				return result = "-1";
			}
			finally
			{
				con.Close();
			}
		}

		public void pro_checkloginstatUsercodeDist(string schoolcode, out string schoolname, out string districtname, out string verifylogin, out string outid)
		{
			SqlConnection con = null;
			string result = "";
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("pro_checkloginstatUsercodeDist", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@schoolcode", schoolcode);
				cmd.Parameters.Add("@schoolname", SqlDbType.NVarChar, 950).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@districtname", SqlDbType.NVarChar, 950).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@verifylogin", SqlDbType.NVarChar, 50).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@OutId", SqlDbType.Int).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery().ToString();
				outid = Convert.ToString(cmd.Parameters["@OutId"].Value);
				districtname = Convert.ToString(cmd.Parameters["@districtname"].Value);
				verifylogin = Convert.ToString(cmd.Parameters["@verifylogin"].Value);
				schoolname = Convert.ToString(cmd.Parameters["@schoolname"].Value);
			}
			catch (Exception ex)
			{
				schoolname = "";
				districtname = "";
				outid = "";
				verifylogin = "";
			}
			finally
			{
				con.Close();
			}
		}

		public DataSet GetStudentCallCenterRecordsSearchSCHL(string schl, string search, int pageIndex, int pagesize = 50)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn2017"].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetStudentCallCenterRecordsSearchSCHL", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@PageIndex", pageIndex);
					cmd.Parameters.AddWithValue("@PageSize", pagesize);
					ad.SelectCommand = cmd;
					ad.Fill(result);
					// int count = Convert.ToInt32(cmd.Parameters["@PageCount"].Value);
					con.Open();
					return result;
				}
			}
			catch (Exception ex)
			{
				return result = null;
			}
		}

		#endregion School Grievance Management System for Support




		#region schl Result Update
		public DataSet GetSchoolRecordsSearch(string search, string year)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{

				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetSchoolRecordsSearchSchl", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@year", year);

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
		public int UpdateStudentRecords(SchoolModels am, string year)
		{
			SqlConnection con = null;
			int result = 0;
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("UpdateStudentRecordsDSCHL", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@stdid", am.id);
				cmd.Parameters.AddWithValue("@TotalMarks", am.TotalMarks);
				cmd.Parameters.AddWithValue("@ObtainedMarks", am.ObtainedMarks);
				cmd.Parameters.AddWithValue("@Result", am.Result);
				cmd.Parameters.AddWithValue("@reclock", am.reclock);
				cmd.Parameters.AddWithValue("@EXAM", am.EXAM);
				cmd.Parameters.AddWithValue("@year", year);
				cmd.Parameters.AddWithValue("@SCHL", am.SCHL);
				cmd.Parameters.Add("@OutId", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery();
				//int outuniqueid = (int)cmd.Parameters["@OutId"].Value;
				return result;

			}
			catch (Exception ex)
			{
				return result = -1;
			}
			finally
			{
				con.Close();
			}
		}
		public int FinalsubmitResult(SchoolModels am)
		{
			SqlConnection con = null;
			int result = 0;
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("SPFinalsubmitResultSchl", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@SCHL", am.SchlCode);
				cmd.Parameters.Add("@OutId", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery();
				//int outuniqueid = (int)cmd.Parameters["@OutId"].Value;
				return result;
			}
			catch (Exception ex)
			{
				return result = -1;
			}
			finally
			{
				con.Close();
			}
		}
		#endregion schl Result Update

		#region Begin CCE_Senior

		public string AllotCCESenior(string stdid, DataTable dtSub, string class1, out int OutStatus)  // BankLoginSP
		{
			SqlConnection con = null;
			string result = "";
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("AllotCCESenior", con);  //AllotCCESenior
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@stdid", stdid);
				cmd.Parameters.AddWithValue("@dtSub", dtSub);
				cmd.Parameters.AddWithValue("@class", class1);
				cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery().ToString();
				OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
				return result;

			}
			catch (Exception ex)
			{
				OutStatus = -1;
				//mbox(ex);
				return result = "";
			}
			finally
			{
				con.Close();
			}
		}

		public DataSet SchoolAllowForCCE(string SCHL, string cls, string PanelName)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SchoolAllowForCCESP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@SCHL", SCHL);
					cmd.Parameters.AddWithValue("@cls", cls);
					cmd.Parameters.AddWithValue("@PanelName", PanelName);
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

		public DataSet GetAllFormName(string SCHL)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
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

		public DataSet GetDataBySCHL(string search, string schl, int pageNumber, string class1, int action1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetDataBySCHL", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
					cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
					cmd.Parameters.AddWithValue("@PageSize", 20);
					cmd.Parameters.AddWithValue("@Action", action1);
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


		public DataSet CCEREPORT(string search, string schl, string class1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("CCEREPORT", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
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

		public DataSet CCEREPORTFinalSubmit(string search, string schl, string class1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("CCEREPORTFinalSubmit", con);//[CCEREPORTFinalSubmit]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
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


		public DataSet CCEFinalReport(string search, string schl, string class1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("CCEFinalReport", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 250;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
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

		#endregion

		#region  Grading

		public string AllotGrading(string stdid, DataTable dtSub, string class1, out int OutStatus)  // BankLoginSP
		{
			SqlConnection con = null;
			string result = "";
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("AllotSub9", con);  //AllotGrading  
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@stdid", stdid);
				cmd.Parameters.AddWithValue("@dtSub", dtSub);
				cmd.Parameters.AddWithValue("@class", class1);
				cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery().ToString();
				OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
				return result;

			}
			catch (Exception ex)
			{
				OutStatus = -1;
				//mbox(ex);
				return result = "";
			}
			finally
			{
				con.Close();
			}
		}


		public DataSet GetDataBySCHLGrading(string search, string schl, int pageNumber, string class1, int action1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetDataBySCHLGrading", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
					cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
					cmd.Parameters.AddWithValue("@PageSize", 20);
					cmd.Parameters.AddWithValue("@Action", action1);
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

		public DataSet GradingREPORT(string search, string schl, string class1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GradingREPORT", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 120;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
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

		public DataSet GradingREPORTFinalSubmit(string search, string schl, string class1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GradingREPORTFinalSubmit", con);//[CCEREPORTFinalSubmit]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
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


		public DataSet GradingFinalReport(string search, string schl, string class1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GradingFinalReport", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 120;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
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
		#endregion Grading

		public string Updated_Bulk_Pic_Data_Open(string Myresult, string PhotoSignName, string Type, string SchlID)
		{
			SqlConnection con = null;
			string result = "";
			try
			{

				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("Uploaded_Bulk_Photo_Sign_Open", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@StudentUniqueID", Myresult);
				cmd.Parameters.AddWithValue("@PhotoSignName", PhotoSignName);
				cmd.Parameters.AddWithValue("@Type", Type);
				cmd.Parameters.AddWithValue("@SchlID", SchlID);

				con.Open();
				result = cmd.ExecuteNonQuery().ToString();
				return result;

			}
			catch (Exception ex)
			{
				//mbox(ex);
				return result = "";
			}
			finally
			{
				con.Close();
			}
		}

		//--Start--------- Signature Chart and Confidential List Matric ------------
		public DataSet SignatureChartMatricSub(SchoolModels sm)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SignatureChartMatricSub_Sp", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cent", sm.ExamCent);
					//cmd.Parameters.AddWithValue("@ExamSub", sm.ExamSub);
					//cmd.Parameters.AddWithValue("@roll", sm.ExamRoll);
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
		public DataSet SignatureChartMatric(SchoolModels sm)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					string roll = "";
					if (sm.ExamRoll != "")
					{
						roll = "and roll='" + sm.ExamRoll + "'";
					}
					//SqlCommand cmd = new SqlCommand("GetSignatureChartMatric_New", con);
					SqlCommand cmd = new SqlCommand("GetSignatureChart_SP2702", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cent", sm.ExamCent);
					cmd.Parameters.AddWithValue("@sub", sm.ExamSub);
					cmd.Parameters.AddWithValue("@roll", roll);
					cmd.Parameters.AddWithValue("@class", sm.CLASS);
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


		public DataSet ConfidentialListMatric(SchoolModels sm)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetConfi_SP2702", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Cent", sm.ExamCent);
					cmd.Parameters.AddWithValue("@class", sm.CLASS);
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


		//--End--------- Signature Chart and Confidential List Matric------------

		//--Start--------- Signature Chart and Confidential List ------------
		public DataSet SignatureChartSr(SchoolModels sm)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					string roll = "";
					if (sm.ExamRoll != "")
					{
						roll = "and roll='" + sm.ExamRoll + "'";
					}

					//SqlCommand cmd = new SqlCommand("GetSignatureChartSenior", con);
					SqlCommand cmd = new SqlCommand("GetSignatureChart_SP2702", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 300;
					cmd.Parameters.AddWithValue("@cent", sm.ExamCent);
					cmd.Parameters.AddWithValue("@sub", sm.ExamSub);
					cmd.Parameters.AddWithValue("@roll", roll);
					cmd.Parameters.AddWithValue("@class", sm.CLASS);
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
		public DataSet SignatureChartSrSub(SchoolModels sm)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SignatureChartSrSub_Sp", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cent", sm.ExamCent);
					cmd.Parameters.AddWithValue("@ExamSub", sm.ExamSub);
					//cmd.Parameters.AddWithValue("@roll", sm.ExamRoll);
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
		public DataSet ConfidentialListSenior(SchoolModels sm)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetConfi_SP2702", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 300;
					cmd.Parameters.AddWithValue("@Cent", sm.ExamCent);
					cmd.Parameters.AddWithValue("@class", sm.CLASS);
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

		public DataSet GetCentcode(string SCHL1)
		{
			// SqlConnection con = null;
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetConfidentialCentcode_SP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@SCHL", SCHL1);
					ad.SelectCommand = cmd;
					ad.Fill(result);
					//con.Open();
					return result;
				}

			}
			catch (Exception ex)
			{
				return result = null;
			}
		}


		//--End--------- Signature Chart and Confidential List ------------

		//---------------Get All School Types------------
		public DataTable SchoolSet(int Type1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SchoolSetSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Type", Type1);
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

		public List<SelectListItem> GetSchoolSetByType(int type1)
		{
			DataTable dssetM = SchoolSet(type1);
			// English
			List<SelectListItem> itemSchoolSetM = new List<SelectListItem>();
			itemSchoolSetM.Add(new SelectListItem { Text = "--Select Set", Value = "" });
			foreach (System.Data.DataRow dr in dssetM.Rows)
			{
				itemSchoolSetM.Add(new SelectListItem { Text = @dr["Set1"].ToString(), Value = @dr["Set1"].ToString() });
			}
			return itemSchoolSetM;
		}
		//---------------Get All School Types------------
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
			catch (Exception ex)
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
			catch (Exception ex)
			{
				return null;
			}
		}

		//---------------Get All School Types------------
		public DataTable GetAllSchoolNameByABBR()
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
			catch (Exception ex)
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
		//------------------View School Master Details---------//
		//public DataSet Fill_School_Master()
		//{
		//    SqlConnection con = null;
		//    DataSet ds = null;
		//    try
		//    {
		//        con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
		//        SqlDataAdapter da = new SqlDataAdapter("select top 10 sm.id,sm.SCHL,sm.IDNO,sm.SCHLE,sm.STATIONE,sm.DISTE,sm.STATUS,su.vflag,su.Approved from SchoolMaster sm,tblSchUser su where su.SCHL=sm.SCHL", con);
		//        ds = new DataSet();
		//        da.Fill(ds);
		//        return ds;

		//    }
		//    catch
		//    {
		//        return ds = null;
		//    }
		//    finally
		//    {
		//        con.Close();
		//    }


		//}

		public DataSet SearchSchoolDetailsPaging(string search, int startIndex, int endIndex)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetAdminSchoolMasterNewPaging", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@startIndex", startIndex);
					cmd.Parameters.AddWithValue("@endIndex", endIndex);
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




		//---------------Searching Data------------
		public DataSet SearchSchoolDetails(string search)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetAdminSchoolMasterNew", con);
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
		//---------------View School Data-----------

		public SchoolModels GetSchoolDataBySchl(string SCHL, out DataSet result)
		{
			SchoolModels sm = new SchoolModels();
			DataSet ds = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SelectSchoolDatabyID", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@SCHL", SCHL);
					ad.SelectCommand = cmd;
					ad.Fill(ds);
					con.Open();
					if (ds.Tables.Count > 0)
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




		public DataSet SelectSchoolDatabyID(string SCHL)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SelectSchoolDatabyID", con);
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
				return null;
			}

		}


		public int InsertSMF(SchoolModels SM, int Type, out string NewSchlCode, string EmpUserId)  // Type 1=Regular, 2=Open
		{
			string hostName = Dns.GetHostName(); // Retrive the Name of HOST
			string userIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

			int result;
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					// SM.SCHL = SM.id.ToString();
					SqlCommand cmd = new SqlCommand("InsertSMFSP_SchlNew", con);//InsertSMFSP   //InsertSMFSPNew
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
					cmd.Parameters.AddWithValue("@STATUS", SM.status);
					cmd.Parameters.AddWithValue("@SESSION", SM.session);
					cmd.Parameters.AddWithValue("@SCHL", SM.SCHL);
					cmd.Parameters.AddWithValue("@Dist", SM.dist);
					cmd.Parameters.AddWithValue("@IDNO", SM.idno);
					cmd.Parameters.AddWithValue("@OCODE", SM.OCODE);
					cmd.Parameters.AddWithValue("@CLASS", SM.CLASS);
					cmd.Parameters.AddWithValue("@AREA", SM.AREA);
					cmd.Parameters.AddWithValue("@SCHLP", SM.SCHLP);
					cmd.Parameters.AddWithValue("@STATIONP", SM.STATIONP);
					cmd.Parameters.AddWithValue("@SCHLE", SM.SCHLE);
					cmd.Parameters.AddWithValue("@STATIONE", SM.STATIONE);
					cmd.Parameters.AddWithValue("@DISTE", SM.DISTE);
					cmd.Parameters.AddWithValue("@DISTP", SM.DISTP);
					cmd.Parameters.AddWithValue("@DISTNM", SM.DISTNM);
					cmd.Parameters.AddWithValue("@MATRIC", SM.MATRIC);
					cmd.Parameters.AddWithValue("@HUM", SM.HUM);
					cmd.Parameters.AddWithValue("@SCI", SM.SCI);
					cmd.Parameters.AddWithValue("@COMM", SM.COMM);
					cmd.Parameters.AddWithValue("@VOC", SM.VOC);
					cmd.Parameters.AddWithValue("@TECH", SM.TECH);
					cmd.Parameters.AddWithValue("@AGRI", SM.AGRI);
					cmd.Parameters.AddWithValue("@OMATRIC", SM.OMATRIC);
					cmd.Parameters.AddWithValue("@OHUM", SM.OHUM);
					cmd.Parameters.AddWithValue("@OSCI", SM.OSCI);
					cmd.Parameters.AddWithValue("@OCOMM", SM.OCOMM);
					cmd.Parameters.AddWithValue("@OVOC", SM.OVOC);
					cmd.Parameters.AddWithValue("@OTECH", SM.OTECH);
					cmd.Parameters.AddWithValue("@OAGRI", SM.OAGRI);
					cmd.Parameters.AddWithValue("@VALIDITY", SM.VALIDITY);
					cmd.Parameters.AddWithValue("@REMARKS", SM.REMARKS);
					cmd.Parameters.AddWithValue("@middle", SM.middle);
					cmd.Parameters.AddWithValue("@omiddle", SM.omiddle);
					cmd.Parameters.AddWithValue("@correctionno", SM.correctionno);
					cmd.Parameters.AddWithValue("@DISTNMPun", SM.DISTNMPun);
					cmd.Parameters.AddWithValue("@username", SM.username);
					// cmd.Parameters.AddWithValue("@userip", SM.userip);
					cmd.Parameters.AddWithValue("@userip", userIP);
					cmd.Parameters.AddWithValue("@ImpschlOcode", SM.ImpschlOcode);
					cmd.Parameters.AddWithValue("@SSET", SM.SSET);
					cmd.Parameters.AddWithValue("@MSET", SM.MSET);
					cmd.Parameters.AddWithValue("@SoSET", SM.SOSET);
					cmd.Parameters.AddWithValue("@MOSET", SM.MOSET);
					cmd.Parameters.AddWithValue("@MID_CR", SM.MID_CR);
					cmd.Parameters.AddWithValue("@MID_NO", SM.MID_NO);
					cmd.Parameters.AddWithValue("@MID_YR", SM.MID_YR);
					cmd.Parameters.AddWithValue("@MID_S", SM.MID_S);
					cmd.Parameters.AddWithValue("@MID_DNO", SM.MID_DNO);
					cmd.Parameters.AddWithValue("@HID_CR", SM.HID_CR);
					cmd.Parameters.AddWithValue("@HID_NO", SM.HID_NO);
					cmd.Parameters.AddWithValue("@HID_YR", SM.HID_YR);
					cmd.Parameters.AddWithValue("@HID_S", SM.HID_S);
					cmd.Parameters.AddWithValue("@HID_DNO", SM.HID_DNO);
					cmd.Parameters.AddWithValue("@SID_CR", SM.SID_CR);
					cmd.Parameters.AddWithValue("@SID_NO", SM.SID_NO);
					cmd.Parameters.AddWithValue("@SID_DNO", SM.SID_DNO);
					cmd.Parameters.AddWithValue("@H", SM.H);
					cmd.Parameters.AddWithValue("@HYR", SM.HYR);
					cmd.Parameters.AddWithValue("@H_S", SM.H_S);
					cmd.Parameters.AddWithValue("@C", SM.C);
					cmd.Parameters.AddWithValue("@CYR", SM.CYR);
					cmd.Parameters.AddWithValue("@C_S", SM.C_S);
					cmd.Parameters.AddWithValue("@S", SM.S);
					cmd.Parameters.AddWithValue("@SYR", SM.SYR);
					cmd.Parameters.AddWithValue("@S_S", SM.S_S);
					cmd.Parameters.AddWithValue("@A", SM.A);
					cmd.Parameters.AddWithValue("@AYR", SM.AYR);
					cmd.Parameters.AddWithValue("@A_S", SM.A_S);
					cmd.Parameters.AddWithValue("@V", SM.V);
					cmd.Parameters.AddWithValue("@VYR", SM.VYR);
					cmd.Parameters.AddWithValue("@V_S", SM.V_S);
					cmd.Parameters.AddWithValue("@T", SM.T);
					cmd.Parameters.AddWithValue("@TYR", SM.TYR);
					cmd.Parameters.AddWithValue("@T_S", SM.T_S);
					cmd.Parameters.AddWithValue("@MID_UTYPE", SM.MID_UTYPE);
					cmd.Parameters.AddWithValue("@HID_UTYPE", SM.HID_UTYPE);
					cmd.Parameters.AddWithValue("@H_UTYPE", SM.H_UTYPE);
					cmd.Parameters.AddWithValue("@S_UTYPE", SM.S_UTYPE);
					cmd.Parameters.AddWithValue("@C_UTYPE", SM.C_UTYPE);
					cmd.Parameters.AddWithValue("@V_UTYPE", SM.V_UTYPE);
					cmd.Parameters.AddWithValue("@A_UTYPE", SM.A_UTYPE);
					cmd.Parameters.AddWithValue("@T_UTYPE", SM.T_UTYPE);
					cmd.Parameters.AddWithValue("@Tcode", SM.Tcode);
					cmd.Parameters.AddWithValue("@Tehsile", SM.Tehsile);
					cmd.Parameters.AddWithValue("@Tehsilp", SM.Tehsilp);
					cmd.Parameters.AddWithValue("@PASSWORD", SM.PASSWORD);
					cmd.Parameters.AddWithValue("@PRINCIPAL", SM.PRINCIPAL);
					cmd.Parameters.AddWithValue("@STDCODE", SM.STDCODE);
					cmd.Parameters.AddWithValue("@PHONE", SM.PHONE);
					// cmd.Parameters.AddWithValue("@PHONE", SM.STDCODE);
					cmd.Parameters.AddWithValue("@MOBILE", SM.MOBILE);
					cmd.Parameters.AddWithValue("@EMAILID", SM.EMAILID);
					cmd.Parameters.AddWithValue("@CONTACTPER", SM.CONTACTPER);
					cmd.Parameters.AddWithValue("@CPSTD", SM.CPSTD);
					cmd.Parameters.AddWithValue("@CPPHONE", SM.CPPHONE);
					cmd.Parameters.AddWithValue("@OtContactno", SM.OtContactno);
					cmd.Parameters.AddWithValue("@USERTYPE", SM.USERTYPE);
					cmd.Parameters.AddWithValue("@ADDRESSE", SM.ADDRESSE);
					cmd.Parameters.AddWithValue("@ADDRESSP", SM.ADDRESSP);
					cmd.Parameters.AddWithValue("@vflag", SM.vflag == "Y" ? 1 : 0);
					//cmd.Parameters.AddWithValue("@cflag", SM.cflag);           
					cmd.Parameters.AddWithValue("@Vcode", SM.Vcode);
					cmd.Parameters.AddWithValue("@Approved", SM.Approved == "Y" ? 1 : 0);
					//cmd.Parameters.AddWithValue("@schlInfoUpdFlag", SM.schlInfoUpdFlag);
					cmd.Parameters.AddWithValue("@mobile2", SM.mobile2);
					//cmd.Parameters.AddWithValue("@PEND_RESULT", SM.PEND_RESULT);
					cmd.Parameters.AddWithValue("@NSQF_flag", SM.NSQF_flag == "Y" ? 1 : 0);
					cmd.Parameters.AddWithValue("@Type", Type);
					// New Edu add
					cmd.Parameters.AddWithValue("@SCHLESTD", SM.SchlEstd);
					cmd.Parameters.AddWithValue("@SCHLTYPE", SM.SchlType);
					cmd.Parameters.AddWithValue("@EDUBLOCK", SM.Edublock);
					cmd.Parameters.AddWithValue("@EDUCLUSTER", SM.EduCluster);
					cmd.Parameters.AddWithValue("@udisecode", SM.udisecode);
					//primary                   
					cmd.Parameters.AddWithValue("@fifth", SM.fifth);
					cmd.Parameters.AddWithValue("@FIF_YR", SM.FIF_YR);
					cmd.Parameters.AddWithValue("@FIF_S", SM.FIF_S);
					cmd.Parameters.AddWithValue("@FIF_UTYPE", SM.FIF_UTYPE);
					cmd.Parameters.AddWithValue("@FIF_NO", SM.FIF_NO);
					cmd.Parameters.AddWithValue("@MyIP", userIP);
					cmd.Parameters.AddWithValue("@lclass", SM.lclass);
					cmd.Parameters.AddWithValue("@uclass", SM.uclass);
					cmd.Parameters.AddWithValue("@APPNO", SM.APPNO);
					//
					cmd.Parameters.Add("@GetCorrectionNo", SqlDbType.Int).Direction = ParameterDirection.Output;
					cmd.Parameters.Add("@NewSchoolCode", SqlDbType.VarChar, 10).Direction = ParameterDirection.Output;
					cmd.Parameters.Add("@Error", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
					con.Open();
					result = cmd.ExecuteNonQuery();
					result = (int)cmd.Parameters["@GetCorrectionNo"].Value;
					NewSchlCode = (string)cmd.Parameters["@NewSchoolCode"].Value;
					string Error = (string)cmd.Parameters["@Error"].Value;
					return result;

				}
			}
			catch (Exception ex)
			{
				NewSchlCode = "";
				return result = -1;
			}
			finally
			{
				// con.Close();
			}
		}



		public int UpdateSMF(SchoolModels SM, int Type, string EmpUserId)  // Type 1=Regular, 2=Open
		{
			string hostName = Dns.GetHostName(); // Retrive the Name of HOST
			string userIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
			string OutError = "";
			int result;
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					//  SM.SCHL = SM.id.ToString();
					SqlCommand cmd = new SqlCommand("UpdateSMFSP_RN", con);//UpdateSMFSP   UpdateSMFSP_RN( for error check )
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
					cmd.Parameters.AddWithValue("@STATUS", SM.status);
					cmd.Parameters.AddWithValue("@SESSION", SM.session);
					cmd.Parameters.AddWithValue("@SCHL", SM.SCHL);
					cmd.Parameters.AddWithValue("@IDNO", SM.idno);
					cmd.Parameters.AddWithValue("@OCODE", SM.OCODE);
					cmd.Parameters.AddWithValue("@CLASS", SM.CLASS);
					cmd.Parameters.AddWithValue("@AREA", SM.AREA);
					cmd.Parameters.AddWithValue("@SCHLP", SM.SCHLP);
					cmd.Parameters.AddWithValue("@STATIONP", SM.STATIONP);
					cmd.Parameters.AddWithValue("@SCHLE", SM.SCHLE);
					cmd.Parameters.AddWithValue("@STATIONE", SM.STATIONE);
					cmd.Parameters.AddWithValue("@DISTE", SM.DISTE);
					cmd.Parameters.AddWithValue("@DISTP", SM.DISTP);
					cmd.Parameters.AddWithValue("@DISTNM", SM.DISTNM);
					cmd.Parameters.AddWithValue("@MATRIC", SM.MATRIC);
					cmd.Parameters.AddWithValue("@HUM", SM.HUM);
					cmd.Parameters.AddWithValue("@SCI", SM.SCI);
					cmd.Parameters.AddWithValue("@COMM", SM.COMM);
					cmd.Parameters.AddWithValue("@VOC", SM.VOC);
					cmd.Parameters.AddWithValue("@TECH", SM.TECH);
					cmd.Parameters.AddWithValue("@AGRI", SM.AGRI);
					cmd.Parameters.AddWithValue("@OMATRIC", SM.OMATRIC);
					cmd.Parameters.AddWithValue("@OHUM", SM.OHUM);
					cmd.Parameters.AddWithValue("@OSCI", SM.OSCI);
					cmd.Parameters.AddWithValue("@OCOMM", SM.OCOMM);
					cmd.Parameters.AddWithValue("@OVOC", SM.OVOC);
					cmd.Parameters.AddWithValue("@OTECH", SM.OTECH);
					cmd.Parameters.AddWithValue("@OAGRI", SM.OAGRI);
					cmd.Parameters.AddWithValue("@VALIDITY", SM.VALIDITY);
					cmd.Parameters.AddWithValue("@REMARKS", SM.REMARKS);
					cmd.Parameters.AddWithValue("@middle", SM.middle);
					cmd.Parameters.AddWithValue("@omiddle", SM.omiddle);
					cmd.Parameters.AddWithValue("@correctionno", SM.correctionno);
					cmd.Parameters.AddWithValue("@DISTNMPun", SM.DISTNMPun);
					cmd.Parameters.AddWithValue("@username", SM.username);
					// cmd.Parameters.AddWithValue("@userip", SM.userip);
					cmd.Parameters.AddWithValue("@userip", userIP);
					cmd.Parameters.AddWithValue("@ImpschlOcode", SM.ImpschlOcode);
					cmd.Parameters.AddWithValue("@SSET", SM.SSET);
					cmd.Parameters.AddWithValue("@MSET", SM.MSET);
					cmd.Parameters.AddWithValue("@SoSET", SM.SOSET);
					cmd.Parameters.AddWithValue("@MOSET", SM.MOSET);
					cmd.Parameters.AddWithValue("@MID_CR", SM.MID_CR);
					cmd.Parameters.AddWithValue("@MID_NO", SM.MID_NO);
					cmd.Parameters.AddWithValue("@MID_YR", SM.MID_YR);
					cmd.Parameters.AddWithValue("@MID_S", SM.MID_S);
					cmd.Parameters.AddWithValue("@MID_DNO", SM.MID_DNO);
					cmd.Parameters.AddWithValue("@HID_CR", SM.HID_CR);
					cmd.Parameters.AddWithValue("@HID_NO", SM.HID_NO);
					cmd.Parameters.AddWithValue("@HID_YR", SM.HID_YR);
					cmd.Parameters.AddWithValue("@HID_S", SM.HID_S);
					cmd.Parameters.AddWithValue("@HID_DNO", SM.HID_DNO);
					cmd.Parameters.AddWithValue("@SID_CR", SM.SID_CR);
					cmd.Parameters.AddWithValue("@SID_NO", SM.SID_NO);
					cmd.Parameters.AddWithValue("@SID_DNO", SM.SID_DNO);
					cmd.Parameters.AddWithValue("@H", SM.H);
					cmd.Parameters.AddWithValue("@HYR", SM.HYR);
					cmd.Parameters.AddWithValue("@H_S", SM.H_S);
					cmd.Parameters.AddWithValue("@C", SM.C);
					cmd.Parameters.AddWithValue("@CYR", SM.CYR);
					cmd.Parameters.AddWithValue("@C_S", SM.C_S);
					cmd.Parameters.AddWithValue("@S", SM.S);
					cmd.Parameters.AddWithValue("@SYR", SM.SYR);
					cmd.Parameters.AddWithValue("@S_S", SM.S_S);
					cmd.Parameters.AddWithValue("@A", SM.A);
					cmd.Parameters.AddWithValue("@AYR", SM.AYR);
					cmd.Parameters.AddWithValue("@A_S", SM.A_S);
					cmd.Parameters.AddWithValue("@V", SM.V);
					cmd.Parameters.AddWithValue("@VYR", SM.VYR);
					cmd.Parameters.AddWithValue("@V_S", SM.V_S);
					cmd.Parameters.AddWithValue("@T", SM.T);
					cmd.Parameters.AddWithValue("@TYR", SM.TYR);
					cmd.Parameters.AddWithValue("@T_S", SM.T_S);
					cmd.Parameters.AddWithValue("@MID_UTYPE", SM.MID_UTYPE);
					cmd.Parameters.AddWithValue("@HID_UTYPE", SM.HID_UTYPE);
					cmd.Parameters.AddWithValue("@H_UTYPE", SM.H_UTYPE);
					cmd.Parameters.AddWithValue("@S_UTYPE", SM.S_UTYPE);
					cmd.Parameters.AddWithValue("@C_UTYPE", SM.C_UTYPE);
					cmd.Parameters.AddWithValue("@V_UTYPE", SM.V_UTYPE);
					cmd.Parameters.AddWithValue("@A_UTYPE", SM.A_UTYPE);
					cmd.Parameters.AddWithValue("@T_UTYPE", SM.T_UTYPE);
					cmd.Parameters.AddWithValue("@Tcode", SM.Tcode);
					cmd.Parameters.AddWithValue("@Tehsile", SM.Tehsile);
					cmd.Parameters.AddWithValue("@Tehsilp", SM.Tehsilp);
					cmd.Parameters.AddWithValue("@PASSWORD", SM.PASSWORD);
					cmd.Parameters.AddWithValue("@PRINCIPAL", SM.PRINCIPAL);
					cmd.Parameters.AddWithValue("@STDCODE", SM.STDCODE);
					cmd.Parameters.AddWithValue("@PHONE", SM.PHONE);
					//cmd.Parameters.AddWithValue("@PHONE", SM.STDCODE);
					cmd.Parameters.AddWithValue("@MOBILE", SM.MOBILE);
					cmd.Parameters.AddWithValue("@EMAILID", SM.EMAILID);
					cmd.Parameters.AddWithValue("@CONTACTPER", SM.CONTACTPER);
					cmd.Parameters.AddWithValue("@CPSTD", SM.CPSTD);
					cmd.Parameters.AddWithValue("@CPPHONE", SM.CPPHONE);
					cmd.Parameters.AddWithValue("@OtContactno", SM.OtContactno);
					cmd.Parameters.AddWithValue("@USERTYPE", SM.USERTYPE);
					cmd.Parameters.AddWithValue("@ADDRESSE", SM.ADDRESSE);
					cmd.Parameters.AddWithValue("@ADDRESSP", SM.ADDRESSP);
					cmd.Parameters.AddWithValue("@vflag", SM.vflag == "Y" ? 1 : 0);
					//cmd.Parameters.AddWithValue("@cflag", SM.cflag);           
					cmd.Parameters.AddWithValue("@Vcode", SM.Vcode == "Y" ? 1 : 0);
					cmd.Parameters.AddWithValue("@Approved", SM.Approved == "Y" ? 1 : 0);
					//cmd.Parameters.AddWithValue("@schlInfoUpdFlag", SM.schlInfoUpdFlag);
					cmd.Parameters.AddWithValue("@mobile2", SM.mobile2);
					//cmd.Parameters.AddWithValue("@PEND_RESULT", SM.PEND_RESULT);
					cmd.Parameters.AddWithValue("@NSQF_flag", SM.NSQF_flag == "Y" ? 1 : 0);
					cmd.Parameters.AddWithValue("@Type", Type);
					cmd.Parameters.AddWithValue("@DIST ", SM.dist);

					// New Edu add
					cmd.Parameters.AddWithValue("@SCHLESTD", SM.SchlEstd);
					cmd.Parameters.AddWithValue("@SCHLTYPE", SM.SchlType);
					cmd.Parameters.AddWithValue("@EDUBLOCK", SM.Edublock);
					cmd.Parameters.AddWithValue("@EDUCLUSTER", SM.EduCluster);
					cmd.Parameters.AddWithValue("@udisecode", SM.udisecode);
					//primary

					cmd.Parameters.AddWithValue("@fifth", SM.fifth);
					cmd.Parameters.AddWithValue("@FIF_YR", SM.FIF_YR);
					cmd.Parameters.AddWithValue("@FIF_S", SM.FIF_S);
					cmd.Parameters.AddWithValue("@FIF_UTYPE", SM.FIF_UTYPE);
					cmd.Parameters.AddWithValue("@FIF_NO", SM.FIF_NO);
					cmd.Parameters.AddWithValue("@lclass", SM.lclass);
					cmd.Parameters.AddWithValue("@uclass", SM.uclass);

					string myIP = AbstractLayer.StaticDB.GetFullIPAddress();
					cmd.Parameters.AddWithValue("@MyIP", myIP);


					cmd.Parameters.Add("@GetCorrectionNo", SqlDbType.Int).Direction = ParameterDirection.Output;
					cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
					con.Open();
					result = cmd.ExecuteNonQuery();
					result = (int)cmd.Parameters["@GetCorrectionNo"].Value;
					OutError = (string)cmd.Parameters["@OutError"].Value;
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


		public int UpdateUSI(SchoolModels SM)
		{
			string hostName = Dns.GetHostName(); // Retrive the Name of HOST
			string userIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

			int result;
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("UpdateUSISP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@SCHL", SM.SCHL);
					// new for primary
					cmd.Parameters.AddWithValue("@SCHLE", SM.SCHLE);
					cmd.Parameters.AddWithValue("@STATIONE", SM.STATIONE);
					cmd.Parameters.AddWithValue("@SCHLP", SM.SCHLP);
					cmd.Parameters.AddWithValue("@STATIONP", SM.STATIONP);
					cmd.Parameters.AddWithValue("@DIST", SM.dist);
					//
					cmd.Parameters.AddWithValue("@AREA", SM.AREA);
					cmd.Parameters.AddWithValue("@udisecode", SM.udisecode);
					cmd.Parameters.AddWithValue("@idno", SM.idno);
					//

					cmd.Parameters.AddWithValue("@PRINCIPAL", SM.PRINCIPAL);
					cmd.Parameters.AddWithValue("@STDCODE", SM.STDCODE);
					cmd.Parameters.AddWithValue("@PHONE", SM.PHONE);
					cmd.Parameters.AddWithValue("@EMAILID", SM.EMAILID);
					cmd.Parameters.AddWithValue("@MOBILE", SM.MOBILE);
					cmd.Parameters.AddWithValue("@CONTACTPER", SM.CONTACTPER);
					cmd.Parameters.AddWithValue("@CPSTD", SM.CPSTD);
					cmd.Parameters.AddWithValue("@CPPHONE", SM.CPPHONE);
					cmd.Parameters.AddWithValue("@OtContactno", SM.OtContactno);
					cmd.Parameters.AddWithValue("@ADDRESSE", SM.ADDRESSE);
					cmd.Parameters.AddWithValue("@ADDRESSP", SM.ADDRESSP);
					cmd.Parameters.AddWithValue("@mobile2", SM.mobile2);
					cmd.Parameters.AddWithValue("@REMARKS", SM.REMARKS);

					cmd.Parameters.AddWithValue("@SCHLESTD", SM.SchlEstd);
					cmd.Parameters.AddWithValue("@SCHLTYPE", SM.SchlType);
					cmd.Parameters.AddWithValue("@Tehsile", SM.Tehsile);
					cmd.Parameters.AddWithValue("@EDUBLOCK", SM.Edublock);
					cmd.Parameters.AddWithValue("@EDUCLUSTER", SM.EduCluster);
					cmd.Parameters.AddWithValue("@DOB", SM.DOB);
					cmd.Parameters.AddWithValue("@DOJ", SM.DOJ);
					cmd.Parameters.AddWithValue("@ExperienceYr", SM.ExperienceYr);
					cmd.Parameters.AddWithValue("@PQualification", SM.PQualification);

					// School Bank Details (2019)
					cmd.Parameters.AddWithValue("@Bank", SM.Bank);
					cmd.Parameters.AddWithValue("@IFSC", SM.IFSC);
					cmd.Parameters.AddWithValue("@acno", SM.acno);

					cmd.Parameters.AddWithValue("@userip", userIP);

					cmd.Parameters.AddWithValue("@correctionno", SM.correctionno);
					cmd.Parameters.Add("@GetCorrectionNo", SqlDbType.Int).Direction = ParameterDirection.Output;
					con.Open();
					result = cmd.ExecuteNonQuery();
					result = (int)cmd.Parameters["@GetCorrectionNo"].Value;
					return result;

					//con.Open();
					//result = cmd.ExecuteNonQuery();
					//return result;

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

		public int SchoolChangePassword(string SCHL, string CurrentPassword, string NewPassword)
		{
			int result;
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SchoolChangePasswordSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@SCHL", SCHL);
					cmd.Parameters.AddWithValue("@CurrentPassword", CurrentPassword);
					cmd.Parameters.AddWithValue("@NewPassword", NewPassword);
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

		// Start Bulk Photo Upload
		public string Updated_Bulk_Pic_Data(string Myresult, string PhotoSignName, string Type, string SchlID, out int OutStatus)
		{
			SqlConnection con = null;
			string result = "";
			try
			{

				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("Uploaded_Bulk_Photo_Sign", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@StudentUniqueID", Myresult);
				cmd.Parameters.AddWithValue("@PhotoSignName", PhotoSignName);
				cmd.Parameters.AddWithValue("@Type", Type);
				cmd.Parameters.AddWithValue("@SchlID", SchlID);
				cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery().ToString();
				OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
				return result;
			}
			catch (Exception ex)
			{
				OutStatus = 0;
				//mbox(ex);
				return result = "";
			}
			finally
			{
				con.Close();
			}
		}

		//
		#region Begin CutList
		public DataSet UpdateCutListSubjects(string schid, string CLASS1, string Type1, out int OutStatus)
		{
			// SqlConnection con = null;
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{

					SqlCommand cmd = new SqlCommand("UpdateCutListSubjects", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@SCHL", schid);
					cmd.Parameters.AddWithValue("@CLASS", CLASS1);
					cmd.Parameters.AddWithValue("@Type", Type1);
					cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
					ad.SelectCommand = cmd;
					ad.Fill(result);
					OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
					con.Open();
					return result;
				}
			}
			catch (Exception ex)
			{
				OutStatus = -1;
				return result = null;
			}
		}
		#endregion End CutList  
		//



		// End Bulk Photo Upload
		#region ---------------TC Generate View School Data -----------
		public DataSet SearchSchoolDetailsTC(string search)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("spSearchSchoolDetailsTC", con);
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

		//public int GenerateTC(SchoolModelsTC sm)
		//{
		//    int result;
		//    SqlDataAdapter ad = new SqlDataAdapter();
		//    try
		//    {
		//        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
		//        {
		//            SqlCommand cmd = new SqlCommand("spGenerateTC", con);
		//            cmd.CommandType = CommandType.StoredProcedure;
		//            cmd.Parameters.AddWithValue("@stdID", sm.ID);
		//            cmd.Parameters.AddWithValue("@SCHL", sm.SCHL);
		//            cmd.Parameters.AddWithValue("@dispatchNo", sm.dispatchNo);
		//            cmd.Parameters.AddWithValue("@attendanceTot", sm.attendanceTot);
		//            cmd.Parameters.AddWithValue("@attendancePresnt", sm.attendancePresnt);
		//            cmd.Parameters.AddWithValue("@struckOff", sm.struckOff);
		//            cmd.Parameters.AddWithValue("@reasonFrSchoolLeav", sm.reasonFrSchoolLeav);
		//            cmd.Parameters.Add("@OutId", SqlDbType.Int).Direction = ParameterDirection.Output;
		//            //ad.SelectCommand = cmd;
		//            //ad.Fill(result);
		//            con.Open();
		//            result = cmd.ExecuteNonQuery();
		//            con.Close();
		//            return result;
		//        }
		//    }
		//    catch (Exception ex)
		//    {
		//        return result = 0;
		//    }
		//}
		#endregion ---------------TC Generate View School Data -----------

		public void findimageandsignature(string form_Name, int Std_id, string schl, out string photo, out string signature, out string District, out int OutStatus)
		{
			SqlConnection con = null;
			int result = 0;
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("findimageandsignature", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@form_Name", form_Name);
				cmd.Parameters.AddWithValue("@Std_id", Std_id);
				cmd.Parameters.AddWithValue("@schl", schl);
				cmd.Parameters.Add("@photo", SqlDbType.VarChar, 400).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@signature", SqlDbType.VarChar, 400).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@District", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@OutStatus", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery();
				OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
				District = Convert.ToString(cmd.Parameters["@District"].Value);
				signature = Convert.ToString(cmd.Parameters["@signature"].Value);
				photo = Convert.ToString(cmd.Parameters["@photo"].Value);
			}
			catch (Exception ex)
			{
				OutStatus = -1;
				District = "-1";
				signature = "-1";
				photo = "-1";

			}
			finally
			{
				con.Close();
			}
		}

		public string DummyUpdate_Uploaded_Photo_Sign(int Std_id, string PhotoSignName, string Type)
		{
			SqlConnection con = null;
			string result = "";
			try
			{

				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("DummyUpdate_Uploaded_Photo_Sign", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@Std_id", Std_id);
				cmd.Parameters.AddWithValue("@PhotoSignName", PhotoSignName);
				cmd.Parameters.AddWithValue("@Type", Type);

				con.Open();
				result = cmd.ExecuteNonQuery().ToString();
				return result;

			}
			catch (Exception ex)
			{
				//mbox(ex);
				return result = "";
			}
			finally
			{
				con.Close();
			}
		}

		public DataSet SelectPrintList(string search, string id, int pageIndex)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					//con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());//
					//SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
					SqlCommand cmd = new SqlCommand("DummySelectPrintList_sp", con);  //SelectPrintList_sp
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@flag", id);
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


		public DataSet SelectImportedPrintList_sp(string search, int pageIndex)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					//con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());//
					//SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
					SqlCommand cmd = new SqlCommand("SelectImportedPrintList_sp", con);
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


		public DataSet SelectPrintListSPOpen(string search, string id, int pageIndex)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SelectPrintListSPOpen", con);  //SelectPrintList_sp
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@flag", id);
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


		public DataSet SelectImportedPrintListOpen(string search, int pageIndex)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					//con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());//
					//SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
					SqlCommand cmd = new SqlCommand("SelectImportedPrintListOpen", con);
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
		public DataSet DownloadExamFinalReport(string ChallanId, string usertype, out int OutStatus)  // ReGenerateChallaan
		{
			SqlDataAdapter dataAdapter = new SqlDataAdapter();
			DataSet dataTable = new DataSet();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("DownloadExamFinalReport", con); //ReGenerateChallaanByIdSP
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@CHALLANID", ChallanId);
					cmd.Parameters.AddWithValue("@type", usertype);
					cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
					con.Open();
					dataAdapter.SelectCommand = cmd;
					dataAdapter.Fill(dataTable);
					OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
					return dataTable;

				}
			}
			catch (Exception ex)
			{
				OutStatus = -1;
				return null;
			}
		}


		public DataSet GetExamFormCalFee(string SCHL)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetExamFormCalFeeSP", con);
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

		public DataSet ExamReGenerateChallaanById(string ChallanId, string usertype, out int OutStatus, out string outCHALLANID)  // ReGenerateChallaan
		{
			SqlDataAdapter dataAdapter = new SqlDataAdapter();
			DataSet dataTable = new DataSet();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("ExamReGenerateChallaanByIdMainSP", con); //ExamReGenerateChallaanByIdSP17
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 300;
					cmd.Parameters.AddWithValue("@CHALLANID", ChallanId);
					cmd.Parameters.AddWithValue("@type", usertype);
					cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
					SqlParameter outPutParameter = new SqlParameter();
					outPutParameter.ParameterName = "@CHALLANIDNew";
					outPutParameter.Size = 100;
					outPutParameter.SqlDbType = System.Data.SqlDbType.VarChar;
					outPutParameter.Direction = System.Data.ParameterDirection.Output;
					cmd.Parameters.Add(outPutParameter);
					con.Open();
					dataAdapter.SelectCommand = cmd;
					dataAdapter.Fill(dataTable);
					OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
					outCHALLANID = (string)cmd.Parameters["@CHALLANIDNew"].Value;
					return dataTable;

				}
			}
			catch (Exception ex)
			{
				OutStatus = -1;
				outCHALLANID = "";
				return null;
			}
		}

		public DataSet ReGenerateFinalPrint(string ChallanId, string usertype, out int OutStatus)  // ReGenerateChallaan
		{
			SqlDataAdapter dataAdapter = new SqlDataAdapter();
			DataSet dataTable = new DataSet();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("ReGenerateFinalPrintSP", con); //ReGenerateChallaanByIdSP
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@CHALLANID", ChallanId);
					cmd.Parameters.AddWithValue("@type", usertype);
					cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
					con.Open();
					dataAdapter.SelectCommand = cmd;
					dataAdapter.Fill(dataTable);
					OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
					return dataTable;

				}
			}
			catch (Exception ex)
			{
				OutStatus = -1;
				return null;
			}
		}

		public DataSet ExamFormDataByChallan(int tbl, string SCHL, string ChallanId)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("ExamFormDataByChallanSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@tbl", tbl);
					cmd.Parameters.AddWithValue("@SCHL", SCHL);
					cmd.Parameters.AddWithValue("@ChallanId", ChallanId);
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



		public DataSet CompleteExamFormFeeByChallan(string SCHL, string ChallanId)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("CompleteExamFormFeeByChallanSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@SCHL", SCHL);
					cmd.Parameters.AddWithValue("@ChallanId", ChallanId);
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


		public SchoolModels getSchoolDetailsByUDISECode(string udisecode)
		{
			SchoolModels sm = new SchoolModels();
			DataSet ds = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("getSchoolDetailsByUDISECode", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@udisecode", udisecode);
					ad.SelectCommand = cmd;
					ad.Fill(ds);
					con.Open();
					if (ds.Tables.Count > 0)
					{
						if (ds.Tables[0].Rows.Count > 0)
						{
							sm.id = Convert.ToInt32(ds.Tables[0].Rows[0]["id"].ToString());
							sm.udisecode = ds.Tables[0].Rows[0]["udisecode"].ToString();
							sm.SCHL = ds.Tables[0].Rows[0]["schl"].ToString();
							sm.idno = ds.Tables[0].Rows[0]["idno"].ToString();
							sm.CLASS = ds.Tables[0].Rows[0]["CLASS"].ToString();
							sm.USERTYPE = ds.Tables[0].Rows[0]["USERTYPE"].ToString();
							sm.status = ds.Tables[0].Rows[0]["status"].ToString();
							sm.Approved = ds.Tables[0].Rows[0]["IsApproved"].ToString() == "Y" ? "YES" : "NO";
							sm.ADDRESSE = ds.Tables[0].Rows[0]["ADDRESSE"].ToString();
							sm.ADDRESSP = ds.Tables[0].Rows[0]["ADDRESSP"].ToString();
							sm.AGRI = ds.Tables[0].Rows[0]["AGRI"].ToString();
							sm.SCHLE = ds.Tables[0].Rows[0]["SCHLE"].ToString();
							sm.dist = ds.Tables[0].Rows[0]["DIST"].ToString();
							sm.DISTE = ds.Tables[0].Rows[0]["DISTE"].ToString();
							sm.STATIONE = ds.Tables[0].Rows[0]["STATIONE"].ToString();
							sm.DISTNM = ds.Tables[0].Rows[0]["DISTNM"].ToString();

							sm.SCHLP = ds.Tables[0].Rows[0]["SCHLP"].ToString();
							// sm.SCHLP = ds.Tables[0].Rows[0]["SCHLPfull"].ToString();
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

							sm.HID_YR = sm.MATRIC == "YES" ? ds.Tables[0].Rows[0]["HID_YR"].ToString() : "XXX";
							sm.MID_YR = sm.MATRIC == "YES" ? ds.Tables[0].Rows[0]["MID_YR"].ToString() : "XXX";
							sm.HYR = sm.HUM == "YES" ? ds.Tables[0].Rows[0]["HYR"].ToString() : "XXX";
							sm.SYR = sm.SCI == "YES" ? ds.Tables[0].Rows[0]["SYR"].ToString() : "XXX";
							sm.CYR = sm.COMM == "YES" ? ds.Tables[0].Rows[0]["CYR"].ToString() : "XXX";
							sm.VYR = sm.VOC == "YES" ? ds.Tables[0].Rows[0]["VYR"].ToString() : "XXX";


							sm.OHID_YR = sm.OMATRIC == "YES" ? ds.Tables[0].Rows[0]["HID_YR"].ToString() : "XXX";
							sm.OMID_YR = sm.OMATRIC == "YES" ? ds.Tables[0].Rows[0]["MID_YR"].ToString() : "XXX";
							sm.OHYR = sm.OHUM == "YES" ? ds.Tables[0].Rows[0]["HYR"].ToString() : "XXX";
							sm.OSYR = sm.OSCI == "YES" ? ds.Tables[0].Rows[0]["SYR"].ToString() : "XXX";
							sm.OCYR = sm.OCOMM == "YES" ? ds.Tables[0].Rows[0]["CYR"].ToString() : "XXX";
							sm.OVYR = sm.OVOC == "YES" ? ds.Tables[0].Rows[0]["VYR"].ToString() : "XXX";

							//---Secsion ---------------
							sm.HID_YR_SEC = sm.MATRIC == "YES" && sm.HID_UTYPE != "AFF" && sm.HID_UTYPE != "ASS" ? "N.A." : "XXX";
							sm.MID_YR_SEC = sm.MATRIC == "YES" && sm.HID_UTYPE != "AFF" && sm.HID_UTYPE != "ASS" ? "N.A." : "XXX";
							sm.HYR_SEC = sm.HUM == "YES" && sm.HID_UTYPE != "AFF" && sm.HID_UTYPE != "ASS" ? "N.A." : "XXX";
							sm.SYR_SEC = sm.SCI == "YES" && sm.HID_UTYPE != "AFF" && sm.HID_UTYPE != "ASS" ? "N.A." : "XXX";
							sm.CYR_SEC = sm.COMM == "YES" && sm.HID_UTYPE != "AFF" && sm.HID_UTYPE != "ASS" ? "N.A." : "XXX";
							sm.VYR_SEC = sm.VOC == "YES" && sm.HID_UTYPE != "AFF" && sm.HID_UTYPE != "ASS" ? "N.A." : "XXX";


							sm.OHID_YR_SEC = sm.OMATRIC == "YES" ? "N.A." : "XXX";
							sm.OMID_YR_SEC = sm.OMATRIC == "YES" ? "N.A." : "XXX";
							sm.OHYR_SEC = sm.OHUM == "YES" ? "N.A." : "XXX";
							sm.OSYR_SEC = sm.OSCI == "YES" ? "N.A." : "XXX";
							sm.OCYR_SEC = sm.OCOMM == "YES" ? "N.A." : "XXX";
							sm.OVYR_SEC = sm.OVOC == "YES" ? "N.A." : "XXX";


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
						}
					}
					return sm;
				}
			}
			catch (Exception ex)
			{
				return null;
			}
		}



		public void getUDISECode(string schl, out string udisecode, out int OutStatus)
		{
			SqlConnection con = null;
			int result = 0;
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("getUDISECode", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@schl", schl);
				cmd.Parameters.Add("@udisecode", SqlDbType.VarChar, 400).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery();
				OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
				udisecode = Convert.ToString(cmd.Parameters["@udisecode"].Value);
			}
			catch (Exception ex)
			{
				OutStatus = -1;
				udisecode = "-1";

			}
			finally
			{
				con.Close();
			}
		}

		public void insertUdisecode(string SCHL, string udisecode, out string outstatus)
		{

			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					// SM.SCHL = SM.id.ToString();
					SqlCommand cmd = new SqlCommand("insertUdisecode", con);//InsertSMFSP   //InsertSMFSPNew
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@SCHL", SCHL);
					cmd.Parameters.AddWithValue("@udisecode", udisecode);
					cmd.Parameters.Add("@outstatus", SqlDbType.NVarChar, 10).Direction = ParameterDirection.Output;
					con.Open();
					cmd.ExecuteNonQuery();
					outstatus = Convert.ToString(cmd.Parameters["@outstatus"].Value);

				}
			}
			catch (Exception ex)
			{
				outstatus = "-1";
			}
			finally
			{
				// con.Close();
			}
		}

		public void updateAashirwardNo(int id, int reg16id, string aashirwardno, out int OutStatus)
		{
			SqlConnection con = null;
			int result = 0;
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("updateAashirwardNo", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@id", id);
				cmd.Parameters.AddWithValue("@reg16id", reg16id);
				cmd.Parameters.AddWithValue("@aashirwardno", aashirwardno);
				cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery();
				OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
			}
			catch (Exception ex)
			{
				OutStatus = -1;

			}
			finally
			{
				con.Close();
			}
		}
		//----------------------Ninth Result page----------
		public DataSet Get_Ninth_Result_Page(string search, string schl, int pageNumber, string class1, int action1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("Get_Ninth_Result_Page_sp", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
					cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
					cmd.Parameters.AddWithValue("@PageSize", 20);
					cmd.Parameters.AddWithValue("@Action", action1);
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
		public DataSet UpdNinthResult(string ResultList, string totmarks, string obtmarks, string stdid, string schl, string EmpUserId, string UPTREMARKS)//90
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("UpdNinthResult_sp", con);//ReGenerateChallaanByIdSPAdminNew
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
					cmd.Parameters.AddWithValue("@stdid", stdid);
					cmd.Parameters.AddWithValue("@totmarks", totmarks);
					cmd.Parameters.AddWithValue("@obtmarks", obtmarks);
					cmd.Parameters.AddWithValue("@ResultList", ResultList);
					cmd.Parameters.AddWithValue("@SCHL", schl);
                    cmd.Parameters.AddWithValue("@UPTREMARKS", UPTREMARKS);
                    

                    ad.SelectCommand = cmd;
					ad.Fill(result);
					con.Open();
					//result = cmd.ExecuteNonQuery().ToString();

					if (con.State == ConnectionState.Open)
						con.Close();

					return result;
				}
			}
			catch (Exception ex)
			{
				return result = null;
			}
		}
		public DataSet Get_Ninth_Result_Page_Report(string search, string schl, string class1, string id)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("Get_Ninth_Result_Page_sp_Report", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
					cmd.Parameters.AddWithValue("@id", id);
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
		public DataSet FinalSubmitNinthResult(string cls, string schl)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("FinalSubmitNinthResult_sp", con);//ReGenerateChallaanByIdSPAdminNew
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cls", cls);
					cmd.Parameters.AddWithValue("@SCHL", schl);
					ad.SelectCommand = cmd;
					ad.Fill(result);
					con.Open();
					//result = cmd.ExecuteNonQuery().ToString();

					if (con.State == ConnectionState.Open)
						con.Close();

					return result;
				}
			}
			catch (Exception ex)
			{
				return result = null;
			}
		}
		public DataSet GetSchoolSection(string cls, string schl)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetSchoolSection_sp", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cls", cls);
					cmd.Parameters.AddWithValue("@SCHL", schl);
					ad.SelectCommand = cmd;
					ad.Fill(result);
					con.Open();
					if (con.State == ConnectionState.Open)
						con.Close();

					return result;
				}
			}
			catch (Exception ex)
			{
				return result = null;
			}
		}
		//-------------------End Ninth Page------------------//

		public void Admin9th11thFinalSubmit(string feemode, int AdminId, string cls, string schl, string receiptno, string receiptfee, string Remarks, out string OutStatus, DateTime? ReceiveDate = null)
		{
			int result;
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("Admin9th11thFinalSubmitSPNew", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@AdminId", AdminId);
					cmd.Parameters.AddWithValue("@cls", cls);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@receiptno", receiptno);
					cmd.Parameters.AddWithValue("@receiptfee", receiptfee);
					cmd.Parameters.AddWithValue("@ReceiveDate", ReceiveDate);
					cmd.Parameters.AddWithValue("@Remarks", Remarks);
					cmd.Parameters.AddWithValue("@feemode", feemode);
					cmd.Parameters.Add("@OutStatus", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
					con.Open();
					result = cmd.ExecuteNonQuery();
					OutStatus = Convert.ToString(cmd.Parameters["@OutStatus"].Value);
				}
			}
			catch (Exception ex)
			{
				OutStatus = "0";
			}
		}



		//----------------------Eleventh Result page----------
		public DataSet Get_Eleventh_Result_Page(string search, string schl, int pageNumber, string class1, int action1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("Get_Eleventh_Result_Page_sp", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
					cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
					cmd.Parameters.AddWithValue("@PageSize", 20);
					cmd.Parameters.AddWithValue("@Action", action1);
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
		public DataSet UpdateEleventhResult(string ResultList, string totmarks, string obtmarks, string stdid, string schl, string EmpUserId, string UPTREMARKS)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("UpdEleventhResult_sp", con);//ReGenerateChallaanByIdSPAdminNew
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
					cmd.Parameters.AddWithValue("@stdid", stdid);
					cmd.Parameters.AddWithValue("@totmarks", totmarks);
					cmd.Parameters.AddWithValue("@obtmarks", obtmarks);
					cmd.Parameters.AddWithValue("@ResultList", ResultList);
					cmd.Parameters.AddWithValue("@SCHL", schl);
                    cmd.Parameters.AddWithValue("@UPTREMARKS", UPTREMARKS);

                    ad.SelectCommand = cmd;
					ad.Fill(result);
					con.Open();
					//result = cmd.ExecuteNonQuery().ToString();

					if (con.State == ConnectionState.Open)
						con.Close();

					return result;
				}
			}
			catch (Exception ex)
			{
				return result = null;
			}
		}
		public DataSet Get_Eleventh_Result_Page_Report(string search, string schl, string class1, string id)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("Get_Eleventh_Result_Page_sp_Report", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
					cmd.Parameters.AddWithValue("@id", id);
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
		public DataSet FinalSubmitEleventhResult(string cls, string schl)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("FinalSubmitEleventhResult_sp", con);//ReGenerateChallaanByIdSPAdminNew
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cls", cls);
					cmd.Parameters.AddWithValue("@SCHL", schl);
					ad.SelectCommand = cmd;
					ad.Fill(result);
					con.Open();
					//result = cmd.ExecuteNonQuery().ToString();

					if (con.State == ConnectionState.Open)
						con.Close();

					return result;
				}
			}
			catch (Exception ex)
			{
				return result = null;
			}
		}
		public DataSet Eleventh_Result_Page_Report_Section(string search, string schl, string class1, string id)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("Get_Eleventh_Result_Page_Section_sp_Report", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
					cmd.Parameters.AddWithValue("@id", id);
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


		//-------------------End Eleventh Page------------------//
		//----------------------School Result Details page----------
		#region  Result Declare
		public DataSet schoolResultType(string schid)
		{
			// SqlConnection con = null;
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				//using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{

					SqlCommand cmd = new SqlCommand("GetSchoolResultType", con);
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
		public DataSet GetSchoolResultDetails(string Search, string schl, string class1, string rp)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				//using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetSchoolResultMarch", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Search", Search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
					cmd.Parameters.AddWithValue("@type", rp);
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
		#endregion  Result Declare
		//-------------------------End Result
		#region Private Signature Chart and Confidential List Both

		public DataSet SignatureChartPvtSet(SchoolModels sm, int cls)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SignatureChartPvtSetSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cent", sm.ExamCent);
					cmd.Parameters.AddWithValue("@class", cls);
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
		public DataSet SignatureChartPvtSub(SchoolModels sm, int cls)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SignatureChartPvtSubSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cent", sm.ExamCent);
					cmd.Parameters.AddWithValue("@class", cls);
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
		public DataSet SignatureChartPvt(SchoolModels sm, int cls)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SignatureChartPvtSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cent", sm.ExamCent);
					cmd.Parameters.AddWithValue("@class", cls);
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

		public DataSet GetSignatureChartPvt(SchoolModels sm, int cls)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetSignatureChartPvt_AG", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cent", sm.ExamCent);
					cmd.Parameters.AddWithValue("@set", sm.SelSet);
					cmd.Parameters.AddWithValue("@sub", sm.ExamSub);
					cmd.Parameters.AddWithValue("@roll", sm.ExamRoll);
					cmd.Parameters.AddWithValue("@class", cls);
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

		public DataSet ConfidentialListPvt(SchoolModels sm, int cls)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("ConfidentialListPvt", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Cent", sm.ExamCent);
					cmd.Parameters.AddWithValue("@class", cls);
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

		public DataSet AdmitCardPrivate(string Search, string schlid, int cls)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("AdmitCardPrivate", con);//GetFinalPrintMatricAdmitCardSearch_SP
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Search", Search);
					cmd.Parameters.AddWithValue("@SCHL", schlid);
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


		#endregion Private Signature Chart and Confidential List Both


		#region viewAllExamCandidate
		public DataSet SelectAView_All_Exam_Candidate(string search, int pageIndex)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					//con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());//
					//SqlCommand cmd = new SqlCommand("GetAdminSchoolMaster", con);
					SqlCommand cmd = new SqlCommand("ViewAllExamCandidate_Sp", con);
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
		#endregion viewAllExamCandidate

		#region CutList SchoolDB
		public DataSet GetCutList_Schl(string Search, string schl, string CLASS, string Type, string Status)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				//using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("CUTLISTSP_AG", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Search", Search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@CLASS", CLASS);
					cmd.Parameters.AddWithValue("@Type", Type);
					cmd.Parameters.AddWithValue("@Status", Status);
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

		public DataSet GetSCHLSET(string CLASS, string Adminid)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				//using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetSCHLSET_SP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					//cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@CLASS", CLASS);
					cmd.Parameters.AddWithValue("@Adminid", Adminid);
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
		public DataSet GetCutList_Schl_Admin(string Search, string schl, string CLASS, string Type, string Status)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				//using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("CUTLISTSPADMIN_AG", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Search", Search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@CLASS", CLASS);
					cmd.Parameters.AddWithValue("@Type", Type);
					cmd.Parameters.AddWithValue("@Status", Status);
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
		public DataSet CutList_Schl_AdminN(string Search, string schl, string CLASS, string Type, string Status)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				//using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("CutList_Schl_AdminN_SP", con);//CUTLISTSPADMIN_AG
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Search", Search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@CLASS", CLASS);
					cmd.Parameters.AddWithValue("@Type", Type);
					cmd.Parameters.AddWithValue("@Status", Status);
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
		public DataSet GetCentreSchl(string schl, string class1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				//using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetCentreSchlSpAdmin_AG", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class1", class1);
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
		#endregion CutList School

		//-----------------------------Ranjan--------------
		public DataSet SelectAllTehsil(string DISTID)
		{

			// SqlConnection con = null;
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();

			try
			{

				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					//con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
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


		public DataSet SelectBlock(string DIST)
		{
			// SqlConnection con = null;
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetALLBLOCK_Sp", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@DIST", DIST);
					ad.SelectCommand = cmd;
					ad.Fill(result);
					//con.Open();
					return result;
				}

			}
			catch (Exception ex)
			{
				return result = null;
			}
		}


		#region pvt cutlist
		public DataSet Pvtcutlist(string Search, string schl, string CLASS, string Type, string Status, string Myset, string SelDistSearch)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				//using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("CUTLISTSP_PVT_AG", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Search", Search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@CLASS", CLASS);
					cmd.Parameters.AddWithValue("@Type", Type);
					cmd.Parameters.AddWithValue("@Status", Status);
					cmd.Parameters.AddWithValue("@Myset", Myset);
					//cmd.Parameters.AddWithValue("@SelDistSearch", SelDistSearch);
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
		#endregion pvt cutlist

		public DataSet SupervisoryStaffList(string Dist)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();

			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SupervisoryStaffList_Sp", con); //GetAll10thPass
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@dist", Dist);
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

		public DataSet GetDeoDISTSCHL(string DeologinSchl)
		{
			// SqlConnection con = null;
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetDeoDISTSCHL_Sp", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@SCHL", DeologinSchl);
					ad.SelectCommand = cmd;
					ad.Fill(result);
					//con.Open();
					return result;
				}

			}
			catch (Exception ex)
			{
				return result = null;
			}
		}

		#region Prac Exam Enter

		public DataSet PracExamEnterMarks(string class1, string rp, string cent, string Search, int SelectedAction, int pageNumber)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("PracExamEnterMarksSP", con);//[PracExamEnterMarksSP]
					//SqlCommand cmd = new SqlCommand("PracExamEnterMarksSP_ExceptVOC", con);//[PracExamEnterMarksSP_ExceptVOC]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 300;
					cmd.Parameters.AddWithValue("@class", class1);
					cmd.Parameters.AddWithValue("@rp", rp);
					cmd.Parameters.AddWithValue("@cent", cent);
					cmd.Parameters.AddWithValue("@Search", Search);
					cmd.Parameters.AddWithValue("@SelectedAction", SelectedAction);
					cmd.Parameters.AddWithValue("@pageNumber", pageNumber);
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

		public DataSet PracExamViewList(string class1, string rp, string cent, string Search, int SelectedAction, int pageNumber, string sub)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					// SqlCommand cmd = new SqlCommand("PracExamViewListSP", con);
					SqlCommand cmd = new SqlCommand("PracExamViewListSP_ExceptVOC", con);//PracExamViewListSP_ExceptVOC
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 300;
					cmd.Parameters.AddWithValue("@class", class1);
					cmd.Parameters.AddWithValue("@rp", rp);
					cmd.Parameters.AddWithValue("@cent", cent);
					cmd.Parameters.AddWithValue("@Search", Search);
					cmd.Parameters.AddWithValue("@sub", sub);
					cmd.Parameters.AddWithValue("@SelectedAction", SelectedAction);
					cmd.Parameters.AddWithValue("@pageNumber", pageNumber);
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


		public DataSet ViewPracExamEnterMarks(string class1, string rp, string cent, string Search, int SelectedAction, int pageNumber, string sub)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					// SqlCommand cmd = new SqlCommand("ViewPracExamEnterMarksSP", con);//
					SqlCommand cmd = new SqlCommand("ViewPracExamEnterMarksSP_ExceptVOC", con);//ViewPracExamEnterMarksSP_ExceptVOC
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 300;
					cmd.Parameters.AddWithValue("@class", class1);
					cmd.Parameters.AddWithValue("@rp", rp);
					cmd.Parameters.AddWithValue("@cent", cent);
					cmd.Parameters.AddWithValue("@Search", Search);
					cmd.Parameters.AddWithValue("@sub", sub);
					cmd.Parameters.AddWithValue("@SelectedAction", SelectedAction);
					cmd.Parameters.AddWithValue("@pageNumber", pageNumber);
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


		public DataSet ViewPracExamFinalSubmit(string class1, string rp, string cent, string Search, int SelectedAction, int pageNumber, string sub)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					//SqlCommand cmd = new SqlCommand("ViewPracExamFinalSubmitSPNew", con);//[ViewPracExamFinalSubmitSPNew_ExceptVOC]
					SqlCommand cmd = new SqlCommand("ViewPracExamFinalSubmitSPNew_ExceptVOC", con);//ViewPracExamFinalSubmitSPNew_ExceptVOC
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 300;
					cmd.Parameters.AddWithValue("@class", class1);
					cmd.Parameters.AddWithValue("@rp", rp);
					cmd.Parameters.AddWithValue("@cent", cent);
					cmd.Parameters.AddWithValue("@Search", Search);
					cmd.Parameters.AddWithValue("@sub", sub);
					cmd.Parameters.AddWithValue("@SelectedAction", SelectedAction);
					cmd.Parameters.AddWithValue("@pageNumber", pageNumber);
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


		public string AllotPracMarks(string RP, DataTable dtSub, string class1, out int OutStatus, out string OutError)  // BankLoginSP
		{
			SqlConnection con = null;
			string result = "";
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("AllotPracMarks", con);  //AllotPracMarks
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@RP", RP);
				cmd.Parameters.AddWithValue("@dtSub", dtSub);
				cmd.Parameters.AddWithValue("@class", class1);
				cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery().ToString();
				OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
				OutError = (string)cmd.Parameters["@OutError"].Value;
				return result;
			}
			catch (Exception ex)
			{
				OutStatus = -1;
				OutError = ex.Message;
				//mbox(ex);
				return result = "";
			}
			finally
			{
				con.Close();
			}
		}


		public string RemovePracMarks(string RP, DataTable dtSub, string class1, out int OutStatus, out string OutError)  // BankLoginSP
		{
			SqlConnection con = null;
			string result = "";
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("RemovePracMarks", con);  //AllotPracMarks
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@RP", RP);
				cmd.Parameters.AddWithValue("@dtSub", dtSub);
				cmd.Parameters.AddWithValue("@class", class1);
				cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery().ToString();
				OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
				OutError = (string)cmd.Parameters["@OutError"].Value;
				return result;
			}
			catch (Exception ex)
			{
				OutStatus = -1;
				OutError = ex.Message;
				//mbox(ex);
				return result = "";
			}
			finally
			{
				con.Close();
			}
		}


		public string PracExamFinalSubmit(string ExamCent, string class1, string RP, string cent, string sub, string schl, DataTable dtSub, out int OutStatus, out string OutError)  // BankLoginSP
		{
			SqlConnection con = null;
			string result = "";
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				//SqlCommand cmd = new SqlCommand("PracExamFinalSubmitSP", con);  //AllotPracMarks
				SqlCommand cmd = new SqlCommand("PracExamFinalSubmitSPRN", con);  //AllotPracMarks                
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = 300;
				cmd.Parameters.AddWithValue("@ExamCent", ExamCent);
				cmd.Parameters.AddWithValue("@class", class1);
				cmd.Parameters.AddWithValue("@RP", RP);
				cmd.Parameters.AddWithValue("@cent", cent);
				cmd.Parameters.AddWithValue("@sub", sub);
				cmd.Parameters.AddWithValue("@schl", schl);
				cmd.Parameters.AddWithValue("@dtSub", dtSub);
				cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery().ToString();
				OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
				OutError = (string)cmd.Parameters["@OutError"].Value;
				return result;
			}
			catch (Exception ex)
			{
				OutStatus = -1;
				OutError = ex.Message;
				//mbox(ex);
				return result = "";
			}
			finally
			{
				con.Close();
			}
		}

		#endregion Prac Exam Enter


		#region  Practical  SignatureChart and Confidential List


		public DataSet GetPracCentcodeByClass(string schl, int cls)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetPracCentcodeByClass", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@SCHL", schl);
					cmd.Parameters.AddWithValue("@class", cls);
					ad.SelectCommand = cmd;
					ad.Fill(result);
					//con.Open();
					return result;
				}

			}
			catch (Exception ex)
			{
				return result = null;
			}
		}



		public DataSet GetSubFromSubMasters(int cls, string type, string schl, string cent)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetSubFromSubMasters", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@cent", cent);
					cmd.Parameters.AddWithValue("@class", cls);
					cmd.Parameters.AddWithValue("@type", type);
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




		public DataSet PracSignatureChart(SchoolModels sm)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					string roll = "";
					if (sm.ExamRoll != "")
					{
						roll = "and roll='" + sm.ExamRoll + "'";
					}
					SqlCommand cmd = new SqlCommand("GetPracticalSignatureChart_SP2702", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cent", sm.ExamCent);
					cmd.Parameters.AddWithValue("@sub", sm.ExamSub);
					cmd.Parameters.AddWithValue("@roll", roll);
					cmd.Parameters.AddWithValue("@class", sm.CLASS);
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


		public DataSet PracConfidentialList(SchoolModels sm)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("[GetPracticalConfi_SP2702]", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Cent", sm.ExamCent);
					cmd.Parameters.AddWithValue("@class", sm.CLASS);
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


		#endregion

		#region SchoolPremisesInformation

		public SchoolPremisesInformation SchoolPremisesInformationBySchl(string SCHL, out DataSet ds1)
		{
			SchoolPremisesInformation sm = new SchoolPremisesInformation();
			DataSet ds = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SchoolPremisesInformationBySchl", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@SCHL", SCHL);
					ad.SelectCommand = cmd;
					ad.Fill(ds);
					con.Open();

					if (ds == null || ds.Tables[0].Rows.Count == 0)
					{
						ds1 = null;
						return null;
					}
					else
					{
						ds1 = ds;
						sm.ID = Convert.ToInt32(ds.Tables[0].Rows[0]["ID"].ToString() == "" ? "0" : ds.Tables[0].Rows[0]["ID"].ToString());
						if (sm.ID > 0)
						{
							sm.SSD1 = ds.Tables[0].Rows[0]["SSD1"].ToString();
							sm.SSD2 = ds.Tables[0].Rows[0]["SSD2"].ToString();
							sm.SSD3 = Convert.ToInt32(ds.Tables[0].Rows[0]["SSD3"].ToString());
							sm.SSD4 = Convert.ToInt32(ds.Tables[0].Rows[0]["SSD4"].ToString());
							sm.CB5 = float.Parse(ds.Tables[0].Rows[0]["CB5"].ToString());
							sm.CB6 = float.Parse(ds.Tables[0].Rows[0]["CB6"].ToString());
							sm.CB7 = Convert.ToInt32(ds.Tables[0].Rows[0]["CB7"].ToString());
							sm.CB8 = Convert.ToInt32(ds.Tables[0].Rows[0]["CB8"].ToString());
							sm.CB9 = Convert.ToInt32(ds.Tables[0].Rows[0]["CB9"].ToString());
							sm.CB10 = Convert.ToInt32(ds.Tables[0].Rows[0]["CB10"].ToString());
							sm.CB11 = Convert.ToInt32(ds.Tables[0].Rows[0]["CB11"].ToString());
							sm.CB12 = Convert.ToInt32(ds.Tables[0].Rows[0]["CB12"].ToString());
							sm.CB13 = Convert.ToInt32(ds.Tables[0].Rows[0]["CB13"].ToString());
							sm.CB14 = Convert.ToInt32(ds.Tables[0].Rows[0]["CB14"].ToString());
							sm.CB15 = Convert.ToInt32(ds.Tables[0].Rows[0]["CB15"].ToString());
							sm.CB16 = Convert.ToString(ds.Tables[0].Rows[0]["CB16"].ToString());
							sm.ECD17 = ds.Tables[0].Rows[0]["ECD17"].ToString();
							sm.ECD18 = Convert.ToInt32(ds.Tables[0].Rows[0]["ECD18"].ToString());
							sm.ECD19 = Convert.ToInt32(ds.Tables[0].Rows[0]["ECD19"].ToString());
							sm.ECD20 = Convert.ToInt32(ds.Tables[0].Rows[0]["ECD20"].ToString());
							sm.ECD21 = Convert.ToInt32(ds.Tables[0].Rows[0]["ECD21"].ToString());
							sm.ECD22 = Convert.ToInt32(ds.Tables[0].Rows[0]["ECD22"].ToString());
							sm.ECD23 = Convert.ToInt32(ds.Tables[0].Rows[0]["ECD23"].ToString());
							sm.ECD24 = Convert.ToInt32(ds.Tables[0].Rows[0]["ECD24"].ToString());
							sm.ECD25 = ds.Tables[0].Rows[0]["ECD25"].ToString();
							sm.CWS26 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS26"].ToString());
							sm.CWS27 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS27"].ToString());
							sm.CWS28 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS28"].ToString());
							sm.CWS29 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS29"].ToString());
							sm.CWS30 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS30"].ToString());
							sm.CWS31 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS31"].ToString());
							sm.CWS32 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS32"].ToString());
							sm.CWS33 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS33"].ToString());
							sm.CWS34 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS34"].ToString());
							sm.CWS35 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS35"].ToString());
							sm.CWS36 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS36"].ToString());
							sm.CWS37 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS37"].ToString());
							sm.CWS38 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS38"].ToString());
							sm.CWS39 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS39"].ToString());
							sm.CWS40 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS40"].ToString());
							sm.CWS41 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS41"].ToString());
							sm.CWS42 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS42"].ToString());
							sm.CWS43 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS43"].ToString());
							sm.CWS44 = Convert.ToInt32(ds.Tables[0].Rows[0]["CWS44"].ToString());
							sm.CSS45 = Convert.ToInt32(ds.Tables[0].Rows[0]["CSS45"].ToString());
							sm.CSS46 = Convert.ToInt32(ds.Tables[0].Rows[0]["CSS46"].ToString());
							sm.CSS47 = Convert.ToInt32(ds.Tables[0].Rows[0]["CSS47"].ToString());
							sm.CSS48 = Convert.ToInt32(ds.Tables[0].Rows[0]["CSS48"].ToString());
							sm.PG49 = float.Parse(ds.Tables[0].Rows[0]["PG49"].ToString());
							sm.PG50 = Convert.ToInt32(ds.Tables[0].Rows[0]["PG50"].ToString());
							sm.PG51 = ds.Tables[0].Rows[0]["PG51"].ToString();
							sm.PG52 = Convert.ToInt32(ds.Tables[0].Rows[0]["PG52"].ToString());
							sm.PG53 = ds.Tables[0].Rows[0]["PG53"].ToString();
							sm.PG54 = Convert.ToInt32(ds.Tables[0].Rows[0]["PG54"].ToString());
							sm.LIB55 = float.Parse(ds.Tables[0].Rows[0]["LIB55"].ToString());
							sm.LIB56 = ds.Tables[0].Rows[0]["LIB56"].ToString();
							sm.LIB57 = Convert.ToInt32(ds.Tables[0].Rows[0]["LIB57"].ToString());
							sm.LIB58 = Convert.ToInt32(ds.Tables[0].Rows[0]["LIB58"].ToString());
							sm.LIB59 = Convert.ToInt32(ds.Tables[0].Rows[0]["LIB59"].ToString());
							sm.LAB60 = float.Parse(ds.Tables[0].Rows[0]["LAB60"].ToString());
							sm.LAB61 = Convert.ToInt32(ds.Tables[0].Rows[0]["LAB61"].ToString());
							sm.LAB62 = float.Parse(ds.Tables[0].Rows[0]["LAB62"].ToString());
							sm.LAB63 = Convert.ToInt32(ds.Tables[0].Rows[0]["LAB63"].ToString());
							sm.LAB64 = float.Parse(ds.Tables[0].Rows[0]["LAB64"].ToString());
							sm.LAB65 = Convert.ToInt32(ds.Tables[0].Rows[0]["LAB65"].ToString());
							sm.LAB66 = float.Parse(ds.Tables[0].Rows[0]["LAB66"].ToString());
							sm.LAB67 = Convert.ToInt32(ds.Tables[0].Rows[0]["LAB67"].ToString());
							sm.CLAB68 = float.Parse(ds.Tables[0].Rows[0]["CLAB68"].ToString());
							sm.CLAB69 = Convert.ToInt32(ds.Tables[0].Rows[0]["CLAB69"].ToString());
							sm.CLAB70 = Convert.ToInt32(ds.Tables[0].Rows[0]["CLAB70"].ToString());
							sm.CLAB71 = Convert.ToString(ds.Tables[0].Rows[0]["CLAB71"].ToString());
							sm.OTH72 = ds.Tables[0].Rows[0]["OTH72"].ToString();
							sm.OTH73 = ds.Tables[0].Rows[0]["OTH73"].ToString();
							sm.OTH74 = ds.Tables[0].Rows[0]["OTH74"].ToString();
							sm.OTH75 = ds.Tables[0].Rows[0]["OTH75"].ToString();
							sm.OTH76 = ds.Tables[0].Rows[0]["OTH76"].ToString();
							sm.OTH77 = ds.Tables[0].Rows[0]["OTH77"].ToString();
							sm.OTH78 = ds.Tables[0].Rows[0]["OTH78"].ToString();
							sm.OTH79 = ds.Tables[0].Rows[0]["OTH79"].ToString();
							sm.OTH80 = ds.Tables[0].Rows[0]["OTH80"].ToString();
							sm.OTH81 = ds.Tables[0].Rows[0]["OTH81"].ToString();
							sm.OTH82 = ds.Tables[0].Rows[0]["OTH82"].ToString();
							sm.OTH83 = ds.Tables[0].Rows[0]["OTH83"].ToString();
							sm.ISACTIVE = Convert.ToBoolean(ds.Tables[0].Rows[0]["ISACTIVE"].ToString());
							sm.CREATEDDATE = Convert.ToDateTime(string.IsNullOrEmpty(ds.Tables[0].Rows[0]["CREATEDDATE"].ToString()) ? "1990-01-01 00:00:00.000" : ds.Tables[0].Rows[0]["CREATEDDATE"].ToString());
							sm.UDISECODE = Convert.ToString(ds.Tables[0].Rows[0]["UDISECODE"].ToString());
							sm.ChallanId = Convert.ToString(ds.Tables[0].Rows[0]["ChallanId"].ToString());
							sm.ChallanDt = Convert.ToString(ds.Tables[0].Rows[0]["ChallanDt"].ToString());
							sm.challanVerify = Convert.ToInt32(ds.Tables[0].Rows[0]["challanVerify"].ToString());


							sm.IsFinalSubmit = Convert.ToInt32(string.IsNullOrEmpty(ds.Tables[0].Rows[0]["IsFinalSubmit"].ToString()) ? "0" : ds.Tables[0].Rows[0]["IsFinalSubmit"].ToString());
							if (string.IsNullOrEmpty(ds.Tables[0].Rows[0]["FinalSubmitOn"].ToString()))
							{
								sm.FinalSubmitOn = Convert.ToDateTime("1990-01-01 00:00:00.000");
							}
							else
							{
								sm.FinalSubmitOn = Convert.ToDateTime(ds.Tables[0].Rows[0]["FinalSubmitOn"].ToString());
								//DateTime FinalSubmitDate;
								//if (DateTime.TryParseExact(ds.Tables[0].Rows[0]["FinalSubmitOn"].ToString().Split(' ')[0], "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out FinalSubmitDate))
								//{
								//    sm.FinalSubmitOn = FinalSubmitDate;
								//}
							}
						}
						return sm;
					}
				}
			}
			catch (Exception ex)
			{
				ds1 = null;
				return null;
			}

		}


		public int SchoolPremisesInformation(SchoolPremisesInformation sm, out string OutError)
		{
			string hostName = Dns.GetHostName(); // Retrive the Name of HOST
			string userIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
			int result;
			OutError = "0";
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SchoolPremisesInformationSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@ID", sm.ID);// if ID=0 then Insert else Update
					cmd.Parameters.AddWithValue("@SCHL", sm.SCHL);
					cmd.Parameters.AddWithValue("@SSD1", sm.SSD1);
					cmd.Parameters.AddWithValue("@SSD2", sm.SSD2);
					cmd.Parameters.AddWithValue("@SSD3", sm.SSD3);
					cmd.Parameters.AddWithValue("@SSD4", sm.SSD4);
					cmd.Parameters.AddWithValue("@CB5", sm.CB5);
					cmd.Parameters.AddWithValue("@CB6", sm.CB6);
					cmd.Parameters.AddWithValue("@CB7", sm.CB7);
					cmd.Parameters.AddWithValue("@CB8", sm.CB8);
					cmd.Parameters.AddWithValue("@CB9", sm.CB9);
					cmd.Parameters.AddWithValue("@CB10", sm.CB10);
					cmd.Parameters.AddWithValue("@CB11", sm.CB11);
					cmd.Parameters.AddWithValue("@CB12", sm.CB12);
					cmd.Parameters.AddWithValue("@CB13", sm.CB13);
					cmd.Parameters.AddWithValue("@CB14", sm.CB14);
					cmd.Parameters.AddWithValue("@CB15", sm.CB15);
					cmd.Parameters.AddWithValue("@CB16", sm.CB16);
					cmd.Parameters.AddWithValue("@ECD17", sm.ECD17);
					cmd.Parameters.AddWithValue("@ECD18", sm.ECD18);
					cmd.Parameters.AddWithValue("@ECD19", sm.ECD19);
					cmd.Parameters.AddWithValue("@ECD20", sm.ECD20);
					cmd.Parameters.AddWithValue("@ECD21", sm.ECD21);
					cmd.Parameters.AddWithValue("@ECD22", sm.ECD22);
					cmd.Parameters.AddWithValue("@ECD23", sm.ECD23);
					cmd.Parameters.AddWithValue("@ECD24", sm.ECD24);
					cmd.Parameters.AddWithValue("@ECD25", sm.ECD25);
					cmd.Parameters.AddWithValue("@CWS26", sm.CWS26);
					cmd.Parameters.AddWithValue("@CWS27", sm.CWS27);
					cmd.Parameters.AddWithValue("@CWS28", sm.CWS28);
					cmd.Parameters.AddWithValue("@CWS29", sm.CWS29);
					cmd.Parameters.AddWithValue("@CWS30", sm.CWS30);
					cmd.Parameters.AddWithValue("@CWS31", sm.CWS31);
					cmd.Parameters.AddWithValue("@CWS32", sm.CWS32);
					cmd.Parameters.AddWithValue("@CWS33", sm.CWS33);
					cmd.Parameters.AddWithValue("@CWS34", sm.CWS34);
					cmd.Parameters.AddWithValue("@CWS35", sm.CWS35);
					cmd.Parameters.AddWithValue("@CWS36", sm.CWS36);
					cmd.Parameters.AddWithValue("@CWS37", sm.CWS37);
					cmd.Parameters.AddWithValue("@CWS38", sm.CWS38);
					cmd.Parameters.AddWithValue("@CWS39", sm.CWS39);
					cmd.Parameters.AddWithValue("@CWS40", sm.CWS40);
					cmd.Parameters.AddWithValue("@CWS41", sm.CWS41);
					cmd.Parameters.AddWithValue("@CWS42", sm.CWS42);
					cmd.Parameters.AddWithValue("@CWS43", sm.CWS43);
					cmd.Parameters.AddWithValue("@CWS44", sm.CWS44);
					cmd.Parameters.AddWithValue("@CSS45", sm.CSS45);
					cmd.Parameters.AddWithValue("@CSS46", sm.CSS46);
					cmd.Parameters.AddWithValue("@CSS47", sm.CSS47);
					cmd.Parameters.AddWithValue("@CSS48", sm.CSS48);
					cmd.Parameters.AddWithValue("@PG49", sm.PG49);
					cmd.Parameters.AddWithValue("@PG50", sm.PG50);
					cmd.Parameters.AddWithValue("@PG51", sm.PG51);
					cmd.Parameters.AddWithValue("@PG52", sm.PG52);
					cmd.Parameters.AddWithValue("@PG53", sm.PG53);
					cmd.Parameters.AddWithValue("@PG54", sm.PG54);
					cmd.Parameters.AddWithValue("@LIB55", sm.LIB55);
					cmd.Parameters.AddWithValue("@LIB56", sm.LIB56);
					cmd.Parameters.AddWithValue("@LIB57", sm.LIB57);
					cmd.Parameters.AddWithValue("@LIB58", sm.LIB58);
					cmd.Parameters.AddWithValue("@LIB59", sm.LIB59);
					cmd.Parameters.AddWithValue("@LAB60", sm.LAB60);
					cmd.Parameters.AddWithValue("@LAB61", sm.LAB61);
					cmd.Parameters.AddWithValue("@LAB62", sm.LAB62);
					cmd.Parameters.AddWithValue("@LAB63", sm.LAB63);
					cmd.Parameters.AddWithValue("@LAB64", sm.LAB64);
					cmd.Parameters.AddWithValue("@LAB65", sm.LAB65);
					cmd.Parameters.AddWithValue("@LAB66", sm.LAB66);
					cmd.Parameters.AddWithValue("@LAB67", sm.LAB67);
					cmd.Parameters.AddWithValue("@CLAB68", sm.CLAB68);
					cmd.Parameters.AddWithValue("@CLAB69", sm.CLAB69);
					cmd.Parameters.AddWithValue("@CLAB70", sm.CLAB70);
					cmd.Parameters.AddWithValue("@CLAB71", sm.CLAB71);
					cmd.Parameters.AddWithValue("@OTH72", sm.OTH72);
					cmd.Parameters.AddWithValue("@OTH73", sm.OTH73);
					cmd.Parameters.AddWithValue("@OTH74", sm.OTH74);
					cmd.Parameters.AddWithValue("@OTH75", sm.OTH75);
					cmd.Parameters.AddWithValue("@OTH76", sm.OTH76);
					cmd.Parameters.AddWithValue("@OTH77", sm.OTH77);
					cmd.Parameters.AddWithValue("@OTH78", sm.OTH78);
					cmd.Parameters.AddWithValue("@OTH79", sm.OTH79);
					cmd.Parameters.AddWithValue("@OTH80", sm.OTH80);
					cmd.Parameters.AddWithValue("@OTH81", sm.OTH81);
					cmd.Parameters.AddWithValue("@OTH82", sm.OTH82);
					cmd.Parameters.AddWithValue("@OTH83", sm.OTH83);
					cmd.Parameters.AddWithValue("@ISACTIVE", sm.ISACTIVE);
					cmd.Parameters.AddWithValue("@CREATEDBY", sm.CREATEDBY);
					cmd.Parameters.AddWithValue("@UPDATEDBY", sm.UPDATEDBY);
					cmd.Parameters.AddWithValue("@UDISECODE", sm.UDISECODE);
					cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
					con.Open();
					result = cmd.ExecuteNonQuery();
					OutError = (string)cmd.Parameters["@OutError"].Value;
					return result;

					//con.Open();
					//result = cmd.ExecuteNonQuery();
					//return result;

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


		public string SchoolPremisesInformationFinalSubmit(string schl, out int OutStatus, out string OutError)  // BankLoginSP
		{
			SqlConnection con = null;
			string result = "";
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("SchoolPremisesInformationFinalSubmit", con);  //AllotPracMarks
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@schl", schl);
				cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery().ToString();
				OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
				OutError = (string)cmd.Parameters["@OutError"].Value;
				return result;
			}
			catch (Exception ex)
			{
				OutStatus = -1;
				OutError = ex.Message;
				return result = "";
			}
			finally
			{
				con.Close();
			}
		}


		public DataSet ViewSchoolPremisesInformation(int type1, string search, string schl, int pageIndex, out int OutStatus, int adminid)
		{
			SchoolPremisesInformation sm = new SchoolPremisesInformation();
			DataSet ds = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("ViewSchoolPremisesInformation", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@PageNumber", pageIndex);
					cmd.Parameters.AddWithValue("@PageSize", 20);
					cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
					cmd.Parameters.AddWithValue("@Adminid", adminid);
					cmd.Parameters.AddWithValue("@Type", type1);
					ad.SelectCommand = cmd;
					ad.Fill(ds);
					con.Open();
					OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
					return ds;

				}
			}
			catch (Exception ex)
			{
				OutStatus = 0;
				return null;
			}

		}



		#endregion SchoolPremisesInformation

		public DataSet UnlockCCE(string Schl, string Type, int AdminId, out int OutStatus)  // BankLoginSP
		{
			SqlDataAdapter dataAdapter = new SqlDataAdapter();
			DataSet dataTable = new DataSet();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("UnlockCCESP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Schl", Schl);
					cmd.Parameters.AddWithValue("@Type", Type);
					cmd.Parameters.AddWithValue("@AdminId", AdminId);
					cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
					con.Open();
					dataAdapter.SelectCommand = cmd;
					dataAdapter.Fill(dataTable);
					OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
					return dataTable;
				}
			}
			catch (Exception ex)
			{
				OutStatus = 0;
				return null;
			}
		}


		public DataSet ListingAllowCCE(int Type, int id, string search, out int OutStatus)  // BankLoginSP
		{
			SqlDataAdapter dataAdapter = new SqlDataAdapter();
			DataSet dataTable = new DataSet();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("ListingAllowCCESP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Type", Type);
					cmd.Parameters.AddWithValue("@id", id);
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
					con.Open();
					dataAdapter.SelectCommand = cmd;
					dataAdapter.Fill(dataTable);
					OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
					return dataTable;
				}
			}
			catch (Exception ex)
			{
				OutStatus = 0;
				return null;
			}
		}


		public int InsertSchoolAllowForCCE(int type, SchoolAllowForCCE FM, out string SchlMobile)
		{
			SqlConnection con = null;
			int result = 0;
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("InsertSchoolAllowForCCE", con);
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@Panel", FM.Panel);
				cmd.Parameters.AddWithValue("@Type", type);
				cmd.Parameters.AddWithValue("@Id", FM.Id);
				cmd.Parameters.AddWithValue("@Schl", FM.Schl);
				cmd.Parameters.AddWithValue("@Cls", FM.Cls);
				cmd.Parameters.AddWithValue("@LastDate", FM.LastDate);
				cmd.Parameters.AddWithValue("@AllowTo", FM.AllowTo);
				cmd.Parameters.AddWithValue("@ReceiptNo", FM.ReceiptNo);
				cmd.Parameters.AddWithValue("@DepositDate", FM.DepositDate);
				cmd.Parameters.AddWithValue("@Amount", FM.Amount);
				cmd.Parameters.AddWithValue("@AllowRemarks", FM.AllowRemarks);
				cmd.Parameters.AddWithValue("@EmpUserId", FM.EmpUserId);

				cmd.Parameters.Add("@OutStatus", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@SchlMobile", SqlDbType.VarChar, 20).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery();
				int outuniqueid = (int)cmd.Parameters["@OutStatus"].Value;
				SchlMobile = (string)cmd.Parameters["@SchlMobile"].Value;
				return outuniqueid;

			}
			catch (Exception ex)
			{
				SchlMobile = "";
				return result = -1;
			}
			finally
			{
				con.Close();
			}
		}

		#region DB Attendance Supervisory Staff 
		public DataSet GetAttendanceSupervisoryStaff(SchoolModels sm, string txtadmisndate)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetAttendancestafftable_Sp", con); //AttendanceSupervisoryStaff_SP
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cent", sm.ExamCent);
					cmd.Parameters.AddWithValue("@Class", sm.CLASS);
					cmd.Parameters.AddWithValue("@eDATE", txtadmisndate);
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

		public DataSet SetAttendanceSupervisoryStaff(SchoolModels sm, string EpunSearch, string attendance, string txtadmisndate)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SetAttendancestafftable_Sp", con); //AttendanceSupervisoryStaff_SP
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cent", sm.ExamCent);
					cmd.Parameters.AddWithValue("@Class", sm.CLASS);
					cmd.Parameters.AddWithValue("@eDATE", txtadmisndate);
					cmd.Parameters.AddWithValue("@Epunjanid", EpunSearch);
					cmd.Parameters.AddWithValue("@attendance", attendance);
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

		public DataSet DelAttendanceSupervisoryStaff(SchoolModels sm, string EpunSearch)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("DelAttendancestafftable_Sp", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cent", sm.ExamCent);
					cmd.Parameters.AddWithValue("@Class", sm.CLASS);
					cmd.Parameters.AddWithValue("@Epunjanid", EpunSearch);
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
		public DataSet FSAttendanceSupervisoryStaff(SchoolModels sm)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("FSAttendanceSupervisoryStaff", con); //AttendanceSupervisoryStaff_SP
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cent", sm.ExamCent);
					cmd.Parameters.AddWithValue("@Class", sm.CLASS);
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
		public DataSet GetAttendanceSupervisoryStaffReport(string Cent, string Class, string finalflg)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetAttendanceSupervisoryStaffReport_Sp", con); //AttendanceSupervisoryStaff_SP
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@cent", Cent);
					cmd.Parameters.AddWithValue("@Class", Class);
					cmd.Parameters.AddWithValue("@finalflg", finalflg);
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
		public DataSet UpdateBankAcStaff(string bank, string acno, string ifsc, string epunjabid, string cent)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("UpdateBankAcStaff_sp", con);//ReGenerateChallaanByIdSPAdminNew
					cmd.CommandType = CommandType.StoredProcedure;

					cmd.Parameters.AddWithValue("@epunjabid", epunjabid);
					cmd.Parameters.AddWithValue("@bank", bank);
					cmd.Parameters.AddWithValue("@acno", acno);
					cmd.Parameters.AddWithValue("@ifsc", ifsc);
					cmd.Parameters.AddWithValue("@cent", cent);
					ad.SelectCommand = cmd;
					ad.Fill(result);
					con.Open();
					//result = cmd.ExecuteNonQuery().ToString();

					if (con.State == ConnectionState.Open)
						con.Close();

					return result;
				}
			}
			catch (Exception ex)
			{
				return result = null;
			}
		}

		public DataSet CheckifscVal(string ifsc)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("CheckifscVal_sp", con);//ReGenerateChallaanByIdSPAdminNew
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@ifsc", ifsc);
					ad.SelectCommand = cmd;
					ad.Fill(result);
					con.Open();
					//result = cmd.ExecuteNonQuery().ToString();

					if (con.State == ConnectionState.Open)
						con.Close();

					return result;
				}
			}
			catch (Exception ex)
			{
				return result = null;
			}
		}
		#endregion DB Attendance Supervisory Staff 


		#region  SchoolAccreditation
		public DataSet GetSchoolAccreditation(string SCHL)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SchoolAccreditationSP", con);
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
				return null;
			}

		}

		public DataSet InsertSchoolAccreditation(SchoolAccreditationModel SA, int type, out string OutError)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("InsertSchoolAccreditationSP", con);// InsertRecheckSubjectList_SP//InsertSMFSP   //InsertSMFSPNew
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@Type", type);
					cmd.Parameters.AddWithValue("@id", SA.id);
					cmd.Parameters.AddWithValue("@schl", SA.schl);
					cmd.Parameters.AddWithValue("@refno", SA.refno);
					cmd.Parameters.AddWithValue("@class", SA.cls);
					cmd.Parameters.AddWithValue("@exam", SA.exam);
					cmd.Parameters.AddWithValue("@Acrtype", SA.Acrtype);
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

		public DataSet GetSchoolAccreditationPayment(string SCHL)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetSchoolAccreditationPaymentSP", con);
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
				return null;
			}

		}

		public DataSet GetSchoolAccreditationReport(string SCHL, string refno)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetSchoolAccreditationReportSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@SCHL", SCHL);
					cmd.Parameters.AddWithValue("@Refno", refno);
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

		#endregion v


		#region School to School Migration

		public DataSet ApplyStudentSchoolMigrationSearch(int type, string search, string schl)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("ApplyStudentSchoolMigrationSearchSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@type", type);
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
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



		public static string CancelStudentSchoolMigration(string cancelremarks, string stdid, string migid, out string outstatus, string updatedby, string Type)
		{
			try
			{
				string result = "";
				Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "CancelStudentSchoolMigrationSP";
				cmd.Parameters.AddWithValue("@MigrationId", migid);
				cmd.Parameters.AddWithValue("@stdid", stdid);
				cmd.Parameters.AddWithValue("@updatedby", updatedby);
				cmd.Parameters.AddWithValue("@cancelremarks", cancelremarks);
				cmd.Parameters.AddWithValue("@Type", Type);
				string userIP = AbstractLayer.StaticDB.GetFullIPAddress();
				cmd.Parameters.AddWithValue("@UserIP", userIP);
				cmd.Parameters.Add("@OutStatus", SqlDbType.VarChar, 10).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
				result = db.ExecuteNonQuery(cmd).ToString();
				outstatus = Convert.ToString(cmd.Parameters["@OutStatus"].Value);
				string OutError = Convert.ToString(cmd.Parameters["@OutError"].Value);
				return outstatus;
			}
			catch (Exception ex)
			{
				outstatus = "-1";
				return outstatus;
			}
		}

		public static string UpdateStatusStudentSchoolMigration(string EmpUserId, string remarks, string stdid, string migid, string status, string AppLevel, out string outstatus, string updatedby, string Type)
		{
			try
			{
				string result = "";
				Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "UpdateStatusStudentSchoolMigrationSP";
				cmd.Parameters.AddWithValue("@MigrationId", migid);
				cmd.Parameters.AddWithValue("@stdid", stdid);
				cmd.Parameters.AddWithValue("@status", status);
				cmd.Parameters.AddWithValue("@AppLevel", AppLevel);
				cmd.Parameters.AddWithValue("@remarks", remarks);
				cmd.Parameters.AddWithValue("@updatedby", updatedby);
				cmd.Parameters.AddWithValue("@Type", Type);
				cmd.Parameters.AddWithValue("@EmpUserId", EmpUserId);
				string userIP = AbstractLayer.StaticDB.GetFullIPAddress();
				cmd.Parameters.AddWithValue("@UserIP", userIP);
				cmd.Parameters.Add("@OutStatus", SqlDbType.VarChar, 10).Direction = ParameterDirection.Output;
				cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 200).Direction = ParameterDirection.Output;
				result = db.ExecuteNonQuery(cmd).ToString();
				outstatus = Convert.ToString(cmd.Parameters["@OutStatus"].Value);
				string OutError = Convert.ToString(cmd.Parameters["@OutError"].Value);
				return outstatus;
			}
			catch (Exception ex)
			{
				outstatus = "-1";
				return outstatus;
			}
		}



		public DataSet StudentSchoolMigrationsSearch(int type, string search, string schl)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("StudentSchoolMigrationsSearchSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@type", type);
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
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


		public List<StudentSchoolMigrationViewModel> StudentSchoolMigrationsSearchModel(int type, string search, string schl)
		{
			List<StudentSchoolMigrationViewModel> studentSchoolMigrationViewModel = new List<StudentSchoolMigrationViewModel>();
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("StudentSchoolMigrationsSearchSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@type", type);
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					ad.SelectCommand = cmd;
					ad.Fill(result);
					con.Open();
					if (result.Tables[0].Rows.Count > 0)
					{
						var itemSubUType = StaticDB.DataTableToList<StudentSchoolMigrationViewModel>(result.Tables[0]);
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


		public static DataSet GetStudentSchoolMigrationsPayment(int migid, string stdid)
		{
			try
			{
				DataSet ds = new DataSet();
				Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = 300;
				cmd.CommandText = "GetStudentSchoolMigrationsPaymentSP"; //InsertOnlinePaymentMISSP // InsertOnlinePaymentMISSPNEW
				cmd.Parameters.AddWithValue("@MigrationId", migid);
				cmd.Parameters.AddWithValue("@StdId", stdid);
				ds = db.ExecuteDataSet(cmd);
				//result = db.ExecuteNonQuery(cmd);                
				return ds;

			}
			catch (Exception ex)
			{

				return null;
			}

		}

		#endregion

		#region Begin Pre-Board Exam Theory 

		public string AllotPreBoardExamTheory(string stdid, DataTable dtSubPreBoard, string class1, out int OutStatus)  // BankLoginSP
		{
			SqlConnection con = null;
			string result = "";
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("AllotPreBoardExamTheory", con);  //AllotCCESenior
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@stdid", stdid);
				cmd.Parameters.AddWithValue("@dtSubPreBoard", dtSubPreBoard);
				cmd.Parameters.AddWithValue("@class", class1);
				cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery().ToString();
				OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
				return result;

			}
			catch (Exception ex)
			{
				OutStatus = -1;
				//mbox(ex);
				return result = "";
			}
			finally
			{
				con.Close();
			}
		}




		public DataSet GetPreBoardExamTheoryBySCHL(string search, string schl, int pageNumber, string class1, int action1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetPreBoardExamTheoryBySCHL", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 300;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
					cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
					cmd.Parameters.AddWithValue("@PageSize", 20);
					cmd.Parameters.AddWithValue("@Action", action1);
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


		public DataSet PreBoardExamTheoryREPORT(string search, string schl, string class1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("PreBoardExamTheoryREPORT", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 250;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
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

		public DataSet PreBoardExamTheoryREPORTFinalSubmit(string search, string schl, string class1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("PreBoardExamTheoryREPORTFinalSubmit", con);//[CCEREPORTFinalSubmit]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
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


		public DataSet PreBoardExamTheoryFinalReport(string search, string schl, string class1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("PreBoardExamTheoryFinalReport", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 250;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
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

		#endregion Pre-Board Exam Theory 


		public DataSet UnlockFinalSubmitNinthandEleventhResult(string cls, string schl)
		{
			try
			{
				DataSet ds = new DataSet();
				Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = 300;
				cmd.CommandText = "UnlockFinalSubmitNinthandEleventhResultSP";
				cmd.Parameters.AddWithValue("@cls", cls);
				cmd.Parameters.AddWithValue("@SCHL", schl);
				ds = db.ExecuteDataSet(cmd);
				//result = db.ExecuteNonQuery(cmd);                
				return ds;

			}
			catch (Exception ex)
			{
				return null;
			}

		}

		#region Open INA Portal 

		public string AllotMarksOpenINA(string stdid, DataTable dtSub, string class1, out int OutStatus)  // BankLoginSP
		{
			SqlConnection con = null;
			string result = "";
			try
			{
				con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
				SqlCommand cmd = new SqlCommand("AllotMarksOpenINA", con);  //AllotMarksOpenINA
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.Parameters.AddWithValue("@stdid", stdid);
				cmd.Parameters.AddWithValue("@dtSub", dtSub);
				cmd.Parameters.AddWithValue("@class", class1);
				cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
				con.Open();
				result = cmd.ExecuteNonQuery().ToString();
				OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
				return result;

			}
			catch (Exception ex)
			{
				OutStatus = -1;
				//mbox(ex);
				return result = "";
			}
			finally
			{
				con.Close();
			}
		}

		public DataSet SchoolAllowForOpenINAMarks(string SCHL, string cls)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("SchoolAllowForOpenINAMarksSP", con);
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@SCHL", SCHL);
					cmd.Parameters.AddWithValue("@cls", cls);
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



		public DataSet GetOpenINAMarksStudentsBySchool(string search, string schl, int pageNumber, string class1, int action1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("GetOpenINAMarksStudentsBySchool", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
					cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
					cmd.Parameters.AddWithValue("@PageSize", 20);
					cmd.Parameters.AddWithValue("@Action", action1);
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


		public DataSet OpenINAMarksReport(string search, string schl, string class1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("OpenINAMarksReport", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
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

		public DataSet OpenINAMarksReportFinalSubmit(string search, string schl, string class1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("OpenINAMarksReportFinalSubmit", con);//[OpenINAMarksReportFinalSubmit]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
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


		public DataSet OpenINAMarksFinalReport(string search, string schl, string class1)
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("OpenINAMarksFinalReport", con);//[GetStudentRegNoNotAllotedSP]
					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandTimeout = 250;
					cmd.Parameters.AddWithValue("@search", search);
					cmd.Parameters.AddWithValue("@schl", schl);
					cmd.Parameters.AddWithValue("@class", class1);
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

		#endregion

		#region 

		public static DataSet GetMeritoriousCentCodeBySchl(string SCHL)
		{
			try
			{
				DataSet ds = new DataSet();
				Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "GetMeritoriousCentCodeBySchlSP";
				cmd.Parameters.AddWithValue("@SCHL", SCHL);
				ds = db.ExecuteDataSet(cmd);
				//result = db.ExecuteNonQuery(cmd);                
				return ds;
			}
			catch (Exception ex)
			{

				return null;
			}
		}

		public static DataSet GetMeritoriousConfidentialList(SchoolModels sm)
		{
			try
			{
				DataSet ds = new DataSet();
				Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = 300;
				cmd.CommandText = "GetMeritoriousConfidentialListSP";
				cmd.Parameters.AddWithValue("@Cent", sm.ExamCent);
				cmd.Parameters.AddWithValue("@ExamRoll", sm.ExamRoll);
				ds = db.ExecuteDataSet(cmd);
				//result = db.ExecuteNonQuery(cmd);                
				return ds;
			}
			catch (Exception ex)
			{

				return null;
			}
		}





		#endregion

		#region  PrivateStudents Signature Chart and Confidential List Primary Middle Both

		public DataSet SignatureChartSP_PrivateStudents(int type, string cls, string SCHL, string cent)
		{
			try
			{
				DataSet ds = new DataSet();
				Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = 300;
				cmd.CommandText = "SignatureChartSP_PrivateStudents";
				cmd.Parameters.AddWithValue("@type", type);
				cmd.Parameters.AddWithValue("@cls", cls);
				cmd.Parameters.AddWithValue("@SCHL", SCHL);
				cmd.Parameters.AddWithValue("@cent", cent);
				ds = db.ExecuteDataSet(cmd);
				//result = db.ExecuteNonQuery(cmd);                
				return ds;
			}
			catch (Exception ex)
			{

				return null;
			}
		}

		public DataSet GetSignatureChartSP_PrivateStudents(SchoolModels sm)
		{
			try
			{
				string roll = "";
				if (sm.ExamRoll != "")
				{
					roll = " and roll='" + sm.ExamRoll + "'";
				}
				DataSet ds = new DataSet();
				Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = 300;
				cmd.CommandText = "GetSignatureChartSP_PrivateStudents";
				cmd.Parameters.AddWithValue("@cent", sm.ExamCent);
				cmd.Parameters.AddWithValue("@sub", sm.ExamSub);
				cmd.Parameters.AddWithValue("@roll", roll);
				cmd.Parameters.AddWithValue("@class", sm.CLASS);
				ds = db.ExecuteDataSet(cmd);
				//result = db.ExecuteNonQuery(cmd);                
				return ds;
			}
			catch (Exception ex)
			{

				return null;
			}
		}


		public DataSet GetConfidentialListSP_PrivateStudents(SchoolModels sm)
		{
			try
			{
				DataSet ds = new DataSet();
				Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = 300;
				cmd.CommandText = "GetConfidentialListSP_PrivateStudents";
				cmd.Parameters.AddWithValue("@Cent", sm.ExamCent);
				cmd.Parameters.AddWithValue("@class", sm.CLASS);
				ds = db.ExecuteDataSet(cmd);
				//result = db.ExecuteNonQuery(cmd);                
				return ds;
			}
			catch (Exception ex)
			{

				return null;
			}
		}


		#endregion PrivateStudents Signature Chart and Confidential List Primary Middle Both

		#region  Online Centre Creations

		public static OnlineCentreCreationsPaymentForm GetOnlineCentreCreationsPayment(string CentreAppNo)
		{
			try
			{
				OnlineCentreCreationsPaymentForm onlineCentreCreationsPaymentForm = new OnlineCentreCreationsPaymentForm();

				DataSet ds = new DataSet();
				Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = 300;
				cmd.CommandText = "GetOnlineCentreCreationsPaymentSP";
				cmd.Parameters.AddWithValue("@CentreAppNo", CentreAppNo);
				ds = db.ExecuteDataSet(cmd);

				if (ds.Tables.Count > 0)
				{
					if (ds.Tables[0].Rows.Count > 0)
					{
						var itemSubUType = StaticDB.DataTableToList<OnlineCentreCreationsPaymentForm>(ds.Tables[0]);
						onlineCentreCreationsPaymentForm = itemSubUType.SingleOrDefault();

					}
				}
				return onlineCentreCreationsPaymentForm;

			}
			catch (Exception ex)
			{
				return null;
			}

		}



		public static DataSet OnlineCenterCreationReport(string term, int ReportType, string Search)
		{
			try
			{
				DataSet ds = new DataSet();
				Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = 300;
				cmd.CommandText = "OnlineCenterCreationReportSP"; //InsertOnlinePaymentMISSP // InsertOnlinePaymentMISSPNEW
				cmd.Parameters.AddWithValue("@term", term);
				cmd.Parameters.AddWithValue("@ReportType", ReportType);
				cmd.Parameters.AddWithValue("@Search", Search);
				ds = db.ExecuteDataSet(cmd);
				return ds;
			}
			catch (Exception ex)
			{

				return null;
			}

		}
		#endregion


		#region PhyChlMarksEntry Marks Entry Panel  

		public DataSet GetPhyChlMarksEntryMarksDataBySCHL(string search, string schl, int pageNumber, string class1, int action1)
		{
			try
			{
				Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "GetPhyChlMarksEntryMarksDataBySCHL"; //GetDataBySCHL
				cmd.Parameters.AddWithValue("@search", search);
				cmd.Parameters.AddWithValue("@schl", schl);
				cmd.Parameters.AddWithValue("@class", class1);
				cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
				cmd.Parameters.AddWithValue("@PageSize", 20);
				cmd.Parameters.AddWithValue("@Action", action1);
				return db.ExecuteDataSet(cmd);
			}
			catch (Exception)
			{

				return null;
			}
		}

		public string AllotPhyChlMarksEntryMarks(string submitby, string stdid, DataTable dtSub, string class1, out int OutStatus)
		{
			try
			{
				Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				string result = "";
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "AllotPhyChlMarksEntryMarks"; //AllotPhyChlMarksEntrySenior
				cmd.Parameters.AddWithValue("@submitby", submitby);
				cmd.Parameters.AddWithValue("@stdid", stdid);
				cmd.Parameters.AddWithValue("@dtSub", dtSub);
				cmd.Parameters.AddWithValue("@class", class1);
				cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
				result = db.ExecuteNonQuery(cmd).ToString();
				OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
				return result;
			}
			catch (Exception)
			{
				OutStatus = -1;
				return null;
			}
		}


		public DataSet PhyChlMarksEntryMarksEntryReport(string CenterId, int reporttype, string search, string schl, string cls, out string OutError)
		{
			try
			{
				Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				DataSet ds = new DataSet();
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "PhyChlMarksEntryMarksEntryReport"; //
				cmd.Parameters.AddWithValue("@CenterId", CenterId);
				cmd.Parameters.AddWithValue("@reporttype", reporttype);
				cmd.Parameters.AddWithValue("@search", search);
				cmd.Parameters.AddWithValue("@schl", schl);
				cmd.Parameters.AddWithValue("@class", cls);
				cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
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
		#endregion PhyChlMarksEntry Marks Entry Panel 


		#region  Re-Exam For Absent Student in Term-1
		public static List<ReExamTermStudentsSearchModel> GetReExamTermStudentList(string type, string RP, string cls, string schl, string search, out DataSet dsOut)
		{
			List<ReExamTermStudentsSearchModel> registrationSearchModels = new List<ReExamTermStudentsSearchModel>();
			DataSet ds = new DataSet();
			Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
			SqlCommand cmd = new SqlCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "GetReExamTermStudentListSP";
			cmd.Parameters.AddWithValue("@type", type);
			cmd.Parameters.AddWithValue("@RP", RP);
			cmd.Parameters.AddWithValue("@Class", cls);
			cmd.Parameters.AddWithValue("@schl", schl);
			cmd.Parameters.AddWithValue("@search", search);
			ds = db.ExecuteDataSet(cmd);
			if (ds != null)
			{
				var eList = ds.Tables[0].AsEnumerable().Select(dataRow => new ReExamTermStudentsSearchModel
				{
					Std_id = dataRow.Field<long>("Std_id"),
					Roll = dataRow.Field<string>("Roll"),
					form = dataRow.Field<string>("form"),
					SCHL = dataRow.Field<string>("SCHL"),
					AdmDate = dataRow.Field<string>("AdmDate"),
					name = dataRow.Field<string>("name"),
					fname = dataRow.Field<string>("fname"),
					mname = dataRow.Field<string>("mname"),
					DOB = dataRow.Field<string>("DOB"),
					Aadhar = dataRow.Field<string>("Aadhar"),
					//
					regno = dataRow.Field<string>("regno"),
					Dist = dataRow.Field<string>("Dist"),
					EXAM = dataRow.Field<string>("EXAM"),
					phy_chal = dataRow.Field<string>("phy_chal"),
					IsExistsInReExamTermStudents = dataRow.Field<int>("IsExistsInReExamTermStudents"),
					ReExamId = dataRow.Field<long>("ReExamId"),
					IsChallanCancel = dataRow.Field<int>("IsChallanCancel"),

				}).ToList();

				registrationSearchModels = eList.ToList();
			}
			dsOut = ds;
			return registrationSearchModels;

		}


		public int InsertReExamTermStudentList(List<ReExamTermStudents> list)
		{
			int result = 0;
			if (list.Count() > 0)
			{
				context.ReExamTermStudents.AddRange(list);
				result = context.SaveChanges();
			}
			return result;
		}

		public int RemoveRangeOnDemandCertificateStudentList(List<ReExamTermStudents> list)
		{
			int result = 0;
			if (list.Count() > 0)
			{
				//context.OnDemandCertificates.RemoveRange(list);  
				int i = 0;
				foreach (ReExamTermStudents reExamTermStudents in list)
				{
					context.ReExamTermStudents.Attach(reExamTermStudents);
					context.ReExamTermStudents.Remove(reExamTermStudents);
					context.SaveChanges();
					i++;
				}
				result = i;
			}
			return result;
		}


		public static List<ReExamTermStudents_ChallanDetailsViews> ReExamTermStudents_ChallanList(string schl, out DataSet dsOut)
		{
			List<ReExamTermStudents_ChallanDetailsViews> registrationSearchModels = new List<ReExamTermStudents_ChallanDetailsViews>();
			DataSet ds = new DataSet();
			Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
			SqlCommand cmd = new SqlCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "ReExamTermStudents_ChallanListSP";
			cmd.Parameters.AddWithValue("@schl", schl);
			ds = db.ExecuteDataSet(cmd);
			if (ds != null)
			{
				var eList = StaticDB.DataTableToList<ReExamTermStudents_ChallanDetailsViews>(ds.Tables[0]);
				registrationSearchModels = eList.ToList();
			}
			dsOut = ds;
			return registrationSearchModels;

		}


		public static DataSet ReExamTermStudentsCountRecordsClassWise(string search, string schl)
		{
			List<ReExamTermStudents_ChallanDetailsViews> registrationSearchModels = new List<ReExamTermStudents_ChallanDetailsViews>();
			DataSet ds = new DataSet();
			Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
			SqlCommand cmd = new SqlCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "ReExamTermStudentsCountRecordsClassWise_SP";
			cmd.Parameters.AddWithValue("@search", search);
			cmd.Parameters.AddWithValue("@Schl", schl);
			ds = db.ExecuteDataSet(cmd);
			return ds;

		}


		public static DataSet ReExamTermStudentCalculateFee(string cls, string date, string search, string schl)
		{
			List<ReExamTermStudents_ChallanDetailsViews> registrationSearchModels = new List<ReExamTermStudents_ChallanDetailsViews>();
			DataSet ds = new DataSet();
			Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
			SqlCommand cmd = new SqlCommand();
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.CommandText = "ReExamTermStudentCalculateFeeSP";
			cmd.Parameters.AddWithValue("@cls", cls);
			cmd.Parameters.AddWithValue("@Schl", schl);
			cmd.Parameters.AddWithValue("@date", date);
			cmd.Parameters.AddWithValue("@search", search);
			ds = db.ExecuteDataSet(cmd);

			return ds;

		}
		#endregion


		#region  SchoolBasedExams Marks Entry Panel     

		public DataSet GetSchoolBasedExamsMarksDataBySCHL(string search, string schl, int pageNumber, string class1, int action1)
		{
			try
			{
				DataSet ds = new DataSet();
				Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "GetSchoolBasedExamsMarksDataBySCHL"; //GetDataBySCHL
				cmd.Parameters.AddWithValue("@search", search);
				cmd.Parameters.AddWithValue("@schl", schl);
				cmd.Parameters.AddWithValue("@class", class1);
				cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
				cmd.Parameters.AddWithValue("@PageSize", 20);
				cmd.Parameters.AddWithValue("@Action", action1);
				ds = db.ExecuteDataSet(cmd);
				return ds;
			}
			catch (Exception ex)
			{

				return null;
			}
		}

		public string AllotSchoolBasedExamsMarks(string submitby, string stdid, DataTable dtSub, string class1, out int OutStatus)
		{
			try
			{
				string result = "";
				Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "AllotSchoolBasedExamsMarks"; //AllotCCESenior
				cmd.Parameters.AddWithValue("@submitby", submitby);
				cmd.Parameters.AddWithValue("@stdid", stdid);
				cmd.Parameters.AddWithValue("@dtSub", dtSub);
				cmd.Parameters.AddWithValue("@class", class1);
				cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
				result = db.ExecuteNonQuery(cmd).ToString();
				OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
				return result;
			}
			catch (Exception ex)
			{
				OutStatus = -1;
				return null;
			}
		}


		public DataSet SchoolBasedExamsMarksEntryReport(string CenterId, int reporttype, string search, string schl, string cls, out string OutError)
		{
			try
			{
				Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				DataSet ds = new DataSet();
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "SchoolBasedExamsMarksEntryReport"; //
				cmd.Parameters.AddWithValue("@CenterId", CenterId);
				cmd.Parameters.AddWithValue("@reporttype", reporttype);
				cmd.Parameters.AddWithValue("@search", search);
				cmd.Parameters.AddWithValue("@schl", schl);
				cmd.Parameters.AddWithValue("@class", cls);
				cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
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





		public DataSet GetSchoolBasedExamsMarksDataBySCHLOpen(string search, string schl, int pageNumber, string class1, int action1)
		{
			try
			{
				DataSet ds = new DataSet();
				Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "GetSchoolBasedExamsMarksDataBySCHLOpen"; //GetDataBySCHL
				cmd.Parameters.AddWithValue("@search", search);
				cmd.Parameters.AddWithValue("@schl", schl);
				cmd.Parameters.AddWithValue("@class", class1);
				cmd.Parameters.AddWithValue("@PageNumber", pageNumber);
				cmd.Parameters.AddWithValue("@PageSize", 20);
				cmd.Parameters.AddWithValue("@Action", action1);
				ds = db.ExecuteDataSet(cmd);
				return ds;
			}
			catch (Exception ex)
			{

				return null;
			}
		}

		public string AllotSchoolBasedExamsMarksOpen(string submitby, string stdid, DataTable dtSub, string class1, out int OutStatus)
		{
			try
			{
				string result = "";
				Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "AllotSchoolBasedExamsMarksOpen"; //AllotCCESenior
				cmd.Parameters.AddWithValue("@submitby", submitby);
				cmd.Parameters.AddWithValue("@stdid", stdid);
				cmd.Parameters.AddWithValue("@dtSub", dtSub);
				cmd.Parameters.AddWithValue("@class", class1);
				cmd.Parameters.Add("@OutStatus", SqlDbType.Int).Direction = ParameterDirection.Output;
				result = db.ExecuteNonQuery(cmd).ToString();
				OutStatus = (int)cmd.Parameters["@OutStatus"].Value;
				return result;
			}
			catch (Exception ex)
			{
				OutStatus = -1;
				return null;
			}
		}


		public DataSet SchoolBasedExamsMarksEntryReportOpen(string CenterId, int reporttype, string search, string schl, string cls, out string OutError)
		{
			try
			{
				Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				DataSet ds = new DataSet();
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "SchoolBasedExamsMarksEntryReportOpen"; //
				cmd.Parameters.AddWithValue("@CenterId", CenterId);
				cmd.Parameters.AddWithValue("@reporttype", reporttype);
				cmd.Parameters.AddWithValue("@search", search);
				cmd.Parameters.AddWithValue("@schl", schl);
				cmd.Parameters.AddWithValue("@class", cls);
				cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
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

		#endregion  SchoolBasedExams Marks Entry Panel  


		#region  InfrasturePerforma

		public DataSet PanelEntryLastDate(string sModule)
		{
			try
			{
				DataSet ds = new DataSet();
				Microsoft.Practices.EnterpriseLibrary.Data.Database db = DatabaseFactory.CreateDatabase("myDBConnection");
				SqlCommand cmd = new SqlCommand();
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandText = "sp_PanelEntryLastDatebyModule"; //GetDataBySCHL
				cmd.Parameters.AddWithValue("@Pv_ModeuleName", sModule);
				ds = db.ExecuteDataSet(cmd);
				return ds;
			}
			catch (Exception ex)
			{

				return null;
			}
		}

		public bool IFSCCheck(string IFSC, string BANK)
		{
			bool b = false;
			Tblifsccodes obj = new Tblifsccodes();
			if (IFSC != null)
			{
				obj = context.Tblifsccodes.SingleOrDefault(x => x.BANK.Trim() == BANK.Trim() && x.IFSC.Trim() == IFSC.Trim());
				if (obj == null)
				{
					b = false;
				}
				else
				{
					b = true;
				}

			}
			return b;
		}

		public Task<InfrasturePerformas> GetInfrasturePerformaBySCHL(LoginSession LM)  // Type 1=Regular, 2=Open
		{
			InfrasturePerformas obj = new InfrasturePerformas();
			if (LM != null)
			{
				obj = context.InfrasturePerformas.SingleOrDefault(x => x.SCHL.Trim() == LM.SCHL.Trim());
				if (obj == null)
				{
					var ipsNew = new InfrasturePerformas()
					{
						SCHL = LM.SCHL
					};
					context.InfrasturePerformas.Add(ipsNew);
					context.SaveChanges();
					obj = context.InfrasturePerformas.SingleOrDefault(x => x.SCHL.Trim() == LM.SCHL.Trim());
				}

			}
			Thread.Sleep(2000);
			return Task.FromResult(obj);

		}

		public DataTable ToDataTable<T>(List<T> items)
		{
			DataTable dataTable = new DataTable(typeof(T).Name);
			//Get all the properties
			PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (PropertyInfo prop in Props)
			{
				//Setting column names as Property names
				dataTable.Columns.Add(prop.Name);
			}
			foreach (T item in items)
			{
				var values = new object[Props.Length];
				for (int i = 0; i < Props.Length; i++)
				{
					//inserting property values to datatable rows
					values[i] = Props[i].GetValue(item, null);
				}
				dataTable.Rows.Add(values);
			}
			//put a breakpoint here and check datatable
			return dataTable;
		}

		public DataTable GetInfrasturePerformaBySCHLListSearch(string SCHL, string Dist)  // Type 1=Regular, 2=Open
		{
			List<InfrasturePerformasList> obj = new List<InfrasturePerformasList>();
			if (Dist.ToLower() == "all" && SCHL.ToLower() == "all")
			{
				obj = context.InfrasturePerformasList.ToList();
			}
			else if (SCHL.ToLower() == "all")
			{
				obj = context.InfrasturePerformasList.Where(x => x.DIST.Trim() == Dist).ToList();
			}
			else if (Dist.ToLower() == "all")
			{
				obj = context.InfrasturePerformasList.Where(x => x.SCHL.Trim() == SCHL).ToList();
			}
			else
			{
				obj = context.InfrasturePerformasList.Where(x => x.DIST.Trim() == Dist && x.SCHL == SCHL).ToList();
			}
			Thread.Sleep(2000);
			return ToDataTable(obj);
			//return obj;

		}

		public Task<ChallanModels> GetChallanDetail(string ChallanID)  // Type 1=Regular, 2=Open
		{
			ChallanModels obj = new ChallanModels();
			obj = context.ChallanModels.Where(x => x.CHALLANID.Trim() == ChallanID).FirstOrDefault();

			return Task.FromResult(obj);

		}
		public DataTable GetAllTCode()
		{
			DataSet result = new DataSet();
			SqlDataAdapter ad = new SqlDataAdapter();
			try
			{
				using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
				{
					SqlCommand cmd = new SqlCommand("select distinct Tcode,DIST from InfrasturePerformasListWithSchools", con);
					cmd.CommandType = CommandType.Text;
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

		public List<SelectListItem> GetTCode()
		{
			DataTable dsSession = GetAllTCode(); // SessionMasterSPAdmin
			List<SelectListItem> itemSession = new List<SelectListItem>();
			// itemSession.Add(new SelectListItem { Text = "2017", Value = "2017" });
			foreach (System.Data.DataRow dr in dsSession.Rows)
			{
				itemSession.Add(new SelectListItem { Text = @dr["Tcode"].ToString(), Value = @dr["DIST"].ToString() });
			}
			return itemSession;
		}


		public Task<List<InfrasturePerformasListWithSchool>> GetInfrasturePerformaWithSchool(string sDIST, string sSCHL, string sTcode)  // Type 1=Regular, 2=Open
		{
			List<InfrasturePerformasListWithSchool> obj = new List<InfrasturePerformasListWithSchool>();
			if (sTcode.ToLower() == "all")
			{
				if (sSCHL.ToLower() == "all" && sDIST.ToLower() == "all")
				{
					obj = context.InfrasturePerformasListWithSchool.ToList();
				}
				else if (sSCHL.ToLower() == "all")
				{
					obj = context.InfrasturePerformasListWithSchool.Where(x => x.DIST == sDIST).ToList();
				}
				else if (sDIST.ToLower() == "all")
				{
					obj = context.InfrasturePerformasListWithSchool.Where(x => x.SCHL == sSCHL).ToList();
				}
				else
				{
					obj = context.InfrasturePerformasListWithSchool.Where(x => x.SCHL == sSCHL && x.DIST == sDIST).ToList();
				}
			}
			else
			{
				if (sSCHL.ToLower() == "all" && sDIST.ToLower() == "all")
				{
					obj = context.InfrasturePerformasListWithSchool.Where(x => x.Tcode == sTcode).ToList();
				}
				else if (sSCHL.ToLower() == "all")
				{
					obj = context.InfrasturePerformasListWithSchool.Where(x => x.DIST == sDIST && x.Tcode == sTcode).ToList();
				}
				else if (sDIST.ToLower() == "all")
				{
					obj = context.InfrasturePerformasListWithSchool.Where(x => x.SCHL == sSCHL && x.Tcode == sTcode).ToList();
				}
				else
				{
					obj = context.InfrasturePerformasListWithSchool.Where(x => x.SCHL == sSCHL && x.DIST == sDIST && x.Tcode == sTcode).ToList();
				}

			}
			Thread.Sleep(2000);
			return Task.FromResult(obj);

		}

		public Task<InfrasturePerformasList> GetInfrasturePerformaBySCHLList(LoginSession LM)  // Type 1=Regular, 2=Open
		{
			InfrasturePerformasList obj = new InfrasturePerformasList();
			if (LM != null)
			{
				obj = context.InfrasturePerformasList.SingleOrDefault(x => x.SCHL.Trim() == LM.SCHL.Trim());
				if (obj == null)
				{
					var ipsNew = new InfrasturePerformasList()
					{
						SCHL = LM.SCHL
					};
					context.InfrasturePerformasList.Add(ipsNew);
					context.SaveChanges();
					obj = context.InfrasturePerformasList.SingleOrDefault(x => x.SCHL.Trim() == LM.SCHL.Trim());
				}

			}
			Thread.Sleep(2000);
			return Task.FromResult(obj);

		}


		public Task<InfrasturePerformas> UpdateInfrasturePerformaBySCHL(InfrasturePerformas ips, out int ireturn)
		{
			InfrasturePerformas obj = context.InfrasturePerformas.SingleOrDefault(s => s.SCHL.ToUpper() == ips.SCHL.ToUpper());
			obj.Col1 = ips.Col1;
			obj.Col2 = ips.Col2;
			obj.Col3 = ips.Col3;
			obj.Col4 = ips.Col4;
			obj.Col5 = ips.Col5;
			obj.Col6 = ips.Col6;
			obj.Col7 = ips.Col7;
			obj.Col8 = ips.Col8;
			obj.Col9 = ips.Col9;
			obj.Col10 = ips.Col10;
			obj.Col11 = ips.Col11;
			obj.Col12 = ips.Col12;
			obj.Col13 = ips.Col13;
			obj.Col14 = ips.Col14;
			obj.Col15 = ips.Col15;
			obj.Col16 = ips.Col16;
			obj.Col17 = ips.Col17;
			obj.Col18 = ips.Col18;
			obj.Col19 = ips.Col19;
			obj.Col20 = ips.Col20;
			obj.Col21 = ips.Col21;
			obj.Col22 = ips.Col22;
			obj.Col23 = ips.Col23;
			obj.Col24 = ips.Col24;
			obj.Col25A = ips.Col25A;
			obj.Col25B = ips.Col25B;
			obj.Col25C = ips.Col25C;
			obj.Col29 = ips.Col29;
			obj.Col30 = ips.Col30;
			obj.IFSC = ips.IFSC;
			obj.Bank = ips.Bank;
			obj.BankAddress = ips.BankAddress;
			obj.SOLID = ips.SOLID;
			obj.IFSC1 = ips.IFSC1;
			obj.Bank1 = ips.Bank1;
			obj.BankBranch1 = ips.BankBranch1;
			obj.IFSC2 = ips.IFSC2;
			obj.Bank2 = ips.Bank2;
			obj.BankBranch2 = ips.BankBranch2;
			obj.IFSC3 = ips.IFSC3;
			obj.Bank3 = ips.Bank3;
			obj.FinalSubmitStatus = ips.FinalSubmitStatus;
			obj.FinalSubmitDate = ips.FinalSubmitDate;
			obj.BankBranch3 = ips.BankBranch3;

			context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
			ireturn = context.SaveChanges();
			if (ips != null)
			{
				obj = context.InfrasturePerformas.SingleOrDefault(x => x.SCHL.Trim() == ips.SCHL.Trim());
			}
			Thread.Sleep(2000);
			return Task.FromResult(obj);

		}
        #endregion

        #region  CentreExamDateView
        public DataSet CentreExamDateView(int type, string search, string cent)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CentreExamDateSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@cent", cent);
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
        #endregion

        
        #region  AttendanceSupervisoryStaffUnlock
        public DataSet AttendanceSupervisoryStaffUnlock(int type, string search, string cent)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("CentreExamUnlockSP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@search", search);
                    cmd.Parameters.AddWithValue("@cent", cent);
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
        #endregion


        #region REGOPENPracticalExamCentre
        public DataSet PracticalExamCentreList(string Class, string Search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("REGOPENPracticalExamCentre", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Class", Class);
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
        #endregion REGOPENPracticalExamCentre

    }
}