using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Wonderly_Blog.Startup))]
namespace Wonderly_Blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
