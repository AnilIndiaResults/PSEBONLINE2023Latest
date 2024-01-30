<%--<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExamPortal.aspx.cs" Inherits="PSEBONLINE.ExamPortal" %>--%>
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
                string Formname = "M3";
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                //report path
                ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/RPTReports/ExamReport1Open.rdlc");
                SqlDataAdapter adp = new SqlDataAdapter("ExamReportSPFinalOPEN", con);//ExamReportSP  for Senior use  "ExamReportSPFinalSenior"
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
                adp.Fill(ds, "ExamReportSPFinalOPEN");

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

                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/RPTReports/ExamReport1Open.rdlc");
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
            string CanId = e.Parameters["CANDID"].Values[0].ToString();// CANDID

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
                string qry = "SELECT * From tblsubjectOPEN where APPNO='" + Canid + "'  order by SUB_SEQ asc";
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
