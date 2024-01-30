using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Text;
using System.Net;
using System.Drawing;
using System.IO;
using System.Drawing.Printing;
using Microsoft.Reporting.WebForms;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Threading;
using PSEBONLINE.AbstractLayer;

public partial class AutoGenerateRDLC : System.Web.UI.Page
{
   // string schlcode = "";
   // string reportType = "PDF";
   // string mimeType;
   // string encoding;
   // string fileNameExtension = ".pdf";
   // static string addpath = @"D:\Report\";
   // static string addrptpath = "Report/";
   // //The DeviceInfo settings should be changed based on the reportType
   // //https://msdn2.microsoft.com/en-us/library/ms155397.aspx
   // string deviceInfo =
   // "<DeviceInfo>" +
   // "  <OutputFormat>PDF</OutputFormat>" +
   // "  <PageWidth>8.7in</PageWidth>" +
   // "  <PageHeight>11in</PageHeight>" +
   // "  <MarginTop>0.0in</MarginTop>" +
   // "  <MarginLeft>0.0in</MarginLeft>" +
   // "  <MarginRight>0.0in</MarginRight>" +
   // "  <MarginBottom>0.0in</MarginBottom>" +
   // "</DeviceInfo>";

   // Warning[] warnings;
   // string[] streams;
   // byte[] renderedBytes;
   // byte[] file1;
   // byte[] file2;
   // byte[] file3;
   // byte[] file4;
   // byte[] file5;
   // byte[] file6;
   // byte[] file7;
   // byte[] file8;
   // byte[] file9;
   // byte[] file10;
   // byte[] file11;
   // byte[] file12;
   // byte[] file13;
   // byte[] file14;
   // byte[] file15;
   // byte[] file16;
   // byte[] file17;
   // byte[] file18;
   // byte[] file19;
   // byte[] file20;
   // byte[] file21;
   // byte[] file22;
   // byte[] file23;
   // byte[] file24;
   // byte[] file25;
   // string rptpath = "";
   // string qry = "";
   // string qry1 = "";
   // PdfReader reader = null;// itextsharp
   // SqlDataReader sqlrdr = null;
   // string chkqry1 = "", chkqry2 = "";
   // DataSet chkds1, chkds2;
   //ReportDB objRDB = new ReportDB();

   // protected void Page_Load(object sender, EventArgs e)
   // {
   //     if (!IsPostBack)
   //     {
   //         //   if (Request.QueryString["dist"] != null)
   //         {
   //             DataSet objds = new DataSet();
   //             //,'0043544','0099543','2070246','0054271','0076133','0073881','0080936','2170132' GULAB
   //             string qryregdate = "select top 1 schl from exammasterregular2016 where print is null";
   //             objds = objRDB.GetDataByQuery(qryregdate);

   //             // objds = DataManager2.getDataSet(new SqlCommand("select top 1 schl from exammasterregular2016 where print is null')"));// + Request.QueryString["dist"].ToString() + "' order by dist"));

   //             // objds = DataManager2.getDataSet(new SqlCommand("select distinct schl from regmaster where SCHL IN('0026362','0043544','0099543','2070246','0054271','0076133','0073881','0080936','2170132')"));// + Request.QueryString["dist"].ToString() + "' order by dist"));

   //             //               done objds = DataManager2.getDataSet(new SqlCommand("select distinct schl from regmaster where SCHL IN(SELECT SCHL FROM MISS2) AND FORM IN('M1','M2','M3')"));// + Request.QueryString["dist"].ToString() + "' order by dist"));

   //             //done   objds = DataManager2.getDataSet(new SqlCommand("select distinct schl from regmaster where SCHL IN(SELECT SCHL FROM ERR1) AND FORM IN('T1','T2','T3')"));// + Request.QueryString["dist"].ToString() + "' order by dist"));

   //             //objds = DataManager2.getDataSet(new SqlCommand("select  * from miss3"));

   //             if (objds != null && objds.Tables.Count > 0)
   //             {
   //                 for (int i = 0; i < objds.Tables[0].Rows.Count; i++)
   //                 {
   //                     //objds.Tables[0].Rows.Count
   //                     string DIST = "";
   //                     string MATBR = "";
   //                     string SSBR = "";
   //                     schlcode = objds.Tables[0].Rows[i]["schl"].ToString().Trim();
   //                     //schlcode = "0010013";
   //                     //string sqlrdr1 = "Select distmaster.DIST,distmaster.MATBR,distmaster.SSBR from schoolmaster,distmaster where schoolmaster.SCHL='" + schlcode + "' and schoolmaster.DIST=distmaster.DIST";
   //                     //sqlrdr = objRDB.GetDataByQuery(sqlrdr1);
   //                     sqlrdr = DataManager2.GetReader(new SqlCommand("Select distmaster.DIST,distmaster.MATBR,distmaster.SSBR from schoolmaster,distmaster where schoolmaster.SCHL='" + schlcode + "' and schoolmaster.DIST=distmaster.DIST"));
   //                     if (sqlrdr.HasRows)
   //                     {
   //                         while (sqlrdr.Read())
   //                         {
   //                             DIST = sqlrdr["DIST"].ToString();
   //                             MATBR = sqlrdr["MATBR"].ToString();
   //                             SSBR = sqlrdr["SSBR"].ToString();
   //                         }
   //                     }

   //                     classs10(schlcode, DIST, MATBR, SSBR);
   //                     classs12(schlcode, DIST, MATBR, SSBR);
   //                     string File10path = "";
   //                     string File12path = "";
   //                     string printlot = "1";
   //                     // move to ftp
   //                     if (File.Exists(addpath + "pdf\\" + MATBR + "/M_" + DIST + "_" + schlcode + ".pdf")) ;
   //                     {
   //                         string CompleteDPath = "ftp://43.224.136.122/pdf/";
   //                         string UName = "ftp.demo.com|userweb500";
   //                         string PWD = "gdfsh@#TY123";
   //                         File10path = CompleteDPath + MATBR + "/M_" + DIST + "_" + schlcode + "_lot_" + printlot.ToString() + ".pdf";
   //                         ///pdf/" + MATBR + "/M_" + distcode + "_" + schlcode + ".pdf
   //                         WebRequest reqObj = WebRequest.Create(File10path);
   //                         reqObj.Method = WebRequestMethods.Ftp.UploadFile;
   //                         reqObj.Credentials = new NetworkCredential(UName, PWD);
   //                         FileStream streamObj = System.IO.File.OpenRead(addpath + "pdf\\" + MATBR + "/M_" + DIST + "_" + schlcode + ".pdf");
   //                         byte[] buffer = new byte[streamObj.Length + 1];
   //                         streamObj.Read(buffer, 0, buffer.Length);
   //                         streamObj.Close();
   //                         streamObj = null;
   //                         reqObj.GetRequestStream().Write(buffer, 0, buffer.Length);
   //                         reqObj = null;
   //                         Thread.Sleep(100);
   //                         File.Delete(addpath + "pdf\\" + MATBR + "/M_" + DIST + "_" + schlcode + ".pdf");

   //                     }
   //                     if (File.Exists(addpath + "pdf\\" + SSBR + "/S_" + DIST + "_" + schlcode + ".pdf")) ;
   //                     {
   //                         string CompleteDPath = "ftp://43.224.136.122/pdf/";
   //                         string UName = "ftp.demo.com|userweb500";
   //                         string PWD = "gdfsh@#TY123";
   //                         File12path = CompleteDPath + SSBR + "/S_" + DIST + "_" + schlcode + "_lot_" + printlot.ToString() + ".pdf";
   //                         WebRequest reqObj = WebRequest.Create(File12path);
   //                         reqObj.Method = WebRequestMethods.Ftp.UploadFile;
   //                         reqObj.Credentials = new NetworkCredential(UName, PWD);
   //                         FileStream streamObj = System.IO.File.OpenRead(addpath + "pdf\\" + SSBR + "/S_" + DIST + "_" + schlcode + ".pdf");
   //                         byte[] buffer = new byte[streamObj.Length + 1];
   //                         streamObj.Read(buffer, 0, buffer.Length);
   //                         streamObj.Close();
   //                         streamObj = null;
   //                         reqObj.GetRequestStream().Write(buffer, 0, buffer.Length);
   //                         reqObj = null;
   //                         Thread.Sleep(100);
   //                         File.Delete(addpath + "pdf\\" + SSBR + "/S_" + DIST + "_" + schlcode + ".pdf");

   //                     }
   //                     //end move
   //                     //DataManager2.WriteToDb(new SqlCommand("update pr set [print]='Y',INSDATE='" + DateTime.Now.ToString() + "' where schl='" + schlcode + "'"));
   //                     Response.Write(schlcode);
   //                 }

   //                 //Response.Redirect("thanks.aspx?"+schlcode);
   //             }

   //         }
   //     }
   // }


   // // main use
   // private byte[] RenderReport(string rptpath, string qry)
   // {
   //     byte[] arr;
   //     ReportViewer1.Reset();
   //     ReportViewer1.LocalReport.ReportPath = rptpath;
   //     LocalReport localReport = new LocalReport();
   //     localReport.ReportPath = Server.MapPath(rptpath);
   //     SqlCommand scommand2 = new SqlCommand(qry);
   //     DataSet ds2 = DataManager2.getDataSet(scommand2);
   //     ds2.Tables[0].TableName = "Regdata";
   //     ReportViewer1.Visible = true;
   //     ReportDataSource datasource2 = new ReportDataSource("Regdata", ds2.Tables[0]);
   //     ReportViewer1.LocalReport.DataSources.Clear();
   //     ReportViewer1.LocalReport.DataSources.Add(datasource2);
   //     arr = ReportViewer1.LocalReport.Render(reportType, deviceInfo, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
   //     ReportViewer1.LocalReport.Refresh();
   //     return arr;
   //     //    RenderReport(Request.QueryString["Schlid"].ToString());

   // }
   // private void Export() // main use
   // {
   //     //Clear the response stream and write the bytes to the outputstream
   //     //Set content-disposition to "attachment" so that user is prompted to take an action
   //     //on the file (open or save)
   //     //Response.Clear();
   //     Response.ContentType = mimeType;
   //     Response.ContentType = "PDF";
   //     //Response.AddHeader("filename","FormReport.pdf");
   //     Response.AddHeader("content-disposition", "attachment; filename=ExamformXIIyyyyyyyyyyyyy." + fileNameExtension);
   //     Response.BinaryWrite(renderedBytes);
   //     Response.End();
   // }


   // public void classs10(string schlcode, string distcode, string MATBR, string SSBR)
   // {
   //     if (File.Exists(Server.MapPath("~/pdf/" + MATBR + "/M_" + distcode + "_" + schlcode + ".pdf")))
   //     {
   //         File.Delete(Server.MapPath("~/pdf/" + MATBR + "/M_" + distcode + "_" + schlcode + ".pdf"));
   //     }

   //     int pageOffset = 0;
   //     ArrayList master = new ArrayList();
   //     int f = 0;
   //     Document document = null;
   //     PdfCopy writer = null;
   //     try
   //     {
   //         string SET1 = string.Empty;
   //         string SET2 = string.Empty;
   //         chkds1 = DataManager2.getDataSet(new SqlCommand("select [SET] from regmaster where form in('M1','M2') and SCHL='" + schlcode + "' and LOT<>'0'"));
   //         chkds2 = DataManager2.getDataSet(new SqlCommand("select [SET] from regmaster where form in('M3') and SCHL='" + schlcode + "' and LOT<>'0'"));
   //         if (chkds1.Tables[0].Rows.Count > 0)
   //         {
   //             for (int j = 1; j <= 7; j++)
   //             {

   //                 SET1 = chkds1.Tables[0].Rows[0]["SET"].ToString().Trim();
   //                 if (j == 1)
   //                 {
   //                     rptpath = "ForwardingReport10.rdlc";
   //                     qry = "select Sub1,NAME_ENG as name,tot from MatricSubMast inner join ("
   //                                                     + "SELECT Sub1,COUNT(*) as tot "
   //                                                       + "FROM (SELECT sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code, addnlsubcode, addnlsubcode2 "
   //                                                       + "FROM regMaster inner join SubjectRecord on "
   //                                                       + "regMaster.id=SubjectRecord.id and schl='" + schlcode + "' and LOT<>'0' and FORM in('M1','M2')) P "
   //                                                       + "UNPIVOT (Sub1 FOR SUB IN (sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code,addnlsubcode, addnlsubcode2))AS Unpvt "
   //                                                       + "WHERE NULLIF(Sub1,'') IS NOT NULL GROUP BY Sub1 "
   //                                                       + ") as tbl on MatricSubMast.SUB=tbl.Sub1";

   //                     qry1 = "select Min(CONVERT(INT,formno)) as YEAR,MAX(CONVERT(INT,formno)) as REGDATE,Count(*) REGFEE from regMaster WHERE schl='" + schlcode + "' and FORM IN('M1','M2') and LOT<>'0'";
   //                     //file1 = RenderReport4(rptpath, qry, qry1, SET1);
   //                     file1 = RenderReport(rptpath, qry);
   //                     reader = new PdfReader(file1);
   //                 }


   //                 reader.ConsolidateNamedDestinations();
   //                 // we retrieve the total number of pages
   //                 int n = reader.NumberOfPages;
   //                 pageOffset += n;

   //                 if (f == 0)
   //                 {
   //                     // step 1: creation of a document-object
   //                     document = new Document(reader.GetPageSizeWithRotation(1));
   //                     // step 2: we create a writer that listens to the document
   //                     writer = new PdfCopy(document, new FileStream(Server.MapPath("~/pdf/" + MATBR + "/M_" + distcode + "_" + schlcode + ".pdf"), FileMode.Create));
   //                     // step 3: we open the document
   //                     document.Open();
   //                 }
   //                 // step 4: we add content
   //                 for (int i = 0; i < n;)
   //                 {
   //                     ++i;
   //                     if (writer != null)
   //                     {
   //                         PdfImportedPage page = writer.GetImportedPage(reader, i);
   //                         writer.AddPage(page);
   //                     }
   //                 }
   //                 PRAcroForm form = reader.AcroForm;
   //                 if (form != null && writer != null)
   //                 {
   //                     writer.CopyAcroForm(reader);
   //                 }
   //                 f++;
   //             }
   //         }
   //         if (chkds2.Tables[0].Rows.Count > 0)
   //         {
   //             for (int j = 8; j <= 13; j++)
   //             {
   //                 SET2 = chkds2.Tables[0].Rows[0]["SET"].ToString().Trim();
   //                 if (j == 8)
   //                 {
   //                     rptpath = "ForwardingReport10OS.rdlc";
   //                     qry = "select Sub1,NAME_ENG as name,tot from MatricSubMast inner join ("
   //                                                     + "SELECT Sub1,COUNT(*) as tot "
   //                                                       + "FROM (SELECT sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code, addnlsubcode, addnlsubcode2 "
   //                                                       + "FROM regMaster inner join SubjectRecord on "
   //                                                       + "regMaster.id=SubjectRecord.id and schl='" + schlcode + "' and LOT<>'0' and FORM in('M3')) P "
   //                                                       + "UNPIVOT (Sub1 FOR SUB IN (sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code,addnlsubcode, addnlsubcode2))AS Unpvt "
   //                                                       + "WHERE NULLIF(Sub1,'') IS NOT NULL GROUP BY Sub1 "
   //                                                       + ") as tbl on MatricSubMast.SUB=tbl.Sub1";
   //                     qry1 = "select Min(CONVERT(INT,formno)) as YEAR,MAX(CONVERT(INT,formno)) as REGDATE,Count(*) REGFEE from regMaster WHERE schl='" + schlcode + "' and lot<>'0' and FORM IN('M3')";
   //                     file8 = RenderReport4(rptpath, qry, qry1, SET2);
   //                     reader = new PdfReader(file8);
   //                 }

   //                 if (j == 9)
   //                 {
   //                     rptpath = "SubjectWiseReportXOpen.rdlc";
   //                     qry = "select Sub1,NAME_ENG as name,tot,'" + SET2 + "' as [SET]  from MatricSubMast inner join ("
   //                                                     + "SELECT Sub1,COUNT(*) as tot "
   //                                                       + "FROM (SELECT sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code, addnlsubcode, addnlsubcode2 "
   //                                                       + "FROM regMaster inner join SubjectRecord on "
   //                                                       + "regMaster.id=SubjectRecord.id and schl='" + schlcode + "' and LOT<>'0' and FORM in('M3')) P "
   //                                                       + "UNPIVOT (Sub1 FOR SUB IN (sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code,addnlsubcode, addnlsubcode2))AS Unpvt "
   //                                                       + "WHERE NULLIF(Sub1,'') IS NOT NULL GROUP BY Sub1 "
   //                                                       + ") as tbl on MatricSubMast.SUB=tbl.Sub1";
   //                     string frm = "'M3'";
   //                     file9 = RenderReport3(rptpath, qry, frm);
   //                     reader = new PdfReader(file9);
   //                 }

   //                 if (j == 10)
   //                 {
   //                     rptpath = "FeedetailReportXOpen.rdlc";
   //                     qry = "SELECT * From regmaster inner join subjectrecord on regmaster.id=subjectrecord.id AND regmaster.form in ('M3') inner join tblschuser on regmaster.schl=tblschuser.schl inner join schoolmaster on tblschuser.schl=schoolmaster.schl and regmaster.schl='" + schlcode + "' and LOT<>'0' order by subjectrecord.stream,regmaster.sex,regmaster.name";
   //                     file10 = RenderReport(rptpath, qry);
   //                     reader = new PdfReader(file10);
   //                 }

   //                 if (j == 11)
   //                 {

   //                     rptpath = "ExamFormReportXOpen.rdlc";
   //                     qry = "SELECT * From regmaster inner join subjectrecord on regmaster.id=subjectrecord.id AND regmaster.form in ('M3') inner join tblschuser on regmaster.schl=tblschuser.schl inner join schoolmaster on tblschuser.schl=schoolmaster.schl and regmaster.schl='" + schlcode + "' and LOT<>'0' order by subjectrecord.stream,regmaster.sex,regmaster.name";
   //                     file11 = RenderReport(rptpath, qry);
   //                     reader = new PdfReader(file11);
   //                 }

   //                 if (j == 12)
   //                 {
   //                     rptpath = "CorrectionPerformaReportXOpen.rdlc";
   //                     qry = "select Sub1,NAME_ENG as name,tot from MatricSubMast inner join ("
   //                                                     + "SELECT Sub1,COUNT(*) as tot "
   //                                                       + "FROM (SELECT sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code, addnlsubcode, addnlsubcode2 "
   //                                                       + "FROM regMaster inner join SubjectRecord on "
   //                                                       + "regMaster.id=SubjectRecord.id and schl='" + schlcode + "' and LOT<>'0' and FORM in('M3')) P "
   //                                                       + "UNPIVOT (Sub1 FOR SUB IN (sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code,addnlsubcode, addnlsubcode2))AS Unpvt "
   //                                                       + "WHERE NULLIF(Sub1,'') IS NOT NULL GROUP BY Sub1 "
   //                                                       + ") as tbl on MatricSubMast.SUB=tbl.Sub1";
   //                     file12 = RenderReport2(rptpath, qry, 14, MATBR, SSBR, "10", SET2);
   //                     reader = new PdfReader(file12);
   //                 }


   //                 if (j == 13)
   //                 {
   //                     rptpath = "PhotoReportXOpen.rdlc";
   //                     qry = "SELECT * From regmaster inner join subjectrecord on regmaster.id=subjectrecord.id AND regmaster.form in ('M3') inner join tblschuser on regmaster.schl=tblschuser.schl inner join schoolmaster on tblschuser.schl=schoolmaster.schl and regmaster.schl='" + schlcode + "' and LOT<>'0' order by subjectrecord.stream,regmaster.sex,regmaster.name";
   //                     file13 = RenderReport(rptpath, qry);
   //                     reader = new PdfReader(file13);
   //                 }

   //                 reader.ConsolidateNamedDestinations();
   //                 // we retrieve the total number of pages
   //                 int n = reader.NumberOfPages;
   //                 pageOffset += n;

   //                 if (f == 0)
   //                 {
   //                     // step 1: creation of a document-object
   //                     document = new Document(reader.GetPageSizeWithRotation(1));
   //                     // step 2: we create a writer that listens to the document
   //                     writer = new PdfCopy(document, new FileStream(Server.MapPath("~/pdf/" + MATBR + "/M_" + distcode + "_" + schlcode + ".pdf"), FileMode.Create));
   //                     // step 3: we open the document
   //                     document.Open();
   //                 }
   //                 // step 4: we add content
   //                 for (int i = 0; i < n;)
   //                 {
   //                     ++i;
   //                     if (writer != null)
   //                     {
   //                         PdfImportedPage page = writer.GetImportedPage(reader, i);
   //                         writer.AddPage(page);
   //                     }
   //                 }
   //                 PRAcroForm form = reader.AcroForm;
   //                 if (form != null && writer != null)
   //                 {
   //                     writer.CopyAcroForm(reader);
   //                 }
   //                 f++;

   //             }
   //         }

   //     }
   //     catch (Exception ex)
   //     {

   //     }
   //     finally
   //     {
   //         if (document != null)
   //         {
   //             document.Close();
   //             document.Dispose();
   //         }


   //     }
   // }


   // public void classs12(string schlcode, string distcode, string MATBR, string SSBR)
   // {

   //     if (File.Exists(Server.MapPath("~/pdf/" + SSBR + "/S_" + distcode + "_" + schlcode + ".pdf")))
   //     {
   //         File.Delete(Server.MapPath("~/pdf/" + SSBR + "/S_" + distcode + "_" + schlcode + ".pdf"));
   //     }

   //     int pageOffset = 0;
   //     ArrayList master = new ArrayList();
   //     int f = 0;
   //     Document document = null;
   //     PdfCopy writer = null;
   //     try
   //     {
   //         string SET1 = string.Empty;
   //         string SET2 = string.Empty;
   //         chkds1 = DataManager2.getDataSet(new SqlCommand("select [SET] from regmaster where form in('T1','T2') and SCHL='" + schlcode + "' and LOT<>'0'"));
   //         chkds2 = DataManager2.getDataSet(new SqlCommand("select [SET] from regmaster where form in('T3') and SCHL='" + schlcode + "' and LOT<>'0'"));

   //         if (chkds1.Tables[0].Rows.Count > 0)
   //         {
   //             for (int j = 1; j <= 6; j++)
   //             {

   //                 SET1 = chkds1.Tables[0].Rows[0]["SET"].ToString().Trim();
   //                 if (j == 1)
   //                 {
   //                     rptpath = "ForwardingReport12.rdlc";
   //                     qry = "select Sub1,NAME_ENG as name,tot from SrSecSubMast inner join ("
   //                                                     + "SELECT Sub1,COUNT(*) as tot "
   //                                                       + "FROM (SELECT sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code, addnlsubcode, addnlsubcode2 "
   //                                                       + "FROM regMaster inner join SubjectRecord on "
   //                                                       + "regMaster.id=SubjectRecord.id and schl='" + schlcode + "' and LOT<>'0' and FORM in('T1','T2')) P "
   //                                                       + "UNPIVOT (Sub1 FOR SUB IN (sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code,addnlsubcode, addnlsubcode2))AS Unpvt "
   //                                                       + "WHERE NULLIF(Sub1,'') IS NOT NULL GROUP BY Sub1 "
   //                                                       + ") as tbl on SrSecSubMast.SUB=tbl.Sub1";
   //                     qry1 = "select Min(CONVERT(INT,formno)) as YEAR,MAX(CONVERT(INT,formno)) as REGDATE,EXAM,Count(*) REGFEE from regMaster WHERE schl='" + schlcode + "' and FORM IN('T1','T2') and lot<>'0' group by EXAM";
   //                     file1 = RenderReport4(rptpath, qry, qry1, SET1);
   //                     reader = new PdfReader(file1);
   //                 }
   //                 if (j == 2)
   //                 {
   //                     rptpath = "SubjectWiseReport.rdlc";
   //                     qry = "select Sub1,NAME_ENG as name,tot,'" + SET1 + "' as [SET]  from SrSecSubMast inner join ("
   //                                                     + "SELECT Sub1,COUNT(*) as tot "
   //                                                       + "FROM (SELECT sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code, addnlsubcode, addnlsubcode2 "
   //                                                       + "FROM regMaster inner join SubjectRecord on "
   //                                                       + "regMaster.id=SubjectRecord.id and schl='" + schlcode + "' and LOT<>'0' and FORM in('T1','T2')) P "
   //                                                       + "UNPIVOT (Sub1 FOR SUB IN (sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code,addnlsubcode, addnlsubcode2))AS Unpvt "
   //                                                       + "WHERE NULLIF(Sub1,'') IS NOT NULL GROUP BY Sub1 "
   //                                                       + ") as tbl on SrSecSubMast.SUB=tbl.Sub1";
   //                     string frm = "'T1','T2'";
   //                     file2 = RenderReport3(rptpath, qry, frm);
   //                     reader = new PdfReader(file2);
   //                 }
   //                 if (j == 3)
   //                 {

   //                     rptpath = "FeedetailReport.rdlc";
   //                     qry = "SELECT * From regmaster inner join subjectrecord on regmaster.id=subjectrecord.id AND regmaster.form in ('T1','T2') inner join tblschuser on regmaster.schl=tblschuser.schl inner join schoolmaster on tblschuser.schl=schoolmaster.schl and regmaster.schl='" + schlcode + "' and LOT<>'0' order by subjectrecord.stream,regmaster.sex,regmaster.name";
   //                     file3 = RenderReport(rptpath, qry);
   //                     reader = new PdfReader(file3);
   //                 }
   //                 if (j == 4)
   //                 {

   //                     rptpath = "ExamFormReportXII.rdlc";
   //                     qry = "SELECT * From regmaster inner join subjectrecord on regmaster.id=subjectrecord.id AND regmaster.form in ('T1','T2') inner join tblschuser on regmaster.schl=tblschuser.schl inner join schoolmaster on tblschuser.schl=schoolmaster.schl and regmaster.schl='" + schlcode + "' and LOT<>'0' order by subjectrecord.stream,regmaster.sex,regmaster.name";
   //                     file4 = RenderReport(rptpath, qry);
   //                     reader = new PdfReader(file4);
   //                 }
   //                 if (j == 5)
   //                 {
   //                     rptpath = "CorrectionPerformaReport.rdlc";
   //                     qry = "select Sub1,NAME_ENG as name,tot from SrSecSubMast inner join ("
   //                                                     + "SELECT Sub1,COUNT(*) as tot "
   //                                                       + "FROM (SELECT sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code, addnlsubcode, addnlsubcode2 "
   //                                                       + "FROM regMaster inner join SubjectRecord on "
   //                                                       + "regMaster.id=SubjectRecord.id and schl='" + schlcode + "' and LOT<>'0' and FORM in('T1','T2')) P "
   //                                                       + "UNPIVOT (Sub1 FOR SUB IN (sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code,addnlsubcode, addnlsubcode2))AS Unpvt "
   //                                                       + "WHERE NULLIF(Sub1,'') IS NOT NULL GROUP BY Sub1 "
   //                                                       + ") as tbl on SrSecSubMast.SUB=tbl.Sub1";
   //                     file5 = RenderReport2(rptpath, qry, 14, MATBR, SSBR, "12", SET1);
   //                     reader = new PdfReader(file5);
   //                 }
   //                 if (j == 6)
   //                 {
   //                     rptpath = "PhotoReport.rdlc";
   //                     qry = "SELECT * From regmaster inner join subjectrecord on regmaster.id=subjectrecord.id AND regmaster.form in ('T1','T2') inner join tblschuser on regmaster.schl=tblschuser.schl inner join schoolmaster on tblschuser.schl=schoolmaster.schl and regmaster.schl='" + schlcode + "' and LOT<>'0' order by subjectrecord.stream,regmaster.sex,regmaster.name";
   //                     file6 = RenderReport(rptpath, qry);
   //                     reader = new PdfReader(file6);
   //                 }
   //                 reader.ConsolidateNamedDestinations();
   //                 // we retrieve the total number of pages
   //                 int n = reader.NumberOfPages;
   //                 pageOffset += n;

   //                 if (f == 0)
   //                 {
   //                     // step 1: creation of a document-object
   //                     document = new Document(reader.GetPageSizeWithRotation(1));
   //                     // step 2: we create a writer that listens to the document
   //                     writer = new PdfCopy(document, new FileStream(Server.MapPath("~/pdf/" + SSBR + "/S_" + distcode + "_" + schlcode + ".pdf"), FileMode.Create));
   //                     // step 3: we open the document
   //                     document.Open();
   //                 }
   //                 // step 4: we add content
   //                 for (int i = 0; i < n;)
   //                 {
   //                     ++i;
   //                     if (writer != null)
   //                     {
   //                         PdfImportedPage page = writer.GetImportedPage(reader, i);
   //                         writer.AddPage(page);
   //                     }
   //                 }
   //                 PRAcroForm form = reader.AcroForm;
   //                 if (form != null && writer != null)
   //                 {
   //                     writer.CopyAcroForm(reader);
   //                 }
   //                 f++;
   //             }
   //         }

   //         if (chkds2.Tables[0].Rows.Count > 0)
   //         {
   //             for (int j = 7; j <= 12; j++)
   //             {
   //                 SET2 = chkds2.Tables[0].Rows[0]["SET"].ToString().Trim();
   //                 if (j == 7)
   //                 {
   //                     rptpath = "ForwardingReport12OS.rdlc";
   //                     qry = "select Sub1,NAME_ENG as name,tot from SrSecSubMast inner join ("
   //                                                     + "SELECT Sub1,COUNT(*) as tot "
   //                                                       + "FROM (SELECT sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code, addnlsubcode, addnlsubcode2 "
   //                                                       + "FROM regMaster inner join SubjectRecord on "
   //                                                       + "regMaster.id=SubjectRecord.id and schl='" + schlcode + "' and LOT<>'0' and FORM in('T3')) P "
   //                                                       + "UNPIVOT (Sub1 FOR SUB IN (sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code,addnlsubcode, addnlsubcode2))AS Unpvt "
   //                                                       + "WHERE NULLIF(Sub1,'') IS NOT NULL GROUP BY Sub1 "
   //                                                       + ") as tbl on SrSecSubMast.SUB=tbl.Sub1";
   //                     qry1 = "select Min(CONVERT(INT,formno)) as YEAR,MAX(CONVERT(INT,formno)) as REGDATE,EXAM,Count(*) REGFEE from regMaster WHERE schl='" + schlcode + "' and LOT<>'0' and FORM IN('T3') group by EXAM";
   //                     file7 = RenderReport4(rptpath, qry, qry1, SET2);
   //                     reader = new PdfReader(file7);
   //                 }

   //                 if (j == 8)
   //                 {
   //                     rptpath = "SubjectWiseReportOpen.rdlc";
   //                     qry = "select Sub1,NAME_ENG as name,tot,'" + SET2 + "' as [SET]  from SrSecSubMast inner join ("
   //                                                     + "SELECT Sub1,COUNT(*) as tot "
   //                                                       + "FROM (SELECT sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code, addnlsubcode, addnlsubcode2 "
   //                                                       + "FROM regMaster inner join SubjectRecord on "
   //                                                       + "regMaster.id=SubjectRecord.id and schl='" + schlcode + "' and LOT<>'0' and FORM in('T3')) P "
   //                                                       + "UNPIVOT (Sub1 FOR SUB IN (sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code,addnlsubcode, addnlsubcode2))AS Unpvt "
   //                                                       + "WHERE NULLIF(Sub1,'') IS NOT NULL GROUP BY Sub1 "
   //                                                       + ") as tbl on SrSecSubMast.SUB=tbl.Sub1";
   //                     string frm = "'T3'";
   //                     file8 = RenderReport3(rptpath, qry, frm);
   //                     reader = new PdfReader(file8);
   //                 }
   //                 if (j == 9)
   //                 {
   //                     rptpath = "FeedetailReportOpen.rdlc";
   //                     qry = "SELECT * From regmaster inner join subjectrecord on regmaster.id=subjectrecord.id AND regmaster.form in ('T3') inner join tblschuser on regmaster.schl=tblschuser.schl inner join schoolmaster on tblschuser.schl=schoolmaster.schl and regmaster.schl='" + schlcode + "' and LOT<>'0' order by subjectrecord.stream,regmaster.sex,regmaster.name";
   //                     file9 = RenderReport(rptpath, qry);
   //                     reader = new PdfReader(file9);
   //                 }
   //                 if (j == 10)
   //                 {

   //                     rptpath = "ExamFormReportXIIOpen.rdlc";
   //                     qry = "SELECT * From regmaster inner join subjectrecord on regmaster.id=subjectrecord.id AND regmaster.form in ('T3') inner join tblschuser on regmaster.schl=tblschuser.schl inner join schoolmaster on tblschuser.schl=schoolmaster.schl and regmaster.schl='" + schlcode + "' and LOT<>'0' order by subjectrecord.stream,regmaster.sex,regmaster.name";
   //                     file10 = RenderReport(rptpath, qry);
   //                     reader = new PdfReader(file10);
   //                 }

   //                 if (j == 11)
   //                 {
   //                     rptpath = "CorrectionPerformaReportOpen.rdlc";
   //                     qry = "select Sub1,NAME_ENG as name,tot from SrSecSubMast inner join ("
   //                                                     + "SELECT Sub1,COUNT(*) as tot "
   //                                                       + "FROM (SELECT sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code, addnlsubcode, addnlsubcode2 "
   //                                                       + "FROM regMaster inner join SubjectRecord on "
   //                                                       + "regMaster.id=SubjectRecord.id and schl='" + schlcode + "' and LOT<>'0' and FORM in('T3')) P "
   //                                                       + "UNPIVOT (Sub1 FOR SUB IN (sub1code, sub2code, sub3code, sub4code, sub5code, sub6code, sub7code, sub8code, sub9code,addnlsubcode, addnlsubcode2))AS Unpvt "
   //                                                       + "WHERE NULLIF(Sub1,'') IS NOT NULL GROUP BY Sub1 "
   //                                                       + ") as tbl on SrSecSubMast.SUB=tbl.Sub1";
   //                     file11 = RenderReport2(rptpath, qry, 14, MATBR, SSBR, "12", SET2);
   //                     reader = new PdfReader(file11);
   //                 }

   //                 if (j == 12)
   //                 {
   //                     rptpath = "PhotoReportOpen.rdlc";
   //                     qry = "SELECT * From regmaster inner join subjectrecord on regmaster.id=subjectrecord.id AND regmaster.form in ('T3') inner join tblschuser on regmaster.schl=tblschuser.schl inner join schoolmaster on tblschuser.schl=schoolmaster.schl and regmaster.schl='" + schlcode + "' and LOT<>'0' order by subjectrecord.stream,regmaster.sex,regmaster.name";
   //                     file12 = RenderReport(rptpath, qry);
   //                     reader = new PdfReader(file12);
   //                 }
   //                 reader.ConsolidateNamedDestinations();
   //                 // we retrieve the total number of pages
   //                 int n = reader.NumberOfPages;
   //                 pageOffset += n;

   //                 if (f == 0)
   //                 {
   //                     // step 1: creation of a document-object
   //                     document = new Document(reader.GetPageSizeWithRotation(1));
   //                     // step 2: we create a writer that listens to the document
   //                     writer = new PdfCopy(document, new FileStream(Server.MapPath("~/pdf/" + SSBR + "/S_" + distcode + "_" + schlcode + ".pdf"), FileMode.Create));
   //                     // step 3: we open the document
   //                     document.Open();
   //                 }
   //                 // step 4: we add content
   //                 for (int i = 0; i < n;)
   //                 {
   //                     ++i;
   //                     if (writer != null)
   //                     {
   //                         PdfImportedPage page = writer.GetImportedPage(reader, i);
   //                         writer.AddPage(page);
   //                     }
   //                 }
   //                 PRAcroForm form = reader.AcroForm;
   //                 if (form != null && writer != null)
   //                 {
   //                     writer.CopyAcroForm(reader);
   //                 }
   //                 f++;
   //             }
   //         }

   //     }
   //     catch (Exception ex)
   //     {

   //     }
   //     finally
   //     {
   //         if (document != null)
   //         {
   //             document.Close();
   //             document.Dispose();
   //         }
   //     }
   // }


}


//////////////////////////////////