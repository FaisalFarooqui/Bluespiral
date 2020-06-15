using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using BlueSpiral.Web.Models;
using BlueSpiral.Common.Security;
using BlueSpiral.Core.ViewModel;
using BlueSpiral.Data;
using BlueSpiral.Data.Repository;
using BlueSpiral.Common.Cryptography;
using System.Collections.Generic;
using BlueSpiral.Core.Repository;
using AutoMapper;
using System.Web.Script.Serialization;
using System;
using System.IO;
using System.Configuration;
using System.Net.Mail;

namespace BlueSpiral.Web.Controllers
{
   // [Authorize]
    public class AccountController : Controller
    {
        
        private readonly Crypto _Crypto;
        private readonly BlueSpiralDBContext _context;
        private readonly IRepository<MS_State> _StateRepo ;
        private readonly IRepository<MS_City> _CityRepo;
        private readonly IRepository<TR_Users> _Users;
        private readonly IRepository<MS_VendorRequestDetails> _VendorRequest;
        private readonly IRepository<MS_HandicraftCategory> _HandicraftCategory;
        private readonly IRepository<MS_Bank> _Bank;
       
        public AccountController()
        {
            _context = new BlueSpiralDBContext();
            _StateRepo = new Repository<MS_State>(_context);
            _CityRepo = new Repository<MS_City>(_context);
            _Users = new Repository<TR_Users>(_context);
            _Crypto = new Crypto();
            _VendorRequest = new Repository<MS_VendorRequestDetails>(_context);
            _HandicraftCategory = new Repository<MS_HandicraftCategory>(_context);
            _Bank = new Repository<MS_Bank>(_context);
           
        }


        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(UsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                string UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();
                var user = _Users.Find(m => m.Email == model.Email).ToList();
                string FileName = "";
                if (user.Count() == 0)
                {
                    if (model.ImageFile != null)
                    {
                        //Use Namespace called :  System.IO  
                         FileName = Path.GetFileNameWithoutExtension("~"+model.ImageFile.FileName);

                        //To Get File Extension  
                        string FileExtension = Path.GetExtension(model.ImageFile.FileName);

                        //Add Current Date To Attached File Name  
                        FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

                        //Get Upload path from Web.Config file AppSettings.  
                        //string UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();

                        //Its Create complete path to store in server.  
                        model.ImagePath =Server.MapPath(UploadPath) + FileName;

                        //To copy and save file into server.  
                        model.ImageFile.SaveAs(model.ImagePath);
                    }

                    model.CreatedOn = System.DateTime.Now;
                    model.UserTypeId = 4;

                    //encript password
                    model.Pswd = _Crypto.Encrypt(model.Pswd);

                    //auto map viewmodel to class
                    Mapper.CreateMap<UsersViewModel, TR_Users>();
                    var users = Mapper.Map<UsersViewModel, TR_Users>(model);

                    //save user data in db
                    _Users.Add(users);
                    var userid = _Users.Save();

                    Session["UserEmail"] = model.Email;
                    Session["PhotoUrl"] = FileName!="" ? (@"..\..\"+ UploadPath + FileName) : "";
                    Session["Name"] = model.FName;


                    // return RedirectToAction("Index", "Home");

                    return RedirectToAction("Welcome", "ShopOwner", new { area = "ShopOwner", status = true });
                }
                else
                {
                    
                }
                
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pswd = _Crypto.Encrypt(model.Password);
                var user = _Users.Find(m => m.Email == model.Email && m.Pswd== pswd).ToList();

                if (user.Count()> 0)
                {
                    int UserTypeId = user.Select(m => m.UserTypeId).FirstOrDefault();
                    Session["UserEmail"] = model.Email;
                    Session["PhotoUrl"] = user[0].ImagePath;
                    Session["Name"] = user[0].FName;
                    Session["UserTypeId"] = UserTypeId;
                    // return RedirectToAction("Index", "Home");
                    if (UserTypeId == 1)
                    {
                        return RedirectToAction("Welcome", "ControlPanel", new { area = "ControlPanel", status = true });
                    }
                    else if (UserTypeId == 3)
                    {
                        return RedirectToAction("Welcome", "ControlPanel", new { area = "ControlPanel", status = true });
                    }
                    else if (UserTypeId == 4 || UserTypeId == 5)
                    {
                        return RedirectToAction("Index2", "ShopOwner", new { area = "ShopOwner", status = true });
                    }
                   

                }
                else
                    return View(model);
            }
            return View();
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            //var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            //switch (result)
            //{
            //    case SignInStatus.Success:
            //        return RedirectToLocal(returnUrl);
            //    case SignInStatus.LockedOut:
            //        return View("Lockout");
            //    case SignInStatus.RequiresVerification:
            //        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
            //    case SignInStatus.Failure:
            //    default:
            //        ModelState.AddModelError("", "Invalid login attempt.");
            //        return View(model);
            //}
        }

      
        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.State = StateList();
            return View();
        }

        public ActionResult RegisterationConfirmation()
        {
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
            var AllBank = _Bank.Find(m=>m.bank_id>0).ToList();
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

        [HttpGet]
        public ActionResult CheckEmailId(string emailid)
        {
            var user = _Users.Find(m => m.Email == emailid).ToList();
            int isExist = 1;
            if (user.Count() == 0)
                isExist = 0;

            return Json(isExist, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult VendorApplication()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult VendorApplication(VendorApplViewModel model)
        {
            if (ModelState.IsValid)
            {
                // model.CreatedOn = System.DateTime.Now;
                model.CreatedOn = System.DateTime.Now;
                model.CreatedBy = 1;
                model.VisibilityStatus = true;
                //auto map viewmodel to class
                Mapper.CreateMap<VendorApplViewModel, MS_VendorRequestDetails>();
                var vendor = Mapper.Map<VendorApplViewModel, MS_VendorRequestDetails>(model);
               
                //save vendor application data in db
                _VendorRequest.Add(vendor);

                var statusid = _VendorRequest.Save();

                if (statusid > 0)
                {
                    SendVedorApplMail(model.Email, model.Name.ToUpper());
                    return RedirectToAction("VendorApplication", "Account");
                }
                else
                    return View(model);
            }
            return View();

        }
        public void SendVedorApplMail(string Reciever, string Name)
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
        // GET: /Account/VendorRegister
        [AllowAnonymous]
        public ActionResult VendorRegister()
        {
            ViewBag.State = StateList();
            ViewBag.HandicraftCategory = HandicraftList();
            ViewBag.BankName = BankNametList();
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VendorRegister(UsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                string UploadPath = ConfigurationManager.AppSettings["UserImagePath"].ToString();
                var user = _Users.Find(m => m.Email == model.Email).ToList();
                string FileName = "";
                if (user.Count() == 0)
                {
                    if (model.ImageFile != null)
                    {
                        //Use Namespace called :  System.IO  
                        FileName = Path.GetFileNameWithoutExtension("~" + model.ImageFile.FileName);

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

                    model.CreatedOn = System.DateTime.Now;
                    model.UserTypeId = 3;

                    //encript password
                    model.Pswd = _Crypto.Encrypt(model.Pswd);

                    //auto map viewmodel to class
                    Mapper.CreateMap<UsersViewModel, TR_Users>();
                    var users = Mapper.Map<UsersViewModel, TR_Users>(model);

                    //save user data in db
                    _Users.Add(users);
                    var userid = _Users.Save();

                    Session["UserEmail"] = model.Email;
                    Session["PhotoUrl"] = FileName != "" ? (@"..\..\" + UploadPath + FileName) : "";
                    Session["Name"] = model.FName;


                    // return RedirectToAction("Index", "Home");

                    return RedirectToAction("Welcome", "ShopOwner", new { area = "ShopOwner", status = true });
                }
                else
                {

                }

            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }
        


    }
}