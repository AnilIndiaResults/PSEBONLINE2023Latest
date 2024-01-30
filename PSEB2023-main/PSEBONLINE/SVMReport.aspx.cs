using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.Reporting.WebForms;


namespace PSEBONLINE.Views
{
    public partial class SVMReport : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
          public string Form_name = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (Request.QueryString["Form"] == null || Request.QueryString["Form"] == "")
            //{
            //    Response.Redirect("~/Login");
            //}
            //else
            //{
            // Form = Request.QueryString["Form"].ToString();
            Form_name = "N1";
            if (!IsPostBack)
            {
                ReportViewer2.Visible = false;
                BindReport(Form_name);
            }
            //}
        }

        private void BindReport(string FormName)
        {
            try
            {
                ReportViewer2.ProcessingMode = ProcessingMode.Local;
                //report path
                ReportViewer2.LocalReport.ReportPath = Server.MapPath("~/RPTReports/RptStudentVerificationFormNew.rdlc");
                SqlDataAdapter adp = new SqlDataAdapter("RoughReportSP", con);
                adp.SelectCommand.CommandType = CommandType.StoredProcedure;
                adp.SelectCommand.Parameters.AddWithValue("@FormName", FormName);
                // adp.SelectCommand.Parameters.AddWithValue("@Section", ddlSection.SelectedItem.Text);
                adp.SelectCommand.Parameters.AddWithValue("@Section", "All");
                //object of Dataset DemoDataSet
                PSEBONLINE.RPTDatasets.firmdata_onlineDataSet ds = new PSEBONLINE.RPTDatasets.firmdata_onlineDataSet(); // datasource of report
                adp.Fill(ds, "RoughReportSP");
                //Datasource for report
                ReportDataSource datasource = new ReportDataSource("RoughReportDataSet", ds.Tables[0]);
                // ReportDataSource rdc = new ReportDataSource("MyDataset", customers);
                ReportViewer2.Width = 600;
                ReportViewer2.LocalReport.DataSources.Clear();
                ReportViewer2.LocalReport.DataSources.Add(datasource);
                ReportViewer2.Visible = true;

                //ReportViewer2.Visible = true;
                //ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                //ReportViewer2.LocalReport.DataSources.Clear();
                //ReportViewer2.LocalReport.DataSources.Add(datasource);
                //ReportViewer2.LocalReport.ReportPath = Server.MapPath("~/RPTReports/RptStudentVerificationFormNew.rdlc");
                //ReportViewer2.LocalReport.Refresh();
            }
            catch (Exception) //Form-icon.png

            {

            }
        }
    }
}