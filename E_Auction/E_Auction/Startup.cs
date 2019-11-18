using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(E_Auction.Startup))]
namespace E_Auction
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
