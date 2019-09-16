using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LKaifer_Blog.Startup))]
namespace LKaifer_Blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
