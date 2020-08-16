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
        private readonly IRepository<TR_Address> _Address;

        public ShopOwnerController()
        {
            _context = new BlueSpiralDBContext();
            _Crypto = new Crypto();

            _StateRepo = new Repository<MS_State>(_context);
            _CityRepo = new Repository<MS_City>(_context);
            _HandicraftCategory = new Repository<MS_HandicraftCategory>(_context);
            _Userd = new Repository<TR_Users>(_context);
            _Cart = new Repository<TR_Cart>(_context);
            _Address = new Repository<TR_Address>(_context);
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
            List<TopSubCategoryViewModel> vData = null;
            List<TopProductViewModel> vProductData = null;
            string Query = "SELECT HandicraftSubCategory,HandicraftSubCategoryId,isnull(ImgUrl,'../../../Uploads/na.jpg') AS ImgUrl FROM MS_HandicraftSubCategory WHERE VisibilityStatus=1 ORDER by EntryDate DESC";
            vData = _context.Database.SqlQuery<TopSubCategoryViewModel>(Query).ToList();
            ViewBag.TopSubCategory = vData;
            string Query2 = "SELECT TR.product_id,TR.moq_defence,TR.moq_shopkeeper,TR.product_name,isnull(TR.price_defence,0) AS price_defence,isnull(TR.price_shopkeeper,0) AS price_shopkeeper,isnull(TR.discount,0) AS discount,TR.price_shopkeeper+ TR.discount AS TotAmountShopkeeper,TR.price_defence+ TR.discount AS TotDefence,isnull((SELECT TOP 1 ImgUrl FROM TR_ProductImages WHERE ProductId = TR.product_id),'../../../Uploads/na.jpg') AS ImgUrl FROM TR_Product TR  ORDER BY TR.EntryDate desc";
            vProductData = _context.Database.SqlQuery<TopProductViewModel>(Query2).ToList();
            ViewBag.TopProduct = vProductData;
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
                FileName = RandomNumber(1, 4).ToString();

                //To Get File Extension  
                string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                //Add Current Date To Attached File Name  
                FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                //Get Upload path from Web.Config file AppSettings.  
                var path = Path.Combine(Server.MapPath("~/Uploads"), model.ImageFile.FileName);
                model.ImageFile.SaveAs(path);

                //Its Create complete path to store in server.  
                model.ImagePath = "../../../Uploads/" + model.ImageFile.FileName;
                ProfileDetails.ImagePath = model.ImagePath;



            }
            _Userd.Update(ProfileDetails);

            //save category data in db
            var statusid = _Userd.Save();

            if (statusid > 0)
            {
                Session["PhotoUrl"] = ProfileDetails.ImagePath != "" ? (@"" + ProfileDetails.ImagePath) : "";
                Session["Name"] = ProfileDetails.FName;
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
        public ActionResult Address(int? id)
        {
            if (Request.QueryString["revert"] != null)
            {
                Session["reverturl"] = Request.QueryString["revert"].ToString();
            }
            Session["Account"] = 1;
            ViewBag.State = StateList();
            
            //transfer Unique value from session
            string Email = Session["UserEmail"].ToString();
           
            List<AddressList> vData = null;
            string query = "SELECT TA.Id,TA.EmailId,TA.BusinessName,MS.StateName,MC.CityName,TA.PinCode,TA.ContactAddress FROM TR_Address TA JOIN dbo.MS_State MS ON(TA.StateId = MS.StateId) JOIN dbo.MS_City MC ON(TA.CityId = MC.CityId)  WHERE VisibiltyStatus = 1  AND TA.EmailId = '" + Email + "'";
            vData = _context.Database.SqlQuery<AddressList>(query).ToList();

            if (vData.Count > 0)
            {
                ViewBag.AddressList = vData;
            }
            //find details of specific id
            if (id != null)
            {
                var Profile = _Address.Find(m => m.Id == id).FirstOrDefault();

                //ViewBag.City = StateWiseCityListType(Profile.StateId);
                ViewBag.JavaScriptFunction = Profile.CityId;

                Mapper.CreateMap<TR_Address, UsersViewModel>();


                var ProfileData = Mapper.Map<TR_Address, UsersViewModel>(Profile);
                return View(ProfileData);
            }
           

            return View();
        }
        // POST: /ShopOwner/Profile
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Address(UsersViewModel model,int? id)
        {
            string Email = Session["UserEmail"].ToString();
            if (id != null )
            {
                

                //Finding row to be updated on specific Id
                var ProfileDetails = _Address.Find(m => m.Id == id).FirstOrDefault();
                //Updating Visibility status
                ProfileDetails.BusinessName = model.BusinessName;
                ProfileDetails.StateId = model.StateId;
                ProfileDetails.CityId = model.CityId;
                ProfileDetails.PinCode = model.Pincode;
                ProfileDetails.ContactAddress = model.ContactAddress;
                ProfileDetails.VisibiltyStatus = true;

                _Address.Update(ProfileDetails);

                //save category data in db
                var statusid = _Address.Save();

                if (statusid > 0)
                {
                    TempData["msg"] = "Address Updated Successfully.";
                  
                  
                        if(Session["reverturl"] != null)
                        {
                       
                        string url = Session["reverturl"].ToString();
                        Session["reverturl"] = null;
                        return RedirectToAction(url, "ShopOwner");
                    }
                    return RedirectToAction("Address", "ShopOwner");
                }
                else
                    return View();
            }
            else
            {
                TR_Address ObjTRA = new TR_Address();
                ObjTRA.EmailId = Email;
                ObjTRA.BusinessName = model.BusinessName;
                ObjTRA.StateId = model.StateId;
                ObjTRA.CityId = model.CityId;
                ObjTRA.PinCode = model.Pincode;
                ObjTRA.ContactAddress = model.ContactAddress;
                ObjTRA.VisibiltyStatus = true;
                _Address.Add(ObjTRA);
                var Addressstatusid = _Address.Save();
                if (Addressstatusid > 0)
                {
                    if (Session["reverturl"] != null)
                    {
                        TempData["msg"] = "Address Added Successfully.";
                        string url = Session["reverturl"].ToString();
                        Session["reverturl"] = null;
                        return RedirectToAction(url, "ShopOwner");
                    }
                    else
                    {
                        TempData["msg"] = "Address Added Successfully.";
                        return RedirectToAction("Address", "ShopOwner");
                    }
                }
                else
                    return View();
            }
        }
        public ActionResult RemoveAdddress(int? id)
        {
            //Finding row to be updated on specific Id
            var AddressDetails = _Address.Find(x => x.Id == id).FirstOrDefault();

            if (AddressDetails != null)
            {
                _Address.Delete(AddressDetails);
                var statusid = _Address.Save();

                if (statusid > 0)
                {
                    TempData["msg"] = "Address Removed Successfully.";
                    if (Request.QueryString["revert"] != null)
                    {
                        return RedirectToAction(Request.QueryString["revert"].ToString(), "ShopOwner");

                    }
                    return RedirectToAction("Address", "ShopOwner");
                }
                else
                {
                    return View();
                }
            }

            return View();

        }
        public ActionResult OrderList()
        {
            List<OrderListViewModel> vData = null;
            string Query = "SELECT OrdeNo,convert(CHAR,EntryDate,106) AS DateOfPurchase,OrderStatus,TotalAmount FROM TR_OrderDetails WHERE EntryUser='"+Session["UserEmail"].ToString()+ "'";
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
            string query = "SELECT TR.product_id,TR.moq_defence,TR.moq_shopkeeper,TR.product_name,isnull(TR.price_defence,0) AS price_defence,isnull(TR.price_shopkeeper,0) AS price_shopkeeper,isnull(TR.discount,0) AS discount,TR.price_shopkeeper+ TR.discount AS TotAmountShopkeeper,TR.price_defence+ TR.discount AS TotDefence,isnull((SELECT TOP 1 ImgUrl FROM TR_ProductImages WHERE ProductId = TR.product_id),'../../../Uploads/na.jpg') AS ImgUrl FROM TR_Product TR   WHERE 1=1";
            if (id != -1)
            {
              query+=" AND  HandicraftSubCategoryId = " + id + "";
                    }
            vData = _context.Database.SqlQuery<ShopProductList>(query).ToList();

            if (vData.Count > 0)
            {
                ViewBag.ShopProductList = vData;
            }
            Session["Account"] = 0;
            return View();
        }
        public ActionResult SingleProduct(int? id)
        {
            //string Query = "SELECT OrderNo,UserId,ProductId,OrderType FROM TR_Order WHERE OrderType = 'Sample' AND UserId = "+Session["UserEmail"].ToString()+" AND ProductId = " + id+"";

            Product ObjReq = new Product();
            List<SingleProductDetail> vData = null;
            string query = "SELECT TP.product_id,isnull(TP.Material,0) AS Material,isnull(TP.SamplePrice,0) AS SamplePrice, isnull(TP.product_size,0) AS product_size,HC.HandicraftCategory,HSC.HandicraftSubCategory,TP.product_name,isnull(TP.product_description, 0) as product_description,isnull(TP.price_defence, 0) as price_defence,isnull(TP.price_shopkeeper, 0) as price_shopkeeper,isnull(TP.moq_defence, 0) as moq_defence,isnull(TP.moq_shopkeeper, 0) as moq_shopkeeper,isnull(TP.discount, 0) as discount,TP.price_shopkeeper + TP.discount AS TotAmountShopkeeper, TP.price_defence + TP.discount AS TotDefence,(SELECT count(1) FROM TR_Order WHERE OrderType = 'Sample' AND UserId = '" + Session["UserEmail"].ToString() + "' AND ProductId = " + id + ") AS SampleStatus FROM TR_Product TP JOIN MS_HandicraftCategory HC ON(TP.HandicraftCategoryId = HC.HandicraftCategoryId) JOIN dbo.MS_HandicraftSubCategory HSC ON(TP.HandicraftSubCategoryId = HSC.HandicraftSubCategoryId)  WHERE TP.product_id = " + id + "";
            vData = _context.Database.SqlQuery<SingleProductDetail>(query).ToList();
            ViewBag.SingleProduct = vData;
            List<SignleProductImage> vImg = null;
            string ImgQuery = "SELECT ImgId, isnull(ImgUrl,'../../../Uploads/na.jpg') AS ImgUrl FROM dbo.TR_ProductImages WHERE ImageType ='Original' AND ProductId=" + id + "";
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
        public ActionResult PlaceOrder(int orderid,string Status,string CreditStatus,int deliveryid)
        {
            
            Session["Account"] = 0;           
           string UserId = Session["UserEmail"].ToString();

            var vData = _context.Database.SqlQuery<PlaceOrder>("Exec dbo.Pro_PlaceOrder @UserId,@Status,@CreditStatus,@deliveryid",
             new SqlParameter("@UserId", UserId),
             new SqlParameter("@Status", Status),
             new SqlParameter("@CreditStatus", CreditStatus),
             new SqlParameter("@deliveryid", deliveryid)

           ).ToList();

                if (vData != null)
                {
                
                TempData["msg"] = "Order Placed Successfully.";
                return Json(vData, JsonRequestBehavior.AllowGet);
            }
                else
                {
                return Json(vData, JsonRequestBehavior.AllowGet);
            }
         
        }
        [HttpPost]
        public ActionResult PlaceSampleOrder( int quantity, int productid)
        {

            Session["Account"] = 0;
            string UserId = Session["UserEmail"].ToString();

            var vData = _context.Database.SqlQuery<PlaceOrder>("Exec dbo.Pro_PlaceSampleOrder @Quantity,@ProductId,@UserId",
               new SqlParameter("@Quantity", quantity),
                new SqlParameter("@ProductId", productid),
                new SqlParameter("@UserId", UserId)

        ).ToList();

            if (vData != null)
            {
                TempData["msg"] = "Sample Order Placed Successfully.";
                return RedirectToAction("OrderList", "ShopOwner");
            }
            else
            {
                return RedirectToAction("Shop/"+ productid, "ShopOwner");
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
            string Query = "SELECT DISTINCT p.product_id,p.product_name,convert(CHAR,p.EntryDate,113) AS EntryDate,p.TimeDuration,p.price_defence,p.price_shopkeeper,p.price_shopkeeper+p.discount AS totshopkeeper,p.price_defence+p.discount AS totdefence,p.moq_defence,p.moq_shopkeeper,p.discount,isnull((SELECT TOP 1 ImgUrl FROM TR_ProductImages WHERE ProductId = p.product_id),'../../../Uploads/na.jpg') AS ImgUrl FROM TR_Product AS p JOIN TR_ProductImages AS pi ON (p.product_id = pi.ProductId) WHERE DailyOfferSignal = 1 ORDER BY discount desc";
            List<DailyOfferViewModel> vData = _context.Database.SqlQuery<DailyOfferViewModel>(Query).ToList();
            ViewBag.dailyoffer = vData;
            return View();
        }
        public ActionResult latestproduct()
        {
            List<TopSubCategoryViewModel> vData = null;
            List<TopProductViewModel> vProductData = null;
            string Query = "SELECT HandicraftSubCategory,HandicraftSubCategoryId,isnull(ImgUrl,'../../../Uploads/na.jpg') AS ImgUrl FROM MS_HandicraftSubCategory WHERE VisibilityStatus=1 ORDER by EntryDate DESC";
            vData = _context.Database.SqlQuery<TopSubCategoryViewModel>(Query).ToList();
            ViewBag.TopSubCategory = vData;
            string Query2 = "SELECT TR.product_id,TR.moq_defence,TR.moq_shopkeeper,TR.product_name,isnull(TR.price_defence,0) AS price_defence,isnull(TR.price_shopkeeper,0) AS price_shopkeeper,isnull(TR.discount,0) AS discount,isnull((SELECT TOP 1 ImgUrl FROM TR_ProductImages WHERE ProductId = TR.product_id),'../../../Uploads/na.jpg') AS ImgUrl FROM TR_Product TR  ORDER BY TR.EntryDate desc";
            vProductData = _context.Database.SqlQuery<TopProductViewModel>(Query2).ToList();
            ViewBag.TopProduct = vProductData;
            Session["Account"] = 0;
            return View();
        }
        public ActionResult helpcenter()
        {

            Session["Account"] = 0;
            return View();
        }
        [HttpGet]
        public ActionResult Cart()
        {
            List<CartListViewModel> vData = null;
            string Query = "DECLARE @TotalAmount DECIMAL(16,2)=( SELECT isnull(sum(TC.Quantity * TP.price_shopkeeper),0)  FROM dbo.TR_Cart TC  JOIN dbo.TR_Product TP ON(TC.ProductId = TP.product_id)  WHERE TC.UserId = '" + Session["UserEmail"].ToString()+ "');            SELECT Tc.Id, TC.ProductId,TP.product_name AS ProductName,TP.moq_defence,TP.moq_shopkeeper, isnull((SELECT TOP 1 ImgUrl FROM TR_ProductImages WHERE ProductId = TC.ProductId),'../../../Uploads/na.jpg') AS ImgUrl,TC.Quantity * (TP.price_shopkeeper + isnull(TP.discount, 0)) AS SubPrice, TC.Quantity* isnull(TP.discount,0) AS Discount,  TC.Quantity* TP.price_shopkeeper AS Price ,TC.Quantity ,@TotalAmount AS TotalAmount  FROM dbo.TR_Cart TC  JOIN dbo.TR_Product TP ON(TC.ProductId = TP.product_id)  WHERE TC.UserId = '" + Session["UserEmail"].ToString()+"'";
            vData = _context.Database.SqlQuery<CartListViewModel>(Query).ToList();
            ViewBag.UserCart = vData;
            if (vData.Count > 0)
            {
                TempData["TotalAmount"] = vData[0].TotalAmount;
            }
            else
            {
                TempData["TotalAmount"] = "0";
            }
            decimal PackagingCharge = Convert.ToDecimal(TempData["TotalAmount"]) * Convert.ToDecimal(0.05);
            TempData["PackagingCharge"] = PackagingCharge;
            TempData["ServiceCharge"] = "0";
            decimal GrandTotalCharge = Convert.ToDecimal(TempData["TotalAmount"]) + Convert.ToDecimal(TempData["PackagingCharge"])+ Convert.ToDecimal(TempData["ServiceCharge"]);
            TempData["GrandTotalCharge"] = GrandTotalCharge;
            string Email = Session["UserEmail"].ToString();

            List<AddressList> vAddData = null;
            string query = "SELECT TA.Id,TA.EmailId,TA.BusinessName,MS.StateName,MC.CityName,TA.PinCode,TA.ContactAddress FROM TR_Address TA JOIN dbo.MS_State MS ON(TA.StateId = MS.StateId) JOIN dbo.MS_City MC ON(TA.CityId = MC.CityId)  WHERE VisibiltyStatus = 1  AND TA.EmailId = '" + Email + "'";
            vAddData = _context.Database.SqlQuery<AddressList>(query).ToList();

            if (vData.Count > 0)
            {
                ViewBag.AddressList = vAddData;
            }
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
                    TempData["msg"] = "Cart Order Removed Successfully.";
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
        public SelectList StateWiseCityListType(int? StateId)
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
        [HttpPost]
        public JsonResult GetOrderDetails(string OrdeNo)
        {
            List<OrderDetail> OrderDetails = null;
            string query = "SELECT isnull((SELECT TOP 1 ImgUrl FROM TR_ProductImages WHERE ProductId=TRO.ProductId),'../../../Uploads/na.jpg') AS ImgUrl,TRP.product_name AS ProductName,TRO.Quantity,TRO.OrderType,TRO.FinalAmount as Price,TRO.VendorId FROM TR_Order AS TRO JOIN dbo.TR_Product TRP ON(TRO.ProductId = TRP.product_id)  WHERE TRO.OrderNo='" + OrdeNo + "'  ORDER BY  TRO.DateofPurchase";
            OrderDetails = _context.Database.SqlQuery<OrderDetail>(query).ToList();
            return Json(OrderDetails, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetTopCategories()
        {
            List<TopSubCategoryViewModel> vData = null;
            string Query = "SELECT HandicraftSubCategory,HandicraftSubCategoryId,isnull(ImgUrl,'../../../Uploads/na.jpg') AS ImgUrl FROM MS_HandicraftSubCategory WHERE VisibilityStatus=1 ORDER by EntryDate DESC";
            vData = _context.Database.SqlQuery<TopSubCategoryViewModel>(Query).ToList();
            return Json(vData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetTopSaleCategories()
        {
            List<TopSubCategoryViewModel> vData = null;
            string Query = "SELECT HandicraftSubCategory,HandicraftSubCategoryId,isnull(ImgUrl,'../../../Uploads/na.jpg') AS ImgUrl FROM dbo.Fun_GetMaxsaleCategories()";
            vData = _context.Database.SqlQuery<TopSubCategoryViewModel>(Query).ToList();
            return Json(vData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ValidateWallet()
        {
            string UserId = Session["UserEmail"].ToString();
            List<WalletAmount> vData = null;
            string Query = "SELECT isnull(WalletAmount,0) AS Amount FROM TR_Users WHERE Email='" + UserId + "'";
            vData = _context.Database.SqlQuery<WalletAmount>(Query).ToList();
            return Json(vData, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult CartDetails()
        {
            string UserId = Session["UserEmail"].ToString();
            List<CartListViewModel> vData = null;
            string Query = "DECLARE @TotalItem DECIMAL(16,2)=( SELECT count(*) FROM dbo.TR_Cart TC    WHERE TC.UserId = '" + Session["UserEmail"].ToString() + "');     SELECT Tc.Id, TC.ProductId,TP.product_name AS ProductName,TP.moq_defence,TP.moq_shopkeeper, isnull((SELECT TOP 1 ImgUrl FROM TR_ProductImages WHERE ProductId = TC.ProductId),'../../../Uploads/na.jpg') AS ImgUrl, TC.Quantity * (TP.price_shopkeeper + isnull(TP.discount, 0)) AS SubPrice, TC.Quantity* isnull(TP.discount,0) AS Discount,   TC.Quantity* TP.price_shopkeeper AS Price ,TC.Quantity ,@TotalItem AS TotalAmount FROM dbo.TR_Cart TC  JOIN dbo.TR_Product TP ON(TC.ProductId = TP.product_id)  WHERE TC.UserId = '" + Session["UserEmail"].ToString()+ "'";
            vData = _context.Database.SqlQuery<CartListViewModel>(Query).ToList();
            return Json(vData, JsonRequestBehavior.AllowGet);
        }
        



    }
}