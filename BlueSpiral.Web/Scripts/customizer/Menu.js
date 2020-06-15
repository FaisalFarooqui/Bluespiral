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

function OnSuccessCallMenu(data) {
    
    var strBuilderMain = "";
    for (var i = 0; i < data.length; i++) {


        //<li class="has-children">
        strBuilderMain += "<li class='has-children'>";

        //    <span><a href="#" > Men's Shoes</a><span class="sub-menu-toggle"></span></span>
        strBuilderMain += "  <span><a href='#' >" + data[i].HandicraftCategory + "</a><span class='sub-menu-toggle'></span></span>";
     
        strBuilderMain += SubMenu(data[i].HandicraftCategoryId);
       
        
        strBuilderMain += "</li>";

        


    }
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
            debugger;
            //$("#temp").empty();
            if (data.length > 0) {

                //<ul class="offcanvas-submenu">
                InnerstrBuilderMain += " <ul class='offcanvas-submenu'>";
                for (var j = 0; j < data.length; j++) {

                    //if (response.d[j]["Url"] != null) {
                    InnerstrBuilderMain += "<li ><a href='../ShopOwner/shop/" + data[j].HandicraftSubCategoryId + "'>" + data[j].HandicraftSubCategory + "</a></li>";
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
function addtocart(OrderType) {

    alert(OrderType)
    var InnerstrBuilderMain = "";
    //var pageUrl = '<%=ResolveUrl("~/AjaxService.asmx")%>'
    // debugger;
    var Productid = GetURLParameter();
    var Quantity = $("#quantity").val();
    $.ajax({
        async: false,

        type: "POST",

        url: "/ShopOwner/ShopOwner/addtocart",

        data: { Productid: Productid, Quantity: Quantity, OrderType: OrderType },

        contenttype: "application/json; charset=utf-8",

        datatype: "json",

        success: function (data) {
            debugger;
            //$("#temp").empty();
            if (data.length > 0) {

                //<ul class="offcanvas-submenu">
                InnerstrBuilderMain += " <ul class='offcanvas-submenu'>";
                for (var j = 0; j < data.length; j++) {

                    //if (response.d[j]["Url"] != null) {
                    InnerstrBuilderMain += "<li ><a href='../ShopOwner/shop/" + data[j].HandicraftSubCategoryId + "'>" + data[j].HandicraftSubCategory + "</a></li>";
                    //  }
                }
                InnerstrBuilderMain += "</ul>";
            }

        },
        error: OnErrorCall



    });
    
}
function updatetocart() {

    var orderid = $("#hdnid").val();
    var Quantity = $("#quantity").val();
    $.ajax({
        async: false,

        type: "POST",

        url: "/ShopOwner/ShopOwner/updatetocart",

        data: { orderid: orderid, Quantity: Quantity },

        contenttype: "application/json; charset=utf-8",

        datatype: "json",

        success: function (data) {
            if (data.length > 0) {

                window.location.replace("Cart");
            }

        },
        error: OnErrorCall



    });


}
function PlaceOrder() {

    var orderid = $("#hdnid").val();
    $.ajax({
        async: false,

        type: "POST",

        url: "/ShopOwner/ShopOwner/PlaceOrder",

        data: { orderid: orderid },

        contenttype: "application/json; charset=utf-8",

        datatype: "json",

        success: function (data) {
            if (data.length > 0) {

                window.location.replace("OrderList");
            }

        },
        error: OnErrorCall



    });


}

function GetURLParameter() {
    var sPageURL = window.location.href;
    var indexOfLastSlash = sPageURL.lastIndexOf("/");

    if (indexOfLastSlash > 0 && sPageURL.length - 1 != indexOfLastSlash)
        return sPageURL.substring(indexOfLastSlash + 1);
    else
        return 0;
}

