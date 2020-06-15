using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BlueSpiral.Web.Areas.ShopOwner.Models
{
    
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
    public class SubCategoryViewModel
    {
        public List<SubCategoryList> SubCategoryList { get; set; }
        [Required]
        [Display(Name = "Sub Category Name")]
        public string HandicraftCategoryId { get; set; }
        public string HandicraftSubCategory { get; set; }

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

}