using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PracticaAlberto.Startup))]
namespace PracticaAlberto
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
