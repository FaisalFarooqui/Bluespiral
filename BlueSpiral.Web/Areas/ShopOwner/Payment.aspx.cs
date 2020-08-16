using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace RazorpaySampleApp
{
    public partial class Payment : System.Web.UI.Page
    {
        public string orderId;
        protected void Page_Load(object sender, EventArgs e)
        {
            orderId = Request.QueryString["id"].ToString();
            DataTable dt = GetOrderDetails(orderId);
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    decimal Amount = Convert.ToDecimal(dt.Rows[0]["GrandTotal"].ToString()) * 100;
                    Dictionary<string, object> options = new Dictionary<string, object>();
                    options.Add("amount", "100"); // amount in the smallest currency unit
                    options.Add("receipt", "order_rcptid_11");
                    options.Add("currency", "INR");
                    options.Add("key", "rzp_test_xCoAjLvQqtMBTa");
                    options.Add("payment_capture", "0");
                    //options.Add("order_id", dt.Rows[0]["OrdeNo"].ToString());

                    //string key = "rzp_test_xCoAjLvQqtMBTa";
                    //string secret = "psyBQoEoZVNIYJzuIgDyN3vr";

                    //RazorpayClient client = new RazorpayClient(key, secret);
                    //Order order = client.Order.Create(options);
                    //orderId = order["id"].ToString();


                //=================Unneccesary================
                    //Dictionary<string, object> input = new Dictionary<string, object>();
                    //input.Add("amount", 100); // this amount should be same as transaction amount
                    //input.Add("currency", "INR");
                    //input.Add("receipt", "12121");
                    //input.Add("payment_capture", 1);

                    //string key = "rzp_test_pIOZRclmHahY4r";
                    //string secret = "IhY0rivKWehNwglwMW2ak7PF";

                    //RazorpayClient client = new RazorpayClient(key, secret);

                    //Razorpay.Api.Order order = client.Order.Create(input);
                    //orderId = order["id"].ToString();
                }
            }



        }
        private DataTable GetOrderDetails(string OrderNo)
        {
            SqlConnection con = new SqlConnection("data source=103.235.105.40;initial catalog=Db_BlueSpiral;user id=BlueSpiral;password=9Ns3?vh4");

            string query = "SELECT OD.Id,OD.OrdeNo,convert(char,OD.EntryDate,103) as EntryDate,OD.TotalAmount,OD.PackagingCharge,OD.ServiceCharge,OD.GrandTotal,U.FName,U.LName,AD.ContactAddress,C.CityName,S.StateName,AD.Pincode FROM TR_OrderDetails OD JOIN dbo.TR_Address AD ON(OD.DeliveryId = AD.Id) JOIN dbo.MS_City C ON(AD.CityId = C.CityId) JOIN dbo.MS_State S ON(AD.StateId = S.StateId) JOIN dbo.TR_Users U ON(AD.EmailId = U.Email) WHERE OD.OrdeNo = '" + OrderNo + "'";

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