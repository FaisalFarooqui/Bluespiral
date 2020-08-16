using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlueSpiral.Web.Areas.ShopOwner.Models
{
    public class DailyOfferViewModel
    {
       public int product_id { get; set; }
       public string product_name { get; set; }
        public string EntryDate { get; set; }
       public string TimeDuration { get; set; }
       public decimal price_defence { get; set; }
       public decimal price_shopkeeper { get; set; }
       public decimal totshopkeeper { get; set; }
       public decimal totdefence { get; set; }
        public int moq_defence { get; set; }
       public int moq_shopkeeper { get; set; }
        public string ImgUrl { get; set; }
       public int discount { get; set; }
    }
    public class CategoryViewModel
    {
        public List<CategoryList> CategoryList { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public string HandicraftCategory { get; set; }
        public bool VisibilityStatus { get; set; }
        
    }
    public class CategoryList
    {
        public int HandicraftCategoryId { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public string HandicraftCategory { get; set; }

    }
    public class SuppliersList
    {
        public string Email { get; set; }
        public string UserInfo { get; set; }

    }
    public class SubCategoryViewModel
    {
        public List<SubCategoryList> SubCategoryList { get; set; }
        [Required]
        [Display(Name = "Sub Category Name")]
        public string HandicraftCategoryId { get; set; }
        public string HandicraftSubCategory { get; set; }
        public string ImgUrl { get; set; }

        public HttpPostedFileBase ImageFile { get; set; }

    }
    public class CashBackViewModel
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

       // public string EmailId { get; set; }

    }
    public class WalletViewModel
    {
        public string UserId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public decimal TotalPurchase { get; set; }
        public decimal CashBackAmount { get; set; }

        // public string EmailId { get; set; }

    }
    public class ColourList
    {
        public int ColourId { get; set; }
        [Required]
        [Display(Name = "Colour Name")]
        public string ColourName { get; set; }

    }
    public class SubCategoryList
    {
        public int HandicraftSubCategoryId { get; set; }
        [Required]
        [Display(Name = "Sub Category Name")]
        public string HandicraftSubCategory { get; set; }
        public string HandicraftCategory { get; set; }

    }
   
    public class Menu
    {
        public int HandicraftCategoryId { get; set; }
        public string HandicraftCategory { get; set; }
        //public int SubMenuExistance { get; set; }


    }
    public class SubMenu
    {
        public int HandicraftCategoryId { get; set; }
        public int HandicraftSubCategoryId { get; set; }
        public string HandicraftSubCategory { get; set; }


    }
    public class OrderDetail
    {
        public string ImgUrl { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string OrderType { get; set; }
        public decimal Price { get; set; }
        public string VendorId { get; set; }


    }
    public class LastScheduledDate
    {
        public string FromDate { get; set; }
        //public int SubMenuExistance { get; set; }


    }

}