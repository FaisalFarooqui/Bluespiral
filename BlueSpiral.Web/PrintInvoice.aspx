<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintInvoice.aspx.cs" Inherits="BlueSpiral.Web.PrintInvoice" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
         <asp:ScriptManager runat="server" ID="S1"></asp:ScriptManager>
    <div>
        <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt">
            <LocalReport ReportPath="RDLC\Invoice.rdlc">
            </LocalReport>
        </rsweb:ReportViewer> 
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" DataSourceMode="DataSet" ProviderName="System.Data.SqlClient" ></asp:SqlDataSource>
    </div>
    </form>
</body>
</html>
