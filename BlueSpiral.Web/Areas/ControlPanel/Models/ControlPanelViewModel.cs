using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlueSpiral.Web.Areas.ControlPanel.Models
{
    public class ControlPanelViewModel
    {
    }
    public class Product
    {
        public int? product_id { get; set; }
        [Required]
        public string product_name { get; set; }
        [Required]
        public string product_description { get; set; }
        public string product_size { get; set; }
        [Required]
        public decimal price_defence { get; set; }
        public string product_colour { get; set; }
        [Required]
        public decimal price_shopkeeper { get; set; }
        [Required]
        public int product_stock { get; set; }
        [Required]
        public int moq_defence { get; set; }
        [Required]
        public int moq_shopkeeper { get; set; }
        [Required]
        public int HandicraftCategoryId { get; set; }
        [Required]
        public int HandicraftSubCategoryId { get; set; }
        public int product_status { get; set; }

        public int discount_type { get; set; }
        [Required]
        public int discount { get; set; }
       
        public decimal max_discount { get; set; }
        public string EntryUser { get; set; }
        public DateTime EntryDate { get; set; }

        public string VendorId { get; set; }
        [Required]
        public string Material { get; set; }
        public decimal SamplePrice { get; set; }
        public string Supplier { get; set; }
        public string TimeDuration { get; set; }

    }
    public class ProductRepList
    {
        [Required]
        public string product_name { get; set; }
        [Required]
        public string product_description { get; set; }
        public string product_size { get; set; }
        [Required]
        public decimal price_defence { get; set; }
        [Required]
        public decimal price_shopkeeper { get; set; }
        [Required]
        public int product_stock { get; set; }
        [Required]
        public int moq_defence { get; set; }
        [Required]
        public int moq_shopkeeper { get; set; }
        [Required]
        public int HandicraftCategoryId { get; set; }
        [Required]
        public int HandicraftSubCategoryId { get; set; }
        public int product_status { get; set; }
        public int discount_type { get; set; }
        [Required]
        public int? discount { get; set; }
        public decimal max_discount { get; set; }


        public string EntryUser { get; set; }

        public string VendorId { get; set; }

    }
    //public class SupplierList
    //{
    //    public int Userid { get; set; }
    //    public int UserTypeId { get; set; }
    //    public string FName { get; set; }
    //    public string Email { get; set; }
    //    public string ContactNo { get; set; }
    //    public string BusinessName { get; set; }

    //    public bool Status { get; set; }

    //}
    //public class UpdateProfileViewModel
    //{
    //    public List<SupplierList> SupplierList { get; set; }

    //    public string FName { get; set; }
    //    public string LName { get; set; }

    //    public string Regiment { get; set; }

    //    [EmailAddress]
    //    public string Email { get; set; }

    //    public string ContactNo { get; set; }

    //    public string ContactAddress { get; set; }
    //    public int StateId { get; set; }
    //    public int CityId { get; set; }
    //    public string Pincode { get; set; }
    //    public bool VisibilityStatus { get; set; }
    //    public bool Status { get; set; }

    //    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    //    [DataType(DataType.Password)]
    //    public string Pswd { get; set; }

    //    [DataType(DataType.Password)]
    //    [Compare("Pswd", ErrorMessage = "The password and confirmation password do not match.")]
    //    public string ConfirmPassword { get; set; }

    //    public string BusinessName { get; set; }
    //    public Nullable<int> HandicraftCategoryId { get; set; }
    //    public string ManufacturerORWholesaler { get; set; }
    //    public int UserTypeId { get; set; }
    //    public string Users { get; set; }
    //    public Nullable<int> ProductId { get; set; }
    //    public Nullable<decimal> MinPrice { get; set; }
    //    public Nullable<decimal> MaxPrice { get; set; }

    //    public string GSTNo { get; set; }

    //    public string PanNo { get; set; }
    //    public int? BankName { get; set; }
    //    public string BankAccountNumber { get; set; }
    //    public string IFSCCode { get; set; }
    //    public string BankAddress { get; set; }
    //    public string OwnBusiness { get; set; }
    //    public int CreatedBy { get; set; }
    //    public DateTime CreatedOn { get; set; }

    //    //To change label title value  
    //    //[Display(Name = "Upload File")]
    //    public string ImagePath { get; set; }

    //    public HttpPostedFileBase ImageFile { get; set; }
    //    public string PanScanPath { get; set; }
    //    public HttpPostedFileBase panScanFile { get; set; }

    //    public string GSTScanPath { get; set; }
    //    public HttpPostedFileBase gstScanFile { get; set; }

    //    public string CanChqPath { get; set; }
    //    public HttpPostedFileBase cancelledChqScanFile { get; set; }
    //}

    public class ProductList
    {
        public int product_id { get; set; }
        public string HandicraftCategory { get; set; }
        public string HandicraftSubCategory { get; set; }
        [Required]
        public string product_name { get; set; }
        [Required]
        public string product_description { get; set; }
       
        public string ImgUrl { get; set; }
        public decimal price_shopkeeper { get; set; }
        public int moq_shopkeeper { get; set; }

        public string UserInfo { get; set; }
        public string Material { get; set; }
        public decimal SamplePrice { get; set; }
        public string Supplier { get; set; }

    }
    public class ShopProductList
    {
        public int product_id { get; set; }
        [Required]
        public string product_name { get; set; }
        [Required]
        public decimal price_defence { get; set; }

        public decimal price_shopkeeper { get; set; }
        public int discount { get; set; }

        public string ImgUrl { get; set; }
        public decimal TotAmountShopkeeper { get; set; }
        public decimal TotDefence { get; set; }
        public int moq_defence { get; set; }
        public int moq_shopkeeper { get; set; }

    }
    public class SingleProductDetail
    {
        public int product_id { get; set; }
        public string Material { get; set; }
        public string product_size { get; set; }
        public string HandicraftCategory { get; set; }
        public string HandicraftSubCategory { get; set; }
        [Required]
        public string product_name { get; set; }
        public string product_description { get; set; }
        [Required]
        public decimal price_defence { get; set; }
        public decimal price_shopkeeper { get; set; }

        public decimal TotAmountShopkeeper { get; set; }
        public decimal TotDefence { get; set; }
        public int moq_defence { get; set; }
        public int moq_shopkeeper { get; set; }
        [Required]
        public int discount { get; set; }
        public decimal SamplePrice { get; set; }

        public int SampleStatus { get; set; }
        



    }


    public class ProductImageViewModel
    {
        [Required]
        public string ImgUrl { get; set; }
        public string ImageType { get; set; }
        public bool isdeleted { get; set; }
        public DateTime entryDate { get; set; }

        public int? ProductId { get; set; }


    }
    public class SignleProductImage
    {

        public int ImgId { get; set; }
        [Required]
        public string ImgUrl { get; set; }
        


    }
    public class CartViewModel
    {
       public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        public int ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public DateTime Entrydate { get; set; }
        public string EntryUser { get; set; }
        public string OrderType { get; set; }




    }
    public class CartListViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ImgUrl { get; set; }

        public decimal SubPrice { get; set; }
        public int Discount { get; set; }
        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public int moq_defence { get; set; }
        public int moq_shopkeeper { get; set; }
        public decimal TotalAmount { get; set; }




    }
    public class PlaceOrder
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }

        public string message { get; set; }



    }

    public class ProductImages
    {
        public string ImgUrl { get; set; }
        public int ImgId { get; set; }
        public int ProductId { get; set; }


    }
    public class CashBackList
    {
        public Int64 SrNo { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string EmailId { get; set; }
        public decimal TotalPurchase { get; set; }
        public decimal CashBackAmount { get; set; }

        public string EntryDate { get; set; }



    }
    public class OrderListViewModel
    {
        public string OrdeNo { get; set; }

        public string EntryUser { get; set; }
        public string DateOfPurchase { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }




    }
    public class NotOrderListViewModel
    {
        public int Id { get; set; }
        public string OrdeNo { get; set; }
        public decimal? TotalAmount { get; set; }

        public string OrderStatus { get; set; }

        public DateTime? EntryDate { get; set; }
        public string EntryUser { get; set; }
        public string CreditStatus { get; set; }
        public int? DeliveryId { get; set; }


      
    }
    public class TopSubCategoryViewModel
    {
        public int HandicraftSubCategoryId { get; set; }
        public string HandicraftSubCategory { get; set; }
        public string ImgUrl { get; set; }

    }
    public class WalletAmount
    {
        public decimal Amount { get; set; }
       

    }
    public class TopProductViewModel
    {
        public int product_id { get; set; }
        public int moq_defence { get; set; }
        public int moq_shopkeeper { get; set; }
        public string product_name { get; set; }
        public decimal price_defence { get; set; }
        public decimal price_shopkeeper { get; set; }
        public int discount { get; set; }
        public string ImgUrl { get; set; }

        public decimal TotAmountShopkeeper { get; set; }
        public decimal TotDefence { get; set; }


    }
    public class AddressList
    {
        public int Id { get; set; }
      
        public string EmailId { get; set; }
        public string BusinessName { get; set; }

        public string StateName { get; set; }
     
        public string CityName { get; set; }
      
        public string PinCode { get; set; }
        
        public string ContactAddress { get; set; }

    }
}