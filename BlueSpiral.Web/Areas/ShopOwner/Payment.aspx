<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Payment.aspx.cs" Inherits="RazorpaySampleApp.Payment" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="width=device-width">
    <title>Razorpay .Net Sample App</title>
</head>
<body>
    <form action="#" method="post">
<button id="rzp-button1">Pay</button>
<script src="https://checkout.razorpay.com/v1/checkout.js"></script>
<script>
var options = {
    "key": "rzp_test_xCoAjLvQqtMBTa", // Enter the Key ID generated from the Dashboard
   // "amount": "100", // Amount is in currency subunits. Default currency is INR. Hence, 50000 refers to 50000 paise
    //"currency": "INR",
    "name": "The Blue Spiral",
    "description": "Test Transaction",
    "image": "http://www.thebluespiral.com//Content/img/logo/logo-new.png",
  //  "order_id": "order_9A33XWu170gUtm", //This is a sample Order ID. Pass the `id` obtained in the response of Step 1
    "callback_url": "http://localhost:55659/ShopOwner/ShopOwner/OrderList",
    "prefill": {
        "name": "Faisal Farooqui",
        "email": "faisal.farooqui@example.com",
        "contact": "9044771277"
    },
    "notes": {
        "address": "Razorpay Corporate Office"
    },
    "theme": {
        "color": "#111e6c"
    }
};
var rzp1 = new Razorpay(options);
document.getElementById('rzp-button1').onclick = function(e){
    rzp1.open();
    e.preventDefault();
}
</script>
</form>
</body>
</html>
