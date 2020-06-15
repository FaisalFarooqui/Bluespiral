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
    public class ProductViewModel
    {
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
        public int max_discount { get; set; }
        public string EntryUser { get; set; }

        public string VendorId { get; set; }


    }
    public class ProductList
    {
        public int product_id { get; set; }
        public string HandicraftCategory { get; set; }
        public string HandicraftSubCategory { get; set; }
        [Required]
        public string product_name { get; set; }
        [Required]
        public string product_description { get; set; }
       




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

    }
    public class SingleProductDetail
    {
        public int product_id { get; set; }
        public string HandicraftCategory { get; set; }
        public string HandicraftSubCategory { get; set; }
        [Required]
        public string product_name { get; set; }
        public string product_description { get; set; }
        [Required]
        public decimal price_defence { get; set; }
        public decimal price_shopkeeper { get; set; }
        public int moq_defence { get; set; }
        public int moq_shopkeeper { get; set; }
        [Required]
        public int discount { get; set; }
     




    }
    public class ProductImageViewModel
    {
        [Required]
        public string ImgUrl { get; set; }
        public int ImageType { get; set; }
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
      



    }
    public class PlaceOrder
    {
        public string OrderId { get; set; }
        public string UserId { get; set; }

        public string message { get; set; }



    }
    public class OrderListViewModel
    {
        public string OrdeNo { get; set; }
        public string DateOfPurchase { get; set; }
        public string OrderStatus { get; set; }
        public decimal TotalAmount { get; set; }




    }
}