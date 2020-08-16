using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
        private readonly IRepository<MS_City> _CityRepo;
        private readonly IRepository<TR_OrderDetails> _OrderDetails;
        private readonly IRepository<SalePurchaseLedger> _SalePurchaseLedger;
        private readonly IRepository<ProductRequest> _ProductRequest;
        private readonly IRepository<MS_Bank> _Bank;
        private readonly IRepository<TR_Address> _Address;
        private readonly IRepository<MS_CashBackHistory> _CashBackHistory;
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
            _CityRepo = new Repository<MS_City>(_context);
            _OrderDetails = new Repository<TR_OrderDetails>(_context);
            _SalePurchaseLedger = new Repository<SalePurchaseLedger>(_context);
            _ProductRequest = new Repository<ProductRequest>(_context);
            _Bank = new Repository<MS_Bank>(_context);
            _Address = new Repository<TR_Address>(_context);
            _CashBackHistory = new Repository<MS_CashBackHistory>(_context);

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

                    TempData["Message"] = "Category Created Successfully";
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
                TempData["Message"] = "Category Deleted Successfully";
                return RedirectToAction("Category", "ControlPanel");
            }
            else
                return View();

        }
        public SelectList HandicraftList()
        {
            var HandicraftVMList = new List<HandicraftViewModel>();
            var AllCategory = _HandicraftCategory.Find(m => m.VisibilityStatus == true).ToList();
            foreach (var category in AllCategory)
            {
                Mapper.CreateMap<MS_HandicraftCategory, HandicraftViewModel>();
                var CategoryData = Mapper.Map<MS_HandicraftCategory, HandicraftViewModel>(category);
                HandicraftVMList.Add(CategoryData);
            }
            var StateList = new SelectList(HandicraftVMList, "HandicraftCategoryId", "HandicraftCategory");
            return StateList;
        }
        public SelectList BankNametList()
        {
            var BankVMList = new List<BankViewModel>();
            var AllBank = _Bank.Find(m => m.bank_id > 0).ToList();
            foreach (var bank in AllBank)
            {
                Mapper.CreateMap<MS_Bank, BankViewModel>();
                var BankData = Mapper.Map<MS_Bank, BankViewModel>(bank);
                BankVMList.Add(BankData);
            }
            var BankList = new SelectList(BankVMList, "bank_id", "bank_name");
            return BankList;
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


        public ActionResult Profile()
        {
            ViewBag.State = StateList();
           
            ViewBag.HandicraftCategory = HandicraftList();
            
            ViewBag.BankName = BankNametList();
            Session["Account"] = 1;
            string Email;
            if (Request.QueryString["id"] != null)
            {
                Email = Request.QueryString["id"].ToString();
            }
            else
            {
                Email = Session["UserEmail"].ToString();
            }

            //transfer Unique value from session
            //find details of specific id
            var Profile = _Users.Find(m => m.Email == Email).FirstOrDefault();
           // ViewBag.City = StateWiseCityListBackEnd(Profile.StateId);
            Mapper.CreateMap<TR_Users, UsersViewModel>();
            var ProfileData = Mapper.Map<TR_Users, UsersViewModel>(Profile);
            ViewBag.JavaScriptFunction = Profile.CityId;
            ProfileData.Pswd = null;


            return View(ProfileData);
        }

        // POST: /ShopOwner/Profile
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Profile(UsersViewModel model)
        {
            ////Finding row to be updated on specific Id
            //var ProfileDetails = _Users.Find(M => M.Email == model.Email).FirstOrDefault();

            ////Updating Visibility status
            //ProfileDetails.FName = model.FName;
            //ProfileDetails.LName = model.LName;
            //ProfileDetails.ContactNo = model.ContactNo;
            //ProfileDetails.GSTNo = model.GSTNo;
            //if (model.Pswd != null)
            //{
            //    ProfileDetails.Pswd = _Crypto.Encrypt(model.Pswd);
            //}
            //string UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();
            //string FileName = "";
            //if (model.ImageFile != null)
            //{
            //    //Use Namespace called :  System.IO  
            //    FileName = RandomNumber(1, 4).ToString();

            //    //To Get File Extension  
            //    string FileExtension = Path.GetExtension(model.ImageFile.FileName);

            //    //Add Current Date To Attached File Name  
            //    FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

            //    //Get Upload path from Web.Config file AppSettings.  
            //    var path = Path.Combine(Server.MapPath("~/Uploads"), model.ImageFile.FileName);
            //    model.ImageFile.SaveAs(path);

            //    //Its Create complete path to store in server.  
            //    model.ImagePath = "../../../Uploads/" + model.ImageFile.FileName;
            //    ProfileDetails.ImagePath = model.ImagePath;
            //}
            //_Users.Update(ProfileDetails);
            //var statusid = _Users.Save();

           // if (ModelState.IsValid)
            //{
                string UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();
                var user = _Users.Find(m => m.Email == model.Email).FirstOrDefault();
                string FileName = "";
                if (user.Email != null)
                {
                    if (model.ImageFile != null)
                    {
                        //Use Namespace called :  System.IO  
                        FileName = "VPP"; //Path.GetFileNameWithoutExtension("~" + model.ImageFile.FileName);

                        //To Get File Extension  
                        string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                        //Add Current Date To Attached File Name  
                        FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                        //Get Upload path from Web.Config file AppSettings.  
                        //string UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();

                        ////Its Create complete path to store in server.  
                        //model.ImagePath = Server.MapPath(UploadPath) + FileName;

                        ////To copy and save file into server.  
                        //model.ImageFile.SaveAs(model.ImagePath);

                        var path = Path.Combine(Server.MapPath("~/Uploads"), model.ImageFile.FileName);
                        model.ImageFile.SaveAs(path);

                        model.ImagePath = "../../../Uploads/" + model.ImageFile.FileName;
                    user.ImagePath = model.ImagePath;

                    }
                    if (model.panScanFile != null)
                    {
                        //Use Namespace called :  System.IO  
                        FileName = "VPAN";//Path.GetFileNameWithoutExtension("~" + model.panScanFile.FileName);

                        //To Get File Extension  
                        string FileExtension = Path.GetExtension(model.panScanFile.FileName);

                        //Add Current Date To Attached File Name  
                        FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                        //Get Upload path from Web.Config file AppSettings.  
                        //string UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();

                        ////Its Create complete path to store in server.  
                        //model.PanScanPath = Server.MapPath(UploadPath) + FileName;

                        ////To copy and save file into server.  
                        //model.panScanFile.SaveAs(model.PanScanPath);
                        var path = Path.Combine(Server.MapPath("~/Uploads"), model.panScanFile.FileName);
                        model.panScanFile.SaveAs(path);

                        model.PanScanPath = "../../../Uploads/" + model.panScanFile.FileName;
                    user.PanScanPath = model.PanScanPath;
                    }

                    if (model.gstScanFile != null)
                    {
                        //Use Namespace called :  System.IO  
                        FileName = "VGST";// Path.GetFileNameWithoutExtension("~" + model.gstScanFile.FileName);

                        //To Get File Extension  
                        string FileExtension = Path.GetExtension(model.gstScanFile.FileName);

                        //Add Current Date To Attached File Name  
                        FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                        //Get Upload path from Web.Config file AppSettings.  
                        //string UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();

                        ////Its Create complete path to store in server.  
                        //model.GSTScanPath = Server.MapPath(UploadPath) + FileName;

                        ////To copy and save file into server.  
                        //model.gstScanFile.SaveAs(model.GSTScanPath);

                        var path = Path.Combine(Server.MapPath("~/Uploads"), model.gstScanFile.FileName);
                        model.gstScanFile.SaveAs(path);

                        model.GSTScanPath = "../../../Uploads/" + model.gstScanFile.FileName;
                    user.GSTScanPath = model.GSTScanPath;
                    }

                    if (model.cancelledChqScanFile != null)
                    {
                        //Use Namespace called :  System.IO  
                        FileName = "CANCHQ";// Path.GetFileNameWithoutExtension("~" + model.cancelledChqScanFile.FileName);

                        //To Get File Extension  
                        string FileExtension = Path.GetExtension(model.cancelledChqScanFile.FileName);

                        //Add Current Date To Attached File Name  
                        FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                        //Get Upload path from Web.Config file AppSettings.  
                        //string UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();

                        ////Its Create complete path to store in server.  
                        //model.CanChqPath = Server.MapPath(UploadPath) + FileName;

                        ////To copy and save file into server.  
                        //model.cancelledChqScanFile.SaveAs(model.CanChqPath);
                        var path = Path.Combine(Server.MapPath("~/Uploads"), model.cancelledChqScanFile.FileName);
                        model.cancelledChqScanFile.SaveAs(path);

                        model.CanChqPath = "../../../Uploads/" + model.cancelledChqScanFile.FileName;
                    user.CanChqPath = model.CanChqPath;
                    }



                    //model.CreatedOn = System.DateTime.Now;
                  
                if (model.Pswd != null)
                {
                    //encript password
                    user.Pswd = _Crypto.Encrypt(model.Pswd);
                }
                user.FName = model.FName;
                user.ContactNo = model.ContactNo;
                user.BusinessName = model.BusinessName;
                user.ContactAddress = model.ContactAddress;
                user.StateId = model.StateId;
                user.CityId = model.CityId;
                user.Pincode = model.Pincode;
                user.HandicraftCategoryId = model.HandicraftCategoryId;
                user.BankName = model.BankName;
                user.BankAccountNumber = model.BankAccountNumber;
                user.IFSCCode = model.IFSCCode;
                user.GSTNo = model.GSTNo;
                user.PanNo = model.PanNo;
                user.ManufacturerORWholesaler = model.ManufacturerORWholesaler;
                _Users.Update(user);
                    //save category data in db
                    var statusid = _Users.Save();

                    if (statusid > 0)
                    {
                    ViewBag.State = StateList();
                    ViewBag.HandicraftCategory = HandicraftList();
                    ViewBag.BankName = BankNametList();
                    Session["PhotoUrl"] = user.ImagePath != "" ? (@"" + user.ImagePath) : "";
                        Session["Name"] = user.FName;
                    // return RedirectToAction("Profile","ControlPanel",new(id= ProfileDetails.Email));
                    if (Session["UserTypeId"].ToString() == "3")
                    {
                        return View("Profile");
                    }
                    else if (Session["UserTypeId"].ToString() == "1")
                    {
                        // Response.Redirect("http://beta.thebluespiral.com/ControlPanel/ControlPanel/Profile?id=" + user.Email);
                        //Response.Redirect(" http://localhost:55659/ControlPanel/ControlPanel/ControlPanel/Profile?id=" + user.Email);
                        //return RedirectToAction("Profile?id=" + user.Email);
                        return RedirectToAction("Userlist", new { id = user.UserTypeId });

                    }
                    return RedirectToAction("Profile");
                }
                    else
                    {
                    return RedirectToAction("Profile");
                }

                }
            //}
            return RedirectToAction("Profile");
        }


        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
        public ActionResult getprofile()
        {
            string EmailId = Request.QueryString["id"].ToString();

            var SupplierVMList = new List<SupplierDetails>();
            string query = "SELECT tu.FName,tu.Email,tu.ContactNo,tu.BusinessName,tu.ContactAddress,mc.CityName,ms.StateName,tu.Pincode,tu.REGIMENT,tu.PanNo,tu.PanScanPath,tu.GSTNo,tu.GSTScanPath,tu.CanChqPath,tu.ImagePath,tut.Users FROM TR_Users tu JOIN DBO.MS_City mc ON(tu.CityId = mc.CityId) JOIN dbo.MS_State ms ON(tu.StateId = ms.StateId) JOIN dbo.TR_UserType tut ON(tu.UserTypeId = tut.UserTypeId) WHERE tu.Email = '" + EmailId + "' and  tu.VisibilityStatus=1";

            var vData = _context.Database.SqlQuery<SupplierDetails>(query).ToList();
            ViewBag.SupplierList = vData;


            return View();
        }


        public ActionResult UserList(int? id)
        {
            var SupplierVMList = new List<SupplierList>();
            var AllSupplier = _Users.Find(m => m.VisibilityStatus == true && m.UserTypeId == id).ToList();
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

        [HttpPost]
        public ActionResult UserList(UsersViewModel model)
        {
            //  if (ModelState.IsValid)
            //   {
            string UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();
            // var user = _Users.Find(m => m.Email == model.Email).ToList();
            string FileName = "";

            model.UserTypeId = 5;
            model.CreatedOn = DateTime.Now;
            model.CreatedBy = 1;
            model.Pswd = _Crypto.Encrypt(model.Pswd);
            if (model.ImageFile != null)
            {
                //Use Namespace called :  System.IO  
                FileName = "DP";// Path.GetFileNameWithoutExtension("~" + model.ImageFile.FileName);

                //To Get File Extension  
                string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                //Add Current Date To Attached File Name  
                FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;
                var path = Path.Combine(Server.MapPath("~/Uploads"), model.ImageFile.FileName);
                model.ImageFile.SaveAs(path);

                model.ImagePath = "../../../Uploads/" + model.ImageFile.FileName;


            }
            //auto map viewmodel to class
            Mapper.CreateMap<UsersViewModel, TR_Users>();
            var user = Mapper.Map<UsersViewModel, TR_Users>(model);

            user.VisibilityStatus = true;

            //save category data in db
            _Users.Add(user);
            var userid = _Users.Save();

            if (userid > 0)
            {
                TR_Address ObjTRA = new TR_Address();
                ObjTRA.EmailId = model.Email;
                ObjTRA.BusinessName = model.BusinessName;
                ObjTRA.StateId = model.StateId;
                ObjTRA.CityId = model.CityId;
                ObjTRA.PinCode = model.Pincode;
                ObjTRA.ContactAddress = model.ContactAddress;
                _Address.Add(ObjTRA);
                var Addressstatusid = _Address.Save();

                TempData["Message"] = "User Added Successfully";
                return RedirectToAction("UserList/5", "ControlPanel");
            }
            else
                return View(model);
            // }

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
                SendShopOwnerApplMail(userDetails.Email, userDetails.FName.ToUpper());
            }
            _Users.Update(userDetails);
            TempData["Message"] = "Status Changed Successfully.";

            //save category data in db
            var statusid = _Users.Save();

            if (statusid > 0)
            {

                return RedirectToAction("UserList/" + Usertypeid, "ControlPanel");
            }
            else
                return View();

        }
        public void SendShopOwnerApplMail(string Reciever, string Name)
        {
            System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
            msg.From = new MailAddress("support@flyaway.co.in");
            msg.To.Add(Reciever);
            msg.Subject = "Vendor Application : The Blue Spriral";
            string htmlimagedesign = "Dear " + Name + ",<br/><br/> THE BLUE SPIRAL WELCOMES YOU <br/><br/>";
            htmlimagedesign += "Our Customer Repesentative will contact you soon.<br/>";
            htmlimagedesign += "Thanks for the intrest ;<br/><br/>";
            htmlimagedesign += "Regards,<br/>THE BLUE SPIRAL TEAM.";

            msg.Body = htmlimagedesign;
            msg.IsBodyHtml = true;

            msg.BodyEncoding = System.Text.Encoding.UTF8;
            msg.SubjectEncoding = System.Text.Encoding.UTF8;
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();

            // client.Port = 21;
            client.Host = "webmail.flyaway.co.in";
            System.Net.NetworkCredential nc = new System.Net.NetworkCredential();
            nc.UserName = "support@flyaway.co.in"; //Your Email ID  
            nc.Password = "9pphJ8*6"; // Your Password  
            client.EnableSsl = false;
            client.UseDefaultCredentials = false;
            client.Credentials = nc;
            client.Send(msg);
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
                TempData["Message"] = "User Removed Successfully.";
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
        public SelectList SupplierList()
        {
            List<SuppliersList> vData = null;
            string query = "SELECT Email,FName + '( '+ BusinessName+' )' AS UserInfo FROM TR_Users WHERE UserTypeId=3 AND VisibilityStatus=1";

            vData = _context.Database.SqlQuery<SuppliersList>(query).ToList();
            var StateList = new SelectList(vData, "Email", "UserInfo");
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
            var CategroyWiseSubCategroy = _HandicraftSubCategory.Find(m => m.HandicraftCategoryId == intCategoryId && m.VisibilityStatus == true).ToList();
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
                string UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();
                // var user = _Users.Find(m => m.Email == model.Email).ToList();
                string FileName = "";
                if (model.ImageFile != null)
                {
                    //Use Namespace called :  System.IO  
                    FileName = "SC";// Path.GetFileNameWithoutExtension("~" + model.ImageFile.FileName);

                    //To Get File Extension  
                    string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                    //Add Current Date To Attached File Name  
                    FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;
                    var path = Path.Combine(Server.MapPath("~/Uploads"), model.ImageFile.FileName);
                    model.ImageFile.SaveAs(path);

                    model.ImgUrl = "../../../Uploads/" + model.ImageFile.FileName;



                    //auto map viewmodel to class
                    Mapper.CreateMap<SubCategoryViewModel, MS_HandicraftSubCategory>();
                    var subcategories = Mapper.Map<SubCategoryViewModel, MS_HandicraftSubCategory>(model);
                    subcategories.VisibilityStatus = true;
                    subcategories.EntryDate = DateTime.Now;

                    //save category data in db
                    _HandicraftSubCategory.Add(subcategories);
                    var subcategoryid = _HandicraftSubCategory.Save();

                    if (subcategoryid > 0)
                    {


                        TempData["Message"] = "Subcategory created Successfully.";
                        return RedirectToAction("SubCategory", "ControlPanel");
                    }
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
                TempData["Message"] = "Subcategory Removed Successfully.";
                return RedirectToAction("SubCategory/", "ControlPanel");
            }
            else
                return View();

        }
        [HttpGet]
        public ActionResult EditProduct(int? Id)
        {
            Product ObjReq = new Product();

            ViewBag.Category = CategoryList();
            ViewBag.Colour = ColourList();
            //ObjReq.PRL = _context.TR_Product.Where(x => x.product_id == Id).Select(x => new ProductRepList { product_name = x.product_name, product_description = x.product_description, product_size = x.product_size, price_defence = x.price_defence, price_shopkeeper = x.price_shopkeeper, product_stock = x.product_stock, moq_defence = x.moq_defence, moq_shopkeeper = x.moq_shopkeeper, HandicraftCategoryId = x.HandicraftCategoryId, HandicraftSubCategoryId = x.HandicraftSubCategoryId, discount = x.discount }).ToList();
            var SubVMList = new List<ProductRepList>();
            var AllProduct = _Product.Find(m => m.product_id == Id).ToList();
            foreach (var Product in AllProduct)
            {
                Mapper.CreateMap<TR_Product, ProductRepList>();
                var ProductData = Mapper.Map<TR_Product, ProductRepList>(Product);
                SubVMList.Add(ProductData);
            }

            return View(SubVMList);

        }
        [HttpGet]
        public ActionResult Product()
        {
            Product ObjReq = new Product();


            List<ProductList> vData = null;
            string query;


            //query for admin login

            if (Session["UserTypeId"].ToString() == "3")
            {
                //query for vendor login
                query = "SELECT product_id,EntryUser,Material,PR.SamplePrice,HC.HandicraftCategory,HSC.HandicraftSubCategory,product_name,product_description,isnull((SELECT TOP 1 ImgUrl FROM dbo.TR_ProductImages WHERE ProductId = TP.product_id AND ImageType = 'WaterMark' ORDER BY ImgId DESC ),'../../../Uploads/na.jpg') AS ImgUrl, PR.MOQ AS moq_shopkeeper,PR.Price as price_shopkeeper FROM TR_Product TP JOIN dbo.ProductRequest PR ON(TP.product_id = PR.ProductId) JOIN MS_HandicraftCategory HC ON(TP.HandicraftCategoryId = HC.HandicraftCategoryId) JOIN dbo.MS_HandicraftSubCategory HSC ON(TP.HandicraftSubCategoryId = HSC.HandicraftSubCategoryId) JOIN dbo.TR_Users TU ON(TP.VendorId = TU.Email) WHERE TP.VendorId = '" + Session["UserEmail"].ToString() + "'";
            }
            else
            {
                query = "SELECT product_id,EntryUser,Material,PR.SamplePrice,HC.HandicraftCategory,HSC.HandicraftSubCategory,product_name,product_description,isnull((SELECT TOP 1 ImgUrl FROM dbo.TR_ProductImages WHERE ProductId = TP.product_id AND ImageType = 'WaterMark' ORDER BY ImgId DESC ),'../../../Uploads/na.jpg') AS ImgUrl, PR.MOQ AS moq_shopkeeper,PR.Price as price_shopkeeper , TU.FName + '( ' + TU.BusinessName + ' )' AS UserInfo FROM TR_Product TP JOIN dbo.ProductRequest PR ON(TP.product_id = PR.ProductId) JOIN MS_HandicraftCategory HC ON(TP.HandicraftCategoryId = HC.HandicraftCategoryId) JOIN dbo.MS_HandicraftSubCategory HSC ON(TP.HandicraftSubCategoryId = HSC.HandicraftSubCategoryId) JOIN dbo.TR_Users TU ON(TP.VendorId = TU.Email) WHERE 1 = 1 ";
                if (Request.QueryString["id"] != null)
                {

                    query += " and TP.VendorId = '" + Request.QueryString["id"].ToString() + "'";
                }
            }
            vData = _context.Database.SqlQuery<ProductList>(query).ToList();


            ViewBag.ProductList = vData;
            ViewBag.Category = CategoryList();
            ViewBag.Colour = ColourList();
            ViewBag.Supplier = SupplierList();
            return View();

        }
        [HttpPost]
        public ActionResult Product(Product obj)
        {
            ViewBag.Category = CategoryList();
            ViewBag.Colour = ColourList();
            ViewBag.Supplier = SupplierList();
            if (Session["UserTypeId"].ToString() == "3")
            {
                obj.discount = 0;

            }
            if (ModelState.IsValid)
            {
                if (Session["UserTypeId"].ToString() == "3")
                {
                    obj.VendorId = Session["UserEmail"].ToString();

                }
                obj.EntryUser = Session["UserEmail"].ToString();
                if (obj.product_id != null)
                {
                    var ProductDetails = _Product.Find(x => x.product_id == obj.product_id).FirstOrDefault();

                    if (ProductDetails != null)
                    {

                        if (Session["UserTypeId"].ToString() == "1")
                        {
                            ProductDetails.discount = obj.discount;
                            ProductDetails.moq_defence = obj.moq_defence;
                            ProductDetails.price_defence = obj.price_defence;
                            ProductDetails.moq_shopkeeper = obj.moq_shopkeeper;
                            ProductDetails.price_shopkeeper = obj.price_shopkeeper;
                            if (obj.TimeDuration != null)
                            {
                                ProductDetails.DailyOfferSignal = 1;
                                ProductDetails.TimeDuration = DateTime.Now.Subtract(DateTime.Parse(obj.TimeDuration)).Hours.ToString();
                            }
                            else
                            {
                                ProductDetails.DailyOfferSignal = 0;
                                ProductDetails.TimeDuration = "0";

                            }
                        }
                        ProductDetails.EntryDate = DateTime.Now;
                        ProductDetails.Material = obj.Material;
                        ProductDetails.product_stock = obj.product_stock;
                        _Product.Update(ProductDetails);
                    }
                }
                else
                {
                    //auto map viewmodel to class
                    Mapper.CreateMap<Product, TR_Product>();
                    var product = Mapper.Map<Product, TR_Product>(obj);
                    product.EntryDate = DateTime.Now;
                    if (obj.TimeDuration != null)
                    {
                        product.DailyOfferSignal = 1;
                        product.TimeDuration = DateTime.Now.Subtract(DateTime.Parse(obj.TimeDuration)).Hours.ToString();
                    }
                    else
                    {
                        product.DailyOfferSignal = 0;
                        product.TimeDuration = "0";

                    }
                    //save category data in db
                    _Product.Add(product);

                    var statusid = _Product.Save();



                    if (statusid > 0)
                    {
                        SalePurchaseLedger SalesDetails = new SalePurchaseLedger();

                        SalesDetails.TransactionType = "Purchase";
                        SalesDetails.Quantity = obj.product_stock;
                        SalesDetails.Entrydate = DateTime.Now;
                        SalesDetails.EntryUser = obj.VendorId;
                        SalesDetails.OrderNo = "N/A";
                        SalesDetails.ProductId = _context.TR_Product.Max(x => x.product_id);
                        SalesDetails.VisibilityStatus = true;
                        _SalePurchaseLedger.Add(SalesDetails);
                        var statusidd = _SalePurchaseLedger.Save();
                        if (statusidd > 0)
                        {
                            ProductRequest ObjPRoreq = new ProductRequest();
                            ObjPRoreq.ProductId = _context.TR_Product.Max(x => x.product_id);
                            ObjPRoreq.SamplePrice = obj.SamplePrice;
                            ObjPRoreq.Price = obj.price_shopkeeper;
                            ObjPRoreq.MOQ = obj.moq_shopkeeper;
                            ObjPRoreq.VendorId = obj.VendorId;
                            ObjPRoreq.DateCreated = DateTime.Now;
                            _ProductRequest.Add(ObjPRoreq);
                            var ProReqstatusid = _ProductRequest.Save();
                            if (ProReqstatusid > 0)
                            {
                                TempData["Message"] = "Product Successfully Updated.";
                            }

                        }
                        return RedirectToAction("Product", "ControlPanel");
                    }
                    else
                        return View(obj);
                }
            }
            return View();
        }
        public ActionResult CallProduct(int? id)
        {
            //onclick="document.getElementById('id01').style.display = 'block';"
            ViewBag.JavaScriptFunction = "Go";
            ViewBag.Category = CategoryList();
            ViewBag.Colour = ColourList();
            Product ObjReq = new Product();
            var Product = _Product.Find(m => m.product_id == id).FirstOrDefault();
            //Updating Visibility status
            //if (Product != null)
            //{
            //    ObjReq.product_name = Product.product_name;
            //    ObjReq.product_description = Product.product_description;
            //    ObjReq.product_size = Product.product_size;
            //    ObjReq.price_defence = Product.price_defence;
            //    //ObjReq.product_colour = Product.product_colour;
            //    ObjReq.price_shopkeeper = Product.price_shopkeeper;
            //    ObjReq.product_stock = Product.product_stock;
            //    ObjReq.moq_defence = Product.moq_defence;
            //    ObjReq.moq_shopkeeper = Product.moq_shopkeeper;

            //    ObjReq.HandicraftCategoryId = Product.HandicraftCategoryId;
            //    ObjReq.HandicraftSubCategoryId = Product.HandicraftSubCategoryId;
            //    ObjReq.product_status = Product.product_status;
            //    ObjReq.discount = Product.discount;
            //    ObjReq.VendorId = Product.VendorId;
            //}
            return View("Product", Product);
        }
        //[HttpPost]
        //public ActionResult Product(Product obj) {
        //    if (ModelState.IsValid)
        //    {
        //        obj.VendorId = Session["UserEmail"].ToString();
        //        obj.EntryUser= Session["UserEmail"].ToString();

        //        //auto map viewmodel to class
        //        Mapper.CreateMap<Product, TR_Product>();
        //        var product = Mapper.Map<Product, TR_Product>(obj);

        //        //save category data in db
        //        _Product.Add(product);
        //        var statusid = _Product.Save();

        //        if (statusid > 0)
        //        {
        //            return RedirectToAction("Product", "ControlPanel");
        //        }
        //        else
        //            return View(obj);
        //    }
        //    return View();
        //}
        public ActionResult productSave(string HandicraftCategory)
        {
            string hccategory = HandicraftCategory;
            CategoryViewModel obj = new CategoryViewModel();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditProduct(Product obj)
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
        public ActionResult UploadFiles(int? id)
        {
            Session["Type"] = Request.QueryString["Type"].ToString();
            string Query = "SELECT ImgUrl,ImgId,ProductId FROM TR_ProductImages WHERE ProductId = '" + id + "' AND ImageType = '" + Session["Type"].ToString() + "' AND isdeleted = 0";
            List<ProductImages> vData = _context.Database.SqlQuery<ProductImages>(Query).ToList();
            ViewBag.ImageList = vData;
            return View();
        }

        public ActionResult RemoveImage(int id, string redurl)
        {
            var ProductImages = _ProductImages.Find(o => o.isdeleted == false && o.ImgId == id).FirstOrDefault();
            ProductImages.isdeleted = true;
            _ProductImages.Update(ProductImages);

            //save category data in db
            var RemoveImage = _ProductImages.Save();
            if (RemoveImage > 0)
            {
                Response.Redirect("Uploadfiles" + redurl);
                return RedirectToAction("Product");
            }
            return View();
        }
        [HttpPost]
        public ActionResult UploadFiles(int? id, List<HttpPostedFileBase> Files)
        {
            //int id=Convert.ToInt32(Request.QueryString["id"].ToString());

            var path = "";
            string FileName = "";
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

                                //Use Namespace called :  System.IO  
                                FileName = "PI" + id.ToString();// Path.GetFileNameWithoutExtension("~"+model.ImageFile.FileName);

                                //To Get File Extension  
                                string FileExtension = Path.GetExtension(item.FileName);

                                //Add Current Date To Attached File Name  
                                FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;
                                path = Path.Combine(Server.MapPath("~/Uploads"), item.FileName);
                                item.SaveAs(path);

                                ProductImageViewModel PIVM = new ProductImageViewModel();
                                PIVM.ProductId = id;
                                PIVM.ImageType = Session["Type"].ToString();
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
                                    TempData["msg"] = "Upload Completed Successfully.";
                                }
                            }
                        }
                    }
                }
            }
            var url = id + "?Type=" + Session["Type"].ToString();
            Response.Redirect("http://beta.thebluespiral.com/ControlPanel/ControlPanel/UploadFiles/" + url);
            //Response.Redirect("http://localhost:55659/ControlPanel/ControlPanel/UploadFiles/" + url);
            return View();
        }
        public ActionResult OrderList(int? id)
        {
            string OrderType = "";
            if (id == 1)
            {
                OrderType = "Sample";
            }
            else
            {
                OrderType = "Normal";
            }
            List<OrderListViewModel> vData = null;
            string Query = "SELECT TRO.OrdeNo,TRO.EntryUser,convert(CHAR,TRO.EntryDate,106) AS DateOfPurchase,TRO.OrderStatus,TRO.TotalAmount FROM TR_OrderDetails TRO JOIN TR_Order T ON(TRO.OrdeNo = T.OrderNo)  WHERE T.OrderType ='" + OrderType + "' AND TRO.OrderStatus!='Reject'";
            if (Session["UserTypeId"].ToString() != "1")
            {
                Query += " and T.VendorId='" + Session["UserEmail"].ToString() + "'";
            }
            vData = _context.Database.SqlQuery<OrderListViewModel>(Query).ToList();
            ViewBag.OrderList = vData;
            Session["Account"] = 1;
            return View();
        }
        [HttpPost]
        public ActionResult UpdateOrderStatus(string orderid, string orderstatus, string returnurl)
        {
            //Finding row to be updated on specific Id
            var OrderDetails = _OrderDetails.Find(x => x.OrdeNo == orderid).FirstOrDefault();

            if (OrderDetails != null)
            {
                //Updating Visibility status
                OrderDetails.OrderStatus = orderstatus;
                _OrderDetails.Update(OrderDetails);
                ////Remove order from SalePurchaseLedger
                if (orderstatus == "Reject")
                {
                    var SalePurchaseLedger = _SalePurchaseLedger.Find(o => o.OrderNo == orderid).FirstOrDefault();
                    SalePurchaseLedger.VisibilityStatus = false;
                    _SalePurchaseLedger.Update(SalePurchaseLedger);

                    //save category data in db
                    var RemovePurchase = _SalePurchaseLedger.Save();
                }

                //save category data in db
                var statusid = _OrderDetails.Save();

                if (statusid > 0)
                {
                    TempData["Message"] = "Status Changed Successfully";
                    return RedirectToAction("OrderList/" + returnurl, "ControlPanel");
                }
                else
                {
                    return View();
                }
            }

            return View();

        }
        public ActionResult helpcenter()
        {

            Session["Account"] = 0;
            return View();
        }
        public ActionResult Contact()
        {

            Session["Account"] = 0;
            return View();
        }
        //GetProductdetails
        [HttpPost]
        public JsonResult GetProductdetails(int productid)
        {
            List<Product> vData = null;
            string Query = "SELECT product_id,Material,SamplePrice,product_name, product_description, product_size, price_defence, price_shopkeeper,moq_defence, moq_shopkeeper, HandicraftCategoryId, HandicraftSubCategoryId, product_status, discount_type, discount,max_discount, EntryUser, VendorId,product_stock FROM TR_Product WHERE product_id = " + productid + "";
            vData = _context.Database.SqlQuery<Product>(Query).ToList();
            return Json(vData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CashBack()
        {

            Session["Account"] = 0;
            string Query = "SELECT row_number() OVER(ORDER BY EmailId) AS SrNo, convert(CHAR,FromDate,103) AS FromDate,convert(CHAR, ToDate,103) AS ToDate, EmailId, isnull(TotalPurchase,0) AS TotalPurchase, isnull(CashBackAmount,0) AS CashBackAmount,convert(CHAR, EntryDate,103) AS EntryDate FROM dbo.MS_CashBackHistory order by ToDate";
            List<CashBackList> vData = _context.Database.SqlQuery<CashBackList>(Query).ToList();
            ViewBag.CashBack = vData;
            ViewBag.GetDate = "1";
            return View(); 
        }
        [HttpPost]
        public ActionResult CashBack(CashBackViewModel model)
        {

            if (ModelState.IsValid)
            {
                string UserId = Session["UserEmail"].ToString();

                var vData = _context.Database.SqlQuery<PlaceOrder>("Exec dbo.Pro_GenerateCashBack @FromDate,@ToDate",
                 new SqlParameter("@FromDate", model.FromDate),
                 new SqlParameter("@ToDate", model.ToDate)

               ).ToList();

                if (vData != null)
                {
                    TempData["Message"] = vData[0].message;
                    return RedirectToAction("CashBack", "ControlPanel");
                }
                else
                {
                    return RedirectToAction("CashBack", "ControlPanel");
                }
            }
            return View();
        }
        [HttpPost]
        public JsonResult LastToDate()
        {
            List<LastScheduledDate> LastDate = null;
            string query = "SELECT convert(CHAR,max(ToDate),101) AS FromDate FROM MS_CashbackSchedule";

            LastDate = _context.Database.SqlQuery<LastScheduledDate>(query).ToList();

            return Json(LastDate, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreditWallet()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreditWallet(WalletViewModel model)
        {
            if (ModelState.IsValid)
            {
                

                var vData = _context.Database.SqlQuery<PlaceOrder>("Exec dbo.Pro_CreditWallet @FromDate,@ToDate,@EmailId,@TotalPurchase,@CashbacAmount",
                 new SqlParameter("@FromDate", model.FromDate),
                 new SqlParameter("@ToDate", model.ToDate),
                 new SqlParameter("@EmailId", model.UserId),
                 new SqlParameter("@TotalPurchase", model.TotalPurchase),
                 new SqlParameter("@CashbacAmount", model.CashBackAmount)


               ).ToList();

                if (vData != null)
                {
                    TempData["Message"] = vData[0].message;
                    return RedirectToAction("CreditWallet", "ControlPanel");
                }
                else
                {
                    return RedirectToAction("CreditWallet", "ControlPanel");
                }
            }
            return View();
        }

    }
}
