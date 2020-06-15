using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BlueSpiral.Web.Startup))]
namespace BlueSpiral.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
          //  ConfigureAuth(app);
        }
    }
}
