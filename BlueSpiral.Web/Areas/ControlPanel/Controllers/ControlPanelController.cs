using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using AutoMapper;
using BlueSpiral.Common.Cryptography;
using BlueSpiral.Core.Repository;
using BlueSpiral.Core.ViewModel;
using BlueSpiral.Data;
using BlueSpiral.Data.Repository;
using BlueSpiral.Web.Areas.ControlPanel.Models;
using BlueSpiral.Web.Areas.ShopOwner.Models;

namespace BlueSpiral.Web.Areas.ControlPanel.Controllers
{
    public class ControlPanelController : Controller
    {
        private readonly Crypto _Crypto;

        private readonly BlueSpiralDBContext _context;
        private readonly IRepository<MS_HandicraftCategory> _HandicraftCategory;
        private readonly IRepository<MS_HandicraftSubCategory> _HandicraftSubCategory;
        private readonly IRepository<TR_Users> _Users;
        private readonly IRepository<MS_Colour> _Colour;
        private readonly IRepository<TR_Product> _Product;
        private readonly IRepository<TR_ProductImages> _ProductImages;
        private readonly IRepository<MS_State> _StateRepo;

        public ControlPanelController()
        {
            _context = new BlueSpiralDBContext();
            _Crypto = new Crypto();
            _HandicraftCategory = new Repository<MS_HandicraftCategory>(_context);
            _HandicraftSubCategory = new Repository<MS_HandicraftSubCategory>(_context);
            _Users = new Repository<TR_Users>(_context);
            _Colour = new Repository<MS_Colour>(_context);
            _Product = new Repository<TR_Product>(_context);
            _ProductImages = new Repository<TR_ProductImages>(_context);
            _StateRepo = new Repository<MS_State>(_context);

        }
        
        // GET: ControlPanel/ControlPanel
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Welcome()
        {
            return View();
        }
        public ActionResult Category()
        {
            var CategoryVMList = new List<CategoryList>();
            var AllCategory = _HandicraftCategory.Find(m => m.VisibilityStatus == true).ToList();
            foreach (var category in AllCategory)
            {
                Mapper.CreateMap<MS_HandicraftCategory, CategoryList>();
                var CategoryData = Mapper.Map<MS_HandicraftCategory, CategoryList>(category);
                CategoryVMList.Add(CategoryData);
            }
            ViewBag.Category = CategoryVMList;
            return View();
        }
        // POST: /ShopOwner/Category
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Category(CategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.VisibilityStatus = true;
                //auto map viewmodel to class
                Mapper.CreateMap<CategoryViewModel, MS_HandicraftCategory>();
                var categories = Mapper.Map<CategoryViewModel, MS_HandicraftCategory>(model);

                //save category data in db
                _HandicraftCategory.Add(categories);
                var categoryid = _HandicraftCategory.Save();

                if (categoryid > 0)
                {



                    return RedirectToAction("Category", "ControlPanel");
                }
                else
                    return View(model);
            }
            return View();

        }
        public ActionResult RemoveCategory(int? id)
        {
            //Finding row to be updated on specific Id
            var categoryDetails = _HandicraftCategory.Find(CategoryId => CategoryId.HandicraftCategoryId == id).FirstOrDefault();

            //Updating Visibility status
            categoryDetails.VisibilityStatus = false;
            _HandicraftCategory.Update(categoryDetails);

            //save category data in db
            var categoryid = _HandicraftCategory.Save();

            if (categoryid > 0)
            {

                return RedirectToAction("Category", "ControlPanel");
            }
            else
                return View();

        }
       
        public ActionResult UserList(int? id)
        {
            var SupplierVMList = new List<SupplierList>();
            var AllSupplier = _Users.Find(m => m.VisibilityStatus == true && m.UserTypeId== id).ToList();
            foreach (var supplier in AllSupplier)
            {
                Mapper.CreateMap<TR_Users, SupplierList>();
                var SupplierData = Mapper.Map<TR_Users, SupplierList>(supplier);
                SupplierVMList.Add(SupplierData);
            }
            ViewBag.SupplierList = SupplierVMList;
            ViewBag.State = StateList();
            return View();
        }
        public SelectList StateList()
        {
            var StateVMList = new List<StateViewModel>();
            var AllState = _StateRepo.Find(m => m.CountryId == 113).ToList();
            foreach (var state in AllState)
            {
                Mapper.CreateMap<MS_State, StateViewModel>();
                var StateData = Mapper.Map<MS_State, StateViewModel>(state);
                StateVMList.Add(StateData);
            }
            var StateList = new SelectList(StateVMList, "StateId", "StateName");
            return StateList;
        }
        public ActionResult UpdateStatus(int? id)
        {
            //Finding row to be updated on specific Id
            var userDetails = _Users.Find(m => m.UserId == id).FirstOrDefault();
            int Usertypeid = userDetails.UserTypeId;
            //Updating Visibility status
            if (userDetails.Status == true)
            {
                userDetails.Status = false;
            }
            else
            {
                userDetails.Status = true;
            }
            _Users.Update(userDetails);

            //save category data in db
            var statusid = _Users.Save();

            if (statusid > 0)
            {

                return RedirectToAction("UserList/"+Usertypeid, "ControlPanel");
            }
            else
                return View();

        }
        public ActionResult RemoveUser(int? id)
        {
            //Finding row to be updated on specific Id
            var userDetails = _Users.Find(m => m.UserId == id).FirstOrDefault();
            int Usertypeid = userDetails.UserTypeId;
            //Updating Visibility status
            userDetails.VisibilityStatus = false;
            _Users.Update(userDetails);

            //save category data in db
            var statusid = _Users.Save();

            if (statusid > 0)
            {

                return RedirectToAction("UserList/" + Usertypeid, "ControlPanel");
            }
            else
                return View();

        }
    
        
        public ActionResult Subcategory()
        {
            SubCategoryViewModel ObjReq = new SubCategoryViewModel();
            List<SubCategoryList> vData = null;
            string query = "SELECT SC.HandicraftSubCategoryId,SC.HandicraftSubCategory,C.HandicraftCategory FROM MS_HandicraftSubCategory SC JOIN MS_HandicraftCategory C ON (SC.HandicraftCategoryId=C.HandicraftCategoryId) WHERE SC.VisibilityStatus=1";
          
            vData = _context.Database.SqlQuery<SubCategoryList>(query).ToList();


            ViewBag.SubCategory = vData;
            ViewBag.Category = CategoryList();
            return View();
        }
        public SelectList CategoryList()
        {
            var CategoryVMList = new List<CategoryList>();
            var AllCategory = _HandicraftCategory.Find(m => m.VisibilityStatus == true).ToList();
            foreach (var Category in AllCategory)
            {
                Mapper.CreateMap<MS_HandicraftCategory, CategoryList>();
                var StateData = Mapper.Map<MS_HandicraftCategory, CategoryList>(Category);
                CategoryVMList.Add(StateData);
            }
            var StateList = new SelectList(CategoryVMList, "HandicraftCategoryId", "HandicraftCategory");
            return StateList;
        }
        public SelectList ColourList()
        {
            var ColourVMList = new List<ColourList>();
            var AllColour = _Colour.Find(m => m.VisibilityStatus == true).ToList();
            foreach (var Colour in AllColour)
            {
                Mapper.CreateMap<MS_Colour, ColourList>();
                var StateData = Mapper.Map<MS_Colour, ColourList>(Colour);
                ColourVMList.Add(StateData);
            }
            var StateList = new SelectList(ColourVMList, "ColourId", "ColourName");
            return StateList;
        }
        
        [HttpGet]
        public ActionResult CategoryWiseSubcategoryList(int CategoryId)
        {
            int intCategoryId = Convert.ToInt32(CategoryId);
            var SubVMList = new List<SubCategoryList>();
            var CategroyWiseSubCategroy = _HandicraftSubCategory.Find(m => m.HandicraftCategoryId == intCategoryId && m.VisibilityStatus==true).ToList();
            foreach (var SubCat in CategroyWiseSubCategroy)
            {
                Mapper.CreateMap<MS_HandicraftSubCategory, SubCategoryList>();
                var CategroyWiseSubCategroyData = Mapper.Map<MS_HandicraftSubCategory, SubCategoryList>(SubCat);
                SubVMList.Add(CategroyWiseSubCategroyData);
            }

            return Json(SubVMList, JsonRequestBehavior.AllowGet);

        }
        // POST: /ControlPanel/Subcategory
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Subcategory(SubCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                //auto map viewmodel to class
                Mapper.CreateMap<SubCategoryViewModel, MS_HandicraftSubCategory>();
                var subcategories = Mapper.Map<SubCategoryViewModel, MS_HandicraftSubCategory>(model);
                subcategories.VisibilityStatus = true;
                //save category data in db
                _HandicraftSubCategory.Add(subcategories);
                var subcategoryid = _HandicraftSubCategory.Save();

                if (subcategoryid > 0)
                {



                    return RedirectToAction("SubCategory", "ControlPanel");
                }
                else
                    return View(model);
            }
            return View();

        }
        public ActionResult RemoveSubCategory(int? id)
        {
            //Finding row to be updated on specific Id
            var SubCategoryDetails = _HandicraftSubCategory.Find(m => m.HandicraftSubCategoryId == id).FirstOrDefault();

            //Updating Visibility status
            SubCategoryDetails.VisibilityStatus = false;
            _HandicraftSubCategory.Update(SubCategoryDetails);

            //save category data in db
            var statusid = _Users.Save();

            if (statusid > 0)
            {

                return RedirectToAction("SubCategory/", "ControlPanel");
            }
            else
                return View();

        }
        [HttpGet]
        public ActionResult Product()
        {
            ProductViewModel ObjReq = new ProductViewModel();
            List<ProductList> vData = null;
            string query = "SELECT product_id,HC.HandicraftCategory,HSC.HandicraftSubCategory,product_name,product_description FROM TR_Product TP JOIN MS_HandicraftCategory HC ON(TP.HandicraftCategoryId = HC.HandicraftCategoryId) JOIN dbo.MS_HandicraftSubCategory HSC ON(TP.HandicraftSubCategoryId = HSC.HandicraftSubCategoryId)";

            vData = _context.Database.SqlQuery<ProductList>(query).ToList();


            ViewBag.ProductList = vData;
            ViewBag.Category = CategoryList();
            ViewBag.Colour = ColourList();
            return View();

        }
        [HttpPost]
        public ActionResult Product(ProductViewModel obj) {
            if (ModelState.IsValid)
            {
                obj.VendorId = Session["UserEmail"].ToString();
                obj.EntryUser= Session["UserEmail"].ToString();
                
                //auto map viewmodel to class
                Mapper.CreateMap<ProductViewModel, TR_Product>();
                var product = Mapper.Map<ProductViewModel, TR_Product>(obj);
               
                //save category data in db
                _Product.Add(product);
                var statusid = _Product.Save();

                if (statusid > 0)
                {
                    return RedirectToAction("Product", "ControlPanel");
                }
                else
                    return View(obj);
            }
            return View();
        }
        public ActionResult productSave(string HandicraftCategory)
        {
            string hccategory = HandicraftCategory;
            CategoryViewModel obj = new CategoryViewModel();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditProduct(ProductViewModel obj)
        {

            return View();
        }
        public ActionResult Logout()
        {
            Session["UserEmail"] = null;
            Session["PhotoUrl"] = null;
            Session["Name"] = null;
            return RedirectToAction("Login", "Account", new { area = "" });

        }
        public ActionResult Cat()
        {
            return View();
        }

        [HttpGet]
        public ActionResult UploadFiles()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadFiles(int? id,List<HttpPostedFileBase> Files)
        {
            var path = "";
            if (Files.Count > 0)
            {
                foreach (var item in Files)
                {
                    if (item != null)
                    {
                        if (item.ContentLength > 0)
                        {
                            if (Path.GetExtension(item.FileName).ToLower() == ".jpg"
                                    || Path.GetExtension(item.FileName).ToLower() == ".png"
                                    || Path.GetExtension(item.FileName).ToLower() == ".gif"
                                    || Path.GetExtension(item.FileName).ToLower() == ".jpeg")
                            {
                                path = Path.Combine(Server.MapPath("~/Uploads"), item.FileName);
                                item.SaveAs(path);
                               
                                ProductImageViewModel PIVM = new ProductImageViewModel();
                                PIVM.ProductId = id;
                                PIVM.ImageType = 1;
                                PIVM.ImgUrl = "../../../Uploads/" + item.FileName;
                                PIVM.isdeleted = false;
                                PIVM.entryDate = DateTime.Now;
                                //auto map viewmodel to class
                                Mapper.CreateMap<ProductImageViewModel, TR_ProductImages>();
                                var proimges = Mapper.Map<ProductImageViewModel, TR_ProductImages>(PIVM);

                                //save category data in db
                                _ProductImages.Add(proimges);
                                var statusid = _ProductImages.Save();
                                if (statusid > 0)
                                {
                                    TempData["msg"] = "Upload Completed Successfull";
                                }
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Product");
        }
    }
}
