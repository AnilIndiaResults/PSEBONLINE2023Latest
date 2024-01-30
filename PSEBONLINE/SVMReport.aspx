<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SVMReport.aspx.cs" Inherits="PSEBONLINE.Views.SVMReport" %>
<%@ Register assembly="Microsoft.ReportViewer.WebForms, Version=11.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" namespace="Microsoft.Reporting.WebForms" tagprefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="https://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="Scripts/jquery-1.8.2.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
     <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>               
     <%-- <rsweb:ReportViewer ID="ReportViewerEmployee" runat="server" AsyncRendering="false" SizeToReportContent="true"
          Width="1000px" Height="800px" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt">
        </rsweb:ReportViewer>  --%>

          <rsweb:ReportViewer Width="100%" ID="ReportViewer2" SizeToReportContent="true" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt">            
        </rsweb:ReportViewer>    
    </div>
    </form>
</body>
</html>
