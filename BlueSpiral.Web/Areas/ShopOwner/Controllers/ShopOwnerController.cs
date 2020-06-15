using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using BlueSpiral.Common.Cryptography;
using BlueSpiral.Core.Repository;
using BlueSpiral.Core.ViewModel;
using BlueSpiral.Data;
using BlueSpiral.Data.Repository;
using BlueSpiral.Web.Areas.ControlPanel.Models;
using BlueSpiral.Web.Areas.ShopOwner.Models;

namespace BlueSpiral.Web.Areas.ShopOwner.Controllers
{
    public class ShopOwnerController : Controller
    {
        private readonly Crypto _Crypto;

        private readonly IRepository<MS_State> _StateRepo;
        private readonly IRepository<MS_City> _CityRepo;
        private readonly BlueSpiralDBContext _context;
        private readonly IRepository<MS_HandicraftCategory> _HandicraftCategory;
        private readonly IRepository<TR_Users> _Userd;
        private readonly IRepository<TR_Cart> _Cart;

        public ShopOwnerController()
        {
            _context = new BlueSpiralDBContext();
            _Crypto = new Crypto();

            _StateRepo = new Repository<MS_State>(_context);
            _CityRepo = new Repository<MS_City>(_context);
            _HandicraftCategory = new Repository<MS_HandicraftCategory>(_context);
            _Userd = new Repository<TR_Users>(_context);
            _Cart = new Repository<TR_Cart>(_context);
        }
        // GET: ShopOwner/ShopOwner
        public ActionResult Dashboard()
        {
            Session["Account"] = 1;
            return View();
        }
        //public ActionResult GetMenuList()
        //{
        //    try
        //    {
        //        var result = (from m in objEntity.Menu_Tree
        //                      select new Dynamic_Menu.Models.Menu_List
        //                      {
        //                          M_ID = m.M_ID,
        //                          M_P_ID = m.M_P_ID,
        //                          M_NAME = m.M_NAME,
        //                          CONTROLLER_NAME = CONTROLLER_NAME,
        //                          ACTION_NAME = ACTION_NAME,
        //                      }).ToList();
        //        return View("Menu", result);
        //    }
        //    catch (Exception ex)
        //    {
        //        var error = ex.Message.ToString();
        //        return Content("Error");
        //    }
        //}
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Index2()
        {
            Session["Account"] = 0;
            return View();
        }

        public ActionResult Welcome()
        {
            Session["Account"] = 1;
            return View();
        }
        
        public ActionResult Profile()
        {
            Session["Account"] = 1;
            //transfer Unique value from session
            string Email = Session["UserEmail"].ToString();
            //find details of specific id
            var Profile = _Userd.Find(m => m.Email == Email).FirstOrDefault();
            
                Mapper.CreateMap<TR_Users, UsersViewModel>();
                var ProfileData = Mapper.Map<TR_Users, UsersViewModel>(Profile);
            ProfileData.Pswd = null;
            
            return View(ProfileData);
        }
        // POST: /ShopOwner/Profile
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Profile(UsersViewModel model)
        {
            //Finding row to be updated on specific Id
            var ProfileDetails = _Userd.Find(M => M.Email == model.Email).FirstOrDefault();

            //Updating Visibility status
            ProfileDetails.FName = model.FName;
            ProfileDetails.LName = model.LName;
            ProfileDetails.ContactNo = model.ContactNo;
            ProfileDetails.GSTNo = model.GSTNo;
            if (model.Pswd != null)
            {
                ProfileDetails.Pswd = _Crypto.Encrypt(model.Pswd);
            }
            string UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();
            string FileName = "";
            if (model.ImageFile != null)
            {
                //Use Namespace called :  System.IO  
                FileName = RandomNumber(4, 4).ToString();

                //To Get File Extension  
                string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                //Add Current Date To Attached File Name  
                FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                //Get Upload path from Web.Config file AppSettings.  
                //string UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();

                //Its Create complete path to store in server.  
                model.ImagePath = Server.MapPath(UploadPath) + FileName;

                //To copy and save file into server.  
                model.ImageFile.SaveAs(model.ImagePath);
            }
            _Userd.Update(ProfileDetails);

            //save category data in db
            var statusid = _Userd.Save();

            if (statusid > 0)
            {

                return RedirectToAction("Profile", "ShopOwner");
            }
            else
                return View();

        }
        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        public ActionResult Address()
        {
            Session["Account"] = 1;
            ViewBag.State = StateList();
            
            //transfer Unique value from session
            string Email = Session["UserEmail"].ToString();
            //find details of specific id
            var Profile = _Userd.Find(m => m.Email == Email).FirstOrDefault();
            ViewBag.City = StateWiseCityListType(Profile.StateId);
            Mapper.CreateMap<TR_Users, UsersViewModel>();
            var ProfileData = Mapper.Map<TR_Users, UsersViewModel>(Profile);


            return View(ProfileData);
        }
        // POST: /ShopOwner/Profile
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Address(UsersViewModel model)
        {
            string Email = Session["UserEmail"].ToString();

            //Finding row to be updated on specific Id
            var ProfileDetails = _Userd.Find(M => M.Email == Email).FirstOrDefault();

            //Updating Visibility status
            ProfileDetails.BusinessName = model.BusinessName;
            ProfileDetails.StateId = model.StateId;
            ProfileDetails.CityId = model.CityId;
            ProfileDetails.Pincode = model.Pincode;
            ProfileDetails.ContactAddress = model.ContactAddress;
           
            _Userd.Update(ProfileDetails);

            //save category data in db
            var statusid = _Userd.Save();

            if (statusid > 0)
            {

                return RedirectToAction("Address", "ShopOwner");
            }
            else
                return View();
        }
        public ActionResult OrderList()
        {
            List<OrderListViewModel> vData = null;
            string Query = "SELECT OrdeNo,convert(CHAR,EntryDate,106) AS DateOfPurchase,OrderStatus,TotalAmount FROM TR_OrderDetails";
            vData = _context.Database.SqlQuery<OrderListViewModel>(Query).ToList();
            ViewBag.OrderList = vData;
            Session["Account"] = 1;
            return View();
        }
        public ActionResult PurchaseHistory()
        {
            Session["Account"] = 1;
            return View();

        }
        public ActionResult PaymentDetails()
        {
            Session["Account"] = 1;
            return View();
        }
        public ActionResult Wallet()
        {
            Session["Account"] = 1;
            return View();

        }
        public ActionResult Logout()
        {
            Session["Account"] = 0;
            Session["UserEmail"] = null;
            Session["PhotoUrl"] = null;
            Session["Name"] = null;
            return RedirectToAction("Login", "Account", new { area = "" });

        }
        public ActionResult Shop(int? id)
        {
            ShopProductList ObjReq = new ShopProductList();
            List<ShopProductList> vData = null;
            string query = "SELECT TR.product_id,TR.product_name,TR.price_defence,TR.price_shopkeeper,TR.discount,TRI.ImgUrl FROM TR_Product TR JOIN TR_ProductImages TRI ON (TR.product_id=TRI.ImgId) WHERE HandicraftSubCategoryId="+ id + "";

            vData = _context.Database.SqlQuery<ShopProductList>(query).ToList();


            ViewBag.ShopProductList = vData;
            Session["Account"] = 0;
            return View();
        }
        public ActionResult SingleProduct(int? id)
        {
            ProductViewModel ObjReq = new ProductViewModel();
            List<SingleProductDetail> vData = null;
            string query = "SELECT TP.product_id,HC.HandicraftCategory,HSC.HandicraftSubCategory,TP.product_name,TP.product_description,TP.price_defence,TP.price_shopkeeper,TP.moq_defence,TP.moq_shopkeeper,TP.discount FROM TR_Product TP JOIN MS_HandicraftCategory HC ON(TP.HandicraftCategoryId = HC.HandicraftCategoryId) JOIN dbo.MS_HandicraftSubCategory HSC ON(TP.HandicraftSubCategoryId = HSC.HandicraftSubCategoryId)  WHERE TP.product_id = " + id+"";
            vData = _context.Database.SqlQuery<SingleProductDetail>(query).ToList();
            ViewBag.SingleProduct = vData;
            List<SignleProductImage> vImg = null;
            string ImgQuery = "SELECT ImgId, ImgUrl FROM dbo.TR_ProductImages WHERE ImageType =1 AND ProductId=" + id+"";
            vImg = _context.Database.SqlQuery<SignleProductImage>(ImgQuery).ToList();
            ViewBag.SingleProductimge = vImg;
            Session["Account"] = 0;
            return View();
        }
        
        [HttpPost]
        public ActionResult addtocart(int Productid, int Quantity,string OrderType)
        {

            CartViewModel model = new CartViewModel();
            Session["Account"] = 0;
            model.UserId= Session["UserEmail"].ToString();
            model.ProductId = Productid;
            model.Quantity = Quantity;
                model.Entrydate = DateTime.Now;
                model.EntryUser = Session["UserEmail"].ToString();
            model.OrderType = OrderType;
                //auto map viewmodel to class
                Mapper.CreateMap<CartViewModel, TR_Cart>();
                var cart = Mapper.Map<CartViewModel, TR_Cart>(model);

                //save category data in db
               _Cart.Add(cart);
                var statusid = _Cart.Save();

                if (statusid > 0)
                {
                    ViewData["Message"] = "successfuly added to cart!";
                    return RedirectToAction("SingleProduct/"+model.ProductId, "ShopOwner");
                }
               
                else
                    ViewData["Message"] = "Error Occured, Please try again.";
                
            
            Session["Account"] = 0;
            return View();
        }
        //
        [HttpPost]
        public ActionResult PlaceOrder(int orderid)
        {
            
            Session["Account"] = 0;           
           string UserId = Session["UserEmail"].ToString();
            
               var vData = _context.Database.SqlQuery<PlaceOrder>("Exec dbo.Pro_PlaceOrder @UserId",
                new SqlParameter("@UserId", UserId)

           ).ToList();

                if (vData != null)
                {
                    return RedirectToAction("OrderList", "ShopOwner");
                }
                else
                {
                    return RedirectToAction("Cart", "ShopOwner");
                }
         
        }

        [HttpPost]
        public ActionResult updatetocart(int orderid, int Quantity)
        {

            CartViewModel model = new CartViewModel();
            Session["Account"] = 0;
            //Finding row to be updated on specific Id
            var ProfileDetails = _Cart.Find(M => M.Id == orderid).FirstOrDefault();
            ProfileDetails.UserId = Session["UserEmail"].ToString();
            ProfileDetails.Id = orderid;
            ProfileDetails.Quantity = Quantity;
            ProfileDetails.Entrydate = DateTime.Now;
            ProfileDetails.EntryUser = Session["UserEmail"].ToString();



            _Cart.Update(ProfileDetails);
            var statusid = _Cart.Save();

            if (statusid > 0)
            {
                ViewData["Message"] = "successfuly added to cart!";
                return RedirectToAction("SingleProduct/" + model.ProductId, "ShopOwner");
            }

            else
                ViewData["Message"] = "Error Occured, Please try again.";


            Session["Account"] = 0;
            return View();
        }
        //
        public ActionResult Contact()
        {
            Session["Account"] = 0;
            return View();
        }
        public ActionResult dailyoffer()
        {
            
            Session["Account"] = 0;
            return View();
        }
        [HttpGet]
        public ActionResult Cart()
        {
            List<CartListViewModel> vData = null;
            string Query = "SELECT Tc.Id, TC.ProductId,TP.product_name AS ProductName,(SELECT TOP 1 ImgUrl FROM TR_ProductImages WHERE ProductId = TC.ProductId) AS ImgUrl,  TC.Quantity * (TP.price_shopkeeper + TP.discount) AS SubPrice, TC.Quantity* TP.discount AS Discount,TC.Quantity* TP.price_shopkeeper AS Price ,TC.Quantity FROM dbo.TR_Cart TC JOIN dbo.TR_Product TP ON(TC.ProductId = TP.product_id)  WHERE TC.UserId = '" + Session["UserEmail"].ToString() + "'";
            vData = _context.Database.SqlQuery<CartListViewModel>(Query).ToList();
            ViewBag.UserCart = vData;
            Session["Account"] = 0;
            return View();

        }
        public ActionResult RemoveCartOrder(int? id)
        {
            //Finding row to be updated on specific Id
            var cartDetails = _Cart.Find(x => x.Id == id).FirstOrDefault();

            if (cartDetails != null)
            {
                _Cart.Delete(cartDetails);
                var statusid = _Cart.Save();

                if (statusid > 0)
                {

                    return RedirectToAction("Cart", "ShopOwner");
                }
                else
                {
                    return View();
                }
            }

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

        [HttpGet]
        public ActionResult StateWiseCityList(int StateId)
        {
            int intStateId = Convert.ToInt32(StateId);
            var CityVMList = new List<CityViewModel>();
            var StateWiseCity = _CityRepo.Find(m => m.StateId == intStateId).ToList();
            foreach (var city in StateWiseCity)
            {
                Mapper.CreateMap<MS_City, CityViewModel>();
                var StateWiseCityData = Mapper.Map<MS_City, CityViewModel>(city);
                CityVMList.Add(StateWiseCityData);
            }

            return Json(CityVMList, JsonRequestBehavior.AllowGet);

        }
        public SelectList StateWiseCityListType(int StateId)
        {
            int intStateId = Convert.ToInt32(StateId);
            var CityVMList = new List<CityViewModel>();
            var StateWiseCity = _CityRepo.Find(m => m.StateId == intStateId).ToList();
            foreach (var city in StateWiseCity)
            {
                Mapper.CreateMap<MS_City, CityViewModel>();
                var StateWiseCityData = Mapper.Map<MS_City, CityViewModel>(city);
                CityVMList.Add(StateWiseCityData);
            }
            var CityList = new SelectList(CityVMList, "CityId", "CityName");
            return CityList;

        }

        [HttpGet]
        public ActionResult CheckEmailId(string emailid)
        {
            var user = _Userd.Find(m => m.Email == emailid).ToList();
            int isExist = 1;
            if (user.Count() == 0)
                isExist = 0;

            return Json(isExist, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetMainMenu()
        {
            List<Menu> MenuDetails = null;
            string query = "SELECT HandicraftCategoryId,HandicraftCategory FROM MS_HandicraftCategory WHERE VisibilityStatus=1";

            MenuDetails = _context.Database.SqlQuery<Menu>(query).ToList();

            return Json(MenuDetails,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetSubMenus(int MenuId)
        {
            List<SubMenu> SubMenuDetails = null;
            string query= "SELECT HandicraftSubCategoryId,HandicraftSubCategory FROM MS_HandicraftSubCategory WHERE VisibilityStatus=1 AND  HandicraftCategoryId="+ MenuId + "";
            SubMenuDetails = _context.Database.SqlQuery<SubMenu>(query).ToList();
            return Json(SubMenuDetails, JsonRequestBehavior.AllowGet);
        }




    }
}