//using System;
//using System.Configuration;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Collections;
//using System.Data;
//using System.Data.SqlClient;
//using System.Net.Mail;
//using System.Text;
//using System.Net;
//using System.Drawing;
//using System.IO;
//using System.Drawing.Printing;
//using Microsoft.Reporting.WebForms;
//using iTextSharp.text.pdf;
//using iTextSharp.text;
//using System.Threading;
//using PSEBONLINE.AbstractLayer;

//public partial class Mergerdlc : System.Web.UI.Page
//{
//    //string schlcode = "";
//    //string plot = "";
//    //string pclass = "";
//    //string reportType = "PDF";
//    //string mimeType;
//    //string encoding;
//    //string fileNameExtension = ".pdf";  
//    //// static string addpath = @"D:\Report\";
//    //static string addpath = ConfigurationManager.AppSettings["UploadRDLC"].ToString();//UploadPath
//    //static string addrptpath = "Report/";
//    ////The DeviceInfo settings should be changed based on the reportType
//    ////https://msdn2.microsoft.com/en-us/library/ms155397.aspx
//    //string deviceInfo =
//    //"<DeviceInfo>" +
//    //"  <OutputFormat>PDF</OutputFormat>" +
//    //"  <PageWidth>8.7in</PageWidth>" +
//    //"  <PageHeight>11in</PageHeight>" +
//    //"  <MarginTop>0.0in</MarginTop>" +
//    //"  <MarginLeft>0.0in</MarginLeft>" +
//    //"  <MarginRight>0.0in</MarginRight>" +
//    //"  <MarginBottom>0.0in</MarginBottom>" +
//    //"</DeviceInfo>";

//    //Warning[] warnings;
//    //string[] streams;
//    //byte[] renderedBytes;
//    //byte[] file1;
//    //byte[] file2;
//    //byte[] file3;
//    //byte[] file4;
//    //byte[] file5;
//    //byte[] file6;
//    //byte[] file7;
//    //byte[] file8;
//    //byte[] file9;
//    //byte[] file10;
//    //byte[] file11;
//    //byte[] file12;
//    //byte[] file13;
//    //byte[] file14;
//    //byte[] file15;
//    //byte[] file16;
//    //byte[] file17;
//    //byte[] file18;
//    //byte[] file19;
//    //byte[] file20;
//    //byte[] file21;
//    //byte[] file22;
//    //byte[] file23;
//    //byte[] file24;
//    //byte[] file25;
//    //string rptpath = "";
//    //string qry = "";
//    //string qry1 = "";
//    //PdfReader reader = null;
//    //SqlDataReader sqlrdr = null;
//    //string chkqry1 = "", chkqry2 = "";
//    //DataSet chkds1, chkds2;
//    //ReportDB objRDB = new ReportDB();

//    //protected void Page_Load(object sender, EventArgs e)
//    //{
//    //    if (!IsPostBack)
//    //    {
//    //     //   if (Request.QueryString["dist"] != null)
//    //        {
//    //            DataSet objds = new DataSet();
//    //            string qryregdate = "select distinct top 1 schl,printlotnew as lot,class from exammasterregular2017 where PrintStatus is null and challanverify=1";
//    //            objds = objRDB.GetDataByQuery(qryregdate);
//    //            //,'0043544','0099543','2070246','0054271','0076133','0073881','0080936','2170132' GULAB
//    //            // objds = DataManager2.getDataSet(new SqlCommand("select distinct schl from regmaster where SCHL IN('0026362','0043544','0099543','2070246','0054271','0076133','0073881','0080936','2170132')"));// + Request.QueryString["dist"].ToString() + "' order by dist"));
//    //            //               done objds = DataManager2.getDataSet(new SqlCommand("select distinct schl from regmaster where SCHL IN(SELECT SCHL FROM MISS2) AND FORM IN('M1','M2','M3')"));// + Request.QueryString["dist"].ToString() + "' order by dist"));
//    //            //done   objds = DataManager2.getDataSet(new SqlCommand("select distinct schl from regmaster where SCHL IN(SELECT SCHL FROM ERR1) AND FORM IN('T1','T2','T3')"));// + Request.QueryString["dist"].ToString() + "' order by dist"));
//    //            //objds = DataManager2.getDataSet(new SqlCommand("select  * from miss3"));
//    //            if (objds != null && objds.Tables.Count > 0)
//    //            {
//    //                for (int i = 0; i < objds.Tables[0].Rows.Count; i++)
//    //                {
//    //                    //objds.Tables[0].Rows.Count
//    //                    string DIST = "";
//    //                    string MATBR = "";
//    //                    string SSBR = "";
//    //                    schlcode = objds.Tables[0].Rows[i]["schl"].ToString().Trim();
//    //                    plot  = objds.Tables[0].Rows[i]["lot"].ToString().Trim();
//    //                    pclass = objds.Tables[0].Rows[i]["class"].ToString().Trim();


//    //                    string File10path = "";
//    //                    string File12path = "";
//    //                    string printlot="1";

//    //                    BindReport(schlcode, plot, pclass);
//    //                    // move to ftp
//    //                    //if (File.Exists(addpath + "pdf\\" + MATBR + "/M_" + DIST + "_" + schlcode + ".pdf")) ;
//    //                    //{
//    //                    //    string CompleteDPath = "ftp://43.224.136.122/pdf/";
//    //                    //    string UName = "ftp.demo.com|userweb500";
//    //                    //    string PWD = "gdfsh@#TY123";
//    //                    //    File10path = CompleteDPath + MATBR + "/M_" + DIST + "_" + schlcode + "_lot_" + printlot.ToString() + ".pdf";
//    //                    //    ///pdf/" + MATBR + "/M_" + distcode + "_" + schlcode + ".pdf
//    //                    //    WebRequest reqObj = WebRequest.Create(File10path);
//    //                    //    reqObj.Method = WebRequestMethods.Ftp.UploadFile;
//    //                    //    reqObj.Credentials = new NetworkCredential(UName, PWD);
//    //                    //    FileStream streamObj = System.IO.File.OpenRead(addpath + "pdf\\"  + MATBR + "/M_" + DIST + "_" + schlcode + ".pdf");
//    //                    //    byte[] buffer = new byte[streamObj.Length + 1];
//    //                    //    streamObj.Read(buffer, 0, buffer.Length);
//    //                    //    streamObj.Close();
//    //                    //    streamObj = null;
//    //                    //    reqObj.GetRequestStream().Write(buffer, 0, buffer.Length);
//    //                    //    reqObj = null;
//    //                    //    Thread.Sleep(100);
//    //                    //    File.Delete(addpath + "pdf\\" + MATBR + "/M_" + DIST + "_" + schlcode + ".pdf");

//    //                    //}
//    //                    //if (File.Exists(addpath + "pdf\\" + SSBR + "/S_" + DIST + "_" + schlcode + ".pdf")) ;
//    //                    //{
//    //                    //    string CompleteDPath = "ftp://43.224.136.122/pdf/";
//    //                    //    string UName = "ftp.demo.com|userweb500";
//    //                    //    string PWD = "gdfsh@#TY123";
//    //                    //    File12path = CompleteDPath + SSBR + "/S_" + DIST + "_" + schlcode + "_lot_" + printlot.ToString() + ".pdf";
//    //                    //    WebRequest reqObj = WebRequest.Create(File12path);
//    //                    //    reqObj.Method = WebRequestMethods.Ftp.UploadFile;
//    //                    //    reqObj.Credentials = new NetworkCredential(UName, PWD);
//    //                    //    FileStream streamObj = System.IO.File.OpenRead(addpath + "pdf\\" + SSBR + "/S_" + DIST + "_" + schlcode + ".pdf");
//    //                    //    byte[] buffer = new byte[streamObj.Length + 1];
//    //                    //    streamObj.Read(buffer, 0, buffer.Length);
//    //                    //    streamObj.Close();
//    //                    //    streamObj = null;
//    //                    //    reqObj.GetRequestStream().Write(buffer, 0, buffer.Length);
//    //                    //    reqObj = null;
//    //                    //    Thread.Sleep(100);
//    //                    //    File.Delete(addpath + "pdf\\" + SSBR + "/S_" + DIST + "_" + schlcode + ".pdf");

//    //                    //}
//    //                    //end move
//    //                    string WriteToDb = "update examMasterRegular2017 set [PrintStatus]='Y',INSDATE='" + DateTime.Now.ToString() + "' where schl='" + schlcode + "'";
//    //                    objds = objRDB.GetDataByQuery(WriteToDb);
//    //                   // DataManager2.WriteToDb(new SqlCommand("update pr set [print]='Y',INSDATE='" + DateTime.Now.ToString() + "' where schl='" + schlcode + "'"));
//    //                    Response.Write(schlcode);       
//    //                }

//    //                //Response.Redirect("thanks.aspx?"+schlcode);
//    //            }

//    //        }
//    //    }
//    //}

//    //public void BindReport(string SCHL, string lot1, string pClass)
//    //{
//    //    //if (File.Exists(Server.MapPath("~/pdf/" + MATBR + "/M_" + distcode + "_" + schlcode + ".pdf")))
//    //    //{
//    //    //    File.Delete(Server.MapPath("~/pdf/" + MATBR + "/M_" + distcode + "_" + schlcode + ".pdf"));
//    //    //}

//    //    int pageOffset = 0;
//    //    ArrayList master = new ArrayList();
//    //    int f = 0;
//    //    Document document = null;
//    //    PdfCopy writer = null;
//    //    try
//    //    {
//    //        string SET1 = string.Empty;
//    //        string SET2 = string.Empty;
//    //        // chkds1 = DataManager2.getDataSet(new SqlCommand("select [SET] from regmaster where form in('M1','M2') and SCHL='" + schlcode + "' and LOT<>'0'"));
//    //        // chkds2 = DataManager2.getDataSet(new SqlCommand("select [SET] from regmaster where form in('M3') and SCHL='" + schlcode + "' and LOT<>'0'"));
                             
//    //        SqlDataAdapter ad = new SqlDataAdapter();
//    //        using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString()))
//    //        {
//    //            SqlCommand cmd = new SqlCommand("ExamReportSPFinalSenior", con);//GetCalculateFeeBySchoolSPNew
//    //            cmd.CommandType = CommandType.StoredProcedure;
//    //            cmd.Parameters.AddWithValue("@SCHL", SCHL);
//    //            cmd.Parameters.AddWithValue("@lot", lot1);
//    //            cmd.Parameters.AddWithValue("@Class", pClass);
//    //            ad.SelectCommand = cmd;
//    //            ad.Fill(chkds1);               
//    //            con.Open();
//    //            if (chkds1.Tables[2].Rows.Count > 0)
//    //            {
//    //               // rptpath = "~/RPTReports/ SeniorExamReport.rdlc";
//    //                file1 = RenderReport(rptpath, chkds1);
//    //                reader = new PdfReader(file1);              
//    //            }
//    //        }
//    //    }
//    //    catch (Exception ex)
//    //    {

//    //    }
//    //    finally
//    //    {
//    //        if (document != null)
//    //        {
//    //            document.Close();
//    //            document.Dispose();
//    //        }


//    //    }
//    //}

//    //void SubReportMatricSubject(object sender, SubreportProcessingEventArgs e)
//    //{
//    //    string CanId = e.Parameters["CANDID"].Values[0].ToString();
//    //    DataSet ds = GetSubjectById(CanId);
//    //    ds.Tables[0].TableName = "MatricSubjectDataset";
//    //    ReportDataSource datasource2 = new ReportDataSource("MatricSubjectDataset", ds.Tables[0]);
//    //    e.DataSources.Add(datasource2);
//    //}
//    //private DataSet GetSubjectById(string Canid)
//    //{
//    //    try
//    //    {
//    //        string qry = "SELECT * From tblSeniorSubjects where candid='" + Canid + "' order by SUB_SEQ asc";
//    //        DataSet ds2 = objRDB.GetDataByQuery(qry);
//    //        ds2.Tables[0].TableName = "MatricSubjectDataset";
//    //        return ds2;
//    //    }
//    //    catch
//    //    {
//    //        return null;
//    //    }
//    //}

//    //private byte[] RenderReport(string rptpath, DataSet ds2)
//    //{
//    //    byte[] arr;
//    //    ReportViewer1.Reset();
//    //    //// ReportViewer1.LocalReport.ReportPath = rptpath;
//    //    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/RPTReports/SeniorExamReport.rdlc");
//    //    LocalReport localReport = new LocalReport();
//    //    localReport.ReportPath = Server.MapPath(rptpath);
//    //    //SqlCommand scommand2 = new SqlCommand(qry);
//    //    //DataSet ds2 = DataManager2.getDataSet(scommand2);
//    //    ds2.Tables[0].TableName = "Regdata";
//    //    ReportViewer1.Visible = true;
//    //  //  ReportDataSource datasource2 = new ReportDataSource("Regdata", ds2.Tables[0]);
//    //    ReportDataSource datasource = new ReportDataSource("ExamReportDataSet", ds2.Tables[2]);
//    //    ReportDataSource datasource1 = new ReportDataSource("SubjectwiseCountOfStudent", ds2.Tables[3]);
//    //    ReportDataSource datasource2 = new ReportDataSource("SubjectwiseCountMatricExam", ds2.Tables[4]);
//    //    ReportDataSource datasource4 = new ReportDataSource("ChallanDetails", ds2.Tables[5]);      
//    //    ReportViewer1.LocalReport.EnableExternalImages = true;
//    //    string imagePath = new Uri(Server.MapPath("~/Upload/")).AbsoluteUri;
//    //    ReportParameter parameter = new ReportParameter("ImgPath", imagePath);
//    //    ReportViewer1.LocalReport.SetParameters(parameter);
//    //    ReportViewer1.LocalReport.SubreportProcessing += new Microsoft.Reporting.WebForms.SubreportProcessingEventHandler(SubReportMatricSubject); 
//    //    ReportViewer1.LocalReport.DataSources.Clear();
//    //    ReportViewer1.LocalReport.DataSources.Add(datasource);
//    //    ReportViewer1.LocalReport.DataSources.Add(datasource1);
//    //    ReportViewer1.LocalReport.DataSources.Add(datasource2);     
//    //    ReportViewer1.LocalReport.DataSources.Add(datasource4);
//    //    arr = ReportViewer1.LocalReport.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
//    //    ReportViewer1.LocalReport.Refresh();
//    //    return arr;
//    //    //    RenderReport(Request.QueryString["Schlid"].ToString());

//    //}

//    //private void Export()
//    //{
//    //    //Clear the response stream and write the bytes to the outputstream
//    //    //Set content-disposition to "attachment" so that user is prompted to take an action
//    //    //on the file (open or save)
//    //    //Response.Clear();
//    //    Response.ContentType = mimeType;
//    //    Response.ContentType = "PDF";
//    //    //Response.AddHeader("filename","FormReport.pdf");
//    //    Response.AddHeader("content-disposition", "attachment; filename=FinalReport12." + fileNameExtension);
//    //    Response.BinaryWrite(renderedBytes);
//    //    Response.End();
//    //}

  
//}


////////////////////////////////////