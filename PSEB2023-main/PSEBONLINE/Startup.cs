using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PSEBONLINE.Startup))]
namespace PSEBONLINE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
