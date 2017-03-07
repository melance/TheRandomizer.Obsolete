using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TheRandomizer.WebApp.Startup))]
namespace TheRandomizer.WebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
