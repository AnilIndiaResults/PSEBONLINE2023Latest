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
<head id="Head1" runat="server">
    <title></title>
    <style>
    @media print
    {
       .noprint
       {
           display:none;
           width:100%;
       } 
       .print
       {
       width:100%;
       }
    }
    </style>

    <style type="text/css">
					body{
				font-family: 'pnb-ttamarenbold';
							}
		</style>   </head>



<body style="margin:0;padding:0;font-family: 'pnb-ttamarenbold';font-size:10px; background-repeat:repeat;">

<div style="position:absolute;top:0;left:0;z-index:-99999;width:100%;height:100%; text-align:center; vertical-align:middle;" id="watermark"></div>
    <form id="form1" runat="server">   
     <center>
     <%--<a href="../Home/FinalPrint" class="submitanc"> Back </a>--%>&nbsp;&nbsp;
    <asp:Button ID="Button1" runat="server" onclick="Button1_Click" class="submit" Text="Click Here to Download & Print" />
     <a href="#"  class="submitanc">If any thing Not readable Click Here to Download Font</a>
     </center>
   
     <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
<%--<rsweb:ReportViewer ID="ReportViewer1" runat="server" width="100%" Height="600px" Font-Names="pnb-ttamarenbold" Font-Size="8pt" InteractiveDeviceInfos="(Collection)" 
WaitMessageFont-Names="pnb-ttamarenbold" WaitMessageFont-Size="14pt" ShowExportControls="False">--%>
<%--<LocalReport ReportPath="pb\pseb\pseb 2014\Report1.rdlc"></LocalReport>--%>
<rsweb:ReportViewer ID="ReportViewer1" runat="server" width="100%" Height="600px" Font-Names="pnb-ttamarenbold" Font-Size="8pt" InteractiveDeviceInfos="(Collection)" 
WaitMessageFont-Names="pnb-ttamarenbold" WaitMessageFont-Size="14pt" ShowExportControls="False">
<LocalReport ReportPath="~/RPTReports/Report1.rdlc"></LocalReport>
</rsweb:ReportViewer>            
        <br />    
    </form>

    
</body>
</html>
