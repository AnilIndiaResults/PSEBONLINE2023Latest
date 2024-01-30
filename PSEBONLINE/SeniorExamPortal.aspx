<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SeniorExamPortal.aspx.cs" Inherits="PSEBONLINE.SeniorExamPortal" %>--%>
<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="Microsoft.Reporting.WebForms" %>
<%@ Import Namespace="System.Drawing" %>
<%@ Import Namespace="System.Drawing.Printing" %>
<%@ Import Namespace="PSEBONLINE" %>
<%@ Import Namespace="PSEBONLINE.AbstractLayer" %>
<!DOCTYPE html>

<html xmlns="https://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script runat="server">
        //  SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
        Hashtable objHT = new Hashtable();
        // somecommonfunctions obj = new somecommonfunctions("pseb2014");
        string uid = string.Empty;
        string schoollogin = string.Empty;
        Int32 lot = 0;
        string form = "";
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
        ReportDB objRDB = new ReportDB();
        public string Form = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = "PSEB";
            if (!IsPostBack)
            {

                if (Session["SCHL"] != null)    //Session["userpseb2014"] change to Session["SCHL"]
                {
                    schoollogin = Session["SCHL"].ToString();

                    if (Request.QueryString["schl"] == null)
                    {
                        Response.Redirect("~/Home");
                    }
                    else
                    {

                        //Form = Session["FormName"] .ToString();
                        string schl2 = Request.QueryString["schl"].ToString();
                        string plot = Request.QueryString["printlot"].ToString();
                        if (plot != "0")
                        {
                            BindReport(schl2, plot);
                        }
                      
                        //BindReport(schl2, "1");
                    }


                    // RenderReport();
                }
                else
                {
                    Response.Redirect("~/Login");
                }
            }
        }


        private void BindReport(string SCHL,string lot1)
        {
            try
            {
                string Formname = "T";

                //string Search = "R.SCHL ='" + SCHL + "' and R.FORM_Name in('" + Formname + "') and LOT = '" + lot1 + "'";

                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                //report path
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/RPTReports/SeniorExamReport.rdlc");
                SqlDataAdapter adp = new SqlDataAdapter("ExamReportSPFinalSenior", con);//ExamReportSP  for Senior use  "ExamReportSPFinalSenior"
                adp.SelectCommand.CommandType = CommandType.StoredProcedure;
                adp.SelectCommand.Parameters.AddWithValue("@SCHL", SCHL);
                adp.SelectCommand.Parameters.AddWithValue("@lot", lot1);
                adp.SelectCommand.Parameters.AddWithValue("@Formname", Formname);
                //adp.SelectCommand.Parameters.AddWithValue("@Search", Search);
                //object of Dataset DemoDataSet
                PSEBONLINE.RPTDatasets.firmdata_onlineDataSet ds = new PSEBONLINE.RPTDatasets.firmdata_onlineDataSet(); // datasource of report
                //  adp.Fill(ds, "RoughReportSP");
                ds.Clear();
                ds.EnforceConstraints = false;
                adp.Fill(ds, "ExamReportSPFinalSenior");

                //Datasource for report
                if (ds.Tables[2].Rows.Count > 0)
                {
                    ReportViewer1.Visible = true;
                    ReportDataSource datasource = new ReportDataSource("ExamReportDataSet", ds.Tables[2]);
                    ReportDataSource datasource1 = new ReportDataSource("SubjectwiseCountOfStudent", ds.Tables[3]);
                    ReportDataSource datasource2 = new ReportDataSource("SubjectwiseCountMatricExam", ds.Tables[4]);
                    //ReportDataSource datasource3 = new ReportDataSource("MatricSubjectsDataset", ds.Tables[5]);MatricSubjectsDataset
                    ReportDataSource datasource4 = new ReportDataSource("ChallanDetails", ds.Tables[5]);
                    // ReportDataSource rdc = new ReportDataSource("MyDataset", customers);
                    //ReportViewer1.Width = 600;
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(datasource);
                    ReportViewer1.LocalReport.DataSources.Add(datasource1);
                    ReportViewer1.LocalReport.DataSources.Add(datasource2);
                    //ReportViewer1.LocalReport.DataSources.Add(datasource3);
                    ReportViewer1.LocalReport.DataSources.Add(datasource4);

                    // by Rohit
                    ReportViewer1.Visible = true;

                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/RPTReports/SeniorExamReport.rdlc");
                    ReportViewer1.LocalReport.EnableExternalImages = true;
                    string imagePath = new Uri(Server.MapPath("~/Upload/")).AbsoluteUri;
                    ReportParameter parameter = new ReportParameter("ImgPath", imagePath);
                    ReportViewer1.LocalReport.SetParameters(parameter);
                    ReportViewer1.LocalReport.SubreportProcessing += new Microsoft.Reporting.WebForms.SubreportProcessingEventHandler(SubReportMatricSubject);
                    ReportViewer1.LocalReport.Refresh();


                }
                else
                {
                    ReportViewer1.Visible = false;
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Record Not Found..');", true);
                    return;
                }

            }
            catch (Exception ex) //Form-icon.png
            {

            }
        }
        void SubReportMatricSubject(object sender, SubreportProcessingEventArgs e)
        {
            string CanId = e.Parameters["CANDID"].Values[0].ToString();

            DataSet ds=GetSubjectById(CanId);
            ds.Tables[0].TableName="MatricSubjectDataset";
            ReportDataSource datasource2 = new ReportDataSource("MatricSubjectDataset", ds.Tables[0]);
            e.DataSources.Add(datasource2);
        }
        private DataSet GetSubjectById(string Canid)
        {
            try
            {
                //DisplayData obj = new DisplayData();
                //LocalReport localReport = new LocalReport();
                // localReport.ReportPath = Server.MapPath("SubReportMatricSubject.rdlc");
                //SqlCommand scommand2 = new SqlCommand("SELECT * From tblMatricSubjects where candid='" + Canid + "'");
                //DataSet ds2 = DataManager2.getDataSet(scommand2);
                string qry = "SELECT * From tblSeniorSubjects where candid='" + Canid + "' order by SUB_SEQ asc";
                DataSet ds2 = objRDB.GetDataByQuery(qry);
                ds2.Tables[0].TableName = "MatricSubjectDataset";
                return ds2;
                //ReportViewer1.Visible = true;
                //ReportDataSource datasource2 = new ReportDataSource("MatricSubject", ds2.Tables[0]);
                //ReportViewer1.LocalReport.DataSources.Clear();
                //ReportViewer1.LocalReport.DataSources.Add(datasource2);
                //ReportViewer1.LocalReport.Refresh();
                //    RenderReport(Request.QueryString["Schlid"].ToString());

            }
            catch
            {
                return null;
            }
        }
        private void RenderReport(string schl1,string lot1)
        {
            try
            {
                // if (Session["PrintLot"] != null && Session["PrintLot"].ToString() != "0")
                if (schl1 != "" && lot1 != "0")
                {
                    //uid = Session["SCHL"].ToString();
                    // string lotno = Session["PrintLot"].ToString();
                    uid = schl1;
                    string lotno = lot1;
                    if (uid == schoollogin)
                    {
                        string search = "";
                        string qry = "SELECT * FROM regMasterregular2016 WHERE SCHL ='" + uid + "' AND lot != '0'";
                        DataSet dschk = objRDB.GetDataByQuery(qry);
                        if (dschk.Tables[0].Rows.Count > 0)
                        {
                            LocalReport localReport = new LocalReport();
                            ReportViewer1.ProcessingMode = ProcessingMode.Local;
                            //report path
                            ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/RPTReports/SeniorExamReport.rdlc");

                            DataSet   dataSet = new DataSet ();
                            DataSet   dssubjectM1=   new DataSet ();

                            DataSet   dsm1=  new DataSet ();
                            DataSet  dsm2 = new DataSet ();

                            DataSet   dst1=  new DataSet ();
                            DataSet  dst2 = new DataSet ();

                            DataSet  dsSchl =  new DataSet ();
                            DataSet  dsformSumm =  new DataSet ();
                            DataSet dsBank=  new DataSet ();
                            DataSet dsregdate = new DataSet();

                            //if (Cache["FinalPrintDataSet"] == null)
                            //{
                            //    Cache["FinalPrintDataSet"] = dschk;



                            // start dataset region for M1 report..................
                            dsm1 = objRDB.SubjectSPRegular(uid, lotno, "M1", "", "2");
                            dsm1.Tables[0].TableName = "PsebDsM1";
                            // End dataset region for M1 report..................

                            // start dataset region for M2 report..................
                            dsm2 = objRDB.SubjectSPRegular(uid, lotno, "M2", "", "2");
                            dsm2.Tables[0].TableName = "PsebDsM2";
                            // End dataset region for M2 report..................                            

                            // start dataset region for T1 report..................
                            dst1 = objRDB.SubjectSPRegular(uid, lotno, "T1", "", "2");
                            dst1.Tables[0].TableName = "PsebDsT1";
                            // End dataset region for T1 report..................

                            // start dataset region for T2 report..................
                            dst2 = objRDB.SubjectSPRegular(uid, lotno, "T2", "", "2");
                            dst2.Tables[0].TableName = "PsebDsT2";
                            // End dataset region for T2 report..................

                            // start dataset region for Schl report..................
                            string qryschl = "SELECT * FROM SchoolMaster WHERE SCHL ='" + uid + "'";
                            ////DataSet dsSchl = SqlHelper.ExecuteDataset(somecommonfunctions.ConnectionStringPSEBLIVE, CommandType.Text, qryschl);
                            dsSchl = objRDB.GetDataByQuery(qryschl);
                            dsSchl.Tables[0].TableName = "PsebDsSchl";
                            // End dataset region for schl report..................

                            // start dataset region for formSummary report..................                           
                            //change by Rohit used Challandate instead of  Regdate  
                            string qryregdate = "select max(convert(varchar,challandt,103)) as finalsubdt from regMasterregular2016 where schl='" + uid + "' and lot=" + lotno.ToString();
                            dsregdate = objRDB.GetDataByQuery(qryregdate);
                            dsformSumm = objRDB.spFeeUndertaking(uid, lotno, DateTime.ParseExact(dsregdate.Tables[0].Rows[0][0].ToString(), "dd/MM/yyyy", null));
                            dsformSumm.Tables[0].TableName = "FormSummary";


                            // End dataset region for formSummary report..................
                            string qrybank = "select [YEAR],[SET],regMasterregular2016.LOT,CLASS,FORM_Name,District DIST,RP,EXAM,IDNO,ocode,SCHOOLE,SCHOOLP,oldSchlCode,regMasterregular2016.feecat,regMasterregular2016.Addfee,regMasterregular2016.latefee,regMasterregular2016.REGFEE,regMasterregular2016.ProsFee,"
+ "regMasterregular2016.AddSubFee,regMasterregular2016.TotFee,regMasterregular2016.add_sub_count,challanmaster.ChallanID as ddno,challanmaster.DEPOSITDT as dddate,convert(varchar,challanmaster.Fee) + '.00' as ddamt,convert(varchar,challanmaster.totfee) + '.00' as challantotfee,"
+ "challanmaster.bcode + '-'+challanmaster.Bank as banknm,formno,PrintLot,INSERTDT,UPDT,wantwriter,sportperson,DOA,oldid,Class_Roll_Num_Section Current_ClassRoll, Section Current_Section,[USER],tblschuser.SCHL,PRINCIPAL,STDCODE,PHONE,tblSchUser.MOBILE,EMAILID,CONTACTPER,CPSTD,"
+ "CPPHONE,OtContactno,ACTIVE,USERTYPE,ADDRESSE,ADDRESSP,mobile2,challanmaster.BRANCH as NATION from regMasterregular2016 inner join tblSchUser on regMasterregular2016.SCHL=tblSchUser.SCHL inner join challanmaster on challanmaster.schoolcode=regMasterregular2016.SCHL  "
+ " and regMasterregular2016.LOT= " + lotno + " and tblSchUser.SCHL='" + uid + "'";
                            dsBank = objRDB.GetDataByQuery(qrybank);
                            dsBank.Tables[0].TableName = "PsebDsBank";

                            //start dataset region for Schl report..................
                            //rdsSchl.Tables[0].Columns.Add("lot", typeof(String));
                            //rdsSchl.Tables[0].Rows[0]["lot"] = "000123";
                            // End dataset region for schl report..................

                            dssubjectM1 = objRDB.finalprintsubDetail(uid, lotno, "M1", "M2", "T1", "T2");
                            dssubjectM1.Tables[0].TableName = "SubDetailM1";
                            dssubjectM1.Tables[1].TableName = "SubDetailM2";
                            dssubjectM1.Tables[2].TableName = "SubDetailT1";
                            dssubjectM1.Tables[3].TableName = "SubDetailT2";

                            //// Add Cache
                            //Cache["dssubjectM1DataSet"] = dssubjectM1;
                            //Cache["dsn1DataSet"] = dsn1;
                            //Cache["dsn2DataSet"] = dsn2;
                            //Cache["dsn3DataSet"] = dsn3;
                            //Cache["dsm1DataSet"] = dsm1;
                            //Cache["dsm2DataSet"] = dsm2;
                            //Cache["dse1DataSet"] = dse1;
                            //Cache["dse2DataSet"] = dse2;
                            //Cache["dst1DataSet"] = dst1;
                            //Cache["dst2DataSet"] = dst2;

                            //Cache["dsSchlDataSet"] = dsSchl;
                            //Cache["dsformSummDataSet"] = dsformSumm;
                            //Cache["dsBankDataSet"] = dsBank;
                            //// End Cache
                            //}
                            //else
                            //{
                            //    //dschk = (DataSet)Cache["FinalPrintDataSet"];
                            //    dssubjectM1 = (DataSet)Cache["dssubjectM1DataSet"];
                            //    dsn1 = (DataSet)Cache["dsn1DataSet"];
                            //    dsn2 = (DataSet)Cache["dsn2DataSet"];
                            //    dsn3 = (DataSet)Cache["dsn3DataSet"];
                            //    dsm1 = (DataSet)Cache["dsm1DataSet"];
                            //    dsm2 = (DataSet)Cache["dsm2DataSet"];
                            //    dse1 = (DataSet)Cache["dse1DataSet"];
                            //    dse2 = (DataSet)Cache["dse2DataSet"];
                            //    dst1 = (DataSet)Cache["dst1DataSet"];
                            //    dst2 = (DataSet)Cache["dst2DataSet"];
                            //    dsSchl = (DataSet)Cache["dsSchlDataSet"];
                            //    dsformSumm = (DataSet)Cache["dsformSummDataSet"];
                            //    dsBank = (DataSet)Cache["dsBankDataSet"];
                            //}

                            //if (dssubjectM1 == null)
                            //{
                            //    HttpRuntime.Cache.Remove("FinalPrintDataSet");
                            //    Response.Redirect("~/Login");
                            //}

                            ReportDataSource rdsSubDetailM1 = new ReportDataSource("SubDetailM1", dssubjectM1.Tables[0]);
                            ReportDataSource rdsSubDetailM2 = new ReportDataSource("SubDetailM2", dssubjectM1.Tables[1]);
                            ReportDataSource rdsSubDetailT1 = new ReportDataSource("SubDetailT1", dssubjectM1.Tables[2]);
                            ReportDataSource rdsSubDetailT2 = new ReportDataSource("SubDetailT2", dssubjectM1.Tables[3]);

                            ////// ReportViewer1.Visible = true;
                            //ReportDataSource datasource = new ReportDataSource("PsebDsN1", dsn1.Tables[0]);
                            //ReportDataSource rdsN2 = new ReportDataSource("PsebDsN2", dsn2.Tables[0]);
                            //ReportDataSource rdsN3 = new ReportDataSource("PsebDsN3", dsn3.Tables[0]);
                            ReportDataSource rdsM1 = new ReportDataSource("PsebDsM1", dsm1.Tables[0]);
                            ReportDataSource rdsM2 = new ReportDataSource("PsebDsM2", dsm2.Tables[0]);

                            //ReportDataSource rdsE1 = new ReportDataSource("PsebDsE1", dse1.Tables[0]);
                            //ReportDataSource rdsE2 = new ReportDataSource("PsebDsE2", dse2.Tables[0]);
                            ReportDataSource rdsT1 = new ReportDataSource("PsebDsT1", dst1.Tables[0]);
                            ReportDataSource rdsT2 = new ReportDataSource("PsebDsT2", dst2.Tables[0]);

                            ReportDataSource rdsSchl = new ReportDataSource("PsebDsSchl", dsSchl.Tables[0]);
                            ReportDataSource rdsformSumm = new ReportDataSource("FormSummary", dsformSumm.Tables[0]);
                            ReportDataSource rdsBank = new ReportDataSource("PsebDsBank", dsBank.Tables[0]);



                            ReportViewer1.LocalReport.DataSources.Clear();
                            //ReportViewer1.LocalReport.DataSources.Add(datasource);
                            //ReportViewer1.LocalReport.DataSources.Add(rdsN2);
                            //ReportViewer1.LocalReport.DataSources.Add(rdsN3);
                            ReportViewer1.LocalReport.DataSources.Add(rdsM1);
                            ReportViewer1.LocalReport.DataSources.Add(rdsM2);

                            //ReportViewer1.LocalReport.DataSources.Add(rdsE1);
                            //ReportViewer1.LocalReport.DataSources.Add(rdsE2);
                            ReportViewer1.LocalReport.DataSources.Add(rdsT1);
                            ReportViewer1.LocalReport.DataSources.Add(rdsT2);

                            ReportViewer1.LocalReport.DataSources.Add(rdsSchl);
                            ReportViewer1.LocalReport.DataSources.Add(rdsformSumm);
                            ReportViewer1.LocalReport.DataSources.Add(rdsBank);

                            ReportViewer1.LocalReport.DataSources.Add(rdsSubDetailM1);
                            ReportViewer1.LocalReport.DataSources.Add(rdsSubDetailM2);

                            ReportViewer1.LocalReport.DataSources.Add(rdsSubDetailT1);
                            ReportViewer1.LocalReport.DataSources.Add(rdsSubDetailT2);

                            // by Rohit
                            ReportViewer1.Visible = true;
                            ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/RPTReports/ExamReport1.rdlc");
                            ReportViewer1.LocalReport.EnableExternalImages = true;
                            string imagePath = new Uri(Server.MapPath("~/Upload/")).AbsoluteUri;
                            ReportParameter parameter = new ReportParameter("ImgPath", imagePath);
                            ReportViewer1.LocalReport.SetParameters(parameter);
                            ReportViewer1.LocalReport.Refresh();

                            Export(); // Export to PDF
                            Session["PrintLot"] = null;
                        }

                        else
                        {
                            //  ClientScript.RegisterStartupScript(this.GetType(), "Massage", "alert('There is no Final Submission.');document.location.href='home.aspx';", true);
                            ClientScript.RegisterStartupScript(this.GetType(), "Massage", "alert('There is no Final Submission.');", true);
                        }
                    }
                    else
                    {
                        Response.Redirect("~/Login");
                        // Response.Redirect("home.aspx", false);
                    }
                }
                else
                {
                    Response.Redirect("~/Login");
                    // Response.Redirect("home.aspx", false);
                }
                //}
                //else
                //{
                //    // Response.Redirect("default.aspx");
                //}
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }

        }


        private void Export()
        {
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension = ".pdf";



            //The DeviceInfo settings should be changed based on the reportType
            //https://msdn2.microsoft.com/en-us/library/ms155397.aspx
            string deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>PDF</OutputFormat>" +
             "  <PageWidth>8.27in</PageWidth>" +
            "  <PageHeight>12.69in</PageHeight>" +
            "  <MarginTop>0.2in</MarginTop>" +
            "  <MarginLeft>0.2in</MarginLeft>" +
            "  <MarginRight>0.2in</MarginRight>" +
            "  <MarginBottom>0.2in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;
            byte[] renderedBytes;

            //Render the report       
            renderedBytes =
                ReportViewer1.LocalReport.Render(
                reportType,
                deviceInfo,
                out mimeType,
                out encoding,
                out fileNameExtension,
                out streams,
                out warnings);


            //Clear the response stream and write the bytes to the outputstream
            //Set content-disposition to "attachment" so that user is prompted to take an action
            //on the file (open or save)
            Response.Clear();
            Response.ContentType = mimeType;
            Response.ContentType = "PDF";
            //Response.AddHeader("filename","FormReport.pdf");
            Response.AddHeader("content-disposition", "attachment; filename=FinalReport." + fileNameExtension);
            Response.BinaryWrite(renderedBytes);
            Response.End();
        }
        protected void Button1_Click(object sender, EventArgs e)
        {
            Export();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
   <center>    
    <asp:Button ID="Button1" runat="server" onclick="Button1_Click" class="submit" Text="Click Here to Download & Print" />
     <a href="#"  class="submitanc">If any thing Not readable Click Here to Download Font</a>
    </center>
         <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" width="100%" Height="600px" Font-Names="pnb-ttamarenbold" Font-Size="8pt" InteractiveDeviceInfos="(Collection)" 
WaitMessageFont-Names="pnb-ttamarenbold" WaitMessageFont-Size="14pt" ShowExportControls="False">
<LocalReport ReportPath="~/RPTReports/ExamReport1.rdlc"></LocalReport>
</rsweb:ReportViewer>
    </form>
</body>
</html>
