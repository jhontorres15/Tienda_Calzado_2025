using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Tienda_Calzado_2025.Startup))]
namespace Tienda_Calzado_2025
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
