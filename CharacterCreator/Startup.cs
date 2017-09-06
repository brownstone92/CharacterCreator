using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CharacterCreator.Startup))]
namespace CharacterCreator
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
