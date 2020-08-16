function GetOrder(OrdeNo) {
    var sessionValue = $("#hdnSession").data('value');
   
    $.ajax({
        async: false,

        type: "POST",

        url: "/ShopOwner/ShopOwner/GetOrderDetails",

        data: { OrdeNo: OrdeNo },

        contenttype: "application/json; charset=utf-8",

        datatype: "json",

        success: function (data) {
            var InnerstrBuilderMain = "";
            var totalprice = 0;
            $("#totprice").empty();
            $("#orderdetail").empty();
            if (data.length > 0) {
               
                for (var i = 0; i < data.length; i++) {

                    InnerstrBuilderMain += "<tr>";
                    InnerstrBuilderMain += "<td>";
                    InnerstrBuilderMain += "<div class='product-item'>";
                    InnerstrBuilderMain += "<a class='product-thumb' href='#'><img src=" + data[i].ImgUrl + " alt='Product'></a>";
                    InnerstrBuilderMain += "<div class='product-info'>";
                    InnerstrBuilderMain += "<h4 class='product-title'><a href='shop-single.html'>" + data[i].ProductName + " <small>x " + data[i].Quantity + "</small></a></h4>";
                    InnerstrBuilderMain += "</div>";
                    InnerstrBuilderMain += "</div>";
                    InnerstrBuilderMain += " </td>";
                    InnerstrBuilderMain += " <td class='text-center text-lg text-medium'>" + data[i].OrderType + "</td>";
                    if (sessionValue != "3") {
                        InnerstrBuilderMain += " <td class='text-center text-lg text-medium'><a href='/ControlPanel/ControlPanel/getprofile?id=" + data[i].VendorId + "'>" + data[i].VendorId + "</a></td>";

                        InnerstrBuilderMain += " <td class='text-center text-lg text-medium'>Rs-" + data[i].Price + "</td>";
                    }
                    InnerstrBuilderMain += "</tr>"
                    totalprice = totalprice + parseFloat(data[i].Price);
                }
                
            }
            $("#totprice").append("Rs: "+totalprice);
            $('#orderdetail').append(InnerstrBuilderMain);

        },
        error: OnErrorCall



    });
}
function DisplayMenu() {

    $.ajax({
        async: false,

        type: "POST",

        url: "/ShopOwner/ShopOwner/GetMainMenu",


        contenttype: "application/json; charset=utf-8",

        datatype: "json",

        success: OnSuccessCallMenu,

        error: OnErrorCall

    });

}
function RemoveImage(id) {
    var redurl = GetURLParameter();
   
    $.ajax({
        async: false,

        type: "POST",

        url: "/ControlPanel/ControlPanel/RemoveImage",

        data: { id: id, redurl: redurl },

        contenttype: "application/json; charset=utf-8",

        datatype: "json",

        success: function () {
            //alert("Yes");
            //var a = GetURLParameter();
            
            //alert(a);
            //alert("http://localhost:55659/ControlPanel/ControlPanel/UploadFiles/" + GetURLParameter());
            //window.location(b);
            //window.location = "http://beta.thebluespiral.com/ShopOwner/ShopOwner/OrderList";
            //var redirecturl = a.concat(b);
            //alert(redirecturl);
            //window.location = redirecturl;
            
        },

        error: OnErrorCall

    });

}

function OnSuccessCallMenu(data) {
    debugger;
    var strBuilderMain = "";
    for (var i = 0; i < data.length; i++) {


        //<li class="has-children">
        strBuilderMain += "<li class='has-children'>";

        //    <span><a href="#" > Men's Shoes</a><span class="sub-menu-toggle"></span></span>
        strBuilderMain += "  <span><a href='#' >" + data[i].HandicraftCategory + "</a><span class='sub-menu-toggle'></span></span>";
     
        strBuilderMain += SubMenu(data[i].HandicraftCategoryId);
       
        
        strBuilderMain += "</li>";

        


    }
    $('#mobilemenulist').append(strBuilderMain);
    $('#menulist').append(strBuilderMain);
  
    

}

function SubMenu(MenuId) {
   
   
    var InnerstrBuilderMain = "";
    //var pageUrl = '<%=ResolveUrl("~/AjaxService.asmx")%>'
    // debugger;
    $.ajax({
        async: false,

        type: "POST",

        url: "/ShopOwner/ShopOwner/GetSubMenus",
        
        data: {MenuId: MenuId},

        contenttype: "application/json; charset=utf-8",

        datatype: "json",

        success: function (data) {
            //$("#temp").empty();
            if (data.length > 0) {

                //<ul class="offcanvas-submenu">
                InnerstrBuilderMain += " <ul class='offcanvas-submenu'>";
                for (var j = 0; j < data.length; j++) {

                    //if (response.d[j]["Url"] != null) {
                    InnerstrBuilderMain += "<li ><a href='http://beta.thebluespiral.com/ShopOwner/ShopOwner/shop/" + data[j].HandicraftSubCategoryId + "'>" + data[j].HandicraftSubCategory + "</a></li>";
                    //  }
                }
                InnerstrBuilderMain += "</ul>";
            }

        },
        error: OnErrorCall

        

    });
    return InnerstrBuilderMain;
}

function OnErrorCall(response) {
    debugger;
    alert(response.d.length);
    alert(response.status + " " + response.statusText);

}

function addtocart(OrderType, signal, Productid) {

    if (signal == 0) {
        var Quantity = $("#quantity").val();
    }

    if (signal > 0) {
        var Quantity = signal;
    }
    $.ajax({
        async: false,

        type: "POST",

        url: "/ShopOwner/ShopOwner/addtocart",

        data: { Productid: Productid, Quantity: Quantity, OrderType: OrderType },

        contenttype: "application/json; charset=utf-8",

        datatype: "json",

        success: function (data) {
            //$("#temp").empty();
            if (data.length > 0) {

                ////<ul class="offcanvas-submenu">
                //InnerstrBuilderMain += " <ul class='offcanvas-submenu'>";
                //for (var j = 0; j < data.length; j++) {

                //    //if (response.d[j]["Url"] != null) {
                //    InnerstrBuilderMain += "<li ><a href='../ShopOwner/shop/" + data[j].HandicraftSubCategoryId + "'>" + data[j].HandicraftSubCategory + "</a></li>";
                //    //  }
                //}
                //InnerstrBuilderMain += "</ul>";
                alert("Product added to cart.");
            }

        },
        error: OnErrorCall



    });
    
}
function updatetocart(moq, orderid, Quantity) {
    //var orderid = parseInt($("#hdnid").val());
    //var Quantity = parseInt($("#quantity").val());

    if (Quantity >= parseInt(moq))
    {
        $.ajax({
            async: false,

            type: "POST",

            url: "/ShopOwner/ShopOwner/updatetocart",

            data: { orderid: orderid, Quantity: Quantity },

            contenttype: "application/json; charset=utf-8",

            datatype: "json",

            success: function (data) {
                if (data.length > 0) {
                    alert("Cart updated successfully.");
                    window.location.replace("Cart");
                }

            },
            error: OnErrorCall



        });
    }
    else
    {
        alert("Quantity cannot be less than the minimum order quantity.");
        //$("#quantity").val(moq);
        window.location.replace("Cart");

    }


}
function PlaceOrder(Status, CreditStatus) {
    var orderid = $("#hdnid").val();
    var deliveryid = $("#deliveryaddressid").val();
    if (deliveryid != "") {
        $.ajax({
            async: false,

            type: "POST",

            url: "/ShopOwner/ShopOwner/PlaceOrder",

            data: { orderid: orderid, Status: Status, CreditStatus: CreditStatus, deliveryid: deliveryid },

            contenttype: "application/json; charset=utf-8",

            datatype: "json",

            success: function (data) {
                if (data.length > 0) {
                    alert("Order placed successfully.");

                    window.location.replace("http://localhost:55659/Areas/ShopOwner/Payment.aspx?id=" + data[0].OrderId);
                }

            },
            error: OnErrorCall



        });
    }
    else {

        alert("Please Select Delivery Address");
    }


}


function PlaceSampleOrder() {

    var quantity = $("#quantity").val();
    var productid = $("#hdnproductid").val();
    $.ajax({
        async: false,

        type: "POST",

        url: "/ShopOwner/ShopOwner/PlaceSampleOrder",

        data: { quantity: quantity, productid: productid },

        contenttype: "application/json; charset=utf-8",

        datatype: "json",

        success: function (data) {
            if (data.length > 0) {
                alert("Sample order placed successfully.");
                window.location = "http://beta.thebluespiral.com/ShopOwner/ShopOwner/OrderList";
            }

        },
        error: OnErrorCall



    });


}
//=================================Display Top 6 Categories=====================================
function DisplayTopCategories() {

    $.ajax({
        async: false,

        type: "POST",

        url: "/ShopOwner/ShopOwner/GetTopCategories",


        contenttype: "application/json; charset=utf-8",

        datatype: "json",

        success: OnSuccessCallTopCategories,

        error: OnErrorCall

    });

}

function OnSuccessCallGetTopSaleCategories(data) {
    alert("yes");
    var strBuilderMain = "";
    for (var i = 0; i < data.length; i++) {

        if (i < 6) { 


        strBuilderMain += "<li><a href='../ShopOwner/shop/" + data[j].HandicraftSubCategoryId + "'>" + data[i].HandicraftSubCategory + "</a></li>";

    }



    }
    alert(strBuilderMain);
    $('#topsalecategories').append(strBuilderMain);

}
function DisplayTopSaleCategories() {

    $.ajax({
        async: false,

        type: "POST",

        url: "/ShopOwner/ShopOwner/GetTopSaleCategories",


        contenttype: "application/json; charset=utf-8",

        datatype: "json",

        success: OnSuccessCallGetTopSaleCategories,

        error: OnErrorCall

    });

}

function OnSuccessCallTopCategories(data) {
    debugger;
    var strBuilderMain = "";
    strBuilderMain += "<ul class='sub-menu'>";
    for (var i = 0; i < data.length; i++) {

        if (i < 6) {


            strBuilderMain += "<li><a href='../ShopOwner/shop/" + data[j].HandicraftSubCategoryId + "'>" + data[i].HandicraftSubCategory + "</a></li>";

        }



    }
    strBuilderMain += "</ul>";
    $('#latestcategories').append(strBuilderMain);

}
//====================================End====================================================
function UpdateOrderStatus(orderid) {
    var orderstatus = $("#status option:selected").text();
    alert(orderid);
    alert($("#status option:selected").text());
    alert(orderstatus);
    var returnurl = GetURLParameter();
    $.ajax({
        async: false,

        type: "POST",

        url: "/ControlPanel/ControlPanel/UpdateOrderStatus",

        data: { orderid: orderid, orderstatus: orderstatus, returnurl: returnurl },

        contenttype: "application/json; charset=utf-8",

        datatype: "json",

        success: function (data) {
            
           // if (data.length > 0) {
                //alert("Status updated successfully.");
                window.location.replace("OrderList");
          //  }

        },
        error: OnErrorCall



    });


}

function getproductdetails(id, stock) {
    if (stock == "nonstock") {

        $("#divnonstock").hide();

    }
    else {
        $("#divnonstock").show();

    }
    $.ajax({
        async: false,

        type: "POST",

        url: "/ControlPanel/ControlPanel/GetProductdetails",


        contenttype: "application/json; charset=utf-8",
        data: { productid: id },

        datatype: "json",

        success: OnSuccessFillProduct,

        error: OnErrorCall

    });
}
function OnSuccessFillProduct(data) {
    $("#VendorId").val(data[0].VendorId);
    $("#product_id").val(data[0].product_id);
    $("#HandicraftCategoryId").val(data[0].HandicraftCategoryId);
    $("#hdnsubcategory").val(data[0].HandicraftSubCategoryId);
   
  
    
        $("#product_name").val(data[0].product_name);
        $("#product_description").val(data[0].product_description);
        $("#product_size").val(data[0].product_size);
        $("#price_defence").val(data[0].price_defence);
        $("#price_shopkeeper").val(data[0].price_shopkeeper);
        $("#product_stock").val(data[0].product_stock);
        $("#moq_defence").val(data[0].moq_defence);
    $("#Material").val(data[0].Material);
    $("#SamplePrice").val(data[0].SamplePrice);
        $("#moq_shopkeeper").val(data[0].moq_shopkeeper);
        $("#discount").val(data[0].discount);
   
    CategoryWiseSubCategory();

}
function validatemoq(moq) {
    var quantity = parseInt($("#quantity").val());
    if (quantity < parseInt(moq)) {
        alert("Quantity cannot be less than Minimum Order Quantity");
        $("#quantity").val(moq);

    }
}
function GetURLParameter() {
    var sPageURL = window.location.href;
    var indexOfLastSlash = sPageURL.lastIndexOf("/");

    if (indexOfLastSlash > 0 && sPageURL.length - 1 != indexOfLastSlash)
        return sPageURL.substring(indexOfLastSlash + 1);
    else
        return 0;
}
function ValidatePswd() {
    var pswd = $("#Pswd").val();
    var cnfpswd = $("#ConfirmPassword").val();
    if (pswd != cnfpswd) {
        alert("Passwords Do not match.")
        $("#Pswd").val("");
        $("#ConfirmPassword").val("");

    }
}
function AssignDeliveryAddress(AddressId) {
    var OldAddresId = $("#deliveryaddressid").val();
    $("#deliveryaddressid").val(AddressId);
    var OldDiv = document.getElementById("div/" + OldAddresId + ""); 
    var div = document.getElementById("div/" + AddressId+"");
    OldDiv.classList.remove("deliver-white");
    // OldDiv.style.backgroundColor = "#f2edf3";
    div.classList.add("deliver-white");
    //div.style.backgroundColor = "#fff";
   

}
function DailyOffer() {
    var text = document.getElementById("TimeDuration");
    if ($("#chkdailyoffer").prop("checked") == true) {
        text.style.display="block";
        }
    else if ($("#chkdailyoffer").prop("checked") == false) {
        $("#TimeDuration").val("");
        text.style.display = "none";
        }
}

function GetLastScheduledDate() {
    $.ajax({
        async: false,

        type: "POST",

        url: "/ControlPanel/ControlPanel/LastToDate",

        //data: { quantity: quantity, productid: productid },

        contenttype: "application/json; charset=utf-8",

        datatype: "json",

        success: function (data) {
           
            if (data.length > 0) {
                var lastDate = data[0].FromDate.trim();              
                $("#FromDate").val(lastDate);
                $("#FromDate").prop('readonly', true);
            }

        },
        error: OnErrorCall



    });


}
function CalcWalletDebit() {
    $.ajax({
        async: false,

        type: "POST",

        url: "/ShopOwner/ShopOwner/ValidateWallet",

        //data: { Amount: deliveryid },

        contenttype: "application/json; charset=utf-8",

        datatype: "json",

        success: function (data) {
            if (data.length > 0) {

                var GrandTotal = parseFloat($("#spngrandtotal").text());
                var WalletAmount = $("#txtwalletamount").val();
                if (WalletAmount <= data[0].Amount) {
                    alert(GrandTotal);
                    if (WalletAmount < GrandTotal) {
                        
                        GrandTotal = GrandTotal - WalletAmount;
                        $("#spngrandtotal").text("Rs: " + GrandTotal);
                    }
                    else {
                        alert("Please make sure Wallet Amount should be less than Grand Total");

                    }
                }
                else {
                    alert("Insufficient Amount");
                    $("#txtwalletamount").val("0");

                }
            }

        },
        error: OnErrorCall


    });
}
function GetWalletAmount() {
    $.ajax({
        async: false,

        type: "POST",

        url: "/ShopOwner/ShopOwner/ValidateWallet",

        //data: { Amount: deliveryid },

        contenttype: "application/json; charset=utf-8",

        datatype: "json",

        success: function (data) {
            if (data.length > 0) {

                $("#spnwalletamount").text("Rs: " + data[0].Amount);
            }

        },
        error: OnErrorCall


    });
}
function FetchSmallCart() {

    $.ajax({
        async: false,

        type: "POST",

        url: "/ShopOwner/ShopOwner/CartDetails",

       // data: { UserId: OrdeNo },

        contenttype: "application/json; charset=utf-8",

        datatype: "json",

        success: function (data) {
            var InnerstrBuilderMain = "";
            var totalitem = 0;
            $("#cartcount").empty();
            $("#divcart").empty();
            if (data.length > 0) {

                for (var i = 0; i < data.length; i++) {

                    InnerstrBuilderMain += "<div class='dropdown-product-item'>";
                    InnerstrBuilderMain += "<span class='dropdown-product-remove'></span> <a class='dropdown-product-thumb' href='shop-single.html'><img src=" + data[i].ImgUrl + " alt='Product'></a>";
                    InnerstrBuilderMain += " <div class='dropdown-product-info'><a class='dropdown-product-title' href='shop-single.html'>" + data[i].ProductName + "</a><span class='dropdown-product-details'>" + data[i].Quantity + " x Rs-" + data[i].Price + "</span></div>";
                    InnerstrBuilderMain += "</div>";
                
                    
                }

            }
            totalitem = parseFloat(data[0].TotalAmount);
            $("#cartcount").append(totalitem);
            $('#divcart').append(InnerstrBuilderMain);

        },
        error: OnErrorCall



    });

}
//divcart
//<div class="dropdown-product-item">
//<span class="dropdown-product-remove"><i class="icon-cross"></i></span> <a class="dropdown-product-thumb" href="shop-single.html"><img src="../../Content/img/cart-dropdown/01.jpg" alt="Product"></a>
//    <div class="dropdown-product-info"><a class="dropdown-product-title" href="shop-single.html">Unionbay Park</a><span class="dropdown-product-details">1 x Rs-43.90</span></div>
//                            </div>