using System.Web.Mvc;

namespace BlueSpiral.Web.Areas.ShopOwner
{
    public class ShopOwnerAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ShopOwner";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ShopOwner_default",
                "ShopOwner/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}