function StateWiseCity() {
     var StateId = $('#StateId').val();
    $.ajax({
        url: '/Account/StateWiseCityList/',
        type: "GET",
        dataType: "json",
        data: { StateId: StateId },
        contentType: "application/josn",
          success: function (cities) {
              debugger;
              $("#City").html(""); // clear before appending new list 
              $("#City").append(
                   $('<option></option>').val(0).html("Choose City"));
            $.each(cities, function (i, city) {
                $("#City").append(
                    $('<option></option>').val(city.CityId).html(city.CityName));
              });
              if ($("#hdnCityId").val() != null) {
                  $("#City").val($("#hdnCityId").val());
              }
          },
          error: function()
          {
              //OnError();
          }
    });
}

function CategoryWiseSubCategory() {
    debugger;
    var CategoryId = $('#HandicraftCategoryId').val();
    $.ajax({
        url: '/ControlPanel/ControlPanel/CategoryWiseSubcategoryList/',
        type: "GET",
        dataType: "json",
        data: { CategoryId: CategoryId },
        contentType: "application/josn",
        success: function (subcategories) {
            debugger;
            $("#SubCategory").html(""); // clear before appending new list 
            $("#SubCategory").append(
                $('<option></option>').val(0).html("Choose Sub Category"));
            $.each(subcategories, function (i, subcategory) {
                $("#SubCategory").append(
                    $('<option></option>').val(subcategory.HandicraftSubCategoryId).html(subcategory.HandicraftSubCategory));
            });
            if ($("#hdnsubcategory").val() != null) {
                $("#SubCategory").val($("#hdnsubcategory").val());
            }
        },
        error: function () {
            //OnError();
        }
    });
}

 function checkEmail() {
     var emailid = $('#emailid').val();
    $.ajax({
        url: '/Account/CheckEmailId/',
        type: "GET",
        dataType: "json",
        data: { emailid: emailid },
        contentType: "application/josn",
          success: function (isExist) {
              debugger;
              //$("#City").html(""); // clear before appending new list 
              //$("#City").append(
              //     $('<option></option>').val(0).html("Choose City"));
              $.each(isExist, function (i, validCheck) {
                  alter('Exist');
            });
          },
          error: function()
          {
              //OnError();
          }
    });
  }
