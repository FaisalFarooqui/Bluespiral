using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;

namespace BlueSpiral.Web
{
    public partial class PrintInvoice : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                //string AccountNo = Request.QueryString["AccountNo"].ToString();

                String OrderId = Request.QueryString["Order"].ToString();
                DataTable dtOrderDetails = GetOrderDetails(OrderId);
                DataTable dtOrderList = GetOrderList(OrderId);

                if (dtOrderDetails.Rows.Count > 0)
                {
                    ReportViewer1.ProcessingMode = ProcessingMode.Local;
                    ReportViewer1.LocalReport.EnableExternalImages = true;
                    ReportViewer1.LocalReport.ReportPath = Server.MapPath("~/RDLC/Invoice.rdlc");
                    ReportDataSource rd1 = new ReportDataSource("DataSet1", dtOrderDetails);
                    ReportDataSource rd2 = new ReportDataSource("DataSet2", dtOrderList);
                    ReportViewer1.LocalReport.DataSources.Clear();
                    ReportViewer1.LocalReport.DataSources.Add(rd1);
                    ReportViewer1.LocalReport.DataSources.Add(rd2);

                    // ReportViewer1.LocalReport.DataSources.Add(rds1);
                    ReportViewer1.LocalReport.EnableExternalImages = true;
                    //ReportViewer1.LocalReport.Refresh();
                }
                //Code For Download Direct PDF    

                Warning[] warnings;
                string[] streamIds;
                string mimeType = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                byte[] file = ReportViewer1.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);

                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "inline;filename=" + DateTime.Now.ToString("ddMMyyyyHHss") + ".pdf");
                Response.Buffer = true;
                Response.Clear();
                Response.BinaryWrite(file);
                //System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
                //byte[] bytes = memoryStream.ToArray();
                //System.IO.File.WriteAllBytes(Server.MapPath("~/AdminPanel/Docs/") + DateTime.Now.ToString("ddMMyyyyHHss") + ".pdf", file);
                //memoryStream.Close();
                //string path = "/Users/Faisal/Desktop/Flyaway Corporation";
                //SavePDF(ReportViewer1, path);


                //Response.Redirect("~/AdminPanel/test.aspx?id=" + path);
                Response.End();


            }
        }
        public void SavePDF(ReportViewer viewer, string savePath)
        {
            byte[] Bytes = viewer.LocalReport.Render(format: "PDF", deviceInfo: "");

            using (FileStream stream = new FileStream(savePath, FileMode.Create))
            {
                stream.Write(Bytes, 0, Bytes.Length);
            }
        }
        private DataTable GetOrderDetails(string OrderNo)
        {
            SqlConnection con = new SqlConnection("data source=103.235.105.40;initial catalog=Db_BlueSpiral;user id=BlueSpiral;password=9Ns3?vh4");

            string query = "SELECT OD.Id,OD.OrdeNo,convert(char,OD.EntryDate,103) as EntryDate,OD.TotalAmount,OD.PackagingCharge,OD.ServiceCharge,OD.GrandTotal,U.FName,U.LName,AD.ContactAddress,C.CityName,S.StateName,AD.Pincode FROM TR_OrderDetails OD JOIN dbo.TR_Address AD ON(OD.DeliveryId = AD.Id) JOIN dbo.MS_City C ON(AD.CityId = C.CityId) JOIN dbo.MS_State S ON(AD.StateId = S.StateId) JOIN dbo.TR_Users U ON(AD.EmailId = U.Email) WHERE OD.OrdeNo = '" + OrderNo+"'";

            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch
            {
                dt = null;
            }
            return dt;

        }
        private DataTable GetOrderList(string OrderNo)
        {
            SqlConnection con = new SqlConnection("data source=103.235.105.40;initial catalog=Db_BlueSpiral;user id=BlueSpiral;password=9Ns3?vh4");

            string query = "SELECT replace((isnull((SELECT TOP 1 ImgUrl FROM TR_ProductImages WHERE ProductId=TRO.ProductId),'../../../Uploads/na.jpg')),'../../../','http://www.thebluespiral.com/') AS ImgUrl,TRP.product_name AS ProductName,TRO.Quantity,TRO.OrderType,TRO.FinalAmount as Price,TRO.VendorId FROM TR_Order AS TRO JOIN dbo.TR_Product TRP ON(TRO.ProductId = TRP.product_id)  WHERE TRO.OrderNo='" + OrderNo+"'  ORDER BY  TRO.DateofPurchase";

            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            try
            {
                da.Fill(dt);
            }
            catch
            {
                dt = null;
            }
            return dt;

        }
    }
}