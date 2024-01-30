using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Drawing.Printing;
using Microsoft.Reporting.WebForms;
using System.Configuration;
using PSEBONLINE;
using PSEBONLINE.AbstractLayer;
public partial class Finalprint : System.Web.UI.Page
{
    Hashtable objHT = new Hashtable();
// somecommonfunctions obj = new somecommonfunctions("pseb2014");
    string uid = string.Empty;
    Int32 lot = 0;
    string form = "";
    SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
    ReportDB objRDB = new ReportDB();
    protected void Page_Load(object sender, EventArgs e)
    {
        Page.Title = "PSEB";
        if (!IsPostBack)
        {
            if (Session["SCHL"] != null)    //Session["userpseb2014"] change to Session["SCHL"]
            {
                // objHT = (Hashtable)Session["SCHL"];
                // uid = objHT["User"].ToString();
                 uid = Session["SCHL"].ToString();
                 RenderReport();
            }
            else
            {
                Response.Redirect("default.aspx");
            }
        }
        

    }


    private void RenderReport()
    {
        try
        {
            if (Request.QueryString["schl"] != null && Request.QueryString["lot"] != null && Request.QueryString["lot"].ToString() != "0")
            {           
              if (Session["SCHL"] != null)
            	{
                    // objHT = (Hashtable)Session["userpseb2014"];
                    //uid = objHT["User"].ToString();
                    uid = Session["SCHL"].ToString();
                    if (uid == Request.QueryString["Schl"].ToString().Trim())
                   {
                        string search = "";
                	   
                        string lotno = Request.QueryString["lot"].ToString();
                        string qry = "SELECT * FROM regMasterregular2016 WHERE SCHL ='" + uid+"' AND lot != '0'";
                        // DataSet dschk = SqlHelper.ExecuteDataset(somecommonfunctions.ConnectionStringPSEBLIVE, CommandType.Text, qry);
                        DataSet dschk = objRDB.GetDataByQuery(qry);
                        if (dschk.Tables[0].Rows.Count > 0)
                        { 
                            LocalReport localReport = new LocalReport();
                            localReport.ReportPath = Server.MapPath("Report1.rdlc");
                            //string qryn1 = "SELECT * FROM regMasterregular inner JOIN SchoolMaster ON regMasterregular.SCHL = SchoolMaster.SCHL WHERE (regMasterregular.SCHL ='"+uid+"') AND (regMasterregular.FORM = 'N1') AND (regMasterregular.lot ="+lotno+") order by srl";
                            // DataSet dsn1 = SqlHelper.ExecuteDataset(somecommonfunctions.ConnectionStringPSEBLIVE, CommandType.Text, qryn1);
                            search = "(regMasterregular2016.SCHL ='" + uid + "') AND (regMasterregular2016.FORM_Name = 'N1') AND (regMasterregular2016.lot =" + lotno + ")";
                            DataSet dsn1 = objRDB.FinalPrint(search);
                            dsn1.Tables[0].TableName = "PsebDsN1";

                            // start dataset region for n2 report..................
                            // string qryn2 = "SELECT * FROM regMasterregular inner JOIN SchoolMaster ON regMasterregular.SCHL = SchoolMaster.SCHL WHERE (regMasterregular.SCHL ='" + uid + "') AND (regMasterregular.FORM = 'N2') AND (regMasterregular.lot =" + lotno + ") order by srl";
                            //DataSet dsn2 = SqlHelper.ExecuteDataset(somecommonfunctions.ConnectionStringPSEBLIVE, CommandType.Text, qryn2);
                            search = "(regMasterregular2016.SCHL ='" + uid + "') AND (regMasterregular2016.FORM_Name = 'N2') AND (regMasterregular2016.lot =" + lotno + ")";
                            DataSet dsn2 = objRDB.FinalPrint(search);
                            dsn2.Tables[0].TableName = "PsebDsN2";

                            // End dataset region for n2 report..................

                            // start dataset region for n3 report..................
                            //string qryn3 = "SELECT * FROM regMasterregular inner JOIN SchoolMaster ON regMasterregular.SCHL = SchoolMaster.SCHL WHERE (regMasterregular.SCHL ='" + uid + "') AND (regMasterregular.FORM = 'N3') AND (regMasterregular.lot =" + lotno + ") order by srl";
                            //DataSet dsn3 = SqlHelper.ExecuteDataset(somecommonfunctions.ConnectionStringPSEBLIVE, CommandType.Text, qryn3);
                            search = "(regMasterregular2016.SCHL ='" + uid + "') AND (regMasterregular2016.FORM_Name = 'N3') AND (regMasterregular2016.lot =" + lotno + ")";
                            DataSet dsn3 = objRDB.FinalPrint(search);

                            dsn3.Tables[0].TableName = "PsebDsN3";
                            // End dataset region for n3 report..................

                            // start dataset region for M1 report..................
                            ////SqlParameter[] sparam1 = new SqlParameter[5];
                            ////sparam1[0] = new SqlParameter("@schl",uid);
                            ////sparam1[1] = new SqlParameter("@lot", lotno);
                            ////sparam1[2] = new SqlParameter("@form", "M1");
                            ////sparam1[3] = new SqlParameter("@exam", "");
                            ////sparam1[4] = new SqlParameter("@flag", 2);
                            ////DataSet dsm1 = SqlHelper.ExecuteDataset(somecommonfunctions.ConnectionStringPSEBLIVE, CommandType.StoredProcedure, "SubjectSPRegular", sparam1);
                            DataSet dsm1 = objRDB.SubjectSPRegular(uid, lotno, "M1", "", "2");
                            dsm1.Tables[0].TableName = "PsebDsM1";
                        // End dataset region for M1 report..................

                        // start dataset region for M2 report..................
                        ////SqlParameter[] sparam2 = new SqlParameter[5];
                        ////sparam2[0] = new SqlParameter("@schl", uid);
                        ////sparam2[1] = new SqlParameter("@lot", lotno);
                        ////sparam2[2] = new SqlParameter("@form", "M2");
                        ////sparam2[3] = new SqlParameter("@exam", "");
                        ////sparam2[4] = new SqlParameter("@flag", 2);
                      //  DataSet dsm2 = SqlHelper.ExecuteDataset(somecommonfunctions.ConnectionStringPSEBLIVE, CommandType.StoredProcedure, "SubjectSPRegular", sparam2);
                            DataSet dsm2 = objRDB.SubjectSPRegular(uid, lotno, "M2", "", "2");
                            dsm2.Tables[0].TableName = "PsebDsM2";
                            // End dataset region for M2 report..................

                            //// start dataset region for M3 report..................
                            //SqlCommand scmdM3 = new SqlCommand("SubjectSP '" + Request.QueryString["Schl"].ToString() + "','" + Request.QueryString["lot"].ToString() + "','M3',''," + 2);
                            ////scmdM3.Parameters.Add("@schl", SqlDbType.NVarChar).Value = Request.QueryString["Schl"].ToString();
                            ////scmdM3.Parameters.Add("@lot", SqlDbType.NVarChar).Value = Request.QueryString["lot"].ToString();
                            //DataSet dsM3 = DataManager.getDataSet(scmdM3);
                            //dsM3.Tables[0].TableName = "PsebDsM3";
                            //// End dataset region for M3 report..................

                            // start dataset region for E1 report..................
                            //// string qrye1 = "SELECT * FROM regMasterregular inner JOIN SchoolMaster ON regMasterregular.SCHL = SchoolMaster.SCHL WHERE (regMasterregular.SCHL ='" + uid + "') AND (regMasterregular.FORM = 'E1') AND (regMasterregular.lot =" + lotno + ") order by srl";
                            //// DataSet dse1 = SqlHelper.ExecuteDataset(somecommonfunctions.ConnectionStringPSEBLIVE, CommandType.Text, qrye1);
                            search = "(regMasterregular2016.SCHL ='" + uid + "') AND (regMasterregular2016.FORM_Name = 'E1') AND (regMasterregular2016.lot =" + lotno + ")";
                            DataSet dse1 = objRDB.FinalPrint(search);
                            dse1.Tables[0].TableName = "PsebDsE1";
                        // End dataset region for E1 report..................

                        // start dataset region for E2 report..................
                       //// string qrye2 = "SELECT * FROM regMasterregular inner JOIN SchoolMaster ON regMasterregular.SCHL = SchoolMaster.SCHL WHERE (regMasterregular.SCHL ='" + uid + "') AND (regMasterregular.FORM = 'E2') AND (regMasterregular.lot =" + lotno + ") order by srl";
                       //// DataSet dse2 = SqlHelper.ExecuteDataset(somecommonfunctions.ConnectionStringPSEBLIVE, CommandType.Text, qrye2);
                            search = "(regMasterregular2016.SCHL ='" + uid + "') AND (regMasterregular2016.FORM_Name = 'E2') AND (regMasterregular2016.lot =" + lotno + ")";
                            DataSet dse2 = objRDB.FinalPrint(search);
                            dse2.Tables[0].TableName = "PsebDsE2";
                        // End dataset region for E2 report..................

                        // start dataset region for T1 report..................
                        ////SqlParameter[] sparat1 = new SqlParameter[5];
                        ////sparat1[0] = new SqlParameter("@schl", uid);
                        ////sparat1[1] = new SqlParameter("@lot", lotno);
                        ////sparat1[2] = new SqlParameter("@form", "T1");
                        ////sparat1[3] = new SqlParameter("@exam", "");
                        ////sparat1[4] = new SqlParameter("@flag", 2);
                       //// DataSet dst1 = SqlHelper.ExecuteDataset(somecommonfunctions.ConnectionStringPSEBLIVE, CommandType.StoredProcedure, "SubjectSPRegular", sparat1);
                            DataSet dst1 = objRDB.SubjectSPRegular(uid, lotno, "T1", "", "2");
                            dst1.Tables[0].TableName = "PsebDsT1";
                        // End dataset region for T1 report..................

                        // start dataset region for T2 report..................
                        ////SqlParameter[] sparat2 = new SqlParameter[5];
                        ////sparat2[0] = new SqlParameter("@schl", uid);
                        ////sparat2[1] = new SqlParameter("@lot", lotno);
                        ////sparat2[2] = new SqlParameter("@form", "T2");
                        ////sparat2[3] = new SqlParameter("@exam", "");
                        ////sparat2[4] = new SqlParameter("@flag", 2);
                        ////DataSet dst2 = SqlHelper.ExecuteDataset(somecommonfunctions.ConnectionStringPSEBLIVE, CommandType.StoredProcedure, "SubjectSPRegular", sparat2);
                        

                            DataSet dst2 = objRDB.SubjectSPRegular(uid, lotno,"T2","","2");
                            dst2.Tables[0].TableName = "PsebDsT2";
                            // End dataset region for T2 report..................

                            //// start dataset region for T3 report..................
                            //SqlCommand scmdT3 = new SqlCommand("SubjectSP '" + Request.QueryString["Schl"].ToString() + "','" + Request.QueryString["lot"].ToString() + "','T3',''," + 2);
                            ////scmdT3.Parameters.Add("@schl", SqlDbType.NVarChar).Value = Request.QueryString["Schl"].ToString();
                            ////scmdT3.Parameters.Add("@lot", SqlDbType.NVarChar).Value = Request.QueryString["lot"].ToString();
                            //DataSet dsT3 = DataManager.getDataSet(scmdT3);
                            //dsT3.Tables[0].TableName = "PsebDsT3";
                            //// End dataset region for T3 report..................

                            // start dataset region for Schl report..................
                            string qryschl = "SELECT * FROM SchoolMaster WHERE SCHL ='" + uid + "'";
                            ////DataSet dsSchl = SqlHelper.ExecuteDataset(somecommonfunctions.ConnectionStringPSEBLIVE, CommandType.Text, qryschl);
                            DataSet dsSchl = objRDB.GetDataByQuery(qryschl);
                            dsSchl.Tables[0].TableName = "PsebDsSchl";
                        // End dataset region for schl report..................

                        // start dataset region for formSummary report..................

                        string qryregdate = "select max(regdate) as finalsubdt from regMasterregular2016 where schl='" + uid + "' and lot="+lotno.ToString();
                            ////  DataSet dsregdate = SqlHelper.ExecuteDataset(somecommonfunctions.ConnectionStringPSEBLIVE, CommandType.Text, qryregdate);
                        DataSet dsregdate = objRDB.GetDataByQuery(qryregdate);



                                  //SqlParameter[] sparasum = new SqlParameter[3];
                                  //sparasum[0] = new SqlParameter("@schl", uid);
                                  //sparasum[1] = new SqlParameter("@Admissiondate", DateTime.ParseExact(dsregdate.Tables[0].Rows[0][0].ToString(), "dd-MM-yyyy", null));
                                  //sparasum[2] = new SqlParameter("@LOT", lotno);

                                  //DataSet dsformSumm = SqlHelper.ExecuteDataset(somecommonfunctions.ConnectionStringPSEBLIVE, CommandType.StoredProcedure, "spFeeUndertaking", sparasum);

                            DataSet dsformSumm = objRDB.finalprintsubDetail(uid, lotno, "M1", "M2", "T1", "T2");
                            dsformSumm.Tables[0].TableName = "FormSummary";


                            // End dataset region for formSummary report..................

                            // string qrybank = "select top 1 [YEAR],[SET],LOT,CLASS,FORM,REGDATE,DIST,RP,EXAM,IDNO,ocode,SCHOOLE,SCHOOLP,oldSchlCode,feecat,Addfee,latefee,REGFEE,ProsFee,AddSubFee,TotFee,add_sub_count,ChallnID,formno,PrintLot,INSERTDT,UPDT,wantwriter,sportperson,DOA,oldid,Current_ClassRoll,Current_Section,ddno,dddate,ddamt,banknm,[USER],tblschuser.SCHL,PRINCIPAL,STDCODE,PHONE,MOBILE,EMAILID,CONTACTPER,CPSTD,CPPHONE,OtContactno,ACTIVE,USERTYPE,ADDRESSE,ADDRESSP,mobile2,payableat as NATION from regMasterregular inner join tblSchUser on regMasterregular.SCHL=tblSchUser.SCHL and regMasterregular.LOT="+lotno+" and tblSchUser.SCHL='"+uid+"'";
                            // // DataSet dsBank = SqlHelper.ExecuteDataset(somecommonfunctions.ConnectionStringPSEBLIVE, CommandType.Text, qrybank);
                            string qrybank = "select top 1 [YEAR],[SET],LOT,CLASS,FORM_Name,District DIST,RP,EXAM,IDNO,ocode,SCHOOLE,SCHOOLP,oldSchlCode,feecat,Addfee,latefee,REGFEE,ProsFee,AddSubFee,TotFee,add_sub_count,ChallanID,formno,PrintLot,INSERTDT,UPDT,wantwriter,sportperson,DOA,oldid,Class_Roll_Num_Section Current_ClassRoll, Section Current_Section,ddno,dddate,ddamt,banknm,[USER],tblschuser.SCHL,PRINCIPAL,STDCODE,PHONE,tblSchUser.MOBILE,EMAILID,CONTACTPER,CPSTD,CPPHONE,OtContactno,ACTIVE,USERTYPE,ADDRESSE,ADDRESSP,mobile2,payableat as NATION from regMasterregular2016 inner join tblSchUser on regMasterregular2016.SCHL=tblSchUser.SCHL and regMasterregular2016.LOT= " + lotno + " and tblSchUser.SCHL='" + uid + "'";
                            DataSet dsBank = objRDB.GetDataByQuery(qrybank);
                            dsBank.Tables[0].TableName = "PsebDsBank";

                        // start dataset region for Schl report..................
                                                //rdsSchl.Tables[0].Columns.Add("lot", typeof(String));
                        //rdsSchl.Tables[0].Rows[0]["lot"] = "000123";
                        // End dataset region for schl report..................

                        ////SqlParameter[] sparasd = new SqlParameter[6];
                        ////sparasd[0] = new SqlParameter("@schl", uid);
                        ////sparasd[1] = new SqlParameter("@lot", lotno);
                        ////sparasd[2] = new SqlParameter("@formM1", "M1");
                        ////sparasd[3] = new SqlParameter("@formM2", "M2");
                        ////sparasd[4] = new SqlParameter("@formT1", "T1");
                        ////sparasd[5] = new SqlParameter("@formT2", "T2");

                    ////    DataSet dssubjectM1 = SqlHelper.ExecuteDataset(somecommonfunctions.ConnectionStringPSEBLIVE, CommandType.StoredProcedure, "finalprintsubDetail", sparasd);

                        DataSet dssubjectM1 = objRDB.finalprintsubDetail(uid, lotno,"M1", "M2", "T1", "T2");   
                        dssubjectM1.Tables[0].TableName = "SubDetailM1";
                        dssubjectM1.Tables[1].TableName = "SubDetailM2";
                        dssubjectM1.Tables[2].TableName = "SubDetailT1";
                        dssubjectM1.Tables[3].TableName = "SubDetailT2";
                        ReportDataSource rdsSubDetailM1 = new ReportDataSource("SubDetailM1", dssubjectM1.Tables[0]);
                        ReportDataSource rdsSubDetailM2 = new ReportDataSource("SubDetailM2", dssubjectM1.Tables[1]);
                        ReportDataSource rdsSubDetailT1 = new ReportDataSource("SubDetailT1", dssubjectM1.Tables[2]);
                        ReportDataSource rdsSubDetailT2 = new ReportDataSource("SubDetailT2", dssubjectM1.Tables[3]);
                       ////// ReportViewer1.Visible = true;
                        ReportDataSource datasource = new ReportDataSource("PsebDsN1", dsn1.Tables[0]);
                        ReportDataSource rdsN2 = new ReportDataSource("PsebDsN2", dsn2.Tables[0]);
                        ReportDataSource rdsN3 = new ReportDataSource("PsebDsN3", dsn3.Tables[0]);
                        ReportDataSource rdsM1 = new ReportDataSource("PsebDsM1", dsm1.Tables[0]);
                        ReportDataSource rdsM2 = new ReportDataSource("PsebDsM2", dsm2.Tables[0]);
                        
                        ReportDataSource rdsE1 = new ReportDataSource("PsebDsE1", dse1.Tables[0]);
                        ReportDataSource rdsE2 = new ReportDataSource("PsebDsE2", dse2.Tables[0]);
                        ReportDataSource rdsT1 = new ReportDataSource("PsebDsT1", dst1.Tables[0]);
                        ReportDataSource rdsT2 = new ReportDataSource("PsebDsT2", dst2.Tables[0]);
                        
                        ReportDataSource rdsSchl = new ReportDataSource("PsebDsSchl", dsSchl.Tables[0]);
                        ReportDataSource rdsformSumm = new ReportDataSource("FormSummary", dsformSumm.Tables[0]);
                        ReportDataSource rdsBank = new ReportDataSource("PsebDsBank", dsBank.Tables[0]);
                       //////ReportViewer1.LocalReport.DataSources.Clear();
                        //////ReportViewer1.LocalReport.DataSources.Add(datasource);
                        //////ReportViewer1.LocalReport.DataSources.Add(rdsN2);
                        //////ReportViewer1.LocalReport.DataSources.Add(rdsN3);
                        //////ReportViewer1.LocalReport.DataSources.Add(rdsM1);
                        //////ReportViewer1.LocalReport.DataSources.Add(rdsM2);
                        
                        //////ReportViewer1.LocalReport.DataSources.Add(rdsE1);
                        //////ReportViewer1.LocalReport.DataSources.Add(rdsE2);
                        //////ReportViewer1.LocalReport.DataSources.Add(rdsT1);
                        //////ReportViewer1.LocalReport.DataSources.Add(rdsT2);
                        
                        //////ReportViewer1.LocalReport.DataSources.Add(rdsSchl);
                        //////ReportViewer1.LocalReport.DataSources.Add(rdsformSumm);
                        //////ReportViewer1.LocalReport.DataSources.Add(rdsBank);

                        //////ReportViewer1.LocalReport.DataSources.Add(rdsSubDetailM1);
                        //////ReportViewer1.LocalReport.DataSources.Add(rdsSubDetailM2);
                        
                        //////ReportViewer1.LocalReport.DataSources.Add(rdsSubDetailT1);
                        //////ReportViewer1.LocalReport.DataSources.Add(rdsSubDetailT2);
                        
                            
                        //////ReportViewer1.LocalReport.Refresh();
                        //    RenderReport(Request.QueryString["Schlid"].ToString());
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "Massage", "alert('There is no Final Submission.');document.location.href='home.aspx';", true);

                    }
                }
                else
                {
                    Response.Redirect("home.aspx", false);
                }
            }
            else
            {
                Response.Redirect("home.aspx", false);
            }
            }
            else
            {
                Response.Redirect("default.aspx");
            }


        }
        catch(Exception ex)
        {
        
            Response.Redirect("home.aspx", false);
        }

    }
    //private void Export()
    //{
    //    string reportType = "PDF";
    //    string mimeType;
    //    string encoding;
    //    string fileNameExtension = ".pdf";

    //    //The DeviceInfo settings should be changed based on the reportType
    //    //https://msdn2.microsoft.com/en-us/library/ms155397.aspx
    //    string deviceInfo =
    //    "<DeviceInfo>" +
    //    "  <OutputFormat>PDF</OutputFormat>" +
    //    "  <PageWidth>14.5in</PageWidth>" +
    //    "  <PageHeight>8.5in</PageHeight>" +
    //    "  <MarginTop>0.2in</MarginTop>" +
    //    "  <MarginLeft>1.5in</MarginLeft>" +
    //    "  <MarginRight>0.2in</MarginRight>" +
    //    "  <MarginBottom>0.2in</MarginBottom>" +
    //    "</DeviceInfo>";

    //    Warning[] warnings;
    //    string[] streams;
    //    byte[] renderedBytes;

    //    //Render the report       
    //    renderedBytes =
    //        ReportViewer1.LocalReport.Render(
    //        reportType,
    //        deviceInfo,
    //        out mimeType,
    //        out encoding,
    //        out fileNameExtension,
    //        out streams,
    //        out warnings);


    //    //Clear the response stream and write the bytes to the outputstream
    //    //Set content-disposition to "attachment" so that user is prompted to take an action
    //    //on the file (open or save)
    //    Response.Clear();
    //    Response.ContentType = mimeType;
    //    Response.ContentType = "PDF";
    //    //Response.AddHeader("filename","FormReport.pdf");
    //    Response.AddHeader("content-disposition", "attachment; filename=FinalReport." + fileNameExtension);
    //    Response.BinaryWrite(renderedBytes);
    //    Response.End();
    //}
    protected void Button1_Click(object sender, EventArgs e)
    {
        //Export();
    }
}