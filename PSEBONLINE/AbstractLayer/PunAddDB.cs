using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using PSEBONLINE.Models;
using System.Web.Mvc;
using Ionic.Zip;
using System.IO;
using System.Web.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;

namespace PSEBONLINE.AbstractLayer
{
    public class PunAddDB
    {
        #region Check ConString
        private string CommonCon = "myDBConnection";
        public PunAddDB()
        {
            CommonCon = "myDBConnection";
        }

        #endregion  Check ConString
        #region Admin Pannel DB
        public DataSet GetPunAddbatchYear()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetPunAddbatchYearAdmit_sp", con); //
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

        public List<SelectListItem> GetActivePBIResultBatchList()
        {
            DataSet result = GetPunAddbatchYear();
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in result.Tables[0].Rows)
            {
                items.Add(new SelectListItem { Text = @dr["batchYear"].ToString(), Value = @dr["batchYear"].ToString() });
            }
            return items;
        }


public DataSet GetPunAddbatchYearAdmitCard()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetPunAddbatchYearAdmitcard_sp", con);
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
        public DataSet GetPunAddRecordByBatch(string Search, int pageIndex)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //SqlCommand cmd = new SqlCommand("GetAllSchoolsByDistTC_sp", con);
                    SqlCommand cmd = new SqlCommand("GetPunAddRecordByBatch_sp", con);
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
       
        public static DataSet AdminInsertPrivateCandidateConfirmation(PunAddModels MS, out string OutError)  // Type 1=Regular, 2=Open
        {

            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "AdminInsertTblPunAddConfirmation_SP";
                //
                cmd.Parameters.AddWithValue("@IsHardCopyCertificate", MS.IsHardCopyCertificate);
                cmd.Parameters.AddWithValue("@Board", MS.Board);
                cmd.Parameters.AddWithValue("@Other_Board", MS.Other_Board);
                cmd.Parameters.AddWithValue("@Result", MS.Result);
                cmd.Parameters.AddWithValue("@MatricMarks", MS.MatricMarks);

                cmd.Parameters.AddWithValue("@refno", MS.refNo);
                cmd.Parameters.AddWithValue("@OROLL", MS.OROLL);
                cmd.Parameters.AddWithValue("@SelExamDist", MS.SelExamDist);

                cmd.Parameters.AddWithValue("@RegNo", MS.RegNo);
                cmd.Parameters.AddWithValue("@Candi_Name", MS.Candi_Name);
                cmd.Parameters.AddWithValue("@Pname", MS.Pname);
                cmd.Parameters.AddWithValue("@Father_Name", MS.Father_Name);
                cmd.Parameters.AddWithValue("@PFname", MS.PFname);
                cmd.Parameters.AddWithValue("@Mother_Name", MS.Mother_Name);
                cmd.Parameters.AddWithValue("@PMname", MS.PMname);
                cmd.Parameters.AddWithValue("@DOB", MS.DOB);

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

                cmd.Parameters.AddWithValue("@Addess", MS.address);
                cmd.Parameters.AddWithValue("@landmark", MS.landmark);
                cmd.Parameters.AddWithValue("@block", MS.block);
                cmd.Parameters.AddWithValue("@SelDist", MS.SelDist);
                cmd.Parameters.AddWithValue("@tehsil", MS.tehsil);
                cmd.Parameters.AddWithValue("@pinCode", MS.pinCode);
                cmd.Parameters.AddWithValue("@cat", MS.category);
                cmd.Parameters.AddWithValue("@Class", MS.Class);
                cmd.Parameters.AddWithValue("@PathPhoto", MS.PathPhoto);
                cmd.Parameters.AddWithValue("@PathSign", MS.PathSign);
                cmd.Parameters.AddWithValue("@EligibilityDoc1", MS.EligibilityDoc1);
                cmd.Parameters.AddWithValue("@EligibilityDoc2", MS.EligibilityDoc2);
                cmd.Parameters.AddWithValue("@AdminId", MS.AdminId);
                cmd.Parameters.AddWithValue("@EmpUserId", MS.EmpUserId);
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


        public static DataSet InsertPrivateCandidateConfirmation(PunAddModels MS, out string OutError)  // Type 1=Regular, 2=Open
        {

            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertTblPunAddConfirmation_SP";
                //
                cmd.Parameters.AddWithValue("@IsHardCopyCertificate", MS.IsHardCopyCertificate);
                cmd.Parameters.AddWithValue("@Board", MS.Board);
                cmd.Parameters.AddWithValue("@Other_Board", MS.Other_Board);
                cmd.Parameters.AddWithValue("@Result", MS.Result);
                cmd.Parameters.AddWithValue("@MatricMarks", MS.MatricMarks);

                cmd.Parameters.AddWithValue("@refno", MS.refNo);
                cmd.Parameters.AddWithValue("@OROLL", MS.OROLL);
                cmd.Parameters.AddWithValue("@SelExamDist", MS.SelExamDist);

                cmd.Parameters.AddWithValue("@RegNo", MS.RegNo);
                cmd.Parameters.AddWithValue("@Candi_Name", MS.Candi_Name);
                cmd.Parameters.AddWithValue("@Pname", MS.Pname);
                cmd.Parameters.AddWithValue("@Father_Name", MS.Father_Name);
                cmd.Parameters.AddWithValue("@PFname", MS.PFname);
                cmd.Parameters.AddWithValue("@Mother_Name", MS.Mother_Name);
                cmd.Parameters.AddWithValue("@PMname", MS.PMname);
                cmd.Parameters.AddWithValue("@DOB", MS.DOB);

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

                cmd.Parameters.AddWithValue("@Addess", MS.address);
                cmd.Parameters.AddWithValue("@landmark", MS.landmark);
                cmd.Parameters.AddWithValue("@block", MS.block);
                cmd.Parameters.AddWithValue("@SelDist", MS.SelDist);
                cmd.Parameters.AddWithValue("@tehsil", MS.tehsil);
                cmd.Parameters.AddWithValue("@pinCode", MS.pinCode);
                cmd.Parameters.AddWithValue("@cat", MS.category);
                cmd.Parameters.AddWithValue("@Class", MS.Class);
                cmd.Parameters.AddWithValue("@PathPhoto", MS.PathPhoto);
                cmd.Parameters.AddWithValue("@PathSign", MS.PathSign);
                cmd.Parameters.AddWithValue("@EligibilityDoc1", MS.EligibilityDoc1);
                cmd.Parameters.AddWithValue("@EligibilityDoc2", MS.EligibilityDoc2);
                cmd.Parameters.Add("@OutError", SqlDbType.VarChar, 1000).Direction = ParameterDirection.Output;
                //
                ds = db.ExecuteDataSet(cmd);
                OutError = (string)cmd.Parameters["@OutError"].Value;                
                return ds;
            }
            catch (Exception ex)
            {
                OutError = "-1";     
                return  null;
            }
        }

        public DataSet GenerateRollPunAdd(string Search,string batch,string batchyear)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GenerateRollPunAdd_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@batch", batch);
                    cmd.Parameters.AddWithValue("@batchyear", batchyear);
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

        public void DownloadFiles(string Search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            string mdbPathPunAdd = WebConfigurationManager.AppSettings["mdbPathPunAdd"].ToString();
            string mdbPathCorBlank = WebConfigurationManager.AppSettings["mdbPathPunBlackAdd"].ToString();
            System.Data.OleDb.OleDbConnection AccessConn = new
            System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbPathPunAdd + "");
            try
            {
                DataSet resultTmp = new DataSet();
                SqlDataAdapter adTmp = new SqlDataAdapter();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmdTmp = new SqlCommand("InsertTblPunAdd", con);
                    cmdTmp.CommandType = CommandType.StoredProcedure;
                    cmdTmp.Parameters.AddWithValue("@Search", Search);
                    adTmp.SelectCommand = cmdTmp;
                    adTmp.Fill(resultTmp);
                    con.Open();
                    //return resultTmp;
                }

                if (File.Exists(mdbPathPunAdd))
                {
                    File.Delete(mdbPathPunAdd);
                }
                else
                {
                    File.Copy(mdbPathCorBlank, mdbPathPunAdd);
                }
                AccessConn.Open();
                System.Data.OleDb.OleDbCommand AccessCommand = new System.Data.OleDb.OleDbCommand("select * into [tblPunAddDataAccess]  FROM [tblPunAddDataAccess$]  IN '' [ODBC;Driver={SQL Server};Server=43.224.136.122,2499;Database=firmdata_online;uid=sa;pwd=pU74h5#x;Connect Timeout=1200;];", AccessConn);
                AccessCommand.ExecuteNonQuery();
                AccessConn.Close();
                System.Threading.Thread.Sleep(1000);
                DownloadFilesData();
                #region old Code
                //using (ZipFile zip = new ZipFile())
                //{
                //    zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                //    string str1 = string.Empty;
                //    foreach (string str in GetListofPhotoSignature())
                //    {
                //        if (File.Exists(str) == true)
                //        {
                //            zip.AddFile(str, "ImgandSing");

                //        }
                //        else
                //        {
                //            str1 = str1 + " " + str;
                //            //Response.Write("Missing Image :" + str);
                //        }

                //    }


                //    zip.AddFile(mdbPathPunAdd, "DataFile");
                //    //zip.AddFile("D:\\PSEBONLINE\\PSEBONLINE\\Download\\Backup.mdb", "DataFile");

                //    HttpContext.Current.Response.Clear();
                //    HttpContext.Current.Response.BufferOutput = false;
                //    string zipName = String.Format("PunAddfile" + "Data_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                //    HttpContext.Current.Response.ContentType = "application/zip";
                //    HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                //    zip.Save(HttpContext.Current.Response.OutputStream);
                //    HttpContext.Current.Response.End();
                //}
                #endregion old Code
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public void DownloadFilesData()
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                if (true)
                {                    
                   string mdbPathCor = WebConfigurationManager.AppSettings["mdbPathPunAdd"].ToString();
                   zip.AddFile(mdbPathCor, "DataFile");
                   //zip.AddFile("D:\\PSEBONLINE\\PSEBONLINE\\Download\\Backup.mdb", "DataFile");                 
                }
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.BufferOutput = false;
                string zipName = String.Format("PbiAdd_Data_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                HttpContext.Current.Response.ContentType = "application/zip";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                zip.Save(HttpContext.Current.Response.OutputStream);
                HttpContext.Current.Response.End();
            }
        }
        public void DownloadFilesImage(string Search)
        {
            DataSet resultTmp = new DataSet();
            SqlDataAdapter adTmp = new SqlDataAdapter();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
            {
                SqlCommand cmdTmp = new SqlCommand("InsertTblPunAdd", con);
                cmdTmp.CommandType = CommandType.StoredProcedure;
                cmdTmp.Parameters.AddWithValue("@Search", Search);
                adTmp.SelectCommand = cmdTmp;
                adTmp.Fill(resultTmp);
                con.Open();
                //return resultTmp;
            }
            using (ZipFile zip = new ZipFile())
            {
                zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                if (true)
                {
                    string str1 = string.Empty;
                    foreach (string str in GetListofPhotoSignature())
                    {
                        // fn = Path.GetFileName(str);
                        //ph=Path.GetDirectoryName(str);
                        if (File.Exists(str) == true)
                        {
                            zip.AddFile(str, "ImgandSing");

                        }
                        else
                        {
                            str1 = str1 + " " + str;
                            //Response.Write("Missing Image :" + str);
                        }

                    }                    
                }
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.BufferOutput = false;
                string zipName = String.Format("PbiAdd_Image_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                HttpContext.Current.Response.ContentType = "application/zip";
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + zipName);
                zip.Save(HttpContext.Current.Response.OutputStream);
                HttpContext.Current.Response.End();
            }
        }
        private string[,] GetListofPhotoSignature()
        {
            try
            {
                string ImagePathCor = WebConfigurationManager.AppSettings["ImagePathCor"].ToString();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("select '" + ImagePathCor + "'+Photo_url as Photo,'" + ImagePathCor + "'+Sign_url as Sign from  tblPunAddDataAccess$", con);
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    con.Open();
                    string[,] arrvalues = new string[2, ds.Tables[0].Rows.Count];
                    //loopcounter
                    for (int loopcounter = 0; loopcounter < ds.Tables[0].Rows.Count; loopcounter++)
                    {
                        //assign dataset values to array
                        arrvalues[0, loopcounter] = ds.Tables[0].Rows[loopcounter]["Photo"].ToString();
                        arrvalues[1, loopcounter] = ds.Tables[0].Rows[loopcounter]["Sign"].ToString();
                    }

                    return arrvalues;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #region begin upload Error List
        public string CheckExamMisExcel(DataSet ds)
        {
            string Result = "";
            try
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][0].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string REFNO = ds.Tables[0].Rows[i][0].ToString();
                        Result += "Please check REFNO " + REFNO + " in row " + RowNo + ",  ";

                    }
                }

                //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //{
                //    if (ds.Tables[0].Rows[i][1].ToString().Length < 7 || ds.Tables[0].Rows[i][1].ToString() == "")
                //    {
                //        int RowNo = i + 2;
                //        string schl = ds.Tables[0].Rows[i][1].ToString();
                //        Result += "Please check SCHL " + schl + " in row " + RowNo + ",  ";
                //    }
                //}

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][1].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string errcode = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check ERRCODE " + errcode + " in row " + RowNo + ",  ";
                    }
                }

                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (ds.Tables[0].Rows[i][2].ToString() == "")
                    {
                        int RowNo = i + 2;
                        string errcode = ds.Tables[0].Rows[i][1].ToString();
                        Result += "Please check STATUS " + errcode + " in row " + RowNo + ",  ";
                    }
                }

            }
            catch (Exception ex)
            {
                Result = ex.Message;
                //return "Please Check All Fields";
            }
            return Result;
        }

        public DataTable ExamErrorListMIS(DataTable dt1, int adminid, out int OutStatus)  // ExamErrorListMISSP
        {

            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("tblPunAddErr_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@adminid", adminid);
                    cmd.Parameters.AddWithValue("@TypetblPunAddErr", dt1);
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

        #endregion  upload Error List

        public DataSet AdminPunAddCutList(string Search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //SqlCommand cmd = new SqlCommand("GetAllSchoolsByDistTC_sp", con);
                    SqlCommand cmd = new SqlCommand("GetAdminPunAddCutList_sp", con);
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

        #region PunAdd Admit Card 
        public DataSet PunAddAdmitCardSearch(string Search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PunAddAdmitCardSearch_sp", con);
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
        public DataSet GetPunAddAdmitCard(PunAddModels MS)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetPunAddAdmitCard_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@refno", MS.refNo);
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

        #endregion Admit Card 
        // Signature Chard PunAdd
        public DataSet AdminPunAddSignChart(string Search, string SearchBy)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AdminPunAddSignChart_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@SubCode", SearchBy);
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
        // Confidecial List PunAdd
        public DataSet AdminPunAddConfidentialList(string Search, string SearchBy)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AdminPunAddConfidentialList", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@SubCode", SearchBy);
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
        // Admin PunAdd Result Sheet
        public DataSet AdminPunAddResultSheet(string Search, string SearchBy)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("AdminPunAddResultSheetnw_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    cmd.Parameters.AddWithValue("@set", SearchBy);
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
        #endregion Admin Pannel DB

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
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
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
        public DataSet SelectAllTehsil(int DISTID)
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
        public DataSet SelectForMigrateSchools(string Search)
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
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
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
                    //con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());//
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

        public DataSet PunAddUnlockPage(PunAddModels MS)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PunAddUnlockPage_sp", con);
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
        public DataSet GetDetailTblPunAdd(PunAddModels MS)
        {
            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetDetailTblPunAdd_sp";
                //
                cmd.Parameters.AddWithValue("@refNo", MS.refNo);
                cmd.Parameters.AddWithValue("@oroll", MS.OROLL);
                //
                ds = db.ExecuteDataSet(cmd);
                return ds;
            }
            catch (Exception ex)
            {
                return null;
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
            MonthList.Add(new SelectListItem { Text = "JUN", Value = "JUN" });
            MonthList.Add(new SelectListItem { Text = "JULY", Value = "JULY" });
            MonthList.Add(new SelectListItem { Text = "AUGUST", Value = "AUGUST" });
            MonthList.Add(new SelectListItem { Text = "SEPTEMBER", Value = "SEPTEMBER" });
            MonthList.Add(new SelectListItem { Text = "OCTOBER", Value = "OCTOBER" });
            MonthList.Add(new SelectListItem { Text = "NOVEMBER", Value = "NOVEMBER" });
            MonthList.Add(new SelectListItem { Text = "DECEMBER", Value = "DECEMBER" });
            return MonthList;
        }
        public DataTable CurrentBatchYear()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("getCurrentBatchYearSP", con);
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
        public List<SelectListItem> GetSessionYear1()
        {
            DataTable dsSession = SessionMaster(); // passing Value to SchoolDB from model
            List<SelectListItem> itemSession = new List<SelectListItem>();
            foreach (System.Data.DataRow dr in dsSession.Rows)
            {
                itemSession.Add(new SelectListItem { Text = @dr["yearfrom"].ToString(), Value = @dr["yearfrom"].ToString() });
            }
            return itemSession;
            //List<SelectListItem> itemSession = new List<SelectListItem>();
            //itemSession.Add(new SelectListItem { Text = "1950", Value = "1950" });
            //itemSession.Add(new SelectListItem { Text = "1951", Value = "1951" });
            //itemSession.Add(new SelectListItem { Text = "1952", Value = "1952" });
            //itemSession.Add(new SelectListItem { Text = "1953", Value = "1953" });
            //itemSession.Add(new SelectListItem { Text = "1954", Value = "1954" });
            //itemSession.Add(new SelectListItem { Text = "1955", Value = "1955" });
            //itemSession.Add(new SelectListItem { Text = "1956", Value = "1956" });
            //itemSession.Add(new SelectListItem { Text = "1957", Value = "1957" });
            //itemSession.Add(new SelectListItem { Text = "1958", Value = "1958" });
            //itemSession.Add(new SelectListItem { Text = "1959", Value = "1959" });
            //itemSession.Add(new SelectListItem { Text = "1960", Value = "1960" });  
                      
            //itemSession.Add(new SelectListItem { Text = "1961", Value = "1961" });
            //itemSession.Add(new SelectListItem { Text = "1962", Value = "1962" });
            //itemSession.Add(new SelectListItem { Text = "1963", Value = "1963" });
            //itemSession.Add(new SelectListItem { Text = "1964", Value = "1964" });
            //itemSession.Add(new SelectListItem { Text = "1965", Value = "1965" });
            //itemSession.Add(new SelectListItem { Text = "1966", Value = "1966" });
            //itemSession.Add(new SelectListItem { Text = "1967", Value = "1967" });
            //itemSession.Add(new SelectListItem { Text = "1968", Value = "1968" });
            //itemSession.Add(new SelectListItem { Text = "1969", Value = "1969" });
            //itemSession.Add(new SelectListItem { Text = "1970", Value = "1970" });
            
            //itemSession.Add(new SelectListItem { Text = "1971", Value = "1971" });
            //itemSession.Add(new SelectListItem { Text = "1972", Value = "1972" });
            //itemSession.Add(new SelectListItem { Text = "1973", Value = "1973" });
            //itemSession.Add(new SelectListItem { Text = "1974", Value = "1974" });
            //itemSession.Add(new SelectListItem { Text = "1975", Value = "1975" });
            //itemSession.Add(new SelectListItem { Text = "1976", Value = "1976" });
            //itemSession.Add(new SelectListItem { Text = "1977", Value = "1977" });
            //itemSession.Add(new SelectListItem { Text = "1978", Value = "1978" });
            //itemSession.Add(new SelectListItem { Text = "1979", Value = "1979" });
            //itemSession.Add(new SelectListItem { Text = "1980", Value = "1980" });
            
            //itemSession.Add(new SelectListItem { Text = "1981", Value = "1981" });
            //itemSession.Add(new SelectListItem { Text = "1982", Value = "1982" });
            //itemSession.Add(new SelectListItem { Text = "1983", Value = "1983" });
            //itemSession.Add(new SelectListItem { Text = "1984", Value = "1984" });
            //itemSession.Add(new SelectListItem { Text = "1985", Value = "1985" });
            //itemSession.Add(new SelectListItem { Text = "1986", Value = "1986" });
            //itemSession.Add(new SelectListItem { Text = "1987", Value = "1987" });
            //itemSession.Add(new SelectListItem { Text = "1988", Value = "1988" });
            //itemSession.Add(new SelectListItem { Text = "1989", Value = "1989" });
            //itemSession.Add(new SelectListItem { Text = "1990", Value = "1990" });
            
            //itemSession.Add(new SelectListItem { Text = "1991", Value = "1991" });
            //itemSession.Add(new SelectListItem { Text = "1992", Value = "1992" });
            //itemSession.Add(new SelectListItem { Text = "1993", Value = "1993" });
            //itemSession.Add(new SelectListItem { Text = "1994", Value = "1994" });
            //itemSession.Add(new SelectListItem { Text = "1995", Value = "1995" });
            //itemSession.Add(new SelectListItem { Text = "1996", Value = "1996" });
            //itemSession.Add(new SelectListItem { Text = "1997", Value = "1997" });
            //itemSession.Add(new SelectListItem { Text = "1998", Value = "1998" });
            //itemSession.Add(new SelectListItem { Text = "1999", Value = "1999" });
            //itemSession.Add(new SelectListItem { Text = "2000", Value = "2000" });
            
            //itemSession.Add(new SelectListItem { Text = "2001", Value = "2001" });
            //itemSession.Add(new SelectListItem { Text = "2002", Value = "2002" });
            //itemSession.Add(new SelectListItem { Text = "2003", Value = "2003" });
            //itemSession.Add(new SelectListItem { Text = "2004", Value = "2004" });
            //itemSession.Add(new SelectListItem { Text = "2005", Value = "2005" });
            //itemSession.Add(new SelectListItem { Text = "2006", Value = "2006" });
            //itemSession.Add(new SelectListItem { Text = "2007", Value = "2007" });
            //itemSession.Add(new SelectListItem { Text = "2008", Value = "2008" });
            //itemSession.Add(new SelectListItem { Text = "2009", Value = "2009" });
            //itemSession.Add(new SelectListItem { Text = "2010", Value = "2010" });
            //itemSession.Add(new SelectListItem { Text = "2011", Value = "2011" });
            //itemSession.Add(new SelectListItem { Text = "2012", Value = "2012" });
            //itemSession.Add(new SelectListItem { Text = "2013", Value = "2013" });
            //itemSession.Add(new SelectListItem { Text = "2014", Value = "2014" });
            //itemSession.Add(new SelectListItem { Text = "2015", Value = "2015" });
            //itemSession.Add(new SelectListItem { Text = "2016", Value = "2016" });
            //itemSession.Add(new SelectListItem { Text = "2017", Value = "2017" });
            //itemSession.Add(new SelectListItem { Text = "2018", Value = "2018" });
            //itemSession.Add(new SelectListItem { Text = "2019", Value = "2019" });
            //return itemSession;
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

        public List<SelectListItem> GetDA()
        {
            List<SelectListItem> DList = new List<SelectListItem>();
            DList.Add(new SelectListItem { Text = "N.A.", Value = "N.A." });
            DList.Add(new SelectListItem { Text = "Blind/Visually Impaired", Value = "Blind" });
            //DList.Add(new SelectListItem { Text = "Handicap", Value = "Handicap" });
            DList.Add(new SelectListItem { Text = "Deaf & Dumb/Hearing", Value = "Deaf&Dumb" });
            DList.Add(new SelectListItem { Text = "Others", Value = "Others" });

            return DList;
        }
        public DataSet InsertTblPunAdd(PunAddModels MS)  // Type 1=Regular, 2=Open
        {

            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "InsertTblPunAdd_SP"; 
                             //
                cmd.Parameters.AddWithValue("@batch", MS.batch);
                cmd.Parameters.AddWithValue("@batchYear", MS.batchYear);
                cmd.Parameters.AddWithValue("@Exam_Type", MS.Exam_Type);
                cmd.Parameters.AddWithValue("@category", MS.category);
                cmd.Parameters.AddWithValue("@Class", MS.Class);
                cmd.Parameters.AddWithValue("@Other_Board", MS.Other_Board);
                cmd.Parameters.AddWithValue("@board", MS.Board);
                cmd.Parameters.AddWithValue("@SelYear", MS.SelYear);
                cmd.Parameters.AddWithValue("@SelMonth", MS.SelMonth);
                cmd.Parameters.AddWithValue("@Result", MS.Result);
                cmd.Parameters.AddWithValue("@MatricMarks", MS.MatricMarks);
                cmd.Parameters.AddWithValue("@OROLL", MS.OROLL);
                cmd.Parameters.AddWithValue("@emailID", MS.emailID);
                cmd.Parameters.AddWithValue("@mobileNo", MS.mobileNo);
                //
                ds = db.ExecuteDataSet(cmd);
                return ds;
            }
            catch (Exception ex)
            {
                return null;
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

        public  DataSet GetPunAddConfirmation(string refno)
        {
            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetTblPunAddConfirmation_sp"; //GetAllFormName_sp
                cmd.Parameters.AddWithValue("@refno", refno);
                ds = db.ExecuteDataSet(cmd);              
                return ds;
            }
            catch (Exception ex)
            {                
                return null;
            }

        }
      


        public DataSet GetPrivateCandidateConfirmation(string refno)
        {
            try
            {
                DataSet ds = new DataSet();
                Database db = DatabaseFactory.CreateDatabase("myDBConnection");
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GetTblPunAddConfirmation_sp"; //GetAllFormName_sp
                cmd.Parameters.AddWithValue("@refno", refno);
                ds = db.ExecuteDataSet(cmd);
                return ds;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataSet InsertPrivateCandidateConfirmation(PunAddModels MS)  // Type 1=Regular, 2=Open
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("InsertTblPunAddConfirmation_SP", con);//InsertSMFSP   //InsertSMFSPNew
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Board", MS.Board);
                    cmd.Parameters.AddWithValue("@Other_Board", MS.Other_Board);
                    cmd.Parameters.AddWithValue("@Result", MS.Result);
                    cmd.Parameters.AddWithValue("@MatricMarks", MS.MatricMarks);

                    cmd.Parameters.AddWithValue("@refno", MS.refNo);
                    cmd.Parameters.AddWithValue("@OROLL", MS.OROLL);                                        
                    cmd.Parameters.AddWithValue("@SelExamDist", MS.SelExamDist);

                    cmd.Parameters.AddWithValue("@RegNo", MS.RegNo);
                    cmd.Parameters.AddWithValue("@Candi_Name", MS.Candi_Name);
                    cmd.Parameters.AddWithValue("@Pname", MS.Pname);
                    cmd.Parameters.AddWithValue("@Father_Name", MS.Father_Name);
                    cmd.Parameters.AddWithValue("@PFname", MS.PFname);
                    cmd.Parameters.AddWithValue("@Mother_Name", MS.Mother_Name);
                    cmd.Parameters.AddWithValue("@PMname", MS.PMname);
                    cmd.Parameters.AddWithValue("@DOB", MS.DOB);

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
                    
                    cmd.Parameters.AddWithValue("@Addess", MS.address);
                    cmd.Parameters.AddWithValue("@landmark", MS.landmark);
                    cmd.Parameters.AddWithValue("@block", MS.block);
                    cmd.Parameters.AddWithValue("@SelDist", MS.SelDist);
                    cmd.Parameters.AddWithValue("@tehsil", MS.tehsil);
                    cmd.Parameters.AddWithValue("@pinCode", MS.pinCode);                                        
                    cmd.Parameters.AddWithValue("@cat", MS.category);
                    cmd.Parameters.AddWithValue("@Class", MS.Class);                                        
                    cmd.Parameters.AddWithValue("@PathPhoto", MS.PathPhoto);
                    cmd.Parameters.AddWithValue("@PathSign", MS.PathSign);
                                     
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


        public int EditPunAddConfirmation(PunAddModels MS)  // Type 1=Regular, 2=Open
        {
            int result;
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    // SM.SCHL = SM.id.ToString();
                    SqlCommand cmd = new SqlCommand("EditPunAddConfirmation_SP", con);
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
        
      public DataSet GetPrivateCandidateConfirmationFinalSubmit(string search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetPunAddConfirmationFinalSubmit_sp", con);
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
                    SqlCommand cmd = new SqlCommand("GetPunAddConfirmationPrint_sp", con);
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

        //--------------------------------------Challan-------------


        public DataSet GetPunAddDetailsPayment(string RefNo,string form)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetPunAddDetailsPaymentSP", con);
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
        
        public string InsertPaymentFormPunAdd_OLD(PunAddChallanMasterModel CM, FormCollection frm, out string CandiMobile)
        {
            SqlConnection con = null;
            string result = "";
            try
            {                
                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                SqlCommand cmd = new SqlCommand("InsertPaymentFormSP", con);   //InsertPaymentFormSP//InsertPaymentFormSPTest  // [InsertPaymentFormSP_Rohit]
                cmd.CommandType = CommandType.StoredProcedure;
               //cmd.Parameters.AddWithValue("@batch", CM.batch);
               //cmd.Parameters.AddWithValue("@batchyear", CM.batchyear);

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
                con.Open();
                result= cmd.ExecuteNonQuery().ToString();
               // result = cmd.ExecuteNonQuery().ToString();
                string outuniqueid = (string)cmd.Parameters["@CHALLANID"].Value;
                CandiMobile = (string)cmd.Parameters["@SchoolMobile"].Value;
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

        public string InsertPaymentFormPunAdd(PunAddChallanMasterModel CM, FormCollection frm, out string CandiMobile)
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

        public DataSet ReGenerateChallaanById(string ChallanId, string usertype, out int OutStatus)  // ReGenerateChallaan
        {
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            DataSet dataTable = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("ReGenerateChallaanByIdSPNew", con); //ReGenerateChallaanByIdSP
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
                
        public DataSet GetChallanDetailsById(string ChallanId)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("GetChallanDetailsByIdPunAddSP", con);
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


        public DataSet getForgotPassword(PunAddModels MS)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("getForgotPasswordTblPunAdd_sp", con);
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
public DataSet PunAddResults(string Search)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //SqlCommand cmd = new SqlCommand("AdminPunAddResultSheetnw_SP", con);
                    SqlCommand cmd = new SqlCommand("PunAddResults_SP", con);
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

        #region Punjabi Additional Certificate   
        public DataSet PunAddResultCer(PunAddModels MS)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    SqlCommand cmd = new SqlCommand("PunAddResultCer_SP", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Rollrefno", MS.SearchString);
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
        #endregion Punjabi Additional Certificate   

        //
        #region Admin PunAdd Report
        public DataSet AdminPunAddReport(string Search, string SearchBy)
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //SqlCommand cmd = new SqlCommand("AdminPunAddConfidentialList", con);
                    SqlCommand cmd = new SqlCommand("AdminPunAddReport_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Search", Search);
                    //cmd.Parameters.AddWithValue("@SubCode", SearchBy);
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

        public DataSet AdminPunAddErrorReport()
        {
            DataSet result = new DataSet();
            SqlDataAdapter ad = new SqlDataAdapter();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString()))
                {
                    //SqlCommand cmd = new SqlCommand("AdminPunAddConfidentialList", con);
                    SqlCommand cmd = new SqlCommand("AdminPunAddErrorReport_sp", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    //cmd.Parameters.AddWithValue("@Search", Search);
                    //cmd.Parameters.AddWithValue("@SubCode", SearchBy);
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
        #endregion Admin PunAdd Report
        public DataSet SelectAllCentre(string batch)
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
                    SqlCommand cmd = new SqlCommand("GetPunAddAllCentre", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@batch", batch);

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